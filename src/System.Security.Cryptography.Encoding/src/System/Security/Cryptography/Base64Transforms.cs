// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains two ICryptoTransforms: ToBase64Transform and FromBase64Transform
// they may be attached to a CryptoStream in either read or write mode

using System.Buffers;
using System.Text;

namespace System.Security.Cryptography
{
    public enum FromBase64TransformMode
    {
        IgnoreWhiteSpaces = 0,
        DoNotIgnoreWhiteSpaces = 1,
    }

    public class ToBase64Transform : ICryptoTransform
    {
        // converting to Base64 takes 3 bytes input and generates 4 bytes output
        public int InputBlockSize => 3;
        public int OutputBlockSize => 4;
        public bool CanTransformMultipleBlocks => false;
        public virtual bool CanReuseTransform => true;

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            ValidateTransformBlock(inputBuffer, inputOffset, inputCount);

            // For now, only convert 3 bytes to 4
            byte[] tempBytes = ConvertToBase64(inputBuffer, inputOffset, 3);

            Buffer.BlockCopy(tempBytes, 0, outputBuffer, outputOffset, tempBytes.Length);
            return tempBytes.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ValidateTransformBlock(inputBuffer, inputOffset, inputCount);

            // Convert.ToBase64CharArray already does padding, so all we have to check is that
            // the inputCount wasn't 0
            if (inputCount == 0)
            {
                return Array.Empty<byte>();
            }

            // Again, for now only a block at a time
            return ConvertToBase64(inputBuffer, inputOffset, inputCount);
        }

        private byte[] ConvertToBase64(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            char[] temp = new char[4];
            Convert.ToBase64CharArray(inputBuffer, inputOffset, inputCount, temp, 0);
            byte[] tempBytes = Encoding.ASCII.GetBytes(temp);
            if (tempBytes.Length != 4)
                throw new CryptographicException(SR.Cryptography_SSE_InvalidDataSize);

            return tempBytes;
        }

