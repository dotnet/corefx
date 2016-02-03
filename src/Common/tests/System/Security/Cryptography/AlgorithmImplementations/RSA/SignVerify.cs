// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Test.IO.Streams;
using Xunit;

namespace System.Security.Cryptography.Rsa.Tests
{
    public class SignVerify
    {
        [Fact]
        public static void ExpectedSignature_SHA1_2048()
        {
            byte[] expectedSignature = new byte[]
            {
                0xA1, 0xFC, 0x74, 0x67, 0x49, 0x91, 0xF4, 0x28,
                0xB0, 0xF6, 0x2B, 0xB8, 0x5E, 0x5F, 0x2E, 0x0F,
                0xD8, 0xBC, 0xB4, 0x6E, 0x0A, 0xF7, 0x11, 0xC2,
                0x65, 0x35, 0x5C, 0x1B, 0x1B, 0xC1, 0x20, 0xC0,
                0x7D, 0x5B, 0x98, 0xAF, 0xB4, 0xC1, 0x6A, 0x25,
                0x17, 0x47, 0x2C, 0x7F, 0x20, 0x2A, 0xDD, 0xF0,
                0x5F, 0xDF, 0x6F, 0x5B, 0x7D, 0xEE, 0xAA, 0x4B,
                0x9E, 0x8B, 0xA6, 0x0D, 0x81, 0x54, 0x93, 0x6E,
                0xB2, 0x86, 0xC8, 0x14, 0x4F, 0xE7, 0x4A, 0xCC,
                0xBE, 0x51, 0x2D, 0x0B, 0x9B, 0x46, 0xF1, 0x39,
                0x80, 0x1D, 0xD0, 0x07, 0xBA, 0x46, 0x48, 0xFC,
                0x7A, 0x50, 0x17, 0xC9, 0x7F, 0xEF, 0xDD, 0x42,
                0xC5, 0x8B, 0x69, 0x38, 0x67, 0xAB, 0xBD, 0x39,
                0xA6, 0xF4, 0x02, 0x34, 0x88, 0x56, 0x50, 0x05,
                0xEA, 0x95, 0x24, 0x7D, 0x34, 0xD9, 0x9F, 0xB1,
                0x05, 0x39, 0x6A, 0x42, 0x9E, 0x5E, 0xEB, 0xC9,
                0x90, 0xC1, 0x93, 0x63, 0x29, 0x0C, 0xC5, 0xBC,
                0xC8, 0x65, 0xB0, 0xFA, 0x63, 0x61, 0x77, 0xD9,
                0x16, 0x59, 0xF0, 0xAD, 0x28, 0xC7, 0x98, 0x3C,
                0x53, 0xF1, 0x6C, 0x91, 0x7E, 0x36, 0xC3, 0x3A,
                0x23, 0x87, 0xA7, 0x3A, 0x18, 0x18, 0xBF, 0xD2,
                0x3E, 0x51, 0x9E, 0xAB, 0x9E, 0x4C, 0x65, 0xBA,
                0x43, 0xC0, 0x7E, 0xA2, 0x6B, 0xCF, 0x69, 0x7C,
                0x8F, 0xAB, 0x22, 0x28, 0xD6, 0xF1, 0x65, 0x0B,
                0x4A, 0x5B, 0x9B, 0x1F, 0xD4, 0xAA, 0xEF, 0x35,
                0xA2, 0x42, 0x32, 0x00, 0x9F, 0x42, 0xBB, 0x19,
                0x99, 0x49, 0x6D, 0xB8, 0x03, 0x3D, 0x35, 0x96,
                0x0C, 0x57, 0xBB, 0x6B, 0x07, 0xA4, 0xB9, 0x7F,
                0x9B, 0xEC, 0x78, 0x90, 0xB7, 0xC8, 0x5E, 0x7F,
                0x3B, 0xAB, 0xC1, 0xB6, 0x0C, 0x84, 0x3C, 0xBC,
                0x7F, 0x04, 0x79, 0xB7, 0x9C, 0xC0, 0xFE, 0xB0,
                0xAE, 0xBD, 0xA5, 0x57, 0x2C, 0xEC, 0x3D, 0x0D,
            };

            ExpectSignature(expectedSignature, TestData.HelloBytes, "SHA1", TestData.RSA2048Params);
        }

        [Fact]
        public static void ExpectedSignature_SHA256_1024()
        {
            byte[] expectedSignature = new byte[]
            {
                0x5C, 0x2F, 0x00, 0xA9, 0xE4, 0x63, 0xD7, 0xB7,
                0x94, 0x93, 0xCE, 0xA8, 0x7E, 0x71, 0xAE, 0x97,
                0xC2, 0x6B, 0x37, 0x31, 0x5B, 0xB8, 0xE3, 0x30,
                0xDF, 0x77, 0xF8, 0xBB, 0xB5, 0xBF, 0x41, 0x9F,
                0x14, 0x6A, 0x61, 0x26, 0x2E, 0x80, 0xE5, 0xE6,
                0x8A, 0xEA, 0xC7, 0x60, 0x0B, 0xAE, 0x2B, 0xB2,
                0x18, 0xD8, 0x5D, 0xC8, 0x58, 0x86, 0x5E, 0x23,
                0x62, 0x44, 0x72, 0xEA, 0x3B, 0xF7, 0x70, 0xC6,
                0x4C, 0x2B, 0x54, 0x5B, 0xF4, 0x24, 0xA1, 0xE5,
                0x63, 0xDD, 0x50, 0x3A, 0x29, 0x26, 0x84, 0x06,
                0xEF, 0x13, 0xD0, 0xCE, 0xCC, 0xA1, 0x05, 0xB4,
                0x72, 0x81, 0x0A, 0x2E, 0x33, 0xF6, 0x2F, 0xD1,
                0xEA, 0x41, 0xB0, 0xB3, 0x93, 0x4C, 0xF3, 0x0F,
                0x6F, 0x21, 0x3E, 0xD7, 0x5F, 0x57, 0x2E, 0xC7,
                0x5F, 0xF5, 0x28, 0x89, 0xB8, 0x07, 0xDB, 0xAC,
                0x70, 0x95, 0x25, 0x49, 0x8A, 0x1A, 0xD7, 0xFC,
            };

            ExpectSignature(expectedSignature, TestData.HelloBytes, "SHA256", TestData.RSA1024Params);
        }

