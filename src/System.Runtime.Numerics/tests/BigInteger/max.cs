// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Numerics.Tests
{
    public class maxTest
    {
        private static int s_samples = 10;
        private static Random s_random = new Random(100);

        [Fact]
        public static void RunMaxTests()
        {
            byte[] tempByteArray1 = new byte[0];
            byte[] tempByteArray2 = new byte[0];

            // Max Method - Two Large BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = GetRandomByteArray(s_random);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }

            // Max Method - Two Small BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }

            // Max Method - One large and one small BigIntegers
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = GetRandomByteArray(s_random);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }

            // Max Method - One large BigIntegers and zero
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = new byte[] { 0 };
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = new byte[] { 0 };
                tempByteArray2 = GetRandomByteArray(s_random);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }

            // Max Method - One small BigIntegers and zero
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = new byte[] { 0 };
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = new byte[] { 0 };
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }


            // Max Method - One large BigIntegers and -1
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = new byte[] { 0xFF };
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = new byte[] { 0xFF };
                tempByteArray2 = GetRandomByteArray(s_random);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }

            // Max Method - One small BigIntegers and -1
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = new byte[] { 0xFF };
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = new byte[] { 0xFF };
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }


            // Max Method - One large BigIntegers and 1
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random);
                tempByteArray2 = new byte[] { 1 };
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = new byte[] { 1 };
                tempByteArray2 = GetRandomByteArray(s_random);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }

            // Max Method - One small BigIntegers and 1
            for (int i = 0; i < s_samples; i++)
            {
                tempByteArray1 = GetRandomByteArray(s_random, 2);
                tempByteArray2 = new byte[] { 1 };
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");

                tempByteArray1 = new byte[] { 1 };
                tempByteArray2 = GetRandomByteArray(s_random, 2);
                VerifyMaxString(Print(tempByteArray1) + Print(tempByteArray2) + "bMax");
            }
        }

        private static void VerifyMaxString(string opstring)
        {
            StackCalc sc = new StackCalc(opstring);
            while (sc.DoNextOperation())
            {
                Assert.Equal(sc.snCalc.Peek().ToString(), sc.myCalc.Peek().ToString());
            }
        }
        
        private static byte[] GetRandomByteArray(Random random)
        {
            return GetRandomByteArray(random, random.Next(0, 100));
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
