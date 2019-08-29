// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleStartArray(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;
            if (jsonPropertyInfo == null)
            {
                jsonPropertyInfo = state.Current.JsonClassInfo.CreateRootObject(options);
            }
            else if (state.Current.JsonClassInfo.ClassType == ClassType.Unknown)
            {
                jsonPropertyInfo = state.Current.JsonClassInfo.CreatePolymorphicProperty(jsonPropertyInfo, typeof(object), options);
            }

            // Verify that we have a valid enumerable.
            Type arrayType = jsonPropertyInfo.RuntimePropertyType;
            if (!typeof(IEnumerable).IsAssignableFrom(arrayType))
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(arrayType, reader, state.JsonPath);
            }

            Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

            // A nested json array so push a new stack frame.
            if (state.Current.CollectionPropertyInitialized)
            {
                Type elementType = jsonPropertyInfo.ElementClassInfo.Type;

                state.Push();
                state.Current.Initialize(elementType, options);
                state.Current.CollectionPropertyInitialized = true;

                JsonClassInfo classInfo = state.Current.JsonClassInfo;

                if (state.Current.IsProcessingICollectionConstructible)
                {
                    state.Current.TempEnumerableValues = (IList)classInfo.CreateConcreteEnumerable();
                }
                else
                {
                    if (classInfo.CreateObject == null)
                    {
                        ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(classInfo.Type, reader, state.JsonPath);
                        return;
                    }
                    state.Current.ReturnValue = classInfo.CreateObject();
                }

                return;
            }

            state.Current.CollectionPropertyInitialized = true;

            JsonClassInfo collectionClassInfo;
            if (jsonPropertyInfo.DeclaredPropertyType == jsonPropertyInfo.ImplementedPropertyType)
            {
                collectionClassInfo = options.GetOrAddClass(jsonPropertyInfo.RuntimePropertyType);
            }
            else
            {
                collectionClassInfo = options.GetOrAddClass(jsonPropertyInfo.DeclaredPropertyType);
            }

            if (jsonPropertyInfo.EnumerableConverter != null)
            {
                state.Current.TempEnumerableValues = (IList)collectionClassInfo.CreateConcreteEnumerable();
            }
            else
            {
                object value = collectionClassInfo.CreateObject();

                if (value != null)
                {
                    if (state.Current.ReturnValue != null)
                    {
                        state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                    }
                    else
                    {
                        // Primitive arrays being returned without object.
                        state.Current.SetReturnValue(value);
                    }
                }
            }
        }

        private static void HandleEndArray(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            if (state.Current.IsEnumerableProperty)
            {
                // We added the items to the enumberable already.
                state.Current.EndProperty();
            }
            else if (state.Current.IsICollectionConstructibleProperty)
            {
                Debug.Assert(state.Current.TempEnumerableValues != null);
                JsonEnumerableConverter converter = state.Current.JsonPropertyInfo.EnumerableConverter;
                state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, converter.CreateFromList(ref state, state.Current.TempEnumerableValues, options));
                state.Current.EndProperty();
            }
            else
            {
                object value;
                if (state.Current.TempEnumerableValues != null)
                {
                    JsonEnumerableConverter converter = state.Current.JsonPropertyInfo.EnumerableConverter;
                    value = converter.CreateFromList(ref state, state.Current.TempEnumerableValues, options);
                }
                else
                {
                    value = state.Current.ReturnValue;
                }

                if (state.IsLastFrame)
                {
                    // Set the return value directly since this will be returned to the user.
                    state.Current.Reset();
                    state.Current.ReturnValue = value;
                }
                else
                {
                    state.Pop();
                    ApplyObjectToEnumerable(value, ref state, ref reader);
                }
            }
        }

        // If this method is changed, also change ApplyValueToEnumerable.
        internal static void ApplyObjectToEnumerable(
            object value,
            ref ReadStack state,
            ref Utf8JsonReader reader,
            bool setPropertyDirectly = false)
        {
            Debug.Assert(!state.Current.SkipProperty);

            if (state.Current.TempEnumerableValues != null)
            {
                // Used by nested arrays, arrays with converters, and IsICollectionConstructibles.
                state.Current.TempEnumerableValues.Add(value);
            }
            else if (state.Current.IsEnumerable || (state.Current.IsEnumerableProperty && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.ReturnValue != null);

                IList list = (IList)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                list.Add(value);
            }
            else if (state.Current.IsICollectionConstructible || (state.Current.IsICollectionConstructibleProperty && !setPropertyDirectly))
            {
                // If we didn't fall into the TempEnumerableValues block above, we have an invalid array.
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(value.GetType(), reader, state.JsonPath);
            }
            else if (state.Current.IsDictionary || (state.Current.IsDictionaryProperty && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.ReturnValue != null);
                IDictionary dictionary = (IDictionary)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));
                dictionary[key] = value;
            }
            else if (state.Current.IsIDictionaryConstructible ||
                (state.Current.IsIDictionaryConstructibleProperty && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.TempDictionaryValues != null);
                IDictionary dictionary = state.Current.TempDictionaryValues;

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));
                dictionary[key] = value;
            }
            else
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
            }
        }

        // If this method is changed, also change ApplyObjectToEnumerable.
        internal static void ApplyValueToEnumerable<TProperty>(
            ref TProperty value,
            ref ReadStack state,
            ref Utf8JsonReader reader)
        {
            Debug.Assert(!state.Current.SkipProperty);

            if (state.Current.TempEnumerableValues != null)
            {
                // Used by nested arrays, arrays with converters, and IsICollectionConstructibles.
                state.Current.TempEnumerableValues.Add(value);
            }
            else if (state.Current.IsProcessingEnumerable)
            {
                Debug.Assert(state.Current.ReturnValue != null);

                if (state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue) is ICollection<TProperty> genericCollection)
                {
                    genericCollection.Add(value);
                    return;
                }

                IList list = (IList)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                list.Add(value);
            }
            else if (state.Current.IsProcessingICollectionConstructible)
            {
                // If we didn't fall into the TempEnumerableValues block above, we have an invalid array.
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(value.GetType(), reader, state.JsonPath);
            }
            else if (state.Current.IsProcessingDictionary)
            {
                Debug.Assert(state.Current.ReturnValue != null);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                if (state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue) is IDictionary<string, TProperty> genericDictionary)
                {
                    genericDictionary[key] = value;
                    return;
                }

                IDictionary dictionary = (IDictionary)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                dictionary.Add(key, value);
            }
            else if (state.Current.IsProcessingIDictionaryConstructible)
            {
                Debug.Assert(state.Current.TempDictionaryValues != null);
                IDictionary<string, TProperty> dictionary = (IDictionary<string, TProperty>)state.Current.TempDictionaryValues;

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));
                dictionary[key] = value;
            }
            else
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
            }
        }
    }
}
