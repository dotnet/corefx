// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
namespace System.Composition.UnitTests
{
    [TestClass]
    public class ContractTests
    {
        class AType { }

        private static readonly Type _DefaultContractType = typeof(AType);

        [TestMethod]
        public void FormattingAContractWithNoDiscriminatorShowsTheSimpleTypeName()
        {
            var c = new CompositionContract(typeof(AType));
            var s = c.ToString();
            Assert.AreEqual("AType", s);
        }

        [TestMethod]
        public void FormattingAContractWithAStringDiscriminatorShowsTheDiscriminatorInQuotes()
        {
            var c = new CompositionContract(typeof(AType), "at");
            var s = c.ToString();
            Assert.AreEqual("AType \"at\"", s);
        }

        [TestMethod]
        public void ChangingTheTypeOfAContractPreservesTheContractName()
        {
            var name = "a";
            var c = new CompositionContract(typeof(object), name);
            var d = c.ChangeType(typeof(AType));
            Assert.AreEqual(name, d.ContractName);
        }

        [TestMethod]
        public void ChangingTheTypeOfAContractChangesTheContractType()
        {
            var c = new CompositionContract(typeof(object));
            var d = c.ChangeType(typeof(AType));
            Assert.AreEqual(typeof(AType), d.ContractType);
        }

        [TestMethod]
        public void ConstraintsWithEquivalentKeysAndValuesAreEqual()
        {
            var mcd1 = new CompositionContract(_DefaultContractType, null, new Dictionary<string, object> { { "A", new[] { "B" } } });
            var mcd2 = new CompositionContract(_DefaultContractType, null, new Dictionary<string, object> { { "A", new[] { "B" } } });
            Assert.IsTrue(mcd1.Equals(mcd2));
        }

        [TestMethod]
        public void ConstraintsWithEquivalentKeysAndValuesHaveTheSameHashCode()
        {
            var mcd1 = new CompositionContract(_DefaultContractType, null, new Dictionary<string, object> { { "A", new[] { "B" } } });
            var mcd2 = new CompositionContract(_DefaultContractType, null, new Dictionary<string, object> { { "A", new[] { "B" } } });
            Assert.AreEqual(mcd1.GetHashCode(), mcd2.GetHashCode());
        }

        [TestMethod]
        public void FormattingTheContractPrintsConstraintKeysAndValues()
        {
            var mcd = new CompositionContract(typeof(AType), null, new Dictionary<string, object> { { "A", 1 }, { "B", "C" } });
            var s = mcd.ToString();
            Assert.AreEqual("AType { A = 1, B = \"C\" }", s);
        }

        [TestMethod]
        public void FormattingTheContractPrintsNameAndDiscriminator()
        {
            var mcd = new CompositionContract(typeof(AType), "inner", new Dictionary<string, object> { { "A", 1 } });
            var s = mcd.ToString();
            Assert.AreEqual("AType \"inner\" { A = 1 }", s);
        }

        [TestMethod]
        public void AContractWithConstraintIsNotEqualToAContractWithoutConstraint()
        {
            var first = new CompositionContract(typeof(string), null, new Dictionary<string, object> { { "A", 1 } });
            var second = new CompositionContract(typeof(string));
            Assert.IsFalse(first.Equals(second));
        }
    }
}
