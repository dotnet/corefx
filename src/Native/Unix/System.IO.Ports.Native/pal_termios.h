// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_types.h"
#include "pal_compiler.h"

BEGIN_EXTERN_C

int32_t Termios_GetSignal(int32_t fd, int32_t signal);
int32_t Termios_SetSignal(int32_t fd, int32_t signal, int32_t set);

int32_t Termios_GetSpeed(int32_t fd);
int32_t Termios_SetSpeed(int32_t fd, uint32_t speed);

int32_t Termios_AvailableBytes(int32_t fd, int32_t readBuffer);

int32_t Termios_Reset(int32_t fd, int speed, int dataBits, int stopBits, int parity, int handshake);
int32_t Termios_Discard(int32_t fd, int32_t queue);
int32_t Termios_Drain(int32_t fd);
int32_t Termios_SendBreak(int32_t fd, uint32_t duration);

END_EXTERN_C
