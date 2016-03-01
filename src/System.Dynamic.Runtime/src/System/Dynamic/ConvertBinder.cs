// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <summary>
    /// Represents the convert dynamic operation at the call site, providing the binding semantic and the details about the operation.
    /// </summary>
    public abstract class ConvertBinder : DynamicMetaObjectBinder
    {
        private readonly Type _type;
        private readonly bool _explicit;

        /// <summary>
        /// Initializes a new intsance of the <see cref="ConvertBinder" />.
        /// </summary>
        /// <param name="type">The type to convert to.</param>
        /// <param name="explicit">true if the conversion should consider explicit conversions; otherwise, false.</param>
        protected ConvertBinder(Type type, bool @explicit)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            _type = type;
            _explicit = @explicit;
        }

        /// <summary>
        /// The type to convert to.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Gets the value indicating if the conversion should consider explicit conversions.
        /// </summary>
        public bool Explicit
        {
            get
            {
                return _explicit;
            }
        }

        /// <summary>
        /// Performs the binding of the dynamic convert operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackConvert(DynamicMetaObject target)
        {
            return FallbackConvert(target, null);
        }

        /// <summary>
        /// When overridden in the derived class, performs the binding of the dynamic convert operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion);

        /// <summary>
        /// Performs the binding of the dynamic convert operation.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <param name="args">An array of arguments of the dynamic convert operation.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.Requires(args == null || args.Length == 0, nameof(args));

            return target.BindConvert(this);
        }

        // this is a standard DynamicMetaObjectBinder
        internal override sealed bool IsStandardBinder
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The result type of the operation.
        /// </summary>
        public override sealed Type ReturnType
        {
            get { return _type; }
        }
    }
}
