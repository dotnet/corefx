// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using Xunit;

namespace System
{
    public static partial class PlatformDetection
    {
        //
        // Do not use the " { get; } = <expression> " pattern here. Having all the initialization happen in the type initializer
        // means that one exception anywhere means all tests using PlatformDetection fail. If you feel a value is worth latching,
        // do it in a way that failures don't cascade.
        //

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsFullFramework => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
        public static bool HasWindowsShell => IsWindows && IsNotWindowsServerCore && IsNotWindowsNanoServer && IsNotWindowsIoTCore;
        public static bool IsWindows7 => IsWindows && GetWindowsVersion() == 6 && GetWindowsMinorVersion() == 1;
        public static bool IsWindows8x => IsWindows && GetWindowsVersion() == 6 && (GetWindowsMinorVersion() == 2 || GetWindowsMinorVersion() == 3);
        public static bool IsWindows8xOrLater => IsWindows && new Version((int)GetWindowsVersion(), (int)GetWindowsMinorVersion()) >= new Version(6, 2);
        public static bool IsWindowsNanoServer => IsWindows && (IsNotWindowsIoTCore && GetWindowsInstallationType().Equals("Nano Server", StringComparison.OrdinalIgnoreCase));
        public static bool IsWindowsServerCore => IsWindows && GetWindowsInstallationType().Equals("Server Core", StringComparison.OrdinalIgnoreCase);
        public static int WindowsVersion => IsWindows ? (int)GetWindowsVersion() : -1;
        public static bool IsNotWindows7 => !IsWindows7;
        public static bool IsNotWindows8x => !IsWindows8x;
        public static bool IsNotWindowsNanoServer => !IsWindowsNanoServer;
        public static bool IsNotWindowsServerCore => !IsWindowsServerCore;
        public static bool IsNotWindowsIoTCore => !IsWindowsIoTCore;
        public static bool IsNotWindowsHomeEdition => !IsWindowsHomeEdition;
        public static bool IsNotInAppContainer => !IsInAppContainer;
        public static bool IsWinRTSupported => IsWindows && IsNotWindows7;
        public static bool IsWinUISupported => IsWinRTSupported && IsNotWindows8x && IsNotWindowsNanoServer && IsNotWindowsServerCore && IsNotWindowsIoTCore;
        public static bool IsNotWinRTSupported => !IsWinRTSupported;
        public static bool IsSoundPlaySupported => IsWindows && IsNotWindowsNanoServer;

        // >= Windows 10 Anniversary Update
        public static bool IsWindows10Version1607OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 14393;

         // >= Windows 10 Creators Update
        public static bool IsWindows10Version1703OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 15063;

        // >= Windows 10 Fall Creators Update
        public static bool IsWindows10Version1709OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 16299;

        // >= Windows 10 April 2018 Update
        public static bool IsWindows10Version1803OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 17134;

        // >= Windows 10 May 2019 Update (19H1)
        public static bool IsWindows10Version1903OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 18362;

        public static bool IsWindowsIoTCore
        {
            get
            {
                if (!IsWindows)
                {
                    return false;
                }

                int productType = GetWindowsProductType();
                if ((productType == PRODUCT_IOTUAPCOMMERCIAL) ||
                    (productType == PRODUCT_IOTUAP))
                {
                    return true;
                }
                return false;
            }
        }

