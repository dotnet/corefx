// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//-----------------------------------------------------------------------------
//
// Description:
//  This is a class for representing a PackageRelationship. This is a part of the 
//  MMCF Packaging Layer. 
//
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Xml;
using System.Text;              // for StringBuilder
using System.Diagnostics;       // for Debug.Assert

namespace System.IO.Packaging
{
    /// <summary>
    /// This class is used to express a relationship between a source and a target part. 
    /// The only way to create a PackageRelationship, is to call the PackagePart.CreateRelationship()
    /// or Package.CreateRelationship(). A relationship is owned by a part or by the package itself. 
    /// If the source part is deleted all the relationships it owns are also deleted. 
    /// A target of the relationship need not be present. </summary>
    public class PackageRelationship
    {
        //------------------------------------------------------
        //
        //  Public Constructors
        //
        //------------------------------------------------------
        // None
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// This is a reference to the parent PackagePart to which this relationship belongs.
        /// </summary>
        /// <value>Uri</value>
        public Uri SourceUri
        {
            get
            {
                if (_source == null)
                    return PackUriHelper.PackageRootUri;
                else
                    return _source.Uri;
            }
        }

        /// <summary>
        /// Uri of the TargetPart, that this relationship points to.
        /// </summary>
        /// <value></value>
        public Uri TargetUri
        {
            get
            {
                return _targetUri;
            }
        }

        /// <summary>
        /// Type of the relationship - used to uniquely define the role of the relationship
        /// </summary>
        /// <value></value>
        public string RelationshipType
        {
            get
            {
                return _relationshipType;
            }
        }

        /// <summary>
        /// Enumeration value indicating the interpretations of the "base" of the target uri.
        /// </summary>
        /// <value></value>
        public TargetMode TargetMode
        {
            get
            {
                return _targetMode;
            }
        }

        /// <summary>
        /// PackageRelationship's identifier. Unique across relationships for the given source.
        /// </summary>
        /// <value>String</value>
        public String Id
        {
            get
            {
                return _id;
            }
        }


        /// <summary>
        /// PackageRelationship's owning Package object.
        /// </summary>
        /// <value>Package</value>
        public Package Package
        {
            get
            {
                return _package;
            }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        // None
        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------
        // None
        //------------------------------------------------------
        //
        //  Internal Constructors
        //
        //------------------------------------------------------

        #region Internal Constructor

        /// <summary>
        /// PackageRelationship constructor
        /// </summary>
        /// <param name="package">Owning Package object for this relationship</param>
        /// <param name="sourcePart">owning part - will be null if the owner is the container</param>
        /// <param name="targetUri">target of relationship</param>
        /// <param name="targetMode">enum specifying the interpretation of the base uri for the target uri</param>
        /// <param name="relationshipType">type name</param>
        /// <param name="id">unique identifier</param>
        internal PackageRelationship(Package package, PackagePart sourcePart, Uri targetUri, TargetMode targetMode, string relationshipType, string id)
        {
            //sourcePart can be null to represent that the relationships are at the package level

            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (targetUri == null)
                throw new ArgumentNullException(nameof(targetUri));

            if (relationshipType == null)
                throw new ArgumentNullException(nameof(relationshipType));

            if (id == null)
                throw new ArgumentNullException(nameof(id));
            // The ID is guaranteed to be an XML ID by the caller (InternalRelationshipCollection).
            // The following check is a precaution against future bug introductions.
#if DEBUG
            try
            {
                // An XSD ID is an NCName that is unique. We can't check uniqueness at this level.
                XmlConvert.VerifyNCName(id);
            }
            catch (XmlException exception)
            {
                throw new XmlException(SR.Format(SR.NotAValidXmlIdString, id), exception);
            }
#endif

            // Additional check - don't accept absolute Uri's if targetMode is Internal.
            Debug.Assert((targetMode == TargetMode.External || !targetUri.IsAbsoluteUri),
                "PackageRelationship target must be relative if the TargetMode is Internal");

            // Additional check - Verify if the Enum value is valid
            Debug.Assert((targetMode >= TargetMode.Internal || targetMode <= TargetMode.External),
                "TargetMode enum value is out of Range");

            // Look for empty string or string with just spaces
            Debug.Assert(relationshipType.Trim() != String.Empty,
                "RelationshipType cannot be empty string or a string with just spaces");

            _package = package;
            _source = sourcePart;
            _targetUri = targetUri;
            _relationshipType = relationshipType;
            _targetMode = targetMode;
            _id = id;
        }

        #endregion Internal Constructor

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal static Uri ContainerRelationshipPartName
        {
            get
            {
                return s_containerRelationshipPartName;
            }
        }

        #endregion Internal Properties

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------
        // None
        //------------------------------------------------------
        //
        //  Internal Events
        //
        //------------------------------------------------------
        // None
        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
        // None
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Members

        private Package _package;
        private PackagePart _source;
        private Uri _targetUri;
        private string _relationshipType;
        private TargetMode _targetMode;
        private String _id;

        private static readonly Uri s_containerRelationshipPartName = PackUriHelper.CreatePartUri(new Uri("/_rels/.rels", UriKind.Relative));

        #endregion Private Members
    }
}
