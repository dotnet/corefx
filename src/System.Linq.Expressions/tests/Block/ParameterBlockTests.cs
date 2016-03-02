// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class ParameterBlockTests : SharedBlockTests
    {
        private static IEnumerable<ParameterExpression> SingleParameter
        {
            get { return Enumerable.Repeat(Expression.Variable(typeof(int)), 1); }
        }

        [Theory]
        [MemberData(nameof(ConstantValueData))]
        public void SingleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                SingleParameter,
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData(nameof(ConstantValueData))]
        public void DoubleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                SingleParameter,
                Expression.Empty(),
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Fact]
        public void NullExpicitType()
        {
            Assert.Throws<ArgumentNullException>("type", () => Expression.Block(null, SingleParameter, Expression.Constant(0)));
            Assert.Throws<ArgumentNullException>("type", () => Expression.Block(null, SingleParameter, Enumerable.Repeat(Expression.Constant(0), 1)));
        }

        [Fact]
        public void NullExpressionList()
        {
            Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(SingleParameter, default(Expression[])));
            Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(SingleParameter, default(IEnumerable<Expression>)));
            Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(typeof(int), SingleParameter, default(Expression[])));
            Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(typeof(int), SingleParameter, default(IEnumerable<Expression>)));
        }

        [Theory]
        [MemberData(nameof(BlockSizes))]
        public void NullExpressionInExpressionList(int size)
        {
            List<Expression> expressionList = Enumerable.Range(0, size).Select(i => (Expression)Expression.Constant(1)).ToList();
            for (int i = 0; i < expressionList.Count; ++i)
            {
                Expression[] expressions = expressionList.ToArray();
                expressions[i] = null;
                Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(SingleParameter, expressions));
                Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(SingleParameter, expressions.Skip(0)));
                Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(typeof(int), SingleParameter, expressions));
                Assert.Throws<ArgumentNullException>("expressions", () => Expression.Block(typeof(int), SingleParameter, expressions.Skip(0)));
            }
        }

        [Theory]
        [MemberData(nameof(BlockSizes))]
        public void UnreadableExpressionInExpressionList(int size)
        {
            List<Expression> expressionList = Enumerable.Range(0, size).Select(i => (Expression)Expression.Constant(1)).ToList();
            for (int i = 0; i != expressionList.Count; ++i)
            {
                Expression[] expressions = expressionList.ToArray();
                expressions[i] = UnreadableExpression;
                Assert.Throws<ArgumentException>("expressions", () => Expression.Block(SingleParameter, expressions));
                Assert.Throws<ArgumentException>("expressions", () => Expression.Block(SingleParameter, expressions.Skip(0)));
                Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(int), SingleParameter, expressions));
                Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(int), SingleParameter, expressions.Skip(0)));
            }
        }

        [Theory]
        [MemberData(nameof(ObjectAssignableConstantValuesAndSizes))]
        public void BlockExplicitType(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            BlockExpression block = Expression.Block(typeof(object), SingleParameter, PadBlock(blockSize - 1, constant));

            Assert.Equal(typeof(object), block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData(nameof(BlockSizes))]
        public void BlockInvalidExplicitType(int blockSize)
        {
            ConstantExpression constant = Expression.Constant(0);
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, Expression.Constant(0));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(string), SingleParameter, expressions));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(string), SingleParameter, expressions.ToArray()));
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void BlockFromEmptyParametersSameAsFromParams(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression fromParamsBlock = Expression.Block(SingleParameter, expressions.ToArray());
            BlockExpression fromEnumBlock = Expression.Block(SingleParameter, expressions);

            Assert.Equal(fromParamsBlock.GetType(), fromEnumBlock.GetType());

            Assert.True(Expression.Lambda<Func<bool>>(Expression.Equal(constant, fromParamsBlock)).Compile()());
            Assert.True(Expression.Lambda<Func<bool>>(Expression.Equal(constant, fromEnumBlock)).Compile()());
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void InvalidExpressionIndex(object value, int blockSize)
        {
            BlockExpression block = Expression.Block(SingleParameter, PadBlock(blockSize - 1, Expression.Constant(value, value.GetType())));
            Assert.Throws<ArgumentOutOfRangeException>("index", () => block.Expressions[-1]);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => block.Expressions[blockSize]);
        }

        [Fact]
        public void EmptyBlockWithParametersAndNonVoidTypeNotAllowed()
        {
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(int), SingleParameter));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(int), SingleParameter, Enumerable.Empty<Expression>()));
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void ResultPropertyFromParams(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(SingleParameter, expressions.ToArray());

            Assert.Same(constant, block.Result);
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void ResultPropertyFromEnumerable(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(SingleParameter, expressions);

            Assert.Same(constant, block.Result);
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void VariableCountCorrect(object value, int blockSize)
        {
            IEnumerable<ParameterExpression> vars = Enumerable.Range(0, blockSize).Select(i => Expression.Variable(value.GetType()));
            BlockExpression block = Expression.Block(vars, Expression.Constant(value, value.GetType()));

            Assert.Equal(blockSize, block.Variables.Count);
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        [ActiveIssue(3883)]
        public void RewriteToSameWithSameValues(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant).ToArray();

            BlockExpression block = Expression.Block(SingleParameter, expressions);
            Assert.Same(block, block.Update(block.Variables.ToArray(), expressions));
            Assert.Same(block, block.Update(block.Variables.ToArray(), expressions));
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void CanFindItems(object value, int blockSize)
        {
            ConstantExpression[] values = new ConstantExpression[blockSize];
            for (int i = 0; i != values.Length; ++i)
                values[i] = Expression.Constant(value);

            BlockExpression block = Expression.Block(SingleParameter, values);

            IList<Expression> expressions = block.Expressions;

            for (int i = 0; i != values.Length; ++i)
                Assert.Equal(i, expressions.IndexOf(values[i]));
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void IdentifyNonAbsentItemAsAbsent(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(SingleParameter, expressions);

            Assert.Equal(-1, block.Expressions.IndexOf(Expression.Default(typeof(long))));
            Assert.False(block.Expressions.Contains(null));
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void ExpressionsEnumerable(object value, int blockSize)
        {
            ConstantExpression[] values = new ConstantExpression[blockSize];
            for (int i = 0; i != values.Length; ++i)
                values[i] = Expression.Constant(value);

            BlockExpression block = Expression.Block(SingleParameter, values);

            Assert.True(values.SequenceEqual(block.Expressions));
            int index = 0;
            foreach (Expression exp in ((IEnumerable)block.Expressions))
                Assert.Same(exp, values[index++]);
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void UpdateWithExpressionsReturnsSame(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(SingleParameter, expressions);

            Assert.Same(block, block.Update(block.Variables, block.Expressions));
        }

        [Theory]
        [MemberData(nameof(ConstantValuesAndSizes))]
        public void Visit(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(SingleParameter, expressions);

            Assert.NotSame(block, new TestVistor().Visit(block));
        }

        [Theory]
        [MemberData(nameof(ObjectAssignableConstantValuesAndSizes))]
        public void VisitTyped(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(typeof(object), SingleParameter, expressions);

            Assert.NotSame(block, new TestVistor().Visit(block));
        }

        [Theory]
        [MemberData(nameof(BlockSizes))]
        public void NullVariables(int blockSize)
        {
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, Expression.Constant(0));
            IEnumerable<ParameterExpression> vars = Enumerable.Repeat(default(ParameterExpression), 1);

            Assert.Throws<ArgumentNullException>(() => Expression.Block(vars, expressions));
            Assert.Throws<ArgumentNullException>(() => Expression.Block(vars, expressions.ToArray()));
            Assert.Throws<ArgumentNullException>(() => Expression.Block(typeof(object), vars, expressions));
            Assert.Throws<ArgumentNullException>(() => Expression.Block(typeof(object), vars, expressions.ToArray()));
        }

        [Theory]
        [MemberData(nameof(BlockSizes))]
        public void ByRefVariables(int blockSize)
        {
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, Expression.Constant(0));
            IEnumerable<ParameterExpression> vars = Enumerable.Repeat(Expression.Parameter(typeof(int).MakeByRefType()), 1);

            Assert.Throws<ArgumentException>(() => Expression.Block(vars, expressions));
            Assert.Throws<ArgumentException>(() => Expression.Block(vars, expressions.ToArray()));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(object), vars, expressions));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(object), vars, expressions.ToArray()));
        }

        [Theory]
        [MemberData(nameof(BlockSizes))]
        public void RepeatedVariables(int blockSize)
        {
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, Expression.Constant(0));
            ParameterExpression variable = Expression.Variable(typeof(int));
            IEnumerable<ParameterExpression> vars = Enumerable.Repeat(variable, 2);

            Assert.Throws<ArgumentException>(() => Expression.Block(vars, expressions));
            Assert.Throws<ArgumentException>(() => Expression.Block(vars, expressions.ToArray()));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(object), vars, expressions));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(object), vars, expressions.ToArray()));
        }
    }
}
