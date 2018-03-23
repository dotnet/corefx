// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using Xunit;

namespace System.ComponentModel.TypeConverterTests
{
    public class ImageFormatConverterTest
    {
        private readonly ImageFormat _imageFmt;
        private readonly ImageFormatConverter _imgFmtConv;
        private readonly ImageFormatConverter _imgFmtConvFrmTD;
        private readonly string _imageFmtStr;

        public ImageFormatConverterTest()
        {
            _imageFmt = ImageFormat.Bmp;
            _imageFmtStr = _imageFmt.ToString();

            _imgFmtConv = new ImageFormatConverter();
            _imgFmtConvFrmTD = (ImageFormatConverter)TypeDescriptor.GetConverter(_imageFmt);
        }

        [Fact]
        public void TestCanConvertFrom()
        {
            Assert.True(_imgFmtConv.CanConvertFrom(typeof(string)), "CCF#1");
            Assert.True(_imgFmtConv.CanConvertFrom(null, typeof(string)), "CCF#1a");
            Assert.True(!_imgFmtConv.CanConvertFrom(null, typeof(ImageFormat)), "CCF#2");
            Assert.True(!_imgFmtConv.CanConvertFrom(null, typeof(Guid)), "CCF#3");
            Assert.True(!_imgFmtConv.CanConvertFrom(null, typeof(object)), "CCF#4");
            Assert.True(!_imgFmtConv.CanConvertFrom(null, typeof(int)), "CCF#5");

            Assert.True(_imgFmtConvFrmTD.CanConvertFrom(typeof(string)), "CCF#1A");
            Assert.True(_imgFmtConvFrmTD.CanConvertFrom(null, typeof(string)), "CCF#1aA");
            Assert.True(!_imgFmtConvFrmTD.CanConvertFrom(null, typeof(ImageFormat)), "CCF#2A");
            Assert.True(!_imgFmtConvFrmTD.CanConvertFrom(null, typeof(Guid)), "CCF#3A");
            Assert.True(!_imgFmtConvFrmTD.CanConvertFrom(null, typeof(object)), "CCF#4A");
            Assert.True(!_imgFmtConvFrmTD.CanConvertFrom(null, typeof(int)), "CCF#5A");
        }

        [Fact]
        public void TestCanConvertTo()
        {
            Assert.True(_imgFmtConv.CanConvertTo(typeof(string)), "CCT#1");
            Assert.True(_imgFmtConv.CanConvertTo(null, typeof(string)), "CCT#1a");
            Assert.True(!_imgFmtConv.CanConvertTo(null, typeof(ImageFormat)), "CCT#2");
            Assert.True(!_imgFmtConv.CanConvertTo(null, typeof(Guid)), "CCT#3");
            Assert.True(!_imgFmtConv.CanConvertTo(null, typeof(object)), "CCT#4");
            Assert.True(!_imgFmtConv.CanConvertTo(null, typeof(int)), "CCT#5");

            Assert.True(_imgFmtConvFrmTD.CanConvertTo(typeof(string)), "CCT#1A");
            Assert.True(_imgFmtConvFrmTD.CanConvertTo(null, typeof(string)), "CCT#1aA");
            Assert.True(!_imgFmtConvFrmTD.CanConvertTo(null, typeof(ImageFormat)), "CCT#2A");
            Assert.True(!_imgFmtConvFrmTD.CanConvertTo(null, typeof(Guid)), "CCT#3A");
            Assert.True(!_imgFmtConvFrmTD.CanConvertTo(null, typeof(object)), "CCT#4A");
            Assert.True(!_imgFmtConvFrmTD.CanConvertTo(null, typeof(int)), "CCT#5A");
        }

        [Fact]
        public void TestConvertFrom()
        {
            Assert.Equal(_imageFmt, (ImageFormat)_imgFmtConv.ConvertFrom(null, CultureInfo.InvariantCulture, ImageFormat.Bmp.ToString()));

            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertFrom("System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertFrom(null, CultureInfo.InvariantCulture, ImageFormat.Bmp));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertFrom(null, CultureInfo.InvariantCulture, ImageFormat.Bmp.Guid));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertFrom(null, CultureInfo.InvariantCulture, 10));

            Assert.Equal(_imageFmt, (ImageFormat)_imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, ImageFormat.Bmp.ToString()));

            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertFrom("System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, ImageFormat.Bmp));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, ImageFormat.Bmp.Guid));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, 10));
        }

        private ImageFormat ShortName(string imgFormatValue)
        {
            return (ImageFormat)_imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, imgFormatValue);
        }

