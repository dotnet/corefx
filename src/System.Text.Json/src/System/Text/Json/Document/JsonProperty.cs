// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Text.Json
{
    /// <summary>
    ///   Represents a single property for a JSON object.
    /// </summary>
    public readonly struct JsonProperty
    {
        /// <summary>
        ///   The value of this property.
        /// </summary>
        public JsonElement Value { get; }

        internal JsonProperty(JsonElement value)
        {
            Value = value;
        }

        /// <summary>
        ///   The name of this property.
        /// </summary>
        public string Name => Value.GetPropertyName();

        /// <summary>
        ///   Compares <paramref name="text" /> to the property name.
        /// </summary>
        /// <param name="text">The text to compare against.</param>
        /// <returns>
        ///   <see langword="true" /> if the property name matches <paramref name="text"/>,
        ///   <see langword="false" /> otherwise.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   This value's <see cref="Type"/> is not <see cref="JsonTokenType.PropertyName"/>.
        /// </exception>
        public bool NameEquals(string text)
        {
            return NameEquals(text.AsSpan());
        }

        /// <summary>
        ///   Compares the text represented by <paramref name="utf8Text" /> to the property name.
        /// </summary>
        /// <param name="utf8Text">The UTF-8 encoded text to compare against.</param>
        /// <returns>
        ///   <see langword="true" /> if the property name has the same UTF-8 encoding as
        ///   <paramref name="utf8Text" />, <see langword="false" /> otherwise.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   This value's <see cref="Type"/> is not <see cref="JsonTokenType.PropertyName"/>.
        /// </exception>
        public bool NameEquals(ReadOnlySpan<byte> utf8Text)
        {
            return Value.TextEqualsHelper(utf8Text, isPropertyName: true);
        }

        /// <summary>
        ///   Compares <paramref name="text" /> to the property name.
        /// </summary>
        /// <param name="text">The text to compare against.</param>
        /// <returns>
        ///   <see langword="true" /> if the property name matches <paramref name="text"/>,
        ///   <see langword="false" /> otherwise.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///   This value's <see cref="Type"/> is not <see cref="JsonTokenType.PropertyName"/>.
        /// </exception>
        public bool NameEquals(ReadOnlySpan<char> text)
        {
            return Value.TextEqualsHelper(text, isPropertyName: true);
        }

        /// <summary>
        ///   Provides a <see cref="string"/> representation of the property for
        ///   debugging purposes.
        /// </summary>
        /// <returns>
        ///   A string containing the un-interpreted value of the property, beginning
        ///   at the declaring open-quote and ending at the last character that is part of
        ///   the value.
        /// </returns>
        public override string ToString()
        {
            return Value.GetPropertyRawText();
        }
    }
}
