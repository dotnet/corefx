// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

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
        private readonly object _value;

        internal CONSTVAL()
            : this(null)
        {
        }

        internal CONSTVAL(object value)
        {
            _value = value;
        }

        public object objectVal => _value;

        public bool boolVal => SpecialUnbox<bool>(_value);

        public sbyte sbyteVal => SpecialUnbox<sbyte>(_value);

        public byte byteVal => SpecialUnbox<byte>(_value);

        public short shortVal => SpecialUnbox<short>(_value);

        public ushort ushortVal => SpecialUnbox<ushort>(_value);

        public int iVal => SpecialUnbox<int>(_value);

        public uint uiVal => SpecialUnbox<uint>(_value);

        public long longVal => SpecialUnbox<long>(_value);

        public ulong ulongVal => SpecialUnbox<ulong>(_value);

        public float floatVal => SpecialUnbox<float>(_value);

        public double doubleVal => SpecialUnbox<double>(_value);

        public decimal decVal => SpecialUnbox<decimal>(_value);

        public char cVal => SpecialUnbox<char>(_value);

        public string strVal => SpecialUnbox<string>(_value);

        public bool IsNullRef => _value == null;

        public bool IsZero(ConstValKind kind)
        {
            switch (kind)
            {
                case ConstValKind.Decimal:
                    return decVal == 0;
                case ConstValKind.String:
                    return false;
                default:
                    return IsDefault(_value);
            }
        }

        private T SpecialUnbox<T>(object o)
        {
            if (IsDefault(o))
            {
                return default(T);
            }

            return (T)Convert.ChangeType(o, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
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
    }
}
