// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;

using Xunit;

public static class ArrayUtil
{
    /// <summary>
    /// Asserts that each element in the first array is equal to its corresponding element in the second array.
    /// </summary>
    public static void AssertAllArrayElementsAreEqual(Array first, Array second, IComparer comparer = null)
    {
        Assert.True(first.Length == second.Length, "The two arrays are not even the same size.");
        for (int g = first.GetLowerBound(0); g < first.GetUpperBound(0); g++)
        {
            AssertArrayElementsAreEqual(first, second, g, comparer);
        }
    }

    /// <summary>
    /// Asserts that the elements at the given index in both arrays are equal.
    /// </summary>
    /// <param name="comparer">An optional comparer to use.</param>
    public static void AssertArrayElementsAreEqual(Array first, Array second, int index, IComparer comparer = null)
    {
        object firstValue = first.GetValue(index);
        object secondValue = second.GetValue(index);

        if (comparer != null)
        {
            Assert.Equal(0, comparer.Compare(firstValue, secondValue));
        }
        else
        {
            Assert.Equal(firstValue, secondValue);
        }
    }

    /// <summary>
    /// Returns a new array containing the input array's elements in reverse order.
    /// </summary>
    /// <param name="input">The input array.</param>
    /// <returns>A reversed clone of the input array.</returns>
    public static Array ReverseArray(Array input)
    {
        Array clone = (Array)input.Clone();
        for (int g = 0; g < clone.Length; g++)
        {
            clone.SetValue(input.GetValue(input.Length - g - 1), g);
        }

        return clone;
    }

    /// <summary>
    /// A simple bubble sort for two arrays, limited to some region of the arrays.
    /// This is a regular bubble sort, nothing fancy.
    /// </summary>
    public static void SimpleSort(Array array, Array items, int startingIndex, int length, IComparer comparer)
    {
        bool finished = false;
        while (!finished)
        {
            bool swapped = false;
            for (int g = startingIndex; g < startingIndex + length - 1; g++)
            {
                object first = array.GetValue(g);
                object second = array.GetValue(g + 1);
                int comparison = comparer.Compare(first, second);
                if (comparison == 1)
                {
                    Swap(g, g + 1, array, items);
                    swapped = true;

                    first = array.GetValue(g);
                    second = array.GetValue(g + 1);
                }
            }
            if (!swapped)
            {
                finished = true;
            }
        }
    }

    private static void Swap(int from, int to, Array array, Array items)
    {
        object temp = array.GetValue(from);
        array.SetValue(array.GetValue(to), from);
        array.SetValue(temp, to);

        if (items != null)
        {
            temp = items.GetValue(from);
            items.SetValue(items.GetValue(to), from);
            items.SetValue(temp, to);
        }
    }
}
