// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.IO;

namespace System.Security.Cryptography
{
    public class CryptoStream : Stream, IDisposable
    {
        // Member veriables
        private readonly Stream _stream;
        private readonly ICryptoTransform _transform;
        private readonly CryptoStreamMode _transformMode;
        private byte[] _inputBuffer;  // read from _stream before _Transform
        private int _inputBufferIndex;
        private int _inputBlockSize;
        private byte[] _outputBuffer; // buffered output of _Transform
        private int _outputBufferIndex;
        private int _outputBlockSize;
        private bool _canRead;
        private bool _canWrite;
        private bool _finalBlockTransformed;

        // Constructors

        public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
        {
            _stream = stream;
            _transformMode = mode;
            _transform = transform;
            switch (_transformMode)
            {
                case CryptoStreamMode.Read:
                    if (!(_stream.CanRead)) throw new ArgumentException(SR.Format(SR.Argument_StreamNotReadable, "stream"));
                    _canRead = true;
                    break;
                case CryptoStreamMode.Write:
                    if (!(_stream.CanWrite)) throw new ArgumentException(SR.Format(SR.Argument_StreamNotWritable, "stream"));
                    _canWrite = true;
                    break;
                default:
                    throw new ArgumentException(SR.Argument_InvalidValue);
            }
            InitializeBuffer();
        }

        public override bool CanRead
        {
            [Pure]
            get { return _canRead; }
        }

        // For now, assume we can never seek into the middle of a cryptostream
        // and get the state right.  This is too strict.
        public override bool CanSeek
        {
            [Pure]
            get { return false; }
        }

        public override bool CanWrite
        {
            [Pure]
            get { return _canWrite; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(SR.NotSupported_UnseekableStream); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(SR.NotSupported_UnseekableStream); }
            set { throw new NotSupportedException(SR.NotSupported_UnseekableStream); }
        }

        public bool HasFlushedFinalBlock
        {
            get { return _finalBlockTransformed; }
        }

        // The flush final block functionality used to be part of close, but that meant you couldn't do something like this:
        // MemoryStream ms = new MemoryStream();
        // CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        // cs.Write(foo, 0, foo.Length);
        // cs.Close();
        // and get the encrypted data out of ms, because the cs.Close also closed ms and the data went away.
        // so now do this:
        // cs.Write(foo, 0, foo.Length);
        // cs.FlushFinalBlock() // which can only be called once
        // byte[] ciphertext = ms.ToArray();
        // cs.Close();
        public void FlushFinalBlock()
        {
            if (_finalBlockTransformed)
                throw new NotSupportedException(SR.Cryptography_CryptoStream_FlushFinalBlockTwice);
            // We have to process the last block here.  First, we have the final block in _InputBuffer, so transform it

            byte[] finalBytes = _transform.TransformFinalBlock(_inputBuffer, 0, _inputBufferIndex);

            _finalBlockTransformed = true;
            // Now, write out anything sitting in the _OutputBuffer...
            if (_canWrite && _outputBufferIndex > 0)
            {
                _stream.Write(_outputBuffer, 0, _outputBufferIndex);
                _outputBufferIndex = 0;
            }
            // Write out finalBytes
            if (_canWrite)
                _stream.Write(finalBytes, 0, finalBytes.Length);

            // If the inner stream is a CryptoStream, then we want to call FlushFinalBlock on it too, otherwise just Flush.
            CryptoStream innerCryptoStream = _stream as CryptoStream;
            if (innerCryptoStream != null)
            {
                if (!innerCryptoStream.HasFlushedFinalBlock)
                {
                    innerCryptoStream.FlushFinalBlock();
                }
            }
            else
            {
                _stream.Flush();
            }
            // zeroize plain text material before returning
            if (_inputBuffer != null)
                Array.Clear(_inputBuffer, 0, _inputBuffer.Length);
            if (_outputBuffer != null)
                Array.Clear(_outputBuffer, 0, _outputBuffer.Length);
            return;
        }

