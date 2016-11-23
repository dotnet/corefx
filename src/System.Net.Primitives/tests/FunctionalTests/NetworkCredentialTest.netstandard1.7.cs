// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using Xunit;

namespace System.Net.Primitives.Functional.Tests
{
    public static partial class NetworkCredentialTest
    {
        public static IEnumerable<object[]> SecurePassword_TestData()
        {
            yield return new object[] { "password", AsSecureString("password") };
            yield return new object[] { "SecurePassword", AsSecureString("SecurePassword") };
            yield return new object[] { "", AsSecureString("") };
            yield return new object[] { null, AsSecureString(null) };
        }

        [Fact]
        public static void Ctor_SecureString_Test()
        {
            string expectedUser = "UserName";

            using (SecureString expectedSecurePassword = AsSecureString("password"))
            {
                NetworkCredential nc = new NetworkCredential(expectedUser, expectedSecurePassword);
                Assert.Equal(expectedUser, nc.UserName);
                Assert.True(expectedSecurePassword.CompareSecureString(nc.SecurePassword));
            }
        }

        [Fact]
        public static void Ctor_SecureStringDomain_Test()
        {
            string expectedUser = "UserName";
            string expectedDomain = "thisDomain";

            using (SecureString expectedSecurePassword = AsSecureString("password"))
            {
                NetworkCredential nc = new NetworkCredential(expectedUser, expectedSecurePassword, expectedDomain);
                Assert.Equal(expectedUser, nc.UserName);
                Assert.Equal(expectedDomain, nc.Domain);
                Assert.True(expectedSecurePassword.CompareSecureString(nc.SecurePassword));
            }
        }

        [Theory]
        [MemberData(nameof(SecurePassword_TestData))]
        public static void SecurePasswordSetGet_Test(string password, SecureString expectedPassword)
        {
            NetworkCredential nc = new NetworkCredential();
            using (SecureString securePassword = AsSecureString(password))
            {
                nc.SecurePassword = securePassword;
                Assert.True(expectedPassword.CompareSecureString(nc.SecurePassword));
            }
        }

        private static bool CompareSecureString(this SecureString s1, SecureString s2)
        {
            if(s1 == null || s2 == null)
                return false;

            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            string str1 = string.Empty;
            string str2 = string.Empty;
            try
            {
                ptr1 = Marshal.SecureStringToGlobalAllocUnicode(s1);
                ptr2 = Marshal.SecureStringToGlobalAllocUnicode(s2);
                str1 = Marshal.PtrToStringUni(ptr1);
                str2 = Marshal.PtrToStringUni(ptr2);
            }
            finally
            {
                if(ptr1 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr1);
                }
                if(ptr2 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr2);
                }
            }
            return str1 == str2;
        }

        private static SecureString AsSecureString(string str)
        {
            SecureString secureString = new SecureString();

            if(str == null)
                return secureString;

            foreach (var ch in str)
            {
                secureString.AppendChar(ch);
            }

            return secureString;
        }
    }
}
