// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Composition.Convention;
using Xunit;

namespace System.Composition.Hosting.Tests
{
    public class ContainerConfigurationTests
    {
        [Fact]
        public void CreateContainer_ExportedSubClass_Success()
        {
            CompositionHost container = new ContainerConfiguration()
                .WithPart(typeof(Derived))
                .CreateContainer();
            Assert.Equal("Derived", container.GetExport<Derived>().Prop);
        }

        [Export]
        public class Derived : Base
        {
            new public string Prop { get; set; } = "Derived";
        }

        public class Base
        {
            public object Prop { get; set; } = "Derived";
        }

        [Fact]
        public void CreateContainer_OpenGenericTypes_Success()
        {
            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IContainer>()
                .Export<IContainer>();
            conventions.ForTypesDerivedFrom(typeof(IRepository<>))
                .Export(t => t.AsContractType(typeof(IRepository<>)));

            CompositionHost container = new ContainerConfiguration()
                .WithParts(new Type[] { typeof(EFRepository<>), typeof(Container) }, conventions)
                .CreateContainer();
            Assert.Equal(0, container.GetExport<IRepository<int>>().Fetch());
        }

        public interface IContainer { }
        public class Container : IContainer { }

        public interface IRepository<T>
        {
            T Fetch();
        }

        public class EFRepository<T> : IRepository<T>
        {
            public EFRepository(IContainer test) { }
            public T Fetch() => default(T);
        }

        [Fact]
        public void CreateContainer_ImportConventionsWithInheritedProperties_Success()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<Imported>().Export();
            conventions.ForType<DerivedFromBaseWithImport>()
                .ImportProperty(b => b.Imported)
                .Export();

            CompositionHost container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(Imported), typeof(DerivedFromBaseWithImport))
                .CreateContainer();

            DerivedFromBaseWithImport export = container.GetExport<DerivedFromBaseWithImport>();
            Assert.IsAssignableFrom(typeof(Imported), export.Imported);
        }

        public class Imported { }

        public class BaseWithImport
        {
            public virtual Imported Imported { get; set; }
        }

        public class DerivedFromBaseWithImport : BaseWithImport { }

        [Fact]
        public void CreateContainer_ExportConventionsWithInheritedProperties_Success()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<DerivedFromBaseWithExport>()
                .ExportProperty(b => b.Exported);

            CompositionHost container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(DerivedFromBaseWithExport))
                .CreateContainer();
            Assert.Equal("A", container.GetExport<string>());
        }

        public class BaseWithExport
        {
            public string Exported { get { return "A"; } }
        }

        public class DerivedFromBaseWithExport : BaseWithExport { }

        [Fact]
        public void CreateContainer_ExportsToInheritedProperties_DontInterfereWithBase()
        {
            var conventions = new ConventionBuilder();
            conventions.ForType<DerivedFromBaseWithExport2>()
                .ExportProperty(b => b.Exported);

            CompositionHost container = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithParts(typeof(BaseWithExport2))
                .WithParts(typeof(DerivedFromBaseWithExport2))
                .CreateContainer();
            Assert.Equal(new string[] { "A", "A" }, container.GetExports<string>());
        }

        public class BaseWithExport2
        {
            [Export]
            public virtual string Exported => "A";
        }

        public class DerivedFromBaseWithExport2 : BaseWithExport { }

        [Fact]
        public void CreateContainer_HasConventions_ClassExportsAreNotInherited()
        {
            CompositionHost container = new ContainerConfiguration()
                .WithPart<DerivedFromBaseWithDeclaredExports>(new ConventionBuilder())
                .CreateContainer();
            Assert.False(container.TryGetExport(out BaseWithDeclaredExports export));
        }

        [Fact]
        public void CreateContainer_HasConventions_PropertyExportsAreNotInherited()
        {
            CompositionHost container = new ContainerConfiguration()
                .WithPart<DerivedFromBaseWithDeclaredExports>(new ConventionBuilder())
                .CreateContainer();
            Assert.False(container.TryGetExport(out string export));
        }

        [Export]
        public class BaseWithDeclaredExports
        {
            public BaseWithDeclaredExports() => Property = "foo";

            [Export]
            public string Property { get; set; }
        }

        public class DerivedFromBaseWithDeclaredExports : BaseWithDeclaredExports { }

        public class CustomExport : ExportAttribute { }

        [Fact]
        public void CreateContainer_HasConventions_CustomAttributesAreNotInherited()
        {
            CompositionHost container = new ContainerConfiguration()
                .WithPart<DerivedFromBaseWithCustomExport>(new ConventionBuilder())
                .CreateContainer();
            Assert.False(container.TryGetExport(out BaseWithCustomExport bce));
        }

        [CustomExport]
        public class BaseWithCustomExport { }

        public class DerivedFromBaseWithCustomExport : BaseWithCustomExport { }
    }
}
