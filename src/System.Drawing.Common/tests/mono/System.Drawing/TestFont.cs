// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Test Font class testing unit
//
// Authors:
// 	Jordi Mas i Hernandez, jordi@ximian.com
// 	Peter Dennis Bartok, pbartok@novell.com
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2003 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004-2007 Novell, Inc (http://www.novell.com)
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

using Xunit;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace MonoTests.System.Drawing
{

    public class FontTest
    {

        private string name;

        public FontTest()
        {
            using (FontFamily ff = new FontFamily(GenericFontFamilies.Monospace))
            {
                name = ff.Name;
            }
        }

        // Test basic Font clone, properties and contructor
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestClone()
        {
            Font f = new Font("Arial", 12);
            Font f2 = (Font)f.Clone();

            Assert.Equal(f.Bold, f2.Bold);
            Assert.Equal(f.FontFamily, f2.FontFamily);
            Assert.Equal(f.GdiCharSet, f2.GdiCharSet);
            Assert.Equal(f.GdiVerticalFont, f2.GdiVerticalFont);
            Assert.Equal(f.Height, f2.Height);
            Assert.Equal(f.Italic, f2.Italic);
            Assert.Equal(f.Name, f2.Name);
            Assert.Equal(f.Size, f2.Size);
            Assert.Equal(f.SizeInPoints, f2.SizeInPoints);
            Assert.Equal(f.Strikeout, f2.Strikeout);
            Assert.Equal(f.Style, f2.Style);
            Assert.Equal(f.Underline, f2.Underline);
            Assert.Equal(f.Unit, f2.Unit);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct LOGFONT_STRUCT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        [ConditionalFact(Helpers.GdiplusIsAvailableOnWindows)]
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public void ToLogFont_AssertUnmanagedCode()
        {
            Font f = new Font("Arial", 10);
            LOGFONT lf = new LOGFONT();

            f.ToLogFont(lf);
            Assert.Equal(400, lf.lfWeight);
            Assert.Equal(1, lf.lfCharSet);
            Assert.Equal(f.Name, lf.lfFaceName);

            LOGFONT_STRUCT lfs = new LOGFONT_STRUCT();
            f.ToLogFont(lfs);
            Assert.Equal(0, lfs.lfWeight);
            Assert.Equal(0, lfs.lfCharSet);
            Assert.Equal(0, lfs.lfHeight);
            Assert.Equal(0, lfs.lfWidth);
            Assert.Equal(0, lfs.lfEscapement);
            Assert.Equal(0, lfs.lfOrientation);
            Assert.Equal(0, lfs.lfWeight);
            Assert.Equal(0, lfs.lfItalic);
            Assert.Equal(0, lfs.lfUnderline);
            Assert.Equal(0, lfs.lfStrikeOut);
            Assert.Equal(0, lfs.lfCharSet);
            Assert.Equal(0, lfs.lfOutPrecision);
            Assert.Equal(0, lfs.lfClipPrecision);
            Assert.Equal(0, lfs.lfQuality);
            Assert.Equal(0, lfs.lfPitchAndFamily);
            Assert.Null(lfs.lfFaceName);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public void ToLogFont_TooSmall()
        {
            Font f = new Font("Arial", 10);
            object o = new object();
            Assert.Throws<ArgumentException>(() => f.ToLogFont(o));
            // no PInvoke conversion exists !?!?
        }

        // This test seems to cause the test host to crash.
        [ActiveIssue(20844)]
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public void ToLogFont_Int()
        {
            Font f = new Font("Arial", 10);
            int i = 1;
            f.ToLogFont(i);
            Assert.Equal(1, i);
        }

        [ActiveIssue(20844)]
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public void ToLogFont_Null()
        {
            Font f = new Font("Arial", 10);
            Assert.Throws<AccessViolationException>(() => f.ToLogFont(null));
        }
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Font_StringNull_Float()
        {
            string family = null;
            Font f = new Font(family, 12.5f);
            Assert.Equal(FontFamily.GenericSansSerif, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(12.5f, f.SizeInPoints);
            Assert.Equal(GraphicsUnit.Point, f.Unit);
        }

        [ActiveIssue(20844)]
        public void Font_String_Float()
        {
            Font f = new Font(name, 12.5f);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.False(f.Bold);
            Assert.Equal(1, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(12.5f, f.SizeInPoints);
            Assert.False(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Point, f.Unit);
        }

        [ActiveIssue(20844)]
        public void Font_String_Float_FontStyle()
        {
            Font f = new Font(name, 12.5f, FontStyle.Bold);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.True(f.Bold);
            Assert.Equal(1, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(12.5f, f.SizeInPoints);
            Assert.False(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Point, f.Unit);
        }

        [ActiveIssue(20844)]
        public void Font_String_Float_FontStyle_GraphicsUnit()
        {
            Font f = new Font(name, 12.5f, FontStyle.Italic, GraphicsUnit.Pixel);
            Assert.False(f.Bold);
            Assert.Equal(1, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.True(f.Italic);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.False(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Pixel, f.Unit);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Font_String_Float_FontStyle_GraphicsUnit_Display()
        {
            Assert.Throws<ArgumentException>(() => new Font(name, 12.5f, FontStyle.Italic, GraphicsUnit.Display));
        }

        [ActiveIssue(20844)]
        public void Font_String_Float_FontStyle_GraphicsUnit_Byte()
        {
            Font f = new Font(name, 12.5f, FontStyle.Strikeout, GraphicsUnit.Inch, Byte.MaxValue);
            Assert.False(f.Bold);
            Assert.Equal(Byte.MaxValue, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(900f, f.SizeInPoints);
            Assert.True(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Inch, f.Unit);
        }

        [ActiveIssue(20844)]
        public void Font_String_Float_FontStyle_GraphicsUnit_Byte_Bool()
        {
            Font f = new Font(name, 12.5f, FontStyle.Underline, GraphicsUnit.Document, Byte.MinValue, true);
            Assert.False(f.Bold);
            Assert.Equal(Byte.MinValue, f.GdiCharSet);
            Assert.True(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(3f, f.SizeInPoints);
            Assert.False(f.Strikeout);
            Assert.True(f.Underline);
            Assert.Equal(GraphicsUnit.Document, f.Unit);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Font_FontFamilyNull_Float()
        {
            FontFamily ff = null;
            Assert.Throws<ArgumentNullException>(() => new Font(ff, 12.5f));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Font_FontNull_FontStyle()
        {
            Font f = null;
            Assert.Throws<NullReferenceException>(() => new Font(f, FontStyle.Bold));
        }

        [ConditionalFact(Helpers.GdiPlusIsAvailableNotRedhat73)]
        public void Font_FontFamily_Float()
        {
            Font f = new Font(FontFamily.GenericMonospace, 12.5f);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.False(f.Bold);
            Assert.Equal(1, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(12.5f, f.SizeInPoints);
            Assert.False(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Point, f.Unit);
        }

        [ConditionalFact(Helpers.GdiPlusIsAvailableNotRedhat73)]
        public void Font_FontFamily_Float_FontStyle()
        {
            Font f = new Font(FontFamily.GenericMonospace, 12.5f, FontStyle.Bold);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.True(f.Bold);
            Assert.Equal(1, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(12.5f, f.SizeInPoints);
            Assert.False(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Point, f.Unit);
        }

        [ConditionalFact(Helpers.GdiPlusIsAvailableNotRedhat73)]
        public void Font_FontFamily_Float_FontStyle_GraphicsUnit()
        {
            Font f = new Font(FontFamily.GenericMonospace, 12.5f, FontStyle.Italic, GraphicsUnit.Millimeter);
            Assert.False(f.Bold);
            Assert.Equal(1, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.True(f.Italic);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(35.43307f, f.SizeInPoints, 4);
            Assert.False(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Millimeter, f.Unit);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Font_FontFamily_Float_FontStyle_GraphicsUnit_Display()
        {
            Assert.Throws<ArgumentException>(() => new Font(FontFamily.GenericMonospace, 12.5f, FontStyle.Italic, GraphicsUnit.Display));
        }

        [ConditionalFact(Helpers.GdiPlusIsAvailableNotRedhat73)]
        public void Font_FontFamily_Float_FontStyle_GraphicsUnit_Byte()
        {
            Font f = new Font(FontFamily.GenericMonospace, 12.5f, FontStyle.Strikeout, GraphicsUnit.Inch, Byte.MaxValue);
            Assert.False(f.Bold);
            Assert.Equal(Byte.MaxValue, f.GdiCharSet);
            Assert.False(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(900f, f.SizeInPoints);
            Assert.True(f.Strikeout);
            Assert.False(f.Underline);
            Assert.Equal(GraphicsUnit.Inch, f.Unit);
        }

        [ConditionalFact(Helpers.GdiPlusIsAvailableNotRedhat73)]
        public void Font_FontFamily_Float_FontStyle_GraphicsUnit_Byte_Bool()
        {
            Font f = new Font(FontFamily.GenericMonospace, 12.5f, FontStyle.Underline, GraphicsUnit.Document, Byte.MinValue, true);
            Assert.False(f.Bold);
            Assert.Equal(Byte.MinValue, f.GdiCharSet);
            Assert.True(f.GdiVerticalFont);
            Assert.True(f.Height > 0);
            Assert.False(f.Italic);
            Assert.Equal(FontFamily.GenericMonospace, f.FontFamily);
            Assert.Equal(f.Name, f.FontFamily.Name);
            Assert.Equal(12.5f, f.Size);
            Assert.Equal(3f, f.SizeInPoints, precision: 3);
            Assert.False(f.Strikeout);
            Assert.True(f.Underline);
            Assert.Equal(GraphicsUnit.Document, f.Unit);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_Double()
        {
            Font f = new Font(name, 12.5f);
            f.Dispose();
            f.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_UseAfter_Works()
        {
            Font f = new Font(name, 12.5f);
            string fname = f.Name;
            f.Dispose();
            // most properties don't throw, everything seems to be cached
            Assert.Equal(fname, f.Name);
            Assert.Equal(12.5f, f.Size);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_Height()
        {
            Font f = new Font(name, 12.5f);
            f.Dispose();
            Assert.Throws<ArgumentException>(() => { var x = f.Height; });
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_ToLogFont()
        {
            Font f = new Font(name, 12.5f);
            f.Dispose();
            LOGFONT lf = new LOGFONT();
            Assert.Throws<ArgumentException>(() => f.ToLogFont(lf));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailableOnWindows)]
        public void Dispose_ToLogFont_LoopCharSet()
        {
            Font f = new Font(name, 12.5f);
            f.Dispose();
            LOGFONT lf = new LOGFONT();

            for (int i = Byte.MinValue; i < Byte.MaxValue; i++)
            {
                byte b = (byte)i;
                lf.lfHeight = b;
                lf.lfWidth = b;
                lf.lfEscapement = b;
                lf.lfOrientation = b;
                lf.lfWeight = b;
                lf.lfItalic = b;
                lf.lfUnderline = b;
                lf.lfStrikeOut = b;
                lf.lfCharSet = b;
                lf.lfOutPrecision = b;
                lf.lfClipPrecision = b;
                lf.lfQuality = b;
                lf.lfPitchAndFamily = b;
                lf.lfFaceName = b.ToString();
                try
                {
                    f.ToLogFont(lf);
                }
                catch (ArgumentException)
                {
                    Assert.Equal(b, lf.lfHeight);
                    Assert.Equal(b, lf.lfWidth);
                    Assert.Equal(b, lf.lfEscapement);
                    Assert.Equal(b, lf.lfOrientation);
                    Assert.Equal(b, lf.lfWeight);
                    Assert.Equal(b, lf.lfItalic);
                    Assert.Equal(b, lf.lfUnderline);
                    Assert.Equal(b, lf.lfStrikeOut);
                    // special case for 0
                    Assert.Equal((i == 0) ? (byte)1 : b, lf.lfCharSet);
                    Assert.Equal(b, lf.lfOutPrecision);
                    Assert.Equal(b, lf.lfClipPrecision);
                    Assert.Equal(b, lf.lfQuality);
                    Assert.Equal(b, lf.lfPitchAndFamily);
                    Assert.Equal(b.ToString(), lf.lfFaceName);
                }
                catch (Exception e)
                {
                    Assert.True(false, string.Format("Unexcepted exception {0} at iteration {1}", e, i));
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_ToHFont()
        {
            Font f = new Font(name, 12.5f);
            f.Dispose();
            Assert.Throws<ArgumentException>(() => f.ToHfont());
        }

        [ConditionalFact(Helpers.GdiPlusIsAvailableNotRedhat73)]
        public void GetHeight_Float()
        {
            using (Font f = new Font(name, 12.5f))
            {
                Assert.Equal(0, f.GetHeight(0));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHeight_Graphics()
        {
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (Font f = new Font(name, 12.5f))
                    {
                        float expected = f.GetHeight(g.DpiY);
                        Assert.Equal(expected, f.GetHeight(g), 3);
                        g.ScaleTransform(2, 4);
                        Assert.Equal(expected, f.GetHeight(g), 3);
                        g.PageScale = 3;
                        Assert.Equal(expected, f.GetHeight(g), 3);
                    }
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHeight_Graphics_Null()
        {
            using (Font f = new Font(name, 12.5f))
            {
                Assert.Throws<ArgumentNullException>(() => f.GetHeight(null));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontUniqueHashCode()
        {
            Font f1 = new Font("Arial", 14);
            Font f2 = new Font("Arial", 12);
            Font f3 = new Font(f1, FontStyle.Bold);

            Assert.False(f1.GetHashCode() == f2.GetHashCode());
            Assert.False(f1.GetHashCode() == f3.GetHashCode());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHashCode_UnitDiffers_HashesNotEqual()
        {
            Font f1 = new Font("Arial", 8.25F, GraphicsUnit.Point);
            Font f2 = new Font("Arial", 8.25F, GraphicsUnit.Pixel);

            Assert.False(f1.GetHashCode() == f2.GetHashCode(),
                "Hashcodes should differ if _unit member differs");
        }

        [ActiveIssue(20844)]
        public void GetHashCode_NameDiffers_HashesNotEqual()
        {
            Font f1 = new Font("Arial", 8.25F, GraphicsUnit.Point);
            Font f2 = new Font("Courier New", 8.25F, GraphicsUnit.Point);

            if (f1.Name != f2.Name)
            {
                Assert.False(f1.GetHashCode() == f2.GetHashCode(),
                               "Hashcodes should differ if _name member differs");
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHashCode_StyleEqualsGdiCharSet_HashesNotEqual()
        {
            Font f1 = new Font("Arial", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            Font f2 = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(1)));

            Assert.False(f1.GetHashCode() == f2.GetHashCode(),
                "Hashcodes should differ if _style member differs");
        }
    }
}
