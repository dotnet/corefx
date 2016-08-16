// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Diagnostics
{
    /// <summary>
    /// Provides a simple on/off switch that controls debugging and tracing output.
    /// </summary>
    public partial class BooleanSwitch : System.Diagnostics.Switch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanSwitch" /> class
        /// with the specified display name and description.
        /// </summary>
        /// <param name="displayName">The name to display on a user interface.</param>
        /// <param name="description">The description of the switch.</param>
        public BooleanSwitch(string displayName, string description) : base(default(string), default(string)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanSwitch" /> class
        /// with the specified display name, description, and default switch value.
        /// </summary>
        /// <param name="displayName">The name to display on the user interface.</param>
        /// <param name="description">The description of the switch.</param>
        /// <param name="defaultSwitchValue">The default value of the switch.</param>
        public BooleanSwitch(string displayName, string description, string defaultSwitchValue) : base(default(string), default(string)) { }
        /// <summary>
        /// Gets or sets a value indicating whether the switch is enabled or disabled.
        /// </summary>
        /// <returns>
        /// true if the switch is enabled; otherwise, false. The default is false.
        /// </returns>
        /// <exception cref="Security.SecurityException">The caller does not have the correct permission.</exception>
        public bool Enabled { get { return default(bool); } set { } }
        /// <summary>
        /// Determines whether the new value of the <see cref="Switch.Value" /> property
        /// can be parsed as a Boolean value.
        /// </summary>
        protected override void OnValueChanged() { }
    }
    /// <summary>
    /// Provides the default output methods and behavior for tracing.
    /// </summary>
    public partial class DefaultTraceListener : System.Diagnostics.TraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTraceListener" />
        /// class with "Default" as its <see cref="TraceListener.Name" /> property
        /// value.
        /// </summary>
        public DefaultTraceListener() { }
        /// <summary>
        /// Emits or displays a message and a stack trace for an assertion that always fails.
        /// </summary>
        /// <param name="message">The message to emit or display.</param>
        public override void Fail(string message) { }
        /// <summary>
        /// Emits or displays detailed messages and a stack trace for an assertion that always fails.
        /// </summary>
        /// <param name="message">The message to emit or display.</param>
        /// <param name="detailMessage">The detailed message to emit or display.</param>
        public override void Fail(string message, string detailMessage) { }
        /// <summary>
        /// Writes the output to the OutputDebugString function and to the
        /// <see cref="Debugger.Log(Int32,String,String)" /> method.
        /// </summary>
        /// <param name="message">
        /// The message to write to OutputDebugString and
        /// <see cref="Debugger.Log(Int32,String,String)" />.
        /// </param>
        public override void Write(string message) { }
        /// <summary>
        /// Writes the output to the OutputDebugString function and to the
        /// <see cref="Debugger.Log(Int32,String,String)" /> method, followed by a carriage return and line feed (\r\n).
        /// </summary>
        /// <param name="message">
        /// The message to write to OutputDebugString and
        /// <see cref="Debugger.Log(Int32,String,String)" />.
        /// </param>
        public override void WriteLine(string message) { }
    }
    /// <summary>
    /// Indicates whether a listener should trace based on the event type.
    /// </summary>
    public partial class EventTypeFilter : System.Diagnostics.TraceFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypeFilter" /> class.
        /// </summary>
        /// <param name="level">
        /// A bitwise combination of the <see cref="SourceLevels" /> values that
        /// specifies the event type of the messages to trace.
        /// </param>
        public EventTypeFilter(System.Diagnostics.SourceLevels level) { }
        /// <summary>
        /// Gets or sets the event type of the messages to trace.
        /// </summary>
        /// <returns>
        /// A bitwise combination of the <see cref="SourceLevels" /> values.
        /// </returns>
        public System.Diagnostics.SourceLevels EventType { get { return default(System.Diagnostics.SourceLevels); } set { } }
        /// <summary>
        /// Determines whether the trace listener should trace the event.
        /// </summary>
        /// <param name="cache">
        /// A <see cref="TraceEventCache" /> that represents the information cache
        /// for the trace event.
        /// </param>
        /// <param name="source">The name of the source.</param>
        /// <param name="eventType">One of the <see cref="TraceEventType" /> values.</param>
        /// <param name="id">A trace identifier number.</param>
        /// <param name="formatOrMessage">The format to use for writing an array of arguments, or a message to write.</param>
        /// <param name="args">An array of argument objects.</param>
        /// <param name="data1">A trace data object.</param>
        /// <param name="data">An array of trace data objects.</param>
        /// <returns>
        /// trueif the trace should be produced; otherwise, false.
        /// </returns>
        public override bool ShouldTrace(System.Diagnostics.TraceEventCache cache, string source, System.Diagnostics.TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data) { return default(bool); }
    }
    /// <summary>
    /// Indicates whether a listener should trace a message based on the source of a trace.
    /// </summary>
    public partial class SourceFilter : System.Diagnostics.TraceFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceFilter" /> class,
        /// specifying the name of the trace source.
        /// </summary>
        /// <param name="source">The name of the trace source.</param>
        public SourceFilter(string source) { }
        /// <summary>
        /// Gets or sets the name of the trace source.
        /// </summary>
        /// <returns>
        /// The name of the trace source.
        /// </returns>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        public string Source { get { return default(string); } set { } }
        /// <summary>
        /// Determines whether the trace listener should trace the event.
        /// </summary>
        /// <param name="cache">An object that represents the information cache for the trace event.</param>
        /// <param name="source">The name of the source.</param>
        /// <param name="eventType">One of the enumeration values that identifies the event type.</param>
        /// <param name="id">A trace identifier number.</param>
        /// <param name="formatOrMessage">The format to use for writing an array of arguments or a message to write.</param>
        /// <param name="args">An array of argument objects.</param>
        /// <param name="data1">A trace data object.</param>
        /// <param name="data">An array of trace data objects.</param>
        /// <returns>
        /// true if the trace should be produced; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> is null.</exception>
        public override bool ShouldTrace(System.Diagnostics.TraceEventCache cache, string source, System.Diagnostics.TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data) { return default(bool); }
    }
    /// <summary>
    /// Specifies the levels of trace messages filtered by the source switch and event type filter.
    /// </summary>
    [System.FlagsAttribute]
    public enum SourceLevels
    {
        /// <summary>
        /// Allows all events through.
        /// </summary>
        All = -1,
        /// <summary>
        /// Allows only <see cref="TraceEventType.Critical" /> events through.
        /// </summary>
        Critical = 1,
        /// <summary>
        /// Allows <see cref="TraceEventType.Critical" /> and
        /// <see cref="TraceEventType.Error" /> events through.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Allows <see cref="TraceEventType.Critical" />,
        /// <see cref="TraceEventType.Error" />, <see cref="TraceEventType.Warning" />, and
        /// <see cref="TraceEventType.Information" /> events through.
        /// </summary>
        Information = 15,
        /// <summary>
        /// Does not allow any events through.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Allows <see cref="TraceEventType.Critical" />,
        /// <see cref="TraceEventType.Error" />, <see cref="TraceEventType.Warning" />,
        /// <see cref="TraceEventType.Information" />, and <see cref="TraceEventType.Verbose" /> events through.
        /// </summary>
        Verbose = 31,
        /// <summary>
        /// Allows <see cref="TraceEventType.Critical" />,
        /// <see cref="TraceEventType.Error" />, and <see cref="TraceEventType.Warning" /> events through.
        /// </summary>
        Warning = 7,
    }
    /// <summary>
    /// Provides a multilevel switch to control tracing and debug output without recompiling your code.
    /// </summary>
    public partial class SourceSwitch : System.Diagnostics.Switch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceSwitch" /> class,
        /// specifying the name of the source.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        public SourceSwitch(string name) : base(default(string), default(string)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceSwitch" /> class,
        /// specifying the display name and the default value for the source switch.
        /// </summary>
        /// <param name="displayName">The name of the source switch.</param>
        /// <param name="defaultSwitchValue">The default value for the switch.</param>
        public SourceSwitch(string displayName, string defaultSwitchValue) : base(default(string), default(string)) { }
        /// <summary>
        /// Gets or sets the level of the switch.
        /// </summary>
        /// <returns>
        /// One of the <see cref="SourceLevels" /> values that represents the event
        /// level of the switch.
        /// </returns>
        public System.Diagnostics.SourceLevels Level { get { return default(System.Diagnostics.SourceLevels); } set { } }
        /// <summary>
        /// Invoked when the value of the <see cref="Switch.Value" /> property changes.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The new value of <see cref="Switch.Value" /> is not one of the
        /// <see cref="SourceLevels" /> values.
        /// </exception>
        protected override void OnValueChanged() { }
        /// <summary>
        /// Determines if trace listeners should be called, based on the trace event type.
        /// </summary>
        /// <param name="eventType">One of the <see cref="TraceEventType" /> values.</param>
        /// <returns>
        /// True if the trace listeners should be called; otherwise, false.
        /// </returns>
        public bool ShouldTrace(System.Diagnostics.TraceEventType eventType) { return default(bool); }
    }
    /// <summary>
    /// Provides an abstract base class to create new debugging and tracing switches.
    /// </summary>
    public abstract partial class Switch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Switch" /> class.
        /// </summary>
        /// <param name="displayName">The name of the switch.</param>
        /// <param name="description">The description for the switch.</param>
        protected Switch(string displayName, string description) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Switch" /> class, specifying
        /// the display name, description, and default value for the switch.
        /// </summary>
        /// <param name="displayName">The name of the switch.</param>
        /// <param name="description">The description of the switch.</param>
        /// <param name="defaultSwitchValue">The default value for the switch.</param>
        protected Switch(string displayName, string description, string defaultSwitchValue) { }
        /// <summary>
        /// Gets a description of the switch.
        /// </summary>
        /// <returns>
        /// The description of the switch. The default value is an empty string ("").
        /// </returns>
        public string Description { get { return default(string); } }
        /// <summary>
        /// Gets a name used to identify the switch.
        /// </summary>
        /// <returns>
        /// The name used to identify the switch. The default value is an empty string ("").
        /// </returns>
        public string DisplayName { get { return default(string); } }
        /// <summary>
        /// Gets or sets the current setting for this switch.
        /// </summary>
        /// <returns>
        /// The current setting for this switch. The default is zero.
        /// </returns>
        protected int SwitchSetting { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the value of the switch.
        /// </summary>
        /// <returns>
        /// A string representing the value of the switch.
        /// </returns>
        /// <exception cref="Configuration.ConfigurationErrorsException">
        /// The value is null.-or-The value does not consist solely of an optional negative sign followed
        /// by a sequence of digits ranging from 0 to 9.-or-The value represents a number less than
        /// <see cref="Int32.MinValue" /> or greater than <see cref="Int32.MaxValue" />.
        /// </exception>
        protected string Value { get { return default(string); } set { } }
        /// <summary>
        /// Invoked when the <see cref="SwitchSetting" /> property is changed.
        /// </summary>
        protected virtual void OnSwitchSettingChanged() { }
        /// <summary>
        /// Invoked when the <see cref="Value" /> property is changed.
        /// </summary>
        protected virtual void OnValueChanged() { }
    }
    /// <summary>
    /// Provides a set of methods and properties that help you trace the execution of your code. This
    /// class cannot be inherited.
    /// </summary>
    public sealed partial class Trace
    {
        internal Trace() { }
        /// <summary>
        /// Gets or sets whether <see cref="Flush" /> should be called on the
        /// <see cref="Listeners" /> after every write.
        /// </summary>
        /// <returns>
        /// true if <see cref="Flush" /> is called on the
        /// <see cref="Listeners" /> after every write; otherwise, false.
        /// </returns>
        public static bool AutoFlush { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the indent level.
        /// </summary>
        /// <returns>
        /// The indent level. The default is zero.
        /// </returns>
        public static int IndentLevel { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the number of spaces in an indent.
        /// </summary>
        /// <returns>
        /// The number of spaces in an indent. The default is four.
        /// </returns>
        public static int IndentSize { get { return default(int); } set { } }
        /// <summary>
        /// Gets the collection of listeners that is monitoring the trace output.
        /// </summary>
        /// <returns>
        /// A <see cref="TraceListenerCollection" /> that represents a collection
        /// of type <see cref="TraceListener" /> monitoring the trace output.
        /// </returns>
        public static System.Diagnostics.TraceListenerCollection Listeners { get { return default(System.Diagnostics.TraceListenerCollection); } }
        /// <summary>
        /// Gets or sets a value indicating whether the global lock should be used.
        /// </summary>
        /// <returns>
        /// true if the global lock is to be used; otherwise, false. The default is true.
        /// </returns>
        public static bool UseGlobalLock { get { return default(bool); } set { } }
        /// <summary>
        /// Checks for a condition; if the condition is false, displays a message box that shows the call
        /// stack.
        /// </summary>
        /// <param name="condition">
        /// The conditional expression to evaluate. If the condition is true, a failure message is not
        /// sent and the message box is not displayed.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
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
        /// The message to send to the <see cref="Listeners" /> collection.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
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
        /// The message to send to the <see cref="Listeners" /> collection.
        /// </param>
        /// <param name="detailMessage">
        /// The detailed message to send to the <see cref="Listeners" /> collection.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Assert(bool condition, string message, string detailMessage) { }
        /// <summary>
        /// Flushes the output buffer, and then closes the <see cref="Listeners" />.
        /// </summary>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Close() { }
        /// <summary>
        /// Emits the specified error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Fail(string message) { }
        /// <summary>
        /// Emits an error message, and a detailed error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Fail(string message, string detailMessage) { }
        /// <summary>
        /// Flushes the output buffer, and causes buffered data to be written to the
        /// <see cref="Listeners" />.
        /// </summary>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Flush() { }
        /// <summary>
        /// Increases the current <see cref="IndentLevel" /> by one.
        /// </summary>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Indent() { }
        /// <summary>
        /// Refreshes the trace configuration data.
        /// </summary>
        public static void Refresh() { }
        /// <summary>
        /// Writes an error message to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void TraceError(string message) { }
        /// <summary>
        /// Writes an error message to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond to objects in the
        /// <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void TraceError(string format, params object[] args) { }
        /// <summary>
        /// Writes an informational message to the trace listeners in the
        /// <see cref="Listeners" /> collection using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void TraceInformation(string message) { }
        /// <summary>
        /// Writes an informational message to the trace listeners in the
        /// <see cref="Listeners" /> collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond to objects in the
        /// <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void TraceInformation(string format, params object[] args) { }
        /// <summary>
        /// Writes a warning message to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void TraceWarning(string message) { }
        /// <summary>
        /// Writes a warning message to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified array of objects and formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond to objects in the
        /// <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void TraceWarning(string format, params object[] args) { }
        /// <summary>
        /// Decreases the current <see cref="IndentLevel" /> by one.
        /// </summary>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Unindent() { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Listeners" /> collection.
        /// </summary>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Write(object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Listeners" />
        /// collection.
        /// </summary>
        /// <param name="value">
        /// An <see cref="Object" /> name is sent to the <see cref="Listeners" />.
        /// </param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Write(object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Listeners" />
        /// collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Write(string message) { }
        /// <summary>
        /// Writes a category name and a message to the trace listeners in the
        /// <see cref="Listeners" /> collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void Write(string message, string category) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Listeners" /> collection if a condition
        /// is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteIf(bool condition, object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteIf(bool condition, object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteIf(bool condition, string message) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Listeners" /> collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteIf(bool condition, string message, string category) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Listeners" /> collection.
        /// </summary>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLine(object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Listeners" />
        /// collection.
        /// </summary>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLine(object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Listeners" />
        /// collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLine(string message) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Listeners" /> collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLine(string message, string category) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the trace
        /// listeners in the <see cref="Listeners" /> collection if a condition
        /// is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLineIf(bool condition, object value) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the trace listeners in the <see cref="Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="value">
        /// An <see cref="Object" /> whose name is sent to the <see cref="Listeners" />.
        /// </param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLineIf(bool condition, object value, string category) { }
        /// <summary>
        /// Writes a message to the trace listeners in the <see cref="Listeners" />
        /// collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="message">A message to write.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLineIf(bool condition, string message) { }
        /// <summary>
        /// Writes a category name and message to the trace listeners in the
        /// <see cref="Listeners" /> collection if a condition is true.
        /// </summary>
        /// <param name="condition">true to cause a message to be written; otherwise, false.</param>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public static void WriteLineIf(bool condition, string message, string category) { }
    }
    /// <summary>
    /// Provides trace event data specific to a thread and a process.
    /// </summary>
    public partial class TraceEventCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceEventCache" /> class.
        /// </summary>
        public TraceEventCache() { }
        /// <summary>
        /// Gets the date and time at which the event trace occurred.
        /// </summary>
        /// <returns>
        /// A <see cref="System.DateTime" /> structure whose value is a date and time expressed in Coordinated
        /// Universal Time (UTC).
        /// </returns>
        public System.DateTime DateTime { get { return default(System.DateTime); } }
        /// <summary>
        /// Gets the unique identifier of the current process.
        /// </summary>
        /// <returns>
        /// The system-generated unique identifier of the current process.
        /// </returns>
        public int ProcessId { get { return default(int); } }
        /// <summary>
        /// Gets a unique identifier for the current managed thread.
        /// </summary>
        /// <returns>
        /// A string that represents a unique integer identifier for this managed thread.
        /// </returns>
        public string ThreadId { get { return default(string); } }
        /// <summary>
        /// Gets the current number of ticks in the timer mechanism.
        /// </summary>
        /// <returns>
        /// The tick counter value of the underlying timer mechanism.
        /// </returns>
        public long Timestamp { get { return default(long); } }
    }
    /// <summary>
    /// Identifies the type of event that has caused the trace.
    /// </summary>
    public enum TraceEventType
    {
        /// <summary>
        /// Fatal error or application crash.
        /// </summary>
        Critical = 1,
        /// <summary>
        /// Recoverable error.
        /// </summary>
        Error = 2,
        /// <summary>
        /// Informational message.
        /// </summary>
        Information = 8,
        /// <summary>
        /// Debugging trace.
        /// </summary>
        Verbose = 16,
        /// <summary>
        /// Noncritical problem.
        /// </summary>
        Warning = 4,
    }
    /// <summary>
    /// Provides the base class for trace filter implementations.
    /// </summary>
    public abstract partial class TraceFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceFilter" /> class.
        /// </summary>
        protected TraceFilter() { }
        /// <summary>
        /// When overridden in a derived class, determines whether the trace listener should trace the
        /// event.
        /// </summary>
        /// <param name="cache">
        /// The <see cref="TraceEventCache" /> that contains information for the
        /// trace event.
        /// </param>
        /// <param name="source">The name of the source.</param>
        /// <param name="eventType">
        /// One of the <see cref="TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A trace identifier number.</param>
        /// <param name="formatOrMessage">
        /// Either the format to use for writing an array of arguments specified by the <paramref name="args" />
        /// parameter, or a message to write.
        /// </param>
        /// <param name="args">An array of argument objects.</param>
        /// <param name="data1">A trace data object.</param>
        /// <param name="data">An array of trace data objects.</param>
        /// <returns>
        /// true to trace the specified event; otherwise, false.
        /// </returns>
        public abstract bool ShouldTrace(System.Diagnostics.TraceEventCache cache, string source, System.Diagnostics.TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data);
    }
    /// <summary>
    /// Specifies what messages to output for the <see cref="Diagnostics.Debug" />,
    /// <see cref="Trace" /> and <see cref="TraceSwitch" /> classes.
    /// </summary>
    public enum TraceLevel
    {
        /// <summary>
        /// Output error-handling messages.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Output informational messages, warnings, and error-handling messages.
        /// </summary>
        Info = 3,
        /// <summary>
        /// Output no tracing and debugging messages.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Output all debugging and tracing messages.
        /// </summary>
        Verbose = 4,
        /// <summary>
        /// Output warnings and error-handling messages.
        /// </summary>
        Warning = 2,
    }
    /// <summary>
    /// Provides the abstract base class for the listeners who monitor trace and debug output.
    /// </summary>
    public abstract partial class TraceListener : System.IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListener" /> class.
        /// </summary>
        protected TraceListener() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListener" /> class
        /// using the specified name as the listener.
        /// </summary>
        /// <param name="name">The name of the <see cref="TraceListener" />.</param>
        protected TraceListener(string name) { }
        /// <summary>
        /// Gets and sets the trace filter for the trace listener.
        /// </summary>
        /// <returns>
        /// An object derived from the <see cref="TraceFilter" /> base class.
        /// </returns>
        public System.Diagnostics.TraceFilter Filter { get { return default(System.Diagnostics.TraceFilter); } set { } }
        /// <summary>
        /// Gets or sets the indent level.
        /// </summary>
        /// <returns>
        /// The indent level. The default is zero.
        /// </returns>
        public int IndentLevel { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the number of spaces in an indent.
        /// </summary>
        /// <returns>
        /// The number of spaces in an indent. The default is four spaces.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Set operation failed because the value is less than zero.
        /// </exception>
        public int IndentSize { get { return default(int); } set { } }
        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <returns>
        /// true if the trace listener is thread safe; otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsThreadSafe { get { return default(bool); } }
        /// <summary>
        /// Gets or sets a name for this <see cref="TraceListener" />.
        /// </summary>
        /// <returns>
        /// A name for this <see cref="TraceListener" />. The default is an empty
        /// string ("").
        /// </returns>
        public virtual string Name { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value indicating whether to indent the output.
        /// </summary>
        /// <returns>
        /// true if the output should be indented; otherwise, false.
        /// </returns>
        protected bool NeedIndent { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the trace output options.
        /// </summary>
        /// <returns>
        /// A bitwise combination of the enumeration values. The default is
        /// <see cref="TraceOptions.None" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Set operation failed because the value is invalid.</exception>
        public System.Diagnostics.TraceOptions TraceOutputOptions { get { return default(System.Diagnostics.TraceOptions); } set { } }
        /// <summary>
        /// Releases all resources used by the <see cref="TraceListener" />.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TraceListener" />
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) { }
        /// <summary>
        /// Emits an error message to the listener you create when you implement the
        /// <see cref="TraceListener" /> class.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        public virtual void Fail(string message) { }
        /// <summary>
        /// Emits an error message and a detailed error message to the listener you create when you implement
        /// the <see cref="TraceListener" /> class.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public virtual void Fail(string message, string detailMessage) { }
        /// <summary>
        /// When overridden in a derived class, flushes the output buffer.
        /// </summary>
        public virtual void Flush() { }
        /// <summary>
        /// Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        public virtual void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data) { }
        /// <summary>
        /// Writes trace information, an array of data objects and event information to the listener specific
        /// output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">An array of objects to emit as data.</param>
        public virtual void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, params object[] data) { }
        /// <summary>
        /// Writes trace and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        public virtual void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id) { }
        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        public virtual void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, string message) { }
        /// <summary>
        /// Writes trace information, a formatted array of objects and event information to the listener
        /// specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="TraceEventCache" /> object that contains the current process
        /// ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the application that generated the
        /// trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="TraceEventType" /> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A format string that contains zero or more format items, which correspond to objects in the
        /// <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public virtual void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, string format, params object[] args) { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the listener
        /// you create when you implement the <see cref="TraceListener" /> class.
        /// </summary>
        /// <param name="o">An <see cref="Object" /> whose fully qualified class name you want to write.</param>
        public virtual void Write(object o) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the listener you create when you implement the <see cref="TraceListener" />
        /// class.
        /// </summary>
        /// <param name="o">An <see cref="Object" /> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public virtual void Write(object o, string category) { }
        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create
        /// in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public abstract void Write(string message);
        /// <summary>
        /// Writes a category name and a message to the listener you create when you implement the
        /// <see cref="TraceListener" /> class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public virtual void Write(string message, string category) { }
        /// <summary>
        /// Writes the indent to the listener you create when you implement this class, and resets the
        /// <see cref="NeedIndent" /> property to false.
        /// </summary>
        protected virtual void WriteIndent() { }
        /// <summary>
        /// Writes the value of the object's <see cref="Object.ToString" /> method to the listener
        /// you create when you implement the <see cref="TraceListener" /> class,
        /// followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="Object" /> whose fully qualified class name you want to write.</param>
        public virtual void WriteLine(object o) { }
        /// <summary>
        /// Writes a category name and the value of the object's <see cref="Object.ToString" />
        /// method to the listener you create when you implement the <see cref="TraceListener" />
        /// class, followed by a line terminator.
        /// </summary>
        /// <param name="o">An <see cref="Object" /> whose fully qualified class name you want to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public virtual void WriteLine(object o, string category) { }
        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived
        /// class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public abstract void WriteLine(string message);
        /// <summary>
        /// Writes a category name and a message to the listener you create when you implement the
        /// <see cref="TraceListener" /> class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        /// <param name="category">A category name used to organize the output.</param>
        public virtual void WriteLine(string message, string category) { }
    }
    /// <summary>
    /// Provides a thread-safe list of <see cref="TraceListener" /> objects.
    /// </summary>
    public partial class TraceListenerCollection : System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        internal TraceListenerCollection() { }
        /// <summary>
        /// Gets the number of listeners in the list.
        /// </summary>
        /// <returns>
        /// The number of listeners in the list.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets or sets the <see cref="TraceListener" /> at the specified index.
        /// </summary>
        /// <param name="i">
        /// The zero-based index of the <see cref="TraceListener" /> to get from
        /// the list.
        /// </param>
        /// <returns>
        /// A <see cref="TraceListener" /> with the specified index.
        /// </returns>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        public System.Diagnostics.TraceListener this[int i] { get { return default(System.Diagnostics.TraceListener); } set { } }
        /// <summary>
        /// Gets the first <see cref="TraceListener" /> in the list with the specified
        /// name.
        /// </summary>
        /// <param name="name">The name of the <see cref="TraceListener" /> to get from the list.</param>
        /// <returns>
        /// The first <see cref="TraceListener" /> in the list with the given
        /// <see cref="TraceListener.Name" />. This item returns null if no
        /// <see cref="TraceListener" /> with the given name can be found.
        /// </returns>
        public System.Diagnostics.TraceListener this[string name] { get { return default(System.Diagnostics.TraceListener); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        /// <summary>
        /// Adds a <see cref="TraceListener" /> to the list.
        /// </summary>
        /// <param name="listener">A <see cref="TraceListener" /> to add to the list.</param>
        /// <returns>
        /// The position at which the new listener was inserted.
        /// </returns>
        public int Add(System.Diagnostics.TraceListener listener) { return default(int); }
        /// <summary>
        /// Adds an array of <see cref="TraceListener" /> objects to the list.
        /// </summary>
        /// <param name="value">
        /// An array of <see cref="TraceListener" /> objects to add to the list.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        public void AddRange(System.Diagnostics.TraceListener[] value) { }
        /// <summary>
        /// Adds the contents of another <see cref="TraceListenerCollection" /> to
        /// the list.
        /// </summary>
        /// <param name="value">
        /// Another <see cref="TraceListenerCollection" /> whose contents are added
        /// to the list.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        public void AddRange(System.Diagnostics.TraceListenerCollection value) { }
        /// <summary>
        /// Clears all the listeners from the list.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Checks whether the list contains the specified listener.
        /// </summary>
        /// <param name="listener">A <see cref="TraceListener" /> to find in the list.</param>
        /// <returns>
        /// true if the listener is in the list; otherwise, false.
        /// </returns>
        public bool Contains(System.Diagnostics.TraceListener listener) { return default(bool); }
        /// <summary>
        /// Copies a section of the current <see cref="TraceListenerCollection" />
        /// list to the specified array at the specified index.
        /// </summary>
        /// <param name="listeners">An array of type <see cref="Array" /> to copy the elements into.</param>
        /// <param name="index">The starting index number in the current list to copy from.</param>
        public void CopyTo(System.Diagnostics.TraceListener[] listeners, int index) { }
        /// <summary>
        /// Gets an enumerator for this list.
        /// </summary>
        /// <returns>
        /// An enumerator of type <see cref="Collections.IEnumerator" />.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Gets the index of the specified listener.
        /// </summary>
        /// <param name="listener">A <see cref="TraceListener" /> to find in the list.</param>
        /// <returns>
        /// The index of the listener, if it can be found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(System.Diagnostics.TraceListener listener) { return default(int); }
        /// <summary>
        /// Inserts the listener at the specified index.
        /// </summary>
        /// <param name="index">
        /// The position in the list to insert the new <see cref="TraceListener" />.
        /// </param>
        /// <param name="listener">A <see cref="TraceListener" /> to insert in the list.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index" /> is not a valid index in the list.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="listener" /> is null.</exception>
        public void Insert(int index, System.Diagnostics.TraceListener listener) { }
        /// <summary>
        /// Removes from the collection the specified <see cref="TraceListener" />.
        /// </summary>
        /// <param name="listener">A <see cref="TraceListener" /> to remove from the list.</param>
        public void Remove(System.Diagnostics.TraceListener listener) { }
        /// <summary>
        /// Removes from the collection the first <see cref="TraceListener" /> with
        /// the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="TraceListener" /> to remove from the list.
        /// </param>
        public void Remove(string name) { }
        /// <summary>
        /// Removes from the collection the <see cref="TraceListener" /> at the specified
        /// index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="TraceListener" /> to remove from
        /// the list.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index" /> is not a valid index in the list.
        /// </exception>
        public void RemoveAt(int index) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        int System.Collections.IList.Add(object value) { return default(int); }
        bool System.Collections.IList.Contains(object value) { return default(bool); }
        int System.Collections.IList.IndexOf(object value) { return default(int); }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
    }
    /// <summary>
    /// Specifies trace data options to be written to the trace output.
    /// </summary>
    [System.FlagsAttribute]
    public enum TraceOptions
    {
        /// <summary>
        /// Write the date and time.
        /// </summary>
        DateTime = 2,
        /// <summary>
        /// Do not write any elements.
        /// </summary>
        None = 0,
        /// <summary>
        /// Write the process identity, which is represented by the return value of the
        /// <see cref="Diagnostics.Process.Id" /> property.
        /// </summary>
        ProcessId = 8,
        /// <summary>
        /// Write the thread identity, which is represented by the return value of the
        /// <see cref="Threading.Thread.ManagedThreadId" /> property for the current thread.
        /// </summary>
        ThreadId = 16,
        /// <summary>
        /// Write the timestamp, which is represented by the return value of the
        /// <see cref="Diagnostics.Stopwatch.GetTimestamp" /> method.
        /// </summary>
        Timestamp = 4,
    }
    /// <summary>
    /// Provides a set of methods and properties that enable applications to trace the execution of
    /// code and associate trace messages with their source.
    /// </summary>
    public partial class TraceSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSource" /> class, using
        /// the specified name for the source.
        /// </summary>
        /// <param name="name">The name of the source (typically, the name of the application).</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="name" /> is an empty string ("").</exception>
        public TraceSource(string name) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSource" /> class, using
        /// the specified name for the source and the default source level at which tracing is to occur.
        /// </summary>
        /// <param name="name">The name of the source, typically the name of the application.</param>
        /// <param name="defaultLevel">
        /// A bitwise combination of the enumeration values that specifies the default source level at
        /// which to trace.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="name" /> is an empty string ("").</exception>
        public TraceSource(string name, System.Diagnostics.SourceLevels defaultLevel) { }
        /// <summary>
        /// Gets the collection of trace listeners for the trace source.
        /// </summary>
        /// <returns>
        /// A <see cref="TraceListenerCollection" /> that contains the active trace
        /// listeners associated with the source.
        /// </returns>
        public System.Diagnostics.TraceListenerCollection Listeners { get { return default(System.Diagnostics.TraceListenerCollection); } }
        /// <summary>
        /// Gets the name of the trace source.
        /// </summary>
        /// <returns>
        /// The name of the trace source.
        /// </returns>
        public string Name { get { return default(string); } }
        /// <summary>
        /// Gets or sets the source switch value.
        /// </summary>
        /// <returns>
        /// A <see cref="SourceSwitch" /> object representing the source switch value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <see cref="Switch" /> is set to null.
        /// </exception>
        public System.Diagnostics.SourceSwitch Switch { get { return default(System.Diagnostics.SourceSwitch); } set { } }
        /// <summary>
        /// Closes all the trace listeners in the trace listener collection.
        /// </summary>
        public void Close() { }
        /// <summary>
        /// Flushes all the trace listeners in the trace listener collection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        public void Flush() { }
        /// <summary>
        /// Writes trace data to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified event type, event identifier, and trace data.
        /// </summary>
        /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data.</param>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceData(System.Diagnostics.TraceEventType eventType, int id, object data) { }
        /// <summary>
        /// Writes trace data to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified event type, event identifier, and trace data array.
        /// </summary>
        /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">An object array containing the trace data.</param>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceData(System.Diagnostics.TraceEventType eventType, int id, params object[] data) { }
        /// <summary>
        /// Writes a trace event message to the trace listeners in the
        /// <see cref="Listeners" /> collection using the specified event type and event identifier.
        /// </summary>
        /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceEvent(System.Diagnostics.TraceEventType eventType, int id) { }
        /// <summary>
        /// Writes a trace event message to the trace listeners in the
        /// <see cref="Listeners" /> collection using the specified event type, event identifier, and message.
        /// </summary>
        /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">The trace message to write.</param>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceEvent(System.Diagnostics.TraceEventType eventType, int id, string message) { }
        /// <summary>
        /// Writes a trace event to the trace listeners in the <see cref="Listeners" />
        /// collection using the specified event type, event identifier, and argument array and format.
        /// </summary>
        /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A composite format string (see Remarks) that contains text intermixed with zero or more format
        /// items, which correspond to objects in the <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format" /> is invalid.-or- The number that indicates an argument to format
        /// is less than zero, or greater than or equal to the number of specified objects to format.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceEvent(System.Diagnostics.TraceEventType eventType, int id, string format, params object[] args) { }
        /// <summary>
        /// Writes an informational message to the trace listeners in the
        /// <see cref="Listeners" /> collection using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceInformation(string message) { }
        /// <summary>
        /// Writes an informational message to the trace listeners in the
        /// <see cref="Listeners" /> collection using the specified object array and formatting information.
        /// </summary>
        /// <param name="format">
        /// A composite format string (see Remarks) that contains text intermixed with zero or more format
        /// items, which correspond to objects in the <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An array containing zero or more objects to format.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format" /> is null.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format" /> is invalid.-or- The number that indicates an argument to format
        /// is less than zero, or greater than or equal to the number of specified objects to format.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// An attempt was made to trace an event during finalization.
        /// </exception>
        [System.Diagnostics.ConditionalAttribute("TRACE")]
        public void TraceInformation(string format, params object[] args) { }
    }
    /// <summary>
    /// Provides a multilevel switch to control tracing and debug output without recompiling your code.
    /// </summary>
    public partial class TraceSwitch : System.Diagnostics.Switch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSwitch" /> class, using
        /// the specified display name and description.
        /// </summary>
        /// <param name="displayName">The name to display on a user interface.</param>
        /// <param name="description">The description of the switch.</param>
        public TraceSwitch(string displayName, string description) : base(default(string), default(string)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSwitch" /> class, using
        /// the specified display name, description, and default value for the switch.
        /// </summary>
        /// <param name="displayName">The name to display on a user interface.</param>
        /// <param name="description">The description of the switch.</param>
        /// <param name="defaultSwitchValue">The default value of the switch.</param>
        public TraceSwitch(string displayName, string description, string defaultSwitchValue) : base(default(string), default(string)) { }
        /// <summary>
        /// Gets or sets the trace level that determines the messages the switch allows.
        /// </summary>
        /// <returns>
        /// One of the <see cref="TraceLevel" /> values that that specifies the level
        /// of messages that are allowed by the switch.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Level" /> is set to a value that is not one of
        /// the <see cref="TraceLevel" /> values.
        /// </exception>
        public System.Diagnostics.TraceLevel Level { get { return default(System.Diagnostics.TraceLevel); } set { } }
        /// <summary>
        /// Gets a value indicating whether the switch allows error-handling messages.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Level" /> property is set to
        /// <see cref="TraceLevel.Error" />, <see cref="TraceLevel.Warning" />,
        /// <see cref="TraceLevel.Info" />, or <see cref="TraceLevel.Verbose" />;
        /// otherwise, false.
        /// </returns>
        public bool TraceError { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the switch allows informational messages.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Level" /> property is set to
        /// <see cref="TraceLevel.Info" /> or <see cref="TraceLevel.Verbose" />;
        /// otherwise, false.
        /// </returns>
        public bool TraceInfo { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the switch allows all messages.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Level" /> property is set to
        /// <see cref="TraceLevel.Verbose" />; otherwise, false.
        /// </returns>
        public bool TraceVerbose { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the switch allows warning messages.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Level" /> property is set to
        /// <see cref="TraceLevel.Warning" />, <see cref="TraceLevel.Info" />,
        /// or <see cref="TraceLevel.Verbose" />; otherwise, false.
        /// </returns>
        public bool TraceWarning { get { return default(bool); } }
        /// <summary>
        /// Updates and corrects the level for this switch.
        /// </summary>
        protected override void OnSwitchSettingChanged() { }
        /// <summary>
        /// Sets the <see cref="Switch.SwitchSetting" /> property to the integer
        /// equivalent of the <see cref="Switch.Value" /> property.
        /// </summary>
        protected override void OnValueChanged() { }
    }
}
