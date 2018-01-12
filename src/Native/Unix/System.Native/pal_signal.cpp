// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_config.h"
#include "pal_console.h"
#include "pal_signal.h"
#include "pal_io.h"
#include "pal_utilities.h"

#include <assert.h>
#include <errno.h>
#include <pthread.h>
#include <signal.h>
#include <stdlib.h>
#include <unistd.h>

static struct sigaction g_origSigIntHandler, g_origSigQuitHandler; // saved signal handlers for ctrl handling
static struct sigaction g_origSigContHandler, g_origSigChldHandler; // saved signal handlers for reinitialization
static volatile CtrlCallback g_ctrlCallback = nullptr; // Callback invoked for SIGINT/SIGQUIT
static int g_signalPipe[2] = {-1, -1}; // Pipe used between signal handler and worker

static void HandleSignalForReinitialize(int sig, siginfo_t* siginfo, void* context)
{
    // SIGCONT will be sent when we're resumed after suspension, at which point
    // we need to set the terminal back up.  Similarly, SIGCHLD will be sent after
    // a child process completes, and that child could have left things in a bad state,
    // so we similarly need to reinitialize.
    assert(sig == SIGCONT || sig == SIGCHLD);

    ReinitializeConsole();

    // Delegate to any saved handler we may have
    struct sigaction origHandler = sig == SIGCONT ? g_origSigContHandler : g_origSigChldHandler;
    if (origHandler.sa_sigaction != nullptr &&
        reinterpret_cast<void*>(origHandler.sa_sigaction) != reinterpret_cast<void*>(SIG_DFL) &&
        reinterpret_cast<void*>(origHandler.sa_sigaction) != reinterpret_cast<void*>(SIG_IGN))
    {
        origHandler.sa_sigaction(sig, siginfo, context);
    }
}

// Signal handler for signals where we want our background thread to do the real processing.
// It simply writes the signal code to a pipe that's read by the thread.
static void TransferSignalToHandlerLoop(int sig, siginfo_t* siginfo, void* context)
{
    (void)siginfo; // unused
    (void)context; // unused

    // Write the signal code to the pipe
    uint8_t signalCodeByte = static_cast<uint8_t>(sig);
    ssize_t writtenBytes;
    while (CheckInterrupted(writtenBytes = write(g_signalPipe[1], &signalCodeByte, 1)));

    if (writtenBytes != 1)
    {
        abort(); // fatal error
    }
}

// Entrypoint for the thread that handles signals where our handling
// isn't signal-safe.  Those signal handlers write the signal to a pipe,
// which this loop reads and processes.
void* SignalHandlerLoop(void* arg)
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
        while (CheckInterrupted(bytesRead = read(pipeFd, &signalCode, 1)));

        if (bytesRead <= 0)
        {
            // Write end of pipe was closed or another error occurred.
            // Regardless, no more data is available, so we close the read
            // end of the pipe and exit.
            close(pipeFd);
            return nullptr;
        }

        assert_msg(signalCode == SIGQUIT || signalCode == SIGINT, "invalid signalCode", static_cast<int>(signalCode));

        // We're now handling SIGQUIT and SIGINT. Invoke the callback, if we have one.
        CtrlCallback callback = g_ctrlCallback;
        int rv = callback != nullptr ? callback(signalCode == SIGQUIT ? Break : Interrupt) : 0;
        if (rv == 0) // callback removed or was invoked and didn't handle the signal
        {
            // In general, we now want to remove our handler and reissue the signal to
            // be picked up by the previously registered handler.  In the most common case,
            // this will be the default handler, causing the process to be torn down.
            // It could also be a custom handle registered by other code before us.

            if (signalCode == SIGINT)
            {
                UninitializeConsole();
                sigaction(SIGINT, &g_origSigIntHandler, NULL);
                kill(getpid(), SIGINT);
            } 
            else if (signalCode == SIGQUIT)
            {
                UninitializeConsole();
                sigaction(SIGQUIT, &g_origSigQuitHandler, NULL);
                kill(getpid(), SIGQUIT);
            }
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

extern "C" void SystemNative_RegisterForCtrl(CtrlCallback callback)
{
    assert(callback != nullptr);
    assert(g_ctrlCallback == nullptr);
    g_ctrlCallback = callback;
}

extern "C" void SystemNative_UnregisterForCtrl()
{
    assert(g_ctrlCallback != nullptr);
    g_ctrlCallback = nullptr;
}

static bool InitializeSignalHandling()
{
    // Create a pipe we'll use to communicate with our worker
    // thread.  We can't do anything interesting in the signal handler,
    // so we instead send a message to another thread that'll do
    // the handling work.
    if (SystemNative_Pipe(g_signalPipe, PAL_O_CLOEXEC) != 0)
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
    if (pthread_create(&handlerThread, nullptr, SignalHandlerLoop, readFdPtr) != 0)
    {
        int err = errno;
        free(readFdPtr);
        CloseSignalHandlingPipe();
        errno = err;
        return false;
    }

    // Finally, register our signal handlers
    struct sigaction newAction;
    memset(&newAction, 0, sizeof(struct sigaction));
    newAction.sa_flags = SA_RESTART | SA_SIGINFO;
    
    sigemptyset(&newAction.sa_mask);
    int rv;

    // Hook up signal handlers for use with ctrl-C / ctrl-Break handling
    // We don't handle ignored signals. If we'd setup a handler, our child processes
    // would reset to the default on exec causing them to terminate on these signals.
    newAction.sa_sigaction = &TransferSignalToHandlerLoop;
    rv = sigaction(SIGINT, NULL, &g_origSigIntHandler);
    assert(rv == 0);
    if (reinterpret_cast<void*>(g_origSigIntHandler.sa_sigaction) != reinterpret_cast<void*>(SIG_IGN))
    {
        rv = sigaction(SIGINT, &newAction, NULL);
        assert(rv == 0);
    }
    rv = sigaction(SIGQUIT, NULL, &g_origSigQuitHandler);
    assert(rv == 0);
    if (reinterpret_cast<void*>(g_origSigQuitHandler.sa_sigaction) != reinterpret_cast<void*>(SIG_IGN))
    {
        rv = sigaction(SIGQUIT, &newAction, NULL);
        assert(rv == 0);
    }

    // Hook up signal handlers for use with signals that require us to reinitialize the terminal
    newAction.sa_sigaction = &HandleSignalForReinitialize;
    rv = sigaction(SIGCONT, &newAction, &g_origSigContHandler);
    assert(rv == 0);
    rv = sigaction(SIGCHLD, &newAction, &g_origSigChldHandler);
    assert(rv == 0);

    return true;
}

extern "C" int32_t SystemNative_InitializeSignalHandling()
{
    return InitializeSignalHandling() ? 1 : 0;
}
