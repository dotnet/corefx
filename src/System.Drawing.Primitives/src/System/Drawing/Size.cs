// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Numerics.Hashing;

namespace System.Drawing
{
    /**
     * Represents a dimension in 2D coordinate space
     */
    /// <summary>
    ///    Represents the size of a rectangular region
    ///    with an ordered pair of width and height.
    /// </summary>
    [Serializable]
    public struct Size : IEquatable<Size>
    {
        /// <summary>
        ///    Initializes a new instance of the <see cref='System.Drawing.Size'/> class.
        /// </summary>
        public static readonly Size Empty = new Size();

        private int width; // Do not rename (binary serialization) 
        private int height; // Do not rename (binary serialization) 

        /**
         * Create a new Size object from a point
         */
        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Drawing.Size'/> class from
        ///       the specified <see cref='System.Drawing.Point'/>.
        ///    </para>
        /// </summary>
        public Size(Point pt)
        {
            width = pt.X;
            height = pt.Y;
        }

        /**
         * Create a new Size object of the specified dimension
         */
        /// <summary>
        ///    Initializes a new instance of the <see cref='System.Drawing.Size'/> class from
        ///    the specified dimensions.
        /// </summary>
        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        ///    Converts the specified <see cref='System.Drawing.Size'/> to a
        /// <see cref='System.Drawing.SizeF'/>.
        /// </summary>
        public static implicit operator SizeF(Size p) => new SizeF(p.Width, p.Height);

        /// <summary>
        ///    <para>
        ///       Performs vector addition of two <see cref='System.Drawing.Size'/> objects.
        ///    </para>
        /// </summary>
        public static Size operator +(Size sz1, Size sz2) => Add(sz1, sz2);

        /// <summary>
        ///    <para>
        ///       Contracts a <see cref='System.Drawing.Size'/> by another <see cref='System.Drawing.Size'/>
        ///    </para>
        /// </summary>
        public static Size operator -(Size sz1, Size sz2) => Subtract(sz1, sz2);

        /// <summary>
        /// Multiplication operator that multiplies <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="left">Multiplier of type <see cref="int"/>.</param>
        /// <param name="right">Multiplicand of type <see cref="Size"/>.</param>
        /// <returns>Product of type <see cref="Size"/>.</returns>
        public static Size operator *(int left, Size right) => Multiply(right, left);

        /// <summary>
        /// Multiplication operator that multiplies <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="left">Multiplicand of type <see cref="Size"/>.</param>
        /// <param name="right">Multiplier of type <see cref="int"/>.</param>
        /// <returns>Product of type <see cref="Size"/>.</returns>
        public static Size operator *(Size left, int right) => Multiply(left, right);

        /// <summary>
        /// Division operator that divides <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
        /// </summary>
        /// <param name="left">Dividend of type <see cref="Size"/>.</param>
        /// <param name="right">Divisor of type <see cref="int"/>.</param>
        /// <returns>Result of type <see cref="Size"/>.</returns>
        public static Size operator /(Size left, int right)
        {
            int invDiv = 1 / right;
            return new Size(unchecked(left._width * invDiv), unchecked(left._height * invDiv));
        }

        /// <summary>
        /// Multiplication operator that multiplies <see cref="Size"/> by a <see cref="float"/> producing <see cref="SizeF"/>.
        /// </summary>
        /// <param name="left">Multiplier of type <see cref="float"/>.</param>
        /// <param name="right">Multiplicand of type <see cref="Size"/>.</param>
        /// <returns>Product of type <see cref="SizeF"/>.</returns>
        public static SizeF operator *(float left, Size right) => Multiply(right, left);

        /// <summary>
        /// Multiplication operator that multiplies <see cref="Size"/> by a <see cref="float"/> producing <see cref="SizeF"/>.
        /// </summary>
        /// <param name="left">Multiplicand of type <see cref="Size"/>.</param>
        /// <param name="right">Multiplier of type <see cref="float"/>.</param>
        /// <returns>Product of type <see cref="SizeF"/>.</returns>
        public static SizeF operator *(Size left, float right) => Multiply(left, right);

        /// <summary>
        /// Division operator that divides <see cref="Size"/> by a <see cref="float"/> producing <see cref="SizeF"/>.
        /// </summary>
        /// <param name="left">Dividend of type <see cref="Size"/>.</param>
        /// <param name="right">Divisor of type <see cref="int"/>.</param>
        /// <returns>Result of type <see cref="SizeF"/>.</returns>
        public static SizeF operator /(Size left, float right)
        {
            float invDiv = 1.0f / right;
            return new SizeF(left._width / invDiv, left._height / invDiv);
        }