        [Fact]
        public static void ExpectedSignature_SHA256_2048()
        {
            byte[] expectedSignature = new byte[]
            {
                0x2C, 0x74, 0x98, 0x23, 0xF4, 0x38, 0x7F, 0x49,
                0x82, 0xB6, 0x55, 0xCF, 0xC3, 0x25, 0x4F, 0xE3,
                0x4B, 0x17, 0xE7, 0xED, 0xEA, 0x58, 0x1E, 0x63,
                0x57, 0x58, 0xCD, 0xB5, 0x06, 0xD6, 0xCA, 0x13,
                0x28, 0x81, 0xE6, 0xE0, 0x8B, 0xDC, 0xC6, 0x05,
                0x35, 0x35, 0x40, 0x73, 0x76, 0x61, 0x67, 0x42,
                0x94, 0xF7, 0x54, 0x0E, 0xB6, 0x30, 0x9A, 0x70,
                0xC3, 0x06, 0xC1, 0x59, 0xA7, 0x89, 0x66, 0x38,
                0x02, 0x5C, 0x52, 0x02, 0x17, 0x4E, 0xEC, 0x21,
                0xE9, 0x24, 0x85, 0xCB, 0x56, 0x42, 0xAB, 0x21,
                0x3A, 0x19, 0xC3, 0x95, 0x06, 0xBA, 0xDB, 0xD9,
                0x89, 0x7C, 0xB9, 0xEC, 0x1D, 0x8B, 0x5A, 0x64,
                0x87, 0xAF, 0x36, 0x71, 0xAC, 0x0A, 0x2B, 0xC7,
                0x7D, 0x2F, 0x44, 0xAA, 0xB4, 0x1C, 0xBE, 0x0B,
                0x0A, 0x4E, 0xEA, 0xF8, 0x75, 0x40, 0xD9, 0x4A,
                0x82, 0x1C, 0x82, 0x81, 0x97, 0xC2, 0xF1, 0xC8,
                0xA7, 0x4B, 0x45, 0x9A, 0x66, 0x8E, 0x35, 0x2E,
                0xE5, 0x1A, 0x2B, 0x0B, 0xF9, 0xAB, 0xC4, 0x2A,
                0xE0, 0x47, 0x72, 0x2A, 0xC2, 0xD8, 0xC6, 0xFD,
                0x91, 0x30, 0xD2, 0x45, 0xA4, 0x7F, 0x0F, 0x39,
                0x80, 0xBC, 0xA9, 0xBD, 0xEC, 0xA5, 0x03, 0x6F,
                0x01, 0xF6, 0x19, 0xD5, 0x2B, 0xD9, 0x40, 0xCD,
                0x7F, 0xEF, 0x0F, 0x9D, 0x93, 0x02, 0xCD, 0x89,
                0xB8, 0x2C, 0xC7, 0xD6, 0xFD, 0xAA, 0x12, 0x6E,
                0x4C, 0x06, 0x35, 0x08, 0x61, 0x79, 0x27, 0xE1,
                0xEA, 0x46, 0x75, 0x08, 0x5B, 0x51, 0xA1, 0x80,
                0x78, 0x02, 0xEA, 0x3E, 0xEC, 0x29, 0xD2, 0x8B,
                0xC5, 0x9E, 0x7D, 0xA4, 0x85, 0x8D, 0xAD, 0x73,
                0x39, 0x17, 0x64, 0x82, 0x46, 0x4A, 0xA4, 0x34,
                0xF0, 0xCC, 0x2F, 0x9F, 0x55, 0xA4, 0xEA, 0xEC,
                0xC9, 0xA7, 0xAB, 0xBA, 0xA8, 0x84, 0x14, 0x62,
                0x6B, 0x9B, 0x97, 0x2D, 0x8C, 0xB2, 0x1C, 0x16,
            };

            ExpectSignature(expectedSignature, TestData.HelloBytes, "SHA256", TestData.RSA2048Params);
        }

        [Fact]
        public static void ExpectSignature_SHA256_1024_Stream()
        {
            byte[] expectedSignature = new byte[]
            {
                0x78, 0x6F, 0x42, 0x00, 0xF4, 0x5A, 0xDB, 0x09,
                0x72, 0xB9, 0xCD, 0xBE, 0xB8, 0x46, 0x54, 0xE0,
                0xCF, 0x02, 0xB5, 0xA1, 0xF1, 0x7C, 0xA7, 0x5A,
                0xCF, 0x09, 0x60, 0xB6, 0xFF, 0x6B, 0x8A, 0x92,
                0x8E, 0xB4, 0xD5, 0x2C, 0x64, 0x90, 0x3E, 0x38,
                0x8B, 0x1D, 0x7D, 0x0E, 0xE8, 0x3C, 0xF0, 0xB9,
                0xBB, 0xEF, 0x90, 0x49, 0x7E, 0x6A, 0x1C, 0xEC,
                0x51, 0xB9, 0x13, 0x9B, 0x02, 0x02, 0x66, 0x59,
                0xC6, 0xB1, 0x51, 0xBD, 0x17, 0x2E, 0x03, 0xEC,
                0x93, 0x2B, 0xE9, 0x41, 0x28, 0x57, 0x8C, 0xB2,
                0x42, 0x60, 0xDE, 0xB4, 0x18, 0x85, 0x81, 0x55,
                0xAE, 0x09, 0xD9, 0xC4, 0x87, 0x57, 0xD1, 0x90,
                0xB3, 0x18, 0xD2, 0x96, 0x18, 0x91, 0x2D, 0x38,
                0x98, 0x0E, 0x68, 0x3C, 0xA6, 0x2E, 0xFE, 0x0D,
                0xD0, 0x50, 0x18, 0x55, 0x75, 0xA9, 0x85, 0x40,
                0xAB, 0x72, 0xE6, 0x7F, 0x9F, 0xDC, 0x30, 0xB9,
            };

            byte[] signature;

            using (Stream stream = new PositionValueStream(10))
            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(TestData.RSA1024Params);
                signature = rsa.SignData(stream, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }

            Assert.Equal(expectedSignature, signature);
        }

