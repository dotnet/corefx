// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.InteropServices
{
    public partial class NativeLibraryTests
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate FunctionIdentifier PinvokeDel_Cdecl_TakesGuid(Guid value);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FunctionIdentifier PinvokeDel_Stdcall();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FunctionIdentifier PinvokeDel_Stdcall_TakesGuid(Guid value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        private delegate FunctionIdentifier PinvokeDel_Winapi_Ansi();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate FunctionIdentifier PinvokeDel_Winapi_NoCharsetSpecified();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        private delegate FunctionIdentifier PinvokeDel_Winapi_Unicode();

        public enum FunctionIdentifier
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
        }
    }
}
