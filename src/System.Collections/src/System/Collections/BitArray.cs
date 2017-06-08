// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace System.Collections
{
    // A vector of bits.  Use this to store bits efficiently, without having to do bit
    // shifting yourself.
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class BitArray : ICollection, ICloneable
    {
        /*=========================================================================
        ** Allocates space to hold length bit values. All of the values in the bit
        ** array are set to false.
        **
        ** Exceptions: ArgumentException if length < 0.
        =========================================================================*/
        public BitArray(int length)
            : this(length, false)
        {
        }

        /*=========================================================================
        ** Allocates space to hold length bit values. All of the values in the bit
        ** array are set to defaultValue.
        **
        ** Exceptions: ArgumentOutOfRangeException if length < 0.
        =========================================================================*/
        public BitArray(int length, bool defaultValue)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            Contract.EndContractBlock();

            m_array = new uint[GetArrayLength(length, BitsPerUInt32)];
            m_length = length;

            uint fillValue = defaultValue ? 0xffffffffU : 0x00000000U;
            Array.Fill(m_array, fillValue);

            _version = 0;
        }

        /*=========================================================================
        ** Allocates space to hold the bit values in bytes. bytes[0] represents
        ** bits 0 - 7, bytes[1] represents bits 8 - 15, etc. The LSB of each byte
        ** represents the lowest index value; bytes[0] & 1 represents bit 0,
        ** bytes[0] & 2 represents bit 1, bytes[0] & 4 represents bit 2, etc.
        **
        ** Exceptions: ArgumentException if bytes == null.
        =========================================================================*/
        public BitArray(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            Contract.EndContractBlock();
            // this value is chosen to prevent overflow when computing m_length.
            // m_length is of type int32 and is exposed as a property, so
            // type of m_length can't be changed to accommodate.
            if (bytes.Length > int.MaxValue / BitsPerByte)
            {
                throw new ArgumentException(SR.Format(SR.Argument_ArrayTooLarge, BitsPerByte), nameof(bytes));
            }

            m_array = new uint[GetArrayLength(bytes.Length, BytesPerUInt32)];
            m_length = bytes.Length * BitsPerByte;

            Buffer.BlockCopy(bytes, 0, m_array, 0, bytes.Length);
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < m_array.Length; i++)
                {
                    // Swap byte order
                    m_array[i] = ((m_array[i] & 0x000000ffU) << 3 * BitsPerByte)
                        | ((m_array[i] & 0x0000ff00U) << BitsPerByte)
                        | ((m_array[i] & 0x00ff0000U) >> BitsPerByte)
                        | ((m_array[i] & 0xff000000U) >> 3 * BitsPerByte);
                }
            }

            _version = 0;
        }

        public BitArray(bool[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            Contract.EndContractBlock();

            m_array = new uint[GetArrayLength(values.Length, BitsPerUInt32)];
            m_length = values.Length;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                    m_array[i / BitsPerUInt32] |= (0x01U << (i % BitsPerUInt32));
            }

            _version = 0;
        }

        /*=========================================================================
        ** Allocates space to hold the bit values in values. values[0] represents
        ** bits 0 - 31, values[1] represents bits 32 - 63, etc. The LSB of each
        ** integer represents the lowest index value; values[0] & 1 represents bit
        ** 0, values[0] & 2 represents bit 1, values[0] & 4 represents bit 2, etc.
        **
        ** Exceptions: ArgumentException if values == null.
        =========================================================================*/
        public BitArray(int[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            Contract.EndContractBlock();
            // this value is chosen to prevent overflow when computing m_length
            if (values.Length > int.MaxValue / BitsPerUInt32)
            {
                throw new ArgumentException(SR.Format(SR.Argument_ArrayTooLarge, BitsPerUInt32), nameof(values));
            }

            m_array = new uint[values.Length];
            Buffer.BlockCopy(values, 0, m_array, 0, m_array.Length * BytesPerUInt32);
            m_length = values.Length * BitsPerUInt32;

            _version = 0;
        }

        /*=========================================================================
        ** Allocates a new BitArray with the same length and bit values as bits.
        **
        ** Exceptions: ArgumentException if bits == null.
        =========================================================================*/
        public BitArray(BitArray bits)
        {
            if (bits == null)
            {
                throw new ArgumentNullException(nameof(bits));
            }
            Contract.EndContractBlock();

            int arrayLength = GetArrayLength(bits.m_length, BitsPerUInt32);

            m_array = new uint[arrayLength];
            Array.Copy(bits.m_array, m_array, arrayLength);
            m_length = bits.m_length;

            _version = bits._version;
        }

        public bool this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }

        /*=========================================================================
        ** Returns the bit value at position index.
        **
        ** Exceptions: ArgumentOutOfRangeException if index < 0 or
        **             index >= GetLength().
        =========================================================================*/
        public bool Get(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
            }
            Contract.EndContractBlock();

            return (m_array[index / BitsPerUInt32] & (0x01U << (index % BitsPerUInt32))) != 0;
        }

        /*=========================================================================
        ** Sets the bit value at position index to value.
        **
        ** Exceptions: ArgumentOutOfRangeException if index < 0 or
        **             index >= GetLength().
        =========================================================================*/
        public void Set(int index, bool value)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
            }
            Contract.EndContractBlock();

            if (value)
            {
                m_array[index / BitsPerUInt32] |= (0x01U << (index % BitsPerUInt32));
            }
            else
            {
                m_array[index / BitsPerUInt32] &= ~(0x01U << (index % BitsPerUInt32));
            }

            _version++;
        }

        /*=========================================================================
        ** Sets all the bit values to value.
        =========================================================================*/
        public void SetAll(bool value)
        {
            uint fillValue = value ? 0xffffffffU : 0x00000000U;
            int ints = GetArrayLength(m_length, BitsPerUInt32);
            for (int i = 0; i < ints; i++)
            {
                m_array[i] = fillValue;
            }

            _version++;
        }

        /*=========================================================================
        ** Returns a reference to the current instance ANDed with value.
        **
        ** Exceptions: ArgumentException if value == null or
        **             value.Length != this.Length.
        =========================================================================*/
        public BitArray And(BitArray value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (Length != value.Length)
                throw new ArgumentException(SR.Arg_ArrayLengthsDiffer);
            Contract.EndContractBlock();

            int ints = GetArrayLength(m_length, BitsPerUInt32);
            for (int i = 0; i < ints; i++)
            {
                m_array[i] &= value.m_array[i];
            }

            _version++;
            return this;
        }

        /*=========================================================================
        ** Returns a reference to the current instance ORed with value.
        **
        ** Exceptions: ArgumentException if value == null or
        **             value.Length != this.Length.
        =========================================================================*/
        public BitArray Or(BitArray value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (Length != value.Length)
                throw new ArgumentException(SR.Arg_ArrayLengthsDiffer);
            Contract.EndContractBlock();

            int ints = GetArrayLength(m_length, BitsPerUInt32);
            for (int i = 0; i < ints; i++)
            {
                m_array[i] |= value.m_array[i];
            }

            _version++;
            return this;
        }

        /*=========================================================================
        ** Returns a reference to the current instance XORed with value.
        **
        ** Exceptions: ArgumentException if value == null or
        **             value.Length != this.Length.
        =========================================================================*/
        public BitArray Xor(BitArray value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (Length != value.Length)
                throw new ArgumentException(SR.Arg_ArrayLengthsDiffer);
            Contract.EndContractBlock();

            int ints = GetArrayLength(m_length, BitsPerUInt32);
            for (int i = 0; i < ints; i++)
            {
                m_array[i] ^= value.m_array[i];
            }

            _version++;
            return this;
        }

        /*=========================================================================
        ** Inverts all the bit values. On/true bit values are converted to
        ** off/false. Off/false bit values are turned on/true. The current instance
        ** is updated and returned.
        =========================================================================*/
        public BitArray Not()
        {
            int ints = GetArrayLength(m_length, BitsPerUInt32);
            for (int i = 0; i < ints; i++)
            {
                m_array[i] = ~m_array[i];
            }

            _version++;
            return this;
        }

        /*=========================================================================
        ** Shift all the bit values to right on count bits. The current instance is
        ** updated and returned.
        *
        ** Exceptions: ArgumentOutOfRangeException if count < 0
        =========================================================================*/
        public BitArray RightShift(int count)
        {
            if (count <= 0)
            {
                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                _version++;
                return this;
            }

            int toIndex = 0;
            int ints = GetArrayLength(m_length, BitsPerUInt32);
            if (count < m_length)
            {
                int shiftCount;
                // We can not use Math.DivRem without taking a dependency on System.Runtime.Extensions
                int fromIndex = count / BitsPerUInt32;
                shiftCount = count - fromIndex * BitsPerUInt32; // Optimized Rem

                if (shiftCount == 0)
                {
                    uint mask = uint.MaxValue >> (BitsPerUInt32 - m_length % BitsPerUInt32);
                    m_array[ints - 1] &= mask;

                    Array.Copy(m_array, fromIndex, m_array, 0, ints - fromIndex);
                    toIndex = ints - fromIndex;
                }
                else
                {
                    int lastIndex = ints - 1;
                    while (fromIndex < lastIndex)
                    {
                        uint right = m_array[fromIndex] >> shiftCount;
                        uint left = m_array[++fromIndex] << (BitsPerUInt32 - shiftCount);
                        m_array[toIndex++] = left | right;
                    }
                    uint mask = uint.MaxValue >> (BitsPerUInt32 - m_length % BitsPerUInt32);
                    mask &= m_array[fromIndex];
                    m_array[toIndex++] = mask >> shiftCount;
                }
            }

            Array.Clear(m_array, toIndex, ints - toIndex);
            _version++;
            return this;
        }

        /*=========================================================================
        ** Shift all the bit values to left on count bits. The current instance is
        ** updated and returned.
        *
        ** Exceptions: ArgumentOutOfRangeException if count < 0
        =========================================================================*/
        public BitArray LeftShift(int count)
        {
            if (count <= 0)
            {
                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                _version++;
                return this;
            }

            int lengthToClear;
            if (count < m_length)
            {
                int lastIndex = (m_length - 1) / BitsPerUInt32;

                int shiftCount;
                // We can not use Math.DivRem without taking a dependency on System.Runtime.Extensions
                lengthToClear = count / BitsPerUInt32;
                shiftCount = count - lengthToClear * BitsPerUInt32; // Optimized Rem

                if (shiftCount == 0)
                {
                    Array.Copy(m_array, 0, m_array, lengthToClear, lastIndex + 1 - lengthToClear);
                }
                else
                {
                    int fromindex = lastIndex - lengthToClear;
                    while (fromindex > 0)
                    {
                        uint left = m_array[fromindex] << shiftCount;
                        uint right = m_array[--fromindex] >> (BitsPerUInt32 - shiftCount);
                        m_array[lastIndex] = left | right;
                        lastIndex--;
                    }
                    m_array[lastIndex] = m_array[fromindex] << shiftCount;
                }
            }
            else
            {
                lengthToClear = GetArrayLength(m_length, BitsPerUInt32); // Clear all
            }

            Array.Clear(m_array, 0, lengthToClear);
            _version++;
            return this;
        }

        public int Length
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return m_length;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.ArgumentOutOfRange_NeedNonNegNum);
                }
                Contract.EndContractBlock();

                int newints = GetArrayLength(value, BitsPerUInt32);
                if (newints > m_array.Length || newints + _ShrinkThreshold < m_array.Length)
                {
                    // grow or shrink (if wasting more than _ShrinkThreshold ints)
                    Array.Resize(ref m_array, newints);
                }

                if (value > m_length)
                {
                    // clear high bit values in the last int
                    int last = GetArrayLength(m_length, BitsPerUInt32) - 1;
                    int bits = m_length % BitsPerUInt32;
                    if (bits > 0)
                    {
                        m_array[last] &= (0x01U << bits) - 1U;
                    }

                    // clear remaining int values
                    Array.Clear(m_array, last + 1, newints - last - 1);
                }

                m_length = value;
                _version++;
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);

            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, nameof(array));

            Contract.EndContractBlock();

            int[] intArray = array as int[];
            if (intArray != null)
            {
                int extraBits = m_length % BitsPerUInt32;
                int arrayLength = GetArrayLength(m_length, BitsPerUInt32);

                if ((array.Length - index) < arrayLength)
                    throw new ArgumentException(SR.Argument_InvalidOffLen, "destinationArray");

                if (extraBits > 0)
                {
                    arrayLength -= 1;
                }

                if (index < int.MaxValue / BytesPerUInt32)
                {
                    Buffer.BlockCopy(m_array, 0, array, index * BytesPerUInt32, arrayLength * BytesPerUInt32);
                }
                else
                {
                    int[] ia = (int[])array;
                    for (int i = 0; i < arrayLength; i++)
                    {
                        ia[i + index] = unchecked((int)m_array[i]);
                    }
                }

                if (extraBits > 0)
                {
                    // the last int needs to be masked
                    intArray[index + arrayLength] = unchecked((int)(m_array[arrayLength] & ((0x01U << extraBits) - 1U)));
                }
            }
            else if (array is byte[])
            {
                int extraBits = m_length % BitsPerByte;

                int arrayLength = GetArrayLength(m_length, BitsPerByte);
                if ((array.Length - index) < arrayLength)
                    throw new ArgumentException(SR.Argument_InvalidOffLen);

                if (extraBits > 0)
                {
                    // last byte is not aligned, we will directly copy one less byte
                    arrayLength -= 1;
                }

                byte[] b = (byte[])array;

                // copy all the perfectly-aligned bytes
                if (BitConverter.IsLittleEndian)
                {
                    Buffer.BlockCopy(m_array, 0, b, index, arrayLength);
                }
                else
                {
                    for (int i = 0; i < arrayLength; i++)
                        b[index + i] = (byte)((m_array[i / BytesPerUInt32] >> ((i % BytesPerUInt32) * BitsPerByte)) & 0xffU); // Shift to bring the required byte to LSB, then mask
                }

                if (extraBits > 0)
                {
                    // mask the final byte
                    int i = arrayLength;
                    b[index + i] = (byte)((m_array[i / BytesPerUInt32] >> ((i % BytesPerUInt32) * BitsPerByte)) & ((0x01U << extraBits) - 1U));
                }
            }
            else if (array is bool[])
            {
                if (array.Length - index < m_length)
                    throw new ArgumentException(SR.Argument_InvalidOffLen);

                bool[] b = (bool[])array;
                for (int i = 0; i < m_length; i++)
                    b[index + i] = ((m_array[i / BitsPerUInt32] >> (i % BitsPerUInt32)) & 0x01U) != 0;
            }
            else
                throw new ArgumentException(SR.Arg_BitArrayTypeUnsupported, nameof(array));
        }

        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return m_length;
            }
        }

        public Object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object Clone()
        {
            return new BitArray(this);
        }

        public IEnumerator GetEnumerator()
        {
            return new BitArrayEnumeratorSimple(this);
        }

        // XPerY=n means that n Xs can be stored in 1 Y.
        private const int BitsPerUInt32 = 32;
        private const int BytesPerUInt32 = 4;
        private const int BitsPerByte = 8;

        /// <summary>
        /// Used for conversion between different representations of bit array.
        /// Returns (n+(div-1))/div, rearranged to avoid arithmetic overflow.
        /// For example, in the bit to int case, the straightforward calc would
        /// be (n+31)/32, but that would cause overflow. So instead it's
        /// rearranged to ((n-1)/32) + 1, with special casing for 0.
        ///
        /// Usage:
        /// GetArrayLength(77, BitsPerUInt32): returns how many ints must be
        /// allocated to store 77 bits.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="div">use a conversion constant, e.g. BytesPerInt32 to get
        /// how many ints are required to store n bytes</param>
        /// <returns></returns>
        private static int GetArrayLength(int n, int div)
        {
            Debug.Assert(div > 0, "GetArrayLength: div arg must be greater than 0");
            return n > 0 ? (((n - 1) / div) + 1) : 0;
        }

        private class BitArrayEnumeratorSimple : IEnumerator, ICloneable
        {
            private BitArray bitarray;
            private int index;
            private int version;
            private bool currentElement;

            internal BitArrayEnumeratorSimple(BitArray bitarray)
            {
                this.bitarray = bitarray;
                this.index = -1;
                version = bitarray._version;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }

            public virtual bool MoveNext()
            {
                ICollection bitarrayAsICollection = bitarray;
                if (version != bitarray._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                if (index < (bitarrayAsICollection.Count - 1))
                {
                    index++;
                    currentElement = bitarray.Get(index);
                    return true;
                }
                else
                    index = bitarrayAsICollection.Count;

                return false;
            }

            public virtual object Current
            {
                get
                {
                    if (index == -1)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                    if (index >= ((ICollection)bitarray).Count)
                        throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
                    return currentElement;
                }
            }

            public void Reset()
            {
                if (version != bitarray._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                index = -1;
            }
        }

        private uint[] m_array;
        private int m_length;
        private int _version;
        [NonSerialized]
        private object _syncRoot;

        private const int _ShrinkThreshold = 256;
    }
}
