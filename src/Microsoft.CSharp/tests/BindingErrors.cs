// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define TEST_DEFINITION

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class BindingErrors
    {


        private class TypeWithConditional
        {
            [Conditional("TEST_DEFINITION")]
            public virtual void DoNothing()
            {
            }
        }

        private class DerivedTypeWithConditional : TypeWithConditional
        {
            public override void DoNothing()
            {
            }
        }

        [Fact]
        public void CannotBindToConditional()
        {
            var obj = new TypeWithConditional();
            obj.DoNothing();
            dynamic d = obj;
            Assert.Throws<RuntimeBinderException>(() => d.DoNothing());
        }

        [Fact]
        public void CannotBindToOverriddenConditional()
        {
            var obj = new DerivedTypeWithConditional();
            obj.DoNothing();
            dynamic d = obj;
            Assert.Throws<RuntimeBinderException>(() => d.DoNothing());
        }
    }
}
