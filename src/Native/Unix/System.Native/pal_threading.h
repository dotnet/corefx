// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#include "pal_compiler.h"
#include "pal_types.h"

/**
 * An opaque type representing the low-level monitor. This is equivalent to
 * POSIX threads mutex and condition variable bundled together.
 */
typedef struct Monitor Monitor;

/**
 * Allocates a new low-level monitor object and initializes it.
 * 
 * Return NULL if the allocation failed, otherwise an opaque object representing the monitor.
 */
DLLEXPORT Monitor *SystemNative_MonitorNew(void);

/**
 * Deallocates the low-level monitor allocated through SystemNative_MonitorNew.
 */
DLLEXPORT void SystemNative_MonitorDelete(Monitor *monitor);

/**
 * Acquires the lock on the low-level monitor object.
 */
DLLEXPORT void SystemNative_MonitorAcquire(Monitor *monitor);

/**
 * Releases the lock on the low-level monitor object.
 */
DLLEXPORT void SystemNative_MonitorRelease(Monitor *monitor);

/**
 * Releases the lock on an object and blocks the current thread until the
 * monitor is signalled, then it reacquires the lock.
 */
DLLEXPORT void SystemNative_MonitorWait(Monitor *monitor);

/**
 * Releases the lock on an object and blocks the current thread until the
 * monitor is signalled, then it reacquires the lock. If the specified time-out
 * interval elapses, the wait is aborted.
 * 
 * Returns 1 if the wait succeeded, or 0 if it timed out.
 */
DLLEXPORT int32_t SystemNative_MonitorTimedWait(Monitor *monitor, int32_t timeoutMilliseconds);

/**
 * Wakes one thread in the waiting queue (enqueued by either SystemNative_MonitorWait or
 * SystemNative_MonitorTimedWait) and releases the lock. Unlike Monitor.Pulse in
 * managed code it doesn't reacquire the lock.
 */
DLLEXPORT void SystemNative_MonitorSignalAndRelease(Monitor *monitor);
