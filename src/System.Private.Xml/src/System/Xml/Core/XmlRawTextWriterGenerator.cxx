//------------------------------------------------------------------------------
// <copyright file="XmlRawTextWriterGenerator.cxx" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">helenak</owner>
//------------------------------------------------------------------------------

// WARNING: This file is generated and should not be modified directly.  Instead,
// modify XmlTextWriterGenerator.cxx and run gen.bat in the same directory.
// This batch file will execute the following commands:
//
//   cl.exe /C /EP /D _XML_UTF8_TEXT_WRITER XmlRawTextWriterGenerator.cxx > XmlUtf8RawTextWriter.cs
//   cl.exe /C /EP /D _XML_ENCODED_TEXT_WRITER XmlRawTextWriterGenerator.cxx > XmlEncodedRawTextWriter.cs
//
// Because these two implementations of XmlTextWriter are so similar, the C++ preprocessor
// is used to generate each implementation from one template file, using macros and ifdefs.

// Note: This file was generated without #define SILVERLIGHT

#ifdef _XML_UTF8_TEXT_WRITER
#define _CLASS_NAME XmlUtf8RawTextWriter
#define _CLASS_NAME_INDENT XmlUtf8RawTextWriterIndent
#define _BUFFER bufBytes
#define _BUFFER_TYPE byte
#define _ENCODE_CHAR(entitizeInvalidChars) \
            /* Surrogate character */   \
            if ( XmlCharType.IsSurrogate( ch ) ) {        \
                pDst = EncodeSurrogate( pSrc, pSrcEnd, pDst );     \
                pSrc += 2;      \
            } \
            /* Invalid XML character */ \
            else if ( ch <= 0x7F || ch >= 0xFFFE ) {  \
                pDst = InvalidXmlChar( ch, pDst, entitizeInvalidChars );  \
                pSrc++; \
            }   \
            /* Multibyte UTF8 character */ \
            else { \
                pDst = EncodeMultibyteUTF8( ch, pDst ); \
                pSrc++; \
            }   

#define _SET_TEXT_CONTENT_MARK(value) 
#endif

#ifdef _XML_ENCODED_TEXT_WRITER
#define _CLASS_NAME XmlEncodedRawTextWriter
#define _CLASS_NAME_INDENT XmlEncodedRawTextWriterIndent
#define _BUFFER bufChars
#define _BUFFER_TYPE char
#define _ENCODE_CHAR(entitizeInvalidChars)  \
            /* Surrogate character */   \
            if ( XmlCharType.IsSurrogate( ch ) ) {        \
                pDst = EncodeSurrogate( pSrc, pSrcEnd, pDst ); \
                pSrc += 2;  \
            }   \
            /* Invalid XML character */ \
            else  if ( ch <= 0x7F || ch >= 0xFFFE ) {  \
                pDst = InvalidXmlChar( ch, pDst, entitizeInvalidChars );  \
                pSrc++; \
            }   \
            /* Other character between SurLowEnd and 0xFFFE */ \
            else {  \
                *pDst = (char)ch;  \
                pDst++; \
                pSrc++; \
            }   \

#define _SET_TEXT_CONTENT_MARK(value) \
    if ( trackTextContent && inTextContent != value ) { \
        ChangeTextContentMark( value );  \
    }

#endif

#define XMLCHARTYPE_TEST(ch, flag)              ( ( xmlCharType.charProperties[ch] & XmlCharType.##flag ) != 0 )
#define XMLCHARTYPE_ISATTRIBUTEVALUECHAR(ch)    XMLCHARTYPE_TEST(ch, fAttrValue)
#define XMLCHARTYPE_ISTEXT(ch)                  XMLCHARTYPE_TEST(ch, fText)