        [Fact]
        public static void VerifySignature_SHA1_2048()
        {
            byte[] signature = new byte[]
            {
                0xA1, 0xFC, 0x74, 0x67, 0x49, 0x91, 0xF4, 0x28,
                0xB0, 0xF6, 0x2B, 0xB8, 0x5E, 0x5F, 0x2E, 0x0F,
                0xD8, 0xBC, 0xB4, 0x6E, 0x0A, 0xF7, 0x11, 0xC2,
                0x65, 0x35, 0x5C, 0x1B, 0x1B, 0xC1, 0x20, 0xC0,
                0x7D, 0x5B, 0x98, 0xAF, 0xB4, 0xC1, 0x6A, 0x25,
                0x17, 0x47, 0x2C, 0x7F, 0x20, 0x2A, 0xDD, 0xF0,
                0x5F, 0xDF, 0x6F, 0x5B, 0x7D, 0xEE, 0xAA, 0x4B,
                0x9E, 0x8B, 0xA6, 0x0D, 0x81, 0x54, 0x93, 0x6E,
                0xB2, 0x86, 0xC8, 0x14, 0x4F, 0xE7, 0x4A, 0xCC,
                0xBE, 0x51, 0x2D, 0x0B, 0x9B, 0x46, 0xF1, 0x39,
                0x80, 0x1D, 0xD0, 0x07, 0xBA, 0x46, 0x48, 0xFC,
                0x7A, 0x50, 0x17, 0xC9, 0x7F, 0xEF, 0xDD, 0x42,
                0xC5, 0x8B, 0x69, 0x38, 0x67, 0xAB, 0xBD, 0x39,
                0xA6, 0xF4, 0x02, 0x34, 0x88, 0x56, 0x50, 0x05,
                0xEA, 0x95, 0x24, 0x7D, 0x34, 0xD9, 0x9F, 0xB1,
                0x05, 0x39, 0x6A, 0x42, 0x9E, 0x5E, 0xEB, 0xC9,
                0x90, 0xC1, 0x93, 0x63, 0x29, 0x0C, 0xC5, 0xBC,
                0xC8, 0x65, 0xB0, 0xFA, 0x63, 0x61, 0x77, 0xD9,
                0x16, 0x59, 0xF0, 0xAD, 0x28, 0xC7, 0x98, 0x3C,
                0x53, 0xF1, 0x6C, 0x91, 0x7E, 0x36, 0xC3, 0x3A,
                0x23, 0x87, 0xA7, 0x3A, 0x18, 0x18, 0xBF, 0xD2,
                0x3E, 0x51, 0x9E, 0xAB, 0x9E, 0x4C, 0x65, 0xBA,
                0x43, 0xC0, 0x7E, 0xA2, 0x6B, 0xCF, 0x69, 0x7C,
                0x8F, 0xAB, 0x22, 0x28, 0xD6, 0xF1, 0x65, 0x0B,
                0x4A, 0x5B, 0x9B, 0x1F, 0xD4, 0xAA, 0xEF, 0x35,
                0xA2, 0x42, 0x32, 0x00, 0x9F, 0x42, 0xBB, 0x19,
                0x99, 0x49, 0x6D, 0xB8, 0x03, 0x3D, 0x35, 0x96,
                0x0C, 0x57, 0xBB, 0x6B, 0x07, 0xA4, 0xB9, 0x7F,
                0x9B, 0xEC, 0x78, 0x90, 0xB7, 0xC8, 0x5E, 0x7F,
                0x3B, 0xAB, 0xC1, 0xB6, 0x0C, 0x84, 0x3C, 0xBC,
                0x7F, 0x04, 0x79, 0xB7, 0x9C, 0xC0, 0xFE, 0xB0,
                0xAE, 0xBD, 0xA5, 0x57, 0x2C, 0xEC, 0x3D, 0x0D,
            };

            VerifySignature(signature, TestData.HelloBytes, "SHA1", TestData.RSA2048Params);
        }

        [Fact]
        public static void VerifySignature_SHA256_1024()
        {
            byte[] signature = new byte[]
            {
                0x5C, 0x2F, 0x00, 0xA9, 0xE4, 0x63, 0xD7, 0xB7,
                0x94, 0x93, 0xCE, 0xA8, 0x7E, 0x71, 0xAE, 0x97,
                0xC2, 0x6B, 0x37, 0x31, 0x5B, 0xB8, 0xE3, 0x30,
                0xDF, 0x77, 0xF8, 0xBB, 0xB5, 0xBF, 0x41, 0x9F,
                0x14, 0x6A, 0x61, 0x26, 0x2E, 0x80, 0xE5, 0xE6,
                0x8A, 0xEA, 0xC7, 0x60, 0x0B, 0xAE, 0x2B, 0xB2,
                0x18, 0xD8, 0x5D, 0xC8, 0x58, 0x86, 0x5E, 0x23,
                0x62, 0x44, 0x72, 0xEA, 0x3B, 0xF7, 0x70, 0xC6,
                0x4C, 0x2B, 0x54, 0x5B, 0xF4, 0x24, 0xA1, 0xE5,
                0x63, 0xDD, 0x50, 0x3A, 0x29, 0x26, 0x84, 0x06,
                0xEF, 0x13, 0xD0, 0xCE, 0xCC, 0xA1, 0x05, 0xB4,
                0x72, 0x81, 0x0A, 0x2E, 0x33, 0xF6, 0x2F, 0xD1,
                0xEA, 0x41, 0xB0, 0xB3, 0x93, 0x4C, 0xF3, 0x0F,
                0x6F, 0x21, 0x3E, 0xD7, 0x5F, 0x57, 0x2E, 0xC7,
                0x5F, 0xF5, 0x28, 0x89, 0xB8, 0x07, 0xDB, 0xAC,
                0x70, 0x95, 0x25, 0x49, 0x8A, 0x1A, 0xD7, 0xFC,
            };

            VerifySignature(signature, TestData.HelloBytes, "SHA256", TestData.RSA1024Params);
        }

