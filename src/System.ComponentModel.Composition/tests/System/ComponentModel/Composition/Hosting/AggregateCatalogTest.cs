// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.Security;
using System.Security.Permissions;
using System.Reflection;

namespace System.ComponentModel.Composition.Hosting
{
    [TestClass]
    public class AggregateCatalogTest
    {
#if FEATURE_APPDOMAINCONTROL
        public delegate bool Work();

        public class Worker : MarshalByRefObject
        {
            public static ExpectationCollection<IEnumerable<CompositionError>, string> expectations = new ExpectationCollection<IEnumerable<CompositionError>, string>();
            static Worker()
            {
            }

            public Work Action;

            internal bool DoWork()
            {
                return Action();
            }
        }
#endif //FEATURE_APPDOMAINCONTROL

        [TestMethod]
        public void Constructor1_ShouldNotThrow()
        {
            new AggregateCatalog();
        }


        [TestMethod]
        public void Constructor1_ShouldSetCatalogsPropertyToEmpty()
        {
            var catalog = new AggregateCatalog();

            EnumerableAssert.IsEmpty(catalog.Catalogs);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void Constructor1_ShouldSetPartsPropertyToEmpty()
        {
            var catalog = new AggregateCatalog();

            EnumerableAssert.IsEmpty(catalog.Parts);
        }

        [TestMethod]
        public void Constructor3_NullAsCatalogsArgument_ShouldSetCatalogsPropertyToEmpty()
        {
            var catalog = new AggregateCatalog((IEnumerable<ComposablePartCatalog>)null);

            EnumerableAssert.IsEmpty(catalog.Catalogs);
        }

        [TestMethod]
        public void Constructor3_EmptyIEnumerableAsCatalogsArgument_ShouldSetCatalogsPropertyToEmpty()
        {
            var catalog = new AggregateCatalog(Enumerable.Empty<ComposablePartCatalog>());

            EnumerableAssert.IsEmpty(catalog.Catalogs);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void Constructor3_NullAsCatalogsArgument_ShouldSetPartsPropertyToEmpty()
        {
            var catalog = new AggregateCatalog((IEnumerable<ComposablePartCatalog>)null);

            EnumerableAssert.IsEmpty(catalog.Parts);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void Constructor3_EmptyIEnumerableAsCatalogsArgument_ShouldSetPartsPropertyToEmpty()
        {
            var catalog = new AggregateCatalog(Enumerable.Empty<ComposablePartCatalog>());

            EnumerableAssert.IsEmpty(catalog.Parts);
        }

        [TestMethod]
        public void Constructor3_ArrayWithNullAsCatalogsArgument_ShouldThrowArgument()
        {
            var catalogs = new ComposablePartCatalog[] { null };

            ExceptionAssert.ThrowsArgument<ArgumentException>("catalogs", () =>
            {
                new AggregateCatalog(catalogs);
            });
        }

        [TestMethod]
        public void Catalogs_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateAggregateCatalog();
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var catalogs = catalog.Catalogs;
            });
        }

        [TestMethod]
        public void Parts_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateAggregateCatalog();
            catalog.Dispose();

            ExceptionAssert.ThrowsDisposed(catalog, () =>
            {
                var parts = catalog.Parts;
            });
        }

        [TestMethod]
        public void GetExports_WhenCatalogDisposed_ShouldThrowObjectDisposed()
        {
            var catalog = CreateAggregateCatalog();
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
            var catalog = CreateAggregateCatalog();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("definition", () =>
            {
                catalog.GetExports((ImportDefinition)null);
            });
        }

        [TestMethod]
        public void Dispose_ShouldNotThrow()
        {
            using (var catalog = CreateAggregateCatalog())
            {
            }
        }

        [TestMethod]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var catalog = CreateAggregateCatalog();
            catalog.Dispose();
            catalog.Dispose();
            catalog.Dispose();
        }

        [TestMethod]
        public void EnumeratePartsProperty_ShouldSucceed()
        {
            using (var catalog = new AggregateCatalog(
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff))))
            {
                Assert.IsTrue(catalog.Catalogs.Count() == 6, "Initialise AggregateCatalog gets right number of catalogs");
                Assert.IsTrue(catalog.Parts.Count() == 6, "Initialise AggregateCatalog gets right number of catalogs");
            }
       }

