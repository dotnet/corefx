﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;

namespace System.Text.Json
{
    internal partial class JsonClassInfo
    {
        private JsonPropertyInfo AddProperty(Type propertyType, PropertyInfo propertyInfo, Type parentClassType, JsonSerializerOptions options)
        {
            bool hasIgnoreAttribute = (JsonPropertyInfo.GetAttribute<JsonIgnoreAttribute>(propertyInfo) != null);
            if (hasIgnoreAttribute)
            {
                return JsonPropertyInfo.CreateIgnoredPropertyPlaceholder(propertyInfo, options);
            }

            ClassType classType = GetClassType(
                propertyType,
                parentClassType,
                propertyInfo,
                out Type runtimeType,
                out Type elementType,
                out Type nullableUnderlyingType,
                out _,
                out JsonConverter converter,
                checkForAddMethod: false,
                options);

            return CreateProperty(
                declaredPropertyType: propertyType,
                runtimePropertyType: runtimeType,
                propertyInfo,
                parentClassType,
                collectionElementType: elementType,
                nullableUnderlyingType,
                converter,
                classType,
                options);
        }

        internal static JsonPropertyInfo CreateProperty(
            Type declaredPropertyType,
            Type runtimePropertyType,
            PropertyInfo propertyInfo,
            Type parentClassType,
            Type collectionElementType,
            Type nullableUnderlyingType,
            JsonConverter converter,
            ClassType classType,
            JsonSerializerOptions options)
        {
            bool treatAsNullable = nullableUnderlyingType != null;

            // Obtain the type of the JsonPropertyInfo class to construct.
            Type propertyInfoClassType;

            if (treatAsNullable && converter != null)
            {
                propertyInfoClassType = typeof(JsonPropertyInfoNullable<,>).MakeGenericType(parentClassType, nullableUnderlyingType);
            }
            else
            {
                Type typeToConvert = converter?.TypeToConvert;
                if (typeToConvert == null)
                {
                    typeToConvert = declaredPropertyType;
                }

                // For the covariant case, create JsonPropertyInfoNotNullable. The generic constraints are "where TConverter : TDeclaredProperty".
                if (runtimePropertyType.IsAssignableFrom(typeToConvert))
                {
                    propertyInfoClassType = typeof(JsonPropertyInfoNotNullable<,,,>).MakeGenericType(
                        parentClassType,
                        declaredPropertyType,
                        runtimePropertyType,
                        typeToConvert);
                }
                else
                {
                    Debug.Assert(typeToConvert.IsAssignableFrom(runtimePropertyType));

                    // For the contravariant case, create JsonPropertyInfoNotNullableContravariant. The generic constraints are "where TDeclaredProperty : TConverter".
                    propertyInfoClassType = typeof(JsonPropertyInfoNotNullableContravariant<,,,>).MakeGenericType(
                        parentClassType,
                        declaredPropertyType,
                        runtimePropertyType,
                        typeToConvert);
                }
            }

            // Create the JsonPropertyInfo instance.
            JsonPropertyInfo jsonPropertyInfo = (JsonPropertyInfo)Activator.CreateInstance(
                propertyInfoClassType,
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null);

            jsonPropertyInfo.Initialize(
                parentClassType,
                declaredPropertyType,
                runtimePropertyType,
                runtimeClassType: classType,
                propertyInfo,
                collectionElementType,
                converter,
                treatAsNullable,
                options);

            return jsonPropertyInfo;
        }

        internal JsonPropertyInfo CreateRootObject(JsonSerializerOptions options)
        {
            JsonConverter converter = options.DetermineConverterForProperty(Type, Type, propertyInfo: null);

            return CreateProperty(
                declaredPropertyType: Type,
                runtimePropertyType: Type,
                propertyInfo: null,
                parentClassType: Type,
                ElementType,
                Nullable.GetUnderlyingType(Type),
                converter,
                ClassType,
                options);
        }

        internal JsonPropertyInfo GetOrAddPolymorphicProperty(JsonPropertyInfo property, Type runtimePropertyType, JsonSerializerOptions options)
        {
            static JsonPropertyInfo CreateRuntimeProperty((JsonPropertyInfo property, Type runtimePropertyType) key, (JsonSerializerOptions options, Type classType) arg)
            {
                ClassType classType = GetClassType(
                    key.runtimePropertyType,
                    arg.classType,
                    key.property.PropertyInfo,
                    out _,
                    out Type elementType,
                    out Type nullableType,
                    out _,
                    out JsonConverter converter,
                    checkForAddMethod: false,
                    arg.options);

                JsonPropertyInfo runtimeProperty = CreateProperty(
                    key.property.DeclaredPropertyType,
                    key.runtimePropertyType,
                    key.property.PropertyInfo,
                    parentClassType: arg.classType,
                    collectionElementType: elementType,
                    nullableType,
                    converter,
                    classType,
                    options: arg.options);
                key.property.CopyRuntimeSettingsTo(runtimeProperty);

                return runtimeProperty;
            }

            ConcurrentDictionary<(JsonPropertyInfo, Type), JsonPropertyInfo> cache =
                LazyInitializer.EnsureInitialized(ref RuntimePropertyCache, () => new ConcurrentDictionary<(JsonPropertyInfo, Type), JsonPropertyInfo>());
#if BUILDING_INBOX_LIBRARY
            return cache.GetOrAdd((property, runtimePropertyType), (key, arg) => CreateRuntimeProperty(key, arg), (options, Type));
#else
            return cache.GetOrAdd((property, runtimePropertyType), key => CreateRuntimeProperty(key, (options, Type)));
#endif
        }
    }
}
