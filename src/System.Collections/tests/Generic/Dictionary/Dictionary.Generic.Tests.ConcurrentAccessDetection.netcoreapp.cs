// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Generic.Dictionary
{
    public class DictionaryConcurrentAccessDetectionTests
    {
        async Task DictionaryConcurrentAccessDetection<TKey, TValue>(Dictionary<TKey, TValue> dictionary, bool isValueType, object comparer, Action<Dictionary<TKey, TValue>> add, Action<Dictionary<TKey, TValue>> get, Action<Dictionary<TKey, TValue>> remove, Action<Dictionary<TKey, TValue>> removeOutParam)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                //break internal state
                FieldInfo entriesType = dictionary.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
                object entriesInstance = (Array)entriesType.GetValue(dictionary);
                Type field = entriesInstance.GetType().GetElementType();
                Array entryArray = (Array)Activator.CreateInstance(entriesInstance.GetType(), new object[] { ((IDictionary)dictionary).Count });                
                entriesType.SetValue(dictionary, entryArray);

                Assert.Equal(comparer, dictionary.GetType().GetField("_comparer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dictionary));
                Assert.Equal(isValueType, dictionary.GetType().GetGenericArguments()[0].IsValueType);
                Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => add(dictionary)).TargetSite.Name);
                Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => get(dictionary)).TargetSite.Name);
                //Remove is not resilient yet
                //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => remove(dictionary)).TargetSite.Name);
                //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => removeOutParam(dictionary)).TargetSite.Name);
            }, TaskCreationOptions.LongRunning);

            //Wait max 60 seconds, could loop forever
            Assert.True((await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(60))) == task) && task.IsCompletedSuccessfully);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(CustomEqualityComparerInt32ValueType))]
        public async Task DictionaryConcurrentAccessDetection_ValueTypeKey(Type comparerType)
        {
            IEqualityComparer<int> customComparer = null;

            Dictionary<int, int> dic = comparerType == null ?
                new Dictionary<int, int>() :
                new Dictionary<int, int>((customComparer = (IEqualityComparer<int>)Activator.CreateInstance(comparerType)));

            dic.Add(1, 1);

            await DictionaryConcurrentAccessDetection(dic,
                typeof(int).IsValueType,
                customComparer,
                d => d.Add(1, 1),
                d => { var v = d[1]; },
                d => d.Remove(1),
                d => d.Remove(1, out int value));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(CustomEqualityComparerDummyRefType))]
        public async Task DictionaryConcurrentAccessDetection_ReferenceTypeKey(Type comparerType)
        {
            IEqualityComparer<DummyRefType> customComparer = null;

            Dictionary<DummyRefType, DummyRefType> dic = comparerType == null ?
                new Dictionary<DummyRefType, DummyRefType>() :
                new Dictionary<DummyRefType, DummyRefType>((customComparer = (IEqualityComparer<DummyRefType>)Activator.CreateInstance(comparerType)));

            dic.Add(new DummyRefType() { Value = 1 }, new DummyRefType() { Value = 1 });

            await DictionaryConcurrentAccessDetection(dic,
                typeof(DummyRefType).IsValueType,
                customComparer,
                d => d.Add(new DummyRefType() { Value = 1 }, new DummyRefType() { Value = 1 }),
                d => { var v = d[new DummyRefType() { Value = 1 }]; },
                d => d.Remove(new DummyRefType() { Value = 1 }),
                d => d.Remove(new DummyRefType() { Value = 1 }, out DummyRefType value));
        }
    }

    class DummyRefType
    {
        public int Value { get; set; }
        public override bool Equals(object obj)
        {
            return ((DummyRefType)obj).Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    class CustomEqualityComparerDummyRefType : EqualityComparer<DummyRefType>
    {
        public override bool Equals(DummyRefType x, DummyRefType y)
        {
            return x.Value == y.Value;
        }

        public override int GetHashCode(DummyRefType obj)
        {
            return obj.GetHashCode();
        }
    }

    class CustomEqualityComparerInt32ValueType : EqualityComparer<int>
    {
        public override bool Equals(int x, int y)
        {
            return EqualityComparer<int>.Default.Equals(x, y);
        }

        public override int GetHashCode(int obj)
        {
            return EqualityComparer<int>.Default.GetHashCode(obj);
        }
    }
}
