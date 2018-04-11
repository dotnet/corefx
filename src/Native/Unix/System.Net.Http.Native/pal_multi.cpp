// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_config.h"
#include "pal_multi.h"
#include "pal_utilities.h"

#include <assert.h>
#include <poll.h>

static_assert((int)PAL_CURLM_CALL_MULTI_PERFORM == (int)CURLM_CALL_MULTI_PERFORM, "");
static_assert((int)PAL_CURLM_OK == (int)CURLM_OK, "");
static_assert((int)PAL_CURLM_BAD_HANDLE == (int)CURLM_BAD_HANDLE, "");
static_assert((int)PAL_CURLM_BAD_EASY_HANDLE == (int)CURLM_BAD_EASY_HANDLE, "");
static_assert((int)PAL_CURLM_OUT_OF_MEMORY == (int)CURLM_OUT_OF_MEMORY, "");
static_assert((int)PAL_CURLM_INTERNAL_ERROR == (int)CURLM_INTERNAL_ERROR, "");
static_assert((int)PAL_CURLM_BAD_SOCKET == (int)CURLM_BAD_SOCKET, "");
static_assert((int)PAL_CURLM_UNKNOWN_OPTION == (int)CURLM_UNKNOWN_OPTION, "");
#if HAVE_CURLM_ADDED_ALREADY
static_assert((int)PAL_CURLM_ADDED_ALREADY == (int)CURLM_ADDED_ALREADY, "");
#endif
static_assert((int)PAL_CURLMOPT_PIPELINING == (int)CURLMOPT_PIPELINING, "");
#ifdef CURLMOPT_MAX_HOST_CONNECTIONS
static_assert((int)PAL_CURLMOPT_MAX_HOST_CONNECTIONS == (int)CURLMOPT_MAX_HOST_CONNECTIONS, "");
#endif
#if HAVE_CURLPIPE_MULTIPLEX
static_assert((int)PAL_CURLPIPE_MULTIPLEX == (int)CURLPIPE_MULTIPLEX, "");
#endif

static_assert((int)PAL_CURLMSG_DONE == (int)CURLMSG_DONE, "");

extern "C" CURLM* HttpNative_MultiCreate()
{
    return curl_multi_init();
}

extern "C" int32_t HttpNative_MultiDestroy(CURLM* multiHandle)
{
    return curl_multi_cleanup(multiHandle);
}

extern "C" int32_t HttpNative_MultiAddHandle(CURLM* multiHandle, CURL* easyHandle)
{
    return curl_multi_add_handle(multiHandle, easyHandle);
}

extern "C" int32_t HttpNative_MultiRemoveHandle(CURLM* multiHandle, CURL* easyHandle)
{
    return curl_multi_remove_handle(multiHandle, easyHandle);
}

extern "C" int32_t HttpNative_MultiWait(CURLM* multiHandle,
                                        intptr_t extraFileDescriptor,
                                        int32_t* isExtraFileDescriptorActive,
                                        int32_t* isTimeout)
{
    assert(isExtraFileDescriptorActive != nullptr);
    assert(isTimeout != nullptr);

    curl_waitfd extraFds = {.fd = ToFileDescriptor(extraFileDescriptor), .events = CURL_WAIT_POLLIN, .revents = 0};

    // Even with our cancellation mechanism, we specify a timeout so that
    // just in case something goes wrong we can recover gracefully.  This timeout is relatively long.
    // Note, though, that libcurl has its own internal timeout, which can be requested separately
    // via curl_multi_timeout, but which is used implicitly by curl_multi_wait if it's shorter
    // than the value we provide.
    const int FailsafeTimeoutMilliseconds = 1000;

    int numFds;
    CURLMcode result = curl_multi_wait(multiHandle, &extraFds, 1, FailsafeTimeoutMilliseconds, &numFds);

    if (numFds == 0)
    {
        *isTimeout = true;
        *isExtraFileDescriptorActive = false;
    }
    else
    {
        *isTimeout = false;

        //
        // Prior to libcurl version 7.32.0, the revents field was not returned properly for "extra" file descriptors
        // passed to curl_multi_wait.  See https://github.com/dotnet/corefx/issues/9751.  So if we have a libcurl
        // prior to that version, we need to do our own poll to get the status of the extra file descriptor.
        //
        if (curl_version_info(CURLVERSION_NOW)->version_num >= 0x073200)
        {
            *isExtraFileDescriptorActive = (extraFds.revents & CURL_WAIT_POLLIN) != 0;
        }
        else
        {
            pollfd pfd = { .fd = ToFileDescriptor(extraFileDescriptor),.events = POLLIN,.revents = 0 };
            poll(&pfd, 1, 0);

            //
            // We ignore any failure in poll(), to preserve the result from curl_multi_wait.  If poll() fails, it should
            // leave revents cleared.
            //
            *isExtraFileDescriptorActive = (pfd.revents & POLLIN) != 0;
        }
    }

    return result;
}

extern "C" int32_t HttpNative_MultiPerform(CURLM* multiHandle)
{
    int running_handles;
    return curl_multi_perform(multiHandle, &running_handles);
}

extern "C" int32_t HttpNative_MultiInfoRead(CURLM* multiHandle, int32_t* message, CURL** easyHandle, int32_t* result)
{
    assert(message != nullptr);
    assert(easyHandle != nullptr);
    assert(result != nullptr);

    int msgs_in_queue;
    CURLMsg* curlMessage = curl_multi_info_read(multiHandle, &msgs_in_queue);
    if (curlMessage == nullptr)
    {
        *message = 0;
        *easyHandle = nullptr;
        *result = 0;

        return 0;
    }

    *message = curlMessage->msg;
    *easyHandle = curlMessage->easy_handle;
    *result = curlMessage->data.result;

    return 1;
}

extern "C" const char* HttpNative_MultiGetErrorString(PAL_CURLMcode code)
{
    return curl_multi_strerror(static_cast<CURLMcode>(code));
}

extern "C" int32_t HttpNative_MultiSetOptionLong(CURLM* handle, PAL_CURLMoption option, int64_t value)
{
    return curl_multi_setopt(handle, static_cast<CURLMoption>(option), value);
}
