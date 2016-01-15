// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "pal_types.h"

#include <curl/curl.h>

enum
{
    CurlOptionLongBase = 0,
    CurlOptionObjectPointBase = 10000,
};

enum PAL_CURLoption : int32_t
{
    PAL_CURLOPT_INFILESIZE = CurlOptionLongBase + 14,
    PAL_CURLOPT_VERBOSE = CurlOptionLongBase + 41,
    PAL_CURLOPT_NOBODY = CurlOptionLongBase + 44,
    PAL_CURLOPT_UPLOAD = CurlOptionLongBase + 46,
    PAL_CURLOPT_POST = CurlOptionLongBase + 47,
    PAL_CURLOPT_FOLLOWLOCATION = CurlOptionLongBase + 52,
    PAL_CURLOPT_PROXYPORT = CurlOptionLongBase + 59,
    PAL_CURLOPT_POSTFIELDSIZE = CurlOptionLongBase + 60,
    PAL_CURLOPT_MAXREDIRS = CurlOptionLongBase + 68,
    PAL_CURLOPT_HTTP_VERSION = CurlOptionLongBase + 84,
    PAL_CURLOPT_NOSIGNAL = CurlOptionLongBase + 99,
    PAL_CURLOPT_PROXYTYPE = CurlOptionLongBase + 101,
    PAL_CURLOPT_HTTPAUTH = CurlOptionLongBase + 107,
    PAL_CURLOPT_PROTOCOLS = CurlOptionLongBase + 181,
    PAL_CURLOPT_REDIR_PROTOCOLS = CurlOptionLongBase + 182,

    PAL_CURLOPT_URL = CurlOptionObjectPointBase + 2,
    PAL_CURLOPT_PROXY = CurlOptionObjectPointBase + 4,
    PAL_CURLOPT_PROXYUSERPWD = CurlOptionObjectPointBase + 6,
    PAL_CURLOPT_COOKIE = CurlOptionObjectPointBase + 22,
    PAL_CURLOPT_HTTPHEADER = CurlOptionObjectPointBase + 23,
    PAL_CURLOPT_CUSTOMREQUEST = CurlOptionObjectPointBase + 36,
    PAL_CURLOPT_ACCEPT_ENCODING = CurlOptionObjectPointBase + 102,
    PAL_CURLOPT_PRIVATE = CurlOptionObjectPointBase + 103,
    PAL_CURLOPT_COPYPOSTFIELDS = CurlOptionObjectPointBase + 165,
    PAL_CURLOPT_USERNAME = CurlOptionObjectPointBase + 173,
    PAL_CURLOPT_PASSWORD = CurlOptionObjectPointBase + 174,
};

enum class ReadWriteFunction : int32_t
{
    Write = 0,
    Read = 1,
    Header = 2,
};

enum PAL_CURLcode : int32_t
{
    PAL_CURLE_OK = 0,
    PAL_CURLE_UNSUPPORTED_PROTOCOL = 1,
    PAL_CURLE_NOT_BUILT_IN = 4,
    PAL_CURLE_COULDNT_RESOLVE_HOST = 6,
    PAL_CURLE_ABORTED_BY_CALLBACK = 42,
    PAL_CURLE_UNKNOWN_OPTION = 48,
};

enum
{
    CurlInfoStringBase = 0x100000,
    CurlInfoLongBase = 0x200000,
};

enum PAL_CURL_HTTP_VERSION
{
    PAL_CURL_HTTP_VERSION_NONE = 0,
    PAL_CURL_HTTP_VERSION_1_0 = 1,
    PAL_CURL_HTTP_VERSION_1_1 = 2,
    PAL_CURL_HTTP_VERSION_2_0 = 3
};

enum PAL_CURLINFO : int32_t
{
    PAL_CURLINFO_PRIVATE = CurlInfoStringBase + 21,
    PAL_CURLINFO_HTTPAUTH_AVAIL = CurlInfoLongBase + 23,
};

enum PAL_CURLAUTH : int64_t
{
    PAL_CURLAUTH_None = 0,
    PAL_CURLAUTH_Basic = 1 << 0,
    PAL_CURLAUTH_Digest = 1 << 1,
    PAL_CURLAUTH_Negotiate = 1 << 2,
};

enum PAL_CURLPROXYTYPE : int32_t
{
    PAL_CURLPROXY_HTTP = 0,
};

enum PAL_CURLPROTO : int32_t
{
    PAL_CURLPROTO_HTTP = (1 << 0),
    PAL_CURLPROTO_HTTPS = (1 << 1),
};

