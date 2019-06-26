// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0.
// See THIRD-PARTY-NOTICES.TXT in the project root for license information.

using System.Diagnostics;
using System.Numerics;

namespace System.Net.Http.HPack
{
    internal class IntegerDecoder
    {
        private int _i;
        private int _m;

        public int Value { get; private set; }

        /// <summary>
        /// Decodes the first byte of the integer.
        /// </summary>
        /// <param name="prefixLength">The length of the prefix, in bits, that the integer was encoded with. Must be between 1 and 8.</param>
        /// <returns>If the integer has been fully decoded, true. Otherwise, false -- <see cref="Decode(byte)"/> must be called on subsequent bytes.</returns>
        public bool StartDecode(byte b, int prefixLength)
        {
            Debug.Assert(prefixLength >= 1 && prefixLength <= 8);

            if (b < ((1 << prefixLength) - 1))
            {
                Value = b;
                return true;
            }
            else
            {
                _i = b;
                _m = 0;
                return false;
            }
        }

        /// <summary>
        /// Decodes subsequent bytes of an integer.
        /// </summary>
        /// <returns>If the integer has been fully decoded, true. Otherwise, false -- <see cref="Decode(byte)"/> must be called on subsequent bytes.</returns>
        public bool Decode(byte b)
        {
            if (_m - BitOperations.LeadingZeroCount(b) > 0)
            {
                // A bit would be shifted beyond 31 bits.
                throw new HPackDecodingException(SR.net_http_hpack_bad_integer);
            }

            _i = _i + ((b & 127) << _m);
            _m = _m + 7;

            if (_i < 0)
            {
                // Addition wrapped.
                throw new HPackDecodingException(SR.net_http_hpack_bad_integer);
            }

            if ((b & 128) == 0)
            {
                if (b == 0 && _m / 7 > 1)
                {
                    // Do not accept overlong encodings.
                    throw new HPackDecodingException(SR.net_http_hpack_bad_integer);
                }

                Value = _i;
                return true;
            }

            return false;
        }
    }
}
