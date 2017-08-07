using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

internal static partial class Interop
{
    internal static class CustomBio
    {
        private static IntPtr s_customBioMethodStructure;
        private static CreateDelegate s_createDelegate;
        private static DestroyDelegate s_destroyDelegate;
        private unsafe static ControlDelegate s_controlDelegate;
        private unsafe static ReadDelegate s_readDelegate;
        private unsafe static WriteDelegate s_writeDelegate;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int CreateDelegate(bio_st* bio);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int ReadDelegate(IntPtr bio, void* buf, int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int WriteDelegate(IntPtr bio, void* buf, int num);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate long ControlDelegate(IntPtr bio, Crypto.BIO_CTRL cmd, long num, void* ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DestroyDelegate(IntPtr bio);

        static CustomBio()
        {
            CustomBio.Initialize();
        }

        internal unsafe static void Initialize()
        {
            s_createDelegate = Create;
            s_controlDelegate = Control;
            s_destroyDelegate = Destroy;
            s_writeDelegate = Write;
            s_readDelegate = Read;

            IntPtr name = Marshal.StringToHGlobalAnsi("Managed Bio");
            var bioStruct = new bio_method_st()
            {
                create = s_createDelegate,
                name = name,
                type = BIO_TYPE.BIO_TYPE_SOURCE_SINK,
                destroy = s_destroyDelegate,
                ctrl = s_controlDelegate,
                bread = s_readDelegate,
                bwrite = s_writeDelegate,
            };

            IntPtr memory = Marshal.AllocHGlobal(Marshal.SizeOf<bio_method_st>());
            Marshal.StructureToPtr(bioStruct, memory, true);
            s_customBioMethodStructure = memory;
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
            var ptr = GCHandle.ToIntPtr(handle);
            Crypto.BioSetAppData(bio, ptr);
        }

        internal static SafeBioHandle CreateCustomBio()
        {
            SafeBioHandle bio = Crypto.CreateCustomBio(s_customBioMethodStructure);
            return bio;
        }

        private unsafe static int Create(bio_st* bio)
        {
            bio[0].init = 1;
            return 1;
        }

        private static int Destroy(IntPtr bio) => 1;

        private static unsafe long Control(IntPtr bio, Crypto.BIO_CTRL cmd, long param, void* ptr)
        {
            switch (cmd)
            {
                case Crypto.BIO_CTRL.BIO_CTRL_FLUSH:
                case Crypto.BIO_CTRL.BIO_CTRL_POP:
                case Crypto.BIO_CTRL.BIO_CTRL_PUSH:
                    return 1;
            }
            return 0;
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

        [Flags]
        private enum BIO_TYPE
        {
            BIO_TYPE_SOURCE_SINK = 0x0400,
        }
                
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct bio_st
        {
            public void* method;
            public void* callback;
            public void* cb_arg;
            public int init;
            public int shutdown;
            public int flags;
            public int retry_reason;
            public int num;
            public void* ptr;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct bio_method_st
        {
            public BIO_TYPE type;
            public IntPtr name;
            public WriteDelegate bwrite; // int (* bwrite) (BIO*, const char*, int);
            public ReadDelegate bread; // int (* bread) (BIO*, char*, int);
            public IntPtr bputs; // int (* bputs) (BIO*, const char*);
            public IntPtr bgets; // int (* bgets) (BIO*, char*, int);
            public ControlDelegate ctrl; // long (* ctrl) (BIO*, int, long, void*);
            public CreateDelegate create; // int (* create) (BIO*);
            public DestroyDelegate destroy; // int (* destroy) (BIO*);
            public IntPtr callback_ctrl; // long (* callback_ctrl) (BIO*, int, bio_info_cb*);
        }

    }
}
