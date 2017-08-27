// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
// Tests for System.Drawing.SystemBrushes.cs
//
// Author: Ravindra (rkumar@novell.com)
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
using System.Security.Permissions;

namespace MonoTests.System.Drawing
{
    public class SystemBrushesTest
    {
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestActiveBorder()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ActiveBorder;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ActiveBorder, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }

            try
            {
                brush.Color = SystemColors.ActiveBorder;
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestActiveCaption()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ActiveCaption;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ActiveCaption, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ActiveCaption;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestActiveCaptionText()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ActiveCaptionText;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ActiveCaptionText, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ActiveCaptionText;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestAppWorkspace()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.AppWorkspace;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.AppWorkspace, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.AppWorkspace;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestControl()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.Control;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.Control, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Control;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestControlDark()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ControlDark;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ControlDark, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ControlDark;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestControlDarkDark()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ControlDarkDark;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ControlDarkDark, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ControlDarkDark;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestControlLight()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ControlLight;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ControlLight, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ControlLight;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestControlLightLight()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ControlLightLight;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ControlLightLight, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ControlLightLight;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestControlText()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ControlText;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ControlText, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ControlText;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }


        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestDesktop()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.Desktop;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.Desktop, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Desktop;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestHighlight()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.Highlight;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.Highlight, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Highlight;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestHighlightText()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.HighlightText;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.HighlightText, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.HighlightText;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestHotTrack()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.HotTrack;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.HotTrack, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.HotTrack;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestInactiveBorder()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.InactiveBorder;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.InactiveBorder, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.InactiveBorder;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestInactiveCaption()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.InactiveCaption;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.InactiveCaption, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.InactiveCaption;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestInfo()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.Info;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.Info, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Info;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestMenu()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.Menu;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.Menu, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Menu;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestScrollBar()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.ScrollBar;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.ScrollBar, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.ScrollBar;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestWindow()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.Window;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.Window, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Window;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestWindowText()
        {
            SolidBrush brush;
            brush = (SolidBrush)SystemBrushes.WindowText;
            Assert.True(brush.Color.IsSystemColor);
            Assert.Equal(SystemColors.WindowText, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.WindowText;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestFromSystemColor()
        {
            SolidBrush brush;

            brush = (SolidBrush)SystemBrushes.FromSystemColor(SystemColors.Menu);
            Assert.Equal(SystemColors.Menu, brush.Color);

            try
            {
                brush.Color = Color.Red;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Color = SystemColors.Menu;
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }

            try
            {
                brush.Dispose();
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }


            try
            {
                brush = (SolidBrush)SystemBrushes.FromSystemColor(Color.Red);
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is ArgumentException);
            }
        }
    }
}
