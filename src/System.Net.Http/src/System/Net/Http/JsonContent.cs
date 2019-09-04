// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class JsonContent : StringContent
    {
        private const string DefaultMediaType = "application/json";
        
        public JsonContent(string content)
            : this(content, null, null)
        {
        }

        public JsonContent(string content, Encoding encoding)
            : this(content, encoding, null)
        {
        }

        public JsonContent(string content, Encoding encoding, string mediaType)
            : base(content,encoding,mediaType)
        {
            // Initialize the 'Content-Type' header with information provided by parameters.
            MediaTypeHeaderValue headerValue = new MediaTypeHeaderValue((mediaType == null) ? DefaultMediaType : mediaType);
            headerValue.CharSet = (encoding == null) ? HttpContent.DefaultStringEncoding.WebName : encoding.WebName;

            Headers.ContentType = headerValue;
        }
        
        internal override Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken) =>
            // Only skip the original protected virtual SerializeToStreamAsync if this
            // isn't a derived type that may have overridden the behavior.
            GetType() == typeof(JsonContent) ? SerializeToStreamAsyncCore(stream, cancellationToken) :
            base.SerializeToStreamAsync(stream, context, cancellationToken);

        internal override Stream TryCreateContentReadStream() =>
            GetType() == typeof(JsonContent) ? CreateMemoryStreamForByteArray() : // type check ensures we use possible derived type's CreateContentReadStreamAsync override
            null;
    }
}
