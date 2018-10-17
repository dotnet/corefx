// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class Utf8StringContent : HttpContent
    {
        private const string DefaultMediaType = "text/plain";

        private readonly Utf8String _content;

        public Utf8StringContent(Utf8String content)
            : this(content, null)
        {
        }

        public Utf8StringContent(Utf8String content, string mediaType)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _content = content;

            // Initialize the 'Content-Type' header with information provided by parameters. 
            MediaTypeHeaderValue headerValue = new MediaTypeHeaderValue(mediaType ?? DefaultMediaType);
            headerValue.CharSet = "utf-8"; // System.Text.Encoding.UTF8.WebName

            Headers.ContentType = headerValue;
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            return Task.FromResult(_content.GetStream());
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Debug.Assert(stream != null);
            return stream.WriteAsync(_content.AsMemoryBytes()).AsTask();
        }

        internal override Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
        {
            Debug.Assert(stream != null);
            return stream.WriteAsync(_content.AsMemoryBytes(), cancellationToken).AsTask();
        }

        protected internal override bool TryComputeLength(out long length)
        {
            length = _content.Length;
            return true;
        }

        internal override Stream TryCreateContentReadStream()
        {
            return _content.GetStream();
        }
    }
}
