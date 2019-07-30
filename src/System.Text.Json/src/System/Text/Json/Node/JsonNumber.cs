// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    ///   Represents JSON numeric value.
    /// </summary>
    public partial class JsonNumber : JsonNode, IEquatable<JsonNumber>
    {
        private string _value;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class representing the value 0.
        /// </summary>
        public JsonNumber() => _value = "0";

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class representing a specified value.
        /// </summary>
        /// <param name="value">The string representation of a numeric value in a legal JSON number format.</param>
        public JsonNumber(string value) => SetFormattedValue(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a Byte value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        public JsonNumber(byte value) => SetByte(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from an Int16 value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        public JsonNumber(short value) => SetInt16(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from an Int32 value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        public JsonNumber(int value) => SetInt32(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from an Int16 value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        public JsonNumber(long value) => SetInt64(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a Single value.
        /// </summary>
        /// <param name="value">A value to represent.</param>
        public JsonNumber(float value) => SetSingle(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a Double value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        public JsonNumber(double value) => SetDouble(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a SByte value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        [CLSCompliant(false)]
        public JsonNumber(sbyte value) => SetSByte(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a UInt16 value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        [CLSCompliant(false)]
        public JsonNumber(ushort value) => SetUInt16(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a UInt32 value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        [CLSCompliant(false)]
        public JsonNumber(uint value) => SetUInt32(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a UInt64 value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        [CLSCompliant(false)]
        public JsonNumber(ulong value) => SetUInt64(value);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonNumber"/> class from a Decimal value.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        public JsonNumber(decimal value) => SetDecimal(value);

        /// <summary>
        ///   Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString() => _value;

        /// <summary>
        ///   Converts the numeric value of this instance to its Byte equivalent.
        /// </summary>
        /// <returns>A Byte equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Byte.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Byte.
        /// </exception>
        public byte GetByte() => byte.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Int16 equivalent.
        /// </summary>
        /// <returns>A Int16 equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Int16.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Int16.
        /// </exception>
        public short GetInt16() => short.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Int32 equivalent.
        /// </summary>
        /// <returns>A Int32 equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Int32.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Int32.
        /// </exception>
        public int GetInt32() => int.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Int64 equivalent.
        /// </summary>
        /// <returns>A Int64 equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Int64.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Int64.
        /// </exception>
        public long GetInt64() => long.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Single equivalent.
        /// </summary>
        /// <returns>A Single equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Single.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Single.
        /// </exception>
        public float GetSingle() => float.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Double equivalent.
        /// </summary>
        /// <returns>A Double equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Double.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Double.
        /// </exception>
        public double GetDouble() => double.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its SByte equivalent.
        /// </summary>
        /// <returns>A SByte equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of SByte.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with SByte.
        /// </exception>
        [CLSCompliant(false)]
        public sbyte GetSByte() => sbyte.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its UInt16 equivalent.
        /// </summary>
        /// <returns>A UInt16 equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of UInt16.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with UInt16.
        /// </exception>
        [CLSCompliant(false)]
        public ushort GetUInt16() => ushort.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its UInt32 equivalent.
        /// </summary>
        /// <returns>A UInt32 equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of UInt32.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with UInt32.
        /// </exception>
        [CLSCompliant(false)]
        public uint GetUInt32() => uint.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its UInt64 equivalent.
        /// </summary>
        /// <returns>A UInt64 equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of UInt64.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with UInt64.
        /// </exception>
        [CLSCompliant(false)]
        public ulong GetUInt64() => ulong.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Decimal equivalent.
        /// </summary>
        /// <returns>A Decimal equivalent to the number stored by this instance.</returns>
        /// <exception cref="OverflowException">
        ///   <see cref="JsonNumber"/> represents a number less than MinValue or greater than MaxValue of Decimal.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <see cref="JsonNumber"/> represents a number in a format not compliant with Decimal.
        /// </exception>
        public decimal GetDecimal() => decimal.Parse(_value);

        /// <summary>
        ///   Converts the numeric value of this instance to its Byte equivalent.
        ///   A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">
        ///   When this method returns, contains the Byte value equivalent of the number contained in this instance,
        ///   if the conversion succeeded, or zero if the conversion failed.
        /// </param>
        /// <returns>
        ///  // <see langword="true"/> if instance was converted successfully; 
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetByte(out byte value) => byte.TryParse(_value, out value);

        public bool TryGetInt16(out short value) => short.TryParse(_value, out value);

        public bool TryGetInt32(out int value) => int.TryParse(_value, out value);

        public bool TryGetInt64(out long value) => long.TryParse(_value, out value);

        public bool TryGetSingle(out float value) => float.TryParse(_value, out value);

        public bool TryGetDouble(out double value) => double.TryParse(_value, out value);

        [CLSCompliant(false)]
        public bool TryGetSByte(out sbyte value) => sbyte.TryParse(_value, out value);

        [CLSCompliant(false)]
        public bool TryGetUInt16(out ushort value) => ushort.TryParse(_value, out value);

        [CLSCompliant(false)]
        public bool TryGetUInt32(out uint value) => uint.TryParse(_value, out value);

        [CLSCompliant(false)]
        public bool TryGetUInt64(out ulong value) => ulong.TryParse(_value, out value);

        public bool TryGetDecimal(out decimal value) => decimal.TryParse(_value, out value);


        public void SetFormattedValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
                throw new ArgumentException("Expected number, but instead got empty string.", nameof(value));
            
            JsonWriterHelper.ValidateNumber(Encoding.UTF8.GetBytes(value).AsSpan());
            _value = value;
        }

        public void SetByte(byte value) => _value = value.ToString();
        
        public void SetInt16(short value) => _value = value.ToString();
        
        public void SetInt32(int value) => _value = value.ToString();
        
        public void SetInt64(long value) => _value = value.ToString();
        
        public void SetSingle(float value) => _value = value.ToString();
        
        public void SetDouble(double value) => _value = value.ToString();
        
        [CLSCompliant(false)]
        public void SetSByte(sbyte value) => _value = value.ToString();
        
        [CLSCompliant(false)]
        public void SetUInt16(ushort value) => _value = value.ToString();
        
        [CLSCompliant(false)]
        public void SetUInt32(uint value) => _value = value.ToString();
        
        [CLSCompliant(false)]
        public void SetUInt64(ulong value) => _value = value.ToString();

        public void SetDecimal(decimal value) => _value = value.ToString();

        public static implicit operator JsonNumber(byte value) => new JsonNumber(value);
        
        public static implicit operator JsonNumber(int value) => new JsonNumber(value);
        
        public static implicit operator JsonNumber(short value) => new JsonNumber(value);
        
        public static implicit operator JsonNumber(long value) => new JsonNumber(value);
        
        public static implicit operator JsonNumber(float value) => new JsonNumber(value);
        
        public static implicit operator JsonNumber(double value) => new JsonNumber(value);
        
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(sbyte value) => new JsonNumber(value);
        
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(ushort value) => new JsonNumber(value);
        
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(uint value) => new JsonNumber(value);
        
        [CLSCompliant(false)]
        public static implicit operator JsonNumber(ulong value) => new JsonNumber(value);

        public static implicit operator JsonNumber(decimal value) => new JsonNumber(value);

        public override bool Equals(object obj) => obj is JsonNumber number && _value == number._value;

        public override int GetHashCode() => _value.GetHashCode();

        public bool Equals(JsonNumber other) => _value == other._value;

        public static bool operator ==(JsonNumber left, JsonNumber right) => left._value == right._value;
        
        public static bool operator !=(JsonNumber left, JsonNumber right) => left._value != right._value;
    }
}
