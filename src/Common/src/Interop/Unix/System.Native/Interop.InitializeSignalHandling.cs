// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        private static bool s_signalHandlingInitialized = false;
        private static readonly object s_signalHandlingInitializeGate = new object();

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_InitializeSignalHandling", SetLastError = true)]
        private static extern bool InitializeSignalHandlingCore();

        public static bool InitializeSignalHandling()
        {
            lock (s_signalHandlingInitializeGate)
            {
                if (!s_signalHandlingInitialized)
                {
                    s_signalHandlingInitialized = InitializeSignalHandlingCore();
                }

                return s_signalHandlingInitialized;
            }
        }
    }
}
