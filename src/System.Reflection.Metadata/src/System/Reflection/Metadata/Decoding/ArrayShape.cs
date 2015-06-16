// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata
{
    public struct ArrayShape
    {
        private readonly int _rank;
        private readonly ImmutableArray<int> _sizes;
        private readonly ImmutableArray<int> _lowerBounds;

        public ArrayShape(int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
        {
            _rank = rank;
            _sizes = sizes;
            _lowerBounds = lowerBounds;
        }

        public int Rank
        {
            get { return _rank; }
        }

        public ImmutableArray<int> Sizes
        {
            get { return _sizes; }
        }

        public ImmutableArray<int> LowerBounds
        {
            get { return _lowerBounds; }
        }
    }
}
