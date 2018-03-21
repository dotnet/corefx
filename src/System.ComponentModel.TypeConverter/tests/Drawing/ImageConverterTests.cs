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
        Image image;
        ImageConverter imgConv;
        ImageConverter imgConvFrmTD;
        string imageStr;
        byte[] imageBytes;

        public ImageConverterTest()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            image = Image.FromStream(assembly.GetManifestResourceStream("Resources.almogaver24bits.bmp"));
            imageStr = image.ToString();

            imgConv = new ImageConverter();
            imgConvFrmTD = (ImageConverter)TypeDescriptor.GetConverter(image);

            using (Stream stream = assembly.GetManifestResourceStream("Resources.almogaver24bits.bmp"))
            {
                int length = (int)stream.Length;
                imageBytes = new byte[length];
                if (stream.Read(imageBytes, 0, length) != length)
                {
                    throw new InvalidOperationException("Failed to load resource image.");
                }
            }
        }

        [Fact]
        public void TestCanConvertFrom()
        {
            Assert.True(imgConv.CanConvertFrom(typeof(byte[])), "CCF#1");
            Assert.True(imgConv.CanConvertFrom(null, typeof(byte[])), "CCF#1a");
            Assert.True(imgConv.CanConvertFrom(null, imageBytes.GetType()), "CCF#1b");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(string)), "CCF#2");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(Rectangle)), "CCF#3");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(Point)), "CCF#4");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(PointF)), "CCF#5");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(Size)), "CCF#6");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(SizeF)), "CCF#7");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(object)), "CCF#8");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(int)), "CCF#9");
            Assert.True(!imgConv.CanConvertFrom(null, typeof(Metafile)), "CCF#10");

            Assert.True(imgConvFrmTD.CanConvertFrom(typeof(byte[])), "CCF#1A");
            Assert.True(imgConvFrmTD.CanConvertFrom(null, typeof(byte[])), "CCF#1aA");
            Assert.True(imgConvFrmTD.CanConvertFrom(null, imageBytes.GetType()), "CCF#1bA");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(string)), "CCF#2A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(Rectangle)), "CCF#3A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(Point)), "CCF#4A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(PointF)), "CCF#5A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(Size)), "CCF#6A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(SizeF)), "CCF#7A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(object)), "CCF#8A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(int)), "CCF#9A");
            Assert.True(!imgConvFrmTD.CanConvertFrom(null, typeof(Metafile)), "CCF#10A");
        }

        [Fact]
        public void TestCanConvertTo()
        {
            Assert.True(imgConv.CanConvertTo(typeof(string)), "CCT#1");
            Assert.True(imgConv.CanConvertTo(null, typeof(string)), "CCT#1a");
            Assert.True(imgConv.CanConvertTo(null, imageStr.GetType()), "CCT#1b");
            Assert.True(imgConv.CanConvertTo(typeof(byte[])), "CCT#2");
            Assert.True(imgConv.CanConvertTo(null, typeof(byte[])), "CCT#2a");
            Assert.True(imgConv.CanConvertTo(null, imageBytes.GetType()), "CCT#2b");
            Assert.True(!imgConv.CanConvertTo(null, typeof(Rectangle)), "CCT#3");
            Assert.True(!imgConv.CanConvertTo(null, typeof(Point)), "CCT#4");
            Assert.True(!imgConv.CanConvertTo(null, typeof(PointF)), "CCT#5");
            Assert.True(!imgConv.CanConvertTo(null, typeof(Size)), "CCT#6");
            Assert.True(!imgConv.CanConvertTo(null, typeof(SizeF)), "CCT#7");
            Assert.True(!imgConv.CanConvertTo(null, typeof(object)), "CCT#8");
            Assert.True(!imgConv.CanConvertTo(null, typeof(int)), "CCT#9");

            Assert.True(imgConvFrmTD.CanConvertTo(typeof(string)), "CCT#1A");
            Assert.True(imgConvFrmTD.CanConvertTo(null, typeof(string)), "CCT#1aA");
            Assert.True(imgConvFrmTD.CanConvertTo(null, imageStr.GetType()), "CCT#1bA");
            Assert.True(imgConvFrmTD.CanConvertTo(typeof(byte[])), "CCT#2A");
            Assert.True(imgConvFrmTD.CanConvertTo(null, typeof(byte[])), "CCT#2aA");
            Assert.True(imgConvFrmTD.CanConvertTo(null, imageBytes.GetType()), "CCT#2bA");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(Rectangle)), "CCT#3A");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(Point)), "CCT#4A");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(PointF)), "CCT#5A");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(Size)), "CCT#6A");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(SizeF)), "CCT#7A");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(object)), "CCT#8A");
            Assert.True(!imgConvFrmTD.CanConvertTo(null, typeof(int)), "CCT#9A");
        }

        [Fact]
        public void ConvertFrom()
        {
            Image newImage = (Image)imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, imageBytes);

            Assert.Equal(image.Height, newImage.Height);
            Assert.Equal(image.Width, newImage.Width);

            Assert.Equal("(none)", imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, null, typeof(string)));

            newImage = (Image)imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, imageBytes);

            Assert.Equal(image.Height, newImage.Height);
            Assert.Equal(image.Width, newImage.Width);
        }

        [Fact]
        public void ConvertFrom_BadString()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertFrom("System.Drawing.String"));
        }

        [Fact]
        public void ConvertFrom_BadString_WithCulture()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
        }

        [Fact]
        public void ConvertFrom_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
        }

        [Fact]
        public void ConvertFrom_Point()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
        }

        [Fact]
        public void ConvertFrom_SizeF()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
        }

        [Fact]
        public void ConvertFrom_Object()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
        }

        [Fact]
        public void TypeDescriptor_ConvertFrom_BadString()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertFrom("System.Drawing.String"));
        }

        [Fact]
        public void TypeDescriptor_ConvertFrom_BadString_Culture()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
        }

        [Fact]
        public void TypeDescriptor_ConvertFrom_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
        }

        [Fact]
        public void TypeDescriptor_ConvertFrom_Point()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
        }

        [Fact]
        public void TypeDescriptor_ConvertFrom_SizeF()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
        }

        [Fact]
        public void TypeDescriptor_ConvertFrom_Object()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
        }

        [Fact]
        public void ConvertTo()
        {
            Assert.Equal(imageStr, (string)imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(string)));
            Assert.Equal(imageStr, (string)imgConv.ConvertTo(image, typeof(string)));
            Assert.Equal(imageStr, (string)imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(string)));
            Assert.Equal(imageStr, (string)imgConvFrmTD.ConvertTo(image, typeof(string)));
        }

        [Fact]
        public void ConvertTo_ByteArray()
        {
            byte[] newImageBytes = (byte[])imgConv.ConvertTo(null, CultureInfo.InvariantCulture,
                image, imageBytes.GetType());

            Assert.Equal(imageBytes.Length, newImageBytes.Length);

            newImageBytes = (byte[])imgConv.ConvertTo(image, imageBytes.GetType());

            Assert.Equal(imageBytes.Length, newImageBytes.Length);

            newImageBytes = (byte[])imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture,
                image, imageBytes.GetType());

            Assert.Equal(imageBytes.Length, newImageBytes.Length);

            newImageBytes = (byte[])imgConvFrmTD.ConvertTo(image, imageBytes.GetType());

            Assert.Equal(imageBytes.Length, newImageBytes.Length);
        }

        [Fact]
        public void ConvertTo_Rectangle()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Rectangle)));
        }

        [Fact]
        public void ConvertTo_Image()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, image.GetType()));
        }

        [Fact]
        public void ConvertTo_Size()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Size)));
        }

        [Fact]
        public void ConvertTo_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Bitmap)));
        }

        [Fact]
        public void ConvertTo_Point()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Point)));
        }

        [Fact]
        public void ConvertTo_Metafile()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Metafile)));
        }

        [Fact]
        public void ConvertTo_Object()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Object)));
        }

        [Fact]
        public void ConvertTo_Int()
        {
            Assert.Throws<NotSupportedException>(() => imgConv.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(int)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Rectangle()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Rectangle)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Image()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, image.GetType()));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Size()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Size)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Bitmap()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Bitmap)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Point()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Point)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Metafile()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Metafile)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Object()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(Object)));
        }

        [Fact]
        public void TypeDescriptor_ConvertTo_Int()
        {
            Assert.Throws<NotSupportedException>(() => imgConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, image, typeof(int)));
        }

        [Fact]
        public void TestGetPropertiesSupported()
        {
            Assert.True(imgConv.GetPropertiesSupported(), "GPS#1");
            Assert.True(imgConv.GetPropertiesSupported(null), "GPS#2");
        }

        [Fact]
        public void TestGetProperties()
        {
            int basecount = 1;
            PropertyDescriptorCollection propsColl;

            propsColl = imgConv.GetProperties(null, image, null);
            Assert.Equal(13 + basecount, propsColl.Count);

            propsColl = imgConv.GetProperties(null, image);
            Assert.Equal(6 + basecount, propsColl.Count);

            propsColl = imgConv.GetProperties(image);
            Assert.Equal(6 + basecount, propsColl.Count);

            propsColl = TypeDescriptor.GetProperties(typeof(Image));
            Assert.Equal(13 + basecount, propsColl.Count);

            propsColl = imgConvFrmTD.GetProperties(null, image, null);
            Assert.Equal(13 + basecount, propsColl.Count);

            propsColl = imgConvFrmTD.GetProperties(null, image);
            Assert.Equal(6 + basecount, propsColl.Count);

            propsColl = imgConvFrmTD.GetProperties(image);
            Assert.Equal(6 + basecount, propsColl.Count);
        }
    }
}
