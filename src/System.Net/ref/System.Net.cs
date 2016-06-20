namespace System.Net
{
    public partial class IPEndPointCollection : System.Collections.ObjectModel.Collection<System.Net.IPEndPoint>
    {
        public IPEndPointCollection() { }
        protected override void InsertItem(int index, System.Net.IPEndPoint item) { }
        protected override void SetItem(int index, System.Net.IPEndPoint item) { }
    }
}
