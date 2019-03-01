// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal static class DefaultConverters
    {
        public static readonly JsonArrayConverterAttribute s_arrayConverterAttibute = new JsonArrayConverterAttribute();
        public static readonly JsonEnumConverterAttribute s_enumConverterAttibute = new JsonEnumConverterAttribute();

        private const int MaxTypeCode = 18;

        private static readonly object[] s_Converters = {
            null,   // Empty 
            null,   // Object
            null,   // DBNull
            new JsonValueConverterBoolean(),
            new JsonValueConverterChar(),
            new JsonValueConverterSByte(),
            new JsonValueConverterByte(),
            new JsonValueConverterInt16(),
            new JsonValueConverterUInt16(),
            new JsonValueConverterInt32(),
            new JsonValueConverterUInt32(),
            new JsonValueConverterInt64(),
            new JsonValueConverterUInt64(),
            new JsonValueConverterSingle(),
            new JsonValueConverterDouble(),
            new JsonValueConverterDecimal(),
            new JsonValueConverterDateTime(),
            null,   // (not a value)
            new JsonValueConverterString()
        };

        private static readonly object[] s_NullableConverters = new object[MaxTypeCode + 1]
        {
            null,
            null,
            null,
            new JsonValueConverterBooleanNullable(),
            new JsonValueConverterCharNullable(),
            new JsonValueConverterSByteNullable(),
            new JsonValueConverterByteNullable(),
            new JsonValueConverterInt16Nullable(),
            new JsonValueConverterUInt16Nullable(),
            new JsonValueConverterInt32Nullable(),
            new JsonValueConverterUInt32Nullable(),
            new JsonValueConverterInt64Nullable(),
            new JsonValueConverterUInt64Nullable(),
            new JsonValueConverterSingleNullable(),
            new JsonValueConverterDoubleNullable(),
            new JsonValueConverterDecimalNullable(),
            new JsonValueConverterDateTimeNullable(),
            null,
            new JsonValueConverterString()
        };

        private static object GetDefaultPropertyValueConverter(Type propertyType, bool isNullable)
        {
            object converter = null;

            int typeCode = (int)Type.GetTypeCode(propertyType);
            if (typeCode <= MaxTypeCode)
            {
                if (isNullable)
                {
                    converter = s_NullableConverters[typeCode];
                }
                else
                {
                    converter = s_Converters[typeCode];
                }
            }

            return converter;
        }

        public static Type GetTypeConverter(Type classType, JsonSerializerOptions options)
        {
            JsonClassAttribute attr = options.GetAttributes<JsonClassAttribute>(classType, inherit: true).FirstOrDefault();

            if (attr == null)
            {
                return null;
            }

            return attr.ConverterType;
        }

        public static TAttribute GetPolicy<TAttribute>(
            Type parentClassType,
            PropertyInfo propertyInfo,
            JsonSerializerOptions options) where TAttribute : Attribute
        {
            Debug.Assert(parentClassType != null);

            TAttribute attr = null;
            if (propertyInfo != null)
            {
                // Use Property first
                attr = options.GetAttributes<TAttribute>(propertyInfo).FirstOrDefault();
            }

            if (attr == null)
            {
                // Look at class first, then assembly and then global.
                attr =
                      options.GetAttributes<TAttribute>(parentClassType, inherit: true).FirstOrDefault() ??
                      options.GetAttributes<TAttribute>(parentClassType.Assembly).FirstOrDefault() ??
                      options.GetAttributes<TAttribute>(JsonSerializerOptions.GlobalAttributesProvider).FirstOrDefault();
            }

            return attr;
        }

        public static TProperty GetPropertyValueOption<TProperty>(
            Type parentClassType,
            PropertyInfo propertyInfo,
            JsonSerializerOptions options,
            Func<JsonPropertyValueAttribute, TProperty> selector)
        {
            TProperty value;

            JsonPropertyValueAttribute attr = null;
            if (propertyInfo != null)
            {
                // Use Property first
                attr = options.GetAttributes<JsonPropertyValueAttribute>(propertyInfo).FirstOrDefault();
            }

            if (attr == null || (value = selector(attr)) == default)
            {
                // Then class type
                attr = options.GetAttributes<JsonPropertyValueAttribute>(parentClassType, inherit: true).FirstOrDefault();
                if (attr == null || (value = selector(attr)) == default)
                {
                    // Then declaring assembly
                    attr = options.GetAttributes<JsonPropertyValueAttribute>(parentClassType.Assembly).FirstOrDefault();

                    if (attr == null || (value = selector(attr)) == default)
                    {
                        // Then global
                        attr = options.GetAttributes<JsonPropertyValueAttribute>(JsonSerializerOptions.GlobalAttributesProvider).FirstOrDefault();

                        if (attr == null)
                        {
                            value = default;
                        }
                        else
                        {
                            value = selector(attr);
                        }
                    }
                }
            }

            return value;
        }

        public static JsonEnumerableConverter GetEnumerableConverter(
            Type parentClassType,
            PropertyInfo propertyInfo,
            Type propertyType,
            JsonSerializerOptions options)
        {
            Type enumerableType;
            if (propertyType.IsGenericType)
            {
                enumerableType = propertyType.GetGenericTypeDefinition();
            }
            else if (propertyType.IsArray)
            {
                enumerableType = typeof(Array);
            }
            else
            {
                enumerableType = propertyType;
            }

            JsonEnumerableConverterAttribute attr = null;
            if (propertyInfo != null)
            {
                // Use Property first
                attr = options.GetAttributes<JsonEnumerableConverterAttribute>(propertyInfo).Where(a => a.EnumerableType == enumerableType).FirstOrDefault();
            }

            if (attr == null)
            {
                attr = options.GetAttributes<JsonEnumerableConverterAttribute>(parentClassType, inherit: true).Where(a => a.EnumerableType == enumerableType).FirstOrDefault() ??
                       options.GetAttributes<JsonEnumerableConverterAttribute>(parentClassType.Assembly).Where(a => a.EnumerableType == enumerableType).FirstOrDefault() ??
                       options.GetAttributes<JsonEnumerableConverterAttribute>(JsonSerializerOptions.GlobalAttributesProvider).Where(a => a.EnumerableType == enumerableType).FirstOrDefault();

                if (attr == null)
                {
                    // Then default
                    if (enumerableType == typeof(Array))
                    {
                        attr = s_arrayConverterAttibute;
                    }
                }
            }

            return attr?.CreateConverter();
        }

        public static JsonValueConverter<TProperty> GetPropertyValueConverter<TProperty>(
            Type parentClassType,
            PropertyInfo propertyInfo,
            Type propertyType,
            JsonSerializerOptions options)
        {
            Type propertyTypeNullableStripped;

            JsonValueConverterAttribute attr = GetPropertyValueConverterAttribute(parentClassType, propertyInfo, options, propertyType);
            if (attr != null)
            {
                return attr.GetConverter<TProperty>();
            }

            bool isNullable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (isNullable)
            {
                propertyTypeNullableStripped = Nullable.GetUnderlyingType(propertyType);
                attr = GetPropertyValueConverterAttribute(parentClassType, propertyInfo, options, propertyTypeNullableStripped);
                if (attr != null)
                {
                    return attr.GetConverter<TProperty>();
                }
            }
            else
            {
                propertyTypeNullableStripped = propertyType;
            }

            // For Enums, support both the type Enum plus strongly-typed Enums.
            if (propertyTypeNullableStripped.IsEnum || propertyTypeNullableStripped == typeof(Enum))
            {
                attr = GetPropertyValueConverterAttribute(parentClassType, propertyInfo, options, typeof(Enum)) ??
                    s_enumConverterAttibute;

                return attr.GetConverter<TProperty>();
            }

            object defaultConverter = GetDefaultPropertyValueConverter(propertyTypeNullableStripped, isNullable);
            return (JsonValueConverter<TProperty>)defaultConverter;
        }

        private static JsonValueConverterAttribute GetPropertyValueConverterAttribute(
            Type parentClassType,
            PropertyInfo propertyInfo,
            JsonSerializerOptions options,
            Type propertyType)
        {
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            JsonValueConverterAttribute attr = null;
            if (propertyInfo != null)
            {
                // Use Property first
                attr = options.GetAttributes<JsonValueConverterAttribute>(propertyInfo).Where(a => a.PropertyType == propertyType).FirstOrDefault();

                if (attr == null)
                {
                    // Then class type
                    attr = options.GetAttributes<JsonValueConverterAttribute>(parentClassType, inherit: true).Where(a => a.PropertyType == propertyType).FirstOrDefault();
                    if (attr == null)
                    {
                        // Then declaring assembly
                        attr = options.GetAttributes<JsonValueConverterAttribute>(parentClassType.Assembly).Where(a => a.PropertyType == propertyType).FirstOrDefault();

                        if (attr == null)
                        {
                            // Then global
                            attr = options.GetAttributes<JsonValueConverterAttribute>(JsonSerializerOptions.GlobalAttributesProvider).Where(a => a.PropertyType == propertyType).FirstOrDefault();
                        }
                    }
                }
            }

            return attr;
        }
    }
}
