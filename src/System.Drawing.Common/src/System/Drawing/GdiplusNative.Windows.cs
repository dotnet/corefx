// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Drawing
{
    internal unsafe partial class SafeNativeMethods
    {
        internal partial class Gdip
        {
            private const string LibraryName = "gdiplus.dll";

            private static SafeLibraryHandle s_gdipHandle;

            private static IntPtr LoadNativeLibrary()
            {
                s_gdipHandle = Interop.Kernel32.LoadLibraryExW(LibraryName, IntPtr.Zero, 0);
                return s_gdipHandle.DangerousGetHandle();
            }

            private static void PlatformInitialize()
            {
                LoadFunctionPointers();
            }

            private static void LoadFunctionPointers()
            {
                GdiplusStartup_ptr = FunctionWrapper.Load<GdiplusStartup_delegate>(s_gdipModule, "GdiplusStartup", LibraryName);
                GdipCreatePath_ptr = FunctionWrapper.Load<GdipCreatePath_delegate>(s_gdipModule, "GdipCreatePath", LibraryName);
                GdipCreatePath2_ptr = FunctionWrapper.Load<GdipCreatePath2_delegate>(s_gdipModule, "GdipCreatePath2", LibraryName);
                GdipCreatePath2I_ptr = FunctionWrapper.Load<GdipCreatePath2I_delegate>(s_gdipModule, "GdipCreatePath2I", LibraryName);
                GdipClonePath_ptr = FunctionWrapper.Load<GdipClonePath_delegate>(s_gdipModule, "GdipClonePath", LibraryName);
                GdipDeletePath_ptr = FunctionWrapper.Load<GdipDeletePath_delegate>(s_gdipModule, "GdipDeletePath", LibraryName);
                GdipResetPath_ptr = FunctionWrapper.Load<GdipResetPath_delegate>(s_gdipModule, "GdipResetPath", LibraryName);
                GdipGetPointCount_ptr = FunctionWrapper.Load<GdipGetPointCount_delegate>(s_gdipModule, "GdipGetPointCount", LibraryName);
                GdipGetPathTypes_ptr = FunctionWrapper.Load<GdipGetPathTypes_delegate>(s_gdipModule, "GdipGetPathTypes", LibraryName);
                GdipGetPathPoints_ptr = FunctionWrapper.Load<GdipGetPathPoints_delegate>(s_gdipModule, "GdipGetPathPoints", LibraryName);
                GdipGetPathFillMode_ptr = FunctionWrapper.Load<GdipGetPathFillMode_delegate>(s_gdipModule, "GdipGetPathFillMode", LibraryName);
                GdipSetPathFillMode_ptr = FunctionWrapper.Load<GdipSetPathFillMode_delegate>(s_gdipModule, "GdipSetPathFillMode", LibraryName);
                GdipGetPathData_ptr = FunctionWrapper.Load<GdipGetPathData_delegate>(s_gdipModule, "GdipGetPathData", LibraryName);
                GdipStartPathFigure_ptr = FunctionWrapper.Load<GdipStartPathFigure_delegate>(s_gdipModule, "GdipStartPathFigure", LibraryName);
                GdipClosePathFigure_ptr = FunctionWrapper.Load<GdipClosePathFigure_delegate>(s_gdipModule, "GdipClosePathFigure", LibraryName);
                GdipClosePathFigures_ptr = FunctionWrapper.Load<GdipClosePathFigures_delegate>(s_gdipModule, "GdipClosePathFigures", LibraryName);
                GdipSetPathMarker_ptr = FunctionWrapper.Load<GdipSetPathMarker_delegate>(s_gdipModule, "GdipSetPathMarker", LibraryName);
                GdipClearPathMarkers_ptr = FunctionWrapper.Load<GdipClearPathMarkers_delegate>(s_gdipModule, "GdipClearPathMarkers", LibraryName);
                GdipReversePath_ptr = FunctionWrapper.Load<GdipReversePath_delegate>(s_gdipModule, "GdipReversePath", LibraryName);
                GdipGetPathLastPoint_ptr = FunctionWrapper.Load<GdipGetPathLastPoint_delegate>(s_gdipModule, "GdipGetPathLastPoint", LibraryName);
                GdipAddPathLine_ptr = FunctionWrapper.Load<GdipAddPathLine_delegate>(s_gdipModule, "GdipAddPathLine", LibraryName);
                GdipAddPathLine2_ptr = FunctionWrapper.Load<GdipAddPathLine2_delegate>(s_gdipModule, "GdipAddPathLine2", LibraryName);
                GdipAddPathArc_ptr = FunctionWrapper.Load<GdipAddPathArc_delegate>(s_gdipModule, "GdipAddPathArc", LibraryName);
                GdipAddPathBezier_ptr = FunctionWrapper.Load<GdipAddPathBezier_delegate>(s_gdipModule, "GdipAddPathBezier", LibraryName);
                GdipAddPathBeziers_ptr = FunctionWrapper.Load<GdipAddPathBeziers_delegate>(s_gdipModule, "GdipAddPathBeziers", LibraryName);
                GdipAddPathCurve_ptr = FunctionWrapper.Load<GdipAddPathCurve_delegate>(s_gdipModule, "GdipAddPathCurve", LibraryName);
                GdipAddPathCurve2_ptr = FunctionWrapper.Load<GdipAddPathCurve2_delegate>(s_gdipModule, "GdipAddPathCurve2", LibraryName);
                GdipAddPathCurve3_ptr = FunctionWrapper.Load<GdipAddPathCurve3_delegate>(s_gdipModule, "GdipAddPathCurve3", LibraryName);
                GdipAddPathClosedCurve_ptr = FunctionWrapper.Load<GdipAddPathClosedCurve_delegate>(s_gdipModule, "GdipAddPathClosedCurve", LibraryName);
                GdipAddPathClosedCurve2_ptr = FunctionWrapper.Load<GdipAddPathClosedCurve2_delegate>(s_gdipModule, "GdipAddPathClosedCurve2", LibraryName);
                GdipAddPathRectangle_ptr = FunctionWrapper.Load<GdipAddPathRectangle_delegate>(s_gdipModule, "GdipAddPathRectangle", LibraryName);
                GdipAddPathRectangles_ptr = FunctionWrapper.Load<GdipAddPathRectangles_delegate>(s_gdipModule, "GdipAddPathRectangles", LibraryName);
                GdipAddPathEllipse_ptr = FunctionWrapper.Load<GdipAddPathEllipse_delegate>(s_gdipModule, "GdipAddPathEllipse", LibraryName);
                GdipAddPathPie_ptr = FunctionWrapper.Load<GdipAddPathPie_delegate>(s_gdipModule, "GdipAddPathPie", LibraryName);
                GdipAddPathPolygon_ptr = FunctionWrapper.Load<GdipAddPathPolygon_delegate>(s_gdipModule, "GdipAddPathPolygon", LibraryName);
                GdipAddPathPath_ptr = FunctionWrapper.Load<GdipAddPathPath_delegate>(s_gdipModule, "GdipAddPathPath", LibraryName);
                GdipAddPathString_ptr = FunctionWrapper.Load<GdipAddPathString_delegate>(s_gdipModule, "GdipAddPathString", LibraryName);
                GdipAddPathStringI_ptr = FunctionWrapper.Load<GdipAddPathStringI_delegate>(s_gdipModule, "GdipAddPathStringI", LibraryName);
                GdipAddPathLineI_ptr = FunctionWrapper.Load<GdipAddPathLineI_delegate>(s_gdipModule, "GdipAddPathLineI", LibraryName);
                GdipAddPathLine2I_ptr = FunctionWrapper.Load<GdipAddPathLine2I_delegate>(s_gdipModule, "GdipAddPathLine2I", LibraryName);
                GdipAddPathArcI_ptr = FunctionWrapper.Load<GdipAddPathArcI_delegate>(s_gdipModule, "GdipAddPathArcI", LibraryName);
                GdipAddPathBezierI_ptr = FunctionWrapper.Load<GdipAddPathBezierI_delegate>(s_gdipModule, "GdipAddPathBezierI", LibraryName);
                GdipAddPathBeziersI_ptr = FunctionWrapper.Load<GdipAddPathBeziersI_delegate>(s_gdipModule, "GdipAddPathBeziersI", LibraryName);
                GdipAddPathCurveI_ptr = FunctionWrapper.Load<GdipAddPathCurveI_delegate>(s_gdipModule, "GdipAddPathCurveI", LibraryName);
                GdipAddPathCurve2I_ptr = FunctionWrapper.Load<GdipAddPathCurve2I_delegate>(s_gdipModule, "GdipAddPathCurve2I", LibraryName);
                GdipAddPathCurve3I_ptr = FunctionWrapper.Load<GdipAddPathCurve3I_delegate>(s_gdipModule, "GdipAddPathCurve3I", LibraryName);
                GdipAddPathClosedCurveI_ptr = FunctionWrapper.Load<GdipAddPathClosedCurveI_delegate>(s_gdipModule, "GdipAddPathClosedCurveI", LibraryName);
                GdipAddPathClosedCurve2I_ptr = FunctionWrapper.Load<GdipAddPathClosedCurve2I_delegate>(s_gdipModule, "GdipAddPathClosedCurve2I", LibraryName);
                GdipAddPathRectangleI_ptr = FunctionWrapper.Load<GdipAddPathRectangleI_delegate>(s_gdipModule, "GdipAddPathRectangleI", LibraryName);
                GdipAddPathRectanglesI_ptr = FunctionWrapper.Load<GdipAddPathRectanglesI_delegate>(s_gdipModule, "GdipAddPathRectanglesI", LibraryName);
                GdipAddPathEllipseI_ptr = FunctionWrapper.Load<GdipAddPathEllipseI_delegate>(s_gdipModule, "GdipAddPathEllipseI", LibraryName);
                GdipAddPathPieI_ptr = FunctionWrapper.Load<GdipAddPathPieI_delegate>(s_gdipModule, "GdipAddPathPieI", LibraryName);
                GdipAddPathPolygonI_ptr = FunctionWrapper.Load<GdipAddPathPolygonI_delegate>(s_gdipModule, "GdipAddPathPolygonI", LibraryName);
                GdipFlattenPath_ptr = FunctionWrapper.Load<GdipFlattenPath_delegate>(s_gdipModule, "GdipFlattenPath", LibraryName);
                GdipWidenPath_ptr = FunctionWrapper.Load<GdipWidenPath_delegate>(s_gdipModule, "GdipWidenPath", LibraryName);
                GdipWarpPath_ptr = FunctionWrapper.Load<GdipWarpPath_delegate>(s_gdipModule, "GdipWarpPath", LibraryName);
                GdipTransformPath_ptr = FunctionWrapper.Load<GdipTransformPath_delegate>(s_gdipModule, "GdipTransformPath", LibraryName);
                GdipGetPathWorldBounds_ptr = FunctionWrapper.Load<GdipGetPathWorldBounds_delegate>(s_gdipModule, "GdipGetPathWorldBounds", LibraryName);
                GdipIsVisiblePathPoint_ptr = FunctionWrapper.Load<GdipIsVisiblePathPoint_delegate>(s_gdipModule, "GdipIsVisiblePathPoint", LibraryName);
                GdipIsVisiblePathPointI_ptr = FunctionWrapper.Load<GdipIsVisiblePathPointI_delegate>(s_gdipModule, "GdipIsVisiblePathPointI", LibraryName);
                GdipIsOutlineVisiblePathPoint_ptr = FunctionWrapper.Load<GdipIsOutlineVisiblePathPoint_delegate>(s_gdipModule, "GdipIsOutlineVisiblePathPoint", LibraryName);
                GdipIsOutlineVisiblePathPointI_ptr = FunctionWrapper.Load<GdipIsOutlineVisiblePathPointI_delegate>(s_gdipModule, "GdipIsOutlineVisiblePathPointI", LibraryName);
                GdipDeleteBrush_ptr = FunctionWrapper.Load<GdipDeleteBrush_delegate>(s_gdipModule, "GdipDeleteBrush", LibraryName);
                GdipLoadImageFromStream_ptr = FunctionWrapper.Load<GdipLoadImageFromStream_delegate>(s_gdipModule, "GdipLoadImageFromStream", LibraryName);
                GdipLoadImageFromFile_ptr = FunctionWrapper.Load<GdipLoadImageFromFile_delegate>(s_gdipModule, "GdipLoadImageFromFile", LibraryName);
                GdipLoadImageFromStreamICM_ptr = FunctionWrapper.Load<GdipLoadImageFromStreamICM_delegate>(s_gdipModule, "GdipLoadImageFromStreamICM", LibraryName);
                GdipLoadImageFromFileICM_ptr = FunctionWrapper.Load<GdipLoadImageFromFileICM_delegate>(s_gdipModule, "GdipLoadImageFromFileICM", LibraryName);
                GdipCloneImage_ptr = FunctionWrapper.Load<GdipCloneImage_delegate>(s_gdipModule, "GdipCloneImage", LibraryName);
                GdipDisposeImage_ptr = FunctionWrapper.Load<GdipDisposeImage_delegate>(s_gdipModule, "GdipDisposeImage", LibraryName);
                GdipSaveImageToFile_ptr = FunctionWrapper.Load<GdipSaveImageToFile_delegate>(s_gdipModule, "GdipSaveImageToFile", LibraryName);
                GdipSaveImageToStream_ptr = FunctionWrapper.Load<GdipSaveImageToStream_delegate>(s_gdipModule, "GdipSaveImageToStream", LibraryName);
                GdipSaveAdd_ptr = FunctionWrapper.Load<GdipSaveAdd_delegate>(s_gdipModule, "GdipSaveAdd", LibraryName);
                GdipSaveAddImage_ptr = FunctionWrapper.Load<GdipSaveAddImage_delegate>(s_gdipModule, "GdipSaveAddImage", LibraryName);
                GdipGetImageGraphicsContext_ptr = FunctionWrapper.Load<GdipGetImageGraphicsContext_delegate>(s_gdipModule, "GdipGetImageGraphicsContext", LibraryName);
                GdipGetImageBounds_ptr = FunctionWrapper.Load<GdipGetImageBounds_delegate>(s_gdipModule, "GdipGetImageBounds", LibraryName);
                GdipGetImageType_ptr = FunctionWrapper.Load<GdipGetImageType_delegate>(s_gdipModule, "GdipGetImageType", LibraryName);
                GdipGetImageThumbnail_ptr = FunctionWrapper.Load<GdipGetImageThumbnail_delegate>(s_gdipModule, "GdipGetImageThumbnail", LibraryName);
                GdipGetEncoderParameterListSize_ptr = FunctionWrapper.Load<GdipGetEncoderParameterListSize_delegate>(s_gdipModule, "GdipGetEncoderParameterListSize", LibraryName);
                GdipGetEncoderParameterList_ptr = FunctionWrapper.Load<GdipGetEncoderParameterList_delegate>(s_gdipModule, "GdipGetEncoderParameterList", LibraryName);
                GdipGetImagePalette_ptr = FunctionWrapper.Load<GdipGetImagePalette_delegate>(s_gdipModule, "GdipGetImagePalette", LibraryName);
                GdipSetImagePalette_ptr = FunctionWrapper.Load<GdipSetImagePalette_delegate>(s_gdipModule, "GdipSetImagePalette", LibraryName);
                GdipGetImagePaletteSize_ptr = FunctionWrapper.Load<GdipGetImagePaletteSize_delegate>(s_gdipModule, "GdipGetImagePaletteSize", LibraryName);
                GdipGetPropertyCount_ptr = FunctionWrapper.Load<GdipGetPropertyCount_delegate>(s_gdipModule, "GdipGetPropertyCount", LibraryName);
                GdipGetPropertyIdList_ptr = FunctionWrapper.Load<GdipGetPropertyIdList_delegate>(s_gdipModule, "GdipGetPropertyIdList", LibraryName);
                GdipGetPropertyItemSize_ptr = FunctionWrapper.Load<GdipGetPropertyItemSize_delegate>(s_gdipModule, "GdipGetPropertyItemSize", LibraryName);
                GdipGetPropertyItem_ptr = FunctionWrapper.Load<GdipGetPropertyItem_delegate>(s_gdipModule, "GdipGetPropertyItem", LibraryName);
                GdipGetPropertySize_ptr = FunctionWrapper.Load<GdipGetPropertySize_delegate>(s_gdipModule, "GdipGetPropertySize", LibraryName);
                GdipGetAllPropertyItems_ptr = FunctionWrapper.Load<GdipGetAllPropertyItems_delegate>(s_gdipModule, "GdipGetAllPropertyItems", LibraryName);
                GdipSetPropertyItem_ptr = FunctionWrapper.Load<GdipSetPropertyItem_delegate>(s_gdipModule, "GdipSetPropertyItem", LibraryName);
                GdipImageForceValidation_ptr = FunctionWrapper.Load<GdipImageForceValidation_delegate>(s_gdipModule, "GdipImageForceValidation", LibraryName);
                GdipCreateFromHDC_ptr = FunctionWrapper.Load<GdipCreateFromHDC_delegate>(s_gdipModule, "GdipCreateFromHDC", LibraryName);
                GdipCreateFromHDC2_ptr = FunctionWrapper.Load<GdipCreateFromHDC2_delegate>(s_gdipModule, "GdipCreateFromHDC2", LibraryName);
                GdipCreateFromHWND_ptr = FunctionWrapper.Load<GdipCreateFromHWND_delegate>(s_gdipModule, "GdipCreateFromHWND", LibraryName);
                GdipDeleteGraphics_ptr = FunctionWrapper.Load<GdipDeleteGraphics_delegate>(s_gdipModule, "GdipDeleteGraphics", LibraryName);
                GdipReleaseDC_ptr = FunctionWrapper.Load<GdipReleaseDC_delegate>(s_gdipModule, "GdipReleaseDC", LibraryName);
                GdipTransformPoints_ptr = FunctionWrapper.Load<GdipTransformPoints_delegate>(s_gdipModule, "GdipTransformPoints", LibraryName);
                GdipTransformPointsI_ptr = FunctionWrapper.Load<GdipTransformPointsI_delegate>(s_gdipModule, "GdipTransformPointsI", LibraryName);
                GdipGetNearestColor_ptr = FunctionWrapper.Load<GdipGetNearestColor_delegate>(s_gdipModule, "GdipGetNearestColor", LibraryName);
                GdipCreateHalftonePalette_ptr = FunctionWrapper.Load<GdipCreateHalftonePalette_delegate>(s_gdipModule, "GdipCreateHalftonePalette", LibraryName);
                GdipDrawBeziers_ptr = FunctionWrapper.Load<GdipDrawBeziers_delegate>(s_gdipModule, "GdipDrawBeziers", LibraryName);
                GdipDrawBeziersI_ptr = FunctionWrapper.Load<GdipDrawBeziersI_delegate>(s_gdipModule, "GdipDrawBeziersI", LibraryName);
                GdipFillPath_ptr = FunctionWrapper.Load<GdipFillPath_delegate>(s_gdipModule, "GdipFillPath", LibraryName);
                GdipEnumerateMetafileDestPoint_ptr = FunctionWrapper.Load<GdipEnumerateMetafileDestPoint_delegate>(s_gdipModule, "GdipEnumerateMetafileDestPoint", LibraryName);
                GdipEnumerateMetafileDestPointI_ptr = FunctionWrapper.Load<GdipEnumerateMetafileDestPointI_delegate>(s_gdipModule, "GdipEnumerateMetafileDestPointI", LibraryName);
                GdipEnumerateMetafileDestRect_ptr = FunctionWrapper.Load<GdipEnumerateMetafileDestRect_delegate>(s_gdipModule, "GdipEnumerateMetafileDestRect", LibraryName);
                GdipEnumerateMetafileDestRectI_ptr = FunctionWrapper.Load<GdipEnumerateMetafileDestRectI_delegate>(s_gdipModule, "GdipEnumerateMetafileDestRectI", LibraryName);
                GdipEnumerateMetafileDestPoints_ptr = FunctionWrapper.Load<GdipEnumerateMetafileDestPoints_delegate>(s_gdipModule, "GdipEnumerateMetafileDestPoints", LibraryName);
                GdipEnumerateMetafileDestPointsI_ptr = FunctionWrapper.Load<GdipEnumerateMetafileDestPointsI_delegate>(s_gdipModule, "GdipEnumerateMetafileDestPointsI", LibraryName);
                GdipEnumerateMetafileSrcRectDestPoint_ptr = FunctionWrapper.Load<GdipEnumerateMetafileSrcRectDestPoint_delegate>(s_gdipModule, "GdipEnumerateMetafileSrcRectDestPoint", LibraryName);
                GdipEnumerateMetafileSrcRectDestPointI_ptr = FunctionWrapper.Load<GdipEnumerateMetafileSrcRectDestPointI_delegate>(s_gdipModule, "GdipEnumerateMetafileSrcRectDestPointI", LibraryName);
                GdipEnumerateMetafileSrcRectDestRect_ptr = FunctionWrapper.Load<GdipEnumerateMetafileSrcRectDestRect_delegate>(s_gdipModule, "GdipEnumerateMetafileSrcRectDestRect", LibraryName);
                GdipEnumerateMetafileSrcRectDestRectI_ptr = FunctionWrapper.Load<GdipEnumerateMetafileSrcRectDestRectI_delegate>(s_gdipModule, "GdipEnumerateMetafileSrcRectDestRectI", LibraryName);
                GdipEnumerateMetafileSrcRectDestPoints_ptr = FunctionWrapper.Load<GdipEnumerateMetafileSrcRectDestPoints_delegate>(s_gdipModule, "GdipEnumerateMetafileSrcRectDestPoints", LibraryName);
                GdipEnumerateMetafileSrcRectDestPointsI_ptr = FunctionWrapper.Load<GdipEnumerateMetafileSrcRectDestPointsI_delegate>(s_gdipModule, "GdipEnumerateMetafileSrcRectDestPointsI", LibraryName);
                GdipPlayMetafileRecord_ptr = FunctionWrapper.Load<GdipPlayMetafileRecord_delegate>(s_gdipModule, "GdipPlayMetafileRecord", LibraryName);
                GdipRestoreGraphics_ptr = FunctionWrapper.Load<GdipRestoreGraphics_delegate>(s_gdipModule, "GdipRestoreGraphics", LibraryName);
                GdipEndContainer_ptr = FunctionWrapper.Load<GdipEndContainer_delegate>(s_gdipModule, "GdipEndContainer", LibraryName);
                GdipGetMetafileHeaderFromWmf_ptr = FunctionWrapper.Load<GdipGetMetafileHeaderFromWmf_delegate>(s_gdipModule, "GdipGetMetafileHeaderFromWmf", LibraryName);
                GdipGetMetafileHeaderFromEmf_ptr = FunctionWrapper.Load<GdipGetMetafileHeaderFromEmf_delegate>(s_gdipModule, "GdipGetMetafileHeaderFromEmf", LibraryName);
                GdipGetMetafileHeaderFromFile_ptr = FunctionWrapper.Load<GdipGetMetafileHeaderFromFile_delegate>(s_gdipModule, "GdipGetMetafileHeaderFromFile", LibraryName);
                GdipGetMetafileHeaderFromStream_ptr = FunctionWrapper.Load<GdipGetMetafileHeaderFromStream_delegate>(s_gdipModule, "GdipGetMetafileHeaderFromStream", LibraryName);
                GdipGetMetafileHeaderFromMetafile_ptr = FunctionWrapper.Load<GdipGetMetafileHeaderFromMetafile_delegate>(s_gdipModule, "GdipGetMetafileHeaderFromMetafile", LibraryName);
                GdipGetHemfFromMetafile_ptr = FunctionWrapper.Load<GdipGetHemfFromMetafile_delegate>(s_gdipModule, "GdipGetHemfFromMetafile", LibraryName);
                GdipCreateMetafileFromWmf_ptr = FunctionWrapper.Load<GdipCreateMetafileFromWmf_delegate>(s_gdipModule, "GdipCreateMetafileFromWmf", LibraryName);
                GdipCreateMetafileFromEmf_ptr = FunctionWrapper.Load<GdipCreateMetafileFromEmf_delegate>(s_gdipModule, "GdipCreateMetafileFromEmf", LibraryName);
                GdipCreateMetafileFromFile_ptr = FunctionWrapper.Load<GdipCreateMetafileFromFile_delegate>(s_gdipModule, "GdipCreateMetafileFromFile", LibraryName);
                GdipCreateMetafileFromStream_ptr = FunctionWrapper.Load<GdipCreateMetafileFromStream_delegate>(s_gdipModule, "GdipCreateMetafileFromStream", LibraryName);
                GdipRecordMetafile_ptr = FunctionWrapper.Load<GdipRecordMetafile_delegate>(s_gdipModule, "GdipRecordMetafile", LibraryName);
                GdipRecordMetafile2_ptr = FunctionWrapper.Load<GdipRecordMetafile2_delegate>(s_gdipModule, "GdipRecordMetafile", LibraryName);
                GdipRecordMetafileI_ptr = FunctionWrapper.Load<GdipRecordMetafileI_delegate>(s_gdipModule, "GdipRecordMetafileI", LibraryName);
                GdipRecordMetafileFileName_ptr = FunctionWrapper.Load<GdipRecordMetafileFileName_delegate>(s_gdipModule, "GdipRecordMetafileFileName", LibraryName);
                GdipRecordMetafileFileName2_ptr = FunctionWrapper.Load<GdipRecordMetafileFileName2_delegate>(s_gdipModule, "GdipRecordMetafileFileName", LibraryName);
                GdipRecordMetafileFileNameI_ptr = FunctionWrapper.Load<GdipRecordMetafileFileNameI_delegate>(s_gdipModule, "GdipRecordMetafileFileNameI", LibraryName);
                GdipRecordMetafileStream_ptr = FunctionWrapper.Load<GdipRecordMetafileStream_delegate>(s_gdipModule, "GdipRecordMetafileStream", LibraryName);
                GdipRecordMetafileStream2_ptr = FunctionWrapper.Load<GdipRecordMetafileStream2_delegate>(s_gdipModule, "GdipRecordMetafileStream", LibraryName);
                GdipRecordMetafileStreamI_ptr = FunctionWrapper.Load<GdipRecordMetafileStreamI_delegate>(s_gdipModule, "GdipRecordMetafileStreamI", LibraryName);
                GdipComment_ptr = FunctionWrapper.Load<GdipComment_delegate>(s_gdipModule, "GdipComment", LibraryName);
                GdipCreateFontFromDC_ptr = FunctionWrapper.Load<GdipCreateFontFromDC_delegate>(s_gdipModule, "GdipCreateFontFromDC", LibraryName);
                GdipCreateFontFromLogfontW_ptr = FunctionWrapper.Load<GdipCreateFontFromLogfontW_delegate>(s_gdipModule, "GdipCreateFontFromLogfontW", LibraryName);
                GdipCreateBitmapFromStream_ptr = FunctionWrapper.Load<GdipCreateBitmapFromStream_delegate>(s_gdipModule, "GdipCreateBitmapFromStream", LibraryName);
                GdipCreateBitmapFromStreamICM_ptr = FunctionWrapper.Load<GdipCreateBitmapFromStreamICM_delegate>(s_gdipModule, "GdipCreateBitmapFromStreamICM", LibraryName);
                GdipFillPie_ptr = FunctionWrapper.Load<GdipFillPie_delegate>(s_gdipModule, "GdipFillPie", LibraryName);
                GdipFillPieI_ptr = FunctionWrapper.Load<GdipFillPieI_delegate>(s_gdipModule, "GdipFillPieI", LibraryName);
            }

            // Imported functions

            private delegate int GdiplusStartup_delegate(out IntPtr token, ref StartupInput input, out StartupOutput output);
            private static FunctionWrapper<GdiplusStartup_delegate> GdiplusStartup_ptr;
            private static int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output) => GdiplusStartup_ptr.Delegate(out token, ref input, out output);

            private delegate int GdipCreatePath_delegate(int brushMode, out IntPtr path);
            private static FunctionWrapper<GdipCreatePath_delegate> GdipCreatePath_ptr;
            internal static int GdipCreatePath(int brushMode, out IntPtr path) => GdipCreatePath_ptr.Delegate(brushMode, out path);

            private delegate int GdipCreatePath2_delegate(PointF* points, byte* types, int count, int brushMode, out IntPtr path);
            private static FunctionWrapper<GdipCreatePath2_delegate> GdipCreatePath2_ptr;
            internal static int GdipCreatePath2(PointF* points, byte* types, int count, int brushMode, out IntPtr path) => GdipCreatePath2_ptr.Delegate(points, types, count, brushMode, out path);

            private delegate int GdipCreatePath2I_delegate(Point* points, byte* types, int count, int brushMode, out IntPtr path);
            private static FunctionWrapper<GdipCreatePath2I_delegate> GdipCreatePath2I_ptr;
            internal static int GdipCreatePath2I(Point* points, byte* types, int count, int brushMode, out IntPtr path) => GdipCreatePath2I_ptr.Delegate(points, types, count, brushMode, out path);

            private delegate int GdipClonePath_delegate(HandleRef path, out IntPtr clonepath);
            private static FunctionWrapper<GdipClonePath_delegate> GdipClonePath_ptr;
            internal static int GdipClonePath(HandleRef path, out IntPtr clonepath) => GdipClonePath_ptr.Delegate(path, out clonepath);

            private delegate int GdipDeletePath_delegate(HandleRef path);
            private static FunctionWrapper<GdipDeletePath_delegate> GdipDeletePath_ptr;
            internal static int IntGdipDeletePath(HandleRef path) => GdipDeletePath_ptr.Delegate(path);

            private delegate int GdipResetPath_delegate(HandleRef path);
            private static FunctionWrapper<GdipResetPath_delegate> GdipResetPath_ptr;
            internal static int GdipResetPath(HandleRef path) => GdipResetPath_ptr.Delegate(path);

            private delegate int GdipGetPointCount_delegate(HandleRef path, out int count);
            private static FunctionWrapper<GdipGetPointCount_delegate> GdipGetPointCount_ptr;
            internal static int GdipGetPointCount(HandleRef path, out int count) => GdipGetPointCount_ptr.Delegate(path, out count);

            private delegate int GdipGetPathTypes_delegate(HandleRef path, byte[] types, int count);
            private static FunctionWrapper<GdipGetPathTypes_delegate> GdipGetPathTypes_ptr;
            internal static int GdipGetPathTypes(HandleRef path, byte[] types, int count) => GdipGetPathTypes_ptr.Delegate(path, types, count);

            private delegate int GdipGetPathPoints_delegate(HandleRef path, PointF* points, int count);
            private static FunctionWrapper<GdipGetPathPoints_delegate> GdipGetPathPoints_ptr;
            internal static int GdipGetPathPoints(HandleRef path, PointF* points, int count) => GdipGetPathPoints_ptr.Delegate(path, points, count);

            private delegate int GdipGetPathFillMode_delegate(HandleRef path, out FillMode fillmode);
            private static FunctionWrapper<GdipGetPathFillMode_delegate> GdipGetPathFillMode_ptr;
            internal static int GdipGetPathFillMode(HandleRef path, out FillMode fillmode) => GdipGetPathFillMode_ptr.Delegate(path, out fillmode);

            private delegate int GdipSetPathFillMode_delegate(HandleRef path, FillMode fillmode);
            private static FunctionWrapper<GdipSetPathFillMode_delegate> GdipSetPathFillMode_ptr;
            internal static int GdipSetPathFillMode(HandleRef path, FillMode fillmode) => GdipSetPathFillMode_ptr.Delegate(path, fillmode);

            private delegate int GdipGetPathData_delegate(HandleRef path, GpPathData* pathData);
            private static FunctionWrapper<GdipGetPathData_delegate> GdipGetPathData_ptr;
            internal static int GdipGetPathData(HandleRef path, GpPathData* pathData) => GdipGetPathData_ptr.Delegate(path, pathData);

            private delegate int GdipStartPathFigure_delegate(HandleRef path);
            private static FunctionWrapper<GdipStartPathFigure_delegate> GdipStartPathFigure_ptr;
            internal static int GdipStartPathFigure(HandleRef path) => GdipStartPathFigure_ptr.Delegate(path);

            private delegate int GdipClosePathFigure_delegate(HandleRef path);
            private static FunctionWrapper<GdipClosePathFigure_delegate> GdipClosePathFigure_ptr;
            internal static int GdipClosePathFigure(HandleRef path) => GdipClosePathFigure_ptr.Delegate(path);

            private delegate int GdipClosePathFigures_delegate(HandleRef path);
            private static FunctionWrapper<GdipClosePathFigures_delegate> GdipClosePathFigures_ptr;
            internal static int GdipClosePathFigures(HandleRef path) => GdipClosePathFigures_ptr.Delegate(path);

            private delegate int GdipSetPathMarker_delegate(HandleRef path);
            private static FunctionWrapper<GdipSetPathMarker_delegate> GdipSetPathMarker_ptr;
            internal static int GdipSetPathMarker(HandleRef path) => GdipSetPathMarker_ptr.Delegate(path);

            private delegate int GdipClearPathMarkers_delegate(HandleRef path);
            private static FunctionWrapper<GdipClearPathMarkers_delegate> GdipClearPathMarkers_ptr;
            internal static int GdipClearPathMarkers(HandleRef path) => GdipClearPathMarkers_ptr.Delegate(path);

            private delegate int GdipReversePath_delegate(HandleRef path);
            private static FunctionWrapper<GdipReversePath_delegate> GdipReversePath_ptr;
            internal static int GdipReversePath(HandleRef path) => GdipReversePath_ptr.Delegate(path);

            private delegate int GdipGetPathLastPoint_delegate(HandleRef path, out PointF lastPoint);
            private static FunctionWrapper<GdipGetPathLastPoint_delegate> GdipGetPathLastPoint_ptr;
            internal static int GdipGetPathLastPoint(HandleRef path, out PointF lastPoint) => GdipGetPathLastPoint_ptr.Delegate(path, out lastPoint);

            private delegate int GdipAddPathLine_delegate(HandleRef path, float x1, float y1, float x2, float y2);
            private static FunctionWrapper<GdipAddPathLine_delegate> GdipAddPathLine_ptr;
            internal static int GdipAddPathLine(HandleRef path, float x1, float y1, float x2, float y2) => GdipAddPathLine_ptr.Delegate(path, x1, y1, x2, y2);

            private delegate int GdipAddPathLine2_delegate(HandleRef path, PointF* points, int count);
            private static FunctionWrapper<GdipAddPathLine2_delegate> GdipAddPathLine2_ptr;
            internal static int GdipAddPathLine2(HandleRef path, PointF* points, int count) => GdipAddPathLine2_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathArc_delegate(HandleRef path, float x, float y, float width, float height, float startAngle, float sweepAngle);
            private static FunctionWrapper<GdipAddPathArc_delegate> GdipAddPathArc_ptr;
            internal static int GdipAddPathArc(HandleRef path, float x, float y, float width, float height, float startAngle, float sweepAngle) => GdipAddPathArc_ptr.Delegate(path, x, y, width, height, startAngle, sweepAngle);

            private delegate int GdipAddPathBezier_delegate(HandleRef path, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);
            private static FunctionWrapper<GdipAddPathBezier_delegate> GdipAddPathBezier_ptr;
            internal static int GdipAddPathBezier(HandleRef path, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) => GdipAddPathBezier_ptr.Delegate(path, x1, y1, x2, y2, x3, y3, x4, y4);

            private delegate int GdipAddPathBeziers_delegate(HandleRef path, PointF* points, int count);
            private static FunctionWrapper<GdipAddPathBeziers_delegate> GdipAddPathBeziers_ptr;
            internal static int GdipAddPathBeziers(HandleRef path, PointF* points, int count) => GdipAddPathBeziers_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathCurve_delegate(HandleRef path, PointF* points, int count);
            private static FunctionWrapper<GdipAddPathCurve_delegate> GdipAddPathCurve_ptr;
            internal static int GdipAddPathCurve(HandleRef path, PointF* points, int count) => GdipAddPathCurve_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathCurve2_delegate(HandleRef path, PointF* points, int count, float tension);
            private static FunctionWrapper<GdipAddPathCurve2_delegate> GdipAddPathCurve2_ptr;
            internal static int GdipAddPathCurve2(HandleRef path, PointF* points, int count, float tension) => GdipAddPathCurve2_ptr.Delegate(path, points, count, tension);

            private delegate int GdipAddPathCurve3_delegate(HandleRef path, PointF* points, int count, int offset, int numberOfSegments, float tension);
            private static FunctionWrapper<GdipAddPathCurve3_delegate> GdipAddPathCurve3_ptr;
            internal static int GdipAddPathCurve3(HandleRef path, PointF* points, int count, int offset, int numberOfSegments, float tension) => GdipAddPathCurve3_ptr.Delegate(path, points, count, offset, numberOfSegments, tension);

            private delegate int GdipAddPathClosedCurve_delegate(HandleRef path, PointF* points, int count);
            private static FunctionWrapper<GdipAddPathClosedCurve_delegate> GdipAddPathClosedCurve_ptr;
            internal static int GdipAddPathClosedCurve(HandleRef path, PointF* points, int count) => GdipAddPathClosedCurve_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathClosedCurve2_delegate(HandleRef path, PointF* points, int count, float tension);
            private static FunctionWrapper<GdipAddPathClosedCurve2_delegate> GdipAddPathClosedCurve2_ptr;
            internal static int GdipAddPathClosedCurve2(HandleRef path, PointF* points, int count, float tension) => GdipAddPathClosedCurve2_ptr.Delegate(path, points, count, tension);

            private delegate int GdipAddPathRectangle_delegate(HandleRef path, float x, float y, float width, float height);
            private static FunctionWrapper<GdipAddPathRectangle_delegate> GdipAddPathRectangle_ptr;
            internal static int GdipAddPathRectangle(HandleRef path, float x, float y, float width, float height) => GdipAddPathRectangle_ptr.Delegate(path, x, y, width, height);

            private delegate int GdipAddPathRectangles_delegate(HandleRef path, RectangleF* rects, int count);
            private static FunctionWrapper<GdipAddPathRectangles_delegate> GdipAddPathRectangles_ptr;
            internal static int GdipAddPathRectangles(HandleRef path, RectangleF* rects, int count) => GdipAddPathRectangles_ptr.Delegate(path, rects, count);

            private delegate int GdipAddPathEllipse_delegate(HandleRef path, float x, float y, float width, float height);
            private static FunctionWrapper<GdipAddPathEllipse_delegate> GdipAddPathEllipse_ptr;
            internal static int GdipAddPathEllipse(HandleRef path, float x, float y, float width, float height) => GdipAddPathEllipse_ptr.Delegate(path, x, y, width, height);

            private delegate int GdipAddPathPie_delegate(HandleRef path, float x, float y, float width, float height, float startAngle, float sweepAngle);
            private static FunctionWrapper<GdipAddPathPie_delegate> GdipAddPathPie_ptr;
            internal static int GdipAddPathPie(HandleRef path, float x, float y, float width, float height, float startAngle, float sweepAngle) => GdipAddPathPie_ptr.Delegate(path, x, y, width, height, startAngle, sweepAngle);

            private delegate int GdipAddPathPolygon_delegate(HandleRef path, PointF* points, int count);
            private static FunctionWrapper<GdipAddPathPolygon_delegate> GdipAddPathPolygon_ptr;
            internal static int GdipAddPathPolygon(HandleRef path, PointF* points, int count) => GdipAddPathPolygon_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathPath_delegate(HandleRef path, HandleRef addingPath, bool connect);
            private static FunctionWrapper<GdipAddPathPath_delegate> GdipAddPathPath_ptr;
            internal static int GdipAddPathPath(HandleRef path, HandleRef addingPath, bool connect) => GdipAddPathPath_ptr.Delegate(path, addingPath, connect);

            private delegate int GdipAddPathString_delegate(HandleRef path, [MarshalAs(UnmanagedType.LPWStr)]string s, int length, HandleRef fontFamily, int style, float emSize, ref RectangleF layoutRect, HandleRef format);
            private static FunctionWrapper<GdipAddPathString_delegate> GdipAddPathString_ptr;
            internal static int GdipAddPathString(HandleRef path, string s, int length, HandleRef fontFamily, int style, float emSize, ref RectangleF layoutRect, HandleRef format) => GdipAddPathString_ptr.Delegate(path, s, length, fontFamily, style, emSize, ref layoutRect, format);

            private delegate int GdipAddPathStringI_delegate(HandleRef path, [MarshalAs(UnmanagedType.LPWStr)]string s, int length, HandleRef fontFamily, int style, float emSize, ref Rectangle layoutRect, HandleRef format);
            private static FunctionWrapper<GdipAddPathStringI_delegate> GdipAddPathStringI_ptr;
            internal static int GdipAddPathStringI(HandleRef path, string s, int length, HandleRef fontFamily, int style, float emSize, ref Rectangle layoutRect, HandleRef format) => GdipAddPathStringI_ptr.Delegate(path, s, length, fontFamily, style, emSize, ref layoutRect, format);

            private delegate int GdipAddPathLineI_delegate(HandleRef path, int x1, int y1, int x2, int y2);
            private static FunctionWrapper<GdipAddPathLineI_delegate> GdipAddPathLineI_ptr;
            internal static int GdipAddPathLineI(HandleRef path, int x1, int y1, int x2, int y2) => GdipAddPathLineI_ptr.Delegate(path, x1, y1, x2, y2);

            private delegate int GdipAddPathLine2I_delegate(HandleRef path, Point* points, int count);
            private static FunctionWrapper<GdipAddPathLine2I_delegate> GdipAddPathLine2I_ptr;
            internal static int GdipAddPathLine2I(HandleRef path, Point* points, int count) => GdipAddPathLine2I_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathArcI_delegate(HandleRef path, int x, int y, int width, int height, float startAngle, float sweepAngle);
            private static FunctionWrapper<GdipAddPathArcI_delegate> GdipAddPathArcI_ptr;
            internal static int GdipAddPathArcI(HandleRef path, int x, int y, int width, int height, float startAngle, float sweepAngle) => GdipAddPathArcI_ptr.Delegate(path, x, y, width, height, startAngle, sweepAngle);

            private delegate int GdipAddPathBezierI_delegate(HandleRef path, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);
            private static FunctionWrapper<GdipAddPathBezierI_delegate> GdipAddPathBezierI_ptr;
            internal static int GdipAddPathBezierI(HandleRef path, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) => GdipAddPathBezierI_ptr.Delegate(path, x1, y1, x2, y2, x3, y3, x4, y4);

            private delegate int GdipAddPathBeziersI_delegate(HandleRef path, Point* points, int count);
            private static FunctionWrapper<GdipAddPathBeziersI_delegate> GdipAddPathBeziersI_ptr;
            internal static int GdipAddPathBeziersI(HandleRef path, Point* points, int count) => GdipAddPathBeziersI_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathCurveI_delegate(HandleRef path, Point* points, int count);
            private static FunctionWrapper<GdipAddPathCurveI_delegate> GdipAddPathCurveI_ptr;
            internal static int GdipAddPathCurveI(HandleRef path, Point* points, int count) => GdipAddPathCurveI_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathCurve2I_delegate(HandleRef path, Point* points, int count, float tension);
            private static FunctionWrapper<GdipAddPathCurve2I_delegate> GdipAddPathCurve2I_ptr;
            internal static int GdipAddPathCurve2I(HandleRef path, Point* points, int count, float tension) => GdipAddPathCurve2I_ptr.Delegate(path, points, count, tension);

            private delegate int GdipAddPathCurve3I_delegate(HandleRef path, Point* points, int count, int offset, int numberOfSegments, float tension);
            private static FunctionWrapper<GdipAddPathCurve3I_delegate> GdipAddPathCurve3I_ptr;
            internal static int GdipAddPathCurve3I(HandleRef path, Point* points, int count, int offset, int numberOfSegments, float tension) => GdipAddPathCurve3I_ptr.Delegate(path, points, count, offset, numberOfSegments, tension);

            private delegate int GdipAddPathClosedCurveI_delegate(HandleRef path, Point* points, int count);
            private static FunctionWrapper<GdipAddPathClosedCurveI_delegate> GdipAddPathClosedCurveI_ptr;
            internal static int GdipAddPathClosedCurveI(HandleRef path, Point* points, int count) => GdipAddPathClosedCurveI_ptr.Delegate(path, points, count);

            private delegate int GdipAddPathClosedCurve2I_delegate(HandleRef path, Point* points, int count, float tension);
            private static FunctionWrapper<GdipAddPathClosedCurve2I_delegate> GdipAddPathClosedCurve2I_ptr;
            internal static int GdipAddPathClosedCurve2I(HandleRef path, Point* points, int count, float tension) => GdipAddPathClosedCurve2I_ptr.Delegate(path, points, count, tension);

            private delegate int GdipAddPathRectangleI_delegate(HandleRef path, int x, int y, int width, int height);
            private static FunctionWrapper<GdipAddPathRectangleI_delegate> GdipAddPathRectangleI_ptr;
            internal static int GdipAddPathRectangleI(HandleRef path, int x, int y, int width, int height) => GdipAddPathRectangleI_ptr.Delegate(path, x, y, width, height);

            private delegate int GdipAddPathRectanglesI_delegate(HandleRef path, Rectangle* rects, int count);
            private static FunctionWrapper<GdipAddPathRectanglesI_delegate> GdipAddPathRectanglesI_ptr;
            internal static int GdipAddPathRectanglesI(HandleRef path, Rectangle* rects, int count) => GdipAddPathRectanglesI_ptr.Delegate(path, rects, count);

            private delegate int GdipAddPathEllipseI_delegate(HandleRef path, int x, int y, int width, int height);
            private static FunctionWrapper<GdipAddPathEllipseI_delegate> GdipAddPathEllipseI_ptr;
            internal static int GdipAddPathEllipseI(HandleRef path, int x, int y, int width, int height) => GdipAddPathEllipseI_ptr.Delegate(path, x, y, width, height);

            private delegate int GdipAddPathPieI_delegate(HandleRef path, int x, int y, int width, int height, float startAngle, float sweepAngle);
            private static FunctionWrapper<GdipAddPathPieI_delegate> GdipAddPathPieI_ptr;
            internal static int GdipAddPathPieI(HandleRef path, int x, int y, int width, int height, float startAngle, float sweepAngle) => GdipAddPathPieI_ptr.Delegate(path, x, y, width, height, startAngle, sweepAngle);

            private delegate int GdipAddPathPolygonI_delegate(HandleRef path, Point* points, int count);
            private static FunctionWrapper<GdipAddPathPolygonI_delegate> GdipAddPathPolygonI_ptr;
            internal static int GdipAddPathPolygonI(HandleRef path, Point* points, int count) => GdipAddPathPolygonI_ptr.Delegate(path, points, count);

            private delegate int GdipFlattenPath_delegate(HandleRef path, HandleRef matrixfloat, float flatness);
            private static FunctionWrapper<GdipFlattenPath_delegate> GdipFlattenPath_ptr;
            internal static int GdipFlattenPath(HandleRef path, HandleRef matrixfloat, float flatness) => GdipFlattenPath_ptr.Delegate(path, matrixfloat, flatness);

            private delegate int GdipWidenPath_delegate(HandleRef path, HandleRef pen, HandleRef matrix, float flatness);
            private static FunctionWrapper<GdipWidenPath_delegate> GdipWidenPath_ptr;
            internal static int GdipWidenPath(HandleRef path, HandleRef pen, HandleRef matrix, float flatness) => GdipWidenPath_ptr.Delegate(path, pen, matrix, flatness);

            private delegate int GdipWarpPath_delegate(HandleRef path, HandleRef matrix, PointF* points, int count, float srcX, float srcY, float srcWidth, float srcHeight, WarpMode warpMode, float flatness);
            private static FunctionWrapper<GdipWarpPath_delegate> GdipWarpPath_ptr;
            internal static int GdipWarpPath(HandleRef path, HandleRef matrix, PointF* points, int count, float srcX, float srcY, float srcWidth, float srcHeight, WarpMode warpMode, float flatness) => GdipWarpPath_ptr.Delegate(path, matrix, points, count, srcX, srcY, srcWidth, srcHeight, warpMode, flatness);

            private delegate int GdipTransformPath_delegate(HandleRef path, HandleRef matrix);
            private static FunctionWrapper<GdipTransformPath_delegate> GdipTransformPath_ptr;
            internal static int GdipTransformPath(HandleRef path, HandleRef matrix) => GdipTransformPath_ptr.Delegate(path, matrix);

            private delegate int GdipGetPathWorldBounds_delegate(HandleRef path, out RectangleF gprectf, HandleRef matrix, HandleRef pen);
            private static FunctionWrapper<GdipGetPathWorldBounds_delegate> GdipGetPathWorldBounds_ptr;
            internal static int GdipGetPathWorldBounds(HandleRef path, out RectangleF gprectf, HandleRef matrix, HandleRef pen) => GdipGetPathWorldBounds_ptr.Delegate(path, out gprectf, matrix, pen);

            private delegate int GdipIsVisiblePathPoint_delegate(HandleRef path, float x, float y, HandleRef graphics, out bool result);
            private static FunctionWrapper<GdipIsVisiblePathPoint_delegate> GdipIsVisiblePathPoint_ptr;
            internal static int GdipIsVisiblePathPoint(HandleRef path, float x, float y, HandleRef graphics, out bool result) => GdipIsVisiblePathPoint_ptr.Delegate(path, x, y, graphics, out result);

            private delegate int GdipIsVisiblePathPointI_delegate(HandleRef path, int x, int y, HandleRef graphics, out bool result);
            private static FunctionWrapper<GdipIsVisiblePathPointI_delegate> GdipIsVisiblePathPointI_ptr;
            internal static int GdipIsVisiblePathPointI(HandleRef path, int x, int y, HandleRef graphics, out bool result) => GdipIsVisiblePathPointI_ptr.Delegate(path, x, y, graphics, out result);

            private delegate int GdipIsOutlineVisiblePathPoint_delegate(HandleRef path, float x, float y, HandleRef pen, HandleRef graphics, out bool result);
            private static FunctionWrapper<GdipIsOutlineVisiblePathPoint_delegate> GdipIsOutlineVisiblePathPoint_ptr;
            internal static int GdipIsOutlineVisiblePathPoint(HandleRef path, float x, float y, HandleRef pen, HandleRef graphics, out bool result) => GdipIsOutlineVisiblePathPoint_ptr.Delegate(path, x, y, pen, graphics, out result);

            private delegate int GdipIsOutlineVisiblePathPointI_delegate(HandleRef path, int x, int y, HandleRef pen, HandleRef graphics, out bool result);
            private static FunctionWrapper<GdipIsOutlineVisiblePathPointI_delegate> GdipIsOutlineVisiblePathPointI_ptr;
            internal static int GdipIsOutlineVisiblePathPointI(HandleRef path, int x, int y, HandleRef pen, HandleRef graphics, out bool result) => GdipIsOutlineVisiblePathPointI_ptr.Delegate(path, x, y, pen, graphics, out result);

            private delegate int GdipDeleteBrush_delegate(HandleRef brush);
            private static FunctionWrapper<GdipDeleteBrush_delegate> GdipDeleteBrush_ptr;
            internal static int IntGdipDeleteBrush(HandleRef brush) => GdipDeleteBrush_ptr.Delegate(brush);

            private delegate int GdipLoadImageFromStream_delegate(Interop.Ole32.IStream stream, out IntPtr image);
            private static FunctionWrapper<GdipLoadImageFromStream_delegate> GdipLoadImageFromStream_ptr;
            internal static int GdipLoadImageFromStream(Interop.Ole32.IStream stream, out IntPtr image) => GdipLoadImageFromStream_ptr.Delegate(stream, out image);

            private delegate int GdipLoadImageFromFile_delegate([MarshalAs(UnmanagedType.LPWStr)]string filename, out IntPtr image);
            private static FunctionWrapper<GdipLoadImageFromFile_delegate> GdipLoadImageFromFile_ptr;
            internal static int GdipLoadImageFromFile(string filename, out IntPtr image) => GdipLoadImageFromFile_ptr.Delegate(filename, out image);

            private delegate int GdipLoadImageFromStreamICM_delegate(Interop.Ole32.IStream stream, out IntPtr image);
            private static FunctionWrapper<GdipLoadImageFromStreamICM_delegate> GdipLoadImageFromStreamICM_ptr;
            internal static int GdipLoadImageFromStreamICM(Interop.Ole32.IStream stream, out IntPtr image) => GdipLoadImageFromStreamICM_ptr.Delegate(stream, out image);

            private delegate int GdipLoadImageFromFileICM_delegate([MarshalAs(UnmanagedType.LPWStr)]string filename, out IntPtr image);
            private static FunctionWrapper<GdipLoadImageFromFileICM_delegate> GdipLoadImageFromFileICM_ptr;
            internal static int GdipLoadImageFromFileICM(string filename, out IntPtr image) => GdipLoadImageFromFileICM_ptr.Delegate(filename, out image);

            private delegate int GdipCloneImage_delegate(HandleRef image, out IntPtr cloneimage);
            private static FunctionWrapper<GdipCloneImage_delegate> GdipCloneImage_ptr;
            internal static int GdipCloneImage(HandleRef image, out IntPtr cloneimage) => GdipCloneImage_ptr.Delegate(image, out cloneimage);

            private delegate int GdipDisposeImage_delegate(HandleRef image);
            private static FunctionWrapper<GdipDisposeImage_delegate> GdipDisposeImage_ptr;
            internal static int IntGdipDisposeImage(HandleRef image) => GdipDisposeImage_ptr.Delegate(image);

            private delegate int GdipSaveImageToFile_delegate(HandleRef image, [MarshalAs(UnmanagedType.LPWStr)] string filename, ref Guid classId, HandleRef encoderParams);
            private static FunctionWrapper<GdipSaveImageToFile_delegate> GdipSaveImageToFile_ptr;
            internal static int GdipSaveImageToFile(HandleRef image, string filename, ref Guid classId, HandleRef encoderParams) => GdipSaveImageToFile_ptr.Delegate(image, filename, ref classId, encoderParams);

            private delegate int GdipSaveImageToStream_delegate(HandleRef image, Interop.Ole32.IStream stream, ref Guid classId, HandleRef encoderParams);
            private static FunctionWrapper<GdipSaveImageToStream_delegate> GdipSaveImageToStream_ptr;
            internal static int GdipSaveImageToStream(HandleRef image, Interop.Ole32.IStream stream, ref Guid classId, HandleRef encoderParams) => GdipSaveImageToStream_ptr.Delegate(image, stream, ref classId, encoderParams);

            private delegate int GdipSaveAdd_delegate(HandleRef image, HandleRef encoderParams);
            private static FunctionWrapper<GdipSaveAdd_delegate> GdipSaveAdd_ptr;
            internal static int GdipSaveAdd(HandleRef image, HandleRef encoderParams) => GdipSaveAdd_ptr.Delegate(image, encoderParams);

            private delegate int GdipSaveAddImage_delegate(HandleRef image, HandleRef newImage, HandleRef encoderParams);
            private static FunctionWrapper<GdipSaveAddImage_delegate> GdipSaveAddImage_ptr;
            internal static int GdipSaveAddImage(HandleRef image, HandleRef newImage, HandleRef encoderParams) => GdipSaveAddImage_ptr.Delegate(image, newImage, encoderParams);

            private delegate int GdipGetImageGraphicsContext_delegate(HandleRef image, out IntPtr graphics);
            private static FunctionWrapper<GdipGetImageGraphicsContext_delegate> GdipGetImageGraphicsContext_ptr;
            internal static int GdipGetImageGraphicsContext(HandleRef image, out IntPtr graphics) => GdipGetImageGraphicsContext_ptr.Delegate(image, out graphics);

            private delegate int GdipGetImageBounds_delegate(HandleRef image, out RectangleF gprectf, out GraphicsUnit unit);
            private static FunctionWrapper<GdipGetImageBounds_delegate> GdipGetImageBounds_ptr;
            internal static int GdipGetImageBounds(HandleRef image, out RectangleF gprectf, out GraphicsUnit unit) => GdipGetImageBounds_ptr.Delegate(image, out gprectf, out unit);

            private delegate int GdipGetImageType_delegate(HandleRef image, out int type);
            private static FunctionWrapper<GdipGetImageType_delegate> GdipGetImageType_ptr;
            internal static int GdipGetImageType(HandleRef image, out int type) => GdipGetImageType_ptr.Delegate(image, out type);

            private delegate int GdipGetImageThumbnail_delegate(HandleRef image, int thumbWidth, int thumbHeight, out IntPtr thumbImage, Image.GetThumbnailImageAbort callback, IntPtr callbackdata);
            private static FunctionWrapper<GdipGetImageThumbnail_delegate> GdipGetImageThumbnail_ptr;
            internal static int GdipGetImageThumbnail(HandleRef image, int thumbWidth, int thumbHeight, out IntPtr thumbImage, Image.GetThumbnailImageAbort callback, IntPtr callbackdata) => GdipGetImageThumbnail_ptr.Delegate(image, thumbWidth, thumbHeight, out thumbImage, callback, callbackdata);

            private delegate int GdipGetEncoderParameterListSize_delegate(HandleRef image, ref Guid clsid, out int size);
            private static FunctionWrapper<GdipGetEncoderParameterListSize_delegate> GdipGetEncoderParameterListSize_ptr;
            internal static int GdipGetEncoderParameterListSize(HandleRef image, ref Guid clsid, out int size) => GdipGetEncoderParameterListSize_ptr.Delegate(image, ref clsid, out size);

            private delegate int GdipGetEncoderParameterList_delegate(HandleRef image, ref Guid clsid, int size, IntPtr buffer);
            private static FunctionWrapper<GdipGetEncoderParameterList_delegate> GdipGetEncoderParameterList_ptr;
            internal static int GdipGetEncoderParameterList(HandleRef image, ref Guid clsid, int size, IntPtr buffer) => GdipGetEncoderParameterList_ptr.Delegate(image, ref clsid, size, buffer);

            private delegate int GdipGetImagePalette_delegate(HandleRef image, IntPtr palette, int size);
            private static FunctionWrapper<GdipGetImagePalette_delegate> GdipGetImagePalette_ptr;
            internal static int GdipGetImagePalette(HandleRef image, IntPtr palette, int size) => GdipGetImagePalette_ptr.Delegate(image, palette, size);

            private delegate int GdipSetImagePalette_delegate(HandleRef image, IntPtr palette);
            private static FunctionWrapper<GdipSetImagePalette_delegate> GdipSetImagePalette_ptr;
            internal static int GdipSetImagePalette(HandleRef image, IntPtr palette) => GdipSetImagePalette_ptr.Delegate(image, palette);

            private delegate int GdipGetImagePaletteSize_delegate(HandleRef image, out int size);
            private static FunctionWrapper<GdipGetImagePaletteSize_delegate> GdipGetImagePaletteSize_ptr;
            internal static int GdipGetImagePaletteSize(HandleRef image, out int size) => GdipGetImagePaletteSize_ptr.Delegate(image, out size);

            private delegate int GdipGetPropertyCount_delegate(HandleRef image, out int count);
            private static FunctionWrapper<GdipGetPropertyCount_delegate> GdipGetPropertyCount_ptr;
            internal static int GdipGetPropertyCount(HandleRef image, out int count) => GdipGetPropertyCount_ptr.Delegate(image, out count);

            private delegate int GdipGetPropertyIdList_delegate(HandleRef image, int count, int[] list);
            private static FunctionWrapper<GdipGetPropertyIdList_delegate> GdipGetPropertyIdList_ptr;
            internal static int GdipGetPropertyIdList(HandleRef image, int count, int[] list) => GdipGetPropertyIdList_ptr.Delegate(image, count, list);

            private delegate int GdipGetPropertyItemSize_delegate(HandleRef image, int propid, out int size);
            private static FunctionWrapper<GdipGetPropertyItemSize_delegate> GdipGetPropertyItemSize_ptr;
            internal static int GdipGetPropertyItemSize(HandleRef image, int propid, out int size) => GdipGetPropertyItemSize_ptr.Delegate(image, propid, out size);

            private delegate int GdipGetPropertyItem_delegate(HandleRef image, int propid, int size, IntPtr buffer);
            private static FunctionWrapper<GdipGetPropertyItem_delegate> GdipGetPropertyItem_ptr;
            internal static int GdipGetPropertyItem(HandleRef image, int propid, int size, IntPtr buffer) => GdipGetPropertyItem_ptr.Delegate(image, propid, size, buffer);

            private delegate int GdipGetPropertySize_delegate(HandleRef image, out int totalSize, ref int count);
            private static FunctionWrapper<GdipGetPropertySize_delegate> GdipGetPropertySize_ptr;
            internal static int GdipGetPropertySize(HandleRef image, out int totalSize, ref int count) => GdipGetPropertySize_ptr.Delegate(image, out totalSize, ref count);

            private delegate int GdipGetAllPropertyItems_delegate(HandleRef image, int totalSize, int count, IntPtr buffer);
            private static FunctionWrapper<GdipGetAllPropertyItems_delegate> GdipGetAllPropertyItems_ptr;
            internal static int GdipGetAllPropertyItems(HandleRef image, int totalSize, int count, IntPtr buffer) => GdipGetAllPropertyItems_ptr.Delegate(image, totalSize, count, buffer);

            private delegate int GdipSetPropertyItem_delegate(HandleRef image, PropertyItemInternal propitem);
            private static FunctionWrapper<GdipSetPropertyItem_delegate> GdipSetPropertyItem_ptr;
            internal static int GdipSetPropertyItem(HandleRef image, PropertyItemInternal propitem) => GdipSetPropertyItem_ptr.Delegate(image, propitem);

            private delegate int GdipImageForceValidation_delegate(HandleRef image);
            private static FunctionWrapper<GdipImageForceValidation_delegate> GdipImageForceValidation_ptr;
            internal static int GdipImageForceValidation(HandleRef image) => GdipImageForceValidation_ptr.Delegate(image);

            private delegate int GdipCreateFromHDC_delegate(HandleRef hdc, out IntPtr graphics);
            private static FunctionWrapper<GdipCreateFromHDC_delegate> GdipCreateFromHDC_ptr;
            internal static int GdipCreateFromHDC(HandleRef hdc, out IntPtr graphics) => GdipCreateFromHDC_ptr.Delegate(hdc, out graphics);

            private delegate int GdipCreateFromHDC2_delegate(HandleRef hdc, HandleRef hdevice, out IntPtr graphics);
            private static FunctionWrapper<GdipCreateFromHDC2_delegate> GdipCreateFromHDC2_ptr;
            internal static int GdipCreateFromHDC2(HandleRef hdc, HandleRef hdevice, out IntPtr graphics) => GdipCreateFromHDC2_ptr.Delegate(hdc, hdevice, out graphics);

            private delegate int GdipCreateFromHWND_delegate(HandleRef hwnd, out IntPtr graphics);
            private static FunctionWrapper<GdipCreateFromHWND_delegate> GdipCreateFromHWND_ptr;
            internal static int GdipCreateFromHWND(HandleRef hwnd, out IntPtr graphics) => GdipCreateFromHWND_ptr.Delegate(hwnd, out graphics);

            private delegate int GdipDeleteGraphics_delegate(HandleRef graphics);
            private static FunctionWrapper<GdipDeleteGraphics_delegate> GdipDeleteGraphics_ptr;
            internal static int IntGdipDeleteGraphics(HandleRef graphics) => GdipDeleteGraphics_ptr.Delegate(graphics);

            private delegate int GdipReleaseDC_delegate(HandleRef graphics, HandleRef hdc);
            private static FunctionWrapper<GdipReleaseDC_delegate> GdipReleaseDC_ptr;
            internal static int IntGdipReleaseDC(HandleRef graphics, HandleRef hdc) => GdipReleaseDC_ptr.Delegate(graphics, hdc);

            private delegate int GdipTransformPoints_delegate(HandleRef graphics, int destSpace, int srcSpace, PointF* points, int count);
            private static FunctionWrapper<GdipTransformPoints_delegate> GdipTransformPoints_ptr;
            internal static int GdipTransformPoints(HandleRef graphics, int destSpace, int srcSpace, PointF* points, int count) => GdipTransformPoints_ptr.Delegate(graphics, destSpace, srcSpace, points, count);

            private delegate int GdipTransformPointsI_delegate(HandleRef graphics, int destSpace, int srcSpace, Point* points, int count);
            private static FunctionWrapper<GdipTransformPointsI_delegate> GdipTransformPointsI_ptr;
            internal static int GdipTransformPointsI(HandleRef graphics, int destSpace, int srcSpace, Point* points, int count) => GdipTransformPointsI_ptr.Delegate(graphics, destSpace, srcSpace, points, count);

            private delegate int GdipGetNearestColor_delegate(HandleRef graphics, ref int color);
            private static FunctionWrapper<GdipGetNearestColor_delegate> GdipGetNearestColor_ptr;
            internal static int GdipGetNearestColor(HandleRef graphics, ref int color) => GdipGetNearestColor_ptr.Delegate(graphics, ref color);

            private delegate IntPtr GdipCreateHalftonePalette_delegate();
            private static FunctionWrapper<GdipCreateHalftonePalette_delegate> GdipCreateHalftonePalette_ptr;
            internal static IntPtr GdipCreateHalftonePalette() => GdipCreateHalftonePalette_ptr.Delegate();

            private delegate int GdipDrawBeziers_delegate(HandleRef graphics, HandleRef pen, PointF* points, int count);
            private static FunctionWrapper<GdipDrawBeziers_delegate> GdipDrawBeziers_ptr;
            internal static int GdipDrawBeziers(HandleRef graphics, HandleRef pen, PointF* points, int count) => GdipDrawBeziers_ptr.Delegate(graphics, pen, points, count);

            private delegate int GdipDrawBeziersI_delegate(HandleRef graphics, HandleRef pen, Point* points, int count);
            private static FunctionWrapper<GdipDrawBeziersI_delegate> GdipDrawBeziersI_ptr;
            internal static int GdipDrawBeziersI(HandleRef graphics, HandleRef pen, Point* points, int count) => GdipDrawBeziersI_ptr.Delegate(graphics, pen, points, count);

            private delegate int GdipFillPath_delegate(HandleRef graphics, HandleRef brush, HandleRef path);
            private static FunctionWrapper<GdipFillPath_delegate> GdipFillPath_ptr;
            internal static int GdipFillPath(HandleRef graphics, HandleRef brush, HandleRef path) => GdipFillPath_ptr.Delegate(graphics, brush, path);
            private delegate int GdipEnumerateMetafileDestPoint_delegate(HandleRef graphics, HandleRef metafile, ref PointF destPoint, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileDestPoint_delegate> GdipEnumerateMetafileDestPoint_ptr;
            internal static int GdipEnumerateMetafileDestPoint(HandleRef graphics, HandleRef metafile, ref PointF destPoint, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileDestPoint_ptr.Delegate(graphics, metafile, ref destPoint, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileDestPointI_delegate(HandleRef graphics, HandleRef metafile, ref Point destPoint, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileDestPointI_delegate> GdipEnumerateMetafileDestPointI_ptr;
            internal static int GdipEnumerateMetafileDestPointI(HandleRef graphics, HandleRef metafile, ref Point destPoint, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileDestPointI_ptr.Delegate(graphics, metafile, ref destPoint, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileDestRect_delegate(HandleRef graphics, HandleRef metafile, ref RectangleF destRect, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileDestRect_delegate> GdipEnumerateMetafileDestRect_ptr;
            internal static int GdipEnumerateMetafileDestRect(HandleRef graphics, HandleRef metafile, ref RectangleF destRect, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileDestRect_ptr.Delegate(graphics, metafile, ref destRect, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileDestRectI_delegate(HandleRef graphics, HandleRef metafile, ref Rectangle destRect, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileDestRectI_delegate> GdipEnumerateMetafileDestRectI_ptr;
            internal static int GdipEnumerateMetafileDestRectI(HandleRef graphics, HandleRef metafile, ref Rectangle destRect, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileDestRectI_ptr.Delegate(graphics, metafile, ref destRect, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileDestPoints_delegate(HandleRef graphics, HandleRef metafile, PointF* destPoints, int count, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileDestPoints_delegate> GdipEnumerateMetafileDestPoints_ptr;
            internal static int GdipEnumerateMetafileDestPoints(HandleRef graphics, HandleRef metafile, PointF* destPoints, int count, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileDestPoints_ptr.Delegate(graphics, metafile, destPoints, count, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileDestPointsI_delegate(HandleRef graphics, HandleRef metafile, Point* destPoints, int count, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileDestPointsI_delegate> GdipEnumerateMetafileDestPointsI_ptr;
            internal static int GdipEnumerateMetafileDestPointsI(HandleRef graphics, HandleRef metafile, Point* destPoints, int count, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileDestPointsI_ptr.Delegate(graphics, metafile, destPoints, count, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileSrcRectDestPoint_delegate(HandleRef graphics, HandleRef metafile, ref PointF destPoint, ref RectangleF srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileSrcRectDestPoint_delegate> GdipEnumerateMetafileSrcRectDestPoint_ptr;
            internal static int GdipEnumerateMetafileSrcRectDestPoint(HandleRef graphics, HandleRef metafile, ref PointF destPoint, ref RectangleF srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileSrcRectDestPoint_ptr.Delegate(graphics, metafile, ref destPoint, ref srcRect, pageUnit, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileSrcRectDestPointI_delegate(HandleRef graphics, HandleRef metafile, ref Point destPoint, ref Rectangle srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileSrcRectDestPointI_delegate> GdipEnumerateMetafileSrcRectDestPointI_ptr;
            internal static int GdipEnumerateMetafileSrcRectDestPointI(HandleRef graphics, HandleRef metafile, ref Point destPoint, ref Rectangle srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileSrcRectDestPointI_ptr.Delegate(graphics, metafile, ref destPoint, ref srcRect, pageUnit, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileSrcRectDestRect_delegate(HandleRef graphics, HandleRef metafile, ref RectangleF destRect, ref RectangleF srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileSrcRectDestRect_delegate> GdipEnumerateMetafileSrcRectDestRect_ptr;
            internal static int GdipEnumerateMetafileSrcRectDestRect(HandleRef graphics, HandleRef metafile, ref RectangleF destRect, ref RectangleF srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileSrcRectDestRect_ptr.Delegate(graphics, metafile, ref destRect, ref srcRect, pageUnit, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileSrcRectDestRectI_delegate(HandleRef graphics, HandleRef metafile, ref Rectangle destRect, ref Rectangle srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileSrcRectDestRectI_delegate> GdipEnumerateMetafileSrcRectDestRectI_ptr;
            internal static int GdipEnumerateMetafileSrcRectDestRectI(HandleRef graphics, HandleRef metafile, ref Rectangle destRect, ref Rectangle srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileSrcRectDestRectI_ptr.Delegate(graphics, metafile, ref destRect, ref srcRect, pageUnit, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileSrcRectDestPoints_delegate(HandleRef graphics, HandleRef metafile, PointF* destPoints, int count, ref RectangleF srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileSrcRectDestPoints_delegate> GdipEnumerateMetafileSrcRectDestPoints_ptr;
            internal static int GdipEnumerateMetafileSrcRectDestPoints(HandleRef graphics, HandleRef metafile, PointF* destPoints, int count, ref RectangleF srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileSrcRectDestPoints_ptr.Delegate(graphics, metafile, destPoints, count, ref srcRect, pageUnit, callback, callbackdata, imageattributes);

            private delegate int GdipEnumerateMetafileSrcRectDestPointsI_delegate(HandleRef graphics, HandleRef metafile, Point* destPoints, int count, ref Rectangle srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes);
            private static FunctionWrapper<GdipEnumerateMetafileSrcRectDestPointsI_delegate> GdipEnumerateMetafileSrcRectDestPointsI_ptr;
            internal static int GdipEnumerateMetafileSrcRectDestPointsI(HandleRef graphics, HandleRef metafile, Point* destPoints, int count, ref Rectangle srcRect, GraphicsUnit pageUnit, Graphics.EnumerateMetafileProc callback, HandleRef callbackdata, HandleRef imageattributes) => GdipEnumerateMetafileSrcRectDestPointsI_ptr.Delegate(graphics, metafile, destPoints, count, ref srcRect, pageUnit, callback, callbackdata, imageattributes);

            private delegate int GdipPlayMetafileRecord_delegate(HandleRef graphics, EmfPlusRecordType recordType, int flags, int dataSize, byte[] data);
            private static FunctionWrapper<GdipPlayMetafileRecord_delegate> GdipPlayMetafileRecord_ptr;
            internal static int GdipPlayMetafileRecord(HandleRef graphics, EmfPlusRecordType recordType, int flags, int dataSize, byte[] data) => GdipPlayMetafileRecord_ptr.Delegate(graphics, recordType, flags, dataSize, data);

            private delegate int GdipRestoreGraphics_delegate(HandleRef graphics, int state);
            private static FunctionWrapper<GdipRestoreGraphics_delegate> GdipRestoreGraphics_ptr;
            internal static int GdipRestoreGraphics(HandleRef graphics, int state) => GdipRestoreGraphics_ptr.Delegate(graphics, state);

            private delegate int GdipGetMetafileHeaderFromWmf_delegate(HandleRef hMetafile, WmfPlaceableFileHeader wmfplaceable, [In] [Out] MetafileHeaderWmf metafileHeaderWmf);
            private static FunctionWrapper<GdipGetMetafileHeaderFromWmf_delegate> GdipGetMetafileHeaderFromWmf_ptr;
            internal static int GdipGetMetafileHeaderFromWmf(HandleRef hMetafile, WmfPlaceableFileHeader wmfplaceable, [In] [Out] MetafileHeaderWmf metafileHeaderWmf) => GdipGetMetafileHeaderFromWmf_ptr.Delegate(hMetafile, wmfplaceable, metafileHeaderWmf);

            private delegate int GdipGetMetafileHeaderFromEmf_delegate(HandleRef hEnhMetafile, [In] [Out] MetafileHeaderEmf metafileHeaderEmf);
            private static FunctionWrapper<GdipGetMetafileHeaderFromEmf_delegate> GdipGetMetafileHeaderFromEmf_ptr;
            internal static int GdipGetMetafileHeaderFromEmf(HandleRef hEnhMetafile, [In] [Out] MetafileHeaderEmf metafileHeaderEmf) => GdipGetMetafileHeaderFromEmf_ptr.Delegate(hEnhMetafile, metafileHeaderEmf);

            private delegate int GdipGetMetafileHeaderFromFile_delegate([MarshalAs(UnmanagedType.LPWStr)]string filename, IntPtr header);
            private static FunctionWrapper<GdipGetMetafileHeaderFromFile_delegate> GdipGetMetafileHeaderFromFile_ptr;
            internal static int GdipGetMetafileHeaderFromFile(string filename, IntPtr header) => GdipGetMetafileHeaderFromFile_ptr.Delegate(filename, header);

            private delegate int GdipGetMetafileHeaderFromStream_delegate(Interop.Ole32.IStream stream, IntPtr header);
            private static FunctionWrapper<GdipGetMetafileHeaderFromStream_delegate> GdipGetMetafileHeaderFromStream_ptr;
            internal static int GdipGetMetafileHeaderFromStream(Interop.Ole32.IStream stream, IntPtr header) => GdipGetMetafileHeaderFromStream_ptr.Delegate(stream, header);

            private delegate int GdipGetMetafileHeaderFromMetafile_delegate(HandleRef metafile, IntPtr header);
            private static FunctionWrapper<GdipGetMetafileHeaderFromMetafile_delegate> GdipGetMetafileHeaderFromMetafile_ptr;
            internal static int GdipGetMetafileHeaderFromMetafile(HandleRef metafile, IntPtr header) => GdipGetMetafileHeaderFromMetafile_ptr.Delegate(metafile, header);

            private delegate int GdipGetHemfFromMetafile_delegate(HandleRef metafile, out IntPtr hEnhMetafile);
            private static FunctionWrapper<GdipGetHemfFromMetafile_delegate> GdipGetHemfFromMetafile_ptr;
            internal static int GdipGetHemfFromMetafile(HandleRef metafile, out IntPtr hEnhMetafile) => GdipGetHemfFromMetafile_ptr.Delegate(metafile, out hEnhMetafile);

            private delegate int GdipCreateMetafileFromWmf_delegate(HandleRef hMetafile, [MarshalAs(UnmanagedType.Bool)]bool deleteWmf, WmfPlaceableFileHeader wmfplacealbeHeader, out IntPtr metafile);
            private static FunctionWrapper<GdipCreateMetafileFromWmf_delegate> GdipCreateMetafileFromWmf_ptr;
            internal static int GdipCreateMetafileFromWmf(HandleRef hMetafile, bool deleteWmf, WmfPlaceableFileHeader wmfplacealbeHeader, out IntPtr metafile) => GdipCreateMetafileFromWmf_ptr.Delegate(hMetafile, deleteWmf, wmfplacealbeHeader, out metafile);

            private delegate int GdipCreateMetafileFromEmf_delegate(HandleRef hEnhMetafile, bool deleteEmf, out IntPtr metafile);
            private static FunctionWrapper<GdipCreateMetafileFromEmf_delegate> GdipCreateMetafileFromEmf_ptr;
            internal static int GdipCreateMetafileFromEmf(HandleRef hEnhMetafile, bool deleteEmf, out IntPtr metafile) => GdipCreateMetafileFromEmf_ptr.Delegate(hEnhMetafile, deleteEmf, out metafile);

            private delegate int GdipCreateMetafileFromFile_delegate([MarshalAs(UnmanagedType.LPWStr)]string file, out IntPtr metafile);
            private static FunctionWrapper<GdipCreateMetafileFromFile_delegate> GdipCreateMetafileFromFile_ptr;
            internal static int GdipCreateMetafileFromFile(string file, out IntPtr metafile) => GdipCreateMetafileFromFile_ptr.Delegate(file, out metafile);

            private delegate int GdipCreateMetafileFromStream_delegate(Interop.Ole32.IStream stream, out IntPtr metafile);
            private static FunctionWrapper<GdipCreateMetafileFromStream_delegate> GdipCreateMetafileFromStream_ptr;
            internal static int GdipCreateMetafileFromStream(Interop.Ole32.IStream stream, out IntPtr metafile) => GdipCreateMetafileFromStream_ptr.Delegate(stream, out metafile);

            private delegate int GdipRecordMetafile_delegate(HandleRef referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafile_delegate> GdipRecordMetafile_ptr;
            internal static int GdipRecordMetafile(HandleRef referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafile_ptr.Delegate(referenceHdc, emfType, ref frameRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafile2_delegate(HandleRef referenceHdc, EmfType emfType, HandleRef pframeRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafile2_delegate> GdipRecordMetafile2_ptr;
            internal static int GdipRecordMetafile(HandleRef referenceHdc, EmfType emfType, HandleRef pframeRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafile2_ptr.Delegate(referenceHdc, emfType, pframeRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileI_delegate(HandleRef referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileI_delegate> GdipRecordMetafileI_ptr;
            internal static int GdipRecordMetafileI(HandleRef referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafileI_ptr.Delegate(referenceHdc, emfType, ref frameRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileFileName_delegate([MarshalAs(UnmanagedType.LPWStr)]string fileName, HandleRef referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileFileName_delegate> GdipRecordMetafileFileName_ptr;
            internal static int GdipRecordMetafileFileName(string fileName, HandleRef referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafileFileName_ptr.Delegate(fileName, referenceHdc, emfType, ref frameRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileFileName2_delegate([MarshalAs(UnmanagedType.LPWStr)]string fileName, HandleRef referenceHdc, EmfType emfType, HandleRef pframeRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileFileName2_delegate> GdipRecordMetafileFileName2_ptr;
            internal static int GdipRecordMetafileFileName(string fileName, HandleRef referenceHdc, EmfType emfType, HandleRef pframeRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafileFileName2_ptr.Delegate(fileName, referenceHdc, emfType, pframeRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileFileNameI_delegate([MarshalAs(UnmanagedType.LPWStr)]string fileName, HandleRef referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileFileNameI_delegate> GdipRecordMetafileFileNameI_ptr;
            internal static int GdipRecordMetafileFileNameI(string fileName, HandleRef referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafileFileNameI_ptr.Delegate(fileName, referenceHdc, emfType, ref frameRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileStream_delegate(Interop.Ole32.IStream stream, HandleRef referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileStream_delegate> GdipRecordMetafileStream_ptr;
            internal static int GdipRecordMetafileStream(Interop.Ole32.IStream stream, HandleRef referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile) => GdipRecordMetafileStream_ptr.Delegate(stream, referenceHdc, emfType, ref frameRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileStream2_delegate(Interop.Ole32.IStream stream, HandleRef referenceHdc, EmfType emfType, HandleRef pframeRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileStream2_delegate> GdipRecordMetafileStream2_ptr;
            internal static int GdipRecordMetafileStream(Interop.Ole32.IStream stream, HandleRef referenceHdc, EmfType emfType, HandleRef pframeRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafileStream2_ptr.Delegate(stream, referenceHdc, emfType, pframeRect, frameUnit, description, out metafile);

            private delegate int GdipRecordMetafileStreamI_delegate(Interop.Ole32.IStream stream, HandleRef referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, [MarshalAs(UnmanagedType.LPWStr)]string description, out IntPtr metafile);
            private static FunctionWrapper<GdipRecordMetafileStreamI_delegate> GdipRecordMetafileStreamI_ptr;
            internal static int GdipRecordMetafileStreamI(Interop.Ole32.IStream stream, HandleRef referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile) => GdipRecordMetafileStreamI_ptr.Delegate(stream, referenceHdc, emfType, ref frameRect, frameUnit, description, out metafile);

            private delegate int GdipComment_delegate(HandleRef graphics, int sizeData, byte[] data);
            private static FunctionWrapper<GdipComment_delegate> GdipComment_ptr;
            internal static int GdipComment(HandleRef graphics, int sizeData, byte[] data) => GdipComment_ptr.Delegate(graphics, sizeData, data);

            private delegate int GdipCreateFontFromDC_delegate(HandleRef hdc, ref IntPtr font);
            private static FunctionWrapper<GdipCreateFontFromDC_delegate> GdipCreateFontFromDC_ptr;
            internal static int GdipCreateFontFromDC(HandleRef hdc, ref IntPtr font) => GdipCreateFontFromDC_ptr.Delegate(hdc, ref font);

#pragma warning disable CS0618 // Legacy code: We don't care about using obsolete API's.
            private delegate int GdipCreateFontFromLogfontW_delegate(HandleRef hdc, [In] [Out] [MarshalAs(UnmanagedType.AsAny)]object lf, out IntPtr font);
#pragma warning restore CS0618
            private static FunctionWrapper<GdipCreateFontFromLogfontW_delegate> GdipCreateFontFromLogfontW_ptr;
            internal static int GdipCreateFontFromLogfontW(HandleRef hdc, [In] [Out] object lf, out IntPtr font) => GdipCreateFontFromLogfontW_ptr.Delegate(hdc, lf, out font);

            private delegate int GdipCreateBitmapFromStream_delegate(Interop.Ole32.IStream stream, out IntPtr bitmap);
            private static FunctionWrapper<GdipCreateBitmapFromStream_delegate> GdipCreateBitmapFromStream_ptr;
            internal static int GdipCreateBitmapFromStream(Interop.Ole32.IStream stream, out IntPtr bitmap) => GdipCreateBitmapFromStream_ptr.Delegate(stream, out bitmap);

            private delegate int GdipCreateBitmapFromStreamICM_delegate(Interop.Ole32.IStream stream, out IntPtr bitmap);
            private static FunctionWrapper<GdipCreateBitmapFromStreamICM_delegate> GdipCreateBitmapFromStreamICM_ptr;
            internal static int GdipCreateBitmapFromStreamICM(Interop.Ole32.IStream stream, out IntPtr bitmap) => GdipCreateBitmapFromStreamICM_ptr.Delegate(stream, out bitmap);
        }
    }
}
