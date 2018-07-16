// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography
{
    public enum X509SelectionFlag
    {
        SingleSelection = 0x00,
        MultiSelection = 0x01
    }

    public static partial class X509Certificate2UI
    {
        static X509Certificate2UI() { }
        public static void DisplayCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate) { throw null; }

        public static void DisplayCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, IntPtr hwndParent)
        {
            throw null;
        }

        public static System.Security.Cryptography.X509Certificates.X509Certificate2Collection SelectFromCollection(System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag)
        {
            throw null;
        }

        public static System.Security.Cryptography.X509Certificates.X509Certificate2Collection SelectFromCollection(System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificates, string title, string message, X509SelectionFlag selectionFlag, IntPtr hwndParent)
        {
            throw null;
        }

    }
}
