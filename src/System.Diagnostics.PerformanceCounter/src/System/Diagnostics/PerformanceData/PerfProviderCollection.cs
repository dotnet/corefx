﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Diagnostics.PerformanceData
{
    internal sealed class PerfProvider
    {
        internal Guid m_providerGuid;
        internal int m_counterSet;
        [SecurityCritical]
        internal SafePerfProviderHandle m_hProvider;

        [System.Security.SecurityCritical]
        internal PerfProvider(Guid providerGuid)
        {
            m_providerGuid = providerGuid;
            uint Status = UnsafeNativeMethods.PerfStartProvider(ref m_providerGuid, null, out m_hProvider);
            // ERROR_INVALID_PARAMETER, ERROR_OUTOFMEMORY
            if (Status != (uint)UnsafeNativeMethods.ERROR_SUCCESS)
            {
                throw new Win32Exception((int)Status);
            }
        }
    }

    internal static class PerfProviderCollection
    {
        // Internal global PERFLIB V2 provider collection that contains a collection of PerfProvider objects.
        // Use mutex to serialize collection initialization/update.
        private static object s_hiddenInternalSyncObject;
        private static List<PerfProvider> s_providerList = new List<PerfProvider>();
        private static Dictionary<object, int> s_counterSetList = new Dictionary<object, int>();
        private static CounterType[] s_counterTypes = (CounterType[])Enum.GetValues(typeof(CounterType));
        private static CounterSetInstanceType[] s_counterSetInstanceTypes = (CounterSetInstanceType[])Enum.GetValues(typeof(CounterSetInstanceType));

        private static object s_lockObject
        {
            get
            {
                if (s_hiddenInternalSyncObject == null)
                {
                    object o = new object();
                    Interlocked.CompareExchange(ref s_hiddenInternalSyncObject, o, null);
                }
                return s_hiddenInternalSyncObject;
            }
        }

        [System.Security.SecurityCritical]
        internal static PerfProvider QueryProvider(Guid providerGuid)
        {
            // Most of the cases should be that the application contains 1 provider that supports several CounterSets;
            // that is, ContainsKey should succeed except for the first time.            
            lock (s_lockObject)
            {
                foreach (PerfProvider ProviderEntry in s_providerList)
                {
                    if (ProviderEntry.m_providerGuid == providerGuid)
                    {
                        return ProviderEntry;
                    }
                }

                PerfProvider NewProvider = new PerfProvider(providerGuid);
                s_providerList.Add(NewProvider);
                return NewProvider;
            }
        }

        [System.Security.SecurityCritical]
        internal static void RemoveProvider(Guid providerGuid)
        {
            lock (s_lockObject)
            {
                PerfProvider MatchedProvider = null;

                foreach (PerfProvider ProviderEntry in s_providerList)
                {
                    if (ProviderEntry.m_providerGuid == providerGuid)
                    {
                        MatchedProvider = ProviderEntry;
                    }
                }
                if (MatchedProvider != null)
                {
                    MatchedProvider.m_hProvider.Dispose();
                    s_providerList.Remove(MatchedProvider);
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        internal static void RegisterCounterSet(Guid counterSetGuid)
        {
            // Input counterSetGuid should not be registered yet. That is, ContainsKey() should fail most of times.
            lock (s_lockObject)
            {
                if (s_counterSetList.ContainsKey(counterSetGuid))
                {
                    throw new ArgumentException(SR.Format(SR.Perflib_Argument_CounterSetAlreadyRegister, counterSetGuid), "CounterSetGuid");
                }
                s_counterSetList.Add(counterSetGuid, 0);
            }
        }

        internal static void UnregisterCounterSet(Guid counterSetGuid)
        {
            lock (s_lockObject)
            {
                s_counterSetList.Remove(counterSetGuid);
            }
        }

        internal static bool ValidateCounterType(CounterType inCounterType)
        {
            foreach (CounterType DefinedCounterType in s_counterTypes)
            {
                if (DefinedCounterType == inCounterType)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool ValidateCounterSetInstanceType(CounterSetInstanceType inCounterSetInstanceType)
        {
            foreach (CounterSetInstanceType DefinedCounterSetInstanceType in s_counterSetInstanceTypes)
            {
                if (DefinedCounterSetInstanceType == inCounterSetInstanceType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
