// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.IO
{
    /// <summary> 
    /// Helper for reading config files where each row is a key-value data pair.
    /// The input key-values must not have any whitespace within them.
    /// </summary>
    internal struct RowConfigReader
    {
        private readonly string _buffer;
        private readonly StringComparison _comparisonKind;
        private int _currentIndex;

        /// <summary>
        /// Constructs a new RowConfigReader which reads from the given string.
        /// <param name="buffer">The string to parse through.</param>
        /// </summary>
        public RowConfigReader(string buffer)
        {
            _buffer = buffer;
            _comparisonKind = StringComparison.Ordinal;
            _currentIndex = 0;
        }

        /// <summary>
        /// Constructs a new RowConfigReader which reads from the given string.
        /// <param name="buffer">The string to parse through.</param>
        /// <param name="comparisonKind">The comparison kind to use.</param>
        /// </summary>
        public RowConfigReader(string buffer, StringComparison comparisonKind)
        {
            _buffer = buffer;
            _comparisonKind = comparisonKind;
            _currentIndex = 0;
        }

        /// <summary>
        /// Gets the next occurrence of the given key, from the current position of the reader,
        /// or throws if no occurrence of the key exists in the remainder of the string.
        /// </summary>
        public string GetNextValue(string key)
        {
            string value;
            if (!TryGetNextValue(key, out value))
            {
                throw new InvalidOperationException("Couldn't get next value with key " + key);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Tries to get the next occurrence of the given key from the current position of the reader.
        /// If successful, returns true and stores the result in 'value'. Otherwise, returns false.
        /// </summary>
        public bool TryGetNextValue(string key, out string value)
        {
            Debug.Assert(_buffer != null);
            if (_currentIndex >= _buffer.Length)
            {
                value = null;
                return false;
            }

            // First, find the key
            int keyIndex = _buffer.IndexOf(key, _currentIndex, _comparisonKind);
            if (keyIndex == -1)
            {
                value = null;
                return false;
            }

            // Next, we will take the end of the line, and look backwards for the start of the value.
            // NOTE: This assumes that the "value" does not have any whitespace in it, nor is there any
            // after. This is the format of most "row-based" config files in /proc/net, etc.
            int afterKey = keyIndex + key.Length;
            int endOfLine = _buffer.IndexOf(Environment.NewLine, afterKey, _comparisonKind);
            Debug.Assert(endOfLine != -1, "RowConfigReader needs a newline after the key, and one was not found.");

            int valueIndex = _buffer.LastIndexOf('\t', endOfLine);
            if (valueIndex == -1)
            {
                valueIndex = _buffer.LastIndexOf(' ', endOfLine); // try space as well
            }

            Debug.Assert(valueIndex != -1, "Key " + key + " was found, but no value on the same line.");
            valueIndex++; // Get the first character after the whitespace.
            value = _buffer.Substring(valueIndex, endOfLine - valueIndex); // Grab the whole value string.

            _currentIndex = endOfLine + 1;
            return true;
        }

        /// <summary>
        /// Gets the next occurence of the key in the string, and parses it as an Int32.
        /// Throws if the key is not found in the remainder of the string, or if the key
        /// cannot be succesfully parsed into an Int32.
        /// </summary>
        /// <remarks>
        /// This is mainly provided as a helper because most Linux config/info files
        /// store integral data.
        /// </remarks>
        public int GetNextValueAsInt32(string key)
        {
            // PERF: We don't need to allocate a new string here, we can parse an Int32 "in-place" in the existing string.
            string value = GetNextValue(key);
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException("Unable to parse value " + value + " of key " + key + " as an Int32.");
            }
        }

        /// <summary>
        /// Reads the value of the first occurrence of the given key contained in the string given.
        /// </summary>
        /// <param name="data">The key-value row configuration string.</param>
        /// <param name="key">The key to find.</param>
        /// <returns>The value of the row containing the first occurrance of the key.</returns>
        public static string ReadFirstValueFromString(string data, string key)
        {
            return new RowConfigReader(data).GetNextValue(key);
        }
    }
}
