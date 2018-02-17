// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices
{
    [PlatformSpecific(TestPlatforms.Windows)]
    public partial class NativeLibraryTests
    {
        private const string LIB_KERNEL32 = "kernel32.dll";

        private readonly NativeLibrary _nativeLibrary;

        public NativeLibraryTests()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return; // this only works on Windows; no-op otherwise
            }

            string nativeLibraryFilename;
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    nativeLibraryFilename = "NativeLibraryWin.x86.dll";
                    break;
                case Architecture.X64:
                    nativeLibraryFilename = "NativeLibraryWin.x64.dll";
                    break;
                default:
                    throw new Exception($"Unexpected architecture {RuntimeInformation.ProcessArchitecture}!");
            }

            // Load the library and ensure its handle matches what we got from the OS, otherwise tests won't succeed

            bool loaded = NativeLibrary.TryLoad(nativeLibraryFilename, typeof(NativeLibraryTests).Assembly, DllImportSearchPath.AssemblyDirectory, out _nativeLibrary);
            Assert.True(loaded);
            Assert.NotNull(_nativeLibrary);
        }

        [Fact]
        public void TryGetDelegate_FunctionCdecl()
        {
            var del = _nativeLibrary.GetDelegate<PinvokeDel_Cdecl_TakesGuid>("FunctionCdecl");
            Assert.NotNull(del);

            Assert.Equal(FunctionIdentifier.FN_FunctionCdecl, del(Guid.Empty));
        }

        [Fact]
        public void TryGetDelegate_FunctionStdcall_AllowMangling()
        {
            var del = _nativeLibrary.GetDelegate<PinvokeDel_Stdcall_TakesGuid>("FunctionStdcall");
            Assert.NotNull(del);

            Assert.Equal(FunctionIdentifier.FN_FunctionStdcall, del(Guid.Empty));
        }

        [Fact]
        public void TryGetDelegate_FunctionStdcall_NoMangling()
        {
            string symbolName = (RuntimeInformation.ProcessArchitecture == Architecture.X86) ? "_FunctionStdcall@16" : "FunctionStdcall";
            var del = _nativeLibrary.GetDelegate<PinvokeDel_Stdcall_TakesGuid>(symbolName, exactSpelling: true);
            Assert.NotNull(del);

            Assert.Equal(FunctionIdentifier.FN_FunctionStdcall, del(Guid.Empty));
        }

        [Theory]
        [InlineData("#100", FunctionIdentifier.FN_ExportedByNameAndOrdinal)]
        [InlineData("#200", FunctionIdentifier.FN_ExportedByOrdinalOnly)]
        public void TryGetDelegate_ByOrdinal(string pseudonym, FunctionIdentifier expectedReturnValue)
        {
            var del = _nativeLibrary.GetDelegate<PinvokeDel_Stdcall>(pseudonym);
            Assert.NotNull(del);

            Assert.Equal(expectedReturnValue, del());
        }

        [Theory]
        [InlineData(typeof(PinvokeDel_Winapi_NoCharsetSpecified), "WinapiWithBaseOnly", FunctionIdentifier.FN_WinapiWithBaseOnly, FunctionIdentifier.FN_WinapiWithBaseOnly)]
        [InlineData(typeof(PinvokeDel_Winapi_NoCharsetSpecified), "WinapiWithBaseAndAnsiAndUnicode", FunctionIdentifier.FN_WinapiWithBaseAndAnsiAndUnicode, FunctionIdentifier.FN_WinapiWithBaseAndAnsiAndUnicode)] // base name wins over ANSI name
        [InlineData(typeof(PinvokeDel_Winapi_Ansi), "WinapiWithBaseAndAnsiAndUnicode", FunctionIdentifier.FN_WinapiWithBaseAndAnsiAndUnicode, FunctionIdentifier.FN_WinapiWithBaseAndAnsiAndUnicode)] // base name wins over ANSI name
        [InlineData(typeof(PinvokeDel_Winapi_Unicode), "WinapiWithBaseAndAnsiAndUnicode", FunctionIdentifier.FN_WinapiWithBaseAndAnsiAndUnicodeW, FunctionIdentifier.FN_WinapiWithBaseAndAnsiAndUnicode)] // Unicode name wins over base name
        [InlineData(typeof(PinvokeDel_Winapi_NoCharsetSpecified), "WinapiWithAnsiAndUnicode", FunctionIdentifier.FN_WinapiWithAnsiAndUnicodeA, null)]
        [InlineData(typeof(PinvokeDel_Winapi_Ansi), "WinapiWithAnsiAndUnicode", FunctionIdentifier.FN_WinapiWithAnsiAndUnicodeA, null)]
        [InlineData(typeof(PinvokeDel_Winapi_Unicode), "WinapiWithAnsiAndUnicode", FunctionIdentifier.FN_WinapiWithAnsiAndUnicodeW, null)]
        [InlineData(typeof(PinvokeDel_Winapi_NoCharsetSpecified), "WinapiWithBaseAndUnicode", FunctionIdentifier.FN_WinapiWithBaseAndUnicode, FunctionIdentifier.FN_WinapiWithBaseAndUnicode)]
        [InlineData(typeof(PinvokeDel_Winapi_Ansi), "WinapiWithBaseAndUnicode", FunctionIdentifier.FN_WinapiWithBaseAndUnicode, FunctionIdentifier.FN_WinapiWithBaseAndUnicode)]
        [InlineData(typeof(PinvokeDel_Winapi_Unicode), "WinapiWithBaseAndUnicode", FunctionIdentifier.FN_WinapiWithBaseAndUnicodeW, FunctionIdentifier.FN_WinapiWithBaseAndUnicode)]
        public void TryGetDelegate_WithAnsiAndUnicodeRenaming(Type delegateType, string name, FunctionIdentifier expectedReturnValue, FunctionIdentifier? expectedReturnValueWithExactSpelling)
        {
            // Allow mangled names (exactSpelling = false) first

            var del = _nativeLibrary.GetDelegateReturning<FunctionIdentifier>(delegateType, name);
            Assert.NotNull(del);
            Assert.Equal(expectedReturnValue, del());

            // Prohibit mangled names (exactSpelling = true) next

            del = _nativeLibrary.GetDelegateReturning<FunctionIdentifier>(delegateType, name, exactSpelling: true);

            if (expectedReturnValueWithExactSpelling != null)
            {
                Assert.NotNull(del);
                Assert.Equal(expectedReturnValueWithExactSpelling, del());
            }
            else
            {
                Assert.Null(del);
            }
        }

        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms684175(v=vs.85).aspx
        [DllImport(LIB_KERNEL32, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr LoadLibrary(
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms683212(v=vs.85).aspx
        [DllImport(LIB_KERNEL32, CallingConvention = CallingConvention.Winapi, SetLastError = true, ThrowOnUnmappableChar = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetProcAddress(
            [In] IntPtr hModule,
            [In, MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms683212(v=vs.85).aspx
        [DllImport(LIB_KERNEL32, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static extern IntPtr GetProcAddress(
            [In] IntPtr hModule,
            [In] IntPtr lpProcName);
    }
}
