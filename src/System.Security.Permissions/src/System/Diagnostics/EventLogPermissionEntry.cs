namespace System.Diagnostics
{
    [Serializable]
    public class EventLogPermissionEntry
    {
        public EventLogPermissionEntry(EventLogPermissionAccess permissionAccess, string machineName) { }
        public string MachineName { get { return null; } }
        public EventLogPermissionAccess PermissionAccess { get; }
    }
}
