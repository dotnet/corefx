// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static partial class CngCommon
    {
        public static byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
        {
            // The classes that call us are sealed and their base class has checked this already.
            Debug.Assert(data != null);
            Debug.Assert(offset >= 0 && offset <= data.Length);
            Debug.Assert(count >= 0 && count <= data.Length);
            Debug.Assert(!string.IsNullOrEmpty(hashAlgorithm.Name));

            using (HashProviderCng hashProvider = new HashProviderCng(hashAlgorithm.Name, null))
            {
                hashProvider.AppendHashData(data, offset, count);
                byte[] hash = hashProvider.FinalizeHashAndReset();
                return hash;
            }
        }

        public static byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
        {
            // The classes that call us are sealed and their base class has checked this already.
            Debug.Assert(data != null);
            Debug.Assert(!string.IsNullOrEmpty(hashAlgorithm.Name));

            using (HashProviderCng hashProvider = new HashProviderCng(hashAlgorithm.Name, null))
            {
                // Default the buffer size to 4K.
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = data.Read(buffer, 0, buffer.Length)) > 0)
                {
                    hashProvider.AppendHashData(buffer, 0, bytesRead);
                }
                byte[] hash = hashProvider.FinalizeHashAndReset();
                return hash;
            }
        }
    }
}

