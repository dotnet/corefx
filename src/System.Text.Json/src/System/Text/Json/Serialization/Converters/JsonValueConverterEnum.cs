// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonValueConverterEnum<TValue> : JsonValueConverter<TValue>
        where TValue : struct, Enum
    {
        private static readonly bool s_isUInt64 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(ulong);
        private static readonly bool s_isInt64 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(long);

        private static readonly bool s_isUInt32 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(uint);
        private static readonly bool s_isInt32 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(int);

        private static readonly bool s_isUInt16 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(ushort);
        private static readonly bool s_isInt16 = Enum.GetUnderlyingType(typeof(TValue)) == typeof(ushort);

        private static readonly bool s_isByte = Enum.GetUnderlyingType(typeof(TValue)) == typeof(byte);
        private static readonly bool s_isSByte = Enum.GetUnderlyingType(typeof(TValue)) == typeof(sbyte);

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

            if (s_isInt32)
            {
                if (reader.TryGetInt32(out int int32))
                {
                    value = (TValue)Enum.ToObject(valueType, int32);
                    return true;
                }
                goto False;
            }

            if (s_isByte)
            {
                if (reader.TryGetUInt32(out uint ubyte8) && JsonHelpers.IsInRangeInclusive(ubyte8, byte.MinValue, byte.MaxValue))
                {
                    value = (TValue)Enum.ToObject(valueType, ubyte8);
                    return true;
                }
                goto False;
            }

            if (s_isUInt64)
            {
                if (reader.TryGetUInt64(out ulong uint64))
                {
                    value = (TValue)Enum.ToObject(valueType, uint64);
                    return true;
                }
                goto False;
            }

            if (s_isInt64)
            {
                if (reader.TryGetInt64(out long int64))
                {
                    value = (TValue)Enum.ToObject(valueType, int64);
                    return true;
                }
                goto False;
            }

            if (s_isUInt32)
            {
                if (reader.TryGetUInt32(out uint uint32))
                {
                    value = (TValue)Enum.ToObject(valueType, uint32);
                    return true;
                }
                goto False;
            }

            // When utf8reader/writer will support all primitive types we should remove custom bound checks
            // https://github.com/dotnet/corefx/issues/36125

            if (s_isUInt16)
            {
                if (reader.TryGetUInt32(out uint uint16) && JsonHelpers.IsInRangeInclusive(uint16, ushort.MinValue, ushort.MaxValue))
                {
                    value = (TValue)Enum.ToObject(valueType, uint16);
                    return true;
                }
                goto False;
            }

            if (s_isInt16)
            {
                if (reader.TryGetInt32(out int int16) && JsonHelpers.IsInRangeInclusive(int16, short.MinValue, short.MaxValue))
                {
                    value = (TValue)Enum.ToObject(valueType, int16);
                    return true;
                }
                goto False;
            }

            if (s_isSByte)
            {
                if (reader.TryGetInt32(out int byte8) && JsonHelpers.IsInRangeInclusive(byte8, sbyte.MinValue, sbyte.MaxValue))
                {
                    value = (TValue)Enum.ToObject(valueType, byte8);
                    return true;
                }
                goto False;
            }

        False:
            value = default;
            return false;
        }

        public override void Write(TValue value, Utf8JsonWriter writer)
        {
            if (TreatAsString)
            {
                writer.WriteStringValue(value.ToString());
            }
            else if (s_isUInt64)
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
            else if (s_isUInt64)
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
