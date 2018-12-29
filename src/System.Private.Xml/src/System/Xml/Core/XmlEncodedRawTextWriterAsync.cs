// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Xml
{
    internal partial class XmlEncodedRawTextWriter
    {
        // Flush all characters in the buffer to output and call Flush() on the output object.
        public override async Task FlushAsync()
        {
            CheckAsyncCall();
            await FlushBufferAsync().ConfigureAwait(false);
            await FlushEncoderAsync().ConfigureAwait(false);

            if (stream != null)
            {
                await stream.FlushAsync().ConfigureAwait(false);
            }
            else if (writer != null)
            {
                await writer.FlushAsync().ConfigureAwait(false);
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
                    Debug.Assert(stream != null || writer != null);

                    if (stream != null)
                    {
                        if (trackTextContent)
                        {
                            _charEntityFallback.Reset(_textContentMarks, _lastMarkPos);
                            // reset text content tracking

                            if ((_lastMarkPos & 1) != 0)
                            {
                                // If the previous buffer ended inside a text content we need to preserve that info
                                //   which means the next index to which we write has to be even
                                _textContentMarks[1] = 1;
                                _lastMarkPos = 1;
                            }
                            else
                            {
                                _lastMarkPos = 0;
                            }
                            Debug.Assert(_textContentMarks[0] == 1);
                        }
                        await EncodeCharsAsync(1, bufPos, true).ConfigureAwait(false);
                    }
                    else
                    {
                        // Write text to TextWriter
                        await writer.WriteAsync(buf, 1, bufPos - 1).ConfigureAwait(false);
                    }
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

                // Reset buffer position
                textPos = textPos == bufPos ? 1 : 0;
                attrEndPos = attrEndPos == bufPos ? 1 : 0;
                contentPos = 0;    // Needs to be zero, since overwriting '>' character is no longer possible
                cdataPos = 0;      // Needs to be zero, since overwriting ']]>' characters is no longer possible
                bufPos = 1;        // Buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
                                   // close an empty element or in CDATA section detection of double ]; buf[0] will always be 0
            }
        }

        private async Task EncodeCharsAsync(int startOffset, int endOffset, bool writeAllToStream)
        {
            // Write encoded text to stream
            while (startOffset < endOffset)
            {
                if (_charEntityFallback != null)
                {
                    _charEntityFallback.StartOffset = startOffset;
                }
                encoder.Convert(buf, startOffset, endOffset - startOffset,
                    bufBytes, bufBytesUsed, bufBytes.Length - bufBytesUsed,
                    false, out int chEnc, out int bEnc, out bool _);
                startOffset += chEnc;
                bufBytesUsed += bEnc;
                if (bufBytesUsed >= bufBytes.Length - 16)
                {
                    await stream.WriteAsync(bufBytes, 0, bufBytesUsed).ConfigureAwait(false);
                    bufBytesUsed = 0;
                }
            }
            if (writeAllToStream && bufBytesUsed > 0)
            {
                await stream.WriteAsync(bufBytes, 0, bufBytesUsed).ConfigureAwait(false);
                bufBytesUsed = 0;
            }
        }

        private Task FlushEncoderAsync()
        {
            Debug.Assert(bufPos == 1);
            if (stream != null)
            {
                // decode no chars, just flush
                encoder.Convert(buf, 1, 0, bufBytes, 0, bufBytes.Length,
                    true, out int _, out int bEnc, out bool _);
                if (bEnc != 0)
                {
                    return stream.WriteAsync(bufBytes, 0, bEnc);
                }
            }

            return Task.CompletedTask;
        }
    }
}

