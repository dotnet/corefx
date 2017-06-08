using System.ComponentModel;
using Xunit;

namespace System.DirectoryServices.Protocols.Tests
{
    public class DirectoryAttributeModificationTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var modification = new DirectoryAttributeModification();
            Assert.Empty(modification.Name);
            Assert.Empty(modification);
            Assert.Equal(DirectoryAttributeOperation.Replace, modification.Operation);
        }

        [Fact]
        public void Operation_SetValid_GetReturnsExpected()
        {
            var modification = new DirectoryAttributeModification { Operation = DirectoryAttributeOperation.Delete };
            Assert.Equal(DirectoryAttributeOperation.Delete, modification.Operation);
        }

        [Theory]
        [InlineData(DirectoryAttributeOperation.Add - 1)]
        [InlineData(DirectoryAttributeOperation.Replace + 1)]
        public void Operation_SetInvalid_InvalidEnumArgumentException(DirectoryAttributeOperation operation)
        {
            var modification = new DirectoryAttributeModification();
            Assert.Throws<InvalidEnumArgumentException>("value", () => modification.Operation = operation);
        }
    }
}
