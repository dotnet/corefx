// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct ImportScope
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly int _rowId;

        internal ImportScope(MetadataReader reader, ImportScopeHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            _reader = reader;
            _rowId = handle.RowId;
        }

        private ImportScopeHandle Handle
        {
            get { return ImportScopeHandle.FromRowId(_rowId); }
        }

        public ImportScopeHandle Parent
        {
            get
            {
                return _reader.ImportScopeTable.GetParent(Handle);
            }
        }

        public BlobHandle ImportsBlob
        {
            get
            {
                return _reader.ImportScopeTable.GetImports(Handle);
            }
        }

        public ImportDefinitionCollection GetImports()
        {
            return new ImportDefinitionCollection(_reader.BlobStream.GetMemoryBlock(ImportsBlob));
        }
    }
}
