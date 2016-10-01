// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.IO.Compression
{
    internal class FastEncoderWindow
    {
        private byte[] _window;             // complete bytes window
        private int _bufPos;               // the start index of uncompressed bytes  
        private int _bufEnd;                // the end index of uncompressed bytes

        // Be very careful about increasing the window size; the code tables will have to
        // be updated, since they assume that extra_distance_bits is never larger than a
        // certain size.
        private const int FastEncoderHashShift = 4;
        private const int FastEncoderHashtableSize = 2048;
        private const int FastEncoderHashMask = FastEncoderHashtableSize - 1;
        private const int FastEncoderWindowSize = 8192;
        private const int FastEncoderWindowMask = FastEncoderWindowSize - 1;
        private const int FastEncoderMatch3DistThreshold = 16384;
        internal const int MaxMatch = 258;
        internal const int MinMatch = 3;

        // Following constants affect the search, 
        // they should be modifiable if we support different compression levels in future.
        private const int SearchDepth = 32;
        private const int GoodLength = 4;
        private const int NiceLength = 32;
        private const int LazyMatchThreshold = 6;

        // Hashtable structure
        private ushort[] _prev;     // next most recent occurance of chars with same hash value 
        private ushort[] _lookup;   // hash table to find most recent occurance of chars with same hash value

        public FastEncoderWindow()
        {
            ResetWindow();
        }

        public int BytesAvailable
        {         // uncompressed bytes 
            get
            {
                Debug.Assert(_bufEnd - _bufPos >= 0, "Ending pointer can't be in front of starting pointer!");
                return _bufEnd - _bufPos;
            }
        }

        public DeflateInput UnprocessedInput
        {
            get
            {
                DeflateInput input = new DeflateInput();
                input.Buffer = _window;
                input.StartIndex = _bufPos;
                input.Count = _bufEnd - _bufPos;
                return input;
            }
        }

        public void FlushWindow()
        {
            ResetWindow();
        }

        private void ResetWindow()
        {
            _window = new byte[2 * FastEncoderWindowSize + MaxMatch + 4];
            _prev = new ushort[FastEncoderWindowSize + MaxMatch];
            _lookup = new ushort[FastEncoderHashtableSize];
            _bufPos = FastEncoderWindowSize;
            _bufEnd = _bufPos;
        }

        public int FreeWindowSpace
        {        // Free space in the window
            get
            {
                return 2 * FastEncoderWindowSize - _bufEnd;
            }
        }

        // copy bytes from input buffer into window
        public void CopyBytes(byte[] inputBuffer, int startIndex, int count)
        {
            Array.Copy(inputBuffer, startIndex, _window, _bufEnd, count);
            _bufEnd += count;
        }

        // slide the history window to the left by FastEncoderWindowSize bytes
        public void MoveWindows()
        {
            int i;
            Debug.Assert(_bufPos == 2 * FastEncoderWindowSize, "only call this at the end of the window");

            // verify that the hash table is correct
            VerifyHashes();   // Debug only code

            Array.Copy(_window, _bufPos - FastEncoderWindowSize, _window, 0, FastEncoderWindowSize);

            // move all the hash pointers back
            for (i = 0; i < FastEncoderHashtableSize; i++)
            {
                int val = ((int)_lookup[i]) - FastEncoderWindowSize;

                if (val <= 0)
                { // too far away now? then set to zero
                    _lookup[i] = (ushort)0;
                }
                else
                {
                    _lookup[i] = (ushort)val;
                }
            }

            // prev[]'s are absolute pointers, not relative pointers, so we have to move them back too
            // making prev[]'s into relative pointers poses problems of its own
            for (i = 0; i < FastEncoderWindowSize; i++)
            {
                long val = ((long)_prev[i]) - FastEncoderWindowSize;

                if (val <= 0)
                {
                    _prev[i] = (ushort)0;
                }
                else
                {
                    _prev[i] = (ushort)val;
                }
            }

#if DEBUG
            // For debugging, wipe the window clean, so that if there is a bug in our hashing,
            // the hash pointers will now point to locations which are not valid for the hash value
            // (and will be caught by our ASSERTs).
            Array.Clear(_window, FastEncoderWindowSize, _window.Length - FastEncoderWindowSize);
#endif

            VerifyHashes(); // debug: verify hash table is correct

            _bufPos = FastEncoderWindowSize;
            _bufEnd = _bufPos;
        }

        private uint HashValue(uint hash, byte b)
        {
            return (hash << FastEncoderHashShift) ^ b;
        }

        // insert string into hash table and return most recent location of same hash value
        private uint InsertString(ref uint hash)
        {
            // Note we only use the lowest 11 bits of the hash vallue (hash table size is 11).
            // This enables fast calculation of hash value for the input string.
            // If we want to get the next hash code starting at next position, 
            // we can just increment bufPos and call this function.

            hash = HashValue(hash, _window[_bufPos + 2]);

            // Need to assert the hash value
            uint search = _lookup[hash & FastEncoderHashMask];
            _lookup[hash & FastEncoderHashMask] = (ushort)_bufPos;
            _prev[_bufPos & FastEncoderWindowMask] = (ushort)search;
            return search;
        }

        //
        // insert strings into hashtable
        // Arguments: 
        //     hash     : intial hash value
        //     matchLen : 1 + number of strings we need to insert.
        //
        private void InsertStrings(ref uint hash, int matchLen)
        {
            Debug.Assert(matchLen > 0, "Invalid match Len!");
            if (_bufEnd - _bufPos <= matchLen)
            {
                _bufPos += (matchLen - 1);
            }
            else
            {
                while (--matchLen > 0)
                {
                    InsertString(ref hash);
                    _bufPos++;
                }
            }
        }

        //
        // Find out what we should generate next. It can be a symbol, a distance/length pair
        // or a symbol followed by distance/length pair
        //
        internal bool GetNextSymbolOrMatch(Match match)
        {
            Debug.Assert(_bufPos >= FastEncoderWindowSize && _bufPos < (2 * FastEncoderWindowSize), "Invalid Buffer Position!");

            // initialise the value of the hash, no problem if locations bufPos, bufPos+1 
            // are invalid (not enough data), since we will never insert using that hash value
            uint hash = HashValue(0, _window[_bufPos]);
            hash = HashValue(hash, _window[_bufPos + 1]);

            int matchLen;
            int matchPos = 0;

            VerifyHashes();   // Debug only code
            if (_bufEnd - _bufPos <= 3)
            {
                // The hash value becomes corrupt when we get within 3 characters of the end of the
                // input window, since the hash value is based on 3 characters.  We just stop
                // inserting into the hash table at this point, and allow no matches.
                matchLen = 0;
            }
            else
            {
                // insert string into hash table and return most recent location of same hash value
                int search = (int)InsertString(ref hash);

                // did we find a recent location of this hash value?
                if (search != 0)
                {
                    // yes, now find a match at what we'll call position X
                    matchLen = FindMatch(search, out matchPos, SearchDepth, NiceLength);

                    // truncate match if we're too close to the end of the input window
                    if (_bufPos + matchLen > _bufEnd)
                        matchLen = _bufEnd - _bufPos;
                }
                else
                {
                    // no most recent location found
                    matchLen = 0;
                }
            }

            if (matchLen < MinMatch)
            {
                // didn't find a match, so output unmatched char
                match.State = MatchState.HasSymbol;
                match.Symbol = _window[_bufPos];
                _bufPos++;
            }
            else
            {
                // bufPos now points to X+1
                _bufPos++;

                // is this match so good (long) that we should take it automatically without
                // checking X+1 ?
                if (matchLen <= LazyMatchThreshold)
                {
                    int nextMatchLen;
                    int nextMatchPos = 0;

                    // search at position X+1
                    int search = (int)InsertString(ref hash);

                    // no, so check for a better match at X+1
                    if (search != 0)
                    {
                        nextMatchLen = FindMatch(search, out nextMatchPos,
                                                   matchLen < GoodLength ? SearchDepth : (SearchDepth >> 2), NiceLength);

                        // truncate match if we're too close to the end of the window
                        // note: nextMatchLen could now be < MinMatch
                        if (_bufPos + nextMatchLen > _bufEnd)
                        {
                            nextMatchLen = _bufEnd - _bufPos;
                        }
                    }
                    else
                    {
                        nextMatchLen = 0;
                    }

                    // right now X and X+1 are both inserted into the search tree
                    if (nextMatchLen > matchLen)
                    {
                        // since nextMatchLen > matchLen, it can't be < MinMatch here

                        // match at X+1 is better, so output unmatched char at X
                        match.State = MatchState.HasSymbolAndMatch;
                        match.Symbol = _window[_bufPos - 1];
                        match.Position = nextMatchPos;
                        match.Length = nextMatchLen;

                        // insert remainder of second match into search tree            
                        // example: (*=inserted already)
                        //
                        // X      X+1               X+2      X+3     X+4
                        // *      *
                        //        nextmatchlen=3
                        //        bufPos
                        //
                        // If nextMatchLen == 3, we want to perform 2
                        // insertions (at X+2 and X+3).  However, first we must 
                        // inc bufPos.
                        //
                        _bufPos++; // now points to X+2
                        matchLen = nextMatchLen;
                        InsertStrings(ref hash, matchLen);
                    }
                    else
                    {
                        // match at X is better, so take it
                        match.State = MatchState.HasMatch;
                        match.Position = matchPos;
                        match.Length = matchLen;

                        // Insert remainder of first match into search tree, minus the first
                        // two locations, which were inserted by the FindMatch() calls.
                        // 
                        // For example, if matchLen == 3, then we've inserted at X and X+1
                        // already (and bufPos is now pointing at X+1), and now we need to insert 
                        // only at X+2.
                        //
                        matchLen--;
                        _bufPos++; // now bufPos points to X+2
                        InsertStrings(ref hash, matchLen);
                    }
                }
                else
                { // match_length >= good_match 
                    // in assertion: bufPos points to X+1, location X inserted already
                    // first match is so good that we're not even going to check at X+1
                    match.State = MatchState.HasMatch;
                    match.Position = matchPos;
                    match.Length = matchLen;

                    // insert remainder of match at X into search tree
                    InsertStrings(ref hash, matchLen);
                }
            }

            if (_bufPos == 2 * FastEncoderWindowSize)
            {
                MoveWindows();
            }
            return true;
        }

        //
        // Find a match starting at specified position and return length of match
        // Arguments:
        //      search       : where to start searching
        //      matchPos    : return match position here
        //      searchDepth  : # links to traverse
        //      NiceLength  : stop immediately if we find a match >= NiceLength
        //
        private int FindMatch(int search, out int matchPos, int searchDepth, int niceLength)
        {
            Debug.Assert(_bufPos >= 0 && _bufPos < 2 * FastEncoderWindowSize, "Invalid Buffer position!");
            Debug.Assert(search < _bufPos, "Invalid starting search point!");
            Debug.Assert(RecalculateHash((int)search) == RecalculateHash(_bufPos));

            int bestMatch = 0;    // best match length found so far
            int bestMatchPos = 0; // absolute match position of best match found

            // the earliest we can look
            int earliest = _bufPos - FastEncoderWindowSize;
            Debug.Assert(earliest >= 0, "bufPos is less than FastEncoderWindowSize!");

            byte wantChar = _window[_bufPos];
            while (search > earliest)
            {
                // make sure all our hash links are valid
                Debug.Assert(RecalculateHash((int)search) == RecalculateHash(_bufPos), "Corrupted hash link!");

                // Start by checking the character that would allow us to increase the match
                // length by one.  This improves performance quite a bit.
                if (_window[search + bestMatch] == wantChar)
                {
                    int j;

                    // Now make sure that all the other characters are correct
                    for (j = 0; j < MaxMatch; j++)
                    {
                        if (_window[_bufPos + j] != _window[search + j])
                            break;
                    }

                    if (j > bestMatch)
                    {
                        bestMatch = j;
                        bestMatchPos = search; // absolute position
                        if (j > NiceLength) break;
                        wantChar = _window[_bufPos + j];
                    }
                }

                if (--searchDepth == 0)
                {
                    break;
                }

                Debug.Assert(_prev[search & FastEncoderWindowMask] < search, "we should always go backwards!");

                search = _prev[search & FastEncoderWindowMask];
            }

            // doesn't necessarily mean we found a match; bestMatch could be > 0 and < MinMatch
            matchPos = _bufPos - bestMatchPos - 1; // convert absolute to relative position

            // don't allow match length 3's which are too far away to be worthwhile
            if (bestMatch == 3 && matchPos >= FastEncoderMatch3DistThreshold)
            {
                return 0;
            }

            Debug.Assert(bestMatch < MinMatch || matchPos < FastEncoderWindowSize, "Only find match inside FastEncoderWindowSize");
            return bestMatch;
        }


        // This function makes any execution take a *very* long time to complete.  
        // Disabling for now by using non-"DEBUG" compilation constant.
        [Conditional("VERIFY_HASHES")] 
        private void VerifyHashes()
        {
            for (int i = 0; i < FastEncoderHashtableSize; i++)
            {
                ushort where = _lookup[i];
                ushort nextWhere;

                while (where != 0 && _bufPos - where < FastEncoderWindowSize)
                {
                    Debug.Assert(RecalculateHash(where) == i, "Incorrect Hashcode!");
                    nextWhere = _prev[where & FastEncoderWindowMask];
                    if (_bufPos - nextWhere >= FastEncoderWindowSize)
                    {
                        break;
                    }

                    Debug.Assert(nextWhere < where, "pointer is messed up!");
                    where = nextWhere;
                }
            }
        }

        // can't use conditional attribute here.
        private uint RecalculateHash(int position)
        {
            return (uint)(((_window[position] << (2 * FastEncoderHashShift)) ^
                     (_window[position + 1] << FastEncoderHashShift) ^
                     (_window[position + 2])) & FastEncoderHashMask);
        }
    }
}

