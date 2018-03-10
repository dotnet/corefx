// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class File_Create_str : FileSystemTest
    {
        public virtual FileStream Create(string path)
        {
            return File.Create(path);
        }

        #region UniversalTests

        [Fact]
        public void NullPath()
        {
            Assert.Throws<ArgumentNullException>(() => Create(null));
        }

        [Fact]
        public void EmptyPath()
        {
            Assert.Throws<ArgumentException>(() => Create(string.Empty));
        }

        [Fact]
        public void NonExistentPath()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Create(Path.Combine(TestDirectory, GetTestFileName(), GetTestFileName())));
        }

        [Fact]
        public void CreateCurrentDirectory()
        {
            Assert.Throws<UnauthorizedAccessException>(() => Create("."));
        }

        [Fact]
        public void ValidCreation()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            using (FileStream stream = Create(testFile))
            {
                Assert.True(File.Exists(testFile));
                Assert.Equal(0, stream.Length);
                Assert.Equal(0, stream.Position);
            }
        }

        [ConditionalFact(nameof(UsingNewNormalization))]
        [PlatformSpecific(TestPlatforms.Windows)]  // Valid Windows path extended prefix
        public void ValidCreation_ExtendedSyntax()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(IOInputs.ExtendedPrefix + GetTestFilePath());
            Assert.StartsWith(IOInputs.ExtendedPrefix, testDir.FullName);
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            using (FileStream stream = Create(testFile))
            {
                Assert.True(File.Exists(testFile));
                Assert.Equal(0, stream.Length);
                Assert.Equal(0, stream.Position);
            }
        }

        [ConditionalFact(nameof(AreAllLongPathsAvailable))]
        [PlatformSpecific(TestPlatforms.Windows)]  // Valid Windows path extended prefix, long path
        public void ValidCreation_LongPathExtendedSyntax()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(IOServices.GetPath(IOInputs.ExtendedPrefix + TestDirectory, characterCount: 500));
            Assert.StartsWith(IOInputs.ExtendedPrefix, testDir.FullName);
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            using (FileStream stream = Create(testFile))
            {
                Assert.True(File.Exists(testFile));
                Assert.Equal(0, stream.Length);
                Assert.Equal(0, stream.Position);
            }
        }

        [Fact]
        public void CreateInParentDirectory()
        {
            string testFile = GetTestFileName();
            using (FileStream stream = Create(Path.Combine(TestDirectory, "DoesntExists", "..", testFile)))
            {
                Assert.True(File.Exists(Path.Combine(TestDirectory, testFile)));
            }
        }

        [Fact]
        public void LegalSymbols()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName() + "!@#$%^&");
            using (FileStream stream = Create(testFile))
            {
                Assert.True(File.Exists(testFile));
            }
        }

        [Fact]
        public void InvalidDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName(), GetTestFileName());
            Assert.Throws<DirectoryNotFoundException>(() => Create(testFile));
        }

        [Fact]
        public void FileInUse()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            using (FileStream stream = Create(testFile))
            {
                Assert.True(File.Exists(testFile));
                Assert.Throws<IOException>(() => Create(testFile));
            }
        }

        [Fact]
        public void FileAlreadyExists()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            Create(testFile).Dispose();
            Assert.True(File.Exists(testFile));
            Create(testFile).Dispose();
            Assert.True(File.Exists(testFile));
        }

        [Fact]
        public void OverwriteReadOnly()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            Create(testFile).Dispose();
            Assert.True(File.Exists(testFile));
            Create(testFile).Dispose();
            Assert.True(File.Exists(testFile));
        }

        [Fact]
        public void LongPathSegment()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());

            // Long path should throw PathTooLongException on Desktop and IOException
            // elsewhere.
            if (PlatformDetection.IsFullFramework)
            {
                Assert.Throws<PathTooLongException>(
                    () => Create(Path.Combine(testDir.FullName, new string('a', 300))));
            }
            else
            {
                AssertExtensions.ThrowsAny<IOException, DirectoryNotFoundException, PathTooLongException>(
                    () => Create(Path.Combine(testDir.FullName, new string('a', 300))));
            }
        }

        #endregion

        #region PlatformSpecific

        [Fact]
        [PlatformSpecific(CaseSensitivePlatforms)]
        public void CaseSensitive()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            using (File.Create(testFile + "AAAA"))
            using (File.Create(testFile + "aAAa"))
            {
                Assert.False(File.Exists(testFile + "AaAa"));
                Assert.True(File.Exists(testFile + "AAAA"));
                Assert.True(File.Exists(testFile + "aAAa"));
                Assert.Equal(2, Directory.GetFiles(testDir.FullName).Length);
            }
            Assert.Throws<DirectoryNotFoundException>(() => File.Create(testFile.ToLowerInvariant()));
        }

        [Fact]
        [PlatformSpecific(CaseInsensitivePlatforms)]
        public void CaseInsensitive()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testFile = Path.Combine(testDir.FullName, GetTestFileName());
            File.Create(testFile + "AAAA").Dispose();
            File.Create(testFile.ToLowerInvariant() + "aAAa").Dispose();
            Assert.Equal(1, Directory.GetFiles(testDir.FullName).Length);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework)]
        public void WindowsWildCharacterPath_Desktop()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.Throws<ArgumentException>(() => Create(Path.Combine(testDir.FullName, "dls;d", "442349-0", "v443094(*)(+*$#$*", new string(Path.DirectorySeparatorChar, 3))));
            Assert.Throws<ArgumentException>(() => Create(Path.Combine(testDir.FullName, "*")));
            Assert.Throws<ArgumentException>(() => Create(Path.Combine(testDir.FullName, "Test*t")));
            Assert.Throws<ArgumentException>(() => Create(Path.Combine(testDir.FullName, "*Tes*t")));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void WindowsWildCharacterPath_Core()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.ThrowsAny<IOException>(() => Create(Path.Combine(testDir.FullName, "dls;d", "442349-0", "v443094(*)(+*$#$*", new string(Path.DirectorySeparatorChar, 3))));
            Assert.ThrowsAny<IOException>(() => Create(Path.Combine(testDir.FullName, "*")));
            Assert.ThrowsAny<IOException>(() => Create(Path.Combine(testDir.FullName, "Test*t")));
            Assert.ThrowsAny<IOException>(() => Create(Path.Combine(testDir.FullName, "*Tes*t")));
        }

        [Theory,
            InlineData("         "),
            InlineData(""),
            InlineData("\0"),
            InlineData(" ")]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void WindowsEmptyPath(string path)
        {
            Assert.Throws<ArgumentException>(() => Create(path));
        }

        [Theory,
            InlineData("\n"),
            InlineData(">"),
            InlineData("<"),
            InlineData("\t")]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework)]
        public void WindowsInvalidPath_Desktop(string path)
        {
            Assert.Throws<ArgumentException>(() => Create(path));
        }

        [Theory,
            InlineData("\n"),
            InlineData(">"),
            InlineData("<"),
            InlineData("\t")]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void WindowsInvalidPath_Core(string path)
        {
            Assert.ThrowsAny<IOException>(() => Create(Path.Combine(TestDirectory, path)));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void CreateNullThrows_Unix()
        {
            Assert.Throws<ArgumentException>(() => Create("\0"));
        }

        [Theory,
            InlineData("         "),
            InlineData(" "),
            InlineData("\n"),
            InlineData(">"),
            InlineData("<"),
            InlineData("\t")]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Valid file name with Whitespace on Unix
        public void UnixWhitespacePath(string path)
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            using (Create(Path.Combine(testDir.FullName, path)))
            {
                Assert.True(File.Exists(Path.Combine(testDir.FullName, path)));
            }
        }

        [Theory,
            InlineData(":bar"),
            InlineData(":bar:$DATA"),
            InlineData("::$DATA")]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void WindowsAlternateDataStream(string streamName)
        {
            DirectoryInfo testDirectory = Directory.CreateDirectory(GetTestFilePath());
            streamName = Path.Combine(testDirectory.FullName, GetTestFileName()) + streamName;
            using (Create(streamName))
            {
                Assert.True(File.Exists(streamName));
            }
        }

        [Theory,
            InlineData(":bar"),
            InlineData(":bar:$DATA")]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public void WindowsAlternateDataStream_OnExisting(string streamName)
        {
            DirectoryInfo testDirectory = Directory.CreateDirectory(GetTestFilePath());

            // On closed file
            string fileName = Path.Combine(testDirectory.FullName, GetTestFileName());
            Create(fileName).Dispose();
            streamName = fileName + streamName;
            using (Create(streamName))
            {
                Assert.True(File.Exists(streamName));
            }

            // On open file
            fileName = Path.Combine(testDirectory.FullName, GetTestFileName());
            using (Create(fileName))
            using (Create(streamName))
            {
                Assert.True(File.Exists(streamName));
            }
        }

        #endregion
    }

    public class File_Create_str_i : File_Create_str
    {
        public override FileStream Create(string path)
        {
            return File.Create(path, 4096); // Default buffer size
        }

        public virtual FileStream Create(string path, int bufferSize)
        {
            return File.Create(path, bufferSize);
        }

        [Fact]
        public void NegativeBuffer()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(GetTestFilePath(), -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(GetTestFilePath(), -100));
        }
    }

    public class File_Create_str_i_fo : File_Create_str_i
    {
        public override FileStream Create(string path)
        {
            return File.Create(path, 4096, FileOptions.Asynchronous);
        }

        public override FileStream Create(string path, int bufferSize)
        {
            return File.Create(path, bufferSize, FileOptions.Asynchronous);
        }
    }
}
