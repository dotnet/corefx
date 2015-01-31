// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct LocalConstant
    {
        private readonly MetadataReader reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint rowId;

        internal LocalConstant(MetadataReader reader, LocalConstantHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            this.reader = reader;
            this.rowId = handle.RowId;
        }

        private LocalConstantHandle Handle
        {
            get { return LocalConstantHandle.FromRowId(rowId); }
        }

        public StringHandle Name
        {
            get
            {
                return reader.LocalConstantTable.GetName(Handle);
            }
        }

        /// <summary>
        /// The type of the constant value.
        /// </summary>
        public ConstantTypeCode TypeCode
        {
            get
            {
                return reader.LocalConstantTable.GetTypeCode(Handle);
            }
        }

        /// <summary>
        /// The constant value.
        /// </summary>
        public BlobHandle Value
        {
            get
            {
                return reader.LocalConstantTable.GetValue(Handle);
            }
        }
    }
}
