using Microsoft.Diagnostics.Tracing.Session;
using System;
using Xunit;

namespace BasicEventSourceTests
{
    public partial class FuzzyTests
    {
        /// <summary>
        /// Tests the EventSource.Write[T] method (can only use the self-describing mechanism) for Windows platform.
        /// </summary>
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsWindows))]
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
