namespace System.Management.Instrumentation
{
    using System;
    using System.IO;
    using System.Security.Principal;
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Versioning;

    internal sealed class WMICapabilities
    {
        const string WMIKeyPath = @"Software\Microsoft\WBEM";
        const string WMINetKeyPath = @"Software\Microsoft\WBEM\.NET";
        const string WMICIMOMKeyPath = @"Software\Microsoft\WBEM\CIMOM";

        const string MultiIndicateSupportedValueNameVal = "MultiIndicateSupported";
        const string AutoRecoverMofsVal = "Autorecover MOFs";
        const string AutoRecoverMofsTimestampVal = "Autorecover MOFs timestamp";
        const string InstallationDirectoryVal = "Installation Directory";
        const string FrameworkSubDirectory = "Framework";

        /// <summary>
        /// Key to WMI.NET information
        /// </summary>
        static RegistryKey wmiNetKey;
	 static RegistryKey wmiKey;
	 
        [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
        static WMICapabilities ()
        {
		wmiNetKey = Registry.LocalMachine.OpenSubKey(WMINetKeyPath, false);
              wmiKey = Registry.LocalMachine.OpenSubKey(WMIKeyPath, false);
        }

        /// <summary>
        /// Indicates if IWbemObjectSink supports calls with multiple objects.
        /// On some versions of WMI, IWbemObjectSink will leak memory if
        /// Indicate is called with lObjectCount greater than 1.
        /// If the registry value,
        /// HKLM\Software\Microsoft\WBEM\.NET\MultiIndicateSupported
        /// exists and is non-zero, it is assumed that we can call Indicate
        /// with multiple objects.
        /// Allowed values
        /// -1 - We have not determined support for multi-indicate yet
        ///  0 - We do not support multi-indicate
        ///  1 - We support multi-indicate
        /// </summary>
        static int multiIndicateSupported = -1;
        static public bool MultiIndicateSupported
        {
            get
            {
                if(-1 == multiIndicateSupported)
                {
                    // Default multi-indicate support to what we think is
                    // possible based on the OS.
                    // This should be true for whistler, or Nova with FastProx.dll FilePrivatePart is >= 56.
                    multiIndicateSupported = MultiIndicatePossible()?1:0;

                    // See if there is a WMI.NET key
                    if(wmiNetKey != null)
                    {
                        // Try to get the 'MultiIndicateSupported' value
                        // Default to the default value in multiIndicateSupported
                        Object result = wmiNetKey.GetValue(MultiIndicateSupportedValueNameVal, multiIndicateSupported);

                        // The value should be a DWORD (returned as an 'int'), and is 1 if supported
                        if(result.GetType() == typeof(int) && (int)result==1)
                            multiIndicateSupported = 1;
                    }
                }
                return multiIndicateSupported == 1;
            }
        }

        [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
        static public void AddAutorecoverMof(string path)
        {
            RegistryKey wmiCIMOMKey = Registry.LocalMachine.OpenSubKey(WMICIMOMKeyPath, true);
            if(null != wmiCIMOMKey)
            {
                object mofsTemp = wmiCIMOMKey.GetValue(AutoRecoverMofsVal);
                string [] mofs = mofsTemp as string[];
                    if(null == mofs)
                    {
                        if(null != mofsTemp)
                        {
                            // Oh No!  We have a auto recover key, but it is not reg multistring
                            // We just give up
                            return;
                        }
                        mofs = new string[] {};
                    }

                // We ALWAYS update the autorecover timestamp
                wmiCIMOMKey.SetValue(AutoRecoverMofsTimestampVal, DateTime.Now.ToFileTime().ToString((IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(System.Int64))));

                // Look for path in existing autorecover key
                foreach(string mof in mofs)
                {
                    if(String.Compare(mof, path, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // We already have this MOF
                        return;
                    }
                }

                // We have the array of strings.  Now, add a new one
                string [] newMofs = new string[mofs.Length+1];
                mofs.CopyTo(newMofs, 0);
                newMofs[newMofs.Length-1] = path;

                wmiCIMOMKey.SetValue(AutoRecoverMofsVal, newMofs);
                wmiCIMOMKey.SetValue(AutoRecoverMofsTimestampVal, DateTime.Now.ToFileTime().ToString((IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(System.Int64))));
            }
        }

        static string installationDirectory = null;
        public static string InstallationDirectory
        {
            get
            {
                if(null == installationDirectory && null != wmiKey)
                    installationDirectory = wmiKey.GetValue(InstallationDirectoryVal).ToString();
                return installationDirectory;
            }
        }

        public static string FrameworkDirectory
        {
            get
            {
                return Path.Combine(InstallationDirectory, FrameworkSubDirectory);
            }
        }

        public static bool IsUserAdmin()
        {
            // Bug#89083 - If we are on Win9x, we are always assumed to be an admin
            if(Environment.OSVersion.Platform == PlatformID.Win32Windows)
                return true;
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return (principal.Identity.IsAuthenticated && principal.IsInRole(WindowsBuiltInRole.Administrator));
        }

        static bool IsNovaFile(FileVersionInfo info)
        {
            // A file is a Nova version if Major=1, Minor=50, and Build=1085
            return (    info.FileMajorPart==1
                &&  info.FileMinorPart==50
                &&  info.FileBuildPart==1085);
        }

       [ResourceExposure(ResourceScope.None),ResourceConsumption(ResourceScope.Machine,ResourceScope.Machine)]
       static bool MultiIndicatePossible()
        {
            // If we are on Whistler or above, we are OK
            // - Platform == Win32NT and OS version >= 5.1.0.0
            // - MajorVersion > 5 or (MajorVersion == 5 && MinorVersion > 0)
            OperatingSystem os = Environment.OSVersion;
            if(os.Platform==PlatformID.Win32NT &&  os.Version >= new Version(5, 1))
                return true;


            // We know that we can do multi-indicate if we are running Nova,
            // and the FastProx.dll FilePrivatePart is >= 56.
            string fastproxPath = Path.Combine(Environment.SystemDirectory, @"wbem\fastprox.dll");
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(fastproxPath);
            if(IsNovaFile(info) && info.FilePrivatePart>=56)
                return true;

            return false;
        }

        public static bool IsWindowsXPOrHigher()
        {
            // If we are on Whistler or above, we are OK
            // - Platform == Win32NT and OS version >= 5.1.0.0
            // - MajorVersion > 5 or (MajorVersion == 5 && MinorVersion > 0)
            OperatingSystem os = Environment.OSVersion;
            if(os.Platform==PlatformID.Win32NT && os.Version >= new Version(5, 1))
                return true;
            return false;
        }
    }
}
