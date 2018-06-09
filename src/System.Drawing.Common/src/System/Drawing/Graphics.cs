// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Internal;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    /// <summary>
    /// Encapsulates a GDI+ drawing surface.
    /// </summary>
    public sealed partial class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
    {
        /// <summary>
        /// Handle to native DC - obtained from the GDI+ graphics object. We need to cache it to implement
        /// IDeviceContext interface.
        /// </summary>
        private IntPtr _nativeHdc;

        public IntPtr GetHdc()
        {
            IntPtr hdc = IntPtr.Zero;
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipGetDC(new HandleRef(this, NativeGraphics), out hdc));

            _nativeHdc = hdc; // need to cache the hdc to be able to release with a call to IDeviceContext.ReleaseHdc().
            return _nativeHdc;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void ReleaseHdc(IntPtr hdc) => ReleaseHdcInternal(hdc);

        public void ReleaseHdc() => ReleaseHdcInternal(_nativeHdc);

        /// <summary>
        /// Forces immediate execution of all operations currently on the stack.
        /// </summary>
        public void Flush() => Flush(FlushIntention.Flush);

        /// <summary>
        /// Forces execution of all operations currently on the stack.
        /// </summary>
        public void Flush(FlushIntention intention)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipFlush(new HandleRef(this, NativeGraphics), intention));
            FlushCore();
        }

        public void SetClip(Graphics g) => SetClip(g, CombineMode.Replace);

        public void SetClip(Graphics g, CombineMode combineMode)
        {
            if (g == null)
                throw new ArgumentNullException(nameof(g));

            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipGraphics(
                new HandleRef(this, NativeGraphics),
                new HandleRef(g, g.NativeGraphics),
                combineMode));
        }

        public void SetClip(Rectangle rect) => SetClip(rect, CombineMode.Replace);

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRectI(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                combineMode));
        }

        public void SetClip(RectangleF rect) => SetClip(rect, CombineMode.Replace);

        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRect(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                combineMode));
        }

        public void SetClip(GraphicsPath path) => SetClip(path, CombineMode.Replace);

        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipPath(
                new HandleRef(this, NativeGraphics),
                new HandleRef(path, path._nativePath),
                combineMode));
        }

        public void SetClip(Region region, CombineMode combineMode)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRegion(
                new HandleRef(this, NativeGraphics),
                new HandleRef(region, region._nativeRegion),
                combineMode));
        }

        public void IntersectClip(Rectangle rect)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRectI(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                CombineMode.Intersect));
        }

        public void IntersectClip(RectangleF rect)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRect(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                CombineMode.Intersect));
        }

        public void IntersectClip(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRegion(
                new HandleRef(this, NativeGraphics),
                new HandleRef(region, region._nativeRegion),
                CombineMode.Intersect));
        }

        public void ExcludeClip(Rectangle rect)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRectI(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                CombineMode.Exclude));
        }

        public void ExcludeClip(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetClipRegion(
                new HandleRef(this, NativeGraphics),
                new HandleRef(region, region._nativeRegion),
                CombineMode.Exclude));
        }

        public void ResetClip()
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipResetClip(new HandleRef(this, NativeGraphics)));
        }

        public void TranslateClip(float dx, float dy)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipTranslateClip(new HandleRef(this, NativeGraphics), dx, dy));
        }

        public void TranslateClip(int dx, int dy)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipTranslateClip(new HandleRef(this, NativeGraphics), dx, dy));
        }

        public Region Clip
        {
            get
            {
                var region = new Region();
                SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipGetClip(
                    new HandleRef(this, NativeGraphics),
                    new HandleRef(region, region._nativeRegion)));

                return region;
            }
            set => SetClip(value, CombineMode.Replace);
        }

        public RectangleF ClipBounds
        {
            get
            {
                var rect = new GPRECTF();
                SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipGetClipBounds(new HandleRef(this, NativeGraphics), ref rect));

                return rect.ToRectangleF();
            }
        }

        public bool IsClipEmpty
        {
            get
            {
                SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipIsClipEmpty(new HandleRef(this, NativeGraphics), out int isEmpty));

                return isEmpty != 0;
            }
        }

        public bool IsVisibleClipEmpty
        {
            get
            {
                SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipIsVisibleClipEmpty(new HandleRef(this, NativeGraphics), out int isEmpty));

                return isEmpty != 0;
            }
        }

        public bool IsVisible(int x, int y) => IsVisible(new Point(x, y));

        public bool IsVisible(Point point)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipIsVisiblePointI(
                new HandleRef(this, NativeGraphics),
                point.X, point.Y,
                out int isVisible));

            return isVisible != 0;
        }

        public bool IsVisible(float x, float y) => IsVisible(new PointF(x, y));

        public bool IsVisible(PointF point)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipIsVisiblePoint(
                new HandleRef(this, NativeGraphics),
                point.X, point.Y,
                out int isVisible));

            return isVisible != 0;
        }

        public bool IsVisible(int x, int y, int width, int height)
        {
            return IsVisible(new Rectangle(x, y, width, height));
        }

        public bool IsVisible(Rectangle rect)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipIsVisibleRectI(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                out int isVisible));

            return isVisible != 0;
        }

        public bool IsVisible(float x, float y, float width, float height)
        {
            return IsVisible(new RectangleF(x, y, width, height));
        }

        public bool IsVisible(RectangleF rect)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipIsVisibleRect(
                new HandleRef(this, NativeGraphics),
                rect.X, rect.Y, rect.Width, rect.Height,
                out int isVisible));

            return isVisible != 0;
        }

        /// <summary>
        /// Gets or sets the world transform for this <see cref='Graphics'/>.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                var matrix = new Matrix();
                SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipGetWorldTransform(
                    new HandleRef(this, NativeGraphics),
                    new HandleRef(matrix, matrix.nativeMatrix)));

                return matrix;
            }
            set
            {
                SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipSetWorldTransform(
                    new HandleRef(this, NativeGraphics),
                    new HandleRef(value, value.nativeMatrix)));
            }
        }

        /// <summary>
        /// Resets the world transform to identity.
        /// </summary>
        public void ResetTransform()
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipResetWorldTransform(new HandleRef(this, NativeGraphics)));
        }

        /// <summary>
        /// Multiplies the <see cref='Matrix'/> that represents the world transform and <paramref name="matrix"/>.
        /// </summary>
        public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

        /// <summary>
        /// Multiplies the <see cref='Matrix'/> that represents the world transform and <paramref name="matrix"/>.
        /// </summary>
        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipMultiplyWorldTransform(
                new HandleRef(this, NativeGraphics),
                new HandleRef(matrix, matrix.nativeMatrix),
                order));
        }

        public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipTranslateWorldTransform(new HandleRef(this, NativeGraphics), dx, dy, order));
        }

        public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipScaleWorldTransform(new HandleRef(this, NativeGraphics), sx, sy, order));
        }

        public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

        public void RotateTransform(float angle, MatrixOrder order)
        {
            SafeNativeMethods.Gdip.CheckStatus(SafeNativeMethods.Gdip.GdipRotateWorldTransform(new HandleRef(this, NativeGraphics), angle, order));
        }
    }
}
