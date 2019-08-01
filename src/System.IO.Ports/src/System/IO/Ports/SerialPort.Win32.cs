// Licensed to the .NET Foundation under one or more agreements.
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
            // WMI: https://msdn.microsoft.com/en-us/library/aa394413.aspx [But Requires .NetFramework !]
            // QueryDosDevice: https://msdn.microsoft.com/en-us/library/windows/desktop/aa365461.aspx [Can produces duplicates eg COM4 also listed as DosDeviceName \\USB#VID_1B4F&PID_214F..]
            //
            // QueryDosDevice involves finding any ports that map to \Device\Serialx (call with null to get all, then iterate to get the actual device name) [Typically maps to COMx value]
            // https://docs.microsoft.com/en-gb/previous-versions/ff546502%28v%3dvs.85%29
            //
            // GetPortNames will return serial port device names for both registry registered SerialComm devices as well as DosDevice names from Interop QueryDosDevice()
            // USB Serial Ports on Win10IoT are not initalised in registry key HKLM\HARDWARE\DEVICEMAP\SERIALCOMM correctly, placing garbage chars in this keys value
            // So to handle this, we are returning a list of port names from both locations.
            // ToDo: If duplicates wanted to be removed, enumerate the rgistery and try and match & remove duplicate 
            //   HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USB is one location, but matching criteria not clear!
            //   HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\COM Name Arbiter\Devices (empty on Win10IoT)

            HashSet<string> resultPortNames = new HashSet<string>();

            // Query registry register COM[x] ports
            //
            using (RegistryKey serialKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
            {
                if (serialKey != null)
                    foreach (string valueName in serialKey.GetValueNames())
                    {
                        string portName = ((string)serialKey.GetValue(valueName)).ToUpper();

                        // Add the key's value (portname) if it is valid (Win10IoT places garbage in this Reg Key)
                        if (portName.StartsWith("COM")) // Filters corrupt chars for COM[x] devices in Registry on Win10IoT 
                            resultPortNames.Add(portName);
                    }
            }

            // Query Interop QueryDosDevice()
            //            
            // Is GUID class ID for COM ports https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-comport
            // Typical DosDevice serial port name 'USB#VID_1B4F&PID_214F&MI_00#6&381731fd&0&0000#{86e0d1e0-8089-11d0-9ce4-08003e301f73}'
            //
            foreach (string dosName in QueryDosDeviceComPorts("86e0d1e0-8089-11d0-9ce4-08003e301f73"))
            {
                resultPortNames.Add(dosName.ToLower());
            }

            string[] result = new string[resultPortNames.Count];
            resultPortNames.CopyTo(result);

            return result;
        }

        private static string[] QueryDosDeviceComPorts(string guid)
        {
            // Build a list of all system Com Port device names.
            // memBuff starts with a small arbitary size and dynamically expands until there is enough room for data returned from Kernal32.QueryDosDevice().
            uint returnSize = 0;
            int maxSize = 1024;
            string allDevices = null;
            const uint ERROR_INSUFFICIENT_BUFFER = 122;

            string[] allDevicesArray = null;
            List<string> returnList = new List<string>();

            while (returnSize == 0)
            {
                unsafe
                {
                    byte[] memBuff = new byte[maxSize];
                    fixed (byte* memPtr = memBuff)
                        try
                        {
                            returnSize = Interop.Kernel32.QueryDosDevice(null, (System.IntPtr)memPtr, maxSize);
                            if (returnSize != 0)
                            {
                                allDevices = Marshal.PtrToStringAnsi((System.IntPtr)memPtr, (int)returnSize);
                                allDevicesArray = allDevices.Split('\0');
                                break;
                            }
                            else if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                            {
                                maxSize += 1024; // Increase mem buffer
                            }
                            else
                            {
                                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                            }
                        }
                        finally
                        {
                        }
                }
            }

            // Build devices matching guid for serial ports
            foreach (string name in allDevicesArray)
            {
                if (name.ToLower().Contains(guid))
                    returnList.Add(name);
            }

            return returnList.ToArray();
        }
    }
}
