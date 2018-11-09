// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections
{
    // A vector of bits.  Use this to store bits efficiently, without having to do bit 
    // shifting yourself.
    [Serializable]
    [Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class BitArray : ICollection, ICloneable
    {
        private int[] m_array; // Do not rename (binary serialization)
        private int m_length; // Do not rename (binary serialization)
        private int _version; // Do not rename (binary serialization)
        [NonSerialized]
        private object _syncRoot;

        private const int _ShrinkThreshold = 256;

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

            m_array = new int[GetIntegerArrayLengthFromBitLength(length)];
            m_length = length;

            if (defaultValue)
            {
                int fillValue = unchecked((int)0xffffffff);
                for (int i = 0; i < m_array.Length; i++)
                {
                    m_array[i] = fillValue;
                }
            }

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

            // this value is chosen to prevent overflow when computing m_length.
            // m_length is of type int32 and is exposed as a property, so 
            // type of m_length can't be changed to accommodate.
            if (bytes.Length > int.MaxValue >> BitShiftPerByte) // Divide by 8.
            {
                throw new ArgumentException(SR.Format(SR.Argument_ArrayTooLarge, BitsPerByte), nameof(bytes));
            }

            m_array = new int[GetIntegerArrayLengthFromByteLength(bytes.Length)];
            m_length = bytes.Length * BitsPerByte;

            int i = 0;
            int j = 0;
            int totalCount = bytes.Length >> 2;     // Divide by 4.

            for (; i < totalCount; i++)
            {
                m_array[i] = (bytes[j + 3] << 24) | (bytes[j + 2] << 16) | (bytes[j + 1] << 8) | bytes[j];
                j += 4;
            }

            Debug.Assert(bytes.Length - j >= 0, "BitArray byteLength problem");
            Debug.Assert(bytes.Length - j < 4, "BitArray byteLength problem #2");
            Debug.Assert(i == totalCount);

            switch (bytes.Length - j)
            {
                case 3:
                    m_array[i] = bytes[j + 2] << 16;
                    goto case 2;
                // fall through
                case 2:
                    m_array[i] |= bytes[j + 1] << 8;
                    goto case 1;
                // fall through
                case 1:
                    m_array[i] |= bytes[j];
                    break;
            }

            _version = 0;
        }

        public BitArray(bool[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            m_array = new int[GetIntegerArrayLengthFromBitLength(values.Length)];
            m_length = values.Length;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    int quotient = Div32Rem(i, out int remainder);
                    m_array[quotient] |= 1 << remainder;
                }
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

            // this value is chosen to prevent overflow when computing m_length
            if (values.Length > int.MaxValue >> BitShiftPerInt32)   // Divide by 32.
            {
                throw new ArgumentException(SR.Format(SR.Argument_ArrayTooLarge, BitsPerInt32), nameof(values));
            }

            m_array = new int[values.Length];
            values.AsSpan().CopyTo(m_array);
            m_length = values.Length * BitsPerInt32;

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

            int arrayLength = GetIntegerArrayLengthFromBitLength(bits.m_length);

            m_array = new int[arrayLength];

            Debug.Assert(bits.m_array.Length <= arrayLength);

            bits.m_array.AsSpan().CopyTo(m_array);
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
            if ((uint)index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
            }

            int quotient = index >> BitShiftPerInt32;   // Divide by 32.
            int remainder = index - (quotient << BitShiftPerInt32);   // Multiply by 32.

            return (m_array[quotient] & (1 << remainder)) != 0;
        }

        /*=========================================================================
        ** Sets the bit value at position index to value.
        **
        ** Exceptions: ArgumentOutOfRangeException if index < 0 or
        **             index >= GetLength().
        =========================================================================*/
        public void Set(int index, bool value)
        {
            if ((uint)index >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
            }

            int quotient = index >> BitShiftPerInt32;   // Divide by 32.
            int remainder = index - (quotient << BitShiftPerInt32);   // Multiply by 32.

            if (value)
            {
                m_array[quotient] |= 1 << remainder;
            }
            else
            {
                m_array[quotient] &= ~(1 << remainder);
            }

            _version++;
        }

        /*=========================================================================
        ** Sets all the bit values to value.
        =========================================================================*/
        public void SetAll(bool value)
        {
            int ints = GetIntegerArrayLengthFromBitLength(m_length);
            if (value)
            {
                int fillValue = unchecked((int)0xffffffff);

                for (int i = 0; i < ints; i++)
                {
                    m_array[i] = fillValue;
                }
            }
            else
            {
                m_array.AsSpan(0, ints).Clear();
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

            int ints = GetIntegerArrayLengthFromBitLength(m_length);
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

            int ints = GetIntegerArrayLengthFromBitLength(m_length);
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

            int ints = GetIntegerArrayLengthFromBitLength(m_length);
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
            int ints = GetIntegerArrayLengthFromBitLength(m_length);
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
            int ints = GetIntegerArrayLengthFromBitLength(m_length);
            if (count < m_length)
            {
                // We can not use Math.DivRem without taking a dependency on System.Runtime.Extensions
                int fromIndex = Div32Rem(count, out int shiftCount);
                Div32Rem(m_length, out int remainder);
                if (shiftCount == 0)
                {
                    unchecked
                    {
                        uint mask = uint.MaxValue >> (BitsPerInt32 - remainder);
                        m_array[ints - 1] &= (int)mask;
                    }
                    m_array.AsSpan(fromIndex, ints - fromIndex).CopyTo(m_array);
                    toIndex = ints - fromIndex;
                }
                else
                {
                    int lastIndex = ints - 1;
                    unchecked
                    {
                        while (fromIndex < lastIndex)
                        {
                            uint right = (uint)m_array[fromIndex] >> shiftCount;
                            int left = m_array[++fromIndex] << (BitsPerInt32 - shiftCount);
                            m_array[toIndex++] = left | (int)right;
                        }
                        uint mask = uint.MaxValue >> (BitsPerInt32 - remainder);
                        mask &= (uint)m_array[fromIndex];
                        m_array[toIndex++] = (int)(mask >> shiftCount);
                    }
                }
            }

            m_array.AsSpan(toIndex, ints - toIndex).Clear();
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
                int lastIndex = (m_length - 1) >> BitShiftPerInt32;  // Divide by 32.

                // We can not use Math.DivRem without taking a dependency on System.Runtime.Extensions
                lengthToClear = Div32Rem(count, out int shiftCount);

                if (shiftCount == 0)
                {
                    m_array.AsSpan(0, lastIndex + 1 - lengthToClear).CopyTo(m_array.AsSpan(lengthToClear));
                }
                else
                {
                    int fromindex = lastIndex - lengthToClear;
                    unchecked
                    {
                        while (fromindex > 0)
                        {
                            int left = m_array[fromindex] << shiftCount;
                            uint right = (uint)m_array[--fromindex] >> (BitsPerInt32 - shiftCount);
                            m_array[lastIndex] = left | (int)right;
                            lastIndex--;
                        }
                        m_array[lastIndex] = m_array[fromindex] << shiftCount;
                    }
                }
            }
            else
            {
                lengthToClear = GetIntegerArrayLengthFromBitLength(m_length); // Clear all
            }

            m_array.AsSpan(0, lengthToClear).Clear();
            _version++;
            return this;
        }

        public int Length
        {
            get
            {
                return m_length;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.ArgumentOutOfRange_NeedNonNegNum);
                }

                int newints = GetIntegerArrayLengthFromBitLength(value);

                // grow or shrink (if wasting more than _ShrinkThreshold ints)
                if (newints > m_array.Length)
                {
                    var newArray = new int[newints];
                    m_array.AsSpan().CopyTo(newArray);
                    m_array = newArray;
                }
                else if (newints + _ShrinkThreshold < m_array.Length)
                {
                    var newArray = new int[newints];
                    m_array.AsSpan(0, newints).CopyTo(newArray);
                    m_array = newArray;
                }

                if (value > m_length)
                {
                    // clear high bit values in the last int
                    int last = (m_length - 1) >> BitShiftPerInt32;
                    Div32Rem(m_length, out int bits);
                    if (bits > 0)
                    {
                        m_array[last] &= (1 << bits) - 1;
                    }

                    // clear remaining int values
                    m_array.AsSpan(last + 1, newints - last - 1).Clear();
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

            if (array is int[] intArray)
            {
                int ints = GetIntegerArrayLengthFromBitLength(m_length);
                Div32Rem(m_length, out int extraBits);

                if (extraBits == 0)
                {
                    // we have perfect bit alignment, no need to sanitize, just copy
                    m_array.AsSpan(0, ints).CopyTo(intArray.AsSpan(index));
                }
                else
                {
                    int last = ints - 1;
                    // do not copy the last int, as it is not completely used
                    m_array.AsSpan(0, last).CopyTo(intArray.AsSpan(index));

                    // the last int needs to be masked
                    intArray[index + last] = m_array[last] & unchecked((1 << extraBits) - 1);
                }
            }
            else if (array is byte[] byteArray)
            {
                int arrayLength = GetByteArrayLengthFromBitLength(m_length);
                if ((array.Length - index) < arrayLength)
                {
                    throw new ArgumentException(SR.Argument_InvalidOffLen);
                }

                Div8Rem(m_length, out int extraBits);
                if (extraBits > 0)
                {
                    // last byte is not aligned, we will directly copy one less byte
                    arrayLength -= 1;
                }

                int quotient = Div4Rem(arrayLength, out int remainder);
                int indexCopy = index;
                for (int i = 0; i < quotient; i++)
                {
                    // Shift to bring the required byte to LSB, then mask
                    int value = m_array[i];
                    byteArray[indexCopy++] = (byte)(value & 0x000000FF);
                    byteArray[indexCopy++] = (byte)((value >> 8) & 0x000000FF);
                    byteArray[indexCopy++] = (byte)((value >> 16) & 0x000000FF);
                    byteArray[indexCopy++] = (byte)((value >> 24) & 0x000000FF);
                }

                switch (remainder)
                {
                    case 3:
                        byteArray[indexCopy++] = (byte)((m_array[quotient] >> 16) & 0x000000FF);
                        goto case 2;
                    // fall through
                    case 2:
                        byteArray[indexCopy++] = (byte)((m_array[quotient] >> 8) & 0x000000FF);
                        goto case 1;
                    // fall through
                    case 1:
                        byteArray[indexCopy++] = (byte)(m_array[quotient] & 0x000000FF);
                        break;
                }

                if (extraBits > 0)
                {
                    // mask the final byte
                    byteArray[index + arrayLength] = (byte)((m_array[quotient] >> (remainder * 8)) & ((1 << extraBits) - 1));
                }
            }
            else if (array is bool[] boolArray)
            {
                if (array.Length - index < m_length)
                {
                    throw new ArgumentException(SR.Argument_InvalidOffLen);
                }

                for (int i = 0; i < m_length; i++)
                {
                    int quotient = Div32Rem(i, out int remainder);
                    boolArray[index + i] = ((m_array[quotient] >> (remainder)) & 0x00000001) != 0;
                }
            }
            else
            {
                throw new ArgumentException(SR.Arg_BitArrayTypeUnsupported, nameof(array));
            }
        }

        public int Count => m_length;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        public bool IsSynchronized => false;

        public bool IsReadOnly => false;

        public object Clone() => new BitArray(this);

        public IEnumerator GetEnumerator() => new BitArrayEnumeratorSimple(this);

        // XPerY=n means that n Xs can be stored in 1 Y. 
        private const int BitsPerInt32 = 32;
        private const int BytesPerInt32 = 4;
        private const int BitsPerByte = 8;

        private const int BitShiftPerInt32 = 5;
        private const int BitShiftPerByte = 3;
        private const int BitShiftForBytesPerInt32 = 2;

        /// <summary>
        /// Used for conversion between different representations of bit array. 
        /// Returns (n + (32 - 1)) / 32, rearranged to avoid arithmetic overflow. 
        /// For example, in the bit to int case, the straightforward calc would 
        /// be (n + 31) / 32, but that would cause overflow. So instead it's 
        /// rearranged to ((n - 1) / 32) + 1.
        /// Due to sign extension, we don't need to special case for n == 0, if we use
        /// bitwise operations (since ((n - 1) >> 5) + 1 = 0).
        /// This doesn't hold true for ((n - 1) / 32) + 1, which equals 1.
        /// 
        /// Usage:
        /// GetArrayLength(77): returns how many ints must be 
        /// allocated to store 77 bits.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>how many ints are required to store n bytes</returns>
        private static int GetIntegerArrayLengthFromBitLength(int n)
        {
            Debug.Assert(n >= 0);
            return ((n - 1) >> BitShiftPerInt32) + 1;
        }

        private static int GetIntegerArrayLengthFromByteLength(int n)
        {
            Debug.Assert(n >= 0);
            // Due to sign extension, we don't need to special case for n == 0, since ((n - 1) >> 2) + 1 = 0
            // This doesn't hold true for ((n - 1) / 4) + 1, which equals 1.
            return ((n - 1) >> BitShiftForBytesPerInt32) + 1;
        }

        private static int GetByteArrayLengthFromBitLength(int n)
        {
            Debug.Assert(n >= 0);
            // Due to sign extension, we don't need to special case for n == 0, since ((n - 1) >> 3) + 1 = 0
            // This doesn't hold true for ((n - 1) / 8) + 1, which equals 1.
            return ((n - 1) >> BitShiftPerByte) + 1;
        }

        private static int Div32Rem(int number, out int remainder)
        {
            int quotient = number >> BitShiftPerInt32;   // Divide by 32.
            remainder = number - (quotient << BitShiftPerInt32);   // Multiply by 32.
            return quotient;
        }

        private static int Div8Rem(int number, out int remainder)
        {
            int quotient = number >> BitShiftPerByte;   // Divide by 8.
            remainder = number - (quotient << BitShiftPerByte);   // Multiply by 8.
            return quotient;
        }

        private static int Div4Rem(int number, out int remainder)
        {
            int quotient = number >> BitShiftForBytesPerInt32;   // Divide by 4.
            remainder = number - (quotient << BitShiftForBytesPerInt32);   // Multiply by 4.
            return quotient;
        }

        private class BitArrayEnumeratorSimple : IEnumerator, ICloneable
        {
            private BitArray _bitarray;
            private int _index;
            private readonly int _version;
            private bool _currentElement;

            internal BitArrayEnumeratorSimple(BitArray bitarray)
            {
                _bitarray = bitarray;
                _index = -1;
                _version = bitarray._version;
            }

            public object Clone() => MemberwiseClone();

            public virtual bool MoveNext()
            {
                ICollection bitarrayAsICollection = _bitarray;
                if (_version != _bitarray._version)
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);

                if (_index < (bitarrayAsICollection.Count - 1))
                {
                    _index++;
                    _currentElement = _bitarray.Get(_index);
                    return true;
                }
                else
                {
                    _index = bitarrayAsICollection.Count;
                }

                return false;
            }

            public virtual object Current
            {
                get
                {
                    if ((uint)_index >= (uint)_bitarray.Count)
                        throw GetInvalidOperationException(_index);
                    return _currentElement;
                }
            }

            public void Reset()
            {
                if (_version != _bitarray._version)
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                _index = -1;
            }

            private InvalidOperationException GetInvalidOperationException(int index)
            {
                if (index == -1)
                {
                    return new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                }
                else
                {
                    Debug.Assert(index >= _bitarray.Count);
                    return new InvalidOperationException(SR.InvalidOperation_EnumEnded);
                }
            }
        }
    }
}
