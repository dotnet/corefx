﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Tests.ExpressionCompiler.Unary
{
    public static class UnaryConvertTests
    {
        #region Test methods

        [Fact]
        public static void ConvertBoxingTest()
        {
            foreach (var e in ConvertBoxing())
            {
                VerifyUnaryConvert(e);
            }
        }

        [Fact]
        public static void ConvertUnboxingTest()
        {
            foreach (var e in ConvertUnboxing())
            {
                VerifyUnaryConvert(e);
            }
        }

        [Fact]
        public static void ConvertUnboxingInvalidCastTest()
        {
            foreach (var e in ConvertUnboxingInvalidCast())
            {
                VerifyUnaryConvertThrows<InvalidCastException>(e);
            }
        }

        [Fact] // [Issue(4019, "https://github.com/dotnet/corefx/issues/4019")]
        public static void CheckUnaryConvertBooleanToNumericTest()
        {
            foreach (var kv in ConvertBooleanToNumeric())
            {
                VerifyUnaryConvert(kv.Key, kv.Value);
            }
        }

        [Fact] // [Issue(4019, "https://github.com/dotnet/corefx/issues/4019")]
        public static void ConvertNullToNonNullableValueTest()
        {
            foreach (var e in ConvertNullToNonNullableValue())
            {
                VerifyUnaryConvertThrows<NullReferenceException>(e);
            }
        }

        [Fact] // [Issue(4019, "https://github.com/dotnet/corefx/issues/4019")]
        public static void ConvertNullToNullableValueTest()
        {
            foreach (var e in ConvertNullToNullableValue())
            {
                VerifyUnaryConvert(e, null);
            }
        }

        [Fact] // [Issue(4019, "https://github.com/dotnet/corefx/issues/4019")]
        public static void ConvertUnderlyingTypeToEnumTypeTest()
        {
            var enumValue = DayOfWeek.Monday;
            var value = (int)enumValue;

            foreach (var o in new[] { Expression.Constant(value, typeof(int)), Expression.Constant(value, typeof(ValueType)), Expression.Constant(value, typeof(object)) })
            {
                VerifyUnaryConvert(Expression.Convert(o, typeof(DayOfWeek)), enumValue);
            }
        }

        [Fact] // [Issue(4019, "https://github.com/dotnet/corefx/issues/4019")]
        public static void ConvertUnderlyingTypeToNullableEnumTypeTest()
        {
            var enumValue = DayOfWeek.Monday;
            var value = (int)enumValue;

            var cInt = Expression.Constant(value, typeof(int));
            VerifyUnaryConvert(Expression.Convert(cInt, typeof(DayOfWeek?)), enumValue);

            var cObj = Expression.Constant(value, typeof(object));
            VerifyUnaryConvertThrows<InvalidCastException>(Expression.Convert(cObj, typeof(DayOfWeek?)));
        }

        [Fact] // [Issue(4019, "https://github.com/dotnet/corefx/issues/4019")]
        public static void ConvertArrayToIncompatibleTypeTest()
        {
            var arr = new object[] { "bar" };

            foreach (var t in new[] { typeof(string[]), typeof(IEnumerable<char>[]) })
            {
                VerifyUnaryConvertThrows<InvalidCastException>(Expression.Convert(Expression.Constant(arr), t));
            }
        }

        private static IEnumerable<KeyValuePair<Expression, object>> ConvertBooleanToNumeric()
        {
            var boolF = Expression.Constant(false);
            var boolT = Expression.Constant(true);

            var factories = new Func<Expression, Type, Expression>[] { Expression.Convert, Expression.ConvertChecked };

            foreach (var factory in factories)
            {
                foreach (var b in new[] { false, true })
                {
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(byte)), (byte)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(sbyte)), (sbyte)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(ushort)), (ushort)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(short)), (short)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(uint)), (uint)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(int)), (int)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(ulong)), (ulong)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(long)), (long)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(float)), (float)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(double)), (double)(b ? 1 : 0));
                    yield return new KeyValuePair<Expression, object>(factory(b ? boolT : boolF, typeof(char)), (char)(b ? 1 : 0));
                }
            }
        }

        private static IEnumerable<Expression> ConvertNullToNonNullableValue()
        {
            var nullC = Expression.Constant(null);

            var factories = new Func<Expression, Type, Expression>[] { Expression.Convert, Expression.ConvertChecked };

            foreach (var factory in factories)
            {
                foreach (var b in new[] { false, true })
                {
                    yield return factory(nullC, typeof(byte));
                    yield return factory(nullC, typeof(sbyte));
                    yield return factory(nullC, typeof(ushort));
                    yield return factory(nullC, typeof(short));
                    yield return factory(nullC, typeof(uint));
                    yield return factory(nullC, typeof(int));
                    yield return factory(nullC, typeof(ulong));
                    yield return factory(nullC, typeof(long));
                    yield return factory(nullC, typeof(float));
                    yield return factory(nullC, typeof(double));
                    yield return factory(nullC, typeof(char));
                    yield return factory(nullC, typeof(TimeSpan));
                    yield return factory(nullC, typeof(DayOfWeek));
                }
            }
        }

        private static IEnumerable<Expression> ConvertNullToNullableValue()
        {
            var nullC = Expression.Constant(null);

            var factories = new Func<Expression, Type, Expression>[] { Expression.Convert, Expression.ConvertChecked };

            foreach (var factory in factories)
            {
                foreach (var b in new[] { false, true })
                {
                    yield return factory(nullC, typeof(byte?));
                    yield return factory(nullC, typeof(sbyte?));
                    yield return factory(nullC, typeof(ushort?));
                    yield return factory(nullC, typeof(short?));
                    yield return factory(nullC, typeof(uint?));
                    yield return factory(nullC, typeof(int?));
                    yield return factory(nullC, typeof(ulong?));
                    yield return factory(nullC, typeof(long?));
                    yield return factory(nullC, typeof(float?));
                    yield return factory(nullC, typeof(double?));
                    yield return factory(nullC, typeof(char?));
                    yield return factory(nullC, typeof(TimeSpan?));
                    yield return factory(nullC, typeof(DayOfWeek?));
                }
            }
        }

        private static IEnumerable<Expression> ConvertBoxing()
        {
            // C# Language Specification - 4.3.1 Boxing conversions
            // ----------------------------------------------------

            var factories = new Func<Expression, Type, Expression>[] { Expression.Convert, Expression.ConvertChecked };

            foreach (var factory in factories)
            {
                // >>> From any value-type to the type object.
                // >>> From any value-type to the type System.ValueType.
                foreach (var t in new[] { typeof(object), typeof(ValueType) })
                {
                    yield return factory(Expression.Constant(1, typeof(int)), t);
                    yield return factory(Expression.Constant(DayOfWeek.Monday, typeof(DayOfWeek)), t);
                    yield return factory(Expression.Constant(new TimeSpan(3, 14, 15), typeof(TimeSpan)), t);

                    yield return factory(Expression.Constant(1, typeof(int?)), t);
                    yield return factory(Expression.Constant(DayOfWeek.Monday, typeof(DayOfWeek?)), t);
                    yield return factory(Expression.Constant(new TimeSpan(3, 14, 15), typeof(TimeSpan?)), t);

                    yield return factory(Expression.Constant(null, typeof(int?)), t);
                    yield return factory(Expression.Constant(null, typeof(DayOfWeek?)), t);
                    yield return factory(Expression.Constant(null, typeof(TimeSpan?)), t);
                }

                // >>> From any non-nullable-value-type to any interface-type implemented by the value-type.
                foreach (var o in new object[] { 1, DayOfWeek.Monday, new TimeSpan(3, 14, 15) })
                {
                    var t = o.GetType();
                    var c = Expression.Constant(o, t);

                    foreach (var i in t.GetTypeInfo().ImplementedInterfaces)
                    {
                        yield return factory(c, i);
                    }
                }

                // >>> From any nullable-type to any interface-type implemented by the underlying type of the nullable-type.
                foreach (var o in new object[] { (int?)1, (DayOfWeek?)DayOfWeek.Monday, (TimeSpan?)new TimeSpan(3, 14, 15) })
                {
                    var t = o.GetType();
                    var n = typeof(Nullable<>).MakeGenericType(t);

                    foreach (var c in new[] { Expression.Constant(o, n), Expression.Constant(null, n) })
                    {
                        foreach (var i in t.GetTypeInfo().ImplementedInterfaces)
                        {
                            yield return factory(c, i);
                        }
                    }
                }

                // >>> From any enum-type to the type System.Enum.
                {
                    yield return factory(Expression.Constant(DayOfWeek.Monday, typeof(DayOfWeek)), typeof(Enum));
                }

                // >>> From any nullable-type with an underlying enum-type to the type System.Enum.
                {
                    yield return factory(Expression.Constant(DayOfWeek.Monday, typeof(DayOfWeek?)), typeof(Enum));
                    yield return factory(Expression.Constant(null, typeof(DayOfWeek?)), typeof(Enum));
                }
            }
        }

        private static IEnumerable<Expression> ConvertUnboxing()
        {
            // C# Language Specification - 4.3.2 Unboxing conversions
            // ----------------------------------------------------

            var factories = new Func<Expression, Type, Expression>[] { Expression.Convert, Expression.ConvertChecked };

            foreach (var factory in factories)
            {
                // >>> From the type object to any value-type.
                // >>> From the type System.ValueType to any value-type.
                foreach (var o in new object[] { 1, DayOfWeek.Monday, new TimeSpan(3, 14, 15) })
                {
                    var t = o.GetType();
                    var n = typeof(Nullable<>).MakeGenericType(t);

                    foreach (var f in new[] { typeof(object), typeof(ValueType) })
                    {
                        yield return factory(Expression.Constant(o, typeof(object)), t);
                        yield return factory(Expression.Constant(o, typeof(object)), n);
                    }
                }

                // >>> From any interface-type to any non-nullable-value-type that implements the interface-type.
                foreach (var o in new object[] { 1, DayOfWeek.Monday, new TimeSpan(3, 14, 15) })
                {
                    var t = o.GetType();

                    foreach (var i in t.GetTypeInfo().ImplementedInterfaces)
                    {
                        yield return factory(Expression.Constant(o, i), t);
                    }
                }

                // >>> From any interface-type to any nullable-type whose underlying type implements the interface-type.
                foreach (var o in new object[] { 1, DayOfWeek.Monday, new TimeSpan(3, 14, 15) })
                {
                    var t = o.GetType();
                    var n = typeof(Nullable<>).MakeGenericType(t);

                    foreach (var i in t.GetTypeInfo().ImplementedInterfaces)
                    {
                        yield return factory(Expression.Constant(o, i), n);
                    }
                }

                // >>> From the type System.Enum to any enum-type.
                {
                    yield return factory(Expression.Constant(DayOfWeek.Monday, typeof(Enum)), typeof(DayOfWeek));
                }

                // >>> From the type System.Enum to any nullable-type with an underlying enum-type.
                {
                    yield return factory(Expression.Constant(DayOfWeek.Monday, typeof(Enum)), typeof(DayOfWeek?));
                    yield return factory(Expression.Constant(null, typeof(Enum)), typeof(DayOfWeek?));
                }
            }
        }

        private static IEnumerable<Expression> ConvertUnboxingInvalidCast()
        {
            var objs = new object[] { 1, 1L, 1.0f, 1.0, true, TimeSpan.FromSeconds(1), "bar" };
            var types = objs.Select(o => o.GetType()).ToArray();

            foreach (var o in objs)
            {
                var c = Expression.Constant(o, typeof(object));

                foreach (var t in types)
                {
                    if (t != o.GetType())
                    {
                        yield return Expression.Convert(c, t);

                        if (t.GetTypeInfo().IsValueType)
                        {
                            var n = typeof(Nullable<>).MakeGenericType(t);
                            yield return Expression.Convert(c, n);
                        }
                    }
                }
            }
        }

        #endregion

        #region Test verifiers

        private static void VerifyUnaryConvert(Expression e, object o)
        {
            Expression<Func<object>> f =
                Expression.Lambda<Func<object>>(
                    Expression.Convert(e, typeof(object)));

            Func<object> c = f.Compile();
            Assert.Equal(o, c());

#if FEATURE_INTERPRET
            Func<object> i = f.Compile(true);
            Assert.Equal(o, i());
#endif
        }

        private static void VerifyUnaryConvertThrows<T>(Expression e)
            where T : Exception
        {
            Expression<Func<object>> f =
                Expression.Lambda<Func<object>>(
                    Expression.Convert(e, typeof(object)));

            Func<object> c = f.Compile();
            Assert.Throws<T>(() => c());

#if FEATURE_INTERPRET
            Func<object> i = f.Compile(true);
            Assert.Throws<T>(() => i());
#endif
        }

        private static void VerifyUnaryConvert(Expression e)
        {
            Expression<Func<object>> f =
                Expression.Lambda<Func<object>>(
                    Expression.Convert(e, typeof(object)));

            Func<object> c = f.Compile();
            object co = c(); // should not throw

#if FEATURE_INTERPRET
            Func<object> i = f.Compile(true);
            object io = i(); // should not throw

            Assert.Equal(io, co);
#endif
        }

        #endregion
    }
}
