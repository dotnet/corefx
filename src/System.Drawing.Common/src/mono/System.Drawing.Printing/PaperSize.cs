// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.PaperSize.cs
//
// Author:
//   Dennis Hayes (dennish@Raytek.com)
//   Herve Poussineau (hpoussineau@fr.st)
//
// (C) 2002 Ximian, Inc
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

namespace System.Drawing.Printing
{
    /// <summary>
    /// Summary description for PaperSize.
    /// </summary>
    [Serializable]
    public class PaperSize
    {
        string name;
        int width;
        int height;
        PaperKind kind;
        internal bool is_default;

        public PaperSize()
        {

        }
        public PaperSize(string name, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.name = name;
        }

        internal PaperSize(string name, int width, int height, PaperKind kind, bool isDefault)
        {
            this.width = width;
            this.height = height;
            this.name = name;
            this.is_default = isDefault;
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                if (kind != PaperKind.Custom)
                    throw new ArgumentException();
                width = value;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (kind != PaperKind.Custom)
                    throw new ArgumentException();
                height = value;
            }
        }

        public string PaperName
        {
            get
            {
                return name;
            }
            set
            {
                if (kind != PaperKind.Custom)
                    throw new ArgumentException();
                name = value;
            }
        }

        public PaperKind Kind
        {
            get
            {
                // .net ignores the values that are less than 0
                // the value returned is not used internally, however.
                if (kind > PaperKind.PrcEnvelopeNumber10Rotated)
                    return PaperKind.Custom;

                return kind;
            }
        }
        public int RawKind
        {
            get
            {
                return (int)kind;
            }
            set
            {
                kind = (PaperKind)value;
            }
        }


        internal bool IsDefault
        {
            get { return this.is_default; }
            set { this.is_default = value; }
        }


        internal void SetKind(PaperKind k) { kind = k; }

        public override string ToString()
        {
            string ret = "[PaperSize {0} Kind={1} Height={2} Width={3}]";
            return String.Format(ret, this.PaperName, this.Kind, this.Height, this.Width);
        }
    }
}