        public override void Flush()
        {
            return;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // argument checking
            if (!CanRead)
                throw new NotSupportedException(SR.NotSupported_UnreadableStream);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            Contract.EndContractBlock();
            // read <= count bytes from the input stream, transforming as we go.
            // Basic idea: first we deliver any bytes we already have in the
            // _OutputBuffer, because we know they're good.  Then, if asked to deliver 
            // more bytes, we read & transform a block at a time until either there are
            // no bytes ready or we've delivered enough.
            int bytesToDeliver = count;
            int currentOutputIndex = offset;
            if (_outputBufferIndex != 0)
            {
                // we have some already-transformed bytes in the output buffer
                if (_outputBufferIndex <= count)
                {
                    Buffer.BlockCopy(_outputBuffer, 0, buffer, offset, _outputBufferIndex);
                    bytesToDeliver -= _outputBufferIndex;
                    currentOutputIndex += _outputBufferIndex;
                    _outputBufferIndex = 0;
                }
                else
                {
                    Buffer.BlockCopy(_outputBuffer, 0, buffer, offset, count);
                    Buffer.BlockCopy(_outputBuffer, count, _outputBuffer, 0, _outputBufferIndex - count);
                    _outputBufferIndex -= count;
                    return (count);
                }
            }
            // _finalBlockTransformed == true implies we're at the end of the input stream
            // if we got through the previous if block then _OutputBufferIndex = 0, meaning
            // we have no more transformed bytes to give
            // so return count-bytesToDeliver, the amount we were able to hand back
            // eventually, we'll just always return 0 here because there's no more to read
            if (_finalBlockTransformed)
            {
                return (count - bytesToDeliver);
            }
            // ok, now loop until we've delivered enough or there's nothing available
            int amountRead = 0;
            int numOutputBytes;

            // OK, see first if it's a multi-block transform and we can speed up things
            if (bytesToDeliver > _outputBlockSize)
            {
                if (_transform.CanTransformMultipleBlocks)
                {
                    int BlocksToProcess = bytesToDeliver / _outputBlockSize;
                    int numWholeBlocksInBytes = BlocksToProcess * _inputBlockSize;
                    byte[] tempInputBuffer = new byte[numWholeBlocksInBytes];
                    // get first the block already read
                    Buffer.BlockCopy(_inputBuffer, 0, tempInputBuffer, 0, _inputBufferIndex);
                    amountRead = _inputBufferIndex;
                    amountRead += _stream.Read(tempInputBuffer, _inputBufferIndex, numWholeBlocksInBytes - _inputBufferIndex);
                    _inputBufferIndex = 0;
                    if (amountRead <= _inputBlockSize)
                    {
                        _inputBuffer = tempInputBuffer;
                        _inputBufferIndex = amountRead;
                        goto slow;
                    }
                    // Make amountRead an integral multiple of _InputBlockSize
                    int numWholeReadBlocksInBytes = (amountRead / _inputBlockSize) * _inputBlockSize;
                    int numIgnoredBytes = amountRead - numWholeReadBlocksInBytes;
                    if (numIgnoredBytes != 0)
                    {
                        _inputBufferIndex = numIgnoredBytes;
                        Buffer.BlockCopy(tempInputBuffer, numWholeReadBlocksInBytes, _inputBuffer, 0, numIgnoredBytes);
                    }
                    byte[] tempOutputBuffer = new byte[(numWholeReadBlocksInBytes / _inputBlockSize) * _outputBlockSize];
                    numOutputBytes = _transform.TransformBlock(tempInputBuffer, 0, numWholeReadBlocksInBytes, tempOutputBuffer, 0);
                    Buffer.BlockCopy(tempOutputBuffer, 0, buffer, currentOutputIndex, numOutputBytes);
                    // Now, tempInputBuffer and tempOutputBuffer are no more needed, so zeroize them to protect plain text
                    Array.Clear(tempInputBuffer, 0, tempInputBuffer.Length);
                    Array.Clear(tempOutputBuffer, 0, tempOutputBuffer.Length);
                    bytesToDeliver -= numOutputBytes;
                    currentOutputIndex += numOutputBytes;
                }
            }

        slow:
            // try to fill _InputBuffer so we have something to transform
            while (bytesToDeliver > 0)
            {
                while (_inputBufferIndex < _inputBlockSize)
                {
                    amountRead = _stream.Read(_inputBuffer, _inputBufferIndex, _inputBlockSize - _inputBufferIndex);
                    // first, check to see if we're at the end of the input stream
                    if (amountRead == 0) goto ProcessFinalBlock;
                    _inputBufferIndex += amountRead;
                }
                numOutputBytes = _transform.TransformBlock(_inputBuffer, 0, _inputBlockSize, _outputBuffer, 0);
                _inputBufferIndex = 0;
                if (bytesToDeliver >= numOutputBytes)
                {
                    Buffer.BlockCopy(_outputBuffer, 0, buffer, currentOutputIndex, numOutputBytes);
                    currentOutputIndex += numOutputBytes;
                    bytesToDeliver -= numOutputBytes;
                }
                else
                {
                    Buffer.BlockCopy(_outputBuffer, 0, buffer, currentOutputIndex, bytesToDeliver);
                    _outputBufferIndex = numOutputBytes - bytesToDeliver;
                    Buffer.BlockCopy(_outputBuffer, bytesToDeliver, _outputBuffer, 0, _outputBufferIndex);
                    return count;
                }
            }
            return count;

        ProcessFinalBlock:
            // if so, then call TransformFinalBlock to get whatever is left
            byte[] finalBytes = _transform.TransformFinalBlock(_inputBuffer, 0, _inputBufferIndex);
            // now, since _OutputBufferIndex must be 0 if we're in the while loop at this point,
            // reset it to be what we just got back
            _outputBuffer = finalBytes;
            _outputBufferIndex = finalBytes.Length;
            // set the fact that we've transformed the final block
            _finalBlockTransformed = true;
            // now, return either everything we just got or just what's asked for, whichever is smaller
            if (bytesToDeliver < _outputBufferIndex)
            {
                Buffer.BlockCopy(_outputBuffer, 0, buffer, currentOutputIndex, bytesToDeliver);
                _outputBufferIndex -= bytesToDeliver;
                Buffer.BlockCopy(_outputBuffer, bytesToDeliver, _outputBuffer, 0, _outputBufferIndex);
                return (count);
            }
            else
            {
                Buffer.BlockCopy(_outputBuffer, 0, buffer, currentOutputIndex, _outputBufferIndex);
                bytesToDeliver -= _outputBufferIndex;
                _outputBufferIndex = 0;
                return (count - bytesToDeliver);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            Contract.EndContractBlock();
            // write <= count bytes to the output stream, transforming as we go.
            // Basic idea: using bytes in the _InputBuffer first, make whole blocks,
            // transform them, and write them out.  Cache any remaining bytes in the _InputBuffer.
            int bytesToWrite = count;
            int currentInputIndex = offset;
            // if we have some bytes in the _InputBuffer, we have to deal with those first,
            // so let's try to make an entire block out of it
            if (_inputBufferIndex > 0)
            {
                if (count >= _inputBlockSize - _inputBufferIndex)
                {
                    // we have enough to transform at least a block, so fill the input block
                    Buffer.BlockCopy(buffer, offset, _inputBuffer, _inputBufferIndex, _inputBlockSize - _inputBufferIndex);
                    currentInputIndex += (_inputBlockSize - _inputBufferIndex);
                    bytesToWrite -= (_inputBlockSize - _inputBufferIndex);
                    _inputBufferIndex = _inputBlockSize;
                    // Transform the block and write it out
                }
                else
                {
                    // not enough to transform a block, so just copy the bytes into the _InputBuffer
                    // and return
                    Buffer.BlockCopy(buffer, offset, _inputBuffer, _inputBufferIndex, count);
                    _inputBufferIndex += count;
                    return;
                }
            }
            // If the OutputBuffer has anything in it, write it out
            if (_outputBufferIndex > 0)
            {
                _stream.Write(_outputBuffer, 0, _outputBufferIndex);
                _outputBufferIndex = 0;
            }
            // At this point, either the _InputBuffer is full, empty, or we've already returned.
            // If full, let's process it -- we now know the _OutputBuffer is empty
            int numOutputBytes;
            if (_inputBufferIndex == _inputBlockSize)
            {
                numOutputBytes = _transform.TransformBlock(_inputBuffer, 0, _inputBlockSize, _outputBuffer, 0);
                // write out the bytes we just got 
                _stream.Write(_outputBuffer, 0, numOutputBytes);
                // reset the _InputBuffer
                _inputBufferIndex = 0;
            }
            while (bytesToWrite > 0)
            {
                if (bytesToWrite >= _inputBlockSize)
                {
                    // We have at least an entire block's worth to transform
                    // If the transform will handle multiple blocks at once, do that
                    if (_transform.CanTransformMultipleBlocks)
                    {
                        int numWholeBlocks = bytesToWrite / _inputBlockSize;
                        int numWholeBlocksInBytes = numWholeBlocks * _inputBlockSize;
                        byte[] _tempOutputBuffer = new byte[numWholeBlocks * _outputBlockSize];
                        numOutputBytes = _transform.TransformBlock(buffer, currentInputIndex, numWholeBlocksInBytes, _tempOutputBuffer, 0);
                        _stream.Write(_tempOutputBuffer, 0, numOutputBytes);
                        currentInputIndex += numWholeBlocksInBytes;
                        bytesToWrite -= numWholeBlocksInBytes;
                    }
                    else
                    {
                        // do it the slow way
                        numOutputBytes = _transform.TransformBlock(buffer, currentInputIndex, _inputBlockSize, _outputBuffer, 0);
                        _stream.Write(_outputBuffer, 0, numOutputBytes);
                        currentInputIndex += _inputBlockSize;
                        bytesToWrite -= _inputBlockSize;
                    }
                }
                else
                {
                    // In this case, we don't have an entire block's worth left, so store it up in the 
                    // input buffer, which by now must be empty.
                    Buffer.BlockCopy(buffer, currentInputIndex, _inputBuffer, 0, bytesToWrite);
                    _inputBufferIndex += bytesToWrite;
                    return;
                }
            }
            return;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (!_finalBlockTransformed)
                    {
                        FlushFinalBlock();
                    }
                    _stream.Dispose();
                }
            }
            finally
            {
                try
                {
                    // Ensure we don't try to transform the final block again if we get disposed twice
                    // since it's null after this
                    _finalBlockTransformed = true;
                    // we need to clear all the internal buffers
                    if (_inputBuffer != null)
                        Array.Clear(_inputBuffer, 0, _inputBuffer.Length);
                    if (_outputBuffer != null)
                        Array.Clear(_outputBuffer, 0, _outputBuffer.Length);

                    _inputBuffer = null;
                    _outputBuffer = null;
                    _canRead = false;
                    _canWrite = false;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        // Private methods 

        private void InitializeBuffer()
        {
            if (_transform != null)
            {
                _inputBlockSize = _transform.InputBlockSize;
                _inputBuffer = new byte[_inputBlockSize];
                _outputBlockSize = _transform.OutputBlockSize;
                _outputBuffer = new byte[_outputBlockSize];
            }
        }
    }
}
