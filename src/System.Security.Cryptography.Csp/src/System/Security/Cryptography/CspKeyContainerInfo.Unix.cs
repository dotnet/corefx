// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
    public sealed class CspKeyContainerInfo
    {
        public CspKeyContainerInfo(CspParameters parameters) { throw new PlatformNotSupportedException(); }
        public bool Accessible { get { throw new PlatformNotSupportedException(); } }
        public bool Exportable { get { throw new PlatformNotSupportedException(); } }
        public bool HardwareDevice { get { throw new PlatformNotSupportedException(); } }
        public string KeyContainerName { get { throw new PlatformNotSupportedException(); } }
        public KeyNumber KeyNumber { get { throw new PlatformNotSupportedException(); } }
        public bool MachineKeyStore { get { throw new PlatformNotSupportedException(); } }
        public bool Protected { get { throw new PlatformNotSupportedException(); } }
        public string ProviderName { get { throw new PlatformNotSupportedException(); } }
        public int ProviderType { get { throw new PlatformNotSupportedException(); } }
        public bool RandomlyGenerated { get { throw new PlatformNotSupportedException(); } }
        public bool Removable { get { throw new PlatformNotSupportedException(); } }
        public string UniqueKeyContainerName { get { throw new PlatformNotSupportedException(); } }
    }
}
