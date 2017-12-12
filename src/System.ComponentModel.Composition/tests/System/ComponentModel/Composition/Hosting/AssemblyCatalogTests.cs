// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.UnitTesting;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition
{
#if FEATURE_REFLECTIONCONTEXT
    // This is a glorious do nothing ReflectionContext
    public class AssemblyCatalogTestsReflectionContext : ReflectionContext
    {
        public override Assembly MapAssembly(Assembly assembly)
        {
            return assembly;
        }

#if FEATURE_INTERNAL_REFLECTIONCONTEXT
        public override Type MapType(Type type)
#else
        public override TypeInfo MapType(TypeInfo type)
#endif
        {
            return type;
        }
    }
#endif //FEATURE_REFLECTIONCONTEXT


    public class AssemblyCatalogTestsHelper
    {
        protected string GetAttributedAssemblyCodeBase()
        {
            return Assembly.GetExecutingAssembly().CodeBase;
        }

        protected Assembly GetAttributedAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        protected AssemblyCatalog CreateAssemblyCatalog()
        {
            return CreateAssemblyCatalog(GetAttributedAssembly());
        }

        protected AssemblyCatalog CreateAssemblyCatalog(Assembly assembly)
        {
            return new AssemblyCatalog(assembly);
        }

        protected class DerivedAssemblyCatalog : AssemblyCatalog
        {
            public DerivedAssemblyCatalog(Assembly assembly)
                : base(assembly)
            {
            }
        }
    }


    [TestClass]
    public class AssemblyCatalogConstructorTests : AssemblyCatalogTestsHelper
    {
#if FEATURE_REFLECTIONFILEIO
        // Test Codebase variant of the APIs
        public static void Constructor_ValueAsCodebaseArgument_ShouldSetAssemblyProperty(Func<string, AssemblyCatalog> catalogCreator)
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = catalogCreator(e.CodeBase);

                Assert.AreSame(e, catalog.Assembly);
            }
        }

        public static void Constructor_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad(Func<string, AssemblyCatalog> catalogCreator)
        {
            using (TemporaryFile file = new TemporaryFile())
            {
                using (FileStream stream = new FileStream(file.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    ExceptionAssert.Throws<FileLoadException>(() =>
                    {
                        var catalog = catalogCreator(file.FileName);
                    });
                }
            }
        }

        public static void Constructor_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull(Func<string, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("codeBase", () =>
            {
                var catalog = catalogCreator((string)null);
            });
        }

        public static void Constructor_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument(Func<string, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentException>("codeBase", () =>
            {
                var catalog = catalogCreator("");
            });
        }

        public static void Constructor_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument(Func<string, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.Throws<ArgumentException>(() =>
            {
                var catalog = catalogCreator("??||>");
            });
        }

        public static void Constructor_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad(Func<string, AssemblyCatalog> catalogCreator)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.System);
            Assert.IsTrue(Directory.Exists(directory));

            ExceptionAssert.Throws<FileLoadException>(() =>
            {
                var catalog = catalogCreator(directory);
            });
        }

        public static void Constructor_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong(Func<string, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.Throws<PathTooLongException>(() =>
            {
                var catalog = catalogCreator(@"c:\This is a very long path\And Just to make sure\We will continue to make it very long\This is a very long path\And Just to make sure\We will continue to make it very long\This is a very long path\And Just to make sure\We will continue to make it very long\myassembly.dll");
            });
        }

        public static void Constructor_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat(Func<string, AssemblyCatalog> catalogCreator)
        {
            using (TemporaryFile temporaryFile = new TemporaryFile())
            {
                ExceptionAssert.Throws<BadImageFormatException>(() =>
                {
                    var catalog = catalogCreator(temporaryFile.FileName);
                });
            }
        }

        public static void Constructor_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound(Func<string, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.Throws<FileNotFoundException>(() =>
            {
                var catalog = catalogCreator(@"FileThat should not ever exist");
            });
        }
