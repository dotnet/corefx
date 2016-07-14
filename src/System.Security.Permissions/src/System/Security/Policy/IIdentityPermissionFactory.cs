namespace System.Security.Policy
{
    public partial interface IIdentityPermissionFactory
    {
        System.Security.IPermission CreateIdentityPermission(System.Security.Policy.Evidence evidence);
    }
}
