namespace System.Runtime.InteropServices.ComTypes
{
    public interface IDataObject
    {
        int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);
        void DUnadvise(int connection);
        int EnumDAdvise(out IEnumSTATDATA enumAdvise);
        IEnumFORMATETC EnumFormatEtc(DATADIR direction);
        int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut);
        void GetData(ref FORMATETC format, out STGMEDIUM medium);
        void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium);
        int QueryGetData(ref FORMATETC format);
        void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release);
    }
}
