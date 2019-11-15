// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        // AggressiveInlining used although a large method it is only called from one locations and is on a hot path.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandlePropertyName(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            if (state.Current.Drain)
            {
                return;
            }

            Debug.Assert(state.Current.ReturnValue != null || state.Current.TempDictionaryValues != null);
            Debug.Assert(state.Current.JsonClassInfo != null);

            bool isProcessingDictObject = state.Current.IsProcessingObject(ClassType.Dictionary);
            if ((isProcessingDictObject || state.Current.IsProcessingProperty(ClassType.Dictionary)) &&
                state.Current.JsonClassInfo.DataExtensionProperty != state.Current.JsonPropertyInfo)
            {
                if (isProcessingDictObject)
                {
                    state.Current.JsonPropertyInfo = state.Current.JsonClassInfo.PolicyProperty;
                }

                state.Current.KeyName = reader.GetString();
            }
            else
            {
                Debug.Assert(state.Current.JsonClassInfo.ClassType == ClassType.Object);

                state.Current.EndProperty();

                ReadOnlySpan<byte> propertyName = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (reader._stringHasEscaping)
                {
                    int idx = propertyName.IndexOf(JsonConstants.BackSlash);
                    Debug.Assert(idx != -1);
                    propertyName = GetUnescapedString(propertyName, idx);
                }

                JsonPropertyInfo jsonPropertyInfo = state.Current.JsonClassInfo.GetProperty(propertyName, ref state.Current);
                if (jsonPropertyInfo == JsonPropertyInfo.s_missingProperty)
                {
                    JsonPropertyInfo dataExtProperty = state.Current.JsonClassInfo.DataExtensionProperty;
                    if (dataExtProperty == null)
                    {
                        state.Current.JsonPropertyInfo = JsonPropertyInfo.s_missingProperty;
                    }
                    else
                    {
                        state.Current.JsonPropertyInfo = dataExtProperty;
                        state.Current.JsonPropertyName = propertyName.ToArray();
                        state.Current.KeyName = JsonHelpers.Utf8GetString(propertyName);
                        state.Current.CollectionPropertyInitialized = true;

                        CreateDataExtensionProperty(dataExtProperty, ref state);
                    }
                }
                else
                {
                    // Support JsonException.Path.
                    Debug.Assert(
                        jsonPropertyInfo.JsonPropertyName == null ||
                        options.PropertyNameCaseInsensitive ||
                        propertyName.SequenceEqual(jsonPropertyInfo.JsonPropertyName));

                    state.Current.JsonPropertyInfo = jsonPropertyInfo;

                    if (jsonPropertyInfo.JsonPropertyName == null)
                    {
                        byte[] propertyNameArray = propertyName.ToArray();
                        if (options.PropertyNameCaseInsensitive)
                        {
                            // Each payload can have a different name here; remember the value on the temporary stack.
                            state.Current.JsonPropertyName = propertyNameArray;
                        }
                        else
                        {
                            // Prevent future allocs by caching globally on the JsonPropertyInfo which is specific to a Type+PropertyName
                            // so it will match the incoming payload except when case insensitivity is enabled (which is handled above).
                            state.Current.JsonPropertyInfo.JsonPropertyName = propertyNameArray;
                        }
                    }
                }

                // Increment the PropertyIndex so JsonClassInfo.GetProperty() starts with the next property.
                state.Current.PropertyIndex++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandlePropertyNameRef(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            if (state.Current.Drain)
            {
                return;
            }

            if (state.Current.ShouldHandleReference)
            {
                throw new JsonException("Reference objects cannot contain other properties.");
            }

            Debug.Assert(state.Current.ReturnValue != null || state.Current.TempDictionaryValues != null);
            Debug.Assert(state.Current.JsonClassInfo != null);

            bool isProcessingDictObject = state.Current.IsProcessingObject(ClassType.Dictionary);
            if ((isProcessingDictObject || state.Current.IsProcessingProperty(ClassType.Dictionary)) &&
                state.Current.JsonClassInfo.DataExtensionProperty != state.Current.JsonPropertyInfo)
            {
                if (isProcessingDictObject)
                {
                    state.Current.JsonPropertyInfo = state.Current.JsonClassInfo.PolicyProperty;
                }
                //TODO: test dictionary with escape keys.
                MetadataPropertyName meta = GetMetadataPropertyName(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);
                state.Current.MetadataProperty = meta;

                if (meta == MetadataPropertyName.Id)
                {
                    if (state.Current.TempDictionaryValues != null)
                    {
                        throw new JsonException("Immutable types and fixed size arrays cannot be preserved.");
                    }

                    SetAsPreserved(ref state.Current);
                    state.Current.ReadMetadataValue = true;

                    if (state.Current.IsProcessingProperty(ClassType.Dictionary))
                    {
                        SetDictionaryCandidateAsValue(ref state);
                    }
                }
                // All code paths except this one need to call SetDictionaryCandidateAsValue.
                else if (meta == MetadataPropertyName.Ref)
                {
                    bool isPreserved = state.Current.IsProcessingProperty(ClassType.Dictionary) ? state.Current.DictionaryPropertyIsPreserved : state.Current.IsPreserved;
                    if (state.Current.KeyName != null || isPreserved || state.Current.ShouldHandleReference)
                    {
                        throw new JsonException("Reference objects cannot contain other properties.");
                    }
                    state.Current.ReadMetadataValue = true;
                    state.Current.ShouldHandleReference = true;
                }
                //Should we throw or let the Deserializer treat it as a extended property?
                //else if(meta == MetadataPropertyName.Values)
                //{
                //    throw new JsonException("Dictionaries cannot contain $values.");
                //}
                else
                {
                    if (state.Current.IsProcessingProperty(ClassType.Dictionary))
                    {
                        SetDictionaryCandidateAsValue(ref state);
                    }
                }

                state.Current.KeyName = reader.GetString();
            }
            else
            {
                Debug.Assert(state.Current.JsonClassInfo.ClassType == ClassType.Object);

                state.Current.EndProperty();

                ReadOnlySpan<byte> propertyName = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (reader._stringHasEscaping)
                {
                    int idx = propertyName.IndexOf(JsonConstants.BackSlash);
                    Debug.Assert(idx != -1);
                    propertyName = GetUnescapedString(propertyName, idx);
                }

                MetadataPropertyName meta = GetMetadataPropertyName(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);
                state.Current.MetadataProperty = meta;

                if (meta != MetadataPropertyName.Unknown)
                {
                    JsonPropertyInfo info;

                    if (meta == MetadataPropertyName.Values)
                    {
                        string stringPropertyName = JsonHelpers.Utf8GetString(propertyName);
                        info = state.Current.JsonClassInfo.PropertyCache[stringPropertyName];
                        info.IsMetadata = true;
                        state.Current.JsonPropertyInfo = info;
                        if (!state.Current.IsPreserved)
                        {
                            throw new JsonException("Preserved arrays canot lack an identifier.");
                        }
                    }
                    else if (meta == MetadataPropertyName.Id)
                    {
                        info = JsonPropertyInfo.s_metadataProperty;
                        state.Current.JsonPropertyInfo = info;
                        SetAsPreserved(ref state.Current);
                        state.Current.ReadMetadataValue = true;
                    }
                    else //meta == $ref
                    {
                        info = JsonPropertyInfo.s_metadataProperty;
                        state.Current.JsonPropertyInfo = info;
                        if (state.Current.PropertyIndex > 0 || state.Current.IsPreserved || state.Current.ShouldHandleReference)
                        {
                            throw new JsonException("Reference objects cannot contain other properties.");
                        }

                        state.Current.ReadMetadataValue = true;
                        state.Current.ShouldHandleReference = true;
                    }

                    info.JsonPropertyName = propertyName.ToArray();
                    return;
                }

                JsonPropertyInfo jsonPropertyInfo = state.Current.JsonClassInfo.GetProperty(propertyName, ref state.Current);
                if (jsonPropertyInfo == JsonPropertyInfo.s_missingProperty)
                {
                    JsonPropertyInfo dataExtProperty = state.Current.JsonClassInfo.DataExtensionProperty;
                    if (dataExtProperty == null)
                    {
                        state.Current.JsonPropertyInfo = JsonPropertyInfo.s_missingProperty;
                    }
                    else
                    {
                        state.Current.JsonPropertyInfo = dataExtProperty;
                        state.Current.JsonPropertyName = propertyName.ToArray();
                        state.Current.KeyName = JsonHelpers.Utf8GetString(propertyName);
                        state.Current.CollectionPropertyInitialized = true;

                        CreateDataExtensionProperty(dataExtProperty, ref state);
                    }
                }
                else
                {
                    // Support JsonException.Path.
                    Debug.Assert(
                        jsonPropertyInfo.JsonPropertyName == null ||
                        options.PropertyNameCaseInsensitive ||
                        propertyName.SequenceEqual(jsonPropertyInfo.JsonPropertyName));

                    state.Current.JsonPropertyInfo = jsonPropertyInfo;

                    if (jsonPropertyInfo.JsonPropertyName == null)
                    {
                        byte[] propertyNameArray = propertyName.ToArray();
                        if (options.PropertyNameCaseInsensitive)
                        {
                            // Each payload can have a different name here; remember the value on the temporary stack.
                            state.Current.JsonPropertyName = propertyNameArray;
                        }
                        else
                        {
                            // Prevent future allocs by caching globally on the JsonPropertyInfo which is specific to a Type+PropertyName
                            // so it will match the incoming payload except when case insensitivity is enabled (which is handled above).
                            state.Current.JsonPropertyInfo.JsonPropertyName = propertyNameArray;
                        }
                    }
                }

                // Increment the PropertyIndex so JsonClassInfo.GetProperty() starts with the next property.
                state.Current.PropertyIndex++;
            }
        }

        private static void CreateDataExtensionProperty(
            JsonPropertyInfo jsonPropertyInfo,
            ref ReadStack state)
        {
            Debug.Assert(jsonPropertyInfo != null);
            Debug.Assert(state.Current.ReturnValue != null);

            IDictionary extensionData = (IDictionary)jsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
            if (extensionData == null)
            {
                // Create the appropriate dictionary type. We already verified the types.
                Debug.Assert(jsonPropertyInfo.DeclaredPropertyType.IsGenericType);
                Debug.Assert(jsonPropertyInfo.DeclaredPropertyType.GetGenericArguments().Length == 2);
                Debug.Assert(jsonPropertyInfo.DeclaredPropertyType.GetGenericArguments()[0].UnderlyingSystemType == typeof(string));
                Debug.Assert(
                    jsonPropertyInfo.DeclaredPropertyType.GetGenericArguments()[1].UnderlyingSystemType == typeof(object) ||
                    jsonPropertyInfo.DeclaredPropertyType.GetGenericArguments()[1].UnderlyingSystemType == typeof(JsonElement));

                extensionData = (IDictionary)jsonPropertyInfo.RuntimeClassInfo.CreateObject();
                jsonPropertyInfo.SetValueAsObject(state.Current.ReturnValue, extensionData);
            }

            // We don't add the value to the dictionary here because we need to support the read-ahead functionality for Streams.
        }
    }
}
