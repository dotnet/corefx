// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Internal
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal class GPPOINTF
    {
        internal float X;
        internal float Y;

        internal GPPOINTF()
        {
        }

        internal GPPOINTF(PointF pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        internal GPPOINTF(Point pt)
        {
            X = (float)pt.X;
            Y = (float)pt.Y;
        }

        internal PointF ToPoint()
        {
            return new PointF(X, Y);
        }
    }
}
