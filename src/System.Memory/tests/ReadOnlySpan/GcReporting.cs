// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static class ReadOnlySpanGcReportingTests
    {
        [Fact]
        public static void DelegateTest()
        {
            DelegateTest(100000, 10000);
        }

        public static void DelegateTest_Stress()
        {
            DelegateTest(100, 100);
        }

        private static void DelegateTest(int iterationCount, int objectCount)
        {
            object[] objects = new object[objectCount];
            Random rng = new Random();
            var delegateTestCore =
                new DelegateTestCoreDelegate(DelegateTest_Core) +
                new DelegateTestCoreDelegate(DelegateTest_Core);

            for (int i = 0; i < iterationCount; i++)
            {
                DelegateTest_CreateSomeObjects(objects, rng);

                var spanContainer = new SpanContainer();
                spanContainer._span = new ReadOnlySpan<int>(new int[] { 1, 2, 3 });
                delegateTestCore(spanContainer, objects, rng);
            }
        }

        private delegate void DelegateTestCoreDelegate(SpanContainer spanContainer, object[] objects, Random rng);

        private static void DelegateTest_Core(SpanContainer spanContainer, object[] objects, Random rng)
        {
            DelegateTest_CreateSomeObjects(objects, rng);

            int sum = 0;
            for (int i = 0; i < spanContainer._span.Length; ++i)
            {
                sum += spanContainer._span[i];
            }
            Assert.Equal(1 + 2 + 3, sum);
        }

        private static void DelegateTest_CreateSomeObjects(object[] objects, Random rng)
        {
            for (int i = 0; i < 100; ++i)
            {
                objects[rng.Next(objects.Length)] = new object();
            }
        }

        private struct SpanContainer
        {
            public ReadOnlySpan<int> _span;
        }
    }
}
