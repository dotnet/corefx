// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using Internal.Cryptography;

using ErrorCode = Interop.NCrypt.ErrorCode;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Managed representation of an NCrypt key
    /// </summary>
    public sealed partial class CngKey : IDisposable
    {
        //
        // Import factory methods
        //
        internal static CngKey Import(ReadOnlySpan<byte> keyBlob, CngKeyBlobFormat format)
        {
            return Import(keyBlob, null, format, CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        public static CngKey Import(byte[] keyBlob, CngKeyBlobFormat format)
        {
            return Import(keyBlob, format, provider: CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        internal static CngKey Import(byte[] keyBlob, string curveName, CngKeyBlobFormat format)
        {
            return Import(keyBlob, curveName, format, provider: CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        public static CngKey Import(byte[] keyBlob, CngKeyBlobFormat format, CngProvider provider)
        {
            return Import(keyBlob, null, format, provider);
        }

        internal static CngKey ImportEncryptedPkcs8(
            ReadOnlySpan<byte> keyBlob,
            ReadOnlySpan<char> password)
        {
            return ImportEncryptedPkcs8(keyBlob, password, CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        internal static unsafe CngKey ImportEncryptedPkcs8(
            ReadOnlySpan<byte> keyBlob,
            ReadOnlySpan<char> password,
            CngProvider provider)
        {
            SafeNCryptProviderHandle providerHandle = provider.OpenStorageProvider();
            SafeNCryptKeyHandle keyHandle;

            using (SafeUnicodeStringHandle passwordHandle = new SafeUnicodeStringHandle(password))
            {
                Interop.NCrypt.NCryptBuffer* buffers = stackalloc Interop.NCrypt.NCryptBuffer[1];

                buffers[0] = new Interop.NCrypt.NCryptBuffer
                {
                    BufferType = Interop.NCrypt.BufferType.PkcsSecret,
                    cbBuffer = checked(2 * (password.Length + 1)),
                    pvBuffer = passwordHandle.DangerousGetHandle(),
                };

                if (buffers[0].pvBuffer == IntPtr.Zero)
                {
                    buffers[0].cbBuffer = 0;
                }

                Interop.NCrypt.NCryptBufferDesc desc = new Interop.NCrypt.NCryptBufferDesc
                {
                    cBuffers = 1,
                    pBuffers = (IntPtr)buffers,
                    ulVersion = 0,
                };

                ErrorCode errorCode = Interop.NCrypt.NCryptImportKey(
                    providerHandle,
                    IntPtr.Zero,
                    Interop.NCrypt.NCRYPT_PKCS8_PRIVATE_KEY_BLOB,
                    ref desc,
                    out keyHandle,
                    ref MemoryMarshal.GetReference(keyBlob),
                    keyBlob.Length,
                    0);

                if (errorCode != ErrorCode.ERROR_SUCCESS)
                {
                    keyHandle.Dispose();
                    throw errorCode.ToCryptographicException();
                }
            }

            CngKey key = new CngKey(providerHandle, keyHandle);
            key.IsEphemeral = true;
            return key;
        }

        internal static CngKey Import(
            byte[] keyBlob,
            string curveName,
            CngKeyBlobFormat format,
            CngProvider provider)
        {
            if (keyBlob == null)
                throw new ArgumentNullException(nameof(keyBlob));

            return Import(keyBlob.AsSpan(), curveName, format, provider);
        }

        internal static CngKey Import(
            ReadOnlySpan<byte> keyBlob,
            string curveName,
            CngKeyBlobFormat format,
            CngProvider provider)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            SafeNCryptProviderHandle providerHandle = provider.OpenStorageProvider();
            SafeNCryptKeyHandle keyHandle = null;
            ErrorCode errorCode;
            
            if (curveName == null)
            {
                errorCode = Interop.NCrypt.NCryptImportKey(
                    providerHandle,
                    IntPtr.Zero,
                    format.Format,
                    IntPtr.Zero,
                    out keyHandle,
                    ref MemoryMarshal.GetReference(keyBlob),
                    keyBlob.Length,
                    0);

                if (errorCode != ErrorCode.ERROR_SUCCESS)
                {
                    throw errorCode.ToCryptographicException();
                }
            }
            else
            {
                keyHandle = ECCng.ImportKeyBlob(format.Format, keyBlob, curveName, providerHandle);
            }

            CngKey key = new CngKey(providerHandle, keyHandle);

            // We can't tell directly if an OpaqueTransport blob imported as an ephemeral key or not
            key.IsEphemeral = format != CngKeyBlobFormat.OpaqueTransportBlob;

            return key;
        }
    }
}
