// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    ///  Specifies that a object has no sub properties that are editable.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ImmutableObjectAttribute : Attribute
    {
        /// <summary>
        ///  Specifies that a object has no sub properties that are editable.
        ///
        ///  This is usually used in the properties window to determine if an expandable object
        ///  should be rendered as read-only.
        /// </summary>
        public static readonly ImmutableObjectAttribute Yes = new ImmutableObjectAttribute(true);

        /// <summary>
        ///  Specifies that a object has at least one editable sub-property.
        ///
        ///  This is usually used in the properties window to determine if an expandable object
        ///  should be rendered as read-only.
        /// </summary>
        public static readonly ImmutableObjectAttribute No = new ImmutableObjectAttribute(false);


        /// <summary>
        ///  Defaults to ImmutableObjectAttribute.No
        /// </summary>
        public static readonly ImmutableObjectAttribute Default = No;

        /// <summary>
        ///  Constructs an ImmutableObjectAttribute object.
        ///
        /// </summary>
        public ImmutableObjectAttribute(bool immutable)
        {
            Immutable = immutable;
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public bool Immutable { get; }

        /// <internalonly/>
        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            ImmutableObjectAttribute other = obj as ImmutableObjectAttribute;
            return other != null && other.Immutable == Immutable;
        }

        /// <summary>
        ///    <para>
        ///       Returns the hashcode for this object.
        ///    </para>
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
