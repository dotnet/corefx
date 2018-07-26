// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Threading;
using Microsoft.Win32;

namespace System.Diagnostics.PerformanceData
{
    /// <summary>
    /// CounterSet is equivalent to "Counter Object" in native performance counter terminology,
    /// or "Counter Category" in previous framework releases. It defines a abstract grouping of
    /// counters, where each counter defines measurable matrix. In the new performance counter
    /// infrastructure, CounterSet is defined by GUID called CounterSetGuid, and is hosted inside
    /// provider application, which is also defined by another GUID called ProviderGuid.
    /// </summary>    
    public class CounterSet : IDisposable
    {
        private static readonly bool s_platformNotSupported = (Environment.OSVersion.Version.Major < 6);
        internal PerfProvider m_provider;
        internal Guid m_providerGuid;
        internal Guid m_counterSet;
        internal CounterSetInstanceType m_instType;
        private readonly object m_lockObject;
        private bool m_instanceCreated;
        internal Dictionary<string, int> m_stringToId;
        internal Dictionary<int, CounterType> m_idToCounter;

        /// <summary>
        /// CounterSet constructor.
        /// </summary>
        /// <param name="providerGuid">ProviderGuid identifies the provider application. A provider identified by ProviderGuid could publish several CounterSets defined by different CounterSetGuids</param>
        /// <param name="counterSetGuid">CounterSetGuid identifies the specific CounterSet. CounterSetGuid should be unique.</param>
        /// <param name="instanceType">One of defined CounterSetInstanceType values</param>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Approved")]
        public CounterSet(Guid providerGuid, Guid counterSetGuid, CounterSetInstanceType instanceType)
        {
            // Check only the mayor version, only support Windows Vista and later.
            if (s_platformNotSupported)
            {
                throw new System.PlatformNotSupportedException(SR.Perflib_PlatformNotSupported);
            }
            if (!PerfProviderCollection.ValidateCounterSetInstanceType(instanceType))
            {
                throw new ArgumentException(SR.Format(SR.Perflib_Argument_InvalidCounterSetInstanceType, instanceType), "instanceType");
            }

            m_providerGuid = providerGuid;
            m_counterSet = counterSetGuid;
            m_instType = instanceType;
            PerfProviderCollection.RegisterCounterSet(m_counterSet);
            m_provider = PerfProviderCollection.QueryProvider(m_providerGuid);
            m_lockObject = new object();
            m_stringToId = new Dictionary<string, int>();
            m_idToCounter = new Dictionary<int, CounterType>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CounterSet()
        {
            Dispose(false);
        }

        [System.Security.SecuritySafeCritical]
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                PerfProviderCollection.UnregisterCounterSet(m_counterSet);
                if (m_instanceCreated)
                {
                    if (m_provider != null)
                    {
                        lock (m_lockObject)
                        {
                            if (m_provider != null)
                            {
                                Interlocked.Decrement(ref m_provider.m_counterSet);
                                if (m_provider.m_counterSet <= 0)
                                {
                                    PerfProviderCollection.RemoveProvider(m_providerGuid);
                                }
                                m_provider = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add non-displayable new counter to CounterSet; that is, perfmon would not display the counter.
        /// </summary>
        /// <param name="counterId">CounterId uniquely identifies the counter within CounterSet</param>
        /// <param name="counterType">One of defined CounterType values</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public void AddCounter(int counterId, CounterType counterType)
        {
            if (m_provider == null)
            {
                throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_NoActiveProvider, m_providerGuid));
            }
            if (!PerfProviderCollection.ValidateCounterType(counterType))
            {
                throw new ArgumentException(SR.Format(SR.Perflib_Argument_InvalidCounterType, counterType), "counterType");
            }
            if (m_instanceCreated)
            {
                throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_AddCounterAfterInstance, m_counterSet));
            }

            lock (m_lockObject)
            {
                if (m_instanceCreated)
                {
                    throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_AddCounterAfterInstance, m_counterSet));
                }
                if (m_idToCounter.ContainsKey(counterId))
                {
                    throw new ArgumentException(SR.Format(SR.Perflib_Argument_CounterAlreadyExists, counterId, m_counterSet), "CounterId");
                }

                m_idToCounter.Add(counterId, counterType);
            }
        }

