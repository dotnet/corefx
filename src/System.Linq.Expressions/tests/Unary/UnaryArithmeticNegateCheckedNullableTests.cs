// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class UnaryArithmeticNegateCheckedNullableTests
    {
        #region Test methods

        [Fact]
        public static void CheckUnaryArithmeticNegateCheckedNullableByteTest()
        {
            byte?[] values = new byte?[] { null, 0, 1, byte.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableByte(values[i]);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateCheckedNullableCharTest()
        {
            char?[] values = new char?[] { null, '\0', '\b', 'A', '\uffff' };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableChar(values[i]);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryArithmeticNegateCheckedNullableDecimalTest(bool useInterpreter)
        {
            decimal?[] values = new decimal?[] { null, decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableDecimal(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryArithmeticNegateCheckedNullableDoubleTest(bool useInterpreter)
        {
            double?[] values = new double?[] { null, 0, 1, -1, double.MinValue, double.MaxValue, double.Epsilon, double.NegativeInfinity, double.PositiveInfinity, double.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableDouble(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryArithmeticNegateCheckedNullableFloatTest(bool useInterpreter)
        {
            float?[] values = new float?[] { null, 0, 1, -1, float.MinValue, float.MaxValue, float.Epsilon, float.NegativeInfinity, float.PositiveInfinity, float.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableFloat(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryArithmeticNegateCheckedNullableIntTest(bool useInterpreter)
        {
            int?[] values = new int?[] { null, 0, 1, -1, int.MinValue, int.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableInt(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryArithmeticNegateCheckedNullableLongTest(bool useInterpreter)
        {
            long?[] values = new long?[] { null, 0, 1, -1, long.MinValue, long.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableLong(values[i], useInterpreter);
            }
        }

        [Fact]
        public static void CheckUnaryArithmeticNegateCheckedNullableSByteTest()
        {
            sbyte?[] values = new sbyte?[] { null, 0, 1, -1, sbyte.MinValue, sbyte.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableSByte(values[i]);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryArithmeticNegateCheckedNullableShortTest(bool useInterpreter)
        {
            short?[] values = new short?[] { null, 0, 1, -1, short.MinValue, short.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyArithmeticNegateCheckedNullableShort(values[i], useInterpreter);
            }
        }

        #endregion

        #region Test verifiers

        private static void VerifyArithmeticNegateCheckedNullableByte(byte? value)
        {
            Assert.Throws<InvalidOperationException>(() => Expression.NegateChecked(Expression.Constant(value, typeof(byte?))));
        }

        private static void VerifyArithmeticNegateCheckedNullableChar(char? value)
        {
            Assert.Throws<InvalidOperationException>(() => Expression.NegateChecked(Expression.Constant(value, typeof(char?))));
        }

        private static void VerifyArithmeticNegateCheckedNullableDecimal(decimal? value, bool useInterpreter)
        {
            Expression<Func<decimal?>> e =
                Expression.Lambda<Func<decimal?>>(
                    Expression.NegateChecked(Expression.Constant(value, typeof(decimal?))),
                    Enumerable.Empty<ParameterExpression>());

            Func<decimal?> f = e.Compile(useInterpreter);

            Assert.Equal(-value, f());
        }

        private static void VerifyArithmeticNegateCheckedNullableDouble(double? value, bool useInterpreter)
        {
            Expression<Func<double?>> e =
                Expression.Lambda<Func<double?>>(
                    Expression.NegateChecked(Expression.Constant(value, typeof(double?))),
                    Enumerable.Empty<ParameterExpression>());

            Func<double?> f = e.Compile(useInterpreter);

            Assert.Equal(-value, f());
        }

        private static void VerifyArithmeticNegateCheckedNullableFloat(float? value, bool useInterpreter)
        {
            Expression<Func<float?>> e =
                Expression.Lambda<Func<float?>>(
                    Expression.NegateChecked(Expression.Constant(value, typeof(float?))),
                    Enumerable.Empty<ParameterExpression>());

            Func<float?> f = e.Compile(useInterpreter);

            Assert.Equal(-value, f());
        }

        private static void VerifyArithmeticNegateCheckedNullableInt(int? value, bool useInterpreter)
        {
            Expression<Func<int?>> e =
                Expression.Lambda<Func<int?>>(
                    Expression.NegateChecked(Expression.Constant(value, typeof(int?))),
                    Enumerable.Empty<ParameterExpression>());

            Func<int?> f = e.Compile(useInterpreter);

            if (value == int.MinValue)
                Assert.Throws<OverflowException>(() => f());
            else
                Assert.Equal(-value, f());
        }

        private static void VerifyArithmeticNegateCheckedNullableLong(long? value, bool useInterpreter)
        {
            Expression<Func<long?>> e =
                Expression.Lambda<Func<long?>>(
                    Expression.NegateChecked(Expression.Constant(value, typeof(long?))),
                    Enumerable.Empty<ParameterExpression>());

            Func<long?> f = e.Compile(useInterpreter);

            if (value == long.MinValue)
                Assert.Throws<OverflowException>(() => f());
            else
                Assert.Equal(-value, f());
        }

        private static void VerifyArithmeticNegateCheckedNullableSByte(sbyte? value)
        {
            Assert.Throws<InvalidOperationException>(() => Expression.NegateChecked(Expression.Constant(value, typeof(sbyte?))));
        }

        private static void VerifyArithmeticNegateCheckedNullableShort(short? value, bool useInterpreter)
        {
            Expression<Func<short?>> e =
                Expression.Lambda<Func<short?>>(
                    Expression.NegateChecked(Expression.Constant(value, typeof(short?))),
                    Enumerable.Empty<ParameterExpression>());

            Func<short?> f = e.Compile(useInterpreter);

            if (value == short.MinValue)
                Assert.Throws<OverflowException>(() => f());
            else
                Assert.Equal(-value, f());
        }

        #endregion
    }
}
