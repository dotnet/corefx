// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography
{
    internal class OpenSslCipher : BasicSymmetricCipher
    {
        private readonly bool _encrypting;
        private SafeEvpCipherCtxHandle _ctx;

        public OpenSslCipher(IntPtr algorithm, CipherMode cipherMode, int blockSizeInBytes, byte[] key, byte[] iv, bool encrypting)
            : base(cipherMode.GetCipherIv(iv), blockSizeInBytes)
        {
            Debug.Assert(algorithm != IntPtr.Zero);

            _encrypting = encrypting;

            OpenKey(algorithm, key);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ctx != null)
                {
                    _ctx.Dispose();
                    _ctx = null;
                }
            }

            base.Dispose(disposing);
        }

        public override unsafe int Transform(byte[] input, int inputOffset, int count, byte[] output, int outputOffset)
        {
            Debug.Assert(input != null);
            Debug.Assert(inputOffset >= 0);
            Debug.Assert(count > 0);
            Debug.Assert((count % BlockSizeInBytes) == 0);
            Debug.Assert(input.Length - inputOffset >= count);
            Debug.Assert(output != null);
            Debug.Assert(outputOffset >= 0);
            Debug.Assert(output.Length - outputOffset >= count);

            return CipherUpdate(input, inputOffset, count, output, outputOffset);
        }

        public override byte[] TransformFinal(byte[] input, int inputOffset, int count)
        {
            Debug.Assert(input != null);
            Debug.Assert(inputOffset >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert((count % BlockSizeInBytes) == 0);
            Debug.Assert(input.Length - inputOffset >= count);

            byte[] output = ProcessFinalBlock(input, inputOffset, count);
            Reset();
            return output;
        }

        private unsafe byte[] ProcessFinalBlock(byte[] input, int inputOffset, int count)
        {
            byte[] output = new byte[count];
            int outputBytes = CipherUpdate(input, inputOffset, count, output, 0);

            fixed (byte* outputStart = output)
            {
                byte* outputCurrent = outputStart + outputBytes;

                int bytesWritten;
                CheckBoolReturn(Interop.Crypto.EvpCipherFinalEx(_ctx, outputCurrent, out bytesWritten));
                outputBytes += bytesWritten;
            }

            if (outputBytes == output.Length)
            {
                return output;
            }

            if (outputBytes == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] userData = new byte[outputBytes];
            Buffer.BlockCopy(output, 0, userData, 0, outputBytes);
            return userData;
        }

        private unsafe int CipherUpdate(byte[] input, int inputOffset, int count, byte[] output, int outputOffset)
        {
            bool status;
            int bytesWritten;

            fixed (byte* inputStart = input)
            fixed (byte* outputStart = output)
            {
                byte* inputCurrent = inputStart + inputOffset;
                byte* outputCurrent = outputStart + outputOffset;

                status = Interop.Crypto.EvpCipherUpdate(
                    _ctx,
                    outputCurrent,
                    out bytesWritten,
                    inputCurrent,
                    count);
            }

            CheckBoolReturn(status);
            return bytesWritten;
        }

        private void OpenKey(IntPtr algorithm, byte[] key)
        {
            _ctx = Interop.Crypto.EvpCipherCreate(
                algorithm,
                key,
                IV,
                _encrypting ? 1 : 0);

            if (_ctx == null)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            // OpenSSL will happily do PKCS#7 padding for us, but since we support padding modes
            // that it doesn't (PaddingMode.Zeros) we'll just always pad the blocks ourselves.
            CheckBoolReturn(Interop.Crypto.EvpCipherCtxSetPadding(_ctx, 0));
        }

        private void Reset()
        {
            bool status = Interop.Crypto.EvpCipherReset(_ctx);

            CheckBoolReturn(status);
        }

        private static void CheckBoolReturn(bool returnValue)
        {
            if (!returnValue)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }
    }
}
