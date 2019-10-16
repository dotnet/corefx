// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Security.Cryptography
{
    /// <summary>
    /// RFC5869  HMAC-based Extract-and-Expand Key Derivation (HKDF)
    /// </summary>
    /// <remarks>
    /// In situations where the input key material is already a uniformly random bitstring, the HKDF standard allows the Extract
    /// phase to be skipped, and the master key to be used directly as the pseudorandom key.
    /// See <a href="https://tools.ietf.org/html/rfc5869">RFC5869</a> for more information.
    /// </remarks>
    public static class HKDF
    {
        /// <summary>
        /// Performs the HKDF-Extract function.
        /// See section 2.2 of <a href="https://tools.ietf.org/html/rfc5869#section-2.2">RFC5869</a>
        /// </summary>
        /// <param name="hashAlgorithmName">Hash algorithm used for HMAC operations</param>
        /// <param name="ikm">Input keying material</param>
        /// <param name="salt">Optional salt value (a non-secret random value). If not provided it defaults to a byte array of <see cref="HashLength"/> zeros.</param>
        /// <returns>Pseudo random key (prk)</returns>
        public static byte[] Extract(HashAlgorithmName hashAlgorithmName, byte[] ikm, byte[] salt = null)
        {
            if (ikm == null)
                throw new ArgumentNullException(nameof(ikm));

            int hashLength = HashLength(hashAlgorithmName);
            ReadOnlySpan<byte> saltSpan = salt ?? ReadOnlySpan<byte>.Empty;

            byte[] prk = new byte[hashLength];

            Extract(hashAlgorithmName, hashLength, ikm, saltSpan, prk);
            return prk;
        }

        /// <summary>
        /// Performs the HKDF-Extract function.
        /// See section 2.2 of <a href="https://tools.ietf.org/html/rfc5869#section-2.2">RFC5869</a>
        /// </summary>
        /// <param name="hashAlgorithmName">Hash algorithm used for HMAC operations</param>
        /// <param name="ikm">Input keying material</param>
        /// <param name="salt">Salt value (a non-secret random value)</param>
        /// <param name="prk">Buffer representing output pseudo-random key (prk)</param>
        /// <returns>Number of bytes written to <paramref name="prk"/> buffer.</returns>
        public static int Extract(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
        {
            int hashLength = HashLength(hashAlgorithmName);

            if (prk.Length > hashLength)
            {
                prk = prk.Slice(0, hashLength);

                // if it's too short we will throw later
            }

            Extract(hashAlgorithmName, hashLength, ikm, salt, prk);
            return hashLength;
        }

        private static void Extract(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
        {
            if (prk.Length != hashLength)
            {
                throw new ArgumentException(nameof(prk));
            }

            Debug.Assert(HashLength(hashAlgorithmName) == hashLength);

            using (IncrementalHash hmac = IncrementalHash.CreateHMAC(hashAlgorithmName, salt))
            {
                hmac.AppendData(ikm);
                GetHashAndReset(hmac, prk);
            }
        }

        /// <summary>
        /// Performs the HKDF-Expand function
        /// See section 2.3 of <a href="https://tools.ietf.org/html/rfc5869#section-2.3">RFC5869</a>
        /// </summary>
        /// <param name="hashAlgorithmName">Hash algorithm used for HMAC operations</param>
        /// <param name="prk">Pseudorandom key of at least <see cref="HashLength"/> bytes (usually the output from Expand step)</param>
        /// <param name="outputLength">Length of the output keying material</param>
        /// <param name="info">Optional context and application specific information</param>
        /// <returns>Output keying material</returns>
        public static byte[] Expand(HashAlgorithmName hashAlgorithmName, byte[] prk, int outputLength, byte[] info = null)
        {
            if (prk == null)
                throw new ArgumentNullException(nameof(prk));

            int hashLength = HashLength(hashAlgorithmName);

            if (outputLength <= 0 || outputLength > 255 * hashLength)
                throw new ArgumentOutOfRangeException(nameof(outputLength));

            info = info ?? Array.Empty<byte>();
            byte[] result = new byte[outputLength];
            Expand(hashAlgorithmName, hashLength, prk, result, info);

            return result;
        }

        /// <summary>
        /// Performs the HKDF-Expand function
        /// See section 2.3 of <a href="https://tools.ietf.org/html/rfc5869#section-2.3">RFC5869</a>
        /// </summary>
        /// <param name="hashAlgorithmName">Hash algorithm used for HMAC operations</param>
        /// <param name="prk">Pseudorandom key of at least <see cref="HashLength"/> bytes (usually the output from Expand step)</param>
        /// <param name="output">Output buffer representing output keying material</param>
        /// <param name="info">Context and application specific information (can be an empty span)</param>
        public static void Expand(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> prk, Span<byte> output, ReadOnlySpan<byte> info)
        {
            int hashLength = HashLength(hashAlgorithmName);

            // We want to throw different exception type for this overload
            if (output.Length > 255 * hashLength)
                throw new ArgumentException(nameof(output));

            Expand(hashAlgorithmName, hashLength, prk, output, info);
        }

        private static void Expand(HashAlgorithmName hashAlgorithmName, int hashLength, ReadOnlySpan<byte> prk, Span<byte> output, ReadOnlySpan<byte> info)
        {
            if (prk.Length < hashLength)
                throw new ArgumentException(nameof(prk));

            if (output.Length > 255 * hashLength)
                throw new ArgumentOutOfRangeException(nameof(output));

            Span<byte> counter = stackalloc byte[1];
            Span<byte> t = Span<byte>.Empty;
            Span<byte> remainingOutput = output;

            using (IncrementalHash hmac = IncrementalHash.CreateHMAC(hashAlgorithmName, prk))
            {
                for (int i = 1; ; i++)
                {
                    hmac.AppendData(t);
                    hmac.AppendData(info);
                    counter[0] = (byte)i;
                    hmac.AppendData(counter);

                    if (remainingOutput.Length >= hashLength)
                    {
                        t = remainingOutput.Slice(0, hashLength);
                        remainingOutput = remainingOutput.Slice(hashLength);
                        GetHashAndReset(hmac, t);
                    }
                    else
                    {
                        if (remainingOutput.Length > 0)
                        {
                            Span<byte> lastChunk = stackalloc byte[hashLength];
                            GetHashAndReset(hmac, lastChunk);
                            lastChunk.Slice(0, remainingOutput.Length).CopyTo(remainingOutput);
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Performs the key derivation HKDF Expand and Extract functions
        /// </summary>
        /// <param name="hashAlgorithmName">Hash algorithm used for HMAC operations</param>
        /// <param name="ikm">Input keying material</param>
        /// <param name="outputLength">Length of the output keying material</param>
        /// <param name="salt">Optional salt value (a non-secret random value). If not provided it defaults to a byte array of <see cref="HashLength"/> zeros.</param>
        /// <param name="info">Optional context and application specific information</param>
        /// <returns>Output keying material</returns>
        public static byte[] DeriveKey(HashAlgorithmName hashAlgorithmName, byte[] ikm, int outputLength, byte[] salt = null, byte[] info = null)
        {
            if (ikm == null)
                throw new ArgumentNullException(nameof(ikm));

            int hashLength = HashLength(hashAlgorithmName);
            Span<byte> prk = stackalloc byte[hashLength];
            ReadOnlySpan<byte> saltSpan = salt ?? ReadOnlySpan<byte>.Empty;

            Extract(hashAlgorithmName, hashLength, ikm, salt, prk);

            byte[] result = new byte[outputLength];
            Expand(hashAlgorithmName, hashLength, prk, result, info);

            return result;
        }

        /// <summary>
        /// Performs the key derivation HKDF Expand and Extract functions
        /// </summary>
        /// <param name="hashAlgorithmName">Hash algorithm used for HMAC operations</param>
        /// <param name="ikm">Input keying material</param>
        /// <param name="output">Output buffer representing output keying material</param>
        /// <param name="salt">Salt value (a non-secret random value)</param>
        /// <param name="info">Context and application specific information (can be an empty span)</param>
        public static void DeriveKey(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, Span<byte> output, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info)
        {
            int hashLength = HashLength(hashAlgorithmName);

            // We want to throw different exception type for this overload
            if (output.Length > 255 * hashLength)
                throw new ArgumentException(nameof(output));

            Span<byte> prk = stackalloc byte[hashLength];

            Extract(hashAlgorithmName, hashLength, ikm, salt, prk);
            Expand(hashAlgorithmName, hashLength, prk, output, info);
        }

        private static void GetHashAndReset(IncrementalHash hmac, Span<byte> output)
        {
            if (!hmac.TryGetHashAndReset(output, out int bytesWritten))
            {
                Debug.Assert(false, "HMAC operation failed unexpectedly");
                throw new CryptographicException(SR.Arg_CryptographyException);
            }

            Debug.Assert(bytesWritten == output.Length, $"Bytes written is {bytesWritten} bytes which does not match output length ({output.Length} bytes)");
        }

        private static int HashLength(HashAlgorithmName hashAlgorithmName)
        {
            if (hashAlgorithmName == HashAlgorithmName.SHA1)
            {
                return 160 / 8;
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA256)
            {
                return 256 / 8;
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA384)
            {
                return 384 / 8;
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA512)
            {
                return 512 / 8;
            }
            else if (hashAlgorithmName == HashAlgorithmName.MD5)
            {
                return 128 / 8;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName));
            }
        }
    }
}
