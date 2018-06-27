// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.InteropServices.Tests
{
    public class CommonHelper
    {
        public static bool IsArrayEqual<T>(T[] array1, T[] array2)
        {
            if (array1.Length != array2.Length)
            {

                return false;
            }

            for (int i = 0; i < array1.Length; i++)
                if (!array1[i].Equals(array2[i]))
                {

                    return false;
                }

            return true;
        }

        public static bool IsSubArrayEqual<T>(T[] array1, T[] array2, int startIndex, int Length)
        {
            if (startIndex + Length > array1.Length)
            {

                return false;
            }

            if (startIndex + Length > array2.Length)
            {

                return false;
            }

            for (int i = 0; i < Length; i++)
                if (!array1[startIndex + i].Equals(array2[startIndex + i]))
                {

                    return false;
                }

            return true;
        }
    }
}
