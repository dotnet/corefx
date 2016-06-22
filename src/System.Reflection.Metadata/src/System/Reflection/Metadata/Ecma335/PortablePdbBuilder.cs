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
            MetadataBuilder.SerializeMetadataHeader(builder, MetadataVersionString, _serializedMetadata.Sizes);

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
}
