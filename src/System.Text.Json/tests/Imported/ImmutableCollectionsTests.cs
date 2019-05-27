using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json.Serialization;
using Xunit;

namespace System.Text.Json.Tests.Imported
{
    public class ImmutableCollectionsTests
    {
        private JsonSerializerOptions indentedOption = new JsonSerializerOptions { WriteIndented = true }; 

        #region List
        [Fact]
        public void SerializeList()
        {
            ImmutableList<string> l = ImmutableList.CreateRange(new List<string>
            {
                "One",
                "II",
                "3"
            });

            string json = JsonSerializer.ToString(l);
            Assert.Equal(@"[""One"",""II"",""3""]", json);
        }

        [Fact]
        public void DeserializeList()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            ImmutableList<string> l = JsonSerializer.Parse<ImmutableList<string>>(json);

            Assert.Equal(3, l.Count);
            Assert.Equal("One", l[0]);
            Assert.Equal("II", l[1]);
            Assert.Equal("3", l[2]);
        }

        [Fact]
        public void DeserializeListInterface()
        {
            string json = @"[
        ""Volibear"",
        ""Teemo"",
        ""Katarina""
      ]";

            // what sorcery is this?!
            IImmutableList<string> champions = JsonSerializer.Parse<IImmutableList<string>>(json);

            Assert.Equal(3, champions.Count);
            Assert.Equal("Volibear", champions[0]);
            Assert.Equal("Teemo", champions[1]);
            Assert.Equal("Katarina", champions[2]);
        }
        #endregion

        #region Array
/*        [Fact]
        public void SerializeArray()
        {
            ImmutableArray<string> l = ImmutableArray.CreateRange(new List<string>
            {
                "One",
                "II",
                "3"
            });

            string json = JsonSerializer.ToString(l, indentedOption);
            Assert.Equal(@"[
  ""One"",
  ""II"",
  ""3""
]", json);
        }

        [Fact]
        public void DeserializeArray()
        {
            string json = @"[
          ""One"",
          ""II"",
          ""3""
        ]";

            ImmutableArray<string> l = JsonSerializer.Parse<ImmutableArray<string>>(json);

            Assert.Equal(3, l.Length);
            Assert.Equal("One", l[0]);
            Assert.Equal("II", l[1]);
            Assert.Equal("3", l[2]);
        }

        [Fact]
        public void SerializeDefaultArray()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(
                () => JsonSerializer.ToString(default(ImmutableArray<int>), indentedOption));
            Assert.Equal(e.Message, "This operation cannot be performed on a default instance of ImmutableArray<T>.  Consider initializing the array, or checking the ImmutableArray<T>.IsDefault property.");
        }*/
        #endregion

        #region Queue
        [Fact]
        public void SerializeQueue()
        {
            ImmutableQueue<string> l = ImmutableQueue.CreateRange(new List<string>
            {
                "One",
                "II",
                "3"
            });

            string json = JsonSerializer.ToString(l);
            Assert.Equal(@"[""One"",""II"",""3""]", json);
        }

        [Fact]
        public void DeserializeQueue()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            ImmutableQueue<string> l = JsonSerializer.Parse<ImmutableQueue<string>>(json);

            Assert.False(l.IsEmpty);
            Assert.Equal("One", l.Peek());
            l = l.Dequeue();
            Assert.Equal("II", l.Peek());
            l = l.Dequeue();
            Assert.Equal("3", l.Peek());
        }

        [Fact]
        public void DeserializeQueueInterface()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            IImmutableQueue<string> l = JsonSerializer.Parse<IImmutableQueue<string>>(json);

            Assert.False(l.IsEmpty);
            Assert.Equal("One", l.Peek());
            l = l.Dequeue();
            Assert.Equal("II", l.Peek());
            l = l.Dequeue();
            Assert.Equal("3", l.Peek());
        }
        #endregion

        #region Stack
        [Fact]
        public void SerializeStack()
        {
            ImmutableStack<string> l = ImmutableStack.CreateRange(new List<string>
            {
                "One",
                "II",
                "3"
            });

            string json = JsonSerializer.ToString(l);
            Assert.Equal(@"[""3"",""II"",""One""]", json);
        }

        [Fact]
        public void DeserializeStack()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            ImmutableStack<string> l = JsonSerializer.Parse<ImmutableStack<string>>(json);

            Assert.False(l.IsEmpty);
            Assert.Equal("3", l.Peek());
            l = l.Pop();
            Assert.Equal("II", l.Peek());
            l = l.Pop();
            Assert.Equal("One", l.Peek());
        }

        [Fact]
        public void DeserializeStackInterface()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            IImmutableStack<string> l = JsonSerializer.Parse<IImmutableStack<string>>(json);

            Assert.False(l.IsEmpty);
            Assert.Equal("3", l.Peek());
            l = l.Pop();
            Assert.Equal("II", l.Peek());
            l = l.Pop();
            Assert.Equal("One", l.Peek());
        }
        #endregion

        #region HashSet
        [Fact]
        public void SerializeHashSet()
        {
            ImmutableHashSet<string> l = ImmutableHashSet.CreateRange(new List<string>
            {
                "One",
                "II",
                "3"
            });

            string json = JsonSerializer.ToString(l, indentedOption);

            ImmutableHashSet<string> a = JsonSerializer.Parse<ImmutableHashSet<string>>(json);
            Assert.Equal(3, a.Count);
            Assert.True(a.Contains("One"));
            Assert.True(a.Contains("II"));
            Assert.True(a.Contains("3"));
        }

        [Fact]
        public void DeserializeHashSet()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            ImmutableHashSet<string> l = JsonSerializer.Parse<ImmutableHashSet<string>>(json);

            Assert.Equal(3, l.Count);
            Assert.True(l.Contains("3"));
            Assert.True(l.Contains("II"));
            Assert.True(l.Contains("One"));
        }

        [Fact]
        public void DeserializeHashSetInterface()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            IImmutableSet<string> l = JsonSerializer.Parse<IImmutableSet<string>>(json);

            Assert.Equal(3, l.Count);
            Assert.True(l.Contains("3"));
            Assert.True(l.Contains("II"));
            Assert.True(l.Contains("One"));
        }
        #endregion

        #region SortedSet
        [Fact]
        public void SerializeSortedSet()
        {
            ImmutableSortedSet<string> l = ImmutableSortedSet.CreateRange(new List<string>
            {
                "One",
                "II",
                "3"
            });

            string json = JsonSerializer.ToString(l);
            Assert.Equal(@"[""3"",""II"",""One""]", json);
        }

        [Fact]
        public void DeserializeSortedSet()
        {
            string json = @"[
  ""One"",
  ""II"",
  ""3""
]";

            ImmutableSortedSet<string> l = JsonSerializer.Parse<ImmutableSortedSet<string>>(json);

            Assert.Equal(3, l.Count);
            Assert.True(l.Contains("3"));
            Assert.True(l.Contains("II"));
            Assert.True(l.Contains("One"));
        }
        #endregion

        #region Dictionary
