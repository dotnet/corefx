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
        private static void ReadCore(
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack readStack)
        {
            try
            {
                JsonReaderState initialState = default;
                long initialBytesConsumed = default;

                while (true)
                {
                    if (readStack.ReadAhead)
                    {
                        // When we're reading ahead we always have to save the state
                        // as we don't know if the next token is an opening object or
                        // array brace.
                        initialState = reader.CurrentState;
                        initialBytesConsumed = reader.BytesConsumed;
                    }

                    if (!reader.Read())
                    {
                        // Need more data
                        break;
                    }

                    JsonTokenType tokenType = reader.TokenType;

                    if (JsonHelpers.IsInRangeInclusive(tokenType, JsonTokenType.String, JsonTokenType.False))
                    {
                        Debug.Assert(tokenType == JsonTokenType.String || tokenType == JsonTokenType.Number || tokenType == JsonTokenType.True || tokenType == JsonTokenType.False);

                        if (readStack.ReadMetadataValue)
                        {
                            HandleMetadataValue(tokenType, ref reader, ref readStack);
                        }
                        else
                        {
                            HandleDelayedStart(options, ref reader, ref readStack);
                            HandleValue(tokenType, options, ref reader, ref readStack);
                        }
                    }
                    else if (tokenType == JsonTokenType.PropertyName)
                    {
                        if (options.ReadReferenceHandling == ReferenceHandlingOnDeserialize.PreserveDuplicates)
                        {
                            HandleMetadataProperty(options, ref reader, ref readStack);
                        }

                        if (readStack.LastMetadata == MetadataPropertyName.Unknown)
                        {
                            HandleDelayedStart(options, ref reader, ref readStack);
                            HandlePropertyName(options, ref reader, ref readStack);
                        }
                    }
                    else if (tokenType == JsonTokenType.StartObject)
                    {
                        HandleDelayedStart(options, ref reader, ref readStack);
                        if (readStack.Current.SkipProperty)
                        {
                            readStack.Push();
                            readStack.Current.Drain = true;
                        }
                        else if (readStack.Current.IsProcessingValue())
                        {
                            if (!HandleObjectAsValue(tokenType, options, ref reader, ref readStack, ref initialState, initialBytesConsumed))
                            {
                                // Need more data
                                break;
                            }
                        }
                        else if (readStack.Current.IsProcessingDictionaryOrIDictionaryConstructible())
                        {
                            readStack.DelayedHandle = InitTaskType.Dictionary;
                        }
                        else
                        {
                            readStack.DelayedHandle = InitTaskType.Object;
                        }
                    }
                    else if (tokenType == JsonTokenType.EndObject)
                    {
                        HandleDelayedStart(options, ref reader, ref readStack);
                        if (readStack.Current.HandleRefEndBrace)
                        {
                            // Skip brace of reference object.
                            readStack.Current.HandleRefEndBrace = false;
                            continue;
                        }
                        else if (readStack.Current.HandleArrayEndWrappingBrace)
                        {
                            // Skip wrapping brace of reference-preserved array.
                            readStack.Current.HandleArrayEndWrappingBrace = false;
                            continue;
                        }
                        else if (readStack.Current.Drain)
                        {
                            readStack.Pop();

                            // Clear the current property in case it is a dictionary, since dictionaries must have EndProperty() called when completed.
                            // A non-dictionary property can also have EndProperty() called when completed, although it is redundant.
                            readStack.Current.EndProperty();
                        }
                        else if (readStack.Current.IsProcessingDictionaryOrIDictionaryConstructible())
                        {
                            HandleEndDictionary(options, ref readStack);
                        }
                        else
                        {
                            HandleEndObject(ref readStack);
                        }
                    }
                    else if (tokenType == JsonTokenType.StartArray)
                    {
                        HandleDelayedStart(options, ref reader, ref readStack);
                        if (!readStack.Current.IsProcessingValue())
                        {
                            readStack.DelayedHandle = InitTaskType.Enumerable;
                        }
                        else if (!HandleObjectAsValue(tokenType, options, ref reader, ref readStack, ref initialState, initialBytesConsumed))
                        {
                            // Need more data
                            break;
                        }
                    }
                    else if (tokenType == JsonTokenType.EndArray)
                    {
                        HandleDelayedStart(options, ref reader, ref readStack);
                        HandleEndArray(options, ref readStack);
                    }
                    else if (tokenType == JsonTokenType.Null)
                    {
                        HandleDelayedStart(options, ref reader, ref readStack);
                        HandleNull(ref reader, ref readStack);
                    }
                }
            }
            catch (JsonReaderException ex)
            {
                // Re-throw with Path information.
                ThrowHelper.ReThrowWithPath(readStack, ex);
            }
            catch (FormatException ex) when (ex.Source == ThrowHelper.ExceptionSourceValueToRethrowAsJsonException)
            {
                ThrowHelper.ReThrowWithPath(readStack, reader, ex);
            }
            catch (InvalidOperationException ex) when (ex.Source == ThrowHelper.ExceptionSourceValueToRethrowAsJsonException)
            {
                ThrowHelper.ReThrowWithPath(readStack, reader, ex);
            }
            catch (JsonException ex)
            {
                ThrowHelper.AddExceptionInformation(readStack, reader, ex);
                throw;
            }

            readStack.BytesConsumed += reader.BytesConsumed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HandleObjectAsValue(
            JsonTokenType tokenType,
            JsonSerializerOptions options,
            ref Utf8JsonReader reader,
            ref ReadStack readStack,
            ref JsonReaderState initialState,
            long initialBytesConsumed)
        {
            if (readStack.ReadAhead)
            {
                // Attempt to skip to make sure we have all the data we need.
                bool complete = reader.TrySkip();

                // We need to restore the state in all cases as we need to be positioned back before
                // the current token to either attempt to skip again or to actually read the value in
                // HandleValue below.

                reader = new Utf8JsonReader(
                    reader.OriginalSpan.Slice(checked((int)initialBytesConsumed)),
                    isFinalBlock: reader.IsFinalBlock,
                    state: initialState);
                Debug.Assert(reader.BytesConsumed == 0);
                readStack.BytesConsumed += initialBytesConsumed;

                if (!complete)
                {
                    // Couldn't read to the end of the object, exit out to get more data in the buffer.
                    return false;
                }

                // Success, requeue the reader to the token for HandleValue.
                reader.Read();
                Debug.Assert(tokenType == reader.TokenType);
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

        internal static void HandleDelayedStart(JsonSerializerOptions options, ref Utf8JsonReader reader, ref ReadStack state)
        {
            if (state.DelayedHandle == InitTaskType.None)
            {
                return;
            }

            if (state.DelayedHandle == InitTaskType.Dictionary)
            {
                HandleStartDictionary(options, ref state);
            }
            else if (state.DelayedHandle == InitTaskType.Object)
            {
                HandleStartObject(options, ref state);
            }
            else if (state.DelayedHandle == InitTaskType.Enumerable)
            {
                HandleStartArray(options, ref reader, ref state);
            }
            else if (state.DelayedHandle == InitTaskType.Ref)
            {
                object refValue = state.GetReference(state.DelayedMetadataId);

                if (refValue == null)
                {
                    HandleNull(ref reader, ref state);
                }
                else
                {
                    // We have a Dictionary or Enumerable as a property.
                    if (!state.Current.CollectionPropertyInitialized && (state.Current.IsProcessingProperty(ClassType.Dictionary | ClassType.Enumerable)))
                    {
                        ApplyObjectToEnumerable(refValue, ref state, setPropertyDirectly: true);
                        state.Current.EndProperty();
                    }
                    else
                    {
                        ApplyObjectToEnumerable(refValue, ref state);
                    }
                }
            }

            state.DelayedMetadataId = null;
            state.DelayedHandle = default;
        }
    }
}
