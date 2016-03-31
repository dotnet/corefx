// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class LiftedNullableTests
    {
        #region Test methods

        [Fact]
        public static void CheckLiftedNullableBoolAndTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolAnd(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolAndAlsoTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolAndAlso(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolOrTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolOr(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolOrElseTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolOrElse(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolAndWithMethodTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolWithMethodAnd(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolAndAlsoWithMethodTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolWithMethodAndAlso(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolWithMethodOrTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolWithMethodOr(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void CheckLiftedNullableBoolWithMethodOrElseTest()
        {
            bool?[] values = new bool?[] { null, true, false };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifyNullableBoolWithMethodOrElse(values[i], values[j]);
                }
            }
        }

        #endregion

        #region Test verifiers

        private static void VerifyNullableBoolAnd(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.And(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a & b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolAndAlso(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.AndAlso(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a == false ? false : a & b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolOr(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.Or(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a | b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolOrElse(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.OrElse(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a == true ? true : a | b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolWithMethodAnd(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.And(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a & b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolWithMethodAndAlso(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.AndAlso(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a == false ? false : a & b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolWithMethodOr(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.Or(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a | b;

            Assert.Equal(expected, f());
        }

        private static void VerifyNullableBoolWithMethodOrElse(bool? a, bool? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(bool), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(bool), "p1");

            Expression<Func<bool?>> e =
                Expression.Lambda<Func<bool?>>(
                    Expression.OrElse(
                        Expression.Constant(a, typeof(bool?)),
                        Expression.Constant(b, typeof(bool?)),
                        null),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool?> f = e.Compile();

            bool? expected = a == true ? true : a | b;

            Assert.Equal(expected, f());
        }

        #endregion
    }
}
