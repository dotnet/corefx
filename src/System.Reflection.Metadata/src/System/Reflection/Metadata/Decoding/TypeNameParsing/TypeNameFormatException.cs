// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    internal class TypeNameFormatException : FormatException
    {
        private readonly TypeNameFormatErrorId _errorId;
        private readonly int _position;        

        public TypeNameFormatException(string message, TypeNameFormatErrorId errorId, int position)
            : base(message)
        {
            Debug.Assert(position > 0);

            _errorId = errorId;
            _position = position;            
        }

        public TypeNameFormatErrorId ErrorId
        {
            get { return _errorId; }
        }

        public int Position
        {
            get { return _position; }
        }
    }
}
