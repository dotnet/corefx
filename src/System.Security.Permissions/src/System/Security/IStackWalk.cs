namespace System.Security
{
    public partial interface IStackWalk
    {
        void Assert();
        void Demand();
        void Deny();
        void PermitOnly();
    }
}
