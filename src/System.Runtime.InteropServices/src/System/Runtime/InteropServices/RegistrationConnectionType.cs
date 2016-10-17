namespace System.Runtime.InteropServices
{
    [Flags]
    public enum RegistrationConnectionType
    {
        SingleUse                = 0, 
        MultipleUse              = 1, 
        MultiSeparate            = 2, 
        Suspended                = 4, 
        Surrogate                = 8, 
    }
}
