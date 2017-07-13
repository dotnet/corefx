// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.DirectoryServices.Interop;

namespace System.DirectoryServices
{
    /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies what kind of acknowledgment to get after sending a message.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum AuthenticationTypes
    {
        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.None"]/*' />
        None = 0,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.Secure"]/*' />
        /// <devdoc>
        ///     Requests secure authentication. When this flag is set, the WinNT provider uses NT LAN Manager (NTLM) 
        ///     to authenticate the client. Active Directory will use Kerberos, and possibly NTLM, to authenticate the client. 
        /// </devdoc>
        Secure = NativeMethods.AuthenticationModes.SecureAuthentication,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.Encription"]/*' />
        /// <devdoc>
        ///     Forces ADSI to use encryption for data exchange over the network. 
        /// </devdoc>
        Encryption = NativeMethods.AuthenticationModes.UseEncryption,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.SecureSocketsLayer"]/*' />
        /// <devdoc>
        ///     Encrypts the channel with SSL. Data will be encrypted using SSL. Active Directory requires that the 
        ///     Certificate Server be installed to support SSL encryption. 
        /// </devdoc>
        SecureSocketsLayer = NativeMethods.AuthenticationModes.UseSSL,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.ReadonlyServer"]/*' />
        /// <devdoc>
        ///     For a WinNT provider, ADSI tries to connect to a primary domain controller or a backup domain 
        ///     controller. For Active Directory, this flag indicates that a writeable server is not required for a 
        ///     serverless binding. 
        /// </devdoc>
        ReadonlyServer = NativeMethods.AuthenticationModes.ReadonlyServer,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.Anonymous"]/*' />
        /// <devdoc>
        ///     Request no authentication. The providers may attempt to bind client, as an anonymous user, to the targeted 
        ///     object. The WinNT provider does not support this flag. Active Directory establishes a connection between 
        ///     the client and the targeted object, but will not perform any authentication. Setting this flag amounts to 
        ///     requesting an anonymous binding, which means "Everyone" as the security context. 
        /// </devdoc>
        Anonymous = NativeMethods.AuthenticationModes.NoAuthentication,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.FastBind"]/*' />
        /// <devdoc>
        ///     When this flag is set, ADSI will not attempt to query the objectClass property and thus will only expose 
        ///     the base interfaces supported by all ADSI objects instead of the full object support. 
        /// </devdoc>
        FastBind = NativeMethods.AuthenticationModes.FastBind,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.Signing"]/*' />
        /// <devdoc>
        ///     Verifies data integrity to ensure the data received is the same as the data sent. The Secure flag 
        ///     must be set also in order to use the signing. 
        /// </devdoc>
        Signing = NativeMethods.AuthenticationModes.UseSigning,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.Sealing"]/*' />
        /// <devdoc>
        ///     Encrypts data using Kerberos. The Secure flag must be set also in order to use the sealing. 
        /// </devdoc>
        Sealing = NativeMethods.AuthenticationModes.UseSealing,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.Delegation"]/*' />
        /// <devdoc>
        ///     Enables ADSI to delegate the user's security context, which is necessary for moving objects across domains. 
        /// </devdoc>
        Delegation = NativeMethods.AuthenticationModes.UseDelegation,

        /// <include file='doc\AuthenticationTypes.uex' path='docs/doc[@for="AuthenticationTypes.ServerBind"]/*' />
        /// <devdoc>
        ///     Specify this flag when using the LDAP provider if your ADsPath includes a server name. Do not use 
        ///     this flag for paths that include a domain name or for serverless paths.
        /// </devdoc>
        ServerBind = NativeMethods.AuthenticationModes.UseServerBinding
    }
}

