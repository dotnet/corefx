// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Graphics.cs
//
// Authors:
//    Gonzalo Paniagua Javier (gonzalo@ximian.com) (stubbed out)
//      Alexandre Pigolkine(pigolkine@gmx.de)
//    Jordi Mas i Hernandez (jordi@ximian.com)
//    Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2003 Ximian, Inc. (http://www.ximian.com)
// Copyright (C) 2004-2006 Novell, Inc. (http://www.novell.com)
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

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.Drawing.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing
{
    public sealed partial class Graphics : MarshalByRefObject, IDisposable
    , IDeviceContext
    {
        internal IMacContext maccontext;
        private bool disposed = false;
        private static float defDpiX = 0;
        private static float defDpiY = 0;

        public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType,
                                int flags,
                                int dataSize,
                                IntPtr data,
                                PlayRecordCallback callbackData);

        public delegate bool DrawImageAbort(IntPtr callbackdata);

        internal Graphics(IntPtr nativeGraphics) => _nativeGraphics = nativeGraphics;

        ~Graphics()
        {
            Dispose();
        }

        static internal float systemDpiX
        {
            get
            {
                if (defDpiX == 0)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    Graphics g = Graphics.FromImage(bmp);
                    defDpiX = g.DpiX;
                    defDpiY = g.DpiY;
                }
                return defDpiX;
            }
        }

        static internal float systemDpiY
        {
            get
            {
                if (defDpiY == 0)
                {
                    Bitmap bmp = new Bitmap(1, 1);
                    Graphics g = Graphics.FromImage(bmp);
                    defDpiX = g.DpiX;
                    defDpiY = g.DpiY;
                }
                return defDpiY;
            }
        }

        public void AddMetafileComment(byte[] data)
        {
            throw new NotImplementedException();
        }

        public GraphicsContainer BeginContainer()
        {
            int state;
            int status = SafeNativeMethods.Gdip.GdipBeginContainer2(new HandleRef(this, _nativeGraphics), out state);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new GraphicsContainer(state);
        }

        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        {
            int state;

            var dstf = new GPRECT(dstrect);
            var srcf = new GPRECT(srcrect);

            int status = SafeNativeMethods.Gdip.GdipBeginContainerI(new HandleRef(this, _nativeGraphics), ref dstf, ref srcf, unchecked((int)unit), out state);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new GraphicsContainer(state);
        }

        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
        {
            int state;

            var dstf = new GPRECTF(dstrect);
            var srcf = new GPRECTF(srcrect);

            int status = SafeNativeMethods.Gdip.GdipBeginContainer(new HandleRef(this, _nativeGraphics), ref dstf, ref srcf, unchecked((int)unit), out state);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new GraphicsContainer(state);
        }


        public void Clear(Color color)
        {
            int status;
            status = SafeNativeMethods.Gdip.GdipGraphicsClear(_nativeGraphics, color.ToArgb());
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
        {
            CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y,
                blockRegionSize, CopyPixelOperation.SourceCopy);
        }

        public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
            CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y,
                blockRegionSize, copyPixelOperation);
        }

        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
        {
            CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize,
                CopyPixelOperation.SourceCopy);
        }

        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
            if (!Enum.IsDefined(typeof(CopyPixelOperation), copyPixelOperation))
                throw new InvalidEnumArgumentException(string.Format("Enum argument value '{0}' is not valid for CopyPixelOperation", copyPixelOperation));

            if (SafeNativeMethods.Gdip.UseX11Drawable)
            {
                CopyFromScreenX11(sourceX, sourceY, destinationX, destinationY, blockRegionSize, copyPixelOperation);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        private void CopyFromScreenX11(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
            IntPtr window, image, defvisual, vPtr;
            int AllPlanes = ~0, nitems = 0, pixel;

            if (copyPixelOperation != CopyPixelOperation.SourceCopy)
                throw new NotImplementedException("Operation not implemented under X11");

            if (SafeNativeMethods.Gdip.Display == IntPtr.Zero)
            {
                SafeNativeMethods.Gdip.Display = LibX11Functions.XOpenDisplay(IntPtr.Zero);
            }

            window = LibX11Functions.XRootWindow(SafeNativeMethods.Gdip.Display, 0);
            defvisual = LibX11Functions.XDefaultVisual(SafeNativeMethods.Gdip.Display, 0);
            XVisualInfo visual = new XVisualInfo();

            /* Get XVisualInfo for this visual */
            visual.visualid = LibX11Functions.XVisualIDFromVisual(defvisual);
            vPtr = LibX11Functions.XGetVisualInfo(SafeNativeMethods.Gdip.Display, 0x1 /* VisualIDMask */, ref visual, ref nitems);
            visual = (XVisualInfo)Marshal.PtrToStructure(vPtr, typeof(XVisualInfo));
            image = LibX11Functions.XGetImage(SafeNativeMethods.Gdip.Display, window, sourceX, sourceY, blockRegionSize.Width,
                blockRegionSize.Height, AllPlanes, 2 /* ZPixmap*/);
            if (image == IntPtr.Zero)
            {
                string s = String.Format("XGetImage returned NULL when asked to for a {0}x{1} region block",
                    blockRegionSize.Width, blockRegionSize.Height);
                throw new InvalidOperationException(s);
            }

            Bitmap bmp = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
            int red, blue, green;
            int red_mask = (int)visual.red_mask;
            int blue_mask = (int)visual.blue_mask;
            int green_mask = (int)visual.green_mask;
            for (int y = 0; y < blockRegionSize.Height; y++)
            {
                for (int x = 0; x < blockRegionSize.Width; x++)
                {
                    pixel = LibX11Functions.XGetPixel(image, x, y);

                    switch (visual.depth)
                    {
                        case 16: /* 16bbp pixel transformation */
                            red = (int)((pixel & red_mask) >> 8) & 0xff;
                            green = (int)(((pixel & green_mask) >> 3)) & 0xff;
                            blue = (int)((pixel & blue_mask) << 3) & 0xff;
                            break;
                        case 24:
                        case 32:
                            red = (int)((pixel & red_mask) >> 16) & 0xff;
                            green = (int)(((pixel & green_mask) >> 8)) & 0xff;
                            blue = (int)((pixel & blue_mask)) & 0xff;
                            break;
                        default:
                            string text = string.Format("{0}bbp depth not supported.", visual.depth);
                            throw new NotImplementedException(text);
                    }

                    bmp.SetPixel(x, y, Color.FromArgb(255, red, green, blue));
                }
            }

            DrawImage(bmp, destinationX, destinationY);
            bmp.Dispose();
            LibX11Functions.XDestroyImage(image);
            LibX11Functions.XFree(vPtr);
        }

        public void Dispose()
        {
            int status;
            if (!disposed)
            {
                if (_nativeHdc != IntPtr.Zero) // avoid a handle leak.
                {
                    ReleaseHdc();
                }

                if (!SafeNativeMethods.Gdip.UseX11Drawable)
                {
                    Flush();
                    if (maccontext != null)
                        maccontext.Release();
                }

                status = SafeNativeMethods.Gdip.GdipDeleteGraphics(_nativeGraphics);
                _nativeGraphics = IntPtr.Zero;
                SafeNativeMethods.Gdip.CheckStatus(status);
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }


        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }


        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }


        public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            int status;
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            status = SafeNativeMethods.Gdip.GdipDrawArc(_nativeGraphics, pen.NativePen,
                                        x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        // Microsoft documentation states that the signature for this member should be
        // public void DrawArc( Pen pen,  int x,  int y,  int width,  int height,   int startAngle,
        // int sweepAngle. However, GdipDrawArcI uses also float for the startAngle and sweepAngle params
        public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            int status;
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            status = SafeNativeMethods.Gdip.GdipDrawArcI(_nativeGraphics, pen.NativePen,
                        x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            int status;
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            status = SafeNativeMethods.Gdip.GdipDrawBezier(_nativeGraphics, pen.NativePen,
                            pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X,
                            pt3.Y, pt4.X, pt4.Y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            int status;
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            status = SafeNativeMethods.Gdip.GdipDrawBezierI(_nativeGraphics, pen.NativePen,
                            pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X,
                            pt3.Y, pt4.X, pt4.Y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            int status;
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            status = SafeNativeMethods.Gdip.GdipDrawBezier(_nativeGraphics, pen.NativePen, x1,
                            y1, x2, y2, x3, y3, x4, y4);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawBeziers(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int length = points.Length;
            int status;

            if (length < 4)
                return;

            for (int i = 0; i < length - 1; i += 3)
            {
                Point p1 = points[i];
                Point p2 = points[i + 1];
                Point p3 = points[i + 2];
                Point p4 = points[i + 3];

                status = SafeNativeMethods.Gdip.GdipDrawBezier(_nativeGraphics,
            pen.NativePen,
                                        p1.X, p1.Y, p2.X, p2.Y,
                                        p3.X, p3.Y, p4.X, p4.Y);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public void DrawBeziers(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int length = points.Length;
            int status;

            if (length < 4)
                return;

            for (int i = 0; i < length - 1; i += 3)
            {
                PointF p1 = points[i];
                PointF p2 = points[i + 1];
                PointF p3 = points[i + 2];
                PointF p4 = points[i + 3];

                status = SafeNativeMethods.Gdip.GdipDrawBezier(_nativeGraphics,
            pen.NativePen,
                                        p1.X, p1.Y, p2.X, p2.Y,
                                        p3.X, p3.Y, p4.X, p4.Y);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }


        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawClosedCurve(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawClosedCurveI(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        // according to MSDN fillmode "is required but ignored" which makes _some_ sense since the unmanaged 
        // GDI+ call doesn't support it (issue spotted using Gendarme's AvoidUnusedParametersRule)
        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawClosedCurve2I(_nativeGraphics, pen.NativePen, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        // according to MSDN fillmode "is required but ignored" which makes _some_ sense since the unmanaged 
        // GDI+ call doesn't support it (issue spotted using Gendarme's AvoidUnusedParametersRule)
        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawClosedCurve2(_nativeGraphics, pen.NativePen, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurveI(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurve(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurve2(_nativeGraphics, pen.NativePen, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurve2I(_nativeGraphics, pen.NativePen, points, points.Length, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurve3(_nativeGraphics, pen.NativePen,
                            points, points.Length, offset,
                            numberOfSegments, 0.5f);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurve3I(_nativeGraphics, pen.NativePen,
                            points, points.Length, offset,
                            numberOfSegments, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            int status;
            status = SafeNativeMethods.Gdip.GdipDrawCurve3(_nativeGraphics, pen.NativePen,
                            points, points.Length, offset,
                            numberOfSegments, tension);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status;
            status = SafeNativeMethods.Gdip.GdipDrawEllipseI(_nativeGraphics, pen.NativePen, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawEllipse(_nativeGraphics, pen.NativePen, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon));

            DrawImage(icon.GetInternalBitmap(), targetRect);
        }

        public void DrawIcon(Icon icon, int x, int y)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon));

            DrawImage(icon.GetInternalBitmap(), x, y);
        }

        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon));

            DrawImageUnscaled(icon.GetInternalBitmap(), targetRect);
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            int status = SafeNativeMethods.Gdip.GdipDrawImageRect(_nativeGraphics, image.nativeImage, rect.X, rect.Y, rect.Width, rect.Height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, PointF point)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            int status = SafeNativeMethods.Gdip.GdipDrawImage(_nativeGraphics, image.nativeImage, point.X, point.Y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Point[] destPoints)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsI(_nativeGraphics, image.nativeImage, destPoints, destPoints.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Point point)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            DrawImage(image, point.X, point.Y);
        }

        public void DrawImage(Image image, Rectangle rect)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawImage(Image image, PointF[] destPoints)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));
            int status = SafeNativeMethods.Gdip.GdipDrawImagePoints(_nativeGraphics, image.nativeImage, destPoints, destPoints.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, int x, int y)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageI(_nativeGraphics, image.nativeImage, x, y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, float x, float y)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImage(_nativeGraphics, image.nativeImage, x, y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRectI(_nativeGraphics, image.nativeImage,
                destRect.X, destRect.Y, destRect.Width, destRect.Height,
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                srcUnit, IntPtr.Zero, null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(_nativeGraphics, image.nativeImage,
                destRect.X, destRect.Y, destRect.Width, destRect.Height,
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                srcUnit, IntPtr.Zero, null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRectI(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero,
                null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRect(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit, IntPtr.Zero,
                null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit,
                                ImageAttributes imageAttr)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRectI(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit,
                imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRect(_nativeGraphics, image.nativeImage, x, y,
                           width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit,
                                ImageAttributes imageAttr)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRect(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit,
                imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointRectI(_nativeGraphics, image.nativeImage, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectI(_nativeGraphics, image.nativeImage, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointRect(_nativeGraphics, image.nativeImage, x, y, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRect(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit,
                imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, callback, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRectI(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit,
                imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, callback, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (destPoints == null)
                throw new ArgumentNullException(nameof(destPoints));

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRectI(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit,
                imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, callback, (IntPtr)callbackData);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(_nativeGraphics, image.nativeImage,
                                destRect.X, destRect.Y, destRect.Width, destRect.Height,
                               srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero,
                               null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData)
        {
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRect(_nativeGraphics, image.nativeImage,
                destPoints, destPoints.Length, srcRect.X, srcRect.Y,
                srcRect.Width, srcRect.Height, srcUnit,
                imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, callback, (IntPtr)callbackData);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRectI(_nativeGraphics, image.nativeImage,
                                destRect.X, destRect.Y, destRect.Width, destRect.Height,
                               srcX, srcY, srcWidth, srcHeight, srcUnit, IntPtr.Zero,
                               null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(_nativeGraphics, image.nativeImage,
                                destRect.X, destRect.Y, destRect.Width, destRect.Height,
                               srcX, srcY, srcWidth, srcHeight, srcUnit,
                imageAttrs != null ? imageAttrs.nativeImageAttributes : IntPtr.Zero, null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRectI(_nativeGraphics, image.nativeImage,
                                        destRect.X, destRect.Y, destRect.Width,
                    destRect.Height, srcX, srcY, srcWidth, srcHeight,
                    srcUnit, imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, null, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRectI(_nativeGraphics, image.nativeImage,
                                        destRect.X, destRect.Y, destRect.Width,
                    destRect.Height, srcX, srcY, srcWidth, srcHeight,
                    srcUnit, imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero, callback,
                    IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(_nativeGraphics, image.nativeImage,
                                        destRect.X, destRect.Y, destRect.Width,
                    destRect.Height, srcX, srcY, srcWidth, srcHeight,
                    srcUnit, imageAttrs != null ? imageAttrs.nativeImageAttributes : IntPtr.Zero,
                    callback, IntPtr.Zero);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(_nativeGraphics, image.nativeImage,
                destRect.X, destRect.Y, destRect.Width, destRect.Height,
                srcX, srcY, srcWidth, srcHeight, srcUnit,
                imageAttrs != null ? imageAttrs.nativeImageAttributes : IntPtr.Zero, callback, callbackData);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(_nativeGraphics, image.nativeImage,
                               destRect.X, destRect.Y, destRect.Width, destRect.Height,
                srcX, srcY, srcWidth, srcHeight, srcUnit,
                imageAttrs != null ? imageAttrs.nativeImageAttributes : IntPtr.Zero, callback, callbackData);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawImageUnscaled(Image image, Point point)
        {
            DrawImageUnscaled(image, point.X, point.Y);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            DrawImageUnscaled(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            DrawImage(image, x, y, image.Width, image.Height);
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            // avoid creating an empty, or negative w/h, bitmap...
            if ((width <= 0) || (height <= 0))
                return;

            using (Image tmpImg = new Bitmap(width, height))
            {
                using (Graphics g = FromImage(tmpImg))
                {
                    g.DrawImage(image, 0, 0, image.Width, image.Height);
                    DrawImage(tmpImg, x, y, width, height);
                }
            }
        }

        public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            int width = (image.Width > rect.Width) ? rect.Width : image.Width;
            int height = (image.Height > rect.Height) ? rect.Height : image.Height;

            DrawImageUnscaled(image, rect.X, rect.Y, width, height);
        }

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawLine(_nativeGraphics, pen.NativePen,
                            pt1.X, pt1.Y, pt2.X, pt2.Y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawLineI(_nativeGraphics, pen.NativePen,
                            pt1.X, pt1.Y, pt2.X, pt2.Y);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawLineI(_nativeGraphics, pen.NativePen, x1, y1, x2, y2);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (!float.IsNaN(x1) && !float.IsNaN(y1) &&
                !float.IsNaN(x2) && !float.IsNaN(y2))
            {
                int status = SafeNativeMethods.Gdip.GdipDrawLine(_nativeGraphics, pen.NativePen, x1, y1, x2, y2);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        public void DrawLines(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipDrawLines(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipDrawLinesI(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            int status = SafeNativeMethods.Gdip.GdipDrawPath(_nativeGraphics, pen.NativePen, path.nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawPie(_nativeGraphics, pen.NativePen, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        // Microsoft documentation states that the signature for this member should be
        // public void DrawPie(Pen pen, int x,  int y,  int width,   int height,   int startAngle
        // int sweepAngle. However, GdipDrawPieI uses also float for the startAngle and sweepAngle params
        public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawPieI(_nativeGraphics, pen.NativePen, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawPolygon(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipDrawPolygonI(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawPolygon(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipDrawPolygon(_nativeGraphics, pen.NativePen, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawRectangle(_nativeGraphics, pen.NativePen, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));
            int status = SafeNativeMethods.Gdip.GdipDrawRectangleI(_nativeGraphics, pen.NativePen, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            if (pen == null)
                throw new ArgumentNullException("image");
            if (rects == null)
                throw new ArgumentNullException(nameof(rects));
            int status = SafeNativeMethods.Gdip.GdipDrawRectangles(_nativeGraphics, pen.NativePen, rects, rects.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            if (pen == null)
                throw new ArgumentNullException("image");
            if (rects == null)
                throw new ArgumentNullException(nameof(rects));
            int status = SafeNativeMethods.Gdip.GdipDrawRectanglesI(_nativeGraphics, pen.NativePen, rects, rects.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle, null);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), null);
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), format);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 0, 0), null);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 0, 0), format);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (s == null || s.Length == 0)
                return;

            int status = SafeNativeMethods.Gdip.GdipDrawString(_nativeGraphics, s, s.Length, font.NativeFont, ref layoutRectangle, format != null ? format.nativeFormat : IntPtr.Zero, brush.NativeBrush);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void EndContainer(GraphicsContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            int status = SafeNativeMethods.Gdip.GdipEndContainer(new HandleRef(this, _nativeGraphics), container.nativeGraphicsContainer);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr)
        {
            throw new NotImplementedException();
        }

        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurve(_nativeGraphics, brush.NativeBrush, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillClosedCurve(Brush brush, Point[] points)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurveI(_nativeGraphics, brush.NativeBrush, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }


        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            FillClosedCurve(brush, points, fillmode, 0.5f);
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            FillClosedCurve(brush, points, fillmode, 0.5f);
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurve2(_nativeGraphics, brush.NativeBrush, points, points.Length, tension, fillmode);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurve2I(_nativeGraphics, brush.NativeBrush, points, points.Length, tension, fillmode);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillEllipse(Brush brush, Rectangle rect)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, RectangleF rect)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            int status = SafeNativeMethods.Gdip.GdipFillEllipse(_nativeGraphics, brush.NativeBrush, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            int status = SafeNativeMethods.Gdip.GdipFillEllipseI(_nativeGraphics, brush.NativeBrush, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            int status = SafeNativeMethods.Gdip.GdipFillPath(_nativeGraphics, brush.NativeBrush, path.nativePath);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            int status = SafeNativeMethods.Gdip.GdipFillPie(_nativeGraphics, brush.NativeBrush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            int status = SafeNativeMethods.Gdip.GdipFillPieI(_nativeGraphics, brush.NativeBrush, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            int status = SafeNativeMethods.Gdip.GdipFillPie(_nativeGraphics, brush.NativeBrush, x, y, width, height, startAngle, sweepAngle);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPolygon(Brush brush, PointF[] points)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillPolygon2(_nativeGraphics, brush.NativeBrush, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPolygon(Brush brush, Point[] points)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillPolygon2I(_nativeGraphics, brush.NativeBrush, points, points.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillPolygonI(_nativeGraphics, brush.NativeBrush, points, points.Length, fillMode);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            int status = SafeNativeMethods.Gdip.GdipFillPolygon(_nativeGraphics, brush.NativeBrush, points, points.Length, fillMode);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, Rectangle rect)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            int status = SafeNativeMethods.Gdip.GdipFillRectangleI(_nativeGraphics, brush.NativeBrush, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            int status = SafeNativeMethods.Gdip.GdipFillRectangle(_nativeGraphics, brush.NativeBrush, x, y, width, height);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (rects == null)
                throw new ArgumentNullException(nameof(rects));

            int status = SafeNativeMethods.Gdip.GdipFillRectanglesI(_nativeGraphics, brush.NativeBrush, rects, rects.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (rects == null)
                throw new ArgumentNullException(nameof(rects));

            int status = SafeNativeMethods.Gdip.GdipFillRectangles(_nativeGraphics, brush.NativeBrush, rects, rects.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }


        public void FillRegion(Brush brush, Region region)
        {
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            int status = (int)SafeNativeMethods.Gdip.GdipFillRegion(new HandleRef(this, _nativeGraphics), new HandleRef(brush, brush.NativeBrush), new HandleRef(region, region._nativeRegion));
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        private void FlushCore() => maccontext?.Synchronize();

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdc(IntPtr hdc)
        {
            IntPtr graphics;
            int status = SafeNativeMethods.Gdip.GdipCreateFromHDC(hdc, out graphics);
            SafeNativeMethods.Gdip.CheckStatus(status);
            return new Graphics(graphics);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdcInternal(IntPtr hdc)
        {
            SafeNativeMethods.Gdip.Display = hdc;
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHwnd(IntPtr hwnd)
        {
            IntPtr graphics;

            if (!SafeNativeMethods.Gdip.UseX11Drawable)
            {
                CarbonContext context = MacSupport.GetCGContextForView(hwnd);
                SafeNativeMethods.Gdip.GdipCreateFromContext_macosx(context.ctx, context.width, context.height, out graphics);

                Graphics g = new Graphics(graphics);
                g.maccontext = context;

                return g;
            }
            else
            {
                if (SafeNativeMethods.Gdip.Display == IntPtr.Zero)
                {
                    SafeNativeMethods.Gdip.Display = LibX11Functions.XOpenDisplay(IntPtr.Zero);
                    if (SafeNativeMethods.Gdip.Display == IntPtr.Zero)
                        throw new NotSupportedException("Could not open display (X-Server required. Check your DISPLAY environment variable)");
                }
                if (hwnd == IntPtr.Zero)
                {
                    hwnd = LibX11Functions.XRootWindow(SafeNativeMethods.Gdip.Display, LibX11Functions.XDefaultScreen(SafeNativeMethods.Gdip.Display));
                }

                return FromXDrawable(hwnd, SafeNativeMethods.Gdip.Display);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHwndInternal(IntPtr hwnd)
        {
            return FromHwnd(hwnd);
        }

        public static Graphics FromImage(Image image)
        {
            IntPtr graphics;

            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if ((image.PixelFormat & PixelFormat.Indexed) != 0)
                throw new ArgumentException("Cannot create Graphics from an indexed bitmap.", nameof(image));            

            int status = SafeNativeMethods.Gdip.GdipGetImageGraphicsContext(image.nativeImage, out graphics);
            SafeNativeMethods.Gdip.CheckStatus(status);
            Graphics result = new Graphics(graphics);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            SafeNativeMethods.Gdip.GdipSetVisibleClip_linux(result.NativeGraphics, ref rect);

            return result;
        }

        internal static Graphics FromXDrawable(IntPtr drawable, IntPtr display)
        {
            IntPtr graphics;

            int s = SafeNativeMethods.Gdip.GdipCreateFromXDrawable_linux(drawable, display, out graphics);
            SafeNativeMethods.Gdip.CheckStatus(s);
            return new Graphics(graphics);
        }

        public static IntPtr GetHalftonePalette()
        {
            throw new NotImplementedException();
        }

        public Color GetNearestColor(Color color)
        {
            int argb;

            int status = SafeNativeMethods.Gdip.GdipGetNearestColor(_nativeGraphics, out argb);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return Color.FromArgb(argb);
        }

        public Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
        {
            if ((text == null) || (text.Length == 0))
                return Array.Empty<Region>();

            if (font == null)
                throw new ArgumentNullException(nameof(font));

            if (stringFormat == null)
                throw new ArgumentException(nameof(stringFormat));

            int regcount = stringFormat.GetMeasurableCharacterRangeCount();
            if (regcount == 0)
                return Array.Empty<Region>();

            IntPtr[] native_regions = new IntPtr[regcount];
            Region[] regions = new Region[regcount];

            for (int i = 0; i < regcount; i++)
            {
                regions[i] = new Region();
                native_regions[i] = regions[i]._nativeRegion;
            }

            int status = SafeNativeMethods.Gdip.GdipMeasureCharacterRanges(_nativeGraphics, text, text.Length,
                font.NativeFont, ref layoutRect, stringFormat.nativeFormat, regcount, out native_regions[0]);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return regions;
        }

        private unsafe SizeF GdipMeasureString(IntPtr graphics, string text, Font font, ref RectangleF layoutRect,
            IntPtr stringFormat)
        {
            if ((text == null) || (text.Length == 0))
                return SizeF.Empty;

            if (font == null)
                throw new ArgumentNullException(nameof(font));

            RectangleF boundingBox = new RectangleF();

            int status = SafeNativeMethods.Gdip.GdipMeasureString(_nativeGraphics, text, text.Length, font.NativeFont,
                ref layoutRect, stringFormat, out boundingBox, null, null);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new SizeF(boundingBox.Width, boundingBox.Height);
        }

        public SizeF MeasureString(string text, Font font)
        {
            return MeasureString(text, font, SizeF.Empty);
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea)
        {
            RectangleF rect = new RectangleF(0, 0, layoutArea.Width, layoutArea.Height);
            return GdipMeasureString(_nativeGraphics, text, font, ref rect, IntPtr.Zero);
        }

        public SizeF MeasureString(string text, Font font, int width)
        {
            RectangleF rect = new RectangleF(0, 0, width, Int32.MaxValue);
            return GdipMeasureString(_nativeGraphics, text, font, ref rect, IntPtr.Zero);
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            RectangleF rect = new RectangleF(0, 0, layoutArea.Width, layoutArea.Height);
            IntPtr format = (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat;
            return GdipMeasureString(_nativeGraphics, text, font, ref rect, format);
        }

        public SizeF MeasureString(string text, Font font, int width, StringFormat format)
        {
            RectangleF rect = new RectangleF(0, 0, width, Int32.MaxValue);
            IntPtr stringFormat = (format == null) ? IntPtr.Zero : format.nativeFormat;
            return GdipMeasureString(_nativeGraphics, text, font, ref rect, stringFormat);
        }

        public SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
        {
            RectangleF rect = new RectangleF(origin.X, origin.Y, 0, 0);
            IntPtr format = (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat;
            return GdipMeasureString(_nativeGraphics, text, font, ref rect, format);
        }

        public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat,
            out int charactersFitted, out int linesFilled)
        {
            charactersFitted = 0;
            linesFilled = 0;

            if ((text == null) || (text.Length == 0))
                return SizeF.Empty;

            if (font == null)
                throw new ArgumentNullException(nameof(font));

            RectangleF boundingBox = new RectangleF();
            RectangleF rect = new RectangleF(0, 0, layoutArea.Width, layoutArea.Height);

            IntPtr format = (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat;

            unsafe
            {
                fixed (int* pc = &charactersFitted, pl = &linesFilled)
                {
                    int status = SafeNativeMethods.Gdip.GdipMeasureString(_nativeGraphics, text, text.Length,
                    font.NativeFont, ref rect, format, out boundingBox, pc, pl);
                    SafeNativeMethods.Gdip.CheckStatus(status);
                }
            }
            return new SizeF(boundingBox.Width, boundingBox.Height);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReleaseHdcInternal(IntPtr hdc)
        {
            int status = SafeNativeMethods.Gdip.InvalidParameter;
            if (hdc == _nativeHdc)
            {
                status = SafeNativeMethods.Gdip.GdipReleaseDC(_nativeGraphics, _nativeHdc);
                _nativeHdc = IntPtr.Zero;
            }
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public void Restore(GraphicsState gstate)
        {
            // the possible NRE thrown by gstate.nativeState match MS behaviour
            int status = SafeNativeMethods.Gdip.GdipRestoreGraphics(_nativeGraphics, (uint)gstate.nativeState);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public GraphicsState Save()
        {
            uint saveState;
            int status = SafeNativeMethods.Gdip.GdipSaveGraphics(_nativeGraphics, out saveState);
            SafeNativeMethods.Gdip.CheckStatus(status);

            GraphicsState state = new GraphicsState((int)saveState);
            return state;
        }


        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));

            IntPtr ptrPt = MarshallingHelpers.FromPointToUnManagedMemory(pts);

            int status = SafeNativeMethods.Gdip.GdipTransformPoints(_nativeGraphics, destSpace, srcSpace, ptrPt, pts.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);

            MarshallingHelpers.FromUnManagedMemoryToPoint(ptrPt, pts);
        }


        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));
            IntPtr ptrPt = MarshallingHelpers.FromPointToUnManagedMemoryI(pts);

            int status = SafeNativeMethods.Gdip.GdipTransformPointsI(_nativeGraphics, destSpace, srcSpace, ptrPt, pts.Length);
            SafeNativeMethods.Gdip.CheckStatus(status);

            MarshallingHelpers.FromUnManagedMemoryToPointI(ptrPt, pts);
        }

        public RectangleF VisibleClipBounds
        {
            get
            {
                var rect = new GPRECTF();
                int status = SafeNativeMethods.Gdip.GdipGetVisibleClipBounds(new HandleRef(this, NativeGraphics), ref rect);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return rect.ToRectangleF();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetContextInfo()
        {
            // only known source of information @ http://blogs.wdevs.com/jdunlap/Default.aspx
            throw new NotImplementedException();
        }
    }
}
