// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
    internal unsafe partial class XmlUtf8RawTextWriter : XmlRawTextWriter<byte>
    {
        protected override bool WriteStartAttributeChecksChangeTextContentMark => false;

        public XmlUtf8RawTextWriter(XmlWriterSettings settings) : base(settings)
        {
        }

        // Construct an instance of this class that serializes to a Stream interface.
        public XmlUtf8RawTextWriter(Stream stream, XmlWriterSettings settings) : this(settings)
        {
            Debug.Assert(stream != null && settings != null);

            this.stream = stream;
            encoding = settings.Encoding;

            // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
            if (settings.Async)
            {
                bufLen = ASYNCBUFSIZE;
            }

            buf = new byte[bufLen + OVERFLOW];

            // Output UTF-8 byte order mark if Encoding object wants it
            if (!stream.CanSeek || stream.Position == 0)
            {
                ReadOnlySpan<byte> bom = encoding.Preamble;
                if (bom.Length != 0)
                {
                    bom.CopyTo(new Span<byte>(buf).Slice(1));
                    bufPos += bom.Length;
                    textPos += bom.Length;
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
        protected override bool WhileIsAttr(int ch) => ch <= 0x7F;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool WhileNotSurHighStart(int ch) => ch <= 0x7F;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void EncodeChar(bool entitizeInvalidChars,
            int ch, ref char* pSrc, ref char* pSrcEnd, ref byte* pDst)
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
            /* Multibyte UTF8 character */
            else
            {
                pDst = EncodeMultibyteUTF8(ch, pDst);
                pSrc++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override byte ToBufferType(char ch) => (byte)ch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override byte ToBufferType(int ch) => (byte)ch;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool CompareBufferType(byte l, char ch) => (char)l == ch;

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
        }

        protected override void FlushBuffer()
        {
            try
            {
                // Output all characters (except for previous characters stored at beginning of buffer)
                if (!writeToNull)
                {
                    Debug.Assert(stream != null);
                    stream.Write(buf, 1, bufPos - 1);
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
                                   // close an empty element or in CDATA section detection of double ]; buffer[0] will always be 0
            }
        }

        private void FlushEncoder()
        {
            // intentionally empty
        }

        // Returns true if UTF8 encoded byte is first of four bytes that encode a surrogate pair.
        // To do this, detect the bit pattern 11110xxx.
        private static bool IsSurrogateByte(byte b)
        {
            return (b & 0xF8) == 0xF0;
        }

        protected byte* InvalidXmlChar(int ch, byte* pDst, bool entitize)
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

            if (ch < 0x80)
            {
                *pDst = (byte)ch;
                pDst++;
            }
            else
            {
                pDst = EncodeMultibyteUTF8(ch, pDst);
            }
            return pDst;
        }

        private static byte* EncodeSurrogate(char* pSrc, char* pSrcEnd, byte* pDst)
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

            // Calculate Unicode scalar value for easier manipulations (see section 3.7 in Unicode spec)
            // The scalar value repositions surrogate values to start at 0x10000.

            ch = XmlCharType.CombineSurrogateChar(lowChar, ch);

            pDst[0] = (byte)(0xF0 | (ch >> 18));
            pDst[1] = (byte)(0x80 | (ch >> 12) & 0x3F);
            pDst[2] = (byte)(0x80 | (ch >> 6) & 0x3F);
            pDst[3] = (byte)(0x80 | ch & 0x3F);
            pDst += 4;

            return pDst;
        }

        internal static byte* EncodeMultibyteUTF8(int ch, byte* pDst)
        {
            Debug.Assert(ch >= 0x80 && !XmlCharType.IsSurrogate(ch));

            unchecked
            {
                /* UTF8-2: If ch is in 0x80-0x7ff range, then use 2 bytes to encode it */
                if (ch < 0x800)
                {
                    *pDst = (byte)((sbyte)0xC0 | (ch >> 6));
                }
                /* UTF8-3: If ch is anything else, then default to using 3 bytes to encode it. */
                else
                {
                    *pDst = (byte)((sbyte)0xE0 | (ch >> 12));
                    pDst++;

                    *pDst = (byte)((sbyte)0x80 | (ch >> 6) & 0x3F);
                }
            }

            pDst++;
            *pDst = (byte)(0x80 | ch & 0x3F);
            return pDst + 1;
        }

        // Encode *pSrc as a sequence of UTF8 bytes.  Write the bytes to pDst and return an updated pointer.
        internal static void CharToUTF8(ref char* pSrc, char* pSrcEnd, ref byte* pDst)
        {
            int ch = *pSrc;
            if (ch <= 0x7F)
            {
                *pDst = (byte)ch;
                pDst++;
                pSrc++;
            }
            else if (XmlCharType.IsSurrogate(ch))
            {
                pDst = EncodeSurrogate(pSrc, pSrcEnd, pDst);
                pSrc += 2;
            }
            else
            {
                pDst = EncodeMultibyteUTF8(ch, pDst);
                pSrc++;
            }
        }
    }

    internal class XmlUtf8RawTextWriterIndent : XmlUtf8RawTextWriter
    {
        public XmlUtf8RawTextWriterIndent(Stream stream, XmlWriterSettings settings) : base(stream, settings)
        {
            InitIndent(settings);
        }

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

        #region Async
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

