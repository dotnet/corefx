// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AstUtils = System.Linq.Expressions.Utils;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class ExceptionHandler
    {
        public readonly Type ExceptionType;
        public readonly int StartIndex;
        public readonly int EndIndex;
        public readonly int LabelIndex;
        public readonly int HandlerStartIndex;
        public readonly int HandlerEndIndex;

        internal TryCatchFinallyHandler Parent = null;

        public bool IsFault { get { return ExceptionType == null; } }

        internal ExceptionHandler(int start, int end, int labelIndex, int handlerStartIndex, int handlerEndIndex, Type exceptionType)
        {
            StartIndex = start;
            EndIndex = end;
            LabelIndex = labelIndex;
            ExceptionType = exceptionType;
            HandlerStartIndex = handlerStartIndex;
            HandlerEndIndex = handlerEndIndex;
        }

        internal void SetParent(TryCatchFinallyHandler tryHandler)
        {
            Debug.Assert(Parent == null);
            Parent = tryHandler;
        }

        public bool Matches(Type exceptionType)
        {
            if (ExceptionType == null || ExceptionType.IsAssignableFrom(exceptionType))
            {
                return true;
            }
            return false;
        }

        public bool IsBetterThan(ExceptionHandler other)
        {
            if (other == null) return true;

            Debug.Assert(StartIndex == other.StartIndex && EndIndex == other.EndIndex, "we only need to compare handlers for the same try block");
            return HandlerStartIndex < other.HandlerStartIndex;
        }

        internal bool IsInsideTryBlock(int index)
        {
            return index >= StartIndex && index < EndIndex;
        }

        internal bool IsInsideCatchBlock(int index)
        {
            return index >= HandlerStartIndex && index < HandlerEndIndex;
        }

        internal bool IsInsideFinallyBlock(int index)
        {
            Debug.Assert(Parent != null);
            return Parent.IsFinallyBlockExist && index >= Parent.FinallyStartIndex && index < Parent.FinallyEndIndex;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} [{1}-{2}] [{3}->{4}]",
                (IsFault ? "fault" : "catch(" + ExceptionType.Name + ")"),
                StartIndex, EndIndex,
                HandlerStartIndex, HandlerEndIndex
            );
        }
    }

    internal sealed class TryCatchFinallyHandler
    {
        internal readonly int TryStartIndex = Instruction.UnknownInstrIndex;
        internal readonly int TryEndIndex = Instruction.UnknownInstrIndex;
        internal readonly int FinallyStartIndex = Instruction.UnknownInstrIndex;
        internal readonly int FinallyEndIndex = Instruction.UnknownInstrIndex;
        internal readonly int GotoEndTargetIndex = Instruction.UnknownInstrIndex;

        private readonly ExceptionHandler[] _handlers;

        internal bool IsFinallyBlockExist
        {
            get { return (FinallyStartIndex != Instruction.UnknownInstrIndex && FinallyEndIndex != Instruction.UnknownInstrIndex); }
        }

        internal bool IsCatchBlockExist
        {
            get { return (_handlers != null); }
        }

        /// <summary>
        /// No finally block
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndTargetIndex, ExceptionHandler[] handlers)
            : this(tryStart, tryEnd, gotoEndTargetIndex, Instruction.UnknownInstrIndex, Instruction.UnknownInstrIndex, handlers)
        {
            Debug.Assert(handlers != null, "catch blocks should exist");
        }

        /// <summary>
        /// No catch blocks
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndTargetIndex, int finallyStart, int finallyEnd)
            : this(tryStart, tryEnd, gotoEndTargetIndex, finallyStart, finallyEnd, null)
        {
            Debug.Assert(finallyStart != Instruction.UnknownInstrIndex && finallyEnd != Instruction.UnknownInstrIndex, "finally block should exist");
        }

        /// <summary>
        /// Generic constructor
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndLabelIndex, int finallyStart, int finallyEnd, ExceptionHandler[] handlers)
        {
            TryStartIndex = tryStart;
            TryEndIndex = tryEnd;
            FinallyStartIndex = finallyStart;
            FinallyEndIndex = finallyEnd;
            GotoEndTargetIndex = gotoEndLabelIndex;

            _handlers = handlers;

            if (_handlers != null)
            {
                foreach (var handler in _handlers)
                {
                    handler.SetParent(this);
                }
            }
        }

        /// <summary>
        /// Goto the index of the first instruction of the suitable catch block
        /// </summary>
        internal int GotoHandler(InterpretedFrame frame, object exception, out ExceptionHandler handler)
        {
            Debug.Assert(_handlers != null, "we should have at least one handler if the method gets called");
            handler = null;
            for (int i = 0; i < _handlers.Length; i++)
            {
                if (_handlers[i].Matches(exception.GetType()))
                {
                    handler = _handlers[i];
                    break;
                }
            }
            if (handler == null) { return 0; }
            return frame.Goto(handler.LabelIndex, exception, gotoExceptionHandler: true);
        }
    }

    /// <summary>
    /// The re-throw instrcution will throw this exception
    /// </summary>
    internal sealed class RethrowException : Exception
    {
    }

    internal class DebugInfo
    {
        public int StartLine, EndLine;
        public int Index;
        public string FileName;
        public bool IsClear;
        private static readonly DebugInfoComparer s_debugComparer = new DebugInfoComparer();

        private class DebugInfoComparer : IComparer<DebugInfo>
        {
            //We allow comparison between int and DebugInfo here
            int IComparer<DebugInfo>.Compare(DebugInfo d1, DebugInfo d2)
            {
                if (d1.Index > d2.Index) return 1;
                else if (d1.Index == d2.Index) return 0;
                else return -1;
            }
        }

        public static DebugInfo GetMatchingDebugInfo(DebugInfo[] debugInfos, int index)
        {
            //Create a faked DebugInfo to do the search
            DebugInfo d = new DebugInfo { Index = index };

            //to find the closest debug info before the current index

            int i = Array.BinarySearch<DebugInfo>(debugInfos, d, s_debugComparer);
            if (i < 0)
            {
                //~i is the index for the first bigger element
                //if there is no bigger element, ~i is the length of the array
                i = ~i;
                if (i == 0)
                {
                    return null;
                }
                //return the last one that is smaller
                i = i - 1;
            }

            return debugInfos[i];
        }

        public override string ToString()
        {
            if (IsClear)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}: clear", Index);
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}: [{1}-{2}] '{3}'", Index, StartLine, EndLine, FileName);
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    internal struct InterpretedFrameInfo
    {
        public readonly string MethodName;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public readonly DebugInfo DebugInfo;

        public InterpretedFrameInfo(string methodName, DebugInfo info)
        {
            MethodName = methodName;
            DebugInfo = info;
        }

        public override string ToString()
        {
            return MethodName + (DebugInfo != null ? ": " + DebugInfo : null);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    internal sealed class LightCompiler
    {
        private readonly InstructionList _instructions;
        private readonly LocalVariables _locals = new LocalVariables();

        private readonly List<DebugInfo> _debugInfos = new List<DebugInfo>();
        private readonly HybridReferenceDictionary<LabelTarget, LabelInfo> _treeLabels = new HybridReferenceDictionary<LabelTarget, LabelInfo>();
        private LabelScopeInfo _labelBlock = new LabelScopeInfo(null, LabelScopeKind.Lambda);

        private readonly Stack<ParameterExpression> _exceptionForRethrowStack = new Stack<ParameterExpression>();

        private readonly LightCompiler _parent;

        private static LocalDefinition[] s_emptyLocals = Array.Empty<LocalDefinition>();

        public LightCompiler()
        {
            _instructions = new InstructionList();
        }

        private LightCompiler(LightCompiler parent)
            : this()
        {
            _parent = parent;
        }

        public InstructionList Instructions
        {
            get { return _instructions; }
        }

        public LocalVariables Locals
        {
            get { return _locals; }
        }

        public LightDelegateCreator CompileTop(LambdaExpression node)
        {
            //Console.WriteLine(node.DebugView);
            foreach (var p in node.Parameters)
            {
                var local = _locals.DefineLocal(p, 0);
                _instructions.EmitInitializeParameter(local.Index, p.Type);
            }

            Compile(node.Body);

            // pop the result of the last expression:
            if (node.Body.Type != typeof(void) && node.ReturnType == typeof(void))
            {
                _instructions.EmitPop();
            }

            Debug.Assert(_instructions.CurrentStackDepth == (node.ReturnType != typeof(void) ? 1 : 0));

            return new LightDelegateCreator(MakeInterpreter(node.Name), node);
        }

        private Interpreter MakeInterpreter(string lambdaName)
        {
            var debugInfos = _debugInfos.ToArray();
            return new Interpreter(lambdaName, _locals, GetBranchMapping(), _instructions.ToArray(), debugInfos);
        }


        private void CompileConstantExpression(Expression expr)
        {
            var node = (ConstantExpression)expr;
            _instructions.EmitLoad(node.Value, node.Type);
        }

        private void CompileDefaultExpression(Expression expr)
        {
            CompileDefaultExpression(expr.Type);
        }

        private void CompileDefaultExpression(Type type)
        {
            if (type != typeof(void))
            {
                if (type.GetTypeInfo().IsValueType)
                {
                    object value = ScriptingRuntimeHelpers.GetPrimitiveDefaultValue(type);
                    if (value != null)
                    {
                        _instructions.EmitLoad(value);
                    }
                    else
                    {
                        _instructions.EmitDefaultValue(type);
                    }
                }
                else
                {
                    _instructions.EmitLoad(null);
                }
            }
        }

        private LocalVariable EnsureAvailableForClosure(ParameterExpression expr)
        {
            LocalVariable local;
            if (_locals.TryGetLocalOrClosure(expr, out local))
            {
                if (!local.InClosure && !local.IsBoxed)
                {
                    _locals.Box(expr, _instructions);
                }
                return local;
            }
            else if (_parent != null)
            {
                _parent.EnsureAvailableForClosure(expr);
                return _locals.AddClosureVariable(expr);
            }
            else
            {
                throw new InvalidOperationException("unbound variable: " + expr);
            }
        }

        private LocalVariable ResolveLocal(ParameterExpression variable)
        {
            LocalVariable local;
            if (!_locals.TryGetLocalOrClosure(variable, out local))
            {
                local = EnsureAvailableForClosure(variable);
            }
            return local;
        }

        public void CompileGetVariable(ParameterExpression variable)
        {
            LoadLocalNoValueTypeCopy(variable);

            _instructions.SetDebugCookie(variable.Name);

            EmitCopyValueType(variable.Type);
        }

        private void EmitCopyValueType(Type valueType)
        {
            if (MaybeMutableValueType(valueType))
            {
                // loading a value type on the stack has copy semantics unless
                // we are specifically loading the address of the object, so we
                // emit a copy here if we don't know the type is immutable.
                _instructions.Emit(ValueTypeCopyInstruction.Instruction);
            }
        }

        private void LoadLocalNoValueTypeCopy(ParameterExpression variable)
        {
            LocalVariable local = ResolveLocal(variable);

            if (local.InClosure)
            {
                _instructions.EmitLoadLocalFromClosure(local.Index);
            }
            else if (local.IsBoxed)
            {
                _instructions.EmitLoadLocalBoxed(local.Index);
            }
            else
            {
                _instructions.EmitLoadLocal(local.Index);
            }
        }

        private bool MaybeMutableValueType(Type type)
        {
            return type.GetTypeInfo().IsValueType && !type.GetTypeInfo().IsEnum && !type.GetTypeInfo().IsPrimitive;
        }

        public void CompileGetBoxedVariable(ParameterExpression variable)
        {
            LocalVariable local = ResolveLocal(variable);

            if (local.InClosure)
            {
                _instructions.EmitLoadLocalFromClosureBoxed(local.Index);
            }
            else
            {
                Debug.Assert(local.IsBoxed);
                _instructions.EmitLoadLocal(local.Index);
            }

            _instructions.SetDebugCookie(variable.Name);
        }

        public void CompileSetVariable(ParameterExpression variable, bool isVoid)
        {
            LocalVariable local = ResolveLocal(variable);

            if (local.InClosure)
            {
                if (isVoid)
                {
                    _instructions.EmitStoreLocalToClosure(local.Index);
                }
                else
                {
                    _instructions.EmitAssignLocalToClosure(local.Index);
                }
            }
            else if (local.IsBoxed)
            {
                if (isVoid)
                {
                    _instructions.EmitStoreLocalBoxed(local.Index);
                }
                else
                {
                    _instructions.EmitAssignLocalBoxed(local.Index);
                }
            }
            else
            {
                if (isVoid)
                {
                    _instructions.EmitStoreLocal(local.Index);
                }
                else
                {
                    _instructions.EmitAssignLocal(local.Index);
                }
            }

            _instructions.SetDebugCookie(variable.Name);
        }

        public void CompileParameterExpression(Expression expr)
        {
            var node = (ParameterExpression)expr;
            CompileGetVariable(node);
        }

        private void CompileBlockExpression(Expression expr, bool asVoid)
        {
            var node = (BlockExpression)expr;
            var end = CompileBlockStart(node);

            var lastExpression = node.Expressions[node.Expressions.Count - 1];
            Compile(lastExpression, asVoid);
            CompileBlockEnd(end);
        }

        private LocalDefinition[] CompileBlockStart(BlockExpression node)
        {
            var start = _instructions.Count;

            LocalDefinition[] locals;
            var variables = node.Variables;
            if (variables.Count != 0)
            {
                // TODO: basic flow analysis so we don't have to initialize all
                // variables.
                locals = new LocalDefinition[variables.Count];
                int localCnt = 0;
                foreach (var variable in variables)
                {
                    var local = _locals.DefineLocal(variable, start);
                    locals[localCnt++] = local;

                    _instructions.EmitInitializeLocal(local.Index, variable.Type);
                    _instructions.SetDebugCookie(variable.Name);
                }
            }
            else
            {
                locals = s_emptyLocals;
            }

            for (int i = 0; i < node.Expressions.Count - 1; i++)
            {
                CompileAsVoid(node.Expressions[i]);
            }
            return locals;
        }

        private void CompileBlockEnd(LocalDefinition[] locals)
        {
            foreach (var local in locals)
            {
                _locals.UndefineLocal(local, _instructions.Count);
            }
        }

        private void CompileIndexExpression(Expression expr)
        {
            var index = (IndexExpression)expr;

            // instance:
            if (index.Object != null)
            {
                EmitThisForMethodCall(index.Object);
            }

            // indexes, byref args not allowed.
            foreach (var arg in index.Arguments)
            {
                Compile(arg);
            }

            EmitIndexGet(index);
        }

        private void EmitIndexGet(IndexExpression index)
        {
            if (index.Indexer != null)
            {
                _instructions.EmitCall(index.Indexer.GetGetMethod(true));
            }
            else if (index.Arguments.Count != 1)
            {
                _instructions.EmitCall(index.Object.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                _instructions.EmitGetArrayItem();
            }
        }

        private void CompileIndexAssignment(BinaryExpression node, bool asVoid)
        {
            var index = (IndexExpression)node.Left;

            // instance:
            if (index.Object != null)
            {
                EmitThisForMethodCall(index.Object);
            }

            // indexes, byref args not allowed.
            foreach (var arg in index.Arguments)
            {
                Compile(arg);
            }

            // value:
            Compile(node.Right);
            LocalDefinition local = default(LocalDefinition);
            if (!asVoid)
            {
                local = _locals.DefineLocal(Expression.Parameter(node.Right.Type), _instructions.Count);
                _instructions.EmitAssignLocal(local.Index);
            }

            if (index.Indexer != null)
            {
                _instructions.EmitCall(index.Indexer.GetSetMethod(true));
            }
            else if (index.Arguments.Count != 1)
            {
                _instructions.EmitCall(index.Object.Type.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                _instructions.EmitSetArrayItem();
            }

            if (!asVoid)
            {
                _instructions.EmitLoadLocal(local.Index);
                _locals.UndefineLocal(local, _instructions.Count);
            }
        }

        private void CompileMemberAssignment(BinaryExpression node, bool asVoid)
        {
            var member = (MemberExpression)node.Left;
            var expr = member.Expression;
            if (expr != null)
            {
                EmitThisForMethodCall(expr);
            }

            CompileMemberAssignment(asVoid, member.Member, node.Right);
        }

        private void CompileMemberAssignment(bool asVoid, MemberInfo refMember, Expression value)
        {
            PropertyInfo pi = refMember as PropertyInfo;
            if (pi != null)
            {
                var method = pi.GetSetMethod(true);
                EmitThisForMethodCall(value);

                int start = _instructions.Count;
                if (!asVoid)
                {
                    LocalDefinition local = _locals.DefineLocal(Expression.Parameter(value.Type), start);
                    _instructions.EmitAssignLocal(local.Index);
                    _instructions.EmitCall(method);
                    _instructions.EmitLoadLocal(local.Index);
                    _locals.UndefineLocal(local, _instructions.Count);
                }
                else
                {
                    _instructions.EmitCall(method);
                }
                return;
            }
            else
            {
                // other types inherited from MemberInfo (EventInfo\MethodBase\Type) cannot be used in MemberAssignment
                FieldInfo fi = (FieldInfo)refMember;
                if (fi != null)
                {
                    EmitThisForMethodCall(value);

                    int start = _instructions.Count;
                    if (!asVoid)
                    {
                        LocalDefinition local = _locals.DefineLocal(Expression.Parameter(value.Type), start);
                        _instructions.EmitAssignLocal(local.Index);
                        _instructions.EmitStoreField(fi);
                        _instructions.EmitLoadLocal(local.Index);
                        _locals.UndefineLocal(local, _instructions.Count);
                    }
                    else
                    {
                        _instructions.EmitStoreField(fi);
                    }
                }
            }
        }

        private void CompileVariableAssignment(BinaryExpression node, bool asVoid)
        {
            this.Compile(node.Right);

            var target = (ParameterExpression)node.Left;
            CompileSetVariable(target, asVoid);
        }

        private void CompileAssignBinaryExpression(Expression expr, bool asVoid)
        {
            var node = (BinaryExpression)expr;

            switch (node.Left.NodeType)
            {
                case ExpressionType.Index:
                    CompileIndexAssignment(node, asVoid);
                    break;

                case ExpressionType.MemberAccess:
                    CompileMemberAssignment(node, asVoid);
                    break;

                case ExpressionType.Parameter:
                case ExpressionType.Extension:
                    CompileVariableAssignment(node, asVoid);
                    break;

                default:
                    throw new InvalidOperationException("Invalid lvalue for assignment: " + node.Left.NodeType);
            }
        }

        private bool EmitLiftedNullCheck(Expression node, BranchLabel makeCall)
        {
            Compile(node);
            if (TypeUtils.IsNullableType(node.Type) || !node.Type.GetTypeInfo().IsValueType)
            {
                _instructions.EmitDup();
                _instructions.EmitLoad(null, typeof(object));
                _instructions.EmitNotEqual(typeof(object));
                _instructions.EmitBranch(makeCall);
                _instructions.EmitPop();
                return true;
            }
            return false;
        }

        private static bool IsNullableOrReferenceType(Type t)
        {
            return !t.GetTypeInfo().IsValueType || TypeUtils.IsNullableType(t);
        }

        private void CompileBinaryExpression(Expression expr)
        {
            var node = (BinaryExpression)expr;

            if (node.Method != null)
            {
                if (node.IsLifted)
                {
                    // lifting: we need to do the null checks for nullable types and reference types.  If the value
                    // is null we return null, or false for a comparison unless it's not equal, in which case we return
                    // true.  

                    // INCOMPAT: The DLR binder short circuits on comparisons other than equal and not equal,
                    // but C# doesn't.
                    BranchLabel end = _instructions.MakeLabel();

                    LocalDefinition leftTemp = _locals.DefineLocal(Expression.Parameter(node.Left.Type), _instructions.Count);
                    Compile(node.Left);
                    _instructions.EmitStoreLocal(leftTemp.Index);

                    LocalDefinition rightTemp = _locals.DefineLocal(Expression.Parameter(node.Right.Type), _instructions.Count);
                    Compile(node.Right);
                    _instructions.EmitStoreLocal(rightTemp.Index);

                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                        case ExpressionType.NotEqual:
                            /* generating (equal/not equal):
                                * if(left == null) {
                                *      right == null/right != null
                                * }else if(right == null) {
                                *      False/True
                                * }else{
                                *      op_Equality(left, right)/op_Inequality(left, right)
                                * }
                                */
                            if (node.IsLiftedToNull)
                            {
                                goto default;
                            }

                            Type resultType = TypeUtils.GetNullableType(node.Type);
                            BranchLabel testRight = _instructions.MakeLabel();
                            BranchLabel callMethod = _instructions.MakeLabel();

                            _instructions.EmitLoadLocal(leftTemp.Index);
                            _instructions.EmitLoad(null, typeof(object));
                            _instructions.EmitEqual(typeof(object));
                            _instructions.EmitBranchFalse(testRight);

                            // left is null
                            _instructions.EmitLoadLocal(rightTemp.Index);
                            _instructions.EmitLoad(null, typeof(object));
                            if (node.NodeType == ExpressionType.Equal)
                            {
                                _instructions.EmitEqual(typeof(object));
                            }
                            else
                            {
                                _instructions.EmitNotEqual(typeof(object));
                            }
                            _instructions.EmitBranch(end, false, true);

                            _instructions.MarkLabel(testRight);

                            // left is not null, check right
                            _instructions.EmitLoadLocal(rightTemp.Index);
                            _instructions.EmitLoad(null, typeof(object));
                            _instructions.EmitEqual(typeof(object));
                            _instructions.EmitBranchFalse(callMethod);

                            if (node.NodeType == ExpressionType.Equal)
                            {
                                // right null, left not, false
                                _instructions.EmitLoad(ScriptingRuntimeHelpers.False, typeof(bool));
                            }
                            else
                            {
                                // right null, left not, true
                                _instructions.EmitLoad(ScriptingRuntimeHelpers.True, typeof(bool));
                            }
                            _instructions.EmitBranch(end, false, true);

                            // both are not null
                            _instructions.MarkLabel(callMethod);
                            _instructions.EmitLoadLocal(leftTemp.Index);
                            _instructions.EmitLoadLocal(rightTemp.Index);
                            _instructions.EmitCall(node.Method);
                            break;
                        default:
                            BranchLabel loadDefault = _instructions.MakeLabel();

                            if (!node.Left.Type.GetTypeInfo().IsValueType || TypeUtils.IsNullableType(node.Left.Type))
                            {
                                _instructions.EmitLoadLocal(leftTemp.Index);
                                _instructions.EmitLoad(null, typeof(object));
                                _instructions.EmitEqual(typeof(object));
                                _instructions.EmitBranchTrue(loadDefault);
                            }

                            if (!node.Right.Type.GetTypeInfo().IsValueType || TypeUtils.IsNullableType(node.Right.Type))
                            {
                                _instructions.EmitLoadLocal(rightTemp.Index);
                                _instructions.EmitLoad(null, typeof(object));
                                _instructions.EmitEqual(typeof(object));
                                _instructions.EmitBranchTrue(loadDefault);
                            }

                            _instructions.EmitLoadLocal(leftTemp.Index);
                            _instructions.EmitLoadLocal(rightTemp.Index);
                            _instructions.EmitCall(node.Method);
                            _instructions.EmitBranch(end, false, true);

                            _instructions.MarkLabel(loadDefault);
                            switch (node.NodeType)
                            {
                                case ExpressionType.LessThan:
                                case ExpressionType.LessThanOrEqual:
                                case ExpressionType.GreaterThan:
                                case ExpressionType.GreaterThanOrEqual:
                                    if (node.IsLiftedToNull)
                                    {
                                        goto default;
                                    }
                                    _instructions.EmitLoad(ScriptingRuntimeHelpers.False, typeof(object));
                                    break;
                                default:
                                    _instructions.EmitLoad(null, typeof(object));
                                    break;
                            }
                            break;
                    }

                    _instructions.MarkLabel(end);

                    _locals.UndefineLocal(leftTemp, _instructions.Count);
                    _locals.UndefineLocal(rightTemp, _instructions.Count);
                }
                else
                {
                    Compile(node.Left);
                    Compile(node.Right);
                    _instructions.EmitCall(node.Method);
                }
            }
            else
            {
                switch (node.NodeType)
                {
                    case ExpressionType.ArrayIndex:
                        Debug.Assert(node.Right.Type == typeof(int));
                        Compile(node.Left);
                        Compile(node.Right);
                        _instructions.EmitGetArrayItem();
                        return;

                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                        CompileArithmetic(node.NodeType, node.Left, node.Right);
                        return;

                    case ExpressionType.ExclusiveOr:
                        Compile(node.Left);
                        Compile(node.Right);
                        _instructions.EmitExclusiveOr(node.Left.Type);
                        break;
                    case ExpressionType.Or:
                        Compile(node.Left);
                        Compile(node.Right);
                        _instructions.EmitOr(node.Left.Type);
                        break;
                    case ExpressionType.And:
                        Compile(node.Left);
                        Compile(node.Right);
                        _instructions.EmitAnd(node.Left.Type);
                        break;

                    case ExpressionType.Equal:
                        CompileEqual(node.Left, node.Right, node.IsLiftedToNull);
                        return;

                    case ExpressionType.NotEqual:
                        CompileNotEqual(node.Left, node.Right, node.IsLiftedToNull);
                        return;

                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                        CompileComparison((BinaryExpression)node);
                        return;

                    case ExpressionType.LeftShift:
                        Compile(node.Left);
                        Compile(node.Right);
                        _instructions.EmitLeftShift(node.Left.Type);
                        break;
                    case ExpressionType.RightShift:
                        Compile(node.Left);
                        Compile(node.Right);
                        _instructions.EmitRightShift(node.Left.Type);
                        break;
                    default:
                        throw new PlatformNotSupportedException(SR.Format(SR.UnsupportedExpressionType, node.NodeType));
                }
            }
        }

        private void CompileEqual(Expression left, Expression right, bool liftedToNull)
        {
            Debug.Assert(left.Type == right.Type || !left.Type.GetTypeInfo().IsValueType && !right.Type.GetTypeInfo().IsValueType);
            Compile(left);
            Compile(right);
            _instructions.EmitEqual(left.Type, liftedToNull);
        }

        private void CompileNotEqual(Expression left, Expression right, bool liftedToNull)
        {
            Debug.Assert(left.Type == right.Type || !left.Type.GetTypeInfo().IsValueType && !right.Type.GetTypeInfo().IsValueType);
            Compile(left);
            Compile(right);
            _instructions.EmitNotEqual(left.Type, liftedToNull);
        }

        private void CompileComparison(BinaryExpression node)
        {
            var left = node.Left;
            var right = node.Right;
            Debug.Assert(left.Type == right.Type && TypeUtils.IsNumeric(left.Type));

            Compile(left);
            Compile(right);

            switch (node.NodeType)
            {
                case ExpressionType.LessThan: _instructions.EmitLessThan(left.Type, node.IsLiftedToNull); break;
                case ExpressionType.LessThanOrEqual: _instructions.EmitLessThanOrEqual(left.Type, node.IsLiftedToNull); break;
                case ExpressionType.GreaterThan: _instructions.EmitGreaterThan(left.Type, node.IsLiftedToNull); break;
                case ExpressionType.GreaterThanOrEqual: _instructions.EmitGreaterThanOrEqual(left.Type, node.IsLiftedToNull); break;
                default: throw Assert.Unreachable;
            }
        }

        private void CompileArithmetic(ExpressionType nodeType, Expression left, Expression right)
        {
            Debug.Assert(left.Type == right.Type && TypeUtils.IsArithmetic(left.Type));
            Compile(left);
            Compile(right);
            switch (nodeType)
            {
                case ExpressionType.Add: _instructions.EmitAdd(left.Type, false); break;
                case ExpressionType.AddChecked: _instructions.EmitAdd(left.Type, true); break;
                case ExpressionType.Subtract: _instructions.EmitSub(left.Type, false); break;
                case ExpressionType.SubtractChecked: _instructions.EmitSub(left.Type, true); break;
                case ExpressionType.Multiply: _instructions.EmitMul(left.Type, false); break;
                case ExpressionType.MultiplyChecked: _instructions.EmitMul(left.Type, true); break;
                case ExpressionType.Divide: _instructions.EmitDiv(left.Type); break;
                case ExpressionType.Modulo: _instructions.EmitModulo(left.Type); break;
                default: throw Assert.Unreachable;
            }
        }

        private void CompileConvertUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;
            if (node.Method != null)
            {
                BranchLabel end = _instructions.MakeLabel();
                BranchLabel loadDefault = _instructions.MakeLabel();

                LocalDefinition opTemp = _locals.DefineLocal(Expression.Parameter(node.Operand.Type), _instructions.Count);
                Compile(node.Operand);
                _instructions.EmitStoreLocal(opTemp.Index);

                if (!node.Operand.Type.GetTypeInfo().IsValueType || 
                    (TypeUtils.IsNullableType(node.Operand.Type) && node.IsLiftedToNull))
                {
                    _instructions.EmitLoadLocal(opTemp.Index);
                    _instructions.EmitLoad(null, typeof(object));
                    _instructions.EmitEqual(typeof(object));
                    _instructions.EmitBranchTrue(loadDefault);
                }

                _instructions.EmitLoadLocal(opTemp.Index);
                if(TypeUtils.IsNullableType(node.Operand.Type) &&
                    node.Method.GetParametersCached()[0].ParameterType.Equals(TypeUtils.GetNonNullableType(node.Operand.Type)))
                {
                    _instructions.Emit(NullableMethodCallInstruction.CreateGetValue());
                }

                _instructions.EmitCall(node.Method);

                _instructions.EmitBranch(end, false, true);

                _instructions.MarkLabel(loadDefault);
                _instructions.EmitLoad(null, typeof(object));

                _instructions.MarkLabel(end);

                _locals.UndefineLocal(opTemp, _instructions.Count);
            }
            else if (node.Type == typeof(void))
            {
                CompileAsVoid(node.Operand);
            }
            else
            {
                Compile(node.Operand);
                CompileConvertToType(node.Operand.Type, node.Type, node.NodeType == ExpressionType.ConvertChecked, node.IsLiftedToNull);
            }
        }

        private void CompileConvertToType(Type typeFrom, Type typeTo, bool isChecked, bool isLiftedToNull)
        {
            Debug.Assert(typeFrom != typeof(void) && typeTo != typeof(void));

            if (typeTo.Equals(typeFrom))
            {
                return;
            }

            if (typeFrom.GetTypeInfo().IsValueType &&
                TypeUtils.IsNullableType(typeTo) &&
                TypeUtils.GetNonNullableType(typeTo).Equals(typeFrom))
            {
                // VT -> vt?, no conversion necessary
                return;
            }

            if (typeTo.GetTypeInfo().IsValueType &&
                TypeUtils.IsNullableType(typeFrom) &&
                TypeUtils.GetNonNullableType(typeFrom).Equals(typeTo))
            {
                // VT? -> vt, call get_Value
                _instructions.Emit(NullableMethodCallInstruction.CreateGetValue());
                return;
            }

            Type nonNullableFrom = typeFrom.GetNonNullableType();
            Type nonNullableTo = typeTo.GetNonNullableType();

            // use numeric conversions for both numeric types and enums
            if ((TypeUtils.IsNumeric(nonNullableFrom) || nonNullableFrom.GetTypeInfo().IsEnum)
                 && (TypeUtils.IsNumeric(nonNullableTo) || nonNullableTo.GetTypeInfo().IsEnum))
            {
                Type enumTypeTo = null;

                if (nonNullableFrom.GetTypeInfo().IsEnum)
                {
                    nonNullableFrom = Enum.GetUnderlyingType(nonNullableFrom);
                }
                if (nonNullableTo.GetTypeInfo().IsEnum)
                {
                    enumTypeTo = nonNullableTo;
                    nonNullableTo = Enum.GetUnderlyingType(nonNullableTo);
                }

                TypeCode from = nonNullableFrom.GetTypeCode();
                TypeCode to = nonNullableTo.GetTypeCode();

                if (isChecked)
                {
                    _instructions.EmitNumericConvertChecked(from, to, isLiftedToNull);
                }
                else
                {
                    _instructions.EmitNumericConvertUnchecked(from, to, isLiftedToNull);
                }

                if ((object)enumTypeTo != null)
                {
                    // Convert from underlying to the enum
                    _instructions.EmitCastToEnum(enumTypeTo);
                }

                if (typeTo.IsNullableType())
                {
                    BranchLabel whenNull = _instructions.MakeLabel();
                    _instructions.EmitDup();
                    _instructions.EmitLoad(null, typeof(object));
                    _instructions.EmitEqual(typeof(object));
                    _instructions.EmitBranchTrue(whenNull);

                    // get constructor for nullable type
                    var constructor = typeTo.GetConstructor(new[] { typeTo.GetNonNullableType() });
                    _instructions.EmitNew(constructor);

                    _instructions.MarkLabel(whenNull);
                }


                return;
            }

            if (typeTo == typeof(object) || typeTo.IsAssignableFrom(typeFrom))
            {
                // Conversions to a super-class or implemented interfaces are no-op. 
                return;
            }

            // A conversion to a non-implemented interface or an unrelated class, etc. should fail.
            _instructions.EmitCast(typeTo);
        }

        private void CompileNotExpression(UnaryExpression node)
        {
            Compile(node.Operand);
            _instructions.EmitNot(node.Operand.Type);
        }

        private void CompileUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;

            if (node.Method != null)
            {
                EmitUnaryMethodCall(node);
            }
            else
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Not:
                        CompileNotExpression(node);
                        break;
                    case ExpressionType.TypeAs:
                        CompileTypeAsExpression(node);
                        break;
                    case ExpressionType.ArrayLength:
                        Compile(node.Operand);
                        _instructions.EmitArrayLength();
                        break;
                    case ExpressionType.NegateChecked:
                        Compile(node.Operand);
                        _instructions.EmitNegateChecked(node.Type);
                        break;
                    case ExpressionType.Negate:
                        Compile(node.Operand);
                        _instructions.EmitNegate(node.Type);
                        break;
                    case ExpressionType.Increment:
                        Compile(node.Operand);
                        _instructions.EmitIncrement(node.Type);
                        break;
                    case ExpressionType.Decrement:
                        Compile(node.Operand);
                        _instructions.EmitDecrement(node.Type);
                        break;
                    case ExpressionType.UnaryPlus:
                        Compile(node.Operand);
                        break;
                    case ExpressionType.IsTrue:
                    case ExpressionType.IsFalse:
                        EmitUnaryBoolCheck(node);
                        break;
                    case ExpressionType.OnesComplement:
                        Compile(node.Operand);
                        _instructions.EmitOnesComplement(node.Type);
                        break;
                    default:
                        throw new PlatformNotSupportedException(SR.Format(SR.UnsupportedExpressionType, node.NodeType));
                }
            }
        }

        private void EmitUnaryMethodCall(UnaryExpression node)
        {
            Compile(node.Operand);
            if (node.IsLifted)
            {
                var notNull = _instructions.MakeLabel();
                var computed = _instructions.MakeLabel();

                _instructions.EmitCoalescingBranch(notNull);
                _instructions.EmitBranch(computed);

                _instructions.MarkLabel(notNull);
                _instructions.EmitCall(node.Method);

                _instructions.MarkLabel(computed);
            }
            else
            {
                _instructions.EmitCall(node.Method);
            }
        }

        private void EmitUnaryBoolCheck(UnaryExpression node)
        {
            Compile(node.Operand);
            if (node.IsLifted)
            {
                var notNull = _instructions.MakeLabel();
                var computed = _instructions.MakeLabel();

                _instructions.EmitCoalescingBranch(notNull);
                _instructions.EmitBranch(computed);

                _instructions.MarkLabel(notNull);
                _instructions.EmitLoad(node.NodeType == ExpressionType.IsTrue);
                _instructions.EmitEqual(typeof(bool));

                _instructions.MarkLabel(computed);
            }
            else
            {
                _instructions.EmitLoad(node.NodeType == ExpressionType.IsTrue);
                _instructions.EmitEqual(typeof(bool));
            }
        }

        private void CompileAndAlsoBinaryExpression(Expression expr)
        {
            CompileLogicalBinaryExpression((BinaryExpression)expr, true);
        }

        private void CompileOrElseBinaryExpression(Expression expr)
        {
            CompileLogicalBinaryExpression((BinaryExpression)expr, false);
        }

        private void CompileLogicalBinaryExpression(BinaryExpression b, bool andAlso)
        {
            if (b.Method != null && !b.IsLiftedLogical)
            {
                CompileMethodLogicalBinaryExpression(b, andAlso);
            }
            else if (b.Left.Type == typeof(bool?))
            {
                CompileLiftedLogicalBinaryExpression(b, andAlso);
            }
            else if (b.IsLiftedLogical)
            {
                Compile(b.ReduceUserdefinedLifted());
            }
            else
            {
                CompileUnliftedLogicalBinaryExpression(b, andAlso);
            }
        }

        private void CompileMethodLogicalBinaryExpression(BinaryExpression expr, bool andAlso)
        {
            var labEnd = _instructions.MakeLabel();
            Compile(expr.Left);
            _instructions.EmitDup();

            MethodInfo opTrue = TypeUtils.GetBooleanOperator(expr.Method.DeclaringType, andAlso ? "op_False" : "op_True");
            Debug.Assert(opTrue != null, "factory should check that the method exists");
            _instructions.EmitCall(opTrue);
            _instructions.EmitBranchTrue(labEnd);

            Compile(expr.Right);

            Debug.Assert(expr.Method.IsStatic);
            _instructions.EmitCall(expr.Method);

            _instructions.MarkLabel(labEnd);
        }

        private void CompileLiftedLogicalBinaryExpression(BinaryExpression node, bool andAlso)
        {
            var computeRight = _instructions.MakeLabel();
            var returnFalse = _instructions.MakeLabel();
            var returnNull = _instructions.MakeLabel();
            var returnValue = _instructions.MakeLabel();
            LocalDefinition result = _locals.DefineLocal(Expression.Parameter(node.Left.Type), _instructions.Count);
            LocalDefinition leftTemp = _locals.DefineLocal(Expression.Parameter(node.Left.Type), _instructions.Count);

            Compile(node.Left);
            _instructions.EmitStoreLocal(leftTemp.Index);

            _instructions.EmitLoadLocal(leftTemp.Index);
            _instructions.EmitLoad(null, typeof(object));
            _instructions.EmitEqual(typeof(object));

            _instructions.EmitBranchTrue(computeRight);

            _instructions.EmitLoadLocal(leftTemp.Index);

            if (andAlso)
            {
                _instructions.EmitBranchFalse(returnFalse);
            }
            else
            {
                _instructions.EmitBranchTrue(returnFalse);
            }

            // compute right                
            _instructions.MarkLabel(computeRight);
            LocalDefinition rightTemp = _locals.DefineLocal(Expression.Parameter(node.Right.Type), _instructions.Count);
            Compile(node.Right);
            _instructions.EmitStoreLocal(rightTemp.Index);

            _instructions.EmitLoadLocal(rightTemp.Index);
            _instructions.EmitLoad(null, typeof(object));
            _instructions.EmitEqual(typeof(object));
            _instructions.EmitBranchTrue(returnNull);

            _instructions.EmitLoadLocal(rightTemp.Index);
            if (andAlso)
            {
                _instructions.EmitBranchFalse(returnFalse);
            }
            else
            {
                _instructions.EmitBranchTrue(returnFalse);
            }

            // check left for null again
            _instructions.EmitLoadLocal(leftTemp.Index);
            _instructions.EmitLoad(null, typeof(object));
            _instructions.EmitEqual(typeof(object));
            _instructions.EmitBranchTrue(returnNull);

            // return true
            _instructions.EmitLoad(andAlso ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False, typeof(object));
            _instructions.EmitStoreLocal(result.Index);
            _instructions.EmitBranch(returnValue);

            // return false
            _instructions.MarkLabel(returnFalse);
            _instructions.EmitLoad(andAlso ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True, typeof(object));
            _instructions.EmitStoreLocal(result.Index);
            _instructions.EmitBranch(returnValue);

            // return null
            _instructions.MarkLabel(returnNull);
            _instructions.EmitLoad(null, typeof(object));
            _instructions.EmitStoreLocal(result.Index);

            _instructions.MarkLabel(returnValue);
            _instructions.EmitLoadLocal(result.Index);

            _locals.UndefineLocal(leftTemp, _instructions.Count);
            _locals.UndefineLocal(rightTemp, _instructions.Count);
            _locals.UndefineLocal(result, _instructions.Count);
        }

        private void CompileUnliftedLogicalBinaryExpression(BinaryExpression expr, bool andAlso)
        {
            var elseLabel = _instructions.MakeLabel();
            var endLabel = _instructions.MakeLabel();
            Compile(expr.Left);

            if (andAlso)
            {
                _instructions.EmitBranchFalse(elseLabel);
            }
            else
            {
                _instructions.EmitBranchTrue(elseLabel);
            }
            Compile(expr.Right);
            _instructions.EmitBranch(endLabel, false, true);
            _instructions.MarkLabel(elseLabel);
            _instructions.EmitLoad(!andAlso);
            _instructions.MarkLabel(endLabel);
        }

        private void CompileConditionalExpression(Expression expr, bool asVoid)
        {
            var node = (ConditionalExpression)expr;
            Compile(node.Test);

            if (node.IfTrue == AstUtils.Empty())
            {
                var endOfFalse = _instructions.MakeLabel();
                _instructions.EmitBranchTrue(endOfFalse);
                Compile(node.IfFalse, asVoid);
                _instructions.MarkLabel(endOfFalse);
            }
            else
            {
                var endOfTrue = _instructions.MakeLabel();
                _instructions.EmitBranchFalse(endOfTrue);
                Compile(node.IfTrue, asVoid);

                if (node.IfFalse != AstUtils.Empty())
                {
                    var endOfFalse = _instructions.MakeLabel();
                    _instructions.EmitBranch(endOfFalse, false, !asVoid);
                    _instructions.MarkLabel(endOfTrue);
                    Compile(node.IfFalse, asVoid);
                    _instructions.MarkLabel(endOfFalse);
                }
                else
                {
                    _instructions.MarkLabel(endOfTrue);
                }
            }
        }

        #region Loops

        private void CompileLoopExpression(Expression expr)
        {
            var node = (LoopExpression)expr;
            var enterLoop = new EnterLoopInstruction(node, _locals, _instructions.Count);

            PushLabelBlock(LabelScopeKind.Statement);
            LabelInfo breakLabel = DefineLabel(node.BreakLabel);
            LabelInfo continueLabel = DefineLabel(node.ContinueLabel);

            _instructions.MarkLabel(continueLabel.GetLabel(this));

            // emit loop body:
            _instructions.Emit(enterLoop);
            CompileAsVoid(node.Body);

            // emit loop branch:
            _instructions.EmitBranch(continueLabel.GetLabel(this), expr.Type != typeof(void), false);

            _instructions.MarkLabel(breakLabel.GetLabel(this));

            PopLabelBlock(LabelScopeKind.Statement);

            enterLoop.FinishLoop(_instructions.Count);
        }

        #endregion

        private void CompileSwitchExpression(Expression expr)
        {
            var node = (SwitchExpression)expr;

            if (node.Cases.All(c => c.TestValues.All(t => t is ConstantExpression)))
            {
                var switchType = System.Dynamic.Utils.TypeExtensions.GetTypeCode(node.SwitchValue.Type);

                if (node.Comparison == null)
                {
                    switch (switchType)
                    {
                        case TypeCode.Int32:
                            CompileIntSwitchExpression<System.Int32>(node);
                            return;

                        // the following cases are uncomon,
                        // so to avoid numerous unecessary generic
                        // instantiations of Dictionary<K, V> and related types
                        // in AOT scenarios, we will just use "object" as the key
                        // NOTE: this does not actually result in any
                        //       extra boxing since both keys and values
                        //       are already boxed when we get them
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.Int16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Int64:
                            CompileIntSwitchExpression<System.Object>(node);
                            return;
                    }
                }

                if (switchType == TypeCode.String)
                {
                    // If we have a comparison other than string equality, bail
                    MethodInfo equality = typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) });
                    if (equality != null && !equality.IsStatic)
                    {
                        equality = null;
                    }

                    if (object.Equals(node.Comparison, equality))
                    {
                        CompileStringSwitchExpression(node);
                        return;
                    }
                }
            }

            LocalDefinition temp = _locals.DefineLocal(Expression.Parameter(node.SwitchValue.Type), _instructions.Count);
            Compile(node.SwitchValue);
            _instructions.EmitStoreLocal(temp.Index);
                        
            var doneLabel = Expression.Label(node.Type, "done");

            foreach(var @case in node.Cases)
            {
                foreach(var val in @case.TestValues)
                {
                    //  temp == val ? 
                    //          goto(Body) doneLabel: 
                    //          {};
                    CompileConditionalExpression(
                        Expression.Condition(
                            Expression.Equal(temp.Parameter, val, false, node.Comparison),
                            Expression.Goto(doneLabel, @case.Body),
                            AstUtils.Empty()
                        ),
                        asVoid: true);
                }
            }

            // doneLabel(DefaultBody):
            CompileLabelExpression(Expression.Label(doneLabel, node.DefaultBody));

            _locals.UndefineLocal(temp, _instructions.Count);
        }

        private void CompileIntSwitchExpression<T>(SwitchExpression node)
        {
            LabelInfo end = DefineLabel(null);
            bool hasValue = node.Type != typeof(void);

            Compile(node.SwitchValue);
            var caseDict = new Dictionary<T, int>();
            int switchIndex = _instructions.Count;
            _instructions.EmitIntSwitch(caseDict);

            if (node.DefaultBody != null)
            {
                Compile(node.DefaultBody);
            }
            else
            {
                Debug.Assert(!hasValue);
            }
            _instructions.EmitBranch(end.GetLabel(this), false, hasValue);

            for (int i = 0; i < node.Cases.Count; i++)
            {
                var switchCase = node.Cases[i];

                int caseOffset = _instructions.Count - switchIndex;
                foreach (ConstantExpression testValue in switchCase.TestValues)
                {
                    var key = (T)testValue.Value;
                    if (!caseDict.ContainsKey(key))
                    {
                        caseDict.Add(key, caseOffset);
                    }
                }

                Compile(switchCase.Body);

                if (i < node.Cases.Count - 1)
                {
                    _instructions.EmitBranch(end.GetLabel(this), false, hasValue);
                }
            }

            _instructions.MarkLabel(end.GetLabel(this));
        }

        private void CompileStringSwitchExpression(SwitchExpression node)
        {
            LabelInfo end = DefineLabel(null);
            bool hasValue = node.Type != typeof(void);

            Compile(node.SwitchValue);
            var caseDict = new Dictionary<string, int>();
            int switchIndex = _instructions.Count;
            // by default same as default
            var nullCase = new StrongBox<int>(1);
            _instructions.EmitStringSwitch(caseDict, nullCase);

            if (node.DefaultBody != null)
            {
                Compile(node.DefaultBody);
            }
            else
            {
                Debug.Assert(!hasValue);
            }
            _instructions.EmitBranch(end.GetLabel(this), false, hasValue);

            for (int i = 0; i < node.Cases.Count; i++)
            {
                var switchCase = node.Cases[i];

                int caseOffset = _instructions.Count - switchIndex;
                foreach (ConstantExpression testValue in switchCase.TestValues)
                {
                    string key = (string)testValue.Value;
                    if (key == null)
                    {
                        if (nullCase.Value == 1)
                        {
                            nullCase.Value = caseOffset;
                        }
                    }
                    else if (!caseDict.ContainsKey(key))
                    {
                        caseDict.Add(key, caseOffset);
                    }
                }

                Compile(switchCase.Body);

                if (i < node.Cases.Count - 1)
                {
                    _instructions.EmitBranch(end.GetLabel(this), false, hasValue);
                }
            }

            _instructions.MarkLabel(end.GetLabel(this));
        }

        private void CompileLabelExpression(Expression expr)
        {
            var node = (LabelExpression)expr;

            // If we're an immediate child of a block, our label will already
            // be defined. If not, we need to define our own block so this
            // label isn't exposed except to its own child expression.
            LabelInfo label = null;

            if (_labelBlock.Kind == LabelScopeKind.Block)
            {
                _labelBlock.TryGetLabelInfo(node.Target, out label);

                // We're in a block but didn't find our label, try switch
                if (label == null && _labelBlock.Parent.Kind == LabelScopeKind.Switch)
                {
                    _labelBlock.Parent.TryGetLabelInfo(node.Target, out label);
                }

                // if we're in a switch or block, we should've found the label
                Debug.Assert(label != null);
            }

            if (label == null)
            {
                label = DefineLabel(node.Target);
            }

            if (node.DefaultValue != null)
            {
                if (node.Target.Type == typeof(void))
                {
                    CompileAsVoid(node.DefaultValue);
                }
                else
                {
                    Compile(node.DefaultValue);
                }
            }

            _instructions.MarkLabel(label.GetLabel(this));
        }

        private void CompileGotoExpression(Expression expr)
        {
            var node = (GotoExpression)expr;
            var labelInfo = ReferenceLabel(node.Target);

            if (node.Value != null)
            {
                Compile(node.Value);
            }

            _instructions.EmitGoto(labelInfo.GetLabel(this),
                node.Type != typeof(void),
                node.Value != null && node.Value.Type != typeof(void),
                node.Target.Type != typeof(void));
        }

        public BranchLabel GetBranchLabel(LabelTarget target)
        {
            return ReferenceLabel(target).GetLabel(this);
        }

        public void PushLabelBlock(LabelScopeKind type)
        {
            _labelBlock = new LabelScopeInfo(_labelBlock, type);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind")]
        public void PopLabelBlock(LabelScopeKind kind)
        {
            Debug.Assert(_labelBlock != null && _labelBlock.Kind == kind);
            _labelBlock = _labelBlock.Parent;
        }

        private LabelInfo EnsureLabel(LabelTarget node)
        {
            LabelInfo result;
            if (!_treeLabels.TryGetValue(node, out result))
            {
                _treeLabels[node] = result = new LabelInfo(node);
            }
            return result;
        }

        private LabelInfo ReferenceLabel(LabelTarget node)
        {
            LabelInfo result = EnsureLabel(node);
            result.Reference(_labelBlock);
            return result;
        }

        internal LabelInfo DefineLabel(LabelTarget node)
        {
            if (node == null)
            {
                return new LabelInfo(null);
            }
            LabelInfo result = EnsureLabel(node);
            result.Define(_labelBlock);
            return result;
        }

        private bool TryPushLabelBlock(Expression node)
        {
            // Anything that is "statement-like" -- e.g. has no associated
            // stack state can be jumped into, with the exception of try-blocks
            // We indicate this by a "Block"
            // 
            // Otherwise, we push an "Expression" to indicate that it can't be
            // jumped into
            switch (node.NodeType)
            {
                default:
                    if (_labelBlock.Kind != LabelScopeKind.Expression)
                    {
                        PushLabelBlock(LabelScopeKind.Expression);
                        return true;
                    }
                    return false;
                case ExpressionType.Label:
                    // LabelExpression is a bit special, if it's directly in a
                    // block it becomes associate with the block's scope. Same
                    // thing if it's in a switch case body.
                    if (_labelBlock.Kind == LabelScopeKind.Block)
                    {
                        var label = ((LabelExpression)node).Target;
                        if (_labelBlock.ContainsTarget(label))
                        {
                            return false;
                        }
                        if (_labelBlock.Parent.Kind == LabelScopeKind.Switch &&
                            _labelBlock.Parent.ContainsTarget(label))
                        {
                            return false;
                        }
                    }
                    PushLabelBlock(LabelScopeKind.Statement);
                    return true;
                case ExpressionType.Block:
                    PushLabelBlock(LabelScopeKind.Block);
                    // Labels defined immediately in the block are valid for
                    // the whole block.
                    if (_labelBlock.Parent.Kind != LabelScopeKind.Switch)
                    {
                        DefineBlockLabels(node);
                    }
                    return true;
                case ExpressionType.Switch:
                    PushLabelBlock(LabelScopeKind.Switch);
                    // Define labels inside of the switch cases so theyare in
                    // scope for the whole switch. This allows "goto case" and
                    // "goto default" to be considered as local jumps.
                    var @switch = (SwitchExpression)node;
                    foreach (SwitchCase c in @switch.Cases)
                    {
                        DefineBlockLabels(c.Body);
                    }
                    DefineBlockLabels(@switch.DefaultBody);
                    return true;

                // Remove this when Convert(Void) goes away.
                case ExpressionType.Convert:
                    if (node.Type != typeof(void))
                    {
                        // treat it as an expression
                        goto default;
                    }
                    PushLabelBlock(LabelScopeKind.Statement);
                    return true;

                case ExpressionType.Conditional:
                case ExpressionType.Loop:
                case ExpressionType.Goto:
                    PushLabelBlock(LabelScopeKind.Statement);
                    return true;
            }
        }

        private void DefineBlockLabels(Expression node)
        {
            var block = node as BlockExpression;
            if (block == null)
            {
                return;
            }

            for (int i = 0, n = block.Expressions.Count; i < n; i++)
            {
                Expression e = block.Expressions[i];

                var label = e as LabelExpression;
                if (label != null)
                {
                    DefineLabel(label.Target);
                }
            }
        }

        private HybridReferenceDictionary<LabelTarget, BranchLabel> GetBranchMapping()
        {
            var newLabelMapping = new HybridReferenceDictionary<LabelTarget, BranchLabel>(_treeLabels.Count);
            foreach (var kvp in _treeLabels)
            {
                kvp.Value.ValidateFinish();
                newLabelMapping[kvp.Key] = kvp.Value.GetLabel(this);
            }
            return newLabelMapping;
        }


        private void CheckRethrow()
        {
            // Rethrow is only valid inside a catch.
            for (LabelScopeInfo j = _labelBlock; j != null; j = j.Parent)
            {
                if (j.Kind == LabelScopeKind.Catch)
                {
                    return;
                }
                else if (j.Kind == LabelScopeKind.Finally)
                {
                    // Rethrow from inside finally is not verifiable
                    break;
                }
            }
            throw new InvalidOperationException("Rethrow requires catch");
        }

        private void CompileThrowUnaryExpression(Expression expr, bool asVoid)
        {
            var node = (UnaryExpression)expr;

            if (node.Operand == null)
            {
                CheckRethrow();

                CompileParameterExpression(_exceptionForRethrowStack.Peek());
                if (asVoid)
                {
                    _instructions.EmitRethrowVoid();
                }
                else
                {
                    _instructions.EmitRethrow();
                }
            }
            else
            {
                Compile(node.Operand);
                if (asVoid)
                {
                    _instructions.EmitThrowVoid();
                }
                else
                {
                    _instructions.EmitThrow();
                }
            }
        }

        private bool EndsWithRethrow(Expression expr)
        {
            if (expr.NodeType == ExpressionType.Throw)
            {
                var node = (UnaryExpression)expr;
                return node.Operand == null;
            }

            BlockExpression block = expr as BlockExpression;
            if (block != null)
            {
                return EndsWithRethrow(block.Expressions[block.Expressions.Count - 1]);
            }
            return false;
        }


        private void CompileAsVoidRemoveRethrow(Expression expr)
        {
            int stackDepth = _instructions.CurrentStackDepth;

            if (expr.NodeType == ExpressionType.Throw)
            {
                Debug.Assert(((UnaryExpression)expr).Operand == null);
                return;
            }

            var node = (BlockExpression)expr;
            var end = CompileBlockStart(node);

            CompileAsVoidRemoveRethrow(node.Expressions[node.Expressions.Count - 1]);

            Debug.Assert(stackDepth == _instructions.CurrentStackDepth);

            CompileBlockEnd(end);
        }

        private void CompileTryExpression(Expression expr)
        {
            var node = (TryExpression)expr;

            BranchLabel end = _instructions.MakeLabel();
            BranchLabel gotoEnd = _instructions.MakeLabel();
            int tryStart = _instructions.Count;

            BranchLabel startOfFinally = null;
            if (node.Finally != null)
            {
                startOfFinally = _instructions.MakeLabel();
                _instructions.EmitEnterTryFinally(startOfFinally);
            }
            else
            {
                _instructions.EmitEnterTryCatch();
            }

            List<ExceptionHandler> exHandlers = null;
            var enterTryInstr = _instructions.GetInstruction(tryStart) as EnterTryCatchFinallyInstruction;
            Debug.Assert(enterTryInstr != null);

            PushLabelBlock(LabelScopeKind.Try);
            Compile(node.Body);

            bool hasValue = node.Body.Type != typeof(void);
            int tryEnd = _instructions.Count;

            // handlers jump here:
            _instructions.MarkLabel(gotoEnd);
            _instructions.EmitGoto(end, hasValue, hasValue, hasValue);

            // keep the result on the stack:     
            if (node.Handlers.Count > 0)
            {
                exHandlers = new List<ExceptionHandler>();

                // emulates faults 
                if (node.Finally == null && node.Handlers.Count == 1)
                {
                    var handler = node.Handlers[0];
                    if (handler.Filter == null && handler.Test == typeof(Exception) && handler.Variable == null)
                    {
                        if (EndsWithRethrow(handler.Body))
                        {
                            if (hasValue)
                            {
                                _instructions.EmitEnterExceptionHandlerNonVoid();
                            }
                            else
                            {
                                _instructions.EmitEnterExceptionHandlerVoid();
                            }

                            // at this point the stack balance is prepared for the hidden exception variable:
                            int handlerLabel = _instructions.MarkRuntimeLabel();
                            int handlerStart = _instructions.Count;

                            CompileAsVoidRemoveRethrow(handler.Body);
                            _instructions.EmitLeaveFault(hasValue);
                            _instructions.MarkLabel(end);

                            exHandlers.Add(new ExceptionHandler(tryStart, tryEnd, handlerLabel, handlerStart, _instructions.Count, null));
                            enterTryInstr.SetTryHandler(new TryCatchFinallyHandler(tryStart, tryEnd, gotoEnd.TargetIndex, exHandlers.ToArray()));
                            PopLabelBlock(LabelScopeKind.Try);
                            return;
                        }
                    }
                }

                foreach (var handler in node.Handlers)
                {
                    PushLabelBlock(LabelScopeKind.Catch);

                    if (handler.Filter != null)
                    {
                        throw new PlatformNotSupportedException(SR.FilterBlockNotSupported);
                    }

                    var parameter = handler.Variable ?? Expression.Parameter(handler.Test);

                    var local = _locals.DefineLocal(parameter, _instructions.Count);
                    _exceptionForRethrowStack.Push(parameter);

                    // add a stack balancing nop instruction (exception handling pushes the current exception):
                    if (hasValue)
                    {
                        _instructions.EmitEnterExceptionHandlerNonVoid();
                    }
                    else
                    {
                        _instructions.EmitEnterExceptionHandlerVoid();
                    }

                    // at this point the stack balance is prepared for the hidden exception variable:
                    int handlerLabel = _instructions.MarkRuntimeLabel();
                    int handlerStart = _instructions.Count;

                    CompileSetVariable(parameter, true);
                    Compile(handler.Body);

                    _exceptionForRethrowStack.Pop();

                    // keep the value of the body on the stack:
                    Debug.Assert(hasValue == (handler.Body.Type != typeof(void)));
                    _instructions.EmitLeaveExceptionHandler(hasValue, gotoEnd);

                    exHandlers.Add(new ExceptionHandler(tryStart, tryEnd, handlerLabel, handlerStart, _instructions.Count, handler.Test));
                    PopLabelBlock(LabelScopeKind.Catch);

                    _locals.UndefineLocal(local, _instructions.Count);
                }

                if (node.Fault != null)
                {
                    throw new PlatformNotSupportedException(SR.FaultBlockNotSupported);
                }
            }

            if (node.Finally != null)
            {
                Debug.Assert(startOfFinally != null);
                PushLabelBlock(LabelScopeKind.Finally);

                _instructions.MarkLabel(startOfFinally);
                _instructions.EmitEnterFinally(startOfFinally);
                CompileAsVoid(node.Finally);
                _instructions.EmitLeaveFinally();

                enterTryInstr.SetTryHandler(
                    new TryCatchFinallyHandler(tryStart, tryEnd, gotoEnd.TargetIndex,
                        startOfFinally.TargetIndex, _instructions.Count,
                        exHandlers != null ? exHandlers.ToArray() : null));
                PopLabelBlock(LabelScopeKind.Finally);
            }
            else
            {
                Debug.Assert(exHandlers != null);
                enterTryInstr.SetTryHandler(
                    new TryCatchFinallyHandler(tryStart, tryEnd, gotoEnd.TargetIndex, exHandlers.ToArray()));
            }

            _instructions.MarkLabel(end);

            PopLabelBlock(LabelScopeKind.Try);
        }

        private void CompileMethodCallExpression(Expression expr)
        {
            var node = (MethodCallExpression)expr;

            var parameters = node.Method.GetParameters();

            // TODO: Support pass by reference.
            List<ByRefUpdater> updaters = null;
            if (!node.Method.IsStatic)
            {
                var updater = CompileAddress(node.Object, -1);
                if (updater != null)
                {
                    updaters = new List<ByRefUpdater>() { updater };
                }
            }

            Debug.Assert(parameters.Length == node.Arguments.Count);

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var arg = node.Arguments[i];

                // byref calls leave out values on the stack, we use a callback
                // to emit the code which processes each value left on the stack.
                if (parameters[i].ParameterType.IsByRef)
                {
                    var updater = CompileAddress(arg, i);
                    if (updater != null)
                    {
                        if (updaters == null)
                        {
                            updaters = new List<ByRefUpdater>();
                        }

                        updaters.Add(updater);
                    }
                }
                else
                {
                    Compile(arg);
                }
            }

            if (!node.Method.IsStatic &&
                node.Object.Type.IsNullableType())
            {
                // reflection doesn't let us call methods on Nullable<T> when the value
                // is null...  so we get to special case those methods!
                _instructions.EmitNullableCall(node.Method, parameters);
            }
            else
            {
                if (updaters == null)
                {
                    _instructions.EmitCall(node.Method, parameters);
                }
                else
                {
                    _instructions.EmitByRefCall(node.Method, parameters, updaters.ToArray());

                    foreach (var updater in updaters)
                    {
                        updater.UndefineTemps(_instructions, _locals);
                    }
                }
            }
        }

        private ByRefUpdater CompileArrayIndexAddress(Expression array, Expression index, int argumentIndex)
        {
            var left = _locals.DefineLocal(Expression.Parameter(array.Type, "array"), _instructions.Count);
            var right = _locals.DefineLocal(Expression.Parameter(index.Type, "index"), _instructions.Count);
            Compile(array);
            _instructions.EmitStoreLocal(left.Index);
            Compile(index);
            _instructions.EmitStoreLocal(right.Index);

            _instructions.EmitLoadLocal(left.Index);
            _instructions.EmitLoadLocal(right.Index);
            _instructions.EmitGetArrayItem();

            return new ArrayByRefUpdater(left, right, argumentIndex);
        }

        private void EmitThisForMethodCall(Expression node)
        {
            CompileAddress(node, -1);
        }

        /// <summary>
        /// Emits the address of the specified node.  
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private ByRefUpdater CompileAddress(Expression node, int index)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Parameter:
                    LoadLocalNoValueTypeCopy((ParameterExpression)node);

                    return new ParameterByRefUpdater(ResolveLocal((ParameterExpression)node), index);
                case ExpressionType.ArrayIndex:
                    BinaryExpression array = (BinaryExpression)node;

                    return CompileArrayIndexAddress(array.Left, array.Right, index);
                case ExpressionType.Index:
                    var indexNode = (IndexExpression)node;
                    if (/*!TypeUtils.AreEquivalent(type, node.Type) || */indexNode.Indexer != null)
                    {
                        LocalDefinition? objTmp = null;
                        if (indexNode.Object != null)
                        {
                            Compile(indexNode.Object);
                            objTmp = _locals.DefineLocal(Expression.Parameter(indexNode.Object.Type), _instructions.Count);
                            _instructions.EmitDup();
                            _instructions.EmitStoreLocal(objTmp.Value.Index);
                        }

                        List<LocalDefinition> indexLocals = new List<LocalDefinition>();
                        for (int i = 0; i < indexNode.Arguments.Count; i++)
                        {
                            Compile(indexNode.Arguments[i]);

                            var argTmp = _locals.DefineLocal(Expression.Parameter(indexNode.Arguments[i].Type), _instructions.Count);
                            _instructions.EmitDup();
                            _instructions.EmitStoreLocal(argTmp.Index);

                            indexLocals.Add(argTmp);
                        }

                        EmitIndexGet(indexNode);

                        return new IndexMethodByRefUpdater(objTmp, indexLocals.ToArray(), indexNode.Indexer.GetSetMethod(), index);
                    }
                    else if (indexNode.Arguments.Count == 1)
                    {
                        return CompileArrayIndexAddress(indexNode.Object, indexNode.Arguments[0], index);
                    }
                    else
                    {
                        return CompileMultiDimArrayAccess(indexNode.Object, indexNode.Arguments, index);
                    }
                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)node;

                    LocalDefinition? memberTemp = null;
                    if (member.Expression != null)
                    {
                        memberTemp = _locals.DefineLocal(Expression.Parameter(member.Expression.Type, "member"), _instructions.Count);
                        EmitThisForMethodCall(member.Expression);
                        _instructions.EmitDup();
                        _instructions.EmitStoreLocal(memberTemp.Value.Index);
                    }

                    FieldInfo field = member.Member as FieldInfo;
                    if (field != null)
                    {
                        _instructions.EmitLoadField(field);
                        return new FieldByRefUpdater(memberTemp, field, index);
                    }
                    PropertyInfo property = member.Member as PropertyInfo;
                    if (property != null)
                    {
                        _instructions.EmitCall(property.GetGetMethod(true));
                        if (property.CanWrite)
                        {
                            return new PropertyByRefUpdater(memberTemp, property, index);
                        }
                        return null;
                    }
                    throw new InvalidOperationException(String.Format("Address of {0}", node.NodeType));
                case ExpressionType.Call:
                    // An array index of a multi-dimensional array is represented by a call to Array.Get,
                    // rather than having its own array-access node. This means that when we are trying to
                    // get the address of a member of a multi-dimensional array, we'll be trying to
                    // get the address of a Get method, and it will fail to do so. Instead, detect
                    // this situation and replace it with a call to the Address method.
                    MethodCallExpression call = (MethodCallExpression)node;
                    if (!call.Method.IsStatic &&
                        call.Object.Type.IsArray &&
                        call.Method == call.Object.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance))
                    {
                        return CompileMultiDimArrayAccess(
                            call.Object,
                            call.Arguments,
                            index
                        );
                    }
                    else
                    {
                        goto default;
                    }

                case ExpressionType.Unbox:
                    Compile(node);
                    return null;
                default:
                    Compile(node);
                    return null;
            }
        }

        private ByRefUpdater CompileMultiDimArrayAccess(Expression array, IList<Expression> arguments, int index)
        {
            Compile(array);
            LocalDefinition objTmp = _locals.DefineLocal(Expression.Parameter(array.Type), _instructions.Count);
            _instructions.EmitDup();
            _instructions.EmitStoreLocal(objTmp.Index);

            List<LocalDefinition> indexLocals = new List<LocalDefinition>();
            for (int i = 0; i < arguments.Count; i++)
            {
                Compile(arguments[i]);

                var argTmp = _locals.DefineLocal(Expression.Parameter(arguments[i].Type), _instructions.Count);
                _instructions.EmitDup();
                _instructions.EmitStoreLocal(argTmp.Index);

                indexLocals.Add(argTmp);
            }

            _instructions.EmitCall(array.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance));

            return new IndexMethodByRefUpdater(objTmp, indexLocals.ToArray(), array.Type.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance), index);
        }

        private void CompileNewExpression(Expression expr)
        {
            var node = (NewExpression)expr;

            if (node.Constructor != null)
            {
                var parameters = node.Constructor.GetParameters();
                List<ByRefUpdater> updaters = null;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        var updater = CompileAddress(node.Arguments[i], i);
                        if (updater != null)
                        {
                            if (updaters == null)
                            {
                                updaters = new List<ByRefUpdater>();
                            }
                            updaters.Add(updater);
                        }
                    }
                    else
                    {
                        Compile(node.Arguments[i]);
                    }
                }

                if (updaters != null)
                {
                    _instructions.EmitByRefNew(node.Constructor, updaters.ToArray());
                }
                else
                {
                    _instructions.EmitNew(node.Constructor);
                }
            }
            else
            {
                Debug.Assert(expr.Type.GetTypeInfo().IsValueType);
                _instructions.EmitDefaultValue(node.Type);
            }
        }

        private void CompileMemberExpression(Expression expr)
        {
            var node = (MemberExpression)expr;

            CompileMember(node.Expression, node.Member);
        }

        private void CompileMember(Expression from, MemberInfo member)
        {
            FieldInfo fi = member as FieldInfo;
            if (fi != null)
            {
                if (fi.IsLiteral)
                {
                    _instructions.EmitLoad(fi.GetValue(null), fi.FieldType);
                }
                else if (fi.IsStatic)
                {
                    if (fi.IsInitOnly)
                    {
                        _instructions.EmitLoad(fi.GetValue(null), fi.FieldType);
                    }
                    else
                    {
                        _instructions.EmitLoadField(fi);
                    }
                }
                else
                {
                    if (from != null)
                    {
                        EmitThisForMethodCall(from);
                    }
                    _instructions.EmitLoadField(fi);
                }
                return;
            }
            else
            {
                // MemberExpression can use either FieldInfo or PropertyInfo - other types derived from MemberInfo are not permitted
                PropertyInfo pi = (PropertyInfo)member;
                if (pi != null)
                {
                    var method = pi.GetGetMethod(true);
                    if (from != null)
                    {
                        EmitThisForMethodCall(from);
                    }

                    if (!method.IsStatic &&
                        from.Type.IsNullableType())
                    {
                        // reflection doesn't let us call methods on Nullable<T> when the value
                        // is null...  so we get to special case those methods!
                        _instructions.EmitNullableCall(method, Array.Empty<ParameterInfo>());
                    }
                    else
                    {
                        _instructions.EmitCall(method);
                    }

                    return;
                }
            }
        }

        private void CompileNewArrayExpression(Expression expr)
        {
            var node = (NewArrayExpression)expr;

            foreach (var arg in node.Expressions)
            {
                Compile(arg);
            }

            Type elementType = node.Type.GetElementType();
            int rank = node.Expressions.Count;

            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                _instructions.EmitNewArrayInit(elementType, rank);
            }
            else
            {
                Debug.Assert(node.NodeType == ExpressionType.NewArrayBounds);
                if (rank == 1)
                {
                    _instructions.EmitNewArray(elementType);
                }
                else
                {
                    _instructions.EmitNewArrayBounds(elementType, rank);
                }
            }
        }

        private void CompileExtensionExpression(Expression expr)
        {
            var instructionProvider = expr as IInstructionProvider;
            if (instructionProvider != null)
            {
                instructionProvider.AddInstructions(this);
                return;
            }

            if (expr.CanReduce)
            {
                Compile(expr.Reduce());
            }
            else
            {
                throw new PlatformNotSupportedException(SR.NonReducibleExpressionExtensionsNotSupported);
            }
        }


        private void CompileDebugInfoExpression(Expression expr)
        {
            var node = (DebugInfoExpression)expr;
            int start = _instructions.Count;
            var info = new DebugInfo()
            {
                Index = start,
                FileName = node.Document.FileName,
                StartLine = node.StartLine,
                EndLine = node.EndLine,
                IsClear = node.IsClear
            };
            _debugInfos.Add(info);
        }

        private void CompileRuntimeVariablesExpression(Expression expr)
        {
            // Generates IRuntimeVariables for all requested variables
            var node = (RuntimeVariablesExpression)expr;
            foreach (var variable in node.Variables)
            {
                EnsureAvailableForClosure(variable);
                CompileGetBoxedVariable(variable);
            }

            _instructions.EmitNewRuntimeVariables(node.Variables.Count);
        }


        private void CompileLambdaExpression(Expression expr)
        {
            var node = (LambdaExpression)expr;
            var compiler = new LightCompiler(this);
            var creator = compiler.CompileTop(node);

            if (compiler._locals.ClosureVariables != null)
            {
                foreach (ParameterExpression variable in compiler._locals.ClosureVariables.Keys)
                {
                    EnsureAvailableForClosure(variable);
                    CompileGetBoxedVariable(variable);
                }
            }
            _instructions.EmitCreateDelegate(creator);
        }

        private void CompileCoalesceBinaryExpression(Expression expr)
        {
            var node = (BinaryExpression)expr;

            var leftNotNull = _instructions.MakeLabel();
            BranchLabel end = null;

            Compile(node.Left);
            _instructions.EmitCoalescingBranch(leftNotNull);
            _instructions.EmitPop();
            Compile(node.Right);

            if (node.Conversion != null)
            {
                // skip over conversion on RHS
                end = _instructions.MakeLabel();
                _instructions.EmitBranch(end);
            }

            _instructions.MarkLabel(leftNotNull);

            if (node.Conversion != null)
            {
                var temp = Expression.Parameter(node.Left.Type, "temp");
                var local = _locals.DefineLocal(temp, _instructions.Count);
                _instructions.EmitStoreLocal(local.Index);

                CompileMethodCallExpression(
                    Expression.Call(node.Conversion, node.Conversion.Type.GetMethod("Invoke"), new[] { temp })
                );

                _locals.UndefineLocal(local, _instructions.Count);

                _instructions.MarkLabel(end);
            }
        }

        private void CompileInvocationExpression(Expression expr)
        {
            var node = (InvocationExpression)expr;

            if (typeof(LambdaExpression).IsAssignableFrom(node.Expression.Type))
            {
                var compMethod = node.Expression.Type.GetMethod("Compile", Array.Empty<Type>());
                CompileMethodCallExpression(
                    Expression.Call(
                        Expression.Call(
                            node.Expression,
                            compMethod
                        ),
                        compMethod.ReturnType.GetMethod("Invoke"),
                        node.Arguments
                    )
                );
            }
            else
            {
                CompileMethodCallExpression(
                    Expression.Call(node.Expression, node.Expression.Type.GetMethod("Invoke"), node.Arguments)
                );
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expr")]
        private void CompileListInitExpression(Expression expr)
        {
            var node = (ListInitExpression)expr;
            EmitThisForMethodCall(node.NewExpression);
            var initializers = node.Initializers;
            CompileListInit(initializers);
        }

        private void CompileListInit(IList<ElementInit> initializers)
        {
            for (int i = 0; i < initializers.Count; i++)
            {
                _instructions.EmitDup();
                foreach (var arg in initializers[i].Arguments)
                {
                    Compile(arg);
                }
                _instructions.EmitCall(initializers[i].AddMethod);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expr")]
        private void CompileMemberInitExpression(Expression expr)
        {
            var node = (MemberInitExpression)expr;
            EmitThisForMethodCall(node.NewExpression);
            CompileMemberInit(node.Bindings);
        }

        private void CompileMemberInit(Collections.ObjectModel.ReadOnlyCollection<MemberBinding> bindings)
        {
            foreach (var binding in bindings)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        _instructions.EmitDup();
                        CompileMemberAssignment(
                            true,
                            ((MemberAssignment)binding).Member,
                            ((MemberAssignment)binding).Expression
                        );
                        break;
                    case MemberBindingType.ListBinding:
                        var memberList = (MemberListBinding)binding;
                        _instructions.EmitDup();
                        CompileMember(null, memberList.Member);
                        CompileListInit(memberList.Initializers);
                        _instructions.EmitPop();
                        break;
                    case MemberBindingType.MemberBinding:
                        var memberMember = (MemberMemberBinding)binding;
                        _instructions.EmitDup();
                        Type type = GetMemberType(memberMember.Member);
                        if (memberMember.Member is PropertyInfo && type.GetTypeInfo().IsValueType)
                        {
                            throw new InvalidOperationException("CannotAutoInitializeValueTypeMemberThroughProperty");
                        }

                        CompileMember(null, memberMember.Member);
                        CompileMemberInit(memberMember.Bindings);
                        _instructions.EmitPop();
                        break;
                }
            }
        }

        private static Type GetMemberType(MemberInfo member)
        {
            FieldInfo fi = member as FieldInfo;
            if (fi != null) return fi.FieldType;
            PropertyInfo pi = member as PropertyInfo;
            if (pi != null) return pi.PropertyType;
            throw new InvalidOperationException("MemberNotFieldOrProperty");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expr")]
        private void CompileQuoteUnaryExpression(Expression expr)
        {
            UnaryExpression unary = (UnaryExpression)expr;

            var visitor = new QuoteVisitor();
            visitor.Visit(unary.Operand);

            Dictionary<ParameterExpression, LocalVariable> mapping = new Dictionary<ParameterExpression, LocalVariable>();

            foreach (var local in visitor._hoistedParameters)
            {
                EnsureAvailableForClosure(local);
                mapping[local] = ResolveLocal(local);
            }

            _instructions.Emit(new QuoteInstruction(unary.Operand, mapping.Count > 0 ? mapping : null));
        }

        private class QuoteVisitor : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, int> _definedParameters = new Dictionary<ParameterExpression, int>();
            public readonly HashSet<ParameterExpression> _hoistedParameters = new HashSet<ParameterExpression>();

            protected internal override Expression VisitParameter(ParameterExpression node)
            {
                if (!_definedParameters.ContainsKey(node))
                {
                    _hoistedParameters.Add(node);
                }
                return node;
            }

            protected internal override Expression VisitBlock(BlockExpression node)
            {
                PushParameters(node.Variables);

                base.VisitBlock(node);

                PopParameters(node.Variables);

                return node;
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Variable != null)
                {
                    PushParameters(new[] { node.Variable });
                }
                Visit(node.Body);
                Visit(node.Filter);
                if (node.Variable != null)
                {
                    PopParameters(new[] { node.Variable });
                }
                return node;
            }

            protected internal override Expression VisitLambda<T>(Expression<T> node)
            {
                PushParameters(node.Parameters);

                base.VisitLambda(node);

                PopParameters(node.Parameters);

                return node;
            }

            private void PushParameters(ICollection<ParameterExpression> parameters)
            {
                foreach (var param in parameters)
                {
                    int count;
                    if (_definedParameters.TryGetValue(param, out count))
                    {
                        _definedParameters[param] = count + 1;
                    }
                    else
                    {
                        _definedParameters[param] = 1;
                    }
                }
            }

            private void PopParameters(ICollection<ParameterExpression> parameters)
            {
                foreach (var param in parameters)
                {
                    int count = _definedParameters[param];
                    if (count == 0)
                    {
                        _definedParameters.Remove(param);
                    }
                    else
                    {
                        _definedParameters[param] = count - 1;
                    }
                }
            }
        }

        private void CompileUnboxUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;
            // unboxing is a nop:
            Compile(node.Operand);
        }

        private void CompileTypeEqualExpression(Expression expr)
        {
            Debug.Assert(expr.NodeType == ExpressionType.TypeEqual);
            var node = (TypeBinaryExpression)expr;

            Compile(node.Expression);
            if (node.TypeOperand.GetTypeInfo().IsGenericType && node.TypeOperand.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                _instructions.EmitLoad(node.TypeOperand.GenericTypeArguments[0]);
                _instructions.EmitNullableTypeEquals();
            }
            else
            {
                _instructions.EmitLoad(node.TypeOperand);
                _instructions.EmitTypeEquals();
            }
        }

        private void CompileTypeAsExpression(UnaryExpression node)
        {
            Compile(node.Operand);
            _instructions.EmitTypeAs(node.Type);
        }

        private void CompileTypeIsExpression(Expression expr)
        {
            Debug.Assert(expr.NodeType == ExpressionType.TypeIs);
            var node = (TypeBinaryExpression)expr;

            AnalyzeTypeIsResult result = ConstantCheck.AnalyzeTypeIs(node);

            Compile(node.Expression);

            if (result == AnalyzeTypeIsResult.KnownTrue ||
                result == AnalyzeTypeIsResult.KnownFalse)
            {
                // Result is known statically, so just emit the expression for
                // its side effects and return the result
                if (node.Expression.Type != typeof(void))
                {
                    _instructions.EmitPop();
                }

                _instructions.EmitLoad(
                    ScriptingRuntimeHelpers.BooleanToObject(result == AnalyzeTypeIsResult.KnownTrue),
                    typeof(bool)
                );
                return;
            }

            if (node.TypeOperand.GetTypeInfo().IsGenericType && node.TypeOperand.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                _instructions.EmitLoad(node.TypeOperand.GenericTypeArguments[0]);
                _instructions.EmitNullableTypeEquals();
            }
            else
            {
                _instructions.EmitTypeIs(node.TypeOperand);
            }
        }

        internal void Compile(Expression expr, bool asVoid)
        {
            if (asVoid)
            {
                CompileAsVoid(expr);
            }
            else
            {
                Compile(expr);
            }
        }

        internal void CompileAsVoid(Expression expr)
        {
            bool pushLabelBlock = TryPushLabelBlock(expr);
            int startingStackDepth = _instructions.CurrentStackDepth;
            switch (expr.NodeType)
            {
                case ExpressionType.Assign:
                    CompileAssignBinaryExpression(expr, true);
                    break;

                case ExpressionType.Block:
                    CompileBlockExpression(expr, true);
                    break;

                case ExpressionType.Throw:
                    CompileThrowUnaryExpression(expr, true);
                    break;

                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.Parameter:
                    // no-op
                    break;

                default:
                    CompileNoLabelPush(expr);
                    if (expr.Type != typeof(void))
                    {
                        _instructions.EmitPop();
                    }
                    break;
            }
            Debug.Assert(_instructions.CurrentStackDepth == startingStackDepth);
            if (pushLabelBlock)
            {
                PopLabelBlock(_labelBlock.Kind);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void CompileNoLabelPush(Expression expr)
        {
            int startingStackDepth = _instructions.CurrentStackDepth;
            switch (expr.NodeType)
            {
                case ExpressionType.Add: CompileBinaryExpression(expr); break;
                case ExpressionType.AddChecked: CompileBinaryExpression(expr); break;
                case ExpressionType.And: CompileBinaryExpression(expr); break;
                case ExpressionType.AndAlso: CompileAndAlsoBinaryExpression(expr); break;
                case ExpressionType.ArrayLength: CompileUnaryExpression(expr); break;
                case ExpressionType.ArrayIndex: CompileBinaryExpression(expr); break;
                case ExpressionType.Call: CompileMethodCallExpression(expr); break;
                case ExpressionType.Coalesce: CompileCoalesceBinaryExpression(expr); break;
                case ExpressionType.Conditional: CompileConditionalExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.Constant: CompileConstantExpression(expr); break;
                case ExpressionType.Convert: CompileConvertUnaryExpression(expr); break;
                case ExpressionType.ConvertChecked: CompileConvertUnaryExpression(expr); break;
                case ExpressionType.Divide: CompileBinaryExpression(expr); break;
                case ExpressionType.Equal: CompileBinaryExpression(expr); break;
                case ExpressionType.ExclusiveOr: CompileBinaryExpression(expr); break;
                case ExpressionType.GreaterThan: CompileBinaryExpression(expr); break;
                case ExpressionType.GreaterThanOrEqual: CompileBinaryExpression(expr); break;
                case ExpressionType.Invoke: CompileInvocationExpression(expr); break;
                case ExpressionType.Lambda: CompileLambdaExpression(expr); break;
                case ExpressionType.LeftShift: CompileBinaryExpression(expr); break;
                case ExpressionType.LessThan: CompileBinaryExpression(expr); break;
                case ExpressionType.LessThanOrEqual: CompileBinaryExpression(expr); break;
                case ExpressionType.ListInit: CompileListInitExpression(expr); break;
                case ExpressionType.MemberAccess: CompileMemberExpression(expr); break;
                case ExpressionType.MemberInit: CompileMemberInitExpression(expr); break;
                case ExpressionType.Modulo: CompileBinaryExpression(expr); break;
                case ExpressionType.Multiply: CompileBinaryExpression(expr); break;
                case ExpressionType.MultiplyChecked: CompileBinaryExpression(expr); break;
                case ExpressionType.Negate: CompileUnaryExpression(expr); break;
                case ExpressionType.UnaryPlus: CompileUnaryExpression(expr); break;
                case ExpressionType.NegateChecked: CompileUnaryExpression(expr); break;
                case ExpressionType.New: CompileNewExpression(expr); break;
                case ExpressionType.NewArrayInit: CompileNewArrayExpression(expr); break;
                case ExpressionType.NewArrayBounds: CompileNewArrayExpression(expr); break;
                case ExpressionType.Not: CompileUnaryExpression(expr); break;
                case ExpressionType.NotEqual: CompileBinaryExpression(expr); break;
                case ExpressionType.Or: CompileBinaryExpression(expr); break;
                case ExpressionType.OrElse: CompileOrElseBinaryExpression(expr); break;
                case ExpressionType.Parameter: CompileParameterExpression(expr); break;
                case ExpressionType.Power: CompileBinaryExpression(expr); break;
                case ExpressionType.Quote: CompileQuoteUnaryExpression(expr); break;
                case ExpressionType.RightShift: CompileBinaryExpression(expr); break;
                case ExpressionType.Subtract: CompileBinaryExpression(expr); break;
                case ExpressionType.SubtractChecked: CompileBinaryExpression(expr); break;
                case ExpressionType.TypeAs: CompileUnaryExpression(expr); break;
                case ExpressionType.TypeIs: CompileTypeIsExpression(expr); break;
                case ExpressionType.Assign: CompileAssignBinaryExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.Block: CompileBlockExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.DebugInfo: CompileDebugInfoExpression(expr); break;
                case ExpressionType.Decrement: CompileUnaryExpression(expr); break;
                case ExpressionType.Default: CompileDefaultExpression(expr); break;
                case ExpressionType.Extension: CompileExtensionExpression(expr); break;
                case ExpressionType.Goto: CompileGotoExpression(expr); break;
                case ExpressionType.Increment: CompileUnaryExpression(expr); break;
                case ExpressionType.Index: CompileIndexExpression(expr); break;
                case ExpressionType.Label: CompileLabelExpression(expr); break;
                case ExpressionType.RuntimeVariables: CompileRuntimeVariablesExpression(expr); break;
                case ExpressionType.Loop: CompileLoopExpression(expr); break;
                case ExpressionType.Switch: CompileSwitchExpression(expr); break;
                case ExpressionType.Throw: CompileThrowUnaryExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.Try: CompileTryExpression(expr); break;
                case ExpressionType.Unbox: CompileUnboxUnaryExpression(expr); break;
                case ExpressionType.TypeEqual: CompileTypeEqualExpression(expr); break;
                case ExpressionType.OnesComplement: CompileUnaryExpression(expr); break;
                case ExpressionType.IsTrue: CompileUnaryExpression(expr); break;
                case ExpressionType.IsFalse: CompileUnaryExpression(expr); break;
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                default:
                    if (expr.CanReduce)
                    {
                        Compile(expr.Reduce());
                        break;
                    }
                    throw new PlatformNotSupportedException(SR.Format(SR.UnsupportedExpressionType, expr.NodeType));
            };
            Debug.Assert(_instructions.CurrentStackDepth == startingStackDepth + (expr.Type == typeof(void) ? 0 : 1),
                String.Format("{0} vs {1} for {2}", _instructions.CurrentStackDepth, startingStackDepth + (expr.Type == typeof(void) ? 0 : 1), expr.NodeType));
        }

        public void Compile(Expression expr)
        {
            bool pushLabelBlock = TryPushLabelBlock(expr);
            CompileNoLabelPush(expr);
            if (pushLabelBlock)
            {
                PopLabelBlock(_labelBlock.Kind);
            }
        }
    }

    internal abstract class ByRefUpdater
    {
        public readonly int ArgumentIndex;

        public ByRefUpdater(int argumentIndex)
        {
            ArgumentIndex = argumentIndex;
        }

        public abstract void Update(InterpretedFrame frame, object value);

        public virtual void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
        }
    }

    internal class ParameterByRefUpdater : ByRefUpdater
    {
        private readonly LocalVariable _parameter;

        public ParameterByRefUpdater(LocalVariable parameter, int argumentIndex)
            : base(argumentIndex)
        {
            _parameter = parameter;
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            if (_parameter.InClosure)
            {
                var box = frame.Closure[_parameter.Index];
                box.Value = value;
            }
            else if (_parameter.IsBoxed)
            {
                var box = (IStrongBox)frame.Data[_parameter.Index];
                box.Value = value;
            }
            else
            {
                frame.Data[_parameter.Index] = value;
            }
        }
    }

    internal class ArrayByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition _array, _index;

        public ArrayByRefUpdater(LocalDefinition array, LocalDefinition index, int argumentIndex)
            : base(argumentIndex)
        {
            _array = array;
            _index = index;
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var index = frame.Data[_index.Index];
            ((Array)frame.Data[_array.Index]).SetValue(value, (int)index);
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            locals.UndefineLocal(_array, instructions.Count);
            locals.UndefineLocal(_index, instructions.Count);
        }
    }

    internal class FieldByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition? _object;
        private readonly FieldInfo _field;

        public FieldByRefUpdater(LocalDefinition? obj, FieldInfo field, int argumentIndex)
            : base(argumentIndex)
        {
            _object = obj;
            _field = field;
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var obj = _object == null ? null : frame.Data[_object.Value.Index];
            _field.SetValue(obj, value);
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            if (_object != null)
            {
                locals.UndefineLocal(_object.Value, instructions.Count);
            }
        }
    }

    internal class PropertyByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition? _object;
        private readonly PropertyInfo _property;

        public PropertyByRefUpdater(LocalDefinition? obj, PropertyInfo property, int argumentIndex)
            : base(argumentIndex)
        {
            _object = obj;
            _property = property;
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var obj = _object == null ? null : frame.Data[_object.Value.Index];
            _property.SetValue(obj, value);
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            if (_object != null)
            {
                locals.UndefineLocal(_object.Value, instructions.Count);
            }
        }
    }

    internal class IndexMethodByRefUpdater : ByRefUpdater
    {
        private readonly MethodInfo _indexer;
        private readonly LocalDefinition? _obj;
        private readonly LocalDefinition[] _args;

        public IndexMethodByRefUpdater(LocalDefinition? obj, LocalDefinition[] args, MethodInfo indexer, int argumentIndex)
            : base(argumentIndex)
        {
            _obj = obj;
            _args = args;
            _indexer = indexer;
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            object[] args = new object[_args.Length + 1];
            for (int i = 0; i < args.Length - 1; i++)
            {
                args[i] = frame.Data[_args[i].Index];
            }
            args[args.Length - 1] = value;
            _indexer.Invoke(
                _obj == null ? null : frame.Data[_obj.Value.Index],
                args
            );
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            if (_obj != null)
            {
                locals.UndefineLocal(_obj.Value, instructions.Count);
            }

            for (int i = 0; i < _args.Length; i++)
            {
                locals.UndefineLocal(_args[i], instructions.Count);
            }
        }
    }
}
