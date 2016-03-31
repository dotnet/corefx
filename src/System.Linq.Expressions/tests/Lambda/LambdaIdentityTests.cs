// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class LambdaIdentityTests
    {
        #region Test methods

        [Fact]
        public static void CheckLambdaIdentityBoolTest()
        {
            foreach (bool value in new bool[] { true, false })
            {
                VerifyIdentityBool(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityByteTest()
        {
            foreach (byte value in new byte[] { 0, 1, byte.MaxValue })
            {
                VerifyIdentityByte(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityCustomTest()
        {
            foreach (C value in new C[] { null, new C(), new D(), new D(0), new D(5) })
            {
                VerifyIdentityCustom(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityCharTest()
        {
            foreach (char value in new char[] { '\0', '\b', 'A', '\uffff' })
            {
                VerifyIdentityChar(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityCustom2Test()
        {
            foreach (D value in new D[] { null, new D(), new D(0), new D(5) })
            {
                VerifyIdentityCustom2(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityDecimalTest()
        {
            foreach (decimal value in new decimal[] { decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue })
            {
                VerifyIdentityDecimal(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityDelegateTest()
        {
            foreach (Delegate value in new Delegate[] { null, (Func<object>)delegate () { return null; }, (Func<int, int>)delegate (int i) { return i + 1; }, (Action<object>)delegate { } })
            {
                VerifyIdentityDelegate(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityDoubleTest()
        {
            foreach (double value in new double[] { 0, 1, -1, double.MinValue, double.MaxValue, double.Epsilon, double.NegativeInfinity, double.PositiveInfinity, double.NaN })
            {
                VerifyIdentityDouble(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityEnumTest()
        {
            foreach (E value in new E[] { (E)0, E.A, E.B, (E)int.MaxValue, (E)int.MinValue })
            {
                VerifyIdentityEnum(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityEnumLongTest()
        {
            foreach (El value in new El[] { (El)0, El.A, El.B, (El)long.MaxValue, (El)long.MinValue })
            {
                VerifyIdentityEnumLong(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityFloatTest()
        {
            foreach (float value in new float[] { 0, 1, -1, float.MinValue, float.MaxValue, float.Epsilon, float.NegativeInfinity, float.PositiveInfinity, float.NaN })
            {
                VerifyIdentityFloat(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityFuncOfObjectTest()
        {
            foreach (Func<object> value in new Func<object>[] { null, (Func<object>)delegate () { return null; } })
            {
                VerifyIdentityFuncOfObject(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityInterfaceTest()
        {
            foreach (I value in new I[] { null, new C(), new D(), new D(0), new D(5) })
            {
                VerifyIdentityInterface(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityIEquatableOfCustomTest()
        {
            foreach (IEquatable<C> value in new IEquatable<C>[] { null, new C(), new D(), new D(0), new D(5) })
            {
                VerifyIdentityIEquatableOfCustom(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityIEquatableOfCustom2Test()
        {
            foreach (IEquatable<D> value in new IEquatable<D>[] { null, new D(), new D(0), new D(5) })
            {
                VerifyIdentityIEquatableOfCustom2(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityIntTest()
        {
            foreach (int value in new int[] { 0, 1, -1, int.MinValue, int.MaxValue })
            {
                VerifyIdentityInt(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityLongTest()
        {
            foreach (long value in new long[] { 0, 1, -1, long.MinValue, long.MaxValue })
            {
                VerifyIdentityLong(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityObjectTest()
        {
            foreach (object value in new object[] { null, new object(), new C(), new D(3) })
            {
                VerifyIdentityObject(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityStructTest()
        {
            foreach (S value in new S[] { default(S), new S() })
            {
                VerifyIdentityStruct(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentitySByteTest()
        {
            foreach (sbyte value in new sbyte[] { 0, 1, -1, sbyte.MinValue, sbyte.MaxValue })
            {
                VerifyIdentitySByte(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityStructWithStringTest()
        {
            foreach (Sc value in new Sc[] { default(Sc), new Sc(), new Sc(null) })
            {
                VerifyIdentityStructWithString(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityStructWithStringAndFieldTest()
        {
            foreach (Scs value in new Scs[] { default(Scs), new Scs(), new Scs(null, new S()) })
            {
                VerifyIdentityStructWithStringAndField(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityShortTest()
        {
            foreach (short value in new short[] { 0, 1, -1, short.MinValue, short.MaxValue })
            {
                VerifyIdentityShort(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityStructWithTwoValuesTest()
        {
            foreach (Sp value in new Sp[] { default(Sp), new Sp(), new Sp(5, 5.0) })
            {
                VerifyIdentityStructWithTwoValues(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityStructWithValueTest()
        {
            foreach (Ss value in new Ss[] { default(Ss), new Ss(), new Ss(new S()) })
            {
                VerifyIdentityStructWithValue(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityStringTest()
        {
            foreach (string value in new string[] { null, "", "a", "foo" })
            {
                VerifyIdentityString(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityUIntTest()
        {
            foreach (uint value in new uint[] { 0, 1, uint.MaxValue })
            {
                VerifyIdentityUInt(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityULongTest()
        {
            foreach (ulong value in new ulong[] { 0, 1, ulong.MaxValue })
            {
                VerifyIdentityULong(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityUShortTest()
        {
            foreach (ushort value in new ushort[] { 0, 1, ushort.MaxValue })
            {
                VerifyIdentityUShort(value);
            }
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithCustomTest()
        {
            CheckLambdaIdentityGenericHelper<C>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithEnumTest()
        {
            CheckLambdaIdentityGenericHelper<E>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithObjectTest()
        {
            CheckLambdaIdentityGenericHelper<object>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithStructTest()
        {
            CheckLambdaIdentityGenericHelper<S>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithStructWithStringAndFieldTest()
        {
            CheckLambdaIdentityGenericHelper<Scs>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithClassRestrictionWithCustomTest()
        {
            CheckLambdaIdentityGenericWithClassRestrictionHelper<C>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithClassRestrictionWithObjectTest()
        {
            CheckLambdaIdentityGenericWithClassRestrictionHelper<object>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithSubClassRestrictionWithCustomTest()
        {
            CheckLambdaIdentityGenericWithSubClassRestrictionHelper<C>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithClassAndNewRestrictionWithCustomTest()
        {
            CheckLambdaIdentityGenericWithClassAndNewRestrictionHelper<C>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithClassAndNewRestrictionWithObjectTest()
        {
            CheckLambdaIdentityGenericWithClassAndNewRestrictionHelper<object>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithSubClassAndNewRestrictionWithCustomTest()
        {
            CheckLambdaIdentityGenericWithSubClassAndNewRestrictionHelper<C>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithStructRestrictionWithEnumTest()
        {
            CheckLambdaIdentityGenericWithStructRestrictionHelper<E>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithStructRestrictionWithStructTest()
        {
            CheckLambdaIdentityGenericWithStructRestrictionHelper<S>();
        }

        [Fact]
        public static void CheckLambdaIdentityGenericWithStructRestrictionWithStructWithStringAndFieldTest()
        {
            CheckLambdaIdentityGenericWithStructRestrictionHelper<Scs>();
        }

        #endregion

        #region Generic helpers

        private static void CheckLambdaIdentityGenericHelper<T>()
        {
            foreach (T value in new T[] { default(T) })
            {
                VerifyIdentityGeneric<T>(value);
            }
        }

        private static void CheckLambdaIdentityGenericWithClassRestrictionHelper<Tc>() where Tc : class
        {
            foreach (Tc value in new Tc[] { null, default(Tc) })
            {
                VerifyIdentityGenericWithClassRestriction<Tc>(value);
            }
        }

        private static void CheckLambdaIdentityGenericWithSubClassRestrictionHelper<TC>() where TC : C
        {
            foreach (TC value in new TC[] { null, default(TC), (TC)new C() })
            {
                VerifyIdentityGenericWithSubClassRestriction<TC>(value);
            }
        }

        private static void CheckLambdaIdentityGenericWithClassAndNewRestrictionHelper<Tcn>() where Tcn : class, new()
        {
            foreach (Tcn value in new Tcn[] { null, default(Tcn), new Tcn() })
            {
                VerifyIdentityGenericWithClassAndNewRestriction<Tcn>(value);
            }
        }

        private static void CheckLambdaIdentityGenericWithSubClassAndNewRestrictionHelper<TCn>() where TCn : C, new()
        {
            foreach (TCn value in new TCn[] { null, default(TCn), new TCn(), (TCn)new C() })
            {
                VerifyIdentityGenericWithSubClassAndNewRestriction<TCn>(value);
            }
        }

        private static void CheckLambdaIdentityGenericWithStructRestrictionHelper<Ts>() where Ts : struct
        {
            foreach (Ts value in new Ts[] { default(Ts), new Ts() })
            {
                VerifyIdentityGenericWithStructRestriction<Ts>(value);
            }
        }

        #endregion

        #region Test verifiers
        private static void VerifyIdentityBool(bool value)
        {
            ParameterExpression p = Expression.Parameter(typeof(bool), "p");

            // parameter hard coded
            Expression<Func<bool>> e1 =
                Expression.Lambda<Func<bool>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<bool, bool>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(bool)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<bool, Func<bool>>> e2 =
                Expression.Lambda<Func<bool, Func<bool>>>(
                    Expression.Lambda<Func<bool>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<bool, Func<bool>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<bool, bool>>> e3 =
                Expression.Lambda<Func<Func<bool, bool>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<bool, bool>>>(
                            Expression.Lambda<Func<bool, bool>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<bool, bool> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<bool, bool>>> e4 =
                Expression.Lambda<Func<Func<bool, bool>>>(
                    Expression.Lambda<Func<bool, bool>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<bool, bool>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityByte(byte value)
        {
            ParameterExpression p = Expression.Parameter(typeof(byte), "p");

            // parameter hard coded
            Expression<Func<byte>> e1 =
                Expression.Lambda<Func<byte>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<byte, byte>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(byte)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<byte, Func<byte>>> e2 =
                Expression.Lambda<Func<byte, Func<byte>>>(
                    Expression.Lambda<Func<byte>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<byte, Func<byte>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<byte, byte>>> e3 =
                Expression.Lambda<Func<Func<byte, byte>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<byte, byte>>>(
                            Expression.Lambda<Func<byte, byte>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<byte, byte> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<byte, byte>>> e4 =
                Expression.Lambda<Func<Func<byte, byte>>>(
                    Expression.Lambda<Func<byte, byte>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<byte, byte>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityCustom(C value)
        {
            ParameterExpression p = Expression.Parameter(typeof(C), "p");

            // parameter hard coded
            Expression<Func<C>> e1 =
                Expression.Lambda<Func<C>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<C, C>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(C)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<C> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<C, Func<C>>> e2 =
                Expression.Lambda<Func<C, Func<C>>>(
                    Expression.Lambda<Func<C>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<C, Func<C>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<C, C>>> e3 =
                Expression.Lambda<Func<Func<C, C>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<C, C>>>(
                            Expression.Lambda<Func<C, C>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<C, C> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<C, C>>> e4 =
                Expression.Lambda<Func<Func<C, C>>>(
                    Expression.Lambda<Func<C, C>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<C, C>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityChar(char value)
        {
            ParameterExpression p = Expression.Parameter(typeof(char), "p");

            // parameter hard coded
            Expression<Func<char>> e1 =
                Expression.Lambda<Func<char>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<char, char>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(char)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<char> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<char, Func<char>>> e2 =
                Expression.Lambda<Func<char, Func<char>>>(
                    Expression.Lambda<Func<char>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<char, Func<char>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<char, char>>> e3 =
                Expression.Lambda<Func<Func<char, char>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<char, char>>>(
                            Expression.Lambda<Func<char, char>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<char, char> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<char, char>>> e4 =
                Expression.Lambda<Func<Func<char, char>>>(
                    Expression.Lambda<Func<char, char>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<char, char>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityCustom2(D value)
        {
            ParameterExpression p = Expression.Parameter(typeof(D), "p");

            // parameter hard coded
            Expression<Func<D>> e1 =
                Expression.Lambda<Func<D>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<D, D>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(D)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<D> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<D, Func<D>>> e2 =
                Expression.Lambda<Func<D, Func<D>>>(
                    Expression.Lambda<Func<D>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<D, Func<D>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<D, D>>> e3 =
                Expression.Lambda<Func<Func<D, D>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<D, D>>>(
                            Expression.Lambda<Func<D, D>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<D, D> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<D, D>>> e4 =
                Expression.Lambda<Func<Func<D, D>>>(
                    Expression.Lambda<Func<D, D>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<D, D>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityDecimal(decimal value)
        {
            ParameterExpression p = Expression.Parameter(typeof(decimal), "p");

            // parameter hard coded
            Expression<Func<decimal>> e1 =
                Expression.Lambda<Func<decimal>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<decimal, decimal>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(decimal)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<decimal, Func<decimal>>> e2 =
                Expression.Lambda<Func<decimal, Func<decimal>>>(
                    Expression.Lambda<Func<decimal>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<decimal, Func<decimal>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<decimal, decimal>>> e3 =
                Expression.Lambda<Func<Func<decimal, decimal>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<decimal, decimal>>>(
                            Expression.Lambda<Func<decimal, decimal>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<decimal, decimal> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<decimal, decimal>>> e4 =
                Expression.Lambda<Func<Func<decimal, decimal>>>(
                    Expression.Lambda<Func<decimal, decimal>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<decimal, decimal>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityDelegate(Delegate value)
        {
            ParameterExpression p = Expression.Parameter(typeof(Delegate), "p");

            // parameter hard coded
            Expression<Func<Delegate>> e1 =
                Expression.Lambda<Func<Delegate>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Delegate, Delegate>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Delegate)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Delegate, Func<Delegate>>> e2 =
                Expression.Lambda<Func<Delegate, Func<Delegate>>>(
                    Expression.Lambda<Func<Delegate>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Delegate, Func<Delegate>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Delegate, Delegate>>> e3 =
                Expression.Lambda<Func<Func<Delegate, Delegate>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Delegate, Delegate>>>(
                            Expression.Lambda<Func<Delegate, Delegate>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Delegate, Delegate> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Delegate, Delegate>>> e4 =
                Expression.Lambda<Func<Func<Delegate, Delegate>>>(
                    Expression.Lambda<Func<Delegate, Delegate>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Delegate, Delegate>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityDouble(double value)
        {
            ParameterExpression p = Expression.Parameter(typeof(double), "p");

            // parameter hard coded
            Expression<Func<double>> e1 =
                Expression.Lambda<Func<double>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<double, double>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(double)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<double> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<double, Func<double>>> e2 =
                Expression.Lambda<Func<double, Func<double>>>(
                    Expression.Lambda<Func<double>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<double, Func<double>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<double, double>>> e3 =
                Expression.Lambda<Func<Func<double, double>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<double, double>>>(
                            Expression.Lambda<Func<double, double>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<double, double> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<double, double>>> e4 =
                Expression.Lambda<Func<Func<double, double>>>(
                    Expression.Lambda<Func<double, double>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<double, double>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityEnum(E value)
        {
            ParameterExpression p = Expression.Parameter(typeof(E), "p");

            // parameter hard coded
            Expression<Func<E>> e1 =
                Expression.Lambda<Func<E>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<E, E>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(E)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<E> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<E, Func<E>>> e2 =
                Expression.Lambda<Func<E, Func<E>>>(
                    Expression.Lambda<Func<E>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<E, Func<E>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<E, E>>> e3 =
                Expression.Lambda<Func<Func<E, E>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<E, E>>>(
                            Expression.Lambda<Func<E, E>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<E, E> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<E, E>>> e4 =
                Expression.Lambda<Func<Func<E, E>>>(
                    Expression.Lambda<Func<E, E>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<E, E>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityEnumLong(El value)
        {
            ParameterExpression p = Expression.Parameter(typeof(El), "p");

            // parameter hard coded
            Expression<Func<El>> e1 =
                Expression.Lambda<Func<El>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<El, El>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(El)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<El> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<El, Func<El>>> e2 =
                Expression.Lambda<Func<El, Func<El>>>(
                    Expression.Lambda<Func<El>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<El, Func<El>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<El, El>>> e3 =
                Expression.Lambda<Func<Func<El, El>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<El, El>>>(
                            Expression.Lambda<Func<El, El>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<El, El> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<El, El>>> e4 =
                Expression.Lambda<Func<Func<El, El>>>(
                    Expression.Lambda<Func<El, El>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<El, El>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityFloat(float value)
        {
            ParameterExpression p = Expression.Parameter(typeof(float), "p");

            // parameter hard coded
            Expression<Func<float>> e1 =
                Expression.Lambda<Func<float>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<float, float>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(float)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<float> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<float, Func<float>>> e2 =
                Expression.Lambda<Func<float, Func<float>>>(
                    Expression.Lambda<Func<float>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<float, Func<float>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<float, float>>> e3 =
                Expression.Lambda<Func<Func<float, float>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<float, float>>>(
                            Expression.Lambda<Func<float, float>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<float, float> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<float, float>>> e4 =
                Expression.Lambda<Func<Func<float, float>>>(
                    Expression.Lambda<Func<float, float>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<float, float>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityFuncOfObject(Func<object> value)
        {
            ParameterExpression p = Expression.Parameter(typeof(Func<object>), "p");

            // parameter hard coded
            Expression<Func<Func<object>>> e1 =
                Expression.Lambda<Func<Func<object>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<object>, Func<object>>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Func<object>)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Func<object>, Func<Func<object>>>> e2 =
                Expression.Lambda<Func<Func<object>, Func<Func<object>>>>(
                    Expression.Lambda<Func<Func<object>>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Func<object>, Func<Func<object>>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Func<object>, Func<object>>>> e3 =
                Expression.Lambda<Func<Func<Func<object>, Func<object>>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Func<object>, Func<object>>>>(
                            Expression.Lambda<Func<Func<object>, Func<object>>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object>, Func<object>> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Func<object>, Func<object>>>> e4 =
                Expression.Lambda<Func<Func<Func<object>, Func<object>>>>(
                    Expression.Lambda<Func<Func<object>, Func<object>>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Func<object>, Func<object>>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityInterface(I value)
        {
            ParameterExpression p = Expression.Parameter(typeof(I), "p");

            // parameter hard coded
            Expression<Func<I>> e1 =
                Expression.Lambda<Func<I>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<I, I>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(I)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<I> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<I, Func<I>>> e2 =
                Expression.Lambda<Func<I, Func<I>>>(
                    Expression.Lambda<Func<I>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<I, Func<I>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<I, I>>> e3 =
                Expression.Lambda<Func<Func<I, I>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<I, I>>>(
                            Expression.Lambda<Func<I, I>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<I, I> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<I, I>>> e4 =
                Expression.Lambda<Func<Func<I, I>>>(
                    Expression.Lambda<Func<I, I>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<I, I>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityIEquatableOfCustom(IEquatable<C> value)
        {
            ParameterExpression p = Expression.Parameter(typeof(IEquatable<C>), "p");

            // parameter hard coded
            Expression<Func<IEquatable<C>>> e1 =
                Expression.Lambda<Func<IEquatable<C>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<IEquatable<C>, IEquatable<C>>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(IEquatable<C>)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<IEquatable<C>, Func<IEquatable<C>>>> e2 =
                Expression.Lambda<Func<IEquatable<C>, Func<IEquatable<C>>>>(
                    Expression.Lambda<Func<IEquatable<C>>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<IEquatable<C>, Func<IEquatable<C>>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<IEquatable<C>, IEquatable<C>>>> e3 =
                Expression.Lambda<Func<Func<IEquatable<C>, IEquatable<C>>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<IEquatable<C>, IEquatable<C>>>>(
                            Expression.Lambda<Func<IEquatable<C>, IEquatable<C>>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<C>, IEquatable<C>> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<IEquatable<C>, IEquatable<C>>>> e4 =
                Expression.Lambda<Func<Func<IEquatable<C>, IEquatable<C>>>>(
                    Expression.Lambda<Func<IEquatable<C>, IEquatable<C>>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<IEquatable<C>, IEquatable<C>>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityIEquatableOfCustom2(IEquatable<D> value)
        {
            ParameterExpression p = Expression.Parameter(typeof(IEquatable<D>), "p");

            // parameter hard coded
            Expression<Func<IEquatable<D>>> e1 =
                Expression.Lambda<Func<IEquatable<D>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<IEquatable<D>, IEquatable<D>>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(IEquatable<D>)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<IEquatable<D>, Func<IEquatable<D>>>> e2 =
                Expression.Lambda<Func<IEquatable<D>, Func<IEquatable<D>>>>(
                    Expression.Lambda<Func<IEquatable<D>>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<IEquatable<D>, Func<IEquatable<D>>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<IEquatable<D>, IEquatable<D>>>> e3 =
                Expression.Lambda<Func<Func<IEquatable<D>, IEquatable<D>>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<IEquatable<D>, IEquatable<D>>>>(
                            Expression.Lambda<Func<IEquatable<D>, IEquatable<D>>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<IEquatable<D>, IEquatable<D>> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<IEquatable<D>, IEquatable<D>>>> e4 =
                Expression.Lambda<Func<Func<IEquatable<D>, IEquatable<D>>>>(
                    Expression.Lambda<Func<IEquatable<D>, IEquatable<D>>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<IEquatable<D>, IEquatable<D>>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityInt(int value)
        {
            ParameterExpression p = Expression.Parameter(typeof(int), "p");

            // parameter hard coded
            Expression<Func<int>> e1 =
                Expression.Lambda<Func<int>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<int, int>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(int)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<int> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<int, Func<int>>> e2 =
                Expression.Lambda<Func<int, Func<int>>>(
                    Expression.Lambda<Func<int>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<int, Func<int>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<int, int>>> e3 =
                Expression.Lambda<Func<Func<int, int>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<int, int>>>(
                            Expression.Lambda<Func<int, int>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<int, int> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<int, int>>> e4 =
                Expression.Lambda<Func<Func<int, int>>>(
                    Expression.Lambda<Func<int, int>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<int, int>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityLong(long value)
        {
            ParameterExpression p = Expression.Parameter(typeof(long), "p");

            // parameter hard coded
            Expression<Func<long>> e1 =
                Expression.Lambda<Func<long>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<long, long>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(long)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<long> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<long, Func<long>>> e2 =
                Expression.Lambda<Func<long, Func<long>>>(
                    Expression.Lambda<Func<long>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<long, Func<long>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<long, long>>> e3 =
                Expression.Lambda<Func<Func<long, long>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<long, long>>>(
                            Expression.Lambda<Func<long, long>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<long, long> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<long, long>>> e4 =
                Expression.Lambda<Func<Func<long, long>>>(
                    Expression.Lambda<Func<long, long>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<long, long>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityObject(object value)
        {
            ParameterExpression p = Expression.Parameter(typeof(object), "p");

            // parameter hard coded
            Expression<Func<object>> e1 =
                Expression.Lambda<Func<object>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<object, object>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(object)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<object> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<object, Func<object>>> e2 =
                Expression.Lambda<Func<object, Func<object>>>(
                    Expression.Lambda<Func<object>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<object, Func<object>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<object, object>>> e3 =
                Expression.Lambda<Func<Func<object, object>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<object, object>>>(
                            Expression.Lambda<Func<object, object>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<object, object> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<object, object>>> e4 =
                Expression.Lambda<Func<Func<object, object>>>(
                    Expression.Lambda<Func<object, object>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<object, object>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityStruct(S value)
        {
            ParameterExpression p = Expression.Parameter(typeof(S), "p");

            // parameter hard coded
            Expression<Func<S>> e1 =
                Expression.Lambda<Func<S>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<S, S>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(S)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<S> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<S, Func<S>>> e2 =
                Expression.Lambda<Func<S, Func<S>>>(
                    Expression.Lambda<Func<S>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<S, Func<S>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<S, S>>> e3 =
                Expression.Lambda<Func<Func<S, S>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<S, S>>>(
                            Expression.Lambda<Func<S, S>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<S, S> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<S, S>>> e4 =
                Expression.Lambda<Func<Func<S, S>>>(
                    Expression.Lambda<Func<S, S>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<S, S>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentitySByte(sbyte value)
        {
            ParameterExpression p = Expression.Parameter(typeof(sbyte), "p");

            // parameter hard coded
            Expression<Func<sbyte>> e1 =
                Expression.Lambda<Func<sbyte>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<sbyte, sbyte>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(sbyte)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<sbyte, Func<sbyte>>> e2 =
                Expression.Lambda<Func<sbyte, Func<sbyte>>>(
                    Expression.Lambda<Func<sbyte>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<sbyte, Func<sbyte>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<sbyte, sbyte>>> e3 =
                Expression.Lambda<Func<Func<sbyte, sbyte>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<sbyte, sbyte>>>(
                            Expression.Lambda<Func<sbyte, sbyte>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<sbyte, sbyte> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<sbyte, sbyte>>> e4 =
                Expression.Lambda<Func<Func<sbyte, sbyte>>>(
                    Expression.Lambda<Func<sbyte, sbyte>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<sbyte, sbyte>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityStructWithString(Sc value)
        {
            ParameterExpression p = Expression.Parameter(typeof(Sc), "p");

            // parameter hard coded
            Expression<Func<Sc>> e1 =
                Expression.Lambda<Func<Sc>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Sc, Sc>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Sc)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Sc, Func<Sc>>> e2 =
                Expression.Lambda<Func<Sc, Func<Sc>>>(
                    Expression.Lambda<Func<Sc>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Sc, Func<Sc>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Sc, Sc>>> e3 =
                Expression.Lambda<Func<Func<Sc, Sc>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Sc, Sc>>>(
                            Expression.Lambda<Func<Sc, Sc>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sc, Sc> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Sc, Sc>>> e4 =
                Expression.Lambda<Func<Func<Sc, Sc>>>(
                    Expression.Lambda<Func<Sc, Sc>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Sc, Sc>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityStructWithStringAndField(Scs value)
        {
            ParameterExpression p = Expression.Parameter(typeof(Scs), "p");

            // parameter hard coded
            Expression<Func<Scs>> e1 =
                Expression.Lambda<Func<Scs>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Scs, Scs>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Scs)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Scs, Func<Scs>>> e2 =
                Expression.Lambda<Func<Scs, Func<Scs>>>(
                    Expression.Lambda<Func<Scs>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Scs, Func<Scs>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Scs, Scs>>> e3 =
                Expression.Lambda<Func<Func<Scs, Scs>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Scs, Scs>>>(
                            Expression.Lambda<Func<Scs, Scs>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Scs, Scs> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Scs, Scs>>> e4 =
                Expression.Lambda<Func<Func<Scs, Scs>>>(
                    Expression.Lambda<Func<Scs, Scs>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Scs, Scs>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityShort(short value)
        {
            ParameterExpression p = Expression.Parameter(typeof(short), "p");

            // parameter hard coded
            Expression<Func<short>> e1 =
                Expression.Lambda<Func<short>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<short, short>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(short)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<short> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<short, Func<short>>> e2 =
                Expression.Lambda<Func<short, Func<short>>>(
                    Expression.Lambda<Func<short>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<short, Func<short>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<short, short>>> e3 =
                Expression.Lambda<Func<Func<short, short>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<short, short>>>(
                            Expression.Lambda<Func<short, short>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<short, short> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<short, short>>> e4 =
                Expression.Lambda<Func<Func<short, short>>>(
                    Expression.Lambda<Func<short, short>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<short, short>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityStructWithTwoValues(Sp value)
        {
            ParameterExpression p = Expression.Parameter(typeof(Sp), "p");

            // parameter hard coded
            Expression<Func<Sp>> e1 =
                Expression.Lambda<Func<Sp>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Sp, Sp>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Sp)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Sp, Func<Sp>>> e2 =
                Expression.Lambda<Func<Sp, Func<Sp>>>(
                    Expression.Lambda<Func<Sp>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Sp, Func<Sp>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Sp, Sp>>> e3 =
                Expression.Lambda<Func<Func<Sp, Sp>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Sp, Sp>>>(
                            Expression.Lambda<Func<Sp, Sp>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Sp, Sp> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Sp, Sp>>> e4 =
                Expression.Lambda<Func<Func<Sp, Sp>>>(
                    Expression.Lambda<Func<Sp, Sp>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Sp, Sp>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityStructWithValue(Ss value)
        {
            ParameterExpression p = Expression.Parameter(typeof(Ss), "p");

            // parameter hard coded
            Expression<Func<Ss>> e1 =
                Expression.Lambda<Func<Ss>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Ss, Ss>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Ss)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Ss, Func<Ss>>> e2 =
                Expression.Lambda<Func<Ss, Func<Ss>>>(
                    Expression.Lambda<Func<Ss>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Ss, Func<Ss>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Ss, Ss>>> e3 =
                Expression.Lambda<Func<Func<Ss, Ss>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Ss, Ss>>>(
                            Expression.Lambda<Func<Ss, Ss>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ss, Ss> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Ss, Ss>>> e4 =
                Expression.Lambda<Func<Func<Ss, Ss>>>(
                    Expression.Lambda<Func<Ss, Ss>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Ss, Ss>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityString(string value)
        {
            ParameterExpression p = Expression.Parameter(typeof(string), "p");

            // parameter hard coded
            Expression<Func<string>> e1 =
                Expression.Lambda<Func<string>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<string, string>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(string)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<string> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<string, Func<string>>> e2 =
                Expression.Lambda<Func<string, Func<string>>>(
                    Expression.Lambda<Func<string>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<string, Func<string>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<string, string>>> e3 =
                Expression.Lambda<Func<Func<string, string>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<string, string>>>(
                            Expression.Lambda<Func<string, string>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<string, string> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<string, string>>> e4 =
                Expression.Lambda<Func<Func<string, string>>>(
                    Expression.Lambda<Func<string, string>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<string, string>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityUInt(uint value)
        {
            ParameterExpression p = Expression.Parameter(typeof(uint), "p");

            // parameter hard coded
            Expression<Func<uint>> e1 =
                Expression.Lambda<Func<uint>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<uint, uint>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(uint)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<uint, Func<uint>>> e2 =
                Expression.Lambda<Func<uint, Func<uint>>>(
                    Expression.Lambda<Func<uint>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<uint, Func<uint>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<uint, uint>>> e3 =
                Expression.Lambda<Func<Func<uint, uint>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<uint, uint>>>(
                            Expression.Lambda<Func<uint, uint>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<uint, uint> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<uint, uint>>> e4 =
                Expression.Lambda<Func<Func<uint, uint>>>(
                    Expression.Lambda<Func<uint, uint>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<uint, uint>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityULong(ulong value)
        {
            ParameterExpression p = Expression.Parameter(typeof(ulong), "p");

            // parameter hard coded
            Expression<Func<ulong>> e1 =
                Expression.Lambda<Func<ulong>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<ulong, ulong>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(ulong)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<ulong, Func<ulong>>> e2 =
                Expression.Lambda<Func<ulong, Func<ulong>>>(
                    Expression.Lambda<Func<ulong>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<ulong, Func<ulong>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<ulong, ulong>>> e3 =
                Expression.Lambda<Func<Func<ulong, ulong>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<ulong, ulong>>>(
                            Expression.Lambda<Func<ulong, ulong>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<ulong, ulong> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<ulong, ulong>>> e4 =
                Expression.Lambda<Func<Func<ulong, ulong>>>(
                    Expression.Lambda<Func<ulong, ulong>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<ulong, ulong>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityUShort(ushort value)
        {
            ParameterExpression p = Expression.Parameter(typeof(ushort), "p");

            // parameter hard coded
            Expression<Func<ushort>> e1 =
                Expression.Lambda<Func<ushort>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<ushort, ushort>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(ushort)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<ushort, Func<ushort>>> e2 =
                Expression.Lambda<Func<ushort, Func<ushort>>>(
                    Expression.Lambda<Func<ushort>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<ushort, Func<ushort>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<ushort, ushort>>> e3 =
                Expression.Lambda<Func<Func<ushort, ushort>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<ushort, ushort>>>(
                            Expression.Lambda<Func<ushort, ushort>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<ushort, ushort> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<ushort, ushort>>> e4 =
                Expression.Lambda<Func<Func<ushort, ushort>>>(
                    Expression.Lambda<Func<ushort, ushort>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<ushort, ushort>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityGeneric<T>(T value)
        {
            ParameterExpression p = Expression.Parameter(typeof(T), "p");

            // parameter hard coded
            Expression<Func<T>> e1 =
                Expression.Lambda<Func<T>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<T, T>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(T)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<T> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<T, Func<T>>> e2 =
                Expression.Lambda<Func<T, Func<T>>>(
                    Expression.Lambda<Func<T>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<T, Func<T>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<T, T>>> e3 =
                Expression.Lambda<Func<Func<T, T>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<T, T>>>(
                            Expression.Lambda<Func<T, T>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<T, T> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<T, T>>> e4 =
                Expression.Lambda<Func<Func<T, T>>>(
                    Expression.Lambda<Func<T, T>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<T, T>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityGenericWithClassRestriction<Tc>(Tc value) where Tc : class
        {
            ParameterExpression p = Expression.Parameter(typeof(Tc), "p");

            // parameter hard coded
            Expression<Func<Tc>> e1 =
                Expression.Lambda<Func<Tc>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Tc, Tc>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Tc)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Tc, Func<Tc>>> e2 =
                Expression.Lambda<Func<Tc, Func<Tc>>>(
                    Expression.Lambda<Func<Tc>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Tc, Func<Tc>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Tc, Tc>>> e3 =
                Expression.Lambda<Func<Func<Tc, Tc>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Tc, Tc>>>(
                            Expression.Lambda<Func<Tc, Tc>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tc, Tc> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Tc, Tc>>> e4 =
                Expression.Lambda<Func<Func<Tc, Tc>>>(
                    Expression.Lambda<Func<Tc, Tc>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Tc, Tc>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityGenericWithSubClassRestriction<TC>(TC value) where TC : C
        {
            ParameterExpression p = Expression.Parameter(typeof(TC), "p");

            // parameter hard coded
            Expression<Func<TC>> e1 =
                Expression.Lambda<Func<TC>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<TC, TC>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(TC)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<TC, Func<TC>>> e2 =
                Expression.Lambda<Func<TC, Func<TC>>>(
                    Expression.Lambda<Func<TC>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<TC, Func<TC>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<TC, TC>>> e3 =
                Expression.Lambda<Func<Func<TC, TC>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<TC, TC>>>(
                            Expression.Lambda<Func<TC, TC>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<TC, TC> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<TC, TC>>> e4 =
                Expression.Lambda<Func<Func<TC, TC>>>(
                    Expression.Lambda<Func<TC, TC>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<TC, TC>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityGenericWithClassAndNewRestriction<Tcn>(Tcn value) where Tcn : class, new()
        {
            ParameterExpression p = Expression.Parameter(typeof(Tcn), "p");

            // parameter hard coded
            Expression<Func<Tcn>> e1 =
                Expression.Lambda<Func<Tcn>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Tcn, Tcn>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Tcn)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Tcn, Func<Tcn>>> e2 =
                Expression.Lambda<Func<Tcn, Func<Tcn>>>(
                    Expression.Lambda<Func<Tcn>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Tcn, Func<Tcn>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Tcn, Tcn>>> e3 =
                Expression.Lambda<Func<Func<Tcn, Tcn>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Tcn, Tcn>>>(
                            Expression.Lambda<Func<Tcn, Tcn>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Tcn, Tcn> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Tcn, Tcn>>> e4 =
                Expression.Lambda<Func<Func<Tcn, Tcn>>>(
                    Expression.Lambda<Func<Tcn, Tcn>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Tcn, Tcn>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityGenericWithSubClassAndNewRestriction<TCn>(TCn value) where TCn : C, new()
        {
            ParameterExpression p = Expression.Parameter(typeof(TCn), "p");

            // parameter hard coded
            Expression<Func<TCn>> e1 =
                Expression.Lambda<Func<TCn>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<TCn, TCn>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(TCn)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<TCn, Func<TCn>>> e2 =
                Expression.Lambda<Func<TCn, Func<TCn>>>(
                    Expression.Lambda<Func<TCn>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<TCn, Func<TCn>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<TCn, TCn>>> e3 =
                Expression.Lambda<Func<Func<TCn, TCn>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<TCn, TCn>>>(
                            Expression.Lambda<Func<TCn, TCn>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<TCn, TCn> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<TCn, TCn>>> e4 =
                Expression.Lambda<Func<Func<TCn, TCn>>>(
                    Expression.Lambda<Func<TCn, TCn>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<TCn, TCn>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }


        private static void VerifyIdentityGenericWithStructRestriction<Ts>(Ts value) where Ts : struct
        {
            ParameterExpression p = Expression.Parameter(typeof(Ts), "p");

            // parameter hard coded
            Expression<Func<Ts>> e1 =
                Expression.Lambda<Func<Ts>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Ts, Ts>>(p, new ParameterExpression[] { p }),
                        new Expression[] { Expression.Constant(value, typeof(Ts)) }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts> f1 = e1.Compile();

            // parameter passed into function generator
            Expression<Func<Ts, Func<Ts>>> e2 =
                Expression.Lambda<Func<Ts, Func<Ts>>>(
                    Expression.Lambda<Func<Ts>>(p, Enumerable.Empty<ParameterExpression>()),
                    new ParameterExpression[] { p });
            Func<Ts, Func<Ts>> f2 = e2.Compile();

            // parameter passed into invoked generated function
            Expression<Func<Func<Ts, Ts>>> e3 =
                Expression.Lambda<Func<Func<Ts, Ts>>>(
                    Expression.Invoke(
                        Expression.Lambda<Func<Func<Ts, Ts>>>(
                            Expression.Lambda<Func<Ts, Ts>>(p, new ParameterExpression[] { p }),
                            Enumerable.Empty<ParameterExpression>()),
                        Enumerable.Empty<Expression>()),
                    Enumerable.Empty<ParameterExpression>());
            Func<Ts, Ts> f3 = e3.Compile()();

            // parameter passed into generated function
            Expression<Func<Func<Ts, Ts>>> e4 =
                Expression.Lambda<Func<Func<Ts, Ts>>>(
                    Expression.Lambda<Func<Ts, Ts>>(p, new ParameterExpression[] { p }),
                    Enumerable.Empty<ParameterExpression>());
            Func<Func<Ts, Ts>> f4 = e4.Compile();

            Assert.Equal(value, f1());
            Assert.Equal(value, f2(value)());
            Assert.Equal(value, f3(value));
            Assert.Equal(value, f4()(value));
        }

        #endregion
    }
}
