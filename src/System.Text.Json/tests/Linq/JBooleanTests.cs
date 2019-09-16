// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JBooleanTests
    {
        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonBoolean = new JBoolean();
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestValueCtor()
        {
            var jsonBoolean = new JBoolean(true);
            Assert.True(jsonBoolean.Value);

            jsonBoolean = new JBoolean(false);
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestImplicitOperator()
        {
            JNode jsonNode = true;
            JBoolean jsonBoolean = (JBoolean)jsonNode;
            Assert.True(jsonBoolean.Value);

            jsonNode = false;
            jsonBoolean = (JBoolean)jsonNode;
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestChangingValue()
        {
            var jsonBoolean = new JBoolean();
            jsonBoolean.Value = true;
            Assert.True(jsonBoolean.Value);
            jsonBoolean.Value = false;
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestToString()
        {
            Assert.Equal("true", new JBoolean(true).ToString());
            Assert.Equal("false", new JBoolean(false).ToString());
        }

        [Fact]
        public static void TestEquals()
        {
            var jsonBooleanTrue = new JBoolean(true);
            var jsonBooleanFalse = new JBoolean(false);

            Assert.True(jsonBooleanTrue.Equals(new JBoolean(true)));
            Assert.True(new JBoolean(true).Equals(jsonBooleanTrue));

            Assert.True(jsonBooleanFalse.Equals(new JBoolean(false)));
            Assert.True(new JBoolean(false).Equals(jsonBooleanFalse));
            Assert.True(new JBoolean().Equals(jsonBooleanFalse));
            Assert.False(new JBoolean().Equals(jsonBooleanTrue));
            Assert.False(jsonBooleanFalse.Equals(jsonBooleanTrue));
            Assert.False(jsonBooleanTrue.Equals(jsonBooleanFalse));

            Assert.True(jsonBooleanTrue == new JBoolean(true));
            Assert.True(jsonBooleanTrue != jsonBooleanFalse);

            JNode jsonNodeTrue = new JBoolean(true);
            Assert.True(jsonBooleanTrue.Equals(jsonNodeTrue));

            JNode jsonNodeFalse= new JBoolean(false);
            Assert.True(jsonBooleanFalse.Equals(jsonNodeFalse));

            IEquatable<JBoolean> jsonBooleanIEquatableTrue = jsonBooleanTrue;
            Assert.True(jsonBooleanIEquatableTrue.Equals(jsonBooleanTrue));
            Assert.True(jsonBooleanTrue.Equals(jsonBooleanIEquatableTrue));

            IEquatable<JBoolean> jsonBooleanIEquatableFalse = jsonBooleanFalse;
            Assert.True(jsonBooleanIEquatableFalse.Equals(jsonBooleanFalse));
            Assert.True(jsonBooleanFalse.Equals(jsonBooleanIEquatableFalse));

            Assert.False(jsonBooleanTrue.Equals(null));

            object jsonBooleanCopyTrue = jsonBooleanTrue;
            object jsonBooleanObjectTrue = new JBoolean(true);
            Assert.True(jsonBooleanCopyTrue.Equals(jsonBooleanObjectTrue));
            Assert.True(jsonBooleanCopyTrue.Equals(jsonBooleanObjectTrue));
            Assert.True(jsonBooleanObjectTrue.Equals(jsonBooleanTrue));

            object jsonBooleanCopyFalse = jsonBooleanFalse;
            object jsonBooleanObjectFalse = new JBoolean(false);
            Assert.True(jsonBooleanCopyFalse.Equals(jsonBooleanObjectFalse));
            Assert.True(jsonBooleanCopyFalse.Equals(jsonBooleanFalse));
            Assert.True(jsonBooleanObjectFalse.Equals(jsonBooleanFalse));

            Assert.False(jsonBooleanTrue.Equals(new Exception()));

            JBoolean jsonBooleanNull = null;
            Assert.False(jsonBooleanTrue == jsonBooleanNull);
            Assert.False(jsonBooleanNull == jsonBooleanTrue);

            Assert.True(jsonBooleanTrue != jsonBooleanNull);
            Assert.True(jsonBooleanNull != jsonBooleanTrue);

            JBoolean otherJBooleanNull = null;
            Assert.True(jsonBooleanNull == otherJBooleanNull);
        }

        [Fact]
        public static void TestGetHashCode()
        {
            var jsonBooleanTrue = new JBoolean(true);
            var jsonBooleanFalse = new JBoolean(false);

            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonBooleanTrue.GetHashCode());
            Assert.Equal(jsonBooleanTrue.GetHashCode(), new JBoolean(true).GetHashCode());
            Assert.Equal(jsonBooleanFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());
            Assert.Equal(jsonBooleanFalse.GetHashCode(), new JBoolean(false).GetHashCode());
            Assert.Equal(jsonBooleanFalse.GetHashCode(), new JBoolean().GetHashCode());
            Assert.NotEqual(jsonBooleanTrue.GetHashCode(), new JBoolean().GetHashCode());

            JNode jsonNodeTrue = new JBoolean(true);
            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonNodeTrue.GetHashCode());
            JNode jsonNodeFalse = new JBoolean(false);
            Assert.Equal(jsonBooleanFalse.GetHashCode(), jsonNodeFalse.GetHashCode());

            Assert.NotEqual(jsonBooleanTrue.GetHashCode(), jsonBooleanFalse.GetHashCode());

            IEquatable<JBoolean> jsonBooleanIEquatableTrue = jsonBooleanTrue;
            Assert.Equal(jsonBooleanIEquatableTrue.GetHashCode(), jsonBooleanTrue.GetHashCode());

            IEquatable<JBoolean> jsonBooleanIEquatableFalse = jsonBooleanFalse;
            Assert.Equal(jsonBooleanIEquatableFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());

            object jsonBooleanCopyTrue = jsonBooleanTrue;
            object jsonBooleanObjectTrue = new JBoolean(true);
            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonBooleanCopyTrue.GetHashCode());
            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonBooleanObjectTrue.GetHashCode());

            object jsonBooleanCopyFalse = jsonBooleanFalse;
            object jsonBooleanObjectFalse = new JBoolean(false);
            Assert.Equal(jsonBooleanCopyFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());
            Assert.Equal(jsonBooleanObjectFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());

            Assert.Equal(0, jsonBooleanFalse.GetHashCode());
            Assert.Equal(1, jsonBooleanTrue.GetHashCode());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.True, new JBoolean(true).ValueKind);
            Assert.Equal(JsonValueKind.False, new JBoolean(false).ValueKind);
        }
    }
}
