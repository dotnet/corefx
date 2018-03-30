// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents a dynamic method call in C#, providing the binding semantics and the details about the operation.
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    internal sealed class CSharpInvokeMemberBinder : InvokeMemberBinder, ICSharpInvokeOrInvokeMemberBinder
    {
        public BindingFlag BindingFlags => 0;

        public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
            => runtimeBinder.DispatchPayload(this, arguments, locals);

        public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
            => RuntimeBinder.PopulateSymbolTableWithPayloadInformation(this, callingType, arguments);

        public bool IsBinderThatCanHaveRefReceiver => true;

        bool ICSharpInvokeOrInvokeMemberBinder.StaticCall => _argumentInfo[0]?.IsStaticType == true;

        public CSharpCallFlags Flags { get; }

        public Type CallingContext { get; }

        public Type[] TypeArguments { get; }

        private readonly CSharpArgumentInfo[] _argumentInfo;

        public CSharpArgumentInfo GetArgumentInfo(int index) => _argumentInfo[index];

        public CSharpArgumentInfo[] ArgumentInfoArray()
        {
            CSharpArgumentInfo[] array = new CSharpArgumentInfo[_argumentInfo.Length];
            _argumentInfo.CopyTo(array, 0);
            return array;
        }

        bool ICSharpInvokeOrInvokeMemberBinder.ResultDiscarded => (Flags & CSharpCallFlags.ResultDiscarded) != 0;

        private readonly RuntimeBinder _binder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpInvokeMemberBinder" />.
        /// </summary>
        /// <param name="flags">Extra information about this operation that is not specific to any particular argument.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="callingContext">The <see cref="System.Type"/> that indicates where this operation is defined.</param>
        /// <param name="typeArguments">The list of user-specified type arguments to this call.</param>
        /// <param name="argumentInfo">The sequence of <see cref="CSharpArgumentInfo"/> instances for the arguments to this operation.</param>
        public CSharpInvokeMemberBinder(
                CSharpCallFlags flags,
                string name,
                Type callingContext,
                IEnumerable<Type> typeArguments,
                IEnumerable<CSharpArgumentInfo> argumentInfo) :
            base(name, false, BinderHelper.CreateCallInfo(ref argumentInfo, 1)) // discard 1 argument: the target object (even if static, arg is type)
        {
            Flags = flags;
            CallingContext = callingContext;
            TypeArguments = BinderHelper.ToArray(typeArguments);
            _argumentInfo = BinderHelper.ToArray(argumentInfo);
            _binder = new RuntimeBinder(callingContext);
        }

        public int BinderEqivalenceHash
        {
            get
            {
                int hash = CallingContext?.GetHashCode() ?? 0;
                hash = HashHelpers.Combine(hash, (int)Flags);
                hash = HashHelpers.Combine(hash, Name.GetHashCode());

                hash = BinderHelper.AddArgHashes(hash, TypeArguments, _argumentInfo);

                return hash;
            }
        }

        public bool IsEquivalentTo(ICSharpBinder other)
        {
            var otherBinder = other as CSharpInvokeMemberBinder;
            if (otherBinder == null)
            {
                return false;
            }

            if (Flags != otherBinder.Flags ||
                CallingContext != otherBinder.CallingContext ||
                Name != otherBinder.Name ||
                TypeArguments.Length != otherBinder.TypeArguments.Length ||
                _argumentInfo.Length != otherBinder._argumentInfo.Length)
            {
                return false;
            }

            return BinderHelper.CompareArgInfos(TypeArguments, otherBinder.TypeArguments, _argumentInfo, otherBinder._argumentInfo);
        }

        /// <summary>
        /// Performs the binding of the dynamic invoke member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic invoke member operation.</param>
        /// <param name="args">The arguments of the dynamic invoke member operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
        {
#if ENABLECOMBINDER
            DynamicMetaObject com;
            if (!BinderHelper.IsWindowsRuntimeObject(target) && ComBinder.TryBindInvokeMember(this, target, args, out com))
            {
                return com;
            }
#endif
            BinderHelper.ValidateBindArgument(target, nameof(target));
            BinderHelper.ValidateBindArgument(args, nameof(args));
            return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, args), _argumentInfo, errorSuggestion);
        }

        /// <summary>
        /// Performs the binding of the dynamic invoke operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic invoke operation.</param>
        /// <param name="args">The arguments of the dynamic invoke operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
        {
            CSharpInvokeBinder c = new CSharpInvokeBinder(Flags, CallingContext, _argumentInfo).TryGetExisting();
            return c.Defer(target, args);
        }
    }
}
