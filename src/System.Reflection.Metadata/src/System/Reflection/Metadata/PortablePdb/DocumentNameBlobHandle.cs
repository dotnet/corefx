﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection.Internal;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata
{
    /// <summary>
    /// <see cref="BlobHandle"/> representing a blob on #Blob heap in Portable PDB 
    /// structured as Document Name. 
    /// </summary>
    /// <remarks>
    /// The kind of the handle is <see cref="HandleKind.Blob"/>. 
    /// The handle is a specialization of <see cref="BlobHandle"/> and doesn't have a distinct kind. 
    /// </remarks>
    public struct DocumentNameBlobHandle : IEquatable<DocumentNameBlobHandle>
    {
        // bits:
        // 29..31: 0
        //  0..28: #Blob heap offset
        private readonly int _heapOffset;

        private DocumentNameBlobHandle(int heapOffset)
        {
            Debug.Assert(HeapHandleType.IsValidHeapOffset((uint)heapOffset));
            _heapOffset = heapOffset;
        }

        internal static DocumentNameBlobHandle FromOffset(int heapOffset)
        {
            return new DocumentNameBlobHandle(heapOffset);
        }

        public static implicit operator BlobHandle(DocumentNameBlobHandle handle)
        {
            return BlobHandle.FromOffset(handle._heapOffset);
        }

        public static explicit operator DocumentNameBlobHandle(BlobHandle handle)
        {
            if (handle.IsVirtual)
            {
                Throw.InvalidCast();
            }

            return FromOffset(handle.GetHeapOffset());
        }

        public bool IsNil
        {
            get { return _heapOffset == 0; }
        }

        public override bool Equals(object obj)
        {
            return obj is DocumentNameBlobHandle && Equals((DocumentNameBlobHandle)obj);
        }

        public bool Equals(DocumentNameBlobHandle other)
        {
            return _heapOffset == other._heapOffset;
        }

        public override int GetHashCode()
        {
            return _heapOffset;
        }

        public static bool operator ==(DocumentNameBlobHandle left, DocumentNameBlobHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DocumentNameBlobHandle left, DocumentNameBlobHandle right)
        {
            return !left.Equals(right);
        }
    }
}
