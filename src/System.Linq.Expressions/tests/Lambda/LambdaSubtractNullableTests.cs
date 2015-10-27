﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Tests.ExpressionCompiler.Lambda
{
    public static unsafe class LambdaSubtractNullableTests
    {
        #region Test methods

        [Fact]
        public static void LambdaSubtractNullableDecimalTest()
        {
            decimal?[] values = new decimal?[] { null, decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableDecimal(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableDoubleTest()
        {
            double?[] values = new double?[] { null, 0, 1, -1, double.MinValue, double.MaxValue, double.Epsilon, double.NegativeInfinity, double.PositiveInfinity, double.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableDouble(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableFloatTest()
        {
            float?[] values = new float?[] { null, 0, 1, -1, float.MinValue, float.MaxValue, float.Epsilon, float.NegativeInfinity, float.PositiveInfinity, float.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableFloat(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableIntTest()
        {
            int?[] values = new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableInt(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableLongTest()
        {
            long?[] values = new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableLong(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableShortTest()
        {
            short?[] values = new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableShort(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableUIntTest()
        {
            uint?[] values = new uint?[] { null, 0, 1, uint.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableUInt(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableULongTest()
        {
            ulong?[] values = new ulong?[] { null, 0, 1, ulong.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableULong(values[i], values[j]);
                }
            }
        }

        [Fact]
        public static void LambdaSubtractNullableUShortTest()
        {
            ushort?[] values = new ushort?[] { null, 0, 1, ushort.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    VerifySubtractNullableUShort(values[i], values[j]);
                }
            }
        }

        #endregion

        #region Test verifiers

        #region Verify decimal?

        private static void VerifySubtractNullableDecimal(decimal? a, decimal? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(decimal?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(decimal?), "p1");

            // verify with parameters supplied
            Expression<Func<decimal?>> e1 =
                Expression.Lambda<Func<decimal?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<decimal?, decimal?, decimal?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(decimal?)),
                    Expression.Constant(b, typeof(decimal?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal?> f1 = e1.CompileForTest();

            decimal? f1Result = default(decimal?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<decimal?, decimal?, Func<decimal?>>> e2 =
                Expression.Lambda<Func<decimal?, decimal?, Func<decimal?>>>(
                    Expression.Lambda<Func<decimal?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<decimal?, decimal?, Func<decimal?>> f2 = e2.CompileForTest();

            decimal? f2Result = default(decimal?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<decimal?, decimal?, decimal?>>> e3 =
                Expression.Lambda<Func<Func<decimal?, decimal?, decimal?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<decimal?, decimal?, decimal?>>>(
                            Expression.Lambda<Func<decimal?, decimal?, decimal?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal?, decimal?, decimal?> f3 = e3.CompileForTest()();

            decimal? f3Result = default(decimal?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<decimal?, decimal?, decimal?>>> e4 =
                Expression.Lambda<Func<Func<decimal?, decimal?, decimal?>>>(
                    Expression.Lambda<Func<decimal?, decimal?, decimal?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<decimal?, decimal?, decimal?>> f4 = e4.CompileForTest();

            decimal? f4Result = default(decimal?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<decimal?, Func<decimal?, decimal?>>> e5 =
                Expression.Lambda<Func<decimal?, Func<decimal?, decimal?>>>(
                    Expression.Lambda<Func<decimal?, decimal?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<decimal?, Func<decimal?, decimal?>> f5 = e5.CompileForTest();

            decimal? f5Result = default(decimal?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<decimal?, decimal?>>> e6 =
                Expression.Lambda<Func<Func<decimal?, decimal?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<decimal?, Func<decimal?, decimal?>>>(
                            Expression.Lambda<Func<decimal?, decimal?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(decimal?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal?, decimal?> f6 = e6.CompileForTest()();

            decimal? f6Result = default(decimal?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            decimal? csResult = default(decimal?);
            Exception csEx = null;
            try
            {
                csResult = (decimal?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify double?

        private static void VerifySubtractNullableDouble(double? a, double? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(double?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(double?), "p1");

            // verify with parameters supplied
            Expression<Func<double?>> e1 =
                Expression.Lambda<Func<double?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<double?, double?, double?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(double?)),
                    Expression.Constant(b, typeof(double?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<double?> f1 = e1.CompileForTest();

            double? f1Result = default(double?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<double?, double?, Func<double?>>> e2 =
                Expression.Lambda<Func<double?, double?, Func<double?>>>(
                    Expression.Lambda<Func<double?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<double?, double?, Func<double?>> f2 = e2.CompileForTest();

            double? f2Result = default(double?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<double?, double?, double?>>> e3 =
                Expression.Lambda<Func<Func<double?, double?, double?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<double?, double?, double?>>>(
                            Expression.Lambda<Func<double?, double?, double?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<double?, double?, double?> f3 = e3.CompileForTest()();

            double? f3Result = default(double?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<double?, double?, double?>>> e4 =
                Expression.Lambda<Func<Func<double?, double?, double?>>>(
                    Expression.Lambda<Func<double?, double?, double?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<double?, double?, double?>> f4 = e4.CompileForTest();

            double? f4Result = default(double?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<double?, Func<double?, double?>>> e5 =
                Expression.Lambda<Func<double?, Func<double?, double?>>>(
                    Expression.Lambda<Func<double?, double?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<double?, Func<double?, double?>> f5 = e5.CompileForTest();

            double? f5Result = default(double?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<double?, double?>>> e6 =
                Expression.Lambda<Func<Func<double?, double?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<double?, Func<double?, double?>>>(
                            Expression.Lambda<Func<double?, double?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(double?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<double?, double?> f6 = e6.CompileForTest()();

            double? f6Result = default(double?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            double? csResult = default(double?);
            Exception csEx = null;
            try
            {
                csResult = (double?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify float?

        private static void VerifySubtractNullableFloat(float? a, float? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(float?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(float?), "p1");

            // verify with parameters supplied
            Expression<Func<float?>> e1 =
                Expression.Lambda<Func<float?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<float?, float?, float?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(float?)),
                    Expression.Constant(b, typeof(float?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<float?> f1 = e1.CompileForTest();

            float? f1Result = default(float?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<float?, float?, Func<float?>>> e2 =
                Expression.Lambda<Func<float?, float?, Func<float?>>>(
                    Expression.Lambda<Func<float?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<float?, float?, Func<float?>> f2 = e2.CompileForTest();

            float? f2Result = default(float?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<float?, float?, float?>>> e3 =
                Expression.Lambda<Func<Func<float?, float?, float?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<float?, float?, float?>>>(
                            Expression.Lambda<Func<float?, float?, float?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<float?, float?, float?> f3 = e3.CompileForTest()();

            float? f3Result = default(float?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<float?, float?, float?>>> e4 =
                Expression.Lambda<Func<Func<float?, float?, float?>>>(
                    Expression.Lambda<Func<float?, float?, float?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<float?, float?, float?>> f4 = e4.CompileForTest();

            float? f4Result = default(float?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<float?, Func<float?, float?>>> e5 =
                Expression.Lambda<Func<float?, Func<float?, float?>>>(
                    Expression.Lambda<Func<float?, float?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<float?, Func<float?, float?>> f5 = e5.CompileForTest();

            float? f5Result = default(float?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<float?, float?>>> e6 =
                Expression.Lambda<Func<Func<float?, float?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<float?, Func<float?, float?>>>(
                            Expression.Lambda<Func<float?, float?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(float?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<float?, float?> f6 = e6.CompileForTest()();

            float? f6Result = default(float?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            float? csResult = default(float?);
            Exception csEx = null;
            try
            {
                csResult = (float?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify int?

        private static void VerifySubtractNullableInt(int? a, int? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(int?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(int?), "p1");

            // verify with parameters supplied
            Expression<Func<int?>> e1 =
                Expression.Lambda<Func<int?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<int?, int?, int?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(int?)),
                    Expression.Constant(b, typeof(int?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<int?> f1 = e1.CompileForTest();

            int? f1Result = default(int?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<int?, int?, Func<int?>>> e2 =
                Expression.Lambda<Func<int?, int?, Func<int?>>>(
                    Expression.Lambda<Func<int?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<int?, int?, Func<int?>> f2 = e2.CompileForTest();

            int? f2Result = default(int?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<int?, int?, int?>>> e3 =
                Expression.Lambda<Func<Func<int?, int?, int?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<int?, int?, int?>>>(
                            Expression.Lambda<Func<int?, int?, int?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<int?, int?, int?> f3 = e3.CompileForTest()();

            int? f3Result = default(int?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<int?, int?, int?>>> e4 =
                Expression.Lambda<Func<Func<int?, int?, int?>>>(
                    Expression.Lambda<Func<int?, int?, int?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<int?, int?, int?>> f4 = e4.CompileForTest();

            int? f4Result = default(int?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<int?, Func<int?, int?>>> e5 =
                Expression.Lambda<Func<int?, Func<int?, int?>>>(
                    Expression.Lambda<Func<int?, int?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<int?, Func<int?, int?>> f5 = e5.CompileForTest();

            int? f5Result = default(int?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<int?, int?>>> e6 =
                Expression.Lambda<Func<Func<int?, int?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<int?, Func<int?, int?>>>(
                            Expression.Lambda<Func<int?, int?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(int?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<int?, int?> f6 = e6.CompileForTest()();

            int? f6Result = default(int?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            int? csResult = default(int?);
            Exception csEx = null;
            try
            {
                csResult = (int?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify long?

        private static void VerifySubtractNullableLong(long? a, long? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(long?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(long?), "p1");

            // verify with parameters supplied
            Expression<Func<long?>> e1 =
                Expression.Lambda<Func<long?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<long?, long?, long?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(long?)),
                    Expression.Constant(b, typeof(long?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<long?> f1 = e1.CompileForTest();

            long? f1Result = default(long?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<long?, long?, Func<long?>>> e2 =
                Expression.Lambda<Func<long?, long?, Func<long?>>>(
                    Expression.Lambda<Func<long?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<long?, long?, Func<long?>> f2 = e2.CompileForTest();

            long? f2Result = default(long?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<long?, long?, long?>>> e3 =
                Expression.Lambda<Func<Func<long?, long?, long?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<long?, long?, long?>>>(
                            Expression.Lambda<Func<long?, long?, long?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<long?, long?, long?> f3 = e3.CompileForTest()();

            long? f3Result = default(long?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<long?, long?, long?>>> e4 =
                Expression.Lambda<Func<Func<long?, long?, long?>>>(
                    Expression.Lambda<Func<long?, long?, long?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<long?, long?, long?>> f4 = e4.CompileForTest();

            long? f4Result = default(long?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<long?, Func<long?, long?>>> e5 =
                Expression.Lambda<Func<long?, Func<long?, long?>>>(
                    Expression.Lambda<Func<long?, long?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<long?, Func<long?, long?>> f5 = e5.CompileForTest();

            long? f5Result = default(long?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<long?, long?>>> e6 =
                Expression.Lambda<Func<Func<long?, long?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<long?, Func<long?, long?>>>(
                            Expression.Lambda<Func<long?, long?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(long?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<long?, long?> f6 = e6.CompileForTest()();

            long? f6Result = default(long?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            long? csResult = default(long?);
            Exception csEx = null;
            try
            {
                csResult = (long?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify short?

        private static void VerifySubtractNullableShort(short? a, short? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(short?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(short?), "p1");

            // verify with parameters supplied
            Expression<Func<short?>> e1 =
                Expression.Lambda<Func<short?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<short?, short?, short?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(short?)),
                    Expression.Constant(b, typeof(short?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<short?> f1 = e1.CompileForTest();

            short? f1Result = default(short?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<short?, short?, Func<short?>>> e2 =
                Expression.Lambda<Func<short?, short?, Func<short?>>>(
                    Expression.Lambda<Func<short?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<short?, short?, Func<short?>> f2 = e2.CompileForTest();

            short? f2Result = default(short?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<short?, short?, short?>>> e3 =
                Expression.Lambda<Func<Func<short?, short?, short?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<short?, short?, short?>>>(
                            Expression.Lambda<Func<short?, short?, short?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<short?, short?, short?> f3 = e3.CompileForTest()();

            short? f3Result = default(short?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<short?, short?, short?>>> e4 =
                Expression.Lambda<Func<Func<short?, short?, short?>>>(
                    Expression.Lambda<Func<short?, short?, short?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<short?, short?, short?>> f4 = e4.CompileForTest();

            short? f4Result = default(short?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<short?, Func<short?, short?>>> e5 =
                Expression.Lambda<Func<short?, Func<short?, short?>>>(
                    Expression.Lambda<Func<short?, short?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<short?, Func<short?, short?>> f5 = e5.CompileForTest();

            short? f5Result = default(short?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<short?, short?>>> e6 =
                Expression.Lambda<Func<Func<short?, short?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<short?, Func<short?, short?>>>(
                            Expression.Lambda<Func<short?, short?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(short?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<short?, short?> f6 = e6.CompileForTest()();

            short? f6Result = default(short?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            short? csResult = default(short?);
            Exception csEx = null;
            try
            {
                csResult = (short?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify uint?

        private static void VerifySubtractNullableUInt(uint? a, uint? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(uint?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(uint?), "p1");

            // verify with parameters supplied
            Expression<Func<uint?>> e1 =
                Expression.Lambda<Func<uint?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<uint?, uint?, uint?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(uint?)),
                    Expression.Constant(b, typeof(uint?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint?> f1 = e1.CompileForTest();

            uint? f1Result = default(uint?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<uint?, uint?, Func<uint?>>> e2 =
                Expression.Lambda<Func<uint?, uint?, Func<uint?>>>(
                    Expression.Lambda<Func<uint?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<uint?, uint?, Func<uint?>> f2 = e2.CompileForTest();

            uint? f2Result = default(uint?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<uint?, uint?, uint?>>> e3 =
                Expression.Lambda<Func<Func<uint?, uint?, uint?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<uint?, uint?, uint?>>>(
                            Expression.Lambda<Func<uint?, uint?, uint?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint?, uint?, uint?> f3 = e3.CompileForTest()();

            uint? f3Result = default(uint?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<uint?, uint?, uint?>>> e4 =
                Expression.Lambda<Func<Func<uint?, uint?, uint?>>>(
                    Expression.Lambda<Func<uint?, uint?, uint?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<uint?, uint?, uint?>> f4 = e4.CompileForTest();

            uint? f4Result = default(uint?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<uint?, Func<uint?, uint?>>> e5 =
                Expression.Lambda<Func<uint?, Func<uint?, uint?>>>(
                    Expression.Lambda<Func<uint?, uint?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<uint?, Func<uint?, uint?>> f5 = e5.CompileForTest();

            uint? f5Result = default(uint?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<uint?, uint?>>> e6 =
                Expression.Lambda<Func<Func<uint?, uint?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<uint?, Func<uint?, uint?>>>(
                            Expression.Lambda<Func<uint?, uint?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(uint?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint?, uint?> f6 = e6.CompileForTest()();

            uint? f6Result = default(uint?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            uint? csResult = default(uint?);
            Exception csEx = null;
            try
            {
                csResult = (uint?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify ulong?

        private static void VerifySubtractNullableULong(ulong? a, ulong? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(ulong?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(ulong?), "p1");

            // verify with parameters supplied
            Expression<Func<ulong?>> e1 =
                Expression.Lambda<Func<ulong?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<ulong?, ulong?, ulong?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(ulong?)),
                    Expression.Constant(b, typeof(ulong?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong?> f1 = e1.CompileForTest();

            ulong? f1Result = default(ulong?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<ulong?, ulong?, Func<ulong?>>> e2 =
                Expression.Lambda<Func<ulong?, ulong?, Func<ulong?>>>(
                    Expression.Lambda<Func<ulong?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<ulong?, ulong?, Func<ulong?>> f2 = e2.CompileForTest();

            ulong? f2Result = default(ulong?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<ulong?, ulong?, ulong?>>> e3 =
                Expression.Lambda<Func<Func<ulong?, ulong?, ulong?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<ulong?, ulong?, ulong?>>>(
                            Expression.Lambda<Func<ulong?, ulong?, ulong?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong?, ulong?, ulong?> f3 = e3.CompileForTest()();

            ulong? f3Result = default(ulong?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<ulong?, ulong?, ulong?>>> e4 =
                Expression.Lambda<Func<Func<ulong?, ulong?, ulong?>>>(
                    Expression.Lambda<Func<ulong?, ulong?, ulong?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<ulong?, ulong?, ulong?>> f4 = e4.CompileForTest();

            ulong? f4Result = default(ulong?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<ulong?, Func<ulong?, ulong?>>> e5 =
                Expression.Lambda<Func<ulong?, Func<ulong?, ulong?>>>(
                    Expression.Lambda<Func<ulong?, ulong?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<ulong?, Func<ulong?, ulong?>> f5 = e5.CompileForTest();

            ulong? f5Result = default(ulong?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<ulong?, ulong?>>> e6 =
                Expression.Lambda<Func<Func<ulong?, ulong?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<ulong?, Func<ulong?, ulong?>>>(
                            Expression.Lambda<Func<ulong?, ulong?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(ulong?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong?, ulong?> f6 = e6.CompileForTest()();

            ulong? f6Result = default(ulong?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            ulong? csResult = default(ulong?);
            Exception csEx = null;
            try
            {
                csResult = (ulong?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #region Verify ushort?

        private static void VerifySubtractNullableUShort(ushort? a, ushort? b)
        {
            ParameterExpression p0 = Expression.Parameter(typeof(ushort?), "p0");
            ParameterExpression p1 = Expression.Parameter(typeof(ushort?), "p1");

            // verify with parameters supplied
            Expression<Func<ushort?>> e1 =
                Expression.Lambda<Func<ushort?>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<ushort?, ushort?, ushort?>>(
                            Expression.Subtract(p0, p1),
                            new ParameterExpression[] { p0, p1 }),
                        new Expression[]
                {
                    Expression.Constant(a, typeof(ushort?)),
                    Expression.Constant(b, typeof(ushort?))
                }),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort?> f1 = e1.CompileForTest();

            ushort? f1Result = default(ushort?);
            Exception f1Ex = null;
            try
            {
                f1Result = f1();
            }
            catch (Exception ex)
            {
                f1Ex = ex;
            }

            // verify with values passed to make parameters
            Expression<Func<ushort?, ushort?, Func<ushort?>>> e2 =
                Expression.Lambda<Func<ushort?, ushort?, Func<ushort?>>>(
                    Expression.Lambda<Func<ushort?>>(
                        Expression.Subtract(p0, p1),
                        Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p0, p1 });
            Func<ushort?, ushort?, Func<ushort?>> f2 = e2.CompileForTest();

            ushort? f2Result = default(ushort?);
            Exception f2Ex = null;
            try
            {
                f2Result = f2(a, b)();
            }
            catch (Exception ex)
            {
                f2Ex = ex;
            }

            // verify with values directly passed
            Expression<Func<Func<ushort?, ushort?, ushort?>>> e3 =
                Expression.Lambda<Func<Func<ushort?, ushort?, ushort?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<ushort?, ushort?, ushort?>>>(
                            Expression.Lambda<Func<ushort?, ushort?, ushort?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p0, p1 }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort?, ushort?, ushort?> f3 = e3.CompileForTest()();

            ushort? f3Result = default(ushort?);
            Exception f3Ex = null;
            try
            {
                f3Result = f3(a, b);
            }
            catch (Exception ex)
            {
                f3Ex = ex;
            }

            // verify as a function generator
            Expression<Func<Func<ushort?, ushort?, ushort?>>> e4 =
                Expression.Lambda<Func<Func<ushort?, ushort?, ushort?>>>(
                    Expression.Lambda<Func<ushort?, ushort?, ushort?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p0, p1 }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<ushort?, ushort?, ushort?>> f4 = e4.CompileForTest();

            ushort? f4Result = default(ushort?);
            Exception f4Ex = null;
            try
            {
                f4Result = f4()(a, b);
            }
            catch (Exception ex)
            {
                f4Ex = ex;
            }

            // verify with currying
            Expression<Func<ushort?, Func<ushort?, ushort?>>> e5 =
                Expression.Lambda<Func<ushort?, Func<ushort?, ushort?>>>(
                    Expression.Lambda<Func<ushort?, ushort?>>(
                        Expression.Subtract(p0, p1),
                        new ParameterExpression[] { p1 }),
                    new ParameterExpression[] { p0 });
            Func<ushort?, Func<ushort?, ushort?>> f5 = e5.CompileForTest();

            ushort? f5Result = default(ushort?);
            Exception f5Ex = null;
            try
            {
                f5Result = f5(a)(b);
            }
            catch (Exception ex)
            {
                f5Ex = ex;
            }

            // verify with one parameter
            Expression<Func<Func<ushort?, ushort?>>> e6 =
                Expression.Lambda<Func<Func<ushort?, ushort?>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<ushort?, Func<ushort?, ushort?>>>(
                            Expression.Lambda<Func<ushort?, ushort?>>(
                                Expression.Subtract(p0, p1),
                                new ParameterExpression[] { p1 }),
                            new ParameterExpression[] { p0 }),
                        new Expression[] { Expression.Constant(a, typeof(ushort?)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort?, ushort?> f6 = e6.CompileForTest()();

            ushort? f6Result = default(ushort?);
            Exception f6Ex = null;
            try
            {
                f6Result = f6(b);
            }
            catch (Exception ex)
            {
                f6Ex = ex;
            }

            // verify with regular IL
            ushort? csResult = default(ushort?);
            Exception csEx = null;
            try
            {
                csResult = (ushort?)(a - b);
            }
            catch (Exception ex)
            {
                csEx = ex;
            }

            // either all should have failed the same way or they should all produce the same result
            if (f1Ex != null || f2Ex != null || f3Ex != null || f4Ex != null || f5Ex != null || f6Ex != null || csEx != null)
            {
                Assert.NotNull(f1Ex);
                Assert.NotNull(f2Ex);
                Assert.NotNull(f3Ex);
                Assert.NotNull(f4Ex);
                Assert.NotNull(f5Ex);
                Assert.NotNull(f6Ex);
                Assert.NotNull(csEx);
                Assert.Equal(csEx.GetType(), f1Ex.GetType());
                Assert.Equal(csEx.GetType(), f2Ex.GetType());
                Assert.Equal(csEx.GetType(), f3Ex.GetType());
                Assert.Equal(csEx.GetType(), f4Ex.GetType());
                Assert.Equal(csEx.GetType(), f5Ex.GetType());
                Assert.Equal(csEx.GetType(), f6Ex.GetType());
            }
            else
            {
                Assert.Equal(csResult, f1Result);
                Assert.Equal(csResult, f2Result);
                Assert.Equal(csResult, f3Result);
                Assert.Equal(csResult, f4Result);
                Assert.Equal(csResult, f5Result);
                Assert.Equal(csResult, f6Result);
            }
        }

        #endregion


        #endregion
    }
}
