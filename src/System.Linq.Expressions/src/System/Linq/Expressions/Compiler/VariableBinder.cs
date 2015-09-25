﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    /// Determines if variables are closed over in nested lambdas and need to
    /// be hoisted.
    /// </summary>
    internal sealed class VariableBinder : ExpressionVisitor
    {
        private readonly AnalyzedTree _tree = new AnalyzedTree();
        private readonly Stack<CompilerScope> _scopes = new Stack<CompilerScope>();
        private readonly Stack<BoundConstants> _constants = new Stack<BoundConstants>();
        private bool _inQuote;

        internal static AnalyzedTree Bind(LambdaExpression lambda)
        {
            var binder = new VariableBinder();
            binder.Visit(lambda);
            return binder._tree;
        }

        private VariableBinder()
        {
        }

        protected internal override Expression VisitConstant(ConstantExpression node)
        {
            // If we're in Quote, we can ignore constants completely
            if (_inQuote)
            {
                return node;
            }

            // Constants that can be emitted into IL don't need to be stored on
            // the delegate
            if (ILGen.CanEmitConstant(node.Value, node.Type))
            {
                return node;
            }

            _constants.Peek().AddReference(node.Value, node.Type);
            return node;
        }

        protected internal override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Quote)
            {
                bool savedInQuote = _inQuote;
                _inQuote = true;
                Visit(node.Operand);
                _inQuote = savedInQuote;
            }
            else
            {
                Visit(node.Operand);
            }
            return node;
        }

        protected internal override Expression VisitLambda<T>(Expression<T> node)
        {
            _scopes.Push(_tree.Scopes[node] = new CompilerScope(node, true));
            _constants.Push(_tree.Constants[node] = new BoundConstants());
            Visit(MergeScopes(node));
            _constants.Pop();
            _scopes.Pop();
            return node;
        }

        protected internal override Expression VisitInvocation(InvocationExpression node)
        {
            LambdaExpression lambda = node.LambdaOperand;

            // optimization: inline code for literal lambda's directly
            if (lambda != null)
            {
                // visit the lambda, but treat it like a scope associated with invocation
                _scopes.Push(_tree.Scopes[node] = new CompilerScope(lambda, false));
                Visit(MergeScopes(lambda));
                _scopes.Pop();
                // visit the invoke's arguments
                Visit(node.Arguments);
                return node;
            }

            return base.VisitInvocation(node);
        }

        protected internal override Expression VisitBlock(BlockExpression node)
        {
            if (node.Variables.Count == 0)
            {
                Visit(node.Expressions);
                return node;
            }
            _scopes.Push(_tree.Scopes[node] = new CompilerScope(node, false));
            Visit(MergeScopes(node));
            _scopes.Pop();
            return node;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            if (node.Variable == null)
            {
                Visit(node.Body);
                return node;
            }
            _scopes.Push(_tree.Scopes[node] = new CompilerScope(node, false));
            Visit(node.Body);
            _scopes.Pop();
            return node;
        }

        // If the immediate child is another scope, merge it into this one
        // This is an optimization to save environment allocations and
        // array accesses.
        private ReadOnlyCollection<Expression> MergeScopes(Expression node)
        {
            ReadOnlyCollection<Expression> body;
            var lambda = node as LambdaExpression;
            if (lambda != null)
            {
                body = new ReadOnlyCollection<Expression>(new[] { lambda.Body });
            }
            else
            {
                body = ((BlockExpression)node).Expressions;
            }

            var currentScope = _scopes.Peek();

            // A block body is mergeable if the body only contains one single block node containing variables,
            // and the child block has the same type as the parent block.
            while (body.Count == 1 && body[0].NodeType == ExpressionType.Block)
            {
                var block = (BlockExpression)body[0];

                if (block.Variables.Count > 0)
                {
                    // Make sure none of the variables are shadowed. If any
                    // are, we can't merge it.
                    foreach (var v in block.Variables)
                    {
                        if (currentScope.Definitions.ContainsKey(v))
                        {
                            return body;
                        }
                    }

                    // Otherwise, merge it
                    if (currentScope.MergedScopes == null)
                    {
                        currentScope.MergedScopes = new Set<object>(ReferenceEqualityComparer<object>.Instance);
                    }
                    currentScope.MergedScopes.Add(block);
                    foreach (var v in block.Variables)
                    {
                        currentScope.Definitions.Add(v, VariableStorageKind.Local);
                    }
                }
                node = block;
                body = block.Expressions;
            }
            return body;
        }


        protected internal override Expression VisitParameter(ParameterExpression node)
        {
            Reference(node, VariableStorageKind.Local);

            //
            // Track reference count so we can emit it in a more optimal way if
            // it is used a lot.
            //
            CompilerScope referenceScope = null;
            foreach (CompilerScope scope in _scopes)
            {
                //
                // There are two times we care about references:
                //   1. When we enter a lambda, we want to cache frequently
                //      used variables
                //   2. When we enter a scope with closed-over variables, we
                //      want to cache it immediately when we allocate the
                //      closure slot for it
                //
                if (scope.IsMethod || scope.Definitions.ContainsKey(node))
                {
                    referenceScope = scope;
                    break;
                }
            }

            Debug.Assert(referenceScope != null);
            if (referenceScope.ReferenceCount == null)
            {
                referenceScope.ReferenceCount = new Dictionary<ParameterExpression, int>();
            }

            Helpers.IncrementCount(node, referenceScope.ReferenceCount);
            return node;
        }

        protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            foreach (var v in node.Variables)
            {
                // Force hoisting of these variables
                Reference(v, VariableStorageKind.Hoisted);
            }
            return node;
        }

        private void Reference(ParameterExpression node, VariableStorageKind storage)
        {
            CompilerScope definition = null;
            foreach (CompilerScope scope in _scopes)
            {
                if (scope.Definitions.ContainsKey(node))
                {
                    definition = scope;
                    break;
                }
                scope.NeedsClosure = true;
                if (scope.IsMethod)
                {
                    storage = VariableStorageKind.Hoisted;
                }
            }
            if (definition == null)
            {
                throw Error.UndefinedVariable(node.Name, node.Type, CurrentLambdaName);
            }
            if (storage == VariableStorageKind.Hoisted)
            {
                if (node.IsByRef)
                {
                    throw Error.CannotCloseOverByRef(node.Name, CurrentLambdaName);
                }
                definition.Definitions[node] = VariableStorageKind.Hoisted;
            }
        }

        private string CurrentLambdaName
        {
            get
            {
                foreach (var scope in _scopes)
                {
                    var lambda = scope.Node as LambdaExpression;
                    if (lambda != null)
                    {
                        return lambda.Name;
                    }
                }
                throw ContractUtils.Unreachable;
            }
        }
    }
}