using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace System.Xml {

    // Concrete implementation of XmlWriter abstract class that serializes events as encoded XML
    // text.  The general-purpose XmlEncodedTextWriter uses the Encoder class to output to any
    // encoding.  The XmlUtf8TextWriter class combined the encoding operation with serialization
    // in order to achieve better performance.
    internal class _CLASS_NAME : XmlRawWriter {
//
// Fields
//
        // main buffer
        protected byte[] bufBytes;

        // output stream
        protected Stream    stream;

        // encoding of the stream or text writer 
        protected Encoding encoding;

        // char type tables
        protected XmlCharType xmlCharType = XmlCharType.Instance;

        // buffer positions
        protected int   bufPos = 1;     // buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
                                        // close an empty element or in CDATA section detection of double ]; _BUFFER[0] will always be 0
        protected int   textPos = 1;    // text end position; don't indent first element, pi, or comment
        protected int   contentPos;     // element content end position
        protected int   cdataPos;       // cdata end position
        protected int   attrEndPos;     // end of the last attribute
        protected int   bufLen = BUFSIZE;

        // flags
        protected bool  writeToNull;
        protected bool  hadDoubleBracket;
        protected bool  inAttributeValue;

#ifdef _XML_ENCODED_TEXT_WRITER
        protected int    bufBytesUsed;
        protected char[] bufChars;

        // encoder for encoding chars in specified encoding when writing to stream
        protected Encoder       encoder;

        // output text writer
        protected TextWriter    writer;

        // escaping of characters invalid in the output encoding
        protected bool  trackTextContent;
        protected bool  inTextContent;
        private int     lastMarkPos;
        private int[]   textContentMarks;   // even indices contain text content start positions
                                            // odd indices contain markup start positions 
        private CharEntityEncoderFallback charEntityFallback;

#endif

        // writer settings
        protected NewLineHandling   newLineHandling;
        protected bool              closeOutput;
        protected bool              omitXmlDeclaration;
        protected string            newLineChars;
        protected bool              checkCharacters;
        protected XmlStandalone     standalone;
        protected XmlOutputMethod   outputMethod;
        protected bool              autoXmlDeclaration;
        protected bool              mergeCDataSections;

//
// Constants
//
        private const int BUFSIZE = 2048 * 3;       // Should be greater than default FileStream size (4096), otherwise the FileStream will try to cache the data
        private const int OVERFLOW = 32;            // Allow overflow in order to reduce checks when writing out constant size markup
        private const int INIT_MARKS_COUNT = 64;

//
// Constructors
//
        // Construct and initialize an instance of this class.
        protected _CLASS_NAME( XmlWriterSettings settings ) {
            // copy settings
            newLineHandling = settings.NewLineHandling;
            omitXmlDeclaration = settings.OmitXmlDeclaration;
            newLineChars = settings.NewLineChars;
            checkCharacters = settings.CheckCharacters;
            closeOutput = settings.CloseOutput;

            standalone = settings.Standalone;
            outputMethod = settings.OutputMethod;
            mergeCDataSections = settings.MergeCDataSections;

            if ( checkCharacters && newLineHandling == NewLineHandling.Replace ) {
                ValidateContentChars( newLineChars, "NewLineChars", false );
            }
        }

#ifdef _XML_ENCODED_TEXT_WRITER
        // Construct an instance of this class that outputs text to the TextWriter interface.
        public _CLASS_NAME( TextWriter writer, XmlWriterSettings settings ) : this( settings ) {
            Debug.Assert( writer != null && settings != null );

            this.writer = writer;
            this.encoding = writer.Encoding;
            // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
            this.bufChars = new _BUFFER_TYPE[BUFSIZE + OVERFLOW];

            // Write the xml declaration
            if (settings.AutoXmlDeclaration ) {
                WriteXmlDeclaration( standalone );
                autoXmlDeclaration = true;
            }
        }
#endif

        // Construct an instance of this class that serializes to a Stream interface.
        public _CLASS_NAME( Stream stream, XmlWriterSettings settings ) : this( settings ) {
            Debug.Assert( stream != null && settings != null );

            this.stream = stream;
            this.encoding = settings.Encoding;

#ifdef _XML_UTF8_TEXT_WRITER
            // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
            bufBytes = new _BUFFER_TYPE[ BUFSIZE + OVERFLOW ];

            // Output UTF-8 byte order mark if Encoding object wants it
            if ( !stream.CanSeek || stream.Position == 0 ) {
                ReadOnlySpan<byte> bom = encoding.Preamble;
                if ( bom.Length != 0 ) {
                    bom.CopyTo(new Span<byte>(bufBytes).Slice(1));
                    bufPos += bom.Length;
                    textPos += bom.Length;
                }
            }
#else
            // the buffer is allocated will OVERFLOW in order to reduce checks when writing out constant size markup
            bufChars = new _BUFFER_TYPE[ BUFSIZE + OVERFLOW ];
            bufBytes = new byte[ bufChars.Length ];
            bufBytesUsed = 0;

            // Init escaping of characters not fitting into the target encoding
            trackTextContent = true;
            inTextContent = false;
            lastMarkPos = 0;
            textContentMarks = new int[INIT_MARKS_COUNT];
            textContentMarks[0] = 1;

            charEntityFallback = new CharEntityEncoderFallback();
            this.encoding = (Encoding)settings.Encoding.Clone();
            encoding.EncoderFallback = charEntityFallback;

            encoder = encoding.GetEncoder();

            if ( !stream.CanSeek || stream.Position == 0 ) {
                ReadOnlySpan<byte> bom = encoding.Preamble;
                if ( bom.Length != 0 ) {
                    this.stream.Write( bom );
                }
            }
#endif

            // Write the xml declaration
            if ( settings.AutoXmlDeclaration ) {
                WriteXmlDeclaration( standalone );
                autoXmlDeclaration = true;
            }
        }

//
// XmlWriter implementation
//
        // Returns settings the writer currently applies.
        public override XmlWriterSettings Settings {
            get {
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.Encoding = this.encoding;
                settings.OmitXmlDeclaration = this.omitXmlDeclaration;
                settings.NewLineHandling = this.newLineHandling;
                settings.NewLineChars = this.newLineChars;
                settings.CloseOutput = this.closeOutput;
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
        internal override void WriteXmlDeclaration( XmlStandalone standalone ) {
            // Output xml declaration only if user allows it and it was not already output
            if ( !omitXmlDeclaration && !autoXmlDeclaration ) {

                _SET_TEXT_CONTENT_MARK(false)

                RawText( "<?xml version=\"" );

                // Version
                RawText( "1.0" );

                // Encoding
                if ( encoding != null ) {
                    RawText( "\" encoding=\"" );
                    RawText( encoding.WebName );
                }

                // Standalone
                if ( standalone != XmlStandalone.Omit ) {
                    RawText( "\" standalone=\"" );
                    RawText( standalone == XmlStandalone.Yes ? "yes" : "no" );
                }

                RawText( "\"?>" );
            }
        }

        internal override void WriteXmlDeclaration( string xmldecl ) {
            // Output xml declaration only if user allows it and it was not already output
            if ( !omitXmlDeclaration && !autoXmlDeclaration ) {
                WriteProcessingInstruction( "xml", xmldecl );
            }
        }

        // Serialize the document type declaration.
        public override void WriteDocType( string name, string pubid, string sysid, string subset ) {
            Debug.Assert( name != null && name.Length > 0 );

            _SET_TEXT_CONTENT_MARK(false)

            RawText( "<!DOCTYPE ");
            RawText(name);
            if ( pubid != null ) {
                RawText( " PUBLIC \"" );
                RawText( pubid );
                RawText( "\" \"");
                if ( sysid != null ) {
                    RawText( sysid );
                }
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '"';
            }
            else if ( sysid != null ) {
                RawText( " SYSTEM \"" );
                RawText( sysid );
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '"';
            }
            else {
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ' ';
            }

            if ( subset != null ) {
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '[';
                RawText( subset );
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ']';
            }

            _BUFFER[this.bufPos++] = (_BUFFER_TYPE) '>';
        }

        // Serialize the beginning of an element start tag: "<prefix:localName"
        public override void WriteStartElement( string prefix, string localName, string ns) {
            Debug.Assert( localName != null && localName.Length > 0 );
            Debug.Assert( prefix != null );

            _SET_TEXT_CONTENT_MARK(false)

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '<';
            if ( prefix != null && prefix.Length != 0 ) {
                RawText( prefix );
                _BUFFER[this.bufPos++] = (_BUFFER_TYPE) ':';
            }

            RawText( localName );

            attrEndPos = bufPos;
        }

        // Serialize the end of an element start tag in preparation for content serialization: ">"
        internal override void StartElementContent() {
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';

            // StartElementContent is always called; therefore, in order to allow shortcut syntax, we save the
            // position of the '>' character.  If WriteEndElement is called and no other characters have been
            // output, then the '>' character can be overwritten with the shortcut syntax " />".
            contentPos = bufPos;
        }

        // Serialize an element end tag: "</prefix:localName>", if content was output.  Otherwise, serialize
        // the shortcut syntax: " />".
        internal override void WriteEndElement( string prefix, string localName, string ns ) {
            Debug.Assert( localName != null && localName.Length > 0 );
            Debug.Assert( prefix != null );

            _SET_TEXT_CONTENT_MARK(false)

            if ( contentPos != bufPos ) {
                // Content has been output, so can't use shortcut syntax
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '<';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '/';

                if ( prefix != null && prefix.Length != 0) {
                    RawText( prefix );
                    _BUFFER[bufPos++] = (_BUFFER_TYPE) ':';
                }
                RawText( localName );
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';
            }
            else {
                // Use shortcut syntax; overwrite the already output '>' character
                bufPos--;
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ' ';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '/';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';
            }
        }

        // Serialize a full element end tag: "</prefix:localName>"
        internal override void WriteFullEndElement( string prefix, string localName, string ns ) {
            Debug.Assert( localName != null && localName.Length > 0 );
            Debug.Assert( prefix != null );

            _SET_TEXT_CONTENT_MARK(false)

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '<';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '/';

            if ( prefix != null && prefix.Length != 0) {
                RawText( prefix );
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ':';
            }
            RawText( localName );
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';
        }

        // Serialize an attribute tag using double quotes around the attribute value: 'prefix:localName="'
        public override void WriteStartAttribute( string prefix, string localName, string ns ) {
            Debug.Assert( localName != null && localName.Length  > 0 );
            Debug.Assert( prefix != null );

            _SET_TEXT_CONTENT_MARK(false)

            if ( attrEndPos == bufPos ) {
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ' ';
            }

            if ( prefix != null && prefix.Length > 0 ) {
                RawText( prefix );
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ':';
            }
            RawText( localName );
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '=';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '"';

            inAttributeValue = true;
        }

        // Serialize the end of an attribute value using double quotes: '"'
        public override void WriteEndAttribute() {
            _SET_TEXT_CONTENT_MARK(false)
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '"';
            inAttributeValue = false;
            attrEndPos = bufPos;
        }

        internal override void WriteNamespaceDeclaration( string prefix, string namespaceName ) {
            Debug.Assert( prefix != null && namespaceName != null );

            this.WriteStartNamespaceDeclaration( prefix );
            this.WriteString( namespaceName );
            this.WriteEndNamespaceDeclaration();
        }

        internal override bool SupportsNamespaceDeclarationInChunks {
            get {
                return true;
            }
        }

        internal override void WriteStartNamespaceDeclaration(string prefix) {
            Debug.Assert( prefix != null );

            _SET_TEXT_CONTENT_MARK(false)

            // VSTFDEVDIV bug #583965: Inconsistency between Silverlight 2 and Dev10 in the way a single xmlns attribute is serialized	
            // Resolved as: Won't fix (breaking change)
            if ( prefix.Length == 0 ) {
                RawText( " xmlns=\"" );
            }
            else {
                RawText( " xmlns:" );
                RawText( prefix );
                _BUFFER[bufPos++] = (_BUFFER_TYPE)'=';
                _BUFFER[bufPos++] = (_BUFFER_TYPE)'"';
            }

            inAttributeValue = true;
            _SET_TEXT_CONTENT_MARK(true)
        }

        internal override void WriteEndNamespaceDeclaration() {
            _SET_TEXT_CONTENT_MARK(false)
            inAttributeValue = false;

            _BUFFER[bufPos++] = (_BUFFER_TYPE)'"';
            attrEndPos = bufPos;
        }

        // Serialize a CData section.  If the "]]>" pattern is found within
        // the text, replace it with "]]><![CDATA[>".
        public override void WriteCData( string text ) {
            Debug.Assert( text != null );

            _SET_TEXT_CONTENT_MARK(false)

            if ( mergeCDataSections && bufPos == cdataPos ) {
                // Merge adjacent cdata sections - overwrite the "]]>" characters
                Debug.Assert( bufPos >= 4 );
                bufPos -= 3;
            }
            else {
                // Start a new cdata section
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '<';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '!';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '[';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) 'C';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) 'D';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) 'A';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) 'T';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) 'A';
                _BUFFER[bufPos++] = (_BUFFER_TYPE) '[';
            }

            WriteCDataSection( text );

            _BUFFER[bufPos++] = (_BUFFER_TYPE) ']';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) ']';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';

            textPos = bufPos;
            cdataPos = bufPos;
        }

        // Serialize a comment.
        public override void WriteComment( string text ) {
            Debug.Assert( text != null );

            _SET_TEXT_CONTENT_MARK(false)

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '<';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '!';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '-';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '-';

            WriteCommentOrPi( text, '-' );

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '-';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '-';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';
        }

        // Serialize a processing instruction.
        public override void WriteProcessingInstruction( string name, string text ) {
            Debug.Assert( name != null && name.Length > 0 );
            Debug.Assert( text != null );

            _SET_TEXT_CONTENT_MARK(false)

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '<';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '?';
            RawText( name );

            if ( text.Length > 0 ) {
                _BUFFER[bufPos++] = (_BUFFER_TYPE) ' ';
                WriteCommentOrPi( text, '?' );
            }

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '?';
            _BUFFER[bufPos++] = (_BUFFER_TYPE) '>';
        }

        // Serialize an entity reference.
        public override void WriteEntityRef( string name ) {
            Debug.Assert( name != null && name.Length > 0 );

            _SET_TEXT_CONTENT_MARK(false)

            _BUFFER[bufPos++] = (_BUFFER_TYPE) '&';
            RawText( name );
            _BUFFER[bufPos++] = (_BUFFER_TYPE) ';';

            if ( bufPos > bufLen ) {
                FlushBuffer();
            }

            textPos = bufPos;
        }

        // Serialize a character entity reference.
        public override void WriteCharEntity( char ch ) {
            string strVal = ((int)ch).ToString( "X", NumberFormatInfo.InvariantInfo );

            if ( checkCharacters && !xmlCharType.IsCharData( ch ) ) {
                // we just have a single char, not a surrogate, therefore we have to pass in '\0' for the second char
                throw XmlConvert.CreateInvalidCharException( ch, '\0' );
            }

            _SET_TEXT_CONTENT_MARK(false)

            _BUFFER[bufPos++] = (_BUFFER_TYPE)'&';
            _BUFFER[bufPos++] = (_BUFFER_TYPE)'#';
            _BUFFER[bufPos++] = (_BUFFER_TYPE)'x';
            RawText( strVal );
            _BUFFER[bufPos++] = (_BUFFER_TYPE)';';

            if ( bufPos > bufLen ) {
                FlushBuffer();
            }

            textPos = bufPos;
        }

        // Serialize a whitespace node.
        public override unsafe void WriteWhitespace( string ws ) {
            Debug.Assert( ws != null );
            _SET_TEXT_CONTENT_MARK(false)

            fixed ( char * pSrc = ws ) {
                char * pSrcEnd = pSrc + ws.Length;
                if ( inAttributeValue) {
                    WriteAttributeTextBlock( pSrc, pSrcEnd );
                }
                else {
                    WriteElementTextBlock( pSrc, pSrcEnd );
                }
            }
        }

        // Serialize either attribute or element text using XML rules.
        public override unsafe void WriteString( string text ) {
            Debug.Assert( text != null );
            _SET_TEXT_CONTENT_MARK(true)

            fixed ( char * pSrc = text ) {
                char * pSrcEnd = pSrc + text.Length;
                if ( inAttributeValue) {
                    WriteAttributeTextBlock( pSrc, pSrcEnd );
                }
                else {
                    WriteElementTextBlock( pSrc, pSrcEnd );
                }
            }
        }

        // Serialize surrogate character entity.
        public override void WriteSurrogateCharEntity( char lowChar, char highChar ) {
            _SET_TEXT_CONTENT_MARK(false)
            int surrogateChar = XmlCharType.CombineSurrogateChar( lowChar, highChar );

            _BUFFER[bufPos++] = (_BUFFER_TYPE)'&';
            _BUFFER[bufPos++] = (_BUFFER_TYPE)'#';
            _BUFFER[bufPos++] = (_BUFFER_TYPE)'x';
            RawText( surrogateChar.ToString( "X", NumberFormatInfo.InvariantInfo ) );
            _BUFFER[bufPos++] = (_BUFFER_TYPE)';';
            textPos = bufPos;
        }

        // Serialize either attribute or element text using XML rules.
        // Arguments are validated in the XmlWellformedWriter layer.
        public override unsafe void WriteChars( char[] buffer, int index, int count ) {
            Debug.Assert( buffer != null );
            Debug.Assert( index >= 0 );
            Debug.Assert( count >= 0 && index + count <= buffer.Length );

            _SET_TEXT_CONTENT_MARK(true)

            fixed ( char * pSrcBegin = &buffer[index] ) {
                if ( inAttributeValue ) {
                    WriteAttributeTextBlock( pSrcBegin, pSrcBegin + count );
                }
                else {
                    WriteElementTextBlock( pSrcBegin, pSrcBegin + count );
                }
            }
        }

        // Serialize raw data.
        // Arguments are validated in the XmlWellformedWriter layer
        public override unsafe void WriteRaw( char[] buffer, int index, int count ) {
            Debug.Assert( buffer != null );
            Debug.Assert( index >= 0 );
            Debug.Assert( count >= 0 && index + count <= buffer.Length );

            _SET_TEXT_CONTENT_MARK(false)

            fixed ( char * pSrcBegin = &buffer[index] ) {
                WriteRawWithCharChecking( pSrcBegin, pSrcBegin + count );
            }
            textPos = bufPos;
        }

        // Serialize raw data.
        public override unsafe void WriteRaw( string data ) {
            Debug.Assert( data != null );

            _SET_TEXT_CONTENT_MARK(false)

            fixed ( char * pSrcBegin = data ) {
                WriteRawWithCharChecking( pSrcBegin, pSrcBegin + data.Length );
            }
            textPos = bufPos;
        }

        // Flush all bytes in the buffer to output and close the output stream or writer.
        public override void Close() {
            try {
                FlushBuffer();
                FlushEncoder();
            }
            finally {
                // Future calls to Close or Flush shouldn't write to Stream or Writer
                writeToNull = true;

                if ( stream != null ) {
                    try {
                        stream.Flush();
                    }
                    finally {
                        try {
                            if ( closeOutput ) {
                                stream.Close();
                            }
                        }
                        finally {
                            stream = null;
                        }
                    }
                }
#ifndef _XML_UTF8_TEXT_WRITER
                else if ( writer != null ) {
                    try {
                        writer.Flush();
                    }
                    finally {
                        try {
                            if ( closeOutput ) {
                                writer.Close();
                            }
                        }
                        finally {
                            writer = null;
                        }
                    }
                }
#endif
            }
        }

        // Flush all characters in the buffer to output and call Flush() on the output object.
        public override void Flush() {
            FlushBuffer();
            FlushEncoder();
#ifdef _XML_UTF8_TEXT_WRITER
            if ( stream != null ) {
                stream.Flush();
            }
#else
            if ( stream != null ) {
                stream.Flush(); 
            }
            else if ( writer != null ) {
                writer.Flush();
            }
#endif
        }

