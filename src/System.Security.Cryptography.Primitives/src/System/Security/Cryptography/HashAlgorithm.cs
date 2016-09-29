// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Security.Cryptography
{
    public abstract class HashAlgorithm : IDisposable, ICryptoTransform
    {
        protected HashAlgorithm() { }

        public virtual int HashSize
        {
            get
            {
                return 0;  // For desktop compatibility, return 0 as this property was always initialized by a subclass.
            }
        }

        public virtual byte[] Hash
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(null);
                if (State != 0)
                    throw new CryptographicUnexpectedOperationException(SR.Cryptography_HashNotYetFinalized);

                return (byte[])HashValue.Clone();
            }
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            HashCoreInternal(buffer, 0, buffer.Length);
            return CaptureHashCodeAndReinitialize();
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0 || (count > buffer.Length))
                throw new ArgumentException(SR.Argument_InvalidValue);
            if ((buffer.Length - count) < offset)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (_disposed)
                throw new ObjectDisposedException(null);

            HashCoreInternal(buffer, offset, count);
            return CaptureHashCodeAndReinitialize();
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            if (_disposed)
                throw new ObjectDisposedException(null);

            // Default the buffer size to 4K.
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                HashCoreInternal(buffer, 0, bytesRead);
            }
            return CaptureHashCodeAndReinitialize();
        }

        private void HashCoreInternal(byte[] array, int ibStart, int cbSize)
        {
            // Emulate desktop semantics where HashCore(nonempty) cannot be
            // called after TransformFinalBlock\HashFinal.
            if (_hashFinalCalledWithoutInitialize && cbSize > 0)
                throw new CryptographicException(SR.Cryptography_BadHashState);

            HashCore(array, ibStart, cbSize);
        }

        private byte[] CaptureHashCodeAndReinitialize()
        {
            if (_hashFinalCalledWithoutInitialize)
            {
                // Keep the previous hash value for desktop compat; the previous call to
                // HashCore was for zero bytes.
            }
            else
            {
                HashValue = HashFinal();
            }

            // Clone the hash value prior to invoking Initialize in case the user-defined Initialize
            // manipulates the array.
            byte[] tmp = (byte[])HashValue.Clone();
            Initialize();
            _hashFinalCalledWithoutInitialize = false;
            return tmp;
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

        // ICryptoTransform methods

        // We assume any HashAlgorithm can take input a byte at a time
        public virtual int InputBlockSize
        {
            get { return (1); }
        }

        public virtual int OutputBlockSize
        {
            get { return (1); }
        }

        public virtual bool CanTransformMultipleBlocks
        {
            get { return true; }
        }

        public virtual bool CanReuseTransform
        {
            get { return true; }
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            ValidateTransformBlock(inputBuffer, inputOffset, inputCount);

            // Change the State value
            State = 1;

            HashCoreInternal(inputBuffer, inputOffset, inputCount);
            if ((outputBuffer != null) && ((inputBuffer != outputBuffer) || (inputOffset != outputOffset)))
            {
                // We let BlockCopy do the destination array validation
                Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
            }
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ValidateTransformBlock(inputBuffer, inputOffset, inputCount);

            HashCoreInternal(inputBuffer, inputOffset, inputCount);
            HashValue = HashFinal();
            byte[] outputBytes;
            if (inputCount != 0)
            {
                outputBytes = new byte[inputCount];
                Buffer.BlockCopy(inputBuffer, inputOffset, outputBytes, 0, inputCount);
            }
            else
            {
                outputBytes = Array.Empty<byte>();
            }

            // Reset the State value
            State = 0;

            // For desktop compat, set a flag to enforce that ComputeHash must be called
            // with non-empty value before another HashCore.
            _hashFinalCalledWithoutInitialize = true;

            return outputBytes;
        }

        private void ValidateTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null)
                throw new ArgumentNullException(nameof(inputBuffer));
            if (inputOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(inputOffset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (inputCount < 0 || inputCount > inputBuffer.Length)
                throw new ArgumentException(SR.Argument_InvalidValue);
            if ((inputBuffer.Length - inputCount) < inputOffset)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (_disposed)
                throw new ObjectDisposedException(null);
        }

        protected abstract void HashCore(byte[] array, int ibStart, int cbSize);
        protected abstract byte[] HashFinal();
        public abstract void Initialize();

        private bool _disposed;
        private bool _hashFinalCalledWithoutInitialize;
        protected internal byte[] HashValue;
        protected int State = 0;
    }
}
