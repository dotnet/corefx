// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.ImageAttributes.cs
//
// Authors:
//	Dennis Hayes (dennish@raytek.com) (stubbed out)
//	Jordi Mas i Hernàndez (jmas@softcatala.org)
//	Sanjay Gupta (gsanjay@novell.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002-4 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004, 2006-2007 Novell, Inc (http://www.novell.com)
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

using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{

    [StructLayout(LayoutKind.Sequential)]
    public sealed class ImageAttributes : ICloneable, IDisposable
    {

        private IntPtr nativeImageAttr;

        internal IntPtr NativeObject
        {
            get
            {
                return nativeImageAttr;
            }
        }

        internal ImageAttributes(IntPtr native)
        {
            nativeImageAttr = native;
        }

        public ImageAttributes()
        {
            Status status = GDIPlus.GdipCreateImageAttributes(out nativeImageAttr);
            GDIPlus.CheckStatus(status);
        }

        public void ClearBrushRemapTable()
        {
            ClearRemapTable(ColorAdjustType.Brush);
        }

        //Clears the color keys for all GDI+ objects
        public void ClearColorKey()
        {
            ClearColorKey(ColorAdjustType.Default);
        }

        public void ClearColorKey(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesColorKeys(nativeImageAttr, type, false, 0, 0);
            GDIPlus.CheckStatus(status);
        }

        public void ClearColorMatrix()
        {
            ClearColorMatrix(ColorAdjustType.Default);
        }

        public void ClearColorMatrix(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, type, false,
                IntPtr.Zero, IntPtr.Zero, ColorMatrixFlag.Default);
            GDIPlus.CheckStatus(status);
        }

        public void ClearGamma()
        {
            ClearGamma(ColorAdjustType.Default);
        }

        public void ClearGamma(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesGamma(nativeImageAttr, type, false, 0);
            GDIPlus.CheckStatus(status);
        }

        public void ClearNoOp()
        {
            ClearNoOp(ColorAdjustType.Default);
        }

        public void ClearNoOp(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesNoOp(nativeImageAttr, type, false);
            GDIPlus.CheckStatus(status);
        }

        public void ClearOutputChannel()
        {
            ClearOutputChannel(ColorAdjustType.Default);
        }

        public void ClearOutputChannel(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesOutputChannel(nativeImageAttr, type, false,
                ColorChannelFlag.ColorChannelLast);
            GDIPlus.CheckStatus(status);
        }

        public void ClearOutputChannelColorProfile()
        {
            ClearOutputChannelColorProfile(ColorAdjustType.Default);
        }

        public void ClearOutputChannelColorProfile(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(nativeImageAttr, type, false,
                null);
            GDIPlus.CheckStatus(status);
        }

        public void ClearRemapTable()
        {
            ClearRemapTable(ColorAdjustType.Default);
        }

        public void ClearRemapTable(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesRemapTable(nativeImageAttr, type, false, 0, IntPtr.Zero);
            GDIPlus.CheckStatus(status);
        }

        public void ClearThreshold()
        {
            ClearThreshold(ColorAdjustType.Default);
        }

        public void ClearThreshold(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesThreshold(nativeImageAttr, type, false, 0);
            GDIPlus.CheckStatus(status);
        }

        //Sets the color keys for all GDI+ objects
        public void SetColorKey(Color colorLow, Color colorHigh)
        {
            SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);
        }

        public void SetColorMatrix(ColorMatrix newColorMatrix)
        {
            SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
        }

        public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag flags)
        {
            SetColorMatrix(newColorMatrix, flags, ColorAdjustType.Default);
        }

        public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag mode, ColorAdjustType type)
        {
            IntPtr cm = ColorMatrix.Alloc(newColorMatrix);
            try
            {
                Status status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr,
                    type, true, cm, IntPtr.Zero, mode);
                GDIPlus.CheckStatus(status);
            }
            finally
            {
                ColorMatrix.Free(cm);
            }
        }

        public void Dispose()
        {
            if (nativeImageAttr != IntPtr.Zero)
            {
                Status status = GDIPlus.GdipDisposeImageAttributes(nativeImageAttr);
                nativeImageAttr = IntPtr.Zero;
                GDIPlus.CheckStatus(status);
            }
            System.GC.SuppressFinalize(this);
        }

        ~ImageAttributes()
        {
            Dispose();
        }

        public object Clone()
        {
            IntPtr imgclone;

            Status status = GDIPlus.GdipCloneImageAttributes(nativeImageAttr, out imgclone);
            GDIPlus.CheckStatus(status);

            return new ImageAttributes(imgclone);
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
        {
            IntPtr colorPalette = palette.getGDIPalette();
            try
            {
                Status status = GDIPlus.GdipGetImageAttributesAdjustedPalette(nativeImageAttr, colorPalette, type);
                GDIPlus.CheckStatus(status);
                palette.setFromGDIPalette(colorPalette);
            }
            finally
            {
                if (colorPalette != IntPtr.Zero)
                    Marshal.FreeHGlobal(colorPalette);
            }
        }

        public void SetBrushRemapTable(ColorMap[] map)
        {
            GdiColorMap gdiclr = new GdiColorMap();
            IntPtr clrmap, lpPointer;
            int mapsize = Marshal.SizeOf(gdiclr);
            int size = mapsize * map.Length;
            clrmap = lpPointer = Marshal.AllocHGlobal(size);
            try
            {
                for (int i = 0; i < map.Length; i++)
                {
                    gdiclr.from = map[i].OldColor.ToArgb();
                    gdiclr.to = map[i].NewColor.ToArgb();

                    Marshal.StructureToPtr(gdiclr, lpPointer, false);
                    lpPointer = (IntPtr)(lpPointer.ToInt64() + mapsize);
                }

                Status status = GDIPlus.GdipSetImageAttributesRemapTable(nativeImageAttr,
                    ColorAdjustType.Brush, true, (uint)map.Length, clrmap);
                GDIPlus.CheckStatus(status);
            }
            finally
            {
                Marshal.FreeHGlobal(clrmap);
            }
        }

        public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesColorKeys(nativeImageAttr, type, true,
                colorLow.ToArgb(), colorHigh.ToArgb());
            GDIPlus.CheckStatus(status);
        }

        public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix)
        {
            SetColorMatrices(newColorMatrix, grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
        }

        public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags)
        {
            SetColorMatrices(newColorMatrix, grayMatrix, flags, ColorAdjustType.Default);
        }

        public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag mode, ColorAdjustType type)
        {
            Status status;
            IntPtr cm = ColorMatrix.Alloc(newColorMatrix);
            try
            {
                if (grayMatrix == null)
                {
                    status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, type, true, cm,
                        IntPtr.Zero, mode);
                }
                else
                {
                    IntPtr gm = ColorMatrix.Alloc(grayMatrix);
                    try
                    {
                        status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, type, true,
                            cm, gm, mode);
                    }
                    finally
                    {
                        ColorMatrix.Free(gm);
                    }
                }
            }
            finally
            {
                ColorMatrix.Free(cm);
            }
            GDIPlus.CheckStatus(status);
        }

        public void SetGamma(float gamma)
        {
            SetGamma(gamma, ColorAdjustType.Default);
        }

        public void SetGamma(float gamma, ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesGamma(nativeImageAttr, type, true, gamma);
            GDIPlus.CheckStatus(status);
        }

        public void SetNoOp()
        {
            SetNoOp(ColorAdjustType.Default);
        }

        public void SetNoOp(ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesNoOp(nativeImageAttr, type, true);
            GDIPlus.CheckStatus(status);
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void SetOutputChannel(ColorChannelFlag flags)
        {
            SetOutputChannel(flags, ColorAdjustType.Default);
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesOutputChannel(nativeImageAttr, type, true, flags);
            GDIPlus.CheckStatus(status);
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void SetOutputChannelColorProfile(string colorProfileFilename)
        {
            SetOutputChannelColorProfile(colorProfileFilename, ColorAdjustType.Default);
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(nativeImageAttr,
                type, true, colorProfileFilename);
            GDIPlus.CheckStatus(status);
        }

        public void SetRemapTable(ColorMap[] map)
        {
            SetRemapTable(map, ColorAdjustType.Default);
        }

        public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
        {
            GdiColorMap gdiclr = new GdiColorMap();
            IntPtr clrmap, lpPointer;
            int mapsize = Marshal.SizeOf(gdiclr);
            int size = mapsize * map.Length;
            clrmap = lpPointer = Marshal.AllocHGlobal(size);
            try
            {
                for (int i = 0; i < map.Length; i++)
                {
                    gdiclr.from = map[i].OldColor.ToArgb();
                    gdiclr.to = map[i].NewColor.ToArgb();

                    Marshal.StructureToPtr(gdiclr, lpPointer, false);
                    lpPointer = (IntPtr)(lpPointer.ToInt64() + mapsize);
                }

                Status status = GDIPlus.GdipSetImageAttributesRemapTable(nativeImageAttr,
                    type, true, (uint)map.Length, clrmap);
                GDIPlus.CheckStatus(status);
            }
            finally
            {
                Marshal.FreeHGlobal(clrmap);
            }
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void SetThreshold(float threshold)
        {
            SetThreshold(threshold, ColorAdjustType.Default);
        }

        [MonoTODO("Not supported by libgdiplus")]
        public void SetThreshold(float threshold, ColorAdjustType type)
        {
            Status status = GDIPlus.GdipSetImageAttributesThreshold(nativeImageAttr, type, true, 0);
            GDIPlus.CheckStatus(status);
        }

        public void SetWrapMode(WrapMode mode)
        {
            SetWrapMode(mode, Color.Black);
        }

        public void SetWrapMode(WrapMode mode, Color color)
        {
            SetWrapMode(mode, color, false);
        }

        public void SetWrapMode(WrapMode mode, Color color, bool clamp)
        {
            Status status = GDIPlus.GdipSetImageAttributesWrapMode(nativeImageAttr, mode, color.ToArgb(), clamp);
            GDIPlus.CheckStatus(status);
        }
    }
}
