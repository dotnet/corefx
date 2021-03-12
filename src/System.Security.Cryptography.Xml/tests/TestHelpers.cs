// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Resolvers;

namespace System.Security.Cryptography.Xml.Tests
{
    internal static class TestHelpers
    {
        /// <summary>
        /// Convert a <see cref="Stream"/> to a <see cref="string"/> using the given <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> to read from. This cannot be null.
        /// </param>
        /// <param name="encoding">
        /// The <see cref="Encoding"/> to use. This cannot be null.
        /// </param>
        /// <returns>
        /// The stream as a string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// No argument can be null.
        /// </exception>
        public static string StreamToString(Stream stream, Encoding encoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            using (StreamReader streamReader = new StreamReader(stream, encoding))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Perform
        /// </summary>
        /// <param name="inputXml">
        /// The XML to transform. This cannot be null, empty or whitespace.
        /// </param>
        /// <param name="transform">
        /// The <see cref="Transform"/> to perform on 
        /// <paramref name="inputXml"/>. This cannot be null.
        /// </param>
        /// <param name="encoding">
        /// An optional <see cref="Encoding"/> to use when serializing or 
        /// deserializing <paramref name="inputXml"/>. This should match the 
        /// encoding specified in <paramref name="inputXml"/>. If omitted or 
        /// null, <see cref="UTF8Encoding"/> is used.
        /// </param>
        /// <param name="resolver">
        /// An optional <see cref="XmlResolver"/> to use. If omitted or null, 
        /// no resolver is used.
        /// </param>
        /// <returns>
        /// The transformed <paramref name="inputXml"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transform"/> cannot be null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="inputXml"/> cannot be null, empty or whitespace.
        /// </exception>
        /// <exception cref="XmlException">
        /// <paramref name="inputXml"/> is not valid XML.
        /// </exception>
        public static string ExecuteTransform(string inputXml, Transform transform, Encoding encoding = null, XmlResolver resolver = null)
        {
            if (string.IsNullOrWhiteSpace(inputXml))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(inputXml));
            }
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(Transform));
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = resolver;
            doc.PreserveWhitespace = true;
            doc.LoadXml(inputXml);

