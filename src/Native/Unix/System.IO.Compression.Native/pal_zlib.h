// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_types.h"

/*
A structure that holds the input and output values for the zlib functions.
*/
struct PAL_ZStream
{
    uint8_t* nextIn; // next input byte
    uint8_t* nextOut; // next output byte should be put there

    char* msg; // last error message, NULL if no error

    // the underlying z_stream object is held in internalState, but it should not be used by external callers
    void* internalState; 

    uint32_t availIn; // number of bytes available at nextIn
    uint32_t availOut; // remaining free space at nextOut
};

/*
Allowed flush values for the Deflate and Inflate functions.
*/
enum PAL_FlushCode : int32_t
{
    PAL_Z_NOFLUSH = 0,
    PAL_Z_FINISH = 4,
};

/*
Error codes from the zlib functions.
*/
enum PAL_ErrorCode : int32_t
{
    PAL_Z_OK = 0,
    PAL_Z_STREAMEND = 1,
    PAL_Z_STREAMERROR = -2,
    PAL_Z_DATAERROR = -3,
    PAL_Z_MEMERROR = -4,
    PAL_Z_BUFERROR = -5,
    PAL_Z_VERSIONERROR = -6
};

/*
Compression levels
*/
enum PAL_CompressionLevel : int32_t
{
    PAL_Z_NOCOMPRESSION = 0,
    PAL_Z_BESTSPEED = 1,
    PAL_Z_BESTCOMPRESSION = 9,
};

/*
Compression strategy
*/
enum PAL_CompressionStrategy : int32_t
{
    PAL_Z_DEFAULTSTRATEGY = 0
};

/*
The deflate compression method (the only one supported in this version)
*/
enum PAL_CompressionMethod : int32_t
{
    PAL_Z_DEFLATED = 8
};

/*
Initializes the PAL_ZStream so the Deflate function can be invoked on it.

Returns a PAL_ErrorCode indicating success or an error number on failure.
*/
extern "C" int32_t DeflateInit2_(PAL_ZStream* stream, int32_t level, int32_t method, int32_t windowBits, int32_t memLevel, int32_t strategy);

/*
Deflates (compresses) the bytes in the PAL_ZStream's nextIn buffer and puts the
compressed bytes in nextOut.

Returns a PAL_ErrorCode indicating success or an error number on failure.
*/
extern "C" int32_t Deflate(PAL_ZStream* stream, int32_t flush);

/*
All dynamically allocated data structures for this stream are freed.

Returns a PAL_ErrorCode indicating success or an error number on failure.
*/
extern "C" int32_t DeflateEnd(PAL_ZStream* stream);

/*
Initializes the PAL_ZStream so the Inflate function can be invoked on it.

Returns a PAL_ErrorCode indicating success or an error number on failure.
*/
extern "C" int32_t InflateInit2_(PAL_ZStream* stream, int32_t windowBits);

/*
Inflates (uncompresses) the bytes in the PAL_ZStream's nextIn buffer and puts the
uncompressed bytes in nextOut.

Returns a PAL_ErrorCode indicating success or an error number on failure.
*/
extern "C" int32_t Inflate(PAL_ZStream* stream, int32_t flush);

/*
All dynamically allocated data structures for this stream are freed.

Returns a PAL_ErrorCode indicating success or an error number on failure.
*/
extern "C" int32_t InflateEnd(PAL_ZStream* stream);

/*
Update a running CRC-32 with the bytes buffer[0..len-1] and return the
updated CRC-32.

Returns the updated CRC-32.
*/
extern "C" uint32_t Crc32(uint32_t crc, uint8_t* buffer, int32_t len);
