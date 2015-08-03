namespace System.Net
{
    internal class Socket
    {
       public static bool OSSupportsIPv6 {
            get
            {
                // TODO: Depending on the dependency graph, either:
                //       A. Move within NameResolution eventually duplicating code
                //       B. Call the Socket equivalent
                return true;
            }
        }

        internal static void InitializeSockets()
        {
            // TODO: Depending on the dependency graph, either:
            //       A. Move the initialization within NameResolution and ensure that a DNS object is created prior to
            //          any socket usage
            //       B. Create a Socket object prior to using DNS.
            throw new NotImplementedException();
        }
    }
}