        /// <summary>
        /// Add named new counter to CounterSet.
        /// </summary>
        /// <param name="counterId">CounterId uniquely identifies the counter within CounterSet</param>
        /// <param name="counterType">One of defined CounterType values</param>
        /// <param name="counterName">This is friendly name to help provider developers as indexer. and it might not match what is displayed in counter consumption applications lie perfmon.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public void AddCounter(int counterId, CounterType counterType, String counterName)
        {
            if (counterName == null)
            {
                throw new ArgumentNullException("CounterName");
            }
            if (counterName.Length == 0)
            {
                throw new ArgumentException(SR.Perflib_Argument_EmptyCounterName, "counterName");
            }
            if (!PerfProviderCollection.ValidateCounterType(counterType))
            {
                throw new ArgumentException(SR.Format(SR.Perflib_Argument_InvalidCounterType, counterType), "counterType");
            }
            if (m_provider == null)
            {
                throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_NoActiveProvider, m_providerGuid));
            }
            if (m_instanceCreated)
            {
                throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_AddCounterAfterInstance, m_counterSet));
            }

            lock (m_lockObject)
            {
                if (m_instanceCreated)
                {
                    throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_AddCounterAfterInstance, m_counterSet));
                }
                if (m_stringToId.ContainsKey(counterName))
                {
                    throw new ArgumentException(SR.Format(SR.Perflib_Argument_CounterNameAlreadyExists, counterName, m_counterSet), "CounterName");
                }
                if (m_idToCounter.ContainsKey(counterId))
                {
                    throw new ArgumentException(SR.Format(SR.Perflib_Argument_CounterAlreadyExists, counterId, m_counterSet), "CounterId");
                }

                m_stringToId.Add(counterName, counterId);
                m_idToCounter.Add(counterId, counterType);
            }
        }

        /// <summary>
        /// Create instances of the CounterSet. Created CounterSetInstance identifies active identity and tracks raw counter data for that identity.
        /// </summary>
        /// <param name="instanceName">Friendly name identifies the instance. InstanceName would be shown in counter consumption applications like perfmon.</param>
        /// <returns>CounterSetInstance object</returns>
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public CounterSetInstance CreateCounterSetInstance(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }
            if (instanceName.Length == 0)
            {
                throw new ArgumentException(SR.Format(SR.Perflib_Argument_EmptyInstanceName), "instanceName");
            }
            if (m_provider == null)
            {
                throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_NoActiveProvider, m_providerGuid));
            }
            if (!m_instanceCreated)
            {
                lock (m_lockObject)
                {
                    if (!m_instanceCreated)
                    {
                        if (m_provider == null)
                        {
                            throw new ArgumentException(SR.Format(SR.Perflib_Argument_ProviderNotFound, m_providerGuid), "ProviderGuid");
                        }
                        if (m_provider.m_hProvider.IsInvalid)
                        {
                            throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_NoActiveProvider, m_providerGuid));
                        }
                        if (m_idToCounter.Count == 0)
                        {
                            throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_CounterSetContainsNoCounter, m_counterSet));
                        }

                        uint Status = (uint)UnsafeNativeMethods.ERROR_SUCCESS;

                        unsafe
                        {
                            uint CounterSetInfoSize = (uint)sizeof(UnsafeNativeMethods.PerfCounterSetInfoStruct)
                                            + (uint)m_idToCounter.Count * (uint)sizeof(UnsafeNativeMethods.PerfCounterInfoStruct);
                            uint CounterSetInfoUsed = 0;
                            byte* CounterSetBuffer = stackalloc byte[(int)CounterSetInfoSize];

                            if (CounterSetBuffer == null)
                            {
                                throw new InsufficientMemoryException(SR.Format(SR.Perflib_InsufficientMemory_CounterSetTemplate, m_counterSet, CounterSetInfoSize));
                            }

                            UnsafeNativeMethods.PerfCounterSetInfoStruct* CounterSetInfo;
                            UnsafeNativeMethods.PerfCounterInfoStruct* CounterInfo;

                            uint CurrentCounter = 0;
                            uint CurrentOffset = 0;

                            CounterSetInfo = (UnsafeNativeMethods.PerfCounterSetInfoStruct*)CounterSetBuffer;
                            CounterSetInfo->CounterSetGuid = m_counterSet;
                            CounterSetInfo->ProviderGuid = m_providerGuid;
                            CounterSetInfo->NumCounters = (uint)m_idToCounter.Count;
                            CounterSetInfo->InstanceType = (uint)m_instType;

                            foreach (KeyValuePair<int, CounterType> CounterDef in m_idToCounter)
                            {
                                CounterSetInfoUsed = (uint)sizeof(UnsafeNativeMethods.PerfCounterSetInfoStruct)
                                                + (uint)CurrentCounter * (uint)sizeof(UnsafeNativeMethods.PerfCounterInfoStruct);
                                if (CounterSetInfoUsed < CounterSetInfoSize)
                                {
                                    CounterInfo = (UnsafeNativeMethods.PerfCounterInfoStruct*)(CounterSetBuffer + CounterSetInfoUsed);
                                    CounterInfo->CounterId = (uint)CounterDef.Key;
                                    CounterInfo->CounterType = (uint)CounterDef.Value;
                                    CounterInfo->Attrib = 0x0000000000000001;   // PERF_ATTRIB_BY_REFERENCE
                                    CounterInfo->Size = (uint)sizeof(void*); // always use pointer size
                                    CounterInfo->DetailLevel = 100;                  // PERF_DETAIL_NOVICE
                                    CounterInfo->Scale = 0;                    // Default scale
                                    CounterInfo->Offset = CurrentOffset;

                                    CurrentOffset += CounterInfo->Size;
                                }
                                CurrentCounter++;
                            }
                            Status = UnsafeNativeMethods.PerfSetCounterSetInfo(m_provider.m_hProvider, CounterSetInfo, CounterSetInfoSize);

                            // ERROR_INVALID_PARAMETER, ERROR_ALREADY_EXISTS, ERROR_NOT_ENOUGH_MEMORY, ERROR_OUTOFMEMORY
                            if (Status != (uint)UnsafeNativeMethods.ERROR_SUCCESS)
                            {
                                switch (Status)
                                {
                                    case (uint)UnsafeNativeMethods.ERROR_ALREADY_EXISTS:
                                        throw new ArgumentException(SR.Format(SR.Perflib_Argument_CounterSetAlreadyRegister, m_counterSet), "CounterSetGuid");

                                    default:
                                        throw new Win32Exception((int)Status);
                                }
                            }

                            Interlocked.Increment(ref m_provider.m_counterSet);
                        }

                        m_instanceCreated = true;
                    }
                }
            }

            CounterSetInstance thisInst = new CounterSetInstance(this, instanceName);
            return thisInst;
        }
    }
}
