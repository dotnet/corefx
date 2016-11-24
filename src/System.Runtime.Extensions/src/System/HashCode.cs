// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

namespace System
{
    public struct HashCode : IEquatable<HashCode>
    {
    	private readonly int _hash;
        
        private HashCode(int hash)
        {
            _hash = hash;
        }

        public static HashCode Empty => default(HashCode);
    
        public static HashCode Create(int hash) => new HashCode(hash);
        
        public static HashCode Create<T>(T value)
        {
        	return Create(GetHashCodeDefaultComparer(value));
        }
        
        public static HashCode Create<T>(T value, IEqualityComparer<T> comparer)
        {
        	return Create(GetHashCodeWithComparer(value, comparer));
        }

        public HashCode Combine(int hash) => Create(CombineCore(_hash, hash));
        
        public HashCode Combine<T>(T value)
        {
        	return Combine(GetHashCodeDefaultComparer(value));
        }
        
        public HashCode Combine<T>(T value, IEqualityComparer<T> comparer)
        {
        	return Combine(GetHashCodeWithComparer(value, comparer));
        }

        public int Value => _hash;

        public static implicit operator int(HashCode hashCode) => hashCode._hash;

        public static bool operator ==(HashCode left, HashCode right) => left._hash == right._hash;
        
        public static bool operator !=(HashCode left, HashCode right) => !(left == right);

        public bool Equals(HashCode other) => _hash == other._hash;
        
        public override bool Equals(object obj) => obj is HashCode other && Equals(other);
        
        public override int GetHashCode() => _hash;

        public override string ToString() => _hash.ToString(CultureInfo.InvariantCulture);
        
        private static int CombineCore(int left, int right)
        {
        	uint rol5 = ((uint)left << 5) | ((uint)left >> 27);
            return ((int)rol5 + left) ^ right;
        }
        
        private static int GetHashCodeDefaultComparer<T>(T value)
        {
        	// This has the same behavior as EqualityComparer<T>.Default.GetHashCode(value).
            // The difference is this is faster because we avoid a virtual call.
            return value?.GetHashCode() ?? 0;
        }
        
        private static int GetHashCodeWithComparer<T>(T value, IEqualityComparer<T> comparer)
        {
            return value == null ? 0 :
                comparer == null ? GetHashCodeDefaultComparer(value) :
                comparer.GetHashCode(value);
        }
    }
}
