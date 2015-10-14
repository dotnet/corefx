// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{
    internal class EnumerableRewriter : OldExpressionVisitor
    {
        internal EnumerableRewriter()
        {
        }

        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = this.Visit(m.Object);
            ReadOnlyCollection<Expression> args = this.VisitExpressionList(m.Arguments);

            // check for args changed
            if (obj != m.Object || args != m.Arguments)
            {
                MethodInfo mInfo = m.Method;
                Type[] typeArgs = (mInfo.IsGenericMethod) ? mInfo.GetGenericArguments() : null;

                if ((mInfo.IsStatic || mInfo.DeclaringType.IsAssignableFrom(obj.Type))
                    && ArgsMatch(mInfo, args, typeArgs))
                {
                    // current method is still valid
                    return Expression.Call(obj, mInfo, args);
                }
                else if (mInfo.DeclaringType == typeof(Queryable))
                {
                    // convert Queryable method to Enumerable method
                    MethodInfo seqMethod = FindEnumerableMethod(mInfo.Name, args, typeArgs);
                    args = this.FixupQuotedArgs(seqMethod, args);
                    return Expression.Call(obj, seqMethod, args);
                }
                else
                {
                    // rebind to new method
                    MethodInfo method = FindMethod(mInfo.DeclaringType, mInfo.Name, args, typeArgs);
                    args = this.FixupQuotedArgs(method, args);
                    return Expression.Call(obj, method, args);
                }
            }
            return m;
        }

        private ReadOnlyCollection<Expression> FixupQuotedArgs(MethodInfo mi, ReadOnlyCollection<Expression> argList)
        {
            ParameterInfo[] pis = mi.GetParameters();
            if (pis.Length > 0)
            {
                List<Expression> newArgs = null;
                for (int i = 0, n = pis.Length; i < n; i++)
                {
                    Expression arg = argList[i];
                    ParameterInfo pi = pis[i];
                    arg = FixupQuotedExpression(pi.ParameterType, arg);
                    if (newArgs == null && arg != argList[i])
                    {
                        newArgs = new List<Expression>(argList.Count);
                        for (int j = 0; j < i; j++)
                        {
                            newArgs.Add(argList[j]);
                        }
                    }
                    if (newArgs != null)
                    {
                        newArgs.Add(arg);
                    }
                }
                if (newArgs != null)
                    argList = newArgs.AsReadOnly();
            }
            return argList;
        }

        private Expression FixupQuotedExpression(Type type, Expression expression)
        {
            Expression expr = expression;
            while (true)
            {
                if (type.IsAssignableFrom(expr.Type))
                    return expr;
                if (expr.NodeType != ExpressionType.Quote)
                    break;
                expr = ((UnaryExpression)expr).Operand;
            }
            if (!type.IsAssignableFrom(expr.Type) && type.IsArray && expr.NodeType == ExpressionType.NewArrayInit)
            {
                Type strippedType = StripExpression(expr.Type);
                if (type.IsAssignableFrom(strippedType))
                {
                    Type elementType = type.GetElementType();
                    NewArrayExpression na = (NewArrayExpression)expr;
                    List<Expression> exprs = new List<Expression>(na.Expressions.Count);
                    for (int i = 0, n = na.Expressions.Count; i < n; i++)
                    {
                        exprs.Add(this.FixupQuotedExpression(elementType, na.Expressions[i]));
                    }
                    expression = Expression.NewArrayInit(elementType, exprs);
                }
            }
            return expression;
        }

        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            return lambda;
        }

        private static Type GetPublicType(Type t)
        {
            // If we create a constant explicitly typed to be a private nested type,
            // such as Lookup<,>.Grouping or a compiler-generated iterator class, then
            // we cannot use the expression tree in a context which has only execution
            // permissions.  We should endeavour to translate constants into 
            // new constants which have public types.
            TypeInfo typeInfo = t.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition().GetTypeInfo().ImplementedInterfaces.Contains(typeof(IGrouping<,>)))
                return typeof(IGrouping<,>).MakeGenericType(t.GetGenericArguments());
            if (!typeInfo.IsNestedPrivate)
                return t;
            foreach (Type iType in t.GetTypeInfo().ImplementedInterfaces)
            {
                TypeInfo iTypeInfo = iType.GetTypeInfo();
                if (iTypeInfo.IsGenericType && iTypeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return iType;
            }
            if (typeof(IEnumerable).IsAssignableFrom(t))
                return typeof(IEnumerable);
            return t;
        }

        internal override Expression VisitConstant(ConstantExpression c)
        {
            EnumerableQuery sq = c.Value as EnumerableQuery;
            if (sq != null)
            {
                if (sq.Enumerable != null)
                {
                    Type t = GetPublicType(sq.Enumerable.GetType());
                    return Expression.Constant(sq.Enumerable, t);
                }
                Expression exp = sq.Expression;
                if (exp != c)
                    return Visit(exp);
            }
            return c;
        }

        internal override Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        private static volatile ILookup<string, MethodInfo> s_seqMethods;
        private static MethodInfo FindEnumerableMethod(string name, ReadOnlyCollection<Expression> args, params Type[] typeArgs)
        {
            if (s_seqMethods == null)
            {
                s_seqMethods = typeof(Enumerable).GetStaticMethods().ToLookup(m => m.Name);
            }
            MethodInfo mi = s_seqMethods[name].FirstOrDefault(m => ArgsMatch(m, args, typeArgs));
            if (mi == null)
                throw Error.NoMethodOnTypeMatchingArguments(name, typeof(Enumerable));
            if (typeArgs != null)
                return mi.MakeGenericMethod(typeArgs);
            return mi;
        }

        internal static MethodInfo FindMethod(Type type, string name, ReadOnlyCollection<Expression> args, Type[] typeArgs)
        {
            using (IEnumerator<MethodInfo> en = type.GetStaticMethods().Where(m => m.Name == name).GetEnumerator())
            {
                if (!en.MoveNext())
                    throw Error.NoMethodOnType(name, type);
                do
                {
                    MethodInfo mi = en.Current;
                    if (ArgsMatch(mi, args, typeArgs))
                        return (typeArgs != null) ? mi.MakeGenericMethod(typeArgs) : mi;
                } while (en.MoveNext());
            }
            throw Error.NoMethodOnTypeMatchingArguments(name, type);
        }

        private static bool ArgsMatch(MethodInfo m, ReadOnlyCollection<Expression> args, Type[] typeArgs)
        {
            ParameterInfo[] mParams = m.GetParameters();
            if (mParams.Length != args.Count)
                return false;
            if (!m.IsGenericMethod && typeArgs != null && typeArgs.Length > 0)
            {
                return false;
            }
            if (!m.IsGenericMethodDefinition && m.IsGenericMethod && m.ContainsGenericParameters)
            {
                m = m.GetGenericMethodDefinition();
            }
            if (m.IsGenericMethodDefinition)
            {
                if (typeArgs == null || typeArgs.Length == 0)
                    return false;
                if (m.GetGenericArguments().Length != typeArgs.Length)
                    return false;
                m = m.MakeGenericMethod(typeArgs);
                mParams = m.GetParameters();
            }
            for (int i = 0, n = args.Count; i < n; i++)
            {
                Type parameterType = mParams[i].ParameterType;
                if (parameterType == null)
                    return false;
                if (parameterType.IsByRef)
                    parameterType = parameterType.GetElementType();
                Expression arg = args[i];
                if (!parameterType.IsAssignableFrom(arg.Type))
                {
                    if (arg.NodeType == ExpressionType.Quote)
                    {
                        arg = ((UnaryExpression)arg).Operand;
                    }
                    if (!parameterType.IsAssignableFrom(arg.Type) &&
                        !parameterType.IsAssignableFrom(StripExpression(arg.Type)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static Type StripExpression(Type type)
        {
            bool isArray = type.IsArray;
            Type tmp = isArray ? type.GetElementType() : type;
            Type eType = TypeHelper.FindGenericType(typeof(Expression<>), tmp);
            if (eType != null)
                tmp = eType.GetGenericArguments()[0];
            if (isArray)
            {
                int rank = type.GetArrayRank();
                return (rank == 1) ? tmp.MakeArrayType() : tmp.MakeArrayType(rank);
            }
            return type;
        }
    }
}
