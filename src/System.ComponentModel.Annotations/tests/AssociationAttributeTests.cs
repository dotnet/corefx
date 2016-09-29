// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.DataAnnotations.Tests
{
    public class AssociationAttributeTests
    {
#pragma warning disable 618
        [Theory]
        [InlineData("TestName", "TestThisKey", "TestOtherKey", new string[] { "TestThisKey" }, new string[] { "TestOtherKey" })]
        [InlineData(null, "", " \t \r \n", new string[] { "" }, new string[] { "\t\r\n" })]
        [InlineData(null, null, null, new string[0], new string[0])]
        [InlineData("Name", "ThisKey1,  ThisKey2, ThisKey3", "OtherKey1,  OtherKey2",new string[] { "ThisKey1", "ThisKey2", "ThisKey3" }, new string[] { "OtherKey1", "OtherKey2" })]
        public static void Constructor(string name, string thisKey, string otherKey, string[] thisKeyMembers, string[] otherKeyMembers)
        {
            var attribute = new AssociationAttribute(name, thisKey, otherKey);
            Assert.Equal(name, attribute.Name);
            Assert.Equal(thisKey, attribute.ThisKey);
            Assert.Equal(otherKey, attribute.OtherKey);

            if (thisKey == null)
            {
                Assert.Throws<NullReferenceException>(() => attribute.ThisKeyMembers);
            }
            else
            {
                Assert.Equal(thisKeyMembers, attribute.ThisKeyMembers);
            }
            if (otherKey == null)
            {
                Assert.Throws<NullReferenceException>(() => attribute.OtherKeyMembers);
            }
            else
            {
                Assert.Equal(otherKeyMembers, attribute.OtherKeyMembers);
            }
        }

        [Fact]
        public static void IsForeignKey_GetSet_ReturnsExpected()
        {
            var attribute = new AssociationAttribute("Name", "ThisKey", "OtherKey");
            Assert.False(attribute.IsForeignKey);

            attribute.IsForeignKey = true;
            Assert.True(attribute.IsForeignKey);

            attribute.IsForeignKey = false;
            Assert.False(attribute.IsForeignKey);
        }
#pragma warning restore 618
    }
}
