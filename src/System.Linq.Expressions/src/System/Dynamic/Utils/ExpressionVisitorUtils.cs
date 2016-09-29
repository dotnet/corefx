﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    internal static partial class ExpressionVisitorUtils
    {
        public static Expression[] VisitBlockExpressions(ExpressionVisitor visitor, BlockExpression block)
        {
            Expression[] newNodes = null;
            for (int i = 0, n = block.ExpressionCount; i < n; i++)
            {
                Expression curNode = block.GetExpression(i);
                Expression node = visitor.Visit(curNode);

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!object.ReferenceEquals(node, curNode))
                {
                    newNodes = new Expression[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = block.GetExpression(j);
                    }
                    newNodes[i] = node;
                }
            }
            return newNodes;
        }
    }
}
