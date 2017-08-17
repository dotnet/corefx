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
    public static class PlatformDetection
    {
        public static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsWindows7 { get; } = IsWindows && GetWindowsVersion() == 6 && GetWindowsMinorVersion() == 1;
        public static bool IsOSX { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsNotOSX { get; } = !IsOSX;
        public static bool IsNetBSD { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD"));
        public static bool IsNotWindowsNanoServer { get; } = (IsWindows &&
                File.Exists(Path.Combine(Environment.GetEnvironmentVariable("windir"), "regedit.exe")));

        public static int WindowsVersion { get; } = GetWindowsVersion();

        public static bool IsDebian8 { get; } = IsDistroAndVersion("debian", "8");
        public static bool IsUbuntu1404 { get; } = IsDistroAndVersion("ubuntu", "14.04");
        public static bool IsUbuntu1510 { get; } = IsDistroAndVersion("ubuntu", "15.10");
        public static bool IsUbuntu1604 { get; } = IsDistroAndVersion("ubuntu", "16.04");
        public static bool IsFedora23 { get; } = IsDistroAndVersion("fedora", "23");

        private static bool IsDistroAndVersion(string distroId, string versionId)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                IdVersionPair v = ParseOsReleaseFile();
                if (v.Id == distroId && v.VersionId == versionId)
                {
                    return true;
                }
            }

            return false;
        }

        public static Version OSXKernelVersion { get; } = GetOSXKernelVersion();

        private static Version GetOSXProductVersion()
        {
            try
            {
                if (IsOSX)
                {
                    // <plist version="1.0">
                    // <dict>
                    //         <key>ProductBuildVersion</key>
                    //         <string>17A330h</string>
                    //         <key>ProductCopyright</key>
                    //         <string>1983-2017 Apple Inc.</string>
                    //         <key>ProductName</key>
                    //         <string>Mac OS X</string>
                    //         <key>ProductUserVisibleVersion</key>
                    //         <string>10.13</string>
                    //         <key>ProductVersion</key>
                    //         <string>10.13</string>
                    // </dict>
                    // </plist>

                    // not using Xml parsers here to avoid having every test project using PlatformDetection 
                    // to include a reference to the Xml assemblies

                    string s = File.ReadAllText("/System/Library/CoreServices/SystemVersion.plist");
                    int index = s.IndexOf(@"ProductVersion", StringComparison.OrdinalIgnoreCase);
                    if (index > 0)
                    {
                        index = s.IndexOf(@"<string>", index + 14, StringComparison.OrdinalIgnoreCase);
                        if (index > 0)
                        {
                            int endIndex = s.IndexOf(@"</string>", index + 8, StringComparison.OrdinalIgnoreCase);
                            if (endIndex > 0)
                            {
                                string versionString = s.Substring(index + 8, endIndex - index - 8);
                                return Version.Parse(versionString);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            // In case of exception or couldn't get the version 
            return new Version(0, 0, 0);
        }

        private static IdVersionPair ParseOsReleaseFile()
        {
            Debug.Assert(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));

            IdVersionPair ret = new IdVersionPair();
            ret.Id = "";
            ret.VersionId = "";

            if (File.Exists("/etc/os-release"))
            {
                foreach (string line in File.ReadLines("/etc/os-release"))
                {
                    if (line.StartsWith("ID=", System.StringComparison.Ordinal))
                    {
                        ret.Id = line.Substring("ID=".Length);
                    }
                    else if (line.StartsWith("VERSION_ID=", System.StringComparison.Ordinal))
                    {
                        ret.VersionId = line.Substring("VERSION_ID=".Length);
                    }
                }
            }

            string versionId = ret.VersionId;
            if (versionId.Length >= 2 && versionId[0] == '"' && versionId[versionId.Length - 1] == '"')
            {
                // Remove Quotes.
                ret.VersionId = versionId.Substring(1, versionId.Length - 2);
            }

            return ret;
        }

        private struct IdVersionPair
        {
            public string Id { get; set; }
            public string VersionId { get; set; }
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

        private static Version GetOSXKernelVersion()
        {
            if (IsOSX)
            {
                byte[] bytes = new byte[256];
                IntPtr bytesLength = new IntPtr(bytes.Length);
                Assert.Equal(0, sysctlbyname("kern.osrelease", bytes, ref bytesLength, null, IntPtr.Zero));
                string versionString = Encoding.UTF8.GetString(bytes);
                return Version.Parse(versionString);
            }

            return new Version(0, 0, 0);
        }

        private static readonly Version s_osxProductVersion = GetOSXProductVersion();

        public static bool IsMacOsHighSierraOrHigher { get; } =
            IsOSX && (s_osxProductVersion.Major > 10 || (s_osxProductVersion.Major == 10 && s_osxProductVersion.Minor >= 13));

        public static bool IsNotMacOsHighSierraOrHigher { get; } = !IsMacOsHighSierraOrHigher;

        [DllImport("libc", SetLastError = true)]
        private static extern int sysctlbyname(string ctlName, byte[] oldp, ref IntPtr oldpLen, byte[] newp, IntPtr newpLen);

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
    }
}
