// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace System.Net.Security
{
    public sealed partial class CipherSuitesPolicy
    {
        internal uint[] TlsCipherSuites { get; private set; }

        private void Initialize(IEnumerable<TlsCipherSuite> allowedCipherSuites)
        {
            TlsCipherSuites = allowedCipherSuites.Select((cs) => (uint)cs).ToArray();
        }

        private IEnumerable<TlsCipherSuite> GetCipherSuites()
        {
            return TlsCipherSuites.Select((cs) => (TlsCipherSuite)cs);
        }
    }
}
