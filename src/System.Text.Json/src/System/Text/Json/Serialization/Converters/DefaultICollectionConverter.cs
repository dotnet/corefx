// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonICollectionT<TSource> : ICollection<TSource>
    {
        private List<TSource> _list;
        public JsonICollectionT(IList sourceList)
        {
            _list = new List<TSource>();
            foreach (object item in sourceList)
            {
                if (item is TSource itemTSource)
                {
                    _list.Add(itemTSource);
                }
            }
        }

        public int Count => ((ICollection<TSource>)_list).Count;

        public bool IsReadOnly => ((ICollection<TSource>)_list).IsReadOnly;

        public void Add(TSource item)
        {
            ((ICollection<TSource>)_list).Add(item);
        }

        public void Clear()
        {
            ((ICollection<TSource>)_list).Clear();
        }

        public bool Contains(TSource item)
        {
            return ((ICollection<TSource>)_list).Contains(item);
        }

        public void CopyTo(TSource[] array, int arrayIndex)
        {
            ((ICollection<TSource>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return ((ICollection<TSource>)_list).GetEnumerator();
        }

        public bool Remove(TSource item)
        {
            return ((ICollection<TSource>)_list).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal sealed class DefaultICollectionTConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(Type elementType, IList sourceList)
        {
            Type t = typeof(JsonICollectionT<>).MakeGenericType(elementType);
            return (IEnumerable)Activator.CreateInstance(t, sourceList);
        }
    }
}
