// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Versioning;

namespace System.Xml
{
    // Specifies the state of the XmlWriter.
    /// <summary>Specifies the state of the <see cref="T:System.Xml.XmlWriter" />.</summary>
    public enum WriteState
    {
        // Nothing has been written yet.
        /// <summary>Indicates that a Write method has not yet been called.</summary>
        Start,

        // Writing the prolog.
        /// <summary>Indicates that the prolog is being written.</summary>
        Prolog,

        // Writing a the start tag for an element.
        /// <summary>Indicates that an element start tag is being written.</summary>
        Element,

        // Writing an attribute value.
        /// <summary>Indicates that an attribute value is being written.</summary>
        Attribute,

        // Writing element content.
        /// <summary>Indicates that element content is being written.</summary>
        Content,

        // XmlWriter is closed; Close has been called.
        /// <summary>Indicates that the <see cref="M:System.Xml.XmlWriter.Close" /> method has been called.</summary>
        Closed,

        // Writer is in error state.
        /// <summary>An exception has been thrown, which has left the <see cref="T:System.Xml.XmlWriter" /> in an invalid state. You can call the <see cref="M:System.Xml.XmlWriter.Close" /> method to put the <see cref="T:System.Xml.XmlWriter" /> in the <see cref="F:System.Xml.WriteState.Closed" /> state. Any other <see cref="T:System.Xml.XmlWriter" /> method calls results in an <see cref="T:System.InvalidOperationException" />.</summary>
        Error,
    };

    // Represents a writer that provides fast non-cached forward-only way of generating XML streams containing XML documents 
    // that conform to the W3C Extensible Markup Language (XML) 1.0 specification and the Namespaces in XML specification.
    /// <summary>Represents a writer that provides a fast, non-cached, forward-only way to generate streams or files that contain XML data.</summary>
    public abstract partial class XmlWriter : IDisposable
    {
        // Helper buffer for WriteNode(XmlReader, bool)
        private char[] _writeNodeBuffer;

        // Constants
        private const int WriteNodeBufferSize = 1024;

        // Returns the settings describing the features of the the writer. Returns null for V1 XmlWriters (XmlTextWriter).
        /// <summary>Gets the <see cref="T:System.Xml.XmlWriterSettings" /> object used to create this <see cref="T:System.Xml.XmlWriter" /> instance.</summary>
        /// <returns>The <see cref="T:System.Xml.XmlWriterSettings" /> object used to create this writer instance. If this writer was not created using the <see cref="Overload:System.Xml.XmlWriter.Create" /> method, this property returns null.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual XmlWriterSettings Settings
        {
            get
            {
                return null;
            }
        }

        // Write methods
        // Writes out the XML declaration with the version "1.0".

        /// <summary>When overridden in a derived class, writes the XML declaration with the version "1.0".</summary>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteStartDocument();

        //Writes out the XML declaration with the version "1.0" and the speficied standalone attribute.

        /// <summary>When overridden in a derived class, writes the XML declaration with the version "1.0" and the standalone attribute.</summary>
        /// <param name="standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no".</param>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteStartDocument(bool standalone);

        //Closes any open elements or attributes and puts the writer back in the Start state.

        /// <summary>When overridden in a derived class, closes any open elements or attributes and puts the writer back in the Start state.</summary>
        /// <exception cref="T:System.ArgumentException">The XML document is invalid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteEndDocument();

        // Writes out the DOCTYPE declaration with the specified name and optional attributes.

        /// <summary>When overridden in a derived class, writes the DOCTYPE declaration with the specified name and optional attributes.</summary>
        /// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
        /// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" where <paramref name="pubid" /> and <paramref name="sysid" /> are replaced with the value of the given arguments.</param>
        /// <param name="sysid">If <paramref name="pubid" /> is null and <paramref name="sysid" /> is non-null it writes SYSTEM "sysid" where <paramref name="sysid" /> is replaced with the value of this argument.</param>
        /// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument.</param>
        /// <exception cref="T:System.InvalidOperationException">This method was called outside the prolog (after the root element). </exception>
        /// <exception cref="T:System.ArgumentException">The value for <paramref name="name" /> would result in invalid XML.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

