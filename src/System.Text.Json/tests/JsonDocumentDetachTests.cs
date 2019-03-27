// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonDocumentDetachTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void DocumentDetachmentLifetime(bool useArrayPools)
        {
            byte[] json = JsonTestHelper.TrueValue.ToArray();

            using (JsonDocument original = JsonDocument.Parse(json))
            using (JsonDocument clone = original.Detach(useArrayPools))
            {
                Assert.NotSame(original, clone);

                JsonElement originalRoot = original.RootElement;
                JsonElement cloneRoot = clone.RootElement;

                Assert.Equal(JsonValueType.True, cloneRoot.Type);
                Assert.Equal(cloneRoot.Type, originalRoot.Type);
                Assert.Equal(originalRoot.GetRawText(), cloneRoot.GetRawText());

                JsonTestHelper.NullValue.CopyTo(json);

                Assert.Equal("null", originalRoot.GetRawText());
                Assert.Equal("true", cloneRoot.GetRawText());

                Assert.False(original.IsDetached, "original.IsDetached");
                Assert.True(clone.IsDetached, "clone.IsDetached");

                Assert.True(original.IsDisposable, "original.IsDisposable");

                if (useArrayPools)
                {
                    Assert.True(clone.IsDisposable, "clone.IsDisposable");

                    clone.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => cloneRoot.GetRawText());
                }
                else
                {
                    Assert.False(clone.IsDisposable, "clone.IsDisposable");

                    clone.Dispose();
                    Assert.Equal("true", cloneRoot.GetRawText());
                }
            }
        }

        [Fact]
        public static void DocumentDetachOnDetached()
        {
            byte[] json = JsonTestHelper.TrueValue.ToArray();

            using (JsonDocument original = JsonDocument.Parse(json))
            using (JsonDocument detachGC = original.Detach())
            using (JsonDocument detachGC2 = original.Detach())
            using (JsonDocument detachPool = original.Detach(poolArrays: true))
            using (JsonDocument detachGCGC = detachGC.Detach())
            using (JsonDocument detachGCPool = detachGC.Detach(poolArrays: true))
            using (JsonDocument detachPoolGC = detachPool.Detach())
            using (JsonDocument detachPoolPool = detachPool.Detach(poolArrays: true))
            {
                Assert.False(original.IsDetached, "original.IsDetached");
                Assert.True(original.IsDisposable, "original.IsDisposable");

                // detachGC is a copy of original using new GC memory
                // Detached=true, Disposable=false
                Assert.NotSame(original, detachGC);
                Assert.True(detachGC.IsDetached, "detachGC.IsDetached");
                Assert.False(detachGC.IsDisposable, "detachGC.IsDisposable");

                // detachGC2 is a second call to Detach(false) on the original
                // There's no cached copy, so it's not reference-equal to detachGC.
                Assert.NotSame(original, detachGC2);
                Assert.NotSame(detachGC, detachGC2);
                Assert.True(detachGC2.IsDetached, "detachGC2.IsDetached");
                Assert.False(detachGC2.IsDisposable, "detachGC2.IsDisposable");

                // detachPool is a copy of original using pooled memory
                // Detached=true, Disposable=true
                Assert.NotSame(original, detachPool);
                Assert.NotSame(detachGC, detachPool);
                Assert.NotSame(detachGC2, detachPool);
                Assert.True(detachPool.IsDetached, "detachPool.IsDetached");
                Assert.True(detachPool.IsDisposable, "detachPool.IsDisposable");

                // detachGCGC is a Detach(false) off of detachGC.
                // since detachGC is already in the most conservative state, the
                // target object is returned.
                Assert.Same(detachGC, detachGCGC);

                // detachGCPool is a Detach(true) off of detachGC.
                // since detachGC is already in the most conservative state, the
                // target object is returned.  (Using pooled memory at this point is
                // more expensive than continuing to use the allocated GC memory)
                Assert.Same(detachGC, detachGCPool);

                // detachPoolGC is a Detach(false) off of detachPool.
                // Detached=true, Disposable=false
                Assert.NotSame(detachPool, detachPoolGC);
                Assert.True(detachPoolGC.IsDetached, "detachPoolGC.IsDetached");
                Assert.False(detachPoolGC.IsDisposable, "detachPoolGC.IsDisposable");

                // detachPoolPool is a Detach(true) off of detachPool.
                // Detached=true, Disposable=true
                Assert.NotSame(detachPool, detachPoolPool);
                Assert.True(detachPoolPool.IsDetached, "detachPoolPool.IsDetached");
                Assert.True(detachPoolPool.IsDisposable, "detachPoolPool.IsDisposable");

                // Since all the objects are independent, disposing detachPool should not
                // affect its "children"
                detachPool.Dispose();
                string originalJson = original.RootElement.GetRawText();
                Assert.Equal(originalJson, detachPoolGC.RootElement.GetRawText());
                Assert.Equal(originalJson, detachPoolPool.RootElement.GetRawText());
            }
        }

        [Fact]
        public static void DetachRootElementFromLiveDocument()
        {
            string json = "[[]]";
            JsonElement detached;

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;
                detached = root.Detach();

                Assert.False(root.IsDetached, "root.IsDetached");
                Assert.True(detached.IsDetached, "detached.IsDetached");

                Assert.Equal(json, detached.GetRawText());
                Assert.NotSame(doc, detached.SniffDocument());
            }

            // After document Dispose
            Assert.Equal(json, detached.GetRawText());
        }

        [Fact]
        public static void DetachRootElementFromIsolatedDocument()
        {
            string json = "[[]]";
            JsonElement detached;

            using (JsonDocument doc = JsonDocument.Parse(json))
            using (JsonDocument isolated = doc.Detach())
            {
                JsonElement root = isolated.RootElement;
                detached = root.Detach();

                Assert.True(root.IsDetached, "root.IsDetached");
                Assert.True(detached.IsDetached, "detached.IsDetached");

                Assert.Equal(json, detached.GetRawText());
                Assert.Same(isolated, detached.SniffDocument());
            }

            // After document Dispose
            Assert.Equal(json, detached.GetRawText());
        }

        [Fact]
        public static void DetachInnerElementFromLiveDocument()
        {
            JsonElement detached;

            using (JsonDocument doc = JsonDocument.Parse("[[]]"))
            {
                JsonElement inner = doc.RootElement[0];
                detached = inner.Detach();

                Assert.False(inner.IsDetached, "inner.IsDetached");
                Assert.True(detached.IsDetached, "detached.IsDetached");

                Assert.Equal(inner.GetRawText(), detached.GetRawText());
                Assert.NotSame(doc, detached.SniffDocument());
            }

            // After document Dispose
            Assert.Equal("[]", detached.GetRawText());
        }

        [Fact]
        public static void DetachInnerElementFromIsolatedDocument()
        {
            JsonElement detached;

            using (JsonDocument doc = JsonDocument.Parse("[[]]"))
            using (JsonDocument isolated = doc.Detach())
            {
                JsonElement inner = isolated.RootElement[0];
                detached = inner.Detach();

                Assert.True(inner.IsDetached, "inner.IsDetached");
                Assert.True(detached.IsDetached, "detached.IsDetached");

                Assert.Equal(inner.GetRawText(), detached.GetRawText());
                Assert.NotSame(doc, detached.SniffDocument());
            }

            // After document Dispose
            Assert.Equal("[]", detached.GetRawText());
        }

        [Fact]
        public static void DetachInnerElementFromDetachedElement()
        {
            JsonElement detached;

            using (JsonDocument doc = JsonDocument.Parse("[[[]]]"))
            {
                JsonElement middle = doc.RootElement[0].Detach();
                JsonElement inner = middle[0];
                detached = inner.Detach();

                Assert.Equal(inner.GetRawText(), detached.GetRawText());
                Assert.NotSame(doc, detached.SniffDocument());
                Assert.NotSame(middle.SniffDocument(), detached.SniffDocument());
                Assert.NotSame(inner.SniffDocument(), detached.SniffDocument());
            }

            // After document Dispose
            Assert.Equal("[]", detached.GetRawText());
        }

        [Fact]
        public static void DetachAtInnerNumber()
        {
            DetachAtInner("1.21e9", JsonValueType.Number);
        }

        [Fact]
        public static void DetachAtInnerString()
        {
            DetachAtInner("\"  this  string  has  \\u0039 spaces\"", JsonValueType.String);
        }

        [Fact]
        public static void DetachAtInnerTrue()
        {
            DetachAtInner("true", JsonValueType.True);
        }

        [Fact]
        public static void DetachAtInnerFalse()
        {
            DetachAtInner("false", JsonValueType.False);
        }

        [Fact]
        public static void DetachAtInnerNull()
        {
            DetachAtInner("null", JsonValueType.Null);
        }

        [Fact]
        public static void DetachAtInnerObject()
        {
            DetachAtInner(
                @"{
  ""this"":
  [
    {
      ""object"": 0,




      ""has"": [ ""whitespace"" ]
    }
  ]
}",
                JsonValueType.Object);
        }

        [Fact]
        public static void DetachAtInnerArray()
        {
            DetachAtInner(
                @"[
{
  ""this"":
  [
    {
      ""object"": 0,




      ""has"": [ ""whitespace"" ]
    }
  ]
},

5

,



false,



null
]",
                JsonValueType.Array);
        }

        private static void DetachAtInner(string innerJson, JsonValueType valueType)
        {
            string json = $"{{ \"obj\": [ {{ \"not target\": true, \"target\": {innerJson} }}, 5 ] }}";

            JsonElement detached;

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement target = doc.RootElement.GetProperty("obj")[0].GetProperty("target");
                Assert.Equal(valueType, target.Type);
                Assert.False(target.IsDetached, "target.IsDetached");
                detached = target.Detach();
                Assert.True(detached.IsDetached, "detached.IsDetached");
            }

            Assert.Equal(innerJson, detached.GetRawText());
        }

        private static JsonDocument SniffDocument(this JsonElement element)
        {
            return (JsonDocument)typeof(JsonElement).
                GetField("_parent", BindingFlags.Instance|BindingFlags.NonPublic).
                GetValue(element);
        }
    }
}
