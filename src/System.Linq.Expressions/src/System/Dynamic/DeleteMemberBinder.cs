// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <summary>
    /// Represents the dynamic delete member operation at the call site, providing the binding semantic and the details about the operation.
    /// </summary>
    public abstract class DeleteMemberBinder : DynamicMetaObjectBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMemberBinder" />.
        /// </summary>
        /// <param name="name">The name of the member to delete.</param>
        /// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
        protected DeleteMemberBinder(string name, bool ignoreCase)
        {
            ContractUtils.RequiresNotNull(name, nameof(name));

            Name = name;
            IgnoreCase = ignoreCase;
        }

        /// <summary>
        /// Gets the name of the member to delete.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value indicating if the string comparison should ignore the case of the member name.
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// The result type of the operation.
        /// </summary>
        public override sealed Type ReturnType => typeof(void);

        /// <summary>
        /// Performs the binding of the dynamic delete member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic delete member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackDeleteMember(DynamicMetaObject target)
        {
            return FallbackDeleteMember(target, null);
        }

        /// <summary>
        /// When overridden in the derived class, performs the binding of the dynamic delete member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic delete member operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackDeleteMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

        /// <summary>
        /// Performs the binding of the dynamic delete member operation.
        /// </summary>
        /// <param name="target">The target of the dynamic delete member operation.</param>
        /// <param name="args">An array of arguments of the dynamic delete member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.Requires(args == null || args.Length == 0, nameof(args));

            return target.BindDeleteMember(this);
        }

        /// <summary>
        /// Always returns <c>true</c> because this is a standard <see cref="DynamicMetaObjectBinder"/>.
        /// </summary>
        internal override sealed bool IsStandardBinder => true;
    }
}
