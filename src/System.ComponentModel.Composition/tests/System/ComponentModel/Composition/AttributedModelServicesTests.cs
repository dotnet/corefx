// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition.Diagnostics;
using System.ComponentModel.Composition.Factories;
using System.Diagnostics;
using System.Linq;
using System.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.Composition.Extensibility;
using System.ComponentModel.Composition.Primitives;
using Xunit;

namespace System.ComponentModel.Composition.AttributedModel
{
    class ConcreteCPD : ComposablePartDefinition
    {
        public override ComposablePart CreatePart()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class CPDTest {}
    
    public class AttributedModelServicesTests
    {
        [Fact]
        public void CreatePartDefinition1_NullAsType_ShouldThrowArgumentNull()
        {
            var origin = ElementFactory.Create();

            ExceptionAssert.ThrowsArgumentNull("type", () =>
            {
                AttributedModelServices.CreatePartDefinition((Type)null, origin);
            });
        }

        [Fact]
        public void CreatePartDefinition2_NullAsType_ShouldThrowArgumentNull()
        {
            var origin = ElementFactory.Create();

            ExceptionAssert.ThrowsArgumentNull("type", () =>
            {
                AttributedModelServices.CreatePartDefinition((Type)null, origin, false);
            });
        }

[Fact]
        public void CreatePart_From_InvalidPartDefiniton_ShouldThrowArgumentException()
        {
            ExceptionAssert.ThrowsArgument("partDefinition", () =>
            {
                try
                {
                    var partDefinition = new ConcreteCPD();
                    var instance = new CPDTest();
                    var part = AttributedModelServices.CreatePart(partDefinition, instance);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
               
            });
        }

#if FEATURE_TRACING

        [Fact]
        public void CreatePartDefinition2_TypeMarkedWithPartNotDiscoverableAttribute_ShouldTraceInformation()
        {
            var types = GetTypesMarkedWithPartNotDiscoverableAttribute();

            foreach (Type type in types)
            {
                using (TraceContext context = new TraceContext(SourceLevels.Information))
                {
                    AttributedModelServices.CreatePartDefinition(type, null, true);

                    Assert.NotNull(context.LastTraceEvent);
                    Assert.Equal(context.LastTraceEvent.EventType, TraceEventType.Information);
                    Assert.Equal(context.LastTraceEvent.Id, TraceId.Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute);
                }
            }
        }

        [Fact]
        public void CreatePartDefinition2_OpenGenericType_ShouldTraceInformation()
        {
            var types = GetOpenGenericTypesWithMismatchedArity();

            foreach (Type type in types)
            {
                using (TraceContext context = new TraceContext(SourceLevels.Information))
                {
                    AttributedModelServices.CreatePartDefinition(type, null, true);

                    Assert.NotNull(context.LastTraceEvent);
                    Assert.Equal(context.LastTraceEvent.EventType, TraceEventType.Information);
                    Assert.Equal(context.LastTraceEvent.Id, TraceId.Discovery_DefinitionMismatchedExportArity);
                }
            }
        }

        [Fact]
        public void CreatePartDefinition2_TypeWithNoExports_ShouldTraceInformation()
        {
            var types = GetTypesWithNoExports();

            foreach (Type type in types)
            {
                using (TraceContext context = new TraceContext(SourceLevels.Information))
                {
                    var result = AttributedModelServices.CreatePartDefinition(type, null, true);

                    Assert.NotNull(context.LastTraceEvent);
                    Assert.Equal(context.LastTraceEvent.EventType, TraceEventType.Information);
                    Assert.Equal(context.LastTraceEvent.Id, TraceId.Discovery_DefinitionContainsNoExports);
                }
            }
        }

        private static IEnumerable<Type> GetTypesMarkedWithPartNotDiscoverableAttribute()
        {
            yield return typeof(ClassMarkedWithPartNotDiscoverableAttribute);
            yield return typeof(ClassMarkedWithPartNotDiscoverableAttribute<>);
        }

        private static IEnumerable<Type> GetOpenGenericTypesWithMismatchedArity()
        {
            yield return typeof(GenericTypeWithMismatchedArity<,>);
        }

        private static IEnumerable<Type> GetTypesWithNoExports()
        {
            yield return typeof(ClassWithNoExports);
            yield return typeof(ClassWithOnlyFieldImport);
            yield return typeof(ClassWithOnlyPropertyImport);
            yield return typeof(ClassWithOnlyParameterImport);
        }

        public class ClassWithNoExports
        {
        }

        public class ClassWithOnlyFieldImport
        {
            [Import]
            public string Field;
        }

        public class ClassWithOnlyPropertyImport
        {
            [Import]
            public string Property
            {
                get;
                set;
            }
        }

        public class ClassWithOnlyParameterImport
        {
            [ImportingConstructor]
            public ClassWithOnlyParameterImport(string parameter)
            {
            }
        }

        public class GenericTypeWithMismatchedArity<T1, T2>
        {
            [Export]
            public IEnumerable<T1> MismatchedArityExport { get; set; }
        }

        [PartNotDiscoverable]
        public class ClassMarkedWithPartNotDiscoverableAttribute
        {
        }

        [PartNotDiscoverable]
        public class ClassMarkedWithPartNotDiscoverableAttribute<T>
        {
        }
#endif //FEATURE_TRACING

        [Fact]
        public void Exports_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            Type contractType = typeof(IContract1);
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.Exports(contractType);
            });
        }

