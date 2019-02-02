// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_config.h"
#include "pal_threading.h"
#include "pal_utilities.h"

#include <stdlib.h>
#include <pthread.h>
#include <time.h>

struct Monitor
{
    pthread_mutex_t Mutex;
    pthread_cond_t Condition;
};

enum
{
    SecondsToNanoSeconds = 1000000000, // 10^9
    MilliSecondsToNanoSeconds = 1000000 // 10^6
};

#ifdef NDEBUG
#define assert_no_error(err) ((void)(err))
#else
#define assert_no_error(err) assert((err) == 0)
#endif

Monitor *SystemNative_MonitorNew(void)
{
    Monitor *monitor;
    int error, initError;

    monitor = (Monitor *)malloc(sizeof(struct Monitor));
    if (monitor == NULL)
    {
        return NULL;
    }

    error = pthread_mutex_init(&monitor->Mutex, NULL);
    if (error != 0)
    {
        free(monitor);
        return NULL;
    }

#if HAVE_MACH_ABSOLUTE_TIME
    // Older versions of OSX don't support CLOCK_MONOTONIC, so we don't use pthread_condattr_setclock. See
    // Wait(int32_t timeoutMilliseconds).
    initError = pthread_cond_init(&monitor->Condition, NULL);
#elif HAVE_PTHREAD_CONDATTR_SETCLOCK && HAVE_CLOCK_MONOTONIC
    pthread_condattr_t conditionAttributes;
    error = pthread_condattr_init(&conditionAttributes);
    if (error != 0)
    {
        error = pthread_mutex_destroy(&monitor->Mutex);
        assert_no_error(error);
        free(monitor);
        return NULL;
    }

    error = pthread_condattr_setclock(&conditionAttributes, CLOCK_MONOTONIC);
    assert_no_error(error);

    initError = pthread_cond_init(&monitor->Condition, &conditionAttributes);

    error = pthread_condattr_destroy(&conditionAttributes);
    assert_no_error(error);
#else
    #error "Don't know how to perform timed wait on this platform"
#endif

    if (initError != 0)
    {
        error = pthread_mutex_destroy(&monitor->Mutex);
        assert_no_error(error);
        free(monitor);
        return NULL;
    }
    
    return monitor;
}

void SystemNative_MonitorDelete(Monitor *monitor)
{
    int error;
        
    error = pthread_mutex_destroy(&monitor->Mutex);
    assert_no_error(error);
    error = pthread_cond_destroy(&monitor->Condition);
    assert_no_error(error);
    free(monitor);
}

void SystemNative_MonitorAcquire(Monitor *monitor)
{
    int error = pthread_mutex_lock(&monitor->Mutex);
    assert_no_error(error);
}

int32_t SystemNative_MonitorTryAcquire(Monitor *monitor)
{
    int error = pthread_mutex_trylock(&monitor->Mutex);
    assert(error == 0 || error == EBUSY);
    return error == 0;
}

void SystemNative_MonitorRelease(Monitor *monitor)
{
    int error = pthread_mutex_unlock(&monitor->Mutex);
    assert_no_error(error);
}

void SystemNative_MonitorWait(Monitor *monitor)
{
    int error = pthread_cond_wait(&monitor->Condition, &monitor->Mutex);
    assert_no_error(error);
}

int32_t SystemNative_MonitorTimedWait(Monitor *monitor, int32_t timeoutMilliseconds)
{
    int error;

    assert(timeoutMilliseconds >= -1);
    if (timeoutMilliseconds < 0)
    {
        SystemNative_MonitorWait(monitor);
        return 1;
    }

    // Calculate the time at which a timeout should occur, and wait. Older versions of OSX don't support clock_gettime with
    // CLOCK_MONOTONIC, so we instead compute the relative timeout duration, and use a relative variant of the timed wait.
    struct timespec timeout;
#if HAVE_MACH_ABSOLUTE_TIME
    timeout.tv_sec = (time_t)(timeoutMilliseconds / 1000);
    timeout.tv_nsec = (long)((timeoutMilliseconds % 1000) * MilliSecondsToNanoSeconds);
    error = pthread_cond_timedwait_relative_np(&monitor->Condition, &monitor->Mutex, &timeout);
#else
    uint64_t nanoseconds;
    error = clock_gettime(CLOCK_MONOTONIC, &timeout);
    assert_no_error(error);
    nanoseconds = ((uint64_t)timeout.tv_sec * SecondsToNanoSeconds) + (uint64_t)timeout.tv_nsec;
    nanoseconds += (uint64_t)timeoutMilliseconds * MilliSecondsToNanoSeconds;
    timeout.tv_sec = (time_t)(nanoseconds / SecondsToNanoSeconds);
    timeout.tv_nsec = (long)(nanoseconds % SecondsToNanoSeconds);
    error = pthread_cond_timedwait(&monitor->Condition, &monitor->Mutex, &timeout);
#endif
    assert(error == 0 || error == ETIMEDOUT);

    return error == 0;
}

void SystemNative_MonitorSignalAndRelease(Monitor *monitor)
{
    int error = pthread_cond_signal(&monitor->Condition);
    assert_no_error(error);
    SystemNative_MonitorRelease(monitor);
}

void SystemNative_MonitorBroadcastAndRelease(Monitor *monitor)
{
    int error = pthread_cond_broadcast(&monitor->Condition);
    assert(error == 0);
    SystemNative_MonitorRelease(monitor);
}