#if FEATURE_APPDOMAINCONTROL
        [TestMethod]
        public void EnumeratePartsPropertyPartialInTrust_ShouldSucceed()
        {
            PermissionSet ps = new PermissionSet(PermissionState.None);
            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            ps.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            //Create a new sandboxed domain 
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            setup.ApplicationBase = Path.GetDirectoryName(typeof(CompositionExceptionTests).Assembly.Location);
            AppDomain newDomain = AppDomain.CreateDomain("test domain", null, setup, ps);

            Worker remoteWorker = (Worker)newDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(Worker).FullName);

            remoteWorker.Action = () => 
            {
                using (var catalog = new AggregateCatalog(
                    new TypeCatalog(typeof(SharedPartStuff)),
                    new TypeCatalog(typeof(SharedPartStuff)),
                    new TypeCatalog(typeof(SharedPartStuff)),
                    new TypeCatalog(typeof(SharedPartStuff)),
                    new TypeCatalog(typeof(SharedPartStuff)),
                    new TypeCatalog(typeof(SharedPartStuff))))
                {
                    return (catalog.Catalogs.Count() == 6) && (catalog.Parts.Count() == 6);
                }
            };
            Assert.IsTrue(remoteWorker.DoWork());
        }
#endif //FEATURE_APPDOMAINCONTROL
        [TestMethod]
        public void MutableCatalogNotifications()
        {
            int step = 0;
            int changedStep = 0;
            var catalog = new AggregateCatalog();

            var typePartCatalog = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog1 = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog2 = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog3 = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog4 = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog5 = new TypeCatalog(typeof(SharedPartStuff));

            // Smoke test on inner collection
            catalog.Catalogs.Add(typePartCatalog);
            catalog.Catalogs.Remove(typePartCatalog);
            catalog.Catalogs.Clear();
            Assert.IsTrue(catalog.Catalogs.Count == 0, "Add/Remove/Clear -- PartsAsCollection.Count is now 0");

            // Add notifications
            catalog.Changed += delegate(object source, ComposablePartCatalogChangeEventArgs args)
            {
                // Local code
                ++step; ++step;
                changedStep = step;
            };

            //Add something then verify counters
            catalog.Catalogs.Add(typePartCatalog);
            Assert.IsTrue(catalog.Catalogs.Count == 1, "Add -- Catalogs.Count is now 1");
            Assert.IsTrue(changedStep == 2, "Add -- Changed must be fired after");

            // Reset counters
            step = changedStep = 0;

            // Remove something then verify counters
            catalog.Catalogs.Remove(typePartCatalog);
            Assert.IsTrue(catalog.Catalogs.Count == 0, "Add -- Catalogs.Count is now 0");
            Assert.IsTrue(changedStep == 2, "Remove -- Changed must be fired after");


            //Now Add it back
            catalog.Catalogs.Add(typePartCatalog);
            Assert.IsTrue(catalog.Catalogs.Count == 1, "Add -- Catalogs.Count is now 1");

            step = changedStep = 0;
            // Now clear the collection and verify counters
            catalog.Catalogs.Clear();
            Assert.IsTrue(catalog.Catalogs.Count == 0, "Add -- Catalogs.Count is now 0");
            Assert.IsTrue(changedStep == 2, "Remove -- Changed must be fired after");

            // Now remove a non existent item and verify counters
            step = changedStep = 0;
            bool removed = catalog.Catalogs.Remove(typePartCatalog);
            Assert.IsTrue(removed == false, "Remove -- correctly returned false");
            Assert.IsTrue(changedStep == 0, "Remove -- Changed should not fire if nothing changed");

            // Add a bunch
            step = changedStep = 0;
            catalog.Catalogs.Add(typePartCatalog);
            Assert.IsTrue(catalog.Catalogs.Count == 1, "Add -- Catalogs.Count is now 1");
            Assert.IsTrue(changedStep == 2, "Add -- Changed must be fired after");

            catalog.Catalogs.Add(typePartCatalog1);
            Assert.IsTrue(catalog.Catalogs.Count == 2, "Add -- Catalogs.Count is now 1");
            Assert.IsTrue(changedStep == 4, "Add -- Changing must be fired after");

            catalog.Catalogs.Add(typePartCatalog2);
            catalog.Catalogs.Add(typePartCatalog3);
            catalog.Catalogs.Add(typePartCatalog4);
            catalog.Catalogs.Add(typePartCatalog5);
            Assert.IsTrue(catalog.Catalogs.Count == 6, "Add -- Catalogs.Count is now 1");
            Assert.IsTrue(changedStep == 12, "Add -- Changing must be fired after");

            removed = catalog.Catalogs.Remove(typePartCatalog3);
            Assert.IsTrue(catalog.Catalogs.Count == 5, "Add -- Catalogs.Count is now 5");
            Assert.IsTrue(removed == true, "Remove should have succeeded");
            Assert.IsTrue(changedStep == 14, "Remove -- Changed must be fired after");
            removed = catalog.Catalogs.Remove(typePartCatalog2);
            removed = catalog.Catalogs.Remove(typePartCatalog1);
            removed = catalog.Catalogs.Remove(typePartCatalog4);
            removed = catalog.Catalogs.Remove(typePartCatalog);
            removed = catalog.Catalogs.Remove(typePartCatalog5);
            Assert.IsTrue(catalog.Catalogs.Count == 0, "Add -- Catalogs.Count is now 0");
            Assert.IsTrue(removed == true, "Remove should have succeeded");
            Assert.IsTrue(changedStep == 24, "Remove -- Changing must be fired after");

            // Add and then clear a lot
            step = changedStep = 0;
            catalog.Catalogs.Add(typePartCatalog);
            catalog.Catalogs.Add(typePartCatalog1);
            catalog.Catalogs.Add(typePartCatalog2);
            catalog.Catalogs.Add(typePartCatalog3);
            catalog.Catalogs.Add(typePartCatalog4);
            catalog.Catalogs.Add(typePartCatalog5);
            Assert.IsTrue(catalog.Catalogs.Count == 6, "Add -- Catalogs.Count should be 6");
            Assert.IsTrue(changedStep == 12, "Add -- Changing must be fired after");

            catalog.Catalogs.Clear();
            Assert.IsTrue(catalog.Catalogs.Count == 0, "Add -- Catalogs.Count should be 0");

            step = changedStep = 0;
            int step2 = 100;
            int changedStep2 = 0;

            catalog.Changed += delegate(object source, ComposablePartCatalogChangeEventArgs args)
            {
                // Local code
                --step2; --step2;
                changedStep2 = step2;
            };

            catalog.Catalogs.Add(typePartCatalog);
            Assert.IsTrue(catalog.Catalogs.Count == 1, "Add -- Catalogs.Count is now 1");
            Assert.IsTrue(changedStep == 2, "Add handler 1 -- Changed must be fired after");
            Assert.IsTrue(changedStep2 == 98, "Add handler 2 -- Changed must be fired after");

            catalog.Catalogs.Add(typePartCatalog1);
            Assert.IsTrue(catalog.Catalogs.Count == 2, "Add -- Catalogs.Count is now 1");
            Assert.IsTrue(changedStep == 4, "Add handler 1 -- Changed must be fired after");
            Assert.IsTrue(changedStep2 == 96, "Add handler 2 -- Changed must be firedafter");

            catalog.Catalogs.Remove(typePartCatalog);
            Assert.IsTrue(catalog.Catalogs.Count == 1, "Add -- PartsAsCollection.Count is now 1");
            Assert.IsTrue(changedStep == 6, "Add handler 1 -- Changed must be fired and fired after");
            Assert.IsTrue(changedStep2 == 94, "Add handler 2 -- Changed must be fired and fired after");

            catalog.Catalogs.Clear();
            Assert.IsTrue(catalog.Catalogs.Count == 0, "Add -- PartsAsCollection.Count is now 0");
            Assert.IsTrue(changedStep == 8, "Add handler 1 -- Changed must be fired after");
            Assert.IsTrue(changedStep2 == 92, "Add handler 2 -- Changed must be fired after");

        }

        [TestMethod]
        public void DisposeAggregatingCatalog()
        {
            int changedNotification = 0;

            var typePartCatalog1 = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog2 = new TypeCatalog(typeof(SharedPartStuff));
            var typePartCatalog3 = new TypeCatalog(typeof(SharedPartStuff));

            var assemblyPartCatalog1 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);
            var assemblyPartCatalog2 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);
            var assemblyPartCatalog3 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);

