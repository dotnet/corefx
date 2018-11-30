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
                utf8Bytes.AsSpan(0, length).Clear();
                ArrayPool<byte>.Shared.Return(utf8Bytes);
                throw;
            }
        }

        public static JsonDocument Parse(Stream utf8Json, JsonReaderOptions readerOptions = default)
        {
            if (utf8Json == null)
                throw new ArgumentNullException(nameof(utf8Json));

            CheckSupportedOptions(readerOptions);

            ArraySegment<byte> drained = ReadToEnd(utf8Json);

            try
            {
                return Parse(drained.AsMemory(), readerOptions, drained.Array);
            }
            catch
            {
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
                throw new ArgumentNullException(nameof(utf8Json));

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
                drained.AsSpan().Clear();
                ArrayPool<byte>.Shared.Return(drained.Array);
                throw;
            }
        }

        public static JsonDocument Parse(ReadOnlyMemory<char> json, JsonReaderOptions readerOptions = default)
        {
            CheckSupportedOptions(readerOptions);

            ReadOnlySpan<char> jsonChars = json.Span;
            int byteCount = Utf8JsonReader.Utf8Encoding.GetByteCount(jsonChars);
            byte[] utf8Bytes = ArrayPool<byte>.Shared.Rent(byteCount);

            try
            {
                int byteCount2 = Utf8JsonReader.Utf8Encoding.GetBytes(jsonChars, utf8Bytes);
                Debug.Assert(byteCount == byteCount2);

                return Parse(utf8Bytes.AsMemory(0, byteCount2), readerOptions, utf8Bytes);
            }
            catch
            {
                utf8Bytes.AsSpan(0, byteCount).Clear();
                ArrayPool<byte>.Shared.Return(utf8Bytes);
                throw;
            }
        }

        public static JsonDocument Parse(string json, JsonReaderOptions readerOptions = default)
        {
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
                new JsonReaderState(JsonReaderState.DefaultMaxDepth, readerOptions));

            var database = new CustomDb(utf8Json.Length);
            var stack = new CustomStack(JsonReaderState.DefaultMaxDepth * StackRow.Size);

            try
            {
                Parse(utf8JsonSpan, reader, ref database, ref stack);

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
            ArrayPool<byte> pool = ArrayPool<byte>.Shared;

            try
            {
                long expectedLength = 0;

                if (stream.CanSeek)
                {
                    expectedLength = Math.Max(1L, stream.Length - stream.Position);
                    rented = pool.Rent(checked((int)expectedLength));
                }
                else
                {
                    rented = pool.Rent(UnseekableStreamInitialRentSize);
                }

                int lastRead;

                do
                {
                    if (expectedLength == 0 && rented.Length == written)
                    {
                        byte[] tmp = pool.Rent(rented.Length * 2);
                        Buffer.BlockCopy(rented, 0, tmp, 0, rented.Length);
                        rented.AsSpan().Clear();
                        pool.Return(rented);
                        rented = tmp;
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
                    rented.AsSpan(0, written).Clear();
                    pool.Return(rented);
                }

                throw;
            }
        }

        private static async Task<ArraySegment<byte>> ReadToEndAsync(
            Stream stream,
            CancellationToken cancellationToken)
        {
            int written = 0;
            byte[] rented = null;
            ArrayPool<byte> pool = ArrayPool<byte>.Shared;

            try
            {
                long expectedLength = 0;

                if (stream.CanSeek)
                {
                    expectedLength = Math.Max(1L, stream.Length - stream.Position);
                    rented = pool.Rent(checked((int)expectedLength));
                }
                else
                {
                    rented = pool.Rent(UnseekableStreamInitialRentSize);
                }

                int lastRead;

                do
                {
                    if (expectedLength == 0 && rented.Length == written)
                    {
                        byte[] tmp = pool.Rent(rented.Length * 2);
                        Buffer.BlockCopy(rented, 0, tmp, 0, rented.Length);
                        rented.AsSpan().Clear();
                        pool.Return(rented);
                        rented = tmp;
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
                    rented.AsSpan(0, written).Clear();
                    pool.Return(rented);
                }

                throw;
            }
        }
    }
}
