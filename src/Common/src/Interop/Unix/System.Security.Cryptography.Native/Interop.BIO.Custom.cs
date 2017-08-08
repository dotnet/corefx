using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

internal static partial class Interop
{
    internal static class CustomBio
    {
        private unsafe static ReadDelegate s_readDelegate;
        private unsafe static WriteDelegate s_writeDelegate;

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_InitCustomBioMethod")]
        private static extern void InitCustomBioMethod(WriteDelegate bwrite, ReadDelegate bread);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int ReadDelegate(IntPtr bio, void* buf, int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int WriteDelegate(IntPtr bio, void* buf, int num);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CreateCustomBio")]
        internal static extern SafeBioHandle CreateCustomBio();

        static CustomBio() => Initialize();

        internal unsafe static void Initialize()
        {
            s_writeDelegate = Write;
            s_readDelegate = Read;

            InitCustomBioMethod(s_writeDelegate, s_readDelegate);
        }

        internal static GCHandle BioGetGCHandle(IntPtr bio)
        {
            IntPtr ptr = Crypto.BioGetAppData(bio);
            if (ptr == IntPtr.Zero)
            {
                return default(GCHandle);
            }
            var handle = GCHandle.FromIntPtr(ptr);
            return handle;
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

        private static unsafe int Write(IntPtr bio, void* input, int size)
        {
            GCHandle handle = BioGetGCHandle(bio);

            if (handle.IsAllocated && handle.Target is SafeSslHandle.WriteBioBuffer buffer)
            {
                return buffer.Write(new Span<byte>(input, size));
            }
            return -1;
        }

        private static unsafe int Read(IntPtr bio, void* output, int size)
        {
            GCHandle handle = BioGetGCHandle(bio);

            if (handle.IsAllocated && handle.Target is SafeSslHandle.ReadBioBuffer buffer)
            {
                return buffer.Read(new Span<byte>(output, size));
            }
            return -1;
        }
    }
}
