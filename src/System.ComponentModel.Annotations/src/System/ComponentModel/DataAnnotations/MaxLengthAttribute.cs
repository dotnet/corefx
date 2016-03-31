// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Specifies the maximum length of collection/string data allowed in a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class MaxLengthAttribute : ValidationAttribute
    {
        private const int MaxAllowableLength = -1;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MaxLengthAttribute" /> class.
        /// </summary>
        /// <param name="length">
        ///     The maximum allowable length of collection/string data.
        ///     Value must be greater than zero.
        /// </param>
        public MaxLengthAttribute(int length)
            : base(() => DefaultErrorMessageString)
        {
            Length = length;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MaxLengthAttribute" /> class.
        ///     The maximum allowable length supported by the database will be used.
        /// </summary>
        public MaxLengthAttribute()
            : base(() => DefaultErrorMessageString)
        {
            Length = MaxAllowableLength;
        }

        /// <summary>
        ///     Gets the maximum allowable length of the collection/string data.
        /// </summary>
        public int Length { get; private set; }

        private static string DefaultErrorMessageString
        {
            get { return SR.MaxLengthAttribute_ValidationError; }
        }

        /// <summary>
        ///     Determines whether a specified object is valid. (Overrides <see cref="ValidationAttribute.IsValid(object)" />)
        /// </summary>
        /// <remarks>
        ///     This method returns <c>true</c> if the <paramref name="value" /> is null.
        ///     It is assumed the <see cref="RequiredAttribute" /> is used if the value may not be null.
        /// </remarks>
        /// <param name="value">The object to validate.</param>
        /// <returns>
        ///     <c>true</c> if the value is null or less than or equal to the specified maximum length, otherwise <c>false</c>
        /// </returns>
        /// <exception cref="InvalidOperationException">Length is zero or less than negative one.</exception>
        public override bool IsValid(object value)
        {
            // Check the lengths for legality
            EnsureLegalLengths();

            var length = 0;
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }
            var str = value as string;
            if (str != null)
            {
                length = str.Length;
            }
            else
            {
                ICollection collection = value as ICollection;

                if (collection != null)
                {
                    length = collection.Count;
                }
                else
                {
                    // A cast exception previously occurred if a non-{string|array} property was passed
                    // in so preserve this behavior if the value does not implement ICollection
                    length = ((Array)value).Length;
                }
            }

            return MaxAllowableLength == Length || length <= Length;
        }

        /// <summary>
        ///     Applies formatting to a specified error message. (Overrides <see cref="ValidationAttribute.FormatErrorMessage" />)
        /// </summary>
        /// <param name="name">The name to include in the formatted string.</param>
        /// <returns>A localized string to describe the maximum acceptable length.</returns>
        public override string FormatErrorMessage(string name)
        {
            // An error occurred, so we know the value is greater than the maximum if it was specified
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
        }

        /// <summary>
        ///     Checks that Length has a legal value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Length is zero or less than negative one.</exception>
        private void EnsureLegalLengths()
        {
            if (Length == 0 || Length < -1)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    SR.MaxLengthAttribute_InvalidMaxLength));
            }
        }
    }
}
