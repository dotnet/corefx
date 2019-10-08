// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonConverterEnum<T> : JsonConverter<T>
        where T : struct, Enum
    {
        private class EnumInfo
        {
            public string Name;
            public T EnumValue;
            public ulong RawValue;
        }

        private static readonly TypeCode s_enumTypeCode = Type.GetTypeCode(typeof(T));
        private static PropertyInfo s_enumMemberAttributeValuePropertyInfo;

        private static ulong GetEnumValue(object value)
        {
            switch (s_enumTypeCode)
            {
                // Switch cases ordered by expected frequency

                case TypeCode.Int32:
                    return (ulong)(int)value;
                case TypeCode.UInt32:
                    return (uint)value;
                case TypeCode.UInt64:
                    return (ulong)value;
                case TypeCode.Int64:
                    return (ulong)(long)value;

                case TypeCode.SByte:
                    return (ulong)(sbyte)value;
                case TypeCode.Byte:
                    return (byte)value;
                case TypeCode.Int16:
                    return (ulong)(short)value;
                case TypeCode.UInt16:
                    return (ushort)value;
            }

            ThrowHelper.ThrowJsonException();
            return 0;
        }

        private static string GetEnumMemberValue(Attribute enumMemberAttribute)
        {
            if (s_enumMemberAttributeValuePropertyInfo == null)
            {
                s_enumMemberAttributeValuePropertyInfo = enumMemberAttribute
                    .GetType()
                    .GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            }

            return (string)s_enumMemberAttributeValuePropertyInfo.GetValue(enumMemberAttribute);
        }

        private readonly EnumConverterOptions _converterOptions;
        private readonly bool _isFlags;
        private readonly Dictionary<ulong, EnumInfo> _rawToTransformed;
        private readonly Dictionary<string, EnumInfo> _transformedToRaw;

        public override bool CanConvert(Type type)
        {
            return type.IsEnum;
        }

        public JsonConverterEnum(EnumConverterOptions options)
            : this(options, namingPolicy: null)
        {
        }

        public JsonConverterEnum(EnumConverterOptions options, JsonNamingPolicy namingPolicy)
        {
            _converterOptions = options;

            Type enumType = typeof(T);

            _isFlags = enumType.IsDefined(typeof(FlagsAttribute), true);

            string[] builtInNames = enumType.GetEnumNames();
            Array builtInValues = enumType.GetEnumValues();

            Debug.Assert(builtInNames.Length == builtInValues.Length);

            _rawToTransformed = new Dictionary<ulong, EnumInfo>();
            _transformedToRaw = new Dictionary<string, EnumInfo>();

            for (int i = 0; i < builtInNames.Length; i++)
            {
                T enumValue = (T)builtInValues.GetValue(i);
                ulong rawValue = GetEnumValue(enumValue);

                string name = builtInNames[i];

                string transformedName;
                if (namingPolicy == null)
                {
                    FieldInfo field = enumType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)!;
                    Attribute enumMemberAttribute = field.GetCustomAttributes()?.FirstOrDefault(ca => ca.GetType().FullName == "System.Runtime.Serialization.EnumMemberAttribute");
                    if (enumMemberAttribute != null)
                    {
                        transformedName = GetEnumMemberValue(enumMemberAttribute) ?? name;
                    }
                    else
                    {
                        transformedName = name;
                    }
                }
                else
                {
                    transformedName = namingPolicy.ConvertName(name) ?? name;
                }

                _rawToTransformed[rawValue] = new EnumInfo
                {
                    Name = transformedName,
                    EnumValue = enumValue,
                    RawValue = rawValue
                };
                _transformedToRaw[transformedName] = new EnumInfo
                {
                    Name = name,
                    EnumValue = enumValue,
                    RawValue = rawValue
                };
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonTokenType token = reader.TokenType;

            if (token == JsonTokenType.String)
            {
                if (!_converterOptions.HasFlag(EnumConverterOptions.AllowStrings))
                {
                    ThrowHelper.ThrowJsonException();
                    return default;
                }

                // Try parsing case sensitive first
                string enumString = reader.GetString();

                // Case sensitive search attempted first.
                if (_transformedToRaw.TryGetValue(enumString, out EnumInfo enumInfo))
                {
                    return Unsafe.As<ulong, T>(ref enumInfo.RawValue);
                }

                if (_isFlags)
                {
                    ulong calculatedValue = 0;

                    string[] flagValues = enumString.Split(", ");
                    foreach (string flagValue in flagValues)
                    {
                        // Case sensitive search attempted first.
                        if (_transformedToRaw.TryGetValue(flagValue, out enumInfo))
                        {
                            calculatedValue |= enumInfo.RawValue;
                        }
                        else
                        {
                            // Case insensitive search attempted second.

                            bool matched = false;
                            foreach (KeyValuePair<string, EnumInfo> enumItem in _transformedToRaw)
                            {
                                if (string.Equals(enumItem.Key, flagValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    calculatedValue |= enumItem.Value.RawValue;
                                    matched = true;
                                    break;
                                }
                            }

                            if (!matched)
                            {
                                ThrowHelper.ThrowJsonException();
                            }
                        }
                    }

                    return Unsafe.As<ulong, T>(ref calculatedValue);
                }
                else
                {
                    // Case insensitive search attempted second.
                    foreach (KeyValuePair<string, EnumInfo> enumItem in _transformedToRaw)
                    {
                        if (string.Equals(enumItem.Key, enumString, StringComparison.OrdinalIgnoreCase))
                        {
                            return Unsafe.As<ulong, T>(ref enumItem.Value.RawValue);
                        }
                    }
                }

                ThrowHelper.ThrowJsonException();
                return default;
            }

            if (token != JsonTokenType.Number || !_converterOptions.HasFlag(EnumConverterOptions.AllowNumbers))
            {
                ThrowHelper.ThrowJsonException();
                return default;
            }

            switch (s_enumTypeCode)
            {
                // Switch cases ordered by expected frequency

                case TypeCode.Int32:
                    if (reader.TryGetInt32(out int int32))
                    {
                        return Unsafe.As<int, T>(ref int32);
                    }
                    break;
                case TypeCode.UInt32:
                    if (reader.TryGetUInt32(out uint uint32))
                    {
                        return Unsafe.As<uint, T>(ref uint32);
                    }
                    break;
                case TypeCode.UInt64:
                    if (reader.TryGetUInt64(out ulong uint64))
                    {
                        return Unsafe.As<ulong, T>(ref uint64);
                    }
                    break;
                case TypeCode.Int64:
                    if (reader.TryGetInt64(out long int64))
                    {
                        return Unsafe.As<long, T>(ref int64);
                    }
                    break;

                // When utf8reader/writer will support all primitive types we should remove custom bound checks
                // https://github.com/dotnet/corefx/issues/36125
                case TypeCode.SByte:
                    if (reader.TryGetInt32(out int byte8) && JsonHelpers.IsInRangeInclusive(byte8, sbyte.MinValue, sbyte.MaxValue))
                    {
                        sbyte byte8Value = (sbyte)byte8;
                        return Unsafe.As<sbyte, T>(ref byte8Value);
                    }
                    break;
                case TypeCode.Byte:
                    if (reader.TryGetUInt32(out uint ubyte8) && JsonHelpers.IsInRangeInclusive(ubyte8, byte.MinValue, byte.MaxValue))
                    {
                        byte ubyte8Value = (byte)ubyte8;
                        return Unsafe.As<byte, T>(ref ubyte8Value);
                    }
                    break;
                case TypeCode.Int16:
                    if (reader.TryGetInt32(out int int16) && JsonHelpers.IsInRangeInclusive(int16, short.MinValue, short.MaxValue))
                    {
                        short shortValue = (short)int16;
                        return Unsafe.As<short, T>(ref shortValue);
                    }
                    break;
                case TypeCode.UInt16:
                    if (reader.TryGetUInt32(out uint uint16) && JsonHelpers.IsInRangeInclusive(uint16, ushort.MinValue, ushort.MaxValue))
                    {
                        ushort ushortValue = (ushort)uint16;
                        return Unsafe.As<ushort, T>(ref ushortValue);
                    }
                    break;
            }

            ThrowHelper.ThrowJsonException();
            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // If strings are allowed, attempt to write it out as a string value
            if (_converterOptions.HasFlag(EnumConverterOptions.AllowStrings))
            {
                ulong rawValue = GetEnumValue(value);

                if (_rawToTransformed.TryGetValue(rawValue, out EnumInfo enumInfo))
                {
                    writer.WriteStringValue(enumInfo.Name);
                    return;
                }

                if (_isFlags)
                {
                    ulong calculatedValue = 0;

                    StringBuilder Builder = new StringBuilder();
                    foreach (KeyValuePair<ulong, EnumInfo> enumItem in _rawToTransformed)
                    {
                        enumInfo = enumItem.Value;
                        if (!value.HasFlag(enumInfo.EnumValue)
                            || enumInfo.RawValue == 0) // Definitions with 'None' should hit the cache case.
                        {
                            continue;
                        }

                        // Track the value to make sure all bits are represented.
                        calculatedValue |= enumInfo.RawValue;

                        if (Builder.Length > 0)
                            Builder.Append(", ");
                        Builder.Append(enumInfo.Name);
                    }
                    if (calculatedValue == rawValue)
                    {
                        writer.WriteStringValue(Builder.ToString());
                        return;
                    }
                }
            }

            if (!_converterOptions.HasFlag(EnumConverterOptions.AllowNumbers))
            {
                ThrowHelper.ThrowJsonException();
            }

            switch (s_enumTypeCode)
            {
                case TypeCode.Int32:
                    writer.WriteNumberValue(Unsafe.As<T, int>(ref value));
                    break;
                case TypeCode.UInt32:
                    writer.WriteNumberValue(Unsafe.As<T, uint>(ref value));
                    break;
                case TypeCode.UInt64:
                    writer.WriteNumberValue(Unsafe.As<T, ulong>(ref value));
                    break;
                case TypeCode.Int64:
                    writer.WriteNumberValue(Unsafe.As<T, long>(ref value));
                    break;
                case TypeCode.Int16:
                    writer.WriteNumberValue(Unsafe.As<T, short>(ref value));
                    break;
                case TypeCode.UInt16:
                    writer.WriteNumberValue(Unsafe.As<T, ushort>(ref value));
                    break;
                case TypeCode.Byte:
                    writer.WriteNumberValue(Unsafe.As<T, byte>(ref value));
                    break;
                case TypeCode.SByte:
                    writer.WriteNumberValue(Unsafe.As<T, sbyte>(ref value));
                    break;
                default:
                    ThrowHelper.ThrowJsonException();
                    break;
            }
        }
    }
}
