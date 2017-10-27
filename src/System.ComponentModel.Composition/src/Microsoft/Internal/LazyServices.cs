// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Internal
{
    internal static class LazyServices
    {
        public static T GetNotNullValue<T>(this Lazy<T> lazy, string argument)
            where T : class
        {
            Assumes.NotNull(lazy);
            T value = lazy.Value;
            if (value == null)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, SR.LazyServices_LazyResolvesToNull, typeof(T), argument));
            }

            return value;
        }
    }
}
