// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace System.Reflection.Metadata.Ecma335
{
    ///// <summary>
    ///// Composes all metadata streams together into a metadata root.
    ///// </summary>
    //public abstract class MetadataStreamsBuilder
    //{
    //    private sealed class PortablePdb : MetadataStreamsBuilder
    //    {

    //    }

    //    private sealed class TypeSystem : MetadataStreamsBuilder
    //    {

    //    }

    //    public static MetadataStreamsBuilder ForPortablePdb(
    //        MetadataBuilder builder,
    //        ImmutableArray<int> typeSystemRowCounts,
    //        MethodDefinitionHandle entryPoint,
    //        Func<IEnumerable<Blob>, BlobContentId> deterministicIdProvider = null)
    //    {
    //        return new MetadataStreamsBuilder();
    //    }

    //    public static MetadataStreamsBuilder ForPortableExecutable(
    //        MetadataBuilder builder,
    //        string metadataVersion)
    //    {

    //    }

    //    public static MetadataStreamsBuilder ForEditAndContinueDelta(
    //        MetadataBuilder builder,
    //        string metadataVersion)
    //    {

    //    }

    //}
}
