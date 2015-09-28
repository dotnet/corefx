// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using System;
using System.Collections;
using System.Collections.Specialized;
using GenStrings;

namespace System.Collections.Specialized.Tests
{
    public class GetStrTests
    {
        public const int MAX_LEN = 50;          // max length of random strings

        [Fact]
        [ActiveIssue(2769, PlatformID.AnyUnix)]
        public void Test01()
        {
            IntlStrings intl;
            NameValueCollection nvc;

            // simple string values
            string[] values =
            {
                "",
                " ",
                "a",
                "aA",
                "text",
                "     SPaces",
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
                "oNe",
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

            // initialize IntStrings
            intl = new IntlStrings();


            // [] NameValueCollection is constructed as expected
            //-----------------------------------------------------------------

            nvc = new NameValueCollection();

            //  [] Get(int) on empty collection
            //
            if (nvc.Get(null) != null)
            {
                Assert.False(true, "Error, returned non-null");
            }
            if (nvc.Get("some_string") != null)
            {
                Assert.False(true, "Error, returned non-null");
            }

            //  [] Get(int) on collection filled with simple strings
            //

            cnt = nvc.Count;
            int len = values.Length;
            for (int i = 0; i < len; i++)
            {
                nvc.Add(keys[i], values[i]);
            }
            if (nvc.Count != len)
            {
                Assert.False(true, string.Format("Error, count is {0} instead of {1}", nvc.Count, values.Length));
            }
            //
            for (int i = 0; i < len; i++)
            {
                if (String.Compare(nvc.Get(keys[i]), values[i]) != 0)
                {
                    Assert.False(true, string.Format("Error, returned: \"{1}\", expected \"{2}\"", i, nvc.Get(keys[i]), values[i]));
                }
            }


            //
            // Intl strings
            //  [] Get(int) on collection filled with Intl strings
            //

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


            nvc.Clear();
            for (int i = 0; i < len; i++)
            {
                nvc.Add(intlValues[i + len], intlValues[i]);
            }
            if (nvc.Count != (len))
            {
                Assert.False(true, string.Format("Error, count is {0} instead of {1}", nvc.Count, len));
            }

            for (int i = 0; i < len; i++)
            {
                //
                if (String.Compare(nvc.Get(intlValues[i + len]), intlValues[i]) != 0)
                {
                    Assert.False(true, string.Format("Error, returned \"{1}\" instead of \"{2}\"", i, nvc.Get(intlValues[i + len]), intlValues[i]));
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

            nvc.Clear();
            //
            // will use first half of array as values and second half as keys
            //
            for (int i = 0; i < len; i++)
            {
                nvc.Add(intlValues[i + len], intlValues[i]);     // adding uppercase strings
            }

            //
            for (int i = 0; i < len; i++)
            {
                // uppercase key
                if (String.Compare(nvc.Get(intlValues[i + len]), intlValues[i]) != 0)
                {
                    Assert.False(true, string.Format("Error, returned \"{1}\" instead of \"{2}\"", i, nvc.Get(intlValues[i + len]), intlValues[i]));
                }

                // lowercase key
                if (String.Compare(nvc.Get(intlValuesLower[i + len]), intlValues[i]) != 0)
                {
                    Assert.False(true, string.Format("Error, returned \"{1}\" instead of \"{2}\"", i, nvc.Get(intlValuesLower[i + len]), intlValues[i]));
                }

                if (!caseInsensitive && String.Compare(nvc.Get(intlValues[i + len]), intlValuesLower[i]) == 0)
                {
                    Assert.False(true, string.Format("Error, returned lowercase when added uppercase", i));
                }
            }

            //  [] Get(int) on filled collection - with multiple items with the same keys
            //

            nvc.Clear();
            len = values.Length;
            string k = "keykey";
            string k1 = "hm1";
            string exp = "";
            string exp1 = "";
            for (int i = 0; i < len; i++)
            {
                nvc.Add(k, "Value" + i);
                nvc.Add(k1, "iTem" + i);
                if (i < len - 1)
                {
                    exp += "Value" + i + ",";
                    exp1 += "iTem" + i + ",";
                }
                else
                {
                    exp += "Value" + i;
                    exp1 += "iTem" + i;
                }
            }
            if (nvc.Count != 2)
            {
                Assert.False(true, string.Format("Error, count is {0} instead of {1}", nvc.Count, 2));
            }

            if (String.Compare(nvc.Get(k), exp) != 0)
            {
                Assert.False(true, string.Format("Error, returned \"{0}\" instead of \"{1}\"", nvc.Get(k), exp));
            }
            if (String.Compare(nvc.Get(k1), exp1) != 0)
            {
                Assert.False(true, string.Format("Error, returned \"{0}\" instead of \"{1}\"", nvc.Get(k1), exp1));
            }


            //
            //   [] Get(-1)
            //
            cnt = nvc.Count;
            nvc.Add(null, "nullValue");
            if (String.Compare(nvc.Get(null), "nullValue") != 0)
            {
                Assert.False(true, string.Format("Error, returned \"{0}\" instead of \"{1}\"", nvc.Get(null), "nullValue"));
            }

            //
            //   [] Get(null)   - when no item with null key
            //
            nvc.Clear();
            for (int i = 0; i < len; i++)
            {
                nvc.Add(keys[i], values[i]);
            }

            string res = nvc.Get(null);
            if (res != null)
            {
                Assert.False(true, "Error, returned non-null ");
            }
        }
    }
}
