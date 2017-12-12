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
using System.Security;
using System.Security.Permissions;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace System.ComponentModel.Composition
{
#if FEATURE_APPDOMAINCONTROL

    [TestClass]
    public class ApplicationCatalogTests
    {
#if FEATURE_REFLECTIONCONTEXT
        // This is a glorious do nothing ReflectionContext
        public class ApplicationCatalogTestsReflectionContext : ReflectionContext
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
#endif

        public class Worker : MarshalByRefObject
        {
            internal void DoWork(Action work)
            {
                work();
            }
        }

        public class Application : TemporaryDirectory
        {
            public void AppMain(Action work)
            {
                ApplicationFilesCopier(
                    typeof(Application).Assembly.Location, 
                    typeof(Assert).Assembly.Location, 
                    typeof(EnumerableAssert).Assembly.Location, 
                    typeof(TransparentTestCase).Assembly.Location,
                    typeof(System.Reflection.ReflectionContext).Assembly.Location,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.ComponentModel.Composition.UnitTests.ReflectionContextTestAssemblyOne.dll"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.ComponentModel.Composition.UnitTests.ReflectionContextTestAssemblyTwo.dll"));

                PermissionSet ps = new PermissionSet(PermissionState.Unrestricted);

                //Create a new sandboxed domain 
                AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                setup.ApplicationBase = DirectoryPath;
                setup.PrivateBinPath = SubDirectoryPath;
                setup.ShadowCopyFiles = "true";

                AppDomain newDomain = AppDomain.CreateDomain("Application Domain"+ Guid.NewGuid(), null, setup, ps);
                Worker remoteWorker = (Worker)newDomain.CreateInstanceAndUnwrap(
                    Assembly.GetExecutingAssembly().FullName,
                    typeof(Worker).FullName);
                
                Exception exception = null;
                try
                {
                    remoteWorker.DoWork(work);
                }
                catch(Exception e)
                {
                    exception = e;
                }
                finally
                {
                    AppDomain.Unload(newDomain);
                }
                
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if(exception != null)
                {
                    throw exception;
                }
            }
            
            private void ApplicationFilesCopier(string application, params string[] fileNames)
            {
                Console.WriteLine("DirectoryPath: {0}", DirectoryPath);

                //Because we test a "private" version of our binary we need to copy it down too.
                string componentModelDll = typeof(ImportAttribute).Assembly.Location;
                File.Copy(componentModelDll, Path.Combine(DirectoryPath, Path.GetFileName(componentModelDll)));

                File.Copy(application, Path.Combine(DirectoryPath, Path.GetFileName(application)));

                Directory.CreateDirectory(SubDirectoryPath);
                foreach (string fileName in fileNames)
                {
                    File.Copy(fileName, Path.Combine(SubDirectoryPath, Path.GetFileName(fileName)));
                }
            }

            public string SubDirectoryPath 
            {
                get
                {
                    return Path.Combine(DirectoryPath, "AddOns");
                }
            }
        }
        

#if FEATURE_REFLECTIONCONTEXT
        [TestMethod]
        public void Constructor1_NullReflectionContextArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("reflectionContext", () =>
            {
                new ApplicationCatalog((ReflectionContext)null);
            });
        }

        [TestMethod]
        public void Constructor3_NullBothArguments_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("reflectionContext", () =>
            {
                new ApplicationCatalog((ReflectionContext)null, (ICompositionElement)null);
            });
        }
