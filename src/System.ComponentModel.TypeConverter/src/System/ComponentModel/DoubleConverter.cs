// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <devdoc>
    ///    <para>Provides a type
    ///       converter to convert double-precision, floating point number objects to and from various
    ///       other representations.</para>
    /// </devdoc>
    public class DoubleConverter : BaseNumberConverter
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
                return typeof(Double);
            }
        }

        /// <devdoc>
        /// Convert the given value to a string using the given radix
        /// </devdoc>
        internal override object FromString(string value, int radix)
        {
            return Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }

        /// <devdoc>
        /// Convert the given value to a string using the given formatInfo
        /// </devdoc>
        internal override object FromString(string value, NumberFormatInfo formatInfo)
        {
            return Double.Parse(value, NumberStyles.Float, formatInfo);
        }


        /// <devdoc>
        /// Convert the given value to a string using the given CultureInfo
        /// </devdoc>
        internal override object FromString(string value, CultureInfo culture)
        {
            return Double.Parse(value, culture);
        }

        /// <devdoc>
        /// Convert the given value from a string using the given formatInfo
        /// </devdoc>
        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            return ((Double)value).ToString("R", formatInfo);
        }
    }
}

