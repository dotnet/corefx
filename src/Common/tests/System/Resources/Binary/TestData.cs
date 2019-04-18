using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Resources.Binary.Tests
{
    public static class TestData
    {
        public static IReadOnlyDictionary<string, object> Primitive { get; } =
            new Dictionary<string, object>()
            {
                ["string"] = "value",
                ["bool"] = true,
                ["char"] = 'b',
                ["int"] = 42,
                ["byte"] = (byte)7,
                ["sbyte"] = (sbyte)-3,
                ["short"] = (short) 31000,
                ["ushort"] = (ushort) 61000,
                ["long"] = 10000000000,
                ["ulong"] = ulong.MaxValue,
                ["float"] = 3.14f,
                ["double"] = 3.14159,
                ["decimal"] = 3.141596536897931m
            };

        public static IReadOnlyDictionary<string, object> BinaryFormatted
        {
            get => PlatformDetection.IsDrawingSupported ?
                  BinaryFormattedDrawing : BinaryFormattedWithoutDrawing;
        }
        public static Dictionary<string, object> BinaryFormattedWithoutDrawing { get; } =
            new Dictionary<string, object>()
            {
                ["enum_bin"] = DayOfWeek.Friday,
                ["point_bin"] = new Point(4, 8)
            };

        public static IReadOnlyDictionary<string, object> BinaryFormattedDrawing
        {
            get => new Dictionary<string, object>(BinaryFormattedWithoutDrawing)
            {
                ["bitmap_bin"] = new Bitmap(Path.Combine("bitmaps", "almogaver24bits.bmp")),
                ["font_bin"] = SystemFonts.DefaultFont
            };
        }

        public static IReadOnlyDictionary<string, object> ByteArrayConverter
        {
            get => PlatformDetection.IsDrawingSupported ?
                ByteArrayConverterDrawing : ByteArrayConverterWithoutDrawing;
        }

        public static Dictionary<string, object> ByteArrayConverterWithoutDrawing { get; } =
            new Dictionary<string, object>()
            {
                ["myResourceType_bytes"] = new MyResourceType(new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 })
            };

        public static IReadOnlyDictionary<string, object> ByteArrayConverterDrawing
        {
            get => new Dictionary<string, object>(ByteArrayConverterWithoutDrawing)
            {
                ["bitmap_bytes"] = new Bitmap(Path.Combine("bitmaps", "almogaver24bits.bmp")),
                ["icon_bytes"] = new Icon(Path.Combine("bitmaps", "32x32_one_entry_4bit.ico"))
            };
        }

        public static IReadOnlyDictionary<string, object> StringConverter
        {
            get => PlatformDetection.IsDrawingSupported ?
                StringConverterDrawing : StringConverterWithoutDrawing;
        }

        public static Dictionary<string, object> StringConverterWithoutDrawing { get; } =
            new Dictionary<string, object>()
            {
                ["color_string"] = Color.AliceBlue,
                ["point_string"] = new Point(2, 6),
                ["rect_string"] = new Rectangle(3, 6, 10, 20),
                ["size_string"] = new Size(4, 8),
                ["sizeF_string"] = new SizeF(4.2f, 8.5f),
                ["cultureInfo_string"] = new CultureInfo("en-US"),
                ["enum_string"] = DayOfWeek.Friday
            };

        public static IReadOnlyDictionary<string, object> StringConverterDrawing
        {
            get => new Dictionary<string, object>(StringConverterWithoutDrawing)
            {
                ["imageFormat_string"] = ImageFormat.Png,
                ["font_string"] = SystemFonts.DefaultFont
            };
        }

        public static IReadOnlyDictionary<string, (Type type, Stream stream)> Stream
        {
            get => PlatformDetection.IsDrawingSupported ?
                StreamDrawing : StreamWithoutDrawing;
        }

        public static Dictionary<string, (Type type, Stream stream)> StreamWithoutDrawing { get; } =
            new Dictionary<string, (Type type, Stream stream)>()
            {
                ["myResourceType_stream"] = (typeof(MyResourceType), new MemoryStream(new byte[] { 0xAB, 0xCD, 0xEF, 0x01, 0x23, 0x45, 0x67, 0x89 }))
            };

        public static IReadOnlyDictionary<string, (Type type, Stream stream)> StreamDrawing
        {
            get => new Dictionary<string, (Type type, Stream stream)>(StreamWithoutDrawing)
            {
                ["icon_stream"] = (typeof(Icon), File.OpenRead(Path.Combine("bitmaps", "32x32_one_entry_4bit.ico"))),
                ["bitmap_stream"] = (typeof(Bitmap), File.OpenRead(Path.Combine("bitmaps", "almogaver24bits.bmp")))
            };
        }
        
        public static void WriteResources(string file)
        {
            using (var writer = new BinaryResourceWriter(file))
            {
                foreach(var pair in Primitive)
                {
                    writer.AddResource(pair.Key, pair.Value);
                }

                var formatter = new BinaryFormatter();
                foreach(var pair in BinaryFormattedWithoutDrawing)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        formatter.Serialize(memoryStream, pair.Value);
                        writer.AddBinaryFormattedResource(pair.Key, pair.Value.GetType().AssemblyQualifiedName, memoryStream.ToArray());
                    }
                }

                foreach (var pair in ByteArrayConverterWithoutDrawing)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(pair.Value.GetType());
                    byte[] buffer = (byte[])converter.ConvertTo(pair.Value, typeof(byte[]));
                    writer.AddTypeConverterResource(pair.Key, pair.Value.GetType().AssemblyQualifiedName, buffer);
                }

                foreach (var pair in StringConverterWithoutDrawing)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(pair.Value.GetType());
                    string value = (string)converter.ConvertTo(pair.Value, typeof(string));
                    writer.AddTypeConverterResource(pair.Key, pair.Value.GetType().AssemblyQualifiedName, value);
                }

                foreach(var pair in StreamWithoutDrawing)
                {
                    writer.AddStreamResource(pair.Key, pair.Value.type.AssemblyQualifiedName, pair.Value.stream, false);
                }

                writer.Generate();
            }
        }
    }
}
