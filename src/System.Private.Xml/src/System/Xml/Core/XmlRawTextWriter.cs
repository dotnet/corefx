// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Xml
{
    internal abstract unsafe partial class XmlRawTextWriter<T> : XmlRawWriter
        where T : unmanaged
    {
        protected const int BUFSIZE = 2048 * 3;       // Should be greater than default FileStream size (4096), otherwise the FileStream will try to cache the data
        protected const int ASYNCBUFSIZE = 64 * 1024; // Set async buffer size to 64KB
        protected const int OVERFLOW = 32;            // Allow overflow in order to reduce checks when writing out constant size markup
        protected const int INIT_MARKS_COUNT = 64;

        // main buffer
        protected T[] buf;

        // output stream
        protected Stream stream;

        // encoding of the stream or text writer
        protected Encoding encoding;

        // char type tables
        protected XmlCharType xmlCharType = XmlCharType.Instance;

        // buffer positions
        protected int bufPos = 1;     // buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
        // close an empty element or in CDATA section detection of double ]; buf[0] will always be 0
        protected int textPos = 1;    // text end position; don't indent first element, pi, or comment
        protected int contentPos;     // element content end position
        protected int cdataPos;       // cdata end position
        protected int attrEndPos;     // end of the last attribute
        protected int bufLen = BUFSIZE;

        // flags
        protected bool writeToNull;
        protected bool hadDoubleBracket;
        protected bool inAttributeValue;

        // writer settings
        private readonly bool _useAsync;
        protected NewLineHandling newLineHandling;
        protected bool closeOutput;
        protected bool omitXmlDeclaration;
        protected string newLineChars;
        protected bool checkCharacters;

        protected XmlStandalone standalone;
        protected XmlOutputMethod outputMethod;

        protected bool autoXmlDeclaration;
        protected readonly bool mergeCDataSections;

        // Indented members
        private int _indentLevel;
        private bool _newLineOnAttributes;
        private string _indentChars;

        private bool _mixedContent;
        private BitStack _mixedContentStack;

        protected ConformanceLevel conformanceLevel = ConformanceLevel.Auto;

        protected abstract bool WriteStartAttributeChecksChangeTextContentMark { get; }
        internal override bool SupportsNamespaceDeclarationInChunks => true;

        // Construct and initialize an instance of this class.
        protected XmlRawTextWriter(XmlWriterSettings settings)
        {
            _useAsync = settings.Async;

            // copy settings
            newLineHandling = settings.NewLineHandling;
            omitXmlDeclaration = settings.OmitXmlDeclaration;
            newLineChars = settings.NewLineChars;
            checkCharacters = settings.CheckCharacters;
            closeOutput = settings.CloseOutput;

            standalone = settings.Standalone;
            outputMethod = settings.OutputMethod;
            mergeCDataSections = settings.MergeCDataSections;

            if (checkCharacters && newLineHandling == NewLineHandling.Replace)
            {
                ValidateContentChars(newLineChars, "NewLineChars", false);
            }
        }

        //
        // XmlWriter implementation
        //
        // Returns settings the writer currently applies.
        public override XmlWriterSettings Settings
        {
            get
            {
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.Encoding = encoding;
                settings.OmitXmlDeclaration = omitXmlDeclaration;
                settings.NewLineHandling = newLineHandling;
                settings.NewLineChars = newLineChars;
                settings.CloseOutput = closeOutput;
                settings.ConformanceLevel = ConformanceLevel.Auto;
                settings.CheckCharacters = checkCharacters;

                settings.AutoXmlDeclaration = autoXmlDeclaration;
                settings.Standalone = standalone;
                settings.OutputMethod = outputMethod;

                settings.ReadOnly = true;
                return settings;
            }
        }

        // Write the xml declaration.  This must be the first call.
        internal override void WriteXmlDeclaration(XmlStandalone standalone)
        {
            // Output xml declaration only if user allows it and it was not already output
            if (!omitXmlDeclaration && !autoXmlDeclaration)
            {
                SetTextContentMark(false);

                RawText("<?xml version=\"");

                // Version
                RawText("1.0");

                // Encoding
                if (encoding != null)
                {
                    RawText("\" encoding=\"");
                    RawText(encoding.WebName);
                }

                // Standalone
                if (standalone != XmlStandalone.Omit)
                {
                    RawText("\" standalone=\"");
                    RawText(standalone == XmlStandalone.Yes ? "yes" : "no");
                }

                RawText("\"?>");
            }
        }

        protected abstract bool WhileIsAttr(int ch);
        protected abstract bool WhileNotSurHighStart(int ch);
        protected abstract T ToBufferType(char ch);
        protected abstract T ToBufferType(int ch);
        protected abstract bool CompareBufferType(T l, char ch);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SetTextContentMark(bool value)
        {
        }

        protected abstract void EncodeChar(
            bool entitizeInvalidChars,
            int ch, ref char* pSrc, ref char* pSrcEnd, ref T* pDst);

        internal override void WriteXmlDeclaration(string xmldecl)
        {
            // Output xml declaration only if user allows it and it was not already output
            if (!omitXmlDeclaration && !autoXmlDeclaration)
            {
                WriteProcessingInstruction("xml", xmldecl);
            }
        }

        // Serialize the document type declaration.
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            Debug.Assert(name != null && name.Length > 0);

            SetTextContentMark(false);

            RawText("<!DOCTYPE ");
            RawText(name);
            if (pubid != null)
            {
                RawText(" PUBLIC \"");
                RawText(pubid);
                RawText("\" \"");
                if (sysid != null)
                {
                    RawText(sysid);
                }
                buf[bufPos++] = ToBufferType('"');
            }
            else if (sysid != null)
            {
                RawText(" SYSTEM \"");
                RawText(sysid);
                buf[bufPos++] = ToBufferType('"');
            }
            else
            {
                buf[bufPos++] = ToBufferType(' ');
            }

            if (subset != null)
            {
                buf[bufPos++] = ToBufferType('[');
                RawText(subset);
                buf[bufPos++] = ToBufferType(']');
            }

            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize the beginning of an element start tag: "<prefix:localName"
        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            if (prefix != null && prefix.Length != 0)
            {
                RawText(prefix);
                buf[bufPos++] = ToBufferType(':');
            }

            RawText(localName);

            attrEndPos = bufPos;
        }

        // Serialize the end of an element start tag in preparation for content serialization: ">"
        internal override void StartElementContent()
        {
            buf[bufPos++] = ToBufferType('>');

            // StartElementContent is always called; therefore, in order to allow shortcut syntax, we save the
            // position of the '>' character.  If WriteEndElement is called and no other characters have been
            // output, then the '>' character can be overwritten with the shortcut syntax " />".
            contentPos = bufPos;
        }

        // Serialize an element end tag: "</prefix:localName>", if content was output.  Otherwise, serialize
        // the shortcut syntax: " />".
        internal override void WriteEndElement(string prefix, string localName, string ns)
        {
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
                    RawText(prefix);
                    buf[bufPos++] = ToBufferType(':');
                }
                RawText(localName);
                buf[bufPos++] = ToBufferType('>');
            }
            else
            {
                // Use shortcut syntax; overwrite the already output '>' character
                bufPos--;
                buf[bufPos++] = ToBufferType(' ');
                buf[bufPos++] = ToBufferType('/');
                buf[bufPos++] = ToBufferType('>');
            }
        }

        // Serialize a full element end tag: "</prefix:localName>"
        internal override void WriteFullEndElement(string prefix, string localName, string ns)
        {
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            buf[bufPos++] = ToBufferType('/');

            if (prefix != null && prefix.Length != 0)
            {
                RawText(prefix);
                buf[bufPos++] = ToBufferType(':');
            }
            RawText(localName);
            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize an attribute tag using double quotes around the attribute value: 'prefix:localName="'
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            Debug.Assert(localName != null && localName.Length > 0);
            Debug.Assert(prefix != null);

            if (WriteStartAttributeChecksChangeTextContentMark)
            {
                SetTextContentMark(false);
            }

            if (attrEndPos == bufPos)
            {
                buf[bufPos++] = ToBufferType(' ');
            }

            if (prefix != null && prefix.Length > 0)
            {
                RawText(prefix);
                buf[bufPos++] = ToBufferType(':');
            }
            RawText(localName);
            buf[bufPos++] = ToBufferType('=');
            buf[bufPos++] = ToBufferType('"');

            inAttributeValue = true;
        }

        // Serialize the end of an attribute value using double quotes: '"'
        public override void WriteEndAttribute()
        {
            SetTextContentMark(false);
            buf[bufPos++] = ToBufferType('"');
            inAttributeValue = false;
            attrEndPos = bufPos;
        }

        internal override void WriteNamespaceDeclaration(string prefix, string namespaceName)
        {
            Debug.Assert(prefix != null && namespaceName != null);

            WriteStartNamespaceDeclaration(prefix);
            WriteString(namespaceName);
            WriteEndNamespaceDeclaration();
        }

        internal override void WriteStartNamespaceDeclaration(string prefix)
        {
            Debug.Assert(prefix != null);

            SetTextContentMark(false);

            if (prefix.Length == 0)
            {
                RawText(" xmlns=\"");
            }
            else
            {
                RawText(" xmlns:");
                RawText(prefix);
                buf[bufPos++] = ToBufferType('=');
                buf[bufPos++] = ToBufferType('"');
            }

            inAttributeValue = true;
            SetTextContentMark(true);
        }

        internal override void WriteEndNamespaceDeclaration()
        {
            SetTextContentMark(false);
            inAttributeValue = false;

            buf[bufPos++] = ToBufferType('"');
            attrEndPos = bufPos;
        }

        // Serialize a CData section.  If the "]]>" pattern is found within
        // the text, replace it with "]]><![CDATA[>".
        public override void WriteCData(string text)
        {
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

            WriteCDataSection(text);

            buf[bufPos++] = ToBufferType(']');
            buf[bufPos++] = ToBufferType(']');
            buf[bufPos++] = ToBufferType('>');

            textPos = bufPos;
            cdataPos = bufPos;
        }

        // Serialize a comment.
        public override void WriteComment(string text)
        {
            Debug.Assert(text != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            buf[bufPos++] = ToBufferType('!');
            buf[bufPos++] = ToBufferType('-');
            buf[bufPos++] = ToBufferType('-');

            WriteCommentOrPi(text, '-');

            buf[bufPos++] = ToBufferType('-');
            buf[bufPos++] = ToBufferType('-');
            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize a processing instruction.
        public override void WriteProcessingInstruction(string name, string text)
        {
            Debug.Assert(name != null && name.Length > 0);
            Debug.Assert(text != null);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('<');
            buf[bufPos++] = ToBufferType('?');
            RawText(name);

            if (text.Length > 0)
            {
                buf[bufPos++] = ToBufferType(' ');
                WriteCommentOrPi(text, '?');
            }

            buf[bufPos++] = ToBufferType('?');
            buf[bufPos++] = ToBufferType('>');
        }

        // Serialize an entity reference.
        public override void WriteEntityRef(string name)
        {
            Debug.Assert(name != null && name.Length > 0);

            SetTextContentMark(false);

            buf[bufPos++] = ToBufferType('&');
            RawText(name);
            buf[bufPos++] = ToBufferType(';');

            if (bufPos > bufLen)
            {
                FlushBuffer();
            }

            textPos = bufPos;
        }

        // Serialize a character entity reference.
        public override void WriteCharEntity(char ch)
        {
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
            RawText(strVal);
            buf[bufPos++] = ToBufferType(';');

            if (bufPos > bufLen)
            {
                FlushBuffer();
            }

            textPos = bufPos;
        }

        // Serialize a whitespace node.
        public override void WriteWhitespace(string ws)
        {
            Debug.Assert(ws != null);
            SetTextContentMark(false);

            fixed (char* pSrc = ws)
            {
                char* pSrcEnd = pSrc + ws.Length;
                if (inAttributeValue)
                {
                    WriteAttributeTextBlock(pSrc, pSrcEnd);
                }
                else
                {
                    WriteElementTextBlock(pSrc, pSrcEnd);
                }
            }
        }

        // Serialize either attribute or element text using XML rules.
        public override void WriteString(string text)
        {
            Debug.Assert(text != null);
            SetTextContentMark(true);

            fixed (char* pSrc = text)
            {
                char* pSrcEnd = pSrc + text.Length;
                if (inAttributeValue)
                {
                    WriteAttributeTextBlock(pSrc, pSrcEnd);
                }
                else
                {
                    WriteElementTextBlock(pSrc, pSrcEnd);
                }
            }
        }

        // Serialize surrogate character entity.
        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            SetTextContentMark(false);
            int surrogateChar = XmlCharType.CombineSurrogateChar(lowChar, highChar);

            buf[bufPos++] = ToBufferType('&');
            buf[bufPos++] = ToBufferType('#');
            buf[bufPos++] = ToBufferType('x');
            RawText(surrogateChar.ToString("X", NumberFormatInfo.InvariantInfo));
            buf[bufPos++] = ToBufferType(';');
            textPos = bufPos;
        }

        // Serialize either attribute or element text using XML rules.
        // Arguments are validated in the XmlWellformedWriter layer.
        public override void WriteChars(char[] buffer, int index, int count)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0 && index + count <= buffer.Length);

            SetTextContentMark(true);

            fixed (char* pSrcBegin = &buffer[index])
            {
                if (inAttributeValue)
                {
                    WriteAttributeTextBlock(pSrcBegin, pSrcBegin + count);
                }
                else
                {
                    WriteElementTextBlock(pSrcBegin, pSrcBegin + count);
                }
            }
        }

        // Serialize raw data.
        // Arguments are validated in the XmlWellformedWriter layer
        public override void WriteRaw(char[] buffer, int index, int count)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0 && index + count <= buffer.Length);

            SetTextContentMark(false);

            fixed (char* pSrcBegin = &buffer[index])
            {
                WriteRawWithCharChecking(pSrcBegin, pSrcBegin + count);
            }

            textPos = bufPos;
        }

        // Serialize raw data.
        public override void WriteRaw(string data)
        {
            Debug.Assert(data != null);

            SetTextContentMark(false);

            fixed (char* pSrcBegin = data)
            {
                WriteRawWithCharChecking(pSrcBegin, pSrcBegin + data.Length);
            }

            textPos = bufPos;
        }

        protected abstract void FlushBuffer();

        // Serialize text that is part of an attribute value.  The '&', '<', '>', and '"' characters
        // are entitized.
        protected void WriteAttributeTextBlock(char* pSrc, char* pSrcEnd)
        {
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
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
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
        }

        // Serialize text that is part of element content.  The '&', '<', and '>' characters
        // are entitized.
        protected void WriteElementTextBlock(char* pSrc, char* pSrcEnd)
        {
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
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
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
                                pDst = WriteNewLine(pDst);
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

                                    pDst = WriteNewLine(pDst);
                                    break;
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
        }

        protected void RawText(string s)
        {
            Debug.Assert(s != null);

            fixed (char* pSrcBegin = s)
            {
                RawText(pSrcBegin, pSrcBegin + s.Length);
            }
        }

        protected void RawText(char* pSrcBegin, char* pSrcEnd)
        {
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
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    EncodeChar(false, ch, ref pSrc, ref pSrcEnd, ref pDst);
                }

                bufPos = (int)(pDst - pDstBegin);
            }
        }

        private void WriteRawWithCharChecking(char* pSrcBegin, char* pSrcEnd)
        {
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
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
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

                                pDst = WriteNewLine(pDst);
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
                                pDst = WriteNewLine(pDst);
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
        }

        protected void WriteCommentOrPi(string text, int stopChar)
        {
            if (text.Length == 0)
            {
                if (bufPos >= bufLen)
                {
                    FlushBuffer();
                }
                return;
            }
            // write text
            fixed (char* pSrcBegin = text)

            fixed (T* pDstBegin = buf)
            {
                char* pSrc = pSrcBegin;

                char* pSrcEnd = pSrcBegin + text.Length;

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
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
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

                                pDst = WriteNewLine(pDst);
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
                                pDst = WriteNewLine(pDst);
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
        }

        private void WriteCDataSection(string text)
        {
            if (text.Length == 0)
            {
                if (bufPos >= bufLen)
                {
                    FlushBuffer();
                }
                return;
            }

            // write text

            fixed (char* pSrcBegin = text)

            fixed (T* pDstBegin = buf)
            {
                char* pSrc = pSrcBegin;

                char* pSrcEnd = pSrcBegin + text.Length;

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
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    // handle special characters
                    switch (ch)
                    {
                        case '>':
                            if (hadDoubleBracket && CompareBufferType(pDst[-1], ']'))
                            {   // pDst[-1] will always correct - there is a padding character at buffer[0]
                                // The characters "]]>" were found within the CData text
                                pDst = RawEndCData(pDst);
                                pDst = RawStartCData(pDst);
                            }
                            *pDst = ToBufferType('>');
                            pDst++;
                            break;
                        case ']':
                            if (CompareBufferType(pDst[-1], ']'))
                            {   // pDst[-1] will always correct - there is a padding character at buffer[0]
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

                                pDst = WriteNewLine(pDst);
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
                                pDst = WriteNewLine(pDst);
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
        }

        internal void EncodeChar(ref char* pSrc, char* pSrcEnd, ref T* pDst)
        {
            int ch = *pSrc;
            EncodeChar(false, ch, ref pSrc, ref pSrcEnd, ref pDst);
        }

        // Write NewLineChars to the specified buffer position and return an updated position.
        protected T* WriteNewLine(T* pDst)
        {
            fixed (T* pDstBegin = buf)
            {
                bufPos = (int)(pDst - pDstBegin);
                // Let RawText do the real work
                RawText(newLineChars);
                return pDstBegin + bufPos;
            }
        }

        // Following methods do not check whether pDst is beyond the bufSize because the buffer was allocated with a OVERFLOW to accommodate
        // for the writes of small constant-length string as below.

        // Entitize '<' as "&lt;".  Return an updated pointer.
        protected T* LtEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('l');
            pDst[2] = ToBufferType('t');
            pDst[3] = ToBufferType(';');
            return pDst + 4;
        }

        // Entitize '>' as "&gt;".  Return an updated pointer.
        protected T* GtEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('g');
            pDst[2] = ToBufferType('t');
            pDst[3] = ToBufferType(';');
            return pDst + 4;
        }

        // Entitize '&' as "&amp;".  Return an updated pointer.
        protected T* AmpEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('a');
            pDst[2] = ToBufferType('m');
            pDst[3] = ToBufferType('p');
            pDst[4] = ToBufferType(';');
            return pDst + 5;
        }

        // Entitize '"' as "&quot;".  Return an updated pointer.
        protected T* QuoteEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('q');
            pDst[2] = ToBufferType('u');
            pDst[3] = ToBufferType('o');
            pDst[4] = ToBufferType('t');
            pDst[5] = ToBufferType(';');
            return pDst + 6;
        }

        // Entitize '\t' as "&#x9;".  Return an updated pointer.
        protected T* TabEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('#');
            pDst[2] = ToBufferType('x');
            pDst[3] = ToBufferType('9');
            pDst[4] = ToBufferType(';');
            return pDst + 5;
        }

        // Entitize 0xa as "&#xA;".  Return an updated pointer.
        protected T* LineFeedEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('#');
            pDst[2] = ToBufferType('x');
            pDst[3] = ToBufferType('A');
            pDst[4] = ToBufferType(';');
            return pDst + 5;
        }

        // Entitize 0xd as "&#xD;".  Return an updated pointer.
        protected T* CarriageReturnEntity(T* pDst)
        {
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('#');
            pDst[2] = ToBufferType('x');
            pDst[3] = ToBufferType('D');
            pDst[4] = ToBufferType(';');
            return pDst + 5;
        }

        protected T* CharEntity(T* pDst, char ch)
        {
            string s = ((int)ch).ToString("X", NumberFormatInfo.InvariantInfo);
            pDst[0] = ToBufferType('&');
            pDst[1] = ToBufferType('#');
            pDst[2] = ToBufferType('x');
            pDst += 3;

            fixed (char* pSrc = s)
            {
                char* pS = pSrc;
                while (CompareBufferType(*pDst++ = ToBufferType(*pS++), '\0'));
            }

            pDst[-1] = ToBufferType(';');
            return pDst;
        }

        // Write "<![CDATA[" to the specified buffer.  Return an updated pointer.
        protected T* RawStartCData(T* pDst)
        {
            pDst[0] = ToBufferType('<');
            pDst[1] = ToBufferType('!');
            pDst[2] = ToBufferType('[');
            pDst[3] = ToBufferType('C');
            pDst[4] = ToBufferType('D');
            pDst[5] = ToBufferType('A');
            pDst[6] = ToBufferType('T');
            pDst[7] = ToBufferType('A');
            pDst[8] = ToBufferType('[');
            return pDst + 9;
        }

        // Write "]]>" to the specified buffer.  Return an updated pointer.
        protected T* RawEndCData(T* pDst)
        {
            pDst[0] = ToBufferType(']');
            pDst[1] = ToBufferType(']');
            pDst[2] = ToBufferType('>');
            return pDst + 3;
        }

        private void ValidateContentChars(string chars, string propertyName, bool allowOnlyWhitespace)
        {
            if (allowOnlyWhitespace)
            {
                if (!xmlCharType.IsOnlyWhitespace(chars))
                {
                    throw new ArgumentException(SR.Format(SR.Xml_IndentCharsNotWhitespace, propertyName));
                }
            }
            else
            {
                string error = null;
                for (int i = 0; i < chars.Length; i++)
                {
                    if (!xmlCharType.IsTextChar(chars[i]))
                    {
                        switch (chars[i])
                        {
                            case '\n':
                            case '\r':
                            case '\t':
                                continue;
                            case '<':
                            case '&':
                            case ']':
                                error = SR.Format(SR.Xml_InvalidCharacter, XmlException.BuildCharExceptionArgs(chars, i));
                                goto Error;
                            default:
                                if (XmlCharType.IsHighSurrogate(chars[i]))
                                {
                                    if (i + 1 < chars.Length)
                                    {
                                        if (XmlCharType.IsLowSurrogate(chars[i + 1]))
                                        {
                                            i++;
                                            continue;
                                        }
                                    }
                                    error = SR.Xml_InvalidSurrogateMissingLowChar;
                                    goto Error;
                                }
                                else if (XmlCharType.IsLowSurrogate(chars[i]))
                                {
                                    error = SR.Format(SR.Xml_InvalidSurrogateHighChar, ((uint)chars[i]).ToString("X", CultureInfo.InvariantCulture));
                                    goto Error;
                                }
                                continue;
                        }
                    }
                }
                return;

            Error:
                throw new ArgumentException(SR.Format(SR.Xml_InvalidCharsInIndent, new string[] { propertyName, error }));
            }
        }

        #region Indent methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteDocTypeIndent()
        {
            // Add indentation
            if (!_mixedContent && textPos != bufPos)
            {
                WriteIndent();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteStartElementIndent()
        {
            // Add indentation
            if (!_mixedContent && textPos != bufPos)
            {
                WriteIndent();
            }
            _indentLevel++;
            _mixedContentStack.PushBit(_mixedContent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StartElementContentIndent()
        {
            // If this is the root element and we're writing a document
            //   do not inherit the mixedContent flag into the root element.
            //   This is to allow for whitespace nodes on root level
            //   without disabling indentation for the whole document.
            if (_indentLevel == 1 && conformanceLevel == ConformanceLevel.Document)
            {
                _mixedContent = false;
            }
            else
            {
                _mixedContent = _mixedContentStack.PeekBit();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteEndElementIndent()
        {
            // Add indentation
            _indentLevel--;
            if (!_mixedContent && contentPos != bufPos)
            {
                // There was content, so try to indent
                if (textPos != bufPos)
                {
                    WriteIndent();
                }
            }
            _mixedContent = _mixedContentStack.PopBit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteFullEndElementIndent()
        {
            // Add indentation
            _indentLevel--;
            if (!_mixedContent && contentPos != bufPos)
            {
                // There was content, so try to indent
                if (textPos != bufPos)
                {
                    WriteIndent();
                }
            }
            _mixedContent = _mixedContentStack.PopBit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteStartAttributeIndent()
        {
            // Add indentation
            if (_newLineOnAttributes)
            {
                WriteIndent();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteCDataIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteCommentIndent()
        {
            if (!_mixedContent && textPos != bufPos)
            {
                WriteIndent();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteProcessingInstructionIndent()
        {
            if (!_mixedContent && textPos != bufPos)
            {
                WriteIndent();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteEntityRefIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteCharEntityIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteSurrogateCharEntityIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteWhitespaceIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteStringIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteCharsIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteRawIndent()
        {
            _mixedContent = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteBase64Indent()
        {
            _mixedContent = true;
        }

        protected void InitIndent(XmlWriterSettings settings)
        {
            _indentLevel = 0;
            _indentChars = settings.IndentChars;
            _newLineOnAttributes = settings.NewLineOnAttributes;
            _mixedContentStack = new BitStack();

            // check indent characters that they are valid XML characters
            if (checkCharacters)
            {
                if (_newLineOnAttributes)
                {
                    ValidateContentChars(_indentChars, "IndentChars", true);
                    ValidateContentChars(newLineChars, "NewLineChars", true);
                }
                else
                {
                    ValidateContentChars(_indentChars, "IndentChars", false);
                    if (newLineHandling != NewLineHandling.Replace)
                    {
                        ValidateContentChars(newLineChars, "NewLineChars", false);
                    }
                }
            }
        }

        // Add indentation to output.  Write newline and then repeat IndentChars for each indent level.
        private void WriteIndent()
        {
            RawText(newLineChars);
            for (int i = _indentLevel; i > 0; i--)
            {
                RawText(_indentChars);
            }
        }
        #endregion
    }
}
