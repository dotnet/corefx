// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal class JsonPreservedReference<T>
    {
        [JsonPropertyName("$values")]
        public T Values { get; set; }
    }

    public static partial class JsonSerializer
    {
        private static void HandleStartObject(JsonSerializerOptions options, ref ReadStack state)
        {
            Debug.Assert(!state.Current.IsProcessingDictionaryOrIDictionaryConstructible());

            if (state.Current.IsProcessingEnumerable())
            {
                // A potential Preserved Array - Hit an StartObject while enumerable has not been initialized.
                if (options.ReferenceHandlingOnDeserialize == ReferenceHandlingOnDeserialize.PreserveDuplicates && !state.Current.CollectionPropertyInitialized)
                {
                    // Is a property.
                    if (state.Current.IsProcessingProperty(ClassType.Enumerable))
                    {
                        if (state.Current.JsonPropertyInfo.IsMetadata)
                        {
                            throw new JsonException("The property is already part of a preserved array object, cannot be read as a preserved array.");
                        }

                        Type preservedObjType = typeof(JsonPreservedReference<>).MakeGenericType(state.Current.JsonPropertyInfo.RuntimePropertyType); // is this the right property?
                        state.Push();
                        state.Current.Initialize(preservedObjType, options);
                        //return;?

                    }
                    // Is the current frame object type.
                    else
                    {
                        Type preservedObjType = typeof(JsonPreservedReference<>).MakeGenericType(state.Current.JsonClassInfo.Type); // is this the right property?
                        // Overwrite the current frame.
                        state.Current.Initialize(preservedObjType, options);
                        //return;?
                    }

                    state.Current.IsPreservedArray = true;
                }
                else
                {
                    // A nested object within an enumerable.
                    Type objType = state.Current.GetElementType();
                    state.Push();
                    state.Current.Initialize(objType, options);
                }
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

            if (state.Current.IsProcessingIDictionaryConstructible())
            {
                state.Current.TempDictionaryValues = (IDictionary)classInfo.CreateConcreteDictionary();
            }
            else
            {
                state.Current.ReturnValue = classInfo.CreateObject();
            }
        }

        private static void HandleEndObject(ref ReadStack state)
        {
            // Only allow dictionaries to be processed here if this is the DataExtensionProperty or a reference object evaluated as null and is now finishing the dictionary object.
            Debug.Assert(
                (!state.Current.IsProcessingDictionary() || (state.Current.JsonClassInfo.DataExtensionProperty == state.Current.JsonPropertyInfo || state.Current.ShouldHandleReference)) &&
                !state.Current.IsProcessingIDictionaryConstructible());

            // Check if we are trying to build the sorted cache.
            if (state.Current.PropertyRefCache != null)
            {
                state.Current.JsonClassInfo.UpdateSortedPropertyCache(ref state.Current);
            }

            object value;
            // If we are returing a preserved array.
            if (state.Current.IsPreservedArray && state.Current.ReturnValue != null)
            {
                Type referenceType = state.Current.ReturnValue.GetType();
                value = referenceType.GetProperty("Values").GetValue(state.Current.ReturnValue);
            }
            else
            {
                value = state.Current.ReturnValue;
            }

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