        // Writes out the specified start tag and associates it with the given namespace.
        /// <summary>When overridden in a derived class, writes the specified start tag and associates it with the given namespace.</summary>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI to associate with the element. If this namespace is already in scope and has an associated prefix, the writer automatically writes that prefix also.</param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteStartElement(string localName, string ns)
        {
            WriteStartElement(null, localName, ns);
        }

        // Writes out the specified start tag and associates it with the given namespace and prefix.

        /// <summary>When overridden in a derived class, writes the specified start tag and associates it with the given namespace and prefix.</summary>
        /// <param name="prefix">The namespace prefix of the element.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI to associate with the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteStartElement(string prefix, string localName, string ns);

        // Writes out a start tag with the specified local name with no namespace.
        /// <summary>When overridden in a derived class, writes out a start tag with the specified local name.</summary>
        /// <param name="localName">The local name of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteStartElement(string localName)
        {
            WriteStartElement(null, localName, (string)null);
        }

        // Closes one element and pops the corresponding namespace scope.

        /// <summary>When overridden in a derived class, closes one element and pops the corresponding namespace scope.</summary>
        /// <exception cref="T:System.InvalidOperationException">This results in an invalid XML document.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteEndElement();

        // Closes one element and pops the corresponding namespace scope. Writes out a full end element tag, e.g. </element>.

        /// <summary>When overridden in a derived class, closes one element and pops the corresponding namespace scope.</summary>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteFullEndElement();

        // Writes out the attribute with the specified LocalName, value, and NamespaceURI.
        /// <summary>When overridden in a derived class, writes an attribute with the specified local name, namespace URI, and value.</summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI to associate with the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">The state of writer is not WriteState.Element or writer is closed. </exception>
        /// <exception cref="T:System.ArgumentException">The xml:space or xml:lang attribute value is invalid. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteAttributeString(string localName, string ns, string value)
        {
            WriteStartAttribute(null, localName, ns);
            WriteString(value);
            WriteEndAttribute();
        }

        // Writes out the attribute with the specified LocalName and value.
        /// <summary>When overridden in a derived class, writes out the attribute with the specified local name and value.</summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">The state of writer is not WriteState.Element or writer is closed. </exception>
        /// <exception cref="T:System.ArgumentException">The xml:space or xml:lang attribute value is invalid. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteAttributeString(string localName, string value)
        {
            WriteStartAttribute(null, localName, (string)null);
            WriteString(value);
            WriteEndAttribute();
        }

        // Writes out the attribute with the specified prefix, LocalName, NamespaceURI and value.
        /// <summary>When overridden in a derived class, writes out the attribute with the specified prefix, local name, namespace URI, and value.</summary>
        /// <param name="prefix">The namespace prefix of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">The state of writer is not WriteState.Element or writer is closed. </exception>
        /// <exception cref="T:System.ArgumentException">The xml:space or xml:lang attribute value is invalid. </exception>
        /// <exception cref="T:System.Xml.XmlException">The <paramref name="localName" /> or <paramref name="ns" /> is null. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteAttributeString(string prefix, string localName, string ns, string value)
        {
            WriteStartAttribute(prefix, localName, ns);
            WriteString(value);
            WriteEndAttribute();
        }

        // Writes the start of an attribute.
        /// <summary>Writes the start of an attribute with the specified local name and namespace URI.</summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteStartAttribute(string localName, string ns)
        {
            WriteStartAttribute(null, localName, ns);
        }

        // Writes the start of an attribute.

        /// <summary>When overridden in a derived class, writes the start of an attribute with the specified prefix, local name, and namespace URI.</summary>
        /// <param name="prefix">The namespace prefix of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI for the attribute.</param>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteStartAttribute(string prefix, string localName, string ns);

        // Writes the start of an attribute.
        /// <summary>Writes the start of an attribute with the specified local name.</summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteStartAttribute(string localName)
        {
            WriteStartAttribute(null, localName, (string)null);
        }

        // Closes the attribute opened by WriteStartAttribute call.

        /// <summary>When overridden in a derived class, closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)" /> call.</summary>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteEndAttribute();

