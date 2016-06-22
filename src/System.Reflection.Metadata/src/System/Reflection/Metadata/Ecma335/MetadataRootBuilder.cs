// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;

namespace System.Reflection.Metadata.Ecma335
{
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
            MetadataBuilder.SerializeMetadataHeader(builder, _metadataVersion, _serializedMetadata.Sizes);

            // #~ or #- stream:
            _tablesAndHeaps.SerializeMetadataTables(builder, _serializedMetadata.Sizes, _serializedMetadata.StringMap, methodBodyStreamRva, mappedFieldDataStreamRva);

            // #Strings, #US, #Guid and #Blob streams:
            _tablesAndHeaps.WriteHeapsTo(builder, _serializedMetadata.StringHeap);
        }
    }
}
