// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     Allows overriding various display-related options for a given field. The options have the same meaning as in
    ///     BoundField.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DisplayFormatAttribute : Attribute
    {
        private readonly LocalizableString nullDisplayText = new LocalizableString("NulLDisplayText");
        private Type resourceType;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public DisplayFormatAttribute()
        {
            ConvertEmptyStringToNull = true; // default to true to match behavior in related components

            HtmlEncode = true; // default to true to match behavior in related components
        }

        /// <summary>
        ///     Gets or sets the format string
        /// </summary>
        public string DataFormatString { get; set; }


        /// <summary>
        ///     Gets or sets the string to display when the value is null, which may be a resource key string.
        ///     <para>
        ///         Consumers must use the <see cref="GetNullDisplayText" /> method to retrieve the UI display string.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     The property contains either the literal, non-localized string or the resource key
        ///     to be used in conjunction with <see cref="NullDisplayTextResourceType" /> to configure a localized
        ///     name for display.
        ///     <para>
        ///         The <see cref="GetNullDisplayText" /> method will return either the literal, non-localized
        ///         string or the localized string when <see cref="NullDisplayTextResourceType" /> has been specified.
        ///     </para>
        /// </remarks>
        /// <value>
        ///     The null dispay text is generally used as placeholder when the value is not specified.
        ///     A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        public string NullDisplayText
        {
            get { return nullDisplayText.Value; }
            set
            {
                if (nullDisplayText.Value != value)
                {
                    nullDisplayText.Value = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether empty strings should be set to null
        /// </summary>
        public bool ConvertEmptyStringToNull { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the format string should be used in edit mode
        /// </summary>
        public bool ApplyFormatInEditMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the field should be html encoded
        /// </summary>
        public bool HtmlEncode { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="System.Type" /> that contains the resources for <see cref="NullDisplayText" />.
        ///     Using <see cref="NullDisplayTextResourceType" /> along with this property, allows the <see cref="GetNullDisplayText" />
        ///     method to return localized values.
        /// </summary>
        public Type NullDisplayTextResourceType
        {
            get { return resourceType; }
            set
            {
                if (resourceType != value)
                {
                    resourceType = value;
                    nullDisplayText.ResourceType = value;
                }
            }
        }

        /// <summary>
        ///     Gets the UI display string for NullDisplayText.
        ///     <para>
        ///         This can be either a literal, non-localized string provided to <see cref="NullDisplayText" /> or the
        ///         localized string found when <see cref="NullDisplayTextResourceType" /> has been specified and <see cref="NullDisplayText" />
        ///         represents a resource key within that resource type.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     When <see cref="NullDisplayTextResourceType" /> has not been specified, the value of
        ///     <see cref="NullDisplayText" /> will be returned.
        ///     <para>
        ///         When <see cref="NullDisplayTextResourceType" /> has been specified and <see cref="NullDisplayText" />
        ///         represents a resource key within that resource type, then the localized value will be returned.
        ///     </para>
        ///     <para>
        ///         Can return <c>null</c> and will not fall back onto other values, as it's more likely for the
        ///         consumer to want to fall back onto the property name.
        ///     </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     After setting both the <see cref="NullDisplayTextResourceType" /> property and the <see cref="NullDisplayText" /> property,
        ///     but a public static property with a name matching the <see cref="NullDisplayText" /> value couldn't be found
        ///     on the <see cref="NullDisplayTextResourceType" />.
        /// </exception>
        public string GetNullDisplayText()
        {
            return nullDisplayText.GetLocalizableValue();
        }
    }
}
