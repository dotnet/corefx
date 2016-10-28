// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// CDSCollectionETWBCLProvider.cs
//
// A helper class for firing ETW events related to the Coordination Data Structure
// collection types. This provider is used by CDS collections in both mscorlib.dll
// and System.dll. The purpose of sharing the provider class is to be able to enable
// ETW tracing on all CDS collection with a single ETW provider GUID, and to minimize
// the number of providers in use.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Collections.Generic;
using System.Text;

#if FEATURE_TRACING

using System.Diagnostics.Tracing;

namespace System.Collections.Concurrent
{
    /// <summary>Provides an event source for tracing CDS collection information.</summary>
    [EventSource(
        Name = "System.Collections.Concurrent.ConcurrentCollectionsEventSource",
        Guid = "35167F8E-49B2-4b96-AB86-435B59336B5E"
        // TODO:Bug455853:Add support for reading localized string in the EventSource il2il transform
        //,LocalizationResources = "mscorlib"
        )]
    internal sealed class CDSCollectionETWBCLProvider : EventSource
    {
        /// <summary>
        /// Defines the singleton instance for the collection ETW provider.
        /// The collection provider GUID is {35167F8E-49B2-4b96-AB86-435B59336B5E}.
        /// </summary>
        public static CDSCollectionETWBCLProvider Log = new CDSCollectionETWBCLProvider();

        /// <summary>
        /// Prevent external instantiation.  All logging should go through the Log instance.
        /// </summary>
        private CDSCollectionETWBCLProvider()
        {
        }

        /// <summary>Enabled for all keywords.</summary>
        private const EventKeywords AllKeywords = (EventKeywords)(-1);

        //-----------------------------------------------------------------------------------
        //
        // CDS Collection Event IDs (must be unique)
        //

        private const int ConcurrentstackFastpushfailedId = 1;
        private const int ConcurrentstackFastpopfailedId = 2;
        private const int ConcurrentdictionaryAcquiringalllocksId = 3;
        private const int ConcurrentbagTrytakestealsId = 4;
        private const int ConcurrentbagTrypeekstealsId = 5;

        /////////////////////////////////////////////////////////////////////////////////////
        //
        // ConcurrentStack Events
        //

        [Event(ConcurrentstackFastpushfailedId, Level = EventLevel.Warning)]
        public void ConcurrentStack_FastPushFailed(int spinCount)
        {
            if (IsEnabled(EventLevel.Warning, AllKeywords))
            {
                WriteEvent(ConcurrentstackFastpushfailedId, spinCount);
            }
        }

        [Event(ConcurrentstackFastpopfailedId, Level = EventLevel.Warning)]
        public void ConcurrentStack_FastPopFailed(int spinCount)
        {
            if (IsEnabled(EventLevel.Warning, AllKeywords))
            {
                WriteEvent(ConcurrentstackFastpopfailedId, spinCount);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //
        // ConcurrentDictionary Events
        //

        [Event(ConcurrentdictionaryAcquiringalllocksId, Level = EventLevel.Warning)]
        public void ConcurrentDictionary_AcquiringAllLocks(int numOfBuckets)
        {
            if (IsEnabled(EventLevel.Warning, AllKeywords))
            {
                WriteEvent(ConcurrentdictionaryAcquiringalllocksId, numOfBuckets);
            }
        }

        //
        // Events below this point are used by the CDS types in System.DLL
        //

        /////////////////////////////////////////////////////////////////////////////////////
        //
        // ConcurrentBag Events
        //

        [Event(ConcurrentbagTrytakestealsId, Level = EventLevel.Verbose)]
        public void ConcurrentBag_TryTakeSteals()
        {
            if (IsEnabled(EventLevel.Verbose, AllKeywords))
            {
                WriteEvent(ConcurrentbagTrytakestealsId);
            }
        }

        [Event(ConcurrentbagTrypeekstealsId, Level = EventLevel.Verbose)]
        public void ConcurrentBag_TryPeekSteals()
        {
            if (IsEnabled(EventLevel.Verbose, AllKeywords))
            {
                WriteEvent(ConcurrentbagTrypeekstealsId);
            }
        }
    }
}

#endif // FEATURE_TRACING
