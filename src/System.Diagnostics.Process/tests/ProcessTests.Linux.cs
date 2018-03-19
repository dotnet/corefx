using Xunit;

namespace System.Diagnostics.Tests
{
    partial class ProcessTests
    {
        [Fact]
        public void TestProcessStartTime_Deterministic_Across_Instances()
        {
            CreateDefaultProcess();
            var p = Process.GetProcessById(_process.Id);
            Assert.Equal(_process.StartTime, p.StartTime);
        }
    }
}
