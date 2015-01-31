// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct LocalVariable
    {
        private readonly MetadataReader reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint rowId;

        internal LocalVariable(MetadataReader reader, LocalVariableHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            this.reader = reader;
            this.rowId = handle.RowId;
        }

        private LocalVariableHandle Handle
        {
            get { return LocalVariableHandle.FromRowId(rowId); }
        }

        public LocalVariableAttributes Attributes
        {
            get
            {
                return reader.LocalVariableTable.GetAttributes(Handle);
            }
        }

        public int Index
        {
            get
            {
                return reader.LocalVariableTable.GetIndex(Handle);
            }
        }

        public StringHandle Name
        {
            get
            {
                return reader.LocalVariableTable.GetName(Handle);
            }
        }
    }
}