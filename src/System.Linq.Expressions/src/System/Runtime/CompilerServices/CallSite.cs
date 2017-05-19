// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.CachedReflectionInfo;

namespace System.Runtime.CompilerServices
{
    //
    // A CallSite provides a fast mechanism for call-site caching of dynamic dispatch
    // behavior. Each site will hold onto a delegate that provides a fast-path dispatch
    // based on previous types that have been seen at the call-site. This delegate will
    // call UpdateAndExecute if it is called with types that it hasn't seen before.
    // Updating the binding will typically create (or lookup) a new delegate
    // that supports fast-paths for both the new type and for any types that
    // have been seen previously.
    //
    // DynamicSites will generate the fast-paths specialized for sets of runtime argument
    // types. However, they will generate exactly the right amount of code for the types
    // that are seen in the program so that int addition will remain as fast as it would
    // be with custom implementation of the addition, and the user-defined types can be
    // as fast as ints because they will all have the same optimal dynamically generated
    // fast-paths.
    //
    // DynamicSites don't encode any particular caching policy, but use their
    // CallSiteBinding to encode a caching policy.
    //


    /// <summary>
    /// A Dynamic Call Site base class. This type is used as a parameter type to the
    /// dynamic site targets. The first parameter of the delegate (T) below must be
    /// of this type.
    /// </summary>
    public class CallSite
    {
        /// <summary>
        /// String used for generated CallSite methods.
        /// </summary>
        internal const string CallSiteTargetMethodName = "CallSite.Target";

        /// <summary>
        /// Cache of CallSite constructors for a given delegate type.
        /// </summary>
        private static volatile CacheDict<Type, Func<CallSiteBinder, CallSite>> s_siteCtors;

        /// <summary>
        /// The Binder responsible for binding operations at this call site.
        /// This binder is invoked by the UpdateAndExecute below if all Level 0,
        /// Level 1 and Level 2 caches experience cache miss.
        /// </summary>
        internal readonly CallSiteBinder _binder;

        // only CallSite<T> derives from this
        internal CallSite(CallSiteBinder binder)
        {
            _binder = binder;
        }

        /// <summary>
        /// Used by Matchmaker sites to indicate rule match.
        /// </summary>
        internal bool _match;

        /// <summary>
        /// Class responsible for binding dynamic operations on the dynamic site.
        /// </summary>
        public CallSiteBinder Binder => _binder;

