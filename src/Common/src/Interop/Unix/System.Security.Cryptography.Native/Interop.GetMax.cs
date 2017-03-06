// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        /// <summary>
        /// Return the maximum value in the array; assumes non-negative values.
        /// </summary>
        private static int GetMax(int[] values)
        {
            int max = 0;

            foreach (var i in values)
            {
                Debug.Assert(i >= 0);
                if (i > max)
                    max = i;
            }

            return max;
        }

        /// <summary>
        /// Return the maximum value in the array; assumes non-negative values.
        /// </summary>
        private static int GetMax(int value1, int value2)
        {
            Debug.Assert(value1 >= 0);
            Debug.Assert(value2 >= 0);
            return (value1 > value2 ? value1 : value2);
        }

        /// <summary>
        /// Return the maximum value in the array; assumes non-negative values.
        /// </summary>
        private static int GetMax(int value1, int value2, int value3)
        {
            return GetMax(GetMax(value1, value2), value3);
        }
    }
}