// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Factories;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionErrorDebuggerProxyTests
    {
        [TestMethod]
        public void Constructor_NullAsErrorArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("error", () =>
            {
                new CompositionErrorDebuggerProxy((CompositionError)null);
            });
        }

        [TestMethod]
        public void Constructor_ValueAsErrorArgument_ShouldSetExceptionProperty()
        {
            var expectations = Expectations.GetInnerExceptionsWithNull();

            foreach (var e in expectations)
            {
                var error = ErrorFactory.Create(e);

                var proxy = new CompositionErrorDebuggerProxy(error);

                Assert.AreSame(error.Exception, proxy.Exception);
            }            
        }

        [TestMethod]
        public void Constructor_ValueAsErrorArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var error = ErrorFactory.Create(e);

                var proxy = new CompositionErrorDebuggerProxy(error);

                Assert.AreSame(error.Description, proxy.Description);
            }
        }

        [TestMethod]
        public void Constructor_ValueAsErrorArgument_ShouldSetElementProperty()
        {
            var expectations = Expectations.GetCompositionElementsWithNull();

            foreach (var e in expectations)
            {
                var error = ErrorFactory.Create(e);

                var proxy = new CompositionErrorDebuggerProxy(error);

                Assert.AreSame(error.Element, proxy.Element);
            }
        }

   }
}
