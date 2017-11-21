// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.Reflection;
using Xunit;

namespace System.ComponentModel.Composition.Hosting
{
    public class AssemblyCatalogDebuggerProxyTests
    {
        [Fact]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new AssemblyCatalogDebuggerProxy((AssemblyCatalog)null);
            });
        }

        [Fact]
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

        [Fact]
        public void Constructor_ValueAsCatalogArgument_ShouldSetAssemblyProperty()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = CreateAssemblyCatalog(e);

                var proxy = new AssemblyCatalogDebuggerProxy(catalog);

                Assert.Same(catalog.Assembly, proxy.Assembly);
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
