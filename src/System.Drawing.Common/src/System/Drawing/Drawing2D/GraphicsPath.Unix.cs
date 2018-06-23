// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.GraphicsPath.cs
//
// Authors:
//
//   Miguel de Icaza (miguel@ximian.com)
//   Duncan Mak (duncan@ximian.com)
//   Jordi Mas i Hernandez (jordi@ximian.com)
//   Ravindra (rkumar@novell.com)
//   Sebastien Pouliot  <sebastien@ximian.com>
//
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
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
    public sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
    {
        // 1/4 is the FlatnessDefault as defined in GdiPlusEnums.h
        private const float FlatnessDefault = 1.0f / 4.0f;

        internal IntPtr _nativePath = IntPtr.Zero;

        GraphicsPath(IntPtr ptr)
        {
            _nativePath = ptr;
        }

        public GraphicsPath()
        {
            int status = SafeNativeMethods.Gdip.GdipCreatePath(FillMode.Alternate, out _nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public GraphicsPath(FillMode fillMode)
        {
            int status = SafeNativeMethods.Gdip.GdipCreatePath(fillMode, out _nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public GraphicsPath(Point[] pts, byte[] types)
            : this(pts, types, FillMode.Alternate)
        {
        }

        public GraphicsPath(PointF[] pts, byte[] types)
            : this(pts, types, FillMode.Alternate)
        {
        }

        public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));
            if (pts.Length != types.Length)
                throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");

            int status = SafeNativeMethods.Gdip.GdipCreatePath2I(pts, types, pts.Length, fillMode, out _nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));
            if (pts.Length != types.Length)
                throw new ArgumentException("Invalid parameter passed. Number of points and types must be same.");

            int status = SafeNativeMethods.Gdip.GdipCreatePath2(pts, types, pts.Length, fillMode, out _nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public object Clone()
        {
            IntPtr clone;

            int status = SafeNativeMethods.Gdip.GdipClonePath(_nativePath, out clone);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new GraphicsPath(clone);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        ~GraphicsPath()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            int status;
            if (_nativePath != IntPtr.Zero)
            {
                status = SafeNativeMethods.Gdip.GdipDeletePath(_nativePath);
                SafeNativeMethods.Gdip.CheckStatus(status);

                _nativePath = IntPtr.Zero;
            }
        }

        public FillMode FillMode
        {
            get
            {
                FillMode mode;
                int status = SafeNativeMethods.Gdip.GdipGetPathFillMode(_nativePath, out mode);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return mode;
            }
            set
            {
                if ((value < FillMode.Alternate) || (value > FillMode.Winding))
                    throw new InvalidEnumArgumentException("FillMode", (int)value, typeof(FillMode));

                int status = SafeNativeMethods.Gdip.GdipSetPathFillMode(_nativePath, value);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public PathData PathData
        {
            get
            {
                int count;
                int status = SafeNativeMethods.Gdip.GdipGetPointCount(_nativePath, out count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                PointF[] points = new PointF[count];
                byte[] types = new byte[count];

                // status would fail if we ask points or types with a 0 count
                // anyway that would only mean two unrequired unmanaged calls
                if (count > 0)
                {
                    status = SafeNativeMethods.Gdip.GdipGetPathPoints(_nativePath, points, count);
                    SafeNativeMethods.Gdip.CheckStatus(status);

                    status = SafeNativeMethods.Gdip.GdipGetPathTypes(_nativePath, types, count);
                    SafeNativeMethods.Gdip.CheckStatus(status);
                }

                PathData pdata = new PathData();
                pdata.Points = points;
                pdata.Types = types;
                return pdata;
            }
        }

        public PointF[] PathPoints
        {
            get
            {
                int count;
                int status = SafeNativeMethods.Gdip.GdipGetPointCount(_nativePath, out count);
                SafeNativeMethods.Gdip.CheckStatus(status);
                if (count == 0)
                    throw new ArgumentException("PathPoints");

                PointF[] points = new PointF[count];
                status = SafeNativeMethods.Gdip.GdipGetPathPoints(_nativePath, points, count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return points;
            }
        }

        public byte[] PathTypes
        {
            get
            {
                int count;
                int status = SafeNativeMethods.Gdip.GdipGetPointCount(_nativePath, out count);
                SafeNativeMethods.Gdip.CheckStatus(status);
                if (count == 0)
                    throw new ArgumentException("PathTypes");

                byte[] types = new byte[count];
                status = SafeNativeMethods.Gdip.GdipGetPathTypes(_nativePath, types, count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return types;
            }
        }

        public int PointCount
        {
            get
            {
                int count;
                int status = SafeNativeMethods.Gdip.GdipGetPointCount(_nativePath, out count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return count;
            }
        }

        internal IntPtr NativeObject
        {
            get
            {
                return _nativePath;
            }
            set
            {
                _nativePath = value;
            }
        }

        //
        // AddArc
        //
        public void AddArc(Rectangle rect, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathArcI(_nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddArc(RectangleF rect, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathArc(_nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathArcI(_nativePath, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathArc(_nativePath, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddBezier
        //
        public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathBezierI(_nativePath, pt1.X, pt1.Y,
                            pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathBezier(_nativePath, pt1.X, pt1.Y,
                            pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathBezierI(_nativePath, x1, y1, x2, y2, x3, y3, x4, y4);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathBezier(_nativePath, x1, y1, x2, y2, x3, y3, x4, y4);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddBeziers
        //
        public void AddBeziers(params Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipAddPathBeziersI(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddBeziers(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipAddPathBeziers(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddEllipse
        //
        public void AddEllipse(RectangleF rect)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathEllipse(_nativePath, rect.X, rect.Y, rect.Width, rect.Height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddEllipse(float x, float y, float width, float height)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathEllipse(_nativePath, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddEllipse(Rectangle rect)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathEllipseI(_nativePath, rect.X, rect.Y, rect.Width, rect.Height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddEllipse(int x, int y, int width, int height)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathEllipseI(_nativePath, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }


        //
        // AddLine
        //
        public void AddLine(Point pt1, Point pt2)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathLineI(_nativePath, pt1.X, pt1.Y, pt2.X, pt2.Y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddLine(PointF pt1, PointF pt2)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathLine(_nativePath, pt1.X, pt1.Y, pt2.X,
                            pt2.Y);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddLine(int x1, int y1, int x2, int y2)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathLineI(_nativePath, x1, y1, x2, y2);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddLine(float x1, float y1, float x2, float y2)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathLine(_nativePath, x1, y1, x2,
                            y2);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddLines
        //
        public void AddLines(Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (points.Length == 0)
                throw new ArgumentException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathLine2I(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddLines(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (points.Length == 0)
                throw new ArgumentException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathLine2(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddPie
        //
        public void AddPie(Rectangle rect, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathPie(
                    _nativePath, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathPieI(_nativePath, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathPie(_nativePath, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddPolygon
        //
        public void AddPolygon(Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathPolygonI(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddPolygon(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathPolygon(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddRectangle
        //
        public void AddRectangle(Rectangle rect)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathRectangleI(_nativePath, rect.X, rect.Y, rect.Width, rect.Height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddRectangle(RectangleF rect)
        {
            int status = SafeNativeMethods.Gdip.GdipAddPathRectangle(_nativePath, rect.X, rect.Y, rect.Width, rect.Height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddRectangles
        //
        public void AddRectangles(Rectangle[] rects)
        {
            if (rects == null)
                throw new ArgumentNullException(nameof(rects));
            if (rects.Length == 0)
                throw new ArgumentException(nameof(rects));

            int status = SafeNativeMethods.Gdip.GdipAddPathRectanglesI(_nativePath, rects, rects.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddRectangles(RectangleF[] rects)
        {
            if (rects == null)
                throw new ArgumentNullException(nameof(rects));
            if (rects.Length == 0)
                throw new ArgumentException(nameof(rects));

            int status = SafeNativeMethods.Gdip.GdipAddPathRectangles(_nativePath, rects, rects.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddPath
        //
        public void AddPath(GraphicsPath addingPath, bool connect)
        {
            if (addingPath == null)
                throw new ArgumentNullException(nameof(addingPath));

            int status = SafeNativeMethods.Gdip.GdipAddPathPath(_nativePath, addingPath._nativePath, connect);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public PointF GetLastPoint()
        {
            PointF pt;
            int status = SafeNativeMethods.Gdip.GdipGetPathLastPoint(_nativePath, out pt);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return pt;
        }

        //
        // AddClosedCurve
        //
        public void AddClosedCurve(Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathClosedCurveI(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddClosedCurve(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathClosedCurve(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddClosedCurve(Point[] points, float tension)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathClosedCurve2I(_nativePath, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddClosedCurve(PointF[] points, float tension)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathClosedCurve2(_nativePath, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        //
        // AddCurve
        //
        public void AddCurve(Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathCurveI(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddCurve(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathCurve(_nativePath, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddCurve(Point[] points, float tension)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathCurve2I(_nativePath, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddCurve(PointF[] points, float tension)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathCurve2(_nativePath, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathCurve3I(_nativePath, points, points.Length,
                            offset, numberOfSegments, tension);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status = SafeNativeMethods.Gdip.GdipAddPathCurve3(_nativePath, points, points.Length,
                            offset, numberOfSegments, tension);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void Reset()
        {
            int status = SafeNativeMethods.Gdip.GdipResetPath(_nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void Reverse()
        {
            int status = SafeNativeMethods.Gdip.GdipReversePath(_nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void Transform(Matrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            int status = SafeNativeMethods.Gdip.GdipTransformPath(_nativePath, matrix.nativeMatrix);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat format)
        {
            Rectangle layout = new Rectangle();
            layout.X = origin.X;
            layout.Y = origin.Y;
            AddString(s, family, style, emSize, layout, format);
        }

        public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat format)
        {
            RectangleF layout = new RectangleF();
            layout.X = origin.X;
            layout.Y = origin.Y;
            AddString(s, family, style, emSize, layout, format);
        }

        public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat format)
        {
            if (family == null)
                throw new ArgumentException(nameof(family));

            IntPtr sformat = (format == null) ? IntPtr.Zero : format.nativeFormat;
            // note: the NullReferenceException on s.Length is the expected (MS) exception
            int status = SafeNativeMethods.Gdip.GdipAddPathStringI(_nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, sformat);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat format)
        {
            if (family == null)
                throw new ArgumentException(nameof(family));

            IntPtr sformat = (format == null) ? IntPtr.Zero : format.nativeFormat;
            // note: the NullReferenceException on s.Length is the expected (MS) exception
            int status = SafeNativeMethods.Gdip.GdipAddPathString(_nativePath, s, s.Length, family.NativeFamily, style, emSize, ref layoutRect, sformat);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void ClearMarkers()
        {
            int s = SafeNativeMethods.Gdip.GdipClearPathMarkers(_nativePath);

            SafeNativeMethods.Gdip.CheckStatus(s);
        }

        public void CloseAllFigures()
        {
            int s = SafeNativeMethods.Gdip.GdipClosePathFigures(_nativePath);

            SafeNativeMethods.Gdip.CheckStatus(s);
        }

        public void CloseFigure()
        {
            int s = SafeNativeMethods.Gdip.GdipClosePathFigure(_nativePath);

            SafeNativeMethods.Gdip.CheckStatus(s);
        }

        public void Flatten()
        {
            Flatten(null, FlatnessDefault);
        }

        public void Flatten(Matrix matrix)
        {
            Flatten(matrix, FlatnessDefault);
        }

        public void Flatten(Matrix matrix, float flatness)
        {
            IntPtr m = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;
            int status = SafeNativeMethods.Gdip.GdipFlattenPath(_nativePath, m, flatness);

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public RectangleF GetBounds()
        {
            return GetBounds(null, null);
        }

        public RectangleF GetBounds(Matrix matrix)
        {
            return GetBounds(matrix, null);
        }

        public RectangleF GetBounds(Matrix matrix, Pen pen)
        {
            RectangleF retval;
            IntPtr m = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;
            IntPtr p = (pen == null) ? IntPtr.Zero : pen.NativePen;

            int s = SafeNativeMethods.Gdip.GdipGetPathWorldBounds(_nativePath, out retval, m, p);

            SafeNativeMethods.Gdip.CheckStatus(s);

            return retval;
        }

        public bool IsOutlineVisible(Point point, Pen pen)
        {
            return IsOutlineVisible(point.X, point.Y, pen, null);
        }

        public bool IsOutlineVisible(PointF point, Pen pen)
        {
            return IsOutlineVisible(point.X, point.Y, pen, null);
        }

        public bool IsOutlineVisible(int x, int y, Pen pen)
        {
            return IsOutlineVisible(x, y, pen, null);
        }

        public bool IsOutlineVisible(float x, float y, Pen pen)
        {
            return IsOutlineVisible(x, y, pen, null);
        }

        public bool IsOutlineVisible(Point pt, Pen pen, Graphics graphics)
        {
            return IsOutlineVisible(pt.X, pt.Y, pen, graphics);
        }

        public bool IsOutlineVisible(PointF pt, Pen pen, Graphics graphics)
        {
            return IsOutlineVisible(pt.X, pt.Y, pen, graphics);
        }

        public bool IsOutlineVisible(int x, int y, Pen pen, Graphics graphics)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            bool result;
            IntPtr g = (graphics == null) ? IntPtr.Zero : graphics.NativeGraphics;

            int s = SafeNativeMethods.Gdip.GdipIsOutlineVisiblePathPointI(_nativePath, x, y, pen.NativePen, g, out result);
            SafeNativeMethods.Gdip.CheckStatus(s);

            return result;
        }

        public bool IsOutlineVisible(float x, float y, Pen pen, Graphics graphics)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            bool result;
            IntPtr g = (graphics == null) ? IntPtr.Zero : graphics.NativeGraphics;

            int s = SafeNativeMethods.Gdip.GdipIsOutlineVisiblePathPoint(_nativePath, x, y, pen.NativePen, g, out result);
            SafeNativeMethods.Gdip.CheckStatus(s);

            return result;
        }

        public bool IsVisible(Point point)
        {
            return IsVisible(point.X, point.Y, null);
        }

        public bool IsVisible(PointF point)
        {
            return IsVisible(point.X, point.Y, null);
        }

        public bool IsVisible(int x, int y)
        {
            return IsVisible(x, y, null);
        }

        public bool IsVisible(float x, float y)
        {
            return IsVisible(x, y, null);
        }

        public bool IsVisible(Point pt, Graphics graphics)
        {
            return IsVisible(pt.X, pt.Y, graphics);
        }

        public bool IsVisible(PointF pt, Graphics graphics)
        {
            return IsVisible(pt.X, pt.Y, graphics);
        }

        public bool IsVisible(int x, int y, Graphics graphics)
        {
            bool retval;

            IntPtr g = (graphics == null) ? IntPtr.Zero : graphics.NativeGraphics;

            int s = SafeNativeMethods.Gdip.GdipIsVisiblePathPointI(_nativePath, x, y, g, out retval);

            SafeNativeMethods.Gdip.CheckStatus(s);

            return retval;
        }

        public bool IsVisible(float x, float y, Graphics graphics)
        {
            bool retval;

            IntPtr g = (graphics == null) ? IntPtr.Zero : graphics.NativeGraphics;

            int s = SafeNativeMethods.Gdip.GdipIsVisiblePathPoint(_nativePath, x, y, g, out retval);

            SafeNativeMethods.Gdip.CheckStatus(s);

            return retval;
        }

        public void SetMarkers()
        {
            int s = SafeNativeMethods.Gdip.GdipSetPathMarker(_nativePath);

            SafeNativeMethods.Gdip.CheckStatus(s);
        }

        public void StartFigure()
        {
            int s = SafeNativeMethods.Gdip.GdipStartPathFigure(_nativePath);

            SafeNativeMethods.Gdip.CheckStatus(s);
        }

        public void Warp(PointF[] destPoints, RectangleF srcRect)
        {
            Warp(destPoints, srcRect, null, WarpMode.Perspective, FlatnessDefault);
        }

        public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix)
        {
            Warp(destPoints, srcRect, matrix, WarpMode.Perspective, FlatnessDefault);
        }

        public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode)
        {
            Warp(destPoints, srcRect, matrix, warpMode, FlatnessDefault);
        }

        public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix matrix, WarpMode warpMode, float flatness)
        {
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));

            IntPtr m = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;

            int s = SafeNativeMethods.Gdip.GdipWarpPath(_nativePath, m, destPoints, destPoints.Length,
                            srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, warpMode, flatness);

            SafeNativeMethods.Gdip.CheckStatus(s);
        }

        public void Widen(Pen pen)
        {
            Widen(pen, null, FlatnessDefault);
        }

        public void Widen(Pen pen, Matrix matrix)
        {
            Widen(pen, matrix, FlatnessDefault);
        }

        public void Widen(Pen pen, Matrix matrix, float flatness)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (PointCount == 0)
                return;
            IntPtr m = (matrix == null) ? IntPtr.Zero : matrix.nativeMatrix;

            int s = SafeNativeMethods.Gdip.GdipWidenPath(_nativePath, pen.NativePen, m, flatness);
            SafeNativeMethods.Gdip.CheckStatus(s);
        }
    }
}
