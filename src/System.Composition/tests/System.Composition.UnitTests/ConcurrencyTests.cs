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
namespace System.Composition.UnitTests
{
    [TestClass]
    public class ConcurrencyTests : ContainerTests
    {
        [Export, Shared]
        public class PausesDuringActivation
        {
            public bool IsActivationComplete { get; set; }

            [OnImportsSatisfied]
            public void OnImportsSatisfied()
            {
                Task.Delay(200).Wait();
                IsActivationComplete = true;
            }
        }

        // This does not test the desired behaviour deterministically,
        // but is close enough to be repeatable at least on my machine :)
        [TestMethod]
        public void SharedInstancesAreNotVisibleUntilActivationCompletes()
        {
            var c = CreateContainer(typeof(PausesDuringActivation));
            Task.Run(() => c.GetExport<PausesDuringActivation>());
            Task.Delay(50).Wait();
            var pda = c.GetExport<PausesDuringActivation>();
            Assert.IsTrue(pda.IsActivationComplete);
        }
    }
}
