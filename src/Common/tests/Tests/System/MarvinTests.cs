// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Test.Cryptography;

using Xunit;

namespace Tests.System
{
    public class MarvinTests
    {
        private const ulong Seed1 = 0x4FB61A001BDBCCLU;
        private const ulong Seed2 = 0x804FB61A001BDBCCLU;
        private const ulong Seed3 = 0x804FB61A801BDBCCLU;

        private const string TestDataString0Byte = "";
        private const string TestDataString1Byte = "af";
        private const string TestDataString2Byte = "e70f";
        private const string TestDataString3Byte = "37f495";
        private const string TestDataString4Byte = "8642dc59";
        private const string TestDataString5Byte = "153fb79826";
        private const string TestDataString6Byte = "0932e6246c47";
        private const string TestDataString7Byte = "ab427ea8d10fc7";

        [Theory]
        [MemberData(nameof(TestDataAndExpectedHashes))]
        public void ComputeHash_Success(ulong seed, string testDataString, uint expectedHash)
        {
            var testDataSpan = new Span<byte>(testDataString.HexToByteArray());
            int hash = Marvin.ComputeHash32(testDataSpan, seed);
            Assert.Equal((int)expectedHash, hash);
        }

        public static object[][] TestDataAndExpectedHashes =
        {
            new object[] { Seed1, TestDataString0Byte, 0x302009BC },
            new object[] { Seed1, TestDataString1Byte, 0x3592E206 },
            new object[] { Seed1, TestDataString2Byte, 0xFDAB5E04 },
            new object[] { Seed1, TestDataString3Byte, 0x6B3C8B90 },
            new object[] { Seed1, TestDataString4Byte, 0xE9407BE },
            new object[] { Seed1, TestDataString5Byte, 0x446F25FA },
            new object[] { Seed1, TestDataString6Byte, 0x31A6FF7A },
            new object[] { Seed1, TestDataString7Byte, 0x117FCBA5 },

            new object[] { Seed2, TestDataString0Byte, 0x89C6038E },
            new object[] { Seed2, TestDataString1Byte, 0xFE2EA000 },
            new object[] { Seed2, TestDataString2Byte, 0x7BDF321B },
            new object[] { Seed2, TestDataString3Byte, 0x4E1CBAE },
            new object[] { Seed2, TestDataString4Byte, 0x62D3D460 },
            new object[] { Seed2, TestDataString5Byte, 0x19405CF6 },
            new object[] { Seed2, TestDataString6Byte, 0x7BC69CAD },
            new object[] { Seed2, TestDataString7Byte, 0x5826B7CE },

            new object[] { Seed3, "00", 0x8DB550B6 },
            new object[] { Seed3, "FF", 0x8ED41324 },
            new object[] { Seed3, "00FF", 0xA63C6FD7 },
            new object[] { Seed3, "FF00", 0xD48848F3 },
            new object[] { Seed3, "FF00FF", 0x6388477F },
            new object[] { Seed3, "00FF00", 0x5A962949 },
            new object[] { Seed3, "00FF00FF", 0xFD4F8BB3 },
            new object[] { Seed3, "FF00FF00", 0xB8BBBE31 },
            new object[] { Seed3, "FF00FF00FF", 0x583569C9 },
            new object[] { Seed3, "00FF00FF00", 0xE08E1E22 },
            new object[] { Seed3, "00FF00FF00FF", 0xFBC906CF },
            new object[] { Seed3, "FF00FF00FF00", 0xC1D08768 },
            new object[] { Seed3, "FF00FF00FF00FF", 0xE0C1FFFF },
            new object[] { Seed3, "00FF00FF00FF00", 0x8A9C61C6 },
        };
    }
}
