// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Net.Security
{
    public sealed class CipherSuitesPolicy
    {
        [CLSCompliant(false)]
        public CipherSuitesPolicy(IEnumerable<TlsCipherSuite> allowedCipherSuites) => throw new PlatformNotSupportedException(SR.net_ssl_ciphersuites_policy_not_supported);
        [CLSCompliant(false)]
        public IEnumerable<TlsCipherSuite> AllowedCipherSuites => throw new PlatformNotSupportedException(SR.net_ssl_ciphersuites_policy_not_supported);
    }
}
