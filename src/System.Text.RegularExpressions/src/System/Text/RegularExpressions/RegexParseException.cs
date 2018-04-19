// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    internal class RegexParseException : ArgumentException
    {
        private readonly RegexParseError _error;

        public RegexParseError Error => _error;

        public RegexParseException(RegexParseError error, string message) : base(message)
        {
            _error = error;
        }

        public RegexParseException() : base()
        {
        }

        public RegexParseException(string message) : base(message)
        {
        }

        public RegexParseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
