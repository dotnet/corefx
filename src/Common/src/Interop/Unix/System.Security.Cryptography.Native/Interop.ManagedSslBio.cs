using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        internal static class ManagedSslBio
        {
            private unsafe readonly static ReadDelegate s_readDelegate;
            private unsafe readonly static WriteDelegate s_writeDelegate;

            internal static SafeBioHandle CreateManagedSslBio() => Crypto.CreateManagedSslBio();

            unsafe static ManagedSslBio()
            {
                s_writeDelegate = Write;
                s_readDelegate = Read;
                Crypto.InitManagedSslBioMethod(s_writeDelegate, s_readDelegate);
            }

            internal static void BioSetGCHandle(SafeBioHandle bio, GCHandle handle)
            {
                IntPtr pointer = handle.IsAllocated ? GCHandle.ToIntPtr(handle) : IntPtr.Zero;
                Crypto.BioSetAppData(bio, pointer);
            }

            private static unsafe int Write(IntPtr bio, void* input, int size, IntPtr data)
            {
                GCHandle handle = GCHandle.FromIntPtr(data);
                Debug.Assert(handle.IsAllocated);

                if (handle.Target is SafeSslHandle.WriteBioBuffer buffer)
                {
                    return buffer.Write(new Span<byte>(input, size));
                }

                return -1;
            }

            private static unsafe int Read(IntPtr bio, void* output, int size, IntPtr data)
            {
                GCHandle handle = GCHandle.FromIntPtr(data);
                Debug.Assert(handle.IsAllocated);

                if (handle.Target is SafeSslHandle.ReadBioBuffer buffer)
                {
                    return buffer.Read(new Span<byte>(output, size));
                }

                return -1;
            }
        }
    }
}
