// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

#if FEATURE_COMPILE
using System.Linq.Expressions.Compiler;
#endif 

namespace System.Runtime.CompilerServices
{
    //
    // A CallSite provides a fast mechanism for call-site caching of dynamic dispatch
    // behvaior. Each site will hold onto a delegate that provides a fast-path dispatch
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
        // Cache of CallSite constructors for a given delegate type
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
        /// used by Matchmaker sites to indicate rule match.
        /// </summary>
        internal bool _match;

        /// <summary>
        /// Class responsible for binding dynamic operations on the dynamic site.
        /// </summary>
        public CallSiteBinder Binder
        {
            get { return _binder; }
        }

        /// <summary>
        /// Creates a CallSite with the given delegate type and binder.
        /// </summary>
        /// <param name="delegateType">The CallSite delegate type.</param>
        /// <param name="binder">The CallSite binder.</param>
        /// <returns>The new CallSite.</returns>
        public static CallSite Create(Type delegateType, CallSiteBinder binder)
        {
            ContractUtils.RequiresNotNull(delegateType, "delegateType");
            ContractUtils.RequiresNotNull(binder, "binder");
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate))) throw Error.TypeMustBeDerivedFromSystemDelegate();

            var ctors = s_siteCtors;
            if (ctors == null) {
                // It's okay to just set this, worst case we're just throwing away some data
                s_siteCtors = ctors = new CacheDict<Type, Func<CallSiteBinder, CallSite>>(100);
            }

            Func<CallSiteBinder, CallSite> ctor;
            MethodInfo method = null;
            if (!ctors.TryGetValue(delegateType, out ctor))
            {
                method = typeof(CallSite<>).MakeGenericType(delegateType).GetMethod("Create");

                if (TypeUtils.CanCache(delegateType))
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
    public partial class CallSite<T> : CallSite where T : class
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
            if (!typeof(T).IsSubclassOf(typeof(MulticastDelegate))) throw Error.TypeMustBeDerivedFromSystemDelegate();
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

            var cache = Binder.Cache;

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
                var rules = Rules;
                var rule = rules[i];

                rules[i] = rules[i - 1];
                rules[i - 1] = rules[i - 2];
                rules[i - 2] = rule;
            }
        }

        internal T MakeUpdateDelegate()
        {
#if !FEATURE_COMPILE
            Type target = typeof(T);
            MethodInfo invoke = target.GetMethod("Invoke");

            s_cachedNoMatch = CreateCustomNoMatchDelegate(invoke);
            return CreateCustomUpdateDelegate(invoke);
#else
            Type target = typeof(T);
            Type[] args;
            MethodInfo invoke = target.GetMethod("Invoke");

            if (target.GetTypeInfo().IsGenericType && IsSimpleSignature(invoke, out args))
            {
                MethodInfo method = null;
                MethodInfo noMatchMethod = null;

                if (invoke.ReturnType == typeof(void))
                {
                    if (target == DelegateHelpers.GetActionType(args.AddFirst(typeof(CallSite))))
                    {
                        method = typeof(UpdateDelegates).GetMethod("UpdateAndExecuteVoid" + args.Length, BindingFlags.NonPublic | BindingFlags.Static);
                        noMatchMethod = typeof(UpdateDelegates).GetMethod("NoMatchVoid" + args.Length, BindingFlags.NonPublic | BindingFlags.Static);
                    }
                }
                else
                {
                    if (target == DelegateHelpers.GetFuncType(args.AddFirst(typeof(CallSite))))
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
            ContractUtils.Requires(pis.Length > 0 && pis[0].ParameterType == typeof(CallSite), "T");

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
            var body = new List<Expression>();
            var vars = new List<ParameterExpression>();
            var @params = invoke.GetParametersCached().Map(p => Expression.Parameter(p.ParameterType, p.Name));
            var @return = Expression.Label(invoke.GetReturnType());
            var typeArgs = new[] { typeof(T) };

            var site = @params[0];
            var arguments = @params.RemoveFirst();

            var @this = Expression.Variable(typeof(CallSite<T>), "this");
            vars.Add(@this);
            body.Add(Expression.Assign(@this, Expression.Convert(site, @this.Type)));

            var applicable = Expression.Variable(typeof(T[]), "applicable");
            vars.Add(applicable);

            var rule = Expression.Variable(typeof(T), "rule");
            vars.Add(rule);

            var originalRule = Expression.Variable(typeof(T), "originalRule");
            vars.Add(originalRule);
            body.Add(Expression.Assign(originalRule, Expression.Field(@this, "Target")));

            ParameterExpression result = null;
            if (@return.Type != typeof(void))
            {
                vars.Add(result = Expression.Variable(@return.Type, "result"));
            }

            var count = Expression.Variable(typeof(int), "count");
            vars.Add(count);
            var index = Expression.Variable(typeof(int), "index");
            vars.Add(index);

            body.Add(
                Expression.Assign(
                    site,
                    Expression.Call(
                        typeof(CallSiteOps),
                        "CreateMatchmaker",
                        typeArgs,
                        @this
                    )
                )
            );

            Expression invokeRule;

            Expression getMatch = Expression.Call(
                typeof(CallSiteOps).GetMethod("GetMatch"),
                site
            );

            Expression resetMatch = Expression.Call(
                typeof(CallSiteOps).GetMethod("ClearMatch"),
                site
            );

            var onMatch = Expression.Call(
                typeof(CallSiteOps),
                "UpdateRules",
                typeArgs,
                @this,
                index
            );

            if (@return.Type == typeof(void))
            {
                invokeRule = Expression.Block(
                    Expression.Invoke(rule, new TrueReadOnlyCollection<Expression>(@params)),
                    Expression.IfThen(
                        getMatch,
                        Expression.Block(onMatch, Expression.Return(@return))
                    )
                );
            }
            else
            {
                invokeRule = Expression.Block(
                    Expression.Assign(result, Expression.Invoke(rule, new TrueReadOnlyCollection<Expression>(@params))),
                    Expression.IfThen(
                        getMatch,
                        Expression.Block(onMatch, Expression.Return(@return, result))
                    )
                );
            }

            Expression getRule = Expression.Assign(rule, Expression.ArrayAccess(applicable, index));

            var @break = Expression.Label();

            var breakIfDone = Expression.IfThen(
                Expression.Equal(index, count),
                Expression.Break(@break)
            );

            var incrementIndex = Expression.PreIncrementAssign(index);

            body.Add(
                Expression.IfThen(
                    Expression.NotEqual(
                        Expression.Assign(applicable, Expression.Call(typeof(CallSiteOps), "GetRules", typeArgs, @this)),
                        Expression.Constant(null, applicable.Type)
                    ),
                    Expression.Block(
                        Expression.Assign(count, Expression.ArrayLength(applicable)),
                        Expression.Assign(index, Expression.Constant(0)),
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
                                            Expression.Field(@this, "Target"),
                                            rule
                                        ),
                                        invokeRule,
                                        resetMatch
                                    )
                                ),
                                incrementIndex
                            ),
                            @break,
                            null
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
            var cache = Expression.Variable(typeof(RuleCache<T>), "cache");
            vars.Add(cache);

            body.Add(
                Expression.Assign(
                    cache,
                    Expression.Call(typeof(CallSiteOps), "GetRuleCache", typeArgs, @this)
                )
            );

            body.Add(
                Expression.Assign(
                    applicable,
                    Expression.Call(typeof(CallSiteOps), "GetCachedRules", typeArgs, cache)
                )
            );

            // L2 invokeRule is different (no onMatch)
            if (@return.Type == typeof(void))
            {
                invokeRule = Expression.Block(
                    Expression.Invoke(rule, new TrueReadOnlyCollection<Expression>(@params)),
                    Expression.IfThen(
                        getMatch,
                        Expression.Return(@return)
                    )
                );
            }
            else
            {
                invokeRule = Expression.Block(
                    Expression.Assign(result, Expression.Invoke(rule, new TrueReadOnlyCollection<Expression>(@params))),
                    Expression.IfThen(
                        getMatch,
                        Expression.Return(@return, result)
                    )
                );
            }

            var tryRule = Expression.TryFinally(
                invokeRule,
                Expression.IfThen(
                    getMatch,
                    Expression.Block(
                        Expression.Call(typeof(CallSiteOps), "AddRule", typeArgs, @this, rule),
                        Expression.Call(typeof(CallSiteOps), "MoveRule", typeArgs, cache, rule, index)
                    )
                )
            );

            getRule = Expression.Assign(
                Expression.Field(@this, "Target"),
                Expression.Assign(rule, Expression.ArrayAccess(applicable, index))
            );

            body.Add(Expression.Assign(index, Expression.Constant(0)));
            body.Add(Expression.Assign(count, Expression.ArrayLength(applicable)));
            body.Add(
                Expression.Loop(
                    Expression.Block(
                        breakIfDone,
                        getRule,
                        tryRule,
                        resetMatch,
                        incrementIndex
                    ),
                    @break,
                    null
                )
            );

            ////
            //// Miss on Level 0, 1 and 2 caches. Create new rule
            ////
            body.Add(Expression.Assign(rule, Expression.Constant(null, rule.Type)));

            var args = Expression.Variable(typeof(object[]), "args");
            vars.Add(args);
            body.Add(
                Expression.Assign(
                    args,
                    Expression.NewArrayInit(typeof(object), arguments.Map(p => Convert(p, typeof(object))))
                )
            );

            Expression setOldTarget = Expression.Assign(
                Expression.Field(@this, "Target"),
                originalRule
            );

            getRule = Expression.Assign(
                Expression.Field(@this, "Target"),
                Expression.Assign(
                    rule,
                    Expression.Call(
                        typeof(CallSiteOps),
                        "Bind",
                        typeArgs,
                        Expression.Property(@this, "Binder"),
                        @this,
                        args
                    )
                )
            );

            tryRule = Expression.TryFinally(
                invokeRule,
                Expression.IfThen(
                    getMatch,
                    Expression.Call(typeof(CallSiteOps), "AddRule", typeArgs, @this, rule)
                )
            );

            body.Add(
                Expression.Loop(
                    Expression.Block(setOldTarget, getRule, tryRule, resetMatch),
                    null, null
                )
            );

            body.Add(Expression.Default(@return.Type));

            var lambda = Expression.Lambda<T>(
                Expression.Label(
                    @return,
                    Expression.Block(
                        new ReadOnlyCollection<ParameterExpression>(vars),
                        new ReadOnlyCollection<Expression>(body)
                    )
                ),
                "CallSite.Target",
                true, // always compile the rules with tail call optimization
                new ReadOnlyCollection<ParameterExpression>(@params)
            );

            // Need to compile with forceDynamic because T could be invisible,
            // or one of the argument types could be invisible
            return lambda.Compile();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private T CreateCustomNoMatchDelegate(MethodInfo invoke)
        {
            var @params = invoke.GetParametersCached().Map(p => Expression.Parameter(p.ParameterType, p.Name));
            return Expression.Lambda<T>(
                Expression.Block(
                    Expression.Call(
                        typeof(CallSiteOps).GetMethod("SetNotMatched"),
                        @params.First()
                    ),
                    Expression.Default(invoke.GetReturnType())
                ),
                @params
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
