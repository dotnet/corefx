// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.WmfPlaceableFileHeader.cs
//
// Authors:
//	Everaldo Canuto  <everaldo.canuto@bol.com.br>
//	Dennis Hayes (dennish@raytek.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004, 2006 Novell, Inc (http://www.novell.com)
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

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public sealed class WmfPlaceableFileHeader
    {

        // field order match: http://wvware.sourceforge.net/caolan/ora-wmf.html
        // for PLACEABLEMETAHEADER structure
        private int key;
        private short handle;
        private short left;
        private short top;
        private short right;
        private short bottom;
        private short inch;
        private int reserved;
        private short checksum;


        public WmfPlaceableFileHeader()
        {
            // header magic number
            key = unchecked((int)0x9AC6CDD7);
        }


        public short BboxBottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        public short BboxLeft
        {
            get { return left; }
            set { left = value; }
        }

        public short BboxRight
        {
            get { return right; }
            set { right = value; }
        }

        public short BboxTop
        {
            get { return top; }
            set { top = value; }
        }

        public short Checksum
        {
            get { return checksum; }
            set { checksum = value; }
        }

        public short Hmf
        {
            get { return handle; }
            set { handle = value; }
        }

        public short Inch
        {
            get { return inch; }
            set { inch = value; }
        }

        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        public int Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
    }
}
