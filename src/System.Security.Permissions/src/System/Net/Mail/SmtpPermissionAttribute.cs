using System.Security;
using System.Security.Permissions;

namespace System.Net.Mail
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class |
        AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class SmtpPermissionAttribute : CodeAccessSecurityAttribute
    {
        public SmtpPermissionAttribute(SecurityAction action) : base(action) { }
        public string Access { get { return null; } set { } }
        public override IPermission CreatePermission() { return null; }
    }
}
