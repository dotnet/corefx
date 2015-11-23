// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"
#include "pal_console.h"
#include "pal_utilities.h"

#include <assert.h>
#include <errno.h>
#include <pthread.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <termios.h>
#include <unistd.h>

extern "C" int32_t GetWindowSize(WinSize* windowSize)
{
    assert(windowSize != nullptr);

#if HAVE_IOCTL && HAVE_TIOCGWINSZ
    int error = ioctl(STDOUT_FILENO, TIOCGWINSZ, windowSize);

    if (error != 0)
    {
        *windowSize = {}; // managed out param must be initialized
    }

    return error;
#else
    errno = ENOTSUP;
    return -1;
#endif
}

extern "C" int32_t IsATty(int fd)
{
    return isatty(fd);
}

extern "C" int32_t ReadStdinUnbuffered(void* buffer, int32_t bufferSize)
{
    assert(buffer != nullptr || bufferSize == 0);
    assert(bufferSize >= 0);

    if (bufferSize < 0)
    {
        errno = EINVAL;
        return -1;
    }

#if HAVE_TCGETATTR && HAVE_TCSETATTR && HAVE_ECHO && HAVE_ICANON && HAVE_TCSANOW
    struct termios oldtermios = {};
    struct termios newtermios = {};

    if (tcgetattr(STDIN_FILENO, &oldtermios) < 0)
        return -1;

    newtermios = oldtermios;
    newtermios.c_lflag &= static_cast<uint32_t>(~(ECHO | ICANON));
    newtermios.c_cc[VMIN] = 1;
    newtermios.c_cc[VTIME] = 0;
    if (tcsetattr(STDIN_FILENO, TCSANOW, &newtermios) < 0)
        return -1;
    ssize_t count = read(STDIN_FILENO, buffer, UnsignedCast(bufferSize));
    tcsetattr(STDIN_FILENO, TCSANOW, &oldtermios);
    return static_cast<int32_t>(count);
#else
    errno = ENOTSUP;
    return -1;
#endif
}

static volatile CtrlCallback g_ctrlCallback = nullptr; // Callback invoked for SIGINT/SIGQUIT
static bool g_signalHandlingInitialized = false; // Whether signal handling is initialized
static int g_signalPipe[2] = {-1, -1}; // Pipe used between signal handler and worker

// Signal handler for SIGINT / SIGQUIT.
static void sigintquit_handler(int code)
{
    // Write the signal code to the pipe
    uint8_t signalCodeByte = static_cast<uint8_t>(code);
    ssize_t writtenBytes;
    do
    {
        writtenBytes = write(g_signalPipe[1], &signalCodeByte, 1);
    } while ((writtenBytes == -1) && (errno == EINTR));

    if (writtenBytes != 1)
    {
        abort(); // fatal error
    }
}

// Ctrl-handling worker thread entry point.
void* CtrlHandleLoop(void* arg)
{
    // Passed in argument is a ptr to the file descriptor
    // for the read end of the pipe.
    assert(arg != nullptr);
    int pipeFd = *reinterpret_cast<int*>(arg);
    free(arg);
    assert(pipeFd >= 0);

    // Continually read a signal code from the signal pipe and process it,
    // until the pipe is closed.
    while (true)
    {
        // Read the next signal, trying again if we were interrupted
        uint8_t signalCode;
        ssize_t bytesRead;
        do
        {
            bytesRead = read(pipeFd, &signalCode, 1);
        } while (bytesRead == -1 && errno == EINTR);

        if (bytesRead <= 0)
        {
            // Write end of pipe was closed or another error occurred.
            // Regardless, no more data is available, so we close the read
            // end of the pipe and exit.
            close(pipeFd);
            return nullptr;
        }

        // Invoke the callback.  We take the lock while calling it so as to prevent
        // it from being removed while we're still using it.
        CtrlCallback callback = g_ctrlCallback;
        int rv = callback != nullptr ? callback(signalCode == SIGQUIT ? Break : Interrupt) : 0;
        if (rv == 0) // callback removed or was invoked and didn't handle the signal
        {
            // The proper behavior for unhandled SIGINT/SIGQUIT is to set the signal
            // handler to the default and then send the signal to self and let the
            // default handler do its work.
            struct sigaction resetAction = {.sa_handler = SIG_DFL, .sa_flags = 0};
            sigemptyset(&resetAction.sa_mask);

            sigaction(SIGINT, &resetAction, nullptr);
            sigaction(SIGQUIT, &resetAction, nullptr);

            kill(getpid(), signalCode);
        }
    }
}

static void CloseSignalHandlingPipe()
{
    assert(g_signalPipe[0] >= 0);
    assert(g_signalPipe[1] >= 0);
    close(g_signalPipe[0]);
    close(g_signalPipe[1]);
    g_signalPipe[0] = -1;
    g_signalPipe[1] = -1;
}

static bool InitializeSignalHandling()
{
    assert(!g_signalHandlingInitialized);

    // Create a pipe we'll use to communicate with our worker
    // thread.  We can't do anything interesting in the signal handler,
    // so we instead send a message to another thread that'll do
    // the handling work.
    if (pipe(g_signalPipe) != 0)
    {
        return false;
    }
    assert(g_signalPipe[0] >= 0);
    assert(g_signalPipe[1] >= 0);

    // Create a small object to pass the read end of the pipe to the worker.
    int* readFdPtr = reinterpret_cast<int*>(malloc(sizeof(int)));
    if (readFdPtr == nullptr)
    {
        CloseSignalHandlingPipe();
        errno = ENOMEM;
        return false;
    }
    *readFdPtr = g_signalPipe[0];

    // The pipe is created.  Create the worker thread.
    pthread_t handlerThread;
    if (pthread_create(&handlerThread, nullptr, CtrlHandleLoop, readFdPtr) != 0)
    {
        int err = errno;
        free(readFdPtr);
        CloseSignalHandlingPipe();
        errno = err;
        return false;
    }

    // Finally, register the signal handlers for SIGINT and SIGQUIT.
    struct sigaction newAction = {
        .sa_flags = SA_RESTART, .sa_handler = &sigintquit_handler,
    };
    sigemptyset(&newAction.sa_mask);

    int rv = sigaction(SIGINT, &newAction, nullptr);
    assert(rv == 0);
    rv = sigaction(SIGQUIT, &newAction, nullptr);
    assert(rv == 0);

    g_signalHandlingInitialized = true;
    return true;
}

extern "C" int32_t RegisterForCtrl(CtrlCallback callback)
{
    assert(callback != nullptr);
    assert(g_ctrlCallback == nullptr);

    g_ctrlCallback = callback;

    if (!g_signalHandlingInitialized && !InitializeSignalHandling())
    {
        g_ctrlCallback = nullptr;
        return 0;
    }

    return 1;
}

extern "C" void UnregisterForCtrl()
{
    assert(g_ctrlCallback != nullptr);
    g_ctrlCallback = nullptr;

    // Keep the signal handlers registered and the worker thread
    // up and running for when registration is done again.
}
