// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

include "pal_types.h"

/**
 * Get states of serial line.
 */
extern "C" int32_t SystemNative_TermiosGetDcd(int fd);
extern "C" int32_t SystemNative_TermiosGetCts(int fd);
extern "C" int32_t SystemNative_TermiosGetRts(int fd);
extern "C" int32_t SystemNative_TermiosGetDsr(int fd);
extern "C" int32_t SystemNative_TermiosGetDtr(int fd);

extern "C" int32_t SystemNative_TermiosGetSpeed(int fd);
extern "C" int32_t SystemNative_TermiosSetSpeed(int fd);

extern "C" int32_t SystemNative_TermiosAvailableBytes(int fd, int readBuffer);

extern "C" int32_t SystemNative_TermiosReset(int fd, int speed, int dataBits, int stopBits, int parity, int handshake);

