﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.IO.Ports
{
    public partial class SerialPort : Component
    {
        public static string[] GetPortNames()
        {
            // Hitting the registry for this isn't the only way to get the ports.
            //
            // WMI: https://msdn.microsoft.com/en-us/library/aa394413.aspx [Requires .NetFramework !]
            // QueryDosDevice: https://msdn.microsoft.com/en-us/library/windows/desktop/aa365461.aspx [Also produces duplicates, eg COM4 also listed at DosDeviceName \\USB#VID_1B4F&PID_214F.....
            //
            // QueryDosDevice involves finding any ports that map to \Device\Serialx (call with null to get all, then iterate to get the actual device name) [Typically maps to COMx value]
            // https://docs.microsoft.com/en-gb/previous-versions/ff546502%28v%3dvs.85%29


            // Use HashSet HashSet<string> strings = new HashSet<string>();
            HashSet<string> uniquPortNames = new HashSet<string>();

            using (RegistryKey serialKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
            {
                if (serialKey != null)
                {
                    foreach (string valueName in serialKey.GetValueNames())
                    {
                        // Add the key's value (portname)
                        uniquPortNames.Add((string)serialKey.GetValue(valueName));
                    }
                }
            }

            // USB Serial Ports on Win10IoT are not initalised in registry key HKLM\HARDWARE\DEVICEMAP\SERIALCOMM correctly, placing garbage chars in this keys value
            // So to handle this, we are building a list of both, and assuming consumer will handle dual mapping between 

            // Is GUID class ID for COM ports https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-comport

            foreach (string dosName in QueryDosDeviceComPorts("86e0d1e0-8089-11d0-9ce4-08003e301f73"))
            {
                uniquPortNames.Add(dosName);
            }

            string[] result = new string[uniquPortNames.Count];
            uniquPortNames.CopyTo(result);

            return result;
        }

        private static string[] QueryDosDeviceComPorts(string guid)
        {
            // Allocate some memory to get a list of all system devices.
            // Start with a small size and dynamically give more space until we have enough room.
            uint returnSize = 0;
            int maxSize = 1024;
            string allDevices = null;
            const uint ERROR_INSUFFICIENT_BUFFER = 122;
           // IntPtr mem;

            string[] allDevicesArray = null;
            List<string> retList = new List<string>();

            while (returnSize == 0)
            {
                unsafe
                {
                    //fixed (mem = new byte[3]); // Marshal.AllocHGlobal(maxSize);
                    fixed (void* m = new byte[maxSize])
                        try
                        {
                            returnSize = Interop.Kernel32.QueryDosDevice(null, (System.IntPtr)m, maxSize);
                            if (returnSize != 0)
                            {
                                allDevices = Marshal.PtrToStringAnsi((System.IntPtr)m, (int)returnSize);
                                allDevicesArray = allDevices.Split('\0');
                                break;
                            }
                            else if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                            {
                                maxSize += 1024;
                            }
                            else
                            {
                                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                            }
                        }
                        finally
                        {
                            //Marshal.FreeHGlobal(mem);
                        }
                }
            }

            // Build devices matching guid for serial ports
            foreach (string name in allDevicesArray)
            {
                if (name.ToLower().Contains(guid))
                {
                    retList.Add(name);
                }
            }

            return retList.ToArray();
        }
    }
}
