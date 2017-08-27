// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Tests for System.Drawing.Brushes.cs
//
// Authors:
//	Ravindra (rkumar@novell.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
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

using Xunit;
using System;
using System.Drawing;
using System.Security.Permissions;

namespace MonoTests.System.Drawing
{

    public class BrushesTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Equality()
        {
            Brush brush1 = Brushes.Blue;
            Brush brush2 = Brushes.Blue;
            Assert.True(brush1.Equals(brush2));
            Assert.True(Object.ReferenceEquals(brush1, brush2));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Properties()
        {
            Brush br;
            SolidBrush solid;

            br = Brushes.Transparent;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Transparent, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Transparent as SolidBrush).Color);
            solid.Color = Color.Transparent; // revert to correct color (for other unit tests)

            br = Brushes.AliceBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.AliceBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.AliceBlue as SolidBrush).Color);
            solid.Color = Color.AliceBlue; // revert to correct color (for other unit tests)

            br = Brushes.AntiqueWhite;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.AntiqueWhite, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.AntiqueWhite as SolidBrush).Color);
            solid.Color = Color.AntiqueWhite; // revert to correct color (for other unit tests)

            br = Brushes.Aqua;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Aqua, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Aqua as SolidBrush).Color);
            solid.Color = Color.Aqua; // revert to correct color (for other unit tests)

            br = Brushes.Aquamarine;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Aquamarine, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Aquamarine as SolidBrush).Color);
            solid.Color = Color.Aquamarine; // revert to correct color (for other unit tests)

            br = Brushes.Azure;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Azure, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Azure as SolidBrush).Color);
            solid.Color = Color.Azure; // revert to correct color (for other unit tests)

            br = Brushes.Beige;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Beige, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Beige as SolidBrush).Color);
            solid.Color = Color.Beige; // revert to correct color (for other unit tests)

            br = Brushes.Bisque;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Bisque, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Bisque as SolidBrush).Color);
            solid.Color = Color.Bisque; // revert to correct color (for other unit tests)

            br = Brushes.Black;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Black, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Black as SolidBrush).Color);
            solid.Color = Color.Black; // revert to correct color (for other unit tests)

            br = Brushes.BlanchedAlmond;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.BlanchedAlmond, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.BlanchedAlmond as SolidBrush).Color);
            solid.Color = Color.BlanchedAlmond; // revert to correct color (for other unit tests)

            br = Brushes.Blue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Blue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Blue as SolidBrush).Color);
            solid.Color = Color.Blue; // revert to correct color (for other unit tests)

            br = Brushes.BlueViolet;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.BlueViolet, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.BlueViolet as SolidBrush).Color);
            solid.Color = Color.BlueViolet; // revert to correct color (for other unit tests)

            br = Brushes.Brown;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Brown, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Brown as SolidBrush).Color);
            solid.Color = Color.Brown; // revert to correct color (for other unit tests)

            br = Brushes.BurlyWood;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.BurlyWood, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.BurlyWood as SolidBrush).Color);
            solid.Color = Color.BurlyWood; // revert to correct color (for other unit tests)

            br = Brushes.CadetBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.CadetBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.CadetBlue as SolidBrush).Color);
            solid.Color = Color.CadetBlue; // revert to correct color (for other unit tests)

            br = Brushes.Chartreuse;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Chartreuse, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Chartreuse as SolidBrush).Color);
            solid.Color = Color.Chartreuse; // revert to correct color (for other unit tests)

            br = Brushes.Chocolate;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Chocolate, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Chocolate as SolidBrush).Color);
            solid.Color = Color.Chocolate; // revert to correct color (for other unit tests)

            br = Brushes.Coral;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Coral, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Coral as SolidBrush).Color);
            solid.Color = Color.Coral; // revert to correct color (for other unit tests)

            br = Brushes.CornflowerBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.CornflowerBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.CornflowerBlue as SolidBrush).Color);
            solid.Color = Color.CornflowerBlue; // revert to correct color (for other unit tests)

            br = Brushes.Cornsilk;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Cornsilk, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Cornsilk as SolidBrush).Color);
            solid.Color = Color.Cornsilk; // revert to correct color (for other unit tests)

            br = Brushes.Crimson;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Crimson, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Crimson as SolidBrush).Color);
            solid.Color = Color.Crimson; // revert to correct color (for other unit tests)

            br = Brushes.Cyan;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Cyan, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Cyan as SolidBrush).Color);
            solid.Color = Color.Cyan; // revert to correct color (for other unit tests)

            br = Brushes.DarkBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkBlue as SolidBrush).Color);
            solid.Color = Color.DarkBlue; // revert to correct color (for other unit tests)

            br = Brushes.DarkCyan;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkCyan, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkCyan as SolidBrush).Color);
            solid.Color = Color.DarkCyan; // revert to correct color (for other unit tests)

            br = Brushes.DarkGoldenrod;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkGoldenrod, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkGoldenrod as SolidBrush).Color);
            solid.Color = Color.DarkGoldenrod; // revert to correct color (for other unit tests)

            br = Brushes.DarkGray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkGray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkGray as SolidBrush).Color);
            solid.Color = Color.DarkGray; // revert to correct color (for other unit tests)

            br = Brushes.DarkGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkGreen as SolidBrush).Color);
            solid.Color = Color.DarkGreen; // revert to correct color (for other unit tests)

            br = Brushes.DarkKhaki;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkKhaki, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkKhaki as SolidBrush).Color);
            solid.Color = Color.DarkKhaki; // revert to correct color (for other unit tests)

            br = Brushes.DarkMagenta;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkMagenta, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkMagenta as SolidBrush).Color);
            solid.Color = Color.DarkMagenta; // revert to correct color (for other unit tests)

            br = Brushes.DarkOliveGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkOliveGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkOliveGreen as SolidBrush).Color);
            solid.Color = Color.DarkOliveGreen; // revert to correct color (for other unit tests)

            br = Brushes.DarkOrange;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkOrange, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkOrange as SolidBrush).Color);
            solid.Color = Color.DarkOrange; // revert to correct color (for other unit tests)

            br = Brushes.DarkOrchid;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkOrchid, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkOrchid as SolidBrush).Color);
            solid.Color = Color.DarkOrchid; // revert to correct color (for other unit tests)

            br = Brushes.DarkRed;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkRed, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkRed as SolidBrush).Color);
            solid.Color = Color.DarkRed; // revert to correct color (for other unit tests)

            br = Brushes.DarkSalmon;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkSalmon, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkSalmon as SolidBrush).Color);
            solid.Color = Color.DarkSalmon; // revert to correct color (for other unit tests)

            br = Brushes.DarkSeaGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkSeaGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkSeaGreen as SolidBrush).Color);
            solid.Color = Color.DarkSeaGreen; // revert to correct color (for other unit tests)

            br = Brushes.DarkSlateBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkSlateBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkSlateBlue as SolidBrush).Color);
            solid.Color = Color.DarkSlateBlue; // revert to correct color (for other unit tests)

            br = Brushes.DarkSlateGray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkSlateGray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkSlateGray as SolidBrush).Color);
            solid.Color = Color.DarkSlateGray; // revert to correct color (for other unit tests)

            br = Brushes.DarkTurquoise;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkTurquoise, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkTurquoise as SolidBrush).Color);
            solid.Color = Color.DarkTurquoise; // revert to correct color (for other unit tests)

            br = Brushes.DarkViolet;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DarkViolet, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DarkViolet as SolidBrush).Color);
            solid.Color = Color.DarkViolet; // revert to correct color (for other unit tests)

            br = Brushes.DeepPink;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DeepPink, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DeepPink as SolidBrush).Color);
            solid.Color = Color.DeepPink; // revert to correct color (for other unit tests)

            br = Brushes.DeepSkyBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DeepSkyBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DeepSkyBlue as SolidBrush).Color);
            solid.Color = Color.DeepSkyBlue; // revert to correct color (for other unit tests)

            br = Brushes.DimGray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DimGray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DimGray as SolidBrush).Color);
            solid.Color = Color.DimGray; // revert to correct color (for other unit tests)

            br = Brushes.DodgerBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.DodgerBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.DodgerBlue as SolidBrush).Color);
            solid.Color = Color.DodgerBlue; // revert to correct color (for other unit tests)

            br = Brushes.Firebrick;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Firebrick, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Firebrick as SolidBrush).Color);
            solid.Color = Color.Firebrick; // revert to correct color (for other unit tests)

            br = Brushes.FloralWhite;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.FloralWhite, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.FloralWhite as SolidBrush).Color);
            solid.Color = Color.FloralWhite; // revert to correct color (for other unit tests)

            br = Brushes.ForestGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.ForestGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.ForestGreen as SolidBrush).Color);
            solid.Color = Color.ForestGreen; // revert to correct color (for other unit tests)

            br = Brushes.Fuchsia;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Fuchsia, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Fuchsia as SolidBrush).Color);
            solid.Color = Color.Fuchsia; // revert to correct color (for other unit tests)

            br = Brushes.Gainsboro;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Gainsboro, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Gainsboro as SolidBrush).Color);
            solid.Color = Color.Gainsboro; // revert to correct color (for other unit tests)

            br = Brushes.GhostWhite;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.GhostWhite, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.GhostWhite as SolidBrush).Color);
            solid.Color = Color.GhostWhite; // revert to correct color (for other unit tests)

            br = Brushes.Gold;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Gold, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Gold as SolidBrush).Color);
            solid.Color = Color.Gold; // revert to correct color (for other unit tests)

            br = Brushes.Goldenrod;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Goldenrod, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Goldenrod as SolidBrush).Color);
            solid.Color = Color.Goldenrod; // revert to correct color (for other unit tests)

            br = Brushes.Gray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Gray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Gray as SolidBrush).Color);
            solid.Color = Color.Gray; // revert to correct color (for other unit tests)

            br = Brushes.Green;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Green, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Green as SolidBrush).Color);
            solid.Color = Color.Green; // revert to correct color (for other unit tests)

            br = Brushes.GreenYellow;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.GreenYellow, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.GreenYellow as SolidBrush).Color);
            solid.Color = Color.GreenYellow; // revert to correct color (for other unit tests)

            br = Brushes.Honeydew;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Honeydew, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Honeydew as SolidBrush).Color);
            solid.Color = Color.Honeydew; // revert to correct color (for other unit tests)

            br = Brushes.HotPink;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.HotPink, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.HotPink as SolidBrush).Color);
            solid.Color = Color.HotPink; // revert to correct color (for other unit tests)

            br = Brushes.IndianRed;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.IndianRed, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.IndianRed as SolidBrush).Color);
            solid.Color = Color.IndianRed; // revert to correct color (for other unit tests)

            br = Brushes.Indigo;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Indigo, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Indigo as SolidBrush).Color);
            solid.Color = Color.Indigo; // revert to correct color (for other unit tests)

            br = Brushes.Ivory;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Ivory, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Ivory as SolidBrush).Color);
            solid.Color = Color.Ivory; // revert to correct color (for other unit tests)

            br = Brushes.Khaki;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Khaki, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Khaki as SolidBrush).Color);
            solid.Color = Color.Khaki; // revert to correct color (for other unit tests)

            br = Brushes.Lavender;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Lavender, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Lavender as SolidBrush).Color);
            solid.Color = Color.Lavender; // revert to correct color (for other unit tests)

            br = Brushes.LavenderBlush;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LavenderBlush, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LavenderBlush as SolidBrush).Color);
            solid.Color = Color.LavenderBlush; // revert to correct color (for other unit tests)

            br = Brushes.LawnGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LawnGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LawnGreen as SolidBrush).Color);
            solid.Color = Color.LawnGreen; // revert to correct color (for other unit tests)

            br = Brushes.LemonChiffon;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LemonChiffon, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LemonChiffon as SolidBrush).Color);
            solid.Color = Color.LemonChiffon; // revert to correct color (for other unit tests)

            br = Brushes.LightBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightBlue as SolidBrush).Color);
            solid.Color = Color.LightBlue; // revert to correct color (for other unit tests)

            br = Brushes.LightCoral;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightCoral, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightCoral as SolidBrush).Color);
            solid.Color = Color.LightCoral; // revert to correct color (for other unit tests)

            br = Brushes.LightCyan;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightCyan, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightCyan as SolidBrush).Color);
            solid.Color = Color.LightCyan; // revert to correct color (for other unit tests)

            br = Brushes.LightGoldenrodYellow;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightGoldenrodYellow, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightGoldenrodYellow as SolidBrush).Color);
            solid.Color = Color.LightGoldenrodYellow; // revert to correct color (for other unit tests)

            br = Brushes.LightGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightGreen as SolidBrush).Color);
            solid.Color = Color.LightGreen; // revert to correct color (for other unit tests)

            br = Brushes.LightGray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightGray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightGray as SolidBrush).Color);
            solid.Color = Color.LightGray; // revert to correct color (for other unit tests)

            br = Brushes.LightPink;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightPink, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightPink as SolidBrush).Color);
            solid.Color = Color.LightPink; // revert to correct color (for other unit tests)

            br = Brushes.LightSalmon;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightSalmon, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightSalmon as SolidBrush).Color);
            solid.Color = Color.LightSalmon; // revert to correct color (for other unit tests)

            br = Brushes.LightSeaGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightSeaGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightSeaGreen as SolidBrush).Color);
            solid.Color = Color.LightSeaGreen; // revert to correct color (for other unit tests)

            br = Brushes.LightSkyBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightSkyBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightSkyBlue as SolidBrush).Color);
            solid.Color = Color.LightSkyBlue; // revert to correct color (for other unit tests)

            br = Brushes.LightSlateGray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightSlateGray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightSlateGray as SolidBrush).Color);
            solid.Color = Color.LightSlateGray; // revert to correct color (for other unit tests)

            br = Brushes.LightSteelBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightSteelBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightSteelBlue as SolidBrush).Color);
            solid.Color = Color.LightSteelBlue; // revert to correct color (for other unit tests)

            br = Brushes.LightYellow;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LightYellow, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LightYellow as SolidBrush).Color);
            solid.Color = Color.LightYellow; // revert to correct color (for other unit tests)

            br = Brushes.Lime;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Lime, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Lime as SolidBrush).Color);
            solid.Color = Color.Lime; // revert to correct color (for other unit tests)

            br = Brushes.LimeGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.LimeGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.LimeGreen as SolidBrush).Color);
            solid.Color = Color.LimeGreen; // revert to correct color (for other unit tests)

            br = Brushes.Linen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Linen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Linen as SolidBrush).Color);
            solid.Color = Color.Linen; // revert to correct color (for other unit tests)

            br = Brushes.Magenta;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Magenta, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Magenta as SolidBrush).Color);
            solid.Color = Color.Magenta; // revert to correct color (for other unit tests)

            br = Brushes.Maroon;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Maroon, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Maroon as SolidBrush).Color);
            solid.Color = Color.Maroon; // revert to correct color (for other unit tests)

            br = Brushes.MediumAquamarine;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumAquamarine, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumAquamarine as SolidBrush).Color);
            solid.Color = Color.MediumAquamarine; // revert to correct color (for other unit tests)

            br = Brushes.MediumBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumBlue as SolidBrush).Color);
            solid.Color = Color.MediumBlue; // revert to correct color (for other unit tests)

            br = Brushes.MediumOrchid;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumOrchid, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumOrchid as SolidBrush).Color);
            solid.Color = Color.MediumOrchid; // revert to correct color (for other unit tests)

            br = Brushes.MediumPurple;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumPurple, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumPurple as SolidBrush).Color);
            solid.Color = Color.MediumPurple; // revert to correct color (for other unit tests)

            br = Brushes.MediumSeaGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumSeaGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumSeaGreen as SolidBrush).Color);
            solid.Color = Color.MediumSeaGreen; // revert to correct color (for other unit tests)

            br = Brushes.MediumSlateBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumSlateBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumSlateBlue as SolidBrush).Color);
            solid.Color = Color.MediumSlateBlue; // revert to correct color (for other unit tests)

            br = Brushes.MediumSpringGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumSpringGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumSpringGreen as SolidBrush).Color);
            solid.Color = Color.MediumSpringGreen; // revert to correct color (for other unit tests)

            br = Brushes.MediumTurquoise;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumTurquoise, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumTurquoise as SolidBrush).Color);
            solid.Color = Color.MediumTurquoise; // revert to correct color (for other unit tests)

            br = Brushes.MediumVioletRed;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MediumVioletRed, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MediumVioletRed as SolidBrush).Color);
            solid.Color = Color.MediumVioletRed; // revert to correct color (for other unit tests)

            br = Brushes.MidnightBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MidnightBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MidnightBlue as SolidBrush).Color);
            solid.Color = Color.MidnightBlue; // revert to correct color (for other unit tests)

            br = Brushes.MintCream;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MintCream, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MintCream as SolidBrush).Color);
            solid.Color = Color.MintCream; // revert to correct color (for other unit tests)

            br = Brushes.MistyRose;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.MistyRose, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.MistyRose as SolidBrush).Color);
            solid.Color = Color.MistyRose; // revert to correct color (for other unit tests)

            br = Brushes.Moccasin;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Moccasin, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Moccasin as SolidBrush).Color);
            solid.Color = Color.Moccasin; // revert to correct color (for other unit tests)

            br = Brushes.NavajoWhite;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.NavajoWhite, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.NavajoWhite as SolidBrush).Color);
            solid.Color = Color.NavajoWhite; // revert to correct color (for other unit tests)

            br = Brushes.Navy;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Navy, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Navy as SolidBrush).Color);
            solid.Color = Color.Navy; // revert to correct color (for other unit tests)

            br = Brushes.OldLace;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.OldLace, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.OldLace as SolidBrush).Color);
            solid.Color = Color.OldLace; // revert to correct color (for other unit tests)

            br = Brushes.Olive;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Olive, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Olive as SolidBrush).Color);
            solid.Color = Color.Olive; // revert to correct color (for other unit tests)

            br = Brushes.OliveDrab;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.OliveDrab, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.OliveDrab as SolidBrush).Color);
            solid.Color = Color.OliveDrab; // revert to correct color (for other unit tests)

            br = Brushes.Orange;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Orange, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Orange as SolidBrush).Color);
            solid.Color = Color.Orange; // revert to correct color (for other unit tests)

            br = Brushes.OrangeRed;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.OrangeRed, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.OrangeRed as SolidBrush).Color);
            solid.Color = Color.OrangeRed; // revert to correct color (for other unit tests)

            br = Brushes.Orchid;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Orchid, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Orchid as SolidBrush).Color);
            solid.Color = Color.Orchid; // revert to correct color (for other unit tests)

            br = Brushes.PaleGoldenrod;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PaleGoldenrod, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PaleGoldenrod as SolidBrush).Color);
            solid.Color = Color.PaleGoldenrod; // revert to correct color (for other unit tests)

            br = Brushes.PaleGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PaleGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PaleGreen as SolidBrush).Color);
            solid.Color = Color.PaleGreen; // revert to correct color (for other unit tests)

            br = Brushes.PaleTurquoise;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PaleTurquoise, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PaleTurquoise as SolidBrush).Color);
            solid.Color = Color.PaleTurquoise; // revert to correct color (for other unit tests)

            br = Brushes.PaleVioletRed;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PaleVioletRed, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PaleVioletRed as SolidBrush).Color);
            solid.Color = Color.PaleVioletRed; // revert to correct color (for other unit tests)

            br = Brushes.PapayaWhip;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PapayaWhip, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PapayaWhip as SolidBrush).Color);
            solid.Color = Color.PapayaWhip; // revert to correct color (for other unit tests)

            br = Brushes.PeachPuff;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PeachPuff, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PeachPuff as SolidBrush).Color);
            solid.Color = Color.PeachPuff; // revert to correct color (for other unit tests)

            br = Brushes.Peru;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Peru, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Peru as SolidBrush).Color);
            solid.Color = Color.Peru; // revert to correct color (for other unit tests)

            br = Brushes.Pink;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Pink, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Pink as SolidBrush).Color);
            solid.Color = Color.Pink; // revert to correct color (for other unit tests)

            br = Brushes.Plum;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Plum, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Plum as SolidBrush).Color);
            solid.Color = Color.Plum; // revert to correct color (for other unit tests)

            br = Brushes.PowderBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.PowderBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.PowderBlue as SolidBrush).Color);
            solid.Color = Color.PowderBlue; // revert to correct color (for other unit tests)

            br = Brushes.Purple;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Purple, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Purple as SolidBrush).Color);
            solid.Color = Color.Purple; // revert to correct color (for other unit tests)

            br = Brushes.Red;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Red, solid.Color);
            solid.Color = Color.White;
            Assert.Equal(Color.White, solid.Color);
            Assert.Equal(Color.White, (Brushes.Red as SolidBrush).Color);
            solid.Color = Color.Red; // revert to correct color (for other unit tests)

            br = Brushes.RosyBrown;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.RosyBrown, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.RosyBrown as SolidBrush).Color);
            solid.Color = Color.RosyBrown; // revert to correct color (for other unit tests)

            br = Brushes.RoyalBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.RoyalBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.RoyalBlue as SolidBrush).Color);
            solid.Color = Color.RoyalBlue; // revert to correct color (for other unit tests)

            br = Brushes.SaddleBrown;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SaddleBrown, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SaddleBrown as SolidBrush).Color);
            solid.Color = Color.SaddleBrown; // revert to correct color (for other unit tests)

            br = Brushes.Salmon;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Salmon, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Salmon as SolidBrush).Color);
            solid.Color = Color.Salmon; // revert to correct color (for other unit tests)

            br = Brushes.SandyBrown;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SandyBrown, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SandyBrown as SolidBrush).Color);
            solid.Color = Color.SandyBrown; // revert to correct color (for other unit tests)

            br = Brushes.SeaGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SeaGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SeaGreen as SolidBrush).Color);
            solid.Color = Color.SeaGreen; // revert to correct color (for other unit tests)

            br = Brushes.SeaShell;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SeaShell, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SeaShell as SolidBrush).Color);
            solid.Color = Color.SeaShell; // revert to correct color (for other unit tests)

            br = Brushes.Sienna;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Sienna, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Sienna as SolidBrush).Color);
            solid.Color = Color.Sienna; // revert to correct color (for other unit tests)

            br = Brushes.Silver;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Silver, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Silver as SolidBrush).Color);
            solid.Color = Color.Silver; // revert to correct color (for other unit tests)

            br = Brushes.SkyBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SkyBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SkyBlue as SolidBrush).Color);
            solid.Color = Color.SkyBlue; // revert to correct color (for other unit tests)

            br = Brushes.SlateBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SlateBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SlateBlue as SolidBrush).Color);
            solid.Color = Color.SlateBlue; // revert to correct color (for other unit tests)

            br = Brushes.SlateGray;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SlateGray, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SlateGray as SolidBrush).Color);
            solid.Color = Color.SlateGray; // revert to correct color (for other unit tests)

            br = Brushes.Snow;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Snow, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Snow as SolidBrush).Color);
            solid.Color = Color.Snow; // revert to correct color (for other unit tests)

            br = Brushes.SpringGreen;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SpringGreen, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SpringGreen as SolidBrush).Color);
            solid.Color = Color.SpringGreen; // revert to correct color (for other unit tests)

            br = Brushes.SteelBlue;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.SteelBlue, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.SteelBlue as SolidBrush).Color);
            solid.Color = Color.SteelBlue; // revert to correct color (for other unit tests)

            br = Brushes.Tan;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Tan, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Tan as SolidBrush).Color);
            solid.Color = Color.Tan; // revert to correct color (for other unit tests)

            br = Brushes.Teal;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Teal, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Teal as SolidBrush).Color);
            solid.Color = Color.Teal; // revert to correct color (for other unit tests)

            br = Brushes.Thistle;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Thistle, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Thistle as SolidBrush).Color);
            solid.Color = Color.Thistle; // revert to correct color (for other unit tests)

            br = Brushes.Tomato;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Tomato, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Tomato as SolidBrush).Color);
            solid.Color = Color.Tomato; // revert to correct color (for other unit tests)

            br = Brushes.Turquoise;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Turquoise, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Turquoise as SolidBrush).Color);
            solid.Color = Color.Turquoise; // revert to correct color (for other unit tests)

            br = Brushes.Violet;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Violet, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Violet as SolidBrush).Color);
            solid.Color = Color.Violet; // revert to correct color (for other unit tests)

            br = Brushes.Wheat;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Wheat, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Wheat as SolidBrush).Color);
            solid.Color = Color.Wheat; // revert to correct color (for other unit tests)

            br = Brushes.White;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.White, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.White as SolidBrush).Color);
            solid.Color = Color.White; // revert to correct color (for other unit tests)

            br = Brushes.WhiteSmoke;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.WhiteSmoke, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.WhiteSmoke as SolidBrush).Color);
            solid.Color = Color.WhiteSmoke; // revert to correct color (for other unit tests)

            br = Brushes.Yellow;
            Assert.True((br is SolidBrush));
            solid = (SolidBrush)br;
            Assert.Equal(Color.Yellow, solid.Color);
            solid.Color = Color.Red;
            Assert.Equal(Color.Red, solid.Color);
            Assert.Equal(Color.Red, (Brushes.Yellow as SolidBrush).Color);
            solid.Color = Color.Yellow; // revert to correct color (for other unit tests)

            /* YellowGreen is broken by "destructive" Dispose test
			br = Brushes.YellowGreen;
			Assert.True ((br is SolidBrush));
			solid = (SolidBrush) br;
			Assert.Equal (Color.YellowGreen, solid.Color);
			solid.Color = Color.Red;
			Assert.Equal (Color.Red, solid.Color);
			Assert.Equal (Color.Red, (Brushes.YellowGreen as SolidBrush).Color);
			solid.Color = Color.YellowGreen; // revert to correct color (for other unit tests)
			*/
        }
    }
}

