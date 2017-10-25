// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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
using System.Drawing.Imaging;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Imaging
{

    public class FrameDimensionTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Empty()
        {
            FrameDimension fd = new FrameDimension(Guid.Empty);
            Assert.Equal("00000000-0000-0000-0000-000000000000", fd.Guid.ToString());
            Assert.Equal(Guid.Empty.GetHashCode(), fd.GetHashCode());
            Assert.Equal("[FrameDimension: 00000000-0000-0000-0000-000000000000]", fd.ToString());

            Assert.True(fd.Equals(new FrameDimension(Guid.Empty)));
            Assert.False(fd.Equals(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WellKnownValues()
        {
            Assert.Equal("7462dc86-6180-4c7e-8e3f-ee7333a7a483", FrameDimension.Page.Guid.ToString());
            Assert.Equal("Page", FrameDimension.Page.ToString());
            Assert.True(Object.ReferenceEquals(FrameDimension.Page, FrameDimension.Page));

            Assert.Equal("84236f7b-3bd3-428f-8dab-4ea1439ca315", FrameDimension.Resolution.Guid.ToString());
            Assert.Equal("Resolution", FrameDimension.Resolution.ToString());
            Assert.True(Object.ReferenceEquals(FrameDimension.Resolution, FrameDimension.Resolution));

            Assert.Equal("6aedbd6d-3fb5-418a-83a6-7f45229dc872", FrameDimension.Time.Guid.ToString());
            Assert.Equal("Time", FrameDimension.Time.ToString());
            Assert.True(Object.ReferenceEquals(FrameDimension.Time, FrameDimension.Time));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Equals()
        {
            FrameDimension fd = new FrameDimension(new Guid("7462dc86-6180-4c7e-8e3f-ee7333a7a483"));
            // equals
            Assert.True(fd.Equals(FrameDimension.Page));
            // but ToString differs!
            Assert.Equal("[FrameDimension: 7462dc86-6180-4c7e-8e3f-ee7333a7a483]", fd.ToString());
        }
    }
}
