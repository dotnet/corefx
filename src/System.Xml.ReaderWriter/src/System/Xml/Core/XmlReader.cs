// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using System.Security;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Xml.Schema;
using System.Runtime.Versioning;

using BufferBuilder = System.Xml.BufferBuilder;

namespace System.Xml
{
    // Represents a reader that provides fast, non-cached forward only stream access to XML data. 
    /// <summary>Represents a reader that provides fast, noncached, forward-only access to XML data.To browse the .NET Framework source code for this type, see the Reference Source.</summary>
    public abstract partial class XmlReader : IDisposable
    {
        static private uint s_isTextualNodeBitmap = 0x6018; // 00 0110 0000 0001 1000
        // 0 None, 
        // 0 Element,
        // 0 Attribute,
        // 1 Text,
        // 1 CDATA,
        // 0 EntityReference,
        // 0 Entity,
        // 0 ProcessingInstruction,
        // 0 Comment,
        // 0 Document,
        // 0 DocumentType,
        // 0 DocumentFragment,
        // 0 Notation,
        // 1 Whitespace,
        // 1 SignificantWhitespace,
        // 0 EndElement,
        // 0 EndEntity,
        // 0 XmlDeclaration

        static private uint s_canReadContentAsBitmap = 0x1E1BC; // 01 1110 0001 1011 1100
        // 0 None, 
        // 0 Element,
        // 1 Attribute,
        // 1 Text,
        // 1 CDATA,
        // 1 EntityReference,
        // 0 Entity,
        // 1 ProcessingInstruction,
        // 1 Comment,
        // 0 Document,
        // 0 DocumentType,
        // 0 DocumentFragment,
        // 0 Notation,
        // 1 Whitespace,
        // 1 SignificantWhitespace,
        // 1 EndElement,
        // 1 EndEntity,
        // 0 XmlDeclaration

        static private uint s_hasValueBitmap = 0x2659C; // 10 0110 0101 1001 1100
        // 0 None, 
        // 0 Element,
        // 1 Attribute,
        // 1 Text,
        // 1 CDATA,
        // 0 EntityReference,
        // 0 Entity,
        // 1 ProcessingInstruction,
        // 1 Comment,
        // 0 Document,
        // 1 DocumentType,
        // 0 DocumentFragment,
        // 0 Notation,
        // 1 Whitespace,
        // 1 SignificantWhitespace,
        // 0 EndElement,
        // 0 EndEntity,
        // 1 XmlDeclaration

        //
        // Constants
        //
        internal const int DefaultBufferSize = 4096;
        internal const int BiggerBufferSize = 8192;
        internal const int MaxStreamLengthForDefaultBufferSize = 64 * 1024; // 64kB

        internal const int AsyncBufferSize = 64 * 1024; //64KB

        // Settings
        /// <summary>Gets the <see cref="T:System.Xml.XmlReaderSettings" /> object used to create this <see cref="T:System.Xml.XmlReader" /> instance.</summary>
        /// <returns>The <see cref="T:System.Xml.XmlReaderSettings" /> object used to create this reader instance. If this reader was not created using the <see cref="Overload:System.Xml.XmlReader.Create" /> method, this property returns null.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual XmlReaderSettings Settings
        {
            get
            {
                return null;
            }
        }

        // Node Properties
        // Get the type of the current node.
        /// <summary>When overridden in a derived class, gets the type of the current node.</summary>
        /// <returns>One of the enumeration values that specify the type of the current node.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract XmlNodeType NodeType { get; }

        // Gets the name of the current node, including the namespace prefix.
        /// <summary>When overridden in a derived class, gets the qualified name of the current node.</summary>
        /// <returns>The qualified name of the current node. For example, Name is bk:book for the element &lt;bk:book&gt;.The name returned is dependent on the <see cref="P:System.Xml.XmlReader.NodeType" /> of the node. The following node types return the listed values. All other node types return an empty string.Node type Name AttributeThe name of the attribute. DocumentTypeThe document type name. ElementThe tag name. EntityReferenceThe name of the entity referenced. ProcessingInstructionThe target of the processing instruction. XmlDeclarationThe literal string xml. </returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string Name
        {
            get
            {
                if (Prefix.Length == 0)
                {
                    return LocalName;
                }
                else
                {
                    return NameTable.Add(string.Concat(Prefix, ":", LocalName));
                }
            }
        }

        // Gets the name of the current node without the namespace prefix.
        /// <summary>When overridden in a derived class, gets the local name of the current node.</summary>
        /// <returns>The name of the current node with the prefix removed. For example, LocalName is book for the element &lt;bk:book&gt;.For node types that do not have a name (like Text, Comment, and so on), this property returns String.Empty.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string LocalName { get; }

        // Gets the namespace URN (as defined in the W3C Namespace Specification) of the current namespace scope.
        /// <summary>When overridden in a derived class, gets the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is positioned.</summary>
        /// <returns>The namespace URI of the current node; otherwise an empty string.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string NamespaceURI { get; }

        // Gets the namespace prefix associated with the current node.
        /// <summary>When overridden in a derived class, gets the namespace prefix associated with the current node.</summary>
        /// <returns>The namespace prefix associated with the current node.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string Prefix { get; }

        // Gets a value indicating whether
        /// <summary>When overridden in a derived class, gets a value indicating whether the current node can have a <see cref="P:System.Xml.XmlReader.Value" />.</summary>
        /// <returns>true if the node on which the reader is currently positioned can have a Value; otherwise, false. If false, the node has a value of String.Empty.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool HasValue
        {
            get
            {
                return HasValueInternal(this.NodeType);
            }
        }

        // Gets the text value of the current node.
        /// <summary>When overridden in a derived class, gets the text value of the current node.</summary>
        /// <returns>The value returned depends on the <see cref="P:System.Xml.XmlReader.NodeType" /> of the node. The following table lists node types that have a value to return. All other node types return String.Empty.Node type Value AttributeThe value of the attribute. CDATAThe content of the CDATA section. CommentThe content of the comment. DocumentTypeThe internal subset. ProcessingInstructionThe entire content, excluding the target. SignificantWhitespaceThe white space between markup in a mixed content model. TextThe content of the text node. WhitespaceThe white space between markup. XmlDeclarationThe content of the declaration. </returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string Value { get; }

        // Gets the depth of the current node in the XML element stack.
        /// <summary>When overridden in a derived class, gets the depth of the current node in the XML document.</summary>
        /// <returns>The depth of the current node in the XML document.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract int Depth { get; }

        // Gets the base URI of the current node.
        /// <summary>When overridden in a derived class, gets the base URI of the current node.</summary>
        /// <returns>The base URI of the current node.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string BaseURI { get; }

        // Gets a value indicating whether the current node is an empty element (for example, <MyElement/>).
        /// <summary>When overridden in a derived class, gets a value indicating whether the current node is an empty element (for example, &lt;MyElement/&gt;).</summary>
        /// <returns>true if the current node is an element (<see cref="P:System.Xml.XmlReader.NodeType" /> equals XmlNodeType.Element) that ends with /&gt;; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool IsEmptyElement { get; }

        // Gets a value indicating whether the current node is an attribute that was generated from the default value defined
        // in the DTD or schema.
        /// <summary>When overridden in a derived class, gets a value indicating whether the current node is an attribute that was generated from the default value defined in the DTD or schema.</summary>
        /// <returns>true if the current node is an attribute whose value was generated from the default value defined in the DTD or schema; false if the attribute value was explicitly set.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool IsDefault
        {
            get
            {
                return false;
            }
        }


