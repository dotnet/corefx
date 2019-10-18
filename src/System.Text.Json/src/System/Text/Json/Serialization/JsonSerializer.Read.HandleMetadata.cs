// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        private static void HandleMetadataPropertyValue(JsonPropertyInfo propertyInfo, ref Utf8JsonReader reader, ref ReadStack state)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Value for metadata properties cannot be other than string.");
            }

            GetMetadataAndValue(ref state, out MetadataPropertyName metadata, out object value);
            string key = reader.GetString();

            if (metadata == MetadataPropertyName.Id)
            {
                state.AddReference(key, value);
            }
            else if (metadata == MetadataPropertyName.Ref)
            {
                state.Current.ReferenceId = key;
                state.Current.ShouldHandleReference = true;
            }
        }

        private static void GetMetadataAndValue(ref ReadStack state, out MetadataPropertyName metadata, out object value)
        {
            if (state.Current.IsProcessingProperty(ClassType.Dictionary))
            {
                state.Current.DictionaryKeyIsMetadata = false;
                metadata = state.Current.DictionaryMetadata;

                value = metadata == MetadataPropertyName.Ref ? null : state.Current.JsonPropertyInfo.GetValueAsObject(state.Current.ReturnValue);
            }
            else if (state.Current.IsProcessingObject(ClassType.Dictionary))
            {
                state.Current.DictionaryKeyIsMetadata = false;
                metadata = state.Current.DictionaryMetadata;

                value = state.Current.ReturnValue;
            }
            else
            {
                metadata = state.Current.JsonPropertyInfo.MetadataName;
                value = state.Current.ReturnValue;
            }
        }

        internal static MetadataPropertyName GetMetadataPropertyName(ReadOnlySpan<byte> propertyName)
        {
            if (propertyName[0] == '$')
            {
                switch (propertyName.Length)
                {
                    case 3:
                        if (propertyName[1] == 'i' &&
                            propertyName[2] == 'd')
                        {
                            return MetadataPropertyName.Id;
                        }
                        break;

                    case 4:
                        if (propertyName[1] == 'r' &&
                            propertyName[2] == 'e' &&
                            propertyName[3] == 'f')
                        {
                            return MetadataPropertyName.Ref;
                        }
                        break;

                    case 7:
                        if (propertyName[1] == 'v' &&
                            propertyName[2] == 'a' &&
                            propertyName[3] == 'l' &&
                            propertyName[4] == 'u' &&
                            propertyName[5] == 'e' &&
                            propertyName[6] == 's')
                        {
                            return MetadataPropertyName.Values;
                        }
                        break;
                }
            }

            return MetadataPropertyName.Unknown;
        }

        private static void HandleReference(JsonSerializerOptions options, ref ReadStack state, ref Utf8JsonReader reader)
        {
            object referenceValue = state.GetReference(state.Current.ReferenceId);
            if (state.Current.IsProcessingProperty(ClassType.Dictionary))
            {
                ApplyObjectToEnumerable(referenceValue, ref state, setPropertyDirectly: true);
                state.Current.EndProperty();
            }
            else
            {
                state.Current.ReturnValue = referenceValue;
                HandleEndObject(ref state);
            }

            state.Current.ShouldHandleReference = false;
        }

        internal static void SetAsPreserved(ref ReadStackFrame frame)
        {
            if (frame.IsProcessingProperty(ClassType.Dictionary))
            {
                if (frame.CollectionIsPreserved)
                {
                    throw new JsonException("Object already defines a reference identifier.");
                }

                frame.CollectionIsPreserved = true;
            }
            else
            {
                if (frame.IsPreserved)
                {
                    throw new JsonException("Object already defines a reference identifier.");
                }

                frame.IsPreserved = true;
            }
        }
    }

    internal enum MetadataPropertyName
    {
        Unknown,
        Id,
        Ref,
        Values,
    }
}