        // Writes out a <![CDATA[...]]>; block containing the specified text.

        /// <summary>When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt; block containing the specified text.</summary>
        /// <param name="text">The text to place inside the CDATA block.</param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteCData(string text);

        // Writes out a comment <!--...-->; containing the specified text.

        /// <summary>When overridden in a derived class, writes out a comment &lt;!--...--&gt; containing the specified text.</summary>
        /// <param name="text">Text to place inside the comment.</param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well-formed XML document.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteComment(string text);

        // Writes out a processing instruction with a space between the name and text as follows: <?name text?>

        /// <summary>When overridden in a derived class, writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.</summary>
        /// <param name="name">The name of the processing instruction.</param>
        /// <param name="text">The text to include in the processing instruction.</param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.<paramref name="name" /> is either null or String.Empty.This method is being used to create an XML declaration after <see cref="M:System.Xml.XmlWriter.WriteStartDocument" /> has already been called. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteProcessingInstruction(string name, string text);

        // Writes out an entity reference as follows: "&"+name+";".

        /// <summary>When overridden in a derived class, writes out an entity reference as &amp;name;.</summary>
        /// <param name="name">The name of the entity reference.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is either null or String.Empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteEntityRef(string name);

        // Forces the generation of a character entity for the specified Unicode character value.

        /// <summary>When overridden in a derived class, forces the generation of a character entity for the specified Unicode character value.</summary>
        /// <param name="ch">The Unicode character for which to generate a character entity.</param>
        /// <exception cref="T:System.ArgumentException">The character is in the surrogate pair character range, 0xd800 - 0xdfff.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteCharEntity(char ch);

        // Writes out the given whitespace.

        /// <summary>When overridden in a derived class, writes out the given white space.</summary>
        /// <param name="ws">The string of white space characters.</param>
        /// <exception cref="T:System.ArgumentException">The string contains non-white space characters.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteWhitespace(string ws);

        // Writes out the specified text content.

        /// <summary>When overridden in a derived class, writes the given text content.</summary>
        /// <param name="text">The text to write.</param>
        /// <exception cref="T:System.ArgumentException">The text string contains an invalid surrogate pair.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteString(string text);

        // Write out the given surrogate pair as an entity reference.

        /// <summary>When overridden in a derived class, generates and writes the surrogate character entity for the surrogate character pair.</summary>
        /// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF.</param>
        /// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF.</param>
        /// <exception cref="T:System.ArgumentException">An invalid surrogate character pair was passed.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

        // Writes out the specified text content.

        /// <summary>When overridden in a derived class, writes text one buffer at a time.</summary>
        /// <param name="buffer">Character array containing the text to write.</param>
        /// <param name="index">The position in the buffer indicating the start of the text to write.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> or <paramref name="count" /> is less than zero.-or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />; the call results in surrogate pair characters being split or an invalid surrogate pair being written.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="buffer" /> parameter value is not valid.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteChars(char[] buffer, int index, int count);

        // Writes raw markup from the given character buffer.

        /// <summary>When overridden in a derived class, writes raw markup manually from a character buffer.</summary>
        /// <param name="buffer">Character array containing the text to write.</param>
        /// <param name="index">The position within the buffer indicating the start of the text to write.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteRaw(char[] buffer, int index, int count);

        // Writes raw markup from the given string.

        /// <summary>When overridden in a derived class, writes raw markup manually from a string.</summary>
        /// <param name="data">String containing the text to write.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="data" /> is either null or String.Empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteRaw(string data);

        // Encodes the specified binary bytes as base64 and writes out the resulting text.

        /// <summary>When overridden in a derived class, encodes the specified binary bytes as Base64 and writes out the resulting text.</summary>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void WriteBase64(byte[] buffer, int index, int count);

        // Encodes the specified binary bytes as binhex and writes out the resulting text.
        /// <summary>When overridden in a derived class, encodes the specified binary bytes as BinHex and writes out the resulting text.</summary>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed or in error state.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteBinHex(byte[] buffer, int index, int count)
        {
            BinHexEncoder.Encode(buffer, index, count, this);
        }

