// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.CLR.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class ExportDefinitionTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var definition = new DerivedExportDefinition();

            EnumerableAssert.IsEmpty(definition.Metadata);
        }

        [TestMethod]
        public void Constructor1_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var definition = new DerivedExportDefinition();

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor2_NullAsContractNameArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("contractName", () =>
            {
                new ExportDefinition((string)null, new Dictionary<string, object>());
            });
        }

        [TestMethod]
        public void Constructor2_EmptyStringAsContractNameArgument_ShouldThrowArgument()
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("contractName", () =>
            {
                new ExportDefinition(string.Empty, new Dictionary<string, object>());
            });
        }

        [TestMethod]
        public void Constructor2_ValueAsContractNameArgument_ShouldSetContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();

            foreach (var e in expectations)
            {
                var definition = new ExportDefinition(e, new Dictionary<string, object>());

                Assert.AreEqual(e, definition.ContractName);
            }
        }

        [TestMethod]
        public void Constructor2_NullAsMetadataArgument_ShouldSetMetadataPropertyToEmptyDictionary()
        {
            var definition = new ExportDefinition("Contract", (IDictionary<string, object>)null); ;

            EnumerableAssert.IsEmpty(definition.Metadata);
        }

        [TestMethod]
        public void Constructor2_NullAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var definition = new ExportDefinition("Contract", (IDictionary<string, object>)null);

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor2_WritableDictionaryAsMetadataArgument_ShouldSetMetadataPropertyToReadOnlyDictionary()
        {
            var definition = new ExportDefinition("Contract", new Dictionary<string, object>());

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                definition.Metadata["Value"] = "Value";
            });
        }

        [TestMethod]
        public void Constructor2_DictionaryAsMetadataArgument_ShouldSetMetadataProperty()
        {
            var expectations = Expectations.GetMetadata();

            foreach (var e in expectations)
            {
                var definition = new ExportDefinition("Contract", e);

                EnumerableAssert.AreEqual(e, definition.Metadata);
            }
        }

        [TestMethod]
        public void ContractName_WhenNotOverridden_ShouldThrowNotImplemented()
        {
            var definition = new DerivedExportDefinition();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                var contractName = definition.ContractName;
            });
        }

        [TestMethod]
        public void ToString_WhenContractNameNotOverridden_ShouldThrowNotImplemented()
        {
            var definition = new DerivedExportDefinition();

            ExceptionAssert.Throws<NotImplementedException>(() =>
            {
                definition.ToString();
            });
        }

        [TestMethod]
        public void ToString_ShouldReturnContractNameProperty()
        {
            var expectations = Expectations.GetContractNames();

            foreach (var e in expectations)
            {
                var definition = new ExportDefinition(e, new Dictionary<string, object>());

                Assert.AreEqual(e, definition.ToString());
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnOverriddenContractNameProperty()
        {
            var expectations = Expectations.GetContractNamesWithEmpty();
            
            foreach (var e in expectations)
            {
                var definition = new DerivedExportDefinition(() => e);

                Assert.AreEqual(e, definition.ToString());
            }
        }

        private class DerivedExportDefinition : ExportDefinition
        {
            private readonly Func<string> _contractNameGetter;

            public DerivedExportDefinition()
            {
            }

            public DerivedExportDefinition(Func<string> contractNameGetter)
            {
                _contractNameGetter = contractNameGetter;
            }

            public override string ContractName
            {
                get 
                {
                    if (_contractNameGetter != null)
                    {
                        return _contractNameGetter();
                    }

                    return base.ContractName;
                }
            }
        }
    }
}

