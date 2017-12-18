// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Threading.Tasks;

namespace System.Xml
{
    // Concrete implementation of XmlWriter abstract class that serializes events as encoded XML
    // text.  The general-purpose XmlEncodedTextWriter uses the Encoder class to output to any
    // encoding.  The XmlUtf8TextWriter class combined the encoding operation with serialization
    // in order to achieve better performance.
    internal partial class XmlEncodedRawTextWriter : XmlRawWriter
    {
        protected void CheckAsyncCall()
        {
            if (!_useAsync)
            {
                throw new InvalidOperationException(SR.Xml_WriterAsyncNotSetException);
            }
        }

        // Write the xml declaration.  This must be the first call.  
        internal override async Task WriteXmlDeclarationAsync(XmlStandalone standalone)
        {
            CheckAsyncCall();
            // Output xml declaration only if user allows it and it was not already output
            if (!omitXmlDeclaration && !autoXmlDeclaration)
            {
                if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

                await RawTextAsync("<?xml version=\"").ConfigureAwait(false);

                // Version
                await RawTextAsync("1.0").ConfigureAwait(false);

                // Encoding
                if (encoding != null)
                {
                    await RawTextAsync("\" encoding=\"").ConfigureAwait(false);
                    await RawTextAsync(encoding.WebName).ConfigureAwait(false);
                }

                // Standalone
                if (standalone != XmlStandalone.Omit)
                {
                    await RawTextAsync("\" standalone=\"").ConfigureAwait(false);
                    await RawTextAsync(standalone == XmlStandalone.Yes ? "yes" : "no").ConfigureAwait(false);
                }

                await RawTextAsync("\"?>").ConfigureAwait(false);
            }
        }

        internal override Task WriteXmlDeclarationAsync(string xmldecl)
        {
            CheckAsyncCall();
            // Output xml declaration only if user allows it and it was not already output
            if (!omitXmlDeclaration && !autoXmlDeclaration)
            {
                return WriteProcessingInstructionAsync("xml", xmldecl);
            }

            return Task.CompletedTask;
        }

        // Serialize the document type declaration.
        public override async Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
        {
            CheckAsyncCall();
            Debug.Assert(name != null && name.Length > 0);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            await RawTextAsync("<!DOCTYPE ").ConfigureAwait(false);
            await RawTextAsync(name).ConfigureAwait(false);
            if (pubid != null)
            {
                await RawTextAsync(" PUBLIC \"").ConfigureAwait(false);
                await RawTextAsync(pubid).ConfigureAwait(false);
                await RawTextAsync("\" \"").ConfigureAwait(false);
                if (sysid != null)
                {
                    await RawTextAsync(sysid).ConfigureAwait(false);
                }
                bufChars[bufPos++] = (char)'"';
            }
            else if (sysid != null)
            {
                await RawTextAsync(" SYSTEM \"").ConfigureAwait(false);
                await RawTextAsync(sysid).ConfigureAwait(false);
                bufChars[bufPos++] = (char)'"';
            }
            else
            {
                bufChars[bufPos++] = (char)' ';
            }

            if (subset != null)
            {
                bufChars[bufPos++] = (char)'[';
                await RawTextAsync(subset).ConfigureAwait(false);
                bufChars[bufPos++] = (char)']';
            }

            bufChars[this.bufPos++] = (char)'>';
        }

        // Serialize the beginning of an element start tag: "<prefix:localName"
        public override Task WriteStartElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
            Task task;
            bufChars[bufPos++] = (char)'<';
            if (prefix != null && prefix.Length != 0)
            {
                task = RawTextAsync(prefix, ":", localName);
            }
            else
            {
                task = RawTextAsync(localName);
            }
            return task.CallVoidFuncWhenFinishAsync(thisRef => thisRef.WriteStartElementAsync_SetAttEndPos(), this);
        }

        private void WriteStartElementAsync_SetAttEndPos()
        {
            attrEndPos = bufPos;
        }

