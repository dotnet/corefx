// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Xml.Tests
{
    public class SupportsTests
    {
        [Fact]
        public static void WrongFeature()
        {
            var xmlDocument = new XmlDocument();

            Assert.False(xmlDocument.Supports("XMLInternalEvents", null));
        }

        [Fact]
        public static void AllRightNames()
        {
            var xmlDocument = new XmlDocument();

            Assert.True(xmlDocument.Supports("XML", null));
            Assert.True(xmlDocument.Supports("XML", "1.0"));
            Assert.True(xmlDocument.Supports("XML", "2.0"));
        }

        [Fact]
        public static void WrongVersions()
        {
            var xmlDocument = new XmlDocument();

            Assert.False(xmlDocument.Supports("XML", "3.0"));
            Assert.False(xmlDocument.Supports("XML", "1"));
            Assert.False(xmlDocument.Supports("XML", "1.1"));
        }
    }
}