#endif //FEATURE_REFLECTIONFILEIO

        // Test Assembly variant of the APIs
        public static void Constructor_ValueAsAssemblyArgument_ShouldSetAssemblyProperty(Func<Assembly, AssemblyCatalog> catalogCreator)
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = catalogCreator(e);

                Assert.AreSame(e, catalog.Assembly);
            }
        }

#if FEATURE_REFLECTIONCONTEXT
        public static void Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull(Func<ReflectionContext, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("reflectionContext", () =>
            {
                var catalog = catalogCreator(null);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT


        public static void Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull(Func<ICompositionElement, AssemblyCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definitionOrigin", () =>
            {
                var catalog = catalogCreator(null);
            });
        }

#if FEATURE_REFLECTIONFILEIO
        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(string codebase) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor1_ValueAsCodebaseArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsCodebaseArgument_ShouldSetAssemblyProperty((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong()
        {
            AssemblyCatalogConstructorTests.Constructor_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat()
        {
            AssemblyCatalogConstructorTests.Constructor_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound()
        {
            AssemblyCatalogConstructorTests.Constructor_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound((s) =>
            {
                return new AssemblyCatalog(s);
            });
        }

        [TestMethod]
        public void Constructor1_ShouldSetOriginToNull()
        {
            var catalog = (ICompositionElement)new AssemblyCatalog(GetAttributedAssemblyCodeBase());

            Assert.IsNull(catalog.Origin);
        }

#if FEATURE_REFLECTIONCONTEXT
        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(string codebase, ReflectionContext reflectionContext) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor2_ValueAsCodebaseArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsCodebaseArgument_ShouldSetAssemblyProperty((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }


        [TestMethod]
        public void Constructor2_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong()
        {
            AssemblyCatalogConstructorTests.Constructor_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat()
        {
            AssemblyCatalogConstructorTests.Constructor_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound()
        {
            AssemblyCatalogConstructorTests.Constructor_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor2_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new AssemblyCatalog(GetAttributedAssemblyCodeBase(), rc);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT

        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(string codebase, ICompositionElement definitonOrigin) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor3_ValueAsCodebaseArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsCodebaseArgument_ShouldSetAssemblyProperty((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong()
        {
            AssemblyCatalogConstructorTests.Constructor_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat()
        {
            AssemblyCatalogConstructorTests.Constructor_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound()
        {
            AssemblyCatalogConstructorTests.Constructor_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound((s) =>
            {
                return new AssemblyCatalog(s, (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor3_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new AssemblyCatalog(GetAttributedAssemblyCodeBase(), dO);
            });
        }

#if FEATURE_REFLECTIONCONTEXT
        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(string codebase, ICompositionElement definitonOrigin, ReflectionContext reflectionContext) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor4_ValueAsCodebaseArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsAssemblyArgument_ShouldSetAssemblyProperty((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_LockedFileAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullFileNameAsCodeBaseArgument_ShouldThrowArgumentNull((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_EmptyFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument()
        {
            AssemblyCatalogConstructorTests.Constructor_InvalidFileNameAsCodeBaseArgument_ShouldThrowArgument((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad()
        {
            AssemblyCatalogConstructorTests.Constructor_DirectoryAsCodeBaseArgument_ShouldThrowFileLoad((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong()
        {
            AssemblyCatalogConstructorTests.Constructor_TooLongFileNameAsCodeBaseArgument_ShouldThrowPathTooLong((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat()
        {
            AssemblyCatalogConstructorTests.Constructor_NonAssemblyFileNameAsCodeBaseArgument_ShouldThrowBadImageFormat((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound()
        {
            AssemblyCatalogConstructorTests.Constructor_NonExistentFileNameAsCodeBaseArgument_ShouldThrowFileNotFound((s) =>
            {
                return new AssemblyCatalog(s, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(s));
            });
        }

        [TestMethod]
        public void Constructor4_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new AssemblyCatalog(GetAttributedAssemblyCodeBase(), rc, (ICompositionElement)new AssemblyCatalog(GetAttributedAssembly()));
            });
        }

        [TestMethod]
        public void Constructor4_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new AssemblyCatalog(GetAttributedAssemblyCodeBase(), new AssemblyCatalogTestsReflectionContext(), dO);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT
        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(string codebase, ICompositionElement definitonOrigin) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor7_ValueAsAssemblyArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsCodebaseArgument_ShouldSetAssemblyProperty((a) =>
            {
                return new AssemblyCatalog(a, (ICompositionElement)new AssemblyCatalog(GetAttributedAssembly()));
            });
        }
#endif //FEATURE_REFLECTIONFILEIO


        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(Assembly assembly) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor5_ValueAsAssemblyArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsAssemblyArgument_ShouldSetAssemblyProperty((a) =>
            {
                return new AssemblyCatalog(a);
            });
        }

#if FEATURE_REFLECTIONCONTEXT
        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(Assembly assembly, ReflectionContext reflectionContext) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor6_ValueAsAssemblyArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsAssemblyArgument_ShouldSetAssemblyProperty((a) =>
            {
                return new AssemblyCatalog(a, new AssemblyCatalogTestsReflectionContext());
            });
        }

        [TestMethod]
        public void Constructor6_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new AssemblyCatalog(GetAttributedAssembly(), rc);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT

        [TestMethod]
        public void Constructor7_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new AssemblyCatalog(GetAttributedAssembly(), dO);
            });
        }

        [TestMethod]
        public void Constructor7_ShouldSetOriginToNull()
        {
            var catalog = (ICompositionElement)new AssemblyCatalog(GetAttributedAssembly());

            Assert.IsNull(catalog.Origin);
        }

#if FEATURE_REFLECTIONCONTEXT
        //=========================================================================================================================================
        //  Test cases for AssemblyCatalog(string codebase, ICompositionElement definitonOrigin, ReflectionContext reflectionContext) constructor
        //=========================================================================================================================================
        [TestMethod]
        public void Constructor8_ValueAsAssemblyArgument_ShouldSetAssemblyProperty()
        {
            AssemblyCatalogConstructorTests.Constructor_ValueAsAssemblyArgument_ShouldSetAssemblyProperty((a) =>
            {
                return new AssemblyCatalog(a, new AssemblyCatalogTestsReflectionContext(), (ICompositionElement)new AssemblyCatalog(GetAttributedAssembly()));
            });
        }

#if FEATURE_FILEIO
        [TestMethod]
        public void Constructor8_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new AssemblyCatalog(GetAttributedAssemblyCodeBase(), rc, (ICompositionElement)new AssemblyCatalog(GetAttributedAssembly()));
            });
        }

        [TestMethod]
        public void Constructor8_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            AssemblyCatalogConstructorTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new AssemblyCatalog(GetAttributedAssembly().CodeBase, new AssemblyCatalogTestsReflectionContext(), dO);
            });
        }
#endif //FEATURE_FILEIO
        //=========================================================================================================================================
        //  Test cases for Assemblies decorated with the CatalogDiscoveryAttribute
        //=========================================================================================================================================

        [TestMethod]
        public void DiscoverCatalogUsingReflectionContextCatalogDiscoveryAttribute()
        {
            var catalog = new AssemblyCatalog(typeof(TestAssemblyOne).Assembly);
            Assert.IsTrue(catalog.Parts.Count() > 0);

            foreach (ICompositionElement definition in catalog.Parts)
            {
                Assert.AreSame(catalog, definition.Origin);
            }
        }

        [TestMethod]
        public void DiscoverCatalogUsingDerivedReflectionContextCatalogDiscoveryAttribute()
        {
            var catalog = new AssemblyCatalog(typeof(TestAssemblyTwo).Assembly);
            Assert.IsTrue(catalog.Parts.Count() > 0);

            foreach (ICompositionElement definition in catalog.Parts)
            {
                Assert.AreSame(catalog, definition.Origin);
            }
        }

        [TestMethod]
        public void DiscoverCatalogUsingNoDefaultConstructorReflectionContextCatalogDiscoveryAttribute_ShouldThrowArgumentException()
        {

            ExceptionAssert.Throws<MissingMethodException>(() =>
            {
                var catalog = new AssemblyCatalog(typeof(TestAssemblyThree).Assembly);
                Assert.IsTrue(catalog.Parts.Count() > 0);
            });  
        }

        [TestMethod]
        public void DiscoverCatalogUsingDerivedReflectionContextCatalogDiscoveryAttribute_ShouldThrowArgumentException()
        {

            ExceptionAssert.Throws<InvalidOperationException>(() =>
            {
                var catalog = new AssemblyCatalog(typeof(TestAssemblyFour).Assembly);
                Assert.IsTrue(catalog.Parts.Count() > 0);
            });  
        }
#endif //FEATURE_REFLECTIONCONTEXT
    }


    [TestClass]
    public class AssemblyCatalogTests : AssemblyCatalogTestsHelper
    {
        [TestMethod]
        public void Assembly_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();

            Assert.IsNotNull(catalog.Assembly);
        }

        [TestMethod]
        public void ICompositionElementDisplayName_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();

            var displayName = ((ICompositionElement)catalog).DisplayName;
        }

        [TestMethod]
        public void ICompositionElementOrigin_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();

            var origin = ((ICompositionElement)catalog).Origin;
        }

        [TestMethod]
        public void ToString_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();

            catalog.ToString();
        }
        
        [TestMethod]
        public void Parts_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var parts = catalog.Parts;
            });
        }

        [TestMethod]
        public void GetExports_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();
            var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                catalog.GetExports(definition);
            });
        }

        [TestMethod]
        public void GetExports_NullAsConstraintArgument_ShouldThrowArgumentNull()
        {
            var catalog = CreateAssemblyCatalog();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                catalog.GetExports((ImportDefinition)null);
            });
        }


        [TestMethod]
        public void Dispose_ShouldNotThrow()
        {
            using (var catalog = CreateAssemblyCatalog())
            {
            }
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var catalog = CreateAssemblyCatalog();
            catalog.Dispose();
            catalog.Dispose();
            catalog.Dispose();
        }

        [TestMethod]
        public void Parts()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            Assert.IsNotNull(catalog.Parts);
            Assert.IsTrue(catalog.Parts.Count()>0);
        }


        [TestMethod]
        public void Parts_ShouldSetDefinitionOriginToCatalogItself()
        {
            var catalog = CreateAssemblyCatalog();
            Assert.IsTrue(catalog.Parts.Count() > 0);

            foreach (ICompositionElement definition in catalog.Parts)
            {
                Assert.AreSame(catalog, definition.Origin);
            }
        }

        [TestMethod]
        public void GetExports()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            Expression<Func<ExportDefinition, bool>> constraint = (ExportDefinition exportDefinition) => exportDefinition.ContractName == AttributedModelServices.GetContractName(typeof(MyExport));
            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> matchingExports = catalog.GetExports(constraint);
            Assert.IsNotNull(matchingExports);
            Assert.IsTrue(matchingExports.Count() >= 0);

            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> expectedMatchingExports = catalog.Parts
                .SelectMany(part => part.ExportDefinitions, (part, export) => new Tuple<ComposablePartDefinition, ExportDefinition>(part, export))
                .Where(partAndExport => partAndExport.Item2.ContractName == AttributedModelServices.GetContractName(typeof(MyExport)));
            Assert.IsTrue(matchingExports.SequenceEqual(expectedMatchingExports));
        }

#if FEATURE_REFLECTIONFILEIO
        [TestMethod]
        public void AddAssemblyUsingFile()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly.Location);
            var container = new CompositionContainer(catalog);

            Assert.IsNotNull(container.GetExportedValue<MyExport>());
        }
#endif

        [TestMethod]
        public void TwoTypesWithSameSimpleName()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);

            NotSoUniqueName unique1 = container.GetExportedValue<NotSoUniqueName>();
            Assert.IsNotNull(unique1);

            Assert.AreEqual(23, unique1.MyIntProperty);

            NotSoUniqueName2.NotSoUniqueName nestedUnique = container.GetExportedValue<NotSoUniqueName2.NotSoUniqueName>();

            Assert.IsNotNull(nestedUnique);
            Assert.AreEqual("MyStringProperty", nestedUnique.MyStringProperty);
        }

        [TestMethod]
        public void GettingFunctionExports()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);

            ImportDefaultFunctions import = container.GetExportedValue<ImportDefaultFunctions>("ImportDefaultFunctions");
            import.VerifyIsBound();
        }

        [TestMethod]
        public void AnExportOfAnInstanceThatFailsToCompose()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);

            // Rejection causes the part in the catalog whose imports cannot be
            // satisfied to be ignored, resulting in a cardinality mismatch instead of a
            // composition exception
            ExceptionAssert.Throws<ImportCardinalityMismatchException>(() =>
            {
                container.GetExportedValue<string>("ExportMyString");
            });
        }

        [TestMethod]
        public void SharedPartCreation()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new Int32Exporter(41));
            container.Compose(batch);

            var sharedPart1 = container.GetExportedValue<MySharedPartExport>();
            Assert.AreEqual(41, sharedPart1.Value);
            var sharedPart2 = container.GetExportedValue<MySharedPartExport>();
            Assert.AreEqual(41, sharedPart2.Value);

            Assert.AreEqual(sharedPart1, sharedPart2, "These should be the same instances");
        }

        [TestMethod]
        public void NonSharedPartCreation()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(new Int32Exporter(41));
            container.Compose(batch);

            var nonSharedPart1 = container.GetExportedValue<MyNonSharedPartExport>();
            Assert.AreEqual(41, nonSharedPart1.Value);
            var nonSharedPart2 = container.GetExportedValue<MyNonSharedPartExport>();
            Assert.AreEqual(41, nonSharedPart2.Value);

            Assert.AreNotSame(nonSharedPart1, nonSharedPart2, "These should be different instances");
        }

        [TestMethod]
        public void RecursiveNonSharedPartCreation()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<DirectCycleNonSharedPart>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart1>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart2>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleWithSharedPartAndNonSharedPart>();
            });

            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart>());
            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart1>());
            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart2>());
            Assert.IsNotNull(container.GetExportedValue<NoCycleNonSharedPart>());
        }

        [TestMethod]
        public void RecursiveNonSharedPartCreationDisableSilentRejection()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<DirectCycleNonSharedPart>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart1>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleNonSharedPart2>();
            });

            CompositionAssert.ThrowsError(ErrorId.ImportEngine_PartCannotGetExportedValue, () =>
            {
                container.GetExportedValue<CycleWithSharedPartAndNonSharedPart>();
            });

            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart>());
            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart1>());
            Assert.IsNotNull(container.GetExportedValue<CycleSharedPart2>());
            Assert.IsNotNull(container.GetExportedValue<NoCycleNonSharedPart>());
        }

        [TestMethod]
        public void TryToDiscoverExportWithGenericParameter()
        {
            var catalog = new AssemblyCatalog(typeof(AssemblyCatalogTests).Assembly);
            var container = new CompositionContainer(catalog);

            // Should find a type that inherits from an export
            Assert.IsNotNull(container.GetExportedValueOrDefault<object>(AttributedModelServices.GetContractName(typeof(ExportWhichInheritsFromGeneric))));

            // This should be exported because it is inherited by ExportWhichInheritsFromGeneric
            Assert.IsNotNull(container.GetExportedValueOrDefault<object>(AttributedModelServices.GetContractName(typeof(ExportWithGenericParameter<string>))));
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldIncludeCatalogTypeNameAndAssemblyFullName()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)CreateAssemblyCatalog(e);

                string expected = string.Format("AssemblyCatalog (Assembly=\"{0}\")", e.FullName);

                Assert.AreEqual(expected, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldIncludeDerivedCatalogTypeNameAndAssemblyFullName()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)new DerivedAssemblyCatalog(e);

                string expected = string.Format("DerivedAssemblyCatalog (Assembly=\"{0}\")", e.FullName);

                Assert.AreEqual(expected, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnICompositionElementDisplayName()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                var catalog = (ICompositionElement)CreateAssemblyCatalog(e);

                Assert.AreEqual(catalog.DisplayName, catalog.ToString());
            }
        }
    }
}
