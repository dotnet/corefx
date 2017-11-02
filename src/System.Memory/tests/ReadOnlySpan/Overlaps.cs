using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void SizeOf1Overlaps()
        {
            byte[] a = new byte[16];

            Assert.True(a.AsReadOnlySpan().Slice(0, 12).Overlaps(a.AsReadOnlySpan().Slice(8, 8), out int elementOffset));
            Assert.Equal(8, elementOffset);
        }

        [Fact]
        public static void SizeOf16Overlaps()
        {
            Guid[] a = new Guid[16];

            Assert.True(a.AsReadOnlySpan().Slice(0, 12).Overlaps(a.AsReadOnlySpan().Slice(8, 8), out int elementOffset));
            Assert.Equal(8, elementOffset);
        }

        [Fact]
        public static unsafe void UnalignedOverlapThrows()
        {
            Assert.Throws<ArgumentException>("second", () =>
            {
                byte* p = stackalloc byte[16];

                ReadOnlySpan<int> first = new ReadOnlySpan<int>(p + 0, 2 * sizeof(int));
                ReadOnlySpan<int> second = new ReadOnlySpan<int>(p + 7, 2 * sizeof(int));

                first.Overlaps(second, out int elementOffset);
            });
        }

        //
        // The following tests were all generated with by this (otherwise unused) method:
        //
        private static void GenerateOverlapsTests()
        {
            const int count = 4;
            string result = "";

            for (int x1 = 0; x1 < count; x1++)
            {
                for (int x2 = x1; x2 < count; x2++)
                {
                    for (int y1 = 0; y1 < count; y1++)
                    {
                        for (int y2 = y1; y2 < count; y2++)
                        {
                            bool expected = (x1 < x2) && (y1 < y2) && (x1 < y2) && (y1 < x2);

                            result += $"[InlineData({x1 * 100}, {x2 * 100}, {y1 * 100}, {y2 * 100}, {(expected ? "true" : "false")})]\r\n";
                        }
                    }
                }
            }
        }

        //
        //               0
        //       first:  |
        //      second:  |
        //               0
        //
        [InlineData(0, 0, 0, 0, false)]

        //
        //               0
        //       first:  |
        //      second:  [---------)
        //               0        100
        //
        [InlineData(0, 0, 0, 100, false)]

        //
        //               0
        //       first:  |
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(0, 0, 0, 200, false)]

        //
        //               0
        //       first:  |
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(0, 0, 0, 300, false)]

        //
        //               0
        //       first:  |
        //      second:            |
        //                        100
        //
        [InlineData(0, 0, 100, 100, false)]

        //
        //               0
        //       first:  |
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(0, 0, 100, 200, false)]

        //
        //               0
        //       first:  |
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(0, 0, 100, 300, false)]

        //
        //               0
        //       first:  |
        //      second:                      |
        //                                  200
        //
        [InlineData(0, 0, 200, 200, false)]

        //
        //               0
        //       first:  |
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(0, 0, 200, 300, false)]

        //
        //               0
        //       first:  |
        //      second:                                |
        //                                            300
        //
        [InlineData(0, 0, 300, 300, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:  |
        //               0
        //
        [InlineData(0, 100, 0, 0, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:  [---------)
        //               0        100
        //
        [InlineData(0, 100, 0, 100, true)]

        //
        //               0        100
        //       first:  [---------)
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(0, 100, 0, 200, true)]

        //
        //               0        100
        //       first:  [---------)
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(0, 100, 0, 300, true)]

        //
        //               0        100
        //       first:  [---------)
        //      second:            |
        //                        100
        //
        [InlineData(0, 100, 100, 100, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(0, 100, 100, 200, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(0, 100, 100, 300, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:                      |
        //                                  200
        //
        [InlineData(0, 100, 200, 200, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(0, 100, 200, 300, false)]

        //
        //               0        100
        //       first:  [---------)
        //      second:                                |
        //                                            300
        //
        [InlineData(0, 100, 300, 300, false)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:  |
        //               0
        //
        [InlineData(0, 200, 0, 0, false)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:  [---------)
        //               0        100
        //
        [InlineData(0, 200, 0, 100, true)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(0, 200, 0, 200, true)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(0, 200, 0, 300, true)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:            |
        //                        100
        //
        [InlineData(0, 200, 100, 100, false)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(0, 200, 100, 200, true)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(0, 200, 100, 300, true)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:                      |
        //                                  200
        //
        [InlineData(0, 200, 200, 200, false)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(0, 200, 200, 300, false)]

        //
        //               0                  200
        //       first:  [-------------------)
        //      second:                                |
        //                                            300
        //
        [InlineData(0, 200, 300, 300, false)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:  |
        //               0
        //
        [InlineData(0, 300, 0, 0, false)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:  [---------)
        //               0        100
        //
        [InlineData(0, 300, 0, 100, true)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(0, 300, 0, 200, true)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(0, 300, 0, 300, true)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:            |
        //                        100
        //
        [InlineData(0, 300, 100, 100, false)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(0, 300, 100, 200, true)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(0, 300, 100, 300, true)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:                      |
        //                                  200
        //
        [InlineData(0, 300, 200, 200, false)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(0, 300, 200, 300, true)]

        //
        //               0                            300
        //       first:  [-----------------------------)
        //      second:                                |
        //                                            300
        //
        [InlineData(0, 300, 300, 300, false)]

        //
        //                        100
        //       first:            |
        //      second:  |
        //               0
        //
        [InlineData(100, 100, 0, 0, false)]

        //
        //                        100
        //       first:            |
        //      second:  [---------)
        //               0        100
        //
        [InlineData(100, 100, 0, 100, false)]

        //
        //                        100
        //       first:            |
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(100, 100, 0, 200, false)]

        //
        //                        100
        //       first:            |
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(100, 100, 0, 300, false)]

        //
        //                        100
        //       first:            |
        //      second:            |
        //                        100
        //
        [InlineData(100, 100, 100, 100, false)]

        //
        //                        100
        //       first:            |
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(100, 100, 100, 200, false)]

        //
        //                        100
        //       first:            |
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(100, 100, 100, 300, false)]

        //
        //                        100
        //       first:            |
        //      second:                      |
        //                                  200
        //
        [InlineData(100, 100, 200, 200, false)]

        //
        //                        100
        //       first:            |
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(100, 100, 200, 300, false)]

        //
        //                        100
        //       first:            |
        //      second:                                |
        //                                            300
        //
        [InlineData(100, 100, 300, 300, false)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:  |
        //               0
        //
        [InlineData(100, 200, 0, 0, false)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:  [---------)
        //               0        100
        //
        [InlineData(100, 200, 0, 100, false)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(100, 200, 0, 200, true)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(100, 200, 0, 300, true)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:            |
        //                        100
        //
        [InlineData(100, 200, 100, 100, false)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(100, 200, 100, 200, true)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(100, 200, 100, 300, true)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:                      |
        //                                  200
        //
        [InlineData(100, 200, 200, 200, false)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(100, 200, 200, 300, false)]

        //
        //                        100       200
        //       first:            [---------)
        //      second:                                |
        //                                            300
        //
        [InlineData(100, 200, 300, 300, false)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:  |
        //               0
        //
        [InlineData(100, 300, 0, 0, false)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:  [---------)
        //               0        100
        //
        [InlineData(100, 300, 0, 100, false)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(100, 300, 0, 200, true)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(100, 300, 0, 300, true)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:            |
        //                        100
        //
        [InlineData(100, 300, 100, 100, false)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(100, 300, 100, 200, true)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(100, 300, 100, 300, true)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:                      |
        //                                  200
        //
        [InlineData(100, 300, 200, 200, false)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(100, 300, 200, 300, true)]

        //
        //                        100                 300
        //       first:            [-------------------)
        //      second:                                |
        //                                            300
        //
        [InlineData(100, 300, 300, 300, false)]

        //
        //                                  200
        //       first:                      |
        //      second:  |
        //               0
        //
        [InlineData(200, 200, 0, 0, false)]

        //
        //                                  200
        //       first:                      |
        //      second:  [---------)
        //               0        100
        //
        [InlineData(200, 200, 0, 100, false)]

        //
        //                                  200
        //       first:                      |
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(200, 200, 0, 200, false)]

        //
        //                                  200
        //       first:                      |
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(200, 200, 0, 300, false)]

        //
        //                                  200
        //       first:                      |
        //      second:            |
        //                        100
        //
        [InlineData(200, 200, 100, 100, false)]

        //
        //                                  200
        //       first:                      |
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(200, 200, 100, 200, false)]

        //
        //                                  200
        //       first:                      |
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(200, 200, 100, 300, false)]

        //
        //                                  200
        //       first:                      |
        //      second:                      |
        //                                  200
        //
        [InlineData(200, 200, 200, 200, false)]

        //
        //                                  200
        //       first:                      |
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(200, 200, 200, 300, false)]

        //
        //                                  200
        //       first:                      |
        //      second:                                |
        //                                            300
        //
        [InlineData(200, 200, 300, 300, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:  |
        //               0
        //
        [InlineData(200, 300, 0, 0, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:  [---------)
        //               0        100
        //
        [InlineData(200, 300, 0, 100, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(200, 300, 0, 200, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(200, 300, 0, 300, true)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:            |
        //                        100
        //
        [InlineData(200, 300, 100, 100, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(200, 300, 100, 200, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(200, 300, 100, 300, true)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:                      |
        //                                  200
        //
        [InlineData(200, 300, 200, 200, false)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(200, 300, 200, 300, true)]

        //
        //                                  200       300
        //       first:                      [---------)
        //      second:                                |
        //                                            300
        //
        [InlineData(200, 300, 300, 300, false)]

        //
        //                                            300
        //       first:                                |
        //      second:  |
        //               0
        //
        [InlineData(300, 300, 0, 0, false)]

        //
        //                                            300
        //       first:                                |
        //      second:  [---------)
        //               0        100
        //
        [InlineData(300, 300, 0, 100, false)]

        //
        //                                            300
        //       first:                                |
        //      second:  [-------------------)
        //               0                  200
        //
        [InlineData(300, 300, 0, 200, false)]

        //
        //                                            300
        //       first:                                |
        //      second:  [-----------------------------)
        //               0                            300
        //
        [InlineData(300, 300, 0, 300, false)]

        //
        //                                            300
        //       first:                                |
        //      second:            |
        //                        100
        //
        [InlineData(300, 300, 100, 100, false)]

        //
        //                                            300
        //       first:                                |
        //      second:            [---------)
        //                        100       200
        //
        [InlineData(300, 300, 100, 200, false)]

        //
        //                                            300
        //       first:                                |
        //      second:            [-------------------)
        //                        100                 300
        //
        [InlineData(300, 300, 100, 300, false)]

        //
        //                                            300
        //       first:                                |
        //      second:                      |
        //                                  200
        //
        [InlineData(300, 300, 200, 200, false)]

        //
        //                                            300
        //       first:                                |
        //      second:                      [---------)
        //                                  200       300
        //
        [InlineData(300, 300, 200, 300, false)]

        //
        //                                            300
        //       first:                                |
        //      second:                                |
        //                                            300
        //
        [InlineData(300, 300, 300, 300, false)]

        [Theory]
        public static void Overlap(int x1, int y1, int x2, int y2, bool expected)
        {
            ReadOnlySpan<int> a = new int[300];

            Assert.Equal(expected, a.Slice(x1, y1 - x1).Overlaps(a.Slice(x2, y2 - x2)));
            Assert.Equal(expected, a.Slice(x1, y1 - x1).Overlaps(a.Slice(x2, y2 - x2), out int elementOffset));
            Assert.Equal(expected ? x2 - x1 : 0, elementOffset);
        }
    }
}
