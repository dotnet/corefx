namespace System.Reflection.Internal
{
    internal static class Hash
    {
        internal static int Combine(int newKey, int currentKey)
        {
            return unchecked((currentKey * (int)0xA5555529) + newKey);
        }

        internal static int Combine(uint newKey, int currentKey)
        {
            return unchecked((currentKey * (int)0xA5555529) + (int)newKey);
        }

        internal static int Combine(bool newKeyPart, int currentKey)
        {
            return Combine(currentKey, newKeyPart ? 1 : 0);
        }
    }
}
