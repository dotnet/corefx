using System;
using System.Collections.Generic;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    public class TypeToTypeInfoMarshaler : ICustomMarshaler
    {
        private static readonly TypeToTypeInfoMarshaler Instance = new TypeToTypeInfoMarshaler();

        public static ICustomMarshaler GetInstance(string cookie) => Instance;

        private TypeToTypeInfoMarshaler()
        {
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.Release(pNativeData);
        }

        public int GetNativeDataSize()
        {
            // Return -1 to indicate the managed type this marshaler handles is not a value type.
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
            {
                throw new ArgumentNullException(nameof(ManagedObj));
            }

            throw new PlatformNotSupportedException();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(pNativeData));
            }

            return typeof(object);
        }
    }
}
