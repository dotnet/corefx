// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Tests for System.Drawing.Drawing2D.Blend.cs
//
// Author:
//   Ravindra (rkumar@novell.com)
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

namespace MonoTests.System.Drawing.Drawing2D
{
    public class BlendTest
    {
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestConstructors()
        {
            Blend blend0 = new Blend();

            Assert.Equal(1, blend0.Factors.Length);
            Assert.Equal(1, blend0.Positions.Length);

            Blend blend1 = new Blend(1);

            Assert.Equal(1, blend1.Factors.Length);
            Assert.Equal(1, blend1.Positions.Length);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestProperties()
        {
            Blend blend0 = new Blend();

            Assert.Equal(0, blend0.Factors[0]);
            Assert.Equal(0, blend0.Positions[0]);

            Blend blend1 = new Blend(1);
            float[] positions = { 0.0F, 0.5F, 1.0F };
            float[] factors = { 0.0F, 0.5F, 1.0F };
            blend1.Factors = factors;
            blend1.Positions = positions;

            Assert.Equal(factors[0], blend1.Factors[0]);
            Assert.Equal(factors[1], blend1.Factors[1]);
            Assert.Equal(factors[2], blend1.Factors[2]);
            Assert.Equal(positions[0], blend1.Positions[0]);
            Assert.Equal(positions[1], blend1.Positions[1]);
            Assert.Equal(positions[2], blend1.Positions[2]);
        }
    }
}
