using System.Runtime.InteropServices;
using Xunit;

namespace System.Runtime.Environment.Tests
{
    public class CheckPlatformTests
    {
        [Fact, PlatformSpecific(PlatformID.Windows)]
        public void CheckWindows()
        {
            Assert.True(RuntimeInformation.IsOSPlatform(OSName.Windows));
            Assert.True(RuntimeInformation.IsOSPlatform(new OSName("WINDOWS")));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("windows")));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("Windows NT")));
            Assert.False(RuntimeInformation.IsOSPlatform(OSName.Linux));
            Assert.False(RuntimeInformation.IsOSPlatform(OSName.OSX));
        }

        [Fact, PlatformSpecific(PlatformID.Linux)]
        public void CheckLinux()
        {
            Assert.True(RuntimeInformation.IsOSPlatform(OSName.Linux));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("linux")));
            Assert.True(RuntimeInformation.IsOSPlatform(new OSName("UNIX")));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("ubuntu")));
            Assert.False(RuntimeInformation.IsOSPlatform(OSName.Windows));
            Assert.False(RuntimeInformation.IsOSPlatform(OSName.OSX));
        }

        [Fact, PlatformSpecific(PlatformID.OSX)]
        public void CheckOSX()
        {
            Assert.True(RuntimeInformation.IsOSPlatform(OSName.OSX));
            Assert.True(RuntimeInformation.IsOSPlatform(new OSName("OSX")));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("osx")));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("mac")));
            Assert.False(RuntimeInformation.IsOSPlatform(new OSName("MACOSX")));
            Assert.False(RuntimeInformation.IsOSPlatform(OSName.Linux));
            Assert.False(RuntimeInformation.IsOSPlatform(OSName.OSX));
        }

        [Fact]
        public void CheckOSName()
        {
            OSName winObj = new OSName("WINDOWS");
            OSName winProp = OSName.Windows;
            OSName randomObj = new OSName("random");
            OSName defaultObj = default(OSName);
            OSName nullObj = new OSName(null);
            OSName conObj = new OSName();
            OSName emptyObj = new OSName("");

            Assert.True(winObj == winProp);
            Assert.True(winObj != randomObj);
            Assert.True(defaultObj == nullObj);
            Assert.True(conObj == emptyObj);
            Assert.True(emptyObj == defaultObj);
            Assert.False(winObj == defaultObj);
            Assert.False(winObj == randomObj);
            Assert.False(winObj != winProp);

            Assert.True(winObj.Equals(winProp));
            Assert.True(nullObj.Equals(defaultObj));
            Assert.True(conObj.Equals(emptyObj));
            Assert.False(defaultObj.Equals(winProp));
            Assert.False(winObj.Equals(null));
            Assert.False(winObj.Equals("something"));

            Assert.Equal(winObj.ToString(), "WINDOWS");
            Assert.Equal(winProp.ToString(), "WINDOWS");
            Assert.Equal(defaultObj.ToString(), null);
            Assert.Equal(nullObj.ToString(), null);
            Assert.Equal(conObj.ToString(), null);
            Assert.Equal(emptyObj.ToString(), string.Empty);
            Assert.Equal(randomObj.ToString(), "random");

            Assert.Equal(winObj.GetHashCode(), winProp.GetHashCode());
            Assert.Equal(defaultObj.GetHashCode(), nullObj.GetHashCode());
            Assert.Equal(nullObj.GetHashCode(), conObj.GetHashCode());
        }
    }
}
