// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Text.FontCollection.cs
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
// Author: Everaldo Canuto everaldo.canuto@bol.com.br
//		Sanjay Gupta (gsanjay@novell.com)
//		Peter Dennis Bartok (pbartok@novell.com)
//
//
// Copyright (C) 2004 - 2006 Novell, Inc (http://www.novell.com)
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

namespace System.Drawing.Text
{

    public abstract class FontCollection : IDisposable
    {

        internal IntPtr nativeFontCollection = IntPtr.Zero;

        internal FontCollection()
        {
        }

        // methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // DO NOT FREE FROM HERE
            // FIXME: InstalledFontCollection cannot be freed safely and will leak one time 
            // (inside libgdiplus). MS has a similar behaviour (but probably doesn't leak)
        }

        // properties
        public FontFamily[] Families
        {
            get
            {
                int found;
                int returned = 0;
                Status status;
                FontFamily[] families;
                IntPtr[] result;

                // MS doesn't throw ObjectDisposedException in this case
                if (nativeFontCollection == IntPtr.Zero)
                    throw new ArgumentException("Collection was disposed.");

                status = GDIPlus.GdipGetFontCollectionFamilyCount(nativeFontCollection, out found);
                GDIPlus.CheckStatus(status);
                if (found == 0)
                    return new FontFamily[0];

                result = new IntPtr[found];
                status = GDIPlus.GdipGetFontCollectionFamilyList(nativeFontCollection, found, result, out returned);

                families = new FontFamily[returned];
                for (int i = 0; i < returned; i++)
                {
                    families[i] = new FontFamily(result[i]);
                }

                return families;
            }
        }

        ~FontCollection()
        {
            Dispose(false);
        }
    }
}
