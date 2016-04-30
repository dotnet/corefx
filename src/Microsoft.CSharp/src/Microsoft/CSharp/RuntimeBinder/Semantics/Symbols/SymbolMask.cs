// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    [Flags]
    internal enum symbmask_t : long
    {
        MASK_NamespaceSymbol = 1 << (int)SymbolKind.NamespaceSymbol,
        MASK_NamespaceDeclaration = 1 << (int)SymbolKind.NamespaceDeclaration,
        MASK_AssemblyQualifiedNamespaceSymbol = 1 << (int)SymbolKind.AssemblyQualifiedNamespaceSymbol,
        MASK_AggregateSymbol = 1 << (int)SymbolKind.AggregateSymbol,
        MASK_AggregateDeclaration = 1 << (int)SymbolKind.AggregateDeclaration,
        MASK_TypeParameterSymbol = 1 << (int)SymbolKind.TypeParameterSymbol,
        MASK_FieldSymbol = 1 << (int)SymbolKind.FieldSymbol,
        MASK_LocalVariableSymbol = 1 << (int)SymbolKind.LocalVariableSymbol,
        MASK_MethodSymbol = 1 << (int)SymbolKind.MethodSymbol,
        MASK_PropertySymbol = 1 << (int)SymbolKind.PropertySymbol,
        MASK_EventSymbol = 1 << (int)SymbolKind.EventSymbol,
        MASK_TransparentIdentifierMemberSymbol = 1 << (int)SymbolKind.TransparentIdentifierMemberSymbol,
        MASK_Scope = 1 << (int)SymbolKind.Scope,
        MASK_CachedNameSymbol = 1 << (int)SymbolKind.CachedNameSymbol,
        MASK_LabelSymbol = 1 << (int)SymbolKind.LabelSymbol,
        MASK_GlobalAttributeDeclaration = 1 << (int)SymbolKind.GlobalAttributeDeclaration,
        MASK_LambdaScope = 1 << (int)SymbolKind.LambdaScope,
        MASK_ALL = ~0,
        LOOKUPMASK = (MASK_AssemblyQualifiedNamespaceSymbol | MASK_FieldSymbol | MASK_LocalVariableSymbol | MASK_MethodSymbol | MASK_PropertySymbol)
    }
}
