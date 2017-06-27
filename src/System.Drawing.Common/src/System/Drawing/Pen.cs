// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Internal;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    /// <summary>
    /// Defines an object used to draw lines and curves.
    /// </summary>
    public sealed class Pen : MarshalByRefObject, ISystemColorTracker, ICloneable, IDisposable
    {
#if FINALIZATION_WATCH
        private string allocationSite = Graphics.GetAllocationStack();
#endif

        // handle to native GDI+ pen object.
        private IntPtr _nativePen;

        // GDI+ doesn't understand system colors, so we need to cache the value here
        private Color _color;
        private bool _immutable;

        /// <summary>
        /// Creates a Pen from a native GDI+ object.
        /// </summary>
        private Pen(IntPtr nativePen)
        {
            SetNativePen(nativePen);
        }


        internal Pen(Color color, bool immutable) : this(color)
        {
            _immutable = immutable;
        }

        /// <summary>
        /// Initializes a new instance of the Pen class with the specified <see cref='Color'/>.
        /// </summary>
        public Pen(Color color) : this(color, (float)1.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Pen'/> class with the specified
        /// <see cref='Color'/> and <see cref='Width'/>.
        /// </summary>
        public Pen(Color color, float width)
        {
            _color = color;

            IntPtr pen = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCreatePen1(color.ToArgb(),
                                                width,
                                                (int)GraphicsUnit.World,
                                                out pen);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);

            SetNativePen(pen);

#if FEATURE_SYSTEM_EVENTS
            if (this.color.IsSystemColor)
            {
                SystemColorTracker.Add(this);
            }
#endif
        }

        /// <summary>
        /// Initializes a new instance of the Pen class with the specified <see cref='Brush'/>.
        /// </summary>
        public Pen(Brush brush) : this(brush, (float)1.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Pen'/> class with the specified <see cref='Drawing.Brush'/> and width.
        /// </summary>
        public Pen(Brush brush, float width)
        {
            IntPtr pen = IntPtr.Zero;

            if (brush == null)
                throw new ArgumentNullException("brush");

            int status = SafeNativeMethods.Gdip.GdipCreatePen2(new HandleRef(brush, brush.NativeBrush),
                width,
                (int)GraphicsUnit.World,
                out pen);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);

            SetNativePen(pen);
        }

        internal void SetNativePen(IntPtr nativePen)
        {
            if (nativePen == IntPtr.Zero)
            {
                throw new ArgumentNullException("nativePen");
            }

            _nativePen = nativePen;
        }

        /// <summary>
        /// Gets the GDI+ native object.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        internal IntPtr NativePen
        {
            get
            {
                //Need to comment this line out to allow for checking this.NativePen == IntPtr.Zero.
                //Debug.Assert(this.nativePen != IntPtr.Zero, "this.nativePen == null." );
                return _nativePen;
            }
        }

        /// <summary>
        /// Creates an exact copy of this <see cref='System.Drawing.Pen'/>.
        /// </summary>
        public object Clone()
        {
            IntPtr clonePen = IntPtr.Zero;

            int status = SafeNativeMethods.Gdip.GdipClonePen(new HandleRef(this, NativePen), out clonePen);

            if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return new Pen(clonePen);
        }

        /// <summary>
        /// Cleans up Windows resources for this <see cref='System.Drawing.Pen'/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
#if FINALIZATION_WATCH
            if (!disposing && nativePen != IntPtr.Zero)
                Debug.WriteLine("**********************\nDisposed through finalization:\n" + allocationSite);
#endif

            if (!disposing)
            {
                // If we are finalizing, then we will be unreachable soon.  Finalize calls dispose to
                // release resources, so we must make sure that during finalization we are
                // not immutable.
                //
                _immutable = false;
            }
            else if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Brush"));
            }

            if (_nativePen != IntPtr.Zero)
            {
                try
                {
#if DEBUG
                    int status =
#endif
                    SafeNativeMethods.Gdip.GdipDeletePen(new HandleRef(this, NativePen));
#if DEBUG
                    Debug.Assert(status == SafeNativeMethods.Gdip.Ok, "GDI+ returned an error status: " + status.ToString(CultureInfo.InvariantCulture));
#endif       
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }

                    Debug.Fail("Exception thrown during Dispose: " + ex.ToString());
                }
                finally
                {
                    _nativePen = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// Cleans up Windows resources for this <see cref='System.Drawing.Pen'/>.
        /// </summary>
        ~Pen()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets the width of this <see cref='System.Drawing.Pen'/>.
        /// </summary>
        public float Width
        {
            get
            {
                float[] width = new float[] { 0 };

                int status = SafeNativeMethods.Gdip.GdipGetPenWidth(new HandleRef(this, NativePen), width);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return width[0];
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenWidth(new HandleRef(this, NativePen), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Sets the values that determine the style of cap used to end lines drawn by this <see cref='Pen'/>.
        /// </summary>
        public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
        {
            if (_immutable)
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));
            int status = SafeNativeMethods.Gdip.GdipSetPenLineCap197819(new HandleRef(this, NativePen),
                unchecked((int)startCap), unchecked((int)endCap), unchecked((int)dashCap));

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);
        }

        /// <summary>
        /// Gets or sets the cap style used at the beginning of lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public LineCap StartCap
        {
            get
            {
                int startCap = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenStartCap(new HandleRef(this, NativePen), out startCap);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (LineCap)startCap;
            }
            set
            {
                //validate the enum value
                switch (value)
                {
                    case LineCap.Flat:
                    case LineCap.Square:
                    case LineCap.Round:
                    case LineCap.Triangle:
                    case LineCap.NoAnchor:
                    case LineCap.SquareAnchor:
                    case LineCap.RoundAnchor:
                    case LineCap.DiamondAnchor:
                    case LineCap.ArrowAnchor:
                    case LineCap.AnchorMask:
                    case LineCap.Custom:
                        break;
                    default:
                        throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(LineCap));
                }
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenStartCap(new HandleRef(this, NativePen), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets the cap style used at the end of lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public LineCap EndCap
        {
            get
            {
                int endCap = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenEndCap(new HandleRef(this, NativePen), out endCap);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (LineCap)endCap;
            }
            set
            {
                //validate the enum value
                switch (value)
                {
                    case LineCap.Flat:
                    case LineCap.Square:
                    case LineCap.Round:
                    case LineCap.Triangle:
                    case LineCap.NoAnchor:
                    case LineCap.SquareAnchor:
                    case LineCap.RoundAnchor:
                    case LineCap.DiamondAnchor:
                    case LineCap.ArrowAnchor:
                    case LineCap.AnchorMask:
                    case LineCap.Custom:
                        break;
                    default:
                        throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(LineCap));
                }

                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenEndCap(new HandleRef(this, NativePen), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets the cap style used at the beginning or end of dashed lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public DashCap DashCap
        {
            get
            {
                int dashCap = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenDashCap197819(new HandleRef(this, NativePen), out dashCap);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (DashCap)dashCap;
            }

            set
            {
                //validate the enum value
                if (!ClientUtils.IsEnumValid_NotSequential(value, unchecked((int)value),
                                                    (int)DashCap.Flat,
                                                    (int)DashCap.Round,
                                                    (int)DashCap.Triangle))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(DashCap));
                }

                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenDashCap197819(new HandleRef(this, NativePen), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets the join style for the ends of two overlapping lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public LineJoin LineJoin
        {
            get
            {
                int lineJoin = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenLineJoin(new HandleRef(this, NativePen), out lineJoin);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (LineJoin)lineJoin;
            }

            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), (int)LineJoin.Miter, (int)LineJoin.MiterClipped))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(LineJoin));
                }

                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenLineJoin(new HandleRef(this, NativePen), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets a custom cap style to use at the beginning of lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public CustomLineCap CustomStartCap
        {
            get
            {
                IntPtr lineCap = IntPtr.Zero;
                int status = SafeNativeMethods.Gdip.GdipGetPenCustomStartCap(new HandleRef(this, NativePen), out lineCap);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return CustomLineCap.CreateCustomLineCapObject(lineCap);
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenCustomStartCap(new HandleRef(this, NativePen),
                                                              new HandleRef(value, (value == null) ? IntPtr.Zero : value.nativeCap));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets a custom cap style to use at the end of lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public CustomLineCap CustomEndCap
        {
            get
            {
                IntPtr lineCap = IntPtr.Zero;
                int status = SafeNativeMethods.Gdip.GdipGetPenCustomEndCap(new HandleRef(this, NativePen), out lineCap);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return CustomLineCap.CreateCustomLineCapObject(lineCap);
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenCustomEndCap(new HandleRef(this, NativePen),
                                                            new HandleRef(value, (value == null) ? IntPtr.Zero : value.nativeCap));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets the limit of the thickness of the join on a mitered corner.
        /// </summary>
        public float MiterLimit
        {
            get
            {
                float[] miterLimit = new float[] { 0 };
                int status = SafeNativeMethods.Gdip.GdipGetPenMiterLimit(new HandleRef(this, NativePen), miterLimit);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return miterLimit[0];
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenMiterLimit(new HandleRef(this, NativePen), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets the alignment for objects drawn with this <see cref='Pen'/>.
        /// </summary>
        public PenAlignment Alignment
        {
            get
            {
                PenAlignment penMode = 0;

                int status = SafeNativeMethods.Gdip.GdipGetPenMode(new HandleRef(this, NativePen), out penMode);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (PenAlignment)penMode;
            }
            set
            {
                //validate the enum value
                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), (int)PenAlignment.Center, (int)PenAlignment.Right))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(PenAlignment));
                }

                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenMode(new HandleRef(this, NativePen), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets the geometrical transform for objects drawn with this <see cref='Pen'/>.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                Matrix matrix = new Matrix();

                int status = SafeNativeMethods.Gdip.GdipGetPenTransform(new HandleRef(this, NativePen), new HandleRef(matrix, matrix.nativeMatrix));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return matrix;
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenTransform(new HandleRef(this, NativePen), new HandleRef(value, value.nativeMatrix));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Resets the geometric transform for this <see cref='Pen'/> to identity.
        /// </summary>
        public void ResetTransform()
        {
            int status = SafeNativeMethods.Gdip.GdipResetPenTransform(new HandleRef(this, NativePen));

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);
        }

        /// <summary>
        /// Multiplies the transform matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/>.
        /// </summary>
        public void MultiplyTransform(Matrix matrix)
        {
            MultiplyTransform(matrix, MatrixOrder.Prepend);
        }

        /// <summary>
        /// Multiplies the transform matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/> in the specified order.
        /// </summary>
        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipMultiplyPenTransform(new HandleRef(this, NativePen),
                                                          new HandleRef(matrix, matrix.nativeMatrix),
                                                          order);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);
        }

        /// <summary>
        /// Translates the local geometrical transform by the specified dimmensions. This method prepends the translation
        /// to the transform.
        /// </summary>
        public void TranslateTransform(float dx, float dy)
        {
            TranslateTransform(dx, dy, MatrixOrder.Prepend);
        }

        /// <summary>
        /// Translates the local geometrical transform by the specified dimmensions in the specified order.
        /// </summary>
        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipTranslatePenTransform(new HandleRef(this, NativePen),
                                                           dx, dy, order);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);
        }

        /// <summary>
        /// Scales the local geometric transform by the specified amounts. This method prepends the scaling matrix to the transform.
        /// </summary>
        public void ScaleTransform(float sx, float sy)
        {
            ScaleTransform(sx, sy, MatrixOrder.Prepend);
        }

        /// <summary>
        /// Scales the local geometric transform by the specified amounts in the specified order.
        /// </summary>
        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipScalePenTransform(new HandleRef(this, NativePen),
                                                       sx, sy, order);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);
        }

        /// <summary>
        /// Rotates the local geometric transform by the specified amount. This method prepends the rotation to the transform.
        /// </summary>
        public void RotateTransform(float angle)
        {
            RotateTransform(angle, MatrixOrder.Prepend);
        }

        /// <summary>
        /// Rotates the local geometric transform by the specified amount in the specified order.
        /// </summary>
        public void RotateTransform(float angle, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipRotatePenTransform(new HandleRef(this, NativePen),
                                                        angle, order);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);
        }

        private void InternalSetColor(Color value)
        {
            int status = SafeNativeMethods.Gdip.GdipSetPenColor(new HandleRef(this, NativePen),
                                                 _color.ToArgb());
            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);

            _color = value;
        }

        /// <summary>
        /// Gets the style of lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public PenType PenType
        {
            get
            {
                int type = -1;

                int status = SafeNativeMethods.Gdip.GdipGetPenFillType(new HandleRef(this, NativePen), out type);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (PenType)type;
            }
        }

        /// <summary>
        /// Gets or sets the color of this <see cref='Pen'/>.
        /// </summary>
        public Color Color
        {
            get
            {
                if (_color == Color.Empty)
                {
                    int colorARGB = 0;
                    int status = SafeNativeMethods.Gdip.GdipGetPenColor(new HandleRef(this, NativePen), out colorARGB);

                    if (status != SafeNativeMethods.Gdip.Ok)
                        throw SafeNativeMethods.Gdip.StatusException(status);

                    _color = Color.FromArgb(colorARGB);
                }

                // GDI+ doesn't understand system colors, so we can't use GdipGetPenColor in the general case
                return _color;
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                if (value != _color)
                {
                    Color oldColor = _color;
                    _color = value;
                    InternalSetColor(value);

#if FEATURE_SYSTEM_EVENTS
                    // NOTE: We never remove pens from the active list, so if someone is
                    // changing their pen colors a lot, this could be a problem.
                    if (value.IsSystemColor && !oldColor.IsSystemColor)
                    {
                        SystemColorTracker.Add(this);
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref='Drawing.Brush'/> that determines attributes of this <see cref='Pen'/>.
        /// </summary>
        public Brush Brush
        {
            get
            {
                Brush brush = null;

                switch (PenType)
                {
                    case PenType.SolidColor:
                        brush = new SolidBrush(GetNativeBrush());
                        break;

                    case PenType.HatchFill:
                        brush = new HatchBrush(GetNativeBrush());
                        break;

                    case PenType.TextureFill:
                        brush = new TextureBrush(GetNativeBrush());
                        break;

                    case PenType.PathGradient:
                        brush = new PathGradientBrush(GetNativeBrush());
                        break;

                    case PenType.LinearGradient:
                        brush = new LinearGradientBrush(GetNativeBrush());
                        break;

                    default:
                        break;
                }

                return brush;
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                if (value == null)
                    throw new ArgumentNullException("value");

                int status = SafeNativeMethods.Gdip.GdipSetPenBrushFill(new HandleRef(this, NativePen),
                    new HandleRef(value, value.NativeBrush));

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        private IntPtr GetNativeBrush()
        {
            IntPtr nativeBrush = IntPtr.Zero;

            int status = SafeNativeMethods.Gdip.GdipGetPenBrushFill(new HandleRef(this, NativePen), out nativeBrush);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);

            return nativeBrush;
        }

        /// <summary>
        /// Gets or sets the style used for dashed lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public DashStyle DashStyle
        {
            get
            {
                int dashstyle = 0;

                int status = SafeNativeMethods.Gdip.GdipGetPenDashStyle(new HandleRef(this, NativePen), out dashstyle);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return (DashStyle)dashstyle;
            }

            set
            {
                //valid values are 0x0 to 0x5
                if (!ClientUtils.IsEnumValid(value, unchecked((int)value), (int)DashStyle.Solid, (int)DashStyle.Custom))
                {
                    throw new InvalidEnumArgumentException("value", unchecked((int)value), typeof(DashStyle));
                }

                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenDashStyle(new HandleRef(this, NativePen), unchecked((int)value));

                if (status != SafeNativeMethods.Gdip.Ok)
                {
                    throw SafeNativeMethods.Gdip.StatusException(status);
                }

                //if we just set pen style to "custom" without defining the custom dash pattern,
                //lets make sure we can return a valid value...
                //
                if (value == DashStyle.Custom)
                {
                    EnsureValidDashPattern();
                }
            }
        }

        /// <summary>
        /// This method is called after the user sets the pen's dash style to custom. Here, we make sure that there
        /// is a default value set for the custom pattern.
        /// </summary>
        private void EnsureValidDashPattern()
        {
            int retval = 0;
            int status = SafeNativeMethods.Gdip.GdipGetPenDashCount(new HandleRef(this, NativePen), out retval);

            if (status != SafeNativeMethods.Gdip.Ok)
                throw SafeNativeMethods.Gdip.StatusException(status);

            if (retval == 0)
            {
                //just set to a solid pattern
                DashPattern = new float[] { 1 };
            }
        }

        /// <summary>
        /// Gets or sets the distance from the start of a line to the beginning of a dash pattern.
        /// </summary>
        public float DashOffset
        {
            get
            {
                float[] dashoffset = new float[] { 0 };

                int status = SafeNativeMethods.Gdip.GdipGetPenDashOffset(new HandleRef(this, NativePen), dashoffset);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return dashoffset[0];
            }
            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenDashOffset(new HandleRef(this, NativePen), value);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Gets or sets an array of cutom dashes and spaces. The dashes are made up of line segments.
        /// </summary>
        public float[] DashPattern
        {
            get
            {
                float[] dashArray;

                // Figure out how many dash elements we have

                int retval = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenDashCount(new HandleRef(this, NativePen), out retval);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                int count = retval;

                // Allocate temporary native memory buffer
                // and pass it to GDI+ to retrieve dash array elements

                IntPtr buf = Marshal.AllocHGlobal(checked(4 * count));
                status = SafeNativeMethods.Gdip.GdipGetPenDashArray(new HandleRef(this, NativePen), buf, count);

                try
                {
                    if (status != SafeNativeMethods.Gdip.Ok)
                    {
                        throw SafeNativeMethods.Gdip.StatusException(status);
                    }

                    dashArray = new float[count];

                    Marshal.Copy(buf, dashArray, 0, count);
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }

                return dashArray;
            }

            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));


                //validate the DashPattern value being set
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentException(SR.Format(SR.InvalidDashPattern));
                }

                int count = value.Length;

                IntPtr buf = Marshal.AllocHGlobal(checked(4 * count));

                try
                {
                    Marshal.Copy(value, 0, buf, count);

                    int status = SafeNativeMethods.Gdip.GdipSetPenDashArray(new HandleRef(this, NativePen), new HandleRef(buf, buf), count);

                    if (status != SafeNativeMethods.Gdip.Ok)
                    {
                        throw SafeNativeMethods.Gdip.StatusException(status);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of cutom dashes and spaces. The dashes are made up of line segments.
        /// </summary>
        public float[] CompoundArray
        {
            get
            {
                int count = 0;

                int status = SafeNativeMethods.Gdip.GdipGetPenCompoundCount(new HandleRef(this, NativePen), out count);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                float[] array = new float[count];

                status = SafeNativeMethods.Gdip.GdipGetPenCompoundArray(new HandleRef(this, NativePen), array, count);
                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);

                return array;
            }
            set
            {
                if (_immutable)
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, "Pen"));

                int status = SafeNativeMethods.Gdip.GdipSetPenCompoundArray(new HandleRef(this, NativePen), value, value.Length);

                if (status != SafeNativeMethods.Gdip.Ok)
                    throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        void ISystemColorTracker.OnSystemColorChanged()
        {
            if (NativePen != IntPtr.Zero)
                InternalSetColor(_color);
        }
    }
}