        // Returns the state of the XmlWriter.
        /// <summary>When overridden in a derived class, gets the state of the writer.</summary>
        /// <returns>One of the <see cref="T:System.Xml.WriteState" /> values.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract WriteState WriteState { get; }

        // Flushes data that is in the internal buffers into the underlying streams/TextReader and flushes the stream/TextReader.
        /// <summary>When overridden in a derived class, flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.</summary>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract void Flush();

        // Returns the closest prefix defined in the current namespace scope for the specified namespace URI.
        /// <summary>When overridden in a derived class, returns the closest prefix defined in the current namespace scope for the namespace URI.</summary>
        /// <returns>The matching prefix or null if no matching namespace URI is found in the current scope.</returns>
        /// <param name="ns">The namespace URI whose prefix you want to find.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="ns" /> is either null or String.Empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public abstract string LookupPrefix(string ns);

        // Gets an XmlSpace representing the current xml:space scope.
        /// <summary>When overridden in a derived class, gets an <see cref="T:System.Xml.XmlSpace" /> representing the current xml:space scope.</summary>
        /// <returns>An XmlSpace representing the current xml:space scope.Value Meaning NoneThis is the default if no xml:space scope exists.DefaultThe current scope is xml:space="default".PreserveThe current scope is xml:space="preserve".</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual XmlSpace XmlSpace
        {
            get
            {
                return XmlSpace.Default;
            }
        }

        // Gets the current xml:lang scope.
        /// <summary>When overridden in a derived class, gets the current xml:lang scope.</summary>
        /// <returns>The current xml:lang scope.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual string XmlLang
        {
            get
            {
                return string.Empty;
            }
        }

        // Scalar Value Methods

        // Writes out the specified name, ensuring it is a valid NmToken according to the XML specification 
        // (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// <summary>When overridden in a derived class, writes out the specified name, ensuring it is a valid NmToken according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
        /// <param name="name">The name to write.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is not a valid NmToken; or <paramref name="name" /> is either null or String.Empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteNmToken(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentException(SR.Xml_EmptyName);
            }
            WriteString(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
        }

