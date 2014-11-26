// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Runtime;
using Microsoft.Composition.Demos.Web.Http.UnitTests.Util;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Composition;

namespace Microsoft.Composition.Demos.Web.Http.UnitTests
{
    [TestClass]
    public class StandaloneDependencyResolverTests
    {
        StandaloneDependencyResolver CreateResolver(params Type[] parts)
        {
            var container = new ContainerConfiguration()
                .WithParts(parts)
                .CreateContainer();

            return new StandaloneDependencyResolver(container);
        }

        [Shared, Export]
        public class SharedPart { }

        [TestMethod]
        public void SharedPartsCanBeCreatedFromTheDependencyResolver()
        {
            var r = CreateResolver(typeof(SharedPart));
            var sp = r.GetService(typeof(SharedPart));
            Assert.IsInstanceOfType(sp, typeof(SharedPart));
        }

        [TestMethod]
        public void SharedPartsCanBeCreatedFromADependencyScope()
        {
            var r = CreateResolver(typeof(SharedPart));
            var s = r.BeginScope();
            var sp = s.GetService(typeof(SharedPart));
            Assert.IsInstanceOfType(sp, typeof(SharedPart));
        }

        [Export]
        public class NonSharedPart { }

        [TestMethod]
        public void NonSharedPartsCanBeCreatedFromTheDependencyResolver()
        {
            var r = CreateResolver(typeof(NonSharedPart));
            var nsp = r.GetService(typeof(NonSharedPart));
            Assert.IsInstanceOfType(nsp, typeof(NonSharedPart));
        }

        [TestMethod]
        public void NonSharedPartsCanBeCreatedFromADependencyScope()
        {
            var r = CreateResolver(typeof(NonSharedPart));
            var s = r.BeginScope();
            var nsp = s.GetService(typeof(NonSharedPart));
            Assert.IsInstanceOfType(nsp, typeof(NonSharedPart));
        }

        [Shared(Boundaries.HttpRequest), Export]
        public class RequestSharedPart { }

        [TestMethod]
        public void HttpRequestBoundedPartsCannotBeCreatedFromTheDependencyResolver()
        {
            var r = CreateResolver(typeof(RequestSharedPart));
            AssertX.Throws<CompositionFailedException>(() => r.GetService(typeof(RequestSharedPart)));
        }

        [TestMethod]
        public void ScopesImplementTheHttpRequestBoundary()
        {
            var r = CreateResolver(typeof(RequestSharedPart));
            var s = r.BeginScope();
            var rsp = s.GetService(typeof(RequestSharedPart));
            Assert.IsInstanceOfType(rsp, typeof(RequestSharedPart));
        }

        [Shared(Boundaries.DataConsistency), Export]
        public class DataConsistencySharedPart { }

        [TestMethod]
        public void ScopesImplementTheDataConsistencyBoundary()
        {
            var r = CreateResolver(typeof(DataConsistencySharedPart));
            var s = r.BeginScope();
            var rsp = s.GetService(typeof(DataConsistencySharedPart));
            Assert.IsInstanceOfType(rsp, typeof(DataConsistencySharedPart));
        }

        [Shared(Boundaries.UserIdentity), Export]
        public class UserIdentitySharedPart { }

        [TestMethod]
        public void ScopesImplementTheUserIdentityBoundary()
        {
            var r = CreateResolver(typeof(UserIdentitySharedPart));
            var s = r.BeginScope();
            var rsp = s.GetService(typeof(UserIdentitySharedPart));
            Assert.IsInstanceOfType(rsp, typeof(UserIdentitySharedPart));
        }

        [Export]
        public class TracksDisposal : IDisposable
        {
            public bool IsDisposed { get; set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }


        [TestMethod]
        public void DisposingTheDependencyResolverDisposesTheContainer()
        {
            var r = CreateResolver(typeof(TracksDisposal));
            var td = (TracksDisposal)r.GetService(typeof(TracksDisposal));
            r.Dispose();
            Assert.IsTrue(td.IsDisposed);
        }

        [TestMethod]
        public void DisposingADependencyScopeReleasesPerRequestParts()
        {
            var r = CreateResolver(typeof(TracksDisposal));
            var s = r.BeginScope();
            var td = (TracksDisposal)s.GetService(typeof(TracksDisposal));
            s.Dispose();
            Assert.IsTrue(td.IsDisposed);
        }

        public interface IService { }

        [Export(typeof(IService))]
        public class ImplementationA : IService { }

        [Export(typeof(IService))]
        public class ImplementationB : IService { }

        [TestMethod]
        public void AllInstancesOfAServiceCanBeRequestedFromTheResolver()
        {
            var r = CreateResolver(typeof(ImplementationA), typeof(ImplementationB));
            var impls = r.GetServices(typeof(IService)).Cast<IService>();
            Assert.AreEqual(2, impls.Count());
            Assert.AreEqual(1, impls.OfType<ImplementationA>().Count());
            Assert.AreEqual(1, impls.OfType<ImplementationB>().Count());
        }

        [TestMethod]
        public void WhenNoInstancesAreAvailableGetServicesReturnsAnEmptyEnumerable()
        {
            var r = CreateResolver();
            var impls = r.GetServices(typeof(IService));
            Assert.IsNotNull(impls);
            Assert.AreEqual(0, impls.Count());
        }

        [TestMethod]
        public void WhenAServiceIsMissingGetServiceReturnsNull()
        {
            var r = CreateResolver();
            var s = r.GetService(typeof(IService));
            Assert.IsNull(s);
        }
    }
}
