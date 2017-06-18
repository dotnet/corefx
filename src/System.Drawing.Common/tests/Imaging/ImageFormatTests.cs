// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Drawing.Imaging.Tests
{
    public class ImageFormatTests
    {
        private static ImageFormat BmpImageFormat = new ImageFormat(new Guid("b96b3cab-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat EmfImageFormat = new ImageFormat(new Guid("b96b3cac-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat ExifImageFormat = new ImageFormat(new Guid("b96b3cb2-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat GifImageFormat = new ImageFormat(new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat TiffImageFormat = new ImageFormat(new Guid("b96b3cb1-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat PngImageFormat = new ImageFormat(new Guid("b96b3caf-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat MemoryBmpImageFormat = new ImageFormat(new Guid("b96b3caa-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat IconImageFormat = new ImageFormat(new Guid("b96b3cb5-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat JpegImageFormat = new ImageFormat(new Guid("b96b3cae-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat WmfImageFormat = new ImageFormat(new Guid("b96b3cad-0728-11d3-9d7b-0000f81ef32e"));
        private static ImageFormat CustomImageFormat = new ImageFormat(new Guid("48749428-316f-496a-ab30-c819a92b3137"));

        [Fact]
        public void DefaultImageFormats()
        {
            Assert.Equal(BmpImageFormat.Guid, ImageFormat.Bmp.Guid);
            Assert.Equal(EmfImageFormat.Guid, ImageFormat.Emf.Guid);
            Assert.Equal(ExifImageFormat.Guid, ImageFormat.Exif.Guid);
            Assert.Equal(GifImageFormat.Guid, ImageFormat.Gif.Guid);
            Assert.Equal(TiffImageFormat.Guid, ImageFormat.Tiff.Guid);
            Assert.Equal(PngImageFormat.Guid, ImageFormat.Png.Guid);
            Assert.Equal(MemoryBmpImageFormat.Guid, ImageFormat.MemoryBmp.Guid);
            Assert.Equal(IconImageFormat.Guid, ImageFormat.Icon.Guid);
            Assert.Equal(JpegImageFormat.Guid, ImageFormat.Jpeg.Guid);
            Assert.Equal(WmfImageFormat.Guid, ImageFormat.Wmf.Guid);
        }

        [Fact]
        public void ToStringTest()
        {
            Assert.Equal("[ImageFormat: b96b3cab-0728-11d3-9d7b-0000f81ef32e]", BmpImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cac-0728-11d3-9d7b-0000f81ef32e]", EmfImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cb2-0728-11d3-9d7b-0000f81ef32e]", ExifImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cb0-0728-11d3-9d7b-0000f81ef32e]", GifImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cb1-0728-11d3-9d7b-0000f81ef32e]", TiffImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3caf-0728-11d3-9d7b-0000f81ef32e]", PngImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3caa-0728-11d3-9d7b-0000f81ef32e]", MemoryBmpImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cb5-0728-11d3-9d7b-0000f81ef32e]", IconImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cae-0728-11d3-9d7b-0000f81ef32e]", JpegImageFormat.ToString());
            Assert.Equal("[ImageFormat: b96b3cad-0728-11d3-9d7b-0000f81ef32e]", WmfImageFormat.ToString());
            Assert.Equal("[ImageFormat: 48749428-316f-496a-ab30-c819a92b3137]", CustomImageFormat.ToString());
        }

        [Fact]
        public void WellKnown_ToString()
        {
            Assert.Equal("Bmp", ImageFormat.Bmp.ToString());
            Assert.Equal("Emf", ImageFormat.Emf.ToString());
            Assert.Equal("Exif", ImageFormat.Exif.ToString());
            Assert.Equal("Gif", ImageFormat.Gif.ToString());
            Assert.Equal("Tiff", ImageFormat.Tiff.ToString());
            Assert.Equal("Png", ImageFormat.Png.ToString());
            Assert.Equal("MemoryBMP", ImageFormat.MemoryBmp.ToString());
            Assert.Equal("Icon", ImageFormat.Icon.ToString());
            Assert.Equal("Jpeg", ImageFormat.Jpeg.ToString());
            Assert.Equal("Wmf", ImageFormat.Wmf.ToString());
        }

        [Fact]
        public void TestEqual()
        {
            Assert.True(BmpImageFormat.Equals(BmpImageFormat), "Bmp-Bmp");
            Assert.True(EmfImageFormat.Equals(EmfImageFormat), "Emf-Emf");
            Assert.True(ExifImageFormat.Equals(ExifImageFormat), "Exif-Exif");
            Assert.True(GifImageFormat.Equals(GifImageFormat), "Gif-Gif");
            Assert.True(TiffImageFormat.Equals(TiffImageFormat), "Tiff-Tiff");
            Assert.True(PngImageFormat.Equals(PngImageFormat), "Png-Png");
            Assert.True(MemoryBmpImageFormat.Equals(MemoryBmpImageFormat), "MemoryBmp-MemoryBmp");
            Assert.True(IconImageFormat.Equals(IconImageFormat), "Icon-Icon");
            Assert.True(JpegImageFormat.Equals(JpegImageFormat), "Jpeg-Jpeg");
            Assert.True(WmfImageFormat.Equals(WmfImageFormat), "Wmf-Wmf");
            Assert.True(CustomImageFormat.Equals(CustomImageFormat), "Custom-Custom");
            Assert.False(BmpImageFormat.Equals(EmfImageFormat), "Bmp-Emf");
            Assert.False(BmpImageFormat.Equals("Bmp"), "Bmp-String-1");
            Assert.False(BmpImageFormat.Equals(BmpImageFormat.ToString()), "Bmp-String-2");
        }

        [Fact]
        public void TestGetHashCode()
        {
            Assert.Equal(BmpImageFormat.GetHashCode(), BmpImageFormat.Guid.GetHashCode());
            Assert.Equal(EmfImageFormat.GetHashCode(), EmfImageFormat.Guid.GetHashCode());
            Assert.Equal(ExifImageFormat.GetHashCode(), ExifImageFormat.Guid.GetHashCode());
            Assert.Equal(GifImageFormat.GetHashCode(), GifImageFormat.Guid.GetHashCode());
            Assert.Equal(TiffImageFormat.GetHashCode(), TiffImageFormat.Guid.GetHashCode());
            Assert.Equal(PngImageFormat.GetHashCode(), PngImageFormat.Guid.GetHashCode());
            Assert.Equal(MemoryBmpImageFormat.GetHashCode(), MemoryBmpImageFormat.Guid.GetHashCode());
            Assert.Equal(IconImageFormat.GetHashCode(), IconImageFormat.Guid.GetHashCode());
            Assert.Equal(JpegImageFormat.GetHashCode(), JpegImageFormat.Guid.GetHashCode());
            Assert.Equal(WmfImageFormat.GetHashCode(), WmfImageFormat.Guid.GetHashCode());
            Assert.Equal(CustomImageFormat.GetHashCode(), CustomImageFormat.Guid.GetHashCode());
        }
    }
}
