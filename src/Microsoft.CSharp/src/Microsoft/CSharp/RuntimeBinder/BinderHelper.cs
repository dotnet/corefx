// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder
{
    internal static class BinderHelper
    {
        private static MethodInfo s_DoubleIsNaN;
        private static MethodInfo s_SingleIsNaN;

        internal static DynamicMetaObject Bind(
                DynamicMetaObjectBinder action,
                RuntimeBinder binder,
                DynamicMetaObject[] args,
                IEnumerable<CSharpArgumentInfo> arginfos,
                DynamicMetaObject onBindingError)
        {
            Expression[] parameters = new Expression[args.Length];
            BindingRestrictions restrictions = BindingRestrictions.Empty;
            ICSharpInvokeOrInvokeMemberBinder callPayload = action as ICSharpInvokeOrInvokeMemberBinder;
            ParameterExpression tempForIncrement = null;
            IEnumerator<CSharpArgumentInfo> arginfosEnum = (arginfos ?? Array.Empty<CSharpArgumentInfo>()).GetEnumerator();

            for (int index = 0; index < args.Length; ++index)
            {
                DynamicMetaObject o = args[index];
                // Our contract with the DLR is such that we will not enter a bind unless we have
                // values for the meta-objects involved.

                if (!o.HasValue)
                {
                    Debug.Assert(false, "The runtime binder is being asked to bind a metaobject without a value");
                    throw Error.InternalCompilerError();
                }

                CSharpArgumentInfo info = arginfosEnum.MoveNext() ? arginfosEnum.Current : null;

                if (index == 0 && IsIncrementOrDecrementActionOnLocal(action))
                {
                    // We have an inc or a dec operation. Insert the temp local instead.
                    //
                    // We need to do this because for value types, the object will come 
                    // in boxed, and we'd need to unbox it to get the original type in order
                    // to increment. The only way to do that is to create a new temporary.
                    object value = o.Value;
                    tempForIncrement = Expression.Variable(value != null ? value.GetType() : typeof(object), "t0");
                    parameters[0] = tempForIncrement;
                }
                else
                {
                    parameters[index] = o.Expression;
                }

                BindingRestrictions r = DeduceArgumentRestriction(index, callPayload, o, info);
                restrictions = restrictions.Merge(r);

                // Here we check the argument info. If the argument info shows that the current argument
                // is a literal constant, then we also add an instance restriction on the value of
                // the constant.
                if (info != null && info.LiteralConstant)
                {
                    if (o.Value is double && double.IsNaN((double)o.Value))
                    {
                        MethodInfo isNaN = s_DoubleIsNaN ?? (s_DoubleIsNaN = typeof(double).GetMethod("IsNaN"));
                        Expression e = Expression.Call(null, isNaN, o.Expression);
                        restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(e));
                    }
                    else if (o.Value is float && float.IsNaN((float)o.Value))
                    {
                        MethodInfo isNaN = s_SingleIsNaN ?? (s_SingleIsNaN = typeof(float).GetMethod("IsNaN"));
                        Expression e = Expression.Call(null, isNaN, o.Expression);
                        restrictions = restrictions.Merge(BindingRestrictions.GetExpressionRestriction(e));
                    }
                    else
                    {
                        Expression e = Expression.Equal(o.Expression, Expression.Constant(o.Value, o.Expression.Type));
                        r = BindingRestrictions.GetExpressionRestriction(e);
                        restrictions = restrictions.Merge(r);
                    }
                }
            }

            // Get the bound expression.
            try
            {
                DynamicMetaObject deferredBinding;
                Expression expression = binder.Bind(action, parameters, args, out deferredBinding);

                if (deferredBinding != null)
                {
                    expression = ConvertResult(deferredBinding.Expression, action);
                    restrictions = deferredBinding.Restrictions.Merge(restrictions);
                    return new DynamicMetaObject(expression, restrictions);
                }

                if (tempForIncrement != null)
                {
                    // If we have a ++ or -- payload, we need to do some temp rewriting. 
                    // We rewrite to the following:
                    //
                    // temp = (type)o;
                    // temp++;
                    // o = temp;
                    // return o;

                    DynamicMetaObject arg0 = args[0];

                    expression = Expression.Block(
                        new[] {tempForIncrement},
                        Expression.Assign(tempForIncrement, Expression.Convert(arg0.Expression, arg0.Value.GetType())),
                        expression,
                        Expression.Assign(arg0.Expression, Expression.Convert(tempForIncrement, arg0.Expression.Type)));
                }

                expression = ConvertResult(expression, action);

                return new DynamicMetaObject(expression, restrictions);
            }
            catch (RuntimeBinderException e)
            {
                if (onBindingError != null)
                {
                    return onBindingError;
                }

                return new DynamicMetaObject(
                    Expression.Throw(
                        Expression.New(
                            typeof(RuntimeBinderException).GetConstructor(new Type[] { typeof(string) }),
                            Expression.Constant(e.Message)
                        ),
                        GetTypeForErrorMetaObject(action, args.Length == 0 ? null : args[0])
                    ),
                    restrictions
                );
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static bool IsTypeOfStaticCall(
            int parameterIndex,
            ICSharpInvokeOrInvokeMemberBinder callPayload)
        {
            return parameterIndex == 0 && callPayload != null && callPayload.StaticCall;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static bool IsComObject(object obj)
        {
            return obj != null && Marshal.IsComObject(obj);
        }

#if ENABLECOMBINDER
        /////////////////////////////////////////////////////////////////////////////////

        // Try to determine if this object represents a WindowsRuntime object - i.e. it either
        // is coming from a WinMD file or is derived from a class coming from a WinMD.
        // The logic here matches the CLR's logic of finding a WinRT object.
        internal static bool IsWindowsRuntimeObject(DynamicMetaObject obj)
        {
            Type curType = obj?.RuntimeType;
            while (curType != null)
            {
                TypeAttributes attributes = curType.Attributes;
                if ((attributes & TypeAttributes.WindowsRuntime) == TypeAttributes.WindowsRuntime)
                {
                    // Found a WinRT COM object
                    return true;
                }
                if ((attributes & TypeAttributes.Import) == TypeAttributes.Import)
                {
                    // Found a class that is actually imported from COM but not WinRT
                    // this is definitely a non-WinRT COM object
                    return false;
                }
                curType = curType.BaseType;
            }

            return false;
        }
#endif
        /////////////////////////////////////////////////////////////////////////////////

        private static bool IsTransparentProxy(object obj)
        {
            // In the full framework, this checks:
            //     return obj != null && RemotingServices.IsTransparentProxy(obj);
            // but transparent proxies don't exist in .NET Core.
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static bool IsDynamicallyTypedRuntimeProxy(DynamicMetaObject argument, CSharpArgumentInfo info)
        {
            // This detects situations where, although the argument has a value with
            // a given type, that type is insufficient to determine, statically, the
            // set of reference conversions that are going to exist at bind time for
            // different values. For instance, one __ComObject may allow a conversion
            // to IFoo while another does not.

            bool isDynamicObject =
                info != null &&
                !info.UseCompileTimeType &&
                (IsComObject(argument.Value) || IsTransparentProxy(argument.Value));

            return isDynamicObject;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static BindingRestrictions DeduceArgumentRestriction(
            int parameterIndex,
            ICSharpInvokeOrInvokeMemberBinder callPayload,
            DynamicMetaObject argument,
            CSharpArgumentInfo info)
        {
            // Here we deduce what predicates the DLR can apply to future calls in order to
            // determine whether to use the previously-computed-and-cached delegate, or
            // whether we need to bind the site again. Ideally we would like the 
            // predicate to be as broad as is possible; if we can re-use analysis based
            // solely on the type of the argument, that is preferable to re-using analysis
            // based on object identity with a previously-analyzed argument.

            // The times when we need to restrict re-use to a particular instance, rather 
            // than its type, are:
            // 
            // * if the argument is a null reference then we have no type information.
            //
            // * if we are making a static call then the first argument is
            //   going to be a Type object. In this scenario we should always check 
            //   for a specific Type object rather than restricting to the Type type.
            //
            // * if the argument was dynamic at compile time and it is a dynamic proxy
            //   object that the runtime manages, such as COM RCWs and transparent
            //   proxies.
            //
            // ** there is also a case for constant values (such as literals) to use
            //    something like value restrictions, and that is accomplished in Bind().

            bool useValueRestriction =
                argument.Value == null ||
                IsTypeOfStaticCall(parameterIndex, callPayload) ||
                IsDynamicallyTypedRuntimeProxy(argument, info);

            return useValueRestriction ?
                    BindingRestrictions.GetInstanceRestriction(argument.Expression, argument.Value) :
                    BindingRestrictions.GetTypeRestriction(argument.Expression, argument.RuntimeType);
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static Expression ConvertResult(Expression binding, DynamicMetaObjectBinder action)
        {
            // Need to handle the following cases:
            //   (1) Call to a constructor: no conversions.
            //   (2) Call to a void-returning method: return null iff result is discarded.
            //   (3) Call to a value-type returning method: box to object.
            //
            // In all other cases, binding.Type should be equivalent or
            // reference assignable to resultType.

            var invokeConstructor = action as CSharpInvokeConstructorBinder;
            if (invokeConstructor != null)
            {
                // No conversions needed, the call site has the correct type.
                return binding;
            }

            if (binding.Type == typeof(void))
            {
                var invoke = action as ICSharpInvokeOrInvokeMemberBinder;
                if (invoke != null && invoke.ResultDiscarded)
                {
                    Debug.Assert(action.ReturnType == typeof(object));
                    return Expression.Block(binding, Expression.Default(action.ReturnType));
                }
                else
                {
                    throw Error.BindToVoidMethodButExpectResult();
                }
            }

            if (binding.Type.IsValueType && !action.ReturnType.IsValueType)
            {
                Debug.Assert(action.ReturnType == typeof(object));
                return Expression.Convert(binding, action.ReturnType);
            }

            return binding;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static Type GetTypeForErrorMetaObject(DynamicMetaObjectBinder action, DynamicMetaObject arg0)
        {
            // This is similar to ConvertResult but has fewer things to worry about.

            var invokeConstructor = action as CSharpInvokeConstructorBinder;
            if (invokeConstructor != null)
            {
                Type result = arg0.Value as Type;
                if (result == null)
                {
                    Debug.Assert(false);
                    return typeof(object);
                }

                return result;
            }

            return action.ReturnType;
        }

        /////////////////////////////////////////////////////////////////////////////////

        private static bool IsIncrementOrDecrementActionOnLocal(DynamicMetaObjectBinder action)
        {
            CSharpUnaryOperationBinder operatorPayload = action as CSharpUnaryOperationBinder;

            return operatorPayload != null &&
                (operatorPayload.Operation == ExpressionType.Increment || operatorPayload.Operation == ExpressionType.Decrement);
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal static T[] Cons<T>(T sourceHead, T[] sourceTail)
        {
            if (sourceTail?.Length != 0)
            {
                T[] array = new T[sourceTail.Length + 1];
                array[0] = sourceHead;
                sourceTail.CopyTo(array, 1);
                return array;
            }

            return new[] { sourceHead };
        }

        internal static T[] Cons<T>(T sourceHead, T[] sourceMiddle, T sourceLast)
        {
            if (sourceMiddle?.Length != 0)
            {
                T[] array = new T[sourceMiddle.Length + 2];
                array[0] = sourceHead;
                array[array.Length - 1] = sourceLast;
                sourceMiddle.CopyTo(array, 1);
                return array;
            }

            return new[] {sourceHead, sourceLast};
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal static List<T> ToList<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                return new List<T>();
            }

            return source.ToList();
        }

        /////////////////////////////////////////////////////////////////////////////////

        internal static CallInfo CreateCallInfo(IEnumerable<CSharpArgumentInfo> argInfos, int discard)
        {
            // This function converts the C# Binder's notion of argument information to the
            // DLR's notion. The DLR counts arguments differently than C#. Here are some
            // examples:

            // Expression      Binder                     C# ArgInfos   DLR CallInfo
            //
            // d.M(1, 2, 3);   CSharpInvokeMemberBinder   4             3
            // d(1, 2, 3);     CSharpInvokeBinder         4             3
            // d[1, 2] = 3;    CSharpSetIndexBinder       4             2
            // d[1, 2, 3]      CSharpGetIndexBinder       4             3
            //
            // The "discard" parameter tells this function how many of the C# arg infos it
            // should not count as DLR arguments.

            int argCount = 0;
            List<string> argNames = new List<string>();

            foreach (CSharpArgumentInfo info in argInfos)
            {
                if (info.NamedArgument)
                {
                    argNames.Add(info.Name);
                }
                ++argCount;
            }

            Debug.Assert(discard <= argCount);
            Debug.Assert(argNames.Count <= argCount - discard);

            return new CallInfo(argCount - discard, argNames);
        }

        internal static string GetCLROperatorName(this ExpressionType p)
        {
            switch (p)
            {
                default:
                    return null;

                // Binary Operators
                case ExpressionType.Add:
                    return SpecialNames.CLR_Add;
                case ExpressionType.Subtract:
                    return SpecialNames.CLR_Subtract;
                case ExpressionType.Multiply:
                    return SpecialNames.CLR_Multiply;
                case ExpressionType.Divide:
                    return SpecialNames.CLR_Division;
                case ExpressionType.Modulo:
                    return SpecialNames.CLR_Modulus;
                case ExpressionType.LeftShift:
                    return SpecialNames.CLR_LShift;
                case ExpressionType.RightShift:
                    return SpecialNames.CLR_RShift;
                case ExpressionType.LessThan:
                    return SpecialNames.CLR_LT;
                case ExpressionType.GreaterThan:
                    return SpecialNames.CLR_GT;
                case ExpressionType.LessThanOrEqual:
                    return SpecialNames.CLR_LTE;
                case ExpressionType.GreaterThanOrEqual:
                    return SpecialNames.CLR_GTE;
                case ExpressionType.Equal:
                    return SpecialNames.CLR_Equality;
                case ExpressionType.NotEqual:
                    return SpecialNames.CLR_Inequality;
                case ExpressionType.And:
                    return SpecialNames.CLR_BitwiseAnd;
                case ExpressionType.ExclusiveOr:
                    return SpecialNames.CLR_ExclusiveOr;
                case ExpressionType.Or:
                    return SpecialNames.CLR_BitwiseOr;

                // "op_LogicalNot";
                case ExpressionType.AddAssign:
                    return SpecialNames.CLR_InPlaceAdd;
                case ExpressionType.SubtractAssign:
                    return SpecialNames.CLR_InPlaceSubtract;
                case ExpressionType.MultiplyAssign:
                    return SpecialNames.CLR_InPlaceMultiply;
                case ExpressionType.DivideAssign:
                    return SpecialNames.CLR_InPlaceDivide;
                case ExpressionType.ModuloAssign:
                    return SpecialNames.CLR_InPlaceModulus;
                case ExpressionType.AndAssign:
                    return SpecialNames.CLR_InPlaceBitwiseAnd;
                case ExpressionType.ExclusiveOrAssign:
                    return SpecialNames.CLR_InPlaceExclusiveOr;
                case ExpressionType.OrAssign:
                    return SpecialNames.CLR_InPlaceBitwiseOr;
                case ExpressionType.LeftShiftAssign:
                    return SpecialNames.CLR_InPlaceLShift;
                case ExpressionType.RightShiftAssign:
                    return SpecialNames.CLR_InPlaceRShift;

                // Unary Operators
                case ExpressionType.Negate:
                    return SpecialNames.CLR_UnaryNegation;
                case ExpressionType.UnaryPlus:
                    return SpecialNames.CLR_UnaryPlus;
                case ExpressionType.Not:
                    return SpecialNames.CLR_LogicalNot;
                case ExpressionType.OnesComplement:
                    return SpecialNames.CLR_OnesComplement;
                case ExpressionType.IsTrue:
                    return SpecialNames.CLR_True;
                case ExpressionType.IsFalse:
                    return SpecialNames.CLR_False;

                case ExpressionType.Increment:
                    return SpecialNames.CLR_PreIncrement;
                case ExpressionType.Decrement:
                    return SpecialNames.CLR_PreDecrement;
            }
        }
    }
}
