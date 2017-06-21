// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Brush.cs
//
// Authors:
//   Miguel de Icaza (miguel@ximian.com)
//   Ravindra (rkumar@novell.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
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

namespace System.Drawing
{

    public abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
    {

        internal IntPtr nativeObject;

        abstract public object Clone();

        internal Brush(IntPtr ptr)
        {
            nativeObject = ptr;
        }

        internal IntPtr NativeObject
        {
            get
            {
                return nativeObject;
            }
            set
            {
                nativeObject = value;
            }
        }

        protected Brush()
        {
        }

        protected internal void SetNativeBrush(IntPtr brush)
        {
            nativeObject = brush;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // NOTE: this has been known to fail in the past (cairo)
            // but it's the only way to reclaim brush related memory
            if (nativeObject != IntPtr.Zero)
            {
                Status status = GDIPlus.GdipDeleteBrush(nativeObject);
                nativeObject = IntPtr.Zero;
                GDIPlus.CheckStatus(status);
            }
        }

        ~Brush()
        {
            Dispose(false);
        }
    }
}
