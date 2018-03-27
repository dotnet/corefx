// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Xunit;

namespace System.ComponentModel.TypeConverterTests
{
    public class ImageConverterTest
    {
        private readonly Image _image;
        private readonly ImageConverter _imgConv;
        private readonly ImageConverter _imgConvFrmTD;
        private readonly string _imageStr;
        private readonly byte[] _imageBytes;

        public ImageConverterTest()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            Stream testImageStream = assembly.GetManifestResourceStream("Resources.almogaver24bits.bmp");

            int length = (int)testImageStream.Length;
            _imageBytes = new byte[length];
            if (testImageStream.Read(_imageBytes, 0, length) != length)
            {
                throw new InvalidOperationException("Failed to load resource image.");
            }

            testImageStream.Position = 0;
            _image = Image.FromStream(testImageStream);
            _imageStr = _image.ToString();

            _imgConv = new ImageConverter();
            _imgConvFrmTD = (ImageConverter)TypeDescriptor.GetConverter(_image);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCanConvertFrom()
        {
            Assert.True(_imgConv.CanConvertFrom(typeof(byte[])), "byte[] (no context)");
            Assert.True(_imgConv.CanConvertFrom(null, typeof(byte[])), "byte[]");
            Assert.True(_imgConv.CanConvertFrom(null, _imageBytes.GetType()), "_imageBytes.GetType()");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(string)), "string");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(Rectangle)), "Rectangle");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(Point)), "Point");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(PointF)), "PointF");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(Size)), "Size");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(SizeF)), "SizeF");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(object)), "object");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(int)), "int");
            Assert.False(_imgConv.CanConvertFrom(null, typeof(Metafile)), "Mefafile");

            Assert.True(_imgConvFrmTD.CanConvertFrom(typeof(byte[])), "TD byte[] (no context)");
            Assert.True(_imgConvFrmTD.CanConvertFrom(null, typeof(byte[])), "TD byte[]");
            Assert.True(_imgConvFrmTD.CanConvertFrom(null, _imageBytes.GetType()), "TD _imageBytes.GetType()");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(string)), "TD string");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(Rectangle)), "TD Rectangle");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(Point)), "TD Point");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(PointF)), "TD PointF");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(Size)), "TD Size");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(SizeF)), "TD SizeF");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(object)), "TD object");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(int)), "TD int");
            Assert.False(_imgConvFrmTD.CanConvertFrom(null, typeof(Metafile)), "TD Metafile");
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCanConvertTo()
        {
            Assert.True(_imgConv.CanConvertTo(typeof(string)), "stirng (no context)");
            Assert.True(_imgConv.CanConvertTo(null, typeof(string)), "string");
            Assert.True(_imgConv.CanConvertTo(null, _imageStr.GetType()), "_imageStr.GetType()");
            Assert.True(_imgConv.CanConvertTo(typeof(byte[])), "byte[] (no context)");
            Assert.True(_imgConv.CanConvertTo(null, typeof(byte[])), "byte[]");
            Assert.True(_imgConv.CanConvertTo(null, _imageBytes.GetType()), "_imageBytes.GetType()");
            Assert.False(_imgConv.CanConvertTo(null, typeof(Rectangle)), "Rectangle");
            Assert.False(_imgConv.CanConvertTo(null, typeof(Point)), "Point");
            Assert.False(_imgConv.CanConvertTo(null, typeof(PointF)), "PointF");
            Assert.False(_imgConv.CanConvertTo(null, typeof(Size)), "Size");
            Assert.False(_imgConv.CanConvertTo(null, typeof(SizeF)), "SizeF");
            Assert.False(_imgConv.CanConvertTo(null, typeof(object)), "object");
            Assert.False(_imgConv.CanConvertTo(null, typeof(int)), "int");

            Assert.True(_imgConvFrmTD.CanConvertTo(typeof(string)), "TD string (no context)");
            Assert.True(_imgConvFrmTD.CanConvertTo(null, typeof(string)), "TD string");
            Assert.True(_imgConvFrmTD.CanConvertTo(null, _imageStr.GetType()), "TD _imageStr.GetType()");
            Assert.True(_imgConvFrmTD.CanConvertTo(typeof(byte[])), "TD byte[] (no context)");
            Assert.True(_imgConvFrmTD.CanConvertTo(null, typeof(byte[])), "TD byte[]");
            Assert.True(_imgConvFrmTD.CanConvertTo(null, _imageBytes.GetType()), "TD _imageBytes.GetType()");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(Rectangle)), "TD Rectangle");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(Point)), "TD Point");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(PointF)), "TD PointF");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(Size)), "TD Size");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(SizeF)), "TD SizeF");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(object)), "TD object");
            Assert.False(_imgConvFrmTD.CanConvertTo(null, typeof(int)), "TD int");
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom()
        {
            Image newImage = (Image)_imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, _imageBytes);

            Assert.Equal(_image.Height, newImage.Height);
            Assert.Equal(_image.Width, newImage.Width);

            Assert.Equal("(none)", _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, null, typeof(string)));

            newImage = (Image)_imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, _imageBytes);

            Assert.Equal(_image.Height, newImage.Height);
            Assert.Equal(_image.Width, newImage.Width);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom_BadString()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertFrom("System.Drawing.String"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom_BadString_WithCulture()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom_Point()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom_SizeF()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertFrom_Object()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertFrom_BadString()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertFrom("System.Drawing.String"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertFrom_BadString_Culture()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertFrom_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertFrom_Point()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertFrom_SizeF()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertFrom_Object()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo()
        {
            Assert.Equal(_imageStr, (string)_imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(string)));
            Assert.Equal(_imageStr, (string)_imgConv.ConvertTo(_image, typeof(string)));
            Assert.Equal(_imageStr, (string)_imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(string)));
            Assert.Equal(_imageStr, (string)_imgConvFrmTD.ConvertTo(_image, typeof(string)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_ByteArray()
        {
            byte[] newImageBytes = (byte[])_imgConv.ConvertTo(null, CultureInfo.InvariantCulture,
                _image, _imageBytes.GetType());

            Assert.Equal(_imageBytes.Length, newImageBytes.Length);

            newImageBytes = (byte[])_imgConv.ConvertTo(_image, _imageBytes.GetType());

            Assert.Equal(_imageBytes.Length, newImageBytes.Length);

            newImageBytes = (byte[])_imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture,
                _image, _imageBytes.GetType());

            Assert.Equal(_imageBytes.Length, newImageBytes.Length);

            newImageBytes = (byte[])_imgConvFrmTD.ConvertTo(_image, _imageBytes.GetType());

            Assert.Equal(_imageBytes.Length, newImageBytes.Length);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Rectangle()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Rectangle)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Image()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, _image.GetType()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Size()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Size)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Bitmap)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Point()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Point)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Metafile()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Metafile)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Object()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(object)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConvertTo_Int()
        {
            Assert.Throws<NotSupportedException>(() => _imgConv.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(int)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Rectangle()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Rectangle)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Image()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, _image.GetType()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Size()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Size)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Bitmap)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Point()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Point)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Metafile()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(Metafile)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Object()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(object)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TypeDescriptor_ConvertTo_Int()
        {
            Assert.Throws<NotSupportedException>(() => _imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _image, typeof(int)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGetPropertiesSupported()
        {
            Assert.True(_imgConv.GetPropertiesSupported(), "GPS#1");
            Assert.True(_imgConv.GetPropertiesSupported(null), "GPS#2");
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGetProperties()
        {
            int basecount = 1;
            PropertyDescriptorCollection propsColl;

            propsColl = _imgConv.GetProperties(null, _image, null);
            Assert.Equal(13 + basecount, propsColl.Count);

            propsColl = _imgConv.GetProperties(null, _image);
            Assert.Equal(6 + basecount, propsColl.Count);

            propsColl = _imgConv.GetProperties(_image);
            Assert.Equal(6 + basecount, propsColl.Count);

            propsColl = TypeDescriptor.GetProperties(typeof(Image));
            Assert.Equal(13 + basecount, propsColl.Count);

            propsColl = _imgConvFrmTD.GetProperties(null, _image, null);
            Assert.Equal(13 + basecount, propsColl.Count);

            propsColl = _imgConvFrmTD.GetProperties(null, _image);
            Assert.Equal(6 + basecount, propsColl.Count);

            propsColl = _imgConvFrmTD.GetProperties(_image);
            Assert.Equal(6 + basecount, propsColl.Count);
        }
    }
}
