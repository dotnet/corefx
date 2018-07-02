// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;
using System.Xml.Xsl;
using OLEDB.Test.ModuleCore;

namespace System.Xml.Tests
{
    //[TestCase(Name = "Load(Type) API Functional Tests", Desc = "This testcase exercises the API tests for the Load(Type) overload")]
    public class XsltcAPITest : XsltcTestCaseBase
    {
        private ITestOutputHelper _output;
        public XsltcAPITest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        [ActiveIssue(30017)]
        //[Variation("1", Desc = "Pass null, Load((Type) null)", Pri = 0)]
        [InlineData()]
        [Theory]
        public void Var1()
        {
            try
            {
                new XslCompiledTransform().Load((Type)null);
            }
            catch (ArgumentNullException)
            {
                return;
            }

            throw new CTestFailedException("Did not throw ArgumentException");
        }

        [ActiveIssue(30017)]
        //[Variation("2", Desc = "Pass types that are not generated by xsltc.exe, Load(typeof(Object))", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var2()
        {
            try
            {
                new XslCompiledTransform().Load(typeof(object));
            }
            catch (ArgumentException)
            {
                return;
            }

            throw new CTestFailedException("Did not throw ArgumentException");
        }
    }
}