// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal partial class JsonClassInfo
    {
        private JsonPropertyInfo AddProperty(Type propertyType, PropertyInfo propertyInfo, Type parentClassType, JsonSerializerOptions options)
        {
            Type elementType;
            Type runtimeType;
            ClassType classType;

            // If a converter was provided, we should use the provided type as the runtime type and use the converter later.
            JsonConverter converter = options.DetermineConverterForProperty(parentClassType, propertyType, propertyInfo);

            if (converter != null)
            {
                elementType = null;
                runtimeType = propertyType;
                classType = propertyType == typeof(object) ? ClassType.Unknown : ClassType.Value;
            }
            else
            {
                classType = GetClassType(propertyType, out runtimeType, out elementType, out _, checkForConverter: false, checkForAddMethod: false, options);
            }

            return CreateProperty(
                declaredPropertyType: propertyType,
                runtimePropertyType: runtimeType,
                propertyInfo,
                parentClassType,
                collectionElementType: elementType,
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
            JsonConverter converter,
            ClassType classType,
            JsonSerializerOptions options)
        {
            bool hasIgnoreAttribute = (JsonPropertyInfo.GetAttribute<JsonIgnoreAttribute>(propertyInfo) != null);
            if (hasIgnoreAttribute)
            {
                return JsonPropertyInfo.CreateIgnoredPropertyPlaceholder(propertyInfo, options);
            }

            return CreatePropertyInternal(
                declaredPropertyType,
                runtimePropertyType,
                propertyInfo,
                parentClassType,
                collectionElementType,
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
            ClassType classType,
            JsonSerializerOptions options)
        {
            bool hasIgnoreAttribute = (JsonPropertyInfo.GetAttribute<JsonIgnoreAttribute>(propertyInfo) != null);
            if (hasIgnoreAttribute)
            {
                return JsonPropertyInfo.CreateIgnoredPropertyPlaceholder(propertyInfo, options);
            }

            JsonConverter converter = options.DetermineConverterForProperty(parentClassType, runtimePropertyType, propertyInfo);

            return CreatePropertyInternal(
                declaredPropertyType,
                runtimePropertyType,
                propertyInfo,
                parentClassType,
                collectionElementType,
                converter,
                classType,
                options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static JsonPropertyInfo CreatePropertyInternal(
            Type declaredPropertyType,
            Type runtimePropertyType,
            PropertyInfo propertyInfo,
            Type parentClassType,
            Type collectionElementType,
            JsonConverter converter,
            ClassType classType,
            JsonSerializerOptions options)
        {

            // Create the JsonPropertyInfo<TType, TProperty>
            Type propertyInfoClassType;
            if (runtimePropertyType.IsGenericType && runtimePropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // If there's no converter for the Nullable, not found use the underlying type.
                // This supports custom converters that want to (de)serialize as null when the value is not null.
                if (converter != null)
                {
                    propertyInfoClassType = typeof(JsonPropertyInfoNotNullable<,,,>).MakeGenericType(
                        parentClassType,
                        declaredPropertyType,
                        runtimePropertyType,
                        runtimePropertyType);
                }
                else
                {
                    Type typeToConvert = Nullable.GetUnderlyingType(runtimePropertyType);
                    converter = options.DetermineConverterForProperty(parentClassType, typeToConvert, propertyInfo);
                    propertyInfoClassType = typeof(JsonPropertyInfoNullable<,>).MakeGenericType(parentClassType, typeToConvert);
                }
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

            JsonPropertyInfo jsonInfo = (JsonPropertyInfo)Activator.CreateInstance(
                propertyInfoClassType,
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null);

            jsonInfo.Initialize(
                parentClassType,
                declaredPropertyType,
                runtimePropertyType,
                runtimeClassType: classType,
                propertyInfo,
                collectionElementType,
                converter,
                options);

            return jsonInfo;
        }

        internal JsonPropertyInfo CreateRootObject(JsonSerializerOptions options)
        {
            return CreateProperty(
                declaredPropertyType: Type,
                runtimePropertyType: Type,
                propertyInfo: null,
                parentClassType: Type,
                ElementType,
                ClassType,
                options: options);
        }

        internal JsonPropertyInfo CreatePolymorphicProperty(JsonPropertyInfo property, Type runtimePropertyType, JsonSerializerOptions options)
        {
            JsonConverter converter = options.DetermineConverterForProperty(Type, runtimePropertyType, property.PropertyInfo);
            ClassType classType;

            if (converter == null)
            {
                classType = GetClassType(runtimePropertyType, checkForConverter: false, options);
            }
            else
            {
                classType = runtimePropertyType == typeof(object) ? ClassType.Unknown : ClassType.Value;
            }

            Type elementType = null;
            if (((classType & (ClassType.Enumerable | ClassType.IListConstructible | ClassType.Dictionary | ClassType.IDictionaryConstructible)) != 0) ||
                classType == ClassType.Unknown)
            {
                elementType = GetElementType(runtimePropertyType);
            }

            JsonPropertyInfo runtimeProperty = CreateProperty(
                property.DeclaredPropertyType,
                runtimePropertyType,
                property.PropertyInfo,
                parentClassType: Type,
                collectionElementType: elementType,
                classType,
                options: options);
            property.CopyRuntimeSettingsTo(runtimeProperty);

            return runtimeProperty;
        }
    }
}
