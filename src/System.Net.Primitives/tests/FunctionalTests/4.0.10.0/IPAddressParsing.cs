namespace NCLTest.Primitives
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using NCLTest.Common;

    [TestClass]
    public class IPAddressParsing
    {
        #region IPv4

        [TestMethod]
        public void ParseIPv4_Basic_Success()
        {
            Assert.AreEqual("192.168.0.1", IPAddress.Parse("192.168.0.1").ToString());
        }

        [TestMethod]
        public void ParseIPv4_WithSubnet_Failure()
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse("192.168.0.0/16"); });
        }

        [TestMethod]
        public void ParseIPv4_WithPort_Failure()
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse("192.168.0.1:80"); });
        }

        #endregion IPv4

        #region IPv6

        [TestMethod]
        public void ParseIPv6_NoBrackets_Success()
        {
            TestRequirements.CheckIPv6Support();
            Assert.AreEqual("fe08::1", IPAddress.Parse("Fe08::1").ToString());
        }

        [TestMethod]
        public void ParseIPv6_Brackets_SuccessBracketsDropped()
        {
            TestRequirements.CheckIPv6Support();
            Assert.AreEqual("fe08::1", IPAddress.Parse("[Fe08::1]").ToString());
        }

        [TestMethod]
        public void ParseIPv6_LeadingBracket_Failure()
        {
            TestRequirements.CheckIPv6Support();
            Assert.Throws<FormatException>(() => { IPAddress.Parse("[Fe08::1"); });
        }

        [TestMethod]
        public void ParseIPv6_TrailingBracket_Failure()
        {
            TestRequirements.CheckIPv6Support();
            Assert.Throws<FormatException>(() => { IPAddress.Parse("Fe08::1]"); });
        }

        [TestMethod]
        public void ParseIPv6_BracketsAndPort_SuccessBracketsAndPortDropped()
        {
            TestRequirements.CheckIPv6Support();
            Assert.AreEqual("fe08::1", IPAddress.Parse("[Fe08::1]:80").ToString());
        }

        [TestMethod]
        public void ParseIPv6_BracketsAndInvalidPort_Failure()
        {
            TestRequirements.CheckIPv6Support(); // Different behavior otherwise
            Assert.Throws<FormatException>(() => { IPAddress.Parse("[Fe08::1]:80Z"); });
        }

        [TestMethod]
        public void ParseIPv6_BracketsAndHexPort_SuccessBracketsAndPortDropped()
        {
            TestRequirements.CheckIPv6Support();
            Assert.AreEqual("fe08::1", IPAddress.Parse("[Fe08::1]:0xFA").ToString());
        }

        [TestMethod]
        public void ParseIPv6_WithSubnet_Failure()
        {
            TestRequirements.CheckIPv6Support();
            Assert.Throws<FormatException>(() => { IPAddress.Parse("Fe08::/64"); });
        }

        [TestMethod]
        public void ParseIPv6_ScopeId_Success()
        {
            TestRequirements.CheckIPv6Support();
            Assert.AreEqual("fe08::1%13542", IPAddress.Parse("Fe08::1%13542").ToString());
        }

        #endregion IPv6

        [TestMethod]
        public void Parse_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => { IPAddress.Parse(null); });
        }
        
        [TestMethod]
        public void TryParse_Null_False()
        {
            IPAddress ipAddress;
            Assert.IsFalse(IPAddress.TryParse(null, out ipAddress));
        }

        [TestMethod]
        public void Parse_Empty_Throws()
        {
            Assert.Throws<FormatException>(() => { IPAddress.Parse(String.Empty); });
        }

        [TestMethod]
        public void TryParse_Empty_False()
        {
            IPAddress ipAddress;
            Assert.IsFalse(IPAddress.TryParse(String.Empty, out ipAddress));
        }
    }
}
