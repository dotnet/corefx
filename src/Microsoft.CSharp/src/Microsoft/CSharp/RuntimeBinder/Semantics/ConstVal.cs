// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    /*
     * The kind of allocation used in a constant value.
     * Since CONSTVALs don't store a discriminant for the union
     * this must be managed by clients.
     */
    internal enum ConstValKind
    {
        Int,
        Double,
        Long,
        String,
        Decimal,
        IntPtr,
        Float,
        Boolean,
        Lim
    };


    internal sealed class CONSTVAL
    {
        private CONSTVAL(object value)
        {
            objectVal = value;
        }

        public object objectVal { get; }

        public bool boolVal => SpecialUnbox<bool>(objectVal);

        public sbyte sbyteVal => SpecialUnbox<sbyte>(objectVal);

        public byte byteVal => SpecialUnbox<byte>(objectVal);

        public short shortVal => SpecialUnbox<short>(objectVal);

        public ushort ushortVal => SpecialUnbox<ushort>(objectVal);

        public int iVal => SpecialUnbox<int>(objectVal);

        public uint uiVal => SpecialUnbox<uint>(objectVal);

        public long longVal => SpecialUnbox<long>(objectVal);

        public ulong ulongVal => SpecialUnbox<ulong>(objectVal);

        public float floatVal => SpecialUnbox<float>(objectVal);

        public double doubleVal => SpecialUnbox<double>(objectVal);

        public decimal decVal => SpecialUnbox<decimal>(objectVal);

        public char cVal => SpecialUnbox<char>(objectVal);

        public string strVal => SpecialUnbox<string>(objectVal);

        public bool IsNullRef => objectVal == null;

        public bool IsZero(ConstValKind kind)
        {
            switch (kind)
            {
                case ConstValKind.Decimal:
                    return decVal == 0;
                case ConstValKind.String:
                    return false;
                default:
                    return IsDefault(objectVal);
            }
        }

        private T SpecialUnbox<T>(object o)
        {
            if (IsDefault(o))
            {
                return default(T);
            }

            return (T)Convert.ChangeType(o, typeof(T), CultureInfo.InvariantCulture);
        }

        private bool IsDefault(object o)
        {
            if (o == null)
                return true;

            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Boolean:
                    return default(bool).Equals(o);
                case TypeCode.SByte:
                    return default(sbyte).Equals(o);
                case TypeCode.Byte:
                    return default(byte).Equals(o);
                case TypeCode.Int16:
                    return default(short).Equals(o);
                case TypeCode.UInt16:
                    return default(ushort).Equals(o);
                case TypeCode.Int32:
                    return default(int).Equals(o);
                case TypeCode.UInt32:
                    return default(uint).Equals(o);
                case TypeCode.Int64:
                    return default(long).Equals(o);
                case TypeCode.UInt64:
                    return default(ulong).Equals(o);
                case TypeCode.Single:
                    return default(float).Equals(o);
                case TypeCode.Double:
                    return default(double).Equals(o);
                case TypeCode.Decimal:
                    return default(decimal).Equals(o);
                case TypeCode.Char:
                    return default(char).Equals(o);
            }

            return false;
        }

        public static CONSTVAL GetDefaultValue(ConstValKind kind)
        {
            switch (kind)
            {
                case ConstValKind.Int:
                    return new CONSTVAL(0);

                case ConstValKind.Double:
                    return new CONSTVAL(0.0);

                case ConstValKind.Long:
                    return new CONSTVAL(0L);

                case ConstValKind.Decimal:
                    return new CONSTVAL(0M);

                case ConstValKind.Float:
                    return new CONSTVAL(0F);

                case ConstValKind.Boolean:
                    return new CONSTVAL(false);
            }

            return new CONSTVAL(null);
        }

        public static CONSTVAL GetNullRef()
        {
            return new CONSTVAL(null);
        }

        public static CONSTVAL Get(bool value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(int value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(uint value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(decimal value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(string value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(float value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(double value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(long value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(ulong value)
        {
            return new CONSTVAL(value);
        }

        public static CONSTVAL Get(object p)
        {
            return new CONSTVAL(p);
        }
    }
}
