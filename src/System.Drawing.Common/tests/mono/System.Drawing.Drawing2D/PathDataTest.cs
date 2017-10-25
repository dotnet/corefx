// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.PathData unit tests
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Drawing2D
{

    public class PathDataTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PathData_Empty()
        {
            PathData data = new PathData();
            Assert.Null(data.Points);
            Assert.Null(data.Types);

            data.Points = new PointF[0];
            data.Types = new byte[0];
            Assert.Equal(0, data.Points.Length);
            Assert.Equal(0, data.Types.Length);

            data.Points = null;
            data.Types = null;
            Assert.Null(data.Points);
            Assert.Null(data.Types);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PathData_LengthMismatch()
        {
            PathData data = new PathData();
            data.Points = new PointF[2];
            data.Types = new byte[1];
            Assert.Equal(2, data.Points.Length);
            Assert.Equal(1, data.Types.Length);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PathData_UnclonedProperties()
        {
            PathData data = new PathData();
            data.Points = new PointF[1] { new PointF(1f, 1f) };
            data.Types = new byte[1] { 1 };
            Assert.Equal(1f, data.Points[0].X);
            Assert.Equal(1f, data.Points[0].Y);
            Assert.Equal(1, data.Types[0]);

            data.Points[0] = new PointF(0f, 0f);
            Assert.Equal(0f, data.Points[0].X);
            Assert.Equal(0f, data.Points[0].Y);

            data.Types[0] = 0;
            Assert.Equal(0, data.Types[0]);
        }
    }
}
