// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Concurrent;

namespace System.Text.Json.Serialization
{
    internal sealed class CachedUtf8JsonWriter
    {
        private static readonly ConcurrentDictionary<int, Utf8JsonWriter> _cache = new ConcurrentDictionary<int, Utf8JsonWriter>();

        public static Utf8JsonWriter Get(IBufferWriter<byte> output, JsonWriterOptions writerOptions)
        {
            int writerOptionsHash = writerOptions.GetHashCode();

            if (!_cache.TryGetValue(writerOptionsHash, out Utf8JsonWriter writer))
            {
                writer = new Utf8JsonWriter(output, writerOptions);
                _cache.TryAdd(writerOptionsHash, writer);
            }

            writer.Reset(output);
            return writer;
        }
    }
}
