// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.GraphicsPath unit tests
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
using SC = System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Drawing2D
{

    public class GraphicsPathTest
    {

        private const float Pi4 = (float)(Math.PI / 4);
        // let's tolerate a few differences
        private const int Precision = 3;
        private const int LowPrecision = 1;

        private void CheckEmpty(string prefix, GraphicsPath gp)
        {
            Assert.Equal(0, gp.PathData.Points.Length);
            Assert.Equal(0, gp.PathData.Types.Length);
            Assert.Equal(0, gp.PointCount);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_InvalidFillMode()
        {
            GraphicsPath gp = new GraphicsPath((FillMode)Int32.MinValue);
            Assert.Equal(Int32.MinValue, (int)gp.FillMode);
            CheckEmpty("InvalidFillMode.", gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Null_Byte()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath((Point[])null, new byte[1]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Byte_Null()
        {
            Assert.Throws<NullReferenceException>(() => new GraphicsPath(new Point[1], null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Point_Byte_LengthMismatch()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath(new Point[1], new byte[2]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_PointF_Null_Byte()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath((PointF[])null, new byte[1]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_PointF_Byte_Null()
        {
            Assert.Throws<NullReferenceException>(() => new GraphicsPath(new PointF[1], null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_PointF_Byte_LengthMismatch()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath(new PointF[2], new byte[1]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GraphicsPath_Empty()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Equal(FillMode.Alternate, gp.FillMode);
            CheckEmpty("Empty.", gp);

            GraphicsPath clone = (GraphicsPath)gp.Clone();
            Assert.Equal(FillMode.Alternate, gp.FillMode);
            CheckEmpty("Clone.", gp);

            gp.Reverse();
            CheckEmpty("Reverse.", gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GraphicsPath_Empty_PathPoints()
        {
            Assert.Throws<ArgumentException>(() => Assert.Null(new GraphicsPath().PathPoints));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GraphicsPath_Empty_PathTypes()
        {
            Assert.Throws<ArgumentException>(() => Assert.Null(new GraphicsPath().PathTypes));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GraphicsPath_SamePoint()
        {
            Point[] points = new Point[] {
                new Point (1, 1),
                new Point (1, 1),
                new Point (1, 1),
                new Point (1, 1),
                new Point (1, 1),
                new Point (1, 1),
            };
            byte[] types = new byte[6] { 0, 1, 1, 1, 1, 1 };
            using (GraphicsPath gp = new GraphicsPath(points, types))
            {
                Assert.Equal(6, gp.PointCount);
            }
            types[0] = 1;
            using (GraphicsPath gp = new GraphicsPath(points, types))
            {
                Assert.Equal(6, gp.PointCount);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GraphicsPath_SamePointF()
        {
            PointF[] points = new PointF[] {
                new PointF (1f, 1f),
                new PointF (1f, 1f),
                new PointF (1f, 1f),
                new PointF (1f, 1f),
                new PointF (1f, 1f),
                new PointF (1f, 1f),
            };
            byte[] types = new byte[6] { 0, 1, 1, 1, 1, 1 };
            using (GraphicsPath gp = new GraphicsPath(points, types))
            {
                Assert.Equal(6, gp.PointCount);
            }
            types[0] = 1;
            using (GraphicsPath gp = new GraphicsPath(points, types))
            {
                Assert.Equal(6, gp.PointCount);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable, Skip = "Internal ArgumentException in System.Drawing")]
        public void FillMode_Invalid()
        {
            // constructor accept an invalid FillMode
            GraphicsPath gp = new GraphicsPath((FillMode)Int32.MaxValue);
            Assert.Equal(Int32.MaxValue, (int)gp.FillMode);
            // but you can't set the FillMode property to an invalid value );-)
            Assert.Throws<SC.InvalidEnumArgumentException>(() => gp.FillMode = (FillMode)Int32.MaxValue);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PathData_CannotChange()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));

            Assert.Equal(1f, gp.PathData.Points[0].X);
            Assert.Equal(1f, gp.PathData.Points[0].Y);

            // now try to change the first point
            gp.PathData.Points[0] = new Point(0, 0);
            // the changes isn't reflected in the property
            Assert.Equal(1f, gp.PathData.Points[0].X);
            Assert.Equal(1f, gp.PathData.Points[0].Y);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PathPoints_CannotChange()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));

            Assert.Equal(1f, gp.PathPoints[0].X);
            Assert.Equal(1f, gp.PathPoints[0].Y);

            // now try to change the first point
            gp.PathPoints[0] = new Point(0, 0);
            // the changes isn't reflected in the property
            Assert.Equal(1f, gp.PathPoints[0].X);
            Assert.Equal(1f, gp.PathPoints[0].Y);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PathTypes_CannotChange()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));

            Assert.Equal(0, gp.PathTypes[0]);

            // now try to change the first type
            gp.PathTypes[0] = 1;
            // the changes isn't reflected in the property
            Assert.Equal(0, gp.PathTypes[0]);
        }

        private void CheckArc(GraphicsPath path)
        {
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(2.99962401f, rect.X, Precision);
            Assert.Equal(2.01370716f, rect.Y, Precision);
            Assert.Equal(0f, rect.Width, Precision);
            Assert.Equal(0.0137047768f, rect.Height);

            Assert.Equal(2.99990582f, path.PathData.Points[0].X, Precision);
            Assert.Equal(2.01370716f, path.PathPoints[0].Y, Precision);
            Assert.Equal(0, path.PathData.Types[0]);
            Assert.Equal(2.99984312f, path.PathData.Points[1].X, Precision);
            Assert.Equal(2.018276f, path.PathPoints[1].Y, Precision);
            Assert.Equal(3, path.PathTypes[1]);
            Assert.Equal(2.99974918f, path.PathData.Points[2].X, Precision);
            Assert.Equal(2.02284455f, path.PathPoints[2].Y, Precision);
            Assert.Equal(3, path.PathData.Types[2]);
            Assert.Equal(2.999624f, path.PathData.Points[3].X, Precision);
            Assert.Equal(2.027412f, path.PathPoints[3].Y, Precision);
            Assert.Equal(3, path.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddArc_Rectangle()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(new Rectangle(1, 1, 2, 2), Pi4, Pi4);
            CheckArc(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddArc_RectangleF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(new RectangleF(1f, 1f, 2f, 2f), Pi4, Pi4);
            CheckArc(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddArc_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(1, 1, 2, 2, Pi4, Pi4);
            CheckArc(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddArc_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
            CheckArc(gp);
        }

        private void CheckBezier(GraphicsPath path)
        {
            Assert.Equal(4, path.PointCount);
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(1f, rect.X);
            Assert.Equal(1f, rect.Y);
            Assert.Equal(3f, rect.Width);
            Assert.Equal(3f, rect.Height);

            Assert.Equal(1f, path.PathData.Points[0].X);
            Assert.Equal(1f, path.PathPoints[0].Y);
            Assert.Equal(0, path.PathData.Types[0]);
            Assert.Equal(2f, path.PathData.Points[1].X);
            Assert.Equal(2f, path.PathPoints[1].Y);
            Assert.Equal(3, path.PathTypes[1]);
            Assert.Equal(3f, path.PathData.Points[2].X);
            Assert.Equal(3f, path.PathPoints[2].Y);
            Assert.Equal(3, path.PathData.Types[2]);
            Assert.Equal(4f, path.PathData.Points[3].X);
            Assert.Equal(4f, path.PathPoints[3].Y);
            Assert.Equal(3, path.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBezier_Point()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4));
            CheckBezier(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBezier_PointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(new PointF(1f, 1f), new PointF(2f, 2f), new PointF(3f, 3f), new PointF(4f, 4f));
            CheckBezier(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBezier_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(1, 1, 2, 2, 3, 3, 4, 4);
            CheckBezier(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBezier_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(1f, 1f, 2f, 2f, 3f, 3f, 4f, 4f);
            CheckBezier(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBezier_SamePoint()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(1, 1, 1, 1, 1, 1, 1, 1);
            // all points are present
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(3, gp.PathTypes[1]);
            Assert.Equal(3, gp.PathTypes[2]);
            Assert.Equal(3, gp.PathTypes[3]);

            gp.AddBezier(new Point(1, 1), new Point(1, 1), new Point(1, 1), new Point(1, 1));
            // the first point (move to) can be compressed (i.e. removed)
            Assert.Equal(7, gp.PointCount);
            Assert.Equal(3, gp.PathTypes[4]);
            Assert.Equal(3, gp.PathTypes[5]);
            Assert.Equal(3, gp.PathTypes[6]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBezier_SamePointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBezier(new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f));
            // all points are present
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(3, gp.PathTypes[1]);
            Assert.Equal(3, gp.PathTypes[2]);
            Assert.Equal(3, gp.PathTypes[3]);

            gp.AddBezier(new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f));
            // the first point (move to) can be compressed (i.e. removed)
            Assert.Equal(7, gp.PointCount);
            Assert.Equal(3, gp.PathTypes[4]);
            Assert.Equal(3, gp.PathTypes[5]);
            Assert.Equal(3, gp.PathTypes[6]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddBeziers((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_3_Points()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddBeziers(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_Point()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBeziers(new Point[4] { new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4) });
            CheckBezier(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddBeziers((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_3_PointFs()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddBeziers(new PointF[3] { new PointF(1f, 1f), new PointF(2f, 2f), new PointF(3f, 3f) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_PointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddBeziers(new PointF[4] { new PointF(1f, 1f), new PointF(2f, 2f), new PointF(3f, 3f), new PointF(4f, 4f) });
            CheckBezier(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_SamePoint()
        {
            Point[] points = new Point[4] { new Point(1, 1), new Point(1, 1), new Point(1, 1), new Point(1, 1) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddBeziers(points);
            // all points are present
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(3, gp.PathTypes[1]);
            Assert.Equal(3, gp.PathTypes[2]);
            Assert.Equal(3, gp.PathTypes[3]);

            gp.AddBeziers(points);
            // the first point (move to) can be compressed (i.e. removed)
            Assert.Equal(7, gp.PointCount);
            Assert.Equal(3, gp.PathTypes[4]);
            Assert.Equal(3, gp.PathTypes[5]);
            Assert.Equal(3, gp.PathTypes[6]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddBeziers_SamePointF()
        {
            PointF[] points = new PointF[4] { new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddBeziers(points);
            // all points are present
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(3, gp.PathTypes[1]);
            Assert.Equal(3, gp.PathTypes[2]);
            Assert.Equal(3, gp.PathTypes[3]);

            gp.AddBeziers(points);
            // the first point (move to) can be compressed (i.e. removed)
            Assert.Equal(7, gp.PointCount);
            Assert.Equal(3, gp.PathTypes[4]);
            Assert.Equal(3, gp.PathTypes[5]);
            Assert.Equal(3, gp.PathTypes[6]);
        }

        private void CheckEllipse(GraphicsPath path)
        {
            Assert.Equal(13, path.PathPoints.Length);
            Assert.Equal(13, path.PathTypes.Length);
            Assert.Equal(13, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(1f, rect.X);
            Assert.Equal(1f, rect.Y);
            Assert.Equal(2f, rect.Width);
            Assert.Equal(2f, rect.Height);

            Assert.Equal(0, path.PathData.Types[0]);
            for (int i = 1; i < 12; i++)
                Assert.Equal(3, path.PathTypes[i]);
            Assert.Equal(131, path.PathData.Types[12]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddEllipse_Rectangle()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(new Rectangle(1, 1, 2, 2));
            CheckEllipse(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddEllipse_RectangleF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(new RectangleF(1f, 1f, 2f, 2f));
            CheckEllipse(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddEllipse_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(1, 1, 2, 2);
            CheckEllipse(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddEllipse_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(1f, 1f, 2f, 2f);
            CheckEllipse(gp);
        }

        private void CheckLine(GraphicsPath path)
        {
            Assert.Equal(2, path.PathPoints.Length);
            Assert.Equal(2, path.PathTypes.Length);
            Assert.Equal(2, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(1f, rect.X);
            Assert.Equal(1f, rect.Y);
            Assert.Equal(1f, rect.Width);
            Assert.Equal(1f, rect.Height);

            Assert.Equal(1f, path.PathData.Points[0].X);
            Assert.Equal(1f, path.PathPoints[0].Y);
            Assert.Equal(0, path.PathData.Types[0]);
            Assert.Equal(2f, path.PathData.Points[1].X);
            Assert.Equal(2f, path.PathPoints[1].Y);
            Assert.Equal(1, path.PathTypes[1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_Point()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(1, 1), new Point(2, 2));
            CheckLine(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_PointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new PointF(1f, 1f), new PointF(2f, 2f));
            CheckLine(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(1, 1, 2, 2);
            CheckLine(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(1f, 1f, 2f, 2f);
            CheckLine(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_SamePoint()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new Point(1, 1), new Point(1, 1));
            Assert.Equal(2, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);

            gp.AddLine(new Point(1, 1), new Point(1, 1));
            // 3 not 4 points, the first point (only) is compressed
            Assert.Equal(3, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);

            gp.AddLine(new Point(1, 1), new Point(1, 1));
            // 4 not 5 (or 6) points, the first point (only) is compressed
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(1, gp.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_SamePointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new PointF(49.2f, 157f), new PointF(49.2f, 157f));
            Assert.Equal(2, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);

            gp.AddLine(new PointF(49.2f, 157f), new PointF(49.2f, 157f));
            // 3 not 4 points, the first point (only) is compressed
            Assert.Equal(3, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLine_SamePointsF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(new PointF(49.2f, 157f), new PointF(75.6f, 196f));
            gp.AddLine(new PointF(75.6f, 196f), new PointF(102f, 209f));
            Assert.Equal(3, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);

            gp.AddLine(new PointF(102f, 209f), new PointF(75.6f, 196f));
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(1, gp.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddLines((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_Point_0()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddLines(new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_Point_1()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(new Point[1] { new Point(1, 1) });
            // Special case - a line with a single point is valid
            Assert.Equal(1, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_Point()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(new Point[2] { new Point(1, 1), new Point(2, 2) });
            CheckLine(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddLines((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_PointF_0()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddLines(new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_PointF_1()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(new PointF[1] { new PointF(1f, 1f) });
            // Special case - a line with a single point is valid
            Assert.Equal(1, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_PointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) });
            CheckLine(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_SamePoint()
        {
            Point[] points = new Point[] { new Point(1, 1), new Point(1, 1) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points);
            Assert.Equal(2, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);

            gp.AddLines(points);
            // 3 not 4 points, the first point (only) is compressed
            Assert.Equal(3, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);

            gp.AddLines(points);
            // 4 not 5 (or 6) points, the first point (only) is compressed
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(1, gp.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddLines_SamePointF()
        {
            PointF[] points = new PointF[] { new PointF(49.2f, 157f), new PointF(49.2f, 157f), new PointF(49.2f, 157f), new PointF(49.2f, 157f) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points);
            // all identical points are added
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(1, gp.PathTypes[3]);

            gp.AddLines(points);
            // only the first new point is compressed
            Assert.Equal(7, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(1, gp.PathTypes[3]);
            Assert.Equal(1, gp.PathTypes[4]);
            Assert.Equal(1, gp.PathTypes[5]);
            Assert.Equal(1, gp.PathTypes[6]);
        }

        private void CheckPie(GraphicsPath path)
        {
            // the number of points generated for a Pie isn't the same between Mono and MS
#if false
			Assert.Equal (5, path.PathPoints.Length);
			Assert.Equal (5, path.PathTypes.Length);
			Assert.Equal (5, path.PathData.Points.Length);

			// GetBounds (well GdipGetPathWorldBounds) isn't implemented
			RectangleF rect = path.GetBounds ();
			Assert.Equal (2f, rect.X);
			Assert.Equal (2f, rect.Y);
			Assert.Equal (0.9999058f, rect.Width);
			Assert.Equal (0.0274119377f, rect.Height);

			Assert.Equal (2f, path.PathData.Points[0].X);
			Assert.Equal (2f, path.PathPoints[0].Y);
			Assert.Equal (0, path.PathData.Types[0]);
			Assert.Equal (2.99990582f, path.PathData.Points[1].X);
			Assert.Equal (2.01370716f, path.PathPoints[1].Y);
			Assert.Equal (1, path.PathTypes[1]);
			Assert.Equal (2.99984312f, path.PathData.Points[2].X);
			Assert.Equal (2.018276f, path.PathPoints[2].Y);
			Assert.Equal (3, path.PathData.Types[2]);
			Assert.Equal (2.99974918f, path.PathData.Points[3].X);
			Assert.Equal (2.02284455f, path.PathPoints[3].Y);
			Assert.Equal (3, path.PathData.Types[3]);
			Assert.Equal (2.999624f, path.PathData.Points[4].X);
			Assert.Equal (2.027412f, path.PathPoints[4].Y);
			Assert.Equal (131, path.PathTypes[4]);
#endif
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPie_Rect()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(new Rectangle(1, 1, 2, 2), Pi4, Pi4);
            CheckPie(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPie_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(1, 1, 2, 2, Pi4, Pi4);
            CheckPie(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPie_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(1f, 1f, 2f, 2f, Pi4, Pi4);
            CheckPie(gp);
        }

        private void CheckPolygon(GraphicsPath path)
        {
            // an extra point is generated by Mono (libgdiplus)
#if false
			Assert.Equal (3, path.PathPoints.Length);
			Assert.Equal (3, path.PathTypes.Length);
			Assert.Equal (3, path.PathData.Points.Length);
#endif
            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(1f, rect.X);
            Assert.Equal(1f, rect.Y);
            Assert.Equal(2f, rect.Width);
            Assert.Equal(2f, rect.Height);

            Assert.Equal(1f, path.PathData.Points[0].X);
            Assert.Equal(1f, path.PathPoints[0].Y);
            Assert.Equal(0, path.PathData.Types[0]);
            Assert.Equal(2f, path.PathData.Points[1].X);
            Assert.Equal(2f, path.PathPoints[1].Y);
            Assert.Equal(1, path.PathTypes[1]);
            Assert.Equal(3f, path.PathData.Points[2].X);
            Assert.Equal(3f, path.PathPoints[2].Y);
            // the extra point change the type of the last point
#if false
			Assert.Equal (129, path.PathData.Types[2]);
#endif
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddPolygon((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_Point_Empty()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_Point_1()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddPolygon(new Point[1] { new Point(1, 1) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_Point_2()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddPolygon(new Point[2] { new Point(1, 1), new Point(2, 2) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_Point_3()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
            CheckPolygon(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddPolygon((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_PointF_Empty()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_PointF_1()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddPolygon(new PointF[1] { new PointF(1f, 1f) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_PointF_2()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddPolygon(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_PointF_3()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new PointF[3] { new PointF(1f, 1f), new PointF(2f, 2f), new PointF(3f, 3f) });
            CheckPolygon(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_SamePoint()
        {
            Point[] points = new Point[3] { new Point(1, 1), new Point(1, 1), new Point(1, 1) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(points);
            // all identical points are added
            Assert.Equal(3, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(129, gp.PathTypes[2]);

            gp.AddPolygon(points);
            // all identical points are added (again)
            Assert.Equal(6, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[3]);
            Assert.Equal(1, gp.PathTypes[4]);
            Assert.Equal(129, gp.PathTypes[5]);

            gp.AddLines(points);
            // all identical points are added as a line (because previous point is closed)
            Assert.Equal(9, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[6]);
            Assert.Equal(1, gp.PathTypes[7]);
            Assert.Equal(1, gp.PathTypes[8]);

            gp.AddPolygon(points);
            // all identical points are added (again)
            Assert.Equal(12, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[9]);
            Assert.Equal(1, gp.PathTypes[10]);
            Assert.Equal(129, gp.PathTypes[11]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPolygon_SamePointF()
        {
            PointF[] points = new PointF[3] { new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(points);
            // all identical points are added
            Assert.Equal(3, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(129, gp.PathTypes[2]);

            gp.AddPolygon(points);
            // all identical points are added (again)
            Assert.Equal(6, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[3]);
            Assert.Equal(1, gp.PathTypes[4]);
            Assert.Equal(129, gp.PathTypes[5]);

            gp.AddLines(points);
            // all identical points are added as a line (because previous point is closed)
            Assert.Equal(9, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[6]);
            Assert.Equal(1, gp.PathTypes[7]);
            Assert.Equal(1, gp.PathTypes[8]);

            gp.AddPolygon(points);
            // all identical points are added (again)
            Assert.Equal(12, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[9]);
            Assert.Equal(1, gp.PathTypes[10]);
            Assert.Equal(129, gp.PathTypes[11]);
        }

        private void CheckRectangle(GraphicsPath path, int count)
        {
            Assert.Equal(count, path.PathPoints.Length);
            Assert.Equal(count, path.PathTypes.Length);
            Assert.Equal(count, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(1f, rect.X);
            Assert.Equal(1f, rect.Y);
            Assert.Equal(2f, rect.Width);
            Assert.Equal(2f, rect.Height);

            // check first four points (first rectangle)
            Assert.Equal(1f, path.PathData.Points[0].X);
            Assert.Equal(1f, path.PathPoints[0].Y);
            Assert.Equal(0, path.PathData.Types[0]);
            Assert.Equal(3f, path.PathData.Points[1].X);
            Assert.Equal(1f, path.PathPoints[1].Y);
            Assert.Equal(1, path.PathTypes[1]);
            Assert.Equal(3f, path.PathData.Points[2].X);
            Assert.Equal(3f, path.PathPoints[2].Y);
            Assert.Equal(1, path.PathData.Types[2]);
            Assert.Equal(1f, path.PathData.Points[3].X);
            Assert.Equal(3f, path.PathPoints[3].Y);
            Assert.Equal(129, path.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangle_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));
            CheckRectangle(gp, 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangle_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new RectangleF(1f, 1f, 2f, 2f));
            CheckRectangle(gp, 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangle_SamePoint()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 0, 0));
            Assert.Equal(0, gp.PointCount);

            gp.AddRectangle(new Rectangle(1, 1, 1, 1));
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(129, gp.PathTypes[3]);
            PointF end = gp.PathPoints[3];

            // add rectangle at the last path point
            gp.AddRectangle(new Rectangle((int)end.X, (int)end.Y, 1, 1));
            // no compression (different type)
            Assert.Equal(8, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(129, gp.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangle_SamePointF()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new RectangleF(1f, 1f, 0f, 0f));
            Assert.Equal(0, gp.PointCount);

            gp.AddRectangle(new RectangleF(1f, 1f, 1f, 1f));
            Assert.Equal(4, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(129, gp.PathTypes[3]);
            PointF end = gp.PathPoints[3];

            // add rectangle at the last path point
            gp.AddRectangle(new RectangleF(end.X, end.Y, 1f, 1f));
            // no compression (different type)
            Assert.Equal(8, gp.PointCount);
            Assert.Equal(0, gp.PathTypes[0]);
            Assert.Equal(1, gp.PathTypes[1]);
            Assert.Equal(1, gp.PathTypes[2]);
            Assert.Equal(129, gp.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Int_Null()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentNullException>(() => gp.AddRectangles((Rectangle[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Int_Empty()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddRectangles(new Rectangle[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Int()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangles(new Rectangle[1] { new Rectangle(1, 1, 2, 2) });
            CheckRectangle(gp, 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Float_Null()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentNullException>(() => gp.AddRectangles((RectangleF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Float_Empty()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddRectangles(new RectangleF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Float()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangles(new RectangleF[1] { new RectangleF(1f, 1f, 2f, 2f) });
            CheckRectangle(gp, 4);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_Two()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangles(new RectangleF[2] {
                new RectangleF (1f, 1f, 2f, 2f),
                new RectangleF (2f, 2f, 1f, 1f) });
            RectangleF rect = gp.GetBounds();
            Assert.Equal(1f, rect.X);
            Assert.Equal(1f, rect.Y);
            Assert.Equal(2f, rect.Width);
            Assert.Equal(2f, rect.Height);
            // second rectangle is completely within the first one
            CheckRectangle(gp, 8);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddRectangles_SamePoint()
        {
            Rectangle r1 = new Rectangle(1, 1, 0, 0);
            Rectangle r2 = new Rectangle(1, 1, 1, 1);
            Rectangle r3 = new Rectangle(1, 2, 1, 1);

            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangles(new Rectangle[] { r1, r2, r3 });
            Assert.Equal(8, gp.PointCount);
            // first rect is ignore, then all other 2x4 (8) points are present, no compression
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPath_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddPath(null, false));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddPath()
        {
            GraphicsPath gpr = new GraphicsPath();
            gpr.AddRectangle(new Rectangle(1, 1, 2, 2));
            GraphicsPath gp = new GraphicsPath();
            gp.AddPath(gpr, true);
            CheckRectangle(gp, 4);
        }

        private void AssertEqualWithTolerance(float expected, float actual, float tolerance)
        {
            var difference = Math.Abs(expected - actual);
            Assert.True(difference < tolerance);
        }

        private void CheckClosedCurve(GraphicsPath path)
        {
            Assert.Equal(10, path.PathPoints.Length);
            Assert.Equal(10, path.PathTypes.Length);
            Assert.Equal(10, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't very precise with curves
            RectangleF rect = path.GetBounds();
            AssertEqualWithTolerance(0.8333333f, rect.X, 0.2f);
            AssertEqualWithTolerance(0.8333333f, rect.Y, 0.2f);
            AssertEqualWithTolerance(2.33333278f, rect.Width, 0.4f);
            AssertEqualWithTolerance(2.33333278f, rect.Height, 0.4f);

            Assert.Equal(0, path.PathData.Types[0]);
            for (int i = 1; i < 9; i++)
                Assert.Equal(3, path.PathTypes[i]);
            Assert.Equal(131, path.PathData.Types[9]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddClosedCurve((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_Point_0()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddClosedCurve(new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_Point_1()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddClosedCurve(new Point[1] { new Point(1, 1) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_Point_2()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddClosedCurve(new Point[2] { new Point(1, 1), new Point(2, 2) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_Point_3()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddClosedCurve(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
            CheckClosedCurve(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddClosedCurve((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_PointF_0()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddClosedCurve(new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_PointF_1()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddClosedCurve(new PointF[1] { new PointF(1f, 1f) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_PointF_2()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddClosedCurve(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_PointF_3()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddClosedCurve(new PointF[3] { new PointF(1f, 1f), new PointF(2f, 2f), new PointF(3f, 3f) });
            CheckClosedCurve(gp);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_SamePoint()
        {
            Point[] points = new Point[3] { new Point(1, 1), new Point(1, 1), new Point(1, 1) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddClosedCurve(points);
            Assert.Equal(10, gp.PointCount);
            gp.AddClosedCurve(points);
            Assert.Equal(20, gp.PointCount);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddClosedCurve_SamePointF()
        {
            PointF[] points = new PointF[3] { new PointF(1f, 1f), new PointF(1f, 1f), new PointF(1f, 1f) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddClosedCurve(points);
            Assert.Equal(10, gp.PointCount);
            gp.AddClosedCurve(points);
            Assert.Equal(20, gp.PointCount);
        }

        private void CheckCurve(GraphicsPath path)
        {
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            // GetBounds (well GdipGetPathWorldBounds) isn't implemented
            RectangleF rect = path.GetBounds();
            Assert.Equal(1.0f, rect.X);
            Assert.Equal(1.0f, rect.Y);
            Assert.Equal(1.0f, rect.Width);
            Assert.Equal(1.0f, rect.Height);

            Assert.Equal(1f, path.PathData.Points[0].X);
            Assert.Equal(1f, path.PathPoints[0].Y);
            Assert.Equal(0, path.PathData.Types[0]);
            // Mono has wrong? results
#if false
			Assert.Equal (1.16666663f, path.PathData.Points[1].X);
			Assert.Equal (1.16666663f, path.PathPoints[1].Y);
#endif
            Assert.Equal(3, path.PathTypes[1]);
            // Mono has wrong? results
#if false
			Assert.Equal (1.83333325f, path.PathData.Points[2].X);
			Assert.Equal (1.83333325f, path.PathPoints[2].Y);
#endif
            Assert.Equal(3, path.PathData.Types[2]);
            Assert.Equal(2f, path.PathData.Points[3].X);
            Assert.Equal(2f, path.PathPoints[3].Y);
            Assert.Equal(3, path.PathTypes[3]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_Point_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddCurve((Point[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_Point_0()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new Point[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_Point_1()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new Point[1] { new Point(1, 1) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_Point_2()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(new Point[2] { new Point(1, 1), new Point(2, 2) });
            CheckCurve(gp);
            // note: GdipAddPathCurveI allows adding a "curve" with only 2 points (a.k.a. a line );-)
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_Point_2_Tension()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(new Point[2] { new Point(1, 1), new Point(2, 2) }, 1.0f);
            CheckCurve(gp);
            // note: GdipAddPathCurve2I allows adding a "curve" with only 2 points (a.k.a. a line );-)
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve3_Point_2()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new Point[2] { new Point(1, 1), new Point(2, 2) }, 0, 2, 0.5f));
            // adding only two points isn't supported by GdipAddCurve3I
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_PointF_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddCurve((PointF[])null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_PointF_0()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new PointF[0]));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_PointF_1()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new PointF[1] { new PointF(1f, 1f) }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_PointF_2()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) });
            CheckCurve(gp);
            // note: GdipAddPathCurve allows adding a "curve" with only 2 points (a.k.a. a line );-)
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_PoinFt_2_Tension()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 1.0f);
            CheckCurve(gp);
            // note: GdipAddPathCurve2 allows adding a "curve" with only 2 points (a.k.a. a line );-)
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve3_PointF_2()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 0, 2, 0.5f));
            // adding only two points isn't supported by GdipAddCurve3
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_LargeTension()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(new PointF[3] { new PointF(1f, 1f), new PointF(0f, 20f), new PointF(20f, 0f) }, 0, 2, Single.MaxValue);
            Assert.Equal(7, gp.PointCount);
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_ZeroSegments()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 0, 0, 0.5f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_NegativeSegments()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 0, -1, 0.5f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_OffsetTooLarge()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddCurve(new PointF[3] { new PointF(1f, 1f), new PointF(0f, 20f), new PointF(20f, 0f) }, 1, 2, 0.5f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_Offset()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(new PointF[4] { new PointF(1f, 1f), new PointF(0f, 20f), new PointF(20f, 0f), new PointF(0f, 10f) }, 1, 2, 0.5f);
            Assert.Equal(7, gp.PointCount);
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_SamePoint()
        {
            Point[] points = new Point[2] { new Point(1, 1), new Point(1, 1) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(points);
            Assert.Equal(4, gp.PointCount);
            gp.AddCurve(points);
            Assert.Equal(7, gp.PointCount);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddCurve_SamePointF()
        {
            PointF[] points = new PointF[2] { new PointF(1f, 1f), new PointF(1f, 1f) };
            GraphicsPath gp = new GraphicsPath();
            gp.AddCurve(points);
            Assert.Equal(4, gp.PointCount);
            gp.AddCurve(points);
            Assert.Equal(7, gp.PointCount);
        }

        [ActiveIssue(20844)]
        public void AddCurve()
        {
            PointF[] points = new PointF[] {
                new PointF (37f, 185f),
                new PointF (99f, 185f),
                new PointF (161f, 159f),
                new PointF (223f, 185f),
                new PointF (285f, 54f),
            };
            int[] count = { 4, 7, 10, 13 };

            using (GraphicsPath gp = new GraphicsPath())
            {
                for (int i = 0; i < points.Length - 1; i++)
                {
                    gp.AddCurve(points, i, 1, 0.5f);
                    // all non-curves points are compressed expect the first one (positioning)
                    Assert.Equal(count[i], gp.PointCount);
                }

                Assert.Equal(0, gp.PathData.Types[0]);
                Assert.Equal(37f, gp.PathData.Points[0].X, Precision);
                Assert.Equal(185f, gp.PathData.Points[1].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[1]);
                Assert.Equal(47.3334f, gp.PathData.Points[1].X, Precision);
                Assert.Equal(185f, gp.PathData.Points[1].Y, 3);
                Assert.Equal(3, gp.PathData.Types[2]);
                Assert.Equal(78.33333f, gp.PathData.Points[2].X, Precision);
                Assert.Equal(189.3333f, gp.PathData.Points[2].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[3]);
                Assert.Equal(99f, gp.PathData.Points[3].X, Precision);
                Assert.Equal(185f, gp.PathData.Points[3].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[4]);
                Assert.Equal(119.6667f, gp.PathData.Points[4].X, Precision);
                Assert.Equal(180.6667f, gp.PathData.Points[4].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[5]);
                Assert.Equal(140.3333f, gp.PathData.Points[5].X, Precision);
                Assert.Equal(159f, gp.PathData.Points[5].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[6]);
                Assert.Equal(161f, gp.PathData.Points[6].X, Precision);
                Assert.Equal(159f, gp.PathData.Points[6].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[7]);
                Assert.Equal(181.6667f, gp.PathData.Points[7].X, Precision);
                Assert.Equal(159f, gp.PathData.Points[7].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[8]);
                Assert.Equal(202.3333f, gp.PathData.Points[8].X, Precision);
                Assert.Equal(202.5f, gp.PathData.Points[8].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[9]);
                Assert.Equal(223f, gp.PathData.Points[9].X, Precision);
                Assert.Equal(185f, gp.PathData.Points[9].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[10]);
                Assert.Equal(243.6667f, gp.PathData.Points[10].X, Precision);
                Assert.Equal(167.5f, gp.PathData.Points[10].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[11]);
                Assert.Equal(274.6667f, gp.PathData.Points[11].X, Precision);
                Assert.Equal(75.83334f, gp.PathData.Points[11].Y, Precision);
                Assert.Equal(3, gp.PathData.Types[12]);
                Assert.Equal(285f, gp.PathData.Points[12].X, Precision);
                Assert.Equal(54f, gp.PathData.Points[12].Y, Precision);
            }
        }

        private FontFamily GetFontFamily()
        {
            try
            {
                return FontFamily.GenericMonospace;
            }
            catch (ArgumentException)
            {
                Assert.True(false, "GenericMonospace FontFamily couldn't be found");
                return null;
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddString_NullString()
        {
            GraphicsPath gp = new GraphicsPath();
            FontFamily ff = GetFontFamily();
            Assert.Throws<NullReferenceException>(() => gp.AddString(null, ff, 0, 10, new Point(10, 10), StringFormat.GenericDefault));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddString_EmptyString()
        {
            GraphicsPath gp = new GraphicsPath();
            FontFamily ff = GetFontFamily();
            gp.AddString(String.Empty, ff, 0, 10, new Point(10, 10), StringFormat.GenericDefault);
            Assert.Equal(0, gp.PointCount);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddString_NullFontFamily()
        {
            GraphicsPath gp = new GraphicsPath();
            Assert.Throws<ArgumentException>(() => gp.AddString("mono", null, 0, 10, new Point(10, 10), StringFormat.GenericDefault));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void AddString_NegativeSize()
        {
            GraphicsPath gp = new GraphicsPath();
            FontFamily ff = GetFontFamily();
            gp.AddString("mono", ff, 0, -10, new Point(10, 10), StringFormat.GenericDefault);
            Assert.True(gp.PointCount > 0);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_Empty_Empty()
        {
            GraphicsPath gp = new GraphicsPath();
            RectangleF rect = gp.GetBounds();
            Assert.Equal(0.0f, rect.X);
            Assert.Equal(0.0f, rect.Y);
            Assert.Equal(0.0f, rect.Width);
            Assert.Equal(0.0f, rect.Height);
        }

        private void CheckRectangleBounds(RectangleF rect)
        {
            Assert.Equal(1.0f, rect.X);
            Assert.Equal(1.0f, rect.Y);
            Assert.Equal(2.0f, rect.Width);
            Assert.Equal(2.0f, rect.Height);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_Empty_Rectangle()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));
            CheckRectangleBounds(gp.GetBounds());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_Null_Rectangle()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));
            CheckRectangleBounds(gp.GetBounds(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_MatrixEmpty_Rectangle()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));
            CheckRectangleBounds(gp.GetBounds(new Matrix()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_NullNull_Rectangle()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(new Rectangle(1, 1, 2, 2));
            CheckRectangleBounds(gp.GetBounds(null, null));
        }

        private void CheckPieBounds(RectangleF rect)
        {
            Assert.Equal(60.0f, rect.X, 1);
            Assert.Equal(60.0f, rect.Y, 1);
            Assert.Equal(43.3f, rect.Width, 1);
            Assert.Equal(48.3f, rect.Height, 1);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_Empty_Pie()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(10, 10, 100, 100, 30, 45);
            CheckPieBounds(gp.GetBounds());
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_Null_Pie()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(10, 10, 100, 100, 30, 45);
            CheckPieBounds(gp.GetBounds(null));
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_MatrixEmpty_Pie()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(10, 10, 100, 100, 30, 45);
            CheckPieBounds(gp.GetBounds(new Matrix()));
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_NullNull_Pie()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPie(10, 10, 100, 100, 30, 45);
            CheckPieBounds(gp.GetBounds(null, null));
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetBounds_Empty_ClosedCurve()
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddClosedCurve(new Point[4] { new Point (20, 100), new Point (70, 10),
                new Point (130, 200), new Point (180, 100) });
#if false
			// so far from reality that it's totally useless
			Assert.Equal (1.666666f, rect.X, 0.00001);
			Assert.Equal (-6.66666f, rect.Y, 1);
			Assert.Equal (196.6666f, rect.Width, 1);
			Assert.Equal (221.6666f, rect.Height, 1);
#endif
            gp.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Transform(null));
        }
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Transform_Empty()
        {
            // no points in path and no exception
            new GraphicsPath().Transform(new Matrix());
        }

        private void ComparePaths(GraphicsPath expected, GraphicsPath actual)
        {
            Assert.Equal(expected.PointCount, actual.PointCount);
            for (int i = 0; i < expected.PointCount; i++)
            {
                Assert.Equal(expected.PathPoints[i], actual.PathPoints[i]);
                Assert.Equal(expected.PathTypes[i], actual.PathTypes[i]);
            }
        }

        private void CompareFlats(GraphicsPath flat, GraphicsPath original)
        {
            Assert.True(flat.PointCount >= original.PointCount);
            for (int i = 0; i < flat.PointCount; i++)
            {
                Assert.True(flat.PathTypes[i] != 3);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Empty()
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath clone = (GraphicsPath)path.Clone();
            // this is a no-op as there's nothing in the path
            path.Flatten();
            ComparePaths(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Null()
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath clone = (GraphicsPath)path.Clone();
            // this is a no-op as there's nothing in the path
            // an no matrix to apply
            path.Flatten(null);
            ComparePaths(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_NullFloat()
        {
            GraphicsPath path = new GraphicsPath();
            GraphicsPath clone = (GraphicsPath)path.Clone();
            // this is a no-op as there's nothing in the path
            // an no matrix to apply
            path.Flatten(null, 1f);
            ComparePaths(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Arc()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0f, 0f, 100f, 100f, 30, 30);
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            CompareFlats(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Bezier()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddBezier(0, 0, 100, 100, 30, 30, 60, 60);
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            CompareFlats(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_ClosedCurve()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddClosedCurve(new Point[4] {
                new Point (0, 0), new Point (40, 20),
                new Point (20, 40), new Point (40, 40)
                });
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            CompareFlats(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Curve()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddCurve(new Point[4] {
                new Point (0, 0), new Point (40, 20),
                new Point (20, 40), new Point (40, 40)
                });
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            CompareFlats(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Ellipse()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(10f, 10f, 100f, 100f);
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            CompareFlats(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Line()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(10f, 10f, 100f, 100f);
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            ComparePaths(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Pie()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPie(0, 0, 100, 100, 30, 30);
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            CompareFlats(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Polygon()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(new Point[4] {
                new Point (0, 0), new Point (10, 10),
                new Point (20, 20), new Point (40, 40)
                });
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            ComparePaths(path, clone);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Flatten_Rectangle()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(0, 0, 100, 100));
            GraphicsPath clone = (GraphicsPath)path.Clone();
            path.Flatten();
            ComparePaths(path, clone);
        }

        private void CheckWrap(GraphicsPath path)
        {
            Assert.Equal(3, path.PointCount);

            PointF[] pts = path.PathPoints;
            Assert.Equal(0, pts[0].X, Precision);
            Assert.Equal(0, pts[0].Y, Precision);
            Assert.Equal(0, pts[1].X, Precision);
            Assert.Equal(0, pts[1].Y, Precision);
            Assert.Equal(0, pts[2].X, Precision);
            Assert.Equal(0, pts[2].Y, Precision);

            byte[] types = path.PathTypes;
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[1]);
            Assert.Equal(129, types[2]);
        }

        private void CheckWrapNaN(GraphicsPath path, bool closed)
        {
            Assert.Equal(3, path.PointCount);

            PointF[] pts = path.PathPoints;
            Assert.Equal(Single.NaN, pts[0].X);
            Assert.Equal(Single.NaN, pts[0].Y);
            Assert.Equal(Single.NaN, pts[1].X);
            Assert.Equal(Single.NaN, pts[1].Y);
            Assert.Equal(Single.NaN, pts[2].X);
            Assert.Equal(Single.NaN, pts[2].Y);

            byte[] types = path.PathTypes;
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[1]);
            Assert.Equal(closed ? 129 : 1, types[2]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Warp_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Warp(null, new RectangleF()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Warp_NoPoints()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().Warp(new PointF[0], new RectangleF()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Wrap_NoPoint()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Assert.Equal(0, gp.PointCount);

                PointF[] pts = new PointF[1] { new PointF(0, 0) };
                RectangleF r = new RectangleF(10, 20, 30, 40);
                gp.Warp(pts, r, new Matrix());
                Assert.Equal(0, gp.PointCount);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Warp_Invalid()
        {
            PointF[] pts = new PointF[1] { new PointF(0, 0) };
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(new Point[3] { new Point(5, 5), new Point(15, 5), new Point(10, 15) });
            RectangleF r = new RectangleF(10, 20, 30, 40);
            path.Warp(pts, r, new Matrix(), (WarpMode)Int32.MinValue);
            Assert.Equal(0, path.PointCount);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetMarkers_EmptyPath()
        {
            new GraphicsPath().SetMarkers();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ClearMarkers_EmptyPath()
        {
            new GraphicsPath().ClearMarkers();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CloseFigure_EmptyPath()
        {
            new GraphicsPath().CloseFigure();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void CloseAllFigures_EmptyPath()
        {
            new GraphicsPath().CloseAllFigures();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddArc()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddArc(10, 10, 100, 100, 90, 180);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(3, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddBezier()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddBezier(10, 10, 100, 100, 20, 20, 200, 200);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(3, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddBeziers()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddBeziers(new Point[7] { new Point (10, 10),
                new Point (20, 10), new Point (20, 20), new Point (30, 20),
                new Point (40, 40), new Point (50, 40), new Point (50, 50)
            });
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(3, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddClosedCurve()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddClosedCurve(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            Assert.Equal(131, types[path.PointCount - 3]);
            Assert.Equal(0, types[path.PointCount - 2]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddCurve()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddCurve(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(3, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddEllipse()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddEllipse(10, 10, 100, 100);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            Assert.Equal(131, types[path.PointCount - 3]);
            Assert.Equal(0, types[path.PointCount - 2]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddLine()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddLine(5, 5, 10, 10);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(1, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddLines()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddLines(new Point[4] { new Point(10, 10), new Point(20, 10), new Point(20, 20), new Point(30, 20) });
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(1, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddPath_Connect()
        {
            GraphicsPath inner = new GraphicsPath();
            inner.AddArc(10, 10, 100, 100, 90, 180);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddPath(inner, true);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[2]);
            // check last types
            Assert.Equal(3, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddPath_NoConnect()
        {
            GraphicsPath inner = new GraphicsPath();
            inner.AddArc(10, 10, 100, 100, 90, 180);
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddPath(inner, false);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            Assert.Equal(3, types[path.PointCount - 3]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddPie()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddPie(10, 10, 10, 10, 90, 180);
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            // libgdiplus draws pie by ending with a line (not a curve) section
            Assert.True((types[path.PointCount - 3] & 128) == 128);
            Assert.Equal(0, types[path.PointCount - 2]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddPolygon()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddPolygon(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            Assert.Equal(129, types[path.PointCount - 3]);
            Assert.Equal(0, types[path.PointCount - 2]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddRectangle()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddRectangle(new RectangleF(10, 10, 20, 20));
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            Assert.Equal(129, types[path.PointCount - 3]);
            Assert.Equal(0, types[path.PointCount - 2]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void StartClose_AddRectangles()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(1, 1, 2, 2);
            path.AddRectangles(new RectangleF[2] {
                new RectangleF (10, 10, 20, 20),
                new RectangleF (20, 20, 10, 10) });
            path.AddLine(10, 10, 20, 20);
            byte[] types = path.PathTypes;
            // check first types
            Assert.Equal(0, types[0]);
            Assert.Equal(0, types[2]);
            // check last types
            Assert.Equal(129, types[path.PointCount - 3]);
            Assert.Equal(0, types[path.PointCount - 2]);
            Assert.Equal(1, types[path.PointCount - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Widen_Pen_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Widen(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Widen_Pen_Null_Matrix()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Widen(null, new Matrix()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Widen_NoPoint()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Assert.Equal(0, gp.PointCount);
                Pen pen = new Pen(Color.Blue);
                gp.Widen(pen);
                Assert.Equal(0, gp.PointCount);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Widen_SinglePoint()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(new Point[1] { new Point(1, 1) });
                // Special case - a line with a single point is valid
                Assert.Equal(1, gp.PointCount);
                Assert.Throws<OutOfMemoryException>(() => gp.Widen(Pens.Red));
                // oops );-)
            }
        }

        private void CheckWiden3(GraphicsPath path)
        {
            PointF[] pts = path.PathPoints;
            Assert.Equal(4.2, pts[0].X, LowPrecision);
            Assert.Equal(4.5, pts[0].Y, LowPrecision);
            Assert.Equal(15.8, pts[1].X, LowPrecision);
            Assert.Equal(4.5, pts[1].Y, LowPrecision);
            Assert.Equal(10.0, pts[2].X, LowPrecision);
            Assert.Equal(16.1, pts[2].Y, LowPrecision);
            Assert.Equal(10.4, pts[3].X, LowPrecision);
            Assert.Equal(14.8, pts[3].Y, LowPrecision);
            Assert.Equal(9.6, pts[4].X, LowPrecision);
            Assert.Equal(14.8, pts[4].Y, LowPrecision);
            Assert.Equal(14.6, pts[5].X, LowPrecision);
            Assert.Equal(4.8, pts[5].Y, LowPrecision);
            Assert.Equal(15.0, pts[6].X, LowPrecision);
            Assert.Equal(5.5, pts[6].Y, LowPrecision);
            Assert.Equal(5.0, pts[7].X, LowPrecision);
            Assert.Equal(5.5, pts[7].Y, LowPrecision);
            Assert.Equal(5.4, pts[8].X, LowPrecision);
            Assert.Equal(4.8, pts[8].Y, LowPrecision);

            byte[] types = path.PathTypes;
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[1]);
            Assert.Equal(129, types[2]);
            Assert.Equal(0, types[3]);
            Assert.Equal(1, types[4]);
            Assert.Equal(1, types[5]);
            Assert.Equal(1, types[6]);
            Assert.Equal(1, types[7]);
            Assert.Equal(129, types[8]);
        }

        private void CheckWidenedBounds(string message, GraphicsPath gp, Matrix m)
        {
            RectangleF bounds = gp.GetBounds(m);
            Assert.Equal(0.5f, bounds.X, Precision);
            Assert.Equal(0.5f, bounds.Y, Precision);
            Assert.Equal(3.0f, bounds.Width, Precision);
            Assert.Equal(3.0f, bounds.Height, Precision);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_IntNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(1, 1, null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_FloatNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(1.0f, 1.0f, null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_PointNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(new Point(), null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_PointFNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(new PointF(), null));
        }

        private void IsOutlineVisible_Line(Graphics graphics)
        {
            Pen p2 = new Pen(Color.Red, 3.0f);
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(10, 1, 14, 1);
                Assert.True(gp.IsOutlineVisible(10, 1, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(10, 2, p2, graphics));
                Assert.False(gp.IsOutlineVisible(10, 2, Pens.Red, graphics));

                Assert.True(gp.IsOutlineVisible(11.0f, 1.0f, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(11.0f, 1.0f, p2, graphics));
                Assert.False(gp.IsOutlineVisible(11.0f, 2.0f, Pens.Red, graphics));

                Point pt = new Point(12, 2);
                Assert.False(gp.IsOutlineVisible(pt, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(pt, p2, graphics));
                pt.Y = 1;
                Assert.True(gp.IsOutlineVisible(pt, Pens.Red, graphics));

                PointF pf = new PointF(13.0f, 2.0f);
                Assert.False(gp.IsOutlineVisible(pf, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(pf, p2, graphics));
                pf.Y = 1;
                Assert.True(gp.IsOutlineVisible(pf, Pens.Red, graphics));
            }
            p2.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Line_WithoutGraphics()
        {
            IsOutlineVisible_Line(null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Line_WithGraphics_Inside()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IsOutlineVisible_Line(g);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Line_WithGraphics_Outside()
        {
            using (Bitmap bitmap = new Bitmap(5, 5))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IsOutlineVisible_Line(g);
                }
                // graphics "seems" ignored as the line is outside the bitmap!
            }
        }

        // docs ways the point is in world coordinates and that the graphics transform 
        // should be applied

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Line_WithGraphics_Transform()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Transform = new Matrix(2, 0, 0, 2, 50, -50);
                    IsOutlineVisible_Line(g);
                }
                // graphics still "seems" ignored (Transform)
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Line_WithGraphics_PageUnit()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.PageUnit = GraphicsUnit.Millimeter;
                    IsOutlineVisible_Line(g);
                }
                // graphics still "seems" ignored (PageUnit)
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Line_WithGraphics_PageScale()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.PageScale = 2.0f;
                    IsOutlineVisible_Line(g);
                }
                // graphics still "seems" ignored (PageScale)
            }
        }

        private void IsOutlineVisible_Rectangle(Graphics graphics)
        {
            Pen p2 = new Pen(Color.Red, 3.0f);
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(10, 10, 20, 20));
                Assert.True(gp.IsOutlineVisible(10, 10, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(10, 11, p2, graphics));
                Assert.False(gp.IsOutlineVisible(11, 11, Pens.Red, graphics));

                Assert.True(gp.IsOutlineVisible(11.0f, 10.0f, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(11.0f, 11.0f, p2, graphics));
                Assert.False(gp.IsOutlineVisible(11.0f, 11.0f, Pens.Red, graphics));

                Point pt = new Point(15, 10);
                Assert.True(gp.IsOutlineVisible(pt, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(pt, p2, graphics));
                pt.Y = 15;
                Assert.False(gp.IsOutlineVisible(pt, Pens.Red, graphics));

                PointF pf = new PointF(29.0f, 29.0f);
                Assert.False(gp.IsOutlineVisible(pf, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(pf, p2, graphics));
                pf.Y = 31.0f;
                Assert.True(gp.IsOutlineVisible(pf, p2, graphics));
            }
            p2.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsOutlineVisible_Rectangle_WithoutGraphics()
        {
            IsOutlineVisible_Rectangle(null);
        }

        private void IsVisible_Rectangle(Graphics graphics)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(10, 10, 20, 20));
                Assert.False(gp.IsVisible(9, 9, graphics));
                Assert.True(gp.IsVisible(10, 10, graphics));
                Assert.True(gp.IsVisible(20, 20, graphics));
                Assert.True(gp.IsVisible(29, 29, graphics));
                Assert.False(gp.IsVisible(30, 29, graphics));
                Assert.False(gp.IsVisible(29, 30, graphics));
                Assert.False(gp.IsVisible(30, 30, graphics));

                Assert.False(gp.IsVisible(9.4f, 9.4f, graphics));
                Assert.True(gp.IsVisible(9.5f, 9.5f, graphics));
                Assert.True(gp.IsVisible(10f, 10f, graphics));
                Assert.True(gp.IsVisible(20f, 20f, graphics));
                // the next diff is too close, so this fails with libgdiplus/cairo
                //Assert.True (gp.IsVisible (29.4f, 29.4f, graphics));
                Assert.False(gp.IsVisible(29.5f, 29.5f, graphics));
                Assert.False(gp.IsVisible(29.5f, 29.4f, graphics));
                Assert.False(gp.IsVisible(29.4f, 29.5f, graphics));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsVisible_Rectangle_WithoutGraphics()
        {
            IsVisible_Rectangle(null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsVisible_Rectangle_WithGraphics()
        {
            using (Bitmap bitmap = new Bitmap(40, 40))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IsVisible_Rectangle(g);
                }
            }
        }

        // bug #325502 has shown that ellipse didn't work with earlier code
        private void IsVisible_Ellipse(Graphics graphics)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(new Rectangle(10, 10, 20, 20));
                Assert.False(gp.IsVisible(10, 10, graphics));
                Assert.True(gp.IsVisible(20, 20, graphics));
                Assert.False(gp.IsVisible(29, 29, graphics));

                Assert.False(gp.IsVisible(10f, 10f, graphics));
                Assert.True(gp.IsVisible(20f, 20f, graphics));
                Assert.False(gp.IsVisible(29.4f, 29.4f, graphics));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsVisible_Ellipse_WithoutGraphics()
        {
            IsVisible_Ellipse(null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsVisible_Ellipse_WithGraphics()
        {
            using (Bitmap bitmap = new Bitmap(40, 40))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IsVisible_Ellipse(g);
                }
            }
        }

        // Reverse simple test cases

        private void Reverse(GraphicsPath gp)
        {
            PointF[] bp = gp.PathPoints;
            byte[] bt = gp.PathTypes;

            gp.Reverse();
            PointF[] ap = gp.PathPoints;
            byte[] at = gp.PathTypes;

            int count = gp.PointCount;
            Assert.Equal(bp.Length, count);
            for (int i = 0; i < count; i++)
            {
                Assert.Equal(bp[i], ap[count - i - 1]);
                Assert.Equal(bt[i], at[i]);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Arc()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Bezier()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddBezier(1, 2, 3, 4, 5, 6, 7, 8);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Beziers()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Point[] beziers = new Point[] { new Point (1,2), new Point (3,4), new Point (5,6),
                    new Point (7,8), new Point (9,10), new Point (11,12), new Point (13,14) };
                gp.AddBeziers(beziers);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_ClosedCurve()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Point[] beziers = new Point[] { new Point (1,2), new Point (3,4), new Point (5,6),
                    new Point (7,8), new Point (9,10), new Point (11,12), new Point (13,14) };
                gp.AddClosedCurve(beziers);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Curve()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Point[] beziers = new Point[] { new Point (1,2), new Point (3,4), new Point (5,6),
                    new Point (7,8), new Point (9,10), new Point (11,12), new Point (13,14) };
                gp.AddCurve(beziers);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Ellipse()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(1, 2, 3, 4);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Line()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 2, 3, 4);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Line_Closed()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 2, 3, 4);
                gp.CloseFigure();
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Lines()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Point[] points = new Point[] { new Point (1,2), new Point (3,4), new Point (5,6),
                    new Point (7,8), new Point (9,10), new Point (11,12), new Point (13,14) };
                gp.AddLines(points);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Polygon()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Point[] points = new Point[] { new Point (1,2), new Point (3,4), new Point (5,6),
                    new Point (7,8), new Point (9,10), new Point (11,12), new Point (13,14) };
                gp.AddPolygon(points);
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Rectangle()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(1, 2, 3, 4));
                Reverse(gp);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Rectangles()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Rectangle[] rects = new Rectangle[] { new Rectangle(1, 2, 3, 4), new Rectangle(5, 6, 7, 8) };
                gp.AddRectangles(rects);
                Reverse(gp);
            }
        }

        // Reverse complex test cases

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Path()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                GraphicsPath path = new GraphicsPath();
                path.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
                path.AddLine(1, 2, 3, 4);
                gp.AddPath(path, true);
                PointF[] bp = gp.PathPoints;
                byte[] expected = new byte[] { 0, 1, 1, 3, 3, 3 };

                gp.Reverse();
                PointF[] ap = gp.PathPoints;
                byte[] at = gp.PathTypes;

                int count = gp.PointCount;
                Assert.Equal(bp.Length, count);
                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(bp[i], ap[count - i - 1]);
                    Assert.Equal(expected[i], at[i]);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Path_2()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(50, 51, 50, 100);
                gp.AddRectangle(new Rectangle(200, 201, 60, 61));
                PointF[] bp = gp.PathPoints;
                byte[] expected = new byte[] { 0, 1, 1, 129, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 131 };

                gp.Reverse();
                PointF[] ap = gp.PathPoints;
                byte[] at = gp.PathTypes;

                int count = gp.PointCount;
                Assert.Equal(bp.Length, count);
                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(bp[i], ap[count - i - 1]);
                    Assert.Equal(expected[i], at[i]);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Marker()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(200, 201, 60, 61));
                gp.SetMarkers();
                PointF[] bp = gp.PathPoints;
                byte[] expected = new byte[] { 0, 1, 1, 129 };

                gp.Reverse();
                PointF[] ap = gp.PathPoints;
                byte[] at = gp.PathTypes;

                int count = gp.PointCount;
                Assert.Equal(bp.Length, count);
                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(bp[i], ap[count - i - 1]);
                    Assert.Equal(expected[i], at[i]);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Subpath_Marker()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(0, 1, 2, 3);
                gp.SetMarkers();
                gp.CloseFigure();
                gp.AddBezier(5, 6, 7, 8, 9, 10, 11, 12);
                gp.CloseFigure();
                PointF[] bp = gp.PathPoints;
                byte[] expected = new byte[] { 0, 3, 3, 163, 0, 129 };

                gp.Reverse();
                PointF[] ap = gp.PathPoints;
                byte[] at = gp.PathTypes;

                int count = gp.PointCount;
                Assert.Equal(bp.Length, count);
                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(bp[i], ap[count - i - 1]);
                    Assert.Equal(expected[i], at[i]);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Reverse_Subpath_Marker_2()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(0, 1, 2, 3);
                gp.SetMarkers();
                gp.StartFigure();
                gp.AddLine(20, 21, 22, 23);
                gp.AddBezier(5, 6, 7, 8, 9, 10, 11, 12);
                PointF[] bp = gp.PathPoints;
                byte[] expected = new byte[] { 0, 3, 3, 3, 1, 33, 0, 1 };

                gp.Reverse();
                PointF[] ap = gp.PathPoints;
                byte[] at = gp.PathTypes;

                int count = gp.PointCount;
                Assert.Equal(bp.Length, count);
                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(bp[i], ap[count - i - 1]);
                    Assert.Equal(expected[i], at[i]);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void bug413461()
        {
            int dX = 520;
            int dY = 320;
            Point[] expected_points = new Point[] {
                new Point(dX-64, dY-24),//start
				new Point(dX-59, dY-34),//focal point 1
				new Point(dX-52, dY-54),//focal point 2
				new Point(dX-18, dY-66),//top
				new Point(dX-34, dY-47),//focal point 1
				new Point(dX-43, dY-27),//focal point 2
				new Point(dX-44, dY-8),//end
				};
            byte[] expected_types = new byte[] {
                (byte)PathPointType.Start,
                (byte)PathPointType.Bezier,
                (byte)PathPointType.Bezier,
                (byte)PathPointType.Bezier,
                (byte)PathPointType.Bezier,
                (byte)PathPointType.Bezier,
                (byte)PathPointType.Bezier };
            using (GraphicsPath path = new GraphicsPath(expected_points, expected_types))
            {
                Assert.Equal(7, path.PointCount);
                byte[] actual_types = path.PathTypes;
                Assert.Equal(expected_types[0], actual_types[0]);
                Assert.Equal(expected_types[1], actual_types[1]);
                Assert.Equal(expected_types[2], actual_types[2]);
                Assert.Equal(expected_types[3], actual_types[3]);
                Assert.Equal(expected_types[4], actual_types[4]);
                Assert.Equal(expected_types[5], actual_types[5]);
                // path is filled like closed but this does not show on the type
                Assert.Equal(expected_types[6], actual_types[6]);
            }
        }
    }
}
