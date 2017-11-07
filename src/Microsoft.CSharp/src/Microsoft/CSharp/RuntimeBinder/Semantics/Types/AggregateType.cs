// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    //
    // AggregateType
    //
    // Represents a generic constructed (or instantiated) type. Parent is the AggregateSymbol.
    // ----------------------------------------------------------------------------

    internal sealed class AggregateType : CType
    {
        private AggregateType _baseType;  // This is the result of calling SubstTypeArray on the aggregate's baseClass.
        private TypeArray _ifacesAll;  // This is the result of calling SubstTypeArray on the aggregate's ifacesAll.
        private TypeArray _winrtifacesAll; //This is the list of collection interfaces implemented by a WinRT object.

        public AggregateType(AggregateSymbol parent, TypeArray typeArgsThis, AggregateType outerType)
            : base(TypeKind.TK_AggregateType)
        {
            OuterType = outerType;
            OwningAggregate = parent;
            TypeArray outerTypeArgs;
            if (OuterType != null)
            {
                Debug.Assert(OuterType.TypeArgsThis != null);
                Debug.Assert(OuterType.TypeArgsAll != null);

                outerTypeArgs = OuterType.TypeArgsAll;
            }
            else
            {
                outerTypeArgs = BSYMMGR.EmptyTypeArray();
            }

            Debug.Assert(typeArgsThis != null);
            TypeArgsThis = typeArgsThis;

            Debug.Assert(TypeArgsThis != null);

            // Here we need to check our current type args. If we have an open placeholder,
            // then we need to have all open placeholders, and we want to flush
            // our outer type args so that they're open placeholders. 
            //
            // This is because of the following scenario:
            //
            // class B<T>
            // {
            //     class C<U>
            //     {
            //     }
            //     class D
            //     {
            //         void Foo()
            //         {
            //             Type T = typeof(C<>);
            //         }
            //     }
            // }
            //
            // The outer type will be B<T>, but the inner type will be C<>. However,
            // this will eventually be represented in IL as B<>.C<>. As such, we should
            // keep our data structures clear - if we have one open type argument, then
            // all of them must be open type arguments.
            //
            // Ensure that invariant here.

            TypeArray pCheckedOuterTypeArgs = outerTypeArgs;
            TypeManager pTypeManager = OwningAggregate.GetTypeManager();
            TypeArgsAll = pTypeManager.ConcatenateTypeArrays(pCheckedOuterTypeArgs, TypeArgsThis);
        }

        public bool fConstraintsChecked;    // Have the constraints been checked yet?
        public bool fConstraintError;       // Did the constraints check produce an error?

        // These two flags are used to track hiding within interfaces.
        // Their use and validity is always localized. See e.g. MemberLookup::LookupInInterfaces.
        public bool fAllHidden;             // All members are hidden by a derived interface member.
        public bool fDiffHidden;            // Members other than a specific kind are hidden by a derived interface member or class member.

        public AggregateType OuterType { get; }          // the outer type if this is a nested type

        public AggregateSymbol OwningAggregate { get; }

        public AggregateType GetBaseClass()
        {
            return _baseType ??
                (_baseType = OwningAggregate.GetTypeManager().SubstType(OwningAggregate.GetBaseClass(), TypeArgsAll) as AggregateType);
        }

        public IEnumerable<AggregateType> TypeHierarchy
        {
            get
            {
                if (isInterfaceType())
                {
                    yield return this;
                    foreach (AggregateType iface in GetIfacesAll().Items)
                    {
                        yield return iface;
                    }

                    yield return OwningAggregate.GetTypeManager().ObjectAggregateType;
                }
                else
                {
                    for (AggregateType agg = this; agg != null; agg = agg.GetBaseClass())
                    {
                        yield return agg;
                    }
                }
            }
        }

        public TypeArray TypeArgsThis { get; }

        public TypeArray TypeArgsAll { get; }         // includes args from outer types

        public TypeArray GetIfacesAll() => _ifacesAll
            ?? (_ifacesAll = OwningAggregate
            .GetTypeManager()
            .SubstTypeArray(OwningAggregate.GetIfacesAll(), TypeArgsAll));

        public TypeArray GetWinRTCollectionIfacesAll(SymbolLoader pSymbolLoader)
        {
            if (_winrtifacesAll == null)
            {
                TypeArray ifaces = GetIfacesAll();
                System.Collections.Generic.List<CType> typeList = new System.Collections.Generic.List<CType>();

                for (int i = 0; i < ifaces.Count; i++)
                {
                    AggregateType type = ifaces[i] as AggregateType;
                    Debug.Assert(type.isInterfaceType());

                    if (type.IsCollectionType())
                    {
                        typeList.Add(type);
                    }
                }
                _winrtifacesAll = pSymbolLoader.getBSymmgr().AllocParams(typeList.Count, typeList.ToArray());
            }
            return _winrtifacesAll;
        }

        public TypeArray GetDelegateParameters(SymbolLoader pSymbolLoader)
        {
            Debug.Assert(isDelegateType());
            MethodSymbol invoke = pSymbolLoader.LookupInvokeMeth(OwningAggregate);
            if (invoke == null || !invoke.isInvoke())
            {
                // This can happen if the delegate is internal to another assembly. 
                return null;
            }
            return OwningAggregate.GetTypeManager().SubstTypeArray(invoke.Params, this);
        }

        public CType GetDelegateReturnType(SymbolLoader pSymbolLoader)
        {
            Debug.Assert(isDelegateType());
            MethodSymbol invoke = pSymbolLoader.LookupInvokeMeth(OwningAggregate);
            if (invoke == null || !invoke.isInvoke())
            {
                // This can happen if the delegate is internal to another assembly. 
                return null;
            }
            return OwningAggregate.GetTypeManager().SubstType(invoke.RetType, this);
        }
    }
}

