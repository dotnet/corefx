// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CSharp.RuntimeBinder.Semantics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder
{
    internal sealed class SymbolTable
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Members
        private readonly HashSet<Type> _typesWithConversionsLoaded = new HashSet<Type>();
        private readonly HashSet<NameHashKey> _namesLoadedForEachType = new HashSet<NameHashKey>();

        // Members from the managed binder.
        private readonly SYMTBL _symbolTable;
        private readonly SymFactory _symFactory;
        private readonly NameManager _nameManager;
        private readonly TypeManager _typeManager;
        private readonly BSYMMGR _bsymmgr;
        private readonly CSemanticChecker _semanticChecker;

        private NamespaceSymbol _rootNamespace;

        /////////////////////////////////////////////////////////////////////////////////

        private sealed class NameHashKey : IEquatable<NameHashKey>
        {
            internal readonly Type type;
            internal readonly string name;

            public NameHashKey(Type type, string name)
            {
                this.type = type;
                this.name = name;
            }

            public bool Equals(NameHashKey other) => other != null && type.Equals(other.type) && name.Equals(other.name);

#if  DEBUG 
            [ExcludeFromCodeCoverage] // Typed overload should always be the method called.
#endif
            public override bool Equals(object obj)
            {
                Debug.Fail("Sub-optimal overload called. Check if this can be avoided.");
                return Equals(obj as NameHashKey);
            }

            public override int GetHashCode()
            {
                return type.GetHashCode() ^ name.GetHashCode();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal SymbolTable(
            SYMTBL symTable,
            SymFactory symFactory,
            NameManager nameManager,
            TypeManager typeManager,
            BSYMMGR bsymmgr,
            CSemanticChecker semanticChecker)
        {
            _symbolTable = symTable;
            _symFactory = symFactory;
            _nameManager = nameManager;
            _typeManager = typeManager;
            _bsymmgr = bsymmgr;
            _semanticChecker = semanticChecker;
            _rootNamespace = _bsymmgr.GetRootNS();

            // Now populate object.
            LoadSymbolsFromType(typeof(object));
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal void PopulateSymbolTableWithName(
            string name,
            IEnumerable<Type> typeArguments,
            Type callingType)
        {
            // The first argument is the object that we're calling off of.
            if (callingType.IsGenericType)
            {
                callingType = callingType.GetGenericTypeDefinition();
            }

            if (name == SpecialNames.Indexer)
            {
                // If we don't find an indexer name for this type, use SpecialNames.Indexer as a key on the
                // empty results we'll get, so that those empty results gets cached.
                name = callingType.GetIndexerName() ?? SpecialNames.Indexer;
            }

            NameHashKey key = new NameHashKey(callingType, name);

            // If we've already populated this name/type pair, then just leave.
            if (_namesLoadedForEachType.Contains(key))
            {
                return;
            }

            // Add the names.
            AddNamesOnType(key);

            // Take each type argument and load its conversions into the symbol table.
            if (typeArguments != null)
            {
                foreach (Type o in typeArguments)
                {
                    AddConversionsForType(o);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal SymWithType LookupMember(
            string name,
            Expr callingObject,
            ParentSymbol context,
            int arity,
            MemberLookup mem,
            bool allowSpecialNames,
            bool requireInvocable)
        {
            CType type = callingObject.Type;

            if (type is ArrayType)
            {
                type = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_ARRAY);
            }
            if (type is NullableType nub)
            {
                type = nub.GetAts();
            }

            if (!mem.Lookup(
                _semanticChecker,
                type,
                callingObject,
                context,
                GetName(name),
                arity,
                MemLookFlags.TypeVarsAllowed |
                    (allowSpecialNames ? 0 : MemLookFlags.UserCallable) |
                    (name == SpecialNames.Indexer ? MemLookFlags.Indexer : 0) |
                    (name == SpecialNames.Constructor ? MemLookFlags.Ctor : 0) |
                    (requireInvocable ? MemLookFlags.MustBeInvocable : 0)))
            {
                return null;
            }
            return mem.SwtFirst();
        }

        private void AddParameterConversions(MethodBase method)
        {
            foreach (ParameterInfo param in method.GetParameters())
            {
                AddConversionsForType(param.ParameterType);
            }
        }

        #region InheritanceHierarchy
        private void AddNamesOnType(NameHashKey key)
        {
            Debug.Assert(!_namesLoadedForEachType.Contains(key));

            // We need to declare all of its inheritance hierarchy.
            List<Type> inheritance = CreateInheritanceHierarchyList(key.type);

            // Now add every method as it appears in the inheritance hierarchy.
            AddNamesInInheritanceHierarchy(key.name, inheritance);
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void AddNamesInInheritanceHierarchy(string name, List<Type> inheritance)
        {
            for (int i = inheritance.Count - 1; i >= 0; --i)
            {
                Type type = inheritance[i];
                if (type.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition();
                }

                if (!_namesLoadedForEachType.Add(new NameHashKey(type, name)))
                {
                    continue;
                }

                // Now loop over all methods and add them.
                IEnumerator<MemberInfo> memberEn = type
                    .GetMembers(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(member => member.DeclaringType == type && member.Name == name).GetEnumerator();
                if (memberEn.MoveNext())
                {
                    List<EventInfo> events = null;
                    CType cType = GetCTypeFromType(type);
                    if (!(cType is AggregateType aggType))
                        continue;
                    AggregateSymbol aggregate = aggType.getAggregate();
                    FieldSymbol addedField = null;

                    // We need to add fields before the actual events, so do the first iteration
                    // excluding events.
                    do
                    {
                        MemberInfo member = memberEn.Current;
                        if (member is MethodInfo method)
                        {
                            MethodKindEnum kind;
                            switch (member.Name)
                            {
                                case SpecialNames.Invoke:
                                    kind = MethodKindEnum.Invoke;
                                    break;

                                case SpecialNames.ImplicitConversion:
                                    kind = MethodKindEnum.ImplicitConv;
                                    break;

                                case SpecialNames.ExplicitConversion:
                                    kind = MethodKindEnum.ExplicitConv;
                                    break;

                                default:
                                    kind = MethodKindEnum.Actual;
                                    break;
                            }

                            AddMethodToSymbolTable(member, aggregate, kind);
                            AddParameterConversions(method);
                        }
                        else if (member is ConstructorInfo ctor)
                        {
                            AddMethodToSymbolTable(member, aggregate, MethodKindEnum.Constructor);
                            AddParameterConversions(ctor);
                        }
                        else if (member is PropertyInfo prop)
                        {
                            AddPropertyToSymbolTable(prop, aggregate);
                        }
                        else if (member is FieldInfo field)
                        {
                            // Store this field so that if we also find an event, we can
                            // mark it as the backing field of the event.
                            Debug.Assert(addedField == null);
                            addedField = AddFieldToSymbolTable(field, aggregate);
                        }
                        else if (member is EventInfo e)
                        {
                            // Store events until after all fields
                            (events = events ?? new List<EventInfo>()).Add(e);
                        }
                    } while (memberEn.MoveNext());

                    if (events != null)
                    {
                        foreach (EventInfo e in events)
                        {
                            AddEventToSymbolTable(e, aggregate, addedField);
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private List<Type> CreateInheritanceHierarchyList(Type type)
        {
            List<Type> list;
            if (type.IsInterface)
            {
                Type[] ifaces = type.GetInterfaces();

                // Since IsWindowsRuntimeType() is rare, this is probably the final size
                list = new List<Type>(ifaces.Length + 2)
                {
                    type
                };
                foreach (Type iface in type.GetInterfaces())
                {
                    LoadSymbolsFromType(iface);
                    list.Add(iface);
                }

                Type obj = typeof(object);
                LoadSymbolsFromType(obj);
                list.Add(obj);
            }
            else
            {
                list = new List<Type> { type };
                for (Type parent = type.BaseType; parent != null; parent = parent.BaseType)
                {
                    // Load it in the symbol table.
                    LoadSymbolsFromType(parent);

                    // Insert into our list of Types.
                    list.Add(parent);
                }
            }

            // If we have a WinRT type then we should load the members of it's collection interfaces
            // as well as those members are on this type as far as the user is concerned.
            CType ctype = GetCTypeFromType(type);
            if (ctype.IsWindowsRuntimeType())
            {
                TypeArray collectioniFaces = ((AggregateType)ctype).GetWinRTCollectionIfacesAll(_semanticChecker.SymbolLoader);

                for (int i = 0; i < collectioniFaces.Count; i++)
                {
                    CType collectionType = collectioniFaces[i];
                    Debug.Assert(collectionType.isInterfaceType());

                    // Insert into our list of Types.
                    list.Add(collectionType.AssociatedSystemType);
                }
            }
            return list;
        }
        #endregion

        #region GetName
        /////////////////////////////////////////////////////////////////////////////////

        private Name GetName(string p)
        {
            return _nameManager.Add(p ?? "");
        }

        /////////////////////////////////////////////////////////////////////////////////

        private Name GetName(Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
            {
                int idx = name.IndexOf('`');
                if (idx >= 0)
                {
                    return _nameManager.Add(name, idx);
                }
            }

            return _nameManager.Add(name);
        }

        #endregion

        #region TypeParameters
        /////////////////////////////////////////////////////////////////////////////////

        private TypeArray GetMethodTypeParameters(MethodInfo method, MethodSymbol parent)
        {
            if (method.IsGenericMethod)
            {
                Type[] genericArguments = method.GetGenericArguments();
                CType[] ctypes = new CType[genericArguments.Length];
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    Type t = genericArguments[i];
                    ctypes[i] = LoadMethodTypeParameter(parent, t);
                }

                // After we load the type parameters, we need to resolve their bounds.
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    Type t = genericArguments[i];
                    ((TypeParameterType)ctypes[i]).GetTypeParameterSymbol().SetBounds(
                        _bsymmgr.AllocParams(
                        GetCTypeArrayFromTypes(t.GetGenericParameterConstraints())));
                }
                return _bsymmgr.AllocParams(ctypes.Length, ctypes);
            }
            return BSYMMGR.EmptyTypeArray();
        }

        /////////////////////////////////////////////////////////////////////////////////

        private TypeArray GetAggregateTypeParameters(Type type, AggregateSymbol agg)
        {
            if (type.IsGenericType)
            {
                Type genericDefinition = type.GetGenericTypeDefinition();
                Type[] genericArguments = genericDefinition.GetGenericArguments();
                List<CType> ctypes = new List<CType>();
                int outerParameters = agg.isNested() ? agg.GetOuterAgg().GetTypeVarsAll().Count : 0;

                for (int i = 0; i < genericArguments.Length; i++)
                {
                    // Suppose we have the following:
                    //
                    // class A<A1, A2, ..., An>
                    // {
                    //     class B<B1, B2, ..., Bm>
                    //     {
                    //     }
                    // }
                    //
                    // B will have m+n generic arguments - { A1, A2, ..., An, B1, B2, ..., Bn }.
                    // As we enumerate these, we need to skip type parameters whose GenericParameterPosition
                    // is less than n, since the first n type parameters are { A1, A2, ..., An }.

                    Type t = genericArguments[i];

                    if (t.GenericParameterPosition < outerParameters)
                    {
                        continue;
                    }

                    CType ctype = null;
                    if (t.IsGenericParameter && t.DeclaringType == genericDefinition)
                    {
                        ctype = LoadClassTypeParameter(agg, t);
                    }
                    else
                    {
                        ctype = GetCTypeFromType(t);
                    }

                    // We check to make sure we own the type parameter - this is because we're
                    // currently calculating TypeArgsThis, NOT TypeArgsAll.
                    if (((TypeParameterType)ctype).GetOwningSymbol() == agg)
                    {
                        ctypes.Add(ctype);
                    }
                }
                return _bsymmgr.AllocParams(ctypes.Count, ctypes.ToArray());
            }
            return BSYMMGR.EmptyTypeArray();
        }

        /////////////////////////////////////////////////////////////////////////////////

        private TypeParameterType LoadClassTypeParameter(AggregateSymbol parent, Type t)
        {
            for (AggregateSymbol p = parent; p != null; p = p.parent as AggregateSymbol)
            {
                for (TypeParameterSymbol typeParam = _bsymmgr.LookupAggMember(
                        GetName(t), p, symbmask_t.MASK_TypeParameterSymbol) as TypeParameterSymbol;
                    typeParam != null;
                    typeParam = BSYMMGR.LookupNextSym(typeParam, p, symbmask_t.MASK_TypeParameterSymbol) as TypeParameterSymbol)
                {
                    if (AreTypeParametersEquivalent(typeParam.GetTypeParameterType().AssociatedSystemType, t))
                    {
                        return typeParam.GetTypeParameterType();
                    }
                }
            }
            return AddTypeParameterToSymbolTable(parent, null, t, true);
        }

        /////////////////////////////////////////////////////////////////////////////////

        private bool AreTypeParametersEquivalent(Type t1, Type t2)
        {
            Debug.Assert(t1.IsGenericParameter && t2.IsGenericParameter);

            if (t1 == t2)
            {
                return true;
            }

            Type t1Original = GetOriginalTypeParameterType(t1);
            Type t2Original = GetOriginalTypeParameterType(t2);

            return t1Original == t2Original;
        }

        /////////////////////////////////////////////////////////////////////////////////

        // GetOriginalTypeParameterType
        // This was added so that LoadClassTypeParameter would not fail to find outer
        // type parameters when given a System.Type from an outer class and a matching
        // type parameter from in an inner class. In Reflection type parameters are
        // always declared by their inner most declaring class. For example, given:
        //
        //   class A<T> {
        //     class B<U> { }
        //   }
        //
        // in Reflection there are two Ts, A<T>'s T, and B<U>'s T. In our world there is
        // only A<T>'s T.
        //
        // So this method here drills down and finds the type parameter type corresponding
        // to the position of the given type parameter, from the outer most containing
        // type. So in the above example, given B<U>'s T from reflection, this will return
        // A<T>'s T, so that you can make a reference comparison of type parameters coming
        // from different nesting levels.
        //
        // There is an exception, we don't handle the case where you have type parameters
        // coming from different partially constructed methods. E.g.
        //
        //   class A<T,S> {
        //     public void M<U> { }
        //   }
        //
        //   A<T,int>.M<U>
        //   A<T,string>.M<U>
        //
        // In the above two methods, the two U's are different in Reflection. Here we just
        // return the type parameter type given if it is in a method, we do not try to
        // generalize these occurrences for reference equality.
        //
        private Type GetOriginalTypeParameterType(Type t)
        {
            Debug.Assert(t.IsGenericParameter);

            int pos = t.GenericParameterPosition;

            Type parentType = t.DeclaringType;
            if (parentType != null && parentType.IsGenericType)
            {
                parentType = parentType.GetGenericTypeDefinition();
            }

            if (t.DeclaringMethod != null)
            {
                MethodBase parentMethod = t.DeclaringMethod;

                if (parentType.GetGenericArguments() == null || pos >= parentType.GetGenericArguments().Length)
                {
                    return t;
                }
            }

            while (parentType.GetGenericArguments().Length > pos)
            {
                Type nextParent = parentType.DeclaringType;
                if (nextParent != null && nextParent.IsGenericType)
                {
                    nextParent = nextParent.GetGenericTypeDefinition();
                }

                if (nextParent?.GetGenericArguments()?.Length > pos)
                {
                    parentType = nextParent;
                }
                else
                {
                    break;
                }
            }

            return parentType.GetGenericArguments()[pos];
        }

        /////////////////////////////////////////////////////////////////////////////////

        private TypeParameterType LoadMethodTypeParameter(MethodSymbol parent, Type t)
        {
            for (Symbol sym = parent.firstChild; sym != null; sym = sym.nextChild)
            {
                if (!(sym is TypeParameterSymbol parSym))
                {
                    continue;
                }

                TypeParameterType type = parSym.GetTypeParameterType();
                if (AreTypeParametersEquivalent(type.AssociatedSystemType, t))
                {
                    return type;
                }
            }

            return AddTypeParameterToSymbolTable(null, parent, t, false);
        }

        /////////////////////////////////////////////////////////////////////////////////

        private TypeParameterType AddTypeParameterToSymbolTable(
                AggregateSymbol agg,
                MethodSymbol meth,
                Type t,
                bool bIsAggregate)
        {
            Debug.Assert((agg != null && bIsAggregate) || (meth != null && !bIsAggregate));

            TypeParameterSymbol typeParam;
            if (bIsAggregate)
            {
                typeParam = _symFactory.CreateClassTypeParameter(
                    GetName(t),
                    agg,
                    t.GenericParameterPosition,
                    t.GenericParameterPosition);
            }
            else
            {
                typeParam = _symFactory.CreateMethodTypeParameter(
                    GetName(t),
                    meth,
                    t.GenericParameterPosition,
                    t.GenericParameterPosition);
            }

            if ((t.GenericParameterAttributes & GenericParameterAttributes.Covariant) != 0)
            {
                typeParam.Covariant = true;
            }
            if ((t.GenericParameterAttributes & GenericParameterAttributes.Contravariant) != 0)
            {
                typeParam.Contravariant = true;
            }

            SpecCons cons = SpecCons.None;

            if ((t.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
            {
                cons |= SpecCons.New;
            }
            if ((t.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
            {
                cons |= SpecCons.Ref;
            }
            if ((t.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
            {
                cons |= SpecCons.Val;
            }

            typeParam.SetConstraints(cons);
            typeParam.SetAccess(ACCESS.ACC_PUBLIC);
            TypeParameterType typeParamType = _typeManager.GetTypeParameter(typeParam);

            return typeParamType;
        }

        #endregion

        #region LoadTypeChain
        /////////////////////////////////////////////////////////////////////////////////

        private CType LoadSymbolsFromType(Type originalType)
        {
            List<object> declarationChain = BuildDeclarationChain(originalType);

            Type type = originalType;
            CType ret = null;
            bool bIsByRef = type.IsByRef;
            if (bIsByRef)
            {
                type = type.GetElementType();
            }

            NamespaceOrAggregateSymbol current = _rootNamespace;

            // Go through the declaration chain and add namespaces and types for
            // each element in the chain.
            for (int i = 0; i < declarationChain.Count; i++)
            {
                object o = declarationChain[i];
                NamespaceOrAggregateSymbol next;
                if (o is Type)
                {
                    Type t = o as Type;
                    Name name = null;
                    name = GetName(t);
                    next = _symbolTable.LookupSym(name, current, symbmask_t.MASK_AggregateSymbol) as AggregateSymbol;

                    // Make sure we match arity as well when we find an aggregate.
                    if (next != null)
                    {
                        next = FindSymWithMatchingArity(next as AggregateSymbol, t);
                    }

                    // In the event that two different types exist that have the same name, they
                    // cannot both have entries in the symbol table with our current architecture.
                    // This can happen in dynamic, since the runtime binder lives across all
                    // call sites in an appdomain, and assemblies can have been loaded at runtime
                    // that have different types with the same name.

                    // In the real compiler, this would have been an error and name lookup would
                    // be ambiguous, but here we never have to lookup names of types for real (only
                    // names of members).

                    // The tactical fix is this: if we encounter this situation, where we have
                    // identically named types that are not the same, then we are going to clear
                    // the entire symbol table and restart this binding. This solution is not
                    // without its own problems, since it is possible to conceive of a single
                    // dynamic binding that needs to simultaneously know about both of the
                    // similarly named types, but we are not going to try to solve that
                    // scenario here.

                    if (next != null)
                    {
                        Type existingType = (next as AggregateSymbol).AssociatedSystemType;
                        Type newType = t.IsGenericType ? t.GetGenericTypeDefinition() : t;

                        // We use "IsEquivalentTo" so that unified local types for NoPIA do
                        // not trigger a reset. There are other mechanisms to make those sorts
                        // of types work in some scenarios.
                        if (!existingType.IsEquivalentTo(newType))
                        {
                            throw new ResetBindException();
                        }
                    }

                    // If we haven't found this type yet, then add it to our symbol table.
                    if (next == null || t.IsNullableType())
                    {
                        // Note that if we have anything other than an AggregateSymbol,
                        // we must be at the end of the line - that is, nothing else can
                        // have children.

                        CType ctype = ProcessSpecialTypeInChain(current, t);
                        if (ctype != null)
                        {
                            // If we had an aggregate type, its possible we're not at the end.
                            // This will happen for nullable<T> for instance.
                            if (ctype is AggregateType cat)
                            {
                                next = cat.GetOwningAggregate();
                            }
                            else
                            {
                                ret = ctype;
                                break;
                            }
                        }
                        else
                        {
                            // This is a regular class.
                            next = AddAggregateToSymbolTable(current, t);
                        }
                    }

                    if (t == type)
                    {
                        ret = GetConstructedType(type, next as AggregateSymbol);
                        break;
                    }
                }
                else if (o is MethodInfo)
                {
                    // We cant be at the end.
                    Debug.Assert(i + 1 < declarationChain.Count);
                    ret = ProcessMethodTypeParameter(o as MethodInfo, declarationChain[++i] as Type, current as AggregateSymbol);
                    break;
                }
                else
                {
                    Debug.Assert(o is string);
                    next = AddNamespaceToSymbolTable(current, o as string);
                }
                current = next;
            }

            Debug.Assert(ret != null);
            if (bIsByRef)
            {
                ret = _typeManager.GetParameterModifier(ret, false);
            }
            return ret;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private TypeParameterType ProcessMethodTypeParameter(MethodInfo methinfo, Type t, AggregateSymbol parent)
        {
            MethodSymbol meth = FindMatchingMethod(methinfo, parent);
            if (meth == null)
            {
                meth = AddMethodToSymbolTable(methinfo, parent, MethodKindEnum.Actual);

                // Because we return null from AddMethodToSymbolTable when we have a MethodKindEnum.Actual
                // and the method that we're trying to add is a special name, we need to assert that
                // we indeed have added a method. This is because no special name should have a method
                // type parameter on it.
                Debug.Assert(meth != null);
            }
            return LoadMethodTypeParameter(meth, t);
        }

        /////////////////////////////////////////////////////////////////////////////////

        private CType GetConstructedType(Type type, AggregateSymbol agg)
        {
            // We've found the one we want, so return it.
            if (type.IsGenericType)
            {
                // If we're a generic type, then we need to add the type arguments.
                List<CType> types = new List<CType>();

                foreach (Type argument in type.GetGenericArguments())
                {
                    types.Add(GetCTypeFromType(argument));
                }

                TypeArray typeArray = _bsymmgr.AllocParams(types.ToArray());
                AggregateType aggType = _typeManager.GetAggregate(agg, typeArray);
                return aggType;
            }
            CType ctype = agg.getThisType();
            return ctype;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private CType ProcessSpecialTypeInChain(NamespaceOrAggregateSymbol parent, Type t)
        {
            CType ctype;
            if (t.IsGenericParameter)
            {
                AggregateSymbol agg = parent as AggregateSymbol;
                Debug.Assert(agg != null);
                ctype = LoadClassTypeParameter(agg, t);
                return ctype;
            }
            else if (t.IsArray)
            {
                // Now we return an array of nesting level corresponding to the rank.
                ctype = _typeManager.GetArray(
                    GetCTypeFromType(t.GetElementType()), 
                    t.GetArrayRank(),
#if netcoreapp
                    t.IsSZArray
#else
                    t.GetElementType().MakeArrayType() == t
#endif
                    );
                return ctype;
            }
            else if (t.IsPointer)
            {
                // Now we return the pointer type that we want.
                ctype = _typeManager.GetPointer(GetCTypeFromType(t.GetElementType()));
                return ctype;
            }
            else if (t.IsNullableType())
            {
                // Get a nullable type of the underlying type.
                if (t.GetGenericArguments()[0].DeclaringType == t)
                {
                    // If the generic argument for nullable is our child, then we're
                    // declaring the initial Nullable<T>.
                    AggregateSymbol agg = _symbolTable.LookupSym(
                        GetName(t), parent, symbmask_t.MASK_AggregateSymbol) as AggregateSymbol;
                    if (agg != null)
                    {
                        agg = FindSymWithMatchingArity(agg, t);
                        if (agg != null)
                        {
                            Debug.Assert(agg.getThisType().AssociatedSystemType == t);
                            return agg.getThisType();
                        }
                    }
                    return AddAggregateToSymbolTable(parent, t).getThisType();
                }
                ctype = _typeManager.GetNullable(GetCTypeFromType(t.GetGenericArguments()[0]));
                return ctype;
            }
            return null;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static List<object> BuildDeclarationChain(Type callingType)
        {
            // We need to build the parent chain of the calling type. Since we only
            // have the type itself, first we need to build the chain up from the
            // type down to the root namespace, then we need to ensure that
            // the chain exists in our symbol table by searching from the root namespace
            // back down to the calling type. Also note that if we have a method type
            // parameter, then we'll also add the MethodBase to the chain.
            //
            // Note that we'll populate this list in a hybrid way - we'll add the
            // types for the type part of the chain, and we'll just add the string names
            // of the namespaces.

            // Strip off the ref-ness.
            if (callingType.IsByRef)
            {
                callingType = callingType.GetElementType();
            }

            List<object> callChain = new List<object>();
            for (Type t = callingType; t != null; t = t.DeclaringType)
            {
                callChain.Add(t);

                if (t.IsGenericParameter && t.DeclaringMethod != null)
                {
                    MethodBase methodBase = t.DeclaringMethod;
                    ParameterInfo[] parameters = methodBase.GetParameters();

                    bool bAdded = false;
#if UNSUPPORTEDAPI
                    foreach (MethodInfo methinfo in Enumerable.Where(t.DeclaringType.GetRuntimeMethods(), m => m.MetadataToken == methodBase.MetadataToken))
#else
                    foreach (MethodInfo methinfo in Enumerable.Where(t.DeclaringType.GetRuntimeMethods(), m => m.HasSameMetadataDefinitionAs(methodBase)))
#endif
                    {
                        if (!methinfo.IsGenericMethod)
                        {
                            continue;
                        }

                        Debug.Assert(!bAdded);
                        callChain.Add(methinfo);
                        bAdded = true;
                    }
                    Debug.Assert(bAdded);
                }
            }
            callChain.Reverse();

            // Now take out the namespaces and add them to the end of the chain.

            if (callingType.Namespace != null)
            {
                string[] namespaces = callingType.Namespace.Split('.');
                int index = 0;
                foreach (string s in namespaces)
                {
                    callChain.Insert(index++, s);
                }
            }
            return callChain;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private AggregateSymbol FindSymWithMatchingArity(AggregateSymbol aggregateSymbol, Type type)
        {
            for (AggregateSymbol agg = aggregateSymbol;
                agg != null;
                agg = BSYMMGR.LookupNextSym(agg, agg.Parent, symbmask_t.MASK_AggregateSymbol) as AggregateSymbol)
            {
                if (agg.GetTypeVarsAll().Count == type.GetGenericArguments().Length)
                {
                    return agg;
                }
            }
            return null;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private NamespaceSymbol AddNamespaceToSymbolTable(NamespaceOrAggregateSymbol parent, string sz)
        {
            Name name = GetName(sz);
            return _symbolTable.LookupSym(name, parent, symbmask_t.MASK_NamespaceSymbol) as NamespaceSymbol
                ?? _symFactory.CreateNamespace(name, parent as NamespaceSymbol);
        }
        #endregion

        #region CTypeFromType
        /////////////////////////////////////////////////////////////////////////////////

        internal CType[] GetCTypeArrayFromTypes(Type[] types)
        {
            Debug.Assert(types != null);

            int length = types.Length;
            if (length == 0)
            {
                return Array.Empty<CType>();
            }

            CType[] ctypes = new CType[length];
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];
                Debug.Assert(t != null);
                ctypes[i] = GetCTypeFromType(t);
            }

            return ctypes;
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal CType GetCTypeFromType(Type t)
        {
            return LoadSymbolsFromType(t);
        }
        #endregion

        #region Aggregates
        /////////////////////////////////////////////////////////////////////////////////

        private AggregateSymbol AddAggregateToSymbolTable(
            NamespaceOrAggregateSymbol parent,
            Type type)
        {
            AggregateSymbol agg = _symFactory.CreateAggregate(GetName(type), parent, _typeManager);
            agg.AssociatedSystemType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            agg.AssociatedAssembly = type.Assembly;

            // We have to set the TypeVars, access, and the AggKind before we can set the aggState
            // because of the assertion checking the compiler does.
            AggKindEnum kind;
            if (type.IsInterface)
            {
                kind = AggKindEnum.Interface;
            }
            else if (type.IsEnum)
            {
                kind = AggKindEnum.Enum;
                agg.SetUnderlyingType((AggregateType)GetCTypeFromType(Enum.GetUnderlyingType(type)));
            }
            else if (type.IsValueType)
            {
                kind = AggKindEnum.Struct;
            }
            else
            {
                // If it derives from Delegate or MulticastDelegate, then its
                // a delegate type. However, MuticastDelegate itself is not a
                // delegate type.
                if (type.BaseType != null &&
                    (type.BaseType.FullName == "System.MulticastDelegate" ||
                    type.BaseType.FullName == "System.Delegate") &&
                    type.FullName != "System.MulticastDelegate")
                {
                    kind = AggKindEnum.Delegate;
                }
                else
                {
                    kind = AggKindEnum.Class;
                }
            }
            agg.SetAggKind(kind);
            agg.SetTypeVars(BSYMMGR.EmptyTypeArray());

            ACCESS access;
            if (type.IsPublic)
            {
                access = ACCESS.ACC_PUBLIC;
            }
            else if (type.IsNested)
            {
                // If its nested, we may have other accessibility options.
                if (type.IsNestedAssembly || type.IsNestedFamANDAssem)
                {
                    // Note that we don't directly support NestedFamANDAssem, but we're just
                    // going to default to internal.
                    access = ACCESS.ACC_INTERNAL;
                }
                else if (type.IsNestedFamORAssem)
                {
                    access = ACCESS.ACC_INTERNALPROTECTED;
                }
                else if (type.IsNestedPrivate)
                {
                    access = ACCESS.ACC_PRIVATE;
                }
                else if (type.IsNestedFamily)
                {
                    access = ACCESS.ACC_PROTECTED;
                }
                else
                {
                    Debug.Assert(type.IsPublic || type.IsNestedPublic);
                    access = ACCESS.ACC_PUBLIC;
                }
            }
            else
            {
                // We're not public and we're not nested - we must be internal.
                access = ACCESS.ACC_INTERNAL;
            }
            agg.SetAccess(access);

            if (!type.IsGenericParameter)
            {
                agg.SetTypeVars(GetAggregateTypeParameters(type, agg));
            }

            if (type.IsGenericType)
            {
                Type genericDefinition = type.GetGenericTypeDefinition();
                Type[] genericArguments = genericDefinition.GetGenericArguments();

                // After we load the type parameters, we need to resolve their bounds.
                for (int i = 0; i < agg.GetTypeVars().Count; i++)
                {
                    Type t = genericArguments[i];
                    if (agg.GetTypeVars()[i] is TypeParameterType typeVar)
                    {
                        typeVar.GetTypeParameterSymbol().SetBounds(
                            _bsymmgr.AllocParams(
                            GetCTypeArrayFromTypes(t.GetGenericParameterConstraints())));
                    }
                }
            }

            agg.SetAbstract(type.IsAbstract);

            {
                string typeName = type.FullName;
                if (type.IsGenericType)
                {
                    typeName = type.GetGenericTypeDefinition().FullName;
                }
                if (typeName != null)
                {
                    PredefinedType predefinedType = PredefinedTypeFacts.TryGetPredefTypeIndex(typeName);
                    if (predefinedType != PredefinedType.PT_UNDEFINEDINDEX)
                    {
                        PredefinedTypes.InitializePredefinedType(agg, predefinedType);
                    }
                }
            }

            agg.SetSealed(type.IsSealed);

            AggregateType baseAggType = agg.getThisType();
            if (type.BaseType != null)
            {
                // type.BaseType can be null for Object or for interface types.
                Type t = type.BaseType;
                if (t.IsGenericType)
                {
                    t = t.GetGenericTypeDefinition();
                }
                agg.SetBaseClass((AggregateType)GetCTypeFromType(t));
            }
            agg.SetTypeManager(_typeManager);
            agg.SetFirstUDConversion(null);
            SetInterfacesOnAggregate(agg, type);
            agg.SetHasPubNoArgCtor(type.GetConstructor(Type.EmptyTypes) != null);

            // If we have a delegate, get its invoke and constructor methods as well.
            if (agg.IsDelegate())
            {
                PopulateSymbolTableWithName(SpecialNames.Constructor, null, type);
                PopulateSymbolTableWithName(SpecialNames.Invoke, null, type);
            }

            return agg;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void SetInterfacesOnAggregate(AggregateSymbol aggregate, Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            Type[] interfaces = type.GetInterfaces();

            // We won't be able to find the difference between Ifaces and
            // IfacesAll anymore - at runtime, the class implements all of its
            // Ifaces and IfacesAll, so theres no way to differentiate.
            //
            // This actually doesn't matter though - for conversions and methodcalls,
            // we don't really care where they've come from as long as we know the overall
            // set of IfacesAll.

            aggregate.SetIfaces(_bsymmgr.AllocParams(interfaces.Length, GetCTypeArrayFromTypes(interfaces)));
            aggregate.SetIfacesAll(aggregate.GetIfaces());
        }
        #endregion

        #region Field
        /////////////////////////////////////////////////////////////////////////////////

        private FieldSymbol AddFieldToSymbolTable(FieldInfo fieldInfo, AggregateSymbol aggregate)
        {
            FieldSymbol field = _symbolTable.LookupSym(
                GetName(fieldInfo.Name),
                aggregate,
                symbmask_t.MASK_FieldSymbol) as FieldSymbol;
            if (field != null)
            {
                return field;
            }

            field = _symFactory.CreateMemberVar(GetName(fieldInfo.Name), aggregate);
            field.AssociatedFieldInfo = fieldInfo;

            field.isStatic = fieldInfo.IsStatic;
            ACCESS access;
            if (fieldInfo.IsPublic)
            {
                access = ACCESS.ACC_PUBLIC;
            }
            else if (fieldInfo.IsPrivate)
            {
                access = ACCESS.ACC_PRIVATE;
            }
            else if (fieldInfo.IsFamily)
            {
                access = ACCESS.ACC_PROTECTED;
            }
            else if (fieldInfo.IsAssembly || fieldInfo.IsFamilyAndAssembly)
            {
                access = ACCESS.ACC_INTERNAL;
            }
            else
            {
                Debug.Assert(fieldInfo.IsFamilyOrAssembly);
                access = ACCESS.ACC_INTERNALPROTECTED;
            }
            field.SetAccess(access);
            field.isReadOnly = fieldInfo.IsInitOnly;
            field.isEvent = false;
            field.isAssigned = true;
            field.SetType(GetCTypeFromType(fieldInfo.FieldType));

            return field;
        }
        #endregion

        #region Events

        /////////////////////////////////////////////////////////////////////////////////

        private static readonly Type s_Sentinel = typeof(SymbolTable);
        private static Type s_EventRegistrationTokenType = s_Sentinel;
        private static Type s_WindowsRuntimeMarshal = s_Sentinel;
        private static Type s_EventRegistrationTokenTable = s_Sentinel;

        internal static Type EventRegistrationTokenType
        {
            get
            {
                return GetTypeByName(ref s_EventRegistrationTokenType, "System.Runtime.InteropServices.WindowsRuntime.EventRegistrationToken, System.Runtime.InteropServices.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            }
        }

        internal static Type WindowsRuntimeMarshalType
        {
            get
            {
                return GetTypeByName(ref s_WindowsRuntimeMarshal, "System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMarshal, System.Runtime.InteropServices.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            }
        }

        private static Type EventRegistrationTokenTableType
        {
            get
            {
                return GetTypeByName(ref s_EventRegistrationTokenTable, "System.Runtime.InteropServices.WindowsRuntime.EventRegistrationTokenTable`1, System.Runtime.InteropServices.WindowsRuntime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            }
        }

        private static Type GetTypeByName(ref Type cachedResult, string name)
        {
            if ((object)cachedResult == s_Sentinel)
            {
                System.Threading.Interlocked.CompareExchange(ref cachedResult, Type.GetType(name, throwOnError: false), s_Sentinel);
            }

            return cachedResult;
        }

        private void AddEventToSymbolTable(EventInfo eventInfo, AggregateSymbol aggregate, FieldSymbol addedField)
        {
            EventSymbol ev = _symbolTable.LookupSym(
                GetName(eventInfo.Name),
                aggregate,
                symbmask_t.MASK_EventSymbol) as EventSymbol;
            if (ev != null)
            {
                Debug.Assert(ev.AssociatedEventInfo == eventInfo);
                return;
            }

            ev = _symFactory.CreateEvent(GetName(eventInfo.Name), aggregate);
            ev.AssociatedEventInfo = eventInfo;

            // EventSymbol
            ACCESS access = ACCESS.ACC_PRIVATE;
            if (eventInfo.AddMethod != null)
            {
                ev.methAdd = AddMethodToSymbolTable(eventInfo.AddMethod, aggregate, MethodKindEnum.EventAccessor);
                ev.methAdd.SetEvent(ev);
                ev.isOverride = ev.methAdd.IsOverride();

                access = ev.methAdd.GetAccess();
            }
            if (eventInfo.RemoveMethod != null)
            {
                ev.methRemove = AddMethodToSymbolTable(eventInfo.RemoveMethod, aggregate, MethodKindEnum.EventAccessor);
                ev.methRemove.SetEvent(ev);
                ev.isOverride = ev.methRemove.IsOverride();

                access = ev.methRemove.GetAccess();
            }
            Debug.Assert(ev.methAdd != null || ev.methRemove != null);
            ev.isStatic = false;
            ev.type = GetCTypeFromType(eventInfo.EventHandlerType);

            // Symbol
            ev.SetAccess(access);

            Type eventRegistrationTokenType = EventRegistrationTokenType;
            if ((object)eventRegistrationTokenType != null && (object)WindowsRuntimeMarshalType != null &&
                ev.methAdd.RetType.AssociatedSystemType == eventRegistrationTokenType &&
                ev.methRemove.Params[0].AssociatedSystemType == eventRegistrationTokenType)
            {
                ev.IsWindowsRuntimeEvent = true;
            }

            // If we imported a field on the same aggregate, with the same name, and it also
            // has the same type, then that field is the backing field for this event, and
            // we mark it as such. This is used for the CSharpIsEventBinder.
            // In the case of a WindowsRuntime event, the field will be of type
            // EventRegistrationTokenTable<delegateType>.
            CType addedFieldType = addedField?.GetType();
            if (addedFieldType != null)
            {
                if (addedFieldType == ev.type)
                {
                    addedField.isEvent = true;
                }
                else
                {
                    Type associated = addedFieldType.AssociatedSystemType;
                    if (associated.IsConstructedGenericType
                        && associated.GetGenericTypeDefinition() == EventRegistrationTokenTableType
                        && associated.GenericTypeArguments[0] == ev.type.AssociatedSystemType)
                    {
                        addedField.isEvent = true;
                    }
                }
            }
        }
        #endregion

        #region Properties
        /////////////////////////////////////////////////////////////////////////////////

        internal void AddPredefinedPropertyToSymbolTable(AggregateSymbol type, Name property)
        {
            AggregateType aggtype = type.getThisType();
            Type t = aggtype.AssociatedSystemType;

            var props = Enumerable.Where(t.GetRuntimeProperties(), x => x.Name == property.Text);

            foreach (PropertyInfo pi in props)
            {
                AddPropertyToSymbolTable(pi, type);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void AddPropertyToSymbolTable(PropertyInfo property, AggregateSymbol aggregate)
        {
            Name name;
            bool isIndexer = property.GetIndexParameters().Length != 0
                             && property.DeclaringType?.GetCustomAttribute<DefaultMemberAttribute>()
                             ?.MemberName == property.Name;

            if (isIndexer)
            {
                name = GetName(SpecialNames.Indexer);
            }
            else
            {
                name = GetName(property.Name);
            }
            PropertySymbol prop = _symbolTable.LookupSym(
                name,
                aggregate,
                symbmask_t.MASK_PropertySymbol) as PropertySymbol;

            // If we already had one, see if it matches.
            if (prop != null)
            {
                PropertySymbol prevProp = null;

                // We'll have multiple properties with the same name if we have indexers.
                // In that case, we need to look at every indexer to see if we find one with
                // the matching associated sym that we want.
                while (prop != null)
                {
                    if (prop.AssociatedPropertyInfo.IsEquivalentTo(property))
                    {
                        return;
                    }

                    prevProp = prop;
                    prop = _semanticChecker.SymbolLoader.LookupNextSym(prop, prop.parent, symbmask_t.MASK_PropertySymbol) as PropertySymbol;
                }

                prop = prevProp;
                if (isIndexer)
                {
                    // We have an indexer for a different property info, so
                    // create a new symbol for it.
                    prop = null;
                }
            }

            // If we already had a property but its associated info doesn't match,
            // then we repurpose the property that we've found. This can happen
            // in the case of generic instantiations.
            //
            // Note that this is a bit of a hack - the best way to fix this is
            // by not depending on the instantiated properties at all, but rather depending
            // on their non-instantiated generic form, which can be gotten from the
            // parent's generic type definition's member. From there, we'll also need to
            // keep track of the instantiation as we move along, so that when we need the
            // associated property, we can instantiate it correctly.
            //
            // This seems far too heavyweight - since we know we will never bind to more
            // than one property per payload, lets just blast it each time.
            if (prop == null)
            {
                if (isIndexer)
                {
                    prop = _semanticChecker.SymbolLoader.GetGlobalSymbolFactory().CreateIndexer(name, aggregate, GetName(property.Name));
                    prop.Params = CreateParameterArray(null, property.GetIndexParameters());
                }
                else
                {
                    prop = _symFactory.CreateProperty(GetName(property.Name), aggregate);
                    prop.Params = BSYMMGR.EmptyTypeArray();
                }
            }
            prop.AssociatedPropertyInfo = property;

            prop.isStatic = property.GetGetMethod(true) != null ? property.GetGetMethod(true).IsStatic : property.GetSetMethod(true).IsStatic;
            prop.isParamArray = DoesMethodHaveParameterArray(property.GetIndexParameters());
            prop.swtSlot = null;
            prop.RetType = GetCTypeFromType(property.PropertyType);
            prop.isOperator = isIndexer;

            // Determine if its an override. We should always have an accessor, unless
            // the metadata was bogus.
            if (property.GetMethod != null || property.SetMethod != null)
            {
                MethodInfo accessor = property.GetMethod ?? property.SetMethod; // Must have at least one.
                prop.isOverride = accessor.IsVirtual && accessor.IsHideBySig && accessor.GetRuntimeBaseDefinition() != accessor;
                prop.isHideByName = !accessor.IsHideBySig;
            }

            SetParameterDataForMethProp(prop, property.GetIndexParameters());

            // Get and set.
            MethodInfo methGet = property.GetMethod;
            MethodInfo methSet = property.SetMethod;
            ACCESS access = ACCESS.ACC_PRIVATE;
            if (methGet != null)
            {
                prop.GetterMethod = AddMethodToSymbolTable(methGet, aggregate, MethodKindEnum.PropAccessor);

                // If we have an indexed property, leave the method as a method we can call,
                // and mark the property as bogus.
                if (isIndexer || prop.GetterMethod.Params.Count == 0)
                {
                    prop.GetterMethod.SetProperty(prop);
                }
                else
                {
                    prop.Bogus = true;
                    prop.GetterMethod.SetMethKind(MethodKindEnum.Actual);
                }

                if (prop.GetterMethod.GetAccess() > access)
                {
                    access = prop.GetterMethod.GetAccess();
                }
            }
            if (methSet != null)
            {
                prop.SetterMethod = AddMethodToSymbolTable(methSet, aggregate, MethodKindEnum.PropAccessor);

                // If we have an indexed property, leave the method as a method we can call,
                // and mark the property as bogus.
                if (isIndexer || prop.SetterMethod.Params.Count == 1)
                {
                    prop.SetterMethod.SetProperty(prop);
                }
                else
                {
                    prop.Bogus = true;
                    prop.SetterMethod.SetMethKind(MethodKindEnum.Actual);
                }

                if (prop.SetterMethod.GetAccess() > access)
                {
                    access = prop.SetterMethod.GetAccess();
                }
            }

            // The access of the property is the least restrictive access of its getter/setter.
            prop.SetAccess(access);
        }

        #endregion

        #region Methods
        /////////////////////////////////////////////////////////////////////////////////

        internal void AddPredefinedMethodToSymbolTable(AggregateSymbol type, Name methodName)
        {
            Type t = type.getThisType().AssociatedSystemType;

            // If we got here, it means we couldn't find it in our initial lookup. Means we haven't loaded it from reflection yet.
            // Lets go and do that now.
            // Check if we have constructors or not.
            if (methodName == NameManager.GetPredefinedName(PredefinedName.PN_CTOR))
            {
                foreach (ConstructorInfo c in t.GetConstructors())
                {
                    AddMethodToSymbolTable(
                        c,
                        type,
                        MethodKindEnum.Constructor);
                }
            }
            else
            {
                var methods = Enumerable.Where(t.GetRuntimeMethods(), m => m.Name == methodName.Text && m.DeclaringType == t);

                foreach (MethodInfo m in methods)
                {
                    AddMethodToSymbolTable(
                        m,
                        type,
                        m.Name == SpecialNames.Invoke ? MethodKindEnum.Invoke : MethodKindEnum.Actual);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private MethodSymbol AddMethodToSymbolTable(MemberInfo member, AggregateSymbol callingAggregate, MethodKindEnum kind)
        {
            MethodInfo method = member as MethodInfo;
            ConstructorInfo ctor = member as ConstructorInfo;

            Debug.Assert(method != null || ctor != null);
#if UNSUPPORTEDAPI
            Debug.Assert(member.DeclaringType == member.ReflectedType);
#endif
            // If we are trying to add an actual method via MethodKindEnum.Actual, and
            // the memberinfo is a special name, and its not static, then return null.
            // We'll re-add the thing later with some other method kind.
            //
            // This will happen for things like indexers and properties. The ones that have
            // special names that we DO want to allow adding are things like operators, which
            // are static and will not be added again later.

            if (kind == MethodKindEnum.Actual && // MethKindEnum.Actual
                (method == null || // Not a ConstructorInfo
                    (!method.IsStatic && method.IsSpecialName))) // Not static and is a special name
            {
                return null;
            }

            MethodSymbol methodSymbol = FindMatchingMethod(member, callingAggregate);
            if (methodSymbol != null)
            {
                return methodSymbol;
            }

            ParameterInfo[] parameters = method != null ? method.GetParameters() : ctor.GetParameters();
            // First create the method.
            methodSymbol = _symFactory.CreateMethod(GetName(member.Name), callingAggregate);
            methodSymbol.AssociatedMemberInfo = member;
            methodSymbol.SetMethKind(kind);
            if (kind == MethodKindEnum.ExplicitConv || kind == MethodKindEnum.ImplicitConv)
            {
                callingAggregate.SetHasConversion();
                methodSymbol.SetConvNext(callingAggregate.GetFirstUDConversion());
                callingAggregate.SetFirstUDConversion(methodSymbol);
            }
            ACCESS access;
            if (method != null)
            {
                if (method.IsPublic)
                {
                    access = ACCESS.ACC_PUBLIC;
                }
                else if (method.IsPrivate)
                {
                    access = ACCESS.ACC_PRIVATE;
                }
                else if (method.IsFamily)
                {
                    access = ACCESS.ACC_PROTECTED;
                }
                else if (method.IsAssembly || method.IsFamilyAndAssembly)
                {
                    access = ACCESS.ACC_INTERNAL;
                }
                else
                {
                    Debug.Assert(method.IsFamilyOrAssembly);
                    access = ACCESS.ACC_INTERNALPROTECTED;
                }
            }
            else
            {
                Debug.Assert(ctor != null);
                if (ctor.IsPublic)
                {
                    access = ACCESS.ACC_PUBLIC;
                }
                else if (ctor.IsPrivate)
                {
                    access = ACCESS.ACC_PRIVATE;
                }
                else if (ctor.IsFamily)
                {
                    access = ACCESS.ACC_PROTECTED;
                }
                else if (ctor.IsAssembly || ctor.IsFamilyAndAssembly)
                {
                    access = ACCESS.ACC_INTERNAL;
                }
                else
                {
                    Debug.Assert(ctor.IsFamilyOrAssembly);
                    access = ACCESS.ACC_INTERNALPROTECTED;
                }
            }
            methodSymbol.SetAccess(access);

            if (method != null)
            {
                methodSymbol.typeVars = GetMethodTypeParameters(method, methodSymbol);
                methodSymbol.isVirtual = method.IsVirtual;
                methodSymbol.isAbstract = method.IsAbstract;
                methodSymbol.isStatic = method.IsStatic;
                methodSymbol.isOverride = method.IsVirtual && method.IsHideBySig && method.GetRuntimeBaseDefinition() != method;
                methodSymbol.isOperator = IsOperator(method);
                methodSymbol.swtSlot = GetSlotForOverride(method);
                methodSymbol.isVarargs = (method.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs;
                methodSymbol.RetType = GetCTypeFromType(method.ReturnType);
            }
            else
            {
                methodSymbol.typeVars = BSYMMGR.EmptyTypeArray();
                methodSymbol.isVirtual = ctor.IsVirtual;
                methodSymbol.isAbstract = ctor.IsAbstract;
                methodSymbol.isStatic = ctor.IsStatic;
                methodSymbol.isOverride = false;
                methodSymbol.isOperator = false;
                methodSymbol.swtSlot = null;
                methodSymbol.isVarargs = false;
                methodSymbol.RetType = _typeManager.GetVoid();
            }
            methodSymbol.modOptCount = GetCountOfModOpts(parameters);

            methodSymbol.isParamArray = DoesMethodHaveParameterArray(parameters);
            methodSymbol.isHideByName = false;

            methodSymbol.errExpImpl = null;
            methodSymbol.Params = CreateParameterArray(methodSymbol.AssociatedMemberInfo, parameters);

            SetParameterDataForMethProp(methodSymbol, parameters);

            return methodSymbol;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void SetParameterDataForMethProp(MethodOrPropertySymbol methProp, ParameterInfo[] parameters)
        {
            if (parameters.Length > 0)
            {
                // See if we have a param array.
                var attributes = parameters[parameters.Length - 1].GetCustomAttributes(false);
                if (attributes != null)
                {
                    foreach (object o in attributes)
                    {
                        if (o is ParamArrayAttribute)
                        {
                            methProp.isParamArray = true;
                        }
                    }
                }

                // Mark the names of the parameters, and their default values.
                for (int i = 0; i < parameters.Length; i++)
                {
                    SetParameterAttributes(methProp, parameters, i);

                    // Insert the name.
                    methProp.ParameterNames.Add(GetName(parameters[i].Name));
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void SetParameterAttributes(MethodOrPropertySymbol methProp, ParameterInfo[] parameters, int i)
        {
            if (((parameters[i].Attributes & ParameterAttributes.Optional) != 0) &&
                !parameters[i].ParameterType.IsByRef)
            {
                methProp.SetOptionalParameter(i);
                PopulateSymbolTableWithName("Value", new Type[] { typeof(Missing) }, typeof(Missing)); // We might need this later
            }

            object[] attrs;

            // Get MarshalAsAttribute
            if ((parameters[i].Attributes & ParameterAttributes.HasFieldMarshal) != 0)
            {
                if ((attrs = parameters[i].GetCustomAttributes(typeof(MarshalAsAttribute), false).ToArray()) != null
                    && attrs.Length > 0)
                {
                    MarshalAsAttribute attr = (MarshalAsAttribute)attrs[0];
                    methProp.SetMarshalAsParameter(i, attr.Value);
                }
            }

            // Get the various kinds of default values
            if ((attrs = parameters[i].GetCustomAttributes(typeof(DateTimeConstantAttribute), false).ToArray()) != null
                && attrs.Length > 0)
            {
                // Get DateTimeConstant

                DateTimeConstantAttribute attr = (DateTimeConstantAttribute)attrs[0];

                ConstVal cv = ConstVal.Get(((DateTime)attr.Value).Ticks);
                CType cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_DATETIME);
                methProp.SetDefaultParameterValue(i, cvType, cv);
            }
            else if ((attrs = parameters[i].GetCustomAttributes(typeof(DecimalConstantAttribute), false).ToArray()) != null
                && attrs.Length > 0)
            {
                // Get DecimalConstant

                DecimalConstantAttribute attr = (DecimalConstantAttribute)attrs[0];

                ConstVal cv = ConstVal.Get(attr.Value);
                CType cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_DECIMAL);
                methProp.SetDefaultParameterValue(i, cvType, cv);
            }
            else if (((parameters[i].Attributes & ParameterAttributes.HasDefault) != 0) &&
                !parameters[i].ParameterType.IsByRef)
            {
                // Only set a default value if we have one, and the type that we're
                // looking at isn't a by ref type or a type parameter.

                ConstVal cv = default(ConstVal);
                CType cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_OBJECT);

                // We need to use RawDefaultValue, because DefaultValue is too clever.
#if UNSUPPORTEDAPI
                if (parameters[i].RawDefaultValue != null)
                {
                    object defValue = parameters[i].RawDefaultValue;
#else
                if (parameters[i].DefaultValue != null)
                {
                    object defValue = parameters[i].DefaultValue;
#endif
                    Type defType = defValue.GetType();

                    if (defType == typeof(byte))
                    {
                        cv = ConstVal.Get((byte)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_BYTE);
                    }
                    else if (defType == typeof(short))
                    {
                        cv = ConstVal.Get((short)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_SHORT);
                    }
                    else if (defType == typeof(int))
                    {
                        cv = ConstVal.Get((int)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_INT);
                    }
                    else if (defType == typeof(long))
                    {
                        cv = ConstVal.Get((long)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_LONG);
                    }
                    else if (defType == typeof(float))
                    {
                        cv = ConstVal.Get((float)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_FLOAT);
                    }
                    else if (defType == typeof(double))
                    {
                        cv = ConstVal.Get((double)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_DOUBLE);
                    }
                    else if (defType == typeof(decimal))
                    {
                        cv = ConstVal.Get((decimal)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_DECIMAL);
                    }
                    else if (defType == typeof(char))
                    {
                        cv = ConstVal.Get((char)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_CHAR);
                    }
                    else if (defType == typeof(bool))
                    {
                        cv = ConstVal.Get((bool)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_BOOL);
                    }
                    else if (defType == typeof(sbyte))
                    {
                        cv = ConstVal.Get((sbyte)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_SBYTE);
                    }
                    else if (defType == typeof(ushort))
                    {
                        cv = ConstVal.Get((ushort)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_USHORT);
                    }
                    else if (defType == typeof(uint))
                    {
                        cv = ConstVal.Get((uint)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_UINT);
                    }
                    else if (defType == typeof(ulong))
                    {
                        cv = ConstVal.Get((ulong)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_ULONG);
                    }
                    else if (defType == typeof(string))
                    {
                        cv = ConstVal.Get((string)defValue);
                        cvType = _semanticChecker.SymbolLoader.GetPredefindType(PredefinedType.PT_STRING);
                    }
                    // if we fall off the end of this cascading if, we get Object/null
                    // because that's how we initialized the constval.
                }
                methProp.SetDefaultParameterValue(i, cvType, cv);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private MethodSymbol FindMatchingMethod(MemberInfo method, AggregateSymbol callingAggregate)
        {
            MethodSymbol meth = _bsymmgr.LookupAggMember(GetName(method.Name), callingAggregate, symbmask_t.MASK_MethodSymbol) as MethodSymbol;
            while (meth != null)
            {
                if (meth.AssociatedMemberInfo.IsEquivalentTo(method))
                {
                    return meth;
                }
                meth = BSYMMGR.LookupNextSym(meth, callingAggregate, symbmask_t.MASK_MethodSymbol) as MethodSymbol;
            }
            return null;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private uint GetCountOfModOpts(ParameterInfo[] parameters)
        {
            uint count = 0;
#if UNSUPPORTEDAPI
            foreach (ParameterInfo p in parameters)
            {
                if (p.GetOptionalCustomModifiers() != null)
                {
                    count += (uint)p.GetOptionalCustomModifiers().Length;
                }
            }
#endif
            return count;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private TypeArray CreateParameterArray(MemberInfo associatedInfo, ParameterInfo[] parameters)
        {
            List<CType> types = new List<CType>();

            foreach (ParameterInfo p in parameters)
            {
                types.Add(GetTypeOfParameter(p, associatedInfo));
            }

            MethodInfo mi = associatedInfo as MethodInfo;

            if (mi != null && (mi.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
            {
                types.Add(_typeManager.GetArgListType());
            }

            return _bsymmgr.AllocParams(types.Count, types.ToArray());
        }

        /////////////////////////////////////////////////////////////////////////////////

        private CType GetTypeOfParameter(ParameterInfo p, MemberInfo m)
        {
            Type t = p.ParameterType;
            CType ctype;
            if (t.IsGenericParameter && t.DeclaringMethod != null && t.DeclaringMethod == m)
            {
                // If its a method type parameter from ourselves, just find it.
                ctype = LoadMethodTypeParameter(FindMethodFromMemberInfo(m), t);
            }
            else
            {
                ctype = GetCTypeFromType(t);
            }

            // Check if we have an out parameter.
            if (ctype is ParameterModifierType mod && p.IsOut && !p.IsIn)
            {
                CType parameterType = mod.GetParameterType();
                ctype = _typeManager.GetParameterModifier(parameterType, true);
            }

            return ctype;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private bool DoesMethodHaveParameterArray(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0)
            {
                return false;
            }

            ParameterInfo p = parameters[parameters.Length - 1];
            var attributes = p.GetCustomAttributes(false);

            foreach (object o in attributes)
            {
                if (o is ParamArrayAttribute)
                {
                    return true;
                }
            }
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private SymWithType GetSlotForOverride(MethodInfo method)
        {
            if (method.IsVirtual && method.IsHideBySig)
            {
                MethodInfo baseMethodInfo = method.GetRuntimeBaseDefinition();
                if (baseMethodInfo == method)
                {
                    // We just found ourselves, so we don't care here.
                    return null;
                }

                // We have the base class method that we're overriding. We can assume
                // that all the parent aggregate symbols were added, and that we added
                // the methods in order. As such, our parent methods should be in the
                // symbol table at this point.

                AggregateSymbol aggregate = GetCTypeFromType(baseMethodInfo.DeclaringType).getAggregate();
                MethodSymbol baseMethod = FindMethodFromMemberInfo(baseMethodInfo);

                // This assert is temporarily disabled to improve testability of the area on .NetNative
                //Debug.Assert(baseMethod != null);
                if ((object)baseMethod == null)
                {
                    throw Error.InternalCompilerError();
                }

                return new SymWithType(baseMethod, aggregate.getThisType());
            }
            return null;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private MethodSymbol FindMethodFromMemberInfo(MemberInfo baseMemberInfo)
        {
            CType t = GetCTypeFromType(baseMemberInfo.DeclaringType);
            Debug.Assert(t is AggregateType);
            AggregateSymbol aggregate = t.getAggregate();
            Debug.Assert(aggregate != null);

            MethodSymbol meth = _semanticChecker.SymbolLoader.LookupAggMember(
                GetName(baseMemberInfo.Name),
                aggregate,
                symbmask_t.MASK_MethodSymbol) as MethodSymbol;
            for (;
                    meth != null && !meth.AssociatedMemberInfo.IsEquivalentTo(baseMemberInfo);
                    meth = _semanticChecker.SymbolLoader.LookupNextSym(meth, aggregate, symbmask_t.MASK_MethodSymbol) as MethodSymbol)
                ;

            return meth;
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal bool AggregateContainsMethod(AggregateSymbol agg, string szName, symbmask_t mask)
        {
            return _semanticChecker.SymbolLoader.LookupAggMember(GetName(szName), agg, mask) != null;
        }
        #endregion

        #region Conversions
        /////////////////////////////////////////////////////////////////////////////////

        internal void AddConversionsForType(Type type)
        {
            if (type.IsInterface)
            {
                AddConversionsForOneType(type);
            }
            for (Type t = type; t.BaseType != null; t = t.BaseType)
            {
                AddConversionsForOneType(t);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private void AddConversionsForOneType(Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            if (!_typesWithConversionsLoaded.Add(type))
            {
                return;
            }

            // Always make the aggregate for the type, regardless of whether or not
            // there are any conversions.
            CType t = GetCTypeFromType(type);

            if (!(t is AggregateType))
            {
                CType endT;
                while ((endT = t.GetBaseOrParameterOrElementType()) != null)
                {
                    t = endT;
                }
            }

            if (t is TypeParameterType paramType)
            {
                // Add conversions for the bounds.
                foreach (CType bound in paramType.GetBounds().Items)
                {
                    AddConversionsForType(bound.AssociatedSystemType);
                }
                return;
            }

            Debug.Assert(t is AggregateType);
            AggregateSymbol aggregate = ((AggregateType)t).getAggregate();

            // Now find all the conversions and make them.
            foreach (MethodInfo conversion in type.GetRuntimeMethods())
            {
                if (conversion.IsPublic && conversion.IsStatic && conversion.DeclaringType == type
                    && conversion.IsSpecialName && !conversion.IsGenericMethod)
                {
                    MethodKindEnum methodKind;
                    switch (conversion.Name)
                    {
                        case SpecialNames.ImplicitConversion:
                            methodKind = MethodKindEnum.ImplicitConv;
                            break;
                        case SpecialNames.ExplicitConversion:
                            methodKind = MethodKindEnum.ExplicitConv;
                            break;
                        default:
                            continue;
                    }

                    AddMethodToSymbolTable(conversion, aggregate, methodKind);
                }
            }
        }
        #endregion

        #region Operators
        /////////////////////////////////////////////////////////////////////////////////

        private static bool IsOperator(MethodInfo method)
        {
            if (method.IsSpecialName && method.IsStatic)
            {
                switch (method.Name)
                {
                    case SpecialNames.ImplicitConversion:
                    case SpecialNames.ExplicitConversion:
                    case SpecialNames.CLR_Add:
                    case SpecialNames.CLR_Subtract:
                    case SpecialNames.CLR_Multiply:
                    case SpecialNames.CLR_Division:
                    case SpecialNames.CLR_Modulus:
                    case SpecialNames.CLR_LShift:
                    case SpecialNames.CLR_RShift:
                    case SpecialNames.CLR_LT:
                    case SpecialNames.CLR_GT:
                    case SpecialNames.CLR_LTE:
                    case SpecialNames.CLR_GTE:
                    case SpecialNames.CLR_Equality:
                    case SpecialNames.CLR_Inequality:
                    case SpecialNames.CLR_BitwiseAnd:
                    case SpecialNames.CLR_ExclusiveOr:
                    case SpecialNames.CLR_BitwiseOr:
                    case SpecialNames.CLR_LogicalNot:
                    case SpecialNames.CLR_UnaryNegation:
                    case SpecialNames.CLR_UnaryPlus:
                    case SpecialNames.CLR_OnesComplement:
                    case SpecialNames.CLR_True:
                    case SpecialNames.CLR_False:
                    case SpecialNames.CLR_PreIncrement:
                    case SpecialNames.CLR_PreDecrement:
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}
