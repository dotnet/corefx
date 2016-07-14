namespace System.Security.Permissions
{
    [System.FlagsAttribute]
    public enum FileDialogPermissionAccess
    {
        None = 0,
        Open = 1,
        OpenSave = 3,
        Save = 2,
    }
}
