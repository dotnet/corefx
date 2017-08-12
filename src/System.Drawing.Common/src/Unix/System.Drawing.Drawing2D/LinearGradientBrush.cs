// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.LinearGradientBrush.cs
//
// Authors:
//   Dennis Hayes (dennish@Raytek.com)
//   Ravindra (rkumar@novell.com)
//
// Copyright (C) 2002/3 Ximian, Inc. http://www.ximian.com
// Copyright (C) 2004,2006 Novell, Inc (http://www.novell.com)
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
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{

    public sealed class LinearGradientBrush : Brush
    {
        RectangleF rectangle;

        internal LinearGradientBrush(IntPtr native)
        {
            Status status = SafeNativeMethods.Gdip.GdipGetLineRect(native, out rectangle);
            SetNativeBrush(native);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public LinearGradientBrush(Point point1, Point point2, Color color1, Color color2)
        {
            IntPtr nativeObject;
            Status status = SafeNativeMethods.Gdip.GdipCreateLineBrushI(ref point1, ref point2, color1.ToArgb(), color2.ToArgb(), WrapMode.Tile, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
            SetNativeBrush(nativeObject);

            status = SafeNativeMethods.Gdip.GdipGetLineRect(nativeObject, out rectangle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
        {
            IntPtr nativeObject;
            Status status = SafeNativeMethods.Gdip.GdipCreateLineBrush(ref point1, ref point2, color1.ToArgb(), color2.ToArgb(), WrapMode.Tile, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
            SetNativeBrush(nativeObject);

            status = SafeNativeMethods.Gdip.GdipGetLineRect(nativeObject, out rectangle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
        {
            IntPtr nativeObject;
            Status status = SafeNativeMethods.Gdip.GdipCreateLineBrushFromRectI(ref rect, color1.ToArgb(), color2.ToArgb(), linearGradientMode, WrapMode.Tile, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
            SetNativeBrush(nativeObject);

            rectangle = (RectangleF)rect;
        }

        public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle) : this(rect, color1, color2, angle, false)
        {
        }

        public LinearGradientBrush(RectangleF rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
        {
            IntPtr nativeObject;
            Status status = SafeNativeMethods.Gdip.GdipCreateLineBrushFromRect(ref rect, color1.ToArgb(), color2.ToArgb(), linearGradientMode, WrapMode.Tile, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
            SetNativeBrush(nativeObject);

            rectangle = rect;
        }

        public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle) : this(rect, color1, color2, angle, false)
        {
        }

        public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle, bool isAngleScaleable)
        {
            IntPtr nativeObject;
            Status status = SafeNativeMethods.Gdip.GdipCreateLineBrushFromRectWithAngleI(ref rect, color1.ToArgb(), color2.ToArgb(), angle, isAngleScaleable, WrapMode.Tile, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
            SetNativeBrush(nativeObject);

            rectangle = (RectangleF)rect;
        }

        public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle, bool isAngleScaleable)
        {
            IntPtr nativeObject;
            Status status = SafeNativeMethods.Gdip.GdipCreateLineBrushFromRectWithAngle(ref rect, color1.ToArgb(), color2.ToArgb(), angle, isAngleScaleable, WrapMode.Tile, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
            SetNativeBrush(nativeObject);

            rectangle = rect;
        }

        // Public Properties

        public Blend Blend
        {
            get
            {
                int count;
                Status status = SafeNativeMethods.Gdip.GdipGetLineBlendCount(NativeBrush, out count);
                SafeNativeMethods.Gdip.CheckStatus(status);
                float[] factors = new float[count];
                float[] positions = new float[count];
                status = SafeNativeMethods.Gdip.GdipGetLineBlend(NativeBrush, factors, positions, count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                Blend blend = new Blend();
                blend.Factors = factors;
                blend.Positions = positions;

                return blend;
            }
            set
            {
                // no null check, MS throws a NullReferenceException here
                int count;
                float[] factors = value.Factors;
                float[] positions = value.Positions;
                count = factors.Length;

                if (count == 0 || positions.Length == 0)
                    throw new ArgumentException("Invalid Blend object. It should have at least 2 elements in each of the factors and positions arrays.");

                if (count != positions.Length)
                    throw new ArgumentException("Invalid Blend object. It should contain the same number of factors and positions values.");

                if (positions[0] != 0.0F)
                    throw new ArgumentException("Invalid Blend object. The positions array must have 0.0 as its first element.");

                if (positions[count - 1] != 1.0F)
                    throw new ArgumentException("Invalid Blend object. The positions array must have 1.0 as its last element.");

                Status status = SafeNativeMethods.Gdip.GdipSetLineBlend(NativeBrush, factors, positions, count);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        [MonoTODO("The GammaCorrection value is ignored when using libgdiplus.")]
        public bool GammaCorrection
        {
            get
            {
                bool gammaCorrection;
                Status status = SafeNativeMethods.Gdip.GdipGetLineGammaCorrection(NativeBrush, out gammaCorrection);
                SafeNativeMethods.Gdip.CheckStatus(status);
                return gammaCorrection;
            }
            set
            {
                Status status = SafeNativeMethods.Gdip.GdipSetLineGammaCorrection(NativeBrush, value);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public ColorBlend InterpolationColors
        {
            get
            {
                int count;
                Status status = SafeNativeMethods.Gdip.GdipGetLinePresetBlendCount(NativeBrush, out count);
                SafeNativeMethods.Gdip.CheckStatus(status);
                int[] intcolors = new int[count];
                float[] positions = new float[count];
                status = SafeNativeMethods.Gdip.GdipGetLinePresetBlend(NativeBrush, intcolors, positions, count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                ColorBlend interpolationColors = new ColorBlend();
                Color[] colors = new Color[count];
                for (int i = 0; i < count; i++)
                    colors[i] = Color.FromArgb(intcolors[i]);
                interpolationColors.Colors = colors;
                interpolationColors.Positions = positions;

                return interpolationColors;
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("InterpolationColors is null");
                int count;
                Color[] colors = value.Colors;
                float[] positions = value.Positions;
                count = colors.Length;

                if (count == 0 || positions.Length == 0)
                    throw new ArgumentException("Invalid ColorBlend object. It should have at least 2 elements in each of the colors and positions arrays.");

                if (count != positions.Length)
                    throw new ArgumentException("Invalid ColorBlend object. It should contain the same number of positions and color values.");

                if (positions[0] != 0.0F)
                    throw new ArgumentException("Invalid ColorBlend object. The positions array must have 0.0 as its first element.");

                if (positions[count - 1] != 1.0F)
                    throw new ArgumentException("Invalid ColorBlend object. The positions array must have 1.0 as its last element.");

                int[] blend = new int[colors.Length];
                for (int i = 0; i < colors.Length; i++)
                    blend[i] = colors[i].ToArgb();

                Status status = SafeNativeMethods.Gdip.GdipSetLinePresetBlend(NativeBrush, blend, positions, count);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public Color[] LinearColors
        {
            get
            {
                int[] colors = new int[2];
                Status status = SafeNativeMethods.Gdip.GdipGetLineColors(NativeBrush, colors);
                SafeNativeMethods.Gdip.CheckStatus(status);
                Color[] linearColors = new Color[2];
                linearColors[0] = Color.FromArgb(colors[0]);
                linearColors[1] = Color.FromArgb(colors[1]);

                return linearColors;
            }
            set
            {
                // no null check, MS throws a NullReferenceException here
                Status status = SafeNativeMethods.Gdip.GdipSetLineColors(NativeBrush, value[0].ToArgb(), value[1].ToArgb());
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public RectangleF Rectangle
        {
            get
            {
                return rectangle;
            }
        }

        public Matrix Transform
        {
            get
            {
                Matrix matrix = new Matrix();
                Status status = SafeNativeMethods.Gdip.GdipGetLineTransform(NativeBrush, matrix.nativeMatrix);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return matrix;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Transform");

                Status status = SafeNativeMethods.Gdip.GdipSetLineTransform(NativeBrush, value.nativeMatrix);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public WrapMode WrapMode
        {
            get
            {
                WrapMode wrapMode;
                Status status = SafeNativeMethods.Gdip.GdipGetLineWrapMode(NativeBrush, out wrapMode);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return wrapMode;
            }
            set
            {
                // note: Clamp isn't valid (context wise) but it is checked in libgdiplus
                if ((value < WrapMode.Tile) || (value > WrapMode.Clamp))
                    throw new InvalidEnumArgumentException("WrapMode");

                Status status = SafeNativeMethods.Gdip.GdipSetLineWrapMode(NativeBrush, value);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        // Public Methods

        public void MultiplyTransform(Matrix matrix)
        {
            MultiplyTransform(matrix, MatrixOrder.Prepend);
        }

        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            Status status = SafeNativeMethods.Gdip.GdipMultiplyLineTransform(NativeBrush, matrix.nativeMatrix, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void ResetTransform()
        {
            Status status = SafeNativeMethods.Gdip.GdipResetLineTransform(NativeBrush);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void RotateTransform(float angle)
        {
            RotateTransform(angle, MatrixOrder.Prepend);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            Status status = SafeNativeMethods.Gdip.GdipRotateLineTransform(NativeBrush, angle, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void ScaleTransform(float sx, float sy)
        {
            ScaleTransform(sx, sy, MatrixOrder.Prepend);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            Status status = SafeNativeMethods.Gdip.GdipScaleLineTransform(NativeBrush, sx, sy, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void SetBlendTriangularShape(float focus)
        {
            SetBlendTriangularShape(focus, 1.0F);
        }

        public void SetBlendTriangularShape(float focus, float scale)
        {
            if (focus < 0 || focus > 1 || scale < 0 || scale > 1)
                throw new ArgumentException("Invalid parameter passed.");

            Status status = SafeNativeMethods.Gdip.GdipSetLineLinearBlend(NativeBrush, focus, scale);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void SetSigmaBellShape(float focus)
        {
            SetSigmaBellShape(focus, 1.0F);
        }

        public void SetSigmaBellShape(float focus, float scale)
        {
            if (focus < 0 || focus > 1 || scale < 0 || scale > 1)
                throw new ArgumentException("Invalid parameter passed.");

            Status status = SafeNativeMethods.Gdip.GdipSetLineSigmaBlend(NativeBrush, focus, scale);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            Status status = SafeNativeMethods.Gdip.GdipTranslateLineTransform(NativeBrush, dx, dy, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public override object Clone()
        {
            IntPtr clonePtr;
            int status = SafeNativeMethods.Gdip.GdipCloneBrush(new HandleRef(this, NativeBrush), out clonePtr);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new LinearGradientBrush(clonePtr);
        }
    }
}