        // Gets the current xml:space scope.
        /// <summary>When overridden in a derived class, gets the current xml:space scope.</summary>
        /// <returns>One of the <see cref="T:System.Xml.XmlSpace" /> values. If no xml:space scope exists, this property defaults to XmlSpace.None.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual XmlSpace XmlSpace
        {
            get
            {
                return XmlSpace.None;
            }
        }

        // Gets the current xml:lang scope.
        /// <summary>When overridden in a derived class, gets the current xml:lang scope.</summary>
        /// <returns>The current xml:lang scope.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string XmlLang
        {
            get
            {
                return string.Empty;
            }
        }


        // returns the type of the current node
        /// <summary>Gets The Common Language Runtime (CLR) type for the current node.</summary>
        /// <returns>The CLR type that corresponds to the typed value of the node. The default is System.String.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual System.Type ValueType
        {
            get
            {
                return typeof(string);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and returns the content as the most appropriate type (by default as string). Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as an <see cref="T:System.Object" />.</summary>
        /// <returns>The text content as the most appropriate common language runtime (CLR) object.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual object ReadContentAsObject()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsObject");
            }
            return InternalReadContentAsString();
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to a boolean. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a Boolean.</summary>
        /// <returns>The text content as a <see cref="T:System.Boolean" /> object.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool ReadContentAsBoolean()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsBoolean");
            }
            try
            {
                return XmlConvert.ToBoolean(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "Boolean", e, this as IXmlLineInfo);
            }
        }


        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to a DateTimeOffset. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a <see cref="T:System.DateTimeOffset" /> object.</summary>
        /// <returns>The text content as a <see cref="T:System.DateTimeOffset" /> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual DateTimeOffset ReadContentAsDateTimeOffset()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsDateTimeOffset");
            }
            try
            {
                return XmlConvert.ToDateTimeOffset(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "DateTimeOffset", e, this as IXmlLineInfo);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to a double. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a double-precision floating-point number.</summary>
        /// <returns>The text content as a double-precision floating-point number.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual double ReadContentAsDouble()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsDouble");
            }
            try
            {
                return XmlConvert.ToDouble(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "Double", e, this as IXmlLineInfo);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to a float. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a single-precision floating point number.</summary>
        /// <returns>The text content at the current position as a single-precision floating point number.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual float ReadContentAsFloat()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsFloat");
            }
            try
            {
                return XmlConvert.ToSingle(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "Float", e, this as IXmlLineInfo);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to a decimal. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a <see cref="T:System.Decimal" /> object.</summary>
        /// <returns>The text content at the current position as a <see cref="T:System.Decimal" /> object.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual decimal ReadContentAsDecimal()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsDecimal");
            }
            try
            {
                return XmlConvert.ToDecimal(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "Decimal", e, this as IXmlLineInfo);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to an int. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a 32-bit signed integer.</summary>
        /// <returns>The text content as a 32-bit signed integer.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadContentAsInt()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsInt");
            }
            try
            {
                return XmlConvert.ToInt32(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "Int", e, this as IXmlLineInfo);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to a long. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a 64-bit signed integer.</summary>
        /// <returns>The text content as a 64-bit signed integer.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual long ReadContentAsLong()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsLong");
            }
            try
            {
                return XmlConvert.ToInt64(InternalReadContentAsString());
            }
            catch (FormatException e)
            {
                throw new XmlException(SR.Xml_ReadContentAsFormatException, "Long", e, this as IXmlLineInfo);
            }
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and returns the content as a string. Stops at start tags and end tags.
        /// <summary>Reads the text content at the current position as a <see cref="T:System.String" /> object.</summary>
        /// <returns>The text content as a <see cref="T:System.String" /> object.</returns>
        /// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
        /// <exception cref="T:System.FormatException">The string format is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string ReadContentAsString()
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAsString");
            }
            return InternalReadContentAsString();
        }

        // Concatenates values of textual nodes of the current content, ignoring comments and PIs, expanding entity references, 
        // and converts the content to the requested type. Stops at start tags and end tags.
        public virtual object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            if (!CanReadContentAs())
            {
                throw CreateReadContentAsException("ReadContentAs");
            }

            string strContentValue = InternalReadContentAsString();
            if (returnType == typeof(string))
            {
                return strContentValue;
            }
            else
            {
                try
                {
                    return XmlUntypedStringConverter.Instance.FromString(strContentValue, returnType, (namespaceResolver == null ? this as IXmlNamespaceResolver : namespaceResolver));
                }
                catch (FormatException e)
                {
                    throw new XmlException(SR.Xml_ReadContentAsFormatException, returnType.ToString(), e, this as IXmlLineInfo);
                }
                catch (InvalidCastException e)
                {
                    throw new XmlException(SR.Xml_ReadContentAsFormatException, returnType.ToString(), e, this as IXmlLineInfo);
                }
            }
        }

        // Returns the content of the current element as the most appropriate type. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as an <see cref="T:System.Object" />.</summary>
        /// <returns>A boxed common language runtime (CLR) object of the most appropriate type. The <see cref="P:System.Xml.XmlReader.ValueType" /> property determines the appropriate CLR type. If the content is typed as a list type, this method returns an array of boxed objects of the appropriate type.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual object ReadElementContentAsObject()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsObject"))
            {
                object value = ReadContentAsObject();
                FinishReadElementContentAsXxx();
                return value;
            }
            return string.Empty;
        }

        // Checks local name and namespace of the current element and returns its content as the most appropriate type. Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as an <see cref="T:System.Object" />.</summary>
        /// <returns>A boxed common language runtime (CLR) object of the most appropriate type. The <see cref="P:System.Xml.XmlReader.ValueType" /> property determines the appropriate CLR type. If the content is typed as a list type, this method returns an array of boxed objects of the appropriate type.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual object ReadElementContentAsObject(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsObject();
        }

        // Returns the content of the current element as a boolean. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as a <see cref="T:System.Boolean" /> object.</summary>
        /// <returns>The element content as a <see cref="T:System.Boolean" /> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.Boolean" /> object.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool ReadElementContentAsBoolean()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsBoolean"))
            {
                bool value = ReadContentAsBoolean();
                FinishReadElementContentAsXxx();
                return value;
            }
            return XmlConvert.ToBoolean(string.Empty);
        }

        // Checks local name and namespace of the current element and returns its content as a boolean. Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.Boolean" /> object.</summary>
        /// <returns>The element content as a <see cref="T:System.Boolean" /> object.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsBoolean();
        }


        // Returns the content of the current element as a double. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as a double-precision floating-point number.</summary>
        /// <returns>The element content as a double-precision floating-point number.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a double-precision floating-point number.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual double ReadElementContentAsDouble()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsDouble"))
            {
                double value = ReadContentAsDouble();
                FinishReadElementContentAsXxx();
                return value;
            }
            return XmlConvert.ToDouble(string.Empty);
        }

        // Checks local name and namespace of the current element and returns its content as a double. 
        // Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a double-precision floating-point number.</summary>
        /// <returns>The element content as a double-precision floating-point number.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual double ReadElementContentAsDouble(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsDouble();
        }

        // Returns the content of the current element as a float. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as single-precision floating-point number.</summary>
        /// <returns>The element content as a single-precision floating point number.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a single-precision floating-point number.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual float ReadElementContentAsFloat()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsFloat"))
            {
                float value = ReadContentAsFloat();
                FinishReadElementContentAsXxx();
                return value;
            }
            return XmlConvert.ToSingle(string.Empty);
        }

        // Checks local name and namespace of the current element and returns its content as a float. 
        // Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a single-precision floating-point number.</summary>
        /// <returns>The element content as a single-precision floating point number.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a single-precision floating-point number.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual float ReadElementContentAsFloat(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsFloat();
        }

        // Returns the content of the current element as a decimal. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as a <see cref="T:System.Decimal" /> object.</summary>
        /// <returns>The element content as a <see cref="T:System.Decimal" /> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.Decimal" />.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual decimal ReadElementContentAsDecimal()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsDecimal"))
            {
                decimal value = ReadContentAsDecimal();
                FinishReadElementContentAsXxx();
                return value;
            }
            return XmlConvert.ToDecimal(string.Empty);
        }

        // Checks local name and namespace of the current element and returns its content as a decimal. 
        // Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.Decimal" /> object.</summary>
        /// <returns>The element content as a <see cref="T:System.Decimal" /> object.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.Decimal" />.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsDecimal();
        }

        // Returns the content of the current element as an int. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as a 32-bit signed integer.</summary>
        /// <returns>The element content as a 32-bit signed integer.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 32-bit signed integer.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadElementContentAsInt()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsInt"))
            {
                int value = ReadContentAsInt();
                FinishReadElementContentAsXxx();
                return value;
            }
            return XmlConvert.ToInt32(string.Empty);
        }

        // Checks local name and namespace of the current element and returns its content as an int. 
        // Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a 32-bit signed integer.</summary>
        /// <returns>The element content as a 32-bit signed integer.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 32-bit signed integer.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadElementContentAsInt(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsInt();
        }

        // Returns the content of the current element as a long. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as a 64-bit signed integer.</summary>
        /// <returns>The element content as a 64-bit signed integer.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 64-bit signed integer.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual long ReadElementContentAsLong()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsLong"))
            {
                long value = ReadContentAsLong();
                FinishReadElementContentAsXxx();
                return value;
            }
            return XmlConvert.ToInt64(string.Empty);
        }

        // Checks local name and namespace of the current element and returns its content as a long. 
        // Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a 64-bit signed integer.</summary>
        /// <returns>The element content as a 64-bit signed integer.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 64-bit signed integer.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual long ReadElementContentAsLong(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsLong();
        }

        // Returns the content of the current element as a string. Moves to the node following the element's end tag.
        /// <summary>Reads the current element and returns the contents as a <see cref="T:System.String" /> object.</summary>
        /// <returns>The element content as a <see cref="T:System.String" /> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.String" /> object.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string ReadElementContentAsString()
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAsString"))
            {
                string value = ReadContentAsString();
                FinishReadElementContentAsXxx();
                return value;
            }
            return string.Empty;
        }

        // Checks local name and namespace of the current element and returns its content as a string. 
        // Moves to the node following the element's end tag.
        /// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.String" /> object.</summary>
        /// <returns>The element content as a <see cref="T:System.String" /> object.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
        /// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.String" /> object.</exception>
        /// <exception cref="T:System.ArgumentNullException">The method is called with null arguments.</exception>
        /// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string ReadElementContentAsString(string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAsString();
        }

        // Returns the content of the current element as the requested type. Moves to the node following the element's end tag.
        public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            if (SetupReadElementContentAsXxx("ReadElementContentAs"))
            {
                object value = ReadContentAs(returnType, namespaceResolver);
                FinishReadElementContentAsXxx();
                return value;
            }
            return (returnType == typeof(string)) ? string.Empty : XmlUntypedStringConverter.Instance.FromString(string.Empty, returnType, namespaceResolver);
        }

        // Checks local name and namespace of the current element and returns its content as the requested type. 
        // Moves to the node following the element's end tag.
        public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
        {
            CheckElement(localName, namespaceURI);
            return ReadElementContentAs(returnType, namespaceResolver);
        }

        // Attribute Accessors
        // The number of attributes on the current node.
        /// <summary>When overridden in a derived class, gets the number of attributes on the current node.</summary>
        /// <returns>The number of attributes on the current node.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract int AttributeCount { get; }

        // Gets the value of the attribute with the specified Name
        /// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.</summary>
        /// <returns>The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned.</returns>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string GetAttribute(string name);

        // Gets the value of the attribute with the LocalName and NamespaceURI
        /// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.</summary>
        /// <returns>The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned. This method does not move the reader.</returns>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string GetAttribute(string name, string namespaceURI);

        // Gets the value of the attribute with the specified index.
        /// <summary>When overridden in a derived class, gets the value of the attribute with the specified index.</summary>
        /// <returns>The value of the specified attribute. This method does not move the reader.</returns>
        /// <param name="i">The index of the attribute. The index is zero-based. (The first attribute has index 0.)</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="i" /> is out of range. It must be non-negative and less than the size of the attribute collection.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string GetAttribute(int i);

        // Gets the value of the attribute with the specified index.
        /// <summary>When overridden in a derived class, gets the value of the attribute with the specified index.</summary>
        /// <returns>The value of the specified attribute.</returns>
        /// <param name="i">The index of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string this[int i]
        {
            get
            {
                return GetAttribute(i);
            }
        }

        // Gets the value of the attribute with the specified Name.
        /// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.</summary>
        /// <returns>The value of the specified attribute. If the attribute is not found, null is returned.</returns>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string this[string name]
        {
            get
            {
                return GetAttribute(name);
            }
        }

        // Gets the value of the attribute with the LocalName and NamespaceURI
        /// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.</summary>
        /// <returns>The value of the specified attribute. If the attribute is not found, null is returned.</returns>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="namespaceURI">The namespace URI of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string this[string name, string namespaceURI]
        {
            get
            {
                return GetAttribute(name, namespaceURI);
            }
        }

        // Moves to the attribute with the specified Name.
        /// <summary>When overridden in a derived class, moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.</summary>
        /// <returns>true if the attribute is found; otherwise, false. If false, the reader's position does not change.</returns>
        /// <param name="name">The qualified name of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
        public abstract bool MoveToAttribute(string name);

        // Moves to the attribute with the specified LocalName and NamespaceURI.
        /// <summary>When overridden in a derived class, moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.</summary>
        /// <returns>true if the attribute is found; otherwise, false. If false, the reader's position does not change.</returns>
        /// <param name="name">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentNullException">Both parameter values are null.</exception>
        public abstract bool MoveToAttribute(string name, string ns);

        // Moves to the attribute with the specified index.
        /// <summary>When overridden in a derived class, moves to the attribute with the specified index.</summary>
        /// <param name="i">The index of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The parameter has a negative value.</exception>
        public virtual void MoveToAttribute(int i)
        {
            if (i < 0 || i >= AttributeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(i));
            }
            MoveToElement();
            MoveToFirstAttribute();
            int j = 0;
            while (j < i)
            {
                MoveToNextAttribute();
                j++;
            }
        }

        // Moves to the first attribute of the current node.
        /// <summary>When overridden in a derived class, moves to the first attribute.</summary>
        /// <returns>true if an attribute exists (the reader moves to the first attribute); otherwise, false (the position of the reader does not change).</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool MoveToFirstAttribute();

        // Moves to the next attribute.
        /// <summary>When overridden in a derived class, moves to the next attribute.</summary>
        /// <returns>true if there is a next attribute; false if there are no more attributes.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool MoveToNextAttribute();

        // Moves to the element that contains the current attribute node.
        /// <summary>When overridden in a derived class, moves to the element that contains the current attribute node.</summary>
        /// <returns>true if the reader is positioned on an attribute (the reader moves to the element that owns the attribute); false if the reader is not positioned on an attribute (the position of the reader does not change).</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool MoveToElement();

        // Parses the attribute value into one or more Text and/or EntityReference node types.

        /// <summary>When overridden in a derived class, parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.</summary>
        /// <returns>true if there are nodes to return.false if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read.An empty attribute, such as, misc="", returns true with a single node with a value of String.Empty.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool ReadAttributeValue();

        // Moving through the Stream
        // Reads the next node from the stream.

        /// <summary>When overridden in a derived class, reads the next node from the stream.</summary>
        /// <returns>true if the next node was read successfully; otherwise, false.</returns>
        /// <exception cref="T:System.Xml.XmlException">An error occurred while parsing the XML.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool Read();

        // Returns true when the XmlReader is positioned at the end of the stream.
        /// <summary>When overridden in a derived class, gets a value indicating whether the reader is positioned at the end of the stream.</summary>
        /// <returns>true if the reader is positioned at the end of the stream; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract bool EOF { get; }

        // Returns the read state of the XmlReader.
        /// <summary>When overridden in a derived class, gets the state of the reader.</summary>
        /// <returns>One of the enumeration values that specifies the state of the reader.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract ReadState ReadState { get; }

        // Skips to the end tag of the current element.
        /// <summary>Skips the children of the current node.</summary>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void Skip()
        {
            if (ReadState != ReadState.Interactive)
            {
                return;
            }
            SkipSubtree();
        }

        // Gets the XmlNameTable associated with the XmlReader.
        /// <summary>When overridden in a derived class, gets the <see cref="T:System.Xml.XmlNameTable" /> associated with this implementation.</summary>
        /// <returns>The XmlNameTable enabling you to get the atomized version of a string within the node.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract XmlNameTable NameTable { get; }

        // Resolves a namespace prefix in the current element's scope.
        /// <summary>When overridden in a derived class, resolves a namespace prefix in the current element's scope.</summary>
        /// <returns>The namespace URI to which the prefix maps or null if no matching prefix is found.</returns>
        /// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass an empty string. </param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string LookupNamespace(string prefix);

        // Returns true if the XmlReader can expand general entities.
        /// <summary>Gets a value indicating whether this reader can parse and resolve entities.</summary>
        /// <returns>true if the reader can parse and resolve entities; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool CanResolveEntity
        {
            get
            {
                return false;
            }
        }

        // Resolves the entity reference for nodes of NodeType EntityReference.
        /// <summary>When overridden in a derived class, resolves the entity reference for EntityReference nodes.</summary>
        /// <exception cref="T:System.InvalidOperationException">The reader is not positioned on an EntityReference node; this implementation of the reader cannot resolve entities (<see cref="P:System.Xml.XmlReader.CanResolveEntity" /> returns false).</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void ResolveEntity();

        // Binary content access methods
        // Returns true if the reader supports call to ReadContentAsBase64, ReadElementContentAsBase64, ReadContentAsBinHex and ReadElementContentAsBinHex.
        /// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlReader" /> implements the binary content read methods.</summary>
        /// <returns>true if the binary content read methods are implemented; otherwise false.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool CanReadBinaryContent
        {
            get
            {
                return false;
            }
        }

        // Returns decoded bytes of the current base64 text content. Call this methods until it returns 0 to get all the data.
        /// <summary>Reads the content and returns the Base64 decoded binary bytes.</summary>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be null.</param>
        /// <param name="index">The offset into the buffer where to start copying the result.</param>
        /// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is null.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Xml.XmlReader.ReadContentAsBase64(System.Byte[],System.Int32,System.Int32)" /> is not supported on the current node.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadContentAsBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(SR.Format(SR.Xml_ReadBinaryContentNotSupported, "ReadContentAsBase64"));
        }

        // Returns decoded bytes of the current base64 element content. Call this methods until it returns 0 to get all the data.
        /// <summary>Reads the element and decodes the Base64 content.</summary>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be null.</param>
        /// <param name="index">The offset into the buffer where to start copying the result.</param>
        /// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
        /// <exception cref="T:System.Xml.XmlException">The element contains mixed-content.</exception>
        /// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadElementContentAsBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(SR.Format(SR.Xml_ReadBinaryContentNotSupported, "ReadElementContentAsBase64"));
        }

        // Returns decoded bytes of the current binhex text content. Call this methods until it returns 0 to get all the data.
        /// <summary>Reads the content and returns the BinHex decoded binary bytes.</summary>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be null.</param>
        /// <param name="index">The offset into the buffer where to start copying the result.</param>
        /// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is null.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Xml.XmlReader.ReadContentAsBinHex(System.Byte[],System.Int32,System.Int32)" /> is not supported on the current node.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadContentAsBinHex(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(SR.Format(SR.Xml_ReadBinaryContentNotSupported, "ReadContentAsBinHex"));
        }

        // Returns decoded bytes of the current binhex element content. Call this methods until it returns 0 to get all the data.
        /// <summary>Reads the element and decodes the BinHex content.</summary>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be null.</param>
        /// <param name="index">The offset into the buffer where to start copying the result.</param>
        /// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
        /// <exception cref="T:System.Xml.XmlException">The element contains mixed-content.</exception>
        /// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(SR.Format(SR.Xml_ReadBinaryContentNotSupported, "ReadElementContentAsBinHex"));
        }

        // Text streaming methods

        // Returns true if the XmlReader supports calls to ReadValueChunk.
        /// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlReader" /> implements the <see cref="M:System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)" /> method.</summary>
        /// <returns>true if the <see cref="T:System.Xml.XmlReader" /> implements the <see cref="M:System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)" /> method; otherwise false.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool CanReadValueChunk
        {
            get
            {
                return false;
            }
        }

        // Returns a chunk of the value of the current node. Call this method in a loop to get all the data. 
        // Use this method to get a streaming access to the value of the current node.
        /// <summary>Reads large streams of text embedded in an XML document.</summary>
        /// <returns>The number of characters read into the buffer. The value zero is returned when there is no more text content.</returns>
        /// <param name="buffer">The array of characters that serves as the buffer to which the text contents are written. This value cannot be null.</param>
        /// <param name="index">The offset within the buffer where the <see cref="T:System.Xml.XmlReader" /> can start to copy the results.</param>
        /// <param name="count">The maximum number of characters to copy into the buffer. The actual number of characters copied is returned from this method.</param>
        /// <exception cref="T:System.InvalidOperationException">The current node does not have a value (<see cref="P:System.Xml.XmlReader.HasValue" /> is false).</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer, or index + count is larger than the allocated buffer size.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
        /// <exception cref="T:System.Xml.XmlException">The XML data is not well-formed.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual int ReadValueChunk(char[] buffer, int index, int count)
        {
            throw new NotSupportedException(SR.Xml_ReadValueChunkNotSupported);
        }


        // Checks whether the current node is a content (non-whitespace text, CDATA, Element, EndElement, EntityReference
        // or EndEntity) node. If the node is not a content node, then the method skips ahead to the next content node or 
        // end of file. Skips over nodes of type ProcessingInstruction, DocumentType, Comment, Whitespace and SignificantWhitespace.
        /// <summary>Checks whether the current node is a content (non-white space text, CDATA, Element, EndElement, EntityReference, or EndEntity) node. If the node is not a content node, the reader skips ahead to the next content node or end of file. It skips over nodes of the following type: ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace.</summary>
        /// <returns>The <see cref="P:System.Xml.XmlReader.NodeType" /> of the current node found by the method or XmlNodeType.None if the reader has reached the end of the input stream.</returns>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML encountered in the input stream.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual XmlNodeType MoveToContent()
        {
            do
            {
                switch (this.NodeType)
                {
                    case XmlNodeType.Attribute:
                        MoveToElement();
                        goto case XmlNodeType.Element;
                    case XmlNodeType.Element:
                    case XmlNodeType.EndElement:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    case XmlNodeType.EntityReference:
                    case XmlNodeType.EndEntity:
                        return this.NodeType;
                }
            } while (Read());
            return this.NodeType;
        }

        // Checks that the current node is an element and advances the reader to the next node.
        /// <summary>Checks that the current node is an element and advances the reader to the next node.</summary>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML was encountered in the input stream.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void ReadStartElement()
        {
            if (MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException(SR.Xml_InvalidNodeType, this.NodeType.ToString(), this as IXmlLineInfo);
            }
            Read();
        }

        // Checks that the current content node is an element with the given Name and advances the reader to the next node.
        /// <summary>Checks that the current content node is an element with the given <see cref="P:System.Xml.XmlReader.Name" /> and advances the reader to the next node.</summary>
        /// <param name="name">The qualified name of the element.</param>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML was encountered in the input stream. -or- The <see cref="P:System.Xml.XmlReader.Name" /> of the element does not match the given <paramref name="name" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void ReadStartElement(string name)
        {
            if (MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException(SR.Xml_InvalidNodeType, this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if (this.Name == name)
            {
                Read();
            }
            else
            {
                throw new XmlException(SR.Xml_ElementNotFound, name, this as IXmlLineInfo);
            }
        }

        // Checks that the current content node is an element with the given LocalName and NamespaceURI
        // and advances the reader to the next node.
        /// <summary>Checks that the current content node is an element with the given <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> and advances the reader to the next node.</summary>
        /// <param name="localname">The local name of the element.</param>
        /// <param name="ns">The namespace URI of the element.</param>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML was encountered in the input stream.-or-The <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> properties of the element found do not match the given arguments.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void ReadStartElement(string localname, string ns)
        {
            if (MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException(SR.Xml_InvalidNodeType, this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if (this.LocalName == localname && this.NamespaceURI == ns)
            {
                Read();
            }
            else
            {
                throw new XmlException(SR.Xml_ElementNotFoundNs, new string[2] { localname, ns }, this as IXmlLineInfo);
            }
        }

        // Checks that the current content node is an end tag and advances the reader to the next node.
        /// <summary>Checks that the current content node is an end tag and advances the reader to the next node.</summary>
        /// <exception cref="T:System.Xml.XmlException">The current node is not an end tag or if incorrect XML is encountered in the input stream.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void ReadEndElement()
        {
            if (MoveToContent() != XmlNodeType.EndElement)
            {
                throw new XmlException(SR.Xml_InvalidNodeType, this.NodeType.ToString(), this as IXmlLineInfo);
            }
            Read();
        }

        // Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element).
        /// <summary>Calls <see cref="M:System.Xml.XmlReader.MoveToContent" /> and tests if the current content node is a start tag or empty element tag.</summary>
        /// <returns>true if <see cref="M:System.Xml.XmlReader.MoveToContent" /> finds a start tag or empty element tag; false if a node type other than XmlNodeType.Element was found.</returns>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML is encountered in the input stream.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool IsStartElement()
        {
            return MoveToContent() == XmlNodeType.Element;
        }

        // Calls MoveToContentand tests if the current content node is a start tag or empty element tag (XmlNodeType.Element) and if the
        // Name property of the element found matches the given argument.
        /// <summary>Calls <see cref="M:System.Xml.XmlReader.MoveToContent" /> and tests if the current content node is a start tag or empty element tag and if the <see cref="P:System.Xml.XmlReader.Name" /> property of the element found matches the given argument.</summary>
        /// <returns>true if the resulting node is an element and the Name property matches the specified string. false if a node type other than XmlNodeType.Element was found or if the element Name property does not match the specified string.</returns>
        /// <param name="name">The string matched against the Name property of the element found.</param>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML is encountered in the input stream.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool IsStartElement(string name)
        {
            return (MoveToContent() == XmlNodeType.Element) &&
                   (this.Name == name);
        }

        // Calls MoveToContent and tests if the current content node is a start tag or empty element tag (XmlNodeType.Element) and if
        // the LocalName and NamespaceURI properties of the element found match the given strings.
        /// <summary>Calls <see cref="M:System.Xml.XmlReader.MoveToContent" /> and tests if the current content node is a start tag or empty element tag and if the <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> properties of the element found match the given strings.</summary>
        /// <returns>true if the resulting node is an element. false if a node type other than XmlNodeType.Element was found or if the LocalName and NamespaceURI properties of the element do not match the specified strings.</returns>
        /// <param name="localname">The string to match against the LocalName property of the element found.</param>
        /// <param name="ns">The string to match against the NamespaceURI property of the element found.</param>
        /// <exception cref="T:System.Xml.XmlException">Incorrect XML is encountered in the input stream.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool IsStartElement(string localname, string ns)
        {
            return (MoveToContent() == XmlNodeType.Element) &&
                   (this.LocalName == localname && this.NamespaceURI == ns);
        }

        // Reads to the following element with the given Name.
        /// <summary>Reads until an element with the specified qualified name is found.</summary>
        /// <returns>true if a matching element is found; otherwise false and the <see cref="T:System.Xml.XmlReader" /> is in an end of file state.</returns>
        /// <param name="name">The qualified name of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
        public virtual bool ReadToFollowing(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
            }
            // atomize name
            name = NameTable.Add(name);

            // find following element with that name
            while (Read())
            {
                if (NodeType == XmlNodeType.Element && Ref.Equal(name, Name))
                {
                    return true;
                }
            }
            return false;
        }

        // Reads to the following element with the given LocalName and NamespaceURI.
        /// <summary>Reads until an element with the specified local name and namespace URI is found.</summary>
        /// <returns>true if a matching element is found; otherwise false and the <see cref="T:System.Xml.XmlReader" /> is in an end of file state.</returns>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="namespaceURI">The namespace URI of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentNullException">Both parameter values are null.</exception>
        public virtual bool ReadToFollowing(string localName, string namespaceURI)
        {
            if (localName == null || localName.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException(nameof(namespaceURI));
            }

            // atomize local name and namespace
            localName = NameTable.Add(localName);
            namespaceURI = NameTable.Add(namespaceURI);

            // find following element with that name
            while (Read())
            {
                if (NodeType == XmlNodeType.Element && Ref.Equal(localName, LocalName) && Ref.Equal(namespaceURI, NamespaceURI))
                {
                    return true;
                }
            }
            return false;
        }

        // Reads to the first descendant of the current element with the given Name.
        /// <summary>Advances the <see cref="T:System.Xml.XmlReader" /> to the next descendant element with the specified qualified name.</summary>
        /// <returns>true if a matching descendant element is found; otherwise false. If a matching child element is not found, the <see cref="T:System.Xml.XmlReader" /> is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is XmlNodeType.EndElement) of the element.If the <see cref="T:System.Xml.XmlReader" /> is not positioned on an element when <see cref="M:System.Xml.XmlReader.ReadToDescendant(System.String)" /> was called, this method returns false and the position of the <see cref="T:System.Xml.XmlReader" /> is not changed.</returns>
        /// <param name="name">The qualified name of the element you wish to move to.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
        public virtual bool ReadToDescendant(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
            }
            // save the element or root depth
            int parentDepth = Depth;
            if (NodeType != XmlNodeType.Element)
            {
                // adjust the depth if we are on root node
                if (ReadState == ReadState.Initial)
                {
                    Debug.Assert(parentDepth == 0);
                    parentDepth--;
                }
                else
                {
                    return false;
                }
            }
            else if (IsEmptyElement)
            {
                return false;
            }

            // atomize name
            name = NameTable.Add(name);

            // find the descendant
            while (Read() && Depth > parentDepth)
            {
                if (NodeType == XmlNodeType.Element && Ref.Equal(name, Name))
                {
                    return true;
                }
            }
            Debug.Assert(NodeType == XmlNodeType.EndElement || NodeType == XmlNodeType.None || ReadState == ReadState.Error);
            return false;
        }

        // Reads to the first descendant of the current element with the given LocalName and NamespaceURI.
        /// <summary>Advances the <see cref="T:System.Xml.XmlReader" /> to the next descendant element with the specified local name and namespace URI.</summary>
        /// <returns>true if a matching descendant element is found; otherwise false. If a matching child element is not found, the <see cref="T:System.Xml.XmlReader" /> is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is XmlNodeType.EndElement) of the element.If the <see cref="T:System.Xml.XmlReader" /> is not positioned on an element when <see cref="M:System.Xml.XmlReader.ReadToDescendant(System.String,System.String)" /> was called, this method returns false and the position of the <see cref="T:System.Xml.XmlReader" /> is not changed.</returns>
        /// <param name="localName">The local name of the element you wish to move to.</param>
        /// <param name="namespaceURI">The namespace URI of the element you wish to move to.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentNullException">Both parameter values are null.</exception>
        public virtual bool ReadToDescendant(string localName, string namespaceURI)
        {
            if (localName == null || localName.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException(nameof(namespaceURI));
            }
            // save the element or root depth
            int parentDepth = Depth;
            if (NodeType != XmlNodeType.Element)
            {
                // adjust the depth if we are on root node
                if (ReadState == ReadState.Initial)
                {
                    Debug.Assert(parentDepth == 0);
                    parentDepth--;
                }
                else
                {
                    return false;
                }
            }
            else if (IsEmptyElement)
            {
                return false;
            }

            // atomize local name and namespace
            localName = NameTable.Add(localName);
            namespaceURI = NameTable.Add(namespaceURI);

            // find the descendant
            while (Read() && Depth > parentDepth)
            {
                if (NodeType == XmlNodeType.Element && Ref.Equal(localName, LocalName) && Ref.Equal(namespaceURI, NamespaceURI))
                {
                    return true;
                }
            }
            Debug.Assert(NodeType == XmlNodeType.EndElement);
            return false;
        }

        // Reads to the next sibling of the current element with the given Name.
        /// <summary>Advances the XmlReader to the next sibling element with the specified qualified name.</summary>
        /// <returns>true if a matching sibling element is found; otherwise false. If a matching sibling element is not found, the XmlReader is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is XmlNodeType.EndElement) of the parent element.</returns>
        /// <param name="name">The qualified name of the sibling element you wish to move to.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
        public virtual bool ReadToNextSibling(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
            }

            // atomize name
            name = NameTable.Add(name);

            // find the next sibling
            XmlNodeType nt;
            do
            {
                if (!SkipSubtree())
                {
                    break;
                }
                nt = NodeType;
                if (nt == XmlNodeType.Element && Ref.Equal(name, Name))
                {
                    return true;
                }
            } while (nt != XmlNodeType.EndElement && !EOF);
            return false;
        }

        // Reads to the next sibling of the current element with the given LocalName and NamespaceURI.
        /// <summary>Advances the XmlReader to the next sibling element with the specified local name and namespace URI.</summary>
        /// <returns>true if a matching sibling element is found; otherwise, false. If a matching sibling element is not found, the XmlReader is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is XmlNodeType.EndElement) of the parent element.</returns>
        /// <param name="localName">The local name of the sibling element you wish to move to.</param>
        /// <param name="namespaceURI">The namespace URI of the sibling element you wish to move to.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.ArgumentNullException">Both parameter values are null.</exception>
        public virtual bool ReadToNextSibling(string localName, string namespaceURI)
        {
            if (localName == null || localName.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException(nameof(namespaceURI));
            }

            // atomize local name and namespace
            localName = NameTable.Add(localName);
            namespaceURI = NameTable.Add(namespaceURI);

            // find the next sibling
            XmlNodeType nt;
            do
            {
                if (!SkipSubtree())
                {
                    break;
                }
                nt = NodeType;
                if (nt == XmlNodeType.Element && Ref.Equal(localName, LocalName) && Ref.Equal(namespaceURI, NamespaceURI))
                {
                    return true;
                }
            } while (nt != XmlNodeType.EndElement && !EOF);
            return false;
        }

        // Returns true if the given argument is a valid Name.
        /// <summary>Returns a value indicating whether the string argument is a valid XML name.</summary>
        /// <returns>true if the name is valid; otherwise, false.</returns>
        /// <param name="str">The name to validate.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="str" /> value is null.</exception>
        public static bool IsName(string str)
        {
            if (str == null)
            {
                throw new NullReferenceException();
            }
            return ValidateNames.IsNameNoNamespaces(str);
        }

        // Returns true if the given argument is a valid NmToken.
        /// <summary>Returns a value indicating whether or not the string argument is a valid XML name token.</summary>
        /// <returns>true if it is a valid name token; otherwise false.</returns>
        /// <param name="str">The name token to validate.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="str" /> value is null.</exception>
        public static bool IsNameToken(string str)
        {
            if (str == null)
            {
                throw new NullReferenceException();
            }
            return ValidateNames.IsNmtokenNoNamespaces(str);
        }

        // Returns the inner content (including markup) of an element or attribute as a string.
        /// <summary>When overridden in a derived class, reads all the content, including markup, as a string.</summary>
        /// <returns>All the XML content, including markup, in the current node. If the current node has no children, an empty string is returned.If the current node is neither an element nor attribute, an empty string is returned.</returns>
        /// <exception cref="T:System.Xml.XmlException">The XML was not well-formed, or an error occurred while parsing the XML.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string ReadInnerXml()
        {
            if (ReadState != ReadState.Interactive)
            {
                return string.Empty;
            }
            if ((this.NodeType != XmlNodeType.Attribute) && (this.NodeType != XmlNodeType.Element))
            {
                Read();
                return string.Empty;
            }

            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            XmlWriter xtw = CreateWriterForInnerOuterXml(sw);

            try
            {
                if (this.NodeType == XmlNodeType.Attribute)
                {
                    WriteAttributeValue(xtw);
                }
                if (this.NodeType == XmlNodeType.Element)
                {
                    this.WriteNode(xtw, false);
                }
            }
            finally
            {
                xtw.Dispose();
            }
            return sw.ToString();
        }

        // Writes the content (inner XML) of the current node into the provided XmlWriter.
        private void WriteNode(XmlWriter xtw, bool defattr)
        {
            int d = this.NodeType == XmlNodeType.None ? -1 : this.Depth;
            while (this.Read() && (d < this.Depth))
            {
                switch (this.NodeType)
                {
                    case XmlNodeType.Element:
                        xtw.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
                        xtw.WriteAttributes(this, defattr);
                        if (this.IsEmptyElement)
                        {
                            xtw.WriteEndElement();
                        }
                        break;
                    case XmlNodeType.Text:
                        xtw.WriteString(this.Value);
                        break;
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        xtw.WriteWhitespace(this.Value);
                        break;
                    case XmlNodeType.CDATA:
                        xtw.WriteCData(this.Value);
                        break;
                    case XmlNodeType.EntityReference:
                        xtw.WriteEntityRef(this.Name);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        xtw.WriteProcessingInstruction(this.Name, this.Value);
                        break;
                    case XmlNodeType.DocumentType:
                        xtw.WriteDocType(this.Name, this.GetAttribute("PUBLIC"), this.GetAttribute("SYSTEM"), this.Value);
                        break;
                    case XmlNodeType.Comment:
                        xtw.WriteComment(this.Value);
                        break;
                    case XmlNodeType.EndElement:
                        xtw.WriteFullEndElement();
                        break;
                }
            }
            if (d == this.Depth && this.NodeType == XmlNodeType.EndElement)
            {
                Read();
            }
        }

        // Writes the attribute into the provided XmlWriter.
        private void WriteAttributeValue(XmlWriter xtw)
        {
            string attrName = this.Name;
            while (ReadAttributeValue())
            {
                if (this.NodeType == XmlNodeType.EntityReference)
                {
                    xtw.WriteEntityRef(this.Name);
                }
                else
                {
                    xtw.WriteString(this.Value);
                }
            }
            this.MoveToAttribute(attrName);
        }

        // Returns the current element and its descendants or an attribute as a string.
        /// <summary>When overridden in a derived class, reads the content, including markup, representing this node and all its children.</summary>
        /// <returns>If the reader is positioned on an element or an attribute node, this method returns all the XML content, including markup, of the current node and all its children; otherwise, it returns an empty string.</returns>
        /// <exception cref="T:System.Xml.XmlException">The XML was not well-formed, or an error occurred while parsing the XML.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string ReadOuterXml()
        {
            if (ReadState != ReadState.Interactive)
            {
                return string.Empty;
            }
            if ((this.NodeType != XmlNodeType.Attribute) && (this.NodeType != XmlNodeType.Element))
            {
                Read();
                return string.Empty;
            }

            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            XmlWriter xtw = CreateWriterForInnerOuterXml(sw);

            try
            {
                if (this.NodeType == XmlNodeType.Attribute)
                {
                    xtw.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
                    WriteAttributeValue(xtw);
                    xtw.WriteEndAttribute();
                }
                else
                {
                    xtw.WriteNode(this, false);
                }
            }
            finally
            {
                xtw.Dispose();
            }
            return sw.ToString();
        }

        private XmlWriter CreateWriterForInnerOuterXml(StringWriter sw)
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
            writerSettings.CheckCharacters = false;
            writerSettings.NewLineHandling = NewLineHandling.None;
            XmlWriter w = XmlWriter.Create(sw, writerSettings);
            return w;
        }


        // Returns an XmlReader that will read only the current element and its descendants and then go to EOF state.
        /// <summary>Returns a new XmlReader instance that can be used to read the current node, and all its descendants.</summary>
        /// <returns>A new XML reader instance set to <see cref="F:System.Xml.ReadState.Initial" />. Calling the <see cref="M:System.Xml.XmlReader.Read" /> method positions the new reader on the node that was current before the call to the <see cref="M:System.Xml.XmlReader.ReadSubtree" /> method.</returns>
        /// <exception cref="T:System.InvalidOperationException">The XML reader isn't positioned on an element when this method is called.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual XmlReader ReadSubtree()
        {
            if (NodeType != XmlNodeType.Element)
            {
                throw new InvalidOperationException(SR.Xml_ReadSubtreeNotOnElement);
            }
            return new XmlSubtreeReader(this);
        }

        // Returns true when the current node has any attributes.
        /// <summary>Gets a value indicating whether the current node has any attributes.</summary>
        /// <returns>true if the current node has attributes; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual bool HasAttributes
        {
            get
            {
                return AttributeCount > 0;
            }
        }

        //
        // IDisposable interface
        //
        /// <summary>Releases all resources used by the current instance of the <see cref="T:System.Xml.XmlReader" /> class.</summary>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Xml.XmlReader" /> and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        protected virtual void Dispose(bool disposing)
        {
        }

        //
        // Internal methods
        //
        // Validation support

        static internal bool IsTextualNode(XmlNodeType nodeType)
        {
#if DEBUG
            // This code verifies IsTextualNodeBitmap mapping of XmlNodeType to a bool specifying
            // whether the node is 'textual' = Text, CDATA, Whitespace or SignificantWhitespace.
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.None)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Element)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Attribute)));
            Debug.Assert(0 != (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Text)));
            Debug.Assert(0 != (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.CDATA)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.EntityReference)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Entity)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.ProcessingInstruction)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Comment)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Document)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.DocumentType)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.DocumentFragment)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Notation)));
            Debug.Assert(0 != (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.Whitespace)));
            Debug.Assert(0 != (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.SignificantWhitespace)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.EndElement)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.EndEntity)));
            Debug.Assert(0 == (s_isTextualNodeBitmap & (1 << (int)XmlNodeType.XmlDeclaration)));
