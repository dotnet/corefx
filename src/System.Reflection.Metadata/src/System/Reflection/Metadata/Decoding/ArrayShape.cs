// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata
{
    public struct ArrayShape
    {
        private readonly int rank;
        private readonly ImmutableArray<int> sizes;
        private readonly ImmutableArray<int> lowerBounds;

        public ArrayShape(int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
        {
            this.rank = rank;
            this.sizes = sizes;
            this.lowerBounds = lowerBounds;
        }

        public int Rank
        {
            get { return rank; }
        }

        public ImmutableArray<int> Sizes
        {
            get { return sizes; }
        }

        public ImmutableArray<int> LowerBounds
        {
            get { return lowerBounds; }
        }
    }
}
