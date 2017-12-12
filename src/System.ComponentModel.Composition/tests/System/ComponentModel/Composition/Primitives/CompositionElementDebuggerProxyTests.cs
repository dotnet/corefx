// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Primitives;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition.Hosting
{
    [TestClass]
    public class CompositionElementDebuggerProxyTests
    {
        [TestMethod]
        public void Constructor_NullAsElementArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("element", () =>
            {
                new CompositionElementDebuggerProxy((CompositionElement)null);
            });
        }

        [TestMethod]
        public void Constructor_ValueAsElementArgument_ShouldSetDisplayNameProperty()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var element = CreateCompositionElement(e);

                var proxy = new CompositionElementDebuggerProxy(element);

                Assert.AreSame(element.DisplayName, proxy.DisplayName);
            }            
        }

        [TestMethod]
        public void Constructor_ValueAsElementArgument_ShouldSetOriginProperty()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var element = CreateCompositionElement(e);

                var proxy = new CompositionElementDebuggerProxy(element);

                Assert.AreSame(element.Origin, proxy.Origin);
            }
        }

        [TestMethod]
        public void Constructor_ValueAsElementArgument_ShouldSetUnderlyingObjectProperty()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var element = CreateCompositionElement(e);

                var proxy = new CompositionElementDebuggerProxy(element);

                Assert.AreSame(element.UnderlyingObject, proxy.UnderlyingObject);
            }
        }

        private static CompositionElement CreateCompositionElement(object underlyingObject)
        {
            return new CompositionElement(underlyingObject);
        }
   }
}
