// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// This is used internally to create best fit behavior as per the original windows best fit behavior.
//

using System.Diagnostics;
using System.Threading;

namespace System.Text
{
    internal sealed class InternalDecoderBestFitFallback : DecoderFallback
    {
        // Our variables
        internal Encoding _encoding;
        internal char[]? _arrayBestFit = null;
        internal char _cReplacement = '?';

        internal InternalDecoderBestFitFallback(Encoding encoding)
        {
            // Need to load our replacement characters table.
            _encoding = encoding;
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new InternalDecoderBestFitFallbackBuffer(this);
        }

        // Maximum number of characters that this instance of this fallback could return
        public override int MaxCharCount
        {
            get
            {
                return 1;
            }
        }

        public override bool Equals(object? value)
        {
            if (value is InternalDecoderBestFitFallback that)
            {
                return _encoding.CodePage == that._encoding.CodePage;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _encoding.CodePage;
        }
    }

    internal sealed class InternalDecoderBestFitFallbackBuffer : DecoderFallbackBuffer
    {
        // Our variables
        private char _cBestFit = '\0';
        private int _iCount = -1;
        private int _iSize;
        private InternalDecoderBestFitFallback _oFallback;

        // Private object for locking instead of locking on a public type for SQL reliability work.
        private static object? s_InternalSyncObject;
        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object o = new object();
                    Interlocked.CompareExchange<object?>(ref s_InternalSyncObject, o, null);
                }
                return s_InternalSyncObject;
            }
        }

        // Constructor
        public InternalDecoderBestFitFallbackBuffer(InternalDecoderBestFitFallback fallback)
        {
            _oFallback = fallback;

            if (_oFallback._arrayBestFit == null)
            {
                // Lock so we don't confuse ourselves.
                lock (InternalSyncObject)
                {
                    // Double check before we do it again.
                    if (_oFallback._arrayBestFit == null)
                        _oFallback._arrayBestFit = fallback._encoding.GetBestFitBytesToUnicodeData();
                }
            }
        }

        // Fallback methods
        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            // We expect no previous fallback in our buffer
            Debug.Assert(_iCount < 1, "[DecoderReplacementFallbackBuffer.Fallback] Calling fallback without a previously empty buffer");

            _cBestFit = TryBestFit(bytesUnknown);
            if (_cBestFit == '\0')
                _cBestFit = _oFallback._cReplacement;

            _iCount = _iSize = 1;

            return true;
        }

        // Default version is overridden in DecoderReplacementFallback.cs
        public override char GetNextChar()
        {
            // We want it to get < 0 because == 0 means that the current/last character is a fallback
            // and we need to detect recursion.  We could have a flag but we already have this counter.
            _iCount--;

            // Do we have anything left? 0 is now last fallback char, negative is nothing left
            if (_iCount < 0)
                return '\0';

            // Need to get it out of the buffer.
            // Make sure it didn't wrap from the fast count-- path
            if (_iCount == int.MaxValue)
            {
                _iCount = -1;
                return '\0';
            }

            // Return the best fit character
            return _cBestFit;
        }

        public override bool MovePrevious()
        {
            // Exception fallback doesn't have anywhere to back up to.
            if (_iCount >= 0)
                _iCount++;

            // Return true if we could do it.
            return (_iCount >= 0 && _iCount <= _iSize);
        }

        // How many characters left to output?
        public override int Remaining
        {
            get
            {
                return (_iCount > 0) ? _iCount : 0;
            }
        }

        // Clear the buffer
        public override unsafe void Reset()
        {
            _iCount = -1;
            byteStart = null;
        }

        // This version just counts the fallback and doesn't actually copy anything.
        internal unsafe override int InternalFallback(byte[] bytes, byte* pBytes)
        // Right now this has both bytes and bytes[], since we might have extra bytes, hence the
        // array, and we might need the index, hence the byte*
        {
            // return our replacement string Length (always 1 for InternalDecoderBestFitFallback, either
            // a best fit char or ?
            return 1;
        }

        // private helper methods
        private char TryBestFit(byte[] bytesCheck)
        {
            // Need to figure out our best fit character, low is beginning of array, high is 1 AFTER end of array
            int lowBound = 0;
            Debug.Assert(_oFallback._arrayBestFit != null);
            int highBound = _oFallback._arrayBestFit.Length;
            int index;
            char cCheck;

            // Check trivial case first (no best fit)
            if (highBound == 0)
                return '\0';

            // If our array is too small or too big we can't check
            if (bytesCheck.Length == 0 || bytesCheck.Length > 2)
                return '\0';

            if (bytesCheck.Length == 1)
                cCheck = unchecked((char)bytesCheck[0]);
            else
                cCheck = unchecked((char)((bytesCheck[0] << 8) + bytesCheck[1]));

            // Check trivial out of range case
            if (cCheck < _oFallback._arrayBestFit[0] || cCheck > _oFallback._arrayBestFit[highBound - 2])
                return '\0';

            // Binary search the array
            int iDiff;
            while ((iDiff = (highBound - lowBound)) > 6)
            {
                // Look in the middle, which is complicated by the fact that we have 2 #s for each pair,
                // so we don't want index to be odd because it must be word aligned.
                // Also note that index can never == highBound (because diff is rounded down)
                index = ((iDiff / 2) + lowBound) & 0xFFFE;

                char cTest = _oFallback._arrayBestFit[index];
                if (cTest == cCheck)
                {
                    // We found it
                    Debug.Assert(index + 1 < _oFallback._arrayBestFit.Length,
                        "[InternalDecoderBestFitFallbackBuffer.TryBestFit]Expected replacement character at end of array");
                    return _oFallback._arrayBestFit[index + 1];
                }
                else if (cTest < cCheck)
                {
                    // We weren't high enough
                    lowBound = index;
                }
                else
                {
                    // We weren't low enough
                    highBound = index;
                }
            }

            for (index = lowBound; index < highBound; index += 2)
            {
                if (_oFallback._arrayBestFit[index] == cCheck)
                {
                    // We found it
                    Debug.Assert(index + 1 < _oFallback._arrayBestFit.Length,
                        "[InternalDecoderBestFitFallbackBuffer.TryBestFit]Expected replacement character at end of array");
                    return _oFallback._arrayBestFit[index + 1];
                }
            }

            // Char wasn't in our table
            return '\0';
        }
    }
}
