﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Xml
{
    internal class BinHexDecoder : IncrementalReadDecoder
    {
        //
        // Fields
        //
        private byte[] _buffer;
        private int _startIndex;
        private int _curIndex;
        private int _endIndex;
        private bool _hasHalfByteCached;
        private byte _cachedHalfByte;

        //
        // IncrementalReadDecoder interface
        //
        internal override int DecodedCount
        {
            get
            {
                return _curIndex - _startIndex;
            }
        }

        internal override bool IsFull
        {
            get
            {
                return _curIndex == _endIndex;
            }
        }

        internal override unsafe int Decode(char[] chars, int startPos, int len)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }
            if (len < 0)
            {
                throw new ArgumentOutOfRangeException("len");
            }
            if (startPos < 0)
            {
                throw new ArgumentOutOfRangeException("startPos");
            }
            if (chars.Length - startPos < len)
            {
                throw new ArgumentOutOfRangeException("len");
            }

            if (len == 0)
            {
                return 0;
            }
            int bytesDecoded, charsDecoded;
            fixed (char* pChars = &chars[startPos])
            {
                fixed (byte* pBytes = &_buffer[_curIndex])
                {
                    Decode(pChars, pChars + len, pBytes, pBytes + (_endIndex - _curIndex),
                            ref this._hasHalfByteCached, ref this._cachedHalfByte, out charsDecoded, out bytesDecoded);
                }
            }
            _curIndex += bytesDecoded;
            return charsDecoded;
        }

        internal override unsafe int Decode(string str, int startPos, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            if (len < 0)
            {
                throw new ArgumentOutOfRangeException("len");
            }
            if (startPos < 0)
            {
                throw new ArgumentOutOfRangeException("startPos");
            }
            if (str.Length - startPos < len)
            {
                throw new ArgumentOutOfRangeException("len");
            }

            if (len == 0)
            {
                return 0;
            }
            int bytesDecoded, charsDecoded;
            fixed (char* pChars = str)
            {
                fixed (byte* pBytes = &_buffer[_curIndex])
                {
                    Decode(pChars + startPos, pChars + startPos + len, pBytes, pBytes + (_endIndex - _curIndex),
                            ref this._hasHalfByteCached, ref this._cachedHalfByte, out charsDecoded, out bytesDecoded);
                }
            }
            _curIndex += bytesDecoded;
            return charsDecoded;
        }

        internal override void Reset()
        {
            this._hasHalfByteCached = false;
            this._cachedHalfByte = 0;
        }

        internal override void SetNextOutputBuffer(byte[] buffer, int index, int count)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(count >= 0);
            Debug.Assert(index >= 0);
            Debug.Assert(buffer.Length - index >= count);
            Debug.Assert((buffer as byte[]) != null);

            this._buffer = (byte[])buffer;
            this._startIndex = index;
            this._curIndex = index;
            this._endIndex = index + count;
        }

        //
        // Private methods
        //

        private static unsafe void Decode(char* pChars, char* pCharsEndPos,
                                    byte* pBytes, byte* pBytesEndPos,
                                    ref bool hasHalfByteCached, ref byte cachedHalfByte,
                                    out int charsDecoded, out int bytesDecoded)
        {
#if DEBUG
            Debug.Assert(pCharsEndPos - pChars >= 0);
            Debug.Assert(pBytesEndPos - pBytes >= 0);
#endif

            char* pChar = pChars;
            byte* pByte = pBytes;
            XmlCharType xmlCharType = XmlCharType.Instance;
            while (pChar < pCharsEndPos && pByte < pBytesEndPos)
            {
                byte halfByte;
                char ch = *pChar++;

                if (ch >= 'a' && ch <= 'f')
                {
                    halfByte = (byte)(ch - 'a' + 10);
                }
                else if (ch >= 'A' && ch <= 'F')
                {
                    halfByte = (byte)(ch - 'A' + 10);
                }
                else if (ch >= '0' && ch <= '9')
                {
                    halfByte = (byte)(ch - '0');
                }
                else if ((xmlCharType.charProperties[ch] & XmlCharType.fWhitespace) != 0)
                {
                    continue;
                }
                else
                {
                    throw new XmlException(SR.Format(SR.Xml_InvalidBinHexValue, new string(pChars, 0, (int)(pCharsEndPos - pChars))));
                }

                if (hasHalfByteCached)
                {
                    *pByte++ = (byte)((cachedHalfByte << 4) + halfByte);
                    hasHalfByteCached = false;
                }
                else
                {
                    cachedHalfByte = halfByte;
                    hasHalfByteCached = true;
                }
            }

            bytesDecoded = (int)(pByte - pBytes);
            charsDecoded = (int)(pChar - pChars);
        }
    }
}
