// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.EncoderParameter.cs
//
// Author: 
//	Ravindra (rkumar@novell.com)
//  Vladimir Vukicevic (vladimir@pobox.com)
//
// (C) 2004 Novell, Inc.  http://www.novell.com
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Text;

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{

    [StructLayout(LayoutKind.Sequential)]
    public sealed class EncoderParameter : IDisposable
    {

        private Encoder encoder;
        private int valuesCount;
        private EncoderParameterValueType type;
        private IntPtr valuePtr;

        internal EncoderParameter()
        {
        }

        public EncoderParameter(Encoder encoder, byte value)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            this.type = EncoderParameterValueType.ValueTypeByte;
            this.valuePtr = Marshal.AllocHGlobal(1);
            Marshal.WriteByte(this.valuePtr, value);
        }

        public EncoderParameter(Encoder encoder, byte[] value)
        {
            this.encoder = encoder;
            this.valuesCount = value.Length;
            this.type = EncoderParameterValueType.ValueTypeByte;
            this.valuePtr = Marshal.AllocHGlobal(1 * valuesCount);
            Marshal.Copy(value, 0, this.valuePtr, valuesCount);
        }

        public EncoderParameter(Encoder encoder, short value)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            this.type = EncoderParameterValueType.ValueTypeShort;
            this.valuePtr = Marshal.AllocHGlobal(2);
            Marshal.WriteInt16(this.valuePtr, value);
        }

        public EncoderParameter(Encoder encoder, short[] value)
        {
            this.encoder = encoder;
            this.valuesCount = value.Length;
            this.type = EncoderParameterValueType.ValueTypeShort;
            this.valuePtr = Marshal.AllocHGlobal(2 * valuesCount);
            Marshal.Copy(value, 0, this.valuePtr, valuesCount);
        }


        public EncoderParameter(Encoder encoder, long value)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            this.type = EncoderParameterValueType.ValueTypeLong;
            this.valuePtr = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(this.valuePtr, (int)value);
        }

        public EncoderParameter(Encoder encoder, long[] value)
        {
            this.encoder = encoder;
            this.valuesCount = value.Length;
            this.type = EncoderParameterValueType.ValueTypeLong;
            this.valuePtr = Marshal.AllocHGlobal(4 * valuesCount);
            int[] ivals = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
                ivals[i] = (int)value[i];
            Marshal.Copy(ivals, 0, this.valuePtr, valuesCount);
        }

        public EncoderParameter(Encoder encoder, string value)
        {
            this.encoder = encoder;

            ASCIIEncoding ascii = new ASCIIEncoding();
            int asciiByteCount = ascii.GetByteCount(value);
            byte[] bytes = new byte[asciiByteCount];
            ascii.GetBytes(value, 0, value.Length, bytes, 0);

            this.valuesCount = bytes.Length;
            this.type = EncoderParameterValueType.ValueTypeAscii;
            this.valuePtr = Marshal.AllocHGlobal(valuesCount);
            Marshal.Copy(bytes, 0, this.valuePtr, valuesCount);
        }

        public EncoderParameter(Encoder encoder, byte value, bool undefined)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            if (undefined)
                this.type = EncoderParameterValueType.ValueTypeUndefined;
            else
                this.type = EncoderParameterValueType.ValueTypeByte;
            this.valuePtr = Marshal.AllocHGlobal(1);
            Marshal.WriteByte(this.valuePtr, value);
        }

        public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
        {
            this.encoder = encoder;
            this.valuesCount = value.Length;
            if (undefined)
                this.type = EncoderParameterValueType.ValueTypeUndefined;
            else
                this.type = EncoderParameterValueType.ValueTypeByte;
            this.valuePtr = Marshal.AllocHGlobal(valuesCount);
            Marshal.Copy(value, 0, this.valuePtr, valuesCount);
        }

        public EncoderParameter(Encoder encoder, int numerator, int denominator)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            this.type = EncoderParameterValueType.ValueTypeRational;
            this.valuePtr = Marshal.AllocHGlobal(8);
            int[] valuearray = { numerator, denominator };
            Marshal.Copy(valuearray, 0, this.valuePtr, valuearray.Length);
        }

        public EncoderParameter(Encoder encoder, int[] numerator, int[] denominator)
        {
            if (numerator.Length != denominator.Length)
                throw new ArgumentException("Invalid parameter used.");

            this.encoder = encoder;
            this.valuesCount = numerator.Length;
            this.type = EncoderParameterValueType.ValueTypeRational;
            this.valuePtr = Marshal.AllocHGlobal(4 * valuesCount * 2);
            for (int i = 0; i < valuesCount; i++)
            {
                Marshal.WriteInt32(valuePtr, i * 4, (int)numerator[i]);
                Marshal.WriteInt32(valuePtr, (i + 1) * 4, (int)denominator[i]);
            }
        }

        public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            this.type = EncoderParameterValueType.ValueTypeLongRange;
            this.valuePtr = Marshal.AllocHGlobal(8);
            int[] valuearray = { (int)rangebegin, (int)rangeend };
            Marshal.Copy(valuearray, 0, this.valuePtr, valuearray.Length);
        }

        public EncoderParameter(Encoder encoder, long[] rangebegin, long[] rangeend)
        {
            if (rangebegin.Length != rangeend.Length)
                throw new ArgumentException("Invalid parameter used.");

            this.encoder = encoder;
            this.valuesCount = rangebegin.Length;
            this.type = EncoderParameterValueType.ValueTypeLongRange;

            this.valuePtr = Marshal.AllocHGlobal(4 * valuesCount * 2);
            IntPtr dest = this.valuePtr;
            for (int i = 0; i < valuesCount; i++)
            {
                Marshal.WriteInt32(dest, i * 4, (int)rangebegin[i]);
                Marshal.WriteInt32(dest, (i + 1) * 4, (int)rangeend[i]);
            }
        }

        public EncoderParameter(Encoder encoder, int NumberOfValues, int Type, int Value)
        {
            this.encoder = encoder;
            this.valuePtr = (IntPtr)Value;
            this.valuesCount = NumberOfValues;
            this.type = (EncoderParameterValueType)Type;
        }

        public EncoderParameter(Encoder encoder, int numerator1, int demoninator1, int numerator2, int demoninator2)
        {
            this.encoder = encoder;
            this.valuesCount = 1;
            this.type = EncoderParameterValueType.ValueTypeRationalRange;
            this.valuePtr = Marshal.AllocHGlobal(4 * 4);
            int[] valuearray = { numerator1, demoninator1, numerator2, demoninator2 };
            Marshal.Copy(valuearray, 0, this.valuePtr, 4);
        }

        public EncoderParameter(Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
        {
            if (numerator1.Length != denominator1.Length ||
                numerator2.Length != denominator2.Length ||
                numerator1.Length != numerator2.Length)
                throw new ArgumentException("Invalid parameter used.");

            this.encoder = encoder;
            this.valuesCount = numerator1.Length;
            this.type = EncoderParameterValueType.ValueTypeRationalRange;

            this.valuePtr = Marshal.AllocHGlobal(4 * valuesCount * 4);
            IntPtr dest = this.valuePtr;
            for (int i = 0; i < valuesCount; i++)
            {
                Marshal.WriteInt32(dest, i * 4, numerator1[i]);
                Marshal.WriteInt32(dest, (i + 1) * 4, denominator1[i]);
                Marshal.WriteInt32(dest, (i + 2) * 4, numerator2[i]);
                Marshal.WriteInt32(dest, (i + 3) * 4, denominator2[i]);
            }
        }

#if NETCORE
        public EncoderParameter(Encoder encoder, int numberValues, EncoderParameterValueType type, IntPtr value)
        {
            int size;

            switch (type)
            {
                case EncoderParameterValueType.ValueTypeByte:
                case EncoderParameterValueType.ValueTypeAscii:
                    size = 1;
                    break;
                case EncoderParameterValueType.ValueTypeShort:
                    size = 2;
                    break;
                case EncoderParameterValueType.ValueTypeLong:
                    size = 4;
                    break;
                case EncoderParameterValueType.ValueTypeRational:
                case EncoderParameterValueType.ValueTypeLongRange:
                    size = 2 * 4;
                    break;
                case EncoderParameterValueType.ValueTypeUndefined:
                    size = 1;
                    break;
                case EncoderParameterValueType.ValueTypeRationalRange:
                    size = 2 * 2 * 4;
                    break;
                default:
                    throw new ArgumentException("Invalid parameter used.");
            }

            int bytes = checked(size * numberValues);

            valuePtr = Marshal.AllocHGlobal(bytes);

            if (valuePtr == IntPtr.Zero)
                throw new ArgumentException("Out of memory.");

            for (int i = 0; i < bytes; i++)
            {
                Marshal.WriteByte(IntPtr.Add(valuePtr, i), Marshal.ReadByte((IntPtr)(value + i)));
            }

            this.encoder = encoder;
            this.type = type;
            this.valuesCount = numberValues;
            GC.KeepAlive(this);
        }
#endif

        public Encoder Encoder
        {
            get
            {
                return encoder;
            }

            set
            {
                encoder = value;
            }
        }

        public int NumberOfValues
        {
            get
            {
                return valuesCount;
            }
        }

        public EncoderParameterValueType Type
        {
            get
            {
                return type;
            }
        }

        public EncoderParameterValueType ValueType
        {
            get
            {
                return type;
            }
        }

        void Dispose(bool disposing)
        {
            if (valuePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(valuePtr);
                valuePtr = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EncoderParameter()
        {
            Dispose(false);
        }

        internal static int NativeSize()
        {
            return Marshal.SizeOf(typeof(GdipEncoderParameter));
        }

        internal void ToNativePtr(IntPtr epPtr)
        {
            GdipEncoderParameter ep = new GdipEncoderParameter();
            ep.guid = this.encoder.Guid;
            ep.numberOfValues = (uint)this.valuesCount;
            ep.type = this.type;
            ep.value = this.valuePtr;
            Marshal.StructureToPtr(ep, epPtr, false);
        }

        internal static EncoderParameter FromNativePtr(IntPtr epPtr)
        {
            GdipEncoderParameter ep;
            ep = (GdipEncoderParameter)Marshal.PtrToStructure(epPtr, typeof(GdipEncoderParameter));

            Type valType;
            uint valCount;

            switch (ep.type)
            {
                case EncoderParameterValueType.ValueTypeAscii:
                case EncoderParameterValueType.ValueTypeByte:
                case EncoderParameterValueType.ValueTypeUndefined:
                    valType = typeof(byte);
                    valCount = ep.numberOfValues;
                    break;
                case EncoderParameterValueType.ValueTypeShort:
                    valType = typeof(short);
                    valCount = ep.numberOfValues;
                    break;
                case EncoderParameterValueType.ValueTypeLong:
                    valType = typeof(int);
                    valCount = ep.numberOfValues;
                    break;
                case EncoderParameterValueType.ValueTypeLongRange:
                case EncoderParameterValueType.ValueTypeRational:
                    valType = typeof(int);
                    valCount = ep.numberOfValues * 2;
                    break;
                case EncoderParameterValueType.ValueTypeRationalRange:
                    valType = typeof(int);
                    valCount = ep.numberOfValues * 4;
                    break;
                default:
                    return null;
            }

            EncoderParameter eparam = new EncoderParameter();
            eparam.encoder = new Encoder(ep.guid);
            eparam.valuesCount = (int)ep.numberOfValues;
            eparam.type = ep.type;
            eparam.valuePtr = Marshal.AllocHGlobal((int)(valCount * Marshal.SizeOf(valType)));

            /* There's nothing in Marshal to do a memcpy() between two IntPtrs.  This sucks. */
            unsafe
            {
                byte* s = (byte*)ep.value;
                byte* d = (byte*)eparam.valuePtr;
                for (int i = 0; i < valCount * Marshal.SizeOf(valType); i++)
                    *d++ = *s++;
            }

            return eparam;
        }
    }
}
