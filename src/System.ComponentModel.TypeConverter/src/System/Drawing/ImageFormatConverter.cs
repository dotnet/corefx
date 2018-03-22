// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;

namespace System.Drawing
{
    public class ImageFormatConverter : TypeConverter
    {
        public ImageFormatConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if ((destinationType == typeof(string)) || (destinationType == typeof(InstanceDescriptor)))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // we must be able to convert from short names and long names
            string strFormat = (value as string);
            if (strFormat == null)
            {
                // case #1, this is not a string
                return base.ConvertFrom(context, culture, value);
            }
            else if (strFormat[0] == '[')
            {
                // case #2, this is probably a long format (guid)
                if (strFormat.Equals(ImageFormat.Bmp.ToString()))
                    return ImageFormat.Bmp;
                else if (strFormat.Equals(ImageFormat.Emf.ToString()))
                    return ImageFormat.Emf;
                else if (strFormat.Equals(ImageFormat.Exif.ToString()))
                    return ImageFormat.Exif;
                else if (strFormat.Equals(ImageFormat.Gif.ToString()))
                    return ImageFormat.Gif;
                else if (strFormat.Equals(ImageFormat.Icon.ToString()))
                    return ImageFormat.Icon;
                else if (strFormat.Equals(ImageFormat.Jpeg.ToString()))
                    return ImageFormat.Jpeg;
                else if (strFormat.Equals(ImageFormat.MemoryBmp.ToString()))
                    return ImageFormat.MemoryBmp;
                else if (strFormat.Equals(ImageFormat.Png.ToString()))
                    return ImageFormat.Png;
                else if (strFormat.Equals(ImageFormat.Tiff.ToString()))
                    return ImageFormat.Tiff;
                else if (strFormat.Equals(ImageFormat.Wmf.ToString()))
                    return ImageFormat.Wmf;
            }
            else
            {
                // case #3, this is probably a short format
                if (strFormat.Equals("Bmp", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Bmp;
                else if (strFormat.Equals("Emf", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Emf;
                else if (strFormat.Equals("Exif", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Exif;
                else if (strFormat.Equals("Gif", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Gif;
                else if (strFormat.Equals("Icon", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Icon;
                else if (strFormat.Equals("Jpeg", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Jpeg;
                else if (strFormat.Equals("MemoryBmp", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.MemoryBmp;
                else if (strFormat.Equals("Png", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Png;
                else if (strFormat.Equals("Tiff", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Tiff;
                else if (strFormat.Equals("Wmf", StringComparison.OrdinalIgnoreCase))
                    return ImageFormat.Wmf;
            }
            // last case, this is an unknown string
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is ImageFormat imgFormat)
            {
                string strFormat = null;
                if (imgFormat.Guid.Equals(ImageFormat.Bmp.Guid))
                    strFormat = "Bmp";
                else if (imgFormat.Guid.Equals(ImageFormat.Emf.Guid))
                    strFormat = "Emf";
                else if (imgFormat.Guid.Equals(ImageFormat.Exif.Guid))
                    strFormat = "Exif";
                else if (imgFormat.Guid.Equals(ImageFormat.Gif.Guid))
                    strFormat = "Gif";
                else if (imgFormat.Guid.Equals(ImageFormat.Icon.Guid))
                    strFormat = "Icon";
                else if (imgFormat.Guid.Equals(ImageFormat.Jpeg.Guid))
                    strFormat = "Jpeg";
                else if (imgFormat.Guid.Equals(ImageFormat.MemoryBmp.Guid))
                    strFormat = "MemoryBmp";
                else if (imgFormat.Guid.Equals(ImageFormat.Png.Guid))
                    strFormat = "Png";
                else if (imgFormat.Guid.Equals(ImageFormat.Tiff.Guid))
                    strFormat = "Tiff";
                else if (imgFormat.Guid.Equals(ImageFormat.Wmf.Guid))
                    strFormat = "Wmf";

                if (destinationType == typeof(string))
                {
                    return strFormat ?? imgFormat.ToString();
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    if (strFormat != null)
                    {
                        return new InstanceDescriptor(typeof(ImageFormat).GetTypeInfo().GetProperty(strFormat), null);
                    }
                    else
                    {
                        ConstructorInfo ctor = typeof(ImageFormat).GetTypeInfo().GetConstructor(new Type[] { typeof(Guid) });
                        return new InstanceDescriptor(ctor, new object[] { imgFormat.Guid });
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(new ImageFormat[] {
                ImageFormat.MemoryBmp,
                ImageFormat.Bmp,
                ImageFormat.Emf,
                ImageFormat.Wmf,
                ImageFormat.Gif,
                ImageFormat.Jpeg,
                ImageFormat.Png,
                ImageFormat.Tiff,
                ImageFormat.Exif,
                ImageFormat.Icon
            });
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
