// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_zlib.h"
#include "pal_utilities.h"

#include <assert.h>
#include <zlib.h>

static_assert(PAL_Z_NOFLUSH == Z_NO_FLUSH, "");
static_assert(PAL_Z_FINISH == Z_FINISH, "");

static_assert(PAL_Z_OK == Z_OK, "");
static_assert(PAL_Z_STREAMEND == Z_STREAM_END, "");
static_assert(PAL_Z_STREAMERROR == Z_STREAM_ERROR, "");
static_assert(PAL_Z_DATAERROR == Z_DATA_ERROR, "");
static_assert(PAL_Z_MEMERROR == Z_MEM_ERROR, "");
static_assert(PAL_Z_BUFERROR == Z_BUF_ERROR, "");
static_assert(PAL_Z_VERSIONERROR == Z_VERSION_ERROR, "");

static_assert(PAL_Z_NOCOMPRESSION == Z_NO_COMPRESSION, "");
static_assert(PAL_Z_BESTSPEED == Z_BEST_SPEED, "");
static_assert(PAL_Z_BESTCOMPRESSION == Z_BEST_COMPRESSION, "");

static_assert(PAL_Z_DEFAULTSTRATEGY == Z_DEFAULT_STRATEGY, "");

static_assert(PAL_Z_DEFLATED == Z_DEFLATED, "");

/*
Initializes the PAL_ZStream by creating and setting its underlying z_stream.
*/
static void Init(PAL_ZStream* stream)
{
    z_stream* zStream = new z_stream();
    zStream->zalloc = Z_NULL;
    zStream->zfree = Z_NULL;
    zStream->opaque = Z_NULL;

    stream->internalState = zStream;
}

/*
Frees any memory on the PAL_ZStream that was created by Init.
*/
static void End(PAL_ZStream* stream)
{
    z_stream* zStream = reinterpret_cast<z_stream*>(stream->internalState);
    assert(zStream != nullptr);

    delete zStream;
    stream->internalState = nullptr;
}

/*
Transfers the output values from the underlying z_stream to the PAL_ZStream.
*/
static void TransferState(z_stream* from, PAL_ZStream* to)
{
    to->nextIn = from->next_in;
    to->availIn = from->avail_in;

    to->nextOut = from->next_out;
    to->availOut = from->avail_out;

    to->msg = from->msg;
}

/*
Transfers the input values from the PAL_ZStream to the underlying z_stream object.
*/
static void TransferState(PAL_ZStream* from, z_stream* to)
{
    to->next_in = from->nextIn;
    to->avail_in = from->availIn;

    to->next_out = from->nextOut;
    to->avail_out = from->availOut;
}

/*
Gets the current z_stream object for the specified PAL_ZStream.

This ensures any inputs are transferred from the PAL_ZStream to the underlying z_stream,
since the current values are always needed.
*/
static z_stream* GetCurrentZStream(PAL_ZStream* stream)
{
    z_stream* zStream = reinterpret_cast<z_stream*>(stream->internalState);
    assert(zStream != nullptr);

    TransferState(stream, zStream);
    return zStream;
}

extern "C" int32_t DeflateInit2_(PAL_ZStream* stream, int32_t level, int32_t method, int32_t windowBits, int32_t memLevel, int32_t strategy)
{
    assert(stream != nullptr);

    Init(stream);

    z_stream* zStream = GetCurrentZStream(stream);
    int32_t result = deflateInit2(zStream, level, method, windowBits, memLevel, strategy);
    TransferState(zStream, stream);

    return result;
}

extern "C" int32_t Deflate(PAL_ZStream* stream, int32_t flush)
{
    assert(stream != nullptr);

    z_stream* zStream = GetCurrentZStream(stream);
    int32_t result = deflate(zStream, flush);
    TransferState(zStream, stream);

    return result;
}

extern "C" int32_t DeflateEnd(PAL_ZStream* stream)
{
    assert(stream != nullptr);

    z_stream* zStream = GetCurrentZStream(stream);
    int32_t result = deflateEnd(zStream);
    End(stream);

    return result;
}

extern "C" int32_t InflateInit2_(PAL_ZStream* stream, int32_t windowBits)
{
    assert(stream != nullptr);

    Init(stream);

    z_stream* zStream = GetCurrentZStream(stream);
    int32_t result = inflateInit2(zStream, windowBits);
    TransferState(zStream, stream);

    return result;
}

extern "C" int32_t Inflate(PAL_ZStream* stream, int32_t flush)
{
    assert(stream != nullptr);

    z_stream* zStream = GetCurrentZStream(stream);
    int32_t result = inflate(zStream, flush);
    TransferState(zStream, stream);

    return result;
}

extern "C" int32_t InflateEnd(PAL_ZStream* stream)
{
    assert(stream != nullptr);

    z_stream* zStream = GetCurrentZStream(stream);
    int32_t result = inflateEnd(zStream);
    End(stream);

    return result;
}

extern "C" uint32_t Crc32(uint32_t crc, uint8_t* buffer, int32_t len)
{
    assert(buffer != nullptr);

    unsigned long result = crc32(crc, buffer, UnsignedCast(len));
    assert(result <= UINT32_MAX);
    return static_cast<uint32_t>(result);
}
