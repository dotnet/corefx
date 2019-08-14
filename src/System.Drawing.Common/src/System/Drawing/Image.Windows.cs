// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Gdip = System.Drawing.SafeNativeMethods.Gdip;

namespace System.Drawing
{
    public abstract partial class Image
    {
#if FINALIZATION_WATCH
        private string allocationSite = Graphics.GetAllocationStack();
#endif

        public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            IntPtr image = IntPtr.Zero;

            if (useEmbeddedColorManagement)
            {
                Gdip.CheckStatus(Gdip.GdipLoadImageFromStreamICM(new GPStream(stream), out image));
            }
            else
            {
                Gdip.CheckStatus(Gdip.GdipLoadImageFromStream(new GPStream(stream), out image));
            }

            if (validateImageData)
                ValidateImage(image);

            Image img = CreateImageObject(image);
            EnsureSave(img, null, stream);
            return img;
        }

        // Used for serialization
        private IntPtr InitializeFromStream(Stream stream)
        {
            IntPtr image = IntPtr.Zero;

            Gdip.CheckStatus(Gdip.GdipLoadImageFromStream(new GPStream(stream), out image));
            ValidateImage(image);

            nativeImage = image;

            int type = -1;

            Gdip.CheckStatus(Gdip.GdipGetImageType(new HandleRef(this, nativeImage), out type));
            EnsureSave(this, null, stream);
            return image;
        }

        internal Image(IntPtr nativeImage) => SetNativeImage(nativeImage);

        /// <summary>
        /// Creates an exact copy of this <see cref='Image'/>.
        /// </summary>
        public object Clone()
        {
            IntPtr cloneImage = IntPtr.Zero;

            Gdip.CheckStatus(Gdip.GdipCloneImage(new HandleRef(this, nativeImage), out cloneImage));
            ValidateImage(cloneImage);

            return CreateImageObject(cloneImage);
        }

