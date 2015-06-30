namespace NCLTest.Primitives
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using NCLTest.Common;
    
    [TestClass]
    public class IPAddressMappingTest
    {
        [TestMethod]
        public void IPAddressMapping_IsIPv4MappedToIPv6_Success()
        {
            Assert.IsTrue(IPAddress.Parse("::FFFF:0:0").IsIPv4MappedToIPv6);
            Assert.IsTrue(IPAddress.Parse("0:0:0:0:0:FFFF::").IsIPv4MappedToIPv6);
            Assert.IsTrue(IPAddress.Parse("::FFFF:" + IPAddress.Loopback.ToString()).IsIPv4MappedToIPv6);
            Assert.IsTrue(IPAddress.Parse("::FFFF:192.168.1.1").IsIPv4MappedToIPv6);
            Assert.IsTrue(IPAddress.Parse("::ffff:192.168.1.1").IsIPv4MappedToIPv6);

            Assert.IsFalse(IPAddress.Parse("1::FFFF:0:0").IsIPv4MappedToIPv6);
            Assert.IsFalse(IPAddress.Loopback.IsIPv4MappedToIPv6);
            Assert.IsFalse(IPAddress.IPv6Loopback.IsIPv4MappedToIPv6);
        }

        [TestMethod]
        public void IPAddressMapping_MapIPv6ToIPv6_Success()
        {
            IPAddress result = IPAddress.IPv6Loopback.MapToIPv6();
            Assert.ReferenceEquals(result, IPAddress.IPv6Loopback);
            Assert.AreEqual(result, IPAddress.IPv6Loopback);
        }

        [TestMethod]
        public void IPAddressMapping_MapIPv4ToIPv4_Success()
        {
            IPAddress result = IPAddress.Loopback.MapToIPv4();
            Assert.ReferenceEquals(result, IPAddress.Loopback);
            Assert.AreEqual(result, IPAddress.Loopback);
        }

        [TestMethod]
        public void IPAddressMapping_MapIPv4ToIPv6_Success()
        {
            IPAddress result = IPAddress.Loopback.MapToIPv6();
            Assert.AreEqual("::ffff:127.0.0.1", result.ToString());
            Assert.AreEqual(IPAddress.Parse("::ffff:127.0.0.1"), result);

            IPAddress roundTrip = result.MapToIPv4();
            Assert.AreEqual(IPAddress.Loopback, roundTrip);
        }

        [TestMethod]
        public void IPAddressMapping_MapIPv6ToIPv4_Success()
        {
            IPAddress result = IPAddress.Parse("::ffff:127.0.0.1").MapToIPv4();
            Assert.AreEqual(IPAddress.Loopback.ToString(), result.ToString());
            Assert.AreEqual(IPAddress.Loopback, result);

            IPAddress roundTrip = result.MapToIPv6();
            Assert.AreEqual(IPAddress.Parse("::ffff:127.0.0.1"), roundTrip);
            Assert.IsTrue(roundTrip.IsIPv4MappedToIPv6);

            IPAddress result2 = IPAddress.Parse("2001:4898:0:fff:0:5efe:10.57.74.64").MapToIPv4();
            Assert.AreEqual(IPAddress.Parse("10.57.74.64"), result2);
        }

        [TestMethod]
        public void IPAddressMapping_VerifyOriginalBugWhenLastByteofIPv4IsGreaterThan127_Success()
        {
            var originalAddress = "1.65.128.190";
            var initAddress = IPAddress.Parse(originalAddress);
            var ipv6Address = initAddress.MapToIPv6();
            Assert.IsTrue(ipv6Address.IsIPv4MappedToIPv6);
            
            var ipv4Address = ipv6Address.MapToIPv4();
            
            Assert.AreEqual(originalAddress, ipv4Address.ToString());
        }        

        [TestMethod]
        public void IPAddressMapping_MapIPv4ToIPv6ToIPv4WhenFirstByteOfIPv4IsGreaterThan127_Success()
        {
            IPAddressMappingHighByteTestHelper("{0}.127.127.127");
        }        
        
        [TestMethod]
        public void IPAddressMapping_MapIPv4ToIPv6ToIPv4WhenSecondByteOfIPv4IsGreaterThan127_Success()
        {
            IPAddressMappingHighByteTestHelper("127.{0}.127.127");
        }        
        
        [TestMethod]
        public void IPAddressMapping_MapIPv4ToIPv6ToIPv4WhenThirdByteOfIPv4IsGreaterThan127_Success()
        {
            IPAddressMappingHighByteTestHelper("127.127.{0}.127");
        }        

        [TestMethod]
        public void IPAddressMapping_MapIPv4ToIPv6ToIPv4WhenLastByteOfIPv4IsGreaterThan127_Success()
        {
            IPAddressMappingHighByteTestHelper("127.127.127.{0}");
        }
        
        private void IPAddressMappingHighByteTestHelper(string ipv4AddressFormat)
        {
            string address;
            IPAddress initialIPv4Address;
            IPAddress ipv6Address;
            IPAddress finalIPv4Address;

            for (var octet = 128; octet < 256; octet++)
            {
                address = string.Format(ipv4AddressFormat, octet);
                
                initialIPv4Address = IPAddress.Parse(address);
                ipv6Address = initialIPv4Address.MapToIPv6();         
                finalIPv4Address = ipv6Address.MapToIPv4();
                
                Assert.AreEqual(address, finalIPv4Address.ToString());
            }
        }
    }
}
