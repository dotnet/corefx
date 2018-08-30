// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Text.Tests
{
    public static partial class Utf8ParserTests
    {
        private static readonly string[] s_Int16TextArray = new string[13]
        {
            "21474",
            "2",
            "-21474",
            "31484",
            "-21",
            "-2",
            "214",
            "2147",
            "-2147",
            "-9345",
            "9345",
            "1000",
            "-214"
        };

        private static readonly string[] s_Int32TextArray = new string[20]
        {
            "214748364",
            "2",
            "21474836",
            "-21474",
            "21474",
            "-21",
            "-2",
            "214",
            "-21474836",
            "-214748364",
            "2147",
            "-2147",
            "-214748",
            "-2147483",
            "214748",
            "-2147483648",
            "2147483647",
            "21",
            "2147483",
            "-214"
        };

        [Benchmark]
        [InlineData("107374182")] // standard parse
        [InlineData("2147483647")] // max value
        [InlineData("0")]
        [InlineData("-2147483648")] // min value
        [InlineData("214748364")]
        [InlineData("2")]
        [InlineData("21474836")]
        [InlineData("-21474")]
        [InlineData("21474")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("214")]
        [InlineData("-21474836")]
        [InlineData("-214748364")]
        [InlineData("2147")]
        [InlineData("-2147")]
        [InlineData("-214748")]
        [InlineData("-2147483")]
        [InlineData("214748")]
        [InlineData("21")]
        [InlineData("2147483")]
        [InlineData("-214")]
        [InlineData("+21474")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("+21474836")]
        [InlineData("+214748364")]
        [InlineData("+2147")]
        [InlineData("+214748")]
        [InlineData("+2147483")]
        [InlineData("+2147483647")]
        [InlineData("+214")]
        [InlineData("000000000000000000001235abcdfg")]
        [InlineData("214748364abcdefghijklmnop")]
        [InlineData("2abcdefghijklmnop")]
        [InlineData("21474836abcdefghijklmnop")]
        [InlineData("-21474abcdefghijklmnop")]
        [InlineData("21474abcdefghijklmnop")]
        [InlineData("-21abcdefghijklmnop")]
        [InlineData("-2abcdefghijklmnop")]
        [InlineData("214abcdefghijklmnop")]
        [InlineData("-21474836abcdefghijklmnop")]
        [InlineData("-214748364abcdefghijklmnop")]
        [InlineData("2147abcdefghijklmnop")]
        [InlineData("-2147abcdefghijklmnop")]
        [InlineData("-214748abcdefghijklmnop")]
        [InlineData("-2147483abcdefghijklmnop")]
        [InlineData("214748abcdefghijklmnop")]
        [InlineData("21abcdefghijklmnop")]
        [InlineData("2147483abcdefghijklmnop")]
        [InlineData("-214abcdefghijklmnop")]
        [InlineData("+21474abcdefghijklmnop")]
        [InlineData("+21abcdefghijklmnop")]
        [InlineData("+2abcdefghijklmnop")]
        [InlineData("+21474836abcdefghijklmnop")]
        [InlineData("+214748364abcdefghijklmnop")]
        [InlineData("+2147abcdefghijklmnop")]
        [InlineData("+214748abcdefghijklmnop")]
        [InlineData("+2147483abcdefghijklmnop")]
        [InlineData("+2147483647abcdefghijklmnop")]
        [InlineData("+214abcdefghijklmnop")]
        private static void PrimitiveParserByteSpanToInt32_BytesConsumed(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out int value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        private static void PrimitiveParserByteSpanToInt32_BytesConsumed_VariableLength()
        {
            int textLength = s_Int32TextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_Int32TextArray[i]);
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray[i % textLength];
                        Utf8Parser.TryParse(utf8ByteSpan, out int value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("107374182")] // standard parse
        [InlineData("2147483647")] // max value
        [InlineData("0")]
        [InlineData("-2147483648")] // min value
        [InlineData("214748364")]
        [InlineData("2")]
        [InlineData("21474836")]
        [InlineData("-21474")]
        [InlineData("21474")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("214")]
        [InlineData("-21474836")]
        [InlineData("-214748364")]
        [InlineData("2147")]
        [InlineData("-2147")]
        [InlineData("-214748")]
        [InlineData("-2147483")]
        [InlineData("214748")]
        [InlineData("21")]
        [InlineData("2147483")]
        [InlineData("-214")]
        [InlineData("+21474")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("+21474836")]
        [InlineData("+214748364")]
        [InlineData("+2147")]
        [InlineData("+214748")]
        [InlineData("+2147483")]
        [InlineData("+2147483647")]
        [InlineData("+214")]
        [InlineData("000000000000000000001235abcdfg")]
        [InlineData("214748364abcdefghijklmnop")]
        [InlineData("2abcdefghijklmnop")]
        [InlineData("21474836abcdefghijklmnop")]
        [InlineData("-21474abcdefghijklmnop")]
        [InlineData("21474abcdefghijklmnop")]
        [InlineData("-21abcdefghijklmnop")]
        [InlineData("-2abcdefghijklmnop")]
        [InlineData("214abcdefghijklmnop")]
        [InlineData("-21474836abcdefghijklmnop")]
        [InlineData("-214748364abcdefghijklmnop")]
        [InlineData("2147abcdefghijklmnop")]
        [InlineData("-2147abcdefghijklmnop")]
        [InlineData("-214748abcdefghijklmnop")]
        [InlineData("-2147483abcdefghijklmnop")]
        [InlineData("214748abcdefghijklmnop")]
        [InlineData("21abcdefghijklmnop")]
        [InlineData("2147483abcdefghijklmnop")]
        [InlineData("-214abcdefghijklmnop")]
        [InlineData("+21474abcdefghijklmnop")]
        [InlineData("+21abcdefghijklmnop")]
        [InlineData("+2abcdefghijklmnop")]
        [InlineData("+21474836abcdefghijklmnop")]
        [InlineData("+214748364abcdefghijklmnop")]
        [InlineData("+2147abcdefghijklmnop")]
        [InlineData("+214748abcdefghijklmnop")]
        [InlineData("+2147483abcdefghijklmnop")]
        [InlineData("+2147483647abcdefghijklmnop")]
        [InlineData("+214abcdefghijklmnop")]
        private static void PrimitiveParserByteSpanToInt32_BytesConsumed_Baseline(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        int.TryParse(text, out int value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        private static void PrimitiveParserByteSpanToInt32_BytesConsumed_VariableLength_Baseline()
        {
            int textLength = s_Int32TextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_Int32TextArray[i]);
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        int.TryParse(s_Int32TextArray[i % textLength], out int value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("10737")] // standard parse
        [InlineData("32767")] // max value
        [InlineData("0")]
        [InlineData("-32768")] // min value
        [InlineData("2147")]
        [InlineData("2")]
        [InlineData("-21474")]
        [InlineData("21474")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("214")]
        [InlineData("2147")]
        [InlineData("-2147")]
        [InlineData("-48")]
        [InlineData("48")]
        [InlineData("483")]
        [InlineData("21")]
        [InlineData("-214")]
        [InlineData("+21474")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("+214")]
        [InlineData("+2147")]
        [InlineData("+21475")]
        [InlineData("+48")]
        [InlineData("+483")]
        [InlineData("+21437")]
        [InlineData("000000000000000000001235abcdfg")]
        [InlineData("2147abcdefghijklmnop")]
        [InlineData("2abcdefghijklmnop")]
        [InlineData("214abcdefghijklmnop")]
        [InlineData("-2147abcdefghijklmnop")]
        [InlineData("21474abcdefghijklmnop")]
        [InlineData("-21abcdefghijklmnop")]
        [InlineData("-2abcdefghijklmnop")]
        [InlineData("487abcdefghijklmnop")]
        [InlineData("-483abcdefghijklmnop")]
        [InlineData("-4836abcdefghijklmnop")]
        [InlineData("21abcdefghijklmnop")]
        [InlineData("+000000000000000000001235abcdfg")]
        [InlineData("+2147abcdefghijklmnop")]
        [InlineData("+214abcdefghijklmnop")]
        [InlineData("+21474abcdefghijklmnop")]
        [InlineData("+2abcdefghijklmnop")]
        [InlineData("+487abcdefghijklmnop")]
        [InlineData("+483abcdefghijklmnop")]
        [InlineData("+4836abcdefghijklmnop")]
        [InlineData("+21abcdefghijklmnop")]
        private static void PrimitiveParserByteSpanToInt16(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out short value, out int consumed);
                        TestHelpers.DoNotIgnore(value, consumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("10737")] // standard parse
        [InlineData("32767")] // max value
        [InlineData("0")]
        [InlineData("-32768")] // min value
        [InlineData("2147")]
        [InlineData("2")]
        [InlineData("-21474")]
        [InlineData("21474")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("214")]
        [InlineData("2147")]
        [InlineData("-2147")]
        [InlineData("-48")]
        [InlineData("48")]
        [InlineData("483")]
        [InlineData("21")]
        [InlineData("-214")]
        [InlineData("+21474")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("+214")]
        [InlineData("+2147")]
        [InlineData("+21475")]
        [InlineData("+48")]
        [InlineData("+483")]
        [InlineData("+21437")]
        [InlineData("000000000000000000001235abcdfg")]
        [InlineData("2147abcdefghijklmnop")]
        [InlineData("2abcdefghijklmnop")]
        [InlineData("214abcdefghijklmnop")]
        [InlineData("-2147abcdefghijklmnop")]
        [InlineData("21474abcdefghijklmnop")]
        [InlineData("-21abcdefghijklmnop")]
        [InlineData("-2abcdefghijklmnop")]
        [InlineData("487abcdefghijklmnop")]
        [InlineData("-483abcdefghijklmnop")]
        [InlineData("-4836abcdefghijklmnop")]
        [InlineData("21abcdefghijklmnop")]
        [InlineData("+000000000000000000001235abcdfg")]
        [InlineData("+2147abcdefghijklmnop")]
        [InlineData("+214abcdefghijklmnop")]
        [InlineData("+21474abcdefghijklmnop")]
        [InlineData("+2abcdefghijklmnop")]
        [InlineData("+487abcdefghijklmnop")]
        [InlineData("+483abcdefghijklmnop")]
        [InlineData("+4836abcdefghijklmnop")]
        [InlineData("+21abcdefghijklmnop")]
        private static void PrimitiveParserByteSpanToInt16_BytesConsumed(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out short value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        private static void PrimitiveParserByteSpanToInt16_BytesConsumed_VariableLength()
        {
            int textLength = s_Int16TextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_Int16TextArray[i]);
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray[i % textLength];
                        Utf8Parser.TryParse(utf8ByteSpan, out short value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("10737")] // standard parse
        [InlineData("32767")] // max value
        [InlineData("0")]
        [InlineData("-32768")] // min value
        [InlineData("2147")]
        [InlineData("2")]
        [InlineData("-21474")]
        [InlineData("21474")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("214")]
        [InlineData("2147")]
        [InlineData("-2147")]
        [InlineData("-48")]
        [InlineData("48")]
        [InlineData("483")]
        [InlineData("21")]
        [InlineData("-214")]
        [InlineData("+21474")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("+214")]
        [InlineData("+2147")]
        [InlineData("+21475")]
        [InlineData("+48")]
        [InlineData("+483")]
        [InlineData("+21437")]
        [InlineData("000000000000000000001235")]
        private static void PrimitiveParserByteSpanToInt16_BytesConsumed_Baseline(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        short.TryParse(text, out short value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        private static void PrimitiveParserByteSpanToInt16_BytesConsumed_VariableLength_Baseline()
        {
            int textLength = s_Int16TextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_Int16TextArray[i]);
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        short.TryParse(s_Int16TextArray[i % textLength], out short value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("True")]
        [InlineData("False")]
        private static void BaselineStringToBool(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        bool.TryParse(text, out bool value);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("True")]
        [InlineData("False")]
        private static void PrimitiveParserByteSpanToBool(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out bool value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("42")] // standard parse
        [InlineData("-128")] // min value
        [InlineData("127")] // max value
        private static void ParserSByte(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out sbyte value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("42")] // standard parse
        [InlineData("0")] // min value
        [InlineData("255")] // max value
        private static void ParserByte(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out byte value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("4212")] // standard parse
        [InlineData("-32768")] // min value
        [InlineData("32767")] // max value
        private static void ParserInt16(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out short value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("4212")] // standard parse
        [InlineData("0")] // min value
        [InlineData("65535")] // max value
        private static void ParserUInt16(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out ushort value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("12837467")] // standard parse
        [InlineData("-2147483648")] // min value
        [InlineData("2147483647")] // max value
        private static void ParserInt32(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out int value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("12837467")] // standard parse
        [InlineData("0")] // min value
        [InlineData("4294967295")] // max value
        private static void ParserUInt32(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out uint value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("12837467")] // standard parse
        [InlineData("-9223372036854775808")] // min value
        [InlineData("9223372036854775807")] // max value
        private static void ParserInt64(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out long value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("12837467")] // standard parse
        [InlineData("0")] // min value
        [InlineData("18446744073709551615")] // max value
        private static void ParserUInt64(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out ulong value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("Fri, 30 Jun 2000 03:15:45 GMT")] // standard parse
        private static void ParserDateTimeOffsetR(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out DateTimeOffset value, out int bytesConsumed, 'R');
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }
    }
}