        /// <summary>
        /// Creates a CallSite with the given delegate type and binder.
        /// </summary>
        /// <param name="delegateType">The CallSite delegate type.</param>
        /// <param name="binder">The CallSite binder.</param>
        /// <returns>The new CallSite.</returns>
        public static CallSite Create(Type delegateType, CallSiteBinder binder)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate))) throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();

            CacheDict<Type, Func<CallSiteBinder, CallSite>> ctors = s_siteCtors;
            if (ctors == null) {
                // It's okay to just set this, worst case we're just throwing away some data
                s_siteCtors = ctors = new CacheDict<Type, Func<CallSiteBinder, CallSite>>(100);
            }

            Func<CallSiteBinder, CallSite> ctor;
            MethodInfo method = null;
            if (!ctors.TryGetValue(delegateType, out ctor))
            {
                method = typeof(CallSite<>).MakeGenericType(delegateType).GetMethod(nameof(Create));

                if (delegateType.CanCache())
                {
                    ctor = (Func<CallSiteBinder, CallSite>)method.CreateDelegate(typeof(Func<CallSiteBinder, CallSite>));
                    ctors.Add(delegateType, ctor);
                }
            }

            if (ctor != null)
            {
                return ctor(binder);
            }

            // slow path
            return (CallSite)method.Invoke(null, new object[] { binder });
        }
    }

    /// <summary>
    /// Dynamic site type.
    /// </summary>
    /// <typeparam name="T">The delegate type.</typeparam>
    public class CallSite<T> : CallSite where T : class
    {
        /// <summary>
        /// The update delegate. Called when the dynamic site experiences cache miss.
        /// </summary>
        /// <returns>The update delegate.</returns>
        public T Update
        {
            get
            {
                // if this site is set up for match making, then use NoMatch as an Update
                if (_match)
                {
                    Debug.Assert(s_cachedNoMatch != null, "all normal sites should have Update cached once there is an instance.");
                    return s_cachedNoMatch;
                }
                else
                {
                    Debug.Assert(s_cachedUpdate != null, "all normal sites should have Update cached once there is an instance.");
                    return s_cachedUpdate;
                }
            }
        }

        /// <summary>
        /// The Level 0 cache - a delegate specialized based on the site history.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public T Target;

        /// <summary>
        /// The Level 1 cache - a history of the dynamic site.
        /// </summary>
        internal T[] Rules;

        // Cached update delegate for all sites with a given T
        private static T s_cachedUpdate;

        // Cached noMatch delegate for all sites with a given T
        private static volatile T s_cachedNoMatch;

        private CallSite(CallSiteBinder binder)
            : base(binder)
        {
            Target = GetUpdateDelegate();
        }

        private CallSite()
            : base(null)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal CallSite<T> CreateMatchMaker()
        {
            return new CallSite<T>();
        }

        /// <summary>
        /// Creates an instance of the dynamic call site, initialized with the binder responsible for the
        /// runtime binding of the dynamic operations at this call site.
        /// </summary>
        /// <param name="binder">The binder responsible for the runtime binding of the dynamic operations at this call site.</param>
        /// <returns>The new instance of dynamic call site.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static CallSite<T> Create(CallSiteBinder binder)
        {
            if (!typeof(T).IsSubclassOf(typeof(MulticastDelegate))) throw System.Linq.Expressions.Error.TypeMustBeDerivedFromSystemDelegate();
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            return new CallSite<T>(binder);
        }

        private T GetUpdateDelegate()
        {
            // This is intentionally non-static to speed up creation - in particular MakeUpdateDelegate
            // as static generic methods are more expensive than instance methods.  We call a ref helper
            // so we only access the generic static field once.
            return GetUpdateDelegate(ref s_cachedUpdate);
        }

        private T GetUpdateDelegate(ref T addr)
        {
            if (addr == null)
            {
                // reduce creation cost by not using Interlocked.CompareExchange.  Calling I.CE causes
                // us to spend 25% of our creation time in JIT_GenericHandle.  Instead we'll rarely
                // create 2 delegates with no other harm caused.
                addr = MakeUpdateDelegate();
            }
            return addr;
        }

        /// <summary>
        /// Clears the rule cache ... used by the call site tests.
        /// </summary>
        private void ClearRuleCache()
        {
            // make sure it initialized/atomized etc...
            Binder.GetRuleCache<T>();

            Dictionary<Type, object> cache = Binder.Cache;

            if (cache != null)
            {
                lock (cache)
                {
                    cache.Clear();
                }
            }
        }

        private const int MaxRules = 10;

        internal void AddRule(T newRule)
        {
            T[] rules = Rules;
            if (rules == null)
            {
                Rules = new[] { newRule };
                return;
            }

            T[] temp;
            if (rules.Length < (MaxRules - 1))
            {
                temp = new T[rules.Length + 1];
                Array.Copy(rules, 0, temp, 1, rules.Length);
            }
            else
            {
                temp = new T[MaxRules];
                Array.Copy(rules, 0, temp, 1, MaxRules - 1);
            }
            temp[0] = newRule;
            Rules = temp;
        }

        // moves rule +2 up.
        internal void MoveRule(int i)
        {
            if (i > 1)
            {
                T[] rules = Rules;
                T rule = rules[i];

                rules[i] = rules[i - 1];
                rules[i - 1] = rules[i - 2];
                rules[i - 2] = rule;
            }
        }

        internal T MakeUpdateDelegate()
        {
#if !FEATURE_COMPILE
            Type target = typeof(T);
            MethodInfo invoke = target.GetInvokeMethod();

            s_cachedNoMatch = CreateCustomNoMatchDelegate(invoke);
            return CreateCustomUpdateDelegate(invoke);
#else
            Type target = typeof(T);
            Type[] args;
            MethodInfo invoke = target.GetInvokeMethod();

            if (target.IsGenericType && IsSimpleSignature(invoke, out args))
            {
                MethodInfo method = null;
                MethodInfo noMatchMethod = null;

                if (invoke.ReturnType == typeof(void))
                {
                    if (target == System.Linq.Expressions.Compiler.DelegateHelpers.GetActionType(args.AddFirst(typeof(CallSite))))
                    {
                        method = typeof(UpdateDelegates).GetMethod("UpdateAndExecuteVoid" + args.Length, BindingFlags.NonPublic | BindingFlags.Static);
                        noMatchMethod = typeof(UpdateDelegates).GetMethod("NoMatchVoid" + args.Length, BindingFlags.NonPublic | BindingFlags.Static);
                    }
                }
                else
                {
                    if (target == System.Linq.Expressions.Compiler.DelegateHelpers.GetFuncType(args.AddFirst(typeof(CallSite))))
                    {
                        method = typeof(UpdateDelegates).GetMethod("UpdateAndExecute" + (args.Length - 1), BindingFlags.NonPublic | BindingFlags.Static);
                        noMatchMethod = typeof(UpdateDelegates).GetMethod("NoMatch" + (args.Length - 1), BindingFlags.NonPublic | BindingFlags.Static);
                    }
                }
                if (method != null)
                {
                    s_cachedNoMatch = (T)(object)CreateDelegateHelper(target, noMatchMethod.MakeGenericMethod(args));
                    return (T)(object)CreateDelegateHelper(target, method.MakeGenericMethod(args));
                }
            }

            s_cachedNoMatch = CreateCustomNoMatchDelegate(invoke);
            return CreateCustomUpdateDelegate(invoke);
#endif
        }

#if FEATURE_COMPILE
        // This needs to be SafeCritical to allow access to
        // internal types from user code as generic parameters.
        //
        // It's safe for a few reasons:
        //   1. The internal types are coming from a lower trust level (app code)
        //   2. We got the internal types from our own generic parameter: T
        //   3. The UpdateAndExecute methods don't do anything with the types,
        //      we just want the CallSite args to be strongly typed to avoid
        //      casting.
        //   4. Works on desktop CLR with AppDomain that has only Execute
        //      permission. In theory it might require RestrictedMemberAccess,
        //      but it's unclear because we have tests passing without RMA.
        //
        // When Silverlight gets RMA we may be able to remove this.
        [System.Security.SecuritySafeCritical]
        private static Delegate CreateDelegateHelper(Type delegateType, MethodInfo method)
        {
            return method.CreateDelegate(delegateType);
        }

        private static bool IsSimpleSignature(MethodInfo invoke, out Type[] sig)
        {
            ParameterInfo[] pis = invoke.GetParametersCached();
            ContractUtils.Requires(pis.Length > 0 && pis[0].ParameterType == typeof(CallSite), nameof(T));

            Type[] args = new Type[invoke.ReturnType != typeof(void) ? pis.Length : pis.Length - 1];
            bool supported = true;

            for (int i = 1; i < pis.Length; i++)
            {
                ParameterInfo pi = pis[i];
                if (pi.IsByRefParameter())
                {
                    supported = false;
                }
                args[i - 1] = pi.ParameterType;
            }
            if (invoke.ReturnType != typeof(void))
            {
                args[args.Length - 1] = invoke.ReturnType;
            }
            sig = args;
            return supported;
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private T CreateCustomUpdateDelegate(MethodInfo invoke)
        {
            Type returnType = invoke.GetReturnType();
            bool isVoid = returnType == typeof(void);

            var body = new ArrayBuilder<Expression>(13);
            var vars = new ArrayBuilder<ParameterExpression>(8 + (isVoid ? 0 : 1));

            ParameterExpression[] @params = Array.ConvertAll(invoke.GetParametersCached(), p => Expression.Parameter(p.ParameterType, p.Name));
            LabelTarget @return = Expression.Label(returnType);
            Type[] typeArgs = new[] { typeof(T) };

            ParameterExpression site = @params[0];
            ParameterExpression[] arguments = @params.RemoveFirst();

            ParameterExpression @this = Expression.Variable(typeof(CallSite<T>), "this");
            vars.UncheckedAdd(@this);
            body.UncheckedAdd(Expression.Assign(@this, Expression.Convert(site, @this.Type)));

            ParameterExpression applicable = Expression.Variable(typeof(T[]), "applicable");
            vars.UncheckedAdd(applicable);

            ParameterExpression rule = Expression.Variable(typeof(T), "rule");
            vars.UncheckedAdd(rule);

            ParameterExpression originalRule = Expression.Variable(typeof(T), "originalRule");
            vars.UncheckedAdd(originalRule);

            Expression target = Expression.Field(@this, nameof(Target));
            body.UncheckedAdd(Expression.Assign(originalRule, target));

            ParameterExpression result = null;
            if (!isVoid)
            {
                vars.UncheckedAdd(result = Expression.Variable(@return.Type, "result"));
            }

            ParameterExpression count = Expression.Variable(typeof(int), "count");
            vars.UncheckedAdd(count);
            ParameterExpression index = Expression.Variable(typeof(int), "index");
            vars.UncheckedAdd(index);

            body.UncheckedAdd(
                Expression.Assign(
                    site,
                    Expression.Call(
                        CallSiteOps_CreateMatchmaker.MakeGenericMethod(typeArgs),
                        @this
                    )
                )
            );

            Expression processRule;

            Expression getMatch = Expression.Call(CallSiteOps_GetMatch, site);

            Expression resetMatch = Expression.Call(CallSiteOps_ClearMatch, site);

            Expression invokeRule = Expression.Invoke(rule, new TrueReadOnlyCollection<Expression>(@params));

            Expression onMatch = Expression.Call(
                CallSiteOps_UpdateRules.MakeGenericMethod(typeArgs),
                @this,
                index
            );

            if (isVoid)
            {
                processRule = Expression.Block(
                    invokeRule,
                    Expression.IfThen(
                        getMatch,
                        Expression.Block(onMatch, Expression.Return(@return))
                    )
                );
            }
            else
            {
                processRule = Expression.Block(
                    Expression.Assign(result, invokeRule),
                    Expression.IfThen(
                        getMatch,
                        Expression.Block(onMatch, Expression.Return(@return, result))
                    )
                );
            }

            Expression getApplicableRuleAtIndex = Expression.Assign(rule, Expression.ArrayAccess(applicable, new TrueReadOnlyCollection<Expression>(index)));
            Expression getRule = getApplicableRuleAtIndex;

            LabelTarget @break = Expression.Label();

            Expression breakIfDone = Expression.IfThen(
                Expression.Equal(index, count),
                Expression.Break(@break)
            );

            Expression incrementIndex = Expression.PreIncrementAssign(index);

            body.UncheckedAdd(
                Expression.IfThen(
                    Expression.NotEqual(
                        Expression.Assign(
                            applicable,
                            Expression.Call(
                                CallSiteOps_GetRules.MakeGenericMethod(typeArgs),
                                @this
                            )
                        ),
                        Expression.Constant(null, applicable.Type)
                    ),
                    Expression.Block(
                        Expression.Assign(count, Expression.ArrayLength(applicable)),
                        Expression.Assign(index, Utils.Constant(0)),
                        Expression.Loop(
                            Expression.Block(
                                breakIfDone,
                                getRule,
                                Expression.IfThen(
                                    Expression.NotEqual(
                                        Expression.Convert(rule, typeof(object)),
                                        Expression.Convert(originalRule, typeof(object))
                                    ),
                                    Expression.Block(
                                        Expression.Assign(
                                            target,
                                            rule
                                        ),
                                        processRule,
                                        resetMatch
                                    )
                                ),
                                incrementIndex
                            ),
                            @break,
                            @continue: null
                        )
                    )
                )
            );

            ////
            //// Level 2 cache lookup
            ////
            //
            ////
            //// Any applicable rules in level 2 cache?
            ////
            ParameterExpression cache = Expression.Variable(typeof(RuleCache<T>), "cache");
            vars.UncheckedAdd(cache);

            body.UncheckedAdd(
                Expression.Assign(
                    cache,
                    Expression.Call(CallSiteOps_GetRuleCache.MakeGenericMethod(typeArgs), @this)
                )
            );

            body.UncheckedAdd(
                Expression.Assign(
                    applicable,
                    Expression.Call(CallSiteOps_GetCachedRules.MakeGenericMethod(typeArgs), cache)
                )
            );

            // L2 invokeRule is different (no onMatch)
            if (isVoid)
            {
                processRule = Expression.Block(
                    invokeRule,
                    Expression.IfThen(
                        getMatch,
                        Expression.Return(@return)
                    )
                );
            }
            else
            {
                processRule = Expression.Block(
                    Expression.Assign(result, invokeRule),
                    Expression.IfThen(
                        getMatch,
                        Expression.Return(@return, result)
                    )
                );
            }

            Expression tryRule = Expression.TryFinally(
                processRule,
                Expression.IfThen(
                    getMatch,
                    Expression.Block(
                        Expression.Call(CallSiteOps_AddRule.MakeGenericMethod(typeArgs), @this, rule),
                        Expression.Call(CallSiteOps_MoveRule.MakeGenericMethod(typeArgs), cache, rule, index)
                    )
                )
            );

            getRule = Expression.Assign(
                target,
                getApplicableRuleAtIndex
            );

            body.UncheckedAdd(Expression.Assign(index, Utils.Constant(0)));
            body.UncheckedAdd(Expression.Assign(count, Expression.ArrayLength(applicable)));
            body.UncheckedAdd(
                Expression.Loop(
                    Expression.Block(
                        breakIfDone,
                        getRule,
                        tryRule,
                        resetMatch,
                        incrementIndex
                    ),
                    @break,
                    @continue: null
                )
            );

            ////
            //// Miss on Level 0, 1 and 2 caches. Create new rule
            ////
            body.UncheckedAdd(Expression.Assign(rule, Expression.Constant(null, rule.Type)));

            ParameterExpression args = Expression.Variable(typeof(object[]), "args");
            Expression[] argsElements = Array.ConvertAll(arguments, p => Convert(p, typeof(object)));
            vars.UncheckedAdd(args);
            body.UncheckedAdd(
                Expression.Assign(
                    args,
                    Expression.NewArrayInit(typeof(object), new TrueReadOnlyCollection<Expression>(argsElements))
                )
            );

            Expression setOldTarget = Expression.Assign(
                target,
                originalRule
            );

            getRule = Expression.Assign(
                target,
                Expression.Assign(
                    rule,
                    Expression.Call(
                        CallSiteOps_Bind.MakeGenericMethod(typeArgs),
                        Expression.Property(@this, nameof(Binder)),
                        @this,
                        args
                    )
                )
            );

            tryRule = Expression.TryFinally(
                processRule,
                Expression.IfThen(
                    getMatch,
                    Expression.Call(
                        CallSiteOps_AddRule.MakeGenericMethod(typeArgs),
                        @this,
                        rule
                    )
                )
            );

            body.UncheckedAdd(
                Expression.Loop(
                    Expression.Block(setOldTarget, getRule, tryRule, resetMatch),
                    @break: null,
                    @continue: null
                )
            );

            body.UncheckedAdd(Expression.Default(@return.Type));

            Expression<T> lambda = Expression.Lambda<T>(
                Expression.Label(
                    @return,
                    Expression.Block(
                        vars.ToReadOnly(),
                        body.ToReadOnly()
                    )
                ),
                CallSiteTargetMethodName,
                true, // always compile the rules with tail call optimization
                new TrueReadOnlyCollection<ParameterExpression>(@params)
            );

            // Need to compile with forceDynamic because T could be invisible,
            // or one of the argument types could be invisible
            return lambda.Compile();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private T CreateCustomNoMatchDelegate(MethodInfo invoke)
        {
            ParameterExpression[] @params = Array.ConvertAll(invoke.GetParametersCached(), p => Expression.Parameter(p.ParameterType, p.Name));
            return Expression.Lambda<T>(
                Expression.Block(
                    Expression.Call(
                        typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.SetNotMatched)),
                        @params[0]
                    ),
                    Expression.Default(invoke.GetReturnType())
                ),
                new TrueReadOnlyCollection<ParameterExpression>(@params)
            ).Compile();
        }

        private static Expression Convert(Expression arg, Type type)
        {
            if (TypeUtils.AreReferenceAssignable(type, arg.Type))
            {
                return arg;
            }
            return Expression.Convert(arg, type);
        }
    }
}
