// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This program uses code hyperlinks available as part of the HyperAddin Visual Studio plug-in.
// It is available from http://www.codeplex.com/hyperAddin 
#if PLATFORM_WINDOWS && !ES_BUILD_STANDALONE && !CORECLR && !ES_BUILD_PN
#define FEATURE_ACTIVITYSAMPLING
#endif // !ES_BUILD_STANDALONE

#if ES_BUILD_STANDALONE
#define FEATURE_MANAGED_ETW_CHANNELS
// #define FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
#endif

/* DESIGN NOTES DESIGN NOTES DESIGN NOTES DESIGN NOTES */
// DESIGN NOTES
// Over the years EventSource has become more complex and so it is important to understand
// the basic structure of the code to insure that it does not grow more complex.
//
// Basic Model
//
// PRINCIPLE: EventSource - ETW decoupling
// 
// Conceptually and EventSouce is something takes event logging data from the source methods 
// To the EventListener that can subscribe them.  Note that CONCEPTUALLY EVENTSOURCES DON'T
// KNOW ABOUT ETW!.   The MODEL of the system is that there is a special EventListern Which
// we will call the EtwEventListener, that forwards commands from ETW to EventSources and
// listeners to the EventSources and forwards on those events to ETW.   THus the model should
// be that you DON'T NEED ETW.    
//
// Now in actual practice, EventSouce have rather intimate knowledge of ETW and send events
// to it directly, but this can be VIEWED AS AN OPTIMIATION.  
//
// Basic Event Data Flow:
// 
// There are two ways for event Data to enter the system
//     1) WriteEvent* and friends.  This is called the 'contract' based approach because 
//        you write a method per event which forms a contract that is know at compile time.
//        In this scheme each event is given an EVENTID (small integer). which is its identity
//     2) Write<T> methods.   This is called the 'dynamic' approach because new events 
//        can be created on the fly.  Event identity is determined by the event NAME, and these
//        are not quite as efficient at runtime since you have at least a hash table lookup
//        on every event write.  
//
// EventSource-EventListener transfer fully support both ways of writing events (either contract
// based (WriteEvent*) or dynamic (Write<T>).   Both way fully support the same set of data
// types.   It is suggested, however, that you use the contract based approach when the event scheme 
// is known at compile time (that is whenever possible).  It is more efficient, but more importantly
// it makes the contract very explicit, and centralizes all policy about logging.  These are good 
// things.    The Write<T> API is really meant for more ad-hoc 
//
// Allowed Data. 
// 
// Note that EventSource-EventListeners have a conceptual serialization-deserialization that happens
// during the transfer.   In particular object identity is not preserved, some objects are morphed, 
// and not all data types are supported.   In particular you can pass
// 
// A Valid type to log to an EventSource include
//   * Primitive data types
//   * IEnumerable<T> of valid types T (this include arrays)  (* New for V4.6)
//   * Explicitly Opted in class or struct with public property Getters over Valid types.  (* New for V4.6)
// 
// This set of types is roughly a generalization of JSON support (Basically primitives, bags, and arrays).   
//
// Explicitly allowed structs include (* New for V4.6)
//   * Marked with the EventData attribute
//   * implicitly defined (e.g the C# new {x = 3, y = 5} syntax)
//   * KeyValuePair<K,V>  (thus dictionaries can be passed since they are an IEnumerable of KeyValuePair)
//         
// When classes are returned in an EventListener, what is returned is something that implements 
// IDictionary<string, T>.  Thus when objects are passed to an EventSource they are transformed
// into a key-value bag (the IDictionary<string, T>) for consumption in the listener.   These 
// are obvious NOT the original objects.  
// 
// ETWserialization formats:
// 
// As mentioned conceptually EventSource's send data to EventListeners and there is a conceptual 
// copy/morph of that data as described above.   In addition the .NET framework supports a conceptual
// ETWListener that will send the data to then ETW stream.   If you use this feature, the data needs 
// to be serialized in a way that ETW supports.  ETW supports the following serialization formats 
// 
//     1) Manifest Based serialization. 
//     2) SelfDescribing serialization (TraceLogging style in the TraceLogging directory)
//
// A key factor is that the Write<T> method, which support on the fly definition of events, can't
// support the manifest based serialization because the manifest needs the schema of all events 
// to be known before any events are emitted.  This implies the following
//
// If you use Write<T> and the output goes to ETW it will use the SelfDescribing format.
// If you use the EventSource(string) constructor for an eventSource (in which you don't
// create a subclass), the default is also to use Self-Describing serialization.  In addition
// you can use the EventSoruce(EventSourceSettings) constructor to also explicitly specify
// Self-Describing serialization format.   These effect the WriteEvent* APIs going to ETW.  
//
// Note that none of this ETW serialization logic affects EventListeners.   Only the ETW listener.  
// 
// *************************************************************************************
// *** INTERNALS: Event Propagation
//
//   Data enters the system either though
//
// 1) A user defined method in the user defined subclass of EventSource which calls 
//     A) A typesafe type specific overload of WriteEvent(ID, ...)  e.g. WriteEvent(ID, string, string) 
//           * which calls into the unsafe WriteEventCore(ID COUNT EventData*) WriteEventWithRelatedActivityIdCore()
//     B) The typesafe overload WriteEvent(ID, object[])  which calls the private helper WriteEventVarargs(ID, Guid* object[])
//     C) Directly into the unsafe WriteEventCore(ID, COUNT EventData*) or WriteEventWithRelatedActivityIdCore()
//
//     All event data eventually flows to one of 
//        * WriteEventWithRelatedActivityIdCore(ID, Guid*, COUNT, EventData*) 
//        * WriteEventVarargs(ID, Guid*, object[])
//
// 2) A call to one of the overloads of Write<T>.   All these overloads end up in 
//        * WriteImpl<T>(EventName, Options, Data, Guid*, Guid*)
// 
// On output there are the following routines
//    Writing to all listeners that are NOT ETW, we have the following routines
//       * WriteToAllListeners(ID, Guid*, COUNT, EventData*) 
//       * WriteToAllListeners(ID, Guid*, object[])
//       * WriteToAllListeners(NAME, Guid*, EventPayload)
//
//       EventPayload is the internal type that implements the IDictionary<string, object> interface
//       The EventListeners will pass back for serialized classes for nested object, but  
//       WriteToAllListeners(NAME, Guid*, EventPayload) unpacks this uses the fields as if they
//       were parameters to a method.  
// 
//       The first two are used for the WriteEvent* case, and the later is used for the Write<T> case.  
// 
//    Writing to ETW, Manifest Based 
//          EventProvider.WriteEvent(EventDescriptor, Guid*, COUNT, EventData*)
//          EventProvider.WriteEvent(EventDescriptor, Guid*, object[])
//    Writing to ETW, Self-Describing format
//          WriteMultiMerge(NAME, Options, Types, EventData*)
//          WriteMultiMerge(NAME, Options, Types, object[])
//          WriteImpl<T> has logic that knows how to serialize (like WriteMultiMerge) but also knows 
//             will write it to 
//
//    All ETW writes eventually call
//      EventWriteTransfer (native PINVOKE wrapper)
//      EventWriteTransferWrapper (fixes compat problem if you pass null as the related activityID)
//         EventProvider.WriteEventRaw   - sets last error
//         EventSource.WriteEventRaw     - Does EventSource exception handling logic
//            WriteMultiMerge
//            WriteImpl<T>
//         EventProvider.WriteEvent(EventDescriptor, Guid*, COUNT, EventData*)
//         EventProvider.WriteEvent(EventDescriptor, Guid*, object[])
// 
// Serialization:  We have a bit of a hodge-podge of serializers right now.   Only the one for ETW knows
// how to deal with nested classes or arrays.   I will call this serializer the 'TypeInfo' serializer
// since it is the TraceLoggingTypeInfo structure that knows how to do this.   Effectively for a type you
// can call one of these 
//      WriteMetadata - transforms the type T into serialization meta data blob for that type
//      WriteObjectData - transforms an object of T into serialization meta data blob for that type
//      GetData - transforms an object of T into its deserialized form suitable for passing to EventListener. 
// The first two are used to serialize something for ETW.   The second one is used to transform the object 
// for use by the EventListener.    We also have a 'DecodeObject' method that will take a EventData* and
// deserialize to pass to an EventListener, but it only works on primitive types (types supported in version V4.5). 
// 
// It is an important observation that while EventSource does support users directly calling with EventData* 
// blobs, we ONLY support that for the primitive types (V4.5 level support).   Thus while there is a EventData*
// path through the system it is only for some types.  The object[] path is the more general (but less efficient) path.  
//
// TODO There is cleanup needed There should be no divergence until WriteEventRaw.  
// 
// TODO: We should have a single choke point (right now we always have this parallel EventData* and object[] path.   This 
// was historical (at one point we tried to pass object directly from EventSoruce to EventListener.  That was always 
// fragile and a compatibility headache, but we have finally been forced into the idea that there is always a transformation.
// This allows us to use the EventData* form to be the canonical data format in the low level APIs.  This also gives us the
// opportunity to expose this format to EventListeners in the future.   
// 
using System;
using System.Runtime.CompilerServices;
#if FEATURE_ACTIVITYSAMPLING
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Security;
#if !CORECLR && !ES_BUILD_PN
using System.Security.Permissions;
#endif // !CORECLR && !ES_BUILD_PN

using System.Text;
using System.Threading;
using Microsoft.Win32;

#if ES_BUILD_STANDALONE
using EventDescriptor = Microsoft.Diagnostics.Tracing.EventDescriptor;
#else
using System.Threading.Tasks;
#endif

using Microsoft.Reflection;

#if !ES_BUILD_AGAINST_DOTNET_V35
using Contract = System.Diagnostics.Contracts.Contract;
#else
using Contract = Microsoft.Diagnostics.Contracts.Internal.Contract;
#endif

#if CORECLR || ES_BUILD_PN
using Internal.Runtime.Augments;
#endif

