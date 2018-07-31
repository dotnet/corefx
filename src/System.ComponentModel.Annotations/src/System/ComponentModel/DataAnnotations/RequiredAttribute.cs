// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Validation attribute to indicate that a property field or parameter is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class RequiredAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <remarks>
        ///     This constructor selects a reasonable default error message for
        ///     <see cref="ValidationAttribute.FormatErrorMessage" />
        /// </remarks>
        public RequiredAttribute()
            : base(() => SR.RequiredAttribute_ValidationError)
        {
        }

        /// <summary>
        ///     Gets or sets a flag indicating whether the attribute should allow empty strings.
        /// </summary>
        public bool AllowEmptyStrings { get; set; }

        /// <summary>
        ///     Gets or sets a flag indicating whether the attribute should allow empty collections.
        /// </summary>
        public bool AllowEmptyCollections { get; set; }

        /// <summary>
        ///     Override of <see cref="ValidationAttribute.IsValid(object)" />
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>
        ///     <c>false</c> if the <paramref name="value" /> is null, an empty string or an empty collection.
        ///     Unless <see cref="RequiredAttribute.AllowEmptyStrings" /> or 
        ///     <see cref="RequiredAttribute.AllowEmptyCollections"/>
        ///     then <c>false</c> is returned only if <paramref name="value" /> is null.
        /// </returns>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string stringValue)
            {
                return AllowEmptyStrings || stringValue.Trim().Length != 0;
            }

            if (value is ICollection collectionValue)
            {
                return AllowEmptyCollections || collectionValue.Count() != 0;
            }

            return true;
        }
    }
}
