// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonDictionaryConverterState
    {
        public delegate DictionaryBuilder DictionaryBuilderConstructorDelegate(object instance);
        public delegate WrappedDictionaryFactory WrappedDictionaryFactoryConstructorDelegate(JsonSerializerOptions options);
        public delegate object DictionaryConstructorDelegate<TSourceDictionary>(TSourceDictionary sourceDictionary) where TSourceDictionary : IDictionary;

        public abstract class DictionaryBuilder
        {
            public abstract object Instance { get; }
            public abstract int Count { get; }

            public abstract void Add<TPropertyType>(string key, ref TPropertyType item);
        }

        public sealed class DictionaryBuilder<T> : DictionaryBuilder
        {
            private readonly IDictionary<string, T> _instance;

            public override object Instance => _instance;
            public override int Count => _instance.Count;

            public DictionaryBuilder(object instance)
            {
                Debug.Assert(instance != null && instance is IDictionary<string, T>);
                _instance = (IDictionary<string, T>)instance;
            }

            public override void Add<TPropertyType>(string key, ref TPropertyType item)
            {
                Debug.Assert(!string.IsNullOrEmpty(key));
                Debug.Assert(item == null || typeof(T).IsAssignableFrom(item.GetType()));

                if (item is T typedItem)
                {
                    _instance[key] = typedItem;
                }
                else if (item == null)
                {
                    // Handle null values for nullable types.
                    _instance[key] = default;
                }
                else
                {
                    ((IDictionary<string, TPropertyType>)_instance)[key] = item;
                }
            }
        }

        public abstract class WrappedDictionaryFactory
        {
            public abstract object CreateFromDictionary(IDictionary sourceDictionary);
        }

        public sealed class WrappedDictionaryFactory<TDictionary, TSourceDictionary> : WrappedDictionaryFactory
            where TDictionary : IEnumerable
            where TSourceDictionary : IDictionary
        {
            private readonly DictionaryConstructorDelegate<TSourceDictionary> _ctor;

            public WrappedDictionaryFactory(JsonSerializerOptions options)
            {
                Debug.Assert(options != null);

                _ctor = options.MemberAccessorStrategy.CreateDictionaryConstructor<TDictionary, TSourceDictionary>();
            }

            public override object CreateFromDictionary(IDictionary sourceDictionary)
            {
                Debug.Assert(sourceDictionary != null && sourceDictionary is TSourceDictionary);

                if (_ctor == null)
                {
                    ThrowHelper.ThrowNotSupportedException_DeserializeInstanceConstructorOfTypeNotFound(typeof(TDictionary), sourceDictionary.GetType());
                }

                return _ctor((TSourceDictionary)sourceDictionary);
            }
        }

        public IDictionary TemporaryDictionary;
        public IDictionary FinalDictionary;
        public DictionaryBuilder Builder;

        public int? Count =>
            FinalDictionary?.Count ??
            Builder?.Count ??
            TemporaryDictionary?.Count;
    }

    internal abstract class JsonTemporaryDictionaryConverter : JsonDictionaryConverter
    {
        // Cache concrete dictionary constructors for performance.
        private static readonly ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate> s_ctors = new ConcurrentDictionary<string, JsonClassInfo.ConstructorDelegate>();

        public override void BeginDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState == null);

            state.Current.DictionaryConverterState = new JsonDictionaryConverterState
            {
                TemporaryDictionary = CreateConcreteDictionary(state.Current.JsonPropertyInfo, options)
            };
        }

        public override void AddItemToDictionary<T>(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options, string key, ref T value)
        {
            IDictionary temporaryDictionary = state.Current.DictionaryConverterState?.TemporaryDictionary;

            if (temporaryDictionary == null)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(value.GetType(), reader, state.JsonPath());
            }

            if (temporaryDictionary is IDictionary<string, T> typedDictionary)
            {
                typedDictionary[key] = value;
            }
            else
            {
                temporaryDictionary[key] = value;
            }
        }

        protected virtual Type ResolveTemporaryDictionaryType(JsonPropertyInfo jsonPropertyInfo)
            => typeof(Dictionary<,>);

        private IDictionary CreateConcreteDictionary(JsonPropertyInfo jsonPropertyInfo, JsonSerializerOptions options)
        {
            Debug.Assert(jsonPropertyInfo?.CollectionElementType != null);

            Type temporaryDictionaryType = ResolveTemporaryDictionaryType(jsonPropertyInfo);
            Type collectionElementType = jsonPropertyInfo.CollectionElementType;

            string key = $"{temporaryDictionaryType.FullName}[{collectionElementType.FullName}]";

            JsonClassInfo.ConstructorDelegate ctor = s_ctors.GetOrAdd(key, _ =>
            {
                return options.MemberAccessorStrategy.CreateConstructor(temporaryDictionaryType.MakeGenericType(typeof(string), collectionElementType));
            });

            return (IDictionary)ctor();
        }
    }

    // Helper to deserialize data into collections that store key-value pairs (not including KeyValuePair<,>)
    // e.g. IDictionary, Hashtable, Dictionary<,> IDictionary<,>, SortedList etc.
    // We'll call these collections "dictionaries".
    // Note: the KeyValuePair<,> type has a value converter, so its deserialization flow will not reach here.
    // Also, KeyValuePair<,> is sealed, so deserialization will flow here to support custom types that
    // implement KeyValuePair<,>.
    internal abstract class JsonDictionaryConverter
    {
        public abstract bool OwnsImplementedCollectionType(Type declaredPropertyType, Type implementedCollectionType, Type collectionElementType);
        public abstract Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo);
        public abstract void BeginDictionary(ref ReadStack state, JsonSerializerOptions options);
        public abstract void AddItemToDictionary<T>(ref Utf8JsonReader reader, ref ReadStack state, JsonSerializerOptions options, string key, ref T value);
        public abstract object EndDictionary(ref ReadStack state, JsonSerializerOptions options);
    }
}
