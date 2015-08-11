// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//-----------------------------------------------------------------------------
//
// Description:
//  This is a subclass for the abstract PackagePart class.
//  This implementation is specific to Zip file format.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace System.IO.Packaging
{
    /// <summary>
    /// This class represents a Part within a Zip container.
    /// This is a part of the Packaging Layer APIs
    /// </summary>
    public sealed class ZipPackagePart : PackagePart
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
        // None
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Custom Implementation for the GetStream Method
        /// </summary>
        /// <param name="streamFileMode">Mode in which the stream should be opened</param>
        /// <param name="streamFileAccess">Access with which the stream should be opened</param>
        /// <returns>Stream Corresponding to this part</returns>
        protected override Stream GetStreamCore(FileMode streamFileMode, FileAccess streamFileAccess)
        {
            if (_zipArchiveEntry != null)
            {
                if (streamFileMode == FileMode.Create)
                {
                    using (var tempStream = _zipStreamManager.Open(_zipArchiveEntry, streamFileMode, streamFileAccess))
                    {
                        tempStream.SetLength(0);
                    }
                }

                var stream = _zipStreamManager.Open(_zipArchiveEntry, streamFileMode, streamFileAccess);
                return stream;
            }
            return null;
        }

        #endregion Public Methods

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

        #region Internal Constructors

        /// <summary>
        /// Constructs a ZipPackagePart for an atomic (i.e. non-interleaved) part.
        /// This is called from the ZipPackage class as a result of GetPartCore,
        /// GetPartsCore or CreatePartCore methods     
        /// </summary>
        /// <param name="zipPackage"></param>
        /// <param name="zipArchive"></param>
        /// <param name="zipArchiveEntry"></param>
        /// <param name="zipStreamManager"></param>
        /// <param name="partUri"></param>
        /// <param name="compressionOption"></param>
        /// <param name="contentType"></param>
        internal ZipPackagePart(ZipPackage zipPackage,
            ZipArchive zipArchive,
            ZipArchiveEntry zipArchiveEntry,
            ZipStreamManager zipStreamManager,
            PackUriHelper.ValidatedPartUri partUri,
            string contentType,
            CompressionOption compressionOption)
            : base(zipPackage, partUri, contentType, compressionOption)
        {
            _zipPackage = zipPackage;
            _zipArchive = zipArchive;
            _zipStreamManager = zipStreamManager;
            _zipArchiveEntry = zipArchiveEntry;
        }

        #endregion Internal Constructors

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        /// <summary>
        /// Obtain the ZipFileInfo descriptor of an atomic part.
        /// </summary>
        internal ZipArchiveEntry ZipArchiveEntry
        {
            get
            {
                return _zipArchiveEntry;
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

        #region Private Variables

        private ZipPackage _zipPackage;
        private ZipArchiveEntry _zipArchiveEntry;
        private ZipArchive _zipArchive;
        private ZipStreamManager _zipStreamManager;

        #endregion Private Variables

        //------------------------------------------------------
    }
}
