// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Numerics
{
    /// <summary>
    /// A structure representing a Color using four single precision floating point values in RGBA format.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Color : IEquatable<Color>, IFormattable
    {
        #region Fields and properties
        /// <summary>
        /// The Red component of the Color, represented on the range 0 to 1.
        /// </summary>
        public float R;
        /// <summary>
        /// The Green component of the Color, represented on the range 0 to 1.
        /// </summary>
        public float G;
        /// <summary>
        /// The Blue component of the Color, represented on the range 0 to 1.
        /// </summary>
        public float B;
        /// <summary>
        /// The Alpha component of the Color, represented on the range 0 to 1. Alpha represents the color's opacity.
        /// </summary>
        public float A;

        /// <summary>
        /// The Red component of the Color represented as an 8-bit byte.
        /// </summary>
        public byte R8
        {
            get => (byte)MathF.Round(R * 255.0f);
            set => R = value / 255.0f;
        }
        /// <summary>
        /// The Green component of the Color represented as an 8-bit byte.
        /// </summary>
        public byte G8
        {
            get => (byte)MathF.Round(G * 255.0f);
            set => G = value / 255.0f;
        }
        /// <summary>
        /// The Blue component of the Color represented as an 8-bit byte.
        /// </summary>
        public byte B8
        {
            get => (byte)MathF.Round(B * 255.0f);
            set => B = value / 255.0f;
        }
        /// <summary>
        /// The Alpha component of the Color represented as an 8-bit byte.
        /// </summary>
        public byte A8
        {
            get => (byte)MathF.Round(A * 255.0f);
            set => A = value / 255.0f;
        }

        /// <summary>
        /// The Hue of the Color, represented on the range 0 to 1.
        /// </summary>
        public float H
        {
            get
            {
                float max = MathF.Max(R, MathF.Max(G, B));
                float min = MathF.Min(R, MathF.Min(G, B));

                float delta = max - min;

                if (delta == 0)
                    return 0;

                float h;

                if (R == max)
                    h = (G - B) / delta; // Between yellow & magenta
                else if (G == max)
                    h = 2 + (B - R) / delta; // Between cyan & yellow
                else
                    h = 4 + (R - G) / delta; // Between magenta & cyan

                h /= 6.0f;

                if (h < 0)
                    h += 1.0f;

                return h;
            }
            set => this = FromHSV(value, S, V, A);
        }

        /// <summary>
        /// The Saturation of the Color, represented on the range 0 to 1.
        /// </summary>
        public float S
        {
            get
            {
                float max = MathF.Max(R, MathF.Max(G, B));
                float min = MathF.Min(R, MathF.Min(G, B));

                float delta = max - min;

                return max != 0 ? delta / max : 0;
            }
            set
            {
                this = FromHSV(H, value, V, A);
            }
        }

        /// <summary>
        /// The brightness value of the Color, represented on the range 0 to 1.
        /// </summary>
        public float V
        {
            get => MathF.Max(R, MathF.Max(G, B));
            set => FromHSV(H, S, value, A);
        }

        /// <summary>
        /// Returns the Color component specified by RGBA index.
        /// </summary>
        /// <value>The RGBA index. Valid values are 0 to 3.</value>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return R;
                    case 1:
                        return G;
                    case 2:
                        return B;
                    case 3:
                        return A;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        R = value;
                        return;
                    case 1:
                        G = value;
                        return;
                    case 2:
                        B = value;
                        return;
                    case 3:
                        A = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        #endregion Fields and properties

        #region Public methods
        /// <summary>
        /// Returns a new Color resulting from blending this Color over another.
        /// </summary>
        /// <param name="over">The base Color to blend over.</param>
        /// <returns>The new blended Color.</returns>
        public Color Blend(Color over)
        {
            float sa = 1.0f - over.A;
            float alpha = A * sa + over.A;

            if (alpha == 0)
            {
                return new Color(0, 0, 0, 0);
            }

            Color res;
            res.R = (R * A * sa + over.R * over.A) / alpha;
            res.G = (G * A * sa + over.G * over.A) / alpha;
            res.B = (B * A * sa + over.B * over.A) / alpha;
            res.A = alpha;

            return res;
        }

        /// <summary>
        /// Returns a new Color resulting from making this Color darker by the specified ratio.
        /// </summary>
        /// <param name="amount">Value between 0 and 1 to make the Color darker by.</param>
        /// <returns>A Color darkened by the given amount.</returns>
        public Color Darkened(float amount)
        {
            Color res = this;
            res.R = res.R * (1.0f - amount);
            res.G = res.G * (1.0f - amount);
            res.B = res.B * (1.0f - amount);
            return res;
        }

        /// <summary>
        /// Returns the inverted version of this Color with components (1 - R, 1 - G, 1 - B, A).
        /// </summary>
        /// <returns>The inverted Color.</returns>
        public Color Inverted()
        {
            return new Color(
                1.0f - R,
                1.0f - G,
                1.0f - B,
                A
            );
        }

        /// <summary>
        /// Interpolates between this Color and a given Color c by a ratio t.
        /// </summary>
        /// <param name="color">The second Color to interpolate with.</param>
        /// <param name="t">The interpolation ratio between this and c.</param>
        /// <returns>A new interpolated Color.</returns>
        public Color Lerp(Color color, float t)
        {
            var res = this;

            res.R += t * (color.R - R);
            res.G += t * (color.G - G);
            res.B += t * (color.B - B);
            res.A += t * (color.A - A);

            return res;
        }

        /// <summary>
        /// Returns a new Color resulting from making this Color lighter by the specified ratio.
        /// </summary>
        /// <param name="amount">Value between 0 and 1 to make the Color lighter by.</param>
        /// <returns>A Color lightened by the given amount.</returns>
        public Color Lightened(float amount)
        {
            Color res = this;
            res.R = res.R + (1.0f - res.R) * amount;
            res.G = res.G + (1.0f - res.G) * amount;
            res.B = res.B + (1.0f - res.B) * amount;
            return res;
        }
        #endregion Public methods

        #region Export methods
        /// <summary>
        /// Packs the Color in ARGB format into a 32-bit integer by storing the channels as four 8-bit values.
        /// ARGB is the format traditionally used by many Microsoft APIs.
        /// </summary>
        /// <returns>A 32-bit int representing this Color.</returns>
        public int ToArgb32()
        {
            int c = (byte)MathF.Round(A * 255.0f);
            c <<= 8;
            c |= (byte)MathF.Round(R * 255.0f);
            c <<= 8;
            c |= (byte)MathF.Round(G * 255.0f);
            c <<= 8;
            c |= (byte)MathF.Round(B * 255.0f);

            return c;
        }

        /// <summary>
        /// Packs the Color in ARGB format into a 64-bit integer by storing the channels as four 16-bit values.
        /// ARGB is the format traditionally used by many Microsoft APIs.
        /// </summary>
        /// <returns>A 64-bit long representing this Color.</returns>
        public long ToArgb64()
        {
            long c = (ushort)MathF.Round(A * 65535.0f);
            c <<= 16;
            c |= (ushort)MathF.Round(R * 65535.0f);
            c <<= 16;
            c |= (ushort)MathF.Round(G * 65535.0f);
            c <<= 16;
            c |= (ushort)MathF.Round(B * 65535.0f);

            return c;
        }

        /// <summary>
        /// Packs the Color in RGBA format into a 32-bit integer by storing the channels as four 8-bit values.
        /// RGBA is the recommended format and is the most commonly used format.
        /// </summary>
        /// <returns>A 32-bit int representing this Color.</returns>
        public int ToRgba32()
        {
            int c = (byte)MathF.Round(R * 255.0f);
            c <<= 8;
            c |= (byte)MathF.Round(G * 255.0f);
            c <<= 8;
            c |= (byte)MathF.Round(B * 255.0f);
            c <<= 8;
            c |= (byte)MathF.Round(A * 255.0f);

            return c;
        }

        /// <summary>
        /// Packs the Color in RGBA format into a 64-bit integer by storing the channels as four 16-bit values.
        /// RGBA is the recommended format and is the most commonly used format.
        /// </summary>
        /// <returns>A 64-bit long representing this Color.</returns>
        public long ToRgba64()
        {
            long c = (ushort)MathF.Round(R * 65535.0f);
            c <<= 16;
            c |= (ushort)MathF.Round(G * 65535.0f);
            c <<= 16;
            c |= (ushort)MathF.Round(B * 65535.0f);
            c <<= 16;
            c |= (ushort)MathF.Round(A * 65535.0f);

            return c;
        }

        /// <summary>
        /// Returns the HTML hexadecimal string representation of this Color in RGBA format.
        /// </summary>
        /// <param name="alpha">Whether or not to include alpha at the end.</param>
        /// <returns>An HTML hexadecimal string representation of this Color.</returns>
        public string ToHtml(bool alpha = true)
        {
            var txt = string.Empty;

            txt += ColorUtils.ToHex8(R);
            txt += ColorUtils.ToHex8(G);
            txt += ColorUtils.ToHex8(B);

            if (alpha)
                txt += ColorUtils.ToHex8(A);

            return txt;
        }

        /// <summary>
        /// Converts the given Color into HSV float values. Same result as with using the HSV properties, but faster.
        /// </summary>
        /// <param name="hue">The hue output.</param>
        /// <param name="saturation">The saturation output.</param>
        /// <param name="value">The brightness value output.</param>
        public void ToHsv(out float hue, out float saturation, out float value)
        {
            float max = MathF.Max(R, MathF.Max(G, B));
            float min = MathF.Min(R, MathF.Min(G, B));

            float delta = max - min;

            if (delta == 0)
            {
                hue = 0;
            }
            else
            {
                if (R == max)
                    hue = (G - B) / delta; // Between yellow & magenta
                else if (G == max)
                    hue = 2 + (B - R) / delta; // Between cyan & yellow
                else
                    hue = 4 + (R - G) / delta; // Between magenta & cyan

                hue /= 6.0f;

                if (hue < 0)
                    hue += 1.0f;
            }

            saturation = max == 0 ? 0 : 1.0f - 1.0f * min / max;
            value = max / 255f;
        }
        #endregion Export methods

        #region Constructors
        /// <summary>
        /// Constructs a Color with the given float values.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        /// <param name="a">Alpha component, optional.</param>
        public Color(float r, float g, float b, float a = 1.0f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs a new Color from four 8-bit bytes.
        /// </summary>
        /// <param name="r8">The Red component of the Color represented as an 8-bit byte.</param>
        /// <param name="g8">The Green component of the Color represented as an 8-bit byte.</param>
        /// <param name="b8">The Blue component of the Color represented as an 8-bit byte.</param>
        /// <param name="a8">The Alpha component of the Color represented as an 8-bit byte.</param>
        /// <returns></returns>
        public Color(byte r8, byte g8, byte b8, byte a8 = 255)
        {
            R = r8 / 255.0f;
            G = g8 / 255.0f;
            B = b8 / 255.0f;
            A = a8 / 255.0f;
        }

        /// <summary>
        /// Constructs a grayscale Color with the given float value(s).
        /// </summary>
        /// <param name="v">Value to assign to the RGB components.</param>
        /// <param name="a">Alpha component, optional.</param>
        public Color(float v, float a = 1.0f)
        {
            R = v;
            G = v;
            B = v;
            A = a;
        }

        /// <summary>
        /// Constructs a grayscale Color with the given byte value(s).
        /// </summary>
        /// <param name="v8">Value to assign to the RGB components.</param>
        /// <param name="a8">Alpha component, optional.</param>
        /// <returns></returns>
        public Color(byte v8, byte a8 = 255)
        {
            R = v8 / 255.0f;
            G = v8 / 255.0f;
            B = v8 / 255.0f;
            A = a8 / 255.0f;
        }

        /// <summary>
        /// Constructs a Color from another Color and an alpha value.
        /// </summary>
        /// <param name="color">Color to construct from.</param>
        /// <param name="alpha">Alpha component, optional.</param>
        public Color(Color color, float alpha = 1.0f)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        /// <summary>
        /// Constructs a Color from a Color8 and an alpha value.
        /// </summary>
        /// <param name="color">Color to construct from.</param>
        /// <param name="alpha">Alpha component, optional.</param>
        public Color(Color8 color, float alpha = 1.0f)
        {
            R = color.Rf;
            G = color.Gf;
            B = color.Bf;
            A = color.Af;
        }

        /// <summary>
        /// Constructs a new Color using a 32-bit integer, treating its bits as four packed 8-bit values.
        /// </summary>
        /// <param name="rgba">The integer containing packed values in RGBA format.</param>
        public Color(int rgba)
        {
            A = (rgba & 0xFF) / 255.0f;
            rgba >>= 8;
            B = (rgba & 0xFF) / 255.0f;
            rgba >>= 8;
            G = (rgba & 0xFF) / 255.0f;
            rgba >>= 8;
            R = (rgba & 0xFF) / 255.0f;
        }

        /// <summary>
        /// Constructs a new Color using a 64-bit integer, treating its bits as four packed 16-bit values.
        /// </summary>
        /// <param name="rgba">The integer containing packed values in RGBA format.</param>
        public Color(long rgba)
        {
            A = (rgba & 0xFFFF) / 65535.0f;
            rgba >>= 16;
            B = (rgba & 0xFFFF) / 65535.0f;
            rgba >>= 16;
            G = (rgba & 0xFFFF) / 65535.0f;
            rgba >>= 16;
            R = (rgba & 0xFFFF) / 65535.0f;
        }

        /// <summary>
        /// Constructs a new Color from the given HSV float values.
        /// </summary>
        /// <param name="hue">The hue input.</param>
        /// <param name="saturation">The saturation input.</param>
        /// <param name="value">The brightness value input.</param>
        /// <param name="alpha">The alpha channel opacity input.</param>
        /// <returns>A new Color constructed from the values.</returns>
        public static Color FromHSV(float hue, float saturation, float value, float alpha = 1.0f)
        {
            if (saturation == 0)
            {
                // acp_hromatic (grey)
                return new Color(value, value, value, alpha);
            }

            int i;
            float f, p, q, t;

            hue *= 6.0f;
            hue %= 6f;
            i = (int)hue;

            f = hue - i;
            p = value * (1 - saturation);
            q = value * (1 - saturation * f);
            t = value * (1 - saturation * (1 - f));

            switch (i)
            {
                case 0: // Red is the dominant color
                    return new Color(value, t, p, alpha);
                case 1: // Green is the dominant color
                    return new Color(q, value, p, alpha);
                case 2:
                    return new Color(p, value, t, alpha);
                case 3: // Blue is the dominant color
                    return new Color(p, q, value, alpha);
                case 4:
                    return new Color(t, p, value, alpha);
                default: // (5) Red is the dominant color
                    return new Color(value, p, q, alpha);
            }
        }

/*
        /// <summary>
        /// Creates a Color structure from the specified name of a predefined Color.
        /// </summary>
        /// <param name="name">The name of the Color.</param>
        /// <param name="alpha">Alpha component, optional.</param>
        /// <returns></returns>
        public static Color FromName(string name, float? alpha = 1.0f)
        {
            name = name.Replace(" ", string.Empty);
            name = name.Replace("-", string.Empty);
            name = name.Replace("_", string.Empty);
            name = name.Replace("'", string.Empty);
            name = name.Replace(".", string.Empty);
            name = name.ToLower();

            if (!Colors.namedColors.ContainsKey(name))
            {
                throw new ArgumentOutOfRangeException($"Invalid Color Name: {name}");
            }

            Color color = Colors.namedColors[name];
            if (alpha != null)
                color.A = (float)alpha;
            return color;
        }
 */

        /// <summary>
        /// Constructs a new Color from the given string representing a hexadecimal color code in RGBA format.
        /// </summary>
        /// <param name="rgba">String representing a hexadecimal color code in RGBA format.</param>
        public Color(string rgba)
        {
            if (rgba.Length == 0)
            {
                R = 0f;
                G = 0f;
                B = 0f;
                A = 1.0f;
                return;
            }

            if (rgba[0] == '#')
                rgba = rgba.Substring(1);

            bool alpha;

            if (rgba.Length == 8)
            {
                alpha = true;
            }
            else if (rgba.Length == 6)
            {
                alpha = false;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Length is " + rgba.Length + " but a length of 6 or 8 is expected: " + rgba);
            }

            // ParseCol8 can return -1, so if there's an error we'll throw it.
            R = ColorUtils.ParseCol8(rgba, 0) / 255.0f;
            if (R < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Red part is not valid hexadecimal: " + rgba);
            }

            G = ColorUtils.ParseCol8(rgba, 2) / 255.0f;
            if (G < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Green part is not valid hexadecimal: " + rgba);
            }

            B = ColorUtils.ParseCol8(rgba, 4) / 255.0f;
            if (B < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Blue part is not valid hexadecimal: " + rgba);
            }

            if (alpha)
            {
                A = ColorUtils.ParseCol8(rgba, 6) / 255.0f;
                if (A < 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid color code. Red part is not valid hexadecimal: " + rgba);
                }
            }
            else
            {
                A = 1.0f;
            }
        }
        #endregion Constructors

        #region Public operators
        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Checks if this Color is exactly equal to the given object.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <returns>Whether or not they are exactly equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Color)
            {
                return Equals((Color)obj);
            }

            return false;
        }

        /// <summary>
        /// Checks if this Color is exactly equal to the given Color.
        /// </summary>
        /// <param name="other">The other Color.</param>
        /// <returns>Whether or not they are exactly equal.</returns>
        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        }

        /// <summary>
        /// Converts the Color to a Color in "R,G,B,A" format.
        /// </summary>
        /// <returns>String representing this Color in "R,G,B,A" format.</returns>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", R.ToString(), G.ToString(), B.ToString(), A.ToString());
        }

        /// <summary>
        /// Converts the Color to a string in "R,G,B,A" format, with custom formatting options passed to each value.
        /// </summary>
        /// <param name="format">Custom formatting to use for each value.</param>
        /// <returns>String representing this Color in "R,G,B,A" format.</returns>
        public string ToString(string? format)
        {
            return string.Format("{0},{1},{2},{3}", R.ToString(format), G.ToString(format), B.ToString(format), A.ToString(format));
        }

        /// <summary>
        /// Converts the Color to a string in "R,G,B,A" format, with custom formatting options passed to each value.
        /// </summary>
        /// <param name="format">Custom formatting to use for each value.</param>
        /// <param name="formatProvider">The format provider to use when formatting elements.</param>
        /// <returns>String representing this Color in "R,G,B,A" format.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format("{0},{1},{2},{3}", R.ToString(format, formatProvider), G.ToString(format, formatProvider), B.ToString(format, formatProvider), A.ToString(format, formatProvider));
        }
        #endregion Public operators
    }
}
