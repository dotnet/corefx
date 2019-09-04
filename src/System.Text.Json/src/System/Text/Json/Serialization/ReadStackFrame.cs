// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    [DebuggerDisplay("ClassType.{JsonClassInfo.ClassType}, {JsonClassInfo.Type.Name}")]
    internal struct ReadStackFrame
    {
        // The object (POCO or IEnumerable) that is being populated
        public object ReturnValue;
        public JsonClassInfo JsonClassInfo;

        // Support Dictionary keys.
        public string KeyName;

        // Support JSON Path on exceptions.
        public byte[] JsonPropertyName;

        // Current property values.
        public JsonPropertyInfo JsonPropertyInfo;

        // Support System.Array and other types that don't implement IList.
        public IList TempEnumerableValues;

        // Has an array or dictionary property been initialized.
        public bool CollectionPropertyInitialized;

        // Support IDictionary constructible types, i.e. types that we
        // support by passing and IDictionary to their constructors:
        // immutable dictionaries, Hashtable, SortedList
        public IDictionary TempDictionaryValues;

        // For performance, we order the properties by the first deserialize and PropertyIndex helps find the right slot quicker.
        public int PropertyIndex;
        public List<PropertyRef> PropertyRefCache;

        // The current JSON data for a property does not match a given POCO, so ignore the property (recursively).
        public bool Drain;

        public bool IsCollectionForClass => IsEnumerable || IsICollectionConstructible || IsDictionary || IsIDictionaryConstructible;
        public bool IsCollectionForProperty => IsEnumerableProperty || IsICollectionConstructibleProperty || IsDictionaryProperty || IsIDictionaryConstructibleProperty;

        public bool IsIDictionaryConstructible => JsonClassInfo.ClassType == ClassType.IDictionaryConstructible;
        public bool IsDictionary => JsonClassInfo.ClassType == ClassType.Dictionary;

        public bool IsDictionaryProperty => JsonPropertyInfo != null &&
            !JsonPropertyInfo.IsPropertyPolicy &&
            JsonPropertyInfo.ClassType == ClassType.Dictionary;
        public bool IsIDictionaryConstructibleProperty => JsonPropertyInfo != null &&
            !JsonPropertyInfo.IsPropertyPolicy && (JsonPropertyInfo.ClassType == ClassType.IDictionaryConstructible);

        public bool IsICollectionConstructible => JsonClassInfo.ClassType == ClassType.ICollectionConstructible;
        public bool IsEnumerable => JsonClassInfo.ClassType == ClassType.Enumerable;

        public bool IsEnumerableProperty =>
            JsonPropertyInfo != null &&
            !JsonPropertyInfo.IsPropertyPolicy &&
            JsonPropertyInfo.ClassType == ClassType.Enumerable;
        public bool IsICollectionConstructibleProperty => JsonPropertyInfo != null &&
            !JsonPropertyInfo.IsPropertyPolicy && (JsonPropertyInfo.ClassType == ClassType.ICollectionConstructible);

        public bool IsProcessingEnumerableOrDictionary => IsProcessingEnumerable || IsProcessingICollectionConstructible || IsProcessingDictionary || IsProcessingIDictionaryConstructible;
        public bool IsProcessingDictionary => IsDictionary || IsDictionaryProperty;
        public bool IsProcessingIDictionaryConstructible => IsIDictionaryConstructible || IsIDictionaryConstructibleProperty;
        public bool IsProcessingEnumerable => IsEnumerable || IsEnumerableProperty;
        public bool IsProcessingICollectionConstructible => IsICollectionConstructible || IsICollectionConstructibleProperty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Determine whether a StartObject or StartArray token should be treated as a value.
        public bool IsProcessingValue()
        {
            if (SkipProperty)
            {
                return false;
            }

            ClassType classType;

            if (CollectionPropertyInitialized)
            {
                classType = JsonPropertyInfo.CollectionElementClassInfo.ClassType;
            }
            else if (JsonPropertyInfo == null)
            {
                classType = JsonClassInfo.ClassType;
            }
            else
            {
                classType = JsonPropertyInfo.ClassType;
            }

            return classType == ClassType.Value || classType == ClassType.Unknown;
        }

        public void Initialize(Type type, JsonSerializerOptions options)
        {
            JsonClassInfo = options.GetOrAddClass(type);
            InitializeJsonPropertyInfo();
        }

        public void InitializeJsonPropertyInfo()
        {
            if (JsonClassInfo.ClassType == ClassType.Value ||
                JsonClassInfo.ClassType == ClassType.Enumerable ||
                JsonClassInfo.ClassType == ClassType.ICollectionConstructible ||
                JsonClassInfo.ClassType == ClassType.Dictionary ||
                JsonClassInfo.ClassType == ClassType.IDictionaryConstructible)
            {
                JsonPropertyInfo = JsonClassInfo.PolicyProperty;
            }
        }

        public void Reset()
        {
            Drain = false;
            JsonClassInfo = null;
            PropertyRefCache = null;
            ReturnValue = null;
            EndObject();
        }

        public void EndObject()
        {
            PropertyIndex = 0;
            EndProperty();
        }

        public void EndProperty()
        {
            CollectionPropertyInitialized = false;
            JsonPropertyInfo = null;
            TempEnumerableValues = null;
            TempDictionaryValues = null;
            JsonPropertyName = null;
            KeyName = null;
        }

        public Type GetElementType()
        {
            if (IsCollectionForProperty)
            {
                return JsonPropertyInfo.CollectionElementClassInfo.Type;
            }

            if (IsCollectionForClass)
            {
                return JsonClassInfo.ElementClassInfo.Type;
            }

            return JsonPropertyInfo.RuntimePropertyType;
        }

        public void SetReturnValue(object value)
        {
            Debug.Assert(ReturnValue == null);
            ReturnValue = value;
        }

        public bool SkipProperty => Drain ||
            ReferenceEquals(JsonPropertyInfo, JsonPropertyInfo.s_missingProperty) ||
            (JsonPropertyInfo?.IsPropertyPolicy == false && JsonPropertyInfo?.ShouldDeserialize == false);
    }
}
