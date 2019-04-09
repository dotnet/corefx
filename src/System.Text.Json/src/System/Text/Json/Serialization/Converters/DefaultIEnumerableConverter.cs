// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal class JsonIEnumerableT<TSource> : IEnumerable<TSource>
    {
        private List<TSource> _list;
        public JsonIEnumerableT(IList sourceList)
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

        public IEnumerator<TSource> GetEnumerator()
        {
            return ((IEnumerable<TSource>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal sealed class DefaultIEnumerableTConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(Type elementType, IList sourceList)
        {
            Type t = typeof(JsonIEnumerableT<>).MakeGenericType(elementType);
            return (IEnumerable)Activator.CreateInstance(t, sourceList);
        }
    }
}
