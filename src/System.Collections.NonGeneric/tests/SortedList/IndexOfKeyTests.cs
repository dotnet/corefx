// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Xunit;

namespace System.Collections.Tests
{
    public class SortedList_IndexOfKeyTests : IComparer
    {
        public virtual int Compare(object obj1, object obj2)  // ICompare satisfier.
        {
            return string.Compare(obj1.ToString(), obj2.ToString());
        }

        [Fact]
        public void TestIndexOfKeyBasic()
        {
            StringBuilder sblMsg = new StringBuilder(99);
            //

            SortedList sl2 = null;

            StringBuilder sbl3 = new StringBuilder(99);
            StringBuilder sbl4 = new StringBuilder(99);
            StringBuilder sblWork1 = new StringBuilder(99);

            string s1 = null;
            string s2 = null;
            string s3 = null;
            string s4 = null;

            int i = 0;
            int j = 0;

            //
            // 	Constructor: Create SortedList using this as IComparer and default settings.
            //
            sl2 = new SortedList(this);

            //  Verify that the SortedList is not null.
            Assert.NotNull(sl2);

            //  Verify that the SortedList is empty.
            Assert.Equal(0, sl2.Count);

            //  boundary
            // null key
            Assert.Throws<ArgumentNullException>(() =>
                             {
                                 j = sl2.IndexOfKey((string)null);
                             }
            );

            // invalid key
            j = sl2.IndexOfKey("No_Such_Key");
            Assert.Equal(-1, j);

            //   Testcase: add few key-val pairs
            for (i = 0; i < 100; i++)
            {
                sblMsg.Length = 0;
                sblMsg.Append("key_");
                sblMsg.Append(i);
                s1 = sblMsg.ToString();

                sblMsg.Length = 0;
                sblMsg.Append("val_");
                sblMsg.Append(i);
                s2 = sblMsg.ToString();

                sl2.Add(s1, s2);
            }

            //
            //   Testcase: test IndexOfKey
            //
            for (i = 0; i < sl2.Count; i++)
            {
                sblMsg.Length = 0;
                sblMsg.Append("key_"); //key
                sblMsg.Append(i);
                s1 = sblMsg.ToString();

                sblMsg.Length = 0;
                sblMsg.Append("val_"); //value
                sblMsg.Append(i);
                s2 = sblMsg.ToString();

                //  now use IndexOfKey and IndexOfVal to obtain the same object through GetByIndex and check
                s3 = (string)sl2.GetByIndex(sl2.IndexOfKey(s1)); //Get the index of this key and then get object
                s4 = (string)sl2.GetByIndex(sl2.IndexOfValue(s2)); //Get the index of this val and then get object
                Assert.True(s3.Equals(s4));

                //  now Get using the index obtained thru IndexOfKey () and compare it with s2
                s3 = (string)sl2.GetByIndex(sl2.IndexOfKey(s1));
                Assert.True(s3.Equals(s2));
            }

            //
            // Remove a key and then check
            //
            sblMsg.Length = 0;
            sblMsg.Append("key_50");
            s1 = sblMsg.ToString();

            sl2.Remove(s1); //removes "Key_50"
            j = sl2.IndexOfKey(s1);

            Assert.Equal(-1, j);
        }
    }
}
