// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.Reflection;

namespace System.ComponentModel.Composition.Hosting
{
    [TestClass]
    public class AssemblyCatalogDebuggerProxyTests
    {
        [TestMethod]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new AssemblyCatalogDebuggerProxy((AssemblyCatalog)null);
            });
        }

        [TestMethod]
        public void Constructor_ValueAsCatalogArgument_ShouldSetPartsProperty()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = CreateAssemblyCatalog(e);

                var proxy = new AssemblyCatalogDebuggerProxy(catalog);

                EnumerableAssert.AreSequenceEqual(catalog.Parts, proxy.Parts);
            }
        }

        [TestMethod]
        public void Constructor_ValueAsCatalogArgument_ShouldSetAssemblyProperty()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = CreateAssemblyCatalog(e);

                var proxy = new AssemblyCatalogDebuggerProxy(catalog);

                Assert.AreSame(catalog.Assembly, proxy.Assembly);
            }
        }

        private AssemblyCatalogDebuggerProxy CreateAssemblyDebuggerProxy(AssemblyCatalog catalog)
        {
            return new AssemblyCatalogDebuggerProxy(catalog);
        }

        private AssemblyCatalog CreateAssemblyCatalog(Assembly assembly)
        {
            return new AssemblyCatalog(assembly);
        }
   }
}
