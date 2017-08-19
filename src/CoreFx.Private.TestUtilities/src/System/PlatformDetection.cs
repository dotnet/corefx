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
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsNetBSD => RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD"));
        public static bool IsNotWindows8x => !IsWindows8x;
        public static bool IsNotWindowsNanoServer => !IsWindowsNanoServer;
        public static bool IsNotWindowsIoTCore => !IsWindowsIoTCore;
        public static bool IsDrawingSupported => (IsNotWindowsNanoServer && IsNotWindowsIoTCore);
        public static bool IsArmProcess => RuntimeInformation.ProcessArchitecture == Architecture.Arm;
        public static bool IsNotArmProcess => !IsArmProcess;

        public static bool IsNotWinRT => !IsWinRT;
        public static bool IsWinRTSupported => IsWinRT || (IsWindows && !IsWindows7);
        public static bool IsNotWinRTSupported => !IsWinRTSupported;
        public static bool IsNotMacOsHighSierraOrHigher => !IsMacOsHighSierraOrHigher;

        // Officially, .Net Native only supports processes running in an AppContainer. However, the majority of tests still work fine 
        // in a normal Win32 process and we often do so as running in an AppContainer imposes a substantial tax in debuggability
        // and investigatability. This predicate is used in ConditionalFacts to disable the specific tests that really need to be
        // running in AppContainer when running on .NetNative.
        public static bool IsNotNetNativeRunningAsConsoleApp => !(IsNetNative && !IsWinRT);

        private static Lazy<bool> m_isWindowsSubsystemForLinux = new Lazy<bool>(GetIsWindowsSubsystemForLinux);

        public static bool IsWindowsSubsystemForLinux => m_isWindowsSubsystemForLinux.Value;
        public static bool IsNotWindowsSubsystemForLinux => !IsWindowsSubsystemForLinux;

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
