// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

namespace System.IO.Ports
{
    public partial class SerialPort : Component
    {
        public static string[] GetPortNames()
        {
            const string sysDir = "/sys/class/tty";

            if (Directory.Exists(sysDir))
            {
                // /sys is mounted. Let's explore tty class and pick active nodes.
                List<string> ports = new List<string>();
                DirectoryInfo di = new DirectoryInfo(@"/sys/class/tty");
                var entries = di.EnumerateFileSystemInfos(@"*", SearchOption.TopDirectoryOnly);
                foreach (var entry in entries)
                {
                    if (Directory.Exists("/sys/bus/usb-serial/devices/" + entry.Name) || File.Exists(entry.FullName + "/device/id"))
                    {
                        ports.Add("/dev/" + entry.Name);
                    }
                }
                return ports.ToArray();
            }
            else
            {
                // Fallback to scanning /dev. That may have more devices then needed as well as
                // This can miss usb or devices with non-standard name.
                return Directory.GetFiles("/dev", "ttyS*");
            }
        }
    }
}
