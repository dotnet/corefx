// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Drawing
{
    /// <summary>
    /// Encapsulates a GDI+ drawing surface.
    /// </summary>
    public sealed class Graphics : MarshalByRefObject, IDisposable, IDeviceContext
    {
#if FINALIZATION_WATCH
        static readonly TraceSwitch GraphicsFinalization = new TraceSwitch("GraphicsFinalization", "Tracks the creation and destruction of finalization");
        internal static string GetAllocationStack() {
            if (GraphicsFinalization.TraceVerbose) {
                return Environment.StackTrace;
            }
            else {
                return "Enabled 'GraphicsFinalization' switch to see stack of allocation";
            }
        }
        private string allocationSite = Graphics.GetAllocationStack();
#endif

        /// <summary>
        /// The context state previous to the current Graphics context (the head of the stack).
        /// We don't keep a GraphicsContext for the current context since it is available at any time from GDI+ and
        /// we don't want to keep track of changes in it.
        /// </summary>
        private GraphicsContext _previousContext;

        private static readonly object s_syncObject = new Object();

        /// <summary>
        /// Handle to native GDI+ graphics object.  This object is created on demand.
        /// </summary>
        private IntPtr _nativeGraphics;

        /// <summary>
        /// Handle to native DC - obtained from the GDI+ graphics object. We need to cache it to implement
        /// IDeviceContext interface.
        /// </summary>
        private IntPtr _nativeHdc;

        // Object reference used for printing; it could point to a PrintPreviewGraphics to obtain the VisibleClipBounds, or 
        // a DeviceContext holding a printer DC.
        private object _printingHelper;

        // GDI+'s preferred HPALETTE.
        private static IntPtr s_halftonePalette;

        // pointer back to the Image backing a specific graphic object
        private Image _backingImage;

        /// <include file='doc\Graphics.uex' path='docs/doc[@for="Graphics.DrawImageAbort"]/*' />
        /// <summary>
        /// </summary>
        public delegate bool DrawImageAbort(IntPtr callbackdata);

        // Callback for EnumerateMetafile methods.  The parameters are:

        // recordType      (if >= MinRecordType, it's an EMF+ record)
        // flags           (always 0 for EMF records)
        // dataSize        size of the data, or 0 if no data
        // data            pointer to the data, or NULL if no data (UINT32 aligned)
        // callbackData    pointer to callbackData, if any

        // This method can then call Metafile.PlayRecord to play the
        // record that was just enumerated.  If this method  returns
        // FALSE, the enumeration process is aborted.  Otherwise, it continues.        

        public delegate bool EnumerateMetafileProc(EmfPlusRecordType recordType,
                                                   int flags,
                                                   int dataSize,
                                                   IntPtr data,
                                                   PlayRecordCallback callbackData);


        /// <summary>
        /// Constructor to initialize this object from a native GDI+ Graphics pointer.
        /// </summary>
        private Graphics(IntPtr gdipNativeGraphics)
        {
            if (gdipNativeGraphics == IntPtr.Zero)
            {
                throw new ArgumentNullException("gdipNativeGraphics");
            }
            _nativeGraphics = gdipNativeGraphics;
        }

        /// <summary>
        /// Creates a new instance of the <see cref='Graphics'/> class from the specified handle to a device context.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdc(IntPtr hdc)
        {
            if (hdc == IntPtr.Zero)
            {
                throw new ArgumentNullException("hdc");
            }

            return FromHdcInternal(hdc);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdcInternal(IntPtr hdc)
        {
            IntPtr nativeGraphics = IntPtr.Zero;

            int status = SafeNativeMethods.Gdip.GdipCreateFromHDC(new HandleRef(null, hdc), out nativeGraphics);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return new Graphics(nativeGraphics);
        }

        /// <summary>
        /// Creates a new instance of the Graphics class from the specified handle to a device context and handle to a device.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
        {
            IntPtr gdipNativeGraphics = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCreateFromHDC2(new HandleRef(null, hdc), new HandleRef(null, hdevice), out gdipNativeGraphics);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return new Graphics(gdipNativeGraphics);
        }

        /// <summary>
        /// Creates a new instance of the <see cref='Graphics'/> class from a window handle.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHwnd(IntPtr hwnd)
        {
            return FromHwndInternal(hwnd);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Graphics FromHwndInternal(IntPtr hwnd)
        {
            IntPtr nativeGraphics = IntPtr.Zero;

            int status = SafeNativeMethods.Gdip.GdipCreateFromHWND(new HandleRef(null, hwnd), out nativeGraphics);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return new Graphics(nativeGraphics);
        }

        /// <summary>
        /// Creates an instance of the <see cref='Graphics'/> class from an existing <see cref='Image'/>.
        /// </summary>
        public static Graphics FromImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if ((image.PixelFormat & PixelFormat.Indexed) != 0)
            {
                throw new Exception(SR.Format(SR.GdiplusCannotCreateGraphicsFromIndexedPixelFormat));
            }
            Contract.Ensures(Contract.Result<Graphics>() != null);

            IntPtr gdipNativeGraphics = IntPtr.Zero;

            int status = SafeNativeMethods.Gdip.GdipGetImageGraphicsContext(new HandleRef(image, image.nativeImage),
                                                                            out gdipNativeGraphics);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            Graphics result = new Graphics(gdipNativeGraphics);
            result._backingImage = image;
            return result;
        }


        internal IntPtr NativeGraphics => _nativeGraphics;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public IntPtr GetHdc()
        {
            IntPtr hdc = IntPtr.Zero;

            int status = SafeNativeMethods.Gdip.GdipGetDC(new HandleRef(this, NativeGraphics), out hdc);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            _nativeHdc = hdc; // need to cache the hdc to be able to release with a call to IDeviceContext.ReleaseHdc().

            return _nativeHdc;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void ReleaseHdc(IntPtr hdc) => ReleaseHdcInternal(hdc);

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void ReleaseHdc() => ReleaseHdcInternal(_nativeHdc);

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReleaseHdcInternal(IntPtr hdc)
        {
            int status = SafeNativeMethods.Gdip.GdipReleaseDC(new HandleRef(this, NativeGraphics), new HandleRef(null, hdc));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            _nativeHdc = IntPtr.Zero;
        }

        /// <summary>
        /// Deletes this <see cref='Graphics'/>, and frees the memory allocated for it.
        /// </summary>        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
#if DEBUG
            if (!disposing && _nativeGraphics != IntPtr.Zero)
            {
                // Recompile commonUI\\system\\Drawing\\Graphics.cs with FINALIZATION_WATCH on to find who allocated it.
#if FINALIZATION_WATCH
                //Debug.Fail("Graphics object Disposed through finalization:\n" + allocationSite);
                Debug.WriteLine("System.Drawing.Graphics: ***************************************************");
                Debug.WriteLine("System.Drawing.Graphics: Object Disposed through finalization:\n" + allocationSite);
#else
                //Debug.Fail("A Graphics object was not Dispose()'d.  Please make sure it's not your code that should be calling Dispose().");
#endif
            }
#endif
            while (_previousContext != null)
            {
                // Dispose entire stack.
                GraphicsContext context = _previousContext.Previous;
                _previousContext.Dispose();
                _previousContext = context;
            }

            if (_nativeGraphics != IntPtr.Zero)
            {
                try
                {
                    if (_nativeHdc != IntPtr.Zero) // avoid a handle leak.
                    {
                        ReleaseHdc();
                    }

                    if (PrintingHelper != null)
                    {
                        DeviceContext printerDC = PrintingHelper as DeviceContext;

                        if (printerDC != null)
                        {
                            printerDC.Dispose();
                            _printingHelper = null;
                        }
                    }

#if DEBUG
                    int status =
#endif
                    SafeNativeMethods.Gdip.GdipDeleteGraphics(new HandleRef(this, _nativeGraphics));

#if DEBUG
                    Debug.Assert(status == SafeNativeMethods.Gdip.Ok, "GDI+ returned an error status: " + status.ToString(CultureInfo.InvariantCulture));
#endif
                }
                catch (Exception ex)  // do not allow exceptions to propagate during disposing.
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }
                    Debug.Fail("Exception thrown during disposing: \r\n" + ex.ToString());
                }
                finally
                {
                    _nativeGraphics = IntPtr.Zero;
                }
            }
        }

        ~Graphics()
        {
            Dispose(false);
        }

        /// <summary>
        /// Forces immediate execution of all operations currently on the stack.
        /// </summary>
        public void Flush()
        {
            Flush(FlushIntention.Flush);
        }

        /// <summary>
        /// Forces execution of all operations currently on the stack.
        /// </summary>
        public void Flush(FlushIntention intention)
        {
            int status = SafeNativeMethods.Gdip.GdipFlush(new HandleRef(this, NativeGraphics), intention);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }


        /// <summary>
        /// Gets or sets the <see cref='Drawing2D.CompositingMode'/> associated with this <see cref='Graphics'/>.
        /// </summary>
        public CompositingMode CompositingMode
        {
            get
            {
                int mode = 0;

                int status = SafeNativeMethods.Gdip.GdipGetCompositingMode(new HandleRef(this, NativeGraphics), out mode);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return (CompositingMode)mode;
            }
            set
            {
                //validate the enum value
                //valid values are 0x0 to 0x1
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), (int)CompositingMode.SourceOver, (int)CompositingMode.SourceCopy))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(CompositingMode));
                }

                int status = SafeNativeMethods.Gdip.GdipSetCompositingMode(new HandleRef(this, NativeGraphics), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public Point RenderingOrigin
        {
            get
            {
                int x, y;

                int status = SafeNativeMethods.Gdip.GdipGetRenderingOrigin(new HandleRef(this, NativeGraphics), out x, out y);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return new Point(x, y);
            }
            set
            {
                int status = SafeNativeMethods.Gdip.GdipSetRenderingOrigin(new HandleRef(this, NativeGraphics), value.X, value.Y);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public CompositingQuality CompositingQuality
        {
            get
            {
                CompositingQuality cq;

                int status = SafeNativeMethods.Gdip.GdipGetCompositingQuality(new HandleRef(this, NativeGraphics), out cq);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return cq;
            }
            set
            {
                //valid values are 0xffffffff to 0x4
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), unchecked((int)CompositingQuality.Invalid), unchecked((int)CompositingQuality.AssumeLinear)))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(CompositingQuality));
                }

                int status = SafeNativeMethods.Gdip.GdipSetCompositingQuality(new HandleRef(this, NativeGraphics),
                                                               value);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        /// <summary>
        /// Gets or sets the rendering mode for text associated with this <see cref='Graphics'/>.
        /// </summary>
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                TextRenderingHint hint = 0;

                int status = SafeNativeMethods.Gdip.GdipGetTextRenderingHint(new HandleRef(this, NativeGraphics), out hint);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return hint;
            }
            set
            {
                //valid values are 0x0 to 0x5
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), (int)TextRenderingHint.SystemDefault, unchecked((int)TextRenderingHint.ClearTypeGridFit)))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(TextRenderingHint));
                }

                int status = SafeNativeMethods.Gdip.GdipSetTextRenderingHint(new HandleRef(this, NativeGraphics), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public int TextContrast
        {
            get
            {
                int tgv = 0;

                int status = SafeNativeMethods.Gdip.GdipGetTextContrast(new HandleRef(this, NativeGraphics), out tgv);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return tgv;
            }
            set
            {
                int status = SafeNativeMethods.Gdip.GdipSetTextContrast(new HandleRef(this, NativeGraphics), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public SmoothingMode SmoothingMode
        {
            get
            {
                SmoothingMode mode = 0;

                int status = SafeNativeMethods.Gdip.GdipGetSmoothingMode(new HandleRef(this, NativeGraphics), out mode);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return mode;
            }
            set
            {
                //valid values are 0xffffffff to 0x4
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), unchecked((int)SmoothingMode.Invalid), unchecked((int)SmoothingMode.AntiAlias)))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(SmoothingMode));
                }

                int status = SafeNativeMethods.Gdip.GdipSetSmoothingMode(new HandleRef(this, NativeGraphics), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public PixelOffsetMode PixelOffsetMode
        {
            get
            {
                PixelOffsetMode mode = 0;

                int status = SafeNativeMethods.Gdip.GdipGetPixelOffsetMode(new HandleRef(this, NativeGraphics), out mode);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return mode;
            }
            set
            {
                //valid values are 0xffffffff to 0x4
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), unchecked((int)PixelOffsetMode.Invalid), unchecked((int)PixelOffsetMode.Half)))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(PixelOffsetMode));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPixelOffsetMode(new HandleRef(this, NativeGraphics), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        /// <summary>
        /// Represents an object used in conection with the printing API, it is used to hold a reference to a
        /// PrintPreviewGraphics (fake graphics) or a printer DeviceContext (and maybe more in the future).
        /// </summary>
        internal object PrintingHelper
        {
            get
            {
                return _printingHelper;
            }
            set
            {
                Debug.Assert(_printingHelper == null, "WARNING: Overwritting the printing helper reference!");
                _printingHelper = value;
            }
        }

        /// <summary>
        /// Gets or sets the interpolation mode associated with this Graphics.
        /// </summary>
        public InterpolationMode InterpolationMode
        {
            get
            {
                int mode = 0;

                int status = SafeNativeMethods.Gdip.GdipGetInterpolationMode(new HandleRef(this, NativeGraphics), out mode);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return (InterpolationMode)mode;
            }
            set
            {
                //validate the enum value
                //valid values are 0xffffffff to 0x7
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), unchecked((int)InterpolationMode.Invalid), unchecked((int)InterpolationMode.HighQualityBicubic)))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(InterpolationMode));
                }

                int status = SafeNativeMethods.Gdip.GdipSetInterpolationMode(new HandleRef(this, NativeGraphics), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        /// <summary>
        /// Gets or sets the world transform for this <see cref='Graphics'/>.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                Matrix matrix = new Matrix();

                int status = SafeNativeMethods.Gdip.GdipGetWorldTransform(new HandleRef(this, NativeGraphics),
                                                           new HandleRef(matrix, matrix.nativeMatrix));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return matrix;
            }
            set
            {
                int status = SafeNativeMethods.Gdip.GdipSetWorldTransform(new HandleRef(this, NativeGraphics),
                                                           new HandleRef(value, value.nativeMatrix));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }


        public GraphicsUnit PageUnit
        {
            get
            {
                int unit = 0;

                int status = SafeNativeMethods.Gdip.GdipGetPageUnit(new HandleRef(this, NativeGraphics), out unit);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return (GraphicsUnit)unit;
            }
            set
            {
                //valid values are 0x0 to 0x6
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), (int)GraphicsUnit.World, (int)GraphicsUnit.Millimeter))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(GraphicsUnit));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPageUnit(new HandleRef(this, NativeGraphics), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public float PageScale
        {
            get
            {
                float[] scale = new float[] { 0.0f };

                int status = SafeNativeMethods.Gdip.GdipGetPageScale(new HandleRef(this, NativeGraphics), scale);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return scale[0];
            }

            set
            {
                int status = SafeNativeMethods.Gdip.GdipSetPageScale(new HandleRef(this, NativeGraphics), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }
            }
        }

        public float DpiX
        {
            get
            {
                float[] dpi = new float[] { 0.0f };

                int status = SafeNativeMethods.Gdip.GdipGetDpiX(new HandleRef(this, NativeGraphics), dpi);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return dpi[0];
            }
        }

        public float DpiY
        {
            get
            {
                float[] dpi = new float[] { 0.0f };

                int status = SafeNativeMethods.Gdip.GdipGetDpiY(new HandleRef(this, NativeGraphics), dpi);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return dpi[0];
            }
        }


        /// <summary>
        /// CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size.
        /// </summary>
        public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize)
        {
            CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize);
        }

        /// <summary>
        /// CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size.
        /// </summary>
        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
        {
            CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);
        }

        /// <summary>
        /// CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size
        /// and specified raster operation.
        /// </summary>
        public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
            CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);
        }

        /// <summary>
        /// CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size
        /// and specified raster operation.
        /// </summary>        
        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
        {
            switch (copyPixelOperation)
            {
                case CopyPixelOperation.Blackness:
                case CopyPixelOperation.NotSourceErase:
                case CopyPixelOperation.NotSourceCopy:
                case CopyPixelOperation.SourceErase:
                case CopyPixelOperation.DestinationInvert:
                case CopyPixelOperation.PatInvert:
                case CopyPixelOperation.SourceInvert:
                case CopyPixelOperation.SourceAnd:
                case CopyPixelOperation.MergePaint:
                case CopyPixelOperation.MergeCopy:
                case CopyPixelOperation.SourceCopy:
                case CopyPixelOperation.SourcePaint:
                case CopyPixelOperation.PatCopy:
                case CopyPixelOperation.PatPaint:
                case CopyPixelOperation.Whiteness:
                case CopyPixelOperation.CaptureBlt:
                case CopyPixelOperation.NoMirrorBitmap:
                    break;
                default:
                    throw new InvalidEnumArgumentException("value", unchecked((int)copyPixelOperation), typeof(CopyPixelOperation));
            }

            int destWidth = blockRegionSize.Width;
            int destHeight = blockRegionSize.Height;

            using (DeviceContext dc = DeviceContext.FromHwnd(IntPtr.Zero))
            {  // screen DC
                HandleRef screenDC = new HandleRef(null, dc.Hdc);
                HandleRef targetDC = new HandleRef(null, GetHdc());      // this DC

                try
                {
                    int result = SafeNativeMethods.BitBlt(targetDC, destinationX, destinationY, destWidth, destHeight,
                                                          screenDC, sourceX, sourceY, unchecked((int)copyPixelOperation));

                    //a zero result indicates a win32 exception has been thrown
                    if (result == 0)
                    {
                        throw new Win32Exception();
                    }
                }
                finally
                {
                    ReleaseHdc();
                }
            }
        }

        /// <summary>
        /// Resets the world transform to identity.
        /// </summary>
        public void ResetTransform()
        {
            int status = SafeNativeMethods.Gdip.GdipResetWorldTransform(new HandleRef(this, NativeGraphics));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Multiplies the <see cref='Matrix'/> that represents the world transform and <paramref name="matrix"/>.
        /// </summary>
        public void MultiplyTransform(Matrix matrix)
        {
            MultiplyTransform(matrix, MatrixOrder.Prepend);
        }

        /// <summary>
        /// Multiplies the <see cref='Matrix'/> that represents the world transform and <paramref name="matrix"/>.
        /// </summary>
        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }

            int status = SafeNativeMethods.Gdip.GdipMultiplyWorldTransform(new HandleRef(this, NativeGraphics),
                                                            new HandleRef(matrix, matrix.nativeMatrix),
                                                            order);
            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }

        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipTranslateWorldTransform(new HandleRef(this, NativeGraphics), dx, dy, order);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void ScaleTransform(float sx, float sy)
        {
            ScaleTransform(sx, sy, MatrixOrder.Prepend);
        }

        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipScaleWorldTransform(new HandleRef(this, NativeGraphics), sx, sy, order);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void RotateTransform(float angle)
        {
            RotateTransform(angle, MatrixOrder.Prepend);
        }

        public void RotateTransform(float angle, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipRotateWorldTransform(new HandleRef(this, NativeGraphics), angle, order);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void TransformPoints(CoordinateSpace destSpace,
                                     CoordinateSpace srcSpace,
                                     PointF[] pts)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(pts);
            int status = SafeNativeMethods.Gdip.GdipTransformPoints(new HandleRef(this, NativeGraphics), unchecked((int)destSpace),
                                                     unchecked((int)srcSpace), buf, pts.Length);

            try
            {
                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                // must do an in-place copy because we only have a reference
                PointF[] newPts = SafeNativeMethods.Gdip.ConvertGPPOINTFArrayF(buf, pts.Length);

                for (int i = 0; i < pts.Length; i++)
                {
                    pts[i] = newPts[i];
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buf);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void TransformPoints(CoordinateSpace destSpace,
                                    CoordinateSpace srcSpace,
                                    Point[] pts)
        {
            if (pts == null)
            {
                throw new ArgumentNullException("pts");
            }

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(pts);
            int status = SafeNativeMethods.Gdip.GdipTransformPointsI(new HandleRef(this, NativeGraphics), unchecked((int)destSpace),
                                                      unchecked((int)srcSpace), buf, pts.Length);

            try
            {
                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                Point[] newPts = SafeNativeMethods.Gdip.ConvertGPPOINTArray(buf, pts.Length);

                for (int i = 0; i < pts.Length; i++)
                {
                    pts[i] = newPts[i];
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buf);
            }
        }

        public Color GetNearestColor(Color color)
        {
            int nearest = color.ToArgb();

            int status = SafeNativeMethods.Gdip.GdipGetNearestColor(new HandleRef(this, NativeGraphics), ref nearest);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return Color.FromArgb(nearest);
        }

        /// <summary>
        /// Draws a line connecting the two specified points.
        /// </summary>
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawLine(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x1, y1, x2, y2);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a line connecting the two specified points.
        /// </summary>
        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// Draws a series of line segments that connect an array of points.
        /// </summary>
        public void DrawLines(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawLines(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                               new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }


        /// <summary>
        /// Draws a line connecting the two specified points.
        /// </summary>
        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawLineI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x1, y1, x2, y2);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a line connecting the two specified points.
        /// </summary>
        public void DrawLine(Pen pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// Draws a series of line segments that connect an array of points.
        /// </summary>
        public void DrawLines(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawLinesI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws an arc from the specified ellipse.
        /// </summary>
        public void DrawArc(Pen pen, float x, float y, float width, float height,
                            float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawArc(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y,
                                             width, height, startAngle, sweepAngle);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws an arc from the specified ellipse.
        /// </summary>
        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws an arc from the specified ellipse.
        /// </summary>
        public void DrawArc(Pen pen, int x, int y, int width, int height,
                            int startAngle, int sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawArcI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y,
                                              width, height, startAngle, sweepAngle);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws an arc from the specified ellipse.
        /// </summary>
        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a cubic bezier curve defined by four ordered pairs that represent points.
        /// </summary>
        public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2,
                               float x3, float y3, float x4, float y4)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawBezier(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x1, y1,
                                                x2, y2, x3, y3, x4, y4);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a cubic bezier curve defined by four points.
        /// </summary>
        public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        /// <summary>
        /// Draws a series of cubic Bezier curves from an array of points.
        /// </summary>
        public void DrawBeziers(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawBeziers(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                 new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a cubic bezier curve defined by four points.
        /// </summary>
        public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        /// <summary>
        /// Draws a series of cubic Bezier curves from an array of points.
        /// </summary>
        public void DrawBeziers(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawBeziersI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                  new HandleRef(this, buf), points.Length);
            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of a rectangle specified by <paramref name="rect"/>.
        /// </summary>
        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws the outline of the specified rectangle.
        /// </summary>
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }

            int status = SafeNativeMethods.Gdip.GdipDrawRectangle(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y,
                                                   width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of the specified rectangle.
        /// </summary>
        public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }

            int status = SafeNativeMethods.Gdip.GdipDrawRectangleI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y, width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outlines of a series of rectangles.
        /// </summary>
        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (rects == null)
            {
                throw new ArgumentNullException("rects");
            }

            IntPtr buf = SafeNativeMethods.Gdip.ConvertRectangleToMemory(rects);
            int status = SafeNativeMethods.Gdip.GdipDrawRectangles(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                    new HandleRef(this, buf), rects.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outlines of a series of rectangles.
        /// </summary>
        public void DrawRectangles(Pen pen, Rectangle[] rects)
        {
            if (pen == null)
            {
                throw new ArgumentNullException("pen");
            }
            if (rects == null)
            {
                throw new ArgumentNullException("rects");
            }

            IntPtr buf = SafeNativeMethods.Gdip.ConvertRectangleToMemory(rects);
            int status = SafeNativeMethods.Gdip.GdipDrawRectanglesI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                     new HandleRef(this, buf), rects.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws the outline of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawEllipse(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y,
                                                 width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of an ellipse specified by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws the outline of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawEllipseI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y,
                                                  width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle,
                    sweepAngle);
        }

        /// <summary>
        /// Draws the outline of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void DrawPie(Pen pen, float x, float y, float width,
                            float height, float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawPie(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y, width,
                                             height, startAngle, sweepAngle);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle,
                    sweepAngle);
        }

        /// <summary>
        /// Draws the outline of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void DrawPie(Pen pen, int x, int y, int width, int height,
                            int startAngle, int sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int status = SafeNativeMethods.Gdip.GdipDrawPieI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), x, y, width,
                                              height, startAngle, sweepAngle);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawPolygon(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                 new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the outline of a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawPolygonI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                                  new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws the lines and curves defined by a <see cref='GraphicsPath'/>.
        /// </summary>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (path == null)
                throw new ArgumentNullException("path");

            int status = SafeNativeMethods.Gdip.GdipDrawPath(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen),
                                              new HandleRef(path, path.nativePath));

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a curve defined by an array of points.
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawCurve(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                               points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a curve defined by an array of points.
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points, float tension)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawCurve2(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                points.Length, tension);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments)
        {
            DrawCurve(pen, points, offset, numberOfSegments, 0.5f);
        }

        /// <summary>
        /// Draws a curve defined by an array of points.
        /// </summary>
        public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments,
                              float tension)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawCurve3(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                points.Length, offset, numberOfSegments,
                                                tension);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a curve defined by an array of points.
        /// </summary>
        public void DrawCurve(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawCurveI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a curve defined by an array of points.
        /// </summary>
        public void DrawCurve(Pen pen, Point[] points, float tension)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawCurve2I(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                 points.Length, tension);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a curve defined by an array of points.
        /// </summary>
        public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments,
                              float tension)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawCurve3I(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                 points.Length, offset, numberOfSegments,
                                                 tension);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a closed curve defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawClosedCurve(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                     points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a closed curve defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawClosedCurve2(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                      points.Length, tension);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a closed curve defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(Pen pen, Point[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawClosedCurveI(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                      points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a closed curve defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipDrawClosedCurve2I(new HandleRef(this, NativeGraphics), new HandleRef(pen, pen.NativePen), new HandleRef(this, buf),
                                                       points.Length, tension);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the entire drawing surface with the specified color.
        /// </summary>
        public void Clear(Color color)
        {
            int status = SafeNativeMethods.Gdip.GdipGraphicsClear(new HandleRef(this, NativeGraphics), color.ToArgb());

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Fills the interior of a rectangle with a <see cref='Brush'/>.
        /// </summary>
        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Fills the interior of a rectangle with a <see cref='Brush'/>.
        /// </summary>
        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            int status = SafeNativeMethods.Gdip.GdipFillRectangle(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush), x, y,
                                                   width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a rectangle with a <see cref='Brush'/>.
        /// </summary>
        public void FillRectangle(Brush brush, Rectangle rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Fills the interior of a rectangle with a <see cref='Brush'/>.
        /// </summary>
        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            int status = SafeNativeMethods.Gdip.GdipFillRectangleI(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush), x, y, width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interiors of a series of rectangles with a <see cref='Brush'/>.
        /// </summary>
        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (rects == null)
            {
                throw new ArgumentNullException("rects");
            }

            IntPtr buf = SafeNativeMethods.Gdip.ConvertRectangleToMemory(rects);
            int status = SafeNativeMethods.Gdip.GdipFillRectangles(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                    new HandleRef(this, buf), rects.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interiors of a series of rectangles with a <see cref='Brush'/>.
        /// </summary>
        public void FillRectangles(Brush brush, Rectangle[] rects)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if (rects == null)
            {
                throw new ArgumentNullException("rects");
            }

            IntPtr buf = SafeNativeMethods.Gdip.ConvertRectangleToMemory(rects);
            int status = SafeNativeMethods.Gdip.GdipFillRectanglesI(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                     new HandleRef(this, buf), rects.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points.
        /// </summary>
        public void FillPolygon(Brush brush, PointF[] points)
        {
            FillPolygon(brush, points, FillMode.Alternate);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points.
        /// </summary>
        public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipFillPolygon(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                 new HandleRef(this, buf), points.Length, unchecked((int)fillMode));

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points.
        /// </summary>
        public void FillPolygon(Brush brush, Point[] points)
        {
            FillPolygon(brush, points, FillMode.Alternate);
        }

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points.
        /// </summary>
        public void FillPolygon(Brush brush, Point[] points, FillMode fillMode)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipFillPolygonI(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                  new HandleRef(this, buf), points.Length, unchecked((int)fillMode));

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void FillEllipse(Brush brush, RectangleF rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void FillEllipse(Brush brush, float x, float y, float width,
                                float height)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            int status = SafeNativeMethods.Gdip.GdipFillEllipse(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush), x, y,
                                                 width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void FillEllipse(Brush brush, Rectangle rect)
        {
            FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Fills the interior of an ellipse defined by a bounding rectangle.
        /// </summary>
        public void FillEllipse(Brush brush, int x, int y, int width, int height)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            int status = SafeNativeMethods.Gdip.GdipFillEllipseI(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush), x, y,
                                                  width, height);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void FillPie(Brush brush, Rectangle rect, float startAngle,
                            float sweepAngle)
        {
            FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle,
                    sweepAngle);
        }

        /// <summary>
        /// Fills the interior of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void FillPie(Brush brush, float x, float y, float width,
                            float height, float startAngle, float sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            int status = SafeNativeMethods.Gdip.GdipFillPie(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush), x, y,
                                             width, height, startAngle, sweepAngle);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a pie section defined by an ellipse and two radial lines.
        /// </summary>
        public void FillPie(Brush brush, int x, int y, int width,
                            int height, int startAngle, int sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            int status = SafeNativeMethods.Gdip.GdipFillPieI(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush), x, y,
                                              width, height, startAngle, sweepAngle);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a path.
        /// </summary>
        public void FillPath(Brush brush, GraphicsPath path)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (path == null)
                throw new ArgumentNullException("path");

            int status = SafeNativeMethods.Gdip.GdipFillPath(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                              new HandleRef(path, path.nativePath));

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior a closed curve defined by an array of points.
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurve(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                               new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a closed curve defined by an array of points.
        /// </summary>
        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode)
        {
            FillClosedCurve(brush, points, fillmode, 0.5f);
        }

        public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurve2(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                      new HandleRef(this, buf), points.Length,
                                                      tension, unchecked((int)fillmode));

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior a closed curve defined by an array of points.
        /// </summary>
        public void FillClosedCurve(Brush brush, Point[] points)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurveI(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                     new HandleRef(this, buf), points.Length);

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode)
        {
            FillClosedCurve(brush, points, fillmode, 0.5f);
        }

        public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(points);
            int status = SafeNativeMethods.Gdip.GdipFillClosedCurve2I(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                      new HandleRef(this, buf), points.Length,
                                                      tension, unchecked((int)fillmode));

            Marshal.FreeHGlobal(buf);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Fills the interior of a <see cref='Region'/>.
        /// </summary>
        public void FillRegion(Brush brush, Region region)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (region == null)
                throw new ArgumentNullException("region");

            int status = SafeNativeMethods.Gdip.GdipFillRegion(new HandleRef(this, NativeGraphics), new HandleRef(brush, brush.NativeBrush),
                                                new HandleRef(region, region._nativeRegion));

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        /// <summary>
        /// Draws a string with the specified font.
        /// </summary>
        public void DrawString(String s, Font font, Brush brush, float x, float y)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 0, 0), null);
        }

        public void DrawString(String s, Font font, Brush brush, PointF point)
        {
            DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), null);
        }

        public void DrawString(String s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 0, 0), format);
        }

        public void DrawString(String s, Font font, Brush brush, PointF point, StringFormat format)
        {
            DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), format);
        }

        public void DrawString(String s, Font font, Brush brush, RectangleF layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle, null);
        }

        public void DrawString(String s, Font font, Brush brush,
                               RectangleF layoutRectangle, StringFormat format)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (s == null || s.Length == 0)
                return;
            if (font == null)
                throw new ArgumentNullException("font");

            GPRECTF grf = new GPRECTF(layoutRectangle);
            IntPtr nativeStringFormat = (format == null) ? IntPtr.Zero : format.nativeFormat;
            int status = SafeNativeMethods.Gdip.GdipDrawString(new HandleRef(this, NativeGraphics), s, s.Length, new HandleRef(font, font.NativeFont), ref grf, new HandleRef(format, nativeStringFormat), new HandleRef(brush, brush.NativeBrush));

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        public SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat,
                                   out int charactersFitted, out int linesFilled)
        {
            if (text == null || text.Length == 0)
            {
                charactersFitted = 0;
                linesFilled = 0;
                return new SizeF(0, 0);
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            GPRECTF grfLayout = new GPRECTF(0, 0, layoutArea.Width, layoutArea.Height);
            GPRECTF grfboundingBox = new GPRECTF();
            int status = SafeNativeMethods.Gdip.GdipMeasureString(new HandleRef(this, NativeGraphics), text, text.Length, new HandleRef(font, font.NativeFont), ref grfLayout,
                                                   new HandleRef(stringFormat, (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat),
                                                   ref grfboundingBox,
                                                   out charactersFitted, out linesFilled);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return grfboundingBox.SizeF;
        }

        public SizeF MeasureString(String text, Font font, PointF origin, StringFormat stringFormat)
        {
            if (text == null || text.Length == 0)
                return new SizeF(0, 0);
            if (font == null)
                throw new ArgumentNullException("font");

            GPRECTF grf = new GPRECTF();
            GPRECTF grfboundingBox = new GPRECTF();

            grf.X = origin.X;
            grf.Y = origin.Y;
            grf.Width = 0;
            grf.Height = 0;

            int a, b;
            int status = SafeNativeMethods.Gdip.GdipMeasureString(new HandleRef(this, NativeGraphics), text, text.Length, new HandleRef(font, font.NativeFont),
                ref grf,
                new HandleRef(stringFormat, (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat),
                ref grfboundingBox, out a, out b);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return grfboundingBox.SizeF;
        }

        public SizeF MeasureString(String text, Font font, SizeF layoutArea)
        {
            return MeasureString(text, font, layoutArea, null);
        }

        public SizeF MeasureString(String text, Font font, SizeF layoutArea, StringFormat stringFormat)
        {
            if (text == null || text.Length == 0)
            {
                return new SizeF(0, 0);
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            GPRECTF grfLayout = new GPRECTF(0, 0, layoutArea.Width, layoutArea.Height);
            GPRECTF grfboundingBox = new GPRECTF();

            int a, b;
            int status = SafeNativeMethods.Gdip.GdipMeasureString(new HandleRef(this, NativeGraphics), text, text.Length, new HandleRef(font, font.NativeFont),
                ref grfLayout,
                new HandleRef(stringFormat, (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat),
                ref grfboundingBox, out a, out b);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return grfboundingBox.SizeF;
        }

        public SizeF MeasureString(String text, Font font)
        {
            return MeasureString(text, font, new SizeF(0, 0));
        }

        public SizeF MeasureString(String text, Font font, int width)
        {
            return MeasureString(text, font, new SizeF(width, 999999));
        }

        public SizeF MeasureString(String text, Font font, int width, StringFormat format)
        {
            return MeasureString(text, font, new SizeF(width, 999999), format);
        }

        public Region[] MeasureCharacterRanges(String text, Font font, RectangleF layoutRect,
                                          StringFormat stringFormat)
        {
            if (text == null || text.Length == 0)
            {
                return new Region[] { };
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            int count;
            int status = SafeNativeMethods.Gdip.GdipGetStringFormatMeasurableCharacterRangeCount(new HandleRef(stringFormat, (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat)
                                                                                    , out count);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            IntPtr[] gpRegions = new IntPtr[count];

            GPRECTF grf = new GPRECTF(layoutRect);

            Region[] regions = new Region[count];

            for (int f = 0; f < count; f++)
            {
                regions[f] = new Region();
                gpRegions[f] = (IntPtr)regions[f]._nativeRegion;
            }

            status = SafeNativeMethods.Gdip.GdipMeasureCharacterRanges(new HandleRef(this, NativeGraphics), text, text.Length, new HandleRef(font, font.NativeFont), ref grf,
                                                         new HandleRef(stringFormat, (stringFormat == null) ? IntPtr.Zero : stringFormat.nativeFormat),
                                                         count, gpRegions);


            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return regions;
        }

        public void DrawIcon(Icon icon, int x, int y)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            if (_backingImage != null)
            {
                // we don't call the icon directly because we want to stay in GDI+ all the time
                // to avoid alpha channel interop issues between gdi and gdi+
                // so we do icon.ToBitmap() and then we call DrawImage. this is probably slower...
                DrawImage(icon.ToBitmap(), x, y);
            }
            else
            {
                icon.Draw(this, x, y);
            }
        }

        /// <summary>
        /// Draws this image to a graphics object.  The drawing command originates on the graphics
        /// object, but a graphics object generally has no idea how to render a given image.  So,
        /// it passes the call to the actual image.  This version crops the image to the given
        /// dimensions and allows the user to specify a rectangle within the image to draw.
        /// </summary>
        public void DrawIcon(Icon icon, Rectangle targetRect)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            if (_backingImage != null)
            {
                // we don't call the icon directly because we want to stay in GDI+ all the time
                // to avoid alpha channel interop issues between gdi and gdi+
                // so we do icon.ToBitmap() and then we call DrawImage. this is probably slower...
                DrawImage(icon.ToBitmap(), targetRect);
            }
            else
            {
                icon.Draw(this, targetRect);
            }
        }

        /// <summary>
        /// Draws this image to a graphics object.  The drawing command originates on the graphics
        /// object, but a graphics object generally has no idea how to render a given image.  So,
        /// it passes the call to the actual image.  This version stretches the image to the given
        /// dimensions and allows the user to specify a rectangle within the image to draw.
        /// </summary>
        public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            if (_backingImage != null)
            {
                DrawImageUnscaled(icon.ToBitmap(), targetRect);
            }
            else
            {
                icon.DrawUnstretched(this, targetRect);
            }
        }

        /// <summary>
        /// Draws the specified image at the specified location.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, PointF point)
        {
            DrawImage(image, point.X, point.Y);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, float x, float y)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImage(new HandleRef(this, NativeGraphics), new HandleRef(image, image.nativeImage),
                                               x, y);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, RectangleF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, float x, float y, float width,
                              float height)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImageRect(new HandleRef(this, NativeGraphics),
                                                   new HandleRef(image, image.nativeImage),
                                                   x, y,
                                                   width, height);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Point point)
        {
            DrawImage(image, point.X, point.Y);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, int x, int y)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImageI(new HandleRef(this, NativeGraphics), new HandleRef(image, image.nativeImage),
                                                x, y);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            int status = SafeNativeMethods.Gdip.GdipDrawImageRectI(new HandleRef(this, NativeGraphics),
                                                    new HandleRef(image, image.nativeImage),
                                                    x, y,
                                                    width, height);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }



        public void DrawImageUnscaled(Image image, Point point)
        {
            DrawImage(image, point.X, point.Y);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImage(image, x, y);
        }

        public void DrawImageUnscaled(Image image, Rectangle rect)
        {
            DrawImage(image, rect.X, rect.Y);
        }

        public void DrawImageUnscaled(Image image, int x, int y, int width, int height)
        {
            DrawImage(image, x, y);
        }

        public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            int width = Math.Min(rect.Width, image.Width);
            int height = Math.Min(rect.Height, image.Height);

            //We could put centering logic here too for the case when the image is smaller than the rect
            DrawImage(image, rect, 0, 0, width, height, GraphicsUnit.Pixel);
        }

        /*
         * Affine or perspective blt
         *  destPoints.Length = 3: rect => parallelogram
         *      destPoints[0] <=> top-left corner of the source rectangle
         *      destPoints[1] <=> top-right corner
         *       destPoints[2] <=> bottom-left corner
         *  destPoints.Length = 4: rect => quad
         *      destPoints[3] <=> bottom-right corner
         *
         *  @notes Perspective blt only works for bitmap images.
         */
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, PointF[] destPoints)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (image == null)
                throw new ArgumentNullException("image");

            int count = destPoints.Length;

            if (count != 3 && count != 4)
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidLength));

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);
            int status = SafeNativeMethods.Gdip.GdipDrawImagePoints(new HandleRef(this, NativeGraphics),
                                                     new HandleRef(image, image.nativeImage),
                                                     new HandleRef(this, buf), count);

            Marshal.FreeHGlobal(buf);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Point[] destPoints)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (image == null)
                throw new ArgumentNullException("image");

            int count = destPoints.Length;

            if (count != 3 && count != 4)
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidLength));

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);
            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsI(new HandleRef(this, NativeGraphics),
                                                      new HandleRef(image, image.nativeImage),
                                                      new HandleRef(this, buf), count);

            Marshal.FreeHGlobal(buf);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, float x, float y, RectangleF srcRect,
                              GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointRect(
                                                       new HandleRef(this, NativeGraphics),
                                                       new HandleRef(image, image.nativeImage),
                                                       x,
                                                       y,
                                                       srcRect.X,
                                                       srcRect.Y,
                                                       srcRect.Width,
                                                       srcRect.Height,
                                                       unchecked((int)srcUnit));

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, int x, int y, Rectangle srcRect,
                              GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointRectI(
                                                        new HandleRef(this, NativeGraphics),
                                                        new HandleRef(image, image.nativeImage),
                                                        x,
                                                        y,
                                                        srcRect.X,
                                                        srcRect.Y,
                                                        srcRect.Width,
                                                        srcRect.Height,
                                                        unchecked((int)srcUnit));

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect,
                              GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(
                                                      new HandleRef(this, NativeGraphics),
                                                      new HandleRef(image, image.nativeImage),
                                                      destRect.X,
                                                      destRect.Y,
                                                      destRect.Width,
                                                      destRect.Height,
                                                      srcRect.X,
                                                      srcRect.Y,
                                                      srcRect.Width,
                                                      srcRect.Height,
                                                      unchecked((int)srcUnit),
                                                      NativeMethods.NullHandleRef,
                                                      null,
                                                      NativeMethods.NullHandleRef
                                                      );

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect,
                              GraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRectI(
                                                       new HandleRef(this, NativeGraphics),
                                                       new HandleRef(image, image.nativeImage),
                                                       destRect.X,
                                                       destRect.Y,
                                                       destRect.Width,
                                                       destRect.Height,
                                                       srcRect.X,
                                                       srcRect.Y,
                                                       srcRect.Width,
                                                       srcRect.Height,
                                                       unchecked((int)srcUnit),
                                                       NativeMethods.NullHandleRef,
                                                       null,
                                                       NativeMethods.NullHandleRef);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect,
                              GraphicsUnit srcUnit)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (image == null)
                throw new ArgumentNullException("image");

            int count = destPoints.Length;

            if (count != 3 && count != 4)
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidLength));

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRect(
                                                        new HandleRef(this, NativeGraphics),
                                                        new HandleRef(image, image.nativeImage),
                                                        new HandleRef(this, buf),
                                                        destPoints.Length,
                                                        srcRect.X,
                                                        srcRect.Y,
                                                        srcRect.Width,
                                                        srcRect.Height,
                                                        unchecked((int)srcUnit),
                                                        NativeMethods.NullHandleRef,
                                                        null,
                                                        NativeMethods.NullHandleRef);

            Marshal.FreeHGlobal(buf);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect,
                              GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, null, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect,
                              GraphicsUnit srcUnit, ImageAttributes imageAttr,
                              DrawImageAbort callback)
        {
            DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect,
                              GraphicsUnit srcUnit, ImageAttributes imageAttr,
                              DrawImageAbort callback, int callbackData)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (image == null)
                throw new ArgumentNullException("image");

            int count = destPoints.Length;

            if (count != 3 && count != 4)
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidLength));

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRect(
                                                        new HandleRef(this, NativeGraphics),
                                                        new HandleRef(image, image.nativeImage),
                                                        new HandleRef(this, buf),
                                                        destPoints.Length,
                                                        srcRect.X,
                                                        srcRect.Y,
                                                        srcRect.Width,
                                                        srcRect.Height,
                                                        unchecked((int)srcUnit),
                                                        new HandleRef(imageAttr, (imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero)),
                                                        callback,
                                                        new HandleRef(null, (IntPtr)callbackData));

            Marshal.FreeHGlobal(buf);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            DrawImage(image, destPoints, srcRect, srcUnit, null, null, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect,
                              GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, null, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect,
                              GraphicsUnit srcUnit, ImageAttributes imageAttr,
                              DrawImageAbort callback)
        {
            DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback, 0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect,
                              GraphicsUnit srcUnit, ImageAttributes imageAttr,
                              DrawImageAbort callback, int callbackData)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (image == null)
                throw new ArgumentNullException("image");

            int count = destPoints.Length;

            if (count != 3 && count != 4)
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidLength));

            IntPtr buf = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            int status = SafeNativeMethods.Gdip.GdipDrawImagePointsRectI(
                                                        new HandleRef(this, NativeGraphics),
                                                        new HandleRef(image, image.nativeImage),
                                                        new HandleRef(this, buf),
                                                        destPoints.Length,
                                                        srcRect.X,
                                                        srcRect.Y,
                                                        srcRect.Width,
                                                        srcRect.Height,
                                                        unchecked((int)srcUnit),
                                                        new HandleRef(imageAttr, (imageAttr != null ? imageAttr.nativeImageAttributes : IntPtr.Zero)),
                                                        callback,
                                                        new HandleRef(null, (IntPtr)callbackData));

            Marshal.FreeHGlobal(buf);

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY,
                              float srcWidth, float srcHeight, GraphicsUnit srcUnit)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY,
                              float srcWidth, float srcHeight, GraphicsUnit srcUnit,
                              ImageAttributes imageAttrs)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY,
                              float srcWidth, float srcHeight, GraphicsUnit srcUnit,
                              ImageAttributes imageAttrs, DrawImageAbort callback)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY,
                              float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs,
                              DrawImageAbort callback, IntPtr callbackData)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRect(
                                                       new HandleRef(this, NativeGraphics),
                                                       new HandleRef(image, image.nativeImage),
                                                       destRect.X,
                                                       destRect.Y,
                                                       destRect.Width,
                                                       destRect.Height,
                                                       srcX,
                                                       srcY,
                                                       srcWidth,
                                                       srcHeight,
                                                       unchecked((int)srcUnit),
                                                       new HandleRef(imageAttrs, (imageAttrs != null ? imageAttrs.nativeImageAttributes : IntPtr.Zero)),
                                                       callback,
                                                       new HandleRef(null, callbackData));

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY,
                              int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY,
                              int srcWidth, int srcHeight, GraphicsUnit srcUnit,
                              ImageAttributes imageAttr)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY,
                              int srcWidth, int srcHeight, GraphicsUnit srcUnit,
                              ImageAttributes imageAttr, DrawImageAbort callback)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY,
                              int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs,
                              DrawImageAbort callback, IntPtr callbackData)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int status = SafeNativeMethods.Gdip.GdipDrawImageRectRectI(
                                                       new HandleRef(this, NativeGraphics),
                                                       new HandleRef(image, image.nativeImage),
                                                       destRect.X,
                                                       destRect.Y,
                                                       destRect.Width,
                                                       destRect.Height,
                                                       srcX,
                                                       srcY,
                                                       srcWidth,
                                                       srcHeight,
                                                       unchecked((int)srcUnit),
                                                       new HandleRef(imageAttrs, (imageAttrs != null ? imageAttrs.nativeImageAttributes : IntPtr.Zero)),
                                                       callback,
                                                       new HandleRef(null, callbackData));

            //ignore emf metafile error
            IgnoreMetafileErrors(image, ref status);

            //check error status sensitive to TS problems
            CheckErrorStatus(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF destPoint,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoint, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF destPoint,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoint, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, PointF destPoint,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileDestPoint(new HandleRef(this, NativeGraphics),
                                                                new HandleRef(metafile, mf),
                                                                new GPPOINTF(destPoint),
                                                                callback,
                                                                new HandleRef(null, callbackData),
                                                                new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point destPoint,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoint, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point destPoint,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoint, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, Point destPoint,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileDestPointI(new HandleRef(this, NativeGraphics),
                                                                 new HandleRef(metafile, mf),
                                                                 new GPPOINT(destPoint),
                                                                 callback,
                                                                 new HandleRef(null, callbackData),
                                                                 new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destRect, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destRect, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            GPRECTF grf = new GPRECTF(destRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileDestRect(
                                                                new HandleRef(this, NativeGraphics),
                                                                new HandleRef(metafile, mf),
                                                                ref grf,
                                                                callback,
                                                                new HandleRef(null, callbackData),
                                                                new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destRect, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destRect, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            GPRECT gprect = new GPRECT(destRect);
            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileDestRectI(new HandleRef(this, NativeGraphics),
                                                                new HandleRef(metafile, mf),
                                                                ref gprect,
                                                                callback,
                                                                new HandleRef(null, callbackData),
                                                                new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (destPoints.Length != 3)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidParallelogram));
            }

            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            IntPtr points = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileDestPoints(new HandleRef(this, NativeGraphics),
                                                                 new HandleRef(metafile, mf),
                                                                 points,
                                                                 destPoints.Length,
                                                                 callback,
                                                                 new HandleRef(null, callbackData),
                                                                 new HandleRef(imageAttr, ia));
            Marshal.FreeHGlobal(points);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoints, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (destPoints.Length != 3)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidParallelogram));
            }

            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            IntPtr points = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileDestPointsI(new HandleRef(this, NativeGraphics),
                                                                  new HandleRef(metafile, mf),
                                                                  points,
                                                                  destPoints.Length,
                                                                  callback,
                                                                  new HandleRef(null, callbackData),
                                                                  new HandleRef(imageAttr, ia));
            Marshal.FreeHGlobal(points);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF destPoint,
                                      RectangleF srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF destPoint,
                                      RectangleF srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, PointF destPoint,
                                      RectangleF srcRect, GraphicsUnit unit,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            GPRECTF grf = new GPRECTF(srcRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileSrcRectDestPoint(new HandleRef(this, NativeGraphics),
                                                                       new HandleRef(metafile, mf),
                                                                       new GPPOINTF(destPoint),
                                                                       ref grf,
                                                                       unchecked((int)unit),
                                                                       callback,
                                                                       new HandleRef(null, callbackData),
                                                                       new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point destPoint,
                                      Rectangle srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point destPoint,
                                      Rectangle srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, Point destPoint,
                                      Rectangle srcRect, GraphicsUnit unit,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            GPPOINT gppoint = new GPPOINT(destPoint);
            GPRECT gprect = new GPRECT(srcRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileSrcRectDestPointI(new HandleRef(this, NativeGraphics),
                                                                        new HandleRef(metafile, mf),
                                                                        gppoint,
                                                                        ref gprect,
                                                                        unchecked((int)unit),
                                                                        callback,
                                                                        new HandleRef(null, callbackData),
                                                                        new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
                                      RectangleF srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
                                      RectangleF srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public void EnumerateMetafile(Metafile metafile, RectangleF destRect,
                                      RectangleF srcRect, GraphicsUnit unit,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            GPRECTF grfdest = new GPRECTF(destRect);
            GPRECTF grfsrc = new GPRECTF(srcRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileSrcRectDestRect(
                                                                         new HandleRef(this, NativeGraphics),
                                                                         new HandleRef(metafile, mf),
                                                                         ref grfdest,
                                                                         ref grfsrc,
                                                                         unchecked((int)unit),
                                                                         callback,
                                                                         new HandleRef(null, callbackData),
                                                                         new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
                                      Rectangle srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
                                      Rectangle srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Rectangle destRect,
                                      Rectangle srcRect, GraphicsUnit unit,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            GPRECT gpDest = new GPRECT(destRect);
            GPRECT gpSrc = new GPRECT(srcRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileSrcRectDestRectI(new HandleRef(this, NativeGraphics),
                                                                       new HandleRef(metafile, mf),
                                                                       ref gpDest,
                                                                       ref gpSrc,
                                                                       unchecked((int)unit),
                                                                       callback,
                                                                       new HandleRef(null, callbackData),
                                                                       new HandleRef(imageAttr, ia));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
                                      RectangleF srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
                                      RectangleF srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, PointF[] destPoints,
                                      RectangleF srcRect, GraphicsUnit unit,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (destPoints.Length != 3)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidParallelogram));
            }

            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            IntPtr buffer = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            GPRECTF grf = new GPRECTF(srcRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileSrcRectDestPoints(new HandleRef(this, NativeGraphics),
                                                                        new HandleRef(metafile, mf),
                                                                        buffer,
                                                                        destPoints.Length,
                                                                        ref grf,
                                                                        unchecked((int)unit),
                                                                        callback,
                                                                        new HandleRef(null, callbackData),
                                                                        new HandleRef(imageAttr, ia));
            Marshal.FreeHGlobal(buffer);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
                                      Rectangle srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback)
        {
            EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, IntPtr.Zero);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
                                      Rectangle srcRect, GraphicsUnit srcUnit,
                                      EnumerateMetafileProc callback, IntPtr callbackData)
        {
            EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData, null);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public void EnumerateMetafile(Metafile metafile, Point[] destPoints,
                                      Rectangle srcRect, GraphicsUnit unit,
                                      EnumerateMetafileProc callback, IntPtr callbackData,
                                      ImageAttributes imageAttr)
        {
            if (destPoints == null)
                throw new ArgumentNullException("destPoints");
            if (destPoints.Length != 3)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusDestPointsInvalidParallelogram));
            }

            IntPtr mf = (metafile == null ? IntPtr.Zero : metafile.nativeImage);
            IntPtr ia = (imageAttr == null ? IntPtr.Zero : imageAttr.nativeImageAttributes);

            IntPtr buffer = SafeNativeMethods.Gdip.ConvertPointToMemory(destPoints);

            GPRECT gpSrc = new GPRECT(srcRect);

            int status = SafeNativeMethods.Gdip.GdipEnumerateMetafileSrcRectDestPointsI(new HandleRef(this, NativeGraphics),
                                                                         new HandleRef(metafile, mf),
                                                                         buffer,
                                                                         destPoints.Length,
                                                                         ref gpSrc,
                                                                         unchecked((int)unit),
                                                                         callback,
                                                                         new HandleRef(null, callbackData),
                                                                         new HandleRef(imageAttr, ia));
            Marshal.FreeHGlobal(buffer);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }


        public void SetClip(Graphics g)
        {
            SetClip(g, CombineMode.Replace);
        }

        public void SetClip(Graphics g, CombineMode combineMode)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            int status = SafeNativeMethods.Gdip.GdipSetClipGraphics(new HandleRef(this, NativeGraphics), new HandleRef(g, g.NativeGraphics), combineMode);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void SetClip(Rectangle rect)
        {
            SetClip(rect, CombineMode.Replace);
        }

        public void SetClip(Rectangle rect, CombineMode combineMode)
        {
            int status = SafeNativeMethods.Gdip.GdipSetClipRectI(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                  rect.Width, rect.Height, combineMode);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void SetClip(RectangleF rect)
        {
            SetClip(rect, CombineMode.Replace);
        }

        public void SetClip(RectangleF rect, CombineMode combineMode)
        {
            int status = SafeNativeMethods.Gdip.GdipSetClipRect(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                 rect.Width, rect.Height, combineMode);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void SetClip(GraphicsPath path)
        {
            SetClip(path, CombineMode.Replace);
        }

        public void SetClip(GraphicsPath path, CombineMode combineMode)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            int status = SafeNativeMethods.Gdip.GdipSetClipPath(new HandleRef(this, NativeGraphics), new HandleRef(path, path.nativePath), combineMode);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void SetClip(Region region, CombineMode combineMode)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            int status = SafeNativeMethods.Gdip.GdipSetClipRegion(new HandleRef(this, NativeGraphics), new HandleRef(region, region._nativeRegion), combineMode);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void IntersectClip(Rectangle rect)
        {
            int status = SafeNativeMethods.Gdip.GdipSetClipRectI(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                  rect.Width, rect.Height, CombineMode.Intersect);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void IntersectClip(RectangleF rect)
        {
            int status = SafeNativeMethods.Gdip.GdipSetClipRect(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                 rect.Width, rect.Height, CombineMode.Intersect);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void IntersectClip(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            int status = SafeNativeMethods.Gdip.GdipSetClipRegion(new HandleRef(this, NativeGraphics), new HandleRef(region, region._nativeRegion),
                                                   CombineMode.Intersect);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void ExcludeClip(Rectangle rect)
        {
            int status = SafeNativeMethods.Gdip.GdipSetClipRectI(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                  rect.Width, rect.Height, CombineMode.Exclude);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void ExcludeClip(Region region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            int status = SafeNativeMethods.Gdip.GdipSetClipRegion(new HandleRef(this, NativeGraphics),
                                                   new HandleRef(region, region._nativeRegion),
                                                   CombineMode.Exclude);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void ResetClip()
        {
            int status = SafeNativeMethods.Gdip.GdipResetClip(new HandleRef(this, NativeGraphics));

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void TranslateClip(float dx, float dy)
        {
            int status = SafeNativeMethods.Gdip.GdipTranslateClip(new HandleRef(this, NativeGraphics), dx, dy);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public void TranslateClip(int dx, int dy)
        {
            int status = SafeNativeMethods.Gdip.GdipTranslateClip(new HandleRef(this, NativeGraphics), dx, dy);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Combines current Graphics context with all previous contexts.
        /// When BeginContainer() is called, a copy of the current context is pushed into the GDI+ context stack, it keeps track of the
        /// absolute clipping and transform but reset the public properties so it looks like a brand new context.
        /// When Save() is called, a copy of the current context is also pushed in the GDI+ stack but the public clipping and transform
        /// properties are not reset (cumulative).  Consecutive Save context are ignored with the exception of the top one which contains 
        /// all previous information.
        /// The return value is an object array where the first element contains the cumulative clip region and the second the cumulative
        /// translate transform matrix.
        /// WARNING: This method is for internal FX support only.
        /// </summary>
        [StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, Name = "System.Windows.Forms", PublicKey = "0x00000000000000000400000000000000")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetContextInfo()
        {
            Region cumulClip = Clip;           // current context clip.
            Matrix cumulTransform = Transform; // current context transform.
            PointF currentOffset = PointF.Empty;    // offset of current context.
            PointF totalOffset = PointF.Empty;      // absolute coord offset of top context.

            if (!cumulTransform.IsIdentity)
            {
                float[] elements = cumulTransform.Elements;
                currentOffset.X = elements[4];
                currentOffset.Y = elements[5];
            }

            GraphicsContext context = _previousContext;

            while (context != null)
            {
                if (!context.TransformOffset.IsEmpty)
                {
                    cumulTransform.Translate(context.TransformOffset.X, context.TransformOffset.Y);
                }

                if (!currentOffset.IsEmpty)
                {
                    // The location of the GDI+ clip region is relative to the coordinate origin after any translate transform
                    // has been applied.  We need to intersect regions using the same coordinate origin relative to the previous
                    // context.
                    cumulClip.Translate(currentOffset.X, currentOffset.Y);
                    totalOffset.X += currentOffset.X;
                    totalOffset.Y += currentOffset.Y;
                }

                if (context.Clip != null)
                {
                    cumulClip.Intersect(context.Clip);
                }

                currentOffset = context.TransformOffset;

                // Ignore subsequent cumulative contexts.
                do
                {
                    context = context.Previous;

                    if (context == null || !context.Next.IsCumulative)
                    {
                        break;
                    }
                } while (context.IsCumulative);
            }

            if (!totalOffset.IsEmpty)
            {
                // We need now to reset the total transform in the region so when calling Region.GetHRgn(Graphics)
                // the HRegion is properly offset by GDI+ based on the total offset of the graphics object.
                cumulClip.Translate(-totalOffset.X, -totalOffset.Y);
            }

            return new object[] { cumulClip, cumulTransform };
        }


        public Region Clip
        {
            get
            {
                Region region = new Region();

                int status = SafeNativeMethods.Gdip.GdipGetClip(new HandleRef(this, NativeGraphics), new HandleRef(region, region._nativeRegion));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return region;
            }
            set
            {
                SetClip(value, CombineMode.Replace);
            }
        }

        public RectangleF ClipBounds
        {
            get
            {
                GPRECTF rect = new GPRECTF();

                int status = SafeNativeMethods.Gdip.GdipGetClipBounds(new HandleRef(this, NativeGraphics), ref rect);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return rect.ToRectangleF();
            }
        }

        public bool IsClipEmpty
        {
            get
            {
                int isEmpty;

                int status = SafeNativeMethods.Gdip.GdipIsClipEmpty(new HandleRef(this, NativeGraphics), out isEmpty);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return isEmpty != 0;
            }
        }

        public RectangleF VisibleClipBounds
        {
            get
            {
                if (PrintingHelper != null)
                {
                    PrintPreviewGraphics ppGraphics = PrintingHelper as PrintPreviewGraphics;
                    if (ppGraphics != null)
                    {
                        return ppGraphics.VisibleClipBounds;
                    }
                }

                GPRECTF rect = new GPRECTF();

                int status = SafeNativeMethods.Gdip.GdipGetVisibleClipBounds(new HandleRef(this, NativeGraphics), ref rect);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return rect.ToRectangleF();
            }
        }

        public bool IsVisibleClipEmpty
        {
            get
            {
                int isEmpty;

                int status = SafeNativeMethods.Gdip.GdipIsVisibleClipEmpty(new HandleRef(this, NativeGraphics), out isEmpty);

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                return isEmpty != 0;
            }
        }


        public bool IsVisible(int x, int y)
        {
            return IsVisible(new Point(x, y));
        }

        public bool IsVisible(Point point)
        {
            int isVisible;

            int status = SafeNativeMethods.Gdip.GdipIsVisiblePointI(new HandleRef(this, NativeGraphics), point.X, point.Y, out isVisible);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return isVisible != 0;
        }

        public bool IsVisible(float x, float y)
        {
            return IsVisible(new PointF(x, y));
        }

        public bool IsVisible(PointF point)
        {
            int isVisible;

            int status = SafeNativeMethods.Gdip.GdipIsVisiblePoint(new HandleRef(this, NativeGraphics), point.X, point.Y, out isVisible);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return isVisible != 0;
        }

        public bool IsVisible(int x, int y, int width, int height)
        {
            return IsVisible(new Rectangle(x, y, width, height));
        }

        public bool IsVisible(Rectangle rect)
        {
            int isVisible;

            int status = SafeNativeMethods.Gdip.GdipIsVisibleRectI(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                    rect.Width, rect.Height, out isVisible);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return isVisible != 0;
        }

        public bool IsVisible(float x, float y, float width, float height)
        {
            return IsVisible(new RectangleF(x, y, width, height));
        }

        public bool IsVisible(RectangleF rect)
        {
            int isVisible;

            int status = SafeNativeMethods.Gdip.GdipIsVisibleRect(new HandleRef(this, NativeGraphics), rect.X, rect.Y,
                                                   rect.Width, rect.Height, out isVisible);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return isVisible != 0;
        }

        /// <summary>
        /// Saves the current context into the context stack.
        /// </summary>
        private void PushContext(GraphicsContext context)
        {
            Debug.Assert(context != null && context.State != 0, "GraphicsContext object is null or not valid.");

            if (_previousContext != null)
            {
                // Push context.
                context.Previous = _previousContext;
                _previousContext.Next = context;
            }
            _previousContext = context;
        }

        /// <summary>
        /// Pops all contexts from the specified one included.  The specified context is becoming the current context.
        /// </summary>
        private void PopContext(int currentContextState)
        {
            Debug.Assert(_previousContext != null, "Trying to restore a context when the stack is empty");
            GraphicsContext context = _previousContext;

            while (context != null)
            {
                if (context.State == currentContextState)
                {
                    _previousContext = context.Previous;
                    // Pop all contexts up the stack.
                    context.Dispose(); // This will dipose all context object up the stack.
                    return;
                }
                context = context.Previous;
            }
            Debug.Fail("Warning: context state not found!");
        }

        public GraphicsState Save()
        {
            GraphicsContext context = new GraphicsContext(this);
            int state = 0;

            int status = SafeNativeMethods.Gdip.GdipSaveGraphics(new HandleRef(this, NativeGraphics), out state);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                context.Dispose();
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            context.State = state;
            context.IsCumulative = true;
            PushContext(context);

            return new GraphicsState(state);
        }

        public void Restore(GraphicsState gstate)
        {
            int status = SafeNativeMethods.Gdip.GdipRestoreGraphics(new HandleRef(this, NativeGraphics), gstate.nativeState);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            PopContext(gstate.nativeState);
        }

        public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
        {
            GraphicsContext context = new GraphicsContext(this);
            int state = 0;

            GPRECTF dstf = dstrect.ToGPRECTF();
            GPRECTF srcf = srcrect.ToGPRECTF();

            int status = SafeNativeMethods.Gdip.GdipBeginContainer(new HandleRef(this, NativeGraphics), ref dstf,
                                                    ref srcf, unchecked((int)unit), out state);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                context.Dispose();
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            context.State = state;
            PushContext(context);

            return new GraphicsContainer(state);
        }

        public GraphicsContainer BeginContainer()
        {
            GraphicsContext context = new GraphicsContext(this);
            int state = 0;

            int status = SafeNativeMethods.Gdip.GdipBeginContainer2(new HandleRef(this, NativeGraphics), out state);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                context.Dispose();
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            context.State = state;
            PushContext(context);

            return new GraphicsContainer(state);
        }

        public void EndContainer(GraphicsContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            int status = SafeNativeMethods.Gdip.GdipEndContainer(new HandleRef(this, NativeGraphics), container.nativeGraphicsContainer);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            PopContext(container.nativeGraphicsContainer);
        }

        public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        {
            GraphicsContext context = new GraphicsContext(this);
            int state = 0;

            GPRECT gpDest = new GPRECT(dstrect);
            GPRECT gpSrc = new GPRECT(srcrect);

            int status = SafeNativeMethods.Gdip.GdipBeginContainerI(new HandleRef(this, NativeGraphics), ref gpDest,
                                                     ref gpSrc, unchecked((int)unit), out state);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                context.Dispose();
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            context.State = state;
            PushContext(context);

            return new GraphicsContainer(state);
        }

        public void AddMetafileComment(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            int status = SafeNativeMethods.Gdip.GdipComment(new HandleRef(this, NativeGraphics), data.Length, data);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        public static IntPtr GetHalftonePalette()
        {
            if (s_halftonePalette == IntPtr.Zero)
            {
                lock (s_syncObject)
                {
                    if (s_halftonePalette == IntPtr.Zero)
                    {
                        AppDomain.CurrentDomain.DomainUnload += new EventHandler(OnDomainUnload);
                        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnDomainUnload);

                        s_halftonePalette = SafeNativeMethods.Gdip.GdipCreateHalftonePalette();
                    }
                }
            }
            return s_halftonePalette;
        }

        // This is called from AppDomain.ProcessExit and AppDomain.DomainUnload.
        [PrePrepareMethod]
        private static void OnDomainUnload(object sender, EventArgs e)
        {
            if (s_halftonePalette != IntPtr.Zero)
            {
                SafeNativeMethods.IntDeleteObject(new HandleRef(null, s_halftonePalette));
                s_halftonePalette = IntPtr.Zero;
            }
        }


        /// <summary>
        /// GDI+ will return a 'generic error' with specific win32 last error codes when
        /// a terminal server session has been closed, minimized, etc...  We don't want 
        /// to throw when this happens, so we'll guard against this by looking at the
        /// 'last win32 error code' and checking to see if it is either 1) access denied
        /// or 2) proc not found and then ignore it.
        /// 
        /// The problem is that when you lock the machine, the secure desktop is enabled and 
        /// rendering fails which is expected (since the app doesn't have permission to draw 
        /// on the secure desktop). Not sure if there's anything you can do, short of catching 
        /// the desktop switch message and absorbing all the exceptions that get thrown while 
        /// it's the secure desktop.
        /// </summary>
        private void CheckErrorStatus(int status)
        {
            if (status != SafeNativeMethods.Gdip.Ok)
            {
                // Generic error from GDI+ can be GenericError or Win32Error.
                if (status == SafeNativeMethods.Gdip.GenericError || status == SafeNativeMethods.Gdip.Win32Error)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == SafeNativeMethods.ERROR_ACCESS_DENIED || error == SafeNativeMethods.ERROR_PROC_NOT_FOUND ||
                            //here, we'll check to see if we are in a term. session...
                            (((UnsafeNativeMethods.GetSystemMetrics(NativeMethods.SM_REMOTESESSION) & 0x00000001) != 0) && (error == 0)))
                    {
                        return;
                    }
                }

                //legitimate error, throw our status exception
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// GDI+ will return a 'generic error' when we attempt to draw an Emf 
        /// image with width/height == 1.  Here, we will hack around this by 
        /// resetting the errorstatus.  Note that we don't do simple arg checking
        /// for height || width == 1 here because transforms can be applied to
        /// the Graphics object making it difficult to identify this scenario.
        /// </summary>
        private void IgnoreMetafileErrors(Image image, ref int errorStatus)
        {
            if (errorStatus != SafeNativeMethods.Gdip.Ok)
            {
                if (image.RawFormat.Equals(ImageFormat.Emf))
                {
                    errorStatus = SafeNativeMethods.Gdip.Ok;
                }
            }
        }
    }
}