// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Mime;

namespace System.Net.Mail
{
    internal class MailWriter : BaseWriter
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="stream">Underlying stream</param>
        internal MailWriter(Stream stream)
            : base(stream, true)
        // This is the only stream that should encoding leading dots on a line.
        // This way it is done message wide and only once.
        {
        }

        internal override void WriteHeaders(NameValueCollection headers, bool allowUnicode)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            foreach (string key in headers)
            {
                string[] values = headers.GetValues(key);
                foreach (string value in values)
                    WriteHeader(key, value, allowUnicode);
            }
        }

        /// <summary>
        /// Closes underlying stream.
        /// </summary>
        internal override void Close()
        {
            _bufferBuilder.Append(s_crlf);
            Flush(null);
            _stream.Close();
        }

        /// <summary>
        /// Called when the current stream is closed.  Allows us to 
        /// prepare for the next message part.
        /// </summary>
        /// <param name="sender">Sender of the close event</param>
        /// <param name="args">Event args (not used)</param>
        protected override void OnClose(object sender, EventArgs args)
        {
            Diagnostics.Debug.Assert(_contentStream == sender);
            _contentStream.Flush();
            _contentStream = null;
        }
    }
}
