// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System
{
    public static partial class Environment
    {
        private static Lazy<OperatingSystem> s_osVersion = PlatformHelper.IsWin32 ? EnvironmentWin32.s_osVersion : PlatformHelper.IsUnix ? EnvironmentUnix.s_osVersion : throw new PlatformNotSupportedException();

        internal static readonly bool IsMac = PlatformHelper.IsUnix ? EnvironmentUnix.IsMac : throw new PlatformNotSupportedException();

        private static bool Is64BitOperatingSystemWhen32BitProcess
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.Is64BitOperatingSystemWhen32BitProcess : PlatformHelper.IsUnix ? EnvironmentUnix.Is64BitOperatingSystemWhen32BitProcess : throw new PlatformNotSupportedException();
        }

        private static string CurrentDirectoryCore
        {
            get => PlatformHelper.IsWindows ? EnvironmentWindows.CurrentDirectoryCore : PlatformHelper.IsUnix ? EnvironmentUnix.CurrentDirectoryCore : throw new PlatformNotSupportedException();
            set
            {
                if (PlatformHelper.IsWindows)
                {
                    EnvironmentWindows.CurrentDirectoryCore = value;
                    return;
                }
                else if (PlatformHelper.IsUnix)
                {
                    EnvironmentUnix.CurrentDirectoryCore = value;
                    return;
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }
        }

        public static int ExitCode
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.ExitCode : PlatformHelper.IsUnix ? EnvironmentUnix.ExitCode : throw new PlatformNotSupportedException();
            set
            {
                if (PlatformHelper.IsWin32)
                {
                    EnvironmentWin32.ExitCode = value;
                    return;
                }
                else if (PlatformHelper.IsUnix)
                {
                    EnvironmentUnix.ExitCode = value;
                    return;
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }
        }

        public static int ProcessorCount
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.ProcessorCount : PlatformHelper.IsUnix ? EnvironmentUnix.ProcessorCount : throw new PlatformNotSupportedException();
        }

        public static int SystemPageSize
        {
            get => PlatformHelper.IsWindows ? EnvironmentWindows.SystemPageSize : PlatformHelper.IsUnix ? EnvironmentUnix.SystemPageSize : throw new PlatformNotSupportedException();
        }

        public static string MachineName
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.MachineName : PlatformHelper.IsUnix ? EnvironmentUnix.MachineName : throw new PlatformNotSupportedException();
        }

        public static string NewLine
        {
            get => PlatformHelper.IsWindows ? EnvironmentWindows.NewLine : PlatformHelper.IsUnix ? EnvironmentUnix.NewLine : throw new PlatformNotSupportedException();
        }

        public static string SystemDirectory
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.SystemDirectory : PlatformHelper.IsUnix ? EnvironmentUnix.SystemDirectory : throw new PlatformNotSupportedException();
        }

        public static string UserDomainName
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.UserDomainName : PlatformHelper.IsUnix ? EnvironmentUnix.UserDomainName : throw new PlatformNotSupportedException();
        }

        public static string UserName
        {
            get => PlatformHelper.IsWin32 ? EnvironmentWin32.UserName : PlatformHelper.IsUnix ? EnvironmentUnix.UserName : throw new PlatformNotSupportedException();
        }

        private static string ExpandEnvironmentVariablesCore(string name)
        {
            if (PlatformHelper.IsWin32)
            {
                return EnvironmentWin32.ExpandEnvironmentVariablesCore(name);
            }
            else if (PlatformHelper.IsUnix)
            {
                return EnvironmentUnix.ExpandEnvironmentVariablesCore(name);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        private static string GetFolderPathCore(SpecialFolder folder, SpecialFolderOption option)
        {
            if (PlatformHelper.IsWin32)
            {
                return EnvironmentWin32.GetFolderPathCore(folder, option);
            }
            else if (PlatformHelper.IsUnix)
            {
                return EnvironmentUnix.GetFolderPathCore(folder, option);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static string[] GetLogicalDrives()
        {
            if (PlatformHelper.IsWindows)
            {
                return EnvironmentWindows.GetLogicalDrives();
            }
            else if (PlatformHelper.IsUnix)
            {
                return EnvironmentUnix.GetLogicalDrives();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
