// Copyright (c) Jon Hanna. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Expressions.Test
{
    public class BlockTests
    {
        [Fact]
        public void SingleElementBlock()
        {
            var block = Expression.Block(
                Expression.Constant(42, typeof(int))
               );

            Assert.Equal(typeof(int), block.Type);
            Assert.Equal(42, Expression.Lambda<Func<int>>(block).Compile()());
        }

        [Fact]
        public void DoubleElementBlock()
        {
            var block = Expression.Block(
                Expression.Empty(),
                Expression.Constant(42, typeof(int))
               );

            Assert.Equal(typeof(int), block.Type);
            Assert.Equal(42, Expression.Lambda<Func<int>>(block).Compile()());
        }

        [Fact]
        public void TripleElementBlock()
        {
            var block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Constant(42, typeof(int))
               );

            Assert.Equal(typeof(int), block.Type);
            Assert.Equal(42, Expression.Lambda<Func<int>>(block).Compile()());
        }

        [Fact]
        public void QuadrupleElementBlock()
        {
            var block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Constant(42, typeof(int))
               );

            Assert.Equal(typeof(int), block.Type);
            Assert.Equal(42, Expression.Lambda<Func<int>>(block).Compile()());
        }

        [Fact]
        public void QuintupleElementBlock()
        {
            var block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Constant(42, typeof(int))
               );

            Assert.Equal(typeof(int), block.Type);
            Assert.Equal(42, Expression.Lambda<Func<int>>(block).Compile()());
        }

        [Fact]
        public void SextupleElementBlock()
        {
            var block = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Constant(42, typeof(int))
               );

            Assert.Equal(typeof(int), block.Type);
            Assert.Equal(42, Expression.Lambda<Func<int>>(block).Compile()());
        }
        
        private static IEnumerable<Expression> PadBlock(int padCount, Expression tailExpression)
        {
            while (padCount-- != 0) yield return Expression.Empty();
            yield return tailExpression;
        }
        
        [Fact]
        public void BlockFromEnumerableSameAsFromParams()
        {
            var tailExp = Expression.Constant(42, typeof(int));
            var directBlock = Expression.Block(
                tailExp
               );
            var fromEnumBlock = Expression.Block(PadBlock(0, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(1, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(2, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(3, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(4, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(5, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(6, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());
        }

        [Fact]
        public void BlockFromEmptyParametersSameAsFromParams()
        {
            var emptyParams = new ParameterExpression[0];
            
            var tailExp = Expression.Constant(42, typeof(int));
            var directBlock = Expression.Block(
                emptyParams,
                tailExp
               );
            var fromEnumBlock = Expression.Block(PadBlock(0, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                emptyParams,
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(1, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                emptyParams,
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(2, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                emptyParams,
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(3, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                emptyParams,
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(4, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                emptyParams,
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(5, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());

            directBlock = Expression.Block(
                emptyParams,
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                Expression.Empty(),
                tailExp
               );
            fromEnumBlock = Expression.Block(PadBlock(6, tailExp));
            Assert.Equal(directBlock.GetType(), fromEnumBlock.GetType());
            Assert.Equal(Expression.Lambda<Func<int>>(directBlock).Compile()(), Expression.Lambda<Func<int>>(fromEnumBlock).Compile()());
        }
    }
}