        // Serialize an element end tag: "</prefix:localName>", if content was output.  Otherwise, serialize
        // the shortcut syntax: " />".
        internal override Task WriteEndElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            if (contentPos != bufPos)
            {
                // Content has been output, so can't use shortcut syntax
                bufChars[bufPos++] = (char)'<';
                bufChars[bufPos++] = (char)'/';

                if (prefix != null && prefix.Length != 0)
                {
                    return RawTextAsync(prefix, ":", localName, ">");
                }
                else
                {
                    return RawTextAsync(localName, ">");
                }
            }
            else
            {
                // Use shortcut syntax; overwrite the already output '>' character
                bufPos--;
                bufChars[bufPos++] = (char)' ';
                bufChars[bufPos++] = (char)'/';
                bufChars[bufPos++] = (char)'>';
            }
            return Task.CompletedTask;
        }

        // Serialize a full element end tag: "</prefix:localName>"
        internal override Task WriteFullEndElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            bufChars[bufPos++] = (char)'<';
            bufChars[bufPos++] = (char)'/';

            if (prefix != null && prefix.Length != 0)
            {
                return RawTextAsync(prefix, ":", localName, ">");
            }
            else
            {
                return RawTextAsync(localName, ">");
            }
        }

        // Serialize an attribute tag using double quotes around the attribute value: 'prefix:localName="'
        protected internal override Task WriteStartAttributeAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            if (attrEndPos == bufPos)
            {
                bufChars[bufPos++] = (char)' ';
            }
            Task task;
            if (prefix != null && prefix.Length > 0)
            {
                task = RawTextAsync(prefix, ":", localName);
            }
            else
            {
                task = RawTextAsync(localName);
            }
            return task.CallVoidFuncWhenFinishAsync(thisRef => thisRef.WriteStartAttribute_SetInAttribute(), this);
        }

        private void WriteStartAttribute_SetInAttribute()
        {
            bufChars[bufPos++] = (char)'=';
            bufChars[bufPos++] = (char)'"';
            inAttributeValue = true;
        }

        // Serialize the end of an attribute value using double quotes: '"'
        protected internal override Task WriteEndAttributeAsync()
        {
            CheckAsyncCall();
            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
            bufChars[bufPos++] = (char)'"';
            inAttributeValue = false;
            attrEndPos = bufPos;

            return Task.CompletedTask;
        }

        internal override async Task WriteNamespaceDeclarationAsync(string prefix, string namespaceName)
        {
            CheckAsyncCall();
            Debug.Assert(prefix != null && namespaceName != null);

            await this.WriteStartNamespaceDeclarationAsync(prefix).ConfigureAwait(false);
            await this.WriteStringAsync(namespaceName).ConfigureAwait(false);
            await this.WriteEndNamespaceDeclarationAsync().ConfigureAwait(false);
        }

        internal override async Task WriteStartNamespaceDeclarationAsync(string prefix)
        {
            CheckAsyncCall();
            Debug.Assert(prefix != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            if (prefix.Length == 0)
            {
                await RawTextAsync(" xmlns=\"").ConfigureAwait(false);
            }
            else
            {
                await RawTextAsync(" xmlns:").ConfigureAwait(false);
                await RawTextAsync(prefix).ConfigureAwait(false);
                bufChars[bufPos++] = (char)'=';
                bufChars[bufPos++] = (char)'"';
            }

            inAttributeValue = true;
            if (trackTextContent && inTextContent != true) { ChangeTextContentMark(true); }
        }

        internal override Task WriteEndNamespaceDeclarationAsync()
        {
            CheckAsyncCall();
            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
            inAttributeValue = false;

            bufChars[bufPos++] = (char)'"';
            attrEndPos = bufPos;

            return Task.CompletedTask;
        }

        // Serialize a CData section.  If the "]]>" pattern is found within
        // the text, replace it with "]]><![CDATA[>".
        public override async Task WriteCDataAsync(string text)
        {
            CheckAsyncCall();
            Debug.Assert(text != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            if (mergeCDataSections && bufPos == cdataPos)
            {
                // Merge adjacent cdata sections - overwrite the "]]>" characters
                Debug.Assert(bufPos >= 4);
                bufPos -= 3;
            }
            else
            {
                // Start a new cdata section
                bufChars[bufPos++] = (char)'<';
                bufChars[bufPos++] = (char)'!';
                bufChars[bufPos++] = (char)'[';
                bufChars[bufPos++] = (char)'C';
                bufChars[bufPos++] = (char)'D';
                bufChars[bufPos++] = (char)'A';
                bufChars[bufPos++] = (char)'T';
                bufChars[bufPos++] = (char)'A';
                bufChars[bufPos++] = (char)'[';
            }

            await WriteCDataSectionAsync(text).ConfigureAwait(false);

            bufChars[bufPos++] = (char)']';
            bufChars[bufPos++] = (char)']';
            bufChars[bufPos++] = (char)'>';

            textPos = bufPos;
            cdataPos = bufPos;
        }

        // Serialize a comment.
        public override async Task WriteCommentAsync(string text)
        {
            CheckAsyncCall();
            Debug.Assert(text != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            bufChars[bufPos++] = (char)'<';
            bufChars[bufPos++] = (char)'!';
            bufChars[bufPos++] = (char)'-';
            bufChars[bufPos++] = (char)'-';

            await WriteCommentOrPiAsync(text, '-').ConfigureAwait(false);

            bufChars[bufPos++] = (char)'-';
            bufChars[bufPos++] = (char)'-';
            bufChars[bufPos++] = (char)'>';
        }

        // Serialize a processing instruction.
        public override async Task WriteProcessingInstructionAsync(string name, string text)
        {
            CheckAsyncCall();
            Debug.Assert(name != null && name.Length > 0);
            Debug.Assert(text != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            bufChars[bufPos++] = (char)'<';
            bufChars[bufPos++] = (char)'?';
            await RawTextAsync(name).ConfigureAwait(false);

            if (text.Length > 0)
            {
                bufChars[bufPos++] = (char)' ';
                await WriteCommentOrPiAsync(text, '?').ConfigureAwait(false);
            }

            bufChars[bufPos++] = (char)'?';
            bufChars[bufPos++] = (char)'>';
        }

        // Serialize an entity reference.
        public override async Task WriteEntityRefAsync(string name)
        {
            CheckAsyncCall();
            Debug.Assert(name != null && name.Length > 0);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            bufChars[bufPos++] = (char)'&';
            await RawTextAsync(name).ConfigureAwait(false);
            bufChars[bufPos++] = (char)';';

            if (bufPos > bufLen)
            {
                await FlushBufferAsync().ConfigureAwait(false);
            }

            textPos = bufPos;
        }

        // Serialize a character entity reference.
        public override async Task WriteCharEntityAsync(char ch)
        {
            CheckAsyncCall();
            string strVal = ((int)ch).ToString("X", NumberFormatInfo.InvariantInfo);

            if (checkCharacters && !xmlCharType.IsCharData(ch))
            {
                // we just have a single char, not a surrogate, therefore we have to pass in '\0' for the second char
                throw XmlConvert.CreateInvalidCharException(ch, '\0');
            }

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            bufChars[bufPos++] = (char)'&';
            bufChars[bufPos++] = (char)'#';
            bufChars[bufPos++] = (char)'x';
            await RawTextAsync(strVal).ConfigureAwait(false);
            bufChars[bufPos++] = (char)';';

            if (bufPos > bufLen)
            {
                await FlushBufferAsync().ConfigureAwait(false);
            }

            textPos = bufPos;
        }

        // Serialize a whitespace node.

        public override Task WriteWhitespaceAsync(string ws)
        {
            CheckAsyncCall();
            Debug.Assert(ws != null);
            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            if (inAttributeValue)
            {
                return WriteAttributeTextBlockAsync(ws);
            }
            else
            {
                return WriteElementTextBlockAsync(ws);
            }
        }

        // Serialize either attribute or element text using XML rules.

        public override Task WriteStringAsync(string text)
        {
            CheckAsyncCall();
            Debug.Assert(text != null);
            if (trackTextContent && inTextContent != true) { ChangeTextContentMark(true); }

            if (inAttributeValue)
            {
                return WriteAttributeTextBlockAsync(text);
            }
            else
            {
                return WriteElementTextBlockAsync(text);
            }
        }

        // Serialize surrogate character entity.
        public override async Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
        {
            CheckAsyncCall();
            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }
            int surrogateChar = XmlCharType.CombineSurrogateChar(lowChar, highChar);

            bufChars[bufPos++] = (char)'&';
            bufChars[bufPos++] = (char)'#';
            bufChars[bufPos++] = (char)'x';
            await RawTextAsync(surrogateChar.ToString("X", NumberFormatInfo.InvariantInfo)).ConfigureAwait(false);
            bufChars[bufPos++] = (char)';';
            textPos = bufPos;
        }

        // Serialize either attribute or element text using XML rules.
        // Arguments are validated in the XmlWellformedWriter layer.

        public override Task WriteCharsAsync(char[] buffer, int index, int count)
        {
            CheckAsyncCall();
            Debug.Assert(buffer != null);
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0 && index + count <= buffer.Length);

            if (trackTextContent && inTextContent != true) { ChangeTextContentMark(true); }

            if (inAttributeValue)
            {
                return WriteAttributeTextBlockAsync(buffer, index, count);
            }
            else
            {
                return WriteElementTextBlockAsync(buffer, index, count);
            }
        }

        // Serialize raw data.
        // Arguments are validated in the XmlWellformedWriter layer

        public override async Task WriteRawAsync(char[] buffer, int index, int count)
        {
            CheckAsyncCall();
            Debug.Assert(buffer != null);
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0 && index + count <= buffer.Length);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            await WriteRawWithCharCheckingAsync(buffer, index, count).ConfigureAwait(false);

            textPos = bufPos;
        }

        // Serialize raw data.

        public override async Task WriteRawAsync(string data)
        {
            CheckAsyncCall();
            Debug.Assert(data != null);

            if (trackTextContent && inTextContent != false) { ChangeTextContentMark(false); }

            await WriteRawWithCharCheckingAsync(data).ConfigureAwait(false);

            textPos = bufPos;
        }

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

        //
        // Implementation methods
        //
        // Flush all characters in the buffer to output.  Do not flush the output object.
        protected virtual async Task FlushBufferAsync()
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
                        await writer.WriteAsync(bufChars, 1, bufPos - 1).ConfigureAwait(false);
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
                bufChars[0] = bufChars[bufPos - 1];

                // Reset buffer position
                textPos = (textPos == bufPos) ? 1 : 0;
                attrEndPos = (attrEndPos == bufPos) ? 1 : 0;
                contentPos = 0;    // Needs to be zero, since overwriting '>' character is no longer possible
                cdataPos = 0;      // Needs to be zero, since overwriting ']]>' characters is no longer possible
                bufPos = 1;        // Buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
                                   // close an empty element or in CDATA section detection of double ]; _BUFFER[0] will always be 0
            }
        }

        private async Task EncodeCharsAsync(int startOffset, int endOffset, bool writeAllToStream)
        {
            // Write encoded text to stream
            int chEnc;
            int bEnc;
            bool completed;
            while (startOffset < endOffset)
            {
                if (_charEntityFallback != null)
                {
                    _charEntityFallback.StartOffset = startOffset;
                }
                encoder.Convert(bufChars, startOffset, endOffset - startOffset, bufBytes, bufBytesUsed, bufBytes.Length - bufBytesUsed, false, out chEnc, out bEnc, out completed);
                startOffset += chEnc;
                bufBytesUsed += bEnc;
                if (bufBytesUsed >= (bufBytes.Length - 16))
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
                int chEnc;
                int bEnc;
                bool completed;
                // decode no chars, just flush
                encoder.Convert(bufChars, 1, 0, bufBytes, 0, bufBytes.Length, true, out chEnc, out bEnc, out completed);
                if (bEnc != 0)
                {
                    return stream.WriteAsync(bufBytes, 0, bEnc);
                }
            }

            return Task.CompletedTask;
        }

        // Serialize text that is part of an attribute value.  The '&', '<', '>', and '"' characters
        // are entitized.
        protected unsafe int WriteAttributeTextBlockNoFlush(char* pSrc, char* pSrcEnd)
        {
            char* pRaw = pSrc;

            fixed (char* pDstBegin = bufChars)
            {
                char* pDst = pDstBegin + this.bufPos;

                int ch = 0;
                for (;;)
                {
                    char* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && xmlCharType.IsAttributeValueChar((char)(ch = *pSrc)))
                    {
                        *pDst = (char)ch;
                        pDst++;
                        pSrc++;
                    }
                    Debug.Assert(pSrc <= pSrcEnd);

                    // end of value
                    if (pSrc >= pSrcEnd)
                    {
                        break;
                    }

                    // end of buffer
                    if (pDst >= pDstEnd)
                    {
                        bufPos = (int)(pDst - pDstBegin);
                        return (int)(pSrc - pRaw);
                    }

                    // some character needs to be escaped
                    switch (ch)
                    {
                        case '&':
                            pDst = AmpEntity(pDst);
                            break;
                        case '<':
                            pDst = LtEntity(pDst);
                            break;
                        case '>':
                            pDst = GtEntity(pDst);
                            break;
                        case '"':
                            pDst = QuoteEntity(pDst);
                            break;
                        case '\'':
                            *pDst = (char)ch;
                            pDst++;
                            break;
                        case (char)0x9:
                            if (newLineHandling == NewLineHandling.None)
                            {
                                *pDst = (char)ch;
                                pDst++;
                            }
                            else
                            {
                                // escape tab in attributes
                                pDst = TabEntity(pDst);
                            }
                            break;
                        case (char)0xD:
                            if (newLineHandling == NewLineHandling.None)
                            {
                                *pDst = (char)ch;
                                pDst++;
                            }
                            else
                            {
                                // escape new lines in attributes
                                pDst = CarriageReturnEntity(pDst);
                            }
                            break;
                        case (char)0xA:
                            if (newLineHandling == NewLineHandling.None)
                            {
                                *pDst = (char)ch;
                                pDst++;
                            }
                            else
                            {
                                // escape new lines in attributes
                                pDst = LineFeedEntity(pDst);
                            }
                            break;
                        default:
                            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, true); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
            }

            return -1;
        }

        protected unsafe int WriteAttributeTextBlockNoFlush(char[] chars, int index, int count)
        {
            if (count == 0)
            {
                return -1;
            }
            fixed (char* pSrc = &chars[index])
            {
                char* pSrcBeg = pSrc;
                char* pSrcEnd = pSrcBeg + count;
                return WriteAttributeTextBlockNoFlush(pSrcBeg, pSrcEnd);
            }
        }

        protected unsafe int WriteAttributeTextBlockNoFlush(string text, int index, int count)
        {
            if (count == 0)
            {
                return -1;
            }
            fixed (char* pSrc = text)
            {
                char* pSrcBeg = pSrc + index;
                char* pSrcEnd = pSrcBeg + count;
                return WriteAttributeTextBlockNoFlush(pSrcBeg, pSrcEnd);
            }
        }

        protected async Task WriteAttributeTextBlockAsync(char[] chars, int index, int count)
        {
            int writeLen = 0;
            int curIndex = index;
            int leftCount = count;
            do
            {
                writeLen = WriteAttributeTextBlockNoFlush(chars, curIndex, leftCount);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0);
        }

        protected Task WriteAttributeTextBlockAsync(string text)
        {
            int writeLen = 0;
            int curIndex = 0;
            int leftCount = text.Length;

            writeLen = WriteAttributeTextBlockNoFlush(text, curIndex, leftCount);
            curIndex += writeLen;
            leftCount -= writeLen;
            if (writeLen >= 0)
            {
                return _WriteAttributeTextBlockAsync(text, curIndex, leftCount);
            }

            return Task.CompletedTask;
        }


        private async Task _WriteAttributeTextBlockAsync(string text, int curIndex, int leftCount)
        {
            int writeLen;
            await FlushBufferAsync().ConfigureAwait(false);
            do
            {
                writeLen = WriteAttributeTextBlockNoFlush(text, curIndex, leftCount);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0);
        }

        // Serialize text that is part of element content.  The '&', '<', and '>' characters
        // are entitized.
        protected unsafe int WriteElementTextBlockNoFlush(char* pSrc, char* pSrcEnd, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            char* pRaw = pSrc;

            fixed (char* pDstBegin = bufChars)
            {
                char* pDst = pDstBegin + this.bufPos;

                int ch = 0;
                for (;;)
                {
                    char* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && xmlCharType.IsAttributeValueChar((char)(ch = *pSrc)))
                    {
                        *pDst = (char)ch;
                        pDst++;
                        pSrc++;
                    }
                    Debug.Assert(pSrc <= pSrcEnd);

                    // end of value
                    if (pSrc >= pSrcEnd)
                    {
                        break;
                    }

                    // end of buffer
                    if (pDst >= pDstEnd)
                    {
                        bufPos = (int)(pDst - pDstBegin);
                        return (int)(pSrc - pRaw);
                    }

                    // some character needs to be escaped
                    switch (ch)
                    {
                        case '&':
                            pDst = AmpEntity(pDst);
                            break;
                        case '<':
                            pDst = LtEntity(pDst);
                            break;
                        case '>':
                            pDst = GtEntity(pDst);
                            break;
                        case '"':
                        case '\'':
                        case (char)0x9:
                            *pDst = (char)ch;
                            pDst++;
                            break;
                        case (char)0xA:
                            if (newLineHandling == NewLineHandling.Replace)
                            {
                                bufPos = (int)(pDst - pDstBegin);
                                needWriteNewLine = true;
                                return (int)(pSrc - pRaw);
                            }
                            else
                            {
                                *pDst = (char)ch;
                                pDst++;
                            }
                            break;
                        case (char)0xD:
                            switch (newLineHandling)
                            {
                                case NewLineHandling.Replace:
                                    // Replace "\r\n", or "\r" with NewLineChars
                                    if (pSrc[1] == '\n')
                                    {
                                        pSrc++;
                                    }

                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);

                                case NewLineHandling.Entitize:
                                    // Entitize 0xD
                                    pDst = CarriageReturnEntity(pDst);
                                    break;
                                case NewLineHandling.None:
                                    *pDst = (char)ch;
                                    pDst++;
                                    break;
                            }
                            break;
                        default:
                            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, true); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
                textPos = bufPos;
                contentPos = 0;
            }

            return -1;
        }

        protected unsafe int WriteElementTextBlockNoFlush(char[] chars, int index, int count, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            if (count == 0)
            {
                contentPos = 0;
                return -1;
            }
            fixed (char* pSrc = &chars[index])
            {
                char* pSrcBeg = pSrc;
                char* pSrcEnd = pSrcBeg + count;
                return WriteElementTextBlockNoFlush(pSrcBeg, pSrcEnd, out needWriteNewLine);
            }
        }

        protected unsafe int WriteElementTextBlockNoFlush(string text, int index, int count, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            if (count == 0)
            {
                contentPos = 0;
                return -1;
            }
            fixed (char* pSrc = text)
            {
                char* pSrcBeg = pSrc + index;
                char* pSrcEnd = pSrcBeg + count;
                return WriteElementTextBlockNoFlush(pSrcBeg, pSrcEnd, out needWriteNewLine);
            }
        }

        protected async Task WriteElementTextBlockAsync(char[] chars, int index, int count)
        {
            int writeLen = 0;
            int curIndex = index;
            int leftCount = count;
            bool needWriteNewLine = false;
            do
            {
                writeLen = WriteElementTextBlockNoFlush(chars, curIndex, leftCount, out needWriteNewLine);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (needWriteNewLine)
                {
                    //hit WriteNewLine
                    await RawTextAsync(newLineChars).ConfigureAwait(false);
                    curIndex++;
                    leftCount--;
                }
                else if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0 || needWriteNewLine);
        }

        protected Task WriteElementTextBlockAsync(string text)
        {
            int writeLen = 0;
            int curIndex = 0;
            int leftCount = text.Length;
            bool needWriteNewLine = false;

            writeLen = WriteElementTextBlockNoFlush(text, curIndex, leftCount, out needWriteNewLine);
            curIndex += writeLen;
            leftCount -= writeLen;
            if (needWriteNewLine)
            {
                return _WriteElementTextBlockAsync(true, text, curIndex, leftCount);
            }
            else if (writeLen >= 0)
            {
                return _WriteElementTextBlockAsync(false, text, curIndex, leftCount);
            }

            return Task.CompletedTask;
        }

        private async Task _WriteElementTextBlockAsync(bool newLine, string text, int curIndex, int leftCount)
        {
            int writeLen = 0;
            bool needWriteNewLine = false;

            if (newLine)
            {
                await RawTextAsync(newLineChars).ConfigureAwait(false);
                curIndex++;
                leftCount--;
            }
            else
            {
                await FlushBufferAsync().ConfigureAwait(false);
            }

            do
            {
                writeLen = WriteElementTextBlockNoFlush(text, curIndex, leftCount, out needWriteNewLine);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (needWriteNewLine)
                {
                    //hit WriteNewLine
                    await RawTextAsync(newLineChars).ConfigureAwait(false);
                    curIndex++;
                    leftCount--;
                }
                else if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0 || needWriteNewLine);
        }

        protected unsafe int RawTextNoFlush(char* pSrcBegin, char* pSrcEnd)
        {
            char* pRaw = pSrcBegin;

            fixed (char* pDstBegin = bufChars)
            {
                char* pDst = pDstBegin + this.bufPos;
                char* pSrc = pSrcBegin;

                int ch = 0;
                for (;;)
                {
                    char* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + this.bufLen)
                    {
                        pDstEnd = pDstBegin + this.bufLen;
                    }

                    while (pDst < pDstEnd && ((ch = *pSrc) < XmlCharType.SurHighStart))
                    {
                        pSrc++;
                        *pDst = (char)ch;
                        pDst++;
                    }
                    Debug.Assert(pSrc <= pSrcEnd);

                    // end of value
                    if (pSrc >= pSrcEnd)
                    {
                        break;
                    }

                    // end of buffer
                    if (pDst >= pDstEnd)
                    {
                        bufPos = (int)(pDst - pDstBegin);
                        return (int)(pSrc - pRaw);
                    }

                    if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
                }

                bufPos = (int)(pDst - pDstBegin);
            }

            return -1;
        }

        protected unsafe int RawTextNoFlush(string text, int index, int count)
        {
            if (count == 0)
            {
                return -1;
            }
            fixed (char* pSrc = text)
            {
                char* pSrcBegin = pSrc + index;
                char* pSrcEnd = pSrcBegin + count;
                return RawTextNoFlush(pSrcBegin, pSrcEnd);
            }
        }

        // special-case the one string overload, as it's so common
        protected Task RawTextAsync(string text)
        {
            int writeLen = RawTextNoFlush(text, 0, text.Length);
            return writeLen >= 0 ?
                _RawTextAsync(text, writeLen, text.Length - writeLen) :
                Task.CompletedTask;
        }

        protected Task RawTextAsync(string text1, string text2 = null, string text3 = null, string text4 = null)
        {
            Debug.Assert(text1 != null);
            Debug.Assert(text2 != null || (text3 == null && text4 == null));
            Debug.Assert(text3 != null || (text4 == null));

            int writeLen;

            // Write out the first string
            writeLen = RawTextNoFlush(text1, 0, text1.Length);
            if (writeLen >= 0)
            {
                // If we were only able to partially write it, write out the remainder
                // and then write out the other strings.
                return _RawTextAsync(text1, writeLen, text1.Length - writeLen, text2, text3, text4);
            }

            // We wrote out the first string.  Try to write out the second, if it exists.
            if (text2 != null)
            {
                writeLen = RawTextNoFlush(text2, 0, text2.Length);
                if (writeLen >= 0)
                {
                    // If we were only able to write out some of the second string,
                    // write out the remainder and then the other strings, 
                    return _RawTextAsync(text2, writeLen, text2.Length - writeLen, text3, text4);
                }
            }

            // We wrote out the first and second strings.  Try to write out the third
            // if it exists.
            if (text3 != null)
            {
                writeLen = RawTextNoFlush(text3, 0, text3.Length);
                if (writeLen >= 0)
                {
                    // If we were only able to write out some of the third string,
                    // write out the remainder and then the last string.
                    return _RawTextAsync(text3, writeLen, text3.Length - writeLen, text4);
                }
            }

            // Finally, try to write out the fourth string, if it exists.
            if (text4 != null)
            {
                writeLen = RawTextNoFlush(text4, 0, text4.Length);
                if (writeLen >= 0)
                {
                    return _RawTextAsync(text4, writeLen, text4.Length - writeLen);
                }
            }

            // All strings written successfully.
            return Task.CompletedTask;
        }

        private async Task _RawTextAsync(
            string text1, int curIndex1, int leftCount1,
            string text2 = null, string text3 = null, string text4 = null)
        {
            Debug.Assert(text1 != null);
            Debug.Assert(text2 != null || (text3 == null && text4 == null));
            Debug.Assert(text3 != null || (text4 == null));

            // Write out the remainder of the first string
            await FlushBufferAsync().ConfigureAwait(false);
            int writeLen = 0;
            do
            {
                writeLen = RawTextNoFlush(text1, curIndex1, leftCount1);
                curIndex1 += writeLen;
                leftCount1 -= writeLen;
                if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0);

            // If there are additional strings, write them out as well
            if (text2 != null)
            {
                await RawTextAsync(text2, text3, text4).ConfigureAwait(false);
            }
        }

        protected unsafe int WriteRawWithCharCheckingNoFlush(char* pSrcBegin, char* pSrcEnd, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            char* pRaw = pSrcBegin;

            fixed (char* pDstBegin = bufChars)
            {
                char* pSrc = pSrcBegin;
                char* pDst = pDstBegin + bufPos;

                int ch = 0;
                for (;;)
                {
                    char* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && xmlCharType.IsTextChar((char)(ch = *pSrc)))
                    {
                        *pDst = (char)ch;
                        pDst++;
                        pSrc++;
                    }

                    Debug.Assert(pSrc <= pSrcEnd);

                    // end of value
                    if (pSrc >= pSrcEnd)
                    {
                        break;
                    }

                    // end of buffer
                    if (pDst >= pDstEnd)
                    {
                        bufPos = (int)(pDst - pDstBegin);
                        return (int)(pSrc - pRaw);
                    }

                    // handle special characters
                    switch (ch)
                    {
                        case ']':
                        case '<':
                        case '&':
                        case (char)0x9:
                            *pDst = (char)ch;
                            pDst++;
                            break;
                        case (char)0xD:
                            if (newLineHandling == NewLineHandling.Replace)
                            {
                                // Normalize "\r\n", or "\r" to NewLineChars
                                if (pSrc[1] == '\n')
                                {
                                    pSrc++;
                                }

                                bufPos = (int)(pDst - pDstBegin);
                                needWriteNewLine = true;
                                return (int)(pSrc - pRaw);
                            }
                            else
                            {
                                *pDst = (char)ch;
                                pDst++;
                            }
                            break;
                        case (char)0xA:
                            if (newLineHandling == NewLineHandling.Replace)
                            {
                                bufPos = (int)(pDst - pDstBegin);
                                needWriteNewLine = true;
                                return (int)(pSrc - pRaw);
                            }
                            else
                            {
                                *pDst = (char)ch;
                                pDst++;
                            }
                            break;
                        default:
                            if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
            }

            return -1;
        }

        protected unsafe int WriteRawWithCharCheckingNoFlush(char[] chars, int index, int count, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            if (count == 0)
            {
                return -1;
            }
            fixed (char* pSrc = &chars[index])
            {
                char* pSrcBeg = pSrc;
                char* pSrcEnd = pSrcBeg + count;
                return WriteRawWithCharCheckingNoFlush(pSrcBeg, pSrcEnd, out needWriteNewLine);
            }
        }

        protected unsafe int WriteRawWithCharCheckingNoFlush(string text, int index, int count, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            if (count == 0)
            {
                return -1;
            }
            fixed (char* pSrc = text)
            {
                char* pSrcBeg = pSrc + index;
                char* pSrcEnd = pSrcBeg + count;
                return WriteRawWithCharCheckingNoFlush(pSrcBeg, pSrcEnd, out needWriteNewLine);
            }
        }

        protected async Task WriteRawWithCharCheckingAsync(char[] chars, int index, int count)
        {
            int writeLen = 0;
            int curIndex = index;
            int leftCount = count;
            bool needWriteNewLine = false;
            do
            {
                writeLen = WriteRawWithCharCheckingNoFlush(chars, curIndex, leftCount, out needWriteNewLine);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (needWriteNewLine)
                {
                    await RawTextAsync(newLineChars).ConfigureAwait(false);
                    curIndex++;
                    leftCount--;
                }
                else if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0 || needWriteNewLine);
        }

        protected async Task WriteRawWithCharCheckingAsync(string text)
        {
            int writeLen = 0;
            int curIndex = 0;
            int leftCount = text.Length;
            bool needWriteNewLine = false;
            do
            {
                writeLen = WriteRawWithCharCheckingNoFlush(text, curIndex, leftCount, out needWriteNewLine);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (needWriteNewLine)
                {
                    await RawTextAsync(newLineChars).ConfigureAwait(false);
                    curIndex++;
                    leftCount--;
                }
                else if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0 || needWriteNewLine);
        }

        protected unsafe int WriteCommentOrPiNoFlush(string text, int index, int count, int stopChar, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            if (count == 0)
            {
                return -1;
            }
            fixed (char* pSrcText = text)
            {
                char* pSrcBegin = pSrcText + index;

                fixed (char* pDstBegin = bufChars)
                {
                    char* pSrc = pSrcBegin;

                    char* pRaw = pSrc;

                    char* pSrcEnd = pSrcBegin + count;

                    char* pDst = pDstBegin + bufPos;

                    int ch = 0;
                    for (;;)
                    {
                        char* pDstEnd = pDst + (pSrcEnd - pSrc);
                        if (pDstEnd > pDstBegin + bufLen)
                        {
                            pDstEnd = pDstBegin + bufLen;
                        }

                        while (pDst < pDstEnd && (xmlCharType.IsTextChar((char)(ch = *pSrc)) && ch != stopChar))
                        {
                            *pDst = (char)ch;
                            pDst++;
                            pSrc++;
                        }

                        Debug.Assert(pSrc <= pSrcEnd);

                        // end of value
                        if (pSrc >= pSrcEnd)
                        {
                            break;
                        }

                        // end of buffer
                        if (pDst >= pDstEnd)
                        {
                            bufPos = (int)(pDst - pDstBegin);
                            return (int)(pSrc - pRaw);
                        }

                        // handle special characters
                        switch (ch)
                        {
                            case '-':
                                *pDst = (char)'-';
                                pDst++;
                                if (ch == stopChar)
                                {
                                    // Insert space between adjacent dashes or before comment's end dashes
                                    if (pSrc + 1 == pSrcEnd || *(pSrc + 1) == '-')
                                    {
                                        *pDst = (char)' ';
                                        pDst++;
                                    }
                                }
                                break;
                            case '?':
                                *pDst = (char)'?';
                                pDst++;
                                if (ch == stopChar)
                                {
                                    // Processing instruction: insert space between adjacent '?' and '>' 
                                    if (pSrc + 1 < pSrcEnd && *(pSrc + 1) == '>')
                                    {
                                        *pDst = (char)' ';
                                        pDst++;
                                    }
                                }
                                break;
                            case ']':
                                *pDst = (char)']';
                                pDst++;
                                break;
                            case (char)0xD:
                                if (newLineHandling == NewLineHandling.Replace)
                                {
                                    // Normalize "\r\n", or "\r" to NewLineChars
                                    if (pSrc[1] == '\n')
                                    {
                                        pSrc++;
                                    }

                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);
                                }
                                else
                                {
                                    *pDst = (char)ch;
                                    pDst++;
                                }
                                break;
                            case (char)0xA:
                                if (newLineHandling == NewLineHandling.Replace)
                                {
                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);
                                }
                                else
                                {
                                    *pDst = (char)ch;
                                    pDst++;
                                }
                                break;
                            case '<':
                            case '&':
                            case (char)0x9:
                                *pDst = (char)ch;
                                pDst++;
                                break;
                            default:
                                if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
                                continue;
                        }
                        pSrc++;
                    }
                    bufPos = (int)(pDst - pDstBegin);
                }

                return -1;
            }
        }

        protected async Task WriteCommentOrPiAsync(string text, int stopChar)
        {
            if (text.Length == 0)
            {
                if (bufPos >= bufLen)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
                return;
            }

            int writeLen = 0;
            int curIndex = 0;
            int leftCount = text.Length;
            bool needWriteNewLine = false;
            do
            {
                writeLen = WriteCommentOrPiNoFlush(text, curIndex, leftCount, stopChar, out needWriteNewLine);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (needWriteNewLine)
                {
                    await RawTextAsync(newLineChars).ConfigureAwait(false);
                    curIndex++;
                    leftCount--;
                }
                else if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0 || needWriteNewLine);
        }

        protected unsafe int WriteCDataSectionNoFlush(string text, int index, int count, out bool needWriteNewLine)
        {
            needWriteNewLine = false;
            if (count == 0)
            {
                return -1;
            }

            // write text

            fixed (char* pSrcText = text)
            {
                char* pSrcBegin = pSrcText + index;

                fixed (char* pDstBegin = bufChars)
                {
                    char* pSrc = pSrcBegin;

                    char* pSrcEnd = pSrcBegin + count;

                    char* pRaw = pSrc;

                    char* pDst = pDstBegin + bufPos;

                    int ch = 0;
                    for (;;)
                    {
                        char* pDstEnd = pDst + (pSrcEnd - pSrc);
                        if (pDstEnd > pDstBegin + bufLen)
                        {
                            pDstEnd = pDstBegin + bufLen;
                        }

                        while (pDst < pDstEnd && (xmlCharType.IsAttributeValueChar((char)(ch = *pSrc)) && ch != ']'))
                        {
                            *pDst = (char)ch;
                            pDst++;
                            pSrc++;
                        }

                        Debug.Assert(pSrc <= pSrcEnd);

                        // end of value
                        if (pSrc >= pSrcEnd)
                        {
                            break;
                        }

                        // end of buffer
                        if (pDst >= pDstEnd)
                        {
                            bufPos = (int)(pDst - pDstBegin);
                            return (int)(pSrc - pRaw);
                        }

                        // handle special characters
                        switch (ch)
                        {
                            case '>':
                                if (hadDoubleBracket && pDst[-1] == (char)']')
                                {   // pDst[-1] will always correct - there is a padding character at _BUFFER[0]
                                    // The characters "]]>" were found within the CData text
                                    pDst = RawEndCData(pDst);
                                    pDst = RawStartCData(pDst);
                                }
                                *pDst = (char)'>';
                                pDst++;
                                break;
                            case ']':
                                if (pDst[-1] == (char)']')
                                {   // pDst[-1] will always correct - there is a padding character at _BUFFER[0]
                                    hadDoubleBracket = true;
                                }
                                else
                                {
                                    hadDoubleBracket = false;
                                }
                                *pDst = (char)']';
                                pDst++;
                                break;
                            case (char)0xD:
                                if (newLineHandling == NewLineHandling.Replace)
                                {
                                    // Normalize "\r\n", or "\r" to NewLineChars
                                    if (pSrc[1] == '\n')
                                    {
                                        pSrc++;
                                    }

                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);
                                }
                                else
                                {
                                    *pDst = (char)ch;
                                    pDst++;
                                }
                                break;
                            case (char)0xA:
                                if (newLineHandling == NewLineHandling.Replace)
                                {
                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);
                                }
                                else
                                {
                                    *pDst = (char)ch;
                                    pDst++;
                                }
                                break;
                            case '&':
                            case '<':
                            case '"':
                            case '\'':
                            case (char)0x9:
                                *pDst = (char)ch;
                                pDst++;
                                break;
                            default:
                                if (XmlCharType.IsSurrogate(ch)) { pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst); pSrc += 2; } else if (ch <= 0x7F || ch >= 0xFFFE) { pDst = InvalidXmlChar(ch, pDst, false); pSrc++; } else { *pDst = (char)ch; pDst++; pSrc++; };
                                continue;
                        }
                        pSrc++;
                    }
                    bufPos = (int)(pDst - pDstBegin);
                }

                return -1;
            }
        }

        protected async Task WriteCDataSectionAsync(string text)
        {
            if (text.Length == 0)
            {
                if (bufPos >= bufLen)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
                return;
            }

            int writeLen = 0;
            int curIndex = 0;
            int leftCount = text.Length;
            bool needWriteNewLine = false;
            do
            {
                writeLen = WriteCDataSectionNoFlush(text, curIndex, leftCount, out needWriteNewLine);
                curIndex += writeLen;
                leftCount -= writeLen;
                if (needWriteNewLine)
                {
                    await RawTextAsync(newLineChars).ConfigureAwait(false);
                    curIndex++;
                    leftCount--;
                }
                else if (writeLen >= 0)
                {
                    await FlushBufferAsync().ConfigureAwait(false);
                }
            } while (writeLen >= 0 || needWriteNewLine);
        }
    }

    // Same as base text writer class except that elements, attributes, comments, and pi's are indented.
    internal partial class XmlEncodedRawTextWriterIndent : XmlEncodedRawTextWriter
    {
        public override async Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
        {
            CheckAsyncCall();
            // Add indentation
            if (!mixedContent && base.textPos != base.bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
            await base.WriteDocTypeAsync(name, pubid, sysid, subset).ConfigureAwait(false);
        }

        public override async Task WriteStartElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length != 0 && prefix != null && ns != null);

            // Add indentation
            if (!mixedContent && base.textPos != base.bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
            indentLevel++;
            _mixedContentStack.PushBit(mixedContent);

            await base.WriteStartElementAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        internal override async Task WriteEndElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            // Add indentation
            indentLevel--;
            if (!mixedContent && base.contentPos != base.bufPos)
            {
                // There was content, so try to indent
                if (base.textPos != base.bufPos)
                {
                    await WriteIndentAsync().ConfigureAwait(false);
                }
            }
            mixedContent = _mixedContentStack.PopBit();

            await base.WriteEndElementAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        internal override async Task WriteFullEndElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            // Add indentation
            indentLevel--;
            if (!mixedContent && base.contentPos != base.bufPos)
            {
                // There was content, so try to indent
                if (base.textPos != base.bufPos)
                {
                    await WriteIndentAsync().ConfigureAwait(false);
                }
            }
            mixedContent = _mixedContentStack.PopBit();

            await base.WriteFullEndElementAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        // Same as base class, plus possible indentation.
        protected internal override async Task WriteStartAttributeAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            // Add indentation
            if (newLineOnAttributes)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }

            await base.WriteStartAttributeAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        public override Task WriteCDataAsync(string text)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteCDataAsync(text);
        }

        public override async Task WriteCommentAsync(string text)
        {
            CheckAsyncCall();
            if (!mixedContent && base.textPos != base.bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }

            await base.WriteCommentAsync(text).ConfigureAwait(false);
        }

        public override async Task WriteProcessingInstructionAsync(string target, string text)
        {
            CheckAsyncCall();
            if (!mixedContent && base.textPos != base.bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }

            await base.WriteProcessingInstructionAsync(target, text).ConfigureAwait(false);
        }

        public override Task WriteEntityRefAsync(string name)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteEntityRefAsync(name);
        }

        public override Task WriteCharEntityAsync(char ch)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteCharEntityAsync(ch);
        }

        public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteSurrogateCharEntityAsync(lowChar, highChar);
        }

        public override Task WriteWhitespaceAsync(string ws)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteWhitespaceAsync(ws);
        }

        public override Task WriteStringAsync(string text)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteStringAsync(text);
        }

        public override Task WriteCharsAsync(char[] buffer, int index, int count)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteCharsAsync(buffer, index, count);
        }

        public override Task WriteRawAsync(char[] buffer, int index, int count)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteRawAsync(buffer, index, count);
        }

        public override Task WriteRawAsync(string data)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteRawAsync(data);
        }

        public override Task WriteBase64Async(byte[] buffer, int index, int count)
        {
            CheckAsyncCall();
            mixedContent = true;
            return base.WriteBase64Async(buffer, index, count);
        }

        // Add indentation to output.  Write newline and then repeat IndentChars for each indent level.
        private async Task WriteIndentAsync()
        {
            CheckAsyncCall();
            await RawTextAsync(base.newLineChars).ConfigureAwait(false);
            for (int i = indentLevel; i > 0; i--)
            {
                await RawTextAsync(indentChars).ConfigureAwait(false);
            }
        }
    }
}

