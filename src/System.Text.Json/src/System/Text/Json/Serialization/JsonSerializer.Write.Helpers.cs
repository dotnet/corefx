﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        private static void VerifyValueAndType(object value, Type type)
        {
            if (type == null)
            {
                if (value != null)
                {
                    throw new ArgumentNullException(nameof(type));
                }
            }
            else if (value != null)
            {
                if (!type.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException(SR.Format(SR.DeserializeWrongType, type.FullName, value.GetType().FullName));
                }
            }
        }

        private static void WriteNull(
            ref JsonWriterState writerState,
            IBufferWriter<byte> bufferWriter)
        {
            Utf8JsonWriter writer = new Utf8JsonWriter(bufferWriter, writerState);
            writer.WriteNullValue();
            writer.Flush(true);
        }

        private static byte[] WriteCore(object value, Type type, JsonSerializerOptions options)
        {
            if (options == null)
                options = s_defaultSettings;

            byte[] result;
            var writerState = new JsonWriterState(options.WriterOptions);

            using (var output = new ArrayBufferWriter<byte>(options.EffectiveBufferSize))
            {
                var writer = new Utf8JsonWriter(output, writerState);

                if (value == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    if (type == null)
                        type = value.GetType();

                    WriteStack state = default;
                    JsonClassInfo classInfo = options.GetOrAddClass(type);
                    state.Current.JsonClassInfo = classInfo;
                    state.Current.CurrentValue = value;
                    if (classInfo.ClassType != ClassType.Object)
                    {
                        state.Current.JsonPropertyInfo = classInfo.GetPolicyProperty();
                    }

                    Write(ref writer, -1, options, ref state);
                }

                writer.Flush(isFinalBlock: true);
                result = output.WrittenMemory.ToArray();
            }

            return result;
        }
    }
}
