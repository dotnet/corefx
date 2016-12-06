// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents a dynamic unary operation in C#, providing the binding semantics and the details about the operation. 
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    internal sealed class CSharpUnaryOperationBinder : UnaryOperationBinder
    {
        internal bool IsChecked { get; }

        internal Type CallingContext { get; }

        internal IList<CSharpArgumentInfo> ArgumentInfo { get { return _argumentInfo.AsReadOnly(); } }
        private readonly List<CSharpArgumentInfo> _argumentInfo;

        private readonly RuntimeBinder _binder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpUnaryOperationBinder"/> class.
        /// </summary>
        /// <param name="operation">The unary operation kind.</param>
        /// <param name="isChecked">True if the operation is defined in a checked context; otherwise, false.</param>
        /// <param name="argumentInfo">The sequence of <see cref="CSharpArgumentInfo"/> instances for the arguments to this operation.</param>
        public CSharpUnaryOperationBinder(
            ExpressionType operation,
            bool isChecked,
            Type callingContext,
            IEnumerable<CSharpArgumentInfo> argumentInfo) :
            base(operation)
        {
            IsChecked = isChecked;
            CallingContext = callingContext;
            _argumentInfo = BinderHelper.ToList(argumentInfo);
            Debug.Assert(_argumentInfo.Count == 1);
            _binder = RuntimeBinder.GetInstance();
        }

        /// <summary>
        /// Performs the binding of the unary dynamic operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic unary operation.</param>
        /// <param name="errorSuggestion">The binding result in case the binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, null), _argumentInfo, errorSuggestion);
        }
    }
}
