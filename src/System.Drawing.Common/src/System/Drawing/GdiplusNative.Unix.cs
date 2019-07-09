// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    internal partial class SafeNativeMethods
    {
        internal unsafe partial class Gdip
        {
            private static IntPtr s_gdipModule;

            private const string LibraryName = "libgdiplus";
            public static IntPtr Display = IntPtr.Zero;

            // Indicates whether X11 is available. It's available on Linux but not on recent macOS versions
            // When set to false, where Carbon Drawing is used instead.
            // macOS users can force X11 by setting the SYSTEM_DRAWING_COMMON_FORCE_X11 flag.
            public static bool UseX11Drawable { get; } =
                !RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                Environment.GetEnvironmentVariable("SYSTEM_DRAWING_COMMON_FORCE_X11") != null;

            private static IntPtr LoadNativeLibrary()
            {
                string libraryName;

                IntPtr lib = IntPtr.Zero;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    libraryName = "libgdiplus.dylib";

#if netcoreapp20
                    lib = Interop.Libdl.dlopen(libraryName, Interop.Libdl.RTLD_LAZY);
#else // use managed NativeLibrary API from .NET Core 3 onwards
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    NativeLibrary.TryLoad(libraryName, assembly, default, out lib);
#endif
                }
                else
                {
                    // Various Unix package managers have chosen different names for the "libgdiplus" shared library.
                    // The mono project, where libgdiplus originated, allowed both of the names below to be used, via
                    // a global configuration setting. We prefer the "unversioned" shared object name, and fallback to
                    // the name suffixed with ".0".
                    libraryName = "libgdiplus.so";

#if netcoreapp20
                    lib = Interop.Libdl.dlopen(libraryName, Interop.Libdl.RTLD_LAZY);
                    if (lib == IntPtr.Zero)
                    {
                        lib = Interop.Libdl.dlopen("libgdiplus.so.0", Interop.Libdl.RTLD_LAZY);
                    }
#else // use managed NativeLibrary API from .NET Core 3 onwards
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    if (!NativeLibrary.TryLoad(libraryName, assembly, default, out lib))
                    {
                         NativeLibrary.TryLoad("libgdiplus.so.0", assembly, default, out lib);
                    }
#endif
                }

#if netcoreapp20
                // If we couldn't find libgdiplus in the system search path, try to look for libgdiplus in the
                // NuGet package folders. This matches the DllImport behavior.
                if (lib == IntPtr.Zero)
                {
                    string[] searchDirectories = ((string)AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES")).Split(':');

                    foreach (var searchDirectory in searchDirectories)
                    {
                        var searchPath = Path.Combine(searchDirectory, libraryName);

                        lib = Interop.Libdl.dlopen(searchPath, Interop.Libdl.RTLD_LAZY);
                        if (lib != IntPtr.Zero)
                        {
                            break;
                        }
                    }
                }
#endif

                // This function may return a null handle. If it does, individual functions loaded from it will throw a DllNotFoundException,
                // but not until an attempt is made to actually use the function (rather than load it). This matches how PInvokes behave.
                return lib;
            }

            private static void PlatformInitialize()
            {
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);

                s_gdipModule = LoadNativeLibrary();
            }

            public static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
            {
                if (libraryName == LibraryName)
                {
                    return LoadNativeLibrary();
                }

                return NativeLibrary.Load(libraryName, assembly, searchPath);
            }

            // Imported functions

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static void GdiplusShutdown(ref ulong token);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static IntPtr GdipAlloc(int size);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static void GdipFree(IntPtr ptr);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDeleteBrush(HandleRef brush);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetBrushType(IntPtr brush, out BrushType type);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFromHDC(IntPtr hDC, out IntPtr graphics);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDeleteGraphics(HandleRef graphics);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipRestoreGraphics(IntPtr graphics, uint graphicsState);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSaveGraphics(IntPtr graphics, out uint state);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawArc(IntPtr graphics, IntPtr pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawArcI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawBezier(IntPtr graphics, IntPtr pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawBezierI(IntPtr graphics, IntPtr pen, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawEllipseI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawEllipse(IntPtr graphics, IntPtr pen, float x, float y, float width, float height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawLine(IntPtr graphics, IntPtr pen, float x1, float y1, float x2, float y2);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawLineI(IntPtr graphics, IntPtr pen, int x1, int y1, int x2, int y2);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawLines(IntPtr graphics, IntPtr pen, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawLinesI(IntPtr graphics, IntPtr pen, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawPath(IntPtr graphics, IntPtr pen, IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawPie(IntPtr graphics, IntPtr pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawPieI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawPolygon(IntPtr graphics, IntPtr pen, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawPolygonI(IntPtr graphics, IntPtr pen, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawRectangle(IntPtr graphics, IntPtr pen, float x, float y, float width, float height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawRectangleI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawRectangles(IntPtr graphics, IntPtr pen, RectangleF[] rects, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawRectanglesI(IntPtr graphics, IntPtr pen, Rectangle[] rects, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillEllipseI(IntPtr graphics, IntPtr pen, int x, int y, int width, int height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillEllipse(IntPtr graphics, IntPtr pen, float x, float y, float width, float height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPolygon(IntPtr graphics, IntPtr brush, PointF[] points, int count, FillMode fillMode);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPolygonI(IntPtr graphics, IntPtr brush, Point[] points, int count, FillMode fillMode);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPolygon2(IntPtr graphics, IntPtr brush, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPolygon2I(IntPtr graphics, IntPtr brush, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillRectangle(IntPtr graphics, IntPtr brush, float x1, float y1, float x2, float y2);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillRectangleI(IntPtr graphics, IntPtr brush, int x1, int y1, int x2, int y2);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillRectangles(IntPtr graphics, IntPtr brush, RectangleF[] rects, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillRectanglesI(IntPtr graphics, IntPtr brush, Rectangle[] rects, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawString(IntPtr graphics, string text, int len, IntPtr font, ref RectangleF rc, IntPtr format, IntPtr brush);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipReleaseDC(HandleRef graphics, HandleRef hdc);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImageRectI(IntPtr graphics, IntPtr image, int x, int y, int width, int height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGraphicsClear(IntPtr graphics, int argb);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawClosedCurve(IntPtr graphics, IntPtr pen, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawClosedCurveI(IntPtr graphics, IntPtr pen, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawClosedCurve2(IntPtr graphics, IntPtr pen, PointF[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawClosedCurve2I(IntPtr graphics, IntPtr pen, Point[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawCurve(IntPtr graphics, IntPtr pen, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawCurveI(IntPtr graphics, IntPtr pen, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawCurve2(IntPtr graphics, IntPtr pen, PointF[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawCurve2I(IntPtr graphics, IntPtr pen, Point[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawCurve3(IntPtr graphics, IntPtr pen, PointF[] points, int count, int offset, int numberOfSegments, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawCurve3I(IntPtr graphics, IntPtr pen, Point[] points, int count, int offset, int numberOfSegments, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillClosedCurve(IntPtr graphics, IntPtr brush, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillClosedCurveI(IntPtr graphics, IntPtr brush, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillClosedCurve2(IntPtr graphics, IntPtr brush, PointF[] points, int count, float tension, FillMode fillMode);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillClosedCurve2I(IntPtr graphics, IntPtr brush, Point[] points, int count, float tension, FillMode fillMode);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPie(IntPtr graphics, IntPtr brush, float x, float y, float width, float height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPieI(IntPtr graphics, IntPtr brush, int x, int y, int width, int height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFillPath(IntPtr graphics, IntPtr brush, IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetNearestColor(IntPtr graphics, out int argb);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipTransformPoints(IntPtr graphics, CoordinateSpace destSpace, CoordinateSpace srcSpace, IntPtr points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipTransformPointsI(IntPtr graphics, CoordinateSpace destSpace, CoordinateSpace srcSpace, IntPtr points, int count);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipAddPathString(IntPtr path, string s, int lenght, IntPtr family, int style, float emSize, ref RectangleF layoutRect, IntPtr format);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipAddPathStringI(IntPtr path, string s, int lenght, IntPtr family, int style, float emSize, ref Rectangle layoutRect, IntPtr format);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFromHWND(IntPtr hwnd, out IntPtr graphics);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipMeasureString(IntPtr graphics, string str, int length, IntPtr font, ref RectangleF layoutRect, IntPtr stringFormat, out RectangleF boundingBox, int* codepointsFitted, int* linesFilled);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipMeasureCharacterRanges(IntPtr graphics, string str, int length, IntPtr font, ref RectangleF layoutRect, IntPtr stringFormat, int regcount, out IntPtr regions);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipLoadImageFromFile(string filename, out IntPtr image);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCloneImage(IntPtr image, out IntPtr imageclone);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipLoadImageFromFileICM(string filename, out IntPtr image);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDisposeImage(HandleRef image);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetImageType(IntPtr image, out ImageType type);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetImagePaletteSize(IntPtr image, out int size);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetImagePalette(IntPtr image, IntPtr palette, int size);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSetImagePalette(IntPtr image, IntPtr palette);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPropertyCount(IntPtr image, out uint propNumbers);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPropertyIdList(IntPtr image, uint propNumbers, [Out] int[] list);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPropertySize(IntPtr image, out int bufferSize, out int propNumbers);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetAllPropertyItems(IntPtr image, int bufferSize, int propNumbers, IntPtr items);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetImageBounds(IntPtr image, out RectangleF source, ref GraphicsUnit unit);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetEncoderParameterListSize(IntPtr image, ref Guid encoder, out uint size);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetEncoderParameterList(IntPtr image, ref Guid encoder, uint size, IntPtr buffer);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPropertyItemSize(IntPtr image, int propertyID, out int propertySize);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPropertyItem(IntPtr image, int propertyID, int propertySize, IntPtr buffer);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSetPropertyItem(IntPtr image, GdipPropertyItem* propertyItem);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetImageThumbnail(IntPtr image, uint width, uint height, out IntPtr thumbImage, IntPtr callback, IntPtr callBackData);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipSaveImageToFile(IntPtr image, string filename, ref Guid encoderClsID, IntPtr encoderParameters);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSaveAdd(IntPtr image, IntPtr encoderParameters);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSaveAddImage(IntPtr image, IntPtr imagenew, IntPtr encoderParameters);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImageI(IntPtr graphics, IntPtr image, int x, int y);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetImageGraphicsContext(IntPtr image, out IntPtr graphics);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImage(IntPtr graphics, IntPtr image, float x, float y);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImagePoints(IntPtr graphics, IntPtr image, PointF[] destPoints, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImagePointsI(IntPtr graphics, IntPtr image, Point[] destPoints, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImageRectRectI(IntPtr graphics, IntPtr image, int dstx, int dsty, int dstwidth, int dstheight, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImageRectRect(IntPtr graphics, IntPtr image, float dstx, float dsty, float dstwidth, float dstheight, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImagePointsRectI(IntPtr graphics, IntPtr image, Point[] destPoints, int count, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImagePointsRect(IntPtr graphics, IntPtr image, PointF[] destPoints, int count, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcUnit, IntPtr imageattr, Graphics.DrawImageAbort callback, IntPtr callbackData);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImageRect(IntPtr graphics, IntPtr image, float x, float y, float width, float height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImagePointRect(IntPtr graphics, IntPtr image, float x, float y, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcUnit);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDrawImagePointRectI(IntPtr graphics, IntPtr image, int x, int y, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcUnit);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreatePath(FillMode brushMode, out IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreatePath2(PointF[] points, byte[] types, int count, FillMode brushMode, out IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreatePath2I(Point[] points, byte[] types, int count, FillMode brushMode, out IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipClonePath(IntPtr path, out IntPtr clonePath);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipDeletePath(HandleRef path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipResetPath(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPointCount(IntPtr path, out int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathTypes(IntPtr path, [Out] byte[] types, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathPoints(IntPtr path, [Out] PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathPointsI(IntPtr path, [Out] Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathFillMode(IntPtr path, out FillMode fillMode);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSetPathFillMode(IntPtr path, FillMode fillMode);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipStartPathFigure(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipClosePathFigure(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipClosePathFigures(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSetPathMarker(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipClearPathMarkers(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipReversePath(IntPtr path);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathLastPoint(IntPtr path, out PointF lastPoint);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathLine(IntPtr path, float x1, float y1, float x2, float y2);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathLine2(IntPtr path, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathLine2I(IntPtr path, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathArc(IntPtr path, float x, float y, float width, float height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathBezier(IntPtr path, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathBeziers(IntPtr path, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathCurve(IntPtr path, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathCurveI(IntPtr path, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathCurve2(IntPtr path, PointF[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathCurve2I(IntPtr path, Point[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathCurve3(IntPtr path, PointF[] points, int count, int offset, int numberOfSegments, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathCurve3I(IntPtr path, Point[] points, int count, int offset, int numberOfSegments, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathClosedCurve(IntPtr path, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathClosedCurveI(IntPtr path, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathClosedCurve2(IntPtr path, PointF[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathClosedCurve2I(IntPtr path, Point[] points, int count, float tension);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathRectangle(IntPtr path, float x, float y, float width, float height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathRectangles(IntPtr path, RectangleF[] rects, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathEllipse(IntPtr path, float x, float y, float width, float height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathEllipseI(IntPtr path, int x, int y, int width, int height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathPie(IntPtr path, float x, float y, float width, float height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathPieI(IntPtr path, int x, int y, int width, int height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathPolygon(IntPtr path, PointF[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathPath(IntPtr path, IntPtr addingPath, bool connect);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathLineI(IntPtr path, int x1, int y1, int x2, int y2);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathArcI(IntPtr path, int x, int y, int width, int height, float startAngle, float sweepAngle);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathBezierI(IntPtr path, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathBeziersI(IntPtr path, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathPolygonI(IntPtr path, Point[] points, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathRectangleI(IntPtr path, int x, int y, int width, int height);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipAddPathRectanglesI(IntPtr path, Rectangle[] rects, int count);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipFlattenPath(IntPtr path, IntPtr matrix, float floatness);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipTransformPath(IntPtr path, IntPtr matrix);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipWarpPath(IntPtr path, IntPtr matrix, PointF[] points, int count, float srcx, float srcy, float srcwidth, float srcheight, WarpMode mode, float flatness);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipWidenPath(IntPtr path, IntPtr pen, IntPtr matrix, float flatness);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathWorldBounds(IntPtr path, out RectangleF bounds, IntPtr matrix, IntPtr pen);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPathWorldBoundsI(IntPtr path, out Rectangle bounds, IntPtr matrix, IntPtr pen);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipIsVisiblePathPoint(IntPtr path, float x, float y, IntPtr graphics, out bool result);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipIsVisiblePathPointI(IntPtr path, int x, int y, IntPtr graphics, out bool result);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipIsOutlineVisiblePathPoint(IntPtr path, float x, float y, IntPtr pen, IntPtr graphics, out bool result);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipIsOutlineVisiblePathPointI(IntPtr path, int x, int y, IntPtr pen, IntPtr graphics, out bool result);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFontFromDC(IntPtr hdc, out IntPtr font);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFontFromLogfont(IntPtr hdc, ref LOGFONT lf, out IntPtr ptr);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFontFromHfont(IntPtr hdc, out IntPtr font, ref LOGFONT lf);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipCreateMetafileFromFile(string filename, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateMetafileFromEmf(IntPtr hEmf, bool deleteEmf, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateMetafileFromWmf(IntPtr hWmf, bool deleteWmf, WmfPlaceableFileHeader wmfPlaceableFileHeader, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipGetMetafileHeaderFromFile(string filename, IntPtr header);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetMetafileHeaderFromMetafile(IntPtr metafile, IntPtr header);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetMetafileHeaderFromEmf(IntPtr hEmf, IntPtr header);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetMetafileHeaderFromWmf(IntPtr hWmf, IntPtr wmfPlaceableFileHeader, IntPtr header);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetHemfFromMetafile(IntPtr metafile, out IntPtr hEmf);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetMetafileDownLevelRasterizationLimit(IntPtr metafile, ref uint metafileRasterizationLimitDpi);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSetMetafileDownLevelRasterizationLimit(IntPtr metafile, uint metafileRasterizationLimitDpi);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipPlayMetafileRecord(IntPtr metafile, EmfPlusRecordType recordType, int flags, int dataSize, byte[] data);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipRecordMetafile(IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipRecordMetafileI(IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipRecordMetafileFileName(string filename, IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipRecordMetafileFileNameI(string filename, IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string description, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFromContext_macosx(IntPtr cgref, int width, int height, out IntPtr graphics);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSetVisibleClip_linux(IntPtr graphics, ref Rectangle rect);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateFromXDrawable_linux(IntPtr drawable, IntPtr display, out IntPtr graphics);

            // Stream functions for non-Win32 (libgdiplus specific)
            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipLoadImageFromDelegate_linux(StreamGetHeaderDelegate getHeader,
                StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek,
                StreamCloseDelegate close, StreamSizeDelegate size, out IntPtr image);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipSaveImageToDelegate_linux(IntPtr image, StreamGetBytesDelegate getBytes,
                StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close,
                StreamSizeDelegate size, ref Guid encoderClsID, IntPtr encoderParameters);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipCreateMetafileFromDelegate_linux(StreamGetHeaderDelegate getHeader,
                StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek,
                StreamCloseDelegate close, StreamSizeDelegate size, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetMetafileHeaderFromDelegate_linux(StreamGetHeaderDelegate getHeader,
                StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek,
                StreamCloseDelegate close, StreamSizeDelegate size, IntPtr header);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipRecordMetafileFromDelegate_linux(StreamGetHeaderDelegate getHeader,
                StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek,
                StreamCloseDelegate close, StreamSizeDelegate size, IntPtr hdc, EmfType type, ref RectangleF frameRect,
                MetafileFrameUnit frameUnit, string description, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true, CharSet = CharSet.Unicode)]
            internal static int GdipRecordMetafileFromDelegateI_linux(StreamGetHeaderDelegate getHeader,
                StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek,
                StreamCloseDelegate close, StreamSizeDelegate size, IntPtr hdc, EmfType type, ref Rectangle frameRect,
                MetafileFrameUnit frameUnit, string description, out IntPtr metafile);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPostScriptGraphicsContext(
                [MarshalAs(UnmanagedType.LPStr)]string filename,
                int width, int height, double dpix, double dpiy, ref IntPtr graphics);

            [DllImport(LibraryName, ExactSpelling = true)]
            internal static int GdipGetPostScriptSavePage(IntPtr graphics);
        }
    }

    // These are unix-only
    internal unsafe delegate int StreamGetHeaderDelegate(byte* buf, int bufsz);
    internal unsafe delegate int StreamGetBytesDelegate(byte* buf, int bufsz, bool peek);
    internal delegate long StreamSeekDelegate(int offset, int whence);
    internal unsafe delegate int StreamPutBytesDelegate(byte* buf, int bufsz);
    internal delegate void StreamCloseDelegate();
    internal delegate long StreamSizeDelegate();
}
