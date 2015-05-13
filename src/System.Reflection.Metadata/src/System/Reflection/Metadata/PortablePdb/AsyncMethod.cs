// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct AsyncMethod
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly int _rowId;

        internal AsyncMethod(MetadataReader reader, AsyncMethodHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            _reader = reader;
            _rowId = handle.RowId;
        }

        private AsyncMethodHandle Handle
        {
            get { return AsyncMethodHandle.FromRowId(_rowId); }
        }

        public MethodDefinitionHandle KickoffMethod
        {
            get
            {
                return _reader.AsyncMethodTable.GetKickoffMethod(Handle);
            }
        }

        public int CatchHandlerOffset
        {
            get
            {
                return _reader.AsyncMethodTable.GetCatchHandlerOffset(Handle);
            }
        }

        public BlobHandle Awaits
        {
            get
            {
                return _reader.AsyncMethodTable.GetAwaits(Handle);
            }
        }
    }
}