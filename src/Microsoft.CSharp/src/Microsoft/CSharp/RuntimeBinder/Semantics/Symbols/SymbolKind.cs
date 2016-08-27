// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal enum SymbolKind
    {
        NamespaceSymbol,
        NamespaceDeclaration,
        AssemblyQualifiedNamespaceSymbol,
        AggregateSymbol,
        AggregateDeclaration,
        TypeParameterSymbol,
        FieldSymbol,
        LocalVariableSymbol,
        MethodSymbol,
        PropertySymbol,
        EventSymbol,
        TransparentIdentifierMemberSymbol,
        AliasSymbol,
        ExternalAliasDefinitionSymbol,
        Scope,
        CachedNameSymbol,
        LabelSymbol,
        GlobalAttributeDeclaration,
        LambdaScope,
        UnresolvedAggregateSymbol,
        InterfaceImplementationMethodSymbol,
        IndexerSymbol,
        ParentSymbol,
        IteratorFinallyMethodSymbol,
        LIM
    }

    // The kinds of Synthesized aggregates
    internal enum SynthAggKind
    {
        NotSynthesized,

        AnonymousMethodDisplayClass,
        IteratorClass,
        FixedBufferStruct,

        Lim
    }
}
