// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.EncoderValue.cs
//
// Author: Dennis Hayes (dennish@raytek.com)
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

    public enum EncoderValue
    {
        ColorTypeCMYK = 0,
        ColorTypeYCCK = 1,
        CompressionCCITT3 = 3,
        CompressionCCITT4 = 4,
        CompressionLZW = 2,
        CompressionNone = 6,
        CompressionRle = 5,
        Flush = 20,
        FrameDimensionPage = 23,
        FrameDimensionResolution = 22,
        FrameDimensionTime = 21,
        LastFrame = 19,
        MultiFrame = 18,
        RenderNonProgressive = 12,
        RenderProgressive = 11,
        ScanMethodInterlaced = 7,
        ScanMethodNonInterlaced = 8,
        TransformFlipHorizontal = 16,
        TransformFlipVertical = 17,
        TransformRotate180 = 14,
        TransformRotate270 = 15,
        TransformRotate90 = 13,
        VersionGif87 = 9,
        VersionGif89 = 10
    }
}
