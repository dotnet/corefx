﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class LoopTests
    {
        private static class Unreadable<T>
        {
            public static T WriteOnly
            {
                set { }
            }
        }

        private class IntegralException : Exception
        {
            public IntegralException(int number)
            {
                Number = number;
            }

            public int Number { get; }
        }

        [Fact]
        public void NullBody()
        {
            Assert.Throws<ArgumentNullException>("body", () => Expression.Loop(null));
            Assert.Throws<ArgumentNullException>("body", () => Expression.Loop(null, null));
            Assert.Throws<ArgumentNullException>("body", () => Expression.Loop(null, null, null));
        }

        [Fact]
        public void UnreadableBody()
        {
            Expression body = Expression.Property(null, typeof(Unreadable<int>), nameof(Unreadable<int>.WriteOnly));
            Assert.Throws<ArgumentException>("body", () => Expression.Loop(body));
            Assert.Throws<ArgumentException>("body", () => Expression.Loop(body, null));
            Assert.Throws<ArgumentException>("body", () => Expression.Loop(body, null, null));
        }

        [Fact]
        public void NonVoidContinue()
        {
            Assert.Throws<ArgumentException>("continue", () => Expression.Loop(Expression.Empty(), null, Expression.Label(typeof(int))));
        }

        [Fact]
        public void TypeWithoutBreakIsVoid()
        {
            Assert.Equal(typeof(void), Expression.Loop(Expression.Constant(3)).Type);
            Assert.Equal(typeof(void), Expression.Loop(Expression.Constant(3), null).Type);
            Assert.Equal(typeof(void), Expression.Loop(Expression.Constant(3), null, null).Type);
        }

        [Fact]
        public void TypeIsBreaksType()
        {
            LabelTarget voidLabelTarget = Expression.Label();
            Assert.Equal(typeof(void), Expression.Loop(Expression.Constant(3), voidLabelTarget).Type);
            Assert.Equal(typeof(void), Expression.Loop(Expression.Constant(3), voidLabelTarget, null).Type);

            LabelTarget int32LabelTarget = Expression.Label(typeof(int));
            Assert.Equal(typeof(int), Expression.Loop(Expression.Empty(), int32LabelTarget).Type);
            Assert.Equal(typeof(int), Expression.Loop(Expression.Empty(), int32LabelTarget).Type);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void BreakWithinLoop(bool useInterpreter)
        {
            string labelName = "Not likely to appear for any other reason {E90FAF9D-1934-4FC9-93EB-BCE70B586146}";
            LabelTarget @break = Expression.Label(labelName);
            Expression<Action> lambda = Expression.Lambda<Action>(Expression.Loop(Expression.Label(@break), @break));
            Assert.Contains(labelName, Assert.Throws<InvalidOperationException>(() => lambda.Compile(useInterpreter)).Message);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void ContinueWithinLoop(bool useInterpreter)
        {
            string labelName = "Not likely to appear for any other reason {F9C549FE-6E6C-44A2-A434-0147E0D49F7F}";
            LabelTarget @continue = Expression.Label(labelName);
            Expression<Action> lambda = Expression.Lambda<Action>(Expression.Loop(Expression.Label(@continue), null, @continue));
            Assert.Throws<InvalidOperationException>(() => lambda.Compile());
            Assert.Contains(labelName, Assert.Throws<InvalidOperationException>(() => lambda.Compile(useInterpreter)).Message);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void BreakOutsideLoop(bool useInterpreter)
        {
            string labelName = "Not likely to appear for any other reason {D3C6FCD8-EA2F-440B-938F-C81560C3BDBA}";
            LabelTarget @break = Expression.Label(labelName);
            Expression<Action> lambda = Expression.Lambda<Action>(
                Expression.Block(
                    Expression.Label(@break),
                    Expression.Loop(Expression.Empty(), @break)
                    )
                );
            Assert.Contains(labelName, Assert.Throws<InvalidOperationException>(() => lambda.Compile(useInterpreter)).Message);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void ContinueOutsideLoop(bool useInterpreter)
        {
            string labelName = "Not likely to appear for any other reason {1107D64D-9FC4-4533-83E2-0F5F78B48315}";
            LabelTarget @continue = Expression.Label(labelName);
            Expression<Action> lambda = Expression.Lambda<Action>(
                Expression.Block(
                    Expression.Label(@continue),
                    Expression.Loop(Expression.Empty(), null, @continue)
                    )
                );
            Assert.Contains(labelName, Assert.Throws<InvalidOperationException>(() => lambda.Compile(useInterpreter)).Message);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void ContinueTheSameAsBreak(bool useInterpreter)
        {
            string labelName = "Not likely to appear for any other reason {B9CD9CF5-6C67-41C9-98C0-F445CAAB5082}";
            LabelTarget label = Expression.Label(labelName);
            Expression<Action> lambda = Expression.Lambda<Action>(
                Expression.Loop(Expression.Empty(), label, label)
                );
            Assert.Contains(labelName, Assert.Throws<InvalidOperationException>(() => lambda.Compile(useInterpreter)).Message);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void NoLabelsInfiniteLoop(bool useInterpreter)
        {
            // Have an error condition so the otherwise-infinite loop can complete.
            ParameterExpression num = Expression.Variable(typeof(int));
            Action spinThenThrow = Expression.Lambda<Action>(
                Expression.Block(
                    new[] {num},
                    Expression.Assign(num, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.IfThen(
                            Expression.GreaterThan(
                                Expression.PreIncrementAssign(num),
                                Expression.Constant(19)
                                ),
                            Expression.Throw(
                                Expression.New(
                                    typeof(IntegralException).GetConstructor(new[] {typeof(int)}),
                                    num
                                    )
                                )
                            )
                        )
                    )
                ).Compile(useInterpreter);

            Assert.Equal(20, Assert.Throws<IntegralException>(spinThenThrow).Number);
        }

        public void NoBreakToLabelInfiniteLoop(bool useInterpreter)
        {
            // Have an error condition so the otherwise-infinite loop can complete.
            ParameterExpression num = Expression.Variable(typeof(int));
            Func<int> spinThenThrow = Expression.Lambda<Func<int>>(
                Expression.Block(
                    new[] { num },
                    Expression.Assign(num, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.IfThen(
                            Expression.GreaterThan(
                                Expression.PreIncrementAssign(num),
                                Expression.Constant(19)
                                ),
                            Expression.Throw(
                                Expression.New(
                                    typeof(IntegralException).GetConstructor(new[] { typeof(int) }),
                                    num
                                    )
                                )
                            ),
                        Expression.Label(typeof(int))
                        )
                    )
                ).Compile(useInterpreter);

            Assert.Equal(20, Assert.Throws<IntegralException>(() => spinThenThrow()).Number);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void ExplicitContinue(bool useInterpreter)
        {
            var builder = new StringBuilder();
            var value = Expression.Variable(typeof(int));
            var @break = Expression.Label();
            var @continue = Expression.Label();
            var append = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] {typeof(int)});
            Action act = Expression.Lambda<Action>(
                Expression.Block(
                    new[] {value},
                    Expression.Assign(value, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.Block(
                            Expression.PostIncrementAssign(value),
                            Expression.IfThen(
                                Expression.GreaterThanOrEqual(value, Expression.Constant(10)),
                                Expression.Break(@break)
                                ),
                            Expression.IfThen(
                                Expression.Equal(
                                    Expression.Modulo(value, Expression.Constant(2)),
                                    Expression.Constant(0)
                                    ),
                                Expression.Continue(@continue)
                                ),
                            Expression.Call(Expression.Constant(builder), append, value)
                            ),
                        @break,
                        @continue
                        )
                    )
                ).Compile(useInterpreter);

            act();
            Assert.Equal("13579", builder.ToString());
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void LoopWithBreak(bool useInterpreter)
        {
            ParameterExpression value = Expression.Parameter(typeof(int));
            ParameterExpression result = Expression.Variable(typeof(int));
            LabelTarget label = Expression.Label(typeof(int));
            BlockExpression block = Expression.Block(
                new[] {result},
                Expression.Assign(result, Expression.Constant(1)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThan(value, Expression.Constant(1)),
                        Expression.MultiplyAssign(result,
                            Expression.PostDecrementAssign(value)),
                        Expression.Break(label, result)
                        ),
                    label
                    )
                );

            Func<int, int> factorial = Expression.Lambda<Func<int, int>>(block, value).Compile(useInterpreter);
            Assert.Equal(120, factorial(5));
        }

        [Fact]
        public void CannotReduce()
        {
            var loop = Expression.Loop(Expression.Empty(), Expression.Label(), Expression.Label());
            Assert.False(loop.CanReduce);
            Assert.Same(loop, loop.Reduce());
            Assert.Throws<ArgumentException>(null, () => loop.ReduceAndCheck());
        }

        [Fact]
        public void UpdateSameIsSame()
        {
            var loop = Expression.Loop(Expression.Empty(), Expression.Label(), Expression.Label());
            Assert.Same(loop, loop.Update(loop.BreakLabel, loop.ContinueLabel, loop.Body));
        }

        [Fact]
        public void UpdateDifferentBodyIsDifferent()
        {
            var loop = Expression.Loop(Expression.Empty(), Expression.Label(), Expression.Label());
            Assert.NotSame(loop, loop.Update(loop.BreakLabel, loop.ContinueLabel, Expression.Empty()));
        }

        [Fact]
        public void UpdateDifferentBreakIsDifferent()
        {
            var loop = Expression.Loop(Expression.Empty(), Expression.Label(), Expression.Label());
            Assert.NotSame(loop, loop.Update(Expression.Label(), loop.ContinueLabel, loop.Body));
        }

        [Fact]
        public void UpdateDifferentContinueIsDifferent()
        {
            var loop = Expression.Loop(Expression.Empty(), Expression.Label(), Expression.Label());
            Assert.NotSame(loop, loop.Update(loop.BreakLabel, Expression.Label(), loop.Body));
        }
    }
}
