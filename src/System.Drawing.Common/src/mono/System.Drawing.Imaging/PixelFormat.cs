// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// created on 20.02.2002 at 21:18
// 
// Image.cs
//
// Author: Christian Meyer
// eMail: Christian.Meyer@cs.tum.edu
//		Dennis Hayes
//		dennish@raytek.com
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
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

namespace System.Drawing.Imaging
{

    public enum PixelFormat
    {
        Alpha = 262144,
        Canonical = 2097152,
        DontCare = 0,
        Extended = 1048576,
        Format16bppArgb1555 = 397319,
        Format16bppGrayScale = 1052676,
        Format16bppRgb555 = 135173,
        Format16bppRgb565 = 135174,
        Format1bppIndexed = 196865,
        Format24bppRgb = 137224,
        Format32bppArgb = 2498570,
        Format32bppPArgb = 925707,
        Format32bppRgb = 139273,
        Format48bppRgb = 1060876,
        Format4bppIndexed = 197634,
        Format64bppArgb = 3424269,
        Format64bppPArgb = 1851406,
        Format8bppIndexed = 198659,
        Gdi = 131072,
        Indexed = 65536,
        Max = 15,
        PAlpha = 524288,
        Undefined = 0 //shows up in enumcheck as second "dontcare".
    }
}
