// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct LocalScope
    {
        private readonly MetadataReader reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint rowId;

        internal LocalScope(MetadataReader reader, LocalScopeHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            this.reader = reader;
            this.rowId = handle.RowId;
        }

        private LocalScopeHandle Handle
        {
            get { return LocalScopeHandle.FromRowId(rowId); }
        }

        public MethodDefinitionHandle Method
        {
            get
            {
                return reader.LocalScopeTable.GetMethod(Handle);
            }
        }

        public ImportScopeHandle ImportScope
        {
            get
            {
                return reader.LocalScopeTable.GetImportScope(Handle);
            }
        }

        public int StartOffset
        {
            get
            {
                return reader.LocalScopeTable.GetStartOffset(Handle);
            }
        }

        public int Length
        {
            get
            {
                return reader.LocalScopeTable.GetLength(Handle);
            }
        }

        public LocalVariableHandleCollection GetLocalVariables()
        {
            return new LocalVariableHandleCollection(reader, Handle);
        }

        public LocalConstantHandleCollection GetLocalConstants()
        {
            return new LocalConstantHandleCollection(reader, Handle);
        }
    }
}