// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using SortedList_SortedListUtils;

namespace SortedListClear
{
    public class Driver<K, V> where K : IComparableValue
    {
        private Test m_test;

        public Driver(Test test)
        {
            m_test = test;
        }

        public void Clear(K[] keys, V[] values, int repeat)
        {
            SortedList<K, V> tbl = new SortedList<K, V>(new ValueKeyComparer<K>());

            tbl.Clear();
            m_test.Eval(tbl.Count == 0);

            for (int i = 0; i < keys.Length; i++)
            {
                tbl.Add(keys[i], values[i]);
            }
            for (int i = 0; i < repeat; i++)
            {
                tbl.Clear();
                m_test.Eval(tbl.Count == 0);
            }
        }

        public void NonGenericIDictionaryClear(K[] keys, V[] values, int repeat)
        {
            SortedList<K, V> tbl = new SortedList<K, V>(new ValueKeyComparer<K>());
            IDictionary _idic = tbl;

            _idic.Clear();
            m_test.Eval(tbl.Count == 0);

            for (int i = 0; i < keys.Length; i++)
            {
                tbl.Add(keys[i], values[i]);
            }
            for (int i = 0; i < repeat; i++)
            {
                _idic.Clear();
                m_test.Eval(tbl.Count == 0);
            }
        }
    }

    public class Clear
    {
        [Fact]
        public static void ClearMain()
        {
            Test test = new Test();

            Driver<RefX1<int>, ValX1<string>> IntDriver = new Driver<RefX1<int>, ValX1<string>>(test);
            RefX1<int>[] intArr = new RefX1<int>[100];
            for (int i = 0; i < 100; i++)
            {
                intArr[i] = new RefX1<int>(i);
            }

            Driver<ValX1<string>, RefX1<int>> StringDriver = new Driver<ValX1<string>, RefX1<int>>(test);
            ValX1<string>[] stringArr = new ValX1<string>[100];
            for (int i = 0; i < 100; i++)
            {
                stringArr[i] = new ValX1<string>("SomeTestString" + i.ToString());
            }

            IntDriver.Clear(intArr, stringArr, 1);
            IntDriver.Clear(intArr, stringArr, 10);
            IntDriver.Clear(new RefX1<int>[] { }, new ValX1<string>[] { }, 1);
            IntDriver.Clear(new RefX1<int>[] { }, new ValX1<string>[] { }, 10);


            StringDriver.Clear(stringArr, intArr, 1);
            StringDriver.Clear(stringArr, intArr, 10);
            StringDriver.Clear(new ValX1<string>[] { }, new RefX1<int>[] { }, 1);
            StringDriver.Clear(new ValX1<string>[] { }, new RefX1<int>[] { }, 10);

            IntDriver.NonGenericIDictionaryClear(intArr, stringArr, 1);
            IntDriver.NonGenericIDictionaryClear(intArr, stringArr, 10);
            IntDriver.NonGenericIDictionaryClear(new RefX1<int>[] { }, new ValX1<string>[] { }, 1);
            IntDriver.NonGenericIDictionaryClear(new RefX1<int>[] { }, new ValX1<string>[] { }, 10);


            StringDriver.NonGenericIDictionaryClear(stringArr, intArr, 1);
            StringDriver.NonGenericIDictionaryClear(stringArr, intArr, 10);
            StringDriver.NonGenericIDictionaryClear(new ValX1<string>[] { }, new RefX1<int>[] { }, 1);
            StringDriver.NonGenericIDictionaryClear(new ValX1<string>[] { }, new RefX1<int>[] { }, 10);

            Assert.True(test.result);
        }
    }
}
