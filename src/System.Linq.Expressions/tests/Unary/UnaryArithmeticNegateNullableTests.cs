// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class UnaryArithmeticNegateNullableTests
    {
        #region Test methods

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableByteTest()
        {
            byte?[] values = new byte?[] { null, 0, 1, byte.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableByte(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableCharTest()
        {
            char?[] values = new char?[] { null, '\0', '\b', 'A', '\uffff' };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableChar(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableDecimalTest()
        {
            decimal?[] values = new decimal?[] { null, decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableDecimal(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableDoubleTest()
        {
            double?[] values = new double?[] { null, 0, 1, -1, double.MinValue, double.MaxValue, double.Epsilon, double.NegativeInfinity, double.PositiveInfinity, double.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableDouble(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableFloatTest()
        {
            float?[] values = new float?[] { null, 0, 1, -1, float.MinValue, float.MaxValue, float.Epsilon, float.NegativeInfinity, float.PositiveInfinity, float.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableFloat(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableIntTest()
        {
            int?[] values = new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableInt(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableLongTest()
        {
            long?[] values = new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableLong(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableSByteTest()
        {
            sbyte?[] values = new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableSByte(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateNullableShortTest()
        {
            short?[] values = new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateNullableShort(values[i]);
            }
        }

        #endregion

        #region Test verifiers

        private static void VerifyArithmeticNegateNullableByte(byte? value)
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Negate(Expression.Constant(value, typeof(byte?))));
        }

        private static void VerifyArithmeticNegateNullableChar(char? value)
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Negate(Expression.Constant(value, typeof(char?))));
        }

        private static void VerifyArithmeticNegateNullableDecimal(decimal? value)
        {
            Expression<Func<decimal?>> e =
                Expression.Lambda<Func<decimal?>>(
                    Expression.Negate(Expression.Constant(value, typeof(decimal?))),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal?> f = e.Compile();
            Assert.Equal((decimal?)(-value), f());
        }

        private static void VerifyArithmeticNegateNullableDouble(double? value)
        {
            Expression<Func<double?>> e =
                Expression.Lambda<Func<double?>>(
                    Expression.Negate(Expression.Constant(value, typeof(double?))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double?> f = e.Compile();
            Assert.Equal((double?)(-value), f());
        }

        private static void VerifyArithmeticNegateNullableFloat(float? value)
        {
            Expression<Func<float?>> e =
                Expression.Lambda<Func<float?>>(
                    Expression.Negate(Expression.Constant(value, typeof(float?))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float?> f = e.Compile();
            Assert.Equal((float?)(-value), f());
        }

        private static void VerifyArithmeticNegateNullableInt(int? value)
        {
            Expression<Func<int?>> e =
                Expression.Lambda<Func<int?>>(
                    Expression.Negate(Expression.Constant(value, typeof(int?))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int?> f = e.Compile();
            Assert.Equal((int?)(-value), f());
        }

        private static void VerifyArithmeticNegateNullableLong(long? value)
        {
            Expression<Func<long?>> e =
                Expression.Lambda<Func<long?>>(
                    Expression.Negate(Expression.Constant(value, typeof(long?))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long?> f = e.Compile();
            Assert.Equal((long?)(-value), f());
        }

        private static void VerifyArithmeticNegateNullableSByte(sbyte? value)
        {
            Assert.Throws<InvalidOperationException>(() => Expression.Negate(Expression.Constant(value, typeof(sbyte?))));
        }

        private static void VerifyArithmeticNegateNullableShort(short? value)
        {
            Expression<Func<short?>> e =
                Expression.Lambda<Func<short?>>(
                    Expression.Negate(Expression.Constant(value, typeof(short?))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short?> f = e.Compile();
            Assert.Equal((short?)(-value), f());
        }

        #endregion
    }
}
