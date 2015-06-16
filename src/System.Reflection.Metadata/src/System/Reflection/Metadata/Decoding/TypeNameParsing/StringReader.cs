﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    // Simple, cheap, forward-only string reader
    internal class StringReader
    {
        private readonly string _input;
        private int _position;

        public StringReader(string input)
            : this(input, 0)
        {
        }

        private StringReader(string input, int startIndex)
        {
            Debug.Assert(input != null);
            Debug.Assert(input.Length > 0);
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex <= input.Length);

            _input = input;
            _position = startIndex;
        }

        public bool CanRead
        {
            get
            {
                if (_position < _input.Length)
                {
                    // Treat null as end of string
                    return PeekChar() != '\0';
                }

                return false;
            }
        }

        public int Position
        {
            get { return _position; }
        }

        public char Peek()
        {
            Debug.Assert(CanRead);

            return PeekChar();
        }

        public char Read()
        {
            char c = Peek();

            _position++;

            return c;
        }

        private char PeekChar()
        {
            return _input[_position];
        }

        public StringReader Clone()
        {
            return new StringReader(_input, _position);
        }
    }
}
