// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    /// <summary>
    /// Provides functionality to serialize objects or value types to JSON and
    /// deserialize JSON into objects or value types.
    /// </summary>
    public static partial class JsonSerializer
    {
        private static void HandleMetadataProperty(JsonSerializerOptions options, ref Utf8JsonReader reader, ref ReadStack state)
        {
            if (state.Current.Drain) //Verify if this is the right condition. Why should I use this over state.Current.SkipProperty?
            {
                return;
            }

            MetadataPropertyName meta = GetMetadataPropertyName(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);
            MetadataPropertyName previous = state.LastMetadata;

            if (meta == MetadataPropertyName.Unknown)
            {
                if (state.Current.HandleRefEndBrace)
                {
                    throw new JsonException("Properties other than '$ref' are not allowed in reference objects.");
                }
            }

            else if (meta == MetadataPropertyName.Id)
            {
                state.ReadMetadataValue = true;
                //TODO: Validate that multiple ids are not found in the same object, however, this is not validated by Json.Net.
            }

            else if (meta == MetadataPropertyName.Values)
            {
                if (previous != MetadataPropertyName.Id || state.DelayedMetadataId == null)
                {
                    throw new JsonException("Cannot have a preserved enumerable without specifying an id.");
                }
                // Remove misconcepted object.
                state.DelayedHandle = InitTaskType.None;
                state.Current.HandleArrayEndWrappingBrace = true;
            }

            else if (meta == MetadataPropertyName.Ref)
            {
                // since pending objects are initialized when a non-metadata property is found, having zero pending tasks means that $ref is not the first property in the object.
                if (state.DelayedHandle == InitTaskType.None)
                {
                    throw new JsonException("Properties other than '$ref' are not allowed in reference objects.");
                }

                state.ReadMetadataValue = true;
                state.DelayedHandle = InitTaskType.Ref;
                state.Current.HandleRefEndBrace = true;
            }

            state.LastMetadata = meta;
        }

        private static void HandleMetadataValue(JsonTokenType tokenType, ref Utf8JsonReader reader, ref ReadStack state)
        {
            if (tokenType != JsonTokenType.String)
            {
                throw new JsonException("Metadata property values must be of type string.");
            }

            state.DelayedMetadataId = reader.GetString();
            state.ReadMetadataValue = false;
        }

        private static MetadataPropertyName GetMetadataPropertyName(ReadOnlySpan<byte> propertyName)
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


    }

    // Preserve Reference Handling
    internal enum MetadataPropertyName
    {
        Unknown, //it may be called invalid as well
        Id,
        Ref,
        Values,
    }

    internal enum InitTaskType
    {
        None,
        Dictionary,
        Object,
        Enumerable,
        Ref
    }
}
