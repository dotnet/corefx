// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Security.Cryptography.Algorithms.Tests
{
    public class DSATests
    {
        [Fact]
        public void BaseVirtualsNotImplementedException()
        {
            var dsa = new EmptyDSA();
            Assert.Throws<NotImplementedException>(() => dsa.HashData(null, HashAlgorithmName.SHA256));
            Assert.Throws<NotImplementedException>(() => dsa.HashData(null, 0, 0, HashAlgorithmName.SHA256));
        }

        private sealed class EmptyDSA : DSA
        {
            public override byte[] CreateSignature(byte[] rgbHash) => throw new NotImplementedException();
            public override void ImportParameters(DSAParameters parameters) => throw new NotImplementedException();
            public override DSAParameters ExportParameters(bool includePrivateParameters) => throw new NotImplementedException();
            public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature) => throw new NotImplementedException();

            public new byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm) =>
                base.HashData(data, offset, count, hashAlgorithm);
            public new byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm) =>
                base.HashData(data, hashAlgorithm);
            public new bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten) =>
                base.TryHashData(source, destination, hashAlgorithm, out bytesWritten);
        }
    }
}
