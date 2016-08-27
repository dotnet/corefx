// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // TODO: This looks like it could derive from int rather than long.
    [Flags]
    internal enum SymbolMask : long
    {
        NamespaceSymbol = 1 << (int)SymbolKind.NamespaceSymbol,
        NamespaceDeclaration = 1 << (int)SymbolKind.NamespaceDeclaration,
        AssemblyQualifiedNamespaceSymbol = 1 << (int)SymbolKind.AssemblyQualifiedNamespaceSymbol,
        AggregateSymbol = 1 << (int)SymbolKind.AggregateSymbol,
        AggregateDeclaration = 1 << (int)SymbolKind.AggregateDeclaration,
        TypeParameterSymbol = 1 << (int)SymbolKind.TypeParameterSymbol,
        FieldSymbol = 1 << (int)SymbolKind.FieldSymbol,
        LocalVariableSymbol = 1 << (int)SymbolKind.LocalVariableSymbol,
        MethodSymbol = 1 << (int)SymbolKind.MethodSymbol,
        PropertySymbol = 1 << (int)SymbolKind.PropertySymbol,
        EventSymbol = 1 << (int)SymbolKind.EventSymbol,
        TransparentIdentifierMemberSymbol = 1 << (int)SymbolKind.TransparentIdentifierMemberSymbol,
        Scope = 1 << (int)SymbolKind.Scope,
        CachedNameSymbol = 1 << (int)SymbolKind.CachedNameSymbol,
        LabelSymbol = 1 << (int)SymbolKind.LabelSymbol,
        GlobalAttributeDeclaration = 1 << (int)SymbolKind.GlobalAttributeDeclaration,
        LambdaScope = 1 << (int)SymbolKind.LambdaScope,
        ALL = ~0,
        LOOKUPMASK = (AssemblyQualifiedNamespaceSymbol | FieldSymbol | LocalVariableSymbol | MethodSymbol | PropertySymbol)
    }
}
