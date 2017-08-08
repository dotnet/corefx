// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.Metafile.cs
//
// Authors:
//    Christian Meyer, eMail: Christian.Meyer@cs.tum.edu
//    Dennis Hayes (dennish@raytek.com)
//    Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
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

using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{

    [MonoTODO("Metafiles, both WMF and EMF formats, are only partially supported.")]
#if !NETCORE
    [Serializable]
    [Editor ("System.Drawing.Design.MetafileEditor, " + Consts.AssemblySystem_Drawing_Design, typeof (System.Drawing.Design.UITypeEditor))]
#endif
    public sealed class Metafile : Image
    {

        // constructors

        internal Metafile(IntPtr ptr)
        {
            nativeObject = ptr;
        }

        // Usually called when cloning images that need to have
        // not only the handle saved, but also the underlying stream
        // (when using MS GDI+ and IStream we must ensure the stream stays alive for all the life of the Image)
        internal Metafile(IntPtr ptr, Stream stream)
        {
            // under Win32 stream is owned by SD/GDI+ code
            if (GDIPlus.RunningOnWindows())
                this.stream = stream;
            nativeObject = ptr;
        }

        public Metafile(Stream stream)
        {
            if (stream == null)
                throw new ArgumentException("stream");

            Status status;
            if (GDIPlus.RunningOnUnix())
            {
                // With libgdiplus we use a custom API for this, because there's no easy way
                // to get the Stream down to libgdiplus. So, we wrap the stream with a set of delegates.
                GDIPlus.GdiPlusStreamHelper sh = new GDIPlus.GdiPlusStreamHelper(stream, false);
                status = SafeNativeMethods.Gdip.GdipCreateMetafileFromDelegate_linux(sh.GetHeaderDelegate, sh.GetBytesDelegate,
                    sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate, sh.SizeDelegate, out nativeObject);
            }
            else
            {
                status = SafeNativeMethods.Gdip.GdipCreateMetafileFromStream(new ComIStreamWrapper(stream), out nativeObject);
            }
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");
            if (filename.Length == 0)
                throw new ArgumentException("filename");

            Status status = SafeNativeMethods.Gdip.GdipCreateMetafileFromFile(filename, out nativeObject);
            if (status == Status.GenericError)
                throw new ExternalException("Couldn't load specified file.");
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(IntPtr henhmetafile, bool deleteEmf)
        {
            Status status = SafeNativeMethods.Gdip.GdipCreateMetafileFromEmf(henhmetafile, deleteEmf, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(IntPtr referenceHdc, EmfType emfType) :
            this(referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, emfType, null)
        {
        }

        public Metafile(IntPtr referenceHdc, Rectangle frameRect) :
            this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr referenceHdc, RectangleF frameRect) :
            this(referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
        {
            Status status = SafeNativeMethods.Gdip.GdipCreateMetafileFromEmf(hmetafile, false, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(Stream stream, IntPtr referenceHdc) :
            this(stream, referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc) :
            this(fileName, referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual,
            null)
        {
        }

        public Metafile(IntPtr referenceHdc, EmfType emfType, string description) :
            this(referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, emfType, description)
        {
        }

        public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
            this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
            this(referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader, bool deleteWmf)
        {
            Status status = SafeNativeMethods.Gdip.GdipCreateMetafileFromEmf(hmetafile, deleteWmf, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(Stream stream, IntPtr referenceHdc, EmfType type) :
            this(stream, referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, type, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect) :
            this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect) :
            this(stream, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, EmfType type) :
            this(fileName, referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, type, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect) :
            this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect) :
            this(fileName, referenceHdc, frameRect, MetafileFrameUnit.GdiCompatible, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type) :
            this(referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, EmfType type, string description) :
            this(stream, referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, type, description)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
            this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
            this(stream, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, EmfType type, string description) :
            this(fileName, referenceHdc, new RectangleF(), MetafileFrameUnit.GdiCompatible, type, description)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit) :
            this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit) :
            this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit, EmfType type,
            string desc)
        {
            Status status = SafeNativeMethods.Gdip.GdipRecordMetafileI(referenceHdc, type, ref frameRect, frameUnit,
                desc, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit, EmfType type,
            string description)
        {
            Status status = SafeNativeMethods.Gdip.GdipRecordMetafile(referenceHdc, type, ref frameRect, frameUnit,
                description, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
            EmfType type) : this(stream, referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
            EmfType type) : this(stream, referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
            EmfType type) : this(fileName, referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
            string description) : this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual, description)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
            EmfType type) : this(fileName, referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
            string desc) : this(fileName, referenceHdc, frameRect, frameUnit, EmfType.EmfPlusDual,
            desc)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
            EmfType type, string description)
        {
            if (stream == null)
                throw new NullReferenceException("stream");

            Status status = Status.NotImplemented;
            if (GDIPlus.RunningOnUnix())
            {
                // With libgdiplus we use a custom API for this, because there's no easy way
                // to get the Stream down to libgdiplus. So, we wrap the stream with a set of delegates.
                GDIPlus.GdiPlusStreamHelper sh = new GDIPlus.GdiPlusStreamHelper(stream, false);
                status = SafeNativeMethods.Gdip.GdipRecordMetafileFromDelegateI_linux(sh.GetHeaderDelegate, sh.GetBytesDelegate,
                    sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate, sh.SizeDelegate, referenceHdc,
                    type, ref frameRect, frameUnit, description, out nativeObject);
            }
            else
            {
                status = SafeNativeMethods.Gdip.GdipRecordMetafileStreamI(new ComIStreamWrapper(stream), referenceHdc,
                    type, ref frameRect, frameUnit, description, out nativeObject);
            }
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
            EmfType type, string description)
        {
            if (stream == null)
                throw new NullReferenceException("stream");

            Status status = Status.NotImplemented;
            if (GDIPlus.RunningOnUnix())
            {
                // With libgdiplus we use a custom API for this, because there's no easy way
                // to get the Stream down to libgdiplus. So, we wrap the stream with a set of delegates.
                GDIPlus.GdiPlusStreamHelper sh = new GDIPlus.GdiPlusStreamHelper(stream, false);
                status = SafeNativeMethods.Gdip.GdipRecordMetafileFromDelegate_linux(sh.GetHeaderDelegate, sh.GetBytesDelegate,
                    sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate, sh.SizeDelegate, referenceHdc,
                    type, ref frameRect, frameUnit, description, out nativeObject);
            }
            else
            {
                status = SafeNativeMethods.Gdip.GdipRecordMetafileStream(new ComIStreamWrapper(stream), referenceHdc,
                    type, ref frameRect, frameUnit, description, out nativeObject);
            }
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(string fileName, IntPtr referenceHdc, Rectangle frameRect, MetafileFrameUnit frameUnit,
            EmfType type, string description)
        {
            Status status = SafeNativeMethods.Gdip.GdipRecordMetafileFileNameI(fileName, referenceHdc, type, ref frameRect,
                frameUnit, description, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        public Metafile(string fileName, IntPtr referenceHdc, RectangleF frameRect, MetafileFrameUnit frameUnit,
            EmfType type, string description)
        {
            Status status = SafeNativeMethods.Gdip.GdipRecordMetafileFileName(fileName, referenceHdc, type, ref frameRect, frameUnit,
                description, out nativeObject);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        // methods

        public IntPtr GetHenhmetafile()
        {
            return nativeObject;
        }

        [MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
        public MetafileHeader GetMetafileHeader()
        {
            IntPtr header = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
            try
            {
                Status status = SafeNativeMethods.Gdip.GdipGetMetafileHeaderFromMetafile(nativeObject, header);
                SafeNativeMethods.Gdip.CheckStatus(status);
                return new MetafileHeader(header);
            }
            finally
            {
                Marshal.FreeHGlobal(header);
            }
        }

        [MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
        public static MetafileHeader GetMetafileHeader(IntPtr henhmetafile)
        {
            IntPtr header = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
            try
            {
                Status status = SafeNativeMethods.Gdip.GdipGetMetafileHeaderFromEmf(henhmetafile, header);
                SafeNativeMethods.Gdip.CheckStatus(status);
                return new MetafileHeader(header);
            }
            finally
            {
                Marshal.FreeHGlobal(header);
            }
        }

        [MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
        public static MetafileHeader GetMetafileHeader(Stream stream)
        {
            if (stream == null)
                throw new NullReferenceException("stream");

            IntPtr header = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
            try
            {
                Status status;

                if (GDIPlus.RunningOnUnix())
                {
                    // With libgdiplus we use a custom API for this, because there's no easy way
                    // to get the Stream down to libgdiplus. So, we wrap the stream with a set of delegates.
                    GDIPlus.GdiPlusStreamHelper sh = new GDIPlus.GdiPlusStreamHelper(stream, false);
                    status = SafeNativeMethods.Gdip.GdipGetMetafileHeaderFromDelegate_linux(sh.GetHeaderDelegate,
                        sh.GetBytesDelegate, sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate,
                        sh.SizeDelegate, header);
                }
                else
                {
                    status = SafeNativeMethods.Gdip.GdipGetMetafileHeaderFromStream(new ComIStreamWrapper(stream), header);
                }
                SafeNativeMethods.Gdip.CheckStatus(status);
                return new MetafileHeader(header);
            }
            finally
            {
                Marshal.FreeHGlobal(header);
            }
        }

        [MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
        public static MetafileHeader GetMetafileHeader(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            IntPtr header = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
            try
            {
                Status status = SafeNativeMethods.Gdip.GdipGetMetafileHeaderFromFile(fileName, header);
                SafeNativeMethods.Gdip.CheckStatus(status);
                return new MetafileHeader(header);
            }
            finally
            {
                Marshal.FreeHGlobal(header);
            }
        }

        [MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
        public static MetafileHeader GetMetafileHeader(IntPtr hmetafile, WmfPlaceableFileHeader wmfHeader)
        {
            IntPtr header = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MetafileHeader)));
            try
            {
                Status status = SafeNativeMethods.Gdip.GdipGetMetafileHeaderFromEmf(hmetafile, header);
                SafeNativeMethods.Gdip.CheckStatus(status);
                return new MetafileHeader(header);
            }
            finally
            {
                Marshal.FreeHGlobal(header);
            }
        }

        [MonoLimitation("Metafiles aren't only partially supported by libgdiplus.")]
        public void PlayRecord(EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
        {
            Status status = SafeNativeMethods.Gdip.GdipPlayMetafileRecord(nativeObject, recordType, flags, dataSize, data);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }
    }
}
