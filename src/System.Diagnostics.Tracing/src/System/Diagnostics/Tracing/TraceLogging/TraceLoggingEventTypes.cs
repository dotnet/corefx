// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Interlocked = System.Threading.Interlocked;

#if !ES_BUILD_AGAINST_DOTNET_V35
using Contract = System.Diagnostics.Contracts.Contract;
#else
using Contract = Microsoft.Diagnostics.Contracts.Internal.Contract;
#endif

#if ES_BUILD_STANDALONE
namespace Microsoft.Diagnostics.Tracing
#else
namespace System.Diagnostics.Tracing
#endif
{
    /// <summary>
    /// TraceLogging: Used when calling EventSource.WriteMultiMerge.
    /// Stores the type information to use when writing the event fields.
    /// </summary>
    public class TraceLoggingEventTypes
    {
        internal readonly TraceLoggingTypeInfo[] typeInfos;
        internal readonly string name;
        internal readonly EventTags tags;
        internal readonly byte level;
        internal readonly byte opcode;
        internal readonly EventKeywords keywords;
        internal readonly byte[] typeMetadata;
        internal readonly int scratchSize;
        internal readonly int dataCount;
        internal readonly int pinCount;
        private ConcurrentSet<KeyValuePair<string, EventTags>, NameInfo> nameInfos;

        /// <summary>
        /// Initializes a new instance of TraceLoggingEventTypes corresponding
        /// to the name, flags, and types provided. Always uses the default
        /// TypeInfo for each Type.
        /// </summary>
        /// <param name="name">
        /// The name to use when the name parameter passed to
        /// EventSource.Write is null. This value must not be null.
        /// </param>
        /// <param name="tags">
        /// Tags to add to the event if the tags are not set via options.
        /// </param>
        /// <param name="types">
        /// The types of the fields in the event. This value must not be null.
        /// </param>
        internal TraceLoggingEventTypes(
            string name,
            EventTags tags,
            params Type[] types)
            : this(tags, name, MakeArray(types))
        {
            return;
        }

        /// <summary>
        /// Returns a new instance of TraceLoggingEventInfo corresponding to the name,
        /// flags, and typeInfos provided.
        /// </summary>
        /// <param name="name">
        /// The name to use when the name parameter passed to
        /// EventSource.Write is null. This value must not be null.
        /// </param>
        /// <param name="tags">
        /// Tags to add to the event if the tags are not set via options.
        /// </param>
        /// <param name="typeInfos">
        /// The types of the fields in the event. This value must not be null.
        /// </param>
        /// <returns>
        /// An instance of TraceLoggingEventInfo with DefaultName set to the specified name
        /// and with the specified typeInfos.
        /// </returns>
        internal TraceLoggingEventTypes(
            string name,
            EventTags tags,
            params TraceLoggingTypeInfo[] typeInfos)
            : this(tags, name, MakeArray(typeInfos))
        {
            return;
        }

        internal TraceLoggingEventTypes(
            string name,
            EventTags tags,
            System.Reflection.ParameterInfo[] paramInfos)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Contract.EndContractBlock();

            this.typeInfos = MakeArray(paramInfos);
            this.name = name;
            this.tags = tags;
            this.level = Statics.DefaultLevel;

            var collector = new TraceLoggingMetadataCollector();
            for (int i = 0; i < typeInfos.Length; ++i)
            {
                var typeInfo = typeInfos[i];
                this.level = Statics.Combine((int)typeInfo.Level, this.level);
                this.opcode = Statics.Combine((int)typeInfo.Opcode, this.opcode);
                this.keywords |= typeInfo.Keywords;
                var paramName = paramInfos[i].Name;
                if (Statics.ShouldOverrideFieldName(paramName))
                {
                    paramName = typeInfo.Name;
                }
                typeInfo.WriteMetadata(collector, paramName, EventFieldFormat.Default);
            }

            this.typeMetadata = collector.GetMetadata();
            this.scratchSize = collector.ScratchSize;
            this.dataCount = collector.DataCount;
            this.pinCount = collector.PinCount;
        }

        private TraceLoggingEventTypes(
            EventTags tags,
            string defaultName,
            TraceLoggingTypeInfo[] typeInfos)
        {
            if (defaultName == null)
            {
                throw new ArgumentNullException("defaultName");
            }

            Contract.EndContractBlock();

            this.typeInfos = typeInfos;
            this.name = defaultName;
            this.tags = tags;
            this.level = Statics.DefaultLevel;

            var collector = new TraceLoggingMetadataCollector();
            foreach (var typeInfo in typeInfos)
            {
                this.level = Statics.Combine((int)typeInfo.Level, this.level);
                this.opcode = Statics.Combine((int)typeInfo.Opcode, this.opcode);
                this.keywords |= typeInfo.Keywords;
                typeInfo.WriteMetadata(collector, null, EventFieldFormat.Default);
            }

            this.typeMetadata = collector.GetMetadata();
            this.scratchSize = collector.ScratchSize;
            this.dataCount = collector.DataCount;
            this.pinCount = collector.PinCount;
        }

        /// <summary>
        /// Gets the default name that will be used for events with this descriptor.
        /// </summary>
        internal string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the default level that will be used for events with this descriptor.
        /// </summary>
        internal EventLevel Level
        {
            get { return (EventLevel)this.level; }
        }

        /// <summary>
        /// Gets the default opcode that will be used for events with this descriptor.
        /// </summary>
        internal EventOpcode Opcode
        {
            get { return (EventOpcode)this.opcode; }
        }

        /// <summary>
        /// Gets the default set of keywords that will added to events with this descriptor.
        /// </summary>
        internal EventKeywords Keywords
        {
            get { return (EventKeywords)this.keywords; }
        }

        /// <summary>
        /// Gets the default tags that will be added events with this descriptor.
        /// </summary>
        internal EventTags Tags
        {
            get { return this.tags; }
        }

        internal NameInfo GetNameInfo(string name, EventTags tags)
        {
            var ret = this.nameInfos.TryGet(new KeyValuePair<string, EventTags>(name, tags));
            if (ret == null)
            {
                ret = this.nameInfos.GetOrAdd(new NameInfo(name, tags, this.typeMetadata.Length));
            }

            return ret;
        }

        private TraceLoggingTypeInfo[] MakeArray(System.Reflection.ParameterInfo[] paramInfos)
        {
            if (paramInfos == null)
            {
                throw new ArgumentNullException("paramInfos");
            }

            Contract.EndContractBlock();

            var recursionCheck = new List<Type>(paramInfos.Length);
            var result = new TraceLoggingTypeInfo[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; ++i)
            {
                result[i] = TraceLoggingTypeInfo.GetInstance(paramInfos[i].ParameterType, recursionCheck);
            }

            return result;
        }

        private static TraceLoggingTypeInfo[] MakeArray(Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }

            Contract.EndContractBlock();

            var recursionCheck = new List<Type>(types.Length);
            var result = new TraceLoggingTypeInfo[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                result[i] = TraceLoggingTypeInfo.GetInstance(types[i], recursionCheck);
            }

            return result;
        }

        private static TraceLoggingTypeInfo[] MakeArray(
            TraceLoggingTypeInfo[] typeInfos)
        {
            if (typeInfos == null)
            {
                throw new ArgumentNullException("typeInfos");
            }

            Contract.EndContractBlock();

            return (TraceLoggingTypeInfo[])typeInfos.Clone(); ;
        }
    }
}
