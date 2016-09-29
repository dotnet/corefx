// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//------------------------------------------------------------------------------


using System.Data.Common;
using System.Runtime.InteropServices;


namespace System.Data.SqlTypes
{
    /// <devdoc>
    ///    <para>
    ///       Represents a 32-bit signed integer to be stored in
    ///       or retrieved from a database.
    ///    </para>
    /// </devdoc>
    [StructLayout(LayoutKind.Sequential)]
    public struct SqlInt32 : INullable, IComparable
    {
        private bool _fNotNull; // false if null, the default ctor (plain 0) will make it Null
        private int _value;

        private static readonly long s_iIntMin = Int32.MinValue;   // minimum (signed) int value
        private static readonly long s_lBitNotIntMax = ~(long)(Int32.MaxValue);

        // constructor
        // construct a Null
        private SqlInt32(bool fNull)
        {
            _fNotNull = false;
            _value = 0;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SqlInt32(int value)
        {
            _value = value;
            _fNotNull = true;
        }


        // INullable
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsNull
        {
            get { return !_fNotNull; }
        }

        // property: Value
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Value
        {
            get
            {
                if (IsNull)
                    throw new SqlNullValueException();
                else
                    return _value;
            }
        }

        // Implicit conversion from int to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static implicit operator SqlInt32(int x)
        {
            return new SqlInt32(x);
        }

        // Explicit conversion from SqlInt32 to int. Throw exception if x is Null.
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator int (SqlInt32 x)
        {
            return x.Value;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override String ToString()
        {
            return IsNull ? SQLResource.NullString : _value.ToString((IFormatProvider)null);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 Parse(String s)
        {
            if (s == SQLResource.NullString)
                return SqlInt32.Null;
            else
                return new SqlInt32(Int32.Parse(s, (IFormatProvider)null));
        }


        // Unary operators
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator -(SqlInt32 x)
        {
            return x.IsNull ? Null : new SqlInt32(-x._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator ~(SqlInt32 x)
        {
            return x.IsNull ? Null : new SqlInt32(~x._value);
        }


        // Binary operators

        // Arithmetic operators
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator +(SqlInt32 x, SqlInt32 y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            int iResult = x._value + y._value;
            if (SameSignInt(x._value, y._value) && !SameSignInt(x._value, iResult))
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlInt32(iResult);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator -(SqlInt32 x, SqlInt32 y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            int iResult = x._value - y._value;
            if (!SameSignInt(x._value, y._value) && SameSignInt(y._value, iResult))
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlInt32(iResult);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator *(SqlInt32 x, SqlInt32 y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            long lResult = (long)x._value * (long)y._value;
            long lTemp = lResult & s_lBitNotIntMax;
            if (lTemp != 0 && lTemp != s_lBitNotIntMax)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlInt32((int)lResult);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator /(SqlInt32 x, SqlInt32 y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            if (y._value != 0)
            {
                if ((x._value == s_iIntMin) && (y._value == -1))
                    throw new OverflowException(SQLResource.ArithOverflowMessage);

                return new SqlInt32(x._value / y._value);
            }
            else
                throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator %(SqlInt32 x, SqlInt32 y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            if (y._value != 0)
            {
                if ((x._value == s_iIntMin) && (y._value == -1))
                    throw new OverflowException(SQLResource.ArithOverflowMessage);

                return new SqlInt32(x._value % y._value);
            }
            else
                throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
        }

        // Bitwise operators
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator &(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? Null : new SqlInt32(x._value & y._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator |(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? Null : new SqlInt32(x._value | y._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlInt32 operator ^(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? Null : new SqlInt32(x._value ^ y._value);
        }


        // Implicit conversions

        // Implicit conversion from SqlBoolean to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlBoolean x)
        {
            return x.IsNull ? Null : new SqlInt32((int)x.ByteValue);
        }

        // Implicit conversion from SqlByte to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static implicit operator SqlInt32(SqlByte x)
        {
            return x.IsNull ? Null : new SqlInt32(x.Value);
        }

        // Implicit conversion from SqlInt16 to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static implicit operator SqlInt32(SqlInt16 x)
        {
            return x.IsNull ? Null : new SqlInt32(x.Value);
        }


        // Explicit conversions

        // Explicit conversion from SqlInt64 to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlInt64 x)
        {
            if (x.IsNull)
                return Null;

            long value = x.Value;
            if (value > (long)Int32.MaxValue || value < (long)Int32.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlInt32((int)value);
        }

        // Explicit conversion from SqlSingle to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlSingle x)
        {
            if (x.IsNull)
                return Null;

            float value = x.Value;
            if (value > (float)Int32.MaxValue || value < (float)Int32.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlInt32((int)value);
        }

        // Explicit conversion from SqlDouble to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlDouble x)
        {
            if (x.IsNull)
                return Null;

            double value = x.Value;
            if (value > (double)Int32.MaxValue || value < (double)Int32.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlInt32((int)value);
        }

        // Explicit conversion from SqlMoney to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlMoney x)
        {
            return x.IsNull ? Null : new SqlInt32(x.ToInt32());
        }

        // Explicit conversion from SqlDecimal to SqlInt32
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlDecimal x)
        {
            if (x.IsNull)
                return SqlInt32.Null;

            x.AdjustScale(-x.Scale, true);

            long ret = (long)x.m_data1;
            if (!x.IsPositive)
                ret = -ret;

            if (x.m_bLen > 1 || ret > (long)Int32.MaxValue || ret < (long)Int32.MinValue)
                throw new OverflowException(SQLResource.ConversionOverflowMessage);

            return new SqlInt32((int)ret);
        }

        // Explicit conversion from SqlString to SqlInt
        // Throws FormatException or OverflowException if necessary.
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static explicit operator SqlInt32(SqlString x)
        {
            return x.IsNull ? SqlInt32.Null : new SqlInt32(Int32.Parse(x.Value, (IFormatProvider)null));
        }

        // Utility functions
        private static bool SameSignInt(int x, int y)
        {
            return ((x ^ y) & 0x80000000) == 0;
        }

        // Overloading comparison operators
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlBoolean operator ==(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value == y._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlBoolean operator !=(SqlInt32 x, SqlInt32 y)
        {
            return !(x == y);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlBoolean operator <(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value < y._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlBoolean operator >(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value > y._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlBoolean operator <=(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value <= y._value);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static SqlBoolean operator >=(SqlInt32 x, SqlInt32 y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value >= y._value);
        }

        //--------------------------------------------------
        // Alternative methods for overloaded operators
        //--------------------------------------------------

        // Alternative method for operator ~
        public static SqlInt32 OnesComplement(SqlInt32 x)
        {
            return ~x;
        }

        // Alternative method for operator +
        public static SqlInt32 Add(SqlInt32 x, SqlInt32 y)
        {
            return x + y;
        }
        // Alternative method for operator -
        public static SqlInt32 Subtract(SqlInt32 x, SqlInt32 y)
        {
            return x - y;
        }

        // Alternative method for operator *
        public static SqlInt32 Multiply(SqlInt32 x, SqlInt32 y)
        {
            return x * y;
        }

        // Alternative method for operator /
        public static SqlInt32 Divide(SqlInt32 x, SqlInt32 y)
        {
            return x / y;
        }

        // Alternative method for operator %
        public static SqlInt32 Mod(SqlInt32 x, SqlInt32 y)
        {
            return x % y;
        }

        public static SqlInt32 Modulus(SqlInt32 x, SqlInt32 y)
        {
            return x % y;
        }

        // Alternative method for operator &
        public static SqlInt32 BitwiseAnd(SqlInt32 x, SqlInt32 y)
        {
            return x & y;
        }

        // Alternative method for operator |
        public static SqlInt32 BitwiseOr(SqlInt32 x, SqlInt32 y)
        {
            return x | y;
        }

        // Alternative method for operator ^
        public static SqlInt32 Xor(SqlInt32 x, SqlInt32 y)
        {
            return x ^ y;
        }

        // Alternative method for operator ==
        public static SqlBoolean Equals(SqlInt32 x, SqlInt32 y)
        {
            return (x == y);
        }

        // Alternative method for operator !=
        public static SqlBoolean NotEquals(SqlInt32 x, SqlInt32 y)
        {
            return (x != y);
        }

        // Alternative method for operator <
        public static SqlBoolean LessThan(SqlInt32 x, SqlInt32 y)
        {
            return (x < y);
        }

        // Alternative method for operator >
        public static SqlBoolean GreaterThan(SqlInt32 x, SqlInt32 y)
        {
            return (x > y);
        }

        // Alternative method for operator <=
        public static SqlBoolean LessThanOrEqual(SqlInt32 x, SqlInt32 y)
        {
            return (x <= y);
        }

        // Alternative method for operator >=
        public static SqlBoolean GreaterThanOrEqual(SqlInt32 x, SqlInt32 y)
        {
            return (x >= y);
        }

        // Alternative method for conversions.

        public SqlBoolean ToSqlBoolean()
        {
            return (SqlBoolean)this;
        }

        public SqlByte ToSqlByte()
        {
            return (SqlByte)this;
        }

        public SqlDouble ToSqlDouble()
        {
            return (SqlDouble)this;
        }

        public SqlInt16 ToSqlInt16()
        {
            return (SqlInt16)this;
        }

        public SqlInt64 ToSqlInt64()
        {
            return (SqlInt64)this;
        }

        public SqlMoney ToSqlMoney()
        {
            return (SqlMoney)this;
        }

        public SqlDecimal ToSqlDecimal()
        {
            return (SqlDecimal)this;
        }

        public SqlSingle ToSqlSingle()
        {
            return (SqlSingle)this;
        }

        public SqlString ToSqlString()
        {
            return (SqlString)this;
        }



        // IComparable
        // Compares this object to another object, returning an integer that
        // indicates the relationship.
        // Returns a value less than zero if this < object, zero if this = object,
        // or a value greater than zero if this > object.
        // null is considered to be less than any instance.
        // If object is not of same type, this method throws an ArgumentException.
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int CompareTo(Object value)
        {
            if (value is SqlInt32)
            {
                SqlInt32 i = (SqlInt32)value;

                return CompareTo(i);
            }
            throw ADP.WrongType(value.GetType(), typeof(SqlInt32));
        }

        public int CompareTo(SqlInt32 value)
        {
            // If both Null, consider them equal.
            // Otherwise, Null is less than anything.
            if (IsNull)
                return value.IsNull ? 0 : -1;
            else if (value.IsNull)
                return 1;

            if (this < value) return -1;
            if (this > value) return 1;
            return 0;
        }

        // Compares this instance with a specified object
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override bool Equals(Object value)
        {
            if (!(value is SqlInt32))
            {
                return false;
            }

            SqlInt32 i = (SqlInt32)value;

            if (i.IsNull || IsNull)
                return (i.IsNull && IsNull);
            else
                return (this == i).Value;
        }

        // For hashing purpose
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override int GetHashCode()
        {
            return IsNull ? 0 : Value.GetHashCode();
        }


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static readonly SqlInt32 Null = new SqlInt32(true);
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static readonly SqlInt32 Zero = new SqlInt32(0);
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static readonly SqlInt32 MinValue = new SqlInt32(Int32.MinValue);
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static readonly SqlInt32 MaxValue = new SqlInt32(Int32.MaxValue);
    } // SqlInt32
} // namespace System.Data.SqlTypes

