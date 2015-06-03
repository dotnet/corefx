// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Resources
{
    //internal sealed class FastResourceComparer : IComparer, IEqualityComparer, IComparer<String>, IEqualityComparer<String>
    internal sealed class FastResourceComparer :IComparer<String>, IEqualityComparer<String>
    {
        internal static readonly FastResourceComparer Default = new FastResourceComparer();


        public int GetHashCode(String key)
        {
            return FastResourceComparer.HashFunction(key);
        }

        // This hash function MUST be publically documented with the resource
        // file format, AND we may NEVER change this hash function's return 
        // value (without changing the file format).
        internal static int HashFunction(String key)
        {
            // Never change this hash function.  We must standardize it so that 
            // others can read & write our .resources files.  Additionally, we
            // have a copy of it in InternalResGen as well.
            uint hash = 5381;
            for (int i = 0; i < key.Length; i++)
                hash = ((hash << 5) + hash) ^ key[i];
            return (int)hash;
        }

        public int Compare(String a, String b)
        {
            return String.CompareOrdinal(a, b);
        }

        public bool Equals(String a, String b)
        {
            return String.Equals(a, b);
        }

     
    }
}

