// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
// Tests for System.Drawing.Pens.cs
//
// Author:
//     Ravindra (rkumar@novell.com)
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
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
using System.Drawing.Drawing2D;
using System.Security.Permissions;

namespace MonoTests.System.Drawing
{
    public class PensTest
    {
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestEquals()
        {
            Pen pen1 = Pens.Blue;
            Pen pen2 = Pens.Blue;

            Assert.Equal(true, pen1.Equals(pen2));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestAliceBlue()
        {
            Pen pen = Pens.AliceBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.AliceBlue);

            try
            {
                pen.Color = Color.AliceBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestAntiqueWhite()
        {
            Pen pen = Pens.AntiqueWhite;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.AntiqueWhite);

            try
            {
                pen.Color = Color.AntiqueWhite;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestAqua()
        {
            Pen pen = Pens.Aqua;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Aqua);

            try
            {
                pen.Color = Color.Aqua;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestAquamarine()
        {
            Pen pen = Pens.Aquamarine;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Aquamarine);

            try
            {
                pen.Color = Color.Aquamarine;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestAzure()
        {
            Pen pen = Pens.Azure;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Azure);

            try
            {
                pen.Color = Color.Azure;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBeige()
        {
            Pen pen = Pens.Beige;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Beige);

            try
            {
                pen.Color = Color.Beige;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBisque()
        {
            Pen pen = Pens.Bisque;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Bisque);

            try
            {
                pen.Color = Color.Bisque;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBlack()
        {
            Pen pen = Pens.Black;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Black);

            try
            {
                pen.Color = Color.Black;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBlanchedAlmond()
        {
            Pen pen = Pens.BlanchedAlmond;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.BlanchedAlmond);

            try
            {
                pen.Color = Color.BlanchedAlmond;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBlue()
        {
            Pen pen = Pens.Blue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Blue);

            try
            {
                pen.Color = Color.Blue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBlueViolet()
        {
            Pen pen = Pens.BlueViolet;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.BlueViolet);

            try
            {
                pen.Color = Color.BlueViolet;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBrown()
        {
            Pen pen = Pens.Brown;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Brown);

            try
            {
                pen.Color = Color.Brown;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestBurlyWood()
        {
            Pen pen = Pens.BurlyWood;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.BurlyWood);

            try
            {
                pen.Color = Color.BurlyWood;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCadetBlue()
        {
            Pen pen = Pens.CadetBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.CadetBlue);

            try
            {
                pen.Color = Color.CadetBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestChartreuse()
        {
            Pen pen = Pens.Chartreuse;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Chartreuse);

            try
            {
                pen.Color = Color.Chartreuse;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestChocolate()
        {
            Pen pen = Pens.Chocolate;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Chocolate);

            try
            {
                pen.Color = Color.Chocolate;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCoral()
        {
            Pen pen = Pens.Coral;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Coral);

            try
            {
                pen.Color = Color.Coral;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCornflowerBlue()
        {
            Pen pen = Pens.CornflowerBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.CornflowerBlue);

            try
            {
                pen.Color = Color.CornflowerBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCornsilk()
        {
            Pen pen = Pens.Cornsilk;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Cornsilk);

            try
            {
                pen.Color = Color.Cornsilk;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCrimson()
        {
            Pen pen = Pens.Crimson;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Crimson);

            try
            {
                pen.Color = Color.Crimson;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestCyan()
        {
            Pen pen = Pens.Cyan;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Cyan);

            try
            {
                pen.Color = Color.Cyan;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkBlue()
        {
            Pen pen = Pens.DarkBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkBlue);

            try
            {
                pen.Color = Color.DarkBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkCyan()
        {
            Pen pen = Pens.DarkCyan;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkCyan);

            try
            {
                pen.Color = Color.DarkCyan;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkGoldenrod()
        {
            Pen pen = Pens.DarkGoldenrod;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkGoldenrod);

            try
            {
                pen.Color = Color.DarkGoldenrod;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkGray()
        {
            Pen pen = Pens.DarkGray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkGray);

            try
            {
                pen.Color = Color.DarkGray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkGreen()
        {
            Pen pen = Pens.DarkGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkGreen);

            try
            {
                pen.Color = Color.DarkGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkKhaki()
        {
            Pen pen = Pens.DarkKhaki;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkKhaki);

            try
            {
                pen.Color = Color.DarkKhaki;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkMagenta()
        {
            Pen pen = Pens.DarkMagenta;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkMagenta);

            try
            {
                pen.Color = Color.DarkMagenta;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkOliveGreen()
        {
            Pen pen = Pens.DarkOliveGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkOliveGreen);

            try
            {
                pen.Color = Color.DarkOliveGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkOrange()
        {
            Pen pen = Pens.DarkOrange;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkOrange);

            try
            {
                pen.Color = Color.DarkOrange;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkOrchid()
        {
            Pen pen = Pens.DarkOrchid;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkOrchid);

            try
            {
                pen.Color = Color.DarkOrchid;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkRed()
        {
            Pen pen = Pens.DarkRed;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkRed);

            try
            {
                pen.Color = Color.DarkRed;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkSalmon()
        {
            Pen pen = Pens.DarkSalmon;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkSalmon);

            try
            {
                pen.Color = Color.DarkSalmon;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkSeaGreen()
        {
            Pen pen = Pens.DarkSeaGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkSeaGreen);

            try
            {
                pen.Color = Color.DarkSeaGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkSlateBlue()
        {
            Pen pen = Pens.DarkSlateBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkSlateBlue);

            try
            {
                pen.Color = Color.DarkSlateBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkSlateGray()
        {
            Pen pen = Pens.DarkSlateGray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkSlateGray);

            try
            {
                pen.Color = Color.DarkSlateGray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkTurquoise()
        {
            Pen pen = Pens.DarkTurquoise;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkTurquoise);

            try
            {
                pen.Color = Color.DarkTurquoise;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDarkViolet()
        {
            Pen pen = Pens.DarkViolet;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DarkViolet);

            try
            {
                pen.Color = Color.DarkViolet;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDeepPink()
        {
            Pen pen = Pens.DeepPink;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DeepPink);

            try
            {
                pen.Color = Color.DeepPink;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDeepSkyBlue()
        {
            Pen pen = Pens.DeepSkyBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DeepSkyBlue);

            try
            {
                pen.Color = Color.DeepSkyBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDimGray()
        {
            Pen pen = Pens.DimGray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DimGray);

            try
            {
                pen.Color = Color.DimGray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDodgerBlue()
        {
            Pen pen = Pens.DodgerBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.DodgerBlue);

            try
            {
                pen.Color = Color.DodgerBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestFirebrick()
        {
            Pen pen = Pens.Firebrick;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Firebrick);

            try
            {
                pen.Color = Color.Firebrick;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestFloralWhite()
        {
            Pen pen = Pens.FloralWhite;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.FloralWhite);

            try
            {
                pen.Color = Color.FloralWhite;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestForestGreen()
        {
            Pen pen = Pens.ForestGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.ForestGreen);

            try
            {
                pen.Color = Color.ForestGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestFuchsia()
        {
            Pen pen = Pens.Fuchsia;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Fuchsia);

            try
            {
                pen.Color = Color.Fuchsia;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGainsboro()
        {
            Pen pen = Pens.Gainsboro;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Gainsboro);

            try
            {
                pen.Color = Color.Gainsboro;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGhostWhite()
        {
            Pen pen = Pens.GhostWhite;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.GhostWhite);

            try
            {
                pen.Color = Color.GhostWhite;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGold()
        {
            Pen pen = Pens.Gold;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Gold);

            try
            {
                pen.Color = Color.Gold;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGoldenrod()
        {
            Pen pen = Pens.Goldenrod;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Goldenrod);

            try
            {
                pen.Color = Color.Goldenrod;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGray()
        {
            Pen pen = Pens.Gray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Gray);

            try
            {
                pen.Color = Color.Gray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGreen()
        {
            Pen pen = Pens.Green;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Green);

            try
            {
                pen.Color = Color.Green;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestGreenYellow()
        {
            Pen pen = Pens.GreenYellow;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.GreenYellow);

            try
            {
                pen.Color = Color.GreenYellow;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestHoneydew()
        {
            Pen pen = Pens.Honeydew;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Honeydew);

            try
            {
                pen.Color = Color.Honeydew;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestHotPink()
        {
            Pen pen = Pens.HotPink;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.HotPink);

            try
            {
                pen.Color = Color.HotPink;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestIndianRed()
        {
            Pen pen = Pens.IndianRed;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.IndianRed);

            try
            {
                pen.Color = Color.IndianRed;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestIndigo()
        {
            Pen pen = Pens.Indigo;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Indigo);

            try
            {
                pen.Color = Color.Indigo;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestIvory()
        {
            Pen pen = Pens.Ivory;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Ivory);

            try
            {
                pen.Color = Color.Ivory;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestKhaki()
        {
            Pen pen = Pens.Khaki;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Khaki);

            try
            {
                pen.Color = Color.Khaki;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLavender()
        {
            Pen pen = Pens.Lavender;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Lavender);

            try
            {
                pen.Color = Color.Lavender;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLavenderBlush()
        {
            Pen pen = Pens.LavenderBlush;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LavenderBlush);

            try
            {
                pen.Color = Color.LavenderBlush;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLawnGreen()
        {
            Pen pen = Pens.LawnGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LawnGreen);

            try
            {
                pen.Color = Color.LawnGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLemonChiffon()
        {
            Pen pen = Pens.LemonChiffon;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LemonChiffon);

            try
            {
                pen.Color = Color.LemonChiffon;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightBlue()
        {
            Pen pen = Pens.LightBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightBlue);

            try
            {
                pen.Color = Color.LightBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightCoral()
        {
            Pen pen = Pens.LightCoral;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightCoral);

            try
            {
                pen.Color = Color.LightCoral;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightCyan()
        {
            Pen pen = Pens.LightCyan;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightCyan);

            try
            {
                pen.Color = Color.LightCyan;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightGoldenrodYellow()
        {
            Pen pen = Pens.LightGoldenrodYellow;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightGoldenrodYellow);

            try
            {
                pen.Color = Color.LightGoldenrodYellow;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightGray()
        {
            Pen pen = Pens.LightGray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightGray);

            try
            {
                pen.Color = Color.LightGray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightGreen()
        {
            Pen pen = Pens.LightGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightGreen);

            try
            {
                pen.Color = Color.LightGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightPink()
        {
            Pen pen = Pens.LightPink;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightPink);

            try
            {
                pen.Color = Color.LightPink;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightSalmon()
        {
            Pen pen = Pens.LightSalmon;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightSalmon);

            try
            {
                pen.Color = Color.LightSalmon;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightSeaGreen()
        {
            Pen pen = Pens.LightSeaGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightSeaGreen);

            try
            {
                pen.Color = Color.LightSeaGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightSkyBlue()
        {
            Pen pen = Pens.LightSkyBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightSkyBlue);

            try
            {
                pen.Color = Color.LightSkyBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightSlateGray()
        {
            Pen pen = Pens.LightSlateGray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightSlateGray);

            try
            {
                pen.Color = Color.LightSlateGray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightSteelBlue()
        {
            Pen pen = Pens.LightSteelBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightSteelBlue);

            try
            {
                pen.Color = Color.LightSteelBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLightYellow()
        {
            Pen pen = Pens.LightYellow;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LightYellow);

            try
            {
                pen.Color = Color.LightYellow;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLime()
        {
            Pen pen = Pens.Lime;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Lime);

            try
            {
                pen.Color = Color.Lime;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLimeGreen()
        {
            Pen pen = Pens.LimeGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.LimeGreen);

            try
            {
                pen.Color = Color.LimeGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestLinen()
        {
            Pen pen = Pens.Linen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Linen);

            try
            {
                pen.Color = Color.Linen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMagenta()
        {
            Pen pen = Pens.Magenta;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Magenta);

            try
            {
                pen.Color = Color.Magenta;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMaroon()
        {
            Pen pen = Pens.Maroon;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Maroon);

            try
            {
                pen.Color = Color.Maroon;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumAquamarine()
        {
            Pen pen = Pens.MediumAquamarine;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumAquamarine);

            try
            {
                pen.Color = Color.MediumAquamarine;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumBlue()
        {
            Pen pen = Pens.MediumBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumBlue);

            try
            {
                pen.Color = Color.MediumBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumOrchid()
        {
            Pen pen = Pens.MediumOrchid;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumOrchid);

            try
            {
                pen.Color = Color.MediumOrchid;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumPurple()
        {
            Pen pen = Pens.MediumPurple;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumPurple);

            try
            {
                pen.Color = Color.MediumPurple;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumSeaGreen()
        {
            Pen pen = Pens.MediumSeaGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumSeaGreen);

            try
            {
                pen.Color = Color.MediumSeaGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumSlateBlue()
        {
            Pen pen = Pens.MediumSlateBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumSlateBlue);

            try
            {
                pen.Color = Color.MediumSlateBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumSpringGreen()
        {
            Pen pen = Pens.MediumSpringGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumSpringGreen);

            try
            {
                pen.Color = Color.MediumSpringGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumTurquoise()
        {
            Pen pen = Pens.MediumTurquoise;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumTurquoise);

            try
            {
                pen.Color = Color.MediumTurquoise;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMediumVioletRed()
        {
            Pen pen = Pens.MediumVioletRed;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MediumVioletRed);

            try
            {
                pen.Color = Color.MediumVioletRed;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMidnightBlue()
        {
            Pen pen = Pens.MidnightBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MidnightBlue);

            try
            {
                pen.Color = Color.MidnightBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMintCream()
        {
            Pen pen = Pens.MintCream;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MintCream);

            try
            {
                pen.Color = Color.MintCream;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMistyRose()
        {
            Pen pen = Pens.MistyRose;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.MistyRose);

            try
            {
                pen.Color = Color.MistyRose;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMoccasin()
        {
            Pen pen = Pens.Moccasin;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Moccasin);

            try
            {
                pen.Color = Color.Moccasin;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestNavajoWhite()
        {
            Pen pen = Pens.NavajoWhite;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.NavajoWhite);

            try
            {
                pen.Color = Color.NavajoWhite;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestNavy()
        {
            Pen pen = Pens.Navy;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Navy);

            try
            {
                pen.Color = Color.Navy;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestOldLace()
        {
            Pen pen = Pens.OldLace;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.OldLace);

            try
            {
                pen.Color = Color.OldLace;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestOlive()
        {
            Pen pen = Pens.Olive;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Olive);

            try
            {
                pen.Color = Color.Olive;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestOliveDrab()
        {
            Pen pen = Pens.OliveDrab;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.OliveDrab);

            try
            {
                pen.Color = Color.OliveDrab;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestOrange()
        {
            Pen pen = Pens.Orange;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Orange);

            try
            {
                pen.Color = Color.Orange;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestOrangeRed()
        {
            Pen pen = Pens.OrangeRed;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.OrangeRed);

            try
            {
                pen.Color = Color.OrangeRed;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestOrchid()
        {
            Pen pen = Pens.Orchid;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Orchid);

            try
            {
                pen.Color = Color.Orchid;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPaleGoldenrod()
        {
            Pen pen = Pens.PaleGoldenrod;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PaleGoldenrod);

            try
            {
                pen.Color = Color.PaleGoldenrod;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPaleGreen()
        {
            Pen pen = Pens.PaleGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PaleGreen);

            try
            {
                pen.Color = Color.PaleGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPaleTurquoise()
        {
            Pen pen = Pens.PaleTurquoise;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PaleTurquoise);

            try
            {
                pen.Color = Color.PaleTurquoise;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPaleVioletRed()
        {
            Pen pen = Pens.PaleVioletRed;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PaleVioletRed);

            try
            {
                pen.Color = Color.PaleVioletRed;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPapayaWhip()
        {
            Pen pen = Pens.PapayaWhip;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PapayaWhip);

            try
            {
                pen.Color = Color.PapayaWhip;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPeachPuff()
        {
            Pen pen = Pens.PeachPuff;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PeachPuff);

            try
            {
                pen.Color = Color.PeachPuff;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPeru()
        {
            Pen pen = Pens.Peru;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Peru);

            try
            {
                pen.Color = Color.Peru;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPink()
        {
            Pen pen = Pens.Pink;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Pink);

            try
            {
                pen.Color = Color.Pink;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPlum()
        {
            Pen pen = Pens.Plum;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Plum);

            try
            {
                pen.Color = Color.Plum;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPowderBlue()
        {
            Pen pen = Pens.PowderBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.PowderBlue);

            try
            {
                pen.Color = Color.PowderBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestPurple()
        {
            Pen pen = Pens.Purple;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Purple);

            try
            {
                pen.Color = Color.Purple;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestRed()
        {
            Pen pen = Pens.Red;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Red);

            try
            {
                pen.Color = Color.Red;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestRosyBrown()
        {
            Pen pen = Pens.RosyBrown;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.RosyBrown);

            try
            {
                pen.Color = Color.RosyBrown;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestRoyalBlue()
        {
            Pen pen = Pens.RoyalBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.RoyalBlue);

            try
            {
                pen.Color = Color.RoyalBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSaddleBrown()
        {
            Pen pen = Pens.SaddleBrown;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SaddleBrown);

            try
            {
                pen.Color = Color.SaddleBrown;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSalmon()
        {
            Pen pen = Pens.Salmon;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Salmon);

            try
            {
                pen.Color = Color.Salmon;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSandyBrown()
        {
            Pen pen = Pens.SandyBrown;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SandyBrown);

            try
            {
                pen.Color = Color.SandyBrown;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSeaGreen()
        {
            Pen pen = Pens.SeaGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SeaGreen);

            try
            {
                pen.Color = Color.SeaGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSeaShell()
        {
            Pen pen = Pens.SeaShell;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SeaShell);

            try
            {
                pen.Color = Color.SeaShell;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSienna()
        {
            Pen pen = Pens.Sienna;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Sienna);

            try
            {
                pen.Color = Color.Sienna;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSilver()
        {
            Pen pen = Pens.Silver;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Silver);

            try
            {
                pen.Color = Color.Silver;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSkyBlue()
        {
            Pen pen = Pens.SkyBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SkyBlue);

            try
            {
                pen.Color = Color.SkyBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSlateBlue()
        {
            Pen pen = Pens.SlateBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SlateBlue);

            try
            {
                pen.Color = Color.SlateBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSlateGray()
        {
            Pen pen = Pens.SlateGray;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SlateGray);

            try
            {
                pen.Color = Color.SlateGray;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSnow()
        {
            Pen pen = Pens.Snow;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Snow);

            try
            {
                pen.Color = Color.Snow;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSpringGreen()
        {
            Pen pen = Pens.SpringGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SpringGreen);

            try
            {
                pen.Color = Color.SpringGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestSteelBlue()
        {
            Pen pen = Pens.SteelBlue;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.SteelBlue);

            try
            {
                pen.Color = Color.SteelBlue;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestTan()
        {
            Pen pen = Pens.Tan;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Tan);

            try
            {
                pen.Color = Color.Tan;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestTeal()
        {
            Pen pen = Pens.Teal;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Teal);

            try
            {
                pen.Color = Color.Teal;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestThistle()
        {
            Pen pen = Pens.Thistle;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Thistle);

            try
            {
                pen.Color = Color.Thistle;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestTomato()
        {
            Pen pen = Pens.Tomato;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Tomato);

            try
            {
                pen.Color = Color.Tomato;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestTransparent()
        {
            Pen pen = Pens.Transparent;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Transparent);

            try
            {
                pen.Color = Color.Transparent;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestTurquoise()
        {
            Pen pen = Pens.Turquoise;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Turquoise);

            try
            {
                pen.Color = Color.Turquoise;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestViolet()
        {
            Pen pen = Pens.Violet;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Violet);

            try
            {
                pen.Color = Color.Violet;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestWheat()
        {
            Pen pen = Pens.Wheat;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Wheat);

            try
            {
                pen.Color = Color.Wheat;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestWhite()
        {
            Pen pen = Pens.White;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.White);

            try
            {
                pen.Color = Color.White;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestWhiteSmoke()
        {
            Pen pen = Pens.WhiteSmoke;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.WhiteSmoke);

            try
            {
                pen.Color = Color.WhiteSmoke;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestYellow()
        {
            Pen pen = Pens.Yellow;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.Yellow);

            try
            {
                pen.Color = Color.Yellow;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestYellowGreen()
        {
            Pen pen = Pens.YellowGreen;
            Assert.Equal(pen.PenType, PenType.SolidColor);
            Assert.Equal(pen.Color, Color.YellowGreen);

            try
            {
                pen.Color = Color.YellowGreen;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }
    }
}

// Following code was used to generate the test methods above
//
//Type type = typeof (Pens);
//PropertyInfo [] properties = type.GetProperties ();
//int count = 1;
//foreach (PropertyInfo property in properties) {
//	Console.WriteLine();
//	Console.WriteLine("\t\t[Fact]");
//	Console.WriteLine("\t\tpublic void Test" + property.Name + " ()");
//	Console.WriteLine("\t\t{");
//	Console.WriteLine("\t\t\tPen pen = Pens." + property.Name + ";");
//	Console.WriteLine("\t\t\tAssertEquals (\"P" + count + "#1\", pen.PenType, PenType.SolidColor);");
//	Console.WriteLine("\t\t\tAssertEquals (\"P" + count + "#2\", pen.Color, Color." + property.Name + ");\n");
//
//	Console.WriteLine("\t\t\ttry {");
//	Console.WriteLine("\t\t\t\tpen.Color = Color." + property.Name + ";");
//	Console.WriteLine("\t\t\t\tAssert.True(false,  (\"P" + count + "#3: must throw ArgumentException\");");
//	Console.WriteLine("\t\t\t} catch (ArgumentException) {");
//	Console.WriteLine("\t\t\t\tAssert.True (\"P" + count + "#3\", true);");
//	Console.WriteLine("\t\t\t}");
//	Console.WriteLine("\t\t}");
//	count++;
//}
