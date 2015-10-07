// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;
using Xunit;

namespace System.Xml.Tests
{
    public partial class CReaderTestModule : CGenericTestModule
    {
        private static void RunTestCaseAsync(Func<CTestBase> testCaseGenerator)
        {
            CModInfo.CommandLine = "/async";
            RunTestCase(testCaseGenerator);
        }

        private static void RunTestCase(Func<CTestBase> testCaseGenerator)
        {
            var module = new CReaderTestModule();

            module.Init(null);
            module.AddChild(testCaseGenerator());
            module.Execute();

            Assert.Equal(0, module.FailCount);
        }

        private static void RunTest(Func<CTestBase> testCaseGenerator)
        {
            RunTestCase(testCaseGenerator);
            RunTestCaseAsync(testCaseGenerator);
        }

        [Fact]
        [OuterLoop]
        static public void RunTests()
        {
            RunTest(() => new TCReadReader() { Attribute = new TestCase() { Name = "Read", Desc = "CustomInheritedReader" } });
        }
    }
}
