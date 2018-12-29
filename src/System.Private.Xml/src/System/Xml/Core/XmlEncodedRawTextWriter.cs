// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{
    internal unsafe partial class XmlEncodedRawTextWriter : XmlRawTextWriter<char>
    {
        protected override bool WriteStartAttributeChecksChangeTextContentMark => true;

        protected byte[] bufBytes;
        protected int bufBytesUsed;

        // encoder for encoding chars in specified encoding when writing to stream
        protected Encoder encoder;

        // output text writer
        protected TextWriter writer;

        // escaping of characters invalid in the output encoding
        protected bool trackTextContent;
        protected bool inTextContent;
        private int _lastMarkPos;
        private int[] _textContentMarks;   // even indices contain text content start positions
        // odd indices contain markup start positions
        private CharEntityEncoderFallback _charEntityFallback;

        public XmlEncodedRawTextWriter(XmlWriterSettings settings) : base(settings)
        {
        }

        // Construct an instance of this class that outputs text to the TextWriter interface.
        public XmlEncodedRawTextWriter(TextWriter writer, XmlWriterSettings settings) : base(settings)
        {
            Debug.Assert(writer != null && settings != null);

            this.writer = writer;
            encoding = writer.Encoding;
            // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
            if (settings.Async)
            {
                bufLen = ASYNCBUFSIZE;
            }
            buf = new char[bufLen + OVERFLOW];

            // Write the xml declaration
            if (settings.AutoXmlDeclaration)
            {
                WriteXmlDeclaration(standalone);
                autoXmlDeclaration = true;
            }
        }

        // Construct an instance of this class that serializes to a Stream interface.
        public XmlEncodedRawTextWriter(Stream stream, XmlWriterSettings settings) : this(settings)
        {
            Debug.Assert(stream != null && settings != null);

            this.stream = stream;
            encoding = settings.Encoding;

            // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
            if (settings.Async)
            {
                bufLen = ASYNCBUFSIZE;
            }

            buf = new char[bufLen + OVERFLOW];

            bufBytes = new byte[buf.Length];
            bufBytesUsed = 0;

            // Init escaping of characters not fitting into the target encoding
            trackTextContent = true;
            inTextContent = false;
            _lastMarkPos = 0;
            _textContentMarks = new int[INIT_MARKS_COUNT];
            _textContentMarks[0] = 1;

            _charEntityFallback = new CharEntityEncoderFallback();

            // grab bom before possibly changing encoding settings
            ReadOnlySpan<byte> bom = encoding.Preamble;

            // the encoding instance this creates can differ from the one passed in
            encoding = Encoding.GetEncoding(
                settings.Encoding.CodePage,
                _charEntityFallback,
                settings.Encoding.DecoderFallback);

            encoder = encoding.GetEncoder();

            if (!stream.CanSeek || stream.Position == 0)
            {
                if (bom.Length != 0)
                {
                    this.stream.Write(bom);
                }
            }

            // Write the xml declaration
            if (settings.AutoXmlDeclaration)
            {
                WriteXmlDeclaration(standalone);
                autoXmlDeclaration = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool WhileIsAttr(int ch) => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool WhileNotSurHighStart(int ch) => ch < XmlCharType.SurHighStart;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override char ToBufferType(char ch) => ch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override char ToBufferType(int ch) => (char)ch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool CompareBufferType(char l, char ch) => l == ch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SetTextContentMark(bool value)
        {
            if (trackTextContent && inTextContent != value)
            {
                ChangeTextContentMark(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void EncodeChar(
            bool entitizeInvalidChars,
            int ch, ref char* pSrc, ref char* pSrcEnd, ref char* pDst)
        {
            /* Surrogate character */
            if (XmlCharType.IsSurrogate(ch))
            {
                pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst);
                pSrc += 2;
            }
            /* Invalid XML character */
            else if (ch <= 0x7F || ch >= 0xFFFE)
            {
                pDst = InvalidXmlChar(ch, pDst, entitizeInvalidChars);
                pSrc++;
            }
            /* Other character between SurLowEnd and 0xFFFE */
            else
            {
                *pDst = (char)ch;
                pDst++;
                pSrc++;
            }
        }

        public override void Close()
        {
            try
            {
                FlushBuffer();
                FlushEncoder();
            }
            finally
            {
                // Future calls to Close or Flush shouldn't write to Stream or Writer
                writeToNull = true;

                if (stream != null)
                {
                    try
                    {
                        stream.Flush();
                    }
                    finally
                    {
                        try
                        {
                            if (closeOutput)
                            {
                                stream.Dispose();
                            }
                        }
                        finally
                        {
                            stream = null;
                        }
                    }
                }
                else if (writer != null)
                {
                    try
                    {
                        writer.Flush();
                    }
                    finally
                    {
                        try
                        {
                            if (closeOutput)
                            {
                                writer.Dispose();
                            }
                        }
                        finally
                        {
                            writer = null;
                        }
                    }
                }
            }
        }

        // Flush all characters in the buffer to output and call Flush() on the output object.
        public override void Flush()
        {
            FlushBuffer();
            FlushEncoder();

            if (stream != null)
            {
                stream.Flush();
            }
            else if (writer != null)
            {
                writer.Flush();
            }
        }

        protected override void FlushBuffer()
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
                        EncodeChars(1, bufPos, true);
                    }
                    else
                    {
                        // Write text to TextWriter
                        writer.Write(buf, 1, bufPos - 1);
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
                textPos = (textPos == bufPos) ? 1 : 0;
                attrEndPos = (attrEndPos == bufPos) ? 1 : 0;
                contentPos = 0;    // Needs to be zero, since overwriting '>' character is no longer possible
                cdataPos = 0;      // Needs to be zero, since overwriting ']]>' characters is no longer possible
                bufPos = 1;        // Buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
                                   // close an empty element or in CDATA section detection of double ]; buffer[0] will always be 0
            }
        }

        private void EncodeChars(int startOffset, int endOffset, bool writeAllToStream)
        {
            // Write encoded text to stream
            while (startOffset < endOffset)
            {
                if (_charEntityFallback != null)
                {
                    _charEntityFallback.StartOffset = startOffset;
                }
                encoder.Convert(buf, startOffset, endOffset - startOffset,
                    bufBytes, bufBytesUsed, bufBytes.Length - bufBytesUsed, false,
                    out int chEnc, out int bEnc, out bool _);
                startOffset += chEnc;
                bufBytesUsed += bEnc;
                if (bufBytesUsed >= bufBytes.Length - 16)
                {
                    stream.Write(bufBytes, 0, bufBytesUsed);
                    bufBytesUsed = 0;
                }
            }
            if (writeAllToStream && bufBytesUsed > 0)
            {
                stream.Write(bufBytes, 0, bufBytesUsed);
                bufBytesUsed = 0;
            }
        }

        private void FlushEncoder()
        {
            Debug.Assert(bufPos == 1);
            if (stream != null)
            {
                // decode no chars, just flush
                encoder.Convert(buf, 1, 0, bufBytes, 0, bufBytes.Length,
                    true, out int _, out int bEnc, out bool _);
                if (bEnc != 0)
                {
                    stream.Write(bufBytes, 0, bEnc);
                }
            }
        }

        protected char* InvalidXmlChar(int ch, char* pDst, bool entitize)
        {
            Debug.Assert(!xmlCharType.IsWhiteSpace((char)ch));
            Debug.Assert(!xmlCharType.IsAttributeValueChar((char)ch));

            if (checkCharacters)
            {
                // This method will never be called on surrogates, so it is ok to pass in '\0' to the CreateInvalidCharException
                throw XmlConvert.CreateInvalidCharException((char)ch, '\0');
            }

            if (entitize)
            {
                return CharEntity(pDst, (char)ch);
            }

            *pDst = ToBufferType(ch);
            pDst++;
            return pDst;
        }

        protected char* EncodeSurrogate(char* pSrc, char* pSrcEnd, char* pDst)
        {
            Debug.Assert(XmlCharType.IsSurrogate(*pSrc));

            int ch = *pSrc;
            if (ch > XmlCharType.SurHighEnd)
            {
                throw XmlConvert.CreateInvalidHighSurrogateCharException((char)ch);
            }

            if (pSrc + 1 >= pSrcEnd)
            {
                throw new ArgumentException(SR.Xml_InvalidSurrogateMissingLowChar);
            }

            int lowChar = pSrc[1];
            if (lowChar < XmlCharType.SurLowStart || (!LocalAppContextSwitches.DontThrowOnInvalidSurrogatePairs &&
                lowChar > XmlCharType.SurLowEnd))
            {
                throw XmlConvert.CreateInvalidSurrogatePairException((char)lowChar, (char)ch);
            }

            pDst[0] = ToBufferType(ch);
            pDst[1] = (char)lowChar;
            pDst += 2;
            return pDst;
        }

        protected void ChangeTextContentMark(bool value)
        {
            Debug.Assert(inTextContent != value);
            Debug.Assert(inTextContent || ((_lastMarkPos & 1) == 0));
            inTextContent = value;
            if (_lastMarkPos + 1 == _textContentMarks.Length)
            {
                GrowTextContentMarks();
            }
            _textContentMarks[++_lastMarkPos] = bufPos;
        }

        private void GrowTextContentMarks()
        {
            Debug.Assert(_lastMarkPos + 1 == _textContentMarks.Length);
            int[] newTextContentMarks = new int[_textContentMarks.Length * 2];
            Array.Copy(_textContentMarks, newTextContentMarks, _textContentMarks.Length);
            _textContentMarks = newTextContentMarks;
        }
    }

    internal class XmlEncodedRawTextWriterIndent : XmlEncodedRawTextWriter
    {
        public XmlEncodedRawTextWriterIndent(TextWriter writer, XmlWriterSettings settings) : base(writer, settings)
        {
            InitIndent(settings);
        }

        public XmlEncodedRawTextWriterIndent(Stream stream, XmlWriterSettings settings) : base(stream, settings)
        {
            InitIndent(settings);
        }

        public override XmlWriterSettings Settings => GetSettingsIndent();

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            WriteDocTypeIndent();
            base.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            Debug.Assert(localName != null && localName.Length != 0 && prefix != null && ns != null);
            WriteStartElementIndent();
            base.WriteStartElement(prefix, localName, ns);
        }

        internal override void StartElementContent()
        {
            StartElementContentIndent();
            base.StartElementContent();
        }

        internal override void OnRootElement(ConformanceLevel currentConformanceLevel)
        {
            conformanceLevel = currentConformanceLevel;
        }

        internal override void WriteEndElement(string prefix, string localName, string ns)
        {
            WriteEndElementIndent();
            base.WriteEndElement(prefix, localName, ns);
        }

        internal override void WriteFullEndElement(string prefix, string localName, string ns)
        {
            WriteFullEndElementIndent();
            base.WriteFullEndElement(prefix, localName, ns);
        }

        // Same as base class, plus possible indentation.
        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            WriteStartAttributeIndent();
            base.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteCData(string text)
        {
            WriteCDataIndent();
            base.WriteCData(text);
        }

        public override void WriteComment(string text)
        {
            WriteCommentIndent();
            base.WriteComment(text);
        }

        public override void WriteProcessingInstruction(string target, string text)
        {
            WriteProcessingInstructionIndent();
            base.WriteProcessingInstruction(target, text);
        }

        public override void WriteEntityRef(string name)
        {
            WriteEntityRefIndent();
            base.WriteEntityRef(name);
        }

        public override void WriteCharEntity(char ch)
        {
            WriteCharEntityIndent();
            base.WriteCharEntity(ch);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            WriteSurrogateCharEntityIndent();
            base.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteWhitespace(string ws)
        {
            WriteWhitespaceIndent();
            base.WriteWhitespace(ws);
        }

        public override void WriteString(string text)
        {
            WriteStringIndent();
            base.WriteString(text);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            WriteCharsIndent();
            base.WriteChars(buffer, index, count);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            WriteRawIndent();
            base.WriteRaw(buffer, index, count);
        }

        public override void WriteRaw(string data)
        {
            WriteRawIndent();
            base.WriteRaw(data);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            WriteBase64Indent();
            base.WriteBase64(buffer, index, count);
        }

        #region async
        public override async Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
        {
            await WriteDocTypeIndentAsync();
            await base.WriteDocTypeAsync(name, pubid, sysid, subset).ConfigureAwait(false);
        }

        public override async Task WriteStartElementAsync(string prefix, string localName, string ns)
        {
            Debug.Assert(localName != null && localName.Length != 0 && prefix != null && ns != null);
            await WriteStartElementIndentAsync();
            await base.WriteStartElementAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        internal override async Task WriteEndElementAsync(string prefix, string localName, string ns)
        {
            await WriteEndElementIndentAsync();
            await base.WriteEndElementAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        internal override async Task WriteFullEndElementAsync(string prefix, string localName, string ns)
        {
            await WriteFullEndElementIndentAsync();
            await base.WriteFullEndElementAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        // Same as base class, plus possible indentation.
        protected internal override async Task WriteStartAttributeAsync(string prefix, string localName, string ns)
        {
            await WriteStartAttributeIndentAsync();
            await base.WriteStartAttributeAsync(prefix, localName, ns).ConfigureAwait(false);
        }

        public override Task WriteCDataAsync(string text)
        {
            WriteCDataIndentAsync();
            return base.WriteCDataAsync(text);
        }

        public override async Task WriteCommentAsync(string text)
        {
            await WriteCommentIndentAsync();
            await base.WriteCommentAsync(text).ConfigureAwait(false);
        }

        public override async Task WriteProcessingInstructionAsync(string target, string text)
        {
            await WriteProcessingInstructionIndentAsync();
            await base.WriteProcessingInstructionAsync(target, text).ConfigureAwait(false);
        }

        public override Task WriteEntityRefAsync(string name)
        {
            WriteEntityRefIndentAsync();
            return base.WriteEntityRefAsync(name);
        }

        public override Task WriteCharEntityAsync(char ch)
        {
            WriteCharEntityIndentAsync();
            return base.WriteCharEntityAsync(ch);
        }

        public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
        {
            WriteSurrogateCharEntityIndentAsync();
            return base.WriteSurrogateCharEntityAsync(lowChar, highChar);
        }

        public override Task WriteWhitespaceAsync(string ws)
        {
            WriteWhitespaceIndentAsync();
            return base.WriteWhitespaceAsync(ws);
        }

        public override Task WriteStringAsync(string text)
        {
            WriteStringIndentAsync();
            return base.WriteStringAsync(text);
        }

        public override Task WriteCharsAsync(char[] buffer, int index, int count)
        {
            WriteCharsIndentAsync();
            return base.WriteCharsAsync(buffer, index, count);
        }

        public override Task WriteRawAsync(char[] buffer, int index, int count)
        {
            WriteRawIndentAsync();
            return base.WriteRawAsync(buffer, index, count);
        }

        public override Task WriteRawAsync(string data)
        {
            WriteRawIndentAsync();
            return base.WriteRawAsync(data);
        }

        public override Task WriteBase64Async(byte[] buffer, int index, int count)
        {
            WriteBase64IndentAsync();
            return base.WriteBase64Async(buffer, index, count);
        }
        #endregion
    }
}

