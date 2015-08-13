namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;

    [TestClass]
    public class LingerStateTest
    {
        private void TestLingerState_Success(Socket sock, bool enabled, int lingerTime)
        {
            sock.LingerState = new LingerOption(enabled, lingerTime);

            Assert.AreEqual<bool>(enabled, sock.LingerState.Enabled, "LingerState.Enabled not propogated correctly!");
            Assert.AreEqual<int>(lingerTime, sock.LingerState.LingerTime, "LingerState.LingerTime not propogated correctly!");
        }

        private void TestLingerState_ArgumentException(Socket sock, bool enabled, int lingerTime)
        {
            try
            {
                sock.LingerState = new LingerOption(enabled, lingerTime);
                Assert.Fail("Expected ArgumentException not thrown from set_LingerState!");
            }
            catch (ArgumentException) { }
        }

        [TestMethod]
        public void Socket_LingerState_Boundaries_CorrectBehavior()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Assert.IsFalse(sock.LingerState.Enabled, "Linger was turned on by default!");
            Assert.AreEqual<int>(sock.LingerState.LingerTime, 0, "Default Linger Time not Zero!");

            TestLingerState_ArgumentException(sock, true, -1);

            TestLingerState_Success(sock, true, 0);
            TestLingerState_Success(sock, true, Int16.MaxValue);
            TestLingerState_Success(sock, true, Int16.MaxValue + 1);
            TestLingerState_Success(sock, true, UInt16.MaxValue);

            TestLingerState_ArgumentException(sock, true, UInt16.MaxValue + 1);
        }
    }
}
