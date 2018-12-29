// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Xml
{
    internal partial class XmlUtf8RawTextWriter
    {
        // Flush all characters in the buffer to output and call Flush() on the output object.
        public override async Task FlushAsync()
        {
            CheckAsyncCall();
            await FlushBufferAsync().ConfigureAwait(false);

            if (stream != null)
            {
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }

        // Flush all characters in the buffer to output.  Do not flush the output object.
        protected override async Task FlushBufferAsync()
        {
            try
            {
                // Output all characters (except for previous characters stored at beginning of buffer)
                if (!writeToNull)
                {
                    Debug.Assert(stream != null);
                    await stream.WriteAsync(buf, 1, bufPos - 1).ConfigureAwait(false);
                }
            }
            catch
            {
                // Future calls to flush (i.e. when Close() is called) don't attempt to write to stream
                writeToNull = true;
                throw;
            }
            finally
            {
                // Move last buffer character to the beginning of the buffer (so that previous character can always be determined)
                buf[0] = buf[bufPos - 1];

                if (IsSurrogateByte(buf[0]))
                {
                    // Last character was the first byte in a surrogate encoding, so move last three
                    // bytes of encoding to the beginning of the buffer.
                    buf[1] = buf[bufPos];
                    buf[2] = buf[bufPos + 1];
                    buf[3] = buf[bufPos + 2];
                }

                // Reset buffer position
                textPos = (textPos == bufPos) ? 1 : 0;
                attrEndPos = (attrEndPos == bufPos) ? 1 : 0;
                contentPos = 0;    // Needs to be zero, since overwriting '>' character is no longer possible
                cdataPos = 0;      // Needs to be zero, since overwriting ']]>' characters is no longer possible
                bufPos = 1;        // Buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
                                   // close an empty element or in CDATA section detection of double ]; buf[0] will always be 0
            }
        }
    }
}

