namespace System.Net
{
    internal class SocketProtocolSupportPal
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

       public static bool OSSupportsIPv4 {
            get
            {
                // TODO: Depending on the dependency graph, either:
                //       A. Move within NameResolution eventually duplicating code
                //       B. Call the Socket equivalent
                return true;
            }
        }

        private static void EnsureInitialized()
        {
            throw NotImplementedException();
        }
    }
}