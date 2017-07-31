// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

        public static bool IsUap => IsWinRT || IsNetNative;
        public static bool IsFullFramework => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
        public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);
        public static bool IsNetCore => RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsWindows7 => IsWindows && GetWindowsVersion() == 6 && GetWindowsMinorVersion() == 1;
        public static bool IsWindows8x => IsWindows && GetWindowsVersion() == 6 && (GetWindowsMinorVersion() == 2 || GetWindowsMinorVersion() == 3);
        public static bool IsNotWindows8x => !IsWindows8x;
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsNetBSD => RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD"));
        public static bool IsOpenSUSE => IsDistroAndVersion("opensuse");
        public static bool IsUbuntu => IsDistroAndVersion("ubuntu");
        public static bool IsWindowsNanoServer => (IsWindows && IsNotWindowsIoTCore && !File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "regedit.exe")));
        public static bool IsNotWindowsNanoServer => !IsWindowsNanoServer;
        public static bool IsWindowsIoTCore
        {
            get
            {
                int productType;
                Assert.True(GetProductInfo(Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor, 0, 0, out productType));
                if ((productType == PRODUCT_IOTUAPCOMMERCIAL) ||
                    (productType == PRODUCT_IOTUAP))
                {
                    return true;
                }
                return false;
            }
        }
        public static bool IsNotWindowsIoTCore => !IsWindowsIoTCore;
        public static bool IsDrawingSupported => (IsNotWindowsNanoServer && IsNotWindowsIoTCore);
        public static bool IsWindows10Version1607OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 14393;
        public static bool IsWindows10Version1703OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 15063;
        public static bool IsWindows10InsiderPreviewBuild16215OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 16215;
        public static bool IsArmProcess => RuntimeInformation.ProcessArchitecture == Architecture.Arm;
        public static bool IsNotArmProcess => !IsArmProcess;

        // Windows OneCoreUAP SKU doesn't have httpapi.dll
        public static bool IsNotOneCoreUAP => (!IsWindows || 
            File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "httpapi.dll")));

        public static int WindowsVersion => GetWindowsVersion();

        public static bool IsNetfx462OrNewer()
        {
            if (!IsFullFramework)
            {
                return false;
            }

            Version net462 = new Version(4, 6, 2);
            Version runningVersion = GetFrameworkVersion();
            return runningVersion != null && runningVersion >= net462;
        }

        public static bool IsNetfx470OrNewer()
        {
            if (!IsFullFramework)
            {
                return false;
            }

            Version net470 = new Version(4, 7, 0);
            Version runningVersion = GetFrameworkVersion();
            return runningVersion != null && runningVersion >= net470;
        }

        public static bool IsNetfx471OrNewer()
        {
            if (!IsFullFramework)
            {
                return false;
            }

            Version net471 = new Version(4, 7, 1);
            Version runningVersion = GetFrameworkVersion();
            return runningVersion != null && runningVersion >= net471;
        }

        public static Version GetFrameworkVersion()
        {
            string[] descriptionArray = RuntimeInformation.FrameworkDescription.Split(' ');
            if (descriptionArray.Length < 3)
                return null;

            if (!Version.TryParse(descriptionArray[2], out Version actualVersion))
                return null;

            foreach (Range currentRange in FrameworkRanges)
            {
                if (currentRange.IsInRange(actualVersion))
                    return currentRange.FrameworkVersion;
            }

            return null;
        }

        private static int s_isWinRT = -1;

        public static bool IsWinRT
        {
            get
            {
                if (s_isWinRT != -1)
                    return s_isWinRT == 1;

                if (!IsWindows || IsWindows7)
                {
                    s_isWinRT = 0;
                    return false;
                }

                byte[] buffer = new byte[0];
                uint bufferSize = 0;
                try
                {
                    int result = GetCurrentApplicationUserModelId(ref bufferSize, buffer);
                    switch (result)
                    {
                        case 15703: // APPMODEL_ERROR_NO_APPLICATION
                            s_isWinRT = 0;
                            break;
                        case 0:     // ERROR_SUCCESS
                        case 122:   // ERROR_INSUFFICIENT_BUFFER
                                    // Success is actually insufficent buffer as we're really only looking for
                                    // not NO_APPLICATION and we're not actually giving a buffer here. The
                                    // API will always return NO_APPLICATION if we're not running under a
                                    // WinRT process, no matter what size the buffer is.
                            s_isWinRT = 1;
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
                        s_isWinRT = 0;
                    }
                    else
                    {
                        throw;
                    }
                }

                return s_isWinRT == 1;
            }
        }

        public static bool IsNotWinRT => !IsWinRT;
        public static bool IsWinRTSupported => IsWinRT || (IsWindows && !IsWindows7);
        public static bool IsNotWinRTSupported => !IsWinRTSupported;

        private static Lazy<bool> m_isWindowsSubsystemForLinux = new Lazy<bool>(GetIsWindowsSubsystemForLinux);

        public static bool IsWindowsSubsystemForLinux => m_isWindowsSubsystemForLinux.Value;
        public static bool IsNotWindowsSubsystemForLinux => !IsWindowsSubsystemForLinux;

        public static bool IsNotFedoraOrRedHatOrCentos => !IsDistroAndVersion("fedora") && !IsDistroAndVersion("rhel") && !IsDistroAndVersion("centos");

        public static bool IsFedora => IsDistroAndVersion("fedora");

        private static bool GetIsWindowsSubsystemForLinux()
        {
            // https://github.com/Microsoft/BashOnWindows/issues/423#issuecomment-221627364

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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

        public static bool IsDebian => IsDistroAndVersion("debian");
        public static bool IsDebian8 => IsDistroAndVersion("debian", "8");
        public static bool IsUbuntu1404 => IsDistroAndVersion("ubuntu", "14.04");
        public static bool IsCentos7 => IsDistroAndVersion("centos", "7");
        public static bool IsTizen => IsDistroAndVersion("tizen");

        // If we need this long-term hopefully we can come up with a better detection than the kernel verison.
        public static bool IsMacOsHighSierra { get; } =
            IsOSX && RuntimeInformation.OSDescription.StartsWith("Darwin 17.0.0");

        /// <summary>
        /// Get whether the OS platform matches the given Linux distro and optional version.
        /// </summary>
        /// <param name="distroId">The distribution id.</param>
        /// <param name="versionId">The distro version.  If omitted, compares the distro only.</param>
        /// <returns>Whether the OS platform matches the given Linux distro and optional version.</returns>
        private static bool IsDistroAndVersion(string distroId, string versionId = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                DistroInfo v = ParseOsReleaseFile();
                if (v.Id == distroId && (versionId == null || v.VersionId == versionId))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetDistroVersionString()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "";
            }

            DistroInfo v = ParseOsReleaseFile();

            return "Distro=" + v.Id + " VersionId=" + v.VersionId + " Pretty=" + v.PrettyName + " Version=" + v.Version;
        }

        private static DistroInfo ParseOsReleaseFile()
        {
            Debug.Assert(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            DistroInfo ret = new DistroInfo();
            ret.Id = "";
            ret.VersionId = "";
            ret.Version = "";
            ret.PrettyName = "";

            if (File.Exists("/etc/os-release"))
            {
                foreach (string line in File.ReadLines("/etc/os-release"))
                {
                    if (line.StartsWith("ID=", System.StringComparison.Ordinal))
                    {
                        ret.Id = RemoveQuotes(line.Substring("ID=".Length));
                    }
                    else if (line.StartsWith("VERSION_ID=", System.StringComparison.Ordinal))
                    {
                        ret.VersionId = RemoveQuotes(line.Substring("VERSION_ID=".Length));
                    }
                    else if (line.StartsWith("VERSION=", System.StringComparison.Ordinal))
                    {
                        ret.Version = RemoveQuotes(line.Substring("VERSION=".Length));
                    }
                    else if (line.StartsWith("PRETTY_NAME=", System.StringComparison.Ordinal))
                    {
                        ret.PrettyName = RemoveQuotes(line.Substring("PRETTY_NAME=".Length));
                    }
                }
            }

            return ret;
        }

        private static string RemoveQuotes(string s)
        {
            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                // Remove quotes.
                s = s.Substring(1, s.Length - 2);
            }

            return s;
        }

        private struct DistroInfo
        {
            public string Id { get; set; }
            public string VersionId { get; set; }
            public string Version { get; set; }
            public string PrettyName { get; set; }
        }

        private static int GetWindowsVersion()
        {
            if (IsWindows)
            {
                RTL_OSVERSIONINFOEX osvi = new RTL_OSVERSIONINFOEX();
                osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
                Assert.Equal(0, RtlGetVersion(out osvi));
                return (int)osvi.dwMajorVersion;
            }

            return -1;
        }

        private static int GetWindowsMinorVersion()
        {
            if (IsWindows)
            {
                RTL_OSVERSIONINFOEX osvi = new RTL_OSVERSIONINFOEX();
                osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
                Assert.Equal(0, RtlGetVersion(out osvi));
                return (int)osvi.dwMinorVersion;
            }

            return -1;
        }

        private static int GetWindowsBuildNumber()
        {
            if (IsWindows)
            {
                RTL_OSVERSIONINFOEX osvi = new RTL_OSVERSIONINFOEX();
                osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
                Assert.Equal(0, RtlGetVersion(out osvi));
                return (int)osvi.dwBuildNumber;
            }

            return -1;
        }

        private const int PRODUCT_IOTUAP = 0x0000007B;
        private const int PRODUCT_IOTUAPCOMMERCIAL = 0x00000083;

        [DllImport("kernel32.dll", SetLastError = false)]
        private static extern bool GetProductInfo(
            int dwOSMajorVersion,
            int dwOSMinorVersion,
            int dwSpMajorVersion,
            int dwSpMinorVersion,
            out int pdwReturnedProductType
        );

        [DllImport("ntdll.dll")]
        private static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

        [StructLayout(LayoutKind.Sequential)]
        private struct RTL_OSVERSIONINFOEX
        {
            internal uint dwOSVersionInfoSize;
            internal uint dwMajorVersion;
            internal uint dwMinorVersion;
            internal uint dwBuildNumber;
            internal uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string szCSDVersion;
        }

        private static int s_isWindowsElevated = -1;

        public static bool IsWindowsAndElevated
        {
            get
            {
                if (s_isWindowsElevated != -1)
                    return s_isWindowsElevated == 1;

                if (!IsWindows || IsWinRT)
                {
                    s_isWindowsElevated = 0;
                    return false;
                }

                IntPtr processToken;
                Assert.True(OpenProcessToken(GetCurrentProcess(), TOKEN_READ, out processToken));

                try
                {
                    uint tokenInfo;
                    uint returnLength;
                    Assert.True(GetTokenInformation(
                        processToken, TokenElevation, out tokenInfo, sizeof(uint), out returnLength));

                    s_isWindowsElevated = tokenInfo == 0 ? 0 : 1;
                }
                finally
                {
                    CloseHandle(processToken);
                }

                return s_isWindowsElevated == 1;
            }
        }

        private const uint TokenElevation = 20;
        private const uint STANDARD_RIGHTS_READ = 0x00020000;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY;

        [DllImport("advapi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            uint TokenInformationClass,
            out uint TokenInformation,
            uint TokenInformationLength,
            out uint ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool CloseHandle(
            IntPtr handle);

        [DllImport("advapi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            uint DesiredAccesss,
            out IntPtr TokenHandle);

        // The process handle does NOT need closing
        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern int GetCurrentApplicationUserModelId(
            ref uint applicationUserModelIdLength,
            byte[] applicationUserModelId);

        public static bool IsNonZeroLowerBoundArraySupported
        {
            get
            {
                if (s_lazyNonZeroLowerBoundArraySupported == null)
                {
                    bool nonZeroLowerBoundArraysSupported = false;
                    try
                    {
                        Array.CreateInstance(typeof(int), new int[] { 5 }, new int[] { 5 });
                        nonZeroLowerBoundArraysSupported = true;
                    }
                    catch (PlatformNotSupportedException)
                    {
                    }
                    s_lazyNonZeroLowerBoundArraySupported = Tuple.Create<bool>(nonZeroLowerBoundArraysSupported);
                }
                return s_lazyNonZeroLowerBoundArraySupported.Item1;
            }
        }

        private static volatile Tuple<bool> s_lazyNonZeroLowerBoundArraySupported;

        public static bool IsReflectionEmitSupported = !PlatformDetection.IsNetNative;

        // Tracked in: https://github.com/dotnet/corert/issues/3643 in case we change our mind about this.
        public static bool IsInvokingStaticConstructorsSupported => !PlatformDetection.IsNetNative;

        // System.Security.Cryptography.Xml.XmlDsigXsltTransform.GetOutput() relies on XslCompiledTransform which relies
        // heavily on Reflection.Emit
        public static bool IsXmlDsigXsltTransformSupported => !PlatformDetection.IsUap;

        public static Range[] FrameworkRanges => new Range[]{
          new Range(new Version(4, 7, 2500, 0), null, new Version(4, 7, 1)),
          new Range(new Version(4, 6, 2000, 0), new Version(4, 7, 2090, 0), new Version(4, 7, 0)),
          new Range(new Version(4, 6, 1500, 0), new Version(4, 6, 1999, 0), new Version(4, 6, 2)),
          new Range(new Version(4, 6, 1000, 0), new Version(4, 6, 1499, 0), new Version(4, 6, 1)),
          new Range(new Version(4, 6, 55, 0), new Version(4, 6, 999, 0), new Version(4, 6, 0)),
          new Range(new Version(4, 0, 30319, 0), new Version(4, 0, 52313, 36313), new Version(4, 5, 2))
        };

        public class Range
        {
            public Version Start { get; private set; }
            public Version Finish { get; private set; }
            public Version FrameworkVersion { get; private set; }

            public Range(Version start, Version finish, Version frameworkVersion)
            {
                Start = start;
                Finish = finish;
                FrameworkVersion = frameworkVersion;
            }

            public bool IsInRange(Version version)
            {
                return version >= Start && (Finish == null || version <= Finish);
            }
        }
    }
}
