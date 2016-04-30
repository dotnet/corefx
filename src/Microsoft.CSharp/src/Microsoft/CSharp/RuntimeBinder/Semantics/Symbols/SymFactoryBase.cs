// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // This class is the base class for the different symbol factories: the
    // SymFactory, TypeFactory and MiscSymFactory. This provides a common
    // way of creating syms that all three classes use.

    internal class SymFactoryBase
    {
        // Members.
        protected SYMTBL m_pSymTable;
        protected Name m_pMissingNameNode;
        protected Name m_pMissingNameSym;

        protected Symbol newBasicSym(
            SymbolKind kind,
            Name name,
            ParentSymbol parent)
        {
            // The parser creates names with PN_MISSING when attempting to recover from errors
            // To prevent spurious errors, we create SYMs with a different name (PN_MISSINGSYM)
            // so that they are never found when doing lookup.
            if (name == m_pMissingNameNode)
            {
                name = m_pMissingNameSym;
            }

            Symbol sym;
            switch (kind)
            {
                case SymbolKind.NamespaceSymbol:
                    sym = new NamespaceSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.NamespaceDeclaration:
                    sym = new NamespaceDeclaration();
                    sym.name = name;
                    break;
                case SymbolKind.AssemblyQualifiedNamespaceSymbol:
                    sym = new AssemblyQualifiedNamespaceSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.AggregateSymbol:
                    sym = new AggregateSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.AggregateDeclaration:
                    sym = new AggregateDeclaration();
                    sym.name = name;
                    break;
                case SymbolKind.TypeParameterSymbol:
                    sym = new TypeParameterSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.FieldSymbol:
                    sym = new FieldSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.LocalVariableSymbol:
                    sym = new LocalVariableSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.MethodSymbol:
                    sym = new MethodSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.PropertySymbol:
                    sym = new PropertySymbol();
                    sym.name = name;
                    break;
                case SymbolKind.EventSymbol:
                    sym = new EventSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.TransparentIdentifierMemberSymbol:
                    sym = new TransparentIdentifierMemberSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.Scope:
                    sym = new Scope();
                    sym.name = name;
                    break;
                case SymbolKind.LabelSymbol:
                    sym = new LabelSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.GlobalAttributeDeclaration:
                    sym = new GlobalAttributeDeclaration();
                    sym.name = name;
                    break;
                case SymbolKind.UnresolvedAggregateSymbol:
                    sym = new UnresolvedAggregateSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.InterfaceImplementationMethodSymbol:
                    sym = new InterfaceImplementationMethodSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.IndexerSymbol:
                    sym = new IndexerSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.ParentSymbol:
                    sym = new ParentSymbol();
                    sym.name = name;
                    break;
                case SymbolKind.IteratorFinallyMethodSymbol:
                    sym = new IteratorFinallyMethodSymbol();
                    sym.name = name;
                    break;
                default:
                    throw Error.InternalCompilerError();
            }

            sym.setKind(kind);

            if (parent != null)
            {
                // Set the parent element of the child symbol.
                parent.AddToChildList(sym);
                m_pSymTable.InsertChild(parent, sym);
            }

            return (sym);
        }

        // This class should never be created on its own.
        protected SymFactoryBase(SYMTBL symtable, NameManager namemgr)
        {
            m_pSymTable = symtable;

            if (namemgr != null)
            {
                m_pMissingNameNode = namemgr.GetPredefName(PredefinedName.PN_MISSING);
                m_pMissingNameSym = namemgr.GetPredefName(PredefinedName.PN_MISSINGSYM);
            }
        }
    }
}
