// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.FontFamily unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing
{

    public class FontFamilyTest
    {

        private Bitmap bitmap;
        private Graphics graphic;
        private string name;

        public FontFamilyTest()
        {
            bitmap = new Bitmap(10, 10);
            graphic = Graphics.FromImage(bitmap);
            using (FontFamily ff = new FontFamily(GenericFontFamilies.Monospace))
            {
                name = ff.Name;
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontFamily_String_Null()
        {
            Assert.Throws<ArgumentException>(() => new FontFamily(null));
        }

        private void CheckMono(FontFamily ff)
        {
            Assert.True(ff.Equals(FontFamily.GenericMonospace));
            // note: Mono has this behaviour on both 1.1 and 2.0 profiles
            Assert.Equal(ff.Name.GetHashCode(), ff.GetHashCode());
        }

        [ActiveIssue(20884)]
        public void FontFamily_String()
        {
            FontFamily ff = new FontFamily(name);
            CheckMono(ff);
            FontStyle style = FontStyle.Bold;
            Assert.Equal(ff.Name, ff.GetName(0));
            Assert.True((ff.GetCellAscent(style) > 0));
            Assert.True((ff.GetCellDescent(style) > 0));
            Assert.True((ff.GetEmHeight(style) > 0));
            Assert.True((ff.GetLineSpacing(style) > 0));
            Assert.True(ff.IsStyleAvailable(style));
        }

        [ActiveIssue(20884, TestPlatforms.Any)]
        public void FontFamily_String_FontCollection_Null()
        {
            FontFamily ff = new FontFamily(name, null);
            CheckMono(ff);
        }

        [ActiveIssue(20884, TestPlatforms.Any)]
        public void FontFamily_String_InstalledFontCollection()
        {
            FontFamily ff = new FontFamily(name, new InstalledFontCollection());
            CheckMono(ff);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontFamily_String_PrivateFontCollection()
        {
            Assert.Throws<ArgumentException>(() => new FontFamily(name, new PrivateFontCollection()));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontFamily_Monospace()
        {
            FontFamily ff = new FontFamily(GenericFontFamilies.Monospace);
            CheckMono(ff);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontFamily_SansSerif()
        {
            FontFamily ff = new FontFamily(GenericFontFamilies.SansSerif);
            Assert.True(ff.Equals(FontFamily.GenericSansSerif));
            // note: Mono has this behaviour on both 1.1 and 2.0 profiles
            Assert.Equal(ff.Name.GetHashCode(), ff.GetHashCode());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontFamily_Serif()
        {
            FontFamily ff = new FontFamily(GenericFontFamilies.Serif);
            Assert.True(ff.Equals(FontFamily.GenericSerif));
            // note: Mono has this behaviour on both 1.1 and 2.0 profiles
            Assert.Equal(ff.Name.GetHashCode(), ff.GetHashCode());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FontFamily_Invalid()
        {
            FontFamily ff = new FontFamily((GenericFontFamilies)Int32.MinValue);
            // default to Monospace
            Assert.True(ff.Equals(FontFamily.GenericMonospace));
            CheckMono(ff);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GenericMonospace()
        {
            FontFamily ff = FontFamily.GenericMonospace;
            string ts = ff.ToString();
            Assert.Equal('[', ts[0]);
            Assert.True((ts.IndexOf(name) >= 0));
            Assert.Equal(']', ts[ts.Length - 1]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GenericSansSerif()
        {
            FontFamily ff = FontFamily.GenericSansSerif;
            string name = ff.Name;
            ff.Dispose();
            Assert.Equal(name, FontFamily.GenericSansSerif.Name);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GenericSerif()
        {
            FontFamily ff = FontFamily.GenericSerif;
            string name = ff.Name;
            ff.Dispose();
            Assert.Equal(name, FontFamily.GenericSerif.Name);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetFamilies_Null()
        {
#pragma warning disable 618
            Assert.Throws<ArgumentNullException>(() => FontFamily.GetFamilies(null));
#pragma warning restore 618
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetFamilies()
        {
#pragma warning disable 618
            FontFamily[] ffc = FontFamily.GetFamilies(graphic);
            Assert.Equal(ffc.Length, FontFamily.Families.Length);
#pragma warning restore 618
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_Double()
        {
            FontFamily ff = FontFamily.GenericSerif;
            ff.Dispose();
            ff.Dispose();
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_UseAfter()
        {
            FontFamily ff = FontFamily.GenericMonospace;
            ff.Dispose();
            Assert.Throws<ArgumentException>(() => { var x = ff.Name; });
        }
    }
}
