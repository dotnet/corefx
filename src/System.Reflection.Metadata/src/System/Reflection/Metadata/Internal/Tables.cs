// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Reflection.Internal;

namespace System.Reflection.Metadata.Ecma335
{
    internal struct ModuleTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsGUIDHeapRefSizeSmall;
        private readonly int _GenerationOffset;
        private readonly int _NameOffset;
        private readonly int _MVIdOffset;
        private readonly int _EnCIdOffset;
        private readonly int _EnCBaseIdOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ModuleTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            int guidHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsGUIDHeapRefSizeSmall = guidHeapRefSize == 2;
            this._GenerationOffset = 0;
            this._NameOffset = this._GenerationOffset + sizeof(UInt16);
            this._MVIdOffset = this._NameOffset + stringHeapRefSize;
            this._EnCIdOffset = this._MVIdOffset + guidHeapRefSize;
            this._EnCBaseIdOffset = this._EnCIdOffset + guidHeapRefSize;
            this.RowSize = this._EnCBaseIdOffset + guidHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, this.RowSize * (int)numberOfRows);
        }

        internal ushort GetGeneration()
        {
            Debug.Assert(NumberOfRows > 0);
            return this.Block.PeekUInt16(this._GenerationOffset);
        }

        internal StringHandle GetName()
        {
            Debug.Assert(NumberOfRows > 0);
            return StringHandle.FromIndex(this.Block.PeekReference(this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal GuidHandle GetMvid()
        {
            Debug.Assert(NumberOfRows > 0);
            return GuidHandle.FromIndex(this.Block.PeekReference(this._MVIdOffset, this._IsGUIDHeapRefSizeSmall));
        }

        internal GuidHandle GetEncId()
        {
            Debug.Assert(NumberOfRows > 0);
            return GuidHandle.FromIndex(this.Block.PeekReference(this._EnCIdOffset, this._IsGUIDHeapRefSizeSmall));
        }

        internal GuidHandle GetEncBaseId()
        {
            Debug.Assert(NumberOfRows > 0);
            return GuidHandle.FromIndex(this.Block.PeekReference(this._EnCBaseIdOffset, this._IsGUIDHeapRefSizeSmall));
        }
    }

    internal struct TypeRefTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsResolutionScopeRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _ResolutionScopeOffset;
        private readonly int _NameOffset;
        private readonly int _NamespaceOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal TypeRefTableReader(
            uint numberOfRows,
            int resolutionScopeRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsResolutionScopeRefSizeSmall = resolutionScopeRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._ResolutionScopeOffset = 0;
            this._NameOffset = this._ResolutionScopeOffset + resolutionScopeRefSize;
            this._NamespaceOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._NamespaceOffset + stringHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal Handle GetResolutionScope(TypeReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return ResolutionScopeTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ResolutionScopeOffset, this._IsResolutionScopeRefSizeSmall));
        }

        internal StringHandle GetName(TypeReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal StringHandle GetNamespace(TypeReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NamespaceOffset, this._IsStringHeapRefSizeSmall));
        }
    }

    internal struct TypeDefTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsFieldRefSizeSmall;
        private readonly bool _IsMethodRefSizeSmall;
        private readonly bool _IsTypeDefOrRefRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _NamespaceOffset;
        private readonly int _ExtendsOffset;
        private readonly int _FieldListOffset;
        private readonly int _MethodListOffset;
        internal readonly int RowSize;
        internal MemoryBlock Block;

        internal TypeDefTableReader(
            uint numberOfRows,
            int fieldRefSize,
            int methodRefSize,
            int typeDefOrRefRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsFieldRefSizeSmall = fieldRefSize == 2;
            this._IsMethodRefSizeSmall = methodRefSize == 2;
            this._IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._NameOffset = this._FlagsOffset + sizeof(UInt32);
            this._NamespaceOffset = this._NameOffset + stringHeapRefSize;
            this._ExtendsOffset = this._NamespaceOffset + stringHeapRefSize;
            this._FieldListOffset = this._ExtendsOffset + typeDefOrRefRefSize;
            this._MethodListOffset = this._FieldListOffset + fieldRefSize;
            this.RowSize = this._MethodListOffset + methodRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal TypeAttributes GetFlags(TypeDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (TypeAttributes)this.Block.PeekUInt32(rowOffset + this._FlagsOffset);
        }

        internal NamespaceDefinitionHandle GetNamespace(TypeDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return NamespaceDefinitionHandle.FromIndexOfFullName(this.Block.PeekReference(rowOffset + this._NamespaceOffset, this._IsStringHeapRefSizeSmall));
        }

        internal StringHandle GetNamespaceString(TypeDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NamespaceOffset, this._IsStringHeapRefSizeSmall));
        }

        internal StringHandle GetName(TypeDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal Handle GetExtends(TypeDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return TypeDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ExtendsOffset, this._IsTypeDefOrRefRefSizeSmall));
        }

        internal uint GetFieldStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + this._FieldListOffset, this._IsFieldRefSizeSmall);
        }

        internal uint GetMethodStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + this._MethodListOffset, this._IsMethodRefSizeSmall);
        }

        internal TypeDefinitionHandle FindTypeContainingMethod(uint methodDefOrPtrRowId, int numberOfMethods)
        {
            uint numOfRows = this.NumberOfRows;
            int slot = this.Block.BinarySearchForSlot(numOfRows, this.RowSize, this._MethodListOffset, methodDefOrPtrRowId, this._IsMethodRefSizeSmall);
            uint row = (uint)(slot + 1);
            if (row == 0) return default(TypeDefinitionHandle);

            if (row > numOfRows)
            {
                if (methodDefOrPtrRowId <= numberOfMethods) return TypeDefinitionHandle.FromRowId(numOfRows);

                return default(TypeDefinitionHandle);
            }

            uint value = this.GetMethodStart(row);
            if (value == methodDefOrPtrRowId)
            {
                while (row < numOfRows)
                {
                    uint newRow = row + 1;
                    value = this.GetMethodStart(newRow);
                    if (value == methodDefOrPtrRowId)
                        row = newRow;
                    else
                        break;
                }
            }

            return TypeDefinitionHandle.FromRowId(row);
        }

        internal TypeDefinitionHandle FindTypeContainingField(uint fieldDefOrPtrRowId, int numberOfFields)
        {
            uint numOfRows = this.NumberOfRows;
            int slot = this.Block.BinarySearchForSlot(numOfRows, this.RowSize, this._FieldListOffset, fieldDefOrPtrRowId, this._IsFieldRefSizeSmall);
            uint row = (uint)(slot + 1);
            if (row == 0) return default(TypeDefinitionHandle);

            if (row > numOfRows)
            {
                if (fieldDefOrPtrRowId <= numberOfFields) return TypeDefinitionHandle.FromRowId(numOfRows);

                return default(TypeDefinitionHandle);
            }

            uint value = this.GetFieldStart(row);
            if (value == fieldDefOrPtrRowId)
            {
                while (row < numOfRows)
                {
                    uint newRow = row + 1;
                    value = this.GetFieldStart(newRow);
                    if (value == fieldDefOrPtrRowId)
                        row = newRow;
                    else
                        break;
                }
            }

            return TypeDefinitionHandle.FromRowId(row);
        }
    }

    internal struct FieldPtrTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsFieldTableRowRefSizeSmall;
        private readonly int _FieldOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal FieldPtrTableReader(
            uint numberOfRows,
            int fieldTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsFieldTableRowRefSizeSmall = fieldTableRowRefSize == 2;
            this._FieldOffset = 0;
            this.RowSize = this._FieldOffset + fieldTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal FieldDefinitionHandle GetFieldFor(int rowId)
        {
            int rowOffset = (rowId - 1) * this.RowSize;
            return FieldDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._FieldOffset, this._IsFieldTableRowRefSizeSmall));
        }

        internal uint GetRowIdForFieldDefRow(uint fieldDefRowId)
        {
            return (uint)(this.Block.LinearSearchReference(this.RowSize, this._FieldOffset, fieldDefRowId, this._IsFieldTableRowRefSizeSmall) + 1);
        }
    }

    internal struct FieldTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _SignatureOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal FieldTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._NameOffset = this._FlagsOffset + sizeof(UInt16);
            this._SignatureOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._SignatureOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal StringHandle GetName(FieldDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal FieldAttributes GetFlags(FieldDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (FieldAttributes)this.Block.PeekUInt16(rowOffset + this._FlagsOffset);
        }

        internal BlobHandle GetSignature(FieldDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._SignatureOffset, this._IsBlobHeapRefSizeSmall));
        }
    }

    internal struct MethodPtrTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsMethodTableRowRefSizeSmall;
        private readonly int _MethodOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal MethodPtrTableReader(
            uint numberOfRows,
            int methodTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsMethodTableRowRefSizeSmall = methodTableRowRefSize == 2;
            this._MethodOffset = 0;
            this.RowSize = this._MethodOffset + methodTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        // returns a rid
        internal MethodDefinitionHandle GetMethodFor(int rowId)
        {
            int rowOffset = (rowId - 1) * this.RowSize;
            return MethodDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._MethodOffset, this._IsMethodTableRowRefSizeSmall)); ;
        }

        internal uint GetRowIdForMethodDefRow(uint methodDefRowId)
        {
            return (uint)(this.Block.LinearSearchReference(this.RowSize, this._MethodOffset, methodDefRowId, this._IsMethodTableRowRefSizeSmall) + 1);
        }
    }

    internal struct MethodTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsParamRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _RVAOffset;
        private readonly int _ImplFlagsOffset;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _SignatureOffset;
        private readonly int _ParamListOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal MethodTableReader(
            uint numberOfRows,
            int paramRefSize,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsParamRefSizeSmall = paramRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._RVAOffset = 0;
            this._ImplFlagsOffset = this._RVAOffset + sizeof(UInt32);
            this._FlagsOffset = this._ImplFlagsOffset + sizeof(UInt16);
            this._NameOffset = this._FlagsOffset + sizeof(UInt16);
            this._SignatureOffset = this._NameOffset + stringHeapRefSize;
            this._ParamListOffset = this._SignatureOffset + blobHeapRefSize;
            this.RowSize = this._ParamListOffset + paramRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal uint GetParamStart(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + this._ParamListOffset, this._IsParamRefSizeSmall);
        }

        internal BlobHandle GetSignature(MethodDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._SignatureOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal int GetRva(MethodDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekInt32(rowOffset + this._RVAOffset);
        }

        internal StringHandle GetName(MethodDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal MethodAttributes GetFlags(MethodDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (MethodAttributes)this.Block.PeekUInt16(rowOffset + this._FlagsOffset);
        }

        internal MethodImplAttributes GetImplFlags(MethodDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (MethodImplAttributes)this.Block.PeekUInt16(rowOffset + this._ImplFlagsOffset);
        }

        internal int GetNextRVA(
          int rva
        )
        {
            int nextRVA = int.MaxValue;
            int endOffset = (int)this.NumberOfRows * this.RowSize;
            for (int iterOffset = this._RVAOffset; iterOffset < endOffset; iterOffset += this.RowSize)
            {
                int currentRVA = this.Block.PeekInt32(iterOffset);
                if (currentRVA > rva && currentRVA < nextRVA)
                {
                    nextRVA = currentRVA;
                }
            }

            return nextRVA == int.MaxValue ? -1 : nextRVA;
        }
    }

    internal struct ParamPtrTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsParamTableRowRefSizeSmall;
        private readonly int _ParamOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ParamPtrTableReader(
            uint numberOfRows,
            int paramTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsParamTableRowRefSizeSmall = paramTableRowRefSize == 2;
            this._ParamOffset = 0;
            this.RowSize = this._ParamOffset + paramTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal ParameterHandle GetParamFor(int rowId)
        {
            int rowOffset = (rowId - 1) * this.RowSize;
            return ParameterHandle.FromRowId(this.Block.PeekReference(rowOffset + this._ParamOffset, this._IsParamTableRowRefSizeSmall));
        }
    }

    internal struct ParamTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _SequenceOffset;
        private readonly int _NameOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ParamTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._SequenceOffset = this._FlagsOffset + sizeof(UInt16);
            this._NameOffset = this._SequenceOffset + sizeof(UInt16);
            this.RowSize = this._NameOffset + stringHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal ParameterAttributes GetFlags(ParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (ParameterAttributes)this.Block.PeekUInt16(rowOffset + this._FlagsOffset);
        }

        internal ushort GetSequence(ParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekUInt16(rowOffset + this._SequenceOffset);
        }

        internal StringHandle GetName(ParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }
    }

    internal struct InterfaceImplTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsTypeDefTableRowRefSizeSmall;
        private readonly bool _IsTypeDefOrRefRefSizeSmall;
        private readonly int _ClassOffset;
        private readonly int _InterfaceOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal InterfaceImplTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int typeDefTableRowRefSize,
            int typeDefOrRefRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
            this._IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
            this._ClassOffset = 0;
            this._InterfaceOffset = this._ClassOffset + typeDefTableRowRefSize;
            this.RowSize = this._InterfaceOffset + typeDefOrRefRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.InterfaceImpl);
            }
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ClassOffset, this._IsTypeDefTableRowRefSizeSmall);
        }

        internal void GetInterfaceImplRange(
            TypeDefinitionHandle typeDef,
            out int firstImplRowId,
            out int lastImplRowId)
        {
            uint typeDefRid = typeDef.RowId;

            int startRowNumber, endRowNumber;
            this.Block.BinarySearchReferenceRange(
                this.NumberOfRows,
                this.RowSize,
                this._ClassOffset,
                typeDefRid,
                this._IsTypeDefTableRowRefSizeSmall,
                out startRowNumber,
                out endRowNumber
            );

            if (startRowNumber == -1)
            {
                firstImplRowId = 1;
                lastImplRowId = 0;
            }
            else
            {
                firstImplRowId = startRowNumber + 1;
                lastImplRowId = endRowNumber + 1;
            }
        }

        internal Handle GetInterface(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return TypeDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._InterfaceOffset, this._IsTypeDefOrRefRefSizeSmall));
        }
    }

    internal struct MemberRefTableReader
    {
        internal uint NumberOfRows;
        private readonly bool _IsMemberRefParentRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _ClassOffset;
        private readonly int _NameOffset;
        private readonly int _SignatureOffset;
        internal readonly int RowSize;
        internal MemoryBlock Block;

        internal MemberRefTableReader(
            uint numberOfRows,
            int memberRefParentRefSize,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsMemberRefParentRefSizeSmall = memberRefParentRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._ClassOffset = 0;
            this._NameOffset = this._ClassOffset + memberRefParentRefSize;
            this._SignatureOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._SignatureOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetSignature(MemberReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._SignatureOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal StringHandle GetName(MemberReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal Handle GetClass(MemberReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MemberRefParentTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ClassOffset, this._IsMemberRefParentRefSizeSmall));
        }
    }

    internal struct ConstantTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsHasConstantRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _TypeOffset;
        private readonly int _ParentOffset;
        private readonly int _ValueOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ConstantTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int hasConstantRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsHasConstantRefSizeSmall = hasConstantRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._TypeOffset = 0;
            this._ParentOffset = this._TypeOffset + sizeof(Byte) + 1; // Alignment here (+1)...
            this._ValueOffset = this._ParentOffset + hasConstantRefSize;
            this.RowSize = this._ValueOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.Constant);
            }
        }

        internal ConstantTypeCode GetType(ConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (ConstantTypeCode)this.Block.PeekByte(rowOffset + this._TypeOffset);
        }

        internal BlobHandle GetValue(ConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._ValueOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal Handle GetParent(ConstantHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return HasConstantTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ParentOffset, this._IsHasConstantRefSizeSmall));
        }

        internal ConstantHandle FindConstant(
          Handle parentToken
        )
        {
            int foundRowNumber =
              this.Block.BinarySearchReference(
                this.NumberOfRows,
                this.RowSize,
                this._ParentOffset,
                HasConstantTag.ConvertToTag(parentToken),
                this._IsHasConstantRefSizeSmall
            );
            return ConstantHandle.FromRowId((uint)(foundRowNumber + 1));
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ParentOffset, this._IsHasConstantRefSizeSmall);
        }
    }

    internal struct CustomAttributeTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsHasCustomAttributeRefSizeSmall;
        private readonly bool _IsCustomAttriubuteTypeRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _ParentOffset;
        private readonly int _TypeOffset;
        private readonly int _ValueOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        // row ids in the CustomAttribute table sorted by parents
        internal readonly uint[] PtrTable;

        internal CustomAttributeTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int hasCustomAttributeRefSize,
            int customAttributeTypeRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsHasCustomAttributeRefSizeSmall = hasCustomAttributeRefSize == 2;
            this._IsCustomAttriubuteTypeRefSizeSmall = customAttributeTypeRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._ParentOffset = 0;
            this._TypeOffset = this._ParentOffset + hasCustomAttributeRefSize;
            this._ValueOffset = this._TypeOffset + customAttributeTypeRefSize;
            this.RowSize = this._ValueOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
            this.PtrTable = null;

            if (!declaredSorted && !CheckSorted())
            {
                this.PtrTable = this.Block.BuildPtrTable(
                    (int)numberOfRows,
                    this.RowSize,
                    this._ParentOffset,
                    this._IsHasCustomAttributeRefSizeSmall);
            }
        }

        internal Handle GetParent(CustomAttributeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return HasCustomAttributeTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ParentOffset, this._IsHasCustomAttributeRefSizeSmall));
        }

        internal Handle GetConstructor(CustomAttributeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return CustomAttributeTypeTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._TypeOffset, this._IsCustomAttriubuteTypeRefSizeSmall));
        }

        internal BlobHandle GetValue(CustomAttributeHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._ValueOffset, this._IsBlobHeapRefSizeSmall));
        }

        private uint GetParentTag(int index)
        {
            return this.Block.PeekReference(index * this.RowSize + this._ParentOffset, this._IsHasCustomAttributeRefSizeSmall);
        }

        internal void GetAttributeRange(Handle parentHandle, out int firstImplRowId, out int lastImplRowId)
        {
            int startRowNumber, endRowNumber;

            if (this.PtrTable != null)
            {
                this.Block.BinarySearchReferenceRange(
                    this.PtrTable,
                    this.RowSize,
                    this._ParentOffset,
                    HasCustomAttributeTag.ConvertToTag(parentHandle),
                    this._IsHasCustomAttributeRefSizeSmall,
                    out startRowNumber,
                    out endRowNumber
                );
            }
            else
            {
                this.Block.BinarySearchReferenceRange(
                    this.NumberOfRows,
                    this.RowSize,
                    this._ParentOffset,
                    HasCustomAttributeTag.ConvertToTag(parentHandle),
                    this._IsHasCustomAttributeRefSizeSmall,
                    out startRowNumber,
                    out endRowNumber
                );
            }

            if (startRowNumber == -1)
            {
                firstImplRowId = 1;
                lastImplRowId = 0;
            }
            else
            {
                firstImplRowId = startRowNumber + 1;
                lastImplRowId = endRowNumber + 1;
            }
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ParentOffset, this._IsHasCustomAttributeRefSizeSmall);
        }
    }

    internal struct FieldMarshalTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsHasFieldMarshalRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _ParentOffset;
        private readonly int _NativeTypeOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal FieldMarshalTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int hasFieldMarshalRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsHasFieldMarshalRefSizeSmall = hasFieldMarshalRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._ParentOffset = 0;
            this._NativeTypeOffset = this._ParentOffset + hasFieldMarshalRefSize;
            this.RowSize = this._NativeTypeOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.FieldMarshal);
            }
        }

        internal Handle GetParent(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return HasFieldMarshalTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ParentOffset, this._IsHasFieldMarshalRefSizeSmall));
        }

        internal BlobHandle GetNativeType(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NativeTypeOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal uint FindFieldMarshalRowId(Handle handle)
        {
            int foundRowNumber =
              this.Block.BinarySearchReference(
                this.NumberOfRows,
                this.RowSize,
                this._ParentOffset,
                HasFieldMarshalTag.ConvertToTag(handle),
                this._IsHasFieldMarshalRefSizeSmall
            );
            return (uint)(foundRowNumber + 1);
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ParentOffset, this._IsHasFieldMarshalRefSizeSmall);
        }
    }

    internal struct DeclSecurityTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsHasDeclSecurityRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _ActionOffset;
        private readonly int _ParentOffset;
        private readonly int _PermissionSetOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal DeclSecurityTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int hasDeclSecurityRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsHasDeclSecurityRefSizeSmall = hasDeclSecurityRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._ActionOffset = 0;
            this._ParentOffset = this._ActionOffset + sizeof(UInt16);
            this._PermissionSetOffset = this._ParentOffset + hasDeclSecurityRefSize;
            this.RowSize = this._PermissionSetOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.DeclSecurity);
            }
        }

        internal DeclarativeSecurityAction GetAction(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return (DeclarativeSecurityAction)this.Block.PeekUInt16(rowOffset + this._ActionOffset);
        }

        internal Handle GetParent(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return HasDeclSecurityTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ParentOffset, this._IsHasDeclSecurityRefSizeSmall));
        }

        internal BlobHandle GetPermissionSet(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._PermissionSetOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal void GetAttributeRange(Handle parentToken, out int firstImplRowId, out int lastImplRowId)
        {
            int startRowNumber, endRowNumber;

            this.Block.BinarySearchReferenceRange(
                this.NumberOfRows,
                this.RowSize,
                this._ParentOffset,
                HasDeclSecurityTag.ConvertToTag(parentToken),
                this._IsHasDeclSecurityRefSizeSmall,
                out startRowNumber,
                out endRowNumber);

            if (startRowNumber == -1)
            {
                firstImplRowId = 1;
                lastImplRowId = 0;
            }
            else
            {
                firstImplRowId = startRowNumber + 1;
                lastImplRowId = endRowNumber + 1;
            }
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ParentOffset, this._IsHasDeclSecurityRefSizeSmall);
        }
    }

    internal struct ClassLayoutTableReader
    {
        internal uint NumberOfRows;
        private readonly bool _IsTypeDefTableRowRefSizeSmall;
        private readonly int _PackagingSizeOffset;
        private readonly int _ClassSizeOffset;
        private readonly int _ParentOffset;
        internal readonly int RowSize;
        internal MemoryBlock Block;

        internal ClassLayoutTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int typeDefTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
            this._PackagingSizeOffset = 0;
            this._ClassSizeOffset = this._PackagingSizeOffset + sizeof(UInt16);
            this._ParentOffset = this._ClassSizeOffset + sizeof(UInt32);
            this.RowSize = this._ParentOffset + typeDefTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.ClassLayout);
            }
        }

        internal TypeDefinitionHandle GetParent(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._ParentOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        internal ushort GetPackingSize(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekUInt16(rowOffset + this._PackagingSizeOffset);
        }

        internal uint GetClassSize(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._ClassSizeOffset);
        }

        // Returns RowId (0 means we there is no record in this table corresponding to the specified type).
        internal uint FindRow(TypeDefinitionHandle typeDef)
        {
            return (uint)(1 + this.Block.BinarySearchReference(this.NumberOfRows, this.RowSize, this._ParentOffset, typeDef.RowId, this._IsTypeDefTableRowRefSizeSmall));
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ParentOffset, this._IsTypeDefTableRowRefSizeSmall);
        }
    }

    internal struct FieldLayoutTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsFieldTableRowRefSizeSmall;
        private readonly int _OffsetOffset;
        private readonly int _FieldOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal FieldLayoutTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int fieldTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsFieldTableRowRefSizeSmall = fieldTableRowRefSize == 2;
            this._OffsetOffset = 0;
            this._FieldOffset = this._OffsetOffset + sizeof(UInt32);
            this.RowSize = this._FieldOffset + fieldTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.FieldLayout);
            }
        }

        /// <summary>
        /// Returns field offset for given field RowId, or -1 if not available. 
        /// </summary>
        internal uint FindFieldLayoutRowId(FieldDefinitionHandle handle)
        {
            int rowNumber =
              this.Block.BinarySearchReference(
                this.NumberOfRows,
                this.RowSize,
                this._FieldOffset,
                handle.RowId,
                this._IsFieldTableRowRefSizeSmall
            );

            return (uint)(rowNumber + 1);
        }

        internal uint GetOffset(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._OffsetOffset);
        }

        internal FieldDefinitionHandle GetField(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return FieldDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._FieldOffset, this._IsFieldTableRowRefSizeSmall));
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._FieldOffset, this._IsFieldTableRowRefSizeSmall);
        }
    }

    internal struct StandAloneSigTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _SignatureOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal StandAloneSigTableReader(
            uint numberOfRows,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._SignatureOffset = 0;
            this.RowSize = this._SignatureOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetSignature(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._SignatureOffset, this._IsBlobHeapRefSizeSmall));
        }
    }

    internal struct EventMapTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsTypeDefTableRowRefSizeSmall;
        private readonly bool _IsEventRefSizeSmall;
        private readonly int _ParentOffset;
        private readonly int _EventListOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal EventMapTableReader(
            uint numberOfRows,
            int typeDefTableRowRefSize,
            int eventRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
            this._IsEventRefSizeSmall = eventRefSize == 2;
            this._ParentOffset = 0;
            this._EventListOffset = this._ParentOffset + typeDefTableRowRefSize;
            this.RowSize = this._EventListOffset + eventRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal uint FindEventMapRowIdFor(TypeDefinitionHandle typeDef)
        {
            // We do a linear scan here because we dont have these tables sorted
            // TODO: We can scan the table to see if it is sorted and use binary search if so.
            // Also, the compilers should make sure it's sorted.
            int rowNumber =
              this.Block.LinearSearchReference(
                this.RowSize,
                this._ParentOffset,
                typeDef.RowId,
                this._IsTypeDefTableRowRefSizeSmall
            );
            return (uint)(rowNumber + 1);
        }

        internal TypeDefinitionHandle GetParentType(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._ParentOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        internal uint GetEventListStartFor(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekReference(rowOffset + this._EventListOffset, this._IsEventRefSizeSmall);
        }
    }

    internal struct EventPtrTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsEventTableRowRefSizeSmall;
        private readonly int _EventOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal EventPtrTableReader(
            uint numberOfRows,
            int eventTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsEventTableRowRefSizeSmall = eventTableRowRefSize == 2;
            this._EventOffset = 0;
            this.RowSize = this._EventOffset + eventTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal EventDefinitionHandle GetEventFor(int rowId)
        {
            int rowOffset = (rowId - 1) * this.RowSize;
            return EventDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._EventOffset, this._IsEventTableRowRefSizeSmall));
        }
    }

    internal struct EventTableReader
    {
        internal uint NumberOfRows;
        private readonly bool _IsTypeDefOrRefRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _EventTypeOffset;
        internal readonly int RowSize;
        internal MemoryBlock Block;

        internal EventTableReader(
            uint numberOfRows,
            int typeDefOrRefRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._NameOffset = this._FlagsOffset + sizeof(UInt16);
            this._EventTypeOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._EventTypeOffset + typeDefOrRefRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal EventAttributes GetFlags(EventDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (EventAttributes)this.Block.PeekUInt16(rowOffset + this._FlagsOffset);
        }

        internal StringHandle GetName(EventDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal Handle GetEventType(EventDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return TypeDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._EventTypeOffset, this._IsTypeDefOrRefRefSizeSmall));
        }
    }

    internal struct PropertyMapTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsTypeDefTableRowRefSizeSmall;
        private readonly bool _IsPropertyRefSizeSmall;
        private readonly int _ParentOffset;
        private readonly int _PropertyListOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal PropertyMapTableReader(
            uint numberOfRows,
            int typeDefTableRowRefSize,
            int propertyRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
            this._IsPropertyRefSizeSmall = propertyRefSize == 2;
            this._ParentOffset = 0;
            this._PropertyListOffset = this._ParentOffset + typeDefTableRowRefSize;
            this.RowSize = this._PropertyListOffset + propertyRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal uint FindPropertyMapRowIdFor(TypeDefinitionHandle typeDef)
        {
            // We do a linear scan here because we dont have these tables sorted.
            // TODO: We can scan the table to see if it is sorted and use binary search if so.
            // Also, the compilers should make sure it's sorted.
            int rowNumber =
              this.Block.LinearSearchReference(
                this.RowSize,
                this._ParentOffset,
                typeDef.RowId,
                this._IsTypeDefTableRowRefSizeSmall
            );
            return (uint)(rowNumber + 1);
        }

        internal TypeDefinitionHandle GetParentType(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._ParentOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        internal uint GetPropertyListStartFor(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            uint propertyList = this.Block.PeekReference(rowOffset + this._PropertyListOffset, this._IsPropertyRefSizeSmall);
            return propertyList;
        }
    }

    internal struct PropertyPtrTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsPropertyTableRowRefSizeSmall;
        private readonly int _PropertyOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal PropertyPtrTableReader(
            uint numberOfRows,
            int propertyTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsPropertyTableRowRefSizeSmall = propertyTableRowRefSize == 2;
            this._PropertyOffset = 0;
            this.RowSize = this._PropertyOffset + propertyTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal PropertyDefinitionHandle GetPropertyFor(
          int rowId
        )
        // ^ requires rowId <= this.NumberOfRows;
        {
            int rowOffset = (rowId - 1) * this.RowSize;
            return PropertyDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._PropertyOffset, this._IsPropertyTableRowRefSizeSmall));
        }
    }

    internal struct PropertyTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _SignatureOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal PropertyTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._NameOffset = this._FlagsOffset + sizeof(UInt16);
            this._SignatureOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._SignatureOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal PropertyAttributes GetFlags(PropertyDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (PropertyAttributes)this.Block.PeekUInt16(rowOffset + this._FlagsOffset);
        }

        internal StringHandle GetName(PropertyDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal BlobHandle GetSignature(PropertyDefinitionHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._SignatureOffset, this._IsBlobHeapRefSizeSmall));
        }
    }

    internal struct MethodSemanticsTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsMethodTableRowRefSizeSmall;
        private readonly bool _IsHasSemanticRefSizeSmall;
        private readonly int _SemanticsFlagOffset;
        private readonly int _MethodOffset;
        private readonly int _AssociationOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal MethodSemanticsTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int methodTableRowRefSize,
            int hasSemanticRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsMethodTableRowRefSizeSmall = methodTableRowRefSize == 2;
            this._IsHasSemanticRefSizeSmall = hasSemanticRefSize == 2;
            this._SemanticsFlagOffset = 0;
            this._MethodOffset = this._SemanticsFlagOffset + sizeof(UInt16);
            this._AssociationOffset = this._MethodOffset + methodTableRowRefSize;
            this.RowSize = this._AssociationOffset + hasSemanticRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.MethodSemantics);
            }
        }

        internal MethodDefinitionHandle GetMethod(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return MethodDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._MethodOffset, this._IsMethodTableRowRefSizeSmall));
        }

        internal MethodSemanticsAttributes GetSemantics(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return (MethodSemanticsAttributes)this.Block.PeekUInt16(rowOffset + this._SemanticsFlagOffset);
        }

        internal Handle GetAssociation(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return HasSemanticsTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._AssociationOffset, this._IsHasSemanticRefSizeSmall));
        }

        // returns rowID
        internal uint FindSemanticMethodsForEvent(EventDefinitionHandle eventDef, out ushort methodCount)
        {
            methodCount = 0;
            uint searchCodedTag = HasSemanticsTag.ConvertEventHandleToTag(eventDef);
            return this.BinarySearchTag(searchCodedTag, ref methodCount);
        }

        internal uint FindSemanticMethodsForProperty(PropertyDefinitionHandle propertyDef, out ushort methodCount)
        {
            methodCount = 0;
            uint searchCodedTag = HasSemanticsTag.ConvertPropertyHandleToTag(propertyDef);
            return this.BinarySearchTag(searchCodedTag, ref methodCount);
        }

        private uint BinarySearchTag(uint searchCodedTag, ref ushort methodCount)
        {
            int startRowNumber, endRowNumber;
            this.Block.BinarySearchReferenceRange(
                this.NumberOfRows,
                this.RowSize,
                this._AssociationOffset,
                searchCodedTag,
                this._IsHasSemanticRefSizeSmall,
                out startRowNumber,
                out endRowNumber
            );

            if (startRowNumber == -1)
            {
                methodCount = 0;
                return 0;
            }

            methodCount = (ushort)(endRowNumber - startRowNumber + 1);
            return (uint)(startRowNumber + 1);
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._AssociationOffset, this._IsHasSemanticRefSizeSmall);
        }
    }

    internal struct MethodImplTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsTypeDefTableRowRefSizeSmall;
        private readonly bool _IsMethodDefOrRefRefSizeSmall;
        private readonly int _ClassOffset;
        private readonly int _MethodBodyOffset;
        private readonly int _MethodDeclarationOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal MethodImplTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int typeDefTableRowRefSize,
            int methodDefOrRefRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
            this._IsMethodDefOrRefRefSizeSmall = methodDefOrRefRefSize == 2;
            this._ClassOffset = 0;
            this._MethodBodyOffset = this._ClassOffset + typeDefTableRowRefSize;
            this._MethodDeclarationOffset = this._MethodBodyOffset + methodDefOrRefRefSize;
            this.RowSize = this._MethodDeclarationOffset + methodDefOrRefRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.MethodImpl);
            }
        }

        internal TypeDefinitionHandle GetClass(MethodImplementationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._ClassOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        internal Handle GetMethodBody(MethodImplementationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._MethodBodyOffset, this._IsMethodDefOrRefRefSizeSmall));
        }

        internal Handle GetMethodDeclaration(MethodImplementationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._MethodDeclarationOffset, this._IsMethodDefOrRefRefSizeSmall));
        }

        internal void GetMethodImplRange(
            TypeDefinitionHandle typeDef,
            out int firstImplRowId,
            out int lastImplRowId)
        {
            uint typeDefRid = typeDef.RowId;

            int startRowNumber, endRowNumber;
            this.Block.BinarySearchReferenceRange(
                this.NumberOfRows,
                this.RowSize,
                this._ClassOffset,
                typeDefRid,
                this._IsTypeDefTableRowRefSizeSmall,
                out startRowNumber,
                out endRowNumber
            );

            if (startRowNumber == -1)
            {
                firstImplRowId = 1;
                lastImplRowId = 0;
            }
            else
            {
                firstImplRowId = startRowNumber + 1;
                lastImplRowId = endRowNumber + 1;
            }
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._ClassOffset, this._IsTypeDefTableRowRefSizeSmall);
        }
    }

    internal struct ModuleRefTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _NameOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ModuleRefTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._NameOffset = 0;
            this.RowSize = this._NameOffset + stringHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal StringHandle GetName(ModuleReferenceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }
    }

    internal struct TypeSpecTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _SignatureOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal TypeSpecTableReader(
            uint numberOfRows,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._SignatureOffset = 0;
            this.RowSize = this._SignatureOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetSignature(TypeSpecificationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._SignatureOffset, this._IsBlobHeapRefSizeSmall));
        }
    }

    internal struct ImplMapTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsModuleRefTableRowRefSizeSmall;
        private readonly bool _IsMemberForwardRowRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _MemberForwardedOffset;
        private readonly int _ImportNameOffset;
        private readonly int _ImportScopeOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ImplMapTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int moduleRefTableRowRefSize,
            int memberForwardedRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsModuleRefTableRowRefSizeSmall = moduleRefTableRowRefSize == 2;
            this._IsMemberForwardRowRefSizeSmall = memberForwardedRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._MemberForwardedOffset = this._FlagsOffset + sizeof(UInt16);
            this._ImportNameOffset = this._MemberForwardedOffset + memberForwardedRefSize;
            this._ImportScopeOffset = this._ImportNameOffset + stringHeapRefSize;
            this.RowSize = this._ImportScopeOffset + moduleRefTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.ImplMap);
            }
        }

        internal MethodImport this[uint rowId]  // This is 1 based...
        {
            get
            {
                int rowOffset = (int)(rowId - 1) * this.RowSize;
                var pInvokeMapFlags = (MethodImportAttributes)Block.PeekUInt16(rowOffset + this._FlagsOffset);
                var importName = StringHandle.FromIndex(Block.PeekReference(rowOffset + this._ImportNameOffset, this._IsStringHeapRefSizeSmall));
                var importScope = ModuleReferenceHandle.FromRowId(Block.PeekReference(rowOffset + this._ImportScopeOffset, this._IsModuleRefTableRowRefSizeSmall));
                return new MethodImport(pInvokeMapFlags, importName, importScope);
            }
        }

        internal Handle GetMemberForwarded(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return MemberForwardedTag.ConvertToToken(Block.PeekTaggedReference(rowOffset + this._MemberForwardedOffset, this._IsMemberForwardRowRefSizeSmall));
        }

        internal uint FindImplForMethod(MethodDefinitionHandle methodDef)
        {
            uint searchCodedTag = MemberForwardedTag.ConvertMethodDefToTag(methodDef);
            return this.BinarySearchTag(searchCodedTag);
        }

        private uint BinarySearchTag(uint searchCodedTag)
        {
            int foundRowNumber =
              this.Block.BinarySearchReference(
                this.NumberOfRows,
                this.RowSize,
                this._MemberForwardedOffset,
                searchCodedTag,
                this._IsMemberForwardRowRefSizeSmall
            );
            return (uint)(foundRowNumber + 1);
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._MemberForwardedOffset, this._IsMemberForwardRowRefSizeSmall);
        }
    }

    internal struct FieldRVATableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsFieldTableRowRefSizeSmall;
        private readonly int _RVAOffset;
        private readonly int _FieldOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal FieldRVATableReader(
            uint numberOfRows,
            bool declaredSorted,
            int fieldTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsFieldTableRowRefSizeSmall = fieldTableRowRefSize == 2;
            this._RVAOffset = 0;
            this._FieldOffset = this._RVAOffset + sizeof(UInt32);
            this.RowSize = this._FieldOffset + fieldTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.FieldRva);
            }
        }

        internal int GetRVA(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return Block.PeekInt32(rowOffset + this._RVAOffset);
        }

        internal uint FindFieldRVARowId(uint fieldDefRowId)
        {
            int foundRowNumber = Block.BinarySearchReference(
                this.NumberOfRows,
                this.RowSize,
                this._FieldOffset,
                fieldDefRowId,
                this._IsFieldTableRowRefSizeSmall
            );

            return (uint)(foundRowNumber + 1);
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._FieldOffset, this._IsFieldTableRowRefSizeSmall);
        }
    }

    internal struct EnCLogTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly int _TokenOffset;
        private readonly int _FuncCodeOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal EnCLogTableReader(
            uint numberOfRows,
            MemoryBlock containingBlock,
            int containingBlockOffset,
            MetadataStreamKind metadataStreamKind)
        {
            // EnC tables are not allowed in a compressed stream.
            // However when asked for a snapshot of the current metadata after an EnC change has been applied 
            // the CLR includes the EnCLog table into the snapshot (but not EnCMap). We pretend EnCLog is empty.
            this.NumberOfRows = (metadataStreamKind == MetadataStreamKind.Compressed) ? 0 : numberOfRows;

            this._TokenOffset = 0;
            this._FuncCodeOffset = this._TokenOffset + sizeof(uint);
            this.RowSize = this._FuncCodeOffset + sizeof(uint);
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal uint GetToken(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._TokenOffset);
        }

