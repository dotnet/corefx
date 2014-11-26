// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Composition.UnitTests.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Composition.Demos.ExtendedCollectionImports;
using Microsoft.Composition.Demos.ExtendedCollectionImports.OrderedCollections;
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
    public class ImportOrderingTests
    {
        public interface IItem
        {
        }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 1)]
        public class Item1 : IItem { }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 4)]
        public class Item4 : IItem { }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 2)]
        public class Item2 : IItem { }

        [Shared, Export(typeof(IItem)), ExportMetadata("Order", 3)]
        public class Item3 : IItem { }

        [Export(typeof(IItem))]
        public class ItemWithoutOrder : IItem { }

        [Export]
        public class HasImportedItems
        {
            [OrderedImportMany("Order")]
            public IItem[] OrderedItems { get; set; }

            [ImportMany]
            public IItem[] UnorderedItems { get; set; }
        }

        [TestMethod]
        public void CollectionsImportedWithAnOrderingAttributeComeInOrder()
        {
            var container = CreateExtendedContainer(typeof(HasImportedItems), typeof(Item1), typeof(Item4), typeof(Item2), typeof(Item3));

            var hasImportedItems = container.GetExport<HasImportedItems>();

            var ordered = hasImportedItems.UnorderedItems.OrderBy(i => i.GetType().Name).ToArray();

            CollectionAssert.AreEqual(ordered, hasImportedItems.OrderedItems);
            CollectionAssert.AreNotEqual(ordered, hasImportedItems.UnorderedItems);
        }

        [TestMethod]
        public void IfAnItemIsMissingMetadataAnInformativeExceptionIsThrown()
        {
            var container = CreateExtendedContainer(typeof(HasImportedItems), typeof(Item1), typeof(ItemWithoutOrder));
            var x = AssertX.Throws<CompositionFailedException>(() => container.GetExport<HasImportedItems>());
            Assert.AreEqual("The metadata 'Order' cannot be used for ordering because it is missing from exports on part(s) 'ItemWithoutOrder'.", x.Message);
        }

        private CompositionContext CreateExtendedContainer(params Type[] partTypes)
        {
            return new ContainerConfiguration()
                .WithParts(partTypes)
                .WithProvider(new OrderedImportManyExportDescriptorProvider())
                .CreateContainer();
        }
    }
}
