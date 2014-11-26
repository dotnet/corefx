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
    public class MetadataConstraintTests : ContainerTests
    {
        [Export, ExportMetadata("SettingName", "TheName")]
        public class SomeSetting { }

        [Export]
        public class SomeSettingUser
        {
            [Import, ImportMetadataConstraint("SettingName", "TheName")]
            public SomeSetting Setting { get; set; }
        }

        [Export]
        public class ManySettingUser
        {
            [ImportMany, ImportMetadataConstraint("SettingName", "TheName")]
            public IEnumerable<SomeSetting> Settings { get; set; }
        }

        [TestMethod]
        public void AnImportMetadataConstraintMatchesMetadataOnTheExport()
        {
            var cc = CreateContainer(typeof(SomeSetting), typeof(SomeSettingUser));
            var ssu = cc.GetExport<SomeSettingUser>();
            Assert.IsNotNull(ssu.Setting);
        }

        [TestMethod]
        public void AnImportMetadataConstraintMatchesMetadataOnTheExportEvenIfDiscoveryHasCompletedForTheExport()
        {
            var cc = CreateContainer(typeof(SomeSetting), typeof(SomeSettingUser));
            cc.GetExport<SomeSetting>();
            var ssu = cc.GetExport<SomeSettingUser>();
            Assert.IsNotNull(ssu.Setting);
        }

        [TestMethod]
        public void ImportMetadataConstraintsComposeWithOtherRelationshipTypes()
        {
            var cc = CreateContainer(typeof(SomeSetting), typeof(ManySettingUser));
            var ssu = cc.GetExport<ManySettingUser>();
            Assert.AreEqual(1, ssu.Settings.Count());
        }

        [Export, ExportMetadata("SettingName", "TheName")]
        public class SomeSetting<T> { }

        [TestMethod]
        public void ConstraintsCanBeAppliedToGenerics()
        {
            var contract = new CompositionContract(typeof(SomeSetting<string>), null, new Dictionary<string, object>
            {
                { "SettingName", "TheName" }
            });

            var c = CreateContainer(typeof(SomeSetting<>));
            object s;
            Assert.IsTrue(c.TryGetExport(contract, out s));
        }

        [Export, ExportMetadata("Items", new[] { 1, 2, 3 })]
        public class Presenter { }

        [Export]
        public class Controller
        {
            [Import, ImportMetadataConstraint("Items", new[] { 1, 2, 3 })]
            public Presenter Presenter { get; set; }
        }

        [TestMethod]
        public void ItemEqualityIsUsedWhenMatchingMetadataValuesThatAreArrays()
        {
            var c = CreateContainer(typeof(Presenter), typeof(Controller));
            var ctrl = c.GetExport<Controller>();
            Assert.IsInstanceOfType(ctrl.Presenter, typeof(Presenter));
        }
    }
}
