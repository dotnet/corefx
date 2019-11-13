// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JTreeBooleanTests
    {
        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonBoolean = new JTreeBoolean();
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestValueCtor()
        {
            var jsonBoolean = new JTreeBoolean(true);
            Assert.True(jsonBoolean.Value);

            jsonBoolean = new JTreeBoolean(false);
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestImplicitOperator()
        {
            JTreeNode jsonNode = true;
            JTreeBoolean jsonBoolean = (JTreeBoolean)jsonNode;
            Assert.True(jsonBoolean.Value);

            jsonNode = false;
            jsonBoolean = (JTreeBoolean)jsonNode;
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestChangingValue()
        {
            var jsonBoolean = new JTreeBoolean();
            jsonBoolean.Value = true;
            Assert.True(jsonBoolean.Value);
            jsonBoolean.Value = false;
            Assert.False(jsonBoolean.Value);
        }

        [Fact]
        public static void TestToString()
        {
            Assert.Equal("true", new JTreeBoolean(true).ToString());
            Assert.Equal("false", new JTreeBoolean(false).ToString());
        }

        [Fact]
        public static void TestEquals()
        {
            var jsonBooleanTrue = new JTreeBoolean(true);
            var jsonBooleanFalse = new JTreeBoolean(false);

            Assert.True(jsonBooleanTrue.Equals(new JTreeBoolean(true)));
            Assert.True(new JTreeBoolean(true).Equals(jsonBooleanTrue));

            Assert.True(jsonBooleanFalse.Equals(new JTreeBoolean(false)));
            Assert.True(new JTreeBoolean(false).Equals(jsonBooleanFalse));
            Assert.True(new JTreeBoolean().Equals(jsonBooleanFalse));
            Assert.False(new JTreeBoolean().Equals(jsonBooleanTrue));
            Assert.False(jsonBooleanFalse.Equals(jsonBooleanTrue));
            Assert.False(jsonBooleanTrue.Equals(jsonBooleanFalse));

            Assert.True(jsonBooleanTrue == new JTreeBoolean(true));
            Assert.True(jsonBooleanTrue != jsonBooleanFalse);

            JTreeNode jsonNodeTrue = new JTreeBoolean(true);
            Assert.True(jsonBooleanTrue.Equals(jsonNodeTrue));

            JTreeNode jsonNodeFalse= new JTreeBoolean(false);
            Assert.True(jsonBooleanFalse.Equals(jsonNodeFalse));

            IEquatable<JTreeBoolean> jsonBooleanIEquatableTrue = jsonBooleanTrue;
            Assert.True(jsonBooleanIEquatableTrue.Equals(jsonBooleanTrue));
            Assert.True(jsonBooleanTrue.Equals(jsonBooleanIEquatableTrue));

            IEquatable<JTreeBoolean> jsonBooleanIEquatableFalse = jsonBooleanFalse;
            Assert.True(jsonBooleanIEquatableFalse.Equals(jsonBooleanFalse));
            Assert.True(jsonBooleanFalse.Equals(jsonBooleanIEquatableFalse));

            Assert.False(jsonBooleanTrue.Equals(null));

            object jsonBooleanCopyTrue = jsonBooleanTrue;
            object jsonBooleanObjectTrue = new JTreeBoolean(true);
            Assert.True(jsonBooleanCopyTrue.Equals(jsonBooleanObjectTrue));
            Assert.True(jsonBooleanCopyTrue.Equals(jsonBooleanObjectTrue));
            Assert.True(jsonBooleanObjectTrue.Equals(jsonBooleanTrue));

            object jsonBooleanCopyFalse = jsonBooleanFalse;
            object jsonBooleanObjectFalse = new JTreeBoolean(false);
            Assert.True(jsonBooleanCopyFalse.Equals(jsonBooleanObjectFalse));
            Assert.True(jsonBooleanCopyFalse.Equals(jsonBooleanFalse));
            Assert.True(jsonBooleanObjectFalse.Equals(jsonBooleanFalse));

            Assert.False(jsonBooleanTrue.Equals(new Exception()));

            JTreeBoolean jsonBooleanNull = null;
            Assert.False(jsonBooleanTrue == jsonBooleanNull);
            Assert.False(jsonBooleanNull == jsonBooleanTrue);

            Assert.True(jsonBooleanTrue != jsonBooleanNull);
            Assert.True(jsonBooleanNull != jsonBooleanTrue);

            JTreeBoolean otherJTreeBooleanNull = null;
            Assert.True(jsonBooleanNull == otherJTreeBooleanNull);
        }

        [Fact]
        public static void TestGetHashCode()
        {
            var jsonBooleanTrue = new JTreeBoolean(true);
            var jsonBooleanFalse = new JTreeBoolean(false);

            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonBooleanTrue.GetHashCode());
            Assert.Equal(jsonBooleanTrue.GetHashCode(), new JTreeBoolean(true).GetHashCode());
            Assert.Equal(jsonBooleanFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());
            Assert.Equal(jsonBooleanFalse.GetHashCode(), new JTreeBoolean(false).GetHashCode());
            Assert.Equal(jsonBooleanFalse.GetHashCode(), new JTreeBoolean().GetHashCode());
            Assert.NotEqual(jsonBooleanTrue.GetHashCode(), new JTreeBoolean().GetHashCode());

            JTreeNode jsonNodeTrue = new JTreeBoolean(true);
            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonNodeTrue.GetHashCode());
            JTreeNode jsonNodeFalse = new JTreeBoolean(false);
            Assert.Equal(jsonBooleanFalse.GetHashCode(), jsonNodeFalse.GetHashCode());

            Assert.NotEqual(jsonBooleanTrue.GetHashCode(), jsonBooleanFalse.GetHashCode());

            IEquatable<JTreeBoolean> jsonBooleanIEquatableTrue = jsonBooleanTrue;
            Assert.Equal(jsonBooleanIEquatableTrue.GetHashCode(), jsonBooleanTrue.GetHashCode());

            IEquatable<JTreeBoolean> jsonBooleanIEquatableFalse = jsonBooleanFalse;
            Assert.Equal(jsonBooleanIEquatableFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());

            object jsonBooleanCopyTrue = jsonBooleanTrue;
            object jsonBooleanObjectTrue = new JTreeBoolean(true);
            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonBooleanCopyTrue.GetHashCode());
            Assert.Equal(jsonBooleanTrue.GetHashCode(), jsonBooleanObjectTrue.GetHashCode());

            object jsonBooleanCopyFalse = jsonBooleanFalse;
            object jsonBooleanObjectFalse = new JTreeBoolean(false);
            Assert.Equal(jsonBooleanCopyFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());
            Assert.Equal(jsonBooleanObjectFalse.GetHashCode(), jsonBooleanFalse.GetHashCode());

            Assert.Equal(0, jsonBooleanFalse.GetHashCode());
            Assert.Equal(1, jsonBooleanTrue.GetHashCode());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.True, new JTreeBoolean(true).ValueKind);
            Assert.Equal(JsonValueKind.False, new JTreeBoolean(false).ValueKind);
        }
    }
}
