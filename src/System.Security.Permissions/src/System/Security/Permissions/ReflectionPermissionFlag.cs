namespace System.Security.Permissions
{
    [System.FlagsAttribute]
    public enum ReflectionPermissionFlag
    {
        [System.ObsoleteAttribute]
        AllFlags = 7,
        MemberAccess = 2,
        NoFlags = 0,
        [System.ObsoleteAttribute]
        ReflectionEmit = 4,
        RestrictedMemberAccess = 8,
        [System.ObsoleteAttribute("not used anymore")]
        TypeInformation = 1,
    }
}