        [Fact]
        public static void VerifySignature_SHA256_2048()
        {
            byte[] signature = new byte[]
            {
                0x2C, 0x74, 0x98, 0x23, 0xF4, 0x38, 0x7F, 0x49,
                0x82, 0xB6, 0x55, 0xCF, 0xC3, 0x25, 0x4F, 0xE3,
                0x4B, 0x17, 0xE7, 0xED, 0xEA, 0x58, 0x1E, 0x63,
                0x57, 0x58, 0xCD, 0xB5, 0x06, 0xD6, 0xCA, 0x13,
                0x28, 0x81, 0xE6, 0xE0, 0x8B, 0xDC, 0xC6, 0x05,
                0x35, 0x35, 0x40, 0x73, 0x76, 0x61, 0x67, 0x42,
                0x94, 0xF7, 0x54, 0x0E, 0xB6, 0x30, 0x9A, 0x70,
                0xC3, 0x06, 0xC1, 0x59, 0xA7, 0x89, 0x66, 0x38,
                0x02, 0x5C, 0x52, 0x02, 0x17, 0x4E, 0xEC, 0x21,
                0xE9, 0x24, 0x85, 0xCB, 0x56, 0x42, 0xAB, 0x21,
                0x3A, 0x19, 0xC3, 0x95, 0x06, 0xBA, 0xDB, 0xD9,
                0x89, 0x7C, 0xB9, 0xEC, 0x1D, 0x8B, 0x5A, 0x64,
                0x87, 0xAF, 0x36, 0x71, 0xAC, 0x0A, 0x2B, 0xC7,
                0x7D, 0x2F, 0x44, 0xAA, 0xB4, 0x1C, 0xBE, 0x0B,
                0x0A, 0x4E, 0xEA, 0xF8, 0x75, 0x40, 0xD9, 0x4A,
                0x82, 0x1C, 0x82, 0x81, 0x97, 0xC2, 0xF1, 0xC8,
                0xA7, 0x4B, 0x45, 0x9A, 0x66, 0x8E, 0x35, 0x2E,
                0xE5, 0x1A, 0x2B, 0x0B, 0xF9, 0xAB, 0xC4, 0x2A,
                0xE0, 0x47, 0x72, 0x2A, 0xC2, 0xD8, 0xC6, 0xFD,
                0x91, 0x30, 0xD2, 0x45, 0xA4, 0x7F, 0x0F, 0x39,
                0x80, 0xBC, 0xA9, 0xBD, 0xEC, 0xA5, 0x03, 0x6F,
                0x01, 0xF6, 0x19, 0xD5, 0x2B, 0xD9, 0x40, 0xCD,
                0x7F, 0xEF, 0x0F, 0x9D, 0x93, 0x02, 0xCD, 0x89,
                0xB8, 0x2C, 0xC7, 0xD6, 0xFD, 0xAA, 0x12, 0x6E,
                0x4C, 0x06, 0x35, 0x08, 0x61, 0x79, 0x27, 0xE1,
                0xEA, 0x46, 0x75, 0x08, 0x5B, 0x51, 0xA1, 0x80,
                0x78, 0x02, 0xEA, 0x3E, 0xEC, 0x29, 0xD2, 0x8B,
                0xC5, 0x9E, 0x7D, 0xA4, 0x85, 0x8D, 0xAD, 0x73,
                0x39, 0x17, 0x64, 0x82, 0x46, 0x4A, 0xA4, 0x34,
                0xF0, 0xCC, 0x2F, 0x9F, 0x55, 0xA4, 0xEA, 0xEC,
                0xC9, 0xA7, 0xAB, 0xBA, 0xA8, 0x84, 0x14, 0x62,
                0x6B, 0x9B, 0x97, 0x2D, 0x8C, 0xB2, 0x1C, 0x16,
            };

            VerifySignature(signature, TestData.HelloBytes, "SHA256", TestData.RSA2048Params);
        }

        [Fact]
        public static void SignAndVerify_SHA1_1024()
        {
            SignAndVerify(TestData.HelloBytes, "SHA1", TestData.RSA1024Params);
        }

        [Fact]
        public static void SignAndVerify_SHA1_2048()
        {
            SignAndVerify(TestData.HelloBytes, "SHA1", TestData.RSA2048Params);
        }

        [Fact]
        public static void SignAndVerify_SHA256_1024()
        {
            SignAndVerify(TestData.HelloBytes, "SHA256", TestData.RSA1024Params);
        }

