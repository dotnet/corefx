namespace System.Security.Permissions
{
    public sealed partial class PublisherIdentityPermission : System.Security.CodeAccessPermission
    {
        public PublisherIdentityPermission(System.Security.Cryptography.X509Certificates.X509Certificate certificate) { }
        public PublisherIdentityPermission(System.Security.Permissions.PermissionState state) { }
        public System.Security.Cryptography.X509Certificates.X509Certificate Certificate { get { return default(System.Security.Cryptography.X509Certificates.X509Certificate); } set { } }
        public override System.Security.IPermission Copy() { return default(System.Security.IPermission); }
        //    public override void FromXml(System.Security.SecurityElement esd) { }
        public override System.Security.IPermission Intersect(System.Security.IPermission target) { return default(System.Security.IPermission); }
        public override bool IsSubsetOf(System.Security.IPermission target) { return default(bool); }
        //    public override System.Security.SecurityElement ToXml() { return default(System.Security.SecurityElement); }
        public override System.Security.IPermission Union(System.Security.IPermission target) { return default(System.Security.IPermission); }
    }
}