#endif //FEATURE_REFLECTIONCONTEXT

        [TestMethod]
        public void Constructor2_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definitionOrigin", () =>
            {
                new ApplicationCatalog((ICompositionElement)null);
            });
        }

        [TestMethod]
        public void Constructor3_NullDefinitionOriginArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definitionOrigin", () =>
            {
                new ApplicationCatalog((ICompositionElement)null);
            });
        }

        [TestMethod]
        public void ICompositionElementDisplayName_ShouldIncludeCatalogTypeNameAndDirectoryPath()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    var catalog = (ICompositionElement)new ApplicationCatalog();

                    string expected = string.Format("ApplicationCatalog (Path=\"{0}\") (PrivateProbingPath=\"{1}\")", AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath);
                    string result = (string)(catalog.DisplayName);
                    //Cross AppDomain AssertFailedException does not marshall
                    if(expected != result)
                    {
                        throw new Exception("Assert.AreEqual(expected, result);");
                    }
                });
            }
        }

        [TestMethod]
        public void ToString_ShouldReturnICompositionElementDisplayName()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    var catalog = (ICompositionElement)new ApplicationCatalog();
                    string expected = catalog.ToString();
                    string result = catalog.DisplayName;
                    Assert.AreEqual(expected, result);
                });
            }
        }

        [TestMethod]
        public void ICompositionElementDisplayName_WhenCatalogDisposed_ShouldNotThrow()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    var catalog = new ApplicationCatalog();
                    catalog.Dispose();
                    var displayName = ((ICompositionElement)catalog).DisplayName;
                });
            }
        }

        [TestMethod]
        public void ICompositionElementOrigin_WhenCatalogDisposed_ShouldNotThrow()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    var catalog = new ApplicationCatalog();
                    catalog.Dispose();
                    var origin = ((ICompositionElement)catalog).Origin;
                });
            }
        }

        [TestMethod]
        public void Parts_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                using(var app = new Application())
                {
                    app.AppMain( () =>
                    {
                        var catalog = new ApplicationCatalog();
                        catalog.Dispose();
                        var parts = catalog.Parts;
                    });
                }
            });
        }

        [TestMethod]
        public void GetEnumerator_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                using(var app = new Application())
                {
                    app.AppMain( () =>
                    {
                        var catalog = new ApplicationCatalog();
                        catalog.Dispose();
                        var enumerator = catalog.GetEnumerator();
                    });
                }
            });
        }

        [TestMethod]
        public void GetExports_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            ExceptionAssert.Throws<ObjectDisposedException>(RetryMode.DoNotRetry, () =>
            {
                using(var app = new Application())
                {
                    app.AppMain( () =>
                    {
                        var catalog = new ApplicationCatalog();
                        catalog.Dispose();
            
                        var definition = ImportDefinitionFactory.Create();
                        catalog.GetExports(definition);
                    });
                }
            });
        }

        [TestMethod]
        public void ToString_WhenCatalogDisposed_ShouldNotThrow()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    var catalog = new ApplicationCatalog();
                    catalog.Dispose();
                    catalog.ToString();
                });
            }
        }

        [TestMethod]
        public void GetExports_NullAsConstraintArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                using(var app = new Application())
                {
                    app.AppMain( () =>
                    {
                        var catalog = new ApplicationCatalog();
                        catalog.GetExports((ImportDefinition)null);
                    });
                }
            });
        }


        [TestMethod]
        public void Dispose_ShouldNotThrow()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    using (var catalog = new ApplicationCatalog())
                    {
                    }
                });
            }
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    var catalog = new ApplicationCatalog();
                    catalog.Dispose();
                    catalog.Dispose();
                    catalog.Dispose();
                });
            }
        }

        [TestMethod]
        public void Test_Parts()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    using(var catalog = new ApplicationCatalog())
                    {
                        var result = catalog.Parts.Count();

                        //Cross AppDomain AssertFailedException does not marshall
                        if(result < 0)
                        {
                            throw new Exception("Assert.IsTrue(result > 0);");
                        }
                    }
                });
            }
        }

        [TestMethod]
        public void Test_GetEnumerator()
        {
            using(var app = new Application())
            {
                app.AppMain( () =>
                {
                    using(var catalog = new ApplicationCatalog())
                    {
                        var result = catalog.Count();
                        //Cross AppDomain AssertFailedException does not marshall
                        if(result < 0)
                        {
                            throw new Exception("Assert.IsTrue(result > 0);");
                        }
                    }
                });
            }
        }

        [TestMethod]
        public void ExecuteOnCreationThread()
        {
            // Add a proper test for event notification on caller thread
        }
    }
#endif //FEATURE_APPDOMAINCONTROL
}
