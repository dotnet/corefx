// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal partial class JsonClassInfo
    {
        private void AddPolicyProperty(Type propertyType, JsonSerializerOptions options)
        {
            // A policy property is not a real property on a type; instead it leverages the existing converter
            // logic and generic support to avoid boxing. It is used with values types and elements from collections and
            // dictionaries. Typically it would represent a CLR type such as System.String.
            PolicyProperty = AddProperty(
                propertyType,
                propertyInfo: null,        // Not a real property so this is null.
                classType: typeof(object), // A dummy type (not used).
                options: options);
        }

        private JsonPropertyInfo AddProperty(Type propertyType, PropertyInfo propertyInfo, Type classType, JsonSerializerOptions options)
        {
            // If a converter was provided, we should use the provided type as the runtime type and use the converter later.
            JsonConverter converter = options.DetermineConverterForProperty(classType, propertyType, propertyInfo);

            //ClassType classType = ClassType;

            if (converter != null ||
                !(typeof(IEnumerable).IsAssignableFrom(propertyType)) ||
                propertyType == typeof(string))
            {
                return CreateProperty(
                    declaredPropertyType: propertyType,
                    runtimePropertyType: propertyType,
                    propertyInfo,
                    parentClassType: classType,
                    collectionElementType: null,
                    converter,
                    detectCollectionElementType: true,
                    detectConverter: false,
                    options);
            }

            Type elementType;

            if (propertyType.IsArray)
            {
                elementType = propertyType.GetElementType();
                ElementType = elementType;

                return CreateProperty(
                    declaredPropertyType: propertyType,
                    runtimePropertyType: propertyType,
                    propertyInfo,
                    parentClassType: classType,
                    collectionElementType: elementType,
                    converter,
                    detectCollectionElementType: false,
                    detectConverter: false,
                    options);
            }
            // Enumerables and dictionaries.
            else
            {
                GetRuntimeInformation(
                    propertyType,
                    out elementType,
                    out Type runtimeType,
                    out MethodInfo methodInfo);

                AddItemToObject = methodInfo;
                ElementType = elementType;

                return CreateProperty(
                    declaredPropertyType: propertyType,
                    runtimePropertyType: runtimeType,
                    propertyInfo,
                    parentClassType: classType,
                    collectionElementType: elementType,
                    converter,
                    detectCollectionElementType: false,
                    detectConverter: false,
                    options);
            }
        }

        internal static JsonPropertyInfo CreateProperty(
            Type declaredPropertyType,
            Type runtimePropertyType,
            PropertyInfo propertyInfo,
            Type parentClassType,
            Type collectionElementType,
            JsonConverter converter,
            bool detectCollectionElementType,
            bool detectConverter,
            JsonSerializerOptions options)
        {
            bool hasIgnoreAttribute = (JsonPropertyInfo.GetAttribute<JsonIgnoreAttribute>(propertyInfo) != null);
            if (hasIgnoreAttribute)
            {
                return JsonPropertyInfo.CreateIgnoredPropertyPlaceholder(propertyInfo, options);
            }

            ClassType classType;

            if (detectConverter)
            {
                // Caller has not checked for and passed the converter.
                converter = options.DetermineConverterForProperty(parentClassType, runtimePropertyType, propertyInfo);
            }

            if (converter == null)
            {
                classType = GetClassType(runtimePropertyType, checkForConverter: false, options);
            }
            else
            {
                if (runtimePropertyType == typeof(object))
                {
                    classType = ClassType.Unknown;
                }
                else
                {
                    classType = ClassType.Value;
                }
            }

            if (detectCollectionElementType)
            {
                switch (classType)
                {
                    case ClassType.Enumerable:
                    case ClassType.Dictionary:
                    case ClassType.IListConstructible:
                    case ClassType.IDictionaryConstructible:
                    case ClassType.Unknown:
                        collectionElementType = GetElementType(runtimePropertyType);
                        break;
                }
            }

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
                collectionElementType: null,
                converter: null,
                detectCollectionElementType: true,
                detectConverter: true,
                options: options);
        }

        internal JsonPropertyInfo CreatePolymorphicProperty(JsonPropertyInfo property, Type runtimePropertyType, JsonSerializerOptions options)
        {
            JsonPropertyInfo runtimeProperty = CreateProperty(
                property.DeclaredPropertyType,
                runtimePropertyType,
                property.PropertyInfo,
                parentClassType: Type,
                collectionElementType: null,
                converter: null,
                detectCollectionElementType: true,
                detectConverter: true,
                options: options);
            property.CopyRuntimeSettingsTo(runtimeProperty);

            return runtimeProperty;
        }

        private void GetRuntimeInformation(
            Type enumerableType,
            out Type elementType,
            out Type runtimeType,
            out MethodInfo addMethod)
        {
            if (IsDictionary(enumerableType, out elementType, out runtimeType))
            {
                addMethod = default;
                return;
            }

            Debug.Assert(typeof(IEnumerable).IsAssignableFrom(enumerableType));

            if (CanPopulateEnumerableWithoutReflection(
                queryType: enumerableType,
                originalType: enumerableType,
                out elementType,
                out runtimeType))
            {
                addMethod = default;
                return;
            }

            addMethod = FindAddMethod(enumerableType);

            if (addMethod == null)
            {
                elementType = GetElementType(enumerableType);
                Debug.Assert(elementType != null);

                runtimeType = GetRuntimeType(enumerableType, elementType);

                if (runtimeType == enumerableType)
                {
                    addMethod = default;
                    return;
                }

                addMethod = FindAddMethod(runtimeType);
                Debug.Assert(addMethod != null);
            }
            else
            {
                Debug.Assert(addMethod.GetParameters().Length == 1);

                elementType = addMethod.GetParameters()[0].ParameterType;
                runtimeType = GetRuntimeType(enumerableType, elementType);
            }
        }

        private static bool IsDictionary(Type type, out Type elementType, out Type runtimeType)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)))
            {
                Type[] genericTypes = type.GetGenericArguments();
                elementType = genericTypes[1];

                runtimeType = typeof(Dictionary<,>).MakeGenericType(genericTypes[0], genericTypes[1]);
                return true;
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType)
                {
                    continue;
                }

                Type genericDef = @interface.GetGenericTypeDefinition();
                if (genericDef == typeof(IDictionary<,>) || genericDef == typeof(IReadOnlyDictionary<,>))
                {
                    Type[] genericTypes = @interface.GetGenericArguments();
                    elementType = genericTypes[1];

                    Type concreteDictionaryType = typeof(Dictionary<,>).MakeGenericType(genericTypes[0], genericTypes[1]);

                    if (type.IsAssignableFrom(concreteDictionaryType))
                    {
                        runtimeType = concreteDictionaryType;
                    }
                    else
                    {
                        runtimeType = type;
                    }

                    return true;
                }
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                elementType = typeof(object);

                Type concreteDictionaryType = typeof(Dictionary<string, object>);

                if (type.IsAssignableFrom(concreteDictionaryType))
                {
                    runtimeType = concreteDictionaryType;
                }
                else
                {
                    runtimeType = type;
                }

                return true;
            }

            elementType = default;
            runtimeType = default;
            return false;
        }

        private static bool CanPopulateEnumerableWithoutReflection(Type queryType, Type originalType, out Type elementType, out Type runtimeType)
        {
            if (queryType.IsGenericType)
            {
                elementType = queryType.GetGenericArguments()[0];

                // Fast path for List<>
                Type genericDef = queryType.GetGenericTypeDefinition();
                if (genericDef == typeof(List<>))
                {
                    runtimeType = originalType;
                    return true;
                }

                Type concreteListType = typeof(List<>).MakeGenericType(elementType);
                if (queryType.IsAssignableFrom(concreteListType))
                {
                    runtimeType = concreteListType;
                    return true;
                }

                Type genericICollectionType = typeof(ICollection<>).MakeGenericType(elementType);
                if (!queryType.IsInterface && genericICollectionType.IsAssignableFrom(queryType))
                {
                    runtimeType = originalType;
                    return true;
                }

                if (genericDef == typeof(Stack<>) || genericDef == typeof(Queue<>))
                {
                    runtimeType = originalType;
                    return true;
                }
            }
            else
            {
                elementType = typeof(object);

                Type concreteListType = typeof(List<>).MakeGenericType(elementType);
                if (queryType.IsAssignableFrom(concreteListType))
                {
                    runtimeType = concreteListType;
                    return true;
                }

                Type genericICollectionType = typeof(ICollection<>).MakeGenericType(elementType);
                if (!queryType.IsInterface && genericICollectionType.IsAssignableFrom(queryType))
                {
                    runtimeType = originalType;
                    return true;
                }
            }

            foreach (Type @interface in queryType.GetInterfaces())
            {
                if (!@interface.IsGenericType)
                {
                    continue;
                }

                Type interfaceGenericDef = @interface.GetGenericTypeDefinition();
                if (interfaceGenericDef == typeof(IEnumerable<>))
                {
                    Type previouslyDetectedElementType = elementType;
                    elementType = @interface.GetGenericArguments()[0];

                    if (elementType != previouslyDetectedElementType)
                    {
                        Type concreteListType = typeof(List<>).MakeGenericType(elementType);
                        if (queryType.IsAssignableFrom(concreteListType))
                        {
                            runtimeType = concreteListType;
                            return true;
                        }

                        Type genericICollectionType = typeof(ICollection<>).MakeGenericType(elementType);
                        if (!queryType.IsInterface && genericICollectionType.IsAssignableFrom(queryType))
                        {
                            runtimeType = originalType;
                            return true;
                        }
                    }

                    break;
                }
            }

            Type baseType = queryType.BaseType;
            if (baseType != null && baseType != typeof(object))
            {
                // Detertime if we can populate this derived type without reflection.
                return CanPopulateEnumerableWithoutReflection(baseType, queryType, out elementType, out runtimeType);
            }

            elementType = default;
            runtimeType = default;
            return false;
        }

        private MethodInfo FindAddMethod(Type enumerableType)
        {
            foreach (MethodInfo method in enumerableType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.Name == "Add" || method.Name == "Push" || method.Name == "Enqueue")
                {
                    ParameterInfo[] @params = method.GetParameters();
                    int paramLength = @params.Length;

                    if (paramLength == 1)
                    {
                        return method;
                    }
                }
            }

            // Check implemented interfaces (needed for types like LinkedList).
            foreach (Type @interface in enumerableType.GetInterfaces())
            {
                MethodInfo addMethod = FindAddMethod(@interface);
                if (addMethod != default)
                {
                    return addMethod;
                }
            }

            return default;
        }

        private Type GetRuntimeType(Type collectionType, Type elementType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(elementType);
            if (collectionType.IsAssignableFrom(genericListType))
            {
                return genericListType;
            }

            Type hashSetType = typeof(HashSet<>).MakeGenericType(elementType);
            if (collectionType.IsAssignableFrom(hashSetType))
            {
                return hashSetType;
            }

            Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(typeof(string), elementType);
            if (collectionType.IsAssignableFrom(dictionaryType))
            {
                return dictionaryType;
            }

            return collectionType;
        }
    }
}
