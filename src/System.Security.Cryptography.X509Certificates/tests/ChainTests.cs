// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.X509Certificates.Tests
{
    public static class ChainTests
    {
        internal static bool CanModifyStores { get; } = TestEnvironmentConfiguration.CanModifyStores;

        private static bool TrustsMicrosoftDotComRoot
        {
            get
            {
                // Verifies that the microsoft.com certs build with only the certificates in the root store

                using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
                using (var chainHolder = new ChainHolder())
                {
                    X509Chain chain = chainHolder.Chain;
                    chain.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                    return chain.Build(microsoftDotCom);
                }
            }
        }

        [Fact]
        public static void BuildChain()
        {
            using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
            using (var microsoftDotComIssuer = new X509Certificate2(TestData.MicrosoftDotComIssuerBytes))
            using (var microsoftDotComRoot = new X509Certificate2(TestData.MicrosoftDotComRootBytes))
            using (var unrelated = new X509Certificate2(TestData.DssCer))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                chain.ChainPolicy.ExtraStore.Add(unrelated);
                chain.ChainPolicy.ExtraStore.Add(microsoftDotComRoot);
                chain.ChainPolicy.ExtraStore.Add(microsoftDotComIssuer);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

                // Halfway between microsoftDotCom's NotBefore and NotAfter
                // This isn't a boundary condition test.
                chain.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                bool valid = chain.Build(microsoftDotCom);
                Assert.True(valid, "Chain built validly");

                // The chain should have 3 members
                Assert.Equal(3, chain.ChainElements.Count);

                // These are the three specific members.
                Assert.Equal(microsoftDotCom, chain.ChainElements[0].Certificate);
                Assert.Equal(microsoftDotComIssuer, chain.ChainElements[1].Certificate);
                Assert.Equal(microsoftDotComRoot, chain.ChainElements[2].Certificate);
            }
        }

        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        public static void VerifyChainFromHandle()
        {
            using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
            using (var microsoftDotComIssuer = new X509Certificate2(TestData.MicrosoftDotComIssuerBytes))
            using (var microsoftDotComRoot = new X509Certificate2(TestData.MicrosoftDotComRootBytes))
            using (var unrelated = new X509Certificate2(TestData.DssCer))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                chain.ChainPolicy.ExtraStore.Add(unrelated);
                chain.ChainPolicy.ExtraStore.Add(microsoftDotComRoot);
                chain.ChainPolicy.ExtraStore.Add(microsoftDotComIssuer);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                bool valid = chain.Build(microsoftDotCom);
                Assert.True(valid, "Source chain built validly");
                Assert.Equal(3, chain.ChainElements.Count);

                using (var chainHolder2 = new ChainHolder(chain.ChainContext))
                {
                    X509Chain chain2 = chainHolder2.Chain;
                    Assert.NotSame(chain, chain2);
                    Assert.Equal(chain.ChainContext, chain2.ChainContext);

                    Assert.Equal(3, chain2.ChainElements.Count);

                    Assert.NotSame(chain.ChainElements[0], chain2.ChainElements[0]);
                    Assert.NotSame(chain.ChainElements[1], chain2.ChainElements[1]);
                    Assert.NotSame(chain.ChainElements[2], chain2.ChainElements[2]);

                    Assert.Equal(microsoftDotCom, chain2.ChainElements[0].Certificate);
                    Assert.Equal(microsoftDotComIssuer, chain2.ChainElements[1].Certificate);
                    Assert.Equal(microsoftDotComRoot, chain2.ChainElements[2].Certificate);

                    // ChainPolicy is not carried over from the Chain(IntPtr) constructor
                    Assert.NotEqual(chain.ChainPolicy.VerificationFlags, chain2.ChainPolicy.VerificationFlags);
                    Assert.NotEqual(chain.ChainPolicy.VerificationTime, chain2.ChainPolicy.VerificationTime);
                    Assert.NotEqual(chain.ChainPolicy.RevocationMode, chain2.ChainPolicy.RevocationMode);
                    Assert.Equal(X509VerificationFlags.NoFlag, chain2.ChainPolicy.VerificationFlags);

                    // Re-set the ChainPolicy properties
                    chain2.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                    chain2.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                    chain2.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                    valid = chain2.Build(microsoftDotCom);
                    Assert.True(valid, "Cloned chain built validly");
                }
            }
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]
        [ConditionalFact(nameof(CanModifyStores))]
        public static void VerifyChainFromHandle_Unix()
        {
            using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                bool valid = chain.Build(microsoftDotCom);

                Assert.Equal(IntPtr.Zero, chain.ChainContext);
            }

            Assert.Throws<PlatformNotSupportedException>(() => new X509Chain(IntPtr.Zero));
        }

        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        public static void TestDispose()
        {
            X509Chain chain;

            using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
            using (var chainHolder = new ChainHolder())
            {
                chain = chainHolder.Chain;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.Build(microsoftDotCom);

                Assert.NotEqual(IntPtr.Zero, chain.ChainContext);
            }
            // No exception thrown for accessing ChainContext on disposed chain
            Assert.Equal(IntPtr.Zero, chain.ChainContext);
        }

        [Fact]
        public static void TestResetMethod()
        {
            using (var sampleCert = new X509Certificate2(TestData.DssCer))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                chain.ChainPolicy.ExtraStore.Add(sampleCert);
                bool valid = chain.Build(sampleCert);
                Assert.False(valid);

                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                valid = chain.Build(sampleCert);
                Assert.True(valid, "Chain built validly");

                Assert.Equal(1, chain.ChainElements.Count);

                chain.Reset();
                Assert.Equal(0, chain.ChainElements.Count);

                // ChainPolicy did not reset (for desktop compat)
                Assert.Equal(X509VerificationFlags.AllowUnknownCertificateAuthority, chain.ChainPolicy.VerificationFlags);

                valid = chain.Build(sampleCert);
                Assert.Equal(1, chain.ChainElements.Count);
                // This succeeds because ChainPolicy did not reset
                Assert.True(valid, "Chain built validly after reset");
            }
        }

        /// <summary>
        /// Tests that when a certificate chain has a root certification which is not trusted by the trust provider,
        /// Build returns false and a ChainStatus returns UntrustedRoot
        /// </summary>
        [Fact]
        [OuterLoop]
        public static void BuildChainExtraStoreUntrustedRoot()
        {
            using (var testCert = new X509Certificate2(Path.Combine("TestData", "test.pfx"), TestData.ChainPfxPassword))
            using (ImportedCollection ic = Cert.Import(Path.Combine("TestData", "test.pfx"), TestData.ChainPfxPassword, X509KeyStorageFlags.DefaultKeySet))
            using (var chainHolder = new ChainHolder())
            {
                X509Certificate2Collection collection = ic.Collection;

                X509Chain chain = chainHolder.Chain;
                chain.ChainPolicy.ExtraStore.AddRange(collection);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationTime = new DateTime(2015, 9, 22, 12, 25, 0);

                bool valid = chain.Build(testCert);

                Assert.False(valid);
                Assert.Contains(chain.ChainStatus, s => s.Status == X509ChainStatusFlags.UntrustedRoot);
            }
        }

        public static IEnumerable<object[]> VerifyExpressionData()
        {
            // The test will be using the chain for TestData.MicrosoftDotComSslCertBytes
            // The leaf cert (microsoft.com) is valid from 2014-10-15 00:00:00Z to 2016-10-15 23:59:59Z
            DateTime[] validTimes =
            {
                // The NotBefore value
                new DateTime(2014, 10, 15, 0, 0, 0, DateTimeKind.Utc),

                // One second before the NotAfter value
                new DateTime(2016, 10, 15, 23, 59, 58, DateTimeKind.Utc),
            };

            // The NotAfter value as a boundary condition differs on Windows and OpenSSL.
            // Windows considers it valid (<= NotAfter).
            // OpenSSL considers it invalid (< NotAfter), with a comment along the lines of
            //   "it'll be invalid in a millisecond, why bother with the <="
            // So that boundary condition is not being tested.

            DateTime[] invalidTimes =
            {
                // One second before the NotBefore time
                new DateTime(2014, 10, 14, 23, 59, 59, DateTimeKind.Utc),

                // One second after the NotAfter time
                new DateTime(2016, 10, 16, 0, 0, 0, DateTimeKind.Utc),
            };

            List<object[]> testCases = new List<object[]>((validTimes.Length + invalidTimes.Length) * 3);

            // Build (date, result, kind) tuples.  The kind is used to help describe the test case.
            // The DateTime format that xunit uses does show a difference in the DateTime itself, but
            // having the Kind be a separate parameter just helps.

            foreach (DateTime utcTime in validTimes)
            {
                DateTime local = utcTime.ToLocalTime();
                DateTime unspecified = new DateTime(local.Ticks);

                testCases.Add(new object[] { utcTime, true, utcTime.Kind });
                testCases.Add(new object[] { local, true, local.Kind });
                testCases.Add(new object[] { unspecified, true, unspecified.Kind });
            }

            foreach (DateTime utcTime in invalidTimes)
            {
                DateTime local = utcTime.ToLocalTime();
                DateTime unspecified = new DateTime(local.Ticks);

                testCases.Add(new object[] { utcTime, false, utcTime.Kind });
                testCases.Add(new object[] { local, false, local.Kind });
                testCases.Add(new object[] { unspecified, false, unspecified.Kind });
            }

            return testCases;
        }

        [Theory]
        [MemberData(nameof(VerifyExpressionData))]
        public static void VerifyExpiration_LocalTime(DateTime verificationTime, bool shouldBeValid, DateTimeKind kind)
        {
            using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
            using (var microsoftDotComIssuer = new X509Certificate2(TestData.MicrosoftDotComIssuerBytes))
            using (var microsoftDotComRoot = new X509Certificate2(TestData.MicrosoftDotComRootBytes))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                chain.ChainPolicy.ExtraStore.Add(microsoftDotComIssuer);
                chain.ChainPolicy.ExtraStore.Add(microsoftDotComRoot);

                // Ignore anything except NotTimeValid
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags & ~X509VerificationFlags.IgnoreNotTimeValid;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationTime = verificationTime;

                bool builtSuccessfully = chain.Build(microsoftDotCom);

                Assert.Equal(shouldBeValid, builtSuccessfully);

                // If we failed to build the chain, ensure that NotTimeValid is one of the reasons.
                if (!shouldBeValid)
                {
                    Assert.Contains(chain.ChainStatus, s => s.Status == X509ChainStatusFlags.NotTimeValid);
                }
            }
        }

        [Fact]
        public static void BuildChain_WithApplicationPolicy_Match()
        {
            using (var msCer = new X509Certificate2(TestData.MsCertificate))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                // Code Signing
                chain.ChainPolicy.ApplicationPolicy.Add(new Oid("1.3.6.1.5.5.7.3.3"));
                chain.ChainPolicy.VerificationTime = msCer.NotBefore.AddHours(2);
                chain.ChainPolicy.VerificationFlags =
                    X509VerificationFlags.AllowUnknownCertificateAuthority;

                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                bool valid = chain.Build(msCer);
                Assert.True(valid, "Chain built validly");
            }
        }

        [Fact]
        public static void BuildChain_WithApplicationPolicy_NoMatch()
        {
            using (var cert = new X509Certificate2(TestData.MsCertificate))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                // Gibberish.  (Code Signing + ".1")
                chain.ChainPolicy.ApplicationPolicy.Add(new Oid("1.3.6.1.5.5.7.3.3.1"));
                chain.ChainPolicy.VerificationFlags =
                    X509VerificationFlags.AllowUnknownCertificateAuthority;

                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationTime = cert.NotBefore.AddHours(2);

                bool valid = chain.Build(cert);
                Assert.False(valid, "Chain built validly");

                Assert.InRange(chain.ChainElements.Count, 1, int.MaxValue);

                Assert.NotSame(cert, chain.ChainElements[0].Certificate);
                Assert.Equal(cert, chain.ChainElements[0].Certificate);

                X509ChainStatus[] chainElementStatus = chain.ChainElements[0].ChainElementStatus;
                Assert.InRange(chainElementStatus.Length, 1, int.MaxValue);
                Assert.Contains(chainElementStatus, x => x.Status == X509ChainStatusFlags.NotValidForUsage);
            }
        }

        [Fact]
        public static void BuildChain_WithCertificatePolicy_Match()
        {
            using (var cert = new X509Certificate2(TestData.CertWithPolicies))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                // Code Signing
                chain.ChainPolicy.CertificatePolicy.Add(new Oid("2.18.19"));
                chain.ChainPolicy.VerificationFlags =
                    X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = cert.NotBefore.AddHours(2);

                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                bool valid = chain.Build(cert);
                Assert.True(valid, "Chain built validly");
            }
        }

        [Fact]
        public static void BuildChain_WithCertificatePolicy_NoMatch()
        {
            using (var cert = new X509Certificate2(TestData.CertWithPolicies))
            using (var chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;

                chain.ChainPolicy.CertificatePolicy.Add(new Oid("2.999"));
                chain.ChainPolicy.VerificationFlags =
                    X509VerificationFlags.AllowUnknownCertificateAuthority;

                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationTime = cert.NotBefore.AddHours(2);

                bool valid = chain.Build(cert);
                Assert.False(valid, "Chain built validly");

                Assert.InRange(chain.ChainElements.Count, 1, int.MaxValue);

                Assert.NotSame(cert, chain.ChainElements[0].Certificate);
                Assert.Equal(cert, chain.ChainElements[0].Certificate);

                X509ChainStatus[] chainElementStatus = chain.ChainElements[0].ChainElementStatus;
                Assert.InRange(chainElementStatus.Length, 1, int.MaxValue);
                Assert.Contains(chainElementStatus, x => x.Status == X509ChainStatusFlags.NotValidForUsage);
            }
        }

        [ConditionalFact(nameof(TrustsMicrosoftDotComRoot), nameof(CanModifyStores))]
        [OuterLoop(/* Modifies user certificate store */)]
        public static void BuildChain_MicrosoftDotCom_WithRootCertInUserAndSystemRootCertStores()
        {
            // Verifies that when the same root cert is placed in both a user and machine root certificate store, 
            // any certs chain building to that root cert will build correctly
            // 
            // We use a copy of the microsoft.com SSL certs and root certs to validate that the chain can build 
            // successfully

            bool shouldInstallCertToUserStore = true;
            bool installedCertToUserStore = false;

            using (var microsoftDotCom = new X509Certificate2(TestData.MicrosoftDotComSslCertBytes))
            using (var microsoftDotComRoot = new X509Certificate2(TestData.MicrosoftDotComRootBytes))
            {
                // Check that microsoft.com's root certificate IS installed in the machine root store as a sanity step
                using (var machineRootStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    machineRootStore.Open(OpenFlags.ReadOnly);
                    bool foundCert = false;

                    foreach (var machineCert in machineRootStore.Certificates)
                    {
                        if (machineCert.Equals(microsoftDotComRoot))
                        {
                            foundCert = true;
                        }

                        machineCert.Dispose();
                    }

                    Assert.True(foundCert, string.Format("Did not find expected certificate with thumbprint '{0}' in the machine root store", microsoftDotComRoot.Thumbprint));
                }

                // Concievably at this point there could still be something wrong and we still don't chain build correctly - if that's 
                // the case, then there's likely something wrong with the machine. Validating that happy path is out of scope 
                // of this particular test. 

                // Check that microsoft.com's root certificate is NOT installed on in the user cert store as a sanity step
                // We won't try to install the microsoft.com root cert into the user root store if it's already there
                using (var userRootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
                {
                    userRootStore.Open(OpenFlags.ReadOnly);

                    foreach (var userCert in userRootStore.Certificates)
                    {
                        bool foundCert = false;
                        if (userCert.Equals(microsoftDotComRoot))
                        {
                            foundCert = true;
                        }

                        userCert.Dispose();

                        if (foundCert)
                        {
                            shouldInstallCertToUserStore = false;
                        }
                    }
                }

                using (var userRootStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
                using (var chainHolder = new ChainHolder())
                {
                    try
                    {
                        if (shouldInstallCertToUserStore)
                        {
                            try
                            {
                                userRootStore.Open(OpenFlags.ReadWrite);
                            }
                            catch (CryptographicException)
                            {
                                return;
                            }

                            userRootStore.Add(microsoftDotComRoot); // throws CryptographicException
                            installedCertToUserStore = true;
                        }

                        X509Chain chainValidator = chainHolder.Chain;
                        chainValidator.ChainPolicy.VerificationTime = new DateTime(2015, 10, 15, 12, 01, 01, DateTimeKind.Local);
                        chainValidator.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                        bool chainBuildResult = chainValidator.Build(microsoftDotCom);

                        StringBuilder builder = new StringBuilder();
                        foreach (var status in chainValidator.ChainStatus)
                        {
                            builder.AppendFormat("{0} {1}{2}", status.Status, status.StatusInformation, Environment.NewLine);
                        }

                        Assert.True(chainBuildResult,
                            string.Format("Certificate chain build failed. ChainStatus is:{0}{1}", Environment.NewLine, builder.ToString()));
                    }
                    finally
                    {
                        if (installedCertToUserStore)
                        {
                            userRootStore.Remove(microsoftDotComRoot);
                        }
                    }
                }
            }
        }

        [Fact]
        [OuterLoop( /* May require using the network, to download CRLs and intermediates */)]
        public static void VerifyWithRevocation()
        {
            using (var cert = new X509Certificate2(Path.Combine("TestData", "MS.cer")))
            using (var onlineChainHolder = new ChainHolder())
            using (var offlineChainHolder = new ChainHolder())
            {
                X509Chain onlineChain = onlineChainHolder.Chain;
                X509Chain offlineChain = offlineChainHolder.Chain;

                onlineChain.ChainPolicy.VerificationFlags =
                    X509VerificationFlags.AllowUnknownCertificateAuthority;

                onlineChain.ChainPolicy.VerificationTime = cert.NotBefore.AddHours(2);
                onlineChain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                onlineChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;

                // Attempt the online test a couple of times, in case there was just a CRL
                // download failure.
                const int RetryLimit = 3;
                bool valid = false;

                for (int i = 0; i < RetryLimit; i++)
                {
                    valid = onlineChain.Build(cert);

                    if (valid)
                    {
                        break;
                    }

                    for (int j = 0; j < onlineChain.ChainElements.Count; j++)
                    {
                        X509ChainElement chainElement = onlineChain.ChainElements[j];

                        // Since `NoError` gets mapped as the empty array, just look for non-empty arrays
                        if (chainElement.ChainElementStatus.Length > 0)
                        {
                            X509ChainStatusFlags allFlags = chainElement.ChainElementStatus.Aggregate(
                                X509ChainStatusFlags.NoError,
                                (cur, status) => cur | status.Status);

                            Console.WriteLine(
                                $"{nameof(VerifyWithRevocation)}: online attempt {i} - errors at depth {j}: {allFlags}");
                        }

                        chainElement.Certificate.Dispose();
                    }

                    Thread.Sleep(1000); // For network flakiness
                }

                if (TestEnvironmentConfiguration.RunManualTests)
                {
                    Assert.True(valid, $"Online Chain Built Validly within {RetryLimit} tries");
                }
                else if (!valid)
                {
                    Console.WriteLine($"SKIP [{nameof(VerifyWithRevocation)}]: Chain failed to build within {RetryLimit} tries.");
                    return;
                }

                // Since the network was enabled, we should get the whole chain.
                Assert.Equal(3, onlineChain.ChainElements.Count);

                Assert.Equal(0, onlineChain.ChainElements[0].ChainElementStatus.Length);
                Assert.Equal(0, onlineChain.ChainElements[1].ChainElementStatus.Length);

                // The root CA is not expected to be installed on everyone's machines,
                // so allow for it to report UntrustedRoot, but nothing else..
                X509ChainStatus[] rootElementStatus = onlineChain.ChainElements[2].ChainElementStatus;
                
                if (rootElementStatus.Length != 0)
                {
                    Assert.Equal(1, rootElementStatus.Length);
                    Assert.Equal(X509ChainStatusFlags.UntrustedRoot, rootElementStatus[0].Status);
                }

                // Now that everything is cached, try again in Offline mode.
                offlineChain.ChainPolicy.VerificationFlags = onlineChain.ChainPolicy.VerificationFlags;
                offlineChain.ChainPolicy.VerificationTime = onlineChain.ChainPolicy.VerificationTime;
                offlineChain.ChainPolicy.RevocationMode = X509RevocationMode.Offline;
                offlineChain.ChainPolicy.RevocationFlag = onlineChain.ChainPolicy.RevocationFlag;

                valid = offlineChain.Build(cert);
                Assert.True(valid, "Offline Chain Built Validly");

                // Everything should look just like the online chain:
                Assert.Equal(onlineChain.ChainElements.Count, offlineChain.ChainElements.Count);

                for (int i = 0; i < offlineChain.ChainElements.Count; i++)
                {
                    X509ChainElement onlineElement = onlineChain.ChainElements[i];
                    X509ChainElement offlineElement = offlineChain.ChainElements[i];

                    Assert.Equal(onlineElement.ChainElementStatus, offlineElement.ChainElementStatus);
                    Assert.Equal(onlineElement.Certificate, offlineElement.Certificate);
                }
            }
        }

        [Fact]
        public static void Create()
        {
            using (var chain = X509Chain.Create())
                Assert.NotNull(chain);
        }

        [Fact]
        public static void BuildChainInvalidValues()
        {
            using (var chain = X509Chain.Create())
            {
                AssertExtensions.Throws<ArgumentException>("certificate", () => chain.Build(null));
                AssertExtensions.Throws<ArgumentException>("certificate", () => chain.Build(new X509Certificate2()));
            }
        }

        [Fact]
        public static void InvalidSelfSignedSignature()
        {
            X509ChainStatusFlags expectedFlags;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                expectedFlags = X509ChainStatusFlags.NotSignatureValid;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // For OSX alone expectedFlags here means OR instead of AND.
                // Because the error code changed in 10.13.4 from UntrustedRoot to PartialChain
                // and we handle that later in this test.
                expectedFlags =
                    X509ChainStatusFlags.UntrustedRoot |
                    X509ChainStatusFlags.PartialChain;
            }
            else
            {
                expectedFlags =
                    X509ChainStatusFlags.NotSignatureValid |
                    X509ChainStatusFlags.UntrustedRoot;
            }

            byte[] certBytes = (byte[])TestData.MicrosoftDotComRootBytes.Clone();
            // The signature goes up to the very last byte, so flip some bits in it.
            certBytes[certBytes.Length - 1] ^= 0xFF;

            using (var cert = new X509Certificate2(certBytes))
            using (ChainHolder holder = new ChainHolder())
            {
                X509Chain chain = holder.Chain;
                X509ChainPolicy policy = chain.ChainPolicy;
                policy.VerificationTime = cert.NotBefore.AddDays(3);
                policy.RevocationMode = X509RevocationMode.NoCheck;

                chain.Build(cert);

                X509ChainStatusFlags allFlags =
                    chain.ChainStatus.Select(cs => cs.Status).Aggregate(
                        X509ChainStatusFlags.NoError,
                        (a, b) => a | b);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // If we're on 10.13.3 or older we get UntrustedRoot.
                    // If we're on 10.13.4 or newer we get PartialChain.
                    //
                    // So make the expectedValue be whichever of those two is set.
                    expectedFlags = (expectedFlags & allFlags);
                    // One of them has to be set.
                    Assert.NotEqual(X509ChainStatusFlags.NoError, expectedFlags);
                    // Continue executing now to ensure that no other unexpected flags were set.
                }

                Assert.Equal(expectedFlags, allFlags);
            }
        }

        [Fact]
        public static void ChainWithEmptySubject()
        {
            using (var cert = new X509Certificate2(TestData.EmptySubjectCertificate))
            using (var issuer = new X509Certificate2(TestData.EmptySubjectIssuerCertificate))
            using (ChainHolder chainHolder = new ChainHolder())
            {
                X509Chain chain = chainHolder.Chain;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationFlags |= X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.ExtraStore.Add(issuer);

                Assert.True(chain.Build(cert), "chain.Build(cert)");
                Assert.Equal(2, chain.ChainElements.Count);
                Assert.Equal(string.Empty, cert.Subject);
                Assert.Equal(cert.RawData, chain.ChainElements[0].Certificate.RawData);
                Assert.Equal(issuer.RawData, chain.ChainElements[1].Certificate.RawData);
            }
        }
    }
}
