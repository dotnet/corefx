// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Provides functionality to serialize objects or value types to JSON and
    /// deserialize JSON into objects or value types.
    /// </summary>
    public static partial class JsonSerializer
    {
        private static void ReadCore(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack state)
        {
            try
            {
                JsonReaderState initialState = default;

                while (true)
                {
                    if (options.ReadAhead)
                    {
                        // When we're reading ahead we always have to save the state
                        // as we don't know if the next token is an opening object or
                        // array brace.
                        initialState = reader.CurrentState;
                    }

                    if (!reader.Read())
                    {
                        break;
                    }

                    JsonTokenType tokenType = reader.TokenType;

                    if (JsonHelpers.IsInRangeInclusive(tokenType, JsonTokenType.String, JsonTokenType.False))
                    {
                        Debug.Assert(tokenType == JsonTokenType.String || tokenType == JsonTokenType.Number || tokenType == JsonTokenType.True || tokenType == JsonTokenType.False);

                        HandleValue(tokenType, options, ref reader, ref state);
                    }
                    else if (tokenType == JsonTokenType.PropertyName)
                    {
                        HandlePropertyName(options, ref reader, ref state);
                    }
                    else if (tokenType == JsonTokenType.StartObject)
                    {
                        if (state.Current.SkipProperty)
                        {
                            state.Push();
                            state.Current.Drain = true;
                        }
                        else if (state.Current.IsProcessingValue)
                        {
                            if (!HandleObjectAsValue(tokenType, options, ref reader, ref state, ref initialState))
                            {
                                // Need more data
                                break;
                            }
                        }
                        else if (state.Current.IsProcessingDictionary)
                        {
                            HandleStartDictionary(options, ref reader, ref state);
                        }
                        else
                        {
                            HandleStartObject(options, ref reader, ref state);
                        }
                    }
                    else if (tokenType == JsonTokenType.EndObject)
                    {
                        if (state.Current.Drain)
                        {
                            state.Pop();
                        }
                        else if (state.Current.IsProcessingDictionary)
                        {
                            HandleEndDictionary(options, ref reader, ref state);
                        }
                        else
                        {
                            HandleEndObject(options, ref reader, ref state);
                        }
                    }
                    else if (tokenType == JsonTokenType.StartArray)
                    {
                        if (!state.Current.IsProcessingValue)
                        {
                            HandleStartArray(options, ref reader, ref state);
                        }
                        else if (!HandleObjectAsValue(tokenType, options, ref reader, ref state, ref initialState))
                        {
                            // Need more data
                            break;
                        }
                    }
                    else if (tokenType == JsonTokenType.EndArray)
                    {
                        HandleEndArray(options, ref reader, ref state);
                    }
                    else if (tokenType == JsonTokenType.Null)
                    {
                        HandleNull(ref reader, ref state, options);
                    }
                }
            }
            catch (JsonReaderException e)
            {
                // Re-throw with Path information.
                ThrowHelper.ReThrowWithPath(e, state.JsonPath);
            }

            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HandleObjectAsValue(
            JsonTokenType tokenType,
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack readStack,
            ref JsonReaderState readerState)
        {
            if (options.ReadAhead)
            {
                // Attempt to skip to make sure we have all the data we need.
                bool complete = reader.TrySkip();

                // We need to restore the state in all cases as we need to be positioned back before the current token.
                reader = new Utf8JsonReader(reader.OriginalSpan, isFinalBlock: reader.IsFinalBlock, state: readerState);
                if (!complete)
                {
                    // Couldn't read to the end of the object, exit out to get more data in the buffer.
                    return false;
                }
            }
            HandleValue(tokenType, options, ref reader, ref readStack);
            return true;
        }

        private static ReadOnlySpan<byte> GetUnescapedString(ReadOnlySpan<byte> utf8Source, int idx)
        {
            // The escaped name is always longer than the unescaped, so it is safe to use escaped name for the buffer length.
            int length = utf8Source.Length;
            byte[] pooledName = null;

            Span<byte> unescapedName = length <= JsonConstants.StackallocThreshold ?
                stackalloc byte[length] :
                (pooledName = ArrayPool<byte>.Shared.Rent(length));

            JsonReaderHelper.Unescape(utf8Source, unescapedName, idx, out int written);
            ReadOnlySpan<byte> propertyName = unescapedName.Slice(0, written).ToArray();

            if (pooledName != null)
            {
                // We clear the array because it is "user data" (although a property name).
                new Span<byte>(pooledName, 0, written).Clear();
                ArrayPool<byte>.Shared.Return(pooledName);
            }

            return propertyName;
        }
    }
}
