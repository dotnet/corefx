// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Internal.Cryptography;
using Internal.Cryptography.Pal.Windows;
using static Interop.Crypt32;

namespace System.Security.Cryptography.Pkcs
{
    public sealed class AlgorithmIdentifier
    {
        public AlgorithmIdentifier()
            : this(Oid.FromOidValue(Oids.TripleDesCbc, OidGroup.EncryptionAlgorithm), 0)
        {
        }

        public AlgorithmIdentifier(Oid oid)
            : this(oid, 0)
        {
        }

        public AlgorithmIdentifier(Oid oid, int keyLength)
        {
            Oid = oid;
            KeyLength = keyLength;
        }

        internal AlgorithmIdentifier(CERT_PUBLIC_KEY_INFO keyInfo)
        {
            int keyLength = CertGetPublicKeyLength(MsgEncodingType.X509_ASN_ENCODING | MsgEncodingType.PKCS_7_ASN_ENCODING, ref keyInfo);
            byte[] parameters = new byte[keyInfo.Algorithm.Parameters.cbData];
            if (parameters.Length > 0)
            {
                Marshal.Copy(keyInfo.Algorithm.Parameters.pbData, parameters, 0, parameters.Length);
            }

            Reset(Oid.FromOidValue(Marshal.PtrToStringAnsi(keyInfo.Algorithm.pszObjId), OidGroup.PublicKeyAlgorithm), keyLength, parameters);
        }

        internal AlgorithmIdentifier(CRYPT_ALGORITHM_IDENTIFIER cryptAlgorithmIdentifer)
        {
            string oidValue = cryptAlgorithmIdentifer.pszObjId.ToStringAnsi();
            AlgId algId = oidValue.ToAlgId();

            int keyLength;
            byte[] parameters = Array.Empty<byte>();
            switch (algId)
            {
                case AlgId.CALG_RC2:
                    {
                        if (cryptAlgorithmIdentifer.Parameters.cbData == 0)
                        {
                            keyLength = 0;
                        }
                        else
                        {
                            CRYPT_RC2_CBC_PARAMETERS rc2Parameters;
                            unsafe
                            {
                                int cbSize = sizeof(CRYPT_RC2_CBC_PARAMETERS);
                                if (!Interop.Crypt32.CryptDecodeObject(CryptDecodeObjectStructType.PKCS_RC2_CBC_PARAMETERS, cryptAlgorithmIdentifer.Parameters.pbData, (int)(cryptAlgorithmIdentifer.Parameters.cbData), &rc2Parameters, ref cbSize))
                                    throw Marshal.GetLastWin32Error().ToCryptographicException();
                            }

                            switch (rc2Parameters.dwVersion)
                            {
                                case CryptRc2Version.CRYPT_RC2_40BIT_VERSION:
                                    keyLength = KeyLengths.Rc2_40Bit;
                                    break;
                                case CryptRc2Version.CRYPT_RC2_56BIT_VERSION:
                                    keyLength = KeyLengths.Rc2_56Bit;
                                    break;
                                case CryptRc2Version.CRYPT_RC2_64BIT_VERSION:
                                    keyLength = KeyLengths.Rc2_64Bit;
                                    break;
                                case CryptRc2Version.CRYPT_RC2_128BIT_VERSION:
                                    keyLength = KeyLengths.Rc2_128Bit;
                                    break;
                                default:
                                    keyLength = 0;
                                    break;
                            }
                            // Retrieve IV if available.
                            if (rc2Parameters.fIV > 0)
                            {
                                rc2Parameters.ToByteArray();
                            }
                        }
                        break;
                    }

                case AlgId.CALG_RC4:
                    {
                        int saltLength = 0;
                        if (cryptAlgorithmIdentifer.Parameters.cbData != 0)
                        {
                            using (SafeHandle sh = Interop.Crypt32.CryptDecodeObjectToMemory(CryptDecodeObjectStructType.X509_OCTET_STRING, cryptAlgorithmIdentifer.Parameters.pbData, (int)cryptAlgorithmIdentifer.Parameters.cbData))
                            {
                                unsafe
                                {
                                    DATA_BLOB* pDataBlob = (DATA_BLOB*)(sh.DangerousGetHandle());
                                    saltLength = (int)(pDataBlob->cbData);
                                    parameters = (*pDataBlob).ToByteArray();
                                }
                            }
                        }

                        // For RC4, keyLength = 128 - (salt length * 8).
                        keyLength = KeyLengths.Rc4Max_128Bit - saltLength * 8;
                        break;
                    }

                case AlgId.CALG_DES:
                    // DES key length is fixed at 64 (or 56 without the parity bits).
                    keyLength = KeyLengths.Des_64Bit;

                    GetParameters(cryptAlgorithmIdentifer, ref parameters);
                    break;

                case AlgId.CALG_3DES:
                    // 3DES key length is fixed at 192 (or 168 without the parity bits).
                    keyLength = KeyLengths.TripleDes_192Bit;

                    GetParameters(cryptAlgorithmIdentifer, ref parameters);
                    break;

                default:
                    // We've exhausted all the algorithm types that the desktop used to set the KeyLength for. Key lengths are not a viable way of
                    // identifying algorithms in the long run so we will not extend this list any further.
                    keyLength = 0;
                    break;
            }

            Reset(Oid.FromOidValue(oidValue, OidGroup.All), keyLength, parameters);
        }

        public Oid Oid { get; set; }

        public int KeyLength { get; set; }

        public byte[] Parameters { get; set; } = Array.Empty<byte>();

        private void Reset(Oid oid, int keyLength, byte[] parameters)
        {
            Oid = oid;
            KeyLength = keyLength;
            Parameters = parameters;
        }

        private void GetParameters(CRYPT_ALGORITHM_IDENTIFIER cryptAlgorithmIdentifer, ref byte[] parameters)
        {
            if (cryptAlgorithmIdentifer.Parameters.cbData > 0)
            {
                CRYPT_RC2_CBC_PARAMETERS rc2Parameters;
                unsafe
                {
                    int cbSize = 24;
                    int cbSizeInit = cbSize;
                    if (!CryptDecodeObject(CryptDecodeObjectStructType.X509_OCTET_STRING, cryptAlgorithmIdentifer.Parameters.pbData, (int)(cryptAlgorithmIdentifer.Parameters.cbData), &rc2Parameters, ref cbSize))
                        throw Marshal.GetLastWin32Error().ToCryptographicException();
                }

                // Retrieve IV if available.
                if (rc2Parameters.fIV > 0)
                {
                    parameters = rc2Parameters.ToByteArray();
                }
            }
        }
    }
}

