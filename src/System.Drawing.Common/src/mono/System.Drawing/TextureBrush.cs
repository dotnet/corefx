// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.TextureBrush.cs
//
// Authors:
//   Dennis Hayes (dennish@Raytek.com)
//   Ravindra (rkumar@novell.com)
//   Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Ximian, Inc
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
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

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Drawing
{

    public sealed class TextureBrush : Brush
    {

        internal TextureBrush(IntPtr ptr) :
            base(ptr)
        {
        }

        public TextureBrush(Image bitmap) :
            this(bitmap, WrapMode.Tile)
        {
        }

        public TextureBrush(Image image, Rectangle dstRect) :
            this(image, WrapMode.Tile, dstRect)
        {
        }

        public TextureBrush(Image image, RectangleF dstRect) :
            this(image, WrapMode.Tile, dstRect)
        {
        }

        public TextureBrush(Image image, WrapMode wrapMode)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            if ((wrapMode < WrapMode.Tile) || (wrapMode > WrapMode.Clamp))
                throw new InvalidEnumArgumentException("WrapMode");

            Status status = GDIPlus.GdipCreateTexture(image.nativeObject, wrapMode, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        [MonoLimitation("ImageAttributes are ignored when using libgdiplus")]
        public TextureBrush(Image image, Rectangle dstRect, ImageAttributes imageAttr)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            IntPtr attr = imageAttr == null ? IntPtr.Zero : imageAttr.NativeObject;
            Status status = GDIPlus.GdipCreateTextureIAI(image.nativeObject, attr, dstRect.X, dstRect.Y,
                dstRect.Width, dstRect.Height, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        [MonoLimitation("ImageAttributes are ignored when using libgdiplus")]
        public TextureBrush(Image image, RectangleF dstRect, ImageAttributes imageAttr)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            IntPtr attr = imageAttr == null ? IntPtr.Zero : imageAttr.NativeObject;
            Status status = GDIPlus.GdipCreateTextureIA(image.nativeObject, attr, dstRect.X, dstRect.Y,
                dstRect.Width, dstRect.Height, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        public TextureBrush(Image image, WrapMode wrapMode, Rectangle dstRect)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            if ((wrapMode < WrapMode.Tile) || (wrapMode > WrapMode.Clamp))
                throw new InvalidEnumArgumentException("WrapMode");

            Status status = GDIPlus.GdipCreateTexture2I(image.nativeObject, wrapMode, dstRect.X, dstRect.Y,
                dstRect.Width, dstRect.Height, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        public TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            if ((wrapMode < WrapMode.Tile) || (wrapMode > WrapMode.Clamp))
                throw new InvalidEnumArgumentException("WrapMode");

            Status status = GDIPlus.GdipCreateTexture2(image.nativeObject, wrapMode, dstRect.X, dstRect.Y,
                dstRect.Width, dstRect.Height, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        // properties

        public Image Image
        {
            get
            {
                // this check is required here as GDI+ doesn't check for it 
                if (nativeObject == IntPtr.Zero)
                    throw new ArgumentException("Object was disposed");

                IntPtr img;
                Status status = GDIPlus.GdipGetTextureImage(nativeObject, out img);
                GDIPlus.CheckStatus(status);
                return new Bitmap(img);
            }
        }

        public Matrix Transform
        {
            get
            {
                Matrix matrix = new Matrix();
                Status status = GDIPlus.GdipGetTextureTransform(nativeObject, matrix.nativeMatrix);
                GDIPlus.CheckStatus(status);

                return matrix;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Transform");

                Status status = GDIPlus.GdipSetTextureTransform(nativeObject, value.nativeMatrix);
                GDIPlus.CheckStatus(status);
            }
        }

        public WrapMode WrapMode
        {
            get
            {
                WrapMode mode;
                Status status = GDIPlus.GdipGetTextureWrapMode(nativeObject, out mode);
                GDIPlus.CheckStatus(status);
                return mode;
            }
            set
            {
                if ((value < WrapMode.Tile) || (value > WrapMode.Clamp))
                    throw new InvalidEnumArgumentException("WrapMode");

                Status status = GDIPlus.GdipSetTextureWrapMode(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        // public methods

        public override object Clone()
        {
            IntPtr clonePtr;
            Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonePtr);
            GDIPlus.CheckStatus(status);

            return new TextureBrush(clonePtr);
        }

        public void MultiplyTransform(Matrix matrix)
        {
            MultiplyTransform(matrix, MatrixOrder.Prepend);
        }

        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            Status status = GDIPlus.GdipMultiplyTextureTransform(nativeObject, matrix.nativeMatrix, order);
            GDIPlus.CheckStatus(status);
        }

        public void ResetTransform()
        {
            Status status = GDIPlus.GdipResetTextureTransform(nativeObject);
            GDIPlus.CheckStatus(status);
        }

        public void RotateTransform(float angle)
        {
            RotateTransform(angle, MatrixOrder.Prepend);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            Status status = GDIPlus.GdipRotateTextureTransform(nativeObject, angle, order);
            GDIPlus.CheckStatus(status);
        }

        public void ScaleTransform(float sx, float sy)
        {
            ScaleTransform(sx, sy, MatrixOrder.Prepend);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            Status status = GDIPlus.GdipScaleTextureTransform(nativeObject, sx, sy, order);
            GDIPlus.CheckStatus(status);
        }

        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            Status status = GDIPlus.GdipTranslateTextureTransform(nativeObject, dx, dy, order);
            GDIPlus.CheckStatus(status);
        }
    }
}
