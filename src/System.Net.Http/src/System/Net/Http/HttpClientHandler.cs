// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Http
{
    public partial class HttpClientHandler : HttpMessageHandler
    {
        // This partial implementation contains members common to all HttpClientHandler implementations.
        private const string SocketsHttpHandlerEnvironmentVariableSettingName = "DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER";
        private const string SocketsHttpHandlerAppCtxSettingName = "System.Net.Http.UseSocketsHttpHandler";

        private static bool UseSocketsHttpHandler
        {
            get
            {
                // First check for the AppContext switch, giving it priority over over the environment variable.
                if (AppContext.TryGetSwitch(SocketsHttpHandlerAppCtxSettingName, out bool useSocketsHttpHandler))
                {
                    return useSocketsHttpHandler;
                }

                // AppContext switch wasn't used. Check the environment variable to see if it's been set to true.
                string envVar = Environment.GetEnvironmentVariable(SocketsHttpHandlerEnvironmentVariableSettingName);
                if (envVar != null && (envVar.Equals("true", StringComparison.OrdinalIgnoreCase) || envVar.Equals("1")))
                {
                    return true;
                }

                // Default to using WinHttpHandler on Windows and CurlHandler on Unix
                return false;
            }
        }

        public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> DangerousAcceptAnyServerCertificateValidator { get; } = delegate { return true; };
    }
}
