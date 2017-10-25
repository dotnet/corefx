// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// ColorTranslator class testing unit
//
// Copyright (C) 2006-2007 Novell, Inc (http://www.novell.com)
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
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing
{

    public class ColorTranslatorTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_Null()
        {
            Assert.Equal(0, ColorTranslator.FromHtml(null).ToArgb());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_Empty()
        {
            Assert.Equal(0, ColorTranslator.FromHtml(String.Empty).ToArgb());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_KnownValues()
        {
            Assert.Equal(SystemColors.Control, ColorTranslator.FromHtml("buttonface"));
            Assert.Equal(SystemColors.ActiveCaptionText, ColorTranslator.FromHtml("CAPTIONTEXT"));
            Assert.Equal(SystemColors.ControlDarkDark, ColorTranslator.FromHtml("threedDARKshadow"));
            Assert.Equal(SystemColors.Desktop, ColorTranslator.FromHtml("background"));
            Assert.Equal(SystemColors.ControlText, ColorTranslator.FromHtml("ButtonText"));
            Assert.Equal(SystemColors.Info, ColorTranslator.FromHtml("infobackground"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_Int()
        {
            Assert.Equal(-1, ColorTranslator.FromHtml("-1").ToArgb());
            Assert.Equal(0, ColorTranslator.FromHtml("0").ToArgb());
            Assert.Equal(1, ColorTranslator.FromHtml("1").ToArgb());
        }

        [ActiveIssue(20844, TestPlatforms.Any)]
        public void FromHtml_PoundInt()
        {
            Assert.Equal(0, ColorTranslator.FromHtml("#0").ToArgb());
            Assert.Equal(1, ColorTranslator.FromHtml("#1").ToArgb());
            Assert.Equal(255, ColorTranslator.FromHtml("#FF").ToArgb());
            Assert.Equal(-15654349, ColorTranslator.FromHtml("#123").ToArgb());
            Assert.Equal(-1, ColorTranslator.FromHtml("#FFF").ToArgb());
            Assert.Equal(65535, ColorTranslator.FromHtml("#FFFF").ToArgb());
            Assert.Equal(-15584170, ColorTranslator.FromHtml("#123456").ToArgb());
            Assert.Equal(-1, ColorTranslator.FromHtml("#FFFFFF").ToArgb());
            Assert.Equal(305419896, ColorTranslator.FromHtml("#12345678").ToArgb());
            Assert.Equal(-1, ColorTranslator.FromHtml("#FFFFFFFF").ToArgb());

            Assert.Equal(Color.White, ColorTranslator.FromHtml("#FFFFFF"));
            Assert.Equal(Color.White, ColorTranslator.FromHtml("0xFFFFFF"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_PoundNegative()
        {
            Assert.Throws<Exception>(() => ColorTranslator.FromHtml("#-1"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_PoundTooLarge()
        {
            Assert.Throws<Exception>(() => ColorTranslator.FromHtml("#100000000"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml_Unknown()
        {
            Assert.Throws<Exception>(() => ColorTranslator.FromHtml("unknown-color-test"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHtml()
        {
            Color[] colors = new Color[] {
Color.Aqua, Color.Black, Color.Blue, Color.Fuchsia, Color.Gray,
Color.Green, Color.Lime, Color.Maroon, Color.Navy, Color.Olive,
Color.Purple, Color.Red, Color.Silver, Color.Teal, Color.White,
Color.Yellow,

SystemColors.ActiveBorder, SystemColors.ActiveCaption,
SystemColors.Control, 
//SystemColors.ControlLightLight,
SystemColors.ActiveCaptionText, SystemColors.GrayText,
//SystemColors.InactiveBorder, SystemColors.InactiveCaption,
SystemColors.InfoText, SystemColors.Menu,
SystemColors.ControlDarkDark, 
//SystemColors.ControlText, SystemColors.ControlDark,
SystemColors.Window,
SystemColors.AppWorkspace, SystemColors.Desktop,
//SystemColors.ControlDark,
SystemColors.ControlText,
SystemColors.Highlight, SystemColors.HighlightText,
//SystemColors.InactiveCaptionText,
SystemColors.Info,
SystemColors.MenuText, SystemColors.ScrollBar,
//SystemColors.ControlLight, SystemColors.ControlLightLight
			};
            string[] htmlColors = new string[] {
"Aqua", "Black", "Blue", "Fuchsia", "Gray", "Green",
"Lime", "Maroon", "Navy", "Olive", "Purple", "Red",
"Silver", "Teal", "White", "Yellow",

"activeborder", "activecaption", "buttonface",
//"buhighlight",
"captiontext", "graytext",
//"iborder", "Icaption", 
"infotext", "menu", "threeddarkshadow",
//"thrface", "Threedshadow",
"window", "appworkspace",
"background", 
//"bshadow",
"buttontext", "highlight",
"highlighttext",
//"icaptiontext",
"infobackground",
"menutext", "scrollbar", 
//"thhighlight", "thlightshadow"
			};

            for (int i = 0; i < colors.Length; i++)
                Assert.Equal(colors[i], ColorTranslator.FromHtml(htmlColors[i]));
        }

        [Fact] // 340917
        public void FromHtml_LightGrey()
        {
            Assert.Equal(Color.LightGray, ColorTranslator.FromHtml(ColorTranslator.ToHtml(Color.LightGray)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromOle()
        {
            Assert.Equal(Color.FromArgb(0x10, 0x20, 0x30), ColorTranslator.FromOle(0x302010));
            Assert.Equal(Color.FromArgb(0xbb, 0x20, 0x30), ColorTranslator.FromOle(unchecked((int)0xee3020bb)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromWin32()
        {
            Assert.Equal(Color.FromArgb(0x10, 0x20, 0x30), ColorTranslator.FromWin32(0x302010));
            Assert.Equal(Color.FromArgb(0xbb, 0x20, 0x30), ColorTranslator.FromWin32(unchecked((int)0xee3020bb)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToHtml()
        {
            string[] htmlColors = new string[] {
"activeborder", "activecaption", "captiontext", "appworkspace", "buttonface",
"buttonshadow", "threeddarkshadow", "buttonface", "buttonhighlight", "buttontext",
"background", "graytext", "highlight", "highlighttext", "highlight", "inactiveborder",
"inactivecaption", "inactivecaptiontext", "infobackground", "infotext", "menu",
"menutext", "scrollbar", "window", "windowframe", "windowtext",

"Transparent", "AliceBlue", "AntiqueWhite", "Aqua", "Aquamarine", "Azure", "Beige",
"Bisque", "Black", "BlanchedAlmond", "Blue", "BlueViolet", "Brown", "BurlyWood",
"CadetBlue", "Chartreuse", "Chocolate", "Coral", "CornflowerBlue", "Cornsilk",
"Crimson", "Cyan", "DarkBlue", "DarkCyan", "DarkGoldenrod", "DarkGray", "DarkGreen",
"DarkKhaki", "DarkMagenta", "DarkOliveGreen", "DarkOrange", "DarkOrchid", "DarkRed",
"DarkSalmon", "DarkSeaGreen", "DarkSlateBlue", "DarkSlateGray", "DarkTurquoise", "DarkViolet",
"DeepPink", "DeepSkyBlue", "DimGray", "DodgerBlue", "Firebrick", "FloralWhite", "ForestGreen",
"Fuchsia", "Gainsboro", "GhostWhite", "Gold", "Goldenrod", "Gray", "Green", "GreenYellow",
"Honeydew", "HotPink", "IndianRed", "Indigo", "Ivory", "Khaki", "Lavender", "LavenderBlush",
"LawnGreen", "LemonChiffon", "LightBlue", "LightCoral", "LightCyan", "LightGoldenrodYellow",
"LightGrey", "LightGreen", "LightPink", "LightSalmon", "LightSeaGreen", "LightSkyBlue",
"LightSlateGray", "LightSteelBlue", "LightYellow", "Lime", "LimeGreen", "Linen", "Magenta",
"Maroon", "MediumAquamarine", "MediumBlue", "MediumOrchid", "MediumPurple", "MediumSeaGreen",
"MediumSlateBlue", "MediumSpringGreen", "MediumTurquoise", "MediumVioletRed", "MidnightBlue",
"MintCream", "MistyRose", "Moccasin", "NavajoWhite", "Navy", "OldLace", "Olive", "OliveDrab",
"Orange", "OrangeRed", "Orchid", "PaleGoldenrod", "PaleGreen", "PaleTurquoise", "PaleVioletRed",
"PapayaWhip", "PeachPuff", "Peru", "Pink", "Plum", "PowderBlue", "Purple", "Red", "RosyBrown",
"RoyalBlue", "SaddleBrown", "Salmon", "SandyBrown", "SeaGreen", "SeaShell", "Sienna", "Silver",
"SkyBlue", "SlateBlue", "SlateGray", "Snow", "SpringGreen", "SteelBlue", "Tan", "Teal",
"Thistle", "Tomato", "Turquoise", "Violet", "Wheat", "White", "WhiteSmoke", "Yellow", "YellowGreen",
                                            };

            for (KnownColor i = KnownColor.ActiveBorder; i <= KnownColor.YellowGreen; i++)
                Assert.Equal(htmlColors[(int)i - 1], ColorTranslator.ToHtml(Color.FromKnownColor(i)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToOle()
        {
            Assert.Equal(0x302010, ColorTranslator.ToOle(Color.FromArgb(0x10, 0x20, 0x30)));
            Assert.Equal(unchecked((int)0x3020bb), ColorTranslator.ToOle(Color.FromArgb(0xee, 0xbb, 0x20, 0x30)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToWin32()
        {
            Assert.Equal(0x302010, ColorTranslator.ToWin32(Color.FromArgb(0x10, 0x20, 0x30)));
            Assert.Equal(unchecked((int)0x3020bb), ColorTranslator.ToWin32(Color.FromArgb(0xee, 0xbb, 0x20, 0x30)));
        }

    }
}

