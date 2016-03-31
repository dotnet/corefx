// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata.Decoding.Tests
{
    // Overrides the disassembling provider to use the hex representations of any tokens in the signature.
    // Used for test signature blobs that we didn't get from an actual metadata reader.
    internal class OpaqueTokenTypeProvider : DisassemblingTypeProvider
    {
        public override string GetTypeFromHandle(MetadataReader reader, EntityHandle handle)
        {
            return MetadataTokens.GetToken(handle).ToString("X");
        }

        public override string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, SignatureTypeHandleCode code)
        {
            return GetTypeFromHandle(reader, handle);
        }

        public override string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, SignatureTypeHandleCode code)
        {
            return GetTypeFromHandle(reader, handle);
        }
    }
}
