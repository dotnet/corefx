// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace EnumerableTests
{
    public class Directory_IEnumeratorTests : IClassFixture<TestFileSystemEntries>
    {
        private static TestFileSystemEntries s_fixture;

        public Directory_IEnumeratorTests(TestFileSystemEntries fixture)
        {
            s_fixture = fixture;
        }

        [Fact]
        public void TestIEnumerator()
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(s_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            IEnumerator<string> enumerator = dirs.GetEnumerator();

            try
            {
                Assert.True(enumerator.MoveNext(), "1: MoveNext returned false. Expected some contents");
                Assert.NotNull(enumerator.Current);
                Assert.Throws<NotSupportedException>(() => enumerator.Reset());
            }
            finally
            {
                // if we jump out while enumerating, the enumerator still holds the file handle
                enumerator.Dispose();
            }
        }

        // This testcase hits the FSEIterator's Clone method. Ensures that 2 different enumerators are returned and quick
        // functionality test.
        [Fact]
        public void TestClone()
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(s_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            IEnumerator<string> enumeratorA = dirs.GetEnumerator();

            enumeratorA.MoveNext();
            string da1 = enumeratorA.Current;
            enumeratorA.MoveNext();
            string da2 = enumeratorA.Current;

            IEnumerator<string> enumeratorB = dirs.GetEnumerator();
            enumeratorB.MoveNext();
            string db1 = enumeratorB.Current;
            enumeratorB.MoveNext();
            string db2 = enumeratorB.Current;

            try
            {
                Assert.Equal(da1, db1);
                Assert.Equal(da2, db2);
                Assert.NotEqual(da1, da2);
            }
            finally
            {
                // if we jump out while enumerating, the enumerator still holds the file handle
                enumeratorA.Dispose();
                enumeratorB.Dispose();
            }
        }
    }
}
