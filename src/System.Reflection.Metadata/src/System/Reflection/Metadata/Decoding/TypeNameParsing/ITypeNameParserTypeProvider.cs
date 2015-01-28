// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Immutable;

namespace System.Reflection.Metadata.Decoding
{
    public interface ITypeNameParserTypeProvider<TType> : ITypeProvider<TType>
    {
        TType GetTypeFromName(AssemblyNameComponents? name, string declaringTypeFullName, ImmutableArray<string> nestedTypeNames);
    }
}
