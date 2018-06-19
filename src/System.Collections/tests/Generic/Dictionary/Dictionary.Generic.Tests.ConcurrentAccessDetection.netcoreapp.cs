// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.System;
using Xunit;

namespace Generic.Dictionary
{
    public class DictionaryConcurrentAccessDetectionTests
    {
        [Fact]
        public static void Add_DictionaryConcurrentAccessDetection_NullComparer_ValueTypeKey()
        {
            Thread customThread = new Thread(() =>
            {
                Dictionary<int, int> dic = new Dictionary<int, int>();
                dic.Add(1, 1);

                //break internal state
                var entriesType = dic.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
                var entriesInstance = (Array)entriesType.GetValue(dic);
                var field = entriesInstance.GetType().GetElementType();
                var entryArray = (Array)Activator.CreateInstance(entriesInstance.GetType(), new object[] { dic.Count });
                var entry = Activator.CreateInstance(field);
                entriesType.SetValue(dic, entryArray);

                Assert.Null(dic.GetType().GetField("_comparer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dic));
                Assert.True(dic.GetType().GetGenericArguments()[0].IsValueType);
                Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Add(1, 1)).TargetSite.Name);
                Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic[1]).TargetSite.Name);
                //Remove is not resilient yet
                //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(1)).TargetSite.Name);
                //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(1, out int value)).TargetSite.Name);
            });
            customThread.IsBackground = true;
            customThread.Start();

            //Wait max 5 seconds, could loop forever
            Assert.True(customThread.Join(TimeSpan.FromSeconds(5)));            
        }

        [Fact]
        public static void Add_DictionaryConcurrentAccessDetection_Comparer_ValueTypeKey()
        {
            Thread customThread = new Thread(() =>
             {
                 Dictionary<int, int> dic = new Dictionary<int, int>(new CustomEqualityComparerInt32ValueType());
                 dic.Add(1, 1);

                 //break internal state
                 var entriesType = dic.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
                 var entriesInstance = (Array)entriesType.GetValue(dic);
                 var field = entriesInstance.GetType().GetElementType();
                 var entryArray = (Array)Activator.CreateInstance(entriesInstance.GetType(), new object[] { dic.Count });
                 var entry = Activator.CreateInstance(field);
                 entriesType.SetValue(dic, entryArray);

                 Assert.NotNull(dic.GetType().GetField("_comparer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dic));
                 Assert.True(dic.GetType().GetGenericArguments()[0].IsValueType);
                 Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Add(1, 1)).TargetSite.Name);
                 Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic[1]).TargetSite.Name);
                 //Remove is not resilient yet
                 //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(1)).TargetSite.Name);
                 //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(1, out int value)).TargetSite.Name);
             });
            customThread.IsBackground = true;
            customThread.Start();

            //Wait max 5 seconds, could loop forever
            Assert.True(customThread.Join(TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public static void Add_DictionaryConcurrentAccessDetection_NullComparer_ReferenceTypeKey()
        {
            Thread customThread = new Thread(() =>
            {
                Dictionary<DummyRefType, DummyRefType> dic = new Dictionary<DummyRefType, DummyRefType>();
                dic.Add(new DummyRefType() { Value = 1 }, new DummyRefType() { Value = 1 });

                //break internal state
                var entriesType = dic.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
                var entriesInstance = (Array)entriesType.GetValue(dic);
                var field = entriesInstance.GetType().GetElementType();
                var entryArray = (Array)Activator.CreateInstance(entriesInstance.GetType(), new object[] { dic.Count });
                var entry = Activator.CreateInstance(field);
                entriesType.SetValue(dic, entryArray);

                Assert.Null(dic.GetType().GetField("_comparer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dic));
                Assert.False(dic.GetType().GetGenericArguments()[0].IsValueType);
                Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Add(new DummyRefType() { Value = 1 }, new DummyRefType() { Value = 1 })).TargetSite.Name);
                Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic[new DummyRefType() { Value = 1 }]).TargetSite.Name);
                //Remove is not resilient yet
                //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(new DummyRefType() { Value = 1 })).TargetSite.Name);
                //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(new DummyRefType() { Value = 1 }, out DummyRefType value)).TargetSite.Name);
            });
            customThread.IsBackground = true;
            customThread.Start();

            //Wait max 5 seconds, could loop forever
            Assert.True(customThread.Join(TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public static void Add_DictionaryConcurrentAccessDetection_Comparer_ReferenceTypeKey()
        {
            Thread customThread = new Thread(() =>
             {
                 Dictionary<DummyRefType, DummyRefType> dic = new Dictionary<DummyRefType, DummyRefType>(new CustomEqualityComparerDummyRefType());
                 dic.Add(new DummyRefType() { Value = 1 }, new DummyRefType() { Value = 1 });

                 //break internal state
                 var entriesType = dic.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
                 var entriesInstance = (Array)entriesType.GetValue(dic);
                 var field = entriesInstance.GetType().GetElementType();
                 var entryArray = (Array)Activator.CreateInstance(entriesInstance.GetType(), new object[] { dic.Count });
                 var entry = Activator.CreateInstance(field);
                 entriesType.SetValue(dic, entryArray);

                 Assert.NotNull(dic.GetType().GetField("_comparer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dic));
                 Assert.False(dic.GetType().GetGenericArguments()[0].IsValueType);
                 Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Add(new DummyRefType() { Value = 1 }, new DummyRefType() { Value = 1 })).TargetSite.Name);
                 Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic[new DummyRefType() { Value = 1 }]).TargetSite.Name);
                 //Remove is not resilient yet
                 //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(new DummyRefType() { Value = 1 })).TargetSite.Name);
                 //Assert.Equal("ThrowInvalidOperationException_ConcurrentOperationsNotSupported", Assert.Throws<InvalidOperationException>(() => dic.Remove(new DummyRefType() { Value = 1 }, out DummyRefType value)).TargetSite.Name);
             });
            customThread.IsBackground = true;
            customThread.Start();

            //Wait max 5 seconds, could loop forever
            Assert.True(customThread.Join(TimeSpan.FromSeconds(5)));
        }
    }

    public class DummyRefType
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

    public class CustomEqualityComparerDummyRefType : EqualityComparer<DummyRefType>
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

    public class CustomEqualityComparerInt32ValueType : EqualityComparer<int>
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

