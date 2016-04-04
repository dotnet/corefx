// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class UnaryIncrementTests
    {
        #region Test methods

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryIncrementShortTest(bool useInterpreter)
        {
            short[] values = new short[] { 0, 1, -1, short.MinValue, short.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementShort(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryIncrementUShortTest(bool useInterpreter)
        {
            ushort[] values = new ushort[] { 0, 1, ushort.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementUShort(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryIncrementIntTest(bool useInterpreter)
        {
            int[] values = new int[] { 0, 1, -1, int.MinValue, int.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementInt(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryIncrementUIntTest(bool useInterpreter)
        {
            uint[] values = new uint[] { 0, 1, uint.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementUInt(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryIncrementLongTest(bool useInterpreter)
        {
            long[] values = new long[] { 0, 1, -1, long.MinValue, long.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementLong(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckUnaryIncrementULongTest(bool useInterpreter)
        {
            ulong[] values = new ulong[] { 0, 1, ulong.MaxValue };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementULong(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckIncrementFloatTest(bool useInterpreter)
        {
            float[] values = new float[] { 0, 1, -1, float.MinValue, float.MaxValue, float.Epsilon, float.NegativeInfinity, float.PositiveInfinity, float.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementFloat(values[i], useInterpreter);
            }
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public static void CheckIncrementDoubleTest(bool useInterpreter)
        {
            double[] values = new double[] { 0, 1, -1, double.MinValue, double.MaxValue, double.Epsilon, double.NegativeInfinity, double.PositiveInfinity, double.NaN };
            for (int i = 0; i < values.Length; i++)
            {
                VerifyIncrementDouble(values[i], useInterpreter);
            }
        }

        #endregion

        #region Test verifiers

        private static void VerifyIncrementShort(short value, bool useInterpreter)
        {
            Expression<Func<short>> e =
                Expression.Lambda<Func<short>>(
                    Expression.Increment(Expression.Constant(value, typeof(short))),
                    Enumerable.Empty<ParameterExpression>());
            Func<short> f = e.Compile(useInterpreter);
            Assert.Equal((short)(++value), f());
        }

        private static void VerifyIncrementUShort(ushort value, bool useInterpreter)
        {
            Expression<Func<ushort>> e =
                Expression.Lambda<Func<ushort>>(
                    Expression.Increment(Expression.Constant(value, typeof(ushort))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort> f = e.Compile(useInterpreter);
            Assert.Equal((ushort)(++value), f());
        }

        private static void VerifyIncrementInt(int value, bool useInterpreter)
        {
            Expression<Func<int>> e =
                Expression.Lambda<Func<int>>(
                    Expression.Increment(Expression.Constant(value, typeof(int))),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f = e.Compile(useInterpreter);
            Assert.Equal((int)(++value), f());
        }

        private static void VerifyIncrementUInt(uint value, bool useInterpreter)
        {
            Expression<Func<uint>> e =
                Expression.Lambda<Func<uint>>(
                    Expression.Increment(Expression.Constant(value, typeof(uint))),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint> f = e.Compile(useInterpreter);
            Assert.Equal((uint)(++value), f());
        }

        private static void VerifyIncrementLong(long value, bool useInterpreter)
        {
            Expression<Func<long>> e =
                Expression.Lambda<Func<long>>(
                    Expression.Increment(Expression.Constant(value, typeof(long))),
                    Enumerable.Empty<ParameterExpression>());
            Func<long> f = e.Compile(useInterpreter);
            Assert.Equal((long)(++value), f());
        }

        private static void VerifyIncrementULong(ulong value, bool useInterpreter)
        {
            Expression<Func<ulong>> e =
                Expression.Lambda<Func<ulong>>(
                    Expression.Increment(Expression.Constant(value, typeof(ulong))),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong> f = e.Compile(useInterpreter);
            Assert.Equal((ulong)(++value), f());
        }

        private static void VerifyIncrementFloat(float value, bool useInterpreter)
        {
            Expression<Func<float>> e =
                Expression.Lambda<Func<float>>(
                    Expression.Increment(Expression.Constant(value, typeof(float))),
                    Enumerable.Empty<ParameterExpression>());
            Func<float> f = e.Compile(useInterpreter);
            Assert.Equal((float)(++value), f());
        }

        private static void VerifyIncrementDouble(double value, bool useInterpreter)
        {
            Expression<Func<double>> e =
                Expression.Lambda<Func<double>>(
                    Expression.Increment(Expression.Constant(value, typeof(double))),
                    Enumerable.Empty<ParameterExpression>());
            Func<double> f = e.Compile(useInterpreter);
            Assert.Equal((double)(++value), f());
        }

        #endregion
    }
}
