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
        public delegate CollectionBuilder CollectionBuilderConstructorDelegate(object instance);
        public delegate WrappedEnumerableFactory WrappedEnumerableFactoryConstructorDelegate(JsonSerializerOptions options);
        public delegate object EnumerableConstructorDelegate<TSourceList>(TSourceList sourceList) where TSourceList : IEnumerable;

        public abstract class CollectionBuilder
        {
            public abstract object Instance { get; }

            public abstract void Add(object item);
        }

        public sealed class CollectionBuilder<T> : CollectionBuilder
        {
            private readonly ICollection<T> _instance;

            public override object Instance => _instance;

            public CollectionBuilder(object instance)
            {
                Debug.Assert(instance != null && instance is ICollection<T>);
                _instance = (ICollection<T>)instance;
            }

            public override void Add(object item)
            {
                Debug.Assert(item == null || item.GetType() == typeof(T));
                _instance.Add((T)item);
            }
        }

        public abstract class WrappedEnumerableFactory
        {
            public abstract object CreateFromList(IEnumerable sourceList);
        }

        public sealed class WrappedEnumerableFactory<TCollection, TSourceList> : WrappedEnumerableFactory
            where TCollection : IEnumerable
            where TSourceList : IEnumerable
        {
            private readonly EnumerableConstructorDelegate<TSourceList> _ctor;

            public WrappedEnumerableFactory(JsonSerializerOptions options)
            {
                Debug.Assert(options != null);

                _ctor = options.MemberAccessorStrategy.CreateEnumerableConstructor<TCollection, TSourceList>();
            }

            public override object CreateFromList(IEnumerable sourceList)
            {
                Debug.Assert(sourceList != null && sourceList is TSourceList);

                if (_ctor == null)
                {
                    ThrowHelper.ThrowNotSupportedException_DeserializeInstanceConstructorOfTypeNotFound(typeof(TCollection), sourceList.GetType());
                }

                return _ctor((TSourceList)sourceList);
            }
        }

        public IList TemporaryList;
        public IList FinalList;
        public CollectionBuilder FinalCollection;
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

        protected virtual Type ResolveTemporaryListType(JsonPropertyInfo jsonPropertyInfo)
            => typeof(List<>);

        private IList CreateConcreteList(JsonPropertyInfo jsonPropertyInfo, JsonSerializerOptions options)
        {
            Debug.Assert(jsonPropertyInfo?.CollectionElementType != null);

            Type TemporaryListType = ResolveTemporaryListType(jsonPropertyInfo).MakeGenericType(jsonPropertyInfo.CollectionElementType);

            string key = TemporaryListType.FullName;

            if (!s_ctors.TryGetValue(key, out JsonClassInfo.ConstructorDelegate ctor))
            {
                ctor = options.MemberAccessorStrategy.CreateConstructor(TemporaryListType);
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