#if FEATURE_REFLECTIONFILEIO
            var dirPartCatalog1 = new DirectoryCatalog(FileIO.GetRootTemporaryDirectory());
            var dirPartCatalog2 = new DirectoryCatalog(FileIO.GetRootTemporaryDirectory());
            var dirPartCatalog3 = new DirectoryCatalog(FileIO.GetRootTemporaryDirectory());
#endif //FEATURE_REFLECTIONFILEIO
            using (var catalog = new AggregateCatalog())
            {
                catalog.Catalogs.Add(typePartCatalog1);
                catalog.Catalogs.Add(typePartCatalog2);
                catalog.Catalogs.Add(typePartCatalog3);

                catalog.Catalogs.Add(assemblyPartCatalog1);
                catalog.Catalogs.Add(assemblyPartCatalog2);
                catalog.Catalogs.Add(assemblyPartCatalog3);

#if FEATURE_REFLECTIONFILEIO
                catalog.Catalogs.Add(dirPartCatalog1);
                catalog.Catalogs.Add(dirPartCatalog2);
                catalog.Catalogs.Add(dirPartCatalog3);
#endif //FEATURE_REFLECTIONFILEIO

                // Add notifications
                catalog.Changed += delegate(object source, ComposablePartCatalogChangeEventArgs args)
                {
                    // Local code
                    ++changedNotification;
                };

            }

            Assert.IsTrue(changedNotification == 0, "No changed notifications");

            //Ensure that the other catalogs are 
            ExceptionAssert.ThrowsDisposed(typePartCatalog1, () =>
            {
                var iEnum = typePartCatalog1.Parts.GetEnumerator();
            });

            ExceptionAssert.ThrowsDisposed(typePartCatalog2, () =>
            {
                var iEnum = typePartCatalog2.Parts.GetEnumerator();
            });

            ExceptionAssert.ThrowsDisposed(typePartCatalog3, () =>
            {
                var iEnum = typePartCatalog3.Parts.GetEnumerator();
            });

            //Ensure that the other catalogs are 
            ExceptionAssert.ThrowsDisposed(assemblyPartCatalog1, () =>
            {
                var iEnum = assemblyPartCatalog1.Parts.GetEnumerator();
            });

            ExceptionAssert.ThrowsDisposed(assemblyPartCatalog2, () =>
            {
                var iEnum = assemblyPartCatalog2.Parts.GetEnumerator();
            });

            ExceptionAssert.ThrowsDisposed(assemblyPartCatalog3, () =>
            {
                var iEnum = assemblyPartCatalog3.Parts.GetEnumerator();
            });

