namespace System.Net
{
    internal class DnsResolveAsyncResult : ContextAwareResult
    {
        // Forward lookup
        internal DnsResolveAsyncResult(string hostName, object myObject, bool includeIPv6, object myState, AsyncCallback myCallBack) :
            base(myObject, myState, myCallBack)
        {
            this.hostName = hostName;
            this.includeIPv6 = includeIPv6;
        }

        // Reverse lookup
        internal DnsResolveAsyncResult(IPAddress address, object myObject, bool includeIPv6, object myState, AsyncCallback myCallBack) :
            base(myObject, myState, myCallBack)
        {
            this.includeIPv6 = includeIPv6;
            this.address = address;
        }

        internal readonly string hostName;
        internal bool includeIPv6;
        internal IPAddress address;
    }
}