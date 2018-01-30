// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    public static partial class TestHelpers
    {
        public static System.TestHelpers.TestStructExplicit s_testExplicitStruct;
        public static System.Collections.Generic.IEnumerable<object[]> StringSlice2ArgTestOutOfRangeData { get { throw null; } }
        public static System.Collections.Generic.IEnumerable<object[]> StringSlice3ArgTestOutOfRangeData { get { throw null; } }
        public static System.Collections.Generic.IEnumerable<object[]> StringSliceTestData { get { throw null; } }
        public static void AssertThrows<E, T>(System.ReadOnlySpan<T> span, System.TestHelpers.AssertThrowsActionReadOnly<T> action) where E : System.Exception { }
        public static void AssertThrows<E, T>(System.Span<T> span, System.TestHelpers.AssertThrowsAction<T> action) where E : System.Exception { }
        public static void DoNotIgnore<T>(T value, int consumed) { }
        public static void DontBox<T>(this System.ReadOnlySpan<T> span) { }
        public static void DontBox<T>(this System.Span<T> span) { }
        public static System.Span<byte> GetSpanBE() { throw null; }
        public static System.Span<byte> GetSpanLE() { throw null; }
        public static void ValidateNonNullEmpty<T>(this System.ReadOnlySpan<T> span) { }
        public static void ValidateNonNullEmpty<T>(this System.Span<T> span) { }
        public static void ValidateReferenceType<T>(this System.Memory<T> memory, params T[] expected) { }
        public static void ValidateReferenceType<T>(this System.ReadOnlyMemory<T> memory, params T[] expected) { }
        public static void ValidateReferenceType<T>(this System.ReadOnlySpan<T> span, params T[] expected) { }
        public static void ValidateReferenceType<T>(this System.Span<T> span, params T[] expected) { }
        public static void Validate<T>(this System.Memory<T> memory, params T[] expected) where T : struct, System.IEquatable<T> { }
        public static void Validate<T>(this System.ReadOnlyMemory<T> memory, params T[] expected) where T : struct, System.IEquatable<T> { }
        public static void Validate<T>(this System.ReadOnlySpan<T> span, params T[] expected) where T : struct, System.IEquatable<T> { }
        public static void Validate<T>(System.Span<byte> span, T value) where T : struct { }
        public static void Validate<T>(this System.Span<T> span, params T[] expected) where T : struct, System.IEquatable<T> { }
        public delegate void AssertThrowsActionReadOnly<T>(System.ReadOnlySpan<T> span);
        public delegate void AssertThrowsAction<T>(System.Span<T> span);
        public sealed partial class TestClass
        {
            public char C0;
            public char C1;
            public char C2;
            public char C3;
            public char C4;
            public TestClass() { }
        }
        public enum TestEnum
        {
            e0 = 0,
            e1 = 1,
            e2 = 2,
            e3 = 3,
            e4 = 4,
        }
        public partial struct TestStructExplicit
        {
            [System.Runtime.InteropServices.FieldOffsetAttribute(2)]
            public int I0;
            [System.Runtime.InteropServices.FieldOffsetAttribute(30)]
            public int I1;
            [System.Runtime.InteropServices.FieldOffsetAttribute(6)]
            public long L0;
            [System.Runtime.InteropServices.FieldOffsetAttribute(34)]
            public long L1;
            [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public short S0;
            [System.Runtime.InteropServices.FieldOffsetAttribute(28)]
            public short S1;
            [System.Runtime.InteropServices.FieldOffsetAttribute(16)]
            public uint UI0;
            [System.Runtime.InteropServices.FieldOffsetAttribute(44)]
            public uint UI1;
            [System.Runtime.InteropServices.FieldOffsetAttribute(20)]
            public ulong UL0;
            [System.Runtime.InteropServices.FieldOffsetAttribute(48)]
            public ulong UL1;
            [System.Runtime.InteropServices.FieldOffsetAttribute(14)]
            public ushort US0;
            [System.Runtime.InteropServices.FieldOffsetAttribute(42)]
            public ushort US1;
        }
        public partial struct TestValueTypeWithReference
        {
            public int I;
            public string S;
        }
    }
}
namespace System.Buffers.Binary.Tests
{
    public partial class BinaryReaderUnitTests
    {
        public BinaryReaderUnitTests() { }
        [Xunit.FactAttribute]
        public void ReadingStructFieldByFieldOrReadAndReverseEndianness() { }
        [Xunit.FactAttribute]
        public void ReadOnlySpanRead() { }
        [Xunit.FactAttribute]
        public void ReadOnlySpanReadFail() { }
        [Xunit.FactAttribute]
        public void ReverseByteDoesNothing() { }
        [Xunit.FactAttribute]
        public void SpanRead() { }
        [Xunit.FactAttribute]
        public void SpanReadFail() { }
        [Xunit.FactAttribute]
        public void SpanWriteAndReadBigEndianHeterogeneousStruct() { }
        [Xunit.FactAttribute]
        public void SpanWriteAndReadLittleEndianHeterogeneousStruct() { }
    }
    public partial class BinaryWriterUnitTests
    {
        public BinaryWriterUnitTests() { }
        [Xunit.FactAttribute]
        public void SpanWrite() { }
        [Xunit.FactAttribute]
        public void SpanWriteFail() { }
        [Xunit.InlineDataAttribute(new object[]{ (short)32767})]
        [Xunit.InlineDataAttribute(new object[]{ (short)-32768})]
        [Xunit.InlineDataAttribute(new object[]{ 255})]
        [Xunit.InlineDataAttribute(new object[]{ 32512})]
        [Xunit.TheoryAttribute]
        public void SpanWriteInt16(short value) { }
        [Xunit.InlineDataAttribute(new object[]{ 16711680})]
        [Xunit.InlineDataAttribute(new object[]{ 2130706432})]
        [Xunit.InlineDataAttribute(new object[]{ 2147483647})]
        [Xunit.InlineDataAttribute(new object[]{ -2147483648})]
        [Xunit.InlineDataAttribute(new object[]{ 255})]
        [Xunit.InlineDataAttribute(new object[]{ 65280})]
        [Xunit.TheoryAttribute]
        public void SpanWriteInt32(int value) { }
        [Xunit.InlineDataAttribute(new object[]{ (long)1095216660480})]
        [Xunit.InlineDataAttribute(new object[]{ (long)280375465082880})]
        [Xunit.InlineDataAttribute(new object[]{ (long)71776119061217280})]
        [Xunit.InlineDataAttribute(new object[]{ (long)9151314442816847872})]
        [Xunit.InlineDataAttribute(new object[]{ (long)9223372036854775807})]
        [Xunit.InlineDataAttribute(new object[]{ (long)-9223372036854775808})]
        [Xunit.InlineDataAttribute(new object[]{ (uint)4278190080})]
        [Xunit.InlineDataAttribute(new object[]{ 16711680})]
        [Xunit.InlineDataAttribute(new object[]{ 255})]
        [Xunit.InlineDataAttribute(new object[]{ 65280})]
        [Xunit.TheoryAttribute]
        public void SpanWriteInt64(long value) { }
        [Xunit.InlineDataAttribute(new object[]{ (ushort)0})]
        [Xunit.InlineDataAttribute(new object[]{ (ushort)65535})]
        [Xunit.InlineDataAttribute(new object[]{ 255})]
        [Xunit.InlineDataAttribute(new object[]{ 65280})]
        [Xunit.TheoryAttribute]
        public void SpanWriteUInt16(ushort value) { }
        [Xunit.InlineDataAttribute(new object[]{ (uint)0})]
        [Xunit.InlineDataAttribute(new object[]{ (uint)4278190080})]
        [Xunit.InlineDataAttribute(new object[]{ (uint)4294967295})]
        [Xunit.InlineDataAttribute(new object[]{ 16711680})]
        [Xunit.InlineDataAttribute(new object[]{ 255})]
        [Xunit.InlineDataAttribute(new object[]{ 65280})]
        [Xunit.TheoryAttribute]
        public void SpanWriteUInt32(uint value) { }
        [Xunit.InlineDataAttribute(new object[]{ (long)1095216660480})]
        [Xunit.InlineDataAttribute(new object[]{ (long)280375465082880})]
        [Xunit.InlineDataAttribute(new object[]{ (long)71776119061217280})]
        [Xunit.InlineDataAttribute(new object[]{ (uint)4278190080})]
        [Xunit.InlineDataAttribute(new object[]{ (ulong)0})]
        [Xunit.InlineDataAttribute(new object[]{ (ulong)18374686479671623680})]
        [Xunit.InlineDataAttribute(new object[]{ (ulong)18446744073709551615})]
        [Xunit.InlineDataAttribute(new object[]{ 16711680})]
        [Xunit.InlineDataAttribute(new object[]{ 255})]
        [Xunit.InlineDataAttribute(new object[]{ 65280})]
        [Xunit.TheoryAttribute]
        public void SpanWriteUInt64(ulong value) { }
    }
}
namespace System.Buffers.Text.Tests
{
    public partial class Base64DecoderUnitTests
    {
        public Base64DecoderUnitTests() { }
        [Xunit.FactAttribute]
        public void BasicDecoding() { }
        [Xunit.FactAttribute]
        public void BasicDecodingWithFinalBlockFalse() { }
        [Xunit.InlineDataAttribute(new object[]{ "AQ==", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQI=", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBA==", 4, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBAU=", 4, 3})]
        [Xunit.TheoryAttribute]
        public void BasicDecodingWithFinalBlockFalseKnownInputInvalid(string inputString, int expectedConsumed, int expectedWritten) { }
        [Xunit.InlineDataAttribute(new object[]{ "A", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQ", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQI", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQID", 4, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBA", 4, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBAU", 4, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBAUG", 8, 6})]
        [Xunit.TheoryAttribute]
        public void BasicDecodingWithFinalBlockFalseKnownInputNeedMoreData(string inputString, int expectedConsumed, int expectedWritten) { }
        [Xunit.InlineDataAttribute(new object[]{ "AQ==", 4, 1})]
        [Xunit.InlineDataAttribute(new object[]{ "AQI=", 4, 2})]
        [Xunit.InlineDataAttribute(new object[]{ "AQID", 4, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBA==", 8, 4})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBAU=", 8, 5})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBAUG", 8, 6})]
        [Xunit.TheoryAttribute]
        public void BasicDecodingWithFinalBlockTrueKnownInputDone(string inputString, int expectedConsumed, int expectedWritten) { }
        [Xunit.InlineDataAttribute(new object[]{ "A", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQ", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQI", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBA", 4, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "AQIDBAU", 4, 3})]
        [Xunit.TheoryAttribute]
        public void BasicDecodingWithFinalBlockTrueKnownInputInvalid(string inputString, int expectedConsumed, int expectedWritten) { }
        [Xunit.FactAttribute]
        public void DecodeEmptySpan() { }
        [Xunit.FactAttribute]
        public void DecodeInPlace() { }
        [Xunit.FactAttribute]
        public void DecodeInPlaceInvalidBytes() { }
        [Xunit.FactAttribute]
        public void DecodeInPlaceInvalidBytesPadding() { }
        [Xunit.FactAttribute]
        public void DecodingInvalidBytes() { }
        [Xunit.FactAttribute]
        public void DecodingInvalidBytesPadding() { }
        [Xunit.FactAttribute]
        public void DecodingOutputTooSmall() { }
        [Xunit.FactAttribute]
        public void DecodingOutputTooSmallRetry() { }
        [Xunit.FactAttribute]
        public void EncodeAndDecodeInPlace() { }
        [Xunit.FactAttribute]
        public void GetMaxDecodedLength() { }
    }
    public partial class Base64EncoderUnitTests
    {
        public Base64EncoderUnitTests() { }
        [Xunit.FactAttribute]
        public void BasicEncoding() { }
        [Xunit.FactAttribute]
        public void BasicEncodingAndDecoding() { }
        [Xunit.FactAttribute]
        public void BasicEncodingWithFinalBlockFalse() { }
        [Xunit.InlineDataAttribute(new object[]{ 1, "", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ 2, "", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ 3, "AQID", 3, 4})]
        [Xunit.InlineDataAttribute(new object[]{ 4, "AQID", 3, 4})]
        [Xunit.InlineDataAttribute(new object[]{ 5, "AQID", 3, 4})]
        [Xunit.InlineDataAttribute(new object[]{ 6, "AQIDBAUG", 6, 8})]
        [Xunit.InlineDataAttribute(new object[]{ 7, "AQIDBAUG", 6, 8})]
        [Xunit.TheoryAttribute]
        public void BasicEncodingWithFinalBlockFalseKnownInput(int numBytes, string expectedText, int expectedConsumed, int expectedWritten) { }
        [Xunit.InlineDataAttribute(new object[]{ 1, "AQ==", 1, 4})]
        [Xunit.InlineDataAttribute(new object[]{ 2, "AQI=", 2, 4})]
        [Xunit.InlineDataAttribute(new object[]{ 3, "AQID", 3, 4})]
        [Xunit.InlineDataAttribute(new object[]{ 4, "AQIDBA==", 4, 8})]
        [Xunit.InlineDataAttribute(new object[]{ 5, "AQIDBAU=", 5, 8})]
        [Xunit.InlineDataAttribute(new object[]{ 6, "AQIDBAUG", 6, 8})]
        [Xunit.InlineDataAttribute(new object[]{ 7, "AQIDBAUGBw==", 7, 12})]
        [Xunit.TheoryAttribute]
        public void BasicEncodingWithFinalBlockTrueKnownInput(int numBytes, string expectedText, int expectedConsumed, int expectedWritten) { }
        [Xunit.FactAttribute]
        public void EncodeEmptySpan() { }
        [Xunit.FactAttribute]
        public void EncodeInPlace() { }
        [Xunit.FactAttribute]
        public void EncodeInPlaceDataLengthTooLarge() { }
        [Xunit.FactAttribute]
        public void EncodeInPlaceOutputTooSmall() { }
        [Xunit.FactAttribute]
        [Xunit.OuterLoopAttribute]
        public void EncodeTooLargeSpan() { }
        [Xunit.FactAttribute]
        public void EncodingOutputTooSmall() { }
        [Xunit.FactAttribute]
        public void EncodingOutputTooSmallRetry() { }
        [Xunit.FactAttribute]
        public void GetMaxEncodedLength() { }
    }
    public static partial class Base64TestHelper
    {
        public static string s_characters;
        public static readonly sbyte[] s_decodingMap;
        public static readonly byte[] s_encodingMap;
        public static readonly byte s_encodingPad;
        public static readonly sbyte s_invalidByte;
        public static byte[] InvalidBytes { get { throw null; } }
        public static int[] FindAllIndexOf<T>(this System.Collections.Generic.IEnumerable<T> values, T valueToFind) { throw null; }
        [Xunit.FactAttribute]
        public static void GenerateDecodingMapAndVerify() { }
        [Xunit.FactAttribute]
        public static void GenerateEncodingMapAndVerify() { }
        public static void InitalizeBytes(System.Span<byte> bytes, int seed = 100) { }
        public static void InitalizeDecodableBytes(System.Span<byte> bytes, int seed = 100) { }
        public static bool VerifyDecodingCorrectness(int expectedConsumed, int expectedWritten, System.Span<byte> source, System.Span<byte> decodedBytes) { throw null; }
        public static bool VerifyEncodingCorrectness(int expectedConsumed, int expectedWritten, System.Span<byte> source, System.Span<byte> encodedBytes) { throw null; }
    }
    public sealed partial class FormatterTestData<T>
    {
        public FormatterTestData(T value, System.Buffers.Text.Tests.SupportedFormat format, byte precision, string expectedOutput) { }
        public string ExpectedOutput { get { throw null; } }
        public System.Buffers.Text.Tests.SupportedFormat Format { get { throw null; } }
        public char FormatSymbol { get { throw null; } }
        public int PassedInBufferLength { get { throw null; } set { } }
        public byte Precision { get { throw null; } }
        public T Value { get { throw null; } }
        public System.Buffers.Text.Tests.ParserTestData<T> ToParserTestData() { throw null; }
        public sealed override string ToString() { throw null; }
    }
    public static partial class FormatterTests
    {
        [Xunit.MemberDataAttribute("TypesThatCanBeFormatted", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestBadFormat(System.Type integerType) { }
        [Xunit.MemberDataAttribute("BooleanFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterBoolean(System.Buffers.Text.Tests.FormatterTestData<bool> testData) { }
        [Xunit.MemberDataAttribute("ByteFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterByte(System.Buffers.Text.Tests.FormatterTestData<byte> testData) { }
        [Xunit.MemberDataAttribute("DateTimeFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterDateTime(System.Buffers.Text.Tests.FormatterTestData<System.DateTime> testData) { }
        [Xunit.MemberDataAttribute("DateTimeOffsetFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterDateTimeOffset(System.Buffers.Text.Tests.FormatterTestData<System.DateTimeOffset> testData) { }
        [Xunit.MemberDataAttribute("DecimalFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterDecimal(System.Buffers.Text.Tests.FormatterTestData<decimal> testData) { }
        [Xunit.MemberDataAttribute("DoubleFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterDouble(System.Buffers.Text.Tests.FormatterTestData<double> testData) { }
        [Xunit.MemberDataAttribute("GuidFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterGuid(System.Buffers.Text.Tests.FormatterTestData<System.Guid> testData) { }
        [Xunit.MemberDataAttribute("Int16FormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterInt16(System.Buffers.Text.Tests.FormatterTestData<short> testData) { }
        [Xunit.MemberDataAttribute("Int32FormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterInt32(System.Buffers.Text.Tests.FormatterTestData<int> testData) { }
        [Xunit.MemberDataAttribute("Int64FormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterInt64(System.Buffers.Text.Tests.FormatterTestData<long> testData) { }
        [Xunit.MemberDataAttribute("SByteFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterSByte(System.Buffers.Text.Tests.FormatterTestData<sbyte> testData) { }
        [Xunit.MemberDataAttribute("SingleFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterSingle(System.Buffers.Text.Tests.FormatterTestData<float> testData) { }
        [Xunit.MemberDataAttribute("TimeSpanFormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterTimeSpan(System.Buffers.Text.Tests.FormatterTestData<System.TimeSpan> testData) { }
        [Xunit.MemberDataAttribute("UInt16FormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterUInt16(System.Buffers.Text.Tests.FormatterTestData<ushort> testData) { }
        [Xunit.MemberDataAttribute("UInt32FormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterUInt32(System.Buffers.Text.Tests.FormatterTestData<uint> testData) { }
        [Xunit.MemberDataAttribute("UInt64FormatterTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestFormatterUInt64(System.Buffers.Text.Tests.FormatterTestData<ulong> testData) { }
        [Xunit.InlineDataAttribute(new object[]{ typeof(decimal)})]
        [Xunit.InlineDataAttribute(new object[]{ typeof(double)})]
        [Xunit.InlineDataAttribute(new object[]{ typeof(float)})]
        [Xunit.MemberDataAttribute("IntegerTypesTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestGFormatWithPrecisionNotSupported(System.Type type) { }
    }
    public sealed partial class ParserTestData<T>
    {
        public ParserTestData(string text, T expectedValue, char formatSymbol, bool expectedSuccess) { }
        public int ExpectedBytesConsumed { get { throw null; } set { } }
        public bool ExpectedSuccess { get { throw null; } }
        public T ExpectedValue { get { throw null; } }
        public char FormatSymbol { get { throw null; } }
        public string Text { get { throw null; } }
        public sealed override string ToString() { throw null; }
    }
    public static partial class ParserTests
    {
        [Xunit.MemberDataAttribute("IntegerTypesTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void FakeTestParserIntegerN(System.Type integerType) { }
        [Xunit.FactAttribute]
        [Xunit.OuterLoopAttribute]
        [Xunit.PlatformSpecificAttribute(5)]
        public static void TestParser2GiBOverflow() { }
        [Xunit.MemberDataAttribute("TypesThatCanBeParsed", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserBadFormat(System.Type type) { }
        [Xunit.MemberDataAttribute("BooleanParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserBoolean(System.Buffers.Text.Tests.ParserTestData<bool> testData) { }
        [Xunit.MemberDataAttribute("ByteParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserByte(System.Buffers.Text.Tests.ParserTestData<byte> testData) { }
        [Xunit.InlineDataAttribute(new object[]{ "12/31/9999 23:59:59", 'G'})]
        [Xunit.InlineDataAttribute(new object[]{ "9999-12-31T23:59:59.9999999", 'O'})]
        [Xunit.TheoryAttribute]
        public static void TestParserDateEndOfTime(string text, char formatSymbol) { }
        [Xunit.MemberDataAttribute("DateTimeParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserDateTime(System.Buffers.Text.Tests.ParserTestData<System.DateTime> testData) { }
        [Xunit.MemberDataAttribute("DateTimeOffsetParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserDateTimeOffset(System.Buffers.Text.Tests.ParserTestData<System.DateTimeOffset> testData) { }
        [Xunit.MemberDataAttribute("DecimalParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserDecimal(System.Buffers.Text.Tests.ParserTestData<decimal> testData) { }
        [Xunit.MemberDataAttribute("DoubleParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserDouble(System.Buffers.Text.Tests.ParserTestData<double> testData) { }
        [Xunit.MemberDataAttribute("GuidParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserGuid(System.Buffers.Text.Tests.ParserTestData<System.Guid> testData) { }
        [Xunit.MemberDataAttribute("Int16ParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserInt16(System.Buffers.Text.Tests.ParserTestData<short> testData) { }
        [Xunit.MemberDataAttribute("Int32ParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserInt32(System.Buffers.Text.Tests.ParserTestData<int> testData) { }
        [Xunit.MemberDataAttribute("Int64ParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserInt64(System.Buffers.Text.Tests.ParserTestData<long> testData) { }
        [Xunit.MemberDataAttribute("SByteParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserSByte(System.Buffers.Text.Tests.ParserTestData<sbyte> testData) { }
        [Xunit.MemberDataAttribute("SingleParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserSingle(System.Buffers.Text.Tests.ParserTestData<float> testData) { }
        [Xunit.MemberDataAttribute("TimeSpanParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserTimeSpan(System.Buffers.Text.Tests.ParserTestData<System.TimeSpan> testData) { }
        [Xunit.MemberDataAttribute("UInt16ParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserUInt16(System.Buffers.Text.Tests.ParserTestData<ushort> testData) { }
        [Xunit.MemberDataAttribute("UInt32ParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserUInt32(System.Buffers.Text.Tests.ParserTestData<uint> testData) { }
        [Xunit.MemberDataAttribute("UInt64ParserTheoryData", new object[]{ }, MemberType=typeof(System.Buffers.Text.Tests.TestData))]
        [Xunit.TheoryAttribute]
        public static void TestParserUInt64(System.Buffers.Text.Tests.ParserTestData<ulong> testData) { }
    }
    public sealed partial class PseudoDateTime
    {
        public PseudoDateTime(int year, int month, int day, int hour, int minute, int second, bool expectSuccess) { }
        public PseudoDateTime(int year, int month, int day, int hour, int minute, int second, int fraction, bool offsetNegative, int offsetHours, int offsetMinutes, bool expectSuccess) { }
        public int Day { get { throw null; } }
        public string DefaultString { get { throw null; } }
        public bool ExpectSuccess { get { throw null; } }
        public int Fraction { get { throw null; } }
        public string GFormatString { get { throw null; } }
        public int Hour { get { throw null; } }
        public string LFormatString { get { throw null; } }
        public int Minute { get { throw null; } }
        public int Month { get { throw null; } }
        public int OffsetHours { get { throw null; } }
        public int OffsetMinutes { get { throw null; } }
        public bool OffsetNegative { get { throw null; } }
        public string OFormatStringNoOffset { get { throw null; } }
        public string OFormatStringOffset { get { throw null; } }
        public string OFormatStringZ { get { throw null; } }
        public string RFormatString { get { throw null; } }
        public int Second { get { throw null; } }
        public int Year { get { throw null; } }
    }
    public static partial class StandardFormatTests
    {
        public static System.Collections.Generic.IEnumerable<object[]> EqualityTestData { get { throw null; } }
        [Xunit.MemberDataAttribute("EqualityTestData", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void StandardFormatBoxedEquality(System.Buffers.StandardFormat f1, System.Buffers.StandardFormat f2, bool expectedToBeEqual) { }
        [Xunit.FactAttribute]
        public static void StandardFormatCtorNegative() { }
        [Xunit.MemberDataAttribute("EqualityTestData", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void StandardFormatEquality(System.Buffers.StandardFormat f1, System.Buffers.StandardFormat f2, bool expectedToBeEqual) { }
        [Xunit.MemberDataAttribute("EqualityTestData", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void StandardFormatGetHashCode(System.Buffers.StandardFormat f1, System.Buffers.StandardFormat f2, bool expectedToBeEqual) { }
        [Xunit.MemberDataAttribute("EqualityTestData", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void StandardFormatGetHashCodeIsContentBased(System.Buffers.StandardFormat f1, System.Buffers.StandardFormat f2, bool expectedToBeEqual) { }
        [Xunit.MemberDataAttribute("EqualityTestData", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void StandardFormatOpEquality(System.Buffers.StandardFormat f1, System.Buffers.StandardFormat f2, bool expectedToBeEqual) { }
        [Xunit.InlineDataAttribute(new object[]{ 'a'})]
        [Xunit.InlineDataAttribute(new object[]{ 'B'})]
        [Xunit.TheoryAttribute]
        public static void StandardFormatOpImplicitFromChar(char c) { }
        [Xunit.MemberDataAttribute("EqualityTestData", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void StandardFormatOpInequality(System.Buffers.StandardFormat f1, System.Buffers.StandardFormat f2, bool expectedToBeEqual) { }
        [Xunit.InlineDataAttribute(new object[]{ "G$"})]
        [Xunit.InlineDataAttribute(new object[]{ "G100"})]
        [Xunit.InlineDataAttribute(new object[]{ "Ga"})]
        [Xunit.TheoryAttribute]
        public static void StandardFormatParseNegative(string badFormatString) { }
        [Xunit.InlineDataAttribute(new object[]{ "", '\0', (byte)0})]
        [Xunit.InlineDataAttribute(new object[]{ "d", 'd', (byte)255})]
        [Xunit.InlineDataAttribute(new object[]{ "G4", 'G', 4})]
        [Xunit.InlineDataAttribute(new object[]{ "n0", 'n', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "x99", 'x', (byte)99})]
        [Xunit.TheoryAttribute]
        public static void StandardFormatParseSpan(string formatString, char expectedSymbol, byte expectedPrecision) { }
        [Xunit.InlineDataAttribute(new object[]{ "", '\0', (byte)0})]
        [Xunit.InlineDataAttribute(new object[]{ "d", 'd', (byte)255})]
        [Xunit.InlineDataAttribute(new object[]{ "G4", 'G', 4})]
        [Xunit.InlineDataAttribute(new object[]{ "n0", 'n', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "x99", 'x', (byte)99})]
        [Xunit.InlineDataAttribute(new object[]{ null, '\0', (byte)0})]
        [Xunit.TheoryAttribute]
        public static void StandardFormatParseString(string formatString, char expectedSymbol, byte expectedPrecision) { }
        [Xunit.InlineDataAttribute(new object[]{ "", '\0', (byte)0})]
        [Xunit.InlineDataAttribute(new object[]{ "d", 'd', (byte)255})]
        [Xunit.InlineDataAttribute(new object[]{ "G4", 'G', 4})]
        [Xunit.InlineDataAttribute(new object[]{ "n0", 'n', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "x99", 'x', (byte)99})]
        [Xunit.TheoryAttribute]
        public static void StandardFormatToString(string expected, char symbol, byte precision) { }
        [Xunit.FactAttribute]
        public static void StandardFormatToStringOversizedPrecision() { }
    }
    public sealed partial class SupportedFormat
    {
        public SupportedFormat(char symbol, bool supportsPrecision) { }
        public char FormatSynonymFor { get { throw null; } set { } }
        public bool IsDefault { get { throw null; } set { } }
        public bool NoRepresentation { get { throw null; } set { } }
        public char ParseSynonymFor { get { throw null; } set { } }
        public bool SupportsPrecision { get { throw null; } }
        public char Symbol { get { throw null; } }
    }
}
namespace System.MemoryTests
{
    public static partial class AsReadOnlyMemory
    {
        [Xunit.FactAttribute]
        public static void EmptyMemoryAsReadOnlyMemory() { }
        [Xunit.FactAttribute]
        public static void MemoryAsReadOnlyMemory() { }
    }
    public partial class CustomMemoryForTest<T> : System.Buffers.OwnedMemory<T>
    {
        public CustomMemoryForTest(T[] array) { }
        public override bool IsDisposed { get { throw null; } }
        protected override bool IsRetained { get { throw null; } }
        public override int Length { get { throw null; } }
        public int OnNoRefencesCalledCount { get { throw null; } }
        public override System.Span<T> Span { get { throw null; } }
        protected override void Dispose(bool disposing) { }
        public override System.Buffers.MemoryHandle Pin(int byteOffset = 0) { throw null; }
        public override bool Release() { throw null; }
        public override void Retain() { }
        protected override bool TryGetArray(out System.ArraySegment<T> arraySegment) { throw null; }
    }
    public static partial class MemoryMarshalTests
    {
        [Xunit.MemberDataAttribute("ReadOnlyMemoryCharInstances", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void AsMemory_Roundtrips(System.ReadOnlyMemory<char> readOnlyMemory) { }
        [Xunit.MemberDataAttribute("ReadOnlyMemoryInt32Instances", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void AsMemory_Roundtrips(System.ReadOnlyMemory<int> readOnlyMemory) { }
        [Xunit.MemberDataAttribute("ReadOnlyMemoryObjectInstances", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void AsMemory_Roundtrips(System.ReadOnlyMemory<object> readOnlyMemory) { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryTryGetArray() { }
        public static System.Collections.Generic.IEnumerable<object[]> ReadOnlyMemoryCharInstances() { throw null; }
        public static System.Collections.Generic.IEnumerable<object[]> ReadOnlyMemoryInt32Instances() { throw null; }
        public static System.Collections.Generic.IEnumerable<object[]> ReadOnlyMemoryObjectInstances() { throw null; }
        [Xunit.FactAttribute]
        public static void ReadOnlyMemoryTryGetArray() { }
        [Xunit.FactAttribute]
        public static void TryGetArrayFromDefaultMemory() { }
    }
    public static partial class MemoryPoolTests
    {
        public static System.Collections.Generic.IEnumerable<object[]> BadSizes { get { throw null; } }
        [Xunit.FactAttribute]
        public static void DisposingTheSharedPoolIsANop() { }
        [Xunit.FactAttribute]
        public static void EachRentalIsUniqueUntilDisposed() { }
        [Xunit.FactAttribute]
        public static void ExtraDisposesAreIgnored() { }
        [Xunit.FactAttribute]
        public static void IsDisposed() { }
        [Xunit.InlineDataAttribute(new object[]{ 0})]
        [Xunit.InlineDataAttribute(new object[]{ 3})]
        [Xunit.InlineDataAttribute(new object[]{ 40})]
        [Xunit.TheoryAttribute]
        public static void MemoryPoolPin(int byteOffset) { }
        [Xunit.InlineDataAttribute(new object[]{ -1})]
        [Xunit.InlineDataAttribute(new object[]{ -2147483648})]
        [Xunit.TheoryAttribute]
        public static void MemoryPoolPinBadOffset(int byteOffset) { }
        [Xunit.FactAttribute]
        public static void MemoryPoolPinBadOffsetTooLarge() { }
        [Xunit.FactAttribute]
        public static void MemoryPoolPinOffsetAtEnd() { }
        [Xunit.FactAttribute]
        public static void MemoryPoolSpan() { }
        [Xunit.FactAttribute]
        public static void MemoryPoolTryGetArray() { }
        [Xunit.FactAttribute]
        public static void NoPinAfterDispose() { }
        [Xunit.FactAttribute]
        public static void NoRelease_AfterDispose() { }
        [Xunit.FactAttribute]
        public static void NoRetainAfterDispose() { }
        [Xunit.FactAttribute]
        public static void NoSpanAfterDispose() { }
        [Xunit.FactAttribute]
        public static void NoTryGetArrayAfterDispose() { }
        [Xunit.FactAttribute]
        public static void RefCounting() { }
        [Xunit.MemberDataAttribute("BadSizes", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void RentBadSizes(int badSize) { }
        [Xunit.FactAttribute]
        public static void RentWithDefaultSize() { }
        [Xunit.FactAttribute]
        public static void RentWithTooLargeASize() { }
        [Xunit.FactAttribute]
        public static void ThereIsOnlyOneSharedPool() { }
    }
    public static partial class MemoryTests
    {
        public static System.Collections.Generic.IEnumerable<object[]> FullArraySegments { get { throw null; } }
        public static System.Collections.Generic.IEnumerable<object[]> ValidArraySegments { get { throw null; } }
        [Xunit.FactAttribute]
        public static void CopyToArray() { }
        [Xunit.FactAttribute]
        public static void CopyToCovariantArray() { }
        [Xunit.FactAttribute]
        public static void CopyToEmptyArray() { }
        [Xunit.FactAttribute]
        public static void CopyToLongerArray() { }
        [Xunit.FactAttribute]
        public static void CopyToShorter() { }
        [Xunit.FactAttribute]
        public static void CopyToShorterArray() { }
        [Xunit.FactAttribute]
        public static void CopyToSingleArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayInt() { }
        [Xunit.FactAttribute]
        public static void CtorArrayLong() { }
        [Xunit.FactAttribute]
        public static void CtorArrayNullArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayObject() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithNegativeStartAndLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthBothEqual() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthInt() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthLong() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthRangeExtendsToEndOfArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthTooLarge() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndNegativeLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartTooLargeAndLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWrongArrayType() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWrongValueType() { }
        [Xunit.FactAttribute]
        public static void CtorArrayZeroLength() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitArray() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitArraySegment() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitDefaultMemory() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitZeroLengthArray() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitZeroLengthArraySegment() { }
        [Xunit.FactAttribute]
        public static void DefaultMemoryCanBeBoxed() { }
        [Xunit.FactAttribute]
        public static void DefaultMemoryHashCode() { }
        [Xunit.InlineDataAttribute(new object[]{ false})]
        [Xunit.InlineDataAttribute(new object[]{ true})]
        [Xunit.TheoryAttribute]
        public static void DefaultMemoryRetain(bool pin) { }
        [Xunit.FactAttribute]
        public static void DisposeOwnedMemoryAfterRetain() { }
        [Xunit.FactAttribute]
        public static void DisposeOwnedMemoryAfterRetainAndRelease() { }
        [Xunit.FactAttribute]
        public static void Empty() { }
        [Xunit.FactAttribute]
        public static void EmptyEqualsDefault() { }
        [Xunit.FactAttribute]
        public static void EmptyMemoryHashCodeNotUnified() { }
        [Xunit.FactAttribute]
        public static void EmptyMemoryNotUnified() { }
        [Xunit.FactAttribute]
        public static void EqualityComparesRangeNotContent() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesBase() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesLength() { }
        [Xunit.FactAttribute]
        public static void EqualityReflexivity() { }
        [Xunit.FactAttribute]
        public static void EqualityTrue() { }
        [Xunit.FactAttribute]
        public static void HashCodeIncludesBase() { }
        [Xunit.FactAttribute]
        public static void HashCodeIncludesLength() { }
        [Xunit.FactAttribute]
        public static void HashCodesDifferentForSameContent() { }
        [Xunit.FactAttribute]
        public static void HashCodesForImplicitCastsAreEqual() { }
        [Xunit.FactAttribute]
        public static void ImplicitReadOnlyMemoryFromOwnedMemory() { }
        [Xunit.FactAttribute]
        public static void IsEmpty() { }
        [Xunit.MemberDataAttribute("FullArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryArrayEquivalenceAndImplicitCastsAreEqual(byte[] bytes) { }
        [Xunit.FactAttribute]
        public static void MemoryCanBeBoxed() { }
        [Xunit.FactAttribute]
        public static void MemoryFromEmptyArrayRetainWithPinning() { }
        [Xunit.FactAttribute]
        public static void MemoryFromOwnedMemoryAfterDispose() { }
        [Xunit.FactAttribute]
        public static void MemoryFromOwnedMemoryInt() { }
        [Xunit.FactAttribute]
        public static void MemoryFromOwnedMemoryLong() { }
        [Xunit.FactAttribute]
        public static void MemoryFromOwnedMemoryObject() { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryOfDifferentValuesAreNotEqual(byte[] bytes, int start, int length) { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryOfEqualValuesAreNotEqual(byte[] bytes, int start, int length) { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryReferencingSameMemoryAreEqualInEveryAspect(byte[] bytes, int start, int length) { }
        [Xunit.FactAttribute]
        public static void MemoryRetainWithoutPinning() { }
        [Xunit.FactAttribute]
        public static void MemoryRetainWithPinning() { }
        [Xunit.FactAttribute]
        public static void MemoryRetainWithPinningAndSlice() { }
        [Xunit.FactAttribute]
        public static void MemoryTryGetArray() { }
        [Xunit.FactAttribute]
        public static void Memory_EqualsAndGetHashCode_ExpectedResults() { }
        [Xunit.FactAttribute]
        public static void Memory_Retain_ExpectedPointerValue() { }
        [Xunit.InlineDataAttribute(new object[]{ "", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 0, 10})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 1, 8})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 1, 9})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 10, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 2, 8})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 5, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 9, 1})]
        [Xunit.TheoryAttribute]
        public static void Memory_Slice_MatchesSubstring(string input, int offset, int count) { }
        [Xunit.MemberDataAttribute("StringInputs", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void Memory_Span_Roundtrips(string input) { }
        [Xunit.MemberDataAttribute("StringInputs", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void Memory_ToArray_Roundtrips(string input) { }
        [Xunit.FactAttribute]
        public static void NullImplicitCast() { }
        [Xunit.FactAttribute]
        public static void Overlapping1() { }
        [Xunit.FactAttribute]
        public static void Overlapping2() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryDispose() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryPinArray() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryPinEmptyArray() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryRetainWithoutPinning() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryRetainWithPinning() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryRetainWithPinningAndSlice() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryTryGetArray() { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void RangedMemoryEquivalenceAndImplicitCastsAreEqual(byte[] bytes, int start, int length) { }
        [Xunit.FactAttribute]
        public static void ReadOnlyMemoryFromMemoryFromOwnedMemoryInt() { }
        [Xunit.FactAttribute]
        public static void SameObjectsHaveSameHashCodes() { }
        [Xunit.FactAttribute]
        public static void SliceRangeChecks() { }
        [Xunit.FactAttribute]
        public static void SliceWithStart() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLength() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLengthDefaultMemory() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLengthPastEnd() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLengthUpToEnd() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartDefaultMemory() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartPastEnd() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayInt() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayLong() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayObject() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayWrongValueType() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayZeroLength() { }
        [Xunit.FactAttribute]
        public static void SpanFromDefaultMemory() { }
        public static System.Collections.Generic.IEnumerable<object[]> StringInputs() { throw null; }
        [Xunit.FactAttribute]
        public static void ToArray1() { }
        [Xunit.FactAttribute]
        public static void ToArrayDefault() { }
        [Xunit.FactAttribute]
        public static void ToArrayEmpty() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndex() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndexAndLength() { }
        [Xunit.FactAttribute]
        public static void ToEnumerable() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableDefault() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableEmpty() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableForEach() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableGivenToExistingConstructor() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableSameAsIEnumerator() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableWithIndex() { }
        [Xunit.FactAttribute]
        public static void ToEnumerableWithIndexAndLength() { }
        [Xunit.FactAttribute]
        public static void TryCopyTo() { }
        [Xunit.FactAttribute]
        public static void TryCopyToArraySegmentImplicit() { }
        [Xunit.FactAttribute]
        public static void TryCopyToEmpty() { }
        [Xunit.FactAttribute]
        public static void TryCopyToLonger() { }
        [Xunit.FactAttribute]
        public static void TryCopyToShorter() { }
        [Xunit.FactAttribute]
        public static void TryCopyToSingle() { }
        [Xunit.FactAttribute]
        public static void TryGetArrayFromDefaultMemory() { }
    }
    public static partial class ReadOnlyMemoryTests
    {
        public static System.Collections.Generic.IEnumerable<object[]> FullArraySegments { get { throw null; } }
        public static System.Collections.Generic.IEnumerable<object[]> ValidArraySegments { get { throw null; } }
        [Xunit.FactAttribute]
        public static void Array_TryGetString_ReturnsFalse() { }
        [Xunit.MemberDataAttribute("StringSlice2ArgTestOutOfRangeData", new object[]{ }, MemberType=typeof(System.TestHelpers))]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlyMemory_2Arg_OutOfRange(string text, int start) { }
        [Xunit.MemberDataAttribute("StringSlice3ArgTestOutOfRangeData", new object[]{ }, MemberType=typeof(System.TestHelpers))]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlyMemory_3Arg_OutOfRange(string text, int start, int length) { }
        [Xunit.FactAttribute]
        public static void AsReadOnlyMemory_EqualsAndGetHashCode_ExpectedResults() { }
        [Xunit.FactAttribute]
        public static void AsReadOnlyMemory_NullString_Throws() { }
        [Xunit.MemberDataAttribute("StringSliceTestData", new object[]{ }, MemberType=typeof(System.TestHelpers))]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlyMemory_PointerAndLength(string text, int start, int length) { }
        [Xunit.FactAttribute]
        public static void AsReadOnlyMemory_Retain_ExpectedPointerValue() { }
        [Xunit.InlineDataAttribute(new object[]{ "", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 0, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 0, 10})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 1, 8})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 1, 9})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 10, 0})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 2, 8})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 5, 3})]
        [Xunit.InlineDataAttribute(new object[]{ "0123456789", 9, 1})]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlyMemory_Slice_MatchesSubstring(string input, int offset, int count) { }
        [Xunit.MemberDataAttribute("StringInputs", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlyMemory_Span_Roundtrips(string input) { }
        [Xunit.MemberDataAttribute("StringInputs", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlyMemory_ToArray_Roundtrips(string input) { }
        [Xunit.FactAttribute]
        public static void AsReadOnlyMemory_TryGetArray_ReturnsFalse() { }
        [Xunit.FactAttribute]
        public static void AsReadOnlyMemory_TryGetString_Roundtrips() { }
        [Xunit.FactAttribute]
        public static void CopyToShorter() { }
        [Xunit.FactAttribute]
        public static void CtorArrayInt() { }
        [Xunit.FactAttribute]
        public static void CtorArrayLong() { }
        [Xunit.FactAttribute]
        public static void CtorArrayNullArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayObject() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithNegativeStartAndLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthBothEqual() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthInt() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthLong() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthRangeExtendsToEndOfArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndLengthTooLarge() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartAndNegativeLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWithStartTooLargeAndLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWrongValueType() { }
        [Xunit.FactAttribute]
        public static void CtorArrayZeroLength() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitArray() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitArraySegment() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitZeroLengthArray() { }
        [Xunit.FactAttribute]
        public static void CtorImplicitZeroLengthArraySegment() { }
        [Xunit.FactAttribute]
        public static void CtorVariantArrayType() { }
        [Xunit.FactAttribute]
        public static void DefaultMemoryHashCode() { }
        [Xunit.InlineDataAttribute(new object[]{ false})]
        [Xunit.InlineDataAttribute(new object[]{ true})]
        [Xunit.TheoryAttribute]
        public static void DefaultMemoryRetain(bool pin) { }
        [Xunit.FactAttribute]
        public static void Empty() { }
        [Xunit.FactAttribute]
        public static void EmptyEqualsDefault() { }
        [Xunit.FactAttribute]
        public static void EmptyMemoryHashCodeNotUnified() { }
        [Xunit.FactAttribute]
        public static void EmptyMemoryNotUnified() { }
        [Xunit.FactAttribute]
        public static void EqualityComparesRangeNotContent() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesBase() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesLength() { }
        [Xunit.FactAttribute]
        public static void EqualityReflexivity() { }
        [Xunit.FactAttribute]
        public static void EqualityTrue() { }
        [Xunit.FactAttribute]
        public static void HashCodeIncludesBase() { }
        [Xunit.FactAttribute]
        public static void HashCodeIncludesLength() { }
        [Xunit.FactAttribute]
        public static void HashCodesDifferentForSameContent() { }
        [Xunit.FactAttribute]
        public static void IsEmpty() { }
        [Xunit.FactAttribute]
        public static void MemoryFromEmptyArrayRetainWithPinning() { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryOfDifferentValuesAreNotEqual(byte[] bytes, int start, int length) { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryOfEqualValuesAreNotEqual(byte[] bytes, int start, int length) { }
        [Xunit.MemberDataAttribute("ValidArraySegments", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void MemoryReferencingSameMemoryAreEqualInEveryAspect(byte[] bytes, int start, int length) { }
        [Xunit.FactAttribute]
        public static void MemoryRetainWithoutPinning() { }
        [Xunit.FactAttribute]
        public static void MemoryRetainWithPinning() { }
        [Xunit.FactAttribute]
        public static void MemoryRetainWithPinningAndSlice() { }
        [Xunit.FactAttribute]
        public static void NullImplicitCast() { }
        [Xunit.FactAttribute]
        public static void Overlapping1() { }
        [Xunit.FactAttribute]
        public static void Overlapping2() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryRetainWithoutPinning() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryRetainWithPinning() { }
        [Xunit.FactAttribute]
        public static void OwnedMemoryRetainWithPinningAndSlice() { }
        [Xunit.FactAttribute]
        public static void SameObjectsHaveSameHashCodes() { }
        [Xunit.FactAttribute]
        public static void SliceRangeChecks() { }
        [Xunit.FactAttribute]
        public static void SliceWithStart() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLength() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLengthDefaultMemory() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLengthPastEnd() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartAndLengthUpToEnd() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartDefaultMemory() { }
        [Xunit.FactAttribute]
        public static void SliceWithStartPastEnd() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayInt() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayLong() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayObject() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayWrongValueType() { }
        [Xunit.FactAttribute]
        public static void SpanFromCtorArrayZeroLength() { }
        [Xunit.FactAttribute]
        public static void SpanFromDefaultMemory() { }
        public static System.Collections.Generic.IEnumerable<object[]> StringInputs() { throw null; }
        [Xunit.FactAttribute]
        public static void ToArray1() { }
        [Xunit.FactAttribute]
        public static void ToArrayDefault() { }
        [Xunit.FactAttribute]
        public static void ToArrayEmpty() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndex() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndexAndLength() { }
        [Xunit.FactAttribute]
        public static void TryCopyTo() { }
        [Xunit.FactAttribute]
        public static void TryCopyToArraySegmentImplicit() { }
        [Xunit.FactAttribute]
        public static void TryCopyToEmpty() { }
        [Xunit.FactAttribute]
        public static void TryCopyToLonger() { }
        [Xunit.FactAttribute]
        public static void TryCopyToShorter() { }
        [Xunit.FactAttribute]
        public static void TryCopyToSingle() { }
    }
}
namespace System.SpanTests
{
    public static partial class MemoryMarshalTests
    {
        [Xunit.FactAttribute]
        public static void CastReadOnlySpanFromTypeContainsReferences() { }
        [Xunit.FactAttribute]
        public static void CastReadOnlySpanOverflow() { }
        [Xunit.FactAttribute]
        public static void CastReadOnlySpanShortToLong() { }
        [Xunit.FactAttribute]
        public static void CastReadOnlySpanToTypeContainsReferences() { }
        [Xunit.FactAttribute]
        public static void CastReadOnlySpanUIntToUShort() { }
        [Xunit.FactAttribute]
        public static void CastSpanFromTypeContainsReferences() { }
        [Xunit.FactAttribute]
        public static void CastSpanOverflow() { }
        [Xunit.FactAttribute]
        public static void CastSpanShortToLong() { }
        [Xunit.FactAttribute]
        public static void CastSpanToEmptyStruct() { }
        [Xunit.FactAttribute]
        public static void CastSpanToTypeContainsReferences() { }
        [Xunit.FactAttribute]
        public static void CastSpanUIntToUShort() { }
        [Xunit.FactAttribute]
        public static void ReadOnlySpanGetReferenceArray() { }
        [Xunit.FactAttribute]
        public static void ReadOnlySpanGetReferenceArrayPastEnd() { }
        [Xunit.FactAttribute]
        public static void ReadOnlySpanGetReferenceEmpty() { }
        [Xunit.FactAttribute]
        public static void ReadOnlySpanGetReferencePointer() { }
        [Xunit.FactAttribute]
        public static void SpanGetReferenceArray() { }
        [Xunit.FactAttribute]
        public static void SpanGetReferenceArrayPastEnd() { }
        [Xunit.FactAttribute]
        public static void SpanGetReferenceEmpty() { }
        [Xunit.FactAttribute]
        public static void SpanGetReferencePointer() { }
    }
    public static partial class ReadOnlySpanTests
    {
        [System.Runtime.CompilerServices.TupleElementNamesAttribute(new string[]{ "Array", "Value", "ExpectedIndex"})]
        public static readonly Xunit.TheoryData<System.ValueTuple<double[], double, int>> s_casesDouble;
        [System.Runtime.CompilerServices.TupleElementNamesAttribute(new string[]{ "Array", "Value", "ExpectedIndex"})]
        public static readonly Xunit.TheoryData<System.ValueTuple<string[], string, int>> s_casesString;
        [System.Runtime.CompilerServices.TupleElementNamesAttribute(new string[]{ "Array", "Value", "ExpectedIndex"})]
        public static readonly Xunit.TheoryData<System.ValueTuple<uint[], uint, int>> s_casesUInt;
        [Xunit.FactAttribute]
        public static void ArraySegmentDefaultImplicitCast() { }
        [Xunit.FactAttribute]
        public static void AsBytesContainsReferences() { }
        [Xunit.FactAttribute]
        public static void AsBytesUIntToByte() { }
        [Xunit.MemberDataAttribute("StringSlice2ArgTestOutOfRangeData", new object[]{ }, MemberType=typeof(System.TestHelpers))]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlySpan_2Arg_OutOfRange(string text, int start) { }
        [Xunit.MemberDataAttribute("StringSlice3ArgTestOutOfRangeData", new object[]{ }, MemberType=typeof(System.TestHelpers))]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlySpan_3Arg_OutOfRange(string text, int start, int length) { }
        [Xunit.MemberDataAttribute("StringSliceTestData", new object[]{ }, MemberType=typeof(System.TestHelpers))]
        [Xunit.TheoryAttribute]
        public static void AsReadOnlySpan_PointerAndLength(string text, int start, int length) { }
        [Xunit.MemberDataAttribute("s_casesDouble", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void BinarySearch_Double([System.Runtime.CompilerServices.TupleElementNamesAttribute(new string[]{ "Array", "Value", "ExpectedIndex"})]System.ValueTuple<double[], double, int> testCase) { }
        [Xunit.FactAttribute]
        [Xunit.OuterLoopAttribute]
        [Xunit.PlatformSpecificAttribute(5)]
        public static void BinarySearch_MaxLength_NoOverflow() { }
        [Xunit.FactAttribute]
        public static void BinarySearch_NullComparableThrows() { }
        [Xunit.FactAttribute]
        public static void BinarySearch_NullComparerThrows() { }
        [Xunit.FactAttribute]
        public static void BinarySearch_Slice() { }
        [Xunit.MemberDataAttribute("s_casesString", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void BinarySearch_String([System.Runtime.CompilerServices.TupleElementNamesAttribute(new string[]{ "Array", "Value", "ExpectedIndex"})]System.ValueTuple<string[], string, int> testCase) { }
        [Xunit.MemberDataAttribute("s_casesUInt", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void BinarySearch_UInt([System.Runtime.CompilerServices.TupleElementNamesAttribute(new string[]{ "Array", "Value", "ExpectedIndex"})]System.ValueTuple<uint[], uint, int> testCase) { }
        [Xunit.FactAttribute]
        public static void CannotCallEqualsOnSpan() { }
        [Xunit.FactAttribute]
        public static void CannotCallGetHashCodeOnSpan() { }
        [Xunit.ActiveIssueAttribute(25254, -1, 0)]
        [Xunit.InlineDataAttribute(new object[]{ (long)4294967296})]
        [Xunit.InlineDataAttribute(new object[]{ (long)4294967552})]
        [Xunit.OuterLoopAttribute]
        [Xunit.PlatformSpecificAttribute(5)]
        [Xunit.TheoryAttribute]
        public static void CopyToLargeSizeTest(long bufferSize) { }
        [Xunit.FactAttribute]
        public static void CopyToShorter() { }
        [Xunit.FactAttribute]
        public static void CopyToVaryingSizes() { }
        [Xunit.FactAttribute]
        public static void CtorArray1() { }
        [Xunit.FactAttribute]
        public static void CtorArray2() { }
        [Xunit.FactAttribute]
        public static void CtorArray3() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntInt1() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntInt2() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntNegativeLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntNegativeStart() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntRangeExtendsToEndOfArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntStartAndLengthTooLarge() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntStartEqualsLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntStartTooLarge() { }
        [Xunit.FactAttribute]
        public static void CtorArrayNullArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWrongValueType() { }
        [Xunit.FactAttribute]
        public static void CtorArrayZeroLength() { }
        [Xunit.FactAttribute]
        public static void CtorPointerInt() { }
        [Xunit.FactAttribute]
        public static void CtorPointerNoContainsReferenceEnforcement() { }
        [Xunit.FactAttribute]
        public static void CtorPointerNull() { }
        [Xunit.FactAttribute]
        public static void CtorPointerRangeChecks() { }
        [Xunit.FactAttribute]
        public static void CtorVariantArrayType() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfMany_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfThree_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void Empty() { }
        [Xunit.FactAttribute]
        public static void EmptyArrayAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void EmptySpanAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void EmptySpansNotUnified() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatch() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatchDifferentSpans() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatchDifferentSpans_Byte() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void EndsWithNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void EqualityBasedOnLocationNotConstructor() { }
        [Xunit.FactAttribute]
        public static void EqualityComparesRangeNotContent() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesBase() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesLength() { }
        [Xunit.FactAttribute]
        public static void EqualityReflexivity() { }
        [Xunit.FactAttribute]
        public static void EqualityTrue() { }
        [Xunit.MemberDataAttribute("IntegerArrays", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void GetEnumerator_ForEach_AllValuesReturnedCorrectly(int[] array) { }
        [Xunit.MemberDataAttribute("IntegerArrays", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void GetEnumerator_Manual_AllValuesReturnedCorrectly(int[] array) { }
        [Xunit.FactAttribute]
        public static void GetEnumerator_MoveNextOnDefault_ReturnsFalse() { }
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/ HTTP/1.1", " %?", '%', 21})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/?key=value HTTP/1.1", " %?", '%', 21})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/ HTTP/1.1", " %?", ' ', 27})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/?key=value HTTP/1.1", " %?", '?', 27})]
        [Xunit.InlineDataAttribute(new object[]{ "a", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "aaaaaaaaaaacmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'm', 12})]
        [Xunit.InlineDataAttribute(new object[]{ "aaaaaaaaaaalmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "aaaaaaaaaaarmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'r', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "aab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "ab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ll", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lo", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "mlr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ol", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "rml", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "c", 'c', 1})]
        [Xunit.TheoryAttribute]
        public static void IndexOfAnyStrings_Byte(string raw, string search, char expectResult, int expectIndex) { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue_String() { }
        [Xunit.FactAttribute]
        [Xunit.OuterLoopAttribute]
        [Xunit.PlatformSpecificAttribute(5)]
        public static void IndexOverflow() { }
        [Xunit.FactAttribute]
        public static void IntArrayAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void IntArraySegmentAsSpan() { }
        public static System.Collections.Generic.IEnumerable<object[]> IntegerArrays() { throw null; }
        [Xunit.FactAttribute]
        public static void IsWhiteSpaceFalse() { }
        [Xunit.FactAttribute]
        public static void IsWhiteSpaceTrue() { }
        [Xunit.FactAttribute]
        public static void IsWhiteSpaceTrueLatin1() { }
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/ HTTP/1.1", " %?", ' ', 30})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/?key=value HTTP/1.1", " %?", ' ', 40})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/ HTTP/1.1", " %?", ' ', 27})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/?key=value HTTP/1.1", " %?", ' ', 37})]
        [Xunit.InlineDataAttribute(new object[]{ "a", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "aab", "a", 'a', 1})]
        [Xunit.InlineDataAttribute(new object[]{ "ab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ll", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lmr", 'r', 17})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lo", 'o', 14})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "mlr", 'r', 17})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ol", 'o', 14})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "rml", 'r', 17})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqlzzzzzzzz", "lmr", 'l', 43})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'r', 43})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrzzzzzzzz", "lmr", 'r', 43})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqxzzzzzzzz", "lmr", 'm', 38})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "a", 'a', 2})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "c", 'c', 1})]
        [Xunit.TheoryAttribute]
        public static void LastIndexOfAnyStrings_Byte(string raw, string search, char expectResult, int expectIndex) { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueMultipleTimes() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueMultipleTimes_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueMultipleTimes_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue_String() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchEndsWith() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchEndsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceEqual() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceEqual_Byte() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceEqual_Char() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchStartsWith() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchStartsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void LongArrayAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void LongArraySegmentAsSpan() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeMany_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeThree_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRange_Char() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoEndsWithChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoIsWhiteSpaceChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange_Char() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoStartsWithChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoStartsWithChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoTrimCharacterChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoTrimCharactersChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoTrimChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void NoTrimCharacter() { }
        [Xunit.FactAttribute]
        public static void NoTrimCharacters() { }
        [Xunit.FactAttribute]
        public static void NoWhiteSpaceTrim() { }
        [Xunit.FactAttribute]
        public static void NullArrayAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void NullImplicitCast() { }
        [Xunit.FactAttribute]
        public static void ObjectArrayAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void ObjectArraySegmentAsSpan() { }
        [Xunit.FactAttribute]
        public static void OnEndsWithOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnlyTrimCharacter() { }
        [Xunit.FactAttribute]
        public static void OnlyTrimCharacters() { }
        [Xunit.FactAttribute]
        public static void OnlyWhiteSpaceTrim() { }
        [Xunit.FactAttribute]
        public static void OnNoMatchMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnNoMatchMakeSureEveryElementIsComparedLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void OnSequenceEqualOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnStartsWithOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 0, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 0, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 100, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 100, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 0, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 0, 100, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 0, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 0, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 100, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 100, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 100, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 0, 100, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 0, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 0, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 100, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 100, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 200, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 0, 100, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 0, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 0, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 100, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 100, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 200, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 0, 300, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 0, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 0, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 100, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 100, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 100, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 0, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 0, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 100, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 100, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 200, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 0, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 0, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 100, 200, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 100, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 200, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 100, 300, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 0, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 0, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 100, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 100, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 200, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 0, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 0, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 100, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 100, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 200, 300, true})]
        [Xunit.InlineDataAttribute(new object[]{ 200, 300, 300, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 0, 0, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 0, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 0, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 0, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 100, 100, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 100, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 100, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 200, 200, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 200, 300, false})]
        [Xunit.InlineDataAttribute(new object[]{ 300, 300, 300, 300, false})]
        [Xunit.TheoryAttribute]
        public static void Overlap(int x1, int y1, int x2, int y2, bool expected) { }
        [Xunit.FactAttribute]
        public static void Overlapping1() { }
        [Xunit.FactAttribute]
        public static void Overlapping2() { }
        [Xunit.FactAttribute]
        public static void SameSpanEndsWith() { }
        [Xunit.FactAttribute]
        public static void SameSpanEndsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceEqual() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceEqual_Byte() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceEqual_Char() { }
        [Xunit.FactAttribute]
        public static void SameSpanStartsWith() { }
        [Xunit.FactAttribute]
        public static void SameSpanStartsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualArrayImplicit_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualArraySegmentImplicit_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualNoMatch() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualNoMatch_Char() { }
        [Xunit.FactAttribute]
        public static void SizeOf16Overlaps() { }
        [Xunit.FactAttribute]
        public static void SizeOf1Overlaps() { }
        [Xunit.FactAttribute]
        public static void SliceInt() { }
        [Xunit.FactAttribute]
        public static void SliceIntInt() { }
        [Xunit.FactAttribute]
        public static void SliceIntIntPastEnd() { }
        [Xunit.FactAttribute]
        public static void SliceIntIntUpToEnd() { }
        [Xunit.FactAttribute]
        public static void SliceIntPastEnd() { }
        [Xunit.FactAttribute]
        public static void SliceIntRangeChecksd() { }
        [Xunit.FactAttribute]
        public static void SpanAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatch() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatchDifferentSpans() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatchDifferentSpans_Byte() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void StartsWithNoMatch() { }
        [Xunit.FactAttribute]
        public static void StartsWithNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void StringAsReadOnlySpanEmptyString() { }
        [Xunit.FactAttribute]
        public static void StringAsReadOnlySpanNullary() { }
        [Xunit.FactAttribute]
        public static void StringAsReadOnlySpanNullChecked() { }
        [Xunit.FactAttribute]
        public static void TestAlignedBackwards() { }
        [Xunit.FactAttribute]
        public static void TestAlignedForwards() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentNoMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatch() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMatchMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchThree_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatch_Char() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatch() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchThree_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatch_Char() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchThree_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestUnalignedBackwards() { }
        [Xunit.FactAttribute]
        public static void TestUnalignedForwards() { }
        [Xunit.FactAttribute]
        public static void ToArray1() { }
        [Xunit.FactAttribute]
        public static void ToArrayEmpty() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndex() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndexAndLength() { }
        [Xunit.FactAttribute]
        public static void TrimCharacterAtEnd() { }
        [Xunit.FactAttribute]
        public static void TrimCharacterAtStart() { }
        [Xunit.FactAttribute]
        public static void TrimCharacterAtStartAndEnd() { }
        [Xunit.FactAttribute]
        public static void TrimCharacterInMiddle() { }
        [Xunit.FactAttribute]
        public static void TrimCharacterMultipleTimes() { }
        [Xunit.FactAttribute]
        public static void TrimCharactersAtEnd() { }
        [Xunit.FactAttribute]
        public static void TrimCharactersAtStart() { }
        [Xunit.FactAttribute]
        public static void TrimCharactersAtStartAndEnd() { }
        [Xunit.FactAttribute]
        public static void TrimCharactersInMiddle() { }
        [Xunit.FactAttribute]
        public static void TrimCharactersMultipleTimes() { }
        [Xunit.FactAttribute]
        public static void TrimWhiteSpaceMultipleTimes() { }
        [Xunit.FactAttribute]
        public static void TryCopyTo() { }
        [Xunit.FactAttribute]
        public static void TryCopyToArraySegmentImplicit() { }
        [Xunit.FactAttribute]
        public static void TryCopyToEmpty() { }
        [Xunit.FactAttribute]
        public static void TryCopyToLonger() { }
        [Xunit.FactAttribute]
        public static void TryCopyToShorter() { }
        [Xunit.FactAttribute]
        public static void TryCopyToSingle() { }
        [Xunit.FactAttribute]
        public static void WhiteSpaceAtEndTrim() { }
        [Xunit.FactAttribute]
        public static void WhiteSpaceAtStartAndEndTrim() { }
        [Xunit.FactAttribute]
        public static void WhiteSpaceAtStartTrim() { }
        [Xunit.FactAttribute]
        public static void WhiteSpaceInMiddleTrim() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthArraySegmentAsReadOnlySpan() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthEndsWith() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthEndsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfMany_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfThree_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_ThreeString() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIsWhiteSpace() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceEqual() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceEqual_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceEqual_Char() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthStartsWith() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthStartsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthTrim() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthTrimCharacter() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthTrimCharacters() { }
    }
    public static partial class SpanGcReportingTests
    {
        [Xunit.FactAttribute]
        [Xunit.OuterLoopAttribute]
        public static void DelegateTest() { }
        public static void DelegateTest_Stress() { }
    }
    public static partial class SpanTests
    {
        [Xunit.FactAttribute]
        public static void ArraySegmentDefaultImplicitCast() { }
        [Xunit.FactAttribute]
        public static void AsBytesContainsReferences() { }
        [Xunit.FactAttribute]
        public static void AsBytesUIntToByte() { }
        [Xunit.FactAttribute]
        public static void CannotCallEqualsOnSpan() { }
        [Xunit.FactAttribute]
        public static void CannotCallGetHashCodeOnSpan() { }
        [Xunit.FactAttribute]
        public static void ClearByteLonger() { }
        [Xunit.FactAttribute]
        public static void ClearByteUnaligned() { }
        [Xunit.FactAttribute]
        public static void ClearByteUnalignedFixed() { }
        [Xunit.FactAttribute]
        public static void ClearEmpty() { }
        [Xunit.FactAttribute]
        public static void ClearEmptyWithReference() { }
        [Xunit.FactAttribute]
        public static void ClearEnumType() { }
        [Xunit.FactAttribute]
        public static void ClearIntPtrLonger() { }
        [Xunit.FactAttribute]
        public static void ClearIntPtrOffset() { }
        [Xunit.FactAttribute]
        public static void ClearReferenceType() { }
        [Xunit.FactAttribute]
        public static void ClearReferenceTypeLonger() { }
        [Xunit.FactAttribute]
        public static void ClearValueTypeWithoutReferences() { }
        [Xunit.FactAttribute]
        public static void ClearValueTypeWithoutReferencesLonger() { }
        [Xunit.FactAttribute]
        public static void ClearValueTypeWithoutReferencesPointerSize() { }
        [Xunit.FactAttribute]
        public static void ClearValueTypeWithReferences() { }
        [Xunit.FactAttribute]
        public static void CopyToArray() { }
        [Xunit.FactAttribute]
        public static void CopyToCovariantArray() { }
        [Xunit.FactAttribute]
        public static void CopyToEmptyArray() { }
        [Xunit.ActiveIssueAttribute(25254, -1, 0)]
        [Xunit.InlineDataAttribute(new object[]{ (long)4294967296})]
        [Xunit.InlineDataAttribute(new object[]{ (long)4294967552})]
        [Xunit.OuterLoopAttribute]
        [Xunit.PlatformSpecificAttribute(5)]
        [Xunit.TheoryAttribute]
        public static void CopyToLargeSizeTest(long bufferSize) { }
        [Xunit.FactAttribute]
        public static void CopyToLongerArray() { }
        [Xunit.FactAttribute]
        public static void CopyToShorter() { }
        [Xunit.FactAttribute]
        public static void CopyToShorterArray() { }
        [Xunit.FactAttribute]
        public static void CopyToSingleArray() { }
        [Xunit.FactAttribute]
        public static void CopyToVaryingSizes() { }
        [Xunit.FactAttribute]
        public static void CtorArray1() { }
        [Xunit.FactAttribute]
        public static void CtorArray2() { }
        [Xunit.FactAttribute]
        public static void CtorArray3() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntInt1() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntInt2() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntNegativeLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntNegativeStart() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntRangeExtendsToEndOfArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntStartAndLengthTooLarge() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntStartEqualsLength() { }
        [Xunit.FactAttribute]
        public static void CtorArrayIntIntStartTooLarge() { }
        [Xunit.FactAttribute]
        public static void CtorArrayNullArray() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWrongArrayType() { }
        [Xunit.FactAttribute]
        public static void CtorArrayWrongValueType() { }
        [Xunit.FactAttribute]
        public static void CtorArrayZeroLength() { }
        [Xunit.FactAttribute]
        public static void CtorPointerInt() { }
        [Xunit.FactAttribute]
        public static void CtorPointerNoContainsReferenceEnforcement() { }
        [Xunit.FactAttribute]
        public static void CtorPointerNull() { }
        [Xunit.FactAttribute]
        public static void CtorPointerRangeChecks() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfMany_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfThree_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOfTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void DefaultFilledLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void Empty() { }
        [Xunit.FactAttribute]
        public static void EmptySpansNotUnified() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatch() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatchDifferentSpans() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatchDifferentSpans_Byte() { }
        [Xunit.FactAttribute]
        public static void EndsWithMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void EndsWithNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void EqualityBasedOnLocationNotConstructor() { }
        [Xunit.FactAttribute]
        public static void EqualityComparesRangeNotContent() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesBase() { }
        [Xunit.FactAttribute]
        public static void EqualityIncludesLength() { }
        [Xunit.FactAttribute]
        public static void EqualityReflexivity() { }
        [Xunit.FactAttribute]
        public static void EqualityTrue() { }
        [Xunit.FactAttribute]
        public static void FillByteLonger() { }
        [Xunit.FactAttribute]
        public static void FillByteUnaligned() { }
        [Xunit.FactAttribute]
        public static void FillEmpty() { }
        [Xunit.FactAttribute]
        public static void FillNativeBytes() { }
        [Xunit.FactAttribute]
        public static void FillReferenceType() { }
        [Xunit.FactAttribute]
        public static void FillValueTypeWithoutReferences() { }
        [Xunit.FactAttribute]
        public static void FillValueTypeWithReferences() { }
        [Xunit.MemberDataAttribute("IntegerArrays", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void GetEnumerator_ForEach_AllValuesReturnedCorrectly(int[] array) { }
        [Xunit.MemberDataAttribute("IntegerArrays", new object[]{ })]
        [Xunit.TheoryAttribute]
        public static void GetEnumerator_Manual_AllValuesReturnedCorrectly(int[] array) { }
        [Xunit.FactAttribute]
        public static void GetEnumerator_MoveNextOnDefault_ReturnsFalse() { }
        [Xunit.FactAttribute]
        public static void GetEnumerator_RefCurrentChangesAreStoredInSpan() { }
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/ HTTP/1.1", " %?", '%', 21})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/?key=value HTTP/1.1", " %?", '%', 21})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/ HTTP/1.1", " %?", ' ', 27})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/?key=value HTTP/1.1", " %?", '?', 27})]
        [Xunit.InlineDataAttribute(new object[]{ "a", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "aaaaaaaaaaacmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'm', 12})]
        [Xunit.InlineDataAttribute(new object[]{ "aaaaaaaaaaalmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "aaaaaaaaaaarmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'r', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "aab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "ab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ll", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lo", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "mlr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ol", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "rml", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "c", 'c', 1})]
        [Xunit.TheoryAttribute]
        public static void IndexOfAnyStrings_Byte(string raw, string search, char expectResult, int expectIndex) { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceJustPastVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValueJustPasttVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceLengthOneValue_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtStart_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMatchAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceMultipleMatch_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNoMatch_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceNotEvenAHeadMatch_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceRestart_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthSpan_String() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue_Byte() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue_Char() { }
        [Xunit.FactAttribute]
        public static void IndexOfSequenceZeroLengthValue_String() { }
        [Xunit.FactAttribute]
        [Xunit.OuterLoopAttribute]
        [Xunit.PlatformSpecificAttribute(5)]
        public static void IndexOverflow() { }
        [Xunit.FactAttribute]
        public static void IntArrayAsSpan() { }
        [Xunit.FactAttribute]
        public static void IntArraySegmentAsSpan() { }
        public static System.Collections.Generic.IEnumerable<object[]> IntegerArrays() { throw null; }
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/ HTTP/1.1", " %?", ' ', 30})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/%2FPATH2/?key=value HTTP/1.1", " %?", ' ', 40})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/ HTTP/1.1", " %?", ' ', 27})]
        [Xunit.InlineDataAttribute(new object[]{ "/localhost:5000/PATH/PATH2/?key=value HTTP/1.1", " %?", ' ', 37})]
        [Xunit.InlineDataAttribute(new object[]{ "a", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "aab", "a", 'a', 1})]
        [Xunit.InlineDataAttribute(new object[]{ "ab", "a", 'a', 0})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ll", 'l', 11})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lmr", 'r', 17})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "lo", 'o', 14})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "mlr", 'r', 17})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "ol", 'o', 14})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyz", "rml", 'r', 17})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqlzzzzzzzz", "lmr", 'l', 43})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz", "lmr", 'r', 43})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrzzzzzzzz", "lmr", 'r', 43})]
        [Xunit.InlineDataAttribute(new object[]{ "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqxzzzzzzzz", "lmr", 'm', 38})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "a", 'a', 2})]
        [Xunit.InlineDataAttribute(new object[]{ "acab", "c", 'c', 1})]
        [Xunit.TheoryAttribute]
        public static void LastIndexOfAnyStrings_Byte(string raw, string search, char expectResult, int expectIndex) { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceJustPastVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueJustPasttVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueMultipleTimes() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueMultipleTimes_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValueMultipleTimes_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceLengthOneValue_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtStart_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMatchAtVeryEnd_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceMultipleMatch_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNoMatch_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceNotEvenAHeadMatch_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceRestart_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthSpan_String() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue_Byte() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue_Char() { }
        [Xunit.FactAttribute]
        public static void LastIndexOfSequenceZeroLengthValue_String() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchEndsWith() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchEndsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceCompareTo() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceCompareTo_Byte() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceEqual() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceEqual_Byte() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchSequenceEqual_Char() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchStartsWith() { }
        [Xunit.FactAttribute]
        public static void LengthMismatchStartsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void LongArrayAsSpan() { }
        [Xunit.FactAttribute]
        public static void LongArraySegmentAsSpan() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeMany_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeThree_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRangeTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoChecksGoOutOfRange_Char() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoEndsWithChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceCompareToChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceCompareToChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange_Char() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoStartsWithChecksGoOutOfRange() { }
        [Xunit.FactAttribute]
        public static void MakeSureNoStartsWithChecksGoOutOfRange_Byte() { }
        [Xunit.FactAttribute]
        public static void NullImplicitCast() { }
        [Xunit.FactAttribute]
        public static void ObjectArrayAsSpan() { }
        [Xunit.FactAttribute]
        public static void ObjectArraySegmentAsSpan() { }
        [Xunit.FactAttribute]
        public static void OnEndsWithOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnNoMatchMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnNoMatchMakeSureEveryElementIsComparedLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void OnSequenceCompareToOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnSequenceEqualOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void OnStartsWithOfEqualSpansMakeSureEveryElementIsCompared() { }
        [Xunit.FactAttribute]
        public static void Overlapping1() { }
        [Xunit.FactAttribute]
        public static void Overlapping2() { }
        [Xunit.FactAttribute]
        public static void ReverseByte() { }
        [Xunit.FactAttribute]
        public static void ReverseByteTwiceReturnsOriginal() { }
        [Xunit.FactAttribute]
        public static void ReverseByteUnaligned() { }
        [Xunit.FactAttribute]
        public static void ReverseEmpty() { }
        [Xunit.FactAttribute]
        public static void ReverseEmptyWithReference() { }
        [Xunit.FactAttribute]
        public static void ReverseEnumType() { }
        [Xunit.FactAttribute]
        public static void ReverseIntPtrOffset() { }
        [Xunit.FactAttribute]
        public static void ReverseReferenceTwiceReturnsOriginal() { }
        [Xunit.FactAttribute]
        public static void ReverseReferenceType() { }
        [Xunit.FactAttribute]
        public static void ReverseValueTypeWithoutReferences() { }
        [Xunit.FactAttribute]
        public static void ReverseValueTypeWithoutReferencesPointerSize() { }
        [Xunit.FactAttribute]
        public static void ReverseValueTypeWithReferences() { }
        [Xunit.FactAttribute]
        public static void SameSpanEndsWith() { }
        [Xunit.FactAttribute]
        public static void SameSpanEndsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceCompareTo() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceCompareTo_Byte() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceEqual() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceEqual_Byte() { }
        [Xunit.FactAttribute]
        public static void SameSpanSequenceEqual_Char() { }
        [Xunit.FactAttribute]
        public static void SameSpanStartsWith() { }
        [Xunit.FactAttribute]
        public static void SameSpanStartsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceCompareToArrayImplicit_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceCompareToArraySegmentImplicit_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceCompareToNoMatch() { }
        [Xunit.FactAttribute]
        public static void SequenceCompareToNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceCompareToSingleMismatch() { }
        [Xunit.FactAttribute]
        public static void SequenceCompareToWithSingleMismatch_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualArrayImplicit_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualArraySegmentImplicit_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualNoMatch() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void SequenceEqualNoMatch_Char() { }
        [Xunit.FactAttribute]
        public static void SliceInt() { }
        [Xunit.FactAttribute]
        public static void SliceIntInt() { }
        [Xunit.FactAttribute]
        public static void SliceIntIntPastEnd() { }
        [Xunit.FactAttribute]
        public static void SliceIntIntUpToEnd() { }
        [Xunit.FactAttribute]
        public static void SliceIntPastEnd() { }
        [Xunit.FactAttribute]
        public static void SliceIntRangeChecksd() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatch() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatchDifferentSpans() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatchDifferentSpans_Byte() { }
        [Xunit.FactAttribute]
        public static void StartsWithMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void StartsWithNoMatch() { }
        [Xunit.FactAttribute]
        public static void StartsWithNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentNoMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestAllignmentNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatch() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void TestMatchIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void TestMatchLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMatchMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchThree_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMatchValuesLargerMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMatch_Char() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatch() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchThree_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatchTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void TestMultipleMatch_Char() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_ThreeString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchThree_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatchValuesLargerMany_Byte() { }
        [Xunit.FactAttribute]
        public static void TestNoMatch_Byte() { }
        [Xunit.FactAttribute]
        public static void ToArray1() { }
        [Xunit.FactAttribute]
        public static void ToArrayEmpty() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndex() { }
        [Xunit.FactAttribute]
        public static void ToArrayWithIndexAndLength() { }
        [Xunit.FactAttribute]
        public static void TryCopyTo() { }
        [Xunit.FactAttribute]
        public static void TryCopyToArraySegmentImplicit() { }
        [Xunit.FactAttribute]
        public static void TryCopyToEmpty() { }
        [Xunit.FactAttribute]
        public static void TryCopyToLonger() { }
        [Xunit.FactAttribute]
        public static void TryCopyToShorter() { }
        [Xunit.FactAttribute]
        public static void TryCopyToSingle() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthArrayAsSpan() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthArraySegmentAsSpan() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthEndsWith() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthEndsWith_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_ManyInteger() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_ManyString() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_ThreeInteger() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_TwoInteger() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfAny_TwoString() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfMany_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfThree_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOfTwo_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_Byte_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_String_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_ThreeByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthIndexOf_ThreeString() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_Byte_ManyByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_Byte_TwoByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_ManyByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_String_ManyByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_String_TwoByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOfAny_TwoByte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf_Char() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthLastIndexOf_String() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceCompareTo() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceCompareTo_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceEqual() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceEqual_Byte() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthSequenceEqual_Char() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthStartsWith() { }
        [Xunit.FactAttribute]
        public static void ZeroLengthStartsWith_Byte() { }
    }
}
