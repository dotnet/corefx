// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using Microsoft.Win32;

namespace System.Diagnostics.PerformanceData
{
    /// <summary>
    /// CounterSetInstance class maps to "Instace" in native performance counter implementation.
    /// </summary>    
    public sealed class CounterSetInstance : IDisposable
    {
        internal CounterSet m_counterSet;
        internal String m_instName;
        private Int32 m_active;
        private CounterSetInstanceCounterDataSet m_counters;
        [SecurityCritical]
        unsafe internal UnsafeNativeMethods.PerfCounterSetInstanceStruct* m_nativeInst;

        [System.Security.SecurityCritical]
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        internal CounterSetInstance(CounterSet counterSetDefined, String instanceName)
        {
            if (counterSetDefined == null)
            {
                throw new ArgumentNullException("counterSetDefined");
            }
            if (instanceName == null)
            {
                throw new ArgumentNullException("InstanceName");
            }
            if (instanceName.Length == 0)
            {
                throw new ArgumentException(SR.Perflib_Argument_EmptyInstanceName, "InstanceName");
            }

            m_counterSet = counterSetDefined;
            m_instName = instanceName;
            unsafe
            {
                m_nativeInst = UnsafeNativeMethods.PerfCreateInstance(
                        m_counterSet.m_provider.m_hProvider, ref m_counterSet.m_counterSet, m_instName, 0);
                int Status = (int)((m_nativeInst != null) ? UnsafeNativeMethods.ERROR_SUCCESS : Marshal.GetLastWin32Error());
                if (m_nativeInst != null)
                {
                    m_counters = new CounterSetInstanceCounterDataSet(this);
                }
                else
                {
                    // ERROR_INVALID_PARAMETER,
                    // ERROR_NOT_FOUND (cannot find installed CounterSet),
                    // ERROR_ALREADY_EXISTS,
                    // ERROR_NOT_ENOUGH_MEMORY

                    switch (Status)
                    {
                        case (int)UnsafeNativeMethods.ERROR_ALREADY_EXISTS:
                            throw new ArgumentException(SR.Format(SR.Perflib_Argument_InstanceAlreadyExists, m_instName, m_counterSet.m_counterSet), "InstanceName");

                        case (int)UnsafeNativeMethods.ERROR_NOT_FOUND:
                            throw new InvalidOperationException(SR.Format(SR.Perflib_InvalidOperation_CounterSetNotInstalled, m_counterSet.m_counterSet));

                        case (int)UnsafeNativeMethods.ERROR_INVALID_PARAMETER:
                            if (m_counterSet.m_instType == CounterSetInstanceType.Single)
                            {
                                throw new ArgumentException(SR.Format(SR.Perflib_Argument_InvalidInstance, m_counterSet.m_counterSet), "InstanceName");
                            }
                            else
                            {
                                throw new Win32Exception(Status);
                            }

                        default:
                            throw new Win32Exception(Status);
                    }
                }
            }

            m_active = 1;
        }

        [System.Security.SecurityCritical]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        [System.Security.SecurityCritical]
        ~CounterSetInstance()
        {
            Dispose(false);
        }
        [System.Security.SecurityCritical]
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_counters != null)
                {
                    m_counters.Dispose();
                    m_counters = null;
                }
            }
            unsafe
            {
                if (m_nativeInst != null)
                {
                    if (Interlocked.Exchange(ref m_active, 0) != 0)
                    {
                        if (m_nativeInst != null)
                        {
                            lock (m_counterSet)
                            {
                                if (m_counterSet.m_provider != null)
                                {
                                    uint Status = UnsafeNativeMethods.PerfDeleteInstance(m_counterSet.m_provider.m_hProvider, m_nativeInst);
                                }
                                m_nativeInst = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Access CounterSetInstanceCounterDataSet property. Developers can then use defined indexer to access
        /// specific CounterData object to query/update raw counter data.
        /// </summary>
        public CounterSetInstanceCounterDataSet Counters
        {
            get { return m_counters; }
        }
    }
}
