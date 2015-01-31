// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection.Internal;

namespace System.Reflection.Metadata.Ecma335
{
    internal struct DocumentTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool IsGuidHeapRefSizeSmall;
        private readonly bool IsBlobHeapRefSizeSmall;

        private const int NameOffset = 0;
        private readonly int HashAlgorithmOffset;
        private readonly int HashOffset;
        private readonly int LanguageOffset;

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
            this.IsGuidHeapRefSizeSmall = guidHeapRefSize == 2;
            this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.HashAlgorithmOffset = NameOffset + blobHeapRefSize;
            this.HashOffset = HashAlgorithmOffset + guidHeapRefSize;
            this.LanguageOffset = HashOffset + blobHeapRefSize;
            this.RowSize = LanguageOffset + guidHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetName(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + NameOffset, this.IsBlobHeapRefSizeSmall));
        }

        internal GuidHandle GetHashAlgorithm(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GuidHandle.FromIndex(this.Block.PeekReference(rowOffset + HashAlgorithmOffset, this.IsGuidHeapRefSizeSmall));
        }

        internal BlobHandle GetHash(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + HashOffset, this.IsBlobHeapRefSizeSmall));
        }

        internal GuidHandle GetLanguage(DocumentHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GuidHandle.FromIndex(this.Block.PeekReference(rowOffset + LanguageOffset, this.IsGuidHeapRefSizeSmall));
        }
    }

    internal struct MethodBodyTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool IsBlobHeapRefSizeSmall;

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
            this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.RowSize = SequencePointsOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetSequencePoints(MethodBodyHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + SequencePointsOffset, this.IsBlobHeapRefSizeSmall));
        }
    }

    internal struct LocalScopeTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool IsMethodRefSmall;
        private readonly bool IsImportScopeRefSmall;
        private readonly bool IsLocalConstantRefSmall;
        private readonly bool IsLocalVariableRefSmall;

        private const int MethodOffset = 0;
        private readonly int ImportScopeOffset;
        private readonly int VariableListOffset;
        private readonly int ConstantListOffset;
        private readonly int StartOffsetOffset;
        private readonly int LengthOffset;

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
            this.IsMethodRefSmall = methodRefSize == 2;
            this.IsImportScopeRefSmall = importScopeRefSize == 2;
            this.IsLocalVariableRefSmall = localVariableRefSize == 2;
            this.IsLocalConstantRefSmall = localConstantRefSize == 2;

            this.ImportScopeOffset = MethodOffset + methodRefSize;
            this.VariableListOffset = ImportScopeOffset + importScopeRefSize;
            this.ConstantListOffset = this.VariableListOffset + localVariableRefSize;
            this.StartOffsetOffset = this.ConstantListOffset + localConstantRefSize;
            this.LengthOffset = this.StartOffsetOffset + sizeof(uint);
            this.RowSize = LengthOffset + sizeof(uint);

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal MethodDefinitionHandle GetMethod(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + MethodOffset, this.IsMethodRefSmall));
        }

        internal ImportScopeHandle GetImportScope(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return ImportScopeHandle.FromRowId(this.Block.PeekReference(rowOffset + ImportScopeOffset, this.IsImportScopeRefSmall));
        }

        internal uint GetVariableStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + this.VariableListOffset, IsLocalVariableRefSmall);
        }

        internal uint GetConstantStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + this.ConstantListOffset, IsLocalConstantRefSmall);
        }

        internal int GetStartOffset(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            // TODO: overflow
            return (int)this.Block.PeekUInt32(rowOffset + StartOffsetOffset);
        }

        internal int GetLength(LocalScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            // TODO: overflow
            return (int)this.Block.PeekUInt32(rowOffset + LengthOffset);
        }
    }

    internal struct LocalVariableTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool IsStringHeapRefSizeSmall;
        private readonly int AttributesOffset;
        private readonly int IndexOffset;
        private readonly int NameOffset;
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
            this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;

            this.AttributesOffset = 0;
            this.IndexOffset = this.AttributesOffset + sizeof(ushort);
            this.NameOffset = this.IndexOffset + sizeof(ushort);
            this.RowSize = this.NameOffset + stringHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal LocalVariableAttributes GetAttributes(LocalVariableHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (LocalVariableAttributes)this.Block.PeekUInt16(rowOffset + this.AttributesOffset);
        }

        internal ushort GetIndex(LocalVariableHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekUInt16(rowOffset + this.IndexOffset);
        }

        internal StringHandle GetName(LocalVariableHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this.NameOffset, this.IsStringHeapRefSizeSmall));
        }
    }

    internal struct LocalConstantTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool IsStringHeapRefSizeSmall;
        private readonly bool IsBlobHeapRefSizeSmall;

        private const int NameOffset = 0;
        private readonly int ValueOffset;
        private readonly int TypeCodeOffset;

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
            this.IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.ValueOffset = NameOffset + stringHeapRefSize;
            this.TypeCodeOffset = ValueOffset + blobHeapRefSize;
            this.RowSize = TypeCodeOffset + sizeof(byte);

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal StringHandle GetName(LocalConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + NameOffset, this.IsStringHeapRefSizeSmall));
        }

        internal BlobHandle GetValue(LocalConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + ValueOffset, this.IsBlobHeapRefSizeSmall));
        }

        internal ConstantTypeCode GetTypeCode(LocalConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (ConstantTypeCode)this.Block.PeekByte(rowOffset + TypeCodeOffset);
        }
    }

    internal struct AsyncMethodTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool IsMethodRefSizeSmall;
        private readonly bool IsBlobHeapRefSizeSmall;

        private readonly int KickoffMethodOffset;
        private readonly int CatchHandlerOffsetOffset;
        private readonly int AwaitsOffset;

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
            this.IsMethodRefSizeSmall = methodRefSize == 2;
            this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.KickoffMethodOffset = 0;
            this.CatchHandlerOffsetOffset = this.KickoffMethodOffset + methodRefSize;
            this.AwaitsOffset = this.CatchHandlerOffsetOffset + sizeof(uint);
            this.RowSize = this.AwaitsOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal MethodDefinitionHandle GetKickoffMethod(AsyncMethodHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this.KickoffMethodOffset, this.IsMethodRefSizeSmall));
        }

        internal int GetCatchHandlerOffset(AsyncMethodHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            // TODO: overflow
            return (int)this.Block.PeekUInt32(rowOffset + this.CatchHandlerOffsetOffset);
        }

        internal BlobHandle GetAwaits(AsyncMethodHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this.AwaitsOffset, this.IsBlobHeapRefSizeSmall));
        }
    }

    internal struct ImportScopeTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool IsImportScopeRefSizeSmall;
        private readonly bool IsBlobHeapRefSizeSmall;

        private const int ParentOffset = 0;
        private readonly int ImportsOffset;

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
            this.IsImportScopeRefSizeSmall = importScopeRefSize == 2;
            this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.ImportsOffset = ParentOffset + importScopeRefSize;
            this.RowSize = ImportsOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal ImportScopeHandle GetParent(ImportScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return ImportScopeHandle.FromRowId(this.Block.PeekReference(rowOffset + ParentOffset, this.IsImportScopeRefSizeSmall));
        }

        internal BlobHandle GetImports(ImportScopeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + ImportsOffset, this.IsBlobHeapRefSizeSmall));
        }
    }

    internal struct CustomDebugInformationTableReader
    {
        internal readonly uint NumberOfRows;

        private readonly bool IsHasCustomDebugInformationRefSizeSmall;
        private readonly bool IsGuidHeapRefSizeSmall;
        private readonly bool IsBlobHeapRefSizeSmall;

        private const int ParentOffset = 0;
        private readonly int KindOffset;
        private readonly int ValueOffset;

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
            this.IsHasCustomDebugInformationRefSizeSmall = hasCustomDebugInformationRefSize == 2;
            this.IsGuidHeapRefSizeSmall = guidHeapRefSize == 2;
            this.IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;

            this.KindOffset = ParentOffset + hasCustomDebugInformationRefSize;
            this.ValueOffset = KindOffset + guidHeapRefSize;
            this.RowSize = ValueOffset + blobHeapRefSize;

            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal Handle GetParent(CustomDebugInformationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return HasCustomDebugInformationTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + ParentOffset, this.IsHasCustomDebugInformationRefSizeSmall));
        }

        internal GuidHandle GetKind(CustomDebugInformationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GuidHandle.FromIndex(this.Block.PeekReference(rowOffset + KindOffset, this.IsGuidHeapRefSizeSmall));
        }

        internal BlobHandle GetValue(CustomDebugInformationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + ValueOffset, this.IsBlobHeapRefSizeSmall));
        }
    }
}
