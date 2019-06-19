﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.CSharp.RuntimeBinder
{
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class DynamicBindingFailedException : Exception
    {
        public DynamicBindingFailedException()
            : base()
        {
        }

        private DynamicBindingFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        { 
        }
    }

    internal sealed class GetMemberValueBinder : GetMemberBinder
    {
        public GetMemberValueBinder(string name, bool ignoreCase)
            : base(name, ignoreCase)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject self, DynamicMetaObject onBindingError)
        {
            if (onBindingError == null)
            {
                var v = new List<DynamicMetaObject> { self };
                var error = new DynamicMetaObject(System.Linq.Expressions.Expression.Throw(
                    System.Linq.Expressions.Expression.Constant(new DynamicBindingFailedException(), typeof(Exception)), typeof(object)), System.Dynamic.BindingRestrictions.Combine(v));
                return error;
            }
            return onBindingError;
        }
    }

    internal sealed class DynamicMetaObjectProviderDebugView
    {
        [System.Diagnostics.DebuggerDisplay("{value}", Name = "{name, nq}", Type = "{type, nq}")]
        internal class DynamicProperty
        {
            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly string name;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly object value;

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            private readonly string type;

            public DynamicProperty(string name, object value)
            {
                this.name = name;
                this.value = value;
                this.type = value == null ? "<null>" : value.GetType().ToString();
            }
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private IList<KeyValuePair<string, object>> results = null;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private object obj;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        internal DynamicProperty[] Items
        {
            get
            {
                if (results == null || results.Count == 0)
                {
                    results = QueryDynamicObject(obj);
                    if (results == null || results.Count == 0)
                    {
                        throw new DynamicDebugViewEmptyException();
                    }
                }
                DynamicProperty[] pairArray = new DynamicProperty[results.Count];
                for (int i = 0; i < results.Count; i++)
                {
                    pairArray[i] = new DynamicProperty(results[i].Key, results[i].Value);
                }
                return pairArray;
            }
        }

        public DynamicMetaObjectProviderDebugView(object arg)
        {
            this.obj = arg;
        }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private static readonly ParameterExpression parameter = Expression.Parameter(typeof(object), "debug");

        public static object TryEvalBinaryOperators<T1, T2>(
            T1 arg1, 
            T2 arg2, 
            CSharpArgumentInfoFlags arg1Flags,
            CSharpArgumentInfoFlags arg2Flags,
            ExpressionType opKind,
            Type accessibilityContext)
        {
            CSharpArgumentInfo arg1Info = CSharpArgumentInfo.Create(arg1Flags, null);
            CSharpArgumentInfo arg2Info = CSharpArgumentInfo.Create(arg2Flags, null);

            CSharpBinaryOperationBinder binder = new CSharpBinaryOperationBinder(
                opKind,
                false, // isChecked
                CSharpBinaryOperationFlags.None,
                accessibilityContext,
                new CSharpArgumentInfo[] { arg1Info, arg2Info });

            var site = CallSite<Func<CallSite, T1, T2, object>>.Create(binder);
            return site.Target(site, arg1, arg2);
        }

        public static object TryEvalUnaryOperators<T>(T obj, ExpressionType oper, Type accessibilityContext)
        {
            if (oper == ExpressionType.IsTrue || oper == ExpressionType.IsFalse)
            {
                var trueFalseSite = CallSite<Func<CallSite, T, bool>>
                    .Create(new Microsoft.CSharp.RuntimeBinder.CSharpUnaryOperationBinder(oper,
                        false,
                        accessibilityContext,
                        new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
                return trueFalseSite.Target(trueFalseSite, obj);
            }

            var site = CallSite<Func<CallSite, T, object>>
                .Create(new Microsoft.CSharp.RuntimeBinder.CSharpUnaryOperationBinder(oper,
                    false,
                    accessibilityContext,
                    new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
            return site.Target(site, obj);
        }

        public static K TryEvalCast<T, K>(T obj, Type type, CSharpBinderFlags kind, Type accessibilityContext)
        {
            var site = CallSite<Func<CallSite, T, K>>.Create(Binder.Convert(kind, type, accessibilityContext));
            return site.Target(site, obj);
        }

        /// <summary>
        /// Creates array of types that describes delegate's signature and array of 
        /// CSharpArgumentInfoFlags that describe each of the arguments.
        /// </summary>
        private static void CreateDelegateSignatureAndArgumentInfos(
            object[] args,
            Type[] argTypes,
            CSharpArgumentInfoFlags[] argFlags,
            out Type[] delegateSignatureTypes,
            out CSharpArgumentInfo[] argInfos)
        {
            int numberOfArguments = args.Length;
            Debug.Assert((numberOfArguments == argTypes.Length) && (numberOfArguments == argFlags.Length), "Argument arrays size mismatch.");

            delegateSignatureTypes = new Type[numberOfArguments + 2];
            delegateSignatureTypes[0] = typeof(CallSite);

            argInfos = new CSharpArgumentInfo[numberOfArguments];

            for (int i = 0; i < numberOfArguments; i++)
            {
                if (argTypes[i] != null)
                {
                    delegateSignatureTypes[i + 1] = argTypes[i];
                }
                else if (args[i] != null)
                {
                    delegateSignatureTypes[i + 1] = args[i].GetType();
                }
                else
                {
                    delegateSignatureTypes[i + 1] = typeof(object);
                }

                argInfos[i] = CSharpArgumentInfo.Create(argFlags[i], null);
            }

            delegateSignatureTypes[numberOfArguments + 1] = typeof(object); // type of return value
        }

        /// <summary>
        /// Creates a delegate based on type array that describe its signature and invokes it.
        /// </summary>
        /// <returns>Result of invoking the delegate.</returns>
        private static object CreateDelegateAndInvoke(Type[] delegateSignatureTypes, CallSiteBinder binder, object[] args)
        {
            Type delegateType = Expression.GetDelegateType(delegateSignatureTypes);
            var site = CallSite.Create(delegateType, binder);

            Delegate target = (Delegate)site.GetType().GetField("Target").GetValue(site);

            object[] argsWithSite = new object[args.Length + 1];
            argsWithSite[0] = site;
            args.CopyTo(argsWithSite, 1);

            object result = target.DynamicInvoke(argsWithSite);
            return result;
        }

        /// <summary>
        /// DynamicOperatorRewriter in EE generates call to this method to dynamically invoke a method.
        /// </summary>
        /// <param name="methodArgs">Array that contains method arguments. The first element is an object on which method should be called.</param>
        /// <param name="argTypes">Type of each argument in methodArgs.</param>
        /// <param name="argFlags">Flags describing each argument.</param>
        /// <param name="methodName">Name of a method to invoke.</param>
        /// <param name="accessibilityContext">Type that determines context in which method should be called.</param>
        /// <param name="typeArguments">Generic type arguments if there are any.</param>
        /// <returns>Result of method invocation.</returns>
        public static object TryEvalMethodVarArgs(
            object[] methodArgs, 
            Type[] argTypes,
            CSharpArgumentInfoFlags[] argFlags, 
            string methodName, 
            Type accessibilityContext, 
            Type[] typeArguments)
        {
            Type[] delegateSignatureTypes = null;
            CSharpArgumentInfo[] argInfos = null;

            CreateDelegateSignatureAndArgumentInfos(
                methodArgs,
                argTypes,
                argFlags,
                out delegateSignatureTypes,
                out argInfos);

            CallSiteBinder binder;
            if (String.IsNullOrEmpty(methodName))
            {
                //null or empty indicates delegate invocation.
                binder = new CSharpInvokeBinder(
                    CSharpCallFlags.ResultDiscarded, 
                    accessibilityContext,
                    argInfos);
            }
            else
            {
                binder = new CSharpInvokeMemberBinder(
                    CSharpCallFlags.ResultDiscarded,
                    methodName,
                    accessibilityContext,
                    typeArguments,
                    argInfos);
            }

            return CreateDelegateAndInvoke(delegateSignatureTypes, binder, methodArgs);
        }

        /// <summary>
        /// DynamicOperatorRewriter in EE generates call to this method to dynamically invoke a property getter
        /// with no arguments.
        /// </summary>
        /// <typeparam name="T">Type of object on which property is defined.</typeparam>
        /// <param name="obj">Object on which property is defined.</param>
        /// <param name="propName">Name of a property to invoke.</param>
        /// <param name="accessibilityContext">Type that determines context in which method should be called.</param>
        /// <param name="isResultIndexed">Determines if COM binder should return a callable object.</param>
        /// <returns>Result of property invocation.</returns>
        public static object TryGetMemberValue<T>(T obj, string propName, Type accessibilityContext, bool isResultIndexed)
        {
            // In most cases it's ok to use CSharpArgumentInfoFlags.None since target of property call is dynamic.
            // The only possible case when target is not dynamic but we still treat is as dynamic access is when 
            // one of arguments is dynamic. This is only possible for indexed properties since we call this method and 
            // TryGetMemberValueVarArgs afterwards. 
             
            CSharpGetMemberBinder binder = new CSharpGetMemberBinder(
                propName,
                isResultIndexed,
                accessibilityContext,
                new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

            var site = CallSite<Func<CallSite, T, object>>.Create(binder);
            return site.Target(site, obj);
        }

        /// <summary>
        /// DynamicOperatorRewriter in EE generates call to this method to dynamically invoke a property/indexer getter.
        /// </summary>
        /// <param name="propArgs">Array that contains property arguments. The first element is an object on 
        /// which indexer should be called or call to TryGetMemberValue that selects the right property in case of
        /// indexed properties.</param>
        /// <param name="argTypes">Type of each argument in propArgs.</param>
        /// <param name="argFlags">Flags describing each argument.</param>
        /// <param name="accessibilityContext">Type that determines context in which method should be called.</param>
        /// <returns>Result of property invocation.</returns>
        public static object TryGetMemberValueVarArgs(
            object[] propArgs,
            Type[] argTypes,
            CSharpArgumentInfoFlags[] argFlags,
            Type accessibilityContext)
        {
            Type[] delegateSignatureTypes = null;
            CSharpArgumentInfo[] argInfos = null;

            CreateDelegateSignatureAndArgumentInfos(
                propArgs,
                argTypes,
                argFlags,
                out delegateSignatureTypes,
                out argInfos);

            CallSiteBinder binder = new CSharpGetIndexBinder(accessibilityContext, argInfos);

            return CreateDelegateAndInvoke(delegateSignatureTypes, binder, propArgs);
        }

        /// <summary>
        /// DynamicOperatorRewriter in EE generates call to this method to dynamically invoke a property setter
        /// with no arguments.
        /// </summary>
        /// <typeparam name="TObject">Type of object on which property is defined.</typeparam>
        /// <typeparam name="TValue">Type of value property needs to be set to.</typeparam>
        /// <param name="obj">Object on which property is defined.</param>
        /// <param name="propName">Name of a property to invoke.</param>
        /// <param name="value">Value property needs to be set to.</param>
        /// <param name="valueFlags"></param>
        /// <param name="accessibilityContext">Type that determines context in which method should be called.</param>
        /// <returns>Result of property invocation.</returns>
        public static object TrySetMemberValue<TObject, TValue>(
            TObject obj, 
            string propName, 
            TValue value, 
            CSharpArgumentInfoFlags valueFlags, 
            Type accessibilityContext)
        {
            CSharpArgumentInfo targetArgInfo = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
            CSharpArgumentInfo valueArgInfo = CSharpArgumentInfo.Create(valueFlags, null);

            CSharpSetMemberBinder binder = new CSharpSetMemberBinder(
                propName,
                false, // isCompoundAssignment 
                false, // isChecked 
                accessibilityContext,
                new CSharpArgumentInfo[] { targetArgInfo, valueArgInfo });

            var site = CallSite<Func<CallSite, TObject, TValue, object>>.Create(binder);
            return site.Target(site, obj, value);
        }

        /// <summary>
        /// DynamicOperatorRewriter in EE generates call to this method to dynamically invoke a property/indexer setter.
        /// </summary>
        /// <param name="propArgs">Array that contains property arguments. The first element is an object on 
        /// which indexer should be called or call to TrySetMemberValue that selects the right property in case of
        /// indexed properties. The last argument is value that property should be set to.</param>
        /// <param name="argTypes">Type of each argument in propArgs.</param>
        /// <param name="argFlags">Flags describing each argument.</param>
        /// <param name="accessibilityContext">Type that determines context in which method should be called.</param>
        /// <returns>Result of property invocation.</returns>
        public static object TrySetMemberValueVarArgs(
            object[] propArgs,
            Type[] argTypes,
            CSharpArgumentInfoFlags[] argFlags,
            Type accessibilityContext)
        {
            Type[] delegateSignatureTypes = null;
            CSharpArgumentInfo[] argInfos = null;

            CreateDelegateSignatureAndArgumentInfos(
                propArgs,
                argTypes,
                argFlags,
                out delegateSignatureTypes,
                out argInfos);

            CallSiteBinder binder = new CSharpSetIndexBinder(/*isCompoundAssignment */ false, /* isChecked */ false, accessibilityContext, argInfos);

            return CreateDelegateAndInvoke(delegateSignatureTypes, binder, propArgs);
        }

        //Called when we don't know if the member is a property or a method
        internal static object TryGetMemberValue(object obj, string name, bool ignoreException)
        {
            // if you want to ignore case for VB, this is how you set it .. make it a member and add a ctor to init it
            bool ignoreCase = false;
            object value = null;

            var site = CallSite<Func<CallSite, object, object>>.Create(new GetMemberValueBinder(name, ignoreCase));

            try
            {
                value = site.Target(site, obj);
            }
            catch (DynamicBindingFailedException exp)
            {
                if (ignoreException)
                    value = null;
                else
                    throw exp;
            }
            catch (MissingMemberException exp)
            {
                if (ignoreException)
                    value = SR.GetValueonWriteOnlyProperty;
                else
                    throw exp;
            }
            return value;
        }

        private static IList<KeyValuePair<string, object>> QueryDynamicObject(object obj)
        {
            IDynamicMetaObjectProvider ido = obj as IDynamicMetaObjectProvider;
            if (ido != null)
            {
                DynamicMetaObject mo = ido.GetMetaObject(parameter);
                List<string> names = new List<string>(mo.GetDynamicMemberNames());
                names.Sort();
                if (names != null)
                {
                    var result = new List<KeyValuePair<string, object>>();
                    foreach (string name in names)
                    {
                        object value;
                        if ((value = TryGetMemberValue(obj, name, true)) != null)
                        {
                            result.Add(new KeyValuePair<string, object>(name, value));
                        }
                    }
                    return result;
                }
            }
            return new KeyValuePair<string, object>[0];
        }

        [Serializable]
        internal class DynamicDebugViewEmptyException : Exception
        {
            public DynamicDebugViewEmptyException()
            {
            }

            protected DynamicDebugViewEmptyException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            public string Empty
            {
                get
                {
                    return SR.EmptyDynamicView;
                }
            }
        }
    }
}
