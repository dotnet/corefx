// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace Microsoft.Internal
{
    internal static class StringComparers
    {
        public static StringComparer ContractName
        {
            get { return StringComparer.Ordinal; }
        }
        
        public static StringComparer MetadataKeyNames
        {
            get { return StringComparer.Ordinal; }
        }
    }
}
