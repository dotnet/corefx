namespace System.Linq.ChainLinq.Links
{
    sealed partial class Identity<T> : Link<T, T>
    {
        public static Link<T, T> Instance { get; } = new Identity<T>();

        public Identity() : base(LinkType.Identity) { }

        public override Chain<T, ChainEnd> Compose(Chain<T, ChainEnd> next) => next;
    }
}
