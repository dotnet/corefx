﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleStartObject(JsonSerializerOptions options, ref ReadStack state)
        {
            Debug.Assert(!state.Current.IsProcessingDictionaryOrIDictionaryConstructible());

            // Note: unless we are a root object, we are going to push a property onto the ReadStack
            // in the if/else if check below.

            if (state.Current.IsProcessingEnumerable())
            {
                // A nested object within an enumerable (non-dictionary).

                if (!state.Current.CollectionPropertyInitialized)
                {
                    // We have bad JSON: enumerable element appeared without preceding StartArray token.
                    ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(state.Current.JsonPropertyInfo.DeclaredPropertyType);
                }

                Type objType = state.Current.GetElementType();
                state.Push();
                state.Current.Initialize(objType, options);
            }
            else if (state.Current.JsonPropertyInfo != null)
            {
                // Nested object within an object.
                Debug.Assert(state.Current.IsProcessingObject(ClassType.Object));

                Type objType = state.Current.JsonPropertyInfo.RuntimePropertyType;
                state.Push();
                state.Current.Initialize(objType, options);
            }

            JsonClassInfo classInfo = state.Current.JsonClassInfo;

            if (state.Current.IsProcessingObject(ClassType.IDictionaryConstructible))
            {
                state.Current.TempDictionaryValues = (IDictionary)classInfo.CreateConcreteDictionary();
                state.Current.CollectionPropertyInitialized = true;
            }
            else if (state.Current.IsProcessingObject(ClassType.Dictionary))
            {
                if (classInfo.CreateObject == null)
                {
                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(classInfo.Type, parentType: null, memberInfo: null);
                }

                state.Current.ReturnValue = classInfo.CreateObject();
                state.Current.CollectionPropertyInitialized = true;
            }
            else if (state.Current.IsProcessingObject(ClassType.Object))
            {
                if (classInfo.CreateObject == null)
                {
                    ThrowHelper.ThrowNotSupportedException_DeserializeCreateObjectDelegateIsNull(classInfo.Type);
                }

                state.Current.ReturnValue = classInfo.CreateObject();

                if (state.Current.IsProcessingDictionary())
                {
                    state.Current.CollectionPropertyInitialized = true;
                }
            }
            else
            {
                // Only dictionaries or objects are valid given the `StartObject` token.
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(classInfo.Type);
            }
        }

        private static void HandleEndObject(ref ReadStack state)
        {
            // Only allow dictionaries to be processed here if this is the DataExtensionProperty.
            Debug.Assert(
                (!state.Current.IsProcessingDictionary() || state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo) &&
                !state.Current.IsProcessingIDictionaryConstructible());

            if (state.Current.JsonClassInfo.ClassType == ClassType.Value)
            {
                // We should be in a converter, thus we must have bad JSON.
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(state.Current.JsonPropertyInfo.RuntimePropertyType);
            }

            // Check if we are trying to build the sorted cache.
            if (state.Current.PropertyRefCache != null)
            {
                state.Current.JsonClassInfo.UpdateSortedPropertyCache(ref state.Current);
            }

            object value = state.Current.ReturnValue;

            if (state.IsLastFrame)
            {
                state.Current.Reset();
                state.Current.ReturnValue = value;
            }
            else
            {
                state.Pop();
                ApplyObjectToEnumerable(value, ref state);
            }
        }
    }
}
