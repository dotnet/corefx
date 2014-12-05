// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata
{
    public struct DeclarativeSecurityAttribute
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly uint _rowId;

        internal DeclarativeSecurityAttribute(MetadataReader reader, uint rowId)
        {
            Debug.Assert(reader != null);
            Debug.Assert(rowId != 0);

            _reader = reader;
            _rowId = rowId;
        }

        private uint RowId
        {
            get { return _rowId & TokenTypeIds.RIDMask; }
        }

        private DeclarativeSecurityAttributeHandle Handle
        {
            get { return DeclarativeSecurityAttributeHandle.FromRowId(RowId); }
        }

        private MethodDefTreatment Treatment
        {
            get { return (MethodDefTreatment)(_rowId >> TokenTypeIds.RowIdBitCount); }
        }

        public DeclarativeSecurityAction Action
        {
            get
            {
                return _reader.DeclSecurityTable.GetAction(_rowId);
            }
        }

        public Handle Parent
        {
            get
            {
                return _reader.DeclSecurityTable.GetParent(_rowId);
            }
        }

        public BlobHandle PermissionSet
        {
            get
            {
                return _reader.DeclSecurityTable.GetPermissionSet(_rowId);
            }
        }
    }
}