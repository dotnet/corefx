// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
    /// <summary>
    /// Represents an 8-bit unsigned integer to be stored in
    /// or retrieved from a database.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [XmlSchemaProvider("GetXsdType")]
    public struct SqlByte : INullable, IComparable, IXmlSerializable
    {
        private bool _fNotNull; // false if null
        private byte _value;

        private static readonly int s_iBitNotByteMax = ~0xff;

        // constructor
        // construct a Null
        private SqlByte(bool fNull)
        {
            _fNotNull = false;
            _value = 0;
        }

        public SqlByte(byte value)
        {
            _value = value;
            _fNotNull = true;
        }

        // INullable
        public bool IsNull
        {
            get { return !_fNotNull; }
        }

        // property: Value
        public byte Value
        {
            get
            {
                if (_fNotNull)
                    return _value;
                else
                    throw new SqlNullValueException();
            }
        }

        // Implicit conversion from byte to SqlByte
        public static implicit operator SqlByte(byte x)
        {
            return new SqlByte(x);
        }

        // Explicit conversion from SqlByte to byte. Throw exception if x is Null.
        public static explicit operator byte (SqlByte x)
        {
            return x.Value;
        }

        public override string ToString()
        {
            return IsNull ? SQLResource.NullString : _value.ToString((IFormatProvider)null);
        }

        public static SqlByte Parse(string s)
        {
            if (s == SQLResource.NullString)
                return SqlByte.Null;
            else
                return new SqlByte(byte.Parse(s, null));
        }

        // Unary operators
        public static SqlByte operator ~(SqlByte x)
        {
            return x.IsNull ? Null : new SqlByte(unchecked((byte)~x._value));
        }


        // Binary operators

        // Arithmetic operators
        public static SqlByte operator +(SqlByte x, SqlByte y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            int iResult = x._value + y._value;
            if ((iResult & s_iBitNotByteMax) != 0)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlByte((byte)iResult);
        }

        public static SqlByte operator -(SqlByte x, SqlByte y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            int iResult = x._value - y._value;
            if ((iResult & s_iBitNotByteMax) != 0)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlByte((byte)iResult);
        }

        public static SqlByte operator *(SqlByte x, SqlByte y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            int iResult = x._value * y._value;
            if ((iResult & s_iBitNotByteMax) != 0)
                throw new OverflowException(SQLResource.ArithOverflowMessage);
            else
                return new SqlByte((byte)iResult);
        }

        public static SqlByte operator /(SqlByte x, SqlByte y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            if (y._value != 0)
            {
                return new SqlByte((byte)(x._value / y._value));
            }
            else
                throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
        }

        public static SqlByte operator %(SqlByte x, SqlByte y)
        {
            if (x.IsNull || y.IsNull)
                return Null;

            if (y._value != 0)
            {
                return new SqlByte((byte)(x._value % y._value));
            }
            else
                throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
        }

        // Bitwise operators
        public static SqlByte operator &(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? Null : new SqlByte((byte)(x._value & y._value));
        }

        public static SqlByte operator |(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? Null : new SqlByte((byte)(x._value | y._value));
        }

        public static SqlByte operator ^(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? Null : new SqlByte((byte)(x._value ^ y._value));
        }



        // Implicit conversions

        // Implicit conversion from SqlBoolean to SqlByte
        public static explicit operator SqlByte(SqlBoolean x)
        {
            return x.IsNull ? Null : new SqlByte(x.ByteValue);
        }


        // Explicit conversions

        // Explicit conversion from SqlMoney to SqlByte
        public static explicit operator SqlByte(SqlMoney x)
        {
            return x.IsNull ? Null : new SqlByte(checked((byte)x.ToInt32()));
        }

        // Explicit conversion from SqlInt16 to SqlByte
        public static explicit operator SqlByte(SqlInt16 x)
        {
            if (x.IsNull)
                return Null;

            if (x.Value > byte.MaxValue || x.Value < byte.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);

            return x.IsNull ? Null : new SqlByte((byte)(x.Value));
        }

        // Explicit conversion from SqlInt32 to SqlByte
        public static explicit operator SqlByte(SqlInt32 x)
        {
            if (x.IsNull)
                return Null;

            if (x.Value > byte.MaxValue || x.Value < byte.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);

            return x.IsNull ? Null : new SqlByte((byte)(x.Value));
        }

        // Explicit conversion from SqlInt64 to SqlByte
        public static explicit operator SqlByte(SqlInt64 x)
        {
            if (x.IsNull)
                return Null;

            if (x.Value > byte.MaxValue || x.Value < byte.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);

            return x.IsNull ? Null : new SqlByte((byte)(x.Value));
        }

        // Explicit conversion from SqlSingle to SqlByte
        public static explicit operator SqlByte(SqlSingle x)
        {
            if (x.IsNull)
                return Null;

            if (x.Value > byte.MaxValue || x.Value < byte.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);

            return x.IsNull ? Null : new SqlByte((byte)(x.Value));
        }

        // Explicit conversion from SqlDouble to SqlByte
        public static explicit operator SqlByte(SqlDouble x)
        {
            if (x.IsNull)
                return Null;

            if (x.Value > byte.MaxValue || x.Value < byte.MinValue)
                throw new OverflowException(SQLResource.ArithOverflowMessage);

            return x.IsNull ? Null : new SqlByte((byte)(x.Value));
        }

        // Explicit conversion from SqlDecimal to SqlByte
        public static explicit operator SqlByte(SqlDecimal x)
        {
            return (SqlByte)(SqlInt32)x;
        }

        // Implicit conversion from SqlString to SqlByte
        // Throws FormatException or OverflowException if necessary.
        public static explicit operator SqlByte(SqlString x)
        {
            return x.IsNull ? Null : new SqlByte(byte.Parse(x.Value, null));
        }

        // Overloading comparison operators
        public static SqlBoolean operator ==(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value == y._value);
        }

        public static SqlBoolean operator !=(SqlByte x, SqlByte y)
        {
            return !(x == y);
        }

        public static SqlBoolean operator <(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value < y._value);
        }

        public static SqlBoolean operator >(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value > y._value);
        }

        public static SqlBoolean operator <=(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value <= y._value);
        }

        public static SqlBoolean operator >=(SqlByte x, SqlByte y)
        {
            return (x.IsNull || y.IsNull) ? SqlBoolean.Null : new SqlBoolean(x._value >= y._value);
        }

        //--------------------------------------------------
        // Alternative methods for overloaded operators
        //--------------------------------------------------

        // Alternative method for operator ~
        public static SqlByte OnesComplement(SqlByte x)
        {
            return ~x;
        }

        // Alternative method for operator +
        public static SqlByte Add(SqlByte x, SqlByte y)
        {
            return x + y;
        }

        // Alternative method for operator -
        public static SqlByte Subtract(SqlByte x, SqlByte y)
        {
            return x - y;
        }

        // Alternative method for operator *
        public static SqlByte Multiply(SqlByte x, SqlByte y)
        {
            return x * y;
        }

        // Alternative method for operator /
        public static SqlByte Divide(SqlByte x, SqlByte y)
        {
            return x / y;
        }

        // Alternative method for operator %
        public static SqlByte Mod(SqlByte x, SqlByte y)
        {
            return x % y;
        }

        public static SqlByte Modulus(SqlByte x, SqlByte y)
        {
            return x % y;
        }

        // Alternative method for operator &
        public static SqlByte BitwiseAnd(SqlByte x, SqlByte y)
        {
            return x & y;
        }

        // Alternative method for operator |
        public static SqlByte BitwiseOr(SqlByte x, SqlByte y)
        {
            return x | y;
        }

        // Alternative method for operator ^
        public static SqlByte Xor(SqlByte x, SqlByte y)
        {
            return x ^ y;
        }

        // Alternative method for operator ==
        public static SqlBoolean Equals(SqlByte x, SqlByte y)
        {
            return (x == y);
        }

        // Alternative method for operator !=
        public static SqlBoolean NotEquals(SqlByte x, SqlByte y)
        {
            return (x != y);
        }

        // Alternative method for operator <
        public static SqlBoolean LessThan(SqlByte x, SqlByte y)
        {
            return (x < y);
        }

        // Alternative method for operator >
        public static SqlBoolean GreaterThan(SqlByte x, SqlByte y)
        {
            return (x > y);
        }

        // Alternative method for operator <=
        public static SqlBoolean LessThanOrEqual(SqlByte x, SqlByte y)
        {
            return (x <= y);
        }

        // Alternative method for operator >=
        public static SqlBoolean GreaterThanOrEqual(SqlByte x, SqlByte y)
        {
            return (x >= y);
        }

        // Alternative method for conversions.

        public SqlBoolean ToSqlBoolean()
        {
            return (SqlBoolean)this;
        }

        public SqlDouble ToSqlDouble()
        {
            return this;
        }

        public SqlInt16 ToSqlInt16()
        {
            return this;
        }

        public SqlInt32 ToSqlInt32()
        {
            return this;
        }

        public SqlInt64 ToSqlInt64()
        {
            return this;
        }

        public SqlMoney ToSqlMoney()
        {
            return this;
        }

        public SqlDecimal ToSqlDecimal()
        {
            return this;
        }

        public SqlSingle ToSqlSingle()
        {
            return this;
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
        public int CompareTo(object value)
        {
            if (value is SqlByte)
            {
                SqlByte i = (SqlByte)value;

                return CompareTo(i);
            }
            throw ADP.WrongType(value.GetType(), typeof(SqlByte));
        }

        public int CompareTo(SqlByte value)
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
        public override bool Equals(object value)
        {
            if (!(value is SqlByte))
            {
                return false;
            }

            SqlByte i = (SqlByte)value;

            if (i.IsNull || IsNull)
                return (i.IsNull && IsNull);
            else
                return (this == i).Value;
        }

        // For hashing purpose
        public override int GetHashCode()
        {
            return IsNull ? 0 : Value.GetHashCode();
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            string isNull = reader.GetAttribute("nil", XmlSchema.InstanceNamespace);
            if (isNull != null && XmlConvert.ToBoolean(isNull))
            {
                // Read the next value.
                reader.ReadElementString();
                _fNotNull = false;
            }
            else
            {
                _value = XmlConvert.ToByte(reader.ReadElementString());
                _fNotNull = true;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (IsNull)
            {
                writer.WriteAttributeString("xsi", "nil", XmlSchema.InstanceNamespace, "true");
            }
            else
            {
                writer.WriteString(XmlConvert.ToString(_value));
            }
        }

        public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
        {
            return new XmlQualifiedName("unsignedByte", XmlSchema.Namespace);
        }

        public static readonly SqlByte Null = new SqlByte(true);
        public static readonly SqlByte Zero = new SqlByte(0);
        public static readonly SqlByte MinValue = new SqlByte(byte.MinValue);
        public static readonly SqlByte MaxValue = new SqlByte(byte.MaxValue);
    } // SqlByte
} // namespace System
