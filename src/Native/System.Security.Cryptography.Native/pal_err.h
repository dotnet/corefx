// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include <stdint.h>

/*
Shim to ERR_get_error which also returns whether the error
was caused by an allocation failure.
*/
extern "C" uint64_t ErrGetError(int32_t* isAllocFailure);

/*
Direct shim to ERR_error_string_n.
*/
extern "C" void ErrErrorStringN(uint64_t e, char* buf, int32_t len);
