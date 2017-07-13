// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Security.Permissions;

namespace System.DirectoryServices
{
    /// <include file='doc\SearchResult.uex' path='docs/doc[@for="SearchResult"]/*' />
    /// <devdoc>
    ///    <para>Encapsulates a node in the Active Directory hierarchy 
    ///       that is returned during a search through <see cref='System.DirectoryServices.DirectorySearcher'/>.</para>
    /// </devdoc>
    public class SearchResult
    {
        private readonly NetworkCredential _parentCredentials;
        private readonly AuthenticationTypes _parentAuthenticationType;

        internal SearchResult(NetworkCredential parentCredentials, AuthenticationTypes parentAuthenticationType)
        {
            _parentCredentials = parentCredentials;
            _parentAuthenticationType = parentAuthenticationType;
        }

        /// <include file='doc\SearchResult.uex' path='docs/doc[@for="SearchResult.GetDirectoryEntry"]/*' />
        /// <devdoc>
        /// <para>Retrieves the <see cref='System.DirectoryServices.DirectoryEntry'/> that corresponds to the <see cref='System.DirectoryServices.SearchResult'/>, from the Active Directory 
        ///    hierarchy.</para>
        /// </devdoc>
        [
            EnvironmentPermission(SecurityAction.Assert, Unrestricted = true),
            SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)
        ]
        public DirectoryEntry GetDirectoryEntry()
        {
            if (_parentCredentials != null)
                return new DirectoryEntry(Path, true, _parentCredentials.UserName, _parentCredentials.Password, _parentAuthenticationType);
            else
            {
                DirectoryEntry newEntry = new DirectoryEntry(Path, true, null, null, _parentAuthenticationType);
                return newEntry;
            }
        }

        /// <include file='doc\SearchResult.uex' path='docs/doc[@for="SearchResult.Path"]/*' />
        /// <devdoc>
        /// <para> Gets the path for this <see cref='System.DirectoryServices.SearchResult'/>.</para>
        /// </devdoc>
        public string Path
        {
            get
            {
                return (string)Properties["ADsPath"][0];
            }
        }

        /// <include file='doc\SearchResult.uex' path='docs/doc[@for="SearchResult.Properties"]/*' />
        /// <devdoc>
        /// <para>Gets a <see cref='System.DirectoryServices.ResultPropertyCollection'/>
        /// of properties set on this object.</para>
        /// </devdoc>
        public ResultPropertyCollection Properties { get; } = new ResultPropertyCollection();
    }
}
