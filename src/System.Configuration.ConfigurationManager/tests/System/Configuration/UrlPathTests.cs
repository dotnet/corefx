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
        private const string TEST_FILE_NAME = "TestFileForUrlPathTests.txt";
        private const string TEST_DIRECTORY_PATH = "C:\\Directory";
        private const string TEST_SUBDIRECTORY_PATH = TEST_DIRECTORY_PATH + "\\SubDirectory";

        public const string TEST_DIRECTORY_PATH_WITH_BACKSLASK = TEST_DIRECTORY_PATH + "\\";
        public const string TEST_SUBDIRECTORY_PATH_WITH_BACKSLASH = TEST_SUBDIRECTORY_PATH + "\\";

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
            Stream fileStream = File.Create(TEST_FILE_NAME);
            fileStream.Close();

            string test = UrlPath.GetDirectoryOrRootName(exePath + TEST_FILE_NAME);

            File.Delete(TEST_FILE_NAME);

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
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_NullSubDir()
        {
            bool test = UrlPath.IsEqualOrSubdirectory("Hello", null);
            Assert.Equal(false, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_NullDirAndSubDir()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(null, null);

            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDIrectory_EmptyDir()
        {
            bool test = UrlPath.IsEqualOrSubdirectory("", null);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_NotEmptyDir_EmptySubDir()
        {
            bool test = UrlPath.IsEqualOrSubdirectory("Hello", "");
            Assert.Equal(false, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_SubDirAndDirAreReversed_NoTrailingBackslash()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TEST_SUBDIRECTORY_PATH, TEST_DIRECTORY_PATH);
            Assert.Equal(false, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_SubDirIsASubDirOfDir_NoTrailingBackslash()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TEST_DIRECTORY_PATH, TEST_SUBDIRECTORY_PATH);
            Assert.Equal(true, test);
        }

        
    }
}