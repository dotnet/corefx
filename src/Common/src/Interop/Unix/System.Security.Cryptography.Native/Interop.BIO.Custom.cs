using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

internal static partial class Interop
{
    internal static class CustomBio
    {
        private static IntPtr _customBioMethodStructure;
        private static CreateDelegate _createDelegate;
        private static DestroyDelegate _destroyDelegate;
        private unsafe static ControlDelegate _controlDelegate;
        private unsafe static ReadDelegate _readDelegate;
        private unsafe static WriteDelegate _writeDelegate;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int CreateDelegate(bio_st* bio);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int ReadDelegate(IntPtr bio, void* buf, int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate int WriteDelegate(IntPtr bio, void* buf, int num);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate long ControlDelegate(IntPtr bio, BIO_CTRL cmd, long num, void* ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DestroyDelegate(IntPtr bio);

        static CustomBio()
        {
            CustomBio.Initialize();
        }

        internal unsafe static void Initialize()
        {
            _createDelegate = Create;
            _controlDelegate = Control;
            _destroyDelegate = Destroy;
            _writeDelegate = Write;
            _readDelegate = Read;

            var name = Marshal.StringToHGlobalAnsi("Managed Bio");
            var bioStruct = new bio_method_st()
            {
                create = _createDelegate,
                name = name,
                type = BIO_TYPE.BIO_TYPE_SOURCE_SINK,
                destroy = _destroyDelegate,
                ctrl = _controlDelegate,
                bread = _readDelegate,
                bwrite = _writeDelegate,
            };

            var memory = Marshal.AllocHGlobal(Marshal.SizeOf<bio_method_st>());
            Marshal.StructureToPtr(bioStruct, memory, true);
            _customBioMethodStructure = memory;
        }

        internal static GCHandle BioGetGCHandle(IntPtr bio)
        {
            var ptr = Crypto.BioGetAppData(bio);
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
            var bio = Crypto.CreateCustomBio(_customBioMethodStructure);
            return bio;
        }

        private unsafe static int Create(bio_st* bio)
        {
            bio[0].init = 1;
            return 1;
        }

        private static int Destroy(IntPtr bio) => 1;

        private static unsafe int Write(IntPtr bio, void* input, int size)
        {
            var handle = BioGetGCHandle(bio);
            Debug.Assert(handle.IsAllocated);
            if (!handle.IsAllocated)
            {
                return -1;
            }
            throw new NotImplementedException();
        }

        private static unsafe int Read(IntPtr bio, void* output, int size)
        {
            var handle = BioGetGCHandle(bio);
            Debug.Assert(handle.IsAllocated);
            if (!handle.IsAllocated)
            {
                return -1;
            }
            throw new NotImplementedException();
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
