// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    // Contains information on a socket's keep-alive settings (RFC 1122 section 4.2.3.6)
    public class KeepAliveOption
    {
        // Enables or disables keep-alives
        public bool Enabled { get; set; }

        // The number of unacknowledged probes to send before considering the connection dead
        public int RetryCount { get; set; }
 
        // The interval in seconds between the last data packet sent and the first keepalive probe
        // No more used after the connection has been marked to need keep-alive
        public int Time { get; set; }
 
        // The interval in seconds between subsequential keepalive probes
        public int Interval { get; set; }

        public KeepAliveOption(bool enable, int retryCount, int time, int interval)
        {
            Enabled = enable;
            RetryCount = retryCount;
            Time = time;
            Interval = interval;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct IOControlKeepAliveValues
    {
        [FieldOffset(0)]
        private unsafe fixed byte Bytes[12];
    
        [FieldOffset(0)]
        public uint OnOff;
            
        // Milliseconds (default 2 hours)
        [FieldOffset(4)]
        public uint Time;
    
        // Milliseconds (default 1 second)
        [FieldOffset(8)]
        public uint Interval;
        
        public IOControlKeepAliveValues(KeepAliveOption option)
        {
            OnOff = option.Enabled ? 1U : 0U;
            Time = (uint)(option.Time * 1000);
            Interval = (uint)(option.Interval * 1000);
        }

        public byte[] ToArray()
        {
            unsafe
            {
                fixed (byte* ptr = Bytes)
                {
                    IntPtr p = new IntPtr(ptr);
                    byte[] bytesArray = new byte[12];
                    Marshal.Copy(p, bytesArray, 0, bytesArray.Length);
                    return bytesArray;
                }
            }
        }
    }
}