        [Fact]
        public void ConvertFrom_ShortName()
        {
            Assert.Equal(ImageFormat.Bmp, ShortName("Bmp"));
            Assert.Equal(ImageFormat.Emf, ShortName("Emf"));
            Assert.Equal(ImageFormat.Exif, ShortName("Exif"));
            Assert.Equal(ImageFormat.Gif, ShortName("Gif"));
            Assert.Equal(ImageFormat.Tiff, ShortName("Tiff"));
            Assert.Equal(ImageFormat.Png, ShortName("Png"));
            Assert.Equal(ImageFormat.MemoryBmp, ShortName("MemoryBmp"));
            Assert.Equal(ImageFormat.Icon, ShortName("Icon"));
            Assert.Equal(ImageFormat.Jpeg, ShortName("Jpeg"));
            Assert.Equal(ImageFormat.Wmf, ShortName("Wmf"));
        }

        private void LongName(ImageFormat iformat)
        {
            Assert.Equal(iformat, (ImageFormat)_imgFmtConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, iformat.ToString()));
        }

        [Fact]
        public void ConvertFrom_LongName()
        {
            LongName(ImageFormat.Bmp);
            LongName(ImageFormat.Emf);
            LongName(ImageFormat.Exif);
            LongName(ImageFormat.Gif);
            LongName(ImageFormat.Tiff);
            LongName(ImageFormat.Png);
            LongName(ImageFormat.MemoryBmp);
            LongName(ImageFormat.Icon);
            LongName(ImageFormat.Jpeg);
            LongName(ImageFormat.Wmf);
        }

        [Fact]
        public void TestConvertTo()
        {
            Assert.Equal(_imageFmtStr, (string)_imgFmtConv.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(string)));
            Assert.Equal(_imageFmtStr, (string)_imgFmtConv.ConvertTo(_imageFmt, typeof(string)));

            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(ImageFormat)));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(Guid)));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(object)));
            Assert.Throws<NotSupportedException>(() => _imgFmtConv.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(int)));

            Assert.Equal(_imageFmtStr, (string)_imgFmtConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(string)));

            Assert.Equal(_imageFmtStr, (string)_imgFmtConvFrmTD.ConvertTo(_imageFmt, typeof(string)));

            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(ImageFormat)));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(Guid)));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(object)));
            Assert.Throws<NotSupportedException>(() => _imgFmtConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _imageFmt, typeof(int)));
        }

        [Fact]
        public void GetStandardValuesSupported()
        {
            Assert.True(_imgFmtConv.GetStandardValuesSupported(), "GetStandardValuesSupported()");
            Assert.True(_imgFmtConv.GetStandardValuesSupported(null), "GetStandardValuesSupported(null)");
        }

        private void CheckStandardValues(ICollection values)
        {
            bool memorybmp = false;
            bool bmp = false;
            bool emf = false;
            bool wmf = false;
            bool gif = false;
            bool jpeg = false;
            bool png = false;
            bool tiff = false;
            bool exif = false;
            bool icon = false;

            foreach (ImageFormat iformat in values)
            {
                switch (iformat.Guid.ToString())
                {
                    case "b96b3caa-0728-11d3-9d7b-0000f81ef32e":
                        memorybmp = true;
                        break;
                    case "b96b3cab-0728-11d3-9d7b-0000f81ef32e":
                        bmp = true;
                        break;
                    case "b96b3cac-0728-11d3-9d7b-0000f81ef32e":
                        emf = true;
                        break;
                    case "b96b3cad-0728-11d3-9d7b-0000f81ef32e":
                        wmf = true;
                        break;
                    case "b96b3cb0-0728-11d3-9d7b-0000f81ef32e":
                        gif = true;
                        break;
                    case "b96b3cae-0728-11d3-9d7b-0000f81ef32e":
                        jpeg = true;
                        break;
                    case "b96b3caf-0728-11d3-9d7b-0000f81ef32e":
                        png = true;
                        break;
                    case "b96b3cb1-0728-11d3-9d7b-0000f81ef32e":
                        tiff = true;
                        break;
                    case "b96b3cb2-0728-11d3-9d7b-0000f81ef32e":
                        exif = true;
                        break;
                    case "b96b3cb5-0728-11d3-9d7b-0000f81ef32e":
                        icon = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown GUID {iformat.Guid}.");
                }
            }
            Assert.True(memorybmp, "MemoryBMP");
            Assert.True(bmp, "Bmp");
            Assert.True(emf, "Emf");
            Assert.True(wmf, "Wmf");
            Assert.True(gif, "Gif");
            Assert.True(jpeg, "Jpeg");
            Assert.True(png, "Png");
            Assert.True(tiff, "Tiff");
            Assert.True(exif, "Exif");
            Assert.True(icon, "Icon");
        }

        [Fact]
        public void GetStandardValues()
        {
            CheckStandardValues(_imgFmtConv.GetStandardValues());
            CheckStandardValues(_imgFmtConv.GetStandardValues(null));
        }
    }
}
