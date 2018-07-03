// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    ///     Holds a managed string marshaled as a LPCWSTR.
    /// </summary>
    internal sealed class SafeUnicodeStringHandle : SafeHandle
    {
        /// <summary>
        ///     Marshal a String to a native LPCWSTR. It is permitted to pass "null" for the string.
        /// </summary>
        public SafeUnicodeStringHandle(string s)
            : base(IntPtr.Zero, ownsHandle: true)
        {
            handle = Marshal.StringToHGlobalUni(s);
        }

        public unsafe SafeUnicodeStringHandle(ReadOnlySpan<char> s)
            : base(IntPtr.Zero, ownsHandle: true)
        {
            int cch = checked(s.Length + 1);
            int cb = checked(cch * sizeof(char));
            handle = Marshal.AllocHGlobal(cb);

            Span<char> dest = new Span<char>(handle.ToPointer(), cch);
            s.CopyTo(dest);
            dest[s.Length] = (char)0;
        }

        public sealed override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        protected sealed override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }
}
