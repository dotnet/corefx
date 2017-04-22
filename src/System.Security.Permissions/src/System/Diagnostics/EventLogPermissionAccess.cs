namespace System.Diagnostics
{
    [System.Flags]
    public enum EventLogPermissionAccess
    {
        Administer = 48,
        Audit = 10,
        Browse = 2,
        Instrument = 6,
        None = 0,
        Write = 16,
    }
}
