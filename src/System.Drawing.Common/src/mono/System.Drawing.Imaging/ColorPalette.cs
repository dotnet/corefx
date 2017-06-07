// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.ColorPalette.cs
//
// (C) 2002 Ximian, Inc.  http://www.ximian.com
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//

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
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
    public sealed class ColorPalette
    {
        // 0x1: the color values in the array contain alpha information
        // 0x2: the color values are grayscale values.
        // 0x4: the colors in the array are halftone values.

        private int flags;
        private Color[] entries;

        //
        // There is no public constructor, this will be used somewhere in the
        // drawing code
        //
        internal ColorPalette()
        {
            entries = new Color[0];
        }

        internal ColorPalette(int flags, Color[] colors)
        {
            this.flags = flags;
            entries = colors;
        }

        public Color[] Entries
        {
            get
            {
                return entries;
            }
        }

        public int Flags
        {
            get
            {
                return flags;
            }
        }
        /* Caller should call FreeHGlobal*/
        internal IntPtr getGDIPalette()
        {
            GdiColorPalette palette = new GdiColorPalette();
            Color[] entries = Entries;
            int entry = 0;
            int size = Marshal.SizeOf(palette) + (Marshal.SizeOf(entry) * entries.Length);
            IntPtr lfBuffer = Marshal.AllocHGlobal(size);

            palette.Flags = Flags;
            palette.Count = entries.Length;

            int[] values = new int[palette.Count];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = entries[i].ToArgb();
            }

            Marshal.StructureToPtr(palette, lfBuffer, false);
            Marshal.Copy(values, 0, (IntPtr)(lfBuffer.ToInt64() + Marshal.SizeOf(palette)), values.Length);

            return lfBuffer;
        }

        internal void setFromGDIPalette(IntPtr palette)
        {
            IntPtr ptr = palette;
            int cnt, color;
            int offset;

            flags = Marshal.ReadInt32(ptr);
            ptr = (IntPtr)(ptr.ToInt64() + 4);
            cnt = Marshal.ReadInt32(ptr);
            ptr = (IntPtr)(ptr.ToInt64() + 4);

            entries = new Color[cnt];

            offset = 0;
            for (int i = 0; i < cnt; i++)
            {
                color = Marshal.ReadInt32(ptr, offset);
                entries[i] = Color.FromArgb(color);
                offset += 4;
            }
        }
    }
}
