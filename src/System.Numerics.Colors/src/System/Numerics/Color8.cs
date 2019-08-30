// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Numerics
{
    /// <summary>
    /// A structure representing a color using four byte values in RGBA format.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Color8 : IEquatable<Color8>, IFormattable
    {
        #region Fields and properties
        /// <summary>
        /// The Red component of the Color8, represented on the range 0 to 255.
        /// </summary>
        public byte R;
        /// <summary>
        /// The Green component of the Color8, represented on the range 0 to 255.
        /// </summary>
        public byte G;
        /// <summary>
        /// The Blue component of the Color8, represented on the range 0 to 255.
        /// </summary>
        public byte B;
        /// <summary>
        /// The Alpha component of the Color8, represented on the range 0 to 255. Alpha represents the color's opacity.
        /// </summary>
        public byte A;

        /// <summary>
        /// The Red component of the Color8 represented as a single-precision float on the range 0 to 1.
        /// </summary>
        public float Rf
        {
            get => R / 255.0f;
            set => R = (byte)MathF.Round(R * 255.0f);
        }
        /// <summary>
        /// The Green component of the Color8 represented as a single-precision float on the range 0 to 1.
        /// </summary>
        public float Gf
        {
            get => G / 255.0f;
            set => G = (byte)MathF.Round(G * 255.0f);
        }
        /// <summary>
        /// The Blue component of the Color8 represented as a single-precision float on the range 0 to 1.
        /// </summary>
        public float Bf
        {
            get => B / 255.0f;
            set => B = (byte)MathF.Round(B * 255.0f);
        }
        /// <summary>
        /// The Alpha component of the Color8 represented as a single-precision float on the range 0 to 1.
        /// </summary>
        public float Af
        {
            get => A / 255.0f;
            set => A = (byte)MathF.Round(A * 255.0f);
        }

        /// <summary>
        /// The Hue of the color, represented on the range 0 to 1.
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
        /// The Saturation of the color, represented on the range 0 to 1.
        /// </summary>
        public float S
        {
            get
            {
                float max = MathF.Max(Rf, MathF.Max(Gf, Bf));
                float min = MathF.Min(Rf, MathF.Min(Gf, Bf));

                float delta = max - min;

                return max != 0 ? delta / max : 0;
            }
            set
            {
                this = FromHSV(H, value, V, A);
            }
        }

        /// <summary>
        /// The brightness value of the color, represented on the range 0 to 1.
        /// </summary>
        public float V
        {
            get => MathF.Max(Rf, MathF.Max(Gf, Bf));
            set => FromHSV(H, S, value, A);
        }

        /// <summary>
        /// Returns the Color8 component specified by RGBA index.
        /// </summary>
        /// <value>The RGBA index. Valid values are 0 to 3.</value>
        public byte this[int index]
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
        /// Returns a new Color8 resulting from blending this Color8 over another.
        /// </summary>
        /// <param name="over">The base Color8 to blend over.</param>
        /// <returns>The new blended Color8.</returns>
        public Color8 Blend(Color8 over)
        {
            float overAf = over.Af;
            float af = Af;
            float sa = 1.0f - overAf;
            float alpha = af * sa + overAf;

            if (alpha == 0)
            {
                return new Color8(0, 0, 0, 0);
            }

            Color res;
            res.R = (Rf * af * sa + over.Rf * overAf) / alpha;
            res.G = (Gf * af * sa + over.Gf * overAf) / alpha;
            res.B = (Bf * af * sa + over.Bf * overAf) / alpha;
            res.A = alpha;

            return new Color8(res);
        }

        /// <summary>
        /// Returns a new Color8 resulting from making this Color8 darker by the specified ratio.
        /// </summary>
        /// <param name="amount">Value between 0 and 255 to make the Color8 darker by.</param>
        /// <returns>A Color8 darkened by the given amount.</returns>
        public Color8 Darkened(float amount)
        {
            Color8 res = this;
            res.Rf = res.Rf * (1.0f - amount);
            res.Gf = res.Gf * (1.0f - amount);
            res.Bf = res.Bf * (1.0f - amount);
            return res;
        }

        /// <summary>
        /// Returns the inverted version of this Color8 with components (255 - R, 255 - G, 255 - B, A).
        /// </summary>
        /// <returns>The inverted Color8.</returns>
        public Color8 Inverted()
        {
            return new Color8(
                255 - R,
                255 - G,
                255 - B,
                A
            );
        }

        /// <summary>
        /// Interpolates between this Color8 and a given Color8 c by a ratio t.
        /// </summary>
        /// <param name="color">The second Color8 to interpolate with.</param>
        /// <param name="t">The interpolation ratio between this and c.</param>
        /// <returns>A new interpolated Color8.</returns>
        public Color8 Lerp(Color8 color, float t)
        {
            var res = this;

            res.R += (byte)MathF.Round(t * (color.R - R));
            res.G += (byte)MathF.Round(t * (color.G - G));
            res.B += (byte)MathF.Round(t * (color.B - B));
            res.A += (byte)MathF.Round(t * (color.A - A));

            return res;
        }

        /// <summary>
        /// Returns a new Color8 resulting from making this Color8 lighter by the specified ratio.
        /// </summary>
        /// <param name="amount">Value between 0 and 1 to make the Color8 lighter by.</param>
        /// <returns>A Color8 lightened by the given amount.</returns>
        public Color8 Lightened(float amount)
        {
            Color8 res = this;
            res.Rf = res.Rf + (1.0f - res.R) * amount;
            res.Gf = res.Gf + (1.0f - res.G) * amount;
            res.Bf = res.Bf + (1.0f - res.B) * amount;
            return res;
        }
        #endregion Public methods

        #region Export methods
        /// <summary>
        /// Packs the Color8 in ARGB format into a 32-bit integer by storing the channels as four 8-bit values.
        /// ARGB is the format traditionally used by many Microsoft APIs.
        /// </summary>
        /// <returns>A 32-bit int representing this Color8.</returns>
        public int ToArgb32()
        {
            int c = A;
            c <<= 8;
            c |= R;
            c <<= 8;
            c |= G;
            c <<= 8;
            c |= B;

            return c;
        }

        /// <summary>
        /// Packs the Color8 in RGBA format into a 32-bit integer by storing the channels as four 8-bit values.
        /// RGBA is the recommended format and is the most commonly used format.
        /// </summary>
        /// <returns>A 32-bit int representing this Color8.</returns>
        public int ToRgba32()
        {
            int c = R;
            c <<= 8;
            c |= G;
            c <<= 8;
            c |= B;
            c <<= 8;
            c |= A;

            return c;
        }

        /// <summary>
        /// Returns the HTML hexadecimal string representation of this Color8 in RGBA format.
        /// </summary>
        /// <param name="alpha">Whether or not to include alpha at the end.</param>
        /// <returns>An HTML hexadecimal string representation of this Color8.</returns>
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
        /// Converts the given Color8 into HSV float values. Same result as with using the HSV properties, but faster.
        /// </summary>
        /// <param name="hue">The hue output.</param>
        /// <param name="saturation">The saturation output.</param>
        /// <param name="value">The brightness value output.</param>
        public void ToHsv(out float hue, out float saturation, out float value)
        {
            float rf = Rf, gf = Gf, bf = Bf;
            float max = MathF.Max(rf, MathF.Max(gf, bf));
            float min = MathF.Min(rf, MathF.Min(gf, bf));

            float delta = max - min;

            if (delta == 0)
            {
                hue = 0;
            }
            else
            {
                if (rf == max)
                    hue = (gf - bf) / delta; // Between yellow & magenta
                else if (gf == max)
                    hue = 2 + (bf - rf) / delta; // Between cyan & yellow
                else
                    hue = 4 + (rf - gf) / delta; // Between magenta & cyan

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
        /// Constructs a Color8 with the given byte values.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        /// <param name="a">Alpha component, optional.</param>
        public Color8(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs a new Color8 from four single-precision floats.
        /// </summary>
        /// <param name="rf">The Red component of the Color8 represented as a single-precision float on the range 0 to 1.</param>
        /// <param name="gf">The Green component of the Color8 represented as a single-precision float on the range 0 to 1.</param>
        /// <param name="bf">The Blue component of the Color8 represented as a single-precision float on the range 0 to 1.</param>
        /// <param name="af">The Alpha component of the Color8 represented as a single-precision float on the range 0 to 1.</param>
        /// <returns></returns>
        public Color8(float rf, float gf, float bf, float af = 1.0f)
        {
            R = (byte)MathF.Round(rf * 255.0f);
            G = (byte)MathF.Round(gf * 255.0f);
            B = (byte)MathF.Round(bf * 255.0f);
            A = (byte)MathF.Round(af * 255.0f);
        }
        /// <summary>
        /// Constructs a grayscale Color with the given float value(s).
        /// </summary>
        /// <param name="v">Value to assign to the RGB components.</param>
        /// <param name="a">Alpha component, optional.</param>
        public Color8(byte v, byte a = 255)
        {
            R = v;
            G = v;
            B = v;
            A = a;
        }

        /// <summary>
        /// Constructs a grayscale Color with the given float value(s).
        /// </summary>
        /// <param name="vf">Value to assign to the RGB components.</param>
        /// <param name="af">Alpha component, optional.</param>
        public Color8(float vf, float af = 1.0f)
        {
            R = (byte)MathF.Round(vf * 255.0f);
            G = (byte)MathF.Round(vf * 255.0f);
            B = (byte)MathF.Round(vf * 255.0f);
            A = (byte)MathF.Round(af * 255.0f);
        }

        /// <summary>
        /// Constructs a Color8 from another Color8 and an alpha value.
        /// </summary>
        /// <param name="color">Color8 to construct from.</param>
        /// <param name="alpha">Alpha component, optional.</param>
        public Color8(Color8 color, byte alpha = 255)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        /// <summary>
        /// Constructs a Color8 from a Color and an alpha value.
        /// </summary>
        /// <param name="color">Color8 to construct from.</param>
        /// <param name="alpha">Alpha component, optional.</param>
        public Color8(Color color, byte alpha = 255)
        {
            R = color.R8;
            G = color.G8;
            B = color.B8;
            A = color.A8;
        }

        /// <summary>
        /// Constructs a new Color8 using a 32-bit integer, treating its bits as four packed 8-bit values.
        /// </summary>
        /// <param name="rgba">The integer containing packed values in RGBA format.</param>
        public Color8(int rgba)
        {
            A = (byte)(rgba & 0xFF);
            rgba >>= 8;
            B = (byte)(rgba & 0xFF);
            rgba >>= 8;
            G = (byte)(rgba & 0xFF);
            rgba >>= 8;
            R = (byte)(rgba & 0xFF);
        }

        /// <summary>
        /// Constructs a new Color8 using a 64-bit integer, treating its bits as four packed 16-bit values.
        /// Only the most significant 8 bits per channel are kept, the rest are discarded.
        /// </summary>
        /// <param name="rgba">The integer containing packed values in RGBA format.</param>
        public Color8(long rgba)
        {
            rgba >>= 8; // Ignore least significant 8 bits, and we move 16 over each time.
            A = (byte)(rgba & 0xFF);
            rgba >>= 16;
            B = (byte)(rgba & 0xFF);
            rgba >>= 16;
            G = (byte)(rgba & 0xFF);
            rgba >>= 16;
            R = (byte)(rgba & 0xFF);
        }

        /// <summary>
        /// Constructs a new Color8 from the given HSV byte values.
        /// </summary>
        /// <param name="hue">The hue input.</param>
        /// <param name="saturation">The saturation input.</param>
        /// <param name="value">The brightness value input.</param>
        /// <param name="alpha">The alpha channel opacity input.</param>
        /// <returns>A new Color8 constructed from the values.</returns>
        public static Color8 FromHSV(float hue, float saturation, float value, float alpha = 1.0f)
        {
            if (saturation == 0)
            {
                // acp_hromatic (grey)
                return new Color8(value, value, value, alpha);
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
                    return new Color8(value, t, p, alpha);
                case 1: // Green is the dominant color
                    return new Color8(q, value, p, alpha);
                case 2: // Green is the dominant color
                    return new Color8(p, value, t, alpha);
                case 3: // Blue is the dominant color
                    return new Color8(p, q, value, alpha);
                case 4: // Blue is the dominant color
                    return new Color8(t, p, value, alpha);
                default: // Red is the dominant color (case 5)
                    return new Color8(value, p, q, alpha);
            }
        }

/*

        /// <summary>
        /// Creates a Color8 structure from the specified name of a predefined Color8.
        /// </summary>
        /// <param name="name">The name of the Color8.</param>
        /// <param name="alpha">Alpha component, optional.</param>
        /// <returns></returns>
        public static Color8 FromName(string name, byte? alpha = null)
        {
            name = name.Replace(" ", string.Empty);
            name = name.Replace("-", string.Empty);
            name = name.Replace("_", string.Empty);
            name = name.Replace("'", string.Empty);
            name = name.Replace(".", string.Empty);
            name = name.ToLower();

            if (!Colors8.namedColors.ContainsKey(name))
            {
                throw new ArgumentOutOfRangeException($"Invalid Color Name: {name}");
            }

            Color8 Color8 = Colors8.namedColors[name];
            if (alpha != null)
                Color8.A = (byte)alpha;
            return Color8;
        }
*/

        /// <summary>
        /// Constructs a new Color8 from the given string representing a hexadecimal color code in RGBA format.
        /// </summary>
        /// <param name="rgba">String representing a hexadecimal color code in RGBA format.</param>
        public Color8(string rgba)
        {
            if (rgba.Length == 0)
            {
                R = 0;
                G = 0;
                B = 0;
                A = 255;
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
            int cache = ColorUtils.ParseCol8(rgba, 0);
            if (cache < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Red part is not valid hexadecimal: " + rgba);
            }
            R = (byte)cache;

            cache = ColorUtils.ParseCol8(rgba, 2);
            if (cache < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Green part is not valid hexadecimal: " + rgba);
            }
            G = (byte)cache;

            cache = ColorUtils.ParseCol8(rgba, 4);
            if (cache < 0)
            {
                throw new ArgumentOutOfRangeException("Invalid color code. Blue part is not valid hexadecimal: " + rgba);
            }
            B = (byte)cache;

            if (alpha)
            {
                cache = ColorUtils.ParseCol8(rgba, 6);
                if (cache < 0)
                {
                    throw new ArgumentOutOfRangeException("Invalid color code. Red part is not valid hexadecimal: " + rgba);
                }
                A = (byte)cache;
            }
            else
            {
                A = 255;
            }
        }
        #endregion Constructors

        #region Public operators
        public static implicit operator Color(Color8 color)
        {
            return new Color(color);
        }

        public static explicit operator Color8(Color color)
        {
            return new Color8(color);
        }

        public static bool operator ==(Color8 left, Color8 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color8 left, Color8 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Checks if this Color8 is exactly equal to the given object.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <returns>Whether or not they are exactly equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Color8)
            {
                return Equals((Color8)obj);
            }

            return false;
        }

        /// <summary>
        /// Checks if this Color8 is exactly equal to the given Color8.
        /// </summary>
        /// <param name="other">The other Color8.</param>
        /// <returns>Whether or not they are exactly equal.</returns>
        public bool Equals(Color8 other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        }

        /// <summary>
        /// Converts the Color8 to a string in "R,G,B,A" format.
        /// </summary>
        /// <returns>String representing this Color8 in "R,G,B,A" format.</returns>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", R.ToString(), G.ToString(), B.ToString(), A.ToString());
        }

        /// <summary>
        /// Converts the Color8 to a string in "R,G,B,A" format, with custom formatting options passed to each value.
        /// </summary>
        /// <param name="format">Custom formatting to use for each value.</param>
        /// <returns>String representing this Color8 in "R,G,B,A" format.</returns>
        public string ToString(string? format)
        {
            return string.Format("{0},{1},{2},{3}", R.ToString(format), G.ToString(format), B.ToString(format), A.ToString(format));
        }

        /// <summary>
        /// Converts the Color8 to a string in "R,G,B,A" format, with custom formatting options passed to each value.
        /// </summary>
        /// <param name="format">Custom formatting to use for each value.</param>
        /// <param name="formatProvider">The format provider to use when formatting elements.</param>
        /// <returns>String representing this Color8 in "R,G,B,A" format.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format("{0},{1},{2},{3}", R.ToString(format, formatProvider), G.ToString(format, formatProvider), B.ToString(format, formatProvider), A.ToString(format, formatProvider));
        }
        #endregion Public operators
    }
}
