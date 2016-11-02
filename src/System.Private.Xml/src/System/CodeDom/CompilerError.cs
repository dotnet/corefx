// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace System.CodeDom.Compiler
{
    [Serializable]
    internal class CompilerError
    {
        public CompilerError() : this(string.Empty, 0, 0, string.Empty, string.Empty) { }

        public CompilerError(string fileName, int line, int column, string errorNumber, string errorText)
        {
            Line = line;
            Column = column;
            ErrorNumber = errorNumber;
            ErrorText = errorText;
            FileName = fileName;
        }

        public int Line { get; set; }

        public int Column { get; set; }

        public string ErrorNumber { get; set; }

        public string ErrorText { get; set; }

        public bool IsWarning { get; set; }

        public string FileName { get; set; }

        public override string ToString() => FileName.Length > 0 ?
            string.Format(CultureInfo.InvariantCulture, "{0}({1},{2}) : {3} {4}: {5}", FileName, Line, Column, WarningString, ErrorNumber, ErrorText) :
            string.Format(CultureInfo.InvariantCulture, "{0} {1}: {2}", WarningString, ErrorNumber, ErrorText);

        private string WarningString => IsWarning ? "warning" : "error";
    }

    [Serializable]
    internal class CompilerErrorCollection : CollectionBase
    {
        public CompilerErrorCollection() { }

        public CompilerErrorCollection(CompilerErrorCollection value)
        {
            AddRange(value);
        }

        public CompilerErrorCollection(CompilerError[] value)
        {
            AddRange(value);
        }

        public CompilerError this[int index]
        {
            get { return (CompilerError)List[index]; }
            set { List[index] = value; }
        }

        public int Add(CompilerError value) => List.Add(value);

        public void AddRange(CompilerError[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            for (int i = 0; i < value.Length; i++)
            {
                this.Add(value[i]);
            }
        }

        public void AddRange(CompilerErrorCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int currentCount = value.Count;
            for (int i = 0; i < currentCount; i++)
            {
                Add(value[i]);
            }
        }

        public bool Contains(CompilerError value) => List.Contains(value);

        public void CopyTo(CompilerError[] array, int index) => List.CopyTo(array, index);

        public bool HasErrors
        {
            get
            {
                if (Count > 0)
                {
                    foreach (CompilerError e in this)
                    {
                        if (!e.IsWarning)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool HasWarnings
        {
            get
            {
                if (Count > 0)
                {
                    foreach (CompilerError e in this)
                    {
                        if (e.IsWarning)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public int IndexOf(CompilerError value) => List.IndexOf(value);

        public void Insert(int index, CompilerError value) => List.Insert(index, value);

        public void Remove(CompilerError value) => List.Remove(value);
    }
}