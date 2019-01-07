// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    public struct JsonWriterOptions
    {
        private int _optionsMask;

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
