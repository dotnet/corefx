// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Diagnostics.Contracts;
using System.Text;

namespace System.Net.Http
{
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Represents a multipart/* content. Even if a collection of HttpContent is stored, " +
        "suffix Collection is not appropriate.")]
    public class MultipartContent : HttpContent, IEnumerable<HttpContent>
    {
        #region Fields

        private const string crlf = "\r\n";

        private readonly List<HttpContent> _nestedContent;
        private readonly string _boundary;

        // Temp context for serialization.
        private int _nextContentIndex;
        private Stream _outputStream;

        #endregion Fields

        #region Construction

        public MultipartContent()
            : this("mixed", GetDefaultBoundary())
        { }

        public MultipartContent(string subtype)
            : this(subtype, GetDefaultBoundary())
        { }

        public MultipartContent(string subtype, string boundary)
        {
            if (string.IsNullOrWhiteSpace(subtype))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "subtype");
            }
            Contract.EndContractBlock();
            ValidateBoundary(boundary);

            _boundary = boundary;

            string quotedBoundary = boundary;
            if (!quotedBoundary.StartsWith("\"", StringComparison.Ordinal))
            {
                quotedBoundary = "\"" + quotedBoundary + "\"";
            }

            MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("multipart/" + subtype);
            contentType.Parameters.Add(new NameValueHeaderValue("boundary", quotedBoundary));
            Headers.ContentType = contentType;

            _nestedContent = new List<HttpContent>();
        }

        private static void ValidateBoundary(string boundary)
        {
            // NameValueHeaderValue is too restrictive for boundary.
            // Instead validate it ourselves and then quote it.
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "boundary");
            }

            // RFC 2046 Section 5.1.1
            // boundary := 0*69<bchars> bcharsnospace
            // bchars := bcharsnospace / " "
            // bcharsnospace := DIGIT / ALPHA / "'" / "(" / ")" / "+" / "_" / "," / "-" / "." / "/" / ":" / "=" / "?"
            if (boundary.Length > 70)
            {
                throw new ArgumentOutOfRangeException("boundary", boundary,
                    string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_content_field_too_long, 70));
            }
            // Cannot end with space.
            if (boundary.EndsWith(" ", StringComparison.Ordinal))
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, boundary), "boundary");
            }
            Contract.EndContractBlock();

            string allowedMarks = @"'()+_,-./:=? ";

            foreach (char ch in boundary)
            {
                if (('0' <= ch && ch <= '9') || // Digit.
                    ('a' <= ch && ch <= 'z') || // alpha.
                    ('A' <= ch && ch <= 'Z') || // ALPHA.
                    (allowedMarks.IndexOf(ch) >= 0)) // Marks.
                {
                    // Valid.
                }
                else
                {
                    throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, boundary), "boundary");
                }
            }
        }

        private static string GetDefaultBoundary()
        {
            return Guid.NewGuid().ToString();
        }

        public virtual void Add(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            Contract.EndContractBlock();

            _nestedContent.Add(content);
        }

        #endregion Construction

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (HttpContent content in _nestedContent)
                {
                    content.Dispose();
                }
                _nestedContent.Clear();
            }
            base.Dispose(disposing);
        }

        #endregion Dispose

        #region IEnumerable<HttpContent> Members

        public IEnumerator<HttpContent> GetEnumerator()
        {
            return _nestedContent.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return _nestedContent.GetEnumerator();
        }

        #endregion

        #region Serialization

        // for-each content
        //   write "--" + boundary
        //   for-each content header
        //     write header: header-value
        //   write content.CopyTo[Async]
        // write "--" + boundary + "--"
        // Can't be canceled directly by the user.  If the overall request is canceled 
        // then the stream will be closed an an exception thrown.
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Debug.Assert(stream != null);
            Debug.Assert(_outputStream == null, "Opperation already in progress");
            Debug.Assert(_nextContentIndex == 0, "Opperation already in progress");

            // Keep a local copy in case the operation completes and cleans up synchronously.
            _outputStream = stream;
            _nextContentIndex = 0;

            return SerializeToStreamCoreAsync();
        }

        private async Task SerializeToStreamCoreAsync()
        {
            try
            {
                // Start Boundary, chain everything else.
                string startBoundary = "--" + _boundary + crlf;
                await EncodeStringToStreamAsync(_outputStream, startBoundary).ConfigureAwait(false);

                string dividingBoundary = crlf + startBoundary;
                StringBuilder output = new StringBuilder();

                while (_nextContentIndex < _nestedContent.Count)
                {
                    if (_nextContentIndex != 0)
                    {
                        // First time, don't write dividing boundary.
                        output.Append(dividingBoundary);
                    }

                    HttpContent content = _nestedContent[_nextContentIndex];

                    // Headers
                    foreach (KeyValuePair<string, IEnumerable<string>> headerPair in content.Headers)
                    {
                        output.Append(headerPair.Key + ": " + string.Join(", ", headerPair.Value) + crlf);
                    }

                    output.Append(crlf); // Extra CRLF to end headers (even if there are no headers).

                    await EncodeStringToStreamAsync(_outputStream, output.ToString()).ConfigureAwait(false);
                    _nextContentIndex++; // Next iteration will operate on the next content object.
                    output.Clear();
                    await content.CopyToAsync(_outputStream).ConfigureAwait(false);
                }

                // Write termination bounary
                await EncodeStringToStreamAsync(_outputStream, crlf + "--" + _boundary + "--" + crlf).ConfigureAwait(false);
                CleanupAsync();
            }
            catch (Exception e)
            {
                HandleAsyncException("SerializeToStreamCoreAsync", e);
            }
        }

        private static Task EncodeStringToStreamAsync(Stream stream, string input)
        {
            byte[] buffer = HttpRuleParser.DefaultHttpEncoding.GetBytes(input);
            return stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private void CleanupAsync()
        {
            _outputStream = null;
            _nextContentIndex = 0;
        }

        private void HandleAsyncException(string method, Exception ex)
        {
            if (Logging.On)
            {
                Logging.Exception(Logging.Http, this, method, ex);
            }
            CleanupAsync();
        }

        protected internal override bool TryComputeLength(out long length)
        {
            long currentLength = 0;
            long internalBoundaryLength = GetEncodedLength(crlf + "--" + _boundary + crlf);

            // Start Boundary.
            currentLength += GetEncodedLength("--" + _boundary + crlf);

            bool first = true;
            foreach (HttpContent content in _nestedContent)
            {
                if (first)
                {
                    first = false; // First boundary already written.
                }
                else
                {
                    // Internal Boundary.
                    currentLength += internalBoundaryLength;
                }

                // Headers.
                foreach (KeyValuePair<string, IEnumerable<string>> headerPair in content.Headers)
                {
                    currentLength += GetEncodedLength(headerPair.Key + ": " + string.Join(", ", headerPair.Value) + crlf);
                }

                currentLength += crlf.Length;

                // Content.
                long tempContentLength = 0;
                if (!content.TryComputeLength(out tempContentLength))
                {
                    length = 0;
                    return false;
                }
                currentLength += tempContentLength;
            }

            // Terminating boundary.
            currentLength += GetEncodedLength(crlf + "--" + _boundary + "--" + crlf);

            length = currentLength;
            return true;
        }

        private static int GetEncodedLength(string input)
        {
            return HttpRuleParser.DefaultHttpEncoding.GetByteCount(input);
        }
        #endregion Serialization
    }
}
