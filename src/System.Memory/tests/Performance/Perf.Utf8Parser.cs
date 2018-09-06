// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Text;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Text.Tests
{
    public static partial class Utf8ParserTests
    {
        private static readonly string[] s_UInt32TextArray = new string[10]
        {
            "42",
            "429496",
            "429496729",
            "42949",
            "4",
            "42949672",
            "4294",
            "429",
            "4294967295",
            "4294967"
        };

        private static readonly string[] s_UInt32TextArrayHex = new string[8]
        {
            "A2",
            "A29496",
            "A2949",
            "A",
            "A2949672",
            "A294",
            "A29",
            "A294967"
        };

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

        private static readonly string[] s_SByteTextArray = new string[17]
        {
            "95",
            "2",
            "112",
            "-112",
            "-21",
            "-2",
            "114",
            "-114",
            "-124",
            "117",
            "-117",
            "-14",
            "14",
            "74",
            "21",
            "83",
            "-127"
        };

        [Benchmark]
        [InlineData("2134567890")] // standard parse
        [InlineData("18446744073709551615")] // max value
        [InlineData("0")] // min value
        private static void StringToUInt64(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ulong.TryParse(text, out ulong value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("2134567890")] // standard parse
        [InlineData("18446744073709551615")] // max value
        [InlineData("0")] // min value
        private static void BaselineByteStarToUInt64(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ulong.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out ulong value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("abcdef")] // standard parse
        [InlineData("ffffffffffffffff")] // max value
        [InlineData("0")] // min value
        private static void BaselineByteStarToUInt64Hex(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ulong.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("2134567890")] // standard parse
        [InlineData("18446744073709551615")] // max value
        [InlineData("0")] // min value
        private static unsafe void ByteSpanToUInt64_BytesConsumed(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);
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
        [InlineData("abcdef")] // standard parse
        [InlineData("ffffffffffffffff")] // max value
        [InlineData("0")] // min value
        private static unsafe void ByteSpanToUInt64Hex_BytesConsumed(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            ReadOnlySpan<byte> utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out ulong value, out int bytesConsumed, 'X');
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("2134567890")] // standard parse
        [InlineData("4294967295")] // max value
        [InlineData("0")] // min value
        private static void BaselineSimpleByteStarToUInt32(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        uint.TryParse(text, out uint value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        private static void SimpleByteStarToUInt32_VariableLength()
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        uint.TryParse(s_UInt32TextArray[i % 10], out uint value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("2134567890")] // standard parse
        [InlineData("4294967295")] // max value
        [InlineData("0")] // min value
        private static void BaselineByteStarToUInt32(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        uint.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out uint value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        private static void BaselineByteStarToUInt32_VariableLength()
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        uint.TryParse(s_UInt32TextArray[i % 10], NumberStyles.None, CultureInfo.InvariantCulture, out uint value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("abcdef")] // standard parse
        [InlineData("ffffffff")] // max value
        [InlineData("0")] // min value
        private static void BaselineByteStarToUInt32Hex(string text)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        private static void BaselineByteStarToUInt32Hex_VariableLength()
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        uint.TryParse(s_UInt32TextArrayHex[i % 8], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("2134567890")] // standard parse
        [InlineData("4294967295")] // max value
        [InlineData("0")] // min value
        private static unsafe void ByteSpanToUInt32_BytesConsumed(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);
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
        private static unsafe void ByteSpanToUInt32_BytesConsumed_VariableLength()
        {
            int textLength = s_UInt32TextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_UInt32TextArray[i]);
            }
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray[i % textLength];
                        Utf8Parser.TryParse(utf8ByteSpan, out uint value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData("abcdef")] // standard parse
        [InlineData("ffffffff")] // max value
        [InlineData("0")] // min value
        private static unsafe void ByteSpanToUInt32Hex_BytesConsumed(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Parser.TryParse(utf8ByteSpan, out uint value, out int bytesConsumed, 'X');
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        private static unsafe void ByteSpanToUInt32Hex_BytesConsumed_VariableLength()
        {
            int textLength = s_UInt32TextArrayHex.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_UInt32TextArrayHex[i]);
            }
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray[i % textLength];
                        Utf8Parser.TryParse(utf8ByteSpan, out uint value, out int bytesConsumed, 'X');
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        private static void ByteSpanToSByte_BytesConsumed_VariableLength()
        {
            int textLength = s_SByteTextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_SByteTextArray[i]);
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlySpan<byte> utf8ByteSpan = utf8ByteArray[i % textLength];
                        Utf8Parser.TryParse(utf8ByteSpan, out sbyte value, out int bytesConsumed);
                        TestHelpers.DoNotIgnore(value, bytesConsumed);
                    }
                }
            }
        }

        [Benchmark]
        #region Inline data
        [InlineData("107")] // standard parse
        [InlineData("127")] // max value
        [InlineData("0")]
        [InlineData("-128")] // min value
        [InlineData("147")]
        [InlineData("2")]
        [InlineData("105")]
        [InlineData("-111")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("-13")]
        [InlineData("-8")]
        [InlineData("-83")]
        [InlineData("+127")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("00000000000000000000123")]
        #endregion
        private static void ByteSpanToSByte_BytesConsumed_Baseline(string text)
        {
            byte[] utf8ByteArray = Encoding.UTF8.GetBytes(text);
            var utf8ByteSpan = new ReadOnlySpan<byte>(utf8ByteArray);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        sbyte.TryParse(text, out sbyte value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        private static void ByteSpanToSByte_BytesConsumed_VariableLength_Baseline()
        {
            int textLength = s_SByteTextArray.Length;
            byte[][] utf8ByteArray = (byte[][])Array.CreateInstance(typeof(byte[]), textLength);
            for (var i = 0; i < textLength; i++)
            {
                utf8ByteArray[i] = Encoding.UTF8.GetBytes(s_SByteTextArray[i]);
            }
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        sbyte.TryParse(s_SByteTextArray[i % textLength], out sbyte value);
                        TestHelpers.DoNotIgnore(value, 0);
                    }
                }
            }
        }

        [Benchmark]
        #region Inline data
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
        #endregion
        private static void ByteSpanToInt32_BytesConsumed(string text)
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
        private static void ByteSpanToInt32_BytesConsumed_VariableLength()
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
        #region Inline data
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
        #endregion
        private static void ByteSpanToInt32_BytesConsumed_Baseline(string text)
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
        private static void ByteSpanToInt32_BytesConsumed_VariableLength_Baseline()
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
        #region Inline data
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
        #endregion
        private static void ByteSpanToInt16(string text)
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
        #region Inline data
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
        #endregion
        private static void ByteSpanToInt16_BytesConsumed(string text)
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
        private static void ByteSpanToInt16_BytesConsumed_VariableLength()
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
        #region Inline data
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
        #endregion
        private static void ByteSpanToInt16_BytesConsumed_Baseline(string text)
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
        private static void ByteSpanToInt16_BytesConsumed_VariableLength_Baseline()
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
        private static void ByteSpanToBool(string text)
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
        [InlineData("107")] // standard parse
        [InlineData("127")] // max value
        [InlineData("0")]
        [InlineData("-128")] // min value
        [InlineData("147")]
        [InlineData("2")]
        [InlineData("105")]
        [InlineData("-111")]
        [InlineData("-21")]
        [InlineData("-2")]
        [InlineData("-13")]
        [InlineData("-8")]
        [InlineData("-83")]
        [InlineData("+127")]
        [InlineData("+21")]
        [InlineData("+2")]
        [InlineData("00000000000000000000123abcdfg")]
        [InlineData("2abcdefghijklmnop")]
        [InlineData("14abcdefghijklmnop")]
        [InlineData("-14abcdefghijklmnop")]
        [InlineData("-21abcdefghijklmnop")]
        [InlineData("-2abcdefghijklmnop")]
        [InlineData("+14abcdefghijklmnop")]
        [InlineData("+21abcdefghijklmnop")]
        [InlineData("+2abcdefghijklmnop")]
        [InlineData("+111abcdefghijklmnop")]
        [InlineData("+000000000000000000123abcdfg")]
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
