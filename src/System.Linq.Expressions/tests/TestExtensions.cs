// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions
{
    static class TestExtensions
    {
        public static T CompileForTest<T>(this Expression<T> expr)
        {
#if FEATURE_INTERPRET && FEATURE_COMPILE && ENABLE_CROSSCHECK
            var c = expr.Compile();
            var i = expr.Compile(true);

            return (T)(object)Combine(typeof(T), c as Delegate, i as Delegate);
#else
            return expr.Compile();
#endif
        }

        public static Delegate CompileForTest(this LambdaExpression expr)
        {
#if FEATURE_INTERPRET && FEATURE_COMPILE && ENABLE_CROSSCHECK
            var c = expr.Compile();
            var i = expr.Compile(true);

            return Combine(expr.Type, c, i);
#else
            return expr.Compile();
#endif
        }

        private static IDictionary<Type, Func<Delegate, Delegate, Delegate>> s_combiners = new Dictionary<Type, Func<Delegate, Delegate, Delegate>>();

        private static Delegate Combine(Type type, Delegate d1, Delegate d2)
        {
            var combine = default(Func<Delegate, Delegate, Delegate>);

            lock (s_combiners)
            {
                if (s_combiners.TryGetValue(type, out combine))
                {
                    return combine(d1, d2);
                }
            }

            var pd1 = Expression.Parameter(typeof(Delegate));
            var pd2 = Expression.Parameter(typeof(Delegate));

            var ad1 = Expression.Convert(pd1, type);
            var ad2 = Expression.Convert(pd2, type);

            var invoke = type.GetTypeInfo().GetDeclaredMethod("Invoke");

            var ps = invoke.GetParameters().Select((p, i) => Expression.Parameter(p.ParameterType, "p" + i)).ToArray();
            var ret = invoke.ReturnType;

            var validate = default(Expression);

            if (ret == typeof(void))
            {
                var exd1 = Expression.Parameter(typeof(Exception), "exd1");
                var exd2 = Expression.Parameter(typeof(Exception), "exd2");

                validate =
                    Expression.Block(
                        new[] { exd1, exd2 },
                        InvokeWithCatch(ad1, ps, exd1),
                        InvokeWithCatch(ad2, ps, exd2),
                        Expression.Call(s_assertVoid, exd1, exd2)
                    );
            }
            else
            {
                var exd1 = Expression.Parameter(typeof(Exception), "exd1");
                var exd2 = Expression.Parameter(typeof(Exception), "exd2");

                var rd1 = Expression.Parameter(ret, "rd1");
                var rd2 = Expression.Parameter(ret, "rd2");

                validate =
                    Expression.Block(
                        new[] { rd1, rd2, exd1, exd2 },
                        Expression.Assign(rd1, InvokeWithCatch(ad1, ps, exd1)),
                        Expression.Assign(rd2, InvokeWithCatch(ad2, ps, exd2)),
                        Expression.Call(s_assertNonVoid, Expression.Convert(rd1, typeof(object)), Expression.Convert(rd2, typeof(object)), exd1, exd2),
                        rd1
                    );
            }

            var body = Expression.Lambda(type, validate, ps);

            combine = Expression.Lambda<Func<Delegate, Delegate, Delegate>>(body, pd1, pd2).Compile();

            lock (s_combiners)
            {
                s_combiners[type] = combine;
            }

            return combine(d1, d2);
        }

        static Expression InvokeWithCatch(Expression d, ParameterExpression[] ps, ParameterExpression ex)
        {
            var exp = Expression.Parameter(typeof(Exception), "ex");

            var i = Expression.Invoke(d, ps);

            return
                Expression.TryCatch(
                    i,
                    Expression.Catch(
                        exp,
                        Expression.Block(
                            Expression.Assign(ex, exp),
                            Expression.Default(i.Type)
                        )
                    )
                );
        }

        private static MethodInfo s_assertVoid = ((MethodCallExpression)((Expression<Action>)(() => AssertVoid(default(Exception), default(Exception)))).Body).Method;
        private static MethodInfo s_assertNonVoid = ((MethodCallExpression)((Expression<Action>)(() => AssertNonVoid(default(object), default(object), default(Exception), default(Exception)))).Body).Method;

        static void AssertVoid(Exception e1, Exception e2)
        {
            if (e1 != null || e2 != null)
            {
                if (e1 == null || e2 == null)
                {
                    Assert.Equal(e1, e2);
                }

                Assert.Equal(e1.GetType(), e2.GetType());

                throw e1;
            }
        }

        static void AssertNonVoid(object o1, object o2, Exception e1, Exception e2)
        {
            AssertVoid(e1, e2);

            if (o1 != null && o2 != null)
            {
                // NB: This is a work-around for Lambda tests that return curried functions. There's no value equality for
                //     delegates, so we'll just check the delegate type matches. The test itself is responsible to check
                //     whether the outcome is semantically equivalent.
                if (o1 is Delegate)
                {
                    Assert.Equal(o1.GetType(), o2.GetType());
                    return;
                }

                // NB: Similar to above, for Quote tests that return expressions.
                if (o1 is LambdaExpression)
                {
                    Assert.Equal(o1.GetType(), o2.GetType());
                    return;
                }
            }

            Assert.Equal(o1, o2);
        }
    }
}
