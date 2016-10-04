// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
    public abstract class AsymmetricKeyExchangeFormatter
    {
        protected AsymmetricKeyExchangeFormatter() {}

        public abstract string Parameters {get;}

        public abstract void SetKey(AsymmetricAlgorithm key);
        public abstract byte[] CreateKeyExchange(byte[] data);

        // review note: keeping this even though synAlgType is not used
        public abstract byte[] CreateKeyExchange(byte[] data, Type symAlgType);
    }
}
