// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Numerics
{
    internal static partial class ColorUtils
    {
        internal static byte ParseCol8(string str, int offset) { throw null; }
        internal static String ToHex8(float val) { throw null; }
        internal static String ToHex8(byte val) { throw null; }
    }

    public partial struct Color : System.IEquatable<Color>, System.IFormattable
    {
        public float R;
        public float G;
        public float B;
        public float A;
        public byte R8 { get { throw null; } set { } }
        public byte G8 { get { throw null; } set { } }
        public byte B8 { get { throw null; } set { } }
        public byte A8 { get { throw null; } set { } }
        public float H { get { throw null; } set { } }
        public float S { get { throw null; } set { } }
        public float V { get { throw null; } set { } }
        public float this[int index] { get { throw null; } set { } }
        public System.Numerics.Color Blend(System.Numerics.Color over) { throw null; }
        public System.Numerics.Color Darkened(float amount) { throw null; }
        public System.Numerics.Color Inverted() { throw null; }
        public System.Numerics.Color Lightened(float amount) { throw null; }
        public System.Numerics.Color Lerp(System.Numerics.Color color, float t) { throw null; }
        public int ToArgb32() { throw null; }
        public long ToArgb64() { throw null; }
        public int ToRgba32() { throw null; }
        public long ToRgba64() { throw null; }
        public string ToHtml(bool alpha = true) { throw null; }
        public void ToHsv(out float hue, out float saturation, out float value) { throw null; }
        public Color(int rgba) { throw null; }
        public Color(long rgba) { throw null; }
        public Color(string rgba) { throw null; }
        public Color(byte v8, byte a8 = 255) { throw null; }
        public Color(float v, float a = 1.0f) { throw null; }
        public Color(byte r8, byte g8, byte b8, byte a8 = 255) { throw null; }
        public Color(float r, float g, float b, float a = 1.0f) { throw null; }
        public Color(System.Numerics.Color color, float alpha = 1.0f) { throw null; }
        public Color(System.Numerics.Color8 color, float alpha = 1.0f) { throw null; }
        public static System.Numerics.Color FromHSV(float hue, float saturation, float value, float alpha = 1.0f) { throw null; }
        //public static System.Numerics.Color FromName(string name, float? alpha = null) { throw null; }
        public static bool operator ==(System.Numerics.Color left, System.Numerics.Color right) { throw null; }
        public static bool operator !=(System.Numerics.Color left, System.Numerics.Color right) { throw null; }
        public bool Equals(System.Numerics.Color other) { throw null; }
        public override bool Equals(object? obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public override readonly string ToString() { throw null; }
        public readonly string ToString(string? format) { throw null; }
        public readonly string ToString(string? format, System.IFormatProvider? formatProvider) { throw null; }
    }

    public partial struct Color8 : System.IEquatable<Color8>, System.IFormattable
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
        public float Rf { get { throw null; } set { } }
        public float Gf { get { throw null; } set { } }
        public float Bf { get { throw null; } set { } }
        public float Af { get { throw null; } set { } }
        public float H { get { throw null; } set { } }
        public float S { get { throw null; } set { } }
        public float V { get { throw null; } set { } }
        public byte this[int index] { get { throw null; } set { } }
        public System.Numerics.Color8 Blend(System.Numerics.Color8 over) { throw null; }
        public System.Numerics.Color8 Darkened(float amount) { throw null; }
        public System.Numerics.Color8 Inverted() { throw null; }
        public System.Numerics.Color8 Lightened(float amount) { throw null; }
        public System.Numerics.Color8 Lerp(System.Numerics.Color8 color, float t) { throw null; }
        public int ToArgb32() { throw null; }
        public int ToRgba32() { throw null; }
        public string ToHtml(bool alpha = true) { throw null; }
        public void ToHsv(out float hue, out float saturation, out float value) { throw null; }
        public Color8(int rgba) { throw null; }
        public Color8(long rgba) { throw null; }
        public Color8(string rgba) { throw null; }
        public Color8(byte v, byte a = 255) { throw null; }
        public Color8(float vf, float af = 1.0f) { throw null; }
        public Color8(byte r, byte g, byte b, byte a = 255) { throw null; }
        public Color8(float rf, float gf, float bf, float af = 1.0f) { throw null; }
        public Color8(System.Numerics.Color color, byte alpha = 255) { throw null; }
        public Color8(System.Numerics.Color8 color, byte alpha = 255) { throw null; }
        public static System.Numerics.Color8 FromHSV(float hue, float saturation, float value, float alpha = 1.0f) { throw null; }
        //public static System.Numerics.Color8 FromName(string name, byte? alpha = null) { throw null; }
        public static implicit operator Color(Color8 color) { throw null; }
        public static explicit operator Color8(Color color) { throw null; }
        public static bool operator ==(System.Numerics.Color8 left, System.Numerics.Color8 right) { throw null; }
        public static bool operator !=(System.Numerics.Color8 left, System.Numerics.Color8 right) { throw null; }
        public bool Equals(System.Numerics.Color8 other) { throw null; }
        public override bool Equals(object? obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public override readonly string ToString() { throw null; }
        public readonly string ToString(string? format) { throw null; }
        public readonly string ToString(string? format, System.IFormatProvider? formatProvider) { throw null; }
    }

    public static partial class Colors
    {
        public static System.Numerics.Color AliceBlue { get { throw null; } }
        public static System.Numerics.Color AntiqueWhite { get { throw null; } }
        public static System.Numerics.Color Aqua { get { throw null; } }
        public static System.Numerics.Color Aquamarine { get { throw null; } }
        public static System.Numerics.Color Azure { get { throw null; } }
        public static System.Numerics.Color Beige { get { throw null; } }
        public static System.Numerics.Color Bisque { get { throw null; } }
        public static System.Numerics.Color Black { get { throw null; } }
        public static System.Numerics.Color BlanchedAlmond { get { throw null; } }
        public static System.Numerics.Color Blue { get { throw null; } }
        public static System.Numerics.Color BlueViolet { get { throw null; } }
        public static System.Numerics.Color Brown { get { throw null; } }
        public static System.Numerics.Color BurlyWood { get { throw null; } }
        public static System.Numerics.Color Burgundy { get { throw null; } }
        public static System.Numerics.Color CadetBlue { get { throw null; } }
        public static System.Numerics.Color Chartreuse { get { throw null; } }
        public static System.Numerics.Color Chocolate { get { throw null; } }
        public static System.Numerics.Color Coral { get { throw null; } }
        public static System.Numerics.Color Cornflower { get { throw null; } }
        public static System.Numerics.Color Cornsilk { get { throw null; } }
        public static System.Numerics.Color Crimson { get { throw null; } }
        public static System.Numerics.Color Cyan { get { throw null; } }
        public static System.Numerics.Color DarkBlue { get { throw null; } }
        public static System.Numerics.Color DarkCyan { get { throw null; } }
        public static System.Numerics.Color DarkGoldenrod { get { throw null; } }
        public static System.Numerics.Color DarkGray { get { throw null; } }
        public static System.Numerics.Color DarkGreen { get { throw null; } }
        public static System.Numerics.Color DarkKhaki { get { throw null; } }
        public static System.Numerics.Color DarkMagenta { get { throw null; } }
        public static System.Numerics.Color DarkOliveGreen { get { throw null; } }
        public static System.Numerics.Color DarkOrange { get { throw null; } }
        public static System.Numerics.Color DarkOrchid { get { throw null; } }
        public static System.Numerics.Color DarkRed { get { throw null; } }
        public static System.Numerics.Color DarkSalmon { get { throw null; } }
        public static System.Numerics.Color DarkSeaGreen { get { throw null; } }
        public static System.Numerics.Color DarkSlateBlue { get { throw null; } }
        public static System.Numerics.Color DarkSlateGray { get { throw null; } }
        public static System.Numerics.Color DarkTurquoise { get { throw null; } }
        public static System.Numerics.Color DarkViolet { get { throw null; } }
        public static System.Numerics.Color DeepPink { get { throw null; } }
        public static System.Numerics.Color DeepSkyBlue { get { throw null; } }
        public static System.Numerics.Color DimGray { get { throw null; } }
        public static System.Numerics.Color DodgerBlue { get { throw null; } }
        public static System.Numerics.Color FireBrick { get { throw null; } }
        public static System.Numerics.Color FloralWhite { get { throw null; } }
        public static System.Numerics.Color ForestGreen { get { throw null; } }
        public static System.Numerics.Color Fuchsia { get { throw null; } }
        public static System.Numerics.Color Gainsboro { get { throw null; } }
        public static System.Numerics.Color GhostWhite { get { throw null; } }
        public static System.Numerics.Color Gold { get { throw null; } }
        public static System.Numerics.Color Goldenrod { get { throw null; } }
        public static System.Numerics.Color Gray { get { throw null; } }
        public static System.Numerics.Color Green { get { throw null; } }
        public static System.Numerics.Color GreenYellow { get { throw null; } }
        public static System.Numerics.Color Honeydew { get { throw null; } }
        public static System.Numerics.Color HotPink { get { throw null; } }
        public static System.Numerics.Color IndianRed { get { throw null; } }
        public static System.Numerics.Color Indigo { get { throw null; } }
        public static System.Numerics.Color Ivory { get { throw null; } }
        public static System.Numerics.Color Khaki { get { throw null; } }
        public static System.Numerics.Color Lavender { get { throw null; } }
        public static System.Numerics.Color LavenderBlush { get { throw null; } }
        public static System.Numerics.Color LawnGreen { get { throw null; } }
        public static System.Numerics.Color LemonChiffon { get { throw null; } }
        public static System.Numerics.Color LightBlue { get { throw null; } }
        public static System.Numerics.Color LightCoral { get { throw null; } }
        public static System.Numerics.Color LightCyan { get { throw null; } }
        public static System.Numerics.Color LightGoldenrod { get { throw null; } }
        public static System.Numerics.Color LightGray { get { throw null; } }
        public static System.Numerics.Color LightGreen { get { throw null; } }
        public static System.Numerics.Color LightPink { get { throw null; } }
        public static System.Numerics.Color LightSalmon { get { throw null; } }
        public static System.Numerics.Color LightSeaGreen { get { throw null; } }
        public static System.Numerics.Color LightSkyBlue { get { throw null; } }
        public static System.Numerics.Color LightSlateGray { get { throw null; } }
        public static System.Numerics.Color LightSteelBlue { get { throw null; } }
        public static System.Numerics.Color LightYellow { get { throw null; } }
        public static System.Numerics.Color Lime { get { throw null; } }
        public static System.Numerics.Color Limegreen { get { throw null; } }
        public static System.Numerics.Color Linen { get { throw null; } }
        public static System.Numerics.Color Magenta { get { throw null; } }
        public static System.Numerics.Color Maroon { get { throw null; } }
        public static System.Numerics.Color MaroonX11 { get { throw null; } }
        public static System.Numerics.Color MediumAquamarine { get { throw null; } }
        public static System.Numerics.Color MediumBlue { get { throw null; } }
        public static System.Numerics.Color MediumOrchid { get { throw null; } }
        public static System.Numerics.Color MediumPurple { get { throw null; } }
        public static System.Numerics.Color MediumSeaGreen { get { throw null; } }
        public static System.Numerics.Color MediumSlateBlue { get { throw null; } }
        public static System.Numerics.Color MediumSpringGreen { get { throw null; } }
        public static System.Numerics.Color MediumTurquoise { get { throw null; } }
        public static System.Numerics.Color MediumVioletRed { get { throw null; } }
        public static System.Numerics.Color MidnightBlue { get { throw null; } }
        public static System.Numerics.Color MintCream { get { throw null; } }
        public static System.Numerics.Color MistyRose { get { throw null; } }
        public static System.Numerics.Color Moccasin { get { throw null; } }
        public static System.Numerics.Color NavajoWhite { get { throw null; } }
        public static System.Numerics.Color NavyBlue { get { throw null; } }
        public static System.Numerics.Color OldLace { get { throw null; } }
        public static System.Numerics.Color Olive { get { throw null; } }
        public static System.Numerics.Color OliveDrab { get { throw null; } }
        public static System.Numerics.Color Orange { get { throw null; } }
        public static System.Numerics.Color OrangeRed { get { throw null; } }
        public static System.Numerics.Color Orchid { get { throw null; } }
        public static System.Numerics.Color PaleGoldenrod { get { throw null; } }
        public static System.Numerics.Color PaleGreen { get { throw null; } }
        public static System.Numerics.Color PaleTurquoise { get { throw null; } }
        public static System.Numerics.Color PaleVioletRed { get { throw null; } }
        public static System.Numerics.Color PapayaWhip { get { throw null; } }
        public static System.Numerics.Color PeachPuff { get { throw null; } }
        public static System.Numerics.Color Peru { get { throw null; } }
        public static System.Numerics.Color Pink { get { throw null; } }
        public static System.Numerics.Color Plum { get { throw null; } }
        public static System.Numerics.Color PowderBlue { get { throw null; } }
        public static System.Numerics.Color Purple { get { throw null; } }
        public static System.Numerics.Color RebeccaPurple { get { throw null; } }
        public static System.Numerics.Color Red { get { throw null; } }
        public static System.Numerics.Color RosyBrown { get { throw null; } }
        public static System.Numerics.Color RoyalBlue { get { throw null; } }
        public static System.Numerics.Color SaddleBrown { get { throw null; } }
        public static System.Numerics.Color Salmon { get { throw null; } }
        public static System.Numerics.Color SandyBrown { get { throw null; } }
        public static System.Numerics.Color SeaGreen { get { throw null; } }
        public static System.Numerics.Color SeaShell { get { throw null; } }
        public static System.Numerics.Color Sienna { get { throw null; } }
        public static System.Numerics.Color Silver { get { throw null; } }
        public static System.Numerics.Color SkyBlue { get { throw null; } }
        public static System.Numerics.Color SlateBlue { get { throw null; } }
        public static System.Numerics.Color SlateGray { get { throw null; } }
        public static System.Numerics.Color Snow { get { throw null; } }
        public static System.Numerics.Color SpringGreen { get { throw null; } }
        public static System.Numerics.Color SteelBlue { get { throw null; } }
        public static System.Numerics.Color Tan { get { throw null; } }
        public static System.Numerics.Color Teal { get { throw null; } }
        public static System.Numerics.Color Thistle { get { throw null; } }
        public static System.Numerics.Color Tomato { get { throw null; } }
        public static System.Numerics.Color Transparent { get { throw null; } }
        public static System.Numerics.Color Turquoise { get { throw null; } }
        public static System.Numerics.Color Violet { get { throw null; } }
        public static System.Numerics.Color WebGreen { get { throw null; } }
        public static System.Numerics.Color WebGray { get { throw null; } }
        public static System.Numerics.Color WebMaroon { get { throw null; } }
        public static System.Numerics.Color WebPurple { get { throw null; } }
        public static System.Numerics.Color Wheat { get { throw null; } }
        public static System.Numerics.Color White { get { throw null; } }
        public static System.Numerics.Color WhiteSmoke { get { throw null; } }
        public static System.Numerics.Color Yellow { get { throw null; } }
        public static System.Numerics.Color YellowGreen { get { throw null; } }
    }

    public static partial class Colors8
    {
        public static System.Numerics.Color8 AliceBlue { get { throw null; } }
        public static System.Numerics.Color8 AntiqueWhite { get { throw null; } }
        public static System.Numerics.Color8 Aqua { get { throw null; } }
        public static System.Numerics.Color8 Aquamarine { get { throw null; } }
        public static System.Numerics.Color8 Azure { get { throw null; } }
        public static System.Numerics.Color8 Beige { get { throw null; } }
        public static System.Numerics.Color8 Bisque { get { throw null; } }
        public static System.Numerics.Color8 Black { get { throw null; } }
        public static System.Numerics.Color8 BlanchedAlmond { get { throw null; } }
        public static System.Numerics.Color8 Blue { get { throw null; } }
        public static System.Numerics.Color8 BlueViolet { get { throw null; } }
        public static System.Numerics.Color8 Brown { get { throw null; } }
        public static System.Numerics.Color8 BurlyWood { get { throw null; } }
        public static System.Numerics.Color8 Burgundy { get { throw null; } }
        public static System.Numerics.Color8 CadetBlue { get { throw null; } }
        public static System.Numerics.Color8 Chartreuse { get { throw null; } }
        public static System.Numerics.Color8 Chocolate { get { throw null; } }
        public static System.Numerics.Color8 Coral { get { throw null; } }
        public static System.Numerics.Color8 Cornflower { get { throw null; } }
        public static System.Numerics.Color8 Cornsilk { get { throw null; } }
        public static System.Numerics.Color8 Crimson { get { throw null; } }
        public static System.Numerics.Color8 Cyan { get { throw null; } }
        public static System.Numerics.Color8 DarkBlue { get { throw null; } }
        public static System.Numerics.Color8 DarkCyan { get { throw null; } }
        public static System.Numerics.Color8 DarkGoldenrod { get { throw null; } }
        public static System.Numerics.Color8 DarkGray { get { throw null; } }
        public static System.Numerics.Color8 DarkGreen { get { throw null; } }
        public static System.Numerics.Color8 DarkKhaki { get { throw null; } }
        public static System.Numerics.Color8 DarkMagenta { get { throw null; } }
        public static System.Numerics.Color8 DarkOliveGreen { get { throw null; } }
        public static System.Numerics.Color8 DarkOrange { get { throw null; } }
        public static System.Numerics.Color8 DarkOrchid { get { throw null; } }
        public static System.Numerics.Color8 DarkRed { get { throw null; } }
        public static System.Numerics.Color8 DarkSalmon { get { throw null; } }
        public static System.Numerics.Color8 DarkSeaGreen { get { throw null; } }
        public static System.Numerics.Color8 DarkSlateBlue { get { throw null; } }
        public static System.Numerics.Color8 DarkSlateGray { get { throw null; } }
        public static System.Numerics.Color8 DarkTurquoise { get { throw null; } }
        public static System.Numerics.Color8 DarkViolet { get { throw null; } }
        public static System.Numerics.Color8 DeepPink { get { throw null; } }
        public static System.Numerics.Color8 DeepSkyBlue { get { throw null; } }
        public static System.Numerics.Color8 DimGray { get { throw null; } }
        public static System.Numerics.Color8 DodgerBlue { get { throw null; } }
        public static System.Numerics.Color8 FireBrick { get { throw null; } }
        public static System.Numerics.Color8 FloralWhite { get { throw null; } }
        public static System.Numerics.Color8 ForestGreen { get { throw null; } }
        public static System.Numerics.Color8 Fuchsia { get { throw null; } }
        public static System.Numerics.Color8 Gainsboro { get { throw null; } }
        public static System.Numerics.Color8 GhostWhite { get { throw null; } }
        public static System.Numerics.Color8 Gold { get { throw null; } }
        public static System.Numerics.Color8 Goldenrod { get { throw null; } }
        public static System.Numerics.Color8 Gray { get { throw null; } }
        public static System.Numerics.Color8 Green { get { throw null; } }
        public static System.Numerics.Color8 GreenYellow { get { throw null; } }
        public static System.Numerics.Color8 Honeydew { get { throw null; } }
        public static System.Numerics.Color8 HotPink { get { throw null; } }
        public static System.Numerics.Color8 IndianRed { get { throw null; } }
        public static System.Numerics.Color8 Indigo { get { throw null; } }
        public static System.Numerics.Color8 Ivory { get { throw null; } }
        public static System.Numerics.Color8 Khaki { get { throw null; } }
        public static System.Numerics.Color8 Lavender { get { throw null; } }
        public static System.Numerics.Color8 LavenderBlush { get { throw null; } }
        public static System.Numerics.Color8 LawnGreen { get { throw null; } }
        public static System.Numerics.Color8 LemonChiffon { get { throw null; } }
        public static System.Numerics.Color8 LightBlue { get { throw null; } }
        public static System.Numerics.Color8 LightCoral { get { throw null; } }
        public static System.Numerics.Color8 LightCyan { get { throw null; } }
        public static System.Numerics.Color8 LightGoldenrod { get { throw null; } }
        public static System.Numerics.Color8 LightGray { get { throw null; } }
        public static System.Numerics.Color8 LightGreen { get { throw null; } }
        public static System.Numerics.Color8 LightPink { get { throw null; } }
        public static System.Numerics.Color8 LightSalmon { get { throw null; } }
        public static System.Numerics.Color8 LightSeaGreen { get { throw null; } }
        public static System.Numerics.Color8 LightSkyBlue { get { throw null; } }
        public static System.Numerics.Color8 LightSlateGray { get { throw null; } }
        public static System.Numerics.Color8 LightSteelBlue { get { throw null; } }
        public static System.Numerics.Color8 LightYellow { get { throw null; } }
        public static System.Numerics.Color8 Lime { get { throw null; } }
        public static System.Numerics.Color8 Limegreen { get { throw null; } }
        public static System.Numerics.Color8 Linen { get { throw null; } }
        public static System.Numerics.Color8 Magenta { get { throw null; } }
        public static System.Numerics.Color8 Maroon { get { throw null; } }
        public static System.Numerics.Color8 MaroonX11 { get { throw null; } }
        public static System.Numerics.Color8 MediumAquamarine { get { throw null; } }
        public static System.Numerics.Color8 MediumBlue { get { throw null; } }
        public static System.Numerics.Color8 MediumOrchid { get { throw null; } }
        public static System.Numerics.Color8 MediumPurple { get { throw null; } }
        public static System.Numerics.Color8 MediumSeaGreen { get { throw null; } }
        public static System.Numerics.Color8 MediumSlateBlue { get { throw null; } }
        public static System.Numerics.Color8 MediumSpringGreen { get { throw null; } }
        public static System.Numerics.Color8 MediumTurquoise { get { throw null; } }
        public static System.Numerics.Color8 MediumVioletRed { get { throw null; } }
        public static System.Numerics.Color8 MidnightBlue { get { throw null; } }
        public static System.Numerics.Color8 MintCream { get { throw null; } }
        public static System.Numerics.Color8 MistyRose { get { throw null; } }
        public static System.Numerics.Color8 Moccasin { get { throw null; } }
        public static System.Numerics.Color8 NavajoWhite { get { throw null; } }
        public static System.Numerics.Color8 NavyBlue { get { throw null; } }
        public static System.Numerics.Color8 OldLace { get { throw null; } }
        public static System.Numerics.Color8 Olive { get { throw null; } }
        public static System.Numerics.Color8 OliveDrab { get { throw null; } }
        public static System.Numerics.Color8 Orange { get { throw null; } }
        public static System.Numerics.Color8 OrangeRed { get { throw null; } }
        public static System.Numerics.Color8 Orchid { get { throw null; } }
        public static System.Numerics.Color8 PaleGoldenrod { get { throw null; } }
        public static System.Numerics.Color8 PaleGreen { get { throw null; } }
        public static System.Numerics.Color8 PaleTurquoise { get { throw null; } }
        public static System.Numerics.Color8 PaleVioletRed { get { throw null; } }
        public static System.Numerics.Color8 PapayaWhip { get { throw null; } }
        public static System.Numerics.Color8 PeachPuff { get { throw null; } }
        public static System.Numerics.Color8 Peru { get { throw null; } }
        public static System.Numerics.Color8 Pink { get { throw null; } }
        public static System.Numerics.Color8 Plum { get { throw null; } }
        public static System.Numerics.Color8 PowderBlue { get { throw null; } }
        public static System.Numerics.Color8 Purple { get { throw null; } }
        public static System.Numerics.Color8 RebeccaPurple { get { throw null; } }
        public static System.Numerics.Color8 Red { get { throw null; } }
        public static System.Numerics.Color8 RosyBrown { get { throw null; } }
        public static System.Numerics.Color8 RoyalBlue { get { throw null; } }
        public static System.Numerics.Color8 SaddleBrown { get { throw null; } }
        public static System.Numerics.Color8 Salmon { get { throw null; } }
        public static System.Numerics.Color8 SandyBrown { get { throw null; } }
        public static System.Numerics.Color8 SeaGreen { get { throw null; } }
        public static System.Numerics.Color8 SeaShell { get { throw null; } }
        public static System.Numerics.Color8 Sienna { get { throw null; } }
        public static System.Numerics.Color8 Silver { get { throw null; } }
        public static System.Numerics.Color8 SkyBlue { get { throw null; } }
        public static System.Numerics.Color8 SlateBlue { get { throw null; } }
        public static System.Numerics.Color8 SlateGray { get { throw null; } }
        public static System.Numerics.Color8 Snow { get { throw null; } }
        public static System.Numerics.Color8 SpringGreen { get { throw null; } }
        public static System.Numerics.Color8 SteelBlue { get { throw null; } }
        public static System.Numerics.Color8 Tan { get { throw null; } }
        public static System.Numerics.Color8 Teal { get { throw null; } }
        public static System.Numerics.Color8 Thistle { get { throw null; } }
        public static System.Numerics.Color8 Tomato { get { throw null; } }
        public static System.Numerics.Color8 Transparent { get { throw null; } }
        public static System.Numerics.Color8 Turquoise { get { throw null; } }
        public static System.Numerics.Color8 Violet { get { throw null; } }
        public static System.Numerics.Color8 WebGreen { get { throw null; } }
        public static System.Numerics.Color8 WebGray { get { throw null; } }
        public static System.Numerics.Color8 WebMaroon { get { throw null; } }
        public static System.Numerics.Color8 WebPurple { get { throw null; } }
        public static System.Numerics.Color8 Wheat { get { throw null; } }
        public static System.Numerics.Color8 White { get { throw null; } }
        public static System.Numerics.Color8 WhiteSmoke { get { throw null; } }
        public static System.Numerics.Color8 Yellow { get { throw null; } }
        public static System.Numerics.Color8 YellowGreen { get { throw null; } }
    }
}
