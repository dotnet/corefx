// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    [Flags]
    internal enum symbmask_t : long
    {
        MASK_NamespaceSymbol = 1 << (int)SymbolKind.SK_NamespaceSymbol,
        MASK_NamespaceDeclaration = 1 << (int)SymbolKind.SK_NamespaceDeclaration,
        MASK_AssemblyQualifiedNamespaceSymbol = 1 << (int)SymbolKind.SK_AssemblyQualifiedNamespaceSymbol,
        MASK_AggregateSymbol = 1 << (int)SymbolKind.SK_AggregateSymbol,
        MASK_AggregateDeclaration = 1 << (int)SymbolKind.SK_AggregateDeclaration,
        MASK_TypeParameterSymbol = 1 << (int)SymbolKind.SK_TypeParameterSymbol,
        MASK_FieldSymbol = 1 << (int)SymbolKind.SK_FieldSymbol,
        MASK_LocalVariableSymbol = 1 << (int)SymbolKind.SK_LocalVariableSymbol,
        MASK_MethodSymbol = 1 << (int)SymbolKind.SK_MethodSymbol,
        MASK_PropertySymbol = 1 << (int)SymbolKind.SK_PropertySymbol,
        MASK_EventSymbol = 1 << (int)SymbolKind.SK_EventSymbol,
        MASK_TransparentIdentifierMemberSymbol = 1 << (int)SymbolKind.SK_TransparentIdentifierMemberSymbol,
        MASK_Scope = 1 << (int)SymbolKind.SK_Scope,
        MASK_CachedNameSymbol = 1 << (int)SymbolKind.SK_CachedNameSymbol,
        MASK_LabelSymbol = 1 << (int)SymbolKind.SK_LabelSymbol,
        MASK_GlobalAttributeDeclaration = 1 << (int)SymbolKind.SK_GlobalAttributeDeclaration,
        MASK_LambdaScope = 1 << (int)SymbolKind.SK_LambdaScope,
        MASK_ALL = ~0,
        LOOKUPMASK = (MASK_AssemblyQualifiedNamespaceSymbol | MASK_FieldSymbol | MASK_LocalVariableSymbol | MASK_MethodSymbol | MASK_PropertySymbol)
    }
}
