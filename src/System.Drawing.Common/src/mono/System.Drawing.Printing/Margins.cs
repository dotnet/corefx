// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Margins.cs
//
// Authors:
//   Dennis Hayes (dennish@Raytek.com)
//   Andreas Nahr (ClassDevelopment@A-SoftTech.com)
//   Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Ximian, Inc
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

using System.ComponentModel;

namespace System.Drawing.Printing
{
    [Serializable]
#if !NETCORE
	[TypeConverter (typeof (MarginsConverter))]
#endif
    public class Margins : ICloneable
    {
        int left;
        int right;
        int top;
        int bottom;

        public Margins()
        {
            left = 100;
            right = 100;
            top = 100;
            bottom = 100;
        }

        public Margins(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                if (value < 0)
                    InvalidMargin("left");
                left = value;
            }
        }

        public int Right
        {
            get
            {
                return right;
            }
            set
            {
                if (value < 0)
                    InvalidMargin("right");
                right = value;
            }
        }

        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                if (value < 0)
                    InvalidMargin("top");
                top = value;
            }
        }

        public int Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                if (value < 0)
                    InvalidMargin("bottom");
                bottom = value;
            }
        }

        private void InvalidMargin(string property)
        {
            string msg = Locale.GetText("All Margins must be greater than 0");
            throw new System.ArgumentException(msg, property);
        }

        public object Clone()
        {
            return new Margins(left, right, top, bottom);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Margins);
        }

        private bool Equals(Margins m)
        {
            // avoid recursion with == operator
            if ((object)m == null)
                return false;
            return ((m.Left == left) && (m.Right == right) && (m.Top == top) && (m.Bottom == bottom));
        }

        public override int GetHashCode()
        {
            return left | (right << 8) | (right >> 24) | (top << 16) | (top >> 16) | (bottom << 24) | (bottom >> 8);
        }

        public override string ToString()
        {
            string ret = "[Margins Left={0} Right={1} Top={2} Bottom={3}]";
            return String.Format(ret, left, right, top, bottom);
        }

        public static bool operator ==(Margins m1, Margins m2)
        {
            // avoid recursion with == operator
            if ((object)m1 == null)
                return ((object)m2 == null);
            return m1.Equals(m2);
        }

        public static bool operator !=(Margins m1, Margins m2)
        {
            // avoid recursion with == operator
            if ((object)m1 == null)
                return ((object)m2 != null);
            return !m1.Equals(m2);
        }
    }
}
