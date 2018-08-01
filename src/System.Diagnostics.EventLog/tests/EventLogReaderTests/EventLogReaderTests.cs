// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using Xunit;
using System.Diagnostics.Eventing.Reader;

namespace System.Diagnostics.Tests
{
    public class EventLogReaderTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void CreateReader()
        {
            // var eventLog = new EventLogReader("XXX");
        }

  
    }
}