/*        [Fact]
        public void SerializeDictionary()
        {
            ImmutableDictionary<int, string> l = ImmutableDictionary.CreateRange(new Dictionary<int, string>
            {
                { 1, "One" },
                { 2, "II" },
                { 3, "3" }
            });

            string json = JsonSerializer.ToString(l, indentedOption);
            ImmutableDictionary<int, string> a = JsonSerializer.Parse<ImmutableDictionary<int, string>>(json);
            Assert.Equal(3, a.Count);
            Assert.Equal("One", (string)a[1]);
            Assert.Equal("II", (string)a[2]);
            Assert.Equal("3", (string)a[3]);
        }

        [Fact]
        public void DeserializeDictionary()
        {
            string json = @"{
  ""1"": ""One"",
  ""2"": ""II"",
  ""3"": ""3""
}";

            ImmutableDictionary<int, string> l = JsonSerializer.Parse<ImmutableDictionary<int, string>>(json);

            Assert.Equal(3, l.Count);
            Assert.Equal("One", l[1]);
            Assert.Equal("II", l[2]);
            Assert.Equal("3", l[3]);
        }

        [Fact]
        public void DeserializeDictionaryInterface()
        {
            string json = @"{
  ""1"": ""One"",
  ""2"": ""II"",
  ""3"": ""3""
}";

            IImmutableDictionary<int, string> l = JsonSerializer.Parse<IImmutableDictionary<int, string>>(json);

            Assert.Equal(3, l.Count);
            Assert.Equal("One", l[1]);
            Assert.Equal("II", l[2]);
            Assert.Equal("3", l[3]);
        }
        #endregion

        #region SortedDictionary
        [Fact]
        public void SerializeSortedDictionary()
        {
            ImmutableSortedDictionary<int, string> l = ImmutableSortedDictionary.CreateRange(new SortedDictionary<int, string>
            {
                { 1, "One" },
                { 2, "II" },
                { 3, "3" }
            });

            string json = JsonSerializer.ToString(l, indentedOption);
            Assert.Equal(@"{
  ""1"": ""One"",
  ""2"": ""II"",
  ""3"": ""3""
}", json);
        }

        [Fact]
        public void DeserializeSortedDictionary()
        {
            string json = @"{
  ""1"": ""One"",
  ""2"": ""II"",
  ""3"": ""3""
}";

            ImmutableSortedDictionary<int, string> l = JsonSerializer.Parse<ImmutableSortedDictionary<int, string>>(json);

            Assert.Equal(3, l.Count);
            Assert.Equal("One", l[1]);
            Assert.Equal("II", l[2]);
            Assert.Equal("3", l[3]);
        }*/
        #endregion
    }
}
