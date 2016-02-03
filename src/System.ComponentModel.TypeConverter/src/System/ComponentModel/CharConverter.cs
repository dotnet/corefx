// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <devdoc>
    ///    <para>Provides
    ///       a type converter to convert Unicode
    ///       character objects to and from various other representations.</para>
    /// </devdoc>
    public class CharConverter : TypeConverter
    {
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object in the given source type to a Unicode character object using
        ///       the specified context.</para>
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <devdoc>
        ///      Converts the given object to another type.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is char)
            {
                if ((char)value == (char)0)
                {
                    return "";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <devdoc>
        ///    <para>Converts the given object to a Unicode character object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                if (text.Length > 1)
                {
                    text = text.Trim();
                }

                if (text.Length > 0)
                {
                    if (text.Length != 1)
                    {
                        throw new FormatException(SR.Format(SR.ConvertInvalidPrimitive, text, "Char"));
                    }
                    return text[0];
                }

                return '\0';
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