        // Writes out the specified name, ensuring it is a valid Name according to the XML specification
        // (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// <summary>When overridden in a derived class, writes out the specified name, ensuring it is a valid name according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
        /// <param name="name">The name to write.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="name" /> is not a valid XML name; or <paramref name="name" /> is either null or String.Empty.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteName(string name)
        {
            WriteString(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
        }

        // Writes out the specified namespace-qualified name by looking up the prefix that is in scope for the given namespace.
        /// <summary>When overridden in a derived class, writes out the namespace-qualified name. This method looks up the prefix that is in scope for the given namespace.</summary>
        /// <param name="localName">The local name to write.</param>
        /// <param name="ns">The namespace URI for the name.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="localName" /> is either null or String.Empty.<paramref name="localName" /> is not a valid name. </exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteQualifiedName(string localName, string ns)
        {
            if (ns != null && ns.Length > 0)
            {
                string prefix = LookupPrefix(ns);
                if (prefix == null)
                {
                    throw new ArgumentException(SR.Format(SR.Xml_UndefNamespace, ns));
                }
                WriteString(prefix);
                WriteString(":");
            }
            WriteString(localName);
        }

        // Writes out the specified value.
        /// <summary>Writes the object value.</summary>
        /// <param name="value">The object value to write.Note   With the release of the .NET Framework 3.5, this method accepts <see cref="T:System.DateTimeOffset" /> as a parameter.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The writer is closed or in error state.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            WriteString(XmlUntypedStringConverter.Instance.ToString(value, null));
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.String" /> value.</summary>
        /// <param name="value">The <see cref="T:System.String" /> value to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(string value)
        {
            if (value == null)
            {
                return;
            }
            WriteString(value);
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.Boolean" /> value.</summary>
        /// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(bool value)
        {
            WriteString(XmlConvert.ToString(value));
        }

        // Writes out the specified value.
        internal virtual void WriteValue(DateTime value)
        {
            WriteString(XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.DateTimeOffset" /> value.</summary>
        /// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(DateTimeOffset value)
        {
            // Under Win8P, WriteValue(DateTime) will invoke this overload, but custom writers
            // might not have implemented it. This base implementation should call WriteValue(DateTime).
            // The following conversion results in the same string as calling ToString with DateTimeOffset.
            if (value.Offset != TimeSpan.Zero)
            {
                WriteValue(value.LocalDateTime);
            }
            else
            {
                WriteValue(value.UtcDateTime);
            }
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.Double" /> value.</summary>
        /// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(double value)
        {
            WriteString(XmlConvert.ToString(value));
        }

        // Writes out the specified value.
        /// <summary>Writes a single-precision floating-point number.</summary>
        /// <param name="value">The single-precision floating-point number to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(float value)
        {
            WriteString(XmlConvert.ToString(value));
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.Decimal" /> value.</summary>
        /// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(decimal value)
        {
            WriteString(XmlConvert.ToString(value));
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.Int32" /> value.</summary>
        /// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(int value)
        {
            WriteString(XmlConvert.ToString(value));
        }

        // Writes out the specified value.
        /// <summary>Writes a <see cref="T:System.Int64" /> value.</summary>
        /// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
        /// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public virtual void WriteValue(long value)
        {
            WriteString(XmlConvert.ToString(value));
        }

        // XmlReader Helper Methods

        // Writes out all the attributes found at the current position in the specified XmlReader.
        public virtual void WriteAttributes(XmlReader reader, bool defattr)
        {
            if (null == reader)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.XmlDeclaration)
            {
                if (reader.MoveToFirstAttribute())
                {
                    WriteAttributes(reader, defattr);
                    reader.MoveToElement();
                }
            }
            else if (reader.NodeType != XmlNodeType.Attribute)
            {
                throw new XmlException(SR.Xml_InvalidPosition, string.Empty);
            }
            else
            {
                do
                {
                    // we need to check both XmlReader.IsDefault and XmlReader.SchemaInfo.IsDefault. 
                    // If either of these is true and defattr=false, we should not write the attribute out
                    if (defattr || !reader.IsDefaultInternal)
                    {
                        WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        while (reader.ReadAttributeValue())
                        {
                            if (reader.NodeType == XmlNodeType.EntityReference)
                            {
                                WriteEntityRef(reader.Name);
                            }
                            else
                            {
                                WriteString(reader.Value);
                            }
                        }
                        WriteEndAttribute();
                    }
                }
                while (reader.MoveToNextAttribute());
            }
        }

        // Copies the current node from the given reader to the writer (including child nodes), and if called on an element moves the XmlReader 
        // to the corresponding end element.
        public virtual void WriteNode(XmlReader reader, bool defattr)
        {
            if (null == reader)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            bool canReadChunk = reader.CanReadValueChunk;
            int d = reader.NodeType == XmlNodeType.None ? -1 : reader.Depth;
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        WriteAttributes(reader, defattr);
                        if (reader.IsEmptyElement)
                        {
                            WriteEndElement();
                            break;
                        }
                        break;
                    case XmlNodeType.Text:
                        if (canReadChunk)
                        {
                            if (_writeNodeBuffer == null)
                            {
                                _writeNodeBuffer = new char[WriteNodeBufferSize];
                            }
                            int read;
                            while ((read = reader.ReadValueChunk(_writeNodeBuffer, 0, WriteNodeBufferSize)) > 0)
                            {
                                this.WriteChars(_writeNodeBuffer, 0, read);
                            }
                        }
                        else
                        {
                            WriteString(reader.Value);
                        }
                        break;
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:

                        WriteWhitespace(reader.Value);

                        break;
                    case XmlNodeType.CDATA:
                        WriteCData(reader.Value);
                        break;
                    case XmlNodeType.EntityReference:
                        WriteEntityRef(reader.Name);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        WriteProcessingInstruction(reader.Name, reader.Value);
                        break;
                    case XmlNodeType.DocumentType:
                        WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                        break;

                    case XmlNodeType.Comment:
                        WriteComment(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        WriteFullEndElement();
                        break;
                }
            } while (reader.Read() && (d < reader.Depth || (d == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
        }


        // Element Helper Methods

        // Writes out an element with the specified name containing the specified string value.
        /// <summary>Writes an element with the specified local name and value.</summary>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="localName" /> value is null or an empty string.-or-The parameter values are not valid.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteElementString(string localName, String value)
        {
            WriteElementString(localName, null, value);
        }

        // Writes out an attribute with the specified name, namespace URI and string value.
        /// <summary>Writes an element with the specified local name, namespace URI, and value.</summary>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI to associate with the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="localName" /> value is null or an empty string.-or-The parameter values are not valid.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteElementString(string localName, String ns, String value)
        {
            WriteStartElement(localName, ns);
            if (null != value && 0 != value.Length)
            {
                WriteString(value);
            }
            WriteEndElement();
        }

        // Writes out an attribute with the specified name, namespace URI, and string value.
        /// <summary>Writes an element with the specified prefix, local name, namespace URI, and value.</summary>
        /// <param name="prefix">The prefix of the element.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI of the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="localName" /> value is null or an empty string.-or-The parameter values are not valid.</exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void WriteElementString(string prefix, String localName, String ns, String value)
        {
            WriteStartElement(prefix, localName, ns);
            if (null != value && 0 != value.Length)
            {
                WriteString(value);
            }
            WriteEndElement();
        }

        /// <summary>Releases all resources used by the current instance of the <see cref="T:System.Xml.XmlWriter" /> class.</summary>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Xml.XmlWriter" /> and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        protected virtual void Dispose(bool disposing)
        {
        }


        //
        // Static methods for creating writers
        //

        // Creates an XmlWriter for writing into the provided stream.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified stream.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
        /// <param name="output">The stream to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> writes XML 1.0 text syntax and appends it to the specified stream.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="stream" /> value is null.</exception>
        public static XmlWriter Create(Stream output)
        {
            return Create(output, null);
        }

        // Creates an XmlWriter for writing into the provided stream with the specified settings.
        public static XmlWriter Create(Stream output, XmlWriterSettings settings)
        {
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return settings.CreateWriter(output);
        }

        // Creates an XmlWriter for writing into the provided TextWriter.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.IO.TextWriter" />.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
        /// <param name="output">The <see cref="T:System.IO.TextWriter" /> to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> writes XML 1.0 text syntax and appends it to the specified <see cref="T:System.IO.TextWriter" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="text" /> value is null.</exception>
        public static XmlWriter Create(TextWriter output)
        {
            return Create(output, null);
        }

        // Creates an XmlWriter for writing into the provided TextWriter with the specified settings.
        public static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
        {
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return settings.CreateWriter(output);
        }

        // Creates an XmlWriter for writing into the provided StringBuilder.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.Text.StringBuilder" />.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
        /// <param name="output">The <see cref="T:System.Text.StringBuilder" /> to which to write to. Content written by the <see cref="T:System.Xml.XmlWriter" /> is appended to the <see cref="T:System.Text.StringBuilder" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="builder" /> value is null.</exception>
        public static XmlWriter Create(StringBuilder output)
        {
            return Create(output, null);
        }

        // Creates an XmlWriter for writing into the provided StringBuilder with the specified settings.
        public static XmlWriter Create(StringBuilder output, XmlWriterSettings settings)
        {
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            return settings.CreateWriter(new StringWriter(output, CultureInfo.InvariantCulture));
        }

        // Creates an XmlWriter wrapped around the provided XmlWriter with the default settings.
        /// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.Xml.XmlWriter" /> object.</summary>
        /// <returns>An <see cref="T:System.Xml.XmlWriter" /> object that is wrapped around the specified <see cref="T:System.Xml.XmlWriter" /> object.</returns>
        /// <param name="output">The <see cref="T:System.Xml.XmlWriter" /> object that you want to use as the underlying writer.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="writer" /> value is null.</exception>
        public static XmlWriter Create(XmlWriter output)
        {
            return Create(output, null);
        }

        // Creates an XmlWriter wrapped around the provided XmlWriter with the specified settings.
        public static XmlWriter Create(XmlWriter output, XmlWriterSettings settings)
        {
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return settings.CreateWriter(output);
        }
    }
}