        public static bool IsWindowsHomeEdition
        {
            get
            {
                if (!IsWindows)
                {
                    return false;
                }

                int productType = GetWindowsProductType();
                switch (productType)
                {
                    case PRODUCT_CORE:
                    case PRODUCT_CORE_COUNTRYSPECIFIC:
                    case PRODUCT_CORE_N:
                    case PRODUCT_CORE_SINGLELANGUAGE:
                    case PRODUCT_HOME_BASIC:
                    case PRODUCT_HOME_BASIC_N:
                    case PRODUCT_HOME_PREMIUM:
                    case PRODUCT_HOME_PREMIUM_N:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static bool IsWindowsSubsystemForLinux => m_isWindowsSubsystemForLinux.Value;
        public static bool IsNotWindowsSubsystemForLinux => !IsWindowsSubsystemForLinux;

        private static Lazy<bool> m_isWindowsSubsystemForLinux = new Lazy<bool>(GetIsWindowsSubsystemForLinux);
        private static bool GetIsWindowsSubsystemForLinux()
        {
            // https://github.com/Microsoft/BashOnWindows/issues/423#issuecomment-221627364
            if (IsLinux)
            {
                const string versionFile = "/proc/version";
                if (File.Exists(versionFile))
                {
                    string s = File.ReadAllText(versionFile);

                    if (s.Contains("Microsoft") || s.Contains("WSL"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetWindowsInstallationType()
        {
            string key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string value = "";

            try
            {
                value = (string)Registry.GetValue(key, "InstallationType", defaultValue: "");
            }
            catch (Exception e) when (e is SecurityException || e is InvalidCastException)
            {
            }

            return value;
        }

        private static int GetWindowsProductType()
        {
            Assert.True(GetProductInfo(Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor, 0, 0, out int productType));
            return productType;
        }

        private const int PRODUCT_IOTUAP = 0x0000007B;
        private const int PRODUCT_IOTUAPCOMMERCIAL = 0x00000083;
        private const int PRODUCT_CORE = 0x00000065;
        private const int PRODUCT_CORE_COUNTRYSPECIFIC = 0x00000063;
        private const int PRODUCT_CORE_N = 0x00000062;
        private const int PRODUCT_CORE_SINGLELANGUAGE = 0x00000064;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;

        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern bool GetProductInfo(
            int dwOSMajorVersion,
            int dwOSMinorVersion,
            int dwSpMajorVersion,
            int dwSpMinorVersion,
            out int pdwReturnedProductType
        );

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern int GetCurrentApplicationUserModelId(ref uint applicationUserModelIdLength, byte[] applicationUserModelId);

        internal static uint GetWindowsVersion()
        {
            Assert.Equal(0, Interop.NtDll.RtlGetVersionEx(out Interop.NtDll.RTL_OSVERSIONINFOEX osvi));
            return osvi.dwMajorVersion;
        }
        internal static uint GetWindowsMinorVersion()
        {
            Assert.Equal(0, Interop.NtDll.RtlGetVersionEx(out Interop.NtDll.RTL_OSVERSIONINFOEX osvi));
            return osvi.dwMinorVersion;
        }
        internal static uint GetWindowsBuildNumber()
        {
            Assert.Equal(0, Interop.NtDll.RtlGetVersionEx(out Interop.NtDll.RTL_OSVERSIONINFOEX osvi));
            return osvi.dwBuildNumber;
        }

        private static int s_isInAppContainer = -1;
        public static bool IsInAppContainer
        {
            // This actually checks whether code is running in a modern app.
            // Currently this is the only situation where we run in app container.
            // If we want to distinguish the two cases in future,
            // EnvironmentHelpers.IsAppContainerProcess in desktop code shows how to check for the AC token.
            get
            {
                if (s_isInAppContainer != -1)
                    return s_isInAppContainer == 1;

                if (!IsWindows || IsWindows7)
                {
                    s_isInAppContainer = 0;
                    return false;
                }

                byte[] buffer = Array.Empty<byte>();
                uint bufferSize = 0;
                try
                {
                    int result = GetCurrentApplicationUserModelId(ref bufferSize, buffer);
                    switch (result)
                    {
                        case 15703: // APPMODEL_ERROR_NO_APPLICATION
                        case 120:   // ERROR_CALL_NOT_IMPLEMENTED
                                    // This function is not supported on this system.
                                    // In example on Windows Nano Server
                            s_isInAppContainer = 0;
                            break;
                        case 0:     // ERROR_SUCCESS
                        case 122:   // ERROR_INSUFFICIENT_BUFFER
                                    // Success is actually insufficent buffer as we're really only looking for
                                    // not NO_APPLICATION and we're not actually giving a buffer here. The
                                    // API will always return NO_APPLICATION if we're not running under a
                                    // WinRT process, no matter what size the buffer is.
                            s_isInAppContainer = 1;
                            break;
                        default:
                            throw new InvalidOperationException($"Failed to get AppId, result was {result}.");
                    }
                }
                catch (Exception e)
                {
                    // We could catch this here, being friendly with older portable surface area should we
                    // desire to use this method elsewhere.
                    if (e.GetType().FullName.Equals("System.EntryPointNotFoundException", StringComparison.Ordinal))
                    {
                        // API doesn't exist, likely pre Win8
                        s_isInAppContainer = 0;
                    }
                    else
                    {
                        throw;
                    }
                }

                return s_isInAppContainer == 1;
            }
        }

        private static int s_isWindowsElevated = -1;
        public static bool IsWindowsAndElevated
        {
            get
            {
                if (s_isWindowsElevated != -1)
                    return s_isWindowsElevated == 1;

                if (!IsWindows || IsInAppContainer)
                {
                    s_isWindowsElevated = 0;
                    return false;
                }

                s_isWindowsElevated = AdminHelpers.IsProcessElevated() ? 1 : 0;

                return s_isWindowsElevated == 1;
            }
        }
    }
}
