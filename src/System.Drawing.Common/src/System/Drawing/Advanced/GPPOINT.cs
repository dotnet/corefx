// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Internal
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class GPPOINT
    {
        internal int X;
        internal int Y;

        internal GPPOINT()
        {
        }

        internal GPPOINT(PointF pt)
        {
            X = (int)pt.X;
            Y = (int)pt.Y;
        }

        internal GPPOINT(Point pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        internal PointF ToPoint()
        {
            return new PointF(X, Y);
        }
    }
}
