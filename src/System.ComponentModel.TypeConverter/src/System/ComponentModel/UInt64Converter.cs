// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert 64-bit unsigned integer objects to and
    /// from various other representations.
    /// </summary>
    public class UInt64Converter : BaseNumberConverter
    {
        /// <summary>
        /// The Type this converter is targeting (e.g. Int16, UInt64, etc.)
        /// </summary>
        internal override Type TargetType => typeof(ulong);

        /// <summary>
        /// Convert the given value to a string using the given radix
        /// </summary>
        internal override object FromString(string value, int radix) => Convert.ToUInt64(value, radix);

        /// <summary>
        /// Convert the given value to a string using the given formatInfo
        /// </summary>
        internal override object FromString(string value, NumberFormatInfo formatInfo)
        {
            return ulong.Parse(value, NumberStyles.Integer, formatInfo);
        }

        /// <summary>
        /// Convert the given value from a string using the given formatInfo
        /// </summary>
        internal override string ToString(object value, NumberFormatInfo formatInfo)
        {
            return ((ulong)value).ToString("G", formatInfo);
        }
    }
}
