﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

using static System.Linq.Expressions.Expression;

namespace System.Linq.Expressions.Tests
{
    public static class UnaryQuoteTests
    {
        [Theory, ClassData(typeof(CompilationTypes))]
        public static void QuotePreservesTypingOfBlock(bool useInterpreter)
        {
            var x = Parameter(typeof(int));

            var f1 =
                Lambda<Func<int, Type>>(
                    Call(
                        typeof(UnaryQuoteTests).GetMethod(nameof(Quote1)),
                        Lambda(
                            Block(typeof(void), x)
                        )
                    ),
                    x
                );

            Assert.Equal(typeof(void), f1.Compile(useInterpreter)(42));

            var s = Parameter(typeof(string));

            var f2 =
                Lambda<Func<string, Type>>(
                    Call(
                        typeof(UnaryQuoteTests).GetMethod(nameof(Quote2)),
                        Lambda(
                            Block(typeof(object), s)
                        )
                    ),
                    s
                );

            Assert.Equal(typeof(object), f2.Compile(useInterpreter)("bar"));
        }

        public static Type Quote1(Expression<Action> e) => e.Body.Type;
        public static Type Quote2(Expression<Func<object>> e) => e.Body.Type;

#if FEATURE_COMPILE
        [Fact]
        public static void Quote_Lambda_Action()
        {
            Expression<Func<LambdaExpression>> f = () => GetQuote<Action>(() => Nop());

            var quote = f.Compile()();

            Assert.Equal(0, quote.Parameters.Count);
            Assert.Equal(ExpressionType.Call, quote.Body.NodeType);

            var call = (MethodCallExpression)quote.Body;
            Assert.Equal(typeof(UnaryQuoteTests).GetMethod(nameof(Nop)), call.Method);
        }

        [Fact]
        public static void Quote_Lambda_IdentityFunc()
        {
            Expression<Func<LambdaExpression>> f = () => GetQuote<Func<int, int>>(x => x);

            var quote = f.Compile()();

            Assert.Equal(1, quote.Parameters.Count);
            Assert.Same(quote.Body, quote.Parameters[0]);
        }

        [Fact]
        public static void Quote_Lambda_Closure1()
        {
            Expression<Func<int, LambdaExpression>> f = x => GetQuote<Func<int>>(() => x);

            var quote = f.Compile()(42);

            Assert.Equal(0, quote.Parameters.Count);
            AssertIsBox(quote.Body, 42);
        }

        [Fact]
        public static void Quote_Lambda_Closure2()
        {
            Expression<Func<int, Func<int, LambdaExpression>>> f = x => y => GetQuote<Func<int>>(() => x + y);

            var quote = f.Compile()(1)(2);

            Assert.Equal(0, quote.Parameters.Count);

            Assert.Equal(ExpressionType.Add, quote.Body.NodeType);

            var add = (BinaryExpression)quote.Body;
            AssertIsBox(add.Left, 1);
            AssertIsBox(add.Right, 2);
        }

        [Fact]
        public static void Quote_Block_Action()
        {
            var expr =
                Block(
                    Call(typeof(UnaryQuoteTests).GetMethod(nameof(Nop)))
                );

            var f = BuildQuote<Func<LambdaExpression>, Action>(expr);

            var quote = f.Compile()();

            Assert.Equal(0, quote.Parameters.Count);
            Assert.Same(expr, quote.Body);
        }

        [Fact]
        public static void Quote_Block_Local()
        {
            var x = Parameter(typeof(int));

            var expr =
                Block(
                    new[] { x },
                    Assign(x, Constant(42)),
                    x
                );

            var f = BuildQuote<Func<LambdaExpression>, Func<int>>(expr);

            var quote = f.Compile()();

            Assert.Equal(0, quote.Parameters.Count);
            Assert.Same(expr, quote.Body);
        }

