// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.Diagnostics
{
    internal static partial class ProcessManager
    {
        public static IntPtr GetMainWindowHandle(int processId) => IntPtr.Zero;
    }

    internal static partial class NtProcessManager
    {
        private static ProcessModuleCollection GetModules(int processId, bool firstModuleOnly)
        {
            // We don't have a good way of getting all of the modules of the particular process,
            // but we can at least get the path to the executable file for the process, and
            // other than for debugging tools, that's the main reason consumers of Modules care about it,
            // and why MainModule exists.
            try
            {
                // Get the path to the executable
                Process process = Process.GetProcessById(processId);
                StringBuilder sb = new StringBuilder(1024);
                int capacity = 1024;
                Interop.Kernel32.QueryFullProcessImageName(process.Handle, 0, sb, ref capacity);
                string exePath = sb.ToString();

                if (!string.IsNullOrEmpty(exePath))
                {
                    return new ProcessModuleCollection(1)
                    {
                        new ProcessModule()
                        {
                            FileName = exePath,
                            ModuleName = Path.GetFileName(exePath)
                        }
                    };
                }
            }
            catch { } // eat all errors

            return new ProcessModuleCollection(0);
        }
    }
}
