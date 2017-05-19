// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Numerics.Hashing;

namespace System.Drawing
{
    /// <summary>
    ///    Represents an ordered pair of x and y coordinates that
    ///    define a point in a two-dimensional plane.
    /// </summary>
    [Serializable]
    public struct PointF : IEquatable<PointF>
    {
        /// <summary>
        ///    <para>
        ///       Creates a new instance of the <see cref='System.Drawing.PointF'/> class
        ///       with member data left uninitialized.
        ///    </para>
        /// </summary>
        public static readonly PointF Empty = new PointF();
        private float _x;
        private float _y;

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Drawing.PointF'/> class
        ///       with the specified coordinates.
        ///    </para>
        /// </summary>
        public PointF(float x, float y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        ///    <para>
        ///       Gets a value indicating whether this <see cref='System.Drawing.PointF'/> is empty.
        ///    </para>
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => _x == 0f && _y == 0f;

        /// <summary>
        ///    <para>
        ///       Gets the x-coordinate of this <see cref='System.Drawing.PointF'/>.
        ///    </para>
        /// </summary>
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        ///    <para>
        ///       Gets the y-coordinate of this <see cref='System.Drawing.PointF'/>.
        ///    </para>
        /// </summary>
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.Size'/> .
        ///    </para>
        /// </summary>
        public static PointF operator +(PointF pt, Size sz) => Add(pt, sz);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.Size'/> .
        ///    </para>
        /// </summary>
        public static PointF operator -(PointF pt, Size sz) => Subtract(pt, sz);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.SizeF'/> .
        ///    </para>
        /// </summary>
        public static PointF operator +(PointF pt, SizeF sz) => Add(pt, sz);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
        ///    </para>
        /// </summary>
        public static PointF operator -(PointF pt, SizeF sz) => Subtract(pt, sz);

        /// <summary>
        ///    <para>
        ///       Compares two <see cref='System.Drawing.PointF'/> objects. The result specifies
        ///       whether the values of the <see cref='System.Drawing.PointF.X'/> and <see cref='System.Drawing.PointF.Y'/> properties of the two <see cref='System.Drawing.PointF'/>
        ///       objects are equal.
        ///    </para>
        /// </summary>
        public static bool operator ==(PointF left, PointF right) => left.X == right.X && left.Y == right.Y;

        /// <summary>
        ///    <para>
        ///       Compares two <see cref='System.Drawing.PointF'/> objects. The result specifies whether the values
        ///       of the <see cref='System.Drawing.PointF.X'/> or <see cref='System.Drawing.PointF.Y'/> properties of the two
        ///    <see cref='System.Drawing.PointF'/> 
        ///    objects are unequal.
        /// </para>
        /// </summary>
        public static bool operator !=(PointF left, PointF right) => !(left == right);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.Size'/> .
        ///    </para>
        /// </summary>
        public static PointF Add(PointF pt, Size sz) => new PointF(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.Size'/> .
        ///    </para>
        /// </summary>
        public static PointF Subtract(PointF pt, Size sz) => new PointF(pt.X - sz.Width, pt.Y - sz.Height);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.SizeF'/> .
        ///    </para>
        /// </summary>
        public static PointF Add(PointF pt, SizeF sz) => new PointF(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        ///    <para>
        ///       Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
        ///    </para>
        /// </summary>
        public static PointF Subtract(PointF pt, SizeF sz) => new PointF(pt.X - sz.Width, pt.Y - sz.Height);

        public override bool Equals(object obj) => obj is PointF && Equals((PointF)obj);

        public bool Equals(PointF other) => this == other;

        public override int GetHashCode() => HashHelpers.Combine(X.GetHashCode(), Y.GetHashCode());

        public override string ToString() => "{X=" + _x.ToString() + ", Y=" + _y.ToString() + "}";
    }
}
