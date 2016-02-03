// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <devdoc>
    /// <para>Provides a type converter to convert <see cref='System.Decimal'/>
    /// objects to and from various
    /// other representations.</para>
    /// </devdoc>
    public class DecimalConverter : BaseNumberConverter
    {
        /// <devdoc>
        /// Determines whether this editor will attempt to convert hex (0x or #) strings
        /// </devdoc>
        internal override bool AllowHex
        {
            get
            {
                return false;
            }
        }

        /// <devdoc>
        /// The Type this converter is targeting (e.g. Int16, UInt32, etc.)
        /// </devdoc>
        internal override Type TargetType
        {
            get
            {
                return typeof(Decimal);
            }
        }

        /// <devdoc>
        /// Convert the given value to a string using the given radix
        /// </devdoc>
        internal override object FromString(string value, int radix)
        {
            return Convert.ToDecimal(value, CultureInfo.CurrentCulture);
        }

        /// <devdoc>
        /// Convert the given value to a string using the given formatInfo
        /// </devdoc>
        internal override object FromString(string value, NumberFormatInfo formatInfo)
        {
            return Decimal.Parse(value, NumberStyles.Float, formatInfo);
        }


        /// <devdoc>
        /// Convert the given value to a string using the given CultureInfo
        /// </devdoc>
        internal override object FromString(string value, CultureInfo culture)
        {
            return Decimal.Parse(value, culture);
        }

        /// <devdoc>
        /// Convert the given value from a string using the given formatInfo
        /// </devdoc>
        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            return ((Decimal)value).ToString("G", formatInfo);
        }
    }
}

