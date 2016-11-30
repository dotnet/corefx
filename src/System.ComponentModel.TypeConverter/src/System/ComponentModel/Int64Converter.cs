// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    ///    <para>Provides a type converter to convert 64-bit signed integer objects to and
    ///       from various other representations.</para>
    /// </summary>
    public class Int64Converter : BaseNumberConverter
    {
        /// <summary>
        /// The Type this converter is targeting (e.g. Int16, UInt32, etc.)
        /// </summary>
        internal override Type TargetType => typeof(Int64);

        /// <summary>
        /// Convert the given value to a string using the given radix
        /// </summary>
        internal override object FromString(string value, int radix)
        {
            return Convert.ToInt64(value, radix);
        }

        /// <summary>
        /// Convert the given value to a string using the given formatInfo
        /// </summary>
        internal override object FromString(string value, NumberFormatInfo formatInfo)
        {
            return Int64.Parse(value, NumberStyles.Integer, formatInfo);
        }


        /// <summary>
        /// Convert the given value to a string using the given CultureInfo
        /// </summary>
        internal override object FromString(string value, CultureInfo culture)
        {
            return Int64.Parse(value, culture);
        }

        /// <summary>
        /// Convert the given value from a string using the given formatInfo
        /// </summary>
        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            return ((Int64)value).ToString("G", formatInfo);
        }
    }
}

