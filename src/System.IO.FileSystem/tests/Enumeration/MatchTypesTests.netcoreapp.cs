// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.IO.Tests.Enumeration
{
    public abstract class MatchTypesTests : FileSystemTest
    {
        protected abstract string[] GetPaths(string directory, string pattern, EnumerationOptions options);

        [Fact]
        public void QuestionMarkBehavior()
        {
            DirectoryInfo testDirectory = Directory.CreateDirectory(GetTestFilePath());
            FileInfo fileOne = new FileInfo(Path.Combine(testDirectory.FullName, "a.one"));
            FileInfo fileTwo = new FileInfo(Path.Combine(testDirectory.FullName, "ab.two"));
            FileInfo fileThree = new FileInfo(Path.Combine(testDirectory.FullName, "abc.three"));

            // Question marks collapse to periods in Win32
            string[] paths = GetPaths(testDirectory.FullName, "a??.*", new EnumerationOptions { MatchType = MatchType.Win32 });
            FSAssert.EqualWhenOrdered(new string[] { fileOne.FullName, fileTwo.FullName, fileThree.FullName }, paths);

            // Simple, one question mark is one character
            paths = GetPaths(testDirectory.FullName, "a??.*", new EnumerationOptions { MatchType = MatchType.Simple });
            FSAssert.EqualWhenOrdered(new string[] { fileThree.FullName }, paths);

            // Regex, previous element 0 or one time
            paths = GetPaths(testDirectory.FullName, "abc?.*", new EnumerationOptions { MatchType = MatchType.Regex });
            FSAssert.EqualWhenOrdered(new string[] { fileTwo.FullName, fileThree.FullName }, paths);
        }
    }

    public class MatchTypesTests_Directory_GetFiles : MatchCasingTests
    {
        protected override string[] GetPaths(string directory, string pattern, EnumerationOptions options)
        {
            return Directory.GetFiles(directory, pattern, options);
        }
    }

    public class MatchTypesTests_DirectoryInfo_GetFiles : MatchCasingTests
    {
        protected override string[] GetPaths(string directory, string pattern, EnumerationOptions options)
        {
            return new DirectoryInfo(directory).GetFiles(pattern, options).Select(i => i.FullName).ToArray();
        }
    }
}
