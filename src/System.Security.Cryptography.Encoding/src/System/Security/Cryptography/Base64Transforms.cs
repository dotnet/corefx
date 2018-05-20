// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains two ICryptoTransforms: ToBase64Transform and FromBase64Transform
// they may be attached to a CryptoStream in either read or write mode

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
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
        private const int BLOCK_SIZE = 4;
        private byte[] _inputBuffer = new byte[BLOCK_SIZE];
        private int _inputIndex;
        private FromBase64TransformMode _whitespaces;
        private bool _finished = false;

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
            ReadOnlySpan<byte> input = ValidateInputBlock(inputBuffer, inputOffset, inputCount);
            if (_inputBuffer == null) throw new ObjectDisposedException(null, SR.ObjectDisposed_Generic);
            Span<byte> output = ValidateOutputBlock(outputBuffer, outputOffset);
            int outputWritten = 0;

            if (_whitespaces == FromBase64TransformMode.DoNotIgnoreWhiteSpaces)
            {
                int totalInput = _inputIndex + input.Length;
                int remainder = totalInput % BLOCK_SIZE;
                int usableInput = input.Length - remainder;

                if (_inputIndex > 0)
                {
                    int copy = Math.Min(input.Length, _inputBuffer.Length - _inputIndex);
                    input.Slice(0, copy).CopyTo(_inputBuffer.AsSpan(_inputIndex));
                    _inputIndex += copy;
                    if (_inputIndex < _inputBuffer.Length)
                    {
                        // not enough bytes to form a single block
                        return outputWritten;
                    }
                    _inputIndex = 0;
                    
                    DecodeBlocks(_inputBuffer, ref output, ref outputWritten);

                    input = input.Slice(copy);
                    usableInput -= copy;
                }

                ReadOnlySpan<byte> newRemainder = input.Slice(usableInput);
                input = input.Slice(0, usableInput);

                DecodeBlocks(input, ref output, ref outputWritten);
                
                newRemainder.CopyTo(_inputBuffer);
                _inputIndex = newRemainder.Length;

                return outputWritten;
            }
            else
            {
                int inputPos = 0;
                while (inputPos < input.Length)
                {
                    while (_inputIndex < _inputBuffer.Length && inputPos < input.Length)
                    {
                        if (!char.IsWhiteSpace((char)input[inputPos]))
                        {
                            _inputBuffer[_inputIndex++] = input[inputPos];
                        }
                        inputPos++;
                    }
                    if (_inputIndex == _inputBuffer.Length)
                    {
                        _inputIndex = 0;
                        DecodeBlocks(_inputBuffer, ref output, ref outputWritten);
                    }
                }

                return outputWritten;
            }
        }

        void DecodeBlocks(ReadOnlySpan<byte> input, ref Span<byte> output, ref int totalBytesWritten)
        {
            if (input.Length == 0)
            {
                return;
            }
            if (_finished)
            {
                throw new FormatException("zzz invalid data");
            }
            Debug.Assert(input.Length % BLOCK_SIZE == 0, nameof(input) + ".Length % " + nameof(BLOCK_SIZE) + " != 0");
            bool final = input[input.Length - 1] == (byte)'=';
            OperationStatus status = Base64.DecodeFromUtf8(input, output, out int bytesConsumed, out int bytesWritten, final);
            switch (status)
            {
                case OperationStatus.DestinationTooSmall:
                    throw new ArgumentException("dst", "zzz output buffer too small");
                case OperationStatus.InvalidData:
                    throw new FormatException("zzz invalid data");
            }
            Debug.Assert(bytesConsumed == input.Length, nameof(bytesConsumed) + " != " + nameof(input) + ".Length");
            output = output.Slice(bytesWritten);
            totalBytesWritten += bytesWritten;
            _finished = final;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ReadOnlySpan<byte> input = ValidateInputBlock(inputBuffer, inputOffset, inputCount);
            if (_inputBuffer == null) throw new ObjectDisposedException(null, SR.ObjectDisposed_Generic);

            if (_inputIndex == 0 && input.Length == 0)
            {
                Reset();
                return Array.Empty<byte>();
            }

            int outputSize;
            int omittedPadding = 0;
            if (_whitespaces == FromBase64TransformMode.DoNotIgnoreWhiteSpaces)
            {
                int totalCount = _inputIndex + input.Length;

                omittedPadding = BLOCK_SIZE - 1 - (totalCount + BLOCK_SIZE - 1) % BLOCK_SIZE;
                if (omittedPadding > 2)
                {
                    throw new FormatException($"zzz invalid data");
                }

                // round up to the nearest whole block
                outputSize = Base64.GetMaxDecodedFromUtf8Length(totalCount + omittedPadding);
                // check the last two characters for padding or make up missing padding
                if (omittedPadding > 0 ||
                    ((input.Length > 0 && input[input.Length - 1] == (byte)'=') ||
                    (input.Length == 0 && _inputBuffer[BLOCK_SIZE - 1] == (byte)'=')))
                {
                    outputSize--;
                    if (omittedPadding > 1 ||
                        ((input.Length > 1 && input[input.Length - 2] == (byte)'=') ||
                        (input.Length == 1 && _inputBuffer[BLOCK_SIZE - 1] == (byte)'=') ||
                        (input.Length == 0 && _inputBuffer[BLOCK_SIZE - 2] == (byte)'=')))
                    {
                        outputSize--;
                    }
                }
            }
            else
            {
                int nonSpace = 0;
                int padding = 0;
                for (int i = 0; i < _inputIndex; i++)
                {
                    if (!char.IsWhiteSpace((char)_inputBuffer[i]))
                    {
                        nonSpace++;
                        if (_inputBuffer[i] == (byte)'=')
                        {
                            padding++;
                        }
                        else if (padding > 0)
                        {
                            throw new FormatException("zzz invalid data");
                        }
                    }
                }
                for (int i = 0; i < input.Length; i++)
                {
                    if (!char.IsWhiteSpace((char)input[i]))
                    {
                        nonSpace++;
                        if (input[i] == (byte)'=')
                        {
                            padding++;
                        }
                        else if (padding > 0)
                        {
                            throw new FormatException("zzz invalid data");
                        }
                    }
                }

                omittedPadding = BLOCK_SIZE - 1 - (nonSpace + BLOCK_SIZE - 1) % BLOCK_SIZE;
                if (padding + omittedPadding > 2)
                {
                    throw new FormatException($"zzz invalid data");
                }

                outputSize = Base64.GetMaxDecodedFromUtf8Length(nonSpace + omittedPadding) - padding - omittedPadding;
            }

            var result = new byte[outputSize];

            int written = TransformBlock(inputBuffer, inputOffset, inputCount, result, 0);
            Debug.Assert((_inputIndex + omittedPadding) % BLOCK_SIZE == 0, "(" + nameof(_inputIndex) + " + " + nameof(omittedPadding) + ") % BLOCK_SIZE != 0");

            if (omittedPadding > 0)
            {
                for (int i = 0; i < omittedPadding; i++)
                {
                    _inputBuffer[BLOCK_SIZE - 1 - i] = (byte)'=';
                }
                Span<byte> output = result.AsSpan(written);
                int paddingWritten = 0;
                DecodeBlocks(_inputBuffer, ref output, ref paddingWritten);
            }

            Reset();

            return result;
        }

        private static ReadOnlySpan<byte> ValidateInputBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null) throw new ArgumentNullException(nameof(inputBuffer));
            if (inputOffset < 0) throw new ArgumentOutOfRangeException(nameof(inputOffset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (inputCount < 0 || (inputCount > inputBuffer.Length)) throw new ArgumentException(SR.Argument_InvalidValue);
            if ((inputBuffer.Length - inputCount) < inputOffset) throw new ArgumentException(SR.Argument_InvalidOffLen);
            return inputBuffer.AsSpan(inputOffset, inputCount);
        }

        private static Span<byte> ValidateOutputBlock(byte[] outputBuffer, int outputOffset)
        {
            if (outputBuffer == null) throw new ArgumentNullException("dst");
            if (outputOffset < 0) throw new ArgumentOutOfRangeException(nameof(outputOffset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (outputBuffer.Length < outputOffset) throw new ArgumentException(SR.Argument_InvalidOffLen);
            return outputBuffer.AsSpan(outputOffset);
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
            _finished = false;
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
