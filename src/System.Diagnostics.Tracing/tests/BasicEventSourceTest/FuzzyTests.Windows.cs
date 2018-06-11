// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace BasicEventSourceTests
{
    public partial class FuzzyTests
    {
        /// <summary>
        /// Tests the EventSource.Write[T] method (can only use the self-describing mechanism) for Windows platform.
        /// </summary>
        [Fact]
        public void Test_Write_Fuzzy_IsWindows()
        {
            TestFixture((logger, tests) =>
            {
                if (TestUtilities.IsProcessElevated)
                {
                    using (var listener = new EtwListener())
                    {
                        EventTestHarness.RunTests(tests, listener, logger);
                    }
                }

                using (var listener = new EventListenerListener())
                {
                    EventTestHarness.RunTests(tests, listener, logger);
                }
            });
        }
    }
}
