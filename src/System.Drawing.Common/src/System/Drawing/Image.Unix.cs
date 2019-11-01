// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Image.cs
//
// Authors:     Christian Meyer (Christian.Meyer@cs.tum.edu)
//         Alexandre Pigolkine (pigolkine@gmx.de)
//        Jordi Mas i Hernandez (jordi@ximian.com)
//        Sanjay Gupta (gsanjay@novell.com)
//        Ravindra (rkumar@novell.com)
//        Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2002 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004, 2007 Novell, Inc (http://www.novell.com)
// Copyright (C) 2013 Kristof Ralovich, changes are available under the terms of the MIT X11 license
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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Gdip = System.Drawing.SafeNativeMethods.Gdip;

namespace System.Drawing
{
    public abstract partial class Image
    {
        // public methods
        // static

        // See http://support.microsoft.com/default.aspx?scid=kb;en-us;831419 for performance discussion
        public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
        {
            return LoadFromStream(stream, false);
        }

        internal static Image LoadFromStream(Stream stream, bool keepAlive)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Image img = CreateImageObject(InitializeFromStream(stream));
            return img;
        }

        private protected static IntPtr InitializeFromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // Unix, with libgdiplus
            // We use a custom API for this, because there's no easy way
            // to get the Stream down to libgdiplus.  So, we wrap the stream
            // with a set of delegates.
            GdiPlusStreamHelper sh = new GdiPlusStreamHelper(stream, true);

            int st = Gdip.GdipLoadImageFromDelegate_linux(sh.GetHeaderDelegate, sh.GetBytesDelegate,
                sh.PutBytesDelegate, sh.SeekDelegate, sh.CloseDelegate, sh.SizeDelegate, out IntPtr imagePtr);

