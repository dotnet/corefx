// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Internal
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///     <para>
    ///         Encapsulates a GDI Region object.
    ///     </para>
    /// </devdoc>

    internal sealed partial class WindowsRegion : MarshalByRefObject, ICloneable, IDisposable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private IntPtr _nativeHandle; // The hRegion, this class always takes ownership of the hRegion.
        private bool _ownHandle;

#if GDI_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        /// <devdoc>
        /// </devdoc>
        private WindowsRegion()
        {
        }

        /// <devdoc>
        /// </devdoc>
        public WindowsRegion(Rectangle rect)
        {
            CreateRegion(rect);
        }

        /// <devdoc>
        /// </devdoc>
        public WindowsRegion(int x, int y, int width, int height)
        {
            CreateRegion(new Rectangle(x, y, width, height));
        }

        // Consider implementing a constructor that calls ExtCreateRegion(XFORM lpXform, DWORD nCount, RGNDATA lpRgnData) if needed.

        /// <devdoc>
        ///     Creates a WindowsRegion from a region handle, if 'takeOwnership' is true, the handle is added to the HandleCollector
        ///     and is removed & destroyed on dispose. 
        /// </devdoc>
        public static WindowsRegion FromHregion(IntPtr hRegion, bool takeOwnership)
        {
            WindowsRegion wr = new WindowsRegion();

            // Note: Passing IntPtr.Zero for hRegion is ok.  GDI+ infinite regions will have hRegion == null.  
            // GDI's SelectClipRgn interprets null region handle as resetting the clip region (all region will be available for painting).
            if (hRegion != IntPtr.Zero)
            {
                wr._nativeHandle = hRegion;

                if (takeOwnership)
                {
                    wr._ownHandle = true;
                    System.Internal.HandleCollector.Add(hRegion, IntSafeNativeMethods.CommonHandles.GDI);
                }
            }
            return wr;
        }

        /// <devdoc>
        ///     Creates a WindowsRegion from a System.Drawing.Region. 
        /// </devdoc>
        public static WindowsRegion FromRegion(Region region, Graphics g)
        {
            if (region.IsInfinite(g))
            {
                // An infinite region would cover the entire device region which is the same as
                // not having a clipping region. Observe that this is not the same as having an
                // empty region, which when clipping to it has the effect of excluding the entire
                // device region.
                // To remove the clip region from a dc the SelectClipRgn() function needs to be
                // called with a null region ptr - that's why we use the empty constructor here.
                // GDI+ will return IntPtr.Zero for Region.GetHrgn(Graphics) when the region is
                // Infinite.
                return new WindowsRegion();
            }

            return WindowsRegion.FromHregion(region.GetHrgn(g), true);
        }

        /// <devdoc>
        /// </devdoc>
        public object Clone()
        {
            // WARNING: WindowsRegion currently supports rectangulare regions only, if the WindowsRegion was created
            //          from an HRegion and it is not rectangular this method won't work as expected.
            // Note:    This method is currently not used and is here just to implement ICloneable.
            return IsInfinite ?
                new WindowsRegion() :
                new WindowsRegion(ToRectangle());
        }

        /// <devdoc>
        ///     Combines region1 & region2 into this region.   The regions cannot be null. 
        ///     The three regions need not be distinct. For example, the sourceRgn1 can equal this region. 
        /// </devdoc>
        public IntNativeMethods.RegionFlags CombineRegion(WindowsRegion region1, WindowsRegion region2, RegionCombineMode mode)
        {
            return IntUnsafeNativeMethods.CombineRgn(new HandleRef(this, HRegion), new HandleRef(region1, region1.HRegion), new HandleRef(region2, region2.HRegion), mode);
        }

        /// <devdoc>
        /// </devdoc>
        private void CreateRegion(Rectangle rect)
        {
            Debug.Assert(_nativeHandle == IntPtr.Zero, "nativeHandle should be null, we're leaking handle");
            _nativeHandle = IntSafeNativeMethods.CreateRectRgn(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
            _ownHandle = true;
        }

        /// <devdoc>
        /// </devdoc>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <devdoc>
        /// </devdoc>
        public void Dispose(bool disposing)
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                DbgUtil.AssertFinalization(this, disposing);

                if (_ownHandle)
                {
                    IntUnsafeNativeMethods.DeleteObject(new HandleRef(this, _nativeHandle));
                }

                _nativeHandle = IntPtr.Zero;

                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <devdoc>
        /// </devdoc>
        ~WindowsRegion()
        {
            Dispose(false);
        }

        /// <devdoc>
        ///     The native region handle. 
        /// </devdoc>
        public IntPtr HRegion
        {
            get
            {
                return _nativeHandle;
            }
        }

        /// <devdoc>
        /// </devdoc>
        public bool IsInfinite
        {
            get
            {
                return _nativeHandle == IntPtr.Zero;
            }
        }

        /// <devdoc>
        ///     A rectangle representing the window region set with the SetWindowRgn function. 
        /// </devdoc>
        public Rectangle ToRectangle()
        {
            if (IsInfinite)
            {
                return new Rectangle(-Int32.MaxValue, -Int32.MaxValue, Int32.MaxValue, Int32.MaxValue);
            }

            IntNativeMethods.RECT rect = new IntNativeMethods.RECT();
            IntUnsafeNativeMethods.GetRgnBox(new HandleRef(this, _nativeHandle), ref rect);
            return new Rectangle(new Point(rect.left, rect.top), rect.Size);
        }
    }
}