        [Fact]
        public static void Quote_Block_Local_Shadow()
        {
            var x = Parameter(typeof(int));

            var expr =
                Block(
                    new[] { x },
                    Assign(x, Constant(42)),
                    x
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(43);

            Assert.Equal(0, quote.Parameters.Count);
            Assert.Same(expr, quote.Body);
        }

        [Fact]
        public static void Quote_Block_Closure()
        {
            var x = Parameter(typeof(int));

            var expr =
                Block(
                    x
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(42);

            Assert.Equal(0, quote.Parameters.Count);

            var block = quote.Body as BlockExpression;
            Assert.NotNull(block);
            Assert.Equal(0, block.Variables.Count);
            Assert.Equal(1, block.Expressions.Count);

            AssertIsBox(block.Expressions[0], 42);
        }

        [Fact]
        public static void Quote_Block_LocalAndClosure()
        {
            var x = Parameter(typeof(int));
            var y = Parameter(typeof(int));

            var expr =
                Block(
                    new[] { y },
                    Assign(y, Constant(2)),
                    Add(
                        x,
                        y
                    )
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(1);

            Assert.Equal(0, quote.Parameters.Count);

            var block = quote.Body as BlockExpression;
            Assert.NotNull(block);

            Assert.Equal(1, block.Variables.Count);
            Assert.Same(y, block.Variables[0]);

            Assert.Equal(2, block.Expressions.Count);
            Assert.Same(block.Expressions[0], expr.Expressions[0]);

            var expr1 = block.Expressions[1];
            Assert.Equal(ExpressionType.Add, expr1.NodeType);

            var add = (BinaryExpression)expr1;

            AssertIsBox(add.Left, 1);
            Assert.Same(y, add.Right);
        }

        [Fact]
        public static void Quote_CatchBlock_Local()
        {
            var ex = Parameter(typeof(Exception));

            var expr =
                TryCatch(
                    Empty(),
                    Catch(
                        ex,
                        Empty()
                    )
                );

            var f = BuildQuote<Func<LambdaExpression>, Action>(expr);

            var quote = f.Compile()();

            Assert.Equal(0, quote.Parameters.Count);
            Assert.Same(expr, quote.Body);
        }

        [Fact]
        public static void Quote_CatchBlock_Variable_Closure1()
        {
            var x = Parameter(typeof(int));
            var ex = Parameter(typeof(Exception));

            var expr =
                TryCatch(
                    x,
                    Catch(
                        ex,
                        Constant(0)
                    )
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(42);

            Assert.Equal(0, quote.Parameters.Count);

            var @try = quote.Body as TryExpression;
            Assert.NotNull(@try);

            AssertIsBox(@try.Body, 42);
            Assert.Null(@try.Fault);
            Assert.Null(@try.Finally);

            Assert.NotNull(@try.Handlers);
            Assert.Equal(1, @try.Handlers.Count);

            var handler = @try.Handlers[0];
            Assert.Same(expr.Handlers[0], handler);
        }

        [Fact]
        public static void Quote_CatchBlock_Variable_Closure2()
        {
            var x = Parameter(typeof(int));
            var ex = Parameter(typeof(Exception));

            var expr =
                TryCatch(
                    Constant(0),
                    Catch(
                        ex,
                        x
                    )
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(42);

            Assert.Equal(0, quote.Parameters.Count);

            var @try = quote.Body as TryExpression;
            Assert.NotNull(@try);

            Assert.Same(expr.Body, @try.Body);
            Assert.Null(@try.Fault);
            Assert.Null(@try.Finally);

            Assert.NotNull(@try.Handlers);
            Assert.Equal(1, @try.Handlers.Count);

            var handler = @try.Handlers[0];
            Assert.Null(handler.Filter);
            Assert.Same(ex, handler.Variable);
            AssertIsBox(@handler.Body, 42);
        }

        [Fact]
        public static void Quote_CatchBlock_NoVariable_Closure1()
        {
            var x = Parameter(typeof(int));

            var expr =
                TryCatch(
                    x,
                    Catch(
                        typeof(Exception),
                        Constant(0)
                    )
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(42);

            Assert.Equal(0, quote.Parameters.Count);

            var @try = quote.Body as TryExpression;
            Assert.NotNull(@try);

            AssertIsBox(@try.Body, 42);
            Assert.Null(@try.Fault);
            Assert.Null(@try.Finally);

            Assert.NotNull(@try.Handlers);
            Assert.Equal(1, @try.Handlers.Count);

            var handler = @try.Handlers[0];
            Assert.Same(expr.Handlers[0], handler);
        }

        [Fact]
        public static void Quote_CatchBlock_NoVariable_Closure2()
        {
            var x = Parameter(typeof(int));

            var expr =
                TryCatch(
                    Constant(0),
                    Catch(
                        typeof(Exception),
                        x
                    )
                );

            var f = BuildQuote<Func<int, LambdaExpression>, Func<int>>(expr, x);

            var quote = f.Compile()(42);

            Assert.Equal(0, quote.Parameters.Count);

            var @try = quote.Body as TryExpression;
            Assert.NotNull(@try);

            Assert.Same(expr.Body, @try.Body);
            Assert.Null(@try.Fault);
            Assert.Null(@try.Finally);

            Assert.NotNull(@try.Handlers);
            Assert.Equal(1, @try.Handlers.Count);

            var handler = @try.Handlers[0];
            Assert.Null(handler.Filter);
            Assert.Null(handler.Variable);
            AssertIsBox(@handler.Body, 42);
        }

        [Fact]
        public static void Quote_RuntimeVariables_Closure()
        {
            var x = Parameter(typeof(int));

            var expr =
                RuntimeVariables(
                    x
                );

            var f = BuildQuote<Func<int, Expression<Func<IRuntimeVariables>>>, Func<IRuntimeVariables>>(expr, x);

            var quote = f.Compile()(42);

            var vars = quote.Compile()();
            Assert.Equal(1, vars.Count);
            Assert.Equal(42, vars[0]);

            vars[0] = 43;
            Assert.Equal(43, vars[0]);
        }

        [Fact]
        public static void Quote_RuntimeVariables_Local()
        {
            var x = Parameter(typeof(int));

            var expr =
                Block(
                    new[] { x },
                    Assign(x, Constant(42)),
                    RuntimeVariables(
                        x
                    )
                );

            var f = BuildQuote<Func<Expression<Func<IRuntimeVariables>>>, Func<IRuntimeVariables>>(expr);

            var quote = f.Compile()();

            var vars = quote.Compile()();
            Assert.Equal(1, vars.Count);
            Assert.Equal(42, vars[0]);

            vars[0] = 43;
            Assert.Equal(43, vars[0]);
        }

        [Fact]
        public static void Quote_RuntimeVariables_ClosureAndLocal()
        {
            var x = Parameter(typeof(int));
            var y = Parameter(typeof(int));

            var expr =
                Block(
                    new[] { y },
                    Assign(y, Constant(2)),
                    RuntimeVariables(
                        x,
                        y
                    )
                );

            var f = BuildQuote<Func<int, Expression<Func<IRuntimeVariables>>>, Func<IRuntimeVariables>>(expr, x);

            var quote = f.Compile()(1);

            var vars = quote.Compile()();
            Assert.Equal(2, vars.Count);
            Assert.Equal(1, vars[0]);
            Assert.Equal(2, vars[1]);

            vars[0] = 3;
            vars[1] = 4;
            Assert.Equal(3, vars[0]);
            Assert.Equal(4, vars[1]);
        }

        private static void AssertIsBox<T>(Expression e, T value)
        {
            Assert.Equal(ExpressionType.MemberAccess, e.NodeType);

            var member = (MemberExpression)e;

            var field = member.Member as FieldInfo;
            Assert.NotNull(field);
            Assert.Equal(typeof(T), field.FieldType);
            Assert.Equal(typeof(StrongBox<T>), field.DeclaringType);

            var constant = member.Expression as ConstantExpression;
            Assert.NotNull(constant);

            var box = constant.Value as StrongBox<T>;
            Assert.NotNull(box);
            Assert.Equal(value, box.Value);
        }

        private static Expression<TDelegate> BuildQuote<TDelegate, TQuoteType>(Expression body, params ParameterExpression[] parameters)
        {
            var expr =
                Lambda<TDelegate>(
                    Call(
                        typeof(UnaryQuoteTests).GetMethod(nameof(GetQuote)).MakeGenericMethod(typeof(TQuoteType)),
                        Quote(
                            Lambda<TQuoteType>(body)
                        )
                    ),
                    parameters
                );

            return expr;
        }

        public static Expression<T> GetQuote<T>(Expression<T> e) => e;

        public static void Nop() { }
#endif
    }
}