//
// Implementation methods
//
        // Flush all characters in the buffer to output.  Do not flush the output object.
        protected virtual void FlushBuffer() {
            try {
                // Output all characters (except for previous characters stored at beginning of buffer)
                if ( !writeToNull ) {
#ifdef _XML_UTF8_TEXT_WRITER
                    Debug.Assert( stream != null);
                    stream.Write( bufBytes, 1, bufPos - 1 );
#else
                    Debug.Assert( stream != null || writer != null );

                    if ( stream != null ) {
                        if ( trackTextContent ) {
                            charEntityFallback.Reset( textContentMarks, lastMarkPos );
                            // reset text content tracking

                            if ((lastMarkPos & 1) != 0) {
                                // If the previous buffer ended inside a text content we need to preserve that info
                                //   which means the next index to which we write has to be even
                                textContentMarks[1] = 1;
                                lastMarkPos = 1;
                            }
                            else {
                                lastMarkPos = 0;
                            }
                            Debug.Assert( textContentMarks[0] == 1 );
                        }
                        EncodeChars( 1, bufPos, true );
                    }
                    else {
                        // Write text to TextWriter
                        writer.Write( bufChars, 1, bufPos - 1 );
                    }
#endif
                }
            }
            catch {
                // Future calls to flush (i.e. when Close() is called) don't attempt to write to stream
                writeToNull = true;
                throw;
            }
            finally {
                // Move last buffer character to the beginning of the buffer (so that previous character can always be determined)
                _BUFFER[0] = _BUFFER[bufPos - 1];

#ifdef _XML_UTF8_TEXT_WRITER
                if ( IsSurrogateByte( _BUFFER[0] ) ) {
                    // Last character was the first byte in a surrogate encoding, so move last three
                    // bytes of encoding to the beginning of the buffer.
                    _BUFFER[1] = _BUFFER[bufPos];
                    _BUFFER[2] = _BUFFER[bufPos + 1];
                    _BUFFER[3] = _BUFFER[bufPos + 2];
                }
#endif

                // Reset buffer position
                textPos = (textPos == bufPos) ? 1 : 0;
                attrEndPos = (attrEndPos == bufPos) ? 1 : 0;
                contentPos = 0;    // Needs to be zero, since overwriting '>' character is no longer possible
                cdataPos = 0;      // Needs to be zero, since overwriting ']]>' characters is no longer possible
                bufPos = 1;        // Buffer position starts at 1, because we need to be able to safely step back -1 in case we need to
                                   // close an empty element or in CDATA section detection of double ]; _BUFFER[0] will always be 0
            }
        }

