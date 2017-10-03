// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;
using System.Net;

namespace System.Buffers.Binary.Tests
{
    public class BinaryReadAndWriteTests
    {
        private const int InnerCount = 100000;

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void ReadStructAndReverseBE()
        {
            Span<byte> spanBE = TestHelpers.GetSpanBE();

            var readStruct = new TestHelpers.TestStructExplicit();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        readStruct = spanBE.Read<TestHelpers.TestStructExplicit>();
                        if (BitConverter.IsLittleEndian)
                        {
                            readStruct.S0 = readStruct.S0.Reverse();
                            readStruct.I0 = readStruct.I0.Reverse();
                            readStruct.L0 = readStruct.L0.Reverse();
                            readStruct.US0 = readStruct.US0.Reverse();
                            readStruct.UI0 = readStruct.UI0.Reverse();
                            readStruct.UL0 = readStruct.UL0.Reverse();
                            readStruct.S1 = readStruct.S1.Reverse();
                            readStruct.I1 = readStruct.I1.Reverse();
                            readStruct.L1 = readStruct.L1.Reverse();
                            readStruct.US1 = readStruct.US1.Reverse();
                            readStruct.UI1 = readStruct.UI1.Reverse();
                            readStruct.UL1 = readStruct.UL1.Reverse();
                        }
                    }
                }
            }

            Assert.Equal(TestHelpers.testExplicitStruct, readStruct);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void ReadStructAndReverseLE()
        {
            Span<byte> spanLE = TestHelpers.GetSpanLE();

            var readStruct = new TestHelpers.TestStructExplicit();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        readStruct = spanLE.Read<TestHelpers.TestStructExplicit>();
                        if (!BitConverter.IsLittleEndian)
                        {
                            readStruct.S0 = readStruct.S0.Reverse();
                            readStruct.I0 = readStruct.I0.Reverse();
                            readStruct.L0 = readStruct.L0.Reverse();
                            readStruct.US0 = readStruct.US0.Reverse();
                            readStruct.UI0 = readStruct.UI0.Reverse();
                            readStruct.UL0 = readStruct.UL0.Reverse();
                            readStruct.S1 = readStruct.S1.Reverse();
                            readStruct.I1 = readStruct.I1.Reverse();
                            readStruct.L1 = readStruct.L1.Reverse();
                            readStruct.US1 = readStruct.US1.Reverse();
                            readStruct.UI1 = readStruct.UI1.Reverse();
                            readStruct.UL1 = readStruct.UL1.Reverse();
                        }
                    }
                }
            }

            Assert.Equal(TestHelpers.testExplicitStruct, readStruct);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void ReadStructFieldByFieldBE()
        {
            Span<byte> spanBE = TestHelpers.GetSpanBE();

            var readStruct = new TestHelpers.TestStructExplicit();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        readStruct = new TestHelpers.TestStructExplicit
                        {
                            S0 = spanBE.ReadInt16BigEndian(),
                            I0 = spanBE.Slice(2).ReadInt32BigEndian(),
                            L0 = spanBE.Slice(6).ReadInt64BigEndian(),
                            US0 = spanBE.Slice(14).ReadUInt16BigEndian(),
                            UI0 = spanBE.Slice(16).ReadUInt32BigEndian(),
                            UL0 = spanBE.Slice(20).ReadUInt64BigEndian(),
                            S1 = spanBE.Slice(28).ReadInt16BigEndian(),
                            I1 = spanBE.Slice(30).ReadInt32BigEndian(),
                            L1 = spanBE.Slice(34).ReadInt64BigEndian(),
                            US1 = spanBE.Slice(42).ReadUInt16BigEndian(),
                            UI1 = spanBE.Slice(44).ReadUInt32BigEndian(),
                            UL1 = spanBE.Slice(48).ReadUInt64BigEndian()
                        };
                    }
                }
            }

            Assert.Equal(TestHelpers.testExplicitStruct, readStruct);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void ReadStructFieldByFieldLE()
        {
            Span<byte> spanLE = TestHelpers.GetSpanLE();

            var readStruct = new TestHelpers.TestStructExplicit();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        readStruct = new TestHelpers.TestStructExplicit
                        {
                            S0 = spanLE.ReadInt16LittleEndian(),
                            I0 = spanLE.Slice(2).ReadInt32LittleEndian(),
                            L0 = spanLE.Slice(6).ReadInt64LittleEndian(),
                            US0 = spanLE.Slice(14).ReadUInt16LittleEndian(),
                            UI0 = spanLE.Slice(16).ReadUInt32LittleEndian(),
                            UL0 = spanLE.Slice(20).ReadUInt64LittleEndian(),
                            S1 = spanLE.Slice(28).ReadInt16LittleEndian(),
                            I1 = spanLE.Slice(30).ReadInt32LittleEndian(),
                            L1 = spanLE.Slice(34).ReadInt64LittleEndian(),
                            US1 = spanLE.Slice(42).ReadUInt16LittleEndian(),
                            UI1 = spanLE.Slice(44).ReadUInt32LittleEndian(),
                            UL1 = spanLE.Slice(48).ReadUInt64LittleEndian()
                        };
                    }
                }
            }

            Assert.Equal(TestHelpers.testExplicitStruct, readStruct);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void ReadStructFieldByFieldUsingBitConverterLE()
        {
            Span<byte> spanLE = TestHelpers.GetSpanLE();
            byte[] arrayLE = spanLE.ToArray();

            var readStruct = new TestHelpers.TestStructExplicit();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        readStruct = new TestHelpers.TestStructExplicit
                        {
                            S0 = BitConverter.ToInt16(arrayLE, 0),
                            I0 = BitConverter.ToInt32(arrayLE, 2),
                            L0 = BitConverter.ToInt64(arrayLE, 6),
                            US0 = BitConverter.ToUInt16(arrayLE, 14),
                            UI0 = BitConverter.ToUInt32(arrayLE, 16),
                            UL0 = BitConverter.ToUInt64(arrayLE, 20),
                            S1 = BitConverter.ToInt16(arrayLE, 28),
                            I1 = BitConverter.ToInt32(arrayLE, 30),
                            L1 = BitConverter.ToInt64(arrayLE, 34),
                            US1 = BitConverter.ToUInt16(arrayLE, 42),
                            UI1 = BitConverter.ToUInt32(arrayLE, 44),
                            UL1 = BitConverter.ToUInt64(arrayLE, 48),
                        };
                    }
                }
            }

            Assert.Equal(TestHelpers.testExplicitStruct, readStruct);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void ReadStructFieldByFieldUsingBitConverterBE()
        {
            Span<byte> spanBE = TestHelpers.GetSpanBE();
            byte[] arrayBE = spanBE.ToArray();

            var readStruct = new TestHelpers.TestStructExplicit();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        readStruct = new TestHelpers.TestStructExplicit
                        {
                            S0 = BitConverter.ToInt16(arrayBE, 0),
                            I0 = BitConverter.ToInt32(arrayBE, 2),
                            L0 = BitConverter.ToInt64(arrayBE, 6),
                            US0 = BitConverter.ToUInt16(arrayBE, 14),
                            UI0 = BitConverter.ToUInt32(arrayBE, 16),
                            UL0 = BitConverter.ToUInt64(arrayBE, 20),
                            S1 = BitConverter.ToInt16(arrayBE, 28),
                            I1 = BitConverter.ToInt32(arrayBE, 30),
                            L1 = BitConverter.ToInt64(arrayBE, 34),
                            US1 = BitConverter.ToUInt16(arrayBE, 42),
                            UI1 = BitConverter.ToUInt32(arrayBE, 44),
                            UL1 = BitConverter.ToUInt64(arrayBE, 48),
                        };
                        if (BitConverter.IsLittleEndian)
                        {
                            readStruct.S0 = readStruct.S0.Reverse();
                            readStruct.I0 = readStruct.I0.Reverse();
                            readStruct.L0 = readStruct.L0.Reverse();
                            readStruct.US0 = readStruct.US0.Reverse();
                            readStruct.UI0 = readStruct.UI0.Reverse();
                            readStruct.UL0 = readStruct.UL0.Reverse();
                            readStruct.S1 = readStruct.S1.Reverse();
                            readStruct.I1 = readStruct.I1.Reverse();
                            readStruct.L1 = readStruct.L1.Reverse();
                            readStruct.US1 = readStruct.US1.Reverse();
                            readStruct.UI1 = readStruct.UI1.Reverse();
                            readStruct.UL1 = readStruct.UL1.Reverse();
                        }
                    }
                }
            }

            Assert.Equal(TestHelpers.testExplicitStruct, readStruct);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void MeasureReverse()
        {
            var myArray = new int[1000];
            
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < myArray.Length; j++)
                        {
                            myArray[j] = myArray[j].Reverse();
                        }
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void MeasureReverseUsingNtoH()
        {
            var myArray = new int[1000];

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < myArray.Length; j++)
                        {
                            myArray[j] = IPAddress.NetworkToHostOrder(myArray[j]);
                        }
                    }
                }
            }
        }
    }
}
