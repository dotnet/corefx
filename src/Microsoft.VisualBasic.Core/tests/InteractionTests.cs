﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.VisualBasic.Tests
{
    public class InteractionTests
    {
        [Theory]
        [MemberData(nameof(CallByName_TestData))]
        public void CallByName(object instance, string methodName, CallType useCallType, object[] args, Func<object, object> getResult, object expected)
        {
            Assert.Equal(getResult is null ? expected : null, Interaction.CallByName(instance, methodName, useCallType, args));
            if (getResult != null)
            {
                Assert.Equal(expected, getResult(instance));
            }
        }

        [Theory]
        [MemberData(nameof(CallByName_ArgumentException_TestData))]
        public void CallByName_ArgumentException(object instance, string methodName, CallType useCallType, object[] args)
        {
            Assert.Throws<ArgumentException>(() => Interaction.CallByName(instance, methodName, useCallType, args));
        }

        [Theory]
        [MemberData(nameof(CallByName_MissingMemberException_TestData))]
        public void CallByName_MissingMemberException(object instance, string methodName, CallType useCallType, object[] args)
        {
            Assert.Throws<MissingMemberException>(() => Interaction.CallByName(instance, methodName, useCallType, args));
        }

        private static IEnumerable<object[]> CallByName_TestData()
        {
            yield return new object[] { new Class(), "Method", CallType.Method, new object[] { 1, 2 }, null, 3 };
            yield return new object[] { new Class(), "Method", CallType.Get, new object[] { 2, 3 }, null, 5 };
            yield return new object[] { new Class(), "P", CallType.Get, new object[0], null, 0 };
            yield return new object[] { new Class(), "Item", CallType.Get, new object[] { 2 }, null, 2 };
            yield return new object[] { new Class(), "P", CallType.Set, new object[] { 3 }, new Func<object, object>(obj => ((Class)obj).Value), 3 };
            yield return new object[] { new Class(), "Item", CallType.Let, new object[] { 4, 5 }, new Func<object, object>(obj => ((Class)obj).Value), 9 };
        }

        private static IEnumerable<object[]> CallByName_ArgumentException_TestData()
        {
            yield return new object[] { null, null, default(CallType), new object[0] };
            yield return new object[] { new Class(), "Method", default(CallType), new object[] { 1, 2 } };
            yield return new object[] { new Class(), "Method", (CallType)int.MaxValue, new object[] { 1, 2 } };
        }

        private static IEnumerable<object[]> CallByName_MissingMemberException_TestData()
        {
            yield return new object[] { new Class(), "Method", CallType.Method, new object[0] };
            yield return new object[] { new Class(), "Q", CallType.Get, new object[0] };
        }

        private sealed class Class
        {
            public int Value;
            public int Method(int x, int y) => x + y;
            public int P
            {
                get { return Value; }
                set { Value = value; }
            }
            public object this[object index]
            {
                get { return Value + (int)index; }
                set { Value = (int)value + (int)index; }
            }
        }


        [Fact]
        public void Choose()
        {
            object[] x = { "Choise1", "Choise2", "Choise3", "Choise4", "Choise5", "Choise6" };
            Assert.Equal(null, Interaction.Choose(5));
            Assert.Equal(null, Interaction.Choose(0, x)); // < 1
            Assert.Equal(null, Interaction.Choose(x.Length + 1, x)); // > UpperBound
            Assert.Equal(2, Interaction.Choose(2, 1, 2, 3));
            Assert.Equal("Choise3", Interaction.Choose(3, x[0], x[1], x[2]));
            for (int i = 1; i <= x.Length; i++)
            {
                Assert.Equal(x[i - 1], Interaction.Choose(i, x));
            }
        }

        [Fact]
        public void CreateObject()
        {
            Assert.Throws<NullReferenceException>(() => Interaction.CreateObject(null));
            Assert.Throws<Exception>(() => Interaction.CreateObject(""));
            // Not tested: valid ProgID.
        }

        [Theory]
        [MemberData(nameof(IIf_TestData))]
        public void IIf(bool expression, object truePart, object falsePart, object expected)
        {
            Assert.Equal(expected, Interaction.IIf(expression, truePart, falsePart));
        }

        private static IEnumerable<object[]> IIf_TestData()
        {
            yield return new object[] { false, 1, null, null };
            yield return new object[] { true, 1, null, 1 };
            yield return new object[] { false, null, 2, 2 };
            yield return new object[] { true, null, 2, null };
            yield return new object[] { false, 3, "str", "str" };
            yield return new object[] { true, 3, "str", 3 };
        }

        [Theory]
        [InlineData(0, 1, 100, 10, "   :  0")]
        [InlineData(1, 1, 100, 10, "  1: 10")]
        [InlineData(15, 1, 100, 10, " 11: 20")]
        [InlineData(25, 1, 100, 10, " 21: 30")]
        [InlineData(35, 1, 100, 10, " 31: 40")]
        [InlineData(45, 1, 100, 10, " 41: 50")]
        [InlineData(5001, 1, 10000, 100, " 5001: 5100")]
        public void Partition(long Number, long Start, long Stop, long Interval, string expected)
        {
            Assert.Equal(expected, Interaction.Partition(Number, Start, Stop, Interval));
        }

        [Theory]
        [InlineData(0, -1, 100, 10)] // Start < 0
        [InlineData(0, 100, 100, 10)] // Stop <= Start
        [InlineData(0, 1, 100, 0)] // Interval < 1
        public void Partition_Invalid(long Number, long Start, long Stop, long Interval)
        {
            Assert.Throws<ArgumentException>(() => Interaction.Partition(Number, Start, Stop, Interval));
        }
    }
}
