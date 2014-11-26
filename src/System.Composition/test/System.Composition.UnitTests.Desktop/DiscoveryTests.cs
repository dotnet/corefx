// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Convention;
using System.Composition.Runtime;
using System.Composition.UnitTests.Util;
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
using System.Composition.Hosting;

namespace System.Composition.UnitTests
{
    [TestClass]
    public class DiscoveryTests : ContainerTests
    {
        public interface IRule { }

        public class RuleExportAttribute : ExportAttribute
        {
            public RuleExportAttribute() : base(typeof(IRule)) { }
        }

        [RuleExport]
        public class UnfairRule : IRule { }

        [Export(typeof(IRule))]
        public class IncompatibleRule { }

        public class IncompatibleRuleProperty
        {
            [Export(typeof(IRule))]
            public string Rule { get; set; }
        }

        [Export, PartNotDiscoverable]
        public class NotDiscoverable { }

        [TestMethod]
        public void DiscoversCustomExportAttributes()
        {
            var container = CreateContainer(typeof(UnfairRule));
            var rule = container.GetExport<IRule>();
            Assert.IsInstanceOfType(rule, typeof(UnfairRule));
        }

        [TestMethod]
        public void DiscoversCustomExportAttributesUnderConventions()
        {
            var container = CreateContainer(new ConventionBuilder(), typeof(UnfairRule));
            var rule = container.GetExport<IRule>();
            Assert.IsInstanceOfType(rule, typeof(UnfairRule));
        }

        [TestMethod]
        public void InstanceExportsOfIncompatibleContractsAreDetected()
        {
            var x = AssertX.Throws<CompositionFailedException>(() => CreateContainer(typeof(IncompatibleRule)));
            Assert.AreEqual("Exported contract type 'IRule' is not assignable from part 'IncompatibleRule'.", x.Message);
        }

        [TestMethod]
        public void PropertyExportsOfIncompatibleContractsAreDetected()
        {
            var x = AssertX.Throws<CompositionFailedException>(() => CreateContainer(typeof(IncompatibleRuleProperty)));
            Assert.AreEqual("Exported contract type 'IRule' is not assignable from property 'Rule' of part 'IncompatibleRuleProperty'.", x.Message);
        }

        [TestMethod]
        public void ANonDiscoverablePartIsIgnored()
        {
            var container = CreateContainer(typeof(NotDiscoverable));
            NotDiscoverable unused;
            Assert.IsFalse(container.TryGetExport(null, out unused));
        }

        public interface IBus { }

        [Export(typeof(IBus))]
        public class CloudBus : IBus { }

        public class SpecialCloudBus : CloudBus { }

        [TestMethod]
        public void DoesNotDiscoverExportAttributesFromBase()
        {
            var container = CreateContainer(typeof(SpecialCloudBus));

            IBus bus;
            Assert.IsFalse(container.TryGetExport(null, out bus));
        }

        public abstract class BaseController
        {
            [Import]
            public IBus Bus { get; set; }
        }

        [Export]
        public class HomeController : BaseController
        {
        }

        [TestMethod]
        public void SatisfiesImportsAppliedToBase()
        {
            var container = CreateContainer(typeof(HomeController), typeof(CloudBus));
            var hc = container.GetExport<HomeController>();
            Assert.IsInstanceOfType(hc.Bus, typeof(CloudBus));
        }

        class CustomImportAttribute : ImportAttribute { }

        [Export]
        public class MultipleImportsOnProperty
        {
            [Import, CustomImport]
            public string MultiImport { get; set; }
        }

        [TestMethod]
        public void MultipleImportAttributesAreDetected()
        {
            var c = new ContainerConfiguration()
                .WithPart<MultipleImportsOnProperty>()
                .CreateContainer();

            var x = AssertX.Throws<CompositionFailedException>(() => c.GetExport<MultipleImportsOnProperty>());
            Assert.AreEqual("Multiple imports have been configured for 'MultiImport'. At most one import can be applied to a single site.", x.Message);
        }
    }
}
