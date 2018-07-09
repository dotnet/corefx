// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Internal;
using Gdip = System.Drawing.SafeNativeMethods.Gdip;

namespace System.Drawing.Imaging
{
    /// <summary>
    /// Defines a graphic metafile. A metafile contains records that describe a sequence of graphics operations that
    /// can be recorded and played back.
    /// </summary>
    public sealed partial class Metafile : Image
    {
        // GDI+ doesn't handle filenames over MAX_PATH very well
        private const int MaxPath = 260;

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified handle and
        /// <see cref='WmfPlaceableFileHeader'/>.
        /// </summary>
        public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader) :
            this(hmetafile, wmfHeader, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified handle and
        /// <see cref='WmfPlaceableFileHeader'/>.
        /// </summary>
        public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader, bool deleteWmf)
        {
            Gdip.CheckStatus(Gdip.GdipCreateMetafileFromWmf(new HandleRef(null, hmetafile), deleteWmf, wmfHeader, out IntPtr metafile));
            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified handle and
        /// <see cref='WmfPlaceableFileHeader'/>.
        /// </summary>
        public Metafile(IntPtr henhmetafile, bool deleteEmf)
        {
            Gdip.CheckStatus(Gdip.GdipCreateMetafileFromEmf(new HandleRef(null, henhmetafile), deleteEmf, out IntPtr metafile));
            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified filename.
        /// </summary>
        public Metafile(string filename)
        {
            // Called in order to emulate exception behavior from netfx related to invalid file paths.
            Path.GetFullPath(filename);
            Gdip.CheckStatus(Gdip.GdipCreateMetafileFromFile(filename, out IntPtr metafile));
            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified stream.
        /// </summary>
        public Metafile(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Gdip.CheckStatus(Gdip.GdipCreateMetafileFromStream(new GPStream(stream), out IntPtr metafile));
            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified handle to a device context.
        /// </summary>
        public Metafile(IntPtr referenceHdc, EmfType emfType) :
            this(referenceHdc, emfType, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified handle to a device context.
        /// </summary>
        public Metafile(IntPtr referenceHdc, EmfType emfType, string description)
        {
            Gdip.CheckStatus(Gdip.GdipRecordMetafile(
                new HandleRef(null, referenceHdc),
                emfType,
                NativeMethods.NullHandleRef,
                MetafileFrameUnit.GdiCompatible,
                description,
                out IntPtr metafile));

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, RectangleF frameRect) :
            this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
            this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(referenceHdc, frameRect, frameUnit, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
        {
            Gdip.CheckStatus(Gdip.GdipRecordMetafile(
                new HandleRef(null, referenceHdc),
                type,
                ref frameRect,
                frameUnit,
                description,
                out IntPtr metafile));

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, Rectangle frameRect) :
            this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
            this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(referenceHdc, frameRect, frameUnit, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified device context, bounded
        /// by the specified rectangle.
        /// </summary>
        public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string desc)
        {
            IntPtr metafile = IntPtr.Zero;

            if (frameRect.IsEmpty)
            {
                Gdip.CheckStatus(Gdip.GdipRecordMetafile(
                    new HandleRef(null, referenceHdc),
                    type,
                    NativeMethods.NullHandleRef,
                    MetafileFrameUnit.GdiCompatible,
                    desc,
                    out metafile));
            }
            else
            {
                Gdip.CheckStatus(Gdip.GdipRecordMetafileI(
                    new HandleRef(null, referenceHdc),
                    type,
                    ref frameRect,
                    frameUnit,
                    desc,
                    out metafile));
            }

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc) :
            this(fileName, referenceHdc, EmfType.EmfPlusDual, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, EmfType type) :
            this(fileName, referenceHdc, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, EmfType type, string description)
        {
            // Called in order to emulate exception behavior from netfx related to invalid file paths.
            Path.GetFullPath(fileName);

            Gdip.CheckStatus(Gdip.GdipRecordMetafileFileName(
                fileName,
                new HandleRef(null, referenceHdc),
                type,
                NativeMethods.NullHandleRef,
                MetafileFrameUnit.GdiCompatible,
                description,
                out IntPtr metafile));

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect) :
            this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
            this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect,  MetafileFrameUnit frameUnit, EmfType type) :
            this(fileName, referenceHdc, frameRect, frameUnit, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, string desc) :
            this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, desc)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
        {
            // Called in order to emulate exception behavior from netfx related to invalid file paths.
            Path.GetFullPath(fileName);

            if (fileName.Length > MaxPath)
                throw new PathTooLongException();

            Gdip.CheckStatus(Gdip.GdipRecordMetafileFileName(
                fileName,
                new HandleRef(null, referenceHdc),
                type,
                ref frameRect,
                frameUnit,
                description,
                out IntPtr metafile));

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect) :
            this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
            this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(fileName, referenceHdc, frameRect, frameUnit, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, string description) :
            this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, description)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
        {
            // Called in order to emulate exception behavior from netfx related to invalid file paths.
            Path.GetFullPath(fileName);

            IntPtr metafile = IntPtr.Zero;

            if (frameRect.IsEmpty)
            {
                Gdip.CheckStatus(Gdip.GdipRecordMetafileFileName(
                    fileName,
                    new HandleRef(null, referenceHdc),
                    type,
                    NativeMethods.NullHandleRef,
                    frameUnit,
                    description,
                    out metafile));
            }
            else
            {
                Gdip.CheckStatus(Gdip.GdipRecordMetafileFileNameI(
                    fileName,
                    new HandleRef(null, referenceHdc),
                    type,
                    ref frameRect,
                    frameUnit,
                    description,
                    out metafile));
            }

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc) :
            this(stream, referenceHdc, EmfType.EmfPlusDual, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, EmfType type) :
            this(stream, referenceHdc, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, EmfType type, string description)
        {
            Gdip.CheckStatus(Gdip.GdipRecordMetafileStream(
                new GPStream(stream),
                new HandleRef(null, referenceHdc),
                type,
                NativeMethods.NullHandleRef,
                MetafileFrameUnit.GdiCompatible,
                description,
                out IntPtr metafile));

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect) :
            this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
            this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(stream, referenceHdc, frameRect, frameUnit, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
        {
            Gdip.CheckStatus(Gdip.GdipRecordMetafileStream(
                new GPStream(stream),
                new HandleRef(null, referenceHdc),
                type,
                ref frameRect,
                frameUnit,
                description,
                out IntPtr metafile));

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class from the specified data stream.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect) :
            this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
            this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(stream, referenceHdc, frameRect, frameUnit, type, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref='Metafile'/> class with the specified filename.
        /// </summary>
        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type, string description)
        {
            IntPtr metafile = IntPtr.Zero;

            if (frameRect.IsEmpty)
            {
                Gdip.CheckStatus(Gdip.GdipRecordMetafileStream(
                    new GPStream(stream),
                    new HandleRef(null, referenceHdc),
                    type,
                    NativeMethods.NullHandleRef,
                    frameUnit,
                    description,
                    out metafile));
            }
            else
            {
                Gdip.CheckStatus(Gdip.GdipRecordMetafileStreamI(
                    new GPStream(stream),
                    new HandleRef(null, referenceHdc),
                    type,
                    ref frameRect,
                    frameUnit,
                    description,
                    out metafile));
            }

            SetNativeImage(metafile);
        }

        /// <summary>
        /// Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
        /// </summary>
        public static MetafileHeader GetMetafileHeader(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
        {
            MetafileHeader header = new MetafileHeader
            {
                wmf = new MetafileHeaderWmf()
            };

            Gdip.CheckStatus(Gdip.GdipGetMetafileHeaderFromWmf(new HandleRef(null, hmetafile), wmfHeader, header.wmf));
            return header;
        }

        /// <summary>
        /// Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
        /// </summary>
        public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile)
        {
            MetafileHeader header = new MetafileHeader
            {
                emf = new MetafileHeaderEmf()
            };

            Gdip.CheckStatus(Gdip.GdipGetMetafileHeaderFromEmf(new HandleRef(null, henhmetafile), header.emf));
            return header;
        }

        /// <summary>
        /// Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
        /// </summary>
        public static MetafileHeader GetMetafileHeader(string fileName)
        {
            // Called in order to emulate exception behavior from netfx related to invalid file paths.
            Path.GetFullPath(fileName);

            MetafileHeader header = new MetafileHeader();

            IntPtr memory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeaderEmf)));

            try
            {
                Gdip.CheckStatus(Gdip.GdipGetMetafileHeaderFromFile(fileName, memory));

                int[] type = new int[] { 0 };

                Marshal.Copy(memory, type, 0, 1);

                MetafileType metafileType = (MetafileType)type[0];

                if (metafileType == MetafileType.Wmf ||
                    metafileType == MetafileType.WmfPlaceable)
                {
                    // WMF header
                    header.wmf = (MetafileHeaderWmf)Marshal.PtrToStructure(memory, typeof(MetafileHeaderWmf));
                    header.emf = null;
                }
                else
                {
                    // EMF header
                    header.wmf = null;
                    header.emf = (MetafileHeaderEmf)Marshal.PtrToStructure(memory, typeof(MetafileHeaderEmf));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }

            return header;
        }

        /// <summary>
        /// Returns the <see cref='MetafileHeader'/> associated with the specified <see cref='Metafile'/>.
        /// </summary>
        public static MetafileHeader GetMetafileHeader(Stream stream)
        {
            MetafileHeader header;

            IntPtr memory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeaderEmf)));

