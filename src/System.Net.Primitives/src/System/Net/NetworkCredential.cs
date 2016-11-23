// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System.Net
{
    /// <devdoc>
    ///    <para>
    ///       Provides credentials for password-based authentication schemes such as basic,
    ///       digest, NTLM and Kerberos.
    ///    </para>
    /// </devdoc>
    public class NetworkCredential : ICredentials, ICredentialsByHost
    {
        private string _domain;
        private string _userName;
        private string _password;
        private SecureString _securePassword;

        public NetworkCredential()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Net.NetworkCredential'/>
        ///       class with name and password set as specified.
        ///    </para>
        /// </devdoc>
        public NetworkCredential(string userName, string password)
        : this(userName, password, string.Empty)
        {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Net.NetworkCredential'/>
        ///       class with name, password and domain set as specified.
        ///    </para>
        /// </devdoc>
        public NetworkCredential(string userName, string password, string domain)
        {
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        [CLSCompliant(false)]
        public NetworkCredential(string userName, SecureString password)
        : this(userName, password, string.Empty)
        {
        }

        [CLSCompliant(false)]
        public NetworkCredential(string userName, SecureString password, string domain)
        {
            UserName = userName;
            SecurePassword = password;
            Domain = domain;
        }

        /// <devdoc>
        ///    <para>
        ///       The user name associated with this credential.
        ///    </para>
        /// </devdoc>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value ?? string.Empty; }
        }

        /// <devdoc>
        ///    <para>
        ///       The password for the user name.
        ///    </para>
        /// </devdoc>
        public string Password
        {
            get { return _password; }
            set { _password = value ?? string.Empty; }
        }

        [CLSCompliant(false)]
        public SecureString SecurePassword
        {
            get { return _securePassword.Copy(); } 
            set { _securePassword = value != null ? value.Copy() : new SecureString(); }
        }

        /// <devdoc>
        ///    <para>
        ///       The machine name that verifies
        ///       the credentials. Usually this is the host machine.
        ///    </para>
        /// </devdoc>
        public string Domain
        {
            get { return _domain; }
            set { _domain = value ?? string.Empty; }
        }

        internal string InternalGetDomainUserName()
        {
            return _domain != "" ? _domain + "\\" + _userName : _userName;
        }

        /// <devdoc>
        ///    <para>
        ///       Returns an instance of the NetworkCredential class for a Uri and
        ///       authentication type.
        ///    </para>
        /// </devdoc>
        public NetworkCredential GetCredential(Uri uri, String authenticationType)
        {
            return this;
        }

        public NetworkCredential GetCredential(string host, int port, String authenticationType)
        {
            return this;
        }

#if DEBUG
        // This method is only called as part of an assert
        internal bool IsEqualTo(object compObject)
        {
            if ((object)compObject == null)
            {
                return false;
            }

            if ((object)this == (object)compObject)
            {
                return true;
            }

            NetworkCredential compCred = compObject as NetworkCredential;
            if ((object)compCred == null)
            {
                return false;
            }

            return (_userName == compCred.UserName &&
                    _domain == compCred.Domain &&
                    string.Equals(_password, compCred.Password, StringComparison.Ordinal));
        }
#endif
    }
}
