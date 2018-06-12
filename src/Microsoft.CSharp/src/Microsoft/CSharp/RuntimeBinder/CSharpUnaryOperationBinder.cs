// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents a dynamic unary operation in C#, providing the binding semantics and the details about the operation.
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    internal sealed class CSharpUnaryOperationBinder : UnaryOperationBinder, ICSharpBinder
    {
        [ExcludeFromCodeCoverage]
        public string Name
        {
            get
            {
                Debug.Fail("Name should not be called for this binder");
                return null;
            }
        }


        public BindingFlag BindingFlags => 0;

        public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
            => runtimeBinder.BindUnaryOperation(this, arguments, locals);

        public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
            => SymbolTable.PopulateSymbolTableWithName(Operation.GetCLROperatorName(), null, arguments[0].Type);

        public bool IsBinderThatCanHaveRefReceiver => false;

        private readonly CSharpArgumentInfo[] _argumentInfo;

        CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index) => _argumentInfo[index];

        private readonly RuntimeBinder _binder;

        private readonly Type _callingContext;

        private bool IsChecked => _binder.IsChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpUnaryOperationBinder"/> class.
        /// </summary>
        /// <param name="operation">The unary operation kind.</param>
        /// <param name="isChecked">True if the operation is defined in a checked context; otherwise, false.</param>
        /// <param name="callingContext">The <see cref="Type"/> that indicates where this operation is defined.</param>
        /// <param name="argumentInfo">The sequence of <see cref="CSharpArgumentInfo"/> instances for the arguments to this operation.</param>
        public CSharpUnaryOperationBinder(
            ExpressionType operation,
            bool isChecked,
            Type callingContext,
            IEnumerable<CSharpArgumentInfo> argumentInfo) :
            base(operation)
        {
            _argumentInfo = BinderHelper.ToArray(argumentInfo);
            Debug.Assert(_argumentInfo.Length == 1);
            _callingContext = callingContext;
            _binder = new RuntimeBinder(callingContext, isChecked);
        }

        public int GetGetBinderEquivalenceHash()
        {
            int hash = _callingContext?.GetHashCode() ?? 0;
            hash = HashHelpers.Combine(hash, (int)Operation);
            if (IsChecked)
            {
                hash = HashHelpers.Combine(hash, 1);
            }
            hash = BinderHelper.AddArgHashes(hash, _argumentInfo);

            return hash;
        }

        public bool IsEquivalentTo(ICSharpBinder other)
        {
            var otherBinder = other as CSharpUnaryOperationBinder;
            if (otherBinder == null)
            {
                return false;
            }

            if (Operation != otherBinder.Operation ||
                IsChecked != otherBinder.IsChecked ||
                _callingContext != otherBinder._callingContext)
            {
                return false;
            }

            return BinderHelper.CompareArgInfos(_argumentInfo, otherBinder._argumentInfo);
        }

        /// <summary>
        /// Performs the binding of the unary dynamic operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic unary operation.</param>
        /// <param name="errorSuggestion">The binding result in case the binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            BinderHelper.ValidateBindArgument(target, nameof(target));
            return BinderHelper.Bind(this, _binder, new[] {target}, _argumentInfo, errorSuggestion);
        }
    }
}
