// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.SolidBrush.cs
//
// Author:
//   Dennis Hayes (dennish@Raytek.com)
//   Alexandre Pigolkine(pigolkine@gmx.de)
//   Ravindra (rkumar@novell.com)
//
// (C) 2002 Ximian, Inc.
// Copyright (C) 2004, 2007 Novell, Inc (http://www.novell.com)
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

    public sealed class SolidBrush : Brush
    {

        internal bool isModifiable = true;
        // we keep this cached because calling GdipGetSolidFillColor 
        // wouldn't return a "named" color like SD is expected to do
        private Color color;

        internal SolidBrush(IntPtr ptr)
                : base(ptr)
        {
            int val;
            Status status = GDIPlus.GdipGetSolidFillColor(ptr, out val);
            GDIPlus.CheckStatus(status);
            color = Color.FromArgb(val);
        }

        public SolidBrush(Color color)
        {
            this.color = color;
            Status status = GDIPlus.GdipCreateSolidFill(color.ToArgb(), out nativeObject);
            GDIPlus.CheckStatus(status);
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (isModifiable)
                {
                    color = value;
                    Status status = GDIPlus.GdipSetSolidFillColor(nativeObject, value.ToArgb());
                    GDIPlus.CheckStatus(status);
                }
                else
                    throw new ArgumentException(Locale.GetText("This SolidBrush object can't be modified."));
            }
        }

        public override object Clone()
        {
            IntPtr clonePtr;
            Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonePtr);
            GDIPlus.CheckStatus(status);
            // we loose the named color in this case (but so does MS SD)
            return new SolidBrush(clonePtr);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !isModifiable)
                throw new ArgumentException(Locale.GetText("This SolidBrush object can't be modified."));

            base.Dispose(disposing);
        }
    }
}
