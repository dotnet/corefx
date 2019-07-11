// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace System.Text.Json
{
    internal abstract class ImmutableCollectionCreator
    {
        public abstract void RegisterCreatorDelegateFromMethod(MethodInfo creator);

        public abstract IEnumerable CreateImmutableCollection(IList items);
    }

    internal sealed class ImmutableCollectionCreatorHelper<TElement, TCollection> : ImmutableCollectionCreator
    {
        private Func<IEnumerable<TElement>, TCollection> _creatorDelegate;

        public override void RegisterCreatorDelegateFromMethod(MethodInfo creator)
        {
            Debug.Assert(_creatorDelegate == null);
            _creatorDelegate = (Func<IEnumerable<TElement>, TCollection>)creator.CreateDelegate(typeof(Func<IEnumerable<TElement>, TCollection>));
        }

        public override IEnumerable CreateImmutableCollection(IList items)
        {
            Debug.Assert(_creatorDelegate != null);
            return (IEnumerable)_creatorDelegate(CreateGenericTElementIEnumerable(items));
        }

        private IEnumerable<TElement> CreateGenericTElementIEnumerable(IList sourceList)
        {
            foreach (object item in sourceList)
            {
                yield return (TElement)item;
            }
        }
    }
}