        protected virtual void Dispose(bool disposing)
        {
#if FINALIZATION_WATCH
            if (!disposing && nativeImage != IntPtr.Zero)
                Debug.WriteLine("**********************\nDisposed through finalization:\n" + allocationSite);
#endif
            if (nativeImage == IntPtr.Zero)
                return;

            try
            {
#if DEBUG
                int status = !Gdip.Initialized ? Gdip.Ok :
#endif
                Gdip.GdipDisposeImage(new HandleRef(this, nativeImage));
#if DEBUG
                Debug.Assert(status == Gdip.Ok, "GDI+ returned an error status: " + status.ToString(CultureInfo.InvariantCulture));
#endif
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }

                Debug.Fail("Exception thrown during Dispose: " + ex.ToString());
            }
            finally
            {
                nativeImage = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Saves this <see cref='Image'/> to the specified file in the specified format.
        /// </summary>
        public void Save(string filename, ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            ImageCodecInfo codec = format.FindEncoder();

            if (codec == null)
                codec = ImageFormat.Png.FindEncoder();

            Save(filename, codec, null);
        }

        /// <summary>
        /// Saves this <see cref='Image'/> to the specified file in the specified format and with the specified encoder parameters.
        /// </summary>
        public void Save(string filename, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            IntPtr encoderParamsMemory = IntPtr.Zero;

            if (encoderParams != null)
            {
                _rawData = null;
                encoderParamsMemory = encoderParams.ConvertToMemory();
            }

            try
            {
                Guid g = encoder.Clsid;
                bool saved = false;

                if (_rawData != null)
                {
                    ImageCodecInfo rawEncoder = RawFormat.FindEncoder();
                    if (rawEncoder != null && rawEncoder.Clsid == g)
                    {
                        using (FileStream fs = File.OpenWrite(filename))
                        {
                            fs.Write(_rawData, 0, _rawData.Length);
                            saved = true;
                        }
                    }
                }

                if (!saved)
                {
                    Gdip.CheckStatus(Gdip.GdipSaveImageToFile(
                        new HandleRef(this, nativeImage),
                        filename,
                        ref g,
                        new HandleRef(encoderParams, encoderParamsMemory)));
                }
            }
            finally
            {
                if (encoderParamsMemory != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(encoderParamsMemory);
                }
            }
        }

        private void Save(MemoryStream stream)
        {
            // Jpeg loses data, so we don't want to use it to serialize...
            ImageFormat dest = RawFormat;
            if (dest.Guid == ImageFormat.Jpeg.Guid)
                dest = ImageFormat.Png;

            // If we don't find an Encoder (for things like Icon), we just switch back to PNG...
            ImageCodecInfo codec = dest.FindEncoder() ?? ImageFormat.Png.FindEncoder();

            Save(stream, codec, null);
        }

        /// <summary>
        /// Saves this <see cref='Image'/> to the specified stream in the specified format.
        /// </summary>
        public void Save(Stream stream, ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            ImageCodecInfo codec = format.FindEncoder();
            Save(stream, codec, null);
        }

        /// <summary>
        /// Saves this <see cref='Image'/> to the specified stream in the specified format.
        /// </summary>
        public void Save(Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            IntPtr encoderParamsMemory = IntPtr.Zero;

            if (encoderParams != null)
            {
                _rawData = null;
                encoderParamsMemory = encoderParams.ConvertToMemory();
            }

            try
            {
                Guid g = encoder.Clsid;
                bool saved = false;

                if (_rawData != null)
                {
                    ImageCodecInfo rawEncoder = RawFormat.FindEncoder();
                    if (rawEncoder != null && rawEncoder.Clsid == g)
                    {
                        stream.Write(_rawData, 0, _rawData.Length);
                        saved = true;
                    }
                }

                if (!saved)
                {
                    Gdip.CheckStatus(Gdip.GdipSaveImageToStream(
                        new HandleRef(this, nativeImage),
                        new GPStream(stream),
                        ref g,
                        new HandleRef(encoderParams, encoderParamsMemory)));
                }
            }
            finally
            {
                if (encoderParamsMemory != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(encoderParamsMemory);
                }
            }
        }

        /// <summary>
        /// Adds an <see cref='EncoderParameters'/> to this <see cref='Image'/>.
        /// </summary>
        public void SaveAdd(EncoderParameters encoderParams)
        {
            IntPtr encoder = IntPtr.Zero;
            if (encoderParams != null)
                encoder = encoderParams.ConvertToMemory();

            _rawData = null;

            try
            {
                Gdip.CheckStatus(Gdip.GdipSaveAdd(new HandleRef(this, nativeImage), new HandleRef(encoderParams, encoder)));
            }
            finally
            {
                if (encoder != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(encoder);
                }
            }
        }

        /// <summary>
        /// Adds an <see cref='EncoderParameters'/> to the specified <see cref='Image'/>.
        /// </summary>
        public void SaveAdd(Image image, EncoderParameters encoderParams)
        {
            IntPtr encoder = IntPtr.Zero;

            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (encoderParams != null)
                encoder = encoderParams.ConvertToMemory();

            _rawData = null;

            try
            {
                Gdip.CheckStatus(Gdip.GdipSaveAddImage(
                    new HandleRef(this, nativeImage),
                    new HandleRef(image, image.nativeImage),
                    new HandleRef(encoderParams, encoder)));
            }
            finally
            {
                if (encoder != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(encoder);
                }
            }
        }

        /// <summary>
        /// Gets a bounding rectangle in the specified units for this <see cref='Image'/>.
        /// </summary>
        public RectangleF GetBounds(ref GraphicsUnit pageUnit)
        {
            Gdip.CheckStatus(Gdip.GdipGetImageBounds(new HandleRef(this, nativeImage), out RectangleF bounds, out pageUnit));
            return bounds;
        }

        /// <summary>
        /// Gets or sets the color palette used for this <see cref='Image'/>.
        /// </summary>
        [Browsable(false)]
        public ColorPalette Palette
        {
            get
            {
                Gdip.CheckStatus(Gdip.GdipGetImagePaletteSize(new HandleRef(this, nativeImage), out int size));

                // "size" is total byte size:
                // sizeof(ColorPalette) + (pal->Count-1)*sizeof(ARGB)

                ColorPalette palette = new ColorPalette(size);

                // Memory layout is:
                //    UINT Flags
                //    UINT Count
                //    ARGB Entries[size]

                IntPtr memory = Marshal.AllocHGlobal(size);
                try
                {
                    Gdip.CheckStatus(Gdip.GdipGetImagePalette(new HandleRef(this, nativeImage), memory, size));
                    palette.ConvertFromMemory(memory);
                }
                finally
                {
                    Marshal.FreeHGlobal(memory);
                }

                return palette;
            }
            set
            {
                IntPtr memory = value.ConvertToMemory();

                try
                {
                    Gdip.CheckStatus(Gdip.GdipSetImagePalette(new HandleRef(this, nativeImage), memory));
                }
                finally
                {
                    if (memory != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(memory);
                    }
                }
            }
        }

        // Thumbnail support

        /// <summary>
        /// Returns the thumbnail for this <see cref='Image'/>.
        /// </summary>
        public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort callback, IntPtr callbackData)
        {
            IntPtr thumbImage = IntPtr.Zero;

            Gdip.CheckStatus(Gdip.GdipGetImageThumbnail(
                new HandleRef(this, nativeImage),
                thumbWidth,
                thumbHeight,
                out thumbImage,
                callback,
                callbackData));

            return CreateImageObject(thumbImage);
        }