            try
            {
                Gdip.CheckStatus(Gdip.GdipGetMetafileHeaderFromStream(new GPStream(stream), memory));

                int[] type = new int[] { 0 };

                Marshal.Copy(memory, type, 0, 1);

                MetafileType metafileType = (MetafileType)type[0];

                header = new MetafileHeader();

                if (metafileType == MetafileType.Wmf ||
                    metafileType == MetafileType.WmfPlaceable)
                {
                    // WMF header
                    header.wmf = (MetafileHeaderWmf)Marshal.PtrToStructure(memory, typeof(MetafileHeaderWmf));
                    header.emf = null;
                }
                else
                {
                    // EMF header
                    header.wmf = null;
                    header.emf = (MetafileHeaderEmf)Marshal.PtrToStructure(memory, typeof(MetafileHeaderEmf));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }

            return header;
        }

        /// <summary>
        /// Returns the <see cref='MetafileHeader'/> associated with this <see cref='Metafile'/>.
        /// </summary>
        public MetafileHeader GetMetafileHeader()
        {
            MetafileHeader header;

            IntPtr memory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeaderEmf)));

            try
            {
                Gdip.CheckStatus(Gdip.GdipGetMetafileHeaderFromMetafile(new HandleRef(this, nativeImage), memory));

                int[] type = new int[] { 0 };

                Marshal.Copy(memory, type, 0, 1);

                MetafileType metafileType = (MetafileType)type[0];

                header = new MetafileHeader();

                if (metafileType == MetafileType.Wmf ||
                    metafileType == MetafileType.WmfPlaceable)
                {
                    // WMF header
                    header.wmf = (MetafileHeaderWmf)Marshal.PtrToStructure(memory, typeof(MetafileHeaderWmf));
                    header.emf = null;
                }
                else
                {
                    // EMF header
                    header.wmf = null;
                    header.emf = (MetafileHeaderEmf)Marshal.PtrToStructure(memory, typeof(MetafileHeaderEmf));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }

            return header;
        }

        /// <summary>
        /// Returns a Windows handle to an enhanced <see cref='Metafile'/>.
        /// </summary>
        public IntPtr GetHenhmetafile()
        {
            Gdip.CheckStatus(Gdip.GdipGetHemfFromMetafile(new HandleRef(this, nativeImage), out IntPtr hEmf));
            return hEmf;
        }

        /// <summary>
        /// Plays an EMF+ file.
        /// </summary>
        public void PlayRecord(EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
        {
            // Used in conjunction with Graphics.EnumerateMetafile to play an EMF+
            // The data must be DWORD aligned if it's an EMF or EMF+.  It must be
            // WORD aligned if it's a WMF.

            Gdip.CheckStatus(Gdip.GdipPlayMetafileRecord(
                new HandleRef(this, nativeImage),
                recordType,
                flags,
                dataSize,
                data));
        }

        /// <summary>
        /// Create a new metafile object from a native metafile handle.
        /// </summary>
        internal static Metafile FromGDIplus(IntPtr nativeImage)
        {
            Metafile metafile = new Metafile();
            metafile.SetNativeImage(nativeImage);
            return metafile;
        }

        private Metafile()
        {
        }
    }
}
