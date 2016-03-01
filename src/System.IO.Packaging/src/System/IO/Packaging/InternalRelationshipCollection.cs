// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//-----------------------------------------------------------------------------
//
// Description:
//  This is a class for representing a PackageRelationshipCollection. This is an internal
//  class for manipulating relationships associated with a part 
//
// Details:
//   This class handles serialization to/from relationship parts, creation of those parts
//   and offers methods to create, delete and enumerate relationships. This code was
//   moved from the PackageRelationshipCollection class.
//
//-----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Xml;                           // for XmlReader/Writer
using System.Diagnostics;

namespace System.IO.Packaging
{
    /// <summary>
    /// Collection of all the relationships corresponding to a given source PackagePart
    /// </summary>
    internal class InternalRelationshipCollection : IEnumerable<PackageRelationship>
    {
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region IEnumerable
        /// <summary>
        /// Returns an enumerator over all the relationships for a Package or a PackagePart
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _relationships.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumertor over all the relationships for a Package or a PackagePart
        /// </summary>
        /// <returns></returns>
        IEnumerator<PackageRelationship> IEnumerable<PackageRelationship>.GetEnumerator()
        {
            return _relationships.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumertor over all the relationships for a Package or a PackagePart
        /// </summary>
        /// <returns></returns>
        public List<PackageRelationship>.Enumerator GetEnumerator()
        {
            return _relationships.GetEnumerator();
        }

        #endregion

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------
        #region Internal Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>For use by PackagePart</remarks>
        internal InternalRelationshipCollection(PackagePart part) : this(part.Package, part)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>For use by Package</remarks>
        internal InternalRelationshipCollection(Package package) : this(package, null)
        {
        }

        /// <summary>
        /// Add new relationship
        /// </summary>
        /// <param name="targetUri">target</param>
        /// <param name="targetMode">Enumeration indicating the base uri for the target uri</param>
        /// <param name="relationshipType">relationship type that uniquely defines the role of the relationship</param>
        /// <param name="id">String that conforms to the xsd:ID datatype. Unique across the source's relationships.
        /// Null OK (ID will be generated).</param>
        internal PackageRelationship Add(Uri targetUri, TargetMode targetMode, string relationshipType, string id)
        {
            return Add(targetUri, targetMode, relationshipType, id, parsing: false);
        }

        /// <summary>
        /// Return the relationship whose id is 'id', and null if not found.
        /// </summary>
        internal PackageRelationship GetRelationship(string id)
        {
            int index = GetRelationshipIndex(id);
            if (index == -1)
                return null;
            return _relationships[index];
        }

        /// <summary>
        /// Delete relationship with ID 'id'
        /// </summary>
        /// <param name="id">ID of the relationship to remove</param>
        internal void Delete(String id)
        {
            int index = GetRelationshipIndex(id);
            if (index == -1)
                return;

            _relationships.RemoveAt(index);
            _dirty = true;
        }

        /// <summary>
        /// Clear all the relationships in this collection
        /// Today it is only used when the entire relationship part is being deleted
        /// </summary>
        internal void Clear()
        {
            _relationships.Clear();
            _dirty = true;
        }

        /// <summary>
        /// Flush to stream (destructive)
        /// </summary>
        /// <remarks>
        /// Flush part.
        /// </remarks>
        internal void Flush()
        {
            if (!_dirty)
                return;

            if (_relationships.Count == 0)  // empty?
            {
                // delete the part
                if (_package.PartExists(_uri))
                {
                    _package.DeletePart(_uri);
                }
                _relationshipPart = null;
            }
            else
            {
                EnsureRelationshipPart();   // lazy init

                // write xml
                WriteRelationshipPart(_relationshipPart);
            }
            _dirty = false;
        }

        internal static void ThrowIfInvalidRelationshipType(string relationshipType)
        {
            // Look for empty string or string with just spaces
            if (relationshipType.Trim() == String.Empty)
                throw new ArgumentException(SR.InvalidRelationshipType);
        }

        // If 'id' is not of the xsd type ID, throw an exception.
        internal static void ThrowIfInvalidXsdId(string id)
        {
            Debug.Assert(id != null, "id should not be null");

            try
            {
                // An XSD ID is an NCName that is unique.
                XmlConvert.VerifyNCName(id);
            }
            catch (XmlException exception)
            {
                var r = SR.NotAValidXmlIdString;
                var s = SR.Format(r, id);
                throw new XmlException(s, exception);
            }
        }

        #endregion Internal Methods

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        #region Private Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="package">package</param>
        /// <param name="part">part will be null if package is the source of the relationships</param>
        /// <remarks>Shared constructor</remarks>
        private InternalRelationshipCollection(Package package, PackagePart part)
        {
            Debug.Assert(package != null, "package parameter passed should never be null");

            _package = package;
            _sourcePart = part;

            //_sourcePart may be null representing that the relationships are at the package level
            _uri = GetRelationshipPartUri(_sourcePart);
            _relationships = new List<PackageRelationship>(4);

            // Load if available (not applicable to write-only mode).
            if ((package.FileOpenAccess == FileAccess.Read ||
                package.FileOpenAccess == FileAccess.ReadWrite) && package.PartExists(_uri))
            {
                _relationshipPart = package.GetPart(_uri);
                ThrowIfIncorrectContentType(_relationshipPart.ValidatedContentType);
                ParseRelationshipPart(_relationshipPart);
            }

            //Any initialization in the constructor should not set the dirty flag to true.
            _dirty = false;
        }

        /// <summary>
        /// Returns the associated RelationshipPart for this part
        /// </summary>
        /// <param name="part">may be null</param>
        /// <returns>name of relationship part for the given part</returns>
        private static Uri GetRelationshipPartUri(PackagePart part)
        {
            Uri sourceUri;

            if (part == null)
                sourceUri = PackUriHelper.PackageRootUri;
            else
                sourceUri = part.Uri;

            return PackUriHelper.GetRelationshipPartUri(sourceUri);
        }

        /// <summary>
        /// Parse PackageRelationship Stream
        /// </summary>
        /// <param name="part">relationship part</param>
        /// <exception cref="XmlException">Thrown if XML is malformed</exception>
        private void ParseRelationshipPart(PackagePart part)
        {
            //We can safely open the stream as FileAccess.Read, as this code
            //should only be invoked if the Package has been opened in Read or ReadWrite mode.
            Debug.Assert(_package.FileOpenAccess == FileAccess.Read || _package.FileOpenAccess == FileAccess.ReadWrite,
                "This method should only be called when FileAccess is Read or ReadWrite");

            using (Stream s = part.GetStream(FileMode.Open, FileAccess.Read))
            {
                // load from the relationship part associated with the given part
                using (XmlReader baseReader = XmlReader.Create(s))
                {
                    using (XmlCompatibilityReader reader = new XmlCompatibilityReader(baseReader, s_relationshipKnownNamespaces))
                    {
                        //This method expects the reader to be in ReadState.Initial.
                        //It will make the first read call.
                        PackagingUtilities.PerformInitialReadAndVerifyEncoding(baseReader);

                        //Note: After the previous method call the reader should be at the first tag in the markup.
                        //MoveToContent - Skips over the following - ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace
                        //If the reader is currently at a content node then this function call is a no-op
                        reader.MoveToContent();

                        // look for our tag and namespace pair - throw if other elements are encountered
                        // Make sure that the current node read is an Element 
                        if (reader.NodeType == XmlNodeType.Element
                            && (reader.Depth == 0)
                            && (String.CompareOrdinal(s_relationshipsTagName, reader.LocalName) == 0)
                            && (String.CompareOrdinal(PackagingUtilities.RelationshipNamespaceUri, reader.NamespaceURI) == 0))
                        {
                            ThrowIfXmlBaseAttributeIsPresent(reader);

                            //There should be a namespace Attribute present at this level. 
                            //Also any other attribute on the <Relationships> tag is an error including xml: and xsi: attributes
                            if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) > 0)
                                throw new XmlException(SR.RelationshipsTagHasExtraAttributes, null, reader.LineNumber, reader.LinePosition);

                            // start tag encountered for Relationships
                            // now parse individual Relationship tags
                            while (reader.Read())
                            {
                                //Skips over the following - ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace
                                //If the reader is currently at a content node then this function call is a no-op
                                reader.MoveToContent();

                                //If MoveToContent() takes us to the end of the content
                                if (reader.NodeType == XmlNodeType.None)
                                    continue;

                                if (reader.NodeType == XmlNodeType.Element
                                    && (reader.Depth == 1)
                                    && (String.CompareOrdinal(s_relationshipTagName, reader.LocalName) == 0)
                                    && (String.CompareOrdinal(PackagingUtilities.RelationshipNamespaceUri, reader.NamespaceURI) == 0))
                                {
                                    ThrowIfXmlBaseAttributeIsPresent(reader);

                                    int expectedAttributesCount = 3;

                                    string targetModeAttributeValue = reader.GetAttribute(s_targetModeAttributeName);
                                    if (targetModeAttributeValue != null)
                                        expectedAttributesCount++;

                                    //check if there are expected number of attributes. 
                                    //Also any other attribute on the <Relationship> tag is an error including xml: and xsi: attributes
                                    if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) == expectedAttributesCount)
                                    {
                                        ProcessRelationshipAttributes(reader);

                                        //Skip the EndElement for Relationship
                                        if (!reader.IsEmptyElement)
                                            ProcessEndElementForRelationshipTag(reader);
                                    }
                                    else
                                    {
                                        throw new XmlException(SR.RelationshipTagDoesntMatchSchema, null, reader.LineNumber, reader.LinePosition);
                                    }
                                }
                                else
                                    if (!(String.CompareOrdinal(s_relationshipsTagName, reader.LocalName) == 0 && (reader.NodeType == XmlNodeType.EndElement)))
                                    throw new XmlException(SR.UnknownTagEncountered, null, reader.LineNumber, reader.LinePosition);
                            }
                        }
                        else throw new XmlException(SR.ExpectedRelationshipsElementTag, null, reader.LineNumber, reader.LinePosition);
                    }
                }
            }
        }


        //This method processes the attributes that are present on the Relationship element
        private void ProcessRelationshipAttributes(XmlCompatibilityReader reader)
        {
            // Attribute : TargetMode

            string targetModeAttributeValue = reader.GetAttribute(s_targetModeAttributeName);

            //If the TargetMode attribute is missing in the underlying markup then we assume it to be internal
            TargetMode relationshipTargetMode = TargetMode.Internal;

            if (targetModeAttributeValue != null)
            {
                try
                {
                    relationshipTargetMode = (TargetMode)(Enum.Parse(typeof(TargetMode), targetModeAttributeValue, ignoreCase: false));
                }
                catch (ArgumentNullException argNullEx)
                {
                    ThrowForInvalidAttributeValue(reader, s_targetModeAttributeName, argNullEx);
                }
                catch (ArgumentException argEx)
                {
                    //if the targetModeAttributeValue is not Internal|External then Argument Exception will be thrown.
                    ThrowForInvalidAttributeValue(reader, s_targetModeAttributeName, argEx);
                }
            }

            // Attribute : Target
            // create a new PackageRelationship
            string targetAttributeValue = reader.GetAttribute(s_targetAttributeName);
            if (string.IsNullOrEmpty(targetAttributeValue))
                throw new XmlException(SR.Format(SR.RequiredRelationshipAttributeMissing, s_targetAttributeName), null, reader.LineNumber, reader.LinePosition);

            Uri targetUri = new Uri(targetAttributeValue, UriKind.RelativeOrAbsolute);

            // Attribute : Type
            string typeAttributeValue = reader.GetAttribute(s_typeAttributeName);
            if (string.IsNullOrEmpty(typeAttributeValue))
                throw new XmlException(SR.Format(SR.RequiredRelationshipAttributeMissing, s_typeAttributeName), null, reader.LineNumber, reader.LinePosition);

            // Attribute : Id
            // Get the Id attribute (required attribute).
            string idAttributeValue = reader.GetAttribute(s_idAttributeName);
            if (string.IsNullOrEmpty(idAttributeValue))
                throw new XmlException(SR.Format(SR.RequiredRelationshipAttributeMissing, s_idAttributeName), null, reader.LineNumber, reader.LinePosition);

            // Add the relationship to the collection
            Add(targetUri, relationshipTargetMode, typeAttributeValue, idAttributeValue, parsing: true);
        }

        //If End element is present for Relationship then we process it
        private void ProcessEndElementForRelationshipTag(XmlCompatibilityReader reader)
        {
            Debug.Assert(!reader.IsEmptyElement, "This method should only be called if the Relationship Element is not empty");

            reader.Read();

            //Skips over the following - ProcessingInstruction, DocumentType, Comment, Whitespace, or SignificantWhitespace
            reader.MoveToContent();

            if (reader.NodeType == XmlNodeType.EndElement && String.CompareOrdinal(s_relationshipTagName, reader.LocalName) == 0)
                return;
            else
                throw new XmlException(SR.Format(SR.ElementIsNotEmptyElement, s_relationshipTagName), null, reader.LineNumber, reader.LinePosition);
        }


        /// <summary>
        /// Add new relationship to the Collection
        /// </summary>
        /// <param name="targetUri">target</param>
        /// <param name="targetMode">Enumeration indicating the base uri for the target uri</param>
        /// <param name="relationshipType">relationship type that uniquely defines the role of the relationship</param>
        /// <param name="id">String that conforms to the xsd:ID datatype. Unique across the source's relationships.
        /// Null OK (ID will be generated).</param>
        /// <param name="parsing">Indicates whether the add call is made while parsing existing relationships
        /// from a relationship part, or we are adding a new relationship</param>
        private PackageRelationship Add(Uri targetUri, TargetMode targetMode, string relationshipType, string id, bool parsing)
        {
            if (targetUri == null)
                throw new ArgumentNullException(nameof(targetUri));

            if (relationshipType == null)
                throw new ArgumentNullException(nameof(relationshipType));

            ThrowIfInvalidRelationshipType(relationshipType);

            //Verify if the Enum value is valid
            if (targetMode < TargetMode.Internal || targetMode > TargetMode.External)
                throw new ArgumentOutOfRangeException(nameof(targetMode));

            // don't accept absolute Uri's if targetMode is Internal.
            if (targetMode == TargetMode.Internal && targetUri.IsAbsoluteUri)
                throw new ArgumentException(SR.RelationshipTargetMustBeRelative, nameof(targetUri));

            // don't allow relationships to relationships
            //  This check should be made for following cases
            //      1. Uri is absolute and it is pack Uri
            //      2. Uri is NOT absolute and its target mode is internal (or NOT external)
            //      Note: if the target is absolute uri and its not a pack scheme then we cannot determine if it is a rels part
            //      Note: if the target is relative uri and target mode is external, we cannot determine if it is a rels part
            if ((!targetUri.IsAbsoluteUri && targetMode != TargetMode.External)
                    || (targetUri.IsAbsoluteUri && targetUri.Scheme == PackUriHelper.UriSchemePack))
            {
                Uri resolvedUri = GetResolvedTargetUri(targetUri, targetMode);
                //GetResolvedTargetUri returns a null if the target mode is external and the 
                //target Uri is a packUri with no "part" component, so in that case we know that 
                //its not a relationship part. 
                if (resolvedUri != null)
                {
                    if (PackUriHelper.IsRelationshipPartUri(resolvedUri))
                        throw new ArgumentException(SR.RelationshipToRelationshipIllegal, nameof(targetUri));
                }
            }

            // Generate an ID if id is null. Throw exception if neither null nor a valid unique xsd:ID.
            if (id == null)
                id = GenerateUniqueRelationshipId();
            else
                ValidateUniqueRelationshipId(id);

            //Ensure the relationship part
            EnsureRelationshipPart();

            // create and add
            PackageRelationship relationship = new PackageRelationship(_package, _sourcePart, targetUri, targetMode, relationshipType, id);
            _relationships.Add(relationship);

            //If we are adding relationships as a part of Parsing the underlying relationship part, we should not set
            //the dirty flag to false.
            _dirty = !parsing;

            return relationship;
        }

        /// <summary>
        /// Write PackageRelationship Stream
        /// </summary>
        /// <param name="part">part to persist to</param>
        private void WriteRelationshipPart(PackagePart part)
        {
            using (IgnoreFlushAndCloseStream s = new IgnoreFlushAndCloseStream(part.GetStream()))
            {
                s.SetLength(0);    // truncate to resolve PS 954048

                // use UTF-8 encoding by default
                using (XmlWriter writer = XmlWriter.Create(s, new XmlWriterSettings { Encoding = System.Text.Encoding.UTF8 }))
                {
                    writer.WriteStartDocument();

                    // start outer Relationships tag
                    writer.WriteStartElement(s_relationshipsTagName, PackagingUtilities.RelationshipNamespaceUri);

                    // Write Relationship elements.
                    WriteRelationshipsAsXml(
                        writer,
                        _relationships,
                        false /* do not systematically write target mode */
                        );

                    // end of Relationships tag
                    writer.WriteEndElement();

                    // close the document
                    writer.WriteEndDocument();
                }
            }
        }

        /// <summary>
        /// Write one Relationship element for each member of relationships.
        /// This method is used by XmlDigitalSignatureProcessor code as well
        /// </summary>
        internal static void WriteRelationshipsAsXml(XmlWriter writer, IEnumerable<PackageRelationship> relationships, bool alwaysWriteTargetModeAttribute)
        {
            foreach (PackageRelationship relationship in relationships)
            {
                writer.WriteStartElement(s_relationshipTagName);

                // Write RelationshipType attribute.
                writer.WriteAttributeString(s_typeAttributeName, relationship.RelationshipType);

                // Write Target attribute.
                // We would like to persist the uri as passed in by the user and so we use the
                // OriginalString property. This makes the persisting behavior consistent
                // for relative and absolute Uris. 
                // Since we accpeted the Uri as a string, we are at the minimum guaranteed that
                // the string can be converted to a valid Uri. 
                // Also, we are just using it here to persist the information and we are not
                // resolving or fetching a resource based on this Uri.
                writer.WriteAttributeString(s_targetAttributeName, relationship.TargetUri.OriginalString);

                // TargetMode is optional attribute in the markup and its default value is TargetMode="Internal" 
                if (alwaysWriteTargetModeAttribute || relationship.TargetMode == TargetMode.External)
                    writer.WriteAttributeString(s_targetModeAttributeName, relationship.TargetMode.ToString());

                // Write Id attribute.
                writer.WriteAttributeString(s_idAttributeName, relationship.Id);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Ensures that the PackageRelationship PackagePart has been created - lazy init
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void EnsureRelationshipPart()
        {
            if (_relationshipPart == null || _relationshipPart.IsDeleted)
            {
                if (_package.PartExists(_uri))
                {
                    _relationshipPart = _package.GetPart(_uri);
                    ThrowIfIncorrectContentType(_relationshipPart.ValidatedContentType);
                }
                else
                {
                    CompressionOption compressionOption = _sourcePart == null ? CompressionOption.NotCompressed : _sourcePart.CompressionOption;
                    _relationshipPart = _package.CreatePart(_uri, PackagingUtilities.RelationshipPartContentType.ToString(), compressionOption);
                }
            }
        }

        /// <summary>
        /// Resolves the target uri in the relationship against the source part or the 
        /// package root. This resolved Uri is then used by the Add method to figure 
        /// out if a relationship is being created to another relationship part.
        /// </summary>
        /// <param name="target">PackageRelationship target uri</param>
        /// <param name="targetMode"> Enum value specifying the interpretation of the base uri
        /// for the relationship target uri</param>
        /// <returns>Resolved Uri</returns>
        private Uri GetResolvedTargetUri(Uri target, TargetMode targetMode)
        {
            if (targetMode == TargetMode.Internal)
            {
                Debug.Assert(!target.IsAbsoluteUri, "Uri should be relative at this stage");

                if (_sourcePart == null) //indicates that the source is the package root
                    return PackUriHelper.ResolvePartUri(PackUriHelper.PackageRootUri, target);
                else
                    return PackUriHelper.ResolvePartUri(_sourcePart.Uri, target);
            }
            else
            {
                if (target.IsAbsoluteUri)
                {
                    if (string.Equals(target.Scheme, PackUriHelper.UriSchemePack))
                        return PackUriHelper.GetPartUri(target);
                }
                else
                    Debug.Fail("Uri should not be relative at this stage");
            }
            // relative to the location of the package.
            return target;
        }

        //Throws an exception if the relationship part does not have the correct content type
        private void ThrowIfIncorrectContentType(ContentType contentType)
        {
            if (!contentType.AreTypeAndSubTypeEqual(PackagingUtilities.RelationshipPartContentType))
                throw new FileFormatException(SR.RelationshipPartIncorrectContentType);
        }

        //Throws an exception if the xml:base attribute is present in the Relationships XML
        private void ThrowIfXmlBaseAttributeIsPresent(XmlCompatibilityReader reader)
        {
            string xmlBaseAttributeValue = reader.GetAttribute(s_xmlBaseAttributeName);

            if (xmlBaseAttributeValue != null)
                throw new XmlException(SR.Format(SR.InvalidXmlBaseAttributePresent, s_xmlBaseAttributeName), null, reader.LineNumber, reader.LinePosition);
        }

        //Throws an XML exception if the attribute value is invalid
        private void ThrowForInvalidAttributeValue(XmlCompatibilityReader reader, String attributeName, Exception ex)
        {
            throw new XmlException(SR.Format(SR.InvalidValueForTheAttribute, attributeName), ex, reader.LineNumber, reader.LinePosition);
        }

        // Generate a unique relation ID.
        private string GenerateUniqueRelationshipId()
        {
            string id;
            do
            {
                id = GenerateRelationshipId();
            } while (GetRelationship(id) != null);
            return id;
        }

        // Build an ID string consisting of the letter 'R' followed by an 8-byte GUID timestamp.
        // Guid.ToString() outputs the bytes in the big-endian order (higher order byte first)
        private string GenerateRelationshipId()
        {
            // The timestamp consists of the first 8 hex octets of the GUID.
            return String.Concat("R", Guid.NewGuid().ToString("N").Substring(0, s_timestampLength));
        }

        // If 'id' is not of the xsd type ID or is not unique for this collection, throw an exception.
        private void ValidateUniqueRelationshipId(string id)
        {
            // An XSD ID is an NCName that is unique.
            ThrowIfInvalidXsdId(id);

            // Check for uniqueness.
            if (GetRelationshipIndex(id) >= 0)
                throw new XmlException(SR.Format(SR.NotAUniqueRelationshipId, id));
        }


        // Retrieve a relationship's index in _relationships given its id.
        // Return a negative value if not found.
        private int GetRelationshipIndex(string id)
        {
            for (int index = 0; index < _relationships.Count; ++index)
                if (string.Equals(_relationships[index].Id, id, StringComparison.Ordinal))
                    return index;

            return -1;
        }

        #endregion

        #region Private Properties

        #endregion Private Properties

        //------------------------------------------------------
        //
        //  Private Members
        //
        //------------------------------------------------------
        #region Private Members
        private List<PackageRelationship> _relationships;
        private bool _dirty;    // true if we have uncommitted changes to _relationships
        private Package _package;     // our package - in case _sourcePart is null
        private PackagePart _sourcePart;      // owning part - null if package is the owner
        private PackagePart _relationshipPart;  // where our relationships are persisted
        private Uri _uri;           // the URI of our relationship part

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        // segment that indicates a relationship part

        private static readonly int s_timestampLength = 16;

        private static readonly string s_relationshipsTagName = "Relationships";
        private static readonly string s_relationshipTagName = "Relationship";
        private static readonly string s_targetAttributeName = "Target";
        private static readonly string s_typeAttributeName = "Type";
        private static readonly string s_idAttributeName = "Id";
        private static readonly string s_xmlBaseAttributeName = "xml:base";
        private static readonly string s_targetModeAttributeName = "TargetMode";

        private static readonly string[] s_relationshipKnownNamespaces
            = new string[] { PackagingUtilities.RelationshipNamespaceUri };

        #endregion    
    }
}
