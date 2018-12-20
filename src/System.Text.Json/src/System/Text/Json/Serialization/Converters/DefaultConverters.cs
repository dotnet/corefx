// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal static class DefaultConverters
    {
        public static readonly JsonArrayConverterAttribute s_arrayConverterAttibute = new JsonArrayConverterAttribute();
        public static readonly JsonEnumConverterAttribute s_enumConverterAttibute = new JsonEnumConverterAttribute();

        public static Type GetTypeConverter(Type classType, JsonSerializerOptions options)
        {
            JsonClassAttribute attr = options.GetAttributes<JsonClassAttribute>(classType, inherit: true).FirstOrDefault();

            if (attr == null)
            {
                return null;
            }

            Type converterType = attr.ConverterType;
            return converterType;
        }

        public static TAttribute GetPolicy<TAttribute>(
            Type parentClassType,
            PropertyInfo propertyInfo,
            JsonSerializerOptions options) where TAttribute : Attribute
        {
            TAttribute attr = null;
            if (propertyInfo != null)
            {
                // Use Property first
                attr = options.GetAttributes<TAttribute>(propertyInfo).FirstOrDefault();
            }

            if (attr == null)
            {
                // Then class type
                attr = options.GetAttributes<TAttribute>(parentClassType, inherit: true).FirstOrDefault();

                if (attr == null)
                {
                    // Then declaring assembly
                    attr = options.GetAttributes<TAttribute>(parentClassType.Assembly).FirstOrDefault();

                    if (attr == null)
                    {
                        // Then global
                        attr = options.GetAttributes<TAttribute>(JsonSerializerOptions.GlobalAttributesProvider).FirstOrDefault();
                    }
                }
            }

            return attr;
        }

        public static TValue GetPropertyValueOption<TValue>(
            Type parentClassType,
            PropertyInfo propertyInfo,
            JsonSerializerOptions options,
            Func<JsonPropertyValueAttribute, TValue> selector)
        {
            TValue value;

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
                // Then class type
                attr = options.GetAttributes<JsonEnumerableConverterAttribute>(parentClassType, inherit: true).Where(a => a.EnumerableType == enumerableType).FirstOrDefault();

                if (attr == null)
                {
                    // Then declaring assembly
                    attr = options.GetAttributes<JsonEnumerableConverterAttribute>(parentClassType.Assembly).Where(a => a.EnumerableType == enumerableType).FirstOrDefault();

                    if (attr == null)
                    {
                        // Then global
                        attr = options.GetAttributes<JsonEnumerableConverterAttribute>(JsonSerializerOptions.GlobalAttributesProvider).Where(a => a.EnumerableType == enumerableType).FirstOrDefault();

                        // Then default
                        if (enumerableType == typeof(Array))
                        {
                            attr = s_arrayConverterAttibute;
                        }
                    }
                }
            }

            if (attr != null)
            {
                return attr.CreateConverter();
            }

            return null;
        }

        public static JsonValueConverterAttribute GetPropertyValueConverter(
            Type parentClassType,
            PropertyInfo propertyInfo,
            Type propertyType,
            JsonSerializerOptions options)
        {
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            JsonValueConverterAttribute attr = GetPropertyValueConverterInternal(parentClassType, propertyInfo, options, propertyType);

            // For Enums, support both the type Enum plus strongly-typed Enums.
            if (attr == null && (propertyType.IsEnum || propertyType == typeof(Enum)))
            {
                attr = GetPropertyValueConverterInternal(parentClassType, propertyInfo, options, typeof(Enum));
                if (attr == null)
                {
                    attr = s_enumConverterAttibute;
                }
            }

            return attr;
        }

        private static JsonValueConverterAttribute GetPropertyValueConverterInternal(
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
