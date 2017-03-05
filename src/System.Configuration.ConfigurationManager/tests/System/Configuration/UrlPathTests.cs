// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using System.IO;
using Xunit;

namespace System.ConfigurationTests
{
    public class UrlPathTests
    {
        [Fact]
        public void GetDirectoOrRootName_Null()
        {
            string test = UrlPath.GetDirectoryOrRootName(null);
            Assert.Equal(null, null);
        }

        [Fact]
        public void GetDirectoryOrRootName_NotDIrectoryOrRoot()
        {
            string test = UrlPath.GetDirectoryOrRootName("Hello");
            Assert.Equal("", test);
        }

        [Fact]
        public void GetDirectoryOrRootName_FileExists()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            Stream fileStream = File.Create("TestFileForUrlPathTests.txt");
            fileStream.Close();

            string test = UrlPath.GetDirectoryOrRootName(exePath +"TestFileForUrlPathTests.txt");

            File.Delete("TestFileForUrlPathTests.txt");

            //exePath has the trailing \.  Gotta get rid of it.
            Assert.Equal(exePath.Substring(0, exePath.Length - 1), test);
        }

        [Fact]
        public void GetDirectoryOrRootName_ofC()
        {
            string test = UrlPath.GetDirectoryOrRootName("C:\\");

            Assert.Equal("C:\\", test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_NullDir()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(null, "Hello");
        }
    }
}