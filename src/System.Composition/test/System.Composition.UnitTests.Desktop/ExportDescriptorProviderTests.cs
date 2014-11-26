﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
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
namespace System.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class ExportDescriptorProviderTests
    {
        public class DefaultObjectExportDescriptorProvider : ExportDescriptorProvider
        {
            private CompositionContract _supportedContract = new CompositionContract(typeof(object));

            public static readonly object DefaultObject = "Hello, World!";

            public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
            {
                if (!contract.Equals(_supportedContract))
                    return NoExportDescriptors;

                var implementations = descriptorAccessor.ResolveDependencies("test for existing", contract, false);
                if (implementations.Any())
                    return NoExportDescriptors;

                return new[] { new ExportDescriptorPromise(contract, "test metadataProvider", false, NoDependencies, _ => ExportDescriptor.Create((c, o) => DefaultObject, NoMetadata)) };
            }
        }

        public class ExportsObject
        {
            [Export]
            public object AnObject { get { return "Not the default"; } }
        }

        [TestMethod]
        public void ProvidersCanLocateImplementationsOfAContractItSupports()
        {
            var container = new ContainerConfiguration()
                .WithProvider(new DefaultObjectExportDescriptorProvider())
                .WithPart<ExportsObject>()
                .CreateContainer();

            var o = container.GetExport<object>();
            Assert.AreNotEqual(DefaultObjectExportDescriptorProvider.DefaultObject, o);
        }

        [TestMethod]
        public void ProvidersCanDetectAbsenceOfAContractItSupports()
        {
            var container = new ContainerConfiguration()
                .WithProvider(new DefaultObjectExportDescriptorProvider())
                .CreateContainer();

            var o = container.GetExport<object>();
            Assert.AreEqual(DefaultObjectExportDescriptorProvider.DefaultObject, o);
        }
    }
}
