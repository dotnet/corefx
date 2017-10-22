// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System
{
    public class Perf_Convert
    {
        [Benchmark(InnerIterationCount = 100000)]
        [InlineData(1024, 1408, Base64FormattingOptions.InsertLineBreaks)]
        [InlineData(1024, 1368, Base64FormattingOptions.None)]
        public void ToBase64CharArray(int binaryDataSize, int arraySize, Base64FormattingOptions formattingOptions)
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte[] binaryData = new byte[binaryDataSize];
            char[] base64CharArray = new char[arraySize];

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBase64CharArray(binaryData, 0, binaryDataSize, base64CharArray, 0, formattingOptions);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000)]
        [InlineData(Base64FormattingOptions.InsertLineBreaks)]
        [InlineData(Base64FormattingOptions.None)]
        public void ToBase64String(Base64FormattingOptions formattingOptions)
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte[] binaryData = new byte[1024];

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBase64String(binaryData, formattingOptions);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 380000000)]
        public void ToBoolean_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 380000000)]
        public void ToBoolean_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 1;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToBoolean_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(100.0m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToBoolean_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(100.0);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToBoolean_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(100);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 5000000)]
        public void ToBoolean_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean("true");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToBoolean_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToBoolean_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            float value = 1;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToBoolean(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToByte_Char()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte('A');
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToByte_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToByte_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte(1.0m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToByte_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte(1.0);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToByte_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte(1);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToByte_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToByte_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            float value = 1.1f;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 2000000)]
        public void ToByte_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToByte("52");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToChar_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 125;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToChar(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToChar_Char()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToChar('c');
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToChar_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToChar(100);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToChar_Short()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            short value = 12;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToChar(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToChar_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            object value = null;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToChar(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToChar_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            string value = "A";

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToChar(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000)]
        [InlineData("Fri, 27 Feb 2009 03:11:21 GMT")]
        [InlineData("Thursday, February 26, 2009")]
        [InlineData("February 26, 2009")]
        [InlineData("12/31/1999 11:59:59 PM")]
        [InlineData("12/31/1999")]
        public void ToDateTime_String(string value)
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDateTime(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToDateTime_DateTime()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            var value = new DateTime(1999, 12, 31, 23, 59, 59);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDateTime(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToDecimal_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToDecimal_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToDecimal_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 5000000)]
        public void ToDecimal_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToDecimal_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 5000000)]
        public void ToDecimal_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 1000000)]
        public void ToDecimal_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToDecimal_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDecimal(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToDouble_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToDouble_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToDouble_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToDouble_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToDouble_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToDouble_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 1000000)]
        public void ToDouble_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToDouble_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToDouble(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt16_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt16_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToInt16_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToInt16_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToInt16_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToInt16_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToInt16_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToInt16_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt16(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt32_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt32_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToInt32_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToInt32_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt32_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToInt32_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToInt32_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToInt32_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt32(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt64_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToInt64_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToInt64_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToInt64_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToInt64_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToInt64_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToInt64_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToInt64_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToInt64(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt16_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt16_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToUInt16_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToUInt16_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToUInt16_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToUInt16_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToUInt16_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToUInt16_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt16(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt32_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt32_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToUInt32_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToUInt32_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt32_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToUInt32_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToUInt32_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToUInt32_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt32(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt64_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToUInt64_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToUInt64_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(145m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToUInt64_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(145d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToUInt64_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(145);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToUInt64_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(145.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToUInt64_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64("145");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToUInt64_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToUInt64(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToSByte_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToSByte_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToSByte_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(100m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToSByte_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(100d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToSByte_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(100);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 50000000)]
        public void ToSByte_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(50.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToSByte_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte("100");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToSByte_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSByte(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToSingle_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToSingle_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 10000000)]
        public void ToSingle_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(100m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToSingle_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(100d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToSingle_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(100);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 400000000)]
        public void ToSingle_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(50.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToSingle_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle("100");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToSingle_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToSingle(null);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100000000)]
        public void ToString_Boolean()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(true);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToString_Byte()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            byte value = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(value);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 2000000)]
        public void ToString_Decimal()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(100m);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 1000000)]
        public void ToString_Double()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(100d);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 3000000)]
        public void ToString_Int()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(100);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 700000)]
        public void ToString_Single()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(50.1f);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 500000000)]
        public void ToString_String()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString("Hello world");
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 7000000)]
        public void ToString_Object()
        {
            int innerIterationCount = (int)Benchmark.InnerIterationCount;
            Perf_Convert value = new Perf_Convert();

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterationCount; i++)
                    {
                        Convert.ToString(value);
                    }
                }
            }
        }
    }
}
