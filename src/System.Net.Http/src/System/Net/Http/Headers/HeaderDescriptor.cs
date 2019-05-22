// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;

namespace System.Net.Http.Headers
{
    // This struct represents a particular named header --
    // if the header is one of our known headers, then it contains a reference to the KnownHeader object;
    // otherwise, for custom headers, it just contains a string for the header name.
    // Use HeaderDescriptor.TryGet to resolve an arbitrary header name to a HeaderDescriptor.
    internal readonly struct HeaderDescriptor : IEquatable<HeaderDescriptor>
    {
        private static readonly Encoding s_utf8DecoderWithExceptionFallback = Encoding.GetEncoding("utf-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);

        private readonly string _headerName;
        private readonly KnownHeader _knownHeader;

        public HeaderDescriptor(KnownHeader knownHeader)
        {
            _knownHeader = knownHeader;
            _headerName = knownHeader.Name;
        }

        // This should not be used directly; use static TryGet below
        private HeaderDescriptor(string headerName)
        {
            _headerName = headerName;
            _knownHeader = null;
        }

        public string Name => _headerName;
        public HttpHeaderParser Parser => _knownHeader?.Parser;
        public HttpHeaderType HeaderType => _knownHeader == null ? HttpHeaderType.Custom : _knownHeader.HeaderType;
        public KnownHeader KnownHeader => _knownHeader;

        public bool Equals(HeaderDescriptor other) =>
            _knownHeader == null ?
                string.Equals(_headerName, other._headerName, StringComparison.OrdinalIgnoreCase) :
                _knownHeader == other._knownHeader;
        public override int GetHashCode() => _knownHeader?.GetHashCode() ?? StringComparer.OrdinalIgnoreCase.GetHashCode(_headerName);
        public override bool Equals(object obj) => throw new InvalidOperationException();   // Ensure this is never called, to avoid boxing

        // Returns false for invalid header name.
        public static bool TryGet(string headerName, out HeaderDescriptor descriptor)
        {
            Debug.Assert(!string.IsNullOrEmpty(headerName));

            KnownHeader knownHeader = KnownHeaders.TryGetKnownHeader(headerName);
            if (knownHeader != null)
            {
                descriptor = new HeaderDescriptor(knownHeader);
                return true;
            }

            if (!HttpRuleParser.IsToken(headerName))
            {
                descriptor = default(HeaderDescriptor);
                return false;
            }

            descriptor = new HeaderDescriptor(headerName);
            return true;
        }

        // Returns false for invalid header name.
        public static bool TryGet(ReadOnlySpan<byte> headerName, out HeaderDescriptor descriptor)
        {
            Debug.Assert(headerName.Length > 0);

            KnownHeader knownHeader = KnownHeaders.TryGetKnownHeader(headerName);
            if (knownHeader != null)
            {
                descriptor = new HeaderDescriptor(knownHeader);
                return true;
            }

            if (!HttpRuleParser.IsToken(headerName))
            {
                descriptor = default(HeaderDescriptor);
                return false;
            }

            descriptor = new HeaderDescriptor(HttpRuleParser.GetTokenString(headerName));
            return true;
        }

        public HeaderDescriptor AsCustomHeader()
        {
            Debug.Assert(_knownHeader != null);
            Debug.Assert(_knownHeader.HeaderType != HttpHeaderType.Custom);
            return new HeaderDescriptor(_knownHeader.Name);
        }

        public string GetHeaderValue(ReadOnlySpan<byte> headerValue)
        {
            if (headerValue.Length == 0)
            {
                return string.Empty;
            }

            // If it's a known header value, use the known value instead of allocating a new string.
            if (_knownHeader != null)
            {
                if (_knownHeader.KnownValues != null)
                {
                    string[] knownValues = _knownHeader.KnownValues;
                    for (int i = 0; i < knownValues.Length; i++)
                    {
                        if (ByteArrayHelpers.EqualsOrdinalAsciiIgnoreCase(knownValues[i], headerValue))
                        {
                            return knownValues[i];
                        }
                    }
                }

                if (KnownHeader == KnownHeaders.Location)
                {
                    // Normally Location should be in ISO-8859-1, ocassionally some servers respond with UTF-8 though
                    if (TryDecodeUtf8(headerValue, out string decoded))
                    {
                        return decoded;
                    }
                }
            }

            return HttpRuleParser.DefaultHttpEncoding.GetString(headerValue);
        }

        private static bool TryDecodeUtf8(ReadOnlySpan<byte> input, out string decoded)
        {
            // TODO: Utilize vectorization helpers here if/when they're made public
            bool possibleUtf8 = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] > 127)
                {
                    possibleUtf8 = true;
                    break;
                }
            }

            if (possibleUtf8)
            {
                try
                {
                    decoded = s_utf8DecoderWithExceptionFallback.GetString(input);
                    return true;
                }
                catch (ArgumentException) { } // Not actually Utf-8
            }

            decoded = null;
            return false;
        }
    }
}
