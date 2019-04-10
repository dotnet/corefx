// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Net.Security
{
    public sealed partial class CipherSuitesPolicy
    {
        private void Initialize(IEnumerable<TlsCipherSuite> allowedCipherSuites) => throw new PlatformNotSupportedException(SR.net_ssl_ciphersuites_policy_not_supported);
        private IEnumerable<TlsCipherSuite> GetCipherSuites() => null;
    }
}
