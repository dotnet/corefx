// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Factories;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    // This is a glorious do nothing ReflectionContext
    public class DirectoryCatalogTestsReflectionContext : ReflectionContext
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

    [TestClass]
    public class DirectoryCatalogTests
    {
        internal const string NonExistentSearchPattern = "*.NonExistentSearchPattern";

        public static void Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull(Func<ReflectionContext, DirectoryCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("reflectionContext", () =>
            {
                var catalog = catalogCreator(null);
            });
        }

        public static void Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull(Func<ICompositionElement, DirectoryCatalog> catalogCreator)
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definitionOrigin", () =>
            {
                var catalog = catalogCreator(null);
            });
        }

        [TestMethod]
        public void Constructor2_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            DirectoryCatalogTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new DirectoryCatalog(FileIO.GetNewTemporaryDirectory(), rc);
            });
        }

        [TestMethod]
        public void Constructor3_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            DirectoryCatalogTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new DirectoryCatalog(FileIO.GetNewTemporaryDirectory(), dO);
            });
        }

        [TestMethod]
        public void Constructor4_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            DirectoryCatalogTests.Constructor_NullReflectionContextArgument_ShouldThrowArgumentNull((rc) =>
            {
                return new DirectoryCatalog(FileIO.GetNewTemporaryDirectory(), rc, CreateDirectoryCatalog());
            });
        }

        [TestMethod]
        public void Constructor4_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            DirectoryCatalogTests.Constructor_NullDefinitionOriginArgument_ShouldThrowArgumentNull((dO) =>
            {
                return new DirectoryCatalog(FileIO.GetNewTemporaryDirectory(), new DirectoryCatalogTestsReflectionContext(), dO);
            });
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldIncludeCatalogTypeNameAndDirectoryPath()
        {
            var paths = GetPathExpectations();

            foreach (var path in paths)
            {
                var catalog = (ICompositionElement)CreateDirectoryCatalog(path, NonExistentSearchPattern);

                string expected = string.Format("DirectoryCatalog (Path=\"{0}\")", path);

                Assert.AreEqual(expected, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldIncludeDerivedCatalogTypeNameAndAssemblyFullName()
        {
            var paths = GetPathExpectations();

            foreach (var path in paths)
            {
                var catalog = (ICompositionElement)new DerivedDirectoryCatalog(path, NonExistentSearchPattern);

                string expected = string.Format("DerivedDirectoryCatalog (Path=\"{0}\")", path);

                Assert.AreEqual(expected, catalog.DisplayName);
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnICompositionElementDisplayName()
        {
            var paths = GetPathExpectations();

            foreach (var path in paths)
            {
                var catalog = (ICompositionElement)CreateDirectoryCatalog(path, NonExistentSearchPattern);

                Assert.AreEqual(catalog.DisplayName, catalog.ToString());
            }
        }

        [TestMethod]
        public void ICompositionElementDisplayName_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();

            var displayName = ((ICompositionElement)catalog).DisplayName;
        }

        [TestMethod]
        public void ICompositionElementOrigin_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();

            var origin = ((ICompositionElement)catalog).Origin;
        }

        [TestMethod]
        public void Parts_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var parts = catalog.Parts;
            });
        }

        [TestMethod]
        public void GetExports_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();
            var definition = ImportDefinitionFactory.Create();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                catalog.GetExports(definition);
            });
        }

        [TestMethod]
        public void Refresh_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                catalog.Refresh();
            });
        }

        [TestMethod]
        public void ToString_WhenCatalogDisposed_ShouldNotThrow()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();

            catalog.ToString();
        }

        [TestMethod]
        public void GetExports_NullAsConstraintArgument_ShouldThrowArgumentNull()
        {
            var catalog = CreateDirectoryCatalog();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                catalog.GetExports((ImportDefinition)null);
            });
        }


        [TestMethod]
        public void Dispose_ShouldNotThrow()
        {
            using (var catalog = CreateDirectoryCatalog())
            {
            }
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var catalog = CreateDirectoryCatalog();
            catalog.Dispose();
            catalog.Dispose();
            catalog.Dispose();
        }


        [TestMethod]
        public void AddAssembly1_NonExistentUriAsAssemblyFileNameArgument_ShouldNotSupportedException()
        {
            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                var catalog = new DirectoryCatalog("http://microsoft.com/myassembly.dll");
            });
        }

        [TestMethod]
        public void AddAssembly1_NullPathArgument_ShouldThrowArugmentNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
                new DirectoryCatalog((string)null));
        }

        [TestMethod]
        public void AddAssembly1_EmptyPathArgument_ShouldThrowArugment()
        {
            ExceptionAssert.Throws<ArgumentException>(() =>
                new DirectoryCatalog(""));
        }

        [TestMethod]
        public void AddAssembly1_InvalidPathName_ShouldThrowDirectoryNotFound()
        {
            ExceptionAssert.Throws<ArgumentException>(() =>
            {
                var c1 = new DirectoryCatalog("*");
            });
        }

        [TestMethod]
        public void AddAssembly1_TooLongPathNameArgument_ShouldThrowPathTooLongException()
        {
            ExceptionAssert.Throws<PathTooLongException>(() =>
            {
                var c1 = new DirectoryCatalog(@"c:\This is a very long path\And Just to make sure\We will continue to make it very long\This is a very long path\And Just to make sure\We will continue to make it very long\This is a very long path\And Just to make sure\We will continue to make it very long\myassembly.dll");
            });
        }


        [TestMethod]
        public void Parts()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var catalog = new DirectoryCatalog(directory.DirectoryPath);
                Assert.IsNotNull(catalog.Parts);
                Assert.IsTrue(catalog.Parts.Count() > 0);
            }
        }

        [TestMethod]
        public void Parts_ShouldSetDefinitionOriginToCatalogItself()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var catalog = new DirectoryCatalog(directory.DirectoryPath);
                Assert.IsTrue(catalog.Parts.Count() > 0);

                foreach (ICompositionElement definition in catalog.Parts)
                {
                    Assert.AreSame(catalog, definition.Origin);
                }
            }
        }

        [TestMethod]
        public void Path_ValidPath_ShouldBeFine()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var expectations = new ExpectationCollection<string, string>();

                expectations.Add(".", ".");
                expectations.Add(FileIO.RootTemporaryDirectoryName, FileIO.RootTemporaryDirectoryName);
                expectations.Add(FileIO.GetRootTemporaryDirectory(), FileIO.GetRootTemporaryDirectory());
                expectations.Add(directory.DirectoryPath, directory.DirectoryPath);

                foreach (var e in expectations)
                {
                    var cat = CreateDirectoryCatalog(e.Input, NonExistentSearchPattern);

                    Assert.AreEqual(e.Output, cat.Path);
                }
            }
        }

        [TestMethod]
        public void FullPath_ValidPath_ShouldBeFine()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var expectations = new ExpectationCollection<string, string>();

                // Ensure the path is always normalized properly.
                string rootTempPath = Path.GetFullPath(FileIO.GetRootTemporaryDirectory()).ToUpperInvariant();

                // Note: These relative paths work properly because the unit test temporary directories are always
                // created as a subfolder off the AppDomain.CurrentDomain.BaseDirectory.
                expectations.Add(".", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".")).ToUpperInvariant());
                expectations.Add(FileIO.RootTemporaryDirectoryName, rootTempPath);
                expectations.Add(FileIO.GetRootTemporaryDirectory(), rootTempPath);
                expectations.Add(directory.DirectoryPath, Path.GetFullPath(directory.DirectoryPath).ToUpperInvariant());

                foreach (var e in expectations)
                {
                    var cat = CreateDirectoryCatalog(e.Input, NonExistentSearchPattern);

                    Assert.AreEqual(e.Output, cat.FullPath);
                }
            }
        }

        [TestMethod]
        public void LoadedFiles_EmptyDirectory_ShouldBeFine()
        {
            using (var directory = new TemporaryDirectory())
            {
                var cat = new DirectoryCatalog(directory.DirectoryPath);

                Assert.AreEqual(0, cat.LoadedFiles.Count);
            }
        }

        [TestMethod]
        public void LoadedFiles_ContainsMultipleDllsAndSomeNonDll_ShouldOnlyContainDlls()
        {
            using (var directory = new TemporaryDirectory())
            {
                // Add one text file
                using (File.CreateText(Path.Combine(directory.DirectoryPath, "Test.txt"))) { }

                // Add two dll's
                string dll1 = Path.Combine(directory.DirectoryPath, "Test1.dll");
                string dll2 = Path.Combine(directory.DirectoryPath, "Test2.dll");
                File.Copy(Assembly.GetExecutingAssembly().Location, dll1);
                File.Copy(Assembly.GetExecutingAssembly().Location, dll2);

                var cat = new DirectoryCatalog(directory.DirectoryPath);

                CollectionAssert.AreEquivalent(new string[] { dll1.ToUpperInvariant(), dll2.ToUpperInvariant() },
                    cat.LoadedFiles);
            }
        }

        [TestMethod]
        public void Constructor_InvalidAssembly_ShouldBeFine()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                using (File.CreateText(Path.Combine(directory.DirectoryPath, "Test.dll"))) { }
                var cat = new DirectoryCatalog(directory.DirectoryPath);
            }
        }

        [TestMethod]
        public void Constructor_NonExistentDirectory_ShouldThrow()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                ExceptionAssert.Throws<DirectoryNotFoundException>( () =>
                    new DirectoryCatalog(directory.DirectoryPath + @"\NonexistentDirectoryWithoutEndingSlash"));

                ExceptionAssert.Throws<DirectoryNotFoundException>( () =>
                    new DirectoryCatalog(directory.DirectoryPath + @"\NonexistentDirectoryWithEndingSlash\"));

            }
        }

        [TestMethod]
        public void Constructor_PassExistingFileName_ShouldThrow()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                using (File.CreateText(Path.Combine(directory.DirectoryPath, "Test.txt"))) { }
                ExceptionAssert.Throws<IOException>(() =>
                    new DirectoryCatalog(Path.Combine(directory.DirectoryPath, "Test.txt")));
            }
        }

        [TestMethod]
        public void Constructor_PassNonExistingFileName_ShouldThrow()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                ExceptionAssert.Throws<DirectoryNotFoundException>(() =>
                    new DirectoryCatalog(Path.Combine(directory.DirectoryPath, "NonExistingFile.txt")));
            }
        }

        [TestMethod]
        public void Refresh_AssemblyAdded_ShouldFireOnChanged()
        {
            using (var directory = new TemporaryDirectory())
            {
                bool changedFired = false;
                bool changingFired = false;
                var cat = new DirectoryCatalog(directory.DirectoryPath);

                Assert.AreEqual(0, cat.Parts.Count(), "Catalog should initially be empty");

                cat.Changing += new EventHandler<ComposablePartCatalogChangeEventArgs>((o, e) =>
                    {
                        Assert.AreEqual(0, cat.Parts.Count(), "Catalog changes should NOT have been completeed yet");
                        changingFired = true;
                    });

                cat.Changed += new EventHandler<ComposablePartCatalogChangeEventArgs>((o, e) =>
                    {
                        Assert.AreNotEqual(0, cat.Parts.Count(), "Catalog changes should have been completeed");
                        changedFired = true;
                    });


                File.Copy(Assembly.GetExecutingAssembly().Location, Path.Combine(directory.DirectoryPath, "Test.dll"));

                cat.Refresh();

                Assert.IsTrue(changingFired);
                Assert.IsTrue(changedFired);
            }
        }

        [TestMethod]
        public void Refresh_AssemblyRemoved_ShouldFireOnChanged()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                string file = Path.Combine(directory.DirectoryPath, "Test.dll");
                File.Copy(Assembly.GetExecutingAssembly().Location, file);
                bool changedFired = false;
                var cat = new DirectoryCatalog(directory.DirectoryPath);

                cat.Changed += new EventHandler<ComposablePartCatalogChangeEventArgs>((o, e) =>
                    changedFired = true);

                // This assembly can be deleted because it was already loaded by the CLR in another context
                // in another location so it isn't locked on disk.
                File.Delete(file);

                cat.Refresh();

                Assert.IsTrue(changedFired);
            }
        }

        [TestMethod]
        public void Refresh_NoChanges_ShouldNotFireOnChanged()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var cat = new DirectoryCatalog(directory.DirectoryPath);

                cat.Changed += new EventHandler<ComposablePartCatalogChangeEventArgs>((o, e) =>
                    Assert.Fail("Should not recieve any change notifications"));

                cat.Refresh();
            }
        }

        [TestMethod]
        public void Refresh_DirectoryRemoved_ShouldThrowDirectoryNotFound()
        {
            DirectoryCatalog cat;
            using (var directory = CreateTemporaryDirectory())
            {
                cat = new DirectoryCatalog(directory.DirectoryPath);
            }

            ExceptionAssert.Throws<DirectoryNotFoundException>(RetryMode.DoNotRetry, () =>
                cat.Refresh());
        }

        [TestMethod]
        public void GetExports()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var catalog = new AggregateCatalog();
                Expression<Func<ExportDefinition, bool>> constraint = (ExportDefinition exportDefinition) => exportDefinition.ContractName == AttributedModelServices.GetContractName(typeof(MyExport));
                IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> matchingExports = null;
    
                matchingExports = catalog.GetExports(constraint);
                Assert.IsNotNull(matchingExports);
                Assert.IsTrue(matchingExports.Count() == 0);

                var testsDirectoryCatalog = new DirectoryCatalog(directory.DirectoryPath);
                catalog.Catalogs.Add(testsDirectoryCatalog);
                matchingExports = catalog.GetExports(constraint);

                Assert.IsNotNull(matchingExports);
                Assert.IsTrue(matchingExports.Count() >= 0);

                IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> expectedMatchingExports = catalog.Parts
                    .SelectMany(part => part.ExportDefinitions, (part, export) => new Tuple<ComposablePartDefinition, ExportDefinition>(part, export))
                    .Where(partAndExport => partAndExport.Item2.ContractName == AttributedModelServices.GetContractName(typeof(MyExport)));

                Assert.IsTrue(matchingExports.SequenceEqual(expectedMatchingExports));

                catalog.Catalogs.Remove(testsDirectoryCatalog);
                matchingExports = catalog.GetExports(constraint);
                Assert.IsNotNull(matchingExports);
                Assert.IsTrue(matchingExports.Count() == 0);
            }
        }

        [TestMethod]
        public void AddAndRemoveDirectory()
        {
            using (var directory = CreateTemporaryDirectory())
            {
                var cat = new AggregateCatalog();
                var container = new CompositionContainer(cat);

                Assert.IsFalse(container.IsPresent<MyExport>());

                var dir1 = new DirectoryCatalog(directory.DirectoryPath);
                cat.Catalogs.Add(dir1);
                Assert.IsTrue(container.IsPresent<MyExport>());

                cat.Catalogs.Remove(dir1);

                Assert.IsFalse(container.IsPresent<MyExport>());
            }
        }

        [TestMethod]
        public void AddDirectoryNotFoundException()
        {
            ExceptionAssert.Throws<DirectoryNotFoundException>(() =>
            {
                var cat = new DirectoryCatalog("Directory That Should Never Exist tadfasdfasdfsdf");
            });
        }

        [TestMethod]
        public void ExecuteOnCreationThread()
        {
            // Add a proper test for event notification on caller thread
        }


        private DirectoryCatalog CreateDirectoryCatalog()
        {
            return CreateDirectoryCatalog(FileIO.GetNewTemporaryDirectory());
        }

        private DirectoryCatalog CreateDirectoryCatalog(string path)
        {
            return new DirectoryCatalog(path);
        }

        private DirectoryCatalog CreateDirectoryCatalog(string path, string searchPattern)
        {
            return new DirectoryCatalog(path, searchPattern);
        }

        private TemporaryDirectory CreateTemporaryDirectory()
        {
            return new TemporaryFileCopier(typeof(DirectoryCatalogTests).Assembly.Location);
        }

        public IEnumerable<string> GetPathExpectations()
        {
            yield return AppDomain.CurrentDomain.BaseDirectory;
            yield return AppDomain.CurrentDomain.BaseDirectory + @"\";
            yield return ".";            
        }

        private class DerivedDirectoryCatalog : DirectoryCatalog
        {
            public DerivedDirectoryCatalog(string path, string searchPattern)
                : base(path, searchPattern)
            {
            }
        }
    }
}
