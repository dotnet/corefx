// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        [Flags]
        internal enum CompilationFlags
        {
            EmitExpressionStart = 0x0001,
            EmitNoExpressionStart = 0x0002,
            EmitAsDefaultType = 0x0010,
            EmitAsVoidType = 0x0020,
            EmitAsTail = 0x0100,   // at the tail position of a lambda, tail call can be safely emitted
            EmitAsMiddle = 0x0200, // in the middle of a lambda, tail call can be emitted if it is in a return
            EmitAsNoTail = 0x0400, // neither at the tail or in a return, or tail call is not turned on, no tail call is emitted

            EmitExpressionStartMask = 0x000f,
            EmitAsTypeMask = 0x00f0,
            EmitAsTailCallMask = 0x0f00
        }

        /// <summary>
        /// Update the flag with a new EmitAsTailCall flag
        /// </summary>
        private static CompilationFlags UpdateEmitAsTailCallFlag(CompilationFlags flags, CompilationFlags newValue)
        {
            Debug.Assert(newValue == CompilationFlags.EmitAsTail || newValue == CompilationFlags.EmitAsMiddle || newValue == CompilationFlags.EmitAsNoTail);
            var oldValue = flags & CompilationFlags.EmitAsTailCallMask;
            return flags ^ oldValue | newValue;
        }

        /// <summary>
        /// Update the flag with a new EmitExpressionStart flag
        /// </summary>
        private static CompilationFlags UpdateEmitExpressionStartFlag(CompilationFlags flags, CompilationFlags newValue)
        {
            Debug.Assert(newValue == CompilationFlags.EmitExpressionStart || newValue == CompilationFlags.EmitNoExpressionStart);
            var oldValue = flags & CompilationFlags.EmitExpressionStartMask;
            return flags ^ oldValue | newValue;
        }

        /// <summary>
        /// Update the flag with a new EmitAsType flag
        /// </summary>
        private static CompilationFlags UpdateEmitAsTypeFlag(CompilationFlags flags, CompilationFlags newValue)
        {
            Debug.Assert(newValue == CompilationFlags.EmitAsDefaultType || newValue == CompilationFlags.EmitAsVoidType);
            var oldValue = flags & CompilationFlags.EmitAsTypeMask;
            return flags ^ oldValue | newValue;
        }

        /// <summary>
        /// Generates code for this expression in a value position.
        /// This method will leave the value of the expression
        /// on the top of the stack typed as Type.
        /// </summary>
        internal void EmitExpression(Expression node)
        {
            EmitExpression(node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitExpressionStart);
        }

        /// <summary>
        /// Emits an expression and discards the result.  For some nodes this emits
        /// more optimal code then EmitExpression/Pop
        /// </summary>
        private void EmitExpressionAsVoid(Expression node)
        {
            EmitExpressionAsVoid(node, CompilationFlags.EmitAsNoTail);
        }

        private void EmitExpressionAsVoid(Expression node, CompilationFlags flags)
        {
            Debug.Assert(node != null);

            CompilationFlags startEmitted = EmitExpressionStart(node);

            switch (node.NodeType)
            {
                case ExpressionType.Assign:
                    EmitAssign((BinaryExpression)node, CompilationFlags.EmitAsVoidType);
                    break;
                case ExpressionType.Block:
                    Emit((BlockExpression)node, UpdateEmitAsTypeFlag(flags, CompilationFlags.EmitAsVoidType));
                    break;
                case ExpressionType.Throw:
                    EmitThrow((UnaryExpression)node, CompilationFlags.EmitAsVoidType);
                    break;
                case ExpressionType.Goto:
                    EmitGotoExpression(node, UpdateEmitAsTypeFlag(flags, CompilationFlags.EmitAsVoidType));
                    break;
                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.Parameter:
                    // no-op
                    break;
                default:
                    if (node.Type == typeof(void))
                    {
                        EmitExpression(node, UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitNoExpressionStart));
                    }
                    else
                    {
                        EmitExpression(node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
                        _ilg.Emit(OpCodes.Pop);
                    }
                    break;
            }
            EmitExpressionEnd(startEmitted);
        }

        private void EmitExpressionAsType(Expression node, Type type, CompilationFlags flags)
        {
            if (type == typeof(void))
            {
                EmitExpressionAsVoid(node, flags);
            }
            else
            {
                // if the node is emitted as a different type, CastClass IL is emitted at the end,
                // should not emit with tail calls.
                if (!TypeUtils.AreEquivalent(node.Type, type))
                {
                    EmitExpression(node);
                    Debug.Assert(TypeUtils.AreReferenceAssignable(type, node.Type));
                    _ilg.Emit(OpCodes.Castclass, type);
                }
                else
                {
                    // emit the with the flags and emit emit expression start
                    EmitExpression(node, UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitExpressionStart));
                }
            }
        }

        #region label block tracking

        private CompilationFlags EmitExpressionStart(Expression node)
        {
            if (TryPushLabelBlock(node))
            {
                return CompilationFlags.EmitExpressionStart;
            }
            return CompilationFlags.EmitNoExpressionStart;
        }

        private void EmitExpressionEnd(CompilationFlags flags)
        {
            if ((flags & CompilationFlags.EmitExpressionStartMask) == CompilationFlags.EmitExpressionStart)
            {
                PopLabelBlock(_labelBlock.Kind);
            }
        }

        #endregion

        #region InvocationExpression

        private void EmitInvocationExpression(Expression expr, CompilationFlags flags)
        {
            InvocationExpression node = (InvocationExpression)expr;

            // Optimization: inline code for literal lambda's directly
            //
            // This is worth it because otherwise we end up with a extra call
            // to DynamicMethod.CreateDelegate, which is expensive.
            //
            if (node.LambdaOperand != null)
            {
                EmitInlinedInvoke(node, flags);
                return;
            }

            expr = node.Expression;
            if (typeof(LambdaExpression).IsAssignableFrom(expr.Type))
            {
                // if the invoke target is a lambda expression tree, first compile it into a delegate
                expr = Expression.Call(expr, expr.Type.GetMethod("Compile", Array.Empty<Type>()));
            }
            expr = Expression.Call(expr, expr.Type.GetMethod("Invoke"), node.Arguments);

            EmitExpression(expr);
        }

        private void EmitInlinedInvoke(InvocationExpression invoke, CompilationFlags flags)
        {
            var lambda = invoke.LambdaOperand;

            // This is tricky: we need to emit the arguments outside of the
            // scope, but set them inside the scope. Fortunately, using the IL
            // stack it is entirely doable.

            // 1. Emit invoke arguments
            List<WriteBack> wb = EmitArguments(lambda.Type.GetMethod("Invoke"), invoke);

            // 2. Create the nested LambdaCompiler
            var inner = new LambdaCompiler(this, lambda, invoke);

            // 3. Emit the body
            // if the inlined lambda is the last expression of the whole lambda,
            // tail call can be applied.
            if (wb.Count != 0)
            {
                flags = UpdateEmitAsTailCallFlag(flags, CompilationFlags.EmitAsNoTail);
            }
            inner.EmitLambdaBody(_scope, true, flags);

            // 4. Emit writebacks if needed
            EmitWriteBack(wb);
        }

        #endregion

        #region IndexExpression

        private void EmitIndexExpression(Expression expr)
        {
            var node = (IndexExpression)expr;

            // Emit instance, if calling an instance method
            Type objectType = null;
            if (node.Object != null)
            {
                EmitInstance(node.Object, objectType = node.Object.Type);
            }

            // Emit indexes. We don't allow byref args, so no need to worry
            // about writebacks or EmitAddress
            foreach (var arg in node.Arguments)
            {
                EmitExpression(arg);
            }

            EmitGetIndexCall(node, objectType);
        }

        private void EmitIndexAssignment(BinaryExpression node, CompilationFlags flags)
        {
            var index = (IndexExpression)node.Left;

            var emitAs = flags & CompilationFlags.EmitAsTypeMask;

            // Emit instance, if calling an instance method
            Type objectType = null;
            if (index.Object != null)
            {
                EmitInstance(index.Object, objectType = index.Object.Type);
            }

            // Emit indexes. We don't allow byref args, so no need to worry
            // about writebacks or EmitAddress
            foreach (var arg in index.Arguments)
            {
                EmitExpression(arg);
            }

            // Emit value
            EmitExpression(node.Right);

            // Save the expression value, if needed
            LocalBuilder temp = null;
            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Stloc, temp = GetLocal(node.Type));
            }

            EmitSetIndexCall(index, objectType);

            // Restore the value
            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                _ilg.Emit(OpCodes.Ldloc, temp);
                FreeLocal(temp);
            }
        }

        private void EmitGetIndexCall(IndexExpression node, Type objectType)
        {
            if (node.Indexer != null)
            {
                // For indexed properties, just call the getter
                var method = node.Indexer.GetGetMethod(true);
                EmitCall(objectType, method);
            }
            else
            {
                EmitGetArrayElement(objectType);
            }
        }

        private void EmitGetArrayElement(Type arrayType)
        {
            if (!arrayType.IsVector())
            {
                // Multidimensional arrays, call get
                _ilg.Emit(OpCodes.Call, arrayType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                // For one dimensional arrays, emit load
                _ilg.EmitLoadElement(arrayType.GetElementType());
            }
        }

        private void EmitSetIndexCall(IndexExpression node, Type objectType)
        {
            if (node.Indexer != null)
            {
                // For indexed properties, just call the setter
                var method = node.Indexer.GetSetMethod(true);
                EmitCall(objectType, method);
            }
            else
            {
                EmitSetArrayElement(objectType);
            }
        }

        private void EmitSetArrayElement(Type arrayType)
        {
            if (!arrayType.IsVector())
            {
                // Multidimensional arrays, call set
                _ilg.Emit(OpCodes.Call, arrayType.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                // For one dimensional arrays, emit store
                _ilg.EmitStoreElement(arrayType.GetElementType());
            }
        }

        #endregion

        #region MethodCallExpression

        private void EmitMethodCallExpression(Expression expr, CompilationFlags flags)
        {
            MethodCallExpression node = (MethodCallExpression)expr;

            EmitMethodCall(node.Object, node.Method, node, flags);
        }

        private void EmitMethodCallExpression(Expression expr)
        {
            EmitMethodCallExpression(expr, CompilationFlags.EmitAsNoTail);
        }

        private void EmitMethodCall(Expression obj, MethodInfo method, IArgumentProvider methodCallExpr)
        {
            EmitMethodCall(obj, method, methodCallExpr, CompilationFlags.EmitAsNoTail);
        }

        private void EmitMethodCall(Expression obj, MethodInfo method, IArgumentProvider methodCallExpr, CompilationFlags flags)
        {
            // Emit instance, if calling an instance method
            Type objectType = null;
            if (!method.IsStatic)
            {
                EmitInstance(obj, objectType = obj.Type);
            }
            // if the obj has a value type, its address is passed to the method call so we cannot destroy the 
            // stack by emitting a tail call
            if (obj != null && obj.Type.GetTypeInfo().IsValueType)
            {
                EmitMethodCall(method, methodCallExpr, objectType);
            }
            else
            {
                EmitMethodCall(method, methodCallExpr, objectType, flags);
            }
        }

        // assumes 'object' of non-static call is already on stack
        private void EmitMethodCall(MethodInfo mi, IArgumentProvider args, Type objectType)
        {
            EmitMethodCall(mi, args, objectType, CompilationFlags.EmitAsNoTail);
        }

        // assumes 'object' of non-static call is already on stack
        private void EmitMethodCall(MethodInfo mi, IArgumentProvider args, Type objectType, CompilationFlags flags)
        {
            // Emit arguments
            List<WriteBack> wb = EmitArguments(mi, args);

            // Emit the actual call
            OpCode callOp = UseVirtual(mi) ? OpCodes.Callvirt : OpCodes.Call;
            if (callOp == OpCodes.Callvirt && objectType.GetTypeInfo().IsValueType)
            {
                // This automatically boxes value types if necessary.
                _ilg.Emit(OpCodes.Constrained, objectType);
            }
            // The method call can be a tail call if 
            // 1) the method call is the last instruction before Ret
            // 2) the method does not have any ByRef parameters, refer to ECMA-335 Partition III Section 2.4.
            //    "Verification requires that no managed pointers are passed to the method being called, since
            //    it does not track pointers into the current frame."
            if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail && !MethodHasByRefParameter(mi))
            {
                _ilg.Emit(OpCodes.Tailcall);
            }
            if (mi.CallingConvention == CallingConventions.VarArgs)
            {
                int count = args.ArgumentCount;
                Type[] types = new Type[count];
                for (int i = 0; i < count; i++)
                {
                    types[i] = args.GetArgument(i).Type;
                }

                _ilg.EmitCall(callOp, mi, types);
            }
            else
            {
                _ilg.Emit(callOp, mi);
            }

            // Emit writebacks for properties passed as "ref" arguments
            EmitWriteBack(wb);
        }

        private static bool MethodHasByRefParameter(MethodInfo mi)
        {
            foreach (var pi in mi.GetParametersCached())
            {
                if (pi.IsByRefParameter())
                {
                    return true;
                }
            }
            return false;
        }

        private void EmitCall(Type objectType, MethodInfo method)
        {
            if (method.CallingConvention == CallingConventions.VarArgs)
            {
                throw Error.UnexpectedVarArgsCall(method);
            }

            OpCode callOp = UseVirtual(method) ? OpCodes.Callvirt : OpCodes.Call;
            if (callOp == OpCodes.Callvirt && objectType.GetTypeInfo().IsValueType)
            {
                _ilg.Emit(OpCodes.Constrained, objectType);
            }
            _ilg.Emit(callOp, method);
        }

        private static bool UseVirtual(MethodInfo mi)
        {
            // There are two factors: is the method static, virtual or non-virtual instance?
            // And is the object ref or value?
            // The cases are:
            //
            // static, ref:     call
            // static, value:   call
            // virtual, ref:    callvirt
            // virtual, value:  call -- eg, double.ToString must be a non-virtual call to be verifiable.
            // instance, ref:   callvirt -- this looks wrong, but is verifiable and gives us a free null check.
            // instance, value: call
            //
            // We never need to generate a nonvirtual call to a virtual method on a reference type because
            // expression trees do not support "base.Foo()" style calling.
            // 
            // We could do an optimization here for the case where we know that the object is a non-null
            // reference type and the method is a non-virtual instance method.  For example, if we had
            // (new Foo()).Bar() for instance method Bar we don't need the null check so we could do a
            // call rather than a callvirt.  However that seems like it would not be a very big win for
            // most dynamically generated code scenarios, so let's not do that for now.

            if (mi.IsStatic)
            {
                return false;
            }
            if (mi.DeclaringType.GetTypeInfo().IsValueType)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Emits arguments to a call, and returns an array of writebacks that
        /// should happen after the call.
        /// </summary>
        private List<WriteBack> EmitArguments(MethodBase method, IArgumentProvider args)
        {
            return EmitArguments(method, args, 0);
        }

        /// <summary>
        /// Emits arguments to a call, and returns an array of writebacks that
        /// should happen after the call. For emitting dynamic expressions, we
        /// need to skip the first parameter of the method (the call site).
        /// </summary>
        private List<WriteBack> EmitArguments(MethodBase method, IArgumentProvider args, int skipParameters)
        {
            ParameterInfo[] pis = method.GetParametersCached();
            Debug.Assert(args.ArgumentCount + skipParameters == pis.Length);

            var writeBacks = new List<WriteBack>();
            for (int i = skipParameters, n = pis.Length; i < n; i++)
            {
                ParameterInfo parameter = pis[i];
                Expression argument = args.GetArgument(i - skipParameters);
                Type type = parameter.ParameterType;

                if (type.IsByRef)
                {
                    type = type.GetElementType();

                    WriteBack wb = EmitAddressWriteBack(argument, type);
                    if (wb != null)
                    {
                        writeBacks.Add(wb);
                    }
                }
                else
                {
                    EmitExpression(argument);
                }
            }
            return writeBacks;
        }

        private static void EmitWriteBack(IList<WriteBack> writeBacks)
        {
            foreach (WriteBack wb in writeBacks)
            {
                wb();
            }
        }

        #endregion

        private void EmitConstantExpression(Expression expr)
        {
            ConstantExpression node = (ConstantExpression)expr;

            EmitConstant(node.Value, node.Type);
        }

        private void EmitConstant(object value, Type type)
        {
            // Try to emit the constant directly into IL
            if (ILGen.CanEmitConstant(value, type))
            {
                _ilg.EmitConstant(value, type);
                return;
            }

            _boundConstants.EmitConstant(this, value, type);
        }

        private void EmitDynamicExpression(Expression expr)
        {
            if (!(_method is DynamicMethod))
            {
                throw Error.CannotCompileDynamic();
            }

            var node = (IDynamicExpression)expr;

            var site = node.CreateCallSite();
            Type siteType = site.GetType();

            var invoke = node.DelegateType.GetMethod("Invoke");

            // site.Target.Invoke(site, args)
            EmitConstant(site, siteType);

            // Emit the temp as type CallSite so we get more reuse
            _ilg.Emit(OpCodes.Dup);
            var siteTemp = GetLocal(siteType);
            _ilg.Emit(OpCodes.Stloc, siteTemp);
            _ilg.Emit(OpCodes.Ldfld, siteType.GetField("Target"));
            _ilg.Emit(OpCodes.Ldloc, siteTemp);
            FreeLocal(siteTemp);

            List<WriteBack> wb = EmitArguments(invoke, node, 1);
            _ilg.Emit(OpCodes.Callvirt, invoke);
            EmitWriteBack(wb);
        }

        private void EmitNewExpression(Expression expr)
        {
            NewExpression node = (NewExpression)expr;

            if (node.Constructor != null)
            {
                if (node.Constructor.DeclaringType.GetTypeInfo().IsAbstract)
                    throw Error.NonAbstractConstructorRequired();

                List<WriteBack> wb = EmitArguments(node.Constructor, node);
                _ilg.Emit(OpCodes.Newobj, node.Constructor);
                EmitWriteBack(wb);
            }
            else
            {
                Debug.Assert(node.Arguments.Count == 0, "Node with arguments must have a constructor.");
                Debug.Assert(node.Type.GetTypeInfo().IsValueType, "Only value type may have constructor not set.");
                LocalBuilder temp = GetLocal(node.Type);
                _ilg.Emit(OpCodes.Ldloca, temp);
                _ilg.Emit(OpCodes.Initobj, node.Type);
                _ilg.Emit(OpCodes.Ldloc, temp);
                FreeLocal(temp);
            }
        }

        private void EmitTypeBinaryExpression(Expression expr)
        {
            TypeBinaryExpression node = (TypeBinaryExpression)expr;

            if (node.NodeType == ExpressionType.TypeEqual)
            {
                EmitExpression(node.ReduceTypeEqual());
                return;
            }

            Type type = node.Expression.Type;

            // Try to determine the result statically
            AnalyzeTypeIsResult result = ConstantCheck.AnalyzeTypeIs(node);

            if (result == AnalyzeTypeIsResult.KnownTrue ||
                result == AnalyzeTypeIsResult.KnownFalse)
            {
                // Result is known statically, so just emit the expression for
                // its side effects and return the result
                EmitExpressionAsVoid(node.Expression);
                _ilg.EmitBoolean(result == AnalyzeTypeIsResult.KnownTrue);
                return;
            }

            if (result == AnalyzeTypeIsResult.KnownAssignable)
            {
                // We know the type can be assigned, but still need to check
                // for null at runtime
                if (type.IsNullableType())
                {
                    EmitAddress(node.Expression, type);
                    _ilg.EmitHasValue(type);
                    return;
                }

                Debug.Assert(!type.GetTypeInfo().IsValueType);
                EmitExpression(node.Expression);
                _ilg.Emit(OpCodes.Ldnull);
                _ilg.Emit(OpCodes.Ceq);
                _ilg.Emit(OpCodes.Ldc_I4_0);
                _ilg.Emit(OpCodes.Ceq);
                return;
            }

            Debug.Assert(result == AnalyzeTypeIsResult.Unknown);

            // Emit a full runtime "isinst" check
            EmitExpression(node.Expression);
            if (type.GetTypeInfo().IsValueType)
            {
                _ilg.Emit(OpCodes.Box, type);
            }
            _ilg.Emit(OpCodes.Isinst, node.TypeOperand);
            _ilg.Emit(OpCodes.Ldnull);
            _ilg.Emit(OpCodes.Cgt_Un);
        }

        private void EmitVariableAssignment(BinaryExpression node, CompilationFlags flags)
        {
            var variable = (ParameterExpression)node.Left;
            var emitAs = flags & CompilationFlags.EmitAsTypeMask;

            EmitExpression(node.Right);
            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                _ilg.Emit(OpCodes.Dup);
            }

            if (variable.IsByRef)
            {
                // Note: the stloc/ldloc pattern is a bit suboptimal, but it
                // saves us from having to spill stack when assigning to a
                // byref parameter. We already make this same tradeoff for
                // hoisted variables, see ElementStorage.EmitStore

                LocalBuilder value = GetLocal(variable.Type);
                _ilg.Emit(OpCodes.Stloc, value);
                _scope.EmitGet(variable);
                _ilg.Emit(OpCodes.Ldloc, value);
                FreeLocal(value);
                _ilg.EmitStoreValueIndirect(variable.Type);
            }
            else
            {
                _scope.EmitSet(variable);
            }
        }

        private void EmitAssignBinaryExpression(Expression expr)
        {
            EmitAssign((BinaryExpression)expr, CompilationFlags.EmitAsDefaultType);
        }

        private void EmitAssign(BinaryExpression node, CompilationFlags emitAs)
        {
            switch (node.Left.NodeType)
            {
                case ExpressionType.Index:
                    EmitIndexAssignment(node, emitAs);
                    return;
                case ExpressionType.MemberAccess:
                    EmitMemberAssignment(node, emitAs);
                    return;
                case ExpressionType.Parameter:
                    EmitVariableAssignment(node, emitAs);
                    return;
                default:
                    throw Error.InvalidLvalue(node.Left.NodeType);
            }
        }

        private void EmitParameterExpression(Expression expr)
        {
            ParameterExpression node = (ParameterExpression)expr;
            _scope.EmitGet(node);
            if (node.IsByRef)
            {
                _ilg.EmitLoadValueIndirect(node.Type);
            }
        }

        private void EmitLambdaExpression(Expression expr)
        {
            LambdaExpression node = (LambdaExpression)expr;
            EmitDelegateConstruction(node);
        }

        private void EmitRuntimeVariablesExpression(Expression expr)
        {
            RuntimeVariablesExpression node = (RuntimeVariablesExpression)expr;
            _scope.EmitVariableAccess(this, node.Variables);
        }

        private void EmitMemberAssignment(BinaryExpression node, CompilationFlags flags)
        {
            MemberExpression lvalue = (MemberExpression)node.Left;
            MemberInfo member = lvalue.Member;

            // emit "this", if any
            Type objectType = null;
            if (lvalue.Expression != null)
            {
                EmitInstance(lvalue.Expression, objectType = lvalue.Expression.Type);
            }

            // emit value
            EmitExpression(node.Right);

            LocalBuilder temp = null;
            var emitAs = flags & CompilationFlags.EmitAsTypeMask;
            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                // save the value so we can return it
                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Stloc, temp = GetLocal(node.Type));
            }

            var fld = member as FieldInfo;
            if ((object)fld != null)
            {
                _ilg.EmitFieldSet((FieldInfo)member);
            }
            else
            {
                var prop = member as PropertyInfo;
                if ((object)prop != null)
                {
                    EmitCall(objectType, prop.GetSetMethod(true));
                }
                else
                {
                    throw Error.InvalidMemberType(member);
                }
            }

            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                _ilg.Emit(OpCodes.Ldloc, temp);
                FreeLocal(temp);
            }
        }

        private void EmitMemberExpression(Expression expr)
        {
            MemberExpression node = (MemberExpression)expr;

            // emit "this", if any
            Type instanceType = null;
            if (node.Expression != null)
            {
                EmitInstance(node.Expression, instanceType = node.Expression.Type);
            }

            EmitMemberGet(node.Member, instanceType);
        }

        // assumes instance is already on the stack
        private void EmitMemberGet(MemberInfo member, Type objectType)
        {
            var fi = member as FieldInfo;
            if ((object)fi != null)
            {
                object value;

                if (fi.IsLiteral && TryGetRawConstantValue(fi, out value))
                {
                    EmitConstant(value, fi.FieldType);
                }
                else
                {
                    _ilg.EmitFieldGet(fi);
                }
            }
            else
            {
                var prop = member as PropertyInfo;
                if ((object)prop != null)
                {
                    EmitCall(objectType, prop.GetGetMethod(true));
                }
                else
                {
                    throw ContractUtils.Unreachable;
                }
            }
        }

        private static bool TryGetRawConstantValue(FieldInfo fi, out object value)
        {
            // TODO: It looks like GetRawConstantValue is not available at the moment, use it when it comes back.
            //value = fi.GetRawConstantValue();
            //return true;

            try
            {
                value = fi.GetValue(null);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
        private void EmitInstance(Expression instance, Type type)
        {
            if (instance != null)
            {
                if (type.GetTypeInfo().IsValueType)
                {
                    EmitAddress(instance, type);
                }
                else
                {
                    EmitExpression(instance);
                }
            }
        }

        private void EmitNewArrayExpression(Expression expr)
        {
            NewArrayExpression node = (NewArrayExpression)expr;

            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                _ilg.EmitArray(
                    node.Type.GetElementType(),
                    node.Expressions.Count,
                    delegate (int index)
                    {
                        EmitExpression(node.Expressions[index]);
                    }
                );
            }
            else
            {
                ReadOnlyCollection<Expression> bounds = node.Expressions;
                for (int i = 0; i < bounds.Count; i++)
                {
                    Expression x = bounds[i];
                    EmitExpression(x);
                    _ilg.EmitConvertToType(x.Type, typeof(int), true);
                }
                _ilg.EmitArray(node.Type);
            }
        }

        private void EmitDebugInfoExpression(Expression expr)
        {
            return;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expr")]
        private static void EmitExtensionExpression(Expression expr)
        {
            throw Error.ExtensionNotReduced();
        }

        #region ListInit, MemberInit

        private void EmitListInitExpression(Expression expr)
        {
            EmitListInit((ListInitExpression)expr);
        }

        private void EmitMemberInitExpression(Expression expr)
        {
            EmitMemberInit((MemberInitExpression)expr);
        }

        private void EmitBinding(MemberBinding binding, Type objectType)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    EmitMemberAssignment((MemberAssignment)binding, objectType);
                    break;
                case MemberBindingType.ListBinding:
                    EmitMemberListBinding((MemberListBinding)binding);
                    break;
                case MemberBindingType.MemberBinding:
                    EmitMemberMemberBinding((MemberMemberBinding)binding);
                    break;
                default:
                    throw Error.UnknownBindingType();
            }
        }

        private void EmitMemberAssignment(MemberAssignment binding, Type objectType)
        {
            EmitExpression(binding.Expression);
            FieldInfo fi = binding.Member as FieldInfo;
            if (fi != null)
            {
                _ilg.Emit(OpCodes.Stfld, fi);
            }
            else
            {
                PropertyInfo pi = binding.Member as PropertyInfo;
                if (pi != null)
                {
                    EmitCall(objectType, pi.GetSetMethod(true));
                }
                else
                {
                    throw Error.UnhandledBinding();
                }
            }
        }

        private void EmitMemberMemberBinding(MemberMemberBinding binding)
        {
            Type type = GetMemberType(binding.Member);
            if (binding.Member is PropertyInfo && type.GetTypeInfo().IsValueType)
            {
                throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(binding.Member);
            }
            if (type.GetTypeInfo().IsValueType)
            {
                EmitMemberAddress(binding.Member, binding.Member.DeclaringType);
            }
            else
            {
                EmitMemberGet(binding.Member, binding.Member.DeclaringType);
            }
            EmitMemberInit(binding.Bindings, false, type);
        }

        private void EmitMemberListBinding(MemberListBinding binding)
        {
            Type type = GetMemberType(binding.Member);
            if (binding.Member is PropertyInfo && type.GetTypeInfo().IsValueType)
            {
                throw Error.CannotAutoInitializeValueTypeElementThroughProperty(binding.Member);
            }
            if (type.GetTypeInfo().IsValueType)
            {
                EmitMemberAddress(binding.Member, binding.Member.DeclaringType);
            }
            else
            {
                EmitMemberGet(binding.Member, binding.Member.DeclaringType);
            }
            EmitListInit(binding.Initializers, false, type);
        }

        private void EmitMemberInit(MemberInitExpression init)
        {
            EmitExpression(init.NewExpression);
            LocalBuilder loc = null;
            if (init.NewExpression.Type.GetTypeInfo().IsValueType && init.Bindings.Count > 0)
            {
                loc = _ilg.DeclareLocal(init.NewExpression.Type);
                _ilg.Emit(OpCodes.Stloc, loc);
                _ilg.Emit(OpCodes.Ldloca, loc);
            }
            EmitMemberInit(init.Bindings, loc == null, init.NewExpression.Type);
            if (loc != null)
            {
                _ilg.Emit(OpCodes.Ldloc, loc);
            }
        }

        // This method assumes that the instance is on the stack and is expected, based on "keepOnStack" flag
        // to either leave the instance on the stack, or pop it.
        private void EmitMemberInit(ReadOnlyCollection<MemberBinding> bindings, bool keepOnStack, Type objectType)
        {
            int n = bindings.Count;
            if (n == 0)
            {
                // If there are no initializers and instance is not to be kept on the stack, we must pop explicitly.
                if (!keepOnStack)
                {
                    _ilg.Emit(OpCodes.Pop);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    if (keepOnStack || i < n - 1)
                    {
                        _ilg.Emit(OpCodes.Dup);
                    }
                    EmitBinding(bindings[i], objectType);
                }
            }
        }

        private void EmitListInit(ListInitExpression init)
        {
            EmitExpression(init.NewExpression);
            LocalBuilder loc = null;
            if (init.NewExpression.Type.GetTypeInfo().IsValueType)
            {
                loc = _ilg.DeclareLocal(init.NewExpression.Type);
                _ilg.Emit(OpCodes.Stloc, loc);
                _ilg.Emit(OpCodes.Ldloca, loc);
            }
            EmitListInit(init.Initializers, loc == null, init.NewExpression.Type);
            if (loc != null)
            {
                _ilg.Emit(OpCodes.Ldloc, loc);
            }
        }

        // This method assumes that the list instance is on the stack and is expected, based on "keepOnStack" flag
        // to either leave the list instance on the stack, or pop it.
        private void EmitListInit(ReadOnlyCollection<ElementInit> initializers, bool keepOnStack, Type objectType)
        {
            int n = initializers.Count;

            if (n == 0)
            {
                // If there are no initializers and instance is not to be kept on the stack, we must pop explicitly.
                if (!keepOnStack)
                {
                    _ilg.Emit(OpCodes.Pop);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    if (keepOnStack || i < n - 1)
                    {
                        _ilg.Emit(OpCodes.Dup);
                    }
                    EmitMethodCall(initializers[i].AddMethod, initializers[i], objectType);

                    // Some add methods, ArrayList.Add for example, return non-void
                    if (initializers[i].AddMethod.ReturnType != typeof(void))
                    {
                        _ilg.Emit(OpCodes.Pop);
                    }
                }
            }
        }

        private static Type GetMemberType(MemberInfo member)
        {
            FieldInfo fi = member as FieldInfo;
            if (fi != null) return fi.FieldType;
            PropertyInfo pi = member as PropertyInfo;
            if (pi != null) return pi.PropertyType;
            throw Error.MemberNotFieldOrProperty(member);
        }

        #endregion

        #region Expression helpers

        internal static void ValidateLift(IList<ParameterExpression> variables, IList<Expression> arguments)
        {
            System.Diagnostics.Debug.Assert(variables != null);
            System.Diagnostics.Debug.Assert(arguments != null);

            if (variables.Count != arguments.Count)
            {
                throw Error.IncorrectNumberOfIndexes();
            }
            for (int i = 0, n = variables.Count; i < n; i++)
            {
                if (!TypeUtils.AreReferenceAssignable(variables[i].Type, TypeUtils.GetNonNullableType(arguments[i].Type)))
                {
                    throw Error.ArgumentTypesMustMatch();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void EmitLift(ExpressionType nodeType, Type resultType, MethodCallExpression mc, ParameterExpression[] paramList, Expression[] argList)
        {
            Debug.Assert(TypeUtils.AreEquivalent(TypeUtils.GetNonNullableType(resultType), TypeUtils.GetNonNullableType(mc.Type)));

            switch (nodeType)
            {
                default:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    {
                        Label exit = _ilg.DefineLabel();
                        Label exitNull = _ilg.DefineLabel();
                        LocalBuilder anyNull = _ilg.DeclareLocal(typeof(bool));
                        for (int i = 0, n = paramList.Length; i < n; i++)
                        {
                            ParameterExpression v = paramList[i];
                            Expression arg = argList[i];
                            if (TypeUtils.IsNullableType(arg.Type))
                            {
                                _scope.AddLocal(this, v);
                                EmitAddress(arg, arg.Type);
                                _ilg.Emit(OpCodes.Dup);
                                _ilg.EmitHasValue(arg.Type);
                                _ilg.Emit(OpCodes.Ldc_I4_0);
                                _ilg.Emit(OpCodes.Ceq);
                                _ilg.Emit(OpCodes.Stloc, anyNull);
                                _ilg.EmitGetValueOrDefault(arg.Type);
                                _scope.EmitSet(v);
                            }
                            else
                            {
                                _scope.AddLocal(this, v);
                                EmitExpression(arg);
                                if (!arg.Type.GetTypeInfo().IsValueType)
                                {
                                    _ilg.Emit(OpCodes.Dup);
                                    _ilg.Emit(OpCodes.Ldnull);
                                    _ilg.Emit(OpCodes.Ceq);
                                    _ilg.Emit(OpCodes.Stloc, anyNull);
                                }
                                _scope.EmitSet(v);
                            }
                            _ilg.Emit(OpCodes.Ldloc, anyNull);
                            _ilg.Emit(OpCodes.Brtrue, exitNull);
                        }
                        EmitMethodCallExpression(mc);
                        if (TypeUtils.IsNullableType(resultType) && !TypeUtils.AreEquivalent(resultType, mc.Type))
                        {
                            ConstructorInfo ci = resultType.GetConstructor(new Type[] { mc.Type });
                            _ilg.Emit(OpCodes.Newobj, ci);
                        }
                        _ilg.Emit(OpCodes.Br_S, exit);
                        _ilg.MarkLabel(exitNull);
                        if (TypeUtils.AreEquivalent(resultType, TypeUtils.GetNullableType(mc.Type)))
                        {
                            if (resultType.GetTypeInfo().IsValueType)
                            {
                                LocalBuilder result = GetLocal(resultType);
                                _ilg.Emit(OpCodes.Ldloca, result);
                                _ilg.Emit(OpCodes.Initobj, resultType);
                                _ilg.Emit(OpCodes.Ldloc, result);
                                FreeLocal(result);
                            }
                            else
                            {
                                _ilg.Emit(OpCodes.Ldnull);
                            }
                        }
                        else
                        {
                            switch (nodeType)
                            {
                                case ExpressionType.LessThan:
                                case ExpressionType.LessThanOrEqual:
                                case ExpressionType.GreaterThan:
                                case ExpressionType.GreaterThanOrEqual:
                                    _ilg.Emit(OpCodes.Ldc_I4_0);
                                    break;
                                default:
                                    throw Error.UnknownLiftType(nodeType);
                            }
                        }
                        _ilg.MarkLabel(exit);
                        return;
                    }
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    {
                        if (TypeUtils.AreEquivalent(resultType, TypeUtils.GetNullableType(mc.Type)))
                        {
                            goto default;
                        }
                        Label exit = _ilg.DefineLabel();
                        Label exitAllNull = _ilg.DefineLabel();
                        Label exitAnyNull = _ilg.DefineLabel();

                        LocalBuilder anyNull = _ilg.DeclareLocal(typeof(bool));
                        LocalBuilder allNull = _ilg.DeclareLocal(typeof(bool));
                        _ilg.Emit(OpCodes.Ldc_I4_0);
                        _ilg.Emit(OpCodes.Stloc, anyNull);
                        _ilg.Emit(OpCodes.Ldc_I4_1);
                        _ilg.Emit(OpCodes.Stloc, allNull);

                        for (int i = 0, n = paramList.Length; i < n; i++)
                        {
                            ParameterExpression v = paramList[i];
                            Expression arg = argList[i];
                            _scope.AddLocal(this, v);
                            if (TypeUtils.IsNullableType(arg.Type))
                            {
                                EmitAddress(arg, arg.Type);
                                _ilg.Emit(OpCodes.Dup);
                                _ilg.EmitHasValue(arg.Type);
                                _ilg.Emit(OpCodes.Ldc_I4_0);
                                _ilg.Emit(OpCodes.Ceq);
                                _ilg.Emit(OpCodes.Dup);
                                _ilg.Emit(OpCodes.Ldloc, anyNull);
                                _ilg.Emit(OpCodes.Or);
                                _ilg.Emit(OpCodes.Stloc, anyNull);
                                _ilg.Emit(OpCodes.Ldloc, allNull);
                                _ilg.Emit(OpCodes.And);
                                _ilg.Emit(OpCodes.Stloc, allNull);
                                _ilg.EmitGetValueOrDefault(arg.Type);
                            }
                            else
                            {
                                EmitExpression(arg);
                                if (!arg.Type.GetTypeInfo().IsValueType)
                                {
                                    _ilg.Emit(OpCodes.Dup);
                                    _ilg.Emit(OpCodes.Ldnull);
                                    _ilg.Emit(OpCodes.Ceq);
                                    _ilg.Emit(OpCodes.Dup);
                                    _ilg.Emit(OpCodes.Ldloc, anyNull);
                                    _ilg.Emit(OpCodes.Or);
                                    _ilg.Emit(OpCodes.Stloc, anyNull);
                                    _ilg.Emit(OpCodes.Ldloc, allNull);
                                    _ilg.Emit(OpCodes.And);
                                    _ilg.Emit(OpCodes.Stloc, allNull);
                                }
                                else
                                {
                                    _ilg.Emit(OpCodes.Ldc_I4_0);
                                    _ilg.Emit(OpCodes.Stloc, allNull);
                                }
                            }
                            _scope.EmitSet(v);
                        }
                        _ilg.Emit(OpCodes.Ldloc, allNull);
                        _ilg.Emit(OpCodes.Brtrue, exitAllNull);
                        _ilg.Emit(OpCodes.Ldloc, anyNull);
                        _ilg.Emit(OpCodes.Brtrue, exitAnyNull);

                        EmitMethodCallExpression(mc);
                        if (TypeUtils.IsNullableType(resultType) && !TypeUtils.AreEquivalent(resultType, mc.Type))
                        {
                            ConstructorInfo ci = resultType.GetConstructor(new Type[] { mc.Type });
                            _ilg.Emit(OpCodes.Newobj, ci);
                        }
                        _ilg.Emit(OpCodes.Br_S, exit);

                        _ilg.MarkLabel(exitAllNull);
                        _ilg.EmitBoolean(nodeType == ExpressionType.Equal);
                        _ilg.Emit(OpCodes.Br_S, exit);

                        _ilg.MarkLabel(exitAnyNull);
                        _ilg.EmitBoolean(nodeType == ExpressionType.NotEqual);

                        _ilg.MarkLabel(exit);
                        return;
                    }
            }
        }

        #endregion
    }
}