#if _XML_UTF8_TEXT_WRITER
        private void FlushEncoder() {
            // intentionally empty
        }
#else 
        private void EncodeChars( int startOffset, int endOffset, bool writeAllToStream ) {
            // Write encoded text to stream
            int chEnc;
            int bEnc;
            bool completed;
            while ( startOffset < endOffset ) {
                if ( charEntityFallback != null ) {
                    charEntityFallback.StartOffset = startOffset;
                }
                encoder.Convert( bufChars, startOffset, endOffset - startOffset, bufBytes, bufBytesUsed, bufBytes.Length - bufBytesUsed, false, out chEnc, out bEnc, out completed );
                startOffset += chEnc;
                bufBytesUsed += bEnc;
                if ( bufBytesUsed >= ( bufBytes.Length - 16 ) ) {
                    stream.Write( bufBytes, 0, bufBytesUsed );
                    bufBytesUsed = 0;
                }
            }
            if ( writeAllToStream && bufBytesUsed > 0 ) {
                stream.Write( bufBytes, 0, bufBytesUsed );
                bufBytesUsed = 0;
            }
        }

        private void FlushEncoder() {
            Debug.Assert( bufPos == 1 );
            if ( stream != null ) {
                int chEnc;
                int bEnc;
                bool completed;
                // decode no chars, just flush
                encoder.Convert( bufChars, 1, 0, bufBytes, 0, bufBytes.Length, true, out chEnc, out bEnc, out completed );
                if ( bEnc != 0 ) {
                    stream.Write( bufBytes, 0, bEnc );
                }
            }
        }
