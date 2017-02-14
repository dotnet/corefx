﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class ListInitExpressionTests
    {
        private class NonEnumerableAddable
        {
            public readonly List<int> Store = new List<int>();

            public void Add(int value)
            {
                Store.Add(value);
            }
        }

        private class EnumerableStaticAdd : IEnumerable<string>
        {
            public IEnumerator<string> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()  => GetEnumerator();

            public static void Add(string value)
            {
            }
        }

        [Fact]
        public void NullNewMethod()
        {
            ConstantExpression validExpression = Expression.Constant(1);
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, validExpression));
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, Enumerable.Repeat(validExpression, 1)));

            MethodInfo validMethod = typeof(List<int>).GetMethod("Add");
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, validMethod, validExpression));
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, validMethod, Enumerable.Repeat(validExpression, 1)));

            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, default(MethodInfo), validExpression));
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, null, Enumerable.Repeat(validExpression, 1)));

            ElementInit validElementInit = Expression.ElementInit(validMethod, validExpression);
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, validElementInit));
            Assert.Throws<ArgumentNullException>("newExpression", () => Expression.ListInit(null, Enumerable.Repeat(validElementInit, 1)));
        }

        [Fact]
        public void NullInitializers()
        {
            NewExpression validNew = Expression.New(typeof(List<int>));
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, default(Expression[])));
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, default(IEnumerable<Expression>)));
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, default(ElementInit[])));
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, default(IEnumerable<ElementInit>)));

            MethodInfo validMethod = typeof(List<int>).GetMethod("Add");
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, validMethod, default(Expression[])));
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, validMethod, default(IEnumerable<Expression>)));

            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, null, default(Expression[])));
            Assert.Throws<ArgumentNullException>("initializers", () => Expression.ListInit(validNew, null, default(IEnumerable<Expression>)));
        }

        private static IEnumerable<object[]> ZeroInitializerInits()
        {
            NewExpression validNew = Expression.New(typeof(List<int>));
            yield return new object[] {Expression.ListInit(validNew, new Expression[0])};
            yield return new object[] {Expression.ListInit(validNew, Enumerable.Empty<Expression>())};
            yield return new object[] {Expression.ListInit(validNew, new ElementInit[0])};
            yield return new object[] {Expression.ListInit(validNew, Enumerable.Empty<ElementInit>())};

            MethodInfo validMethod = typeof(List<int>).GetMethod("Add");
            yield return new object[] {Expression.ListInit(validNew, validMethod)};
            yield return new object[] {Expression.ListInit(validNew, validMethod, Enumerable.Empty<Expression>())};

            yield return new object[] {Expression.ListInit(validNew, null, new Expression[0])};
            yield return new object[] {Expression.ListInit(validNew, null, Enumerable.Empty<Expression>())};
        }

        [Theory, PerCompilationType(nameof(ZeroInitializerInits))]
        public void ZeroInitializers(Expression init, bool useInterpreter)
        {
            Expression<Func<List<int>>> exp = Expression.Lambda<Func<List<int>>>(init);
            Func<List<int>> func = exp.Compile(useInterpreter);
            Assert.Empty(func());
        }

        [Fact]
        public void TypeWithoutAdd()
        {
            NewExpression newExp = Expression.New(typeof(string).GetConstructor(new[] { typeof(char[]) }), Expression.Constant("aaaa".ToCharArray()));
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(newExp, Expression.Constant('a')));
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(newExp, Enumerable.Repeat(Expression.Constant('a'), 1)));
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(newExp, default(MethodInfo), Expression.Constant('a')));
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(newExp, default(MethodInfo), Enumerable.Repeat(Expression.Constant('a'), 1)));
        }

        [Fact]
        public void InitializeNonEnumerable()
        {
            // () => new NonEnumerableAddable { 1, 2, 4, 16, 42 } isn't allowed because list initialization
            // is allowed only with enumerable types.
            Assert.Throws<ArgumentException>("newExpression", () => Expression.ListInit(Expression.New(typeof(NonEnumerableAddable)), Expression.Constant(1)));
        }

        [Fact]
        public void StaticAddMethodOnType()
        {
            NewExpression newExp = Expression.New(typeof(EnumerableStaticAdd));
            MethodInfo adder = typeof(EnumerableStaticAdd).GetMethod(nameof(EnumerableStaticAdd.Add));

            // this exception behavior (rather than ArgumentException) is compatible with the .NET Framework
            Assert.Throws<InvalidOperationException>(() => Expression.ListInit(newExp, Expression.Constant("")));

            Assert.Throws<ArgumentException>("addMethod", () => Expression.ListInit(newExp, adder, Expression.Constant("")));
            Assert.Throws<ArgumentException>("addMethod", () => Expression.ElementInit(adder, Expression.Constant("")));
            Assert.Throws<ArgumentException>("addMethod", () => Expression.ElementInit(adder, Enumerable.Repeat(Expression.Constant(""), 1)));
        }

        [Fact]
        public void InitializersWrappedExactly()
        {
            NewExpression newExp = Expression.New(typeof(List<int>));
            ConstantExpression[] expressions = new[] { Expression.Constant(1), Expression.Constant(2), Expression.Constant(int.MaxValue) };
            ListInitExpression listInit = Expression.ListInit(newExp, expressions);
            Assert.Equal(expressions, listInit.Initializers.Select(i => i.GetArgument(0)));
        }

        [Fact]
        public void CanReduce()
        {
            ListInitExpression listInit = Expression.ListInit(
                Expression.New(typeof(List<int>)),
                Expression.Constant(0)
                );
            Assert.True(listInit.CanReduce);
            Assert.NotSame(listInit, listInit.ReduceAndCheck());
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void InitializeVoidAdd(bool useInterpreter)
        {
            Expression<Func<List<int>>> listInit = () => new List<int> { 1, 2, 4, 16, 42 };
            Func<List<int>> func = listInit.Compile(useInterpreter);
            Assert.Equal(new[] { 1, 2, 4, 16, 42 }, func());
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void InitializeNonVoidAdd(bool useInterpreter)
        {
            Expression<Func<HashSet<int>>> hashInit = () => new HashSet<int> { 1, 2, 4, 16, 42 };
            Func<HashSet<int>> func = hashInit.Compile(useInterpreter);
            Assert.Equal(new[] { 1, 2, 4, 16, 42 }, func().OrderBy(i => i));
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void InitializeTwoParameterAdd(bool useInterpreter)
        {
            Expression<Func<Dictionary<string, int>>> dictInit = () => new Dictionary<string, int>
            {
                { "a", 1 }, {"b", 2 }, {"c", 3 }
            };
            Func<Dictionary<string, int>> func = dictInit.Compile(useInterpreter);
            var expected = new Dictionary<string, int>
            {
                { "a", 1 }, {"b", 2 }, {"c", 3 }
            };
            Assert.Equal(expected.OrderBy(kvp => kvp.Key), func().OrderBy(kvp => kvp.Key));
        }

        [Fact]
        public void UpdateSameReturnsSame()
        {
            ListInitExpression init = Expression.ListInit(
                Expression.New(typeof(List<int>)),
                Expression.Constant(1),
                Expression.Constant(2),
                Expression.Constant(3));
            Assert.Same(init, init.Update(init.NewExpression, init.Initializers.ToArray()));
        }

        [Fact]
        public void UpdateNullThrows()
        {
            ListInitExpression init = Expression.ListInit(
                Expression.New(typeof(List<int>)),
                Expression.Constant(1),
                Expression.Constant(2),
                Expression.Constant(3));
            Assert.Throws<ArgumentNullException>("newExpression", () => init.Update(null, init.Initializers));
            Assert.Throws<ArgumentNullException>("initializers", () => init.Update(init.NewExpression, null));
        }

        [Fact]
        public void UpdateDifferentNewReturnsDifferent()
        {
            ListInitExpression init = Expression.ListInit(
                Expression.New(typeof(List<int>)),
                Expression.Constant(1),
                Expression.Constant(2),
                Expression.Constant(3));
            Assert.NotSame(init, init.Update(Expression.New(typeof(List<int>)), init.Initializers));
        }

        [Fact]
        public void UpdateDifferentInitializersReturnsDifferent()
        {
            MethodInfo meth = typeof(List<int>).GetMethod("Add");
            ElementInit[] inits = new[]
            {
                Expression.ElementInit(meth, Expression.Constant(1)),
                Expression.ElementInit(meth, Expression.Constant(2)),
                Expression.ElementInit(meth, Expression.Constant(3))
            };
            ListInitExpression init = Expression.ListInit(Expression.New(typeof(List<int>)), inits);
            inits = new[]
            {
                Expression.ElementInit(meth, Expression.Constant(1)),
                Expression.ElementInit(meth, Expression.Constant(2)),
                Expression.ElementInit(meth, Expression.Constant(3))
            };
            Assert.NotSame(init, init.Update(init.NewExpression, inits));
        }

        [Fact]
        public void UpdateDoesntRepeatEnumeration()
        {
            MethodInfo meth = typeof(List<int>).GetMethod("Add");
            ElementInit[] inits = new[]
            {
                Expression.ElementInit(meth, Expression.Constant(1)),
                Expression.ElementInit(meth, Expression.Constant(2)),
                Expression.ElementInit(meth, Expression.Constant(3))
            };
            ListInitExpression init = Expression.ListInit(Expression.New(typeof(List<int>)), inits);
            IEnumerable<ElementInit> newInits = new RunOnceEnumerable<ElementInit>(
                new[]
                {
                    Expression.ElementInit(meth, Expression.Constant(1)),
                    Expression.ElementInit(meth, Expression.Constant(2)),
                    Expression.ElementInit(meth, Expression.Constant(3))
                });
            Assert.NotSame(init, init.Update(init.NewExpression, newInits));
        }

        [Fact]
        public static void ToStringTest()
        {
            ListInitExpression e1 = Expression.ListInit(Expression.New(typeof(List<int>)), Expression.Parameter(typeof(int), "x"));
            Assert.Equal("new List`1() {Void Add(Int32)(x)}", e1.ToString());

            ListInitExpression e2 = Expression.ListInit(Expression.New(typeof(List<int>)), Expression.Parameter(typeof(int), "x"), Expression.Parameter(typeof(int), "y"));
            Assert.Equal("new List`1() {Void Add(Int32)(x), Void Add(Int32)(y)}", e2.ToString());
        }
    }
}
