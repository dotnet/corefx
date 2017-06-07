// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Copyright 2002 Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004-2006 Novell, Inc (http://www.novell.com)
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
//
// Authors:
//   Jordi Mas i Hernandez (jordi@ximian.com)
// 
// Partially based on work by:
//	
//   Everaldo Canuto (everaldo.canuto@bol.com.br)
//   Andreas Nahr (ClassDevelopment@A-SoftTech.com)
//   Dennis Hayes (dennish@raytek.com)
//   

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ColorMatrix
    {
        private float color00;
        private float color01;
        private float color02;
        private float color03;
        private float color04;
        private float color10;
        private float color11;
        private float color12;
        private float color13;
        private float color14;
        private float color20;
        private float color21;
        private float color22;
        private float color23;
        private float color24;
        private float color30;
        private float color31;
        private float color32;
        private float color33;
        private float color34;
        private float color40;
        private float color41;
        private float color42;
        private float color43;
        private float color44;

        // constructors
        public ColorMatrix()
        {
            color01 = color02 = color03 = color04 = 0;
            color10 = color12 = color13 = color14 = 0;
            color20 = color21 = color23 = color24 = 0;
            color30 = color31 = color32 = color34 = 0;
            color40 = color41 = color42 = color43 = 0;
            color00 = color11 = color22 = color33 = color44 = 1;
        }

        [CLSCompliant(false)]
        public ColorMatrix(float[][] newColorMatrix)
        {
            color00 = newColorMatrix[0][0];
            color01 = newColorMatrix[0][1];
            color02 = newColorMatrix[0][2];
            color03 = newColorMatrix[0][3];
            color04 = newColorMatrix[0][4];
            color10 = newColorMatrix[1][0];
            color11 = newColorMatrix[1][1];
            color12 = newColorMatrix[1][2];
            color13 = newColorMatrix[1][3];
            color14 = newColorMatrix[1][4];
            color20 = newColorMatrix[2][0];
            color21 = newColorMatrix[2][1];
            color22 = newColorMatrix[2][2];
            color23 = newColorMatrix[2][3];
            color24 = newColorMatrix[2][4];
            color30 = newColorMatrix[3][0];
            color31 = newColorMatrix[3][1];
            color32 = newColorMatrix[3][2];
            color33 = newColorMatrix[3][3];
            color34 = newColorMatrix[3][4];
            color40 = newColorMatrix[4][0];
            color41 = newColorMatrix[4][1];
            color42 = newColorMatrix[4][2];
            color43 = newColorMatrix[4][3];
            color44 = newColorMatrix[4][4];
        }

        // properties
        public float this[int row, int column]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        {
                            switch (column)
                            {
                                case 0:
                                    return color00;
                                case 1:
                                    return color01;
                                case 2:
                                    return color02;
                                case 3:
                                    return color03;
                                case 4:
                                    return color04;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 1:
                        {
                            switch (column)
                            {
                                case 0:
                                    return color10;
                                case 1:
                                    return color11;
                                case 2:
                                    return color12;
                                case 3:
                                    return color13;
                                case 4:
                                    return color14;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 2:
                        {
                            switch (column)
                            {
                                case 0:
                                    return color20;
                                case 1:
                                    return color21;
                                case 2:
                                    return color22;
                                case 3:
                                    return color23;
                                case 4:
                                    return color24;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 3:
                        {
                            switch (column)
                            {
                                case 0:
                                    return color30;
                                case 1:
                                    return color31;
                                case 2:
                                    return color32;
                                case 3:
                                    return color33;
                                case 4:
                                    return color34;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 4:
                        {
                            switch (column)
                            {
                                case 0:
                                    return color40;
                                case 1:
                                    return color41;
                                case 2:
                                    return color42;
                                case 3:
                                    return color43;
                                case 4:
                                    return color44;
                                default:
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }

                throw new IndexOutOfRangeException("Index was outside the bounds of the array");
            }

            set
            {
                switch (row)
                {
                    case 0:
                        {
                            switch (column)
                            {
                                case 0:
                                    color00 = value;
                                    return;
                                case 1:
                                    color01 = value;
                                    return;
                                case 2:
                                    color02 = value;
                                    return;
                                case 3:
                                    color03 = value;
                                    return;
                                case 4:
                                    color04 = value;
                                    return;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 1:
                        {
                            switch (column)
                            {
                                case 0:
                                    color10 = value;
                                    return;
                                case 1:
                                    color11 = value;
                                    return;
                                case 2:
                                    color12 = value;
                                    return;
                                case 3:
                                    color13 = value;
                                    return;
                                case 4:
                                    color14 = value;
                                    return;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 2:
                        {
                            switch (column)
                            {
                                case 0:
                                    color20 = value;
                                    return;
                                case 1:
                                    color21 = value;
                                    return;
                                case 2:
                                    color22 = value;
                                    return;
                                case 3:
                                    color23 = value;
                                    return;
                                case 4:
                                    color24 = value;
                                    return;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 3:
                        {
                            switch (column)
                            {
                                case 0:
                                    color30 = value;
                                    return;
                                case 1:
                                    color31 = value;
                                    return;
                                case 2:
                                    color32 = value;
                                    return;
                                case 3:
                                    color33 = value;
                                    return;
                                case 4:
                                    color34 = value;
                                    return;
                                default:
                                    break;
                            }
                            break;
                        }
                    case 4:
                        {
                            switch (column)
                            {
                                case 0:
                                    color40 = value;
                                    return;
                                case 1:
                                    color41 = value;
                                    return;
                                case 2:
                                    color42 = value;
                                    return;
                                case 3:
                                    color43 = value;
                                    return;
                                case 4:
                                    color44 = value;
                                    return;
                                default:
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }

                throw new IndexOutOfRangeException("Index was outside the bounds of the array");
            }
        }


        public float Matrix00
        {
            get { return color00; }
            set { color00 = value; }
        }

        public float Matrix01
        {
            get { return color01; }
            set { color01 = value; }
        }

        public float Matrix02
        {
            get { return color02; }
            set { color02 = value; }
        }

        public float Matrix03
        {
            get { return color03; }
            set { color03 = value; }
        }

        public float Matrix04
        {
            get { return color04; }
            set { color04 = value; }
        }

        public float Matrix10
        {
            get { return color10; }
            set { color10 = value; }
        }

        public float Matrix11
        {
            get { return color11; }
            set { color11 = value; }
        }

        public float Matrix12
        {
            get { return color12; }
            set { color12 = value; }
        }

        public float Matrix13
        {
            get { return color13; }
            set { color13 = value; }
        }

        public float Matrix14
        {
            get { return color14; }
            set { color14 = value; }
        }

        public float Matrix20
        {
            get { return color20; }
            set { color20 = value; }
        }

        public float Matrix21
        {
            get { return color21; }
            set { color21 = value; }
        }

        public float Matrix22
        {
            get { return color22; }
            set { color22 = value; }
        }

        public float Matrix23
        {
            get { return color23; }
            set { color23 = value; }
        }

        public float Matrix24
        {
            get { return color24; }
            set { color24 = value; }
        }

        public float Matrix30
        {
            get { return color30; }
            set { color30 = value; }
        }

        public float Matrix31
        {
            get { return color31; }
            set { color31 = value; }
        }

        public float Matrix32
        {
            get { return color32; }
            set { color32 = value; }
        }

        public float Matrix33
        {
            get { return color33; }
            set { color33 = value; }
        }

        public float Matrix34
        {
            get { return color34; }
            set { color34 = value; }
        }

        public float Matrix40
        {
            get { return color40; }
            set { color40 = value; }
        }

        public float Matrix41
        {
            get { return color41; }
            set { color41 = value; }
        }

        public float Matrix42
        {
            get { return color42; }
            set { color42 = value; }
        }

        public float Matrix43
        {
            get { return color43; }
            set { color43 = value; }
        }

        public float Matrix44
        {
            get { return color44; }
            set { color44 = value; }
        }


        static internal IntPtr Alloc(ColorMatrix cm)
        {
            if (cm == null)
                return IntPtr.Zero;

            IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ColorMatrix)));
            Marshal.StructureToPtr(cm, result, false);
            return result;
        }

        static internal void Free(IntPtr cm)
        {
            if (cm != IntPtr.Zero)
                Marshal.FreeHGlobal(cm);
        }
    }
}
