// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Composition.Hosting.Core;
using System.Composition;
using System.Reflection;
using Microsoft.Composition.Demos.Web.Mvc.UnitTests.MvcContainerConfigurationScenario.Parts;
using System.Web.Mvc;
using Microsoft.Composition.Demos.Web.Mvc.UnitTests.MvcContainerConfigurationScenario.Controllers;
using Microsoft.Composition.Demos.Web.Mvc.UnitTests.MvcContainerConfigurationScenario;
using Microsoft.Composition.Demos.Web.Mvc.UnitTests.Util;
using System.Composition.Runtime;
using System.Collections.Generic;
using System.Composition.Hosting;

namespace Microsoft.Composition.Demos.Web.Mvc.UnitTests
{
    namespace MvcContainerConfigurationScenario
    {
        namespace Parts
        {
        }

        namespace Controllers
        {
            public class SimpleController : Controller { }
        }
    }

    [TestClass]
    public class MvcContainerConfigurationTests
    {
        ExportFactory<CompositionContext> CreateRequestScopeFactory(params Type[] partTypes)
        {
            var container = ConfigureContainer(partTypes);

            var rsfcontract = new CompositionContract(typeof(ExportFactory<CompositionContext>), null, new Dictionary<string, object> {
                { "SharingBoundaryNames", new[] { Boundaries.HttpRequest, Boundaries.DataConsistency, Boundaries.UserIdentity }}
            });
            return (ExportFactory<CompositionContext>)container.GetExport(rsfcontract);
        }

        CompositionHost ConfigureContainer(params Type[] partTypes)
        {
            var configuration = new MvcContainerConfiguration(new Assembly[0])
                .WithParts(partTypes);

            return configuration.CreateContainer();
        }

        [TestMethod]
        public void ControllersAreExportedAndNonShared()
        {
            var rsf = CreateRequestScopeFactory(typeof(SimpleController));

            var r1 = rsf.CreateExport();
            var r1sp1 = r1.Value.GetExport<SimpleController>();
            var r1sp2 = r1.Value.GetExport<SimpleController>();

            Assert.AreNotSame(r1sp1, r1sp2);
        }
    }
}
