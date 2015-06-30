namespace NCLTest.Primitives
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;

    [TestClass]
    public class CookiePortTest
    {
        private CookieContainer cc;
        private Cookie cookie;

        #region Additional test attributes
        [TestInitialize]
        public void TestInitialize() 
        {
            cc = new CookieContainer();
            cookie = new Cookie("name", "value1", "/path", "localhost");
            // use both space and comma as delimiter
            cookie.Port = "\"80 110,1050, 1090 ,1100\"";
            cc.Add(new Uri("http://localhost/path"), cookie);
        }

        public CookiePortTest()
        {
            // Workaround for 916993
            TestInitialize();
        }

        #endregion

        [TestMethod]
        public void Port_SetMultiplePorts_Port1Set()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:80/path"));
            Assert.AreEqual(1, cookies.Count);          
        }

        [TestMethod]
        public void Port_SpaceDelimiter_PortSet()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:110/path"));
            Assert.AreEqual(1, cookies.Count); 
        }

        [TestMethod]
        public void Port_CommaDelimiter_PortSet()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:1050/path"));
            Assert.AreEqual(1, cookies.Count); 
        }

        [TestMethod]
        public void Port_CommaSpaceDelimiter_PortSet()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:1090/path"));
            Assert.AreEqual(1, cookies.Count);
        }

        [TestMethod]
        public void Port_SpaceCommaDelimiter_PortSet()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:1100/path"));
            Assert.AreEqual(1, cookies.Count);
        }

        [TestMethod]
        public void Port_SetMultiplePorts_NoPortMatch()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:1000/path"));
            Assert.AreEqual(0, cookies.Count);
        }

        [TestMethod]
        public void Port_SetMultiplePorts_NoPathMatch()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost:1050"));
            Assert.AreEqual(0, cookies.Count);
        }

        [TestMethod]
        public void Port_NoPortSpecified_ReturnsCookies()
        {
            CookieCollection cookies = cc.GetCookies(new Uri("http://localhost/path"));
            Assert.AreEqual(1, cookies.Count);
        }
    }
}
