namespace System.Linq
{
    internal class EmptyAction
    {
        public static readonly Action Instance = Action;

        private static void Action()
        {
        }
    }
}