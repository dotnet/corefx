// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EnumerableTests
{
    public class TestFileSystemEntries : IDisposable
    {
        private const string subDir_a = "lev1_a";
        private const string subDir_b = "lev1_b";
        private const string subDir_c = "lev1_c";
        private static readonly string subDir_d = Path.Combine(subDir_a, "lev2_d");
        private static readonly string subDir_e = Path.Combine(subDir_a, "lev2_e");
        private static readonly string subDir_f = Path.Combine(subDir_b, "lev2_f");

        private const string file1 = "file1";
        private static readonly string file2 = Path.Combine(subDir_b, "file2");
        private static readonly string file3 = Path.Combine(subDir_f, "file3");

        static TestFileSystemEntries()
        {
            ExpectedDirs_Deep = new HashSet<string> { subDir_a, subDir_b, subDir_c, subDir_d, subDir_e, subDir_f };

            ExpectedDirs_Shallow = new HashSet<string> { subDir_a, subDir_b, subDir_c };

            ExpectedDirs_Lev2SearchPattern = new HashSet<string> { subDir_d, subDir_e, subDir_f };

            ExpectedDirs_ExactSearchPattern = new HashSet<string> { subDir_f };

            ExpectedDirs_SubDir = new HashSet<string> { subDir_d, subDir_e };

            ExpectedFiles_Deep = new HashSet<string> { file1, file2, file3 };

            ExpectedFiles_Shallow = new HashSet<string> { file1 };

            FSIEntry entry_a = new FSIEntry("lev1_a", subDir_a, null, "lev1_a");
            FSIEntry entry_b = new FSIEntry("lev1_b", subDir_b, null, "lev1_b");
            FSIEntry entry_c = new FSIEntry("lev1_c", subDir_c, null, "lev1_c");
            FSIEntry entry_d = new FSIEntry("lev2_d", subDir_d, null, "lev2_d");
            FSIEntry entry_e = new FSIEntry("lev2_e", subDir_e, null, "lev2_e");
            FSIEntry entry_f = new FSIEntry("lev2_f", subDir_f, null, "lev2_f");

            FSIEntry entry_1 = new FSIEntry("file1", file1, "", "file1");
            FSIEntry entry_2 = new FSIEntry("file2", file2, subDir_b, "file2");
            FSIEntry entry_3 = new FSIEntry("file3", file3, subDir_f, "file3");

            ExpectedDirs_Deep_FSI = new HashSet<FSIEntry> { entry_a, entry_b, entry_c, entry_d, entry_e, entry_f };

            ExpectedDirs_Shallow_FSI = new HashSet<FSIEntry> { entry_a, entry_b, entry_c };

            ExpectedFiles_Deep_FSI = new HashSet<FSIEntry> { entry_1, entry_2, entry_3 };

            ExpectedFiles_Shallow_FSI = new HashSet<FSIEntry> { entry_1 };

            ExpectedDirs_SubDir_FSI = new HashSet<FSIEntry> { entry_d, entry_e };
        }

        public TestFileSystemEntries()
        {
            CreateTestDirs();
        }

        // test setup and tear-down
        private void CreateTestDirs()
        {
            string currentDir = Directory.GetCurrentDirectory();
            TestDirectoryName = Path.GetRandomFileName();
            TestDirectoryPath = Path.Combine(currentDir, TestDirectoryName);

            if (Directory.Exists(TestDirectoryPath))
                Directory.Delete(TestDirectoryPath, true);

            Directory.CreateDirectory(TestDirectoryPath);
            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, subDir_a));
            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, subDir_b));
            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, subDir_c));

            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, subDir_d));
            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, subDir_e));
            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, subDir_f));

            File.WriteAllText(Path.Combine(TestDirectoryPath, file1), "this is file 1" + Environment.NewLine + "Line 2 in file 1" + Environment.NewLine + "Line 3 in file 1" + Environment.NewLine + "Line 4 in file 1");
            File.WriteAllText(Path.Combine(TestDirectoryPath, file2), "this is file 2");
            File.WriteAllText(Path.Combine(TestDirectoryPath, file3), "this is file 3");
        }

        public static string GetUnusedDrive()
        {
            return IOServices.GetNonExistentDrive();
        }

        private void DeleteTestDirs()
        {
            bool deleted = false; int attemptsRemaining = 5;
            while (!deleted && attemptsRemaining > 0)
            {
                try
                {
                    if (Directory.Exists(TestDirectoryPath))
                        Directory.Delete(TestDirectoryPath, true);
                    deleted = true;
                    break;
                }
                catch (IOException)
                {
                    if (--attemptsRemaining == 0)
                        throw;
                    
                    Task.Delay(200).Wait();
                }
            }
        }

        public void ChangeFSAdd()
        {
            string newDir = Path.Combine(subDir_b, "newDir");
            string newFile = Path.Combine(subDir_c, "newFile");

            Directory.CreateDirectory(Path.Combine(TestDirectoryPath, newDir));
            File.WriteAllText(Path.Combine(TestDirectoryPath, newFile), "new file");

            ExpectedDirs_Changed = new HashSet<string>
            {
                subDir_a,
                subDir_b,
                subDir_c,
                subDir_d,
                subDir_e,
                subDir_f,
                newDir
            };

            ExpectedFiles_Changed = new HashSet<string> { file1, file2, newFile, file3 };
        }

        public void ChangeFSDelete()
        {
            Directory.Delete(Path.Combine(TestDirectoryPath, subDir_a), true); // also deletes d and e
            File.Delete(Path.Combine(TestDirectoryPath, file1));
            File.Delete(Path.Combine(TestDirectoryPath, file3));

            ExpectedDirs_Changed = new HashSet<string> { subDir_a, subDir_b, subDir_c, subDir_f };
            ExpectedFiles_Changed = new HashSet<string> { file1, file2 };
        }

        public void Dispose()
        {
            DeleteTestDirs();
        }

        public string TestDirectoryName { get; private set; }

        public string TestDirectoryPath { get; private set; }

        public string TestFilePath { get { return Path.Combine(TestDirectoryPath, file1); } }

        public static HashSet<string> ExpectedDirs_Deep { get; private set; }

        public static HashSet<string> ExpectedDirs_Shallow { get; private set; }

        public static HashSet<string> ExpectedDirs_SubDir { get; private set; }

        public static HashSet<string> ExpectedDirs_Lev2SearchPattern { get; private set; }

        public static HashSet<string> ExpectedFiles_Deep { get; private set; }

        public static HashSet<string> ExpectedFiles_Shallow { get; private set; }

        public static HashSet<string> ExpectedDirs_ExactSearchPattern { get; private set; }

        public static HashSet<string> ExpectedDirs_Changed { get; private set; }

        public static HashSet<string> ExpectedFiles_Changed { get; private set; }

        public static HashSet<FSIEntry> ExpectedDirs_Deep_FSI { get; private set; }

        public static HashSet<FSIEntry> ExpectedDirs_Shallow_FSI { get; private set; }

        public static HashSet<FSIEntry> ExpectedDirs_SubDir_FSI { get; private set; }

        public static HashSet<FSIEntry> ExpectedFiles_Deep_FSI { get; private set; }

        public static HashSet<FSIEntry> ExpectedFiles_Shallow_FSI { get; private set; }
    }

    public sealed class FSIEntry : IEquatable<FSIEntry>
    {
        public FSIEntry(string name, string fullName, string directoryName, string expectedToString)
        {
            Name = name;
            FullName = fullName;
            DirectoryName = directoryName;
            ExpectedToString = expectedToString;
        }

        public string Name { get; private set; }

        public string FullName { get; private set; }

        public string DirectoryName { get; private set; }

        public string ExpectedToString { get; private set; }

        public bool Equals(FSIEntry other)
        {
            if (other == null)
                return false;
            return Name == other.Name &&
                   FullName == other.FullName &&
                   DirectoryName == other.DirectoryName &&
                   ExpectedToString == other.ExpectedToString;
        }

        public override bool Equals(Object o)
        {
            return Equals(o as FSIEntry);
        }

        public override int GetHashCode()
        {
            int hc = Name.GetHashCode() + FullName.GetHashCode() + ExpectedToString.GetHashCode();
            if (DirectoryName != null)
            {
                hc += DirectoryName.GetHashCode();
            }
            return hc;
        }
    }
}
