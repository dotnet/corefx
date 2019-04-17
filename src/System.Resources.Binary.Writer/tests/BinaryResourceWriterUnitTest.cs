// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.Resources.Binary.Writer.Tests
{
    public class BinaryResourceWriterTests
    {
        [Fact]
        public static void ExceptionforNullStream()
        {
            Assert.Throws<ArgumentNullException>("stream", () => new BinaryResourceWriter((Stream)null));
        }
        [Fact]
        public static void ExceptionforNullFile()
        {
            Assert.Throws<ArgumentNullException>("fileName", () => new BinaryResourceWriter((string)null));
        }

        [Fact]
        public static void ExceptionforReadOnlyStream()
        {
            AssertExtensions.Throws<ArgumentException>(null, () =>
            {
                using (var readOnlyStream = new MemoryStream(new byte[1], false))
                {
                    new BinaryResourceWriter(readOnlyStream);
                }
            });
        }

        [Fact]
        public static void ExceptionforNullResourceId()
        {
            using (var writer = new BinaryResourceWriter(new MemoryStream()))
            {
                Assert.Throws<ArgumentNullException>("name", () => writer.AddResource(null, "value"));
                Assert.Throws<ArgumentNullException>("name", () => writer.AddResource(null, new object()));
                Assert.Throws<ArgumentNullException>("name", () => writer.AddResource(null, new byte[0]));

                using (var stream = new MemoryStream())
                {
                    Assert.Throws<ArgumentNullException>("name", () => writer.AddResource(null, stream));
                    Assert.Throws<ArgumentNullException>("name", () => writer.AddResource(null, stream, true));
                    Assert.Throws<ArgumentNullException>("name", () => writer.AddStreamResource(null, "System.DayOfWeek", stream, false));
                }
                
                Assert.Throws<ArgumentNullException>("name", () => writer.AddBinaryFormattedResource(null, "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentNullException>("name", () => writer.AddTypeConverterResource(null, "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentNullException>("name", () => writer.AddTypeConverterResource(null, "System.DayOfWeek", "Monday"));
                Assert.Throws<ArgumentNullException>("name", () => writer.AddStreamResource(null, "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentNullException>("name", () => writer.AddResourceData(null, "System.DayOfWeek", new byte[1]));
            }
        }

        [Fact]
        public static void ExceptionforDuplicateKey()
        {
            using (var writer = new BinaryResourceWriter(new MemoryStream()))
            {
                writer.AddResource("duplicate", "value");

                Assert.Throws<ArgumentException>(null, () => writer.AddResource("duplicate", "value"));
                Assert.Throws<ArgumentException>(null, () => writer.AddResource("duplicate", new object()));
                Assert.Throws<ArgumentException>(null, () => writer.AddResource("duplicate", new byte[0]));

                using (var stream = new MemoryStream())
                {
                    Assert.Throws<ArgumentException>(null, () => writer.AddResource("duplicate", stream));
                    Assert.Throws<ArgumentException>(null, () => writer.AddResource("duplicate", stream, true));
                    Assert.Throws<ArgumentException>(null, () => writer.AddStreamResource("duplicate", "System.DayOfWeek", stream, false));
                }

                Assert.Throws<ArgumentException>(null, () => writer.AddBinaryFormattedResource("duplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentException>(null, () => writer.AddTypeConverterResource("duplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentException>(null, () => writer.AddTypeConverterResource("duplicate", "System.DayOfWeek", "Monday"));
                Assert.Throws<ArgumentException>(null, () => writer.AddStreamResource("duplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentException>(null, () => writer.AddResourceData("duplicate", "System.DayOfWeek", new byte[1]));


                Assert.Throws<ArgumentException>(null, () => writer.AddResource("Duplicate", "value"));
                Assert.Throws<ArgumentException>(null, () => writer.AddResource("dUplicate", new object()));
                Assert.Throws<ArgumentException>(null, () => writer.AddResource("duPlicate", new byte[0]));

                using (var stream = new MemoryStream())
                {
                    Assert.Throws<ArgumentException>(null, () => writer.AddResource("dupLicate", stream));
                    Assert.Throws<ArgumentException>(null, () => writer.AddResource("duplIcate", stream, true));
                    Assert.Throws<ArgumentException>(null, () => writer.AddStreamResource("dupliCate", "System.DayOfWeek", stream, false));
                }

                Assert.Throws<ArgumentException>(null, () => writer.AddBinaryFormattedResource("duplicAte", "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentException>(null, () => writer.AddTypeConverterResource("duplicaTe", "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentException>(null, () => writer.AddTypeConverterResource("duplicatE", "System.DayOfWeek", "Monday"));
                Assert.Throws<ArgumentException>(null, () => writer.AddStreamResource("DUplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<ArgumentException>(null, () => writer.AddResourceData("dUPlicate", "System.DayOfWeek", new byte[1]));
            }
        }

        [Fact]
        public static void ExceptionForAddAfterGenerate()
        {
            using (var writer = new BinaryResourceWriter(new MemoryStream()))
            {
                writer.AddResource("duplicate", "value");

                writer.Generate();

                Assert.Throws<InvalidOperationException>(() => writer.AddResource("duplicate", "value"));
                Assert.Throws<InvalidOperationException>(() => writer.AddResource("duplicate", new object()));
                Assert.Throws<InvalidOperationException>(() => writer.AddResource("duplicate", new byte[0]));

                using (var stream = new MemoryStream())
                {
                    Assert.Throws<InvalidOperationException>(() => writer.AddResource("duplicate", stream));
                    Assert.Throws<InvalidOperationException>(() => writer.AddResource("duplicate", stream, true));
                    Assert.Throws<InvalidOperationException>(() => writer.AddStreamResource("duplicate", "System.DayOfWeek", stream, false));
                }

                Assert.Throws<InvalidOperationException>(() => writer.AddBinaryFormattedResource("duplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<InvalidOperationException>(() => writer.AddTypeConverterResource("duplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<InvalidOperationException>(() => writer.AddTypeConverterResource("duplicate", "System.DayOfWeek", "Monday"));
                Assert.Throws<InvalidOperationException>(() => writer.AddStreamResource("duplicate", "System.DayOfWeek", new byte[1]));
                Assert.Throws<InvalidOperationException>(() => writer.AddResourceData("duplicate", "System.DayOfWeek", new byte[1]));
            }
        }

        [Fact]
        public static void EmptyResources()
        {
            byte[] writerBuffer, binaryWriterBuffer;
            using (MemoryStream ms = new MemoryStream())
            using (ResourceWriter writer = new ResourceWriter(ms))
            {
                writer.Generate();
                writerBuffer = ms.ToArray();
            }

            using (MemoryStream ms = new MemoryStream())
            using (BinaryResourceWriter writer = new BinaryResourceWriter(ms))
            {
                writer.Generate();
                binaryWriterBuffer = ms.ToArray();
            }

            Assert.Equal(writerBuffer, binaryWriterBuffer);
        }

        private static IEnumerable<object[]> GetPrimitiveResources()
        {
            yield return new object[]
            {
                new Dictionary<string, object>()
                {
                    {"string", "value" },
                    {"bool", true },
                    {"char", 'b' },
                    {"int", 42 },
                    {"byte", (byte)7 },
                    {"sbyte", (sbyte)-3 },
                    {"short", (short) 31000 },
                    {"ushort", (ushort) 61000 },
                    {"long", 10000000000 },
                    { "ulong", ulong.MaxValue },
                    {"float", 3.14f },
                    {"double", 3.14159 },
                    {"decimal", 3.141596536897931m }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetPrimitiveResources))]
        public static void PrimitiveResources(IDictionary<string, object> values)
        {
            Action<IResourceWriter> addData = (writer) =>
            {
                foreach (var pair in values)
                {
                    writer.AddResource(pair.Key, pair.Value);
                }
            };

            byte[] writerBuffer, binaryWriterBuffer;
            using (MemoryStream ms = new MemoryStream())
            using (ResourceWriter writer = new ResourceWriter(ms))
            {
                addData(writer);
                writer.Generate();
                writerBuffer = ms.ToArray();
            }

            using (MemoryStream ms = new MemoryStream())
            using (BinaryResourceWriter writer = new BinaryResourceWriter(ms))
            {
                addData(writer);
                writer.Generate();
                binaryWriterBuffer = ms.ToArray();
            }

            Assert.Equal(writerBuffer, binaryWriterBuffer);

            using (MemoryStream ms = new MemoryStream(binaryWriterBuffer, false))
            using (ResourceReader reader = new ResourceReader(ms))
            {
                IDictionaryEnumerator dictEnum = reader.GetEnumerator();

                while(dictEnum.MoveNext())
                {
                    Assert.Equal(values[(string)dictEnum.Key], dictEnum.Value);
                }
            }

            Assert.Throws<NotSupportedException>(() => new BinaryResourceReader(new MemoryStream(binaryWriterBuffer, false)));
        }

        [Fact]
        public static void BinaryFormattedResources()
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
                {
                    {"enum", DayOfWeek.Friday },
                    {"point", new Point(4, 8) }
                };

            if (PlatformDetection.IsDrawingSupported)
            {
                values.Add("bitmap", new Bitmap(Path.Combine("bitmaps", "almogaver24bits.bmp")));
                values.Add("font", SystemFonts.DefaultFont);
            }

            byte[] writerBuffer, binaryWriterBuffer;
            using (MemoryStream ms = new MemoryStream())
            using (ResourceWriter writer = new ResourceWriter(ms))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                foreach (var pair in values)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(memoryStream, pair.Value);
                        writer.AddResourceData(pair.Key, pair.Value.GetType().AssemblyQualifiedName, memoryStream.ToArray());
                    }
                }
                writer.Generate();
                writerBuffer = ms.ToArray();
            }

            using (MemoryStream ms = new MemoryStream())
            using (BinaryResourceWriter writer = new BinaryResourceWriter(ms))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                foreach (var pair in values)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(memoryStream, pair.Value);
                        writer.AddBinaryFormattedResource(pair.Key, pair.Value.GetType().AssemblyQualifiedName, memoryStream.ToArray());
                    }
                }
                writer.Generate();
                binaryWriterBuffer = ms.ToArray();
            }

            Assert.Equal(writerBuffer, binaryWriterBuffer);

            using (MemoryStream ms = new MemoryStream(writerBuffer, false))
            using (ResourceReader reader = new ResourceReader(ms))
            {
                typeof(ResourceReader).GetField("_permitDeserialization", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(reader, true);

                IDictionaryEnumerator dictEnum = reader.GetEnumerator();

                while (dictEnum.MoveNext())
                {
                    ResourceValueEquals(values[(string)dictEnum.Key], dictEnum.Value);
                }
            }

            Assert.Throws<NotSupportedException>(() => new BinaryResourceReader(new MemoryStream(binaryWriterBuffer, false)));
        }



        [Fact]
        public static void TypeConverterByteArrayResources()
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
                {
                    { "myResourceType", new MyResourceType(new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }) }
                };

            // ImageConverter only on windows at the moment https://github.com/dotnet/corefx/issues/36949
            if (PlatformDetection.IsDrawingSupported && PlatformDetection.IsWindows)  
            {
                values.Add("bitmap", new Bitmap(Path.Combine("bitmaps", "almogaver24bits.bmp")));
                values.Add("icon", new Icon(Path.Combine("bitmaps", "32x32_one_entry_4bit.ico")));
            }

            byte[] binaryWriterBuffer;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryResourceWriter writer = new BinaryResourceWriter(ms))
            {
                foreach (var pair in values)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(pair.Value.GetType());
                    byte[] buffer = (byte[])converter.ConvertTo(pair.Value, typeof(byte[]));
                    writer.AddTypeConverterResource(pair.Key, pair.Value.GetType().AssemblyQualifiedName, buffer);
                }
                writer.Generate();
                binaryWriterBuffer = ms.ToArray();
            }

            using (MemoryStream ms = new MemoryStream(binaryWriterBuffer, false))
            using (BinaryResourceReader reader = new BinaryResourceReader(ms))
            {
                IDictionaryEnumerator dictEnum = reader.GetEnumerator();

                while (dictEnum.MoveNext())
                {
                    ResourceValueEquals(values[(string)dictEnum.Key], dictEnum.Value);
                }
            }
        }


        [Fact]
        public static void TypeConverterStringResources()
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
                {
                    { "color", Color.AliceBlue },
                    { "point", new Point(2, 6) },
                    { "rect", new Rectangle(3, 6, 10, 20) },
                    { "size", new Size(4, 8) },
                    { "sizeF", new SizeF(4.2f, 8.5f) },
                    { "cultureInfo", new CultureInfo("en-US") },
                    { "enum", DayOfWeek.Friday }
                };

            // ImageConverter only on windows at the moment https://github.com/dotnet/corefx/issues/36949
            if (PlatformDetection.IsDrawingSupported && PlatformDetection.IsWindows)
            {
                values.Add("imageFormat", ImageFormat.Png);
                values.Add("font", SystemFonts.DefaultFont);
            }

            byte[] binaryWriterBuffer;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryResourceWriter writer = new BinaryResourceWriter(ms))
            {
                foreach (var pair in values)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(pair.Value.GetType());
                    string value = (string)converter.ConvertTo(pair.Value, typeof(string));
                    writer.AddTypeConverterResource(pair.Key, pair.Value.GetType().AssemblyQualifiedName, value);
                }
                writer.Generate();
                binaryWriterBuffer = ms.ToArray();
            }

            using (MemoryStream ms = new MemoryStream(binaryWriterBuffer, false))
            using (BinaryResourceReader reader = new BinaryResourceReader(ms))
            {
                IDictionaryEnumerator dictEnum = reader.GetEnumerator();

                while (dictEnum.MoveNext())
                {
                    ResourceValueEquals(values[(string)dictEnum.Key], dictEnum.Value);
                }
            }
        }

        [Fact]
        public static void StreamResources()
        {
            Dictionary<string, (Type type, Stream stream)> values = new Dictionary<string, (Type, Stream)>()
                {
                    { "myResourceType", (typeof(MyResourceType),  new MemoryStream(new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 })) }
                };

            if (PlatformDetection.IsDrawingSupported)
            {
                values.Add("icon", (typeof(Icon), File.OpenRead(Path.Combine("bitmaps", "32x32_one_entry_4bit.ico"))));
                values.Add("bitmap", (typeof(Bitmap), File.OpenRead(Path.Combine("bitmaps", "almogaver24bits.bmp"))));
            }

            byte[] binaryWriterBuffer;

            using (MemoryStream ms = new MemoryStream())
            using (BinaryResourceWriter writer = new BinaryResourceWriter(ms))
            {
                foreach (var pair in values)
                {
                    writer.AddStreamResource(pair.Key, pair.Value.type.AssemblyQualifiedName, pair.Value.stream, false);
                }
                writer.Generate();
                binaryWriterBuffer = ms.ToArray();
            }

            using (MemoryStream ms = new MemoryStream(binaryWriterBuffer, false))
            using (BinaryResourceReader reader = new BinaryResourceReader(ms))
            {
                IDictionaryEnumerator dictEnum = reader.GetEnumerator();

                while (dictEnum.MoveNext())
                {
                    var expectedTuple = values[(string)dictEnum.Key];
                    expectedTuple.stream.Seek(0, SeekOrigin.Begin);
                    object expected = Activator.CreateInstance(expectedTuple.type, new object[] { expectedTuple.stream });
                    ResourceValueEquals(expected, dictEnum.Value);
                }
            }
        }
        private static void ResourceValueEquals(object expected, object actual)
        {
            if (actual is Bitmap bitmap)
            {
                BitmapEquals((Bitmap)expected, bitmap);
            }
            else if (actual is Icon icon)
            {
                BitmapEquals(((Icon)expected).ToBitmap(), icon.ToBitmap());
            }
            else if (actual is Font font)
            {
                Font expectedFont = (Font)expected;
                Assert.Equal(expectedFont.FontFamily, font.FontFamily);
                Assert.Equal(expectedFont.Size, font.Size);
                Assert.Equal(expectedFont.Style, font.Style);
                Assert.Equal(expectedFont.Unit, font.Unit);
            }
            else
            {
                Assert.Equal(expected, actual);
            }
        }

        private static void BitmapEquals(Bitmap left, Bitmap right)
        {
            Assert.Equal(left.Size, right.Size);

            for (int x = 0; x < left.Width; ++x)
            {
                for (int y = 0; y < left.Height; ++y)
                {
                    Assert.Equal(left.GetPixel(x, y), right.GetPixel(x, y));
                }
            }
        }
    }

}





