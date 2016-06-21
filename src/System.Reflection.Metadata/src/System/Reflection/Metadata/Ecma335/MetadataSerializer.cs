// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace System.Reflection.Metadata.Ecma335
{
    /// <summary>
    /// Builder of a Portable PDB image.
    /// </summary>
    public sealed class PortablePdbBuilder
    {
        public static string MetadataVersionString => "PDB v1.0";
        public static ushort FormatVersion => 0x0100;

        private Blob _pdbIdBlob;
        private readonly MethodDefinitionHandle _entryPoint;
        private readonly MetadataBuilder _builder;
        private readonly SerializedMetadata _serializedMetadata;

        public Func<IEnumerable<Blob>, BlobContentId> IdProvider { get; }

        /// <summary>
        /// Creates a builder of a Portable PDB image.
        /// </summary>
        /// <param name="tablesAndHeaps">
        /// Builder populated with debug metadata entities stored in tables and values stored in heaps.
        /// The entities and values will be enumerated when serializing the Portable PDB image.
        /// </param>
        /// <param name="typeSystemRowCounts">
        /// Row counts of all tables that the associated type-system metadata contain.
        /// </param>
        /// <param name="entryPoint">
        /// Entry point method definition handle.
        /// </param>
        /// <param name="idProvider">
        /// Function calculating id of content represented as a sequence of blobs.
        /// If not specified a default function that ignores the content and returns current time-based content id is used 
        /// (<see cref="BlobContentId.GetTimeBasedProvider()"/>).
        /// You must specify a deterministic function to produce a deterministic Portable PDB image.
        /// </param>
        public PortablePdbBuilder(
            MetadataBuilder tablesAndHeaps, 
            ImmutableArray<int> typeSystemRowCounts,
            MethodDefinitionHandle entryPoint,
            Func<IEnumerable<Blob>, BlobContentId> idProvider = null)
        {
            if (tablesAndHeaps == null)
            {
                Throw.ArgumentNull(nameof(tablesAndHeaps));
            }

            if (typeSystemRowCounts.IsDefault)
            {
                Throw.ArgumentNull(nameof(typeSystemRowCounts));
            }

            _builder = tablesAndHeaps;
            _entryPoint = entryPoint;
            _serializedMetadata = tablesAndHeaps.GetSerializedMetadata(typeSystemRowCounts, isStandaloneDebugMetadata: true);
            IdProvider = idProvider ?? BlobContentId.GetTimeBasedProvider();
        }

        /// <summary>
        /// Serialized #Pdb stream.
        /// </summary>
        private void SerializeStandalonePdbStream(BlobBuilder builder)
        {
            int startPosition = builder.Count;

            // the id will be filled in later
            _pdbIdBlob = builder.ReserveBytes(MetadataSizes.PdbIdSize);
            
            builder.WriteInt32(_entryPoint.IsNil ? 0 : MetadataTokens.GetToken(_entryPoint));

            builder.WriteUInt64(_serializedMetadata.Sizes.ExternalTablesMask);
            MetadataWriterUtilities.SerializeRowCounts(builder, _serializedMetadata.Sizes.ExternalRowCounts);

            int endPosition = builder.Count;
            Debug.Assert(_serializedMetadata.Sizes.CalculateStandalonePdbStreamSize() == endPosition - startPosition);
        }

        /// <summary>
        /// Serializes Portable PDB content into the given <see cref="BlobBuilder"/>.
        /// </summary>
        /// <param name="builder">Builder to write to.</param>
        /// <returns>The id of the serialized content.</returns>
        public BlobContentId Serialize(BlobBuilder builder)
        {
            if (builder == null)
            {
                Throw.ArgumentNull(nameof(builder));
            }

            // header:
            MetadataBuilderHelpers.SerializeMetadataHeader(builder, MetadataVersionString, _serializedMetadata.Sizes);

            // #Pdb stream
            SerializeStandalonePdbStream(builder);

            // #~ or #- stream:
            _builder.SerializeMetadataTables(builder, _serializedMetadata.Sizes, _serializedMetadata.StringMap, methodBodyStreamRva: 0, mappedFieldDataStreamRva: 0);

            // #Strings, #US, #Guid and #Blob streams:
            _builder.WriteHeapsTo(builder, _serializedMetadata.StringHeap);

            var contentId = IdProvider(builder.GetBlobs());

            // fill in the id:
            var idWriter = new BlobWriter(_pdbIdBlob);
            idWriter.WriteGuid(contentId.Guid);
            idWriter.WriteUInt32(contentId.Stamp);
            Debug.Assert(idWriter.RemainingBytes == 0);

            return contentId;
        }
    }

    /// <summary>
    /// Builder of a Metadata Root to be embedded in a Portable Executable image.
    /// </summary>
    /// <remarks>
    /// Metadata root constitutes of a metadata header followed by metadata streams (#~, #Strings, #US, #Guid and #Blob).
    /// </remarks>
    public sealed class MetadataRootBuilder
    {
        public static string MetadataVersionString => "v4.0.30319";

        // internal for testing
        internal static readonly ImmutableArray<int> EmptyRowCounts = ImmutableArray.CreateRange(Enumerable.Repeat(0, MetadataTokens.TableCount));

        private readonly string _metadataVersion;
        private readonly MetadataBuilder _tablesAndHeaps;
        private readonly SerializedMetadata _serializedMetadata;

        /// <summary>
        /// Creates a builder of a metadata root.
        /// </summary>
        /// <param name="tablesAndHeaps">
        /// Builder populated with metadata entities stored in tables and values stored in heaps.
        /// The entities and values will be enumerated when serializing the metadata root.
        /// </param>
        /// <param name="metadataVersion">
        /// The version string written to the metadata header. The default value is <see cref="MetadataVersionString"/>.
        /// </param>
        public MetadataRootBuilder(MetadataBuilder tablesAndHeaps, string metadataVersion = null)
        {
            if (tablesAndHeaps == null)
            {
                Throw.ArgumentNull(nameof(tablesAndHeaps));
            }

            _tablesAndHeaps = tablesAndHeaps;
            _metadataVersion = metadataVersion ?? MetadataVersionString;
            _serializedMetadata = tablesAndHeaps.GetSerializedMetadata(EmptyRowCounts, isStandaloneDebugMetadata: false);
        }

        internal MetadataSizes Sizes => _serializedMetadata.Sizes;

        /// <summary>
        /// Serialized the metadata root content into the given <see cref="BlobBuilder"/>.
        /// </summary>
        /// <param name="builder">Builder to write to.</param>
        /// <param name="methodBodyStreamRva">
        /// The relative virtual address of the start of the method body stream.
        /// Used to calculate the final value of RVA fields of MethodDef table.
        /// </param>
        /// <param name="mappedFieldDataStreamRva">
        /// The relative virtual address of the start of the field init data stream.
        /// Used to calculate the final value of RVA fields of FieldRVA table.
        /// </param>
        public void Serialize(BlobBuilder builder, int methodBodyStreamRva, int mappedFieldDataStreamRva)
        {
            if (builder == null)
            {
                Throw.ArgumentNull(nameof(builder));
            }

            if (methodBodyStreamRva < 0)
            {
                Throw.ArgumentOutOfRange(nameof(methodBodyStreamRva));
            }

            if (mappedFieldDataStreamRva < 0)
            {
                Throw.ArgumentOutOfRange(nameof(mappedFieldDataStreamRva));
            }

            // header:
            MetadataBuilderHelpers.SerializeMetadataHeader(builder, _metadataVersion, _serializedMetadata.Sizes);

            // #~ or #- stream:
            _tablesAndHeaps.SerializeMetadataTables(builder, _serializedMetadata.Sizes, _serializedMetadata.StringMap, methodBodyStreamRva, mappedFieldDataStreamRva);

            // #Strings, #US, #Guid and #Blob streams:
            _tablesAndHeaps.WriteHeapsTo(builder, _serializedMetadata.StringHeap);
        }
    }

    internal static class MetadataBuilderHelpers
    {
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
