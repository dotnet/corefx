// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.PathGradientBrush unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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

    public class PathGradientBrushTest
    {

        private Point[] pts_2i;
        private PointF[] pts_2f;
        private Matrix empty_matrix;

        private void CheckDefaultRectangle(string message, RectangleF rect)
        {
            Assert.Equal(1f, rect.X);
            Assert.Equal(2f, rect.Y);
            Assert.Equal(19f, rect.Width);
            Assert.Equal(28f, rect.Height);
        }

        private void CheckDefaults(PathGradientBrush pgb)
        {
            Assert.Equal(1, pgb.Blend.Factors.Length);
            Assert.Equal(1f, pgb.Blend.Factors[0]);
            Assert.Equal(1, pgb.Blend.Positions.Length);
            Assert.Equal(0f, pgb.Blend.Positions[0]);
            Assert.Equal(10.5f, pgb.CenterPoint.X);
            Assert.Equal(16f, pgb.CenterPoint.Y);
            Assert.True(pgb.FocusScales.IsEmpty);
            Assert.Equal(1, pgb.InterpolationColors.Colors.Length);
            Assert.Equal(0, pgb.InterpolationColors.Colors[0].ToArgb());
            Assert.Equal(1, pgb.InterpolationColors.Positions.Length);
            Assert.Equal(0f, pgb.InterpolationColors.Positions[0]);
            CheckDefaultRectangle(String.Empty, pgb.Rectangle);
            Assert.Equal(1, pgb.SurroundColors.Length);
            Assert.Equal(-1, pgb.SurroundColors[0].ToArgb());
            Assert.True(pgb.Transform.IsIdentity);
        }

        private void CheckPointsDefaults(PathGradientBrush pgb)
        {
            CheckDefaults(pgb);
            Assert.Equal(-16777216, pgb.CenterColor.ToArgb());
        }

        private void CheckPathDefaults(PathGradientBrush pgb)
        {
            CheckDefaults(pgb);
            Assert.Equal(-1, pgb.CenterColor.ToArgb());
        }

        public PathGradientBrushTest()
        {
            pts_2i = new Point[2] { new Point(1, 2), new Point(20, 30) };
            pts_2f = new PointF[2] { new PointF(1, 2), new PointF(20, 30) };
            empty_matrix = new Matrix();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_GraphicsPath_Null()
        {
            GraphicsPath gp = null;
            Assert.Throws<ArgumentNullException>(() => new PathGradientBrush(gp));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_GraphicsPath_Empty()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(gp));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_GraphicsPath_SinglePoint()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(new Point[1] { new Point(1, 1) });
                // Special case - a line with a single point is valid
                Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(gp));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Deskop & Core")]
        public void Constructor_GraphicsPath_Line()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(pts_2f);
                using (PathGradientBrush pgb = new PathGradientBrush(gp))
                {
                    CheckPathDefaults(pgb);
                    Assert.Equal(WrapMode.Clamp, pgb.WrapMode);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Null()
        {
            Point[] pts = null;
            Assert.Throws<ArgumentNullException>(() => new PathGradientBrush(pts));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Empty()
        {
            Point[] pts = new Point[0];
            Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(pts));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_One()
        {
            Point[] pts = new Point[1] { new Point(1, 1) };
            Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(pts));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_Point_Two()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2i))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.Clamp, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_Point_WrapMode_Clamp()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2i, WrapMode.Clamp))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.Clamp, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_Point_WrapMode_Tile()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2i, WrapMode.Tile))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.Tile, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & Core")]
        public void Constructor_Point_WrapMode_TileFlipX()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2i, WrapMode.TileFlipX))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.TileFlipX, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & Core")]
        public void Constructor_Point_WrapMode_TileFlipY()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2i, WrapMode.TileFlipY))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.TileFlipY, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconstent between Desktop & CoreFX")]
        public void Constructor_Point_WrapMode_TileFlipXY()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2i, WrapMode.TileFlipXY))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.TileFlipXY, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_PointF_Null()
        {
            PointF[] pts = null;
            Assert.Throws<ArgumentNullException>(() => new PathGradientBrush(pts));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_PointF_Empty()
        {
            PointF[] pts = new PointF[0];
            Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(pts));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_PointF_One()
        {
            PointF[] pts = new PointF[1] { new PointF(1, 1) };
            Assert.Throws<OutOfMemoryException>(() => new PathGradientBrush(pts));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_PointF_Two()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.Clamp, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void Constructor_PointF_WrapMode_Invalid()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => new PathGradientBrush(pts_2f, (WrapMode)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void Constructor_PointF_WrapMode_Clamp()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.Clamp, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_PointF_WrapMode_Tile()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Tile))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.Tile, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_PointF_WrapMode_TileFlipX()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipX))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.TileFlipX, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_PointF_WrapMode_TileFlipY()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipY))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.TileFlipY, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Inconsistent between Desktop & CoreFX")]
        public void Constructor_PointF_WrapMode_TileFlipXY()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                CheckPointsDefaults(pgb);
                Assert.Equal(WrapMode.TileFlipXY, pgb.WrapMode);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Blend()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                // change not accepted - but no exception is thrown
                pgb.Blend.Factors = new float[0];
                Assert.Equal(1, pgb.Blend.Factors.Length);
                pgb.Blend.Factors = new float[2];
                Assert.Equal(1, pgb.Blend.Factors.Length);

                // change not accepted - but no exception is thrown
                pgb.Blend.Positions = new float[0];
                Assert.Equal(1, pgb.Blend.Positions.Length);
                pgb.Blend.Positions = new float[2];
                Assert.Equal(1, pgb.Blend.Positions.Length);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FocusScales()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                PointF fs = new PointF(Single.MaxValue, Single.MinValue);
                pgb.FocusScales = fs;
                Assert.Equal(Single.MaxValue, pgb.FocusScales.X);
                Assert.Equal(Single.MinValue, pgb.FocusScales.Y);

                fs.X = Single.NaN;
                fs.Y = Single.NegativeInfinity;
                pgb.FocusScales = fs;
                Assert.Equal(Single.NaN, pgb.FocusScales.X);
                Assert.Equal(Single.NegativeInfinity, pgb.FocusScales.Y);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CenterColor()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                pgb.CenterColor = Color.Black;
                Assert.Equal(Color.Black.ToArgb(), pgb.CenterColor.ToArgb());
                pgb.CenterColor = Color.Transparent;
                Assert.Equal(Color.Transparent.ToArgb(), pgb.CenterColor.ToArgb());
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CenterPoint()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                PointF cp = new PointF(Single.MaxValue, Single.MinValue);
                pgb.CenterPoint = cp;
                Assert.Equal(Single.MaxValue, pgb.CenterPoint.X);
                Assert.Equal(Single.MinValue, pgb.CenterPoint.Y);

                cp.X = Single.NaN;
                cp.Y = Single.NegativeInfinity;
                pgb.CenterPoint = cp;
                Assert.Equal(Single.NaN, pgb.CenterPoint.X);
                Assert.Equal(Single.NegativeInfinity, pgb.CenterPoint.Y);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void InterpolationColors()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                // change not accepted - but no exception is thrown
                pgb.InterpolationColors.Colors = new Color[0];
                Assert.Equal(1, pgb.InterpolationColors.Colors.Length);
                pgb.InterpolationColors.Colors = new Color[2];
                Assert.Equal(1, pgb.InterpolationColors.Colors.Length);

                // change not accepted - but no exception is thrown
                pgb.InterpolationColors.Positions = new float[0];
                Assert.Equal(1, pgb.InterpolationColors.Positions.Length);
                pgb.InterpolationColors.Positions = new float[2];
                Assert.Equal(1, pgb.InterpolationColors.Positions.Length);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Rectangle()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                CheckDefaultRectangle("Original", pgb.Rectangle);
                pgb.MultiplyTransform(new Matrix(2, 0, 0, 2, 2, 2));
                CheckDefaultRectangle("Multiply", pgb.Rectangle);
                pgb.ResetTransform();
                CheckDefaultRectangle("Reset", pgb.Rectangle);
                pgb.RotateTransform(90);
                CheckDefaultRectangle("Rotate", pgb.Rectangle);
                pgb.ScaleTransform(4, 0.25f);
                CheckDefaultRectangle("Scale", pgb.Rectangle);
                pgb.TranslateTransform(-10, -20);
                CheckDefaultRectangle("Translate", pgb.Rectangle);

                pgb.SetBlendTriangularShape(0.5f);
                CheckDefaultRectangle("SetBlendTriangularShape", pgb.Rectangle);
                pgb.SetSigmaBellShape(0.5f);
                CheckDefaultRectangle("SetSigmaBellShape", pgb.Rectangle);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SurroundColors_Empty()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                Assert.Throws<ArgumentException>(() => pgb.SurroundColors = new Color[0]);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SurroundColors_2PointF()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.TileFlipXY))
            {
                // default values
                Assert.Equal(1, pgb.SurroundColors.Length);
                Assert.Equal(-1, pgb.SurroundColors[0].ToArgb());

                // default can't be changed
                pgb.SurroundColors[0] = Color.Gold;
                Assert.Equal(-1, pgb.SurroundColors[0].ToArgb());

                // 2 empty color isn't valid, change isn't accepted
                pgb.SurroundColors = new Color[2];
                Assert.Equal(1, pgb.SurroundColors.Length);

                pgb.SurroundColors = new Color[2] { Color.Black, Color.White };
                Assert.Equal(2, pgb.SurroundColors.Length);
                Assert.Equal(-16777216, pgb.SurroundColors[0].ToArgb());
                Assert.Equal(-1, pgb.SurroundColors[1].ToArgb());
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SurroundColors_3PointsF()
        {
            PointF[] points = new PointF[3] { new PointF(5, 50), new PointF(10, 100), new PointF(20, 75) };
            using (PathGradientBrush pgb = new PathGradientBrush(points))
            {
                // 3 empty color isn't valid, change isn't accepted
                pgb.SurroundColors = new Color[3] { Color.Empty, Color.Empty, Color.Empty };
                Assert.Equal(1, pgb.SurroundColors.Length);

                pgb.SurroundColors = new Color[3] { Color.Red, Color.Green, Color.Blue };
                // change not accepted - but no exception is thrown
                Assert.Equal(3, pgb.SurroundColors.Length);
                Assert.Equal(-65536, pgb.SurroundColors[0].ToArgb());
                Assert.Equal(-16744448, pgb.SurroundColors[1].ToArgb());
                Assert.Equal(-16776961, pgb.SurroundColors[2].ToArgb());
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new PathGradientBrush(pts_2f, WrapMode.Clamp).Transform = null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Empty()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                pgb.Transform = new Matrix();
                Assert.True(pgb.Transform.IsIdentity);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_NonInvertible()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.Transform = new Matrix(123, 24, 82, 16, 47, 30));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WrapMode_All()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                foreach (WrapMode wm in Enum.GetValues(typeof(WrapMode)))
                {
                    pgb.WrapMode = wm;
                    Assert.Equal(wm, pgb.WrapMode);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void WrapMode_Invalid()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.WrapMode = (WrapMode)Int32.MinValue);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void Clone()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(pts_2f);
                using (PathGradientBrush pgb = new PathGradientBrush(gp))
                {
                    using (PathGradientBrush clone = (PathGradientBrush)pgb.Clone())
                    {
                        CheckPathDefaults(clone);
                        Assert.Equal(WrapMode.Clamp, clone.WrapMode);
                    }
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform1_Null()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentNullException>(() => pgb.MultiplyTransform(null));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform2_Null()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentNullException>(() => pgb.MultiplyTransform(null, MatrixOrder.Append));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform2_Invalid()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                pgb.MultiplyTransform(empty_matrix, (MatrixOrder)Int32.MinValue);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void MultiplyTransform_NonInvertible()
        {
            using (Matrix noninvertible = new Matrix(123, 24, 82, 16, 47, 30))
            {
                using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
                {
                    Assert.Throws<ArgumentException>(() => pgb.MultiplyTransform(noninvertible));
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ResetTransform()
        {
            using (Matrix m = new Matrix(2, 0, 0, 2, 10, -10))
            {
                using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
                {
                    pgb.Transform = m;
                    Assert.False(pgb.Transform.IsIdentity);
                    pgb.ResetTransform();
                    Assert.True(pgb.Transform.IsIdentity);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateTransform()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                pgb.RotateTransform(90);
                float[] elements = pgb.Transform.Elements;
                Assert.Equal(0, elements[0], 1);
                Assert.Equal(1, elements[1], 1);
                Assert.Equal(-1, elements[2], 1);
                Assert.Equal(0, elements[3], 1);
                Assert.Equal(0, elements[4], 1);
                Assert.Equal(0, elements[5], 1);

                pgb.RotateTransform(270);
                Assert.True(pgb.Transform.IsIdentity);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateTransform_InvalidOrder()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.RotateTransform(720, (MatrixOrder)Int32.MinValue));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                pgb.ScaleTransform(2, 4);
                float[] elements = pgb.Transform.Elements;
                Assert.Equal(2, elements[0], 1);
                Assert.Equal(0, elements[1], 1);
                Assert.Equal(0, elements[2], 1);
                Assert.Equal(4, elements[3], 1);
                Assert.Equal(0, elements[4], 1);
                Assert.Equal(0, elements[5], 1);

                pgb.ScaleTransform(0.5f, 0.25f);
                Assert.True(pgb.Transform.IsIdentity);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform_MaxMin()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                pgb.ScaleTransform(Single.MaxValue, Single.MinValue);
                float[] elements = pgb.Transform.Elements;
                Assert.Equal(Single.MaxValue, elements[0]);
                Assert.Equal(0, elements[1], 1);
                Assert.Equal(0, elements[2], 1);
                Assert.Equal(Single.MinValue, elements[3]);
                Assert.Equal(0, elements[4], 1);
                Assert.Equal(0, elements[5], 1);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ScaleTransform_InvalidOrder()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.ScaleTransform(1, 1, (MatrixOrder)Int32.MinValue));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_Focus()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                // max valid
                pgb.SetBlendTriangularShape(1);
                Assert.True(pgb.Transform.IsIdentity);
                // min valid
                pgb.SetBlendTriangularShape(0);
                Assert.True(pgb.Transform.IsIdentity);
                // middle
                pgb.SetBlendTriangularShape(0.5f);
                Assert.True(pgb.Transform.IsIdentity);
                // no impact on matrix
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_Scale()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                // max valid
                pgb.SetBlendTriangularShape(0, 1);
                Assert.True(pgb.Transform.IsIdentity);
                // min valid
                pgb.SetBlendTriangularShape(1, 0);
                Assert.True(pgb.Transform.IsIdentity);
                // middle
                pgb.SetBlendTriangularShape(0.5f, 0.5f);
                Assert.True(pgb.Transform.IsIdentity);
                // no impact on matrix
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_FocusTooSmall()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetBlendTriangularShape(-1));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_FocusTooBig()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetBlendTriangularShape(1.01f));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_ScaleTooSmall()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetBlendTriangularShape(1, -1));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetBlendTriangularShape_ScaleTooBig()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetBlendTriangularShape(1, 1.01f));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_Focus()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                // max valid
                pgb.SetSigmaBellShape(1);
                Assert.True(pgb.Transform.IsIdentity);
                // min valid
                pgb.SetSigmaBellShape(0);
                Assert.True(pgb.Transform.IsIdentity);
                // middle
                pgb.SetSigmaBellShape(0.5f);
                Assert.True(pgb.Transform.IsIdentity);
                // no impact on matrix
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_Scale()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                // max valid
                pgb.SetSigmaBellShape(0, 1);
                Assert.True(pgb.Transform.IsIdentity);
                // min valid
                pgb.SetSigmaBellShape(1, 0);
                Assert.True(pgb.Transform.IsIdentity);
                // middle
                pgb.SetSigmaBellShape(0.5f, 0.5f);
                Assert.True(pgb.Transform.IsIdentity);
                // no impact on matrix
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_FocusTooSmall()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetSigmaBellShape(-1));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_FocusTooBig()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetSigmaBellShape(1.01f));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_ScaleTooSmall()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetSigmaBellShape(1, -1));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetSigmaBellShape_ScaleTooBig()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.SetSigmaBellShape(1, 1.01f));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TranslateTransform()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                pgb.TranslateTransform(1, 1);
                float[] elements = pgb.Transform.Elements;
                Assert.Equal(1, elements[0], 1);
                Assert.Equal(0, elements[1], 1);
                Assert.Equal(0, elements[2], 1);
                Assert.Equal(1, elements[3], 1);
                Assert.Equal(1, elements[4], 1);
                Assert.Equal(1, elements[5], 1);

                pgb.TranslateTransform(-1, -1);
                // strangely lgb.Transform.IsIdentity is false
                elements = pgb.Transform.Elements;
                Assert.Equal(1, elements[0], 1);
                Assert.Equal(0, elements[1], 1);
                Assert.Equal(0, elements[2], 1);
                Assert.Equal(1, elements[3], 1);
                Assert.Equal(0, elements[4], 1);
                Assert.Equal(0, elements[5], 1);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TranslateTransform_InvalidOrder()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Assert.Throws<ArgumentException>(() => pgb.TranslateTransform(1, 1, (MatrixOrder)Int32.MinValue));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Operations()
        {
            using (PathGradientBrush pgb = new PathGradientBrush(pts_2f, WrapMode.Clamp))
            {
                Matrix clone = pgb.Transform.Clone();
                Matrix mul = clone.Clone();

                clone.Multiply(mul, MatrixOrder.Append);
                pgb.MultiplyTransform(mul, MatrixOrder.Append);
                Assert.Equal(pgb.Transform, clone);

                clone.Multiply(mul, MatrixOrder.Prepend);
                pgb.MultiplyTransform(mul, MatrixOrder.Prepend);
                Assert.Equal(pgb.Transform, clone);

                clone.Rotate(45, MatrixOrder.Append);
                pgb.RotateTransform(45, MatrixOrder.Append);
                Assert.Equal(pgb.Transform, clone);

                clone.Rotate(45, MatrixOrder.Prepend);
                pgb.RotateTransform(45, MatrixOrder.Prepend);
                Assert.Equal(pgb.Transform, clone);

                clone.Scale(0.25f, 2, MatrixOrder.Append);
                pgb.ScaleTransform(0.25f, 2, MatrixOrder.Append);
                Assert.Equal(pgb.Transform, clone);

                clone.Scale(0.25f, 2, MatrixOrder.Prepend);
                pgb.ScaleTransform(0.25f, 2, MatrixOrder.Prepend);
                Assert.Equal(pgb.Transform, clone);

                clone.Translate(10, 20, MatrixOrder.Append);
                pgb.TranslateTransform(10, 20, MatrixOrder.Append);
                Assert.Equal(pgb.Transform, clone);

                clone.Translate(30, 40, MatrixOrder.Prepend);
                pgb.TranslateTransform(30, 40, MatrixOrder.Prepend);
                Assert.Equal(pgb.Transform, clone);

                clone.Reset();
                pgb.ResetTransform();
                Assert.Equal(pgb.Transform, clone);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Blend_Null()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(pts_2f);
                using (PathGradientBrush pgb = new PathGradientBrush(gp))
                {
                    Assert.Throws<NullReferenceException>(() => pgb.Blend = null);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void InterpolationColors_Null()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(pts_2f);
                using (PathGradientBrush pgb = new PathGradientBrush(gp))
                {
                    Assert.Throws<NullReferenceException>(() => pgb.InterpolationColors = null);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SurroundColors_Null()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(pts_2f);
                using (PathGradientBrush pgb = new PathGradientBrush(gp))
                {
                    Assert.Throws<NullReferenceException>(() => pgb.SurroundColors = null);
                }
            }
        }
    }
}
