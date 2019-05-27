using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xunit;

namespace System.Text.Json.Tests.Imported
{
    public class IncompleteTestClass
    {
        public int Key { get; set; }
    }

   public class JsonSerializerTests
    { 
        [Fact]
        public void DeserializeBoolean_Null()
        {
            Assert.Throws<ArgumentNullException>(
                () => JsonSerializer.Parse<IList<bool>>(@"[null]"));
        }

        [Fact]
        public void DeserializeBoolean_DateTime()
        {
            Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IList<bool>>(@"['2000-12-20T10:55:55Z']"));
        }

        [Fact]
        public void DeserializeBoolean_BadString()
        {
            Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IList<bool>>(@"['pie']"));
        }

        [Fact]
        public void DeserializeBoolean_EmptyString()
        {
            Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IList<bool>>(@"['']"));
        }

        [Fact]
        public void IncompleteContainers()
        {
            JsonException e = Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IList<object>>("[1,"));
            Assert.Equal(e.Message, "Expected start of a property name or value, but instead reached end of data. Path: [System.Collections.Generic.IList`1[[System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]] | LineNumber: 0 | BytePositionInLine: 2.");
            
            e = Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IList<int>>("[1,"));
            Assert.Equal(e.Message, "Expected start of a property name or value, but instead reached end of data. Path: [System.Collections.Generic.IList`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]] | LineNumber: 0 | BytePositionInLine: 2.");

            e = Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IList<int>>("[1"));
            // Assert.Equal(e.Message, "Appropriate message to show");

            e = Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IDictionary<string, int>>("{\"key\":1,"));
            Assert.Equal(e.Message, "Expected start of a property name or value, but instead reached end of data. Path: [System.Collections.Generic.IDictionary`2[[System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]] | LineNumber: 0 | BytePositionInLine: 8.");

            e = Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IDictionary<string, int>>("{\"key\":1"));
            // Assert.Equal(e.Message, "Appropriate message to show");

            e = Assert.Throws<JsonException>(
                () => JsonSerializer.Parse<IncompleteTestClass>("{\"key\":1,"));
            Assert.Equal(e.Message, "Expected start of a property name or value, but instead reached end of data. Path: [System.Text.Json.Tests.Imported.IncompleteTestClass] | LineNumber: 0 | BytePositionInLine: 8.");
        }

        [Fact]
        public void NewProperty()
        {
            Assert.Equal(@"{""IsTransient"":true}", JsonSerializer.ToString(new ChildClass { IsTransient = true }));

            var childClass = JsonSerializer.Parse<ChildClass>(@"{""IsTransient"":true}");
            Assert.Equal(true, childClass.IsTransient);
        }

        [Fact]
        public void NewPropertyVirtual()
        {
            Assert.Equal(@"{""IsTransient"":true}", JsonSerializer.ToString(new ChildClassVirtual { IsTransient = true }));

            var childClass = JsonSerializer.Parse<ChildClassVirtual>(@"{""IsTransient"":true}");
            Assert.Equal(true, childClass.IsTransient);
        }

        [Fact]
        public void DeserializeCommentTestObjectWithComments()
        {
            CommentTestObject o = JsonSerializer.Parse<CommentTestObject>(@"{/* Test */}", new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
            Assert.Equal(false, o.A);

            o = JsonSerializer.Parse<CommentTestObject>(@"{""A"": true/* Test */}", new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip });
            Assert.Equal(true, o.A);
        }

        [Fact]
        public void PreserveReferencesCallbackTest()
        {
            var p1 = new PersonReference
            {
                Name = "John Smith"
            };
            var p2 = new PersonReference
            {
                Name = "Mary Sue",
            };

            p1.Spouse = p2;
            p2.Spouse = p1;
            Assert.Throws<InvalidOperationException> (() => JsonSerializer.ToString(p1));
        }
    }

    public class PersonReference
    {
        internal Guid Id { get; set; }
        public string Name { get; set; }
        public PersonReference Spouse { get; set; }
    }

    public class CommentTestObject
    {
        public bool A { get; set; }
    }

    public class ChildClassVirtual : BaseClassVirtual
    {
        public new virtual bool IsTransient { get; set; }
    }

    public class BaseClassVirtual
    {
        internal virtual bool IsTransient { get; set; }
    }

    public class BaseClass
    {
        internal bool IsTransient { get; set; }
    }

    public class ChildClass : BaseClass
    {
        public new bool IsTransient { get; set; }
    }
}
