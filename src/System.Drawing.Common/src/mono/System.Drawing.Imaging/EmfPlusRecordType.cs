// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.EmfPlusRecordType.cs
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

    public enum EmfPlusRecordType
    {
        BeginContainer = 16423,
        BeginContainerNoParams = 16424,
        Clear = 16393,
        Comment = 16387,
        DrawArc = 16402,
        DrawBeziers = 16409,
        DrawClosedCurve = 16407,
        DrawCurve = 16408,
        DrawDriverString = 16438,
        DrawEllipse = 16399,
        DrawImage = 16410,
        DrawImagePoints = 16411,
        DrawLines = 16397,
        DrawPath = 16405,
        DrawPie = 16401,
        DrawRects = 16395,
        DrawString = 16412,
        EmfAbortPath = 68,
        EmfAlphaBlend = 114,
        EmfAngleArc = 41,
        EmfArcTo = 55,
        EmfBeginPath = 59,
        EmfBitBlt = 76,
        EmfChord = 46,
        EmfCloseFigure = 61,
        EmfColorCorrectPalette = 111,
        EmfColorMatchToTargetW = 121,
        EmfCreateBrushIndirect = 39,
        EmfCreateColorSpace = 99,
        EmfCreateColorSpaceW = 122,
        EmfCreateDibPatternBrushPt = 94,
        EmfCreateMonoBrush = 93,
        EmfCreatePalette = 49,
        EmfCreatePen = 38,
        EmfDeleteColorSpace = 101,
        EmfDeleteObject = 40,
        EmfDrawEscape = 105,
        EmfEllipse = 42,
        EmfEndPath = 60,
        EmfEof = 14,
        EmfExcludeClipRect = 29,
        EmfExtCreateFontIndirect = 82,
        EmfExtCreatePen = 95,
        EmfExtEscape = 106,
        EmfExtFloodFill = 53,
        EmfExtSelectClipRgn = 75,
        EmfExtTextOutA = 83,
        EmfExtTextOutW = 84,
        EmfFillPath = 62,
        EmfFillRgn = 71,
        EmfFlattenPath = 65,
        EmfForceUfiMapping = 109,
        EmfFrameRgn = 72,
        EmfGdiComment = 70,
        EmfGlsBoundedRecord = 103,
        EmfGlsRecord = 102,
        EmfGradientFill = 118,
        EmfHeader = 1,
        EmfIntersectClipRect = 30,
        EmfInvertRgn = 73,
        EmfLineTo = 54,
        EmfMaskBlt = 78,
        EmfMax = 122,
        EmfMin = 1,
        EmfModifyWorldTransform = 36,
        EmfMoveToEx = 27,
        EmfNamedEscpae = 110,
        EmfOffsetClipRgn = 26,
        EmfPaintRgn = 74,
        EmfPie = 47,
        EmfPixelFormat = 104,
        EmfPlgBlt = 79,
        EmfPlusRecordBase = 16384,
        EmfPolyBezier = 2,
        EmfPolyBezier16 = 85,
        EmfPolyBezierTo = 5,
        EmfPolyBezierTo16 = 88,
        EmfPolyDraw = 56,
        EmfPolyDraw16 = 92,
        EmfPolygon = 3,
        EmfPolygon16 = 86,
        EmfPolyline = 4,
        EmfPolyPolygon16 = 91,
        EmfPolyPolyline = 7,
        EmfPolyline16 = 87,
        EmfPolyPolygon = 8,
        EmfPolyPolyline16 = 90,
        EmfPolyTextOutA = 96,
        EmfPolyTextOutW = 97,
        EmfRealizePalette = 52,
        EmfRectangle = 43,
        EmfReserved069 = 69,
        EmfReserved117 = 117,
        EmfResizePalette = 51,
        EmfRestoreDC = 34,
        EmfRoundArc = 45,
        EmfRoundRect = 44,
        EmfSaveDC = 33,
        EmfScaleViewportExtEx = 31,
        EmfScaleWindowExtEx = 32,
        EmfSelectClipPath = 67,
        EmfSelectObject = 37,
        EmfSelectPalette = 48,
        EmfSetArcDirection = 57,
        EmfSetBkColor = 25,
        EmfSetBkMode = 18,
        EmfSetBrushOrgEx = 13,
        EmfSetColorAdjustment = 23,
        EmfSetColorSpace = 100,
        EmfSetDIBitsToDevice = 80,
        EmfSetIcmMode = 98,
        EmfSetIcmProfileA = 112,
        EmfSetIcmProfileW = 113,
        EmfSetLayout = 115,
        EmfSetLinkedUfis = 119,
        EmfSetMapMode = 17,
        EmfSetMapperFlags = 16,
        EmfSetMetaRgn = 28,
        EmfSetMiterLimit = 58,
        EmfSetPaletteEntries = 50,
        EmfSetPixelV = 15,
        EmfSetPolyFillMode = 19,
        EmfSetROP2 = 20,
        EmfSetStretchBltMode = 21,
        EmfSetTextAlign = 22,
        EmfSetTextColor = 24,
        EmfSetTextJustification = 120,
        EmfSetViewportExtEx = 11,
        EmfSetViewportOrgEx = 12,
        EmfSetWindowExtEx = 9,
        EmfSetWindowOrgEx = 10,
        EmfSetWorldTransform = 35,
        EmfSmallTextOut = 108,
        EmfStartDoc = 107,
        EmfStretchBlt = 77,
        EmfStretchDIBits = 81,
        EmfStrokeAndFillPath = 63,
        EmfStrokePath = 64,
        EmfTransparentBlt = 116,
        EmfWidenPath = 66,
        EndContainer = 16425,
        EndOfFile = 16386,
        FillClosedCurve = 16406,
        FillEllipse = 16398,
        FillPath = 16404,
        FillPie = 16400,
        FillPolygon = 16396,
        FillRects = 16394,
        FillRegion = 16403,
        GetDC = 16388,
        Header = 16385,
        Invalid = 16384,
        Max = 16438,
        Min = 16385,
        MultiFormatEnd = 16391,
        MultiFormatSection = 16390,
        MultiFormatStart = 16389,
        MultiplyWorldTransform = 16428,
        Object = 16392,
        OffsetClip = 16437,
        ResetClip = 16433,
        ResetWorldTransform = 16427,
        Restore = 16422,
        RotateWorldTransform = 16431,
        Save = 16421,
        ScaleWorldTransform = 16430,
        SetAntiAliasMode = 16414,
        SetClipPath = 16435,
        SetClipRect = 16434,
        SetClipRegion = 16436,
        SetCompositingMode = 16419,
        SetCompositingQuality = 16420,
        SetInterpolationMode = 16417,
        SetPageTransform = 16432,
        SetPixelOffsetMode = 16418,
        SetRenderingOrigin = 16413,
        SetTextContrast = 16416,
        SetTextRenderingHint = 16415,
        SetWorldTransform = 16426,
        Total = 16439,
        TranslateWorldTransform = 16429,
        WmfAnimatePalette = 66614,
        WmfArc = 67607,
        WmfBitBlt = 67874,
        WmfChord = 67632,
        WmfCreateBrushIndirect = 66300,
        WmfCreateFontIndirect = 66299,
        WmfCreatePalette = 65783,
        WmfCreatePatternBrush = 66041,
        WmfCreatePenIndirect = 66298,
        WmfCreateRegion = 67327,
        WmfDeleteObject = 66032,
        WmfDibBitBlt = 67904,
        WmfDibCreatePatternBrush = 65858,
        WmfFillRegion = 66088,
        WmfFloodFill = 66585,
        WmfFrameRegion = 66601,
        WmfIntersectClipRect = 66582,
        WmfInvertRegion = 65834,
        WmfLineTo = 66067,
        WmfMoveTo = 66068,
        WmfOffsetCilpRgn = 66080,
        WmfOffsetViewportOrg = 66065,
        WmfOffsetWindowOrg = 66063,
        WmfPaintRegion = 65835,
        WmfPatBlt = 67101,
        WmfPie = 67610,
        WmfPolygon = 66340,
        WmfPolyline = 66341,
        WmfPolyPolygon = 66872,
        WmfRealizePalette = 65589,
        WmfRecordBase = 65536,
        WmfRectangle = 66587,
        WmfResizePalette = 65849,
        WmfRestoreDC = 65831,
        WmfRoundRect = 67100,
        WmfSaveDC = 65566,
        WmfScaleViewportExt = 66578,
        WmfScaleWindowExt = 66576,
        WmfSelectClipRegion = 65836,
        WmfSelectObject = 65837,
        WmfSelectPalette = 66100,
        WmfSetBkColor = 66049,
        WmfSetBkMode = 65794,
        WmfSetDibToDev = 68915,
        WmfSetLayout = 65865,
        WmfSetMapMode = 65795,
        WmfSetMapperFlags = 66097,
        WmfSetPalEntries = 65591,
        WmfSetPixel = 66591,
        WmfSetPolyFillMode = 65798,
        WmfSetRelAbs = 65797,
        WmfSetROP2 = 65796,
        WmfSetStretchBltMode = 65799,
        WmfSetTextAlign = 65838,
        WmfSetTextCharExtra = 65800,
        WmfSetTextColor = 66057,
        WmfSetTextJustification = 66058,
        WmfSetViewportExt = 66062,
        WmfSetViewportOrg = 66061,
        WmfSetWindowExt = 66060,
        WmfSetWindowOrg = 66059,
        WmfStretchBlt = 68387,
        WmfStretchDib = 69443,
        WmfTextOut = 66849,
        EmfPolyLineTo = 6,
        EmfPolylineTo16 = 89,
        WmfDibStretchBlt = 68417,
        WmfEllipse = 66584,
        WmfEscape = 67110,
        WmfExcludeClipRect = 66581,
        WmfExtFloodFill = 66888,
        WmfExtTextOut = 68146
    }
}