        /// <summary>
        /// Gets an array of the property IDs stored in this <see cref='Image'/>.
        /// </summary>
        [Browsable(false)]
        public int[] PropertyIdList
        {
            get
            {
                Gdip.CheckStatus(Gdip.GdipGetPropertyCount(new HandleRef(this, nativeImage), out int count));

                int[] propid = new int[count];

                //if we have a 0 count, just return our empty array
                if (count == 0)
                    return propid;

                Gdip.CheckStatus(Gdip.GdipGetPropertyIdList(new HandleRef(this, nativeImage), count, propid));

                return propid;
            }
        }

        /// <summary>
        /// Gets the specified property item from this <see cref='Image'/>.
        /// </summary>
        public PropertyItem GetPropertyItem(int propid)
        {
            Gdip.CheckStatus(Gdip.GdipGetPropertyItemSize(new HandleRef(this, nativeImage), propid, out int size));

            if (size == 0)
                return null;

            IntPtr propdata = Marshal.AllocHGlobal(size);

            if (propdata == IntPtr.Zero)
                throw Gdip.StatusException(Gdip.OutOfMemory);

            try
            {
                Gdip.CheckStatus(Gdip.GdipGetPropertyItem(new HandleRef(this, nativeImage), propid, size, propdata));
                return PropertyItemInternal.ConvertFromMemory(propdata, 1)[0];
            }
            finally
            {
                Marshal.FreeHGlobal(propdata);
            }
        }

        /// <summary>
        /// Sets the specified property item to the specified value.
        /// </summary>
        public void SetPropertyItem(PropertyItem propitem)
        {
            PropertyItemInternal propItemInternal = PropertyItemInternal.ConvertFromPropertyItem(propitem);

            using (propItemInternal)
            {
                Gdip.CheckStatus(Gdip.GdipSetPropertyItem(new HandleRef(this, nativeImage), propItemInternal));
            }
        }

        /// <summary>
        /// Gets an array of <see cref='PropertyItem'/> objects that describe this <see cref='Image'/>.
        /// </summary>
        [Browsable(false)]
        public PropertyItem[] PropertyItems
        {
            get
            {
                Gdip.CheckStatus(Gdip.GdipGetPropertyCount(new HandleRef(this, nativeImage), out int count));
                Gdip.CheckStatus(Gdip.GdipGetPropertySize(new HandleRef(this, nativeImage), out int size, ref count));

                if (size == 0 || count == 0)
                    return Array.Empty<PropertyItem>();

                IntPtr propdata = Marshal.AllocHGlobal(size);
                try
                {
                    Gdip.CheckStatus(Gdip.GdipGetAllPropertyItems(new HandleRef(this, nativeImage), size, count, propdata));
                    return PropertyItemInternal.ConvertFromMemory(propdata, count);
                }
                finally
                {
                    Marshal.FreeHGlobal(propdata);
                }
            }
        }

        internal static void ValidateImage(IntPtr image)
        {
            try
            {
                Gdip.CheckStatus(Gdip.GdipImageForceValidation(image));
            }
            catch
            {
                Gdip.GdipDisposeImage(image);
                throw;
            }
        }
    }
}
