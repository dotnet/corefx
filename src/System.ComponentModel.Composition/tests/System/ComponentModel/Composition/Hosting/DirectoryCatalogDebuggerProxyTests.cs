// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.UnitTesting;
using System.Reflection;
using Xunit;

namespace System.ComponentModel.Composition.Primitives
{
    
    public class DirectoryCatalogDebuggerProxyTests
    {
        [Fact]
        public void Constructor_NullAsCatalogArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("catalog", () =>
            {
                new DirectoryCatalog.DirectoryCatalogDebuggerProxy((DirectoryCatalog)null);
            });
        }

        [Fact]
        public void Constructor_ValueAsCatalogArgument_ShouldSetPartsProperty()
        {
            var expectations = Expectations.GetAssemblies();

            foreach (var e in expectations)
            {
                using (TemporaryFileCopier copier = new TemporaryFileCopier(e.Location))
                {
                    var catalog = CreateDirectoryCatalog(copier.DirectoryPath);

                    var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(catalog);

                    EnumerableAssert.AreSequenceEqual(catalog.Parts, proxy.Parts);
                }
            }
        }

        [Fact]
        public void Constructor_ValueAsCatalogArgument_ShouldSetAssemblyProperty()
        {
            var expectations = Expectations.GetAssemblies();

            using (TemporaryFileCopier copier = new TemporaryFileCopier(expectations.Select(assembly => assembly.Location).ToArray()))
            {
                var catalog = CreateDirectoryCatalog(copier.DirectoryPath);
                var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(catalog);

                EnumerableAssert.AreEqual(expectations, proxy.Assemblies);
            }
        }

        [Fact]
        public void Constuctor_ValueAsCatalogArgument_ShouldSetPathProperty()
        {
            string path = FileIO.GetNewTemporaryDirectory();

            var catalog = CreateDirectoryCatalog(path);
            var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(catalog);

            Assert.Equal(path, proxy.Path);
        }

        [Fact]
        public void Constuctor_ValueAsCatalogArgument_ShouldSetSearchPatternProperty()
        {
            using (TemporaryDirectory directory = new TemporaryDirectory())
            {
                var expectations = new ExpectationCollection<string, string>();

                expectations.Add("*.*", "*.*");
                expectations.Add("*.doc", "*.doc");
                expectations.Add("*.exe", "*.exe");
                expectations.Add("*.dll", "*.dll");

                foreach (var e in expectations)
                {
                    var catalog = CreateDirectoryCatalog(directory.DirectoryPath, e.Input);
                    var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(catalog);

                    Assert.Equal(e.Output, proxy.SearchPattern);
                }
            }
        }

        [Fact]
        public void FullPath_ValidPath_ShouldBeFine()
        {
            using (TemporaryDirectory directory = new TemporaryDirectory())
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
                    var cat = CreateDirectoryCatalog(e.Input, DirectoryCatalogTests.NonExistentSearchPattern);
                    var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(cat);

                    Assert.Equal(e.Output, proxy.FullPath);
                }
            }
        }

        [Fact]
        public void LoadedFiles_EmptyDirectory_ShouldBeFine()
        {
            using (var directory = new TemporaryDirectory())
            {
                var cat = CreateDirectoryCatalog(directory.DirectoryPath);
                var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(cat);

                Assert.Equal(0, proxy.LoadedFiles.Count);
            }
        }

        [Fact]
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

                var cat = CreateDirectoryCatalog(directory.DirectoryPath);
                var proxy = new DirectoryCatalog.DirectoryCatalogDebuggerProxy(cat);

                CollectionAssert.AreEquivalent(new string[] { dll1.ToUpperInvariant(), dll2.ToUpperInvariant() },
                    proxy.LoadedFiles);
            }
        }

        private DirectoryCatalog.DirectoryCatalogDebuggerProxy CreateAssemblyDebuggerProxy(DirectoryCatalog catalog)
        {
            return new DirectoryCatalog.DirectoryCatalogDebuggerProxy(catalog);
        }

        private DirectoryCatalog CreateDirectoryCatalog(string path)
        {
            return new DirectoryCatalog(path);
        }

        private DirectoryCatalog CreateDirectoryCatalog(string path, string filter)
        {
            return new DirectoryCatalog(path, filter);
        }
    }
}
