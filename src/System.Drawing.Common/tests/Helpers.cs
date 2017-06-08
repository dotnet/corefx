using System.IO;
using System.Reflection;

namespace System.Drawing.Tests
{
    public static class Helpers
    {
        public static string GetEmbeddedResourcePath(string name)
        {
            return Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "bitmaps", name);
        }
    }
}
