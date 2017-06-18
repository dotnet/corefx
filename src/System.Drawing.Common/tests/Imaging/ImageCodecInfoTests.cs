// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Authors:
// 	Jordi Mas i Hernàndez (jordi@ximian.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2004 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004-2007 Novell, Inc (http://www.novell.com)
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
using System.Collections;
using System.Text.RegularExpressions;
using Xunit;

namespace System.Drawing.Imaging.Tests
{
    public class ImageCodecInfoTests
    {
        private const string GIF_CSID = "557cf402-1a04-11d3-9a73-0000f81ef32e";
        private const string EMF_CSID = "557cf403-1a04-11d3-9a73-0000f81ef32e";
        private const string BMP_DIB_RLE_CSID = "557cf400-1a04-11d3-9a73-0000f81ef32e";
        private const string JPG_JPEG_JPE_JFIF_CSID = "557cf401-1a04-11d3-9a73-0000f81ef32e";
        private const string PNG_CSID = "557cf406-1a04-11d3-9a73-0000f81ef32e";
        private const string ICO_CSID = "557cf407-1a04-11d3-9a73-0000f81ef32e";
        private const string WMF_CSID = "557cf404-1a04-11d3-9a73-0000f81ef32e";
        private const string TIF_CSID = "557cf405-1a04-11d3-9a73-0000f81ef32e";

        Hashtable decoders;
        Hashtable encoders;

        ImageCodecInfo GetEncoder(Guid clsid)
        {
            return (ImageCodecInfo)encoders[clsid];
        }

        ImageCodecInfo GetDecoder(Guid clsid)
        {
            return (ImageCodecInfo)decoders[clsid];
        }

        public ImageCodecInfoTests()
        {
            decoders = new Hashtable();
            encoders = new Hashtable();

            foreach (ImageCodecInfo decoder in ImageCodecInfo.GetImageDecoders())
                decoders[decoder.Clsid] = decoder;

            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                encoders[encoder.Clsid] = encoder;
        }

