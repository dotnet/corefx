﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Collections.Tests
{
    public class HashSet_IEnumerable_NonGeneric_Tests : IEnumerable_NonGeneric_Tests
    {
        protected override IEnumerable NonGenericIEnumerableFactory(int count)
        {
            var set = new HashSet<string>();
            int seed = 12354;
            while (set.Count < count)
                set.Add(CreateT(set, seed++));
            return set;
        }

        protected override bool Enumerator_Current_UndefinedOperation_Throws { get { return true; } }

        /// <summary>
        /// Returns a set of ModifyEnumerable delegates that modify the enumerable passed to them.
        /// </summary>
        protected override IEnumerable<ModifyEnumerable> ModifyEnumerables
        {
            get
            {
                yield return (IEnumerable enumerable) => {
                    HashSet<string> casted = ((HashSet<string>)enumerable);
                    if (casted.Count > 0)
                    {
                        casted.Clear();
                        return true;
                    }
                    return false;
                };
            }
        }

        protected string CreateT(HashSet<string> set, int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            string ret = Convert.ToBase64String(bytes);
            while (set.Contains(ret))
            {
                rand.NextBytes(bytes);
                ret = Convert.ToBase64String(bytes);
            }
            return ret;
        }

    }
}