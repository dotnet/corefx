// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    // This converter returns enumerables in the System.Collections.Immutable namespace.
    internal sealed class DefaultImmutableConverter : JsonEnumerableConverter
    {
        public const string ImmutableNamespace = "System.Collections.Immutable";

        private const string ImmutableListTypeName = "System.Collections.Immutable.ImmutableList";
        private const string ImmutableListGenericTypeName = "System.Collections.Immutable.ImmutableList`1";
        private const string ImmutableListGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableList`1";

        private const string ImmutableStackTypeName = "System.Collections.Immutable.ImmutableStack";
        private const string ImmutableStackGenericTypeName = "System.Collections.Immutable.ImmutableStack`1";
        private const string ImmutableStackGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableStack`1";

        private const string ImmutableQueueTypeName = "System.Collections.Immutable.ImmutableQueue";
        private const string ImmutableQueueGenericTypeName = "System.Collections.Immutable.ImmutableQueue`1";
        private const string ImmutableQueueGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableQueue`1";

        private const string ImmutableSortedSetTypeName = "System.Collections.Immutable.ImmutableSortedSet";
        private const string ImmutableSortedSetGenericTypeName = "System.Collections.Immutable.ImmutableSortedSet`1";

        private const string ImmutableHashSetTypeName = "System.Collections.Immutable.ImmutableHashSet";
        private const string ImmutableHashSetGenericTypeName = "System.Collections.Immutable.ImmutableHashSet`1";
        private const string ImmutableSetGenericInterfaceTypeName = "System.Collections.Immutable.IImmutableSet`1";

        internal delegate object ImmutableCreateRangeDelegate<T>(IEnumerable<T> items);
        internal delegate IEnumerable CreateImmutableCollectionDelegate(string createRangeDelegateKey, IList sourceList);

        private static readonly ConcurrentDictionary<string, CreateImmutableCollectionDelegate> _CreateImmutableCollectionDelegates = new ConcurrentDictionary<string, CreateImmutableCollectionDelegate>();

        private static ConcurrentDictionary<string, object> _createRangeDelegates = new ConcurrentDictionary<string, object>();

        private static readonly ConcurrentDictionary<Type, string> _createRangeDelegateKeys = new ConcurrentDictionary<Type, string>();

        public string GetConstructingTypeName(string immutableCollectionTypeName)
        {
            switch (immutableCollectionTypeName)
            {
                case ImmutableListGenericTypeName:
                case ImmutableListGenericInterfaceTypeName:
                    return ImmutableListTypeName;
                case ImmutableStackGenericTypeName:
                case ImmutableStackGenericInterfaceTypeName:
                    return  ImmutableStackTypeName;
                case ImmutableQueueGenericTypeName:
                case ImmutableQueueGenericInterfaceTypeName:
                    return  ImmutableQueueTypeName;
                case ImmutableSortedSetGenericTypeName:
                    return  ImmutableSortedSetTypeName;
                case ImmutableHashSetGenericTypeName:
                case ImmutableSetGenericInterfaceTypeName:
                    return  ImmutableHashSetTypeName;
                default:
                    Debug.Fail("Unknown immutable: " + immutableCollectionTypeName);
                    return null;
            }
        }

        // This method creates a TElement[] and populates it with the items in the sourceList argument,
        // then uses the createRangeDelegateKey argument to identify the appropriate cached
        // CreateRange<TElement> method to create and return the desired immutable collection type.
        // The method is constructed for each element type with reflection and cached.
        public static IEnumerable CreateImmutableCollectionFromList<TElement>(string createRangeDelegateKey, IList sourceList)
        {
            Array array = CreateArrayFromList(typeof(TElement), sourceList);

            Debug.Assert(_createRangeDelegates.ContainsKey(createRangeDelegateKey));

            IEnumerable immutableCollection = (IEnumerable)((ImmutableCreateRangeDelegate<TElement>)_createRangeDelegates[createRangeDelegateKey]).Invoke((IEnumerable<TElement>)array);
            return immutableCollection;
        }

        public void RegisterImmutableCollectionType(Type immutableCollectionType, Type elementType, JsonSerializerOptions options)
        {
            // If we have registered this immutable collection type:
            if (_createRangeDelegateKeys.ContainsKey(immutableCollectionType))
            {
                // We must have the appropriate ImmutableX.CreateRange<elementType> delegate cached.
                Debug.Assert(_createRangeDelegates.ContainsKey(_createRangeDelegateKeys[immutableCollectionType]));
                return;
            }

            // Use the generic type definition of the immutable collection to determine an appropriate constructing type,
            // i.e. a type that we can invoke the `CreateRange<elementType>` method on, which returns an assignable immutable collection.
            Type underlyingType = immutableCollectionType.GetGenericTypeDefinition();
            string constructingTypeName = GetConstructingTypeName(underlyingType.FullName);

            string elementTypeName = elementType.FullName;

            // Construct a unique identifier for a delegate which will point to the appropiate CreateRange method.
            string createRangeDelegateKey = $"{constructingTypeName}:{elementTypeName}";

            // If we haven't yet created and cached a delegate for the appropriate CreateRange method.
            if(!_createRangeDelegates.ContainsKey(createRangeDelegateKey))
            {
                // Get the constructing type.
                Type constructingType = underlyingType.Assembly.GetType(constructingTypeName);

                // Create a delegate which will point to the CreateRange method.
                object createRangeDelegate = options.ClassMaterializerStrategy.ImmutableCreateRange(constructingType, elementType);
                Debug.Assert(createRangeDelegate != null);

                // Cache the delegate
                _createRangeDelegates.TryAdd(createRangeDelegateKey, createRangeDelegate);
                Debug.Assert(_createRangeDelegates.ContainsKey(createRangeDelegateKey));
            }

            // If we haven't seen this element type before, create a delegate to the above CreateImmutableCollectionFromList<TElement> method.
            if (!_CreateImmutableCollectionDelegates.ContainsKey(elementTypeName))
            {
                CreateImmutableCollectionDelegate createImmutableCollection = options.ClassMaterializerStrategy.CreateImmutableCollection(elementType);
                Debug.Assert(createImmutableCollection != null);

                _CreateImmutableCollectionDelegates.TryAdd(elementTypeName, createImmutableCollection);
                Debug.Assert(_CreateImmutableCollectionDelegates.ContainsKey(elementTypeName));
            }

            // Mark the immutable collection type as registered.
            _createRangeDelegateKeys.TryAdd(immutableCollectionType, createRangeDelegateKey);
            Debug.Assert(_createRangeDelegateKeys.ContainsKey(immutableCollectionType));
        }

        public override IEnumerable CreateFromList(Type immutableCollectionType, Type elementType, IList sourceList)
        {
            Debug.Assert(elementType != null);
            Debug.Assert(_createRangeDelegateKeys.ContainsKey(immutableCollectionType));

            return (IEnumerable)_CreateImmutableCollectionDelegates[elementType.FullName].Invoke(_createRangeDelegateKeys[immutableCollectionType], sourceList);
        }
    }
}
