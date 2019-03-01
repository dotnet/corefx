// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_config.h"
#include "pal_console.h"
#include "pal_utilities.h"
#include "pal_signal.h"

#include <assert.h>
#include <errno.h>
#include <fcntl.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/ioctl.h>
#include <termios.h>
#include <unistd.h>
#include <poll.h>
#include <pthread.h>
#include <signal.h>

int32_t SystemNative_GetWindowSize(WinSize* windowSize)
{
    assert(windowSize != NULL);

#if HAVE_IOCTL && HAVE_TIOCGWINSZ
    int error = ioctl(STDOUT_FILENO, TIOCGWINSZ, windowSize);

    if (error != 0)
    {
        memset(windowSize, 0, sizeof(WinSize)); // managed out param must be initialized
    }

    return error;
#else
    errno = ENOTSUP;
    return -1;
#endif
}

int32_t SystemNative_IsATty(intptr_t fd)
{
    return isatty(ToFileDescriptor(fd));
}

static char* g_keypadXmit = NULL; // string used to enable application mode, from terminfo

static void WriteKeypadXmit()
{
    // If a terminfo "application mode" keypad_xmit string has been supplied,
    // write it out to the terminal to enter the mode.
    if (g_keypadXmit != NULL)
    {
        ssize_t ret;
        while (CheckInterrupted(ret = write(STDOUT_FILENO, g_keypadXmit, (size_t)(sizeof(char) * strlen(g_keypadXmit)))));
        assert(ret >= 0); // failure to change the mode should not prevent app from continuing
    }
}

void SystemNative_SetKeypadXmit(const char* terminfoString)
{
    assert(terminfoString != NULL);

    if (g_keypadXmit != NULL) // should only happen if initialization initially failed
    {
        free(g_keypadXmit);
        assert(false && "g_keypadXmit was already initialized");
    }

    // Store the string to use to enter application mode, then enter
    g_keypadXmit = strdup(terminfoString);
    WriteKeypadXmit();
}

static bool g_signalForBreak = true;          // tracks whether the terminal should send signals for breaks, such that attributes have been changed
static bool g_haveInitTermios = false;        // whether g_initTermios has been initialized
static bool g_hasInteractiveChildren = false; // tracks whether the application has interactive child processes.
static struct termios g_initTermios;          // the initial attributes captured when Console was initialized
static bool g_consoleUninitialized = false;   // tracks whether the application is terminating
static pthread_mutex_t g_lock = PTHREAD_MUTEX_INITIALIZER;
static volatile bool g_receivedSigTtou = false;
static bool g_noTty = false;                  // cache we are not a tty

static void ApplyTerminalSettings(struct termios* termios, bool signalForBreak, bool hasInteractiveChildren)
{
    if (signalForBreak)
        termios->c_lflag |= (uint32_t)ISIG;
    else
        termios->c_lflag &= (uint32_t)(~ISIG);

    if (hasInteractiveChildren)
    {
        assert(g_haveInitTermios);
        termios->c_iflag = g_initTermios.c_iflag;
        termios->c_lflag = g_initTermios.c_lflag;
    }
    else
    {
        termios->c_iflag &= (uint32_t)(~(IXON | IXOFF));
        termios->c_lflag &= (uint32_t)(~(ECHO | ICANON | IEXTEN));
    }
}

static bool TcGetAttr(struct termios* termios, bool signalForBreak, bool hasInteractiveChildren)
{
    if (g_noTty)
    {
        errno = ENOTTY;
        return false;
    }

    bool rv = tcgetattr(STDIN_FILENO, termios) >= 0;

    if (!rv && errno == ENOTTY)
    {
        g_noTty = true;
    }

    if (rv)
    {
        ApplyTerminalSettings(termios, signalForBreak, hasInteractiveChildren);
    }
    return rv;
}

static void ttou_handler(int signo)
{
    (void)signo;
    g_receivedSigTtou = true;
}

static void InstallTTOUHandler(void (*handler)(int))
{
    struct sigaction action;
    memset(&action, 0, sizeof(action));
    action.sa_handler = handler;
    int rvSigaction = sigaction(SIGTTOU, &action, NULL);
    assert(rvSigaction == 0);
    (void)rvSigaction;
}

