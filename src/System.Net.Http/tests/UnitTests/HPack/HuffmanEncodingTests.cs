using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Huffman = System.Net.Http.HPack.Huffman;

namespace System.Net.Http.Unit.Tests.HPack
{
    public class HuffmanEncodingTests
    {
        [Theory]
        [MemberData(nameof(GetConcatenateHeaderValuesData))]
        public void HuffmanEncode_ConcatenateHeaderValues_RoundTrip(string[] values, string separator, bool lowerCase)
        {
            // Encode and check that our Encode and GetEncodedLength return equal values.
            int expectedEncodedLength = Huffman.GetEncodedLength(values, separator, lowerCase);

            byte[] encodedBuffer = new byte[expectedEncodedLength];
            int encodedLength = Huffman.Encode(values, separator, lowerCase, encodedBuffer);

            Assert.Equal(expectedEncodedLength, encodedLength);

            // Check that our decoded string matches what we expect.
            byte[] decodedBuffer = new byte[1];
            int decodedLength = Huffman.Decode(encodedBuffer, ref decodedBuffer);
            string decodedString = Encoding.ASCII.GetString(decodedBuffer.AsSpan(0, decodedLength));

            string expectedDecodedString = string.Join(separator, values);
            if (lowerCase) expectedDecodedString = expectedDecodedString.ToLowerInvariant();

            Assert.Equal(expectedDecodedString, decodedString);
        }

        public static IEnumerable<object[]> GetConcatenateHeaderValuesData()
        {
            var singleValues =
                from v in new[]
                {
                    "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ=="
                }
                select new string[] { v };

            var permutatedValues =
                from a in new[] { null, "", "A", "A`", "Foo", "Foo```" }
                from b in new[] { null, "", "B", "B`", "Bar", "Bar```" }
                from c in new[] { null, "", "C", "C`", "Baz", "Baz```" }
                select new string[] { a, b, c }.Where(x => x != null).ToArray();

            return
                from v in Enumerable.Concat(singleValues, permutatedValues)
                from separator in new[] { "", ",", ", " }
                from lowerCase in new[] { true, false }
                select new object[]
                {
                    v,
                    separator,
                    lowerCase
                };
        }
    }
}
