// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.TestHatchBrush.cs 
//
// Author:
//	Ravindra (rkumar@novell.com)
//
// Copyright (C) 2004,2006 Novell, Inc (http://www.novell.com)
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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Drawing2D
{
    public class HatchBrushTest
    {
        Graphics gr;
        Bitmap bmp;
        Font font;
        Color bgColor;  // background color
        Color fgColor;  // foreground color
        int currentTop; // the location for next drawing operation
        int spacing;    // space between two consecutive drawing operations
        int fontSize;   // text size
        int textStart;  // text starting location
        int lineStart;  // line starting location
        int length;     // length of the line
        int penWidth;   // width of the Pen used to draw lines


        public HatchBrushTest()
        {
            fontSize = 16;
            textStart = 10;
            lineStart = 200;
            length = 400;
            penWidth = 50;
            currentTop = 0;
            spacing = 50;

            bgColor = Color.Yellow;
            fgColor = Color.Red;
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestProperties()
        {
            HatchBrush hbr = new HatchBrush(HatchStyle.SolidDiamond, fgColor);

            Assert.Equal(hbr.HatchStyle, HatchStyle.SolidDiamond);
            Assert.Equal(hbr.ForegroundColor.ToArgb(), fgColor.ToArgb());
            Assert.Equal(hbr.BackgroundColor.ToArgb(), Color.Black.ToArgb());

            hbr = new HatchBrush(HatchStyle.Cross, fgColor, bgColor);

            Assert.Equal(hbr.HatchStyle, HatchStyle.Cross);
            Assert.Equal(hbr.ForegroundColor.ToArgb(), fgColor.ToArgb());
            Assert.Equal(hbr.BackgroundColor.ToArgb(), bgColor.ToArgb());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestClone()
        {
            HatchBrush hbr = new HatchBrush(HatchStyle.Cross, fgColor, bgColor);

            HatchBrush clone = (HatchBrush)hbr.Clone();

            Assert.Equal(hbr.HatchStyle, clone.HatchStyle);
            Assert.Equal(hbr.ForegroundColor, clone.ForegroundColor);
            Assert.Equal(hbr.BackgroundColor, clone.BackgroundColor);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDrawing()
        {
            // create a bitmap with big enough dimensions 
            // to accomodate all the tests
            bmp = new Bitmap(700, 6000); // width, height
            gr = Graphics.FromImage(bmp);
            try
            {
                font = new Font(new FontFamily("Arial"), fontSize);
            }
            catch (ArgumentException)
            {
                Assert.True(false, "Arial FontFamily couldn't be found");
            }

            // make the background white
            gr.Clear(Color.White);

            // draw figures using hatch brush constructed
            // using different constructors
            Constructors();

            // draw figures using different hatchstyles
            HatchStyles();

            // save the drawing
            string file = "TestHatchBrush" + getOutSufix() + ".png";
            bmp.Save(file, ImageFormat.Png);
            File.Delete(file);
        }

        private void Constructors()
        {
            int top = currentTop;
            SolidBrush br = new SolidBrush(Color.Black);

            top += spacing;

            gr.DrawString("Test Constructors", font, br, textStart, top);

            // #1
            top += spacing;
            gr.DrawString("Test #1 Horizontal, BackgroundColor=Black, ForegroundColor=White", font, br, textStart, top);

            top += spacing;
            Pen pen = new Pen(new HatchBrush(HatchStyle.Horizontal, Color.White), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #2
            top += spacing;
            gr.DrawString("Test #2 Vertical, BackgroundColor=Blue, ForegroundColor=Red", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Vertical, Color.Red, Color.Blue), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            currentTop = top;
        }

        private void HatchStyles()
        {
            int top = currentTop;
            HatchBrush hbr;
            Pen pen;
            SolidBrush br = new SolidBrush(Color.Black);

            top += spacing;

            gr.DrawString("Test HatchStyles", font, br, textStart, top);

            // #1
            top += spacing;
            gr.DrawString("Test #1 Horizontal", font, br, textStart, top);

            top += spacing;
            hbr = new HatchBrush(HatchStyle.Horizontal, fgColor, bgColor);
            pen = new Pen(hbr, penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #2
            top += spacing;
            gr.DrawString("Test #2 Min", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.Min, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #3
            top += spacing;
            gr.DrawString("Test #3 DarkHorizontal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DarkHorizontal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #4
            top += spacing;
            gr.DrawString("Test #4 LightHorizontal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.LightHorizontal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #5
            top += spacing;
            gr.DrawString("Test #5 NarrowHorizontal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.NarrowHorizontal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #6
            top += spacing;
            gr.DrawString("Test #6 Vertical", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.Vertical, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #7
            top += spacing;
            gr.DrawString("Test #7 DarkVertical", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DarkVertical, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #8
            top += spacing;
            gr.DrawString("Test #8 LightVertical", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.LightVertical, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #9
            top += spacing;
            gr.DrawString("Test #9 NarrowVertical", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.NarrowVertical, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #10
            top += spacing;
            gr.DrawString("Test #10 Cross", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.Cross, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #11
            top += spacing;
            gr.DrawString("Test #11 LargeGrid", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.LargeGrid, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #12
            top += spacing;
            gr.DrawString("Test #12 SmallGrid", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.SmallGrid, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #13
            top += spacing;
            gr.DrawString("Test #13 DottedGrid", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DottedGrid, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #14
            top += spacing;
            gr.DrawString("Test #14 DiagonalCross", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DiagonalCross, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #15
            top += spacing;
            gr.DrawString("Test #15 BackwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.BackwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #16
            top += spacing;
            gr.DrawString("Test #16 ForwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.ForwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #17
            top += spacing;
            gr.DrawString("Test #17 LightDownwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.LightDownwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #18
            top += spacing;
            gr.DrawString("Test #18 DarkDownwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DarkDownwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #19
            top += spacing;
            gr.DrawString("Test #19 WideDownwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.WideDownwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #20
            top += spacing;
            gr.DrawString("Test #20 LightUpwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.LightUpwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #21
            top += spacing;
            gr.DrawString("Test #21 DarkUpwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DarkUpwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #22
            top += spacing;
            gr.DrawString("Test #22 WideUpwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.WideUpwardDiagonal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #23
            top += spacing;
            gr.DrawString("Test #23 DashedHorizontal", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DashedHorizontal, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #24
            top += spacing;
            gr.DrawString("Test #24 DashedVertical", font, br, textStart, top);

            top += spacing;
            hbr = new HatchBrush(HatchStyle.DashedVertical, fgColor, bgColor);
            gr.FillRectangle(hbr, lineStart, top, length, penWidth);

            // #25
            top += spacing;
            gr.DrawString("Test #25 DashedDownwardDiagonal", font, br, textStart, top);

            top += spacing;
            hbr = new HatchBrush(HatchStyle.DashedDownwardDiagonal, fgColor, bgColor);
            gr.FillRectangle(hbr, lineStart, top, length, penWidth);

            // #26
            top += spacing;
            gr.DrawString("Test #26 DashedUpwardDiagonal", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.DashedUpwardDiagonal, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #27
            top += spacing;
            gr.DrawString("Test #27 05Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent05, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #28
            top += spacing;
            gr.DrawString("Test #28 10Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent10, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #29
            top += spacing;
            gr.DrawString("Test #29 20Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent20, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #30
            top += spacing;
            gr.DrawString("Test #30 25Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent25, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #31
            top += spacing;
            gr.DrawString("Test #31 30Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent30, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #32
            top += spacing;
            gr.DrawString("Test #32 40Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent40, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #33
            top += spacing;
            gr.DrawString("Test #33 50Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent50, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #34
            top += spacing;
            gr.DrawString("Test #34 60Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent60, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #35
            top += spacing;
            gr.DrawString("Test #35 70Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent70, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #36
            top += spacing;
            gr.DrawString("Test #36 75Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent75, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #37
            top += spacing;
            gr.DrawString("Test #37 80Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent80, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #38
            top += spacing;
            gr.DrawString("Test #38 90Percent", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Percent90, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #39
            top += spacing;
            gr.DrawString("Test #39 SmallConfetti", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.SmallConfetti, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #40
            top += spacing;
            gr.DrawString("Test #40 LargeConfetti", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.LargeConfetti, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #41
            top += spacing;
            gr.DrawString("Test #41 ZigZag", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.ZigZag, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #42
            top += spacing;
            gr.DrawString("Test #42 Wave", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Wave, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #43
            top += spacing;
            gr.DrawString("Test #43 HorizontalBrick", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.HorizontalBrick, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #44
            top += spacing;
            gr.DrawString("Test #44 DiagonalBrick", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.DiagonalBrick, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #45
            top += spacing;
            gr.DrawString("Test #45 Weave", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Weave, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #46
            top += spacing;
            gr.DrawString("Test #46 Plaid", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Plaid, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #47
            top += spacing;
            gr.DrawString("Test #47 Divot", font, br, textStart, top);

            top += spacing;
            pen = new Pen(new HatchBrush(HatchStyle.Divot, fgColor, bgColor), penWidth);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #48
            top += spacing;
            gr.DrawString("Test #48 SmallCheckerBoard", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.SmallCheckerBoard, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #49
            top += spacing;
            gr.DrawString("Test #49 LargeCheckerBoard", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.LargeCheckerBoard, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #50
            top += spacing;
            gr.DrawString("Test #50 OutlinedDiamond", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.OutlinedDiamond, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #51
            top += spacing;
            gr.DrawString("Test #51 SolidDiamond", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.SolidDiamond, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #52
            top += spacing;
            gr.DrawString("Test #52 DottedDiamond", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.DottedDiamond, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #53
            top += spacing;
            gr.DrawString("Test #53 Shingle", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.Shingle, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #54
            top += spacing;
            gr.DrawString("Test #54 Trellis", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.Trellis, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            // #55
            top += spacing;
            gr.DrawString("Test #55 Sphere", font, br, textStart, top);

            top += spacing;
            pen.Brush = new HatchBrush(HatchStyle.Sphere, fgColor, bgColor);
            gr.DrawLine(pen, lineStart, top, lineStart + length, top);

            currentTop = top;
        }

        internal string getOutSufix()
        {
            string s;

            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 128) || (p == 6))
                s = "-unix";
            else
                s = "-windows";

            if (Type.GetType("Mono.Runtime", false) == null)
                s += "-msnet";
            else
                s += "-mono";

            return s;
        }
    }
}
