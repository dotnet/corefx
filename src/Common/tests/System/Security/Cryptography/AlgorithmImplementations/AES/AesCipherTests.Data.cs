// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Encryption.Aes.Tests
{
    public partial class AesCipherTests
    {
        private static readonly Encoding s_asciiEncoding = new ASCIIEncoding();
        private static readonly byte[] s_helloBytes = s_asciiEncoding.GetBytes("Hello");

        // This is the expected output of many decryptions. Changing this value requires re-generating test input.
        private static readonly byte[] s_multiBlockBytes =
            s_asciiEncoding.GetBytes("This is a sentence that is longer than a block, it ensures that multi-block functions work.");

        // A randomly generated 256-bit key.
        private static readonly byte[] s_aes256Key = new byte[]
        {
            0x3E, 0x8A, 0xB2, 0x5B, 0x41, 0xF2, 0x5D, 0xEF,
            0x48, 0x4E, 0x0C, 0x50, 0xBB, 0xCF, 0x89, 0xA1,
            0x1B, 0x6A, 0x26, 0x86, 0x60, 0x36, 0x7C, 0xFD,
            0x04, 0x3D, 0xE3, 0x97, 0x6D, 0xB0, 0x86, 0x60,
        };

        // A randomly generated IV, for use in the AES-256CBC tests (and other cases' negative tests)
        private static readonly byte[] s_aes256CbcIv = new byte[]
        {
            0x43, 0x20, 0xC3, 0xE1, 0xCA, 0x80, 0x0C, 0xD1,
            0xDB, 0x74, 0xF7, 0x30, 0x6D, 0xED, 0x40, 0xF7,
        };

        // A randomly generated 192-bit key.
        private static readonly byte[] s_aes192Key = new byte[]
        {
            0xA6, 0x1E, 0xC7, 0x54, 0x37, 0x4D, 0x8C, 0xA5,
            0xA4, 0xBB, 0x99, 0x50, 0x35, 0x4B, 0x30, 0x4D,
            0x6C, 0xFE, 0x3B, 0x59, 0x65, 0xCB, 0x93, 0xE3,
        };

        // A randomly generated 128-bit key.
        private static readonly byte[] s_aes128Key = new byte[]
        {
            0x8B, 0x74, 0xCF, 0x71, 0x34, 0x99, 0x97, 0x68,
            0x22, 0x86, 0xE7, 0x52, 0xED, 0xFC, 0x56, 0x7E,
        };

        // https://csrc.nist.gov/Projects/Cryptographic-Algorithm-Validation-Program/CAVP-TESTING-BLOCK-CIPHER-MODES

        // https://csrc.nist.gov/CSRC/media/Projects/Cryptographic-Algorithm-Validation-Program/documents/mac/gcmtestvectors.zip
        private const string NistGcmTestVectors = "NIST GCM Test Vectors";

        // http://web.archive.org/web/20170811123217/http://csrc.nist.gov/groups/ST/toolkit//BCM/documents/proposedmodes/gcm/gcm-revised-spec.pdf
        private const string NistGcmSpecTestCases = "NIST GCM Spec test cases";

        // https://csrc.nist.gov/CSRC/media/Projects/Cryptographic-Algorithm-Validation-Program/documents/mac/ccmtestvectors.zip
        private const string NistCcmTestVectors = "NIST CCM Test Vectors";

        public static IEnumerable<object[]> GetNistGcmTestCases()
        {
            foreach (AEADTest test in s_nistGcmSpecTestCases)
            {
                yield return new object[] { test };
            }

            foreach (AEADTest test in s_nistGcmTestVectorsSelectedCases)
            {
                yield return new object[] { test };
            }
        }

        public static IEnumerable<object[]> GetNistCcmTestCases()
        {
            foreach (AEADTest test in s_nistCcmTestVectorsSelectedCases)
            {
                yield return new object[] { test };
            }
        }

        // CaseId is unique per test case
        private static AEADTest[] s_nistGcmSpecTestCases = new AEADTest[]
        {
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 1,
                Key = "00000000000000000000000000000000".HexToByteArray(),
                Nonce = "000000000000000000000000".HexToByteArray(),
                Tag = "58e2fccefa7e3061367f1d57a4e7455a".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 2,
                Key = "00000000000000000000000000000000".HexToByteArray(),
                Nonce = "000000000000000000000000".HexToByteArray(),
                Plaintext = "00000000000000000000000000000000".HexToByteArray(),
                Ciphertext = "0388dace60b6a392f328c2b971b2fe78".HexToByteArray(),
                Tag = "ab6e47d42cec13bdf53a67b21257bddf".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 3,
                Key = "feffe9928665731c6d6a8f9467308308".HexToByteArray(),
                Nonce = "cafebabefacedbaddecaf888".HexToByteArray(),
                Plaintext = (
                    "d9313225f88406e5a55909c5aff5269a" +
                    "86a7a9531534f7da2e4c303d8a318a72" +
                    "1c3c0c95956809532fcf0e2449a6b525" +
                    "b16aedf5aa0de657ba637b391aafd255").HexToByteArray(),
                Ciphertext = (
                    "42831ec2217774244b7221b784d0d49c" +
                    "e3aa212f2c02a4e035c17e2329aca12e" +
                    "21d514b25466931c7d8f6a5aac84aa05" +
                    "1ba30b396a0aac973d58e091473f5985").HexToByteArray(),
                Tag = "4d5c2af327cd64a62cf35abd2ba6fab4".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 4,
                Key = "feffe9928665731c6d6a8f9467308308".HexToByteArray(),
                Nonce = "cafebabefacedbaddecaf888".HexToByteArray(),
                Plaintext = (
                    "d9313225f88406e5a55909c5aff5269a" +
                    "86a7a9531534f7da2e4c303d8a318a72" +
                    "1c3c0c95956809532fcf0e2449a6b525" +
                    "b16aedf5aa0de657ba637b39").HexToByteArray(),
                AssociatedData = (
                    "feedfacedeadbeeffeedfacedeadbeef" +
                    "abaddad2").HexToByteArray(),
                Ciphertext = (
                    "42831ec2217774244b7221b784d0d49c" +
                    "e3aa212f2c02a4e035c17e2329aca12e" +
                    "21d514b25466931c7d8f6a5aac84aa05" +
                    "1ba30b396a0aac973d58e091").HexToByteArray(),
                Tag = "5bc94fbc3221a5db94fae95ae7121a47".HexToByteArray(),
            },
            // cases 5, 6 have not supported nonce size
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 7,
                Key = (
                    "00000000000000000000000000000000" +
                    "0000000000000000").HexToByteArray(),
                Nonce = "000000000000000000000000".HexToByteArray(),
                Tag = "cd33b28ac773f74ba00ed1f312572435".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 8,
                Key = (
                    "00000000000000000000000000000000" +
                    "0000000000000000").HexToByteArray(),
                Nonce = "000000000000000000000000".HexToByteArray(),
                Plaintext = "00000000000000000000000000000000".HexToByteArray(),
                Ciphertext = "98e7247c07f0fe411c267e4384b0f600".HexToByteArray(),
                Tag = "2ff58d80033927ab8ef4d4587514f0fb".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 9,
                Key = (
                    "feffe9928665731c6d6a8f9467308308" +
                    "feffe9928665731c").HexToByteArray(),
                Nonce = "cafebabefacedbaddecaf888".HexToByteArray(),
                Plaintext = (
                    "d9313225f88406e5a55909c5aff5269a" +
                    "86a7a9531534f7da2e4c303d8a318a72" +
                    "1c3c0c95956809532fcf0e2449a6b525" +
                    "b16aedf5aa0de657ba637b391aafd255").HexToByteArray(),
                Ciphertext = (
                    "3980ca0b3c00e841eb06fac4872a2757" +
                    "859e1ceaa6efd984628593b40ca1e19c" +
                    "7d773d00c144c525ac619d18c84a3f47" +
                    "18e2448b2fe324d9ccda2710acade256").HexToByteArray(),
                Tag = "9924a7c8587336bfb118024db8674a14".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 10,
                Key = (
                    "feffe9928665731c6d6a8f9467308308" +
                    "feffe9928665731c").HexToByteArray(),
                Nonce = "cafebabefacedbaddecaf888".HexToByteArray(),
                Plaintext = (
                    "d9313225f88406e5a55909c5aff5269a" +
                    "86a7a9531534f7da2e4c303d8a318a72" +
                    "1c3c0c95956809532fcf0e2449a6b525" +
                    "b16aedf5aa0de657ba637b39").HexToByteArray(),
                AssociatedData = (
                    "feedfacedeadbeeffeedfacedeadbeef" +
                    "abaddad2").HexToByteArray(),
                Ciphertext = (
                    "3980ca0b3c00e841eb06fac4872a2757" +
                    "859e1ceaa6efd984628593b40ca1e19c" +
                    "7d773d00c144c525ac619d18c84a3f47" +
                    "18e2448b2fe324d9ccda2710").HexToByteArray(),
                Tag = "2519498e80f1478f37ba55bd6d27618c".HexToByteArray(),
            },
            // cases 11, 12 have not supported nonce size
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 13,
                Key = (
                    "00000000000000000000000000000000" +
                    "00000000000000000000000000000000").HexToByteArray(),
                Nonce = "000000000000000000000000".HexToByteArray(),
                Tag = "530f8afbc74536b9a963b4f1c4cb738b".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 14,
                Key = (
                    "00000000000000000000000000000000" +
                    "00000000000000000000000000000000").HexToByteArray(),
                Nonce = "000000000000000000000000".HexToByteArray(),
                Plaintext = "00000000000000000000000000000000".HexToByteArray(),
                Ciphertext = "cea7403d4d606b6e074ec5d3baf39d18".HexToByteArray(),
                Tag = "d0d1c8a799996bf0265b98b5d48ab919".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 15,
                Key = (
                    "feffe9928665731c6d6a8f9467308308" +
                    "feffe9928665731c6d6a8f9467308308").HexToByteArray(),
                Nonce = "cafebabefacedbaddecaf888".HexToByteArray(),
                Plaintext = (
                    "d9313225f88406e5a55909c5aff5269a" +
                    "86a7a9531534f7da2e4c303d8a318a72" +
                    "1c3c0c95956809532fcf0e2449a6b525" +
                    "b16aedf5aa0de657ba637b391aafd255").HexToByteArray(),
                Ciphertext = (
                    "522dc1f099567d07f47f37a32a84427d" +
                    "643a8cdcbfe5c0c97598a2bd2555d1aa" +
                    "8cb08e48590dbb3da7b08b1056828838" +
                    "c5f61e6393ba7a0abcc9f662898015ad").HexToByteArray(),
                Tag = "b094dac5d93471bdec1a502270e3cc6c".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmSpecTestCases,
                CaseId = 16,
                Key = (
                    "feffe9928665731c6d6a8f9467308308" +
                    "feffe9928665731c6d6a8f9467308308").HexToByteArray(),
                Nonce = "cafebabefacedbaddecaf888".HexToByteArray(),
                Plaintext = (
                    "d9313225f88406e5a55909c5aff5269a" +
                    "86a7a9531534f7da2e4c303d8a318a72" +
                    "1c3c0c95956809532fcf0e2449a6b525" +
                    "b16aedf5aa0de657ba637b39").HexToByteArray(),
                AssociatedData = (
                    "feedfacedeadbeeffeedfacedeadbeef" +
                    "abaddad2").HexToByteArray(),
                Ciphertext = (
                    "522dc1f099567d07f47f37a32a84427d" +
                    "643a8cdcbfe5c0c97598a2bd2555d1aa" +
                    "8cb08e48590dbb3da7b08b1056828838" +
                    "c5f61e6393ba7a0abcc9f662").HexToByteArray(),
                Tag = "76fc6ece0f4e1768cddf8853bb2d551b".HexToByteArray(),
            },
            // cases 17, 18 have not supported nonce size
        };

        // [ CaseId; Key.Length; Nonce.Length; Plaintext.Length; AssociatedData.Length; Tag.Length ] is unique
        private static AEADTest[] s_nistGcmTestVectorsSelectedCases = new AEADTest[]
        {
            // key length = 128
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "11754cd72aec309bf52f7687212e8957".HexToByteArray(),
                Nonce = "3c819d9a9bed087615030b65".HexToByteArray(),
                Tag = "250327c674aaf477aef2675748cf6971".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "4df7a13e43c3d7b66b1a72fac5ba398e".HexToByteArray(),
                Nonce = "97179a3a2d417908dcf0fb28".HexToByteArray(),
                AssociatedData = "cbb7fc0010c255661e23b07dbd804b1e06ae70ac".HexToByteArray(),
                Tag = "37791edae6c137ea946cfb40".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "0ef21489e942ae5240e41749346a86a2".HexToByteArray(),
                Nonce = "431ae3f1a702cc34b55b90bf".HexToByteArray(),
                Plaintext = "882deb960fd0f8c98c707ade59".HexToByteArray(),
                AssociatedData = (
                    "d6d20f982bdad4b70213bbc5f3921f068e7784c30070f" +
                    "fe5c06f0daa8019b6ed95b95ba294630c21008d749eb7" +
                    "1e83e847fb6ca797aaa3035e714cdb13a867ad90b2eba" +
                    "a652d50a5b6adc84e34afc1985449f45eed08cac3cb34").HexToByteArray(),
                Ciphertext = "ec8fdf5f4afb96ebe0e845dc3b".HexToByteArray(),
                Tag = "45d4b03158be4e07953767ee".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "594157ec4693202b030f33798b07176d".HexToByteArray(),
                Nonce = "49b12054082660803a1df3df".HexToByteArray(),
                Plaintext = (
                    "3feef98a976a1bd634f364ac428bb59cd51fb159ec178994691" +
                    "8dbd50ea6c9d594a3a31a5269b0da6936c29d063a5fa2cc8a1c").HexToByteArray(),
                Ciphertext = (
                    "c1b7a46a335f23d65b8db4008a49796906e225474f4fe7d39e5" +
                    "5bf2efd97fd82d4167de082ae30fa01e465a601235d8d68bc69").HexToByteArray(),
                Tag = "ba92d3661ce8b04687e8788d55417dc2".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "0493024bab2833edef571ce7224750ab".HexToByteArray(),
                Nonce = "ab8dedbcdc57f283493fe7b3".HexToByteArray(),
                Plaintext = (
                    "5f6691c5813169d128e7af7678281085af09fb1ddacfc89e1a1" +
                    "4cf14372d74eda6298a0772a594eb5a80a4c56b65744c2347d2").HexToByteArray(),
                AssociatedData = "8aca2645dd27195855b62f7d39ace11e".HexToByteArray(),
                Ciphertext = (
                    "b5d0733ade2203f5095bff60c9f5abef7770e38a56a9699e960" +
                    "8a69969141a912a0b186f7cabe2dc187cb77331c625832510e2").HexToByteArray(),
                Tag = "d34a843edbf8234abffeb7de".HexToByteArray(),
            },
            // key length = 192
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "aa740abfadcda779220d3b406c5d7ec09a77fe9d94104539".HexToByteArray(),
                Nonce = "ab2265b4c168955561f04315".HexToByteArray(),
                Tag = "f149e2b5f0adaa9842ca5f45b768a8fc".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "c8ceaa125d2fb67e6d06e4c892d3ddf87081ef9be42fd9cb".HexToByteArray(),
                Nonce = "38b081bda77b18484252c200".HexToByteArray(),
                AssociatedData = "f284d23f6dde9a417046426f5a014b37".HexToByteArray(),
                Tag = "a768033d680198aabb37cf09".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "c43e3bee7c89809f3f16f534a34a5526e2a1db16211dce7a".HexToByteArray(),
                Nonce = "62ec4fc5576ae52b5efcc715".HexToByteArray(),
                Plaintext = "bd4364e215aa459433f08e2fcc9184d9".HexToByteArray(),
                Ciphertext = "8683a2e7241113c0c5a991d19c13d306".HexToByteArray(),
                Tag = "d874766acb70effd5890955f3c".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "f6fde0b9fbd2ac2105db66c94425c8db359987d9b706badf".HexToByteArray(),
                Nonce = "9c0df1c51d2bcbb4a3c9e759".HexToByteArray(),
                Plaintext = "58d0eb269f92491374de8675ce9fafa7".HexToByteArray(),
                AssociatedData = "a5c8d41e16165a4df9a6d59ab3556440".HexToByteArray(),
                Ciphertext = "bd0d9036e9f2a6ee82992936b2b3767b".HexToByteArray(),
                Tag = "589498ce7f55a48ecb87721308ee02".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "cf1d9456f6bd4b2fc95e40197f6950843bb2ed771f5f88dc".HexToByteArray(),
                Nonce = "0a34907cd0ec7e9b92258e14".HexToByteArray(),
                Plaintext = (
                    "cd67721f6e756727a0075b4e805d13f6702f14e572fe1cd7cd5" +
                    "5bca281d6e02176c6288703d121ea73bc923d4aae919cab5878").HexToByteArray(),
                Ciphertext = (
                    "41fb3e8030d693bbbeabfeb7346ad2b4d7518594c9ef7e2f9b0" +
                    "3177ba2f2d9d10ae1dce68d370a79886dea990f472f2ab46e8b").HexToByteArray(),
                Tag = "3e169ae8466b010f51d3d88fda92".HexToByteArray(),
            },
            // key length = 256
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "b52c505a37d78eda5dd34f20c22540ea1b58963cf8e5bf8ffa85f9f2492505b4".HexToByteArray(),
                Nonce = "516c33929df5a3284ff463d7".HexToByteArray(),
                Tag = "bdc1ac884d332457a1d2664f168c76f0".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "7cb746fbd70e929a8efa65d16b1aa8a37f5b4478edc686b3a9d31631d5bf114b".HexToByteArray(),
                Nonce = "2f007847f97273c353af2b18".HexToByteArray(),
                AssociatedData = (
                    "17e84902ef33808d450f6d19b19fb3f863ca6c5476fa4" +
                    "4105ab09a34ad530b9e606ebd606529b6d088a513fdf8" +
                    "948ae78f44aff67b6f2429effc126d3c5de8cc2ca8b9b" +
                    "f7a5b4417c0a8a4f90742637d73acfbb615cde7352463").HexToByteArray(),
                Tag = "44ecc2383ae85a8cbad1f1b0".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "810bf78086dc8f630134934f9d978e0f308858e20b21dd4d319f0e6c811d6cec".HexToByteArray(),
                Nonce = "afc220a95ad53a376dadba12".HexToByteArray(),
                Plaintext = "edd60681c4919db5e32b6e44e1".HexToByteArray(),
                Ciphertext = "74e5334c28504d10116371d4c9".HexToByteArray(),
                Tag = "e6737691a08f9a08e901b3902977".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "e29e006956c7532d40bd56df5f565d57ee1ea49037404cca7b6ea9dc9e36ab0f".HexToByteArray(),
                Nonce = "ed2caad30eb367d2d89a5ffb".HexToByteArray(),
                Plaintext =
                    "b982ea6ff68af4c5202d71466f9f9f63614ad5378859a62d7a38ee32aa370bd9".HexToByteArray(),
                AssociatedData = (
                    "416a7b1db963ed683fd91bc2c5e9df3998944c3d0cbea2d2" +
                    "302c8a67249973525d0dbe8d13f806174dd983ab18854ae6").HexToByteArray(),
                Ciphertext = "656539e12450db9dd88e4113f7890e80c6186768e6c8b1fc869c42dfad7b58bf".HexToByteArray(),
                Tag = "4366e2ce0396f0410ebcb893".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistGcmTestVectors,
                CaseId = 0,
                Key = "6db07c6e834108aa97f4fb9b59378b75b6d58002f0063d8ec48af5adca3327a4".HexToByteArray(),
                Nonce = "cb2892bb9b841ff16ba0bee6".HexToByteArray(),
                Plaintext = (
                    "241f625f0560e9bf6bdb2c3734d79700d18ab0b6d0a2ae8d322" +
                    "b28195705f9db1f407b9f21372a69478b2d0b960af184c556fc").HexToByteArray(),
                AssociatedData = (
                    "e739451bbc939ae0f7b1caecf23c65112969bfbfe4b5b" +
                    "1b1c0c040cbac468e37dbef25d770f1f8b579880063c3" +
                    "37386c7033e1d0bd65924cd4ad9609c4eefc40804730a" +
                    "4474471e5a8cdda361b868074daab3e6feec3da5d5f0c").HexToByteArray(),
                Ciphertext = (
                    "19e1bf9c4b7c5f51de8a2fa0dc5d4d8cb8cbcd1c2b7df193688" +
                    "d961aa106cfd5ea9bd7c62b492df4514877b209f29e11c2efa8").HexToByteArray(),
                Tag = "4ce8aff15debc1b23c50665b9c".HexToByteArray(),
            },
        };

        private static AEADTest[] s_nistCcmTestVectorsSelectedCases = new AEADTest[]
        {
            // NIST CCM test vector define very few test cases with nonce length = 12 (10 per key size)
            // their test cases define ciphertext as a concatenation of ciphertext and tag
            // VNT128.rsp
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 50,
                Key = "005e8f4d8e0cbf4e1ceeb5d87a275848".HexToByteArray(),
                Nonce = "0ec3ac452b547b9062aac8fa".HexToByteArray(),
                Plaintext = "b6f345204526439daf84998f380dcfb4b4167c959c04ff65".HexToByteArray(),
                AssociatedData = "2f1821aa57e5278ffd33c17d46615b77363149dbc98470413f6543a6b749f2ca".HexToByteArray(),
                Ciphertext = "9575e16f35da3c88a19c26a7b762044f4d7bbbafeff05d75".HexToByteArray(),
                Tag = "4829e2a7752fa3a14890972884b511d8".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 51,
                Key = "005e8f4d8e0cbf4e1ceeb5d87a275848".HexToByteArray(),
                Nonce = "472711261a9262bef077c0b7".HexToByteArray(),
                Plaintext = "9d63df773b3799e361c5328d44bbb12f4154747ecf7cc667".HexToByteArray(),
                AssociatedData = "17c87889a2652636bcf712d111c86b9d68d64d18d531928030a5ec97c59931a4".HexToByteArray(),
                Ciphertext = "53323b82d7a754d82cebf0d4bc930ef06d11e162c5c027c4".HexToByteArray(),
                Tag = "715a641834bbb75bb6572ca5a45c3183".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 52,
                Key = "005e8f4d8e0cbf4e1ceeb5d87a275848".HexToByteArray(),
                Nonce = "6a7b80b6738ff0a23ad58fb2".HexToByteArray(),
                Plaintext = "ba1978d58492c7f827cafef87d00f1a137f3f05a2dedb14d".HexToByteArray(),
                AssociatedData = "26c12e5cdfe225a5be56d7a8aaf9fd4eb327d2f29c2ebc7396022f884f33ce54".HexToByteArray(),
                Ciphertext = "aa1d9eacabdcdd0f54681653ac44042a3dd47e338d15604e".HexToByteArray(),
                Tag = "86a0e926daf21d17b359253d0d5d5d00".HexToByteArray(),
            },
            // VNT192.rsp
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 50,
                Key = "d49b255aed8be1c02eb6d8ae2bac6dcd7901f1f61df3bbf5".HexToByteArray(),
                Nonce = "1af29e721c98e81fb6286370".HexToByteArray(),
                Plaintext = "062eafb0cd09d26e65108c0f56fcc7a305f31c34e0f3a24c".HexToByteArray(),
                AssociatedData = "64f8a0eee5487a4958a489ed35f1327e2096542c1bdb2134fb942ca91804c274".HexToByteArray(),
                Ciphertext = "721344e2fd05d2ee50713531052d75e4071103ab0436f65f".HexToByteArray(),
                Tag = "0af2a663da51bac626c9f4128ba5ec0b".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 51,
                Key = "d49b255aed8be1c02eb6d8ae2bac6dcd7901f1f61df3bbf5".HexToByteArray(),
                Nonce = "ca650ed993c4010c1b0bd1f2".HexToByteArray(),
                Plaintext = "fc375d984fa13af4a5a7516f3434365cd9473cd316e8964c".HexToByteArray(),
                AssociatedData = "4efbd225553b541c3f53cabe8a1ac03845b0e846c8616b3ea2cc7d50d344340c".HexToByteArray(),
                Ciphertext = "5b300c718d5a64f537f6cbb4d212d0f903b547ab4b21af56".HexToByteArray(),
                Tag = "ef7662525021c5777c2d74ea239a4c44".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 52,
                Key = "d49b255aed8be1c02eb6d8ae2bac6dcd7901f1f61df3bbf5".HexToByteArray(),
                Nonce = "318adeb8d8df47878ca59117".HexToByteArray(),
                Plaintext = "610a52216f47a544ec562117e0741e5f8b2e02bc9bc9122e".HexToByteArray(),
                AssociatedData = "feccf08d8c3a9be9a2c0f93f888e486b0076e2e9e2fd068c04b2db735cbeb23a".HexToByteArray(),
                Ciphertext = "83f14f6ba09a6e6b50f0d94d7d79376561f891f9a6162d0f".HexToByteArray(),
                Tag = "8925c37cc35c1c8530b0be4817814a8e".HexToByteArray(),
            },
            // VNT256.rsp
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 50,
                Key = "d6ff67379a2ead2ca87aa4f29536258f9fb9fc2e91b0ed18e7b9f5df332dd1dc".HexToByteArray(),
                Nonce = "2f1d0717a822e20c7cd28f0a".HexToByteArray(),
                Plaintext = "98626ffc6c44f13c964e7fcb7d16e988990d6d063d012d33".HexToByteArray(),
                AssociatedData = "d50741d34c8564d92f396b97be782923ff3c855ea9757bde419f632c83997630".HexToByteArray(),
                Ciphertext = "50e22db70ac2bab6d6af7059c90d00fbf0fb52eee5eb650e".HexToByteArray(),
                Tag = "08aca7dec636170f481dcb9fefb85c05".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 51,
                Key = "d6ff67379a2ead2ca87aa4f29536258f9fb9fc2e91b0ed18e7b9f5df332dd1dc".HexToByteArray(),
                Nonce = "819ecbe71f851743871163cc".HexToByteArray(),
                Plaintext = "8d164f598ea141082b1069776fccd87baf6a2563cbdbc9d1".HexToByteArray(),
                AssociatedData = "48e06c3b2940819e58eb24122a2988c997697347a6e34c21267d76049febdcf8".HexToByteArray(),
                Ciphertext = "70fd9d3c7d9e8af610edb3d329f371cf3052d820e79775a9".HexToByteArray(),
                Tag = "32d42f9954f9d35d989a09e4292949fc".HexToByteArray(),
            },
            new AEADTest
            {
                Source = NistCcmTestVectors,
                CaseId = 52,
                Key = "d6ff67379a2ead2ca87aa4f29536258f9fb9fc2e91b0ed18e7b9f5df332dd1dc".HexToByteArray(),
                Nonce = "22168c66967d545823ea0b7a".HexToByteArray(),
                Plaintext = "b28a5bc814e7f71ae94586b58281ff05a71191c92e45db74".HexToByteArray(),
                AssociatedData = "7f596bc7a815d103ed9f6dc428b60e72aeadcb9382ccde4ac9f3b61e7e8047fd".HexToByteArray(),
                Ciphertext = "30254fe7c249c0125c56c90bad3983c7f852df91fa4e828b".HexToByteArray(),
                Tag = "7522efcd96cd4de4cf41e9b67c708f9f".HexToByteArray(),
            },
        };

        public class AEADTest
        {
            public string Source { get; set; }
            public int CaseId { get; set; }
            public byte[] Key { get; set; }
            public byte[] Nonce { get; set; }
            public byte[] Plaintext { get; set; } = new byte[0];
            public byte[] Ciphertext { get; set; } = new byte[0];
            public byte[] AssociatedData { get; set; }
            public byte[] Tag { get; set; }

            private static string BitLength(byte[] data)
            {
                if (data == null)
                    return "0";
                return (data.Length * 8).ToString();
            }

            public override string ToString()
            {
                return $"{Source}; Id={CaseId}; KeyLen={BitLength(Key)}; NonceLen={BitLength(Nonce)}; PTLen={BitLength(Plaintext)}; AADLen={BitLength(AssociatedData)}; TagLen={BitLength(Tag)}";
            }
        }
    }
}
