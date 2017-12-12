// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace Tests.Integration
{
    [TestClass]
    public class RequiredCreationPolicyTests
    {
        // Matrix that details which policy to use for a given part to satisfy a given import.
        //                   Part.Any   Part.Shared  Part.NonShared
        // Import.Any        Shared     Shared       NonShared
        // Import.Shared     Shared     Shared       N/A
        // Import.NonShared  NonShared  N/A          NonShared

        public interface ICreationPolicyExport
        {

        }

        [Export(typeof(ICreationPolicyExport))]
        public class CreationPolicyAnyExportImplicit : ICreationPolicyExport
        {

        }

        [Export(typeof(ICreationPolicyExport))]
        [PartCreationPolicy(CreationPolicy.Any)]
        public class CreationPolicyAnyExportExplicit : ICreationPolicyExport
        {

        }

        [Export(typeof(ICreationPolicyExport))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class CreationPolicySharedExport : ICreationPolicyExport
        {

        }

        [Export(typeof(ICreationPolicyExport))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class CreationPolicyNonSharedExport : ICreationPolicyExport
        {

        }

        [Export]
        public class RequiredAnyImporterImplicit
        {
            [ImportMany]
            public IEnumerable<ICreationPolicyExport> Exports { get; set; }
        }

        [Export]
        public class RequiredAnyImporterExplicit
        {
            [ImportMany(RequiredCreationPolicy = CreationPolicy.Any)]
            public IEnumerable<ICreationPolicyExport> Exports { get; set; }
        }

        [Export]
        public class RequiredSharedImporter
        {
            [ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
            public IEnumerable<ICreationPolicyExport> Exports { get; set; }
        }

        [Export]
        public class RequiredNonSharedImporter
        {
            [ImportMany(RequiredCreationPolicy = CreationPolicy.NonShared)]
            public IEnumerable<ICreationPolicyExport> Exports { get; set; }
        }

        private static CompositionContainer CreateDefaultContainer()
        {
            return ContainerFactory.CreateWithAttributedCatalog(
                typeof(ICreationPolicyExport),

                typeof(CreationPolicyAnyExportImplicit),
                typeof(CreationPolicyAnyExportExplicit),
                typeof(CreationPolicySharedExport),
                typeof(CreationPolicyNonSharedExport),
                
                typeof(RequiredAnyImporterImplicit),
                typeof(RequiredAnyImporterExplicit),
                typeof(RequiredSharedImporter),
                typeof(RequiredNonSharedImporter));
        }

        [TestMethod]
        public void RequiredAnyImporterImplicit_ShouldIncludeAll()
        {
            var container = CreateDefaultContainer();

            var importer = container.GetExportedValue<RequiredAnyImporterImplicit>();

            EnumerableAssert.AreEqual(
                importer.Exports.Select(obj => obj.GetType()),
                typeof(CreationPolicyAnyExportImplicit),
                typeof(CreationPolicyAnyExportExplicit),
                typeof(CreationPolicySharedExport),
                typeof(CreationPolicyNonSharedExport));
        }

        [TestMethod]
        public void RequiredAnyImporterExplicit_ShouldIncludeAll()
        {
            var container = CreateDefaultContainer();

            var importer = container.GetExportedValue<RequiredAnyImporterExplicit>();

            EnumerableAssert.AreEqual(
                importer.Exports.Select(obj => obj.GetType()),
                typeof(CreationPolicyAnyExportImplicit),
                typeof(CreationPolicyAnyExportExplicit),
                typeof(CreationPolicySharedExport),
                typeof(CreationPolicyNonSharedExport));
        }

        [TestMethod]
        public void RequiredSharedImporter_ShouldFilterNonShared()
        {
            var container = CreateDefaultContainer();

            var importer = container.GetExportedValue<RequiredSharedImporter>();

            EnumerableAssert.AreEqual(
                importer.Exports.Select(obj => obj.GetType()),
                typeof(CreationPolicyAnyExportImplicit),
                typeof(CreationPolicyAnyExportExplicit),
                typeof(CreationPolicySharedExport));
        }

        [TestMethod]
        public void RequiredNonSharedImporter_ShouldFilterShared()
        {
            var container = CreateDefaultContainer();

            var importer = container.GetExportedValue<RequiredNonSharedImporter>();

            EnumerableAssert.AreEqual(
                importer.Exports.Select(obj => obj.GetType()),
                typeof(CreationPolicyAnyExportImplicit),
                typeof(CreationPolicyAnyExportExplicit),
                typeof(CreationPolicyNonSharedExport));
        }
    }
}
