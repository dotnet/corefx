// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.HatchBrush.cs
//
// Authors:
//   Dennis Hayes (dennish@Raytek.com)
//   Ravindra (rkumar@novell.com)
//
// (C) 2002/3 Ximian, Inc
// (C) 2004  Novell, Inc.
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
using System;

namespace System.Drawing.Drawing2D
{
    /// <summary>
    /// Summary description for HatchBrush.
    /// </summary>
    public sealed class HatchBrush : Brush
    {

        internal HatchBrush(IntPtr ptr) : base(ptr)
        {
        }

        public HatchBrush(HatchStyle hatchstyle, Color foreColor)
                    : this(hatchstyle, foreColor, Color.Black)
        {
        }

        public HatchBrush(HatchStyle hatchstyle, Color foreColor, Color backColor)
        {
            Status status = GDIPlus.GdipCreateHatchBrush(hatchstyle, foreColor.ToArgb(), backColor.ToArgb(), out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        public Color BackgroundColor
        {
            get
            {
                int argb;
                Status status = GDIPlus.GdipGetHatchBackgroundColor(nativeObject, out argb);
                GDIPlus.CheckStatus(status);
                return Color.FromArgb(argb);
            }
        }

        public Color ForegroundColor
        {
            get
            {
                int argb;
                Status status = GDIPlus.GdipGetHatchForegroundColor(nativeObject, out argb);
                GDIPlus.CheckStatus(status);
                return Color.FromArgb(argb);
            }
        }

        public HatchStyle HatchStyle
        {
            get
            {
                HatchStyle hatchStyle;
                Status status = GDIPlus.GdipGetHatchStyle(nativeObject, out hatchStyle);
                GDIPlus.CheckStatus(status);
                return hatchStyle;
            }
        }

        public override object Clone()
        {
            IntPtr clonePtr;
            Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonePtr);
            GDIPlus.CheckStatus(status);

            HatchBrush clone = new HatchBrush(clonePtr);
            return clone;
        }

    }
}
