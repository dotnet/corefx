using Xunit;
using System.Diagnostics;

namespace System.EventLog.Tests
{
    public class EventInstanceTests
    {
        [Fact]
        public void Constructor1_test()
        {
             EventInstance ei = null;
             ei = new EventInstance(5, 10);
             Assert.Equal(10, ei.CategoryId);
//             Assert.Equal(5, ei.InstanceId);
//             Assert.Equal(EventLogEntryType.Information, ei.EntryType);


 //            ei = new EventInstance(uint.MaxValue, ushort.MaxValue);
//             Assert.Equal(ushort.MaxValue, ei.CategoryId);
//             Assert.Equal(uint.MaxValue, ei.InstanceId);

        }
    }
}
