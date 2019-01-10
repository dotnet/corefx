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
    public struct JsonWriterOptions
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
                return (_optionsMask & 1) != 0;
            }
            set
            {
                if (value)
                    _optionsMask |= 1;
                else
                    _optionsMask &= ~1;
            }
        }

        /// <summary>
        /// Defines whether the <see cref="Utf8JsonWriter"/> should skip structural validation and allow
        /// the user to write invalid JSON, when set to true. If set to false, any attempts to write invalid JSON will result in
        /// a <exception cref="JsonWriterException"/> to be thrown (for example, writing a value within an object
        /// without a property name). If the JSON being written is known to be correct
        /// then skipping validation (by setting it to true) could improve performance.
        /// </summary>
        public bool SkipValidation
        {
            get
            {
                return (_optionsMask & 2) != 0;
            }
            set
            {
                if (value)
                    _optionsMask |= 2;
                else
                    _optionsMask &= ~2;
            }
        }

        internal bool SlowPath => _optionsMask != 2; // Equivalent to: Indented || !SkipValidation;
    }
}
