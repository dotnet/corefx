// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed partial class HttpProxyConnectionHandler : HttpMessageHandler
    {
        static private IWebProxy GetDefaultroxy()
        {
            // Return Environmental proxy now for testing
            // Windows normally does not use environment but it has system
            // configuration. That has not been implemented yet.
            // TODO #23150: windows portion
            new HttpEnvironmentProxy();
        }
    }
}

