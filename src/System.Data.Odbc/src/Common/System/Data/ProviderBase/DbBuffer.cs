// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Data.ProviderBase
{
    // DbBuffer is abstract to require derived class to exist
    // so that when debugging, we can tell the difference between one DbBuffer and another
    internal abstract class DbBuffer : SafeHandle
    {
        private readonly int _bufferLength;

        protected DbBuffer(int initialSize) : base(IntPtr.Zero, true)
        {
            if (0 < initialSize)
            {
                _bufferLength = initialSize;
                RuntimeHelpers.PrepareConstrainedRegions();
                try { }
                finally
                {
                    base.handle = SafeNativeMethods.LocalAlloc((IntPtr)initialSize);
                }
                if (IntPtr.Zero == base.handle)
                {
                    throw new OutOfMemoryException();
                }
            }
        }

        protected DbBuffer(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
        {
        }

        private static int BaseOffset => 0;

        public override bool IsInvalid => (IntPtr.Zero == base.handle);

        internal int Length => _bufferLength;

        internal string PtrToStringUni(int offset)
        {
            offset += BaseOffset;
            Validate(offset, 2);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");

            string value = null;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                value = Marshal.PtrToStringUni(ptr);
                Validate(offset, (2 * (value.Length + 1)));
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }

            return value;
        }

        internal string PtrToStringUni(int offset, int length)
        {
            offset += BaseOffset;
            Validate(offset, 2 * length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");

            string value = null;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                value = Marshal.PtrToStringUni(ptr, length);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
            return value;
        }

        internal byte ReadByte(int offset)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 1);
            Debug.Assert(0 == offset % 4, "invalid alignment");

            byte value;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                value = Marshal.ReadByte(ptr, offset);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
            return value;
        }

        internal byte[] ReadBytes(int offset, int length)
        {
            byte[] value = new byte[length];
            ReadBytes(offset, value);
            return value;
        }

        internal void ReadBytes(int offset, Span<byte> destination)
        {
            offset += BaseOffset;
            Validate(offset, destination.Length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != destination, "null destination");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                unsafe
                {
                    new Span<byte>(ptr.ToPointer(), destination.Length).TryCopyTo(destination);
                }
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal char ReadChar(int offset)
        {
            short value = ReadInt16(offset);
            return unchecked((char)value);
        }

        internal void ReadChars(int offset, Span<char> destination)
        {
            offset += BaseOffset;
            Validate(offset, 2 * destination.Length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != destination, "null destination");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                unsafe
                {
                    new Span<char>(ptr.ToPointer(), destination.Length).CopyTo(destination);
                }
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal double ReadDouble(int offset)
        {
            long value = ReadInt64(offset);
            return BitConverter.Int64BitsToDouble(value);
        }

        internal short ReadInt16(int offset)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 2);
            Debug.Assert(0 == offset % 2, "invalid alignment");

            short value;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                value = Marshal.ReadInt16(ptr, offset);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
            return value;
        }

        internal void ReadInt16Array(int offset, Span<short> destination)
        {
            offset += BaseOffset;
            Validate(offset, 2 * destination.Length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != destination, "null destination");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                unsafe
                {
                    new Span<short>(ptr.ToPointer(), destination.Length).TryCopyTo(destination);
                }
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal int ReadInt32(int offset)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 4);
            Debug.Assert(0 == offset % 4, "invalid alignment");

            int value;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                value = Marshal.ReadInt32(ptr, offset);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
            return value;
        }

        //TODO: Remove unused method?
        internal void ReadInt32Array(int offset, int[] destination, int startIndex, int length)
        {
            offset += BaseOffset;
            Validate(offset, 4 * length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != destination, "null destination");
            Debug.Assert(startIndex + length <= destination.Length, "destination too small");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                Marshal.Copy(ptr, destination, startIndex, length);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal long ReadInt64(int offset)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 8);
            Debug.Assert(0 == offset % IntPtr.Size, "invalid alignment");

            long value;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                value = Marshal.ReadInt64(ptr, offset);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
            return value;
        }

        internal IntPtr ReadIntPtr(int offset)
        {
            offset += BaseOffset;
            ValidateCheck(offset, IntPtr.Size);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");

            IntPtr value;
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                value = Marshal.ReadIntPtr(ptr, offset);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
            return value;
        }

        internal unsafe float ReadSingle(int offset)
        {
            int value = ReadInt32(offset);
            return *(float*)&value;
        }

        protected override bool ReleaseHandle()
        {
            // NOTE: The SafeHandle class guarantees this will be called exactly once.
            IntPtr ptr = base.handle;
            base.handle = IntPtr.Zero;
            if (IntPtr.Zero != ptr)
            {
                SafeNativeMethods.LocalFree(ptr);
            }
            return true;
        }

        private void StructureToPtr(int offset, object structure)
        {
            Debug.Assert(null != structure, "null structure");
            offset += BaseOffset;
            ValidateCheck(offset, Marshal.SizeOf(structure.GetType()));
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                Marshal.StructureToPtr(structure, ptr, false/*fDeleteOld*/);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteByte(int offset, byte value)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 1);
            Debug.Assert(0 == offset % 4, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                Marshal.WriteByte(ptr, offset, value);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteBytes(int offset, ReadOnlySpan<byte> source)
        {
            offset += BaseOffset;
            Validate(offset, source.Length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                unsafe
                {
                    source.CopyTo(new Span<byte>(ptr.ToPointer(), source.Length));
                }
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteCharArray(int offset, ReadOnlySpan<char> source)
        {
            offset += BaseOffset;
            Validate(offset, 2 * source.Length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != source, "null source");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                unsafe
                {
                    source.CopyTo(new Span<char>(ptr.ToPointer(), source.Length));
                }
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteDouble(int offset, double value)
        {
            WriteInt64(offset, BitConverter.DoubleToInt64Bits(value));
        }

        internal void WriteInt16(int offset, short value)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 2);
            Debug.Assert(0 == offset % 2, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                Marshal.WriteInt16(ptr, offset, value);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteInt16Array(int offset, ReadOnlySpan<short> source)
        {
            offset += BaseOffset;
            Validate(offset, 2 * source.Length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != source, "null source");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                unsafe
                {
                    source.CopyTo(new Span<short>(ptr.ToPointer(), source.Length));
                }
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteInt32(int offset, int value)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 4);
            Debug.Assert(0 == offset % 4, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                Marshal.WriteInt32(ptr, offset, value);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        //TODO: Remove unused method?
        internal void WriteInt32Array(int offset, int[] source, int startIndex, int length)
        {
            offset += BaseOffset;
            Validate(offset, 4 * length);
            Debug.Assert(0 == offset % ADP.PtrSize, "invalid alignment");
            Debug.Assert(null != source, "null source");
            Debug.Assert(startIndex + length <= source.Length, "source too small");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = ADP.IntPtrOffset(DangerousGetHandle(), offset);
                Marshal.Copy(source, startIndex, ptr, length);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteInt64(int offset, long value)
        {
            offset += BaseOffset;
            ValidateCheck(offset, 8);
            Debug.Assert(0 == offset % IntPtr.Size, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                Marshal.WriteInt64(ptr, offset, value);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal void WriteIntPtr(int offset, IntPtr value)
        {
            offset += BaseOffset;
            ValidateCheck(offset, IntPtr.Size);
            Debug.Assert(0 == offset % IntPtr.Size, "invalid alignment");

            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                Marshal.WriteIntPtr(ptr, offset, value);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal unsafe void WriteSingle(int offset, float value)
        {
            WriteInt32(offset, *(int*)&value);
        }

        internal void ZeroMemory()
        {
            bool mustRelease = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                DangerousAddRef(ref mustRelease);

                IntPtr ptr = DangerousGetHandle();
                SafeNativeMethods.ZeroMemory(ptr, Length);
            }
            finally
            {
                if (mustRelease)
                {
                    DangerousRelease();
                }
            }
        }

        internal Guid ReadGuid(int offset)
        {
            // faster than Marshal.PtrToStructure(offset, typeof(Guid))
            Span<byte> buffer = stackalloc byte[16];
            ReadBytes(offset, buffer);
            return new Guid(buffer);
        }
        internal void WriteGuid(int offset, Guid value)
        {
            // faster than Marshal.Copy(value.GetByteArray()
            StructureToPtr(offset, value);
        }

        internal DateTime ReadDate(int offset)
        {
            Span<short> buffer = stackalloc short[3];
            ReadInt16Array(offset, buffer);
            return new DateTime(
                unchecked((ushort)buffer[0]),   // Year
                unchecked((ushort)buffer[1]),   // Month
                unchecked((ushort)buffer[2]));  // Day
        }
        internal void WriteDate(int offset, DateTime value)
        {
            Span<short> buffer = stackalloc short[3] {
                unchecked((short)value.Year),
                unchecked((short)value.Month),
                unchecked((short)value.Day),
            };
            WriteInt16Array(offset, buffer);
        }

        internal TimeSpan ReadTime(int offset)
        {
            Span<short> buffer = stackalloc short[3];
            ReadInt16Array(offset, buffer);
            return new TimeSpan(
                unchecked((ushort)buffer[0]),   // Hours
                unchecked((ushort)buffer[1]),   // Minutes
                unchecked((ushort)buffer[2]));  // Seconds
        }
        internal void WriteTime(int offset, TimeSpan value)
        {
            Span<short> buffer = stackalloc short[3] {
                unchecked((short)value.Hours),
                unchecked((short)value.Minutes),
                unchecked((short)value.Seconds),
            };
            WriteInt16Array(offset, buffer);
        }

        internal DateTime ReadDateTime(int offset)
        {
            Span<short> buffer = stackalloc short[6];
            ReadInt16Array(offset, buffer);
            int ticks = ReadInt32(offset + 12);
            DateTime value = new DateTime(
                unchecked((ushort)buffer[0]),  // Year
                unchecked((ushort)buffer[1]),  // Month
                unchecked((ushort)buffer[2]),  // Day
                unchecked((ushort)buffer[3]),  // Hours
                unchecked((ushort)buffer[4]),  // Minutes
                unchecked((ushort)buffer[5])); // Seconds
            return value.AddTicks(ticks / 100);
        }
        internal void WriteDateTime(int offset, DateTime value)
        {
            int ticks = (int)(value.Ticks % 10000000L) * 100;
            Span<short> buffer = stackalloc short[6] {
                unchecked((short)value.Year),
                unchecked((short)value.Month),
                unchecked((short)value.Day),
                unchecked((short)value.Hour),
                unchecked((short)value.Minute),
                unchecked((short)value.Second),
            };
            WriteInt16Array(offset, buffer);
            WriteInt32(offset + 12, ticks);
        }

        internal decimal ReadNumeric(int offset)
        {
            Span<byte> bits = stackalloc byte[20];
            ReadBytes(offset, bits.Slice(1, 19));

            //TODO: Refactor when span -> decimal
            int[] buffer = new int[4];
            buffer[3] = ((int)bits[2]) << 16; // scale
            if (0 == bits[3])
            {
                buffer[3] |= unchecked((int)0x80000000); //sign
            }
            buffer[0] = BitConverter.ToInt32(bits.Slice(4));     // low
            buffer[1] = BitConverter.ToInt32(bits.Slice(8));     // mid
            buffer[2] = BitConverter.ToInt32(bits.Slice(12));     // high
            if (0 != BitConverter.ToInt32(bits.Slice(16)))
            {
                throw ADP.NumericToDecimalOverflow();
            }
            return new decimal(buffer);
        }

        internal void WriteNumeric(int offset, decimal value, byte precision)
        {
            //TODO: Refactor when decimal -> span
            Span<byte> tmp = MemoryMarshal.Cast<int, byte>(decimal.GetBits(value));
            Span<byte> buffer = stackalloc byte[20];

            buffer[1] = precision;
            tmp.Slice(14, 2).CopyTo(buffer.Slice(2));
            buffer[3] = (byte)((0 == buffer[3]) ? 1 : 0); // flip sign for native
            tmp.Slice(0, 12).CopyTo(buffer.Slice(4));
            buffer[16] = 0;
            buffer[17] = 0;
            buffer[18] = 0;
            buffer[19] = 0;
            WriteBytes(offset, buffer.Slice(1, 19));
        }

        [ConditionalAttribute("DEBUG")]
        protected void ValidateCheck(int offset, int count)
        {
            Validate(offset, count);
        }

        protected void Validate(int offset, int count)
        {
            if ((offset < 0) || (count < 0) || (Length < checked(offset + count)))
            {
                throw ADP.InternalError(ADP.InternalErrorCode.InvalidBuffer);
            }
        }
    }
}
