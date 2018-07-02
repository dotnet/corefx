// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    ///     Copies bytes and holds them as native blob
    /// </summary>
    internal unsafe sealed class SafeKeyHandle : SafeHandle
    {
        private int _length;

        public SafeKeyHandle(ReadOnlySpan<byte> key)
            : base(IntPtr.Zero, ownsHandle: true)
        {
            _length = key.Length;
            handle = Marshal.AllocHGlobal(key.Length);

            Span<byte> copiedKey = new Span<byte>(handle.ToPointer(), key.Length);
            key.CopyTo(copiedKey);
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
            CryptographicOperations.ZeroMemory(new Span<byte>(handle.ToPointer(), _length));
            Marshal.FreeHGlobal(handle);
            return true;
        }

        public ReadOnlySpan<byte> Key => new ReadOnlySpan<byte>(handle.ToPointer(), _length);
    }
}
