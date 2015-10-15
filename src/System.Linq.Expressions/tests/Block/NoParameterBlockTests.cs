﻿// Copyright (c) Jon Hanna. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Expressions.Test
{
    public class NoParameterBlockTests : BlockTests
    {
        [Theory]
        [MemberData("ConstantValueData")]
        public void SingleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("ConstantValueData")]
        public void DoubleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                Expression.Empty(),
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("ConstantValueData")]
        public void TripleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("ConstantValueData")]
        public void QuadrupleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("ConstantValueData")]
        public void QuintupleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("ConstantValueData")]
        public void SextupleElementBlock(object value)
        {
            Type type = value.GetType();
            ConstantExpression constant = Expression.Constant(value, type);
            BlockExpression block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                constant
               );

            Assert.Equal(type, block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("ObjectAssignableConstantValuesAndSizes")]
        public void BlockExplicitType(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            BlockExpression block = Expression.Block(typeof(object), PadBlock(blockSize - 1, constant));

            Assert.Equal(typeof(object), block.Type);

            Expression equal = Expression.Equal(constant, block);
            Assert.True(Expression.Lambda<Func<bool>>(equal).Compile()());
        }

        [Theory]
        [MemberData("BlockSizes")]
        public void BlockInvalidExplicitType(int blockSize)
        {
            ConstantExpression constant = Expression.Constant(0);
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, Expression.Constant(0));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(string), expressions));
            Assert.Throws<ArgumentException>(() => Expression.Block(typeof(string), expressions.ToArray()));
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        [ActiveIssue(3881)]
        public void InvalidExpressionIndex(object value, int blockSize)
        {
            BlockExpression block = Expression.Block(PadBlock(blockSize - 1, Expression.Constant(value, value.GetType())));
            Assert.Throws<ArgumentOutOfRangeException>(() => block.Expressions[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => block.Expressions[blockSize]);
        }

        // Remove below if issue blocking above is fixed.
        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void InvalidExpressionIndexVaryingExceptin(object value, int blockSize)
        {
            BlockExpression block = Expression.Block(PadBlock(blockSize - 1, Expression.Constant(value, value.GetType())));
            Assert.ThrowsAny<Exception>(() => block.Expressions[-1]);
            Assert.ThrowsAny<Exception>(() => block.Expressions[blockSize]);
        }

        // See https://github.com/dotnet/corefx/issues/3043
        [Fact]
        public void EmptyBlockNotAllowed()
        {
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block());
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(void)));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(Enumerable.Empty<Expression>()));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(void), Enumerable.Empty<Expression>()));
        }

        [Fact]
        [ActiveIssue(3882)]
        public void EmptyBlockWithParametersNotAllowed()
        {
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(Enumerable.Repeat<ParameterExpression>(Expression.Parameter(typeof(int)), 1)));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(Enumerable.Repeat<ParameterExpression>(Expression.Parameter(typeof(int)), 1), Enumerable.Empty<Expression>()));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(void), Enumerable.Repeat<ParameterExpression>(Expression.Parameter(typeof(int)), 1)));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(void), Enumerable.Repeat<ParameterExpression>(Expression.Parameter(typeof(int)), 1), Enumerable.Empty<Expression>()));
        }

        // If https://github.com/dotnet/corefx/issues/3043 is ever actioned, this case would still be prohibited.

        [Fact]
        public void EmptyBlockWithNonVoidTypeNotAllowed()
        {
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(int)));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(int), Enumerable.Empty<Expression>()));
        }

        [Fact]
        [ActiveIssue(3882)]
        public void EmptyBlockWithParametersAndNonVoidTypeNotAllowed()
        {
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(int), Enumerable.Repeat<ParameterExpression>(Expression.Parameter(typeof(int)), 1)));
            Assert.Throws<ArgumentException>("expressions", () => Expression.Block(typeof(int), Enumerable.Repeat<ParameterExpression>(Expression.Parameter(typeof(int)), 1), Enumerable.Empty<Expression>()));
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void ResultPropertyFromParams(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions.ToArray());

            Assert.Same(constant, block.Result);
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void ResultPropertyFromEnumerable(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions);

            Assert.Same(constant, block.Result);
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void VariableCountZeroOnNonVariableAcceptingForms(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions);

            Assert.Empty(block.Variables);
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        [ActiveIssue(3883)]
        public void RewriteToSameWithSameValues(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions);
            Assert.Same(block, block.Update(null, expressions));
            Assert.Same(block, block.Update(Enumerable.Empty<ParameterExpression>(), expressions));
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void CanFindItems(object value, int blockSize)
        {
            ConstantExpression[] values = new ConstantExpression[blockSize];
            for (int i = 0; i != values.Length; ++i)
                values[i] = Expression.Constant(value);

            BlockExpression block = Expression.Block(values);

            IList<Expression> expressions = block.Expressions;

            for (int i = 0; i != values.Length; ++i)
                Assert.Equal(i, expressions.IndexOf(values[i]));
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void IdentifyNonAbsentItemAsAbsent(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions);

            Assert.Equal(-1, block.Expressions.IndexOf(Expression.Default(typeof(long))));
            Assert.False(block.Expressions.Contains(null));
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void ExpressionsEnumerable(object value, int blockSize)
        {
            ConstantExpression[] values = new ConstantExpression[blockSize];
            for (int i = 0; i != values.Length; ++i)
                values[i] = Expression.Constant(value);

            BlockExpression block = Expression.Block(values);

            Assert.True(values.SequenceEqual(block.Expressions));
            int index = 0;
            foreach (Expression exp in ((IEnumerable)block.Expressions))
                Assert.Same(exp, values[index++]);
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void UpdateWithExpressionsReturnsSame(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions);

            Assert.Same(block, block.Update(block.Variables, block.Expressions));
        }

        [Theory]
        [MemberData("ConstantValuesAndSizes")]
        public void Visit(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(expressions);

            Assert.NotSame(block, new TestVistor().Visit(block));
        }

        [Theory]
        [MemberData("ObjectAssignableConstantValuesAndSizes")]
        public void VisitTyped(object value, int blockSize)
        {
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            IEnumerable<Expression> expressions = PadBlock(blockSize - 1, constant);

            BlockExpression block = Expression.Block(typeof(object), expressions);

            Assert.NotSame(block, new TestVistor().Visit(block));
        }
    }
}
