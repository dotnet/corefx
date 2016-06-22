// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace System.Reflection.Metadata.Ecma335
{
    public sealed partial class MetadataBuilder
    {
        internal SerializedMetadata GetSerializedMetadata(ImmutableArray<int> externalRowCounts, bool isStandaloneDebugMetadata)
        {
            var stringHeapBuilder = new HeapBlobBuilder(_stringHeapCapacity);
            var stringMap = SerializeStringHeap(stringHeapBuilder, _strings, _stringHeapStartOffset);

            Debug.Assert(HeapIndex.UserString == 0);
            Debug.Assert((int)HeapIndex.String == 1);
            Debug.Assert((int)HeapIndex.Blob == 2);
            Debug.Assert((int)HeapIndex.Guid == 3);

            var heapSizes = ImmutableArray.Create(
                _userStringBuilder.Count,
                stringHeapBuilder.Count,
                _blobHeapSize,
                _guidBuilder.Count);

            var sizes = new MetadataSizes(GetRowCounts(), externalRowCounts, heapSizes, isStandaloneDebugMetadata);

            return new SerializedMetadata(sizes, stringHeapBuilder, stringMap);
        }

        internal static void SerializeMetadataHeader(BlobBuilder builder, string metadataVersion, MetadataSizes sizes)
        {
            int startOffset = builder.Count;

            // signature
            builder.WriteUInt32(0x424A5342);

            // major version
            builder.WriteUInt16(1);

            // minor version
            builder.WriteUInt16(1);

            // reserved
            builder.WriteUInt32(0);

            // metadata version length
            builder.WriteUInt32(MetadataSizes.MetadataVersionPaddedLength);

            int n = Math.Min(MetadataSizes.MetadataVersionPaddedLength, metadataVersion.Length);
            for (int i = 0; i < n; i++)
            {
                builder.WriteByte((byte)metadataVersion[i]);
            }

            for (int i = n; i < MetadataSizes.MetadataVersionPaddedLength; i++)
            {
                builder.WriteByte(0);
            }

            // reserved
            builder.WriteUInt16(0);

            // number of streams
            builder.WriteUInt16((ushort)(5 + (sizes.IsEncDelta ? 1 : 0) + (sizes.IsStandaloneDebugMetadata ? 1 : 0)));

            // stream headers
            int offsetFromStartOfMetadata = sizes.MetadataHeaderSize;

            // emit the #Pdb stream first so that only a single page has to be read in order to find out PDB ID
            if (sizes.IsStandaloneDebugMetadata)
            {
                SerializeStreamHeader(ref offsetFromStartOfMetadata, sizes.StandalonePdbStreamSize, "#Pdb", builder);
            }

            // Spec: Some compilers store metadata in a #- stream, which holds an uncompressed, or non-optimized, representation of metadata tables;
            // this includes extra metadata -Ptr tables. Such PE files do not form part of ECMA-335 standard.
            //
            // Note: EnC delta is stored as uncompressed metadata stream.
            SerializeStreamHeader(ref offsetFromStartOfMetadata, sizes.MetadataTableStreamSize, (sizes.IsCompressed ? "#~" : "#-"), builder);

            SerializeStreamHeader(ref offsetFromStartOfMetadata, sizes.GetAlignedHeapSize(HeapIndex.String), "#Strings", builder);
            SerializeStreamHeader(ref offsetFromStartOfMetadata, sizes.GetAlignedHeapSize(HeapIndex.UserString), "#US", builder);
            SerializeStreamHeader(ref offsetFromStartOfMetadata, sizes.GetAlignedHeapSize(HeapIndex.Guid), "#GUID", builder);
            SerializeStreamHeader(ref offsetFromStartOfMetadata, sizes.GetAlignedHeapSize(HeapIndex.Blob), "#Blob", builder);

            if (sizes.IsEncDelta)
            {
                SerializeStreamHeader(ref offsetFromStartOfMetadata, 0, "#JTD", builder);
            }

            int endOffset = builder.Count;
            Debug.Assert(endOffset - startOffset == sizes.MetadataHeaderSize);
        }

        private static void SerializeStreamHeader(ref int offsetFromStartOfMetadata, int alignedStreamSize, string streamName, BlobBuilder builder)
        {
            // 4 for the first uint (offset), 4 for the second uint (padded size), length of stream name + 1 for null terminator (then padded)
            int sizeOfStreamHeader = MetadataSizes.GetMetadataStreamHeaderSize(streamName);
            builder.WriteInt32(offsetFromStartOfMetadata);
            builder.WriteInt32(alignedStreamSize);
            foreach (char ch in streamName)
            {
                builder.WriteByte((byte)ch);
            }

            // After offset, size, and stream name, write 0-bytes until we reach our padded size.
            for (uint i = 8 + (uint)streamName.Length; i < sizeOfStreamHeader; i++)
            {
                builder.WriteByte(0);
            }

            offsetFromStartOfMetadata += alignedStreamSize;
        }
    }
}
