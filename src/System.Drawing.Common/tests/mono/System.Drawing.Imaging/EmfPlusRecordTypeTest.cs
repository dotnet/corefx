// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// EmfPlusRecordType class unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Permissions;
using Xunit;

namespace MonoTests.System.Drawing.Imaging
{

    public class EmfPlusRecordTypeTest
    {

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void EmfRecords()
        {
            Assert.Equal(1, (int)EmfPlusRecordType.EmfMin);
            Assert.Equal(1, (int)EmfPlusRecordType.EmfHeader);
            Assert.Equal(2, (int)EmfPlusRecordType.EmfPolyBezier);
            Assert.Equal(3, (int)EmfPlusRecordType.EmfPolygon);
            Assert.Equal(4, (int)EmfPlusRecordType.EmfPolyline);
            Assert.Equal(5, (int)EmfPlusRecordType.EmfPolyBezierTo);
            Assert.Equal(6, (int)EmfPlusRecordType.EmfPolyLineTo);
            Assert.Equal(7, (int)EmfPlusRecordType.EmfPolyPolyline);
            Assert.Equal(8, (int)EmfPlusRecordType.EmfPolyPolygon);
            Assert.Equal(9, (int)EmfPlusRecordType.EmfSetWindowExtEx);
            Assert.Equal(10, (int)EmfPlusRecordType.EmfSetWindowOrgEx);
            Assert.Equal(11, (int)EmfPlusRecordType.EmfSetViewportExtEx);
            Assert.Equal(12, (int)EmfPlusRecordType.EmfSetViewportOrgEx);
            Assert.Equal(13, (int)EmfPlusRecordType.EmfSetBrushOrgEx);
            Assert.Equal(14, (int)EmfPlusRecordType.EmfEof);
            Assert.Equal(15, (int)EmfPlusRecordType.EmfSetPixelV);
            Assert.Equal(16, (int)EmfPlusRecordType.EmfSetMapperFlags);
            Assert.Equal(17, (int)EmfPlusRecordType.EmfSetMapMode);
            Assert.Equal(18, (int)EmfPlusRecordType.EmfSetBkMode);
            Assert.Equal(19, (int)EmfPlusRecordType.EmfSetPolyFillMode);
            Assert.Equal(20, (int)EmfPlusRecordType.EmfSetROP2);
            Assert.Equal(21, (int)EmfPlusRecordType.EmfSetStretchBltMode);
            Assert.Equal(22, (int)EmfPlusRecordType.EmfSetTextAlign);
            Assert.Equal(23, (int)EmfPlusRecordType.EmfSetColorAdjustment);
            Assert.Equal(24, (int)EmfPlusRecordType.EmfSetTextColor);
            Assert.Equal(25, (int)EmfPlusRecordType.EmfSetBkColor);
            Assert.Equal(26, (int)EmfPlusRecordType.EmfOffsetClipRgn);
            Assert.Equal(27, (int)EmfPlusRecordType.EmfMoveToEx);
            Assert.Equal(28, (int)EmfPlusRecordType.EmfSetMetaRgn);
            Assert.Equal(29, (int)EmfPlusRecordType.EmfExcludeClipRect);
            Assert.Equal(30, (int)EmfPlusRecordType.EmfIntersectClipRect);
            Assert.Equal(31, (int)EmfPlusRecordType.EmfScaleViewportExtEx);
            Assert.Equal(32, (int)EmfPlusRecordType.EmfScaleWindowExtEx);
            Assert.Equal(33, (int)EmfPlusRecordType.EmfSaveDC);
            Assert.Equal(34, (int)EmfPlusRecordType.EmfRestoreDC);
            Assert.Equal(35, (int)EmfPlusRecordType.EmfSetWorldTransform);
            Assert.Equal(36, (int)EmfPlusRecordType.EmfModifyWorldTransform);
            Assert.Equal(37, (int)EmfPlusRecordType.EmfSelectObject);
            Assert.Equal(38, (int)EmfPlusRecordType.EmfCreatePen);
            Assert.Equal(39, (int)EmfPlusRecordType.EmfCreateBrushIndirect);
            Assert.Equal(40, (int)EmfPlusRecordType.EmfDeleteObject);
            Assert.Equal(41, (int)EmfPlusRecordType.EmfAngleArc);
            Assert.Equal(42, (int)EmfPlusRecordType.EmfEllipse);
            Assert.Equal(43, (int)EmfPlusRecordType.EmfRectangle);
            Assert.Equal(44, (int)EmfPlusRecordType.EmfRoundRect);
            Assert.Equal(45, (int)EmfPlusRecordType.EmfRoundArc);
            Assert.Equal(46, (int)EmfPlusRecordType.EmfChord);
            Assert.Equal(47, (int)EmfPlusRecordType.EmfPie);
            Assert.Equal(48, (int)EmfPlusRecordType.EmfSelectPalette);
            Assert.Equal(49, (int)EmfPlusRecordType.EmfCreatePalette);
            Assert.Equal(50, (int)EmfPlusRecordType.EmfSetPaletteEntries);
            Assert.Equal(51, (int)EmfPlusRecordType.EmfResizePalette);
            Assert.Equal(52, (int)EmfPlusRecordType.EmfRealizePalette);
            Assert.Equal(53, (int)EmfPlusRecordType.EmfExtFloodFill);
            Assert.Equal(54, (int)EmfPlusRecordType.EmfLineTo);
            Assert.Equal(55, (int)EmfPlusRecordType.EmfArcTo);
            Assert.Equal(56, (int)EmfPlusRecordType.EmfPolyDraw);
            Assert.Equal(57, (int)EmfPlusRecordType.EmfSetArcDirection);
            Assert.Equal(58, (int)EmfPlusRecordType.EmfSetMiterLimit);
            Assert.Equal(59, (int)EmfPlusRecordType.EmfBeginPath);
            Assert.Equal(60, (int)EmfPlusRecordType.EmfEndPath);
            Assert.Equal(61, (int)EmfPlusRecordType.EmfCloseFigure);
            Assert.Equal(62, (int)EmfPlusRecordType.EmfFillPath);
            Assert.Equal(63, (int)EmfPlusRecordType.EmfStrokeAndFillPath);
            Assert.Equal(64, (int)EmfPlusRecordType.EmfStrokePath);
            Assert.Equal(65, (int)EmfPlusRecordType.EmfFlattenPath);
            Assert.Equal(66, (int)EmfPlusRecordType.EmfWidenPath);
            Assert.Equal(67, (int)EmfPlusRecordType.EmfSelectClipPath);
            Assert.Equal(68, (int)EmfPlusRecordType.EmfAbortPath);
            Assert.Equal(69, (int)EmfPlusRecordType.EmfReserved069);
            Assert.Equal(70, (int)EmfPlusRecordType.EmfGdiComment);
            Assert.Equal(71, (int)EmfPlusRecordType.EmfFillRgn);
            Assert.Equal(72, (int)EmfPlusRecordType.EmfFrameRgn);
            Assert.Equal(73, (int)EmfPlusRecordType.EmfInvertRgn);
            Assert.Equal(74, (int)EmfPlusRecordType.EmfPaintRgn);
            Assert.Equal(75, (int)EmfPlusRecordType.EmfExtSelectClipRgn);
            Assert.Equal(76, (int)EmfPlusRecordType.EmfBitBlt);
            Assert.Equal(77, (int)EmfPlusRecordType.EmfStretchBlt);
            Assert.Equal(78, (int)EmfPlusRecordType.EmfMaskBlt);
            Assert.Equal(79, (int)EmfPlusRecordType.EmfPlgBlt);
            Assert.Equal(80, (int)EmfPlusRecordType.EmfSetDIBitsToDevice);
            Assert.Equal(81, (int)EmfPlusRecordType.EmfStretchDIBits);
            Assert.Equal(82, (int)EmfPlusRecordType.EmfExtCreateFontIndirect);
            Assert.Equal(83, (int)EmfPlusRecordType.EmfExtTextOutA);
            Assert.Equal(84, (int)EmfPlusRecordType.EmfExtTextOutW);
            Assert.Equal(85, (int)EmfPlusRecordType.EmfPolyBezier16);
            Assert.Equal(86, (int)EmfPlusRecordType.EmfPolygon16);
            Assert.Equal(87, (int)EmfPlusRecordType.EmfPolyline16);
            Assert.Equal(88, (int)EmfPlusRecordType.EmfPolyBezierTo16);
            Assert.Equal(89, (int)EmfPlusRecordType.EmfPolylineTo16);
            Assert.Equal(90, (int)EmfPlusRecordType.EmfPolyPolyline16);
            Assert.Equal(91, (int)EmfPlusRecordType.EmfPolyPolygon16);
            Assert.Equal(92, (int)EmfPlusRecordType.EmfPolyDraw16);
            Assert.Equal(93, (int)EmfPlusRecordType.EmfCreateMonoBrush);
            Assert.Equal(94, (int)EmfPlusRecordType.EmfCreateDibPatternBrushPt);
            Assert.Equal(95, (int)EmfPlusRecordType.EmfExtCreatePen);
            Assert.Equal(96, (int)EmfPlusRecordType.EmfPolyTextOutA);
            Assert.Equal(97, (int)EmfPlusRecordType.EmfPolyTextOutW);
            Assert.Equal(98, (int)EmfPlusRecordType.EmfSetIcmMode);
            Assert.Equal(99, (int)EmfPlusRecordType.EmfCreateColorSpace);
            Assert.Equal(100, (int)EmfPlusRecordType.EmfSetColorSpace);
            Assert.Equal(101, (int)EmfPlusRecordType.EmfDeleteColorSpace);
            Assert.Equal(102, (int)EmfPlusRecordType.EmfGlsRecord);
            Assert.Equal(103, (int)EmfPlusRecordType.EmfGlsBoundedRecord);
            Assert.Equal(104, (int)EmfPlusRecordType.EmfPixelFormat);
            Assert.Equal(105, (int)EmfPlusRecordType.EmfDrawEscape);
            Assert.Equal(106, (int)EmfPlusRecordType.EmfExtEscape);
            Assert.Equal(107, (int)EmfPlusRecordType.EmfStartDoc);
            Assert.Equal(108, (int)EmfPlusRecordType.EmfSmallTextOut);
            Assert.Equal(109, (int)EmfPlusRecordType.EmfForceUfiMapping);
            Assert.Equal(110, (int)EmfPlusRecordType.EmfNamedEscpae);
            Assert.Equal(111, (int)EmfPlusRecordType.EmfColorCorrectPalette);
            Assert.Equal(112, (int)EmfPlusRecordType.EmfSetIcmProfileA);
            Assert.Equal(113, (int)EmfPlusRecordType.EmfSetIcmProfileW);
            Assert.Equal(114, (int)EmfPlusRecordType.EmfAlphaBlend);
            Assert.Equal(115, (int)EmfPlusRecordType.EmfSetLayout);
            Assert.Equal(116, (int)EmfPlusRecordType.EmfTransparentBlt);
            Assert.Equal(117, (int)EmfPlusRecordType.EmfReserved117);
            Assert.Equal(118, (int)EmfPlusRecordType.EmfGradientFill);
            Assert.Equal(119, (int)EmfPlusRecordType.EmfSetLinkedUfis);
            Assert.Equal(120, (int)EmfPlusRecordType.EmfSetTextJustification);
            Assert.Equal(121, (int)EmfPlusRecordType.EmfColorMatchToTargetW);
            Assert.Equal(122, (int)EmfPlusRecordType.EmfCreateColorSpaceW);
            Assert.Equal(122, (int)EmfPlusRecordType.EmfMax);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void EmfPlusRecords()
        {
            Assert.Equal(16384, (int)EmfPlusRecordType.EmfPlusRecordBase);
            Assert.Equal(16384, (int)EmfPlusRecordType.Invalid);
            Assert.Equal(16385, (int)EmfPlusRecordType.Min);
            Assert.Equal(16385, (int)EmfPlusRecordType.Header);
            Assert.Equal(16386, (int)EmfPlusRecordType.EndOfFile);
            Assert.Equal(16387, (int)EmfPlusRecordType.Comment);
            Assert.Equal(16388, (int)EmfPlusRecordType.GetDC);
            Assert.Equal(16389, (int)EmfPlusRecordType.MultiFormatStart);
            Assert.Equal(16390, (int)EmfPlusRecordType.MultiFormatSection);
            Assert.Equal(16391, (int)EmfPlusRecordType.MultiFormatEnd);
            Assert.Equal(16392, (int)EmfPlusRecordType.Object);
            Assert.Equal(16393, (int)EmfPlusRecordType.Clear);
            Assert.Equal(16394, (int)EmfPlusRecordType.FillRects);
            Assert.Equal(16395, (int)EmfPlusRecordType.DrawRects);
            Assert.Equal(16396, (int)EmfPlusRecordType.FillPolygon);
            Assert.Equal(16397, (int)EmfPlusRecordType.DrawLines);
            Assert.Equal(16398, (int)EmfPlusRecordType.FillEllipse);
            Assert.Equal(16399, (int)EmfPlusRecordType.DrawEllipse);
            Assert.Equal(16400, (int)EmfPlusRecordType.FillPie);
            Assert.Equal(16401, (int)EmfPlusRecordType.DrawPie);
            Assert.Equal(16402, (int)EmfPlusRecordType.DrawArc);
            Assert.Equal(16403, (int)EmfPlusRecordType.FillRegion);
            Assert.Equal(16404, (int)EmfPlusRecordType.FillPath);
            Assert.Equal(16405, (int)EmfPlusRecordType.DrawPath);
            Assert.Equal(16406, (int)EmfPlusRecordType.FillClosedCurve);
            Assert.Equal(16407, (int)EmfPlusRecordType.DrawClosedCurve);
            Assert.Equal(16408, (int)EmfPlusRecordType.DrawCurve);
            Assert.Equal(16409, (int)EmfPlusRecordType.DrawBeziers);
            Assert.Equal(16410, (int)EmfPlusRecordType.DrawImage);
            Assert.Equal(16411, (int)EmfPlusRecordType.DrawImagePoints);
            Assert.Equal(16412, (int)EmfPlusRecordType.DrawString);
            Assert.Equal(16413, (int)EmfPlusRecordType.SetRenderingOrigin);
            Assert.Equal(16414, (int)EmfPlusRecordType.SetAntiAliasMode);
            Assert.Equal(16415, (int)EmfPlusRecordType.SetTextRenderingHint);
            Assert.Equal(16416, (int)EmfPlusRecordType.SetTextContrast);
            Assert.Equal(16417, (int)EmfPlusRecordType.SetInterpolationMode);
            Assert.Equal(16418, (int)EmfPlusRecordType.SetPixelOffsetMode);
            Assert.Equal(16419, (int)EmfPlusRecordType.SetCompositingMode);
            Assert.Equal(16420, (int)EmfPlusRecordType.SetCompositingQuality);
            Assert.Equal(16421, (int)EmfPlusRecordType.Save);
            Assert.Equal(16422, (int)EmfPlusRecordType.Restore);
            Assert.Equal(16423, (int)EmfPlusRecordType.BeginContainer);
            Assert.Equal(16424, (int)EmfPlusRecordType.BeginContainerNoParams);
            Assert.Equal(16425, (int)EmfPlusRecordType.EndContainer);
            Assert.Equal(16426, (int)EmfPlusRecordType.SetWorldTransform);
            Assert.Equal(16427, (int)EmfPlusRecordType.ResetWorldTransform);
            Assert.Equal(16428, (int)EmfPlusRecordType.MultiplyWorldTransform);
            Assert.Equal(16429, (int)EmfPlusRecordType.TranslateWorldTransform);
            Assert.Equal(16430, (int)EmfPlusRecordType.ScaleWorldTransform);
            Assert.Equal(16431, (int)EmfPlusRecordType.RotateWorldTransform);
            Assert.Equal(16432, (int)EmfPlusRecordType.SetPageTransform);
            Assert.Equal(16433, (int)EmfPlusRecordType.ResetClip);
            Assert.Equal(16434, (int)EmfPlusRecordType.SetClipRect);
            Assert.Equal(16435, (int)EmfPlusRecordType.SetClipPath);
            Assert.Equal(16436, (int)EmfPlusRecordType.SetClipRegion);
            Assert.Equal(16437, (int)EmfPlusRecordType.OffsetClip);
            Assert.Equal(16438, (int)EmfPlusRecordType.DrawDriverString);
            Assert.Equal(16438, (int)EmfPlusRecordType.Max);
            Assert.Equal(16439, (int)EmfPlusRecordType.Total);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WmfRecords()
        {
            Assert.Equal(65536, (int)EmfPlusRecordType.WmfRecordBase);
            Assert.Equal(65566, (int)EmfPlusRecordType.WmfSaveDC);
            Assert.Equal(65589, (int)EmfPlusRecordType.WmfRealizePalette);
            Assert.Equal(65591, (int)EmfPlusRecordType.WmfSetPalEntries);
            Assert.Equal(65783, (int)EmfPlusRecordType.WmfCreatePalette);
            Assert.Equal(65794, (int)EmfPlusRecordType.WmfSetBkMode);
            Assert.Equal(65795, (int)EmfPlusRecordType.WmfSetMapMode);
            Assert.Equal(65796, (int)EmfPlusRecordType.WmfSetROP2);
            Assert.Equal(65797, (int)EmfPlusRecordType.WmfSetRelAbs);
            Assert.Equal(65798, (int)EmfPlusRecordType.WmfSetPolyFillMode);
            Assert.Equal(65799, (int)EmfPlusRecordType.WmfSetStretchBltMode);
            Assert.Equal(65800, (int)EmfPlusRecordType.WmfSetTextCharExtra);
            Assert.Equal(65831, (int)EmfPlusRecordType.WmfRestoreDC);
            Assert.Equal(65834, (int)EmfPlusRecordType.WmfInvertRegion);
            Assert.Equal(65835, (int)EmfPlusRecordType.WmfPaintRegion);
            Assert.Equal(65836, (int)EmfPlusRecordType.WmfSelectClipRegion);
            Assert.Equal(65837, (int)EmfPlusRecordType.WmfSelectObject);
            Assert.Equal(65838, (int)EmfPlusRecordType.WmfSetTextAlign);
            Assert.Equal(65849, (int)EmfPlusRecordType.WmfResizePalette);
            Assert.Equal(65858, (int)EmfPlusRecordType.WmfDibCreatePatternBrush);
            Assert.Equal(65865, (int)EmfPlusRecordType.WmfSetLayout);
            Assert.Equal(66032, (int)EmfPlusRecordType.WmfDeleteObject);
            Assert.Equal(66041, (int)EmfPlusRecordType.WmfCreatePatternBrush);
            Assert.Equal(66049, (int)EmfPlusRecordType.WmfSetBkColor);
            Assert.Equal(66057, (int)EmfPlusRecordType.WmfSetTextColor);
            Assert.Equal(66058, (int)EmfPlusRecordType.WmfSetTextJustification);
            Assert.Equal(66059, (int)EmfPlusRecordType.WmfSetWindowOrg);
            Assert.Equal(66060, (int)EmfPlusRecordType.WmfSetWindowExt);
            Assert.Equal(66061, (int)EmfPlusRecordType.WmfSetViewportOrg);
            Assert.Equal(66062, (int)EmfPlusRecordType.WmfSetViewportExt);
            Assert.Equal(66063, (int)EmfPlusRecordType.WmfOffsetWindowOrg);
            Assert.Equal(66065, (int)EmfPlusRecordType.WmfOffsetViewportOrg);
            Assert.Equal(66067, (int)EmfPlusRecordType.WmfLineTo);
            Assert.Equal(66068, (int)EmfPlusRecordType.WmfMoveTo);
            Assert.Equal(66080, (int)EmfPlusRecordType.WmfOffsetCilpRgn);
            Assert.Equal(66088, (int)EmfPlusRecordType.WmfFillRegion);
            Assert.Equal(66097, (int)EmfPlusRecordType.WmfSetMapperFlags);
            Assert.Equal(66100, (int)EmfPlusRecordType.WmfSelectPalette);
            Assert.Equal(66298, (int)EmfPlusRecordType.WmfCreatePenIndirect);
            Assert.Equal(66299, (int)EmfPlusRecordType.WmfCreateFontIndirect);
            Assert.Equal(66300, (int)EmfPlusRecordType.WmfCreateBrushIndirect);
            Assert.Equal(66340, (int)EmfPlusRecordType.WmfPolygon);
            Assert.Equal(66341, (int)EmfPlusRecordType.WmfPolyline);
            Assert.Equal(66576, (int)EmfPlusRecordType.WmfScaleWindowExt);
            Assert.Equal(66578, (int)EmfPlusRecordType.WmfScaleViewportExt);
            Assert.Equal(66581, (int)EmfPlusRecordType.WmfExcludeClipRect);
            Assert.Equal(66582, (int)EmfPlusRecordType.WmfIntersectClipRect);
            Assert.Equal(66584, (int)EmfPlusRecordType.WmfEllipse);
            Assert.Equal(66585, (int)EmfPlusRecordType.WmfFloodFill);
            Assert.Equal(66587, (int)EmfPlusRecordType.WmfRectangle);
            Assert.Equal(66591, (int)EmfPlusRecordType.WmfSetPixel);
            Assert.Equal(66601, (int)EmfPlusRecordType.WmfFrameRegion);
            Assert.Equal(66614, (int)EmfPlusRecordType.WmfAnimatePalette);
            Assert.Equal(66849, (int)EmfPlusRecordType.WmfTextOut);
            Assert.Equal(66872, (int)EmfPlusRecordType.WmfPolyPolygon);
            Assert.Equal(66888, (int)EmfPlusRecordType.WmfExtFloodFill);
            Assert.Equal(67100, (int)EmfPlusRecordType.WmfRoundRect);
            Assert.Equal(67101, (int)EmfPlusRecordType.WmfPatBlt);
            Assert.Equal(67110, (int)EmfPlusRecordType.WmfEscape);
            Assert.Equal(67327, (int)EmfPlusRecordType.WmfCreateRegion);
            Assert.Equal(67607, (int)EmfPlusRecordType.WmfArc);
            Assert.Equal(67610, (int)EmfPlusRecordType.WmfPie);
            Assert.Equal(67632, (int)EmfPlusRecordType.WmfChord);
            Assert.Equal(67874, (int)EmfPlusRecordType.WmfBitBlt);
            Assert.Equal(67904, (int)EmfPlusRecordType.WmfDibBitBlt);
            Assert.Equal(68146, (int)EmfPlusRecordType.WmfExtTextOut);
            Assert.Equal(68387, (int)EmfPlusRecordType.WmfStretchBlt);
            Assert.Equal(68417, (int)EmfPlusRecordType.WmfDibStretchBlt);
            Assert.Equal(68915, (int)EmfPlusRecordType.WmfSetDibToDev);
            Assert.Equal(69443, (int)EmfPlusRecordType.WmfStretchDib);
        }
    }
}
