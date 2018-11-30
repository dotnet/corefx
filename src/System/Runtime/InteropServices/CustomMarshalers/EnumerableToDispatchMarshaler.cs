using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Runtime.InteropServices.CustomMarshalers
{
    public class EnumerableToDispatchMarshaler : ICustomMarshaler
    {
        private static readonly EnumerableToDispatchMarshaler Instance = new EnumerableToDispatchMarshaler();

        public static ICustomMarshaler GetInstance(string cookie) => Instance;

        private EnumerableToDispatchMarshaler()
        {}

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.Release(pNativeData);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
            {
                throw new ArgumentNullException(nameof(ManagedObj));
            }

            return Marshal.GetComInterfaceForObject<object, IEnumerable>(ManagedObj);
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(pNativeData));
            }

            object comObject = Marshal.GetObjectForIUnknown(pNativeData);

            return ComDataHelpers.GetOrCreateManagedViewFromComData<object, EnumerableViewOfDispatch>(comObject, obj => new EnumerableViewOfDispatch(obj));
        }
    }
}
