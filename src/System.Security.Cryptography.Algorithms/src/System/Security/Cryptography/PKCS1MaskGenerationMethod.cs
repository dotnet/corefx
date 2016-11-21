// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography;

namespace System.Security.Cryptography
{
    public class PKCS1MaskGenerationMethod : MaskGenerationMethod
    {
        private string _hashNameValue;
        
        public PKCS1MaskGenerationMethod()
        {
            _hashNameValue = "SHA1";
        }

        public string HashName
        {
            get { return _hashNameValue; }
            set { _hashNameValue = value ?? "SHA1"; }
        }

        public override byte[] GenerateMask(byte[] rgbSeed, int cbReturn)
        {
            HashAlgorithm hash = (HashAlgorithm)CryptoConfig.CreateFromName(_hashNameValue);
            byte[] rgbCounter = new byte[4];
            byte[] rgbT = new byte[cbReturn];

            uint counter = 0;
            for (int ib = 0; ib < rgbT.Length;)
            {
                //  Increment counter -- up to 2^32 * sizeof(Hash)
                Helpers.ConvertIntToByteArray(counter++, ref rgbCounter);
                hash.TransformBlock(rgbSeed, 0, rgbSeed.Length, rgbSeed, 0);
                hash.TransformFinalBlock(rgbCounter, 0, 4);
                byte[] _hash = hash.Hash;
                hash.Initialize();
                if (rgbT.Length - ib > _hash.Length)
                {
                    Buffer.BlockCopy(_hash, 0, rgbT, ib, _hash.Length);
                } 
                else
                {
                    Buffer.BlockCopy(_hash, 0, rgbT, ib, rgbT.Length - ib);
                }

                ib += hash.Hash.Length;
            }
            return rgbT;
        }
    }
}
