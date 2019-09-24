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
            else if (state.Current.JsonClassInfo.ClassType == ClassType.Unknown)
            {
                jsonPropertyInfo = state.Current.JsonClassInfo.CreatePolymorphicProperty(jsonPropertyInfo, typeof(object), options);
            }

            // Verify that we have a valid enumerable.
            Type arrayType = jsonPropertyInfo.RuntimePropertyType;
            if (!typeof(IEnumerable).IsAssignableFrom(arrayType))
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(arrayType, reader, state.JsonPath());
            }

            Debug.Assert(state.Current.IsProcessingEnumerableOrDictionary);

            if (state.Current.CollectionPropertyInitialized)
            {
                // A nested json array so push a new stack frame.
                Type elementType = jsonPropertyInfo.ElementClassInfo.Type;

                state.Push();
                state.Current.Initialize(elementType, options);
            }

            state.Current.CollectionPropertyInitialized = true;

            if (state.Current.JsonClassInfo.ClassType == ClassType.Value)
            {
                // Custom converter code path.
                state.Current.JsonPropertyInfo.Read(JsonTokenType.StartObject, ref state, ref reader);
            }
            else
            {
                // Set or replace the existing enumerable value.
                object value = ReadStackFrame.CreateEnumerableValue(ref state);

                // If value is not null, then we don't have a converter so apply the value.
                if (value != null)
                {
                    state.Current.CreateEnumerableAddMethod(options, value);

                    if (state.Current.ReturnValue != null)
                    {
                        state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                    }
                    else
                    {
                        state.Current.ReturnValue = value;
                    }
                }
            }
        }

        private static bool HandleEndArray(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            bool lastFrame = state.IsLastFrame;

            if (state.Current.Drain)
            {
                // The array is not being applied to the object.
                state.Pop();
                return lastFrame;
            }

            IEnumerable value = ReadStackFrame.GetEnumerableValue(ref state.Current);

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
            else if (state.Current.IsEnumerableProperty)
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
                else if (state.Current.IsEnumerable || state.Current.IsDictionary ||state.Current.IsIListConstructible || state.Current.IsIDictionaryConstructible)
                {
                    // Returning a non-converted list.
                    return true;
                }
                // else there must be an outer object, so we'll return false here.
            }
            else if (state.Current.IsEnumerable || state.Current.IsIListConstructible)
            {
                state.Pop();
            }

            ApplyObjectToEnumerable(options, value, ref state, ref reader);

            return false;
        }

        // If this method is changed, also change ApplyValueToEnumerable.
        internal static void ApplyObjectToEnumerable(
            JsonSerializerOptions options,
            object value,
            ref ReadStack state,
            ref Utf8JsonReader reader,
            bool setPropertyDirectly = false)
        {
            Debug.Assert(!state.Current.SkipProperty);

            if (state.Current.IsEnumerable)
            {
                state.Current.AddObjectToEnumerable(value);
            }
            else if (!setPropertyDirectly && state.Current.IsEnumerableProperty)
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                Debug.Assert(state.Current.ReturnValue != null);

                object currentEnumerable = state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                if (currentEnumerable == null ||
                    // ImmutableArray<T> is a struct, so default value won't be null.
                    state.Current.JsonPropertyInfo.RuntimePropertyType.FullName.StartsWith(DefaultImmutableEnumerableConverter.ImmutableArrayGenericTypeName))
                {
                    state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                }
                else
                {
                    state.Current.AddObjectToEnumerable(value);
                }

            }
            else if (state.Current.IsDictionary || (state.Current.IsDictionaryProperty && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.ReturnValue != null);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                if (state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo)
                {
                    state.Current.AddObjectToExtensionData(key, value);
                }
                else
                {
                    state.Current.AddObjectToDictionary(key, value);
                }
            }
            else if (state.Current.IsIListConstructible)
            {
                if (state.Current.TempEnumerableValues == null)
                {
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(state.Current.JsonClassInfo.Type, reader, state.JsonPath());
                    return;
                }

                state.Current.TempEnumerableValues.Add(value);
            }
            else if (state.Current.IsIListConstructibleProperty && !setPropertyDirectly)
            {
                if (state.Current.TempEnumerableValues != null)
                {
                    state.Current.TempEnumerableValues.Add(value);
                }
                else
                {
                    object currentEnumerable = state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                    if (currentEnumerable == null ||
                        // ImmutableArray<T> is a struct, so default value won't be null.
                        state.Current.JsonPropertyInfo.RuntimePropertyType.FullName.StartsWith(DefaultImmutableEnumerableConverter.ImmutableArrayGenericTypeName))
                    {
                        state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                    }
                    else
                    {
                        state.Current.AddValueToEnumerable(value);
                    }
                }
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

            if (state.Current.IsEnumerable)
            {
                state.Current.AddValueToEnumerable(value);
            }
            else if (state.Current.IsEnumerableProperty)
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                Debug.Assert(state.Current.ReturnValue != null);

                object currentEnumerable = state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
                if (currentEnumerable == null ||
                    // ImmutableArray<T> is a struct, so default value won't be null.
                    state.Current.JsonPropertyInfo.RuntimePropertyType.FullName.StartsWith(DefaultImmutableEnumerableConverter.ImmutableArrayGenericTypeName))
                {
                    state.Current.JsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, value);
                }
                else
                {
                    state.Current.AddValueToEnumerable(value);
                }
            }
            else if (state.Current.IsProcessingDictionary)
            {
                Debug.Assert(state.Current.ReturnValue != null);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                if (state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo)
                {
                    state.Current.AddObjectToExtensionData(key, value);
                }
                else
                {
                    state.Current.AddObjectToDictionary(key, value);
                }

            }
            else if (state.Current.IsIListConstructible)
            {
                Debug.Assert(state.Current.TempEnumerableValues != null);
                ((IList<TProperty>)state.Current.TempEnumerableValues).Add(value);
            }
            else if (state.Current.IsIListConstructibleProperty)
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                Debug.Assert(state.Current.ReturnValue != null);
                Debug.Assert(state.Current.TempEnumerableValues != null);

                ((IList<TProperty>)state.Current.TempEnumerableValues).Add(value);
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
