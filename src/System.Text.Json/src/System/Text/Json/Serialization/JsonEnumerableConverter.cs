﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonEnumerableConverterState
    {
        public delegate CollectionBuilder CollectionBuilderConstructorDelegate(JsonPropertyInfo source, object instance);
        public delegate WrappedEnumerableFactory WrappedEnumerableFactoryConstructorDelegate(JsonSerializerOptions options);
        public delegate object EnumerableConstructorDelegate<TSourceList>(TSourceList sourceList) where TSourceList : IEnumerable;

        public abstract class CollectionBuilder
        {
            public abstract object Instance { get; }
            public abstract int Count { get; }

            public abstract void Add<TPropertyType>(ref TPropertyType item);
        }

        public sealed class CollectionBuilder<T> : CollectionBuilder
        {
            private readonly ICollection<T> _instance;

            public override object Instance => _instance;
            public override int Count => _instance.Count;

            public CollectionBuilder(JsonPropertyInfo source, object instance)
            {
                Debug.Assert(source != null && instance != null);

                if (instance is ICollection<T> collectionInstance)
                {
                    _instance = collectionInstance;
                    return;
                }

                ThrowHelper.ThrowNotSupportedException_SerializationNotSupportedCollection(
                    source.DeclaredPropertyType,
                    source.ParentClassType,
                    source.PropertyInfo);
            }

            public override void Add<TPropertyType>(ref TPropertyType item)
            {
                Debug.Assert(item == null || typeof(T).IsAssignableFrom(item.GetType()));

                if (item is T typedItem)
                {
                    _instance.Add(typedItem);
                }
                else
                {
                    ((ICollection<TPropertyType>)_instance).Add(item);
                }
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
        public CollectionBuilder Builder;

        public int? Count =>
            FinalList?.Count ??
            Builder?.Count ??
            TemporaryList?.Count;
    }

    internal abstract class JsonTemporaryListConverter : JsonEnumerableConverter
    {
        // Cache concrete list constructors for performance.
        private static readonly ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate>();

        public override void BeginEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState == null);

            state.Current.EnumerableConverterState = new JsonEnumerableConverterState
            {
                TemporaryList = CreateConcreteList(state.Current.JsonPropertyInfo, options)
            };
        }

        public override void AddItemToEnumerable<T>(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options, ref T value)
        {
            IList temporaryList = state.Current.EnumerableConverterState?.TemporaryList;

            if (temporaryList == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(value.GetType(), reader, state.JsonPath());
            }

            if (temporaryList is IList<T> typedList)
            {
                typedList.Add(value);
            }
            else
            {
                temporaryList.Add(value);
            }
        }

        protected virtual Type ResolveTemporaryListType(JsonPropertyInfo jsonPropertyInfo)
            => typeof(List<>);

        private IList CreateConcreteList(JsonPropertyInfo jsonPropertyInfo, JsonSerializerOptions options)
        {
            Debug.Assert(jsonPropertyInfo?.CollectionElementType != null);

            Type temporaryListType = ResolveTemporaryListType(jsonPropertyInfo);
            Type collectionElementType = jsonPropertyInfo.CollectionElementType;

            string key = $"{temporaryListType.FullName}[{collectionElementType.FullName}]";

            JsonClassInfo.ConstructorDelegate ctor = s_ctors.GetOrAdd(key, _ =>
            {
                return options.MemberAccessorStrategy.CreateConstructor(temporaryListType.MakeGenericType(collectionElementType));
            });

            return (IList)ctor();
        }
    }

    internal abstract class JsonEnumerableConverter
    {
        public abstract bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType);
        public abstract Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo);
        public abstract void BeginEnumerable(ref ReadStack state, JsonSerializerOptions options);
        public abstract void AddItemToEnumerable<T>(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options, ref T value);
        public abstract object EndEnumerable(ref ReadStack state, JsonSerializerOptions options);
    }
}