#if ES_BUILD_STANDALONE
namespace Microsoft.Diagnostics.Tracing
#else
namespace System.Diagnostics.Tracing
#endif
{
    /// <summary>
    /// This class is meant to be inherited by a user-defined event source in order to define a managed
    /// ETW provider.   Please See DESIGN NOTES above for the internal architecture.  
    /// The minimal definition of an EventSource simply specifies a number of ETW event methods that
    /// call one of the EventSource.WriteEvent overloads, <see cref="EventSource.WriteEventCore"/>, 
    /// or <see cref="EventSource.WriteEventWithRelatedActivityIdCore"/> to log them. This functionality 
    /// is sufficient for many users.
    /// <para>
    /// To achieve more control over the ETW provider manifest exposed by the event source type, the 
    /// [<see cref="EventAttribute"/>] attributes can be specified for the ETW event methods.
    /// </para><para>
    /// For very advanced EventSources, it is possible to intercept the commands being given to the
    /// eventSource and change what filtering is done (see EventListener.EnableEvents and 
    /// <see cref="EventListener.DisableEvents"/>) or cause actions to be performed by the eventSource, 
    /// e.g. dumping a data structure (see EventSource.SendCommand and
    /// <see cref="EventSource.OnEventCommand"/>).
    /// </para><para>
    /// The eventSources can be turned on with Windows ETW controllers (e.g. logman), immediately. 
    /// It is also possible to control and intercept the data dispatcher programmatically.  See 
    /// <see cref="EventListener"/> for more.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This is a minimal definition for a custom event source:
    /// <code>
    /// [EventSource(Name="Samples-Demos-Minimal")]
    /// sealed class MinimalEventSource : EventSource
    /// {
    ///     public static MinimalEventSource Log = new MinimalEventSource();
    ///     public void Load(long ImageBase, string Name) { WriteEvent(1, ImageBase, Name); }
    ///     public void Unload(long ImageBase) { WriteEvent(2, ImageBase); }
    ///     private MinimalEventSource() {}
    /// }
    /// </code>
    /// </remarks>
    public partial class EventSource : IDisposable
    {

#if FEATURE_EVENTSOURCE_XPLAT
        private static readonly EventListener persistent_Xplat_Listener = XplatEventLogger.InitializePersistentListener();
#endif //FEATURE_EVENTSOURCE_XPLAT

        /// <summary>
        /// The human-friendly name of the eventSource.  It defaults to the simple name of the class
        /// </summary>
        public string Name { get { return m_name; } }
        /// <summary>
        /// Every eventSource is assigned a GUID to uniquely identify it to the system. 
        /// </summary>
        public Guid Guid { get { return m_guid; } }

        /// <summary>
        /// Returns true if the eventSource has been enabled at all. This is the prefered test
        /// to be performed before a relatively expensive EventSource operation.
        /// </summary>
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        public bool IsEnabled()
        {
            return m_eventSourceEnabled;
        }

        /// <summary>
        /// Returns true if events with greater than or equal 'level' and have one of 'keywords' set are enabled. 
        /// 
        /// Note that the result of this function is only an approximation on whether a particular
        /// event is active or not. It is only meant to be used as way of avoiding expensive
        /// computation for logging when logging is not on, therefore it sometimes returns false
        /// positives (but is always accurate when returning false).  EventSources are free to 
        /// have additional filtering.    
        /// </summary>
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        public bool IsEnabled(EventLevel level, EventKeywords keywords)
        {
            return IsEnabled(level, keywords, EventChannel.None);
        }

        /// <summary>
        /// Returns true if events with greater than or equal 'level' and have one of 'keywords' set are enabled, or
        /// if 'keywords' specifies a channel bit for a channel that is enabled.
        /// 
        /// Note that the result of this function only an approximation on whether a particular
        /// event is active or not. It is only meant to be used as way of avoiding expensive
        /// computation for logging when logging is not on, therefore it sometimes returns false
        /// positives (but is always accurate when returning false).  EventSources are free to 
        /// have additional filtering.    
        /// </summary>
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        public bool IsEnabled(EventLevel level, EventKeywords keywords, EventChannel channel)
        {
            if (!m_eventSourceEnabled)
                return false;

            if (!IsEnabledCommon(m_eventSourceEnabled, m_level, m_matchAnyKeyword, level, keywords, channel))
                return false;

#if !FEATURE_ACTIVITYSAMPLING

            return true;

#else // FEATURE_ACTIVITYSAMPLING

            return true;

#if OPTIMIZE_IS_ENABLED
            //================================================================================
            // 2013/03/06 - The code below is a possible optimization for IsEnabled(level, kwd)
            //    in case activity tracing/sampling is enabled. The added complexity of this
            //    code however weighs against having it "on" until we know it's really needed.
            //    For now we'll have this #ifdef-ed out in case we see evidence this is needed.
            //================================================================================            

            // At this point we believe the event is enabled, however we now need to check
            // if we filter because of activity 

            // Optimization, all activity filters also register a delegate here, so if there 
            // is no delegate, we know there are no activity filters, which means that there
            // is no additional filtering, which means that we can return true immediately.  
            if (s_activityDying == null)
                return true;

            // if there's at least one legacy ETW listener we can't filter this
            if (m_legacySessions != null && m_legacySessions.Count > 0)
                return true;

            // if any event ID that triggers a new activity, or "transfers" activities
            // is covered by 'keywords' we can't filter this
            if (unchecked(((long)keywords & m_keywordTriggers)) != 0)
                return true;

            // See if all listeners have activity filters that would block the event.
            for (int perEventSourceSessionId = 0; perEventSourceSessionId < SessionMask.MAX; ++perEventSourceSessionId)
            {
                EtwSession etwSession = m_etwSessionIdMap[perEventSourceSessionId];
                if (etwSession == null)
                    continue;

                ActivityFilter activityFilter = etwSession.m_activityFilter;
                if (activityFilter == null || 
                    ActivityFilter.GetFilter(activityFilter, this) == null)
                {
                    // No activity filter for ETW, if event is active for ETW, we can't filter.  
                    for (int i = 0; i < m_eventData.Length; i++)
                        if (m_eventData[i].EnabledForETW)
                            return true;
                }
                else if (ActivityFilter.IsCurrentActivityActive(activityFilter))
                    return true;
            }

            // for regular event listeners
            var curDispatcher = m_Dispatchers;
            while (curDispatcher != null)
            {
                ActivityFilter activityFilter = curDispatcher.m_Listener.m_activityFilter;
                if (activityFilter == null)
                {
                    // See if any event is enabled.   
                    for (int i = 0; i < curDispatcher.m_EventEnabled.Length; i++)
                        if (curDispatcher.m_EventEnabled[i])
                            return true;
                }
                else if (ActivityFilter.IsCurrentActivityActive(activityFilter))
                    return true;
                curDispatcher = curDispatcher.m_Next;
            }

            // Every listener has an activity filter that is blocking writing the event, 
            // thus the event is not enabled.  
            return false;
#endif // OPTIMIZE_IS_ENABLED

#endif // FEATURE_ACTIVITYSAMPLING
        }

        /// <summary>
        /// Returns the settings for the event source instance
        /// </summary>
        public EventSourceSettings Settings
        {
            get { return m_config; }
        }

        // Manifest support 
        /// <summary>
        /// Returns the GUID that uniquely identifies the eventSource defined by 'eventSourceType'.  
        /// This API allows you to compute this without actually creating an instance of the EventSource.   
        /// It only needs to reflect over the type.  
        /// </summary>
        public static Guid GetGuid(Type eventSourceType)
        {
            if (eventSourceType == null)
                throw new ArgumentNullException(nameof(eventSourceType));
            Contract.EndContractBlock();

            EventSourceAttribute attrib = (EventSourceAttribute)GetCustomAttributeHelper(eventSourceType, typeof(EventSourceAttribute));
            string name = eventSourceType.Name;
            if (attrib != null)
            {
                if (attrib.Guid != null)
                {
                    Guid g = Guid.Empty;
#if !ES_BUILD_AGAINST_DOTNET_V35
                    if (Guid.TryParse(attrib.Guid, out g))
                        return g;
#else
                    try { return new Guid(attrib.Guid); }
                    catch (Exception) { }
#endif
                }

                if (attrib.Name != null)
                    name = attrib.Name;
            }

            if (name == null)
            {
                throw new ArgumentException(Resources.GetResourceString("Argument_InvalidTypeName"), nameof(eventSourceType));
            }
            return GenerateGuidFromName(name.ToUpperInvariant());       // Make it case insensitive.  
        }
        /// <summary>
        /// Returns the official ETW Provider name for the eventSource defined by 'eventSourceType'.  
        /// This API allows you to compute this without actually creating an instance of the EventSource.   
        /// It only needs to reflect over the type.  
        /// </summary>
        public static string GetName(Type eventSourceType)
        {
            return GetName(eventSourceType, EventManifestOptions.None);
        }

        /// <summary>
        /// Returns a string of the XML manifest associated with the eventSourceType. The scheme for this XML is
        /// documented at in EventManifest Schema http://msdn.microsoft.com/en-us/library/aa384043(VS.85).aspx.
        /// This is the preferred way of generating a manifest to be embedded in the ETW stream as it is fast and
        /// the fact that it only includes localized entries for the current UI culture is an acceptable tradeoff.
        /// </summary>
        /// <param name="eventSourceType">The type of the event source class for which the manifest is generated</param>
        /// <param name="assemblyPathToIncludeInManifest">The manifest XML fragment contains the string name of the DLL name in
        /// which it is embedded.  This parameter specifies what name will be used</param>
        /// <returns>The XML data string</returns>
        public static string GenerateManifest(Type eventSourceType, string assemblyPathToIncludeInManifest)
        {
            return GenerateManifest(eventSourceType, assemblyPathToIncludeInManifest, EventManifestOptions.None);
        }
        /// <summary>
        /// Returns a string of the XML manifest associated with the eventSourceType. The scheme for this XML is
        /// documented at in EventManifest Schema http://msdn.microsoft.com/en-us/library/aa384043(VS.85).aspx.
        /// Pass EventManifestOptions.AllCultures when generating a manifest to be registered on the machine. This
        /// ensures that the entries in the event log will be "optimally" localized.
        /// </summary>
        /// <param name="eventSourceType">The type of the event source class for which the manifest is generated</param>
        /// <param name="assemblyPathToIncludeInManifest">The manifest XML fragment contains the string name of the DLL name in
        /// which it is embedded.  This parameter specifies what name will be used</param>
        /// <param name="flags">The flags to customize manifest generation. If flags has bit OnlyIfNeededForRegistration specified
        /// this returns null when the eventSourceType does not require explicit registration</param>
        /// <returns>The XML data string or null</returns>
        public static string GenerateManifest(Type eventSourceType, string assemblyPathToIncludeInManifest, EventManifestOptions flags)
        {
            if (eventSourceType == null)
                throw new ArgumentNullException(nameof(eventSourceType));
            Contract.EndContractBlock();

            byte[] manifestBytes = EventSource.CreateManifestAndDescriptors(eventSourceType, assemblyPathToIncludeInManifest, null, flags);
            return (manifestBytes == null) ? null : Encoding.UTF8.GetString(manifestBytes, 0, manifestBytes.Length);
        }

        // EventListener support
        /// <summary>
        /// returns a list (IEnumerable) of all sources in the appdomain).  EventListeners typically need this.  
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<EventSource> GetSources()
        {
            var ret = new List<EventSource>();
            lock (EventListener.EventListenersLock)
            {
                foreach (WeakReference eventSourceRef in EventListener.s_EventSources)
                {
                    EventSource eventSource = eventSourceRef.Target as EventSource;
                    if (eventSource != null && !eventSource.IsDisposed)
                        ret.Add(eventSource);
                }
            }
            return ret;
        }

        /// <summary>
        /// Send a command to a particular EventSource identified by 'eventSource'.
        /// Calling this routine simply forwards the command to the EventSource.OnEventCommand
        /// callback.  What the EventSource does with the command and its arguments are from 
        /// that point EventSource-specific.  
        /// </summary>
        /// <param name="eventSource">The instance of EventSource to send the command to</param>
        /// <param name="command">A positive user-defined EventCommand, or EventCommand.SendManifest</param>
        /// <param name="commandArguments">A set of (name-argument, value-argument) pairs associated with the command</param>
        public static void SendCommand(EventSource eventSource, EventCommand command, IDictionary<string, string> commandArguments)
        {
            if (eventSource == null)
                throw new ArgumentNullException(nameof(eventSource));

            // User-defined EventCommands should not conflict with the reserved commands.
            if ((int)command <= (int)EventCommand.Update && (int)command != (int)EventCommand.SendManifest)
            {
                throw new ArgumentException(Resources.GetResourceString("EventSource_InvalidCommand"), nameof(command));
            }

            eventSource.SendCommand(null, 0, 0, command, true, EventLevel.LogAlways, EventKeywords.None, commandArguments);
        }

#if !ES_BUILD_STANDALONE
        /// <summary>
        /// This property allows EventSource code to appropriately handle as "different" 
        /// activities started on different threads that have not had an activity created on them.
        /// </summary>
        internal static Guid InternalCurrentThreadActivityId
        {
            get
            {
                Guid retval = CurrentThreadActivityId;
                if (retval == Guid.Empty)
                {
                    retval = FallbackActivityId;
                }
                return retval;
            }
        }

        internal static Guid FallbackActivityId
        {
            get
            {
#pragma warning disable 612, 618
                int threadID = AppDomain.GetCurrentThreadId();

                // Managed thread IDs are more aggressively re-used than native thread IDs,
                // so we'll use the latter...
                return new Guid(unchecked((uint)threadID),
                                unchecked((ushort)s_currentPid), unchecked((ushort)(s_currentPid >> 16)),
                                0x94, 0x1b, 0x87, 0xd5, 0xa6, 0x5c, 0x36, 0x64);
#pragma warning restore 612, 618
            }
        }
#endif // !ES_BUILD_STANDALONE

        // Error APIs.  (We don't throw by default, but you can probe for status)
        /// <summary>
        /// Because
        /// 
        ///     1) Logging is often optional and thus should not generate fatal errors (exceptions)
        ///     2) EventSources are often initialized in class constructors (which propagate exceptions poorly)
        ///     
        /// The event source constructor does not throw exceptions.  Instead we remember any exception that 
        /// was generated (it is also logged to Trace.WriteLine).
        /// </summary>
        public Exception ConstructionException { get { return m_constructionException; } }

        /// <summary>
        /// EventSources can have arbitrary string key-value pairs associated with them called Traits.  
        /// These traits are not interpreted by the EventSource but may be interpreted by EventListeners
        /// (e.g. like the built in ETW listener).   These traits are specififed at EventSource 
        /// construction time and can be retrieved by using this GetTrait API.  
        /// </summary>
        /// <param name="key">The key to look up in the set of key-value pairs passed to the EventSource constructor</param>
        /// <returns>The value string associated iwth key.  Will return null if there is no such key.</returns>
        public string GetTrait(string key)
        {
            if (m_traits != null)
            {
                for (int i = 0; i < m_traits.Length - 1; i += 2)
                {
                    if (m_traits[i] == key)
                        return m_traits[i + 1];
                }
            }
            return null;
        }

        /// <summary>
        /// Displays the name and GUID for the eventSource for debugging purposes.  
        /// </summary>
        public override string ToString()
        {
            return Resources.GetResourceString("EventSource_ToString", Name, Guid);
        }

        /// <summary>
        /// Fires when a Command (e.g. Enable) comes from a an EventListener.  
        /// </summary>
        public event EventHandler<EventCommandEventArgs> EventCommandExecuted
        {
            add
            {
                m_eventCommandExecuted += value;

                // If we have an EventHandler<EventCommandEventArgs> attached to the EventSource before the first command arrives
                // It should get a chance to handle the deferred commands.
                EventCommandEventArgs deferredCommands = m_deferredCommands;
                while (deferredCommands != null)
                {
                    value(this, deferredCommands);
                    deferredCommands = deferredCommands.nextCommand;
                }
            }
            remove
            {
                m_eventCommandExecuted -= value;
            }
        }

        #region protected
        /// <summary>
        /// This is the constructor that most users will use to create their eventSource.   It takes 
        /// no parameters.  The ETW provider name and GUID of the EventSource are determined by the EventSource 
        /// custom attribute (so you can determine these things declaratively).   If the GUID for the eventSource
        /// is not specified in the EventSourceAttribute (recommended), it is Generated by hashing the name.
        /// If the ETW provider name of the EventSource is not given, the name of the EventSource class is used as
        /// the ETW provider name.
        /// </summary>
        protected EventSource()
            : this(EventSourceSettings.EtwManifestEventFormat)
        {
        }

        /// <summary>
        /// By default calling the 'WriteEvent' methods do NOT throw on errors (they silently discard the event).  
        /// This is because in most cases users assume logging is not 'precious' and do NOT wish to have logging failures
        /// crash the program. However for those applications where logging is 'precious' and if it fails the caller
        /// wishes to react, setting 'throwOnEventWriteErrors' will cause an exception to be thrown if WriteEvent
        /// fails. Note the fact that EventWrite succeeds does not necessarily mean that the event reached its destination
        /// only that operation of writing it did not fail. These EventSources will not generate self-describing ETW events.
        /// 
        /// For compatibility only use the EventSourceSettings.ThrowOnEventWriteErrors flag instead.  
        /// </summary>
        // [Obsolete("Use the EventSource(EventSourceSettings) overload")]
        protected EventSource(bool throwOnEventWriteErrors)
            : this(EventSourceSettings.EtwManifestEventFormat | (throwOnEventWriteErrors ? EventSourceSettings.ThrowOnEventWriteErrors : 0))
        { }

        /// <summary>
        /// Construct an EventSource with additional non-default settings (see EventSourceSettings for more)  
        /// </summary>
        protected EventSource(EventSourceSettings settings) : this(settings, null) { }

        /// <summary>
        /// Construct an EventSource with additional non-default settings.  
        /// 
        /// Also specify a list of key-value pairs called traits (you must pass an even number of strings).   
        /// The first string is the key and the second is the value.   These are not interpreted by EventSource
        /// itself but may be interprated the listeners.  Can be fetched with GetTrait(string).   
        /// </summary>
        /// <param name="settings">See EventSourceSettings for more.</param>
        /// <param name="traits">A collection of key-value strings (must be an even number).</param>
        protected EventSource(EventSourceSettings settings, params string[] traits)
        {
            m_config = ValidateSettings(settings);

            Guid eventSourceGuid;
            string eventSourceName;

            EventMetadata[] eventDescriptors;
            byte[] manifest;
            GetMetadata(out eventSourceGuid, out eventSourceName, out eventDescriptors, out manifest);

            if (eventSourceGuid.Equals(Guid.Empty) || eventSourceName == null)
            {
                var myType = this.GetType();
                eventSourceGuid = GetGuid(myType);
                eventSourceName = GetName(myType);
            }

            Initialize(eventSourceGuid, eventSourceName, traits);
        }

#if FEATURE_PERFTRACING
        // Generate the serialized blobs that describe events for all strongly typed events (that is events that define strongly
        // typed event methods. Dynamically defined events (that use Write) hare defined on the fly and are handled elsewhere.
        private unsafe void DefineEventPipeEvents()
        {
            Debug.Assert(m_eventData != null);
            Debug.Assert(m_provider != null);
            int cnt = m_eventData.Length;
            for (int i = 0; i < cnt; i++)
            {
                uint eventID = (uint)m_eventData[i].Descriptor.EventId;
                if (eventID == 0)
                    continue;

                string eventName = m_eventData[i].Name;
                Int64 keywords = m_eventData[i].Descriptor.Keywords;
                uint eventVersion = m_eventData[i].Descriptor.Version;
                uint level = m_eventData[i].Descriptor.Level;

                // evnetID          : 4 bytes
                // eventName        : (eventName.Length + 1) * 2 bytes
                // keywords         : 8 bytes
                // eventVersion     : 4 bytes
                // level            : 4 bytes
                // parameterCount   : 4 bytes
                uint metadataLength = 24 + ((uint)eventName.Length + 1) * 2;

                // Increase the metadataLength for the types of all parameters.
                metadataLength += (uint)m_eventData[i].Parameters.Length * 4;

                // Increase the metadataLength for the names of all parameters.
                foreach (var parameter in m_eventData[i].Parameters)
                {
                    string parameterName = parameter.Name;
                    metadataLength = metadataLength + ((uint)parameterName.Length + 1) * 2;
                }

                byte[] metadata = new byte[metadataLength];

                // Write metadata: evnetID, eventName, keywords, eventVersion, level, parameterCount, param1 type, param1 name...
                fixed (byte *pMetadata = metadata)
                {
                    uint offset = 0;
                    WriteToBuffer(pMetadata, metadataLength, ref offset, eventID);
                    fixed(char *pEventName = eventName)
                    {
                        WriteToBuffer(pMetadata, metadataLength, ref offset, (byte *)pEventName, ((uint)eventName.Length + 1) * 2);
                    }
                    WriteToBuffer(pMetadata, metadataLength, ref offset, keywords);
                    WriteToBuffer(pMetadata, metadataLength, ref offset, eventVersion);
                    WriteToBuffer(pMetadata, metadataLength, ref offset, level);
                    WriteToBuffer(pMetadata, metadataLength, ref offset, (uint)m_eventData[i].Parameters.Length);
                    foreach (var parameter in m_eventData[i].Parameters)
                    {
                        // Write parameter type.
                        WriteToBuffer(pMetadata, metadataLength, ref offset, (uint)Type.GetTypeCode(parameter.ParameterType));
                        
                        // Write parameter name.
                        string parameterName = parameter.Name;
                        fixed (char *pParameterName = parameterName)
                        {
                            WriteToBuffer(pMetadata, metadataLength, ref offset, (byte *)pParameterName, ((uint)parameterName.Length + 1) * 2);
                        }
                    }
                    Debug.Assert(metadataLength == offset);
                    IntPtr eventHandle = m_provider.m_eventProvider.DefineEventHandle(eventID, eventName, keywords, eventVersion, level, pMetadata, metadataLength);
                    m_eventData[i].EventHandle = eventHandle; 
                }
            }
        }

        // Copy src to buffer and modify the offset.
        // Note: We know the buffer size ahead of time to make sure no buffer overflow.
        private static unsafe void WriteToBuffer(byte *buffer, uint bufferLength, ref uint offset, byte *src, uint srcLength)
        {
            Debug.Assert(bufferLength >= (offset + srcLength));
            for (int i = 0; i < srcLength; i++)
            {
                *(byte *)(buffer + offset + i) = *(byte *)(src + i);
            }
            offset += srcLength;
        }

        // Copy uint value to buffer.
        private static unsafe void WriteToBuffer(byte *buffer, uint bufferLength, ref uint offset, uint value)
        {
            Debug.Assert(bufferLength >= (offset + 4));
            *(uint *)(buffer + offset) = value;
            offset += 4;
        }

        // Copy long value to buffer.
        private static unsafe void WriteToBuffer(byte *buffer, uint bufferLength, ref uint offset, long value)
        {
            Debug.Assert(bufferLength >= (offset + 8));
            *(long *)(buffer + offset) = value;
            offset += 8;
        }           
#endif

        internal virtual void GetMetadata(out Guid eventSourceGuid, out string eventSourceName, out EventMetadata[] eventData, out byte[] manifestBytes)
        {
            //
            // In ProjectN subclasses need to override this method, and return the data from their EventSourceAttribute and EventAttribute annotations.
            // On other architectures it is a no-op.
            //
            // eventDescriptors needs to contain one EventDescriptor for each event; the event's ID should be the same as its index in this array.
            // manifestBytes is a UTF-8 encoding of the ETW manifest for the type.
            //
            // This will be implemented by an IL rewriter, so we can't make this method abstract or the initial build of the subclass would fail.
            //
            eventSourceGuid = Guid.Empty;
            eventSourceName = null;
            eventData = null;
            manifestBytes = null;

            return;
        }

        /// <summary>
        /// This method is called when the eventSource is updated by the controller.  
        /// </summary>
        protected virtual void OnEventCommand(EventCommandEventArgs command) { }

#pragma warning disable 1591
        // optimized for common signatures (no args)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId)
        {
            WriteEventCore(eventId, 0, null);
        }

        // optimized for common signatures (ints)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, int arg1)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[1];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 4;
                WriteEventCore(eventId, 1, descrs);
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, int arg1, int arg2)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 4;
                descrs[1].DataPointer = (IntPtr)(&arg2);
                descrs[1].Size = 4;
                WriteEventCore(eventId, 2, descrs);
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, int arg1, int arg2, int arg3)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[3];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 4;
                descrs[1].DataPointer = (IntPtr)(&arg2);
                descrs[1].Size = 4;
                descrs[2].DataPointer = (IntPtr)(&arg3);
                descrs[2].Size = 4;
                WriteEventCore(eventId, 3, descrs);
            }
        }

        // optimized for common signatures (longs)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, long arg1)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[1];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 8;
                WriteEventCore(eventId, 1, descrs);
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, long arg1, long arg2)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 8;
                descrs[1].DataPointer = (IntPtr)(&arg2);
                descrs[1].Size = 8;
                WriteEventCore(eventId, 2, descrs);
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, long arg1, long arg2, long arg3)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[3];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 8;
                descrs[1].DataPointer = (IntPtr)(&arg2);
                descrs[1].Size = 8;
                descrs[2].DataPointer = (IntPtr)(&arg3);
                descrs[2].Size = 8;
                WriteEventCore(eventId, 3, descrs);
            }
        }

        // optimized for common signatures (strings)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, string arg1)
        {
            if (m_eventSourceEnabled)
            {
                if (arg1 == null) arg1 = "";
                fixed (char* string1Bytes = arg1)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[1];
                    descrs[0].DataPointer = (IntPtr)string1Bytes;
                    descrs[0].Size = ((arg1.Length + 1) * 2);
                    WriteEventCore(eventId, 1, descrs);
                }
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, string arg1, string arg2)
        {
            if (m_eventSourceEnabled)
            {
                if (arg1 == null) arg1 = "";
                if (arg2 == null) arg2 = "";
                fixed (char* string1Bytes = arg1)
                fixed (char* string2Bytes = arg2)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                    descrs[0].DataPointer = (IntPtr)string1Bytes;
                    descrs[0].Size = ((arg1.Length + 1) * 2);
                    descrs[1].DataPointer = (IntPtr)string2Bytes;
                    descrs[1].Size = ((arg2.Length + 1) * 2);
                    WriteEventCore(eventId, 2, descrs);
                }
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3)
        {
            if (m_eventSourceEnabled)
            {
                if (arg1 == null) arg1 = "";
                if (arg2 == null) arg2 = "";
                if (arg3 == null) arg3 = "";
                fixed (char* string1Bytes = arg1)
                fixed (char* string2Bytes = arg2)
                fixed (char* string3Bytes = arg3)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[3];
                    descrs[0].DataPointer = (IntPtr)string1Bytes;
                    descrs[0].Size = ((arg1.Length + 1) * 2);
                    descrs[1].DataPointer = (IntPtr)string2Bytes;
                    descrs[1].Size = ((arg2.Length + 1) * 2);
                    descrs[2].DataPointer = (IntPtr)string3Bytes;
                    descrs[2].Size = ((arg3.Length + 1) * 2);
                    WriteEventCore(eventId, 3, descrs);
                }
            }
        }

        // optimized for common signatures (string and ints)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, string arg1, int arg2)
        {
            if (m_eventSourceEnabled)
            {
                if (arg1 == null) arg1 = "";
                fixed (char* string1Bytes = arg1)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                    descrs[0].DataPointer = (IntPtr)string1Bytes;
                    descrs[0].Size = ((arg1.Length + 1) * 2);
                    descrs[1].DataPointer = (IntPtr)(&arg2);
                    descrs[1].Size = 4;
                    WriteEventCore(eventId, 2, descrs);
                }
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, string arg1, int arg2, int arg3)
        {
            if (m_eventSourceEnabled)
            {
                if (arg1 == null) arg1 = "";
                fixed (char* string1Bytes = arg1)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[3];
                    descrs[0].DataPointer = (IntPtr)string1Bytes;
                    descrs[0].Size = ((arg1.Length + 1) * 2);
                    descrs[1].DataPointer = (IntPtr)(&arg2);
                    descrs[1].Size = 4;
                    descrs[2].DataPointer = (IntPtr)(&arg3);
                    descrs[2].Size = 4;
                    WriteEventCore(eventId, 3, descrs);
                }
            }
        }

        // optimized for common signatures (string and longs)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, string arg1, long arg2)
        {
            if (m_eventSourceEnabled)
            {
                if (arg1 == null) arg1 = "";
                fixed (char* string1Bytes = arg1)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                    descrs[0].DataPointer = (IntPtr)string1Bytes;
                    descrs[0].Size = ((arg1.Length + 1) * 2);
                    descrs[1].DataPointer = (IntPtr)(&arg2);
                    descrs[1].Size = 8;
                    WriteEventCore(eventId, 2, descrs);
                }
            }
        }

        // optimized for common signatures (long and string)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, long arg1, string arg2)
        {
            if (m_eventSourceEnabled)
            {
                if (arg2 == null) arg2 = "";
                fixed (char* string2Bytes = arg2)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                    descrs[0].DataPointer = (IntPtr)(&arg1);
                    descrs[0].Size = 8;
                    descrs[1].DataPointer = (IntPtr)string2Bytes;
                    descrs[1].Size = ((arg2.Length + 1) * 2);
                    WriteEventCore(eventId, 2, descrs);
                }
            }
        }

        // optimized for common signatures (int and string)
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, int arg1, string arg2)
        {
            if (m_eventSourceEnabled)
            {
                if (arg2 == null) arg2 = "";
                fixed (char* string2Bytes = arg2)
                {
                    EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                    descrs[0].DataPointer = (IntPtr)(&arg1);
                    descrs[0].Size = 4;
                    descrs[1].DataPointer = (IntPtr)string2Bytes;
                    descrs[1].Size = ((arg2.Length + 1) * 2);
                    WriteEventCore(eventId, 2, descrs);
                }
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, byte[] arg1)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
                if (arg1 == null || arg1.Length == 0)
                {
                    int blobSize = 0;
                    descrs[0].DataPointer = (IntPtr)(&blobSize);
                    descrs[0].Size = 4;
                    descrs[1].DataPointer = (IntPtr)(&blobSize); // valid address instead of empty content 
                    descrs[1].Size = 0;
                    WriteEventCore(eventId, 2, descrs);
                }
                else
                {
                    int blobSize = arg1.Length;
                    fixed (byte* blob = &arg1[0])
                    {
                        descrs[0].DataPointer = (IntPtr)(&blobSize);
                        descrs[0].Size = 4;
                        descrs[1].DataPointer = (IntPtr)blob;
                        descrs[1].Size = blobSize;
                        WriteEventCore(eventId, 2, descrs);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, long arg1, byte[] arg2)
        {
            if (m_eventSourceEnabled)
            {
                EventSource.EventData* descrs = stackalloc EventSource.EventData[3];
                descrs[0].DataPointer = (IntPtr)(&arg1);
                descrs[0].Size = 8;
                if (arg2 == null || arg2.Length == 0)
                {
                    int blobSize = 0;
                    descrs[1].DataPointer = (IntPtr)(&blobSize);
                    descrs[1].Size = 4;
                    descrs[2].DataPointer = (IntPtr)(&blobSize); // valid address instead of empty contents 
                    descrs[2].Size = 0;
                    WriteEventCore(eventId, 3, descrs);
                }
                else
                {
                    int blobSize = arg2.Length;
                    fixed (byte* blob = &arg2[0])
                    {
                        descrs[1].DataPointer = (IntPtr)(&blobSize);
                        descrs[1].Size = 4;
                        descrs[2].DataPointer = (IntPtr)blob;
                        descrs[2].Size = blobSize;
                        WriteEventCore(eventId, 3, descrs);
                    }
                }
            }
        }

#pragma warning restore 1591

        /// <summary>
        /// Used to construct the data structure to be passed to the native ETW APIs - EventWrite and EventWriteTransfer.
        /// </summary>
        protected internal struct EventData
        {
            /// <summary>
            /// Address where the one argument lives (if this points to managed memory you must ensure the
            /// managed object is pinned.
            /// </summary>
            public IntPtr DataPointer { get { return (IntPtr)m_Ptr; } set { m_Ptr = unchecked((long)value); } }
            /// <summary>
            /// Size of the argument referenced by DataPointer
            /// </summary>
            public int Size { get { return m_Size; } set { m_Size = value; } }

            #region private
            /// <summary>
            /// Initializes the members of this EventData object to point at a previously-pinned
            /// tracelogging-compatible metadata blob.
            /// </summary>
            /// <param name="pointer">Pinned tracelogging-compatible metadata blob.</param>
            /// <param name="size">The size of the metadata blob.</param>
            /// <param name="reserved">Value for reserved: 2 for per-provider metadata, 1 for per-event metadata</param>
            internal unsafe void SetMetadata(byte* pointer, int size, int reserved)
            {
                this.m_Ptr = (long)(ulong)(UIntPtr)pointer;
                this.m_Size = size;
                this.m_Reserved = reserved; // Mark this descriptor as containing tracelogging-compatible metadata.
            }

            //Important, we pass this structure directly to the Win32 EventWrite API, so this structure must be layed out exactly
            // the way EventWrite wants it.  
            internal long m_Ptr;
            internal int m_Size;
#pragma warning disable 0649
            internal int m_Reserved;       // Used to pad the size to match the Win32 API
#pragma warning restore 0649
            #endregion
        }

        /// <summary>
        /// This routine allows you to create efficient WriteEvent helpers, however the code that you use to
        /// do this, while straightforward, is unsafe.
        /// </summary>
        /// <remarks>
        /// <code>
        ///    protected unsafe void WriteEvent(int eventId, string arg1, long arg2)
        ///    {
        ///        if (IsEnabled())
        ///        {
        ///            if (arg2 == null) arg2 = "";
        ///            fixed (char* string2Bytes = arg2)
        ///            {
        ///                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
        ///                descrs[0].DataPointer = (IntPtr)(&amp;arg1);
        ///                descrs[0].Size = 8;
        ///                descrs[1].DataPointer = (IntPtr)string2Bytes;
        ///                descrs[1].Size = ((arg2.Length + 1) * 2);
        ///                WriteEventCore(eventId, 2, descrs);
        ///            }
        ///        }
        ///    }
        /// </code>
        /// </remarks>
        [CLSCompliant(false)]
        protected unsafe void WriteEventCore(int eventId, int eventDataCount, EventSource.EventData* data)
        {
            WriteEventWithRelatedActivityIdCore(eventId, null, eventDataCount, data);
        }

        /// <summary>
        /// This routine allows you to create efficient WriteEventWithRelatedActivityId helpers, however the code 
        /// that you use to do this, while straightforward, is unsafe. The only difference from
        /// <see cref="WriteEventCore"/> is that you pass the relatedActivityId from caller through to this API
        /// </summary>
        /// <remarks>
        /// <code>
        ///    protected unsafe void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, string arg1, long arg2)
        ///    {
        ///        if (IsEnabled())
        ///        {
        ///            if (arg2 == null) arg2 = "";
        ///            fixed (char* string2Bytes = arg2)
        ///            {
        ///                EventSource.EventData* descrs = stackalloc EventSource.EventData[2];
        ///                descrs[0].DataPointer = (IntPtr)(&amp;arg1);
        ///                descrs[0].Size = 8;
        ///                descrs[1].DataPointer = (IntPtr)string2Bytes;
        ///                descrs[1].Size = ((arg2.Length + 1) * 2);
        ///                WriteEventWithRelatedActivityIdCore(eventId, relatedActivityId, 2, descrs);
        ///            }
        ///        }
        ///    }
        /// </code>
        /// </remarks>
        [CLSCompliant(false)]
        protected unsafe void WriteEventWithRelatedActivityIdCore(int eventId, Guid* relatedActivityId, int eventDataCount, EventSource.EventData* data)
        {
            if (m_eventSourceEnabled)
            {
                try
                {
                    Debug.Assert(m_eventData != null);  // You must have initialized this if you enabled the source.
                    if (relatedActivityId != null)
                        ValidateEventOpcodeForTransfer(ref m_eventData[eventId], m_eventData[eventId].Name);

                    EventOpcode opcode = (EventOpcode)m_eventData[eventId].Descriptor.Opcode;
                    EventActivityOptions activityOptions = m_eventData[eventId].ActivityOptions;
                    Guid* pActivityId = null;
                    Guid activityId = Guid.Empty;
                    Guid relActivityId = Guid.Empty;

                    if (opcode != EventOpcode.Info && relatedActivityId == null &&
                       ((activityOptions & EventActivityOptions.Disable) == 0))
                    {
                        if (opcode == EventOpcode.Start)
                        {
                            m_activityTracker.OnStart(m_name, m_eventData[eventId].Name, m_eventData[eventId].Descriptor.Task, ref activityId, ref relActivityId, m_eventData[eventId].ActivityOptions);
                        }
                        else if (opcode == EventOpcode.Stop)
                        {
                            m_activityTracker.OnStop(m_name, m_eventData[eventId].Name, m_eventData[eventId].Descriptor.Task, ref activityId);
                        }

                        if (activityId != Guid.Empty)
                            pActivityId = &activityId;
                        if (relActivityId != Guid.Empty)
                            relatedActivityId = &relActivityId;
                    }

#if FEATURE_MANAGED_ETW
                    if (m_eventData[eventId].EnabledForETW)
                    {

#if FEATURE_ACTIVITYSAMPLING
                        // this code should be kept in sync with WriteEventVarargs().
                        SessionMask etwSessions = SessionMask.All;
                        // only compute etwSessions if there are *any* ETW filters enabled...
                        if ((ulong)m_curLiveSessions != 0)
                            etwSessions = GetEtwSessionMask(eventId, relatedActivityId);
                        // OutputDebugString(string.Format("{0}.WriteEvent(id {1}) -> to sessions {2:x}", 
                        //                   m_name, m_eventData[eventId].Name, (ulong) etwSessions));

                        if ((ulong)etwSessions != 0 || m_legacySessions != null && m_legacySessions.Count > 0)
                        {
                            if (!SelfDescribingEvents)
                            {
                                if (etwSessions.IsEqualOrSupersetOf(m_curLiveSessions))
                                {
                                    // OutputDebugString(string.Format("  (1) id {0}, kwd {1:x}", 
                                    //                   m_eventData[eventId].Name, m_eventData[eventId].Descriptor.Keywords));
                                    // by default the Descriptor.Keyword will have the perEventSourceSessionId bit 
                                    // mask set to 0x0f so, when all ETW sessions want the event we don't need to 
                                    // synthesize a new one
                                    if (!m_provider.WriteEvent(ref m_eventData[eventId].Descriptor, m_eventData[eventId].EventHandle, pActivityId, relatedActivityId, eventDataCount, (IntPtr)data))
                                        ThrowEventSourceException(m_eventData[eventId].Name);
                                }
                                else
                                {
                                    long origKwd = unchecked((long)((ulong)m_eventData[eventId].Descriptor.Keywords & ~(SessionMask.All.ToEventKeywords())));
                                    // OutputDebugString(string.Format("  (2) id {0}, kwd {1:x}", 
                                    //                   m_eventData[eventId].Name, etwSessions.ToEventKeywords() | (ulong) origKwd));
                                    // only some of the ETW sessions will receive this event. Synthesize a new
                                    // Descriptor whose Keywords field will have the appropriate bits set.
                                    // etwSessions might be 0, if there are legacy ETW listeners that want this event
                                    var desc = new EventDescriptor(
                                        m_eventData[eventId].Descriptor.EventId,
                                        m_eventData[eventId].Descriptor.Version,
                                        m_eventData[eventId].Descriptor.Channel,
                                        m_eventData[eventId].Descriptor.Level,
                                        m_eventData[eventId].Descriptor.Opcode,
                                        m_eventData[eventId].Descriptor.Task,
                                        unchecked((long)etwSessions.ToEventKeywords() | origKwd));

                                    if (!m_provider.WriteEvent(ref desc, m_eventData[eventId].EventHandle, pActivityId, relatedActivityId, eventDataCount, (IntPtr)data))
                                        ThrowEventSourceException(m_eventData[eventId].Name);
                                }
                            }
                            else
                            {
                                TraceLoggingEventTypes tlet = m_eventData[eventId].TraceLoggingEventTypes;
                                if (tlet == null)
                                {
                                    tlet = new TraceLoggingEventTypes(m_eventData[eventId].Name,
                                                                            EventTags.None,
                                                                        m_eventData[eventId].Parameters);
                                    Interlocked.CompareExchange(ref m_eventData[eventId].TraceLoggingEventTypes, tlet, null);

                                }
                                long origKwd = unchecked((long)((ulong)m_eventData[eventId].Descriptor.Keywords & ~(SessionMask.All.ToEventKeywords())));
                                // TODO: activity ID support
                                EventSourceOptions opt = new EventSourceOptions
                                {
                                    Keywords = (EventKeywords)unchecked((long)etwSessions.ToEventKeywords() | origKwd),
                                    Level = (EventLevel)m_eventData[eventId].Descriptor.Level,
                                    Opcode = (EventOpcode)m_eventData[eventId].Descriptor.Opcode
                                };

                                WriteMultiMerge(m_eventData[eventId].Name, ref opt, tlet, pActivityId, relatedActivityId, data);
                            }
                        }
#else
                        if (!SelfDescribingEvents)
                        {
                            if (!m_provider.WriteEvent(ref m_eventData[eventId].Descriptor, m_eventData[eventId].EventHandle, pActivityId, relatedActivityId, eventDataCount, (IntPtr)data))
                                ThrowEventSourceException(m_eventData[eventId].Name);
                        }
                        else
                        {
                            TraceLoggingEventTypes tlet = m_eventData[eventId].TraceLoggingEventTypes;
                            if (tlet == null)
                            {
                                tlet = new TraceLoggingEventTypes(m_eventData[eventId].Name,
                                                                    m_eventData[eventId].Tags,
                                                                    m_eventData[eventId].Parameters);
                                Interlocked.CompareExchange(ref m_eventData[eventId].TraceLoggingEventTypes, tlet, null);

                            }
                            EventSourceOptions opt = new EventSourceOptions
                            {
                                Keywords = (EventKeywords)m_eventData[eventId].Descriptor.Keywords,
                                Level = (EventLevel)m_eventData[eventId].Descriptor.Level,
                                Opcode = (EventOpcode)m_eventData[eventId].Descriptor.Opcode
                            };

                            WriteMultiMerge(m_eventData[eventId].Name, ref opt, tlet, pActivityId, relatedActivityId, data);
                        }
#endif // FEATURE_ACTIVITYSAMPLING
                    }
#endif // FEATURE_MANAGED_ETW

                    if (m_Dispatchers != null && m_eventData[eventId].EnabledForAnyListener)
                        WriteToAllListeners(eventId, relatedActivityId, eventDataCount, data);
                }
                catch (Exception ex)
                {
                    if (ex is EventSourceException)
                        throw;
                    else
                        ThrowEventSourceException(m_eventData[eventId].Name, ex);
                }
            }
        }

        // fallback varags helpers. 
        /// <summary>
        /// This is the varargs helper for writing an event. It does create an array and box all the arguments so it is
        /// relatively inefficient and should only be used for relatively rare events (e.g. less than 100 / sec). If your
        /// rates are faster than that you should use <see cref="WriteEventCore"/> to create fast helpers for your particular 
        /// method signature. Even if you use this for rare events, this call should be guarded by an <see cref="IsEnabled()"/> 
        /// check so that the varargs call is not made when the EventSource is not active.  
        /// </summary>
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        protected unsafe void WriteEvent(int eventId, params object[] args)
        {
            WriteEventVarargs(eventId, null, args);
        }

        /// <summary>
        /// This is the varargs helper for writing an event which also specifies a related activity. It is completely analogous
        /// to corresponding WriteEvent (they share implementation). It does create an array and box all the arguments so it is
        /// relatively inefficient and should only be used for relatively rare events (e.g. less than 100 / sec).  If your
        /// rates are faster than that you should use <see cref="WriteEventWithRelatedActivityIdCore"/> to create fast helpers for your 
        /// particular method signature. Even if you use this for rare events, this call should be guarded by an <see cref="IsEnabled()"/>
        /// check so that the varargs call is not made when the EventSource is not active.
        /// </summary>
        protected unsafe void WriteEventWithRelatedActivityId(int eventId, Guid relatedActivityId, params object[] args)
        {
            WriteEventVarargs(eventId, &relatedActivityId, args);
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Disposes of an EventSource.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Disposes of an EventSource.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true, and from the finalizer (~EventSource) with disposing=false.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#if FEATURE_MANAGED_ETW
#if !FEATURE_PERFTRACING
                // Send the manifest one more time to ensure circular buffers have a chance to get to this information
                // even in scenarios with a high volume of ETW events.
                if (m_eventSourceEnabled)
                {
                    try
                    {
                        SendManifest(m_rawManifest);
                    }
                    catch (Exception)
                    { }           // If it fails, simply give up.   
                    m_eventSourceEnabled = false;
                }
#endif
                if (m_provider != null)
                {
                    m_provider.Dispose();
                    m_provider = null;
                }
#endif
            }
            m_eventSourceEnabled = false;
            m_eventSourceDisposed = true;
        }
        /// <summary>
        /// Finalizer for EventSource
        /// </summary>
        ~EventSource()
        {
            this.Dispose(false);
        }
        #endregion

        #region private
#if FEATURE_ACTIVITYSAMPLING
        internal void WriteStringToListener(EventListener listener, string msg, SessionMask m)
        {
            Debug.Assert(listener == null || (uint)m == (uint)SessionMask.FromId(0));

            if (m_eventSourceEnabled)
            {
                if (listener == null)
                {
                    WriteEventString(0, unchecked((long)m.ToEventKeywords()), msg);
                }
                else
                {
                    EventWrittenEventArgs eventCallbackArgs = new EventWrittenEventArgs(this);
                    eventCallbackArgs.EventId = 0;
                    eventCallbackArgs.Message = msg;
                    eventCallbackArgs.Payload = new ReadOnlyCollection<object>(new List<object>() { msg });
                    eventCallbackArgs.PayloadNames = new ReadOnlyCollection<string>(new List<string> { "message" });
                    eventCallbackArgs.EventName = "EventSourceMessage";
                    listener.OnEventWritten(eventCallbackArgs);
                }
            }
        }
#endif

        private unsafe void WriteEventRaw(
            string eventName,
            ref EventDescriptor eventDescriptor,
            Guid* activityID,
            Guid* relatedActivityID,
            int dataCount,
            IntPtr data)
        {
#if FEATURE_MANAGED_ETW
            if (m_provider == null)
            {
                ThrowEventSourceException(eventName);
            }
            else
            {
                if (!m_provider.WriteEventRaw(ref eventDescriptor, activityID, relatedActivityID, dataCount, data))
                    ThrowEventSourceException(eventName);
            }
#endif // FEATURE_MANAGED_ETW
        }

        // FrameworkEventSource is on the startup path for the framework, so we have this internal overload that it can use
        // to prevent the working set hit from looking at the custom attributes on the type to get the Guid.
        internal EventSource(Guid eventSourceGuid, string eventSourceName)
            : this(eventSourceGuid, eventSourceName, EventSourceSettings.EtwManifestEventFormat)
        { }

        // Used by the internal FrameworkEventSource constructor and the TraceLogging-style event source constructor
        internal EventSource(Guid eventSourceGuid, string eventSourceName, EventSourceSettings settings, string[] traits = null)
        {
            m_config = ValidateSettings(settings);
            Initialize(eventSourceGuid, eventSourceName, traits);
        }

        /// <summary>
        /// This method is responsible for the common initialization path from our constructors. It must
        /// not leak any exceptions (otherwise, since most EventSource classes define a static member, 
        /// "Log", such an exception would become a cached exception for the initialization of the static
        /// member, and any future access to the "Log" would throw the cached exception).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        private unsafe void Initialize(Guid eventSourceGuid, string eventSourceName, string[] traits)
        {
            try
            {
                m_traits = traits;
                if (m_traits != null && m_traits.Length % 2 != 0)
                {
                    throw new ArgumentException(Resources.GetResourceString("TraitEven"), nameof(traits));
                }

                if (eventSourceGuid == Guid.Empty)
                {
                    throw new ArgumentException(Resources.GetResourceString("EventSource_NeedGuid"));
                }

                if (eventSourceName == null)
                {
                    throw new ArgumentException(Resources.GetResourceString("EventSource_NeedName"));
                }

                m_name = eventSourceName;
                m_guid = eventSourceGuid;
#if FEATURE_ACTIVITYSAMPLING
                m_curLiveSessions = new SessionMask(0);
                m_etwSessionIdMap = new EtwSession[SessionMask.MAX];
#endif // FEATURE_ACTIVITYSAMPLING

                //Enable Implicit Activity tracker
                m_activityTracker = ActivityTracker.Instance;

#if FEATURE_MANAGED_ETW
                // Create and register our provider traits.  We do this early because it is needed to log errors 
                // In the self-describing event case. 
                this.InitializeProviderMetadata();

                // Register the provider with ETW
                var provider = new OverideEventProvider(this);
                provider.Register(eventSourceGuid);
#endif
                // Add the eventSource to the global (weak) list.  
                // This also sets m_id, which is the index in the list. 
                EventListener.AddEventSource(this);

#if FEATURE_MANAGED_ETW
                // OK if we get this far without an exception, then we can at least write out error messages. 
                // Set m_provider, which allows this.  
                m_provider = provider;

#if PLATFORM_WINDOWS
#if (!ES_BUILD_STANDALONE && !ES_BUILD_PN)
                // API available on OS >= Win 8 and patched Win 7.
                // Disable only for FrameworkEventSource to avoid recursion inside exception handling.
                if (this.Name != "System.Diagnostics.Eventing.FrameworkEventSource" || Environment.IsWindows8OrAbove)
#endif
                {
                    int setInformationResult;
                    System.Runtime.InteropServices.GCHandle metadataHandle =
                        System.Runtime.InteropServices.GCHandle.Alloc(this.providerMetadata, System.Runtime.InteropServices.GCHandleType.Pinned);
                    IntPtr providerMetadata = metadataHandle.AddrOfPinnedObject();

                    setInformationResult = m_provider.SetInformation(
                        UnsafeNativeMethods.ManifestEtw.EVENT_INFO_CLASS.SetTraits,
                        providerMetadata,
                        (uint)this.providerMetadata.Length);

                    metadataHandle.Free();
                }
#endif // PLATFORM_WINDOWS
#endif // FEATURE_MANAGED_ETW
                Debug.Assert(!m_eventSourceEnabled);     // We can't be enabled until we are completely initted.  
                // We are logically completely initialized at this point.  
                m_completelyInited = true;
            }
            catch (Exception e)
            {
                if (m_constructionException == null)
                    m_constructionException = e;
                ReportOutOfBandMessage("ERROR: Exception during construction of EventSource " + Name + ": " + e.Message, true);
            }

            // Once m_completelyInited is set, you can have concurrency, so all work is under the lock.  
            lock (EventListener.EventListenersLock)
            {
                // If there are any deferred commands, we can do them now.   
                // This is the most likely place for exceptions to happen.  
                // Note that we are NOT resetting m_deferredCommands to NULL here, 
                // We are giving for EventHandler<EventCommandEventArgs> that will be attached later
                EventCommandEventArgs deferredCommands = m_deferredCommands;
                while (deferredCommands != null)
                {
                    DoCommand(deferredCommands);      // This can never throw, it catches them and reports the errors.   
                    deferredCommands = deferredCommands.nextCommand;
                }
            }
        }

        private static string GetName(Type eventSourceType, EventManifestOptions flags)
        {
            if (eventSourceType == null)
                throw new ArgumentNullException(nameof(eventSourceType));
            Contract.EndContractBlock();

            EventSourceAttribute attrib = (EventSourceAttribute)GetCustomAttributeHelper(eventSourceType, typeof(EventSourceAttribute), flags);
            if (attrib != null && attrib.Name != null)
                return attrib.Name;

            return eventSourceType.Name;
        }

        /// <summary>
        /// Implements the SHA1 hashing algorithm. Note that this
        /// implementation is for hashing public information. Do not
        /// use this code to hash private data, as this implementation does
        /// not take any steps to avoid information disclosure.
        /// </summary>
        private struct Sha1ForNonSecretPurposes
        {
            private long length; // Total message length in bits
            private uint[] w; // Workspace
            private int pos; // Length of current chunk in bytes

            /// <summary>
            /// Call Start() to initialize the hash object.
            /// </summary>
            public void Start()
            {
                if (this.w == null)
                {
                    this.w = new uint[85];
                }

                this.length = 0;
                this.pos = 0;
                this.w[80] = 0x67452301;
                this.w[81] = 0xEFCDAB89;
                this.w[82] = 0x98BADCFE;
                this.w[83] = 0x10325476;
                this.w[84] = 0xC3D2E1F0;
            }

            /// <summary>
            /// Adds an input byte to the hash.
            /// </summary>
            /// <param name="input">Data to include in the hash.</param>
            public void Append(byte input)
            {
                this.w[this.pos / 4] = (this.w[this.pos / 4] << 8) | input;
                if (64 == ++this.pos)
                {
                    this.Drain();
                }
            }

            /// <summary>
            /// Adds input bytes to the hash.
            /// </summary>
            /// <param name="input">
            /// Data to include in the hash. Must not be null.
            /// </param>
            public void Append(byte[] input)
            {
                foreach (var b in input)
                {
                    this.Append(b);
                }
            }

            /// <summary>
            /// Retrieves the hash value.
            /// Note that after calling this function, the hash object should
            /// be considered uninitialized. Subsequent calls to Append or
            /// Finish will produce useless results. Call Start() to
            /// reinitialize.
            /// </summary>
            /// <param name="output">
            /// Buffer to receive the hash value. Must not be null.
            /// Up to 20 bytes of hash will be written to the output buffer.
            /// If the buffer is smaller than 20 bytes, the remaining hash
            /// bytes will be lost. If the buffer is larger than 20 bytes, the
            /// rest of the buffer is left unmodified.
            /// </param>
            public void Finish(byte[] output)
            {
                long l = this.length + 8 * this.pos;
                this.Append(0x80);
                while (this.pos != 56)
                {
                    this.Append(0x00);
                }

                unchecked
                {
                    this.Append((byte)(l >> 56));
                    this.Append((byte)(l >> 48));
                    this.Append((byte)(l >> 40));
                    this.Append((byte)(l >> 32));
                    this.Append((byte)(l >> 24));
                    this.Append((byte)(l >> 16));
                    this.Append((byte)(l >> 8));
                    this.Append((byte)l);

                    int end = output.Length < 20 ? output.Length : 20;
                    for (int i = 0; i != end; i++)
                    {
                        uint temp = this.w[80 + i / 4];
                        output[i] = (byte)(temp >> 24);
                        this.w[80 + i / 4] = temp << 8;
                    }
                }
            }

            /// <summary>
            /// Called when this.pos reaches 64.
            /// </summary>
            private void Drain()
            {
                for (int i = 16; i != 80; i++)
                {
                    this.w[i] = Rol1((this.w[i - 3] ^ this.w[i - 8] ^ this.w[i - 14] ^ this.w[i - 16]));
                }

                unchecked
                {
                    uint a = this.w[80];
                    uint b = this.w[81];
                    uint c = this.w[82];
                    uint d = this.w[83];
                    uint e = this.w[84];

                    for (int i = 0; i != 20; i++)
                    {
                        const uint k = 0x5A827999;
                        uint f = (b & c) | ((~b) & d);
                        uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                    }

                    for (int i = 20; i != 40; i++)
                    {
                        uint f = b ^ c ^ d;
                        const uint k = 0x6ED9EBA1;
                        uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                    }

                    for (int i = 40; i != 60; i++)
                    {
                        uint f = (b & c) | (b & d) | (c & d);
                        const uint k = 0x8F1BBCDC;
                        uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                    }

                    for (int i = 60; i != 80; i++)
                    {
                        uint f = b ^ c ^ d;
                        const uint k = 0xCA62C1D6;
                        uint temp = Rol5(a) + f + e + k + this.w[i]; e = d; d = c; c = Rol30(b); b = a; a = temp;
                    }

                    this.w[80] += a;
                    this.w[81] += b;
                    this.w[82] += c;
                    this.w[83] += d;
                    this.w[84] += e;
                }

                this.length += 512; // 64 bytes == 512 bits
                this.pos = 0;
            }

            private static uint Rol1(uint input)
            {
                return (input << 1) | (input >> 31);
            }

            private static uint Rol5(uint input)
            {
                return (input << 5) | (input >> 27);
            }

            private static uint Rol30(uint input)
            {
                return (input << 30) | (input >> 2);
            }
        }

        private static Guid GenerateGuidFromName(string name)
        {
            byte[] bytes = Encoding.BigEndianUnicode.GetBytes(name);
            var hash = new Sha1ForNonSecretPurposes();
            hash.Start();
            hash.Append(namespaceBytes);
            hash.Append(bytes);
            Array.Resize(ref bytes, 16);
            hash.Finish(bytes);

            bytes[7] = unchecked((byte)((bytes[7] & 0x0F) | 0x50));    // Set high 4 bits of octet 7 to 5, as per RFC 4122
            return new Guid(bytes);
        }

        private unsafe object DecodeObject(int eventId, int parameterId, ref EventSource.EventData* data)
        {
            // TODO FIX : We use reflection which in turn uses EventSource, right now we carefully avoid
            // the recursion, but can we do this in a robust way?  

            IntPtr dataPointer = data->DataPointer;
            // advance to next EventData in array
            ++data;

            Type dataType = GetDataType(m_eventData[eventId], parameterId);

            Again:
            if (dataType == typeof(IntPtr))
            {
                return *((IntPtr*)dataPointer);
            }
            else if (dataType == typeof(int))
            {
                return *((int*)dataPointer);
            }
            else if (dataType == typeof(uint))
            {
                return *((uint*)dataPointer);
            }
            else if (dataType == typeof(long))
            {
                return *((long*)dataPointer);
            }
            else if (dataType == typeof(ulong))
            {
                return *((ulong*)dataPointer);
            }
            else if (dataType == typeof(byte))
            {
                return *((byte*)dataPointer);
            }
            else if (dataType == typeof(sbyte))
            {
                return *((sbyte*)dataPointer);
            }
            else if (dataType == typeof(short))
            {
                return *((short*)dataPointer);
            }
            else if (dataType == typeof(ushort))
            {
                return *((ushort*)dataPointer);
            }
            else if (dataType == typeof(float))
            {
                return *((float*)dataPointer);
            }
            else if (dataType == typeof(double))
            {
                return *((double*)dataPointer);
            }
            else if (dataType == typeof(decimal))
            {
                return *((decimal*)dataPointer);
            }
            else if (dataType == typeof(bool))
            {
                // The manifest defines a bool as a 32bit type (WIN32 BOOL), not 1 bit as CLR Does.
                if (*((int*)dataPointer) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (dataType == typeof(Guid))
            {
                return *((Guid*)dataPointer);
            }
            else if (dataType == typeof(char))
            {
                return *((char*)dataPointer);
            }
            else if (dataType == typeof(DateTime))
            {
                long dateTimeTicks = *((long*)dataPointer);
                return DateTime.FromFileTimeUtc(dateTimeTicks);
            }
            else if (dataType == typeof(byte[]))
            {
                // byte[] are written to EventData* as an int followed by a blob
                int cbSize = *((int*)dataPointer);
                byte[] blob = new byte[cbSize];
                dataPointer = data->DataPointer;
                data++;
                for (int i = 0; i < cbSize; ++i)
                    blob[i] = *((byte*)(dataPointer + i));
                return blob;
            }
            else if (dataType == typeof(byte*))
            {
                // TODO: how do we want to handle this? For now we ignore it...
                return null;
            }
            else
            {
                if (m_EventSourcePreventRecursion && m_EventSourceInDecodeObject)
                {
                    return null;
                }

                try
                {
                    m_EventSourceInDecodeObject = true;

                    if (dataType.IsEnum())
                    {
                        dataType = Enum.GetUnderlyingType(dataType);
                        goto Again;
                    }


                    // Everything else is marshaled as a string.
                    // ETW strings are NULL-terminated, so marshal everything up to the first
                    // null in the string.
                    //return System.Runtime.InteropServices.Marshal.PtrToStringUni(dataPointer);
                    if(dataPointer == IntPtr.Zero)
                    {
                        return null;
                    }

                    return new string((char *)dataPointer);

                }
                finally
                {
                    m_EventSourceInDecodeObject = false;
                }
            }
        }

        // Finds the Dispatcher (which holds the filtering state), for a given dispatcher for the current
        // eventSource).  
        private EventDispatcher GetDispatcher(EventListener listener)
        {
            EventDispatcher dispatcher = m_Dispatchers;
            while (dispatcher != null)
            {
                if (dispatcher.m_Listener == listener)
                    return dispatcher;
                dispatcher = dispatcher.m_Next;
            }
            return dispatcher;
        }

        private unsafe void WriteEventVarargs(int eventId, Guid* childActivityID, object[] args)
        {
            if (m_eventSourceEnabled)
            {
                try
                {
                    Debug.Assert(m_eventData != null);  // You must have initialized this if you enabled the source.  
                    if (childActivityID != null)
                    {
                        ValidateEventOpcodeForTransfer(ref m_eventData[eventId], m_eventData[eventId].Name);

                        // If you use WriteEventWithRelatedActivityID you MUST declare the first argument to be a GUID 
                        // with the name 'relatedActivityID, and NOT pass this argument to the WriteEvent method.  
                        // During manifest creation we modify the ParameterInfo[] that we store to strip out any
                        // first parameter that is of type Guid and named "relatedActivityId." Thus, if you call
                        // WriteEventWithRelatedActivityID from a method that doesn't name its first parameter correctly
                        // we can end up in a state where the ParameterInfo[] doesn't have its first parameter stripped,
                        // and this leads to a mismatch between the number of arguments and the number of ParameterInfos,
                        // which would cause a cryptic IndexOutOfRangeException later if we don't catch it here.
                        if (!m_eventData[eventId].HasRelatedActivityID)
                        {
                            throw new ArgumentException(Resources.GetResourceString("EventSource_NoRelatedActivityId"));
                        }
                    }

                    LogEventArgsMismatches(m_eventData[eventId].Parameters, args);

                    Guid* pActivityId = null;
                    Guid activityId = Guid.Empty;
                    Guid relatedActivityId = Guid.Empty;
                    EventOpcode opcode = (EventOpcode)m_eventData[eventId].Descriptor.Opcode;
                    EventActivityOptions activityOptions = m_eventData[eventId].ActivityOptions;

                    if (childActivityID == null &&
                       ((activityOptions & EventActivityOptions.Disable) == 0))
                    {
                        if (opcode == EventOpcode.Start)
                        {
                            m_activityTracker.OnStart(m_name, m_eventData[eventId].Name, m_eventData[eventId].Descriptor.Task, ref activityId, ref relatedActivityId, m_eventData[eventId].ActivityOptions);
                        }
                        else if (opcode == EventOpcode.Stop)
                        {
                            m_activityTracker.OnStop(m_name, m_eventData[eventId].Name, m_eventData[eventId].Descriptor.Task, ref activityId);
                        }

                        if (activityId != Guid.Empty)
                            pActivityId = &activityId;
                        if (relatedActivityId != Guid.Empty)
                            childActivityID = &relatedActivityId;
                    }

#if FEATURE_MANAGED_ETW
                    if (m_eventData[eventId].EnabledForETW)
                    {
#if FEATURE_ACTIVITYSAMPLING
                        // this code should be kept in sync with WriteEventWithRelatedActivityIdCore().
                        SessionMask etwSessions = SessionMask.All;
                        // only compute etwSessions if there are *any* ETW filters enabled...
                        if ((ulong)m_curLiveSessions != 0)
                            etwSessions = GetEtwSessionMask(eventId, childActivityID);

                        if ((ulong)etwSessions != 0 || m_legacySessions != null && m_legacySessions.Count > 0)
                        {
                            if (!SelfDescribingEvents)
                            {
                                if (etwSessions.IsEqualOrSupersetOf(m_curLiveSessions))
                                {
                                    // by default the Descriptor.Keyword will have the perEventSourceSessionId bit 
                                    // mask set to 0x0f so, when all ETW sessions want the event we don't need to 
                                    // synthesize a new one
                                    if (!m_provider.WriteEvent(ref m_eventData[eventId].Descriptor, m_eventData[eventId].EventHandle, pActivityId, childActivityID, args))
                                        ThrowEventSourceException(m_eventData[eventId].Name);
                                }
                                else
                                {
                                    long origKwd = unchecked((long)((ulong)m_eventData[eventId].Descriptor.Keywords & ~(SessionMask.All.ToEventKeywords())));
                                    // only some of the ETW sessions will receive this event. Synthesize a new
                                    // Descriptor whose Keywords field will have the appropriate bits set.
                                    var desc = new EventDescriptor(
                                        m_eventData[eventId].Descriptor.EventId,
                                        m_eventData[eventId].Descriptor.Version,
                                        m_eventData[eventId].Descriptor.Channel,
                                        m_eventData[eventId].Descriptor.Level,
                                        m_eventData[eventId].Descriptor.Opcode,
                                        m_eventData[eventId].Descriptor.Task,
                                        unchecked((long)etwSessions.ToEventKeywords() | origKwd));

                                    if (!m_provider.WriteEvent(ref desc, m_eventData[eventId].EventHandle, pActivityId, childActivityID, args))
                                        ThrowEventSourceException(m_eventData[eventId].Name);
                                }
                            }
                            else
                            {
                                TraceLoggingEventTypes tlet = m_eventData[eventId].TraceLoggingEventTypes;
                                if (tlet == null)
                                {
                                    tlet = new TraceLoggingEventTypes(m_eventData[eventId].Name,
                                                                        EventTags.None,
                                                                        m_eventData[eventId].Parameters);
                                    Interlocked.CompareExchange(ref m_eventData[eventId].TraceLoggingEventTypes, tlet, null);

                                }
                                long origKwd = unchecked((long)((ulong)m_eventData[eventId].Descriptor.Keywords & ~(SessionMask.All.ToEventKeywords())));
                                // TODO: activity ID support
                                EventSourceOptions opt = new EventSourceOptions
                                {
                                    Keywords = (EventKeywords)unchecked((long)etwSessions.ToEventKeywords() | origKwd),
                                    Level = (EventLevel)m_eventData[eventId].Descriptor.Level,
                                    Opcode = (EventOpcode)m_eventData[eventId].Descriptor.Opcode
                                };

                                WriteMultiMerge(m_eventData[eventId].Name, ref opt, tlet, pActivityId, childActivityID, args);
                            }
                        }
#else
                        if (!SelfDescribingEvents)
                        {
                            if (!m_provider.WriteEvent(ref m_eventData[eventId].Descriptor, m_eventData[eventId].EventHandle, pActivityId, childActivityID, args))
                                ThrowEventSourceException(m_eventData[eventId].Name);
                        }
                        else
                        {
                            TraceLoggingEventTypes tlet = m_eventData[eventId].TraceLoggingEventTypes;
                            if (tlet == null)
                            {
                                tlet = new TraceLoggingEventTypes(m_eventData[eventId].Name,
                                                                    EventTags.None,
                                                                    m_eventData[eventId].Parameters);
                                Interlocked.CompareExchange(ref m_eventData[eventId].TraceLoggingEventTypes, tlet, null);

                            }
                            // TODO: activity ID support
                            EventSourceOptions opt = new EventSourceOptions
                            {
                                Keywords = (EventKeywords)m_eventData[eventId].Descriptor.Keywords,
                                Level = (EventLevel)m_eventData[eventId].Descriptor.Level,
                                Opcode = (EventOpcode)m_eventData[eventId].Descriptor.Opcode
                            };

                            WriteMultiMerge(m_eventData[eventId].Name, ref opt, tlet, pActivityId, childActivityID, args);
                        }
#endif // FEATURE_ACTIVITYSAMPLING
                    }
#endif // FEATURE_MANAGED_ETW
                    if (m_Dispatchers != null && m_eventData[eventId].EnabledForAnyListener)
                    {
#if (!ES_BUILD_STANDALONE && !ES_BUILD_PN)
                        // Maintain old behavior - object identity is preserved
                        if (AppContextSwitches.PreserveEventListnerObjectIdentity)
                        {
                            WriteToAllListeners(eventId, childActivityID, args);
                        }
                        else
#endif // !ES_BUILD_STANDALONE
                        {
                            object[] serializedArgs = SerializeEventArgs(eventId, args);
                            WriteToAllListeners(eventId, childActivityID, serializedArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is EventSourceException)
                        throw;
                    else
                        ThrowEventSourceException(m_eventData[eventId].Name, ex);
                }
            }
        }

        unsafe private object[] SerializeEventArgs(int eventId, object[] args)
        {
            TraceLoggingEventTypes eventTypes = m_eventData[eventId].TraceLoggingEventTypes;
            if (eventTypes == null)
            {
                eventTypes = new TraceLoggingEventTypes(m_eventData[eventId].Name,
                                                        EventTags.None,
                                                        m_eventData[eventId].Parameters);
                Interlocked.CompareExchange(ref m_eventData[eventId].TraceLoggingEventTypes, eventTypes, null);
            }
            var eventData = new object[eventTypes.typeInfos.Length];
            for (int i = 0; i < eventTypes.typeInfos.Length; i++)
            {
                eventData[i] = eventTypes.typeInfos[i].GetData(args[i]);
            }
            return eventData;
        }

        /// <summary>
        /// We expect that the arguments to the Event method and the arguments to WriteEvent match. This function 
        /// checks that they in fact match and logs a warning to the debugger if they don't.
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="args"></param>
        private void LogEventArgsMismatches(ParameterInfo[] infos, object[] args)
        {
#if (!ES_BUILD_PCL && !ES_BUILD_PN)
            // It would be nice to have this on PCL builds, but it would be pointless since there isn't support for 
            // writing to the debugger log on PCL.
            bool typesMatch = args.Length == infos.Length;

            int i = 0;
            while (typesMatch && i < args.Length)
            {
                Type pType = infos[i].ParameterType;

                // Checking to see if the Parameter types (from the Event method) match the supplied argument types.
                // Fail if one of two things hold : either the argument type is not equal to the parameter type, or the 
                // argument is null and the parameter type is non-nullable.
                if ((args[i] != null && (args[i].GetType() != pType))
                    || (args[i] == null && (!(pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>))))
                    )
                {
                    typesMatch = false;
                    break;
                }

                ++i;
            }

            if (!typesMatch)
            {
                System.Diagnostics.Debugger.Log(0, null, Resources.GetResourceString("EventSource_VarArgsParameterMismatch") + "\r\n");
            }
#endif //!ES_BUILD_PCL
        }

        unsafe private void WriteToAllListeners(int eventId, Guid* childActivityID, int eventDataCount, EventSource.EventData* data)
        {
            // We represent a byte[] as a integer denoting the length  and then a blob of bytes in the data pointer. This causes a spurious
            // warning because eventDataCount is off by one for the byte[] case since a byte[] has 2 items associated it. So we want to check
            // that the number of parameters is correct against the byte[] case, but also we the args array would be one too long if
            // we just used the modifiedParamCount here -- so we need both.
            int paramCount = GetParameterCount(m_eventData[eventId]);
            int modifiedParamCount = 0;
            for (int i = 0; i < paramCount; i++)
            {
                Type parameterType = GetDataType(m_eventData[eventId], i);
                if (parameterType == typeof(byte[]))
                {
                    modifiedParamCount += 2;
                }
                else
                {
                    modifiedParamCount++;
                }
            }
            if (eventDataCount != modifiedParamCount)
            {
                ReportOutOfBandMessage(Resources.GetResourceString("EventSource_EventParametersMismatch", eventId, eventDataCount, paramCount), true);
                paramCount = Math.Min(paramCount, eventDataCount);
            }

            object[] args = new object[paramCount];

            EventSource.EventData* dataPtr = data;
            for (int i = 0; i < paramCount; i++)
                args[i] = DecodeObject(eventId, i, ref dataPtr);
            WriteToAllListeners(eventId, childActivityID, args);
        }

        // helper for writing to all EventListeners attached the current eventSource.  
        unsafe private void WriteToAllListeners(int eventId, Guid* childActivityID, params object[] args)
        {
            EventWrittenEventArgs eventCallbackArgs = new EventWrittenEventArgs(this);
            eventCallbackArgs.EventId = eventId;
            if (childActivityID != null)
                eventCallbackArgs.RelatedActivityId = *childActivityID;
            eventCallbackArgs.EventName = m_eventData[eventId].Name;
            eventCallbackArgs.Message = m_eventData[eventId].Message;
            eventCallbackArgs.Payload = new ReadOnlyCollection<object>(args);

            DispatchToAllListeners(eventId, childActivityID, eventCallbackArgs);
        }

        private unsafe void DispatchToAllListeners(int eventId, Guid* childActivityID, EventWrittenEventArgs eventCallbackArgs)
        {
            Exception lastThrownException = null;
            for (EventDispatcher dispatcher = m_Dispatchers; dispatcher != null; dispatcher = dispatcher.m_Next)
            {
                Debug.Assert(dispatcher.m_EventEnabled != null);
                if (eventId == -1 || dispatcher.m_EventEnabled[eventId])
                {
#if FEATURE_ACTIVITYSAMPLING
                    var activityFilter = dispatcher.m_Listener.m_activityFilter;
                    // order below is important as PassesActivityFilter will "flow" active activities
                    // even when the current EventSource doesn't have filtering enabled. This allows
                    // interesting activities to be updated so that sources that do sample can get
                    // accurate data
                    if (activityFilter == null ||
                        ActivityFilter.PassesActivityFilter(activityFilter, childActivityID,
                                                            m_eventData[eventId].TriggersActivityTracking > 0,
                                                            this, eventId) ||
                        !dispatcher.m_activityFilteringEnabled)
#endif // FEATURE_ACTIVITYSAMPLING
                    {
                        try
                        {
                            dispatcher.m_Listener.OnEventWritten(eventCallbackArgs);
                        }
                        catch (Exception e)
                        {
                            ReportOutOfBandMessage("ERROR: Exception during EventSource.OnEventWritten: "
                                 + e.Message, false);
                            lastThrownException = e;
                        }
                    }
                }
            }

            if (lastThrownException != null)
            {
                throw new EventSourceException(lastThrownException);
            }
        }

        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        private unsafe void WriteEventString(EventLevel level, long keywords, string msgString)
        {
#if FEATURE_MANAGED_ETW && !FEATURE_PERFTRACING
            if (m_provider != null)
            {
                string eventName = "EventSourceMessage";
                if (SelfDescribingEvents)
                {
                    EventSourceOptions opt = new EventSourceOptions
                    {
                        Keywords = (EventKeywords)unchecked(keywords),
                        Level = level
                    };
                    var msg = new { message = msgString };
                    var tlet = new TraceLoggingEventTypes(eventName, EventTags.None, new Type[] { msg.GetType() });
                    WriteMultiMergeInner(eventName, ref opt, tlet, null, null, msg);
                }
                else
                {
                    // We want the name of the provider to show up so if we don't have a manifest we create 
                    // on that at least has the provider name (I don't define any events).   
                    if (m_rawManifest == null && m_outOfBandMessageCount == 1)
                    {
                        ManifestBuilder manifestBuilder = new ManifestBuilder(Name, Guid, Name, null, EventManifestOptions.None);
                        manifestBuilder.StartEvent(eventName, new EventAttribute(0) { Level = EventLevel.LogAlways, Task = (EventTask)0xFFFE });
                        manifestBuilder.AddEventParameter(typeof(string), "message");
                        manifestBuilder.EndEvent();
                        SendManifest(manifestBuilder.CreateManifest());
                    }

                    // We use this low level routine to to bypass the enabled checking, since the eventSource itself is only partially inited. 
                    fixed (char* msgStringPtr = msgString)
                    {
                        EventDescriptor descr = new EventDescriptor(0, 0, 0, (byte)level, 0, 0, keywords);
                        EventProvider.EventData data = new EventProvider.EventData();
                        data.Ptr = (ulong)msgStringPtr;
                        data.Size = (uint)(2 * (msgString.Length + 1));
                        data.Reserved = 0;
                        m_provider.WriteEvent(ref descr, IntPtr.Zero, null, null, 1, (IntPtr)((void*)&data));
                    }
                }
            }
#endif // FEATURE_MANAGED_ETW
        }

        /// <summary>
        /// Since this is a means of reporting errors (see ReportoutOfBandMessage) any failure encountered 
        /// while writing the message to any one of the listeners will be silently ignored.
        /// </summary>
        private void WriteStringToAllListeners(string eventName, string msg)
        {
            EventWrittenEventArgs eventCallbackArgs = new EventWrittenEventArgs(this);
            eventCallbackArgs.EventId = 0;
            eventCallbackArgs.Message = msg;
            eventCallbackArgs.Payload = new ReadOnlyCollection<object>(new List<object>() { msg });
            eventCallbackArgs.PayloadNames = new ReadOnlyCollection<string>(new List<string> { "message" });
            eventCallbackArgs.EventName = eventName;

            for (EventDispatcher dispatcher = m_Dispatchers; dispatcher != null; dispatcher = dispatcher.m_Next)
            {
                bool dispatcherEnabled = false;
                if (dispatcher.m_EventEnabled == null)
                {
                    // if the listeners that weren't correctly initialized, we will send to it
                    // since this is an error message and we want to see it go out. 
                    dispatcherEnabled = true;
                }
                else
                {
                    // if there's *any* enabled event on the dispatcher we'll write out the string
                    // otherwise we'll treat the listener as disabled and skip it
                    for (int evtId = 0; evtId < dispatcher.m_EventEnabled.Length; ++evtId)
                    {
                        if (dispatcher.m_EventEnabled[evtId])
                        {
                            dispatcherEnabled = true;
                            break;
                        }
                    }
                }
                try
                {
                    if (dispatcherEnabled)
                        dispatcher.m_Listener.OnEventWritten(eventCallbackArgs);
                }
                catch
                {
                    // ignore any exceptions thrown by listeners' OnEventWritten
                }
            }
        }

#if FEATURE_ACTIVITYSAMPLING
        unsafe private SessionMask GetEtwSessionMask(int eventId, Guid* childActivityID)
        {
            SessionMask etwSessions = new SessionMask();

            for (int i = 0; i < SessionMask.MAX; ++i)
            {
                EtwSession etwSession = m_etwSessionIdMap[i];
                if (etwSession != null)
                {
                    ActivityFilter activityFilter = etwSession.m_activityFilter;
                    // PassesActivityFilter() will flow "interesting" activities, so make sure
                    // to perform this test first, before ORing with ~m_activityFilteringForETWEnabled
                    // (note: the first test for !m_activityFilteringForETWEnabled[i] ensures we
                    //  do not fire events indiscriminately, when no filters are specified, but only 
                    //  if, in addition, the session did not also enable ActivitySampling)
                    if (activityFilter == null && !m_activityFilteringForETWEnabled[i] ||
                        activityFilter != null &&
                            ActivityFilter.PassesActivityFilter(activityFilter, childActivityID,
                                m_eventData[eventId].TriggersActivityTracking > 0, this, eventId) ||
                        !m_activityFilteringForETWEnabled[i])
                    {
                        etwSessions[i] = true;
                    }
                }
            }
            // flow "interesting" activities for all legacy sessions in which there's some 
            // level of activity tracing enabled (even other EventSources)
            if (m_legacySessions != null && m_legacySessions.Count > 0 &&
                (EventOpcode)m_eventData[eventId].Descriptor.Opcode == EventOpcode.Send)
            {
                // only calculate InternalCurrentThreadActivityId once
                Guid* pCurrentActivityId = null;
                Guid currentActivityId;
                foreach (var legacyEtwSession in m_legacySessions)
                {
                    if (legacyEtwSession == null)
                        continue;

                    ActivityFilter activityFilter = legacyEtwSession.m_activityFilter;
                    if (activityFilter != null)
                    {
                        if (pCurrentActivityId == null)
                        {
                            currentActivityId = InternalCurrentThreadActivityId;
                            pCurrentActivityId = &currentActivityId;
                        }
                        ActivityFilter.FlowActivityIfNeeded(activityFilter, pCurrentActivityId, childActivityID);
                    }
                }
            }

            return etwSessions;
        }
#endif // FEATURE_ACTIVITYSAMPLING

        /// <summary>
        /// Returns true if 'eventNum' is enabled if you only consider the level and matchAnyKeyword filters.
        /// It is possible that eventSources turn off the event based on additional filtering criteria.  
        /// </summary>
        private bool IsEnabledByDefault(int eventNum, bool enable, EventLevel currentLevel, EventKeywords currentMatchAnyKeyword)
        {
            if (!enable)
                return false;

            EventLevel eventLevel = (EventLevel)m_eventData[eventNum].Descriptor.Level;
            EventKeywords eventKeywords = unchecked((EventKeywords)((ulong)m_eventData[eventNum].Descriptor.Keywords & (~(SessionMask.All.ToEventKeywords()))));

#if FEATURE_MANAGED_ETW_CHANNELS
            EventChannel channel = unchecked((EventChannel)m_eventData[eventNum].Descriptor.Channel);
#else
            EventChannel channel = EventChannel.None;
#endif

            return IsEnabledCommon(enable, currentLevel, currentMatchAnyKeyword, eventLevel, eventKeywords, channel);
        }

        private bool IsEnabledCommon(bool enabled, EventLevel currentLevel, EventKeywords currentMatchAnyKeyword,
                                                          EventLevel eventLevel, EventKeywords eventKeywords, EventChannel eventChannel)
        {
            if (!enabled)
                return false;

            // does is pass the level test?
            if ((currentLevel != 0) && (currentLevel < eventLevel))
                return false;

            // if yes, does it pass the keywords test?
            if (currentMatchAnyKeyword != 0 && eventKeywords != 0)
            {
#if FEATURE_MANAGED_ETW_CHANNELS
                // is there a channel with keywords that match currentMatchAnyKeyword?
                if (eventChannel != EventChannel.None && this.m_channelData != null && this.m_channelData.Length > (int)eventChannel)
                {
                    EventKeywords channel_keywords = unchecked((EventKeywords)(m_channelData[(int)eventChannel] | (ulong)eventKeywords));
                    if (channel_keywords != 0 && (channel_keywords & currentMatchAnyKeyword) == 0)
                        return false;
                }
                else
#endif
                {
                    if ((unchecked((ulong)eventKeywords & (ulong)currentMatchAnyKeyword)) == 0)
                        return false;
                }
            }
            return true;

        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private void ThrowEventSourceException(string eventName, Exception innerEx = null)
        {
            // If we fail during out of band logging we may end up trying 
            // to throw another EventSourceException, thus hitting a StackOverflowException. 
            // Avoid StackOverflow by making sure we do not recursively call this method.
            if (m_EventSourceExceptionRecurenceCount > 0)
                return;
            try
            {
                m_EventSourceExceptionRecurenceCount++;

                string errorPrefix = "EventSourceException";
                if (eventName != null)
                {
                    errorPrefix += " while processing event \"" + eventName + "\"";
                }

                // TODO Create variations of EventSourceException that indicate more information using the error code.
                switch (EventProvider.GetLastWriteEventError())
                {
                    case EventProvider.WriteEventErrorCode.EventTooBig:
                        ReportOutOfBandMessage(errorPrefix + ": " + Resources.GetResourceString("EventSource_EventTooBig"), true);
                        if (ThrowOnEventWriteErrors) throw new EventSourceException(Resources.GetResourceString("EventSource_EventTooBig"), innerEx);
                        break;
                    case EventProvider.WriteEventErrorCode.NoFreeBuffers:
                        ReportOutOfBandMessage(errorPrefix + ": " + Resources.GetResourceString("EventSource_NoFreeBuffers"), true);
                        if (ThrowOnEventWriteErrors) throw new EventSourceException(Resources.GetResourceString("EventSource_NoFreeBuffers"), innerEx);
                        break;
                    case EventProvider.WriteEventErrorCode.NullInput:
                        ReportOutOfBandMessage(errorPrefix + ": " + Resources.GetResourceString("EventSource_NullInput"), true);
                        if (ThrowOnEventWriteErrors) throw new EventSourceException(Resources.GetResourceString("EventSource_NullInput"), innerEx);
                        break;
                    case EventProvider.WriteEventErrorCode.TooManyArgs:
                        ReportOutOfBandMessage(errorPrefix + ": " + Resources.GetResourceString("EventSource_TooManyArgs"), true);
                        if (ThrowOnEventWriteErrors) throw new EventSourceException(Resources.GetResourceString("EventSource_TooManyArgs"), innerEx);
                        break;
                    default:
                        if (innerEx != null)
                            ReportOutOfBandMessage(errorPrefix + ": " + innerEx.GetType() + ":" + innerEx.Message, true);
                        else
                            ReportOutOfBandMessage(errorPrefix, true);
                        if (ThrowOnEventWriteErrors) throw new EventSourceException(innerEx);
                        break;
                }
            }
            finally
            {
                m_EventSourceExceptionRecurenceCount--;
            }
        }

        private void ValidateEventOpcodeForTransfer(ref EventMetadata eventData, string eventName)
        {
            if ((EventOpcode)eventData.Descriptor.Opcode != EventOpcode.Send &&
                (EventOpcode)eventData.Descriptor.Opcode != EventOpcode.Receive &&
                (EventOpcode)eventData.Descriptor.Opcode != EventOpcode.Start)
            {
                ThrowEventSourceException(eventName);
            }
        }

        internal static EventOpcode GetOpcodeWithDefault(EventOpcode opcode, string eventName)
        {
            if (opcode == EventOpcode.Info && eventName != null)
            {
                if (eventName.EndsWith(s_ActivityStartSuffix, StringComparison.Ordinal))
                {
                    return EventOpcode.Start;
                }
                else if (eventName.EndsWith(s_ActivityStopSuffix, StringComparison.Ordinal))
                {
                    return EventOpcode.Stop;
                }
            }

            return opcode;
        }

#if FEATURE_MANAGED_ETW
        /// <summary>
        /// This class lets us hook the 'OnEventCommand' from the eventSource.  
        /// </summary>
        private class OverideEventProvider : EventProvider
        {
            public OverideEventProvider(EventSource eventSource)
            {
                this.m_eventSource = eventSource;
            }
            protected override void OnControllerCommand(ControllerCommand command, IDictionary<string, string> arguments,
                                                              int perEventSourceSessionId, int etwSessionId)
            {
                // We use null to represent the ETW EventListener.  
                EventListener listener = null;
                m_eventSource.SendCommand(listener, perEventSourceSessionId, etwSessionId,
                                          (EventCommand)command, IsEnabled(), Level, MatchAnyKeyword, arguments);
            }
            private EventSource m_eventSource;
        }
#endif

        /// <summary>
        /// Used to hold all the static information about an event.  This includes everything in the event
        /// descriptor as well as some stuff we added specifically for EventSource. see the
        /// code:m_eventData for where we use this.  
        /// </summary>

        /*
         EventMetadata was public in the separate System.Diagnostics.Tracing assembly(pre NS2.0), 
         now the move to CoreLib marked them as private.
         While they are technically private (it's a contract used between the library and the ILC toolchain), 
         we need them to be rooted and exported from shared library for the system to work.
         For now I'm simply marking them as public again.A cleaner solution might be to use.rd.xml to 
         root them and modify shared library definition to force export them.
         */
#if ES_BUILD_PN
       public
#else
       internal
#endif
        partial struct EventMetadata
        {
            public EventDescriptor Descriptor;
            public IntPtr EventHandle;              // EventPipeEvent handle.
            public EventTags Tags;
            public bool EnabledForAnyListener;      // true if any dispatcher has this event turned on
            public bool EnabledForETW;              // is this event on for the OS ETW data dispatcher?

            public bool HasRelatedActivityID;       // Set if the event method's first parameter is a Guid named 'relatedActivityId'
#if !FEATURE_ACTIVITYSAMPLING
#pragma warning disable 0649
#endif
            public byte TriggersActivityTracking;   // count of listeners that marked this event as trigger for start of activity logging.
#if !FEATURE_ACTIVITYSAMPLING
#pragma warning restore 0649
#endif
            public string Name;                     // the name of the event
            public string Message;                  // If the event has a message associated with it, this is it.  
            public ParameterInfo[] Parameters;      // TODO can we remove? 

            public TraceLoggingEventTypes TraceLoggingEventTypes;
            public EventActivityOptions ActivityOptions;

#if ES_BUILD_PN
            public EventParameterType[] ParameterTypes;
#endif
        };

        // This is the internal entry point that code:EventListeners call when wanting to send a command to a
        // eventSource. The logic is as follows
        // 
        // * if Command == Update
        //     * perEventSourceSessionId specifies the per-provider ETW session ID that the command applies 
        //         to (if listener != null)
        //         perEventSourceSessionId = 0 - reserved for EventListeners
        //         perEventSourceSessionId = 1..SessionMask.MAX - reserved for activity tracing aware ETW sessions
        //                  perEventSourceSessionId-1 represents the bit in the reserved field (bits 44..47) in 
        //                  Keywords that identifies the session
        //         perEventSourceSessionId = SessionMask.MAX+1 - reserved for legacy ETW sessions; these are 
        //                  discriminated by etwSessionId
        //     * etwSessionId specifies a machine-wide ETW session ID; this allows correlation of
        //         activity tracing across different providers (which might have different sessionIds
        //         for the same ETW session)
        //     * enable, level, matchAnyKeywords are used to set a default for all events for the
        //         eventSource.  In particular, if 'enabled' is false, 'level' and
        //         'matchAnyKeywords' are not used.  
        //     * OnEventCommand is invoked, which may cause calls to
        //         code:EventSource.EnableEventForDispatcher which may cause changes in the filtering
        //         depending on the logic in that routine.
        // * else (command != Update)
        //     * Simply call OnEventCommand. The expectation is that filtering is NOT changed.
        //     * The 'enabled' 'level', matchAnyKeyword' arguments are ignored (must be true, 0, 0).  
        // 
        // dispatcher == null has special meaning. It is the 'ETW' dispatcher.
        internal void SendCommand(EventListener listener, int perEventSourceSessionId, int etwSessionId,
                                  EventCommand command, bool enable,
                                  EventLevel level, EventKeywords matchAnyKeyword,
                                  IDictionary<string, string> commandArguments)
        {
            var commandArgs = new EventCommandEventArgs(command, commandArguments, this, listener, perEventSourceSessionId, etwSessionId, enable, level, matchAnyKeyword);
            lock (EventListener.EventListenersLock)
            {
                if (m_completelyInited)
                {
                    // After the first command arrive after construction, we are ready to get rid of the deferred commands
                    this.m_deferredCommands = null;
                    // We are fully initialized, do the command 
                    DoCommand(commandArgs);
                }
                else
                {
                    // We can't do the command, simply remember it and we do it when we are fully constructed.  
                    commandArgs.nextCommand = m_deferredCommands;
                    m_deferredCommands = commandArgs;
                }
            }
        }

        /// <summary>
        /// We want the eventSource to be fully initialized when we do commands because that way we can send 
        /// error messages and other logging directly to the event stream.   Unfortunately we can get callbacks
        /// when we are not fully initialized.  In that case we store them in 'commandArgs' and do them later. 
        /// This helper actually does all actual command logic. 
        /// </summary>
        internal void DoCommand(EventCommandEventArgs commandArgs)
        {
            // PRECONDITION: We should be holding the EventListener.EventListenersLock
            // We defer commands until we are completely inited.  This allows error messages to be sent.  
            Debug.Assert(m_completelyInited);

#if FEATURE_MANAGED_ETW
            if (m_provider == null)     // If we failed to construct
                return;
#endif // FEATURE_MANAGED_ETW

            m_outOfBandMessageCount = 0;
            bool shouldReport = (commandArgs.perEventSourceSessionId > 0) && (commandArgs.perEventSourceSessionId <= SessionMask.MAX);
            try
            {
                EnsureDescriptorsInitialized();
                Debug.Assert(m_eventData != null);

                // Find the per-EventSource dispatcher corresponding to registered dispatcher
                commandArgs.dispatcher = GetDispatcher(commandArgs.listener);
                if (commandArgs.dispatcher == null && commandArgs.listener != null)     // dispatcher == null means ETW dispatcher
                {
                    throw new ArgumentException(Resources.GetResourceString("EventSource_ListenerNotFound"));
                }

                if (commandArgs.Arguments == null)
                    commandArgs.Arguments = new Dictionary<string, string>();

                if (commandArgs.Command == EventCommand.Update)
                {
                    // Set it up using the 'standard' filtering bitfields (use the "global" enable, not session specific one)
                    for (int i = 0; i < m_eventData.Length; i++)
                        EnableEventForDispatcher(commandArgs.dispatcher, i, IsEnabledByDefault(i, commandArgs.enable, commandArgs.level, commandArgs.matchAnyKeyword));

                    if (commandArgs.enable)
                    {
                        if (!m_eventSourceEnabled)
                        {
                            // EventSource turned on for the first time, simply copy the bits.  
                            m_level = commandArgs.level;
                            m_matchAnyKeyword = commandArgs.matchAnyKeyword;
                        }
                        else
                        {
                            // Already enabled, make it the most verbose of the existing and new filter
                            if (commandArgs.level > m_level)
                                m_level = commandArgs.level;
                            if (commandArgs.matchAnyKeyword == 0)
                                m_matchAnyKeyword = 0;
                            else if (m_matchAnyKeyword != 0)
                                m_matchAnyKeyword = unchecked(m_matchAnyKeyword | commandArgs.matchAnyKeyword);
                        }
                    }

                    // interpret perEventSourceSessionId's sign, and adjust perEventSourceSessionId to 
                    // represent 0-based positive values
                    bool bSessionEnable = (commandArgs.perEventSourceSessionId >= 0);
                    if (commandArgs.perEventSourceSessionId == 0 && commandArgs.enable == false)
                        bSessionEnable = false;

                    if (commandArgs.listener == null)
                    {
                        if (!bSessionEnable)
                            commandArgs.perEventSourceSessionId = -commandArgs.perEventSourceSessionId;
                        // for "global" enable/disable (passed in with listener == null and
                        //  perEventSourceSessionId == 0) perEventSourceSessionId becomes -1
                        --commandArgs.perEventSourceSessionId;
                    }

                    commandArgs.Command = bSessionEnable ? EventCommand.Enable : EventCommand.Disable;

                    // perEventSourceSessionId = -1 when ETW sent a notification, but the set of active sessions
                    // hasn't changed.
                    // sesisonId = SessionMask.MAX when one of the legacy ETW sessions changed
                    // 0 <= perEventSourceSessionId < SessionMask.MAX for activity-tracing aware sessions
                    Debug.Assert(commandArgs.perEventSourceSessionId >= -1 && commandArgs.perEventSourceSessionId <= SessionMask.MAX);

                    // Send the manifest if we are enabling an ETW session
                    if (bSessionEnable && commandArgs.dispatcher == null)
                    {
                        // eventSourceDispatcher == null means this is the ETW manifest

#if !FEATURE_PERFTRACING
                        // Note that we unconditionally send the manifest whenever we are enabled, even if
                        // we were already enabled.   This is because there may be multiple sessions active
                        // and we can't know that all the sessions have seen the manifest.  
                        if (!SelfDescribingEvents)
                            SendManifest(m_rawManifest);
#endif
                    }

#if FEATURE_ACTIVITYSAMPLING
                    if (bSessionEnable && commandArgs.perEventSourceSessionId != -1)
                    {
                        bool participateInSampling = false;
                        string activityFilters;
                        int sessionIdBit;

                        ParseCommandArgs(commandArgs.Arguments, out participateInSampling,
                                            out activityFilters, out sessionIdBit);

                        if (commandArgs.listener == null && commandArgs.Arguments.Count > 0 && commandArgs.perEventSourceSessionId != sessionIdBit)
                        {
                            throw new ArgumentException(Resources.GetResourceString("EventSource_SessionIdError",
                                                    commandArgs.perEventSourceSessionId + SessionMask.SHIFT_SESSION_TO_KEYWORD,
                                                        sessionIdBit + SessionMask.SHIFT_SESSION_TO_KEYWORD));
                        }

                        if (commandArgs.listener == null)
                        {
                            UpdateEtwSession(commandArgs.perEventSourceSessionId, commandArgs.etwSessionId, true, activityFilters, participateInSampling);
                        }
                        else
                        {
                            ActivityFilter.UpdateFilter(ref commandArgs.listener.m_activityFilter, this, 0, activityFilters);
                            commandArgs.dispatcher.m_activityFilteringEnabled = participateInSampling;
                        }
                    }
                    else if (!bSessionEnable && commandArgs.listener == null)
                    {
                        // if we disable an ETW session, indicate that in a synthesized command argument
                        if (commandArgs.perEventSourceSessionId >= 0 && commandArgs.perEventSourceSessionId < SessionMask.MAX)
                        {
                            commandArgs.Arguments["EtwSessionKeyword"] = (commandArgs.perEventSourceSessionId + SessionMask.SHIFT_SESSION_TO_KEYWORD).ToString(CultureInfo.InvariantCulture);
                        }
                    }
#endif // FEATURE_ACTIVITYSAMPLING

                    // Turn on the enable bit before making the OnEventCommand callback  This allows you to do useful
                    // things like log messages, or test if keywords are enabled in the callback.  
                    if (commandArgs.enable)
                    {
                        Debug.Assert(m_eventData != null);
                        m_eventSourceEnabled = true;
                    }

                    this.OnEventCommand(commandArgs);
                    var eventCommandCallback = this.m_eventCommandExecuted;
                    if (eventCommandCallback != null)
                        eventCommandCallback(this, commandArgs);

#if FEATURE_ACTIVITYSAMPLING
                    if (commandArgs.listener == null && !bSessionEnable && commandArgs.perEventSourceSessionId != -1)
                    {
                        // if we disable an ETW session, complete disabling it
                        UpdateEtwSession(commandArgs.perEventSourceSessionId, commandArgs.etwSessionId, false, null, false);
                    }
#endif // FEATURE_ACTIVITYSAMPLING

                    if (!commandArgs.enable)
                    {
                        // If we are disabling, maybe we can turn on 'quick checks' to filter
                        // quickly.  These are all just optimizations (since later checks will still filter)

#if FEATURE_ACTIVITYSAMPLING
                        // Turn off (and forget) any information about Activity Tracing.  
                        if (commandArgs.listener == null)
                        {
                            // reset all filtering information for activity-tracing-aware sessions
                            for (int i = 0; i < SessionMask.MAX; ++i)
                            {
                                EtwSession etwSession = m_etwSessionIdMap[i];
                                if (etwSession != null)
                                    ActivityFilter.DisableFilter(ref etwSession.m_activityFilter, this);
                            }
                            m_activityFilteringForETWEnabled = new SessionMask(0);
                            m_curLiveSessions = new SessionMask(0);
                            // reset activity-tracing-aware sessions
                            if (m_etwSessionIdMap != null)
                                for (int i = 0; i < SessionMask.MAX; ++i)
                                    m_etwSessionIdMap[i] = null;
                            // reset legacy sessions
                            if (m_legacySessions != null)
                                m_legacySessions.Clear();
                        }
                        else
                        {
                            ActivityFilter.DisableFilter(ref commandArgs.listener.m_activityFilter, this);
                            commandArgs.dispatcher.m_activityFilteringEnabled = false;
                        }
#endif // FEATURE_ACTIVITYSAMPLING

                        // There is a good chance EnabledForAnyListener are not as accurate as
                        // they could be, go ahead and get a better estimate.  
                        for (int i = 0; i < m_eventData.Length; i++)
                        {
                            bool isEnabledForAnyListener = false;
                            for (EventDispatcher dispatcher = m_Dispatchers; dispatcher != null; dispatcher = dispatcher.m_Next)
                            {
                                if (dispatcher.m_EventEnabled[i])
                                {
                                    isEnabledForAnyListener = true;
                                    break;
                                }
                            }
                            m_eventData[i].EnabledForAnyListener = isEnabledForAnyListener;
                        }

                        // If no events are enabled, disable the global enabled bit.
                        if (!AnyEventEnabled())
                        {
                            m_level = 0;
                            m_matchAnyKeyword = 0;
                            m_eventSourceEnabled = false;
                        }
                    }
#if FEATURE_ACTIVITYSAMPLING
                    UpdateKwdTriggers(commandArgs.enable);
#endif // FEATURE_ACTIVITYSAMPLING
                }
                else
                {
#if !FEATURE_PERFTRACING
                    if (commandArgs.Command == EventCommand.SendManifest)
                    {
                        // TODO: should we generate the manifest here if we hadn't already?
                        if (m_rawManifest != null)
                            SendManifest(m_rawManifest);
                    }
#endif

                    // These are not used for non-update commands and thus should always be 'default' values
                    // Debug.Assert(enable == true);
                    // Debug.Assert(level == EventLevel.LogAlways);
                    // Debug.Assert(matchAnyKeyword == EventKeywords.None);

                    this.OnEventCommand(commandArgs);
                    var eventCommandCallback = m_eventCommandExecuted;
                    if (eventCommandCallback != null)
                        eventCommandCallback(this, commandArgs);
                }

#if FEATURE_ACTIVITYSAMPLING
                if (m_completelyInited && (commandArgs.listener != null || shouldReport))
                {
                    SessionMask m = SessionMask.FromId(commandArgs.perEventSourceSessionId);
                    ReportActivitySamplingInfo(commandArgs.listener, m);
                }
#endif // FEATURE_ACTIVITYSAMPLING
            }
            catch (Exception e)
            {
                // When the ETW session is created after the EventSource has registered with the ETW system
                // we can send any error messages here.
                ReportOutOfBandMessage("ERROR: Exception in Command Processing for EventSource " + Name + ": " + e.Message, true);
                // We never throw when doing a command.  
            }
        }

#if FEATURE_ACTIVITYSAMPLING

        internal void UpdateEtwSession(
            int sessionIdBit,
            int etwSessionId,
            bool bEnable,
            string activityFilters,
            bool participateInSampling)
        {
            if (sessionIdBit < SessionMask.MAX)
            {
                // activity-tracing-aware etw session
                if (bEnable)
                {
                    var etwSession = EtwSession.GetEtwSession(etwSessionId, true);
                    ActivityFilter.UpdateFilter(ref etwSession.m_activityFilter, this, sessionIdBit, activityFilters);
                    m_etwSessionIdMap[sessionIdBit] = etwSession;
                    m_activityFilteringForETWEnabled[sessionIdBit] = participateInSampling;
                }
                else
                {
                    var etwSession = EtwSession.GetEtwSession(etwSessionId);
                    m_etwSessionIdMap[sessionIdBit] = null;
                    m_activityFilteringForETWEnabled[sessionIdBit] = false;
                    if (etwSession != null)
                    {
                        ActivityFilter.DisableFilter(ref etwSession.m_activityFilter, this);
                        // the ETW session is going away; remove it from the global list
                        EtwSession.RemoveEtwSession(etwSession);
                    }
                }
                m_curLiveSessions[sessionIdBit] = bEnable;
            }
            else
            {
                // legacy etw session    
                if (bEnable)
                {
                    if (m_legacySessions == null)
                        m_legacySessions = new List<EtwSession>(8);
                    var etwSession = EtwSession.GetEtwSession(etwSessionId, true);
                    if (!m_legacySessions.Contains(etwSession))
                        m_legacySessions.Add(etwSession);
                }
                else
                {
                    var etwSession = EtwSession.GetEtwSession(etwSessionId);
                    if (etwSession != null)
                    {
                        if (m_legacySessions != null)
                            m_legacySessions.Remove(etwSession);
                        // the ETW session is going away; remove it from the global list
                        EtwSession.RemoveEtwSession(etwSession);
                    }
                }
            }
        }

        internal static bool ParseCommandArgs(
                        IDictionary<string, string> commandArguments,
                        out bool participateInSampling,
                        out string activityFilters,
                        out int sessionIdBit)
        {
            bool res = true;
            participateInSampling = false;
            string activityFilterString;
            if (commandArguments.TryGetValue("ActivitySamplingStartEvent", out activityFilters))
            {
                // if a start event is specified default the event source to participate in sampling
                participateInSampling = true;
            }

            if (commandArguments.TryGetValue("ActivitySampling", out activityFilterString))
            {
                if (string.Compare(activityFilterString, "false", StringComparison.OrdinalIgnoreCase) == 0 ||
                    activityFilterString == "0")
                    participateInSampling = false;
                else
                    participateInSampling = true;
            }

            string sSessionKwd;
            int sessionKwd = -1;
            if (!commandArguments.TryGetValue("EtwSessionKeyword", out sSessionKwd) ||
                !int.TryParse(sSessionKwd, out sessionKwd) ||
                sessionKwd < SessionMask.SHIFT_SESSION_TO_KEYWORD ||
                sessionKwd >= SessionMask.SHIFT_SESSION_TO_KEYWORD + SessionMask.MAX)
            {
                sessionIdBit = -1;
                res = false;
            }
            else
            {
                sessionIdBit = sessionKwd - SessionMask.SHIFT_SESSION_TO_KEYWORD;
            }
            return res;
        }

        internal void UpdateKwdTriggers(bool enable)
        {
            if (enable)
            {
                // recompute m_keywordTriggers
                ulong gKeywords = unchecked((ulong)m_matchAnyKeyword);
                if (gKeywords == 0)
                    gKeywords = 0xFFFFffffFFFFffff;

                m_keywordTriggers = 0;
                for (int sessId = 0; sessId < SessionMask.MAX; ++sessId)
                {
                    EtwSession etwSession = m_etwSessionIdMap[sessId];
                    if (etwSession == null)
                        continue;

                    ActivityFilter activityFilter = etwSession.m_activityFilter;
                    ActivityFilter.UpdateKwdTriggers(activityFilter, m_guid, this, unchecked((EventKeywords)gKeywords));
                }
            }
            else
            {
                m_keywordTriggers = 0;
            }
        }

#endif // FEATURE_ACTIVITYSAMPLING

        /// <summary>
        /// If 'value is 'true' then set the eventSource so that 'dispatcher' will receive event with the eventId
        /// of 'eventId.  If value is 'false' disable the event for that dispatcher.   If 'eventId' is out of
        /// range return false, otherwise true.  
        /// </summary>
        internal bool EnableEventForDispatcher(EventDispatcher dispatcher, int eventId, bool value)
        {
            if (dispatcher == null)
            {
                if (eventId >= m_eventData.Length)
                    return false;
#if FEATURE_MANAGED_ETW
                if (m_provider != null)
                    m_eventData[eventId].EnabledForETW = value;
#endif
            }
            else
            {
                if (eventId >= dispatcher.m_EventEnabled.Length)
                    return false;
                dispatcher.m_EventEnabled[eventId] = value;
                if (value)
                    m_eventData[eventId].EnabledForAnyListener = true;
            }
            return true;
        }

        /// <summary>
        /// Returns true if any event at all is on.  
        /// </summary>
        private bool AnyEventEnabled()
        {
            for (int i = 0; i < m_eventData.Length; i++)
                if (m_eventData[i].EnabledForETW || m_eventData[i].EnabledForAnyListener)
                    return true;
            return false;
        }

        private bool IsDisposed
        {
            get { return m_eventSourceDisposed; }
        }

        private void EnsureDescriptorsInitialized()
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(Monitor.IsEntered(EventListener.EventListenersLock));
#endif
            if (m_eventData == null)
            {
                Guid eventSourceGuid = Guid.Empty;
                string eventSourceName = null;
                EventMetadata[] eventData = null;
                byte[] manifest = null;

                // Try the GetMetadata provided by the ILTransform in ProjectN. The default sets all to null, and in that case we fall back
                // to the reflection approach.
                GetMetadata(out eventSourceGuid, out eventSourceName, out eventData, out manifest);

                if (eventSourceGuid.Equals(Guid.Empty) || eventSourceName == null || eventData == null || manifest == null)
                {
                    // GetMetadata failed, so we have to set it via reflection.
                    Debug.Assert(m_rawManifest == null);

                    m_rawManifest = CreateManifestAndDescriptors(this.GetType(), Name, this);
                    Debug.Assert(m_eventData != null);

                }
                else
                {
                    // GetMetadata worked, so set the fields as appropriate.
                    m_name = eventSourceName;
                    m_guid = eventSourceGuid;
                    m_eventData = eventData;
                    m_rawManifest = manifest;
                }
                // TODO Enforce singleton pattern 
                foreach (WeakReference eventSourceRef in EventListener.s_EventSources)
                {
                    EventSource eventSource = eventSourceRef.Target as EventSource;
                    if (eventSource != null && eventSource.Guid == m_guid && !eventSource.IsDisposed)
                    {
                        if (eventSource != this)
                        {
                            throw new ArgumentException(Resources.GetResourceString("EventSource_EventSourceGuidInUse", m_guid));
                        }
                    }
                }

                // Make certain all dispatchers also have their arrays initialized
                EventDispatcher dispatcher = m_Dispatchers;
                while (dispatcher != null)
                {
                    if (dispatcher.m_EventEnabled == null)
                        dispatcher.m_EventEnabled = new bool[m_eventData.Length];
                    dispatcher = dispatcher.m_Next;
                }
#if FEATURE_PERFTRACING
                // Initialize the EventPipe event handles.
                DefineEventPipeEvents();
#endif
            }
            if (s_currentPid == 0)
            {
#if ES_BUILD_STANDALONE && !ES_BUILD_PCL && !CORECLR
                // for non-BCL EventSource we must assert SecurityPermission
                new SecurityPermission(PermissionState.Unrestricted).Assert();
#endif
                s_currentPid = Win32Native.GetCurrentProcessId();
            }
        }

        // Send out the ETW manifest XML out to ETW
        // Today, we only send the manifest to ETW, custom listeners don't get it. 
        private unsafe bool SendManifest(byte[] rawManifest)
        {
            bool success = true;

            if (rawManifest == null)
                return false;

            Debug.Assert(!SelfDescribingEvents);

#if FEATURE_MANAGED_ETW
            fixed (byte* dataPtr = rawManifest)
            {
                // we don't want the manifest to show up in the event log channels so we specify as keywords 
                // everything but the first 8 bits (reserved for the 8 channels)
                var manifestDescr = new EventDescriptor(0xFFFE, 1, 0, 0, 0xFE, 0xFFFE, 0x00ffFFFFffffFFFF);
                ManifestEnvelope envelope = new ManifestEnvelope();

                envelope.Format = ManifestEnvelope.ManifestFormats.SimpleXmlFormat;
                envelope.MajorVersion = 1;
                envelope.MinorVersion = 0;
                envelope.Magic = 0x5B;              // An unusual number that can be checked for consistency. 
                int dataLeft = rawManifest.Length;
                envelope.ChunkNumber = 0;

                EventProvider.EventData* dataDescrs = stackalloc EventProvider.EventData[2];

                dataDescrs[0].Ptr = (ulong)&envelope;
                dataDescrs[0].Size = (uint)sizeof(ManifestEnvelope);
                dataDescrs[0].Reserved = 0;

                dataDescrs[1].Ptr = (ulong)dataPtr;
                dataDescrs[1].Reserved = 0;

                int chunkSize = ManifestEnvelope.MaxChunkSize;
                TRY_AGAIN_WITH_SMALLER_CHUNK_SIZE:
                envelope.TotalChunks = (ushort)((dataLeft + (chunkSize - 1)) / chunkSize);
                while (dataLeft > 0)
                {
                    dataDescrs[1].Size = (uint)Math.Min(dataLeft, chunkSize);
                    if (m_provider != null)
                    {
                        if (!m_provider.WriteEvent(ref manifestDescr, IntPtr.Zero, null, null, 2, (IntPtr)dataDescrs))
                        {
                            // Turns out that if users set the BufferSize to something less than 64K then WriteEvent
                            // can fail.   If we get this failure on the first chunk try again with something smaller
                            // The smallest BufferSize is 1K so if we get to 256 (to account for envelope overhead), we can give up making it smaller. 
                            if (EventProvider.GetLastWriteEventError() == EventProvider.WriteEventErrorCode.EventTooBig)
                            {
                                if (envelope.ChunkNumber == 0 && chunkSize > 256)
                                {
                                    chunkSize = chunkSize / 2;
                                    goto TRY_AGAIN_WITH_SMALLER_CHUNK_SIZE;
                                }
                            }
                            success = false;
                            if (ThrowOnEventWriteErrors)
                                ThrowEventSourceException("SendManifest");
                            break;
                        }
                    }
                    dataLeft -= chunkSize;
                    dataDescrs[1].Ptr += (uint)chunkSize;
                    envelope.ChunkNumber++;

                    // For large manifests we want to not overflow any receiver's buffer. Most manifests will fit within
                    // 5 chunks, so only the largest manifests will hit the pause.
                    if ((envelope.ChunkNumber % 5) == 0)
                    {
                        RuntimeThread.Sleep(15);
                    }
                }
            }
#endif // FEATURE_MANAGED_ETW
            return success;
        }

#if (ES_BUILD_PCL)
        internal static Attribute GetCustomAttributeHelper(Type type, Type attributeType, EventManifestOptions flags = EventManifestOptions.None)
        {
            return GetCustomAttributeHelper(type.GetTypeInfo(), attributeType, flags);
        }
#endif

        // Helper to deal with the fact that the type we are reflecting over might be loaded in the ReflectionOnly context.
        // When that is the case, we have the build the custom assemblies on a member by hand.         
        internal static Attribute GetCustomAttributeHelper(MemberInfo member, Type attributeType, EventManifestOptions flags = EventManifestOptions.None)
        {
            if (!member.Module.Assembly.ReflectionOnly() && (flags & EventManifestOptions.AllowEventSourceOverride) == 0)
            {
                // Let the runtime to the work for us, since we can execute code in this context.
                Attribute firstAttribute = null;
                foreach (var attribute in member.GetCustomAttributes(attributeType, false))
                {
                    firstAttribute = (Attribute)attribute;
                    break;
                }
                return firstAttribute;
            }

#if (!ES_BUILD_PCL && !ES_BUILD_PN)
            // In the reflection only context, we have to do things by hand.
            string fullTypeNameToFind = attributeType.FullName;

#if EVENT_SOURCE_LEGACY_NAMESPACE_SUPPORT
            fullTypeNameToFind = fullTypeNameToFind.Replace("System.Diagnostics.Eventing", "System.Diagnostics.Tracing");
#endif

            foreach (CustomAttributeData data in CustomAttributeData.GetCustomAttributes(member))
            {
                if (AttributeTypeNamesMatch(attributeType, data.Constructor.ReflectedType))
                {
                    Attribute attr = null;

                    Debug.Assert(data.ConstructorArguments.Count <= 1);

                    if (data.ConstructorArguments.Count == 1)
                    {
                        attr = (Attribute)Activator.CreateInstance(attributeType, new object[] { data.ConstructorArguments[0].Value });
                    }
                    else if (data.ConstructorArguments.Count == 0)
                    {
                        attr = (Attribute)Activator.CreateInstance(attributeType);
                    }

                    if (attr != null)
                    {
                        Type t = attr.GetType();

                        foreach (CustomAttributeNamedArgument namedArgument in data.NamedArguments)
                        {
                            PropertyInfo p = t.GetProperty(namedArgument.MemberInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                            object value = namedArgument.TypedValue.Value;

                            if (p.PropertyType.IsEnum)
                            {
                                value = Enum.Parse(p.PropertyType, value.ToString());
                            }

                            p.SetValue(attr, value, null);
                        }

                        return attr;
                    }
                }
            }

            return null;
#else // ES_BUILD_PCL && ES_BUILD_PN
            // Don't use nameof here because the resource doesn't exist on some platforms, which results in a compilation error.
            throw new ArgumentException(Resources.GetResourceString("EventSource", "EventSource_PCLPlatformNotSupportedReflection"));
#endif
        }

        /// <summary>
        /// Evaluates if two related "EventSource"-domain types should be considered the same
        /// </summary>
        /// <param name="attributeType">The attribute type in the load context - it's associated with the running 
        /// EventSource type. This type may be different fromt he base type of the user-defined EventSource.</param>
        /// <param name="reflectedAttributeType">The attribute type in the reflection context - it's associated with
        /// the user-defined EventSource, and is in the same assembly as the eventSourceType passed to 
        /// </param>
        /// <returns>True - if the types should be considered equivalent, False - otherwise</returns>
        private static bool AttributeTypeNamesMatch(Type attributeType, Type reflectedAttributeType)
        {
            return
                // are these the same type?
                attributeType == reflectedAttributeType ||
                // are the full typenames equal?
                string.Equals(attributeType.FullName, reflectedAttributeType.FullName, StringComparison.Ordinal) ||
                    // are the typenames equal and the namespaces under "Diagnostics.Tracing" (typically
                    // either Microsoft.Diagnostics.Tracing or System.Diagnostics.Tracing)?
                    string.Equals(attributeType.Name, reflectedAttributeType.Name, StringComparison.Ordinal) &&
                    attributeType.Namespace.EndsWith("Diagnostics.Tracing", StringComparison.Ordinal) &&
                    (reflectedAttributeType.Namespace.EndsWith("Diagnostics.Tracing", StringComparison.Ordinal)
#if EVENT_SOURCE_LEGACY_NAMESPACE_SUPPORT
                     || reflectedAttributeType.Namespace.EndsWith("Diagnostics.Eventing", StringComparison.Ordinal)
#endif
);
        }

        private static Type GetEventSourceBaseType(Type eventSourceType, bool allowEventSourceOverride, bool reflectionOnly)
        {
            // return false for "object" and interfaces
            if (eventSourceType.BaseType() == null)
                return null;

            // now go up the inheritance chain until hitting a concrete type ("object" at worse)
            do
            {
                eventSourceType = eventSourceType.BaseType();
            }
            while (eventSourceType != null && eventSourceType.IsAbstract());

            if (eventSourceType != null)
            {
                if (!allowEventSourceOverride)
                {
                    if (reflectionOnly && eventSourceType.FullName != typeof(EventSource).FullName ||
                        !reflectionOnly && eventSourceType != typeof(EventSource))
                        return null;
                }
                else
                {
                    if (eventSourceType.Name != "EventSource")
                        return null;
                }
            }
            return eventSourceType;
        }

        // Use reflection to look at the attributes of a class, and generate a manifest for it (as UTF8) and
        // return the UTF8 bytes.  It also sets up the code:EventData structures needed to dispatch events
        // at run time.  'source' is the event source to place the descriptors.  If it is null,
        // then the descriptors are not creaed, and just the manifest is generated.  
        private static byte[] CreateManifestAndDescriptors(Type eventSourceType, string eventSourceDllName, EventSource source,
            EventManifestOptions flags = EventManifestOptions.None)
        {
            ManifestBuilder manifest = null;
            bool bNeedsManifest = source != null ? !source.SelfDescribingEvents : true;
            Exception exception = null; // exception that might get raised during validation b/c we couldn't/didn't recover from a previous error
            byte[] res = null;

            if (eventSourceType.IsAbstract() && (flags & EventManifestOptions.Strict) == 0)
                return null;

#if DEBUG && ES_BUILD_STANDALONE
            TestSupport.TestHooks.MaybeThrow(eventSourceType,
                                        TestSupport.Category.ManifestError,
                                        "EventSource_CreateManifestAndDescriptors",
                                        new ArgumentException("EventSource_CreateManifestAndDescriptors"));
#endif

            try
            {
                MethodInfo[] methods = eventSourceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                EventAttribute defaultEventAttribute;
                int eventId = 1;        // The number given to an event that does not have a explicitly given ID. 
                EventMetadata[] eventData = null;
                Dictionary<string, string> eventsByName = null;
                if (source != null || (flags & EventManifestOptions.Strict) != 0)
                {
                    eventData = new EventMetadata[methods.Length + 1];
                    eventData[0].Name = "";         // Event 0 is the 'write messages string' event, and has an empty name.
                }

                // See if we have localization information.  
                ResourceManager resources = null;
                EventSourceAttribute eventSourceAttrib = (EventSourceAttribute)GetCustomAttributeHelper(eventSourceType, typeof(EventSourceAttribute), flags);
                if (eventSourceAttrib != null && eventSourceAttrib.LocalizationResources != null)
                    resources = new ResourceManager(eventSourceAttrib.LocalizationResources, eventSourceType.Assembly());

                manifest = new ManifestBuilder(GetName(eventSourceType, flags), GetGuid(eventSourceType), eventSourceDllName,
                                               resources, flags);

                // Add an entry unconditionally for event ID 0 which will be for a string message.  
                manifest.StartEvent("EventSourceMessage", new EventAttribute(0) { Level = EventLevel.LogAlways, Task = (EventTask)0xFFFE });
                manifest.AddEventParameter(typeof(string), "message");
                manifest.EndEvent();

                // eventSourceType must be sealed and must derive from this EventSource
                if ((flags & EventManifestOptions.Strict) != 0)
                {
                    bool typeMatch = GetEventSourceBaseType(eventSourceType, (flags & EventManifestOptions.AllowEventSourceOverride) != 0, eventSourceType.Assembly().ReflectionOnly()) != null;

                    if (!typeMatch)
                    {
                        manifest.ManifestError(Resources.GetResourceString("EventSource_TypeMustDeriveFromEventSource"));
                    }
                    if (!eventSourceType.IsAbstract() && !eventSourceType.IsSealed())
                    {
                        manifest.ManifestError(Resources.GetResourceString("EventSource_TypeMustBeSealedOrAbstract"));
                    }
                }

                // Collect task, opcode, keyword and channel information
#if FEATURE_MANAGED_ETW_CHANNELS && FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                foreach (var providerEnumKind in new string[] { "Keywords", "Tasks", "Opcodes", "Channels" })
#else
                foreach (var providerEnumKind in new string[] { "Keywords", "Tasks", "Opcodes" })
#endif
                {
                    Type nestedType = eventSourceType.GetNestedType(providerEnumKind);
                    if (nestedType != null)
                    {
                        if (eventSourceType.IsAbstract())
                        {
                            manifest.ManifestError(Resources.GetResourceString("EventSource_AbstractMustNotDeclareKTOC", nestedType.Name));
                        }
                        else
                        {
                            foreach (FieldInfo staticField in nestedType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                            {
                                AddProviderEnumKind(manifest, staticField, providerEnumKind);
                            }
                        }
                    }
                }
                // ensure we have keywords for the session-filtering reserved bits
                {
                    manifest.AddKeyword("Session3", (long)0x1000 << 32);
                    manifest.AddKeyword("Session2", (long)0x2000 << 32);
                    manifest.AddKeyword("Session1", (long)0x4000 << 32);
                    manifest.AddKeyword("Session0", (long)0x8000 << 32);
                }

                if (eventSourceType != typeof(EventSource))
                {
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo method = methods[i];
                        ParameterInfo[] args = method.GetParameters();

                        // Get the EventDescriptor (from the Custom attributes)
                        EventAttribute eventAttribute = (EventAttribute)GetCustomAttributeHelper(method, typeof(EventAttribute), flags);

                        // Compat: until v4.5.1 we ignored any non-void returning methods as well as virtual methods for 
                        // the only reason of limiting the number of methods considered to be events. This broke a common 
                        // design of having event sources implement specific interfaces. To fix this in a compatible way
                        // we will now allow both non-void returning and virtual methods to be Event methods, as long 
                        // as they are marked with the [Event] attribute
                        if (/* method.IsVirtual || */ method.IsStatic)
                        {
                            continue;
                        }

                        if (eventSourceType.IsAbstract())
                        {
                            if (eventAttribute != null)
                            {
                                manifest.ManifestError(Resources.GetResourceString("EventSource_AbstractMustNotDeclareEventMethods", method.Name, eventAttribute.EventId));
                            }
                            continue;
                        }
                        else if (eventAttribute == null)
                        {
                            // Methods that don't return void can't be events, if they're NOT marked with [Event].
                            // (see Compat comment above)
                            if (method.ReturnType != typeof(void))
                            {
                                continue;
                            }

                            // Continue to ignore virtual methods if they do NOT have the [Event] attribute
                            // (see Compat comment above)
                            if (method.IsVirtual)
                            {
                                continue;
                            }

                            // If we explicitly mark the method as not being an event, then honor that.  
                            if (GetCustomAttributeHelper(method, typeof(NonEventAttribute), flags) != null)
                                continue;

                            defaultEventAttribute = new EventAttribute(eventId);
                            eventAttribute = defaultEventAttribute;
                        }
                        else if (eventAttribute.EventId <= 0)
                        {
                            manifest.ManifestError(Resources.GetResourceString("EventSource_NeedPositiveId", method.Name), true);
                            continue;   // don't validate anything else for this event
                        }
                        if (method.Name.LastIndexOf('.') >= 0)
                        {
                            manifest.ManifestError(Resources.GetResourceString("EventSource_EventMustNotBeExplicitImplementation", method.Name, eventAttribute.EventId));
                        }

                        eventId++;
                        string eventName = method.Name;

                        if (eventAttribute.Opcode == EventOpcode.Info)      // We are still using the default opcode. 
                        {
                            // By default pick a task ID derived from the EventID, starting with the highest task number and working back 
                            bool noTask = (eventAttribute.Task == EventTask.None);
                            if (noTask)
                                eventAttribute.Task = (EventTask)(0xFFFE - eventAttribute.EventId);

                            // Unless we explicitly set the opcode to Info (to override the auto-generate of Start or Stop opcodes, 
                            // pick a default opcode based on the event name (either Info or start or stop if the name ends with that suffix).  
                            if (!eventAttribute.IsOpcodeSet)
                                eventAttribute.Opcode = GetOpcodeWithDefault(EventOpcode.Info, eventName);

                            // Make the stop opcode have the same task as the start opcode.
                            if (noTask)
                            {
                                if (eventAttribute.Opcode == EventOpcode.Start)
                                {
                                    string taskName = eventName.Substring(0, eventName.Length - s_ActivityStartSuffix.Length); // Remove the Stop suffix to get the task name
                                    if (string.Compare(eventName, 0, taskName, 0, taskName.Length) == 0 &&
                                        string.Compare(eventName, taskName.Length, s_ActivityStartSuffix, 0, Math.Max(eventName.Length - taskName.Length, s_ActivityStartSuffix.Length)) == 0)
                                    {
                                        // Add a task that is just the task name for the start event.   This suppress the auto-task generation
                                        // That would otherwise happen (and create 'TaskName'Start as task name rather than just 'TaskName'
                                        manifest.AddTask(taskName, (int)eventAttribute.Task);
                                    }
                                }
                                else if (eventAttribute.Opcode == EventOpcode.Stop)
                                {
                                    // Find the start associated with this stop event.  We require start to be immediately before the stop
                                    int startEventId = eventAttribute.EventId - 1;
                                    if (eventData != null && startEventId < eventData.Length)
                                    {
                                        Debug.Assert(0 <= startEventId);                // Since we reserve id 0, we know that id-1 is <= 0
                                        EventMetadata startEventMetadata = eventData[startEventId];

                                        // If you remove the Stop and add a Start does that name match the Start Event's Name?
                                        // Ideally we would throw an error 
                                        string taskName = eventName.Substring(0, eventName.Length - s_ActivityStopSuffix.Length); // Remove the Stop suffix to get the task name
                                        if (startEventMetadata.Descriptor.Opcode == (byte)EventOpcode.Start &&
                                            string.Compare(startEventMetadata.Name, 0, taskName, 0, taskName.Length) == 0 &&
                                            string.Compare(startEventMetadata.Name, taskName.Length, s_ActivityStartSuffix, 0, Math.Max(startEventMetadata.Name.Length - taskName.Length, s_ActivityStartSuffix.Length)) == 0)
                                        {

                                            // Make the stop event match the start event
                                            eventAttribute.Task = (EventTask)startEventMetadata.Descriptor.Task;
                                            noTask = false;
                                        }
                                    }
                                    if (noTask && (flags & EventManifestOptions.Strict) != 0)        // Throw an error if we can compatibly.   
                                    {
                                        throw new ArgumentException(Resources.GetResourceString("EventSource_StopsFollowStarts"));
                                    }
                                }
                            }
                        }

                        bool hasRelatedActivityID = RemoveFirstArgIfRelatedActivityId(ref args);
                        if (!(source != null && source.SelfDescribingEvents))
                        {
                            manifest.StartEvent(eventName, eventAttribute);
                            for (int fieldIdx = 0; fieldIdx < args.Length; fieldIdx++)
                            {
                                manifest.AddEventParameter(args[fieldIdx].ParameterType, args[fieldIdx].Name);
                            }
                            manifest.EndEvent();
                        }

                        if (source != null || (flags & EventManifestOptions.Strict) != 0)
                        {
                            // Do checking for user errors (optional, but not a big deal so we do it).  
                            DebugCheckEvent(ref eventsByName, eventData, method, eventAttribute, manifest, flags);

#if FEATURE_MANAGED_ETW_CHANNELS
                            // add the channel keyword for Event Viewer channel based filters. This is added for creating the EventDescriptors only
                            // and is not required for the manifest
                            if (eventAttribute.Channel != EventChannel.None)
                            {
                                unchecked
                                {
                                    eventAttribute.Keywords |= (EventKeywords)manifest.GetChannelKeyword(eventAttribute.Channel, (ulong)eventAttribute.Keywords);
                                }
                            }
#endif
                            string eventKey = "event_" + eventName;
                            string msg = manifest.GetLocalizedMessage(eventKey, CultureInfo.CurrentUICulture, etwFormat: false);
                            // overwrite inline message with the localized message
                            if (msg != null) eventAttribute.Message = msg;

                            AddEventDescriptor(ref eventData, eventName, eventAttribute, args, hasRelatedActivityID);
                        }
                    }
                }

                // Tell the TraceLogging stuff where to start allocating its own IDs.  
                NameInfo.ReserveEventIDsBelow(eventId);

                if (source != null)
                {
                    TrimEventDescriptors(ref eventData);
                    source.m_eventData = eventData;     // officially initialize it. We do this at most once (it is racy otherwise). 
#if FEATURE_MANAGED_ETW_CHANNELS
                    source.m_channelData = manifest.GetChannelData();
#endif
                }

                // if this is an abstract event source we've already performed all the validation we can
                if (!eventSourceType.IsAbstract() && (source == null || !source.SelfDescribingEvents))
                {
                    bNeedsManifest = (flags & EventManifestOptions.OnlyIfNeededForRegistration) == 0
#if FEATURE_MANAGED_ETW_CHANNELS
                                            || manifest.GetChannelData().Length > 0
#endif
;

                    // if the manifest is not needed and we're not requested to validate the event source return early
                    if (!bNeedsManifest && (flags & EventManifestOptions.Strict) == 0)
                        return null;

                    res = manifest.CreateManifest();
                }
            }
            catch (Exception e)
            {
                // if this is a runtime manifest generation let the exception propagate
                if ((flags & EventManifestOptions.Strict) == 0)
                    throw;
                // else store it to include it in the Argument exception we raise below
                exception = e;
            }

            if ((flags & EventManifestOptions.Strict) != 0 && (manifest.Errors.Count > 0 || exception != null))
            {
                string msg = String.Empty;
                if (manifest.Errors.Count > 0)
                {
                    bool firstError = true;
                    foreach (string error in manifest.Errors)
                    {
                        if (!firstError)
                            msg += Environment.NewLine;
                        firstError = false;
                        msg += error;
                    }
                }
                else
                    msg = "Unexpected error: " + exception.Message;

                throw new ArgumentException(msg, exception);
            }

            return bNeedsManifest ? res : null;
        }

        private static bool RemoveFirstArgIfRelatedActivityId(ref ParameterInfo[] args)
        {
            // If the first parameter is (case insensitive) 'relatedActivityId' then skip it.  
            if (args.Length > 0 && args[0].ParameterType == typeof(Guid) &&
                string.Compare(args[0].Name, "relatedActivityId", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var newargs = new ParameterInfo[args.Length - 1];
                Array.Copy(args, 1, newargs, 0, args.Length - 1);
                args = newargs;

                return true;
            }

            return false;
        }

        // adds a enumeration (keyword, opcode, task or channel) represented by 'staticField'
        // to the manifest.  
        private static void AddProviderEnumKind(ManifestBuilder manifest, FieldInfo staticField, string providerEnumKind)
        {
            bool reflectionOnly = staticField.Module.Assembly.ReflectionOnly();
            Type staticFieldType = staticField.FieldType;
            if (!reflectionOnly && (staticFieldType == typeof(EventOpcode)) || AttributeTypeNamesMatch(staticFieldType, typeof(EventOpcode)))
            {
                if (providerEnumKind != "Opcodes") goto Error;
                int value = (int)staticField.GetRawConstantValue();
                manifest.AddOpcode(staticField.Name, value);
            }
            else if (!reflectionOnly && (staticFieldType == typeof(EventTask)) || AttributeTypeNamesMatch(staticFieldType, typeof(EventTask)))
            {
                if (providerEnumKind != "Tasks") goto Error;
                int value = (int)staticField.GetRawConstantValue();
                manifest.AddTask(staticField.Name, value);
            }
            else if (!reflectionOnly && (staticFieldType == typeof(EventKeywords)) || AttributeTypeNamesMatch(staticFieldType, typeof(EventKeywords)))
            {
                if (providerEnumKind != "Keywords") goto Error;
                ulong value = unchecked((ulong)(long)staticField.GetRawConstantValue());
                manifest.AddKeyword(staticField.Name, value);
            }
#if FEATURE_MANAGED_ETW_CHANNELS && FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
            else if (!reflectionOnly && (staticFieldType == typeof(EventChannel)) || AttributeTypeNamesMatch(staticFieldType, typeof(EventChannel)))
            {
                if (providerEnumKind != "Channels") goto Error;
                var channelAttribute = (EventChannelAttribute)GetCustomAttributeHelper(staticField, typeof(EventChannelAttribute));
                manifest.AddChannel(staticField.Name, (byte)staticField.GetRawConstantValue(), channelAttribute);
            }
#endif
            return;
            Error:
            manifest.ManifestError(Resources.GetResourceString("EventSource_EnumKindMismatch", staticField.Name, staticField.FieldType.Name, providerEnumKind));
        }

        // Helper used by code:CreateManifestAndDescriptors to add a code:EventData descriptor for a method
        // with the code:EventAttribute 'eventAttribute'.  resourceManger may be null in which case we populate it
        // it is populated if we need to look up message resources
        private static void AddEventDescriptor(ref EventMetadata[] eventData, string eventName,
                                EventAttribute eventAttribute, ParameterInfo[] eventParameters,
                                bool hasRelatedActivityID)
        {
            if (eventData == null || eventData.Length <= eventAttribute.EventId)
            {
                EventMetadata[] newValues = new EventMetadata[Math.Max(eventData.Length + 16, eventAttribute.EventId + 1)];
                Array.Copy(eventData, 0, newValues, 0, eventData.Length);
                eventData = newValues;
            }

            eventData[eventAttribute.EventId].Descriptor = new EventDescriptor(
                    eventAttribute.EventId,
                    eventAttribute.Version,
#if FEATURE_MANAGED_ETW_CHANNELS
                    (byte)eventAttribute.Channel,
#else
                    (byte)0,
#endif
                    (byte)eventAttribute.Level,
                    (byte)eventAttribute.Opcode,
                    (int)eventAttribute.Task,
                    unchecked((long)((ulong)eventAttribute.Keywords | SessionMask.All.ToEventKeywords())));

            eventData[eventAttribute.EventId].Tags = eventAttribute.Tags;
            eventData[eventAttribute.EventId].Name = eventName;
            eventData[eventAttribute.EventId].Parameters = eventParameters;
            eventData[eventAttribute.EventId].Message = eventAttribute.Message;
            eventData[eventAttribute.EventId].ActivityOptions = eventAttribute.ActivityOptions;
            eventData[eventAttribute.EventId].HasRelatedActivityID = hasRelatedActivityID;
            eventData[eventAttribute.EventId].EventHandle = IntPtr.Zero;
        }

        // Helper used by code:CreateManifestAndDescriptors that trims the m_eventData array to the correct
        // size after all event descriptors have been added. 
        private static void TrimEventDescriptors(ref EventMetadata[] eventData)
        {
            int idx = eventData.Length;
            while (0 < idx)
            {
                --idx;
                if (eventData[idx].Descriptor.EventId != 0)
                    break;
            }
            if (eventData.Length - idx > 2)      // allow one wasted slot. 
            {
                EventMetadata[] newValues = new EventMetadata[idx + 1];
                Array.Copy(eventData, 0, newValues, 0, newValues.Length);
                eventData = newValues;
            }
        }

        // Helper used by code:EventListener.AddEventSource and code:EventListener.EventListener
        // when a listener gets attached to a eventSource
        internal void AddListener(EventListener listener)
        {
            lock (EventListener.EventListenersLock)
            {
                bool[] enabledArray = null;
                if (m_eventData != null)
                    enabledArray = new bool[m_eventData.Length];
                m_Dispatchers = new EventDispatcher(m_Dispatchers, enabledArray, listener);
                listener.OnEventSourceCreated(this);
            }
        }

        // Helper used by code:CreateManifestAndDescriptors to find user mistakes like reusing an event
        // index for two distinct events etc.  Throws exceptions when it finds something wrong. 
        private static void DebugCheckEvent(ref Dictionary<string, string> eventsByName,
            EventMetadata[] eventData, MethodInfo method, EventAttribute eventAttribute,
            ManifestBuilder manifest, EventManifestOptions options)
        {
            int evtId = eventAttribute.EventId;
            string evtName = method.Name;
            int eventArg = GetHelperCallFirstArg(method);
            if (eventArg >= 0 && evtId != eventArg)
            {
                manifest.ManifestError(Resources.GetResourceString("EventSource_MismatchIdToWriteEvent", evtName, evtId, eventArg), true);
            }

            if (evtId < eventData.Length && eventData[evtId].Descriptor.EventId != 0)
            {
                manifest.ManifestError(Resources.GetResourceString("EventSource_EventIdReused", evtName, evtId, eventData[evtId].Name), true);
            }

            // We give a task to things if they don't have one.  
            // TODO this is moderately expensive (N*N).   We probably should not even bother....   
            Debug.Assert(eventAttribute.Task != EventTask.None || eventAttribute.Opcode != EventOpcode.Info);
            for (int idx = 0; idx < eventData.Length; ++idx)
            {
                // skip unused Event IDs. 
                if (eventData[idx].Name == null)
                    continue;

                if (eventData[idx].Descriptor.Task == (int)eventAttribute.Task && eventData[idx].Descriptor.Opcode == (int)eventAttribute.Opcode)
                {
                    manifest.ManifestError(Resources.GetResourceString("EventSource_TaskOpcodePairReused",
                                            evtName, evtId, eventData[idx].Name, idx));
                    // If we are not strict stop on first error.   We have had problems with really large providers taking forever.  because of many errors.  
                    if ((options & EventManifestOptions.Strict) == 0)
                        break;
                }
            }

            // for non-default event opcodes the user must define a task!
            if (eventAttribute.Opcode != EventOpcode.Info)
            {
                bool failure = false;
                if (eventAttribute.Task == EventTask.None)
                    failure = true;
                else
                {
                    // If you have the auto-assigned Task, then you did not explicitly set one.  
                    // This is OK for Start events because we have special logic to assign the task to a prefix derived from the event name
                    // But all other cases we want to catch the omission.  
                    var autoAssignedTask = (EventTask)(0xFFFE - evtId);
                    if ((eventAttribute.Opcode != EventOpcode.Start && eventAttribute.Opcode != EventOpcode.Stop) && eventAttribute.Task == autoAssignedTask)
                        failure = true;
                }
                if (failure)
                {
                    manifest.ManifestError(Resources.GetResourceString("EventSource_EventMustHaveTaskIfNonDefaultOpcode", evtName, evtId));
                }
            }

            // If we ever want to enforce the rule: MethodName = TaskName + OpcodeName here's how:
            //  (the reason we don't is backwards compat and the need for handling this as a non-fatal error
            //  by eventRegister.exe)
            // taskName & opcodeName could be passed in by the caller which has opTab & taskTab handy
            // if (!(((int)eventAttribute.Opcode == 0 && evtName == taskName) || (evtName == taskName+opcodeName)))
            // {
            //     throw new WarningException(Resources.GetResourceString("EventSource_EventNameDoesNotEqualTaskPlusOpcode"));
            // }

            if (eventsByName == null)
                eventsByName = new Dictionary<string, string>();

            if (eventsByName.ContainsKey(evtName))
            {
                manifest.ManifestError(Resources.GetResourceString("EventSource_EventNameReused", evtName), true);
            }

            eventsByName[evtName] = evtName;
        }

        /// <summary>
        /// This method looks at the IL and tries to pattern match against the standard
        /// 'boilerplate' event body 
        /// <code>
        ///     { if (Enabled()) WriteEvent(#, ...) } 
        /// </code>
        /// If the pattern matches, it returns the literal number passed as the first parameter to
        /// the WriteEvent.  This is used to find common user errors (mismatching this
        /// number with the EventAttribute ID).  It is only used for validation.   
        /// </summary>
        /// <param name="method">The method to probe.</param>
        /// <returns>The literal value or -1 if the value could not be determined. </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Switch statement is clearer than alternatives")]
        static private int GetHelperCallFirstArg(MethodInfo method)
        {
#if (!ES_BUILD_PCL && !ES_BUILD_PN)
            // Currently searches for the following pattern
            // 
            // ...     // CAN ONLY BE THE INSTRUCTIONS BELOW
            // LDARG0
            // LDC.I4 XXX
            // ...     // CAN ONLY BE THE INSTRUCTIONS BELOW CAN'T BE A BRANCH OR A CALL
            // CALL 
            // NOP     // 0 or more times
            // RET
            // 
            // If we find this pattern we return the XXX.  Otherwise we return -1.  
#if !CORECLR
            (new ReflectionPermission(ReflectionPermissionFlag.MemberAccess)).Assert();
#endif
            byte[] instrs = method.GetMethodBody().GetILAsByteArray();
            int retVal = -1;
            for (int idx = 0; idx < instrs.Length;)
            {
                switch (instrs[idx])
                {
                    case 0: // NOP
                    case 1: // BREAK
                    case 2: // LDARG_0
                    case 3: // LDARG_1
                    case 4: // LDARG_2
                    case 5: // LDARG_3
                    case 6: // LDLOC_0
                    case 7: // LDLOC_1
                    case 8: // LDLOC_2
                    case 9: // LDLOC_3
                    case 10: // STLOC_0
                    case 11: // STLOC_1
                    case 12: // STLOC_2
                    case 13: // STLOC_3
                        break;
                    case 14: // LDARG_S
                    case 16: // STARG_S
                        idx++;
                        break;
                    case 20: // LDNULL
                        break;
                    case 21: // LDC_I4_M1
                    case 22: // LDC_I4_0
                    case 23: // LDC_I4_1
                    case 24: // LDC_I4_2
                    case 25: // LDC_I4_3
                    case 26: // LDC_I4_4
                    case 27: // LDC_I4_5
                    case 28: // LDC_I4_6
                    case 29: // LDC_I4_7
                    case 30: // LDC_I4_8
                        if (idx > 0 && instrs[idx - 1] == 2)  // preceeded by LDARG0
                            retVal = instrs[idx] - 22;
                        break;
                    case 31: // LDC_I4_S
                        if (idx > 0 && instrs[idx - 1] == 2)  // preceeded by LDARG0
                            retVal = instrs[idx + 1];
                        idx++;
                        break;
                    case 32: // LDC_I4
                        idx += 4;
                        break;
                    case 37: // DUP
                        break;
                    case 40: // CALL
                        idx += 4;

                        if (retVal >= 0)
                        {
                            // Is this call just before return?  
                            for (int search = idx + 1; search < instrs.Length; search++)
                            {
                                if (instrs[search] == 42)  // RET
                                    return retVal;
                                if (instrs[search] != 0)   // NOP
                                    break;
                            }
                        }
                        retVal = -1;
                        break;
                    case 44: // BRFALSE_S
                    case 45: // BRTRUE_S
                        retVal = -1;
                        idx++;
                        break;
                    case 57: // BRFALSE
                    case 58: // BRTRUE
                        retVal = -1;
                        idx += 4;
                        break;
                    case 103: // CONV_I1
                    case 104: // CONV_I2
                    case 105: // CONV_I4
                    case 106: // CONV_I8
                    case 109: // CONV_U4
                    case 110: // CONV_U8
                        break;
                    case 140: // BOX
                    case 141: // NEWARR
                        idx += 4;
                        break;
                    case 162: // STELEM_REF
                        break;
                    case 254: // PREFIX
                        idx++;
                        // Covers the CEQ instructions used in debug code for some reason.
                        if (idx >= instrs.Length || instrs[idx] >= 6)
                            goto default;
                        break;
                    default:
                        /* Debug.Assert(false, "Warning: User validation code sub-optimial: Unsuported opcode " + instrs[idx] +
                            " at " + idx + " in method " + method.Name); */
                        return -1;
                }
                idx++;
            }
#endif
            return -1;
        }

#if false // This routine is not needed at all, it was used for unit test debugging. 
        [Conditional("DEBUG")]
        private static void OutputDebugString(string msg)
        {
#if !ES_BUILD_PCL
            msg = msg.TrimEnd('\r', '\n') +
                    string.Format(CultureInfo.InvariantCulture, ", Thrd({0})" + Environment.NewLine, Thread.CurrentThread.ManagedThreadId);
            System.Diagnostics.Debugger.Log(0, null, msg);
#endif
        }
#endif

        /// <summary>
        /// Sends an error message to the debugger (outputDebugString), as well as the EventListeners 
        /// It will do this even if the EventSource is not enabled.  
        /// TODO remove flush parameter it is not used.  
        /// </summary>
        [SuppressMessage("Microsoft.Concurrency", "CA8001", Justification = "This does not need to be correct when racing with other threads")]
        internal void ReportOutOfBandMessage(string msg, bool flush)
        {
            try
            {
#if (!ES_BUILD_PCL && !ES_BUILD_PN)
                // send message to debugger without delay
                System.Diagnostics.Debugger.Log(0, null, String.Format("EventSource Error: {0}{1}", msg, Environment.NewLine));
#endif

                // Send it to all listeners.
                if (m_outOfBandMessageCount < 16 - 1)     // Note this is only if size byte
                    m_outOfBandMessageCount++;
                else
                {
                    if (m_outOfBandMessageCount == 16)
                        return;
                    m_outOfBandMessageCount = 16;    // Mark that we hit the limit.  Notify them that this is the case. 
                    msg = "Reached message limit.   End of EventSource error messages.";
                }

                WriteEventString(EventLevel.LogAlways, -1, msg);
                WriteStringToAllListeners("EventSourceMessage", msg);
            }
            catch (Exception) { }      // If we fail during last chance logging, well, we have to give up....
        }

        private EventSourceSettings ValidateSettings(EventSourceSettings settings)
        {
            var evtFormatMask = EventSourceSettings.EtwManifestEventFormat |
                                EventSourceSettings.EtwSelfDescribingEventFormat;
            if ((settings & evtFormatMask) == evtFormatMask)
            {
                throw new ArgumentException(Resources.GetResourceString("EventSource_InvalidEventFormat"), nameof(settings));
            }

            // If you did not explicitly ask for manifest, you get self-describing.  
            if ((settings & evtFormatMask) == 0)
                settings |= EventSourceSettings.EtwSelfDescribingEventFormat;
            return settings;
        }

        private bool ThrowOnEventWriteErrors
        {
            get { return (m_config & EventSourceSettings.ThrowOnEventWriteErrors) != 0; }
            set
            {
                if (value) m_config |= EventSourceSettings.ThrowOnEventWriteErrors;
                else m_config &= ~EventSourceSettings.ThrowOnEventWriteErrors;
            }
        }

        private bool SelfDescribingEvents
        {
            get
            {
                Debug.Assert(((m_config & EventSourceSettings.EtwManifestEventFormat) != 0) !=
                                ((m_config & EventSourceSettings.EtwSelfDescribingEventFormat) != 0));
                return (m_config & EventSourceSettings.EtwSelfDescribingEventFormat) != 0;
            }
            set
            {
                if (!value)
                {
                    m_config |= EventSourceSettings.EtwManifestEventFormat;
                    m_config &= ~EventSourceSettings.EtwSelfDescribingEventFormat;
                }
                else
                {
                    m_config |= EventSourceSettings.EtwSelfDescribingEventFormat;
                    m_config &= ~EventSourceSettings.EtwManifestEventFormat;
                }
            }
        }

#if FEATURE_ACTIVITYSAMPLING
        private void ReportActivitySamplingInfo(EventListener listener, SessionMask sessions)
        {
            Debug.Assert(listener == null || (uint)sessions == (uint)SessionMask.FromId(0));

            for (int perEventSourceSessionId = 0; perEventSourceSessionId < SessionMask.MAX; ++perEventSourceSessionId)
            {
                if (!sessions[perEventSourceSessionId])
                    continue;

                ActivityFilter af;
                if (listener == null)
                {
                    EtwSession etwSession = m_etwSessionIdMap[perEventSourceSessionId];
                    Debug.Assert(etwSession != null);
                    af = etwSession.m_activityFilter;
                }
                else
                {
                    af = listener.m_activityFilter;
                }

                if (af == null)
                    continue;

                SessionMask m = new SessionMask();
                m[perEventSourceSessionId] = true;

                foreach (var t in af.GetFilterAsTuple(m_guid))
                {
                    WriteStringToListener(listener, string.Format(CultureInfo.InvariantCulture, "Session {0}: {1} = {2}", perEventSourceSessionId, t.Item1, t.Item2), m);
                }

                bool participateInSampling = (listener == null) ?
                                               m_activityFilteringForETWEnabled[perEventSourceSessionId] :
                                               GetDispatcher(listener).m_activityFilteringEnabled;
                WriteStringToListener(listener, string.Format(CultureInfo.InvariantCulture, "Session {0}: Activity Sampling support: {1}",
                                          perEventSourceSessionId, participateInSampling ? "enabled" : "disabled"), m);
            }
        }
#endif // FEATURE_ACTIVITYSAMPLING

        // private instance state 
        private string m_name;                          // My friendly name (privided in ctor)
        internal int m_id;                              // A small integer that is unique to this instance.  
        private Guid m_guid;                            // GUID representing the ETW eventSource to the OS.  
        internal volatile EventMetadata[] m_eventData;  // None per-event data
        private volatile byte[] m_rawManifest;          // Bytes to send out representing the event schema

        private EventHandler<EventCommandEventArgs> m_eventCommandExecuted;

        private EventSourceSettings m_config;      // configuration information

        private bool m_eventSourceDisposed;              // has Dispose been called.

        // Enabling bits
        private bool m_eventSourceEnabled;              // am I enabled (any of my events are enabled for any dispatcher)
        internal EventLevel m_level;                    // highest level enabled by any output dispatcher
        internal EventKeywords m_matchAnyKeyword;       // the logical OR of all levels enabled by any output dispatcher (zero is a special case) meaning 'all keywords'

        // Dispatching state 
        internal volatile EventDispatcher m_Dispatchers;    // Linked list of code:EventDispatchers we write the data to (we also do ETW specially)
#if FEATURE_MANAGED_ETW
        private volatile OverideEventProvider m_provider;   // This hooks up ETW commands to our 'OnEventCommand' callback
#endif
        private bool m_completelyInited;                // The EventSource constructor has returned without exception.   
        private Exception m_constructionException;      // If there was an exception construction, this is it 
        private byte m_outOfBandMessageCount;           // The number of out of band messages sent (we throttle them
        private EventCommandEventArgs m_deferredCommands;// If we get commands before we are fully we store them here and run the when we are fully inited.  

        private string[] m_traits;                      // Used to implement GetTraits

        internal static uint s_currentPid;              // current process id, used in synthesizing quasi-GUIDs
        [ThreadStatic]
        private static byte m_EventSourceExceptionRecurenceCount = 0; // current recursion count inside ThrowEventSourceException

        [ThreadStatic]
        private static bool m_EventSourceInDecodeObject = false;

#if FEATURE_MANAGED_ETW_CHANNELS
        internal volatile ulong[] m_channelData;
#endif

#if FEATURE_ACTIVITYSAMPLING
        private SessionMask m_curLiveSessions;          // the activity-tracing aware sessions' bits
        private EtwSession[] m_etwSessionIdMap;         // the activity-tracing aware sessions
        private List<EtwSession> m_legacySessions;      // the legacy ETW sessions listening to this source
        internal long m_keywordTriggers;                // a bit is set if it corresponds to a keyword that's part of an enabled triggering event
        internal SessionMask m_activityFilteringForETWEnabled; // does THIS EventSource have activity filtering turned on for each ETW session
        static internal Action<Guid> s_activityDying;   // Fires when something calls SetCurrentThreadToActivity()
        // Also used to mark that activity tracing is on for some case
#endif // FEATURE_ACTIVITYSAMPLING

        // We use a single instance of ActivityTracker for all EventSources instances to allow correlation between multiple event providers.
        // We have m_activityTracker field simply because instance field is more efficient than static field fetch.
        ActivityTracker m_activityTracker;
        internal const string s_ActivityStartSuffix = "Start";
        internal const string s_ActivityStopSuffix = "Stop";

        // used for generating GUID from eventsource name
        private static readonly byte[] namespaceBytes = new byte[] {
            0x48, 0x2C, 0x2D, 0xB2, 0xC3, 0x90, 0x47, 0xC8,
            0x87, 0xF8, 0x1A, 0x15, 0xBF, 0xC1, 0x30, 0xFB,
        };

        #endregion
    }

    /// <summary>
    /// Enables specifying event source configuration options to be used in the EventSource constructor.
    /// </summary>
    [Flags]
    public enum EventSourceSettings
    {
        /// <summary>
        /// This specifies none of the special configuration options should be enabled.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Normally an EventSource NEVER throws; setting this option will tell it to throw when it encounters errors.  
        /// </summary>
        ThrowOnEventWriteErrors = 1,
        /// <summary>
        /// Setting this option is a directive to the ETW listener should use manifest-based format when
        /// firing events. This is the default option when defining a type derived from EventSource 
        /// (using the protected EventSource constructors).
        /// Only one of EtwManifestEventFormat or EtwSelfDescribingEventFormat should be specified
        /// </summary>
        EtwManifestEventFormat = 4,
        /// <summary>
        /// Setting this option is a directive to the ETW listener should use self-describing event format
        /// when firing events. This is the default option when creating a new instance of the EventSource
        /// type (using the public EventSource constructors).  
        /// Only one of EtwManifestEventFormat or EtwSelfDescribingEventFormat should be specified
        /// </summary>
        EtwSelfDescribingEventFormat = 8,
    }

    /// <summary>
    /// An EventListener represents a target for the events generated by EventSources (that is subclasses
    /// of <see cref="EventSource"/>), in the current appdomain. When a new EventListener is created
    /// it is logically attached to all eventSources in that appdomain. When the EventListener is Disposed, then
    /// it is disconnected from the event eventSources. Note that there is a internal list of STRONG references
    /// to EventListeners, which means that relying on the lack of references to EventListeners to clean up
    /// EventListeners will NOT work. You must call EventListener.Dispose explicitly when a dispatcher is no
    /// longer needed.
    /// <para>
    /// Once created, EventListeners can enable or disable on a per-eventSource basis using verbosity levels
    /// (<see cref="EventLevel"/>) and bitfields (<see cref="EventKeywords"/>) to further restrict the set of 
    /// events to be sent to the dispatcher. The dispatcher can also send arbitrary commands to a particular 
    /// eventSource using the 'SendCommand' method. The meaning of the commands are eventSource specific.
    /// </para><para>
    /// The Null Guid (that is (new Guid()) has special meaning as a wildcard for 'all current eventSources in
    /// the appdomain'. Thus it is relatively easy to turn on all events in the appdomain if desired.
    /// </para><para>
    /// It is possible for there to be many EventListener's defined in a single appdomain. Each dispatcher is
    /// logically independent of the other listeners. Thus when one dispatcher enables or disables events, it
    /// affects only that dispatcher (other listeners get the events they asked for). It is possible that
    /// commands sent with 'SendCommand' would do a semantic operation that would affect the other listeners
    /// (like doing a GC, or flushing data ...), but this is the exception rather than the rule.
    /// </para><para>
    /// Thus the model is that each EventSource keeps a list of EventListeners that it is sending events
    /// to. Associated with each EventSource-dispatcher pair is a set of filtering criteria that determine for
    /// that eventSource what events that dispatcher will receive.
    /// </para><para>
    /// Listeners receive the events on their 'OnEventWritten' method. Thus subclasses of EventListener must
    /// override this method to do something useful with the data.
    /// </para><para>
    /// In addition, when new eventSources are created, the 'OnEventSourceCreate' method is called. The
    /// invariant associated with this callback is that every eventSource gets exactly one
    /// 'OnEventSourceCreate' call for ever eventSource that can potentially send it log messages. In
    /// particular when a EventListener is created, typically a series of OnEventSourceCreate' calls are
    /// made to notify the new dispatcher of all the eventSources that existed before the EventListener was
    /// created.
    /// </para>
    /// </summary>
    public class EventListener : IDisposable
    {
        private event EventHandler<EventSourceCreatedEventArgs> _EventSourceCreated;

        /// <summary>
        /// This event is raised whenever a new eventSource is 'attached' to the dispatcher.
        /// This can happen for all existing EventSources when the EventListener is created
        /// as well as for any EventSources that come into existence after the EventListener
        /// has been created.
        /// 
        /// These 'catch up' events are called during the construction of the EventListener.
        /// Subclasses need to be prepared for that.
        /// 
        /// In a multi-threaded environment, it is possible that 'EventSourceEventWrittenCallback' 
        /// events for a particular eventSource to occur BEFORE the EventSourceCreatedCallback is issued.
        /// </summary>
        public event EventHandler<EventSourceCreatedEventArgs> EventSourceCreated
        {
            add
            {
                CallBackForExistingEventSources(false, value);

                this._EventSourceCreated = (EventHandler<EventSourceCreatedEventArgs>)Delegate.Combine(_EventSourceCreated, value);
            }
            remove
            {
                this._EventSourceCreated = (EventHandler<EventSourceCreatedEventArgs>)Delegate.Remove(_EventSourceCreated, value);
            }
        }

        /// <summary>
        /// This event is raised whenever an event has been written by a EventSource for which 
        /// the EventListener has enabled events.  
        /// </summary>
        public event EventHandler<EventWrittenEventArgs> EventWritten;

        /// <summary>
        /// Create a new EventListener in which all events start off turned off (use EnableEvents to turn
        /// them on).  
        /// </summary>
        public EventListener()
        {
            // This will cause the OnEventSourceCreated callback to fire. 
            CallBackForExistingEventSources(true, (obj, args) => args.EventSource.AddListener((EventListener)obj));
        }

        /// <summary>
        /// Dispose should be called when the EventListener no longer desires 'OnEvent*' callbacks. Because
        /// there is an internal list of strong references to all EventListeners, calling 'Dispose' directly
        /// is the only way to actually make the listen die. Thus it is important that users of EventListener
        /// call Dispose when they are done with their logging.
        /// </summary>
#if ES_BUILD_STANDALONE
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
#endif
        public virtual void Dispose()
        {
            lock (EventListenersLock)
            {
                if (s_Listeners != null)
                {
                    if (this == s_Listeners)
                    {
                        EventListener cur = s_Listeners;
                        s_Listeners = this.m_Next;
                        RemoveReferencesToListenerInEventSources(cur);
                    }
                    else
                    {
                        // Find 'this' from the s_Listeners linked list.  
                        EventListener prev = s_Listeners;
                        for (;;)
                        {
                            EventListener cur = prev.m_Next;
                            if (cur == null)
                                break;
                            if (cur == this)
                            {
                                // Found our Listener, remove references to to it in the eventSources
                                prev.m_Next = cur.m_Next;       // Remove entry. 
                                RemoveReferencesToListenerInEventSources(cur);
                                break;
                            }
                            prev = cur;
                        }
                    }
                }
                Validate();
            }
        }
        // We don't expose a Dispose(bool), because the contract is that you don't have any non-syncronous
        // 'cleanup' associated with this object

        /// <summary>
        /// Enable all events from the eventSource identified by 'eventSource' to the current 
        /// dispatcher that have a verbosity level of 'level' or lower.
        ///   
        /// This call can have the effect of REDUCING the number of events sent to the 
        /// dispatcher if 'level' indicates a less verbose level than was previously enabled.
        /// 
        /// This call never has an effect on other EventListeners.
        ///
        /// </summary>
        public void EnableEvents(EventSource eventSource, EventLevel level)
        {
            EnableEvents(eventSource, level, EventKeywords.None);
        }
        /// <summary>
        /// Enable all events from the eventSource identified by 'eventSource' to the current
        /// dispatcher that have a verbosity level of 'level' or lower and have a event keyword
        /// matching any of the bits in 'matchAnyKeyword'.
        /// 
        /// This call can have the effect of REDUCING the number of events sent to the 
        /// dispatcher if 'level' indicates a less verbose level than was previously enabled or
        /// if 'matchAnyKeyword' has fewer keywords set than where previously set.
        /// 
        /// This call never has an effect on other EventListeners.
        /// </summary>
        public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword)
        {
            EnableEvents(eventSource, level, matchAnyKeyword, null);
        }
        /// <summary>
        /// Enable all events from the eventSource identified by 'eventSource' to the current
        /// dispatcher that have a verbosity level of 'level' or lower and have a event keyword
        /// matching any of the bits in 'matchAnyKeyword' as well as any (eventSource specific)
        /// effect passing additional 'key-value' arguments 'arguments' might have.  
        /// 
        /// This call can have the effect of REDUCING the number of events sent to the 
        /// dispatcher if 'level' indicates a less verbose level than was previously enabled or
        /// if 'matchAnyKeyword' has fewer keywords set than where previously set.
        /// 
        /// This call never has an effect on other EventListeners.
        /// </summary>       
        public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword, IDictionary<string, string> arguments)
        {
            if (eventSource == null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }
            Contract.EndContractBlock();

            eventSource.SendCommand(this, 0, 0, EventCommand.Update, true, level, matchAnyKeyword, arguments);
        }
        /// <summary>
        /// Disables all events coming from eventSource identified by 'eventSource'.  
        /// 
        /// This call never has an effect on other EventListeners.      
        /// </summary>
        public void DisableEvents(EventSource eventSource)
        {
            if (eventSource == null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }
            Contract.EndContractBlock();

            eventSource.SendCommand(this, 0, 0, EventCommand.Update, false, EventLevel.LogAlways, EventKeywords.None, null);
        }

        /// <summary>
        /// EventSourceIndex is small non-negative integer (suitable for indexing in an array)
        /// identifying EventSource. It is unique per-appdomain. Some EventListeners might find
        /// it useful to store additional information about each eventSource connected to it,
        /// and EventSourceIndex allows this extra information to be efficiently stored in a
        /// (growable) array (eg List(T)).
        /// </summary>
        public static int EventSourceIndex(EventSource eventSource) { return eventSource.m_id; }

        /// <summary>
        /// This method is called whenever a new eventSource is 'attached' to the dispatcher.
        /// This can happen for all existing EventSources when the EventListener is created
        /// as well as for any EventSources that come into existence after the EventListener
        /// has been created.
        /// 
        /// These 'catch up' events are called during the construction of the EventListener.
        /// Subclasses need to be prepared for that.
        /// 
        /// In a multi-threaded environment, it is possible that 'OnEventWritten' callbacks
        /// for a particular eventSource to occur BEFORE the OnEventSourceCreated is issued.
        /// </summary>
        /// <param name="eventSource"></param>
        internal protected virtual void OnEventSourceCreated(EventSource eventSource)
        {
            EventHandler<EventSourceCreatedEventArgs> callBack = this._EventSourceCreated;
            if (callBack != null)
            {
                EventSourceCreatedEventArgs args = new EventSourceCreatedEventArgs();
                args.EventSource = eventSource;
                callBack(this, args);
            }
        }

        /// <summary>
        /// This method is called whenever an event has been written by a EventSource for which 
        /// the EventListener has enabled events.  
        /// </summary>
        /// <param name="eventData"></param>
        internal protected virtual void OnEventWritten(EventWrittenEventArgs eventData)
        {
            EventHandler<EventWrittenEventArgs> callBack = this.EventWritten;
            if (callBack != null)
            {
                callBack(this, eventData);
            }
        }


        #region private
        /// <summary>
        /// This routine adds newEventSource to the global list of eventSources, it also assigns the
        /// ID to the eventSource (which is simply the ordinal in the global list).
        /// 
        /// EventSources currently do not pro-actively remove themselves from this list. Instead
        /// when eventSources's are GCed, the weak handle in this list naturally gets nulled, and
        /// we will reuse the slot. Today this list never shrinks (but we do reuse entries
        /// that are in the list). This seems OK since the expectation is that EventSources
        /// tend to live for the lifetime of the appdomain anyway (they tend to be used in
        /// global variables).
        /// </summary>
        /// <param name="newEventSource"></param>
        internal static void AddEventSource(EventSource newEventSource)
        {
            lock (EventListenersLock)
            {
                if (s_EventSources == null)
                    s_EventSources = new List<WeakReference>(2);

                if (!s_EventSourceShutdownRegistered)
                {
                    s_EventSourceShutdownRegistered = true;
                }


                // Periodically search the list for existing entries to reuse, this avoids
                // unbounded memory use if we keep recycling eventSources (an unlikely thing). 
                int newIndex = -1;
                if (s_EventSources.Count % 64 == 63)   // on every block of 64, fill up the block before continuing
                {
                    int i = s_EventSources.Count;      // Work from the top down. 
                    while (0 < i)
                    {
                        --i;
                        WeakReference weakRef = s_EventSources[i];
                        if (!weakRef.IsAlive)
                        {
                            newIndex = i;
                            weakRef.Target = newEventSource;
                            break;
                        }
                    }
                }
                if (newIndex < 0)
                {
                    newIndex = s_EventSources.Count;
                    s_EventSources.Add(new WeakReference(newEventSource));
                }
                newEventSource.m_id = newIndex;

                // Add every existing dispatcher to the new EventSource
                for (EventListener listener = s_Listeners; listener != null; listener = listener.m_Next)
                    newEventSource.AddListener(listener);

                Validate();
            }
        }

        // Whenver we have async callbacks from native code, there is an ugly issue where
        // during .NET shutdown native code could be calling the callback, but the CLR
        // has already prohibited callbacks to managed code in the appdomain, causing the CLR 
        // to throw a COMPLUS_BOOT_EXCEPTION.   The guideline we give is that you must unregister
        // such callbacks on process shutdown or appdomain so that unmanaged code will never 
        // do this.  This is what this callback is for.  
        // See bug 724140 for more
        private static void DisposeOnShutdown(object sender, EventArgs e)
        {
            lock (EventListenersLock)
            {
                foreach (var esRef in s_EventSources)
                {
                    EventSource es = esRef.Target as EventSource;
                    if (es != null)
                        es.Dispose();
                }
            }
        }

        /// <summary>
        /// Helper used in code:Dispose that removes any references to 'listenerToRemove' in any of the
        /// eventSources in the appdomain.  
        /// 
        /// The EventListenersLock must be held before calling this routine. 
        /// </summary>
        private static void RemoveReferencesToListenerInEventSources(EventListener listenerToRemove)
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(Monitor.IsEntered(EventListener.EventListenersLock));
#endif
            // Foreach existing EventSource in the appdomain
            foreach (WeakReference eventSourceRef in s_EventSources)
            {
                EventSource eventSource = eventSourceRef.Target as EventSource;
                if (eventSource != null)
                {
                    // Is the first output dispatcher the dispatcher we are removing?
                    if (eventSource.m_Dispatchers.m_Listener == listenerToRemove)
                        eventSource.m_Dispatchers = eventSource.m_Dispatchers.m_Next;
                    else
                    {
                        // Remove 'listenerToRemove' from the eventSource.m_Dispatchers linked list.  
                        EventDispatcher prev = eventSource.m_Dispatchers;
                        for (;;)
                        {
                            EventDispatcher cur = prev.m_Next;
                            if (cur == null)
                            {
                                Debug.Assert(false, "EventSource did not have a registered EventListener!");
                                break;
                            }
                            if (cur.m_Listener == listenerToRemove)
                            {
                                prev.m_Next = cur.m_Next;       // Remove entry. 
                                break;
                            }
                            prev = cur;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks internal consistency of EventSources/Listeners. 
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Validate()
        {
            lock (EventListenersLock)
            {
                // Get all listeners 
                Dictionary<EventListener, bool> allListeners = new Dictionary<EventListener, bool>();
                EventListener cur = s_Listeners;
                while (cur != null)
                {
                    allListeners.Add(cur, true);
                    cur = cur.m_Next;
                }

                // For all eventSources 
                int id = -1;
                foreach (WeakReference eventSourceRef in s_EventSources)
                {
                    id++;
                    EventSource eventSource = eventSourceRef.Target as EventSource;
                    if (eventSource == null)
                        continue;
                    Debug.Assert(eventSource.m_id == id, "Unexpected event source ID.");

                    // None listeners on eventSources exist in the dispatcher list.   
                    EventDispatcher dispatcher = eventSource.m_Dispatchers;
                    while (dispatcher != null)
                    {
                        Debug.Assert(allListeners.ContainsKey(dispatcher.m_Listener), "EventSource has a listener not on the global list.");
                        dispatcher = dispatcher.m_Next;
                    }

                    // Every dispatcher is on Dispatcher List of every eventSource. 
                    foreach (EventListener listener in allListeners.Keys)
                    {
                        dispatcher = eventSource.m_Dispatchers;
                        for (;;)
                        {
                            Debug.Assert(dispatcher != null, "Listener is not on all eventSources.");
                            if (dispatcher.m_Listener == listener)
                                break;
                            dispatcher = dispatcher.m_Next;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a global lock that is intended to protect the code:s_Listeners linked list and the
        /// code:s_EventSources WeakReference list.  (We happen to use the s_EventSources list as
        /// the lock object)
        /// </summary>
        internal static object EventListenersLock
        {
            get
            {
                if (s_EventSources == null)
                    Interlocked.CompareExchange(ref s_EventSources, new List<WeakReference>(2), null);
                return s_EventSources;
            }
        }

        private void CallBackForExistingEventSources(bool addToListenersList, EventHandler<EventSourceCreatedEventArgs> callback)
        {
            lock (EventListenersLock)
            {
                // Disallow creating EventListener reentrancy. 
                if (s_CreatingListener)
                {
                    throw new InvalidOperationException(Resources.GetResourceString("EventSource_ListenerCreatedInsideCallback"));
                }

                try
                {
                    s_CreatingListener = true;

                    if (addToListenersList)
                    {
                        // Add to list of listeners in the system, do this BEFORE firing the 'OnEventSourceCreated' so that 
                        // Those added sources see this listener.
                        this.m_Next = s_Listeners;
                        s_Listeners = this;
                    }

                    // Find all existing eventSources call OnEventSourceCreated to 'catchup'
                    // Note that we DO have reentrancy here because 'AddListener' calls out to user code (via OnEventSourceCreated callback) 
                    // We tolerate this by iterating over a copy of the list here. New event sources will take care of adding listeners themselves
                    // EventSources are not guaranteed to be added at the end of the s_EventSource list -- We re-use slots when a new source
                    // is created.
                    WeakReference[] eventSourcesSnapshot = s_EventSources.ToArray();

                    for (int i = 0; i < eventSourcesSnapshot.Length; i++)
                    {
                        WeakReference eventSourceRef = eventSourcesSnapshot[i];
                        EventSource eventSource = eventSourceRef.Target as EventSource;
                        if (eventSource != null)
                        {
                            EventSourceCreatedEventArgs args = new EventSourceCreatedEventArgs();
                            args.EventSource = eventSource;
                            callback(this, args);
                        }
                    }

                    Validate();
                }
                finally
                {
                    s_CreatingListener = false;
                }
            }

        }

        // Instance fields
        internal volatile EventListener m_Next;                         // These form a linked list in s_Listeners
#if FEATURE_ACTIVITYSAMPLING
        internal ActivityFilter m_activityFilter;                       // If we are filtering by activity on this Listener, this keeps track of it. 
#endif // FEATURE_ACTIVITYSAMPLING

        // static fields

        /// <summary>
        /// The list of all listeners in the appdomain.  Listeners must be explicitly disposed to remove themselves 
        /// from this list.   Note that EventSources point to their listener but NOT the reverse.  
        /// </summary>
        internal static EventListener s_Listeners;
        /// <summary>
        /// The list of all active eventSources in the appdomain.  Note that eventSources do NOT 
        /// remove themselves from this list this is a weak list and the GC that removes them may
        /// not have happened yet.  Thus it can contain event sources that are dead (thus you have 
        /// to filter those out.  
        /// </summary>
        internal static List<WeakReference> s_EventSources;

        /// <summary>
        /// Used to disallow reentrancy.  
        /// </summary>
        private static bool s_CreatingListener = false;

        /// <summary>
        /// Used to register AD/Process shutdown callbacks.
        /// </summary>
        private static bool s_EventSourceShutdownRegistered = false;
        #endregion
    }

    /// <summary>
    /// Passed to the code:EventSource.OnEventCommand callback
    /// </summary>
    public class EventCommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the command for the callback.
        /// </summary>
        public EventCommand Command { get; internal set; }

        /// <summary>
        /// Gets the arguments for the callback.
        /// </summary>
        public IDictionary<String, String> Arguments { get; internal set; }

        /// <summary>
        /// Enables the event that has the specified identifier.
        /// </summary>
        /// <param name="eventId">Event ID of event to be enabled</param>
        /// <returns>true if eventId is in range</returns>
        public bool EnableEvent(int eventId)
        {
            if (Command != EventCommand.Enable && Command != EventCommand.Disable)
                throw new InvalidOperationException();
            return eventSource.EnableEventForDispatcher(dispatcher, eventId, true);
        }

        /// <summary>
        /// Disables the event that have the specified identifier.
        /// </summary>
        /// <param name="eventId">Event ID of event to be disabled</param>
        /// <returns>true if eventId is in range</returns>
        public bool DisableEvent(int eventId)
        {
            if (Command != EventCommand.Enable && Command != EventCommand.Disable)
                throw new InvalidOperationException();
            return eventSource.EnableEventForDispatcher(dispatcher, eventId, false);
        }

        #region private

        internal EventCommandEventArgs(EventCommand command, IDictionary<string, string> arguments, EventSource eventSource,
            EventListener listener, int perEventSourceSessionId, int etwSessionId, bool enable, EventLevel level, EventKeywords matchAnyKeyword)
        {
            this.Command = command;
            this.Arguments = arguments;
            this.eventSource = eventSource;
            this.listener = listener;
            this.perEventSourceSessionId = perEventSourceSessionId;
            this.etwSessionId = etwSessionId;
            this.enable = enable;
            this.level = level;
            this.matchAnyKeyword = matchAnyKeyword;
        }

        internal EventSource eventSource;
        internal EventDispatcher dispatcher;

        // These are the arguments of sendCommand and are only used for deferring commands until after we are fully initialized. 
        internal EventListener listener;
        internal int perEventSourceSessionId;
        internal int etwSessionId;
        internal bool enable;
        internal EventLevel level;
        internal EventKeywords matchAnyKeyword;
        internal EventCommandEventArgs nextCommand;     // We form a linked list of these deferred commands.      

        #endregion
    }

    /// <summary>
    /// EventSourceCreatedEventArgs is passed to <see cref="EventListener.EventSourceCreated"/>
    /// </summary>
    public class EventSourceCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// The EventSource that is attaching to the listener.
        /// </summary>
        public EventSource EventSource
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// EventWrittenEventArgs is passed to the user-provided override for
    /// <see cref="EventListener.OnEventWritten"/> when an event is fired.
    /// </summary>
    public class EventWrittenEventArgs : EventArgs
    {
        /// <summary>
        /// The name of the event.   
        /// </summary>
        public string EventName
        {
            get
            {
                if (m_eventName != null || EventId < 0)      // TraceLogging convention EventID == -1
                {
                    return m_eventName;
                }
                else
                    return m_eventSource.m_eventData[EventId].Name;
            }
            internal set
            {
                m_eventName = value;
            }
        }

        /// <summary>
        /// Gets the event ID for the event that was written.
        /// </summary>
        public int EventId { get; internal set; }

        /// <summary>
        /// Gets the activity ID for the thread on which the event was written.
        /// </summary>
        public Guid ActivityId
        {
            get { return EventSource.CurrentThreadActivityId; }
        }

        /// <summary>
        /// Gets the related activity ID if one was specified when the event was written.
        /// </summary>
        public Guid RelatedActivityId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the payload for the event.
        /// </summary>
        public ReadOnlyCollection<Object> Payload { get; internal set; }

        /// <summary>
        /// Gets the payload argument names.
        /// </summary>
        public ReadOnlyCollection<string> PayloadNames
        {
            get
            {
                // For contract based events we create the list lazily.
                if (m_payloadNames == null)
                {
                    // Self described events are identified by id -1.
                    Debug.Assert(EventId != -1);

                    var names = new List<string>();
                    foreach (var parameter in m_eventSource.m_eventData[EventId].Parameters)
                    {
                        names.Add(parameter.Name);
                    }
                    m_payloadNames = new ReadOnlyCollection<string>(names);
                }

                return m_payloadNames;
            }

            internal set
            {
                m_payloadNames = value;
            }
        }

        /// <summary>
        /// Gets the event source object.
        /// </summary>
        public EventSource EventSource { get { return m_eventSource; } }

        /// <summary>
        /// Gets the keywords for the event.
        /// </summary>
        public EventKeywords Keywords
        {
            get
            {
                if (EventId < 0)      // TraceLogging convention EventID == -1
                    return m_keywords;

                return (EventKeywords)m_eventSource.m_eventData[EventId].Descriptor.Keywords;
            }
        }

        /// <summary>
        /// Gets the operation code for the event.
        /// </summary>
        public EventOpcode Opcode
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return m_opcode;
                return (EventOpcode)m_eventSource.m_eventData[EventId].Descriptor.Opcode;
            }
        }

        /// <summary>
        /// Gets the task for the event.
        /// </summary>
        public EventTask Task
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return EventTask.None;

                return (EventTask)m_eventSource.m_eventData[EventId].Descriptor.Task;
            }
        }

        /// <summary>
        /// Any provider/user defined options associated with the event.  
        /// </summary>
        public EventTags Tags
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return m_tags;
                return m_eventSource.m_eventData[EventId].Tags;
            }
        }

        /// <summary>
        /// Gets the message for the event.  If the message has {N} parameters they are NOT substituted.  
        /// </summary>
        public string Message
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return m_message;
                else
                    return m_eventSource.m_eventData[EventId].Message;
            }
            internal set
            {
                m_message = value;
            }
        }


#if FEATURE_MANAGED_ETW_CHANNELS
        /// <summary>
        /// Gets the channel for the event.
        /// </summary>
        public EventChannel Channel
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return EventChannel.None;
                return (EventChannel)m_eventSource.m_eventData[EventId].Descriptor.Channel;
            }
        }
#endif

        /// <summary>
        /// Gets the version of the event.
        /// </summary>
        public byte Version
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return 0;
                return m_eventSource.m_eventData[EventId].Descriptor.Version;
            }
        }

        /// <summary>
        /// Gets the level for the event.
        /// </summary>
        public EventLevel Level
        {
            get
            {
                if (EventId <= 0)      // TraceLogging convention EventID == -1
                    return m_level;
                return (EventLevel)m_eventSource.m_eventData[EventId].Descriptor.Level;
            }
        }

        #region private
        internal EventWrittenEventArgs(EventSource eventSource)
        {
            m_eventSource = eventSource;
        }
        private string m_message;
        private string m_eventName;
        private EventSource m_eventSource;
        private ReadOnlyCollection<string> m_payloadNames;
        internal EventTags m_tags;
        internal EventOpcode m_opcode;
        internal EventLevel m_level;
        internal EventKeywords m_keywords;
        #endregion
    }

    /// <summary>
    /// Allows customizing defaults and specifying localization support for the event source class to which it is applied. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EventSourceAttribute : Attribute
    {
        /// <summary>
        /// Overrides the ETW name of the event source (which defaults to the class name)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Overrides the default (calculated) Guid of an EventSource type. Explicitly defining a GUID is discouraged, 
        /// except when upgrading existing ETW providers to using event sources.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// <para>
        /// EventSources support localization of events. The names used for events, opcodes, tasks, keywords and maps 
        /// can be localized to several languages if desired. This works by creating a ResX style string table 
        /// (by simply adding a 'Resource File' to your project). This resource file is given a name e.g. 
        /// 'DefaultNameSpace.ResourceFileName' which can be passed to the ResourceManager constructor to read the 
        /// resources. This name is the value of the LocalizationResources property. 
        /// </para><para>
        /// If LocalizationResources property is non-null, then EventSource will look up the localized strings for events by 
        /// using the following resource naming scheme
        /// </para>
        ///     <para>* event_EVENTNAME</para>
        ///     <para>* task_TASKNAME</para>
        ///     <para>* keyword_KEYWORDNAME</para>
        ///     <para>* map_MAPNAME</para>
        /// <para>
        /// where the capitalized name is the name of the event, task, keyword, or map value that should be localized.   
        /// Note that the localized string for an event corresponds to the Message string, and can have {0} values 
        /// which represent the payload values.  
        /// </para>
        /// </summary>
        public string LocalizationResources { get; set; }
    }

    /// <summary>
    /// Any instance methods in a class that subclasses <see cref="EventSource"/> and that return void are
    /// assumed by default to be methods that generate an ETW event. Enough information can be deduced from the
    /// name of the method and its signature to generate basic schema information for the event. The
    /// <see cref="EventAttribute"/> class allows you to specify additional event schema information for an event if
    /// desired.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EventAttribute : Attribute
    {
        /// <summary>Construct an EventAttribute with specified eventId</summary>
        /// <param name="eventId">ID of the ETW event (an integer between 1 and 65535)</param>
        public EventAttribute(int eventId) { this.EventId = eventId; Level = EventLevel.Informational; this.m_opcodeSet = false; }
        /// <summary>Event's ID</summary>
        public int EventId { get; private set; }
        /// <summary>Event's severity level: indicates the severity or verbosity of the event</summary>
        public EventLevel Level { get; set; }
        /// <summary>Event's keywords: allows classification of events by "categories"</summary>
        public EventKeywords Keywords { get; set; }
        /// <summary>Event's operation code: allows defining operations, generally used with Tasks</summary>
        public EventOpcode Opcode
        {
            get
            {
                return m_opcode;
            }
            set
            {
                this.m_opcode = value;
                this.m_opcodeSet = true;
            }
        }

        internal bool IsOpcodeSet
        {
            get
            {
                return m_opcodeSet;
            }
        }

        /// <summary>Event's task: allows logical grouping of events</summary>
        public EventTask Task { get; set; }
#if FEATURE_MANAGED_ETW_CHANNELS
        /// <summary>Event's channel: defines an event log as an additional destination for the event</summary>
        public EventChannel Channel { get; set; }
#endif
        /// <summary>Event's version</summary>
        public byte Version { get; set; }

        /// <summary>
        /// This can be specified to enable formatting and localization of the event's payload. You can 
        /// use standard .NET substitution operators (eg {1}) in the string and they will be replaced 
        /// with the 'ToString()' of the corresponding part of the  event payload.   
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// User defined options associated with the event.  These do not have meaning to the EventSource but
        /// are passed through to listeners which given them semantics.  
        /// </summary>
        public EventTags Tags { get; set; }

        /// <summary>
        /// Allows fine control over the Activity IDs generated by start and stop events
        /// </summary>
        public EventActivityOptions ActivityOptions { get; set; }

        #region private
        EventOpcode m_opcode;
        private bool m_opcodeSet;
        #endregion
    }

    /// <summary>
    /// By default all instance methods in a class that subclasses code:EventSource that and return
    /// void are assumed to be methods that generate an event. This default can be overridden by specifying
    /// the code:NonEventAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NonEventAttribute : Attribute
    {
        /// <summary>
        /// Constructs a default NonEventAttribute
        /// </summary>
        public NonEventAttribute() { }
    }

    // FUTURE we may want to expose this at some point once we have a partner that can help us validate the design.
