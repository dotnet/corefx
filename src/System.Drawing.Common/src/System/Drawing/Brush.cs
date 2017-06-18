// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /**
     * Represent a Brush object
     */
    /// <include file='doc\Brush.uex' path='docs/doc[@for="Brush"]/*' />
    /// <devdoc>
    ///     <para>
    ///         Classes derrived from this abstract base class define objects used to fill the 
    ///         interiors of graphical shapes such as rectangles, ellipses, pies, polygons, and paths.
    ///     </para>
    /// </devdoc>
    public abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
    {
#if FINALIZATION_WATCH
        private string allocationSite = Graphics.GetAllocationStack();
#endif
        // Handle to native GDI+ brush object to be used on demand.
        private IntPtr _nativeBrush;

        public abstract object Clone();

        protected internal void SetNativeBrush(IntPtr brush) => SetNativeBrushInternal(brush);
        internal void SetNativeBrushInternal(IntPtr brush) => _nativeBrush = brush;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        internal IntPtr NativeBrush => _nativeBrush;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
#if FINALIZATION_WATCH
            if (!disposing && nativeBrush != IntPtr.Zero )
                Debug.WriteLine("**********************\nDisposed through finalization:\n" + allocationSite);
#endif

            if (_nativeBrush != IntPtr.Zero)
            {
                try
                {
#if DEBUG
                    int status =
#endif
                    SafeNativeMethods.Gdip.GdipDeleteBrush(new HandleRef(this, _nativeBrush));
#if DEBUG
                    Debug.Assert(status == SafeNativeMethods.Gdip.Ok, "GDI+ returned an error status: " + status.ToString(CultureInfo.InvariantCulture));
#endif
                }
                finally
                {
                    _nativeBrush = IntPtr.Zero;
                }
            }
        }

        ~Brush() => Dispose(false);
    }
}
