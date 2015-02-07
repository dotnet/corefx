// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection.Internal;

namespace System.Reflection.Metadata.Ecma335
{
    internal struct DocumentTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool _isGuidHeapRefSizeSmall;
        private readonly bool _isBlobHeapRefSizeSmall;

        private const int NameOffset = 0;
        private readonly int _hashAlgorithmOffset;
        private readonly int _hashOffset;
        private readonly int _languageOffset;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal DocumentTableReader(
            uint numberOfRows,
            int guidHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            _isGuidHeapRefSizeSmall = guidHeapRefSize == 2;
            _isBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            _hashAlgorithmOffset = NameOffset + blobHeapRefSize;
            _hashOffset = _hashAlgorithmOffset + guidHeapRefSize;
            _languageOffset = _hashOffset + blobHeapRefSize;
            this.RowSize = _languageOffset + guidHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetName(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + NameOffset, _isBlobHeapRefSizeSmall));
        }

        internal GuidHandle GetHashAlgorithm(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GuidHandle.FromIndex(this.Block.PeekReference(rowOffset + _hashAlgorithmOffset, _isGuidHeapRefSizeSmall));
        }

        internal BlobHandle GetHash(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + _hashOffset, _isBlobHeapRefSizeSmall));
        }

        internal GuidHandle GetLanguage(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GuidHandle.FromIndex(this.Block.PeekReference(rowOffset + _languageOffset, _isGuidHeapRefSizeSmall));
        }
    }

    internal struct MethodBodyTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool _isBlobHeapRefSizeSmall;

        private const int SequencePointsOffset = 0;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal MethodBodyTableReader(
            uint numberOfRows,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            _isBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.RowSize = SequencePointsOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetSequencePoints(MethodBodyHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + SequencePointsOffset, _isBlobHeapRefSizeSmall));
        }
    }

    internal struct LocalScopeTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool _isMethodRefSmall;
        private readonly bool _isImportScopeRefSmall;
        private readonly bool _isLocalConstantRefSmall;
        private readonly bool _isLocalVariableRefSmall;

        private const int MethodOffset = 0;
        private readonly int _importScopeOffset;
        private readonly int _variableListOffset;
        private readonly int _constantListOffset;
        private readonly int _startOffsetOffset;
        private readonly int _lengthOffset;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal LocalScopeTableReader(
            uint numberOfRows,
            int methodRefSize,
            int importScopeRefSize,
            int localVariableRefSize,
            int localConstantRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            _isMethodRefSmall = methodRefSize == 2;
            _isImportScopeRefSmall = importScopeRefSize == 2;
            _isLocalVariableRefSmall = localVariableRefSize == 2;
            _isLocalConstantRefSmall = localConstantRefSize == 2;

            _importScopeOffset = MethodOffset + methodRefSize;
            _variableListOffset = _importScopeOffset + importScopeRefSize;
            _constantListOffset = _variableListOffset + localVariableRefSize;
            _startOffsetOffset = _constantListOffset + localConstantRefSize;
            _lengthOffset = _startOffsetOffset + sizeof(uint);
            this.RowSize = _lengthOffset + sizeof(uint);

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal MethodDefinitionHandle GetMethod(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + MethodOffset, _isMethodRefSmall));
        }

        internal ImportScopeHandle GetImportScope(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return ImportScopeHandle.FromRowId(this.Block.PeekReference(rowOffset + _importScopeOffset, _isImportScopeRefSmall));
        }

        internal uint GetVariableStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + _variableListOffset, _isLocalVariableRefSmall);
        }

        internal uint GetConstantStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + _constantListOffset, _isLocalConstantRefSmall);
        }

        internal int GetStartOffset(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            // TODO: overflow
            return (int)this.Block.PeekUInt32(rowOffset + _startOffsetOffset);
        }

        internal int GetLength(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            // TODO: overflow
            return (int)this.Block.PeekUInt32(rowOffset + _lengthOffset);
        }
    }

    internal struct LocalVariableTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _isStringHeapRefSizeSmall;
        private readonly int _attributesOffset;
        private readonly int _indexOffset;
        private readonly int _nameOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal LocalVariableTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            _isStringHeapRefSizeSmall = stringHeapRefSize == 2;

            _attributesOffset = 0;
            _indexOffset = _attributesOffset + sizeof(ushort);
            _nameOffset = _indexOffset + sizeof(ushort);
            this.RowSize = _nameOffset + stringHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal LocalVariableAttributes GetAttributes(LocalVariableHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (LocalVariableAttributes)this.Block.PeekUInt16(rowOffset + _attributesOffset);
        }

        internal ushort GetIndex(LocalVariableHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekUInt16(rowOffset + _indexOffset);
        }

        internal StringHandle GetName(LocalVariableHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + _nameOffset, _isStringHeapRefSizeSmall));
        }
    }

    internal struct LocalConstantTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _isStringHeapRefSizeSmall;
        private readonly bool _isBlobHeapRefSizeSmall;

        private const int NameOffset = 0;
        private readonly int _valueOffset;
        private readonly int _typeCodeOffset;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal LocalConstantTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            _isStringHeapRefSizeSmall = stringHeapRefSize == 2;
            _isBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            _valueOffset = NameOffset + stringHeapRefSize;
            _typeCodeOffset = _valueOffset + blobHeapRefSize;
            this.RowSize = _typeCodeOffset + sizeof(byte);

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal StringHandle GetName(LocalConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + NameOffset, _isStringHeapRefSizeSmall));
        }

        internal BlobHandle GetValue(LocalConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + _valueOffset, _isBlobHeapRefSizeSmall));
        }

        internal ConstantTypeCode GetTypeCode(LocalConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (ConstantTypeCode)this.Block.PeekByte(rowOffset + _typeCodeOffset);
        }
    }

    internal struct AsyncMethodTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool _isMethodRefSizeSmall;
        private readonly bool _isBlobHeapRefSizeSmall;

        private readonly int _kickoffMethodOffset;
        private readonly int _catchHandlerOffsetOffset;
        private readonly int _awaitsOffset;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AsyncMethodTableReader(
            uint numberOfRows,
            int methodRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            _isMethodRefSizeSmall = methodRefSize == 2;
            _isBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            _kickoffMethodOffset = 0;
            _catchHandlerOffsetOffset = _kickoffMethodOffset + methodRefSize;
            _awaitsOffset = _catchHandlerOffsetOffset + sizeof(uint);
            this.RowSize = _awaitsOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal MethodDefinitionHandle GetKickoffMethod(AsyncMethodHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + _kickoffMethodOffset, _isMethodRefSizeSmall));
        }

        internal int GetCatchHandlerOffset(AsyncMethodHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            // TODO: overflow
            return (int)this.Block.PeekUInt32(rowOffset + _catchHandlerOffsetOffset);
        }

        internal BlobHandle GetAwaits(AsyncMethodHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + _awaitsOffset, _isBlobHeapRefSizeSmall));
        }
    }

    internal struct ImportScopeTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool _isImportScopeRefSizeSmall;
        private readonly bool _isBlobHeapRefSizeSmall;

        private const int ParentOffset = 0;
        private readonly int _importsOffset;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ImportScopeTableReader(
            uint numberOfRows,
            int importScopeRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            _isImportScopeRefSizeSmall = importScopeRefSize == 2;
            _isBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            _importsOffset = ParentOffset + importScopeRefSize;
            this.RowSize = _importsOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal ImportScopeHandle GetParent(ImportScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return ImportScopeHandle.FromRowId(this.Block.PeekReference(rowOffset + ParentOffset, _isImportScopeRefSizeSmall));
        }

        internal BlobHandle GetImports(ImportScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + _importsOffset, _isBlobHeapRefSizeSmall));
        }
    }

    internal struct CustomDebugInformationTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool _isHasCustomDebugInformationRefSizeSmall;
        private readonly bool _isGuidHeapRefSizeSmall;
        private readonly bool _isBlobHeapRefSizeSmall;

        private const int ParentOffset = 0;
        private readonly int _kindOffset;
        private readonly int _valueOffset;

        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal CustomDebugInformationTableReader(
            uint numberOfRows,
            int hasCustomDebugInformationRefSize,
            int guidHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            _isHasCustomDebugInformationRefSizeSmall = hasCustomDebugInformationRefSize == 2;
            _isGuidHeapRefSizeSmall = guidHeapRefSize == 2;
            _isBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            _kindOffset = ParentOffset + hasCustomDebugInformationRefSize;
            _valueOffset = _kindOffset + guidHeapRefSize;
            this.RowSize = _valueOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal Handle GetParent(CustomDebugInformationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return HasCustomDebugInformationTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + ParentOffset, _isHasCustomDebugInformationRefSizeSmall));
        }

        internal GuidHandle GetKind(CustomDebugInformationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GuidHandle.FromIndex(this.Block.PeekReference(rowOffset + _kindOffset, _isGuidHeapRefSizeSmall));
        }

        internal BlobHandle GetValue(CustomDebugInformationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + _valueOffset, _isBlobHeapRefSizeSmall));
        }
    }
}
