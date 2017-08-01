// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

namespace System.IO.Ports
{
    internal abstract partial class SerialCommunication
    {
        public static SerialCommunication Current { get { return s_current; } }

        public abstract SafeFileHandle OpenPort(ulong portName);
    }
}