#endif

        // Serialize text that is part of an attribute value.  The '&', '<', '>', and '"' characters
        // are entitized.
        protected unsafe void WriteAttributeTextBlock( char *pSrc, char *pSrcEnd ) {
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                _BUFFER_TYPE * pDst = pDstBegin + this.bufPos;

                int ch = 0;
                for (;;) {
                    _BUFFER_TYPE * pDstEnd = pDst + ( pSrcEnd - pSrc );
                    if ( pDstEnd > pDstBegin + bufLen ) {
                        pDstEnd = pDstBegin + bufLen;
                    }

#if _XML_UTF8_TEXT_WRITER
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISATTRIBUTEVALUECHAR( ( ch = *pSrc ) ) && ch <= 0x7F ) ) {
#else
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISATTRIBUTEVALUECHAR( ( ch = *pSrc ) ) ) ) {
#endif
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
                        pSrc++;
                    }
                    Debug.Assert( pSrc <= pSrcEnd );

                    // end of value
                    if ( pSrc >= pSrcEnd ) {
                        break;
                    }

                    // end of buffer
                    if ( pDst >= pDstEnd ) {
                        bufPos = (int)(pDst - pDstBegin);
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    // some character needs to be escaped
                    switch ( ch ) {
                        case '&':
                            pDst = AmpEntity( pDst );
                            break;
                        case '<':
                            pDst = LtEntity( pDst );
                            break;
                        case '>':
                            pDst = GtEntity( pDst );
                            break;
                        case '"':
                            pDst = QuoteEntity( pDst );
                            break;
                        case '\'':
                            *pDst = (_BUFFER_TYPE)ch;
                            pDst++;
                            break;
                        case (char)0x9:
                            if ( newLineHandling == NewLineHandling.None ) {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            else {
                                // escape tab in attributes
                                pDst = TabEntity( pDst );
                            }
                            break;
                        case (char)0xD:
                            if ( newLineHandling == NewLineHandling.None ) {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            else {
                                // escape new lines in attributes
                                pDst = CarriageReturnEntity( pDst );
                            }
                            break;
                        case (char)0xA:
                            if ( newLineHandling == NewLineHandling.None ) {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            else {
                                // escape new lines in attributes
                                pDst = LineFeedEntity( pDst );
                            }
                            break;
                        default:
                           _ENCODE_CHAR(true);
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
            }
        }

        // Serialize text that is part of element content.  The '&', '<', and '>' characters
        // are entitized.
        protected unsafe void WriteElementTextBlock( char *pSrc, char *pSrcEnd ) {
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                _BUFFER_TYPE * pDst = pDstBegin + this.bufPos;

                int ch = 0;
                for (;;) {
                    _BUFFER_TYPE * pDstEnd = pDst + ( pSrcEnd - pSrc );
                    if ( pDstEnd > pDstBegin + bufLen ) {
                        pDstEnd = pDstBegin + bufLen;
                    }

#if _XML_UTF8_TEXT_WRITER
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISATTRIBUTEVALUECHAR( ( ch = *pSrc ) ) && ch <= 0x7F ) ) {
#else
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISATTRIBUTEVALUECHAR( ( ch = *pSrc ) ) ) ) {
#endif
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
                        pSrc++;
                    }
                    Debug.Assert( pSrc <= pSrcEnd );

                    // end of value
                    if ( pSrc >= pSrcEnd ) {
                        break;
                    }

                    // end of buffer
                    if ( pDst >= pDstEnd ) {
                        bufPos = (int)(pDst - pDstBegin);
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    // some character needs to be escaped
                    switch ( ch ) {
                        case '&':
                            pDst = AmpEntity( pDst );
                            break;
                        case '<':
                            pDst = LtEntity( pDst );
                            break;
                        case '>':
                            pDst = GtEntity( pDst );
                            break;
                        case '"':
                        case '\'':
                        case (char)0x9:
                            *pDst = (_BUFFER_TYPE)ch;
                            pDst++;
                            break;
                        case (char)0xA:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        case (char)0xD:
                            switch ( newLineHandling ) {
                                case NewLineHandling.Replace:
                                    // Replace "\r\n", or "\r" with NewLineChars
                                    if ( pSrc[1] == '\n' ) {
                                        pSrc++;
                                    }
                                    pDst = WriteNewLine( pDst );
                                    break;
                                case NewLineHandling.Entitize:
                                    // Entitize 0xD
                                    pDst = CarriageReturnEntity( pDst );
                                    break;
                                case NewLineHandling.None:
                                    *pDst = (_BUFFER_TYPE)ch;
                                    pDst++;
                                    break;
                            }
                            break;
                        default:
                            _ENCODE_CHAR(true);
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
                textPos = bufPos;
                contentPos = 0;
            }
        }

        protected unsafe void RawText( string s ) {
            Debug.Assert( s != null );
            fixed ( char * pSrcBegin = s ) {
                RawText( pSrcBegin, pSrcBegin + s.Length );
            }
        }

        protected unsafe void RawText( char * pSrcBegin, char * pSrcEnd ) {
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                _BUFFER_TYPE * pDst = pDstBegin + this.bufPos;
                char * pSrc = pSrcBegin;

                int ch = 0;
                for (;;) {
                    _BUFFER_TYPE * pDstEnd = pDst + ( pSrcEnd - pSrc );
                    if ( pDstEnd > pDstBegin + this.bufLen ) {
                        pDstEnd = pDstBegin + this.bufLen;
                    }

#ifdef _XML_UTF8_TEXT_WRITER
                    while ( pDst < pDstEnd && ( ( ch = *pSrc ) <= 0x7F ) ) {
#else
                    while ( pDst < pDstEnd && ( ( ch = *pSrc ) < XmlCharType.SurHighStart ) ) {
#endif
                        pSrc++;
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
                    }
                    Debug.Assert( pSrc <= pSrcEnd );

                    // end of value
                    if ( pSrc >= pSrcEnd ) {
                        break;
                    }

                    // end of buffer
                    if ( pDst >= pDstEnd ) {
                        bufPos = (int)(pDst - pDstBegin);
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    _ENCODE_CHAR(false);
                }

                bufPos = (int)(pDst - pDstBegin);
            }
        }

        protected unsafe void WriteRawWithCharChecking( char * pSrcBegin, char * pSrcEnd ) {
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                char * pSrc = pSrcBegin;
                _BUFFER_TYPE * pDst = pDstBegin + bufPos;

                int ch = 0;
                for (;;) {
                    _BUFFER_TYPE * pDstEnd = pDst + ( pSrcEnd - pSrc );
                    if ( pDstEnd > pDstBegin + bufLen ) {
                        pDstEnd = pDstBegin + bufLen;
                    }

#ifdef _XML_UTF8_TEXT_WRITER
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISTEXT( ( ch = *pSrc ) ) && ch <= 0x7F ) ) {
#else
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISTEXT( ( ch = *pSrc ) ) ) ) {
#endif
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
                        pSrc++;
                    }

                    Debug.Assert( pSrc <= pSrcEnd );

                    // end of value
                    if ( pSrc >= pSrcEnd ) {
                        break;
                    }

                    // end of buffer
                    if ( pDst >= pDstEnd ) {
                        bufPos = (int)(pDst - pDstBegin);
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    // handle special characters
                    switch ( ch ) {
                        case ']':
                        case '<':
                        case '&':
                        case (char)0x9:
                            *pDst = (_BUFFER_TYPE)ch;
                            pDst++;
                            break;
                        case (char)0xD:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                // Normalize "\r\n", or "\r" to NewLineChars
                                if ( pSrc[1] == '\n' ) {
                                    pSrc++;
                                }
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        case (char)0xA:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        default:
                            _ENCODE_CHAR(false);
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
            }
        }

        protected unsafe void WriteCommentOrPi( string text, int stopChar ) {
            if ( text.Length == 0 ) {
                if ( bufPos >= bufLen ) {
                    FlushBuffer();
                }
                return;
            }
            // write text
            fixed ( char * pSrcBegin = text )
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                char * pSrc = pSrcBegin;
                char * pSrcEnd = pSrcBegin + text.Length;
                _BUFFER_TYPE * pDst = pDstBegin + bufPos;

                int ch = 0;
                for (;;) {
                    _BUFFER_TYPE * pDstEnd = pDst + ( pSrcEnd - pSrc );
                    if ( pDstEnd > pDstBegin + bufLen ) {
                        pDstEnd = pDstBegin + bufLen;
                    }

#ifdef _XML_UTF8_TEXT_WRITER
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISTEXT( ( ch = *pSrc ) ) && ch != stopChar && ch <= 0x7F ) ) {
#else
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISTEXT( ( ch = *pSrc ) ) && ch != stopChar ) ) {
#endif
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
                        pSrc++;
                    }

                    Debug.Assert( pSrc <= pSrcEnd );

                    // end of value
                    if ( pSrc >= pSrcEnd ) {
                        break;
                    }

                    // end of buffer
                    if ( pDst >= pDstEnd ) {
                        bufPos = (int)(pDst - pDstBegin);
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    // handle special characters
                    switch ( ch ) {
                        case '-':
                            *pDst = (_BUFFER_TYPE) '-';
                            pDst++;
                            if ( ch == stopChar ) {
                                // Insert space between adjacent dashes or before comment's end dashes
                                if ( pSrc + 1 == pSrcEnd || *(pSrc + 1)== '-' ) {
                                    *pDst = (_BUFFER_TYPE) ' ';
                                    pDst++;
                                }
                            }
                            break;
                        case '?':
                            *pDst = (_BUFFER_TYPE) '?';
                            pDst++;
                            if ( ch == stopChar ) {
                                // Processing instruction: insert space between adjacent '?' and '>' 
                                if ( pSrc + 1 < pSrcEnd && *(pSrc + 1)== '>' ) {
                                    *pDst = (_BUFFER_TYPE) ' ';
                                    pDst++;
                                }
                            }
                            break;
                        case ']':
                            *pDst = (_BUFFER_TYPE) ']';
                            pDst++;
                            break;
                        case (char)0xD:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                // Normalize "\r\n", or "\r" to NewLineChars
                                if ( pSrc[1] == '\n' ) {
                                    pSrc++;
                                }
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        case (char)0xA:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        case '<':
                        case '&':
                        case (char)0x9:
                            *pDst = (_BUFFER_TYPE)ch;
                            pDst++;
                            break;
                        default:
                            _ENCODE_CHAR(false);
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
            }
        }

        protected unsafe void WriteCDataSection( string text ) {
            if ( text.Length == 0 ) {
                if ( bufPos >= bufLen ) {
                    FlushBuffer();
                }
                return;
            }
            // write text
            fixed ( char * pSrcBegin = text )
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                char * pSrc = pSrcBegin;
                char * pSrcEnd = pSrcBegin + text.Length;
                _BUFFER_TYPE * pDst = pDstBegin + bufPos;

                int ch = 0;
                for (;;) {
                    _BUFFER_TYPE * pDstEnd = pDst + ( pSrcEnd - pSrc );
                    if ( pDstEnd > pDstBegin + bufLen ) {
                        pDstEnd = pDstBegin + bufLen;
                    }

#ifdef _XML_UTF8_TEXT_WRITER
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISATTRIBUTEVALUECHAR( ( ch = *pSrc ) ) && ch != ']' && ch <= 0x7F ) ) {
#else
                    while ( pDst < pDstEnd && ( XMLCHARTYPE_ISATTRIBUTEVALUECHAR( ( ch = *pSrc ) ) && ch != ']' ) ) {
#endif
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
                        pSrc++;
                    }

                    Debug.Assert( pSrc <= pSrcEnd );

                    // end of value
                    if ( pSrc >= pSrcEnd ) {
                        break;
                    }

                    // end of buffer
                    if ( pDst >= pDstEnd ) {
                        bufPos = (int)(pDst - pDstBegin);
                        FlushBuffer();
                        pDst = pDstBegin + 1;
                        continue;
                    }

                    // handle special characters
                    switch ( ch ) {
                        case '>':
                            if ( hadDoubleBracket && pDst[-1] == (_BUFFER_TYPE) ']') {   // pDst[-1] will always correct - there is a padding character at _BUFFER[0]
                                // The characters "]]>" were found within the CData text
                                pDst = RawEndCData( pDst );
                                pDst = RawStartCData( pDst );
                            }
                            *pDst = (_BUFFER_TYPE) '>';
                            pDst++;
                            break;
                        case ']':
                            if ( pDst[-1] == (_BUFFER_TYPE)']' ) {   // pDst[-1] will always correct - there is a padding character at _BUFFER[0]
                                hadDoubleBracket = true;
                            }
                            else {
                                hadDoubleBracket = false;
                            }
                            *pDst = (_BUFFER_TYPE)']';
                            pDst++;
                            break;
                        case (char)0xD:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                // Normalize "\r\n", or "\r" to NewLineChars
                                if ( pSrc[1] == '\n' ) {
                                    pSrc++;
                                }
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        case (char)0xA:
                            if ( newLineHandling == NewLineHandling.Replace ) {
                                pDst = WriteNewLine( pDst );
                            }
                            else {
                                *pDst = (_BUFFER_TYPE)ch;
                                pDst++;
                            }
                            break;
                        case '&':
                        case '<':
                        case '"':
                        case '\'':
                        case (char)0x9:
                            *pDst = (_BUFFER_TYPE)ch;
                            pDst++;
                            break;
                        default:
                            _ENCODE_CHAR(false);
                            continue;
                    }
                    pSrc++;
                }
                bufPos = (int)(pDst - pDstBegin);
            }
        }

