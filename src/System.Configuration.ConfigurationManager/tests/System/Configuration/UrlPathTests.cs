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
        private const string TestFileName = "TestFileForUrlPathTests.txt";
        private const string TestDirectoryPath = "C:\\Directory";
        private const string TestSubdirectoryPath = TestDirectoryPath + "\\SubDirectory";

        public const string TestDirectoryPathWithBackslash = TestDirectoryPath + "\\";
        public const string TestSubdirectoryPathWithBackslash = TestSubdirectoryPath + "\\";

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
            using (TempDirectory tempDirectory = new TempDirectory())
            using (TempFile tempFile = new TempFile(tempDirectory.Path + "\\" + TestFileName))
            {
                string test = UrlPath.GetDirectoryOrRootName(tempFile.Path);
                Assert.Equal(tempDirectory.Path, test);
            }
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
        public void IsEqualOrSubDirectory_EmptyDir()
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
            bool test = UrlPath.IsEqualOrSubdirectory(TestSubdirectoryPath, TestDirectoryPath);
            Assert.Equal(false, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_SubDirIsASubDirOfDir_NoTrailingBackslash()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPath, TestSubdirectoryPath);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_True_TrailingBackslashOnBoth()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPathWithBackslash, TestSubdirectoryPathWithBackslash);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_True_DirBacksalsh()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPathWithBackslash, TestSubdirectoryPath);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_True_SubDirBackslash()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPath, TestSubdirectoryPathWithBackslash);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_Equal_NoBackslashes()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPath, TestDirectoryPath);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_Equal_BothBackslashes()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPathWithBackslash, TestDirectoryPathWithBackslash);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_Equal_FirstHasBackslash()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPathWithBackslash, TestDirectoryPath);
            Assert.Equal(true, test);
        }

        [Fact]
        public void IsEqualOrSubDirectory_Equal_SecondHasBackslash()
        {
            bool test = UrlPath.IsEqualOrSubdirectory(TestDirectoryPath, TestDirectoryPathWithBackslash);
            Assert.Equal(true, test);
        }
        
    }
}