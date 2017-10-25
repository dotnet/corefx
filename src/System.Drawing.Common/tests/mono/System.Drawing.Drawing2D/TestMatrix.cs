// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Tests for System.Drawing.Drawing2D.Matrix.cs
//
// Authors:
//	Jordi Mas i Hernandez <jordi@ximian.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
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

using Xunit;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;

namespace MonoTests.System.Drawing.Drawing2D
{
    public class MatrixTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Default()
        {
            Matrix matrix = new Matrix();
            Assert.Equal(6, matrix.Elements.Length);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_SixFloats()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            Assert.Equal(6, matrix.Elements.Length);
            Assert.Equal(10, matrix.Elements[0]);
            Assert.Equal(20, matrix.Elements[1]);
            Assert.Equal(30, matrix.Elements[2]);
            Assert.Equal(40, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Float()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            Assert.Equal(6, matrix.Elements.Length);
            Assert.Equal(10, matrix.Elements[0]);
            Assert.Equal(20, matrix.Elements[1]);
            Assert.Equal(30, matrix.Elements[2]);
            Assert.Equal(40, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Int_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix(default(Rectangle), null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Int_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix(default(Rectangle), new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Int_4Point()
        {
            Assert.Throws<ArgumentException>(() => new Matrix(default(Rectangle), new Point[4]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Rect_Point()
        {
            Rectangle r = new Rectangle(100, 200, 300, 400);
            Matrix m = new Matrix(r, new Point[3] { new Point(10, 20), new Point(30, 40), new Point(50, 60) });
            float[] elements = m.Elements;
            Assert.Equal(0.06666666, elements[0], 5);
            Assert.Equal(0.06666666, elements[1], 5);
            Assert.Equal(0.09999999, elements[2], 5);
            Assert.Equal(0.09999999, elements[3], 5);
            Assert.Equal(-16.6666679, elements[4], 5);
            Assert.Equal(-6.666667, elements[5], 5);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Float_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix(default(RectangleF), null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Float_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix(default(RectangleF), new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Float_2PointF()
        {
            Assert.Throws<ArgumentException>(() => new Matrix(default(RectangleF), new PointF[2]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_RectF_PointF()
        {
            RectangleF r = new RectangleF(100, 200, 300, 400);
            Matrix m = new Matrix(r, new PointF[3] { new PointF(10, 20), new PointF(30, 40), new PointF(50, 60) });
            float[] elements = m.Elements;
            Assert.Equal(0.06666666, elements[0], 5);
            Assert.Equal(0.06666666, elements[1], 5);
            Assert.Equal(0.09999999, elements[2], 5);
            Assert.Equal(0.09999999, elements[3], 5);
            Assert.Equal(-16.6666679, elements[4], 5);
            Assert.Equal(-6.666667, elements[5], 5);
        }

        // Properties

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Invertible()
        {
            Matrix matrix = new Matrix(123, 24, 82, 16, 47, 30);
            Assert.Equal(false, matrix.IsInvertible);

            matrix = new Matrix(156, 46, 0, 0, 106, 19);
            Assert.Equal(false, matrix.IsInvertible);

            matrix = new Matrix(146, 66, 158, 104, 42, 150);
            Assert.Equal(true, matrix.IsInvertible);

            matrix = new Matrix(119, 140, 145, 74, 102, 58);
            Assert.Equal(true, matrix.IsInvertible);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsIdentity()
        {
            Matrix identity = new Matrix();
            Matrix matrix = new Matrix(123, 24, 82, 16, 47, 30);
            Assert.Equal(false, matrix.IsIdentity);
            Assert.True(!identity.Equals(matrix));

            matrix = new Matrix(1, 0, 0, 1, 0, 0);
            Assert.Equal(true, matrix.IsIdentity);
            Assert.True(identity.Equals(matrix));

            // so what's the required precision ?

            matrix = new Matrix(1.1f, 0.1f, -0.1f, 0.9f, 0, 0);
            Assert.True(!matrix.IsIdentity);
            Assert.True(!identity.Equals(matrix));

            matrix = new Matrix(1.01f, 0.01f, -0.01f, 0.99f, 0, 0);
            Assert.True(!matrix.IsIdentity);
            Assert.True(!identity.Equals(matrix));

            matrix = new Matrix(1.001f, 0.001f, -0.001f, 0.999f, 0, 0);
            Assert.True(!matrix.IsIdentity);
            Assert.True(!identity.Equals(matrix));

            matrix = new Matrix(1.0001f, 0.0001f, -0.0001f, 0.9999f, 0, 0);
            Assert.True(matrix.IsIdentity);
            // note: NOT equal
            Assert.True(!identity.Equals(matrix));

            matrix = new Matrix(1.0009f, 0.0009f, -0.0009f, 0.99995f, 0, 0);
            Assert.True(!matrix.IsIdentity);
            Assert.True(!identity.Equals(matrix));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOffsetX()
        {
            Matrix matrix = new Matrix(123, 24, 82, 16, 47, 30);
            Assert.Equal(47, matrix.OffsetX);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOffsetY()
        {
            Matrix matrix = new Matrix(123, 24, 82, 16, 47, 30);
            Assert.Equal(30, matrix.OffsetY);
        }

        // Elements Property is checked implicity in other test

        //
        // Methods
        //


        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Clone()
        {
            Matrix matsrc = new Matrix(10, 20, 30, 40, 50, 60);
            Matrix matrix = matsrc.Clone();

            Assert.Equal(6, matrix.Elements.Length);
            Assert.Equal(10, matrix.Elements[0]);
            Assert.Equal(20, matrix.Elements[1]);
            Assert.Equal(30, matrix.Elements[2]);
            Assert.Equal(40, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void HashCode()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            Matrix clone = matrix.Clone();
            Assert.True(matrix.GetHashCode() != clone.GetHashCode());

            Matrix matrix2 = new Matrix(10, 20, 30, 40, 50, 60);
            Assert.True(matrix.GetHashCode() != matrix2.GetHashCode());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reset()
        {
            Matrix matrix = new Matrix(51, 52, 53, 54, 55, 56);
            matrix.Reset();

            Assert.Equal(6, matrix.Elements.Length);
            Assert.Equal(1, matrix.Elements[0]);
            Assert.Equal(0, matrix.Elements[1]);
            Assert.Equal(0, matrix.Elements[2]);
            Assert.Equal(1, matrix.Elements[3]);
            Assert.Equal(0, matrix.Elements[4]);
            Assert.Equal(0, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Rotate()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Rotate(180);

            Assert.Equal(-10.0f, matrix.Elements[0], 4);
            Assert.Equal(-20, matrix.Elements[1], 4);
            Assert.Equal(-30.0000019f, matrix.Elements[2], 4);
            Assert.Equal(-40.0000038f, matrix.Elements[3], 4);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Rotate_45_135()
        {
            Matrix matrix = new Matrix();
            Assert.True(matrix.IsIdentity);

            matrix.Rotate(45);
            Assert.True(!matrix.IsIdentity);
            float[] elements = matrix.Elements;
            Assert.Equal(0.707106769f, elements[0], 4);
            Assert.Equal(0.707106769f, elements[1], 4);
            Assert.Equal(-0.707106829f, elements[2], 4);
            Assert.Equal(0.707106769f, elements[3], 4);
            Assert.Equal(0, elements[4], 3);
            Assert.Equal(0, elements[5], 3);

            matrix.Rotate(135);
            Assert.True(!matrix.IsIdentity);
            elements = matrix.Elements;
            Assert.Equal(-1, elements[0], 4);
            Assert.Equal(0, elements[1], 4);
            Assert.Equal(0, elements[2], 4);
            Assert.Equal(-1, elements[3], 4);
            Assert.Equal(0, elements[4]);
            Assert.Equal(0, elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Rotate_90_270_Matrix()
        {
            Matrix matrix = new Matrix();
            Assert.True(matrix.IsIdentity);

            matrix.Rotate(90);
            Assert.True(!matrix.IsIdentity);
            float[] elements = matrix.Elements;
            Assert.Equal(0, elements[0], 4);
            Assert.Equal(1, elements[1], 4);
            Assert.Equal(-1, elements[2], 4);
            Assert.Equal(0, elements[3], 4);
            Assert.Equal(0, elements[4]);
            Assert.Equal(0, elements[5]);

            matrix.Rotate(270);
            // this isn't a perfect 1, 0, 0, 1, 0, 0 matrix - but close enough
            Assert.True(matrix.IsIdentity);
            Assert.True(!new Matrix().Equals(matrix));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Rotate_InvalidOrder()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().Rotate(180, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateAt()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.RotateAt(180, new PointF(10, 10));

            Assert.Equal(-10, matrix.Elements[0], 2);
            Assert.Equal(-20, matrix.Elements[1], 2);
            Assert.Equal(-30, matrix.Elements[2], 2);
            Assert.Equal(-40, matrix.Elements[3], 2);
            Assert.Equal(850, matrix.Elements[4], 2);
            Assert.Equal(1260, matrix.Elements[5], 2);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void RotateAt_InvalidOrder()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().RotateAt(180, new PointF(10, 10), (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Multiply_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix(10, 20, 30, 40, 50, 60).Multiply(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Multiply()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Multiply(new Matrix(10, 20, 30, 40, 50, 60));

            Assert.Equal(700, matrix.Elements[0]);
            Assert.Equal(1000, matrix.Elements[1]);
            Assert.Equal(1500, matrix.Elements[2]);
            Assert.Equal(2200, matrix.Elements[3]);
            Assert.Equal(2350, matrix.Elements[4]);
            Assert.Equal(3460, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Multiply_Null_Order()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix(10, 20, 30, 40, 50, 60).Multiply(null, MatrixOrder.Append));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Multiply_Append()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Multiply(new Matrix(10, 20, 30, 40, 50, 60), MatrixOrder.Append);

            Assert.Equal(700, matrix.Elements[0]);
            Assert.Equal(1000, matrix.Elements[1]);
            Assert.Equal(1500, matrix.Elements[2]);
            Assert.Equal(2200, matrix.Elements[3]);
            Assert.Equal(2350, matrix.Elements[4]);
            Assert.Equal(3460, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Multiply_Prepend()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Multiply(new Matrix(10, 20, 30, 40, 50, 60), MatrixOrder.Prepend);

            Assert.Equal(700, matrix.Elements[0]);
            Assert.Equal(1000, matrix.Elements[1]);
            Assert.Equal(1500, matrix.Elements[2]);
            Assert.Equal(2200, matrix.Elements[3]);
            Assert.Equal(2350, matrix.Elements[4]);
            Assert.Equal(3460, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Multiply_InvalidOrder()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            Assert.Throws<ArgumentException>(() => matrix.Multiply(new Matrix(10, 20, 30, 40, 50, 60), (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Equals()
        {
            Matrix mat1 = new Matrix(10, 20, 30, 40, 50, 60);
            Matrix mat2 = new Matrix(10, 20, 30, 40, 50, 60);
            Matrix mat3 = new Matrix(10, 20, 30, 40, 50, 10);

            Assert.Equal(true, mat1.Equals(mat2));
            Assert.Equal(false, mat2.Equals(mat3));
            Assert.Equal(false, mat1.Equals(mat3));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Invert()
        {
            Matrix matrix = new Matrix(1, 2, 3, 4, 5, 6);
            matrix.Invert();

            Assert.Equal(-2, matrix.Elements[0]);
            Assert.Equal(1, matrix.Elements[1]);
            Assert.Equal(1.5, matrix.Elements[2]);
            Assert.Equal(-0.5, matrix.Elements[3]);
            Assert.Equal(1, matrix.Elements[4]);
            Assert.Equal(-2, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Invert_Translation()
        {
            Matrix matrix = new Matrix(1, 0, 0, 1, 8, 8);
            matrix.Invert();

            float[] elements = matrix.Elements;
            Assert.Equal(1, elements[0]);
            Assert.Equal(0, elements[1]);
            Assert.Equal(0, elements[2]);
            Assert.Equal(1, elements[3]);
            Assert.Equal(-8, elements[4]);
            Assert.Equal(-8, elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Invert_Identity()
        {
            Matrix matrix = new Matrix();
            Assert.True(matrix.IsIdentity);
            Assert.True(matrix.IsInvertible);
            matrix.Invert();
            Assert.True(matrix.IsIdentity);
            Assert.True(matrix.IsInvertible);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Scale()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Scale(2, 4);

            Assert.Equal(20, matrix.Elements[0]);
            Assert.Equal(40, matrix.Elements[1]);
            Assert.Equal(120, matrix.Elements[2]);
            Assert.Equal(160, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);

            matrix.Scale(0.5f, 0.25f);

            Assert.Equal(10, matrix.Elements[0]);
            Assert.Equal(20, matrix.Elements[1]);
            Assert.Equal(30, matrix.Elements[2]);
            Assert.Equal(40, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Scale_Negative()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Scale(-2, -4);

            Assert.Equal(-20, matrix.Elements[0]);
            Assert.Equal(-40, matrix.Elements[1]);
            Assert.Equal(-120, matrix.Elements[2]);
            Assert.Equal(-160, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Scale_InvalidOrder()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().Scale(2, 1, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Shear()
        {
            Matrix matrix = new Matrix(10, 20, 30, 40, 50, 60);
            matrix.Shear(2, 4);

            Assert.Equal(130, matrix.Elements[0]);
            Assert.Equal(180, matrix.Elements[1]);
            Assert.Equal(50, matrix.Elements[2]);
            Assert.Equal(80, matrix.Elements[3]);
            Assert.Equal(50, matrix.Elements[4]);
            Assert.Equal(60, matrix.Elements[5]);

            matrix = new Matrix(5, 3, 9, 2, 2, 1);
            matrix.Shear(10, 20);

            Assert.Equal(185, matrix.Elements[0]);
            Assert.Equal(43, matrix.Elements[1]);
            Assert.Equal(59, matrix.Elements[2]);
            Assert.Equal(32, matrix.Elements[3]);
            Assert.Equal(2, matrix.Elements[4]);
            Assert.Equal(1, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Shear_InvalidOrder()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().Shear(-1, 1, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformPoints()
        {
            Matrix matrix = new Matrix(2, 4, 6, 8, 10, 12);
            PointF[] pointsF = new PointF[] { new PointF(2, 4), new PointF(4, 8) };
            matrix.TransformPoints(pointsF);

            Assert.Equal(38, pointsF[0].X);
            Assert.Equal(52, pointsF[0].Y);
            Assert.Equal(66, pointsF[1].X);
            Assert.Equal(92, pointsF[1].Y);

            Point[] points = new Point[] { new Point(2, 4), new Point(4, 8) };
            matrix.TransformPoints(points);
            Assert.Equal(38, pointsF[0].X);
            Assert.Equal(52, pointsF[0].Y);
            Assert.Equal(66, pointsF[1].X);
            Assert.Equal(92, pointsF[1].Y);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformPoints_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix().TransformPoints((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformPoints_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix().TransformPoints((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformPoints_Point_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().TransformPoints(new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformPoints_PointF_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().TransformPoints(new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformVectors()
        {
            Matrix matrix = new Matrix(2, 4, 6, 8, 10, 12);
            PointF[] pointsF = new PointF[] { new PointF(2, 4), new PointF(4, 8) };
            matrix.TransformVectors(pointsF);

            Assert.Equal(28, pointsF[0].X);
            Assert.Equal(40, pointsF[0].Y);
            Assert.Equal(56, pointsF[1].X);
            Assert.Equal(80, pointsF[1].Y);

            Point[] points = new Point[] { new Point(2, 4), new Point(4, 8) };
            matrix.TransformVectors(points);
            Assert.Equal(28, pointsF[0].X);
            Assert.Equal(40, pointsF[0].Y);
            Assert.Equal(56, pointsF[1].X);
            Assert.Equal(80, pointsF[1].Y);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformVectors_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix().TransformVectors((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformVectors_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix().TransformVectors((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformVectors_Point_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().TransformVectors(new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TransformVectors_PointF_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().TransformVectors(new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Translate()
        {
            Matrix matrix = new Matrix(2, 4, 6, 8, 10, 12);
            matrix.Translate(5, 10);

            Assert.Equal(2, matrix.Elements[0]);
            Assert.Equal(4, matrix.Elements[1]);
            Assert.Equal(6, matrix.Elements[2]);
            Assert.Equal(8, matrix.Elements[3]);
            Assert.Equal(80, matrix.Elements[4]);
            Assert.Equal(112, matrix.Elements[5]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Translate_InvalidOrder()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().Translate(-1, 1, (MatrixOrder)Int32.MinValue));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void VectorTransformPoints_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Matrix().VectorTransformPoints((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void VectorTransformPoints_Empty()
        {
            Assert.Throws<ArgumentException>(() => new Matrix().VectorTransformPoints(new Point[0]));
        }
    }
}
