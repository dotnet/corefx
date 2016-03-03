// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

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
    // Represents a writer that provides fast non-cached forward-only way of generating XML streams containing XML documents 
    // that conform to the W3C Extensible Markup Language (XML) 1.0 specification and the Namespaces in XML specification.
    /// <summary>Represents a writer that provides a fast, non-cached, forward-only way to generate streams or files that contain XML data.</summary>
    public abstract partial class XmlWriter : IDisposable
    {
        // Write methods
        // Writes out the XML declaration with the version "1.0".

        /// <summary>Asynchronously writes the XML declaration with the version "1.0".</summary>
        /// <returns>The task that represents the asynchronous WriteStartDocument operation.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteStartDocumentAsync()
        {
            throw NotImplemented.ByDesign;
        }

        //Writes out the XML declaration with the version "1.0" and the speficied standalone attribute.

        /// <summary>Asynchronously writes the XML declaration with the version "1.0" and the standalone attribute.</summary>
        /// <returns>The task that represents the asynchronous WriteStartDocument operation.</returns>
        /// <param name="standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no".</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteStartDocumentAsync(bool standalone)
        {
            throw NotImplemented.ByDesign;
        }

        //Closes any open elements or attributes and puts the writer back in the Start state.

        /// <summary>Asynchronously closes any open elements or attributes and puts the writer back in the Start state.</summary>
        /// <returns>The task that represents the asynchronous WriteEndDocument operation.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteEndDocumentAsync()
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out the DOCTYPE declaration with the specified name and optional attributes.

        /// <summary>Asynchronously writes the DOCTYPE declaration with the specified name and optional attributes.</summary>
        /// <returns>The task that represents the asynchronous WriteDocType operation.</returns>
        /// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
        /// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" where <paramref name="pubid" /> and <paramref name="sysid" /> are replaced with the value of the given arguments.</param>
        /// <param name="sysid">If <paramref name="pubid" /> is null and <paramref name="sysid" /> is non-null it writes SYSTEM "sysid" where <paramref name="sysid" /> is replaced with the value of this argument.</param>
        /// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out the specified start tag and associates it with the given namespace and prefix.

        /// <summary>Asynchronously writes the specified start tag and associates it with the given namespace and prefix.</summary>
        /// <returns>The task that represents the asynchronous WriteStartElement operation.</returns>
        /// <param name="prefix">The namespace prefix of the element.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI to associate with the element.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteStartElementAsync(string prefix, string localName, string ns)
        {
            throw NotImplemented.ByDesign;
        }

        // Closes one element and pops the corresponding namespace scope.

        /// <summary>Asynchronously closes one element and pops the corresponding namespace scope.</summary>
        /// <returns>The task that represents the asynchronous WriteEndElement operation.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteEndElementAsync()
        {
            throw NotImplemented.ByDesign;
        }

        // Closes one element and pops the corresponding namespace scope. Writes out a full end element tag, e.g. </element>.

        /// <summary>Asynchronously closes one element and pops the corresponding namespace scope.</summary>
        /// <returns>The task that represents the asynchronous WriteFullEndElement operation.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteFullEndElementAsync()
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out the attribute with the specified LocalName, value, and NamespaceURI.
        // Writes out the attribute with the specified prefix, LocalName, NamespaceURI and value.
        /// <summary>Asynchronously writes out the attribute with the specified prefix, local name, namespace URI, and value.</summary>
        /// <returns>The task that represents the asynchronous WriteAttributeString operation.</returns>
        /// <param name="prefix">The namespace prefix of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public Task WriteAttributeStringAsync(string prefix, string localName, string ns, string value)
        {
            Task task = WriteStartAttributeAsync(prefix, localName, ns);
            if (task.IsSuccess())
            {
                return WriteStringAsync(value).CallTaskFuncWhenFinishAsync(thisRef => thisRef.WriteEndAttributeAsync(), this);
            }
            else
            {
                return WriteAttributeStringAsyncHelper(task, value);
            }
        }

        private async Task WriteAttributeStringAsyncHelper(Task task, string value)
        {
            await task.ConfigureAwait(false);
            await WriteStringAsync(value).ConfigureAwait(false);
            await WriteEndAttributeAsync().ConfigureAwait(false);
        }

        // Writes the start of an attribute.

        /// <summary>Asynchronously writes the start of an attribute with the specified prefix, local name, and namespace URI.</summary>
        /// <returns>The task that represents the asynchronous WriteStartAttribute operation.</returns>
        /// <param name="prefix">The namespace prefix of the attribute.</param>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="ns">The namespace URI for the attribute.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        protected internal virtual Task WriteStartAttributeAsync(string prefix, string localName, string ns)
        {
            throw NotImplemented.ByDesign;
        }

        // Closes the attribute opened by WriteStartAttribute call.

        /// <summary>Asynchronously closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)" /> call.</summary>
        /// <returns>The task that represents the asynchronous WriteEndAttribute operation.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        protected internal virtual Task WriteEndAttributeAsync()
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out a <![CDATA[...]]>; block containing the specified text.

        /// <summary>Asynchronously writes out a &lt;![CDATA[...]]&gt; block containing the specified text.</summary>
        /// <returns>The task that represents the asynchronous WriteCData operation.</returns>
        /// <param name="text">The text to place inside the CDATA block.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteCDataAsync(string text)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out a comment <!--...-->; containing the specified text.

        /// <summary>Asynchronously writes out a comment &lt;!--...--&gt; containing the specified text.</summary>
        /// <returns>The task that represents the asynchronous WriteComment operation.</returns>
        /// <param name="text">Text to place inside the comment.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteCommentAsync(string text)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out a processing instruction with a space between the name and text as follows: <?name text?>

        /// <summary>Asynchronously writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.</summary>
        /// <returns>The task that represents the asynchronous WriteProcessingInstruction operation.</returns>
        /// <param name="name">The name of the processing instruction.</param>
        /// <param name="text">The text to include in the processing instruction.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteProcessingInstructionAsync(string name, string text)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out an entity reference as follows: "&"+name+";".

        /// <summary>Asynchronously writes out an entity reference as &amp;name;.</summary>
        /// <returns>The task that represents the asynchronous WriteEntityRef operation.</returns>
        /// <param name="name">The name of the entity reference.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteEntityRefAsync(string name)
        {
            throw NotImplemented.ByDesign;
        }

        // Forces the generation of a character entity for the specified Unicode character value.

        /// <summary>Asynchronously forces the generation of a character entity for the specified Unicode character value.</summary>
        /// <returns>The task that represents the asynchronous WriteCharEntity operation.</returns>
        /// <param name="ch">The Unicode character for which to generate a character entity.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteCharEntityAsync(char ch)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out the given whitespace.

        /// <summary>Asynchronously writes out the given white space.</summary>
        /// <returns>The task that represents the asynchronous WriteWhitespace operation.</returns>
        /// <param name="ws">The string of white space characters.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteWhitespaceAsync(string ws)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out the specified text content.

        /// <summary>Asynchronously writes the given text content.</summary>
        /// <returns>The task that represents the asynchronous WriteString operation.</returns>
        /// <param name="text">The text to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteStringAsync(string text)
        {
            throw NotImplemented.ByDesign;
        }

        // Write out the given surrogate pair as an entity reference.

        /// <summary>Asynchronously generates and writes the surrogate character entity for the surrogate character pair.</summary>
        /// <returns>The task that represents the asynchronous WriteSurrogateCharEntity operation.</returns>
        /// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF.</param>
        /// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes out the specified text content.

        /// <summary>Asynchronously writes text one buffer at a time.</summary>
        /// <returns>The task that represents the asynchronous WriteChars operation.</returns>
        /// <param name="buffer">Character array containing the text to write.</param>
        /// <param name="index">The position in the buffer indicating the start of the text to write.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteCharsAsync(char[] buffer, int index, int count)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes raw markup from the given character buffer.

        /// <summary>Asynchronously writes raw markup manually from a character buffer.</summary>
        /// <returns>The task that represents the asynchronous WriteRaw operation.</returns>
        /// <param name="buffer">Character array containing the text to write.</param>
        /// <param name="index">The position within the buffer indicating the start of the text to write.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteRawAsync(char[] buffer, int index, int count)
        {
            throw NotImplemented.ByDesign;
        }

        // Writes raw markup from the given string.

        /// <summary>Asynchronously writes raw markup manually from a string.</summary>
        /// <returns>The task that represents the asynchronous WriteRaw operation.</returns>
        /// <param name="data">String containing the text to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteRawAsync(string data)
        {
            throw NotImplemented.ByDesign;
        }

        // Encodes the specified binary bytes as base64 and writes out the resulting text.

        /// <summary>Asynchronously encodes the specified binary bytes as Base64 and writes out the resulting text.</summary>
        /// <returns>The task that represents the asynchronous WriteBase64 operation.</returns>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteBase64Async(byte[] buffer, int index, int count)
        {
            throw NotImplemented.ByDesign;
        }

        // Encodes the specified binary bytes as binhex and writes out the resulting text.
        /// <summary>Asynchronously encodes the specified binary bytes as BinHex and writes out the resulting text.</summary>
        /// <returns>The task that represents the asynchronous WriteBinHex operation.</returns>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteBinHexAsync(byte[] buffer, int index, int count)
        {
            return BinHexEncoder.EncodeAsync(buffer, index, count, this);
        }

        // Flushes data that is in the internal buffers into the underlying streams/TextReader and flushes the stream/TextReader.

        /// <summary>Asynchronously flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.</summary>
        /// <returns>The task that represents the asynchronous Flush operation.</returns>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task FlushAsync()
        {
            throw NotImplemented.ByDesign;
        }

        // Scalar Value Methods

        // Writes out the specified name, ensuring it is a valid NmToken according to the XML specification 
        // (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// <summary>Asynchronously writes out the specified name, ensuring it is a valid NmToken according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
        /// <returns>The task that represents the asynchronous WriteNmToken operation.</returns>
        /// <param name="name">The name to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteNmTokenAsync(string name)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentException(SR.Xml_EmptyName);
            }
            return WriteStringAsync(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
        }

        // Writes out the specified name, ensuring it is a valid Name according to the XML specification
        // (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        /// <summary>Asynchronously writes out the specified name, ensuring it is a valid name according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
        /// <returns>The task that represents the asynchronous WriteName operation.</returns>
        /// <param name="name">The name to write.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual Task WriteNameAsync(string name)
        {
            return WriteStringAsync(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
        }

        // Writes out the specified namespace-qualified name by looking up the prefix that is in scope for the given namespace.
        /// <summary>Asynchronously writes out the namespace-qualified name. This method looks up the prefix that is in scope for the given namespace.</summary>
        /// <returns>The task that represents the asynchronous WriteQualifiedName operation.</returns>
        /// <param name="localName">The local name to write.</param>
        /// <param name="ns">The namespace URI for the name.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public virtual async Task WriteQualifiedNameAsync(string localName, string ns)
        {
            if (ns != null && ns.Length > 0)
            {
                string prefix = LookupPrefix(ns);
                if (prefix == null)
                {
                    throw new ArgumentException(SR.Format(SR.Xml_UndefNamespace, ns));
                }
                await WriteStringAsync(prefix).ConfigureAwait(false);
                await WriteStringAsync(":").ConfigureAwait(false);
            }
            await WriteStringAsync(localName).ConfigureAwait(false);
        }

        // XmlReader Helper Methods

        // Writes out all the attributes found at the current position in the specified XmlReader.
        public virtual async Task WriteAttributesAsync(XmlReader reader, bool defattr)
        {
            if (null == reader)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.XmlDeclaration)
            {
                if (reader.MoveToFirstAttribute())
                {
                    await WriteAttributesAsync(reader, defattr).ConfigureAwait(false);
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
                        await WriteStartAttributeAsync(reader.Prefix, reader.LocalName, reader.NamespaceURI).ConfigureAwait(false);
                        while (reader.ReadAttributeValue())
                        {
                            if (reader.NodeType == XmlNodeType.EntityReference)
                            {
                                await WriteEntityRefAsync(reader.Name).ConfigureAwait(false);
                            }
                            else
                            {
                                await WriteStringAsync(reader.Value).ConfigureAwait(false);
                            }
                        }
                        await WriteEndAttributeAsync().ConfigureAwait(false);
                    }
                }
                while (reader.MoveToNextAttribute());
            }
        }

        // Copies the current node from the given reader to the writer (including child nodes), and if called on an element moves the XmlReader 
        // to the corresponding end element.
        public virtual Task WriteNodeAsync(XmlReader reader, bool defattr)
        {
            if (null == reader)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.Settings != null && reader.Settings.Async)
            {
                return WriteNodeAsync_CallAsyncReader(reader, defattr);
            }
            else
            {
                return WriteNodeAsync_CallSyncReader(reader, defattr);
            }
        }


        // Copies the current node from the given reader to the writer (including child nodes), and if called on an element moves the XmlReader 
        // to the corresponding end element.
        //use sync methods on the reader
        internal async Task WriteNodeAsync_CallSyncReader(XmlReader reader, bool defattr)
        {
            bool canReadChunk = reader.CanReadValueChunk;
            int d = reader.NodeType == XmlNodeType.None ? -1 : reader.Depth;
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        await WriteStartElementAsync(reader.Prefix, reader.LocalName, reader.NamespaceURI).ConfigureAwait(false);
                        await WriteAttributesAsync(reader, defattr).ConfigureAwait(false);
                        if (reader.IsEmptyElement)
                        {
                            await WriteEndElementAsync().ConfigureAwait(false);
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
                                await this.WriteCharsAsync(_writeNodeBuffer, 0, read).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            await WriteStringAsync(reader.Value).ConfigureAwait(false);
                        }
                        break;
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        await WriteWhitespaceAsync(reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.CDATA:
                        await WriteCDataAsync(reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.EntityReference:
                        await WriteEntityRefAsync(reader.Name).ConfigureAwait(false);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        await WriteProcessingInstructionAsync(reader.Name, reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.DocumentType:
                        await WriteDocTypeAsync(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value).ConfigureAwait(false);
                        break;

                    case XmlNodeType.Comment:
                        await WriteCommentAsync(reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.EndElement:
                        await WriteFullEndElementAsync().ConfigureAwait(false);
                        break;
                }
            } while (reader.Read() && (d < reader.Depth || (d == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
        }

        // Copies the current node from the given reader to the writer (including child nodes), and if called on an element moves the XmlReader 
        // to the corresponding end element.
        //use async methods on the reader
        internal async Task WriteNodeAsync_CallAsyncReader(XmlReader reader, bool defattr)
        {
            bool canReadChunk = reader.CanReadValueChunk;
            int d = reader.NodeType == XmlNodeType.None ? -1 : reader.Depth;
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        await WriteStartElementAsync(reader.Prefix, reader.LocalName, reader.NamespaceURI).ConfigureAwait(false);
                        await WriteAttributesAsync(reader, defattr).ConfigureAwait(false);
                        if (reader.IsEmptyElement)
                        {
                            await WriteEndElementAsync().ConfigureAwait(false);
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
                            while ((read = await reader.ReadValueChunkAsync(_writeNodeBuffer, 0, WriteNodeBufferSize).ConfigureAwait(false)) > 0)
                            {
                                await this.WriteCharsAsync(_writeNodeBuffer, 0, read).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            //reader.Value may block on Text or WhiteSpace node, use GetValueAsync
                            await WriteStringAsync(await reader.GetValueAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        }
                        break;
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        await WriteWhitespaceAsync(await reader.GetValueAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        break;
                    case XmlNodeType.CDATA:
                        await WriteCDataAsync(reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.EntityReference:
                        await WriteEntityRefAsync(reader.Name).ConfigureAwait(false);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        await WriteProcessingInstructionAsync(reader.Name, reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.DocumentType:
                        await WriteDocTypeAsync(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value).ConfigureAwait(false);
                        break;

                    case XmlNodeType.Comment:
                        await WriteCommentAsync(reader.Value).ConfigureAwait(false);
                        break;
                    case XmlNodeType.EndElement:
                        await WriteFullEndElementAsync().ConfigureAwait(false);
                        break;
                }
            } while (await reader.ReadAsync().ConfigureAwait(false) && (d < reader.Depth || (d == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
        }


        // Element Helper Methods

        // Writes out an attribute with the specified name, namespace URI, and string value.
        /// <summary>Asynchronously writes an element with the specified prefix, local name, namespace URI, and value.</summary>
        /// <returns>The task that represents the asynchronous WriteElementString operation.</returns>
        /// <param name="prefix">The prefix of the element.</param>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">The namespace URI of the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
        /// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to true. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
        public async Task WriteElementStringAsync(string prefix, String localName, String ns, String value)
        {
            await WriteStartElementAsync(prefix, localName, ns).ConfigureAwait(false);
            if (null != value && 0 != value.Length)
            {
                await WriteStringAsync(value).ConfigureAwait(false);
            }
            await WriteEndElementAsync().ConfigureAwait(false);
        }
    }
}