        [Fact]
        public static void NegativeVerify_WrongAlgorithm()
        {
            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(TestData.RSA2048Params);
                byte[] signature = rsa.SignData(TestData.HelloBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                bool signatureMatched = rsa.VerifyData(TestData.HelloBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                Assert.False(signatureMatched);
            }
        }

        [Fact]
        public static void NegativeVerify_WrongSignature()
        {
            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(TestData.RSA2048Params);
                byte[] signature = rsa.SignData(TestData.HelloBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

                // Invalidate the signature.
                signature[0] = (byte)~signature[0];

                bool signatureMatched = rsa.VerifyData(TestData.HelloBytes, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                Assert.False(signatureMatched);
            }
        }

        [Fact]
        public static void NegativeVerify_TamperedData()
        {
            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(TestData.RSA2048Params);
                byte[] signature = rsa.SignData(TestData.HelloBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                bool signatureMatched = rsa.VerifyData(Array.Empty<byte>(), signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                Assert.False(signatureMatched);
            }
        }

        [Fact]
        public static void NegativeVerify_BadKeysize()
        {
            byte[] signature;

            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(TestData.RSA2048Params);
                signature = rsa.SignData(TestData.HelloBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }

            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(TestData.RSA1024Params);
                bool signatureMatched = rsa.VerifyData(TestData.HelloBytes, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

                Assert.False(signatureMatched);
            }
        }

        [Fact]
        public static void ExpectedHashSignature_SHA1_2048()
        {
            byte[] expectedHashSignature = new byte[]
            {
                0xA1, 0xFC, 0x74, 0x67, 0x49, 0x91, 0xF4, 0x28,
                0xB0, 0xF6, 0x2B, 0xB8, 0x5E, 0x5F, 0x2E, 0x0F,
                0xD8, 0xBC, 0xB4, 0x6E, 0x0A, 0xF7, 0x11, 0xC2,
                0x65, 0x35, 0x5C, 0x1B, 0x1B, 0xC1, 0x20, 0xC0,
                0x7D, 0x5B, 0x98, 0xAF, 0xB4, 0xC1, 0x6A, 0x25,
                0x17, 0x47, 0x2C, 0x7F, 0x20, 0x2A, 0xDD, 0xF0,
                0x5F, 0xDF, 0x6F, 0x5B, 0x7D, 0xEE, 0xAA, 0x4B,
                0x9E, 0x8B, 0xA6, 0x0D, 0x81, 0x54, 0x93, 0x6E,
                0xB2, 0x86, 0xC8, 0x14, 0x4F, 0xE7, 0x4A, 0xCC,
                0xBE, 0x51, 0x2D, 0x0B, 0x9B, 0x46, 0xF1, 0x39,
                0x80, 0x1D, 0xD0, 0x07, 0xBA, 0x46, 0x48, 0xFC,
                0x7A, 0x50, 0x17, 0xC9, 0x7F, 0xEF, 0xDD, 0x42,
                0xC5, 0x8B, 0x69, 0x38, 0x67, 0xAB, 0xBD, 0x39,
                0xA6, 0xF4, 0x02, 0x34, 0x88, 0x56, 0x50, 0x05,
                0xEA, 0x95, 0x24, 0x7D, 0x34, 0xD9, 0x9F, 0xB1,
                0x05, 0x39, 0x6A, 0x42, 0x9E, 0x5E, 0xEB, 0xC9,
                0x90, 0xC1, 0x93, 0x63, 0x29, 0x0C, 0xC5, 0xBC,
                0xC8, 0x65, 0xB0, 0xFA, 0x63, 0x61, 0x77, 0xD9,
                0x16, 0x59, 0xF0, 0xAD, 0x28, 0xC7, 0x98, 0x3C,
                0x53, 0xF1, 0x6C, 0x91, 0x7E, 0x36, 0xC3, 0x3A,
                0x23, 0x87, 0xA7, 0x3A, 0x18, 0x18, 0xBF, 0xD2,
                0x3E, 0x51, 0x9E, 0xAB, 0x9E, 0x4C, 0x65, 0xBA,
                0x43, 0xC0, 0x7E, 0xA2, 0x6B, 0xCF, 0x69, 0x7C,
                0x8F, 0xAB, 0x22, 0x28, 0xD6, 0xF1, 0x65, 0x0B,
                0x4A, 0x5B, 0x9B, 0x1F, 0xD4, 0xAA, 0xEF, 0x35,
                0xA2, 0x42, 0x32, 0x00, 0x9F, 0x42, 0xBB, 0x19,
                0x99, 0x49, 0x6D, 0xB8, 0x03, 0x3D, 0x35, 0x96,
                0x0C, 0x57, 0xBB, 0x6B, 0x07, 0xA4, 0xB9, 0x7F,
                0x9B, 0xEC, 0x78, 0x90, 0xB7, 0xC8, 0x5E, 0x7F,
                0x3B, 0xAB, 0xC1, 0xB6, 0x0C, 0x84, 0x3C, 0xBC,
                0x7F, 0x04, 0x79, 0xB7, 0x9C, 0xC0, 0xFE, 0xB0,
                0xAE, 0xBD, 0xA5, 0x57, 0x2C, 0xEC, 0x3D, 0x0D,
            };

            byte[] dataHash;

            using (HashAlgorithm hash = SHA1.Create())
            {
                dataHash = hash.ComputeHash(TestData.HelloBytes);
            }

            ExpectHashSignature(expectedHashSignature, dataHash, "SHA1", TestData.RSA2048Params);
        }

        [Fact]
        public static void ExpectedHashSignature_SHA256_1024()
        {
            byte[] expectedHashSignature = new byte[]
            {
                0x5C, 0x2F, 0x00, 0xA9, 0xE4, 0x63, 0xD7, 0xB7,
                0x94, 0x93, 0xCE, 0xA8, 0x7E, 0x71, 0xAE, 0x97,
                0xC2, 0x6B, 0x37, 0x31, 0x5B, 0xB8, 0xE3, 0x30,
                0xDF, 0x77, 0xF8, 0xBB, 0xB5, 0xBF, 0x41, 0x9F,
                0x14, 0x6A, 0x61, 0x26, 0x2E, 0x80, 0xE5, 0xE6,
                0x8A, 0xEA, 0xC7, 0x60, 0x0B, 0xAE, 0x2B, 0xB2,
                0x18, 0xD8, 0x5D, 0xC8, 0x58, 0x86, 0x5E, 0x23,
                0x62, 0x44, 0x72, 0xEA, 0x3B, 0xF7, 0x70, 0xC6,
                0x4C, 0x2B, 0x54, 0x5B, 0xF4, 0x24, 0xA1, 0xE5,
                0x63, 0xDD, 0x50, 0x3A, 0x29, 0x26, 0x84, 0x06,
                0xEF, 0x13, 0xD0, 0xCE, 0xCC, 0xA1, 0x05, 0xB4,
                0x72, 0x81, 0x0A, 0x2E, 0x33, 0xF6, 0x2F, 0xD1,
                0xEA, 0x41, 0xB0, 0xB3, 0x93, 0x4C, 0xF3, 0x0F,
                0x6F, 0x21, 0x3E, 0xD7, 0x5F, 0x57, 0x2E, 0xC7,
                0x5F, 0xF5, 0x28, 0x89, 0xB8, 0x07, 0xDB, 0xAC,
                0x70, 0x95, 0x25, 0x49, 0x8A, 0x1A, 0xD7, 0xFC,
            };

            byte[] dataHash;

            using (HashAlgorithm hash = SHA256.Create())
            {
                dataHash = hash.ComputeHash(TestData.HelloBytes);
            }

            ExpectHashSignature(expectedHashSignature, dataHash, "SHA256", TestData.RSA1024Params);
        }

        [Fact]
        public static void ExpectedHashSignature_SHA256_2048()
        {
            byte[] expectedHashSignature = new byte[]
            {
                0x2C, 0x74, 0x98, 0x23, 0xF4, 0x38, 0x7F, 0x49,
                0x82, 0xB6, 0x55, 0xCF, 0xC3, 0x25, 0x4F, 0xE3,
                0x4B, 0x17, 0xE7, 0xED, 0xEA, 0x58, 0x1E, 0x63,
                0x57, 0x58, 0xCD, 0xB5, 0x06, 0xD6, 0xCA, 0x13,
                0x28, 0x81, 0xE6, 0xE0, 0x8B, 0xDC, 0xC6, 0x05,
                0x35, 0x35, 0x40, 0x73, 0x76, 0x61, 0x67, 0x42,
                0x94, 0xF7, 0x54, 0x0E, 0xB6, 0x30, 0x9A, 0x70,
                0xC3, 0x06, 0xC1, 0x59, 0xA7, 0x89, 0x66, 0x38,
                0x02, 0x5C, 0x52, 0x02, 0x17, 0x4E, 0xEC, 0x21,
                0xE9, 0x24, 0x85, 0xCB, 0x56, 0x42, 0xAB, 0x21,
                0x3A, 0x19, 0xC3, 0x95, 0x06, 0xBA, 0xDB, 0xD9,
                0x89, 0x7C, 0xB9, 0xEC, 0x1D, 0x8B, 0x5A, 0x64,
                0x87, 0xAF, 0x36, 0x71, 0xAC, 0x0A, 0x2B, 0xC7,
                0x7D, 0x2F, 0x44, 0xAA, 0xB4, 0x1C, 0xBE, 0x0B,
                0x0A, 0x4E, 0xEA, 0xF8, 0x75, 0x40, 0xD9, 0x4A,
                0x82, 0x1C, 0x82, 0x81, 0x97, 0xC2, 0xF1, 0xC8,
                0xA7, 0x4B, 0x45, 0x9A, 0x66, 0x8E, 0x35, 0x2E,
                0xE5, 0x1A, 0x2B, 0x0B, 0xF9, 0xAB, 0xC4, 0x2A,
                0xE0, 0x47, 0x72, 0x2A, 0xC2, 0xD8, 0xC6, 0xFD,
                0x91, 0x30, 0xD2, 0x45, 0xA4, 0x7F, 0x0F, 0x39,
                0x80, 0xBC, 0xA9, 0xBD, 0xEC, 0xA5, 0x03, 0x6F,
                0x01, 0xF6, 0x19, 0xD5, 0x2B, 0xD9, 0x40, 0xCD,
                0x7F, 0xEF, 0x0F, 0x9D, 0x93, 0x02, 0xCD, 0x89,
                0xB8, 0x2C, 0xC7, 0xD6, 0xFD, 0xAA, 0x12, 0x6E,
                0x4C, 0x06, 0x35, 0x08, 0x61, 0x79, 0x27, 0xE1,
                0xEA, 0x46, 0x75, 0x08, 0x5B, 0x51, 0xA1, 0x80,
                0x78, 0x02, 0xEA, 0x3E, 0xEC, 0x29, 0xD2, 0x8B,
                0xC5, 0x9E, 0x7D, 0xA4, 0x85, 0x8D, 0xAD, 0x73,
                0x39, 0x17, 0x64, 0x82, 0x46, 0x4A, 0xA4, 0x34,
                0xF0, 0xCC, 0x2F, 0x9F, 0x55, 0xA4, 0xEA, 0xEC,
                0xC9, 0xA7, 0xAB, 0xBA, 0xA8, 0x84, 0x14, 0x62,
                0x6B, 0x9B, 0x97, 0x2D, 0x8C, 0xB2, 0x1C, 0x16,
            };

            byte[] dataHash;

            using (HashAlgorithm hash = SHA256.Create())
            {
                dataHash = hash.ComputeHash(TestData.HelloBytes);
            }

            ExpectHashSignature(expectedHashSignature, dataHash, "SHA256", TestData.RSA2048Params);
        }

        [Fact]
        public static void VerifyHashSignature_SHA1_2048()
        {
            byte[] hashSignature = new byte[]
            {
                0xA1, 0xFC, 0x74, 0x67, 0x49, 0x91, 0xF4, 0x28,
                0xB0, 0xF6, 0x2B, 0xB8, 0x5E, 0x5F, 0x2E, 0x0F,
                0xD8, 0xBC, 0xB4, 0x6E, 0x0A, 0xF7, 0x11, 0xC2,
                0x65, 0x35, 0x5C, 0x1B, 0x1B, 0xC1, 0x20, 0xC0,
                0x7D, 0x5B, 0x98, 0xAF, 0xB4, 0xC1, 0x6A, 0x25,
                0x17, 0x47, 0x2C, 0x7F, 0x20, 0x2A, 0xDD, 0xF0,
                0x5F, 0xDF, 0x6F, 0x5B, 0x7D, 0xEE, 0xAA, 0x4B,
                0x9E, 0x8B, 0xA6, 0x0D, 0x81, 0x54, 0x93, 0x6E,
                0xB2, 0x86, 0xC8, 0x14, 0x4F, 0xE7, 0x4A, 0xCC,
                0xBE, 0x51, 0x2D, 0x0B, 0x9B, 0x46, 0xF1, 0x39,
                0x80, 0x1D, 0xD0, 0x07, 0xBA, 0x46, 0x48, 0xFC,
                0x7A, 0x50, 0x17, 0xC9, 0x7F, 0xEF, 0xDD, 0x42,
                0xC5, 0x8B, 0x69, 0x38, 0x67, 0xAB, 0xBD, 0x39,
                0xA6, 0xF4, 0x02, 0x34, 0x88, 0x56, 0x50, 0x05,
                0xEA, 0x95, 0x24, 0x7D, 0x34, 0xD9, 0x9F, 0xB1,
                0x05, 0x39, 0x6A, 0x42, 0x9E, 0x5E, 0xEB, 0xC9,
                0x90, 0xC1, 0x93, 0x63, 0x29, 0x0C, 0xC5, 0xBC,
                0xC8, 0x65, 0xB0, 0xFA, 0x63, 0x61, 0x77, 0xD9,
                0x16, 0x59, 0xF0, 0xAD, 0x28, 0xC7, 0x98, 0x3C,
                0x53, 0xF1, 0x6C, 0x91, 0x7E, 0x36, 0xC3, 0x3A,
                0x23, 0x87, 0xA7, 0x3A, 0x18, 0x18, 0xBF, 0xD2,
                0x3E, 0x51, 0x9E, 0xAB, 0x9E, 0x4C, 0x65, 0xBA,
                0x43, 0xC0, 0x7E, 0xA2, 0x6B, 0xCF, 0x69, 0x7C,
                0x8F, 0xAB, 0x22, 0x28, 0xD6, 0xF1, 0x65, 0x0B,
                0x4A, 0x5B, 0x9B, 0x1F, 0xD4, 0xAA, 0xEF, 0x35,
                0xA2, 0x42, 0x32, 0x00, 0x9F, 0x42, 0xBB, 0x19,
                0x99, 0x49, 0x6D, 0xB8, 0x03, 0x3D, 0x35, 0x96,
                0x0C, 0x57, 0xBB, 0x6B, 0x07, 0xA4, 0xB9, 0x7F,
                0x9B, 0xEC, 0x78, 0x90, 0xB7, 0xC8, 0x5E, 0x7F,
                0x3B, 0xAB, 0xC1, 0xB6, 0x0C, 0x84, 0x3C, 0xBC,
                0x7F, 0x04, 0x79, 0xB7, 0x9C, 0xC0, 0xFE, 0xB0,
                0xAE, 0xBD, 0xA5, 0x57, 0x2C, 0xEC, 0x3D, 0x0D,
            };

            byte[] dataHash;

            using (HashAlgorithm hash = SHA1.Create())
            {
                dataHash = hash.ComputeHash(TestData.HelloBytes);
            }

            VerifyHashSignature(hashSignature, dataHash, "SHA1", TestData.RSA2048Params);
        }

        [Fact]
        public static void VerifyHashSignature_SHA256_1024()
        {
            byte[] hashSignature = new byte[]
            {
                0x5C, 0x2F, 0x00, 0xA9, 0xE4, 0x63, 0xD7, 0xB7,
                0x94, 0x93, 0xCE, 0xA8, 0x7E, 0x71, 0xAE, 0x97,
                0xC2, 0x6B, 0x37, 0x31, 0x5B, 0xB8, 0xE3, 0x30,
                0xDF, 0x77, 0xF8, 0xBB, 0xB5, 0xBF, 0x41, 0x9F,
                0x14, 0x6A, 0x61, 0x26, 0x2E, 0x80, 0xE5, 0xE6,
                0x8A, 0xEA, 0xC7, 0x60, 0x0B, 0xAE, 0x2B, 0xB2,
                0x18, 0xD8, 0x5D, 0xC8, 0x58, 0x86, 0x5E, 0x23,
                0x62, 0x44, 0x72, 0xEA, 0x3B, 0xF7, 0x70, 0xC6,
                0x4C, 0x2B, 0x54, 0x5B, 0xF4, 0x24, 0xA1, 0xE5,
                0x63, 0xDD, 0x50, 0x3A, 0x29, 0x26, 0x84, 0x06,
                0xEF, 0x13, 0xD0, 0xCE, 0xCC, 0xA1, 0x05, 0xB4,
                0x72, 0x81, 0x0A, 0x2E, 0x33, 0xF6, 0x2F, 0xD1,
                0xEA, 0x41, 0xB0, 0xB3, 0x93, 0x4C, 0xF3, 0x0F,
                0x6F, 0x21, 0x3E, 0xD7, 0x5F, 0x57, 0x2E, 0xC7,
                0x5F, 0xF5, 0x28, 0x89, 0xB8, 0x07, 0xDB, 0xAC,
                0x70, 0x95, 0x25, 0x49, 0x8A, 0x1A, 0xD7, 0xFC,
            };

            byte[] dataHash;

            using (HashAlgorithm hash = SHA256.Create())
            {
                dataHash = hash.ComputeHash(TestData.HelloBytes);
            }

            VerifyHashSignature(hashSignature, dataHash, "SHA256", TestData.RSA1024Params);
        }

        [Fact]
        public static void VerifyHashSignature_SHA256_2048()
        {
            byte[] hashSignature = new byte[]
            {
                0x2C, 0x74, 0x98, 0x23, 0xF4, 0x38, 0x7F, 0x49,
                0x82, 0xB6, 0x55, 0xCF, 0xC3, 0x25, 0x4F, 0xE3,
                0x4B, 0x17, 0xE7, 0xED, 0xEA, 0x58, 0x1E, 0x63,
                0x57, 0x58, 0xCD, 0xB5, 0x06, 0xD6, 0xCA, 0x13,
                0x28, 0x81, 0xE6, 0xE0, 0x8B, 0xDC, 0xC6, 0x05,
                0x35, 0x35, 0x40, 0x73, 0x76, 0x61, 0x67, 0x42,
                0x94, 0xF7, 0x54, 0x0E, 0xB6, 0x30, 0x9A, 0x70,
                0xC3, 0x06, 0xC1, 0x59, 0xA7, 0x89, 0x66, 0x38,
                0x02, 0x5C, 0x52, 0x02, 0x17, 0x4E, 0xEC, 0x21,
                0xE9, 0x24, 0x85, 0xCB, 0x56, 0x42, 0xAB, 0x21,
                0x3A, 0x19, 0xC3, 0x95, 0x06, 0xBA, 0xDB, 0xD9,
                0x89, 0x7C, 0xB9, 0xEC, 0x1D, 0x8B, 0x5A, 0x64,
                0x87, 0xAF, 0x36, 0x71, 0xAC, 0x0A, 0x2B, 0xC7,
                0x7D, 0x2F, 0x44, 0xAA, 0xB4, 0x1C, 0xBE, 0x0B,
                0x0A, 0x4E, 0xEA, 0xF8, 0x75, 0x40, 0xD9, 0x4A,
                0x82, 0x1C, 0x82, 0x81, 0x97, 0xC2, 0xF1, 0xC8,
                0xA7, 0x4B, 0x45, 0x9A, 0x66, 0x8E, 0x35, 0x2E,
                0xE5, 0x1A, 0x2B, 0x0B, 0xF9, 0xAB, 0xC4, 0x2A,
                0xE0, 0x47, 0x72, 0x2A, 0xC2, 0xD8, 0xC6, 0xFD,
                0x91, 0x30, 0xD2, 0x45, 0xA4, 0x7F, 0x0F, 0x39,
                0x80, 0xBC, 0xA9, 0xBD, 0xEC, 0xA5, 0x03, 0x6F,
                0x01, 0xF6, 0x19, 0xD5, 0x2B, 0xD9, 0x40, 0xCD,
                0x7F, 0xEF, 0x0F, 0x9D, 0x93, 0x02, 0xCD, 0x89,
                0xB8, 0x2C, 0xC7, 0xD6, 0xFD, 0xAA, 0x12, 0x6E,
                0x4C, 0x06, 0x35, 0x08, 0x61, 0x79, 0x27, 0xE1,
                0xEA, 0x46, 0x75, 0x08, 0x5B, 0x51, 0xA1, 0x80,
                0x78, 0x02, 0xEA, 0x3E, 0xEC, 0x29, 0xD2, 0x8B,
                0xC5, 0x9E, 0x7D, 0xA4, 0x85, 0x8D, 0xAD, 0x73,
                0x39, 0x17, 0x64, 0x82, 0x46, 0x4A, 0xA4, 0x34,
                0xF0, 0xCC, 0x2F, 0x9F, 0x55, 0xA4, 0xEA, 0xEC,
                0xC9, 0xA7, 0xAB, 0xBA, 0xA8, 0x84, 0x14, 0x62,
                0x6B, 0x9B, 0x97, 0x2D, 0x8C, 0xB2, 0x1C, 0x16,
            };

            byte[] dataHash;

            using (HashAlgorithm hash = SHA256.Create())
            {
                dataHash = hash.ComputeHash(TestData.HelloBytes);
            }

            VerifyHashSignature(hashSignature, dataHash, "SHA256", TestData.RSA2048Params);
        }

        private static void ExpectSignature(
            byte[] expectedSignature,
            byte[] data,
            string hashAlgorithmName,
            RSAParameters rsaParameters)
        {
            // RSA signatures use PKCS 1.5 EMSA encoding (encoding method, signature algorithm).
            // EMSA specifies a fixed filler type of { 0x01, 0xFF, 0xFF ... 0xFF, 0x00 } whose length
            // is as long as it needs to be to match the block size.  Since the filler is deterministic,
            // the signature is deterministic, so we can safely verify it here.
            byte[] signature;

            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(rsaParameters);
                signature = rsa.SignData(data, new HashAlgorithmName(hashAlgorithmName), RSASignaturePadding.Pkcs1);
            }

            Assert.Equal(expectedSignature, signature);
        }

        private static void ExpectHashSignature(
            byte[] expectedSignature,
            byte[] dataHash,
            string hashAlgorithmName,
            RSAParameters rsaParameters)
        {
            // RSA signatures use PKCS 1.5 EMSA encoding (encoding method, signature algorithm).
            // EMSA specifies a fixed filler type of { 0x01, 0xFF, 0xFF ... 0xFF, 0x00 } whose length
            // is as long as it needs to be to match the block size.  Since the filler is deterministic,
            // the signature is deterministic, so we can safely verify it here.
            byte[] signature;

            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(rsaParameters);
                signature = rsa.SignHash(dataHash, new HashAlgorithmName(hashAlgorithmName), RSASignaturePadding.Pkcs1);
            }

            Assert.Equal(expectedSignature, signature);
        }

        private static void VerifySignature(
            byte[] signature,
            byte[] data,
            string hashAlgorithmName,
            RSAParameters rsaParameters)
        {
            RSAParameters publicOnly = new RSAParameters
            {
                Modulus = rsaParameters.Modulus,
                Exponent = rsaParameters.Exponent,
            };

            bool signatureMatched;

            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(publicOnly);
                signatureMatched = rsa.VerifyData(data, signature, new HashAlgorithmName(hashAlgorithmName), RSASignaturePadding.Pkcs1);
            }

            Assert.True(signatureMatched);
        }

        private static void VerifyHashSignature(
            byte[] signature,
            byte[] dataHash,
            string hashAlgorithmName,
            RSAParameters rsaParameters)
        {
            RSAParameters publicOnly = new RSAParameters
            {
                Modulus = rsaParameters.Modulus,
                Exponent = rsaParameters.Exponent,
            };

            bool signatureMatched;

            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(publicOnly);
                signatureMatched = rsa.VerifyHash(dataHash, signature, new HashAlgorithmName(hashAlgorithmName), RSASignaturePadding.Pkcs1);
            }

            Assert.True(signatureMatched);
        }

        private static void SignAndVerify(byte[] data, string hashAlgorithmName, RSAParameters rsaParameters)
        {
            using (RSA rsa = RSAFactory.Create())
            {
                rsa.ImportParameters(rsaParameters);
                byte[] signature = rsa.SignData(data, new HashAlgorithmName(hashAlgorithmName), RSASignaturePadding.Pkcs1);
                bool signatureMatched = rsa.VerifyData(data, signature, new HashAlgorithmName(hashAlgorithmName), RSASignaturePadding.Pkcs1);
                Assert.True(signatureMatched);
            }
        }
    }
}
