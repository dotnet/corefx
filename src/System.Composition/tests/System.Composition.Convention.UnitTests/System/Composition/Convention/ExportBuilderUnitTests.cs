// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Convention.UnitTests;
using System.Reflection;
using System.Linq;
using System.Text;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif
namespace System.Composition.Convention
{
    [TestClass]
    public class ExportBuilderUnitTests
    {
        public interface IFoo { }

        public class CFoo : IFoo { }

        [TestMethod]
        public void ExportInterfaceWithTypeOf1()
        {
            var builder = new ConventionBuilder();
            builder.ForType<CFoo>().Export<IFoo>();

            var exports = builder.GetDeclaredAttributes(typeof(CFoo), typeof(CFoo).GetTypeInfo()).Where<Attribute>(e => e is ExportAttribute).Cast<ExportAttribute>();
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(exports.First().ContractType, typeof(IFoo));
        }

        [TestMethod]
        public void ExportInterfaceWithTypeOf2()
        {
            var builder = new ConventionBuilder();
            builder.ForType(typeof(CFoo)).Export((c) => c.AsContractType(typeof(IFoo)));

            var exports = builder.GetDeclaredAttributes(typeof(CFoo), typeof(CFoo).GetTypeInfo()).Where<Attribute>(e => e is ExportAttribute).Cast<ExportAttribute>();
            Assert.AreEqual(1, exports.Count());
            Assert.AreEqual(exports.First().ContractType, typeof(IFoo));
        }

        [TestMethod]
        public void ExportBuilderApiTestsNull_ShouldThrowArgumentNull()
        {
            var builder = new ConventionBuilder();
            ExceptionAssert.ThrowsArgumentNull("contractName", () => builder.ForTypesMatching((t) => true).Export(c => c.AsContractName(null as string)));
            ExceptionAssert.ThrowsArgument("contractName", () => builder.ForTypesMatching((t) => true).Export(c => c.AsContractName("")));
            ExceptionAssert.ThrowsArgumentNull("getContractNameFromPartType", () => builder.ForTypesMatching((t) => true).Export(c => c.AsContractName(null as Func<Type, string>)));
            ExceptionAssert.ThrowsArgumentNull("type", () => builder.ForTypesMatching((t) => true).Export(c => c.AsContractType(null as Type)));
            ExceptionAssert.ThrowsArgumentNull("name", () => builder.ForTypesMatching((t) => true).Export(c => c.AddMetadata(null as string, null as object)));
            ExceptionAssert.ThrowsArgumentNull("name", () => builder.ForTypesMatching((t) => true).Export(c => c.AddMetadata(null as string, null as Func<Type, object>)));
            ExceptionAssert.ThrowsArgumentNull("getValueFromPartType", () => builder.ForTypesMatching((t) => true).Export(c => c.AddMetadata("name", null as Func<Type, object>)));
        }
    }
}
