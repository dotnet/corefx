// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.UnitTests;
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
    public class LooseImportsTests : ContainerTests
    {
        [Export]
        public class Transaction { }

        public class SaveChangesAttribute
        {
            [Import]
            public Transaction Transaction { get; set; }
        }

        [TestMethod]
        public void SatisfyImportsSetsLooseImportsOnAttributedPart()
        {
            var container = CreateContainer(typeof(Transaction));
            var hasLoose = new SaveChangesAttribute();
            container.SatisfyImports(hasLoose);
            Assert.IsNotNull(hasLoose.Transaction);
        }
    }
}
