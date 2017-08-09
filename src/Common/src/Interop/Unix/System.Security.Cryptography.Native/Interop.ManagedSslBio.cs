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
            private unsafe static ReadDelegate s_readDelegate;
            private unsafe static WriteDelegate s_writeDelegate;

            static ManagedSslBio() => Initialize();

            internal static SafeBioHandle CreateManagedSslBio() => Crypto.CreateManagedSslBio();
            
            private unsafe static void Initialize()
            {
                s_writeDelegate = Write;
                s_readDelegate = Read;
                Crypto.InitManagedSslBioMethod(s_writeDelegate, s_readDelegate);
            }

            internal static void BioSetGCHandle(SafeBioHandle bio, GCHandle handle)
            {
                IntPtr pointer;
                if (handle.IsAllocated)
                {
                    pointer = GCHandle.ToIntPtr(handle);
                }
                else
                {
                    pointer = IntPtr.Zero;
                }

                Crypto.BioSetAppData(bio, pointer);
            }

            private static unsafe int Write(IntPtr bio, void* input, int size, IntPtr data)
            {
                GCHandle handle = GCHandle.FromIntPtr(data);

                if (handle.IsAllocated && handle.Target is SafeSslHandle.WriteBioBuffer buffer)
                {
                    return buffer.Write(new Span<byte>(input, size));
                }

                return -1;
            }

            private static unsafe int Read(IntPtr bio, void* output, int size, IntPtr data)
            {
                GCHandle handle = GCHandle.FromIntPtr(data);

                if (handle.IsAllocated && handle.Target is SafeSslHandle.ReadBioBuffer buffer)
                {
                    return buffer.Read(new Span<byte>(output, size));
                }

                return -1;
            }
        }
    }
}
