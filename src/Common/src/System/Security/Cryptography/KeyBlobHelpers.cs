// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography
{
    internal static class KeyBlobHelpers
    {
        internal static byte[] ExportKeyParameter(this BigInteger value, int length)
        {
            byte[] target = new byte[length];

            if (value.TryWriteBytes(target, out int bytesWritten, isUnsigned: true, isBigEndian: true))
            {
                if (bytesWritten < length)
                {
                    Buffer.BlockCopy(target, 0, target, length - bytesWritten, bytesWritten);
                    target.AsSpan(0, length - bytesWritten).Clear();
                }

                return target;
            }

            throw new CryptographicException(SR.Cryptography_NotValidPublicOrPrivateKey);
        }

        internal static void WriteKeyParameterInteger(this AsnWriter writer, ReadOnlySpan<byte> integer)
        {
            Debug.Assert(!integer.IsEmpty);

            if (integer[0] == 0)
            {
                int newStart = 1;

                while (newStart < integer.Length)
                {
                    if (integer[newStart] >= 0x80)
                    {
                        newStart--;
                        break;
                    }

                    if (integer[newStart] != 0)
                    {
                        break;
                    }

                    newStart++;
                }

                if (newStart == integer.Length)
                {
                    newStart--;
                }

                integer = integer.Slice(newStart);
            }

            writer.WriteIntegerUnsigned(integer);
        }
        
        internal static byte[] TrimPaddingByte(byte[] data)
        {
            byte[] dataLocal = data;
            TrimPaddingByte(ref dataLocal);
            return dataLocal;
        }

        internal static void TrimPaddingByte(ref byte[] data)
        {
            if (data[0] != 0)
                return;

            byte[] newData = new byte[data.Length - 1];
            Buffer.BlockCopy(data, 1, newData, 0, newData.Length);
            data = newData;
        }

        internal static byte[] PadOrTrim(byte[] data, int length)
        {
            byte[] dataLocal = data;
            PadOrTrim(ref dataLocal, length);
            return dataLocal;
        }

        internal static void PadOrTrim(ref byte[] data, int length)
        {
            if (data.Length == length)
                return;

            // Need to skip the sign-padding byte.
            if (data.Length == length + 1 && data[0] == 0)
            {
                TrimPaddingByte(ref data);
                return;
            }

            int offset = length - data.Length;

            byte[] newData = new byte[length];
            Buffer.BlockCopy(data, 0, newData, offset, data.Length);
            data = newData;
        }
    }
}
