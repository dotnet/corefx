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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Xunit;

namespace Drawing2D
{
    public class GraphicsPathTest
    {
        private class RunIfFontFamilyGenericMonospaceNotNull : TheoryAttribute
        {
            public RunIfFontFamilyGenericMonospaceNotNull()
            {
                if (true)
                {
                    Skip = "GenericMonospace FontFamily couldn't be found";
                }
            }
        }

        private const float Pi4 = (float)(Math.PI / 4);
        private const float Delta = 0.0003f;

        [Fact]
        public void Ctor_Default()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Assert.Equal(FillMode.Alternate, gp.FillMode);
                AssertEmptyGrahicsPath(gp);
            }
        }

        [Theory]
        [InlineData(FillMode.Alternate, FillMode.Winding)]
        public void Ctor_FillMode_Succses(FillMode alternate, FillMode winding)
        {
            using (GraphicsPath gpa = new GraphicsPath(alternate))
            using (GraphicsPath gpw = new GraphicsPath(winding))
            {
                Assert.Equal(FillMode.Alternate, gpa.FillMode);
                AssertEmptyGrahicsPath(gpa);
                Assert.Equal(FillMode.Winding, gpw.FillMode);
                AssertEmptyGrahicsPath(gpw);
            }
        }

        [Fact]
        public void Ctor_SamePoints_Succses()
        {
            byte[] types = new byte[6] { 0, 1, 1, 1, 1, 1 };
            Point[] points = new Point[]
            {
                new Point (1, 1), new Point (1, 1), new Point (1, 1),
                new Point (1, 1), new Point (1, 1), new Point (1, 1),
            };

            PointF[] fPoints = new PointF[]
            {
                new PointF (1f, 1f), new PointF (1f, 1f), new PointF (1f, 1f),
                new PointF (1f, 1f), new PointF (1f, 1f), new PointF (1f, 1f),
            };

            using (GraphicsPath gp = new GraphicsPath(points, types))
            using (GraphicsPath gpf = new GraphicsPath(fPoints, types))
            {
                Assert.Equal(FillMode.Alternate, gp.FillMode);
                Assert.Equal(6, gp.PointCount);
                Assert.Equal(FillMode.Alternate, gpf.FillMode);
                Assert.Equal(6, gpf.PointCount);
                types[0] = 1;
                Assert.Equal(FillMode.Alternate, gp.FillMode);
                Assert.Equal(6, gp.PointCount);
                Assert.Equal(FillMode.Alternate, gpf.FillMode);
                Assert.Equal(6, gpf.PointCount);
            }
        }

        [Fact]
        public void Ctor_PointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath((Point[])null, new byte[1]));
        }

        [Fact]
        public void Ctor_PointsTypesLengthMismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath(new Point[1], new byte[2]));
            Assert.Throws<ArgumentException>(() => new GraphicsPath(new Point[2], new byte[1]));
            Assert.Throws<ArgumentException>(() => new GraphicsPath(new PointF[1], new byte[2]));
            Assert.Throws<ArgumentException>(() => new GraphicsPath(new PointF[2], new byte[1]));
        }

        [Fact]
        public void Clone_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                Assert.Equal(FillMode.Alternate, clone.FillMode);
                AssertEmptyGrahicsPath(clone);
            }
        }

        [Fact]
        public void Reset_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.Reset();

                Assert.Equal(FillMode.Alternate, gp.FillMode);
                AssertEmptyGrahicsPath(gp);
            }
        }

        [Fact]
        public void GraphicsPath_FillModeChange()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.FillMode = FillMode.Winding;
                Assert.Equal(FillMode.Winding, gp.FillMode);
            }
        }

        [Fact]
        public void GraphicsPath_InvalidFillMode_ThrowsInvalidEnumArgumentException()
        {
            Assert.Throws<InvalidEnumArgumentException>(() => new GraphicsPath().FillMode = ((FillMode)int.MaxValue));
        }

        [Fact]
        public void PathData_ReturnsExpected()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Assert.Equal(0, gp.PathData.Points.Length);
                Assert.Equal(0, gp.PathData.Types.Length);
            }
        }

        [Fact]
        public void PathData_CannotChange()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(1, 1, 2, 2));
                Assert.Equal(1f, gp.PathData.Points[0].X);
                Assert.Equal(1f, gp.PathData.Points[0].Y);

                gp.PathData.Points[0] = new Point(0, 0);
                Assert.Equal(1f, gp.PathData.Points[0].X);
                Assert.Equal(1f, gp.PathData.Points[0].Y);
            }
        }

        [Fact]
        public void PathPoints_CannotChange()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(1, 1, 2, 2));
                Assert.Equal(1f, gp.PathPoints[0].X);
                Assert.Equal(1f, gp.PathPoints[0].Y);

                gp.PathPoints[0] = new Point(0, 0);
                Assert.Equal(1f, gp.PathPoints[0].X);
                Assert.Equal(1f, gp.PathPoints[0].Y);
            }
        }

        [Fact]
        public void PathPoints_EmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Assert.Null(new GraphicsPath().PathPoints));
        }

        [Fact]
        public void PathTypes_CannotChange()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(1, 1, 2, 2));
                Assert.Equal(0, gp.PathTypes[0]);

                gp.PathTypes[0] = 1;
                Assert.Equal(0, gp.PathTypes[0]);
            }
        }

        [Fact]
        public void PathTypes_EmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Assert.Null(new GraphicsPath().PathTypes));
        }

        [Fact]
        public void GetLastPoint_Success()
        {
            byte[] types = new byte[3] { 0, 1, 1 };
            PointF[] points = new PointF[]
            {
                new PointF (1f, 1f), new PointF (2f, 2f), new PointF (3f, 3f),
            };

            using (GraphicsPath gp = new GraphicsPath(points, types))
            {
                Assert.Equal(gp.GetLastPoint(), points[2]);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddLine_Success(int x1, int y1, int x2, int y2)
        {
            using (GraphicsPath gpInt = new GraphicsPath())
            using (GraphicsPath gpFloat = new GraphicsPath())
            using (GraphicsPath gpPointsInt = new GraphicsPath())
            using (GraphicsPath gpfPointsloat = new GraphicsPath())
            {
                gpInt.AddLine(x1, y1, x2, y2);
                AssertLine(gpInt);

                gpFloat.AddLine((float)x1, (float)y1, (float)x2, (float)y2);
                AssertLine(gpFloat);

                gpPointsInt.AddLine(new Point(x1, y1), new Point(x2, y2));
                AssertLine(gpPointsInt);

                gpfPointsloat.AddLine(new PointF(x1, y1), new PointF(x2, y2));
                AssertLine(gpfPointsloat);
            }
        }

        [Theory]
        [InlineData(49, 157, 75, 196, 102, 209)]
        public void AddLine_SamePoints_Success(int x1, int y1, int x2, int y2, int x3, int y3)
        {

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddLine(new Point(x1, y1), new Point(x2, y2));
                gpi.AddLine(new Point(x2, y2), new Point(x3, y3));
                Assert.Equal(3, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(1, gpi.PathTypes[2]);

                gpi.AddLine(new Point(x3, y3), new Point(x2, y2));
                Assert.Equal(4, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(1, gpi.PathTypes[2]);
                Assert.Equal(1, gpi.PathTypes[3]);

                gpf.AddLine(new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2));
                gpf.AddLine(new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3));
                Assert.Equal(3, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(1, gpf.PathTypes[2]);

                gpf.AddLine(new PointF((float)x3, (float)y3), new PointF((float)x2, (float)y2));
                Assert.Equal(4, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(1, gpf.PathTypes[2]);
                Assert.Equal(1, gpf.PathTypes[3]);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddLines_Success(int x1, int y1, int x2, int y2)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddLines(new Point[] { new Point(x1, y1), new Point(x2, y2) });
                AssertLine(gpi);

                gpf.AddLines(new PointF[] { new PointF(x1, y1), new PointF(x2, y2) });
                AssertLine(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1)]
        public void AddLines_SinglePoint_Success(int x1, int y1)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddLines(new PointF[] { new PointF(x1, y1) });
                AssertLinesSinglePoint(gpi);

                gpf.AddLines(new PointF[] { new PointF(x1, y1) });
                AssertLinesSinglePoint(gpf);
            }
        }

        [Theory]
        [InlineData(49, 157)]
        public void AddLines_SamePoint_Success(int x1, int y1)
        {
            Point[] intPoints = new Point[]
            {
                new Point(x1, y1), new Point(x1, y1)
            };

            PointF[] floeatPoints = new PointF[]
            {
                new PointF((float)x1, (float)y1), new PointF((float)x1, (float)y1),
                new PointF((float)x1, (float)y1), new PointF((float)x1, (float)y1)
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddLines(intPoints);
                Assert.Equal(2, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);

                gpi.AddLines(intPoints);
                // 3 not 4 points, the first point (only) is compressed
                Assert.Equal(3, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(1, gpi.PathTypes[2]);

                gpi.AddLines(intPoints);
                // 4 not 5 (or 6) points, the first point (only) is compressed
                Assert.Equal(4, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(1, gpi.PathTypes[2]);
                Assert.Equal(1, gpi.PathTypes[3]);

                gpf.AddLines(floeatPoints);
                // all identical points are added
                Assert.Equal(4, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(1, gpf.PathTypes[2]);
                Assert.Equal(1, gpf.PathTypes[3]);

                gpf.AddLines(floeatPoints);
                // only the first new point is compressed
                Assert.Equal(7, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(1, gpf.PathTypes[2]);
                Assert.Equal(1, gpf.PathTypes[3]);
                Assert.Equal(1, gpf.PathTypes[4]);
                Assert.Equal(1, gpf.PathTypes[5]);
                Assert.Equal(1, gpf.PathTypes[6]);
            }
        }

        [Fact]
        public void AddLines_PointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddLines((Point[])null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddLines((PointF[])null));
        }

        [Fact]
        public void AddLines_ZeroPoints_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddLines(new Point[0]));
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddLines(new PointF[0]));
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddArc_Values_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddArc(x, y, width, height, Pi4, Pi4);
                AssertArc(gpi);

                gpf.AddArc((float)x, (float)y, (float)width, (float)height, Pi4, Pi4);
                AssertArc(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddArc_Rectangle_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddArc(new Rectangle(x, y, width, height), Pi4, Pi4);
                AssertArc(gpi);

                gpf.AddArc(new RectangleF(x, y, width, height), Pi4, Pi4);
                AssertArc(gpf);
            }
        }

        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 0, 1, 0)]
        [InlineData(0, 0, 0, 1)]
        public void AddArc_ZeroWidthHeight_ThrowsArgumentException(int x, int y, int width, int height)
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddArc(x, y, width, height, Pi4, Pi4));
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddArc((float)x, (float)y, (float)width, (float)height, Pi4, Pi4));
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3, 4, 4)]
        public void AddBezier_Points_Success(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddBezier(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4));
                AssertBezier(gpi);

                gpf.AddBezier(new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
                          new PointF((float)x3, (float)y3), new PointF((float)x4, (float)y4));
                AssertBezier(gpf);
            }
        }

        [Theory]
        [InlineData(0, 0, 0, 0, 0, 0, 0, 0)]
        public void AddBezier_SamePoints_Success(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddBezier(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4));

                // all points are present
                Assert.Equal(4, gp.PointCount);
                Assert.Equal(0, gp.PathTypes[0]);
                Assert.Equal(3, gp.PathTypes[1]);
                Assert.Equal(3, gp.PathTypes[2]);
                Assert.Equal(3, gp.PathTypes[3]);

                gp.AddBezier(new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x4, y4));

                // the first point (move to) can be compressed (i.e. removed)
                Assert.Equal(7, gp.PointCount);
                Assert.Equal(3, gp.PathTypes[4]);
                Assert.Equal(3, gp.PathTypes[5]);
                Assert.Equal(3, gp.PathTypes[6]);

                GraphicsPath gpf = new GraphicsPath();
                gpf.AddBezier(new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
                              new PointF((float)x3, (float)y3), new PointF((float)x4, (float)y4));

                // all points are present
                Assert.Equal(4, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(3, gpf.PathTypes[1]);
                Assert.Equal(3, gpf.PathTypes[2]);
                Assert.Equal(3, gpf.PathTypes[3]);

                gpf.AddBezier(new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
                              new PointF((float)x3, (float)y3), new PointF((float)x4, (float)y4));

                // the first point (move to) can be compressed (i.e. removed)
                Assert.Equal(7, gpf.PointCount);
                Assert.Equal(3, gpf.PathTypes[4]);
                Assert.Equal(3, gpf.PathTypes[5]);
                Assert.Equal(3, gpf.PathTypes[6]);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3, 4, 4)]
        public void AddBezier_Values_Success(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddBezier(x1, y1, x2, y2, x3, y3, x4, y4);
                AssertBezier(gpi);

                gpf.AddBezier((float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3, (float)x4, (float)y4);
                AssertBezier(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3, 4, 4)]
        public void AddBeziers_Points_Success(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            PointF[] points = new PointF[]
            {
                new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
                new PointF((float)x3, (float)y3), new PointF((float)x4, (float)y4)
            };

            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpf.AddBeziers(points);
                AssertBezier(gpf);
            }
        }

        [Fact]
        public void AddBeziers_PointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddBeziers((PointF[])null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddBeziers((Point[])null));
        }

        [Fact]
        public void AddBeziers_ThreePoints_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath()
                .AddBeziers(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) }));

            Assert.Throws<ArgumentException>(() => new GraphicsPath()
                .AddBeziers(new PointF[3] { new PointF(1f, 1f), new PointF(2f, 2f), new PointF(3f, 3f) }));
        }


        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddCurve_TwoPoints_Success(int x1, int y1, int x2, int y2)
        {
            Point[] intPoints = new Point[]
            {
                new Point(x1, y1), new Point(x2, y2),
            };

            PointF[] floatPoints = new PointF[]
            {
                new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpf.AddCurve(floatPoints);
                AssertCurve(gpf);

                gpi.AddCurve(intPoints);
                AssertCurve(gpi);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 0.5f)]
        public void AddCurve_TwoPointsWithTension_Success(int x1, int y1, int x2, int y2, float tension)
        {
            Point[] intPoints = new Point[]
            {
                new Point(x1, y1), new Point(x2, y2),
            };

            PointF[] floatPoints = new PointF[]
            {
                new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddCurve(intPoints, tension);
                AssertCurve(gpi);

                gpf.AddCurve(floatPoints, tension);
                AssertCurve(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        public void AddCurve_SamePoints_Success(int x1, int y1, int x2, int y2)
        {
            Point[] intPoints = new Point[]
            {
                new Point(x1, y1), new Point(x2, y2),
            };

            PointF[] floatPoints = new PointF[]
            {
                new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2),
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddCurve(intPoints);
                Assert.Equal(4, gpi.PointCount);
                gpi.AddCurve(intPoints);
                Assert.Equal(7, gpi.PointCount);

                gpf.AddCurve(floatPoints);
                Assert.Equal(4, gpf.PointCount);
                gpf.AddCurve(floatPoints);
                Assert.Equal(7, gpf.PointCount);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, float.MaxValue)]
        public void AddCurve_LargeTension_Success(int x1, int y1, int x2, int y2, float tension)
        {
            Point[] intPoints = new Point[]
            {
                new Point(x1, y1),
                new Point(x2, y2),
            };

            PointF[] floatPoints = new PointF[]
            {
                new PointF((float)x1, (float)y1),
                new PointF((float)x2, (float)y2),
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddCurve(intPoints, tension);
                Assert.Equal(4, gpi.PointCount);

                gpf.AddCurve(floatPoints, tension);
                Assert.Equal(4, gpf.PointCount);
            }
        }

        [Fact]
        public void AddCurve_Success()
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
                AssertEqual(37f, gp.PathData.Points[0].X, Delta);
                AssertEqual(185f, gp.PathData.Points[0].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[1]);
                AssertEqual(47.3334f, gp.PathData.Points[1].X, Delta);
                AssertEqual(185f, gp.PathData.Points[1].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[2]);
                AssertEqual(78.33333f, gp.PathData.Points[2].X, Delta);
                AssertEqual(189.3333f, gp.PathData.Points[2].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[3]);
                AssertEqual(99f, gp.PathData.Points[3].X, Delta);
                AssertEqual(185f, gp.PathData.Points[3].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[4]);
                AssertEqual(119.6667f, gp.PathData.Points[4].X, Delta);
                AssertEqual(180.6667f, gp.PathData.Points[4].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[5]);
                AssertEqual(140.3333f, gp.PathData.Points[5].X, Delta);
                AssertEqual(159f, gp.PathData.Points[5].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[6]);
                AssertEqual(161f, gp.PathData.Points[6].X, Delta);
                AssertEqual(159f, gp.PathData.Points[6].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[7]);
                AssertEqual(181.6667f, gp.PathData.Points[7].X, Delta);
                AssertEqual(159f, gp.PathData.Points[7].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[8]);
                AssertEqual(202.3333f, gp.PathData.Points[8].X, Delta);
                AssertEqual(202.5f, gp.PathData.Points[8].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[9]);
                AssertEqual(223f, gp.PathData.Points[9].X, Delta);
                AssertEqual(185f, gp.PathData.Points[9].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[10]);
                AssertEqual(243.6667f, gp.PathData.Points[10].X, Delta);
                AssertEqual(167.5f, gp.PathData.Points[10].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[11]);
                AssertEqual(274.6667f, gp.PathData.Points[11].X, Delta);
                AssertEqual(75.83334f, gp.PathData.Points[11].Y, Delta);
                Assert.Equal(3, gp.PathData.Types[12]);
                AssertEqual(285f, gp.PathData.Points[12].X, Delta);
                AssertEqual(54f, gp.PathData.Points[12].Y, Delta);
            }
        }

        [Fact]
        public void AddCurve_PointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddCurve((PointF[])null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddCurve((Point[])null));
        }

        [Fact]
        public void AddCurve_BadValues_ThrowsArgumentException()
        {
            // zero points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[0]));
            // one point
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[1] { new PointF(1f, 1f) }));
            // zero segment
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 0, 0, 0.5f));
            // negative segment
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 0, -1, 0.5f));
            // offset too large
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[3] { new PointF(1f, 1f), new PointF(0f, 20f), new PointF(20f, 0f) }, 1, 2, 0.5f));
            // adding only two points isn't supported by GdipAddCurve3
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }, 0, 2, 0.5f));

            // zero points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new Point[0]));
            // one point
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new Point[1] { new Point(1, 1) }));
            // zero segment
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new Point[2] { new Point(1, 1), new Point(2, 2) }, 0, 0, 0.5f));
            // negative segment
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new Point[2] { new Point(1, 1), new Point(2, 2) }, 0, -1, 0.5f));
            // offset too large
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new Point[3] { new Point(1, 1), new Point(0, 20), new Point(20, 0) }, 1, 2, 0.5f));
            // adding only two points isn't supported by GdipAddCurve3
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddCurve(
                new PointF[2] { new Point(1, 1), new Point(2, 2) }, 0, 2, 0.5f));

        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3)]
        public void AddClosedCurve_Points_Success(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddClosedCurve(
                new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                AssertClosedCurve(gpi);

                gpf.AddClosedCurve(
                    new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                AssertClosedCurve(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1, 1, 1)]
        public void AddClosedCurve_SamePoints_Success(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddClosedCurve(
                new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                Assert.Equal(10, gpi.PointCount);
                gpi.AddClosedCurve(
                    new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                Assert.Equal(20, gpi.PointCount);

                gpf.AddClosedCurve(
                    new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                Assert.Equal(10, gpf.PointCount);
                gpf.AddClosedCurve(
                    new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                Assert.Equal(20, gpf.PointCount);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3, 0.5f)]
        public void AddClosedCurve_Tension_Success(int x1, int y1, int x2, int y2, int x3, int y3, float tension)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddClosedCurve(
                new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) },
                tension);
                AssertClosedCurve(gpi);

                gpf.AddClosedCurve(
                    new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) },
                    tension);
                AssertClosedCurve(gpf);
            }
        }

        [Fact]
        public void AddClosedCurve_PointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddClosedCurve((PointF[])null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddClosedCurve((Point[])null));
        }

        [Fact]
        public void AddClosedCurve_LessThenThreePoints_ThrowsArgumentException()
        {
            // zero points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddClosedCurve(
                new PointF[0]));
            // one point
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddClosedCurve(
                new PointF[1] { new PointF(1f, 1f) }));
            // two points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddClosedCurve(
                new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }));

            // zero points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddClosedCurve(
                new Point[0]));
            // one point
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddClosedCurve(
                new Point[1] { new Point(1, 1) }));
            // two points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddClosedCurve(
                new Point[2] { new Point(1, 1), new Point(2, 2) }));
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddRectangle_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddRectangle(new Rectangle(x, y, width, height));
                AssertRectangle(gpi);

                gpf.AddRectangle(new RectangleF((float)x, (float)y, (float)width, (float)height));
                AssertRectangle(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        public void AddRectangle_SameRectangles_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddRectangle(new Rectangle(x, y, width, height));
                Assert.Equal(4, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(1, gpi.PathTypes[2]);
                Assert.Equal(129, gpi.PathTypes[3]);
                PointF endi = gpi.PathPoints[3];

                // add rectangle at the last path point
                gpi.AddRectangle(new Rectangle((int)endi.X, (int)endi.Y, width, height));
                // no compression (different type)
                Assert.Equal(8, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(1, gpi.PathTypes[2]);
                Assert.Equal(129, gpi.PathTypes[3]);

                gpf.AddRectangle(new RectangleF((float)x, (float)y, (float)width, (float)height));
                Assert.Equal(4, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(1, gpf.PathTypes[2]);
                Assert.Equal(129, gpf.PathTypes[3]);
                PointF endf = gpf.PathPoints[3];

                // add rectangle at the last path point
                gpf.AddRectangle(new RectangleF(endf.X, endf.Y, (float)width, (float)height));
                // no compression (different type)
                Assert.Equal(8, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(1, gpf.PathTypes[2]);
                Assert.Equal(129, gpf.PathTypes[3]);
            }
        }

        [Theory]
        [InlineData(1, 2, 0, 0)]
        [InlineData(1, 2, 3, 0)]
        [InlineData(1, 2, 0, 4)]
        public void AddRectangle_ZeroWidthHeight_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddRectangle(new Rectangle(x, y, width, height));
                Assert.Equal(0, gpi.PathData.Points.Length);

                gpf.AddRectangle(new RectangleF((float)x, (float)y, (float)width, (float)height));
                Assert.Equal(0, gpf.PathData.Points.Length);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3, 4, 4)]
        public void AddRectangles_Success(int x, int y, int width, int height, int x1, int y1, int width1, int height1)
        {
            Rectangle[] rectInt = new Rectangle[]
            {
                new Rectangle(x, y, width, height),
                new Rectangle(x1, y1, width1, height1)
            };

            RectangleF[] rectFloat = new RectangleF[]
            {
                new RectangleF((float)x, (float)y, (float)width, (float)height),
                new RectangleF((float)x1, (float)y1, (float)width1, (float)height1)
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddRectangles(rectInt);
                Assert.Equal(8, gpi.PathPoints.Length);
                Assert.Equal(8, gpi.PathTypes.Length);
                Assert.Equal(8, gpi.PathData.Points.Length);

                gpf.AddRectangles(rectFloat);
                Assert.Equal(8, gpf.PathPoints.Length);
                Assert.Equal(8, gpf.PathTypes.Length);
                Assert.Equal(8, gpf.PathData.Points.Length);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddRectangles_SamePoints_Success(int x, int y, int width, int height)
        {
            Rectangle[] rectInt = new Rectangle[]
            {
                new Rectangle(x, y, 0, 0),
                new Rectangle(x, y, width, height),
                new Rectangle(x, y, width, height)
            };

            RectangleF[] rectFloat = new RectangleF[]
            {
                new RectangleF((float)x, (float)y, 0f, 0f),
                new RectangleF((float)x, (float)y, (float)width, (float)height),
                new RectangleF((float)x, (float)y, (float)width, (float)height)
            };

            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddRectangles(rectInt);
                Assert.Equal(8, gpi.PathPoints.Length);
                Assert.Equal(8, gpi.PathTypes.Length);
                Assert.Equal(8, gpi.PathData.Points.Length);

                gpf.AddRectangles(rectFloat);
                Assert.Equal(8, gpf.PathPoints.Length);
                Assert.Equal(8, gpf.PathTypes.Length);
                Assert.Equal(8, gpf.PathData.Points.Length);
                // first Rectangle is ignored, then all other 2x4 (8) points are present, no compression
            }
        }

        [Fact]
        public void AddRectangles_RectangleNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddRectangles((RectangleF[])null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddRectangles((Rectangle[])null));
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddEllipse_Rectangl_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddEllipse(new Rectangle(x, y, width, height));
                AssertEllipse(gpi);

                gpf.AddEllipse(new RectangleF((float)x, (float)y, (float)width, (float)height));
                AssertEllipse(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddEllipse_Values_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddEllipse(x, y, width, height);
                AssertEllipse(gpi);

                gpf.AddEllipse((float)x, (float)y, (float)width, (float)height);
                AssertEllipse(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 0, 0)]
        [InlineData(1, 1, 2, 0)]
        [InlineData(1, 1, 0, 2)]
        public void AddEllipse_ZeroWidthHeight_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddEllipse(x, y, width, height);
                Assert.Equal(13, gpi.PathData.Points.Length);

                gpf.AddEllipse((float)x, (float)y, (float)width, (float)height);
                Assert.Equal(13, gpf.PathData.Points.Length);
            }
        }

        [Fact]
        public void AddPie_Rectangl_Success()
        {
            using (GraphicsPath gpi = new GraphicsPath())
            {
                gpi.AddPie(new Rectangle(1, 1, 2, 2), Pi4, Pi4);
                AssertPie(gpi);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, Pi4, Pi4)]
        public void AddPie_Values_Success(int x, int y, int width, int height, float startAngle, float sweepAngle)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddPie(x, y, height, width, Pi4, Pi4);
                AssertPie(gpi);

                gpf.AddPie((float)x, (float)y, height, width, Pi4, Pi4);
                AssertPie(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 0, 0, Pi4, Pi4)]
        [InlineData(1, 1, 2, 0, Pi4, Pi4)]
        [InlineData(1, 1, 0, 2, Pi4, Pi4)]
        public void AddPie_ZeroWidthHeight_ThrowsArgumentException(int x, int y, int width, int height, float startAngle, float sweepAngle)
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPie(x, y, height, width, Pi4, Pi4));
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPie((float)x, (float)y, height, width, Pi4, Pi4));
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPie(new Rectangle(x, y, height, width), Pi4, Pi4));
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3)]
        public void AddPolygon_Points_Success(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddPolygon(new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                AssertPolygon(gpi);

                gpf.AddPolygon(new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                AssertPolygon(gpf);
            }
        }

        [Theory]
        [InlineData(1, 1, 2, 2, 3, 3)]
        public void AddPolygon_SamePoints_Success(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                gpi.AddPolygon(new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                // all identical points are added
                Assert.Equal(3, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[0]);
                Assert.Equal(1, gpi.PathTypes[1]);
                Assert.Equal(129, gpi.PathTypes[2]);

                gpi.AddPolygon(new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                // all identical points are added (again)
                Assert.Equal(6, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[3]);
                Assert.Equal(1, gpi.PathTypes[4]);
                Assert.Equal(129, gpi.PathTypes[5]);

                gpi.AddPolygon(new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                // all identical points are added as a line (because previous point is closed)
                Assert.Equal(9, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[6]);
                Assert.Equal(1, gpi.PathTypes[7]);
                Assert.Equal(129, gpi.PathTypes[8]);

                gpi.AddPolygon(new Point[3] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
                // all identical points are added (again)
                Assert.Equal(12, gpi.PointCount);
                Assert.Equal(0, gpi.PathTypes[9]);
                Assert.Equal(1, gpi.PathTypes[10]);
                Assert.Equal(129, gpi.PathTypes[11]);

                gpf.AddPolygon(new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                // all identical points are added
                Assert.Equal(3, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[0]);
                Assert.Equal(1, gpf.PathTypes[1]);
                Assert.Equal(129, gpf.PathTypes[2]);

                gpf.AddPolygon(new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                // all identical points are added (again)
                Assert.Equal(6, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[3]);
                Assert.Equal(1, gpf.PathTypes[4]);
                Assert.Equal(129, gpf.PathTypes[5]);

                gpf.AddPolygon(new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                // all identical points are added as a line (because previous point is closed)
                Assert.Equal(9, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[6]);
                Assert.Equal(1, gpf.PathTypes[7]);
                Assert.Equal(129, gpf.PathTypes[8]);

                gpf.AddPolygon(new PointF[3] { new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2), new PointF((float)x3, (float)y3) });
                // all identical points are added (again)
                Assert.Equal(12, gpf.PointCount);
                Assert.Equal(0, gpf.PathTypes[9]);
                Assert.Equal(1, gpf.PathTypes[10]);
                Assert.Equal(129, gpf.PathTypes[11]);
            }
        }

        [Fact]
        public void AddPolygon_PointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddPolygon((Point[])null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddPolygon((PointF[])null));
        }

        [Fact]
        public void AddPolygon_LessThenThreePoints_ThrowsArgumentException()
        {
            // zero points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(
                new PointF[0]));
            // one point
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(
                new PointF[1] { new PointF(1f, 1f) }));
            // two points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(
                new PointF[2] { new PointF(1f, 1f), new PointF(2f, 2f) }));

            // zero points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(
                new Point[0]));
            // one point
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(
                new Point[1] { new Point(1, 1) }));
            // two points
            Assert.Throws<ArgumentException>(() => new GraphicsPath().AddPolygon(
                new Point[2] { new Point(1, 1), new Point(2, 2) }));
        }

        [Theory]
        [InlineData(1, 1, 2, 2)]
        public void AddPath_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath inner = new GraphicsPath())
            using (GraphicsPath gp = new GraphicsPath())
            {
                inner.AddRectangle(new Rectangle(x, y, width, height));
                gp.AddPath(inner, true);
                AssertRectangle(gp);
            }
        }

        [Fact]
        public void AddPath_PathNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().AddPath(null, false));
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        [InlineData(10, 10)]
        public void AddString_Point_Success(int x, int y)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                FontFamily ff = GetFontFamily();
                gpi.AddString("mono", ff, 0, 10, new Point(x, y), StringFormat.GenericDefault);
                Assert.True(gpi.PointCount > 0);

                gpf.AddString("mono", ff, 0, 10, new PointF((float)x, (float)y), StringFormat.GenericDefault);
                Assert.True(gpf.PointCount > 0);
            }
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        [InlineData(10, 10, 10, 10)]
        public void AddString_Rectangle_Success(int x, int y, int width, int height)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                FontFamily ff = GetFontFamily();
                gpi.AddString("mono", ff, 0, 10, new Rectangle(x, y, width, height), StringFormat.GenericDefault);
                Assert.True(gpi.PointCount > 0);

                gpf.AddString("mono", ff, 0, 10, new RectangleF((float)x, (float)y, (float)width, (float)height), StringFormat.GenericDefault);
                Assert.True(gpf.PointCount > 0);
            }
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        [InlineData(-10)]
        public void AddString_NegativeSize_Success(int size)
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                FontFamily ff = GetFontFamily();
                gpi.AddString("mono", ff, 0, size, new Point(10, 10), StringFormat.GenericDefault);
                Assert.True(gpi.PointCount > 0);

                int gpiLenghtOld = gpi.PathPoints.Length;
                gpi.AddString("mono", ff, 0, size, new Rectangle(10, 10, 10, 10), StringFormat.GenericDefault);
                Assert.True(gpi.PointCount > gpiLenghtOld);

                gpf.AddString("mono", ff, 0, size, new PointF(10f, 10f), StringFormat.GenericDefault);
                Assert.True(gpf.PointCount > 0);

                int pgfLenghtOld = gpf.PathPoints.Length;
                gpf.AddString("mono", ff, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault);
                Assert.True(gpf.PointCount > pgfLenghtOld);
            }
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        public void AddString_StringFormat_Success()
        {
            using (GraphicsPath gp1 = new GraphicsPath())
            using (GraphicsPath gp2 = new GraphicsPath())
            using (GraphicsPath gp3 = new GraphicsPath())
            {
                FontFamily ff = GetFontFamily();
                gp1.AddString("mono", ff, 0, 10, new RectangleF(10f, 10f, 10f, 10f), null);
                Assert.True(gp1.PointCount > 0);

                // StringFormat.GenericDefault
                gp2.AddString("mono", ff, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault);
                Assert.Equal(gp1.PointCount, gp2.PointCount);

                // StringFormat.GenericTypographic
                gp3.AddString("mono", ff, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericTypographic);
                Assert.False(gp1.PointCount == gp3.PointCount);
            }
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        public void AddString_EmptyString_Success()
        {
            using (GraphicsPath gpi = new GraphicsPath())
            using (GraphicsPath gpf = new GraphicsPath())
            {
                FontFamily ff = GetFontFamily();
                gpi.AddString(string.Empty, ff, 0, 10, new Point(10, 10), StringFormat.GenericDefault);
                Assert.Equal(0, gpi.PointCount);

                gpi.AddString(string.Empty, ff, 0, 10, new PointF(10f, 10f), StringFormat.GenericDefault);
                Assert.Equal(0, gpf.PointCount);
            }
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        public void AddString_StringNull_ThrowsNullReferenceException()
        {
            FontFamily ff = GetFontFamily();
            Assert.Throws<NullReferenceException>(() =>
                new GraphicsPath().AddString(null, ff, 0, 10, new Point(10, 10), StringFormat.GenericDefault));
            Assert.Throws<NullReferenceException>(() =>
                new GraphicsPath().AddString(null, ff, 0, 10, new PointF(10f, 10f), StringFormat.GenericDefault));
            Assert.Throws<NullReferenceException>(() =>
                new GraphicsPath().AddString(null, ff, 0, 10, new Rectangle(10, 10, 10, 10), StringFormat.GenericDefault));
            Assert.Throws<NullReferenceException>(() =>
                new GraphicsPath().AddString(null, ff, 0, 10, new RectangleF(10f, 10f, 10f, 10f), StringFormat.GenericDefault));
        }

        [Fact]
        public void AddString_FontFamilyNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new GraphicsPath().AddString("mono", null, 0, 10, new Point(10, 10), StringFormat.GenericDefault));
        }

        [Fact]
        public void Transform_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix(1f, 1f, 2f, 2f, 3f, 3f))
            {
                gp.AddRectangle(new Rectangle(1, 1, 2, 2));
                AssertRectangle(gp);
                gp.Transform(matrix);
                Assert.Equal(new float[] { 1f, 1f, 2f, 2f, 3f, 3f }, matrix.Elements);

                RectangleF rect = gp.GetBounds();
                AssertEqual(6f, rect.X, Delta);
                AssertEqual(6f, rect.Y, Delta);
                AssertEqual(6f, rect.Width, Delta);
                AssertEqual(6f, rect.Height, Delta);

                AssertEqual(6f, gp.PathData.Points[0].X, Delta);
                AssertEqual(6f, gp.PathData.Points[0].Y, Delta);
                Assert.Equal(0, gp.PathData.Types[0]);
                AssertEqual(8f, gp.PathData.Points[1].X, Delta);
                AssertEqual(8f, gp.PathData.Points[1].Y, Delta);
                Assert.Equal(1, gp.PathTypes[1]);
                AssertEqual(12f, gp.PathData.Points[2].X, Delta);
                AssertEqual(12f, gp.PathData.Points[2].Y, Delta);
                Assert.Equal(1, gp.PathData.Types[2]);
                AssertEqual(10f, gp.PathData.Points[3].X, Delta);
                AssertEqual(10f, gp.PathData.Points[3].Y, Delta);
                Assert.Equal(129, gp.PathTypes[3]);
            }
        }

        [Fact]
        public void Transform_PathEmpty_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix(1f, 1f, 2f, 2f, 3f, 3f))
            {
                gp.Transform(matrix);
                Assert.Equal(new float[] { 1f, 1f, 2f, 2f, 3f, 3f }, matrix.Elements);
                AssertEmptyGrahicsPath(gp);
            }
        }

        [Fact]
        public void Transform_MatrixNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Transform(null));
        }

        [Fact]
        public void GetBounds_PathEmpty_ReturnsExpected()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                AssertRectangleBounds(gp.GetBounds(), 0f, 0f, 0f, 0f);
            }
        }

        private static IEnumerable<object[]> GetBounds_TestData()
        {
            yield return new object[] { new RectangleF(1f, 1f, 2f, 2f) };
        }

        [Theory]
        [MemberData(nameof(GetBounds_TestData))]
        public void GetBounds_Rectangle_ReturnsExpected(RectangleF testRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddRectangle(testRectangle);
                AssertRectangleBounds(gp.GetBounds(), testRectangle.X, testRectangle.Y, testRectangle.Width, testRectangle.Height);
                AssertRectangleBounds(gp.GetBounds(null), testRectangle.X, testRectangle.Y, testRectangle.Width, testRectangle.Height);
                AssertRectangleBounds(gp.GetBounds(matrix), testRectangle.X, testRectangle.Y, testRectangle.Width, testRectangle.Height);
                AssertRectangleBounds(gp.GetBounds(null, null), testRectangle.X, testRectangle.Y, testRectangle.Width, testRectangle.Height);
            }
        }

        [Theory(Skip = "Can't/won't duplicate the lack of precision.")]
        [MemberData(nameof(GetBounds_TestData))]
        public void GetBounds_Rectangle_WithPen_ReturnsExpected(RectangleF testRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            using (Pen pen = new Pen(Color.Aqua, 0))
            {
                gp.AddRectangle(testRectangle);
                // those bounds doesn't make any sense (even visually)
                // probably null gets mis-interpreted ???
                RectangleF bounds = gp.GetBounds(null, pen);
                AssertEqual(-6.09999943f, bounds.X, Delta);
                AssertEqual(-6.09999943f, bounds.Y, Delta);
                AssertEqual(16.1999989f, bounds.Width, Delta);
                AssertEqual(16.1999989f, bounds.Height, Delta);

                bounds = gp.GetBounds(matrix, pen);
                AssertEqual(-0.42f, bounds.X, Delta);
                AssertEqual(-0.42f, bounds.Y, Delta);
                AssertEqual(4.84f, bounds.Width, Delta);
                AssertEqual(4.84f, bounds.Height, Delta);

                gp.Widen(pen);
                bounds = gp.GetBounds();
                AssertEqual(0.499999523f, bounds.X, Delta);
                AssertEqual(0.499999523f, bounds.Y, Delta);
                AssertEqual(3.000001f, bounds.Width, Delta);
                AssertEqual(3.000001f, bounds.Height, Delta);

                bounds = gp.GetBounds(matrix);
                AssertEqual(0.499999523f, bounds.X, Delta);
                AssertEqual(0.499999523f, bounds.Y, Delta);
                AssertEqual(3.000001f, bounds.Width, Delta);
                AssertEqual(3.000001f, bounds.Height, Delta);
            }
        }

        private static IEnumerable<object[]> GetBounds_Pie_TestData()
        {
            yield return new object[] { new Rectangle(10, 10, 100, 100), 30, 45, 60f, 60f, 43.3f, 48.3f };
        }

        [Theory]
        [MemberData(nameof(GetBounds_Pie_TestData))]
        public void GetBounds_Pie_ReturnsExpected(
            Rectangle testRectangle, int startAngle, int sweepAngle, float expectedX, float expectedY, float expectedWidth, float expectedHeight)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddPie(testRectangle, startAngle, sweepAngle);
                AssertPieBounds(gp.GetBounds(), expectedX, expectedY, expectedWidth, expectedHeight);
                AssertPieBounds(gp.GetBounds(null), expectedX, expectedY, expectedWidth, expectedHeight);
                AssertPieBounds(gp.GetBounds(matrix), expectedX, expectedY, expectedWidth, expectedHeight);
                AssertPieBounds(gp.GetBounds(null, null), expectedX, expectedY, expectedWidth, expectedHeight);
            }
        }

        [Fact]
        public void Flatten_Empty_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.Flatten();
                AssertPaths(gp, clone);
            }
        }

        [Fact]
        public void Flatten_MatrixNull_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.Flatten(null);
                AssertPaths(gp, clone);
            }
        }

        [Fact]
        public void Flatten_MatrixNullFloat_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.Flatten(null, 1f);
                AssertPaths(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Arc_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddArc(0f, 0f, 100f, 100f, 30, 30);
                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Bezier_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddBezier(0, 0, 100, 100, 30, 30, 60, 60);
                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_ClosedCurve_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddClosedCurve(new Point[4]
                {
                    new Point (0, 0), new Point (40, 20),
                    new Point (20, 40), new Point (40, 40)
                });

                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Curve_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddCurve(new Point[4]
                {
                    new Point (0, 0), new Point (40, 20),
                    new Point (20, 40), new Point (40, 40)
                });

                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Ellipse_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddEllipse(10f, 10f, 100f, 100f);
                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Line_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddLine(10f, 10f, 100f, 100f);
                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Pie_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddPie(0, 0, 100, 100, 30, 30);
                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Polygon_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddPolygon(new Point[4]
                {
                    new Point (0, 0), new Point (10, 10),
                    new Point (20, 20), new Point (40, 40)
                });

                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Flatten_Rectangle_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath clone = Assert.IsType<GraphicsPath>(gp.Clone()))
            {
                gp.AddRectangle(new Rectangle(0, 0, 100, 100));
                gp.Flatten();
                AssertFlats(gp, clone);
            }
        }

        [Fact]
        public void Warp_DestinationPointsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Warp(null, new RectangleF()));
        }

        [Fact]
        public void Warp_DestinationPointsZero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GraphicsPath().Warp(new PointF[0], new RectangleF()));
        }

        private static IEnumerable<object[]> Warp_TestData()
        {
            yield return new object[]
            {
                new PointF[1] { new PointF(0, 0) },
                new Point[3] { new Point(5, 5), new Point(15, 5), new Point(10, 15) },
                new RectangleF(10, 20, 30, 40)
            };
        }

        [Theory]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_PathEmpty_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                Assert.Equal(0, gp.PointCount);
                gp.Warp(destPoints, srcRectangle, matrix);
                Assert.Equal(0, gp.PointCount);
            }
        }

        [Theory]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_WarpModeInvalid_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix, (WarpMode)int.MinValue);
                Assert.Equal(0, gp.PointCount);
            }
        }

        [Theory(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_WarpModePerspective_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix, WarpMode.Perspective);
                Assert.True(0 < gp.PointCount);
                AssertWrap(gp, 0f, 0f, 0f, 0f, 0f, 0f);
            }
        }

        [Theory(Skip = "The last point is no more closed.")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_WarpModeBilinear_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix, WarpMode.Bilinear);
                Assert.True(0 < gp.PointCount);
                // Note that the last point is no more closed!
                AssertWrapNaN(gp, false);
            }
        }

        [Theory(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_MatrixNonInvertible_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix(123, 24, 82, 16, 47, 30))
            {
                Assert.False(matrix.IsInvertible);
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix);
                AssertWrap(gp, 47f, 30f, 47f, 30f, 47f, 30f);
            }
        }

        [Theory(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_MatrixNull_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, null);
                AssertWrap(gp, 0f, 0f, 0f, 0f, 0f, 0f);
            }
        }

        [Theory(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_MatrixEmpty_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                Assert.False(matrix.IsInvertible);
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix);
                AssertWrap(gp, 47f, 30f, 47f, 30f, 47f, 30f);
            }
        }

        [Theory(Skip = "Results aren't always constant.")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_RectangleNegativeWidthHeight_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, new Rectangle(10, 20, -30, -40), null);
                AssertWrap(gp, 1.131355e-39f, -2.0240637E-33f, 1.070131E-39f, -2.02406389E-33f, 3.669146E-40f, -6.746879E-34f);
            }
        }

        [Theory]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_RectangleEmpty_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, new Rectangle(), null);
                AssertWrapNaN(gp, true);
            }
        }

        [Theory(Skip = "Results aren't always constant.")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_Line_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddLine(points[0], points[1]);
                Assert.Equal(2, gp.PointCount);
                gp.Warp(destPoints, srcRectangle, matrix);
                Assert.Equal(2, gp.PointCount);
                AssertWrap(gp, 0f, 0f, 0f, 0f, 0f, 0f);
            }
        }

        [Theory(Skip = "Not Working")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_LinesSinglePoint_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddLines(new Point[1] { points[0] });
                // Special case - a line with a single point is valid
                Assert.Equal(1, gp.PointCount);
                gp.Warp(destPoints, srcRectangle, matrix);
                Assert.Equal(0, gp.PointCount);
            }
        }

        [Theory(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_FlatnessNegative_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix, WarpMode.Perspective, -1f);
                AssertWrap(gp, 0f, 0f, 0f, 0f, 0f, 0f);
            }
        }

        [Theory(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        [MemberData(nameof(Warp_TestData))]
        public void Warp_FlatnessOverOne_Success(PointF[] destPoints, Point[] points, RectangleF srcRectangle)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Matrix matrix = new Matrix())
            {
                gp.AddPolygon(points);
                gp.Warp(destPoints, srcRectangle, matrix, WarpMode.Perspective, 2.0f);
                AssertWrap(gp, 0f, 0f, 0f, 0f, 0f, 0f);
            }
        }

        [Fact]
        public void SetMarkers_EmptyPath_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.SetMarkers();
            }
        }

        [Fact]
        public void SetMarkers_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(new Point(1, 1), new Point(2, 2));
                Assert.Equal(1, gp.PathTypes[1]);
                gp.SetMarkers();
                Assert.Equal(33, gp.PathTypes[1]);
            }
        }

        [Fact]
        public void ClearMarkers_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(new Point(1, 1), new Point(2, 2));
                Assert.Equal(1, gp.PathTypes[1]);
                gp.SetMarkers();
                Assert.Equal(33, gp.PathTypes[1]);
                gp.ClearMarkers();
                Assert.Equal(1, gp.PathTypes[1]);
            }
        }

        [Fact]
        public void ClearMarkers_EmptyPath_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.ClearMarkers();
            }
        }

        [Fact]
        public void CloseFigure_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(new Point(1, 1), new Point(2, 2));
                Assert.Equal(1, gp.PathTypes[1]);
                gp.CloseFigure();
                Assert.Equal(129, gp.PathTypes[1]);
            }
        }

        [Fact]
        public void CloseFigure_EmptyPath_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.CloseFigure();
            }
        }

        [Fact]
        public void CloseAllFigures_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(new Point(1, 1), new Point(2, 2));
                gp.StartFigure();
                gp.AddLine(new Point(3, 3), new Point(4, 4));
                Assert.Equal(1, gp.PathTypes[1]);
                Assert.Equal(1, gp.PathTypes[3]);
                gp.CloseAllFigures();
                Assert.Equal(129, gp.PathTypes[1]);
                Assert.Equal(129, gp.PathTypes[3]);
            }
        }

        [Fact]
        public void CloseAllFigures_EmptyPath_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.CloseAllFigures();
            }
        }

        [Fact]
        public void StartClose_AddArc()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddArc(10, 10, 100, 100, 90, 180);
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(1, types[2]);
                // check last types
                Assert.Equal(3, types[gp.PointCount - 3]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddBezier()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddBezier(10, 10, 100, 100, 20, 20, 200, 200);
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(1, types[2]);
                // check last types
                Assert.Equal(3, types[gp.PointCount - 3]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddBeziers()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddBeziers(new Point[7]
                {
                    new Point (10, 10), new Point (20, 10), new Point (20, 20),
                    new Point (30, 20), new Point (40, 40), new Point (50, 40),
                    new Point (50, 50)
                });

                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(1, types[2]);
                // check last types
                Assert.Equal(3, types[gp.PointCount - 3]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddClosedCurve()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddClosedCurve(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(0, types[2]);
                // check last types
                Assert.Equal(131, types[gp.PointCount - 3]);
                Assert.Equal(0, types[gp.PointCount - 2]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
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

        [Fact]
        public void StartClose_AddEllipse()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddEllipse(10, 10, 100, 100);
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(0, types[2]);
                // check last types
                Assert.Equal(131, types[gp.PointCount - 3]);
                Assert.Equal(0, types[gp.PointCount - 2]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
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

        [Fact]
        public void StartClose_AddLines()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddLines(new Point[4] { new Point(10, 10), new Point(20, 10), new Point(20, 20), new Point(30, 20) });
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(1, types[2]);
                // check last types
                Assert.Equal(1, types[gp.PointCount - 3]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddPath_Connect()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (GraphicsPath inner = new GraphicsPath())
            {
                inner.AddArc(10, 10, 100, 100, 90, 180);
                gp.AddLine(1, 1, 2, 2);
                gp.AddPath(inner, true);
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(1, types[2]);
                // check last types
                Assert.Equal(3, types[gp.PointCount - 3]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void StartClose_AddPolygon()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddPolygon(new Point[3] { new Point(1, 1), new Point(2, 2), new Point(3, 3) });
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(0, types[2]);
                // check last types
                Assert.Equal(129, types[gp.PointCount - 3]);
                Assert.Equal(0, types[gp.PointCount - 2]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddRectangle()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddRectangle(new RectangleF(10, 10, 20, 20));
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(0, types[2]);
                // check last types
                Assert.Equal(129, types[gp.PointCount - 3]);
                Assert.Equal(0, types[gp.PointCount - 2]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddRectangles()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddRectangles(new RectangleF[2]
                {
                new RectangleF (10, 10, 20, 20),
                new RectangleF (20, 20, 10, 10)
                });

                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(0, types[2]);
                // check last types
                Assert.Equal(129, types[gp.PointCount - 3]);
                Assert.Equal(0, types[gp.PointCount - 2]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        public void StartClose_AddString()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 1, 2, 2);
                gp.AddString("mono", FontFamily.GenericMonospace, 0, 10, new Point(20, 20), StringFormat.GenericDefault);
                gp.AddLine(10, 10, 20, 20);
                byte[] types = gp.PathTypes;
                // check first types
                Assert.Equal(0, types[0]);
                Assert.Equal(0, types[2]);
                // check last types
                Assert.Equal(163, types[gp.PointCount - 3]);
                Assert.Equal(1, types[gp.PointCount - 2]);
                Assert.Equal(1, types[gp.PointCount - 1]);
            }
        }

        [Fact]
        //[Fact(Skip = "Not Working.")]
        public void Widen_Pen_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Blue))
            {
                gp.AddRectangle(new Rectangle(1, 1, 2, 2));
                Assert.Equal(4, gp.PointCount);
                gp.Widen(pen);
                Assert.Equal(12, gp.PointCount);
                AssertEqual(0.5f, gp.PathPoints[0].X, Delta);
                AssertEqual(0.5f, gp.PathPoints[0].Y, Delta);
                AssertEqual(3.5f, gp.PathPoints[1].X, Delta);
                AssertEqual(0.5f, gp.PathPoints[1].Y, Delta);
                AssertEqual(3.5f, gp.PathPoints[2].X, Delta);
                AssertEqual(3.5f, gp.PathPoints[2].Y, Delta);
                AssertEqual(0.5f, gp.PathPoints[3].X, Delta);
                AssertEqual(3.5f, gp.PathPoints[3].Y, Delta);
                AssertEqual(1.5f, gp.PathPoints[4].X, Delta);
                AssertEqual(3.0f, gp.PathPoints[4].Y, Delta);
                AssertEqual(1.0f, gp.PathPoints[5].X, Delta);
                AssertEqual(2.5f, gp.PathPoints[5].Y, Delta);
                AssertEqual(3.0f, gp.PathPoints[6].X, Delta);
                AssertEqual(2.5f, gp.PathPoints[6].Y, Delta);
                AssertEqual(2.5f, gp.PathPoints[7].X, Delta);
                AssertEqual(3.0f, gp.PathPoints[7].Y, Delta);
                AssertEqual(2.5f, gp.PathPoints[8].X, Delta);
                AssertEqual(1.0f, gp.PathPoints[8].Y, Delta);
                AssertEqual(3.0f, gp.PathPoints[9].X, Delta);
                AssertEqual(1.5f, gp.PathPoints[9].Y, Delta);
                AssertEqual(1.0f, gp.PathPoints[10].X, Delta);
                AssertEqual(1.5f, gp.PathPoints[10].Y, Delta);
                AssertEqual(1.5f, gp.PathPoints[11].X, Delta);
                AssertEqual(1.0f, gp.PathPoints[11].Y, Delta);

                Assert.Equal(0, gp.PathTypes[0]);
                Assert.Equal(1, gp.PathTypes[1]);
                Assert.Equal(1, gp.PathTypes[2]);
                Assert.Equal(129, gp.PathTypes[3]);
                Assert.Equal(0, gp.PathTypes[4]);
                Assert.Equal(1, gp.PathTypes[5]);
                Assert.Equal(1, gp.PathTypes[6]);
                Assert.Equal(1, gp.PathTypes[7]);
                Assert.Equal(1, gp.PathTypes[8]);
                Assert.Equal(1, gp.PathTypes[9]);
                Assert.Equal(1, gp.PathTypes[10]);
                Assert.Equal(129, gp.PathTypes[11]);
            }
        }

        [Fact]
        public void Widen_EmptyPath_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Blue))
            {
                Assert.Equal(0, gp.PointCount);
                gp.Widen(pen);
                Assert.Equal(0, gp.PointCount);
            }
        }

        [Fact(Skip = "Not Working. Throws OutOfMemoryException.")]
        public void Widen_SinglePoint_Success()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(new Point[1] { new Point(1, 1) });
                // Special case - a line with a single point is valid
                Assert.Equal(1, gp.PointCount);
                Assert.Throws<OutOfMemoryException>(() => gp.Widen(Pens.Red));
                // oops ;-)
            }
        }

        [Fact]
        public void Widen_PenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Widen(null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Widen(null, new Matrix()));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().Widen(null, new Matrix(), 0.67f));
        }

        private static IEnumerable<object[]> Widen_TestData()
        {
            yield return new object[] { new Point[3] { new Point(5, 5), new Point(15, 5), new Point(10, 15) } };
        }

        [Theory]
        //[Theory(Skip = "Not Working.")]
        [MemberData(nameof(Widen_TestData))]
        public void Widen_MatrixNull_Succses(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Blue))
            {
                gp.AddPolygon(points);
                gp.Widen(pen, null);
                Assert.Equal(9, gp.PointCount);
                AssertWiden3(gp);
            }
        }

        [Theory]
        //[Theory(Skip = "Not Working.")]
        [MemberData(nameof(Widen_TestData))]
        public void Widen_MatrixEmpty_Succses(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Blue))
            using (Matrix matrix = new Matrix())
            {
                gp.AddPolygon(points);
                gp.Widen(pen, new Matrix());
                Assert.Equal(9, gp.PointCount);
                AssertWiden3(gp);
            }

        }

        [Fact(Skip = "Results aren't always constant and differs from 1.x and 2.0")]
        public void Widen_MatrixNonInvertible_Succses()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Blue))
            using (Matrix matrix = new Matrix(123, 24, 82, 16, 47, 30))
            {
                Assert.False(matrix.IsInvertible);
                gp.Widen(pen, matrix);
                Assert.Equal(0, gp.PointCount);
            }
        }

        private static IEnumerable<object[]> Widen_PenSmallWidth_TestData()
        {
            yield return new object[] { new Rectangle(1, 1, 2, 2), 0f, 0.5f, 0.5f, 3.0f, 3.0f };
            yield return new object[] { new Rectangle(1, 1, 2, 2), 0.5f, 0.5f, 0.5f, 3.0f, 3.0f };
            yield return new object[] { new Rectangle(1, 1, 2, 2), 1.0f, 0.5f, 0.5f, 3.0f, 3.0f };
            yield return new object[] { new Rectangle(1, 1, 2, 2), 1.1f, 0.45f, 0.45f, 3.10f, 3.10f };
        }

        [Theory]
        //[Theory(Skip = "Not Working.")]
        [MemberData(nameof(Widen_PenSmallWidth_TestData))]
        public void Widen_Pen_SmallWidth_Succes(
            Rectangle rectangle, float penWidth, float expectedX, float expectedY, float expectedWidth, float expectedHeight)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Aqua, 0))
            using (Matrix matrix = new Matrix())
            {
                // pen's smaller than 1.0 (width) are "promoted" to 1
                pen.Width = penWidth;
                gp.AddRectangle(rectangle);
                gp.Widen(pen);
                AssertWidenedBounds(gp, null, expectedX, expectedY, expectedWidth, expectedHeight);
                AssertWidenedBounds(gp, matrix, expectedX, expectedY, expectedWidth, expectedHeight);
            }
        }

        [Fact]
        public void IsOutlineVisible_PenNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(1, 1, null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(1.0f, 1.0f, null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(new Point(), null));
            Assert.Throws<ArgumentNullException>(() => new GraphicsPath().IsOutlineVisible(new PointF(), null));
        }

        [Fact]
        public void IsOutlineVisible_LineWithoutGraphics_ReturnsExpected()
        {
            AssertIsOutlineVisibleLine(null);
        }

        [Fact]
        public void IsOutlineVisible_LineInsideGraphics_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                AssertIsOutlineVisibleLine(graphics);
            }
        }

        [Fact]
        public void IsOutlineVisible_LineOutsideGraphics_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(5, 5))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                AssertIsOutlineVisibleLine(graphics);
                // Graphics "seems" ignored as the line is outside the bitmap!
            }
        }

        // Docs ways the point is in world coordinates and that the graphics transform 
        // should be applied.

        [Fact]
        public void IsOutlineVisible_LineWithGraphicsTransform_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Matrix matrix = new Matrix(2, 0, 0, 2, 50, -50))
            {
                graphics.Transform = matrix;
                AssertIsOutlineVisibleLine(graphics);
                // Graphics still "seems" ignored (Transform).
            }
        }

        [Fact]
        public void IsOutlineVisible_LineWithGraphicsPageUnit_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.PageUnit = GraphicsUnit.Millimeter;
                AssertIsOutlineVisibleLine(graphics);
                // Graphics still "seems" ignored (PageUnit).
            }
        }

        [Fact]
        public void IsOutlineVisible_LineWithGraphicsPageScale_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(20, 20))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.PageScale = 2.0f;
                AssertIsOutlineVisibleLine(graphics);
                // Graphics still "seems" ignored (PageScale).
            }
        }

        [Fact(Skip = "Not Working.")]
        public void IsOutlineVisible_LineWithGraphicsTransformPageUnitPageScale_ReturnsExpected()
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Bitmap bitmap = new Bitmap(20, 20))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Matrix matrix = new Matrix(2, 0, 0, 2, 50, -50))
            {
                graphics.Transform = matrix;
                graphics.PageUnit = GraphicsUnit.Millimeter;
                graphics.PageScale = 2.0f;
                gp.AddLine(10, 1, 14, 1);
                Assert.False(gp.IsOutlineVisible(10, 1, Pens.Red, graphics));
                // Graphics ISN'T ignored (Transform + PageUnit + PageScale).
            }
        }

        // Looks buggy - reported to MS as FDBK50868
        [Fact(Skip = "Not Working.")]
        public void IsOutlineVisible_LineEnd_ReturnsExpected()
        {
            // Horizontal line.
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(10, 1, 14, 1);
                Assert.False(gp.IsOutlineVisible(14, 1, Pens.Red, null));
                Assert.False(gp.IsOutlineVisible(13.5f, 1.0f, Pens.Red, null));
                Assert.True(gp.IsOutlineVisible(13.4f, 1.0f, Pens.Red, null));
                Assert.False(gp.IsOutlineVisible(new Point(14, 1), Pens.Red, null));
                Assert.False(gp.IsOutlineVisible(new PointF(13.5f, 1.0f), Pens.Red, null));
                Assert.True(gp.IsOutlineVisible(new PointF(13.49f, 1.0f), Pens.Red, null));
            }

            // Vertical line.
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 10, 1, 14);
                Assert.False(gp.IsOutlineVisible(1, 14, Pens.Red, null));
                Assert.False(gp.IsOutlineVisible(1.0f, 13.5f, Pens.Red, null));
                Assert.True(gp.IsOutlineVisible(1.0f, 13.4f, Pens.Red, null));
                Assert.False(gp.IsOutlineVisible(new Point(1, 14), Pens.Red, null));
                Assert.False(gp.IsOutlineVisible(new PointF(1.0f, 13.5f), Pens.Red, null));
                Assert.True(gp.IsOutlineVisible(new PointF(1.0f, 13.49f), Pens.Red, null));
            }
        }

        [Fact]
        public void IsOutlineVisible_RectangleWithoutGraphics_ReturnsExpected()
        {
            AssertIsOutlineVisibleRectangle(null);
        }

        [Fact]
        public void IsVisible_RectangleWithoutGraphics_ReturnsExpected()
        {
            AssertIsVisibleRectangle(null);
        }

        [Fact]
        public void IsVisible_RectangleWithGraphics_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(40, 40))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                AssertIsVisibleRectangle(graphics);
            }
        }

        [Fact]
        public void IsVisible_EllipseWithoutGraphics_ReturnsExpected()
        {
            AssertIsVisibleEllipse(null);
        }

        [Fact]
        public void IsVisible_EllipseWithGraphics_ReturnsExpected()
        {
            using (Bitmap bitmap = new Bitmap(40, 40))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                AssertIsVisibleEllipse(graphics);
            }
        }        

        [Fact]
        public void Reverse_Arc_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        public void Reverse_Bezier_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddBezier(1, 2, 3, 4, 5, 6, 7, 8);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        private static IEnumerable<object[]> Reverse_TestData()
        {
            yield return new object[] 
            {
                new Point[]
                {
                    new Point (1,2), new Point (3,4), new Point (5,6), new Point (7,8),
                    new Point (9,10), new Point (11,12), new Point (13,14)
                }
            };
        }

        [Theory]
        [MemberData(nameof(Reverse_TestData))]
        public void Reverse_Beziers_Succes(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddBeziers(points);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Theory]
        [MemberData(nameof(Reverse_TestData))]
        public void Reverse_ClosedCurve_Succes(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddClosedCurve(points);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Theory]
        [MemberData(nameof(Reverse_TestData))]
        public void Reverse_Curve_Succes(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddCurve(points);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        public void Reverse_Ellipse_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(1, 2, 3, 4);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        public void Reverse_Line_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 2, 3, 4);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        public void Reverse_LineClosed_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(1, 2, 3, 4);
                gp.CloseFigure();
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Theory]
        [MemberData(nameof(Reverse_TestData))]
        public void Reverse_Lines_Succes(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLines(points);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Theory]
        [MemberData(nameof(Reverse_TestData))]
        public void Reverse_Polygon_Succes(Point[] points)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddPolygon(points);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        public void Reverse_Rectangle_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(1, 2, 3, 4));
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        public void Reverse_Rectangles_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                Rectangle[] rects = new Rectangle[] { new Rectangle(1, 2, 3, 4), new Rectangle(5, 6, 7, 8) };
                gp.AddRectangles(rects);
                AssertReverse(gp, gp.PathPoints, gp.PathTypes);
            }
        }

        [Fact]
        //[Category("NotWorking")] // the output differs from GDI+ and libgdiplus
        public void Reverse_Pie_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddPie(1, 2, 3, 4, 10, 20);
                byte[] expectedTypes = new byte[] { 0, 3, 3, 3, 129 };
                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }
        }

        [Fact]
        public void Reverse_ArcLineInnerPath_Succes()
        {
            using (GraphicsPath inner = new GraphicsPath())
            using (GraphicsPath gp = new GraphicsPath())
            {
                inner.AddArc(1f, 1f, 2f, 2f, Pi4, Pi4);
                inner.AddLine(1, 2, 3, 4);
                byte[] expectedTypes = new byte[] { 0, 1, 1, 3, 3, 3 };
                gp.AddPath(inner, true);
                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }
        }

        [Fact]
        public void Reverse_EllipseRectangle_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(50, 51, 50, 100);
                gp.AddRectangle(new Rectangle(200, 201, 60, 61));
                byte[] expectedTypes = new byte[] { 0, 1, 1, 129, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 131 };
                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }
        }

        [RunIfFontFamilyGenericMonospaceNotNull]
        //[Category("NotWorking")] // the output differs from GDI+ and libgdiplus
        public void Reverse_String_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                FontFamily ff = GetFontFamily();
                gp.AddString("Mono::", ff, 0, 10, new Point(10, 10), StringFormat.GenericDefault);
                byte[] expectedTypes = new byte[] 
                {
                    0,3,3,3,3,3,3,3,3,3,3,3,3,1,3,3,3,3,3,3,3,3,3,3,3,3,129,
                    0,3,3,3,3,3,3,3,3,3,3,3,3,1,3,3,3,3,3,3,3,3,3,3,3,3,161,
                    0,3,3,3,3,3,3,3,3,3,3,3,3,1,3,3,3,3,3,3,3,3,3,3,3,3,129,
                    0,3,3,3,3,3,3,3,3,3,3,3,3,1,3,3,3,3,3,3,3,3,3,3,3,3,161,
                    0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,131,0,3,
                    3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,163,0,3,3,3,
                    3,3,3,3,3,3,3,3,3,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,1,3,3,3,
                    3,3,3,3,3,3,3,3,3,1,1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,3,3,3,
                    3,3,3,3,3,3,3,3,3,1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,3,3,3,3,
                    3,3,3,3,3,3,3,3,3,3,3,161,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,
                    3,3,3,3,3,3,3,3,3,131,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,
                    3,3,3,3,3,3,3,163,0,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,1,3,3,
                    3,3,3,3,3,3,3,3,3,3,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,
                    1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,
                    1,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,129
                };

                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }
        }

        [Fact]
        public void Reverse_Marker_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(200, 201, 60, 61));
                gp.SetMarkers();
                byte[] expectedTypes = new byte[] { 0, 1, 1, 129 };
                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }
        }

        [Fact]
        public void Reverse_SubpathMarker_Succes()
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(0, 1, 2, 3);
                gp.SetMarkers();
                gp.CloseFigure();
                gp.AddBezier(5, 6, 7, 8, 9, 10, 11, 12);
                gp.CloseFigure();
                byte[] expectedTypes = new byte[] { 0, 3, 3, 163, 0, 129 };
                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(0, 1, 2, 3);
                gp.SetMarkers();
                gp.StartFigure();
                gp.AddLine(20, 21, 22, 23);
                gp.AddBezier(5, 6, 7, 8, 9, 10, 11, 12);
                byte[] expectedTypes = new byte[] { 0, 3, 3, 3, 1, 33, 0, 1 };
                AssertReverse(gp, gp.PathPoints, expectedTypes);
            }
        }

        [Fact]
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

        private FontFamily GetFontFamily()
        {
            try
            {
                return FontFamily.GenericMonospace;
            }
            catch (ArgumentException)
            {
                //Assert.Ignore("GenericMonospace FontFamily couldn't be found");
                return null;
            }
        }

        private void AssertEmptyGrahicsPath(GraphicsPath gp)
        {
            Assert.Equal(0, gp.PathData.Points.Length);
            Assert.Equal(0, gp.PathData.Types.Length);
            Assert.Equal(0, gp.PointCount);
        }

        private void AssertLinesSinglePoint(GraphicsPath path)
        {
            Assert.Equal(1, path.PointCount);
            Assert.Equal(0, path.PathTypes[0]);
        }

        private void AssertEqual(float expexted, float actual, float tollerance)
        {
            Assert.True(Math.Abs(expexted - actual) <= tollerance);
        }

        private void AssertLine(GraphicsPath path)
        {
            Assert.Equal(2, path.PathPoints.Length);
            Assert.Equal(2, path.PathTypes.Length);
            Assert.Equal(2, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(1f, rect.X, Delta);
            AssertEqual(1f, rect.Y, Delta);
            AssertEqual(1f, rect.Width, Delta);
            AssertEqual(1f, rect.Height, Delta);

            AssertEqual(1f, path.PathData.Points[0].X, Delta);
            AssertEqual(1f, path.PathPoints[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(2f, path.PathData.Points[1].X, Delta);
            AssertEqual(2f, path.PathPoints[1].Y, Delta);
            Assert.Equal(1, path.PathTypes[1]);
        }

        private void AssertArc(GraphicsPath path)
        {
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(2.99962401f, rect.X, Delta);
            AssertEqual(2.01370716f, rect.Y, Delta);
            AssertEqual(0f, rect.Width, Delta);
            AssertEqual(0.0137047768f, rect.Height, Delta);

            AssertEqual(2.99990582f, path.PathData.Points[0].X, Delta);
            AssertEqual(2.01370716f, path.PathPoints[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(2.99984312f, path.PathData.Points[1].X, Delta);
            AssertEqual(2.018276f, path.PathPoints[1].Y, Delta);
            Assert.Equal(3, path.PathTypes[1]);
            AssertEqual(2.99974918f, path.PathData.Points[2].X, Delta);
            AssertEqual(2.02284455f, path.PathPoints[2].Y, Delta);
            Assert.Equal(3, path.PathData.Types[2]);
            AssertEqual(2.999624f, path.PathData.Points[3].X, Delta);
            AssertEqual(2.027412f, path.PathPoints[3].Y, Delta);
            Assert.Equal(3, path.PathTypes[3]);
        }

        private void AssertBezier(GraphicsPath path)
        {
            Assert.Equal(4, path.PointCount);
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(1f, rect.X, Delta);
            AssertEqual(1f, rect.Y, Delta);
            AssertEqual(3f, rect.Width, Delta);
            AssertEqual(3f, rect.Height, Delta);

            AssertEqual(1f, path.PathData.Points[0].X, Delta);
            AssertEqual(1f, path.PathPoints[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(2f, path.PathData.Points[1].X, Delta);
            AssertEqual(2f, path.PathPoints[1].Y, Delta);
            Assert.Equal(3, path.PathTypes[1]);
            AssertEqual(3f, path.PathData.Points[2].X, Delta);
            AssertEqual(3f, path.PathPoints[2].Y, Delta);
            Assert.Equal(3, path.PathData.Types[2]);
            AssertEqual(4f, path.PathData.Points[3].X, Delta);
            AssertEqual(4f, path.PathPoints[3].Y, Delta);
            Assert.Equal(3, path.PathTypes[3]);
        }

        private void AssertCurve(GraphicsPath path)
        {
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(1f, rect.X, Delta);
            AssertEqual(1f, rect.Y, Delta);
            AssertEqual(1f, rect.Width, Delta);
            AssertEqual(1f, rect.Height, Delta);


            AssertEqual(1f, path.PathData.Points[0].X, Delta);
            AssertEqual(1f, path.PathData.Points[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(1.16666663f, path.PathData.Points[1].X, Delta);
            AssertEqual(1.16666663f, path.PathData.Points[1].Y, Delta);
            Assert.Equal(3, path.PathData.Types[1]);
            AssertEqual(1.83333325f, path.PathData.Points[2].X, Delta);
            AssertEqual(1.83333325f, path.PathData.Points[2].Y, Delta);
            Assert.Equal(3, path.PathData.Types[2]);
            AssertEqual(2f, path.PathData.Points[3].X, Delta);
            AssertEqual(2f, path.PathData.Points[3].Y, Delta);
            Assert.Equal(3, path.PathTypes[3]);
        }

        private void AssertClosedCurve(GraphicsPath path)
        {
            Assert.Equal(10, path.PathPoints.Length);
            Assert.Equal(10, path.PathTypes.Length);
            Assert.Equal(10, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(0.8333333f, rect.X, Delta);
            AssertEqual(0.8333333f, rect.Y, Delta);
            AssertEqual(2.33333278f, rect.Width, Delta);
            AssertEqual(2.33333278f, rect.Height, Delta);

            Assert.Equal(0, path.PathData.Types[0]);
            for (int i = 1; i < 9; i++)
            {
                Assert.Equal(3, path.PathTypes[i]);
            }

            Assert.Equal(131, path.PathData.Types[9]);
        }

        private void AssertRectangle(GraphicsPath path)
        {
            Assert.Equal(4, path.PathPoints.Length);
            Assert.Equal(4, path.PathTypes.Length);
            Assert.Equal(4, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(1f, rect.X, Delta);
            AssertEqual(1f, rect.Y, Delta);
            AssertEqual(2f, rect.Width, Delta);
            AssertEqual(2f, rect.Height, Delta);

            AssertEqual(1f, path.PathData.Points[0].X, Delta);
            AssertEqual(1f, path.PathData.Points[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(3f, path.PathData.Points[1].X, Delta);
            AssertEqual(1f, path.PathData.Points[1].Y, Delta);
            Assert.Equal(1, path.PathTypes[1]);
            AssertEqual(3f, path.PathData.Points[2].X, Delta);
            AssertEqual(3f, path.PathData.Points[2].Y, Delta);
            Assert.Equal(1, path.PathData.Types[2]);
            AssertEqual(1f, path.PathData.Points[3].X, Delta);
            AssertEqual(3f, path.PathData.Points[3].Y, Delta);
            Assert.Equal(129, path.PathTypes[3]);
        }

        private void AssertEllipse(GraphicsPath path)
        {
            Assert.Equal(13, path.PathPoints.Length);
            Assert.Equal(13, path.PathTypes.Length);
            Assert.Equal(13, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(1f, rect.X, Delta);
            AssertEqual(1f, rect.Y, Delta);
            AssertEqual(2f, rect.Width, Delta);
            AssertEqual(2f, rect.Height, Delta);

            Assert.Equal(0, path.PathData.Types[0]);
            for (int i = 1; i < 12; i++)
                Assert.Equal(3, path.PathTypes[i]);
            Assert.Equal(131, path.PathData.Types[12]);
        }

        private void AssertPie(GraphicsPath path)
        {
            // the number of points generated for a Pie isn't the same between Mono and MS
            Assert.Equal(5, path.PathPoints.Length);
            Assert.Equal(5, path.PathTypes.Length);
            Assert.Equal(5, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(2f, rect.X, Delta);
            AssertEqual(2f, rect.Y, Delta);
            AssertEqual(0.9999058f, rect.Width, Delta);
            AssertEqual(0.0274119377f, rect.Height, Delta);

            AssertEqual(2f, path.PathData.Points[0].X, Delta);
            AssertEqual(2f, path.PathData.Points[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(2.99990582f, path.PathData.Points[1].X, Delta);
            AssertEqual(2.01370716f, path.PathData.Points[1].Y, Delta);
            Assert.Equal(1, path.PathTypes[1]);
            AssertEqual(2.99984312f, path.PathData.Points[2].X, Delta);
            AssertEqual(2.018276f, path.PathData.Points[2].Y, Delta);
            Assert.Equal(3, path.PathData.Types[2]);
            AssertEqual(2.99974918f, path.PathData.Points[3].X, Delta);
            AssertEqual(2.02284455f, path.PathData.Points[3].Y, Delta);
            Assert.Equal(3, path.PathData.Types[3]);
            AssertEqual(2.999624f, path.PathData.Points[4].X, Delta);
            AssertEqual(2.027412f, path.PathData.Points[4].Y, Delta);
            Assert.Equal(131, path.PathTypes[4]);
        }

        private void AssertPolygon(GraphicsPath path)
        {
            Assert.Equal(3, path.PathPoints.Length);
            Assert.Equal(3, path.PathTypes.Length);
            Assert.Equal(3, path.PathData.Points.Length);

            RectangleF rect = path.GetBounds();
            AssertEqual(1f, rect.X, Delta);
            AssertEqual(1f, rect.Y, Delta);
            AssertEqual(2f, rect.Width, Delta);
            AssertEqual(2f, rect.Height, Delta);

            AssertEqual(1f, path.PathData.Points[0].X, Delta);
            AssertEqual(1f, path.PathData.Points[0].Y, Delta);
            Assert.Equal(0, path.PathData.Types[0]);
            AssertEqual(2f, path.PathData.Points[1].X, Delta);
            AssertEqual(2f, path.PathData.Points[1].Y, Delta);
            Assert.Equal(1, path.PathTypes[1]);
            AssertEqual(3f, path.PathData.Points[2].X, Delta);
            AssertEqual(3f, path.PathData.Points[2].Y, Delta);
            Assert.Equal(129, path.PathData.Types[2]);
        }

        private void AssertRectangleBounds(RectangleF rect, float x, float y, float width, float height)
        {
            AssertEqual(x, rect.X, Delta);
            AssertEqual(y, rect.Y, Delta);
            AssertEqual(width, rect.Width, Delta);
            AssertEqual(height, rect.Height, Delta);
        }

        private void AssertPieBounds(RectangleF rect, float x, float y, float width, float height)
        {
            Assert.Equal(x, rect.X, 1);
            Assert.Equal(y, rect.Y, 1);
            Assert.Equal(width, rect.Width, 1);
            Assert.Equal(height, rect.Height, 1);
        }

        private void AssertPaths(GraphicsPath expected, GraphicsPath actual)
        {
            Assert.Equal(expected.PointCount, actual.PointCount);
            for (int i = 0; i < expected.PointCount; i++)
            {
                Assert.Equal(expected.PathPoints[i], actual.PathPoints[i]);
                Assert.Equal(expected.PathTypes[i], actual.PathTypes[i]);
            }
        }

        private void AssertFlats(GraphicsPath flat, GraphicsPath original)
        {
            Assert.True(flat.PointCount >= original.PointCount);
            for (int i = 0; i < flat.PointCount; i++)
            {
                Assert.True(flat.PathTypes[i] != 3);
            }
        }

        private void AssertWrap(
            GraphicsPath path, float expectedX1, float expectedY1, float expectedX2, float expectedY2, float expectedX3, float expectedY3)
        {
            Assert.Equal(3, path.PointCount);

            var tolerance = 1e-30f;
            PointF[] pts = path.PathPoints;
            AssertEqual(expectedX1, pts[0].X, tolerance);
            AssertEqual(expectedY1, pts[0].Y, tolerance);
            AssertEqual(expectedX2, pts[1].X, tolerance);
            AssertEqual(expectedY2, pts[1].Y, tolerance);
            AssertEqual(expectedX3, pts[2].X, tolerance);
            AssertEqual(expectedY3, pts[2].Y, tolerance);

            byte[] types = path.PathTypes;
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[1]);
            Assert.Equal(129, types[2]);
        }

        private void AssertWrapNaN(GraphicsPath path, bool closed)
        {
            Assert.Equal(3, path.PointCount);

            PointF[] pts = path.PathPoints;
            Assert.Equal(float.NaN, pts[0].X);
            Assert.Equal(float.NaN, pts[0].Y);
            Assert.Equal(float.NaN, pts[1].X);
            Assert.Equal(float.NaN, pts[1].Y);
            Assert.Equal(float.NaN, pts[2].X);
            Assert.Equal(float.NaN, pts[2].Y);

            byte[] types = path.PathTypes;
            Assert.Equal(0, types[0]);
            Assert.Equal(1, types[1]);
            Assert.Equal(closed ? 129 : 1, types[2]);
        }

        private void AssertWiden3(GraphicsPath path)
        {
            float tolerance = 0.25f;
            AssertEqual(4.2f, path.PathPoints[0].X, tolerance);
            AssertEqual(4.5f, path.PathPoints[0].Y, tolerance);
            AssertEqual(15.8f, path.PathPoints[1].X, tolerance);
            AssertEqual(4.5f, path.PathPoints[1].Y, tolerance);
            AssertEqual(10.0f, path.PathPoints[2].X, tolerance);
            AssertEqual(16.1f, path.PathPoints[2].Y, tolerance);
            AssertEqual(10.4f, path.PathPoints[3].X, tolerance);
            AssertEqual(14.8f, path.PathPoints[3].Y, tolerance);
            AssertEqual(9.6f, path.PathPoints[4].X, tolerance);
            AssertEqual(14.8f, path.PathPoints[4].Y, tolerance);
            AssertEqual(14.6f, path.PathPoints[5].X, tolerance);
            AssertEqual(4.8f, path.PathPoints[5].Y, tolerance);
            AssertEqual(15.0f, path.PathPoints[6].X, tolerance);
            AssertEqual(5.5f, path.PathPoints[6].Y, tolerance);
            AssertEqual(5.0f, path.PathPoints[7].X, tolerance);
            AssertEqual(5.5f, path.PathPoints[7].Y, tolerance);
            AssertEqual(5.4f, path.PathPoints[8].X, tolerance);
            AssertEqual(4.8f, path.PathPoints[8].Y, tolerance);

            Assert.Equal(0, path.PathTypes[0]);
            Assert.Equal(1, path.PathTypes[1]);
            Assert.Equal(129, path.PathTypes[2]);
            Assert.Equal(0, path.PathTypes[3]);
            Assert.Equal(1, path.PathTypes[4]);
            Assert.Equal(1, path.PathTypes[5]);
            Assert.Equal(1, path.PathTypes[6]);
            Assert.Equal(1, path.PathTypes[7]);
            Assert.Equal(129, path.PathTypes[8]);
        }

        private void AssertWidenedBounds(
            GraphicsPath gp, Matrix matrix, float expectedX, float expectedY, float expectedWidth, float expectedHeight)
        {
            RectangleF bounds = gp.GetBounds(matrix);
            AssertEqual(expectedX, bounds.X, 0.00001f);
            AssertEqual(expectedY, bounds.Y, 0.00001f);
            AssertEqual(expectedWidth, bounds.Width, 0.00001f);
            AssertEqual(expectedHeight, bounds.Height, 0.00001f);
        }

        private void AssertIsOutlineVisibleLine(Graphics graphics)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Red, 3.0f))
            {
                gp.AddLine(10, 1, 14, 1);
                Assert.True(gp.IsOutlineVisible(10, 1, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(10, 2, pen, graphics));
                Assert.False(gp.IsOutlineVisible(10, 2, Pens.Red, graphics));

                Assert.True(gp.IsOutlineVisible(11.0f, 1.0f, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(11.0f, 1.0f, pen, graphics));
                Assert.False(gp.IsOutlineVisible(11.0f, 2.0f, Pens.Red, graphics));

                Point point = new Point(12, 2);
                Assert.False(gp.IsOutlineVisible(point, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(point, pen, graphics));

                point.Y = 1;
                Assert.True(gp.IsOutlineVisible(point, Pens.Red, graphics));

                PointF fPoint = new PointF(13.0f, 2.0f);
                Assert.False(gp.IsOutlineVisible(fPoint, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(fPoint, pen, graphics));

                fPoint.Y = 1;
                Assert.True(gp.IsOutlineVisible(fPoint, Pens.Red, graphics));
            }
        }

        private void AssertIsOutlineVisibleRectangle(Graphics graphics)
        {
            using (Pen pen = new Pen(Color.Red, 3.0f))
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(10, 10, 20, 20));
                Assert.True(gp.IsOutlineVisible(10, 10, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(10, 11, pen, graphics));
                Assert.False(gp.IsOutlineVisible(11, 11, Pens.Red, graphics));

                Assert.True(gp.IsOutlineVisible(11.0f, 10.0f, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(11.0f, 11.0f, pen, graphics));
                Assert.False(gp.IsOutlineVisible(11.0f, 11.0f, Pens.Red, graphics));

                Point pt = new Point(15, 10);
                Assert.True(gp.IsOutlineVisible(pt, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(pt, pen, graphics));

                pt.Y = 15;
                Assert.False(gp.IsOutlineVisible(pt, Pens.Red, graphics));

                PointF pf = new PointF(29.0f, 29.0f);
                Assert.False(gp.IsOutlineVisible(pf, Pens.Red, graphics));
                Assert.True(gp.IsOutlineVisible(pf, pen, graphics));

                pf.Y = 31.0f;
                Assert.True(gp.IsOutlineVisible(pf, pen, graphics));
            }
        }

        private void AssertIsVisibleRectangle(Graphics graphics)
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
                Assert.True(gp.IsVisible(29.4f, 29.4f, graphics));

                Assert.False(gp.IsVisible(29.5f, 29.5f, graphics));
                Assert.False(gp.IsVisible(29.5f, 29.4f, graphics));
                Assert.False(gp.IsVisible(29.4f, 29.5f, graphics));
            }
        }

        private void AssertIsVisibleEllipse(Graphics graphics)
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

        private void AssertReverse(GraphicsPath gp, PointF[] expectedPoints, byte[] expectedTypes)
        {
            gp.Reverse();
            PointF[] reversedPoints = gp.PathPoints;
            byte[] reversedTypes = gp.PathTypes;

            int count = gp.PointCount;
            Assert.Equal(expectedPoints.Length, count);
            for (int i = 0; i < count; i++)
            {
                Assert.Equal(expectedPoints[i], reversedPoints[count - i - 1]);
                Assert.Equal(expectedTypes[i], reversedTypes[i]);
            }
        }
    }    
}


