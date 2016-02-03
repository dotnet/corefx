// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Collections;
using System.Collections.Specialized;
using GenStrings;

namespace System.Collections.Specialized.Tests
{
    public class AddStrStrStringDictionaryTests
    {
        public const int MAX_LEN = 50;          // max length of random strings

        [Fact]
        public void Test01()
        {
            IntlStrings intl;
            StringDictionary sd;
            // simple string values
            string[] values =
            {
                "",
                " ",
                "a",
                "aa",
                "text",
                "     spaces",
                "1",
                "$%^#",
                "2222222222222222222222222",
                System.DateTime.Today.ToString(),
                Int32.MaxValue.ToString()
            };

            // keys for simple string values
            string[] keys =
            {
                "zero",
                "one",
                " ",
                "",
                "aa",
                "1",
                System.DateTime.Today.ToString(),
                "$%^#",
                Int32.MaxValue.ToString(),
                "     spaces",
                "2222222222222222222222222"
            };

            int cnt = 0;            // Count
            string ind;            // key
            // initialize IntStrings
            intl = new IntlStrings();


            // [] StringDictionary is constructed as expected
            //-----------------------------------------------------------------

            sd = new StringDictionary();

            // [] Add() simple strings
            //
            for (int i = 0; i < values.Length; i++)
            {
                cnt = sd.Count;
                sd.Add(keys[i], values[i]);
                if (sd.Count != cnt + 1)
                {
                    Assert.False(true, string.Format("Error, count is {1} instead of {2}", i, sd.Count, cnt + 1));
                }

                // verify that collection contains newly added item
                //
                if (!sd.ContainsValue(values[i]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain value of new item", i));
                }

                if (!sd.ContainsKey(keys[i]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain key of new item", i));
                }

                //  access the item
                //
                if (String.Compare(sd[keys[i]], values[i]) != 0)
                {
                    Assert.False(true, string.Format("Error, returned item \"{1}\" instead of \"{2}\"", i, sd[keys[i]], values[i]));
                }
            }

            //
            // Intl strings
            // [] Add() Intl strings
            //
            int len = values.Length;
            string[] intlValues = new string[len * 2];

            // fill array with unique strings
            //
            for (int i = 0; i < len * 2; i++)
            {
                string val = intl.GetRandomString(MAX_LEN);
                while (Array.IndexOf(intlValues, val) != -1)
                    val = intl.GetRandomString(MAX_LEN);
                intlValues[i] = val;
            }

            Boolean caseInsensitive = false;
            for (int i = 0; i < len * 2; i++)
            {
                if (intlValues[i].Length != 0 && intlValues[i].ToLowerInvariant() == intlValues[i].ToUpperInvariant())
                    caseInsensitive = true;
            }

            //
            // will use first half of array as values and second half as keys
            //
            for (int i = 0; i < len; i++)
            {
                cnt = sd.Count;

                sd.Add(intlValues[i + len], intlValues[i]);
                if (sd.Count != cnt + 1)
                {
                    Assert.False(true, string.Format("Error, count is {1} instead of {2}", i, sd.Count, cnt + 1));
                }

                // verify that collection contains newly added item
                //
                if (!sd.ContainsValue(intlValues[i]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain value of new item", i));
                }

                if (!sd.ContainsKey(intlValues[i + len]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain key of new item", i));
                }

                //  access the item
                //
                ind = intlValues[i + len];
                if (String.Compare(sd[ind], intlValues[i]) != 0)
                {
                    Assert.False(true, string.Format("Error, returned item \"{1}\" instead of \"{2}\"", i, sd[ind], intlValues[i]));
                }
            }

            //
            // [] Case sensitivity
            //
            string[] intlValuesLower = new string[len * 2];

            // fill array with unique strings
            //
            for (int i = 0; i < len * 2; i++)
            {
                intlValues[i] = intlValues[i].ToUpperInvariant();
            }

            for (int i = 0; i < len * 2; i++)
            {
                intlValuesLower[i] = intlValues[i].ToLowerInvariant();
            }

            sd.Clear();
            //
            // will use first half of array as values and second half as keys
            //
            for (int i = 0; i < len; i++)
            {
                cnt = sd.Count;

                sd.Add(intlValues[i + len], intlValues[i]);
                if (sd.Count != cnt + 1)
                {
                    Assert.False(true, string.Format("Error, count is {1} instead of {2}", i, sd.Count, cnt + 1));
                }

                // verify that collection contains newly added uppercase item
                //
                if (!sd.ContainsValue(intlValues[i]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain value of new item", i));
                }

                if (!sd.ContainsKey(intlValues[i + len]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain key of new item", i));
                }

                // verify that collection doesn't contains lowercase item
                //
                if (!caseInsensitive && sd.ContainsValue(intlValuesLower[i]))
                {
                    Assert.False(true, string.Format("Error, collection contains lowercase value of new item", i));
                }

                // key is case insensitive
                if (!sd.ContainsKey(intlValuesLower[i + len]))
                {
                    Assert.False(true, string.Format("Error, collection doesn't contain lowercase key of new item", i));
                }
            }

            //
            // [] Add (string, null)
            //

            cnt = sd.Count;
            sd.Add("keykey", null);

            if (sd.Count != cnt + 1)
            {
                Assert.False(true, string.Format("Error, count is {0} instead of {1}", sd.Count, cnt + 1));
            }

            // verify that collection contains null
            //
            if (!sd.ContainsValue(null))
            {
                Assert.False(true, string.Format("Error, collection doesn't contain null"));
            }

            // access item-null
            //
            if (sd["keykey"] != null)
            {
                Assert.False(true, string.Format("Error, returned non-null on place of null"));
            }

            //
            // Add item with null key - ArgumentNullException expected
            // [] Add (null, string)
            //
            Assert.Throws<ArgumentNullException>(() => { sd.Add(null, "item"); });

            //
            // Add duplicate key item - ArgumentException expected
            // [] Add duplicate key item
            //

            // generate key
            string k = intl.GetRandomString(MAX_LEN);
            if (!sd.ContainsKey(k))
            {
                sd.Add(k, "newItem");
            }
            if (!sd.ContainsKey(k))
            {
                Assert.False(true, string.Format("Error,failed to add item"));
            }
            else
            {
                Assert.Throws<ArgumentException>(() => { sd.Add(k, "itemitemitem"); });
            }
        }
    }
}