enum PAL_CurlSeekResult : int32_t
{
    PAL_CURL_SEEKFUNC_OK = 0,
    PAL_CURL_SEEKFUNC_FAIL = 1,
    PAL_CURL_SEEKFUNC_CANTSEEK = 2,
};

const uint64_t PAL_CURL_READFUNC_ABORT = 0x10000000;
const uint64_t PAL_CURL_READFUNC_PAUSE = 0x10000001;
const uint64_t PAL_CURL_WRITEFUNC_PAUSE = 0x10000001;

/*
Creates a new CURL instance.

Returns the new CURL instance or nullptr if something went wrong.
*/
extern "C" CURL* HttpNative_EasyCreate();

/*
Cleans up and deletes a CURL instance.

No-op if handle is null.
The given CURL pointer is invalid after this call.
*/
extern "C" void HttpNative_EasyDestroy(CURL* handle);

/*
Shims the curl_easy_setopt function, which takes a variable number of
arguments, but must be a long, a function pointer, an object pointer or a curl_off_t,
depending on what option is supplied.
*/
extern "C" int32_t HttpNative_EasySetOptionString(CURL* handle, PAL_CURLoption option, const char* value);
extern "C" int32_t HttpNative_EasySetOptionLong(CURL* handle, PAL_CURLoption option, int64_t value);
extern "C" int32_t HttpNative_EasySetOptionPointer(CURL* handle, PAL_CURLoption option, void* value);

/*
Returns a string describing the CURLcode error code.
*/
extern "C" const char* HttpNative_EasyGetErrorString(PAL_CURLcode code);

/*
Shims the curl_easy_setopt function, which takes a variable number of
arguments.
*/
extern "C" int32_t HttpNative_EasyGetInfoPointer(CURL* handle, PAL_CURLINFO info, void** value);
extern "C" int32_t HttpNative_EasyGetInfoLong(CURL* handle, PAL_CURLINFO info, int64_t* value);

/*
Shims the curl_easy_perform function.

Returns CURLE_OK (0) if everything was ok, non-zero means an error occurred.
*/
extern "C" int32_t HttpNative_EasyPerform(CURL* handle);

/*
Unpauses the CURL request.

Returns CURLE_OK (0) if everything was ok, non-zero means an error occurred.
*/
extern "C" int32_t HttpNative_EasyUnpause(CURL* handle);

// the function pointer definition for the callback used in RegisterSeekCallback
typedef int32_t (*SeekCallback)(void* userPointer, int64_t offset, int32_t origin);

// the function pointer definition for the callback used in RegisterReadWriteCallback
typedef uint64_t (*ReadWriteCallback)(uint8_t* buffer, uint64_t bufferSize, uint64_t nitems, void* userPointer);

// the function pointer definition for the callback used in RegisterSslCtxCallback
typedef int32_t (*SslCtxCallback)(CURL* curl, void* sslCtx, void* userPointer);

/*
The object that is returned from RegisterXXXCallback functions.
This holds the data necessary to know what managed callback to invoke and with what args.
*/
struct CallbackHandle;

/*
Registers a callback in libcurl for seeking in an input stream.

This function gets called by libcurl to seek to a certain position in the input stream
and can be used to fast forward a file in a resumed upload.
*/
extern "C" void
HttpNative_RegisterSeekCallback(CURL* curl, SeekCallback callback, void* userPointer, CallbackHandle** callbackHandle);

/*
Registers a callback in libcurl for reading/writing input/output streams.
*/
extern "C" void HttpNative_RegisterReadWriteCallback(CURL* curl,
                                                     ReadWriteFunction functionType,
                                                     ReadWriteCallback callback,
                                                     void* userPointer,
                                                     CallbackHandle** callbackHandle);

/*
Registers a callback in libcurl for initializing SSL connections.

This callback function gets called by libcurl just before the initialization of an SSL connection
after having processed all other SSL related options to give a last chance to an application
to modify the behaviour of the SSL initialization.

Returns a CURLcode that describes whether registering the callback was successful or not.
*/
extern "C" int32_t HttpNative_RegisterSslCtxCallback(CURL* curl,
                                                     SslCtxCallback callback,
                                                     void* userPointer,
                                                     CallbackHandle** callbackHandle);

/*
Frees the CallbackHandle created by a RegisterXXXCallback function.
*/
extern "C" void HttpNative_FreeCallbackHandle(CallbackHandle* callbackHandle);
