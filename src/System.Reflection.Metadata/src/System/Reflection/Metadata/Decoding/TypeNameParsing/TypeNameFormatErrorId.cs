// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    internal enum TypeNameFormatErrorId
    {
        IdExpected_EncounteredOnlyWhiteSpace,
        IdExpected_EncounteredDelimiter,
        IdExpected_EncounteredEndOfString,
        DelimiterExpected,
        DelimiterExpected_EncounteredEndOfString,
        EscapedDelimiterExpected,
        EscapedDelimiterExpected_EncounteredEndOfString,
        EndOfStringExpected_EncounteredExtraCharacters,
        DuplicateAssemblyComponent,
    }
}
