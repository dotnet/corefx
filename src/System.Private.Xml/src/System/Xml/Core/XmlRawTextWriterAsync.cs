// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
    internal partial class XmlRawTextWriter<T>
    {
        protected abstract Task FlushBufferAsync();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                SetTextContentMark(false);

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

            SetTextContentMark(false);

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
                buf[bufPos++] = ToBufferType('"');
            }
            else if (sysid != null)
            {
                await RawTextAsync(" SYSTEM \"").ConfigureAwait(false);
                await RawTextAsync(sysid).ConfigureAwait(false);
                buf[bufPos++] = ToBufferType('"');
            }
            else
            {
                buf[bufPos++] = ToBufferType(' ');
            }

            if (subset != null)
            {
                buf[bufPos++] = ToBufferType('[');
                await RawTextAsync(subset).ConfigureAwait(false);
                buf[bufPos++] = ToBufferType(']');
            }

            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize the beginning of an element start tag: "<prefix:localName"
        public override Task WriteStartElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            SetTextContentMark(false);
            Task task;
            buf[bufPos++] = ToBufferType('<');
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

            SetTextContentMark(false);

            if (contentPos != bufPos)
            {
                // Content has been output, so can't use shortcut syntax
                buf[bufPos++] = ToBufferType('<');
                buf[bufPos++] = ToBufferType('/');

                if (prefix != null && prefix.Length != 0)
                {
                    return RawTextAsync(prefix, ":", localName, ">");
                }

                return RawTextAsync(localName, ">");
            }

            // Use shortcut syntax; overwrite the already output '>' character
            bufPos--;
            buf[bufPos++] = ToBufferType(' ');
            buf[bufPos++] = ToBufferType('/');
            buf[bufPos++] = ToBufferType('>');
            return Task.CompletedTask;
        }

        // Serialize a full element end tag: "</prefix:localName>"
        internal override Task WriteFullEndElementAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            buf[bufPos++] = ToBufferType('/');

            if (prefix != null && prefix.Length != 0)
            {
                return RawTextAsync(prefix, ":", localName, ">");
            }

            return RawTextAsync(localName, ">");
        }

        // Serialize an attribute tag using double quotes around the attribute value: 'prefix:localName="'
        protected internal override Task WriteStartAttributeAsync(string prefix, string localName, string ns)
        {
            CheckAsyncCall();
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            SetTextContentMark(false);

            if (attrEndPos == bufPos)
            {
                buf[bufPos++] = ToBufferType(' ');
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
            buf[bufPos++] = ToBufferType('=');
            buf[bufPos++] = ToBufferType('"');
            inAttributeValue = true;
        }

        // Serialize the end of an attribute value using double quotes: '"'
        protected internal override Task WriteEndAttributeAsync()
        {
            CheckAsyncCall();
            SetTextContentMark(false);
            buf[bufPos++] = ToBufferType('"');
            inAttributeValue = false;
            attrEndPos = bufPos;

            return Task.CompletedTask;
        }

        internal override async Task WriteNamespaceDeclarationAsync(string prefix, string namespaceName)
        {
            CheckAsyncCall();
            Debug.Assert(prefix != null && namespaceName != null);

            await WriteStartNamespaceDeclarationAsync(prefix).ConfigureAwait(false);
            await WriteStringAsync(namespaceName).ConfigureAwait(false);
            await WriteEndNamespaceDeclarationAsync().ConfigureAwait(false);
        }

        internal override async Task WriteStartNamespaceDeclarationAsync(string prefix)
        {
            CheckAsyncCall();
            Debug.Assert(prefix != null);

            SetTextContentMark(false);

            if (prefix.Length == 0)
            {
                await RawTextAsync(" xmlns=\"").ConfigureAwait(false);
            }
            else
            {
                await RawTextAsync(" xmlns:").ConfigureAwait(false);
                await RawTextAsync(prefix).ConfigureAwait(false);
                buf[bufPos++] = ToBufferType('=');
                buf[bufPos++] = ToBufferType('"');
            }

            inAttributeValue = true;
            SetTextContentMark(true);
        }

        internal override Task WriteEndNamespaceDeclarationAsync()
        {
            CheckAsyncCall();
            SetTextContentMark(false);
            inAttributeValue = false;

            buf[bufPos++] = ToBufferType('"');
            attrEndPos = bufPos;

            return Task.CompletedTask;
        }

        // Serialize a CData section.  If the "]]>" pattern is found within
        // the text, replace it with "]]><![CDATA[>".
        public override async Task WriteCDataAsync(string text)
        {
            CheckAsyncCall();
            Debug.Assert(text != null);

            SetTextContentMark(false);

            if (mergeCDataSections && bufPos == cdataPos)
            {
                // Merge adjacent cdata sections - overwrite the "]]>" characters
                Debug.Assert(bufPos >= 4);
                bufPos -= 3;
            }
            else
            {
                // Start a new cdata section
                buf[bufPos++] = ToBufferType('<');
                buf[bufPos++] = ToBufferType('!');
                buf[bufPos++] = ToBufferType('[');
                buf[bufPos++] = ToBufferType('C');
                buf[bufPos++] = ToBufferType('D');
                buf[bufPos++] = ToBufferType('A');
                buf[bufPos++] = ToBufferType('T');
                buf[bufPos++] = ToBufferType('A');
                buf[bufPos++] = ToBufferType('[');
            }

            await WriteCDataSectionAsync(text).ConfigureAwait(false);

            buf[bufPos++] = ToBufferType(']');
            buf[bufPos++] = ToBufferType(']');
            buf[bufPos++] = ToBufferType('>');

            textPos = bufPos;
            cdataPos = bufPos;
        }

        // Serialize a comment.
        public override async Task WriteCommentAsync(string text)
        {
            CheckAsyncCall();
            Debug.Assert(text != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            buf[bufPos++] = ToBufferType('!');
            buf[bufPos++] = ToBufferType('-');
            buf[bufPos++] = ToBufferType('-');

            await WriteCommentOrPiAsync(text, '-').ConfigureAwait(false);

            buf[bufPos++] = ToBufferType('-');
            buf[bufPos++] = ToBufferType('-');
            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize a processing instruction.
        public override async Task WriteProcessingInstructionAsync(string name, string text)
        {
            CheckAsyncCall();
            Debug.Assert(name != null && name.Length > 0);
            Debug.Assert(text != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            buf[bufPos++] = ToBufferType('?');
            await RawTextAsync(name).ConfigureAwait(false);

            if (text.Length > 0)
            {
                buf[bufPos++] = ToBufferType(' ');
                await WriteCommentOrPiAsync(text, '?').ConfigureAwait(false);
            }

            buf[bufPos++] = ToBufferType('?');
            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize an entity reference.
        public override async Task WriteEntityRefAsync(string name)
        {
            CheckAsyncCall();
            Debug.Assert(name != null && name.Length > 0);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('&');
            await RawTextAsync(name).ConfigureAwait(false);
            buf[bufPos++] = ToBufferType(';');

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

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('&');
            buf[bufPos++] = ToBufferType('#');
            buf[bufPos++] = ToBufferType('x');
            await RawTextAsync(strVal).ConfigureAwait(false);
            buf[bufPos++] = ToBufferType(';');

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
            SetTextContentMark(false);

            if (inAttributeValue)
            {
                return WriteAttributeTextBlockAsync(ws);
            }

            return WriteElementTextBlockAsync(ws);
        }

        // Serialize either attribute or element text using XML rules.

        public override Task WriteStringAsync(string text)
        {
            CheckAsyncCall();
            Debug.Assert(text != null);
            SetTextContentMark(true);

            if (inAttributeValue)
            {
                return WriteAttributeTextBlockAsync(text);
            }

            return WriteElementTextBlockAsync(text);
        }

        // Serialize surrogate character entity.
        public override async Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
        {
            CheckAsyncCall();
            SetTextContentMark(false);
            int surrogateChar = XmlCharType.CombineSurrogateChar(lowChar, highChar);

            buf[bufPos++] = ToBufferType('&');
            buf[bufPos++] = ToBufferType('#');
            buf[bufPos++] = ToBufferType('x');
            await RawTextAsync(surrogateChar.ToString("X", NumberFormatInfo.InvariantInfo)).ConfigureAwait(false);
            buf[bufPos++] = ToBufferType(';');
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

            SetTextContentMark(true);

            if (inAttributeValue)
            {
                return WriteAttributeTextBlockAsync(buffer, index, count);
            }

            return WriteElementTextBlockAsync(buffer, index, count);
        }

        // Serialize raw data.
        // Arguments are validated in the XmlWellformedWriter layer

        public override async Task WriteRawAsync(char[] buffer, int index, int count)
        {
            CheckAsyncCall();
            Debug.Assert(buffer != null);
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0 && index + count <= buffer.Length);

            SetTextContentMark(false);

            await WriteRawWithCharCheckingAsync(buffer, index, count).ConfigureAwait(false);

            textPos = bufPos;
        }

        // Serialize raw data.

        public override async Task WriteRawAsync(string data)
        {
            CheckAsyncCall();
            Debug.Assert(data != null);

            SetTextContentMark(false);

            await WriteRawWithCharCheckingAsync(data).ConfigureAwait(false);

            textPos = bufPos;
        }

        // Serialize text that is part of an attribute value.  The '&', '<', '>', and '"' characters
        // are entitized.
        protected unsafe int WriteAttributeTextBlockNoFlush(char* pSrc, char* pSrcEnd)
        {
            char* pRaw = pSrc;

            fixed (T* pDstBegin = buf)
            {
                T* pDst = pDstBegin + bufPos;

                int ch = 0;
                for (; ; )
                {
                    T* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && (xmlCharType.IsAttributeValueChar((char)(ch = *pSrc)) && WhileIsAttr(ch)))
                    {
                        *pDst = ToBufferType(ch);
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
                            *pDst = ToBufferType(ch);
                            pDst++;
                            break;
                        case (char)0x9:
                            if (newLineHandling == NewLineHandling.None)
                            {
                                *pDst = ToBufferType(ch);
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
                                *pDst = ToBufferType(ch);
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
                                *pDst = ToBufferType(ch);
                                pDst++;
                            }
                            else
                            {
                                // escape new lines in attributes
                                pDst = LineFeedEntity(pDst);
                            }
                            break;
                        default:
                            EncodeChar(true, ch, ref pSrc, ref pSrcEnd, ref pDst);
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

            fixed (T* pDstBegin = buf)
            {
                T* pDst = pDstBegin + bufPos;

                int ch = 0;
                for (; ; )
                {
                    T* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && xmlCharType.IsAttributeValueChar((char)(ch = *pSrc)) && WhileIsAttr(ch))
                    {
                        *pDst = ToBufferType(ch);
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
                            *pDst = ToBufferType(ch);
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
                                *pDst = ToBufferType(ch);
                                pDst++;
                            }
                            break;
                        case (char)0xD:
                            switch (newLineHandling)
                            {
                                case NewLineHandling.Replace:
                                    // Replace "\r\n", or "\r" with NewLineChars
                                    if (pSrc + 1 < pSrcEnd && pSrc[1] == '\n')
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
                                    *pDst = ToBufferType(ch);
                                    pDst++;
                                    break;
                            }
                            break;
                        default:
                            EncodeChar(true, ch, ref pSrc, ref pSrcEnd, ref pDst);
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

            writeLen = WriteElementTextBlockNoFlush(text, curIndex, leftCount, out bool needWriteNewLine);
            curIndex += writeLen;
            leftCount -= writeLen;
            if (needWriteNewLine)
            {
                return _WriteElementTextBlockAsync(true, text, curIndex, leftCount);
            }

            if (writeLen >= 0)
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

            fixed (T* pDstBegin = buf)
            {
                T* pDst = pDstBegin + bufPos;
                char* pSrc = pSrcBegin;

                int ch = 0;
                for (; ; )
                {
                    T* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && WhileNotSurHighStart((char)(ch = *pSrc)))
                    {
                        pSrc++;
                        *pDst = ToBufferType(ch);
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

                    EncodeChar(false, ch, ref pSrc, ref pSrcEnd, ref pDst);
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

            fixed (T* pDstBegin = buf)
            {
                char* pSrc = pSrcBegin;
                T* pDst = pDstBegin + bufPos;

                int ch = 0;
                for (; ; )
                {
                    T* pDstEnd = pDst + (pSrcEnd - pSrc);
                    if (pDstEnd > pDstBegin + bufLen)
                    {
                        pDstEnd = pDstBegin + bufLen;
                    }

                    while (pDst < pDstEnd && xmlCharType.IsTextChar((char)(ch = *pSrc)) && WhileIsAttr(ch))
                    {
                        *pDst = ToBufferType(ch);
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
                            *pDst = ToBufferType(ch);
                            pDst++;
                            break;
                        case (char)0xD:
                            if (newLineHandling == NewLineHandling.Replace)
                            {
                                // Normalize "\r\n", or "\r" to NewLineChars
                                if (pSrc + 1 < pSrcEnd && pSrc[1] == '\n')
                                {
                                    pSrc++;
                                }

                                bufPos = (int)(pDst - pDstBegin);
                                needWriteNewLine = true;
                                return (int)(pSrc - pRaw);
                            }
                            else
                            {
                                *pDst = ToBufferType(ch);
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
                                *pDst = ToBufferType(ch);
                                pDst++;
                            }
                            break;
                        default:
                            EncodeChar(false, ch, ref pSrc, ref pSrcEnd, ref pDst);
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

                fixed (T* pDstBegin = buf)
                {
                    char* pSrc = pSrcBegin;

                    char* pRaw = pSrc;

                    char* pSrcEnd = pSrcBegin + count;

                    T* pDst = pDstBegin + bufPos;

                    int ch = 0;
                    for (; ; )
                    {
                        T* pDstEnd = pDst + (pSrcEnd - pSrc);
                        if (pDstEnd > pDstBegin + bufLen)
                        {
                            pDstEnd = pDstBegin + bufLen;
                        }

                        while (pDst < pDstEnd && xmlCharType.IsTextChar((char)(ch = *pSrc)) && ch != stopChar && WhileIsAttr(ch))
                        {
                            *pDst = ToBufferType(ch);
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
                                *pDst = ToBufferType('-');
                                pDst++;
                                if (ch == stopChar)
                                {
                                    // Insert space between adjacent dashes or before comment's end dashes
                                    if (pSrc + 1 == pSrcEnd || *(pSrc + 1) == '-')
                                    {
                                        *pDst = ToBufferType(' ');
                                        pDst++;
                                    }
                                }
                                break;
                            case '?':
                                *pDst = ToBufferType('?');
                                pDst++;
                                if (ch == stopChar)
                                {
                                    // Processing instruction: insert space between adjacent '?' and '>'
                                    if (pSrc + 1 < pSrcEnd && *(pSrc + 1) == '>')
                                    {
                                        *pDst = ToBufferType(' ');
                                        pDst++;
                                    }
                                }
                                break;
                            case ']':
                                *pDst = ToBufferType(']');
                                pDst++;
                                break;
                            case (char)0xD:
                                if (newLineHandling == NewLineHandling.Replace)
                                {
                                    // Normalize "\r\n", or "\r" to NewLineChars
                                    if (pSrc + 1 < pSrcEnd && pSrc[1] == '\n')
                                    {
                                        pSrc++;
                                    }

                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);
                                }
                                else
                                {
                                    *pDst = ToBufferType(ch);
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
                                    *pDst = ToBufferType(ch);
                                    pDst++;
                                }
                                break;
                            case '<':
                            case '&':
                            case (char)0x9:
                                *pDst = ToBufferType(ch);
                                pDst++;
                                break;
                            default:
                                EncodeChar(false, ch, ref pSrc, ref pSrcEnd, ref pDst);
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

                fixed (T* pDstBegin = buf)
                {
                    char* pSrc = pSrcBegin;

                    char* pSrcEnd = pSrcBegin + count;

                    char* pRaw = pSrc;

                    T* pDst = pDstBegin + bufPos;

                    int ch = 0;
                    for (; ; )
                    {
                        T* pDstEnd = pDst + (pSrcEnd - pSrc);
                        if (pDstEnd > pDstBegin + bufLen)
                        {
                            pDstEnd = pDstBegin + bufLen;
                        }

                        while (pDst < pDstEnd && xmlCharType.IsAttributeValueChar((char)(ch = *pSrc)) && ch != ']' && WhileIsAttr(ch))
                        {
                            *pDst = ToBufferType(ch);
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
                                if (hadDoubleBracket && CompareBufferType(pDst[-1], ']'))
                                {   // pDst[-1] will always correct - there is a padding character at buf[0]
                                    // The characters "]]>" were found within the CData text
                                    pDst = RawEndCData(pDst);
                                    pDst = RawStartCData(pDst);
                                }
                                *pDst = ToBufferType('>');
                                pDst++;
                                break;
                            case ']':
                                if (CompareBufferType(pDst[-1], ']'))
                                {   // pDst[-1] will always correct - there is a padding character at buf[0]
                                    hadDoubleBracket = true;
                                }
                                else
                                {
                                    hadDoubleBracket = false;
                                }
                                *pDst = ToBufferType(']');
                                pDst++;
                                break;
                            case (char)0xD:
                                if (newLineHandling == NewLineHandling.Replace)
                                {
                                    // Normalize "\r\n", or "\r" to NewLineChars
                                    if (pSrc + 1 < pSrcEnd && pSrc[1] == '\n')
                                    {
                                        pSrc++;
                                    }

                                    bufPos = (int)(pDst - pDstBegin);
                                    needWriteNewLine = true;
                                    return (int)(pSrc - pRaw);
                                }
                                else
                                {
                                    *pDst = ToBufferType(ch);
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
                                    *pDst = ToBufferType(ch);
                                    pDst++;
                                }
                                break;
                            case '&':
                            case '<':
                            case '"':
                            case '\'':
                            case (char)0x9:
                                *pDst = ToBufferType(ch);
                                pDst++;
                                break;
                            default:
                                EncodeChar(false, ch, ref pSrc, ref pSrcEnd, ref pDst);
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

        #region Indent methods
        protected async Task WriteDocTypeIndentAsync()
        {
            CheckAsyncCall();
            // Add indentation
            if (!_mixedContent && textPos != bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
        }

        protected async Task WriteStartElementIndentAsync()
        {
            CheckAsyncCall();

            // Add indentation
            if (!_mixedContent && textPos != bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
            _indentLevel++;
            _mixedContentStack.PushBit(_mixedContent);

        }

        protected async Task WriteEndElementIndentAsync()
        {
            CheckAsyncCall();
            // Add indentation
            _indentLevel--;
            if (!_mixedContent && contentPos != bufPos)
            {
                // There was content, so try to indent
                if (textPos != bufPos)
                {
                    await WriteIndentAsync().ConfigureAwait(false);
                }
            }
            _mixedContent = _mixedContentStack.PopBit();
        }

        protected async Task WriteFullEndElementIndentAsync()
        {
            CheckAsyncCall();
            // Add indentation
            _indentLevel--;
            if (!_mixedContent && contentPos != bufPos)
            {
                // There was content, so try to indent
                if (textPos != bufPos)
                {
                    await WriteIndentAsync().ConfigureAwait(false);
                }
            }
            _mixedContent = _mixedContentStack.PopBit();
        }

        // Same as base class, plus possible indentation.
        protected async Task WriteStartAttributeIndentAsync()
        {
            CheckAsyncCall();
            // Add indentation
            if (_newLineOnAttributes)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
        }

        protected void WriteCDataIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected async Task WriteCommentIndentAsync()
        {
            CheckAsyncCall();
            if (!_mixedContent && textPos != bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
        }

        protected async Task WriteProcessingInstructionIndentAsync()
        {
            CheckAsyncCall();
            if (!_mixedContent && textPos != bufPos)
            {
                await WriteIndentAsync().ConfigureAwait(false);
            }
        }

        protected void WriteEntityRefIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteCharEntityIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteSurrogateCharEntityIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteWhitespaceIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteStringIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteCharsIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteRawIndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        protected void WriteBase64IndentAsync()
        {
            CheckAsyncCall();
            _mixedContent = true;
        }

        // Add indentation to output.  Write newline and then repeat IndentChars for each indent level.
        private async Task WriteIndentAsync()
        {
            CheckAsyncCall();
            await RawTextAsync(newLineChars).ConfigureAwait(false);
            for (int i = _indentLevel; i > 0; i--)
            {
                await RawTextAsync(_indentChars).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
