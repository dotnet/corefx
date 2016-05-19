// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Net.Primitives.Functional.Tests
{
    public class IPAddressParsing
    {
        #region IPv4

        [Theory]
        [InlineData("192.168.0.1", "192.168.0.1")]
        [InlineData("0.0.0.0", "0.0.0.0")]
        [InlineData("0", "0.0.0.0")]
        [InlineData("0x0", "0.0.0.0")]
        [InlineData("255.255.255.255", "255.255.255.255")]
        [InlineData("0xFF.0xFF.0xFF.0xFF", "255.255.255.255")]
        [InlineData("0377.0377.0377.0377", "255.255.255.255")]
        [InlineData("4294967294", "255.255.255.254")]
        [InlineData("4294967295", "255.255.255.255")]
        [InlineData("0xFFFFFFFE", "255.255.255.254")]
        [InlineData("0xFFFFFFFF", "255.255.255.255")]
        [InlineData("037777777776", "255.255.255.254")] // Octal
        [InlineData("037777777777", "255.255.255.255")] // Octal
        [InlineData("2637895963", "157.59.25.27")]
        [InlineData("157.3873051", "157.59.25.27")]
        [InlineData("157.6427", "157.0.25.27")]
        [InlineData("0x9D3B191B", "157.59.25.27")]
        [InlineData("0X9D.0x3B.0X19.0x1B", "157.59.25.27")]
        [InlineData("0x89.0xab.0xcd.0xef", "137.171.205.239")]
        [InlineData("157.59.25.0x1B", "157.59.25.27")]
        [InlineData("157.59.0x001B", "157.59.0.27")]
        [InlineData("157.0x00001B", "157.0.0.27")]
        [InlineData("023516614433", "157.59.25.27")] // Octal
        [InlineData("00000023516614433", "157.59.25.27")] // Octal
        [InlineData("157.59.0x25.033", "157.59.37.27")]
        public void ParseIPv4_Success(string address, string expected)
        {
            Assert.Equal(expected, IPAddress.Parse(address).ToString());
        }

        [Theory]
        [ActiveIssue(8362, ~PlatformID.OSX)]
        [InlineData("000235.000073.0000031.00000033", "157.59.25.27")] // Octal
        [InlineData("0235.073.031.033", "157.59.25.27")] // Octal
        [InlineData("157.59.25.033", "157.59.25.27")] // Partial octal
        public void ParseIPv4_Success_Octal_NonOSX(string address, string expected)
        {
            Assert.Equal(expected, IPAddress.Parse(address).ToString());
        }

        [Theory]
        [InlineData("192.168.0.0/16")] // with subnet
        [InlineData("192.168.0.1:80")] // with port
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" 127.0.0.1")]
        [InlineData("157.3B191B")] // Hex without 0x
        [InlineData("260.156")] // Left dotted segments can't be more than 255
        [InlineData("255.260.156")] // Left dotted segments can't be more than 255
        [InlineData("1.1.1.0x")] // Empty trailing hex segment
        [InlineData("...")] // Empty sections
        [InlineData("1.1.1.")] // Empty trailing section
        [InlineData("1..1.1")] // Empty internal section
        [InlineData(".1.1.1")] // Empty leading section
        [InlineData("..11.1")] // Empty sections
        [InlineData("0xFF.0xFFFFFF.0xFF")] // Middle segment too large
        [InlineData("0xFFFFFF.0xFF.0xFFFFFF")] // Leading segment too large
        [InlineData("1.1\u67081.1.1")] // Unicode, Crashes .NET 4.0 IPAddress.TryParse
        [InlineData("0000X9D.0x3B.0X19.0x1B")] // Leading zeros on hex
        public void ParseIPv4_Failure(string address)
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(address); });
        }

        [Theory]
        [PlatformSpecific(~PlatformID.OSX)] // There does't appear to be an OSX API that will fail for these
        [InlineData("4294967296")] // Decimal overflow by 1
        [InlineData("040000000000")] // Octal overflow by 1
        [InlineData("01011101001110110001100100011011")] // Binary? Read as octal, overflows
        [InlineData("10011101001110110001100100011011")] // Binary? Read as decimal, overflows
        [InlineData("0x100000000")] // Hex overflow by 1
        [InlineData("0x.1.1.1")] // Empty leading hex segment
        public void ParseIPv4_Failure_NonOSX(string address)
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(address); });
        }

        [Theory]
        [ActiveIssue(8362, ~PlatformID.OSX)]
        [InlineData("0.0.0.089")] // Octal (leading zero) but with 8 or 9
        public void ParseIPv4_Failure_Octal_NonOSX(string address)
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(address); });
        }

        #endregion

        #region IPv6

        [Theory]
        [InlineData("Fe08::1", "fe08::1")]
        [InlineData("[Fe08::1]", "fe08::1")]
        [InlineData("[Fe08::1]:80", "fe08::1")]
        [InlineData("[Fe08::1]:0xFA", "fe08::1")]
        [InlineData("Fe08::1%13542", "fe08::1%13542")]
        [InlineData("::", "::")]
        [InlineData("0000:0000:0000:0000:0000:0000:0000:0000", "::")]
        [InlineData("FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
        [InlineData("0:0:0:0:0:0:0:0", "::")]
        [InlineData("1:0:0:0:0:0:0:0", "1::")]
        [InlineData("0:1:0:0:0:0:0:0", "0:1::")]
        [InlineData("0:0:1:0:0:0:0:0", "0:0:1::")]
        [InlineData("0:0:0:1:0:0:0:0", "0:0:0:1::")]
        [InlineData("0:0:0:0:1:0:0:0", "::1:0:0:0")]
        [InlineData("0:0:0:0:0:1:0:0", "::1:0:0")]
        [InlineData("0:0:0:0:0:0:1:0", "::0.1.0.0")]
        [InlineData("0:0:0:0:0:0:0:1", "::1")]
        [InlineData("1:0:0:0:0:0:0:1", "1::1")]
        [InlineData("1:1:0:0:0:0:0:1", "1:1::1")]
        [InlineData("1:0:1:0:0:0:0:1", "1:0:1::1")]
        [InlineData("1:0:0:1:0:0:0:1", "1:0:0:1::1")]
        [InlineData("1:0:0:0:1:0:0:1", "1::1:0:0:1")]
        [InlineData("1:0:0:0:0:1:0:1", "1::1:0:1")]
        [InlineData("1:0:0:0:0:0:1:1", "1::1:1")]
        [InlineData("1:1:0:0:1:0:0:1", "1:1::1:0:0:1")]
        [InlineData("1:0:1:0:0:1:0:1", "1:0:1::1:0:1")]
        [InlineData("1:0:0:1:0:0:1:1", "1::1:0:0:1:1")]
        [InlineData("1:1:0:0:0:1:0:1", "1:1::1:0:1")]
        [InlineData("1:0:0:0:1:0:1:1", "1::1:0:1:1")]
        [InlineData("1::%1", "1::%1")]
        [InlineData("::1%12", "::1%12")]
        [InlineData("::%123", "::%123")]
        [InlineData("FE08::192.168.0.1", "fe08::c0a8:1")] // Output is not IPv4 mapped
        [InlineData("::192.168.0.1", "::192.168.0.1")]
        [InlineData("::FFFF:192.168.0.1", "::ffff:192.168.0.1")] // SIIT
        public void ParseIPv6_Success(string address, string expected)
        {
            Assert.Equal(expected, IPAddress.Parse(address).ToString());
        }

        [Theory]
        [PlatformSpecific(~PlatformID.AnyUnix)] // Linux/OSX don't recognize these as IPv4 addresses
        // Linux/OSX don't do the IPv6->IPv4 formatting for these addresses
        [InlineData("::FFFF:0:192.168.0.1", "::ffff:0:192.168.0.1")] // SIIT
        [InlineData("::5EFE:192.168.0.1", "::5efe:192.168.0.1")] // ISATAP
        [InlineData("1::5EFE:192.168.0.1", "1::5efe:192.168.0.1")] // ISATAP
        public void ParseIPv6_Success_v4_NonUnix(string address, string expected)
        {
            Assert.Equal(expected, IPAddress.Parse(address).ToString());
        }

        [Theory]
        [PlatformSpecific(~PlatformID.Linux)] // Linux does not appear to recognize this as a valid address
        [InlineData("::192.168.0.010", "::192.168.0.10")] // Embedded IPv4 octal, read as decimal
        public void ParseIPv6_Success_v4_NonLinux(string address, string expected)
        {
            Assert.Equal(expected, IPAddress.Parse(address).ToString());
        }

        [Theory]
        [InlineData("[Fe08::1")]
        [InlineData("Fe08::1]")]
        [InlineData("[Fe08::1]:80Z")]
        [InlineData("Fe08::/64")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("[1]")]
        [InlineData(":1")]
        [InlineData("1:")]
        [InlineData("::1 ")]
        [InlineData(" ::1")]
        [InlineData("1::1::1")] // Ambigious
        [InlineData("1:1\u67081:1:1")] // Unicoded. Crashes .NET 4.0 IPAddress.TryParse
        [InlineData("FE08::260.168.0.1")] // Embedded IPv4 out of range
        [InlineData("::192.168.0.0x0")] // Embedded IPv4 hex
        [InlineData("[192.168.0.1]")] // Raw IPv4
        [InlineData("G::")] // Hex out of range
        [InlineData("FFFFF::")] // Hex out of range
        [InlineData(":%12")] // Colon Scope
        [InlineData("%12")] // Just Scope
        public void ParseIPv6_Failure(string address)
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(address); });
        }

        [Theory]
        [PlatformSpecific(~PlatformID.OSX)]
        [InlineData("::%1a")] // Alpha numeric Scope
        public void ParseIPv6_Failure_NonOSX(string address)
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(address); });
        }


        #endregion

        [Fact]
        public void Parse_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => { IPAddress.Parse(null); });
        }

        [Fact]
        public void TryParse_Null_False()
        {
            IPAddress ipAddress;
            Assert.False(IPAddress.TryParse(null, out ipAddress));
        }

        [Fact]
        public void Parse_Empty_Throws()
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(String.Empty); });
        }

        [Fact]
        public void TryParse_Empty_False()
        {
            IPAddress ipAddress;
            Assert.False(IPAddress.TryParse(String.Empty, out ipAddress));
        }
    }
}
