// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{
    /// <summary>
    /// Defines a particular format for text, including font face, size, and style attributes.
    /// </summary>
#if !NETCORE
    [Editor ("System.Drawing.Design.FontEditor, " + Consts.AssemblySystem_Drawing_Design, typeof (System.Drawing.Design.UITypeEditor))]
    [TypeConverter (typeof (FontConverter))]
#endif
    public sealed partial class Font : MarshalByRefObject, ICloneable, IDisposable, ISerializable
    {
        private const int LogFontCharSetOffset = 23;
        private const int LogFontNameOffset = 28;

        private float _fontSize;
        private FontStyle _fontStyle;
        private FontFamily _fontFamily;
        private GraphicsUnit _fontUnit;
        private byte _gdiCharSet = SafeNativeMethods.DEFAULT_CHARSET;
        private bool _gdiVerticalFont;
        private string _systemFontName = "";
        private string _originalFontName;
        private IntPtr _nativeFont = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class from the specified existing <see cref='Font'/>
        /// and <see cref='FontStyle'/>.
        /// </summary>
        public Font(Font prototype, FontStyle newStyle)
        {
            // Copy over the originalFontName because it won't get initialized
            _originalFontName = prototype.OriginalFontName;
            Initialize(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, SafeNativeMethods.DEFAULT_CHARSET, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
        {
            Initialize(family, emSize, style, unit, SafeNativeMethods.DEFAULT_CHARSET, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
        {
            Initialize(family, emSize, style, unit, gdiCharSet, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
        {
            Initialize(family, emSize, style, unit, gdiCharSet, gdiVerticalFont);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
        {
            Initialize(familyName, emSize, style, unit, gdiCharSet, IsVerticalName(familyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
        {
            if (float.IsNaN(emSize) || float.IsInfinity(emSize) || emSize <= 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidBoundArgument, "emSize", emSize, 0, "System.Single.MaxValue"), "emSize");
            }

            Initialize(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(FontFamily family, float emSize, FontStyle style)
        {
            Initialize(family, emSize, style, GraphicsUnit.Point, SafeNativeMethods.DEFAULT_CHARSET, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(FontFamily family, float emSize, GraphicsUnit unit)
        {
            Initialize(family, emSize, FontStyle.Regular, unit, SafeNativeMethods.DEFAULT_CHARSET, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(FontFamily family, float emSize)
        {
            Initialize(family, emSize, FontStyle.Regular, GraphicsUnit.Point, SafeNativeMethods.DEFAULT_CHARSET, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit)
        {
            Initialize(familyName, emSize, style, unit, SafeNativeMethods.DEFAULT_CHARSET, IsVerticalName(familyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(string familyName, float emSize, FontStyle style)
        {
            Initialize(familyName, emSize, style, GraphicsUnit.Point, SafeNativeMethods.DEFAULT_CHARSET, IsVerticalName(familyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(string familyName, float emSize, GraphicsUnit unit)
        {
            Initialize(familyName, emSize, FontStyle.Regular, unit, SafeNativeMethods.DEFAULT_CHARSET, IsVerticalName(familyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
        /// </summary>
        public Font(string familyName, float emSize)
        {
            Initialize(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, SafeNativeMethods.DEFAULT_CHARSET, IsVerticalName(familyName));
        }

        ///<summary>
        /// Creates the GDI+ native font object.
        ///</summary>
        private void CreateNativeFont()
        {
            Debug.Assert(_nativeFont == IntPtr.Zero, "nativeFont already initialized, this will generate a handle leak.");
            Debug.Assert(_fontFamily != null, "fontFamily not initialized.");

            // Note: GDI+ creates singleton font family objects (from the corresponding font file) and reference count them so
            // if creating the font object from an external FontFamily, this object's FontFamily will share the same native object.
            int status = SafeNativeMethods.Gdip.GdipCreateFont(
                                    new HandleRef(this, _fontFamily.NativeFamily),
                                    _fontSize,
                                    _fontStyle,
                                    _fontUnit,
                                    out _nativeFont);

            // Special case this common error message to give more information
            if (status == SafeNativeMethods.Gdip.FontStyleNotFound)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusFontStyleNotFound, _fontFamily.Name, _fontStyle.ToString()));
            }
            else if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }
        }

        /// <summary>
        /// Initializes this object's fields.
        /// </summary>
        private void Initialize(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
        {
            _originalFontName = familyName;

            SetFontFamily(new FontFamily(StripVerticalName(familyName), createDefaultOnFail: true));
            Initialize(_fontFamily, emSize, style, unit, gdiCharSet, gdiVerticalFont);
        }

        /// <summary>
        /// Initializes this object's fields.
        /// </summary>
        private void Initialize(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }

            if (float.IsNaN(emSize) || float.IsInfinity(emSize) || emSize <= 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidBoundArgument, nameof(emSize), emSize, 0, "System.Single.MaxValue"), nameof(emSize));
            }

            int status;

            _fontSize = emSize;
            _fontStyle = style;
            _fontUnit = unit;
            _gdiCharSet = gdiCharSet;
            _gdiVerticalFont = gdiVerticalFont;

            if (_fontFamily == null)
            {
                // GDI+ FontFamily is a singleton object.
                SetFontFamily(new FontFamily(family.NativeFamily));
            }

            if (_nativeFont == IntPtr.Zero)
            {
                CreateNativeFont();
            }

            // Get actual size.
            status = SafeNativeMethods.Gdip.GdipGetFontSize(new HandleRef(this, _nativeFont), out _fontSize);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Constructor to initialize fields from an existing native GDI+ object reference. Used by ToLogFont.
        /// </summary>
        private Font(IntPtr nativeFont, byte gdiCharSet, bool gdiVerticalFont)
        {
            Debug.Assert(_nativeFont == IntPtr.Zero, "GDI+ native font already initialized, this will generate a handle leak");
            Debug.Assert(nativeFont != IntPtr.Zero, "nativeFont is null");

            int status = 0;
            float size = 0;
            GraphicsUnit unit = GraphicsUnit.Point;
            FontStyle style = FontStyle.Regular;
            IntPtr nativeFamily = IntPtr.Zero;

            _nativeFont = nativeFont;

            status = SafeNativeMethods.Gdip.GdipGetFontUnit(new HandleRef(this, nativeFont), out unit);
            SafeNativeMethods.Gdip.CheckStatus(status);

            status = SafeNativeMethods.Gdip.GdipGetFontSize(new HandleRef(this, nativeFont), out size);
            SafeNativeMethods.Gdip.CheckStatus(status);

            status = SafeNativeMethods.Gdip.GdipGetFontStyle(new HandleRef(this, nativeFont), out style);
            SafeNativeMethods.Gdip.CheckStatus(status);

            status = SafeNativeMethods.Gdip.GdipGetFamily(new HandleRef(this, nativeFont), out nativeFamily);
            SafeNativeMethods.Gdip.CheckStatus(status);

            SetFontFamily(new FontFamily(nativeFamily));
            Initialize(_fontFamily, size, style, unit, gdiCharSet, gdiVerticalFont);
        }

        public static Font FromLogFont(object lf, IntPtr hdc)
        {
            IntPtr font = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCreateFontFromLogfontW(new HandleRef(null, hdc), lf, out font);

            // Special case this incredibly common error message to give more information
            if (status == SafeNativeMethods.Gdip.NotTrueTypeFont)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusNotTrueTypeFont_NoName));
            }
            else if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            // GDI+ returns font = 0 even though the status is Ok.
            if (font == IntPtr.Zero)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusNotTrueTypeFont, lf.ToString()));
            }

#pragma warning disable 0618
            bool gdiVerticalFont = (Marshal.ReadInt16(lf, LogFontNameOffset) == (short)'@');
            return new Font(font, Marshal.ReadByte(lf, LogFontCharSetOffset), gdiVerticalFont);
#pragma warning restore 0618
        }

        /// <summary>
        /// Gets the face name of this <see cref='Font'/> .
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !NETCORE
        [Editor("System.Drawing.Design.FontNameEditor, " + Consts.AssemblySystem_Drawing_Design, typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(FontConverter.FontNameConverter))]
#endif
        public string Name => FontFamily.Name;

        /// <summary>
        /// Gets the <see cref='Drawing.FontFamily'/> of this <see cref='Font'/>.
        /// </summary>
        [Browsable(false)]
        public FontFamily FontFamily
        {
            get
            {
                Debug.Assert(_fontFamily != null, "fontFamily should never be null");
                return _fontFamily;
            }
        }

        // Return value is in Unit (the unit the font was created in)
        /// <summary>
        /// Gets the size of this <see cref='Font'/>.
        /// </summary>
        public float Size => _fontSize;

        /// <summary>
        /// Gets style information for this <see cref='Font'/>.
        /// </summary>
        [Browsable(false)]
        public FontStyle Style => _fontStyle;

        /// <summary>
        /// Gets a value indicating whether this <see cref='System.Drawing.Font'/> is bold.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Bold => (Style & FontStyle.Bold) != 0;

        /// <summary>
        /// Gets a value indicating whether this <see cref='Font'/> is Italic.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Italic => (Style & FontStyle.Italic) != 0;

        /// <summary>
        /// Gets a value indicating whether this <see cref='Font'/> is strikeout (has a line through it).
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Strikeout => (Style & FontStyle.Strikeout) != 0;

        /// <summary>
        /// Gets a value indicating whether this <see cref='Font'/> is underlined.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Underline => (Style & FontStyle.Underline) != 0;

        /// <summary>
        /// Gets the unit of measure for this <see cref='Font'/>.
        /// </summary>
#if !NETCORE
        [TypeConverter(typeof(FontConverter.FontUnitConverter))]
#endif
        public GraphicsUnit Unit => _fontUnit;

        /// <summary>
        /// Returns the GDI char set for this instance of a font. This will only
        /// be valid if this font was created from a classic GDI font definition,
        /// like a LOGFONT or HFONT, or it was passed into the constructor.
        ///
        /// This is here for compatibility with native Win32 intrinsic controls
        /// on non-Unicode platforms.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte GdiCharSet => _gdiCharSet;

        /// <summary>
        /// Determines if this font was created to represent a GDI vertical font. This will only be valid if this font
        /// was created from a classic GDIfont definition, like a LOGFONT or HFONT, or it was passed into the constructor.
        ///
        /// This is here for compatibility with native Win32 intrinsic controls on non-Unicode platforms.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GdiVerticalFont => _gdiVerticalFont;

        /// <summary>
        /// This property is required by the framework and not intended to be used directly.
        /// </summary>
        [Browsable(false)]
        public string OriginalFontName => _originalFontName;

        /// <summary>
        /// Returns true if this <see cref='Font'/> is a SystemFont.
        /// </summary>
        [Browsable(false)]
        public bool IsSystemFont => !string.IsNullOrEmpty(_systemFontName);

        /// <summary>
        /// Gets the name of this <see cref='Drawing.SystemFont'/>.
        /// </summary>
        [Browsable(false)]
        public string SystemFontName => _systemFontName;

        /// <summary>
        /// Gets the height of this <see cref='Font'/>.
        /// </summary>
        [Browsable(false)]
        public int Height => (int)Math.Ceiling(GetHeight());

        /// <summary>
        /// Get native GDI+ object pointer. This property triggers the creation of the GDI+ native object if not initialized yet.
        /// </summary>
        internal IntPtr NativeFont => _nativeFont;

        /// <summary>
        /// Returns the height of this Font in the specified graphics context.
        /// </summary>
        public float GetHeight(Graphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            float height;
            int status = SafeNativeMethods.Gdip.GdipGetFontHeight(new HandleRef(this, NativeFont), new HandleRef(graphics, graphics.NativeGraphics), out height);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return height;
        }

        public float GetHeight(float dpi)
        {
            float height;
            int status = SafeNativeMethods.Gdip.GdipGetFontHeightGivenDPI(new HandleRef(this, NativeFont), dpi, out height);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return height;
        }

        /// <summary>
        /// Returns a value indicating whether the specified object is a <see cref='Font'/> equivalent to this
        /// <see cref='Font'/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is Font font))
            {
                return false;
            }

            // Note: If this and/or the passed-in font are disposed, this method can still return true since we check for cached properties
            // here.
            // We need to call properties on the passed-in object since it could be a proxy in a remoting scenario and proxies don't
            // have access to private/internal fields.
            return font.FontFamily.Equals(FontFamily) &&
                font.GdiVerticalFont == GdiVerticalFont &&
                font.GdiCharSet == GdiCharSet &&
                font.Style == Style &&
                font.Size == Size &&
                font.Unit == Unit;
        }

        /// <summary>
        /// Gets the hash code for this <see cref='Font'/>.
        /// </summary>
        public override int GetHashCode()
        {
            return unchecked((int)((((uint)_fontStyle << 13) | ((uint)_fontStyle >> 19)) ^
                         (((uint)_fontUnit << 26) | ((uint)_fontUnit >> 6)) ^
                         (((uint)_fontSize << 7) | ((uint)_fontSize >> 25))));
        }

        /// <summary>
        /// Returns a human-readable string representation of this <see cref='Font'/>.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[{0}: Name={1}, Size={2}, Units={3}, GdiCharSet={4}, GdiVerticalFont={5}]",
                                    GetType().Name,
                                    FontFamily.Name,
                                    _fontSize,
                                    (int)_fontUnit,
                                    _gdiCharSet,
                                    _gdiVerticalFont);
        }

        /// <summary>
        /// Cleans up Windows resources for this <see cref='Font'/>.
        /// </summary>
        ~Font() => Dispose(false);

        /// <summary>
        /// Cleans up Windows resources for this <see cref='Font'/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_nativeFont != IntPtr.Zero)
            {
                try
                {
#if DEBUG
                    int status =
#endif
                    SafeNativeMethods.Gdip.GdipDeleteFont(new HandleRef(this, _nativeFont));
#if DEBUG
                    Debug.Assert(status == SafeNativeMethods.Gdip.Ok, "GDI+ returned an error status: " + status.ToString(CultureInfo.InvariantCulture));
#endif
                }
                catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
                {
                }
                finally
                {
                    _nativeFont = IntPtr.Zero;
                }
            }
        }

        private static bool IsVerticalName(string familyName) => familyName?.Length > 0 && familyName[0] == '@';

        private static string StripVerticalName(string familyName)
        {
            if (familyName?.Length > 1 && familyName[0] == '@')
            {
                return familyName.Substring(1);
            }

            return familyName;
        }

        // This is used by SystemFonts when constructing a system Font objects.
        internal void SetSystemFontName(string systemFontName) => _systemFontName = systemFontName;

        private void SetFontFamily(FontFamily family)
        {
            _fontFamily = family;

            // GDI+ creates ref-counted singleton FontFamily objects based on the family name so all managed 
            // objects with same family name share the underlying GDI+ native pointer.  The unmanged object is
            // destroyed when its ref-count gets to zero.
            // Make sure this.fontFamily is not finalized so the underlying singleton object is kept alive.
            GC.SuppressFinalize(_fontFamily);
        }

        private float GetSizeInPoints(float dpi, float lineSpacingInPixels)
        {
            float pixelsPerPoint = (float)(dpi / 72.0);
            float emHeightInPixels = lineSpacingInPixels * FontFamily.GetEmHeight(Style) / FontFamily.GetLineSpacing(Style);

            return emHeightInPixels / pixelsPerPoint;
        }
    }
}
