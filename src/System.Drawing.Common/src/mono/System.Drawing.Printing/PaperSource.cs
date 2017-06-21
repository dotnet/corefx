// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// System.Drawing.PaperSource.cs
//
// Author:
//   Dennis Hayes (dennish@Raytek.com)
//   Herve Poussineau (hpoussineau@fr.st)
//
// (C) 2002 Ximian, Inc
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

namespace System.Drawing.Printing
{
    /// <summary>
    /// Summary description for PaperSource.
    /// </summary>
    [Serializable]
    public class PaperSource
    {
        private PaperSourceKind kind;
        private string source_name;
        internal bool is_default;

        public PaperSource()
        {

        }

        internal PaperSource(string sourceName, PaperSourceKind kind)
        {
            this.source_name = sourceName;
            this.kind = kind;
        }

        internal PaperSource(string sourceName, PaperSourceKind kind, bool isDefault)
        {
            this.source_name = sourceName;
            this.kind = kind;
            this.is_default = IsDefault;
        }

        public PaperSourceKind Kind
        {
            get
            {
                // Exactly at 256 (as opposed to Custom, which is 257 and the max value of PaperSourceKind),
                // we must return Custom always.
                if ((int)kind >= 256)
                    return PaperSourceKind.Custom;

                return this.kind;
            }
        }
        public string SourceName
        {
            get
            {
                return this.source_name;
            }
            set
            {
                this.source_name = value;
            }
        }

        public int RawKind
        {
            get
            {
                return (int)kind;
            }
            set
            {
                kind = (PaperSourceKind)value;
            }
        }

        internal bool IsDefault
        {
            get { return is_default; }
            set { is_default = value; }
        }

        public override string ToString()
        {
            string ret = "[PaperSource {0} Kind={1}]";
            return String.Format(ret, this.SourceName, this.Kind);
        }

    }
}
