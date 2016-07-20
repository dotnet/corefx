// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Runtime.Augments;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    public static partial class Environment
    {
        public static int ExitCode { get { return EnvironmentAugments.ExitCode; } set { EnvironmentAugments.ExitCode = value; } }

        private static string ExpandEnvironmentVariablesCore(string name)
        {
            int currentSize = 100;
            StringBuilder result = StringBuilderCache.Acquire(currentSize); // A somewhat reasonable default size

            result.Length = 0;
            int size = Interop.mincore.ExpandEnvironmentStringsW(name, result, currentSize);
            if (size == 0)
            {
                StringBuilderCache.Release(result);
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            while (size > currentSize)
            {
                currentSize = size;
                result.Capacity = currentSize;
                result.Length = 0;

                size = Interop.mincore.ExpandEnvironmentStringsW(name, result, currentSize);
                if (size == 0)
                {
                    StringBuilderCache.Release(result);
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }

            return StringBuilderCache.GetStringAndRelease(result);
        }

        private static string GetEnvironmentVariableCore(string variable)
        {
            StringBuilder sb = StringBuilderCache.Acquire(128); // a somewhat reasonable default size
            int requiredSize = Interop.mincore.GetEnvironmentVariableW(variable, sb, sb.Capacity);
            if (requiredSize == 0 && Marshal.GetLastWin32Error() == Interop.mincore.Errors.ERROR_ENVVAR_NOT_FOUND)
            {
                StringBuilderCache.Release(sb);
                return null;
            }

            while (requiredSize > sb.Capacity)
            {
                sb.Capacity = requiredSize;
                sb.Length = 0;
                requiredSize = Interop.mincore.GetEnvironmentVariableW(variable, sb, sb.Capacity);
            }

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        private static string GetEnvironmentVariableCore(string variable, EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                return GetEnvironmentVariableCore(variable);
            }
            else if (target == EnvironmentVariableTarget.Machine)
            {
                // TODO #8533: Uncomment/fix when registry APIs available
                //using (RegistryKey environmentKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\Environment", false))
                //{
                //    return environmentKey?.GetValue(variable) as string ?? null;
                //}
                return null;
            }
            else
            {
                Debug.Assert(target == EnvironmentVariableTarget.User);

                // TODO #8533: Uncomment/fix when registry APIs available
                //using (RegistryKey environmentKey = Registry.CurrentUser.OpenSubKey("Environment", false))
                //{
                //    return environmentKey?.GetValue(variable) as string ?? null;
                //}
                return null;
            }
        }

        private static IDictionary GetEnvironmentVariablesCore()
        {
            // Format for GetEnvironmentStrings is:
            // (=HiddenVar=value\0 | Variable=value\0)* \0
            // See the description of Environment Blocks in MSDN's
            // CreateProcess page (null-terminated array of null-terminated strings).
            // Note the =HiddenVar's aren't always at the beginning.

            // Copy strings out, parsing into pairs and inserting into the table.
            // The first few environment variable entries start with an '='.
            // The current working directory of every drive (except for those drives
            // you haven't cd'ed into in your DOS window) are stored in the 
            // environment block (as =C:=pwd) and the program's exit code is 
            // as well (=ExitCode=00000000).

            var results = new LowLevelDictionary<string, string>();
            char[] block = GetEnvironmentCharArray();
            for (int i = 0; i < block.Length; i++)
            {
                int startKey = i;

                // Skip to key. On some old OS, the environment block can be corrupted. 
                // Some will not have '=', so we need to check for '\0'. 
                while (block[i] != '=' && block[i] != '\0') i++;
                if (block[i] == '\0') continue;

                // Skip over environment variables starting with '='
                if (i - startKey == 0)
                {
                    while (block[i] != 0) i++;
                    continue;
                }

                string key = new string(block, startKey, i - startKey);
                i++;  // skip over '='

                int startValue = i;
                while (block[i] != 0) i++; // Read to end of this entry 
                string value = new string(block, startValue, i - startValue); // skip over 0 handled by for loop's i++

                results[key] = value;
            }
            return results;
        }

        private static IDictionary GetEnvironmentVariablesCore(EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                return GetEnvironmentVariablesCore();
            }

            var results = new LowLevelDictionary<string, string>();
            if (target == EnvironmentVariableTarget.Machine)
            {
                // TODO #8533: Uncomment/fix when registry APIs available
                //using (RegistryKey environmentKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\Environment", false))
                //{
                //    return GetRegistryKeyNameValuePairs(environmentKey);
                //}
            }
            else
            {
                Debug.Assert(target == EnvironmentVariableTarget.User);
                // TODO #8533: Uncomment/fix when registry APIs available
                //using (RegistryKey environmentKey = Registry.CurrentUser.OpenSubKey("Environment", false))
                //{
                //    return GetRegistryKeyNameValuePairs(environmentKey);
                //}
            }
            return results;
        }

        // TODO #8533: Uncomment/fix when registry APIs available
        //private static IDictionary GetRegistryKeyNameValuePairs(RegistryKey registryKey)
        //{
        //    Hashtable table = new Hashtable(20);
        //    if (registryKey != null)
        //    {
        //        string[] names = registryKey.GetValueNames();
        //        foreach (string name in names)
        //        {
        //            string value = registryKey.GetValue(name, "").ToString();
        //            table.Add(name, value);
        //        }
        //    }
        //    return table;
        //}

        private unsafe static char[] GetEnvironmentCharArray()
        {
            // Format for GetEnvironmentStrings is:
            // [=HiddenVar=value\0]* [Variable=value\0]* \0
            // See the description of Environment Blocks in MSDN's
            // CreateProcess page (null-terminated array of null-terminated strings).
            char* pStrings = Interop.mincore.GetEnvironmentStringsW();
            if (pStrings == null)
            {
                throw new OutOfMemoryException();
            }
            try
            {
                // Search for terminating \0\0 (two unicode \0's).
                char* p = pStrings;
                while (!(*p == '\0' && *(p + 1) == '\0')) p++;

                var block = new char[(int)(p - pStrings + 1)];
                Marshal.Copy((IntPtr)pStrings, block, 0, block.Length);
                return block;
            }
            finally
            {
                Interop.mincore.FreeEnvironmentStringsW(pStrings); // ignore any cleanup error
            }
        }

        private static string GetFolderPathCore(SpecialFolder folder, SpecialFolderOption option)
        {
            switch (folder)
            {
                case SpecialFolder.System:
                    return SystemDirectory;
                default:
                    // TODO: SHGetFolderPath is not available in the approved API list
                    throw new PlatformNotSupportedException();
            }
        }

        private static bool Is64BitOperatingSystemWhen32BitProcess
        {
            get
            {
                bool isWow64;
                return Interop.mincore.IsWow64Process(Interop.mincore.GetCurrentProcess(), out isWow64) && isWow64;
            }
        }

        public static string MachineName
        {
            get
            {
                string name = Interop.mincore.GetComputerName();
                if (name == null)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_ComputerName);
                }
                return name;
            }
        }

        private static unsafe Lazy<OperatingSystem> s_osVersion = new Lazy<OperatingSystem>(() =>
        {
            var version = new Interop.mincore.OSVERSIONINFOEX { dwOSVersionInfoSize = sizeof(Interop.mincore.OSVERSIONINFOEX) };
            if (!Interop.mincore.GetVersionExW(ref version))
            {
                throw new InvalidOperationException(SR.InvalidOperation_GetVersion);
            }

            return new OperatingSystem(
                PlatformID.Win32NT,
                new Version(version.dwMajorVersion, version.dwMinorVersion, version.dwBuildNumber, (version.wServicePackMajor << 16) | version.wServicePackMinor),
                Marshal.PtrToStringUni((IntPtr)version.szCSDVersion));
        });

        private static unsafe int ProcessorCountCore
        {
            get
            {
                // Determine how much size we need for a call to GetLogicalProcessorInformationEx
                uint len = 0;
                if (!Interop.mincore.GetLogicalProcessorInformationEx(Interop.mincore.LOGICAL_PROCESSOR_RELATIONSHIP.RelationGroup, IntPtr.Zero, ref len) &&
                    Marshal.GetLastWin32Error() == Interop.mincore.Errors.ERROR_INSUFFICIENT_BUFFER)
                {
                    // Allocate that much space
                    Debug.Assert(len > 0);
                    var buffer = new byte[len];
                    fixed (byte* bufferPtr = buffer)
                    {
                        // Call GetLogicalProcessorInformationEx with the allocated buffer
                        if (Interop.mincore.GetLogicalProcessorInformationEx(Interop.mincore.LOGICAL_PROCESSOR_RELATIONSHIP.RelationGroup, (IntPtr)bufferPtr, ref len))
                        {
                            // Walk each SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX in the buffer, where the Size of each dictates how
                            // much space it's consuming.  For each group relation, count the number of active processors in each of its group infos.
                            int processorCount = 0;
                            byte* ptr = bufferPtr, endPtr = bufferPtr + len;
                            while (ptr < endPtr)
                            {
                                var current = (Interop.mincore.SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX*)ptr;
                                if (current->Relationship == Interop.mincore.LOGICAL_PROCESSOR_RELATIONSHIP.RelationGroup)
                                {
                                    Interop.mincore.PROCESSOR_GROUP_INFO* groupInfo = &current->Group.GroupInfo;
                                    int groupCount = current->Group.ActiveGroupCount;
                                    for (int i = 0; i < groupCount; i++)
                                    {
                                        processorCount += (groupInfo + i)->ActiveProcessorCount;
                                    }
                                }
                                ptr += current->Size;
                            }
                            return processorCount;
                        }
                    }
                }

                // GetLogicalProcessorInformationEx didn't work for some reason.  Fall back to using GetSystemInfo.
                return ProcessorCountFromSystemInfo;
            }
        }

        private static void SetEnvironmentVariableCore(string variable, string value)
        {
            if (!Interop.mincore.SetEnvironmentVariableW(variable, value))
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case Interop.mincore.Errors.ERROR_ENVVAR_NOT_FOUND: // Allow user to try to clear a environment variable                
                        return;
                    case Interop.mincore.Errors.ERROR_FILENAME_EXCED_RANGE: // Fix inaccurate error code from Win32
                        throw new ArgumentException(SR.Argument_LongEnvVarValue, nameof(value));
                    default:
                        throw new ArgumentException(Interop.mincore.GetMessage(errorCode));
                }
            }
        }

        private static void SetEnvironmentVariableCore(string variable, string value, EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                SetEnvironmentVariableCore(variable, value);
            }
            else if (target == EnvironmentVariableTarget.Machine)
            {
                // TODO #8533: Uncomment/fix when registry APIs available
                //using (RegistryKey environmentKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Session Manager\Environment", true))
                //{
                //    if (environmentKey != null)
                //    {
                //        if (value == null) environmentKey.DeleteValue(variable, false);
                //        else environmentKey.SetValue(variable, value);
                //    }
                //}
            }
            else
            {
                Debug.Assert(target == EnvironmentVariableTarget.User);

                // User-wide environment variables stored in the registry are limited to 255 chars for the environment variable name.
                const int MaxUserEnvVariableLength = 255;
                if (variable.Length >= MaxUserEnvVariableLength)
                {
                    throw new ArgumentException(SR.Argument_LongEnvVarValue, nameof(variable));
                }

                // TODO #8533: Uncomment/fix when registry APIs available
                //using (RegistryKey environmentKey = Registry.CurrentUser.OpenSubKey("Environment", true))
                //{
                //    if (environmentKey != null)
                //    {
                //        if (value == null) environmentKey.DeleteValue(variable, false);
                //        else environmentKey.SetValue(variable, value);
                //    }
                //}
            }

            //// Desktop sends a WM_SETTINGCHANGE message to all windows.  Not available on all platforms.
            //Interop.mincore.SendMessageTimeout(
            //    new IntPtr(Interop.mincore.HWND_BROADCAST), Interop.mincore.WM_SETTINGCHANGE,
            //    IntPtr.Zero, "Environment", 0, 1000, IntPtr.Zero);
        }

        public static string SystemDirectory
        {
            get
            {
                StringBuilder sb = StringBuilderCache.Acquire(Path.MaxPath);
                if (Interop.mincore.GetSystemDirectoryW(sb, Path.MaxPath) == 0)
                {
                    StringBuilderCache.Release(sb);
                    throw Win32Marshal.GetExceptionForLastWin32Error();
                }
                return StringBuilderCache.GetStringAndRelease(sb);
            }
        }

        public static string UserName
        {
            get
            {
                // Use GetUserNameExW, as GetUserNameW isn't available on all platforms, e.g. Win7
                var domainName = new StringBuilder(1024);
                uint domainNameLen = (uint)domainName.Capacity;
                if (Interop.mincore.GetUserNameExW(Interop.mincore.NameSamCompatible, domainName, ref domainNameLen) == 1)
                {
                    string samName = domainName.ToString();
                    int index = samName.IndexOf('\\');
                    if (index != -1)
                    {
                        return samName.Substring(index + 1);
                    }
                }

                return string.Empty;
            }
        }

        public static string UserDomainName
        {
            get
            {
                var domainName = new StringBuilder(1024);
                uint domainNameLen = (uint)domainName.Capacity;
                if (Interop.mincore.GetUserNameExW(Interop.mincore.NameSamCompatible, domainName, ref domainNameLen) == 1)
                {
                    string samName = domainName.ToString();
                    int index = samName.IndexOf('\\');
                    if (index != -1)
                    {
                        return samName.Substring(0, index);
                    }
                }
                domainNameLen = (uint)domainName.Capacity;

                byte[] sid = new byte[1024];
                int sidLen = sid.Length;
                int peUse;
                if (!Interop.mincore.LookupAccountNameW(null, UserName, sid, ref sidLen, domainName, ref domainNameLen, out peUse))
                {
                    throw new InvalidOperationException(Win32Marshal.GetExceptionForLastWin32Error().Message);
                }

                return domainName.ToString();
            }
        }

    }
}
