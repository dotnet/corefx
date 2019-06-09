// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterEnum<TValue> : JsonValueConverter<TValue>
        where TValue : struct, Enum
    {
        private static readonly bool s_isUint64 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(ulong);
        private static readonly bool s_isInt64 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(long);

        private static readonly bool s_isUint32 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(uint);
        private static readonly bool s_isInt32 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(int);

        private static readonly bool s_isUshort = Enum.GetUnderlyingType(typeof(TValue)) == typeof(ushort);
        private static readonly bool s_isshort = Enum.GetUnderlyingType(typeof(TValue)) == typeof(ushort);

        private static readonly bool s_isByte = Enum.GetUnderlyingType(typeof(TValue)) == typeof(byte);

        public bool TreatAsString { get; private set; }

        internal JsonValueConverterEnum(bool treatAsString)
        {
            TreatAsString = treatAsString;
        }

        public override bool TryRead(Type valueType, ref Utf8JsonReader reader, out TValue value)
        {
            if (TreatAsString)
            {
                // Assume the token is a string
                if (reader.TokenType != JsonTokenType.String)
                {
                    value = default;
                    return false;
                }

                string enumString = reader.GetString();
                return Enum.TryParse(enumString, out value);
            }

            if (reader.TokenType != JsonTokenType.Number)
            {
                value = default;
                return false;
            }

            if (s_isUint64)
            {
                if (reader.TryGetUInt64(out ulong ulongValue))
                {
                    value = (TValue)Enum.ToObject(valueType, ulongValue);
                    return true;
                }
                else if (reader.TryGetInt64(out long fallback) && fallback == -1)
                {
                    value = (TValue)Enum.ToObject(valueType, fallback);
                    return true;
                }

                value = default;
                return false;
            }

            if (s_isInt64)
            {
                if (reader.TryGetInt64(out long ulongValue))
                {
                    value = (TValue)Enum.ToObject(valueType, ulongValue);
                    return true;
                }

                value = default;
                return false;
            }

            if (s_isUint32)
            {
                if (reader.TryGetUInt32(out uint uintValue))
                {
                    value = (TValue)Enum.ToObject(valueType, uintValue);
                    return true;
                } 
                else if (reader.TryGetInt64(out long fallback) && fallback == -1)
                {
                    value = (TValue)Enum.ToObject(valueType, fallback);
                    return true;
                }

                value = default;
                return false;
            }

            if (s_isInt32)
            {
                if (reader.TryGetInt32(out int intValue))
                {
                    value = (TValue)Enum.ToObject(valueType, intValue);
                    return true;
                } 

                value = default;
                return false;
            }

            if (s_isUshort)
            {
                if (reader.TryGetUInt32(out uint uintValue) && uintValue >= ushort.MinValue && uintValue <= ushort.MaxValue)
                {
                    value = (TValue)Enum.ToObject(valueType, uintValue);
                    return true;
                }
                else if (reader.TryGetInt64(out long fallback) && fallback == -1)
                {
                    value = (TValue)Enum.ToObject(valueType, fallback);
                    return true;
                }

                value = default;
                return false;
            }

            if (s_isshort)
            {
                if (reader.TryGetInt32(out int intValue) && intValue >= short.MinValue && intValue <= short.MaxValue)
                {
                    value = (TValue)Enum.ToObject(valueType, intValue);
                    return true;
                }
                else if (reader.TryGetInt64(out long fallback) && fallback == -1)
                {
                    value = (TValue)Enum.ToObject(valueType, fallback);
                    return true;
                }

                value = default;
                return false;
            }

            if (s_isByte)
            {
                if (reader.TryGetInt32(out int intValue) && intValue >= byte.MinValue && intValue <= byte.MinValue)
                {
                    value = (TValue)Enum.ToObject(valueType, intValue);
                    return true;
                }
                else if (reader.TryGetInt64(out long fallback) && fallback == -1)
                {
                    value = (TValue)Enum.ToObject(valueType, fallback);
                    return true;
                }

                value = default;
                return false;
            }

            value = default;
            return false;
        }

        public override void Write(TValue value, Utf8JsonWriter writer)
        {
            if (TreatAsString)
            {
                writer.WriteStringValue(value.ToString());
            }
            else if (s_isUint64)
            {
                // Use the ulong converter to prevent conversion into a signed\long value.
                ulong ulongValue = Convert.ToUInt64(value);
                writer.WriteNumberValue(ulongValue);
            }
            else
            {
                // long can hold the signed\unsigned values of other integer types
                long longValue = Convert.ToInt64(value);
                writer.WriteNumberValue(longValue);
            }
        }

        public override void Write(JsonEncodedText propertyName, TValue value, Utf8JsonWriter writer)
        {
            if (TreatAsString)
            {
                writer.WriteString(propertyName, value.ToString());
            }
            else if (s_isUint64)
            {
                // Use the ulong converter to prevent conversion into a signed\long value.
                ulong ulongValue = Convert.ToUInt64(value);
                writer.WriteNumber(propertyName, ulongValue);
            }
            else
            {
                // long can hold the signed\unsigned values of other integer types.
                long longValue = Convert.ToInt64(value);
                writer.WriteNumber(propertyName, longValue);
            }
        }
    }
}
