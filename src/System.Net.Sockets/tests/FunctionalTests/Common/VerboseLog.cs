using CoreFXTestLibrary;

namespace NCLTest.Sockets
{
    public static class VerboseLog
    {
        public static void Log(string str)
        {
            Log("{0}", str);
        }

        public static void Log(string format, params object[] arg)
        {
#if TRAVE
            Logger.LogInformation(format, arg);
#endif
        }
    }
}
