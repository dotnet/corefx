// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Copyright (C) 2005-2007 Novell, Inc (http://www.novell.com)
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
//
// Authors:
//   Jordi Mas i Hernandez (jordi@ximian.com)
//   Sebastien Pouliot  <sebastien@ximian.com>
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Imaging
{

    public class ColorMatrixTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_Null()
        {
            Assert.Throws<NullReferenceException>(() => new ColorMatrix(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_TooSmallArraySize()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new ColorMatrix(new float[][] { }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_TooWideArraySize()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new ColorMatrix(new float[][] {
                new float[] { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f }
            }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_TooTallArraySize()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new ColorMatrix(new float[][] {
                new float[] { 0.0f },
                new float[] { 1.0f },
                new float[] { 2.0f },
                new float[] { 3.0f },
                new float[] { 4.0f },
                new float[] { 5.0f }
            }));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Constructor_TooBigArraySize()
        {
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[] { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
                new float[] { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f },
                new float[] { 2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f },
                new float[] { 3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f },
                new float[] { 4.0f, 4.1f, 4.2f, 4.3f, 4.4f, 4.5f },
                new float[] { 5.0f, 5.1f, 5.2f, 5.3f, 5.4f, 5.5f }
            });

            Assert.Equal(0.0f, cm.Matrix00);
            Assert.Equal(0.1f, cm.Matrix01);
            Assert.Equal(0.2f, cm.Matrix02);
            Assert.Equal(0.3f, cm.Matrix03);
            Assert.Equal(0.4f, cm.Matrix04);
            Assert.Equal(1.0f, cm.Matrix10);
            Assert.Equal(1.1f, cm.Matrix11);
            Assert.Equal(1.2f, cm.Matrix12);
            Assert.Equal(1.3f, cm.Matrix13);
            Assert.Equal(1.4f, cm.Matrix14);
            Assert.Equal(2.0f, cm.Matrix20);
            Assert.Equal(2.1f, cm.Matrix21);
            Assert.Equal(2.2f, cm.Matrix22);
            Assert.Equal(2.3f, cm.Matrix23);
            Assert.Equal(2.4f, cm.Matrix24);
            Assert.Equal(3.0f, cm.Matrix30);
            Assert.Equal(3.1f, cm.Matrix31);
            Assert.Equal(3.2f, cm.Matrix32);
            Assert.Equal(3.3f, cm.Matrix33);
            Assert.Equal(3.4f, cm.Matrix34);
            Assert.Equal(4.0f, cm.Matrix40);
            Assert.Equal(4.1f, cm.Matrix41);
            Assert.Equal(4.2f, cm.Matrix42);
            Assert.Equal(4.3f, cm.Matrix43);
            Assert.Equal(4.4f, cm.Matrix44);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TooBigItems()
        {
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[] { 0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
                new float[] { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f },
                new float[] { 2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f },
                new float[] { 3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f },
                new float[] { 4.0f, 4.1f, 4.2f, 4.3f, 4.4f, 4.5f },
                new float[] { 5.0f, 5.1f, 5.2f, 5.3f, 5.4f, 5.5f }
            });
            Assert.Throws<IndexOutOfRangeException>(() => { var x = cm[5, 5]; });
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void DefaultConstructor()
        {
            ColorMatrix cm = new ColorMatrix();

            Assert.Equal(1, cm.Matrix00);
            Assert.Equal(1, cm.Matrix11);
            Assert.Equal(1, cm.Matrix22);
            Assert.Equal(1, cm.Matrix33);
            Assert.Equal(1, cm.Matrix44);
            Assert.Equal(0, cm.Matrix01);
            Assert.Equal(0, cm.Matrix02);
            Assert.Equal(0, cm.Matrix03);
            Assert.Equal(0, cm.Matrix04);
            Assert.Equal(0, cm.Matrix10);
            Assert.Equal(0, cm.Matrix12);
            Assert.Equal(0, cm.Matrix13);
            Assert.Equal(0, cm.Matrix14);
            Assert.Equal(0, cm.Matrix20);
            Assert.Equal(0, cm.Matrix21);
            Assert.Equal(0, cm.Matrix23);
            Assert.Equal(0, cm.Matrix24);
            Assert.Equal(0, cm.Matrix30);
            Assert.Equal(0, cm.Matrix31);
            Assert.Equal(0, cm.Matrix32);
            Assert.Equal(0, cm.Matrix34);
            Assert.Equal(0, cm.Matrix40);
            Assert.Equal(0, cm.Matrix41);
            Assert.Equal(0, cm.Matrix42);
            Assert.Equal(0, cm.Matrix43);
            Assert.Equal(100, Marshal.SizeOf(cm));
            Assert.Equal(100, Marshal.SizeOf(typeof(ColorMatrix)));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ConstructorArrayAndMethods()
        {
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[] {0.393f, 0.349f, 0.272f, 0, 0},
                new float[] {0.769f, 0.686f, 0.534f, 0, 0},
                new float[] {0.189f, 0.168f, 0.131f, 0, 0},
                new float[] {     0,      0,      0, 1, 0},
                new float[] {     0,      0,      0, 0, 1}
            });

            Assert.Equal(0.393f, cm.Matrix00);
            Assert.Equal(0.349f, cm.Matrix01);
            Assert.Equal(0.272f, cm.Matrix02);
            Assert.Equal(0, cm.Matrix03);
            Assert.Equal(0, cm.Matrix04);

            Assert.Equal(0.769f, cm.Matrix10);
            Assert.Equal(0.686f, cm.Matrix11);
            Assert.Equal(0.534f, cm.Matrix12);
            Assert.Equal(0, cm.Matrix13);
            Assert.Equal(0, cm.Matrix14);

            Assert.Equal(0.189f, cm.Matrix20);
            Assert.Equal(0.168f, cm.Matrix21);
            Assert.Equal(0.131f, cm.Matrix22);
            Assert.Equal(0, cm.Matrix23);
            Assert.Equal(0, cm.Matrix24);

            Assert.Equal(0, cm.Matrix30);
            Assert.Equal(0, cm.Matrix31);
            Assert.Equal(0, cm.Matrix32);
            Assert.Equal(1, cm.Matrix33);
            Assert.Equal(0, cm.Matrix34);

            Assert.Equal(0, cm.Matrix40);
            Assert.Equal(0, cm.Matrix41);
            Assert.Equal(0, cm.Matrix42);
            Assert.Equal(0, cm.Matrix43);
            Assert.Equal(1, cm.Matrix44);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IndexerProperty()
        {
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[]     {1, 0,  0,  0,  0},
                new float[]     {0.5f,  1,  0,  0,  0},
                new float[]     {0, 0.1f,   1.5f,   0,  0},
                new float[]     {0.5f,  3,  0.5f,   1,  0},
                new float[]     {0, 0,  0,  0,  0}
            });

            Assert.Equal(1, cm[0, 0]);
            Assert.Equal(0, cm[0, 1]);
            Assert.Equal(0, cm[0, 2]);
            Assert.Equal(0, cm[0, 3]);
            Assert.Equal(0, cm[0, 4]);

            Assert.Equal(0.5f, cm[1, 0]);
            Assert.Equal(1, cm[1, 1]);
            Assert.Equal(0, cm[1, 2]);
            Assert.Equal(0, cm[1, 3]);
            Assert.Equal(0, cm[1, 4]);

            Assert.Equal(0, cm[2, 0]);
            Assert.Equal(0.1f, cm[2, 1]);
            Assert.Equal(1.5f, cm[2, 2]);
            Assert.Equal(0, cm[2, 3]);
            Assert.Equal(0, cm[2, 4]);

            Assert.Equal(0.5f, cm[3, 0]);
            Assert.Equal(3, cm[3, 1]);
            Assert.Equal(0.5f, cm[3, 2]);
            Assert.Equal(1, cm[3, 3]);
            Assert.Equal(0, cm[3, 4]);

            Assert.Equal(0, cm[4, 0]);
            Assert.Equal(0, cm[4, 1]);
            Assert.Equal(0, cm[4, 2]);
            Assert.Equal(0, cm[4, 3]);
            Assert.Equal(0, cm[4, 4]);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IndividualProperties()
        {
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[]     {1, 0,  0,  0,  0},
                new float[]     {0.5f,  1,  0,  0,  0},
                new float[]     {0, 0.1f,   1.5f,   0,  0},
                new float[]     {0.5f,  3,  0.5f,   1,  0},
                new float[]     {0, 0,  0,  0,  0}
            });

            Assert.Equal(1, cm.Matrix00);
            Assert.Equal(0, cm.Matrix01);
            Assert.Equal(0, cm.Matrix02);
            Assert.Equal(0, cm.Matrix03);
            Assert.Equal(0, cm.Matrix04);

            Assert.Equal(0.5f, cm.Matrix10);
            Assert.Equal(1, cm.Matrix11);
            Assert.Equal(0, cm.Matrix12);
            Assert.Equal(0, cm.Matrix13);
            Assert.Equal(0, cm.Matrix14);

            Assert.Equal(0, cm.Matrix20);
            Assert.Equal(0.1f, cm.Matrix21);
            Assert.Equal(1.5f, cm.Matrix22);
            Assert.Equal(0, cm.Matrix23);
            Assert.Equal(0, cm.Matrix24);

            Assert.Equal(0.5f, cm.Matrix30);
            Assert.Equal(3, cm.Matrix31);
            Assert.Equal(0.5f, cm.Matrix32);
            Assert.Equal(1, cm.Matrix33);
            Assert.Equal(0, cm.Matrix34);

            Assert.Equal(0, cm.Matrix40);
            Assert.Equal(0, cm.Matrix41);
            Assert.Equal(0, cm.Matrix42);
            Assert.Equal(0, cm.Matrix43);
            Assert.Equal(0, cm.Matrix44);
        }
    }
}
