// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Fonts.cs
//
// Authors:
//    Alexandre Pigolkine (pigolkine@gmx.de)
//    Miguel de Icaza (miguel@ximian.com)
//    Todd Berman (tberman@sevenl.com)
//    Jordi Mas i Hernandez (jordi@ximian.com)
//    Ravindra (rkumar@novell.com)
//
// Copyright (C) 2004 Ximian, Inc. (http://www.ximian.com)
// Copyright (C) 2004, 2006 Novell, Inc (http://www.novell.com)
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

using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace System.Drawing
{
    [Serializable]
    public sealed partial class Font
    {
        private static int CharSetOffset = -1;

        private Font(SerializationInfo info, StreamingContext context)
        {
            string name = (string)info.GetValue("Name", typeof(string));
            float size = (float)info.GetValue("Size", typeof(float));
            FontStyle style = (FontStyle)info.GetValue("Style", typeof(FontStyle));
            GraphicsUnit unit = (GraphicsUnit)info.GetValue("Unit", typeof(GraphicsUnit));

            Initialize(name, size, style, unit, SafeNativeMethods.DEFAULT_CHARSET, IsVerticalName(name));
        }

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            si.AddValue("Name", Name);
            si.AddValue("Size", Size);
            si.AddValue("Style", Style);
            si.AddValue("Unit", Unit);
        }

        public static Font FromHfont(IntPtr hfont)
        {
            IntPtr newObject;
            FontStyle newStyle = FontStyle.Regular;
            var lf = new SafeNativeMethods.LOGFONT();

            // Sanity. Should we throw an exception?
            if (hfont == IntPtr.Zero)
            {
                return new Font("Arial", (float)10.0, FontStyle.Regular);
            }

            // If we're on Unix we use our private gdiplus API to avoid Wine 
            // dependencies in S.D
            int s = SafeNativeMethods.Gdip.GdipCreateFontFromHfont(new HandleRef(null, hfont), out newObject, ref lf);
            SafeNativeMethods.Gdip.CheckStatus(s);

            if (lf.lfItalic != 0)
            {
                newStyle |= FontStyle.Italic;
            }

            if (lf.lfUnderline != 0)
            {
                newStyle |= FontStyle.Underline;
            }

            if (lf.lfStrikeOut != 0)
            {
                newStyle |= FontStyle.Strikeout;
            }

            if (lf.lfWeight > 400)
            {
                newStyle |= FontStyle.Bold;
            }

            return new Font(newObject, lf.lfFaceName, newStyle, Math.Abs(lf.lfHeight));
        }

        public IntPtr ToHfont()
        {
            if (_nativeFont == IntPtr.Zero)
                throw new ArgumentException("Object has been disposed.");

            return _nativeFont;
        }

        internal Font(IntPtr newFontObject, string familyName, FontStyle style, float size)
        {
            _nativeFont = newFontObject;
            Initialize(familyName, size, style, GraphicsUnit.Pixel, 0, IsVerticalName(familyName));
        }

        internal Font(string familyName, float emSize, string systemName)
            : this(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, SafeNativeMethods.DEFAULT_CHARSET, false)
        {
            _systemFontName = systemName;
        }

        public object Clone() => new Font(this, Style);

        [Browsable(false)]
        public float SizeInPoints => GetSizeInPoints(Graphics.systemDpiY, GetHeight());

        [MonoTODO("The hdc parameter has no direct equivalent in libgdiplus.")]
        public static Font FromHdc(IntPtr hdc)
        {
            throw new NotImplementedException();
        }

        public float GetHeight() => GetHeight(Graphics.systemDpiY);

        public static Font FromLogFont(object lf) => FromLogFont(lf, IntPtr.Zero);

        public void ToLogFont(object logFont)
        {
            // Unix - We don't have a window we could associate the DC with
            // so we use an image instead
            using (Bitmap img = new Bitmap(1, 1, Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    ToLogFont(logFont, g);
                }
            }
        }

        public void ToLogFont(object logFont, Graphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if (logFont == null)
            {
                throw new AccessViolationException("logFont");
            }

            Type st = logFont.GetType();
            if (!st.GetTypeInfo().IsLayoutSequential)
                throw new ArgumentException("logFont", "Layout must be sequential.");

            // note: there is no exception if 'logFont' isn't big enough
            Type lf = typeof(LOGFONT);
            int size = Marshal.SizeOf(logFont);
            if (size >= Marshal.SizeOf(lf))
            {
                int status;
                IntPtr copy = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(logFont, copy, false);

                    status = SafeNativeMethods.Gdip.GdipGetLogFontW(new HandleRef(null, NativeFont), new HandleRef(null, graphics.NativeObject), logFont);
                    if (status != SafeNativeMethods.Gdip.Ok)
                    {
                        // reset to original values
                        Marshal.PtrToStructure(copy, logFont);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(copy);
                }

                if (CharSetOffset == -1)
                {
                    // not sure why this methods returns an IntPtr since it's an offset
                    // anyway there's no issue in downcasting the result into an int32
                    CharSetOffset = (int)Marshal.OffsetOf(lf, "lfCharSet");
                }

                // note: Marshal.WriteByte(object,*) methods are unimplemented on Mono
                GCHandle gch = GCHandle.Alloc(logFont, GCHandleType.Pinned);
                try
                {
                    IntPtr ptr = gch.AddrOfPinnedObject();
                    // if GDI+ lfCharSet is 0, then we return (S.D.) 1, otherwise the value is unchanged
                    if (Marshal.ReadByte(ptr, CharSetOffset) == 0)
                    {
                        // set lfCharSet to 1 
                        Marshal.WriteByte(ptr, CharSetOffset, 1);
                    }
                }
                finally
                {
                    gch.Free();
                }

                // now we can throw, if required
                SafeNativeMethods.Gdip.CheckStatus(status);
            }
        }
    }
}
