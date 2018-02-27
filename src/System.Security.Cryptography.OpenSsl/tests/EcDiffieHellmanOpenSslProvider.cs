// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography.EcDiffieHellman.Tests
{
    public partial class ECDiffieHellmanProvider : IECDiffieHellmanProvider
    {
        private EcDsa.Tests.ECDsaProvider _ecdsaProvider = new EcDsa.Tests.ECDsaProvider();

        public ECDiffieHellman Create()
        {
            return new ECDiffieHellmanOpenSsl();
        }

        public ECDiffieHellman Create(int keySize)
        {
            return new ECDiffieHellmanOpenSsl(keySize);
        }

        public ECDiffieHellman Create(ECCurve curve)
        {
            return new ECDiffieHellmanOpenSsl(curve);
        }

        public bool IsCurveValid(Oid oid) => _ecdsaProvider.IsCurveValid(oid);

        public bool ExplicitCurvesSupported => _ecdsaProvider.ExplicitCurvesSupported;
    }

    public partial class ECDiffieHellmanFactory
    {
        private static readonly IECDiffieHellmanProvider s_provider = new ECDiffieHellmanProvider();
    }
}

