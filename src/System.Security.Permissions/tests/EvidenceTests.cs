using Xunit;

namespace System.Security.Permissions.Tests
{
    public class EvidenceTests
    {
        [Fact]
        public static void EvidenceCallMethods()
        {
            Policy.Evidence e = new Policy.Evidence();
            e = new Policy.Evidence(new Policy.Evidence());
            e.Clear();
            Policy.Evidence e2 = e.Clone();
            System.Collections.IEnumerator ie = e.GetAssemblyEnumerator();
            ie = e.GetHostEnumerator();
            e.Merge(e2);
        }
    }
}
