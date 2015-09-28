﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Collections.ObjectModel.Tests
{
    public class KeyedCollectionTest
    {
        [Fact]
        public void StringComparer()
        {
            var collection =
                new TestKeyedCollectionOfIKeyedItem<string, int>(
                    System.StringComparer.OrdinalIgnoreCase)
                {
                    new KeyedItem<string, int>("foo", 0),
                    new KeyedItem<string, int>("bar", 1)
                };
            Assert.Throws<ArgumentException>(
                () =>
                collection.Add(new KeyedItem<string, int>("Foo", 0)));
            Assert.Throws<ArgumentException>(
                () =>
                collection.Add(new KeyedItem<string, int>("fOo", 0)));
            Assert.Throws<ArgumentException>(
                () =>
                collection.Add(new KeyedItem<string, int>("baR", 0)));
        }

        [Fact]
        public void ThresholdThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                new TestKeyedCollectionOfIKeyedItem<string, int>(-2));
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                new TestKeyedCollectionOfIKeyedItem<string, int>(
                    int.MinValue));
        }
    }

    public class IListTestKeyedCollectionIntInt :
        IListTestKeyedCollection<int, int>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return "foo";
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override int CreateItem()
        {
            return s_item++;
        }

        protected override int GetKeyForItem(int item)
        {
            return item;
        }
    }

    public class IListTestKeyedCollectionStringString :
        IListTestKeyedCollection<string, string>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return 5;
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override string CreateItem()
        {
            return (s_item++).ToString();
        }

        protected override string GetKeyForItem(string item)
        {
            return item;
        }
    }

    public class IListTestKeyedCollectionIntString :
        IListTestKeyedCollection<int, string>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return 5;
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override string CreateItem()
        {
            return (s_item++).ToString();
        }

        protected override int GetKeyForItem(string item)
        {
            return item == null ? 0 : int.Parse(item);
        }
    }

    public class IListTestKeyedCollectionStringInt :
        IListTestKeyedCollection<string, int>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return "bar";
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override int CreateItem()
        {
            return s_item++;
        }

        protected override string GetKeyForItem(int item)
        {
            return item.ToString();
        }
    }

    public class IListTestKeyedCollectionIntIntBadKey :
        IListTestKeyedCollectionBadKey<int, int>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return "foo";
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override int CreateItem()
        {
            return s_item++;
        }

        protected override int GetKeyForItem(int item)
        {
            return item;
        }
    }

    public class IListTestKeyedCollectionStringStringBadKey :
        IListTestKeyedCollectionBadKey<string, string>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return 5;
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override string CreateItem()
        {
            return (s_item++).ToString();
        }

        protected override string GetKeyForItem(string item)
        {
            return item;
        }
    }

    public class IListTestKeyedCollectionIntStringBadKey :
        IListTestKeyedCollectionBadKey<int, string>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return 5;
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override string CreateItem()
        {
            return (s_item++).ToString();
        }

        protected override int GetKeyForItem(string item)
        {
            return item == null ? 0 : int.Parse(item);
        }
    }

    public class IListTestKeyedCollectionStringIntBadKey :
        IListTestKeyedCollectionBadKey<string, int>
    {
        private static int s_item = 1;

        /// <summary>
        ///     When overridden in a derived class, Gets a list of values that are not valid elements in the collection under test.
        /// </summary>
        /// <returns>An <see cref="IEnumerable" /> containing the invalid values.</returns>
        protected override IEnumerable GetInvalidValues()
        {
            yield return (long) 5;
            yield return "bar";
            yield return new object();
        }

        /// <summary>
        ///     When overridden in a derived class, Gets a new, unique item.
        /// </summary>
        /// <returns>The new item.</returns>
        protected override int CreateItem()
        {
            return s_item++;
        }

        protected override string GetKeyForItem(int item)
        {
            return item.ToString();
        }
    }

    public class KeyedCollectionTestsIntInt :
        KeyedCollectionTests<int, int>
    {
        private int _curValue = 1;

        public override int GetKeyForItem(int item)
        {
            return item;
        }

        public override int GenerateValue()
        {
            return _curValue++;
        }
    }

    public class KeyedCollectionTestsStringString :
        KeyedCollectionTests<string, string>
    {
        private int _curValue = 1;

        public override string GetKeyForItem(string item)
        {
            return item;
        }

        public override string GenerateValue()
        {
            return (_curValue++).ToString();
        }
    }

    public class KeyedCollectionTestsIntString :
        KeyedCollectionTests<int, string>
    {
        private int _curValue = 1;

        public override int GetKeyForItem(string item)
        {
            return int.Parse(item);
        }

        public override string GenerateValue()
        {
            return (_curValue++).ToString();
        }
    }

    public class KeyedCollectionTestsStringInt :
        KeyedCollectionTests<string, int>
    {
        private int _curValue = 1;

        public override string GetKeyForItem(int item)
        {
            return item.ToString();
        }

        public override int GenerateValue()
        {
            return _curValue++;
        }
    }
}
