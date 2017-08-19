// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Security.Cryptography
{
    //
    // If you change this file, make the corresponding changes to all of the SHA*CryptoServiceProvider.cs files.
    //
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SHA256CryptoServiceProvider : SHA256
    {
        private const int HashSizeBits = 256;
        private readonly IncrementalHash _incrementalHash;

        public SHA256CryptoServiceProvider()
        {
            _incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            HashSizeValue = HashSizeBits;
        }

        public override void Initialize()
        {
            // Nothing to do here. We expect HashAlgorithm to invoke HashFinal() and Initialize() as a pair. This reflects the 
            // reality that our native crypto providers (e.g. CNG) expose hash finalization and object reinitialization as an atomic operation.
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize) =>
            _incrementalHash.AppendData(array, ibStart, cbSize);

        protected override void HashCore(ReadOnlySpan<byte> source) =>
            _incrementalHash.AppendData(source);

        protected override byte[] HashFinal() =>
            _incrementalHash.GetHashAndReset();

        protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten) =>
            _incrementalHash.TryGetHashAndReset(destination, out bytesWritten);

        // The Hash and HashSize properties are not overridden since the correct values are returned from base.

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _incrementalHash.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