#ifdef _XML_UTF8_TEXT_WRITER
        // Returns true if UTF8 encoded byte is first of four bytes that encode a surrogate pair.
        // To do this, detect the bit pattern 11110xxx.
        private static bool IsSurrogateByte( byte b ) {
            return (b & 0xF8) == 0xF0;
        }
#endif

        private static unsafe _BUFFER_TYPE* EncodeSurrogate( char* pSrc, char* pSrcEnd, _BUFFER_TYPE* pDst ) {
            Debug.Assert( XmlCharType.IsSurrogate( *pSrc ) );

            int ch = *pSrc;
            if ( ch <= XmlCharType.SurHighEnd ) {
                if ( pSrc + 1 < pSrcEnd ) {
                    int lowChar = pSrc[1];
                    if ( lowChar >= XmlCharType.SurLowStart ) {
#ifdef _XML_UTF8_TEXT_WRITER
                        // Calculate Unicode scalar value for easier manipulations (see section 3.7 in Unicode spec)
                        // The scalar value repositions surrogate values to start at 0x10000.
                        
                        ch = XmlCharType.CombineSurrogateChar( lowChar, ch );

                        pDst[0] = (byte)( 0xF0 | ( ch >> 18 ) );
                        pDst[1] = (byte)( 0x80 | ( ch >> 12 ) & 0x3F );
                        pDst[2] = (byte)( 0x80 | ( ch >> 6  ) & 0x3F );
                        pDst[3] = (byte)( 0x80 | ch & 0x3F);
                        pDst += 4;
#else
                        pDst[0] = (char)ch;
                        pDst[1] = (char)lowChar;
                        pDst += 2;
#endif
                        return pDst;
                    }
                    throw XmlConvert.CreateInvalidSurrogatePairException( (char)lowChar, (char)ch );
                }
                throw new ArgumentException( string.Format( SR.Xml_InvalidSurrogateMissingLowChar ) );
            }
            throw XmlConvert.CreateInvalidHighSurrogateCharException( (char)ch );
        }

        private unsafe _BUFFER_TYPE* InvalidXmlChar( int ch, _BUFFER_TYPE* pDst, bool entitize ) {
            Debug.Assert( !xmlCharType.IsWhiteSpace( (char)ch ) );
            Debug.Assert( !xmlCharType.IsAttributeValueChar( (char)ch ) );

            if ( checkCharacters ) {
                // This method will never be called on surrogates, so it is ok to pass in '\0' to the CreateInvalidCharException
                throw XmlConvert.CreateInvalidCharException( (char)ch, '\0' );
            }
            else {
                if ( entitize ) {
                    return CharEntity( pDst, (char)ch );
                }
                else {
#ifdef _XML_UTF8_TEXT_WRITER
                    if ( ch < 0x80 ) {
#endif
                        *pDst = (_BUFFER_TYPE)ch;
                        pDst++;
#ifdef _XML_UTF8_TEXT_WRITER
                    }
                    else {
                        pDst = EncodeMultibyteUTF8( ch, pDst );
                    }
#endif
                    return pDst;
                }
            }
        }

        internal unsafe void EncodeChar(ref char* pSrc, char*pSrcEnd, ref _BUFFER_TYPE* pDst) {
            int ch = *pSrc;
            _ENCODE_CHAR(false);
        }

