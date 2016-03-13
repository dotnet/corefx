// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Represents an expression that applies a delegate or lambda expression to a list of argument expressions.
    /// </summary>
    [DebuggerTypeProxy(typeof(Expression.InvocationExpressionProxy))]
    public class InvocationExpression : Expression, IArgumentProvider
    {
        private readonly Expression _lambda;
        private readonly Type _returnType;

        internal InvocationExpression(Expression lambda, Type returnType)
        {
            _lambda = lambda;
            _returnType = returnType;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type
        {
            get { return _returnType; }
        }

        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> of the expression.</returns>
        public sealed override ExpressionType NodeType
        {
            get { return ExpressionType.Invoke; }
        }

        /// <summary>
        /// Gets the delegate or lambda expression to be applied.
        /// </summary>
        public Expression Expression
        {
            get { return _lambda; }
        }

        /// <summary>
        /// Gets the arguments that the delegate or lambda expression is applied to.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments
        {
            get { return GetOrMakeArguments(); }
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InvocationExpression Update(Expression expression, IEnumerable<Expression> arguments)
        {
            if (expression == Expression && arguments == Arguments)
            {
                return this;
            }

            return Expression.Invoke(expression, arguments);
        }

        [ExcludeFromCodeCoverage] // Unreachable
        internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            throw ContractUtils.Unreachable;
        }

        [ExcludeFromCodeCoverage] // Unreachable
        public virtual Expression GetArgument(int index)
        {
            throw ContractUtils.Unreachable;
        }

        [ExcludeFromCodeCoverage] // Unreachable
        public virtual int ArgumentCount
        {
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitInvocation(this);
        }

        [ExcludeFromCodeCoverage] // Unreachable
        internal virtual InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            throw ContractUtils.Unreachable;
        }

        internal LambdaExpression LambdaOperand
        {
            get
            {
                return (_lambda.NodeType == ExpressionType.Quote)
                    ? (LambdaExpression)((UnaryExpression)_lambda).Operand
                    : (_lambda as LambdaExpression);
            }
        }
    }

    #region Specialized Subclasses

    internal class InvocationExpressionN : InvocationExpression
    {
        private IList<Expression> _arguments;

        public InvocationExpressionN(Expression lambda, IList<Expression> arguments, Type returnType)
            : base(lambda, returnType)
        {
            _arguments = arguments;
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ReturnReadOnly(ref _arguments);
        }

        public override Expression GetArgument(int index)
        {
            return _arguments[index];
        }

        public override int ArgumentCount
        {
            get
            {
                return _arguments.Count;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == _arguments.Count);

            return Expression.Invoke(lambda, arguments ?? _arguments);
        }
    }

    internal class InvocationExpression0 : InvocationExpression
    {
        public InvocationExpression0(Expression lambda, Type returnType)
            : base(lambda, returnType)
        {
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return EmptyReadOnlyCollection<Expression>.Instance;
        }

        public override Expression GetArgument(int index)
        {
            throw new InvalidOperationException();
        }

        public override int ArgumentCount
        {
            get
            {
                return 0;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 0);

            return Expression.Invoke(lambda);
        }
    }

    internal class InvocationExpression1 : InvocationExpression
    {
        private object _arg0;       // storage for the 1st argument or a readonly collection.  See IArgumentProvider

        public InvocationExpression1(Expression lambda, Type returnType, Expression arg0)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ReturnReadOnly(this, ref _arg0);
        }

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ReturnObject<Expression>(_arg0);
                default: throw new InvalidOperationException();
            }
        }

        public override int ArgumentCount
        {
            get
            {
                return 1;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 1);

            if (arguments != null)
            {
                return Expression.Invoke(lambda, arguments[0]);
            }
            return Expression.Invoke(lambda, ReturnObject<Expression>(_arg0));
        }
    }

    internal class InvocationExpression2 : InvocationExpression
    {
        private object _arg0;               // storage for the 1st argument or a readonly collection.  See IArgumentProvider
        private readonly Expression _arg1;  // storage for the 2nd arg

        public InvocationExpression2(Expression lambda, Type returnType, Expression arg0, Expression arg1)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ReturnReadOnly(this, ref _arg0);
        }

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                default: throw new InvalidOperationException();
            }
        }

        public override int ArgumentCount
        {
            get
            {
                return 2;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 2);

            if (arguments != null)
            {
                return Expression.Invoke(lambda, arguments[0], arguments[1]);
            }
            return Expression.Invoke(lambda, ReturnObject<Expression>(_arg0), _arg1);
        }
    }

    internal class InvocationExpression3 : InvocationExpression
    {
        private object _arg0;               // storage for the 1st argument or a readonly collection.  See IArgumentProvider
        private readonly Expression _arg1;  // storage for the 2nd arg
        private readonly Expression _arg2;  // storage for the 3rd arg

        public InvocationExpression3(Expression lambda, Type returnType, Expression arg0, Expression arg1, Expression arg2)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ReturnReadOnly(this, ref _arg0);
        }

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                case 2: return _arg2;
                default: throw new InvalidOperationException();
            }
        }

        public override int ArgumentCount
        {
            get
            {
                return 3;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 3);

            if (arguments != null)
            {
                return Expression.Invoke(lambda, arguments[0], arguments[1], arguments[2]);
            }
            return Expression.Invoke(lambda, ReturnObject<Expression>(_arg0), _arg1, _arg2);
        }
    }

    internal class InvocationExpression4 : InvocationExpression
    {
        private object _arg0;               // storage for the 1st argument or a readonly collection.  See IArgumentProvider
        private readonly Expression _arg1;  // storage for the 2nd arg
        private readonly Expression _arg2;  // storage for the 3rd arg
        private readonly Expression _arg3;  // storage for the 4th arg

        public InvocationExpression4(Expression lambda, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ReturnReadOnly(this, ref _arg0);
        }

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                case 2: return _arg2;
                case 3: return _arg3;
                default: throw new InvalidOperationException();
            }
        }

        public override int ArgumentCount
        {
            get
            {
                return 4;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 4);

            if (arguments != null)
            {
                return Expression.Invoke(lambda, arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            return Expression.Invoke(lambda, ReturnObject<Expression>(_arg0), _arg1, _arg2, _arg3);
        }
    }

    internal class InvocationExpression5 : InvocationExpression
    {
        private object _arg0;               // storage for the 1st argument or a readonly collection.  See IArgumentProvider
        private readonly Expression _arg1;  // storage for the 2nd arg
        private readonly Expression _arg2;  // storage for the 3rd arg
        private readonly Expression _arg3;  // storage for the 4th arg
        private readonly Expression _arg4;  // storage for the 5th arg

        public InvocationExpression5(Expression lambda, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ReturnReadOnly(this, ref _arg0);
        }

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                case 2: return _arg2;
                case 3: return _arg3;
                case 4: return _arg4;
                default: throw new InvalidOperationException();
            }
        }

        public override int ArgumentCount
        {
            get
            {
                return 5;
            }
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 5);

            if (arguments != null)
            {
                return Expression.Invoke(lambda, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            return Expression.Invoke(lambda, ReturnObject<Expression>(_arg0), _arg1, _arg2, _arg3, _arg4);
        }
    }

    #endregion

    public partial class Expression
    {
        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression with no arguments.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression" />.</exception>
        internal static InvocationExpression Invoke(Expression expression)
        {
            // COMPAT: This method is marked as non-public to avoid a gap between a 0-ary and 2-ary overload (see remark for the unary case below).

            RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 0, pis);

            return new InvocationExpression0(expression, method.ReturnType);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to one argument expression.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arg0">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an arugment expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression" />.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0)
        {
            // COMPAT: This method is marked as non-public to ensure compile-time compatibility for Expression.Invoke(e, null).

            RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 1, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0]);

            return new InvocationExpression1(expression, method.ReturnType, arg0);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to two argument expressions.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arg0">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.
        ///</param>
        ///<param name="arg1">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an arugment expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression" />.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.
            RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 2, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0]);
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1]);

            return new InvocationExpression2(expression, method.ReturnType, arg0, arg1);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to three argument expressions.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arg0">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.
        ///</param>
        ///<param name="arg1">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.
        ///</param>
        ///<param name="arg2">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an arugment expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression" />.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.

            RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 3, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0]);
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1]);
            arg2 = ValidateOneArgument(method, ExpressionType.Invoke, arg2, pis[2]);

            return new InvocationExpression3(expression, method.ReturnType, arg0, arg1, arg2);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to four argument expressions.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arg0">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.
        ///</param>
        ///<param name="arg1">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.
        ///</param>
        ///<param name="arg2">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.
        ///</param>
        ///<param name="arg3">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the fourth argument.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an arugment expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression" />.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.

            RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 4, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0]);
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1]);
            arg2 = ValidateOneArgument(method, ExpressionType.Invoke, arg2, pis[2]);
            arg3 = ValidateOneArgument(method, ExpressionType.Invoke, arg3, pis[3]);

            return new InvocationExpression4(expression, method.ReturnType, arg0, arg1, arg2, arg3);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to five argument expressions.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arg0">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the first argument.
        ///</param>
        ///<param name="arg1">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the second argument.
        ///</param>
        ///<param name="arg2">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the third argument.
        ///</param>
        ///<param name="arg3">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the fourth argument.
        ///</param>
        ///<param name="arg4">
        ///The <see cref="T:System.Linq.Expressions.Expression" /> that represents the fifth argument.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an arugment expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression" />.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.

            RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 5, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0]);
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1]);
            arg2 = ValidateOneArgument(method, ExpressionType.Invoke, arg2, pis[2]);
            arg3 = ValidateOneArgument(method, ExpressionType.Invoke, arg3, pis[3]);
            arg4 = ValidateOneArgument(method, ExpressionType.Invoke, arg4, pis[4]);

            return new InvocationExpression5(expression, method.ReturnType, arg0, arg1, arg2, arg3, arg4);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to a list of argument expressions.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arguments">
        ///An array of <see cref="T:System.Linq.Expressions.Expression" /> objects
        ///that represent the arguments that the delegate or lambda expression is applied to.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///<paramref name="arguments" /> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression" />.</exception>
        public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
        {
            return Invoke(expression, (IEnumerable<Expression>)arguments);
        }

        ///<summary>
        ///Creates an <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies a delegate or lambda expression to a list of argument expressions.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Linq.Expressions.InvocationExpression" /> that 
        ///applies the specified delegate or lambda expression to the provided arguments.
        ///</returns>
        ///<param name="expression">
        ///An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate
        ///or lambda expression to be applied.
        ///</param>
        ///<param name="arguments">
        ///An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Linq.Expressions.Expression" /> objects
        ///that represent the arguments that the delegate or lambda expression is applied to.
        ///</param>
        ///<exception cref="T:System.ArgumentNullException">
        ///<paramref name="expression" /> is null.</exception>
        ///<exception cref="T:System.ArgumentException">
        ///<paramref name="expression" />.Type does not represent a delegate type or an <see cref="T:System.Linq.Expressions.Expression`1" />.-or-The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of an element of <paramref name="arguments" /> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression" />.</exception>
        ///<exception cref="T:System.InvalidOperationException">
        ///<paramref name="arguments" /> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression" />.</exception>
        public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
        {
            var argumentList = arguments as IReadOnlyList<Expression> ?? arguments.ToReadOnly();

            switch (argumentList.Count)
            {
                case 0:
                    return Invoke(expression);
                case 1:
                    return Invoke(expression, argumentList[0]);
                case 2:
                    return Invoke(expression, argumentList[0], argumentList[1]);
                case 3:
                    return Invoke(expression, argumentList[0], argumentList[1], argumentList[2]);
                case 4:
                    return Invoke(expression, argumentList[0], argumentList[1], argumentList[2], argumentList[3]);
                case 5:
                    return Invoke(expression, argumentList[0], argumentList[1], argumentList[2], argumentList[3], argumentList[4]);
            }

            RequiresCanRead(expression, nameof(expression));

            var args = argumentList.ToReadOnly(); // Ensure is TrueReadOnlyCollection when count > 5. Returns fast if it already is.
            var mi = GetInvokeMethod(expression);
            ValidateArgumentTypes(mi, ExpressionType.Invoke, ref args);
            return new InvocationExpressionN(expression, args, mi.ReturnType);
        }

        /// <summary>
        /// Gets the delegate's Invoke method; used by InvocationExpression.
        /// </summary>
        /// <param name="expression">The expression to be invoked.</param>
        internal static MethodInfo GetInvokeMethod(Expression expression)
        {
            Type delegateType = expression.Type;
            if (!expression.Type.IsSubclassOf(typeof(MulticastDelegate)))
            {
                Type exprType = TypeUtils.FindGenericType(typeof(Expression<>), expression.Type);
                if (exprType == null)
                {
                    throw Error.ExpressionTypeNotInvocable(expression.Type);
                }
                delegateType = exprType.GetGenericArguments()[0];
            }

            return delegateType.GetMethod("Invoke");
        }
    }
}
