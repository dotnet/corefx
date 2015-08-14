
namespace System.Net.Sockets.Tests
{
    // Each individual test must configure this class by defining s_implementationType within 
    // SocketTestServer.DefaultFactoryConfiguration.cs
    public abstract partial class SocketTestServer : IDisposable
    {
        protected const SocketImplementationType s_implementationType = SocketImplementationType.APM;
    }
}
