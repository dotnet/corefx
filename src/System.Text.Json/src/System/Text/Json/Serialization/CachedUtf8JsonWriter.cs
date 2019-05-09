// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Text.Json.Serialization
{
    internal sealed class CachedUtf8JsonWriter
    {
        [ThreadStatic]
        private static CachedUtf8JsonWriter s_cachedInstance;

        private readonly Utf8JsonWriter _writer;

#if DEBUG
        private bool _inUse;
#endif

        private CachedUtf8JsonWriter(IBufferWriter<byte> stream, JsonWriterOptions writerOptions)
        {
            _writer = new Utf8JsonWriter(stream, writerOptions);
        }

        public static CachedUtf8JsonWriter Get(IBufferWriter<byte> stream, JsonWriterOptions writerOptions)
        {
            var writer = s_cachedInstance ?? new CachedUtf8JsonWriter(stream, writerOptions);

            // Taken off the thread static
            s_cachedInstance = null;
#if DEBUG
            if (writer._inUse)
            {
                throw new InvalidOperationException("The writer wasn't returned!");
            }

            writer._inUse = true;
#endif
            writer._writer.Reset(stream);
            return writer;
        }

        public static void Return(CachedUtf8JsonWriter writer)
        {
            s_cachedInstance = writer;
            writer._writer.Reset();

#if DEBUG
            writer._inUse = false;
#endif
        }

        public Utf8JsonWriter GetJsonWriter()
        {
            return _writer;
        }
    }
}