#ifdef _XML_UTF8_TEXT_WRITER
        internal static unsafe byte* EncodeMultibyteUTF8( int ch, byte* pDst ) {
            Debug.Assert( ch >= 0x80 && !XmlCharType.IsSurrogate( ch ) );

            unchecked {
                /* UTF8-2: If ch is in 0x80-0x7ff range, then use 2 bytes to encode it */ \
                if ( ch < 0x800 ) {
                    *pDst = (byte)( (sbyte)0xC0 | (ch >> 6) );
                }
                /* UTF8-3: If ch is anything else, then default to using 3 bytes to encode it. */
                else {
                    *pDst = (byte)( (sbyte)0xE0 | ( ch >> 12 ) );
                    pDst++;

                    *pDst = (byte)( (sbyte)0x80 | ( ch >> 6 ) & 0x3F);
                }
            }

            pDst++;
            *pDst = (byte)( 0x80 | ch & 0x3F );   
            return pDst + 1;
        }

        // Encode *pSrc as a sequence of UTF8 bytes.  Write the bytes to pDst and return an updated pointer.
        internal static unsafe void CharToUTF8( ref char * pSrc, char * pSrcEnd, ref byte * pDst ) {
            int ch = *pSrc;
            if ( ch <= 0x7F ) {
                *pDst = (byte)ch;
                pDst++;
                pSrc++;
            }
            else if ( XmlCharType.IsSurrogate( ch ) ) { 
                pDst = EncodeSurrogate( pSrc, pSrcEnd, pDst ); 
                pSrc += 2; 
            } 
            else { 
                pDst = EncodeMultibyteUTF8( ch, pDst ); 
                pSrc++; 
            }
        }

#endif

#ifdef _XML_ENCODED_TEXT_WRITER

        protected void ChangeTextContentMark( bool value ) {
            Debug.Assert( inTextContent != value );
            Debug.Assert( inTextContent || ((lastMarkPos & 1) == 0) );
            inTextContent = value;
            if ( lastMarkPos + 1 == textContentMarks.Length ) {
                GrowTextContentMarks();
            }
            textContentMarks[++lastMarkPos] = this.bufPos;
        }

        private void GrowTextContentMarks() {
            Debug.Assert( lastMarkPos + 1 == textContentMarks.Length );
            int[] newTextContentMarks = new int[ textContentMarks.Length * 2 ];
            Array.Copy( textContentMarks, newTextContentMarks, textContentMarks.Length );
            textContentMarks = newTextContentMarks;
        }

