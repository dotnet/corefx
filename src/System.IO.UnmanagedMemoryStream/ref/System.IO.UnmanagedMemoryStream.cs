// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.IO.UnmanagedMemoryStream))]

namespace System.IO
{
    public partial class UnmanagedMemoryAccessor : System.IDisposable
    {
        protected UnmanagedMemoryAccessor() { }
        public UnmanagedMemoryAccessor(System.Runtime.InteropServices.SafeBuffer buffer, long offset, long capacity) { }
        public UnmanagedMemoryAccessor(System.Runtime.InteropServices.SafeBuffer buffer, long offset, long capacity, System.IO.FileAccess access) { }
        public bool CanRead { get { return default(bool); } }
        public bool CanWrite { get { return default(bool); } }
        public long Capacity { get { return default(long); } }
        protected bool IsOpen { get { return default(bool); } }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        protected void Initialize(System.Runtime.InteropServices.SafeBuffer buffer, long offset, long capacity, System.IO.FileAccess access) { }
        public bool ReadBoolean(long position) { return default(bool); }
        public byte ReadByte(long position) { return default(byte); }
        public char ReadChar(long position) { return default(char); }
        public decimal ReadDecimal(long position) { return default(decimal); }
        public double ReadDouble(long position) { return default(double); }
        public short ReadInt16(long position) { return default(short); }
        public int ReadInt32(long position) { return default(int); }
        public long ReadInt64(long position) { return default(long); }
        [System.CLSCompliantAttribute(false)]
        public sbyte ReadSByte(long position) { return default(sbyte); }
        public float ReadSingle(long position) { return default(float); }
        [System.CLSCompliantAttribute(false)]
        public ushort ReadUInt16(long position) { return default(ushort); }
        [System.CLSCompliantAttribute(false)]
        public uint ReadUInt32(long position) { return default(uint); }
        [System.CLSCompliantAttribute(false)]
        public ulong ReadUInt64(long position) { return default(ulong); }
        public void Read<T>(long position, out T structure) where T : struct { throw null; }
        public int ReadArray<T>(long position, T[] array, int offset, int count) where T : struct { throw null; }
        public void Write(long position, bool value) { }
        public void Write(long position, byte value) { }
        public void Write(long position, char value) { }
        public void Write(long position, decimal value) { }
        public void Write(long position, double value) { }
        public void Write(long position, short value) { }
        public void Write(long position, int value) { }
        public void Write(long position, long value) { }
        [System.CLSCompliantAttribute(false)]
        public void Write(long position, sbyte value) { }
        public void Write(long position, float value) { }
        [System.CLSCompliantAttribute(false)]
        public void Write(long position, ushort value) { }
        [System.CLSCompliantAttribute(false)]
        public void Write(long position, uint value) { }
        [System.CLSCompliantAttribute(false)]
        public void Write(long position, ulong value) { }
        public void Write<T>(long position, ref T structure) where T : struct { throw null; }
        public void WriteArray<T>(long position, T[] array, int offset, int count) where T : struct { throw null; }
    }
}
