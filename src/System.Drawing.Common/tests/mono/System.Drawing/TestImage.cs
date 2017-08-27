// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Image class testing unit
//
// Authors:
// 	Jordi Mas i Hernàndez (jmas@softcatala.org>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2005 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2005-2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml.Serialization;
using Xunit;

namespace MonoTests.System.Drawing
{

    public class ImageTest : IDisposable
    {

        private string fname;
        private bool callback;

        public ImageTest()
        {
            fname = Path.GetTempFileName();
            callback = false;
        }

        public void Dispose()
        {
            try
            {
                File.Delete(fname);
            }
            catch
            {
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FileDoesNotExists()
        {
            Assert.Throws<FileNotFoundException>(() => Image.FromFile("FileDoesNotExists.jpg"));
        }

        private bool CallbackTrue()
        {
            callback = true;
            return true;
        }

        private bool CallbackFalse()
        {
            callback = true;
            return false;
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_NullCallback_Tiff()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                // according to documentation a callback is mandatory
                Image tn = bmp.GetThumbnailImage(10, 5, null, IntPtr.Zero);
                Assert.Equal(5, tn.Height);
                Assert.Equal(10, tn.Width);
                Assert.False(callback);
                tn.Save(fname, ImageFormat.Tiff);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_Height_Zero()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                Assert.Throws<OutOfMemoryException>(() => bmp.GetThumbnailImage(5, 0, new Image.GetThumbnailImageAbort(CallbackFalse), IntPtr.Zero));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_Width_Negative()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                Assert.Throws<OutOfMemoryException>(() => bmp.GetThumbnailImage(-5, 5, new Image.GetThumbnailImageAbort(CallbackFalse), IntPtr.Zero));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_CallbackData_Invalid()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                // according to documentation IntPtr.Zero must be supplied as data
                Image tn = bmp.GetThumbnailImage(5, 5, new Image.GetThumbnailImageAbort(CallbackFalse), (IntPtr)Int32.MaxValue);
                Assert.Equal(5, tn.Height);
                Assert.Equal(5, tn.Width);
                Assert.False(callback);
                tn.Save(fname, ImageFormat.Tiff);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_SameSize_Bmp()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                Image tn = bmp.GetThumbnailImage(10, 10, new Image.GetThumbnailImageAbort(CallbackFalse), IntPtr.Zero);
                Assert.Equal(10, tn.Height);
                Assert.Equal(10, tn.Width);
                Assert.False(callback);
                tn.Save(fname, ImageFormat.Bmp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_Smaller_Gif()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                Image tn = bmp.GetThumbnailImage(4, 4, new Image.GetThumbnailImageAbort(CallbackTrue), IntPtr.Zero);
                Assert.Equal(4, tn.Height);
                Assert.Equal(4, tn.Width);
                Assert.False(callback);
                tn.Save(fname, ImageFormat.Gif);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetThumbnailImage_Bigger_Png()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                Image tn = bmp.GetThumbnailImage(40, 40, new Image.GetThumbnailImageAbort(CallbackTrue), IntPtr.Zero);
                Assert.Equal(40, tn.Height);
                Assert.Equal(40, tn.Width);
                Assert.False(callback);
                tn.Save(fname, ImageFormat.Png);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Stream_Unlocked()
        {
            try
            {
                Image img = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap bmp = new Bitmap(10, 10))
                    {
                        bmp.Save(ms, ImageFormat.Png);
                    }
                    ms.Position = 0;
                    img = Image.FromStream(ms);
                }
                // stream isn't available anymore
                ((Bitmap)img).MakeTransparent(Color.Transparent);
            }
            catch (OutOfMemoryException)
            {
                int p = (int)Environment.OSVersion.Platform;
                // libgdiplus (UNIX) doesn't lazy load the image so the
                // stream may be freed (and this exception will never occur)
                if ((p == 4) || (p == 128) || (p == 6))
                    throw;
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Stream_Locked()
        {
            Image img = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bmp = new Bitmap(10, 10))
                {
                    bmp.Save(ms, ImageFormat.Png);
                }
                ms.Position = 0;
                img = Image.FromStream(ms);
                // stream is available
                ((Bitmap)img).MakeTransparent(Color.Transparent);
            }
        }

        private void Wmf(Image img)
        {
            Assert.False(img is Bitmap);
            Assert.True(img is Metafile);
            // as Image
            Assert.Equal(327683, img.Flags);
            Assert.True(img.RawFormat.Equals(ImageFormat.Wmf));
            Assert.Null(img.Tag);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromFile_Metafile_Wmf()
        {
            string filename = Helpers.GetTestBitmapPath("telescope_01.wmf");
            using (Image img = Image.FromFile(filename))
            {
                Wmf(img);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromStream_Metafile_Wmf()
        {
            string filename = Helpers.GetTestBitmapPath("telescope_01.wmf");
            using (FileStream fs = File.OpenRead(filename))
            {
                using (Image img = Image.FromStream(fs))
                {
                    Wmf(img);
                }
            }
        }

        private void Emf(Image img)
        {
            Assert.False(img is Bitmap);
            Assert.True(img is Metafile);
            // as Image
            Assert.Equal(327683, img.Flags);
            Assert.True(img.RawFormat.Equals(ImageFormat.Emf));
            Assert.Null(img.Tag);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromFile_Metafile_Emf()
        {
            string filename = Helpers.GetTestBitmapPath("milkmateya01.emf");
            using (Image img = Image.FromFile(filename))
            {
                Emf(img);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromStream_Metafile_Emf()
        {
            string filename = Helpers.GetTestBitmapPath("milkmateya01.emf");
            using (FileStream fs = File.OpenRead(filename))
            {
                using (Image img = Image.FromStream(fs))
                {
                    Emf(img);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromFile_Invalid()
        {
            string filename = Assembly.GetExecutingAssembly().Location;
            Assert.Throws<OutOfMemoryException>(() => Image.FromFile(filename));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromStream_Invalid()
        {
            string filename = Assembly.GetExecutingAssembly().Location;
            using (FileStream fs = File.OpenRead(filename))
            {
                Assert.Throws<ArgumentException>(() => Image.FromStream(fs));
            }
        }

        private Bitmap GetBitmap()
        {
            Bitmap bmp = new Bitmap(20, 10, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Pen pen = new Pen(Color.Black, 3);
                g.DrawRectangle(pen, 0, 0, 5, 10);
            }
            return bmp;
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StreamSaveLoad()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Bitmap bmp = GetBitmap())
                {
                    Assert.Equal(0, ms.Position);
                    bmp.Save(ms, ImageFormat.Bmp);
                    Assert.True(ms.Position > 0);

                    ms.Position = ms.Length;
                    Assert.Equal(ms.Length, ms.Position);

                    Bitmap bmp2 = (Bitmap)Image.FromStream(ms);
                    Assert.True(ms.Position > 20);

                    Assert.True(bmp2.RawFormat.Equals(ImageFormat.Bmp));

                    Assert.Equal(bmp.GetPixel(0, 0), bmp2.GetPixel(0, 0));
                    Assert.Equal(bmp.GetPixel(10, 0), bmp2.GetPixel(10, 0));
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StreamJunkSaveLoad()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // junk
                ms.WriteByte(0xff);
                ms.WriteByte(0xef);
                Assert.Equal(2, ms.Position);

                using (Bitmap bmp = GetBitmap())
                {
                    bmp.Save(ms, ImageFormat.Bmp);
                    Assert.True(ms.Position > 2);
                    // exception here
                    Assert.Throws<ArgumentException>(() => Image.FromStream(ms));
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void XmlSerialization()
        {
            new XmlSerializer(typeof(Image));
        }
    }
}
