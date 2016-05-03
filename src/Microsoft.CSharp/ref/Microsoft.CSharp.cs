// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Contains factory methods to create dynamic call site binders for CSharp.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public static partial class Binder
    {
        /// <summary>
        /// Initializes a new CSharp binary operation binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="operation">The binary operation kind.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp binary operation binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder BinaryOperation(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Linq.Expressions.ExpressionType operation, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp convert binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="type">The type to convert to.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <returns>
        /// Returns a new CSharp convert binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder Convert(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Type type, System.Type context) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp get index binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp get index binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder GetIndex(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp get member binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp get member binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder GetMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, string name, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp invoke binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp invoke binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder Invoke(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp invoke constructor binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp invoke constructor binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder InvokeConstructor(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp invoke member binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="name">The name of the member to invoke.</param>
        /// <param name="typeArguments">The list of type arguments specified for this invoke.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp invoke member binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder InvokeMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, string name, System.Collections.Generic.IEnumerable<System.Type> typeArguments, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp is event binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="name">The name of the event to look for.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <returns>
        /// Returns a new CSharp is event binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder IsEvent(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, string name, System.Type context) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp set index binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp set index binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder SetIndex(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp set member binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="name">The name of the member to set.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp set member binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder SetMember(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, string name, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
        /// <summary>
        /// Initializes a new CSharp unary operation binder.
        /// </summary>
        /// <param name="flags">The flags with which to initialize the binder.</param>
        /// <param name="operation">The unary operation kind.</param>
        /// <param name="context">The <see cref="System.Type" /> that indicates where this operation is used.</param>
        /// <param name="argumentInfo">
        /// The sequence of <see cref="CSharpArgumentInfo" /> instances
        /// for the arguments to this operation.
        /// </param>
        /// <returns>
        /// Returns a new CSharp unary operation binder.
        /// </returns>
        public static System.Runtime.CompilerServices.CallSiteBinder UnaryOperation(Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags flags, System.Linq.Expressions.ExpressionType operation, System.Type context, System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo> argumentInfo) { return default(System.Runtime.CompilerServices.CallSiteBinder); }
    }
    /// <summary>
    /// Represents information about C# dynamic operations that are specific to particular arguments
    /// at a call site. Instances of this class are generated by the C# compiler.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    public sealed partial class CSharpArgumentInfo
    {
        internal CSharpArgumentInfo() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpArgumentInfo" />
        /// class.
        /// </summary>
        /// <param name="flags">The flags for the argument.</param>
        /// <param name="name">The name of the argument, if named; otherwise null.</param>
        /// <returns>
        /// A new instance of the <see cref="CSharpArgumentInfo" /> class.
        /// </returns>
        public static Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags flags, string name) { return default(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo); }
    }
    /// <summary>
    /// Represents information about C# dynamic operations that are specific to particular arguments
    /// at a call site. Instances of this class are generated by the C# compiler.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    [System.FlagsAttribute]
    public enum CSharpArgumentInfoFlags
    {
        /// <summary>
        /// The argument is a constant.
        /// </summary>
        Constant = 2,
        /// <summary>
        /// The argument is passed to an out parameter.
        /// </summary>
        IsOut = 16,
        /// <summary>
        /// The argument is passed to a ref parameter.
        /// </summary>
        IsRef = 8,
        /// <summary>
        /// The argument is a <see cref="System.Type" /> indicating an actual type name used in source.
        /// Used only for target objects in static calls.
        /// </summary>
        IsStaticType = 32,
        /// <summary>
        /// The argument is a named argument.
        /// </summary>
        NamedArgument = 4,
        /// <summary>
        /// No additional information to represent.
        /// </summary>
        None = 0,
        /// <summary>
        /// The argument's compile-time type should be considered during binding.
        /// </summary>
        UseCompileTimeType = 1,
    }
    /// <summary>
    /// Represents information about C# dynamic operations that are not specific to particular arguments
    /// at a call site. Instances of this class are generated by the C# compiler.
    /// </summary>
    [System.ComponentModel.EditorBrowsableAttribute((System.ComponentModel.EditorBrowsableState)(1))]
    [System.FlagsAttribute]
    public enum CSharpBinderFlags
    {
        /// <summary>
        /// The binder represents a logical AND or logical OR that is part of a conditional logical operator
        /// evaluation.
        /// </summary>
        BinaryOperationLogical = 8,
        /// <summary>
        /// The evaluation of this binder happens in a checked context.
        /// </summary>
        CheckedContext = 1,
        /// <summary>
        /// The binder represents an implicit conversion for use in an array creation expression.
        /// </summary>
        ConvertArrayIndex = 32,
        /// <summary>
        /// The binder represents an explicit conversion.
        /// </summary>
        ConvertExplicit = 16,
        /// <summary>
        /// The binder represents an invoke on a simple name.
        /// </summary>
        InvokeSimpleName = 2,
        /// <summary>
        /// The binder represents an invoke on a specialname.
        /// </summary>
        InvokeSpecialName = 4,
        /// <summary>
        /// There is no additional information required for this binder.
        /// </summary>
        None = 0,
        /// <summary>
        /// The binder is used in a position that does not require a result, and can therefore bind to
        /// a void returning method.
        /// </summary>
        ResultDiscarded = 256,
        /// <summary>
        /// The result of any bind is going to be indexed get a set index or get index binder.
        /// </summary>
        ResultIndexed = 64,
        /// <summary>
        /// The value in this set index or set member comes a compound assignment operator.
        /// </summary>
        ValueFromCompoundAssignment = 128,
    }
    /// <summary>
    /// Represents an error that occurs when a dynamic bind in the C# runtime binder is processed.
    /// </summary>
    public partial class RuntimeBinderException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBinderException" />
        /// class.
        /// </summary>
        public RuntimeBinderException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBinderException" />
        /// class that has a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the exception. The caller of this constructor is required to ensure
        /// that this string has been localized for the current system culture.
        /// </param>
        public RuntimeBinderException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeBinderException" />
        /// class that has a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception
        /// is specified.
        /// </param>
        public RuntimeBinderException(string message, System.Exception innerException) { }
    }
    /// <summary>
    /// Represents an error that occurs when a dynamic bind in the C# runtime binder is processed.
    /// </summary>
    public partial class RuntimeBinderInternalCompilerException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RuntimeBinderInternalCompilerException" /> class with a system-supplied message that describes the error.
        /// </summary>
        public RuntimeBinderInternalCompilerException() { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RuntimeBinderInternalCompilerException" /> class with a specified message that describes the error.
        /// </summary>
        /// <param name="message">
        /// The message that describes the exception. The caller of this constructor is required to ensure
        /// that this string has been localized for the current system culture.
        /// </param>
        public RuntimeBinderInternalCompilerException(string message) { }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RuntimeBinderInternalCompilerException" /> class that has a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference if no inner exception
        /// is specified.
        /// </param>
        public RuntimeBinderInternalCompilerException(string message, System.Exception innerException) { }
    }
}
