// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.EncoderParameters.cs
//
// Author: 
//	Ravindra (rkumar@novell.com)
//  Vladimir Vukicevic (vladimir@pobox.com)
//
// Copyright (C) 2004, 2008 Novell, Inc (http://www.novell.com)
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
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
    public sealed class EncoderParameters : IDisposable
    {
        private EncoderParameter[] parameters;

        public EncoderParameters()
        {
            parameters = new EncoderParameter[1];
        }

        public EncoderParameters(int count)
        {
            parameters = new EncoderParameter[count];
        }

        public EncoderParameter[] Param
        {
            get
            {
                return parameters;
            }

            set
            {
                parameters = value;
            }
        }

        public void Dispose()
        {
            // Nothing
            GC.SuppressFinalize(this);
        }
        internal IntPtr ToNativePtr()
        {
            IntPtr result;
            IntPtr ptr;

            // 4 is the initial int32 "count" value
            result = Marshal.AllocHGlobal(4 + parameters.Length * EncoderParameter.NativeSize());

            ptr = result;
            Marshal.WriteInt32(ptr, parameters.Length);

            ptr = (IntPtr)(ptr.ToInt64() + 4);
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i].ToNativePtr(ptr);
                ptr = (IntPtr)(ptr.ToInt64() + EncoderParameter.NativeSize());
            }

            return result;
        }

        /* The IntPtr passed in here is a blob returned from
		 * GdipImageGetEncoderParameterList.  Its internal pointers
		 * (i.e. the Value pointers in the EncoderParameter entries)
		 * point to areas within this block of memeory; this means
		 * that we need to free it as a whole, and also means that
		 * we can't Marshal.PtrToStruct our way to victory.
		 */
        internal static EncoderParameters FromNativePtr(IntPtr epPtr)
        {
            if (epPtr == IntPtr.Zero)
                return null;

            IntPtr ptr = epPtr;

            int count = Marshal.ReadInt32(ptr);
            ptr = (IntPtr)(ptr.ToInt64() + 4);

            if (count == 0)
                return null;

            EncoderParameters result = new EncoderParameters(count);

            for (int i = 0; i < count; i++)
            {
                result.parameters[i] = EncoderParameter.FromNativePtr(ptr);
                ptr = (IntPtr)(ptr.ToInt64() + EncoderParameter.NativeSize());
            }

            return result;
        }
    }
}
