namespace System.Runtime.InteropServices.ComTypes
{
    public interface IEnumSTATDATA
    {
        void Clone(out IEnumSTATDATA newEnum);
        int Next(int celt, STATDATA[] rgelt, int[] pceltFetched);
        int Reset();
        int Skip(int celt);
    }
}
