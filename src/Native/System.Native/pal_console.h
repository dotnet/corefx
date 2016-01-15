// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "pal_types.h"

/*
 * Window Size of the terminal
 */
struct WinSize
{
    uint16_t Row;
    uint16_t Col;
    uint16_t XPixel;
    uint16_t YPixel;
};

/**
 * Gets the windows size of the terminal
 *
 * Returns 0 on success; otherwise, returns errorNo.
 */
extern "C" int32_t SystemNative_GetWindowSize(WinSize* windowsSize);

/**
 * Gets whether the specified file descriptor is for a terminal.
 *
 * Returns 1 if the file descriptor is referring to a terminal;
 * otherwise returns 0 and sets errno.
 */
extern "C" int32_t SystemNative_IsATty(intptr_t fd);

/**
 * Initializes the console for use by System.Console.
 */
extern "C" void SystemNative_InitializeConsole();

/**
 * Returns 1 if any input is waiting on stdin; otherwise, 0.
 */
extern "C" int32_t SystemNative_StdinReady();

/**
 * Reads the number of bytes specified into the provided buffer from stdin.
 * in a non-echo and non-canonical mode.
 * Returns the number of bytes read on success; otherwise, -1 is returned an errno is set.
 */
extern "C" int32_t SystemNative_ReadStdinUnbuffered(void* buffer, int32_t bufferSize);

enum CtrlCode : int32_t
{
    Interrupt = 0,
    Break = 1
};

typedef int32_t (*CtrlCallback)(CtrlCode signalCode);

/**
 * Hooks up the specified callback for notifications when SIGINT or SIGQUIT is received.
 *
 * Not thread safe.  Caller must provide its owns synchronization to ensure RegisterForCtrl
 * is not called concurrently with itself or with UnregisterForCtrl.
 *
 * Should only be called when a callback is not currently registered.
 *
 * Returns 1 on success, 0 on failure.
 */
extern "C" int32_t SystemNative_RegisterForCtrl(CtrlCallback callback);

/**
 * Unregisters the previously registered ctrlCCallback.
 *
 * Not thread safe.  Caller must provide its owns synchronization to ensure UnregisterForCtrl
 * is not called concurrently with itself or with RegisterForCtrl.
 *
 * Should only be called when a callback is currently registered. The pointer
 * previously registered must remain valid until all ctrl handling activity
 * has quiesced.
 */
extern "C" void SystemNative_UnregisterForCtrl();
