using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System.Drawing.Primitives.Tests
{
    public partial class ColorTests
    {
        public static IEnumerable<object[]> NamedArgbValues =>
            new[]
            {
                new object[] {"Transparent", 0, 255, 255, 255},
                new object[] {"AliceBlue", 255, 240, 248, 255},
                new object[] {"AntiqueWhite", 255, 250, 235, 215},
                new object[] {"Aqua", 255, 0, 255, 255},
                new object[] {"Aquamarine", 255, 127, 255, 212},
                new object[] {"Azure", 255, 240, 255, 255},
                new object[] {"Beige", 255, 245, 245, 220},
                new object[] {"Bisque", 255, 255, 228, 196},
                new object[] {"Black", 255, 0, 0, 0},
                new object[] {"BlanchedAlmond", 255, 255, 235, 205},
                new object[] {"Blue", 255, 0, 0, 255},
                new object[] {"BlueViolet", 255, 138, 43, 226},
                new object[] {"Brown", 255, 165, 42, 42},
                new object[] {"BurlyWood", 255, 222, 184, 135},
                new object[] {"CadetBlue", 255, 95, 158, 160},
                new object[] {"Chartreuse", 255, 127, 255, 0},
                new object[] {"Chocolate", 255, 210, 105, 30},
                new object[] {"Coral", 255, 255, 127, 80},
                new object[] {"CornflowerBlue", 255, 100, 149, 237},
                new object[] {"Cornsilk", 255, 255, 248, 220},
                new object[] {"Crimson", 255, 220, 20, 60},
                new object[] {"Cyan", 255, 0, 255, 255},
                new object[] {"DarkBlue", 255, 0, 0, 139},
                new object[] {"DarkCyan", 255, 0, 139, 139},
                new object[] {"DarkGoldenrod", 255, 184, 134, 11},
                new object[] {"DarkGray", 255, 169, 169, 169},
                new object[] {"DarkGreen", 255, 0, 100, 0},
                new object[] {"DarkKhaki", 255, 189, 183, 107},
                new object[] {"DarkMagenta", 255, 139, 0, 139},
                new object[] {"DarkOliveGreen", 255, 85, 107, 47},
                new object[] {"DarkOrange", 255, 255, 140, 0},
                new object[] {"DarkOrchid", 255, 153, 50, 204},
                new object[] {"DarkRed", 255, 139, 0, 0},
                new object[] {"DarkSalmon", 255, 233, 150, 122},
                new object[] {"DarkSeaGreen", 255, 143, 188, 139},
                new object[] {"DarkSlateBlue", 255, 72, 61, 139},
                new object[] {"DarkSlateGray", 255, 47, 79, 79},
                new object[] {"DarkTurquoise", 255, 0, 206, 209},
                new object[] {"DarkViolet", 255, 148, 0, 211},
                new object[] {"DeepPink", 255, 255, 20, 147},
                new object[] {"DeepSkyBlue", 255, 0, 191, 255},
                new object[] {"DimGray", 255, 105, 105, 105},
                new object[] {"DodgerBlue", 255, 30, 144, 255},
                new object[] {"Firebrick", 255, 178, 34, 34},
                new object[] {"FloralWhite", 255, 255, 250, 240},
                new object[] {"ForestGreen", 255, 34, 139, 34},
                new object[] {"Fuchsia", 255, 255, 0, 255},
                new object[] {"Gainsboro", 255, 220, 220, 220},
                new object[] {"GhostWhite", 255, 248, 248, 255},
                new object[] {"Gold", 255, 255, 215, 0},
                new object[] {"Goldenrod", 255, 218, 165, 32},
                new object[] {"Gray", 255, 128, 128, 128},
                new object[] {"Green", 255, 0, 128, 0},
                new object[] {"GreenYellow", 255, 173, 255, 47},
                new object[] {"Honeydew", 255, 240, 255, 240},
                new object[] {"HotPink", 255, 255, 105, 180},
                new object[] {"IndianRed", 255, 205, 92, 92},
                new object[] {"Indigo", 255, 75, 0, 130},
                new object[] {"Ivory", 255, 255, 255, 240},
                new object[] {"Khaki", 255, 240, 230, 140},
                new object[] {"Lavender", 255, 230, 230, 250},
                new object[] {"LavenderBlush", 255, 255, 240, 245},
                new object[] {"LawnGreen", 255, 124, 252, 0},
                new object[] {"LemonChiffon", 255, 255, 250, 205},
                new object[] {"LightBlue", 255, 173, 216, 230},
                new object[] {"LightCoral", 255, 240, 128, 128},
                new object[] {"LightCyan", 255, 224, 255, 255},
                new object[] {"LightGoldenrodYellow", 255, 250, 250, 210},
                new object[] {"LightGreen", 255, 144, 238, 144},
                new object[] {"LightGray", 255, 211, 211, 211},
                new object[] {"LightPink", 255, 255, 182, 193},
                new object[] {"LightSalmon", 255, 255, 160, 122},
                new object[] {"LightSeaGreen", 255, 32, 178, 170},
                new object[] {"LightSkyBlue", 255, 135, 206, 250},
                new object[] {"LightSlateGray", 255, 119, 136, 153},
                new object[] {"LightSteelBlue", 255, 176, 196, 222},
                new object[] {"LightYellow", 255, 255, 255, 224},
                new object[] {"Lime", 255, 0, 255, 0},
                new object[] {"LimeGreen", 255, 50, 205, 50},
                new object[] {"Linen", 255, 250, 240, 230},
                new object[] {"Magenta", 255, 255, 0, 255},
                new object[] {"Maroon", 255, 128, 0, 0},
                new object[] {"MediumAquamarine", 255, 102, 205, 170},
                new object[] {"MediumBlue", 255, 0, 0, 205},
                new object[] {"MediumOrchid", 255, 186, 85, 211},
                new object[] {"MediumPurple", 255, 147, 112, 219},
                new object[] {"MediumSeaGreen", 255, 60, 179, 113},
                new object[] {"MediumSlateBlue", 255, 123, 104, 238},
                new object[] {"MediumSpringGreen", 255, 0, 250, 154},
                new object[] {"MediumTurquoise", 255, 72, 209, 204},
                new object[] {"MediumVioletRed", 255, 199, 21, 133},
                new object[] {"MidnightBlue", 255, 25, 25, 112},
                new object[] {"MintCream", 255, 245, 255, 250},
                new object[] {"MistyRose", 255, 255, 228, 225},
                new object[] {"Moccasin", 255, 255, 228, 181},
                new object[] {"NavajoWhite", 255, 255, 222, 173},
                new object[] {"Navy", 255, 0, 0, 128},
                new object[] {"OldLace", 255, 253, 245, 230},
                new object[] {"Olive", 255, 128, 128, 0},
                new object[] {"OliveDrab", 255, 107, 142, 35},
                new object[] {"Orange", 255, 255, 165, 0},
                new object[] {"OrangeRed", 255, 255, 69, 0},
                new object[] {"Orchid", 255, 218, 112, 214},
                new object[] {"PaleGoldenrod", 255, 238, 232, 170},
                new object[] {"PaleGreen", 255, 152, 251, 152},
                new object[] {"PaleTurquoise", 255, 175, 238, 238},
                new object[] {"PaleVioletRed", 255, 219, 112, 147},
                new object[] {"PapayaWhip", 255, 255, 239, 213},
                new object[] {"PeachPuff", 255, 255, 218, 185},
                new object[] {"Peru", 255, 205, 133, 63},
                new object[] {"Pink", 255, 255, 192, 203},
                new object[] {"Plum", 255, 221, 160, 221},
                new object[] {"PowderBlue", 255, 176, 224, 230},
                new object[] {"Purple", 255, 128, 0, 128},
                new object[] {"Red", 255, 255, 0, 0},
                new object[] {"RosyBrown", 255, 188, 143, 143},
                new object[] {"RoyalBlue", 255, 65, 105, 225},
                new object[] {"SaddleBrown", 255, 139, 69, 19},
                new object[] {"Salmon", 255, 250, 128, 114},
                new object[] {"SandyBrown", 255, 244, 164, 96},
                new object[] {"SeaGreen", 255, 46, 139, 87},
                new object[] {"SeaShell", 255, 255, 245, 238},
                new object[] {"Sienna", 255, 160, 82, 45},
                new object[] {"Silver", 255, 192, 192, 192},
                new object[] {"SkyBlue", 255, 135, 206, 235},
                new object[] {"SlateBlue", 255, 106, 90, 205},
                new object[] {"SlateGray", 255, 112, 128, 144},
                new object[] {"Snow", 255, 255, 250, 250},
                new object[] {"SpringGreen", 255, 0, 255, 127},
                new object[] {"SteelBlue", 255, 70, 130, 180},
                new object[] {"Tan", 255, 210, 180, 140},
                new object[] {"Teal", 255, 0, 128, 128},
                new object[] {"Thistle", 255, 216, 191, 216},
                new object[] {"Tomato", 255, 255, 99, 71},
                new object[] {"Turquoise", 255, 64, 224, 208},
                new object[] {"Violet", 255, 238, 130, 238},
                new object[] {"Wheat", 255, 245, 222, 179},
                new object[] {"White", 255, 255, 255, 255},
                new object[] {"WhiteSmoke", 255, 245, 245, 245},
                new object[] {"Yellow", 255, 255, 255, 0},
                new object[] {"YellowGreen", 255, 154, 205, 50},
            };

        public static IEnumerable<object[]> ColorNames => Enum.GetNames(typeof(KnownColor)).Select(n => new object[] { n });

        private Color? GetColorByProperty(string name)
        {
            return (Color?)typeof(Color).GetProperty(name)?.GetValue(null);
        }

        [Theory]
        [MemberData(nameof(NamedArgbValues))]
        public void ArgbValues(string name, int alpha, int red, int green, int blue)
        {
            Color? color = GetColorByProperty(name);
            if (color.HasValue)
            {
                Assert.Equal(alpha, color.Value.A);
                Assert.Equal(red, color.Value.R);
                Assert.Equal(green, color.Value.G);
                Assert.Equal(blue, color.Value.B);
            }
        }

        [Fact]
        public void Empty()
        {
            Assert.True(Color.Empty.IsEmpty);
            Assert.False(Color.FromArgb(0, Color.Black).IsEmpty);
        }

        [Fact]
        public void IsKnownColor()
        {
            Assert.True(Color.FromKnownColor(KnownColor.AliceBlue).IsKnownColor);
            Assert.True(Color.FromName("AliceBlue").IsKnownColor);
            Assert.False(Color.FromArgb(Color.AliceBlue.A, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B).IsKnownColor);
        }

        [Fact]
        public void IsNamedColor()
        {
            Assert.True(Color.AliceBlue.IsNamedColor);
            Assert.True(Color.FromKnownColor(KnownColor.AliceBlue).IsNamedColor);
            Assert.True(Color.FromName("AliceBlue").IsNamedColor);
            Assert.False(Color.FromArgb(Color.AliceBlue.A, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B).IsNamedColor);
        }

        [Fact]
        public void IsSystemColor()
        {
            Color c = Color.FromKnownColor(KnownColor.ActiveBorder);
            Assert.True(c.IsSystemColor);
            Assert.True(Color.FromName("ActiveBorder").IsSystemColor);
            Assert.False(Color.FromArgb(c.A, c.R, c.G, c.B).IsSystemColor);
            Assert.False(Color.FromKnownColor(KnownColor.AliceBlue).IsSystemColor);
            Assert.False(Color.FromName("AliceBlue").IsSystemColor);
        }

        [Theory]
        [MemberData(nameof(ColorNames))]
        public void KnownNames(string name)
        {
            Assert.Equal(name, Color.FromName(name).Name);
            var colorByProperty = GetColorByProperty(name);
            if (colorByProperty.HasValue)
            {
                Assert.Equal(name, colorByProperty.Value.Name);
            }
        }

        [Fact]
        public void Name()
        {
            Assert.Equal("1122ccff", Color.FromArgb(0x11, 0x22, 0xcc, 0xff).Name);
        }

        public static IEnumerable<object[]> ColorNamePairs => ColorNames.Zip(ColorNames.Skip(1), (l, r) => new[] { l[0], r[0] });

        [Theory]
        [MemberData(nameof(ColorNamePairs))]
        public void GetHashCode(string name1, string name2)
        {
            Assert.NotEqual(name1, name2);
            Color c1 = GetColorByProperty(name1) ?? Color.FromName(name1);
            Color c2 = GetColorByProperty(name2) ?? Color.FromName(name2);
            Assert.NotEqual(c2.GetHashCode(), c1.GetHashCode());
            Assert.Equal(c1.GetHashCode(), Color.FromName(name1).GetHashCode());
        }

        [Theory]
        [InlineData(0x11cc8833, 0x11, 0xcc, 0x88, 0x33)]
        [InlineData(unchecked((int)0xf1cc8833), 0xf1, 0xcc, 0x88, 0x33)]
        public void ToArgb(int argb, int alpha, int red, int green, int blue)
        {
            Assert.Equal(argb, Color.FromArgb(alpha, red, green, blue).ToArgb());
        }

        [Theory]
        [MemberData(nameof(ColorNames))]
        public void ToKnownColor(string name)
        {
            var knownColor = (KnownColor)Enum.Parse(typeof(KnownColor), name);
            var colorByProperty = GetColorByProperty(name);
            if (colorByProperty.HasValue)
            {
                Assert.Equal(knownColor, colorByProperty.Value.ToKnownColor());
            }
            Assert.Equal(knownColor, Color.FromName(name).ToKnownColor());
        }

        [Fact]
        public void ToStringEmpty()
        {
            Assert.Equal("Color [Empty]", Color.Empty.ToString());
        }

        [Theory]
        [MemberData(nameof(ColorNames))]
        public void ToStringNamed(string name)
        {
            string expected = $"Color [{name}]";
            Assert.Equal(expected, Color.FromName(name).ToString());
        }

        [Theory]
        [InlineData("Color [A=1, R=2, G=3, B=4]", 1, 2, 3, 4)]
        public void ToStringArgb(string expected, int alpha, int red, int green, int blue)
        {
            Assert.Equal(expected, Color.FromArgb(alpha, red, green, blue).ToString());
        }

        public static IEnumerable<object[]> InvalidValues =>
            new[]
            {
                new object[] {-1},
                new object[] {256},
            };

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void FromArgb_InvalidAlpha(int alpha)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(alpha, Color.Red);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(alpha, 0, 0, 0);
            });
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void FromArgb_InvalidRed(int red)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(red, 0, 0);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(0, red, 0, 0);
            });
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void FromArgb_InvalidGreen(int green)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(0, green, 0);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(0, 0, green, 0);
            });
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public void FromArgb_InvalidBlue(int blue)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(0, 0, blue);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                Color.FromArgb(0, 0, 0, blue);
            });
        }
        
        [Fact]
        public void FromName_Invalid()
        {
            Color c = Color.FromName("OingoBoingo");
            Assert.True(c.IsNamedColor);
            Assert.Equal(0, c.ToArgb());
            Assert.Equal("OingoBoingo", c.Name);
        }
        
        private void CheckRed(Color color)
        {
            Assert.Equal(255, color.A);
            Assert.Equal(255, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(0, color.B);
            Assert.Equal("Red", color.Name);
            Assert.False(color.IsEmpty, "IsEmpty");
            Assert.True(color.IsKnownColor, "IsKnownColor");
            Assert.True(color.IsNamedColor, "IsNamedColor");
            Assert.False(color.IsSystemColor, "IsSystemColor");
        }

        [Fact]
        public void SerializationRoundTrip()
        {
            Color c = Color.Red;
            CheckRed(c);

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, c);

            ms.Position = 0;
            Color color = (Color)bf.Deserialize(ms);
            CheckRed(color);
        }
    }
}
