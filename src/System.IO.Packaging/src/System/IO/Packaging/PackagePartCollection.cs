// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//-----------------------------------------------------------------------------
//
// Description:
//  This is a base abstract class for PackagePartCollection. This is a part of the 
//  MMCF Packaging Layer
//
//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.IO.Packaging
{
    /// <summary>
    /// This class is used to get an enumerator for the Parts in a container. 
    /// This is a part of the Packaging Layer APIs
    /// </summary>   
    public class PackagePartCollection : IEnumerable<PackagePart>
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
        /// Returns an enumerator over all the Parts in the container
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        /// <summary>
        /// Returns an enumerator over all the Parts in the container
        /// </summary>
        /// <returns></returns>
        IEnumerator<PackagePart> IEnumerable<PackagePart>.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Returns an enumerator over all the Parts in the Container
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PackagePart> GetEnumerator()
        {
            //The Dictionary.Values property always returns a collection, even if empty. It never returns a null.
            return _partList.Values.GetEnumerator();
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

        #region Internal Constructor

        internal PackagePartCollection(SortedList<PackUriHelper.ValidatedPartUri, PackagePart> partList)
        {
            Debug.Assert(partList != null, "partDictionary parameter cannot be null");
            _partList = partList;
        }

        #endregion Internal Constructor

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------
        // None
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

        private SortedList<PackUriHelper.ValidatedPartUri, PackagePart> _partList;

        #endregion Private Members
    }
}