#pragma warning disable 618 // Edit and continue API marked obsolete to give us more time to refactor
        internal EditAndContinueOperation GetFuncCode(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return (EditAndContinueOperation)this.Block.PeekUInt32(rowOffset + this._FuncCodeOffset);
        }
    }

    internal struct EnCMapTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly int _TokenOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal EnCMapTableReader(
            uint numberOfRows,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._TokenOffset = 0;
            this.RowSize = this._TokenOffset + sizeof(uint);
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal uint GetToken(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._TokenOffset);
        }
    }
#pragma warning restore 618 // Edit and continue API marked obsolete to give us more time to refactor

    internal struct AssemblyTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _HashAlgIdOffset;
        private readonly int _MajorVersionOffset;
        private readonly int _MinorVersionOffset;
        private readonly int _BuildNumberOffset;
        private readonly int _RevisionNumberOffset;
        private readonly int _FlagsOffset;
        private readonly int _PublicKeyOffset;
        private readonly int _NameOffset;
        private readonly int _CultureOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AssemblyTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            // NOTE: obfuscated assemblies may have more than one row in Assembly table,
            //       we ignore all rows but the first one
            this.NumberOfRows = numberOfRows > 1 ? 1 : numberOfRows;

            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._HashAlgIdOffset = 0;
            this._MajorVersionOffset = this._HashAlgIdOffset + sizeof(UInt32);
            this._MinorVersionOffset = this._MajorVersionOffset + sizeof(UInt16);
            this._BuildNumberOffset = this._MinorVersionOffset + sizeof(UInt16);
            this._RevisionNumberOffset = this._BuildNumberOffset + sizeof(UInt16);
            this._FlagsOffset = this._RevisionNumberOffset + sizeof(UInt16);
            this._PublicKeyOffset = this._FlagsOffset + sizeof(UInt32);
            this._NameOffset = this._PublicKeyOffset + blobHeapRefSize;
            this._CultureOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._CultureOffset + stringHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal AssemblyHashAlgorithm GetHashAlgorithm()
        {
            Debug.Assert(NumberOfRows == 1);
            return (AssemblyHashAlgorithm)this.Block.PeekUInt32(this._HashAlgIdOffset);
        }

        internal Version GetVersion()
        {
            Debug.Assert(NumberOfRows == 1);
            return new Version(
                this.Block.PeekUInt16(this._MajorVersionOffset),
                this.Block.PeekUInt16(this._MinorVersionOffset),
                this.Block.PeekUInt16(this._BuildNumberOffset),
                this.Block.PeekUInt16(this._RevisionNumberOffset));
        }

        internal AssemblyFlags GetFlags()
        {
            Debug.Assert(NumberOfRows == 1);
            return (AssemblyFlags)this.Block.PeekUInt32(this._FlagsOffset);
        }

        internal BlobHandle GetPublicKey()
        {
            Debug.Assert(NumberOfRows == 1);
            return BlobHandle.FromIndex(this.Block.PeekReference(this._PublicKeyOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal StringHandle GetName()
        {
            Debug.Assert(NumberOfRows == 1);
            return StringHandle.FromIndex(this.Block.PeekReference(this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal StringHandle GetCulture()
        {
            Debug.Assert(NumberOfRows == 1);
            return StringHandle.FromIndex(this.Block.PeekReference(this._CultureOffset, this._IsStringHeapRefSizeSmall));
        }
    }

    internal struct AssemblyProcessorTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly int _ProcessorOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AssemblyProcessorTableReader(
            uint numberOfRows,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._ProcessorOffset = 0;
            this.RowSize = this._ProcessorOffset + sizeof(UInt32);
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }
    }

    internal struct AssemblyOSTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly int _OSPlatformIdOffset;
        private readonly int _OSMajorVersionIdOffset;
        private readonly int _OSMinorVersionIdOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AssemblyOSTableReader(
            uint numberOfRows,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._OSPlatformIdOffset = 0;
            this._OSMajorVersionIdOffset = this._OSPlatformIdOffset + sizeof(UInt32);
            this._OSMinorVersionIdOffset = this._OSMajorVersionIdOffset + sizeof(UInt32);
            this.RowSize = this._OSMinorVersionIdOffset + sizeof(UInt32);
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }
    }

    internal struct AssemblyRefTableReader
    {
        /// <summary>
        /// In CLI metadata equal to the actual number of entries in AssemblyRef table.
        /// In WinMD metadata it includes synthesized AssemblyRefs in addition.
        /// </summary>
        internal readonly int NumberOfNonVirtualRows;
        internal readonly int NumberOfVirtualRows;

        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _MajorVersionOffset;
        private readonly int _MinorVersionOffset;
        private readonly int _BuildNumberOffset;
        private readonly int _RevisionNumberOffset;
        private readonly int _FlagsOffset;
        private readonly int _PublicKeyOrTokenOffset;
        private readonly int _NameOffset;
        private readonly int _CultureOffset;
        private readonly int _HashValueOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AssemblyRefTableReader(
            int numberOfRows,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset,
            MetadataKind metadataKind)
        {
            this.NumberOfNonVirtualRows = numberOfRows;
            this.NumberOfVirtualRows = (metadataKind == MetadataKind.Ecma335) ? 0 : (int)AssemblyReferenceHandle.VirtualIndex.Count;

            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._MajorVersionOffset = 0;
            this._MinorVersionOffset = this._MajorVersionOffset + sizeof(UInt16);
            this._BuildNumberOffset = this._MinorVersionOffset + sizeof(UInt16);
            this._RevisionNumberOffset = this._BuildNumberOffset + sizeof(UInt16);
            this._FlagsOffset = this._RevisionNumberOffset + sizeof(UInt16);
            this._PublicKeyOrTokenOffset = this._FlagsOffset + sizeof(UInt32);
            this._NameOffset = this._PublicKeyOrTokenOffset + blobHeapRefSize;
            this._CultureOffset = this._NameOffset + stringHeapRefSize;
            this._HashValueOffset = this._CultureOffset + stringHeapRefSize;
            this.RowSize = this._HashValueOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, this.RowSize * numberOfRows);
        }

        internal Version GetVersion(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return new Version(
                this.Block.PeekUInt16(rowOffset + this._MajorVersionOffset),
                this.Block.PeekUInt16(rowOffset + this._MinorVersionOffset),
                this.Block.PeekUInt16(rowOffset + this._BuildNumberOffset),
                this.Block.PeekUInt16(rowOffset + this._RevisionNumberOffset));
        }

        internal AssemblyFlags GetFlags(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return (AssemblyFlags)this.Block.PeekUInt32(rowOffset + this._FlagsOffset);
        }

        internal BlobHandle GetPublicKeyOrToken(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._PublicKeyOrTokenOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal StringHandle GetName(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal StringHandle GetCulture(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._CultureOffset, this._IsStringHeapRefSizeSmall));
        }

        internal BlobHandle GetHashValue(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._HashValueOffset, this._IsBlobHeapRefSizeSmall));
        }
    }

    internal struct AssemblyRefProcessorTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsAssemblyRefTableRowSizeSmall;
        private readonly int _ProcessorOffset;
        private readonly int _AssemblyRefOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AssemblyRefProcessorTableReader(
            uint numberOfRows,
            int assembyRefTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsAssemblyRefTableRowSizeSmall = assembyRefTableRowRefSize == 2;
            this._ProcessorOffset = 0;
            this._AssemblyRefOffset = this._ProcessorOffset + sizeof(UInt32);
            this.RowSize = this._AssemblyRefOffset + assembyRefTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }
    }

    internal struct AssemblyRefOSTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsAssemblyRefTableRowRefSizeSmall;
        private readonly int _OSPlatformIdOffset;
        private readonly int _OSMajorVersionIdOffset;
        private readonly int _OSMinorVersionIdOffset;
        private readonly int _AssemblyRefOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal AssemblyRefOSTableReader(
            uint numberOfRows,
            int assembyRefTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsAssemblyRefTableRowRefSizeSmall = assembyRefTableRowRefSize == 2;
            this._OSPlatformIdOffset = 0;
            this._OSMajorVersionIdOffset = this._OSPlatformIdOffset + sizeof(UInt32);
            this._OSMinorVersionIdOffset = this._OSMajorVersionIdOffset + sizeof(UInt32);
            this._AssemblyRefOffset = this._OSMinorVersionIdOffset + sizeof(UInt32);
            this.RowSize = this._AssemblyRefOffset + assembyRefTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }
    }

    internal struct FileTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _HashValueOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal FileTableReader(
            uint numberOfRows,
            int stringHeapRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._NameOffset = this._FlagsOffset + sizeof(UInt32);
            this._HashValueOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._HashValueOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal BlobHandle GetHashValue(AssemblyFileHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._HashValueOffset, this._IsBlobHeapRefSizeSmall));
        }

        internal uint GetFlags(AssemblyFileHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._FlagsOffset);
        }

        internal StringHandle GetName(AssemblyFileHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }
    }

    internal struct ExportedTypeTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsImplementationRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _FlagsOffset;
        private readonly int _TypeDefIdOffset;
        private readonly int _TypeNameOffset;
        private readonly int _TypeNamespaceOffset;
        private readonly int _ImplementationOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ExportedTypeTableReader(
            uint numberOfRows,
            int implementationRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsImplementationRefSizeSmall = implementationRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._FlagsOffset = 0;
            this._TypeDefIdOffset = this._FlagsOffset + sizeof(UInt32);
            this._TypeNameOffset = this._TypeDefIdOffset + sizeof(UInt32);
            this._TypeNamespaceOffset = this._TypeNameOffset + stringHeapRefSize;
            this._ImplementationOffset = this._TypeNamespaceOffset + stringHeapRefSize;
            this.RowSize = this._ImplementationOffset + implementationRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal StringHandle GetTypeName(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._TypeNameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal StringHandle GetTypeNamespaceString(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._TypeNamespaceOffset, this._IsStringHeapRefSizeSmall));
        }

        internal NamespaceDefinitionHandle GetTypeNamespace(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return NamespaceDefinitionHandle.FromIndexOfFullName(this.Block.PeekReference(rowOffset + this._TypeNamespaceOffset, this._IsStringHeapRefSizeSmall));
        }

        internal Handle GetImplementation(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return ImplementationTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ImplementationOffset, this._IsImplementationRefSizeSmall));
        }

        internal TypeAttributes GetFlags(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return (TypeAttributes)this.Block.PeekUInt32(rowOffset + this._FlagsOffset);
        }

        internal uint GetTypeDefId(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._TypeDefIdOffset);
        }

        internal uint GetNamespace(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            uint typeNamespace = this.Block.PeekReference(rowOffset + this._TypeNamespaceOffset, this._IsStringHeapRefSizeSmall);
            return typeNamespace;
        }
    }

    internal struct ManifestResourceTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsImplementationRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _OffsetOffset;
        private readonly int _FlagsOffset;
        private readonly int _NameOffset;
        private readonly int _ImplementationOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal ManifestResourceTableReader(
            uint numberOfRows,
            int implementationRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsImplementationRefSizeSmall = implementationRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._OffsetOffset = 0;
            this._FlagsOffset = this._OffsetOffset + sizeof(UInt32);
            this._NameOffset = this._FlagsOffset + sizeof(UInt32);
            this._ImplementationOffset = this._NameOffset + stringHeapRefSize;
            this.RowSize = this._ImplementationOffset + implementationRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal StringHandle GetName(ManifestResourceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal Handle GetImplementation(ManifestResourceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return ImplementationTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ImplementationOffset, this._IsImplementationRefSizeSmall));
        }

        internal uint GetOffset(ManifestResourceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekUInt32(rowOffset + this._OffsetOffset);
        }

        internal ManifestResourceAttributes GetFlags(ManifestResourceHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (ManifestResourceAttributes)this.Block.PeekUInt32(rowOffset + this._FlagsOffset);
        }
    }

    internal struct NestedClassTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsTypeDefTableRowRefSizeSmall;
        private readonly int _NestedClassOffset;
        private readonly int _EnclosingClassOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal NestedClassTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int typeDefTableRowRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset
        )
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeDefTableRowRefSizeSmall = typeDefTableRowRefSize == 2;
            this._NestedClassOffset = 0;
            this._EnclosingClassOffset = this._NestedClassOffset + typeDefTableRowRefSize;
            this.RowSize = this._EnclosingClassOffset + typeDefTableRowRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.NestedClass);
            }
        }

        internal TypeDefinitionHandle GetNestedClass(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._NestedClassOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        internal TypeDefinitionHandle GetEnclosingClass(uint rowId)
        {
            int rowOffset = (int)(rowId - 1) * this.RowSize;
            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowOffset + this._EnclosingClassOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        internal TypeDefinitionHandle FindEnclosingType(TypeDefinitionHandle nestedTypeDef)
        {
            int rowNumber =
              this.Block.BinarySearchReference(
                this.NumberOfRows,
                this.RowSize,
                this._NestedClassOffset,
                nestedTypeDef.RowId,
                this._IsTypeDefTableRowRefSizeSmall);

            if (rowNumber == -1)
                return default(TypeDefinitionHandle);

            return TypeDefinitionHandle.FromRowId(this.Block.PeekReference(rowNumber * this.RowSize + this._EnclosingClassOffset, this._IsTypeDefTableRowRefSizeSmall));
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._NestedClassOffset, this._IsTypeDefTableRowRefSizeSmall);
        }
    }

    internal struct GenericParamTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsTypeOrMethodDefRefSizeSmall;
        private readonly bool _IsStringHeapRefSizeSmall;
        private readonly int _NumberOffset;
        private readonly int _FlagsOffset;
        private readonly int _OwnerOffset;
        private readonly int _NameOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal GenericParamTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int typeOrMethodDefRefSize,
            int stringHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsTypeOrMethodDefRefSizeSmall = typeOrMethodDefRefSize == 2;
            this._IsStringHeapRefSizeSmall = stringHeapRefSize == 2;
            this._NumberOffset = 0;
            this._FlagsOffset = this._NumberOffset + sizeof(UInt16);
            this._OwnerOffset = this._FlagsOffset + sizeof(UInt16);
            this._NameOffset = this._OwnerOffset + typeOrMethodDefRefSize;
            this.RowSize = this._NameOffset + stringHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.GenericParam);
            }
        }

        internal ushort GetNumber(GenericParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return this.Block.PeekUInt16(rowOffset + this._NumberOffset);
        }

        internal GenericParameterAttributes GetFlags(GenericParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return (GenericParameterAttributes)this.Block.PeekUInt16(rowOffset + this._FlagsOffset);
        }

        internal StringHandle GetName(GenericParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return StringHandle.FromIndex(this.Block.PeekReference(rowOffset + this._NameOffset, this._IsStringHeapRefSizeSmall));
        }

        internal Handle GetOwner(GenericParameterHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return TypeOrMethodDefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._OwnerOffset, this._IsTypeOrMethodDefRefSizeSmall));
        }

        internal GenericParameterHandleCollection FindGenericParametersForType(TypeDefinitionHandle typeDef)
        {
            ushort count = 0;
            uint searchCodedTag = TypeOrMethodDefTag.ConvertTypeDefRowIdToTag(typeDef);
            int startRid = (int)this.BinarySearchTag(searchCodedTag, ref count);

            return new GenericParameterHandleCollection(startRid, count);
        }

        internal GenericParameterHandleCollection FindGenericParametersForMethod(MethodDefinitionHandle methodDef)
        {
            ushort count = 0;
            uint searchCodedTag = TypeOrMethodDefTag.ConvertMethodDefToTag(methodDef);
            int startRid = (int)this.BinarySearchTag(searchCodedTag, ref count);

            return new GenericParameterHandleCollection(startRid, count);
        }

        private uint BinarySearchTag(uint searchCodedTag, ref ushort genericParamCount)
        {
            int startRowNumber, endRowNumber;
            this.Block.BinarySearchReferenceRange(
                this.NumberOfRows,
                this.RowSize,
                this._OwnerOffset,
                searchCodedTag,
                this._IsTypeOrMethodDefRefSizeSmall,
                out startRowNumber,
                out endRowNumber);

            if (startRowNumber == -1)
            {
                genericParamCount = 0;
                return 0;
            }

            genericParamCount = (ushort)(endRowNumber - startRowNumber + 1);
            return (uint)(startRowNumber + 1);
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._OwnerOffset, this._IsTypeOrMethodDefRefSizeSmall);
        }
    }

    internal struct MethodSpecTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsMethodDefOrRefRefSizeSmall;
        private readonly bool _IsBlobHeapRefSizeSmall;
        private readonly int _MethodOffset;
        private readonly int _InstantiationOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal MethodSpecTableReader(
            uint numberOfRows,
            int methodDefOrRefRefSize,
            int blobHeapRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsMethodDefOrRefRefSizeSmall = methodDefOrRefRefSize == 2;
            this._IsBlobHeapRefSizeSmall = blobHeapRefSize == 2;
            this._MethodOffset = 0;
            this._InstantiationOffset = this._MethodOffset + methodDefOrRefRefSize;
            this.RowSize = this._InstantiationOffset + blobHeapRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));
        }

        internal Handle GetMethod(MethodSpecificationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return MethodDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._MethodOffset, this._IsMethodDefOrRefRefSizeSmall));
        }

        internal BlobHandle GetInstantiation(MethodSpecificationHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return BlobHandle.FromIndex(this.Block.PeekReference(rowOffset + this._InstantiationOffset, this._IsBlobHeapRefSizeSmall));
        }
    }

    internal struct GenericParamConstraintTableReader
    {
        internal readonly uint NumberOfRows;
        private readonly bool _IsGenericParamTableRowRefSizeSmall;
        private readonly bool _IsTypeDefOrRefRefSizeSmall;
        private readonly int _OwnerOffset;
        private readonly int _ConstraintOffset;
        internal readonly int RowSize;
        internal readonly MemoryBlock Block;

        internal GenericParamConstraintTableReader(
            uint numberOfRows,
            bool declaredSorted,
            int genericParamTableRowRefSize,
            int typeDefOrRefRefSize,
            MemoryBlock containingBlock,
            int containingBlockOffset)
        {
            this.NumberOfRows = numberOfRows;
            this._IsGenericParamTableRowRefSizeSmall = genericParamTableRowRefSize == 2;
            this._IsTypeDefOrRefRefSizeSmall = typeDefOrRefRefSize == 2;
            this._OwnerOffset = 0;
            this._ConstraintOffset = this._OwnerOffset + genericParamTableRowRefSize;
            this.RowSize = this._ConstraintOffset + typeDefOrRefRefSize;
            this.Block = containingBlock.GetMemoryBlockAt(containingBlockOffset, (int)(this.RowSize * numberOfRows));

            if (!declaredSorted && !CheckSorted())
            {
                MetadataReader.ThrowTableNotSorted(TableIndex.GenericParamConstraint);
            }
        }

        internal GenericParameterConstraintHandleCollection FindConstraintsForGenericParam(GenericParameterHandle genericParameter)
        {
            int startRowNumber, endRowNumber;
            this.Block.BinarySearchReferenceRange(
                this.NumberOfRows,
                this.RowSize,
                this._OwnerOffset,
                genericParameter.RowId,
                this._IsGenericParamTableRowRefSizeSmall,
                out startRowNumber,
                out endRowNumber);

            if (startRowNumber == -1)
            {
                return default(GenericParameterConstraintHandleCollection);
            }

            return new GenericParameterConstraintHandleCollection(
                firstRowId: startRowNumber + 1,
                count: (ushort)(endRowNumber - startRowNumber + 1));
        }

        private bool CheckSorted()
        {
            return this.Block.IsOrderedByReferenceAscending(this.RowSize, this._OwnerOffset, this._IsGenericParamTableRowRefSizeSmall);
        }

        internal Handle GetConstraint(GenericParameterConstraintHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return TypeDefOrRefTag.ConvertToToken(this.Block.PeekTaggedReference(rowOffset + this._ConstraintOffset, this._IsTypeDefOrRefRefSizeSmall));
        }

        internal GenericParameterHandle GetOwner(GenericParameterConstraintHandle handle)
        {
            int rowOffset = (int)(handle.RowId - 1) * this.RowSize;
            return GenericParameterHandle.FromRowId(this.Block.PeekReference(rowOffset + this._OwnerOffset, this._IsGenericParamTableRowRefSizeSmall));
        }
    }
}