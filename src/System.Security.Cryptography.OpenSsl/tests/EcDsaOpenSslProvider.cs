// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography.EcDsa.Tests
{
    public class ECDsaProvider : IECDsaProvider
    {
        public ECDsa Create()
        {
            return new ECDsaOpenSsl();
        }

        public ECDsa Create(int keySize)
        {
            return new ECDsaOpenSsl(keySize);
        }

        public ECDsa Create(ECCurve curve)
        {
            return new ECDsaOpenSsl(curve);
        }

        public bool IsCurveValid(Oid oid)
        {
            return NativeOidValueExists(oid.Value) ||
                NativeOidFriendlyNameExists(oid.FriendlyName);
        }

        public bool ExplicitCurvesSupported
        {
            get
            {
                return true;
            }
        }

        private static bool NativeOidValueExists(string oidValue)
        {
            if (string.IsNullOrEmpty(oidValue))
                return false;

            IntPtr friendlyNamePtr = IntPtr.Zero;
            int result = Interop.Crypto.LookupFriendlyNameByOid(oidValue, ref friendlyNamePtr);
            return (result == 1);
        }

        private static bool NativeOidFriendlyNameExists(string oidFriendlyName)
        {
            if (string.IsNullOrEmpty(oidFriendlyName))
                return false;

            IntPtr sharedObject = Interop.Crypto.GetObjectDefinitionByName(oidFriendlyName);
            return (sharedObject != IntPtr.Zero);
        }
    }

    public partial class ECDsaFactory
    {
        private static readonly IECDsaProvider s_provider = new ECDsaProvider();
    }
}
