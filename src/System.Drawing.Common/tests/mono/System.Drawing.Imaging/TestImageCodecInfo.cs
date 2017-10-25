// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// ImageCodecInfo class testing unit
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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Xunit;
using System.Collections;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace MonoTests.System.Drawing.Imaging
{

    public class ImageCodecInfoTest
    {

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

        public ImageCodecInfoTest()
        {
            decoders = new Hashtable();
            encoders = new Hashtable();

            foreach (ImageCodecInfo decoder in ImageCodecInfo.GetImageDecoders())
                decoders[decoder.Clsid] = decoder;

            foreach (ImageCodecInfo encoder in ImageCodecInfo.GetImageEncoders())
                encoders[encoder.Clsid] = encoder;
        }

        static void Check(ImageCodecInfo e, ImageCodecInfo d, Guid FormatID, string CodecName, string DllName,
            string FilenameExtension, ImageCodecFlags Flags, string FormatDescription,
            string MimeType, int Version, int signatureLength, string mask, string pattern, string pattern2)
        {
            Regex extRegex = new Regex(@"^(\*\.\w+(;(\*\.\w+))*;)?" +
                Regex.Escape(FilenameExtension) + @"(;\*\.\w+(;(\*\.\w+))*)?$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (e != null)
            {
                Assert.Equal(FormatID, e.FormatID);
                Assert.True(e.CodecName.IndexOf(CodecName) >= 0,
                    "Encoder.CodecName contains " + CodecName);
                Assert.Equal(DllName, e.DllName);
                Assert.True(extRegex.IsMatch(e.FilenameExtension),
                    "Encoder.FilenameExtension is a right list with " + FilenameExtension);
                Assert.Equal(Flags, e.Flags);
                Assert.True(e.FormatDescription.IndexOf(FormatDescription) >= 0,
                    "Encoder.FormatDescription contains " + FormatDescription);
                Assert.True(e.MimeType.IndexOf(MimeType) >= 0,
                    "Encoder.MimeType contains " + MimeType);

                Assert.Equal(signatureLength, e.SignatureMasks.Length);
                for (int i = 0; i < signatureLength; i++)
                {
                    Assert.Equal(mask, BitConverter.ToString(e.SignatureMasks[i]));
                }
                Assert.Equal(signatureLength, e.SignaturePatterns.Length);
                Assert.Equal(pattern, BitConverter.ToString(e.SignaturePatterns[0]));
                if (pattern2 != null)
                    Assert.Equal(pattern2, BitConverter.ToString(e.SignaturePatterns[1]));
            }
            if (d != null)
            {
                Assert.Equal(FormatID, d.FormatID);
                Assert.True(d.CodecName.IndexOf(CodecName) >= 0,
                    "Decoder.CodecName contains " + CodecName);
                Assert.Equal(DllName, d.DllName);
                Assert.True(extRegex.IsMatch(d.FilenameExtension),
                    "Decoder.FilenameExtension is a right list with " + FilenameExtension);
                Assert.Equal(Flags, d.Flags);
                Assert.True(d.FormatDescription.IndexOf(FormatDescription) >= 0,
                    "Decoder.FormatDescription contains " + FormatDescription);
                Assert.True(d.MimeType.IndexOf(MimeType) >= 0,
                    "Decoder.MimeType contains " + MimeType);

                Assert.Equal(signatureLength, d.SignatureMasks.Length);
                for (int i = 0; i < signatureLength; i++)
                {
                    Assert.Equal(mask, BitConverter.ToString(d.SignatureMasks[i]));
                }
                Assert.Equal(signatureLength, d.SignaturePatterns.Length);
                Assert.Equal(pattern, BitConverter.ToString(d.SignaturePatterns[0]));
                if (pattern2 != null)
                    Assert.Equal(pattern2, BitConverter.ToString(d.SignaturePatterns[1]));
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Decoders()
        {
            Assert.Equal(8, decoders.Count);
            foreach (DictionaryEntry de in decoders)
            {
                string guid = de.Key.ToString();
                switch (guid)
                {
                    case "557cf402-1a04-11d3-9a73-0000f81ef32e": // GIF
                    case "557cf403-1a04-11d3-9a73-0000f81ef32e": // EMF
                    case "557cf400-1a04-11d3-9a73-0000f81ef32e": // BMP/DIB/RLE
                    case "557cf401-1a04-11d3-9a73-0000f81ef32e": // JPG,JPEG,JPE,JFIF
                    case "557cf406-1a04-11d3-9a73-0000f81ef32e": // PNG
                    case "557cf407-1a04-11d3-9a73-0000f81ef32e": // ICO
                    case "557cf404-1a04-11d3-9a73-0000f81ef32e": // WMF
                    case "557cf405-1a04-11d3-9a73-0000f81ef32e": // TIF,TIFF
                        break;
                    default:
                        Assert.True(false, "Unknown decoder " + guid);
                        break;
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Encoders()
        {
            Assert.Equal(5, encoders.Count);
            foreach (DictionaryEntry de in encoders)
            {
                string guid = de.Key.ToString();
                switch (guid)
                {
                    case "557cf402-1a04-11d3-9a73-0000f81ef32e": // GIF
                    case "557cf400-1a04-11d3-9a73-0000f81ef32e": // BMP/DIB/RLE
                    case "557cf401-1a04-11d3-9a73-0000f81ef32e": // JPG,JPEG,JPE,JFIF
                    case "557cf406-1a04-11d3-9a73-0000f81ef32e": // PNG
                    case "557cf405-1a04-11d3-9a73-0000f81ef32e": // TIF,TIFF
                        break;
                    default:
                        Assert.True(false, "Unknown encoder " + guid);
                        break;
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void BMPCodec()
        {
            Guid g = new Guid("557cf400-1a04-11d3-9a73-0000f81ef32e");
            Check(GetEncoder(g), GetDecoder(g), ImageFormat.Bmp.Guid,
                "BMP", null, "*.BMP",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "BMP", "image/bmp", 1, 1, "FF-FF", "42-4D", null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GifCodec()
        {
            Guid g = new Guid("557cf402-1a04-11d3-9a73-0000f81ef32e");
            Check(GetEncoder(g), GetDecoder(g), ImageFormat.Gif.Guid,
                "GIF", null, "*.GIF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "GIF", "image/gif", 1, 2, "FF-FF-FF-FF-FF-FF", "47-49-46-38-39-61", "47-49-46-38-37-61");
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void JpegCodec()
        {
            Guid g = new Guid("557cf401-1a04-11d3-9a73-0000f81ef32e");
            Check(GetEncoder(g), GetDecoder(g), ImageFormat.Jpeg.Guid,
                "JPEG", null, "*.JPG",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "JPEG", "image/jpeg", 1, 1, "FF-FF", "FF-D8", null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void PngCodec()
        {
            Guid g = new Guid("557cf406-1a04-11d3-9a73-0000f81ef32e");
            Check(GetEncoder(g), GetDecoder(g), ImageFormat.Png.Guid,
                "PNG", null, "*.PNG",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "PNG", "image/png", 1, 1, "FF-FF-FF-FF-FF-FF-FF-FF", "89-50-4E-47-0D-0A-1A-0A", null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TiffCodec()
        {
            Guid g = new Guid("557cf405-1a04-11d3-9a73-0000f81ef32e");
            Check(GetEncoder(g), GetDecoder(g), ImageFormat.Tiff.Guid,
                "TIFF", null, "*.TIF;*.TIFF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "TIFF", "image/tiff", 1, 2, "FF-FF", "49-49", "4D-4D");
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IconCodec_Encoder()
        {
            Guid g = new Guid("557cf407-1a04-11d3-9a73-0000f81ef32e");
            Assert.Null(GetEncoder(g));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IconCodec_Decoder()
        {
            Guid g = new Guid("557cf407-1a04-11d3-9a73-0000f81ef32e");
            Check(null, GetDecoder(g), ImageFormat.Icon.Guid,
                "ICO", null, "*.ICO",
                ImageCodecFlags.Builtin | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "ICO", "image/x-icon", 1, 1, "FF-FF-FF-FF", "00-00-01-00", null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void EmfCodec_Encoder()
        {
            Guid g = new Guid("557cf403-1a04-11d3-9a73-0000f81ef32e");
            Assert.Null(GetEncoder(g));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void EmfCodec_Decoder()
        {
            Guid g = new Guid("557cf403-1a04-11d3-9a73-0000f81ef32e");
            Check(null, GetDecoder(g), ImageFormat.Emf.Guid,
                "EMF", null, "*.EMF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "EMF", "image/x-emf", 1, 1, "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-FF-FF-FF-FF",
                "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-20-45-4D-46", null);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WmfCodec_Encoder()
        {
            Guid g = new Guid("557cf404-1a04-11d3-9a73-0000f81ef32e");
            Assert.Null(GetEncoder(g));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void WmfCodec_Decoder()
        {
            Guid g = new Guid("557cf404-1a04-11d3-9a73-0000f81ef32e");
            Check(null, GetDecoder(g), ImageFormat.Wmf.Guid,
                "WMF", null, "*.WMF",
                ImageCodecFlags.Builtin | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap,
                "WMF", "image/x-wmf", 1, 1, "FF-FF-FF-FF", "D7-CD-C6-9A", null);
        }
    }
}
