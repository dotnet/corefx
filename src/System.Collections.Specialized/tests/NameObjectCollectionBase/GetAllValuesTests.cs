// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Collections.Specialized.Tests
{
    public class GetAllValuesTests
    {
        private String _strErr = "Error!";

        [Fact]
        public void Test01()
        {
            MyNameObjectCollection noc;
            Object[] objarray;
            Foo[] fooarray;


            // [] GetAllValues() on empty collection
            noc = new MyNameObjectCollection();
            objarray = noc.GetAllValues();
            CheckObjArray(objarray, noc);
            // [] GetAllValues() on non-empty collection
            noc = new MyNameObjectCollection();
            for (int i = 0; i < 15; i++)
            {
                noc.Add("str_" + i.ToString(), new Foo());
            }
            objarray = noc.GetAllValues();
            CheckObjArray(objarray, noc);

            // [] GetAllValues(Type) on empty collection
            _strErr = "Err_003, ";
            noc = new MyNameObjectCollection();
            fooarray = (Foo[])noc.GetAllValues(typeof(Foo));
            CheckFooArray(fooarray, noc);

            // [] GetAllValues(Type) on non-empty collection
            noc = new MyNameObjectCollection();
            for (int i = 0; i < 15; i++)
            {
                noc.Add("str_" + i.ToString(), new Foo());
            }
            fooarray = (Foo[])noc.GetAllValues(typeof(Foo));
            CheckFooArray(fooarray, noc);

            // [] GetAllValues(Type) with incompatible type
            noc = new MyNameObjectCollection();
            for (int i = 0; i < 15; i++)
            {
                noc.Add("str_" + i.ToString(), new Foo());
            }

            Assert.Throws<ArrayTypeMismatchException>(() => { Array array = noc.GetAllValues(typeof(String)); });
        }

        [Fact]
        public static void TestGetAllValues_NullType_Throws()
        {
            var nameObjectCollection = new MyNameObjectCollection();
            Assert.Throws<ArgumentNullException>("type", () => nameObjectCollection.GetAllValues(null));
        }

        void CheckObjArray(Object[] array, MyNameObjectCollection expected)
        {
            if (array.Length != expected.Count)
            {
                Assert.False(true, string.Format(_strErr + "Array length != {0}.  Length == {1}", expected.Count, array.Length));
            }
            for (int i = 0; i < expected.Count; i++)
            {
                if ((Foo)(array[i]) != expected[i])
                {
                    Assert.False(true, string.Format("Value {0} is incorrect.  array[{0}]={1}, should be {2}", i, (Foo)(array[i]), expected[i]));
                }
            }
        }

        void CheckFooArray(Foo[] array, MyNameObjectCollection expected)
        {
            if (array.Length != expected.Count)
            {
                Assert.False(true, string.Format(_strErr + "Array length != {0}.  Length == {1}", expected.Count, array.Length));
            }
            for (int i = 0; i < expected.Count; i++)
            {
                if ((array[i]) != expected[i])
                {
                    Assert.False(true, string.Format("Value {0} is incorrect.  array[{0}]={1}, should be {2}", i, (array[i]), expected[i]));
                }
            }
        }
    }
}


