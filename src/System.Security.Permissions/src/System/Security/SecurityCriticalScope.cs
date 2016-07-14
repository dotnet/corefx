namespace System.Security
{
    [System.ObsoleteAttribute("SecurityCriticalScope is only used for .NET 2.0 transparency compatibility.")]
    public enum SecurityCriticalScope
    {
        Everything = 1,
        Explicit = 0,
    }
}
