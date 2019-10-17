// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.HPack;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace System.Net.Http.Unit.Tests.HPack
{
    public class HPackRoundtripTests
    {

        [Property(Arbitrary = new[] { typeof(HeadersGenerator) }, MaxTest = 100, QuietOnSuccess = true)]
        public void HPack_HeaderEncodeDecodeRoundtrip_ShouldMatchOriginalInput(HttpHeaders headers)
        {
            Memory<byte> encoding = HPackEncode(headers);
            HttpHeaders decodedHeaders = HPackDecode(encoding);

            // Assert: decoded headers are structurally equal to original headers
            Assert.Equal(headers.Count(), decodedHeaders.Count());
            Assert.All(headers.Zip(decodedHeaders), pair =>
            {
                Assert.Equal(pair.First.Key, pair.Second.Key, ignoreCase: true);
                Assert.Equal(pair.First.Value, pair.Second.Value);
            });
        }

        // adapted from Header serialization code in Http2Connection.cs
        private static Memory<byte> HPackEncode(HttpHeaders headers)
        {
            var buffer = new ArrayBuffer(4);
            FillAvailableSpaceWithOnes(buffer);
            string[] headerValues = Array.Empty<string>();

            foreach (KeyValuePair<HeaderDescriptor, HttpHeaders.HeaderStoreItemInfo> header in headers.HeaderStore ?? Enumerable.Empty<KeyValuePair<HeaderDescriptor, HttpHeaders.HeaderStoreItemInfo>>())
            {
                int headerValuesCount = HttpHeaders.GetValuesAsStrings(header.Key, header.Value, ref headerValues);
                Assert.InRange(headerValuesCount, 0, int.MaxValue);
                ReadOnlySpan<string> headerValuesSpan = headerValues.AsSpan(0, headerValuesCount);

                KnownHeader knownHeader = header.Key.KnownHeader;
                if (knownHeader != null)
                {
                    // For all other known headers, send them via their pre-encoded name and the associated value.
                    WriteBytes(knownHeader.Http2EncodedName);
                    string separator = null;
                    if (headerValuesSpan.Length > 1)
                    {
                        HttpHeaderParser parser = header.Key.Parser;
                        if (parser != null && parser.SupportsMultipleValues)
                        {
                            separator = parser.Separator;
                        }
                        else
                        {
                            separator = HttpHeaderParser.DefaultSeparator;
                        }
                    }

                    WriteLiteralHeaderValues(headerValuesSpan, separator);
                }
                else
                {
                    // The header is not known: fall back to just encoding the header name and value(s).
                    WriteLiteralHeader(header.Key.Name, headerValuesSpan);
                }
            }

            return buffer.ActiveMemory;

            void WriteBytes(ReadOnlySpan<byte> bytes)
            {
                if (bytes.Length > buffer.AvailableLength)
                {
                    buffer.EnsureAvailableSpace(bytes.Length);
                    FillAvailableSpaceWithOnes(buffer);
                }

                bytes.CopyTo(buffer.AvailableSpan);
                buffer.Commit(bytes.Length);
            }

            void WriteLiteralHeaderValues(ReadOnlySpan<string> values, string separator)
            {
                int bytesWritten;
                while (!HPackEncoder.EncodeStringLiterals(values, separator, buffer.AvailableSpan, out bytesWritten))
                {
                    buffer.EnsureAvailableSpace(buffer.AvailableLength + 1);
                    FillAvailableSpaceWithOnes(buffer);
                }

                buffer.Commit(bytesWritten);
            }

            void WriteLiteralHeader(string name, ReadOnlySpan<string> values)
            {
                int bytesWritten;
                while (!HPackEncoder.EncodeLiteralHeaderFieldWithoutIndexingNewName(name, values, HttpHeaderParser.DefaultSeparator, buffer.AvailableSpan, out bytesWritten))
                {
                    buffer.EnsureAvailableSpace(buffer.AvailableLength + 1);
                    FillAvailableSpaceWithOnes(buffer);
                }

                buffer.Commit(bytesWritten);
            }

            // force issues related to buffer not being zeroed out
            void FillAvailableSpaceWithOnes(ArrayBuffer buffer) => buffer.AvailableSpan.Fill(0xff);
        }

        // adapted from header deserialization code in Http2Connection.cs
        private static HttpHeaders HPackDecode(Memory<byte> memory)
        {
            var handler = new HeaderHandler();
            var hpackDecoder = new HPackDecoder(maxDynamicTableSize: 0, maxResponseHeadersLength: HttpHandlerDefaults.DefaultMaxResponseHeadersLength * 1024);

            hpackDecoder.Decode(memory.Span, true, handler);

            return handler.Headers;
        }

        private class HeaderHandler : IHttpHeadersHandler
        {
            public HttpHeaders Headers { get; }

            public HeaderHandler()
            {
                Headers = new TestHttpHeaders();
            }

            public void OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
            {
                if (!HeaderDescriptor.TryGet(name, out HeaderDescriptor descriptor))
                {
                    throw new HttpRequestException(SR.Format(SR.net_http_invalid_response_header_name, Encoding.ASCII.GetString(name)));
                }

                string headerValue = descriptor.GetHeaderValue(value);

                Headers.TryAddWithoutValidation(descriptor, headerValue.Split(',').Select(x => x.Trim()));
            }

            public void OnHeadersComplete(bool endStream)
            {
                throw new NotImplementedException();
            }
        }

        private class TestHttpHeaders : HttpHeaders { }

        // FsCheck arbitrary header generator logic
        public static class HeadersGenerator
        {
            public static Arbitrary<HttpHeaders> GenerateHeaders()
            {
                return
                    GenerateHeader()
                    .NonEmptyListOf()
                    .Select(headerValues =>
                    {
                        HttpHeaders headers = new TestHttpHeaders();
                        foreach ((string name, string[] values) in headerValues)
                        {
                            headers.TryAddWithoutValidation(name, values);
                        }
                        return headers;
                    })
                    .ToArbitrary();

                Gen<(string name, string[] values)> GenerateHeader() =>
                    GenerateHeaderName()
                        .Zip(GenerateRandomHttpToken().NonEmptyListOf())
                        .Select(x => (x.Item1, x.Item2.ToArray()));

                Gen<string> GenerateHeaderName() =>
                    Gen.Frequency(
                        new (int, Gen<string>)[]
                        {
                            (60, GenerateStaticHeaderNames()),
                            (40, GenerateRandomHttpToken()),
                        }.Select(x => x.ToTuple()));

                Gen<string> GenerateRandomHttpToken()
                {
                    return Arb.From<NonEmptyString>().Generator.Select(s => Normalize(s.Get));

                    string Normalize(string x) =>
                        HttpUtility
                            .UrlEncode(x)
                            // HttpUtility does not encode parens
                            .Replace("(", "%28")
                            .Replace(")", "%29");
                }

                Gen<string> GenerateStaticHeaderNames() =>
                    // NB uses uniform distribution
                    Gen.Elements(
                        // static table header names
                        "accept-charset",
                        "accept-encoding",
                        "accept-language",
                        "accept-ranges",
                        "accept",
                        "access-control-allow-origin",
                        "age",
                        "allow",
                        "authorization",
                        "cache-control",
                        "content-disposition",
                        "content-encoding",
                        "content-language",
                        "content-length",
                        "content-location",
                        "content-range",
                        "content-type",
                        "cookie",
                        "date",
                        "etag",
                        "expect",
                        "expires",
                        "from",
                        "host",
                        "if-match",
                        "if-modified-since",
                        "if-none-match",
                        "if-range",
                        "if-unmodified-since",
                        "last-modified",
                        "link",
                        "location",
                        "max-forwards",
                        "proxy-authenticate",
                        "proxy-authorization",
                        "range",
                        "referer",
                        "refresh",
                        "retry-after",
                        "server",
                        "set-cookie",
                        "strict-transport-security",
                        "transfer-encoding",
                        "user-agent",
                        "vary",
                        "via",
                        "www-authenticate");
            }
        }
    }
}