#endif
            return 0 != (s_isTextualNodeBitmap & (1 << (int)nodeType));
        }

        static internal bool CanReadContentAs(XmlNodeType nodeType)
        {
#if DEBUG
            // This code verifies IsTextualNodeBitmap mapping of XmlNodeType to a bool specifying
            // whether ReadContentAsXxx calls are allowed on his node type
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.None)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Element)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Attribute)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Text)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.CDATA)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.EntityReference)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Entity)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.ProcessingInstruction)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Comment)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Document)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.DocumentType)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.DocumentFragment)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Notation)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.Whitespace)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.SignificantWhitespace)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.EndElement)));
            Debug.Assert(0 != (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.EndEntity)));
            Debug.Assert(0 == (s_canReadContentAsBitmap & (1 << (int)XmlNodeType.XmlDeclaration)));
#endif
            return 0 != (s_canReadContentAsBitmap & (1 << (int)nodeType));
        }

        static internal bool HasValueInternal(XmlNodeType nodeType)
        {
#if DEBUG
            // This code verifies HasValueBitmap mapping of XmlNodeType to a bool specifying
            // whether the node can have a non-empty Value
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.None)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.Element)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.Attribute)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.Text)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.CDATA)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.EntityReference)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.Entity)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.ProcessingInstruction)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.Comment)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.Document)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.DocumentType)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.DocumentFragment)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.Notation)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.Whitespace)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.SignificantWhitespace)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.EndElement)));
            Debug.Assert(0 == (s_hasValueBitmap & (1 << (int)XmlNodeType.EndEntity)));
            Debug.Assert(0 != (s_hasValueBitmap & (1 << (int)XmlNodeType.XmlDeclaration)));
