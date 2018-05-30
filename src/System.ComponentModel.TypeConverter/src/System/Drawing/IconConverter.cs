// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
    public class IconConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(byte[]));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(byte[]) || destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            byte[] bytes = value as byte[];
            return bytes == null ? base.ConvertFrom(context, culture, value) : new Icon(new MemoryStream(bytes));
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return SR.none;
                }
                else if (value is Icon)
                {
                    return value.ToString();
                }
            }
            else if (destinationType == typeof(byte[]))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ((Icon)value).Save(ms);
                    return ms.ToArray();
                }
            }

            throw GetConvertFromException(value);
        }
    }
}
