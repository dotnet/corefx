// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.even

using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogPropertySelectorTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Ctor_NullPropertyQueries_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EventLogPropertySelector(null));
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Ctor_Null_Throws()
        {
            IDictionary<string, string> dictionary = new SortedDictionary<string, string>() { ["key"] = "value" };
            var selector = new EventLogPropertySelector(dictionary.Keys);
            Assert.NotNull(selector);
            selector.Dispose();
        }
    }
}
