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
//	Jordi Mas i Hernandez (jordi@ximian.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Imaging
{

    public class ImageAttributesTest
    {

        static ColorMatrix global_color_matrix = new ColorMatrix(new float[][] {
            new float[]     {2, 0,  0,  0,  0}, //R
			new float[]     {0, 1,  0,  0,  0}, //G
			new float[]     {0, 0,  1,  0,  0}, //B
			new float[]     {0, 0,  0,  1,  0}, //A
			new float[]     {0.2f,  0,  0,  0,  0}, //Translation
		});

        static ColorMatrix global_gray_matrix = new ColorMatrix(new float[][] {
            new float[]     {1, 0,  0,  0,  0}, //R
			new float[]     {0, 2,  0,  0,  0}, //G
			new float[]     {0, 0,  3,  0,  0}, //B
			new float[]     {0, 0,  0,  1,  0}, //A
			new float[]     {0.5f,  0,  0,  0,  0}, //Translation
		});

        private static Color ProcessColorMatrix(Color color, ColorMatrix colorMatrix)
        {
            using (Bitmap bmp = new Bitmap(64, 64))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    ImageAttributes imageAttr = new ImageAttributes();
                    bmp.SetPixel(0, 0, color);
                    imageAttr.SetColorMatrix(colorMatrix);
                    gr.DrawImage(bmp, new Rectangle(0, 0, 64, 64), 0, 0, 64, 64, GraphicsUnit.Pixel, imageAttr);
                    return bmp.GetPixel(0, 0);
                }
            }
        }


        // Text Color Matrix processing
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ColorMatrix1()
        {
            Color clr_src, clr_rslt;

            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[]     {2, 0,  0,  0,  0}, //R
				new float[]     {0, 1,  0,  0,  0}, //G
				new float[]     {0, 0,  1,  0,  0}, //B
				new float[]     {0, 0,  0,  1,  0}, //A
				new float[]     {0.2f,  0,  0,  0,  0}, //Translation
			  });

            clr_src = Color.FromArgb(255, 100, 20, 50);
            clr_rslt = ProcessColorMatrix(clr_src, cm);

            Assert.Equal(Color.FromArgb(255, 251, 20, 50), clr_rslt);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ColorMatrix2()
        {
            Color clr_src, clr_rslt;

            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[]     {1, 0,  0,  0,  0}, //R
				new float[]     {0, 1,  0,  0,  0}, //G
				new float[]     {0, 0,  1.5f,   0,  0}, //B
				new float[]     {0, 0,  0.5f,   1,  0}, //A
				new float[]     {0, 0,  0,  0,  0}, //Translation
			  });

            clr_src = Color.FromArgb(255, 100, 40, 25);
            clr_rslt = ProcessColorMatrix(clr_src, cm);
            Assert.Equal(Color.FromArgb(255, 100, 40, 165), clr_rslt);
        }

        private void Bug80323(Color c)
        {
            string fileName = String.Format("80323-{0}.png", c.ToArgb().ToString("X"));

            // test case from bug #80323
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[]     {1, 0,  0,  0,  0}, //R
				new float[]     {0, 1,  0,  0,  0}, //G
				new float[]     {0, 0,  1,  0,  0}, //B
				new float[]     {0, 0,  0,  0.5f,   0}, //A
				new float[]     {0, 0,  0,  0,  1}, //Translation
			  });

            using (SolidBrush sb = new SolidBrush(c))
            {
                using (Bitmap bmp = new Bitmap(100, 100))
                {
                    using (Graphics gr = Graphics.FromImage(bmp))
                    {
                        gr.FillRectangle(Brushes.White, 0, 0, 100, 100);
                        gr.FillEllipse(sb, 0, 0, 100, 100);
                    }
                    using (Bitmap b = new Bitmap(200, 100))
                    {
                        using (Graphics g = Graphics.FromImage(b))
                        {
                            g.FillRectangle(Brushes.White, 0, 0, 200, 100);

                            ImageAttributes ia = new ImageAttributes();
                            ia.SetColorMatrix(cm);
                            g.DrawImage(bmp, new Rectangle(0, 0, 100, 100), 0, 0, 100, 100, GraphicsUnit.Pixel, null);
                            g.DrawImage(bmp, new Rectangle(100, 0, 100, 100), 0, 0, 100, 100, GraphicsUnit.Pixel, ia);
                        }
                        b.Save(fileName);
                        Assert.Equal(Color.FromArgb(255, 255, 155, 155), b.GetPixel(50, 50));
                        Assert.Equal(Color.FromArgb(255, 255, 205, 205), b.GetPixel(150, 50));
                    }
                }
            }

            File.Delete(fileName);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ColorMatrix_80323_UsingAlpha()
        {
            Bug80323(Color.FromArgb(100, 255, 0, 0));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ColorMatrix_80323_WithoutAlpha()
        {
            // this color is identical, once drawn over the bitmap, to Color.FromArgb (100, 255, 0, 0)
            Bug80323(Color.FromArgb(255, 255, 155, 155));
        }



        private static Color ProcessColorMatrices(Color color, ColorMatrix colorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags, ColorAdjustType type)
        {
            using (Bitmap bmp = new Bitmap(64, 64))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    ImageAttributes imageAttr = new ImageAttributes();
                    bmp.SetPixel(0, 0, color);
                    imageAttr.SetColorMatrices(colorMatrix, grayMatrix, flags, type);
                    gr.DrawImage(bmp, new Rectangle(0, 0, 64, 64), 0, 0, 64, 64, GraphicsUnit.Pixel, imageAttr);
                    return bmp.GetPixel(0, 0);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_Null()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(null));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_Default()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Brush);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Pen);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Text);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_Default_Any()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Any));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_Default_Count()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Count));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Any()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Any));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Bitmap()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Bitmap));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Brush()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Brush));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Count()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Count));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Default()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Default));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Pen()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Pen));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_AltGrays_Text()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Text));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_SkipGrays()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Bitmap);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Brush);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Default);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Pen);
                ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Text);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_SkipGrays_Any()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Any));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_SkipGrays_Count()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Count));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_InvalidFlag()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, (ColorMatrixFlag)Int32.MinValue));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrix_InvalidType()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrix(global_color_matrix, ColorMatrixFlag.Default, (ColorAdjustType)Int32.MinValue));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_Null_ColorMatrix()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrices(null, global_color_matrix));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_ColorMatrix_Null()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrices(global_color_matrix, null);
                ia.SetColorMatrices(global_color_matrix, null, ColorMatrixFlag.Default);
                ia.SetColorMatrices(global_color_matrix, null, ColorMatrixFlag.SkipGrays);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_ColorMatrix_Null_AltGrays()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrices(global_color_matrix, null, ColorMatrixFlag.AltGrays));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_ColorMatrix_ColorMatrix()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrices(global_color_matrix, global_color_matrix);
                ia.SetColorMatrices(global_color_matrix, global_color_matrix, ColorMatrixFlag.Default);
                ia.SetColorMatrices(global_color_matrix, global_color_matrix, ColorMatrixFlag.SkipGrays);
                ia.SetColorMatrices(global_color_matrix, global_color_matrix, ColorMatrixFlag.AltGrays);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_Gray()
        {
            Color c = ProcessColorMatrices(Color.Gray, global_color_matrix, global_gray_matrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
            Assert.Equal(0xFFFF8080, (uint)c.ToArgb());

            c = ProcessColorMatrices(Color.Gray, global_color_matrix, global_gray_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Default);
            Assert.Equal(0xFF808080, (uint)c.ToArgb());

            c = ProcessColorMatrices(Color.Gray, global_color_matrix, global_gray_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Default);
            Assert.Equal(0xFFFFFFFF, (uint)c.ToArgb());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_Color()
        {
            Color c = ProcessColorMatrices(Color.MidnightBlue, global_color_matrix, global_gray_matrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
            Assert.Equal(0xFF651970, (uint)c.ToArgb());

            c = ProcessColorMatrices(Color.MidnightBlue, global_color_matrix, global_gray_matrix, ColorMatrixFlag.SkipGrays, ColorAdjustType.Default);
            Assert.Equal(0xFF651970, (uint)c.ToArgb());

            c = ProcessColorMatrices(Color.MidnightBlue, global_color_matrix, global_gray_matrix, ColorMatrixFlag.AltGrays, ColorAdjustType.Default);
            Assert.Equal(0xFF651970, (uint)c.ToArgb());
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_InvalidFlags()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrices(global_color_matrix, global_color_matrix, (ColorMatrixFlag)Int32.MinValue));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetColorMatrices_InvalidType()
        {
            using (ImageAttributes ia = new ImageAttributes())
            {
                Assert.Throws<ArgumentException>(() => ia.SetColorMatrices(global_color_matrix, global_color_matrix, ColorMatrixFlag.Default, (ColorAdjustType)Int32.MinValue));
            }
        }

        private void Alpha(string prefix, int n, float a)
        {
            ColorMatrix cm = new ColorMatrix(new float[][] {
                new float[]     {1, 0,  0,  0,  0}, //R
				new float[]     {0, 1,  0,  0,  0}, //G
				new float[]     {0, 0,  1,  0,  0}, //B
				new float[]     {0, 0,  0,  a,  0}, //A
				new float[]     {0, 0,  0,  0,  1}, //Translation
			  });

            using (Bitmap bmp = new Bitmap(1, 4))
            {
                bmp.SetPixel(0, 0, Color.White);
                bmp.SetPixel(0, 1, Color.Red);
                bmp.SetPixel(0, 2, Color.Lime);
                bmp.SetPixel(0, 3, Color.Blue);
                using (Bitmap b = new Bitmap(1, 4))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        ImageAttributes ia = new ImageAttributes();
                        ia.SetColorMatrix(cm);
                        g.FillRectangle(Brushes.White, new Rectangle(0, 0, 1, 4));
                        g.DrawImage(bmp, new Rectangle(0, 0, 1, 4), 0, 0, 1, 4, GraphicsUnit.Pixel, ia);
                        Assert.Equal(Color.FromArgb(255, 255, 255, 255), b.GetPixel(0, 0));
                        int val = 255 - n;
                        Assert.Equal(Color.FromArgb(255, 255, val, val), b.GetPixel(0, 1));
                        Assert.Equal(Color.FromArgb(255, val, 255, val), b.GetPixel(0, 2));
                        Assert.Equal(Color.FromArgb(255, val, val, 255), b.GetPixel(0, 3));
                    }
                }
            }
        }

        [ActiveIssue(20844)]
        public void ColorMatrixAlpha()
        {
            for (int i = 0; i < 256; i++)
            {
                Alpha(i.ToString(), i, (float)i / 255);
                // generally color matrix are specified with values between [0..1]
                Alpha("small-" + i.ToString(), i, (float)i / 255);
                // but GDI+ also accept value > 1
                Alpha("big-" + i.ToString(), i, 256 - i);
            }
        }
    }
}
