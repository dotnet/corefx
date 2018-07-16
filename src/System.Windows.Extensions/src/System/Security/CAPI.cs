// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System.Security
{
    internal static class CAPI
    {
        internal const string CRYPT32 = "crypt32.dll";
        internal const string CRYPTUI = "cryptui.dll";
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
            internal IntPtr hwndParent;
            internal uint dwFlags;
            internal string szTitle;
            internal IntPtr pCertContext;
            internal IntPtr rgszPurposes;
            internal uint cPurposes;
            internal IntPtr pCryptProviderData;
            internal bool fpCryptProviderDataTrustedUsage;
            internal uint idxSigner;
            internal uint idxCert;
            internal bool fCounterSigner;
            internal uint idxCounterSigner;
            internal uint cStores;
            internal IntPtr rghStores;
            internal uint cPropSheetPages;
            internal IntPtr rgPropSheetPages;
            internal uint nStartPage;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class CRYPTUI_SELECTCERTIFICATE_STRUCTW
        {
            internal uint dwSize;
            internal IntPtr hwndParent;
            internal uint dwFlags;
            internal string szTitle;
            internal uint dwDontUseColumn;
            internal string szDisplayString;
            internal IntPtr pFilterCallback;
            internal IntPtr pDisplayCallback;
            internal IntPtr pvCallbackData;
            internal uint cDisplayStores;
            internal IntPtr rghDisplayStores;
            internal uint cStores;
            internal IntPtr rghStores;
            internal uint cPropSheetPages;
            internal IntPtr rgPropSheetPages;
            internal IntPtr hSelectedCertStore;
        }

        [DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal extern static bool CertAddCertificateLinkToStore(SafeCertStoreHandle hCertStore, SafeCertContextHandle pCertContext, uint dwAddDisposition, [In, Out] SafeCertContextHandle ppStoreContext);

        [DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeCertContextHandle CertDuplicateCertificateContext(IntPtr pCertContext);

        [DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CertEnumCertificatesInStore(SafeCertStoreHandle hCertStore, IntPtr pPrevCertContext);

        [DllImport(CRYPT32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeCertStoreHandle CertOpenStore(IntPtr lpszStoreProvider, uint dwMsgAndCertEncodingType, IntPtr hCryptProv, uint dwFlags, string pvPara);

        [DllImport(CRYPTUI, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CryptUIDlgViewCertificateW([MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_VIEWCERTIFICATE_STRUCTW ViewInfo, IntPtr pfPropertiesChanged);

        [DllImport(CRYPTUI, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeCertContextHandle CryptUIDlgSelectCertificateW([In, Out, MarshalAs(UnmanagedType.LPStruct)] CRYPTUI_SELECTCERTIFICATE_STRUCTW csc);
    }
}