#if FEATURE_MANAGED_ETW_CHANNELS
    /// <summary>
    /// EventChannelAttribute allows customizing channels supported by an EventSource. This attribute must be
    /// applied to an member of type EventChannel defined in a Channels class nested in the EventSource class:
    /// <code>
    ///     public static class Channels
    ///     {
    ///         [Channel(Enabled = true, EventChannelType = EventChannelType.Admin)]
    ///         public const EventChannel Admin = (EventChannel)16;
    ///     
    ///         [Channel(Enabled = false, EventChannelType = EventChannelType.Operational)]
    ///         public const EventChannel Operational = (EventChannel)17;
    ///     }
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
    public 
#endif
    class EventChannelAttribute : Attribute
    {
        /// <summary>
        /// Specified whether the channel is enabled by default
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Legal values are in EventChannelType
        /// </summary>
        public EventChannelType EventChannelType { get; set; }

#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
        /// <summary>
        /// Specifies the isolation for the channel
        /// </summary>
        public EventChannelIsolation Isolation { get; set; }

        /// <summary>
        /// Specifies an SDDL access descriptor that controls access to the log file that backs the channel.
        /// See MSDN ((http://msdn.microsoft.com/en-us/library/windows/desktop/aa382741.aspx) for details.
        /// </summary>
        public string Access { get; set; } 

        /// <summary>
        /// Allows importing channels defined in external manifests
        /// </summary>
        public string ImportChannel { get; set; }
#endif

        // TODO: there is a convention that the name is the Provider/Type   Should we provide an override?
        // public string Name { get; set; }
    }

    /// <summary>
    /// Allowed channel types
    /// </summary>
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
    public 
