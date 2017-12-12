//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CLR.UnitTesting;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ImportDefinitionTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetCardinalityPropertyToExactlyOne()
        {
            var definition = new NoOverridesImportDefinition();

            Assert.AreEqual(ImportCardinality.ExactlyOne, definition.Cardinality);
        }

        [TestMethod]
        public void Constructor1_ShouldSetIsPrerequisitePropertyToTrue()
        {
            var definition = new NoOverridesImportDefinition();

            Assert.IsTrue(definition.IsPrerequisite);
        }

        [TestMethod]
        public void Constructor1_ShouldSetIsRecomposablePropertyToFalse()
        {
            var definition = new NoOverridesImportDefinition();

            Assert.IsFalse(definition.IsRecomposable);
        }

        [TestMethod]
        public void Constructor2_NullAsConstraintArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("constraint", () =>
            {
                new ImportDefinition((Expression<Func<ExportDefinition, bool>>)null, "", ImportCardinality.ExactlyOne, false, false);
            });
        }

        [TestMethod]
        public void Constructor2_OutOfRangeValueAsCardinalityArgument_ShouldThrowArgument()
        {
            var expectations = Expectations.GetInvalidEnumValues<ImportCardinality>();

            foreach (var e in expectations)
            {
                ExceptionAssert.ThrowsArgument<ArgumentException>("cardinality", () =>
                {
                    new ImportDefinition(d => true, "", e, false, false);
                });
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsCardinalityArgument_ShouldSetCardinalityProperty()
        {
            var expectations = Expectations.GetEnumValues<ImportCardinality>();

            foreach (var e in expectations)
            {
                var definition = new ImportDefinition(d => true, "", e, false, false);

                Assert.AreEqual(e, definition.Cardinality);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsConstraintArgument_ShouldSetConstraintProperty()
        {
            var expectations = new List<Expression<Func<ExportDefinition, bool>>>();
            expectations.Add(d => d.ContractName == "ContractName");
            expectations.Add(d => d.ContractName.Equals("ContractName"));
            expectations.Add(d => (string)d.Metadata["Name"] == "Value");
            expectations.Add(d => true);

            foreach (var e in expectations)
            {
                var definition = new ImportDefinition(e, "", ImportCardinality.ExactlyOne, false, false);

                Assert.AreEqual(e, definition.Constraint);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsIsRecomposableArgument_ShouldSetIsRecomposableProperty()
        {
            var expectations = Expectations.GetBooleans();

            foreach (var e in expectations)
            {
                var definition = new ImportDefinition(d => true, "", ImportCardinality.ExactlyOne, e, false);

                Assert.AreEqual(e, definition.IsRecomposable);
            }
        }

        [TestMethod]
        public void Constructor2_ValueAsIsPrerequisiteArgument_ShouldSetIsPrerequisiteProperty()
        {
            var expectations = Expectations.GetBooleans();

            foreach (var e in expectations)
            {
                var definition = new ImportDefinition(d => true, "", ImportCardinality.ExactlyOne, false, e);

                Assert.AreEqual(e, definition.IsPrerequisite);
            }
        }

        [TestMethod]
        public void Constructor2_ContractName_ShouldSetAppropriately()
        {
            var expectations = new ExpectationCollection<string, string>();

            expectations.Add(null, string.Empty);
            expectations.Add(string.Empty, string.Empty);
            expectations.Add("Contract", "Contract");

            string cn = AttributedModelServices.GetContractName(typeof(ImportDefinitionTests));
            expectations.Add(cn, cn);

            foreach (var e in expectations)
            {
                var definition = new ImportDefinition(d => true, e.Input, ImportCardinality.ExactlyOne, false, false);

                Assert.AreEqual(e.Output, definition.ContractName);
            }
        }


        [TestMethod]
        public void Constraint_WhenNotOverridden_ShouldThrowNotImplemented()
        {
            var definition = new NoOverridesImportDefinition();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var constraint = definition.Constraint;
            });
        }

        [TestMethod]
        public void ToString_WhenConstraintPropertyNotOverridden_ShouldThrowNotImplemented()
        {
            var definition = new NoOverridesImportDefinition();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                definition.ToString();
            });
        }

        [TestMethod]
        public void ToString_ValueAsConstraintArgument_ShouldReturnConstraintProperty()
        {
            var expectations = new ExpectationCollection<Expression<Func<ExportDefinition, bool>>, string>();
            expectations.Add(d => d.ContractName == "ContractName", @"d.ContractName ==? ""ContractName""");
            expectations.Add(d => d.ContractName.Equals("ContractName"), @"d.ContractName.Equals\(""ContractName""\)");
            expectations.Add(d => (string)d.Metadata["Name"] == "Value", @"Convert\(d.Metadata.get_Item\(""Name""\)\) ==? ""Value""");
            expectations.Add(d => true, "True");

            foreach (var e in expectations)
            {
                var item = new ImportDefinition(e.Input, "", ImportCardinality.ExactlyOne, false, false);

                Assert.IsTrue(Regex.IsMatch(item.ToString(), e.Output));
            }
        }

        [TestMethod]
        public void ToString_DerivedImportDefinition_ShouldReturnOverriddenConstraintProperty()
        {
            var expectations = new ExpectationCollection<Expression<Func<ExportDefinition, bool>>, string>();
            expectations.Add(d => d.ContractName == "ContractName", @"d.ContractName ==? ""ContractName""");
            expectations.Add(d => d.ContractName.Equals("ContractName"), @"d.ContractName.Equals\(""ContractName""\)");
            expectations.Add(d => (string)d.Metadata["Name"] == "Value", @"Convert\(d.Metadata.get_Item\(""Name""\)\) ==? ""Value""");
            expectations.Add(d => true, "True");

            foreach (var e in expectations)
            {
                var item = new DerivedImportDefinition(e.Input);

                Assert.IsTrue(Regex.IsMatch(item.ToString(), e.Output));
            }
        }

        [TestMethod]
        [WorkItem(738535)]
        [Ignore]
        public void ContractName_ShouldBeIncludedInConstraintAutomatically()
        {
            string testContractName = "TestContractName";
            var contractImportDefinition = new ImportDefinition(ed => true, testContractName, ImportCardinality.ZeroOrMore, false, false);

            var shouldMatch = new ExportDefinition(testContractName, null);
            var shouldNotMatch = new ExportDefinition(testContractName + testContractName, null);

            Assert.IsTrue(contractImportDefinition.IsConstraintSatisfiedBy(shouldMatch));
            Assert.IsFalse(contractImportDefinition.IsConstraintSatisfiedBy(shouldNotMatch));
        }

        [TestMethod]
        public void EmptyContractName_ShouldMatchAllContractNames()
        {
            var importDefinition = new ImportDefinition(ed => true, string.Empty, ImportCardinality.ZeroOrMore, false, false);

            var shouldMatch1 = new ExportDefinition("contract1", null);
            var shouldMatch2 = new ExportDefinition("contract2", null);

            Assert.IsTrue(importDefinition.IsConstraintSatisfiedBy(shouldMatch1));
            Assert.IsTrue(importDefinition.IsConstraintSatisfiedBy(shouldMatch2));
        }

        private class NoOverridesImportDefinition : ImportDefinition
        {
        }

        private class DerivedImportDefinition : ImportDefinition
        {
            private readonly Expression<Func<ExportDefinition, bool>> _constraint;

            public DerivedImportDefinition(Expression<Func<ExportDefinition, bool>> constraint)
            {
                _constraint = constraint;
            }

            public override Expression<Func<ExportDefinition, bool>> Constraint
            {
                get { return _constraint ?? base.Constraint; }
            }
        }
    }
}
