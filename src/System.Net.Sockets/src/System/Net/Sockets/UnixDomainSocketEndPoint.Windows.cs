// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Sockets
{
    /// <summary>Represents a Unix Domain Socket endpoint as a path.</summary>
    public sealed partial class UnixDomainSocketEndPoint : EndPoint
    {
        private static readonly int s_nativePathOffset = 2; // sizeof(sun_family)
        private static readonly int s_nativePathLength = 108; // sizeof(sun_path)
        private static readonly int s_nativeAddressSize = s_nativePathOffset + s_nativePathLength; // sizeof(sockaddr_un)

        private SocketAddress CreateSocketAddressForSerialize() =>
            new SocketAddress(AddressFamily.Unix, s_nativeAddressSize);

        // from afunix.h:
        //#define UNIX_PATH_MAX 108
        //typedef struct sockaddr_un
        //{
        //    ADDRESS_FAMILY sun_family;     /* AF_UNIX */
        //    char sun_path[UNIX_PATH_MAX];  /* pathname */
        //}
        //SOCKADDR_UN, *PSOCKADDR_UN;
    }
}