// Following code was used to generate the TestProperties method.
/*
using System;
using System.Drawing;
using System.Reflection;
class Program {
	static void Main ()
	{
		Type type = typeof (Brushes);
		PropertyInfo[] properties = type.GetProperties ();
		int count = 1;
		foreach (PropertyInfo property in properties) {
			Console.WriteLine("\n\t\t\tbr = Brushes." + property.Name + ";");
			Console.WriteLine("\t\t\tAssert.True ((br is SolidBrush), \"P" + count + "#1\");");
			Console.WriteLine("\t\t\tsolid = (SolidBrush) br;");
			Console.WriteLine("\t\t\tAssert.Equal (Color." + property.Name + ", solid.Color, \"P" + count + "#2\");");

			if (property.Name != "Red") {
				Console.WriteLine("\t\t\tsolid.Color = Color.Red;");
				Console.WriteLine("\t\t\tAssert.Equal (Color.Red, solid.Color, \"P" + count + "#3\");");
				Console.WriteLine("\t\t\tAssert.Equal (Color.Red, (Brushes." + property.Name + " as SolidBrush).Color, \"P" + count + "#4\");");
			} else {
				Console.WriteLine("\t\t\tsolid.Color = Color.White;");
				Console.WriteLine("\t\t\tAssert.Equal (Color.White, solid.Color, \"P" + count + "#3\");");
				Console.WriteLine("\t\t\tAssert.Equal (Color.White, (Brushes." + property.Name + " as SolidBrush).Color, \"P" + count + "#4\");");
			}
			Console.WriteLine("\t\t\tsolid.Color = Color." + property.Name + "; // revert to correct color (for other unit tests)");
			count++;
		}
	}
}
 */
