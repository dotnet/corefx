// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JNullTests
    {
        [Fact]
        public static void TestToString()
        {
            Assert.Equal("null", new JNull().ToString());
        }

        [Fact]
        public static void TestEquals()
        {
            var jsonNull1 = new JNull();
            var jsonNull2 = new JNull();

            Assert.True(jsonNull1.Equals(jsonNull2));
            Assert.True(jsonNull1 == jsonNull2);
            Assert.False(jsonNull1 != jsonNull2);

            JNode jsonNodeJNull = new JNull();
            Assert.True(jsonNull1.Equals(jsonNodeJNull));
            Assert.True(jsonNodeJNull.Equals(jsonNull1));

            IEquatable<JNull> jsonNullIEquatable = jsonNull2;
            Assert.True(jsonNullIEquatable.Equals(jsonNull1));
            Assert.True(jsonNull1.Equals(jsonNullIEquatable));

            object jsonNullCopy = jsonNull1;

            Assert.True(jsonNullCopy.Equals(jsonNull1));
            Assert.True(jsonNull1.Equals(jsonNullCopy));

            object jsonNullObject = new JNull();

            Assert.True(jsonNullObject.Equals(jsonNull1));
            Assert.True(jsonNull1.Equals(jsonNullObject));

            Assert.False(jsonNull1.Equals(new JString("null")));
            Assert.False(jsonNull1.Equals(new Exception()));

            // Null comparisons behave this way because of implicit conversion from null to JNull:
           
            Assert.True(jsonNull1.Equals(null));
            Assert.True(jsonNull1 == null);
            Assert.False(jsonNull1 != null);

            JNull jsonNullNull = null;

            Assert.True(jsonNull1.Equals(jsonNullNull));
            Assert.True(jsonNull1 == jsonNullNull);
            Assert.False(jsonNull1 != jsonNullNull);

            JNull otherJNullNull = null;
            Assert.True(jsonNullNull == otherJNullNull);

            // Only for null JNode / different derived type this will return false:

            JNode jsonNodeNull = null;
            Assert.False(jsonNull1.Equals(jsonNodeNull));
          
            JArray jsonArrayNull = null;
            Assert.False(jsonNull1.Equals(jsonArrayNull));
        }

        [Fact]
        public static void TestGetHashCode()
        {
            var jsonNull = new JNull();

            Assert.Equal(jsonNull.GetHashCode(), new JNull().GetHashCode());
            
            JNode jsonNodeNull = new JNull();
            Assert.Equal(jsonNull.GetHashCode(), jsonNodeNull.GetHashCode());
            
            IEquatable<JNull> jsonNullIEquatable = new JNull();
            Assert.Equal(jsonNull.GetHashCode(), jsonNullIEquatable.GetHashCode());

            object jsonNullCopy= jsonNull;
            object jsonNullObject = new JNull();
            Assert.Equal(jsonNull.GetHashCode(), jsonNullCopy.GetHashCode());
            Assert.Equal(jsonNull.GetHashCode(), jsonNullObject.GetHashCode());

            Assert.Equal(-1, jsonNull.GetHashCode());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.Null, new JNull().ValueKind);
        }
    }
}
