// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Generic
{
    internal static class ComparerUtilities
    {
        public static bool AreComparersEqual<T>(IComparer<T> left, IComparer<T> right)
        {
            if (left == right)
            {
                return true;
            }

            var @default = Comparer<T>.Default;

            if (left == null)
            {
                // Micro-opt: Typically it's impossible to get a different instance
                // of the default comparer without reflection/serialization.
                // Save a virtual method call to Equals in the common case.
                return right == @default || right.Equals(@default);
            }

            if (right == null)
            {
                return left == @default || left.Equals(@default);
            }

            return left.Equals(right);
        }

        public static bool AreEqualityComparersEqual<T>(IEqualityComparer<T> left, IEqualityComparer<T> right)
        {
            if (left == right)
            {
                return true;
            }

            var @default = EqualityComparer<T>.Default;

            if (left == null)
            {
                // Micro-opt: Typically it's impossible to get a different instance
                // of the default comparer without reflection/serialization.
                // Save a virtual method call to Equals in the common case.
                return right == @default || right.Equals(@default);
            }

            if (right == null)
            {
                return left == @default || left.Equals(@default);
            }

            return left.Equals(right);
        }
    }
}
