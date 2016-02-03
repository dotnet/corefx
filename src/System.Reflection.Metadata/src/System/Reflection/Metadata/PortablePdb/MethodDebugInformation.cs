// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Reflection.Metadata
{
    public struct MethodDebugInformation
    {
        private readonly MetadataReader _reader;

        // Workaround: JIT doesn't generate good code for nested structures, so use RowId.
        private readonly int _rowId;

        internal MethodDebugInformation(MetadataReader reader, MethodDebugInformationHandle handle)
        {
            Debug.Assert(reader != null);
            Debug.Assert(!handle.IsNil);

            _reader = reader;
            _rowId = handle.RowId;
        }

        private MethodDebugInformationHandle Handle
        {
            get { return MethodDebugInformationHandle.FromRowId(_rowId); }
        }

        /// <summary>
        /// Returns a blob encoding sequence points.
        /// Use <see cref="GetSequencePoints()"/> to decode.
        /// </summary>
        public BlobHandle SequencePointsBlob
        {
            get
            {
                return _reader.MethodDebugInformationTable.GetSequencePoints(Handle);
            }
        }

        /// <summary>
        /// The document containing the first sequence point of the method, 
        /// or nil if the method doesn't have sequence points.
        /// </summary>
        public DocumentHandle Document
        {
            get
            {
                return _reader.MethodDebugInformationTable.GetDocument(Handle);
            }
        }

        /// <summary>
        /// Returns local signature handle.
        /// </summary>
        public StandaloneSignatureHandle LocalSignature
        {
            get
            {
                if (SequencePointsBlob.IsNil)
                {
                    return default(StandaloneSignatureHandle);
                }

                return StandaloneSignatureHandle.FromRowId(_reader.GetBlobReader(SequencePointsBlob).ReadCompressedInteger());
            }
        }

        public SequencePointCollection GetSequencePoints()
        {
            return new SequencePointCollection(_reader.BlobStream.GetMemoryBlock(SequencePointsBlob), Document);
        }

        /// <summary>
        /// If the method is a MoveNext method of a state machine returns the kickoff method of the state machine, otherwise returns nil handle.
        /// </summary>
        public MethodDefinitionHandle GetStateMachineKickoffMethod()
        {
            return _reader.StateMachineMethodTable.FindKickoffMethod(_rowId);
        }
    }
}
