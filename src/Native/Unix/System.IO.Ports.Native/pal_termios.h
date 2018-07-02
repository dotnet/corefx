// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_types.h"
#include "pal_compiler.h"

BEGIN_EXTERN_C

int32_t SystemIoPortsNative_TermiosGetSignal(intptr_t fd, int32_t signal);
int32_t SystemIoPortsNative_TermiosSetSignal(intptr_t fd, int32_t signal, int32_t set);

int32_t SystemIoPortsNative_TermiosGetSpeed(intptr_t fd);
int32_t SystemIoPortsNative_TermiosSetSpeed(intptr_t fd, int32_t speed);

int32_t SystemIoPortsNative_TermiosAvailableBytes(intptr_t fd, int32_t readBuffer);

int32_t SystemIoPortsNative_TermiosReset(intptr_t fd, int32_t speed, int32_t dataBits, int32_t stopBits, int32_t parity, int32_t handshake);
int32_t SystemIoPortsNative_TermiosDiscard(intptr_t fd, int32_t queue);
int32_t SystemIoPortsNative_TermiosDrain(intptr_t fd);
int32_t SystemIoPortsNative_TermiosSendBreak(intptr_t fd, int32_t duration);

END_EXTERN_C
