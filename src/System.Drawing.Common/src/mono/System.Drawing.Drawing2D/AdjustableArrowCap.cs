// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.AdjustableArrowCap.cs
//
// Authors:
//	Dennis Hayes (dennish@Raytek.com)
//	Ravindra (rkumar@novell.com)
//
// Copyright (C) 2002/3 Ximian, Inc. http://www.ximian.com
// Copyright (C) 2004 Novell, Inc. http://www.novell.com
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
    /// Summary description for AdjustableArrowCap.
    /// </summary>
    public sealed class AdjustableArrowCap : CustomLineCap
    {
        // Constructors

        internal AdjustableArrowCap(IntPtr ptr) : base(ptr)
        {
        }

        public AdjustableArrowCap(float width, float height) : this(width, height, true)
        {
        }

        public AdjustableArrowCap(float width, float height, bool isFilled)
        {
            Status status = GDIPlus.GdipCreateAdjustableArrowCap(height, width, isFilled, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        // Public Properities

        public bool Filled
        {
            get
            {
                bool isFilled;
                Status status = GDIPlus.GdipGetAdjustableArrowCapFillState(nativeObject, out isFilled);
                GDIPlus.CheckStatus(status);

                return isFilled;
            }

            set
            {
                Status status = GDIPlus.GdipSetAdjustableArrowCapFillState(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        public float Width
        {
            get
            {
                float width;
                Status status = GDIPlus.GdipGetAdjustableArrowCapWidth(nativeObject, out width);
                GDIPlus.CheckStatus(status);

                return width;
            }

            set
            {
                Status status = GDIPlus.GdipSetAdjustableArrowCapWidth(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        public float Height
        {
            get
            {
                float height;
                Status status = GDIPlus.GdipGetAdjustableArrowCapHeight(nativeObject, out height);
                GDIPlus.CheckStatus(status);

                return height;
            }

            set
            {
                Status status = GDIPlus.GdipSetAdjustableArrowCapHeight(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        public float MiddleInset
        {
            get
            {
                float middleInset;
                Status status = GDIPlus.GdipGetAdjustableArrowCapMiddleInset(nativeObject, out middleInset);
                GDIPlus.CheckStatus(status);

                return middleInset;
            }

            set
            {
                Status status = GDIPlus.GdipSetAdjustableArrowCapMiddleInset(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }
    }
}
