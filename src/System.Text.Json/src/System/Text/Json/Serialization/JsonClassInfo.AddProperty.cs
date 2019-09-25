// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal partial class JsonClassInfo
    {
        private void AddPolicyProperty(ClassType propertyClassType, Type propertyType, Type implementedCollectionType, JsonConverter converter, JsonSerializerOptions options)
        {
            // A policy property is not a real property on a type; instead it leverages the existing converter
            // logic and generic support to avoid boxing. It is used with values types and elements from collections and
            // dictionaries. Typically it would represent a CLR type such as System.String.
            PolicyProperty = AddProperty(
                propertyClassType: propertyClassType,
                parentClassType: typeof(object), // A dummy type (not used).
                propertyType: propertyType,
                propertyInfo: null,        // Not a real property so this is null.
                implementedCollectionType: implementedCollectionType,
                converter: converter,
                options: options);
        }

        private JsonPropertyInfo AddProperty(Type parentClassType, Type propertyType, PropertyInfo propertyInfo, JsonSerializerOptions options)
        {
            // Get implemented type, if applicable.
            // Will return the propertyType itself if it's a non-enumerable, string, natively supported collection,
            // or if a custom converter has been provided for the type.
            Type implementedCollectionType = GetImplementedCollectionType(parentClassType, propertyType, propertyInfo, out JsonConverter converter, options);

            ClassType classType = GetClassType(propertyType, implementedCollectionType, options);

            return AddProperty(
                propertyClassType: classType,
                parentClassType: parentClassType,
                propertyType: propertyType,
                propertyInfo: propertyInfo,
                implementedCollectionType: implementedCollectionType,
                converter: converter,
                options: options);
        }

        private JsonPropertyInfo AddProperty(ClassType propertyClassType, Type parentClassType, Type propertyType, PropertyInfo propertyInfo, Type implementedCollectionType, JsonConverter converter, JsonSerializerOptions options)
        {
            bool hasIgnoreAttribute = JsonPropertyInfo.GetAttribute<JsonIgnoreAttribute>(propertyInfo) != null;
            if (hasIgnoreAttribute)
            {
                return JsonPropertyInfo.CreateIgnoredPropertyPlaceholder(propertyInfo, options);
            }

            Type collectionElementType = null;
            switch (propertyClassType)
            {
                case ClassType.Enumerable:
                case ClassType.Dictionary:
                case ClassType.Unknown:
                    collectionElementType = GetElementType(propertyClassType, propertyType, implementedCollectionType, parentClassType, propertyInfo);
                    break;
            }

            return CreateProperty(propertyClassType, propertyType, implementedCollectionType, collectionElementType, propertyInfo, parentClassType, converter, options);
        }

        private static JsonPropertyInfo CreateProperty(
            ClassType propertyClassType,
            Type declaredPropertyType,
            Type implementedCollectionType,
            Type collectionElementType,
            PropertyInfo propertyInfo,
            Type parentClassType,
            JsonConverter converter,
            JsonSerializerOptions options,
            Type runtimePropertyType = null)
        {
            // Create the JsonPropertyInfo<TType, TProperty>
            Type propertyInfoClassType;
            if (declaredPropertyType.IsGenericType && declaredPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // First try to find a converter for the Nullable, then if not found use the underlying type.
                // This supports custom converters that want to (de)serialize as null when the value is not null.
                if (converter == null)
                {
                    converter = options.DetermineConverterForProperty(parentClassType, declaredPropertyType, propertyInfo);
                }

                if (converter != null)
                {
                    propertyInfoClassType = typeof(JsonPropertyInfoNotNullable<,,>).MakeGenericType(
                        parentClassType,
                        declaredPropertyType,
                        declaredPropertyType);
                }
                else
                {
                    Type typeToConvert = Nullable.GetUnderlyingType(declaredPropertyType);
                    converter = options.DetermineConverterForProperty(parentClassType, typeToConvert, propertyInfo);
                    propertyInfoClassType = typeof(JsonPropertyInfoNullable<,>).MakeGenericType(parentClassType, typeToConvert);
                }
            }
            else
            {
                if (converter == null)
                {
                    converter = options.DetermineConverterForProperty(parentClassType, declaredPropertyType, propertyInfo);
                }

                Type typeToConvert = converter?.TypeToConvert;
                if (typeToConvert == null)
                {
                    if (IsNativelySupportedCollection(declaredPropertyType))
                    {
                        typeToConvert = implementedCollectionType;
                    }
                    else
                    {
                        typeToConvert = declaredPropertyType;
                    }
                }

                // For the covariant case, create JsonPropertyInfoNotNullable. The generic constraints are "where TConverter : TDeclaredProperty".
                if (declaredPropertyType.IsAssignableFrom(typeToConvert))
                {
                    propertyInfoClassType = typeof(JsonPropertyInfoNotNullable<,,>).MakeGenericType(
                        parentClassType,
                        declaredPropertyType,
                        typeToConvert);
                }
                else
                {
                    Debug.Assert(typeToConvert.IsAssignableFrom(declaredPropertyType));

                    // For the contravariant case, create JsonPropertyInfoNotNullableContravariant. The generic constraints are "where TDeclaredProperty : TConverter".
                    propertyInfoClassType = typeof(JsonPropertyInfoNotNullableContravariant<,,>).MakeGenericType(
                        parentClassType,
                        declaredPropertyType,
                        typeToConvert);
                }
            }

            JsonPropertyInfo jsonInfo = (JsonPropertyInfo)Activator.CreateInstance(
                propertyInfoClassType,
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null);

            jsonInfo.Initialize(propertyClassType, parentClassType, declaredPropertyType, runtimePropertyType, implementedCollectionType, collectionElementType, propertyInfo, converter, options);

            return jsonInfo;
        }

        internal JsonPropertyInfo CreateRootObject(JsonSerializerOptions options)
        {
            return CreateProperty(
                ClassType.Object,
                declaredPropertyType: Type,
                implementedCollectionType: Type,
                collectionElementType: null,
                propertyInfo: null,
                parentClassType: Type,
                converter: null,
                options: options);
        }

        internal JsonPropertyInfo CreatePolymorphicProperty(JsonPropertyInfo property, Type runtimePropertyType, JsonSerializerOptions options)
        {
            JsonPropertyInfo runtimeProperty = CreateProperty(
                property.ClassType,
                property.DeclaredPropertyType,
                property.ImplementedCollectionPropertyType,
                property.CollectionElementType,
                property.PropertyInfo,
                parentClassType: Type,
                converter: null,
                options: options,
                runtimePropertyType: runtimePropertyType);

            property.CopyRuntimeSettingsTo(runtimeProperty);

            return runtimeProperty;
        }
    }
}
