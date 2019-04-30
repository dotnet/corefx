// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using Xunit;

namespace Tests.System.IO
{
    public class PathInternal_Windows_Tests
    {
        [Theory,
            InlineData("", "", true, 0),
            InlineData("", "", false, 0),
            InlineData("a", "", true, 0),
            InlineData("a", "", false, 0),
            InlineData("", "b", true, 0),
            InlineData("", "b", false, 0),
            InlineData("\0", "\0", true, 1),
            InlineData("\0", "\0", false, 1),
            InlineData("ABcd", "ABCD", true, 4),
            InlineData("ABCD", "ABcd", true, 4),
            InlineData("ABcd", "ABCD", false, 2),
            InlineData("ABCD", "ABcd", false, 2),
            InlineData("AB\0cd", "AB\0CD", true, 5),
            InlineData("AB\0CD", "AB\0cd", true, 5),
            InlineData("AB\0cd", "AB\0CD", false, 3),
            InlineData("AB\0CD", "AB\0cd", false, 3),
            InlineData("ABc\0", "ABC\0", true, 4),
            InlineData("ABC\0", "ABc\0", true, 4),
            InlineData("ABc\0", "ABC\0", false, 2),
            InlineData("ABC\0", "ABc\0", false, 2),
            InlineData("ABcdxyzl", "ABCDpdq", true, 4),
            InlineData("ABCDxyz", "ABcdpdql", true, 4),
            InlineData("ABcdxyz", "ABCDpdq", false, 2),
            InlineData("ABCDxyzoo", "ABcdpdq", false, 2)]
        public void EqualStartingCharacterCount(string first, string second, bool ignoreCase, int expected)
        {
            Assert.Equal(expected, PathInternal.EqualStartingCharacterCount(first, second, ignoreCase));
        }


        [Theory,
            InlineData(@"", @"", true, 0),
            InlineData(@"", @"", false, 0),
            InlineData(@"a", @"A", true, 1),
            InlineData(@"A", @"a", true, 1),
            InlineData(@"a", @"A", false, 0),
            InlineData(@"A", @"a", false, 0),
            InlineData(@"foo", @"foobar", true, 0),
            InlineData(@"foo", @"foobar", false, 0),
            InlineData(@"foo", @"foo/bar", true, 3),
            InlineData(@"foo", @"foo/bar", false, 3),
            InlineData(@"foo/", @"foo/bar", true, 4),
            InlineData(@"foo/", @"foo/bar", false, 4),
            InlineData(@"foo/bar", @"foo/bar", true, 7),
            InlineData(@"foo/bar", @"foo/bar", false, 7),
            InlineData(@"foo/bar", @"foo/BAR", true, 7),
            InlineData(@"foo/bar", @"foo/BAR", false, 4),
            InlineData(@"foo/bar", @"foo/barb", true, 4),
            InlineData(@"foo/bar", @"foo/barb", false, 4)]
        public void GetCommonPathLength(string first, string second, bool ignoreCase, int expected)
        {
            Assert.Equal(expected, PathInternal.GetCommonPathLength(first, second, ignoreCase));
        }

