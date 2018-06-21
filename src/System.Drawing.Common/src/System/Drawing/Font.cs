// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Drawing
{
    /// <summary>
    /// Defines a particular format for text, including font face, size, and style attributes.
    /// </summary>
    public sealed partial class Font : MarshalByRefObject, ICloneable, IDisposable, ISerializable
    {
        private IntPtr _nativeFont;
        private float _fontSize;
        private FontStyle _fontStyle;
        private FontFamily _fontFamily;
        private GraphicsUnit _fontUnit;
        private byte _gdiCharSet = SafeNativeMethods.DEFAULT_CHARSET;
        private bool _gdiVerticalFont;
        private string _systemFontName = "";
        private string _originalFontName;

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
        /// Gets the <see cref='Drawing.FontFamily'/> of this <see cref='Font'/>.
        /// </summary>
        [Browsable(false)]
        public FontFamily FontFamily => _fontFamily;

        /// <summary>
        /// Gets the face name of this <see cref='Font'/> .
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !NETCORE
        [Editor ("System.Drawing.Design.FontNameEditor, " + Consts.AssemblySystem_Drawing_Design, typeof (System.Drawing.Design.UITypeEditor))]
        [TypeConverter (typeof (FontConverter.FontNameConverter))]
#endif
        public string Name => FontFamily.Name;

        /// <summary>
        /// Gets the unit of measure for this <see cref='Font'/>.
        /// </summary>
#if !NETCORE
        [TypeConverter (typeof (FontConverter.FontUnitConverter))]
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
        /// Gets the name of this <see cref='Drawing.SystemFont'/>.
        /// </summary>
        [Browsable(false)]
        public string SystemFontName => _systemFontName;

        /// <summary>
        /// Returns true if this <see cref='Font'/> is a SystemFont.
        /// </summary>
        [Browsable(false)]
        public bool IsSystemFont => !string.IsNullOrEmpty(_systemFontName);

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
    }
}