#endif
    enum EventChannelType
    {
        /// <summary>The admin channel</summary>
        Admin = 1,
        /// <summary>The operational channel</summary>
        Operational,
        /// <summary>The Analytic channel</summary>
        Analytic,
        /// <summary>The debug channel</summary>
        Debug,
    }

#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
    /// <summary>
    /// Allowed isolation levels. See MSDN (http://msdn.microsoft.com/en-us/library/windows/desktop/aa382741.aspx) 
    /// for the default permissions associated with each level. EventChannelIsolation and Access allows control over the 
    /// access permissions for the channel and backing file.
    /// </summary>
    public 
    enum EventChannelIsolation
    {
        /// <summary>
        /// This is the default isolation level. All channels that specify Application isolation use the same ETW session
        /// </summary>
        Application = 1,
        /// <summary>
        /// All channels that specify System isolation use the same ETW session
        /// </summary>
        System,
        /// <summary>
        /// Use sparingly! When specifying Custom isolation, a separate ETW session is created for the channel.
        /// Using Custom isolation lets you control the access permissions for the channel and backing file.
        /// Because there are only 64 ETW sessions available, you should limit your use of Custom isolation.
        /// </summary>
        Custom,
    }
#endif
#endif

    /// <summary>
    /// Describes the pre-defined command (EventCommandEventArgs.Command property) that is passed to the OnEventCommand callback.
    /// </summary>
    public enum EventCommand
    {
        /// <summary>
        /// Update EventSource state
        /// </summary>
        Update = 0,
        /// <summary>
        /// Request EventSource to generate and send its manifest
        /// </summary>
        SendManifest = -1,
        /// <summary>
        /// Enable event
        /// </summary>
        Enable = -2,
        /// <summary>
        /// Disable event
        /// </summary>
        Disable = -3
    };


    #region private classes

