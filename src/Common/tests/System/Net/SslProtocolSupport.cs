// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace System.Net.Test.Common
{
    public class SslProtocolSupport
    {
        public const SslProtocols DefaultSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

        public static SslProtocols SupportedSslProtocols
        {
            get
            {
                SslProtocols supported = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
#pragma warning disable 0618 // SSL2/3 are deprecated
                if (PlatformDetection.IsWindows ||
                    PlatformDetection.IsOSX ||
                    (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && PlatformDetection.OpenSslVersion < new Version(1, 0, 2) && !PlatformDetection.IsDebian))
                {
                    supported |= SslProtocols.Ssl3;
                }
#pragma warning restore 0618
                return supported;
            }
        }

        public class SupportedSslProtocolsTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (SslProtocols protocol in Enum.GetValues(typeof(SslProtocols)))
                {
                    if (protocol != 0 && (protocol & SupportedSslProtocols) == protocol)
                    {
                        yield return new object[] { protocol };
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
