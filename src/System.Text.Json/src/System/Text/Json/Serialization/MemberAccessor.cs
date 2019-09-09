// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    internal abstract class MemberAccessor
    {
        public abstract JsonClassInfo.ConstructorDelegate CreateConstructor(Type classType);

        public abstract JsonEnumerableConverterState.CollectionBuilderConstructorDelegate CreateCollectionBuilderConstructor(Type collectionType);

        public abstract JsonEnumerableConverterState.WrappedEnumerableFactoryConstructorDelegate CreateWrappedEnumerableFactoryConstructor(Type collectionType, Type sourceListType);

        public abstract JsonEnumerableConverterState.EnumerableConstructorDelegate<TSourceList> CreateEnumerableConstructor<TCollection, TSourceList>()
            where TCollection : IEnumerable
            where TSourceList : IEnumerable;

        public abstract JsonDictionaryConverterState.DictionaryBuilderConstructorDelegate CreateDictionaryBuilderConstructor(Type dictionaryType);

        public abstract JsonDictionaryConverterState.WrappedDictionaryFactoryConstructorDelegate CreateWrappedDictionaryFactoryConstructor(Type dictionaryType, Type sourceDictionaryType);

        public abstract JsonDictionaryConverterState.DictionaryConstructorDelegate<TSourceDictionary> CreateDictionaryConstructor<TDictionary, TSourceDictionary>()
            where TDictionary : IDictionary
            where TSourceDictionary : IDictionary;

        public abstract ImmutableCollectionCreator ImmutableCollectionCreateRange(Type constructingType, Type collectionType, Type elementType);

        public abstract ImmutableCollectionCreator ImmutableDictionaryCreateRange(Type constructingType, Type collectionType, Type elementType);

        protected MethodInfo ImmutableCollectionCreateRangeMethod(Type constructingType, Type elementType)
        {
            MethodInfo createRangeMethod = FindImmutableCreateRangeMethod(constructingType);

            if (createRangeMethod == null)
            {
                return null;
            }

            return createRangeMethod.MakeGenericMethod(elementType);
        }

        protected MethodInfo ImmutableDictionaryCreateRangeMethod(Type constructingType, Type elementType)
        {
            MethodInfo createRangeMethod = FindImmutableCreateRangeMethod(constructingType);

            if (createRangeMethod == null)
            {
                return null;
            }

            return createRangeMethod.MakeGenericMethod(typeof(string), elementType);
        }

        private MethodInfo FindImmutableCreateRangeMethod(Type constructingType)
        {
            MethodInfo[] constructingTypeMethods = constructingType.GetMethods();

            foreach (MethodInfo method in constructingTypeMethods)
            {
                if (method.Name == "CreateRange" && method.GetParameters().Length == 1)
                {
                    return method;
                }
            }

            // This shouldn't happen because constructingType should be an immutable type with
            // a CreateRange method. `null` being returned here will cause a JsonException to be
            // thrown when the desired CreateRange delegate is about to be invoked.
            Debug.Fail("Could not create the appropriate CreateRange method.");
            return null;
        }

        public abstract Func<object, TProperty> CreatePropertyGetter<TClass, TProperty>(PropertyInfo propertyInfo);

        public abstract Action<object, TProperty> CreatePropertySetter<TClass, TProperty>(PropertyInfo propertyInfo);
    }
}
