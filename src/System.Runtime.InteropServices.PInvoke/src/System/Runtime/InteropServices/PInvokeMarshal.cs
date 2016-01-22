// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime.InteropServices
{
    public static partial class PInvokeMarshal
    {
        public static int SystemDefaultCharSize
        {
            get
            {
                return Marshal.SystemDefaultCharSize;
            }
        }
        public static int SystemMaxDBCSCharSize
        {
            get
            {
                return Marshal.SystemMaxDBCSCharSize;
            }
        }
        public static System.IntPtr AllocateMemory(int sizeInBytes)
        {
            return Marshal.AllocCoTaskMem(sizeInBytes);
        }
        public static void DestroyStructure(System.IntPtr ptr, System.Type structureType)
        {
            Marshal.DestroyStructure(ptr, structureType);
        }
        public static void DestroyStructure<T>(System.IntPtr ptr)
        {
            Marshal.DestroyStructure<T>(ptr);
        }
        public static void FreeMemory(System.IntPtr ptr)
        {
            Marshal.FreeCoTaskMem(ptr);
        }
        public static System.Delegate GetDelegateForFunctionPointer(System.IntPtr ptr, System.Type delegateType)
        {
            return Marshal.GetDelegateForFunctionPointer(ptr, delegateType);
        }
        public static TDelegate GetDelegateForFunctionPointer<TDelegate>(System.IntPtr ptr)
        {
            return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);
        }
        public static System.IntPtr GetFunctionPointerForDelegate(System.Delegate d)
        {
            return Marshal.GetFunctionPointerForDelegate(d);
        }
        public static System.IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate d)
        {
            return Marshal.GetFunctionPointerForDelegate<TDelegate>(d);
        }
        public static int GetLastWin32Error()
        {
            return Marshal.GetLastWin32Error();
        }
        public static System.IntPtr OffsetOf(System.Type type, string fieldName)
        {
            return Marshal.OffsetOf(type, fieldName);
        }
        public static System.IntPtr OffsetOf<T>(string fieldName)
        {
            return Marshal.OffsetOf<T>(fieldName);
        }
        public static string PtrToStringAnsi(System.IntPtr ptr)
        {
            return Marshal.PtrToStringAnsi(ptr);
        }
        public static string PtrToStringAnsi(System.IntPtr ptr, int len)
        {
            return Marshal.PtrToStringAnsi(ptr, len);
        }
        public static string PtrToStringUni(System.IntPtr ptr)
        {
            return Marshal.PtrToStringUni(ptr);
        }
        public static string PtrToStringUni(System.IntPtr ptr, int len)
        {
            return Marshal.PtrToStringUni(ptr, len);
        }
        public static void PtrToStructure(System.IntPtr ptr, object structure)
        {
            Marshal.PtrToStructure(ptr, structure);
        }
        public static object PtrToStructure(System.IntPtr ptr, System.Type structureType)
        {
            return Marshal.PtrToStructure(ptr, structureType);
        }
        public static T PtrToStructure<T>(System.IntPtr ptr)
        {
            return Marshal.PtrToStructure<T>(ptr);
        }
        public static void PtrToStructure<T>(System.IntPtr ptr, T structure)
        {
            Marshal.PtrToStructure<T>(ptr, structure);
        }
        public static byte ReadByte(System.IntPtr ptr)
        {
            return Marshal.ReadByte(ptr);
        }
        public static byte ReadByte(System.IntPtr ptr, int offset)
        {
            return Marshal.ReadByte(ptr, offset);
        }
        public static byte ReadByte(object ptr, int offset)
        {
            return Marshal.ReadByte(ptr, offset);
        }
        public static short ReadInt16(System.IntPtr ptr)
        {
            return Marshal.ReadInt16(ptr);
        }
        public static short ReadInt16(System.IntPtr ptr, int offset)
        {
            return Marshal.ReadInt16(ptr, offset);
        }
        public static short ReadInt16(object ptr, int offset)
        {
            return Marshal.ReadInt16(ptr, offset);
        }
        public static int ReadInt32(System.IntPtr ptr)
        {
            return Marshal.ReadInt32(ptr);
        }
        public static int ReadInt32(System.IntPtr ptr, int offset)
        {
            return Marshal.ReadInt32(ptr, offset);
        }
        public static int ReadInt32(object ptr, int offset)
        {
            return Marshal.ReadInt32(ptr, offset);
        }
        public static long ReadInt64(System.IntPtr ptr)
        {
            return Marshal.ReadInt64(ptr);
        }
        public static long ReadInt64(System.IntPtr ptr, int offset)
        {
            return Marshal.ReadInt64(ptr, offset);
        }
        public static long ReadInt64(object ptr, int offset)
        {
            return Marshal.ReadInt64(ptr, offset);
        }
        public static System.IntPtr ReadIntPtr(System.IntPtr ptr)
        {
            return Marshal.ReadIntPtr(ptr);
        }
        public static System.IntPtr ReadIntPtr(System.IntPtr ptr, int offset)
        {
            return Marshal.ReadIntPtr(ptr, offset);
        }
        public static System.IntPtr ReadIntPtr(object ptr, int offset)
        {
            return Marshal.ReadIntPtr(ptr, offset);
        }
        public static System.IntPtr ReallocateMemory(System.IntPtr ptr, int sizeInBytes)
        {
            return Marshal.ReAllocCoTaskMem(ptr, sizeInBytes);
        }
        public static int SizeOf(object structure)
        {
            return Marshal.SizeOf(structure);
        }
        public static int SizeOf(System.Type type)
        {
            return Marshal.SizeOf(type);
        }
        public static int SizeOf<T>()
        {
            return Marshal.SizeOf<T>();
        }
        public static System.IntPtr StringToBSTR(string s)
        {
            return Marshal.StringToBSTR(s);
        }
        public static System.IntPtr StringToAllocatedMemoryAnsi(string s)
        {
            return Marshal.StringToCoTaskMemAnsi(s);
        }
        public static System.IntPtr StringToAllocatedMemoryUni(string s)
        {
            return Marshal.StringToCoTaskMemUni(s);
        }
        public static void StructureToPtr(object structure, System.IntPtr ptr, bool fDeleteOld)
        {
            Marshal.StructureToPtr(structure, ptr, fDeleteOld);
        }
        public static void StructureToPtr<T>(T structure, System.IntPtr ptr, bool fDeleteOld)
        {
            Marshal.StructureToPtr<T>(structure, ptr, fDeleteOld);
        }
        public static System.IntPtr UnsafeAddrOfPinnedArrayElement(System.Array arr, int index)
        {
            return Marshal.UnsafeAddrOfPinnedArrayElement(arr, index);
        }
        public static System.IntPtr UnsafeAddrOfPinnedArrayElement<T>(T[] arr, int index)
        {
            return Marshal.UnsafeAddrOfPinnedArrayElement<T>(arr, index);
        }
        public static void WriteInt32(object ptr, int offset, int value)
        {
            Marshal.WriteInt32(ptr, offset, value);
        }
        public static void ZeroFreeBSTR(System.IntPtr s)
        {
            Marshal.ZeroFreeBSTR(s);
        }
        public static void ZeroFreeMemoryAnsi(System.IntPtr s)
        {
            Marshal.ZeroFreeCoTaskMemAnsi(s);
        }
        public static void ZeroFreeMemoryUnicode(System.IntPtr s)
        {
            Marshal.ZeroFreeCoTaskMemUnicode(s);
        }
    }
}
