// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Diagnostics.Tracing
{
    /// <summary>
    /// Specifies the tracking of activity start and stop events.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventActivityOptions
    {
        /// <summary>
        /// Allow overlapping activities. By default, activity starts and stops must be property nested.
        /// That is, a sequence of Start A, Start B, Stop A, Stop B is not allowed will result in B stopping at
        /// the same time as A.
        /// </summary>
        Detachable = 8,
        /// <summary>
        /// Turn off start and stop tracking.
        /// </summary>
        Disable = 2,
        /// <summary>
        /// Use the default behavior for start and stop tracking.
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow recursive activity starts. By default, an activity cannot be recursive. That is, a sequence
        /// of Start A, Start A, Stop A, Stop A is not allowed. Unintentional recursive activities can occur
        /// if the app executes and for some the stop is not reached before another start is called.
        /// </summary>
        Recursive = 4,
    }
    /// <summary>
    /// Specifies additional event schema information for an event.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
    public sealed partial class EventAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttribute" />
        /// class with the specified event identifier.
        /// </summary>
        /// <param name="eventId">The event identifier for the event.</param>
        public EventAttribute(int eventId) { }
        /// <summary>
        /// Specifies the behavior of the start and stop events of an activity. An activity is the region
        /// of time in an app between the start and the stop.
        /// </summary>
        /// <returns>
        /// Returns <see cref="EventActivityOptions" />.
        /// </returns>
        public System.Diagnostics.Tracing.EventActivityOptions ActivityOptions { get { return default(System.Diagnostics.Tracing.EventActivityOptions); } set { } }
        /// <summary>
        /// Gets or sets an additional event log where the event should be written.
        /// </summary>
        /// <returns>
        /// An additional event log where the event should be written.
        /// </returns>
        public System.Diagnostics.Tracing.EventChannel Channel { get { return default(System.Diagnostics.Tracing.EventChannel); } set { } }
        /// <summary>
        /// Gets or sets the identifier for the event.
        /// </summary>
        /// <returns>
        /// The event identifier. This value should be between 0 and 65535.
        /// </returns>
        public int EventId { get { return default(int); } }
        /// <summary>
        /// Gets or sets the keywords for the event.
        /// </summary>
        /// <returns>
        /// A bitwise combination of the enumeration values.
        /// </returns>
        public System.Diagnostics.Tracing.EventKeywords Keywords { get { return default(System.Diagnostics.Tracing.EventKeywords); } set { } }
        /// <summary>
        /// Gets or sets the level for the event.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the level for the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventLevel Level { get { return default(System.Diagnostics.Tracing.EventLevel); } set { } }
        /// <summary>
        /// Gets or sets the message for the event.
        /// </summary>
        /// <returns>
        /// The message for the event.
        /// </returns>
        public string Message { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the operation code for the event.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that specifies the operation code.
        /// </returns>
        public System.Diagnostics.Tracing.EventOpcode Opcode { get { return default(System.Diagnostics.Tracing.EventOpcode); } set { } }
        /// <summary>
        /// Gets and sets the <see cref="EventTags" /> value for this
        /// <see cref="EventAttribute" /> object. An event tag is a user-defined
        /// value that is passed through when the event is logged.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="EventTags" /> value.
        /// </returns>
        public System.Diagnostics.Tracing.EventTags Tags { get { return default(System.Diagnostics.Tracing.EventTags); } set { } }
        /// <summary>
        /// Gets or sets the task for the event.
        /// </summary>
        /// <returns>
        /// The task for the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventTask Task { get { return default(System.Diagnostics.Tracing.EventTask); } set { } }
        /// <summary>
        /// Gets or sets the version of the event.
        /// </summary>
        /// <returns>
        /// The version of the event.
        /// </returns>
        public byte Version { get { return default(byte); } set { } }
    }
    /// <summary>
    /// Specifies the event log channel for the event.
    /// </summary>
    public enum EventChannel : byte
    {
        /// <summary>
        /// The administrator log channel.
        /// </summary>
        Admin = (byte)16,
        /// <summary>
        /// The analytic channel.
        /// </summary>
        Analytic = (byte)18,
        /// <summary>
        /// The debug channel.
        /// </summary>
        Debug = (byte)19,
        /// <summary>
        /// No channel specified.
        /// </summary>
        None = (byte)0,
        /// <summary>
        /// The operational channel.
        /// </summary>
        Operational = (byte)17,
    }
    /// <summary>
    /// Describes the command (<see cref="EventCommandEventArgs.Command" />
    /// property) that is passed to the
    /// <see cref="EventSource.OnEventCommand(EventCommandEventArgs)" /> callback.
    /// </summary>
    public enum EventCommand
    {
        /// <summary>
        /// Disable the event.
        /// </summary>
        Disable = -3,
        /// <summary>
        /// Enable the event.
        /// </summary>
        Enable = -2,
        /// <summary>
        /// Send the manifest.
        /// </summary>
        SendManifest = -1,
        /// <summary>
        /// Update the event.
        /// </summary>
        Update = 0,
    }
    /// <summary>
    /// Provides the arguments for the
    /// <see cref="EventSource.OnEventCommand(EventCommandEventArgs)" /> callback.
    /// </summary>
    public partial class EventCommandEventArgs : System.EventArgs
    {
        internal EventCommandEventArgs() { }
        /// <summary>
        /// Gets the array of arguments for the callback.
        /// </summary>
        /// <returns>
        /// An array of callback arguments.
        /// </returns>
        public System.Collections.Generic.IDictionary<string, string> Arguments { get { return default(System.Collections.Generic.IDictionary<string, string>); } }
        /// <summary>
        /// Gets the command for the callback.
        /// </summary>
        /// <returns>
        /// The callback command.
        /// </returns>
        public System.Diagnostics.Tracing.EventCommand Command { get { return default(System.Diagnostics.Tracing.EventCommand); } }
        /// <summary>
        /// Disables the event that have the specified identifier.
        /// </summary>
        /// <param name="eventId">The identifier of the event to disable.</param>
        /// <returns>
        /// true if <paramref name="eventId" /> is in range; otherwise, false.
        /// </returns>
        public bool DisableEvent(int eventId) { return default(bool); }
        /// <summary>
        /// Enables the event that has the specified identifier.
        /// </summary>
        /// <param name="eventId">The identifier of the event to enable.</param>
        /// <returns>
        /// true if <paramref name="eventId" /> is in range; otherwise, false.
        /// </returns>
        public bool EnableEvent(int eventId) { return default(bool); }
    }
    public class EventCounter
    {
        public EventCounter(string name, EventSource eventSource) { }
        public void WriteMetric(float value) { }
    }
    /// <summary>
    /// Specifies a type to be passed to the
    /// <see cref="EventSource.Write{T}(string, EventSourceOptions, T)"/> method.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(12), Inherited = false)]
    public partial class EventDataAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDataAttribute" />
        /// class.
        /// </summary>
        public EventDataAttribute() { }
        /// <summary>
        /// Gets or set the name to apply to an event if the event type or property is not explicitly named.
        /// </summary>
        /// <returns>
        /// The name to apply to the event or property.
        /// </returns>
        public string Name { get { return default(string); } set { } }
    }
    /// <summary>
    /// The <see cref="EventFieldAttribute" /> is placed on fields of
    /// user-defined types that are passed as <see cref="EventSource" />
    /// payloads.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(128))]
    public partial class EventFieldAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFieldAttribute" />
        /// class.
        /// </summary>
        public EventFieldAttribute() { }
        /// <summary>
        /// Gets and sets the value that specifies how to format the value of a user-defined type.
        /// </summary>
        /// <returns>
        /// Returns a<see cref="EventFieldFormat" /> value.
        /// </returns>
        public System.Diagnostics.Tracing.EventFieldFormat Format { get { return default(System.Diagnostics.Tracing.EventFieldFormat); } set { } }
        /// <summary>
        /// Gets and sets the user-defined <see cref="EventFieldTags" />
        /// value that is required for fields that contain data that isn't one of the supported types.
        /// </summary>
        /// <returns>
        /// Returns <see cref="EventFieldTags" />.
        /// </returns>
        public System.Diagnostics.Tracing.EventFieldTags Tags { get { return default(System.Diagnostics.Tracing.EventFieldTags); } set { } }
    }
    /// <summary>
    /// Specifies how to format the value of a user-defined type and can be used to override the default
    /// formatting for a field.
    /// </summary>
    public enum EventFieldFormat
    {
        /// <summary>
        /// Boolean
        /// </summary>
        Boolean = 3,
        /// <summary>
        /// Default.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Hexadecimal.
        /// </summary>
        Hexadecimal = 4,
        /// <summary>
        /// HResult.
        /// </summary>
        HResult = 15,
        /// <summary>
        /// JSON.
        /// </summary>
        Json = 12,
        /// <summary>
        /// String.
        /// </summary>
        String = 2,
        /// <summary>
        /// XML.
        /// </summary>
        Xml = 11,
    }
    /// <summary>
    /// Specifies the user-defined tag that is placed on fields of user-defined types that are passed
    /// as <see cref="EventSource" /> payloads through the
    /// <see cref="EventFieldAttribute" />.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventFieldTags
    {
        /// <summary>
        /// Specifies no tag and is equal to zero.
        /// </summary>
        None = 0,
    }
    /// <summary>
    /// Specifies a property should be ignored when writing an event type with the
    /// <see cref="EventSource.Write``1(System.String,System.Diagnostics.Tracing.EventSourceOptions@,``0@)" /> method.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(128))]
    public partial class EventIgnoreAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventIgnoreAttribute" />
        /// class.
        /// </summary>
        public EventIgnoreAttribute() { }
    }
    /// <summary>
    /// Defines the standard keywords that apply to events.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventKeywords : long
    {
        /// <summary>
        /// All the bits are set to 1, representing every possible group of events.
        /// </summary>
        All = (long)-1,
        /// <summary>
        /// Attached to all failed security audit events. Use this keyword only  for events in the security
        /// log.
        /// </summary>
        AuditFailure = (long)4503599627370496,
        /// <summary>
        /// Attached to all successful security audit events. Use this keyword only for events in the security
        /// log.
        /// </summary>
        AuditSuccess = (long)9007199254740992,
        /// <summary>
        /// Attached to transfer events where the related activity ID (correlation ID) is a computed value
        /// and is not guaranteed to be unique (that is, it is not a real GUID).
        /// </summary>
        CorrelationHint = (long)4503599627370496,
        /// <summary>
        /// Attached to events that are raised by using the RaiseEvent function.
        /// </summary>
        EventLogClassic = (long)36028797018963968,
        /// <summary>
        /// No filtering on keywords is performed when the event is published.
        /// </summary>
        None = (long)0,
        /// <summary>
        /// Attached to all Service Quality Mechanism (SQM) events.
        /// </summary>
        Sqm = (long)2251799813685248,
        /// <summary>
        /// Attached to all Windows Diagnostics Infrastructure (WDI) context events.
        /// </summary>
        WdiContext = (long)562949953421312,
        /// <summary>
        /// Attached to all Windows Diagnostics Infrastructure (WDI) diagnostic events.
        /// </summary>
        WdiDiagnostic = (long)1125899906842624,
    }
    /// <summary>
    /// Identifies the level of an event.
    /// </summary>
    public enum EventLevel
    {
        /// <summary>
        /// This level corresponds to a critical error, which is a serious error that has caused a major
        /// failure.
        /// </summary>
        Critical = 1,
        /// <summary>
        /// This level adds standard errors that signify a problem.
        /// </summary>
        Error = 2,
        /// <summary>
        /// This level adds informational events or messages that are not errors. These events can help
        /// trace the progress or state of an application.
        /// </summary>
        Informational = 4,
        /// <summary>
        /// No level filtering is done on the event.
        /// </summary>
        LogAlways = 0,
        /// <summary>
        /// This level adds lengthy events or messages. It causes all events to be logged.
        /// </summary>
        Verbose = 5,
        /// <summary>
        /// This level adds warning events (for example, events that are published because a disk is nearing
        /// full capacity).
        /// </summary>
        Warning = 3,
    }
    /// <summary>
    /// Provides methods for enabling and disabling events from event sources.
    /// </summary>
    public abstract partial class EventListener : System.IDisposable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EventListener" /> class.
        /// </summary>
        protected EventListener() { }
        /// <summary>
        /// Disables all events for the specified event source.
        /// </summary>
        /// <param name="eventSource">The event source to disable events for.</param>
        public void DisableEvents(System.Diagnostics.Tracing.EventSource eventSource) { }
        /// <summary>
        /// Releases the resources used by the current instance of the
        /// <see cref="EventListener" /> class.
        /// </summary>
        public virtual void Dispose() { }
        /// <summary>
        /// Enables events for the specified event source that has the specified verbosity level or lower.
        /// </summary>
        /// <param name="eventSource">The event source to enable events for.</param>
        /// <param name="level">The level of events to enable.</param>
        public void EnableEvents(System.Diagnostics.Tracing.EventSource eventSource, System.Diagnostics.Tracing.EventLevel level) { }
        /// <summary>
        /// Enables events for the specified event source that has the specified verbosity level or lower,
        /// and matching keyword flags.
        /// </summary>
        /// <param name="eventSource">The event source to enable events for.</param>
        /// <param name="level">The level of events to enable.</param>
        /// <param name="matchAnyKeyword">The keyword flags necessary to enable the events.</param>
        public void EnableEvents(System.Diagnostics.Tracing.EventSource eventSource, System.Diagnostics.Tracing.EventLevel level, System.Diagnostics.Tracing.EventKeywords matchAnyKeyword) { }
        /// <summary>
        /// Enables events for the specified event source that has the specified verbosity level or lower,
        /// matching event keyword flag, and matching arguments.
        /// </summary>
        /// <param name="eventSource">The event source to enable events for.</param>
        /// <param name="level">The level of events to enable.</param>
        /// <param name="matchAnyKeyword">The keyword flags necessary to enable the events.</param>
        /// <param name="arguments">The arguments to be matched to enable the events.</param>
        public void EnableEvents(System.Diagnostics.Tracing.EventSource eventSource, System.Diagnostics.Tracing.EventLevel level, System.Diagnostics.Tracing.EventKeywords matchAnyKeyword, System.Collections.Generic.IDictionary<string, string> arguments) { }
        /// <summary>
        /// Gets a small non-negative number that represents the specified event source.
        /// </summary>
        /// <param name="eventSource">The event source to find the index for.</param>
        /// <returns>
        /// A small non-negative number that represents the specified event source.
        /// </returns>
        public static int EventSourceIndex(System.Diagnostics.Tracing.EventSource eventSource) { return default(int); }
        /// <summary>
        /// Called for all existing event sources when the event listener is created and when a new event
        /// source is attached to the listener.
        /// </summary>
        /// <param name="eventSource">The event source.</param>
        protected internal virtual void OnEventSourceCreated(System.Diagnostics.Tracing.EventSource eventSource) { }
        /// <summary>
        /// Called whenever an event has been written by an event source for which the event listener has
        /// enabled events.
        /// </summary>
        /// <param name="eventData">The event arguments that describe the event.</param>
        protected internal abstract void OnEventWritten(System.Diagnostics.Tracing.EventWrittenEventArgs eventData);
    }
    /// <summary>
    /// Specifies how the ETW manifest for the event source is generated.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventManifestOptions
    {
        /// <summary>
        /// Generates a resources node under the localization folder for every satellite assembly provided.
        /// </summary>
        AllCultures = 2,
        /// <summary>
        /// Overrides the default behavior that the current <see cref="EventSource" />
        /// must be the base class of the user-defined type passed to the write method. This enables
        /// the validation of .NET event sources.
        /// </summary>
        AllowEventSourceOverride = 8,
        /// <summary>
        /// No options are specified.
        /// </summary>
        None = 0,
        /// <summary>
        /// A manifest is generated only the event source must be registered on the host computer.
        /// </summary>
        OnlyIfNeededForRegistration = 4,
        /// <summary>
        /// Causes an exception to be raised if any inconsistencies occur when writing the manifest file.
        /// </summary>
        Strict = 1,
    }
    /// <summary>
    /// Defines the standard operation codes that the event source attaches to events.
    /// </summary>
    public enum EventOpcode
    {
        /// <summary>
        /// A trace collection start event.
        /// </summary>
        DataCollectionStart = 3,
        /// <summary>
        /// A trace collection stop event.
        /// </summary>
        DataCollectionStop = 4,
        /// <summary>
        /// An extension event.
        /// </summary>
        Extension = 5,
        /// <summary>
        /// An informational event.
        /// </summary>
        Info = 0,
        /// <summary>
        /// An event that is published when one activity in an application receives data.
        /// </summary>
        Receive = 240,
        /// <summary>
        /// An event that is published after an activity in an application replies to an event.
        /// </summary>
        Reply = 6,
        /// <summary>
        /// An event that is published after an activity in an application resumes from a suspended state.
        /// The event should follow an event that has the <see cref="Suspend" />
        /// operation code.
        /// </summary>
        Resume = 7,
        /// <summary>
        /// An event that is published when one activity in an application transfers data or system resources
        /// to another activity.
        /// </summary>
        Send = 9,
        /// <summary>
        /// An event that is published when an application starts a new transaction or activity. This
        /// operation code can be embedded within another transaction or activity when multiple events
        /// that have the <see cref="Start" /> code follow each
        /// other without an intervening event that has a <see cref="Stop" />
        /// code.
        /// </summary>
        Start = 1,
        /// <summary>
        /// An event that is published when an activity or a transaction in an application ends. The event
        /// corresponds to the last unpaired event that has a <see cref="Start" />
        /// operation code.
        /// </summary>
        Stop = 2,
        /// <summary>
        /// An event that is published when an activity in an application is suspended.
        /// </summary>
        Suspend = 8,
    }
    /// <summary>
    /// Provides the ability to create events for event tracing for Windows (ETW).
    /// </summary>
    public partial class EventSource : System.IDisposable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EventSource" /> class.
        /// </summary>
        protected EventSource() { }
        /// <summary>
        /// Creates a new instance of the <see cref="EventSource" /> class
        /// and specifies whether to throw an exception when an error occurs in the underlying Windows
        /// code.
        /// </summary>
        /// <param name="throwOnEventWriteErrors">
        /// true to throw an exception when an error occurs in the underlying Windows code; otherwise,
        /// false.
        /// </param>
        protected EventSource(bool throwOnEventWriteErrors) { }
        /// <summary>
        /// Creates a new instance of the <see cref="EventSource" /> class
        /// with the specified configuration settings.
        /// </summary>
        /// <param name="settings">
        /// A bitwise combination of the enumeration values that specify the configuration settings to
        /// apply to the event source.
        /// </param>
        protected EventSource(System.Diagnostics.Tracing.EventSourceSettings settings) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSource" />
        /// to be used with non-contract events that contains the specified settings and traits.
        /// </summary>
        /// <param name="settings">
        /// A bitwise combination of the enumeration values that specify the configuration settings to
        /// apply to the event source.
        /// </param>
        /// <param name="traits">The key-value pairs that specify traits for the event source.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="traits" /> is not specified in key-value pairs.
        /// </exception>
        protected EventSource(System.Diagnostics.Tracing.EventSourceSettings settings, params string[] traits) { }
        /// <summary>
        /// Creates a new instance of the <see cref="EventSource" /> class
        /// with the specified name.
        /// </summary>
        /// <param name="eventSourceName">The name to apply to the event source. Must not be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventSourceName" /> is null.</exception>
        public EventSource(string eventSourceName) { }
        /// <summary>
        /// Creates a new instance of the <see cref="EventSource" /> class
        /// with the specified name and settings.
        /// </summary>
        /// <param name="eventSourceName">The name to apply to the event source. Must not be null.</param>
        /// <param name="config">
        /// A bitwise combination of the enumeration values that specify the configuration settings to
        /// apply to the event source.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="eventSourceName" /> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="eventSourceName" /> is null.</exception>
        public EventSource(string eventSourceName, System.Diagnostics.Tracing.EventSourceSettings config) { }
        /// <summary>
        /// Creates a new instance of the <see cref="EventSource" /> class
        /// with the specified configuration settings.
        /// </summary>
        /// <param name="eventSourceName">The name to apply to the event source. Must not be null.</param>
        /// <param name="config">
        /// A bitwise combination of the enumeration values that specify the configuration settings to
        /// apply to the event source.
        /// </param>
        /// <param name="traits">The key-value pairs that specify traits for the event source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventSourceName" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="traits" /> is not specified in key-value pairs.
        /// </exception>
        public EventSource(string eventSourceName, System.Diagnostics.Tracing.EventSourceSettings config, params string[] traits) { }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Gets any exception that was thrown
        /// during the construction of the event source.
        /// </summary>
        /// <returns>
        /// The exception that was thrown during the construction of the event source, or null if no exception
        /// was thrown.
        /// </returns>
        public System.Exception ConstructionException { get { return default(System.Exception); } }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Gets the activity ID of the current
        /// thread.
        /// </summary>
        /// <returns>
        /// The activity ID of the current thread.
        /// </returns>
        public static System.Guid CurrentThreadActivityId {[System.Security.SecuritySafeCriticalAttribute]get { return default(System.Guid); } }
        /// <summary>
        /// The unique identifier for the event source.
        /// </summary>
        /// <returns>
        /// A unique identifier for the event source.
        /// </returns>
        public System.Guid Guid { get { return default(System.Guid); } }
        /// <summary>
        /// The friendly name of the class that is derived from the event source.
        /// </summary>
        /// <returns>
        /// The friendly name of the derived class.  The default is the simple name of the class.
        /// </returns>
        public string Name { get { return default(string); } }
        /// <summary>
        /// Gets the settings applied to this event source.
        /// </summary>
        /// <returns>
        /// The settings applied to this event source.
        /// </returns>
        public System.Diagnostics.Tracing.EventSourceSettings Settings { get { return default(System.Diagnostics.Tracing.EventSourceSettings); } }
        /// <summary>
        /// Occurs when a command comes from an event listener.
        /// </summary>
        public event EventHandler<EventCommandEventArgs> EventCommandExecuted { add {} remove {} }
        /// <summary>
        /// Releases all resources used by the current instance of the
        /// <see cref="EventSource" /> class.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="EventSource" />
        /// class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) { }
        /// <summary>
        /// Allows the <see cref="EventSource" /> object to attempt to free
        /// resources and perform other cleanup operations before the  object is reclaimed by garbage
        /// collection.
        /// </summary>
        ~EventSource() { }
        /// <summary>
        /// Returns a string of the XML manifest that is associated with the current event source.
        /// </summary>
        /// <param name="eventSourceType">The type of the event source.</param>
        /// <param name="assemblyPathToIncludeInManifest">
        /// The path to the assembly file (.dll) to include in the provider element of the manifest.
        /// </param>
        /// <returns>
        /// The XML data string.
        /// </returns>
        public static string GenerateManifest(System.Type eventSourceType, string assemblyPathToIncludeInManifest) { return default(string); }
        /// <summary>
        /// Returns a string of the XML manifest that is associated with the current event source.
        /// </summary>
        /// <param name="eventSourceType">The type of the event source.</param>
        /// <param name="assemblyPathToIncludeInManifest">
        /// The path to the assembly file (.dll) file to include in the provider element of the manifest.
        /// </param>
        /// <param name="flags">
        /// A bitwise combination of the enumeration values that specify how the manifest is generated.
        /// </param>
        /// <returns>
        /// The XML data string or null (see remarks).
        /// </returns>
        public static string GenerateManifest(System.Type eventSourceType, string assemblyPathToIncludeInManifest, System.Diagnostics.Tracing.EventManifestOptions flags) { return default(string); }
        /// <summary>
        /// Gets the unique identifier for this implementation of the event source.
        /// </summary>
        /// <param name="eventSourceType">The type of the event source.</param>
        /// <returns>
        /// A unique identifier for this event source type.
        /// </returns>
        public static System.Guid GetGuid(System.Type eventSourceType) { return default(System.Guid); }
        /// <summary>
        /// Gets the friendly name of the event source.
        /// </summary>
        /// <param name="eventSourceType">The type of the event source.</param>
        /// <returns>
        /// The friendly name of the event source. The default is the simple name of the class.
        /// </returns>
        public static string GetName(System.Type eventSourceType) { return default(string); }
        /// <summary>
        /// Gets a snapshot of all the event sources for the application domain.
        /// </summary>
        /// <returns>
        /// An enumeration of all the event sources in the application domain.
        /// </returns>
        public static System.Collections.Generic.IEnumerable<System.Diagnostics.Tracing.EventSource> GetSources() { return default(System.Collections.Generic.IEnumerable<System.Diagnostics.Tracing.EventSource>); }
        /// <summary>
        /// Gets the trait value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the trait to get.</param>
        /// <returns>
        /// The trait value associated with the specified key. If the key is not found, returns null.
        /// </returns>
        public string GetTrait(string key) { return default(string); }
        /// <summary>
        /// Determines whether the current event source is enabled.
        /// </summary>
        /// <returns>
        /// true if the current event source is enabled; otherwise, false.
        /// </returns>
        public bool IsEnabled() { return default(bool); }
        /// <summary>
        /// Determines whether the current event source that has the specified level and keyword is enabled.
        /// </summary>
        /// <param name="level">The level of the event source.</param>
        /// <param name="keywords">The keyword of the event source.</param>
        /// <returns>
        /// true if the event source is enabled; otherwise, false.
        /// </returns>
        public bool IsEnabled(System.Diagnostics.Tracing.EventLevel level, System.Diagnostics.Tracing.EventKeywords keywords) { return default(bool); }
        /// <summary>
        /// Determines whether the current event source is enabled for events with the specified level,
        /// keywords and channel.
        /// </summary>
        /// <param name="level">
        /// The event level to check. An event source will be considered enabled when its level is greater
        /// than or equal to <paramref name="level" />.
        /// </param>
        /// <param name="keywords">The event keywords to check.</param>
        /// <param name="channel">The event channel to check.</param>
        /// <returns>
        /// true if the event source is enabled for the specified event level, keywords and channel; otherwise,
        /// false.The result of this method is only an approximation of whether a particular event is active.
        /// Use it to avoid expensive computation for logging when logging is disabled.   Event sources
        /// may have additional filtering that determines their activity..
        /// </returns>
        public bool IsEnabled(System.Diagnostics.Tracing.EventLevel level, System.Diagnostics.Tracing.EventKeywords keywords, System.Diagnostics.Tracing.EventChannel channel) { return default(bool); }
        /// <summary>
        /// Called when the current event source is updated by the controller.
        /// </summary>
        /// <param name="command">The arguments for the event.</param>
        protected virtual void OnEventCommand(System.Diagnostics.Tracing.EventCommandEventArgs command) { }
        /// <summary>
        /// Sends a command to a specified event source.
        /// </summary>
        /// <param name="eventSource">The event source to send the command to.</param>
        /// <param name="command">The event command to send.</param>
        /// <param name="commandArguments">The arguments for the event command.</param>
        public static void SendCommand(System.Diagnostics.Tracing.EventSource eventSource, System.Diagnostics.Tracing.EventCommand command, System.Collections.Generic.IDictionary<string, string> commandArguments) { }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Sets the activity ID on the current
        /// thread.
        /// </summary>
        /// <param name="activityId">
        /// The current thread's new activity ID, or <see cref="System.Guid.Empty" /> to indicate that
        /// work on the current thread is not associated with any activity.
        /// </param>
        public static void SetCurrentThreadActivityId(System.Guid activityId) { }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Sets the activity ID on the current
        /// thread, and returns the previous activity ID.
        /// </summary>
        /// <param name="activityId">
        /// The current thread's new activity ID, or <see cref="System.Guid.Empty" /> to indicate that
        /// work on the current thread is not associated with any activity.
        /// </param>
        /// <param name="oldActivityThatWillContinue">
        /// When this method returns, contains the previous activity ID on the current thread.
        /// </param>
        public static void SetCurrentThreadActivityId(System.Guid activityId, out System.Guid oldActivityThatWillContinue) { oldActivityThatWillContinue = default(System.Guid); }
        /// <summary>
        /// Obtains a string representation of the current event source instance.
        /// </summary>
        /// <returns>
        /// The name and unique identifier that identify the current event source.
        /// </returns>
        public override string ToString() { return default(string); }
        /// <summary>
        /// Writes an event without fields, but with the specified name and default options.
        /// </summary>
        /// <param name="eventName">The name of the event to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventName" /> is null.</exception>
        public void Write(string eventName) { }
        /// <summary>
        /// Writes an event without fields, but with the specified name and options.
        /// </summary>
        /// <param name="eventName">The name of the event to write.</param>
        /// <param name="options">The options such as level, keywords and operation code for the event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="eventName" /> is null.</exception>
        public void Write(string eventName, System.Diagnostics.Tracing.EventSourceOptions options) { }
        /// <summary>
        /// Writes an event with the specified name and data.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">
        /// The event data. This type must be an anonymous type or marked with the
        /// <see cref="EventDataAttribute" /> attribute.
        /// </param>
        /// <typeparam name="T">
        /// The type that defines the event and its associated data. This type must be an anonymous type
        /// or marked with the <see cref="EventSourceAttribute" /> attribute.
        /// </typeparam>
        public void Write<T>(string eventName, T data) { }
        /// <summary>
        /// Writes an event with the specified name, event data and options.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="options">The event options.</param>
        /// <param name="data">
        /// The event data. This type must be an anonymous type or marked with the
        /// <see cref="EventDataAttribute" /> attribute.
        /// </param>
        /// <typeparam name="T">
        /// The type that defines the event and its associated data. This type must be an anonymous type
        /// or marked with the <see cref="EventSourceAttribute" /> attribute.
        /// </typeparam>
        public void Write<T>(string eventName, System.Diagnostics.Tracing.EventSourceOptions options, T data) { }
        /// <summary>
        /// Writes an event with the specified name, options and event data.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="options">The event options.</param>
        /// <param name="data">
        /// The event data. This type must be an anonymous type or marked with the
        /// <see cref="EventDataAttribute" /> attribute.
        /// </param>
        /// <typeparam name="T">
        /// The type that defines the event and its associated data. This type must be an anonymous type
        /// or marked with the <see cref="EventSourceAttribute" /> attribute.
        /// </typeparam>
        public void Write<T>(string eventName, ref System.Diagnostics.Tracing.EventSourceOptions options, ref T data) { }
        /// <summary>
        /// Writes an event with the specified name, options, related activity and event data.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="options">The event options.</param>
        /// <param name="activityId">The ID of the activity associated with the event.</param>
        /// <param name="relatedActivityId">
        /// The ID of an associated activity, or <see cref="System.Guid.Empty" /> if there is no associated
        /// activity.
        /// </param>
        /// <param name="data">
        /// The event data. This type must be an anonymous type or marked with the
        /// <see cref="EventDataAttribute" /> attribute.
        /// </param>
        /// <typeparam name="T">
        /// The type that defines the event and its associated data. This type must be an anonymous type
        /// or marked with the <see cref="EventSourceAttribute" /> attribute.
        /// </typeparam>
        public void Write<T>(string eventName, ref System.Diagnostics.Tracing.EventSourceOptions options, ref System.Guid activityId, ref System.Guid relatedActivityId, ref T data) { }
        /// <summary>
        /// Writes an event by using the provided event identifier.
        /// </summary>
        /// <param name="eventId">The event identifier. This value should be between 0 and 65535.</param>
        protected void WriteEvent(int eventId) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and byte array argument.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A byte array argument.</param>
        protected void WriteEvent(int eventId, byte[] arg1) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 32-bit integer argument.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">An integer argument.</param>
        protected void WriteEvent(int eventId, int arg1) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 32-bit integer arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">An integer argument.</param>
        /// <param name="arg2">An integer argument.</param>
        protected void WriteEvent(int eventId, int arg1, int arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 32-bit integer arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">An integer argument.</param>
        /// <param name="arg2">An integer argument.</param>
        /// <param name="arg3">An integer argument.</param>
        protected void WriteEvent(int eventId, int arg1, int arg2, int arg3) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 32-bit integer and string arguments.
        /// </summary>
        /// <param name="eventId">The event identifier. This value should be between 0 and 65535.</param>
        /// <param name="arg1">A 32-bit integer argument.</param>
        /// <param name="arg2">A string argument.</param>
        protected void WriteEvent(int eventId, int arg1, string arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 64-bit integer argument.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A 64 bit integer argument.</param>
        protected void WriteEvent(int eventId, long arg1) { }
        /// <summary>
        /// Writes the event data using the specified indentifier and 64-bit integer and byte array arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A 64-bit integer argument.</param>
        /// <param name="arg2">A byte array argument.</param>
        protected void WriteEvent(int eventId, long arg1, byte[] arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 64-bit arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A 64 bit integer argument.</param>
        /// <param name="arg2">A 64 bit integer argument.</param>
        protected void WriteEvent(int eventId, long arg1, long arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 64-bit arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A 64 bit integer argument.</param>
        /// <param name="arg2">A 64 bit integer argument.</param>
        /// <param name="arg3">A 64 bit integer argument.</param>
        protected void WriteEvent(int eventId, long arg1, long arg2, long arg3) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and 64-bit integer, and string arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A 64-bit integer argument.</param>
        /// <param name="arg2">A string argument.</param>
        protected void WriteEvent(int eventId, long arg1, string arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and array of arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="args">An array of objects.</param>
        protected void WriteEvent(int eventId, params object[] args) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and string argument.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A string argument.</param>
        protected void WriteEvent(int eventId, string arg1) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A 32 bit integer argument.</param>
        protected void WriteEvent(int eventId, string arg1, int arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A 32 bit integer argument.</param>
        /// <param name="arg3">A 32 bit integer argument.</param>
        protected void WriteEvent(int eventId, string arg1, int arg2, int arg3) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A 64 bit integer argument.</param>
        protected void WriteEvent(int eventId, string arg1, long arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and string arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A string argument.</param>
        protected void WriteEvent(int eventId, string arg1, string arg2) { }
        /// <summary>
        /// Writes an event by using the provided event identifier and string arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.  This value should be between 0 and 65535.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A string argument.</param>
        /// <param name="arg3">A string argument.</param>
        protected void WriteEvent(int eventId, string arg1, string arg2, string arg3) { }
        /// <summary>
        /// Creates a new <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overload
        /// by using the provided event identifier and event data.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventDataCount">The number of event data items.</param>
        /// <param name="data">The structure that contains the event data.</param>
        [System.CLSCompliantAttribute(false)]
        [System.Security.SecurityCriticalAttribute]
        protected unsafe void WriteEventCore(int eventId, int eventDataCount, System.Diagnostics.Tracing.EventSource.EventData* data) { }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Writes an event that indicates that
        /// the current activity is related to another activity.
        /// </summary>
        /// <param name="eventId">
        /// An identifier that uniquely identifies this event within the
        /// <see cref="EventSource" />.
        /// </param>
        /// <param name="relatedActivityId">The related activity identifier.</param>
        /// <param name="args">An array of objects that contain data about the event.</param>
        protected void WriteEventWithRelatedActivityId(int eventId, System.Guid relatedActivityId, params object[] args) { }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Writes an event that indicates that
        /// the current activity is related to another activity.
        /// </summary>
        /// <param name="eventId">
        /// An identifier that uniquely identifies this event within the
        /// <see cref="EventSource" />.
        /// </param>
        /// <param name="relatedActivityId">A pointer to the GUID of the related activity ID.</param>
        /// <param name="eventDataCount">The number of items in the <paramref name="data" /> field.</param>
        /// <param name="data">A pointer to the first item in the event data field.</param>
        [System.CLSCompliantAttribute(false)]
        [System.Security.SecurityCriticalAttribute]
        protected unsafe void WriteEventWithRelatedActivityIdCore(int eventId, System.Guid* relatedActivityId, int eventDataCount, System.Diagnostics.Tracing.EventSource.EventData* data) { }
        /// <summary>
        /// Provides the event data for creating fast
        /// <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overloads by using the
        /// <see cref="WriteEventCore(Int32,Int32,EventData*)" /> method.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        protected internal partial struct EventData
        {
            /// <summary>
            /// Gets or sets the pointer to the data for the new
            /// <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overload.
            /// </summary>
            /// <returns>
            /// The pointer to the data.
            /// </returns>
            public System.IntPtr DataPointer { get { return default(System.IntPtr); } set { } }
            /// <summary>
            /// Gets or sets the number of payload items in the new
            /// <see cref="Overload:System.Diagnostics.Tracing.EventSource.WriteEvent" /> overload.
            /// </summary>
            /// <returns>
            /// The number of payload items in the new overload.
            /// </returns>
            public int Size { get { return default(int); } set { } }
        }
    }
    /// <summary>
    /// Allows the event tracing for Windows (ETW) name to be defined independently of the name of
    /// the event source class.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(4))]
    public sealed partial class EventSourceAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceAttribute" />
        /// class.
        /// </summary>
        public EventSourceAttribute() { }
        /// <summary>
        /// Gets or sets the event source identifier.
        /// </summary>
        /// <returns>
        /// The event source identifier.
        /// </returns>
        public string Guid { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the name of the localization resource file.
        /// </summary>
        /// <returns>
        /// The name of the localization resource file, or null if the localization resource file does
        /// not exist.
        /// </returns>
        public string LocalizationResources { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the name of the event source.
        /// </summary>
        /// <returns>
        /// The name of the event source.
        /// </returns>
        public string Name { get { return default(string); } set { } }
    }
    /// <summary>
    /// The exception that is thrown when an error occurs during event tracing for Windows (ETW).
    /// </summary>
    public partial class EventSourceException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceException" />
        /// class.
        /// </summary>
        public EventSourceException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceException" />
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EventSourceException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceException" />
        /// class with a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or null if no inner exception is
        /// specified.
        /// </param>
        public EventSourceException(string message, System.Exception innerException) { }
    }
    /// <summary>
    /// Specifies overrides of default event settings such as the log level, keywords and operation
    /// code when the
    /// <see cref="EventSource.Write{T}(string, EventSourceOptions, T)" /> method is called.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct EventSourceOptions
    {
        /// <summary>
        /// The activity options defined for this event source.
        /// </summary>
        /// <returns>
        /// Returns <see cref="EventActivityOptions" />.
        /// </returns>
        public System.Diagnostics.Tracing.EventActivityOptions ActivityOptions { get { return default(System.Diagnostics.Tracing.EventActivityOptions); } set { } }
        /// <summary>
        /// Gets or sets the keywords applied to the event. If this property is not set, the event’s keywords
        /// will be None.
        /// </summary>
        /// <returns>
        /// The keywords applied to the event, or None if no keywords are set.
        /// </returns>
        public System.Diagnostics.Tracing.EventKeywords Keywords { get { return default(System.Diagnostics.Tracing.EventKeywords); } set { } }
        /// <summary>
        /// Gets or sets the event level applied to the event.
        /// </summary>
        /// <returns>
        /// The event level for the event. If not set, the default is Verbose (5).
        /// </returns>
        public System.Diagnostics.Tracing.EventLevel Level { get { return default(System.Diagnostics.Tracing.EventLevel); } set { } }
        /// <summary>
        /// Gets or sets the operation code to use for the specified event.
        /// </summary>
        /// <returns>
        /// The operation code to use for the specified event. If not set, the default is Info (0).
        /// </returns>
        public System.Diagnostics.Tracing.EventOpcode Opcode { get { return default(System.Diagnostics.Tracing.EventOpcode); } set { } }
        /// <summary>
        /// The event tags defined for this event source.
        /// </summary>
        /// <returns>
        /// Returns <see cref="EventTags" />.
        /// </returns>
        public System.Diagnostics.Tracing.EventTags Tags { get { return default(System.Diagnostics.Tracing.EventTags); } set { } }
    }
    /// <summary>
    /// Specifies configuration options for an event source.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventSourceSettings
    {
        /// <summary>
        /// None of the special configuration options are enabled.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The ETW listener should use a manifest-based format when raising events. Setting this option
        /// is a directive to the ETW listener should use manifest-based format when raising events. This
        /// is the default option when defining a type derived from <see cref="EventSource" />
        /// using one of the protected <see cref="EventSource" /> constructors.
        /// </summary>
        EtwManifestEventFormat = 4,
        /// <summary>
        /// The ETW listener should use self-describing event format. This is the default option when
        /// creating a new instance of the <see cref="EventSource" /> using
        /// one of the public <see cref="EventSource" /> constructors.
        /// </summary>
        EtwSelfDescribingEventFormat = 8,
        /// <summary>
        /// The event source throws an exception when an error occurs.
        /// </summary>
        ThrowOnEventWriteErrors = 1,
    }
    /// <summary>
    /// Specifies the tracking of activity start and stop events. You should only use the lower 24
    /// bits. For more information, see <see cref="EventSourceOptions" />
    /// and
    /// <see cref="EventSource.Write(String,EventSourceOptions)" />.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventTags
    {
        /// <summary>
        /// Specifies no tag and is equal to zero.
        /// </summary>
        None = 0,
    }
    /// <summary>
    /// Defines the tasks that apply to events.
    /// </summary>
    public enum EventTask
    {
        /// <summary>
        /// Undefined task.
        /// </summary>
        None = 0,
    }
    /// <summary>
    /// Provides data for the
    /// <see cref="EventListener.OnEventWritten(EventWrittenEventArgs)" /> callback.
    /// </summary>
    public partial class EventWrittenEventArgs : System.EventArgs
    {
        internal EventWrittenEventArgs() { }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Gets the activity ID on the thread
        /// that the event was written to.
        /// </summary>
        /// <returns>
        /// The activity ID on the thread that the event was written to.
        /// </returns>
        public System.Guid ActivityId {[System.Security.SecurityCriticalAttribute]get { return default(System.Guid); } }
        /// <summary>
        /// Gets the channel for the event.
        /// </summary>
        /// <returns>
        /// The channel for the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventChannel Channel { get { return default(System.Diagnostics.Tracing.EventChannel); } }
        /// <summary>
        /// Gets the event identifier.
        /// </summary>
        /// <returns>
        /// The event identifier.
        /// </returns>
        public int EventId { get { return default(int); } }
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        /// <returns>
        /// The name of the event.
        /// </returns>
        public string EventName { get { return default(string); } }
        /// <summary>
        /// Gets the event source object.
        /// </summary>
        /// <returns>
        /// The event source object.
        /// </returns>
        public System.Diagnostics.Tracing.EventSource EventSource { get { return default(System.Diagnostics.Tracing.EventSource); } }
        /// <summary>
        /// Gets the keywords for the event.
        /// </summary>
        /// <returns>
        /// The keywords for the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventKeywords Keywords { get { return default(System.Diagnostics.Tracing.EventKeywords); } }
        /// <summary>
        /// Gets the level of the event.
        /// </summary>
        /// <returns>
        /// The level of the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventLevel Level { get { return default(System.Diagnostics.Tracing.EventLevel); } }
        /// <summary>
        /// Gets the message for the event.
        /// </summary>
        /// <returns>
        /// The message for the event.
        /// </returns>
        public string Message { get { return default(string); } }
        /// <summary>
        /// Gets the operation code for the event.
        /// </summary>
        /// <returns>
        /// The operation code for the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventOpcode Opcode { get { return default(System.Diagnostics.Tracing.EventOpcode); } }
        /// <summary>
        /// Gets the payload for the event.
        /// </summary>
        /// <returns>
        /// The payload for the event.
        /// </returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<object> Payload { get { return default(System.Collections.ObjectModel.ReadOnlyCollection<object>); } }
        /// <summary>
        /// Returns a list of strings that represent the property names of the event.
        /// </summary>
        /// <returns>
        /// Returns <see cref="Collections.ObjectModel.ReadOnlyCollection{T}" />.
        /// </returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<string> PayloadNames { get { return default(System.Collections.ObjectModel.ReadOnlyCollection<string>); } }
        /// <summary>
        /// [Supported in the .NET Framework 4.5.1 and later versions] Gets the identifier of an activity
        /// that is related to the activity represented by the current instance.
        /// </summary>
        /// <returns>
        /// The identifier of the related activity, or <see cref="Guid.Empty" /> if there is
        /// no related activity.
        /// </returns>
        public System.Guid RelatedActivityId {[System.Security.SecurityCriticalAttribute]get { return default(System.Guid); } }
        /// <summary>
        /// Returns the tags specified in the call to the
        /// <see cref="EventSource.Write(String,EventSourceOptions)" /> method.
        /// </summary>
        /// <returns>
        /// Returns <see cref="EventTags" />.
        /// </returns>
        public System.Diagnostics.Tracing.EventTags Tags { get { return default(System.Diagnostics.Tracing.EventTags); } }
        /// <summary>
        /// Gets the task for the event.
        /// </summary>
        /// <returns>
        /// The task for the event.
        /// </returns>
        public System.Diagnostics.Tracing.EventTask Task { get { return default(System.Diagnostics.Tracing.EventTask); } }
        /// <summary>
        /// Gets the version of the event.
        /// </summary>
        /// <returns>
        /// The version of the event.
        /// </returns>
        public byte Version { get { return default(byte); } }
    }
    /// <summary>
    /// Identifies a method that is not generating an event.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
    public sealed partial class NonEventAttribute : System.Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NonEventAttribute" />
        /// class.
        /// </summary>
        public NonEventAttribute() { }
    }
}
