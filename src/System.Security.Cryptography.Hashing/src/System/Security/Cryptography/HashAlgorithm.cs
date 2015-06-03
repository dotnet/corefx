// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Diagnostics;

namespace System.Security.Cryptography
{
    public abstract class HashAlgorithm : IDisposable
    {
        protected HashAlgorithm()
        {
        }

        public virtual int HashSize
        {
            get
            {
                return 0;  // For desktop compatibility, return 0 as this property was always initialized by a subclass.
            }
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            HashCore(buffer, 0, buffer.Length);
            return CaptureHashCodeAndReinitialize();
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0 || (count > buffer.Length))
                throw new ArgumentException(SR.Argument_InvalidValue);
            if ((buffer.Length - count) < offset)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (_disposed)
                throw new ObjectDisposedException(null);

            HashCore(buffer, offset, count);
            return CaptureHashCodeAndReinitialize();
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            // Default the buffer size to 4K.
            byte[] buffer = new byte[4096];
            int bytesRead;
            do
            {
                bytesRead = inputStream.Read(buffer, 0, 4096);
                if (bytesRead > 0)
                {
                    HashCore(buffer, 0, bytesRead);
                }
            } while (bytesRead > 0);
            return CaptureHashCodeAndReinitialize();
        }

        private byte[] CaptureHashCodeAndReinitialize()
        {
            byte[] hashValue = HashFinal();
            // Clone the hash value prior to invoking Initialize in case the user-defined Initialize
            // manipulates the array.
            hashValue = hashValue.CloneByteArray();
            Initialize();
            return hashValue;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Although we don't have any resources to dispose at this level,
                // we need to continue to throw ObjectDisposedExceptions from CalculateHash
                // for compatibility with the desktop framework.
                _disposed = true;
            }
            return;
        }

        protected abstract void HashCore(byte[] array, int ibStart, int cbSize);
        protected abstract byte[] HashFinal();
        public abstract void Initialize();

        private bool _disposed;
    }
}

