// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using Microsoft.CLR.UnitTesting;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;

namespace System.ComponentModel.Composition
{
    class ExportImplementer
    {
        [Export]
        public ExportOnIndexer this[int index]
        {
            get { return new ExportOnIndexer(); }
        }
    }

    public class ExportOnIndexer { }

    [TestClass]
    public class ExportAttributeTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetContractNamePropertyToEmptyString()
        {
            var attribute = new ExportAttribute();

            Assert.IsNull(attribute.ContractName);
            Assert.IsNull(attribute.ContractType);
        }

        [TestMethod]
        public void Constructor2_NullAsContractNameArgument_ShouldSetContractNamePropertyToEmptyString()
        {
            var attribute = new ExportAttribute((string)null);

            Assert.IsNull(attribute.ContractName);
            Assert.IsNull(attribute.ContractType);
        }

        [TestMethod]
        public void Constructor3_NullAsContractTypeArgument_ShouldSetContractNamePropertyToEmptyString()
        {
            var attribute = new ExportAttribute((Type)null);

            Assert.IsNull(attribute.ContractName);
            Assert.IsNull(attribute.ContractType);
        }

        [TestMethod]
        public void Constructor4_NullAsContractTypeArgument_ShouldSetContractNamePropertyToEmptyString()
        {
            var attribute = new ExportAttribute((string)null, (Type)null);

            Assert.IsNull(attribute.ContractName);
            Assert.IsNull(attribute.ContractType);
        }

        [TestMethod]
        public void Constructor2_ValueAsContractNameArgument_ShouldSetContractNameProperty()
        {
            var expectations = Expectations.GetContractNamesWithEmpty();
            
            foreach (var e in expectations)
            {
                var attribute = new ExportAttribute(e);

                Assert.AreEqual(e, attribute.ContractName);
            }
        }

        [TestMethod]
        public void ExportIndexers_ShouldThrowSomething()
        {
            var con = new CompositionContainer(
                new TypeCatalog(typeof(WorkingType), typeof(Constants), typeof(ExportImplementer), typeof(ExportOnIndexer))
            );

            var v1 = con.GetExportedValue<WorkingType>();

            ExceptionAssert.Throws<CompositionException>(RetryMode.DoNotRetry, () =>
            {
                var v2 = con.GetExportedValue<ExportOnIndexer>();
                Console.WriteLine(v2.ToString());
            });
        }
    
    }
}
