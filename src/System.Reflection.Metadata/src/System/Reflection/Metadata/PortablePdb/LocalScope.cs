// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct LocalScope
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint _rowId;

        internal LocalScope(MetadataReader reader, LocalScopeHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            _reader = reader;
            _rowId = handle.RowId;
        }

        private LocalScopeHandle Handle
        {
            get { return LocalScopeHandle.FromRowId(_rowId); }
        }

        public MethodDefinitionHandle Method
        {
            get
            {
                return _reader.LocalScopeTable.GetMethod(Handle);
            }
        }

        public ImportScopeHandle ImportScope
        {
            get
            {
                return _reader.LocalScopeTable.GetImportScope(Handle);
            }
        }

        public int StartOffset
        {
            get
            {
                return _reader.LocalScopeTable.GetStartOffset((int)_rowId);
            }
        }

        public int Length
        {
            get
            {
                return _reader.LocalScopeTable.GetLength((int)_rowId);
            }
        }

        public int EndOffset
        {
            get
            {
                return _reader.LocalScopeTable.GetEndOffset((int)_rowId);
            }
        }

        public LocalVariableHandleCollection GetLocalVariables()
        {
            return new LocalVariableHandleCollection(_reader, Handle);
        }

        public LocalConstantHandleCollection GetLocalConstants()
        {
            return new LocalConstantHandleCollection(_reader, Handle);
        }

        public LocalScopeHandleCollection.ChildrenEnumerator GetChildren()
        {
            return new LocalScopeHandleCollection.ChildrenEnumerator(_reader, (int)_rowId);
        }
    }
}