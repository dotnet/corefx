// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

#if SRM
namespace System.Reflection
#else
namespace Roslyn.Reflection
#endif
{
    internal static class BlobWriterImpl
    {
        internal const int SingleByteCompressedIntegerMaxValue = 0x7f;
        internal const int TwoByteCompressedIntegerMaxValue = 0x3fff;
        internal const int MaxCompressedIntegerValue = 0x1fffffff;

        internal static int GetCompressedIntegerSize(int value)
        {
            Debug.Assert(value <= MaxCompressedIntegerValue);

            if (value <= SingleByteCompressedIntegerMaxValue)
            {
                return 1;
            }

            if (value <= TwoByteCompressedIntegerMaxValue)
            {
                return 2;
            }

            return 4;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowValueArgumentOutOfRange()
        {
            throw new ArgumentOutOfRangeException("value");
        }

        internal static void WriteCompressedInteger(ref BlobWriter writer, int value)
        {
            unchecked
            {
                if (value <= SingleByteCompressedIntegerMaxValue)
                {
                    writer.WriteByte((byte)value);
                }
                else if (value <= TwoByteCompressedIntegerMaxValue)
                {
                    writer.WriteUInt16BE((ushort)(0x8000 | value));
                }
                else if (value <= MaxCompressedIntegerValue)
                {
                    writer.WriteUInt32BE(0xc0000000 | (uint)value);
                }
                else
                {
                    ThrowValueArgumentOutOfRange();
                }
            }
        }

        internal static void WriteCompressedInteger(BlobBuilder writer, int value)
        {
            unchecked
            {
                if (value <= SingleByteCompressedIntegerMaxValue)
                {
                    writer.WriteByte((byte)value);
                }
                else if (value <= TwoByteCompressedIntegerMaxValue)
                {
                    writer.WriteUInt16BE((ushort)(0x8000 | value));
                }
                else if (value <= MaxCompressedIntegerValue)
                {
                    writer.WriteUInt32BE(0xc0000000 | (uint)value);
                }
                else
                {
                    ThrowValueArgumentOutOfRange();
                }
            }
        }

        internal static void WriteCompressedSignedInteger(ref BlobWriter writer, int value)
        {
            unchecked
            {
                const int b6 = (1 << 6) - 1;
                const int b13 = (1 << 13) - 1;
                const int b28 = (1 << 28) - 1;

                // 0xffffffff for negative value
                // 0x00000000 for non-negative
                int signMask = value >> 31;

                if ((value & ~b6) == (signMask & ~b6))
                {
                    int n = ((value & b6) << 1) | (signMask & 1);
                    writer.WriteByte((byte)n);
                }
                else if ((value & ~b13) == (signMask & ~b13))
                {
                    int n = ((value & b13) << 1) | (signMask & 1);
                    writer.WriteUInt16BE((ushort)(0x8000 | n));
                }
                else if ((value & ~b28) == (signMask & ~b28))
                {
                    int n = ((value & b28) << 1) | (signMask & 1);
                    writer.WriteUInt32BE(0xc0000000 | (uint)n);
                }
                else
                {
                    ThrowValueArgumentOutOfRange();
                }
            }
        }

        internal static void WriteCompressedSignedInteger(BlobBuilder writer, int value)
        {
            unchecked
            {
                const int b6 = (1 << 6) - 1;
                const int b13 = (1 << 13) - 1;
                const int b28 = (1 << 28) - 1;

                // 0xffffffff for negative value
                // 0x00000000 for non-negative
                int signMask = value >> 31;

                if ((value & ~b6) == (signMask & ~b6))
                {
                    int n = ((value & b6) << 1) | (signMask & 1);
                    writer.WriteByte((byte)n);
                }
                else if ((value & ~b13) == (signMask & ~b13))
                {
                    int n = ((value & b13) << 1) | (signMask & 1);
                    writer.WriteUInt16BE((ushort)(0x8000 | n));
                }
                else if ((value & ~b28) == (signMask & ~b28))
                {
                    int n = ((value & b28) << 1) | (signMask & 1);
                    writer.WriteUInt32BE(0xc0000000 | (uint)n);
                }
                else
                {
                    ThrowValueArgumentOutOfRange();
                }
            }
        }

        internal static void WriteConstant(ref BlobWriter writer, object value)
        {
            if (value == null)
            {
                // The encoding of Type for the nullref value for FieldInit is ELEMENT_TYPE_CLASS with a Value of a 32-bit.
                writer.WriteUInt32(0);
                return;
            }

            var type = value.GetType();
            if (type.GetTypeInfo().IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            if (type == typeof(bool))
            {
                writer.WriteBoolean((bool)value);
            }
            else if (type == typeof(int))
            {
                writer.WriteInt32((int)value);
            }
            else if (type == typeof(string))
            {
                writer.WriteUTF16((string)value);
            }
            else if (type == typeof(byte))
            {
                writer.WriteByte((byte)value);
            }
            else if (type == typeof(char))
            {
                writer.WriteUInt16((char)value);
            }
            else if (type == typeof(double))
            {
                writer.WriteDouble((double)value);
            }
            else if (type == typeof(short))
            {
                writer.WriteInt16((short)value);
            }
            else if (type == typeof(long))
            {
                writer.WriteInt64((long)value);
            }
            else if (type == typeof(sbyte))
            {
                writer.WriteSByte((sbyte)value);
            }
            else if (type == typeof(float))
            {
                writer.WriteSingle((float)value);
            }
            else if (type == typeof(ushort))
            {
                writer.WriteUInt16((ushort)value);
            }
            else if (type == typeof(uint))
            {
                writer.WriteUInt32((uint)value);
            }
            else if (type == typeof(ulong))
            {
                writer.WriteUInt64((ulong)value);
            }
            else
            {
                // TODO: message
                throw new ArgumentException();
            }
        }

        internal static void WriteConstant(BlobBuilder writer, object value)
        {
            if (value == null)
            {
                // The encoding of Type for the nullref value for FieldInit is ELEMENT_TYPE_CLASS with a Value of a 32-bit.
                writer.WriteUInt32(0);
                return;
            }

            var type = value.GetType();
            if (type.GetTypeInfo().IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            if (type == typeof(bool))
            {
                writer.WriteBoolean((bool)value);
            }
            else if (type == typeof(int))
            {
                writer.WriteInt32((int)value);
            }
            else if (type == typeof(string))
            {
                writer.WriteUTF16((string)value);
            }
            else if (type == typeof(byte))
            {
                writer.WriteByte((byte)value);
            }
            else if (type == typeof(char))
            {
                writer.WriteUInt16((char)value);
            }
            else if (type == typeof(double))
            {
                writer.WriteDouble((double)value);
            }
            else if (type == typeof(short))
            {
                writer.WriteInt16((short)value);
            }
            else if (type == typeof(long))
            {
                writer.WriteInt64((long)value);
            }
            else if (type == typeof(sbyte))
            {
                writer.WriteSByte((sbyte)value);
            }
            else if (type == typeof(float))
            {
                writer.WriteSingle((float)value);
            }
            else if (type == typeof(ushort))
            {
                writer.WriteUInt16((ushort)value);
            }
            else if (type == typeof(uint))
            {
                writer.WriteUInt32((uint)value);
            }
            else if (type == typeof(ulong))
            {
                writer.WriteUInt64((ulong)value);
            }
            else
            {
                // TODO: message
                throw new ArgumentException();
            }
        }
    }
}
