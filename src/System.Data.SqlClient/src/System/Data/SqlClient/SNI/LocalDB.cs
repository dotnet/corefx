// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Data.SqlClient.SNI
{
    internal class LocalDB
    {
        public static readonly LocalDB Singleton = new LocalDB();

        //HKEY_LOCAL_MACHINE
        private const string LocalDBInstalledVersionRegistryKey = "SOFTWARE\\Microsoft\\Microsoft SQL Server Local DB\\Installed Versions\\";

        private const string InstanceAPIPathValueName = "InstanceAPIPath";

        private const string ProcLocalDBStartInstance = "LocalDBStartInstance";

        private const string ProcLocalDBFormatMessage = "LocalDBFormatMessage";

        private const int MAX_LOCAL_DB_CONNECTION_STRING_SIZE = 260;

        private IntPtr _startInstanceHandle = IntPtr.Zero;

        private IntPtr _formatMessageHandle = IntPtr.Zero;

        // Local Db api doc https://msdn.microsoft.com/en-us/library/hh217143.aspx
        // HRESULT LocalDBStartInstance( [Input ] PCWSTR pInstanceName, [Input ] DWORD dwFlags,[Output] LPWSTR wszSqlConnection,[Input/Output] LPDWORD lpcchSqlConnection);  
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBStartInstance(
                [MarshalAs(UnmanagedType.LPWStr)] string localDBInstanceName,
                int flags,
                [MarshalAs(UnmanagedType.LPWStr)][Out] StringBuilder sqlConnectionDataSource,
                ref int bufferLength);

        LocalDBStartInstance funcInstance = null;

        private LocalDB()
        {
            LoadUserInstanceDll();
        }

        internal string GetLocalDBConnectionString(string localDbInstance)
        {
            StringBuilder localDBConnectionString = new StringBuilder(MAX_LOCAL_DB_CONNECTION_STRING_SIZE+1);
            int sizeOfbuffer = localDBConnectionString.Capacity;
            int result = funcInstance(localDbInstance, 0, localDBConnectionString, ref sizeOfbuffer);
            Console.WriteLine
                (localDBConnectionString.ToString());
            return localDBConnectionString.ToString();//  Regex.Unescape(localDBConnectionString.ToString());
        }

        /// <summary>
        /// Loads the User Instance dll.
        /// </summary>
        private void LoadUserInstanceDll()
        {
            //Get UserInstance Dll path

            string dllPath = GetUserInstanceDllPath();

            SafeLibraryHandle libraryHandle = Interop.Kernel32.LoadLibraryExW(dllPath, IntPtr.Zero, 0);
            _startInstanceHandle = Interop.Kernel32.GetProcAddress(libraryHandle, ProcLocalDBStartInstance);
            _formatMessageHandle = Interop.Kernel32.GetProcAddress(libraryHandle, ProcLocalDBFormatMessage);
            funcInstance = (LocalDBStartInstance)Marshal.GetDelegateForFunctionPointer(_startInstanceHandle, typeof(LocalDBStartInstance));

        }

        /// <summary>
        /// Gets the sqluserinstance DLL path from the registry
        /// </summary>
        /// <returns></returns>
        private string GetUserInstanceDllPath()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(LocalDBInstalledVersionRegistryKey))
            {
                Version latestVersion = new Version("0.0");
                foreach (string subKey in key.GetSubKeyNames())
                {
                    try
                    {
                        Version currentKeyVersion = new Version(subKey);
                        latestVersion = latestVersion.CompareTo(currentKeyVersion) < 0 ? currentKeyVersion : latestVersion;
                    }
                    catch (FormatException)
                    {
                        // Handle bad registry key
                        break;
                    }
                }
                // Use the latest version to get the DLL path
                using (RegistryKey latestVersionKey = key.OpenSubKey(latestVersion.ToString()))
                {
                    RegistryValueKind valueKind = latestVersionKey.GetValueKind(InstanceAPIPathValueName);

                    string userDllPath = valueKind == RegistryValueKind.String
                        ? (string)latestVersionKey.GetValue(InstanceAPIPathValueName)
                        : null;

                    if (userDllPath == null)
                    {
                        // Error
                    }
                    return userDllPath;
                }
            }
        }
    }
}
