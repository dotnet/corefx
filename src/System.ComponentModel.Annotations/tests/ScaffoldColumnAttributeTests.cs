using Xunit;

namespace System.ComponentModel.DataAnnotations
{
    public class ScaffoldColumnAttributeTests
    {
        [Fact]
        public static void Can_construct_and_get_Scaffold()
        {
            var attribute = new ScaffoldColumnAttribute(true);
            Assert.Equal(true, attribute.Scaffold);
        }
    }
}
