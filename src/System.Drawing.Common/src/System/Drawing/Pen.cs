﻿// Licensed to the .NET Foundation under one or more agreements.
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
    public sealed partial class Pen : MarshalByRefObject, ICloneable, IDisposable
#if FEATURE_SYSTEM_EVENTS
        , ISystemColorTracker
#endif
    {
#if FINALIZATION_WATCH
        private string allocationSite = Graphics.GetAllocationStack();
#endif

        // Handle to native GDI+ pen object.
        private IntPtr _nativePen;

        // GDI+ doesn't understand system colors, so we need to cache the value here.
        private Color _color;
        private bool _immutable;

        // Tracks whether the dash style has been changed to something else than Solid during the lifetime of this object.
        private bool _dashStyleWasOrIsNotSolid;

        /// <summary>
        /// Creates a Pen from a native GDI+ object.
        /// </summary>
        private Pen(IntPtr nativePen) => SetNativePen(nativePen);

        internal Pen(Color color, bool immutable) : this(color) =>  _immutable = immutable;

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
            SafeNativeMethods.Gdip.CheckStatus(status);

            SetNativePen(pen);

#if FEATURE_SYSTEM_EVENTS
            if (ColorUtil.IsSystemColor(_color))
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
            if (brush == null)
            {
                throw new ArgumentNullException(nameof(brush));
            }

            IntPtr pen = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCreatePen2(new HandleRef(brush, brush.NativeBrush),
                width,
                (int)GraphicsUnit.World,
                out pen);
            SafeNativeMethods.Gdip.CheckStatus(status);

            SetNativePen(pen);
        }

        internal void SetNativePen(IntPtr nativePen)
        {
            Debug.Assert(nativePen != IntPtr.Zero);
            _nativePen = nativePen;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        internal IntPtr NativePen => _nativePen;

        /// <summary>
        /// Creates an exact copy of this <see cref='System.Drawing.Pen'/>.
        /// </summary>
        public object Clone()
        {
            IntPtr clonedPen = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipClonePen(new HandleRef(this, NativePen), out clonedPen);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new Pen(clonedPen);
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
            {
                Debug.WriteLine("**********************\nDisposed through finalization:\n" + allocationSite);
            }
#endif

            if (!disposing)
            {
                // If we are finalizing, then we will be unreachable soon. Finalize calls dispose to
                // release resources, so we must make sure that during finalization we are
                // not immutable.
                _immutable = false;
            }
            else if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Brush)));
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
                catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
                {
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
        ~Pen() => Dispose(false);

        /// <summary>
        /// Gets or sets the width of this <see cref='System.Drawing.Pen'/>.
        /// </summary>
        public float Width
        {
            get
            {
                var width = new float[] { 0 };
                int status = SafeNativeMethods.Gdip.GdipGetPenWidth(new HandleRef(this, NativePen), width);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return width[0];
            }
            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenWidth(new HandleRef(this, NativePen), value);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        /// <summary>
        /// Sets the values that determine the style of cap used to end lines drawn by this <see cref='Pen'/>.
        /// </summary>
        public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            int status = SafeNativeMethods.Gdip.GdipSetPenLineCap197819(new HandleRef(this, NativePen),
                unchecked((int)startCap), unchecked((int)endCap), unchecked((int)dashCap));
                SafeNativeMethods.Gdip.CheckStatus(status);
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
                SafeNativeMethods.Gdip.CheckStatus(status);

                return (LineCap)startCap;
            }
            set
            {
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
                        throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(LineCap));
                }
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenStartCap(new HandleRef(this, NativePen), unchecked((int)value));
                SafeNativeMethods.Gdip.CheckStatus(status);
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
                SafeNativeMethods.Gdip.CheckStatus(status);

                return (LineCap)endCap;
            }
            set
            {
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
                        throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(LineCap));
                }

                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenEndCap(new HandleRef(this, NativePen), unchecked((int)value));
                SafeNativeMethods.Gdip.CheckStatus(status);
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
                SafeNativeMethods.Gdip.CheckStatus(status);

                return (DashCap)dashCap;
            }
            set
            {
                if (value != DashCap.Flat && value != DashCap.Round && value != DashCap.Triangle)
                {
                    throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(DashCap));
                }

                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenDashCap197819(new HandleRef(this, NativePen), unchecked((int)value));
                SafeNativeMethods.Gdip.CheckStatus(status);
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
                SafeNativeMethods.Gdip.CheckStatus(status);

                return (LineJoin)lineJoin;
            }
            set
            {
                if (value < LineJoin.Miter || value > LineJoin.MiterClipped)
                {
                    throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(LineJoin));
                }

                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenLineJoin(new HandleRef(this, NativePen), unchecked((int)value));
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        /// <summary>
        /// Gets or sets the limit of the thickness of the join on a mitered corner.
        /// </summary>
        public float MiterLimit
        {
            get
            {
                var miterLimit = new float[] { 0 };
                int status = SafeNativeMethods.Gdip.GdipGetPenMiterLimit(new HandleRef(this, NativePen), miterLimit);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return miterLimit[0];
            }
            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenMiterLimit(new HandleRef(this, NativePen), value);
                SafeNativeMethods.Gdip.CheckStatus(status);
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
                SafeNativeMethods.Gdip.CheckStatus(status);

                return penMode;
            }
            set
            {
                if (value < PenAlignment.Center || value > PenAlignment.Right)
                {
                    throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(PenAlignment));
                }

                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenMode(new HandleRef(this, NativePen), value);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        /// <summary>
        /// Gets or sets the geometrical transform for objects drawn with this <see cref='Pen'/>.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                var matrix = new Matrix();
                int status = SafeNativeMethods.Gdip.GdipGetPenTransform(new HandleRef(this, NativePen), new HandleRef(matrix, matrix.nativeMatrix));
                SafeNativeMethods.Gdip.CheckStatus(status);

                return matrix;
            }

            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenTransform(new HandleRef(this, NativePen), new HandleRef(value, value.nativeMatrix));
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        /// <summary>
        /// Resets the geometric transform for this <see cref='Pen'/> to identity.
        /// </summary>
        public void ResetTransform()
        {
            int status = SafeNativeMethods.Gdip.GdipResetPenTransform(new HandleRef(this, NativePen));
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Multiplies the transform matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/>.
        /// </summary>
        public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

        /// <summary>
        /// Multiplies the transform matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/> in the specified order.
        /// </summary>
        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            if (matrix.nativeMatrix == IntPtr.Zero)
            {
                // Disposed matrices should result in a no-op.
                return;
            }

            int status = SafeNativeMethods.Gdip.GdipMultiplyPenTransform(new HandleRef(this, NativePen),
                                                          new HandleRef(matrix, matrix.nativeMatrix),
                                                          order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Translates the local geometrical transform by the specified dimensions. This method prepends the translation
        /// to the transform.
        /// </summary>
        public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

        /// <summary>
        /// Translates the local geometrical transform by the specified dimensions in the specified order.
        /// </summary>
        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipTranslatePenTransform(new HandleRef(this, NativePen),
                                                           dx, dy, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Scales the local geometric transform by the specified amounts. This method prepends the scaling matrix to the transform.
        /// </summary>
        public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

        /// <summary>
        /// Scales the local geometric transform by the specified amounts in the specified order.
        /// </summary>
        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipScalePenTransform(new HandleRef(this, NativePen),
                                                       sx, sy, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Rotates the local geometric transform by the specified amount. This method prepends the rotation to the transform.
        /// </summary>
        public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

        /// <summary>
        /// Rotates the local geometric transform by the specified amount in the specified order.
        /// </summary>
        public void RotateTransform(float angle, MatrixOrder order)
        {
            int status = SafeNativeMethods.Gdip.GdipRotatePenTransform(new HandleRef(this, NativePen),
                                                        angle, order);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        private void InternalSetColor(Color value)
        {
            int status = SafeNativeMethods.Gdip.GdipSetPenColor(new HandleRef(this, NativePen),
                                                 _color.ToArgb());
            SafeNativeMethods.Gdip.CheckStatus(status);

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
                SafeNativeMethods.Gdip.CheckStatus(status);

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
                    SafeNativeMethods.Gdip.CheckStatus(status);

                    _color = Color.FromArgb(colorARGB);
                }

                // GDI+ doesn't understand system colors, so we can't use GdipGetPenColor in the general case.
                return _color;
            }
            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                if (value != _color)
                {
                    Color oldColor = _color;
                    _color = value;
                    InternalSetColor(value);

#if FEATURE_SYSTEM_EVENTS
                    // NOTE: We never remove pens from the active list, so if someone is
                    // changing their pen colors a lot, this could be a problem.
                    if (ColorUtil.IsSystemColor(value) && !ColorUtil.IsSystemColor(oldColor))
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
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenBrushFill(new HandleRef(this, NativePen),
                    new HandleRef(value, value.NativeBrush));
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        private IntPtr GetNativeBrush()
        {
            IntPtr nativeBrush = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipGetPenBrushFill(new HandleRef(this, NativePen), out nativeBrush);
                SafeNativeMethods.Gdip.CheckStatus(status);

            return nativeBrush;
        }

        /// <summary>
        /// Gets or sets the style used for dashed lines drawn with this <see cref='Pen'/>.
        /// </summary>
        public DashStyle DashStyle
        {
            get
            {
                int dashStyle = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenDashStyle(new HandleRef(this, NativePen), out dashStyle);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return (DashStyle)dashStyle;
            }
            set
            {
                if (value < DashStyle.Solid || value > DashStyle.Custom)
                {
                    throw new InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(DashStyle));
                }

                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenDashStyle(new HandleRef(this, NativePen), unchecked((int)value));
                SafeNativeMethods.Gdip.CheckStatus(status);

                // If we just set the pen style to Custom without defining the custom dash pattern,
                // make sure that we can return a valid value.
                if (value == DashStyle.Custom)
                {
                    EnsureValidDashPattern();
                }

                if (value != DashStyle.Solid)
                {
                    this._dashStyleWasOrIsNotSolid = true;
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
            SafeNativeMethods.Gdip.CheckStatus(status);

            if (retval == 0)
            {
                // Set to a solid pattern.
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
                var dashOffset = new float[] { 0 };
                int status = SafeNativeMethods.Gdip.GdipGetPenDashOffset(new HandleRef(this, NativePen), dashOffset);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return dashOffset[0];
            }
            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenDashOffset(new HandleRef(this, NativePen), value);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

        /// <summary>
        /// Gets or sets an array of custom dashes and spaces. The dashes are made up of line segments.
        /// </summary>
        public float[] DashPattern
        {
            get
            {
                int status = SafeNativeMethods.Gdip.GdipGetPenDashCount(new HandleRef(this, NativePen), out int count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                float[] pattern;
                // don't call GdipGetPenDashArray with a 0 count
                if (count > 0)
                {
                    pattern = new float[count];
                    status = SafeNativeMethods.Gdip.GdipGetPenDashArray(new HandleRef(this, NativePen), pattern, count);
                    SafeNativeMethods.Gdip.CheckStatus(status);
                }
                else if (DashStyle == DashStyle.Solid && !this._dashStyleWasOrIsNotSolid)
                {
                    // Most likely we're replicating an existing System.Drawing bug here, it doesn't make much sense to
                    // ask for a dash pattern when using a solid dash.
                    throw new OutOfMemoryException();
                }
                else if (DashStyle == DashStyle.Solid)
                {
                    pattern = Array.Empty<float>();
                }
                else
                {
                    // special case (not handled inside GDI+)
                    pattern = new float[1];
                    pattern[0] = 1.0f;
                }

                return pattern;
            }
            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }


                if (value == null || value.Length == 0)
                {
                    throw new ArgumentException(SR.Format(SR.InvalidDashPattern));
                }

                foreach (float val in value)
                {
                    if (val <= 0)
                    {
                        throw new ArgumentException(SR.Format(SR.InvalidDashPattern));
                    }
                }

                int count = value.Length;
                IntPtr buf = Marshal.AllocHGlobal(checked(4 * count));

                try
                {
                    Marshal.Copy(value, 0, buf, count);

                    int status = SafeNativeMethods.Gdip.GdipSetPenDashArray(new HandleRef(this, NativePen), new HandleRef(buf, buf), count);
                    SafeNativeMethods.Gdip.CheckStatus(status);
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of custom dashes and spaces. The dashes are made up of line segments.
        /// </summary>
        public float[] CompoundArray
        {
            get
            {
                int count = 0;
                int status = SafeNativeMethods.Gdip.GdipGetPenCompoundCount(new HandleRef(this, NativePen), out count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                var array = new float[count];
                status = SafeNativeMethods.Gdip.GdipGetPenCompoundArray(new HandleRef(this, NativePen), array, count);
                SafeNativeMethods.Gdip.CheckStatus(status);

                return array;
            }
            set
            {
                if (_immutable)
                {
                    throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
                }

                if (value.Length <= 1)
                {
                    throw new ArgumentException(SR.Format(SR.GdiplusInvalidParameter));
                }

                foreach (float val in value)
                {
                    if (val < 0 || val > 1)
                    {
                        throw new ArgumentException(SR.Format(SR.GdiplusInvalidParameter));
                    }
                }

                int status = SafeNativeMethods.Gdip.GdipSetPenCompoundArray(new HandleRef(this, NativePen), value, value.Length);
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }

#if FEATURE_SYSTEM_EVENTS
        void ISystemColorTracker.OnSystemColorChanged()
        {
            if (NativePen != IntPtr.Zero)
            {
                InternalSetColor(_color);
            }
        }
#endif
    }
}
