// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Tests for System.Drawing.SystemIconsTest.cs 
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

using Xunit;
using System;
using System.Drawing;
using System.Security.Permissions;

namespace MonoTests.System.Drawing
{

    public class SystemIconsTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Same()
        {
            // SystemIcons always return the same icon
            Assert.True(Object.ReferenceEquals(SystemIcons.Application, SystemIcons.Application));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_SystemIcons()
        {
            // SystemIcons icon's can't be disposed
            SystemIcons.Application.Dispose();
            Assert.NotNull(SystemIcons.Application.ToBitmap());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_Indirect()
        {
            // SystemIcons icon's can't be disposed
            Icon app = SystemIcons.Application;
            app.Dispose();
            Assert.NotNull(app.ToBitmap());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Clone_Dispose()
        {
            // Clones of SystemIcons icon's can be disposed
            Icon app = SystemIcons.Application;
            Icon clone = (Icon)app.Clone();
            clone.Dispose();
            Assert.Throws<ObjectDisposedException>(() => clone.ToBitmap());
        }
    }
}
