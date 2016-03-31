﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace System.Collections.Tests
{
    public class Dictionary_Generic_Tests_Values : ICollection_Generic_Tests<string>
    {
        protected override bool DefaultValueAllowed { get { return true; } }
        protected override bool DuplicateValuesAllowed { get { return true; } }
        protected override bool IsReadOnly { get { return true; } }
        protected override IEnumerable<ModifyEnumerable> ModifyEnumerables { get { return new List<ModifyEnumerable>(); } }

        protected override ICollection<string> GenericICollectionFactory()
        {
            return new Dictionary<string, string>().Values;
        }

        protected override ICollection<string> GenericICollectionFactory(int count)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            int seed = 13453;
            for (int i = 0; i < count; i++)
                list.Add(CreateT(seed++), CreateT(seed++));
            return list.Values;
        }

        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_ValueCollection_Constructor_NullDictionary(int count)
        {
            Assert.Throws<ArgumentNullException>(() => new Dictionary<string, string>.ValueCollection(null));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_ValueCollection_GetEnumerator(int count)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            int seed = 13453;
            while (dictionary.Count < count)
                dictionary.Add(CreateT(seed++), CreateT(seed++));
            dictionary.Values.GetEnumerator();
        }
    }

    public class Dictionary_Generic_Tests_Values_AsICollection : ICollection_NonGeneric_Tests
    {
        protected override bool NullAllowed { get { return true; } }
        protected override bool DuplicateValuesAllowed { get { return true; } }
        protected override bool IsReadOnly { get { return true; } }
        protected override bool Enumerator_Current_UndefinedOperation_Throws { get { return true; } }
        protected override bool ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowsArgumentException { get { return true; } }
        protected override IEnumerable<ModifyEnumerable> ModifyEnumerables { get { return new List<ModifyEnumerable>(); } }
        protected override ICollection NonGenericICollectionFactory()
        {
            return (ICollection)(new Dictionary<string, string>().Values);
        }

        protected override ICollection NonGenericICollectionFactory(int count)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            int seed = 13453;
            for (int i = 0; i < count; i++)
                list.Add(CreateT(seed++), CreateT(seed++));
            return (ICollection)(list.Values);
        }

        private string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        protected override void AddToCollection(ICollection collection, int numberOfItemsToAdd)
        {
            Debug.Assert(false);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void Dictionary_Generic_ValueCollection_CopyTo_ExactlyEnoughSpaceInTypeCorrectArray(int count)
        {
            ICollection collection = NonGenericICollectionFactory(count);
            string[] array = new string[count];
            collection.CopyTo(array, 0);
            int i = 0;
            foreach (object obj in collection)
                Assert.Equal(array[i++], obj);
        }
    }
}
