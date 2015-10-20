// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    [SecurityCritical]
    internal sealed class SafeX509Handle : SafeHandle
    {
        internal static readonly SafeX509Handle InvalidHandle = new SafeX509Handle();

        private SafeX509Handle() : 
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            Interop.Crypto.X509Destroy(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return handle == IntPtr.Zero; }
        }
    }

    internal sealed class SafeX509CrlHandle : SafeHandle
    {
        private SafeX509CrlHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            Interop.Crypto.X509CrlDestroy(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }

    [SecurityCritical]
    internal sealed class SafeX509StoreHandle : SafeHandle
    {
        private SafeX509StoreHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            Interop.Crypto.X509StoreDestory(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }

    [SecurityCritical]
    internal sealed class SafeX509StoreCtxHandle : SafeHandle
    {
        private SafeX509StoreCtxHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        internal SafeX509StoreCtxHandle(IntPtr handle, bool ownsHandle) :
            base(handle, ownsHandle)
        {
        }

        protected override bool ReleaseHandle()
        {
            Interop.Crypto.X509StoreCtxDestroy(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }

    [SecurityCritical]
    internal sealed class SafeX509StackHandle : SafeHandle
    {
        private SafeX509StackHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle()
        {
            Interop.Crypto.RecursiveFreeX509Stack(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }

    /// <summary>
    /// Represents access to a STACK_OF(X509)* which is a member of a structure tracked
    /// by another SafeHandle.
    /// </summary>
    [SecurityCritical]
    internal sealed class SafeSharedX509StackHandle : SafeInteriorHandle
    {
        internal static readonly SafeSharedX509StackHandle InvalidHandle = new SafeSharedX509StackHandle();

        private SafeSharedX509StackHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {
        }
    }
}