        private static void ValidateTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null) throw new ArgumentNullException(nameof(inputBuffer));
            if (inputOffset < 0) throw new ArgumentOutOfRangeException(nameof(inputOffset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (inputCount < 0 || (inputCount > inputBuffer.Length)) throw new ArgumentException(SR.Argument_InvalidValue);
            if ((inputBuffer.Length - inputCount) < inputOffset) throw new ArgumentException(SR.Argument_InvalidOffLen);
        }

        // Must implement IDisposable, but in this case there's nothing to do.

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }

        ~ToBase64Transform()
        {
            // A finalizer is not necessary here, however since we shipped a finalizer that called
            // Dispose(false) in desktop v2.0, we need to keep it in case any existing code had subclassed
            // this transform and expects to have a base class finalizer call its dispose method.
            Dispose(false);
        }
    }

    public class FromBase64Transform : ICryptoTransform
    {
        private byte[] _inputBuffer = new byte[4];
        private int _inputIndex;
        private FromBase64TransformMode _whitespaces;

        public FromBase64Transform() : this(FromBase64TransformMode.IgnoreWhiteSpaces) { }
        public FromBase64Transform(FromBase64TransformMode whitespaces)
        {
            _whitespaces = whitespaces;
            _inputIndex = 0;
        }

        // Converting from Base64 generates 3 bytes output from each 4 bytes input block
        public int InputBlockSize => 1;
        public int OutputBlockSize => 3;
        public bool CanTransformMultipleBlocks => false;
        public virtual bool CanReuseTransform => true;

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
            if (_inputBuffer == null) throw new ObjectDisposedException(null, SR.ObjectDisposed_Generic);

            var (temp, tempCount) = GetTempBuffer(inputBuffer, inputOffset, inputCount);

            if (temp == null)
            {
                return 0;
            }

            byte[] result;
            try
            {
                result = Convert.FromBase64CharArray(temp, 0, tempCount);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(temp, true);
            }

            Buffer.BlockCopy(result, 0, outputBuffer, outputOffset, result.Length);

            return result.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
            if (_inputBuffer == null) throw new ObjectDisposedException(null, SR.ObjectDisposed_Generic);

            var (temp, tempCount) = GetTempBuffer(inputBuffer, inputOffset, inputCount);

            if (temp == null)
            {
                Reset();
                return Array.Empty<byte>();
            }

            byte[] result;
            try
            {
                result = Convert.FromBase64CharArray(temp, 0, tempCount);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(temp, true);
            }

            // reinitialize the transform
            Reset();

            return result;
        }

        private void FillRemainderBuffer(byte[] buffer, int offset, int count)
        {
            if (_whitespaces == FromBase64TransformMode.IgnoreWhiteSpaces)
            {
                for (var i = 0; i < count; i++)
                {
                    var value = buffer[offset + i];
                    if (!char.IsWhiteSpace((char)value))
                    {
                        _inputBuffer[_inputIndex++] = value;
                    }
                }
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, _inputBuffer, _inputIndex, count);
                _inputIndex += count;
            }
        }

        private (char[], int) GetTempBuffer(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var effectiveCount = inputCount;
            if (_whitespaces == FromBase64TransformMode.IgnoreWhiteSpaces)
            {
                for (var i = 0; i < inputCount; i++)
                {
                    if (char.IsWhiteSpace((char)inputBuffer[inputOffset + i])) effectiveCount--;
                }
            }

            // are there insufficient characters to decode a block?
            if (effectiveCount + _inputIndex < 4)
            {
                FillRemainderBuffer(inputBuffer, inputOffset, inputCount);
                return (null, 0);
            }

            // copy current remainder + input -> temp
            var totalBytes = _inputIndex + effectiveCount;
            var remainder = totalBytes % 4;
            var tempCount = _inputIndex + effectiveCount - remainder;
            var temp = ArrayPool<char>.Shared.Rent(tempCount);
            var tempIndex = 0;
            var inputIndex = inputOffset;
            for (var i = 0; i < _inputIndex; i++)
            {
                temp[tempIndex++] = (char)_inputBuffer[i];
            }
            _inputIndex = 0;
            if (_whitespaces == FromBase64TransformMode.IgnoreWhiteSpaces)
            {
                while (tempIndex < tempCount)
                {
                    var value = (char)inputBuffer[inputIndex++];
                    if (!char.IsWhiteSpace(value))
                    {
                        temp[tempIndex++] = value;
                    }
                }
            }
            else
            {
                var usableCount = inputCount - remainder;
                Encoding.ASCII.GetChars(inputBuffer, inputOffset, usableCount, temp, tempIndex);
                inputIndex += usableCount;
            }

            FillRemainderBuffer(inputBuffer, inputIndex, inputOffset + inputCount - inputIndex);

            return (temp, tempCount);
        }

        private static void ValidateTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null) throw new ArgumentNullException(nameof(inputBuffer));
            if (inputOffset < 0) throw new ArgumentOutOfRangeException(nameof(inputOffset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (inputCount < 0 || (inputCount > inputBuffer.Length)) throw new ArgumentException(SR.Argument_InvalidValue);
            if ((inputBuffer.Length - inputCount) < inputOffset) throw new ArgumentException(SR.Argument_InvalidOffLen);
        }

        // must implement IDisposable, which in this case means clearing the input buffer

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Reset the state of the transform so it can be used again
        private void Reset()
        {
            _inputIndex = 0;
        }

        public void Clear()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            // we always want to clear the input buffer
            if (disposing)
            {
                if (_inputBuffer != null)
                    Array.Clear(_inputBuffer, 0, _inputBuffer.Length);
                _inputBuffer = null;
                _inputIndex = 0;
            }
        }

        ~FromBase64Transform()
        {
            Dispose(false);
        }
    }
}
