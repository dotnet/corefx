using System;

namespace System.Runtime.InteropServices
{
    [Flags]
    [ComVisible(true)]
    public enum AssemblyRegistrationFlags
    {
        None                    = 0x00000000,
        SetCodeBase             = 0x00000001,
    }
}
