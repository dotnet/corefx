using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct
        | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true, Inherited = false)]
    public class EventLogPermissionAttribute : CodeAccessSecurityAttribute
    {
        public EventLogPermissionAttribute(SecurityAction action) : base(action) { }
        public string MachineName { get { return null; } set { } }
        public EventLogPermissionAccess PermissionAccess { get; set; }
        public override IPermission CreatePermission() { return null; }
    }
}