static bool TcSetAttr(struct termios* t, bool blockIfBackground)
{
    if (g_noTty)
    {
        errno = ENOTTY;
        return false;
    }

    if (g_consoleUninitialized)
    {
        // The application is exiting, we mustn't change terminal settings.
        return true;
    }

    if (!blockIfBackground)
    {
        // When the process is running in background, changing terminal settings
        // will stop it (default SIGTTOU action).
        // We change SIGTTOU to to get EINTR instead of blocking.
        InstallTTOUHandler(ttou_handler);

        g_receivedSigTtou = false;
    }

    bool rv = tcsetattr(STDIN_FILENO, TCSANOW, t) >= 0;

    if (!blockIfBackground)
    {
        if (!rv && errno == EINTR && g_receivedSigTtou)
        {
            // Operation failed because we are background
            // pretend it went fine.
            rv = true;
        }

        // Restore default SIGTTOU handler.
        InstallTTOUHandler(SIG_DFL);
    }

    return rv;
}

void UninitializeConsole()
{
    // This method is called on SIGQUIT/SIGINT from the signal dispatching thread
    // and on atexit.

    if (pthread_mutex_lock(&g_lock) == 0)
    {
        if (!g_consoleUninitialized)
        {
            if (g_haveInitTermios)
            {
                TcSetAttr(&g_initTermios, /* blockIfBackground */ false);
            }

            g_consoleUninitialized = true;
        }

        pthread_mutex_unlock(&g_lock);
    }
}

void SystemNative_ConfigureConsoleTimeout(uint8_t minChars, uint8_t decisecondsTimeout)
{
    if (pthread_mutex_lock(&g_lock) == 0)
    {
        struct termios termios;
        if (TcGetAttr(&termios, g_signalForBreak, g_hasInteractiveChildren))
        {
            termios.c_cc[VMIN] = minChars;
            termios.c_cc[VTIME] = decisecondsTimeout;

            TcSetAttr(&termios, /* blockIfBackground */ true);
        }
        pthread_mutex_unlock(&g_lock);
    }
}

void SystemNative_ConfigureConsoleForInteractiveChild(int32_t hasInteractiveChildren)
{
    assert(hasInteractiveChildren == 0 || hasInteractiveChildren == 1);

    if (pthread_mutex_lock(&g_lock) == 0)
    {
        if (g_hasInteractiveChildren != hasInteractiveChildren)
        {
            struct termios termios;
            if (TcGetAttr(&termios, g_signalForBreak, hasInteractiveChildren))
            {
                TcSetAttr(&termios, /* blockIfBackground */ false);
            }
            g_hasInteractiveChildren = hasInteractiveChildren;

            // Redo "Application mode" when there are no more interactive children.
            if (!hasInteractiveChildren)
            {
                WriteKeypadXmit();
            }
        }

        pthread_mutex_unlock(&g_lock);
    }
}

static int TranslatePalControlCharacterName(int name)
{
    switch (name)
    {
#ifdef VINTR
        case PAL_VINTR: return VINTR;
#endif
#ifdef VQUIT
        case PAL_VQUIT: return VQUIT;
#endif
#ifdef VERASE
        case PAL_VERASE: return VERASE;
#endif
#ifdef VKILL
        case PAL_VKILL: return VKILL;
#endif
#ifdef VEOF
        case PAL_VEOF: return VEOF;
#endif
#ifdef VTIME
        case PAL_VTIME: return VTIME;
#endif
#ifdef VMIN
        case PAL_VMIN: return VMIN;
#endif
#ifdef VSWTC
        case PAL_VSWTC: return VSWTC;
#endif
#ifdef VSTART
        case PAL_VSTART: return VSTART;
#endif
#ifdef VSTOP
        case PAL_VSTOP: return VSTOP;
#endif
#ifdef VSUSP
        case PAL_VSUSP: return VSUSP;
#endif
#ifdef VEOL
        case PAL_VEOL: return VEOL;
#endif
#ifdef VREPRINT
        case PAL_VREPRINT: return VREPRINT;
#endif
#ifdef VDISCARD
        case PAL_VDISCARD: return VDISCARD;
#endif
#ifdef VWERASE
        case PAL_VWERASE: return VWERASE;
#endif
#ifdef VLNEXT
        case PAL_VLNEXT: return VLNEXT;
#endif
#ifdef VEOL2
        case PAL_VEOL2: return VEOL2;
#endif
        default: return -1;
    }
}

