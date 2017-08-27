// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.TextureBrush unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2006-2007 Novell, Inc (http://www.novell.com)
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing
{

    public class TextureBrushTest
    {

        private Image image;
        private Rectangle rect;
        private RectangleF rectf;
        private ImageAttributes attr;
        private Bitmap bmp;

        public TextureBrushTest()
        {
            image = new Bitmap(10, 10);
            rect = new Rectangle(0, 0, 10, 10);
            rectf = new RectangleF(0, 0, 10, 10);
            attr = new ImageAttributes();
            bmp = new Bitmap(50, 50);
        }

        private void Common(TextureBrush t, WrapMode wm)
        {
            using (Image img = t.Image)
            {
                Assert.NotNull(img);
            }
            Assert.False(Object.ReferenceEquals(image, t.Image));
            Assert.True(t.Transform.IsIdentity);
            Assert.Equal(wm, t.WrapMode);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage()
        {
            TextureBrush t = new TextureBrush(image);
            Common(t, WrapMode.Tile);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage_Null_WrapMode()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(null, WrapMode.Clamp));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageWrapMode()
        {
            foreach (WrapMode wm in Enum.GetValues(typeof(WrapMode)))
            {
                TextureBrush t = new TextureBrush(image, wm);
                Common(t, wm);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void CtorImageWrapMode_Invalid()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => new TextureBrush(image, (WrapMode)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage_Null_Rectangle()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(null, rect));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangle_Empty()
        {
            Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, new Rectangle()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangle()
        {
            TextureBrush t = new TextureBrush(image, rect);
            Common(t, WrapMode.Tile);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage_Null_RectangleF()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(null, rectf));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleF_Empty()
        {
            Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, new RectangleF()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleF()
        {
            TextureBrush t = new TextureBrush(image, rectf);
            Common(t, WrapMode.Tile);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage_Null_RectangleAttributes()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(null, rect, attr));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangle_Empty_Attributes()
        {
            Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, new Rectangle(), attr));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleAttributes_Null()
        {
            TextureBrush t = new TextureBrush(image, rect, null);
            Common(t, WrapMode.Tile);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleAttributes()
        {
            TextureBrush t = new TextureBrush(image, rect, attr);
            Common(t, WrapMode.Clamp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImage_Null_RectangleFAttributes()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(null, rectf, attr));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleF_Empty_Attributes()
        {
            Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, new RectangleF()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleFAttributes_Null()
        {
            TextureBrush t = new TextureBrush(image, rectf, null);
            Common(t, WrapMode.Tile);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageRectangleFAttributes()
        {
            TextureBrush t = new TextureBrush(image, rectf, attr);
            Common(t, WrapMode.Clamp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageWrapModeRectangle()
        {
            foreach (WrapMode wm in Enum.GetValues(typeof(WrapMode)))
            {
                TextureBrush t = new TextureBrush(image, wm, rect);
                Common(t, wm);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void CtorImageWrapMode_Invalid_Rectangle()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => new TextureBrush(image, (WrapMode)Int32.MinValue, rect));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CtorImageWrapModeRectangleF()
        {
            foreach (WrapMode wm in Enum.GetValues(typeof(WrapMode)))
            {
                TextureBrush t = new TextureBrush(image, wm, rectf);
                Common(t, wm);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void CtorImageWrapMode_Invalid_RectangleF()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => new TextureBrush(image, (WrapMode)Int32.MinValue, rectf));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TextureBush_RectangleInsideBitmap()
        {
            Rectangle r = new Rectangle(10, 10, 40, 40);
            Assert.True(r.Y + r.Height <= bmp.Height);
            Assert.True(r.X + r.Width <= bmp.Width);
            TextureBrush b = new TextureBrush(bmp, r);
            using (Image img = b.Image)
            {
                Assert.Equal(r.Height, img.Height);
                Assert.Equal(r.Width, img.Width);
            }
            Assert.True(b.Transform.IsIdentity);
            Assert.Equal(WrapMode.Tile, b.WrapMode);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TextureBush_RectangleOutsideBitmap()
        {
            Rectangle r = new Rectangle(50, 50, 50, 50);
            Assert.False(r.Y + r.Height <= bmp.Height);
            Assert.False(r.X + r.Width <= bmp.Width);
            Assert.Throws<OutOfMemoryException>(() => new TextureBrush(bmp, r));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(image).Transform = null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform()
        {
            Matrix m = new Matrix();
            TextureBrush t = new TextureBrush(image);
            t.Transform = m;
            Assert.False(Object.ReferenceEquals(m, t.Transform));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WrapMode_Valid()
        {
            foreach (WrapMode wm in Enum.GetValues(typeof(WrapMode)))
            {
                TextureBrush t = new TextureBrush(image);
                t.WrapMode = wm;
                Assert.Equal(wm, t.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void WrapMode_Invalid()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => new TextureBrush(image).WrapMode = (WrapMode)Int32.MinValue);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Clone()
        {
            TextureBrush t = new TextureBrush(image);
            TextureBrush clone = (TextureBrush)t.Clone();
            Common(clone, t.WrapMode);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_Clone()
        {
            TextureBrush t = new TextureBrush(image);
            t.Dispose();
            Assert.Throws<ArgumentException>(() => t.Clone());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_Dispose()
        {
            TextureBrush t = new TextureBrush(image);
            t.Dispose();
            t.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(image).MultiplyTransform(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform_Null_Order()
        {
            Assert.Throws<ArgumentNullException>(() => new TextureBrush(image).MultiplyTransform(null, MatrixOrder.Append));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransformOrder_Invalid()
        {
            TextureBrush t = new TextureBrush(image);
            t.MultiplyTransform(new Matrix(), (MatrixOrder)Int32.MinValue);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform_NonInvertible()
        {
            TextureBrush t = new TextureBrush(image);
            Matrix noninvertible = new Matrix(123, 24, 82, 16, 47, 30);
            Assert.Throws<ArgumentException>(() => t.MultiplyTransform(noninvertible));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ResetTransform()
        {
            TextureBrush t = new TextureBrush(image);
            t.RotateTransform(90);
            Assert.False(t.Transform.IsIdentity);
            t.ResetTransform();
            Assert.True(t.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateTransform()
        {
            TextureBrush t = new TextureBrush(image);
            t.RotateTransform(90);
            float[] elements = t.Transform.Elements;
            Assert.Equal(0, elements[0], 1);
            Assert.Equal(1, elements[1], 1);
            Assert.Equal(-1, elements[2], 1);
            Assert.Equal(0, elements[3], 1);
            Assert.Equal(0, elements[4], 1);
            Assert.Equal(0, elements[5], 1);

            t.RotateTransform(270);
            Assert.True(t.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateTransform_InvalidOrder()
        {
            TextureBrush t = new TextureBrush(image);
            Assert.Throws<ArgumentException>(() => t.RotateTransform(720, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform()
        {
            TextureBrush t = new TextureBrush(image);
            t.ScaleTransform(2, 4);
            float[] elements = t.Transform.Elements;
            Assert.Equal(2, elements[0], 1);
            Assert.Equal(0, elements[1], 1);
            Assert.Equal(0, elements[2], 1);
            Assert.Equal(4, elements[3], 1);
            Assert.Equal(0, elements[4], 1);
            Assert.Equal(0, elements[5], 1);

            t.ScaleTransform(0.5f, 0.25f);
            Assert.True(t.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform_MaxMin()
        {
            TextureBrush t = new TextureBrush(image);
            t.ScaleTransform(Single.MaxValue, Single.MinValue);
            float[] elements = t.Transform.Elements;
            Assert.Equal(Single.MaxValue, elements[0]);
            Assert.Equal(0, elements[1], 1);
            Assert.Equal(0, elements[2], 1);
            Assert.Equal(Single.MinValue, elements[3]);
            Assert.Equal(0, elements[4], 1);
            Assert.Equal(0, elements[5], 1);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform_InvalidOrder()
        {
            TextureBrush t = new TextureBrush(image);
            Assert.Throws<ArgumentException>(() => t.ScaleTransform(1, 1, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TranslateTransform()
        {
            TextureBrush t = new TextureBrush(image);
            t.TranslateTransform(1, 1);
            float[] elements = t.Transform.Elements;
            Assert.Equal(1, elements[0], 1);
            Assert.Equal(0, elements[1], 1);
            Assert.Equal(0, elements[2], 1);
            Assert.Equal(1, elements[3], 1);
            Assert.Equal(1, elements[4], 1);
            Assert.Equal(1, elements[5], 1);

            t.TranslateTransform(-1, -1);
            elements = t.Transform.Elements;
            Assert.Equal(1, elements[0], 1);
            Assert.Equal(0, elements[1], 1);
            Assert.Equal(0, elements[2], 1);
            Assert.Equal(1, elements[3], 1);
            Assert.Equal(0, elements[4], 1);
            Assert.Equal(0, elements[5], 1);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TranslateTransform_InvalidOrder()
        {
            TextureBrush t = new TextureBrush(image);
            Assert.Throws<ArgumentException>(() => t.TranslateTransform(1, 1, (MatrixOrder)Int32.MinValue));
        }

        private void Alpha_81828(WrapMode mode, bool equals)
        {
            using (Bitmap bm = new Bitmap(2, 1))
            {
                using (Graphics g = Graphics.FromImage(bm))
                {
                    Color t = Color.FromArgb(128, Color.White);
                    g.Clear(Color.Green);
                    g.FillRectangle(new SolidBrush(t), 0, 0, 1, 1);
                    using (Bitmap bm_for_brush = new Bitmap(1, 1))
                    {
                        bm_for_brush.SetPixel(0, 0, t);
                        using (TextureBrush tb = new TextureBrush(bm_for_brush, mode))
                        {
                            g.FillRectangle(tb, 1, 0, 1, 1);
                        }
                    }
                    Color c1 = bm.GetPixel(0, 0);
                    Color c2 = bm.GetPixel(1, 0);
                    if (equals)
                        Assert.Equal(c1, c2);
                    else
                        Assert.Equal(-16744448, c2.ToArgb());
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Alpha_81828_Clamp()
        {
            Alpha_81828(WrapMode.Clamp, false);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Alpha_81828_Tile()
        {
            Alpha_81828(WrapMode.Tile, true);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Alpha_81828_TileFlipX()
        {
            Alpha_81828(WrapMode.TileFlipX, true);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Alpha_81828_TileFlipY()
        {
            Alpha_81828(WrapMode.TileFlipY, true);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Alpha_81828_TileFlipXY()
        {
            Alpha_81828(WrapMode.TileFlipXY, true);
        }
    }
}
