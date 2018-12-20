﻿using System.Diagnostics;

namespace System.Linq.ChainLinq.Consumables
{
    /// <summary>
    /// https://github.com/xunit/xunit/issues/1870
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal abstract class Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<U, T> : ConsumableForMerging<U>, IConsumableInternal
    {
        protected ILink<T, U> Link { get; }

        protected Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug(ILink<T, U> link) =>
            Link = link;

        public override Consumable<V> AddTail<V>(ILink<U, V> next) =>
            Create(Links.Composition.Create(Link, next));

        public abstract Consumable<V> Create<V>(ILink<T, V> first);

        protected bool IsIdentity => ReferenceEquals(Link, Links.Identity<T>.Instance);

        public override object TailLink
        {
            get
            {
                if (Link is Links.Composition<T, U> composition)
                {
                    return composition.TailLink;
                }

                return Link;
            }
        }

        public override Consumable<V> ReplaceTailLink<Unknown,V>(ILink<Unknown,V> newLink)
        {
            if (Link is Links.Composition<T, U> composition)
            {
                return Create(composition.ReplaceTail(newLink));
            }

            Debug.Assert(typeof(Unknown) == typeof(T));
            return Create((ILink<T,V>)newLink);
        }
    }
}
