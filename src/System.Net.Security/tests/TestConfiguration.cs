namespace NCLTest.Security
{
    using System;
    using System.Diagnostics;
    using System.Security.Authentication;

    internal static class TestConfiguration
    {
        public const SslProtocols DefaultSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
    }
}
