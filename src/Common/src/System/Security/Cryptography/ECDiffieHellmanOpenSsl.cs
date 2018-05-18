// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    internal static partial class ECDiffieHellmanImplementation
    {
#endif
        public sealed partial class ECDiffieHellmanOpenSsl : ECDiffieHellman
        {
            private ECOpenSsl _key;

            public ECDiffieHellmanOpenSsl(ECCurve curve)
            {
                _key = new ECOpenSsl(curve);
                KeySizeValue = _key.KeySize;
            }

            public ECDiffieHellmanOpenSsl()
                : this(521)
            {
            }

            public ECDiffieHellmanOpenSsl(int keySize)
            {
                base.KeySize = keySize;
                _key = new ECOpenSsl(this);
            }
            
            public override KeySizes[] LegalKeySizes =>
                new[] {
                    new KeySizes(minSize: 256, maxSize: 384, skipSize: 128),
                    new KeySizes(minSize: 521, maxSize: 521, skipSize: 0)
                };

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _key.Dispose();
                }

                base.Dispose(disposing);
            }

            public override int KeySize
            {
                get
                {
                    return base.KeySize;
                }
                set
                {
                    if (KeySize == value)
                    {
                        return;
                    }

                    // Set the KeySize before FreeKey so that an invalid value doesn't throw away the key
                    base.KeySize = value;
                    _key?.Dispose();
                    _key = new ECOpenSsl(this);
                }
            }

            public override void GenerateKey(ECCurve curve)
            {
                KeySizeValue = _key.GenerateKey(curve);
            }

            public override ECDiffieHellmanPublicKey PublicKey =>
                new ECDiffieHellmanOpenSslPublicKey(_key.UpRefKeyHandle());

            public override void ImportParameters(ECParameters parameters)
            {
                KeySizeValue = _key.ImportParameters(parameters);
            }

            public override ECParameters ExportExplicitParameters(bool includePrivateParameters) =>
                ECOpenSsl.ExportExplicitParameters(_key.Value, includePrivateParameters);

            public override ECParameters ExportParameters(bool includePrivateParameters) =>
                ECOpenSsl.ExportParameters(_key.Value, includePrivateParameters);
        }
#if INTERNAL_ASYMMETRIC_IMPLEMENTATIONS
    }
#endif
}
