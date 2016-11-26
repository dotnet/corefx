// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    internal sealed class CSharpGetIndexBinder : GetIndexBinder
    {
        internal Type CallingContext { get { return _callingContext; } }
        private Type _callingContext;

        internal IList<CSharpArgumentInfo> ArgumentInfo { get { return _argumentInfo.AsReadOnly(); } }
        private readonly List<CSharpArgumentInfo> _argumentInfo;

        private readonly RuntimeBinder _binder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpGetIndexBinder" />.
        /// </summary>
        /// <param name="callingContext">The <see cref="System.Type"/> that indicates where this operation is defined.</param>
        /// <param name="argumentInfo">The sequence of <see cref="CSharpArgumentInfo"/> instances for the arguments to this operation.</param>
        public CSharpGetIndexBinder(
            Type callingContext,
            IEnumerable<CSharpArgumentInfo> argumentInfo) :
            base(BinderHelper.CreateCallInfo(argumentInfo, 1)) // discard 1 argument: the target object
        {
            _callingContext = callingContext;
            _argumentInfo = BinderHelper.ToList(argumentInfo);
            _binder = RuntimeBinder.GetInstance();
        }

        /// <summary>
        /// Performs the binding of the dynamic get index operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic get index operation.</param>
        /// <param name="indexes">The arguments of the dynamic get index operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion)
        {
#if ENABLECOMBINDER
            DynamicMetaObject com;
            if (!BinderHelper.IsWindowsRuntimeObject(target) && ComBinder.TryBindGetIndex(this, target, indexes, out com))
            {
                return com;
            }
#endif
            return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, indexes), _argumentInfo, errorSuggestion);
        }
    }
}