            // Since we're just passing to native code the delegates inside the wrapper, we need to keep sh alive
            // to avoid the object being collected and therefore the delegates would be collected as well.
            GC.KeepAlive(sh);
            Gdip.CheckStatus(st);
            return imagePtr;
        }

        // non-static
        public RectangleF GetBounds(ref GraphicsUnit pageUnit)
        {
            RectangleF source;

            int status = Gdip.GdipGetImageBounds(nativeImage, out source, ref pageUnit);
            Gdip.CheckStatus(status);

            return source;
        }

        public PropertyItem GetPropertyItem(int propid)
        {
            int propSize;
            IntPtr property;
            PropertyItem item = new PropertyItem();
            GdipPropertyItem gdipProperty = new GdipPropertyItem();
            int status;

            status = Gdip.GdipGetPropertyItemSize(nativeImage, propid,
                                        out propSize);
            Gdip.CheckStatus(status);

            /* Get PropertyItem */
            property = Marshal.AllocHGlobal(propSize);
            try
            {
                status = Gdip.GdipGetPropertyItem(nativeImage, propid, propSize, property);
                Gdip.CheckStatus(status);
                gdipProperty = (GdipPropertyItem)Marshal.PtrToStructure(property,
                                    typeof(GdipPropertyItem));
                GdipPropertyItem.MarshalTo(gdipProperty, item);
            }
            finally
            {
                Marshal.FreeHGlobal(property);
            }
            return item;
        }

        public Image GetThumbnailImage(int thumbWidth, int thumbHeight, Image.GetThumbnailImageAbort callback, IntPtr callbackData)
        {
            if ((thumbWidth <= 0) || (thumbHeight <= 0))
                throw new OutOfMemoryException("Invalid thumbnail size");

            Image ThumbNail = new Bitmap(thumbWidth, thumbHeight);

            using (Graphics g = Graphics.FromImage(ThumbNail))
            {
                int status = Gdip.GdipDrawImageRectRectI(
                    new HandleRef(this, g.NativeGraphics),
                    new HandleRef(this, nativeImage),
                    0, 0, thumbWidth, thumbHeight,
                    0, 0, this.Width, this.Height,
                    GraphicsUnit.Pixel,
                    new HandleRef(this, IntPtr.Zero), null,
                    new HandleRef(this, IntPtr.Zero));

                Gdip.CheckStatus(status);
            }

            return ThumbNail;
        }

        internal ImageCodecInfo FindEncoderForFormat(ImageFormat format)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo encoder = null;

            if (format.Guid.Equals(ImageFormat.MemoryBmp.Guid))
                format = ImageFormat.Png;

            /* Look for the right encoder for our format*/
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].FormatID.Equals(format.Guid))
                {
                    encoder = encoders[i];
                    break;
                }
            }

            return encoder;
        }

        public void Save(string filename, ImageFormat format)
        {
            ImageCodecInfo encoder = FindEncoderForFormat(format);
            if (encoder == null)
            {
                // second chance
                encoder = FindEncoderForFormat(RawFormat);
                if (encoder == null)
                {
                    string msg = string.Format("No codec available for saving format '{0}'.", format.Guid);
                    throw new ArgumentException(msg, nameof(format));
                }
            }
            Save(filename, encoder, null);
        }

        public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            int st;
            Guid guid = encoder.Clsid;

            if (encoderParams == null)
            {
                st = Gdip.GdipSaveImageToFile(nativeImage, filename, ref guid, IntPtr.Zero);
            }
            else
            {
                IntPtr nativeEncoderParams = encoderParams.ConvertToMemory();
                st = Gdip.GdipSaveImageToFile(nativeImage, filename, ref guid, nativeEncoderParams);
                Marshal.FreeHGlobal(nativeEncoderParams);
            }

            Gdip.CheckStatus(st);
        }

        private void Save(MemoryStream stream)
        {
            // Jpeg loses data, so we don't want to use it to serialize...
            ImageFormat dest = RawFormat;
            if (dest.Guid == ImageFormat.Jpeg.Guid)
                dest = ImageFormat.Png;

            // If we don't find an Encoder (for things like Icon), we just switch back to PNG...
            ImageCodecInfo codec = FindEncoderForFormat(dest) ?? FindEncoderForFormat(ImageFormat.Png);

            Save(stream, codec, null);
        }

        public void Save(Stream stream, ImageFormat format)
        {
            ImageCodecInfo encoder = FindEncoderForFormat(format);

            if (encoder == null)
                throw new ArgumentException("No codec available for format:" + format.Guid);

            Save(stream, encoder, null);
        }

        public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            int st;
            IntPtr nativeEncoderParams;
            Guid guid = encoder.Clsid;

            if (encoderParams == null)
                nativeEncoderParams = IntPtr.Zero;
            else
                nativeEncoderParams = encoderParams.ConvertToMemory();

            try
            {
                GdiPlusStreamHelper sh = new GdiPlusStreamHelper(stream, false);
                st = Gdip.GdipSaveImageToDelegate_linux(nativeImage, sh.GetBytesDelegate, sh.PutBytesDelegate,
                    sh.SeekDelegate, sh.CloseDelegate, sh.SizeDelegate, ref guid, nativeEncoderParams);

                // Since we're just passing to native code the delegates inside the wrapper, we need to keep sh alive
                // to avoid the object being collected and therefore the delegates would be collected as well.
                GC.KeepAlive(sh);
            }
            finally
            {
                if (nativeEncoderParams != IntPtr.Zero)
                    Marshal.FreeHGlobal(nativeEncoderParams);
            }

            Gdip.CheckStatus(st);
        }

        public void SaveAdd(EncoderParameters encoderParams)
        {
            int st;

            IntPtr nativeEncoderParams = encoderParams.ConvertToMemory();
            st = Gdip.GdipSaveAdd(nativeImage, nativeEncoderParams);
            Marshal.FreeHGlobal(nativeEncoderParams);
            Gdip.CheckStatus(st);
        }

        public void SaveAdd(Image image, EncoderParameters encoderParams)
        {
            int st;

            IntPtr nativeEncoderParams = encoderParams.ConvertToMemory();
            st = Gdip.GdipSaveAddImage(nativeImage, image.nativeImage, nativeEncoderParams);
            Marshal.FreeHGlobal(nativeEncoderParams);
            Gdip.CheckStatus(st);
        }

        public void SetPropertyItem(PropertyItem propitem)
        {
            if (propitem == null)
                throw new ArgumentNullException(nameof(propitem));

            int nItemSize = Marshal.SizeOf(propitem.Value[0]);
            int size = nItemSize * propitem.Value.Length;
            IntPtr dest = Marshal.AllocHGlobal(size);
            try
            {
                GdipPropertyItem pi = new GdipPropertyItem();
                pi.id = propitem.Id;
                pi.len = propitem.Len;
                pi.type = propitem.Type;

                Marshal.Copy(propitem.Value, 0, dest, size);
                pi.value = dest;

                unsafe
                {
                    int status = Gdip.GdipSetPropertyItem(nativeImage, &pi);

                    Gdip.CheckStatus(status);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(dest);
            }
        }

        [Browsable(false)]
        public ColorPalette Palette
        {
            get
            {
                return retrieveGDIPalette();
            }
            set
            {
                storeGDIPalette(value);
            }
        }

        internal ColorPalette retrieveGDIPalette()
        {
            int bytes;
            ColorPalette ret = new ColorPalette();

            int st = Gdip.GdipGetImagePaletteSize(nativeImage, out bytes);
            Gdip.CheckStatus(st);
            IntPtr palette_data = Marshal.AllocHGlobal(bytes);
            try
            {
                st = Gdip.GdipGetImagePalette(nativeImage, palette_data, bytes);
                Gdip.CheckStatus(st);
                ret.ConvertFromMemory(palette_data);
                return ret;
            }

            finally
            {
                Marshal.FreeHGlobal(palette_data);
            }
        }

        internal void storeGDIPalette(ColorPalette palette)
        {
            if (palette == null)
            {
                throw new ArgumentNullException(nameof(palette));
            }
            IntPtr palette_data = palette.ConvertToMemory();
            if (palette_data == IntPtr.Zero)
            {
                return;
            }

            try
            {
                int st = Gdip.GdipSetImagePalette(nativeImage, palette_data);
                Gdip.CheckStatus(st);
            }

            finally
            {
                Marshal.FreeHGlobal(palette_data);
            }
        }

        [Browsable(false)]
        public int[] PropertyIdList
        {
            get
            {
                uint propNumbers;

                int status = Gdip.GdipGetPropertyCount(nativeImage,
                                        out propNumbers);
                Gdip.CheckStatus(status);

                int[] idList = new int[propNumbers];
                status = Gdip.GdipGetPropertyIdList(nativeImage,
                                    propNumbers, idList);
                Gdip.CheckStatus(status);

                return idList;
            }
        }

        [Browsable(false)]
        public PropertyItem[] PropertyItems
        {
            get
            {
                int propNums, propsSize, propSize;
                IntPtr properties, propPtr;
                PropertyItem[] items;
                GdipPropertyItem gdipProperty = new GdipPropertyItem();
                int status;

                status = Gdip.GdipGetPropertySize(nativeImage, out propsSize, out propNums);
                Gdip.CheckStatus(status);

                items = new PropertyItem[propNums];

                if (propNums == 0)
                    return items;

                /* Get PropertyItem list*/
                properties = Marshal.AllocHGlobal(propsSize * propNums);
                try
                {
                    status = Gdip.GdipGetAllPropertyItems(nativeImage, propsSize,
                                    propNums, properties);
                    Gdip.CheckStatus(status);

                    propSize = Marshal.SizeOf(gdipProperty);
                    propPtr = properties;

                    for (int i = 0; i < propNums; i++, propPtr = new IntPtr(propPtr.ToInt64() + propSize))
                    {
                        gdipProperty = (GdipPropertyItem)Marshal.PtrToStructure
                            (propPtr, typeof(GdipPropertyItem));
                        items[i] = new PropertyItem();
                        GdipPropertyItem.MarshalTo(gdipProperty, items[i]);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(properties);
                }
                return items;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (nativeImage != IntPtr.Zero)
            {
                int status = Gdip.GdipDisposeImage(new HandleRef(this, nativeImage));
                // ... set nativeImage to null before (possibly) throwing an exception
                nativeImage = IntPtr.Zero;
                Gdip.CheckStatus(status);
            }
        }

        public object Clone()
        {
            IntPtr newimage = IntPtr.Zero;
            int status = Gdip.GdipCloneImage(nativeImage, out newimage);
            Gdip.CheckStatus(status);

            if (this is Bitmap)
                return new Bitmap(newimage);
            else
                return new Metafile(newimage);
        }

        internal static void ValidateImage(IntPtr bitmap)
        {
            // No validation is performed on Unix.
        }
    }
}
