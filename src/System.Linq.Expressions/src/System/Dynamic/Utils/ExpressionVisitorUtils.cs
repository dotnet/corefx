// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    internal static class ExpressionVisitorUtils
    {
        public static Expression[] VisitBlockExpressions(ExpressionVisitor visitor, BlockExpression block)
        {
            for (int i = 0, n = block.ExpressionCount; i < n; i++)
            {
                Expression curNode = block.GetExpression(i);
                Expression node = visitor.Visit(curNode);

                if (node != curNode)
                {
                    Expression[] newNodes = new Expression[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = block.GetExpression(j);
                    }

                    newNodes[i] = node;
                    while (++i < n)
                    {
                        newNodes[i] = visitor.Visit(block.GetExpression(i));
                    }

                    return newNodes;
                }
            }

            return null;
        }

        public static ParameterExpression[] VisitParameters(ExpressionVisitor visitor, IParameterProvider nodes, string callerName)
        {
            for (int i = 0, n = nodes.ParameterCount; i < n; i++)
            {
                ParameterExpression curNode = nodes.GetParameter(i);
                ParameterExpression node = visitor.VisitAndConvert(curNode, callerName);

                if (node != curNode)
                {
                    ParameterExpression[] newNodes = new ParameterExpression[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes.GetParameter(j);
                    }

                    newNodes[i] = node;
                    while (++i < n)
                    {
                        newNodes[i] = visitor.VisitAndConvert(nodes.GetParameter(i), callerName);
                    }

                    return newNodes;
                }
            }

            return null;
        }

        public static Expression[] VisitArguments(ExpressionVisitor visitor, IArgumentProvider nodes)
        {
            for (int i = 0, n = nodes.ArgumentCount; i < n; i++)
            {
                Expression curNode = nodes.GetArgument(i);
                Expression node = visitor.Visit(curNode);

                if (node != curNode)
                {
                    Expression[] newNodes = new Expression[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes.GetArgument(j);
                    }

                    newNodes[i] = node;
                    while (++i < n)
                    {
                        newNodes[i] = visitor.Visit(nodes.GetArgument(i));
                    }

                    return newNodes;
                }

            }

            return null;
        }
    }
}
