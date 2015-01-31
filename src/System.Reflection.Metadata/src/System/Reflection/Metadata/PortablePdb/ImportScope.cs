// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct ImportScope
    {
        private readonly MetadataReader reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint rowId;

        internal ImportScope(MetadataReader reader, ImportScopeHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            this.reader = reader;
            this.rowId = handle.RowId;
        }

        private ImportScopeHandle Handle
        {
            get { return ImportScopeHandle.FromRowId(rowId); }
        }

        public ImportScopeHandle Parent
        {
            get
            {
                return reader.ImportScopeTable.GetParent(Handle);
            }
        }

        public BlobHandle Imports
        {
            get
            {
                return reader.ImportScopeTable.GetImports(Handle);
            }
        }
    }
}