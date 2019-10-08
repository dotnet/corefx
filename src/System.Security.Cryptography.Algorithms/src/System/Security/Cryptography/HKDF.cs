
namespace System.Security.Cryptography
{
    /// <summary>
    /// RFC5869  HMAC-based Extract-and-Expand Key Derivation (HKDF)
    /// </summary>
    /// <remarks>
    /// In situations where the input key material is already a uniformly random bitstring, the HKDF standard allows the Extract
    /// phase to be skipped, and the master key to be used directly as the pseudorandom key.
    /// <a href="https://tools.ietf.org/html/rfc5869">RFC5869</a>
    /// </remarks>
    public class HKDF
    {
        private readonly HashAlgorithmName hashAlgorithmName;

        public int HashLength { get; }

        /// <summary>
        /// Create a new HKDF using the specified hash algorithm
        /// </summary>
        /// <param name="hashAlgorithmName">Name of the hash algorithm to use</param>
        public HKDF(HashAlgorithmName hashAlgorithmName)
        {
            this.hashAlgorithmName = hashAlgorithmName;

            switch (hashAlgorithmName.Name)
            {
                case nameof(HashAlgorithmName.SHA1):
                    this.HashLength = 160 / 8;
                    break;
                case nameof(HashAlgorithmName.SHA256):
                    this.HashLength = 256 / 8;
                    break;
                case nameof(HashAlgorithmName.SHA384):
                    this.HashLength = 384 / 8;
                    break;
                case nameof(HashAlgorithmName.SHA512):
                    this.HashLength = 512 / 8;
                    break;
                default:
                    throw new NotSupportedException($"Hash algorithm {hashAlgorithmName} is not supported");
            }
        }

        /// <summary>
        /// Performs the HKDF-Extract function
        /// See section 2.2 of <a href="https://tools.ietf.org/html/rfc5869#section-2.2">RFC5869</a>
        /// </summary>
        /// <param name="ikm">Input keying material. It need not be uniformly random</param>
        /// <param name="salt">
        /// Optional salt; a random value, ideally <see cref="HashLength"/> bytes in length. It need not be kept secret.
        /// If not provided, it defaults to a byte array of <see cref="HashLength"/> zeros.
        /// </param>
        /// <returns>Pseudorandom key of <see cref="HashLength"/> bytes</returns>
        public byte[] Extract(byte[] ikm, byte[] salt = null)
        {
            if (ikm == null)
                throw new ArgumentNullException(nameof(ikm));

            salt = salt ?? new byte[this.HashLength];

            using (var hmac = BuildHMAC(salt))
            {
                return hmac.ComputeHash(ikm);
            }
        }

        /// <summary>
        /// Performs the HKDF-Expand function
        /// See section 2.3 of <a href="https://tools.ietf.org/html/rfc5869#section-2.3">RFC5869</a>
        /// </summary>
        /// <param name="prk">Pseudorandom key of at least <see cref="HashLength"/> bytes (usually the output from <see cref="Extract(byte[], byte[])"/>)</param>
        /// <param name="outputLen">Length of output keying material in bytes (must be greater than 0 and smaller or equal to 255*<see cref="HashLength"/>)</param>
        /// <param name="info">Optional context and application specific information</param>
        /// <returns>Output keying material</returns>
        public byte[] Expand(byte[] prk, int outputLen, byte[] info = null)
        {
            if (prk == null)
                throw new ArgumentNullException(nameof(prk));

            if (prk.Length < this.HashLength)
                throw new ArgumentException($"The length of {nameof(prk)} must be >= {this.HashLength}", nameof(prk));

            if (outputLen < 0 || outputLen > 255 * this.HashLength)
                throw new ArgumentOutOfRangeException(nameof(outputLen), $"Output cannot be longer than 255*{this.HashLength}");

            if (outputLen == 0)
                throw new ArgumentOutOfRangeException(nameof(outputLen), "Output length must be > 0");

            info = info ?? new byte[0];

            var n = (int)Math.Ceiling((float)outputLen / this.HashLength);
            var output = new byte[n * this.HashLength];

            var counter = new byte[1];
            var t = new byte[0];
            using (var hmac = BuildHMAC(prk))
            {
                for (var i = 1; i <= n; i++)
                {
                    counter[0] = (byte)i;
                    var combined = BlockConcat(t, info, counter);

                    t = hmac.ComputeHash(combined);
                    Buffer.BlockCopy(t, 0, output, (i - 1) * this.HashLength, this.HashLength);
                }
            }

            var result = new byte[outputLen];
            Buffer.BlockCopy(output, 0, result, 0, result.Length);
            Array.Clear(output, 0, output.Length);

            return result;
        }

        private HMAC BuildHMAC(byte[] hmacKey)
        {
            switch (this.hashAlgorithmName.Name)
            {
                case nameof(HashAlgorithmName.SHA1):
                    return new HMACSHA1(hmacKey);
                case nameof(HashAlgorithmName.SHA256):
                    return new HMACSHA256(hmacKey);
                case nameof(HashAlgorithmName.SHA384):
                    return new HMACSHA384(hmacKey);
                case nameof(HashAlgorithmName.SHA512):
                    return new HMACSHA512(hmacKey);
                default:
                    throw new NotSupportedException($"Hash algorithm {this.hashAlgorithmName} is not supported");
            }
        }

        /// <summary>
        /// Concatenate byte arrays into a single byte array
        /// </summary>
        private static byte[] BlockConcat(params byte[][] sources)
        {
            // First determine how many bytes are to be concatenated
            var sourcesLen = 0;
            foreach (var source in sources)
            {
                sourcesLen += source.Length;
            }

            // Create a buffer big enough to hold all the concatenated data
            var buffer = new byte[sourcesLen];

            // Append each byte array to the buffer
            var idx = 0;
            foreach (var source in sources)
            {
                Buffer.BlockCopy(source, 0, buffer, idx, source.Length);
                idx += source.Length;
            }

            return buffer;
        }
    }
}
