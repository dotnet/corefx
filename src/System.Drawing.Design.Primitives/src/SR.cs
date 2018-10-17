using System.Resources;

namespace System
{
    internal static partial class SR
    {
        public static string GetString(string name)
        {
            return GetResourceString(name, null);
        }

        public static string GetString(string name, params object[] args)
        {
            return Format(GetResourceString(name, null), args);
        }

        public static object GetObject(string name)
        {
            object resourceObject = null;
            try { resourceObject = ResourceManager.GetObject(name); }
            catch (MissingManifestResourceException) { }
            return resourceObject;
        }
    }
}
