﻿// Licensed to the .NET Foundation under one or more agreements.
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
            if (state.Current.SkipProperty)
            {
                // The array is not being applied to the object.
                state.Push();
                state.Current.Drain = true;
                return;
            }

            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;
            if (jsonPropertyInfo == null)
            {
                jsonPropertyInfo = state.Current.JsonClassInfo.CreateRootObject(options);
            }

            // Verify that we are processing a valid enumerable or dictionary.
            if (((ClassType.Enumerable | ClassType.Dictionary | ClassType.IDictionaryConstructible) & jsonPropertyInfo.ClassType) == 0)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(jsonPropertyInfo.RuntimePropertyType);
            }

            if (state.Current.CollectionPropertyInitialized)
            {
                // An array nested in a dictionary or array, so push a new stack frame.
                Type elementType = jsonPropertyInfo.ElementClassInfo.Type;

                state.Push();
                state.Current.Initialize(elementType, options);
            }

            state.Current.CollectionPropertyInitialized = true;

            // The current JsonPropertyInfo will be null if the current type is not one of
            // ClassType.Value | ClassType.Enumerable | ClassType.Dictionary.
            // We should not see ClassType.Value here because we handle it on a different code
            // path invoked in the main read loop.
            // Only ClassType.Enumerable is valid here since we just saw a StartArray token.
            if (state.Current.JsonPropertyInfo == null ||
                state.Current.JsonPropertyInfo.ClassType != ClassType.Enumerable)
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(state.Current.JsonClassInfo.Type);
            }

            // Set or replace the existing enumerable value.
            object value = ReadStackFrame.CreateEnumerableValue(ref reader, ref state);

            // If value is not null, then we don't have a converter so apply the value.
            if (value != null)
            {
                if (state.Current.ReturnValue != null)
                {
                    state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                }
                else
                {
                    // Primitive arrays being returned without object
                    state.Current.SetReturnValue(value);
                }
            }
        }

        private static bool HandleEndArray(
            JsonSerializerOptions options,
            ref ReadStack state)
        {
            bool lastFrame = state.IsLastFrame;

            if (state.Current.Drain)
            {
                // The array is not being applied to the object.
                state.Pop();
                return lastFrame;
            }

            IEnumerable value = ReadStackFrame.GetEnumerableValue(state.Current);

            if (state.Current.TempEnumerableValues != null)
            {
                // We have a converter; possibilities:
                // - Add value to current frame's current property or TempEnumerableValues.
                // - Add value to previous frame's current property or TempEnumerableValues.
                // - Set current property on current frame to value.
                // - Set current property on previous frame to value.
                // - Set ReturnValue if root frame and value is the actual return value.
                JsonEnumerableConverter converter = state.Current.JsonPropertyInfo.EnumerableConverter;
                Debug.Assert(converter != null);

                value = converter.CreateFromList(ref state, (IList)value, options);
                state.Current.TempEnumerableValues = null;
            }
            else if (state.Current.IsProcessingProperty(ClassType.Enumerable))
            {
                // We added the items to the list already.
                state.Current.EndProperty();
                return false;
            }

            if (lastFrame)
            {
                if (state.Current.ReturnValue == null)
                {
                    // Returning a converted list or object.
                    state.Current.Reset();
                    state.Current.ReturnValue = value;
                    return true;
                }
                else if (state.Current.IsProcessingCollectionObject())
                {
                    // Returning a non-converted list.
                    return true;
                }
                // else there must be an outer object, so we'll return false here.
            }
            else if (state.Current.IsProcessingObject(ClassType.Enumerable))
            {
                state.Pop();
            }

            ApplyObjectToEnumerable(value, ref state);

            return false;
        }

        // If this method is changed, also change ApplyValueToEnumerable.
        internal static void ApplyObjectToEnumerable(
            object value,
            ref ReadStack state,
            bool setPropertyDirectly = false)
        {
            Debug.Assert(!state.Current.SkipProperty);

            if (state.Current.IsProcessingObject(ClassType.Enumerable))
            {
                if (state.Current.TempEnumerableValues != null)
                {
                    state.Current.TempEnumerableValues.Add(value);
                }
                else
                {
                    if (!(state.Current.ReturnValue is IList list))
                    {
                        ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(value.GetType());
                        return;
                    }
                    list.Add(value);
                }
            }
            else if (!setPropertyDirectly && state.Current.IsProcessingProperty(ClassType.Enumerable))
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                Debug.Assert(state.Current.ReturnValue != null);
                if (state.Current.TempEnumerableValues != null)
                {
                    state.Current.TempEnumerableValues.Add(value);
                }
                else
                {
                    IList list = (IList)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                    if (list == null ||
                        // ImmutableArray<T> is a struct, so default value won't be null.
                        state.Current.JsonPropertyInfo.RuntimePropertyType.FullName.StartsWith(DefaultImmutableEnumerableConverter.ImmutableArrayGenericTypeName)) 
                    {
                        state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                    }
                    else
                    {
                        list.Add(value);
                    }
                }
            }
            else if (state.Current.IsProcessingObject(ClassType.Dictionary) ||
                (state.Current.IsProcessingProperty(ClassType.Dictionary) && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.ReturnValue != null);
                IDictionary dictionary = (IDictionary)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));
                dictionary[key] = value;
            }
            else if (state.Current.IsProcessingObject(ClassType.IDictionaryConstructible) ||
                (state.Current.IsProcessingProperty(ClassType.IDictionaryConstructible) && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.TempDictionaryValues != null);
                IDictionary dictionary = (IDictionary)state.Current.TempDictionaryValues;

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
            ref ReadStack state)
        {
            Debug.Assert(!state.Current.SkipProperty);

            if (state.Current.IsProcessingObject(ClassType.Enumerable))
            {
                if (state.Current.TempEnumerableValues != null)
                {
                    ((IList<TProperty>)state.Current.TempEnumerableValues).Add(value);
                }
                else
                {
                    ((IList<TProperty>)state.Current.ReturnValue).Add(value);
                }
            }
            else if (state.Current.IsProcessingProperty(ClassType.Enumerable))
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                Debug.Assert(state.Current.ReturnValue != null);
                if (state.Current.TempEnumerableValues != null)
                {
                    ((IList<TProperty>)state.Current.TempEnumerableValues).Add(value);
                }
                else
                {
                    IList<TProperty> list = (IList<TProperty>)state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                    if (list == null)
                    {
                        state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                    }
                    else
                    {
                        list.Add(value);
                    }
                }
            }
            else if (state.Current.IsProcessingDictionary())
            {
                Debug.Assert(state.Current.ReturnValue != null);

                object currentDictionary = state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                if (currentDictionary is IDictionary<string, TProperty> genericDict)
                {
                    Debug.Assert(!genericDict.IsReadOnly);
                    genericDict[key] = value;
                }
                else if (currentDictionary is IDictionary dict)
                {
                    Debug.Assert(!dict.IsReadOnly);
                    dict[key] = value;
                }
                else
                {
                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(currentDictionary.GetType(), parentType: null, memberInfo: null);
                }
            }
            else if (state.Current.IsProcessingIDictionaryConstructible())
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