#endif
       
        // Write NewLineChars to the specified buffer position and return an updated position.
        protected unsafe _BUFFER_TYPE * WriteNewLine( _BUFFER_TYPE * pDst ) {
            fixed ( _BUFFER_TYPE * pDstBegin = _BUFFER ) {
                bufPos = (int) (pDst - pDstBegin);
                // Let RawText do the real work
                RawText( newLineChars );
                return pDstBegin + bufPos;
            }
        }

        // Following methods do not check whether pDst is beyond the bufSize because the buffer was allocated with a OVERFLOW to accommodate
        // for the writes of small constant-length string as below.

        // Entitize '<' as "&lt;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * LtEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'l'; 
            pDst[2] = (_BUFFER_TYPE)'t'; 
            pDst[3] = (_BUFFER_TYPE)';';
            return pDst + 4;
        }

        // Entitize '>' as "&gt;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * GtEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'g'; 
            pDst[2] = (_BUFFER_TYPE)'t'; 
            pDst[3] = (_BUFFER_TYPE)';';
            return pDst + 4;
        }

        // Entitize '&' as "&amp;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * AmpEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'a'; 
            pDst[2] = (_BUFFER_TYPE)'m'; 
            pDst[3] = (_BUFFER_TYPE)'p'; 
            pDst[4] = (_BUFFER_TYPE)';';
            return pDst + 5;
        }

        // Entitize '"' as "&quot;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * QuoteEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'q'; 
            pDst[2] = (_BUFFER_TYPE)'u'; 
            pDst[3] = (_BUFFER_TYPE)'o'; 
            pDst[4] = (_BUFFER_TYPE)'t'; 
            pDst[5] = (_BUFFER_TYPE)';';
            return pDst + 6;
        }

        // Entitize '\t' as "&#x9;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * TabEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'#'; 
            pDst[2] = (_BUFFER_TYPE)'x'; 
            pDst[3] = (_BUFFER_TYPE)'9'; 
            pDst[4] = (_BUFFER_TYPE)';';
            return pDst + 5;
        }

        // Entitize 0xa as "&#xA;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * LineFeedEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'#'; 
            pDst[2] = (_BUFFER_TYPE)'x'; 
            pDst[3] = (_BUFFER_TYPE)'A'; 
            pDst[4] = (_BUFFER_TYPE)';';
            return pDst + 5;
        }

        // Entitize 0xd as "&#xD;".  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * CarriageReturnEntity( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'#'; 
            pDst[2] = (_BUFFER_TYPE)'x'; 
            pDst[3] = (_BUFFER_TYPE)'D'; 
            pDst[4] = (_BUFFER_TYPE)';';
            return pDst + 5;
        }

        private static unsafe _BUFFER_TYPE * CharEntity( _BUFFER_TYPE * pDst, char ch ) {
            string s = ((int)ch).ToString( "X",NumberFormatInfo.InvariantInfo );
            pDst[0] = (_BUFFER_TYPE)'&'; 
            pDst[1] = (_BUFFER_TYPE)'#'; 
            pDst[2] = (_BUFFER_TYPE)'x'; 
            pDst += 3;
            
            fixed ( char *pSrc = s ) {
                char *pS = pSrc;
                while ( ( *pDst++ = (_BUFFER_TYPE)*pS++ ) != 0 );
            }

            pDst[-1] = (_BUFFER_TYPE)';';
            return pDst;
        }

        // Write "<![CDATA[" to the specified buffer.  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * RawStartCData( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)'<'; 
            pDst[1] = (_BUFFER_TYPE)'!'; 
            pDst[2] = (_BUFFER_TYPE)'['; 
            pDst[3] = (_BUFFER_TYPE)'C'; 
            pDst[4] = (_BUFFER_TYPE)'D';
            pDst[5] = (_BUFFER_TYPE)'A'; 
            pDst[6] = (_BUFFER_TYPE)'T'; 
            pDst[7] = (_BUFFER_TYPE)'A'; 
            pDst[8] = (_BUFFER_TYPE)'[';
            return pDst + 9;
        }

        // Write "]]>" to the specified buffer.  Return an updated pointer.
        protected static unsafe _BUFFER_TYPE * RawEndCData( _BUFFER_TYPE * pDst ) {
            pDst[0] = (_BUFFER_TYPE)']'; 
            pDst[1] = (_BUFFER_TYPE)']'; 
            pDst[2] = (_BUFFER_TYPE)'>';
            return pDst + 3;
        }

        protected unsafe void ValidateContentChars( string chars, string propertyName, bool allowOnlyWhitespace ) {
            if ( allowOnlyWhitespace ) {
                if ( !xmlCharType.IsOnlyWhitespace( chars ) ) {
                    throw new ArgumentException( string.Format( SR.Xml_IndentCharsNotWhitespace, propertyName ) );
                }
            }
            else {
                string error = null;
                for ( int i = 0; i < chars.Length; i++ ) {
                    if ( !xmlCharType.IsTextChar( chars[i] ) ) {
                        switch ( chars[i] ) {
                            case '\n':
                            case '\r':
                            case '\t':
                                continue;
                            case '<':
                            case '&':
                            case ']':
                                error = string.Format( SR.Xml_InvalidCharacter, XmlException.BuildCharExceptionArgs( chars, i ) );
                                goto Error;
                            default:
                                if ( XmlCharType.IsHighSurrogate(chars[i]) ) {
                                    if ( i + 1 < chars.Length ) {
                                        if ( XmlCharType.IsLowSurrogate(chars[i + 1])  ) {
                                            i++;
                                            continue;
                                        }
                                    }
                                    error = string.Format( SR.Xml_InvalidSurrogateMissingLowChar );
                                    goto Error;
                                }
                                else if ( XmlCharType.IsLowSurrogate(chars[i]) ) {
                                    error = string.Format( SR.Xml_InvalidSurrogateHighChar, ((uint)chars[i]).ToString( "X", CultureInfo.InvariantCulture ) );
                                    goto Error;
                                }
                                continue;
                        }
                    }
                }
                return;

            Error:
                throw new ArgumentException( string.Format( SR.Xml_InvalidCharsInIndent, new string[] { propertyName, error } ) );
            }
        }
    }

    // Same as base text writer class except that elements, attributes, comments, and pi's are indented.
    internal class _CLASS_NAME_INDENT : _CLASS_NAME {
//
// Fields
//
        protected int       indentLevel;
        protected bool      newLineOnAttributes;
        protected string    indentChars;

        protected bool      mixedContent;
        private BitStack    mixedContentStack;

        protected ConformanceLevel conformanceLevel = ConformanceLevel.Auto;

//
// Constructors
//

#ifdef _XML_ENCODED_TEXT_WRITER
        public _CLASS_NAME_INDENT( TextWriter writer, XmlWriterSettings settings ) : base( writer, settings ) {
            Init( settings );
        }
#endif

        public _CLASS_NAME_INDENT( Stream stream, XmlWriterSettings settings ) : base( stream, settings ) {
            Init( settings );
        }

//
// XmlWriter methods
//
        public override XmlWriterSettings Settings {
            get {
                XmlWriterSettings settings = base.Settings;

                settings.ReadOnly = false;
                settings.Indent = true;
                settings.IndentChars = indentChars;
                settings.NewLineOnAttributes = newLineOnAttributes;
                settings.ReadOnly = true;

                return settings;
            }
        }

        public override void WriteDocType( string name, string pubid, string sysid, string subset ) {
            // Add indentation
            if ( !mixedContent && base.textPos != base.bufPos) {
                WriteIndent();
            }
            base.WriteDocType( name, pubid, sysid, subset );
        }

        public override void WriteStartElement( string prefix, string localName, string ns ) {
            Debug.Assert( localName != null && localName.Length != 0 && prefix != null && ns != null );

            // Add indentation
            if ( !mixedContent && base.textPos != base.bufPos) {
                WriteIndent();
            }
            indentLevel++;
            mixedContentStack.PushBit( mixedContent );

            base.WriteStartElement( prefix, localName, ns );
        }

        internal override void StartElementContent() {
            // If this is the root element and we're writing a document
            //   do not inherit the mixedContent flag into the root element.
            //   This is to allow for whitespace nodes on root level
            //   without disabling indentation for the whole document.
            if (indentLevel == 1 && conformanceLevel == ConformanceLevel.Document) {
                mixedContent = false;
            }
            else {
                mixedContent = mixedContentStack.PeekBit();
            }
            base.StartElementContent();
        }

        internal override void OnRootElement(ConformanceLevel currentConformanceLevel) { 
            // Just remember the current conformance level
            conformanceLevel = currentConformanceLevel;
        }

        internal override void WriteEndElement(string prefix, string localName, string ns) {
            // Add indentation
            indentLevel--;
            if ( !mixedContent && base.contentPos != base.bufPos ) {
                // There was content, so try to indent
                if ( base.textPos != base.bufPos ) {
                    WriteIndent();
                }
            }
            mixedContent = mixedContentStack.PopBit();

            base.WriteEndElement( prefix, localName, ns );
        }

        internal override void WriteFullEndElement(string prefix, string localName, string ns) {
            // Add indentation
            indentLevel--;
            if ( !mixedContent && base.contentPos != base.bufPos ) {
                // There was content, so try to indent
                if ( base.textPos != base.bufPos ) {
                    WriteIndent();
                }
            }
            mixedContent = mixedContentStack.PopBit();

            base.WriteFullEndElement( prefix, localName, ns );
        }

        // Same as base class, plus possible indentation.
        public override void WriteStartAttribute( string prefix, string localName, string ns ) {
            // Add indentation
            if ( newLineOnAttributes ) {
                WriteIndent();
            }

            base.WriteStartAttribute( prefix, localName, ns );
        }

        public override void WriteCData( string text ) {
            mixedContent = true;
            base.WriteCData( text );
        }
    
        public override void WriteComment( string text ) {
            if ( !mixedContent && base.textPos != base.bufPos ) {
                WriteIndent();
            }

            base.WriteComment( text );
        }

        public override void WriteProcessingInstruction( string target, string text ) {
            if ( !mixedContent && base.textPos != base.bufPos ) {
                WriteIndent();
            }

            base.WriteProcessingInstruction( target, text );
        }

        public override void WriteEntityRef( string name ) {
            mixedContent = true;
            base.WriteEntityRef( name );
        }

        public override void WriteCharEntity( char ch ) {
            mixedContent = true;
            base.WriteCharEntity( ch );
        }

        public override void WriteSurrogateCharEntity( char lowChar, char highChar ) {
            mixedContent = true;
            base.WriteSurrogateCharEntity( lowChar, highChar );
        }

        public override void WriteWhitespace( string ws ) {
            mixedContent = true;
            base.WriteWhitespace( ws );
        }

        public override void WriteString( string text ) {
            mixedContent = true;
            base.WriteString( text );
        }

        public override void WriteChars( char[] buffer, int index, int count ) {
            mixedContent = true;
            base.WriteChars( buffer, index, count );
        }

        public override void WriteRaw( char[] buffer, int index, int count ) {
            mixedContent = true;
            base.WriteRaw( buffer, index, count );
        }

        public override void WriteRaw( string data ) {
            mixedContent = true;
            base.WriteRaw( data );
        }

        public override void WriteBase64( byte[] buffer, int index, int count ) {
            mixedContent = true;
            base.WriteBase64( buffer, index, count );
        }

//
// Private methods
//
        private void Init( XmlWriterSettings settings ) {
            indentLevel = 0;
            indentChars = settings.IndentChars;
            newLineOnAttributes = settings.NewLineOnAttributes;
            mixedContentStack = new BitStack();

            // check indent characters that they are valid XML characters
            if ( base.checkCharacters ) {
                if ( newLineOnAttributes ) {
                    base.ValidateContentChars( indentChars, "IndentChars", true );
                    base.ValidateContentChars( newLineChars, "NewLineChars", true );
                }
                else {
                    base.ValidateContentChars( indentChars, "IndentChars", false );
                    if ( base.newLineHandling != NewLineHandling.Replace ) {
                        base.ValidateContentChars( newLineChars, "NewLineChars", false );
                    }
                }
            }
        }

        // Add indentation to output.  Write newline and then repeat IndentChars for each indent level.
        private void WriteIndent() {
            RawText( base.newLineChars );
            for ( int i = indentLevel; i > 0; i-- ) {
                RawText( indentChars );
            }
        }
    }
}
