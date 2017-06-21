// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.Imaging.Encoder.cs
//
// (C) 2004 Novell, Inc.  http://www.novell.com
// Author: Ravindra (rkumar@novell.com)
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

namespace System.Drawing.Imaging
{
    public sealed class Encoder
    {
        private Guid guid;

        public static readonly Encoder ChrominanceTable;
        public static readonly Encoder ColorDepth;
        public static readonly Encoder Compression;
        public static readonly Encoder LuminanceTable;
        public static readonly Encoder Quality;
        public static readonly Encoder RenderMethod;
        public static readonly Encoder SaveFlag;
        public static readonly Encoder ScanMethod;
        public static readonly Encoder Transformation;
        public static readonly Encoder Version;

        static Encoder()
        {
            // GUID values are taken from my windows machine.
            ChrominanceTable = new Encoder("f2e455dc-09b3-4316-8260-676ada32481c");
            ColorDepth = new Encoder("66087055-ad66-4c7c-9a18-38a2310b8337");
            Compression = new Encoder("e09d739d-ccd4-44ee-8eba-3fbf8be4fc58");
            LuminanceTable = new Encoder("edb33bce-0266-4a77-b904-27216099e717");
            Quality = new Encoder("1d5be4b5-fa4a-452d-9cdd-5db35105e7eb");
            RenderMethod = new Encoder("6d42c53a-229a-4825-8bb7-5c99e2b9a8b8");
            SaveFlag = new Encoder("292266fc-ac40-47bf-8cfc-a85b89a655de");
            ScanMethod = new Encoder("3a4e2661-3109-4e56-8536-42c156e7dcfa");
            Transformation = new Encoder("8d0eb2d1-a58e-4ea8-aa14-108074b7b6f9");
            Version = new Encoder("24d18c76-814a-41a4-bf53-1c219cccf797");
        }

        internal Encoder(String guid)
        {
            this.guid = new Guid(guid);
        }

        public Encoder(Guid guid)
        {
            this.guid = guid;
        }

        public Guid Guid
        {
            get
            {
                return guid;
            }
        }
    }
}