#endif
            return 0 != (s_hasValueBitmap & (1 << (int)nodeType));
        }

        //
        // Private methods
        //
        //SkipSubTree is called whenever validation of the skipped subtree is required on a reader with XsdValidation
        private bool SkipSubtree()
        {
            MoveToElement();
            if (NodeType == XmlNodeType.Element && !IsEmptyElement)
            {
                int depth = Depth;

                while (Read() && depth < Depth)
                {
                    // Nothing, just read on
                }

                // consume end tag
                if (NodeType == XmlNodeType.EndElement)
                    return Read();
            }
            else
            {
                return Read();
            }

            return false;
        }

        internal void CheckElement(string localName, string namespaceURI)
        {
            if (localName == null || localName.Length == 0)
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException(nameof(namespaceURI));
            }
            if (NodeType != XmlNodeType.Element)
            {
                throw new XmlException(SR.Xml_InvalidNodeType, this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if (LocalName != localName || NamespaceURI != namespaceURI)
            {
                throw new XmlException(SR.Xml_ElementNotFoundNs, new string[2] { localName, namespaceURI }, this as IXmlLineInfo);
            }
        }

        internal Exception CreateReadContentAsException(string methodName)
        {
            return CreateReadContentAsException(methodName, NodeType, this as IXmlLineInfo);
        }

        internal Exception CreateReadElementContentAsException(string methodName)
        {
            return CreateReadElementContentAsException(methodName, NodeType, this as IXmlLineInfo);
        }

        internal bool CanReadContentAs()
        {
            return CanReadContentAs(this.NodeType);
        }

        static internal Exception CreateReadContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
        {
            return new InvalidOperationException(AddLineInfo(SR.Format(SR.Xml_InvalidReadContentAs, methodName, nodeType.ToString()), lineInfo));
        }

        static internal Exception CreateReadElementContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
        {
            return new InvalidOperationException(AddLineInfo(SR.Format(SR.Xml_InvalidReadElementContentAs, methodName, nodeType.ToString()), lineInfo));
        }

        private static string AddLineInfo(string message, IXmlLineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                string[] lineArgs = new string[2];
                lineArgs[0] = lineInfo.LineNumber.ToString(CultureInfo.InvariantCulture);
                lineArgs[1] = lineInfo.LinePosition.ToString(CultureInfo.InvariantCulture);
                message += " " + SR.Format(SR.Xml_ErrorPosition, lineArgs);
            }
            return message;
        }

        internal string InternalReadContentAsString()
        {
            string value = string.Empty;
            BufferBuilder sb = null;
            do
            {
                switch (this.NodeType)
                {
                    case XmlNodeType.Attribute:
                        return this.Value;
                    case XmlNodeType.Text:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.CDATA:
                        // merge text content
                        if (value.Length == 0)
                        {
                            value = this.Value;
                        }
                        else
                        {
                            if (sb == null)
                            {
                                sb = new BufferBuilder();
                                sb.Append(value);
                            }
                            sb.Append(this.Value);
                        }
                        break;
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.Comment:
                    case XmlNodeType.EndEntity:
                        // skip comments, pis and end entity nodes
                        break;
                    case XmlNodeType.EntityReference:
                        if (this.CanResolveEntity)
                        {
                            this.ResolveEntity();
                            break;
                        }
                        goto default;
                    case XmlNodeType.EndElement:
                    default:
                        goto ReturnContent;
                }
            } while ((this.AttributeCount != 0) ? this.ReadAttributeValue() : this.Read());

        ReturnContent:
            return (sb == null) ? value : sb.ToString();
        }

        private bool SetupReadElementContentAsXxx(string methodName)
        {
            if (this.NodeType != XmlNodeType.Element)
            {
                throw CreateReadElementContentAsException(methodName);
            }

            bool isEmptyElement = this.IsEmptyElement;

            // move to content or beyond the empty element
            this.Read();

            if (isEmptyElement)
            {
                return false;
            }

            XmlNodeType nodeType = this.NodeType;
            if (nodeType == XmlNodeType.EndElement)
            {
                this.Read();
                return false;
            }
            else if (nodeType == XmlNodeType.Element)
            {
                throw new XmlException(SR.Xml_MixedReadElementContentAs, string.Empty, this as IXmlLineInfo);
            }
            return true;
        }

        private void FinishReadElementContentAsXxx()
        {
            if (this.NodeType != XmlNodeType.EndElement)
            {
                throw new XmlException(SR.Xml_InvalidNodeType, this.NodeType.ToString());
            }
            this.Read();
        }

        internal bool IsDefaultInternal
        {
            get
            {
                return this.IsDefault;
            }
        }



        internal static ConformanceLevel GetV1ConformanceLevel(XmlReader reader)
        {
            XmlTextReaderImpl tri = GetXmlTextReaderImpl(reader);
            return tri != null ? tri.V1ComformanceLevel : ConformanceLevel.Document;
        }

        private static XmlTextReaderImpl GetXmlTextReaderImpl(XmlReader reader)
        {
            XmlTextReaderImpl tri = reader as XmlTextReaderImpl;
            if (tri != null)
            {
                return tri;
            }
            return null;
        }

        //
        // Static methods for creating readers
        //

        // Creates an XmlReader for parsing XML from the given Uri.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance with specified URI.</summary>
        /// <returns>An object that is used to read the XML data in the stream.</returns>
        /// <param name="inputUri">The URI for the file that contains the XML data. The <see cref="T:System.Xml.XmlUrlResolver" /> class is used to convert the path to a canonical data representation.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="inputUri" /> value is null.</exception>
        /// <exception cref="T:System.Security.SecurityException">The <see cref="T:System.Xml.XmlReader" /> does not have sufficient permissions to access the location of the XML data.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The file identified by the URI does not exist.</exception>
        /// <exception cref="T:System.UriFormatException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.The URI format is not correct.</exception>
        public static XmlReader Create(string inputUri)
        {
            return XmlReader.Create(inputUri, (XmlReaderSettings)null, (XmlParserContext)null);
        }

        // Creates an XmlReader according to the settings for parsing XML from the given Uri.
        public static XmlReader Create(string inputUri, XmlReaderSettings settings)
        {
            return XmlReader.Create(inputUri, settings, (XmlParserContext)null);
        }


        private static XmlReader Create(String inputUri, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return settings.CreateReader(inputUri, inputContext);
        }

        // Creates an XmlReader according for parsing XML from the given stream.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance using the specified stream with default settings.</summary>
        /// <returns>An object that is used to read the XML data in the stream.</returns>
        /// <param name="input">The stream that contains the XML data.The <see cref="T:System.Xml.XmlReader" /> scans the first bytes of the stream looking for a byte order mark or other sign of encoding. When encoding is determined, the encoding is used to continue reading the stream, and processing continues parsing the input as a stream of (Unicode) characters.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is null.</exception>
        /// <exception cref="T:System.Security.SecurityException">The <see cref="T:System.Xml.XmlReader" /> does not have sufficient permissions to access the location of the XML data.</exception>
        public static XmlReader Create(Stream input)
        {
            return Create(input, (XmlReaderSettings)null, (string)string.Empty);
        }

        // Creates an XmlReader according to the settings for parsing XML from the given stream.
        public static XmlReader Create(Stream input, XmlReaderSettings settings)
        {
            return Create(input, settings, string.Empty);
        }

        // Creates an XmlReader according to the settings and base Uri for parsing XML from the given stream.
        private static XmlReader Create(Stream input, XmlReaderSettings settings, String baseUri)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return settings.CreateReader(input, null, (string)baseUri, null);
        }

        // Creates an XmlReader according to the settings and parser context for parsing XML from the given stream.
        public static XmlReader Create(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return settings.CreateReader(input, null, (string)string.Empty, inputContext);
        }

        // Creates an XmlReader according for parsing XML from the given TextReader.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified text reader.</summary>
        /// <returns>An object that is used to read the XML data in the stream.</returns>
        /// <param name="input">The text reader from which to read the XML data. A text reader returns a stream of Unicode characters, so the encoding specified in the XML declaration is not used by the XML reader to decode the data stream.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is null.</exception>
        public static XmlReader Create(TextReader input)
        {
            return Create(input, (XmlReaderSettings)null, (string)string.Empty);
        }

        // Creates an XmlReader according to the settings for parsing XML from the given TextReader.
        public static XmlReader Create(TextReader input, XmlReaderSettings settings)
        {
            return Create(input, settings, string.Empty);
        }

        // Creates an XmlReader according to the settings and baseUri for parsing XML from the given TextReader.
        private static XmlReader Create(TextReader input, XmlReaderSettings settings, String baseUri)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return settings.CreateReader(input, baseUri, null);
        }

        // Creates an XmlReader according to the settings and parser context for parsing XML from the given TextReader.
        public static XmlReader Create(TextReader input, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return settings.CreateReader(input, string.Empty, inputContext);
        }

        // Creates an XmlReader according to the settings wrapped over the given reader.
        public static XmlReader Create(XmlReader reader, XmlReaderSettings settings)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return settings.CreateReader(reader);
        }


        internal static int CalcBufferSize(Stream input)
        {
            // determine the size of byte buffer
            int bufferSize = DefaultBufferSize;
            if (input.CanSeek)
            {
                long len = input.Length;
                if (len < bufferSize)
                {
                    bufferSize = checked((int)len);
                }
                else if (len > MaxStreamLengthForDefaultBufferSize)
                {
                    bufferSize = BiggerBufferSize;
                }
            }

            // return the byte buffer size
            return bufferSize;
        }
    }
}

