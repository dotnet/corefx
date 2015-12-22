// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using System.Globalization;

namespace System.Net
{
    [EventSource(Name = "Microsoft-System-Net-Security",
        Guid = "066c0e27-a02d-5a98-9a4d-078cc3b1a896",
        LocalizationResources = "FxResources.System.Net.Security.SR")]
    internal sealed class SecurityEventSource : EventSource
    {
        private const int ENUMERATE_SECURITY_PACKAGES_ID = 1;

        private static SecurityEventSource s_log = new SecurityEventSource();
        private SecurityEventSource() { }
        public static SecurityEventSource Log
        {
            get
            {
                return s_log;
            }
        }

        [Event(1, Keywords = Keywords.Default,
            Level = EventLevel.Informational)]
        internal void EnumerateSecurityPackages(string securityPackage)
        {
            if (securityPackage != null)
            {
                s_log.WriteEvent(1, securityPackage);
            }
            else
            {
                s_log.WriteEvent(1, "");
            }
        }

        public class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
            public const EventKeywords Debug = (EventKeywords)0x0002;
        }
    }
}
