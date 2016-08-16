// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Diagnostics
{
    /// <summary>
    /// Provides a set of methods and properties that help debug your code. This class cannot be inherited.
    /// </summary>
    public static partial class Debug
    {
        /// <summary>
        /// Checks for a condition; if the condition is false, displays a message box that shows the call
        /// stack.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, a failure message is not
        /// sent and the message box is not displayed.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition) { }
        /// <summary>
        /// Checks for a condition; if the condition is false, outputs a specified message and displays
        /// a message box that shows the call stack.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the specified message is
        /// not sent and the message box is not displayed.
        /// </param>
        /// <param name="message">
        /// The message to send to the <see cref="Diagnostics.Trace.Listeners" /> collection.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition, string message) { }
        /// <summary>
        /// Checks for a condition; if the condition is false, outputs two specified messages and displays
        /// a message box that shows the call stack.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the specified messages are
        /// not sent and the message box is not displayed.
        /// </param>
        /// <param name="message">
        /// The message to send to the <see cref="Diagnostics.Trace.Listeners" /> collection.
        /// </param>
        /// <param name="detailMessage">
        /// The detailed message to send to the <see cref="Diagnostics.Trace.Listeners" /> collection.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessage) { }
        /// <summary>
        /// Checks for a condition; if the condition is false, outputs two messages (simple and formatted)
        /// and displays a message box that shows the call stack.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the specified messages are
        /// not sent and the message box is not displayed.
        /// </param>
        /// <param name="message">
        /// The message to send to the <see cref="Diagnostics.Trace.Listeners" /> collection.
        /// </param>
        /// <param name="detailMessageFormat">
        /// The composite format string (see Remarks) to send to the <see cref="Diagnostics.Trace.Listeners" />
        /// collection. This message contains text intermixed with zero or more format items, which
        /// correspond to objects in the <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args) { }
        /// <summary>
        /// Emits the specified error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Fail(string message) { }
        /// <summary>
        /// Emits an error message and a detailed error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Fail(string message, string detailMessage) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Debug.Listeners" /> collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Write(object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Write(object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Write(string message) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Debug.Listeners" /> collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Write(string message, string category) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Debug.Listeners" /> collection if a condition
        /// is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the value is written to the
        /// trace listeners in the collection.
        /// </param>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteIf(bool condition, object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the category name and value
        /// are written to the trace listeners in the collection.
        /// </param>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteIf(bool condition, object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the message is written to
        /// the trace listeners in the collection.
        /// </param>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteIf(bool condition, string message) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Debug.Listeners" /> collection if a condition is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the category name and message
        /// are written to the trace listeners in the collection.
        /// </param>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteIf(bool condition, string message, string category) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Debug.Listeners" /> collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLine(object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection.
        /// </summary>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLine(object value, string category) { }
        /// <summary>
        /// Writes a message followed by a line terminator to the trace listeners in the
        /// <see cref="Debug.Listeners" /> collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLine(string message) { }
        /// <summary>
        /// Writes a formatted message followed by a line terminator to the trace listeners in the
        /// <see cref="Debug.Listeners" /> collection.
        /// </summary>
        /// <param name="format">
        /// A composite format string (see Remarks) that contains text intermixed with zero or more format
        /// items, which correspond to objects in the <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLine(string format, params object[] args) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Debug.Listeners" /> collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLine(string message, string category) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Debug.Listeners" /> collection if a condition
        /// is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the value is written to the
        /// trace listeners in the collection.
        /// </param>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLineIf(bool condition, object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the category name and value
        /// are written to the trace listeners in the collection.
        /// </param>
        /// <param name="value">An object whose name is sent to the <see cref="Debug.Listeners" />.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLineIf(bool condition, object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Debug.Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, the message is written to
        /// the trace listeners in the collection.
        /// </param>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLineIf(bool condition, string message) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Debug.Listeners" /> collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void WriteLineIf(bool condition, string message, string category) { }
    }
    /// <summary>
    /// Enables communication with a debugger. This class cannot be inherited.
    /// </summary>
    public static partial class Debugger
    {
        /// <summary>
        /// Gets a value that indicates whether a debugger is attached to the process.
        /// </summary>
        /// <returns>
        /// true if a debugger is attached; otherwise, false.
        /// </returns>
        public static bool IsAttached { get { return default(bool); } }
        /// <summary>
        /// Signals a breakpoint to an attached debugger.
        /// </summary>
        /// <exception cref="Security.SecurityException">
        /// The <see cref="Security.Permissions.UIPermission" /> is not set to break into the
        /// debugger.
        /// </exception>
        public static void Break() { }
        /// <summary>
        /// Launches and attaches a debugger to the process.
        /// </summary>
        /// <returns>
        /// true if the startup is successful or if the debugger is already attached; otherwise, false.
        /// </returns>
        /// <exception cref="Security.SecurityException">
        /// The <see cref="Security.Permissions.UIPermission" /> is not set to start the debugger.
        /// </exception>
        public static bool Launch() { return default(bool); }
    }
    /// <summary>
    /// Determines if and how a member is displayed in the debugger variable windows. This class cannot
    /// be inherited.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public sealed partial class DebuggerBrowsableAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerBrowsableAttribute" />
        /// class.
        /// </summary>
        /// <param name="state">
        /// One of the <see cref="DebuggerBrowsableState" /> values that specifies
        /// how to display the member.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="state" /> is not one of the <see cref="DebuggerBrowsableState" />
        /// values.
        /// </exception>
        public DebuggerBrowsableAttribute(System.Diagnostics.DebuggerBrowsableState state) { }
        /// <summary>
        /// Gets the display state for the attribute.
        /// </summary>
        /// <returns>
        /// One of the <see cref="DebuggerBrowsableState" /> values.
        /// </returns>
        public System.Diagnostics.DebuggerBrowsableState State { get { return default(System.Diagnostics.DebuggerBrowsableState); } }
    }
    /// <summary>
    /// Provides display instructions for the debugger.
    /// </summary>
    public enum DebuggerBrowsableState
    {
        /// <summary>
        /// Show the element as collapsed.
        /// </summary>
        Collapsed = 2,
        /// <summary>
        /// Never show the element.
        /// </summary>
        Never = 0,
        /// <summary>
        /// Do not display the root element; display the child elements if the element is a collection
        /// or array of items.
        /// </summary>
        RootHidden = 3,
    }
    /// <summary>
    /// Determines how a class or field is displayed in the debugger variable windows.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4509), AllowMultiple = true)]
    public sealed partial class DebuggerDisplayAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerDisplayAttribute" />
        /// class.
        /// </summary>
        /// <param name="value">
        /// The string to be displayed in the value column for instances of the type; an empty string ("")
        /// causes the value column to be hidden.
        /// </param>
        public DebuggerDisplayAttribute(string value) { }
        /// <summary>
        /// Gets or sets the name to display in the debugger variable windows.
        /// </summary>
        /// <returns>
        /// The name to display in the debugger variable windows.
        /// </returns>
        public string Name { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the type of the attribute's target.
        /// </summary>
        /// <returns>
        /// The attribute's target type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="Target" /> is set to null.
        /// </exception>
        public System.Type Target { get { return default(System.Type); } set { } }
        /// <summary>
        /// Gets or sets the type name of the attribute's target.
        /// </summary>
        /// <returns>
        /// The name of the attribute's target type.
        /// </returns>
        public string TargetTypeName { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the string to display in the type column of the debugger variable windows.
        /// </summary>
        /// <returns>
        /// The string to display in the type column of the debugger variable windows.
        /// </returns>
        public string Type { get { return default(string); } set { } }
        /// <summary>
        /// Gets the string to display in the value column of the debugger variable windows.
        /// </summary>
        /// <returns>
        /// The string to display in the value column of the debugger variable.
        /// </returns>
        public string Value { get { return default(string); } }
    }
    /// <summary>
    /// Specifies the <see cref="DebuggerHiddenAttribute" />. This class cannot
    /// be inherited.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(224), Inherited = false)]
    public sealed partial class DebuggerHiddenAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerHiddenAttribute" />
        /// class.
        /// </summary>
        public DebuggerHiddenAttribute() { }
    }
    /// <summary>
    /// Identifies a type or member that is not part of the user code for an application.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(236), Inherited = false)]
    public sealed partial class DebuggerNonUserCodeAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerNonUserCodeAttribute" />
        /// class.
        /// </summary>
        public DebuggerNonUserCodeAttribute() { }
    }
    /// <summary>
    /// Instructs the debugger to step through the code instead of stepping into the code. This class
    /// cannot be inherited.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(108), Inherited = false)]
    public sealed partial class DebuggerStepThroughAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerStepThroughAttribute" />
        /// class.
        /// </summary>
        public DebuggerStepThroughAttribute() { }
    }
    /// <summary>
    /// Specifies the display proxy for a type.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(13), AllowMultiple = true)]
    public sealed partial class DebuggerTypeProxyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerTypeProxyAttribute" />
        /// class using the type name of the proxy.
        /// </summary>
        /// <param name="typeName">The type name of the proxy type.</param>
        public DebuggerTypeProxyAttribute(string typeName) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerTypeProxyAttribute" />
        /// class using the type of the proxy.
        /// </summary>
        /// <param name="type">The proxy type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> is null.</exception>
        public DebuggerTypeProxyAttribute(System.Type type) { }
        /// <summary>
        /// Gets the type name of the proxy type.
        /// </summary>
        /// <returns>
        /// The type name of the proxy type.
        /// </returns>
        public string ProxyTypeName { get { return default(string); } }
        /// <summary>
        /// Gets or sets the target type for the attribute.
        /// </summary>
        /// <returns>
        /// The target type for the attribute.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="Target" /> is set to null.
        /// </exception>
        public System.Type Target { get { return default(System.Type); } set { } }
        /// <summary>
        /// Gets or sets the name of the target type.
        /// </summary>
        /// <returns>
        /// The name of the target type.
        /// </returns>
        public string TargetTypeName { get { return default(string); } set { } }
    }
}
