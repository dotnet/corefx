// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Text;

namespace System.Reflection.Metadata
{
    public struct MethodBody
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly int _rowId;

        internal MethodBody(MetadataReader reader, MethodBodyHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            _reader = reader;
            _rowId = handle.RowId;
        }

        private MethodBodyHandle Handle
        {
            get { return MethodBodyHandle.FromRowId(_rowId); }
        }

        /// <summary>
        /// Returns Sequence Points Blob.
        /// </summary>
        public BlobHandle SequencePoints
        {
            get
            {
                return _reader.MethodBodyTable.GetSequencePoints(Handle);
            }
        }
    }
}
