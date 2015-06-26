// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//------------------------------------------------------------------------------
//

//
//------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Authentication.ExtendedProtection
{
    public abstract class ChannelBinding : SafeHandle
    {
        protected ChannelBinding()
            : base(IntPtr.Zero, true)
        {
        }

        protected ChannelBinding(bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        {
        }

        public abstract int Size
        {
            get;
        }

        // Copied from SafeHandleZeroOrMinusOneIsInvalid
        public override bool IsInvalid
        {
            get { return handle == new IntPtr(0) || handle == new IntPtr(-1); }
        }
    }
}