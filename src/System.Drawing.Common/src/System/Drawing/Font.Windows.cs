// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Internal;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Drawing
{
    public sealed partial class Font : MarshalByRefObject, ICloneable, IDisposable, ISerializable
    {
        /// <summary>
        /// Creates a <see cref='System.Drawing.Font'/> from the specified Windows handle.
        /// </summary>
        public static Font FromHfont(IntPtr hfont)
        {
            var lf = new SafeNativeMethods.LOGFONT();
            SafeNativeMethods.GetObject(new HandleRef(null, hfont), lf);
            
            IntPtr screenDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
            try
            {
                return FromLogFont(lf, screenDC);
            }
            finally
            {
                UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, screenDC));
            }
        }

        public static Font FromLogFont(object lf)
        {
            IntPtr screenDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
            try
            {
                return FromLogFont(lf, screenDC);
            }
            finally
            {
                UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, screenDC));
            }
        }

        /// <summary>
        /// Creates a Font from the specified Windows handle to a device context.
        /// </summary>
        public static Font FromHdc(IntPtr hdc)
        {
            IntPtr font = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCreateFontFromDC(new HandleRef(null, hdc), ref font);

            // Special case this incredibly common error message to give more information
            if (status == SafeNativeMethods.Gdip.NotTrueTypeFont)
            {
                throw new ArgumentException(SR.Format(SR.GdiplusNotTrueTypeFont_NoName));
            }
            else if (status != SafeNativeMethods.Gdip.Ok)
            {
                throw SafeNativeMethods.Gdip.StatusException(status);
            }

            return new Font(font, 0, false);
        }

        /// <summary>
        /// Creates an exact copy of this <see cref='Font'/>.
        /// </summary>
        public object Clone()
        {
            IntPtr clonedFont = IntPtr.Zero;
            int status = SafeNativeMethods.Gdip.GdipCloneFont(new HandleRef(this, _nativeFont), out clonedFont);
            SafeNativeMethods.Gdip.CheckStatus(status);

            return new Font(clonedFont, _gdiCharSet, _gdiVerticalFont);
        }

        public void ToLogFont(object logFont)
        {
            IntPtr screenDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
            try
            {
                Graphics graphics = Graphics.FromHdcInternal(screenDC);
                try
                {
                    ToLogFont(logFont, graphics);
                }
                finally
                {
                    graphics.Dispose();
                }
            }
            finally
            {
                UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, screenDC));
            }
        }

        public unsafe void ToLogFont(object logFont, Graphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            int status = SafeNativeMethods.Gdip.GdipGetLogFontW(new HandleRef(this, NativeFont), new HandleRef(graphics, graphics.NativeGraphics), logFont);

            // Prefix the string with '@' this is a gdiVerticalFont.
#pragma warning disable 0618
            if (_gdiVerticalFont)
            {
                // Copy the Unicode contents of the name.
                for (int i = 60; i >= 0; i -= 2)
                {
                    Marshal.WriteInt16(logFont,
                                        LogFontNameOffset + i + 2,
                                        Marshal.ReadInt16(logFont, LogFontNameOffset + i));
                }

                // Prefix the name with an '@' sign.
                Marshal.WriteInt16(logFont, LogFontNameOffset, (short)'@');
            }
            if (Marshal.ReadByte(logFont, LogFontCharSetOffset) == 0)
            {
                Marshal.WriteByte(logFont, LogFontCharSetOffset, _gdiCharSet);
            }
#pragma warning restore 0618

            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Returns a handle to this <see cref='Font'/>.
        /// </summary>
        public IntPtr ToHfont()
        {
            var lf = new SafeNativeMethods.LOGFONT();
            ToLogFont(lf);

            IntPtr handle = IntUnsafeNativeMethods.IntCreateFontIndirect(lf);
            if (handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            return handle;
        }

        public float GetHeight()
        {
            IntPtr screenDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
            try
            {
                using (Graphics graphics = Graphics.FromHdcInternal(screenDC))
                {
                    return GetHeight(graphics);
                }
            }
            finally
            {
                UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, screenDC));
            }
        }

        /// <summary>
        /// Gets the size, in points, of this <see cref='Font'/>.
        /// </summary>
        [Browsable(false)]
        public float SizeInPoints
        {
            get
            {
                if (Unit == GraphicsUnit.Point)
                {
                    return Size;
                }

                IntPtr screenDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
                try
                {
                    using (Graphics graphics = Graphics.FromHdcInternal(screenDC))
                    {
                        return GetSizeInPoints(graphics.DpiY, GetHeight(graphics));
                    }
                }
                finally
                {
                    UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, screenDC));
                }
            }
        }
    }
}
