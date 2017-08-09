// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Hashing.Algorithms.Tests
{
    public abstract partial class HashAlgorithmTest
    {
        partial void Verify_Span(byte[] input, string output)
        {
            byte[] expected = ByteUtils.HexToByteArray(output);
            byte[] actual;
            int bytesWritten;

            using (HashAlgorithm hash = Create())
            {
                actual = new byte[expected.Length - 1];
                Assert.False(hash.TryComputeHash(input, actual, out bytesWritten));
                Assert.Equal(0, bytesWritten);

                actual = new byte[expected.Length];
                Assert.True(hash.TryComputeHash(input, actual, out bytesWritten));
                Assert.Equal(expected.Length, bytesWritten);
                Assert.Equal(expected, actual);
                Assert.Throws<NullReferenceException>(() => hash.Hash); // byte[] not initialized
            }
        }

        [Fact]
        public void Dispose_TryComputeHash_ThrowsException()
        {
            HashAlgorithm hash = Create();
            hash.Dispose();
            Assert.Throws<ObjectDisposedException>(() => hash.ComputeHash(new byte[1]));
            Assert.Throws<ObjectDisposedException>(() => hash.TryComputeHash(new byte[1], new byte[1], out int bytesWritten));
        }
    }
}
