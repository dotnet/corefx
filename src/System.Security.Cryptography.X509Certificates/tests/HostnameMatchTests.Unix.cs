// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public static class HostnameMatchTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public static void MatchCN_NoWildcards(bool wantsWildcard, bool mixedCase)
        {
            string targetName = "LocalHost.loCAldoMaIn";
            string subjectCN = wantsWildcard ? "*.LOcaLdomain" : targetName;

            RunTest(targetName, subjectCN, null, !mixedCase, !wantsWildcard);
        }

        [Theory]
        [InlineData("Capitalized.SomeDomain.TLD", false, true)]
        [InlineData("Capitalized.SomeDomain.TLD", true, true)]
        [InlineData("Too.Many.SomeDomain.TLD", false, false)]
        [InlineData("Too.Many.SomeDomain.TLD", true, false)]
        [InlineData("Now.Lower.SomeDomain.TLD", false, true)]
        [InlineData("Now.Lower.SomeDomain.TLD", true, true)]
        public static void MatchSubjectAltName(string targetName, bool mixedCase, bool expectedResult)
        {
            string[] sanEntries =
            {
                "Capitalized.SomeDomain.TLD",
                "*.SomeDomain.TLD",
                "*.lower.someDomain.Tld",
            };

            RunTest(targetName, "SAN Certificate", sanEntries, !mixedCase, expectedResult);
        }

        [Fact]
        public static void SubjectAltName_NoFallback()
        {
            string[] sanEntries =
            {
                "reference.example.org",
                "other.example.org",
                "reference.example",
            };

            RunTest("www.example.org", "www.example.org", sanEntries, false, false);
        }

        private static void RunTest(
            string targetName,
            string subjectCN,
            IList<string> sanDnsNames,
            bool flattenCase,
            bool expectedResult)
        {
            using (RSA rsa = RSA.Create(TestData.RsaBigExponentParams))
            {
                CertificateRequest request = new CertificateRequest(
                    $"CN={FixCase(subjectCN, flattenCase)}, O=.NET Framework (CoreFX)",
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature,
                        false));

                if (sanDnsNames != null)
                {
                    var builder = new SubjectAlternativeNameBuilder();

                    foreach (string sanDnsName in sanDnsNames)
                    {
                        builder.AddDnsName(FixCase(sanDnsName, flattenCase));
                    }

                    X509Extension extension = builder.Build();
                    Console.WriteLine(extension.ToString());
                    request.CertificateExtensions.Add(extension);
                }

                DateTimeOffset start = DateTimeOffset.UtcNow.AddYears(-1);
                DateTimeOffset end = start.AddYears(1);

                using (X509Certificate2 cert = request.CreateSelfSigned(start, end))
                {
                    bool isMatch = CheckHostname(cert, targetName);
                    string lowerTarget = targetName.ToLowerInvariant();
                    bool isLowerMatch = CheckHostname(cert, lowerTarget);

                    if (expectedResult)
                    {
                        Assert.True(isMatch, $"{targetName} matches");
                        Assert.True(isLowerMatch, $"{lowerTarget} (lowercase) matches");
                    }
                    else
                    {
                        Assert.False(isMatch, $"{targetName} matches");
                        Assert.False(isLowerMatch, $"{lowerTarget} (lowercase) matches");
                    }
                }
            }
        }

        private static string FixCase(string input, bool flatten)
        {
            return flatten ? input.ToLowerInvariant() : input;
        }

        private static bool CheckHostname(X509Certificate2 cert, string targetName)
        {
            int value = CheckX509Hostname(cert.Handle, targetName, targetName.Length);
            GC.KeepAlive(cert);
            Assert.InRange(value, 0, 1);
            return value != 0;
        }

        [DllImport(Interop.Libraries.CryptoNative, EntryPoint = "CryptoNative_CheckX509Hostname")]
        private static extern int CheckX509Hostname(IntPtr x509, string hostname, int cchHostname);
    }
}
