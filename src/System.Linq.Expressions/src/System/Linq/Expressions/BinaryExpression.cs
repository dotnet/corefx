// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Represents an expression that has a binary operator.
    /// </summary>
    [DebuggerTypeProxy(typeof(Expression.BinaryExpressionProxy))]
    public class BinaryExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;

        internal BinaryExpression(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced. 
        /// </summary>
        public override bool CanReduce
        {
            get
            {
                // Only OpAssignments are reducible.
                return IsOpAssignment(NodeType);
            }
        }

        private static bool IsOpAssignment(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.AddAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.DivideAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ExclusiveOrAssign:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the right operand of the binary operation.
        /// </summary>
        public Expression Right
        {
            get { return _right; }
        }

        /// <summary>
        /// Gets the left operand of the binary operation.
        /// </summary>
        public Expression Left
        {
            get { return _left; }
        }

        /// <summary>
        /// Gets the implementing method for the binary operation.
        /// </summary>
        public MethodInfo Method
        {
            get { return GetMethod(); }
        }

        internal virtual MethodInfo GetMethod()
        {
            return null;
        }

        // Note: takes children in evaluation order, which is also the order
        // that ExpressionVisitor visits them. Having them this way reduces the
        // chances people will make a mistake and use an inconsistent order in
        // derived visitors.

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="left">The <see cref="Left" /> property of the result.</param>
        /// <param name="conversion">The <see cref="Conversion" /> property of the result.</param>
        /// <param name="right">The <see cref="Right" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public BinaryExpression Update(Expression left, LambdaExpression conversion, Expression right)
        {
            if (left == Left && right == Right && conversion == Conversion)
            {
                return this;
            }
            if (IsReferenceComparison)
            {
                if (NodeType == ExpressionType.Equal)
                {
                    return Expression.ReferenceEqual(left, right);
                }
                else
                {
                    return Expression.ReferenceNotEqual(left, right);
                }
            }
            return Expression.MakeBinary(NodeType, left, right, IsLiftedToNull, Method, conversion);
        }

        /// <summary>
        /// Reduces the binary expression node to a simpler expression. 
        /// If CanReduce returns true, this should return a valid expression.
        /// This method is allowed to return another node which itself 
        /// must be reduced.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            // Only reduce OpAssignment expressions.
            if (IsOpAssignment(NodeType))
            {
                switch (_left.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        return ReduceMember();

                    case ExpressionType.Index:
                        return ReduceIndex();

                    default:
                        return ReduceVariable();
                }
            }
            return this;
        }

        // Return the corresponding Op of an assignment op.
        private static ExpressionType GetBinaryOpFromAssignmentOp(ExpressionType op)
        {
            Debug.Assert(IsOpAssignment(op));
            switch (op)
            {
                case ExpressionType.AddAssign:
                    return ExpressionType.Add;
                case ExpressionType.AddAssignChecked:
                    return ExpressionType.AddChecked;
                case ExpressionType.SubtractAssign:
                    return ExpressionType.Subtract;
                case ExpressionType.SubtractAssignChecked:
                    return ExpressionType.SubtractChecked;
                case ExpressionType.MultiplyAssign:
                    return ExpressionType.Multiply;
                case ExpressionType.MultiplyAssignChecked:
                    return ExpressionType.MultiplyChecked;
                case ExpressionType.DivideAssign:
                    return ExpressionType.Divide;
                case ExpressionType.ModuloAssign:
                    return ExpressionType.Modulo;
                case ExpressionType.PowerAssign:
                    return ExpressionType.Power;
                case ExpressionType.AndAssign:
                    return ExpressionType.And;
                case ExpressionType.OrAssign:
                    return ExpressionType.Or;
                case ExpressionType.RightShiftAssign:
                    return ExpressionType.RightShift;
                case ExpressionType.LeftShiftAssign:
                    return ExpressionType.LeftShift;
                case ExpressionType.ExclusiveOrAssign:
                    return ExpressionType.ExclusiveOr;
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private Expression ReduceVariable()
        {
            // v (op)= r
            // ... is reduced into ...
            // v = v (op) r
            ExpressionType op = GetBinaryOpFromAssignmentOp(NodeType);
            Expression r = Expression.MakeBinary(op, _left, _right, false, Method);
            LambdaExpression conversion = GetConversion();
            if (conversion != null)
            {
                r = Expression.Invoke(conversion, r);
            }
            return Expression.Assign(_left, r);
        }

        private Expression ReduceMember()
        {
            MemberExpression member = (MemberExpression)_left;

            if (member.Expression == null)
            {
                // static member, reduce the same as variable
                return ReduceVariable();
            }
            else
            {
                // left.b (op)= r
                // ... is reduced into ...
                // temp1 = left
                // temp2 = temp1.b (op) r
                // temp1.b = temp2
                // temp2
                ParameterExpression temp1 = Variable(member.Expression.Type, "temp1");

                // 1. temp1 = left
                Expression e1 = Expression.Assign(temp1, member.Expression);

                // 2. temp2 = temp1.b (op) r
                ExpressionType op = GetBinaryOpFromAssignmentOp(NodeType);
                Expression e2 = Expression.MakeBinary(op, Expression.MakeMemberAccess(temp1, member.Member), _right, false, Method);
                LambdaExpression conversion = GetConversion();
                if (conversion != null)
                {
                    e2 = Expression.Invoke(conversion, e2);
                }
                ParameterExpression temp2 = Variable(e2.Type, "temp2");
                e2 = Expression.Assign(temp2, e2);

                // 3. temp1.b = temp2
                Expression e3 = Expression.Assign(Expression.MakeMemberAccess(temp1, member.Member), temp2);

                // 3. temp2
                Expression e4 = temp2;

                return Expression.Block(
                    new ParameterExpression[] { temp1, temp2 },
                    e1, e2, e3, e4
                );
            }
        }

        private Expression ReduceIndex()
        {
            // left[a0, a1, ... aN] (op)= r
            //
            // ... is reduced into ...
            //
            // tempObj = left
            // tempArg0 = a0
            // ...
            // tempArgN = aN
            // tempValue = tempObj[tempArg0, ... tempArgN] (op) r
            // tempObj[tempArg0, ... tempArgN] = tempValue

            var index = (IndexExpression)_left;

            var vars = new List<ParameterExpression>(index.Arguments.Count + 2);
            var exprs = new List<Expression>(index.Arguments.Count + 3);

            var tempObj = Expression.Variable(index.Object.Type, "tempObj");
            vars.Add(tempObj);
            exprs.Add(Expression.Assign(tempObj, index.Object));

            var tempArgs = new List<Expression>(index.Arguments.Count);
            foreach (var arg in index.Arguments)
            {
                var tempArg = Expression.Variable(arg.Type, "tempArg" + tempArgs.Count);
                vars.Add(tempArg);
                tempArgs.Add(tempArg);
                exprs.Add(Expression.Assign(tempArg, arg));
            }

            var tempIndex = Expression.MakeIndex(tempObj, index.Indexer, tempArgs);

            // tempValue = tempObj[tempArg0, ... tempArgN] (op) r
            ExpressionType binaryOp = GetBinaryOpFromAssignmentOp(NodeType);
            Expression op = Expression.MakeBinary(binaryOp, tempIndex, _right, false, Method);
            LambdaExpression conversion = GetConversion();
            if (conversion != null)
            {
                op = Expression.Invoke(conversion, op);
            }
            var tempValue = Expression.Variable(op.Type, "tempValue");
            vars.Add(tempValue);
            exprs.Add(Expression.Assign(tempValue, op));

            // tempObj[tempArg0, ... tempArgN] = tempValue
            exprs.Add(Expression.Assign(tempIndex, tempValue));

            return Expression.Block(vars, exprs);
        }

        /// <summary>
        /// Gets the type conversion function that is used by a coalescing or compound assignment operation.
        /// </summary>
        public LambdaExpression Conversion
        {
            get { return GetConversion(); }
        }

        internal virtual LambdaExpression GetConversion()
        {
            return null;
        }

        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator.
        /// </summary>
        public bool IsLifted
        {
            get
            {
                if (NodeType == ExpressionType.Coalesce || NodeType == ExpressionType.Assign)
                {
                    return false;
                }
                if (TypeUtils.IsNullableType(_left.Type))
                {
                    MethodInfo method = GetMethod();
                    return method == null ||
                        !TypeUtils.AreEquivalent(method.GetParametersCached()[0].ParameterType.GetNonRefType(), _left.Type);
                }
                return false;
            }
        }
        /// <summary>
        /// Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return type is lifted to a nullable type.
        /// </summary>
        public bool IsLiftedToNull
        {
            get
            {
                return IsLifted && TypeUtils.IsNullableType(Type);
            }
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitBinary(this);
        }

        internal static Expression Create(ExpressionType nodeType, Expression left, Expression right, Type type, MethodInfo method, LambdaExpression conversion)
        {
            if (nodeType == ExpressionType.Assign)
            {
                Debug.Assert(method == null && TypeUtils.AreEquivalent(type, left.Type));
                return new AssignBinaryExpression(left, right);
            }
            if (conversion != null)
            {
                Debug.Assert(method == null && TypeUtils.AreEquivalent(type, right.Type) && nodeType == ExpressionType.Coalesce);
                return new CoalesceConversionBinaryExpression(left, right, conversion);
            }
            if (method != null)
            {
                return new MethodBinaryExpression(nodeType, left, right, type, method);
            }
            if (type == typeof(bool))
            {
                return new LogicalBinaryExpression(nodeType, left, right);
            }
            return new SimpleBinaryExpression(nodeType, left, right, type);
        }

        internal bool IsLiftedLogical
        {
            get
            {
                Type left = _left.Type;
                Type right = _right.Type;
                MethodInfo method = GetMethod();
                ExpressionType kind = NodeType;

                return
                    (kind == ExpressionType.AndAlso || kind == ExpressionType.OrElse) &&
                    TypeUtils.AreEquivalent(right, left) &&
                    TypeUtils.IsNullableType(left) &&
                    method != null &&
                    TypeUtils.AreEquivalent(method.ReturnType, TypeUtils.GetNonNullableType(left));
            }
        }

        internal bool IsReferenceComparison
        {
            get
            {
                Type left = _left.Type;
                Type right = _right.Type;
                MethodInfo method = GetMethod();
                ExpressionType kind = NodeType;

                return (kind == ExpressionType.Equal || kind == ExpressionType.NotEqual) &&
                    method == null && !left.GetTypeInfo().IsValueType && !right.GetTypeInfo().IsValueType;
            }
        }

        //
        // For a userdefined type T which has op_False defined and L, R are
        // nullable, (L AndAlso R) is computed as:
        //
        // L.HasValue
        //     ? T.op_False(L.GetValueOrDefault())
        //         ? L
        //         : R.HasValue 
        //             ? (T?)(T.op_BitwiseAnd(L.GetValueOrDefault(), R.GetValueOrDefault()))
        //             : null
        //     : null
        //
        // For a userdefined type T which has op_True defined and L, R are
        // nullable, (L OrElse R)  is computed as:
        //
        // L.HasValue
        //     ? T.op_True(L.GetValueOrDefault())
        //         ? L
        //         : R.HasValue 
        //             ? (T?)(T.op_BitwiseOr(L.GetValueOrDefault(), R.GetValueOrDefault()))
        //             : null
        //     : null
        //
        //
        // This is the same behavior as VB. If you think about it, it makes
        // sense: it's combining the normal pattern for short-circuiting 
        // operators, with the normal pattern for lifted operations: if either
        // of the operands is null, the result is also null.
        //
        internal Expression ReduceUserdefinedLifted()
        {
            Debug.Assert(IsLiftedLogical);

            var left = Parameter(_left.Type, "left");
            var right = Parameter(Right.Type, "right");
            string opName = NodeType == ExpressionType.AndAlso ? "op_False" : "op_True";
            MethodInfo opTrueFalse = TypeUtils.GetBooleanOperator(Method.DeclaringType, opName);
            Debug.Assert(opTrueFalse != null);

            return Block(
                new[] { left },
                Assign(left, _left),
                Condition(
                    Property(left, "HasValue"),
                    Condition(
                        Call(opTrueFalse, Call(left, "GetValueOrDefault", null)),
                        left,
                        Block(
                            new[] { right },
                            Assign(right, _right),
                            Condition(
                                Property(right, "HasValue"),
                                Convert(
                                    Call(
                                        Method,
                                        Call(left, "GetValueOrDefault", null),
                                        Call(right, "GetValueOrDefault", null)
                                    ),
                                    Type
                                ),
                                Constant(null, Type)
                            )
                        )
                    ),
                    Constant(null, Type)
                )
            );
        }
    }

    // Optimized representation of simple logical expressions:
    // && || == != > < >= <=
    internal sealed class LogicalBinaryExpression : BinaryExpression
    {
        private readonly ExpressionType _nodeType;

        internal LogicalBinaryExpression(ExpressionType nodeType, Expression left, Expression right)
            : base(left, right)
        {
            _nodeType = nodeType;
        }

        public sealed override Type Type
        {
            get { return typeof(bool); }
        }

        public sealed override ExpressionType NodeType
        {
            get { return _nodeType; }
        }
    }

    // Optimized assignment node, only holds onto children
    internal sealed class AssignBinaryExpression : BinaryExpression
    {
        internal AssignBinaryExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        public sealed override Type Type
        {
            get { return Left.Type; }
        }

        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.Assign; }
        }
    }

    // Coalesce with conversion
    // This is not a frequently used node, but rather we want to save every
    // other BinaryExpression from holding onto the null conversion lambda
    internal sealed class CoalesceConversionBinaryExpression : BinaryExpression
    {
        private readonly LambdaExpression _conversion;

        internal CoalesceConversionBinaryExpression(Expression left, Expression right, LambdaExpression conversion)
            : base(left, right)
        {
            _conversion = conversion;
        }

        internal override LambdaExpression GetConversion()
        {
            return _conversion;
        }

        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.Coalesce; }
        }

        public sealed override Type Type
        {
            get { return Right.Type; }
        }
    }

    // OpAssign with conversion
    // This is not a frequently used node, but rather we want to save every
    // other BinaryExpression from holding onto the null conversion lambda
    internal sealed class OpAssignMethodConversionBinaryExpression : MethodBinaryExpression
    {
        private readonly LambdaExpression _conversion;

        internal OpAssignMethodConversionBinaryExpression(ExpressionType nodeType, Expression left, Expression right, Type type, MethodInfo method, LambdaExpression conversion)
            : base(nodeType, left, right, type, method)
        {
            _conversion = conversion;
        }

        internal override LambdaExpression GetConversion()
        {
            return _conversion;
        }
    }

    // Class that handles most binary expressions
    // If needed, it can be optimized even more (often Type == left.Type)
    internal class SimpleBinaryExpression : BinaryExpression
    {
        private readonly ExpressionType _nodeType;
        private readonly Type _type;

        internal SimpleBinaryExpression(ExpressionType nodeType, Expression left, Expression right, Type type)
            : base(left, right)
        {
            _nodeType = nodeType;
            _type = type;
        }

        public sealed override ExpressionType NodeType
        {
            get { return _nodeType; }
        }

        public sealed override Type Type
        {
            get { return _type; }
        }
    }

    // Class that handles binary expressions with a method
    // If needed, it can be optimized even more (often Type == method.ReturnType)
    internal class MethodBinaryExpression : SimpleBinaryExpression
    {
        private readonly MethodInfo _method;

        internal MethodBinaryExpression(ExpressionType nodeType, Expression left, Expression right, Type type, MethodInfo method)
            : base(nodeType, left, right, type)
        {
            _method = method;
        }

        internal override MethodInfo GetMethod()
        {
            return _method;
        }
    }

    public partial class Expression
    {
        #region Assign

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see ref="F:ExpressionType.Assign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Assign(Expression left, Expression right)
        {
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            TypeUtils.ValidateType(left.Type);
            TypeUtils.ValidateType(right.Type);
            if (!TypeUtils.AreReferenceAssignable(left.Type, right.Type))
            {
                throw Error.ExpressionTypeDoesNotMatchAssignment(right.Type, left.Type);
            }
            return new AssignBinaryExpression(left, right);
        }

        #endregion


        private static BinaryExpression GetUserDefinedBinaryOperator(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
        {
            // try exact match first
            MethodInfo method = GetUserDefinedBinaryOperator(binaryType, left.Type, right.Type, name);
            if (method != null)
            {
                return new MethodBinaryExpression(binaryType, left, right, method.ReturnType, method);
            }
            // try lifted call
            if (TypeUtils.IsNullableType(left.Type) && TypeUtils.IsNullableType(right.Type))
            {
                Type nnLeftType = TypeUtils.GetNonNullableType(left.Type);
                Type nnRightType = TypeUtils.GetNonNullableType(right.Type);
                method = GetUserDefinedBinaryOperator(binaryType, nnLeftType, nnRightType, name);
                if (method != null && method.ReturnType.GetTypeInfo().IsValueType && !TypeUtils.IsNullableType(method.ReturnType))
                {
                    if (method.ReturnType != typeof(bool) || liftToNull)
                    {
                        return new MethodBinaryExpression(binaryType, left, right, TypeUtils.GetNullableType(method.ReturnType), method);
                    }
                    else
                    {
                        return new MethodBinaryExpression(binaryType, left, right, typeof(bool), method);
                    }
                }
            }
            return null;
        }


        private static BinaryExpression GetMethodBasedBinaryOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, bool liftToNull)
        {
            System.Diagnostics.Debug.Assert(method != null);
            ValidateOperator(method);
            ParameterInfo[] pms = method.GetParametersCached();
            if (pms.Length != 2)
                throw Error.IncorrectNumberOfMethodCallArguments(method);
            if (ParameterIsAssignable(pms[0], left.Type) && ParameterIsAssignable(pms[1], right.Type))
            {
                ValidateParamswithOperandsOrThrow(pms[0].ParameterType, left.Type, binaryType, method.Name);
                ValidateParamswithOperandsOrThrow(pms[1].ParameterType, right.Type, binaryType, method.Name);
                return new MethodBinaryExpression(binaryType, left, right, method.ReturnType, method);
            }
            // check for lifted call
            if (TypeUtils.IsNullableType(left.Type) && TypeUtils.IsNullableType(right.Type) &&
                ParameterIsAssignable(pms[0], TypeUtils.GetNonNullableType(left.Type)) &&
                ParameterIsAssignable(pms[1], TypeUtils.GetNonNullableType(right.Type)) &&
                method.ReturnType.GetTypeInfo().IsValueType && !TypeUtils.IsNullableType(method.ReturnType))
            {
                if (method.ReturnType != typeof(bool) || liftToNull)
                {
                    return new MethodBinaryExpression(binaryType, left, right, TypeUtils.GetNullableType(method.ReturnType), method);
                }
                else
                {
                    return new MethodBinaryExpression(binaryType, left, right, typeof(bool), method);
                }
            }
            throw Error.OperandTypesDoNotMatchParameters(binaryType, method.Name);
        }

        private static BinaryExpression GetMethodBasedAssignOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, LambdaExpression conversion, bool liftToNull)
        {
            BinaryExpression b = GetMethodBasedBinaryOperator(binaryType, left, right, method, liftToNull);
            if (conversion == null)
            {
                // return type must be assignable back to the left type
                if (!TypeUtils.AreReferenceAssignable(left.Type, b.Type))
                {
                    throw Error.UserDefinedOpMustHaveValidReturnType(binaryType, b.Method.Name);
                }
            }
            else
            {
                // add the conversion to the result
                ValidateOpAssignConversionLambda(conversion, b.Left, b.Method, b.NodeType);
                b = new OpAssignMethodConversionBinaryExpression(b.NodeType, b.Left, b.Right, b.Left.Type, b.Method, conversion);
            }
            return b;
        }


        private static BinaryExpression GetUserDefinedBinaryOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
        {
            BinaryExpression b = GetUserDefinedBinaryOperator(binaryType, name, left, right, liftToNull);
            if (b != null)
            {
                ParameterInfo[] pis = b.Method.GetParametersCached();
                ValidateParamswithOperandsOrThrow(pis[0].ParameterType, left.Type, binaryType, name);
                ValidateParamswithOperandsOrThrow(pis[1].ParameterType, right.Type, binaryType, name);
                return b;
            }
            throw Error.BinaryOperatorNotDefined(binaryType, left.Type, right.Type);
        }

        private static BinaryExpression GetUserDefinedAssignOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, LambdaExpression conversion, bool liftToNull)
        {
            BinaryExpression b = GetUserDefinedBinaryOperatorOrThrow(binaryType, name, left, right, liftToNull);
            if (conversion == null)
            {
                // return type must be assignable back to the left type
                if (!TypeUtils.AreReferenceAssignable(left.Type, b.Type))
                {
                    throw Error.UserDefinedOpMustHaveValidReturnType(binaryType, b.Method.Name);
                }
            }
            else
            {
                // add the conversion to the result
                ValidateOpAssignConversionLambda(conversion, b.Left, b.Method, b.NodeType);
                b = new OpAssignMethodConversionBinaryExpression(b.NodeType, b.Left, b.Right, b.Left.Type, b.Method, conversion);
            }
            return b;
        }


        private static MethodInfo GetUserDefinedBinaryOperator(ExpressionType binaryType, Type leftType, Type rightType, string name)
        {
            // This algorithm is wrong, we should be checking for uniqueness and erroring if
            // it is defined on both types.
            Type[] types = new Type[] { leftType, rightType };
            Type nnLeftType = TypeUtils.GetNonNullableType(leftType);
            Type nnRightType = TypeUtils.GetNonNullableType(rightType);
            MethodInfo method = nnLeftType.GetAnyStaticMethodValidated(name, types);
            if (method == null && !TypeUtils.AreEquivalent(leftType, rightType))
            {
                method = nnRightType.GetAnyStaticMethodValidated(name, types);
            }

            if (IsLiftingConditionalLogicalOperator(leftType, rightType, method, binaryType))
            {
                method = GetUserDefinedBinaryOperator(binaryType, nnLeftType, nnRightType, name);
            }
            return method;
        }


        private static bool IsLiftingConditionalLogicalOperator(Type left, Type right, MethodInfo method, ExpressionType binaryType)
        {
            return TypeUtils.IsNullableType(right) &&
                    TypeUtils.IsNullableType(left) &&
                    method == null &&
                    (binaryType == ExpressionType.AndAlso || binaryType == ExpressionType.OrElse);
        }


        internal static bool ParameterIsAssignable(ParameterInfo pi, Type argType)
        {
            Type pType = pi.ParameterType;
            if (pType.IsByRef)
                pType = pType.GetElementType();
            return TypeUtils.AreReferenceAssignable(pType, argType);
        }


        private static void ValidateParamswithOperandsOrThrow(Type paramType, Type operandType, ExpressionType exprType, string name)
        {
            if (TypeUtils.IsNullableType(paramType) && !TypeUtils.IsNullableType(operandType))
            {
                throw Error.OperandTypesDoNotMatchParameters(exprType, name);
            }
        }


        private static void ValidateOperator(MethodInfo method)
        {
            System.Diagnostics.Debug.Assert(method != null);
            ValidateMethodInfo(method);
            if (!method.IsStatic)
                throw Error.UserDefinedOperatorMustBeStatic(method);
            if (method.ReturnType == typeof(void))
                throw Error.UserDefinedOperatorMustNotBeVoid(method);
        }


        private static void ValidateMethodInfo(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
                throw Error.MethodIsGeneric(method);
            if (method.ContainsGenericParameters)
                throw Error.MethodContainsGenericParameters(method);
        }


        private static bool IsNullComparison(Expression left, Expression right)
        {
            // If we have x==null, x!=null, null==x or null!=x where x is
            // nullable but not null, then this is treated as a call to x.HasValue
            // and is legal even if there is no equality operator defined on the
            // type of x.
            if (IsNullConstant(left) && !IsNullConstant(right) && TypeUtils.IsNullableType(right.Type))
            {
                return true;
            }
            if (IsNullConstant(right) && !IsNullConstant(left) && TypeUtils.IsNullableType(left.Type))
            {
                return true;
            }
            return false;
        }


        // Note: this has different meaning than ConstantCheck.IsNull
        // That function attempts to determine if the result of a tree will be
        // null at runtime. This function is used at tree construction time and
        // only looks for a ConstantExpression with a null Value. It can't
        // become "smarter" or that would break tree construction.
        private static bool IsNullConstant(Expression e)
        {
            var c = e as ConstantExpression;
            return c != null && c.Value == null;
        }


        private static void ValidateUserDefinedConditionalLogicOperator(ExpressionType nodeType, Type left, Type right, MethodInfo method)
        {
            ValidateOperator(method);
            ParameterInfo[] pms = method.GetParametersCached();
            if (pms.Length != 2)
                throw Error.IncorrectNumberOfMethodCallArguments(method);
            if (!ParameterIsAssignable(pms[0], left))
            {
                if (!(TypeUtils.IsNullableType(left) && ParameterIsAssignable(pms[0], TypeUtils.GetNonNullableType(left))))
                    throw Error.OperandTypesDoNotMatchParameters(nodeType, method.Name);
            }
            if (!ParameterIsAssignable(pms[1], right))
            {
                if (!(TypeUtils.IsNullableType(right) && ParameterIsAssignable(pms[1], TypeUtils.GetNonNullableType(right))))
                    throw Error.OperandTypesDoNotMatchParameters(nodeType, method.Name);
            }
            if (pms[0].ParameterType != pms[1].ParameterType)
            {
                throw Error.UserDefinedOpMustHaveConsistentTypes(nodeType, method.Name);
            }
            if (method.ReturnType != pms[0].ParameterType)
            {
                throw Error.UserDefinedOpMustHaveConsistentTypes(nodeType, method.Name);
            }
            if (IsValidLiftedConditionalLogicalOperator(left, right, pms))
            {
                left = TypeUtils.GetNonNullableType(left);
                right = TypeUtils.GetNonNullableType(left);
            }
            MethodInfo opTrue = TypeUtils.GetBooleanOperator(method.DeclaringType, "op_True");
            MethodInfo opFalse = TypeUtils.GetBooleanOperator(method.DeclaringType, "op_False");
            if (opTrue == null || opTrue.ReturnType != typeof(bool) ||
                opFalse == null || opFalse.ReturnType != typeof(bool))
            {
                throw Error.LogicalOperatorMustHaveBooleanOperators(nodeType, method.Name);
            }
            VerifyOpTrueFalse(nodeType, left, opFalse);
            VerifyOpTrueFalse(nodeType, left, opTrue);
        }

        private static void VerifyOpTrueFalse(ExpressionType nodeType, Type left, MethodInfo opTrue)
        {
            ParameterInfo[] pmsOpTrue = opTrue.GetParametersCached();
            if (pmsOpTrue.Length != 1)
                throw Error.IncorrectNumberOfMethodCallArguments(opTrue);

            if (!ParameterIsAssignable(pmsOpTrue[0], left))
            {
                if (!(TypeUtils.IsNullableType(left) && ParameterIsAssignable(pmsOpTrue[0], TypeUtils.GetNonNullableType(left))))
                    throw Error.OperandTypesDoNotMatchParameters(nodeType, opTrue.Name);
            }
        }

        private static bool IsValidLiftedConditionalLogicalOperator(Type left, Type right, ParameterInfo[] pms)
        {
            return TypeUtils.AreEquivalent(left, right) &&
                   TypeUtils.IsNullableType(right) &&
                   TypeUtils.AreEquivalent(pms[1].ParameterType, TypeUtils.GetNonNullableType(right));
        }


        /// <summary>
        /// Creates a BinaryExpression, given the left and right operands, by calling an appropriate factory method.
        /// </summary>
        /// <param name="binaryType">The ExpressionType that specifies the type of binary operation.</param>
        /// <param name="left">An Expression that represents the left operand.</param>
        /// <param name="right">An Expression that represents the right operand.</param>
        /// <returns>The BinaryExpression that results from calling the appropriate factory method.</returns>
        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
        {
            return MakeBinary(binaryType, left, right, false, null, null);
        }

        /// <summary>
        /// Creates a BinaryExpression, given the left and right operands, by calling an appropriate factory method.
        /// </summary>
        /// <param name="binaryType">The ExpressionType that specifies the type of binary operation.</param>
        /// <param name="left">An Expression that represents the left operand.</param>
        /// <param name="right">An Expression that represents the right operand.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <param name="method">A MethodInfo that specifies the implementing method.</param>
        /// <returns>The BinaryExpression that results from calling the appropriate factory method.</returns>
        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            return MakeBinary(binaryType, left, right, liftToNull, method, null);
        }

        ///
        /// <summary>
        /// Creates a BinaryExpression, given the left and right operands, by calling an appropriate factory method.
        /// </summary>
        /// <param name="binaryType">The ExpressionType that specifies the type of binary operation.</param>
        /// <param name="left">An Expression that represents the left operand.</param>
        /// <param name="right">An Expression that represents the right operand.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <param name="method">A MethodInfo that specifies the implementing method.</param>
        /// <param name="conversion">A LambdaExpression that represents a type conversion function. This parameter is used if binaryType is Coalesce or compound assignment.</param>
        /// <returns>The BinaryExpression that results from calling the appropriate factory method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
        {
            switch (binaryType)
            {
                case ExpressionType.Add:
                    return Add(left, right, method);
                case ExpressionType.AddChecked:
                    return AddChecked(left, right, method);
                case ExpressionType.Subtract:
                    return Subtract(left, right, method);
                case ExpressionType.SubtractChecked:
                    return SubtractChecked(left, right, method);
                case ExpressionType.Multiply:
                    return Multiply(left, right, method);
                case ExpressionType.MultiplyChecked:
                    return MultiplyChecked(left, right, method);
                case ExpressionType.Divide:
                    return Divide(left, right, method);
                case ExpressionType.Modulo:
                    return Modulo(left, right, method);
                case ExpressionType.Power:
                    return Power(left, right, method);
                case ExpressionType.And:
                    return And(left, right, method);
                case ExpressionType.AndAlso:
                    return AndAlso(left, right, method);
                case ExpressionType.Or:
                    return Or(left, right, method);
                case ExpressionType.OrElse:
                    return OrElse(left, right, method);
                case ExpressionType.LessThan:
                    return LessThan(left, right, liftToNull, method);
                case ExpressionType.LessThanOrEqual:
                    return LessThanOrEqual(left, right, liftToNull, method);
                case ExpressionType.GreaterThan:
                    return GreaterThan(left, right, liftToNull, method);
                case ExpressionType.GreaterThanOrEqual:
                    return GreaterThanOrEqual(left, right, liftToNull, method);
                case ExpressionType.Equal:
                    return Equal(left, right, liftToNull, method);
                case ExpressionType.NotEqual:
                    return NotEqual(left, right, liftToNull, method);
                case ExpressionType.ExclusiveOr:
                    return ExclusiveOr(left, right, method);
                case ExpressionType.Coalesce:
                    return Coalesce(left, right, conversion);
                case ExpressionType.ArrayIndex:
                    return ArrayIndex(left, right);
                case ExpressionType.RightShift:
                    return RightShift(left, right, method);
                case ExpressionType.LeftShift:
                    return LeftShift(left, right, method);
                case ExpressionType.Assign:
                    return Assign(left, right);
                case ExpressionType.AddAssign:
                    return AddAssign(left, right, method, conversion);
                case ExpressionType.AndAssign:
                    return AndAssign(left, right, method, conversion);
                case ExpressionType.DivideAssign:
                    return DivideAssign(left, right, method, conversion);
                case ExpressionType.ExclusiveOrAssign:
                    return ExclusiveOrAssign(left, right, method, conversion);
                case ExpressionType.LeftShiftAssign:
                    return LeftShiftAssign(left, right, method, conversion);
                case ExpressionType.ModuloAssign:
                    return ModuloAssign(left, right, method, conversion);
                case ExpressionType.MultiplyAssign:
                    return MultiplyAssign(left, right, method, conversion);
                case ExpressionType.OrAssign:
                    return OrAssign(left, right, method, conversion);
                case ExpressionType.PowerAssign:
                    return PowerAssign(left, right, method, conversion);
                case ExpressionType.RightShiftAssign:
                    return RightShiftAssign(left, right, method, conversion);
                case ExpressionType.SubtractAssign:
                    return SubtractAssign(left, right, method, conversion);
                case ExpressionType.AddAssignChecked:
                    return AddAssignChecked(left, right, method, conversion);
                case ExpressionType.SubtractAssignChecked:
                    return SubtractAssignChecked(left, right, method, conversion);
                case ExpressionType.MultiplyAssignChecked:
                    return MultiplyAssignChecked(left, right, method, conversion);
                default:
                    throw Error.UnhandledBinary(binaryType);
            }
        }

        #region Equality Operators


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an equality comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Equal"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Equal(Expression left, Expression right)
        {
            return Equal(left, right, false, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an equality comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Equal"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.IsLiftedToNull"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                return GetEqualityComparisonOperator(ExpressionType.Equal, "op_Equality", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Equal, left, right, method, liftToNull);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a reference equality comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Equal"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ReferenceEqual(Expression left, Expression right)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (TypeUtils.HasReferenceEquality(left.Type, right.Type))
            {
                return new LogicalBinaryExpression(ExpressionType.Equal, left, right);
            }
            throw Error.ReferenceEqualityNotDefined(left.Type, right.Type);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an inequality comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.NotEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression NotEqual(Expression left, Expression right)
        {
            return NotEqual(left, right, false, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an inequality comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.NotEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.IsLiftedToNull"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                return GetEqualityComparisonOperator(ExpressionType.NotEqual, "op_Inequality", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.NotEqual, left, right, method, liftToNull);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a reference inequality comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.NotEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ReferenceNotEqual(Expression left, Expression right)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (TypeUtils.HasReferenceEquality(left.Type, right.Type))
            {
                return new LogicalBinaryExpression(ExpressionType.NotEqual, left, right);
            }
            throw Error.ReferenceEqualityNotDefined(left.Type, right.Type);
        }

        private static BinaryExpression GetEqualityComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
        {
            // known comparison - numeric types, bools, object, enums
            if (left.Type == right.Type && (TypeUtils.IsNumeric(left.Type) ||
                left.Type == typeof(object) ||
                TypeUtils.IsBool(left.Type) ||
                TypeUtils.GetNonNullableType(left.Type).GetTypeInfo().IsEnum))
            {
                if (TypeUtils.IsNullableType(left.Type) && liftToNull)
                {
                    return new SimpleBinaryExpression(binaryType, left, right, typeof(bool?));
                }
                else
                {
                    return new LogicalBinaryExpression(binaryType, left, right);
                }
            }
            // look for user defined operator
            BinaryExpression b = GetUserDefinedBinaryOperator(binaryType, opName, left, right, liftToNull);
            if (b != null)
            {
                return b;
            }
            if (TypeUtils.HasBuiltInEqualityOperator(left.Type, right.Type) || IsNullComparison(left, right))
            {
                if (TypeUtils.IsNullableType(left.Type) && liftToNull)
                {
                    return new SimpleBinaryExpression(binaryType, left, right, typeof(bool?));
                }
                else
                {
                    return new LogicalBinaryExpression(binaryType, left, right);
                }
            }
            throw Error.BinaryOperatorNotDefined(binaryType, left.Type, right.Type);
        }

        #endregion

        #region Comparison Expressions


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "greater than" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.GreaterThan"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression GreaterThan(Expression left, Expression right)
        {
            return GreaterThan(left, right, false, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "greater than" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.GreaterThan"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.IsLiftedToNull"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.GreaterThan, "op_GreaterThan", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.GreaterThan, left, right, method, liftToNull);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "less than" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LessThan"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>

        public static BinaryExpression LessThan(Expression left, Expression right)
        {
            return LessThan(left, right, false, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "less than" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LessThan"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.IsLiftedToNull"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.LessThan, "op_LessThan", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.LessThan, left, right, method, liftToNull);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "greater than or equal" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.GreaterThanOrEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
        {
            return GreaterThanOrEqual(left, right, false, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "greater than or equal" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.GreaterThanOrEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.IsLiftedToNull"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.GreaterThanOrEqual, "op_GreaterThanOrEqual", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.GreaterThanOrEqual, left, right, method, liftToNull);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "less than or equal" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LessThanOrEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
        {
            return LessThanOrEqual(left, right, false, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a "less than or equal" numeric comparison.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="liftToNull">true to set IsLiftedToNull to true; false to set IsLiftedToNull to false.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LessThanOrEqual"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.IsLiftedToNull"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.LessThanOrEqual, "op_LessThanOrEqual", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.LessThanOrEqual, left, right, method, liftToNull);
        }


        private static BinaryExpression GetComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
        {
            if (left.Type == right.Type && TypeUtils.IsNumeric(left.Type))
            {
                if (TypeUtils.IsNullableType(left.Type) && liftToNull)
                {
                    return new SimpleBinaryExpression(binaryType, left, right, typeof(bool?));
                }
                else
                {
                    return new LogicalBinaryExpression(binaryType, left, right);
                }
            }
            return GetUserDefinedBinaryOperatorOrThrow(binaryType, opName, left, right, liftToNull);
        }

        #endregion

        #region Boolean Expressions


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if it has to.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AndAlso"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression AndAlso(Expression left, Expression right)
        {
            return AndAlso(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if it has to.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AndAlso"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            Type returnType;
            if (method == null)
            {
                if (left.Type == right.Type)
                {
                    if (left.Type == typeof(bool))
                    {
                        return new LogicalBinaryExpression(ExpressionType.AndAlso, left, right);
                    }
                    else if (left.Type == typeof(bool?))
                    {
                        return new SimpleBinaryExpression(ExpressionType.AndAlso, left, right, left.Type);
                    }
                }
                method = GetUserDefinedBinaryOperator(ExpressionType.AndAlso, left.Type, right.Type, "op_BitwiseAnd");
                if (method != null)
                {
                    ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
                    returnType = (TypeUtils.IsNullableType(left.Type) && TypeUtils.AreEquivalent(method.ReturnType, TypeUtils.GetNonNullableType(left.Type))) ? left.Type : method.ReturnType;
                    return new MethodBinaryExpression(ExpressionType.AndAlso, left, right, returnType, method);
                }
                throw Error.BinaryOperatorNotDefined(ExpressionType.AndAlso, left.Type, right.Type);
            }
            ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
            returnType = (TypeUtils.IsNullableType(left.Type) && TypeUtils.AreEquivalent(method.ReturnType, TypeUtils.GetNonNullableType(left.Type))) ? left.Type : method.ReturnType;
            return new MethodBinaryExpression(ExpressionType.AndAlso, left, right, returnType, method);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a conditional OR operation that evaluates the second operand only if it has to.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.OrElse"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression OrElse(Expression left, Expression right)
        {
            return OrElse(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a conditional OR operation that evaluates the second operand only if it has to.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.OrElse"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            Type returnType;
            if (method == null)
            {
                if (left.Type == right.Type)
                {
                    if (left.Type == typeof(bool))
                    {
                        return new LogicalBinaryExpression(ExpressionType.OrElse, left, right);
                    }
                    else if (left.Type == typeof(bool?))
                    {
                        return new SimpleBinaryExpression(ExpressionType.OrElse, left, right, left.Type);
                    }
                }
                method = GetUserDefinedBinaryOperator(ExpressionType.OrElse, left.Type, right.Type, "op_BitwiseOr");
                if (method != null)
                {
                    ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
                    returnType = (TypeUtils.IsNullableType(left.Type) && method.ReturnType == TypeUtils.GetNonNullableType(left.Type)) ? left.Type : method.ReturnType;
                    return new MethodBinaryExpression(ExpressionType.OrElse, left, right, returnType, method);
                }
                throw Error.BinaryOperatorNotDefined(ExpressionType.OrElse, left.Type, right.Type);
            }
            ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
            returnType = (TypeUtils.IsNullableType(left.Type) && method.ReturnType == TypeUtils.GetNonNullableType(left.Type)) ? left.Type : method.ReturnType;
            return new MethodBinaryExpression(ExpressionType.OrElse, left, right, returnType, method);
        }

        #endregion

        #region Coalescing Expressions


        /// <summary>
        /// Creates a BinaryExpression that represents a coalescing operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A BinaryExpression that has the NodeType property equal to Coalesce and the Left and Right properties set to the specified values.</returns>
        public static BinaryExpression Coalesce(Expression left, Expression right)
        {
            return Coalesce(left, right, null);
        }


        /// <summary>
        /// Creates a BinaryExpression that represents a coalescing operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="conversion">A LambdaExpression to set the Conversion property equal to.</param>
        /// <returns>A BinaryExpression that has the NodeType property equal to Coalesce and the Left, Right and Conversion properties set to the specified values.
        /// </returns>
        public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");

            if (conversion == null)
            {
                Type resultType = ValidateCoalesceArgTypes(left.Type, right.Type);
                return new SimpleBinaryExpression(ExpressionType.Coalesce, left, right, resultType);
            }

            if (left.Type.GetTypeInfo().IsValueType && !TypeUtils.IsNullableType(left.Type))
            {
                throw Error.CoalesceUsedOnNonNullType();
            }

            Type delegateType = conversion.Type;
            Debug.Assert(typeof(System.MulticastDelegate).IsAssignableFrom(delegateType) && delegateType != typeof(System.MulticastDelegate));
            MethodInfo method = delegateType.GetMethod("Invoke");
            if (method.ReturnType == typeof(void))
            {
                throw Error.UserDefinedOperatorMustNotBeVoid(conversion);
            }
            ParameterInfo[] pms = method.GetParametersCached();
            Debug.Assert(pms.Length == conversion.Parameters.Count);
            if (pms.Length != 1)
            {
                throw Error.IncorrectNumberOfMethodCallArguments(conversion);
            }
            // The return type must match exactly.
            // We could weaken this restriction and
            // say that the return type must be assignable to from
            // the return type of the lambda.
            if (!TypeUtils.AreEquivalent(method.ReturnType, right.Type))
            {
                throw Error.OperandTypesDoNotMatchParameters(ExpressionType.Coalesce, conversion.ToString());
            }
            // The parameter of the conversion lambda must either be assignable
            // from the erased or unerased type of the left hand side.
            if (!ParameterIsAssignable(pms[0], TypeUtils.GetNonNullableType(left.Type)) &&
                !ParameterIsAssignable(pms[0], left.Type))
            {
                throw Error.OperandTypesDoNotMatchParameters(ExpressionType.Coalesce, conversion.ToString());
            }
            return new CoalesceConversionBinaryExpression(left, right, conversion);
        }


        private static Type ValidateCoalesceArgTypes(Type left, Type right)
        {
            Type leftStripped = TypeUtils.GetNonNullableType(left);
            if (left.GetTypeInfo().IsValueType && !TypeUtils.IsNullableType(left))
            {
                throw Error.CoalesceUsedOnNonNullType();
            }
            else if (TypeUtils.IsNullableType(left) && TypeUtils.IsImplicitlyConvertible(right, leftStripped))
            {
                return leftStripped;
            }
            else if (TypeUtils.IsImplicitlyConvertible(right, left))
            {
                return left;
            }
            else if (TypeUtils.IsImplicitlyConvertible(leftStripped, right))
            {
                return right;
            }
            else
            {
                throw Error.ArgumentTypesMustMatch();
            }
        }



        #endregion

        #region Arithmetic Expressions


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic addition operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Add"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Add(Expression left, Expression right)
        {
            return Add(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic addition operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Add"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.Add, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Add, "op_Addition", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Add, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an addition assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression AddAssign(Expression left, Expression right)
        {
            return AddAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an addition assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AddAssign(Expression left, Expression right, MethodInfo method)
        {
            return AddAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an addition assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>

        public static BinaryExpression AddAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.AddAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.AddAssign, "op_Addition", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.AddAssign, left, right, method, conversion, true);
        }

        private static void ValidateOpAssignConversionLambda(LambdaExpression conversion, Expression left, MethodInfo method, ExpressionType nodeType)
        {
            Type delegateType = conversion.Type;
            Debug.Assert(typeof(System.MulticastDelegate).IsAssignableFrom(delegateType) && delegateType != typeof(System.MulticastDelegate));
            MethodInfo mi = delegateType.GetMethod("Invoke");
            ParameterInfo[] pms = mi.GetParametersCached();
            Debug.Assert(pms.Length == conversion.Parameters.Count);
            if (pms.Length != 1)
            {
                throw Error.IncorrectNumberOfMethodCallArguments(conversion);
            }
            if (!TypeUtils.AreEquivalent(mi.ReturnType, left.Type))
            {
                throw Error.OperandTypesDoNotMatchParameters(nodeType, conversion.ToString());
            }
            if (method != null)
            {
                // The parameter type of conversion lambda must be the same as the return type of the overload method
                if (!TypeUtils.AreEquivalent(pms[0].ParameterType, method.ReturnType))
                {
                    throw Error.OverloadOperatorTypeDoesNotMatchConversionType(nodeType, conversion.ToString());
                }
            }
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an addition assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to 
        /// <see cref="F:ExpressionType.AddAssignChecked"/> and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> 
        /// properties set to the specified values.
        /// </returns>
        public static BinaryExpression AddAssignChecked(Expression left, Expression right)
        {
            return AddAssignChecked(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an addition assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AddAssignChecked(Expression left, Expression right, MethodInfo method)
        {
            return AddAssignChecked(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an addition assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AddAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");

            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.AddAssignChecked, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.AddAssignChecked, "op_Addition", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.AddAssignChecked, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic addition operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression AddChecked(Expression left, Expression right)
        {
            return AddChecked(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic addition operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AddChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.AddChecked, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.AddChecked, "op_Addition", left, right, false);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.AddChecked, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic subtraction operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Subtract"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Subtract(Expression left, Expression right)
        {
            return Subtract(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic subtraction operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Subtract"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.Subtract, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Subtract, "op_Subtraction", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Subtract, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a subtraction assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression SubtractAssign(Expression left, Expression right)
        {
            return SubtractAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a subtraction assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression SubtractAssign(Expression left, Expression right, MethodInfo method)
        {
            return SubtractAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a subtraction assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression SubtractAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.SubtractAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.SubtractAssign, "op_Subtraction", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.SubtractAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a subtraction assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression SubtractAssignChecked(Expression left, Expression right)
        {
            return SubtractAssignChecked(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a subtraction assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method)
        {
            return SubtractAssignChecked(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a subtraction assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression SubtractAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.SubtractAssignChecked, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.SubtractAssignChecked, "op_Subtraction", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.SubtractAssignChecked, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic subtraction operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression SubtractChecked(Expression left, Expression right)
        {
            return SubtractChecked(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic subtraction operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.SubtractChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.SubtractChecked, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.SubtractChecked, "op_Subtraction", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.SubtractChecked, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic division operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Divide"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Divide(Expression left, Expression right)
        {
            return Divide(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic division operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Divide"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.Divide, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Divide, "op_Division", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Divide, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a division assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.DivideAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression DivideAssign(Expression left, Expression right)
        {
            return DivideAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a division assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.DivideAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression DivideAssign(Expression left, Expression right, MethodInfo method)
        {
            return DivideAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a division assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.DivideAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression DivideAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.DivideAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.DivideAssign, "op_Division", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.DivideAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic remainder operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Modulo"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Modulo(Expression left, Expression right)
        {
            return Modulo(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic remainder operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Modulo"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.Modulo, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Modulo, "op_Modulus", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Modulo, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a remainder assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ModuloAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression ModuloAssign(Expression left, Expression right)
        {
            return ModuloAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a remainder assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ModuloAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ModuloAssign(Expression left, Expression right, MethodInfo method)
        {
            return ModuloAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a remainder assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ModuloAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ModuloAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.ModuloAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.ModuloAssign, "op_Modulus", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.ModuloAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic multiplication operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Multiply"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            return Multiply(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic multiplication operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Multiply"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.Multiply, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Multiply, "op_Multiply", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Multiply, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a multiplication assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression MultiplyAssign(Expression left, Expression right)
        {
            return MultiplyAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a multiplication assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression MultiplyAssign(Expression left, Expression right, MethodInfo method)
        {
            return MultiplyAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a multiplication assignment operation that does not have overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression MultiplyAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.MultiplyAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.MultiplyAssign, "op_Multiply", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.MultiplyAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a multiplication assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right)
        {
            return MultiplyAssignChecked(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a multiplication assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method)
        {
            return MultiplyAssignChecked(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a multiplication assignment operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyAssignChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression MultiplyAssignChecked(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.MultiplyAssignChecked, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.MultiplyAssignChecked, "op_Multiply", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.MultiplyAssignChecked, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic multiplication operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression MultiplyChecked(Expression left, Expression right)
        {
            return MultiplyChecked(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an arithmetic multiplication operation that has overflow checking.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.MultiplyChecked"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsArithmetic(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.MultiplyChecked, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.MultiplyChecked, "op_Multiply", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.MultiplyChecked, left, right, method, true);
        }

        private static bool IsSimpleShift(Type left, Type right)
        {
            return TypeUtils.IsInteger(left)
                && TypeUtils.GetNonNullableType(right) == typeof(int);
        }

        private static Type GetResultTypeOfShift(Type left, Type right)
        {
            if (!left.IsNullableType() && right.IsNullableType())
            {
                // lift the result type to Nullable<T>
                return typeof(Nullable<>).MakeGenericType(left);
            }
            return left;
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise left-shift operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LeftShift"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression LeftShift(Expression left, Expression right)
        {
            return LeftShift(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise left-shift operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LeftShift"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (IsSimpleShift(left.Type, right.Type))
                {
                    Type resultType = GetResultTypeOfShift(left.Type, right.Type);
                    return new SimpleBinaryExpression(ExpressionType.LeftShift, left, right, resultType);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.LeftShift, "op_LeftShift", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.LeftShift, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise left-shift assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LeftShiftAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression LeftShiftAssign(Expression left, Expression right)
        {
            return LeftShiftAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise left-shift assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LeftShiftAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method)
        {
            return LeftShiftAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise left-shift assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.LeftShiftAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression LeftShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (IsSimpleShift(left.Type, right.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    Type resultType = GetResultTypeOfShift(left.Type, right.Type);
                    return new SimpleBinaryExpression(ExpressionType.LeftShiftAssign, left, right, resultType);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.LeftShiftAssign, "op_LeftShift", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.LeftShiftAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise right-shift operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.RightShift"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression RightShift(Expression left, Expression right)
        {
            return RightShift(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise right-shift operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.RightShift"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (IsSimpleShift(left.Type, right.Type))
                {
                    Type resultType = GetResultTypeOfShift(left.Type, right.Type);
                    return new SimpleBinaryExpression(ExpressionType.RightShift, left, right, resultType);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.RightShift, "op_RightShift", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.RightShift, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise right-shift assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.RightShiftAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression RightShiftAssign(Expression left, Expression right)
        {
            return RightShiftAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise right-shift assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.RightShiftAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression RightShiftAssign(Expression left, Expression right, MethodInfo method)
        {
            return RightShiftAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise right-shift assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.RightShiftAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression RightShiftAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (IsSimpleShift(left.Type, right.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    Type resultType = GetResultTypeOfShift(left.Type, right.Type);
                    return new SimpleBinaryExpression(ExpressionType.RightShiftAssign, left, right, resultType);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.RightShiftAssign, "op_RightShift", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.RightShiftAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise AND operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.And"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression And(Expression left, Expression right)
        {
            return And(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise AND operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.And"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.And, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.And, "op_BitwiseAnd", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.And, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise AND assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AndAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression AndAssign(Expression left, Expression right)
        {
            return AndAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise AND assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AndAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AndAssign(Expression left, Expression right, MethodInfo method)
        {
            return AndAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise AND assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.AndAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression AndAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.AndAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.AndAssign, "op_BitwiseAnd", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.AndAssign, left, right, method, conversion, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise OR operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Or"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Or(Expression left, Expression right)
        {
            return Or(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents an bitwise OR operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Or"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.Or, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Or, "op_BitwiseOr", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Or, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise OR assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.OrAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression OrAssign(Expression left, Expression right)
        {
            return OrAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise OR assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.OrAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression OrAssign(Expression left, Expression right, MethodInfo method)
        {
            return OrAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise OR assignment operation.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.OrAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression OrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.OrAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.OrAssign, "op_BitwiseOr", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.OrAssign, left, right, method, conversion, true);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise or logical XOR operation, using op_ExclusiveOr for user-defined types.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ExclusiveOr"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression ExclusiveOr(Expression left, Expression right)
        {
            return ExclusiveOr(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise or logical XOR operation, using op_ExclusiveOr for user-defined types.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ExclusiveOr"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
                {
                    return new SimpleBinaryExpression(ExpressionType.ExclusiveOr, left, right, left.Type);
                }
                return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.ExclusiveOr, "op_ExclusiveOr", left, right, true);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.ExclusiveOr, left, right, method, true);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise or logical XOR assignment operation, using op_ExclusiveOr for user-defined types.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ExclusiveOrAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right)
        {
            return ExclusiveOrAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise or logical XOR assignment operation, using op_ExclusiveOr for user-defined types.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ExclusiveOrAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method)
        {
            return ExclusiveOrAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents a bitwise or logical XOR assignment operation, using op_ExclusiveOr for user-defined types.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.ExclusiveOrAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression ExclusiveOrAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                if (left.Type == right.Type && TypeUtils.IsIntegerOrBool(left.Type))
                {
                    // conversion is not supported for binary ops on arithmetic types without operator overloading
                    if (conversion != null)
                    {
                        throw Error.ConversionIsNotSupportedForArithmeticTypes();
                    }
                    return new SimpleBinaryExpression(ExpressionType.ExclusiveOrAssign, left, right, left.Type);
                }
                return GetUserDefinedAssignOperatorOrThrow(ExpressionType.ExclusiveOrAssign, "op_ExclusiveOr", left, right, conversion, true);
            }
            return GetMethodBasedAssignOperator(ExpressionType.ExclusiveOrAssign, left, right, method, conversion, true);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents raising a number to a power.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Power"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression Power(Expression left, Expression right)
        {
            return Power(left, right, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents raising a number to a power.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.Power"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
        {
            RequiresCanRead(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                Type mathType = typeof(System.Math);
                method = mathType.GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    throw Error.BinaryOperatorNotDefined(ExpressionType.Power, left.Type, right.Type);
                }
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Power, left, right, method, true);
        }


        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents raising an expression to a power and assigning the result back to the expression.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.PowerAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/> and <see cref="P:BinaryExpression.Right"/> properties set to the specified values.</returns>
        public static BinaryExpression PowerAssign(Expression left, Expression right)
        {
            return PowerAssign(left, right, null, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents raising an expression to a power and assigning the result back to the expression.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.PowerAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, and <see cref="P:BinaryExpression.Method"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression PowerAssign(Expression left, Expression right, MethodInfo method)
        {
            return PowerAssign(left, right, method, null);
        }

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> that represents raising an expression to a power and assigning the result back to the expression.
        /// </summary>
        /// <param name="left">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Left"/> property equal to.</param>
        /// <param name="right">An <see cref="Expression"/> to set the <see cref="P:BinaryExpression.Right"/> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo"/> to set the <see cref="P:BinaryExpression.Method"/> property equal to.</param>
        /// <param name="conversion">A <see cref="LambdaExpression"/> to set the <see cref="P:BinaryExpression.Conversion"/> property equal to.</param>
        /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="P:Expression.NodeType"/> property equal to <see cref="F:ExpressionType.PowerAssign"/> 
        /// and the <see cref="P:BinaryExpression.Left"/>, <see cref="P:BinaryExpression.Right"/>, <see cref="P:BinaryExpression.Method"/>,
        /// and <see cref="P:BinaryExpression.Conversion"/> properties set to the specified values.
        /// </returns>
        public static BinaryExpression PowerAssign(Expression left, Expression right, MethodInfo method, LambdaExpression conversion)
        {
            RequiresCanRead(left, "left");
            RequiresCanWrite(left, "left");
            RequiresCanRead(right, "right");
            if (method == null)
            {
                Type mathType = typeof(System.Math);
                method = mathType.GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    throw Error.BinaryOperatorNotDefined(ExpressionType.PowerAssign, left.Type, right.Type);
                }
            }
            return GetMethodBasedAssignOperator(ExpressionType.PowerAssign, left, right, method, conversion, true);
        }

        #endregion

        #region ArrayIndex Expression


        /// <summary>
        /// Creates a BinaryExpression that represents applying an array index operator to an array of rank one.
        /// </summary>
        /// <param name="array">An Expression to set the Left property equal to.</param>
        /// <param name="index">An Expression to set the Right property equal to.</param>
        /// <returns>A BinaryExpression that has the NodeType property equal to ArrayIndex and the Left and Right properties set to the specified values.</returns>
        public static BinaryExpression ArrayIndex(Expression array, Expression index)
        {
            RequiresCanRead(array, "array");
            RequiresCanRead(index, "index");
            if (index.Type != typeof(int))
            {
                throw Error.ArgumentMustBeArrayIndexType();
            }

            Type arrayType = array.Type;
            if (!arrayType.IsArray)
            {
                throw Error.ArgumentMustBeArray();
            }
            if (arrayType.GetArrayRank() != 1)
            {
                throw Error.IncorrectNumberOfIndexes();
            }

            return new SimpleBinaryExpression(ExpressionType.ArrayIndex, array, index, arrayType.GetElementType());
        }
        #endregion
    }
}
