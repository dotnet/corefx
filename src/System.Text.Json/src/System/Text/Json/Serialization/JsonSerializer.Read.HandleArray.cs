// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(arrayType);
            }

            Debug.Assert(state.Current.IsProcessingCollection());

            if (state.Current.CollectionPropertyInitialized)
            {
                // A nested json array so push a new stack frame.
                Type elementType = jsonPropertyInfo.ElementClassInfo.Type;

                state.Push();
                state.Current.Initialize(elementType, options);
            }

            state.Current.CollectionPropertyInitialized = true;

            // We should not be processing custom converters here.
            Debug.Assert(state.Current.JsonClassInfo.ClassType != ClassType.Value);

            // Set or replace the existing enumerable value.
            object value = ReadStackFrame.CreateEnumerableValue(ref state);

            // If value is not null, then we don't have a converter so apply the value.
            if (value != null)
            {
                state.Current.DetermineEnumerablePopulationStrategy(options, value);

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
            else if (state.Current.IsProcessingObject(ClassType.Enumerable | ClassType.IListConstructible))
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
                //state.Current.AddObjectToEnumerable(state.Current.ReturnValue, value);
                state.Current.ElementPropertyInfo.AddObjectToEnumerable(state.Current.ReturnValue, value);
            }
            else if (!setPropertyDirectly && state.Current.IsProcessingProperty(ClassType.Enumerable))
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
                    state.Current.ElementPropertyInfo.AddObjectToEnumerable(currentEnumerable, value);
                }

            }
            else if (state.Current.IsProcessingObject(ClassType.Dictionary) || (state.Current.IsProcessingProperty(ClassType.Dictionary) && !setPropertyDirectly))
            {
                Debug.Assert(state.Current.ReturnValue != null);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                state.Current.ElementPropertyInfo.AddObjectToDictionary(
                    state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue),
                    key,
                    value);
            }
            else if (state.Current.IsProcessingObject(ClassType.IListConstructible))
            {
                if (state.Current.TempEnumerableValues == null)
                {
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(state.Current.JsonClassInfo.Type);
                    return;
                }

                state.Current.TempEnumerableValues.Add(value);
            }
            else if (state.Current.IsProcessingProperty(ClassType.IListConstructible) && !setPropertyDirectly)
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
                        //state.Current.AddObjectToEnumerable(currentEnumerable, value);
                        state.Current.ElementPropertyInfo.AddObjectToEnumerable(currentEnumerable, value);
                    }
                }
            }
            else if (state.Current.IsProcessingObject(ClassType.IDictionaryConstructible) ||
                (state.Current.IsProcessingProperty(ClassType.IDictionaryConstructible) && !setPropertyDirectly))
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
            ref ReadStack state)
        {
            Debug.Assert(!state.Current.SkipProperty);

            if (state.Current.IsProcessingObject(ClassType.Enumerable))
            {
                AddValueToEnumerable(state.Current.ElementPropertyInfo, state.Current.ReturnValue, value);
            }
            else if (state.Current.IsProcessingProperty(ClassType.Enumerable))
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
                    AddValueToEnumerable(state.Current.ElementPropertyInfo, currentEnumerable, value);
                }
            }
            else if (state.Current.IsProcessingDictionary())
            {
                Debug.Assert(state.Current.ReturnValue != null);

                string key = state.Current.KeyName;
                Debug.Assert(!string.IsNullOrEmpty(key));

                object currentDictionary = state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);

                if (currentDictionary is IDictionary dict)
                {
                    Debug.Assert(!dict.IsReadOnly);
                    dict[key] = value;
                }
                else if (currentDictionary is IDictionary<string, TProperty> genericDict)
                {
                    Debug.Assert(!genericDict.IsReadOnly);
                    genericDict[key] = value;
                }
                else
                {
                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(currentDictionary.GetType(), parentType: null, memberInfo: null);
                }

            }
            else if (state.Current.IsProcessingIListConstructible())
            {
                Debug.Assert(state.Current.JsonPropertyInfo != null);
                Debug.Assert(state.Current.TempEnumerableValues != null);

                ((IList<TProperty>)state.Current.TempEnumerableValues).Add(value);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddValueToEnumerable<TProperty>(JsonPropertyInfo elementPropertyInfo, object target, TProperty value)
        {
            if (target is ICollection<TProperty> collection)
            {
                Debug.Assert(!collection.IsReadOnly);
                collection.Add(value);
            }
            else if (target is IList list)
            {
                Debug.Assert(!list.IsReadOnly);
                list.Add(value);
            }
            else if (target is Stack<TProperty> stack)
            {
                stack.Push(value);
            }
            else if (target is Queue<TProperty> queue)
            {
                queue.Enqueue(value);
            }
            else
            {
                elementPropertyInfo.AddObjectToEnumerableWithReflection(target, value);
            }
        }
    }
}
