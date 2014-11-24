// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Validation;

namespace System.Collections.Immutable
{
    /// <content>
    /// Contains the inner MutationResult class.
    /// </content>
    public partial class ImmutableHashSet<T>
    {
        /// <summary>
        /// Interpretations for a count member.
        /// </summary>
        private enum CountType
        {
            /// <summary>
            /// The count member describes an adjustment to the previous count of the collection.
            /// </summary>
            Adjustment,

            /// <summary>
            /// The count member describes the actual count of the collection.
            /// </summary>
            FinalValue,
        }

        /// <summary>
        /// Describes the result of a mutation on the immutable data structure.
        /// </summary>
        private struct MutationResult
        {
            /// <summary>
            /// The root node of the data structure after the mutation.
            /// </summary>
            private readonly ImmutableSortedDictionary<int, HashBucket>.Node _root;

            /// <summary>
            /// Either the number of elements added or removed from the collection as a result of the operation (a negative number represents removed elements),
            /// or the total number of elements in the collection after the mutation.  The appropriate interpretation of this value is indicated by the 
            /// <see cref="_countType"/> field.
            /// </summary>
            private readonly int _count;

            /// <summary>
            /// Whether to consider the <see cref="_count"/> field to be a count adjustment or total count.
            /// </summary>
            private readonly CountType _countType;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmutableHashSet&lt;T&gt;.MutationResult"/> struct.
            /// </summary>
            /// <param name="root">The root node of the result.</param>
            /// <param name="count">The total element count or a count adjustment.</param>
            /// <param name="countType">The appropriate interpretation for the <paramref name="count"/> parameter.</param>
            internal MutationResult(ImmutableSortedDictionary<int, HashBucket>.Node root, int count, CountType countType = ImmutableHashSet<T>.CountType.Adjustment)
            {
                Requires.NotNull(root, "root");
                this._root = root;
                this._count = count;
                this._countType = countType;
            }

            /// <summary>
            /// Gets the root node of the data structure after the mutation.
            /// </summary>
            internal ImmutableSortedDictionary<int, HashBucket>.Node Root
            {
                get { return this._root; }
            }

            /// <summary>
            /// Gets either the number of elements added or removed from the collection as a result of the operation (a negative number represents removed elements),
            /// or the total number of elements in the collection after the mutation.  The appropriate interpretation of this value is indicated by the 
            /// <see cref="CountType"/> property.
            /// </summary>
            internal int Count
            {
                get { return this._count; }
            }

            /// <summary>
            /// Gets the appropriate interpration for the <see cref="Count"/> property; whether to be a count adjustment or total count.
            /// </summary>
            internal CountType CountType
            {
                get { return this._countType; }
            }

            /// <summary>
            /// Returns an immutable hash set that captures the result of this mutation.
            /// </summary>
            /// <param name="priorSet">The prior version of the set.  Used to capture the equality comparer and previous count, when applicable.</param>
            /// <returns>The new collection.</returns>
            internal ImmutableHashSet<T> Finalize(ImmutableHashSet<T> priorSet)
            {
                Requires.NotNull(priorSet, "priorSet");
                int count = this.Count;
                if (this.CountType == ImmutableHashSet<T>.CountType.Adjustment)
                {
                    count += priorSet._count;
                }

                return priorSet.Wrap(this.Root, count);
            }
        }
    }
}
