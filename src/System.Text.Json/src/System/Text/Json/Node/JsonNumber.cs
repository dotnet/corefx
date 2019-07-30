// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// for now disabling error caused by not adding documentation to methods 
#pragma warning disable CS1591

using System.Buffers;

namespace System.Text.Json
{
    public partial class JsonNumber : JsonNode, IEquatable<JsonNumber>
    {
        private string _value;

        public JsonNumber() => _value = "0";

        public JsonNumber(string value) => SetFormattedValue(value);
        
        public JsonNumber(byte value) => SetByte(value);
        
        public JsonNumber(short value) => SetInt16(value);
        
        public JsonNumber(int value) => SetInt32(value);
        
        public JsonNumber(long value) => SetInt64(value);
        
        public JsonNumber(float value) => SetSingle(value);
        
        public JsonNumber(double value) => SetDouble(value);
        
        [CLSCompliant(false)]
        public JsonNumber(sbyte value) => SetSByte(value);
        
        [CLSCompliant(false)]
        public JsonNumber(ushort value) => SetUInt16(value);
        
        [CLSCompliant(false)]
        public JsonNumber(uint value) => SetUInt32(value);
        
        [CLSCompliant(false)]
        public JsonNumber(ulong value) => SetUInt64(value);

        public JsonNumber(decimal value) => SetDecimal(value);

        public override string ToString() => _value; 
        
        public byte GetByte() => byte.Parse(_value);
        
        public short GetInt16() => short.Parse(_value);
        
        public int GetInt32() => int.Parse(_value);
        
        public long GetInt64() => long.Parse(_value);
        
        public float GetSingle() => float.Parse(_value);
        
        public double GetDouble() => double.Parse(_value);
        
        [CLSCompliant(false)]
        public sbyte GetSByte() => sbyte.Parse(_value);
        
        [CLSCompliant(false)]
        public ushort GetUInt16() => ushort.Parse(_value);
        
        [CLSCompliant(false)]
        public uint GetUInt32() => uint.Parse(_value);
        
        [CLSCompliant(false)]
        public ulong GetUInt64() => ulong.Parse(_value);

        public decimal GetDecimal() => decimal.Parse(_value);

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

#pragma warning restore CS1591
