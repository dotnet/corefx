﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace System.Security.Policy
{
    public sealed partial class Hash : EvidenceBase, System.Runtime.Serialization.ISerializable
    {
        public Hash(System.Reflection.Assembly assembly) { }
        public byte[] MD5 { get { return null; } }
        public byte[] SHA1 { get { return null; } }
        public byte[] SHA256 { get { return null; } }
        public static Hash CreateMD5(byte[] md5) { return default(Hash); }
        public static Hash CreateSHA1(byte[] sha1) { return default(Hash); }
        public static Hash CreateSHA256(byte[] sha256) { return default(Hash); }
        public byte[] GenerateHash(HashAlgorithm hashAlg) { return null; }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        public override string ToString() => base.ToString();
    }
}