        [Fact]
        public void Exports_Throws_OnNullContractType()
        {
            ComposablePartDefinition part = typeof(PartExportingContract1).AsPart();
            Type contractType = null;
            ExceptionAssert.ThrowsArgumentNull("contractType", () =>
            {
                part.Exports(contractType);
            });
        }

        [Fact]
        public void Exports()
        {
            ComposablePartDefinition part1 = typeof(PartExportingContract1).AsPart();
            ComposablePartDefinition part2 = typeof(PartExportingContract2).AsPart();

            Assert.True(part1.Exports(typeof(IContract1)));
            Assert.True(part2.Exports(typeof(IContract2)));

            Assert.False(part2.Exports(typeof(IContract1)));
            Assert.False(part1.Exports(typeof(IContract2)));
        }

        [Fact]
        public void ExportsGeneric_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.Exports<IContract1>();
            });
        }

        [Fact]
        public void ExportsGeneric()
        {
            ComposablePartDefinition part1 = typeof(PartExportingContract1).AsPart();
            ComposablePartDefinition part2 = typeof(PartExportingContract2).AsPart();

            Assert.True(part1.Exports<IContract1>());
            Assert.True(part2.Exports<IContract2>());

            Assert.False(part2.Exports<IContract1>());
            Assert.False(part1.Exports<IContract2>());
        }

        [Fact]
        public void Imports_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            Type contractType = typeof(IContract1);
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.Imports(contractType);
            });
        }

        [Fact]
        public void Imports_Throws_OnNullContractName()
        {
            ComposablePartDefinition part = typeof(PartImportingContract1).AsPart();
            Type contractType = null;
            ExceptionAssert.ThrowsArgumentNull("contractType", () =>
            {
                part.Imports(contractType);
            });
        }

        [Fact]
        public void Imports()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part2 = typeof(PartImportingContract2).AsPart();

            Assert.True(part1.Imports(typeof(IContract1)));
            Assert.True(part2.Imports(typeof(IContract2)));

            Assert.False(part2.Imports(typeof(IContract1)));
            Assert.False(part1.Imports(typeof(IContract2)));
        }

        [Fact]
        public void Imports_CardinalityIgnored_WhenNotSpecified()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part1Multiple = typeof(PartImportingContract1Multiple).AsPart();
            ComposablePartDefinition part1Optional = typeof(PartImportingContract1Optionally).AsPart();

            Assert.True(part1.Imports(typeof(IContract1)));
            Assert.True(part1Optional.Imports(typeof(IContract1)));
            Assert.True(part1Multiple.Imports(typeof(IContract1)));
        }

        [Fact]
        public void Imports_CardinalityNotIgnored_WhenSpecified()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part1Multiple = typeof(PartImportingContract1Multiple).AsPart();
            ComposablePartDefinition part1Optional = typeof(PartImportingContract1Optionally).AsPart();

            Assert.True(part1.Imports(typeof(IContract1), ImportCardinality.ExactlyOne));
            Assert.False(part1.Imports(typeof(IContract1), ImportCardinality.ZeroOrMore));
            Assert.False(part1.Imports(typeof(IContract1), ImportCardinality.ZeroOrOne));

            Assert.False(part1Multiple.Imports(typeof(IContract1), ImportCardinality.ExactlyOne));
            Assert.True(part1Multiple.Imports(typeof(IContract1), ImportCardinality.ZeroOrMore));
            Assert.False(part1Multiple.Imports(typeof(IContract1), ImportCardinality.ZeroOrOne));

            Assert.False(part1Optional.Imports(typeof(IContract1), ImportCardinality.ExactlyOne));
            Assert.False(part1Optional.Imports(typeof(IContract1), ImportCardinality.ZeroOrMore));
            Assert.True(part1Optional.Imports(typeof(IContract1), ImportCardinality.ZeroOrOne));
        }

        [Fact]
        public void ImportsGeneric_Throws_OnNullPart()
        {
            ComposablePartDefinition part = null;
            ExceptionAssert.ThrowsArgumentNull("part", () =>
            {
                part.Imports<IContract1>();
            });
        }

