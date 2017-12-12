// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition.Hosting
{
    [TestClass]
    public class ScopeExtensionsTests
    {
        [TestMethod]
        public void Exports_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            string contractName = "Contract1";
            ExceptionAssert.ThrowsArgumentNull("part", () =>
                {
                    part.Exports(contractName);
                });
        }

        [TestMethod]
        public void Exports_Throws_OnNullContractName()
        {
            ComposablePartDefinition part = typeof(PartExportingContract1).AsPart();
            string contractName = null;
            ExceptionAssert.ThrowsArgumentNull("contractName", () =>
            {
                part.Exports(contractName);
            });
        }

        [TestMethod]
        public void Exports()
        {
            ComposablePartDefinition part1 = typeof(PartExportingContract1).AsPart();
            ComposablePartDefinition part2 = typeof(PartExportingContract2).AsPart();

            Assert.IsTrue(part1.Exports("Contract1"));
            Assert.IsTrue(part2.Exports("Contract2"));

            Assert.IsFalse(part2.Exports("Contract1"));
            Assert.IsFalse(part1.Exports("Contract2"));
        }

        [TestMethod]
        public void Imports_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            string contractName = "Contract1";
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.Imports(contractName);
            });
        }

        [TestMethod]
        public void Imports_Throws_OnNullContractName()
        {
            ComposablePartDefinition part = typeof(PartImportingContract1).AsPart();
            string contractName = null;
            ExceptionAssert.ThrowsArgumentNull("contractName", () =>
            {
                part.Imports(contractName);
            });
        }

        [TestMethod]
        public void Imports()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part2 = typeof(PartImportingContract2).AsPart();

            Assert.IsTrue(part1.Imports("Contract1"));
            Assert.IsTrue(part2.Imports("Contract2"));

            Assert.IsFalse(part2.Imports("Contract1"));
            Assert.IsFalse(part1.Imports("Contract2"));
        }

        [TestMethod]
        public void Imports_CardinalityIgnored_WhenNotSpecified()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part1Multiple = typeof(PartImportingContract1Multiple).AsPart();
            ComposablePartDefinition part1Optional = typeof(PartImportingContract1Optionally).AsPart();

            Assert.IsTrue(part1.Imports("Contract1"));
            Assert.IsTrue(part1Optional.Imports("Contract1"));
            Assert.IsTrue(part1Multiple.Imports("Contract1"));
        }

        [TestMethod]
        public void Imports_CardinalityNotIgnored_WhenSpecified()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part1Multiple = typeof(PartImportingContract1Multiple).AsPart();
            ComposablePartDefinition part1Optional = typeof(PartImportingContract1Optionally).AsPart();

            Assert.IsTrue(part1.Imports("Contract1", ImportCardinality.ExactlyOne));
            Assert.IsFalse(part1.Imports("Contract1", ImportCardinality.ZeroOrMore));
            Assert.IsFalse(part1.Imports("Contract1", ImportCardinality.ZeroOrOne));

            Assert.IsFalse(part1Multiple.Imports("Contract1", ImportCardinality.ExactlyOne));
            Assert.IsTrue(part1Multiple.Imports("Contract1", ImportCardinality.ZeroOrMore));
            Assert.IsFalse(part1Multiple.Imports("Contract1", ImportCardinality.ZeroOrOne));

            Assert.IsFalse(part1Optional.Imports("Contract1", ImportCardinality.ExactlyOne));
            Assert.IsFalse(part1Optional.Imports("Contract1", ImportCardinality.ZeroOrMore));
            Assert.IsTrue(part1Optional.Imports("Contract1", ImportCardinality.ZeroOrOne));

        }

        [TestMethod]
        public void ContainsMetadataWithKey_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.ContainsPartMetadataWithKey("Name");
            });
        }

        [TestMethod]
        public void ContainsMetadataWithKey_Throws_OnNullKey()
        {
            ComposablePartDefinition part = typeof(PartImportingContract1).AsPart();
            ExceptionAssert.ThrowsArgumentNull("key", () =>
            {
                part.ContainsPartMetadataWithKey(null);
            });
        }

        [TestMethod]
        public void ContainsMetadataWithKey()
        {
            ComposablePartDefinition part1 = typeof(PartWithMetadada).AsPart();

            Assert.IsTrue(part1.ContainsPartMetadataWithKey("Name"));
            Assert.IsTrue(part1.ContainsPartMetadataWithKey("Spores"));
            Assert.IsTrue(part1.ContainsPartMetadataWithKey("Adds"));

            Assert.IsFalse(part1.ContainsPartMetadataWithKey("Description"));
        }

        [TestMethod]
        public void ContainsMetadata_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.ContainsPartMetadata("Name", "Festergut");
            });
        }

        [TestMethod]
        public void ContainsMetadata_Throws_OnNullKey()
        {
            ComposablePartDefinition part = typeof(PartImportingContract1).AsPart();
            ExceptionAssert.ThrowsArgumentNull("key", () =>
            {
                part.ContainsPartMetadata(null, "Festergut");
            });
        }

        [TestMethod]
        public void ContainsMetadata()
        {
            ComposablePartDefinition part1 = typeof(PartWithMetadada).AsPart();

            Assert.IsTrue(part1.ContainsPartMetadata("Name", "Festergut"));
            Assert.IsFalse(part1.ContainsPartMetadata("Name", "Rotface"));
            Assert.IsFalse(part1.ContainsPartMetadata<string>("Name", null));

            Assert.IsTrue(part1.ContainsPartMetadata("Spores", 3));
            Assert.IsFalse(part1.ContainsPartMetadata("Spores", 3L));
            Assert.IsFalse(part1.ContainsPartMetadata("Spores", 5));
            Assert.IsFalse(part1.ContainsPartMetadata<string>("Spores", null));

            Assert.IsTrue(part1.ContainsPartMetadata<string>("Adds", null));
            Assert.IsFalse(part1.ContainsPartMetadata("Adds", "Ooze"));

            Assert.IsFalse(part1.ContainsPartMetadata("Ability", "Pungent Blight"));
        }


        [Export("Contract1")]
        public class PartExportingContract1
        {
        }

        [Export("Contract2")]
        public class PartExportingContract2
        {
        }

        public class PartImportingContract1
        {
            [Import("Contract1")]
            public object import;
        }

        public class PartImportingContract2
        {
            [Import("Contract2")]
            public object import;
        }

        public class PartImportingContract1Optionally
        {
            [Import("Contract1", AllowDefault=true)]
            public object import;
        }

        public class PartImportingContract1Multiple
        {
            [ImportMany("Contract1")]
            public object import;
        }

        [PartMetadata("Name", "Festergut")]
        [PartMetadata("Spores", 3)]
        [PartMetadata("Adds", null)]
        [Export("Contract1")]
        public class PartWithMetadada
        {
        }
    }
}
