// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.ComponentModel
{
    /// <devdoc>
    ///    <para>Provides a
    ///       type converter to convert globally unique identifier objects to and from various
    ///       other representations.</para>
    /// </devdoc>
    public class GuidConverter : TypeConverter
    {
        /// <devdoc>
        ///    <para>Gets a value indicating whether this
        ///       converter can convert an object in the given source type to a globally unique identifier object
        ///       using the context.</para>
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
        ///    <para>Converts
        ///       the given object to a globally unique identifier object.</para>
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (text != null)
            {
                text = text.Trim();
                return new Guid(text);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
