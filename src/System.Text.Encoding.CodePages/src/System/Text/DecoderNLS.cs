// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace System.Text
{
    // A Decoder is used to decode a sequence of blocks of bytes into a
    // sequence of blocks of characters. Following instantiation of a decoder,
    // sequential blocks of bytes are converted into blocks of characters through
    // calls to the GetChars method. The decoder maintains state between the
    // conversions, allowing it to correctly decode byte sequences that span
    // adjacent blocks.
    //
    // Instances of specific implementations of the Decoder abstract base
    // class are typically obtained through calls to the GetDecoder method
    // of Encoding objects.
    //

    internal class DecoderNLS : Decoder
    {
        // Remember our encoding
        protected EncodingNLS m_encoding;
        protected bool m_mustFlush;
        internal bool m_throwOnOverflow;
        internal int m_bytesUsed;
        internal DecoderFallback m_fallback;
        internal DecoderFallbackBuffer m_fallbackBuffer;

        internal DecoderNLS(EncodingNLS encoding)
        {
            m_encoding = encoding;
            m_fallback = m_encoding.DecoderFallback;
            Reset();
        }

        // This is used by our child deserializers
        internal DecoderNLS()
        {
            m_encoding = null;
            Reset();
        }

        internal new DecoderFallback Fallback
        {
            get { return m_fallback; }
        }

        internal bool InternalHasFallbackBuffer
        {
            get
            {
                return m_fallbackBuffer != null;
            }
        }

        public new DecoderFallbackBuffer FallbackBuffer
        {
            get
            {
                if (m_fallbackBuffer == null)
                {
                    if (m_fallback != null)
                        m_fallbackBuffer = m_fallback.CreateFallbackBuffer();
                    else
                        m_fallbackBuffer = DecoderFallback.ReplacementFallback.CreateFallbackBuffer();
                }

                return m_fallbackBuffer;
            }
        }

        public override void Reset()
        {
            if (m_fallbackBuffer != null)
                m_fallbackBuffer.Reset();
        }

        public override unsafe int GetCharCount(byte[] bytes, int index, int count)
        {
            return GetCharCount(bytes, index, count, false);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public override unsafe int GetCharCount(byte[] bytes, int index, int count, bool flush)
        {
            // Validate Parameters
            if (bytes == null)
                throw new ArgumentNullException("bytes", SR.ArgumentNull_Array);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException((index < 0 ? "index" : "count"), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException("bytes", SR.ArgumentOutOfRange_IndexCountBuffer);

            Contract.EndContractBlock();

            // Avoid null fixed problem
            if (bytes.Length == 0)
                bytes = new byte[1];

            // Just call pointer version
            fixed (byte* pBytes = bytes)
                return GetCharCount(pBytes + index, count, flush);
        }

        [System.Security.SecurityCritical]  // auto-generated
        public unsafe int GetCharCount(byte* bytes, int count, bool flush)
        {
            // Validate parameters
            if (bytes == null)
                throw new ArgumentNullException("bytes", SR.ArgumentNull_Array);

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            Contract.EndContractBlock();

            // Remember the flush
            m_mustFlush = flush;
            m_throwOnOverflow = true;

            // By default just call the encoding version, no flush by default
            return m_encoding.GetCharCount(bytes, count, this);
        }

        public override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount,
                                             char[] chars, int charIndex)
        {
            return GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount,
                                             char[] chars, int charIndex, bool flush)
        {
            // Validate Parameters
            if (bytes == null || chars == null)
                throw new ArgumentNullException(bytes == null ? "bytes" : "chars", SR.ArgumentNull_Array);

            if (byteIndex < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException((byteIndex < 0 ? "byteIndex" : "byteCount"), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException("bytes", SR.ArgumentOutOfRange_IndexCountBuffer);

            if ((uint)charIndex > (uint)chars.Length)
                throw new ArgumentOutOfRangeException("charIndex", SR.ArgumentOutOfRange_Index);

            Contract.EndContractBlock();

            // Avoid empty input fixed problem
            if (bytes.Length == 0)
                bytes = new byte[1];

            int charCount = chars.Length - charIndex;
            if (chars.Length == 0)
                chars = new char[1];

            // Just call pointer version
            fixed (byte* pBytes = bytes)
                fixed (char* pChars = chars)
                    // Remember that charCount is # to decode, not size of array
                    return GetChars(pBytes + byteIndex, byteCount,
                                    pChars + charIndex, charCount, flush);
        }

        [System.Security.SecurityCritical]  // auto-generated
        public unsafe int GetChars(byte* bytes, int byteCount,
                                              char* chars, int charCount, bool flush)
        {
            // Validate parameters
            if (chars == null || bytes == null)
                throw new ArgumentNullException((chars == null ? "chars" : "bytes"), SR.ArgumentNull_Array);

            if (byteCount < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException((byteCount < 0 ? "byteCount" : "charCount"), SR.ArgumentOutOfRange_NeedNonNegNum);
            Contract.EndContractBlock();

            // Remember our flush
            m_mustFlush = flush;
            m_throwOnOverflow = true;

            // By default just call the encoding's version
            return m_encoding.GetChars(bytes, byteCount, chars, charCount, this);
        }

        // This method is used when the output buffer might not be big enough.
        // Just call the pointer version.  (This gets chars)
        [System.Security.SecuritySafeCritical]  // auto-generated
        public override unsafe void Convert(byte[] bytes, int byteIndex, int byteCount,
                                              char[] chars, int charIndex, int charCount, bool flush,
                                              out int bytesUsed, out int charsUsed, out bool completed)
        {
            // Validate parameters
            if (bytes == null || chars == null)
                throw new ArgumentNullException((bytes == null ? "bytes" : "chars"), SR.ArgumentNull_Array);

            if (byteIndex < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException((byteIndex < 0 ? "byteIndex" : "byteCount"), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (charIndex < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException((charIndex < 0 ? "charIndex" : "charCount"), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException("bytes", SR.ArgumentOutOfRange_IndexCountBuffer);

            if (chars.Length - charIndex < charCount)
                throw new ArgumentOutOfRangeException("chars", SR.ArgumentOutOfRange_IndexCountBuffer);

            Contract.EndContractBlock();

            // Avoid empty input problem
            if (bytes.Length == 0)
                bytes = new byte[1];
            if (chars.Length == 0)
                chars = new char[1];

            // Just call the pointer version (public overrides can't do this)
            fixed (byte* pBytes = bytes)
            {
                fixed (char* pChars = chars)
                {
                    Convert(pBytes + byteIndex, byteCount, pChars + charIndex, charCount, flush,
                        out bytesUsed, out charsUsed, out completed);
                }
            }
        }

        // This is the version that used pointers.  We call the base encoding worker function
        // after setting our appropriate internal variables.  This is getting chars
        [System.Security.SecurityCritical]  // auto-generated
        public unsafe void Convert(byte* bytes, int byteCount,
                                              char* chars, int charCount, bool flush,
                                              out int bytesUsed, out int charsUsed, out bool completed)
        {
            // Validate input parameters
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? "chars" : "bytes", SR.ArgumentNull_Array);

            if (byteCount < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException((byteCount < 0 ? "byteCount" : "charCount"), SR.ArgumentOutOfRange_NeedNonNegNum);
            Contract.EndContractBlock();

            // We don't want to throw
            m_mustFlush = flush;
            m_throwOnOverflow = false;
            m_bytesUsed = 0;

            // Do conversion
            charsUsed = m_encoding.GetChars(bytes, byteCount, chars, charCount, this);
            bytesUsed = m_bytesUsed;

            // It's completed if they've used what they wanted AND if they didn't want flush or if we are flushed
            completed = (bytesUsed == byteCount) && (!flush || !HasState) &&
                               (m_fallbackBuffer == null || m_fallbackBuffer.Remaining == 0);
            // Our data thingies are now full, we can return
        }

        public bool MustFlush
        {
            get
            {
                return m_mustFlush;
            }
        }

        // Anything left in our decoder?
        internal virtual bool HasState
        {
            get
            {
                return false;
            }
        }

        // Allow encoding to clear our must flush instead of throwing (in ThrowCharsOverflow)
        internal void ClearMustFlush()
        {
            m_mustFlush = false;
        }
    }
}
