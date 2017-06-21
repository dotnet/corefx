// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Bitmap.cs
//
// Copyright (C) 2002 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004 Novell, Inc.  http://www.novell.com
//
// Authors: 
//	Alexandre Pigolkine (pigolkine@gmx.de)
//	Christian Meyer (Christian.Meyer@cs.tum.edu)
//	Miguel de Icaza (miguel@ximian.com)
//	Jordi Mas i Hernandez (jmas@softcatala.org)
//	Ravindra (rkumar@novell.com)
//

//
// Copyright (C) 2004-2005 Novell, Inc (http://www.novell.com)
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
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace System.Drawing
{
    [ComVisible(true)]
    [Serializable]
#if !NETCORE
	[Editor ("System.Drawing.Design.BitmapEditor, " + Consts.AssemblySystem_Drawing_Design, typeof (System.Drawing.Design.UITypeEditor))]
#endif
    public sealed class Bitmap : Image
    {
        #region constructors
        // constructors

        // required for XmlSerializer (#323246)
        private Bitmap()
        {
        }

        internal Bitmap(IntPtr ptr)
        {
            nativeObject = ptr;
        }

        // Usually called when cloning images that need to have
        // not only the handle saved, but also the underlying stream
        // (when using MS GDI+ and IStream we must ensure the stream stays alive for all the life of the Image)
        internal Bitmap(IntPtr ptr, Stream stream)
        {
            // under Win32 stream is owned by SD/GDI+ code
            if (GDIPlus.RunningOnWindows())
                this.stream = stream;
            nativeObject = ptr;
        }

        public Bitmap(int width, int height) : this(width, height, PixelFormat.Format32bppArgb)
        {
        }

        public Bitmap(int width, int height, Graphics g)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            IntPtr bmp;
            Status s = GDIPlus.GdipCreateBitmapFromGraphics(width, height, g.nativeObject, out bmp);
            GDIPlus.CheckStatus(s);
            nativeObject = bmp;
        }

        public Bitmap(int width, int height, PixelFormat format)
        {
            IntPtr bmp;
            Status s = GDIPlus.GdipCreateBitmapFromScan0(width, height, 0, format, IntPtr.Zero, out bmp);
            GDIPlus.CheckStatus(s);
            nativeObject = bmp;

        }

        public Bitmap(Image original) : this(original, original.Width, original.Height) { }

        public Bitmap(Stream stream) : this(stream, false) { }

        public Bitmap(string filename) : this(filename, false) { }

        public Bitmap(Image original, Size newSize) : this(original, newSize.Width, newSize.Height) { }

        public Bitmap(Stream stream, bool useIcm)
        {
            // false: stream is owned by user code
            nativeObject = InitFromStream(stream);
        }

        public Bitmap(string filename, bool useIcm)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            IntPtr imagePtr;
            Status st;

            if (useIcm)
                st = GDIPlus.GdipCreateBitmapFromFileICM(filename, out imagePtr);
            else
                st = GDIPlus.GdipCreateBitmapFromFile(filename, out imagePtr);

            GDIPlus.CheckStatus(st);
            nativeObject = imagePtr;
        }

        public Bitmap(Type type, string resource)
        {
            if (resource == null)
                throw new ArgumentException("resource");

            // For compatibility with the .NET Framework
            if (type == null)
                throw new NullReferenceException();

            Stream s = type.GetTypeInfo().Assembly.GetManifestResourceStream(type, resource);
            if (s == null)
            {
                string msg = string.Format("Resource '{0}' was not found.", resource);
                throw new FileNotFoundException(msg);
            }

            nativeObject = InitFromStream(s);
            // under Win32 stream is owned by SD/GDI+ code
            if (GDIPlus.RunningOnWindows())
                stream = s;
        }

        public Bitmap(Image original, int width, int height) : this(width, height, PixelFormat.Format32bppArgb)
        {
            Graphics graphics = Graphics.FromImage(this);

            graphics.DrawImage(original, 0, 0, width, height);
            graphics.Dispose();
        }

        public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
        {
            IntPtr bmp;

            Status status = GDIPlus.GdipCreateBitmapFromScan0(width, height, stride, format, scan0, out bmp);
            GDIPlus.CheckStatus(status);
            nativeObject = bmp;
        }

        private Bitmap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
        // methods
        public Color GetPixel(int x, int y)
        {

            int argb;

            Status s = GDIPlus.GdipBitmapGetPixel(nativeObject, x, y, out argb);
            GDIPlus.CheckStatus(s);

            return Color.FromArgb(argb);
        }

        public void SetPixel(int x, int y, Color color)
        {
            Status s = GDIPlus.GdipBitmapSetPixel(nativeObject, x, y, color.ToArgb());
            if (s == Status.InvalidParameter)
            {
                // check is done in case of an error only to avoid another
                // unmanaged call for normal (successful) calls
                if ((this.PixelFormat & PixelFormat.Indexed) != 0)
                {
                    string msg = "SetPixel cannot be called on indexed bitmaps.";
                    throw new InvalidOperationException(msg);
                }
            }
            GDIPlus.CheckStatus(s);
        }

        public Bitmap Clone(Rectangle rect, PixelFormat format)
        {
            IntPtr bmp;
            Status status = GDIPlus.GdipCloneBitmapAreaI(rect.X, rect.Y, rect.Width, rect.Height,
                format, nativeObject, out bmp);
            GDIPlus.CheckStatus(status);
            return new Bitmap(bmp);
        }

        public Bitmap Clone(RectangleF rect, PixelFormat format)
        {
            IntPtr bmp;
            Status status = GDIPlus.GdipCloneBitmapArea(rect.X, rect.Y, rect.Width, rect.Height,
                format, nativeObject, out bmp);
            GDIPlus.CheckStatus(status);
            return new Bitmap(bmp);
        }

        public static Bitmap FromHicon(IntPtr hicon)
        {
            IntPtr bitmap;
            Status status = GDIPlus.GdipCreateBitmapFromHICON(hicon, out bitmap);
            GDIPlus.CheckStatus(status);
            return new Bitmap(bitmap);
        }

        public static Bitmap FromResource(IntPtr hinstance, string bitmapName)  //TODO: Untested
        {
            IntPtr bitmap;
            Status status = GDIPlus.GdipCreateBitmapFromResource(hinstance, bitmapName, out bitmap);
            GDIPlus.CheckStatus(status);
            return new Bitmap(bitmap);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IntPtr GetHbitmap()
        {
            return GetHbitmap(Color.Gray);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IntPtr GetHbitmap(Color background)
        {
            IntPtr HandleBmp;

            Status status = GDIPlus.GdipCreateHBITMAPFromBitmap(nativeObject, out HandleBmp, background.ToArgb());
            GDIPlus.CheckStatus(status);

            return HandleBmp;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IntPtr GetHicon()
        {
            IntPtr HandleIcon;

            Status status = GDIPlus.GdipCreateHICONFromBitmap(nativeObject, out HandleIcon);
            GDIPlus.CheckStatus(status);

            return HandleIcon;
        }

        public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
        {
            BitmapData result = new BitmapData();
            return LockBits(rect, flags, format, result);
        }

        public
        BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
        {
            Status status = GDIPlus.GdipBitmapLockBits(nativeObject, ref rect, flags, format, bitmapData);
            //NOTE: scan0 points to piece of memory allocated in the unmanaged space
            GDIPlus.CheckStatus(status);

            return bitmapData;
        }

        public void MakeTransparent()
        {
            Color clr = GetPixel(0, 0);
            MakeTransparent(clr);
        }

        public void MakeTransparent(Color transparentColor)
        {
            // We have to draw always over a 32-bitmap surface that supports alpha channel
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            Graphics gr = Graphics.FromImage(bmp);
            Rectangle destRect = new Rectangle(0, 0, Width, Height);
            ImageAttributes imageAttr = new ImageAttributes();

            imageAttr.SetColorKey(transparentColor, transparentColor);

            gr.DrawImage(this, destRect, 0, 0, Width, Height, GraphicsUnit.Pixel, imageAttr);

            IntPtr oldBmp = nativeObject;
            nativeObject = bmp.nativeObject;
            bmp.nativeObject = oldBmp;

            gr.Dispose();
            bmp.Dispose();
            imageAttr.Dispose();
        }

        public void SetResolution(float xDpi, float yDpi)
        {
            Status status = GDIPlus.GdipBitmapSetResolution(nativeObject, xDpi, yDpi);
            GDIPlus.CheckStatus(status);
        }

        public void UnlockBits(BitmapData bitmapdata)
        {
            Status status = GDIPlus.GdipBitmapUnlockBits(nativeObject, bitmapdata);
            GDIPlus.CheckStatus(status);
        }
    }
}
