using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CoreFx.Private.TestUtilities.Tests
{
    [TestCaseOrderer("CoreFx.Private.TestUtilities.Tests.AlphabeticalOrderer", "CoreFx.Private.TestUtilities.Tests")]
    public class ConditionalAttributeTests : IClassFixture<ConditionalAttributeFixture>
    {
        // The tests under this class are to validate that ConditionalFact and ConditionalAttribute tests are being executed correctly
        // In the past we have had cases where infrastructure changes broke this feature and tests where not running in certain framework.
        // This test class is test order dependent so do not rename the tests.
        // If new tests need to be added, follow the same naming pattern ConditionalAttribute{LetterToOrderTest} and then add a Validate{TestName}.

        // Private shared instance containing the shared properties used to validate the tests are running correctly.
        private readonly ConditionalAttributeFixture _fixture;

        public static bool AlwaysTrue => true;

        // The fixture object is injected by xunit.
        public ConditionalAttributeTests(ConditionalAttributeFixture fixture)
        {
            _fixture = fixture;
        }

        [ConditionalFact(nameof(AlwaysTrue))]
        public void ConditionalAttributeA()
        {
            _fixture.ConditionalFactExecuted = true;
        }

        [ConditionalTheory(nameof(AlwaysTrue))]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ConditionalAttributeB(int _)
        {
            _fixture.ConditionalTheoryCount++;
        }

        [Fact]
        public void ValidateConditionalFact()
        {
            Assert.True(_fixture.ConditionalFactExecuted);
        }

        [Fact]
        public void ValidateConditionalTheory()
        {
            Assert.Equal(3, _fixture.ConditionalTheoryCount);
        }
    }

    // A shared instance of this class will get created and will be injected into ConditionalAttributeTests class for every test.
    public class ConditionalAttributeFixture
    {
        public bool ConditionalFactExecuted { get; set; }
        public int ConditionalTheoryCount { get; set; }
    }

    // We need this TestCaseOrderer in order to guarantee that the ConditionalAttributeTests are always executed in the same order.
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
                where TTestCase : ITestCase
        {
            List<TTestCase> result = testCases.ToList();
            result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            return result;
        }
    }
}
