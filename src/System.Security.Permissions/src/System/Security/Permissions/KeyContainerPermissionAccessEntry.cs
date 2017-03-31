﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;

namespace System.Security.Permissions
{
    public sealed class KeyContainerPermissionAccessEntry
    {
        public KeyContainerPermissionAccessEntry(string keyContainerName, KeyContainerPermissionFlags flags) { }
        public KeyContainerPermissionAccessEntry(CspParameters parameters, KeyContainerPermissionFlags flags) { }
        public KeyContainerPermissionAccessEntry(string keyStore, string providerName, int providerType,
                        string keyContainerName, int keySpec, KeyContainerPermissionFlags flags)
        { }
        public string KeyStore { get; set; }
        public string ProviderName { get; set; }
        public int ProviderType { get; set; }
        public string KeyContainerName { get; set; }
        public int KeySpec { get; set; }
        public KeyContainerPermissionFlags Flags { get; set; }
        public override bool Equals(object o) { return false; }
        public override int GetHashCode() { return 0; }
    }
}
