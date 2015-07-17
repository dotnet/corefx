using Xunit;
using System;
using System.IO;
using System.Text;

namespace Tests
{
    public class BinaryWriterTests
    {

        [Fact]
        public static void BinaryWriter_CtorAndWriteTests1()
        {
            BinaryWriter dw2;
            BinaryReader dr2;
            MemoryStream mstr;

            // [] Smoke test to ensure that we can write with the constructed writer
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write(true);
            dw2.Flush();
            mstr.Position = 0;
            dr2 = new BinaryReader(mstr);
            Assert.IsTrue(dr2.ReadBoolean(), "Error_298yx! Incorrect value on stream");
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_CtorAndWriteTests1_Negative()
        {
            BinaryWriter dw2;
            MemoryStream mstr;

            // [] Should throw ArgumentNullException for null argument
            Assert.Throws<ArgumentNullException>(() => { dw2 = new BinaryWriter(null); }, "Error_1099x! Expected exception not thrown");

            // [] Can't construct a BinaryWriter on a readonly stream
            mstr = new MemoryStream(new byte[10], false);
            Assert.Throws<ArgumentException>(() => { dw2 = new BinaryWriter(mstr); }, "Error_98t4y! Expected exception not thrown");
            mstr.Dispose();

            // [] Can't construct a BinaryWriter with a closed stream
            mstr = new MemoryStream();
            mstr.Dispose();
            Assert.Throws<ArgumentException>(() => { dw2 = new BinaryWriter(mstr); }, "Error_98t4y! Expected exception not thrown");
        }

        [Fact]
        public static void BinaryWriter_EncodingCtorAndWriteTests()
        {
            MemoryStream ms2;
            BinaryReader sr2;
            BinaryWriter sw2;
            String str2;

            // [] Test that we can read and write UTF8
            ms2 = new MemoryStream();
            sw2 = new BinaryWriter(ms2, Encoding.UTF8);
            sw2.Write("This is UTF8\u00FF");
            sw2.Flush();
            ms2.Position = 0;
            sr2 = new BinaryReader(ms2, Encoding.UTF8);
            str2 = sr2.ReadString();
            if (!str2.Equals("This is UTF8\u00FF"))
            {
                Assert.Fail("Error_1y8xx! Incorrect string on stream");
            }
            sr2.Dispose();
            ms2.Dispose();

            // [] Test that we can read and write BigEndian
            string testString = "This is BigEndianUnicode\u00FF";
            ms2 = new MemoryStream();
            sw2 = new BinaryWriter(ms2, Encoding.BigEndianUnicode);
            sw2.Write(testString);
            sw2.Flush();
            ms2.Position = 0;
            sr2 = new BinaryReader(ms2, Encoding.BigEndianUnicode);
            str2 = sr2.ReadString();
            Assert.AreEqual(testString, str2, "Error_8f7yv! Incorrect string on stream==" + str2);
            sr2.Dispose();
            ms2.Dispose();

            // [] Test that we can read and write Unicode
            testString = "This is Unicode\u00FF";
            ms2 = new MemoryStream();
            sw2 = new BinaryWriter(ms2, Encoding.Unicode);
            sw2.Write(testString);
            sw2.Flush();
            ms2.Position = 0;
            sr2 = new BinaryReader(ms2, Encoding.Unicode);
            str2 = sr2.ReadString();
            Assert.AreEqual(testString, str2, "Error_f897h! Incorrect string on stream==" + str2);
            sr2.Dispose();
            ms2.Dispose();
        }

        [Fact]
        public static void BinaryWriter_EncodingCtorAndWriteTests_Negative()
        {
            // [] Check for ArgumentNullException on null stream
            Assert.Throws<ArgumentNullException>(
                () => { new BinaryReader((Stream)null, Encoding.UTF8); },
                "Error_2yc83! Expected exception not thrown");

            // [] Check for ArgumentNullException on null encoding
            Assert.Throws<ArgumentNullException>(
               () => { new BinaryReader(new MemoryStream(), null); },
               "Error_209x7! Expected exception not thrown");
        }

        [Fact]
        public static void BinaryWriter_SeekTests()
        {
            int[] iArrLargeValues = new Int32[] { 10000, 100000, Int32.MaxValue / 200, Int32.MaxValue / 1000, Int16.MaxValue, Int32.MaxValue, Int32.MaxValue - 1, Int32.MaxValue / 2, Int32.MaxValue / 10, Int32.MaxValue / 100 };

            BinaryWriter dw2 = null;
            MemoryStream mstr = null;
            Byte[] bArr = null;
            StringBuilder sb = new StringBuilder();
            Int64 lReturn = 0;

            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("Hello, this is my string".ToCharArray());
            for (int iLoop = 0; iLoop < iArrLargeValues.Length; iLoop++)
            {
                lReturn = dw2.Seek(iArrLargeValues[iLoop], SeekOrigin.Begin);
                if (lReturn != iArrLargeValues[iLoop])
                    Assert.Fail("Error_43434! Incorrect return value, lReturn==" + lReturn);
            }
            dw2.Dispose();
            mstr.Dispose();

            // [] Seek from start of stream
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            lReturn = dw2.Seek(0, SeekOrigin.Begin);

            Assert.IsTrue(lReturn == 0, "Error_9t8vb! Incorrect return value, lReturn==" + lReturn);

            dw2.Write("lki".ToCharArray());
            dw2.Flush();
            bArr = mstr.ToArray();
            sb = new StringBuilder();
            for (int i = 0; i < bArr.Length; i++)
                sb.Append((Char)bArr[i]);
            if (!sb.ToString().Equals("lki3456789"))
                Assert.Fail("Error_837sy! Incorrect sequence in stream, sb==" + sb.ToString());

            dw2.Dispose();
            mstr.Dispose();

            // [] Seek into stream from start
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            lReturn = dw2.Seek(3, SeekOrigin.Begin);
            Assert.IsTrue(lReturn == 3, "Error_209gu! Incorrect return value, lReturn==" + lReturn);

            dw2.Write("lk".ToCharArray());
            dw2.Flush();
            bArr = mstr.ToArray();
            sb = new StringBuilder();
            for (int i = 0; i < bArr.Length; i++)
                sb.Append((Char)bArr[i]);
            if (!sb.ToString().Equals("012lk56789"))
                Assert.Fail("Error_87vhe! Incorrect sequence in stream, sb==" + sb.ToString());

            dw2.Dispose();
            mstr.Dispose();

            // [] Seek from end of stream
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            lReturn = dw2.Seek(-3, SeekOrigin.End);
            Assert.IsTrue(lReturn == 7, "Error_20r45! Incorrec return value, lReturn==" + lReturn);

            dw2.Write("ll".ToCharArray());
            dw2.Flush();
            bArr = mstr.ToArray();
            sb = new StringBuilder();
            for (int i = 0; i < bArr.Length; i++)
                sb.Append((Char)bArr[i]);
            if (!sb.ToString().Equals("0123456ll9"))
                Assert.Fail("Error_874ty! Incorrect sequence in stream, sb==" + sb.ToString());

            dw2.Dispose();
            mstr.Dispose();

            // [] Seeking from current position
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            mstr.Position = 2;
            lReturn = dw2.Seek(2, SeekOrigin.Current);
            Assert.IsTrue(lReturn == 4, "Error_23118! Incorrect return value, lReturn==" + lReturn);

            dw2.Write("ll".ToCharArray());
            dw2.Flush();
            bArr = mstr.ToArray();
            sb = new StringBuilder();
            for (int i = 0; i < bArr.Length; i++)
                sb.Append((Char)bArr[i]);
            if (!sb.ToString().Equals("0123ll6789"))
                Assert.Fail("Error_0198u! Incorrect sequence in stream, sb==" + sb.ToString());

            dw2.Dispose();
            mstr.Dispose();

            // [] Seeking past the end from middle
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            try
            {
                lReturn = dw2.Seek(4, SeekOrigin.End); //This wont throw any exception now.
            }
            catch (Exception exc)
            {
                Assert.Fail("Error_398yg! Incorrect exception thrown, exc==" + exc.ToString());
            }
            Assert.IsTrue(mstr.Position == 14, "Err_1234!! Unexpected position value .." + mstr.Position);

            dw2.Dispose();
            mstr.Dispose();

            // [] Seek pas the end from beginning
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            try
            {
                lReturn = dw2.Seek(11, SeekOrigin.Begin);  //This wont throw any exception now.
            }
            catch (Exception exc)
            {
                Assert.Fail("Error_98yfx! Incorrect exception thrown, exc==" + exc.ToString());
            }

            Assert.IsTrue(mstr.Position == 11, "Err_4321!! Unexpected position value .." + mstr.Position);

            dw2.Dispose();
            mstr.Dispose();

            // [] Seek to the end
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("0123456789".ToCharArray());
            lReturn = dw2.Seek(10, SeekOrigin.Begin);
            Assert.IsTrue(lReturn == 10, "Error_938tv! Incorrect return value, lReturn==" + lReturn);

            dw2.Write("ll".ToCharArray());
            bArr = mstr.ToArray();
            sb = new StringBuilder();
            for (int i = 0; i < bArr.Length; i++)
                sb.Append((Char)bArr[i]);
            if (!sb.ToString().Equals("0123456789ll"))
                Assert.Fail("Error_27g8d! Incorrect stream, sb==" + sb.ToString());

            dw2.Dispose();
            mstr.Dispose();
        }
        [Fact]
        public static void BinaryWriter_SeekTests_Negative()
        {
            int[] iArrInvalidValues = new Int32[] { -1, -2, -100, -1000, -10000, -100000, -1000000, -10000000, -100000000, -1000000000, Int32.MinValue, Int16.MinValue };
            Int64 lReturn = 0;
            
            // [] ArgumentOutOfRangeException if offset is negative
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            dw2.Write("Hello, this is my string".ToCharArray());
            for (int iLoop = 0; iLoop < iArrInvalidValues.Length; iLoop++)
            {
                Assert.Throws<IOException>(() => { lReturn = dw2.Seek(iArrInvalidValues[iLoop], SeekOrigin.Begin); }, "Error_17dy7! Expected exception not thrown");
                Assert.IsTrue(lReturn == 0, "Error_2t8gy! Incorrect return value, lReturn==" + lReturn);
            }
            dw2.Dispose();
            mstr.Dispose();

            // [] ArgumentException for invalid seekOrigin
            mstr = new MemoryStream();
            dw2 = new BinaryWriter(mstr);
            dw2.Write("012345789".ToCharArray());
            Assert.Throws<ArgumentException>(() => { lReturn = dw2.Seek(3, ~SeekOrigin.Begin); }, "Error_109ux! Expected exception not thrown");

            dw2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_BaseStreamTests()
        {
            // [] Get the base stream for MemoryStream
            MemoryStream ms2 = new MemoryStream();
            BinaryWriter sr2 = new BinaryWriter(ms2);
            if (sr2.BaseStream != ms2)
                Assert.Fail("Error_g57hb! Incorrect basestream");

            sr2.Dispose();
            ms2.Dispose();
        }

        [Fact]
        public static void BinaryWriter_FlushTests()
        {
            // [] Check that flush updates the underlying stream
            MemoryStream memstr2 = new MemoryStream();
            BinaryWriter bw2 = new BinaryWriter(memstr2);

            bw2.Write("HelloWorld");
            //TODO:: Ckeck with dev why it's 11 bytes.
            Assert.AreEqual(11, memstr2.Length, "Error_984y7! Incorrect stream length==" + memstr2.Length);
            bw2.Flush();
            Assert.AreEqual(11, memstr2.Length, "Error_8589v! Incorrect stream length after flush==" + memstr2.Length);

            // [] Flushing closed writer should not throw an exception.
            memstr2 = new MemoryStream();
            bw2 = new BinaryWriter(memstr2);
            bw2.Dispose();
            try
            {
                bw2.Flush();
            }
            catch (Exception exc)
            {
                Assert.Fail("Error_399c;! Unexpected exception exception thrown, exc==" + exc.ToString());
            }
        }

        [Fact]
        public static void BinaryWriter_DisposeTests()
        {
            BinaryWriter binaryWriter;
            MemoryStream memoryStream;
            // [] Calling all methods after closing the stream
            memoryStream = new MemoryStream();
            binaryWriter = new BinaryWriter(memoryStream);
            try
            {
                binaryWriter.Dispose();
                binaryWriter.Dispose();
                binaryWriter.Dispose();
            }
            catch (Exception exc)
            {
                Assert.Fail("Error_a1cf3! Unexpected exception thrown, exc==" + exc);
            }

            memoryStream.Dispose();
        }

        [Fact]
        public static void BinaryWriter_DisposeTests_Negative()
        {
            BinaryWriter binaryWriter;
            MemoryStream memoryStream;
            // [] Calling all methods after closing the stream
            memoryStream = new MemoryStream();
            binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Dispose();
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Seek(1, SeekOrigin.Begin), "Error_687yv! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new Byte[2], 0, 2), "Error_9177g! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new Char[2], 0, 2), "Error_2398j! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(true), "Error_10408! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((Byte)4), "Error_17985! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new Byte[] { 1, 2 }), "Error_10989! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write('a'), "ERror_1980f! Expected Exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(new Char[] { 'a', 'b' }), "Error_19876! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(5.3), "Error_1089g! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((Int16)3), "Error_92889! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write(33), "Error_109g7! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((Int64)42), "Error_967gb! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((SByte)4), "Error_16908! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write("Hello There"), "Error_t5087! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((Single)4.3), "Error_5698v! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((UInt16)3), "Error_58743! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((UInt32)4), "Error_58074! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write((UInt64)5), "Error_78884! Expected exception not thrown");
            Assert.Throws<ObjectDisposedException>(() => binaryWriter.Write("Bah"), "Error_3498v! Expected exception not thrown");

            memoryStream.Dispose();
        }
    }
}
