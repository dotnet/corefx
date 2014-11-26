// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif PORTABLE_TESTS
using Microsoft.Bcl.Testing;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using System.Composition.Hosting;

namespace System.Composition.UnitTests
{
    [Export]
    public class Imported { }

    [Export]
    public class TracksImportSatisfaction
    {
        [Import]
        public Imported Imported { get; set; }

        public Imported SetOnImportsSatisfied { get; set; }

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            SetOnImportsSatisfied = Imported;
        }
    }

    [TestClass]
    public class ActivationEventOrderingTests : ContainerTests
    {
        [TestMethod]
        public void OnImportsSatisfiedIsCalledAfterPropertyInjection()
        {
            var cc = CreateContainer(typeof(TracksImportSatisfaction), typeof(Imported));

            var tis = cc.GetExport<TracksImportSatisfaction>();

            Assert.IsNotNull(tis.SetOnImportsSatisfied);
        }
    }
}