#if FEATURE_ACTIVITYSAMPLING

    /// <summary>
    /// ActivityFilter is a helper structure that is used to keep track of run-time state
    /// associated with activity filtering. It is 1-1 with EventListeners (logically
    /// every listener has one of these, however we actually allocate them lazily), as well
    /// as 1-to-1 with tracing-aware EtwSessions.
    /// 
    /// This structure also keeps track of the sampling counts associated with 'trigger'
    /// events.  Because these trigger events are rare, and you typically only have one of
    /// them, we store them here as a linked list.
    /// </summary>
    internal sealed class ActivityFilter : IDisposable
    {
        /// <summary>
        /// Disable all activity filtering for the listener associated with 'filterList', 
        /// (in the session associated with it) that is triggered by any event in 'source'.
        /// </summary>
        public static void DisableFilter(ref ActivityFilter filterList, EventSource source)
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(Monitor.IsEntered(EventListener.EventListenersLock));
#endif

            if (filterList == null)
                return;

            ActivityFilter cur;
            // Remove it from anywhere in the list (except the first element, which has to 
            // be treated specially)
            ActivityFilter prev = filterList;
            cur = prev.m_next;
            while (cur != null)
            {
                if (cur.m_providerGuid == source.Guid)
                {
                    // update TriggersActivityTracking bit
                    if (cur.m_eventId >= 0 && cur.m_eventId < source.m_eventData.Length)
                        --source.m_eventData[cur.m_eventId].TriggersActivityTracking;

                    // Remove it from the linked list.
                    prev.m_next = cur.m_next;
                    // dispose of the removed node
                    cur.Dispose();
                    // update cursor
                    cur = prev.m_next;
                }
                else
                {
                    // update cursors
                    prev = cur;
                    cur = prev.m_next;
                }
            }

            // Sadly we have to treat the first element specially in linked list removal in C#
            if (filterList.m_providerGuid == source.Guid)
            {
                // update TriggersActivityTracking bit
                if (filterList.m_eventId >= 0 && filterList.m_eventId < source.m_eventData.Length)
                    --source.m_eventData[filterList.m_eventId].TriggersActivityTracking;

                // We are the first element in the list.   
                var first = filterList;
                filterList = first.m_next;
                // dispose of the removed node
                first.Dispose();
            }
            // the above might have removed the one ActivityFilter in the session that contains the 
            // cleanup delegate; re-create the delegate if needed
            if (filterList != null)
            {
                EnsureActivityCleanupDelegate(filterList);
            }
        }

        /// <summary>
        /// Currently this has "override" semantics. We first disable all filters
        /// associated with 'source', and next we add new filters for each entry in the 
        /// string 'startEvents'. participateInSampling specifies whether non-startEvents 
        /// always trigger or only trigger when current activity is 'active'.
        /// </summary>
        public static void UpdateFilter(
                                    ref ActivityFilter filterList,
                                    EventSource source,
                                    int perEventSourceSessionId,
                                    string startEvents)
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(Monitor.IsEntered(EventListener.EventListenersLock));
#endif

            // first remove all filters associated with 'source'
            DisableFilter(ref filterList, source);

            if (!string.IsNullOrEmpty(startEvents))
            {
                // ActivitySamplingStartEvents is a space-separated list of Event:Frequency pairs.
                // The Event may be specified by name or by ID. Errors in parsing such a pair 
                // result in the error being reported to the listeners, and the pair being ignored.
                // E.g. "CustomActivityStart:1000 12:10" specifies that for event CustomActivityStart
                // we should initiate activity tracing once every 1000 events, *and* for event ID 12
                // we should initiate activity tracing once every 10 events.
                string[] activityFilterStrings = startEvents.Split(' ');

                for (int i = 0; i < activityFilterStrings.Length; ++i)
                {
                    string activityFilterString = activityFilterStrings[i];
                    int sampleFreq = 1;
                    int eventId = -1;
                    int colonIdx = activityFilterString.IndexOf(':');
                    if (colonIdx < 0)
                    {
                        source.ReportOutOfBandMessage("ERROR: Invalid ActivitySamplingStartEvent specification: " +
                            activityFilterString, false);
                        // ignore failure...
                        continue;
                    }
                    string sFreq = activityFilterString.Substring(colonIdx + 1);
                    if (!int.TryParse(sFreq, out sampleFreq))
                    {
                        source.ReportOutOfBandMessage("ERROR: Invalid sampling frequency specification: " + sFreq, false);
                        continue;
                    }
                    activityFilterString = activityFilterString.Substring(0, colonIdx);
                    if (!int.TryParse(activityFilterString, out eventId))
                    {
                        // reset eventId
                        eventId = -1;
                        // see if it's an event name
                        for (int j = 0; j < source.m_eventData.Length; j++)
                        {
                            EventSource.EventMetadata[] ed = source.m_eventData;
                            if (ed[j].Name != null && ed[j].Name.Length == activityFilterString.Length &&
                                string.Compare(ed[j].Name, activityFilterString, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                eventId = ed[j].Descriptor.EventId;
                                break;
                            }
                        }
                    }
                    if (eventId < 0 || eventId >= source.m_eventData.Length)
                    {
                        source.ReportOutOfBandMessage("ERROR: Invalid eventId specification: " + activityFilterString, false);
                        continue;
                    }
                    EnableFilter(ref filterList, source, perEventSourceSessionId, eventId, sampleFreq);
                }
            }
        }

        /// <summary>
        /// Returns the first ActivityFilter from 'filterList' corresponding to 'source'.
        /// </summary>
        public static ActivityFilter GetFilter(ActivityFilter filterList, EventSource source)
        {
            for (var af = filterList; af != null; af = af.m_next)
            {
                if (af.m_providerGuid == source.Guid && af.m_samplingFreq != -1)
                    return af;
            }
            return null;
        }

        /// <summary>
        /// Returns a session mask representing all sessions in which the activity 
        /// associated with the current thread is allowed  through the activity filter. 
        /// If 'triggeringEvent' is true the event MAY be a triggering event. Ideally 
        /// most of the time this is false as you can guarentee this event is NOT a 
        /// triggering event. If 'triggeringEvent' is true, then it checks the 
        /// 'EventSource' and 'eventID' of the event being logged to see if it is actually
        /// a trigger. If so it activates the current activity. 
        /// 
        /// If 'childActivityID' is present, it will be added to the active set if the 
        /// current activity is active.  
        /// </summary>
        unsafe public static bool PassesActivityFilter(
                                    ActivityFilter filterList,
                                    Guid* childActivityID,
                                    bool triggeringEvent,
                                    EventSource source,
                                    int eventId)
        {
            Debug.Assert(filterList != null && filterList.m_activeActivities != null);
            bool shouldBeLogged = false;
            if (triggeringEvent)
            {
                for (ActivityFilter af = filterList; af != null; af = af.m_next)
                {
                    if (eventId == af.m_eventId && source.Guid == af.m_providerGuid)
                    {
                        // Update the sampling count with wrap-around
                        int curSampleCount, newSampleCount;
                        do
                        {
                            curSampleCount = af.m_curSampleCount;
                            if (curSampleCount <= 1)
                                newSampleCount = af.m_samplingFreq;        // Wrap around, counting down to 1
                            else
                                newSampleCount = curSampleCount - 1;
                        }
                        while (Interlocked.CompareExchange(ref af.m_curSampleCount, newSampleCount, curSampleCount) != curSampleCount);
                        // If we hit zero, then start tracking the activity.  
                        if (curSampleCount <= 1)
                        {
                            Guid currentActivityId = EventSource.InternalCurrentThreadActivityId;
                            Tuple<Guid, int> startId;
                            // only add current activity if it's not already a root activity
                            if (!af.m_rootActiveActivities.TryGetValue(currentActivityId, out startId))
                            {
                                // EventSource.OutputDebugString(string.Format("  PassesAF - Triggering(session {0}, evt {1})", af.m_perEventSourceSessionId, eventId));
                                shouldBeLogged = true;
                                af.m_activeActivities[currentActivityId] = Environment.TickCount;
                                af.m_rootActiveActivities[currentActivityId] = Tuple.Create(source.Guid, eventId);
                            }
                        }
                        else
                        {
                            // a start event following a triggering start event
                            Guid currentActivityId = EventSource.InternalCurrentThreadActivityId;
                            Tuple<Guid, int> startId;
                            // only remove current activity if we added it
                            if (af.m_rootActiveActivities.TryGetValue(currentActivityId, out startId) &&
                                startId.Item1 == source.Guid && startId.Item2 == eventId)
                            {
                                // EventSource.OutputDebugString(string.Format("Activity dying: {0} -> StartEvent({1})", currentActivityId, eventId));
                                // remove activity only from current logging scope (af)
                                int dummy;
                                af.m_activeActivities.TryRemove(currentActivityId, out dummy);
                            }
                        }
                        break;
                    }
                }
            }

            var activeActivities = GetActiveActivities(filterList);
            if (activeActivities != null)
            {
                // if we hadn't already determined this should be logged, test further
                if (!shouldBeLogged)
                {
                    shouldBeLogged = !activeActivities.IsEmpty &&
                                     activeActivities.ContainsKey(EventSource.InternalCurrentThreadActivityId);
                }
                if (shouldBeLogged && childActivityID != null &&
                    ((EventOpcode)source.m_eventData[eventId].Descriptor.Opcode == EventOpcode.Send))
                {
                    FlowActivityIfNeeded(filterList, null, childActivityID);
                    // EventSource.OutputDebugString(string.Format("  PassesAF - activity {0}", *childActivityID));
                }
            }
            // EventSource.OutputDebugString(string.Format("  PassesAF - shouldBeLogged(evt {0}) = {1:x}", eventId, shouldBeLogged));
            return shouldBeLogged;
        }

        public static bool IsCurrentActivityActive(ActivityFilter filterList)
        {
            var activeActivities = GetActiveActivities(filterList);
            if (activeActivities != null &&
                activeActivities.ContainsKey(EventSource.InternalCurrentThreadActivityId))
                return true;

            return false;
        }

        /// <summary>
        /// For the EventListener/EtwSession associated with 'filterList', add 'childActivityid'
        /// to list of active activities IF 'currentActivityId' is also active. Passing in a null
        /// value for  'currentActivityid' is an indication tha caller has already verified
        /// that the current activity is active.
        /// </summary>
        unsafe public static void FlowActivityIfNeeded(ActivityFilter filterList, Guid* currentActivityId, Guid* childActivityID)
        {
            Debug.Assert(childActivityID != null);

            var activeActivities = GetActiveActivities(filterList);
            Debug.Assert(activeActivities != null);

            // take currentActivityId == null to mean we *know* the current activity is "active"
            if (currentActivityId != null && !activeActivities.ContainsKey(*currentActivityId))
                return;

            if (activeActivities.Count > MaxActivityTrackCount)
            {
                TrimActiveActivityStore(activeActivities);
                // make sure current activity is still in the set:
                activeActivities[EventSource.InternalCurrentThreadActivityId] = Environment.TickCount;
            }
            // add child activity to list of actives
            activeActivities[*childActivityID] = Environment.TickCount;

        }

        /// <summary>
        /// </summary>
        public static void UpdateKwdTriggers(ActivityFilter activityFilter, Guid sourceGuid, EventSource source, EventKeywords sessKeywords)
        {
            for (var af = activityFilter; af != null; af = af.m_next)
            {
                if ((sourceGuid == af.m_providerGuid) &&
                    (source.m_eventData[af.m_eventId].TriggersActivityTracking > 0 ||
                    ((EventOpcode)source.m_eventData[af.m_eventId].Descriptor.Opcode == EventOpcode.Send)))
                {
                    // we could be more precise here, if we tracked 'anykeywords' per session
                    unchecked
                    {
                        source.m_keywordTriggers |= (source.m_eventData[af.m_eventId].Descriptor.Keywords & (long)sessKeywords);
                    }
                }
            }
        }

        /// <summary>
        /// For the EventSource specified by 'sourceGuid' and the EventListener/EtwSession 
        /// associated with 'this' ActivityFilter list, return configured sequence of 
        /// [eventId, sampleFreq] pairs that defines the sampling policy.
        /// </summary>
        public IEnumerable<Tuple<int, int>> GetFilterAsTuple(Guid sourceGuid)
        {
            for (ActivityFilter af = this; af != null; af = af.m_next)
            {
                if (af.m_providerGuid == sourceGuid)
                    yield return Tuple.Create(af.m_eventId, af.m_samplingFreq);
            }
        }

        /// <summary>
        /// The cleanup being performed consists of removing the m_myActivityDelegate from
        /// the static s_activityDying, therefore allowing the ActivityFilter to be reclaimed.
        /// </summary>
        public void Dispose()
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(Monitor.IsEntered(EventListener.EventListenersLock));
#endif
            // m_myActivityDelegate is still alive (held by the static EventSource.s_activityDying). 
            // Therefore we are ok to take a dependency on m_myActivityDelegate being valid even 
            // during the finalization of the ActivityFilter
            if (m_myActivityDelegate != null)
            {
                EventSource.s_activityDying = (Action<Guid>)Delegate.Remove(EventSource.s_activityDying, m_myActivityDelegate);
                m_myActivityDelegate = null;
            }
        }

        #region private

        /// <summary>
        /// Creates a new ActivityFilter that is triggered by 'eventId' from 'source' ever
        /// 'samplingFreq' times the event fires. You can have several of these forming a 
        /// linked list.
        /// </summary>
        private ActivityFilter(EventSource source, int perEventSourceSessionId, int eventId, int samplingFreq, ActivityFilter existingFilter = null)
        {
            m_providerGuid = source.Guid;
            m_perEventSourceSessionId = perEventSourceSessionId;
            m_eventId = eventId;
            m_samplingFreq = samplingFreq;
            m_next = existingFilter;

            Debug.Assert(existingFilter == null ||
                            (existingFilter.m_activeActivities == null) == (existingFilter.m_rootActiveActivities == null));

            // if this is the first filter we add for this session, we need to create a new 
            // table of activities. m_activeActivities is common across EventSources in the same
            // session
            ConcurrentDictionary<Guid, int> activeActivities = null;
            if (existingFilter == null ||
                (activeActivities = GetActiveActivities(existingFilter)) == null)
            {
                m_activeActivities = new ConcurrentDictionary<Guid, int>();
                m_rootActiveActivities = new ConcurrentDictionary<Guid, Tuple<Guid, int>>();

                // Add a delegate to the 'SetCurrentThreadToActivity callback so that I remove 'dead' activities
                m_myActivityDelegate = GetActivityDyingDelegate(this);
                EventSource.s_activityDying = (Action<Guid>)Delegate.Combine(EventSource.s_activityDying, m_myActivityDelegate);
            }
            else
            {
                m_activeActivities = activeActivities;
                m_rootActiveActivities = existingFilter.m_rootActiveActivities;
            }

        }

        /// <summary>
        /// Ensure there's at least one ActivityFilter in the 'filterList' that contains an
        /// activity-removing delegate for the listener/session associated with 'filterList'.
        /// </summary>
        private static void EnsureActivityCleanupDelegate(ActivityFilter filterList)
        {
            if (filterList == null)
                return;

            for (ActivityFilter af = filterList; af != null; af = af.m_next)
            {
                if (af.m_myActivityDelegate != null)
                    return;
            }

            // we didn't find a delegate
            filterList.m_myActivityDelegate = GetActivityDyingDelegate(filterList);
            EventSource.s_activityDying = (Action<Guid>)Delegate.Combine(EventSource.s_activityDying, filterList.m_myActivityDelegate);
        }

        /// <summary>
        /// Builds the delegate to be called when an activity is dying. This is responsible
        /// for performing whatever cleanup is needed for the ActivityFilter list passed in.
        /// This gets "added" to EventSource.s_activityDying and ends up being called from
        /// EventSource.SetCurrentThreadActivityId and ActivityFilter.PassesActivityFilter.
        /// </summary>
        /// <returns>The delegate to be called when an activity is dying</returns>
        private static Action<Guid> GetActivityDyingDelegate(ActivityFilter filterList)
        {
            return (Guid oldActivity) =>
            {
                int dummy;
                filterList.m_activeActivities.TryRemove(oldActivity, out dummy);
                Tuple<Guid, int> dummyTuple;
                filterList.m_rootActiveActivities.TryRemove(oldActivity, out dummyTuple);
            };
        }

        /// <summary>
        /// Enables activity filtering for the listener associated with 'filterList', triggering on
        /// the event 'eventID' from 'source' with a sampling frequency of 'samplingFreq'
        /// 
        /// if 'eventID' is out of range (e.g. negative), it means we are not triggering (but we are 
        /// activitySampling if something else triggered).  
        /// </summary>
        /// <returns>true if activity sampling is enabled the samplingFreq is non-zero </returns>
        private static bool EnableFilter(ref ActivityFilter filterList, EventSource source, int perEventSourceSessionId, int eventId, int samplingFreq)
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(Monitor.IsEntered(EventListener.EventListenersLock));
#endif
            Debug.Assert(samplingFreq > 0);
            Debug.Assert(eventId >= 0);

            filterList = new ActivityFilter(source, perEventSourceSessionId, eventId, samplingFreq, filterList);

            // Mark the 'quick Check' that indicates this is a trigger event.  
            // If eventId is out of range then this mark is not done which has the effect of ignoring 
            // the trigger.
            if (0 <= eventId && eventId < source.m_eventData.Length)
                ++source.m_eventData[eventId].TriggersActivityTracking;

            return true;
        }

        /// <summary>
        /// Normally this code never runs, it is here just to prevent run-away resource usage.  
        /// </summary>
        private static void TrimActiveActivityStore(ConcurrentDictionary<Guid, int> activities)
        {
            if (activities.Count > MaxActivityTrackCount)
            {
                // Remove half of the oldest activity ids.  
                var keyValues = activities.ToArray();
                var tickNow = Environment.TickCount;

                // Sort by age, taking into account wrap-around.   As long as x and y are within 
                // 23 days of now then (0x7FFFFFFF & (tickNow - x.Value)) is the delta (even if 
                // TickCount wraps).  I then sort by DESCENDING age.  (that is oldest value first)
                Array.Sort(keyValues, (x, y) => (0x7FFFFFFF & (tickNow - y.Value)) - (0x7FFFFFFF & (tickNow - x.Value)));
                for (int i = 0; i < keyValues.Length / 2; i++)
                {
                    int dummy;
                    activities.TryRemove(keyValues[i].Key, out dummy);
                }
            }
        }

        private static ConcurrentDictionary<Guid, int> GetActiveActivities(
                                    ActivityFilter filterList)
        {
            for (ActivityFilter af = filterList; af != null; af = af.m_next)
            {
                if (af.m_activeActivities != null)
                    return af.m_activeActivities;
            }
            return null;
        }

        // m_activeActivities always points to the sample dictionary for EVERY ActivityFilter  
        // in the m_next list. The 'int' value in the m_activities set is a timestamp 
        // (Environment.TickCount) of when the entry was put in the system and is used to 
        // remove 'old' entries that if the set gets too big.
        ConcurrentDictionary<Guid, int> m_activeActivities;

        // m_rootActiveActivities holds the "root" active activities, i.e. the activities 
        // that were marked as active because a Start event fired on them. We need to keep
        // track of these to enable sampling in the scenario of an app's main thread that 
        // never explicitly sets distinct activity IDs as it executes. To handle these
        // situations we manufacture a Guid from the thread's ID, and:
        //   (a) we consider the firing of a start event when the sampling counter reaches 
        //       zero to mark the beginning of an interesting activity, and 
        //   (b) we consider the very next firing of the same start event to mark the
        //       ending of that activity.
        // We use a ConcurrentDictionary to avoid taking explicit locks.
        //   The key (a guid) represents the activity ID of the root active activity
        //   The value is made up of the Guid of the event provider and the eventId of
        //      the start event.
        ConcurrentDictionary<Guid, Tuple<Guid, int>> m_rootActiveActivities;
        Guid m_providerGuid;        // We use the GUID rather than object identity because we don't want to keep the eventSource alive
        int m_eventId;              // triggering event
        int m_samplingFreq;         // Counter reset to this when it hits 0
        int m_curSampleCount;       // We count down to 0 and then activate the activity. 
        int m_perEventSourceSessionId;  // session ID bit for ETW, 0 for EventListeners

        const int MaxActivityTrackCount = 100000;   // maximum number of tracked activities

        ActivityFilter m_next;      // We create a linked list of these
        Action<Guid> m_myActivityDelegate;
        #endregion
    };


    /// <summary>
    /// An EtwSession instance represents an activity-tracing-aware ETW session. Since these
    /// are limited to 8 concurrent sessions per machine (currently) we're going to store
    /// the active ones in a singly linked list.
    /// </summary>
    internal class EtwSession
    {
        public static EtwSession GetEtwSession(int etwSessionId, bool bCreateIfNeeded = false)
        {
            if (etwSessionId < 0)
                return null;

            EtwSession etwSession;
            foreach (var wrEtwSession in s_etwSessions)
            {
#if ES_BUILD_STANDALONE
                if ((etwSession = (EtwSession) wrEtwSession.Target) != null && etwSession.m_etwSessionId == etwSessionId)
                    return etwSession;
#else
                if (wrEtwSession.TryGetTarget(out etwSession) && etwSession.m_etwSessionId == etwSessionId)
                    return etwSession;
#endif
            }

            if (!bCreateIfNeeded)
                return null;

#if ES_BUILD_STANDALONE
            if (s_etwSessions == null)
                s_etwSessions = new List<WeakReference>();

            etwSession = new EtwSession(etwSessionId);
            s_etwSessions.Add(new WeakReference(etwSession));
#else
            if (s_etwSessions == null)
                s_etwSessions = new List<WeakReference<EtwSession>>();

            etwSession = new EtwSession(etwSessionId);
            s_etwSessions.Add(new WeakReference<EtwSession>(etwSession));
#endif

            if (s_etwSessions.Count > s_thrSessionCount)
                TrimGlobalList();

            return etwSession;

        }

        public static void RemoveEtwSession(EtwSession etwSession)
        {
            Debug.Assert(etwSession != null);
            if (s_etwSessions == null || etwSession == null)
                return;

            s_etwSessions.RemoveAll((wrEtwSession) =>
                {
                    EtwSession session;
#if ES_BUILD_STANDALONE
                    return (session = (EtwSession) wrEtwSession.Target) != null &&
                           (session.m_etwSessionId == etwSession.m_etwSessionId);
#else
                    return wrEtwSession.TryGetTarget(out session) &&
                           (session.m_etwSessionId == etwSession.m_etwSessionId);
#endif
                });

            if (s_etwSessions.Count > s_thrSessionCount)
                TrimGlobalList();
        }

        private static void TrimGlobalList()
        {
            if (s_etwSessions == null)
                return;

            s_etwSessions.RemoveAll((wrEtwSession) =>
                {
#if ES_BUILD_STANDALONE
                    return wrEtwSession.Target == null;
#else
                    EtwSession session;
                    return !wrEtwSession.TryGetTarget(out session);
#endif
                });
        }

        private EtwSession(int etwSessionId)
        {
            m_etwSessionId = etwSessionId;
        }

        public readonly int m_etwSessionId;        // ETW session ID (as retrieved by EventProvider)
        public ActivityFilter m_activityFilter;    // all filters enabled for this session

#if ES_BUILD_STANDALONE
        private static List<WeakReference> s_etwSessions = new List<WeakReference>();
#else
        private static List<WeakReference<EtwSession>> s_etwSessions = new List<WeakReference<EtwSession>>();
#endif
        private const int s_thrSessionCount = 16;
    }

