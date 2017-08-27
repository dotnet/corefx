// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.LinearGradientBrush unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2006, 2008 Novell, Inc (http://www.novell.com)
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
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Drawing2D
{

    public class LinearGradientBrushTest
    {

        private Point pt1;
        private Point pt2;
        private Color c1;
        private Color c2;
        private LinearGradientBrush default_brush;
        private Matrix empty_matrix;
        private RectangleF rect;

        public LinearGradientBrushTest()
        {
            pt1 = new Point(0, 0);
            pt2 = new Point(32, 32);
            c1 = Color.Blue;
            c2 = Color.Red;
            default_brush = new LinearGradientBrush(pt1, pt2, c1, c2);
            empty_matrix = new Matrix();
            rect = new RectangleF(0, 0, 32, 32);
        }

        private void CheckDefaultRectangle(string msg, RectangleF rect)
        {
            Assert.Equal(pt1.X, rect.X);
            Assert.Equal(pt1.Y, rect.Y);
            Assert.Equal(pt2.X, rect.Width);
            Assert.Equal(pt2.Y, rect.Height);
        }

        private void CheckDefaultMatrix(Matrix matrix)
        {
            float[] elements = matrix.Elements;
            Assert.Equal(1.0f, elements[0], 1);
            Assert.Equal(1.0f, elements[1], 1);
            Assert.Equal(-1.0f, elements[2], 1);
            Assert.Equal(1.0f, elements[3], 1);
            Assert.Equal(16.0f, elements[4], 1);
            Assert.Equal(-16.0f, elements[5], 1);
        }

        private void CheckBrushAt45(LinearGradientBrush lgb)
        {
            CheckDefaultRectangle("4", lgb.Rectangle);
            Assert.Equal(1, lgb.Blend.Factors.Length);
            Assert.Equal(1, lgb.Blend.Factors[0]);
            Assert.Equal(1, lgb.Blend.Positions.Length);
            // lgb.Blend.Positions [0] is always small (e-39) but never quite the same
            Assert.False(lgb.GammaCorrection);
            Assert.Equal(2, lgb.LinearColors.Length);
            Assert.NotNull(lgb.Transform);
            CheckDefaultMatrix(lgb.Transform);
        }

        private void CheckMatrixAndRect(PointF pt1, PointF pt2, float[] testVals)
        {
            Matrix m;
            RectangleF rect;

            using (LinearGradientBrush b = new LinearGradientBrush(pt1, pt2, Color.Black, Color.White))
            {
                m = b.Transform;
                rect = b.Rectangle;
            }

            Assert.Equal(testVals[0], m.Elements[0], 3);
            Assert.Equal(testVals[1], m.Elements[1], 3);
            Assert.Equal(testVals[2], m.Elements[2], 3);
            Assert.Equal(testVals[3], m.Elements[3], 3);
            Assert.Equal(testVals[4], m.Elements[4], 3);
            Assert.Equal(testVals[5], m.Elements[5], 3);

            Assert.Equal(testVals[6], rect.X, 3);
            Assert.Equal(testVals[7], rect.Y, 3);
            Assert.Equal(testVals[8], rect.Width, 3);
            Assert.Equal(testVals[9], rect.Height, 3);
        }

        private void CheckMatrixForScalableAngle(RectangleF rect, float angle, float[] testVals)
        {
            Matrix m;

            using (LinearGradientBrush b = new LinearGradientBrush(rect, Color.Firebrick, Color.Lavender, angle, true))
            {
                m = b.Transform;
            }

            Assert.Equal(testVals[0], m.Elements[0], 3);
            Assert.Equal(testVals[1], m.Elements[1], 3);
            Assert.Equal(testVals[2], m.Elements[2], 3);
            Assert.Equal(testVals[3], m.Elements[3], 3);
            Assert.Equal(testVals[4], m.Elements[4], 3);
            Assert.Equal(testVals[5], m.Elements[5], 3);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Point_Color_Color()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            CheckBrushAt45(lgb);

            Assert.Equal(WrapMode.Tile, lgb.WrapMode);
            lgb.WrapMode = WrapMode.TileFlipX;
            Assert.Equal(WrapMode.TileFlipX, lgb.WrapMode);
            lgb.WrapMode = WrapMode.TileFlipY;
            Assert.Equal(WrapMode.TileFlipY, lgb.WrapMode);
            lgb.WrapMode = WrapMode.TileFlipXY;
            Assert.Equal(WrapMode.TileFlipXY, lgb.WrapMode);
            // can't set WrapMode.Clamp
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Point_Color_Color_1()
        {
            PointF pt1 = new Point(100, 200);
            PointF pt2 = new Point(200, 200);
            CheckMatrixAndRect(pt1, pt2, new float[] { 1, 0, 0, 1, 0, 0, 100, 150, 100, 100 });

            pt1 = new Point(100, 200);
            pt2 = new Point(0, 200);
            CheckMatrixAndRect(pt1, pt2, new float[] { -1, 0, 0, -1, 100, 400, 0, 150, 100, 100 });

            pt1 = new Point(100, 200);
            pt2 = new Point(100, 300);
            CheckMatrixAndRect(pt1, pt2, new float[] { 0, 1, -1, 0, 350, 150, 50, 200, 100, 100 });

            pt1 = new Point(100, 200);
            pt2 = new Point(100, 100);
            CheckMatrixAndRect(pt1, pt2, new float[] { 0, -1, 1, 0, -50, 250, 50, 100, 100, 100 });

            pt1 = new Point(100, 100);
            pt2 = new Point(150, 225);
            CheckMatrixAndRect(pt1, pt2, new float[] { 1, 2.5f, -0.6896552f, 0.2758622f, 112.069f, -194.8276f, 100, 100, 50, 125 });

            pt1 = new Point(100, 100);
            pt2 = new Point(55, 200);
            CheckMatrixAndRect(pt1, pt2, new float[] { -1, 2.222222f, -0.7484408f, -0.3367983f, 267.2661f, 28.29753f, 55, 100, 45, 100 });

            pt1 = new Point(100, 100);
            pt2 = new Point(150, 60);
            CheckMatrixAndRect(pt1, pt2, new float[] { 1, -0.8000001f, 0.9756095f, 1.219512f, -78.04876f, 82.43903f, 100, 60, 50, 40 });

            pt1 = new Point(100, 100);
            pt2 = new Point(27, 59);
            CheckMatrixAndRect(pt1, pt2, new float[] { -1, -0.5616435f, 0.8539224f, -1.520399f, 59.11317f, 236.0361f, 27, 59, 73, 41 });
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_0()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            Assert.Equal(1, lgb.Blend.Factors.Length);
            Assert.Equal(1, lgb.Blend.Factors[0]);
            Assert.Equal(1, lgb.Blend.Positions.Length);
            // lgb.Blend.Positions [0] is always small (e-39) but never quite the same
            Assert.False(lgb.GammaCorrection);
            Assert.Equal(c1.ToArgb(), lgb.LinearColors[0].ToArgb());
            Assert.Equal(c2.ToArgb(), lgb.LinearColors[1].ToArgb());
            Assert.Equal(rect, lgb.Rectangle);
            Assert.True(lgb.Transform.IsIdentity);
            Assert.Equal(WrapMode.Tile, lgb.WrapMode);

            Matrix matrix = new Matrix(2, -1, 1, 2, 10, 10);
            lgb.Transform = matrix;
            Assert.Equal(matrix, lgb.Transform);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_22_5()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 22.5f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(1.207107, elements[0], 4);
            Assert.Equal(0.5, elements[1], 4);
            Assert.Equal(-0.5, elements[2], 4);
            Assert.Equal(1.207107, elements[3], 4);
            Assert.Equal(4.686291, elements[4], 4);
            Assert.Equal(-11.313709, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_45()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 45f);
            CheckBrushAt45(lgb);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_90()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 90f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(0, elements[0], 4);
            Assert.Equal(1, elements[1], 4);
            Assert.Equal(-1, elements[2], 4);
            Assert.Equal(0, elements[3], 4);
            Assert.Equal(32, elements[4], 4);
            Assert.Equal(0, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_135()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 135f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(-1, elements[0], 4);
            Assert.Equal(1, elements[1], 4);
            Assert.Equal(-1, elements[2], 4);
            Assert.Equal(-1, elements[3], 4);
            Assert.Equal(48, elements[4], 4);
            Assert.Equal(16, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_180()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 180f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(-1, elements[0], 4);
            Assert.Equal(0, elements[1], 4);
            Assert.Equal(0, elements[2], 4);
            Assert.Equal(-1, elements[3], 4);
            Assert.Equal(32, elements[4], 4);
            Assert.Equal(32, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_270()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 270f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(0, elements[0], 4);
            Assert.Equal(-1, elements[1], 4);
            Assert.Equal(1, elements[2], 4);
            Assert.Equal(0, elements[3], 4);
            Assert.Equal(0, elements[4], 4);
            Assert.Equal(32, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_315()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 315f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(1, elements[0], 4);
            Assert.Equal(-1, elements[1], 4);
            Assert.Equal(1, elements[2], 4);
            Assert.Equal(1, elements[3], 4);
            Assert.Equal(-16, elements[4], 4);
            Assert.Equal(16, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_360()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 360f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            // just like 0'
            Assert.Equal(1, elements[0], 4);
            Assert.Equal(0, elements[1], 4);
            Assert.Equal(0, elements[2], 4);
            Assert.Equal(1, elements[3], 4);
            Assert.Equal(0, elements[4], 4);
            Assert.Equal(0, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectangleF_Color_Color_Single_540()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 540f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            float[] elements = lgb.Transform.Elements;
            // just like 180'
            Assert.Equal(-1, elements[0], 4);
            Assert.Equal(0, elements[1], 4);
            Assert.Equal(0, elements[2], 4);
            Assert.Equal(-1, elements[3], 4);
            Assert.Equal(32, elements[4], 4);
            Assert.Equal(32, elements[5], 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void InterpolationColors_Colors_InvalidBlend()
        {
            // default Blend doesn't allow getting this property
            Assert.Throws<ArgumentException>(() => { var x = default_brush.InterpolationColors.Colors; });
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void InterpolationColors_Positions_InvalidBlend()
        {
            // default Blend doesn't allow getting this property
            Assert.Throws<ArgumentException>(() => { var x = default_brush.InterpolationColors.Positions; });
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void LinearColors_Empty()
        {
            Assert.Throws<IndexOutOfRangeException>(() => default_brush.LinearColors = new Color[0]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void LinearColors_One()
        {
            Assert.Throws<IndexOutOfRangeException>(() => default_brush.LinearColors = new Color[1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void LinearColors_Two()
        {
            Assert.Equal(Color.FromArgb(255, 0, 0, 255), default_brush.LinearColors[0]);
            Assert.Equal(Color.FromArgb(255, 255, 0, 0), default_brush.LinearColors[1]);

            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            lgb.LinearColors = new Color[2] { Color.Black, Color.White };
            // not the same, the alpha is changed to 255 so they can't compare
            Assert.Equal(Color.FromArgb(255, 0, 0, 0), lgb.LinearColors[0]);
            Assert.Equal(Color.FromArgb(255, 255, 255, 255), lgb.LinearColors[1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void LinearColors_Three()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            lgb.LinearColors = new Color[3] { Color.Red, Color.Green, Color.Blue };
            // not the same, the alpha is changed to 255 so they can't compare
            Assert.Equal(Color.FromArgb(255, 255, 0, 0), lgb.LinearColors[0]);
            Assert.Equal(Color.FromArgb(255, 0, 128, 0), lgb.LinearColors[1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Rectangle()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            CheckDefaultRectangle("Original", lgb.Rectangle);
            lgb.MultiplyTransform(new Matrix(2, 0, 0, 2, 2, 2));
            CheckDefaultRectangle("Multiply", lgb.Rectangle);
            lgb.ResetTransform();
            CheckDefaultRectangle("Reset", lgb.Rectangle);
            lgb.RotateTransform(90);
            CheckDefaultRectangle("Rotate", lgb.Rectangle);
            lgb.ScaleTransform(4, 0.25f);
            CheckDefaultRectangle("Scale", lgb.Rectangle);
            lgb.TranslateTransform(-10, -20);
            CheckDefaultRectangle("Translate", lgb.Rectangle);

            lgb.SetBlendTriangularShape(0.5f);
            CheckDefaultRectangle("SetBlendTriangularShape", lgb.Rectangle);
            lgb.SetSigmaBellShape(0.5f);
            CheckDefaultRectangle("SetSigmaBellShape", lgb.Rectangle);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Null()
        {
            Assert.Throws<ArgumentNullException>(() => default_brush.Transform = null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Empty()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            lgb.Transform = new Matrix();
            Assert.True(lgb.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_NonInvertible()
        {
            Assert.Throws<ArgumentException>(() => default_brush.Transform = new Matrix(123, 24, 82, 16, 47, 30));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WrapMode_AllValid()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            lgb.WrapMode = WrapMode.Tile;
            Assert.Equal(WrapMode.Tile, lgb.WrapMode);
            lgb.WrapMode = WrapMode.TileFlipX;
            Assert.Equal(WrapMode.TileFlipX, lgb.WrapMode);
            lgb.WrapMode = WrapMode.TileFlipY;
            Assert.Equal(WrapMode.TileFlipY, lgb.WrapMode);
            lgb.WrapMode = WrapMode.TileFlipXY;
            Assert.Equal(WrapMode.TileFlipXY, lgb.WrapMode);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WrapMode_Clamp()
        {
            Assert.Throws<ArgumentException>(() => default_brush.WrapMode = WrapMode.Clamp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void WrapMode_Invalid()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => default_brush.WrapMode = (WrapMode)Int32.MinValue);
        }


        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Clone()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            LinearGradientBrush clone = (LinearGradientBrush)lgb.Clone();
            Assert.Equal(lgb.Blend.Factors.Length, clone.Blend.Factors.Length);
            Assert.Equal(lgb.Blend.Positions.Length, clone.Blend.Positions.Length);
            Assert.Equal(lgb.GammaCorrection, clone.GammaCorrection);
            Assert.Equal(lgb.LinearColors.Length, clone.LinearColors.Length);
            Assert.Equal(lgb.LinearColors.Length, clone.LinearColors.Length);
            Assert.Equal(lgb.Rectangle, clone.Rectangle);
            Assert.Equal(lgb.Transform, clone.Transform);
            Assert.Equal(lgb.WrapMode, clone.WrapMode);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform1_Null()
        {
            Assert.Throws<ArgumentNullException>(() => default_brush.MultiplyTransform(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform2_Null()
        {
            Assert.Throws<ArgumentNullException>(() => default_brush.MultiplyTransform(null, MatrixOrder.Append));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform2_Invalid()
        {
            default_brush.MultiplyTransform(empty_matrix, (MatrixOrder)Int32.MinValue);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform_NonInvertible()
        {
            Matrix noninvertible = new Matrix(123, 24, 82, 16, 47, 30);
            Assert.Throws<ArgumentException>(() => default_brush.MultiplyTransform(noninvertible));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ResetTransform()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            Assert.False(lgb.Transform.IsIdentity);
            lgb.ResetTransform();
            Assert.True(lgb.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateTransform()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            lgb.RotateTransform(90);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(0, elements[0], 1);
            Assert.Equal(1, elements[1], 1);
            Assert.Equal(-1, elements[2], 1);
            Assert.Equal(0, elements[3], 1);
            Assert.Equal(0, elements[4], 1);
            Assert.Equal(0, elements[5], 1);

            lgb.RotateTransform(270);
            Assert.True(lgb.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateTransform_InvalidOrder()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            Assert.Throws<ArgumentException>(() => lgb.RotateTransform(720, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            lgb.ScaleTransform(2, 4);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(2, elements[0], 1);
            Assert.Equal(0, elements[1], 1);
            Assert.Equal(0, elements[2], 1);
            Assert.Equal(4, elements[3], 1);
            Assert.Equal(0, elements[4], 1);
            Assert.Equal(0, elements[5], 1);

            lgb.ScaleTransform(0.5f, 0.25f);
            Assert.True(lgb.Transform.IsIdentity);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform_45()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 45f);
            lgb.ScaleTransform(3, 3);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(3, elements[0], 1);
            Assert.Equal(3, elements[1], 1);
            Assert.Equal(-3, elements[2], 1);
            Assert.Equal(3, elements[3], 1);
            Assert.Equal(16, elements[4], 1);
            Assert.Equal(-16, elements[5], 1);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform_MaxMin()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            lgb.ScaleTransform(Single.MaxValue, Single.MinValue);
            float[] elements = lgb.Transform.Elements;
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
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            Assert.Throws<ArgumentException>(() => lgb.ScaleTransform(1, 1, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_Focus()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            // max valid
            lgb.SetBlendTriangularShape(1);
            Assert.True(lgb.Transform.IsIdentity);
            // min valid
            lgb.SetBlendTriangularShape(0);
            Assert.True(lgb.Transform.IsIdentity);
            // middle
            lgb.SetBlendTriangularShape(0.5f);
            Assert.True(lgb.Transform.IsIdentity);
            // no impact on matrix
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_Scale()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            // max valid
            lgb.SetBlendTriangularShape(0, 1);
            Assert.True(lgb.Transform.IsIdentity);
            // min valid
            lgb.SetBlendTriangularShape(1, 0);
            Assert.True(lgb.Transform.IsIdentity);
            // middle
            lgb.SetBlendTriangularShape(0.5f, 0.5f);
            Assert.True(lgb.Transform.IsIdentity);
            // no impact on matrix
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_FocusTooSmall()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetBlendTriangularShape(-1));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_FocusTooBig()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetBlendTriangularShape(1.01f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_ScaleTooSmall()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetBlendTriangularShape(1, -1));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_ScaleTooBig()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetBlendTriangularShape(1, 1.01f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_Focus()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            // max valid
            lgb.SetSigmaBellShape(1);
            Assert.True(lgb.Transform.IsIdentity);
            // min valid
            lgb.SetSigmaBellShape(0);
            Assert.True(lgb.Transform.IsIdentity);
            // middle
            lgb.SetSigmaBellShape(0.5f);
            Assert.True(lgb.Transform.IsIdentity);
            // no impact on matrix
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_Scale()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            // max valid
            lgb.SetSigmaBellShape(0, 1);
            Assert.True(lgb.Transform.IsIdentity);
            // min valid
            lgb.SetSigmaBellShape(1, 0);
            Assert.True(lgb.Transform.IsIdentity);
            // middle
            lgb.SetSigmaBellShape(0.5f, 0.5f);
            Assert.True(lgb.Transform.IsIdentity);
            // no impact on matrix
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_FocusTooSmall()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetSigmaBellShape(-1));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_FocusTooBig()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetSigmaBellShape(1.01f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_ScaleTooSmall()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetSigmaBellShape(1, -1));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_ScaleTooBig()
        {
            Assert.Throws<ArgumentException>(() => default_brush.SetSigmaBellShape(1, 1.01f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TranslateTransform()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 0f);
            lgb.TranslateTransform(1, 1);
            float[] elements = lgb.Transform.Elements;
            Assert.Equal(1, elements[0], 1);
            Assert.Equal(0, elements[1], 1);
            Assert.Equal(0, elements[2], 1);
            Assert.Equal(1, elements[3], 1);
            Assert.Equal(1, elements[4], 1);
            Assert.Equal(1, elements[5], 1);

            lgb.TranslateTransform(-1, -1);
            // strangely lgb.Transform.IsIdentity is false
            elements = lgb.Transform.Elements;
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
            LinearGradientBrush lgb = new LinearGradientBrush(pt1, pt2, c1, c2);
            Assert.Throws<ArgumentException>(() => lgb.TranslateTransform(1, 1, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Operations()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 45f);
            Matrix clone = lgb.Transform.Clone();
            Matrix mul = clone.Clone();

            clone.Multiply(mul, MatrixOrder.Append);
            lgb.MultiplyTransform(mul, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Multiply(mul, MatrixOrder.Prepend);
            lgb.MultiplyTransform(mul, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Rotate(45, MatrixOrder.Append);
            lgb.RotateTransform(45, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Rotate(45, MatrixOrder.Prepend);
            lgb.RotateTransform(45, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Scale(0.25f, 2, MatrixOrder.Append);
            lgb.ScaleTransform(0.25f, 2, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Scale(0.25f, 2, MatrixOrder.Prepend);
            lgb.ScaleTransform(0.25f, 2, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Translate(10, 20, MatrixOrder.Append);
            lgb.TranslateTransform(10, 20, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Translate(30, 40, MatrixOrder.Prepend);
            lgb.TranslateTransform(30, 40, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Reset();
            lgb.ResetTransform();
            Assert.Equal(lgb.Transform, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Operations_OnScalableAngle()
        {
            LinearGradientBrush lgb = new LinearGradientBrush(rect, c1, c2, 360f, true);
            Matrix clone = lgb.Transform.Clone();
            Matrix mul = clone.Clone();
            Matrix m = new Matrix();
            m.Scale(2, 1);
            m.Translate(rect.Width, rect.Height);
            m.Rotate(30f);

            clone.Multiply(mul, MatrixOrder.Append);
            lgb.MultiplyTransform(mul, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Multiply(mul, MatrixOrder.Prepend);
            lgb.MultiplyTransform(mul, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Rotate(45, MatrixOrder.Append);
            lgb.RotateTransform(45, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Rotate(45, MatrixOrder.Prepend);
            lgb.RotateTransform(45, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Scale(0.25f, 2, MatrixOrder.Append);
            lgb.ScaleTransform(0.25f, 2, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Scale(0.25f, 2, MatrixOrder.Prepend);
            lgb.ScaleTransform(0.25f, 2, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Translate(10, 20, MatrixOrder.Append);
            lgb.TranslateTransform(10, 20, MatrixOrder.Append);
            Assert.Equal(lgb.Transform, clone);

            clone.Translate(30, 40, MatrixOrder.Prepend);
            lgb.TranslateTransform(30, 40, MatrixOrder.Prepend);
            Assert.Equal(lgb.Transform, clone);

            clone.Reset();
            lgb.ResetTransform();
            Assert.Equal(lgb.Transform, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Rectangle_Angle_Scalable()
        {
            CheckMatrixForScalableAngle(new RectangleF(0, 0, 10, 10), 15, new float[] { 1.183013f, 0.3169873f, -0.3169873f, 1.183012f, 0.6698728f, -2.5f });

            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 50), 15, new float[] { 1.183012f, 0.176104f, -0.5705772f, 1.183012f, 34.77311f, -28.76387f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 50), 75, new float[] { 0.3169872f, 0.6572293f, -2.129423f, 0.3169873f, 232.2269f, 8.763878f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 50), 95, new float[] { -0.09442029f, 0.599571f, -1.942611f, -0.09442017f, 247.2034f, 48.05788f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 50), 150, new float[] { -1.183013f, 0.3794515f, -1.229423f, -1.183013f, 268.2269f, 157.0972f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 50), 215, new float[] { -1.140856f, -0.4437979f, 1.437905f, -1.140856f, 38.34229f, 215.2576f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 50), 300, new float[] { 0.6830127f, -0.6572294f, 2.129422f, 0.6830124f, -157.2269f, 76.23613f });

            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 150), 15, new float[] { 1.183012f, 0.5283121f, -0.1901924f, 1.183012f, 11.95002f, -64.33012f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 150), 75, new float[] { 0.3169872f, 1.971688f, -0.7098077f, 0.3169872f, 147.05f, -55.66987f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 150), 95, new float[] { -0.09442029f, 1.798713f, -0.6475369f, -0.09442022f, 169.499f, 12.84323f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 150), 150, new float[] { -1.183013f, 1.138354f, -0.4098077f, -1.183013f, 219.05f, 209.3301f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 150), 215, new float[] { -1.140856f, -1.331394f, 0.4793016f, -1.140856f, 95.85849f, 388.8701f });
            CheckMatrixForScalableAngle(new RectangleF(30, 60, 90, 150), 300, new float[] { 0.6830127f, -1.971688f, 0.7098075f, 0.6830125f, -72.04998f, 190.6699f });
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void LinearColors_Null()
        {
            Assert.Throws<NullReferenceException>(() => default_brush.LinearColors = null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void InterpolationColors_Null()
        {
            Assert.Throws<ArgumentException>(() => default_brush.InterpolationColors = null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Blend_Null()
        {
            Assert.Throws<NullReferenceException>(() => default_brush.Blend = null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ZeroWidthRectangle()
        {
            Rectangle r = new Rectangle(10, 10, 0, 10);
            Assert.Equal(0, r.Width);
            Assert.Throws<ArgumentException>(() => new LinearGradientBrush(r, Color.Red, Color.Blue, LinearGradientMode.Vertical));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ZeroHeightRectangleF()
        {
            RectangleF r = new RectangleF(10.0f, 10.0f, 10.0f, 0.0f);
            Assert.Equal(0.0f, r.Height);
            Assert.Throws<ArgumentException>(() => new LinearGradientBrush(r, Color.Red, Color.Blue, LinearGradientMode.Vertical));
        }
    }
}
