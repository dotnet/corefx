// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonIListT<TSource> : IList<TSource>
    {
        private List<TSource> _list;
        public JsonIListT(IList sourceList)
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

        public TSource this[int index] { get => ((IList<TSource>)_list)[index]; set => ((IList<TSource>)_list)[index] = value; }

        public int Count => ((IList<TSource>)_list).Count;

        public bool IsReadOnly => ((IList<TSource>)_list).IsReadOnly;

        public void Add(TSource item)
        {
            ((IList<TSource>)_list).Add(item);
        }

        public void Clear()
        {
            ((IList<TSource>)_list).Clear();
        }

        public bool Contains(TSource item)
        {
            return ((IList<TSource>)_list).Contains(item);
        }

        public void CopyTo(TSource[] array, int arrayIndex)
        {
            ((IList<TSource>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return ((IList<TSource>)_list).GetEnumerator();
        }

        public int IndexOf(TSource item)
        {
            return ((IList<TSource>)_list).IndexOf(item);
        }

        public void Insert(int index, TSource item)
        {
            ((IList<TSource>)_list).Insert(index, item);
        }

        public bool Remove(TSource item)
        {
            return ((IList<TSource>)_list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<TSource>)_list).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal sealed class DefaultIListTConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(Type elementType, IList sourceList)
        {
            Type t = typeof(JsonIListT<>).MakeGenericType(elementType);
            return (IEnumerable)Activator.CreateInstance(t, sourceList);
        }
    }
}
