// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.Serialization.Tests
{
    public class CollectionDataContractAttributeTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var attribute = new CollectionDataContractAttribute();
            Assert.False(attribute.IsReference);
            Assert.False(attribute.IsReferenceSetExplicitly);
            Assert.Null(attribute.ItemName);
            Assert.False(attribute.IsItemNameSetExplicitly);
            Assert.Null(attribute.Name);
            Assert.False(attribute.IsNameSetExplicitly);
            Assert.Null(attribute.Namespace);
            Assert.False(attribute.IsNamespaceSetExplicitly);
            Assert.Null(attribute.KeyName);
            Assert.False(attribute.IsKeyNameSetExplicitly);
            Assert.Null(attribute.ValueName);
            Assert.False(attribute.IsValueNameSetExplicitly);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsReference_Set_GetReturnsExpected(bool value)
        {
            var attribute = new CollectionDataContractAttribute() { IsReference = value };
            Assert.Equal(value, attribute.IsReference);
            Assert.True(attribute.IsReferenceSetExplicitly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("value")]
        public void ItemName_Set_GetReturnsExpected(string value)
        {
            var attribute = new CollectionDataContractAttribute() { ItemName = value };
            Assert.Equal(value, attribute.ItemName);
            Assert.True(attribute.IsItemNameSetExplicitly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("value")]
        public void Name_Set_GetReturnsExpected(string value)
        {
            var attribute = new CollectionDataContractAttribute() { Name = value };
            Assert.Equal(value, attribute.Name);
            Assert.True(attribute.IsNameSetExplicitly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("value")]
        public void Namespace_Set_GetReturnsExpected(string value)
        {
            var attribute = new CollectionDataContractAttribute() { Namespace = value };
            Assert.Equal(value, attribute.Namespace);
            Assert.True(attribute.IsNamespaceSetExplicitly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("value")]
        public void KeyName_Set_GetReturnsExpected(string value)
        {
            var attribute = new CollectionDataContractAttribute() { KeyName = value };
            Assert.Equal(value, attribute.KeyName);
            Assert.True(attribute.IsKeyNameSetExplicitly);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("value")]
        public void ValueName_Set_GetReturnsExpected(string value)
        {
            var attribute = new CollectionDataContractAttribute() { ValueName = value };
            Assert.Equal(value, attribute.ValueName);
            Assert.True(attribute.IsValueNameSetExplicitly);
        }
    }
}
