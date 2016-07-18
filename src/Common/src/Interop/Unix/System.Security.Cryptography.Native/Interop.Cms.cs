// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;

using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsDecode")]
        internal static extern SafeCmsHandle CmsDecode(byte[] buf, int len);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsDestroy")]
        internal static extern void CmsDestroy(IntPtr cms);

        [DllImport(Libraries.CryptoNative)]
        private static extern SafeSharedAsn1OctetStringHandle CryptoNative_CmsGetEmbeddedContent(SafeCmsHandle cms);

        internal static SafeSharedAsn1OctetStringHandle CmsGetEmbeddedContent(SafeCmsHandle cms)
        {
            CheckValidOpenSslHandle(cms);

            return SafeInteriorHandle.OpenInteriorHandle(
                (handle) => CryptoNative_CmsGetEmbeddedContent(handle),
                cms);
        }

        [DllImport(Libraries.CryptoNative)]
        private static extern SafeSharedAsn1ObjectHandle CryptoNative_CmsGetEmbeddedContentType(SafeCmsHandle cms);

        internal static SafeSharedAsn1ObjectHandle CmsGetEmbeddedContentType(SafeCmsHandle cms)
        {
            CheckValidOpenSslHandle(cms);

            return SafeInteriorHandle.OpenInteriorHandle(
                (handle) => CryptoNative_CmsGetEmbeddedContentType(handle),
                cms);
        }

        [DllImport(Libraries.CryptoNative)]
        private static extern SafeSharedAsn1ObjectHandle CryptoNative_CmsGetMessageContentType(SafeCmsHandle cms);

        internal static SafeSharedAsn1ObjectHandle CmsGetMessageContentType(SafeCmsHandle cms)
        {
            CheckValidOpenSslHandle(cms);

            return SafeInteriorHandle.OpenInteriorHandle(
                (handle) => CryptoNative_CmsGetMessageContentType(handle),
                cms);
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsGetOriginatorCerts")]
        internal static extern SafeX509StackHandle CmsGetOriginatorCerts(SafeCmsHandle cms);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsGetRecipientStackFieldCount")]
        internal static extern int CmsGetRecipientStackFieldCount(SafeSharedCmsRecipientInfoStackHandle recipientStack);

        [DllImport(Libraries.CryptoNative)]
        private static extern SafeSharedCmsRecipientInfoStackHandle CryptoNative_CmsGetRecipients(SafeCmsHandle cms);

        internal static SafeSharedCmsRecipientInfoStackHandle CmsGetRecipients(SafeCmsHandle cms)
        {
            CheckValidOpenSslHandle(cms);

            return SafeInteriorHandle.OpenInteriorHandle(
                (handle) => CryptoNative_CmsGetRecipients(handle),
                cms);
        }

        [DllImport(Libraries.CryptoNative)]
        private static extern SafeSharedCmsRecipientInfoHandle CryptoNative_CmsGetRecipientStackField(
            SafeSharedCmsRecipientInfoStackHandle recipientStack, int index);

        internal static SafeSharedCmsRecipientInfoHandle CmsGetRecipientStackField(
            SafeSharedCmsRecipientInfoStackHandle recipientStack, int index)
        {
            CheckValidOpenSslHandle(recipientStack);

            return SafeInteriorHandle.OpenInteriorHandle(
                (handle, i) => CryptoNative_CmsGetRecipientStackField(handle, i),
                recipientStack,
                index);
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsDecrypt")]
        internal static extern int CmsDecrypt(
            SafeCmsHandle cms, SafeX509Handle recipientCert, SafeEvpPKeyHandle pKeyKandle, SafeBioHandle decryptedBuffered, SubjectIdentifierType type);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsGetDerSize")]
        internal static extern int CmsGetDerSize(SafeCmsHandle cms);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsEncode")]
        internal static extern int CmsEncode(SafeCmsHandle cms, byte[] buffer);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsInitializeEnvelope")]
        internal static extern SafeCmsHandle CmsInitializeEnvelope(SafeAsn1ObjectHandle algoOid, out int status);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsAddSkidRecipient")]
        internal static extern int CmsAddSkidRecipient(SafeCmsHandle cms, SafeX509Handle skidCert);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsAddIssuerAndSerialRecipient")]
        internal static extern int CmsAddIssuerAndSerialRecipient(SafeCmsHandle cms, SafeX509Handle issuerAndSerialCert);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsCompleteMessage")]
        internal static extern int CmsCompleteMessage(SafeCmsHandle cms, SafeBioHandle data, bool detached);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CmsSetEmbeddedContentType")]
        internal static extern int CmsSetEmbeddedContentType(SafeCmsHandle cms, SafeAsn1ObjectHandle oid);
    }
}