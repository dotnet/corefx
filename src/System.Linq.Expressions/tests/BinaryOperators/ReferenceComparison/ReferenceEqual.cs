// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class ReferenceEqual : ReferenceEqualityTests
    {
        [Theory]
        [MemberData(nameof(ReferenceObjectsData))]
        public void TrueOnSame(object item)
        {
            Expression exp = Expression.ReferenceEqual(
                    Expression.Constant(item, item.GetType()),
                    Expression.Constant(item, item.GetType())
                );
            Assert.True(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(ReferenceTypesData))]
        public void TrueOnBothNull(Type type)
        {
            Expression exp = Expression.ReferenceEqual(
                Expression.Constant(null, type),
                Expression.Constant(null, type)
                );
            Assert.True(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(ReferenceObjectsData))]
        public void FalseIfLeftNull(object item)
        {
            Expression exp = Expression.ReferenceEqual(
                    Expression.Constant(null, item.GetType()),
                    Expression.Constant(item, item.GetType())
                );
            Assert.False(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(ReferenceObjectsData))]
        public void FalseIfRightNull(object item)
        {
            Expression exp = Expression.ReferenceEqual(
                    Expression.Constant(item, item.GetType()),
                    Expression.Constant(null, item.GetType())
                );
            Assert.False(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(DifferentObjects))]
        public void FalseIfDifferentObjectsAsObject(object x, object y)
        {
            Expression exp = Expression.ReferenceEqual(
                    Expression.Constant(x, typeof(object)),
                    Expression.Constant(y, typeof(object))
                );
            Assert.False(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(DifferentObjects))]
        public void FalseIfDifferentObjectsOwnType(object x, object y)
        {
            Expression exp = Expression.ReferenceEqual(
                    Expression.Constant(x),
                    Expression.Constant(y)
                );
            Assert.False(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(LeftValueType))]
        [MemberData(nameof(RightValueType))]
        [MemberData(nameof(BothValueType))]
        public void ThrowsOnValueTypeArguments(object x, object y)
        {
            Expression xExp = Expression.Constant(x);
            Expression yExp = Expression.Constant(y);
            Assert.Throws<InvalidOperationException>(() => Expression.ReferenceEqual(xExp, yExp));
        }

        [Theory]
        [MemberData(nameof(UnassignablePairs))]
        public void ThrowsOnUnassignablePairs(object x, object y)
        {
            Expression xExp = Expression.Constant(x);
            Expression yExp = Expression.Constant(y);
            Assert.Throws<InvalidOperationException>(() => Expression.ReferenceEqual(xExp, yExp));
        }

        [Theory]
        [MemberData(nameof(ComparableValuesData))]
        public void TrueOnSameViaInterface(object item)
        {
            Expression exp = Expression.ReferenceEqual(
                Expression.Constant(item, typeof(IComparable)),
                Expression.Constant(item, typeof(IComparable))
            );
            Assert.True(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(DifferentComparableValues))]
        public void FalseOnDifferentViaInterface(object x, object y)
        {
            Expression exp = Expression.ReferenceEqual(
                Expression.Constant(x, typeof(IComparable)),
                Expression.Constant(y, typeof(IComparable))
            );
            Assert.False(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(ComparableReferenceTypesData))]
        public void TrueOnSameLeftViaInterface(object item)
        {
            Expression exp = Expression.ReferenceEqual(
                Expression.Constant(item, typeof(IComparable)),
                Expression.Constant(item)
            );
            Assert.True(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Theory]
        [MemberData(nameof(ComparableReferenceTypesData))]
        public void TrueOnSameRightViaInterface(object item)
        {
            Expression exp = Expression.ReferenceEqual(
                Expression.Constant(item),
                Expression.Constant(item, typeof(IComparable))
            );
            Assert.True(Expression.Lambda<Func<bool>>(exp).Compile()());
        }

        [Fact]
        public void CannotReduce()
        {
            Expression exp = Expression.ReferenceEqual(Expression.Constant(""), Expression.Constant(""));
            Assert.False(exp.CanReduce);
            Assert.Same(exp, exp.Reduce());
            Assert.Throws<ArgumentException>(null, () => exp.ReduceAndCheck());
        }

        [Fact]
        public void ThrowsOnLeftNull()
        {
            Assert.Throws<ArgumentNullException>("left", () => Expression.ReferenceEqual(null, Expression.Constant("")));
        }

        [Fact]
        public void ThrowsOnRightNull()
        {
            Assert.Throws<ArgumentNullException>("right", () => Expression.ReferenceEqual(Expression.Constant(""), null));
        }

        [Fact]
        public static void ThrowsOnLeftUnreadable()
        {
            Expression value = Expression.Property(null, typeof(Unreadable<string>), "WriteOnly");
            Assert.Throws<ArgumentException>("left", () => Expression.ReferenceEqual(value, Expression.Constant("")));
        }

        [Fact]
        public static void ThrowsOnRightUnreadable()
        {
            Expression value = Expression.Property(null, typeof(Unreadable<string>), "WriteOnly");
            Assert.Throws<ArgumentException>("right", () => Expression.ReferenceEqual(Expression.Constant(""), value));
        }
    }
}
