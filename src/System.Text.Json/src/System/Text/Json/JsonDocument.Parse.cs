// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json
{
    public sealed partial class JsonDocument
    {
        private const int UnseekableStreamInitialRentSize = 4096;

        public static JsonDocument Parse(ReadOnlyMemory<byte> utf8Json, JsonReaderOptions readerOptions = default)
        {
            CheckSupportedOptions(readerOptions);

            return Parse(utf8Json, readerOptions, null);
        }

        public static JsonDocument Parse(ReadOnlySequence<byte> utf8Json, JsonReaderOptions readerOptions = default)
        {
            CheckSupportedOptions(readerOptions);

            if (utf8Json.IsSingleSegment)
            {
                return Parse(utf8Json.First, readerOptions, null);
            }

            int length = checked((int)utf8Json.Length);
            byte[] utf8Bytes = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                utf8Json.CopyTo(utf8Bytes.AsSpan());
                return Parse(utf8Bytes.AsMemory(0, length), readerOptions, utf8Bytes);
            }
            catch
            {
                // Holds document content, clear it before returning it.
                utf8Bytes.AsSpan(0, length).Clear();
                ArrayPool<byte>.Shared.Return(utf8Bytes);
                throw;
            }
        }

        public static JsonDocument Parse(Stream utf8Json, JsonReaderOptions readerOptions = default)
        {
            if (utf8Json == null)
            {
                throw new ArgumentNullException(nameof(utf8Json));
            }

            CheckSupportedOptions(readerOptions);

            ArraySegment<byte> drained = ReadToEnd(utf8Json);

            try
            {
                return Parse(drained.AsMemory(), readerOptions, drained.Array);
            }
            catch
            {
                // Holds document content, clear it before returning it.
                drained.AsSpan().Clear();
                ArrayPool<byte>.Shared.Return(drained.Array);
                throw;
            }
        }

        public static Task<JsonDocument> ParseAsync(
            Stream utf8Json,
            JsonReaderOptions readerOptions = default,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json == null)
            {
                throw new ArgumentNullException(nameof(utf8Json));
            }

            CheckSupportedOptions(readerOptions);

            return ParseAsyncCore(utf8Json, readerOptions, cancellationToken);
        }

        private static async Task<JsonDocument> ParseAsyncCore(
            Stream utf8Json,
            JsonReaderOptions readerOptions = default,
            CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> drained = await ReadToEndAsync(utf8Json, cancellationToken).ConfigureAwait(false);

            try
            {
                return Parse(drained.AsMemory(), readerOptions, drained.Array);
            }
            catch
            {
                // Holds document content, clear it before returning it.
                drained.AsSpan().Clear();
                ArrayPool<byte>.Shared.Return(drained.Array);
                throw;
            }
        }

        public static JsonDocument Parse(ReadOnlyMemory<char> json, JsonReaderOptions readerOptions = default)
        {
            CheckSupportedOptions(readerOptions);

            ReadOnlySpan<char> jsonChars = json.Span;
            int byteCount = Utf8JsonReader.s_utf8Encoding.GetByteCount(jsonChars);
            byte[] utf8Bytes = ArrayPool<byte>.Shared.Rent(byteCount);

            try
            {
                int byteCount2 = Utf8JsonReader.s_utf8Encoding.GetBytes(jsonChars, utf8Bytes);
                Debug.Assert(byteCount == byteCount2);

                return Parse(utf8Bytes.AsMemory(0, byteCount2), readerOptions, utf8Bytes);
            }
            catch
            {
                // Holds document content, clear it before returning it.
                utf8Bytes.AsSpan(0, byteCount).Clear();
                ArrayPool<byte>.Shared.Return(utf8Bytes);
                throw;
            }
        }

        public static JsonDocument Parse(string json, JsonReaderOptions readerOptions = default)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            CheckSupportedOptions(readerOptions);

            return Parse(json.AsMemory(), readerOptions);
        }

        private static JsonDocument Parse(
            ReadOnlyMemory<byte> utf8Json,
            JsonReaderOptions readerOptions,
            byte[] extraRentedBytes)
        {
            ReadOnlySpan<byte> utf8JsonSpan = utf8Json.Span;
            Utf8JsonReader reader = new Utf8JsonReader(
                utf8JsonSpan,
                isFinalBlock: true,
                new JsonReaderState(options: readerOptions));

            var database = new MetadataDb(utf8Json.Length);
            var stack = new StackRowStack(JsonReaderState.DefaultMaxDepth * StackRow.Size);

            try
            {
                Parse(utf8JsonSpan, reader, ref database, ref stack);

                // TODO(#34155): Remove this if/throw after the reader throws this exception for us.
                if (database.Length == 0)
                {
                    throw new JsonReaderException("Cannot load the empty document", -1, -1);
                }
            }
            catch
            {
                database.Dispose();
                throw;
            }
            finally
            {
                stack.Dispose();
            }

            return new JsonDocument(utf8Json, database, extraRentedBytes);
        }

        private static ArraySegment<byte> ReadToEnd(Stream stream)
        {
            int written = 0;
            byte[] rented = null;

            try
            {
                long expectedLength = 0;

                if (stream.CanSeek)
                {
                    expectedLength = Math.Max(1L, stream.Length - stream.Position);
                    rented = ArrayPool<byte>.Shared.Rent(checked((int)expectedLength));
                }
                else
                {
                    rented = ArrayPool<byte>.Shared.Rent(UnseekableStreamInitialRentSize);
                }

                int lastRead;

                do
                {
                    if (expectedLength == 0 && rented.Length == written)
                    {
                        byte[] toReturn = rented;
                        rented = ArrayPool<byte>.Shared.Rent(checked(toReturn.Length * 2));
                        Buffer.BlockCopy(toReturn, 0, rented, 0, toReturn.Length);
                        // Holds document content, clear it.
                        ArrayPool<byte>.Shared.Return(toReturn, clearArray: true);
                    }

                    lastRead = stream.Read(rented, written, rented.Length - written);
                    written += lastRead;
                } while (lastRead > 0);

                return new ArraySegment<byte>(rented, 0, written);
            }
            catch
            {
                if (rented != null)
                {
                    // Holds document content, clear it before returning it.
                    rented.AsSpan(0, written).Clear();
                    ArrayPool<byte>.Shared.Return(rented);
                }

                throw;
            }
        }

        private static async ValueTask<ArraySegment<byte>> ReadToEndAsync(
            Stream stream,
            CancellationToken cancellationToken)
        {
            int written = 0;
            byte[] rented = null;

            try
            {
                long expectedLength = 0;

                if (stream.CanSeek)
                {
                    expectedLength = Math.Max(1L, stream.Length - stream.Position);
                    rented = ArrayPool<byte>.Shared.Rent(checked((int)expectedLength));
                }
                else
                {
                    rented = ArrayPool<byte>.Shared.Rent(UnseekableStreamInitialRentSize);
                }

                int lastRead;

                do
                {
                    if (expectedLength == 0 && rented.Length == written)
                    {
                        byte[] toReturn = rented;
                        rented = ArrayPool<byte>.Shared.Rent(toReturn.Length * 2);
                        Buffer.BlockCopy(toReturn, 0, rented, 0, toReturn.Length);
                        // Holds document content, clear it.
                        ArrayPool<byte>.Shared.Return(toReturn, clearArray: true);
                    }

                    lastRead = await stream.ReadAsync(
                        rented.AsMemory(written, rented.Length - written),
                        cancellationToken).ConfigureAwait(false);

                    written += lastRead;

                } while (lastRead > 0);

                return new ArraySegment<byte>(rented, 0, written);
            }
            catch
            {
                if (rented != null)
                {
                    // Holds document content, clear it before returning it.
                    rented.AsSpan(0, written).Clear();
                    ArrayPool<byte>.Shared.Return(rented);
                }

                throw;
            }
        }
    }
}
