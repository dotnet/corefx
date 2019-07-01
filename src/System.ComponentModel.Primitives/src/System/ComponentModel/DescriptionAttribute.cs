// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    /// Specifies a description for a property or event.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Specifies the default value for the <see cref='System.ComponentModel.DescriptionAttribute'/>,
        /// which is an empty string (""). This <see langword='static'/> field is read-only.
        /// </summary>
        public static readonly DescriptionAttribute Default = new DescriptionAttribute();

        public DescriptionAttribute() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.DescriptionAttribute'/> class.
        /// </summary>
        public DescriptionAttribute(string description)
        {
            DescriptionValue = description;
        }

        /// <summary>
        /// Gets the description stored in this attribute.
        /// </summary>
        public virtual string Description => DescriptionValue;

        /// <summary>
        /// Read/Write property that directly modifies the string stored in the description
        /// attribute. The default implementation of the <see cref="Description"/> property
        /// simply returns this value.
        /// </summary>
        protected string DescriptionValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            DescriptionAttribute other = obj as DescriptionAttribute;
            return other != null && other.Description == Description;
        }

        public override int GetHashCode() => Description?.GetHashCode() ?? 0;

        public override bool IsDefaultAttribute() => Equals(Default);
    }
}
