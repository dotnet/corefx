// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Xunit;

namespace System.Collections.Tests
{
    public class SortedList_GetValueListTests
    {
        [Fact]
        public void TestGetValueListBasic()
        {
            StringBuilder sblMsg = new StringBuilder(99);

            SortedList sl2 = null;
            IEnumerator en = null;
            StringBuilder sbl3 = new StringBuilder(99);
            StringBuilder sbl4 = new StringBuilder(99);
            StringBuilder sblWork1 = new StringBuilder(99);

            int i3 = 0;
            int i = 0;
            int j = 0;
            //
            // Constructor: Create SortedList using this as IComparer and default settings.
            //
            sl2 = new SortedList();

            // Verify that the SortedList is not null.
            Assert.NotNull(sl2);

            // Verify that the SortedList is empty.
            Assert.Equal(0, sl2.Count);

            // Testcase: Set - null key, ArgExc expected
            Assert.Throws<ArgumentNullException>(() =>
                {
                    sl2[null] = 0;
                });

            Assert.Equal(0, sl2.Count);

            // Testcase: Set - null val
            sl2[(object)100] = (object)null;
            Assert.Equal(1, sl2.Count);

            // Testcase: vanila Set
            sl2[(object)100] = 1;
            Assert.Equal(1, sl2.Count);
            sl2.Clear();
            Assert.Equal(0, sl2.Count);

            // Testcase: add key-val pairs
            for (i = 0; i < 100; i++)
            {
                sl2.Add(i + 100, i);
            }
            Assert.Equal(100, sl2.Count);

            for (i = 0; i < 100; i++)
            {
                j = i + 100;
                Assert.True(sl2.ContainsKey((int)j));
                Assert.True(sl2.ContainsValue(i));

                object o2 = sl2[(int)j];

                Assert.NotNull(o2);
                Assert.True(o2.Equals(i), "Error, entry for key " + j.ToString() + " is " + o2.ToString() + " but should have been " + i.ToString());
            } // FOR

            //  testcase: GetValueList
            // ICollection.GetEnumerator() first test the boundaries on the Remove method thru GetEnumerator implementation
            en = (IEnumerator)sl2.GetValueList().GetEnumerator();

            // Boundary for Current
            Assert.Throws<InvalidOperationException>(() =>
                    {
                        object throwaway = en.Current;
                    }
            );

            j = 0;
            // go over the enumarator
            en = (IEnumerator)sl2.GetValueList().GetEnumerator();
            while (en.MoveNext())
            {
                // Current to see the order
                i3 = (int)en.Current;
                Assert.Equal(i3, j);

                // GetObject again to see the same order
                i3 = (int)en.Current;
                Assert.Equal(i3, j);

                j++;
            }

            // Boundary for GetObject
            Assert.Throws<InvalidOperationException>(() =>
                    {
                        object throwawayobj = en.Current;
                    }
            );

            // Boundary for MoveNext: call MoveNext to make sure it returns false
            Assert.False((en.MoveNext()) || (j != 100));
            // call again MoveNext to make sure it still returns false
            Assert.False(en.MoveNext());
            Assert.Equal(100, sl2.Count);

            // now modify the sortedlist while enumerator is still active
            en = (IEnumerator)sl2.GetKeyList().GetEnumerator(); //can remove an item thru en
            en.MoveNext();

            sl2[1] = 0;  // Set (int index, object val) // this works fine

            // Boundary for MoveNext
            Assert.Throws<InvalidOperationException>(() =>
                {
                    en.MoveNext();
                });
        }
    }
}
