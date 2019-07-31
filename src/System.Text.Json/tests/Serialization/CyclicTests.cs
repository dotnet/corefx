// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class CyclicTests
    {
        [Fact]
        public static void WriteCyclicFailDefault()
        {
            TestClassWithCycle obj = new TestClassWithCycle();
            obj.Parent = obj;

            // We don't allow graph cycles; we throw JsonException instead of an unrecoverable StackOverflow.
            Assert.Throws<JsonException>(() => JsonSerializer.Serialize(obj));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(70)]
        public static void WriteCyclicFail(int depth)
        {
            var rootObj = new TestClassWithCycle("root");
            CreateObjectHierarchy(1, depth, rootObj);

            {
                var options = new JsonSerializerOptions();
                options.MaxDepth = depth + 1;

                // No exception since depth was not passed.
                string json = JsonSerializer.Serialize(rootObj, options);
                Assert.False(string.IsNullOrEmpty(json));
            }

            {
                var options = new JsonSerializerOptions();
                options.MaxDepth = depth;
                Assert.Throws<JsonException>(() => JsonSerializer.Serialize(rootObj, options));
            }
        }

        private static TestClassWithCycle CreateObjectHierarchy(int i, int max, TestClassWithCycle previous)
        {
            if (i == max)
            {
                return null;
            }

            var obj = new TestClassWithCycle(i.ToString());
            previous.Parent = obj;
            return CreateObjectHierarchy(++i, max, obj);
        }

        [Fact]
        public static void SimpleTypeCycle()
        {
            TestClassWithArrayOfElementsOfTheSameClass obj = new TestClassWithArrayOfElementsOfTheSameClass();

            // A cycle in just Types (not data) is allowed.
            string json = JsonSerializer.Serialize(obj);
            Assert.Equal(@"{""Array"":null}", json);
        }

        [Fact]
        public static void DeepTypeCycleWithRoundTrip()
        {
            TestClassWithCycle root = new TestClassWithCycle("root");
            TestClassWithCycle parent = new TestClassWithCycle("parent");
            root.Parent = parent;
            root.Children.Add(new TestClassWithCycle("child1"));
            root.Children.Add(new TestClassWithCycle("child2"));

            // A cycle in just Types (not data) is allowed.
            string json = JsonSerializer.Serialize(root);

            // Round-trip the JSON.
            TestClassWithCycle rootCopy = JsonSerializer.Deserialize<TestClassWithCycle>(json);
            Assert.Equal("root", rootCopy.Name);
            Assert.Equal(2, rootCopy.Children.Count);

            Assert.Equal("parent", rootCopy.Parent.Name);
            Assert.Equal(0, rootCopy.Parent.Children.Count);
            Assert.Null(rootCopy.Parent.Parent);

            Assert.Equal("child1", rootCopy.Children[0].Name);
            Assert.Equal(0, rootCopy.Children[0].Children.Count);
            Assert.Null(rootCopy.Children[0].Parent);

            Assert.Equal("child2", rootCopy.Children[1].Name);
            Assert.Equal(0, rootCopy.Children[1].Children.Count);
            Assert.Null(rootCopy.Children[1].Parent);
        }

        public class TestClassWithCycle
        {
            public TestClassWithCycle() { }

            public TestClassWithCycle(string name)
            {
                Name = name;
            }

            public TestClassWithCycle Parent { get; set; }
            public List<TestClassWithCycle> Children { get; set; } = new List<TestClassWithCycle>();
            public string Name { get; set; }
        }

        public class TestClassWithArrayOfElementsOfTheSameClass
        {
            public TestClassWithArrayOfElementsOfTheSameClass[] Array { get; set; }
        }

        public class CycleRoot
        {
            public Child1 Child1 { get; set; }
        }

        public class Child1
        {
            public IList<Child2> Child2List { get; set; } = new List<Child2>();
            public Child2 Child2 { get; set; }
        }

        public class Child2
        {
            public Child1 Child1 { get; set; }
            public IList<Child1> Child1List { get; set; } = new List<Child1>();
        }

        [Fact]
        public static void MultiClassCycle()
        {
            CycleRoot root = new CycleRoot();
            root.Child1 = new Child1();
            root.Child1.Child2List.Add(new Child2());
            root.Child1.Child2 = new Child2();
            root.Child1.Child2.Child1 = new Child1();

            // A cycle in just Types (not data) is allowed.
            string json = JsonSerializer.Serialize(root);

            root = JsonSerializer.Deserialize<CycleRoot>(json);
            Assert.NotNull(root.Child1);
            Assert.NotNull(root.Child1.Child2List[0]);
            Assert.NotNull(root.Child1.Child2);
            Assert.NotNull(root.Child1.Child2.Child1);

            // Round-trip
            string jsonRoundTrip = JsonSerializer.Serialize(root);
            Assert.Equal(json, jsonRoundTrip);
        }









                public sealed class RootObject
                {
                    public string A { get; set; }
                    public long B { get; set; }
                    public bool C { get; set; }
                    public ModelA D { get; set; } = new ModelA();
                    public ModelE E { get; set; } = new ModelE();
                    public ModelF F { get; set; }
                    public ModelG G { get; set; }
                    public ModelH H { get; set; } = new ModelH();
                    public ModelI I { get; set; } = new ModelI();
                    public string J { get; set; }
                    public bool K { get; set; }
                    public bool L { get; set; }
                    public bool M { get; set; }
                    public ModelN N { get; set; }
                    public ModelO O { get; set; } = new ModelO();
                    public ModelP P { get; set; }
                }

                public class ModelA
                {
                    public string A { get; set; }
                    public ModelZ B { get; set; }
                    public DateTimeOffset C { get; set; }
                    public DateTimeOffset D { get; set; }
                    public DateTimeOffset E { get; set; }
                    public bool F { get; set; }
                }

                public class ModelB
                {
                    public string A { get; set; }
                    public string B { get; set; }
                    public string C { get; set; }
                    public double? D { get; set; }
                    public double? E { get; set; }
                }

                public sealed class ModelC
                {
                    public string A { get; set; }
                    public string B { get; set; }
                }

                public sealed class ModelD
                {
                    public long A { get; set; }
                    public int B { get; set; }
                    public string C { get; set; }
                    public string D { get; set; }
                }

                public class ModelE
                {
                    public int A { get; set; }
                    public string B { get; set; }
                    public string C { get; set; }
                    public ModelB D { get; set; }
                    public string E { get; set; }
                    public string F { get; set; }
                    public string G { get; set; }
                    public string H { get; set; }
                    public bool I { get; set; }
                    public bool J { get; set; }
                    public ModelX K { get; set; }
                    public bool L { get; set; }
                    public bool M { get; set; }
                    public bool N { get; set; }
                    public bool O { get; set; }
                    public DateTime? P { get; set; }
                    public DateTime? Q { get; set; }
                    public bool R { get; set; }
                    public bool S { get; set; }
                    public IList<ModelC> T { get; set; }
                    public IList<ModelD> U { get; set; }
                }

                public class ModelF
                {
                    public string A { get; set; }
                    public IList<ModelJ> B { get; set; } = new List<ModelJ>();
                    public decimal C { get; set; }
                    public decimal D { get; set; }
                    public DateTimeOffset? E { get; set; }
                    public string F { get; set; }
                }

                public sealed class ModelG
                {
                    public string A { get; set; }
                    public string B { get; set; }
                    public string C { get; set; }
                    public string D { get; set; }
                    public string E { get; set; }
                    public string F { get; set; }
                    public string G { get; set; }
                    public string H { get; set; }
                    public string I { get; set; }
                    public string J { get; set; }
                    public ModelK K { get; set; }
                }

                public sealed class ModelH
                {
                    public string A { get; set; }
                    public int B { get; set; }
                    public IList<ModelL> C { get; set; }
                    public IList<ModelU> D { get; set; }
                    public decimal E { get; set; }
                    public decimal F { get; set; }
                    public decimal G { get; set; }
                    public decimal H { get; set; }
                    public decimal I { get; set; }
                    public decimal J { get; set; }
                    public IList<ModelV> K { get; set; }
                    public ModelW L { get; set; }
                }

                public class ModelI
                {
                    public string A { get; set; }
                    public string B { get; set; }
                }

                public sealed class ModelJ
                {
                    public ModelY A { get; set; }
                    public decimal B { get; set; }
                    public string C { get; set; }
                    public decimal D { get; set; }
                    public string E { get; set; }
                    public string F { get; set; }
                    public string G { get; set; }
                    public string H { get; set; }
                }

                public sealed class ModelK
                {
                    public string A { get; set; }
                    public double B { get; set; }
                    public double C { get; set; }
                    public double D { get; set; }
                    public string E { get; set; }
                }

                public sealed class ModelL
                {
                    public int A { get; set; }
                    public int B { get; set; }
                    public string C { get; set; }
                    public string D { get; set; }
                    public string E { get; set; }
                    public string F { get; set; }
                    public string G { get; set; }
                    public decimal H { get; set; }
                    public decimal I { get; set; }
                    public IList<ModelM> J { get; set; }
                    public IList<ModelQ> K { get; set; }
                    public IList<ModelR> L { get; set; }
                    public IList<ModelS> M { get; set; }
                    public IList<ModelH> N { get; set; }
                }

                public sealed class ModelM
                {
                    public int A { get; set; }
                    public int B { get; set; }
                    public string C { get; set; }
                    public string D { get; set; }
                    public ICollection<ModelQ> E { get; set; } = new List<ModelQ>();
                    public ICollection<ModelR> F { get; set; } = new List<ModelR>();
                }

                public sealed class ModelN
                {
                    public int A { get; set; }
                    public int B { get; set; }
                    public int C { get; set; }
                    public double D { get; set; }
                    public string E { get; set; }
                }

                public sealed class ModelO
                {
                    public string A { get; set; }
                    public int B { get; set; }
                }

                public sealed class ModelP
                {
                    public string A { get; set; }
                    public string B { get; set; }
                    public string C { get; set; }
                    public string D { get; set; }
                    public string E { get; set; }
                }

                public sealed class ModelQ
                {
                    public int A { get; set; }
                    public int B { get; set; }
                    public decimal C { get; set; }
                    public string D { get; set; }
                }

                public sealed class ModelR
                {
                    public int A { get; set; }
                    public int B { get; set; }
                    public decimal C { get; set; }
                    public string D { get; set; }
                }

                public sealed class ModelS
                {
                    public decimal A { get; set; }
                    public string B { get; set; }
                }

                public sealed class ModelT
                {
                    public decimal A { get; set; }
                    public string B { get; set; }
                    public string C { get; set; }
                    public int D { get; set; }
                }

                public sealed class ModelU
                {
                    public int A { get; set; }
                    public decimal B { get; set; }
                    public string C { get; set; }
                    public decimal D { get; set; }
                }

                public sealed class ModelV
                {
                    public string A { get; set; }
                    public decimal B { get; set; }
                    public decimal C { get; set; }
                }

                public sealed class ModelW
                {
                    public string A { get; set; }
                    public decimal B { get; set; }
                    public decimal C { get; set; }
                }

                public enum ModelX
                {
                    A = 0,
                    B = 1,
                    C = 2,
                    D = 3,
                    E = 4,
                    F = 5,
                    G = 6,
                    H = 8,
                    I = 9,
                    J = 10,
                    K = 11,
                    L = 12,
                }

                public enum ModelY
                {
                    A = 1,
                    B = 2,
                    C = 3,
                    D = 4,
                    E = 5,
                    F = 6,
                    G = 7,
                }

                public enum ModelZ
                {
                    A = 1,
                    B = 2,
                }

                public sealed class Parent
                {
                    public string A { get; set; }
                    public int B { get; set; }
                    public int C { get; set; }
                    public string D { get; set; }
                    public string E { get; set; }
                    public DateTime F { get; set; }
                    public int G { get; set; }
                    public Child H { get; set; }
                }

                public sealed class Child
                {
                    public double A { get; set; }
                    public string B { get; set; }
                    public int C { get; set; }
                    public int D { get; set; }
                    public string E { get; set; }
                    public DateTime F { get; set; }
                    public int G { get; set; }
                }





























                    [Fact]
                    public static async Task ReadAsync_Does_Not_Throw()
                    {
                        // Arrange
                        var json = @"{
          ""A"": """",
          ""B"": 1,
          ""C"": false,
          ""D"": {
            ""A"": """",
            ""B"": 2,
            ""C"": ""2019-04-21T08:22:35.8241219+00:00"",
            ""D"": ""2019-04-21T09:07:35.8241219+00:00"",
            ""E"": ""2019-04-21T09:12:35.8241219+00:00"",
            ""F"": false
          },
          ""E"": {
            ""A"": 1,
            ""B"": """",
            ""C"": """",
            ""D"": null,
            ""E"": null,
            ""F"": null,
            ""G"": null,
            ""H"": """",
            ""I"": false,
            ""J"": false,
            ""K"": 0,
            ""L"": true,
            ""M"": true,
            ""N"": false,
            ""O"": false,
            ""P"": null,
            ""Q"": null,
            ""R"": false,
            ""S"": false,
            ""T"": null,
            ""U"": null
          },
          ""F"": {
            ""A"": null,
            ""B"": [],
            ""C"": 0.0,
            ""D"": 0.0,
            ""E"": null,
            ""F"": null
          },
          ""G"": null,
          ""H"":{
            ""A"": """",
            ""B"": 0,
            ""D"": [],
            ""E"": 0.0,
            ""F"": 0.0,
            ""G"": 0.0,
            ""H"": 0.0,
            ""I"": 0.0,
            ""J"": 0.0,
            ""K"": [],
            ""L"": null
          },
          ""I"": {
            ""A"": """",
            ""B"":null
          },
          ""J"": """",
          ""K"": false,
          ""L"": false,
          ""M"": false,
          ""N"": null,
          ""O"": null,
          ""P"": {
            ""A"": """",
            ""B"": """",
            ""C"": """",
            ""D"": """",
            ""E"": """"
          }
        }";

                        var buffer = Encoding.UTF8.GetBytes(json);
                        var utf8Json = new MemoryStream(buffer);
                        var options = new JsonSerializerOptions();

                    // Act
                    //var model = await JsonSerializer.DeserializeAsync<RootObject>(utf8Json, options);
                    var model0 = JsonSerializer.Deserialize<RootObject>("{}", options);
                    var model1 = JsonSerializer.Deserialize<RootObject>(json, options);

                    var model = await JsonSerializer.DeserializeAsync<RootObject>(utf8Json, options);

                    // Assert
                    Assert.NotNull(model);
                    }

                [Fact]
                public static async Task ReadAsync_Does_Not_Throw_2()
                {
                    // Arrange
                    var json = @"[{""A"":"""",""B"":0,""C"":0,""D"":"""",""E"":"""",""F"":""2019-04-21T16:21:08.1495557Z"",""G"":0,""H"":null}]";

                    var buffer = Encoding.UTF8.GetBytes(json);
                    var utf8Json = new MemoryStream(buffer);
                    var options = new JsonSerializerOptions();

                    // Act
                    var model = await JsonSerializer.DeserializeAsync<IList<Parent>>(utf8Json, options);

                    // Assert
                    Assert.NotNull(model);
                }











    }
}
