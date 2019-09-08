// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Linq;
using System.Buffers;

namespace System.IO.Ports
{
    public partial class SerialPort : Component
    {
        public static string[] GetPortNames()
        {
            // Hitting the registry for this isn't the only way to get the ports.
            //
            // WMI: https://msdn.microsoft.com/en-us/library/aa394413.aspx [requires .NetFramework]
            // QueryDosDevice: https://msdn.microsoft.com/en-us/library/windows/desktop/aa365461.aspx [can produce duplicates eg COM4 also listed as DosDeviceName \\USB#VID_1B4F&PID_214F..]
            //
            // QueryDosDevice involves finding any ports that map to \Device\Serialx (call with null to get all, then iterate to get the actual device name) [Typically maps to COMx value]
            // https://docs.microsoft.com/en-gb/previous-versions/ff546502%28v%3dvs.85%29
            //
            // GetPortNames will return serial port device names for both registry registered SerialComm devices as well as DosDevice names from Interop QueryDosDevice()
            // USB Serial Ports on Win10IoT are not initalised in registry key HKLM\HARDWARE\DEVICEMAP\SERIALCOMM correctly, placing garbage chars in this keys value
            // So to handle this, we are returning a list of port names from both locations.
            //   HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USB is one location, but matching criteria not clear!
            //   HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\COM Name Arbiter\Devices (empty on Win10IoT)

            HashSet<string> resultPortNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            // If running on Windows IoT, search and add the Serial Devices from QueryDosDevice
            // If this is allowed to happen on Windows devices where port detection happens normally, duplicate Serial Port names are added.
            // This causes issues with System.IO.Ports.Tests
            // Port detection broken on Windows IoT (Does not initialise registry with COM port names), so use QueryDosDevice
            if (RuntimeInformation.OSArchitecture == Architecture.Arm || RuntimeInformation.OSArchitecture == Architecture.Arm64)
            {
                // Query Interop QueryDosDevice()
                //
                // Is GUID class ID for COM ports https://docs.microsoft.com/en-us/windows-hardware/drivers/install/guid-devinterface-comport
                // Typical DosDevice serial port name 'USB#VID_1B4F&PID_214F&MI_00#6&381731fd&0&0000#{86e0d1e0-8089-11d0-9ce4-08003e301f73}'
                //
                foreach (string dosName in QueryDosDeviceComPorts(GuidDevInterfaceComPort))
                {
                    resultPortNames.Add(dosName);
                }
            }
            else
            {
                // Query registry register COM[x] ports
                //
                using (RegistryKey serialKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
                {
                    if (serialKey != null)
                    {
                        foreach (string valueName in serialKey.GetValueNames())
                        {
                            string portName = ((string)serialKey.GetValue(valueName));

                            // Add the key's value (portname) if it is valid (Win10IoT places garbage in this Reg Key)
                            // Filters corrupt chars for COM[x] devices in Registry on Win10IoT
                            if (portName.StartsWith("COM", StringComparison.InvariantCultureIgnoreCase))
                            {
                                resultPortNames.Add(portName);
                            }
                        }
                    }
                }
            }

            return resultPortNames.ToArray<string>();
        }

        private static IEnumerable<string> QueryDosDeviceComPorts(string filterGuid)
        {
            // Build a list of all system Com Port device names.
            // memBuff starts with a small arbitary size and dynamically expands until there is enough room for data returned from Kernal32.QueryDosDeviceW().

            List<string> returnList = new List<string>();

            var buffPool = System.Buffers.ArrayPool<char>.Shared;
            int maxBuffSize = 1024;
            uint returnSize = 0;
            char[] memBuff = buffPool.Rent(maxBuffSize);

            while ((returnSize = Interop.Kernel32.QueryDosDeviceW(null, memBuff, memBuff.Length)) == 0)
            {
                int error = Marshal.GetLastWin32Error();
                switch (error)
                {
                    case Interop.Errors.ERROR_INSUFFICIENT_BUFFER:
                    case Interop.Errors.ERROR_MORE_DATA:
                        // Return and rent a larger char buffer
                        maxBuffSize += 1024;
                        buffPool.Return(memBuff);
                        memBuff = buffPool.Rent(maxBuffSize);
                        break;
                    default:
                        throw new Win32Exception(error);
                }
            }

            ReadOnlySpan<char> allPortNames = new Span<char>(memBuff);
            int head = 0;
            int skip = allPortNames.IndexOf('\0');

            // Build list of device names filtered by SerialPort Guid
            while (skip > 0)
            {
                var singlePortName = allPortNames.Slice(head, skip);

                // If device name contains the SerialPort GUID, add device name to returnList
                if (singlePortName.Contains(filterGuid.ToCharArray(), StringComparison.OrdinalIgnoreCase))
                {
                    returnList.Add(@"\\?\" + singlePortName.ToString());
                }

                head = head + skip + 1;
                skip = allPortNames.Slice(head, allPortNames.Length - head).IndexOf('\0');
            }

            return returnList;
        }
    }
}
