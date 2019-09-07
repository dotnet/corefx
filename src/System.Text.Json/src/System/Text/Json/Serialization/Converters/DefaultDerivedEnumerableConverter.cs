// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultDerivedEnumerableConverter : JsonEnumerableConverter
    {
        // Cache concrete list constructors for performance.
        private static readonly Dictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new Dictionary<string, JsonClassInfo.ConstructorDelegate>();

        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return typeof(IList).IsAssignableFrom(implementedCollectionType) ||
                (implementedCollectionType.IsGenericType && typeof(ICollection<>).MakeGenericType(collectionElementType).IsAssignableFrom(implementedCollectionType));
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            if (jsonPropertyInfo.DeclaredPropertyType.IsInterface)
            {
                if (jsonPropertyInfo.DeclaredPropertyType.IsGenericType)
                {
                    if (typeof(ISet<>).MakeGenericType(jsonPropertyInfo.CollectionElementType).IsAssignableFrom(jsonPropertyInfo.DeclaredPropertyType))
                        return typeof(HashSet<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
                    if (typeof(ICollection<>).MakeGenericType(jsonPropertyInfo.CollectionElementType).IsAssignableFrom(jsonPropertyInfo.DeclaredPropertyType))
                        return typeof(Collection<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
                }
                return typeof(List<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
            }

            return jsonPropertyInfo.DeclaredPropertyType;
        }

        public override void BeginEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState == null);

            JsonClassInfo.ConstructorDelegate ctor = state.Current.JsonPropertyInfo.DeclaredPropertyType.IsInterface
                ? FindCachedCtor(state.Current.JsonPropertyInfo.RuntimePropertyType, options)
                : state.Current.JsonPropertyInfo.DeclaredClassInfo.CreateObject;

            object instance = ctor();

            if (instance is IList list)
            {
                state.Current.EnumerableConverterState = new JsonEnumerableConverterState
                {
                    FinalList = list
                };
            }
            else
            {
                //var c = typeof(ImmutableEnumerableCreator<,>).MakeGenericType(typeof(int), typeof(List<>).MakeGenericType(typeof(int))).GetConstructors();

                //var Test = typeof(JsonEnumerableConverterStateCollection<int>).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null);

                //var c = new JsonEnumerableConverterStateCollection<int>();

                Type collectionType = typeof(JsonEnumerableConverterState.Collection<>).MakeGenericType(state.Current.JsonPropertyInfo.CollectionElementType);

                //var p = Activator.CreateInstance(collectionType);

                JsonEnumerableConverterState.Collection CollectionInstance = (JsonEnumerableConverterState.Collection)FindCachedCtor(
                    collectionType,
                    options)();
                CollectionInstance.Instance = instance;

                /*Type CollectionElementType = state.Current.JsonPropertyInfo.CollectionElementType;

                MethodInfo AddMethod = instance
                    .GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m =>
                    {
                        ParameterInfo[] Parameters = m.GetParameters();
                        return (m.Name == "Add" || m.Name == "System.Collections.Generic.ICollection<T>.Add") &&
                            m.ReturnType == typeof(void) &&
                            Parameters.Length == 1 &&
                            Parameters[0].ParameterType == CollectionElementType;
                    });*/

                state.Current.EnumerableConverterState = new JsonEnumerableConverterState
                {
                    FinalCollection = instance,
                    //CollectionAddAction = (Action<object>)AddMethod.CreateDelegate(typeof(Action<object>))
                };
            }
        }

        public override void AddItemToEnumerable(ref ReadStack state, JsonSerializerOptions options, object value)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.FinalList != null ||
                state.Current.EnumerableConverterState.FinalCollection != null);

            if (state.Current.EnumerableConverterState.FinalList != null)
            {
                state.Current.EnumerableConverterState.FinalList.Add(value);
            }
            else
            {
                //state.Current.EnumerableConverterState.CollectionAddAction(value);
            }
        }

        public override object EndEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.FinalList != null ||
                state.Current.EnumerableConverterState.FinalCollection != null);

            return state.Current.EnumerableConverterState.FinalList ?? state.Current.EnumerableConverterState.FinalCollection;
        }

        private JsonClassInfo.ConstructorDelegate FindCachedCtor(Type runtimePropertyType, JsonSerializerOptions options)
        {
            string key = runtimePropertyType.FullName;

            if (!s_ctors.TryGetValue(key, out JsonClassInfo.ConstructorDelegate ctor))
            {
                ctor = options.MemberAccessorStrategy.CreateConstructor(runtimePropertyType);
                s_ctors[key] = ctor;
            }

            return ctor;
        }
    }
}
