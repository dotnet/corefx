using System.Security.Permissions;

namespace System.Diagnostics
{
    [Serializable]
    public sealed class EventLogPermission : ResourcePermissionBase
    {
        public EventLogPermission() { }
        public EventLogPermission(EventLogPermissionAccess permissionAccess, string machineName) { }
        public EventLogPermission(EventLogPermissionEntry[] permissionAccessEntries) { }
        public EventLogPermission(PermissionState state) { }
        public EventLogPermissionEntryCollection PermissionEntries { get { throw null; } }
    }
}
