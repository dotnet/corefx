// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JTreeNullTests
    {
        [Fact]
        public static void TestToString()
        {
            Assert.Equal("null", new JTreeNull().ToString());
        }

        [Fact]
        public static void TestEquals()
        {
            var jsonNull1 = new JTreeNull();
            var jsonNull2 = new JTreeNull();

            Assert.True(jsonNull1.Equals(jsonNull2));
            Assert.True(jsonNull1 == jsonNull2);
            Assert.False(jsonNull1 != jsonNull2);

            JTreeNode jsonNodeJTreeNull = new JTreeNull();
            Assert.True(jsonNull1.Equals(jsonNodeJTreeNull));
            Assert.True(jsonNodeJTreeNull.Equals(jsonNull1));

            IEquatable<JTreeNull> jsonNullIEquatable = jsonNull2;
            Assert.True(jsonNullIEquatable.Equals(jsonNull1));
            Assert.True(jsonNull1.Equals(jsonNullIEquatable));

            object jsonNullCopy = jsonNull1;

            Assert.True(jsonNullCopy.Equals(jsonNull1));
            Assert.True(jsonNull1.Equals(jsonNullCopy));

            object jsonNullObject = new JTreeNull();

            Assert.True(jsonNullObject.Equals(jsonNull1));
            Assert.True(jsonNull1.Equals(jsonNullObject));

            Assert.False(jsonNull1.Equals(new JTreeString("null")));
            Assert.False(jsonNull1.Equals(new Exception()));

            // Null comparisons behave this way because of implicit conversion from null to JTreeNull:
           
            Assert.True(jsonNull1.Equals(null));
            Assert.True(jsonNull1 == null);
            Assert.False(jsonNull1 != null);

            JTreeNull jsonNullNull = null;

            Assert.True(jsonNull1.Equals(jsonNullNull));
            Assert.True(jsonNull1 == jsonNullNull);
            Assert.False(jsonNull1 != jsonNullNull);

            JTreeNull otherJTreeNullNull = null;
            Assert.True(jsonNullNull == otherJTreeNullNull);

            // Only for null JTreeNode / different derived type this will return false:

            JTreeNode jsonNodeNull = null;
            Assert.False(jsonNull1.Equals(jsonNodeNull));
          
            JTreeArray jsonArrayNull = null;
            Assert.False(jsonNull1.Equals(jsonArrayNull));
        }

        [Fact]
        public static void TestGetHashCode()
        {
            var jsonNull = new JTreeNull();

            Assert.Equal(jsonNull.GetHashCode(), new JTreeNull().GetHashCode());
            
            JTreeNode jsonNodeNull = new JTreeNull();
            Assert.Equal(jsonNull.GetHashCode(), jsonNodeNull.GetHashCode());
            
            IEquatable<JTreeNull> jsonNullIEquatable = new JTreeNull();
            Assert.Equal(jsonNull.GetHashCode(), jsonNullIEquatable.GetHashCode());

            object jsonNullCopy= jsonNull;
            object jsonNullObject = new JTreeNull();
            Assert.Equal(jsonNull.GetHashCode(), jsonNullCopy.GetHashCode());
            Assert.Equal(jsonNull.GetHashCode(), jsonNullObject.GetHashCode());

            Assert.Equal(-1, jsonNull.GetHashCode());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.Null, new JTreeNull().ValueKind);
        }
    }
}
