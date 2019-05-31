// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Write the provided value into the provided writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value to convert.</param>
        /// <param name="options">Options to control the serializer behavior while writing.</param>
        /// <remarks>
        /// The <see cref="JsonWriterOptions"/> used to create the instance of the <see cref="Utf8JsonWriter"/> take precedence over the <see cref="JsonSerializerOptions"/> when they conflict.
        /// Hence, <see cref="JsonWriterOptions.Indented"/>, is used while writing.
        /// </remarks>
        public static void WriteValue<TValue>(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options = null)
        {
            WriteCore(writer, value, typeof(TValue), options);
        }

        /// <summary>
        /// Write the provided value into the provided writer as a JSON value.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value to convert.</param>
        /// <param name="type">The type of the <paramref name="value"/> to convert.</param>
        /// <param name="options">Options to control the serializer behavior while writing.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="type"/> is null while <paramref name="value"/> is not.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the type of <paramref name="type"/> and <paramref name="value"/> do not match.
        /// </exception>
        /// <remarks>
        /// The <see cref="JsonWriterOptions"/> used to create the instance of the <see cref="Utf8JsonWriter"/> take precedence over the <see cref="JsonSerializerOptions"/> when they conflict.
        /// Hence, <see cref="JsonWriterOptions.Indented"/>, is used while writing.
        /// </remarks>
        public static void WriteValue(Utf8JsonWriter writer, object value, Type type, JsonSerializerOptions options = null)
        {
            VerifyValueAndType(value, type);
            WriteCore(writer, value, type, options);
        }

        private static void WriteCore(Utf8JsonWriter writer, object value, Type type, JsonSerializerOptions options)
        {
            if (options == null)
            {
                options = JsonSerializerOptions.s_defaultOptions;
            }

            Debug.Assert(type != null || value == null);

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                //  We treat typeof(object) special and allow polymorphic behavior.
                if (type == typeof(object))
                {
                    type = value.GetType();
                }

                WriteStack state = default;
                state.Current.Initialize(type, options);
                state.Current.CurrentValue = value;

                Write(writer, -1, options, ref state);
            }

            writer.Flush();
        }
    }
}
