using Xunit;
using System;
using System.IO;
using System.Text;

namespace BinaryWriterTests
{
    public class BinaryWriter_WriteTests
    {

        [Fact]
        public static void BinaryWriter_WriteBoolTest()
        {
            // [] Write a series of booleans to a stream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            dw2.Write(false);
            dw2.Write(false);
            dw2.Write(true);
            dw2.Write(false);
            dw2.Write(true);
            dw2.Write(5);
            dw2.Write(0);

            dw2.Flush();
            mstr.Position = 0;

            BinaryReader dr2 = new BinaryReader(mstr);
            if (dr2.ReadBoolean())
            {
                Assert.Fail("Error_948yg! Incorrect value in stream");
            }
            if (dr2.ReadBoolean())
            {
                Assert.Fail("Error_t598h! Incorrect value in stream");
            }
            if (!dr2.ReadBoolean())
            {
                Assert.Fail("Error_9084b! Incorrect value in stream");
            }
            if (dr2.ReadBoolean())
            {
                Assert.Fail("Error_209uc! Incorrect value in stream");
            }
            if (!dr2.ReadBoolean())
            {
                Assert.Fail("Error_98y8c! Incorrect value in stream");
            }
            if (!dr2.ReadBoolean())
            {
                Assert.Fail("Error_198xy! Incorrect value in stream");
            }
            if (dr2.ReadBoolean())
            {
                Assert.Fail("Error_39t8g! Incorrect value in stream");
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteSingleTest()
        {
            Single sgl2 = 0;
            int ii = 0;
            Single[] sglArr = new Single[] { 
                Single.MinValue, Single.MaxValue, Single.Epsilon, Single.PositiveInfinity, Single.NegativeInfinity, new Single(), 
                0, (Single)(-1E20), (Single)(-3.5E-20), (Single)(1.4E-10), (Single)10000.2, (Single)2.3E30 
            };
            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);


            for (ii = 0; ii < sglArr.Length; ii++)
                dw2.Write(sglArr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < sglArr.Length; ii++)
            {
                if ((sgl2 = dr2.ReadSingle()) != sglArr[ii])
                    Assert.Fail("Error_48yf4_" + ii + "! Expected==" + sglArr[ii] + " , got==" + sgl2);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteSingleTest_Negative()
        {
            Single sgl2 = 0;
            int ii = 0;
            Single[] sglArr = new Single[] { 
                Single.MinValue, Single.MaxValue, Single.Epsilon, Single.PositiveInfinity, Single.NegativeInfinity, new Single(), 
                0, (Single)(-1E20), (Single)(-3.5E-20), (Single)(1.4E-10), (Single)10000.2, (Single)2.3E30 
            };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            for (ii = 0; ii < sglArr.Length; ii++)
                dw2.Write(sglArr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);
            for (ii = 0; ii < sglArr.Length; ii++)
                sgl2 = dr2.ReadSingle();

            // [] Check End Of Stream
            try
            {
                sgl2 = dr2.ReadSingle();
                Assert.Fail("Error_2d847! Expected exception not thrown, sgl2==" + sgl2);
            }
            catch (IOException)
            {
            }
            catch (Exception exc)
            {
                Assert.Fail("Error_238gy! Unexpected exception thrown, exc==" + exc.ToString());
            }
        }

        [Fact]
        public static void BinaryWriter_WriteDecimalTest()
        {
            Decimal dbl2 = 0;
            int ii = 0;
            Decimal[] dblArr = new Decimal[] { 
                Decimal.One, Decimal.Zero, Decimal.MinusOne, Decimal.MinValue, Decimal.MaxValue, 
                new Decimal(-1000.5), new Decimal(-10.0E-40), new Decimal(3.4E-40898), new Decimal(3.4E-28), 
                new Decimal(3.4E+28), new Decimal(0.45), new Decimal(5.55), new Decimal(3.4899E23) 
            };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < dblArr.Length; ii++)
                dw2.Write(dblArr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < dblArr.Length; ii++)
            {
                if ((dbl2 = dr2.ReadDecimal()) != dblArr[ii])
                    Assert.Fail("Error_948yg! Expected==" + dblArr[ii] + ", got==" + dbl2);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteDecimalTest_Negative()
        {
            Decimal dbl2 = 0;
            int ii = 0;
            Decimal[] dblArr = new Decimal[] { 
                Decimal.One, Decimal.Zero, Decimal.MinusOne, Decimal.MinValue, Decimal.MaxValue, 
                new Decimal(-1000.5), new Decimal(-10.0E-40), new Decimal(3.4E-40898), new Decimal(3.4E-28), 
                new Decimal(3.4E+28), new Decimal(0.45), new Decimal(5.55), new Decimal(3.4899E23) 
            };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < dblArr.Length; ii++)
                dw2.Write(dblArr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < dblArr.Length; ii++)
                dbl2 = dr2.ReadDecimal();

            try
            {
                dbl2 = dr2.ReadDecimal();
                Assert.Fail("EndOfStreamException expected");
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected but got: " + e);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteDoubleTest()
        {
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            BinaryReader dr2 = new BinaryReader(mstr);

            Double dbl2 = 0;
            Double[] dblArr = new Double[0];
            int ii = 0;

            dblArr = new Double[] { 
                Double.NegativeInfinity, Double.PositiveInfinity, Double.Epsilon, Double.MinValue, Double.MaxValue, 
                -3E59, -1000.5, -1E-40, 3.4E-37, 0.45, 5.55, 3.4899E233 
            };

            // [] read/Write with Memorystream
            for (ii = 0; ii < dblArr.Length; ii++)
                dw2.Write(dblArr[ii]);

            dw2.Flush();
            mstr.Position = 0;

            for (ii = 0; ii < dblArr.Length; ii++)
            {
                if ((dbl2 = dr2.ReadDouble()) != dblArr[ii])
                    Assert.Fail("Error_948yg! Expected==" + dblArr[ii] + ", got==" + dbl2);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteDoubleTest_Negative()
        {
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            Double[] dblArr = new Double[0];
            int ii = 0;

            dblArr = new Double[] { 
                Double.NegativeInfinity, Double.PositiveInfinity, Double.Epsilon, Double.MinValue, Double.MaxValue, 
                -3E59, -1000.5, -1E-40, 3.4E-37, 0.45, 5.55, 3.4899E233 
            };

            // [] read/Write with Memorystream
            for (ii = 0; ii < dblArr.Length; ii++)
                dw2.Write(dblArr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < dblArr.Length; ii++)
                dr2.ReadDouble();

            try
            {
                dr2.ReadDouble();
                Assert.Fail("EndOfStreamException expected.");
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteInt16Test()
        {
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            BinaryReader dr2 = new BinaryReader(mstr);
            Int16 i16a = 0;
            Int16[] i16Arr = new Int16[0];
            int ii = 0;

            // [] read/Write with Memorystream
            i16Arr = new Int16[] { Int16.MinValue, Int16.MaxValue, 0, -10000, 10000, -50, 50 };

            for (ii = 0; ii < i16Arr.Length; ii++)
                dw2.Write(i16Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;

            for (ii = 0; ii < i16Arr.Length; ii++)
            {
                if ((i16a = dr2.ReadInt16()) != i16Arr[ii])
                    Assert.Fail("Error_398xu_" + ii + "! Expected==" + i16Arr[ii] + " , got==" + i16a);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteInt16Test_Negative()
        {
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            BinaryReader dr2 = new BinaryReader(mstr);
            Int16 i16a = 0;
            Int16[] i16Arr = new Int16[] { Int16.MinValue, Int16.MaxValue, 0, -10000, 10000, -50, 50 };
            int ii = 0;

            // [] read/Write with Memorystream
            for (ii = 0; ii < i16Arr.Length; ii++)
                dw2.Write(i16Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;

            for (ii = 0; ii < i16Arr.Length; ii++)
                i16a = dr2.ReadInt16();

            // [] Check End Of Stream
            try
            {
                i16a = dr2.ReadInt16();
                Assert.Fail("Error_2d847! Expected exception not thrown, i16a==" + i16a);
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteInt32Test()
        {
            Int32 i32a = 0;
            int ii = 0;

            // [] read/Write with  Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            Int32[] i32arr = new Int32[] { Int32.MinValue, Int32.MaxValue, 0, -10000, 10000, -50, 50 };

            for (ii = 0; ii < i32arr.Length; ii++)
                dw2.Write(i32arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < i32arr.Length; ii++)
            {
                if ((i32a = dr2.ReadInt32()) != i32arr[ii])
                    Assert.Fail("Error_298hg! Expected==" + i32arr[ii] + " , got==" + i32a);
            }

            dr2.Dispose();
            dw2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteInt32Test_Negative()
        {
            Int32 i32a = 0;
            int ii = 0;

            // [] read/Write with  Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            Int32[] i32arr = new Int32[] { Int32.MinValue, Int32.MaxValue, 0, -10000, 10000, -50, 50 };

            for (ii = 0; ii < i32arr.Length; ii++)
                dw2.Write(i32arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < i32arr.Length; ii++)
                i32a = dr2.ReadInt32();

            // [] Check End Of Stream
            try
            {
                i32a = dr2.ReadInt32();
                Assert.Fail("Error_2d847! Expected exception not thrown, i32a==" + i32a);
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }
            dr2.Dispose();
            dw2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteInt64Test()
        {
            Int64 i64a = 0;
            int ii = 0;
            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            Int64[] i64arr = new Int64[] { Int64.MinValue, Int64.MaxValue, 0, -10000, 10000, -50, 50 };
            for (ii = 0; ii < i64arr.Length; ii++)
                dw2.Write(i64arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < i64arr.Length; ii++)
            {
                if ((i64a = dr2.ReadInt64()) != i64arr[ii])
                    Assert.Fail("Error_48yf4_" + ii + "! Expected==" + i64arr[ii] + " , got==" + i64a);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteInt64Test_Negative()
        {
            Int64 i64a = 0;
            int ii = 0;
            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            Int64[] i64arr = new Int64[] { Int64.MinValue, Int64.MaxValue, 0, -10000, 10000, -50, 50 };
            for (ii = 0; ii < i64arr.Length; ii++)
                dw2.Write(i64arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < i64arr.Length; ii++)
                i64a = dr2.ReadInt64();

            // [] Check End Of Stream
            try
            {
                i64a = dr2.ReadInt64();
                Assert.Fail("Error_2d847! Expected exception not thrown, i64a==" + i64a);
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }
            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteUInt16Test()
        {
            UInt16 ui16a = 0;
            int ii = 0;
            UInt16[] ui16Arr = new UInt16[] { UInt16.MinValue, UInt16.MaxValue, 0, 100, 1000, 10000, UInt16.MaxValue - 100 };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < ui16Arr.Length; ii++)
                dw2.Write(ui16Arr[ii]);

            dw2.Flush();

            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < ui16Arr.Length; ii++)
            {
                if ((ui16a = dr2.ReadUInt16()) != ui16Arr[ii])
                    Assert.Fail("Error_48yf4_" + ii + "! Expected==" + ui16Arr[ii] + " , got==" + ui16a);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteUInt16Test_Negative()
        {
            UInt16 ui16a = 0;
            int ii = 0;
            UInt16[] ui16Arr = new UInt16[] { UInt16.MinValue, UInt16.MaxValue, 0, 100, 1000, 10000, UInt16.MaxValue - 100 };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < ui16Arr.Length; ii++)
                dw2.Write(ui16Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < ui16Arr.Length; ii++)
                ui16a = dr2.ReadUInt16();

            try
            {
                ui16a = dr2.ReadUInt16();
                Assert.Fail("Error_2d847! Expected exception not thrown, ui16a==" + ui16a);
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteUInt32Test()
        {
            UInt32 ui32a = 0;
            int ii = 0;
            UInt32[] ui32Arr = new UInt32[] { UInt32.MinValue, UInt32.MaxValue, 0, 100, 1000, 10000, UInt32.MaxValue - 100 };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < ui32Arr.Length; ii++)
                dw2.Write(ui32Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;

            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < ui32Arr.Length; ii++)
            {
                if ((ui32a = dr2.ReadUInt32()) != ui32Arr[ii])
                    Assert.Fail("Error_48yf4_" + ii + "! Expected==" + ui32Arr[ii] + " , got==" + ui32a);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteUInt32Test_Negative()
        {
            UInt32 ui32a = 0;
            int ii = 0;
            UInt32[] ui32Arr = new UInt32[] { UInt32.MinValue, UInt32.MaxValue, 0, 100, 1000, 10000, UInt32.MaxValue - 100 };

            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < ui32Arr.Length; ii++)
                dw2.Write(ui32Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;

            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < ui32Arr.Length; ii++)
                ui32a = dr2.ReadUInt32();

            // [] Check End Of Stream
            try
            {
                ui32a = dr2.ReadUInt32();
                Assert.Fail("Error_2d847! Expected exception not thrown, ui32a==" + ui32a);
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteUInt64Test()
        {
            UInt64 ui64a = 0;
            int ii = 0;

            UInt64[] ui64Arr = new UInt64[] { UInt64.MinValue, UInt64.MaxValue, 0, 100, 1000, 10000, UInt64.MaxValue - 100 };
            // [] read/Write with Memorystream

            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < ui64Arr.Length; ii++)
                dw2.Write(ui64Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < ui64Arr.Length; ii++)
            {
                if ((ui64a = dr2.ReadUInt64()) != ui64Arr[ii])
                    Assert.Fail("Error_48yf4_" + ii + "! Expected==" + ui64Arr[ii] + " , got==" + ui64a);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteUInt64Test_Negative()
        {
            UInt64 ui64a = 0;
            int ii = 0;

            UInt64[] ui64Arr = new UInt64[] { UInt64.MinValue, UInt64.MaxValue, 0, 100, 1000, 10000, UInt64.MaxValue - 100 };
            // [] read/Write with Memorystream

            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < ui64Arr.Length; ii++)
                dw2.Write(ui64Arr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader dr2 = new BinaryReader(mstr);

            for (ii = 0; ii < ui64Arr.Length; ii++)
                ui64a = dr2.ReadUInt64();

            // [] Check End Of Stream

            try
            {
                ui64a = dr2.ReadUInt64();
                Assert.Fail("Error_2d847! Expected exception not thrown, ui64a==" + ui64a);
            }
            catch (EndOfStreamException)
            { }
            catch (Exception e)
            {
                Assert.Fail("EndOfStreamException expected. Got: " + e);
            }

            dw2.Dispose();
            dr2.Dispose();
            mstr.Dispose();
        }

        [Fact]
        public static void BinaryWriter_WriteStringTest()
        {
            String str2 = String.Empty;
            int ii = 0;
            Byte[] tempbyt = new Byte[1];

            StringBuilder sb = new StringBuilder();
            String str1;
            for (ii = 0; ii < 5; ii++)
                sb.Append("abc");
            str1 = sb.ToString();

            String[] strArr = new String[] { 
                "ABC", "\t\t\n\n\n\0\r\r\v\v\t\0\rHello", "This is a normal string", "12345667789!@#$%^&&())_+_)@#", 
                "ABSDAFJPIRUETROPEWTGRUOGHJDOLJHLDHWEROTYIETYWsdifhsiudyoweurscnkjhdfusiyugjlskdjfoiwueriye", "     ", 
                "\0\0\0\t\t\tHey\"\"", "\u0022\u0011", str1, String.Empty };
            // [] read/Write with Memorystream
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);

            for (ii = 0; ii < strArr.Length; ii++)
                dw2.Write(strArr[ii]);

            dw2.Flush();
            mstr.Position = 0;
            BinaryReader br = new BinaryReader(mstr);
            for (ii = 0; ii < strArr.Length; ii++)
            {
                str2 = br.ReadString();
                for (int jj = 0; jj < str2.Length; jj++)
                {
                    if (str2[jj] != strArr[ii][jj])
                        Assert.Fail("Error_298hg_" + ii + "! Expected==" + strArr[ii][jj] + " , got==" + str2[jj]);
                }
            }

            mstr.Dispose();
            dw2.Dispose();
            br.Dispose();
        }
        [Fact]
        public static void BinaryWriter_WriteStringTest_Negative()
        {
            // [] ArgumentNullException for null argument
            MemoryStream mstr = new MemoryStream();
            BinaryWriter dw2 = new BinaryWriter(mstr);
            Assert.Throws<ArgumentNullException>(() => dw2.Write((String)null), "Error_398ty! Expected exception not thrown");
            mstr.Dispose();
            dw2.Dispose();
        }
    }
}
