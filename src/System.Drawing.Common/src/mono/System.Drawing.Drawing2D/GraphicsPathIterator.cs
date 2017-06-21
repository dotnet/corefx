// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Drawing2D.GraphicsPathIterator.cs
//
// Author:
//   Dennis Hayes (dennish@Raytek.com)
//   Duncan Mak (duncan@ximian.com)
//   Ravindra (rkumar@novell.com)
//
// Copyright (C) 2002/3 Ximian, Inc (http://www.ximian.com)
//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Drawing;

namespace System.Drawing.Drawing2D
{
    public sealed class GraphicsPathIterator : MarshalByRefObject, IDisposable
    {
        private IntPtr nativeObject = IntPtr.Zero;

        // Constructors
        internal GraphicsPathIterator(IntPtr native)
        {
            this.nativeObject = native;
        }

        public GraphicsPathIterator(GraphicsPath path)
        {
            if (path != null)
            {
                Status status = GDIPlus.GdipCreatePathIter(out nativeObject, path.NativeObject);
                GDIPlus.CheckStatus(status);
            }
        }

        internal IntPtr NativeObject
        {
            get
            {
                return nativeObject;
            }
            set
            {
                nativeObject = value;
            }
        }

        // Public Properites

        public int Count
        {
            get
            {
                if (nativeObject == IntPtr.Zero)
                    return 0;

                int count;
                Status status = GDIPlus.GdipPathIterGetCount(nativeObject, out count);
                GDIPlus.CheckStatus(status);

                return count;
            }
        }

        public int SubpathCount
        {
            get
            {
                int count;
                Status status = GDIPlus.GdipPathIterGetSubpathCount(nativeObject, out count);
                GDIPlus.CheckStatus(status);

                return count;
            }
        }

        internal void Dispose(bool disposing)
        {
            Status status;
            if (nativeObject != IntPtr.Zero)
            {
                status = GDIPlus.GdipDeletePathIter(nativeObject);
                GDIPlus.CheckStatus(status);

                nativeObject = IntPtr.Zero;
            }
        }

        // Public Methods.

        public int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
        {
            Status status;
            int resultCount;

            // no null checks, MS throws a NullReferenceException here
            if (points.Length != types.Length)
                throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");

            status = GDIPlus.GdipPathIterCopyData(nativeObject, out resultCount, points, types, startIndex, endIndex);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        ~GraphicsPathIterator()
        {
            Dispose(false);
        }

        public int Enumerate(ref PointF[] points, ref byte[] types)
        {
            Status status;
            int resultCount;
            // no null checks, MS throws a NullReferenceException here
            int count = points.Length;
            if (count != types.Length)
                throw new ArgumentException("Invalid arguments passed. Both arrays should have the same length.");

            status = GDIPlus.GdipPathIterEnumerate(nativeObject, out resultCount, points, types, count);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public bool HasCurve()
        {
            bool curve;
            Status status = GDIPlus.GdipPathIterHasCurve(nativeObject, out curve);
            GDIPlus.CheckStatus(status);

            return curve;
        }

        public int NextMarker(GraphicsPath path)
        {
            int resultCount;
            IntPtr ptr = (path == null) ? IntPtr.Zero : path.NativeObject;
            Status status = GDIPlus.GdipPathIterNextMarkerPath(nativeObject, out resultCount, ptr);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public int NextMarker(out int startIndex, out int endIndex)
        {
            Status status;
            int resultCount;
            status = GDIPlus.GdipPathIterNextMarker(nativeObject, out resultCount, out startIndex, out endIndex);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
        {
            Status status;
            int resultCount;
            status = GDIPlus.GdipPathIterNextPathType(nativeObject, out resultCount, out pathType, out startIndex, out endIndex);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public int NextSubpath(GraphicsPath path, out bool isClosed)
        {
            int resultCount;
            IntPtr ptr = (path == null) ? IntPtr.Zero : path.NativeObject;
            Status status = GDIPlus.GdipPathIterNextSubpathPath(nativeObject, out resultCount, ptr, out isClosed);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
        {
            Status status;
            int resultCount;
            status = GDIPlus.GdipPathIterNextSubpath(nativeObject, out resultCount, out startIndex, out endIndex, out isClosed);
            GDIPlus.CheckStatus(status);

            return resultCount;
        }

        public void Rewind()
        {
            Status status = GDIPlus.GdipPathIterRewind(nativeObject);
            GDIPlus.CheckStatus(status);
        }
    }
}
