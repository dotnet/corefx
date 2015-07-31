namespace System.Net
{
    internal class Socket
    {
        public static bool LegacySupportsIPv6 { get { return true; } }

        public static bool OSSupportsIPv6 { get; internal set; }

        internal static void InitializeSockets()
        {
            throw new NotImplementedException();
        }
    }
}