        /// <summary>
        ///    Tests whether two <see cref='System.Drawing.Size'/> objects
        ///    are identical.
        /// </summary>
        public static bool operator ==(Size sz1, Size sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

        /// <summary>
        ///    <para>
        ///       Tests whether two <see cref='System.Drawing.Size'/> objects are different.
        ///    </para>
        /// </summary>
        public static bool operator !=(Size sz1, Size sz2) => !(sz1 == sz2);

        /// <summary>
        ///    Converts the specified <see cref='System.Drawing.Size'/> to a
        /// <see cref='System.Drawing.Point'/>.
        /// </summary>
        public static explicit operator Point(Size size) => new Point(size.Width, size.Height);

        /// <summary>
        ///    Tests whether this <see cref='System.Drawing.Size'/> has zero
        ///    width and height.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => width == 0 && height == 0;

        /**
         * Horizontal dimension
         */

        /// <summary>
        ///    <para>
        ///       Represents the horizontal component of this
        ///    <see cref='System.Drawing.Size'/>.
        ///    </para>
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /**
         * Vertical dimension
         */

        /// <summary>
        ///    Represents the vertical component of this
        /// <see cref='System.Drawing.Size'/>.
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        ///    <para>
        ///       Performs vector addition of two <see cref='System.Drawing.Size'/> objects.
        ///    </para>
        /// </summary>
        public static Size Add(Size sz1, Size sz2) =>
            new Size(unchecked(sz1.Width + sz2.Width), unchecked(sz1.Height + sz2.Height));

        /// <summary>
        ///   Converts a SizeF to a Size by performing a ceiling operation on
        ///   all the coordinates.
        /// </summary>
        public static Size Ceiling(SizeF value) =>
            new Size(unchecked((int)Math.Ceiling(value.Width)), unchecked((int)Math.Ceiling(value.Height)));

        /// <summary>
        ///    <para>
        ///       Contracts a <see cref='System.Drawing.Size'/> by another <see cref='System.Drawing.Size'/> .
        ///    </para>
        /// </summary>
        public static Size Subtract(Size sz1, Size sz2) =>
            new Size(unchecked(sz1.Width - sz2.Width), unchecked(sz1.Height - sz2.Height));

        /// <summary>
        ///   Converts a SizeF to a Size by performing a truncate operation on
        ///   all the coordinates.
        /// </summary>
        public static Size Truncate(SizeF value) => new Size(unchecked((int)value.Width), unchecked((int)value.Height));

        /// <summary>
        ///   Converts a SizeF to a Size by performing a round operation on
        ///   all the coordinates.
        /// </summary>
        public static Size Round(SizeF value) =>
            new Size(unchecked((int)Math.Round(value.Width)), unchecked((int)Math.Round(value.Height)));

        /// <summary>
        ///    <para>
        ///       Tests to see whether the specified object is a
        ///    <see cref='System.Drawing.Size'/> 
        ///    with the same dimensions as this <see cref='System.Drawing.Size'/>.
        /// </para>
        /// </summary>
        public override bool Equals(object obj) => obj is Size && Equals((Size)obj);

        public bool Equals(Size other) => this == other;

        /// <summary>
        ///    <para>
        ///       Returns a hash code.
        ///    </para>
        /// </summary>
        public override int GetHashCode() => HashHelpers.Combine(Width, Height);

        /// <summary>
        ///    <para>
        ///       Creates a human-readable string that represents this
        ///    <see cref='System.Drawing.Size'/>.
        ///    </para>
        /// </summary>
        public override string ToString() => "{Width=" + width.ToString() + ", Height=" + height.ToString() + "}";

        /// <summary>
        /// Helper method for operator * (multiply) that accepts type <see cref="Size"/> and <see cref="int"/>,
        /// returning a product of type <see cref="Size"/>.
        /// </summary>
        /// <param name="size">Multiplicand of type <see cref="Size"/>.</param>
        /// <param name="multiplier">Multiplier of type <see cref='int'>.</param>
        /// <returns>Product of type <see cref="Size"/>.</returns>
        private static Size Multiply(Size size, int multiplier) =>
            new Size(unchecked(size._width * multiplier), unchecked(size._height * multiplier));

        /// <summary>
        /// Helper method for operator * (multiply) that accepts type <see cref="Size"/> and <see cref="float"/>, 
        /// returning a product of type <see cref="SizeF"/>.
        /// </summary>
        /// <param name="size">Multiplicand of type <see cref="Size"/>.</param>
        /// <param name="multiplier">Multiplier of type <see cref="float"/>.</param>
        /// <returns>Product of type SizeF.</returns>
        private static SizeF Multiply(Size size, float multiplier) =>
            new SizeF(size._width * multiplier, size._height * multiplier);
    }
}
