namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;

    [TestClass]
    public class SendPacketsElementTest
    {
        #region Buffer

        [TestMethod]
        public void NullBufferCtor_Throws()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement((byte[])null);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void NullBufferCtorWithOffset_Throws()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement((byte[])null, 0, 0);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void NullBufferCtorWithEndOfPacket_Throws()
        {
            try
            {
                // Elements with null Buffers are ignored on Send
                SendPacketsElement element = new SendPacketsElement((byte[])null, 0, 0, true);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void EmptyBufferCtor_Success()
        {
            // Elements with empty Buffers are ignored on Send
            SendPacketsElement element = new SendPacketsElement(new byte[0]);
            Assert.IsNotNull(element.Buffer);
            Assert.AreEqual(0, element.Buffer.Length);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(0, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.IsNull(element.FilePath);
        }

        [TestMethod]
        public void BufferCtorNormal_Success()
        {
            SendPacketsElement element = new SendPacketsElement(new byte[10]);
            Assert.IsNotNull(element.Buffer);
            Assert.AreEqual(10, element.Buffer.Length);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(10, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.IsNull(element.FilePath);
        }

        [TestMethod]
        public void BufferCtorNegOffset_ArgumentOutOfRangeException()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement(new byte[10], -1, 11);
                Assert.Fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void BufferCtorNegCount_ArgumentOutOfRangeException()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement(new byte[10], 0, -1);
                Assert.Fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void BufferCtorLargeOffset_ArgumentOutOfRangeException()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement(new byte[10], 11, 1);
                Assert.Fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void BufferCtorLargeCount_ArgumentOutOfRangeException()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement(new byte[10], 5, 10);
                Assert.Fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void BufferCtorEndOfBufferTrue_Success()
        {
            SendPacketsElement element = new SendPacketsElement(new byte[10], 2, 8, true);
            Assert.IsNotNull(element.Buffer);
            Assert.AreEqual(10, element.Buffer.Length);
            Assert.AreEqual(2, element.Offset);
            Assert.AreEqual(8, element.Count);
            Assert.IsTrue(element.EndOfPacket);
            Assert.IsNull(element.FilePath);
        }

        [TestMethod]
        public void BufferCtorEndOfBufferFalse_Success()
        {
            SendPacketsElement element = new SendPacketsElement(new byte[10], 6, 4, false);
            Assert.IsNotNull(element.Buffer);
            Assert.AreEqual(10, element.Buffer.Length);
            Assert.AreEqual(6, element.Offset);
            Assert.AreEqual(4, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.IsNull(element.FilePath);
        }

        [TestMethod]
        public void BufferCtorZeroCount_Success()
        {
            // Elements with empty Buffers are ignored on Send
            SendPacketsElement element = new SendPacketsElement(new byte[0], 0, 0);
            Assert.IsNotNull(element.Buffer);
            Assert.AreEqual(0, element.Buffer.Length);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(0, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.IsNull(element.FilePath);
        }

        #endregion Buffer

        #region File

        [TestMethod]
        public void FileCtorNull_Throws()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement((string)null);
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void FileCtorEmpty_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement(String.Empty);
            Assert.IsNull(element.Buffer);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(0, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.AreEqual(String.Empty, element.FilePath);
        }

        [TestMethod]
        public void FileCtorWhiteSpace_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement("   \t ");
            Assert.IsNull(element.Buffer);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(0, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.AreEqual("   \t ", element.FilePath);
        }

        [TestMethod]
        public void FileCtorNormal_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement("SomeFileName"); // Send whole file
            Assert.IsNull(element.Buffer);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(0, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.AreEqual("SomeFileName", element.FilePath);
        }

        [TestMethod]
        public void FileCtorZeroCountLength_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement("SomeFileName", 0, 0); // Send whole file
            Assert.IsNull(element.Buffer);
            Assert.AreEqual(0, element.Offset);
            Assert.AreEqual(0, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.AreEqual("SomeFileName", element.FilePath);
        }

        [TestMethod]
        public void FileCtorNegOffset_ArgumentOutOfRangeException()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement("SomeFileName", -1, 11);
                Assert.Fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
                return;
            }
        }

        [TestMethod]
        public void FileCtorNegCount_ArgumentOutOfRangeException()
        {
            try
            {
                SendPacketsElement element = new SendPacketsElement("SomeFileName", 0, -1);
                Assert.Fail("Expected ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // expected
                return;
            }
        }

        // File lengths are validated on send

        [TestMethod]
        public void FileCtorEndOfBufferTrue_Success()
        {
            SendPacketsElement element = new SendPacketsElement("SomeFileName", 2, 8, true);
            Assert.IsNull(element.Buffer);
            Assert.AreEqual(2, element.Offset);
            Assert.AreEqual(8, element.Count);
            Assert.IsTrue(element.EndOfPacket);
            Assert.AreEqual("SomeFileName", element.FilePath);
        }

        [TestMethod]
        public void FileCtorEndOfBufferFalse_Success()
        {
            SendPacketsElement element = new SendPacketsElement("SomeFileName", 6, 4, false);
            Assert.IsNull(element.Buffer);
            Assert.AreEqual(6, element.Offset);
            Assert.AreEqual(4, element.Count);
            Assert.IsFalse(element.EndOfPacket);
            Assert.AreEqual("SomeFileName", element.FilePath);
        }

        #endregion File
    }
}
