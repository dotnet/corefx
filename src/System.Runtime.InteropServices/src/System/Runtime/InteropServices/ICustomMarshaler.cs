namespace System.Runtime.InteropServices
{
    public interface ICustomMarshaler
    {
        void CleanUpManagedData(object ManagedObj);
        void CleanUpNativeData(IntPtr pNativeData);
        int GetNativeDataSize();
        IntPtr MarshalManagedToNative(object ManagedObj);
        object MarshalNativeToManaged(IntPtr pNativeData);
    }
}
