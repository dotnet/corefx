// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultIEnumerableConstructibleConverter : JsonEnumerableConverter
    {
        // This method is invoked with reflection in CreateFromList.
        private static IEnumerable CreateFromListInternal<TEnumerable, TElement>(IList sourceList)
        {
            List<TElement> list = new List<TElement>();

            foreach (object item in sourceList)
            {
                if (item is TElement itemTElement)
                {
                    list.Add(itemTElement);
                }
            }

            return (IEnumerable)Activator.CreateInstance(typeof(TEnumerable), list);
        }

        public override IEnumerable CreateFromList(Type enumerableType, Type elementType, IList sourceList)
        {
            MethodInfo mi = typeof(DefaultIEnumerableConstructibleConverter).GetMethod("CreateFromListInternal", BindingFlags.NonPublic | BindingFlags.Static);

            if (mi == null)
            {
                return sourceList;
            }

            MethodInfo createFromListInternalMethod = mi.MakeGenericMethod(enumerableType, elementType);
            return (IEnumerable)createFromListInternalMethod.Invoke(null, new object[] { sourceList });
        }
    }
}
