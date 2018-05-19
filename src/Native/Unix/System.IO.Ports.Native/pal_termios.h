// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_types.h"
#include "pal_compiler.h"

BEGIN_EXTERN_C

int32_t TermiosGetSignal(int32_t fd, int32_t signal);
int32_t TermiosSetSignal(int32_t fd, int32_t signal, int32_t set);

int32_t TermiosGetSpeed(int32_t fd);
int32_t TermiosSetSpeed(int32_t fd, uint32_t speed);

int32_t TermiosAvailableBytes(int32_t fd, int32_t readBuffer);

int32_t TermiosReset(int32_t fd, int speed, int dataBits, int stopBits, int parity, int handshake);
int32_t TermiosDiscard(int32_t fd, int32_t queue);
int32_t TermiosDrain(int32_t fd);
int32_t TermiosSendBreak(int32_t fd, uint32_t duration);

END_EXTERN_C
