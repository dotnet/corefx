﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Enumeration;
using System.Linq;
using Xunit;

namespace System.IO.Tests.Enumeration
{
    public class SkipAttributeTests : FileSystemTest
    {
        protected virtual string[] GetPaths(string directory, EnumerationOptions options)
        {
            return new FileSystemEnumerable<string>(
                directory,
                (ref FileSystemEntry entry) => entry.ToFullPath(),
                options)
            {
                ShouldIncludePredicate = (ref FileSystemEntry entry) => { return !entry.IsDirectory; }
            }.ToArray();
        }

        [Fact]
        public void SkippingHiddenFiles()
        {
            DirectoryInfo testDirectory = Directory.CreateDirectory(GetTestFilePath());
            DirectoryInfo testSubdirectory = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, GetTestFileName()));
            FileInfo fileOne = new FileInfo(Path.Combine(testDirectory.FullName, GetTestFileName()));

            // Put a period in front to make it hidden on Unix
            FileInfo fileTwo = new FileInfo(Path.Combine(testDirectory.FullName, "." + GetTestFileName()));
            FileInfo fileThree = new FileInfo(Path.Combine(testSubdirectory.FullName, GetTestFileName()));
            FileInfo fileFour = new FileInfo(Path.Combine(testSubdirectory.FullName, "." + GetTestFileName()));

            fileOne.Create().Dispose();
            fileTwo.Create().Dispose();
            if (PlatformDetection.IsWindows)
                fileTwo.Attributes = fileTwo.Attributes | FileAttributes.Hidden;
            fileThree.Create().Dispose();
            fileFour.Create().Dispose();
            if (PlatformDetection.IsWindows)
                fileFour.Attributes = fileTwo.Attributes | FileAttributes.Hidden;

            // Default EnumerationOptions is to skip hidden
            string[] paths = GetPaths(testDirectory.FullName, new EnumerationOptions());
            Assert.Equal(new string[] { fileOne.FullName }, paths);

            paths = GetPaths(testDirectory.FullName, new EnumerationOptions { AttributesToSkip = 0 });
            FSAssert.EqualWhenOrdered(new string[] { fileOne.FullName, fileTwo.FullName }, paths);

            paths = GetPaths(testDirectory.FullName, new EnumerationOptions { RecurseSubdirectories = true });
            Assert.Equal(new string[] { fileOne.FullName, fileThree.FullName }, paths);

            if (PlatformDetection.IsWindows)
            {
                // Shouldn't recurse into the subdirectory now that it is hidden
                testSubdirectory.Attributes = testSubdirectory.Attributes | FileAttributes.Hidden;
            }
            else
            {
                Directory.Move(testSubdirectory.FullName, Path.Combine(testDirectory.FullName, "." + testSubdirectory.Name));
            }

            paths = GetPaths(testDirectory.FullName, new EnumerationOptions { RecurseSubdirectories = true });
            Assert.Equal(new string[] { fileOne.FullName }, paths);
        }
    }

    public class SkipAttributeTests_Directory_GetFiles : SkipAttributeTests
    {
        protected override string[] GetPaths(string directory, EnumerationOptions options)
        {
            return Directory.GetFiles(directory, "*", options);
        }
    }

    public class SkipAttributeTests_DirectoryInfo_GetFiles : SkipAttributeTests
    {
        protected override string[] GetPaths(string directory, EnumerationOptions options)
        {
            return new DirectoryInfo(directory).GetFiles("*", options).Select(i => i.FullName).ToArray();
        }
    }
}
