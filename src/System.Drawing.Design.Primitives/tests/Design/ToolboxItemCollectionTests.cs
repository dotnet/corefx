
using Xunit;

namespace System.Drawing.Design.Primitives.Tests.System.Drawing.Design
{
    public class ToolboxItemCollectionTests
    {

        private ToolboxItemCollection CreateToolboxItemCollection()
        {
            ToolboxItem[]  tools = { new ToolboxItem(typeof(Bitmap)), new ToolboxItem(typeof(Image)) };

            return new ToolboxItemCollection(tools);
        }

        [Fact]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Contains_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateToolboxItemCollection();
            ToolboxItem value = unitUnderTest[0];

            // Act
            var result = unitUnderTest.Contains(
                value);

            // Assert
            Assert.True(result);
        }

        [Fact]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IndexOf_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateToolboxItemCollection();
            ToolboxItem value = unitUnderTest[0];

            // Act
            var result = unitUnderTest.IndexOf(
                value);

            // Assert
            Assert.Equal(result, 0);
        }
    }
}
