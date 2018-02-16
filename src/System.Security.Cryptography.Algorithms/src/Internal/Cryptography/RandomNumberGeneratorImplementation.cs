// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
    internal sealed partial class RandomNumberGeneratorImplementation : RandomNumberGenerator
    {
        // As long as each implementation can provide a static GetBytes(ref byte buf, int length)
        // they can share this one implementation of FillSpan.
        internal static void FillSpan(Span<byte> data)
        {
            if (data.Length > 0)
            {
                GetBytes(ref MemoryMarshal.GetReference(data), data.Length);
            }
        }

        public override void GetBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            GetBytes(new Span<byte>(data));
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            VerifyGetBytes(data, offset, count);
            GetBytes(new Span<byte>(data, offset, count));
        }

        public override void GetBytes(Span<byte> data)
        {
            if (data.Length > 0)
            {
                GetBytes(ref MemoryMarshal.GetReference(data), data.Length);
            }
        }

        public override void GetNonZeroBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            GetNonZeroBytes(new Span<byte>(data));
        }

        public override void GetNonZeroBytes(Span<byte> data)
        {
            while (data.Length > 0)
            {
                // Fill the remaining portion of the span with random bytes.
                GetBytes(data);

                // Find the first zero in the remaining portion.
                int indexOfFirst0Byte = data.Length;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0)
                    {
                        indexOfFirst0Byte = i;
                        break;
                    }
                }

                // If there were any zeros, shift down all non-zeros.
                for (int i = indexOfFirst0Byte + 1; i < data.Length; i++)
                {
                    if (data[i] != 0)
                    {
                        data[indexOfFirst0Byte++] = data[i];
                    }
                }

                // Request new random bytes if necessary; dont re-use
                // existing bytes since they were shifted down.
                data = data.Slice(indexOfFirst0Byte);
            }
        }
    }
}
