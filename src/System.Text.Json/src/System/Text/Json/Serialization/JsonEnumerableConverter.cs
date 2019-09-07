// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonEnumerableConverterState
    {
        public abstract class Collection
        {
            public object Instance;
            public abstract void Add(object item);
        }

        public sealed class Collection<T> : Collection
        {
            public override void Add(object item)
            {
                Debug.Assert(Instance != null &&
                    typeof(ICollection<T>).IsAssignableFrom(Instance.GetType()) &&
                    (item == null || item.GetType() == typeof(T)));
                ((ICollection<T>)Instance).Add((T)item);
            }
        }

        public IList TemporaryList;
        public IList FinalList;
        public object FinalCollection;
        //public Action<object> CollectionAddAction;
    }

    internal abstract class JsonTemporaryListConverter : JsonEnumerableConverter
    {
        // Cache concrete list constructors for performance.
        private static readonly Dictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new Dictionary<string, JsonClassInfo.ConstructorDelegate>();

        public override void BeginEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState == null);

            state.Current.EnumerableConverterState = new JsonEnumerableConverterState
            {
                TemporaryList = CreateConcreteList(state.Current.JsonPropertyInfo, options)
            };
        }

        public override void AddItemToEnumerable(ref ReadStack state, JsonSerializerOptions options, object value)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.TemporaryList != null);

            state.Current.EnumerableConverterState.TemporaryList.Add(value);
        }

        private IList CreateConcreteList(JsonPropertyInfo jsonPropertyInfo, JsonSerializerOptions options)
        {
            Debug.Assert(jsonPropertyInfo?.CollectionElementType != null);

            string key = jsonPropertyInfo.CollectionElementType.FullName;

            if (!s_ctors.TryGetValue(key, out JsonClassInfo.ConstructorDelegate ctor))
            {
                ctor = options.MemberAccessorStrategy.CreateConstructor(typeof(List<>).MakeGenericType(jsonPropertyInfo.CollectionElementType));
                s_ctors[key] = ctor;
            }

            return (IList)ctor();
        }
    }

    internal abstract class JsonEnumerableConverter
    {
        public abstract bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType);
        public abstract Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo);
        public abstract void BeginEnumerable(ref ReadStack state, JsonSerializerOptions options);
        public abstract void AddItemToEnumerable(ref ReadStack state, JsonSerializerOptions options, object value);
        public abstract object EndEnumerable(ref ReadStack state, JsonSerializerOptions options);
    }
}