#endif // FEATURE_ACTIVITYSAMPLING

    // holds a bitfield representing a session mask
    /// <summary>
    /// A SessionMask represents a set of (at most MAX) sessions as a bit mask. The perEventSourceSessionId
    /// is the index in the SessionMask of the bit that will be set. These can translate to
    /// EventSource's reserved keywords bits using the provided ToEventKeywords() and
    /// FromEventKeywords() methods.
    /// </summary>
    internal struct SessionMask
    {
        public SessionMask(SessionMask m)
        { m_mask = m.m_mask; }

        public SessionMask(uint mask = 0)
        { m_mask = mask & MASK; }

        public bool IsEqualOrSupersetOf(SessionMask m)
        {
            return (this.m_mask | m.m_mask) == this.m_mask;
        }

        public static SessionMask All
        {
            get { return new SessionMask(MASK); }
        }

        public static SessionMask FromId(int perEventSourceSessionId)
        {
            Debug.Assert(perEventSourceSessionId < MAX);
            return new SessionMask((uint)1 << perEventSourceSessionId);
        }

        public ulong ToEventKeywords()
        {
            return (ulong)m_mask << SHIFT_SESSION_TO_KEYWORD;
        }

        public static SessionMask FromEventKeywords(ulong m)
        {
            return new SessionMask((uint)(m >> SHIFT_SESSION_TO_KEYWORD));
        }

        public bool this[int perEventSourceSessionId]
        {
            get
            {
                Debug.Assert(perEventSourceSessionId < MAX);
                return (m_mask & (1 << perEventSourceSessionId)) != 0;
            }
            set
            {
                Debug.Assert(perEventSourceSessionId < MAX);
                if (value) m_mask |= ((uint)1 << perEventSourceSessionId);
                else m_mask &= ~((uint)1 << perEventSourceSessionId);
            }
        }

        public static SessionMask operator |(SessionMask m1, SessionMask m2)
        {
            return new SessionMask(m1.m_mask | m2.m_mask);
        }

        public static SessionMask operator &(SessionMask m1, SessionMask m2)
        {
            return new SessionMask(m1.m_mask & m2.m_mask);
        }

        public static SessionMask operator ^(SessionMask m1, SessionMask m2)
        {
            return new SessionMask(m1.m_mask ^ m2.m_mask);
        }

        public static SessionMask operator ~(SessionMask m)
        {
            return new SessionMask(MASK & ~(m.m_mask));
        }

        public static explicit operator ulong(SessionMask m)
        { return m.m_mask; }

        public static explicit operator uint(SessionMask m)
        { return m.m_mask; }

        private uint m_mask;

        internal const int SHIFT_SESSION_TO_KEYWORD = 44;         // bits 44-47 inclusive are reserved
        internal const uint MASK = 0x0fU;                         // the mask of 4 reserved bits
        internal const uint MAX = 4;                              // maximum number of simultaneous ETW sessions supported
    }

    /// <summary>
    /// code:EventDispatchers are a simple 'helper' structure that holds the filtering state
    /// (m_EventEnabled) for a particular EventSource X EventListener tuple
    /// 
    /// Thus a single EventListener may have many EventDispatchers (one for every EventSource 
    /// that that EventListener has activate) and a Single EventSource may also have many
    /// event Dispatchers (one for every EventListener that has activated it). 
    /// 
    /// Logically a particular EventDispatcher belongs to exactly one EventSource and exactly  
    /// one EventListener (alhtough EventDispatcher does not 'remember' the EventSource it is
    /// associated with. 
    /// </summary>
    internal class EventDispatcher
    {
        internal EventDispatcher(EventDispatcher next, bool[] eventEnabled, EventListener listener)
        {
            m_Next = next;
            m_EventEnabled = eventEnabled;
            m_Listener = listener;
        }

        // Instance fields
        readonly internal EventListener m_Listener;   // The dispatcher this entry is for
        internal bool[] m_EventEnabled;               // For every event in a the eventSource, is it enabled?
#if FEATURE_ACTIVITYSAMPLING
        internal bool m_activityFilteringEnabled;     // does THIS EventSource have activity filtering turned on for this listener?
#endif // FEATURE_ACTIVITYSAMPLING

        // Only guaranteed to exist after a InsureInit()
        internal EventDispatcher m_Next;              // These form a linked list in code:EventSource.m_Dispatchers
        // Of all listeners for that eventSource.  
    }

    /// <summary>
    /// Flags that can be used with EventSource.GenerateManifest to control how the ETW manifest for the EventSource is
    /// generated.
    /// </summary>
    [Flags]
    public enum EventManifestOptions
    {
        /// <summary>
        /// Only the resources associated with current UI culture are included in the  manifest
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Throw exceptions for any inconsistency encountered
        /// </summary>
        Strict = 0x1,
        /// <summary>
        /// Generate a "resources" node under "localization" for every satellite assembly provided
        /// </summary>
        AllCultures = 0x2,
        /// <summary>
        /// Generate the manifest only if the event source needs to be registered on the machine,
        /// otherwise return null (but still perform validation if Strict is specified)
        /// </summary>
        OnlyIfNeededForRegistration = 0x4,
        /// <summary>
        /// When generating the manifest do *not* enforce the rule that the current EventSource class
        /// must be the base class for the user-defined type passed in. This allows validation of .net
        /// event sources using the new validation code
        /// </summary>
        AllowEventSourceOverride = 0x8,
    }

    /// <summary>
    /// ManifestBuilder is designed to isolate the details of the message of the event from the
    /// rest of EventSource.  This one happens to create XML. 
    /// </summary>
    internal partial class ManifestBuilder
    {
        /// <summary>
        /// Build a manifest for 'providerName' with the given GUID, which will be packaged into 'dllName'.
        /// 'resources, is a resource manager.  If specified all messages are localized using that manager.  
        /// </summary>
        public ManifestBuilder(string providerName, Guid providerGuid, string dllName, ResourceManager resources,
                               EventManifestOptions flags)
        {
#if FEATURE_MANAGED_ETW_CHANNELS
            this.providerName = providerName;
#endif
            this.flags = flags;

            this.resources = resources;
            sb = new StringBuilder();
            events = new StringBuilder();
            templates = new StringBuilder();
            opcodeTab = new Dictionary<int, string>();
            stringTab = new Dictionary<string, string>();
            errors = new List<string>();
            perEventByteArrayArgIndices = new Dictionary<string, List<int>>();

            sb.AppendLine("<instrumentationManifest xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
            sb.AppendLine(" <instrumentation xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:win=\"http://manifests.microsoft.com/win/2004/08/windows/events\">");
            sb.AppendLine("  <events xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
            sb.Append("<provider name=\"").Append(providerName).
               Append("\" guid=\"{").Append(providerGuid.ToString()).Append("}");
            if (dllName != null)
                sb.Append("\" resourceFileName=\"").Append(dllName).Append("\" messageFileName=\"").Append(dllName);

            var symbolsName = providerName.Replace("-", "").Replace(".", "_");  // Period and - are illegal replace them.
            sb.Append("\" symbol=\"").Append(symbolsName);
            sb.Append("\">").AppendLine();
        }

        public void AddOpcode(string name, int value)
        {
            if ((flags & EventManifestOptions.Strict) != 0)
            {
                if (value <= 10 || value >= 239)
                {
                    ManifestError(Resources.GetResourceString("EventSource_IllegalOpcodeValue", name, value));
                }
                string prevName;
                if (opcodeTab.TryGetValue(value, out prevName) && !name.Equals(prevName, StringComparison.Ordinal))
                {
                    ManifestError(Resources.GetResourceString("EventSource_OpcodeCollision", name, prevName, value));
                }
            }
            opcodeTab[value] = name;
        }
        public void AddTask(string name, int value)
        {
            if ((flags & EventManifestOptions.Strict) != 0)
            {
                if (value <= 0 || value >= 65535)
                {
                    ManifestError(Resources.GetResourceString("EventSource_IllegalTaskValue", name, value));
                }
                string prevName;
                if (taskTab != null && taskTab.TryGetValue(value, out prevName) && !name.Equals(prevName, StringComparison.Ordinal))
                {
                    ManifestError(Resources.GetResourceString("EventSource_TaskCollision", name, prevName, value));
                }
            }
            if (taskTab == null)
                taskTab = new Dictionary<int, string>();
            taskTab[value] = name;
        }
        public void AddKeyword(string name, ulong value)
        {
            if ((value & (value - 1)) != 0)   // Is it a power of 2?
            {
                ManifestError(Resources.GetResourceString("EventSource_KeywordNeedPowerOfTwo", "0x" + value.ToString("x", CultureInfo.CurrentCulture), name), true);
            }
            if ((flags & EventManifestOptions.Strict) != 0)
            {
                if (value >= 0x0000100000000000UL && !name.StartsWith("Session", StringComparison.Ordinal))
                {
                    ManifestError(Resources.GetResourceString("EventSource_IllegalKeywordsValue", name, "0x" + value.ToString("x", CultureInfo.CurrentCulture)));
                }
                string prevName;
                if (keywordTab != null && keywordTab.TryGetValue(value, out prevName) && !name.Equals(prevName, StringComparison.Ordinal))
                {
                    ManifestError(Resources.GetResourceString("EventSource_KeywordCollision", name, prevName, "0x" + value.ToString("x", CultureInfo.CurrentCulture)));
                }
            }
            if (keywordTab == null)
                keywordTab = new Dictionary<ulong, string>();
            keywordTab[value] = name;
        }

#if FEATURE_MANAGED_ETW_CHANNELS
        /// <summary>
        /// Add a channel.  channelAttribute can be null
        /// </summary>
        public void AddChannel(string name, int value, EventChannelAttribute channelAttribute)
        {
            EventChannel chValue = (EventChannel)value;
            if (value < (int)EventChannel.Admin || value > 255)
                ManifestError(Resources.GetResourceString("EventSource_EventChannelOutOfRange", name, value));
            else if (chValue >= EventChannel.Admin && chValue <= EventChannel.Debug &&
                     channelAttribute != null && EventChannelToChannelType(chValue) != channelAttribute.EventChannelType)
            {
                // we want to ensure developers do not define EventChannels that conflict with the builtin ones,
                // but we want to allow them to override the default ones...
                ManifestError(Resources.GetResourceString("EventSource_ChannelTypeDoesNotMatchEventChannelValue",
                                                                            name, ((EventChannel)value).ToString()));
            }

            // TODO: validate there are no conflicting manifest exposed names (generally following the format "provider/type")

            ulong kwd = GetChannelKeyword(chValue);

            if (channelTab == null)
                channelTab = new Dictionary<int, ChannelInfo>(4);
            channelTab[value] = new ChannelInfo { Name = name, Keywords = kwd, Attribs = channelAttribute };
        }

        private EventChannelType EventChannelToChannelType(EventChannel channel)
        {
#if !ES_BUILD_STANDALONE
            Debug.Assert(channel >= EventChannel.Admin && channel <= EventChannel.Debug);
#endif
            return (EventChannelType)((int)channel - (int)EventChannel.Admin + (int)EventChannelType.Admin);
        }
        private EventChannelAttribute GetDefaultChannelAttribute(EventChannel channel)
        {
            EventChannelAttribute attrib = new EventChannelAttribute();
            attrib.EventChannelType = EventChannelToChannelType(channel);
            if (attrib.EventChannelType <= EventChannelType.Operational)
                attrib.Enabled = true;
            return attrib;
        }

        public ulong[] GetChannelData()
        {
            if (this.channelTab == null)
            {
                return new ulong[0];
            }

            // We create an array indexed by the channel id for fast look up.
            // E.g. channelMask[Admin] will give you the bit mask for Admin channel.
            int maxkey = -1;
            foreach (var item in this.channelTab.Keys)
            {
                if (item > maxkey)
                {
                    maxkey = item;
                }
            }

            ulong[] channelMask = new ulong[maxkey + 1];
            foreach (var item in this.channelTab)
            {
                channelMask[item.Key] = item.Value.Keywords;
            }

            return channelMask;
        }

#endif
        public void StartEvent(string eventName, EventAttribute eventAttribute)
        {
            Debug.Assert(numParams == 0);
            Debug.Assert(this.eventName == null);
            this.eventName = eventName;
            numParams = 0;
            byteArrArgIndices = null;

            events.Append("  <event").
                 Append(" value=\"").Append(eventAttribute.EventId).Append("\"").
                 Append(" version=\"").Append(eventAttribute.Version).Append("\"").
                 Append(" level=\"").Append(GetLevelName(eventAttribute.Level)).Append("\"").
                 Append(" symbol=\"").Append(eventName).Append("\"");

            // at this point we add to the manifest's stringTab a message that is as-of-yet 
            // "untranslated to manifest convention", b/c we don't have the number or position 
            // of any byte[] args (which require string format index updates)
            WriteMessageAttrib(events, "event", eventName, eventAttribute.Message);

            if (eventAttribute.Keywords != 0)
                events.Append(" keywords=\"").Append(GetKeywords((ulong)eventAttribute.Keywords, eventName)).Append("\"");
            if (eventAttribute.Opcode != 0)
                events.Append(" opcode=\"").Append(GetOpcodeName(eventAttribute.Opcode, eventName)).Append("\"");
            if (eventAttribute.Task != 0)
                events.Append(" task=\"").Append(GetTaskName(eventAttribute.Task, eventName)).Append("\"");
#if FEATURE_MANAGED_ETW_CHANNELS
            if (eventAttribute.Channel != 0)
            {
                events.Append(" channel=\"").Append(GetChannelName(eventAttribute.Channel, eventName, eventAttribute.Message)).Append("\"");
            }
#endif
        }

        public void AddEventParameter(Type type, string name)
        {
            if (numParams == 0)
                templates.Append("  <template tid=\"").Append(eventName).Append("Args\">").AppendLine();
            if (type == typeof(byte[]))
            {
                // mark this index as "extraneous" (it has no parallel in the managed signature)
                // we use these values in TranslateToManifestConvention()
                if (byteArrArgIndices == null)
                    byteArrArgIndices = new List<int>(4);
                byteArrArgIndices.Add(numParams);

                // add an extra field to the template representing the length of the binary blob
                numParams++;
                templates.Append("   <data name=\"").Append(name).Append("Size\" inType=\"win:UInt32\"/>").AppendLine();
            }
            numParams++;
            templates.Append("   <data name=\"").Append(name).Append("\" inType=\"").Append(GetTypeName(type)).Append("\"");
            // TODO: for 'byte*' types it assumes the user provided length is named using the same naming convention
            //       as for 'byte[]' args (blob_arg_name + "Size")
            if ((type.IsArray || type.IsPointer) && type.GetElementType() == typeof(byte))
            {
                // add "length" attribute to the "blob" field in the template (referencing the field added above)
                templates.Append(" length=\"").Append(name).Append("Size\"");
            }
            // ETW does not support 64-bit value maps, so we don't specify these as ETW maps
            if (type.IsEnum() && Enum.GetUnderlyingType(type) != typeof(UInt64) && Enum.GetUnderlyingType(type) != typeof(Int64))
            {
                templates.Append(" map=\"").Append(type.Name).Append("\"");
                if (mapsTab == null)
                    mapsTab = new Dictionary<string, Type>();
                if (!mapsTab.ContainsKey(type.Name))
                    mapsTab.Add(type.Name, type);        // Remember that we need to dump the type enumeration  
            }

            templates.Append("/>").AppendLine();
        }
        public void EndEvent()
        {
            if (numParams > 0)
            {
                templates.Append("  </template>").AppendLine();
                events.Append(" template=\"").Append(eventName).Append("Args\"");
            }
            events.Append("/>").AppendLine();

            if (byteArrArgIndices != null)
                perEventByteArrayArgIndices[eventName] = byteArrArgIndices;

            // at this point we have all the information we need to translate the C# Message
            // to the manifest string we'll put in the stringTab
            string msg;
            if (stringTab.TryGetValue("event_" + eventName, out msg))
            {
                msg = TranslateToManifestConvention(msg, eventName);
                stringTab["event_" + eventName] = msg;
            }

            eventName = null;
            numParams = 0;
            byteArrArgIndices = null;
        }

#if FEATURE_MANAGED_ETW_CHANNELS
        // Channel keywords are generated one per channel to allow channel based filtering in event viewer. These keywords are autogenerated
        // by mc.exe for compiling a manifest and are based on the order of the channels (fields) in the Channels inner class (when advanced
        // channel support is enabled), or based on the order the predefined channels appear in the EventAttribute properties (for simple 
        // support). The manifest generated *MUST* have the channels specified in the same order (that's how our computed keywords are mapped
        // to channels by the OS infrastructure).
        // If channelKeyworkds is present, and has keywords bits in the ValidPredefinedChannelKeywords then it is 
        // assumed that that the keyword for that channel should be that bit.   
        // otherwise we allocate a channel bit for the channel.  
        // explicit channel bits are only used by WCF to mimic an existing manifest, 
        // so we don't dont do error checking.  
        public ulong GetChannelKeyword(EventChannel channel, ulong channelKeyword = 0)
        {
            // strip off any non-channel keywords, since we are only interested in channels here.  
            channelKeyword &= ValidPredefinedChannelKeywords;
            if (channelTab == null)
            {
                channelTab = new Dictionary<int, ChannelInfo>(4);
            }

            if (channelTab.Count == MaxCountChannels)
                ManifestError(Resources.GetResourceString("EventSource_MaxChannelExceeded"));

            ChannelInfo info;
            if (!channelTab.TryGetValue((int)channel, out info))
            {
                // If we were not given an explicit channel, allocate one.  
                if (channelKeyword != 0)
                {
                    channelKeyword = nextChannelKeywordBit;
                    nextChannelKeywordBit >>= 1;
                }
            }
            else
            {
                channelKeyword = info.Keywords;
            }

            return channelKeyword;
        }
#endif

        public byte[] CreateManifest()
        {
            string str = CreateManifestString();
            return Encoding.UTF8.GetBytes(str);
        }

        public IList<string> Errors { get { return errors; } }

        /// <summary>
        /// When validating an event source it adds the error to the error collection.
        /// When not validating it throws an exception if runtimeCritical is "true".
        /// Otherwise the error is ignored.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="runtimeCritical"></param>
        public void ManifestError(string msg, bool runtimeCritical = false)
        {
            if ((flags & EventManifestOptions.Strict) != 0)
                errors.Add(msg);
            else if (runtimeCritical)
                throw new ArgumentException(msg);
        }

        private string CreateManifestString()
        {

#if FEATURE_MANAGED_ETW_CHANNELS
            // Write out the channels
            if (channelTab != null)
            {
                sb.Append(" <channels>").AppendLine();
                var sortedChannels = new List<KeyValuePair<int, ChannelInfo>>();
                foreach (KeyValuePair<int, ChannelInfo> p in channelTab) { sortedChannels.Add(p); }
                sortedChannels.Sort((p1, p2) => -Comparer<ulong>.Default.Compare(p1.Value.Keywords, p2.Value.Keywords));
                foreach (var kvpair in sortedChannels)
                {
                    int channel = kvpair.Key;
                    ChannelInfo channelInfo = kvpair.Value;

                    string channelType = null;
                    string elementName = "channel";
                    bool enabled = false;
                    string fullName = null;
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                    string isolation = null;
                    string access = null;
#endif
                    if (channelInfo.Attribs != null)
                    {
                        var attribs = channelInfo.Attribs;
                        if (Enum.IsDefined(typeof(EventChannelType), attribs.EventChannelType))
                            channelType = attribs.EventChannelType.ToString();
                        enabled = attribs.Enabled;
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                        if (attribs.ImportChannel != null)
                        {
                            fullName = attribs.ImportChannel;
                            elementName = "importChannel";
                        }
                        if (Enum.IsDefined(typeof(EventChannelIsolation), attribs.Isolation))
                            isolation = attribs.Isolation.ToString();
                        access = attribs.Access;
#endif
                    }
                    if (fullName == null)
                        fullName = providerName + "/" + channelInfo.Name;

                    sb.Append("  <").Append(elementName);
                    sb.Append(" chid=\"").Append(channelInfo.Name).Append("\"");
                    sb.Append(" name=\"").Append(fullName).Append("\"");
                    if (elementName == "channel")   // not applicable to importChannels.
                    {
                        WriteMessageAttrib(sb, "channel", channelInfo.Name, null);
                        sb.Append(" value=\"").Append(channel).Append("\"");
                        if (channelType != null)
                            sb.Append(" type=\"").Append(channelType).Append("\"");
                        sb.Append(" enabled=\"").Append(enabled.ToString().ToLower()).Append("\"");
#if FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
                        if (access != null)
                            sb.Append(" access=\"").Append(access).Append("\"");
                        if (isolation != null)
                            sb.Append(" isolation=\"").Append(isolation).Append("\"");
#endif
                    }
                    sb.Append("/>").AppendLine();
                }
                sb.Append(" </channels>").AppendLine();
            }
#endif

            // Write out the tasks
            if (taskTab != null)
            {

                sb.Append(" <tasks>").AppendLine();
                var sortedTasks = new List<int>(taskTab.Keys);
                sortedTasks.Sort();
                foreach (int task in sortedTasks)
                {
                    sb.Append("  <task");
                    WriteNameAndMessageAttribs(sb, "task", taskTab[task]);
                    sb.Append(" value=\"").Append(task).Append("\"/>").AppendLine();
                }
                sb.Append(" </tasks>").AppendLine();
            }

            // Write out the maps
            if (mapsTab != null)
            {
                sb.Append(" <maps>").AppendLine();
                foreach (Type enumType in mapsTab.Values)
                {
                    bool isbitmap = EventSource.GetCustomAttributeHelper(enumType, typeof(FlagsAttribute), flags) != null;
                    string mapKind = isbitmap ? "bitMap" : "valueMap";
                    sb.Append("  <").Append(mapKind).Append(" name=\"").Append(enumType.Name).Append("\">").AppendLine();

                    // write out each enum value 
                    FieldInfo[] staticFields = enumType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
                    foreach (FieldInfo staticField in staticFields)
                    {
                        object constantValObj = staticField.GetRawConstantValue();
                        if (constantValObj != null)
                        {
                            long hexValue;
                            if (constantValObj is int)
                                hexValue = ((int)constantValObj);
                            else if (constantValObj is long)
                                hexValue = ((long)constantValObj);
                            else
                                continue;

                            // ETW requires all bitmap values to be powers of 2.  Skip the ones that are not. 
                            // TODO: Warn people about the dropping of values. 
                            if (isbitmap && ((hexValue & (hexValue - 1)) != 0 || hexValue == 0))
                                continue;

                            sb.Append("   <map value=\"0x").Append(hexValue.ToString("x", CultureInfo.InvariantCulture)).Append("\"");
                            WriteMessageAttrib(sb, "map", enumType.Name + "." + staticField.Name, staticField.Name);
                            sb.Append("/>").AppendLine();
                        }
                    }
                    sb.Append("  </").Append(mapKind).Append(">").AppendLine();
                }
                sb.Append(" </maps>").AppendLine();
            }

            // Write out the opcodes
            sb.Append(" <opcodes>").AppendLine();
            var sortedOpcodes = new List<int>(opcodeTab.Keys);
            sortedOpcodes.Sort();
            foreach (int opcode in sortedOpcodes)
            {
                sb.Append("  <opcode");
                WriteNameAndMessageAttribs(sb, "opcode", opcodeTab[opcode]);
                sb.Append(" value=\"").Append(opcode).Append("\"/>").AppendLine();
            }
            sb.Append(" </opcodes>").AppendLine();

            // Write out the keywords
            if (keywordTab != null)
            {
                sb.Append(" <keywords>").AppendLine();
                var sortedKeywords = new List<ulong>(keywordTab.Keys);
                sortedKeywords.Sort();
                foreach (ulong keyword in sortedKeywords)
                {
                    sb.Append("  <keyword");
                    WriteNameAndMessageAttribs(sb, "keyword", keywordTab[keyword]);
                    sb.Append(" mask=\"0x").Append(keyword.ToString("x", CultureInfo.InvariantCulture)).Append("\"/>").AppendLine();
                }
                sb.Append(" </keywords>").AppendLine();
            }

            sb.Append(" <events>").AppendLine();
            sb.Append(events);
            sb.Append(" </events>").AppendLine();

            sb.Append(" <templates>").AppendLine();
            if (templates.Length > 0)
            {
                sb.Append(templates);
            }
            else
            {
                // Work around a cornercase ETW issue where a manifest with no templates causes 
                // ETW events to not get sent to their associated channel.
                sb.Append("    <template tid=\"_empty\"></template>").AppendLine();
            }
            sb.Append(" </templates>").AppendLine();

            sb.Append("</provider>").AppendLine();
            sb.Append("</events>").AppendLine();
            sb.Append("</instrumentation>").AppendLine();

            // Output the localization information.  
            sb.Append("<localization>").AppendLine();

            List<CultureInfo> cultures = null;
            if (resources != null && (flags & EventManifestOptions.AllCultures) != 0)
            {
                cultures = GetSupportedCultures(resources);
            }
            else
            {
                cultures = new List<CultureInfo>();
                cultures.Add(CultureInfo.CurrentUICulture);
            }
#if ES_BUILD_STANDALONE || ES_BUILD_PN
            var sortedStrings = new List<string>(stringTab.Keys);
            sortedStrings.Sort();
#else
            // DD 947936
            var sortedStrings = new string[stringTab.Keys.Count];
            stringTab.Keys.CopyTo(sortedStrings, 0);
            // Avoid using public Array.Sort as that attempts to access BinaryCompatibility. Unfortunately FrameworkEventSource gets called 
            // very early in the app domain creation, when _FusionStore is not set up yet, resulting in a failure to run the static constructory
            // for BinaryCompatibility. This failure is then cached and a TypeInitializationException is thrown every time some code attampts to
            // access BinaryCompatibility.
            ArraySortHelper<string>.IntrospectiveSort(sortedStrings, 0, sortedStrings.Length, string.Compare);
#endif
            foreach (var ci in cultures)
            {
                sb.Append(" <resources culture=\"").Append(ci.Name).Append("\">").AppendLine();
                sb.Append("  <stringTable>").AppendLine();

                foreach (var stringKey in sortedStrings)
                {
                    string val = GetLocalizedMessage(stringKey, ci, etwFormat: true);
                    sb.Append("   <string id=\"").Append(stringKey).Append("\" value=\"").Append(val).Append("\"/>").AppendLine();
                }
                sb.Append("  </stringTable>").AppendLine();
                sb.Append(" </resources>").AppendLine();
            }
            sb.Append("</localization>").AppendLine();
            sb.AppendLine("</instrumentationManifest>");
            return sb.ToString();
        }

        #region private
        private void WriteNameAndMessageAttribs(StringBuilder stringBuilder, string elementName, string name)
        {
            stringBuilder.Append(" name=\"").Append(name).Append("\"");
            WriteMessageAttrib(sb, elementName, name, name);
        }
        private void WriteMessageAttrib(StringBuilder stringBuilder, string elementName, string name, string value)
        {
            string key = elementName + "_" + name;
            // See if the user wants things localized.  
            if (resources != null)
            {
                // resource fallback: strings in the neutral culture will take precedence over inline strings
                string localizedString = resources.GetString(key, CultureInfo.InvariantCulture);
                if (localizedString != null)
                    value = localizedString;
            }
            if (value == null)
                return;

            stringBuilder.Append(" message=\"$(string.").Append(key).Append(")\"");
            string prevValue;
            if (stringTab.TryGetValue(key, out prevValue) && !prevValue.Equals(value))
            {
                ManifestError(Resources.GetResourceString("EventSource_DuplicateStringKey", key), true);
                return;
            }

            stringTab[key] = value;
        }
        internal string GetLocalizedMessage(string key, CultureInfo ci, bool etwFormat)
        {
            string value = null;
            if (resources != null)
            {
                string localizedString = resources.GetString(key, ci);
                if (localizedString != null)
                {
                    value = localizedString;
                    if (etwFormat && key.StartsWith("event_", StringComparison.Ordinal))
                    {
                        var evtName = key.Substring("event_".Length);
                        value = TranslateToManifestConvention(value, evtName);
                    }
                }
            }
            if (etwFormat && value == null)
                stringTab.TryGetValue(key, out value);

            return value;
        }

        /// <summary>
        /// There's no API to enumerate all languages an assembly is localized into, so instead
        /// we enumerate through all the "known" cultures and attempt to load a corresponding satellite 
        /// assembly
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        private static List<CultureInfo> GetSupportedCultures(ResourceManager resources)
        {
            var cultures = new List<CultureInfo>();

            if (!cultures.Contains(CultureInfo.CurrentUICulture))
                cultures.Insert(0, CultureInfo.CurrentUICulture);
            return cultures;
        }

        private static string GetLevelName(EventLevel level)
        {
            return (((int)level >= 16) ? "" : "win:") + level.ToString();
        }

#if FEATURE_MANAGED_ETW_CHANNELS
        private string GetChannelName(EventChannel channel, string eventName, string eventMessage)
        {
            ChannelInfo info = null;
            if (channelTab == null || !channelTab.TryGetValue((int)channel, out info))
            {
                if (channel < EventChannel.Admin) // || channel > EventChannel.Debug)
                    ManifestError(Resources.GetResourceString("EventSource_UndefinedChannel", channel, eventName));

                // allow channels to be auto-defined.  The well known ones get their well known names, and the
                // rest get names Channel<N>.  This allows users to modify the Manifest if they want more advanced features. 
                if (channelTab == null)
                    channelTab = new Dictionary<int, ChannelInfo>(4);

                string channelName = channel.ToString();        // For well know channels this is a nice name, otherwise a number 
                if (EventChannel.Debug < channel)
                    channelName = "Channel" + channelName;      // Add a 'Channel' prefix for numbers.  

                AddChannel(channelName, (int)channel, GetDefaultChannelAttribute(channel));
                if (!channelTab.TryGetValue((int)channel, out info))
                    ManifestError(Resources.GetResourceString("EventSource_UndefinedChannel", channel, eventName));
            }
            // events that specify admin channels *must* have non-null "Message" attributes
            if (resources != null && eventMessage == null)
                eventMessage = resources.GetString("event_" + eventName, CultureInfo.InvariantCulture);
            if (info.Attribs.EventChannelType == EventChannelType.Admin && eventMessage == null)
                ManifestError(Resources.GetResourceString("EventSource_EventWithAdminChannelMustHaveMessage", eventName, info.Name));
            return info.Name;
        }
#endif
        private string GetTaskName(EventTask task, string eventName)
        {
            if (task == EventTask.None)
                return "";

            string ret;
            if (taskTab == null)
                taskTab = new Dictionary<int, string>();
            if (!taskTab.TryGetValue((int)task, out ret))
                ret = taskTab[(int)task] = eventName;
            return ret;
        }

        private string GetOpcodeName(EventOpcode opcode, string eventName)
        {
            switch (opcode)
            {
                case EventOpcode.Info:
                    return "win:Info";
                case EventOpcode.Start:
                    return "win:Start";
                case EventOpcode.Stop:
                    return "win:Stop";
                case EventOpcode.DataCollectionStart:
                    return "win:DC_Start";
                case EventOpcode.DataCollectionStop:
                    return "win:DC_Stop";
                case EventOpcode.Extension:
                    return "win:Extension";
                case EventOpcode.Reply:
                    return "win:Reply";
                case EventOpcode.Resume:
                    return "win:Resume";
                case EventOpcode.Suspend:
                    return "win:Suspend";
                case EventOpcode.Send:
                    return "win:Send";
                case EventOpcode.Receive:
                    return "win:Receive";
            }

            string ret;
            if (opcodeTab == null || !opcodeTab.TryGetValue((int)opcode, out ret))
            {
                ManifestError(Resources.GetResourceString("EventSource_UndefinedOpcode", opcode, eventName), true);
                ret = null;
            }
            return ret;
        }

        private string GetKeywords(ulong keywords, string eventName)
        {
#if FEATURE_MANAGED_ETW_CHANNELS
            // ignore keywords associate with channels
            // See ValidPredefinedChannelKeywords def for more. 
            keywords &= ~ValidPredefinedChannelKeywords;
#endif

            string ret = "";
            for (ulong bit = 1; bit != 0; bit <<= 1)
            {
                if ((keywords & bit) != 0)
                {
                    string keyword = null;
                    if ((keywordTab == null || !keywordTab.TryGetValue(bit, out keyword)) &&
                        (bit >= (ulong)0x1000000000000))
                    {
                        // do not report Windows reserved keywords in the manifest (this allows the code
                        // to be resilient to potential renaming of these keywords)
                        keyword = string.Empty;
                    }
                    if (keyword == null)
                    {
                        ManifestError(Resources.GetResourceString("EventSource_UndefinedKeyword", "0x" + bit.ToString("x", CultureInfo.CurrentCulture), eventName), true);
                        keyword = string.Empty;
                    }
                    if (ret.Length != 0 && keyword.Length != 0)
                        ret = ret + " ";
                    ret = ret + keyword;
                }
            }
            return ret;
        }

        private string GetTypeName(Type type)
        {
            if (type.IsEnum())
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                var typeName = GetTypeName(fields[0].FieldType);
                return typeName.Replace("win:Int", "win:UInt"); // ETW requires enums to be unsigned.  
            }

            return GetTypeNameHelper(type);
        }

        private static void UpdateStringBuilder(ref StringBuilder stringBuilder, string eventMessage, int startIndex, int count)
        {
            if (stringBuilder == null)
                stringBuilder = new StringBuilder();
            stringBuilder.Append(eventMessage, startIndex, count);
        }

        private static readonly string[] s_escapes = { "&amp;", "&lt;", "&gt;", "&apos;", "&quot;", "%r", "%n", "%t" };
        // Manifest messages use %N conventions for their message substitutions.   Translate from
        // .NET conventions.   We can't use RegEx for this (we are in mscorlib), so we do it 'by hand' 
        private string TranslateToManifestConvention(string eventMessage, string evtName)
        {
            StringBuilder stringBuilder = null;        // We lazily create this 
            int writtenSoFar = 0;
            int chIdx = -1;
            for (int i = 0; ;)
            {
                if (i >= eventMessage.Length)
                {
                    if (stringBuilder == null)
                        return eventMessage;
                    UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
                    return stringBuilder.ToString();
                }

                if (eventMessage[i] == '%')
                {
                    // handle format message escaping character '%' by escaping it
                    UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
                    stringBuilder.Append("%%");
                    i++;
                    writtenSoFar = i;
                }
                else if (i < eventMessage.Length - 1 &&
                    (eventMessage[i] == '{' && eventMessage[i + 1] == '{' || eventMessage[i] == '}' && eventMessage[i + 1] == '}'))
                {
                    // handle C# escaped '{" and '}'
                    UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
                    stringBuilder.Append(eventMessage[i]);
                    i++; i++;
                    writtenSoFar = i;
                }
                else if (eventMessage[i] == '{')
                {
                    int leftBracket = i;
                    i++;
                    int argNum = 0;
                    while (i < eventMessage.Length && Char.IsDigit(eventMessage[i]))
                    {
                        argNum = argNum * 10 + eventMessage[i] - '0';
                        i++;
                    }
                    if (i < eventMessage.Length && eventMessage[i] == '}')
                    {
                        i++;
                        UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, leftBracket - writtenSoFar);
                        int manIndex = TranslateIndexToManifestConvention(argNum, evtName);
                        stringBuilder.Append('%').Append(manIndex);
                        // An '!' after the insert specifier {n} will be interpreted as a literal.
                        // We'll escape it so that mc.exe does not attempt to consider it the 
                        // beginning of a format string.
                        if (i < eventMessage.Length && eventMessage[i] == '!')
                        {
                            i++;
                            stringBuilder.Append("%!");
                        }
                        writtenSoFar = i;
                    }
                    else
                    {
                        ManifestError(Resources.GetResourceString("EventSource_UnsupportedMessageProperty", evtName, eventMessage));
                    }
                }
                else if ((chIdx = "&<>'\"\r\n\t".IndexOf(eventMessage[i])) >= 0)
                {
                    UpdateStringBuilder(ref stringBuilder, eventMessage, writtenSoFar, i - writtenSoFar);
                    i++;
                    stringBuilder.Append(s_escapes[chIdx]);
                    writtenSoFar = i;
                }
                else
                    i++;
            }
        }

        private int TranslateIndexToManifestConvention(int idx, string evtName)
        {
            List<int> byteArrArgIndices;
            if (perEventByteArrayArgIndices.TryGetValue(evtName, out byteArrArgIndices))
            {
                foreach (var byArrIdx in byteArrArgIndices)
                {
                    if (idx >= byArrIdx)
                        ++idx;
                    else
                        break;
                }
            }
            return idx + 1;
        }

#if FEATURE_MANAGED_ETW_CHANNELS
        class ChannelInfo
        {
            public string Name;
            public ulong Keywords;
            public EventChannelAttribute Attribs;
        }
#endif

        Dictionary<int, string> opcodeTab;
        Dictionary<int, string> taskTab;
#if FEATURE_MANAGED_ETW_CHANNELS
        Dictionary<int, ChannelInfo> channelTab;
#endif
        Dictionary<ulong, string> keywordTab;
        Dictionary<string, Type> mapsTab;

        Dictionary<string, string> stringTab;       // Maps unlocalized strings to localized ones  

#if FEATURE_MANAGED_ETW_CHANNELS
        // WCF used EventSource to mimic a existing ETW manifest.   To support this
        // in just their case, we allowed them to specify the keywords associated 
        // with their channels explicitly.   ValidPredefinedChannelKeywords is 
        // this set of channel keywords that we allow to be explicitly set.  You
        // can ignore these bits otherwise.  
        internal const ulong ValidPredefinedChannelKeywords = 0xF000000000000000;
        ulong nextChannelKeywordBit = 0x8000000000000000;   // available Keyword bit to be used for next channel definition, grows down
        const int MaxCountChannels = 8; // a manifest can defined at most 8 ETW channels
#endif

        StringBuilder sb;               // Holds the provider information. 
        StringBuilder events;           // Holds the events. 
        StringBuilder templates;

#if FEATURE_MANAGED_ETW_CHANNELS
        string providerName;
#endif
        ResourceManager resources;      // Look up localized strings here.  
        EventManifestOptions flags;
        IList<string> errors;           // list of currently encountered errors
        Dictionary<string, List<int>> perEventByteArrayArgIndices;  // "event_name" -> List_of_Indices_of_Byte[]_Arg

        // State we track between StartEvent and EndEvent.  
        string eventName;               // Name of the event currently being processed. 
        int numParams;                  // keeps track of the number of args the event has. 
        List<int> byteArrArgIndices;    // keeps track of the index of each byte[] argument
        #endregion
    }

    /// <summary>
    /// Used to send the m_rawManifest into the event dispatcher as a series of events.  
    /// </summary>
    internal struct ManifestEnvelope
    {
        public const int MaxChunkSize = 0xFF00;
        public enum ManifestFormats : byte
        {
            SimpleXmlFormat = 1,          // simply dump the XML manifest as UTF8
        }

#if FEATURE_MANAGED_ETW
        public ManifestFormats Format;
        public byte MajorVersion;
        public byte MinorVersion;
        public byte Magic;
        public ushort TotalChunks;
        public ushort ChunkNumber;
#endif
    };

    #endregion
}

