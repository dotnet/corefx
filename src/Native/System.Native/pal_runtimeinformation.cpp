// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_types.h"
#include "pal_utilities.h"
#include <sys/utsname.h>

extern "C" int32_t UName(char* machine, int32_t capacity)
{
	assert(machine != nullptr);
	struct utsname _utsdata;
	int result = uname(&_utsdata);
	if (result != -1)
	{
		memcpy(machine, _utsdata.machine, static_cast<size_t>(capacity));
	}
	else
	{
		machine = NULL;
	}
	
	return result;
}
