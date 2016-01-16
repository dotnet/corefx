﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static partial class ExpressionVisitorTests
    {
        [Fact]
        public static void ExpressionVisitor_NoCloning()
        {
            var v = new NoOpVisitor();

            foreach (var e in ExpressionCatalog.Expressions.Where(e => e.NodeType != ExpressionType.Extension))
            {
                Assert.Same(e, v.Visit(e));
            }
        }

        class NoOpVisitor : ExpressionVisitor
        {
        }
    }
}
