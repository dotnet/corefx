// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.ImageFlags.cs
//
// Author: Dennis Hayes (dennish@raytek.com)
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
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

namespace System.Drawing.Imaging
{

    [Flags]
    public enum ImageFlags
    {
        Caching = 131072,
        ColorSpaceCmyk = 32,
        ColorSpaceGray = 64,
        ColorSpaceRgb = 16,
        ColorSpaceYcbcr = 128,
        ColorSpaceYcck = 256,
        HasAlpha = 2,
        HasRealDpi = 4096,
        HasRealPixelSize = 8192,
        HasTranslucent = 4,
        None = 0,
        PartiallyScalable = 8,
        ReadOnly = 65536,
        Scalable = 1
    }
}
