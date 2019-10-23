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
            Debug.Assert(!state.Current.IsProcessingDictionary());

            if (state.Current.IsProcessingEnumerable())
            {
                // A nested object within an enumerable.
                Type objType = state.Current.GetElementType();
                state.Push();
                state.Current.Initialize(objType, options);
            }
            else if (state.Current.JsonPropertyInfo != null)
            {
                // Nested object.
                Type objType = state.Current.JsonPropertyInfo.RuntimePropertyType;
                state.Push();
                state.Current.Initialize(objType, options);
            }

            JsonClassInfo classInfo = state.Current.JsonClassInfo;

            if (classInfo.CreateObject is null && classInfo.ClassType == ClassType.Object)
            {
                if (classInfo.Type.IsInterface)
                {
                    ThrowHelper.ThrowInvalidOperationException_DeserializePolymorphicInterface(classInfo.Type);
                }
                else
                {
                    ThrowHelper.ThrowInvalidOperationException_DeserializeMissingParameterlessConstructor(classInfo.Type);
                }
            }

            if (state.Current.IsProcessingObject(ClassType.Dictionary))
            {
                object value = ReadStackFrame.CreateDictionaryValue(ref state);

                // If value is not null, then we don't have a converter so apply the value.
                if (value != null)
                {
                    state.Current.ReturnValue = value;
                    state.Current.DetermineIfDictionaryCanBePopulated(state.Current.ReturnValue);
                }

                state.Current.CollectionPropertyInitialized = true;
            }
            else
            {
                state.Current.ReturnValue = classInfo.CreateObject();
            }
        }

        private static void HandleEndObject(ref ReadStack state)
        {
            // Only allow dictionaries to be processed here if this is the DataExtensionProperty.
            Debug.Assert(!state.Current.IsProcessingDictionary() || state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo);

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