[Fact]
        public void ImportsGeneric()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part2 = typeof(PartImportingContract2).AsPart();

            Assert.True(part1.Imports<IContract1>());
            Assert.True(part2.Imports<IContract2>());

            Assert.False(part2.Imports<IContract1>());
            Assert.False(part1.Imports<IContract2>());
        }

        [Fact]
        public void ImportsGeneric_CardinalityIgnored_WhenNotSpecified()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part1Multiple = typeof(PartImportingContract1Multiple).AsPart();
            ComposablePartDefinition part1Optional = typeof(PartImportingContract1Optionally).AsPart();

            Assert.True(part1.Imports<IContract1>());
            Assert.True(part1Optional.Imports<IContract1>());
            Assert.True(part1Multiple.Imports<IContract1>());
        }

        [Fact]
        public void ImportsGeneric_CardinalityNotIgnored_WhenSpecified()
        {
            ComposablePartDefinition part1 = typeof(PartImportingContract1).AsPart();
            ComposablePartDefinition part1Multiple = typeof(PartImportingContract1Multiple).AsPart();
            ComposablePartDefinition part1Optional = typeof(PartImportingContract1Optionally).AsPart();

            Assert.True(part1.Imports<IContract1>(ImportCardinality.ExactlyOne));
            Assert.False(part1.Imports<IContract1>(ImportCardinality.ZeroOrMore));
            Assert.False(part1.Imports<IContract1>(ImportCardinality.ZeroOrOne));

            Assert.False(part1Multiple.Imports<IContract1>(ImportCardinality.ExactlyOne));
            Assert.True(part1Multiple.Imports<IContract1>(ImportCardinality.ZeroOrMore));
            Assert.False(part1Multiple.Imports<IContract1>(ImportCardinality.ZeroOrOne));

            Assert.False(part1Optional.Imports<IContract1>(ImportCardinality.ExactlyOne));
            Assert.False(part1Optional.Imports<IContract1>(ImportCardinality.ZeroOrMore));
            Assert.True(part1Optional.Imports<IContract1>(ImportCardinality.ZeroOrOne));
        }

        public interface IContract1
        {
        }

        public interface IContract2
        {
        }

        [Export(typeof(IContract1))]
        public class PartExportingContract1 : IContract1
        {
        }

        [Export(typeof(IContract2))]
        public class PartExportingContract2 : IContract2
        {
        }

        public class PartImportingContract1
        {
            [Import]
            public IContract1 import;
        }

        public class PartImportingContract2
        {
            [Import]
            public IContract2 import;
        }

        public class PartImportingContract1Optionally
        {
            [Import(AllowDefault = true)]
            public IContract1 import;
        }

        public class PartImportingContract1Multiple
        {
            [ImportMany]
            public IEnumerable<IContract1> import;
        }
    }
}
