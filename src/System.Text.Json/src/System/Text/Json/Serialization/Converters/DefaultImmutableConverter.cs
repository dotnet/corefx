// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
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
        internal delegate IEnumerable CreateImmutableCollectionDelegate(string delegateKey, IList sourceList);

        private static ConcurrentDictionary<string, object> _createRangeDelegates = new ConcurrentDictionary<string, object>();
        private static readonly ConcurrentDictionary<string, CreateImmutableCollectionDelegate> _createImmutableDelegates = new ConcurrentDictionary<string, CreateImmutableCollectionDelegate>();

        private string GetConstructingTypeName(string immutableCollectionTypeName)
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
                    // TODO: Refactor exception throw following serialization exception changes. 
                    throw new NotSupportedException(SR.Format(SR.DeserializeTypeNotSupported, immutableCollectionTypeName));
            }
        }

        private string GetDelegateKey(
            Type immutableCollectionType,
            Type elementType,
            out Type underlyingType,
            out string constructingTypeName,
            out string elementTypeName)
        {
            // Use the generic type definition of the immutable collection to determine an appropriate constructing type,
            // i.e. a type that we can invoke the `CreateRange<elementType>` method on, which returns an assignable immutable collection.
            underlyingType = immutableCollectionType.GetGenericTypeDefinition();
            constructingTypeName = GetConstructingTypeName(underlyingType.FullName);

            elementTypeName = elementType.FullName;

            return $"{constructingTypeName}:{elementTypeName}";
        }

        // This method creates a TElement[] and populates it with the items in the sourceList argument,
        // then uses the createRangeDelegateKey argument to identify the appropriate cached
        // CreateRange<TElement> method to create and return the desired immutable collection type.
        // The method is constructed for each element type with reflection and cached.
        public static IEnumerable CreateImmutableCollectionFromList<TElement>(string delegateKey, IList sourceList)
        {
            // TODO: Cache reflection here, or modify JsonPropertyInfoCommon to have a TElementType generic parameter.
            MethodInfo createIEnumerable = typeof(JsonEnumerableConverter).GetMethod(
                "GetGenericEnumerableFromList",
                BindingFlags.NonPublic | BindingFlags.Static);
            createIEnumerable = createIEnumerable.MakeGenericMethod(typeof(TElement));

            Debug.Assert(_createRangeDelegates.ContainsKey(delegateKey));

            ImmutableCreateRangeDelegate<TElement> createRangeDelegate = (ImmutableCreateRangeDelegate<TElement>)_createRangeDelegates[delegateKey];

            IEnumerable immutableCollection = (IEnumerable)createRangeDelegate.Invoke(
                (IEnumerable<TElement>)createIEnumerable.Invoke(null, new object[] { sourceList }));
            return immutableCollection;
        }

        public void RegisterImmutableCollectionType(Type immutableCollectionType, Type elementType, JsonSerializerOptions options)
        {
            // Get a unique identifier for a delegate which will point to the appropiate CreateRange method.
            string delegateKey = GetDelegateKey(immutableCollectionType, elementType, out Type underlyingType, out string constructingTypeName, out string elementTypeName);

            // If we have registered this immutable collection type:
            if (_createImmutableDelegates.ContainsKey(delegateKey))
            {
                // We must have the appropriate CreateImmutableCollectionFromList<elementType> and
                // ImmutableX.CreateRange<elementType> delegate cached.
                Debug.Assert(_createImmutableDelegates[delegateKey] != null && _createRangeDelegates[elementTypeName] != null);
                return;
            }

            // If we haven't yet created and cached a delegate for the appropriate CreateRange method.
            if(!_createRangeDelegates.ContainsKey(delegateKey))
            {
                // Get the constructing type.
                Type constructingType = underlyingType.Assembly.GetType(constructingTypeName);

                // Create a delegate which will point to the CreateRange method.
                object createRangeDelegate = options.ClassMaterializerStrategy.ImmutableCreateRange(constructingType, elementType);
                Debug.Assert(createRangeDelegate != null);

                // Cache the delegate
                _createRangeDelegates.TryAdd(delegateKey, createRangeDelegate);
            }

            // Create a delegate to the above CreateImmutableCollectionFromList<TElement> method.
            CreateImmutableCollectionDelegate createImmutableCollection = options.ClassMaterializerStrategy.CreateImmutableCollection(elementType);
            Debug.Assert(createImmutableCollection != null);

            _createImmutableDelegates.TryAdd(elementTypeName, createImmutableCollection);
        }

        public override IEnumerable CreateFromList(Type immutableCollectionType, Type elementType, IList sourceList)
        {
            Debug.Assert(immutableCollectionType != null && elementType != null);

            string delegateKey = GetDelegateKey(immutableCollectionType, elementType, out _, out _, out string elementTypeName);

            Debug.Assert(_createRangeDelegates.ContainsKey(delegateKey) && _createImmutableDelegates.ContainsKey(elementTypeName));

            return _createImmutableDelegates[elementTypeName].Invoke(delegateKey, sourceList);
        }
    }
}
