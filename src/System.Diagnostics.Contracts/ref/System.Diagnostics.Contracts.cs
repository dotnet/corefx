// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Diagnostics.Contracts
{
    /// <summary>
    /// Contains static methods for representing program contracts such as preconditions, postconditions,
    /// and object invariants.
    /// </summary>
    public static partial class Contract
    {
        /// <summary>
        /// Occurs when a contract fails.
        /// </summary>
        public static event System.EventHandler<System.Diagnostics.Contracts.ContractFailedEventArgs> ContractFailed { add { } remove { } }
        /// <summary>
        /// Checks for a condition; if the condition is false, follows the escalation policy set for the
        /// analyzer.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition) { }
        /// <summary>
        /// Checks for a condition; if the condition is false, follows the escalation policy set by the
        /// analyzer and displays the specified message.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <param name="userMessage">A message to display if the condition is not met.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition, string userMessage) { }
        /// <summary>
        /// Instructs code analysis tools to assume that the specified condition is true, even if it cannot
        /// be statically proven to always be true.
        /// </summary>
        /// <param name="condition">The conditional expression to assume true.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assume(bool condition) { }
        /// <summary>
        /// Instructs code analysis tools to assume that a condition is true, even if it cannot be statically
        /// proven to always be true, and displays a message if the assumption fails.
        /// </summary>
        /// <param name="condition">The conditional expression to assume true.</param>
        /// <param name="userMessage">The message to post if the assumption fails.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assume(bool condition, string userMessage) { }
        /// <summary>
        /// Marks the end of the contract section when a method's contracts contain only preconditions
        /// in the if-then-throw form.
        /// </summary>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void EndContractBlock() { }
        /// <summary>
        /// Specifies a postcondition contract for the enclosing method or property.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to test. The expression may include
        /// <see cref="OldValue``1(``0)" />, <see cref="ValueAtReturn``1(``0@)" />, and
        /// <see cref="Result``1" /> values.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Ensures(bool condition) { }
        /// <summary>
        /// Specifies a postcondition contract for a provided exit condition and a message to display if
        /// the condition is false.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to test. The expression may include
        /// <see cref="OldValue``1(``0)" /> and <see cref="Result``1" /> values.
        /// </param>
        /// <param name="userMessage">The message to display if the expression is not true.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Ensures(bool condition, string userMessage) { }
        /// <summary>
        /// Specifies a postcondition contract for the enclosing method or property, based on the provided
        /// exception and condition.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <typeparam name="TException">The type of exception that invokes the postcondition check.</typeparam>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void EnsuresOnThrow<TException>(bool condition) where TException : System.Exception { }
        /// <summary>
        /// Specifies a postcondition contract and a message to display if the condition is false for the
        /// enclosing method or property, based on the provided exception and condition.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <param name="userMessage">The message to display if the expression is false.</param>
        /// <typeparam name="TException">The type of exception that invokes the postcondition check.</typeparam>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void EnsuresOnThrow<TException>(bool condition, string userMessage) where TException : System.Exception { }
        /// <summary>
        /// Determines whether a specified test is true for any integer within a range of integers.
        /// </summary>
        /// <param name="fromInclusive">The first integer to pass to <paramref name="predicate" />.</param>
        /// <param name="toExclusive">One more than the last integer to pass to <paramref name="predicate" />.</param>
        /// <param name="predicate">The function to evaluate for any value of the integer in the specified range.</param>
        /// <returns>
        /// true if <paramref name="predicate" /> returns true for any integer starting from
        /// <paramref name="fromInclusive" /> to <paramref name="toExclusive" /> - 1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="toExclusive " />is less than <paramref name="fromInclusive" />.
        /// </exception>
        public static bool Exists(int fromInclusive, int toExclusive, System.Predicate<int> predicate) { return default(bool); }
        /// <summary>
        /// Determines whether an element within a collection of elements exists within a function.
        /// </summary>
        /// <param name="collection">
        /// The collection from which elements of type <paramref name="T" /> will be drawn to pass to
        /// <paramref name="predicate" />.
        /// </param>
        /// <param name="predicate">The function to evaluate for an element in <paramref name="collection" />.</param>
        /// <typeparam name="T">The type that is contained in <paramref name="collection" />.</typeparam>
        /// <returns>
        /// true if and only if <paramref name="predicate" /> returns true for any element of type <paramref name="T" />
        /// in <paramref name="collection" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> or <paramref name="predicate" /> is null.
        /// </exception>
        public static bool Exists<T>(System.Collections.Generic.IEnumerable<T> collection, System.Predicate<T> predicate) { return default(bool); }
        /// <summary>
        /// Determines whether a particular condition is valid for all integers in a specified range.
        /// </summary>
        /// <param name="fromInclusive">The first integer to pass to <paramref name="predicate" />.</param>
        /// <param name="toExclusive">One more than the last integer to pass to <paramref name="predicate" />.</param>
        /// <param name="predicate">The function to evaluate for the existence of the integers in the specified range.</param>
        /// <returns>
        /// true if <paramref name="predicate" /> returns true for all integers starting from
        /// <paramref name="fromInclusive" /> to <paramref name="toExclusive" /> - 1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="toExclusive " />is less than <paramref name="fromInclusive" />.
        /// </exception>
        public static bool ForAll(int fromInclusive, int toExclusive, System.Predicate<int> predicate) { return default(bool); }
        /// <summary>
        /// Determines whether all the elements in a collection exist within a function.
        /// </summary>
        /// <param name="collection">
        /// The collection from which elements of type <paramref name="T" /> will be drawn to pass to
        /// <paramref name="predicate" />.
        /// </param>
        /// <param name="predicate">
        /// The function to evaluate for the existence of all the elements in <paramref name="collection" />.
        /// </param>
        /// <typeparam name="T">The type that is contained in <paramref name="collection" />.</typeparam>
        /// <returns>
        /// true if and only if <paramref name="predicate" /> returns true for all elements of type <paramref name="T" />
        /// in <paramref name="collection" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> or <paramref name="predicate" /> is null.
        /// </exception>
        public static bool ForAll<T>(System.Collections.Generic.IEnumerable<T> collection, System.Predicate<T> predicate) { return default(bool); }
        /// <summary>
        /// Specifies an invariant contract for the enclosing method or property.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Invariant(bool condition) { }
        /// <summary>
        /// Specifies an invariant contract for the enclosing method or property, and displays a message
        /// if the condition for the contract fails.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <param name="userMessage">The message to display if the condition is false.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Invariant(bool condition, string userMessage) { }
        /// <summary>
        /// Represents values as they were at the start of a method or property.
        /// </summary>
        /// <param name="value">The value to represent (field or parameter).</param>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>
        /// The value of the parameter or field at the start of a method or property.
        /// </returns>
        public static T OldValue<T>(T value) { return default(T); }
        /// <summary>
        /// Specifies a precondition contract for the enclosing method or property.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Requires(bool condition) { }
        /// <summary>
        /// Specifies a precondition contract for the enclosing method or property, and displays a message
        /// if the condition for the contract fails.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <param name="userMessage">The message to display if the condition is false.</param>
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Requires(bool condition, string userMessage) { }
        /// <summary>
        /// Specifies a precondition contract for the enclosing method or property, and throws an exception
        /// if the condition for the contract fails.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <typeparam name="TException">The exception to throw if the condition is false.</typeparam>
        public static void Requires<TException>(bool condition) where TException : System.Exception { }
        /// <summary>
        /// Specifies a precondition contract for the enclosing method or property, and throws an exception
        /// with the provided message if the condition for the contract fails.
        /// </summary>
        /// <param name="condition">The conditional expression to test.</param>
        /// <param name="userMessage">The message to display if the condition is false.</param>
        /// <typeparam name="TException">The exception to throw if the condition is false.</typeparam>
        public static void Requires<TException>(bool condition, string userMessage) where TException : System.Exception { }
        /// <summary>
        /// Represents the return value of a method or property.
        /// </summary>
        /// <typeparam name="T">Type of return value of the enclosing method or property.</typeparam>
        /// <returns>
        /// Return value of the enclosing method or property.
        /// </returns>
        public static T Result<T>() { return default(T); }
        /// <summary>
        /// Represents the final (output) value of an out parameter when returning from a method.
        /// </summary>
        /// <param name="value">The out parameter.</param>
        /// <typeparam name="T">The type of the out parameter.</typeparam>
        /// <returns>
        /// The output value of the out parameter.
        /// </returns>
        public static T ValueAtReturn<T>(out T value) { value = default(T); return default(T); }
    }
    /// <summary>
    /// Defines abbreviations that you can use in place of the full contract syntax.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(64), AllowMultiple = false)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractAbbreviatorAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractAbbreviatorAttribute" />
        /// class.
        /// </summary>
        public ContractAbbreviatorAttribute() { }
    }
    /// <summary>
    /// Enables the factoring of legacy if-then-throw code into separate methods for reuse, and provides
    /// full control over thrown exceptions and arguments.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(64), AllowMultiple = false)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractArgumentValidatorAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContractArgumentValidatorAttribute" /> class.
        /// </summary>
        public ContractArgumentValidatorAttribute() { }
    }
    /// <summary>
    /// Specifies that a separate type contains the code contracts for this type.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(5124), AllowMultiple = false, Inherited = false)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    [System.Diagnostics.ConditionalAttribute("DEBUG")]
    public sealed partial class ContractClassAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractClassAttribute" />
        /// class.
        /// </summary>
        /// <param name="typeContainingContracts">The type that contains the code contracts for this type.</param>
        public ContractClassAttribute(System.Type typeContainingContracts) { }
        /// <summary>
        /// Gets the type that contains the code contracts for this type.
        /// </summary>
        /// <returns>
        /// The type that contains the code contracts for this type.
        /// </returns>
        public System.Type TypeContainingContracts { get { return default(System.Type); } }
    }
    /// <summary>
    /// Specifies that a class is a contract for a type.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4), AllowMultiple = false, Inherited = false)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractClassForAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractClassForAttribute" />
        /// class, specifying the type the current class is a contract for.
        /// </summary>
        /// <param name="typeContractsAreFor">The type the current class is a contract for.</param>
        public ContractClassForAttribute(System.Type typeContractsAreFor) { }
        /// <summary>
        /// Gets the type that this code contract applies to.
        /// </summary>
        /// <returns>
        /// The type that this contract applies to.
        /// </returns>
        public System.Type TypeContractsAreFor { get { return default(System.Type); } }
    }
    /// <summary>
    /// Provides methods and data for the <see cref="Contract.ContractFailed" />
    /// event.
    /// </summary>
    public sealed partial class ContractFailedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Provides data for the <see cref="Contract.ContractFailed" />
        /// event.
        /// </summary>
        /// <param name="failureKind">One of the enumeration values that specifies the contract that failed.</param>
        /// <param name="message">The message for the event.</param>
        /// <param name="condition">The condition for the event.</param>
        /// <param name="originalException">The exception that caused the event.</param>
        public ContractFailedEventArgs(System.Diagnostics.Contracts.ContractFailureKind failureKind, string message, string condition, System.Exception originalException) { }
        /// <summary>
        /// Gets the condition for the failure of the contract.
        /// </summary>
        /// <returns>
        /// The condition for the failure.
        /// </returns>
        public string Condition { get { return default(string); } }
        /// <summary>
        /// Gets the type of contract that failed.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the type of contract that failed.
        /// </returns>
        public System.Diagnostics.Contracts.ContractFailureKind FailureKind { get { return default(System.Diagnostics.Contracts.ContractFailureKind); } }
        /// <summary>
        /// Indicates whether the <see cref="Contract.ContractFailed" />
        /// event has been handled.
        /// </summary>
        /// <returns>
        /// true if the event has been handled; otherwise, false.
        /// </returns>
        public bool Handled { get { return default(bool); } }
        /// <summary>
        /// Gets the message that describes the <see cref="Contract.ContractFailed" />
        /// event.
        /// </summary>
        /// <returns>
        /// The message that describes the event.
        /// </returns>
        public string Message { get { return default(string); } }
        /// <summary>
        /// Gets the original exception that caused the
        /// <see cref="Contract.ContractFailed" /> event.
        /// </summary>
        /// <returns>
        /// The exception that caused the event.
        /// </returns>
        public System.Exception OriginalException { get { return default(System.Exception); } }
        /// <summary>
        /// Indicates whether the code contract escalation policy should be applied.
        /// </summary>
        /// <returns>
        /// true to apply the escalation policy; otherwise, false. The default is false.
        /// </returns>
        public bool Unwind { get { return default(bool); } }
        /// <summary>
        /// Sets the <see cref="Handled" /> property
        /// to true.
        /// </summary>
        [System.Security.SecurityCriticalAttribute]
        public void SetHandled() { }
        /// <summary>
        /// Sets the <see cref="Unwind" /> property
        /// to true.
        /// </summary>
        [System.Security.SecurityCriticalAttribute]
        public void SetUnwind() { }
    }
    /// <summary>
    /// Specifies the type of contract that failed.
    /// </summary>
    public enum ContractFailureKind
    {
        /// <summary>
        /// An <see cref="Overload:System.Diagnostics.Contracts.Contract.Assert" /> contract failed.
        /// </summary>
        Assert = 4,
        /// <summary>
        /// An <see cref="Overload:System.Diagnostics.Contracts.Contract.Assume" /> contract failed.
        /// </summary>
        Assume = 5,
        /// <summary>
        /// An <see cref="Overload:System.Diagnostics.Contracts.Contract.Invariant" /> contract failed.
        /// </summary>
        Invariant = 3,
        /// <summary>
        /// An <see cref="Overload:System.Diagnostics.Contracts.Contract.Ensures" /> contract failed.
        /// </summary>
        Postcondition = 1,
        /// <summary>
        /// An <see cref="Overload:System.Diagnostics.Contracts.Contract.EnsuresOnThrow" /> contract failed.
        /// </summary>
        PostconditionOnException = 2,
        /// <summary>
        /// A <see cref="Overload:System.Diagnostics.Contracts.Contract.Requires" /> contract failed.
        /// </summary>
        Precondition = 0,
    }
    /// <summary>
    /// Marks a method as being the invariant method for a class.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(64), AllowMultiple = false, Inherited = false)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractInvariantMethodAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContractInvariantMethodAttribute" /> class.
        /// </summary>
        public ContractInvariantMethodAttribute() { }
    }
    /// <summary>
    /// Enables you to set contract and tool options at assembly, type, or method granularity.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(32767), AllowMultiple = true, Inherited = false)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractOptionAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractOptionAttribute" />
        /// class by using the provided category, setting, and enable/disable value.
        /// </summary>
        /// <param name="category">The category for the option to be set.</param>
        /// <param name="setting">The option setting.</param>
        /// <param name="enabled">true to enable the option; false to disable the option.</param>
        public ContractOptionAttribute(string category, string setting, bool enabled) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractOptionAttribute" />
        /// class by using the provided category, setting, and value.
        /// </summary>
        /// <param name="category">The category of the option to be set.</param>
        /// <param name="setting">The option setting.</param>
        /// <param name="value">The value for the setting.</param>
        public ContractOptionAttribute(string category, string setting, string value) { }
        /// <summary>
        /// Gets the category of the option.
        /// </summary>
        /// <returns>
        /// The category of the option.
        /// </returns>
        public string Category { get { return default(string); } }
        /// <summary>
        /// Determines if an option is enabled.
        /// </summary>
        /// <returns>
        /// true if the option is enabled; otherwise, false.
        /// </returns>
        public bool Enabled { get { return default(bool); } }
        /// <summary>
        /// Gets the setting for the option.
        /// </summary>
        /// <returns>
        /// The setting for the option.
        /// </returns>
        public string Setting { get { return default(string); } }
        /// <summary>
        /// Gets the value for the option.
        /// </summary>
        /// <returns>
        /// The value for the option.
        /// </returns>
        public string Value { get { return default(string); } }
    }
    /// <summary>
    /// Specifies that a field can be used in method contracts when the field has less visibility than
    /// the method.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(256))]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractPublicPropertyNameAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContractPublicPropertyNameAttribute" /> class.
        /// </summary>
        /// <param name="name">The property name to apply to the field.</param>
        public ContractPublicPropertyNameAttribute(string name) { }
        /// <summary>
        /// Gets the property name to be applied to the field.
        /// </summary>
        /// <returns>
        /// The property name to be applied to the field.
        /// </returns>
        public string Name { get { return default(string); } }
    }
    /// <summary>
    /// Specifies that an assembly is a reference assembly that contains contracts.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(1))]
    public sealed partial class ContractReferenceAssemblyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ContractReferenceAssemblyAttribute" /> class.
        /// </summary>
        public ContractReferenceAssemblyAttribute() { }
    }
    /// <summary>
    /// Identifies a member that has no run-time behavior.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(192), AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractRuntimeIgnoredAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractRuntimeIgnoredAttribute" />
        /// class.
        /// </summary>
        public ContractRuntimeIgnoredAttribute() { }
    }
    /// <summary>
    /// Instructs analysis tools to assume the correctness of an assembly, type, or member without
    /// performing static verification.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(237))]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class ContractVerificationAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractVerificationAttribute" />
        /// class.
        /// </summary>
        /// <param name="value">true to require verification; otherwise, false.</param>
        public ContractVerificationAttribute(bool value) { }
        /// <summary>
        /// Gets the value that indicates whether to verify the contract of the target.
        /// </summary>
        /// <returns>
        /// true if verification is required; otherwise, false.
        /// </returns>
        public bool Value { get { return default(bool); } }
    }
    /// <summary>
    /// Indicates that a type or method is pure, that is, it does not make any visible state changes.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(6884), AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
    public sealed partial class PureAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PureAttribute" />
        /// class.
        /// </summary>
        public PureAttribute() { }
    }
}
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Provides methods that the binary rewriter uses to handle contract failures.
    /// </summary>
    public static partial class ContractHelper
    {
        /// <summary>
        /// Used by the binary rewriter to activate the default failure behavior.
        /// </summary>
        /// <param name="failureKind">One of the enumeration values that specifies the type of failure.</param>
        /// <param name="userMessage">Additional user information.</param>
        /// <param name="conditionText">The description of the condition that caused the failure.</param>
        /// <param name="innerException">The inner exception that caused the current exception.</param>
        /// <returns>
        /// A null reference (Nothing in Visual Basic) if the event was handled and should not trigger
        /// a failure; otherwise, returns the localized failure message.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="failureKind" /> is not a valid
        /// <see cref="Diagnostics.Contracts.ContractFailureKind" /> value.
        /// </exception>
        public static string RaiseContractFailedEvent(System.Diagnostics.Contracts.ContractFailureKind failureKind, string userMessage, string conditionText, System.Exception innerException) { return default(string); }
        /// <summary>
        /// Triggers the default failure behavior.
        /// </summary>
        /// <param name="kind">One of the enumeration values that specifies the type of failure.</param>
        /// <param name="displayMessage">The message to display.</param>
        /// <param name="userMessage">Additional user information.</param>
        /// <param name="conditionText">The description of the condition that caused the failure.</param>
        /// <param name="innerException">The inner exception that caused the current exception.</param>
        public static void TriggerFailure(System.Diagnostics.Contracts.ContractFailureKind kind, string displayMessage, string userMessage, string conditionText, System.Exception innerException) { }
    }
}
