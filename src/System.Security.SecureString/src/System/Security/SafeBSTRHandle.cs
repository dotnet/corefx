// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Security
{
    [System.Security.SecurityCritical]  // auto-generated
    internal sealed class SafeBSTRHandle : SafeBuffer
    {
        internal SafeBSTRHandle() : base(true) { }

        internal static SafeBSTRHandle Allocate(string src, uint lenInChars)
        {
            SafeBSTRHandle bstr = Interop.OleAut32.SysAllocStringLen(src, lenInChars);
            if (bstr.IsInvalid) // SysAllocStringLen returns a NULL ptr when there's insufficient memory
            {
                throw new OutOfMemoryException();
            }
            bstr.Initialize(lenInChars * sizeof(char));
            return bstr;
        }

        internal static SafeBSTRHandle Allocate(IntPtr src, uint lenInBytes)
        {
            Debug.Assert(lenInBytes % sizeof(char) == 0);
            SafeBSTRHandle bstr = Interop.OleAut32.SysAllocStringLen(src, lenInBytes / sizeof(char));
            if (bstr.IsInvalid) // SysAllocStringLen returns a NULL ptr when there's insufficient memory
            {
                throw new OutOfMemoryException();
            }
            bstr.Initialize(lenInBytes);
            return bstr;
        }

        [System.Security.SecurityCritical]
        override protected bool ReleaseHandle()
        {
            Interop.NtDll.ZeroMemory(handle, (UIntPtr)(Interop.OleAut32.SysStringLen(handle) * 2));
            Interop.OleAut32.SysFreeString(handle);
            return true;
        }

        internal unsafe void ClearBuffer()
        {
            byte* bufferPtr = null;
            try
            {
                AcquirePointer(ref bufferPtr);
                Interop.NtDll.ZeroMemory((IntPtr)bufferPtr, (UIntPtr)(Interop.OleAut32.SysStringLen((IntPtr)bufferPtr) * 2));
            }
            finally
            {
                if (bufferPtr != null)
                    ReleasePointer();
            }
        }

        internal unsafe uint Length
        {
            get
            {
                return Interop.OleAut32.SysStringLen(this);
            }
        }

        internal unsafe static void Copy(SafeBSTRHandle source, SafeBSTRHandle target, uint bytesToCopy)
        {
            if (bytesToCopy == 0)
                return;

            byte* sourcePtr = null, targetPtr = null;
            try
            {
                source.AcquirePointer(ref sourcePtr);
                target.AcquirePointer(ref targetPtr);

                Debug.Assert(Interop.OleAut32.SysStringLen((IntPtr)sourcePtr) * sizeof(char) >= bytesToCopy, "Source buffer is too small.");
                Buffer.MemoryCopy(sourcePtr, targetPtr, Interop.OleAut32.SysStringLen((IntPtr)targetPtr) * sizeof(char), bytesToCopy);
            }
            finally
            {
                if (sourcePtr != null)
                    source.ReleasePointer();
                if (targetPtr != null)
                    target.ReleasePointer();
            }
        }
    }
}
