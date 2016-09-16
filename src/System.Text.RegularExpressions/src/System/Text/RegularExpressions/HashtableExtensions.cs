using System.Collections;

namespace System.Text.RegularExpressions
{
    internal static class HashtableExtensions
    {
        public static bool TryGetValue<T>(this Hashtable table, object key, out T value)
        {
            if (table.ContainsKey(key))
            {
                value = (T)table[key];
                return true;
            }
            value = default(T);
            return false;
        }
    }
}