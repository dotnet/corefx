// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This program uses code hyperlinks available as part of the HyperAddin Visual Studio plug-in.
// It is available from http://www.codeplex.com/hyperAddin 

#if PLATFORM_WINDOWS
#define FEATURE_MANAGED_ETW
#endif // PLATFORM_WINDOWS

#if ES_BUILD_STANDALONE
#define FEATURE_MANAGED_ETW_CHANNELS
// #define FEATURE_ADVANCED_MANAGED_ETW_CHANNELS
#endif

#if ES_BUILD_STANDALONE
using Environment = Microsoft.Diagnostics.Tracing.Internal.Environment;
using EventDescriptor = Microsoft.Diagnostics.Tracing.EventDescriptor;
#endif

using System;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.ObjectModel;

#if !ES_BUILD_AGAINST_DOTNET_V35
using Contract = System.Diagnostics.Contracts.Contract;
using System.Collections.Generic;
using System.Text;
#else
using Contract = Microsoft.Diagnostics.Contracts.Internal.Contract;
using System.Collections.Generic;
using System.Text;
#endif

#if ES_BUILD_STANDALONE
namespace Microsoft.Diagnostics.Tracing
#else
namespace System.Diagnostics.Tracing
#endif
{
    public partial class EventSource
    {
#if FEATURE_MANAGED_ETW
        private byte[] providerMetadata;
#endif

        /// <summary>
        /// Construct an EventSource with a given name for non-contract based events (e.g. those using the Write() API).
        /// </summary>
        /// <param name="eventSourceName">
        /// The name of the event source. Must not be null.
        /// </param>
        public EventSource(
            string eventSourceName)
            : this(eventSourceName, EventSourceSettings.EtwSelfDescribingEventFormat)
        { }

        /// <summary>
        /// Construct an EventSource with a given name for non-contract based events (e.g. those using the Write() API).
        /// </summary>
        /// <param name="eventSourceName">
        /// The name of the event source. Must not be null.
        /// </param>
        /// <param name="config">
        /// Configuration options for the EventSource as a whole. 
        /// </param>
        public EventSource(
            string eventSourceName,
            EventSourceSettings config)
            : this(eventSourceName, config, null) { }

        /// <summary>
        /// Construct an EventSource with a given name for non-contract based events (e.g. those using the Write() API).
        /// 
        /// Also specify a list of key-value pairs called traits (you must pass an even number of strings).   
        /// The first string is the key and the second is the value.   These are not interpreted by EventSource
        /// itself but may be interpreted the listeners.  Can be fetched with GetTrait(string).   
        /// </summary>
        /// <param name="eventSourceName">
        /// The name of the event source. Must not be null.
        /// </param>
        /// <param name="config">
        /// Configuration options for the EventSource as a whole. 
        /// </param>
        /// <param name="traits">A collection of key-value strings (must be an even number).</param>
        public EventSource(
            string eventSourceName,
            EventSourceSettings config,
            params string[] traits)
            : this(
                eventSourceName == null ? new Guid() : GenerateGuidFromName(eventSourceName.ToUpperInvariant()),
                eventSourceName,
                config, traits)
        {
            if (eventSourceName == null)
            {
                throw new ArgumentNullException(nameof(eventSourceName));
            }
        }

        /// <summary>
        /// Writes an event with no fields and default options.
        /// (Native API: EventWriteTransfer)
        /// </summary>
        /// <param name="eventName">The name of the event. Must not be null.</param>
        public unsafe void Write(string eventName)
        {
            if (eventName == null)
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (!this.IsEnabled())
            {
                return;
            }

            var options = new EventSourceOptions();
            this.WriteImpl(eventName, ref options, null, null, null, SimpleEventTypes<EmptyStruct>.Instance);
        }

        /// <summary>
        /// Writes an event with no fields.
        /// (Native API: EventWriteTransfer)
        /// </summary>
        /// <param name="eventName">The name of the event. Must not be null.</param>
        /// <param name="options">
        /// Options for the event, such as the level, keywords, and opcode. Unset
        /// options will be set to default values.
        /// </param>
        public unsafe void Write(string eventName, EventSourceOptions options)
        {
            if (eventName == null)
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (!this.IsEnabled())
            {
                return;
            }

            this.WriteImpl(eventName, ref options, null, null, null, SimpleEventTypes<EmptyStruct>.Instance);
        }

        /// <summary>
        /// Writes an event.
        /// (Native API: EventWriteTransfer)
        /// </summary>
        /// <typeparam name="T">
        /// The type that defines the event and its payload. This must be an
        /// anonymous type or a type with an [EventData] attribute.
        /// </typeparam>
        /// <param name="eventName">
        /// The name for the event. If null, the event name is automatically
        /// determined based on T, either from the Name property of T's EventData
        /// attribute or from typeof(T).Name.
        /// </param>
        /// <param name="data">
        /// The object containing the event payload data. The type T must be
        /// an anonymous type or a type with an [EventData] attribute. The
        /// public instance properties of data will be written recursively to
        /// create the fields of the event.
        /// </param>
        public unsafe void Write<T>(
            string eventName,
            T data)
        {
            if (!this.IsEnabled())
            {
                return;
            }

            var options = new EventSourceOptions();
            this.WriteImpl(eventName, ref options, data, null, null, SimpleEventTypes<T>.Instance);
        }

        /// <summary>
        /// Writes an event.
        /// (Native API: EventWriteTransfer)
        /// </summary>
        /// <typeparam name="T">
        /// The type that defines the event and its payload. This must be an
        /// anonymous type or a type with an [EventData] attribute.
        /// </typeparam>
        /// <param name="eventName">
        /// The name for the event. If null, the event name is automatically
        /// determined based on T, either from the Name property of T's EventData
        /// attribute or from typeof(T).Name.
        /// </param>
        /// <param name="options">
        /// Options for the event, such as the level, keywords, and opcode. Unset
        /// options will be set to default values.
        /// </param>
        /// <param name="data">
        /// The object containing the event payload data. The type T must be
        /// an anonymous type or a type with an [EventData] attribute. The
        /// public instance properties of data will be written recursively to
        /// create the fields of the event.
        /// </param>
        public unsafe void Write<T>(
            string eventName,
            EventSourceOptions options,
            T data)
        {
            if (!this.IsEnabled())
            {
                return;
            }

            this.WriteImpl(eventName, ref options, data, null, null, SimpleEventTypes<T>.Instance);
        }

        /// <summary>
        /// Writes an event.
        /// This overload is for use with extension methods that wish to efficiently
        /// forward the options or data parameter without performing an extra copy.
        /// (Native API: EventWriteTransfer)
        /// </summary>
        /// <typeparam name="T">
        /// The type that defines the event and its payload. This must be an
        /// anonymous type or a type with an [EventData] attribute.
        /// </typeparam>
        /// <param name="eventName">
        /// The name for the event. If null, the event name is automatically
        /// determined based on T, either from the Name property of T's EventData
        /// attribute or from typeof(T).Name.
        /// </param>
        /// <param name="options">
        /// Options for the event, such as the level, keywords, and opcode. Unset
        /// options will be set to default values.
        /// </param>
        /// <param name="data">
        /// The object containing the event payload data. The type T must be
        /// an anonymous type or a type with an [EventData] attribute. The
        /// public instance properties of data will be written recursively to
        /// create the fields of the event.
        /// </param>
        public unsafe void Write<T>(
            string eventName,
            ref EventSourceOptions options,
            ref T data)
        {
            if (!this.IsEnabled())
            {
                return;
            }

            this.WriteImpl(eventName, ref options, data, null, null, SimpleEventTypes<T>.Instance);
        }

        /// <summary>
        /// Writes an event.
        /// This overload is meant for clients that need to manipuate the activityId
        /// and related ActivityId for the event.  
        /// </summary>
        /// <typeparam name="T">
        /// The type that defines the event and its payload. This must be an
        /// anonymous type or a type with an [EventData] attribute.
        /// </typeparam>
        /// <param name="eventName">
        /// The name for the event. If null, the event name is automatically
        /// determined based on T, either from the Name property of T's EventData
        /// attribute or from typeof(T).Name.
        /// </param>
        /// <param name="options">
        /// Options for the event, such as the level, keywords, and opcode. Unset
        /// options will be set to default values.
        /// </param>
        /// <param name="activityId">
        /// The GUID of the activity associated with this event.
        /// </param>
        /// <param name="relatedActivityId">
        /// The GUID of another activity that is related to this activity, or Guid.Empty
        /// if there is no related activity. Most commonly, the Start operation of a
        /// new activity specifies a parent activity as its related activity.
        /// </param>
        /// <param name="data">
        /// The object containing the event payload data. The type T must be
        /// an anonymous type or a type with an [EventData] attribute. The
        /// public instance properties of data will be written recursively to
        /// create the fields of the event.
        /// </param>
        public unsafe void Write<T>(
            string eventName,
            ref EventSourceOptions options,
            ref Guid activityId,
            ref Guid relatedActivityId,
            ref T data)
        {
            if (!this.IsEnabled())
            {
                return;
            }

            fixed (Guid* pActivity = &activityId, pRelated = &relatedActivityId)
            {
                this.WriteImpl(
                    eventName,
                    ref options,
                    data,
                    pActivity,
                    relatedActivityId == Guid.Empty ? null : pRelated,
                    SimpleEventTypes<T>.Instance);
            }
        }

        /// <summary>
        /// Writes an extended event, where the values of the event are the
        /// combined properties of any number of values. This method is
        /// intended for use in advanced logging scenarios that support a
        /// dynamic set of event context providers.
        /// This method does a quick check on whether this event is enabled.
        /// </summary>
        /// <param name="eventName">
        /// The name for the event. If null, the name from eventTypes is used.
        /// (Note that providing the event name via the name parameter is slightly
        /// less efficient than using the name from eventTypes.)
        /// </param>
        /// <param name="options">
        /// Optional overrides for the event, such as the level, keyword, opcode,
        /// activityId, and relatedActivityId. Any settings not specified by options
        /// are obtained from eventTypes.
        /// </param>
        /// <param name="eventTypes">
        /// Information about the event and the types of the values in the event.
        /// Must not be null. Note that the eventTypes object should be created once and
        /// saved. It should not be recreated for each event.
        /// </param>
        /// <param name="activityID">
        /// A pointer to the activity ID GUID to log 
        /// </param>
        /// <param name="childActivityID">
        /// A pointer to the child activity ID to log (can be null) </param>
        /// <param name="values">
        /// The values to include in the event. Must not be null. The number and types of
        /// the values must match the number and types of the fields described by the
        /// eventTypes parameter.
        /// </param>
        private unsafe void WriteMultiMerge(
            string eventName,
            ref EventSourceOptions options,
            TraceLoggingEventTypes eventTypes,
             Guid* activityID,
             Guid* childActivityID,
            params object[] values)
        {
            if (!this.IsEnabled())
            {
                return;
            }
            byte level = (options.valuesSet & EventSourceOptions.levelSet) != 0
                ? options.level
                : eventTypes.level;
            EventKeywords keywords = (options.valuesSet & EventSourceOptions.keywordsSet) != 0
                ? options.keywords
                : eventTypes.keywords;

            if (this.IsEnabled((EventLevel)level, keywords))
            {
                WriteMultiMergeInner(eventName, ref options, eventTypes, activityID, childActivityID, values);
            }
        }

        /// <summary>
        /// Writes an extended event, where the values of the event are the
        /// combined properties of any number of values. This method is
        /// intended for use in advanced logging scenarios that support a
        /// dynamic set of event context providers.
        /// Attention: This API does not check whether the event is enabled or not. 
        /// Please use WriteMultiMerge to avoid spending CPU cycles for events that are 
        /// not enabled.
        /// </summary>
        /// <param name="eventName">
        /// The name for the event. If null, the name from eventTypes is used.
        /// (Note that providing the event name via the name parameter is slightly
        /// less efficient than using the name from eventTypes.)
        /// </param>
        /// <param name="options">
        /// Optional overrides for the event, such as the level, keyword, opcode,
        /// activityId, and relatedActivityId. Any settings not specified by options
        /// are obtained from eventTypes.
        /// </param>
        /// <param name="eventTypes">
        /// Information about the event and the types of the values in the event.
        /// Must not be null. Note that the eventTypes object should be created once and
        /// saved. It should not be recreated for each event.
        /// </param>
        /// <param name="activityID">
        /// A pointer to the activity ID GUID to log 
        /// </param>
        /// <param name="childActivityID">
        /// A pointer to the child activity ID to log (can be null)
        /// </param>
        /// <param name="values">
        /// The values to include in the event. Must not be null. The number and types of
        /// the values must match the number and types of the fields described by the
        /// eventTypes parameter.
        /// </param>
        private unsafe void WriteMultiMergeInner(
            string eventName,
            ref EventSourceOptions options,
            TraceLoggingEventTypes eventTypes,
            Guid* activityID,
            Guid* childActivityID,
            params object[] values)
        {
#if FEATURE_MANAGED_ETW
            int identity = 0;
            byte level = (options.valuesSet & EventSourceOptions.levelSet) != 0
                ? options.level
                : eventTypes.level;
            byte opcode = (options.valuesSet & EventSourceOptions.opcodeSet) != 0
                ? options.opcode
                : eventTypes.opcode;
            EventTags tags = (options.valuesSet & EventSourceOptions.tagsSet) != 0
                ? options.tags
                : eventTypes.Tags;
            EventKeywords keywords = (options.valuesSet & EventSourceOptions.keywordsSet) != 0
                ? options.keywords
                : eventTypes.keywords;

            var nameInfo = eventTypes.GetNameInfo(eventName ?? eventTypes.Name, tags);
            if (nameInfo == null)
            {
                return;
            }
            identity = nameInfo.identity;
            EventDescriptor descriptor = new EventDescriptor(identity, level, opcode, (long)keywords);

            var pinCount = eventTypes.pinCount;
            var scratch = stackalloc byte[eventTypes.scratchSize];
            var descriptors = stackalloc EventData[eventTypes.dataCount + 3];

            var pins = stackalloc GCHandle[pinCount];
            for (int i = 0; i < pinCount; i++)
                pins[i] = default(GCHandle);

            fixed (byte*
                pMetadata0 = this.providerMetadata,
                pMetadata1 = nameInfo.nameMetadata,
                pMetadata2 = eventTypes.typeMetadata)
            {
                descriptors[0].SetMetadata(pMetadata0, this.providerMetadata.Length, 2);
                descriptors[1].SetMetadata(pMetadata1, nameInfo.nameMetadata.Length, 1);
                descriptors[2].SetMetadata(pMetadata2, eventTypes.typeMetadata.Length, 1);

#if (!ES_BUILD_PCL && !ES_BUILD_PN)
                System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();
#endif
                try
                {
                    DataCollector.ThreadInstance.Enable(
                        scratch,
                        eventTypes.scratchSize,
                        descriptors + 3,
                        eventTypes.dataCount,
                        pins,
                        pinCount);

                    for (int i = 0; i < eventTypes.typeInfos.Length; i++)
                    {
                        var info = eventTypes.typeInfos[i];
                        info.WriteData(TraceLoggingDataCollector.Instance, info.PropertyValueFactory(values[i]));
                    }

                    this.WriteEventRaw(
                        eventName,
                        ref descriptor,
                        activityID,
                        childActivityID,
                        (int)(DataCollector.ThreadInstance.Finish() - descriptors),
                        (IntPtr)descriptors);
                }
                finally
                {
                    this.WriteCleanup(pins, pinCount);
                }
            }
#endif // FEATURE_MANAGED_ETW
        }

        /// <summary>
        /// Writes an extended event, where the values of the event have already
        /// been serialized in "data".
        /// </summary>
        /// <param name="eventName">
        /// The name for the event. If null, the name from eventTypes is used.
        /// (Note that providing the event name via the name parameter is slightly
        /// less efficient than using the name from eventTypes.)
        /// </param>
        /// <param name="options">
        /// Optional overrides for the event, such as the level, keyword, opcode,
        /// activityId, and relatedActivityId. Any settings not specified by options
        /// are obtained from eventTypes.
        /// </param>
        /// <param name="eventTypes">
        /// Information about the event and the types of the values in the event.
        /// Must not be null. Note that the eventTypes object should be created once and
        /// saved. It should not be recreated for each event.
        /// </param>
        /// <param name="activityID">
        /// A pointer to the activity ID GUID to log 
        /// </param>
        /// <param name="childActivityID">
        /// A pointer to the child activity ID to log (can be null)
        /// </param> 
        /// <param name="data">
        /// The previously serialized values to include in the event. Must not be null.
        /// The number and types of the values must match the number and types of the 
        /// fields described by the eventTypes parameter.
        /// </param>
        internal unsafe void WriteMultiMerge(
            string eventName,
            ref EventSourceOptions options,
            TraceLoggingEventTypes eventTypes,
            Guid* activityID,
            Guid* childActivityID,
            EventData* data)
        {
#if FEATURE_MANAGED_ETW
            if (!this.IsEnabled())
            {
                return;
            }

            fixed (EventSourceOptions* pOptions = &options)
            {
                EventDescriptor descriptor;
                var nameInfo = this.UpdateDescriptor(eventName, eventTypes, ref options, out descriptor);
                if (nameInfo == null)
                {
                    return;
                }

                // We make a descriptor for each EventData, and because we morph strings to counted strings
                // we may have 2 for each arg, so we allocate enough for this.  
                var descriptors = stackalloc EventData[eventTypes.dataCount + eventTypes.typeInfos.Length * 2 + 3];

                fixed (byte*
                    pMetadata0 = this.providerMetadata,
                    pMetadata1 = nameInfo.nameMetadata,
                    pMetadata2 = eventTypes.typeMetadata)
                {
                    descriptors[0].SetMetadata(pMetadata0, this.providerMetadata.Length, 2);
                    descriptors[1].SetMetadata(pMetadata1, nameInfo.nameMetadata.Length, 1);
                    descriptors[2].SetMetadata(pMetadata2, eventTypes.typeMetadata.Length, 1);
                    int numDescrs = 3;

                    for (int i = 0; i < eventTypes.typeInfos.Length; i++)
                    {
                        // Until M3, we need to morph strings to a counted representation
                        // When TDH supports null terminated strings, we can remove this.  
                        if (eventTypes.typeInfos[i].DataType == typeof(string))
                        {
                            // Write out the size of the string 
                            descriptors[numDescrs].DataPointer = (IntPtr) (&descriptors[numDescrs + 1].m_Size);
                            descriptors[numDescrs].m_Size = 2;
                            numDescrs++;

                            descriptors[numDescrs].m_Ptr = data[i].m_Ptr;
                            descriptors[numDescrs].m_Size = data[i].m_Size - 2;   // Remove the null terminator
                            numDescrs++;
                        }
                        else
                        {
                            descriptors[numDescrs].m_Ptr = data[i].m_Ptr;
                            descriptors[numDescrs].m_Size = data[i].m_Size;

                            // old conventions for bool is 4 bytes, but meta-data assumes 1.  
                            if (data[i].m_Size == 4 && eventTypes.typeInfos[i].DataType == typeof(bool))
                                descriptors[numDescrs].m_Size = 1;

                            numDescrs++;
                        }
                    }

                    this.WriteEventRaw(
                        eventName,
                        ref descriptor,
                        activityID,
                        childActivityID,
                        numDescrs,
                        (IntPtr)descriptors);
                }
            }
#endif // FEATURE_MANAGED_ETW
        }

        private unsafe void WriteImpl(
            string eventName,
            ref EventSourceOptions options,
            object data,
            Guid* pActivityId,
            Guid* pRelatedActivityId,
            TraceLoggingEventTypes eventTypes)
        {
            try
            {
                fixed (EventSourceOptions* pOptions = &options)
                {
                    EventDescriptor descriptor;
                    options.Opcode = options.IsOpcodeSet ? options.Opcode : GetOpcodeWithDefault(options.Opcode, eventName);
                    var nameInfo = this.UpdateDescriptor(eventName, eventTypes, ref options, out descriptor);
                    if (nameInfo == null)
                    {
                        return;
                    }

#if FEATURE_MANAGED_ETW
                    var pinCount = eventTypes.pinCount;
                    var scratch = stackalloc byte[eventTypes.scratchSize];
                    var descriptors = stackalloc EventData[eventTypes.dataCount + 3];

                    var pins = stackalloc GCHandle[pinCount];
                    for (int i = 0; i < pinCount; i++)
                        pins[i] = default(GCHandle);

                    fixed (byte*
                        pMetadata0 = this.providerMetadata,
                        pMetadata1 = nameInfo.nameMetadata,
                        pMetadata2 = eventTypes.typeMetadata)
                    {
                        descriptors[0].SetMetadata(pMetadata0, this.providerMetadata.Length, 2);
                        descriptors[1].SetMetadata(pMetadata1, nameInfo.nameMetadata.Length, 1);
                        descriptors[2].SetMetadata(pMetadata2, eventTypes.typeMetadata.Length, 1);
#endif // FEATURE_MANAGED_ETW

#if (!ES_BUILD_PCL && !ES_BUILD_PN)
                        System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();
#endif
                        EventOpcode opcode = (EventOpcode)descriptor.Opcode;

                        Guid activityId = Guid.Empty;
                        Guid relatedActivityId = Guid.Empty;
                        if (pActivityId == null && pRelatedActivityId == null &&
                           ((options.ActivityOptions & EventActivityOptions.Disable) == 0))
                        {
                            if (opcode == EventOpcode.Start)
                            {
                                m_activityTracker.OnStart(m_name, eventName, 0, ref activityId, ref relatedActivityId, options.ActivityOptions);
                            }
                            else if (opcode == EventOpcode.Stop)
                            {
                                m_activityTracker.OnStop(m_name, eventName, 0, ref activityId);
                            }
                            if (activityId != Guid.Empty)
                                pActivityId = &activityId;
                            if (relatedActivityId != Guid.Empty)
                                pRelatedActivityId = &relatedActivityId;
                        }

                        try
                        {
#if FEATURE_MANAGED_ETW
                            DataCollector.ThreadInstance.Enable(
                                scratch,
                                eventTypes.scratchSize,
                                descriptors + 3,
                                eventTypes.dataCount,
                                pins,
                                pinCount);

                            var info = eventTypes.typeInfos[0];
                            info.WriteData(TraceLoggingDataCollector.Instance, info.PropertyValueFactory(data));

                            this.WriteEventRaw(
                                eventName,
                                ref descriptor,
                                pActivityId,
                                pRelatedActivityId,
                                (int)(DataCollector.ThreadInstance.Finish() - descriptors),
                                (IntPtr)descriptors);
#endif // FEATURE_MANAGED_ETW

                            // TODO enable filtering for listeners.
                            if (m_Dispatchers != null)
                            {
                                var eventData = (EventPayload)(eventTypes.typeInfos[0].GetData(data));
                                WriteToAllListeners(eventName, ref descriptor, nameInfo.tags, pActivityId, eventData);
                            }

                        }
                        catch (Exception ex)
                        {
                            if (ex is EventSourceException)
                                throw;
                            else
                                ThrowEventSourceException(eventName, ex);
                        }
#if FEATURE_MANAGED_ETW
                        finally
                        {
                            this.WriteCleanup(pins, pinCount);
                        }
                    }
#endif // FEATURE_MANAGED_ETW
                }
            }
            catch (Exception ex)
            {
                if (ex is EventSourceException)
                    throw;
                else
                    ThrowEventSourceException(eventName, ex);
            }
        }

        private unsafe void WriteToAllListeners(string eventName, ref EventDescriptor eventDescriptor, EventTags tags, Guid* pActivityId, EventPayload payload)
        {
            EventWrittenEventArgs eventCallbackArgs = new EventWrittenEventArgs(this);
            eventCallbackArgs.EventName = eventName;
            eventCallbackArgs.m_level = (EventLevel) eventDescriptor.Level;
            eventCallbackArgs.m_keywords = (EventKeywords) eventDescriptor.Keywords;
            eventCallbackArgs.m_opcode = (EventOpcode) eventDescriptor.Opcode;
            eventCallbackArgs.m_tags = tags;

            // Self described events do not have an id attached. We mark it internally with -1.
            eventCallbackArgs.EventId = -1;
            if (pActivityId != null)
                eventCallbackArgs.RelatedActivityId = *pActivityId;

            if (payload != null)
            {
                eventCallbackArgs.Payload = new ReadOnlyCollection<object>((IList<object>)payload.Values);
                eventCallbackArgs.PayloadNames = new ReadOnlyCollection<string>((IList<string>)payload.Keys);
            }

            DispatchToAllListeners(-1, pActivityId, eventCallbackArgs);
        }

#if (!ES_BUILD_PCL && !ES_BUILD_PN)
        [System.Runtime.ConstrainedExecution.ReliabilityContract(
            System.Runtime.ConstrainedExecution.Consistency.WillNotCorruptState,
            System.Runtime.ConstrainedExecution.Cer.Success)]
#endif
        [NonEvent]
        private unsafe void WriteCleanup(GCHandle* pPins, int cPins)
        {
            DataCollector.ThreadInstance.Disable();

            for (int i = 0; i < cPins; i++)
            {
                if (pPins[i].IsAllocated)
                {
                    pPins[i].Free();
                }
            }
        }

        private void InitializeProviderMetadata()
        {
#if FEATURE_MANAGED_ETW
            if (m_traits != null)
            {
                List<byte> traitMetaData = new List<byte>(100);
                for (int i = 0; i < m_traits.Length - 1; i += 2)
                {
                    if (m_traits[i].StartsWith("ETW_", StringComparison.Ordinal))
                    {
                        string etwTrait = m_traits[i].Substring(4);
                        byte traitNum;
                        if (!byte.TryParse(etwTrait, out traitNum))
                        {
                            if (etwTrait == "GROUP")
                            {
                                traitNum = 1;
                            }
                            else
                            {
                                throw new ArgumentException(SR.Format(SR.EventSource_UnknownEtwTrait, etwTrait), "traits");
                            }
                        }
                        string value = m_traits[i + 1];
                        int lenPos = traitMetaData.Count;
                        traitMetaData.Add(0);                                           // Emit size (to be filled in later) 
                        traitMetaData.Add(0);
                        traitMetaData.Add(traitNum);                                    // Emit Trait number
                        var valueLen = AddValueToMetaData(traitMetaData, value) + 3;    // Emit the value bytes +3 accounts for 3 bytes we emited above.  
                        traitMetaData[lenPos] = unchecked((byte)valueLen);              // Fill in size
                        traitMetaData[lenPos + 1] = unchecked((byte)(valueLen >> 8));
                    }
                }
                providerMetadata = Statics.MetadataForString(this.Name, 0, traitMetaData.Count, 0);
                int startPos = providerMetadata.Length - traitMetaData.Count;
                foreach (var b in traitMetaData)
                    providerMetadata[startPos++] = b;
            }
            else
                providerMetadata = Statics.MetadataForString(this.Name, 0, 0, 0);
#endif //FEATURE_MANAGED_ETW
        }

        private static int AddValueToMetaData(List<byte> metaData, string value)
        {
            if (value.Length == 0)
                return 0;

            int startPos = metaData.Count;
            char firstChar = value[0];

            if (firstChar == '@')
                metaData.AddRange(Encoding.UTF8.GetBytes(value.Substring(1)));
            else if (firstChar == '{')
                metaData.AddRange(new Guid(value).ToByteArray());
            else if (firstChar == '#')
            {
                for (int i = 1; i < value.Length; i++)
                {
                    if (value[i] != ' ')        // Skip spaces between bytes.  
                    {
                        if (!(i + 1 < value.Length))
                        {
                            throw new ArgumentException(SR.EventSource_EvenHexDigits, "traits");
                        }
                        metaData.Add((byte)(HexDigit(value[i]) * 16 + HexDigit(value[i + 1])));
                        i++;
                    }
                }
            }
            else if ('A' <= firstChar || ' ' == firstChar)  // Is it alphabetic or space (excludes digits and most punctuation). 
            {
                metaData.AddRange(Encoding.UTF8.GetBytes(value));
            }
            else
            {
                throw new ArgumentException(SR.Format(SR.EventSource_IllegalValue, value), "traits");
            }

            return metaData.Count - startPos;
        }

        /// <summary>
        /// Returns a value 0-15 if 'c' is a hexadecimal digit.   If  it throws an argument exception.  
        /// </summary>
        private static int HexDigit(char c)
        {
            if ('0' <= c && c <= '9')
            {
                return (c - '0');
            }
            if ('a' <= c)
            {
                c = unchecked((char)(c - ('a' - 'A')));        // Convert to lower case
            }
            if ('A' <= c && c <= 'F')
            {
                return (c - 'A' + 10);
            }
            
            throw new ArgumentException(SR.Format(SR.EventSource_BadHexDigit, c), "traits");
        }

        private NameInfo UpdateDescriptor(
            string name,
            TraceLoggingEventTypes eventInfo,
            ref EventSourceOptions options,
            out EventDescriptor descriptor)
        {
            NameInfo nameInfo = null;
            int identity = 0;
            byte level = (options.valuesSet & EventSourceOptions.levelSet) != 0
                ? options.level
                : eventInfo.level;
            byte opcode = (options.valuesSet & EventSourceOptions.opcodeSet) != 0
                ? options.opcode
                : eventInfo.opcode;
            EventTags tags = (options.valuesSet & EventSourceOptions.tagsSet) != 0
                ? options.tags
                : eventInfo.Tags;
            EventKeywords keywords = (options.valuesSet & EventSourceOptions.keywordsSet) != 0
                ? options.keywords
                : eventInfo.keywords;

            if (this.IsEnabled((EventLevel)level, keywords))
            {
                nameInfo = eventInfo.GetNameInfo(name ?? eventInfo.Name, tags);
                identity = nameInfo.identity;
            }

            descriptor = new EventDescriptor(identity, level, opcode, (long)keywords);
            return nameInfo;
        }
    }
}
