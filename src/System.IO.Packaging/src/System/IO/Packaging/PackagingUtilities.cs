// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//---------------------------------------------------------------------------
//
// Description:
//
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;               // For XmlReader
using System.Diagnostics;       // For Debug.Assert
using System.Text;              // For Encoding

namespace System.IO.Packaging
{
    internal static class PackagingUtilities
    {
        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------
        internal static readonly string RelationshipNamespaceUri = "http://schemas.openxmlformats.org/package/2006/relationships";
        internal static readonly ContentType RelationshipPartContentType
            = new ContentType("application/vnd.openxmlformats-package.relationships+xml");

        internal const string ContainerFileExtension = "xps";
        internal const string XamlFileExtension = "xaml";

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// This method is used to determine if we support a given Encoding as per the
        /// OPC and XPS specs. Currently the only two encodings supported are UTF-8 and
        /// UTF-16 (Little Endian and Big Endian)
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <returns>throws an exception if the encoding is not UTF-8 or UTF-16</returns>
        internal static void PerformInitailReadAndVerifyEncoding(XmlReader reader)
        {
            Invariant.Assert(reader != null && reader.ReadState == ReadState.Initial);

            //If the first node is XmlDeclaration we check to see if the encoding attribute is present
            if (reader.Read() && reader.NodeType == XmlNodeType.XmlDeclaration && reader.Depth == 0)
            {
                string encoding;
                encoding = reader.GetAttribute(_encodingAttribute);

                if (encoding != null && encoding.Length > 0)
                {
                    encoding = encoding.ToUpperInvariant();

                    //If a non-empty encoding attribute is present [for example - <?xml version="1.0" encoding="utf-8" ?>]
                    //we check to see if the value is either "utf-8" or utf-16. Only these two values are supported
                    //Note: For Byte order markings that require additional information to be specified in
                    //the encoding attribute in XmlDeclaration have already been ruled out by this check as we allow for
                    //only two valid values.
                    if (String.CompareOrdinal(encoding, s_webNameUTF8) == 0
                        || String.CompareOrdinal(encoding, s_webNameUnicode) == 0)
                        return;
                    else
                        //if the encoding attribute has any other value we throw an exception
                        throw new FileFormatException(SR.Get(SRID.EncodingNotSupported));
                }
            }

            //if the XmlDeclaration is not present, or encoding attribute is not present, we
            //base our decision on byte order marking. reader.Encoding will take that into account
            //and return the correct value. 
            //Note: For Byte order markings that require additional information to be specified in
            //the encoding attribute in XmlDeclaration have already been ruled out by the check above.
            //Note: If not encoding attribute is present or no byte order marking is present the 
            //encoding default to UTF8

            // todo ew
            // if (!(reader.Encoding is UnicodeEncoding || reader.Encoding is UTF8Encoding))
            //     throw new FileFormatException(SR.Get(SRID.EncodingNotSupported));
        }

        /// <summary>
        /// VerifyStreamReadArgs
        /// </summary>
        /// <param name="s">stream</param>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        /// <remarks>Common argument verification for Stream.Read()</remarks>
        static internal void VerifyStreamReadArgs(Stream s, byte[] buffer, int offset, int count)
        {
            if (!s.CanRead)
                throw new NotSupportedException(SR.Get(SRID.ReadNotSupported));

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", SR.Get(SRID.OffsetNegative));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", SR.Get(SRID.ReadCountNegative));
            }