void SystemNative_GetControlCharacters(
    int32_t* controlCharacterNames, uint8_t* controlCharacterValues, int32_t controlCharacterLength,
    uint8_t* posixDisableValue)
{
    assert(controlCharacterNames != NULL);
    assert(controlCharacterValues != NULL);
    assert(controlCharacterLength >= 0);
    assert(posixDisableValue != NULL);

#ifdef _POSIX_VDISABLE
    *posixDisableValue = _POSIX_VDISABLE;
#else
    *posixDisableValue = 0;
#endif

    memset(controlCharacterValues, *posixDisableValue, sizeof(uint8_t) * Int32ToSizeT(controlCharacterLength));

    if (controlCharacterLength > 0)
    {
        struct termios newTermios;
        memset(&newTermios, 0, sizeof(struct termios));

        if (tcgetattr(STDIN_FILENO, &newTermios) >= 0)
        {
            for (int i = 0; i < controlCharacterLength; i++)
            {
                int name = TranslatePalControlCharacterName(controlCharacterNames[i]);
                if (name >= 0)
                {
                    controlCharacterValues[i] = newTermios.c_cc[name];
                }
            }
        }
    }
}

int32_t SystemNative_StdinReady()
{
    // TODO ??: https://github.com/dotnet/corefx/pull/6619
    struct pollfd fd = { .fd = STDIN_FILENO, .events = POLLIN };
    int rv = poll(&fd, 1, 0) > 0 ? 1 : 0;
    return rv;
}

int32_t SystemNative_ReadStdin(void* buffer, int32_t bufferSize)
{
    assert(buffer != NULL || bufferSize == 0);
    assert(bufferSize >= 0);

     if (bufferSize < 0)
    {
        errno = EINVAL;
        return -1;
    }

    ssize_t count;
    while (CheckInterrupted(count = read(STDIN_FILENO, buffer, Int32ToSizeT(bufferSize))));
    return (int32_t)count;
}

int32_t SystemNative_GetSignalForBreak()
{
    return g_signalForBreak;
}

int32_t SystemNative_SetSignalForBreak(int32_t signalForBreak)
{
    assert(signalForBreak == 0 || signalForBreak == 1);

    int rv = 0;

    if (pthread_mutex_lock(&g_lock) == 0)
    {
        struct termios termios;
        if (TcGetAttr(&termios, signalForBreak, g_hasInteractiveChildren))
        {
            if (TcSetAttr(&termios, /* blockIfBackground */ false))
            {
                g_signalForBreak = signalForBreak;
                rv = 1;
            }
        }
        pthread_mutex_unlock(&g_lock);
    }

    return rv;
}

void ReinitializeConsole()
{
    // Restores the state of the console after being suspended.
    // pal_signal.cpp calls this on SIGCONT from the signal handling thread.

    if (pthread_mutex_lock(&g_lock) == 0)
    {
        struct termios termios;
        if (TcGetAttr(&termios, g_signalForBreak, g_hasInteractiveChildren))
        {
            TcSetAttr(&termios, /* blockIfBackground */ false);
        }

        // Redo "Application mode" unless there are interactive children.
        // In that case, we'll redo it when there are no more interactive children.
        if (!g_hasInteractiveChildren)
        {
            WriteKeypadXmit();
        }

        pthread_mutex_unlock(&g_lock);
    }
}

int32_t SystemNative_InitializeConsoleAndSignalHandling()
{
    static int32_t initialized = 0;

    // Both the Process and Console class call this method for initialization.
    if (pthread_mutex_lock(&g_lock) == 0)
    {
        if (initialized == 0)
        {
            initialized = InitializeSignalHandlingCore();

            if (initialized == 1)
            {
                g_haveInitTermios = tcgetattr(STDIN_FILENO, &g_initTermios) >= 0;

                if (g_haveInitTermios)
                {
                    struct termios termios = g_initTermios;
                    ApplyTerminalSettings(&termios, g_signalForBreak, g_hasInteractiveChildren);
                    TcSetAttr(&termios, /* blockIfBackground */ false);
                }

                atexit(UninitializeConsole);
            }
        }
        pthread_mutex_unlock(&g_lock);
    }

    return initialized;
}
