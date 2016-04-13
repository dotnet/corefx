// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
**
**
** Purpose: Hash table implementation
**
** 
===========================================================*/

using System;
using System.Runtime;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;

namespace System.Collections
{
    internal static class HashHelpers
    {
        public static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException(SR.Arg_HTCapacityOverflow);
            Contract.EndContractBlock();

            // Table of prime numbers to use as hash table sizes. 
            // A typical resize algorithm would pick the smallest prime number below
            // that is larger than twice the previous capacity. 
            // Suppose our Hashtable currently has capacity x and enough elements are added 
            // such that a resize needs to occur. Resizing first computes 2x then finds the 
            // first prime in the table greater than 2x, i.e. if primes are ordered 
            // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n. 
            // Doubling is important for preserving the asymptotic complexity of the 
            // hashtable operations such as add.  Having a prime guarantees that double 
            // hashing does not lead to infinite loops.  IE, your hash function will be 
            // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
            
            if (3 >= min) return 3;
            if (7 >= min) return 7;
            if (11 >= min) return 11;
            if (17 >= min) return 17;
            if (23 >= min) return 23;
            if (29 >= min) return 29;
            if (37 >= min) return 37;
            if (47 >= min) return 47;
            if (59 >= min) return 59;
            if (71 >= min) return 71;
            if (89 >= min) return 89;
            if (107 >= min) return 107;
            if (131 >= min) return 131;
            if (163 >= min) return 163;
            if (197 >= min) return 197;
            if (239 >= min) return 239;
            if (293 >= min) return 293;
            if (353 >= min) return 353;
            if (431 >= min) return 431;
            if (521 >= min) return 521;
            if (631 >= min) return 631;
            if (761 >= min) return 761;
            if (919 >= min) return 919;
            if (1103 >= min) return 1103;
            if (1327 >= min) return 1327;
            if (1597 >= min) return 1597;
            if (1931 >= min) return 1931;
            if (2333 >= min) return 2333;
            if (2801 >= min) return 2801;
            if (3371 >= min) return 3371;
            if (4049 >= min) return 4049;
            if (4861 >= min) return 4861;
            if (5839 >= min) return 5839;
            if (7013 >= min) return 7013;
            if (8419 >= min) return 8419;
            if (10103 >= min) return 10103;
            if (12143 >= min) return 12143;
            if (14591 >= min) return 14591;
            if (17519 >= min) return 17519;
            if (21023 >= min) return 21023;
            if (25229 >= min) return 25229;
            if (30293 >= min) return 30293;
            if (36353 >= min) return 36353;
            if (43627 >= min) return 43627;
            if (52361 >= min) return 52361;
            if (62851 >= min) return 62851;
            if (75431 >= min) return 75431;
            if (90523 >= min) return 90523;
            if (108631 >= min) return 108631;
            if (130363 >= min) return 130363;
            if (156437 >= min) return 156437;
            if (187751 >= min) return 187751;
            if (225307 >= min) return 225307;
            if (270371 >= min) return 270371;
            if (324449 >= min) return 324449;
            if (389357 >= min) return 389357;
            if (467237 >= min) return 467237;
            if (560689 >= min) return 560689;
            if (672827 >= min) return 672827;
            if (807403 >= min) return 807403;
            if (968897 >= min) return 968897;
            if (1162687 >= min) return 1162687;
            if (1395263 >= min) return 1395263;
            if (1674319 >= min) return 1674319;
            if (2009191 >= min) return 2009191;
            if (2411033 >= min) return 2411033;
            if (2893249 >= min) return 2893249;
            if (3471899 >= min) return 3471899;
            if (4166287 >= min) return 4166287;
            if (4999559 >= min) return 4999559;
            if (5999471 >= min) return 5999471;
            if (7199369 >= min) return 7199369;
            if (8639249 >= min) return 8639249;
            if (10367101 >= min) return 10367101;
            if (12440537 >= min) return 12440537;
            if (14928671 >= min) return 14928671;
            if (17914409 >= min) return 17914409;
            if (21497293 >= min) return 21497293;
            if (25796759 >= min) return 25796759;
            if (30956117 >= min) return 30956117;
            if (37147349 >= min) return 37147349;
            if (44576837 >= min) return 44576837;
            if (53492207 >= min) return 53492207;
            if (64190669 >= min) return 64190669;
            if (77028803 >= min) return 77028803;
            if (92434613 >= min) return 92434613;
            if (110921543 >= min) return 110921543;
            if (133105859 >= min) return 133105859;
            if (159727031 >= min) return 159727031;
            if (191672443 >= min) return 191672443;
            if (230006941 >= min) return 230006941;
            if (276008387 >= min) return 276008387;
            if (331210079 >= min) return 331210079;
            if (397452101 >= min) return 397452101;
            if (476942527 >= min) return 476942527;
            if (572331049 >= min) return 572331049;
            if (686797261 >= min) return 686797261;
            if (824156741 >= min) return 824156741;
            if (988988137 >= min) return 988988137;
            if (1186785773 >= min) return 1186785773;
            if (1424142949 >= min) return 1424142949;
            if (1708971541 >= min) return 1708971541;
            if (2050765853 >= min) return 2050765853;
            if (MaxPrimeArrayLength >= min) return MaxPrimeArrayLength;

            return min;
        }

        // Returns size of hashtable to grow to.
        public static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                Debug.Assert(MaxPrimeArrayLength == GetPrime(MaxPrimeArrayLength), "Invalid MaxPrimeArrayLength");
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }


        // This is the maximum prime smaller than Array.MaxArrayLength
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;
    }
}