            checked     // catch any integer overflows
            {
                if (offset + count > buffer.Length)
                {
                    throw new ArgumentException(SR.Get(SRID.ReadBufferTooSmall), "buffer");
                }
            }
        }

        /// <summary>
        /// VerifyStreamWriteArgs
        /// </summary>
        /// <param name="s"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <remarks>common argument verification for Stream.Write</remarks>
        static internal void VerifyStreamWriteArgs(Stream s, byte[] buffer, int offset, int count)
        {
            if (!s.CanWrite)
                throw new NotSupportedException(SR.Get(SRID.WriteNotSupported));

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", SR.Get(SRID.OffsetNegative));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", SR.Get(SRID.WriteCountNegative));
            }

            checked
            {
                if (offset + count > buffer.Length)
                    throw new ArgumentException(SR.Get(SRID.WriteBufferTooSmall), "buffer");
            }
        }

        /// <summary>
        /// Read utility that is guaranteed to return the number of bytes requested
        /// if they are available.
        /// </summary>
        /// <param name="stream">stream to read from</param>
        /// <param name="buffer">buffer to read into</param>
        /// <param name="offset">offset in buffer to write to</param>
        /// <param name="count">bytes to read</param>
        /// <returns>bytes read</returns>
        /// <remarks>Normal Stream.Read does not guarantee how many bytes it will
        /// return.  This one does.</remarks>
        internal static int ReliableRead(Stream stream, byte[] buffer, int offset, int count)
        {
            return ReliableRead(stream, buffer, offset, count, count);
        }

        /// <summary>
        /// Read utility that is guaranteed to return the number of bytes requested
        /// if they are available.
        /// </summary>
        /// <param name="stream">stream to read from</param>
        /// <param name="buffer">buffer to read into</param>
        /// <param name="offset">offset in buffer to write to</param>
        /// <param name="requestedCount">count of bytes that we would like to read (max read size to try)</param>
        /// <param name="requiredCount">minimal count of bytes that we would like to read (min read size to achieve)</param>
        /// <returns>bytes read</returns>
        /// <remarks>Normal Stream.Read does not guarantee how many bytes it will
        /// return.  This one does.</remarks>
        internal static int ReliableRead(Stream stream, byte[] buffer, int offset, int requestedCount, int requiredCount)
        {
            Invariant.Assert(stream != null);
            Invariant.Assert(buffer != null);
            Invariant.Assert(buffer.Length > 0);
            Invariant.Assert(offset >= 0);
            Invariant.Assert(requestedCount >= 0);
            Invariant.Assert(requiredCount >= 0);
            Invariant.Assert(checked(offset + requestedCount <= buffer.Length));
            Invariant.Assert(requiredCount <= requestedCount);

            // let's read the whole block into our buffer 
            int totalBytesRead = 0;
            while (totalBytesRead < requiredCount)
            {
                int bytesRead = stream.Read(buffer,
                                offset + totalBytesRead,
                                requestedCount - totalBytesRead);
                if (bytesRead == 0)
                {
                    break;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// Read utility that is guaranteed to return the number of bytes requested
        /// if they are available.
        /// </summary>
        /// <param name="reader">BinaryReader to read from</param>
        /// <param name="buffer">buffer to read into</param>
        /// <param name="offset">offset in buffer to write to</param>
        /// <param name="count">bytes to read</param>
        /// <returns>bytes read</returns>
        /// <remarks>Normal Stream.Read does not guarantee how many bytes it will
        /// return.  This one does.</remarks>
        internal static int ReliableRead(BinaryReader reader, byte[] buffer, int offset, int count)
        {
            return ReliableRead(reader, buffer, offset, count, count);
        }

        /// <summary>
        /// Read utility that is guaranteed to return the number of bytes requested
        /// if they are available.
        /// </summary>
        /// <param name="reader">BinaryReader to read from</param>
        /// <param name="buffer">buffer to read into</param>
        /// <param name="offset">offset in buffer to write to</param>
        /// <param name="requestedCount">count of bytes that we would like to read (max read size to try)</param>
        /// <param name="requiredCount">minimal count of bytes that we would like to read (min read size to achieve)</param>
        /// <returns>bytes read</returns>
        /// <remarks>Normal Stream.Read does not guarantee how many bytes it will
        /// return.  This one does.</remarks>
        internal static int ReliableRead(BinaryReader reader, byte[] buffer, int offset, int requestedCount, int requiredCount)
        {
            Invariant.Assert(reader != null);
            Invariant.Assert(buffer != null);
            Invariant.Assert(buffer.Length > 0);
            Invariant.Assert(offset >= 0);
            Invariant.Assert(requestedCount >= 0);
            Invariant.Assert(requiredCount >= 0);
            Invariant.Assert(checked(offset + requestedCount <= buffer.Length));
            Invariant.Assert(requiredCount <= requestedCount);

            // let's read the whole block into our buffer 
            int totalBytesRead = 0;
            while (totalBytesRead < requiredCount)
            {
                int bytesRead = reader.Read(buffer,
                                offset + totalBytesRead,
                                requestedCount - totalBytesRead);
                if (bytesRead == 0)
                {
                    break;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// CopyStream utility that is guaranteed to return the number of bytes copied (may be less then requested,
        /// if source stream doesn't have enough data)
        /// </summary>
        /// <param name="sourceStream">stream to read from</param>
        /// <param name="targetStream">stream to write to </param>
        /// <param name="bytesToCopy">number of bytes to be copied(use Int64.MaxValue if the whole stream needs to be copied)</param>
        /// <param name="bufferSize">number of bytes to be copied (usually it is 4K for scenarios where we expect a lot of data 
        ///  like in SparseMemoryStream case it could be larger </param>
        /// <returns>bytes copied (might be less than requested if source stream is too short</returns>
        /// <remarks>Neither source nor target stream are seeked; it is up to the caller to make sure that their positions are properly set.
        ///  Target stream isn't truncated even if it has more data past the area that was copied.</remarks> 
        internal static long CopyStream(Stream sourceStream, Stream targetStream, long bytesToCopy, int bufferSize)
        {
            Invariant.Assert(sourceStream != null);
            Invariant.Assert(targetStream != null);
            Invariant.Assert(bytesToCopy >= 0);
            Invariant.Assert(bufferSize > 0);

            byte[] buffer = new byte[bufferSize];

            // let's read the whole block into our buffer 
            long bytesLeftToCopy = bytesToCopy;
            while (bytesLeftToCopy > 0)
            {
                int bytesRead = sourceStream.Read(buffer, 0, (int)Math.Min(bytesLeftToCopy, (long)bufferSize));
                if (bytesRead == 0)
                {
                    targetStream.Flush();
                    return bytesToCopy - bytesLeftToCopy;
                }

                targetStream.Write(buffer, 0, bytesRead);
                bytesLeftToCopy -= bytesRead;
            }

            // It must not be negative
            Debug.Assert(bytesLeftToCopy == 0);

            targetStream.Flush();
            return bytesToCopy;
        }


        /// <summary>
        /// Calculate overlap between two blocks, returning the offset and length of the overlap
        /// </summary>
        /// <param name="block1Offset"></param>
        /// <param name="block1Size"></param>
        /// <param name="block2Offset"></param>
        /// <param name="block2Size"></param>
        /// <param name="overlapBlockOffset"></param>
        /// <param name="overlapBlockSize"></param>
        internal static void CalculateOverlap(long block1Offset, long block1Size,
                                              long block2Offset, long block2Size,
                                              out long overlapBlockOffset, out long overlapBlockSize)
        {
            checked
            {
                overlapBlockOffset = Math.Max(block1Offset, block2Offset);
                overlapBlockSize = Math.Min(block1Offset + block1Size, block2Offset + block2Size) - overlapBlockOffset;

                if (overlapBlockSize <= 0)
                {
                    overlapBlockSize = 0;
                }
            }
        }

        /// <summary>
        /// This method returns the count of xml attributes other than:
        /// 1. xmlns="namespace"
        /// 2. xmlns:someprefix="namespace"
        /// Reader should be positioned at the Element whose attributes
        /// are to be counted.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>An integer indicating the number of non-xmlns attributes</returns>
        internal static int GetNonXmlnsAttributeCount(XmlReader reader)
        {
            Debug.Assert(reader != null, "xmlReader should not be null");
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "XmlReader should be positioned at an Element");

            int readerCount = 0;

            //If true, reader moves to the attribute
            //If false, there are no more attributes (or none)
            //and in that case the position of the reader is unchanged.
            //First time through, since the reader will be positioned at an Element, 
            //MoveToNextAttribute is the same as MoveToFirstAttribute.
            while (reader.MoveToNextAttribute())
            {
                if (String.CompareOrdinal(reader.Name, XmlNamespace) != 0 &&
                    String.CompareOrdinal(reader.Prefix, XmlNamespace) != 0)
                    readerCount++;
            }

            //re-position the reader to the element
            reader.MoveToElement();

            return readerCount;
        }

        #endregion Internal Methods


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        /// <summary>
        /// Synchronize access to IsolatedStorage methods that can step on each-other
        /// </summary>
        /// <remarks>See PS 1468964 for details.</remarks>
        private const string XmlNamespace = "xmlns";
        private const string _encodingAttribute = "encoding";
        private static readonly string s_webNameUTF8 = Encoding.UTF8.WebName.ToUpperInvariant();
        private static readonly string s_webNameUnicode = Encoding.Unicode.WebName.ToUpperInvariant();

    }
}
