
using Xunit;

namespace System.Drawing.Design.Primitives.Tests.System.Drawing.Design
{
    public class ToolboxItemTests
    {
        [Fact]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_DefaultsAssignedCorrectly()
        {
            // Arrange
            var type = typeof(Bitmap);
            var unitUnderTest = new ToolboxItem(type);

            // Assert
            Assert.Equal(type.FullName, unitUnderTest.TypeName);
            Assert.Equal(type.Name, unitUnderTest.DisplayName);
            Assert.Equal(type.Assembly.GetName(true), unitUnderTest.AssemblyName);
            Assert.NotNull(unitUnderTest.DependentAssemblies);
            Assert.NotEqual(unitUnderTest.Description, "");
            Assert.NotEqual(unitUnderTest.Filter.Count, 0);
        }

        [Fact]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToString_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var type = typeof(Bitmap);
            var unitUnderTest = new ToolboxItem(type);

            // Act
            var result = unitUnderTest.ToString();

            // Assert
            Assert.Equal(type.Name, result);
        }
    }
}
