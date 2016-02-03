// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct LocalVariable
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly int _rowId;

        internal LocalVariable(MetadataReader reader, LocalVariableHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            _reader = reader;
            _rowId = handle.RowId;
        }

        private LocalVariableHandle Handle
        {
            get { return LocalVariableHandle.FromRowId(_rowId); }
        }

        public LocalVariableAttributes Attributes
        {
            get
            {
                return _reader.LocalVariableTable.GetAttributes(Handle);
            }
        }

        public int Index
        {
            get
            {
                return _reader.LocalVariableTable.GetIndex(Handle);
            }
        }

        public StringHandle Name
        {
            get
            {
                return _reader.LocalVariableTable.GetName(Handle);
            }
        }
    }
}
