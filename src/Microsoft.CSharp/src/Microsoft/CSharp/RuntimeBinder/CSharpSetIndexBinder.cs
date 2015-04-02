// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents a dynamic indexer access in C#, providing the binding semantics and the details about the operation. 
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    internal sealed class CSharpSetIndexBinder : SetIndexBinder
    {
        internal bool IsCompoundAssignment { get { return _bIsCompoundAssignment; } }
        private bool _bIsCompoundAssignment;

        internal bool IsChecked { get { return _isChecked; } }
        private bool _isChecked;

        internal Type CallingContext { get { return _callingContext; } }
        private Type _callingContext;

        internal IList<CSharpArgumentInfo> ArgumentInfo { get { return _argumentInfo.AsReadOnly(); } }
        private List<CSharpArgumentInfo> _argumentInfo;

        private RuntimeBinder _binder;

        //////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpSetIndexBinder" />.
        /// </summary>
        /// <param name="isCompoundAssignment">True if the assignment comes from a compound assignment in source.</param>
        /// <param name="callingContext">The <see cref="System.Type"/> that indicates where this operation is defined.</param>
        /// <param name="argumentInfo">The sequence of <see cref="CSharpArgumentInfo"/> instances for the arguments to this operation.</param>
        public CSharpSetIndexBinder(
            bool isCompoundAssignment,
            bool isChecked,
            Type callingContext,
            IEnumerable<CSharpArgumentInfo> argumentInfo) :
            base(BinderHelper.CreateCallInfo(argumentInfo, 2)) // discard 2 arguments: the target object and the value
        {
            _bIsCompoundAssignment = isCompoundAssignment;
            _isChecked = isChecked;
            _callingContext = callingContext;
            _argumentInfo = BinderHelper.ToList(argumentInfo);
            _binder = RuntimeBinder.GetInstance();
        }

        /// <summary>
        /// Performs the binding of the dynamic set index operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic set index operation.</param>
        /// <param name="indexes">The arguments of the dynamic set index operation.</param>
        /// <param name="value">The value to set to the collection.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject FallbackSetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
#if ENABLECOMBINDER
            DynamicMetaObject com;
            if (!BinderHelper.IsWindowsRuntimeObject(target) && ComBinder.TryBindSetIndex(this, target, indexes, value, out com))
            {
                return com;
            }
#endif
            return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, indexes, value), _argumentInfo, errorSuggestion);
        }
    }
}