#if FEATURE_REFLECTIONFILEIO
            //Ensure that the other catalogs are 
            ExceptionAssert.ThrowsDisposed(dirPartCatalog1, () =>
            {
                var iEnum = dirPartCatalog1.Parts.GetEnumerator();
            });

            ExceptionAssert.ThrowsDisposed(dirPartCatalog2, () =>
            {
                var iEnum = dirPartCatalog2.Parts.GetEnumerator();
            });

            ExceptionAssert.ThrowsDisposed(dirPartCatalog3, () =>
            {
                var iEnum = dirPartCatalog3.Parts.GetEnumerator();
            });
#endif //FEATURE_REFLECTIONFILEIO
        }

        [TestMethod]
        [Ignore]
        [WorkItem(514749)]
        public void MutableMultithreadedEnumerations()
        {
            var catalog = new AggregateCatalog();

            ThreadStart func = delegate()
            {
                var typePart = new TypeCatalog(typeof(SharedPartStuff));
                var typePart1 = new TypeCatalog(typeof(SharedPartStuff));
                var typePart2 = new TypeCatalog(typeof(SharedPartStuff));
                var typePart3 = new TypeCatalog(typeof(SharedPartStuff));
                var typePart4 = new TypeCatalog(typeof(SharedPartStuff));
                var typePart5 = new TypeCatalog(typeof(SharedPartStuff));

                for (int i = 0; i < 100; i++)
                {
                    catalog.Catalogs.Add(typePart);
                    catalog.Catalogs.Add(typePart1);
                    catalog.Catalogs.Add(typePart2);
                    catalog.Catalogs.Add(typePart3);
                    catalog.Catalogs.Add(typePart4);
                    catalog.Catalogs.Add(typePart5);

                    Assert.IsTrue(catalog.Catalogs.Count >= 6, "Catalogs Collection must be at least 6 big");

                    for (int k = 0; k < 5; k++)
                    {
                        int j;
                        // Ensure that iterating the returned queryable is okay even though there are many threads mutationg it
                        // We are really just looking to ensure that ollection changed exceptions are not thrown
                        j = 0;
                        var iq = catalog.Parts.GetEnumerator();
                        while (iq.MoveNext())
                        {
                            ++j;
                        }

                        Assert.IsTrue(j >= 6, "Catalogs Collection must be at least 6 big");

                        // Ensure that iterating the returned enumerator is okay even though there are many threads mutationg it
                        // We are really just looking to ensure that collection changed exceptions are not thrown
                        j = 0;
                        var ie = catalog.Catalogs.GetEnumerator();
                        while (ie.MoveNext())
                        {
                            ++j;
                        }
                        Assert.IsTrue(j >= 6, "Catalogs Collection must be at least 6 big");
                    }


                    catalog.Catalogs.Remove(typePart);
                    catalog.Catalogs.Remove(typePart1);
                    catalog.Catalogs.Remove(typePart2);
                    catalog.Catalogs.Remove(typePart3);
                    catalog.Catalogs.Remove(typePart4);
                    catalog.Catalogs.Remove(typePart5);
                }
            };

            Thread[] threads = new Thread[100];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(func);
            }

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            Assert.IsTrue(catalog.Catalogs.Count == 0, "Collection must be empty");
        }


        public void CreateMainAndOtherChildren(
            out AggregateCatalog[] mainChildren,
            out AggregateCatalog[] otherChildren,
            out TypeCatalog[] componentCatalogs)
        {
            componentCatalogs = new TypeCatalog[] 
            {
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff)),
                new TypeCatalog(typeof(SharedPartStuff))
            };

            // Create our child catalogs
            mainChildren = new AggregateCatalog[5];
            for (int i = 0; i < mainChildren.Length; i++)
            {
                mainChildren[i] = new AggregateCatalog(componentCatalogs);
            }

            otherChildren = new AggregateCatalog[5];
            for (int i = 0; i < otherChildren.Length; i++)
            {
                otherChildren[i] = new AggregateCatalog(componentCatalogs);
            }
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void AggregatingCatalogAddAndRemoveChildren()
        {
            int changedCount = 0;
            int typesChanged = 0;
            EventHandler<ComposablePartCatalogChangeEventArgs> onChanged = delegate(object sender, ComposablePartCatalogChangeEventArgs e)
            {
                ++changedCount;
                typesChanged += e.AddedDefinitions.Concat(e.RemovedDefinitions).Count();
            };

            // Create our child catalogs
            AggregateCatalog[] mainChildren;
            AggregateCatalog[] otherChildren;
            TypeCatalog[] componentCatalogs;

            CreateMainAndOtherChildren(out mainChildren, out otherChildren, out componentCatalogs);

            var parent = new AggregateCatalog(mainChildren);
            parent.Changed += onChanged;

            for (int i = 0; i < otherChildren.Length; i++)
            {
                parent.Catalogs.Add(otherChildren[i]);
            }

            Assert.AreEqual(otherChildren.Length, changedCount);
            Assert.AreEqual(otherChildren.Length * 3, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            parent.Catalogs.Remove(otherChildren[0]);
            parent.Catalogs.Remove(otherChildren[1]);

            Assert.AreEqual(2, changedCount);
            Assert.AreEqual(2 * 3, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            parent.Catalogs.Add(otherChildren[0]);
            parent.Catalogs.Add(otherChildren[1]);

            Assert.AreEqual(2, changedCount);
            Assert.AreEqual(2 * 3, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            parent.Catalogs.Clear();
            Assert.AreEqual(1, changedCount);
            Assert.AreEqual((mainChildren.Length + otherChildren.Length) * 3, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            // These have already been removed and so I should be able remove components from them without recieving notifications
            otherChildren[0].Catalogs.Remove(componentCatalogs[0]);
            otherChildren[1].Catalogs.Remove(componentCatalogs[1]);
            Assert.AreEqual(0, changedCount);
            Assert.AreEqual(0, typesChanged);

            // These have already been Cleared and so I should be able remove components from them without recieving notifications
            otherChildren[3].Catalogs.Remove(componentCatalogs[0]);
            otherChildren[4].Catalogs.Remove(componentCatalogs[1]);
            Assert.AreEqual(0, changedCount);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void AggregatingCatalogAddAndRemoveNestedChildren()
        {
            int changedCount = 0;
            int typesChanged = 0;

            EventHandler<ComposablePartCatalogChangeEventArgs> onChanged = delegate(object sender, ComposablePartCatalogChangeEventArgs e)
            {
                ++changedCount;
                typesChanged += e.AddedDefinitions.Concat(e.RemovedDefinitions).Count();
            };

            // Create our child catalogs
            AggregateCatalog[] mainChildren;
            AggregateCatalog[] otherChildren;
            TypeCatalog[] componentCatalogs;
            CreateMainAndOtherChildren(out mainChildren, out otherChildren, out componentCatalogs);


            var parent = new AggregateCatalog(mainChildren);
            parent.Changed += onChanged;

            for (int i = 0; i < otherChildren.Length; i++)
            {
                parent.Catalogs.Add(otherChildren[i]);
            }

            Assert.AreEqual(otherChildren.Length, changedCount);
            Assert.AreEqual(otherChildren.Length * 3, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            otherChildren[0].Catalogs.Remove(componentCatalogs[0]);
            otherChildren[1].Catalogs.Remove(componentCatalogs[1]);

            Assert.AreEqual(2, changedCount);
            Assert.AreEqual(2, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            otherChildren[0].Catalogs.Add(componentCatalogs[0]);
            otherChildren[1].Catalogs.Add(componentCatalogs[1]);

            Assert.AreEqual(2, changedCount);
            Assert.AreEqual(2, typesChanged);

            changedCount = 0;
            typesChanged = 0;
            otherChildren[1].Catalogs.Clear();
            Assert.AreEqual(1, changedCount);
            Assert.AreEqual(componentCatalogs.Length, typesChanged);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void AggregatingDisposedAndNotifications()
        {
            int changedCount = 0;
            int typesChanged = 0;

            EventHandler<ComposablePartCatalogChangeEventArgs> onChanged = delegate(object sender, ComposablePartCatalogChangeEventArgs e)
            {
                ++changedCount;
                typesChanged += e.AddedDefinitions.Concat(e.RemovedDefinitions).Count();
            };

            // Create our child catalogs
            AggregateCatalog[] mainChildren;
            AggregateCatalog[] otherChildren;
            TypeCatalog[] componentCatalogs;
            CreateMainAndOtherChildren(out mainChildren, out otherChildren, out componentCatalogs);


            var parent = new AggregateCatalog(mainChildren);
            parent.Changed += onChanged;

            for (int i = 0; i < otherChildren.Length; i++)
            {
                parent.Catalogs.Add(otherChildren[i]);
            }

            Assert.AreEqual(otherChildren.Length, changedCount);
            Assert.AreEqual(otherChildren.Length * 3, typesChanged);

            changedCount = 0;
            typesChanged = 0;

            parent.Dispose();

            Assert.AreEqual(0, changedCount);
            Assert.AreEqual(0, typesChanged);

            //Ensure that the children are also disposed
            ExceptionAssert.ThrowsDisposed(otherChildren[0], () =>
            {
                otherChildren[0].Catalogs.Remove(componentCatalogs[0]);
            });

            //Ensure that the children are also disposed
            ExceptionAssert.ThrowsDisposed(otherChildren[4], () =>
            {
                otherChildren[4].Catalogs.Remove(componentCatalogs[0]);
            });

            Assert.AreEqual(0, changedCount);
            Assert.AreEqual(0, typesChanged);
        }

        [TestMethod]
        public void AggregatingCatalogParmsConstructorAggregateAggregateCatalogs()
        {
            var aggCatalog1 = new AggregateCatalog();
            var aggCatalog2 = new AggregateCatalog();
            var aggCatalog3 = new AggregateCatalog();

            // Construct with one catalog parameter
            var catalog = new AggregateCatalog(aggCatalog1);
            Assert.IsTrue(catalog.Catalogs.Count == 1);

            // Construct with two catalog parameters
            catalog = new AggregateCatalog(aggCatalog1, aggCatalog2);
            Assert.IsTrue(catalog.Catalogs.Count == 2);

            // Construct with three catalog parameters
            catalog = new AggregateCatalog(aggCatalog1, aggCatalog2, aggCatalog3);
            Assert.IsTrue(catalog.Catalogs.Count == 3);
        }


        [TestMethod]
        public void AggregatingCatalogParmsConstructorAggregateAssemblyCatalogs()
        {
            var assemblyCatalog1 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);
            var assemblyCatalog2 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);
            var assemblyCatalog3 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);

            // Construct with one catalog parameter
            var catalog = new AggregateCatalog(assemblyCatalog1);
            Assert.IsTrue(catalog.Catalogs.Count == 1);

            // Construct with two catalog parameters
            catalog = new AggregateCatalog(assemblyCatalog1, assemblyCatalog2);
            Assert.IsTrue(catalog.Catalogs.Count == 2);

            // Construct with three catalog parameters
            catalog = new AggregateCatalog(assemblyCatalog1, assemblyCatalog2, assemblyCatalog3);
            Assert.IsTrue(catalog.Catalogs.Count == 3);
        }

        [TestMethod]
        public void AggregatingCatalogParmsConstructorMixedCatalogs()
        {
            var typePartCatalog1 = new TypeCatalog(typeof(SharedPartStuff));
            var assemblyCatalog2 = new AssemblyCatalog(typeof(SharedPartStuff).Assembly);
            var typePartCatalog3 = new TypeCatalog(typeof(SharedPartStuff));

            // Construct with three catalog parameters
            var catalog = new AggregateCatalog(typePartCatalog1, assemblyCatalog2, typePartCatalog3);
            Assert.IsTrue(catalog.Catalogs.Count == 3);
        }

        [TestMethod]
        public void AggregatingCatalogRaisesChangesForCatalogsPassedToConstructor()
        {
            var subCatalog = CreateAggregateCatalog();
            var testCatalog = new AggregateCatalog(subCatalog);

            bool changedCalled = false;
            testCatalog.Changed += delegate
            {
                changedCalled = true;
            };

            subCatalog.Catalogs.Add(new TypeCatalog(typeof(SharedPartStuff)));

            Assert.IsTrue(changedCalled);
        }

        private AggregateCatalog CreateAggregateCatalog()
        {
            return new AggregateCatalog();
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void CatalogEvents_AggregateAddRemove()
        {
            var catalog = new AggregateCatalog();
            AggregateTests(catalog, catalog);
        }

        [TestMethod]
        [Ignore]
        [WorkItem(812029)]
        public void CatalogEvents_DeepAggregateAddRemove()
        {
            var deepCatalog = new AggregateCatalog();
            var midCatalog = new AggregateCatalog(new ComposablePartCatalog[] { deepCatalog });
            var topCatalog = new AggregateCatalog(new ComposablePartCatalog[] { midCatalog });
            AggregateTests(topCatalog, deepCatalog);
        }

        private void AggregateTests(AggregateCatalog watchedCatalog, AggregateCatalog modifiedCatalog)
        {
            var fooCatalog = new TypeCatalog(new Type[] { typeof(FooExporter) });
            var barCatalog = new TypeCatalog(new Type[] { typeof(BarExporter) });
            var bothCatalog = new TypeCatalog(new Type[] { typeof(FooExporter), typeof(BarExporter) });

            var catalogListener = new CatalogListener(watchedCatalog, modifiedCatalog);

            catalogListener.VerifyAdd(fooCatalog, typeof(FooExporter));
            catalogListener.VerifyAdd(barCatalog, typeof(BarExporter));
            catalogListener.VerifyRemove(fooCatalog, typeof(FooExporter));
            catalogListener.VerifyRemove(barCatalog, typeof(BarExporter));

            catalogListener.VerifyAdd(bothCatalog, typeof(FooExporter), typeof(BarExporter));
            catalogListener.VerifyClear(typeof(FooExporter), typeof(BarExporter));

            catalogListener.VerifyAdd(bothCatalog, typeof(FooExporter), typeof(BarExporter));
            catalogListener.VerifyRemove(bothCatalog, typeof(FooExporter), typeof(BarExporter));
        }


        public interface IFoo { }
        public interface IBar { }

        [Export(typeof(IFoo))]
        public class FooExporter : IFoo
        {
        }

        [Export(typeof(IBar))]
        public class BarExporter : IBar
        {
        }

        public class CatalogListener
        {
            private AggregateCatalog _watchedCatalog;
            private AggregateCatalog _modifiedCatalog;
            private string[] _expectedAdds;
            private string[] _expectedRemoves;
            private int _changedEventCount;
            private int _changingEventCount;

            public CatalogListener(AggregateCatalog watchCatalog, AggregateCatalog modifiedCatalog)
            {
                watchCatalog.Changing += OnChanging;
                watchCatalog.Changed += OnChanged;
                this._watchedCatalog = watchCatalog;
                this._modifiedCatalog = modifiedCatalog;
            }

            public void VerifyAdd(ComposablePartCatalog catalogToAdd, params Type[] expectedTypesAdded)
            {
                this._expectedAdds = GetDisplayNames(expectedTypesAdded);

                this._modifiedCatalog.Catalogs.Add(catalogToAdd);

                Assert.IsTrue(this._changingEventCount == 1, "Changing event should have been called");
                Assert.IsTrue(this._changedEventCount == 1, "Changed event should have been called");

                ResetState();
            }

            public void VerifyRemove(ComposablePartCatalog catalogToRemove, params Type[] expectedTypesRemoved)
            {
                this._expectedAdds = null;
                this._expectedRemoves = GetDisplayNames(expectedTypesRemoved);

                this._modifiedCatalog.Catalogs.Remove(catalogToRemove);

                Assert.IsTrue(this._changingEventCount == 1, "Changing event should have been called");
                Assert.IsTrue(this._changedEventCount == 1, "Changed event should have been called");

                ResetState();
            }

            public void VerifyClear(params Type[] expectedTypesRemoved)
            {
                this._expectedAdds = null;
                this._expectedRemoves = GetDisplayNames(expectedTypesRemoved);

                this._modifiedCatalog.Catalogs.Clear();

                Assert.IsTrue(this._changingEventCount == 1, "Changing event should have been called");
                Assert.IsTrue(this._changedEventCount == 1, "Changed event should have been called");

                ResetState();
            }

            public void OnChanging(object sender, ComposablePartCatalogChangeEventArgs args)
            {
                Assert.IsTrue(this._expectedAdds != null || this._expectedRemoves != null);

                if (this._expectedAdds == null)
                {
                    EnumerableAssert.IsEmpty(args.AddedDefinitions);
                }
                else
                {
                    EnumerableAssert.AreSequenceEqual(this._expectedAdds, GetDisplayNames(args.AddedDefinitions));
                }

                if (this._expectedRemoves == null)
                {
                    EnumerableAssert.IsEmpty(args.RemovedDefinitions);
                }
                else
                {
                    EnumerableAssert.AreSequenceEqual(this._expectedRemoves, GetDisplayNames(args.RemovedDefinitions));
                }

                Assert.IsFalse(ContainsChanges(), "The catalog should NOT contain the changes yet");

                this._changingEventCount++;
            }

            public void OnChanged(object sender, ComposablePartCatalogChangeEventArgs args)
            {
                Assert.IsTrue(this._expectedAdds != null || this._expectedRemoves != null);

                if (this._expectedAdds == null)
                {
                    EnumerableAssert.IsEmpty(args.AddedDefinitions);
                }
                else
                {
                    EnumerableAssert.AreSequenceEqual(this._expectedAdds, GetDisplayNames(args.AddedDefinitions));
                }

                if (this._expectedRemoves == null)
                {
                    EnumerableAssert.IsEmpty(args.RemovedDefinitions);
                }
                else
                {
                    EnumerableAssert.AreSequenceEqual(this._expectedRemoves, GetDisplayNames(args.RemovedDefinitions));
                }

                Assert.IsNull(args.AtomicComposition);
                Assert.IsTrue(ContainsChanges(), "The catalog should contain the changes");

                this._changedEventCount++;
            }

            private bool ContainsChanges()
            {
                var allParts = GetDisplayNames(this._watchedCatalog.Parts);

                if (this._expectedAdds != null)
                {
                    foreach (var add in this._expectedAdds)
                    {
                        if (!allParts.Contains(add))
                        {
                            return false;
                        }
                    }
                }

                if (this._expectedRemoves != null)
                {
                    foreach (var remove in this._expectedRemoves)
                    {
                        if (allParts.Contains(remove))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private void ResetState()
            {
                this._expectedAdds = null;
                this._expectedRemoves = null;
                this._changedEventCount = 0;
                this._changingEventCount = 0;
            }

            private static string[] GetDisplayNames(IEnumerable<ComposablePartDefinition> definitions)
            {
                return definitions.OfType<ICompositionElement>().Select(p => p.DisplayName).ToArray();
            }

            private static string[] GetDisplayNames(IEnumerable<Type> types)
            {
                return GetDisplayNames(types.Select(t => AttributedModelServices.CreatePartDefinition(t, null)));
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SharedPartStuff
        {
            Guid id = Guid.NewGuid();

            public override string ToString()
            {
                return id.ToString();
            }
        }
    }
}
