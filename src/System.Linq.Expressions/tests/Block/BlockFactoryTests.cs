﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Tests
{
    public static class BlockFactoryTests
    {
        [Fact]
        public static void CheckBlockFactoryOptimization2()
        {
            AssertBlockIsOptimized(2);
        }

        [Fact]
        public static void CheckBlockFactoryOptimization3()
        {
            AssertBlockIsOptimized(3);
        }

        [Fact]
        public static void CheckBlockFactoryOptimization4()
        {
            AssertBlockIsOptimized(4);
        }

        [Fact]
        public static void CheckBlockFactoryOptimization5()
        {
            AssertBlockIsOptimized(5);
        }

        private static void AssertBlockIsOptimized(int n)
        {
            // (Expression[]) overload
            {
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToArray();
                var expr = Expression.Block(args);

                AssertBlockIsOptimized(expr, args);
            }

            // (IEnumerable<Expression>) overload
            {
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToList();
                var expr = Expression.Block(args);

                AssertBlockIsOptimized(expr, args);
            }

            // (Type, Expression[]) overload
            {
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToArray();
                var expr = Expression.Block(args.Last().Type, args);

                AssertBlockIsOptimized(expr, args);
            }

            // (Type, IEnumerable<Expression>) overload
            {
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToList();
                var expr = Expression.Block(args.Last().Type, args);

                AssertBlockIsOptimized(expr, args);
            }

            // (IEnumerable<ParameterExpression>, Expression[]) overload
            {
                var vars = new ParameterExpression[0];
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToArray();
                var expr = Expression.Block(vars, args);

                AssertBlockIsOptimized(expr, args);
            }

            // (IEnumerable<ParameterExpression>, IEnumerable<Expression>) overload
            {
                var vars = new ParameterExpression[0];
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToList();
                var expr = Expression.Block(vars, args);

                AssertBlockIsOptimized(expr, args);
            }

            // (Type, IEnumerable<ParameterExpression>, Expression[]) overload
            {
                var vars = new ParameterExpression[0];
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToArray();
                var expr = Expression.Block(args.Last().Type, vars, args);

                AssertBlockIsOptimized(expr, args);
            }

            // (Type, IEnumerable<ParameterExpression>, IEnumerable<Expression>) overload
            {
                var vars = new ParameterExpression[0];
                var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i)).ToList();
                var expr = Expression.Block(args.Last().Type, vars, args);

                AssertBlockIsOptimized(expr, args);
            }
        }

        private static void AssertBlockIsOptimized(BlockExpression expr, IReadOnlyList<Expression> args)
        {
            var n = args.Count;

            var updated = Update(expr);
            var visited = Visit(expr);

            foreach (var node in new[] { expr, updated, visited })
            {
                AssertBlock(n, node);

                Assert.Equal(n, node.Expressions.Count);

                if (node != visited) // our visitor clones argument nodes
                {
                    for (var i = 0; i < n; i++)
                    {
                        Assert.Same(args[i], node.Expressions[i]);
                    }
                }
            }
        }

        private static void AssertBlock(int n, object obj)
        {
            AssertTypeName("Block" + n, obj);
        }

        private static void AssertTypeName(string expected, object obj)
        {
            Assert.Equal(expected, obj.GetType().Name);
        }

        private static BlockExpression Update(BlockExpression node)
        {
            // Tests the call of Update to Expression.Block factories.

            var res = node.Update(node.Variables, node.Expressions.ToArray());

            Assert.NotSame(node, res);

            return res;
        }

        private static BlockExpression Visit(BlockExpression node)
        {
            // Tests dispatch of ExpressionVisitor into Rewrite method which calls Expression.Block factories.

            return (BlockExpression)new Visitor().Visit(node);
        }

        class Visitor : ExpressionVisitor
        {
            protected override Expression VisitConstant(ConstantExpression node)
            {
                return Expression.Constant(node.Value, node.Type); // clones
            }
        }
    }
}
