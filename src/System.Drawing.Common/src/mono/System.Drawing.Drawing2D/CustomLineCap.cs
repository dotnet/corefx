// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.CustomLineCap.cs
//
// Authors:
//	Dennis Hayes (dennish@Raytek.com)
//	Andreas Nahr (ClassDevelopment@A-SoftTech.com)
//	Ravindra (rkumar@novell.com)
//
// Copyright (C) 2002/3 Ximian, Inc. http://www.ximian.com
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

namespace System.Drawing.Drawing2D
{

    public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
    {
        private bool disposed;
        internal IntPtr nativeObject;

        // Constructors

        internal CustomLineCap()
        {
        }

        internal CustomLineCap(IntPtr ptr)
        {
            nativeObject = ptr;
        }

        public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath) : this(fillPath, strokePath, LineCap.Flat, 0)
        {
        }

        public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap) : this(fillPath, strokePath, baseCap, 0)
        {
        }

        public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
        {
            IntPtr fill = IntPtr.Zero;
            IntPtr stroke = IntPtr.Zero;

            if (fillPath != null)
                fill = fillPath.nativePath;
            if (strokePath != null)
                stroke = strokePath.nativePath;

            Status status = GDIPlus.GdipCreateCustomLineCap(fill, stroke, baseCap, baseInset, out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        public LineCap BaseCap
        {
            get
            {
                LineCap baseCap;
                Status status = GDIPlus.GdipGetCustomLineCapBaseCap(nativeObject, out baseCap);
                GDIPlus.CheckStatus(status);

                return baseCap;
            }

            set
            {
                Status status = GDIPlus.GdipSetCustomLineCapBaseCap(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        public LineJoin StrokeJoin
        {
            get
            {
                LineJoin strokeJoin;
                Status status = GDIPlus.GdipGetCustomLineCapStrokeJoin(nativeObject, out strokeJoin);
                GDIPlus.CheckStatus(status);

                return strokeJoin;
            }

            set
            {
                Status status = GDIPlus.GdipSetCustomLineCapStrokeJoin(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        public float BaseInset
        {
            get
            {
                float baseInset;
                Status status = GDIPlus.GdipGetCustomLineCapBaseInset(nativeObject, out baseInset);
                GDIPlus.CheckStatus(status);

                return baseInset;
            }

            set
            {
                Status status = GDIPlus.GdipSetCustomLineCapBaseInset(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        public float WidthScale
        {
            get
            {
                float widthScale;
                Status status = GDIPlus.GdipGetCustomLineCapWidthScale(nativeObject, out widthScale);
                GDIPlus.CheckStatus(status);

                return widthScale;
            }

            set
            {
                Status status = GDIPlus.GdipSetCustomLineCapWidthScale(nativeObject, value);
                GDIPlus.CheckStatus(status);
            }
        }

        // Public Methods

        public object Clone()
        {
            IntPtr clonePtr;
            Status status = GDIPlus.GdipCloneCustomLineCap(nativeObject, out clonePtr);
            GDIPlus.CheckStatus(status);

            return new CustomLineCap(clonePtr);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Status status = GDIPlus.GdipDeleteCustomLineCap(nativeObject);
                GDIPlus.CheckStatus(status);
                disposed = true;
                nativeObject = IntPtr.Zero;
            }
        }

        ~CustomLineCap()
        {
            Dispose(false);
        }

        public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
        {
            Status status = GDIPlus.GdipGetCustomLineCapStrokeCaps(nativeObject, out startCap, out endCap);
            GDIPlus.CheckStatus(status);
        }

        public void SetStrokeCaps(LineCap startCap, LineCap endCap)
        {
            Status status = GDIPlus.GdipSetCustomLineCapStrokeCaps(nativeObject, startCap, endCap);
            GDIPlus.CheckStatus(status);
        }
    }
}
