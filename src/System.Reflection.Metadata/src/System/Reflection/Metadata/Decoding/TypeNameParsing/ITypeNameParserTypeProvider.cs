// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
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
