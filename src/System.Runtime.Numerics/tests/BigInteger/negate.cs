// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Numerics.Tests
{
    public class negateTest
    {
        private static int s_samples = 10;
        private static Random s_random = new Random(100);

        [Fact]
        public static void RunMinusTests()
        {
            long temp;
            byte[] tempByteArray1 = new byte[0];

            // Negate Method - Large BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                VerifyMinusString(Print(tempByteArray1) + "uNegate");
            }

            // Negate Method - Small BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                VerifyMinusString(Print(tempByteArray1) + "uNegate");
            }

            // Negate Method - zero
            VerifyMinusString("0 uNegate");

            // Negate Method - -1
            VerifyMinusString("-1 uNegate");

            // Negate Method - 1
            VerifyMinusString("1 uNegate");

            temp = Int32.MinValue;
            // Negate Method - Int32.MinValue
            VerifyMinusString(temp.ToString() + " uNegate");

            // Negate Method - Int32.MinValue-1
            VerifyMinusString(temp.ToString() + " -1 b+ uNegate");

            // Negate Method - Int32.MinValue+1
            VerifyMinusString(temp.ToString() + " 1 b+ uNegate");

            temp = Int32.MaxValue;
            // Negate Method - Int32.MaxValue
            VerifyMinusString(temp.ToString() + " uNegate");

            // Negate Method - Int32.MaxValue-1
            VerifyMinusString(temp.ToString() + " -1 b+ uNegate");

            // Negate Method - Int32.MaxValue+1
            VerifyMinusString(temp.ToString() + " 1 b+ uNegate");

            temp = Int64.MinValue;
            // Negate Method - Int64.MinValue
            VerifyMinusString(temp.ToString() + " uNegate");

            // Negate Method - Int64.MinValue-1
            VerifyMinusString(temp.ToString() + " -1 b+ uNegate");

            // Negate Method - Int64.MinValue+1
            VerifyMinusString(temp.ToString() + " 1 b+ uNegate");

            temp = Int64.MaxValue;
            // Negate Method - Int64.MaxValue
            VerifyMinusString(temp.ToString() + " uNegate");

            // Negate Method - Int64.MaxValue-1
            VerifyMinusString(temp.ToString() + " -1 b+ uNegate");

            // Negate Method - Int64.MaxValue+1
            VerifyMinusString(temp.ToString() + " 1 b+ uNegate");
        }

        private static void VerifyMinusString(string opstring)
        {
            StackCalc sc = new StackCalc(opstring);
            while (sc.DoNextOperation())
            {
                Assert.Equal(sc.snCalc.Peek().ToString(), sc.myCalc.Peek().ToString());
            }
        }

        private static void VerifyIdentityString(string opstring1, string opstring2)
        {
            StackCalc sc1 = new StackCalc(opstring1);
            while (sc1.DoNextOperation())
            {
                //Run the full calculation
                sc1.DoNextOperation();
            }

            StackCalc sc2 = new StackCalc(opstring2);
            while (sc2.DoNextOperation())
            {	
                //Run the full calculation
                sc2.DoNextOperation();
            }

            Assert.Equal(sc1.snCalc.Peek().ToString(), sc2.snCalc.Peek().ToString());
        }

        private static byte[] GetRandomByteArray(Random random)
        {
            return GetRandomByteArray(random, random.Next(0, 1024));
        }

        private static byte[] GetRandomByteArray(Random random, int size)
        {
            return MyBigIntImp.GetRandomByteArray(random, size);
        }

        private static String Print(byte[] bytes)
        {
            return MyBigIntImp.Print(bytes);
        }
    }
}
