// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.Linq;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionServiceExtensionsTests
    {
        [TestMethod]
        public void SatisfyImports_BooleanOverride_PartAndFalseHaveBeenPassed()
        {
            MockCompositionService compositionService = new MockCompositionService();
            ComposablePart part = PartFactory.Create();

            bool importsSatisfiedCalled = false;
            compositionService.ImportsSatisfied += delegate(object sender, SatisfyImportsEventArgs e)
            {
                Assert.IsFalse(importsSatisfiedCalled);
                Assert.AreEqual(part, e.Part);
                importsSatisfiedCalled = true;
            };

            compositionService.SatisfyImportsOnce(part);
            Assert.IsTrue(importsSatisfiedCalled);
        }


        [TestMethod]
        public void SatisfyImports_AttributedOverride_NullAsCompositionService()
        {
            ICompositionService compositionService = null;
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("compositionService", () =>
            {
                compositionService.SatisfyImportsOnce(new MockAttributedPart());
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedOverride_NullAsAttributedPart()
        {
            MockCompositionService compositionService = new MockCompositionService();
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("attributedPart", () =>
            {
                compositionService.SatisfyImportsOnce((object)null);
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedOverride_PartAndFalseHaveBeenPassed()
        {
            MockCompositionService compositionService = new MockCompositionService();
            object attributedPart = new MockAttributedPart();

            bool importsSatisfiedCalled = false;
            compositionService.ImportsSatisfied += delegate(object sender, SatisfyImportsEventArgs e)
            {
                Assert.IsFalse(importsSatisfiedCalled);
                Assert.IsTrue(e.Part is ReflectionComposablePart);
                Assert.IsTrue(((ReflectionComposablePart)e.Part).Definition.GetPartType() == typeof(MockAttributedPart));
                importsSatisfiedCalled = true;
            };

            compositionService.SatisfyImportsOnce(attributedPart);
            Assert.IsTrue(importsSatisfiedCalled);
        }

        [TestMethod]
        public void SatisfyImports_AttributedAndBooleanOverride_NullAsCompositionService()
        {
            ICompositionService compositionService = null;
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("compositionService", () =>
            {
                compositionService.SatisfyImportsOnce(new MockAttributedPart());
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedAndBooleanOverride_NullAsAttributedPart()
        {
            MockCompositionService compositionService = new MockCompositionService();
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("attributedPart", () =>
            {
                compositionService.SatisfyImportsOnce((object)null);
            });
        }

        [TestMethod]
        public void SatisfyImports_AttributedAndBooleanOverride_PartAndFalseHaveBeenPassed()
        {
            MockCompositionService compositionService = new MockCompositionService();
            object attributedPart = new MockAttributedPart();

            bool importsSatisfiedCalled = false;
            compositionService.ImportsSatisfied += delegate(object sender, SatisfyImportsEventArgs e)
            {
                Assert.IsFalse(importsSatisfiedCalled);
                Assert.IsTrue(e.Part is ReflectionComposablePart);
                Assert.IsTrue(((ReflectionComposablePart)e.Part).Definition.GetPartType() == typeof(MockAttributedPart));
                importsSatisfiedCalled = true;
            };

            compositionService.SatisfyImportsOnce(attributedPart);
            Assert.IsTrue(importsSatisfiedCalled);
        }

        internal class SatisfyImportsEventArgs : EventArgs
        {
            public SatisfyImportsEventArgs(ComposablePart part)
            {
                this.Part = part;
            }

            public ComposablePart Part { get; private set; }
        }

        internal class MockCompositionService : ICompositionService
        {
            public MockCompositionService()
            {
            }

            public void SatisfyImportsOnce(ComposablePart part)
            {
                if (this.ImportsSatisfied != null)
                {
                    this.ImportsSatisfied.Invoke(this, new SatisfyImportsEventArgs(part));
                }
            }

            public event EventHandler<SatisfyImportsEventArgs> ImportsSatisfied;
        }

        public class MockAttributedPart
        {
            public MockAttributedPart()
            {
            }
        }
    }
}
