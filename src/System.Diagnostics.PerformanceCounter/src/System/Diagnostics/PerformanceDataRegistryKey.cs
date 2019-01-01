using System.Buffers;
using System.IO;
using Microsoft.Win32;
using Internal.Win32.SafeHandles;

namespace System.Diagnostics
{
    internal class PerformanceDataRegistryKey
    {
        private readonly SafeRegistryHandle _hkey;

        private const int PerformanceData = (int)RegistryHive.PerformanceData;

        private PerformanceDataRegistryKey(SafeRegistryHandle hkey)
        {
            _hkey = hkey;
        }

        public static PerformanceDataRegistryKey OpenRemoteBaseKey(string machineName)
        {
            // connect to the specified remote registry
            SafeRegistryHandle foreignHKey = null;
            int ret = Interop.Advapi32.RegConnectRegistry(machineName, new SafeRegistryHandle(new IntPtr(PerformanceData), false), out foreignHKey);

            if (ret == Interop.Errors.ERROR_DLL_INIT_FAILED)
            {
                // return value indicates an error occurred
                throw new ArgumentException(SR.Format(SR.Arg_DllInitFailure, machineName));
            }

            if (ret != 0)
            {
                Win32Error(ret, null);
            }

            if (foreignHKey.IsInvalid)
            {
                // return value indicates an error occurred
                throw new ArgumentException(SR.Format(SR.Arg_RegKeyNoRemoteConnect, machineName));
            }

            return new PerformanceDataRegistryKey(foreignHKey);
        }

        public static PerformanceDataRegistryKey OpenLocal()
        {
            var key = new SafeRegistryHandle(new IntPtr(PerformanceData), true);
            return new PerformanceDataRegistryKey(key);
        }

        public byte[] GetValue(string name, bool usePool)
        {
            int size = 65000;
            int sizeInput = size;

            int ret;
            int type = 0;
            byte[] blob = CreateBlob(size, usePool);
            while (Interop.Errors.ERROR_MORE_DATA == (ret = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref type, blob, ref sizeInput)))
            {
                if (size == int.MaxValue)
                {
                    // ERROR_MORE_DATA was returned however we cannot increase the buffer size beyond Int32.MaxValue
                    Win32Error(ret, name);
                }
                else if (size > (int.MaxValue / 2))
                {
                    // at this point in the loop "size * 2" would cause an overflow
                    size = int.MaxValue;
                }
                else
                {
                    size *= 2;
                }
                sizeInput = size;

                ReleaseBlob(blob, usePool);
                blob = CreateBlob(size, usePool);
            }

            if (ret != 0)
            {
                Win32Error(ret, name);
            }

            return blob;
        }

        public void ReleaseBlob(byte[] blob, bool usePool = true)
        {
            if (usePool)
            {
                ArrayPool<byte>.Shared.Return(blob);
            }
        }

        private byte[] CreateBlob(int size, in bool usePool)
        {
            return usePool
                ? ArrayPool<byte>.Shared.Rent(size)
                : new byte[size];
        }

        private static void Win32Error(in int errorCode, string name)
        {
            if (errorCode == Interop.Errors.ERROR_ACCESS_DENIED)
            {
                throw new UnauthorizedAccessException(SR.Format(SR.UnauthorizedAccess_RegistryKeyGeneric_Key, name));
            }

            throw new IOException(Interop.Kernel32.GetMessage(errorCode), errorCode);
        }

        public void Close()
        {
            Interop.Advapi32.RegCloseKey(new IntPtr(PerformanceData));
        }
    }
}
