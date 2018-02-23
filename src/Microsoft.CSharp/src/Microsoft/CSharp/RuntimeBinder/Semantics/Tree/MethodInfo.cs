// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprMethodInfo : ExprWithType
    {
        public ExprMethodInfo(CType type, MethodSymbol method, AggregateType methodType, TypeArray methodParameters)
            : base(ExpressionKind.MethodInfo, type)
        {
            Debug.Assert(method != null);
            Debug.Assert(methodType != null);
            Method = new MethWithInst(method, methodType, methodParameters);
        }

        public MethWithInst Method { get; }

        public MethodInfo MethodInfo
        {
            get
            {
                // To do this, we need to construct a type array of the parameter types,
                // get the parent constructed type, and get the method from it.
                AggregateType aggType = Method.Ats;
                MethodSymbol methSym = Method.Meth();

                TypeArray genericParams = TypeManager.SubstTypeArray(methSym.Params, aggType, methSym.typeVars);
                CType genericReturn = TypeManager.SubstType(methSym.RetType, aggType, methSym.typeVars);

                Type type = aggType.AssociatedSystemType;
                MethodInfo methodInfo = methSym.AssociatedMemberInfo as MethodInfo;

                // This is to ensure that for embedded nopia types, we have the
                // appropriate local type from the member itself; this is possible
                // because nopia types are not generic or nested.
                if (!type.IsGenericType && !type.IsNested)
                {
                    type = methodInfo.DeclaringType;
                }

                // We need to find the associated methodinfo on the instantiated type.
                foreach (MethodInfo m in type.GetRuntimeMethods())
                {
#if UNSUPPORTEDAPI
                if ((m.MetadataToken != methodInfo.MetadataToken) || (m.Module != methodInfo.Module))
#else
                    if (!m.HasSameMetadataDefinitionAs(methodInfo))
#endif
                    {
                        continue;
                    }

                    Debug.Assert(m.Name == methodInfo.Name &&
                        m.GetParameters().Length == genericParams.Count &&
                        TypesAreEqual(m.ReturnType, genericReturn.AssociatedSystemType));

                    bool match = true;
                    ParameterInfo[] parameters = m.GetParameters();
                    for (int i = 0; i < genericParams.Count; i++)
                    {
                        if (!TypesAreEqual(parameters[i].ParameterType, genericParams[i].AssociatedSystemType))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        if (m.IsGenericMethod)
                        {
                            int size = Method.TypeArgs?.Count ?? 0;
                            Type[] typeArgs = new Type[size];
                            if (size > 0)
                            {
                                for (int i = 0; i < Method.TypeArgs.Count; i++)
                                {
                                    typeArgs[i] = Method.TypeArgs[i].AssociatedSystemType;
                                }
                            }

                            return m.MakeGenericMethod(typeArgs);
                        }

                        return m;
                    }
                }

                throw Error.InternalCompilerError();
            }
        }

        public ConstructorInfo ConstructorInfo
        {
            get
            {
                // To do this, we need to construct a type array of the parameter types,
                // get the parent constructed type, and get the method from it.
                AggregateType aggType = Method.Ats;
                MethodSymbol methSym = Method.Meth();

                TypeArray genericInstanceParams = TypeManager.SubstTypeArray(methSym.Params, aggType);
                Type type = aggType.AssociatedSystemType;
                ConstructorInfo ctorInfo = (ConstructorInfo)methSym.AssociatedMemberInfo;

                // This is to ensure that for embedded nopia types, we have the
                // appropriate local type from the member itself; this is possible
                // because nopia types are not generic or nested.
                if (!type.IsGenericType && !type.IsNested)
                {
                    type = ctorInfo.DeclaringType;
                }

                foreach (ConstructorInfo c in type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
#if UNSUPPORTEDAPI
                if ((c.MetadataToken != ctorInfo.MetadataToken) || (c.Module != ctorInfo.Module))
#else
                    if (!c.HasSameMetadataDefinitionAs(ctorInfo))
#endif
                    {
                        continue;
                    }

                    Debug.Assert(c.GetParameters() == null || c.GetParameters().Length == genericInstanceParams.Count);

                    bool match = true;
                    ParameterInfo[] parameters = c.GetParameters();
                    for (int i = 0; i < genericInstanceParams.Count; i++)
                    {
                        if (!TypesAreEqual(parameters[i].ParameterType, genericInstanceParams[i].AssociatedSystemType))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        return c;
                    }
                }

                throw Error.InternalCompilerError();
            }
        }

        public override object Object => MethodInfo;
    }
}
