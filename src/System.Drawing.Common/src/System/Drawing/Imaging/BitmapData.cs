// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Imaging
{
    public partial class BitmapData
    {
        /// <summary>
        /// Specifies the pixel width of the <see cref='Bitmap'/>.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Specifies the pixel height of the <see cref='Bitmap'/>.
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Specifies the stride width of the <see cref='Bitmap'/>.
        /// </summary>
        public int Stride
        {
            get { return _stride; }
            set { _stride = value; }
        }

        /// <summary>
        /// Specifies the format of the pixel information in this <see cref='Bitmap'/>.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return _pixelFormat; }
            set
            {
                if (!Enum.IsDefined(typeof(PixelFormat), value))
                {
                    throw new System.ComponentModel.InvalidEnumArgumentException(nameof(value), unchecked((int)value), typeof(PixelFormat));
                }

                _pixelFormat = value;
            }
        }

        /// <summary>
        /// Specifies the address of the pixel data.
        /// </summary>
        public IntPtr Scan0
        {
            get { return _scan0; }
            set { _scan0 = value; }
        }

        /// <summary>
        /// Reserved. Do not use.
        /// </summary>
        public int Reserved
        {
            get { return _reserved; }
            set { _reserved = value; }
        }
    }
}
