// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using Xunit;

namespace System.Composition.Tests
{
    public class CompositionContextTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExportT_Default_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Null(contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });
            if (success)
            {
                Assert.Equal(10, context.GetExport<int>());
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExport<int>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExportT_ContractName_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Equal("contractName", contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });
            if (success)
            {
                Assert.Equal(10, context.GetExport<int>("contractName"));
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExport<int>("contractName"));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TryGetExportT_Default_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Null(contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });
            Assert.Equal(success, context.TryGetExport(out int export));
            Assert.Equal(success ? 10 : 0, export);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TryGetExportT_ContractName_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Equal("contractName", contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });
            Assert.Equal(success, context.TryGetExport("contractName", out int export));
            Assert.Equal(success ? 10 : 0, export);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExport_ContractType_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Null(contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });
            if (success)
            {
                Assert.Equal(10, context.GetExport(typeof(int)));
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExport(typeof(int)));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExport_ContractTypeContractName_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Equal("contractName", contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });
            if (success)
            {
                Assert.Equal(10, context.GetExport(typeof(int), "contractName"));
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExport(typeof(int), "contractName"));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TryGetExport_ContractType_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Null(contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });

            // Failure leaks through.
            Assert.Equal(success, context.TryGetExport(typeof(int), out object export));
            Assert.Equal(10, export);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TryGetExport_ContractTypeContractName_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int), contract.ContractType);
                Assert.Equal("contractName", contract.ContractName);
                Assert.Null(contract.MetadataConstraints);

                return (success, 10);
            });

            // Failure leaks through.
            Assert.Equal(success, context.TryGetExport(typeof(int), "contractName", out object export));
            Assert.Equal(10, export);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExportsT_Default_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(object[]), contract.ContractType);
                Assert.Null(contract.ContractName);
                Assert.Equal(new Dictionary<string, object> { { "IsImportMany", true } }, contract.MetadataConstraints);

                return (success, new object[] { 10 });
            });
            if (success)
            {
                Assert.Equal(new object[] { 10 }, context.GetExports<object>());
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExports<object>());
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExportsT_ContractName_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(object[]), contract.ContractType);
                Assert.Equal("contractName", contract.ContractName);
                Assert.Equal(new Dictionary<string, object> { { "IsImportMany", true } }, contract.MetadataConstraints);

                return (success, new object[] { 10 });
            });
            if (success)
            {
                Assert.Equal(new object[] { 10 }, context.GetExports<object>("contractName"));
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExports<object>("contractName"));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExports_ContractType_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int[]), contract.ContractType);
                Assert.Null(contract.ContractName);
                Assert.Equal(new Dictionary<string, object> { { "IsImportMany", true } }, contract.MetadataConstraints);

                return (success, new object[] { 10 });
            });
            if (success)
            {
                Assert.Equal(new object[] { 10 }, context.GetExports(typeof(int)));
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExports(typeof(int)));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetExports_ContractTypeContractName_ReturnsExpected(bool success)
        {
            var context = new SubContext(contract =>
            {
                Assert.Equal(typeof(int[]), contract.ContractType);
                Assert.Equal("contractName", contract.ContractName);
                Assert.Equal(new Dictionary<string, object> { { "IsImportMany", true } }, contract.MetadataConstraints);

                return (success, new object[] { 10 });
            });
            if (success)
            {
                Assert.Equal(new object[] { 10 }, context.GetExports(typeof(int), "contractName"));
            }
            else
            {
                Assert.Throws<CompositionFailedException>(() => context.GetExports(typeof(int), "contractName"));
            }
        }

        public class SubContext : CompositionContext
        {
            public Func<CompositionContract, (bool, object)> Handler { get; }

            public SubContext(Func<CompositionContract, (bool, object)> handler) => Handler = handler;

            public override bool TryGetExport(CompositionContract contract, out object export)
            {
                (bool result, object exportResult) = Handler(contract);
                export = exportResult;
                return result;
            }
        }
    }
}
