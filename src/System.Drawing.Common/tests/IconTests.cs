﻿// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Copyright (C) 2004,2006-2008 Novell, Inc (http://www.novell.com)
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.Drawing.Tests
{
    public class IconTests : RemoteExecutorTestBase
    {
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData("48x48_multiple_entries_4bit.ico")]
        [InlineData("256x256_seven_entries_multiple_bits.ico")]
        [InlineData("pngwithheight_icon.ico")]
        public void Ctor_FilePath(string name)
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath(name)))
            {
                Assert.Equal(32, icon.Width);
                Assert.Equal(32, icon.Height);
                Assert.Equal(new Size(32, 32), icon.Size);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void Unix_OverflowException_CorruptIcon()
        {
            Assert.Throws<OverflowException>(() => new Icon(Helpers.GetTestBitmapPath("overflowicon.ico")));
        }

        public static IEnumerable<object[]> Size_TestData()
        {
            // Normal size
            yield return new object[] { "48x48_multiple_entries_4bit.ico", new Size(16, 16), new Size(16, 16) };
            yield return new object[] { "48x48_multiple_entries_4bit.ico", new Size(-32, -32), new Size(16, 16) };
            yield return new object[] { "48x48_multiple_entries_4bit.ico", new Size(32, 16), new Size(32, 32) };
            yield return new object[] { "256x256_seven_entries_multiple_bits.ico", new Size(48, 48), new Size(48, 48) };
            yield return new object[] { "256x256_seven_entries_multiple_bits.ico", new Size(0, 0), new Size(32, 32) };
            yield return new object[] { "256x256_seven_entries_multiple_bits.ico", new Size(1, 1), new Size(256, 256) };

            // Unusual size
            yield return new object[] { "10x16_one_entry_32bit.ico", new Size(16, 16), new Size(10, 16) };
            yield return new object[] { "10x16_one_entry_32bit.ico", new Size(32, 32), new Size(11, 22) };

            // Only 256
            yield return new object[] { "256x256_one_entry_32bit.ico", new Size(0, 0), new Size(256, 256) };

            yield return new object[] { "256x256_one_entry_32bit.ico", new Size(int.MaxValue, int.MaxValue), new Size(256, 256) };
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Size_TestData))]
        public void Ctor_FilePath_Width_Height(string fileName, Size size, Size expectedSize)
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath(fileName), size.Width, size.Height))
            {
                Assert.Equal(expectedSize.Width, icon.Width);
                Assert.Equal(expectedSize.Height, icon.Height);
                Assert.Equal(expectedSize, icon.Size);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Size_TestData))]
        public void Ctor_FilePath_Size(string fileName, Size size, Size expectedSize)
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath(fileName), size))
            {
                Assert.Equal(expectedSize.Width, icon.Width);
                Assert.Equal(expectedSize.Height, icon.Height);
                Assert.Equal(expectedSize, icon.Size);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_NullFilePath_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("path", () => new Icon((string)null));
            AssertExtensions.Throws<ArgumentNullException>("path", () => new Icon((string)null, new Size(32, 32)));
            AssertExtensions.Throws<ArgumentNullException>("path", () => new Icon((string)null, 32, 32));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_Stream()
        {
            using (var stream = File.OpenRead(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            {
                var icon = new Icon(stream);
                Assert.Equal(32, icon.Width);
                Assert.Equal(32, icon.Height);
                Assert.Equal(new Size(32, 32), icon.Size);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Size_TestData))]
        public void Ctor_Stream_Width_Height(string fileName, Size size, Size expectedSize)
        {
            using (var stream = File.OpenRead(Helpers.GetTestBitmapPath(fileName)))
            using (var icon = new Icon(stream, size.Width, size.Height))
            {
                Assert.Equal(expectedSize.Width, icon.Width);
                Assert.Equal(expectedSize.Height, icon.Height);
                Assert.Equal(expectedSize, icon.Size);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Size_TestData))]
        public void Ctor_Stream_Size(string fileName, Size size, Size expectedSize)
        {
            using (var stream = File.OpenRead(Helpers.GetTestBitmapPath(fileName)))
            using (var icon = new Icon(stream, size))
            {
                Assert.Equal(expectedSize.Width, icon.Width);
                Assert.Equal(expectedSize.Height, icon.Height);
                Assert.Equal(expectedSize, icon.Size);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_NullStream_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException, ArgumentException>("stream", null, () => new Icon((Stream)null));
            AssertExtensions.Throws<ArgumentNullException, ArgumentException>("stream", null, () => new Icon((Stream)null, 32, 32));
            AssertExtensions.Throws<ArgumentNullException, ArgumentException>("stream", null, () => new Icon((Stream)null, new Size(32, 32)));
        }

        public static IEnumerable<object[]> Ctor_InvalidBytesInStream_TestData()
        {
            // No start entry.
            yield return new object[] { new byte[0], typeof(ArgumentException) };
            yield return new object[] { new byte[6], typeof(ArgumentException) };
            yield return new object[] { new byte[21], typeof(ArgumentException) };

            // First two reserved bits are not zero.
            yield return new object[] { new byte[] { 10, 0, 1, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };
            yield return new object[] { new byte[] { 0, 10, 1, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };

            // The type is not one.
            yield return new object[] { new byte[] { 0, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };
            yield return new object[] { new byte[] { 0, 0, 2, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };
            yield return new object[] { new byte[] { 0, 0, 1, 2, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };

            // The count is zero.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };

            // No space for the number of entries specified.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(ArgumentException) };

            // The number of entries specified is negative.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(Win32Exception) };

            // The size of an entry is negative.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255, 0, 0, 0, 0 }, typeof(Win32Exception) };

            // The offset of an entry is negative.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255 }, typeof(ArgumentException) };

            // The size and offset of an entry refers to an invalid position in the list of entries.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 0, 0, 0, 12, 0, 0, 0 }, typeof(ArgumentException) };

            // The size and offset of an entry overflows.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 127, 255, 255, 255, 127 }, typeof(Win32Exception) };

            // The offset and the size of the list of entries overflows.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 127 }, typeof(ArgumentException) };

            // No handle can be created from this.
            yield return new object[] { new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, typeof(Win32Exception) };
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Ctor_InvalidBytesInStream_TestData))]
        public void Ctor_InvalidBytesInStream_ThrowsException(byte[] bytes, Type exceptionType)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);

                stream.Position = 0;
                Assert.Throws(exceptionType, () => new Icon(stream));
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Size_TestData))]
        public void Ctor_Icon_Width_Height(string fileName, Size size, Size expectedSize)
        {
            using (var sourceIcon = new Icon(Helpers.GetTestBitmapPath(fileName)))
            using (var icon = new Icon(sourceIcon, size.Width, size.Height))
            {
                Assert.Equal(expectedSize.Width, icon.Width);
                Assert.Equal(expectedSize.Height, icon.Height);
                Assert.Equal(expectedSize, icon.Size);
                Assert.NotSame(sourceIcon.Handle, icon.Handle);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(Size_TestData))]
        public void Ctor_Icon_Size(string fileName, Size size, Size expectedSize)
        {
            using (var sourceIcon = new Icon(Helpers.GetTestBitmapPath(fileName)))
            using (var icon = new Icon(sourceIcon, size))
            {
                Assert.Equal(expectedSize.Width, icon.Width);
                Assert.Equal(expectedSize.Height, icon.Height);
                Assert.Equal(expectedSize, icon.Size);
                Assert.NotSame(sourceIcon.Handle, icon.Handle);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_NullIcon_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException, ArgumentException>("original", null, () => new Icon((Icon)null, 32, 32));
            AssertExtensions.Throws<ArgumentNullException, ArgumentException>("original", null, () => new Icon((Icon)null, new Size(32, 32)));
        }

        // libgdiplus causes a segfault when given an invalid Icon handle.
        [PlatformSpecific(TestPlatforms.Windows)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_InvalidHandle_Success()
        {
            using (Icon icon = Icon.FromHandle((IntPtr)1))
            using (var stream = new MemoryStream())
            {
                Exception ex = Assert.ThrowsAny<Exception>(() => icon.Save(stream));
                Assert.True(ex is COMException || ex is ObjectDisposedException, $"{ex.GetType().ToString()} was thrown.");

                AssertExtensions.Throws<ArgumentException>(null, () => icon.ToBitmap());
                Assert.Equal(Size.Empty, icon.Size);

                using (var newIcon = new Icon(icon, 10, 10))
                {
                    Assert.Throws<ObjectDisposedException>(() => newIcon.Handle);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_Type_Resource()
        {
            using (var icon = new Icon(typeof(IconTests), "48x48_multiple_entries_4bit.ico"))
            {
                Assert.Equal(32, icon.Height);
                Assert.Equal(32, icon.Width);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_NullType_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => new Icon(null, "48x48_multiple_entries_4bit.ico"));
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData(typeof(Icon), null)]
        [InlineData(typeof(Icon), "")]
        [InlineData(typeof(Icon), "48x48_multiple_entries_4bit.ico")]
        [InlineData(typeof(IconTests), "48x48_MULTIPLE_entries_4bit.ico")]
        public void Ctor_InvalidResource_ThrowsArgumentException(Type type, string resource)
        {
            AssertExtensions.Throws<ArgumentException>(null, () => new Icon(type, resource));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Clone_ConstructedIcon_Success()
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            using (Icon clone = Assert.IsType<Icon>(icon.Clone()))
            {
                Assert.NotSame(icon, clone);
                Assert.NotSame(icon.Handle, clone.Handle);
                Assert.Equal(32, clone.Width);
                Assert.Equal(32, clone.Height);
                Assert.Equal(new Size(32, 32), clone.Size);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Clone_IconFromHandle_Success()
        {
            using (var icon = Icon.FromHandle(SystemIcons.Hand.Handle))
            using (Icon clone = Assert.IsType<Icon>(icon.Clone()))
            {
                Assert.NotSame(icon, clone);
                Assert.NotSame(icon.Handle, clone.Handle);
                Assert.Equal(SystemIcons.Hand.Width, clone.Width);
                Assert.Equal(SystemIcons.Hand.Height, clone.Height);
                Assert.Equal(SystemIcons.Hand.Size, clone.Size);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_IconData_DestroysHandle()
        {
            var icon = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico"));
            icon.Dispose();
            Assert.Throws<ObjectDisposedException>(() => icon.Handle);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_OwnsHandle_DestroysHandle()
        {
            Icon icon = Icon.ExtractAssociatedIcon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico"));
            icon.Dispose();

            Assert.Throws<ObjectDisposedException>(() => icon.Handle);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Dispose_DoesNotOwnHandle_DoesNotDestroyHandle()
        {
            using (var source = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            using (var icon = Icon.FromHandle(source.Handle))
            {
                IntPtr handle = icon.Handle;
                Assert.NotEqual(IntPtr.Zero, handle);

                icon.Dispose();
                Assert.Equal(handle, icon.Handle);
            }
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(48)]
        public void XpIcon_ToBitmap_Success(int size)
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_32bit.ico"), size, size))
            {
                Assert.Equal(size, icon.Width);
                Assert.Equal(size, icon.Height);
                Assert.Equal(new Size(size, size), icon.Size);

                using (Bitmap bitmap = icon.ToBitmap())
                {
                    Assert.Equal(size, bitmap.Width);
                    Assert.Equal(size, bitmap.Height);
                    Assert.Equal(new Size(size, size), bitmap.Size);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ExtractAssociatedIcon_FilePath_Success()
        {
            using (Icon icon = Icon.ExtractAssociatedIcon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            {
                Assert.Equal(32, icon.Width);
                Assert.Equal(32, icon.Height);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ExtractAssociatedIcon_NonFilePath_ReturnsNull()
        {
            Assert.Null(Icon.ExtractAssociatedIcon("http://microsoft.com"));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ExtractAssociatedIcon_NullFilePath_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException, ArgumentException>("filePath", null, () => Icon.ExtractAssociatedIcon(null));
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData("", "path")]
        [InlineData("\\\\uncpath", null)]
        public void ExtractAssociatedIcon_InvalidFilePath_ThrowsArgumentException(string filePath, string paramName)
        {
            AssertExtensions.Throws<ArgumentException>(paramName, null, () => Icon.ExtractAssociatedIcon(filePath));
        }


        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ExtractAssociatedIcon_NoSuchPath_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => Icon.ExtractAssociatedIcon("no-such-file.png"));
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData("16x16_one_entry_4bit.ico")]
        [InlineData("32x32_one_entry_4bit.ico")]
        [InlineData("48x48_one_entry_1bit.ico")]
        [InlineData("64x64_one_entry_8bit.ico")]
        [InlineData("96x96_one_entry_8bit.ico")]
        [InlineData("256x256_seven_entries_multiple_bits.ico")]
        public void Save_OutputStream_Success(string fileName)
        {
            SaveAndCompare(new Icon(Helpers.GetTestBitmapPath(fileName)), true);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_OutputStream_ProducesIdenticalBytes()
        {
            string filePath = Helpers.GetTestBitmapPath("256x256_seven_entries_multiple_bits.ico");
            using (var icon = new Icon(filePath))
            using (var outputStream = new MemoryStream())
            {
                icon.Save(outputStream);
                Assert.Equal(File.ReadAllBytes(filePath), outputStream.ToArray());
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_HasIconDataAndDisposed_ProducesIdenticalBytes()
        {
            string filePath = Helpers.GetTestBitmapPath("256x256_seven_entries_multiple_bits.ico");
            var icon = new Icon(filePath);
            icon.Dispose();
            using (var outputStream = new MemoryStream())
            {
                icon.Save(outputStream);
                Assert.Equal(File.ReadAllBytes(filePath), outputStream.ToArray());
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_NullOutputStreamIconData_ThrowsNullReferenceException()
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            {
                Assert.Throws<NullReferenceException>(() => icon.Save(null));
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_NullOutputStreamNoIconData_ThrowsArgumentNullException()
        {
            using (var source = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            {
                var icon = Icon.FromHandle(source.Handle);
                icon.Dispose();

                AssertExtensions.Throws<ArgumentNullException>("dataStream", () => icon.Save(null));
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_ClosedOutputStreamIconData_ThrowsException()
        {
            using (var icon = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            {
                var stream = new MemoryStream();
                stream.Close();

                Assert.Throws<ObjectDisposedException>(() => icon.Save(stream));
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_ClosedOutputStreamNoIconData_DoesNothing()
        {
            using (var source = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico")))
            using (var icon = Icon.FromHandle(source.Handle))
            {
                var stream = new MemoryStream();
                stream.Close();

                icon.Save(stream);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Save_NoIconDataOwnsHandleAndDisposed_ThrowsObjectDisposedException()
        {
            Icon icon = Icon.ExtractAssociatedIcon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico"));
            icon.Dispose();

            Assert.Throws<ObjectDisposedException>(() => icon.Save(new MemoryStream()));
        }

        public static IEnumerable<object[]> ToBitmap_TestData()
        {
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico")) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("32x32_one_entry_4bit.ico")) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("48x48_one_entry_1bit.ico")) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("64x64_one_entry_8bit.ico")) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("96x96_one_entry_8bit.ico")) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("256x256_two_entries_multiple_bits.ico"), 48, 48) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("256x256_two_entries_multiple_bits.ico"), 256, 256) };
            yield return new object[] { new Icon(Helpers.GetTestBitmapPath("256x256_two_entries_multiple_bits.ico"), 0, 0) };
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(ToBitmap_TestData))]
        public void ToBitmap_BitmapIcon_ReturnsExpected(Icon icon)
        {
            try
            {
                using (Bitmap bitmap = icon.ToBitmap())
                {
                    Assert.NotSame(icon.ToBitmap(), bitmap);
                    Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
                    Assert.Empty(bitmap.Palette.Entries);
                    Assert.Equal(icon.Width, bitmap.Width);
                    Assert.Equal(icon.Height, bitmap.Height);

                    Assert.Equal(ImageFormat.MemoryBmp, bitmap.RawFormat);
                    Assert.Equal(2, bitmap.Flags);
                }
            }
            finally
            {
                icon.Dispose();
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToBitmap_BitmapIconFromHandle_ReturnsExpected()
        {
            // Handle refers to an icon without any colour. This is not in ToBitmap_TestData as there is
            // a chance that the original icon will be finalized as it is not kept alive in the iterator.
            using (var originalIcon = new Icon(Helpers.GetTestBitmapPath("48x48_one_entry_1bit.ico")))
            using (Icon icon = Icon.FromHandle(originalIcon.Handle))
            {
                ToBitmap_BitmapIcon_ReturnsExpected(icon);
            }
        }

        private const string DontSupportPngFramesInIcons = "Switch.System.Drawing.DontSupportPngFramesInIcons";

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToBitmap_PngIconSupportedInSwitches_Success()
        {
            void VerifyPng()
            {
                using (Icon icon = GetPngIcon())
                using (Bitmap bitmap = icon.ToBitmap())
                {
                    using (Bitmap secondBitmap = icon.ToBitmap())
                    {
                        Assert.NotSame(icon.ToBitmap(), bitmap);
                    }
                    Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
                    Assert.Empty(bitmap.Palette.Entries);
                    Assert.Equal(icon.Width, bitmap.Width);
                    Assert.Equal(icon.Height, bitmap.Height);

                    Assert.Equal(ImageFormat.Png, bitmap.RawFormat);
                    Assert.Equal(77842, bitmap.Flags);
                }
            }

            if (!AppContext.TryGetSwitch(DontSupportPngFramesInIcons, out bool isEnabled) || isEnabled)
            {
                RemoteInvoke(() =>
                {
                    AppContext.SetSwitch(DontSupportPngFramesInIcons, false);
                    VerifyPng();
                    return SuccessExitCode;
                }).Dispose();
            }
            else
            {
                VerifyPng();
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToBitmap_PngIconNotSupportedInSwitches_ThrowsArgumentOutOfRangeException()
        {
            void VerifyPngNotSupported()
            {
                using (Icon icon = GetPngIcon())
                {
                    AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => icon.ToBitmap());
                }
            }

            if (!AppContext.TryGetSwitch(DontSupportPngFramesInIcons, out bool isEnabled) || !isEnabled)
            {
                RemoteInvoke(() =>
                {
                    AppContext.SetSwitch(DontSupportPngFramesInIcons, true);
                    VerifyPngNotSupported();
                    return SuccessExitCode;
                }).Dispose();
            }
            else
            {
                VerifyPngNotSupported();
            }
        }

        private static Icon GetPngIcon()
        {
            using (var stream = new MemoryStream())
            {
                // Create a PNG inside an ICO.
                using (var bitmap = new Bitmap(10, 10))
                {
                    stream.Write(new byte[] { 0, 0, 1, 0, 1, 0, (byte)bitmap.Width, (byte)bitmap.Height, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 22, 0, 0, 0 }, 0, 22);

                    // Writing actual data
                    bitmap.Save(stream, ImageFormat.Png);
                }

                // Getting data length (file length minus header)
                long length = stream.Length - 22;
                stream.Seek(14, SeekOrigin.Begin);
                stream.WriteByte((byte)length);
                stream.WriteByte((byte)(length >> 8));

                // Read the PNG inside an ICO.
                stream.Position = 0;
                return new Icon(stream);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHandle_IconHandleOneTime_Success()
        {
            using (var icon1 = new Icon(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico")))
            using (Icon icon2 = Icon.FromHandle(icon1.Handle))
            {
                Assert.Equal(icon1.Handle, icon2.Handle);
                Assert.Equal(icon1.Size, icon2.Size);
                SaveAndCompare(icon2, false);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHandle_IconHandleMultipleTime_Success()
        {
            using (var icon1 = new Icon(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico")))
            {
                using (Icon icon2 = Icon.FromHandle(icon1.Handle))
                {
                    Assert.Equal(icon1.Handle, icon2.Handle);
                    Assert.Equal(icon1.Size, icon2.Size);
                    SaveAndCompare(icon2, false);
                }
                using (Icon icon3 = Icon.FromHandle(icon1.Handle))
                {
                    Assert.Equal(icon1.Handle, icon3.Handle);
                    Assert.Equal(icon1.Size, icon3.Size);
                    SaveAndCompare(icon3, false);
                }
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHandle_BitmapHandleOneTime_Success()
        {
            IntPtr handle;
            using (var icon1 = new Icon(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico")))
            {
                handle = icon1.ToBitmap().GetHicon();
            }
            using (Icon icon2 = Icon.FromHandle(handle))
            {
                Assert.Equal(handle, icon2.Handle);
                Assert.Equal(new Size(16, 16), icon2.Size);
                SaveAndCompare(icon2, false);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHandle_BitmapHandleMultipleTime_Success()
        {
            IntPtr handle;
            using (var icon1 = new Icon(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico")))
            {
                handle = icon1.ToBitmap().GetHicon();
            }
            using (Icon icon2 = Icon.FromHandle(handle))
            {
                Assert.Equal(handle, icon2.Handle);
                Assert.Equal(new Size(16, 16), icon2.Size);
                SaveAndCompare(icon2, false);
            }
            using (Icon icon3 = Icon.FromHandle(handle))
            {
                Assert.Equal(handle, icon3.Handle);
                Assert.Equal(new Size(16, 16), icon3.Size);
                SaveAndCompare(icon3, false);
            }
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void FromHandle_Zero_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>(null, () => Icon.FromHandle(IntPtr.Zero));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Size_GetWhenDisposed_ThrowsObjectDisposedException()
        {
            var icon = new Icon(Helpers.GetTestBitmapPath("48x48_multiple_entries_4bit.ico"));
            icon.Dispose();

            Assert.Throws<ObjectDisposedException>(() => icon.Width);
            Assert.Throws<ObjectDisposedException>(() => icon.Height);
            Assert.Throws<ObjectDisposedException>(() => icon.Size);
        }

        private static void SaveAndCompare(Icon icon, bool alpha)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                icon.Save(outputStream);
                outputStream.Position = 0;

                using (Icon loaded = new Icon(outputStream))
                {
                    Assert.Equal(icon.Height, loaded.Height);
                    Assert.Equal(icon.Width, loaded.Width);

                    using (Bitmap expected = icon.ToBitmap())
                    using (Bitmap actual = loaded.ToBitmap())
                    {
                        Assert.Equal(expected.Height, actual.Height);
                        Assert.Equal(expected.Width, actual.Width);

                        for (int y = 0; y < expected.Height; y++)
                        {
                            for (int x = 0; x < expected.Width; x++)
                            {
                                Color e = expected.GetPixel(x, y);
                                Color a = actual.GetPixel(x, y);
                                if (alpha)
                                {
                                    Assert.Equal(e.A, a.A);
                                }
                                Assert.Equal(e.R, a.R);
                                Assert.Equal(e.G, a.G);
                                Assert.Equal(e.B, a.B);
                            }
                        }
                    }
                }
            }
        }
    }
}
