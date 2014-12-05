// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace System.Composition.UnitTests.Util
{
    static class AssertX
    {
        public static TException Throws<TException>(Action action)
            where TException : Exception
        {
            return Assert.Throws<TException>(action);
        }

        public static void Contains(string expectedSubstring, string actualString, string format, params object[] args)
        {
            if (actualString == null || actualString.IndexOf(expectedSubstring) < 0)
            {
                string message = string.Format(format, args);
                throw new AssertActualExpectedException(expectedSubstring, actualString, message, "Not found", "In value");
            }
        }

        public static void Equal<T>(T expected, T actual, string format, params object[] args)
        {
            try
            {
                Assert.Equal(expected, actual);
            }
            catch (EqualException)
            {
                //  Assert failed, throw an exception with the specified user message
                string message = string.Format(format, args);
                throw new AssertActualExpectedException(expected, actual, message);
            }
        }

        public static void Same(object expected, object actual, string format, params object[] args)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                string message = string.Format(format, args);
                throw new AssertActualExpectedException(expected, actual, message);
            }
        }

        public static void Fail(string format, params object[] args)
        {
            Assert.False(true, string.Format(format, args));
        }

        public static void Equivalent(ICollection expected, ICollection actual, string message = null, params object[] args)
        {
            Dictionary<object, int> expectedCounts = GetObjectCounts(expected);
            Dictionary<object, int> actualCounts = GetObjectCounts(actual);

            if (!AreCollectionsEquivalent(expectedCounts, actualCounts))
            {
                string messageToUse = (message != null) ? String.Format(message, args) : "Collections are not equivalent.";
                throw new AssertActualExpectedException(expected, actual, messageToUse);
            }
        }

        private static bool AreCollectionsEquivalent(Dictionary<object, int> expectedCounts, Dictionary<object, int> actualCounts)
        {
            if (expectedCounts.Count != actualCounts.Count)
            {
                return false;
            }

            foreach (var kv in expectedCounts)
            {
                int actualCount;
                if (actualCounts.TryGetValue(kv.Key, out actualCount))
                {
                    if (actualCount != kv.Value)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<object, int> GetObjectCounts(ICollection collection)
        {
            var dict = new Dictionary<object, int>();
            foreach (var obj in collection)
            {
                int count;
                if (dict.TryGetValue(obj, out count))
                {
                    dict[obj] = count + 1;
                }
                else
                {
                    dict[obj] = 1;
                }
            }
            return dict;
        }
    }
}