        void Check(string clsid, ImageFormat format, string CodecName, string DllName,
            string FilenameExtension, ImageCodecFlags Flags, string FormatDescription,
            string MimeType, int Version, int signatureLength, string mask, string pattern, string pattern2)
        {
            ImageCodecInfo encoder = GetEncoder(new Guid(clsid));
            ImageCodecInfo decoder = GetDecoder(new Guid(clsid));

            Regex extRegex = new Regex(@"^(\*\.\w+(;(\*\.\w+))*;)?" +
                Regex.Escape(FilenameExtension) + @"(;\*\.\w+(;(\*\.\w+))*)?$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (encoder != null)
            {
                Assert.Equal(format.Guid, encoder.FormatID);
                Assert.True(encoder.CodecName.IndexOf(CodecName) >= 0,
                    "Encoder.CodecName contains " + CodecName);
                Assert.Equal(DllName, encoder.DllName);
                Assert.True(extRegex.IsMatch(encoder.FilenameExtension),
                    "Encoder.FilenameExtension is a right list with " + FilenameExtension);
                Assert.Equal(Flags, encoder.Flags);
                Assert.True(encoder.FormatDescription.IndexOf(FormatDescription) >= 0,
                    "Encoder.FormatDescription contains " + FormatDescription);
                Assert.True(encoder.MimeType.IndexOf(MimeType) >= 0,
                    "Encoder.MimeType contains " + MimeType);

                Assert.Equal(signatureLength, encoder.SignatureMasks.Length);
                for (int i = 0; i < signatureLength; i++)
                {
                    Assert.Equal(mask, BitConverter.ToString(encoder.SignatureMasks[i]));
                }
                Assert.Equal(signatureLength, encoder.SignaturePatterns.Length);
                Assert.Equal(pattern, BitConverter.ToString(encoder.SignaturePatterns[0]));
                if (pattern2 != null)
                    Assert.Equal(pattern2, BitConverter.ToString(encoder.SignaturePatterns[1]));
            }
            if (decoder != null)
            {
                Assert.Equal(format.Guid, decoder.FormatID);
                Assert.True(decoder.CodecName.IndexOf(CodecName) >= 0,
                    "Decoder.CodecName contains " + CodecName);
                Assert.Equal(DllName, decoder.DllName);
                Assert.True(extRegex.IsMatch(decoder.FilenameExtension),
                    "Decoder.FilenameExtension is a right list with " + FilenameExtension);
                Assert.Equal(Flags, decoder.Flags);
                Assert.True(decoder.FormatDescription.IndexOf(FormatDescription) >= 0,
                    "Decoder.FormatDescription contains " + FormatDescription);
                Assert.True(decoder.MimeType.IndexOf(MimeType) >= 0,
                    "Decoder.MimeType contains " + MimeType);

                Assert.Equal(signatureLength, decoder.SignatureMasks.Length);
                for (int i = 0; i < signatureLength; i++)
                {
                    Assert.Equal(mask, BitConverter.ToString(decoder.SignatureMasks[i]));
                }
                Assert.Equal(signatureLength, decoder.SignaturePatterns.Length);
                Assert.Equal(pattern, BitConverter.ToString(decoder.SignaturePatterns[0]));
                if (pattern2 != null)
                    Assert.Equal(pattern2, BitConverter.ToString(decoder.SignaturePatterns[1]));
            }
        }

        [Fact]
        public void Decoders()
        {
            Assert.Equal(8, decoders.Count);
            foreach (DictionaryEntry de in decoders)
            {
                string guid = de.Key.ToString();
                switch (guid)
                {
                    case GIF_CSID:
                    case EMF_CSID:
                    case BMP_DIB_RLE_CSID:
                    case JPG_JPEG_JPE_JFIF_CSID:
                    case PNG_CSID:
                    case ICO_CSID:
                    case WMF_CSID:
                    case TIF_CSID:
                        break;
                    default:
                        break;
                }
            }
        }

        [Fact]
        public void Encoders()
        {
            Assert.Equal(5, encoders.Count);
            foreach (DictionaryEntry de in encoders)
            {
                string guid = de.Key.ToString();
                switch (guid)
                {
                    case GIF_CSID:
                    case BMP_DIB_RLE_CSID:
                    case JPG_JPEG_JPE_JFIF_CSID:
                    case PNG_CSID:
                    case TIF_CSID:
                        break;
                    default:
                        break;
                }
            }
        }

        [Fact]
        public void BMPCodec()
        {
            Check("557cf400-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Bmp,
                "BMP", null, "*.BMP",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "BMP", "image/bmp", 1, 1, "FF-FF", "42-4D", null);
        }

        [Fact]
        public void GifCodec()
        {
            Check("557cf402-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Gif,
                "GIF", null, "*.GIF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "GIF", "image/gif", 1, 2, "FF-FF-FF-FF-FF-FF", "47-49-46-38-39-61", "47-49-46-38-37-61");
        }

        [Fact]
        public void JpegCodec()
        {
            Check("557cf401-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Jpeg,
                "JPEG", null, "*.JPG",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "JPEG", "image/jpeg", 1, 1, "FF-FF", "FF-D8", null);
        }

        [Fact]
        public void PngCodec()
        {
            Check("557cf406-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Png,
                "PNG", null, "*.PNG",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "PNG", "image/png", 1, 1, "FF-FF-FF-FF-FF-FF-FF-FF", "89-50-4E-47-0D-0A-1A-0A", null);
        }

        [Fact]
        public void TiffCodec()
        {
            Check("557cf405-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Tiff,
                "TIFF", null, "*.TIF;*.TIFF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "TIFF", "image/tiff", 1, 2, "FF-FF", "49-49", "4D-4D");
        }

        [Fact]
        public void IconCodec_Decoder()
        {
            Check("557cf407-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Icon,
                "ICO", null, "*.ICO",
                ImageCodecFlags.Builtin | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "ICO", "image/x-icon", 1, 1, "FF-FF-FF-FF", "00-00-01-00", null);
        }

        [Fact]
        public void EmfCodec_Decoder()
        {
            Check("557cf403-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Emf,
                "EMF", null, "*.EMF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "EMF", "image/x-emf", 1, 1, "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-FF-FF-FF-FF",
                "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-20-45-4D-46", null);
        }

        [Fact]
        public void WmfCodec_Decoder()
        {
            Check("557cf404-1a04-11d3-9a73-0000f81ef32e", ImageFormat.Wmf,
                "WMF", null, "*.WMF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "WMF", "image/x-wmf", 1, 1, "FF-FF-FF-FF", "D7-CD-C6-9A", null);
        }

        [Fact]
        public void IconCodec_Encoder()
        {
            Guid g = new Guid("557cf407-1a04-11d3-9a73-0000f81ef32e");
            Assert.Null(GetEncoder(g));
        }

        [Fact]
        public void EmfCodec_Encoder()
        {
            Guid g = new Guid("557cf403-1a04-11d3-9a73-0000f81ef32e");
            Assert.Null(GetEncoder(g));
        }

        [Fact]
        public void WmfCodec_Encoder()
        {
            Guid g = new Guid("557cf404-1a04-11d3-9a73-0000f81ef32e");
            Assert.Null(GetEncoder(g));
        }
    }
}
