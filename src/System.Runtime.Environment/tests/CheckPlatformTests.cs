using System.Runtime.InteropServices;
using Xunit;

namespace System.Runtime.Environment.Tests
{
    public class CheckPlatformTests
    {
        [Fact, PlatformSpecific(PlatformID.Windows)]
        public void CheckWindows()
        {
            Assert.True(RuntimeInfo.IsOSPlatform(OSName.Windows));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("WINDOWS")));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("windows")));
            Assert.False(RuntimeInfo.IsOSPlatform(new OSName("Windows NT")));
            Assert.False(RuntimeInfo.IsOSPlatform(OSName.Linux));
            Assert.False(RuntimeInfo.IsOSPlatform(OSName.OSX));
        }

        [Fact, PlatformSpecific(PlatformID.Linux)]
        public void CheckLinux()
        {
            Assert.True(RuntimeInfo.IsOSPlatform(OSName.Linux));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("linux")));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("UNIX")));
            Assert.False(RuntimeInfo.IsOSPlatform(new OSName("ubuntu")));
            Assert.False(RuntimeInfo.IsOSPlatform(OSName.Windows));
            Assert.False(RuntimeInfo.IsOSPlatform(OSName.OSX));
        }

        [Fact, PlatformSpecific(PlatformID.OSX)]
        public void CheckOSX()
        {
            Assert.True(RuntimeInfo.IsOSPlatform(OSName.OSX));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("osx")));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("mac")));
            Assert.True(RuntimeInfo.IsOSPlatform(new OSName("MACOSX")));
            Assert.False(RuntimeInfo.IsOSPlatform(OSName.Linux));
            Assert.False(RuntimeInfo.IsOSPlatform(OSName.OSX));
        }
    }
}
