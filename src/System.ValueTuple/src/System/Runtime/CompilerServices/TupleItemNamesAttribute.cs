// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that the use of <see cref="System.ValueTuple"/> on a member is meant to be treated as a tuple with item names.
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Event )]
    public sealed class TupleItemNamesAttribute : Attribute
    {
        private readonly string[] _itemNames;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="TupleItemNamesAttribute"/> class.
        /// </summary>
        /// <param name="itemNames">
        /// Specifies, in a pre-order depth-first traversal of a type's
        /// construction, which <see cref="System.ValueType"/> occurrences are
        /// meant to carry item names.
        /// </param>
        /// <remarks>
        /// This constructor is meant to be used on types that contain an
        /// instantiation of <see cref="System.ValueType"/> that contains
        /// item names.  For instance, if <c>C</c> is a generic type with
        /// two type parameters, then a use of the constructed type <c>C{<see
        /// cref="System.ValueTuple{T1, T2}"/>, <see
        /// cref="System.ValueTuple{T1, T2, T3}"/></c> might be intended to
        /// treat the first type argument as a tuple with item names and the
        /// second as a tuple without item names. In which case, the
        /// appropriate attribute specification should use a
        /// <c>itemNames</c> value of <c>{ "name1", "name2", null, null,
        /// null }</c>.
        /// </remarks>
        public TupleItemNamesAttribute(string[] itemNames)
        {
            if (itemNames == null)
            {
                throw new ArgumentNullException(nameof(itemNames));
            }

            _itemNames = itemNames;
        }

        /// <summary>
        /// Specifies, in a pre-order depth-first traversal of a type's
        /// construction, which <see cref="System.ValueTuple"/> items are
        /// meant to carry item names.
        /// </summary>
        public IList<string> ItemNames => _itemNames;
    }
}
