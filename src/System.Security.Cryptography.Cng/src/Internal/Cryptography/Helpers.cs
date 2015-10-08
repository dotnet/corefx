// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

using ErrorCode = Interop.NCrypt.ErrorCode;

namespace Internal.Cryptography
{
    internal static class Helpers
    {
        public static byte[] CloneByteArray(this byte[] src)
        {
            return (byte[])(src.Clone());
        }

        //
        // The C# construct
        //   
        //    fixed (byte* p = new byte[0])
        //
        // sets "p" to 0 rather than a valid address. Sometimes, we actually want a non-NULL pointer instead. (Some CNG apis actually care whether the buffer pointer is
        // NULL or not, even if the accompanying size argument is 0.)
        //
        // This helper enables the syntax:
        //
        //    fixed (byte* p = new byte[0].MapZeroLengthArrayToNonNullPointer())
        //
        // which always sets "p" to a non-NULL pointer for a non-null byte array. 
        //
        public static byte[] MapZeroLengthArrayToNonNullPointer(this byte[] src)
        {
            if (src != null && src.Length == 0)
                return new byte[1];
            return src;
        }

        public static CryptographicException ToCryptographicException(this ErrorCode errorCode)
        {
            return ((int)errorCode).ToCryptographicException();
        }

        public static SafeNCryptProviderHandle OpenStorageProvider(this CngProvider provider)
        {
            string providerName = provider.Provider;
            SafeNCryptProviderHandle providerHandle;
            ErrorCode errorCode = Interop.NCrypt.NCryptOpenStorageProvider(out providerHandle, providerName, 0);
            if (errorCode != ErrorCode.ERROR_SUCCESS)
                throw errorCode.ToCryptographicException();
            return providerHandle;
        }

        /// <summary>
        /// Returns a CNG key property.
        /// </summary>
        /// <returns>
        /// null - if property not defined on key.
        /// throws - for any other type of error.
        /// </returns>
        public static byte[] GetProperty(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
        {
            unsafe
            {
                int numBytesNeeded;
                ErrorCode errorCode = Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, null, 0, out numBytesNeeded, options);
                if (errorCode == ErrorCode.NTE_NOT_FOUND)
                    return null;
                if (errorCode != ErrorCode.ERROR_SUCCESS)
                    throw errorCode.ToCryptographicException();

                byte[] propertyValue = new byte[numBytesNeeded];
                fixed (byte* pPropertyValue = propertyValue)
                {
                    errorCode = Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, pPropertyValue, propertyValue.Length, out numBytesNeeded, options);
                }
                if (errorCode == ErrorCode.NTE_NOT_FOUND)
                    return null;
                if (errorCode != ErrorCode.ERROR_SUCCESS)
                    throw errorCode.ToCryptographicException();

                Array.Resize(ref propertyValue, numBytesNeeded);
                return propertyValue;
            }
        } 

        /// <summary>
        /// Retrieve a well-known CNG string property. (Note: desktop compat: this helper likes to return special values rather than throw exceptions for missing
        /// or ill-formatted property values. Only use it for well-known properties that are unlikely to be ill-formatted.) 
        /// </summary>
        public static string GetPropertyAsString(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
        {
            byte[] value = ncryptHandle.GetProperty(propertyName, options);
            if (value == null)
                return null;   // Desktop compat: return null if key not present.
            if (value.Length == 0)
                return string.Empty; // Desktop compat: return empty if property value is 0-length.
            unsafe
            {
                fixed (byte* pValue = value)
                {
                    string valueAsString = Marshal.PtrToStringUni((IntPtr)pValue);
                    return valueAsString;
                }
            }
        }

        /// <summary>
        /// Retrieve a well-known CNG dword property. (Note: desktop compat: this helper likes to return special values rather than throw exceptions for missing
        /// or ill-formatted property values. Only use it for well-known properties that are unlikely to be ill-formatted.) 
        /// </summary>
        public static int GetPropertyAsDword(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
        {
            byte[] value = ncryptHandle.GetProperty(propertyName, options);
            if (value == null)
                return 0;   // Desktop compat: return 0 if key not present.
            return BitConverter.ToInt32(value, 0);
        }

        /// <summary>
        /// Retrieve a well-known CNG pointer property. (Note: desktop compat: this helper likes to return special values rather than throw exceptions for missing
        /// or ill-formatted property values. Only use it for well-known properties that are unlikely to be ill-formatted.) 
        /// </summary>
        public static IntPtr GetPropertyAsIntPtr(this SafeNCryptHandle ncryptHandle, string propertyName, CngPropertyOptions options)
        {
            unsafe
            {
                int numBytesNeeded;
                IntPtr value;
                ErrorCode errorCode = Interop.NCrypt.NCryptGetProperty(ncryptHandle, propertyName, &value, IntPtr.Size, out numBytesNeeded, options);
                if (errorCode == ErrorCode.NTE_NOT_FOUND)
                    return IntPtr.Zero;
                if (errorCode != ErrorCode.ERROR_SUCCESS)
                    throw errorCode.ToCryptographicException();
                return value;
            }
        }

        /// <summary>
        ///     Modify a CNG key's export policy.
        /// </summary>
        public static void SetExportPolicy(this SafeNCryptKeyHandle keyHandle, CngExportPolicies exportPolicy)
        {
            unsafe
            {
                ErrorCode errorCode = Interop.NCrypt.NCryptSetProperty(keyHandle, KeyPropertyName.ExportPolicy, &exportPolicy, sizeof(CngExportPolicies), CngPropertyOptions.Persist);
                if (errorCode != ErrorCode.ERROR_SUCCESS)
                    throw errorCode.ToCryptographicException();
            }
        }

        public static bool IsLegalSize(this int size, KeySizes[] legalSizes)
        {
            for (int i = 0; i < legalSizes.Length; i++)
            {
                // If a cipher has only one valid key size, MinSize == MaxSize and SkipSize will be 0
                if (legalSizes[i].SkipSize == 0)
                {
                    if (legalSizes[i].MinSize == size)
                        return true;
                }
                else
                {
                    for (int j = legalSizes[i].MinSize; j <= legalSizes[i].MaxSize; j += legalSizes[i].SkipSize)
                    {
                        if (j == size)
                            return true;
                    }
                }
            }
            return false;
        }

        public static int BitSizeToByteSize(this int bits)
        {
            return (bits + 7) / 8;
        }

        public static byte[] GenerateRandom(int count)
        {
            byte[] buffer = new byte[count];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
            return buffer;
        }
    }
}


