// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
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
namespace System.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class CustomerReportedMetadataBug
    {
        public class ServiceMetadata
        {
            public string Name { get; set; }
        }

        public interface IService
        {
        }

        [Export(typeof(IService)), ExportMetadata("Name", "1")]
        public class SampleService1 : IService
        {
            public SampleService1()
            {
            }
        }

        public class LooseImporter
        {
            [ImportMany]
            public IList<Lazy<IService, ServiceMetadata>> Services { get; set; }
        }

        [TestMethod]
        public void SampleServicesCorrectlyImported()
        {
            var container = new ContainerConfiguration()
                .WithPart<SampleService1>()
                .CreateContainer();

            var importer = new LooseImporter();
            container.SatisfyImports(importer);

            Assert.AreEqual(1, importer.Services.Count);
        }
    }
}
