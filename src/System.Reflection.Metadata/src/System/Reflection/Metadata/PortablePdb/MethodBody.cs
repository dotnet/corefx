// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Text;

namespace System.Reflection.Metadata
{
    public struct MethodBody
    {
        private readonly MetadataReader reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint rowId;

        internal MethodBody(MetadataReader reader, MethodBodyHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            this.reader = reader;
            this.rowId = handle.RowId;
        }

        private MethodBodyHandle Handle
        {
            get { return MethodBodyHandle.FromRowId(rowId); }
        }

        /// <summary>
        /// Returns Sequence Points Blob.
        /// </summary>
        public BlobHandle SequencePoints
        {
            get
            {
                return reader.MethodBodyTable.GetSequencePoints(Handle);
            }
        }
    }
}
