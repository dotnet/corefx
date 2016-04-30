// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // Alias ID's are indices into BitSets.
    // 0 is reserved for the global namespace alias.
    // 1 is reserved for this assembly.
    // Start assigning at kaidStartAssigning.
    internal enum KAID
    {
        kaidNil = -1,

        kaidGlobal = 0,
        kaidErrorAssem,  // NOTE: !CSEE only
        kaidThisAssembly,
        kaidUnresolved,
        kaidStartAssigning,

        // Module id's are in their own range.
        kaidMinModule = 0x10000000,
    }

    /*
     * Define the different access levels that symbols can have.
     */
    internal enum ACCESS
    {
        ACC_UNKNOWN,     // Not yet determined.
        ACC_PRIVATE,
        ACC_INTERNAL,
        ACC_PROTECTED,
        ACC_INTERNALPROTECTED,   // internal OR protected
        ACC_PUBLIC
    }

    // The kinds of aggregates.
    internal enum AggKindEnum
    {
        Unknown,

        Class,
        Delegate,
        Interface,
        Struct,
        Enum,

        Lim
    }

    // The pseudo-methods uses for accessing arrays (except in
    // the optimized 1-d case.
    internal enum ARRAYMETHOD
    {
        ARRAYMETH_LOAD,
        ARRAYMETH_LOADADDR,
        ARRAYMETH_STORE,
        ARRAYMETH_CTOR,
        ARRAYMETH_GETAT,   // Keep these in this order!!!

        ARRAYMETH_COUNT
    };

    /////////////////////////////////////////////////////////////////////////////////

    // Special constraints.
    internal enum SpecCons
    {
        None = 0x00,

        New = 0x01,
        Ref = 0x02,
        Val = 0x04
    }

    // ----------------------------------------------------------------------------
    //
    // Symbol - the base symbol.
    //
    // ----------------------------------------------------------------------------

    internal class Symbol
    {
        private SymbolKind _kind;     // the symbol kind
        private bool _isBogus;     // can't be used in our language -- unsupported type(s)
        private bool _checkedBogus; // Have we checked a method args/return for bogus types
        private ACCESS _access;    // access level

        // If this is true, then we had an error the first time so do not give an error the second time.

        public Name name;         // name of the symbol
        public ParentSymbol parent;  // parent of the symbol
        public Symbol nextChild;     // next child of this parent
        public Symbol nextSameName;  // next child of this parent with same name.


        public ACCESS GetAccess()
        {
            Debug.Assert(_access != ACCESS.ACC_UNKNOWN);
            return _access;
        }

        public void SetAccess(ACCESS access)
        {
            _access = access;
        }

        public SymbolKind getKind()
        {
            return _kind;
        }

        public void setKind(SymbolKind kind)
        {
            _kind = kind;
        }

        public symbmask_t mask()
        {
            return (symbmask_t)(1 << (int)_kind);
        }

        public bool checkBogus()
        {
            Debug.Assert(_checkedBogus);
            return _isBogus;
        } // if this Debug.Assert fires then call COMPILER_BASE::CheckBogus() instead

        public bool getBogus()
        {
            return _isBogus;
        }

        public bool hasBogus()
        {
            return _checkedBogus;
        }

        public void setBogus(bool isBogus)
        {
            _isBogus = isBogus;
            _checkedBogus = true;
        }

        public void initBogus()
        {
            _isBogus = false;
            _checkedBogus = false;
        }

        public bool computeCurrentBogusState()
        {
            if (this.hasBogus())
            {
                return this.checkBogus();
            }

            bool fBogus = false;

            switch (this.getKind())
            {
                case SymbolKind.SK_PropertySymbol:
                case SymbolKind.SK_MethodSymbol:
                    {
                        MethodOrPropertySymbol meth = this.AsMethodOrPropertySymbol();

                        if (meth.RetType != null)
                        {
                            fBogus = meth.RetType.computeCurrentBogusState();
                        }
                        if (meth.Params != null)
                        {
                            for (int i = 0; !fBogus && i < meth.Params.Size; i++)
                            {
                                fBogus |= meth.Params.Item(i).computeCurrentBogusState();
                            }
                        }
                    }
                    break;

                /*
                case SymbolKind.SK_ParameterModifierType:
                case SymbolKind.SK_OptionalModifierType:
                case SymbolKind.SK_PointerType:
                case SymbolKind.SK_ArrayType:
                case SymbolKind.SK_NullableType:
                case SymbolKind.SK_PinnedType:
                    if (this.AsType().GetBaseOrParameterOrElementType() != null)
                    {
                        fBogus = this.AsType().GetBaseOrParameterOrElementType().computeCurrentBogusState();
                    }
                    break;
                    */

                case SymbolKind.SK_EventSymbol:
                    if (this.AsEventSymbol().type != null)
                    {
                        fBogus = this.AsEventSymbol().type.computeCurrentBogusState();
                    }
                    break;

                case SymbolKind.SK_FieldSymbol:
                    if (this.AsFieldSymbol().GetType() != null)
                    {
                        fBogus = this.AsFieldSymbol().GetType().computeCurrentBogusState();
                    }
                    break;

                /*
                case SymbolKind.SK_ErrorType:
                    this.setBogus(false);
                    break;

                case SymbolKind.SK_AggregateType:
                    fBogus = this.AsAggregateType().getAggregate().computeCurrentBogusState();
                    for (int i = 0; !fBogus && i < this.AsAggregateType().GetTypeArgsAll().size; i++)
                    {
                        fBogus |= this.AsAggregateType().GetTypeArgsAll().Item(i).computeCurrentBogusState();
                    }
                    break;
                 */

                case SymbolKind.SK_TypeParameterSymbol:
                /*
            case SymbolKind.SK_TypeParameterType:
            case SymbolKind.SK_VoidType:
            case SymbolKind.SK_NullType:
            case SymbolKind.SK_OpenTypePlaceholderType:
            case SymbolKind.SK_ArgumentListType:
            case SymbolKind.SK_NaturalIntegerType:
                 */
                case SymbolKind.SK_LocalVariableSymbol:
                    this.setBogus(false);
                    break;

                case SymbolKind.SK_AggregateSymbol:
                    fBogus = this.hasBogus() && this.checkBogus();
                    break;

                case SymbolKind.SK_Scope:
                case SymbolKind.SK_LambdaScope:
                case SymbolKind.SK_NamespaceSymbol:
                case SymbolKind.SK_NamespaceDeclaration:
                default:
                    Debug.Assert(false, "CheckBogus with invalid Symbol kind");
                    this.setBogus(false);
                    break;
            }

            if (fBogus)
            {
                // Only set this if at least 1 declared thing is bogus
                this.setBogus(fBogus);
            }

            return this.hasBogus() && this.checkBogus();
        }

        public bool IsNamespaceSymbol() { return _kind == SymbolKind.SK_NamespaceSymbol; }
        public bool IsNamespaceDeclaration() { return _kind == SymbolKind.SK_NamespaceDeclaration; }
        public bool IsAggregateSymbol() { return _kind == SymbolKind.SK_AggregateSymbol; }
        public bool IsAggregateDeclaration() { return _kind == SymbolKind.SK_AggregateDeclaration; }
        public bool IsFieldSymbol() { return _kind == SymbolKind.SK_FieldSymbol; }
        public bool IsLocalVariableSymbol() { return _kind == SymbolKind.SK_LocalVariableSymbol; }
        public bool IsMethodSymbol() { return _kind == SymbolKind.SK_MethodSymbol; }
        public bool IsPropertySymbol() { return _kind == SymbolKind.SK_PropertySymbol; }
        public bool IsTypeParameterSymbol() { return _kind == SymbolKind.SK_TypeParameterSymbol; }
        public bool IsEventSymbol() { return _kind == SymbolKind.SK_EventSymbol; }

        public bool IsMethodOrPropertySymbol()
        {
            return this.IsMethodSymbol() || this.IsPropertySymbol();
        }

        public bool IsFMETHSYM()
        {
            return this.IsMethodSymbol();
        }

        public CType getType()
        {
            CType type = null;
            if (IsMethodOrPropertySymbol())
            {
                type = this.AsMethodOrPropertySymbol().RetType;
            }
            else if (IsFieldSymbol())
            {
                type = this.AsFieldSymbol().GetType();
            }
            else if (IsEventSymbol())
            {
                type = this.AsEventSymbol().type;
            }
            return type;
        }

        public bool isStatic
        {
            get
            {
                bool fStatic = false;
                if (IsFieldSymbol())
                {
                    fStatic = this.AsFieldSymbol().isStatic;
                }
                else if (IsEventSymbol())
                {
                    fStatic = this.AsEventSymbol().isStatic;
                }
                else if (IsMethodOrPropertySymbol())
                {
                    fStatic = this.AsMethodOrPropertySymbol().isStatic;
                }
                else if (IsAggregateSymbol())
                {
                    fStatic = true;
                }
                return fStatic;
            }
        }

        public Assembly GetAssembly()
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                case SymbolKind.SK_PropertySymbol:
                case SymbolKind.SK_FieldSymbol:
                case SymbolKind.SK_EventSymbol:
                case SymbolKind.SK_TypeParameterSymbol:
                    return parent.AsAggregateSymbol().AssociatedAssembly;

                case SymbolKind.SK_AggregateDeclaration:
                    return this.AsAggregateDeclaration().GetAssembly();
                case SymbolKind.SK_AggregateSymbol:
                    return this.AsAggregateSymbol().AssociatedAssembly;
                case SymbolKind.SK_NamespaceDeclaration:
                case SymbolKind.SK_NamespaceSymbol:
                case SymbolKind.SK_AssemblyQualifiedNamespaceSymbol:
                default:
                    // Should never call this with any other kind.
                    Debug.Assert(false, "GetAssemblyID called on bad sym kind");
                    return null;
            }
        }

        /*
         * returns the assembly id for the declaration of this symbol
         */
        public bool InternalsVisibleTo(Assembly assembly)
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                case SymbolKind.SK_PropertySymbol:
                case SymbolKind.SK_FieldSymbol:
                case SymbolKind.SK_EventSymbol:
                case SymbolKind.SK_TypeParameterSymbol:
                    return parent.AsAggregateSymbol().InternalsVisibleTo(assembly);

                case SymbolKind.SK_AggregateDeclaration:
                    return this.AsAggregateDeclaration().Agg().InternalsVisibleTo(assembly);
                case SymbolKind.SK_AggregateSymbol:
                    return this.AsAggregateSymbol().InternalsVisibleTo(assembly);
                case SymbolKind.SK_NamespaceDeclaration:
                case SymbolKind.SK_ExternalAliasDefinitionSymbol:
                case SymbolKind.SK_NamespaceSymbol:
                case SymbolKind.SK_AssemblyQualifiedNamespaceSymbol:
                default:
                    // Should never call this with any other kind.
                    Debug.Assert(false, "InternalsVisibleTo called on bad sym kind");
                    return false;
            }
        }

        public bool SameAssemOrFriend(Symbol sym)
        {
            Assembly assem = GetAssembly();
            return assem == sym.GetAssembly() || sym.InternalsVisibleTo(assem);
        }

        /*
         * returns the inputfile where a symbol was declared.
         *
         * returns null for namespaces because they can be declared
         * in multiple files.
         */
        public InputFile getInputFile()
        {
            switch (_kind)
            {
                case SymbolKind.SK_NamespaceSymbol:
                case SymbolKind.SK_AssemblyQualifiedNamespaceSymbol:
                    // namespaces don't have input files
                    // call with a NamespaceDeclaration instead
                    Debug.Assert(false);
                    return null;

                case SymbolKind.SK_NamespaceDeclaration:
                    return null;

                case SymbolKind.SK_AggregateSymbol:
                    {
#if !CSEE
                        AggregateSymbol AggregateSymbol = this.AsAggregateSymbol();
                        if (!AggregateSymbol.IsSource())
                            return AggregateSymbol.DeclOnly().getInputFile();

                        // Because an AggregateSymbol that isn't metadata can be defined across multiple
                        // files, getInputFile isn't a reasonable operation.
                        Debug.Assert(false);
                        return null;
#endif
                    }

                /*
                case SK_AggregateType:
                    return ((Symbol)this.AsAggregateType().getAggregate()).getInputFile();
                 */

                case SymbolKind.SK_AggregateDeclaration:
                    return this.AsAggregateDeclaration().getInputFile();

                /*
                case SK_TypeParameterType:
                    if (this.AsTypeParameterType().GetOwningSymbol().IsAggregateSymbol())
                    {
                        ASSERT(0);
                        return null;
                    }
                    else
                    {
                        ASSERT(this.AsTypeParameterType().GetOwningSymbol().IsMethodSymbol());
                        return AsTypeParameterType().GetOwningSymbol().AsMethodSymbol().getInputFile();
                    }
                 */
                case SymbolKind.SK_TypeParameterSymbol:
                    if (this.parent.IsAggregateSymbol())
                    {
                        // Because an AggregateSymbol that isn't metadata can be defined across multiple
                        // files, getInputFile isn't a reasonable operation.
                        Debug.Assert(false);
                        return null;
                    }
                    else if (this.parent.IsMethodSymbol())
                        return this.parent.AsMethodSymbol().getInputFile();
                    Debug.Assert(false);
                    break;

                case SymbolKind.SK_FieldSymbol:
                    return this.AsFieldSymbol().containingDeclaration().getInputFile();
                case SymbolKind.SK_MethodSymbol:
                    return this.AsMethodSymbol().containingDeclaration().getInputFile();
                case SymbolKind.SK_PropertySymbol:
                    return this.AsPropertySymbol().containingDeclaration().getInputFile();
                case SymbolKind.SK_EventSymbol:
                    return this.AsEventSymbol().containingDeclaration().getInputFile();

                /*
                case SK_PointerType:
                case SK_NullableType:
                case SK_ArrayType:
                case SK_PinnedType:
                case SK_ParameterModifierType:
                case SK_OptionalModifierType:
                    return AsType().GetBaseOrParameterOrElementType().getInputFile();
                 */

                case SymbolKind.SK_GlobalAttributeDeclaration:
                    return parent.getInputFile();

                /*
                case SK_NullType:
                case SK_VoidType:
                    return null;
                 */

                default:
                    Debug.Assert(false);
                    break;
            }

            return null;
        }


        /* Returns if the symbol is virtual. */
        public bool IsVirtual()
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                    return this.AsMethodSymbol().isVirtual;
                case SymbolKind.SK_EventSymbol:
                    return this.AsEventSymbol().methAdd != null && this.AsEventSymbol().methAdd.isVirtual;
                case SymbolKind.SK_PropertySymbol:
                    return (this.AsPropertySymbol().methGet != null && this.AsPropertySymbol().methGet.isVirtual) ||
                           (this.AsPropertySymbol().methSet != null && this.AsPropertySymbol().methSet.isVirtual);
                default:
                    return false;
            }
        }

        public bool IsOverride()
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                case SymbolKind.SK_PropertySymbol:
                    return this.AsMethodOrPropertySymbol().isOverride;
                case SymbolKind.SK_EventSymbol:
                    return this.AsEventSymbol().isOverride;
                default:
                    return false;
            }
        }

        public bool IsHideByName()
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                case SymbolKind.SK_PropertySymbol:
                    return this.AsMethodOrPropertySymbol().isHideByName;
                case SymbolKind.SK_EventSymbol:
                    return this.AsEventSymbol().methAdd != null && this.AsEventSymbol().methAdd.isHideByName;
                default:
                    return true;
            }
        }

        // Returns the virtual that this sym overrides (if IsOverride() is true), null otherwise.
        public Symbol SymBaseVirtual()
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                case SymbolKind.SK_PropertySymbol:
                    return this.AsMethodOrPropertySymbol().swtSlot.Sym;
                case SymbolKind.SK_EventSymbol:
                default:
                    return null;
            }
        }

        /*
         * returns true if this symbol is a normal symbol visible to the user
         */
        public bool isUserCallable()
        {
            switch (_kind)
            {
                case SymbolKind.SK_MethodSymbol:
                    return this.AsMethodSymbol().isUserCallable();
                default:
                    break;
            }

            return true;
        }
    }

    /*
     * We have member functions here to do casts that, in DEBUG, check the 
     * symbol kind to make sure it is right. For example, the casting method
     * for METHODSYM is called "asMETHODSYM". In retail builds, these 
     * methods optimize away to nothing.
     */

    internal static class SymbolExtensions
    {
        public static IEnumerable<Symbol> Children(this ParentSymbol symbol)
        {
            if (symbol == null)
                yield break;
            Symbol current = symbol.firstChild;
            while (current != null)
            {
                yield return current;
                current = current.nextChild;
            }
        }

        internal static MethodSymbol AsFMETHSYM(this Symbol symbol) { return symbol as MethodSymbol; }

        internal static NamespaceOrAggregateSymbol AsNamespaceOrAggregateSymbol(this Symbol symbol) { return symbol as NamespaceOrAggregateSymbol; }
        internal static NamespaceSymbol AsNamespaceSymbol(this Symbol symbol) { return symbol as NamespaceSymbol; }
        internal static AssemblyQualifiedNamespaceSymbol AsAssemblyQualifiedNamespaceSymbol(this Symbol symbol) { return symbol as AssemblyQualifiedNamespaceSymbol; }
        internal static NamespaceDeclaration AsNamespaceDeclaration(this Symbol symbol) { return symbol as NamespaceDeclaration; }
        internal static AggregateSymbol AsAggregateSymbol(this Symbol symbol) { return symbol as AggregateSymbol; }
        internal static AggregateDeclaration AsAggregateDeclaration(this Symbol symbol) { return symbol as AggregateDeclaration; }
        internal static FieldSymbol AsFieldSymbol(this Symbol symbol) { return symbol as FieldSymbol; }
        internal static LocalVariableSymbol AsLocalVariableSymbol(this Symbol symbol) { return symbol as LocalVariableSymbol; }
        internal static MethodSymbol AsMethodSymbol(this Symbol symbol) { return symbol as MethodSymbol; }
        internal static PropertySymbol AsPropertySymbol(this Symbol symbol) { return symbol as PropertySymbol; }
        internal static MethodOrPropertySymbol AsMethodOrPropertySymbol(this Symbol symbol) { return symbol as MethodOrPropertySymbol; }
        internal static Scope AsScope(this Symbol symbol) { return symbol as Scope; }
        internal static TypeParameterSymbol AsTypeParameterSymbol(this Symbol symbol) { return symbol as TypeParameterSymbol; }
        internal static EventSymbol AsEventSymbol(this Symbol symbol) { return symbol as EventSymbol; }
    }
}
