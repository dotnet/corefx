// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <summary>
    /// Represents the dynamic get member operation at the call site, providing the binding semantic and the details about the operation.
    /// </summary>
    public abstract class GetMemberBinder : DynamicMetaObjectBinder
    {
        private readonly string _name;
        private readonly bool _ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMemberBinder" />.
        /// </summary>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
        protected GetMemberBinder(string name, bool ignoreCase)
        {
            ContractUtils.RequiresNotNull(name, nameof(name));

            _name = name;
            _ignoreCase = ignoreCase;
        }

        /// <summary>
        /// The result type of the operation.
        /// </summary>
        public override sealed Type ReturnType
        {
            get { return typeof(object); }
        }

        /// <summary>
        /// Gets the name of the member to get.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the value indicating if the string comparison should ignore the case of the member name.
        /// </summary>
        public bool IgnoreCase
        {
            get
            {
                return _ignoreCase;
            }
        }

        /// <summary>
        /// Performs the binding of the dynamic get member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic get member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackGetMember(DynamicMetaObject target)
        {
            return FallbackGetMember(target, null);
        }

        /// <summary>
        /// When overridden in the derived class, performs the binding of the dynamic get member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic get member operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

        /// <summary>
        /// Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="target">The target of the dynamic get member operation.</param>
        /// <param name="args">An array of arguments of the dynamic get member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, params DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.Requires(args == null || args.Length == 0, nameof(args));

            return target.BindGetMember(this);
        }

        // this is a standard DynamicMetaObjectBinder
        internal override sealed bool IsStandardBinder
        {
            get
            {
                return true;
            }
        }
    }
}
