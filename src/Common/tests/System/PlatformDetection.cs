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

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsWindows7 => IsWindows && GetWindowsVersion() == 6 && GetWindowsMinorVersion() == 1;
        public static bool IsWindows8x => IsWindows && GetWindowsVersion() == 6 && (GetWindowsMinorVersion() == 2 || GetWindowsMinorVersion() == 3);
        public static bool IsNotWindows8x => !IsWindows8x;
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsNetBSD => RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD"));
        public static bool IsOpenSUSE => IsDistroAndVersion("opensuse");
        public static bool IsUbuntu => IsDistroAndVersion("ubuntu");
        public static bool IsWindowsNanoServer => (IsWindows && !File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "regedit.exe")));
        public static bool IsNotWindowsNanoServer => !IsWindowsNanoServer;
        public static bool IsWindows10Version1607OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 14393;
        public static bool IsWindows10Version1703OrGreater => IsWindows &&
            GetWindowsVersion() == 10 && GetWindowsMinorVersion() == 0 && GetWindowsBuildNumber() >= 15063;

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

        private static Lazy<bool> m_isWindowsSubsystemForLinux = new Lazy<bool>(GetIsWindowsSubsystemForLinux);

        public static bool IsWindowsSubsystemForLinux => m_isWindowsSubsystemForLinux.Value;
        public static bool IsNotWindowsSubsystemForLinux => !IsWindowsSubsystemForLinux;

        public static bool IsFedora => IsDistroAndVersion("fedora");

        // RedHat family covers RedHat and CentOS
        public static bool IsRedHatFamily => IsRedHatFamilyAndVersion();
        public static bool IsRedHatFamily7 => IsRedHatFamilyAndVersion("7");
        public static bool IsRedHatFamily69 => IsRedHatFamilyAndVersion("6.9");
        public static bool IsNotRedHatFamily69 => !IsRedHatFamily69;
        public static bool IsNotFedoraOrRedHatFamily => !IsFedora && !IsRedHatFamily;

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

        private static readonly Version s_osxProductVersion = GetOSXProductVersion();
 
        public static bool IsMacOsHighSierraOrHigher { get; } =
            IsOSX && (s_osxProductVersion.Major > 10 || (s_osxProductVersion.Major == 10 && s_osxProductVersion.Minor >= 13));
 
        public static bool IsNotMacOsHighSierraOrHigher { get; } = !IsMacOsHighSierraOrHigher;

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
                IdVersionPair v = ParseOsReleaseFile();
                if (v.Id == distroId && (versionId == null || v.VersionId == versionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsRedHatFamilyAndVersion(string versionId = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                IdVersionPair v = ParseOsReleaseFile();

                // RedHat includes minor version. We need to account for that when comparing
                if ((v.Id == "rhel" || v.Id == "centos") && VersionEqual(versionId, v.VersionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VersionEqual(string expectedVersionId, string actualVersionId)
        {
            if (expectedVersionId == null)
            {
                return true;
            }

            string[] expected = expectedVersionId.Split('.');
            string[] actual = actualVersionId.Split('.');

            if (expected.Length > actual.Length)
            {
                return false;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    return false;
                }
            }

            return true;
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
                        ret.Id = RemoveQuotes(line.Substring("ID=".Length));
                    }
                    else if (line.StartsWith("VERSION_ID=", System.StringComparison.Ordinal))
                    {
                        ret.VersionId = RemoveQuotes(line.Substring("VERSION_ID=".Length));
                    }
                }
            }
            else 
            {
                string fileName = null;
                if (File.Exists("/etc/redhat-release"))
                    fileName = "/etc/redhat-release";

                if (fileName == null && File.Exists("/etc/system-release"))
                    fileName = "/etc/system-release";
                
                if (fileName != null)
                {
                    // Parse the format like the following line:
                    // Red Hat Enterprise Linux Server release 7.3 (Maipo)
                    using (StreamReader file = new StreamReader(fileName))
                    {
                        string line = file.ReadLine();
                        if (!String.IsNullOrEmpty(line))
                        {
                            if (line.StartsWith("Red Hat Enterprise Linux", StringComparison.OrdinalIgnoreCase))
                            {
                                ret.Id = "rhel";
                            }
                            else if (line.StartsWith("Centos", StringComparison.OrdinalIgnoreCase))
                            {
                                ret.Id = "centos";
                            }
                            else 
                            {
                                // automatically generate the distro label
                                string [] words = line.Split(' ');
                                StringBuilder sb = new StringBuilder();

                                foreach (string word in words)
                                {
                                    if (word.Length > 0)
                                    {
                                        if (Char.IsNumber(word[0]) || 
                                            word.Equals("release", StringComparison.OrdinalIgnoreCase) ||
                                            word.Equals("server", StringComparison.OrdinalIgnoreCase))
                                            {
                                                break;
                                            }
                                        sb.Append(Char.ToLower(word[0]));
                                    }
                                }
                                ret.Id = sb.ToString();
                            }

                            int i = 0;
                            while (i < line.Length && !Char.IsNumber(line[i])) // stop at first number
                                i++;

                            if (i < line.Length)
                            {
                                int j = i + 1;
                                while (j < line.Length && (Char.IsNumber(line[j]) || line[j] == '.'))
                                    j++;

                                ret.VersionId = line.Substring(i, j - i);
                            }
                        }
                    }
                }
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

        // System.Security.Cryptography.Xml.XmlDsigXsltTransform.GetOutput() relies on XslCompiledTransform which relies
        // heavily on Reflection.Emit
        public static bool IsXmlDsigXsltTransformSupported => PlatformDetection.IsReflectionEmitSupported;

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