        public static TheoryData<string, string> RemoveRelativeSegmentNewData => new TheoryData<string, string>
        {
            { @"C:\git\corefx", @"C:\git\corefx"},
            { @"C:\\git\corefx", @"C:\git\corefx"},
            { @"C:\git\\corefx", @"C:\git\corefx"},
            { @"C:\.\.\.\.\.\git\\corefx", @"C:\git\corefx"},
            { @"C:\..\git\\corefx", @"C:\git\corefx"},
            { @"asd\asd\..\.\", @"asd\"},
            { @"asd\asd\..\..\..\.\.\", @"\"},

            { @"C:A\.", @"C:A"},
            { @"C:A\..", @"C:"},
            { @"C:A\..\..", @"C:"},
            { @"C:A\..\..\..", @"C:"},
        };


        public static TheoryData<string, string> RemoveRelativeSegmentsData => new TheoryData<string, string>
        {
            { @"C:\git\corefx", @"C:\git\corefx"},
            { @"C:\\git\corefx", @"C:\git\corefx"},
            { @"C:\git\\corefx", @"C:\git\corefx"},
            { @"C:\git\.\corefx\.\\", @"C:\git\corefx\"},
            { @"C:\git\corefx", @"C:\git\corefx"},
            { @"C:\git\..\corefx", @"C:\corefx"},
            { @"C:\git\corefx\..\", @"C:\git\"},
            { @"C:\git\corefx\..\..\..\", @"C:\"},
            { @"C:\git\corefx\..\..\.\", @"C:\"},
            { @"C:\git\..\.\corefx\temp\..", @"C:\corefx"},
            { @"C:\git\..\\\.\..\corefx", @"C:\corefx"},
            { @"C:\git\corefx\", @"C:\git\corefx\"},
            { @"C:\git\temp\..\corefx\", @"C:\git\corefx\"},

            { @"C:\.", @"C:\"},
            { @"C:\..", @"C:\"},
            { @"C:\..\..", @"C:\"},
            { @"C:\.", @"C:\"},
            { @"C:\..", @"C:\"},
            { @"C:\..\..", @"C:\"},

            { @"C:\tmp\home", @"C:\tmp\home" },
            { @"C:\tmp\..", @"C:\" },
            { @"C:\tmp\home\..\.\.\", @"C:\tmp\" },
            { @"C:\tmp\..\..\..\", @"C:\" },
            { @"C:\tmp\\home", @"C:\tmp\home" },
            { @"C:\.\tmp\\home", @"C:\tmp\home" },
            { @"C:\..\tmp\home", @"C:\tmp\home" },
            { @"C:\..\..\..\tmp\.\home", @"C:\tmp\home" },
            { @"C:\\tmp\\\home", @"C:\tmp\home" },
            { @"C:\tmp\home\git\.\..\.\git\corefx\..\", @"C:\tmp\home\git\" },
            { @"C:\.\tmp\home", @"C:\tmp\home" },

            { @"C:\tmp\home", @"C:\tmp\home" },
            { @"C:\tmp\..", @"C:\" },
            { @"C:\tmp\home\..\.\.\", @"C:\tmp\" },
            { @"C:\tmp\..\..\..\", @"C:\" },
            { @"C:\tmp\\home", @"C:\tmp\home" },
            { @"C:\.\tmp\\home", @"C:\tmp\home" },
            { @"C:\..\tmp\home", @"C:\tmp\home" },
            { @"C:\..\..\..\tmp\.\home", @"C:\tmp\home" },
            { @"C:\\tmp\\\home", @"C:\tmp\home" },
            { @"C:\tmp\home\git\.\..\.\git\corefx\..\", @"C:\tmp\home\git\" },
            { @"C:\.\tmp\home", @"C:\tmp\home" },

            { @"C:\tmp\..", @"C:\" },
            { @"C:\tmp\home\..\..\.\", @"C:\" },
            { @"C:\tmp\..\..\..\", @"C:\" },
            { @"C:\tmp\\home", @"C:\tmp\home" },
            { @"C:\.\tmp\\home", @"C:\tmp\home" },
            { @"C:\..\tmp\home", @"C:\tmp\home" },
            { @"C:\..\..\..\tmp\.\home", @"C:\tmp\home" },
            { @"C:\\tmp\\\home", @"C:\tmp\home" },
            { @"C:\tmp\home\git\.\..\.\git\corefx\..\", @"C:\tmp\home\git\" },
            { @"C:\.\tmp\home", @"C:\tmp\home" },

            { @"C:\tmp\..\..\", @"C:\" },
            { @"C:\tmp\home\..\.\.\", @"C:\tmp\" },
            { @"C:\tmp\..\..\..\", @"C:\" },
            { @"C:\tmp\\home\..\.\\", @"C:\tmp\" },
            { @"C:\.\tmp\\home\git\git", @"C:\tmp\home\git\git" },
            { @"C:\..\tmp\.\home", @"C:\tmp\home" },
            { @"C:\..\..\..\tmp\.\home", @"C:\tmp\home" },
            { @"C:\\tmp\\\home\..", @"C:\tmp" },
            { @"C:\tmp\home\git\.\..\.\git\corefx\..\", @"C:\tmp\home\git\" },
            { @"C:\.\tmp\home\.\.\", @"C:\tmp\home\" },
        };

        public static TheoryData<string, string> RemoveRelativeSegmentsFirstRelativeSegment => new TheoryData<string, string>
        {
            { @"C:\\git\corefx", @"C:\git\corefx"},
            { @"C:\.\git\corefx", @"C:\git\corefx"},
            { @"C:\\.\git\.\corefx", @"C:\git\corefx"},
            { @"C:\..\git\corefx", @"C:\git\corefx"},
            { @"C:\.\git\..\corefx", @"C:\corefx"},
            { @"C:\.\git\corefx\..\", @"C:\git\"},
            { @"C:\.\git\corefx\..\..\..\", @"C:\"},
            { @"C:\.\git\corefx\..\..\.\", @"C:\"},
            { @"C:\.\git\..\.\corefx\temp\..", @"C:\corefx"},
            { @"C:\.\git\..\\\.\..\corefx", @"C:\corefx"},
            { @"C:\.\git\corefx\", @"C:\git\corefx\"},
            { @"C:\.\git\temp\..\corefx\", @"C:\git\corefx\"},
            { @"C:\\..\..", @"C:\"}
        };

        public static TheoryData<string, string> RemoveRelativeSegmentsSkipAboveRoot => new TheoryData<string, string>
        {
            { @"C:\temp\..\" , @"C:\" },
            { @"C:\temp\..\git" , @"C:\git" },
            { @"C:\temp\..\.\" , @"C:\" },
            { @"C:\git\..\temp\..\" , @"C:\" },
            { @"C:\\\.\..\..\temp\..\" , @"C:\" },
        };

        public static TheoryData<string, string> RemoveRelativeSegmentsFirstRelativeSegmentRoot => new TheoryData<string, string>
        {
            { @"C:\\git\corefx", @"C:\git\corefx"},
            { @"C:\.\git\corefx", @"C:\git\corefx"},
            { @"C:\\.\git\.\corefx", @"C:\git\corefx"},
            { @"C:\..\git\corefx", @"C:\git\corefx"},
            { @"C:\.\git\..\corefx", @"C:\corefx"},
            { @"C:\.\git\corefx\..\", @"C:\git\"},
            { @"C:\.\git\corefx\..\..\..\", @"C:\"},
            { @"C:\.\git\corefx\..\..\.\", @"C:\"},
            { @"C:\.\git\..\.\corefx\temp\..", @"C:\corefx"},
            { @"C:\.\git\..\\\.\..\corefx", @"C:\corefx"},
            { @"C:\.\git\corefx\", @"C:\git\corefx\"},
            { @"C:\.\git\temp\..\corefx\", @"C:\git\corefx\"},
        };

        [Theory,
            MemberData(nameof(RemoveRelativeSegmentsData)),
            MemberData(nameof(RemoveRelativeSegmentsFirstRelativeSegment)),
            MemberData(nameof(RemoveRelativeSegmentsFirstRelativeSegmentRoot)),
            MemberData(nameof(RemoveRelativeSegmentsSkipAboveRoot))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void RemoveRelativeSegmentsTest(string path, string expected)
        {
            Assert.Equal(expected, PathInternal.RemoveRelativeSegments(path));
            Assert.Equal(@"\\.\" + expected, PathInternal.RemoveRelativeSegments(@"\\.\" + path));
            Assert.Equal(@"\\?\" + expected, PathInternal.RemoveRelativeSegments(@"\\?\" + path));
        }

        [Theory,
            MemberData(nameof(RemoveRelativeSegmentNewData))]
        public void RemoveRelativeSegmentsTest2(string path, string expected)
        {
            Assert.Equal(expected, PathInternal.RemoveRelativeSegments(path));
        }

        public static TheoryData<string, string> RemoveRelativeSegmentsUncData => new TheoryData<string, string>
        {
            { @"Server\Share\git\corefx", @"Server\Share\git\corefx"},
            { @"Server\Share\\git\corefx", @"Server\Share\git\corefx"},
            { @"Server\Share\git\\corefx", @"Server\Share\git\corefx"},
            { @"Server\Share\git\.\corefx\.\\", @"Server\Share\git\corefx\"},
            { @"Server\Share\git\corefx", @"Server\Share\git\corefx"},
            { @"Server\Share\git\..\corefx", @"Server\Share\corefx"},
            { @"Server\Share\git\corefx\..\", @"Server\Share\git\"},
            { @"Server\Share\git\corefx\..\..\..\", @"Server\Share\"},
            { @"Server\Share\git\corefx\..\..\.\", @"Server\Share\"},
            { @"Server\Share\git\..\.\corefx\temp\..", @"Server\Share\corefx"},
            { @"Server\Share\git\..\\\.\..\corefx", @"Server\Share\corefx"},
            { @"Server\Share\git\corefx\", @"Server\Share\git\corefx\"},
            { @"Server\Share\git\temp\..\corefx\", @"Server\Share\git\corefx\"},
        };

        [Theory,
            MemberData(nameof(RemoveRelativeSegmentsUncData))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void RemoveRelativeSegmentsUncTest(string path, string expected)
        {
            Assert.Equal(@"\\" + expected, PathInternal.RemoveRelativeSegments(@"\\" + path));
            Assert.Equal(@"\\.\UNC\" + expected, PathInternal.RemoveRelativeSegments(@"\\.\UNC\" + path));
            Assert.Equal(@"\\?\UNC\" + expected, PathInternal.RemoveRelativeSegments(@"\\?\UNC\" + path));
        }

        public static TheoryData<string, string> RemoveRelativeSegmentsDeviceData => new TheoryData<string, string>
        {
            { @"\\.\git\corefx", @"\\.\git\corefx"},
            { @"\\.\git\corefx", @"\\.\git\corefx"},
            { @"\\.\git\\corefx", @"\\.\git\corefx"},
            { @"\\.\git\.\corefx\.\\", @"\\.\git\corefx\"},
            { @"\\.\git\corefx", @"\\.\git\corefx"},
            { @"\\.\git\..\corefx", @"\\.\git\corefx"},
            { @"\\.\git\corefx\..\", @"\\.\git\"},
            { @"\\.\git\corefx\..\..\..\", @"\\.\git\"},
            { @"\\.\git\corefx\..\..\.\", @"\\.\git\"},
            { @"\\.\git\..\.\corefx\temp\..", @"\\.\git\corefx"},
            { @"\\.\git\..\\\.\..\corefx", @"\\.\git\corefx"},
            { @"\\.\git\corefx\", @"\\.\git\corefx\"},
            { @"\\.\git\temp\..\corefx\", @"\\.\git\corefx\"},

            { @"\\.\.\corefx", @"\\.\.\corefx"},
            { @"\\.\.\corefx", @"\\.\.\corefx"},
            { @"\\.\.\\corefx", @"\\.\.\corefx"},
            { @"\\.\.\.\corefx\.\\", @"\\.\.\corefx\"},
            { @"\\.\.\corefx", @"\\.\.\corefx"},
            { @"\\.\.\..\corefx", @"\\.\.\corefx"},
            { @"\\.\.\corefx\..\", @"\\.\.\"},
            { @"\\.\.\corefx\..\..\..\", @"\\.\.\"},
            { @"\\.\.\corefx\..\..\.\", @"\\.\.\"},
            { @"\\.\.\..\.\corefx\temp\..", @"\\.\.\corefx"},
            { @"\\.\.\..\\\.\..\corefx", @"\\.\.\corefx"},
            { @"\\.\.\corefx\", @"\\.\.\corefx\"},
            { @"\\.\.\temp\..\corefx\", @"\\.\.\corefx\"},

            { @"\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\\corefx", @"\\.\..\corefx"},
            { @"\\.\..\.\corefx\.\\", @"\\.\..\corefx\"},
            { @"\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\corefx\..\", @"\\.\..\"},
            { @"\\.\..\corefx\..\..\..\", @"\\.\..\"},
            { @"\\.\..\corefx\..\..\.\", @"\\.\..\"},
            { @"\\.\..\..\.\corefx\temp\..", @"\\.\..\corefx"},
            { @"\\.\..\..\\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\corefx\", @"\\.\..\corefx\"},
            { @"\\.\..\temp\..\corefx\", @"\\.\..\corefx\"},

            { @"\\.\\corefx", @"\\.\corefx"},
            { @"\\.\\corefx", @"\\.\corefx"},
            { @"\\.\\\corefx", @"\\.\corefx"},
            { @"\\.\\.\corefx\.\\", @"\\.\corefx\"},
            { @"\\.\\corefx", @"\\.\corefx"},
            { @"\\.\\..\corefx", @"\\.\corefx"},
            { @"\\.\\corefx\..\", @"\\.\"},
            { @"\\.\\corefx\..\..\..\", @"\\.\"},
            { @"\\.\\corefx\..\..\.\", @"\\.\"},
            { @"\\.\\..\.\corefx\temp\..", @"\\.\corefx"},
            { @"\\.\\..\\\.\..\corefx", @"\\.\corefx"},
            { @"\\.\\corefx\", @"\\.\corefx\"},
            { @"\\.\\temp\..\corefx\", @"\\.\corefx\"},

            { @"\\.\C:A\.", @"\\.\C:A\"},
            { @"\\.\C:A\..", @"\\.\C:A\"},
            { @"\\.\C:A\..\..", @"\\.\C:A\"},
            { @"\\.\C:A\..\..\..", @"\\.\C:A\"},
        };

        public static TheoryData<string, string> RemoveRelativeSegmentsDeviceRootData => new TheoryData<string, string>
        {
            { @"\\.\git\corefx", @"\\.\git\corefx"},
            { @"\\.\git\corefx", @"\\.\git\corefx"},
            { @"\\.\git\\corefx", @"\\.\git\corefx"},
            { @"\\.\git\.\corefx\.\\", @"\\.\git\corefx\"},
            { @"\\.\git\corefx", @"\\.\git\corefx"},
            { @"\\.\git\..\corefx", @"\\.\git\corefx"},
            { @"\\.\git\corefx\..\", @"\\.\git\"},
            { @"\\.\git\corefx\..\..\..\", @"\\.\git\"},
            { @"\\.\git\corefx\..\..\.\", @"\\.\git\"},
            { @"\\.\git\..\.\corefx\temp\..", @"\\.\git\corefx"},
            { @"\\.\git\..\\\.\..\corefx", @"\\.\git\corefx"},
            { @"\\.\git\corefx\", @"\\.\git\corefx\"},
            { @"\\.\git\temp\..\corefx\", @"\\.\git\corefx\"},

            { @"\\.\.\corefx", @"\\.\.\corefx"},
            { @"\\.\.\corefx", @"\\.\.\corefx"},
            { @"\\.\.\\corefx", @"\\.\.\corefx"},
            { @"\\.\.\.\corefx\.\\", @"\\.\.\corefx\"},
            { @"\\.\.\corefx", @"\\.\.\corefx"},
            { @"\\.\.\..\corefx", @"\\.\.\corefx"},
            { @"\\.\.\corefx\..\", @"\\.\.\"},
            { @"\\.\.\corefx\..\..\..\", @"\\.\.\"},
            { @"\\.\.\corefx\..\..\.\", @"\\.\.\"},
            { @"\\.\.\..\.\corefx\temp\..", @"\\.\.\corefx"},
            { @"\\.\.\..\\\.\..\corefx", @"\\.\.\corefx"},
            { @"\\.\.\corefx\", @"\\.\.\corefx\"},
            { @"\\.\.\temp\..\corefx\", @"\\.\.\corefx\"},

            { @"\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\\corefx", @"\\.\..\corefx"},
            { @"\\.\..\.\corefx\.\\", @"\\.\..\corefx\"},
            { @"\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\corefx\..\", @"\\.\..\"},
            { @"\\.\..\corefx\..\..\..\", @"\\.\..\"},
            { @"\\.\..\corefx\..\..\.\", @"\\.\..\"},
            { @"\\.\..\..\.\corefx\temp\..", @"\\.\..\corefx"},
            { @"\\.\..\..\\\.\..\corefx", @"\\.\..\corefx"},
            { @"\\.\..\corefx\", @"\\.\..\corefx\"},
            { @"\\.\..\temp\..\corefx\", @"\\.\..\corefx\"}
        };

        [Theory,
            MemberData(nameof(RemoveRelativeSegmentsDeviceData)),
            MemberData(nameof(RemoveRelativeSegmentsDeviceRootData))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void RemoveRelativeSegmentsDeviceTest(string path, string expected)
        {
            Assert.Equal(expected, PathInternal.RemoveRelativeSegments(path));
            StringBuilder sb = new StringBuilder(expected);
            sb.Replace('.', '?', 0, 4);
            expected = sb.ToString();

            sb = new StringBuilder(path);
            sb.Replace('.', '?', 0, 4);
            path = sb.ToString();
            Assert.Equal(expected, PathInternal.RemoveRelativeSegments(path));
        }

        public static TheoryData<string, string> RemoveRelativeSegmentUnixData => new TheoryData<string, string>
        {
            { "/tmp/home", "/tmp/home" },
            { "/tmp/..", "/" },
            { "/tmp/home/../././", "/tmp/" },
            { "/tmp/../../../", "/" },
            { "/tmp//home", "/tmp/home" },
            { "/./tmp//home", "/tmp/home" },
            { "/../tmp/home", "/tmp/home" },
            { "/../../../tmp/./home", "/tmp/home" },
            { "//tmp///home", "/tmp/home" },
            { "/tmp/home/git/./.././git/corefx/../", "/tmp/home/git/" },
            { "/./tmp/home", "/tmp/home" },

            { "/tmp/home", "/tmp/home" },
            { "/tmp/..", "/tmp" },
            { "/tmp/home/../././", "/tmp/" },
            { "/tmp/../../../", "/tmp/" },
            { "/tmp//home", "/tmp/home" },
            { "/./tmp//home", "/./tmp/home" },
            { "/../tmp/home", "/../tmp/home" },
            { "/../../../tmp/./home", "/../tmp/home" },
            { "//tmp///home", "//tmp/home" },
            { "/tmp/home/git/./.././git/corefx/../", "/tmp/home/git/" },
            { "/./tmp/home", "/./tmp/home" },

            { "/tmp/../../", "/tmp/../" },
            { "/tmp/home/../././", "/tmp/home/" },
            { "/tmp/../../../", "/tmp/../" },
            { "/tmp//home/.././/", "/tmp//home/" },
            { "/./tmp//home/git/git", "/./tmp/home/git/git" },
            { "/../tmp/./home", "/../tmp/home" },
            { "/../../../tmp/./home", "/../../../tmp/home" },
            { "//tmp///home/..", "//tmp/" },
            { "/tmp/home/git/./.././git/corefx/../", "/tmp/home/git/./git/" },
            { "/./tmp/home/././", "/./tmp/home/" },
        };

        [Theory,
            MemberData(nameof(RemoveRelativeSegmentUnixData))]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void RemoveRelativeSegmentsUnix(string path, string expected)
        {
            Assert.Equal(expected, PathInternal.RemoveRelativeSegments(path));
        }
    }
}
