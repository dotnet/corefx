// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace System
{
    public static partial class PlatformDetection
    {
        //
        // Do not use the " { get; } = <expression> " pattern here. Having all the initialization happen in the type initializer
        // means that one exception anywhere means all tests using PlatformDetection fail. If you feel a value is worth latching,
        // do it in a way that failures don't cascade.
        //

        private static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsCentos6 => IsDistroAndVersion("centos", 6);
        public static bool IsOpenSUSE => IsDistroAndVersion("opensuse");
        public static bool IsUbuntu => IsDistroAndVersion("ubuntu");
        public static bool IsDebian => IsDistroAndVersion("debian");
        public static bool IsAlpine => IsDistroAndVersion("alpine");
        public static bool IsDebian8 => IsDistroAndVersion("debian", 8);
        public static bool IsDebian10 => IsDistroAndVersion("debian", 10);
        public static bool IsUbuntu1404 => IsDistroAndVersion("ubuntu", 14, 4);
        public static bool IsUbuntu1604 => IsDistroAndVersion("ubuntu", 16, 4);
        public static bool IsUbuntu1704 => IsDistroAndVersion("ubuntu", 17, 4);
        public static bool IsUbuntu1710 => IsDistroAndVersion("ubuntu", 17, 10);
        public static bool IsUbuntu1710OrHigher => IsDistroAndVersionOrHigher("ubuntu", 17, 10);
        public static bool IsUbuntu1804 => IsDistroAndVersion("ubuntu", 18, 04);
        public static bool IsUbuntu1810OrHigher => IsDistroAndVersionOrHigher("ubuntu", 18, 10);
        public static bool IsTizen => IsDistroAndVersion("tizen");
        public static bool IsFedora => IsDistroAndVersion("fedora");

        // OSX family
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsNotOSX => !IsOSX;
        public static Version OSXVersion => IsOSX ?
            ToVersion(Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemVersion) :
            throw new PlatformNotSupportedException();
        private static Lazy<Version> m_osxProductVersion = new Lazy<Version>(GetOSXProductVersion);
        public static bool IsMacOsHighSierraOrHigher => IsOSX && (m_osxProductVersion.Value.Major > 10 || (m_osxProductVersion.Value.Major == 10 && m_osxProductVersion.Value.Minor >= 13));
        public static bool IsNotMacOsHighSierraOrHigher => !IsMacOsHighSierraOrHigher;
        public static bool IsMacOsMojaveOrHigher => IsOSX && (m_osxProductVersion.Value.Major > 10 || (m_osxProductVersion.Value.Major == 10 && m_osxProductVersion.Value.Minor >= 14));
        public static bool IsMacOsCatalinaOrHigher => IsOSX && (m_osxProductVersion.Value.Major > 10 || (m_osxProductVersion.Value.Major == 10 && m_osxProductVersion.Value.Minor >= 15));

        // RedHat family covers RedHat and CentOS
        public static bool IsRedHatFamily => IsRedHatFamilyAndVersion();
        public static bool IsNotRedHatFamily => !IsRedHatFamily;
        public static bool IsRedHatFamily6 => IsRedHatFamilyAndVersion(6);
        public static bool IsNotRedHatFamily6 => !IsRedHatFamily6;
        public static bool IsRedHatFamily7 => IsRedHatFamilyAndVersion(7);
        public static bool IsNotFedoraOrRedHatFamily => !IsFedora && !IsRedHatFamily;
        public static bool IsNotDebian10 => !IsDebian10;

        private static Lazy<Version> m_icuVersion = new Lazy<Version>(GetICUVersion);
        public static Version ICUVersion => m_icuVersion.Value;

        public static bool IsSuperUser => !IsWindows ?
            libc.geteuid() == 0 :
            throw new PlatformNotSupportedException();

        public static Version OpenSslVersion => !IsOSX && !IsWindows ?
            GetOpenSslVersion() :
            throw new PlatformNotSupportedException();

        /// <summary>
        /// If gnulibc is available, returns the release, such as "stable".
        /// Otherwise returns "glibc_not_found".
        /// </summary>
        public static string LibcRelease
        {
            get
            {
                if (IsWindows)
                {
                    return "glibc_not_found";
                }

                try
                {
                    return Marshal.PtrToStringAnsi(libc.gnu_get_libc_release());
                }
                catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException)
                {
                    return "glibc_not_found";
                }
            }
        }

        /// <summary>
        /// If gnulibc is available, returns the version, such as "2.22".
        /// Otherwise returns "glibc_not_found". (In future could run "ldd -version" for musl)
        /// </summary>
        public static string LibcVersion
        {
            get
            {
                if (IsWindows)
                {
                    return "glibc_not_found";
                }

                try
                {
                    return Marshal.PtrToStringAnsi(libc.gnu_get_libc_version());
                }
                catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException)
                {
                    return "glibc_not_found";
                }
            }
        }

        private static Version GetICUVersion()
        {
            if (IsWindows)
            {
                return new Version(0, 0, 0, 0);
            }
            else
            {
                int ver = libc.GlobalizationNative_GetICUVersion();
                return new Version( ver & 0xFF,
                                (ver >> 8)  & 0xFF,
                                (ver >> 16) & 0xFF,
                                    ver >> 24);
            }
        }

        private static Version GetOSXProductVersion()
        {
            if (IsOSX)
            {
                try
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

                    XElement dict = XDocument.Load("/System/Library/CoreServices/SystemVersion.plist").Root.Element("dict");
                    if (dict != null)
                    {
                        foreach (XElement key in dict.Elements("key"))
                        {
                            if ("ProductVersion".Equals(key.Value))
                            {
                                XElement stringElement = key.NextNode as XElement;
                                if (stringElement != null && stringElement.Name.LocalName.Equals("string"))
                                {
                                    string versionString = stringElement.Value;
                                    if (versionString != null)
                                    {
                                        return Version.Parse(versionString);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            // In case of exception, couldn't get the version or non osx
            return new Version(0, 0, 0);
        }

        private static Version s_opensslVersion;
        private static Version GetOpenSslVersion()
        {
            if (s_opensslVersion == null)
            {
                // OpenSSL version numbers are encoded as
                // 0xMNNFFPPS: major (one nybble), minor (one byte, unaligned),
                // "fix" (one byte, unaligned), patch (one byte, unaligned), status (one nybble)
                //
                // e.g. 1.0.2a final is 0x1000201F
                //
                // Currently they don't exceed 29-bit values, but we use long here to account
                // for the expanded range on their 64-bit C-long return value.
                long versionNumber = Interop.OpenSsl.OpenSslVersionNumber();
                int major = (int)((versionNumber >> 28) & 0xF);
                int minor = (int)((versionNumber >> 20) & 0xFF);
                int fix = (int)((versionNumber >> 12) & 0xFF);

                s_opensslVersion = new Version(major, minor, fix);
            }

            return s_opensslVersion;
        }

        private static Version ToVersion(string versionString)
        {
            // In some distros/versions we cannot discover the distro version; return something valid.
            // Pick a high version number, since this seems to happen on newer distros.
            if (string.IsNullOrEmpty(versionString))
            {
                versionString = new Version(Int32.MaxValue, Int32.MaxValue).ToString();
            }

            try
            {
                if (versionString.IndexOf('.') != -1)
                    return new Version(versionString);

                // minor version is required by Version
                // let's default it to 0
                return new Version(int.Parse(versionString), 0);
            }
            catch (Exception exc)
            {
                throw new FormatException($"Failed to parse version string: '{versionString}'", exc);
            }
        }

        private static DistroInfo GetDistroInfo() => new DistroInfo()
        {
            Id = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystem,
            VersionId = ToVersion(Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.OperatingSystemVersion)
        };

        private static bool IsRedHatFamilyAndVersion(int major = -1, int minor = -1, int build = -1, int revision = -1)
        {
            return IsDistroAndVersion((distro) => distro == "rhel" || distro == "centos", major, minor, build, revision);
        }

        /// <summary>
        /// Get whether the OS platform matches the given Linux distro and optional version.
        /// </summary>
        /// <param name="distroId">The distribution id.</param>
        /// <param name="major">The distro major version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <param name="minor">The distro minor version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <param name="build">The distro build version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <param name="revision">The distro revision version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <returns>Whether the OS platform matches the given Linux distro and optional version.</returns>
        private static bool IsDistroAndVersion(string distroId, int major = -1, int minor = -1, int build = -1, int revision = -1)
        {
            return IsDistroAndVersion(distro => (distro == distroId), major, minor, build, revision);
        }

        /// <summary>
        /// Get whether the OS platform matches the given Linux distro and optional version is same or higher.
        /// </summary>
        /// <param name="distroId">The distribution id.</param>
        /// <param name="major">The distro major version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <param name="minor">The distro minor version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <param name="build">The distro build version. If omitted, this portion of the version is not included in the comparison.</param>
        /// <param name="revision">The distro revision version.  If omitted, this portion of the version is not included in the comparison.</param>
        /// <returns>Whether the OS platform matches the given Linux distro and optional version is same or higher.</returns>
        private static bool IsDistroAndVersionOrHigher(string distroId, int major = -1, int minor = -1, int build = -1, int revision = -1)
        {
            return IsDistroAndVersionOrHigher(distro => (distro == distroId), major, minor, build, revision);
        }

        private static bool IsDistroAndVersion(Predicate<string> distroPredicate, int major = -1, int minor = -1, int build = -1, int revision = -1)
        {
            if (IsLinux)
            {
                DistroInfo v = GetDistroInfo();
                if (distroPredicate(v.Id) && VersionEquivalentTo(major, minor, build, revision, v.VersionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsDistroAndVersionOrHigher(Predicate<string> distroPredicate, int major = -1, int minor = -1, int build = -1, int revision = -1)
        {
            if (IsLinux)
            {
                DistroInfo v = GetDistroInfo();
                if (distroPredicate(v.Id) && VersionEquivalentToOrHigher(major, minor, build, revision, v.VersionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool VersionEquivalentTo(int major, int minor, int build, int revision, Version actualVersionId)
        {
            return (major == -1 || major == actualVersionId.Major)
                && (minor == -1 || minor == actualVersionId.Minor)
                && (build == -1 || build == actualVersionId.Build)
                && (revision == -1 || revision == actualVersionId.Revision);
        }

        private static bool VersionEquivalentToOrHigher(int major, int minor, int build, int revision, Version actualVersionId)
        {
            return
                VersionEquivalentTo(major, minor, build, revision, actualVersionId) ||
                    (actualVersionId.Major > major ||
                        (actualVersionId.Major == major && (actualVersionId.Minor > minor ||
                            (actualVersionId.Minor == minor && (actualVersionId.Build > build ||
                                (actualVersionId.Build == build && (actualVersionId.Revision > revision ||
                                    (actualVersionId.Revision == revision))))))));
        }

        private struct DistroInfo
        {
            public string Id { get; set; }
            public Version VersionId { get; set; }
        }

        private static class libc
        {
            [DllImport("libc", SetLastError = true)]
            public static extern unsafe uint geteuid();

            [DllImport("libc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gnu_get_libc_release();

            [DllImport("libc", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gnu_get_libc_version();

            [DllImport("System.Globalization.Native", SetLastError = true)]
            public static extern int GlobalizationNative_GetICUVersion();
        }
    }
}
