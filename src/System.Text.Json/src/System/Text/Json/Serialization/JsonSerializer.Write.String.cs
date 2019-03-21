// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Convert the value and return as <see cref="System.String"/>.
        /// </summary>
        /// <returns>The converted value.</returns>
        /// <param name="value">The value to convert.</param>
        /// <param name="options">The options used to convert the value.</param>
        /// <remarks>Using a UTF-16 <see cref="System.String"/> is not as efficient as a
        /// UTF-8 <see cref="System.Byte"/> array since the implementation natively uses UTF-8 and
        /// requires a conversion to a UTF-16 <see cref="System.String"/>.
        /// </remarks>
        public static string ToString<TValue>(TValue value, JsonSerializerOptions options = null)
        {
            return ToStringInternal(value, typeof(TValue), options);
        }

        /// <summary>
        /// Convert the value and return as <see cref="System.String"/>.
        /// </summary>
        /// <returns>The converted value.</returns>
        /// <param name="value">The value to convert.</param>
        /// <param name="type">The type of the <paramref name="value"/> to convert.</param>
        /// <param name="options">The options used to convert the value.</param>
        /// <remarks>Using a UTF-16 <see cref="System.String"/> is not as efficient as a
        /// UTF-8 <see cref="System.Byte"/> array since the implementation natively uses UTF-8 and
        /// requires a conversion to a UTF-16 <see cref="System.String"/>.
        /// </remarks>
        public static string ToString(object value, Type type, JsonSerializerOptions options = null)
        {
            VerifyValueAndType(value, type);

            return ToStringInternal(value, type, options);
        }

        private static string ToStringInternal(object value, Type type, JsonSerializerOptions options)
        {
            return WriteCoreString(value, type, options);
        }
    }
}
