// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Tests
{
    public class SingletonTests
    {
        [Fact]
        public void Singleton_ProduceOneElement()
        {
            var singleton = Enumerable.Singleton(1);
            int count = 0;
            foreach (var val in singleton)
            {
                count++;
                Assert.Equal(1, val);
            }

            Assert.Equal(1, count);
        }

        [Fact]
        public void Singleton_ToArray_ProduceCorrectResult()
        {
            var array = Enumerable.Singleton(1).ToArray();
            Assert.Equal(array.Length, 1);
            for (var i = 0; i < array.Length; i++)
                Assert.Equal(1, array[i]);
        }

        [Fact]
        public void Singleton_ProduceSameObject()
        {
            object objectInstance = new object();
            var array = Enumerable.Singleton(objectInstance).ToArray();
            Assert.Equal(array.Length, 1);
            for (var i = 0; i < array.Length; i++)
                Assert.Same(objectInstance, array[i]);
        }

        [Fact]
        public void Singleton_WorkWithNullElement()
        {
            object objectInstance = null;
            var array = Enumerable.Singleton(objectInstance).ToArray();
            Assert.Equal(array.Length, 1);
            for (var i = 0; i < array.Length; i++)
                Assert.Null(array[i]);
        }

        [Fact]
        public void Singleton_NotEnumerateAfterEnd()
        {
            using (var enumerator = Enumerable.Singleton(1).GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.False(enumerator.MoveNext());
                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact]
        public void Singleton_EnumerableAndEnumeratorAreSame()
        {
            var enumerable = Enumerable.Singleton(1);
            using (var enumerator = enumerable.GetEnumerator())
            {
                Assert.Same(enumerable, enumerator);
            }
        }

        [Fact]
        public void Singleton_GetEnumeratorReturnUniqueInstances()
        {
            var singleton = Enumerable.Singleton(1);
            using (var enum1 = singleton.GetEnumerator())
            using (var enum2 = singleton.GetEnumerator())
            {
                Assert.NotSame(enum1, enum2);
            }
        }

        [Fact]
        public void SameResultsRepeatedCallsIntQuery()
        {
            Assert.Equal(Enumerable.Singleton(-3), Enumerable.Singleton(-3));
        }

        [Fact]
        public void SameResultsRepeatedCallsStringQuery()
        {
            Assert.Equal(Enumerable.Singleton("SSS"), Enumerable.Singleton("SSS"));
        }
    }
}