            Encoding actualEncoding = encoding ?? Encoding.UTF8;
            byte[] data = actualEncoding.GetBytes(inputXml);
            using (Stream stream = new MemoryStream(data))
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { ValidationType = ValidationType.None, DtdProcessing = DtdProcessing.Parse, XmlResolver = resolver }))
            {
                doc.Load(reader);
                transform.LoadInput(doc);
                return StreamToString((Stream)transform.GetOutput(), actualEncoding);
            }
        }

        /// <summary>
        /// Convert <paramref name="fileName"/> to a full URI for referencing 
        /// in an <see cref="XmlPreloadedResolver"/>.
        /// </summary>
        /// <param name="fileName">
        /// The file name. This cannot be null, empty or whitespace.
        /// </param>
        /// <returns>
        /// The created <see cref="Uri"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileName"/> cannot be null, empty or whitespace.
        /// </exception>
        public static Uri ToUri(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Cannot be null, empty or whitespace", nameof(fileName));
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            return new Uri("file://" + (path[0] == '/' ? path : '/' + path));
        }

        /// <summary>
        /// Get specification URL from algorithm implementation
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetEncryptionMethodName(SymmetricAlgorithm key, bool keyWrap = false)
        {
            if (key is TripleDES)
            {
                return keyWrap ? EncryptedXml.XmlEncTripleDESKeyWrapUrl : EncryptedXml.XmlEncTripleDESUrl;
            }
            else if (key is DES)
            {
                return keyWrap ? EncryptedXml.XmlEncTripleDESKeyWrapUrl : EncryptedXml.XmlEncDESUrl;
            }
            else if (key is Rijndael || key is Aes)
            {
                switch (key.KeySize)
                {
                    case 128:
                        return keyWrap ? EncryptedXml.XmlEncAES128KeyWrapUrl : EncryptedXml.XmlEncAES128Url;
                    case 192:
                        return keyWrap ? EncryptedXml.XmlEncAES192KeyWrapUrl : EncryptedXml.XmlEncAES192Url;
                    case 256:
                        return keyWrap ? EncryptedXml.XmlEncAES256KeyWrapUrl : EncryptedXml.XmlEncAES256Url;
                }
            }

            throw new ArgumentException($"The specified algorithm `{key.GetType().FullName}` is not supported for XML Encryption.");
        }

        /// <summary>
        /// Lists functions creating symmetric algorithms
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SymmetricAlgorithmFactory> GetSymmetricAlgorithms(bool skipDes = false)
        {
            if (!skipDes)
            {
                yield return new SymmetricAlgorithmFactory("DES", () => DES.Create());
            }

            yield return new SymmetricAlgorithmFactory("TripleDES", () => TripleDES.Create());

            foreach (var keySize in new[] { 128, 192, 256 })
            {
                yield return new SymmetricAlgorithmFactory($"AES{keySize}", () =>
                {
                    Aes aes = Aes.Create();
                    aes.KeySize = keySize;
                    return aes;
                });
            }
        }

        private static readonly byte[] SamplePfx = Convert.FromBase64String(
    @"MIIFuQIBAzCCBX8GCSqGSIb3DQEHAaCCBXAEggVsMIIFaDCCAm8GCSqGSIb3DQEHBqCCAmAwggJcAgEAMIICVQYJKoZIhvcNAQcBMBwGCiqGSIb3DQEMAQYwDgQIsT5d6IgBq60CAggAgIICKKbgzzQEQCYYvCxqVfuMGiJr1BEb1mvMqDTKEWi2H7/AV/ORaodTqdolJQpVLFvjJKzdO60WazEd9A3/+AkfZS1df01DWvYcA2SriPj7bF5MO6Q7PiV3VLcia+xantZ8aaHnYy9J/GrbVPMt+0IdeKmN+SOEFzD3OvRhwApF85winEM47ciCLYcnYJiP9uROx0SO4fz9B1s7ar9KH8N5wm3R29DINQICdOhZU8DPdxIALsLKxEj7g3Hh4aMkPyZU4/IQ58ItmZS4e5irlIzGYYU7tpxa06FF6m7RPns2ZBw5TpX7lMUiok7rrmUq5V84lLdyrv1E3UwVAnKEeTwIB0XnzpHE3WGjwxiV6WCiVnDu/h4vGRYS8HHKhuC+OF6SU+7P2Q+B/7F2xQFDE9uWxCtkP2fBqhq7aKtsl6zGssVldkSww4Im0htfv7r6lvqe/AwPXDDxYRfmYh93uMRPosBl55HtYkVMz0XWJBc8nc3c6uHr4Z6t6EnNAoj6mgc4k63Me48eudXkxD45YYtfp0ocv/nTL3ZNxCP72sI/04ZYwKuw2ElLhEC8oDf6TC0ftuqFR9zZNQIJ8Sbj9zMG/Fa34l3NXOCi9A9u1KeEpX25Pia82U78JbfJMq0YUfGy8Hcz0cNs4f/JfF2mESQjQqK96L4KcPdOym8KIMoCRAUnMpATJBwZsJAUnq9R0Q0Q0TwPwJ8VGwFH8eb+xxHfttiPqnCg3VVdSjCCAvEGCSqGSIb3DQEHAaCCAuIEggLeMIIC2jCCAtYGCyqGSIb3DQEMCgECoIICnjCCApowHAYKKoZIhvcNAQwBAzAOBAhsdnN4cibOjgICCAAEggJ4JnhI4MBEzkjEMtpk1/sQ6RlP4AgIu0HgbDmhheFiZ88PEtf8aQOV1e21gxwnm1jWiDchOMMi9MO03gLsnMwfL6HsLMi+IOW0m2m6e1tl/QSSzvUS6TKYdwfDvcVDbCaETZOUXP3tFInWHm8O8EtOnmVpnXxAaLdepfxKyTUOk1lkfy9iZHoAvSipupwf+M1EHNnMPUT1HIJE2h76feNc9k0bNEbOKGxrjhwQRLM1CQyZ5gIzAWxJq8ZMpt5Ngw8d2SlenEwR3pnUYNUQHkULhXzNVbfoMkpiZYBdqecxLNbrS8Uv8S/RxERx4koFIEWBI570RZP1AhaBJONtNNU/4oyRWza5fAaH1Ej6wXTGtJgNqxjJglhjmfuXwyee1e9LWbWIGm+RruUxqwkagkjYhZF9G9Hg2Bc+mCidoX78j8+GOvsnObBEdJxKO8qe4n4s/H/oTwgwcikgNTA2RX7sWvc1NUx9TvdOKG+w8YvF0+a5xVbERRQlDT87X0lSeU5LhwcikkeD4WHyU31f510stAsiwY0xMY8JlP1MDylUUR9S1cqplYwl66ECckEijcpxxlvdxouRMKQ3yMejbpEtIddiLgaV3htUqVzeDQMN2pPBYCYd9qls2GzjiuUUNyz6N1knWfwak+V6gmrD884f/DsQYojUZaev3yfW+VlhWPaRupw8u70ARx6sH+S1e010BCiidZbcgj4GopI6vn1F5LzCiWF9CgNKyJmLAFhR9mmTN213Fj902C3TCKXQirXHMqvKK/btDUQNVaqM/5wrCR5M6LeAcyn0v/O1eYiVrSMe3UiqQO7U7Q8B1P2usS7Z2nuq6/PTHIUxJTAjBgkqhkiG9w0BCRUxFgQU7pIhWsIBVCkxlqGvRxgIM0O2LcUwMTAhMAkGBSsOAwIaBQAEFD14Ryp7yXCPNFqqsG1CTrb5l12uBAhGCzGln9YWkgICCAA=");

        public static X509Certificate2 GetSampleX509Certificate()
        {
            return new X509Certificate2(SamplePfx, "PLACEHOLDER");
        }

        public static Stream LoadResourceStream(string resourceName)
        {
            return typeof(TestHelpers).Assembly.GetManifestResourceStream(resourceName);
        }

        public static byte[] LoadResource(string resourceName)
        {
            using (Stream stream = typeof(TestHelpers).Assembly.GetManifestResourceStream(resourceName))
            {
                long length = stream.Length;
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, (int)length);
                return buffer;
            }
        }
    }
}
