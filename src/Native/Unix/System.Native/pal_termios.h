// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

include "pal_types.h"

/**
 * Get states of serial line.
 */
extern "C" int32_t SystemNative_Terminal_GetDcd(int fd);
extern "C" int32_t SystemNative_Terminal_GetCts(int fd);
extern "C" int32_t SystemNative_Terminal_GetRts(int fd);
extern "C" int32_t SystemNative_Terminal_GetDsr(int fd);
extern "C" int32_t SystemNative_Terminal_GetDtr(int fd);

extern "C" int32_t SystemNative_Terminal_GetSpeed(int fd);
extern "C" int32_t SystemNative_Terminal_SetSpeed(int fd);

extern "C" int32_t SystemNative_Terminal_AvailableBytes(int fd, int readBuffer);

extern "C" int32_t SystemNative_Terminal_Reset(int fd, int speed, int dataBits, int stopBits, int parity, int handshake);

