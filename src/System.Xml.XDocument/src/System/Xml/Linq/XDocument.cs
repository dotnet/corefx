// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;
using Encoding = System.Text.Encoding;

namespace System.Xml.Linq
{
    /// <summary>
    /// Represents an XML document.
    /// </summary>
    /// <remarks>
    /// An <see cref="XDocument"/> can contain:
    /// <list>
    ///   <item>
    ///   A Document Type Declaration (DTD), see <see cref="XDocumentType"/>
    ///   </item>
    ///   <item>One root element.</item>
    ///   <item>Zero or more <see cref="XComment"/> objects.</item>
    ///   <item>Zero or more <see cref="XProcessingInstruction"/> objects.</item>
    /// </list>
    /// </remarks>
    public class XDocument : XContainer
    {
        private XDeclaration _declaration;

        ///<overloads>
        /// Initializes a new instance of the <see cref="XDocument"/> class.
        /// Overloaded constructors are provided for creating a new empty 
        /// <see cref="XDocument"/>, creating an <see cref="XDocument"/> with
        /// a parameter list of initial content, and as a copy of another
        /// <see cref="XDocument"/> object.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class.
        /// </summary>
        public XDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class with the specified content.
        /// </summary>
        /// <param name="content">
        /// A parameter list of content objects to add to this document.
        /// </param>
        /// <remarks>
        /// Valid content includes:
        /// <list>
        /// <item>Zero or one <see cref="XDocumentType"/> objects</item>
        /// <item>Zero or one elements</item>
        /// <item>Zero or more comments</item>
        /// <item>Zero or more processing instructions</item>
        /// </list>
        /// See <see cref="XContainer.Add(object)"/> for details about the content that can be added
        /// using this method.
        /// </remarks>
        public XDocument(params object[] content)
            : this()
        {
            AddContentSkipNotify(content);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class
        /// with the specified <see cref="XDeclaration"/> and content.
        /// </summary>
        /// <param name="declaration">
        /// The XML declaration for the document.
        /// </param>
        /// <param name="content">
        /// The contents of the document.
        /// </param>
        /// <remarks>
        /// Valid content includes:
        /// <list>
        /// <item>Zero or one <see cref="XDocumentType"/> objects</item>
        /// <item>Zero or one elements</item>
        /// <item>Zero or more comments</item>
        /// <item>Zero or more processing instructions</item>
        /// <item></item>
        /// </list>
        /// See <see cref="XContainer.Add(object)"/> for details about the content that can be added
        /// using this method.
        /// </remarks>
        public XDocument(XDeclaration declaration, params object[] content)
            : this(content)
        {
            _declaration = declaration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class from an
        /// existing XDocument object.
        /// </summary>
        /// <param name="other">
        /// The <see cref="XDocument"/> object that will be copied.
        /// </param>
        public XDocument(XDocument other)
            : base(other)
        {
            if (other._declaration != null)
            {
                _declaration = new XDeclaration(other._declaration);
            }
        }

        /// <summary>
        /// Gets the XML declaration for this document.
        /// </summary>
        public XDeclaration Declaration
        {
            get { return _declaration; }
            set { _declaration = value; }
        }

        /// <summary>
        /// Gets the Document Type Definition (DTD) for this document.
        /// </summary>
        public XDocumentType DocumentType
        {
            get
            {
                return GetFirstNode<XDocumentType>();
            }
        }

        /// <summary>
        /// Gets the node type for this node.
        /// </summary>
        /// <remarks>
        /// This property will always return XmlNodeType.Document.
        /// </remarks>
        public override XmlNodeType NodeType
        {
            get
            {
                return XmlNodeType.Document;
            }
        }

        /// <summary>
        /// Gets the root element of the XML Tree for this document.
        /// </summary>
        public XElement Root
        {
            get
            {
                return GetFirstNode<XElement>();
            }
        }

        /// <overloads>
        /// The Load method provides multiple strategies for creating a new 
        /// <see cref="XDocument"/> and initializing it from a data source containing
        /// raw XML.  Load from a file (passing in a URI to the file), a
        /// <see cref="Stream"/>, a <see cref="TextReader"/>, or an
        /// <see cref="XmlReader"/>.  Note:  Use <see cref="XDocument.Parse(string)"/>
        /// to create an <see cref="XDocument"/> from a string containing XML.
        /// <seealso cref="XDocument.Parse(string)"/>
        /// </overloads>
        /// <summary>
        /// Create a new <see cref="XDocument"/> based on the contents of the file 
        /// referenced by the URI parameter passed in.  Note: Use 
        /// <see cref="XDocument.Parse(string)"/> to create an <see cref="XDocument"/> from
        /// a string containing XML.
        /// <seealso cref="XmlReader.Create(string)"/>
        /// <seealso cref="XDocument.Parse(string)"/>
        /// </summary>
        /// <remarks>
        /// This method uses the <see cref="XmlReader.Create(string)"/> method to create
        /// an <see cref="XmlReader"/> to read the raw XML into the underlying
        /// XML tree.
        /// </remarks>
        /// <param name="uri">
        /// A URI string referencing the file to load into a new <see cref="XDocument"/>.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> initialized with the contents of the file referenced
        /// in the passed in uri parameter.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Back-compat with System.Xml.")]
        public static XDocument Load(string uri)
        {
            return Load(uri, LoadOptions.None);
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> based on the contents of the file 
        /// referenced by the URI parameter passed in.  Optionally, whitespace can be preserved.  
        /// <see cref="XmlReader.Create(string)"/>
        /// </summary>
        /// <remarks>
        /// This method uses the <see cref="XmlReader.Create(string)"/> method to create
        /// an <see cref="XmlReader"/> to read the raw XML into an underlying
        /// XML tree.  If LoadOptions.PreserveWhitespace is enabled then
        /// the <see cref="XmlReaderSettings"/> property <see cref="XmlReaderSettings.IgnoreWhitespace"/>
        /// is set to false.
        /// </remarks>
        /// <param name="uri">
        /// A string representing the URI of the file to be loaded into a new <see cref="XDocument"/>.
        /// </param>
        /// <param name="options">
        /// A set of <see cref="LoadOptions"/>.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> initialized with the contents of the file referenced
        /// in the passed uri parameter.  If LoadOptions.PreserveWhitespace is enabled then
        /// all whitespace will be preserved.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Back-compat with System.Xml.")]
        public static XDocument Load(string uri, LoadOptions options)
        {
            XmlReaderSettings rs = GetXmlReaderSettings(options);
            using (XmlReader r = XmlReader.Create(uri, rs))
            {
                return Load(r, options);
            }
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
        /// the passed <see cref="Stream"/> parameter.  
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> containing the raw XML to read into the newly
        /// created <see cref="XDocument"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XDocument"/> containing the contents of the passed in
        /// <see cref="Stream"/>.
        /// </returns>
        public static XDocument Load(Stream stream)
        {
            return Load(stream, LoadOptions.None);
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
        /// the passed <see cref="Stream"/> parameter.  Optionally whitespace handling
        /// can be preserved.
        /// </summary>
        /// <remarks>
        /// If LoadOptions.PreserveWhitespace is enabled then
        /// the underlying <see cref="XmlReaderSettings"/> property <see cref="XmlReaderSettings.IgnoreWhitespace"/>
        /// is set to false.
        /// </remarks>
        /// <param name="stream">
        /// A <see cref="Stream"/> containing the raw XML to read into the newly
        /// created <see cref="XDocument"/>.
        /// </param>
        /// <param name="options">
        /// A set of <see cref="LoadOptions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XDocument"/> containing the contents of the passed in
        /// <see cref="Stream"/>.
        /// </returns>
        public static XDocument Load(Stream stream, LoadOptions options)
        {
            XmlReaderSettings rs = GetXmlReaderSettings(options);
            using (XmlReader r = XmlReader.Create(stream, rs))
            {
                return Load(r, options);
            }
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
        /// the passed <see cref="TextReader"/> parameter.  
        /// </summary>
        /// <param name="textReader">
        /// A <see cref="TextReader"/> containing the raw XML to read into the newly
        /// created <see cref="XDocument"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XDocument"/> containing the contents of the passed in
        /// <see cref="TextReader"/>.
        /// </returns>
        public static XDocument Load(TextReader textReader)
        {
            return Load(textReader, LoadOptions.None);
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
        /// the passed <see cref="TextReader"/> parameter.  Optionally whitespace handling
        /// can be preserved.
        /// </summary>
        /// <remarks>
        /// If LoadOptions.PreserveWhitespace is enabled then
        /// the <see cref="XmlReaderSettings"/> property <see cref="XmlReaderSettings.IgnoreWhitespace"/>
        /// is set to false.
        /// </remarks>
        /// <param name="textReader">
        /// A <see cref="TextReader"/> containing the raw XML to read into the newly
        /// created <see cref="XDocument"/>.
        /// </param>
        /// <param name="options">
        /// A set of <see cref="LoadOptions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XDocument"/> containing the contents of the passed in
        /// <see cref="TextReader"/>.
        /// </returns>
        public static XDocument Load(TextReader textReader, LoadOptions options)
        {
            XmlReaderSettings rs = GetXmlReaderSettings(options);
            using (XmlReader r = XmlReader.Create(textReader, rs))
            {
                return Load(r, options);
            }
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> containing the contents of the
        /// passed in <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">
        /// An <see cref="XmlReader"/> containing the XML to be read into the new
        /// <see cref="XDocument"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XDocument"/> containing the contents of the passed
        /// in <see cref="XmlReader"/>.
        /// </returns>
        public static XDocument Load(XmlReader reader)
        {
            return Load(reader, LoadOptions.None);
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> containing the contents of the
        /// passed in <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">
        /// An <see cref="XmlReader"/> containing the XML to be read into the new
        /// <see cref="XDocument"/>.
        /// </param>
        /// <param name="options">
        /// A set of <see cref="LoadOptions"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XDocument"/> containing the contents of the passed
        /// in <see cref="XmlReader"/>.
        /// </returns>
        public static XDocument Load(XmlReader reader, LoadOptions options)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (reader.ReadState == ReadState.Initial) reader.Read();
            XDocument d = new XDocument();
            if ((options & LoadOptions.SetBaseUri) != 0)
            {
                string baseUri = reader.BaseURI;
                if (!string.IsNullOrEmpty(baseUri))
                {
                    d.SetBaseUri(baseUri);
                }
            }
            if ((options & LoadOptions.SetLineInfo) != 0)
            {
                IXmlLineInfo li = reader as IXmlLineInfo;
                if (li != null && li.HasLineInfo())
                {
                    d.SetLineInfo(li.LineNumber, li.LinePosition);
                }
            }
            if (reader.NodeType == XmlNodeType.XmlDeclaration)
            {
                d.Declaration = new XDeclaration(reader);
            }
            d.ReadContentFrom(reader, options);
            if (!reader.EOF) throw new InvalidOperationException(SR.InvalidOperation_ExpectedEndOfFile);
            if (d.Root == null) throw new InvalidOperationException(SR.InvalidOperation_MissingRoot);
            return d;
        }

        /// <overloads>
        /// Create a new <see cref="XDocument"/> from a string containing
        /// XML.  Optionally whitespace can be preserved.
        /// </overloads>
        /// <summary>
        /// Create a new <see cref="XDocument"/> from a string containing
        /// XML.
        /// </summary>
        /// <param name="text">
        /// A string containing XML.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> containing an XML tree initialized from the 
        /// passed in XML string.
        /// </returns>
        public static XDocument Parse(string text)
        {
            return Parse(text, LoadOptions.None);
        }

        /// <summary>
        /// Create a new <see cref="XDocument"/> from a string containing
        /// XML.  Optionally whitespace can be preserved.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="XmlReader.Create(TextReader, XmlReaderSettings)"/>,
        /// passing it a StringReader constructed from the passed in XML String. If
        /// <see cref="LoadOptions.PreserveWhitespace"/> is enabled then
        /// <see cref="XmlReaderSettings.IgnoreWhitespace"/> is set to false. See
        /// <see cref="XmlReaderSettings.IgnoreWhitespace"/> for more information on
        /// whitespace handling.
        /// </remarks>
        /// <param name="text">
        /// A string containing XML.
        /// </param>
        /// <param name="options">
        /// A set of <see cref="LoadOptions"/>.
        /// </param>
        /// <returns>
        /// An <see cref="XDocument"/> containing an XML tree initialized from the 
        /// passed in XML string.
        /// </returns>
        public static XDocument Parse(string text, LoadOptions options)
        {
            using (StringReader sr = new StringReader(text))
            {
                XmlReaderSettings rs = GetXmlReaderSettings(options);
                using (XmlReader r = XmlReader.Create(sr, rs))
                {
                    return Load(r, options);
                }
            }
        }

        /// <summary>
        /// Output this <see cref="XDocument"/> to the passed in <see cref="Stream"/>.
        /// </summary>
        /// <remarks>
        /// The format will be indented by default.  If you want
        /// no indenting then use the SaveOptions version of Save (see
        /// <see cref="XDocument.Save(Stream, SaveOptions)"/>) enabling
        /// SaveOptions.DisableFormatting
        /// There is also an option SaveOptions.OmitDuplicateNamespaces for removing duplicate namespace declarations. 
        /// Or instead use the SaveOptions as an annotation on this node or its ancestors, then this method will use those options.
        /// </remarks>
        /// <param name="stream">
        /// The <see cref="Stream"/> to output this <see cref="XDocument"/> to.
        /// </param>
        public void Save(Stream stream)
        {
            Save(stream, GetSaveOptionsFromAnnotations());
        }

        /// <summary>
        /// Output this <see cref="XDocument"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to output the XML to.  
        /// </param>
        /// <param name="options">
        /// If SaveOptions.DisableFormatting is enabled the output is not indented.
        /// If SaveOptions.OmitDuplicateNamespaces is enabled duplicate namespace declarations will be removed.
        /// </param>
        public void Save(Stream stream, SaveOptions options)
        {
            XmlWriterSettings ws = GetXmlWriterSettings(options);
            if (_declaration != null && !string.IsNullOrEmpty(_declaration.Encoding))
            {
                try
                {
                    ws.Encoding = Encoding.GetEncoding(_declaration.Encoding);
                }
                catch (ArgumentException)
                {
                }
            }
            using (XmlWriter w = XmlWriter.Create(stream, ws))
            {
                Save(w);
            }
        }

        /// <summary>
        /// Output this <see cref="XDocument"/> to the passed in <see cref="TextWriter"/>.
        /// </summary>
        /// <remarks>
        /// The format will be indented by default.  If you want
        /// no indenting then use the SaveOptions version of Save (see
        /// <see cref="XDocument.Save(TextWriter, SaveOptions)"/>) enabling
        /// SaveOptions.DisableFormatting
        /// There is also an option SaveOptions.OmitDuplicateNamespaces for removing duplicate namespace declarations. 
        /// Or instead use the SaveOptions as an annotation on this node or its ancestors, then this method will use those options.
        /// </remarks>
        /// <param name="textWriter">
        /// The <see cref="TextWriter"/> to output this <see cref="XDocument"/> to.
        /// </param>
        public void Save(TextWriter textWriter)
        {
            Save(textWriter, GetSaveOptionsFromAnnotations());
        }

        /// <summary>
        /// Output this <see cref="XDocument"/> to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">
        /// The <see cref="TextWriter"/> to output the XML to.  
        /// </param>
        /// <param name="options">
        /// If SaveOptions.DisableFormatting is enabled the output is not indented.
        /// If SaveOptions.OmitDuplicateNamespaces is enabled duplicate namespace declarations will be removed.
        /// </param>
        public void Save(TextWriter textWriter, SaveOptions options)
        {
            XmlWriterSettings ws = GetXmlWriterSettings(options);
            using (XmlWriter w = XmlWriter.Create(textWriter, ws))
            {
                Save(w);
            }
        }

        /// <summary>
        /// Output this <see cref="XDocument"/> to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to output the XML to.
        /// </param>
        public void Save(XmlWriter writer)
        {
            WriteTo(writer);
        }


        /// <summary>
        /// Output this <see cref="XDocument"/>'s underlying XML tree to the
        /// passed in <see cref="XmlWriter"/>.
        /// <seealso cref="XDocument.Save(XmlWriter)"/>
        /// </summary>
        /// <param name="writer">
        /// The <see cref="XmlWriter"/> to output the content of this 
        /// <see cref="XDocument"/>.
        /// </param>
        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (_declaration != null && _declaration.Standalone == "yes")
            {
                writer.WriteStartDocument(true);
            }
            else if (_declaration != null && _declaration.Standalone == "no")
            {
                writer.WriteStartDocument(false);
            }
            else
            {
                writer.WriteStartDocument();
            }
            WriteContentTo(writer);
            writer.WriteEndDocument();
        }

        internal override void AddAttribute(XAttribute a)
        {
            throw new ArgumentException(SR.Argument_AddAttribute);
        }

        internal override void AddAttributeSkipNotify(XAttribute a)
        {
            throw new ArgumentException(SR.Argument_AddAttribute);
        }

        internal override XNode CloneNode()
        {
            return new XDocument(this);
        }

        internal override bool DeepEquals(XNode node)
        {
            XDocument other = node as XDocument;
            return other != null && ContentsEqual(other);
        }

        internal override int GetDeepHashCode()
        {
            return ContentsHashCode();
        }

        private T GetFirstNode<T>() where T : XNode
        {
            XNode n = content as XNode;
            if (n != null)
            {
                do
                {
                    n = n.next;
                    T e = n as T;
                    if (e != null) return e;
                } while (n != content);
            }
            return null;
        }

        internal static bool IsWhitespace(string s)
        {
            foreach (char ch in s)
            {
                if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n') return false;
            }
            return true;
        }

        internal override void ValidateNode(XNode node, XNode previous)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Text:
                    ValidateString(((XText)node).Value);
                    break;
                case XmlNodeType.Element:
                    ValidateDocument(previous, XmlNodeType.DocumentType, XmlNodeType.None);
                    break;
                case XmlNodeType.DocumentType:
                    ValidateDocument(previous, XmlNodeType.None, XmlNodeType.Element);
                    break;
                case XmlNodeType.CDATA:
                    throw new ArgumentException(SR.Format(SR.Argument_AddNode, XmlNodeType.CDATA));
                case XmlNodeType.Document:
                    throw new ArgumentException(SR.Format(SR.Argument_AddNode, XmlNodeType.Document));
            }
        }

        private void ValidateDocument(XNode previous, XmlNodeType allowBefore, XmlNodeType allowAfter)
        {
            XNode n = content as XNode;
            if (n != null)
            {
                if (previous == null) allowBefore = allowAfter;
                do
                {
                    n = n.next;
                    XmlNodeType nt = n.NodeType;
                    if (nt == XmlNodeType.Element || nt == XmlNodeType.DocumentType)
                    {
                        if (nt != allowBefore) throw new InvalidOperationException(SR.InvalidOperation_DocumentStructure);
                        allowBefore = XmlNodeType.None;
                    }
                    if (n == previous) allowBefore = allowAfter;
                } while (n != content);
            }
        }

        internal override void ValidateString(string s)
        {
            if (!IsWhitespace(s)) throw new ArgumentException(SR.Argument_AddNonWhitespace);
        }
    }
}
