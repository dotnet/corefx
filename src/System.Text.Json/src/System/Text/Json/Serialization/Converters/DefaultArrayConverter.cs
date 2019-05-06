﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultArrayConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(Type enumerableType, Type elementType, IList sourceList)
        {
            return CreateArrayFromList(elementType, sourceList);
        }
    }
}
