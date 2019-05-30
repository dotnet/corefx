// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains two ICryptoTransforms: ToBase64Transform and FromBase64Transform
// they may be attached to a CryptoStream in either read or write mode

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;

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
            ThrowHelper.ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
            if (outputBuffer == null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.outputBuffer);

            // For now, only convert 3 bytes to 4
            Span<byte> input = inputBuffer.AsSpan(inputOffset, InputBlockSize);
            Span<byte> output = outputBuffer.AsSpan(outputOffset, OutputBlockSize);

            OperationStatus status = Base64.EncodeToUtf8(input, output, out int bytesConsumed, out int bytesWritten);

            if (bytesWritten != OutputBlockSize)
                ThrowHelper.ThrowCryptographicException();

            Debug.Assert(status == OperationStatus.Done);
            Debug.Assert(bytesConsumed == InputBlockSize);

            return bytesWritten;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ThrowHelper.ValidateTransformBlock(inputBuffer, inputOffset, inputCount);

            // Convert.ToBase64CharArray already does padding, so all we have to check is that
            // the inputCount wasn't 0
            if (inputCount == 0)
            {
                return Array.Empty<byte>();
            }
            else if (inputCount > InputBlockSize)
            {
                ThrowHelper.ThrowInvalidOffLen();
            }

            // Again, for now only a block at a time
            Span<byte> input = inputBuffer.AsSpan(inputOffset, inputCount);
            byte[] output = new byte[OutputBlockSize];

            OperationStatus status = Base64.EncodeToUtf8(input, output, out int bytesConsumed, out int bytesWritten);

            if (bytesWritten != OutputBlockSize)
                ThrowHelper.ThrowCryptographicException();

            Debug.Assert(status == OperationStatus.Done);
            Debug.Assert(bytesConsumed == inputCount);

            return output;
        }

        public void Clear() => Dispose();

        // Must implement IDisposable, but in this case there's nothing to do.

        public void Dispose()
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
        private byte[] _tmpBuffer;
        private int _inputIndex;
        private readonly FromBase64TransformMode _whitespaces;

        public FromBase64Transform() : this(FromBase64TransformMode.IgnoreWhiteSpaces) { }
        public FromBase64Transform(FromBase64TransformMode whitespaces)
        {
            _whitespaces = whitespaces;
        }

        // Converting from Base64 generates 3 bytes output from each 4 bytes input block
        public int InputBlockSize => 4;
        public int OutputBlockSize => 3;
        public bool CanTransformMultipleBlocks => false;
        public virtual bool CanReuseTransform => true;

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            ThrowHelper.ValidateTransformBlock(inputBuffer, inputOffset, inputCount);
            if (outputBuffer == null) ThrowHelper.ThrowArgumentNull(ThrowHelper.ExceptionArgument.outputBuffer);

            if (_inputBuffer == null)
            {
                ThrowHelper.ThrowObjectDisposed();
            }

            Span<byte> tmpBuffer = GetTempBuffer(inputBuffer.AsSpan(inputOffset, inputCount));

            try
            {
                int bytesToTransform = _inputIndex + tmpBuffer.Length;

                // To less data to decode: save data to _inputBuffer, so it can be transformed later
                if (bytesToTransform < InputBlockSize)
                {
                    tmpBuffer.CopyTo(_inputBuffer.AsSpan(_inputIndex));

                    _inputIndex = bytesToTransform;
                    return 0;
                }

                ConvertFromBase64(tmpBuffer, outputBuffer.AsSpan(outputOffset), out _, out int written);

                return written;
            }
            finally
            {
                ReturnTmpBuffer();
            }
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ThrowHelper.ValidateTransformBlock(inputBuffer, inputOffset, inputCount);

            if (_inputBuffer == null)
            {
                ThrowHelper.ThrowObjectDisposed();
            }

            try
            {
                if (inputCount == 0)
                {
                    return Array.Empty<byte>();
                }

                Span<byte> tmpBuffer = GetTempBuffer(inputBuffer.AsSpan(inputOffset, inputCount));

                int bytesToTransform = _inputIndex + tmpBuffer.Length;

                // To less data to decode
                if (bytesToTransform < InputBlockSize)
                {
                    return Array.Empty<byte>();
                }

                int outputSize = Base64.GetMaxDecodedFromUtf8Length(bytesToTransform);
                byte[] resultBufferArray = null;
                Span<byte> resultBuffer = outputSize <= 256
                    ? stackalloc byte[256]
                    : resultBufferArray = ArrayPool<byte>.Shared.Rent(outputSize);

                ConvertFromBase64(tmpBuffer, resultBuffer, out int consumed, out int written);
                resultBuffer = resultBuffer.Slice(0, written);

                if (resultBufferArray != null)
                {
                    resultBuffer.Clear();
                    ArrayPool<byte>.Shared.Return(resultBufferArray);
                }

                return resultBuffer.ToArray();
            }
            finally
            {
                ReturnTmpBuffer();

                // reinitialize the transform
                Reset();
            }
        }

        private Span<byte> GetTempBuffer(Span<byte> inputBuffer)
        {
            if (_whitespaces == FromBase64TransformMode.DoNotIgnoreWhiteSpaces)
            {
                return inputBuffer;
            }

            // TODO: eliminate
            _tmpBuffer = ArrayPool<byte>.Shared.Rent(inputBuffer.Length);
            return DiscardWhiteSpaces(inputBuffer, _tmpBuffer);
        }

        private Span<byte> DiscardWhiteSpaces(Span<byte> inputBuffer, Span<byte> tmpBuffer)
        {
            int count = 0;

            for (int i = 0; i < inputBuffer.Length; i++)
            {
                if (!char.IsWhiteSpace((char)inputBuffer[i]))
                {
                    tmpBuffer[count++] = inputBuffer[i];
                }
            }

            return tmpBuffer.Slice(0, count);
        }

        private void ConvertFromBase64(Span<byte> tmpBuffer, Span<byte> outputBuffer, out int consumed, out int written)
        {
            int bytesToTransform = _inputIndex + tmpBuffer.Length;

            byte[] transformBufferArray = null;
            Span<byte> transformBuffer = bytesToTransform <= 256
                ? stackalloc byte[256]
                : transformBufferArray = ArrayPool<byte>.Shared.Rent(bytesToTransform);
            transformBuffer = transformBuffer.Slice(0, bytesToTransform);

            // Copy _inputBuffer to transformBuffer and append tmpBuffer
            Debug.Assert(_inputIndex < _inputBuffer.Length);
            _inputBuffer.AsSpan(0, _inputIndex).CopyTo(transformBuffer);
            tmpBuffer.CopyTo(transformBuffer.Slice(_inputIndex));

            // Save data that won't be transformed to _inputBuffer, so it can be transformed later
            _inputIndex = bytesToTransform & 3;     // bit hack for % 4
            Debug.Assert(_inputIndex < _inputBuffer.Length);
            tmpBuffer.Slice(tmpBuffer.Length - _inputIndex).CopyTo(_inputBuffer);

            OperationStatus status = Base64.DecodeFromUtf8(transformBuffer, outputBuffer, out consumed, out written);

            if (status == OperationStatus.InvalidData)
            {
                ThrowHelper.ThrowBase64FormatException();
            }
            Debug.Assert(status == OperationStatus.Done);

            if (transformBufferArray != null)
            {
                transformBuffer.Clear();
                ArrayPool<byte>.Shared.Return(transformBufferArray);
            }
        }

        public void Clear()
        {
            if (_inputBuffer != null)
            {
                _inputBuffer.AsSpan().Clear();
                _inputBuffer = null;
            }

            Reset();
        }

        // Reset the state of the transform so it can be used again
        private void Reset()
        {
            _inputIndex = 0;
        }

        private void ReturnTmpBuffer()
        {
            if (_tmpBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(_tmpBuffer, clearArray: true);
                _tmpBuffer = null;
            }
        }

        // must implement IDisposable, which in this case means clearing the input buffer

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // we always want to clear the input buffer
            if (disposing)
            {
                Clear();
                ReturnTmpBuffer();
            }
        }

        ~FromBase64Transform()
        {
            Dispose(false);
        }
    }

    internal class ThrowHelper
    {
        public static void ValidateTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null) ThrowArgumentNull(ExceptionArgument.inputBuffer);
            if (inputOffset < 0) ThrowArgumentOutOfRangeNeedNonNegNum(ExceptionArgument.inputOffset);
            if ((uint)inputCount > inputBuffer.Length) ThrowArgumentInvalidValue();
            if ((inputBuffer.Length - inputCount) < inputOffset) ThrowInvalidOffLen();
        }

        public static void ThrowArgumentNull(ExceptionArgument argument) => throw new ArgumentNullException(argument.ToString());
        public static void ThrowArgumentOutOfRangeNeedNonNegNum(ExceptionArgument argument) => throw new ArgumentOutOfRangeException(argument.ToString(), SR.ArgumentOutOfRange_NeedNonNegNum);
        public static void ThrowArgumentInvalidValue() => throw new ArgumentException(SR.Argument_InvalidValue);
        public static void ThrowInvalidOffLen() => throw new ArgumentException(SR.Argument_InvalidOffLen);
        public static void ThrowObjectDisposed() => throw new ObjectDisposedException(null, SR.ObjectDisposed_Generic);
        public static void ThrowCryptographicException() => throw new CryptographicException(SR.Cryptography_SSE_InvalidDataSize);
        internal static void ThrowBase64FormatException() => throw new FormatException();

        public enum ExceptionArgument
        {
            inputBuffer,
            outputBuffer,
            inputOffset
        }
    }
}
