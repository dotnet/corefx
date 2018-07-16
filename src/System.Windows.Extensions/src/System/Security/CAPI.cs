// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;

namespace System.Security
{
    internal static class CAPI
    {
        internal const String CRYPT32 = "crypt32.dll";
        internal const String CRYPTUI = "cryptui.dll";
        internal const int ERROR_SUCCESS = 0;
        internal const int ERROR_CANCELLED = 1223;
        internal const uint CERT_STORE_PROV_MEMORY = 2;
        internal const uint X509_ASN_ENCODING = 0x00000001;
        internal const uint PKCS_7_ASN_ENCODING = 0x00010000;
        internal const uint CERT_STORE_ENUM_ARCHIVED_FLAG = 0x00000200;
        internal const uint CERT_STORE_CREATE_NEW_FLAG = 0x00002000;
        internal const uint CERT_STORE_ADD_ALWAYS = 4;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class CRYPTUI_VIEWCERTIFICATE_STRUCTW
        {
            internal uint dwSize;
            internal IntPtr hwndParent;                         // OPTIONAL
            internal uint dwFlags;                            // OPTIONAL
            internal string szTitle;                            // OPTIONAL
            internal IntPtr pCertContext;
            internal IntPtr rgszPurposes;                       // OPTIONAL
            internal uint cPurposes;                          // OPTIONAL
            internal IntPtr pCryptProviderData;                 // OPTIONAL
            internal bool fpCryptProviderDataTrustedUsage;    // OPTIONAL
            internal uint idxSigner;                          // OPTIONAL
            internal uint idxCert;                            // OPTIONAL
            internal bool fCounterSigner;                     // OPTIONAL
            internal uint idxCounterSigner;                   // OPTIONAL
            internal uint cStores;                            // OPTIONAL
            internal IntPtr rghStores;                          // OPTIONAL
            internal uint cPropSheetPages;                    // OPTIONAL
            internal IntPtr rgPropSheetPages;                   // OPTIONAL
            internal uint nStartPage;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class CRYPTUI_SELECTCERTIFICATE_STRUCTW
        {
            internal uint dwSize;
            internal IntPtr hwndParent;             // OPTIONAL
            internal uint dwFlags;                // OPTIONAL
            internal string szTitle;                // OPTIONAL
            internal uint dwDontUseColumn;        // OPTIONAL
            internal string szDisplayString;        // OPTIONAL
            internal IntPtr pFilterCallback;        // OPTIONAL
            internal IntPtr pDisplayCallback;       // OPTIONAL
            internal IntPtr pvCallbackData;         // OPTIONAL
            internal uint cDisplayStores;
            internal IntPtr rghDisplayStores;
            internal uint cStores;                // OPTIONAL
            internal IntPtr rghStores;              // OPTIONAL
            internal uint cPropSheetPages;        // OPTIONAL
            internal IntPtr rgPropSheetPages;       // OPTIONAL
            internal IntPtr hSelectedCertStore;     // OPTIONAL
        }

        [DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static
        bool CertAddCertificateLinkToStore(
            [In]     SafeCertStoreHandle hCertStore,
            [In]     SafeCertContextHandle pCertContext,
            [In]     uint dwAddDisposition,
            [In, Out] SafeCertContextHandle ppStoreContext);

        [DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern
        SafeCertContextHandle CertDuplicateCertificateContext(
            [In]     IntPtr pCertContext);

        [DllImport(CRYPT32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern
        IntPtr CertEnumCertificatesInStore(
            [In]     SafeCertStoreHandle hCertStore,
            [In]     IntPtr pPrevCertContext);

        [DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern
        SafeCertStoreHandle CertOpenStore(
            [In]     IntPtr lpszStoreProvider,
            [In]     uint dwMsgAndCertEncodingType,
            [In]     IntPtr hCryptProv,
            [In]     uint dwFlags,
            [In]     string pvPara);

        //[DllImport(CRYPTUI, CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern
        //bool CryptUIDlgViewCertificateW(
        //    [In, MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_VIEWCERTIFICATE_STRUCTW ViewInfo,
        //    [In, Out] IntPtr pfPropertiesChanged);

        //[DllImport(CRYPTUI, CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern
        //SafeCertContextHandle CryptUIDlgSelectCertificateW(
        //    [In, Out, MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_SELECTCERTIFICATE_STRUCTW csc);
    }
}
