// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    /// Provides the ability for the user to define custom behavior when writing JSON
    /// using the <see cref="Utf8JsonWriter"/>. By default, the JSON is written without
    /// any indentation or extra white space. Also, the <see cref="Utf8JsonWriter"/> will
    /// throw an exception if the user attempts to write structurally invalid JSON.
    /// </summary>
    public struct JsonWriterOptions : IEquatable<JsonWriterOptions>
    {
        private int _optionsMask;

        /// <summary>
        /// Defines whether the <see cref="Utf8JsonWriter"/> should pretty print the JSON which includes:
        /// indenting nested JSON tokens, adding new lines, and adding white space between property names and values.
        /// By default, the JSON is written without any extra white space.
        /// </summary>
        public bool Indented
        {
            get
            {
                return (_optionsMask & IndentBit) != 0;
            }
            set
            {
                if (value)
                    _optionsMask |= IndentBit;
                else
                    _optionsMask &= ~IndentBit;
            }
        }

        /// <summary>
        /// Defines whether the <see cref="Utf8JsonWriter"/> should skip structural validation and allow
        /// the user to write invalid JSON, when set to true. If set to false, any attempts to write invalid JSON will result in
        /// a <exception cref="InvalidOperationException"/> to be thrown.
        /// </summary>
        /// <remarks>
        /// If the JSON being written is known to be correct,
        /// then skipping validation (by setting it to true) could improve performance.
        /// An example of invalid JSON where the writer will throw (when SkipValidation
        /// is set to false) is when you write a value within a JSON object
        /// without a property name. 
        /// </remarks>
        public bool SkipValidation
        {
            get
            {
                return (_optionsMask & SkipValidationBit) != 0;
            }
            set
            {
                if (value)
                    _optionsMask |= SkipValidationBit;
                else
                    _optionsMask &= ~SkipValidationBit;
            }
        }

        internal bool IndentedOrNotSkipValidation => _optionsMask != SkipValidationBit; // Equivalent to: Indented || !SkipValidation;

        private const int IndentBit = 1;
        private const int SkipValidationBit = 2;

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object
        /// </summary>
        /// <param name="other">The object to compare with this instance</param>
        /// <returns>true if other is an instance of <see cref="JsonWriterOptions"/> and equals the value of the instance; otherwise, false</returns>
        public bool Equals(JsonWriterOptions other)
        {
            return _optionsMask == other._optionsMask;
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object
        /// </summary>
        /// <param name="obj">The object to compare with this instance</param>
        /// <returns>true if obj is an instance of <see cref="JsonWriterOptions"/> and equals the value of the instance; otherwise, false</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is JsonWriterOptions))
            {
                return false;
            }

            return Equals((JsonWriterOptions)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return _optionsMask.GetHashCode();
        }
    }
}
