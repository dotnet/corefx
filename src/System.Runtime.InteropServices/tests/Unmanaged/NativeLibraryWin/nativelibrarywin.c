// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "stdafx.h"

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

typedef enum
{
    FN_FunctionStdcall = 0,
    FN_FunctionCdecl,
    FN_WinapiWithBaseOnly,
    FN_WinapiWithBaseAndAnsiAndUnicode,
    FN_WinapiWithBaseAndAnsiAndUnicodeA,
    FN_WinapiWithBaseAndAnsiAndUnicodeW,
    FN_WinapiWithAnsiAndUnicodeA,
    FN_WinapiWithAnsiAndUnicodeW,
    FN_WinapiWithBaseAndUnicode,
    FN_WinapiWithBaseAndUnicodeW,
    FN_ExportedByNameAndOrdinal,
    FN_ExportedByOrdinalOnly
} FUNCTION_IDENTIFIER;

__declspec(dllexport)
GUID _globalGuid;

__declspec(dllexport)
FUNCTION_IDENTIFIER __stdcall FunctionStdcall(GUID guid)
{
    UNREFERENCED_PARAMETER(guid);
    return FN_FunctionStdcall;
}

__declspec(dllexport)
FUNCTION_IDENTIFIER __cdecl FunctionCdecl(GUID guid)
{
    UNREFERENCED_PARAMETER(guid);
    return FN_FunctionCdecl;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithBaseOnly()
{
    return FN_WinapiWithBaseOnly;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithBaseAndAnsiAndUnicode()
{
    return FN_WinapiWithBaseAndAnsiAndUnicode;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithBaseAndAnsiAndUnicodeA()
{
    return FN_WinapiWithBaseAndAnsiAndUnicodeA;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithBaseAndAnsiAndUnicodeW()
{
    return FN_WinapiWithBaseAndAnsiAndUnicodeW;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithAnsiAndUnicodeA()
{
    return FN_WinapiWithAnsiAndUnicodeA;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithAnsiAndUnicodeW()
{
    return FN_WinapiWithAnsiAndUnicodeW;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithBaseAndUnicode()
{
    return FN_WinapiWithBaseAndUnicode;
}

FUNCTION_IDENTIFIER WINAPI WinapiWithBaseAndUnicodeW()
{
    return FN_WinapiWithBaseAndUnicodeW;
}

FUNCTION_IDENTIFIER __stdcall ExportedByNameAndOrdinal()
{
    return FN_ExportedByNameAndOrdinal;
}

FUNCTION_IDENTIFIER __stdcall ExportedByOrdinalOnly()
{
    return FN_ExportedByOrdinalOnly;
}
