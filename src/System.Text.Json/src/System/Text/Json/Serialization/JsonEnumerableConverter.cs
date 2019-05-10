// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json.Serialization.Policies
{
    internal abstract class JsonEnumerableConverter
    {
        public abstract IEnumerable CreateFromList(Type enumerableType, Type elementType, IList sourceList);

        protected static IEnumerable<T> GetGenericEnumerableFromList<T>(IList sourceList)
        {
            foreach (object item in sourceList)
            {
                yield return (T)item;
            }
        }
    }
}
